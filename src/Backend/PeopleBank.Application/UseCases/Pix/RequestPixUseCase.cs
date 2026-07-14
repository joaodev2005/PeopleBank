using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Events;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Application.UseCases.Pix;

public class RequestPixUseCase : IRequestPixUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IRedisService _redisService;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IUnitOfWork _unitOfWork;

    public RequestPixUseCase(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        IRedisService redisService,
        IKafkaProducer kafkaProducer,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _redisService = redisService;
        _kafkaProducer = kafkaProducer;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponsePixJson> Execute(RequestPixJson request)
    {
        var cachedResponse = await _redisService.GetIdempotentResponseAsync<ResponsePixJson>(request.IdempotencyKey);
        if (cachedResponse is not null)
            return cachedResponse;

        var sourceAccount = await _accountRepository.GetByIdAsync(request.SourceAccountId);
        if (sourceAccount is null)
            throw new NotFoundException(ResourceMessagesException.ACCOUNT_NOT_FOUND);

        var targetAccount = await _accountRepository.GetByPixKeyAsync(request.TargetPixKey);
        if (targetAccount is null)
            throw new NotFoundException("Destination account not found.");

        if (sourceAccount.Id == targetAccount.Id)
            throw new ErrorOnValidationException(new List<string> { "Cannot transfer to yourself." });

        if (request.Amount <= 0)
            throw new ErrorOnValidationException(new List<string> { "Amount must be greater than zero." });

        var lockAcquired = await _redisService.AcquireLockAsync(
            $"lock:account:{sourceAccount.Id}", request.Amount);
        if (!lockAcquired)
            throw new DomainException("Unable to lock funds for this transaction. Try again later.");

        try
        {
            var transaction = new Transaction(
                sourceAccount.Id,
                targetAccount.Id,
                request.Amount,
                request.IdempotencyKey,
                "Pix Transfer"
            );

            await _transactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            var pixEvent = new PixRequestedEvent
            {
                TransactionId = transaction.Id,
                SourceAccountId = sourceAccount.Id,
                TargetAccountId = targetAccount.Id,
                Amount = request.Amount,
                IdempotencyKey = request.IdempotencyKey,
                Description = "Pix Transfer",
                RequestedAt = DateTime.UtcNow
            };

            await _kafkaProducer.PublishAsync("pix-requested", transaction.Id.ToString(), pixEvent);

            var response = new ResponsePixJson
            {
                TransactionId = transaction.Id,
                Status = "Pending",
                Message = "Pix request accepted and being processed."
            };

            await _redisService.SetIdempotentResponseAsync(request.IdempotencyKey, response);

            return response;
        }
        catch
        {
            await _redisService.ReleaseLockAsync($"lock:account:{sourceAccount.Id}");
            throw;
        }
    }
}
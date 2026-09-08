using System;
using System.Threading.Tasks;
using PeopleBank.Communication.Requests;
using PeopleBank.Communication.Responses;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Enums;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Domain.Interfaces.Services;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Application.UseCases.Benefits.SpendBenefits;

public class SpendBenefitUseCase : ISpendBenefitUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBenefitWalletRepository _benefitWalletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SpendBenefitUseCase(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        IBenefitWalletRepository benefitWalletRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _benefitWalletRepository = benefitWalletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SpendBenefitResponseJson> Execute(SpendBenefitRequestJson request)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId);
        if (account == null || !account.Active)
            throw new NotFoundException("Account not found.");

        // Load wallet with benefit definition
        var wallet = await _benefitWalletRepository.GetByAccountAndCategoryAsync(request.AccountId, request.BenefitCategory);

        if (wallet == null)
            throw new NotFoundException("Benefit wallet not found or expired.");

        // Validate compatibility
        var allowed = IsEstablishmentAllowed(request.BenefitCategory, request.EstablishmentCategory);
        if (!allowed)
            throw new DomainException("Invalid establishment for benefit category.");

        if (wallet.Balance < request.Amount)
            throw new DomainException("Insufficient benefit balance.");

        // Idempotency key
        var idempotencyKey = request.IdempotencyKey
            ?? $"spend-{request.AccountId}-{request.BenefitCategory}-{request.Amount}-{request.EstablishmentCategory}-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

        var existingTx = await _transactionRepository.GetByIdempotencyKeyAsync(idempotencyKey);
        if (existingTx != null)
        {
            // Return existing transaction info
            return new SpendBenefitResponseJson
            {
                TransactionId = existingTx.Id,
                NewBalance = wallet.Balance,
                BenefitCategory = request.BenefitCategory.ToString()
            };
        }

        // Debit wallet
        wallet.Debit(request.Amount);
        _benefitWalletRepository.Update(wallet);

        // Create transaction
        var description = request.Description
            ?? $"Spend {request.BenefitCategory} at {request.EstablishmentCategory}";

        var transaction = new Transaction(
            account.Id,
            null,
            request.Amount,
            idempotencyKey,
            description);

        await _transactionRepository.AddAsync(transaction);
        transaction.MarkAsCompleted();
        account.AddTransaction(transaction);

        await _unitOfWork.SaveChangesAsync();

        return new SpendBenefitResponseJson
        {
            TransactionId = transaction.Id,
            NewBalance = wallet.Balance,
            BenefitCategory = request.BenefitCategory.ToString()
        };
    }

    private static bool IsEstablishmentAllowed(BenefitCategory benefitCategory, EstablishmentCategory establishmentCategory)
    {
        return benefitCategory switch
        {
            BenefitCategory.Meal => establishmentCategory == EstablishmentCategory.Restaurant
                                 || establishmentCategory == EstablishmentCategory.Market,
            BenefitCategory.Food => establishmentCategory == EstablishmentCategory.Grocery
                                 || establishmentCategory == EstablishmentCategory.Pharmacy,
            _ => false
        };
    }
}
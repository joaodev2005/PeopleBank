using PeopleBank.Communication.Responses;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Exception;
using PeopleBank.Exception.ExceptionBase;

namespace PeopleBank.Application.UseCases.Account;

public class GetStatementUseCase : IGetStatementUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    public GetStatementUseCase(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<ResponseStatementJson> Execute(Guid accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account is null)
            throw new NotFoundException(ResourceMessagesException.ACCOUNT_NOT_FOUND);

        var transactions = await _transactionRepository.GetByAccountAsync(accountId, 1, 50);

        var transactionItems = transactions.Select(t =>
        {
            bool isDebit;
            if (t.TargetAccountId == null)
            {
                isDebit = false;
            }
            else
            {
                isDebit = t.SourceAccountId == account.Id;
            }

            return new TransactionItemJson
            {
                TransactionId = t.Id,
                Type = isDebit ? "DEBIT" : "CREDIT",
                Amount = t.Amount,
                Counterparty = isDebit
                    ? t.TargetAccount?.Employee?.Name ?? "External"
                    : t.SourceAccount?.Employee?.Name ?? "External",
                Description = t.Description ?? string.Empty,
                CreatedAt = t.CreatedAt,
                Status = t.Status.ToString()
            };
        }).ToList();

        return new ResponseStatementJson
        {
            AccountId = account.Id,
            EmployeeName = account.Employee?.Name ?? "Unknown",
            Balance = account.Balance,
            AvailableBalance = account.AvailableBalance,
            Transactions = transactionItems
        };
    }
}
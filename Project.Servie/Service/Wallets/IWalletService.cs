using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Data.Models;

namespace Project.Service.Service.Wallets
{
    public interface IWalletService
    {
        Task<decimal> GetWalletBalanceAsync(int userId);
        Task<IEnumerable<Transaction>> GetTransactionHistoryAsync(int walletId);
        Task<bool> DepositAsync(int userId, decimal amount);
        //Task<bool> WithdrawalAsync(int walletId, decimal amount, string bank);
        Task<string> WithdrawalAsync(int walletId, decimal amount, string bank, string nameUser);
        Task ProcessQueueAsync();

        Task<Wallet> GetWalletWithTransactionsAsync(int userId);

        Task<bool> UpdateWithdrawalStatusAsync(int transactionId, string status);

        Task UpdateAllTransactionsStatusToFailedAsync();
            Task<IEnumerable<Transaction>> GetAllDepositAndWithdrawalTransactionsAsync();
    

        Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(string status);
        Task DeductBalanceAsync(int userId, decimal amount);
    }
}

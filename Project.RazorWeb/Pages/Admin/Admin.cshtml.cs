using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data.Models;
using Project.Service.Service.Wallets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.RazorWeb.Pages.Admin
{
    public class AdminModel : PageModel
    {
        private readonly IWalletService _walletService;

        public AdminModel(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public List<Transaction> InProcessingTransactions { get; set; } = new List<Transaction>();

        public async Task OnGetAsync()
        {
            // Fetch all deposit and withdrawal transactions
            var transactions = await _walletService.GetAllDepositAndWithdrawalTransactionsAsync();
            Transactions = transactions != null ? new List<Transaction>(transactions) : new List<Transaction>();

            // Fetch transactions with status "inprocessing"
            var inProcessingTransactions = await _walletService.GetTransactionsByStatusAsync("inprocessing");
            InProcessingTransactions = inProcessingTransactions != null ? new List<Transaction>(inProcessingTransactions) : new List<Transaction>();
        }
    }
}

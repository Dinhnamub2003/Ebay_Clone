using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Project.Data.Models;
using Project.Bussiness.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Project.Service.Service.Wallets
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<WalletService> _logger;
        private readonly ConcurrentQueue<Func<Task>> _transactionQueue;
        private bool _isProcessingQueue;

        public WalletService(IUnitOfWork unitOfWork, ILogger<WalletService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _transactionQueue = new ConcurrentQueue<Func<Task>>();
            _isProcessingQueue = false;
        }

        // Lấy số dư ví
        public async Task<decimal> GetWalletBalanceAsync(int userId)
        {
            var wallet = await _unitOfWork.WalletRepository.GetQuery()
                .FirstOrDefaultAsync(w => w.UserId == userId);
            return wallet?.Balance ?? 0;
        }

        // Lấy lịch sử nạp/rút tiền
        public async Task<IEnumerable<Transaction>> GetTransactionHistoryAsync(int walletId)
        {
            return await _unitOfWork.TransactionRepository.GetQuery().Include(x => x.TransactionType)
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        // Tạo đơn nạp tiền
        // Hàm nạp tiền với Queue
        public async Task<bool> DepositAsync(int userId, decimal amount)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // Đẩy yêu cầu nạp tiền vào Queue
            _transactionQueue.Enqueue(async () =>
            {
                try
                {
                    // Lấy thông tin ví của người dùng
                    var wallet = await _unitOfWork.WalletRepository.GetQuery()
                        .FirstOrDefaultAsync(w => w.UserId == userId);

                    if (wallet == null)
                    {
                        _logger.LogWarning("Wallet not found for user with ID: {UserId}", userId);
                        taskCompletionSource.SetResult(false);
                        return;
                    }

                    // Tính toán số dư mới
                    decimal balanceBefore = wallet.Balance ?? 0;
                    decimal balanceAfter = balanceBefore + amount;

                    // Cập nhật số dư trong ví
                    wallet.Balance = balanceAfter;
                    wallet.LastUpdated = DateTime.Now;
                    _unitOfWork.WalletRepository.Update(wallet);

                    // Ghi lại lịch sử giao dịch trong Transaction
                    var transaction = new Transaction
                    {
                        WalletId = wallet.WalletId,
                        TransactionTypeId = 1, // Giả sử 1 là mã cho "deposit"
                        Bank = "VnPay",
                        ImgBank = "VnPay.jpg",
                        Amount = amount,
                        BalanceBefore = balanceBefore,
                        BalanceAfter = balanceAfter,
                        TransactionDate = DateTime.Now,
                        Status = "completed",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _unitOfWork.TransactionRepository.Add(transaction);

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Deposit of {Amount} to wallet for user {UserId} successful. New balance: {Balance}",
                        amount.ToString("C", CultureInfo.CreateSpecificCulture("vi-VN")), userId, balanceAfter.ToString("C", CultureInfo.CreateSpecificCulture("vi-VN")));

                    taskCompletionSource.SetResult(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during deposit for user with ID: {UserId}", userId);
                    taskCompletionSource.SetResult(false);
                }
            });

            // Kiểm tra và xử lý Queue
            await ProcessQueueAsync();

            return await taskCompletionSource.Task;
        }









        public async Task<string> WithdrawalAsync(int walletId, decimal amount, string bank, string nameUser)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            var bankCodes = new Dictionary<string, (string BankCode, string ImgBank)>
    {
        { "http://localhost:5081/assets1/images/bidvBank.jpg", ("970418", "assets1/images/bidvBank.jpg") },
        { "http://localhost:5081/assets1/images/vcbBank.jpg", ("970436", "assets1/images/vcbBank.jpg") },
        { "http://localhost:5081/assets1/images/acb.png", ("970416", "assets1/images/acb.png") },
        { "http://localhost:5081/assets1/images/mbbank.jpg", ("970422", "assets1/images/mbbank.jpg") },
        { "http://localhost:5081/assets1/images/techcombank.jpg", ("970407", "assets1/images/techcombank.jpg") },
        { "http://localhost:5081/assets1/images/tienphongbank.webp", ("970423", "assets1/images/tienphongbank.webp") },
        { "http://localhost:5081/assets1/images/viettinBank.jpg", ("970415", "assets1/images/viettinBank.jpg") },
        { "http://localhost:5081/assets1/images/agriBank.png", ("970405", "assets1/images/agriBank.png") },
        { "http://localhost:5081/assets1/images/oceanbank.webp", ("970414", "assets1/images/oceanbank.webp") },
        { "http://localhost:5081/assets1/images/ocb.webp", ("970448", "assets1/images/ocb.webp") },
        { "http://localhost:5081/assets1/images/vpbank.png", ("970432", "assets1/images/vpbank.png") },
        { "http://localhost:5081/assets1/images/eximbank.png", ("970431", "assets1/images/eximbank.png") },
        { "http://localhost:5081/assets1/images/SacomBank.png", ("970403", "assets1/images/SacomBank.png") }
    };


            // Đẩy yêu cầu rút tiền vào Queue
            _transactionQueue.Enqueue(async () =>
            {
                try
                {
                    // Lấy thông tin ví
                    var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(walletId);
                    if (wallet == null || wallet.Balance < amount)
                    {
                        taskCompletionSource.SetResult("Insufficient balance or wallet not found.");
                        return;
                    }

                    // Tìm mã ngân hàng và hình ảnh tương ứng
                    if (!bankCodes.TryGetValue(bank.ToLower(), out var bankInfo))
                    {
                        taskCompletionSource.SetResult("Bank not recognized.");
                        return;
                    }

                    var (BankCode, ImgBank) = bankInfo;

                    // Tạo giao dịch rút tiền
                    decimal balanceBefore = wallet.Balance ?? 0;
                    decimal balanceAfter = balanceBefore - amount;

                    var transaction = new Transaction
                    {
                        WalletId = walletId,
                        TransactionTypeId = 2, // Withdrawal
                        Amount = amount,
                        Bank = BankCode,
                        ImgBank = nameUser,
                        Status = "Inprocessing",
                        TransactionDate = DateTime.Now,
                        BalanceBefore = balanceBefore,
                        BalanceAfter = balanceAfter,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    // Cập nhật số dư trong ví
                    wallet.Balance = balanceAfter;
                    wallet.LastUpdated = DateTime.Now;
                    _unitOfWork.WalletRepository.Update(wallet);

                    // Lưu giao dịch vào Transaction
                    _unitOfWork.TransactionRepository.Add(transaction);

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation("Withdrawal of {Amount} from wallet ID {WalletId} successful. New balance: {BalanceAfter}",
                        amount, walletId, balanceAfter);

                    taskCompletionSource.SetResult(null); // Thành công, không có lỗi
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during withdrawal for wallet ID: {WalletId}", walletId);
                    taskCompletionSource.SetResult($"Error: {ex.Message}");
                }
            });

            // Kiểm tra và xử lý Queue
            await ProcessQueueAsync();

            return await taskCompletionSource.Task;
        }





        // Lấy tất cả đơn nạp và rút tiền
        public async Task<IEnumerable<Transaction>> GetAllDepositAndWithdrawalTransactionsAsync()
        {

          
                return await _unitOfWork.TransactionRepository.GetQuery().Include(x => x.Wallet).ThenInclude(x => x.User)
              .Where(t => t.TransactionTypeId == 1 || t.TransactionTypeId == 2) // 1 = Deposit, 2 = Withdrawal
              .OrderByDescending(t => t.TransactionDate)
              .ToListAsync();
            
          

          
        }


        public async Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(string status)
        {
            return await _unitOfWork.TransactionRepository.GetQuery()
                .Include(x => x.Wallet)
                .ThenInclude(x => x.User)
                .Where(t => t.Status == status && t.TransactionTypeId == 2) // Filter by status and TransactionTypeId = 2 (withdrawals)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        private readonly ConcurrentQueue<Func<Task>> _refundQueue = new ConcurrentQueue<Func<Task>>();
        private bool _isProcessingRefundQueue = false;

        public async Task<bool> UpdateWithdrawalStatusAsync(int transactionId, string status)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetByIdAsync(transactionId);
            if (transaction == null || transaction.TransactionTypeId != 2) // Đảm bảo đây là giao dịch rút tiền
            {
                _logger.LogWarning("Transaction not found or not a withdrawal transaction with ID: {TransactionId}", transactionId);
                return false;
            }

            // Kiểm tra xem trạng thái hiện tại có phải "inprocessing" không
            if (transaction.Status != "inprocessing")
            {
                _logger.LogWarning("Cannot update transaction ID {TransactionId} as it is not in 'inprocessing' state. Current state: {CurrentStatus}",
                    transactionId, transaction.Status);
                return false;
            }

            // Cập nhật trạng thái giao dịch
            transaction.Status = status;
            transaction.UpdatedAt = DateTime.Now;

            // Nếu trạng thái mới là "canceled" hoặc "failed", thêm yêu cầu hoàn lại tiền vào hàng đợi
            if (status == "canceled" || status == "failed")
            {
                _refundQueue.Enqueue(async () =>
                {
                    var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(transaction.WalletId);
                    if (wallet == null)
                    {
                        _logger.LogWarning("Wallet not found for transaction with ID: {TransactionId}", transactionId);
                        return;
                    }

                    // Hoàn lại số tiền đã rút
                    wallet.Balance += transaction.Amount;
                    wallet.LastUpdated = DateTime.Now;

                    // Cập nhật ví trong database
                    _unitOfWork.WalletRepository.Update(wallet);

                    _logger.LogInformation("Transaction ID {TransactionId} was canceled. Amount {Amount} refunded to wallet ID {WalletId}. New balance: {Balance}",
                        transactionId, transaction.Amount, wallet.WalletId, wallet.Balance);

                    // Lưu giao dịch cập nhật vào database
                    await _unitOfWork.SaveChangesAsync();
                });

                // Khởi động xử lý hàng đợi nếu chưa xử lý
                await ProcessRefundQueueAsync();
            }

            // Cập nhật trạng thái giao dịch trong database
            _unitOfWork.TransactionRepository.Update(transaction);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        // Xử lý hàng đợi hoàn tiền
        private async Task ProcessRefundQueueAsync()
        {
            if (_isProcessingRefundQueue)
                return;

            _isProcessingRefundQueue = true;

            while (_refundQueue.TryDequeue(out var refundTask))
            {
                try
                {
                    await refundTask();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing refund in queue.");
                }
            }

            _isProcessingRefundQueue = false;
        }








        public async Task UpdateAllTransactionsStatusToFailedAsync()
        {
            try
            {
                // Lấy tất cả các giao dịch mà trạng thái hiện tại không phải "failed"
                var transactions = await _unitOfWork.TransactionRepository.GetQuery()
                    .Where(t => t.Status != "failed")
                    .ToListAsync();

                // Cập nhật trạng thái của từng giao dịch thành "failed"
                foreach (var transaction in transactions)
                {
                    transaction.Status = "failed";
                    transaction.UpdatedAt = DateTime.Now;
                    _unitOfWork.TransactionRepository.Update(transaction);
                }

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully updated all transactions' status to 'failed'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating all transactions' status to 'failed'.");
            }
        }




        // Xử lý hàng đợi
        public async Task ProcessQueueAsync()
        {
            while (_transactionQueue.TryDequeue(out var transactionTask))
            {
                await transactionTask();
            }
        }

        // Xử lý giao dịch với bảo mật
        private async Task ProcessTransactionAsync(Transaction transaction)
        {
            if (transaction.Status == "pending")
            {
                try
                {
                    transaction.Status = "completed";

                    // Cập nhật ví và trạng thái giao dịch
                    var wallet = await _unitOfWork.WalletRepository.GetByIdAsync(transaction.WalletId);
                    if (wallet != null)
                    {
                        wallet.Balance = transaction.BalanceAfter;
                        wallet.LastUpdated = DateTime.Now;
                        _unitOfWork.WalletRepository.Update(wallet);
                    }

                    // Lưu giao dịch vào cơ sở dữ liệu
                    _unitOfWork.TransactionRepository.Add(transaction);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    transaction.Status = "failed";
                    _logger.LogError(ex, "Error processing transaction.");
                }
            }
        }



        public async Task<Wallet> GetWalletWithTransactionsAsync(int userId)
        {
            return await _unitOfWork.WalletRepository.GetQuery()
                .Include(w => w.Transactions)
                    .ThenInclude(t => t.TransactionType)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }


    }
}

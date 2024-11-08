using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Service.Service.VnPay;
using Project.Service.Service.Wallets;
using System.Security.Claims;
using System.Transactions;

namespace Project.RazorWeb.Pages.Wallet
{
    public class WalletManagerModel : PageModel
    {



        private readonly IConfiguration _configuration;
        private readonly ILogger<DepositModel> _logger;
        private readonly IWalletService _walletService;

        public WalletManagerModel(IConfiguration configuration, ILogger<DepositModel> logger, IWalletService walletService)
        {
            _configuration = configuration;
            _logger = logger;
            _walletService = walletService;
        }

        [BindProperty]

        public string Amount { get; set; } // Sử dụng string thay vì decimal
        public decimal Balance { get; set; }



        public IEnumerable<Project.Data.Models.Transaction> Transactions { get; set; }



        [BindProperty]
        public string WithdrawerName { get; set; }

        [BindProperty]
        public string SelectedBank { get; set; }

        [BindProperty]
        public string WithdrawAmount { get; set; }


        [BindProperty]
        public string NumberBank { get; set; }

        [BindProperty]
        public string OTP1 { get; set; }

        [BindProperty]
        public string OTP2 { get; set; }

        [BindProperty]
        public string OTP3 { get; set; }

        [BindProperty]
        public string OTP4 { get; set; }

        [BindProperty]
        public string OTP5 { get; set; }

        [BindProperty]
        public string OTP6 { get; set; }







        public IEnumerable<Project.Data.Models.Transaction> UserTransactions { get; set; }

        public async Task OnGetAsync()
        {

           



            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                ViewData["Message"] = "Bạn cần đăng nhập trước khi thực hiện tính năng này.";


            }
            else
            {
                var userID = int.Parse(userIdClaim);

                var wallet = await _walletService.GetWalletWithTransactionsAsync(userID);


                UserTransactions = await _walletService.GetTransactionHistoryAsync(wallet.WalletId);

                Balance = await _walletService.GetWalletBalanceAsync(userID);
                ViewData["Balance"] = Balance.ToString("C");
            }



        }







        public async Task<IActionResult> OnPostWithdrawAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                ViewData["Message"] = "Bạn cần đăng nhập trước khi thực hiện tính năng này.";
                return RedirectToPage("/Auth/Login");
            }

            var wallet = await _walletService.GetWalletWithTransactionsAsync(int.Parse(userIdClaim));
            var otpCode = $"{OTP1}{OTP2}{OTP3}{OTP4}{OTP5}{OTP6}";

            if (string.IsNullOrEmpty(WithdrawerName) || string.IsNullOrEmpty(SelectedBank) || string.IsNullOrEmpty(WithdrawAmount) || string.IsNullOrEmpty(NumberBank) || string.IsNullOrEmpty(otpCode))
            {
                ViewData["Message"] = "Vui lòng nhập đầy đủ thông tin để rút tiền.";
                return Page();
            }

            // Kiểm tra và loại bỏ dấu phân cách hàng nghìn
            var amountString = WithdrawAmount.Replace(".", "").Replace(",", "");

            // Thử chuyển đổi sang decimal
            if (decimal.TryParse(amountString, out decimal amount) && amount > 0)
            {
                decimal yourCurrentBalance = await _walletService.GetWalletBalanceAsync(int.Parse(userIdClaim));
                if (amount > yourCurrentBalance)
                {
                    ViewData["Message"] = "Số dư của bạn không đủ để rút, hãy rút số tiền <= " + yourCurrentBalance;
                    return Page();
                }

                // Thực hiện rút tiền và nhận thông tin lỗi (nếu có)
                var errorMessage = await _walletService.WithdrawalAsync(wallet.WalletId, amount, SelectedBank , WithdrawerName);
                if (errorMessage == null)
                {
                    ViewData["Message"] = "Rút tiền thành công, chờ admin phê duyệt có thể mất tầm vài tiếng!";
                }
                else
                {
                    ViewData["Message"] = $"Rút tiền thất bại: {errorMessage}";
                }

                return Page();
            }

            ViewData["Message"] = "Số tiền không hợp lệ. Vui lòng thử lại.";
            return Page();
        }



        public async Task<IActionResult> OnPostDepositAsync()
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                ViewData["Message"] = "Bạn cần đăng nhập trước khi thực hiện tính năng này.";

                return RedirectToPage("/Auth/Login");
            }


            // Kiểm tra và loại bỏ dấu phân cách hàng nghìn (dấu chấm)
            var amountString = Amount.Replace(".", "").Replace(",", "");

            // Thử chuyển đổi sang decimal
            if (decimal.TryParse(amountString, out decimal amount) && amount > 0)
            {
                // Thực hiện các xử lý tiếp theo với amount đã chuyển đổi thành công
                // Ví dụ: Gọi phương thức xử lý VNPAY như mã hiện tại

                string vnp_Returnurl = _configuration["VnPay:ReturnUrl"];
                string vnp_Url = _configuration["VnPay:Url"];
                string vnp_TmnCode = _configuration["VnPay:TmnCode"];
                string vnp_HashSecret = _configuration["VnPay:HashSecret"];

                VnPayLibrary vnpay = new VnPayLibrary();
                vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", (amount * 100).ToString());
                vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");

                string clientIpAddress = Utils.GetIpAddress(HttpContext);
                vnpay.AddRequestData("vnp_IpAddr", clientIpAddress);
                vnpay.AddRequestData("vnp_OrderInfo", "Nap tien vao vi");
                vnpay.AddRequestData("vnp_OrderType", "other");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

                string bankCode = "VNBANK";
                vnpay.AddRequestData("vnp_BankCode", bankCode);
                string locale = "vn";
                vnpay.AddRequestData("vnp_Locale", locale);

                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                _logger.LogInformation("VNPAY URL: {0}", paymentUrl);

                return Redirect(paymentUrl);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Số tiền không hợp lệ.");
                return Page();
            }
        }



        public async Task<IActionResult> OnGetCallbackAsync()
        {
            // Khởi tạo Dictionary để lưu trữ các tham số trả về từ VNPAY
            var vnpParams = new Dictionary<string, string>();
            foreach (var key in Request.Query.Keys)
            {
                vnpParams[key] = Request.Query[key];
            }

            string vnp_HashSecret = _configuration["VnPay:HashSecret"];
            VnPayLibrary vnpay = new VnPayLibrary();

            // Thêm các tham số trả về từ VNPAY vào VnPayLibrary
            foreach (var param in vnpParams)
            {
                if (!string.IsNullOrEmpty(param.Key) && param.Key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(param.Key, param.Value);
                }
            }

            // Lấy các giá trị từ dữ liệu trả về
            long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
            string vnp_SecureHash = vnpParams["vnp_SecureHash"];
            string terminalID = vnpParams["vnp_TmnCode"];
            long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
            string bankCode = vnpParams["vnp_BankCode"];

            // Kiểm tra chữ ký hợp lệ
            bool isValidSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
            if (isValidSignature)
            {
                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim))
                    {

                        return RedirectToPage("/Auth/Login");
                    }

                    var userId = int.Parse(userIdClaim);

                    // Gọi hàm nạp tiền trong WalletService
                    bool depositSuccess = await _walletService.DepositAsync(userId, vnp_Amount);

                    if (depositSuccess)
                    {
                        ViewData["Message"] = "Giao dịch được thực hiện thành công. Số dư đã được cập nhật.";
                        _logger.LogInformation("Thanh toán thành công, OrderId={0}, VNPAY TranId={1}", orderId, vnpayTranId);
                    }
                    else
                    {
                        ViewData["Message"] = "Giao dịch thành công nhưng không thể cập nhật số dư.";
                        _logger.LogWarning("Không thể cập nhật số dư cho người dùng {UserId}", userId);
                    }
                }
                else
                {
                    ViewData["Message"] = $"Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: {vnp_ResponseCode}";
                    _logger.LogInformation("Thanh toán lỗi, OrderId={0}, VNPAY TranId={1}, ResponseCode={2}", orderId, vnpayTranId, vnp_ResponseCode);
                }

                ViewData["TerminalID"] = "Mã Website (Terminal ID): " + terminalID;
                ViewData["TxnRef"] = "Mã giao dịch thanh toán: " + orderId.ToString();
                ViewData["VnpayTranNo"] = "Mã giao dịch tại VNPAY: " + vnpayTranId.ToString();
                ViewData["Amount"] = "Số tiền thanh toán (VND): " + vnp_Amount.ToString();
                ViewData["BankCode"] = "Ngân hàng thanh toán: " + bankCode;
            }
            else
            {
                ViewData["Message"] = "Có lỗi xảy ra trong quá trình xử lý";
            }

            return Page();
        }





    }
}

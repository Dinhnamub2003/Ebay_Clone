using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Project.Service.Service.VnPay;
using Project.Service.Service.Wallets;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.RazorWeb.Pages.Wallet
{
    public class DepositModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DepositModel> _logger;
        private readonly IWalletService _walletService;

        public DepositModel(IConfiguration configuration, ILogger<DepositModel> logger, IWalletService walletService)
        {
            _configuration = configuration;
            _logger = logger;
            _walletService = walletService;
        }

        [BindProperty]
        public decimal Amount { get; set; }




        public async Task<IActionResult> OnPostDepositAsync() { 
    if (Amount <= 0)
    {
        ModelState.AddModelError(string.Empty, "Số tiền không hợp lệ.");
        return Page();
    }

    // Lấy các cấu hình từ appsettings.json
    string vnp_Returnurl = _configuration["VnPay:ReturnUrl"];
    string vnp_Url = _configuration["VnPay:Url"];
    string vnp_TmnCode = _configuration["VnPay:TmnCode"];
    string vnp_HashSecret = _configuration["VnPay:HashSecret"];

    // Khởi tạo VnPayLibrary và thêm dữ liệu yêu cầu
    VnPayLibrary vnpay = new VnPayLibrary();
    vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
    vnpay.AddRequestData("vnp_Command", "pay");
    vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
    vnpay.AddRequestData("vnp_Amount", (Amount * 100).ToString()); // Số tiền nhân 100
    vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
    vnpay.AddRequestData("vnp_CurrCode", "VND");

    // Lấy địa chỉ IP của người dùng
    string clientIpAddress = Utils.GetIpAddress(HttpContext);
    vnpay.AddRequestData("vnp_IpAddr", clientIpAddress);

    // Thêm thông tin đơn hàng và loại đơn hàng
    vnpay.AddRequestData("vnp_OrderInfo", "Nap tien vao vi");
    vnpay.AddRequestData("vnp_OrderType", "other");

    // Thêm URL callback và mã tham chiếu giao dịch
    vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
    vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); // Mã tham chiếu giao dịch

    // Thêm mã ngân hàng cố định
    string bankCode = "VNBANK"; // Thay đổi mã ngân hàng nếu cần
    vnpay.AddRequestData("vnp_BankCode", bankCode);

    // Thêm ngôn ngữ (locale)
    string locale = "vn"; // Hoặc "en" cho tiếng Anh nếu cần
    vnpay.AddRequestData("vnp_Locale", locale);

    // Tạo URL thanh toán với chữ ký
    string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

    // Ghi log URL thanh toán (tùy chọn)
    _logger.LogInformation("VNPAY URL: {0}", paymentUrl);

    // Chuyển hướng đến URL thanh toán
    return Redirect(paymentUrl);
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




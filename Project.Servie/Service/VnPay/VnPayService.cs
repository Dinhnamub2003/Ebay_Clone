using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Project.Servie.Service.VnPay
{
    public class VnPayService : IVnPayService
    {
        private readonly string _vnpTmnCode = "5ZIBZGYR";
        private readonly string _vnpHashSecret = "DEJO6Q46CQWP2PF441P7BGTDWRHUYOQO";
        private readonly string _vnpPayUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private readonly string _vnpReturnUrl = "http://localhost:5081/Deposit?handler=Callback";
        private readonly string _defaultBankCode = "VNBANK"; // Sử dụng BankCode cố định

        public string CreatePaymentUrl(decimal amount, string orderInfo, string locale, string ipAddress)
        {
            var vnpParams = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", _vnpTmnCode },
                { "vnp_Amount", ((long)amount * 100).ToString() }, // Số tiền nhân 100 để loại bỏ phần thập phân
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", GetRandomTransactionReference() },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_Locale", string.IsNullOrEmpty(locale) ? "vn" : locale },
                { "vnp_ReturnUrl", _vnpReturnUrl },
                { "vnp_IpAddr", ipAddress },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_BankCode", _defaultBankCode }
            };

            string queryString = BuildQueryString(vnpParams);
            string secureHash = CreateSecureHash(vnpParams);
            return $"{_vnpPayUrl}?{queryString}&vnp_SecureHash={secureHash}";
        }

        public bool ValidateResponse(Dictionary<string, string> vnpParams)
        {
            if (!vnpParams.ContainsKey("vnp_SecureHash")) return false;

            string secureHash = vnpParams["vnp_SecureHash"];
            vnpParams.Remove("vnp_SecureHash");

            // Sắp xếp các tham số để đảm bảo thứ tự đúng
            var sortedParams = new SortedDictionary<string, string>(vnpParams);
            string calculatedHash = CreateSecureHash(sortedParams);

            return secureHash.Equals(calculatedHash, StringComparison.OrdinalIgnoreCase);
        }

        private string CreateSecureHash(SortedDictionary<string, string> vnpParams)
        {
            // Tạo chuỗi dữ liệu từ các tham số đã sắp xếp
            string rawHashData = string.Join("&", vnpParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_vnpHashSecret));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHashData));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        private string BuildQueryString(SortedDictionary<string, string> vnpParams)
        {
            // Xây dựng chuỗi truy vấn từ các tham số đã mã hóa URL
            return string.Join("&", vnpParams.Select(kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));
        }

        private string GetRandomTransactionReference()
        {
            // Tạo mã giao dịch ngẫu nhiên cho mỗi yêu cầu
            var random = new Random();
            return random.Next(10000000, 99999999).ToString();
        }
    }
}

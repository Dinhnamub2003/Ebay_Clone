using System.Collections.Generic;

namespace Project.Servie.Service.VnPay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(decimal amount, string orderInfo, string locale, string ipAddress);
        bool ValidateResponse(Dictionary<string, string> vnpParams);
    }
}

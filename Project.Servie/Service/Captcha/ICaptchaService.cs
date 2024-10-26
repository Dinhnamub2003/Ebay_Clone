using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service.Service.Captcha
{
    public interface ICaptchaService
    {
        string GenerateCaptchaCode(int number);
        byte[] GenerateCaptchaImage(string captchaCode);
        bool ValidateCaptchaCode(string inputCaptchaCode, string actualCaptchaCode);
    }
}

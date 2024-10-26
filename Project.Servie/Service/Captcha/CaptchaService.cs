using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace Project.Service.Service.Captcha
{
      public class CaptchaService : ICaptchaService
    {
        public string GenerateCaptchaCode(int number)
        {
            var random = new Random();
            string captcha = "";
            for (int i =0; i < number; i++)
            {
                captcha+= random.Next(1, 9).ToString();
            }


           
            return captcha;
        }

        public byte[] GenerateCaptchaImage(string captchaCode)
        {
            int width = 180;
            int height = 80;
            using var image = new Image<Rgba32>(width, height);
            var random = new Random();

            // Apply background noise
            image.Mutate(ctx =>
            {
                ctx.Fill(Color.Gray);
                ApplyBackgroundNoise(ctx, width, height);
            });

            // Draw the CAPTCHA text with some distortion
            image.Mutate(ctx =>
            {
                var font = SystemFonts.CreateFont("Arial", 36, FontStyle.Bold);

                ctx.DrawText(captchaCode, font, Color.AntiqueWhite, new PointF(width / 6, height / 4));

                // Apply random lines as additional noise
                ApplyRandomLines(ctx, width, height);
            });

            // Save image to a byte array
            using var ms = new MemoryStream();
            image.Save(ms, new PngEncoder());
            return ms.ToArray();
        }

        public bool ValidateCaptchaCode(string inputCaptchaCode, string actualCaptchaCode)
        {
            return inputCaptchaCode.Equals(actualCaptchaCode, StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyRandomLines(IImageProcessingContext ctx, int width, int height)
        {
            var random = new Random();
            var pen = Pens.Solid(Color.Black, 1);
            for (int i = 0; i < 10; i++) // Increase the number of lines
            {
                var points = new PointF[]
                {
                    new PointF(random.Next(0, width), random.Next(0, height)),
                    new PointF(random.Next(0, width), random.Next(0, height))
                };
                ctx.DrawLine(pen, points[0], points[1]); // Corrected to DrawLine
            }
        }

        private void ApplyBackgroundNoise(IImageProcessingContext ctx, int width, int height)
        {
            var random = new Random();
            for (int i = 0; i < 300; i++) // Add more dots for noise
            {
                var x = random.Next(0, width);
                var y = random.Next(0, height);
                ctx.Fill(Color.DarkBlue, new RectangleF(x, y, 2, 2)); // Small dots
            }
        }
    }
}

using Net.Codecrete.QrCodeGenerator;
using SkiaSharp;

namespace SimpLN.Services.QrService;

public class QrCodeService
{
    public string? GenerateQrCodeBase64(string content, int scale = 10, int margin = 4)
    {
        var qrCode = QrCode.EncodeText(content, QrCode.Ecc.Medium);

        using var memoryStream = new MemoryStream();
        qrCode.ToPng(memoryStream, scale, margin);

        return Convert.ToBase64String(memoryStream.ToArray());
    }
}

public static class QrCodeExtensions
{
    public static void ToPng(this QrCode qrCode, Stream stream, int scale, int margin)
    {
        int size = qrCode.Size;
        int imageSize = (size + 2 * margin) * scale;

        using var bitmap = new SKBitmap(imageSize, imageSize);
        using var canvas = new SKCanvas(bitmap);

        // Set transparent background
        canvas.Clear(SKColors.Transparent);

        // Configure paint for shadow effect
        using var shadowPaint = new SKPaint
        {
            Color = SKColors.Gray.WithAlpha(128), // Semi-transparent gray shadow
            IsAntialias = true,
            MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, scale / 2f) // Blur effect for shadow
        };

        // Draw shadow behind the QR code
        float shadowOffset = scale / 3f; // Offset for the shadow
        canvas.Save();
        canvas.Translate(shadowOffset, shadowOffset);
        DrawRoundedModules(canvas, qrCode, scale, margin, shadowPaint);
        canvas.Restore();

        // Configure paint for black rounded modules
        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true // Enable smooth edges for rounded rectangles
        };

        // Draw QR code modules as rounded rectangles
        DrawRoundedModules(canvas, qrCode, scale, margin, paint);

        // Overlay a logo in the center of the QR code
        string logoPath = "wwwroot/media/192x192.png";
        if (File.Exists(logoPath))
        {
            using var logoBitmap = SKBitmap.Decode(logoPath);
            float logoSize = imageSize / 5f; // Logo size is 1/5th of the QR code size
            float left = (imageSize - logoSize) / 2f;
            float top = (imageSize - logoSize) / 2f;

            var logoRect = new SKRect(left, top, left + logoSize, top + logoSize);
            canvas.DrawBitmap(logoBitmap, logoRect);
        }

        // Encode bitmap as PNG and save to stream
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        data.SaveTo(stream);
    }

    private static void DrawRoundedModules(SKCanvas canvas, QrCode qrCode, int scale, int margin, SKPaint paint)
    {
        int size = qrCode.Size;
        float cornerRadius = scale / 2.5f; // Adjust corner radius for more rounded modules

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (qrCode.GetModule(x, y))
                {
                    float left = (x + margin) * scale;
                    float top = (y + margin) * scale;
                    float right = left + scale;
                    float bottom = top + scale;

                    var rect = new SKRect(left, top, right, bottom);
                    canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                }
            }
        }
    }
}

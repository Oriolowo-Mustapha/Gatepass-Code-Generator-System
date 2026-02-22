using Application.Interfaces.Services;
using QRCoder;

namespace Infrastructure.Services;

public class QRCodeGeneratorService : IQRCodeGenerator
{
    public string GenerateQRCodeBase64(string content)
    {
        using var qrGenerator = new QRCoder.QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(content, QRCoder.QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(20);
        return Convert.ToBase64String(qrCodeBytes);
    }
}

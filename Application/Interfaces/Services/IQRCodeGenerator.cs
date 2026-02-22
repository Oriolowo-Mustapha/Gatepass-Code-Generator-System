namespace Application.Interfaces.Services;

public interface IQRCodeGenerator
{
    string GenerateQRCodeBase64(string content);
}

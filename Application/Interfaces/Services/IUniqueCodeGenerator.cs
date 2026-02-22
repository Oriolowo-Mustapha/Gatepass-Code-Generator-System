namespace Application.Interfaces.Services;

public interface IUniqueCodeGenerator
{
    Task<string> GenerateCodeAsync(CancellationToken cancellationToken = default);
}

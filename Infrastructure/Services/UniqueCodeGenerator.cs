using System.Security.Cryptography;
using Application.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UniqueCodeGenerator : IUniqueCodeGenerator
{
    private readonly ApplicationDbContext _context;
    private const string AlphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public UniqueCodeGenerator(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateCodeAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        var datePart = utcNow.ToString("yyMMdd");
        var randomPart = GenerateSecureRandom(4);
        var sequencePart = await GetDailySequenceAsync(utcNow, cancellationToken);
        var payload = $"{datePart}{randomPart}{sequencePart}";
        var checksum = CalculateChecksum(payload);

        return $"GP-{datePart}-{randomPart}-{sequencePart}-{checksum}";
    }

    private static string GenerateSecureRandom(int length)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        var chars = new char[length];
        for (var i = 0; i < length; i++)
            chars[i] = AlphanumericChars[bytes[i] % AlphanumericChars.Length];
        return new string(chars);
    }

    private async Task<string> GetDailySequenceAsync(DateTime utcNow, CancellationToken cancellationToken)
    {
        var todayStart = utcNow.Date;
        var todayEnd = todayStart.AddDays(1);

        var count = await _context.Gatepasses
            .CountAsync(g => g.IssueDate >= todayStart && g.IssueDate < todayEnd, cancellationToken);

        return (count + 1).ToString("D3");
    }

    private static char CalculateChecksum(string payload)
    {
        var sum = 0;
        for (var i = payload.Length - 1; i >= 0; i--)
        {
            var digit = char.IsDigit(payload[i])
                ? payload[i] - '0'
                : payload[i] - 'A' + 10;

            if ((payload.Length - 1 - i) % 2 == 1)
            {
                digit *= 2;
                if (digit > 9) digit -= 9;
            }

            sum += digit;
        }

        var checkValue = (10 - (sum % 10)) % 10;
        return checkValue.ToString()[0];
    }
}

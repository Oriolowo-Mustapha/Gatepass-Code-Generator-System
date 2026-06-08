using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infrastructure.Configuration;

public static class ConnectionStringResolver
{
    public static string Resolve(IConfiguration configuration)
    {
        var configuredConnection = Normalize(configuration.GetConnectionString("DefaultConnection"));
        if (!string.IsNullOrWhiteSpace(configuredConnection))
        {
            return NormalizeConnectionString(configuredConnection);
        }

        var databaseUrl = Normalize(configuration["DATABASE_URL"]);
        if (!string.IsNullOrWhiteSpace(databaseUrl))
        {
            return FromDatabaseUrl(databaseUrl);
        }

        var host = Normalize(configuration["DB_HOST"]);
        var db = Normalize(configuration["DB_NAME"]);
        var user = Normalize(configuration["DB_USERNAME"]);
        var password = Normalize(configuration["DB_PASSWORD"]);
        var port = Normalize(configuration["DB_PORT"]);

        if (!string.IsNullOrWhiteSpace(host) &&
            !string.IsNullOrWhiteSpace(db) &&
            !string.IsNullOrWhiteSpace(user) &&
            !string.IsNullOrWhiteSpace(password))
        {
            return BuildFromParts(host, port, db, user, password);
        }

        throw new InvalidOperationException("Database connection settings were not found.");
    }

    private static string NormalizeConnectionString(string value)
    {
        if (value.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
        {
            return FromDatabaseUrl(value);
        }

        return value;
    }

    private static string BuildFromParts(string host, string? port, string database, string username, string password)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = NormalizeHost(host),
            Port = int.TryParse(port, out var parsedPort) ? parsedPort : 5432,
            Username = username,
            Password = password,
            Database = database,
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };

        return builder.ToString();
    }

    private static string FromDatabaseUrl(string databaseUrl)
    {
        if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var uri) ||
            string.IsNullOrWhiteSpace(uri.Host))
        {
            return databaseUrl;
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Database = uri.AbsolutePath.Trim('/'),
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };

        if (!string.IsNullOrWhiteSpace(uri.UserInfo))
        {
            var credentials = uri.UserInfo.Split(':', 2);
            builder.Username = Uri.UnescapeDataString(credentials[0]);
            builder.Password = credentials.Length > 1
                ? Uri.UnescapeDataString(credentials[1])
                : string.Empty;
        }

        return builder.ToString();
    }

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var trimmed = value.Trim();
        if (trimmed.Length >= 2 &&
            ((trimmed.StartsWith('"') && trimmed.EndsWith('"')) ||
             (trimmed.StartsWith('\'') && trimmed.EndsWith('\''))))
        {
            trimmed = trimmed[1..^1];
        }

        return trimmed;
    }

    private static string NormalizeHost(string host)
    {
        if (Uri.TryCreate(host, UriKind.Absolute, out var uri) && !string.IsNullOrWhiteSpace(uri.Host))
        {
            return uri.Host;
        }

        return host
            .Replace("tcp://", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("postgresql://", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("postgres://", string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}

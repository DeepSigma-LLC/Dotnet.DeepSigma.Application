using Microsoft.Extensions.Configuration;

namespace DeepSigma.Application.Settings;

/// <summary>
/// Represents a service for accessing application configuration settings.
/// </summary>
public sealed class AppConfigurationService : IAppConfigurationService
{
    private readonly IConfiguration _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppConfigurationService"/> class with the specified configuration.
    /// </summary>
    /// <param name="config"></param>
    public AppConfigurationService(IConfiguration config) => _config = config;

    /// <inheritdoc/>
    public T? Get<T>(string key) => _config.GetValue<T>(key);

    /// <inheritdoc/>
    public T Get<T>(string key, T defaultValue) => _config.GetValue(key, defaultValue) ?? defaultValue;

    /// <inheritdoc/>
    public IReadOnlyList<ConfigurationEntry> GetAll() =>
        _config.AsEnumerable()
               .Where(kvp => kvp.Value is not null)      // skip section nodes, keep leaves
               .OrderBy(kvp => kvp.Key)
               .Select(kvp => new ConfigurationEntry(
                   Key: kvp.Key,
                   Value: Redact(kvp.Key, kvp.Value),
                   Provider: ProviderFor(kvp.Key)))
               .ToList();

    // See the security note below — this is not optional.
    private static string? Redact(string key, string? value) =>
        LooksSensitive(key) ? "••••••••" : value;

    private static bool LooksSensitive(string key) =>
        key.Contains("Password", StringComparison.OrdinalIgnoreCase) ||
        key.Contains("Secret", StringComparison.OrdinalIgnoreCase) ||
        key.Contains("ApiKey", StringComparison.OrdinalIgnoreCase) ||
        key.Contains("Token", StringComparison.OrdinalIgnoreCase) ||
        key.Contains("ConnectionString", StringComparison.OrdinalIgnoreCase);

    private string ProviderFor(string key) =>
        _config is IConfigurationRoot root
            ? root.Providers.LastOrDefault(p => p.TryGet(key, out _))?.GetType().Name ?? "Unknown"
            : "Unknown";
}
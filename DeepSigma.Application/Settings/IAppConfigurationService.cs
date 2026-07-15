
namespace DeepSigma.Application.Settings;

/// <summary>
/// Read-only view over the app's deployment configuration (appsettings.json,
/// environment variables, etc.). Values are set by whoever deploys the app,
/// not by the user — hence no write members.
/// </summary>
public interface IAppConfigurationService
{
    /// <summary>
    /// Gets the value associated with the specified key. If the key does not exist, returns null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    T? Get<T>(string key);

    /// <summary>
    /// Gets the value associated with the specified key. If the key does not exist, returns the specified default value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    T Get<T>(string key, T defaultValue);

    /// <summary>
    /// Flattened key/value pairs for display, e.g. "Logging:LogLevel:Default" → "Information".
    /// </summary>
    IReadOnlyList<ConfigurationEntry> GetAll();
}

/// <summary>
/// Represents a single configuration entry with a key, value, and provider.
/// </summary>
/// <param name="Key"></param>
/// <param name="Value"></param>
/// <param name="Provider"></param>
public sealed record ConfigurationEntry(string Key, string? Value, string Provider);
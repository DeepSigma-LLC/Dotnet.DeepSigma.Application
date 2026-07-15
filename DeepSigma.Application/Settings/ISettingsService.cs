
namespace DeepSigma.Application.Settings;

/// <summary>
/// Represents a service for managing application settings.
/// </summary>
/// <remarks>
/// Example DI usage:
/// <code>
/// services.AddSingleton&lt;ISettingsService, SettingsService&gt;();
/// </code>
/// </remarks>
public interface ISettingsService
{
    /// <summary>
    /// Gets the value associated with the specified key. If the key does not exist, returns the provided default value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key of the setting.</param>
    /// <param name="defaultValue">The default value to return if the key does not exist.</param>
    /// <returns>The value associated with the specified key, or the default value if the key does not exist.</returns>
    T Get<T>(string key, T defaultValue);

    /// <summary>
    /// Gets the value associated with the specified key asynchronously. If the key does not exist, returns the provided default value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key of the setting.</param>
    /// <param name="defaultValue">The default value to return if the key does not exist.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value associated with the specified key, or the default value if the key does not exist.</returns>
    Task<T> GetAsync<T>(string key, T defaultValue);

    /// <summary>
    /// Gets the value associated with the specified key. If the key does not exist, returns null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key of the setting.</param>
    /// <returns>The value associated with the specified key, or null if the key does not exist.</returns>
    T? Get<T>(string key);

    /// <summary>
    /// Gets the value associated with the specified key asynchronously. If the key does not exist, returns null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key of the setting.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the value associated with the specified key, or null if the key does not exist.</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Sets the value for the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key of the setting.</param>
    /// <param name="value">The value to set.</param>
    void Set<T>(string key, T value);

    /// <summary>
    /// Sets the value for the specified key asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The key of the setting.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value);

    /// <summary>
    /// Determines whether the specified key exists in the settings.
    /// </summary>
    /// <param name="key">The key of the setting.</param>
    /// <returns><c>true</c> if the key exists; otherwise, <c>false</c>.</returns>
    bool Contains(string key);

    /// <summary>
    /// Determines whether the specified key exists in the settings asynchronously.
    /// </summary>
    /// <param name="key">The key of the setting.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if the key exists; otherwise, <c>false</c>.</returns>
    Task<bool> ContainsAsync(string key);

    /// <summary>
    /// Removes the value associated with the specified key from the settings.
    /// </summary>
    /// <param name="key">The key of the setting to remove.</param>
    void Remove(string key);

    /// <summary>
    /// Removes the value associated with the specified key from the settings asynchronously.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task RemoveAsync(string key);
}
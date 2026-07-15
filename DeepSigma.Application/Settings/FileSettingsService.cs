using System.Text.Json;

namespace DeepSigma.Application.Settings;

/// <summary>
/// <see cref="ISettingsService"/> backed by a single JSON document in the user's
/// app-data folder (e.g. <c>%AppData%/Cortex/usersettings.json</c>).
/// </summary>
/// <remarks>
/// All settings live in one document with a <c>schemaVersion</c> so the whole file
/// can be migrated when the shape changes across app versions. Writes are atomic
/// (temp file + move) to avoid corruption on a mid-write crash.
/// </remarks>
public partial class FileSettingsService : ISettingsService
{
    private const int CurrentSchemaVersion = 1;
    private readonly string _directory;
    private readonly string _filePath;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly JsonSerializerOptions _json = new()
    {
        WriteIndented = true,                          // human-readable on disk
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // In-memory cache of the document; null until first load.
    private SettingsDocument? _doc;

    /// <param name="directory">
    /// Optional override for the storage folder (useful for tests). Defaults to
    /// <c>%AppData%/Cortex</c>.
    /// </param>
    /// <param name="appName">The app name used to construct the default storage folder.</param>
    /// <param name="fileName">Optional file name. Defaults to <c>usersettings.json</c>.</param>
    public FileSettingsService(string appName, string? directory = null, string fileName = "usersettings.json")
    {
        if (string.IsNullOrWhiteSpace(appName))
            throw new ArgumentException("App name cannot be null or whitespace.", nameof(appName));

        _directory = directory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            appName);
        _filePath = Path.Combine(_directory, fileName);
    }

    // ---- Sync API (genuinely synchronous I/O) --------------------------

    /// <inheritdoc/>
    public T Get<T>(string key, T defaultValue)
    {
        _gate.Wait();
        try
        {
            EnsureLoaded();
            return _doc!.TryRead<T>(key, out var value) ? value! : defaultValue;
        }
        finally { _gate.Release(); }
    }

    /// <inheritdoc/>
    public T? Get<T>(string key)
    {
        _gate.Wait();
        try
        {
            EnsureLoaded();
            _doc!.TryRead<T>(key, out var value);
            return value;
        }
        finally { _gate.Release(); }
    }

    /// <inheritdoc/>
    public void Set<T>(string key, T value)
    {
        _gate.Wait();
        try
        {
            EnsureLoaded();
            _doc!.Values[key] = JsonSerializer.SerializeToElement(value, _json);
            Save();
        }
        finally { _gate.Release(); }
    }

    /// <inheritdoc/>
    public bool Contains(string key)
    {
        _gate.Wait();
        try
        {
            EnsureLoaded();
            return _doc!.Values.ContainsKey(key);
        }
        finally { _gate.Release(); }
    }

    /// <inheritdoc/>
    public void Remove(string key)
    {
        _gate.Wait();
        try
        {
            EnsureLoaded();
            if (_doc!.Values.Remove(key))
                Save();
        }
        finally { _gate.Release(); }
    }

    // ---- Async API (genuinely asynchronous I/O) ------------------------
    /// <inheritdoc/>
    public async Task<T> GetAsync<T>(string key, T defaultValue)
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            await EnsureLoadedAsync().ConfigureAwait(false);
            return _doc!.TryRead<T>(key, out var value) ? value! : defaultValue;
        }
        finally { _gate.Release(); }
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(string key)
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            await EnsureLoadedAsync().ConfigureAwait(false);
            _doc!.TryRead<T>(key, out var value);
            return value;
        }
        finally { _gate.Release(); }
    }

    /// <inheritdoc/>
    public async Task SetAsync<T>(string key, T value)
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            await EnsureLoadedAsync().ConfigureAwait(false);
            _doc!.Values[key] = JsonSerializer.SerializeToElement(value, _json);
            await SaveAsync().ConfigureAwait(false);
        }
        finally { _gate.Release(); }
    }

    /// <inheritdoc/>
    public async Task<bool> ContainsAsync(string key)
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            await EnsureLoadedAsync().ConfigureAwait(false);
            return _doc!.Values.ContainsKey(key);
        }
        finally { _gate.Release(); }
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string key)
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            await EnsureLoadedAsync().ConfigureAwait(false);
            if (_doc!.Values.Remove(key))
                await SaveAsync().ConfigureAwait(false);
        }
        finally { _gate.Release(); }
    }

    // ---- Load / Save (the only sync-vs-async difference) ---------------

    private void EnsureLoaded()
    {
        if (_doc is not null) return;
        var json = File.Exists(_filePath) ? File.ReadAllText(_filePath) : null;
        _doc = json is null ? new SettingsDocument() : SettingsDocument.Parse(json);
        if (_doc.Migrate()) Save();
    }

    private async Task EnsureLoadedAsync()
    {
        if (_doc is not null) return;
        var json = File.Exists(_filePath)
            ? await File.ReadAllTextAsync(_filePath).ConfigureAwait(false)
            : null;
        _doc = json is null ? new SettingsDocument() : SettingsDocument.Parse(json);
        if (_doc.Migrate()) await SaveAsync().ConfigureAwait(false);
    }

    private void Save()
    {
        Directory.CreateDirectory(_directory);
        var tmp = _filePath + ".tmp";
        File.WriteAllText(tmp, _doc!.Serialize());
        File.Move(tmp, _filePath, overwrite: true);   // atomic replace
    }

    private async Task SaveAsync()
    {
        Directory.CreateDirectory(_directory);
        var tmp = _filePath + ".tmp";
        await File.WriteAllTextAsync(tmp, _doc!.Serialize()).ConfigureAwait(false);
        File.Move(tmp, _filePath, overwrite: true);
    }
}
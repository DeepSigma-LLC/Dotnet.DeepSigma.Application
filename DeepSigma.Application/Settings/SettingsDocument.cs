using System.Text.Json;

namespace DeepSigma.Application.Settings;

internal sealed class SettingsDocument
{
    public const int CurrentSchemaVersion = 1;

    public int SchemaVersion { get; set; } = CurrentSchemaVersion;
    public Dictionary<string, JsonElement> Values { get; set; } = new();

    private static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>Parse a document from JSON, falling back to empty on corruption.</summary>
    public static SettingsDocument Parse(string json)
    {
        try { return JsonSerializer.Deserialize<SettingsDocument>(json, Json) ?? new(); }
        catch (JsonException) { return new(); }
    }

    public string Serialize() => JsonSerializer.Serialize(this, Json);

    public bool TryRead<T>(string key, out T? value)
    {
        if (Values.TryGetValue(key, out var element))
        {
            try { value = element.Deserialize<T>(Json); return value is not null; }
            catch (JsonException) { value = default; return false; }
        }
        value = default;
        return false;
    }

    public void Write<T>(string key, T value) =>
        Values[key] = JsonSerializer.SerializeToElement(value, Json);

    /// <summary>Upgrades an older document in place. Returns true if it changed.</summary>
    public bool Migrate()
    {
        if (SchemaVersion == CurrentSchemaVersion) return false;
        // if (SchemaVersion < 2) { ... }
        SchemaVersion = CurrentSchemaVersion;
        return true;
    }
}
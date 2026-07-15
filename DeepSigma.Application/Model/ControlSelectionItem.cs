namespace DeepSigma.Application.Model;

/// <summary>
/// Represents an item in a control selection, with a display name and a value of type T.
/// </summary>
/// <typeparam name="T">The type of the value associated with the item.</typeparam>
/// <param name="DisplayName">The display name of the item.</param>
/// <param name="Value">The value associated with the item.</param>
public record class ControlSelectionItem<T>(string DisplayName, T Value) where T : notnull;
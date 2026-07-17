using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DeepSigma.WinUI;

/// <summary>
/// Provides a generic observable value wrapper that raises
/// <see cref="INotifyPropertyChanged.PropertyChanged" /> when <see cref="Value" /> changes.
/// </summary>
/// <typeparam name="T">
/// The type of value to wrap.
/// </typeparam>
/// <remarks>
/// This type is useful for simple binding scenarios where a value needs change
/// notifications without defining a dedicated view model property.
/// <para>C# usage:</para>
/// <code>
/// public ObservableProperty&lt;string&gt; ShowHideToolTip { get; } = new("Hide");
///
/// ShowHideToolTip.Value = "Show";
/// </code>
///
/// <para>XAML usage:</para>
/// <code language="xml">
/// &lt;Button ToolTipService.ToolTip="{x:Bind ShowHideToolTip.Value, Mode=OneWay}" /&gt;
/// </code>
/// </remarks>
public partial class ObservableValue<T> : INotifyPropertyChanged
{
    /// <summary>
    /// Property changed event that is raised when the <see cref="Value" /> property changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValue{T}" /> class with the specified initial value.
    /// </summary>
    /// <param name="value"></param>
    [SetsRequiredMembers]
    public ObservableValue(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets or sets the value of the property. When the value is set, it checks if the new value is different from the current value. 
    /// If they are different, it updates the value and raises the PropertyChanged event.
    /// </summary>
    public required T Value
    {
        get => field;
        set
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return;

            field = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc />
    public override string? ToString() => Value?.ToString();

    /// <summary>
    /// Defines an implicit conversion from PropertyChangeNotificationWrapper&lt;T&gt; to T, allowing the wrapper to be used directly as its underlying value type.
    /// </summary>
    /// <remarks>
    /// Example usage:
    /// <code>
    /// ObservableProperty&lt;string&gt; wrapper = new("Hello");
    /// string value = wrapper; // Implicitly converts to string
    /// </code>
    /// </remarks>
    /// <param name="wrapper">The wrapper instance to convert.</param>
    public static implicit operator T(ObservableValue<T> wrapper) => wrapper.Value;


    // Was going to enable implicit conversion from T to ObservableProperty<T> but decided against it since setting a value my change the referance object which could cause issues with binding. Instead, you can use the constructor to create a new ObservableProperty<T> instance with the desired value.
    //public static implicit operator ObservableProperty<T>(T value) => new(value);
}
namespace Vote.Monitor.Core.Models;

/// <summary>
/// Wrapper over nullable types to indicate if value is undefined.
/// </summary>
/// <remarks>NOTE! NULL is a value in this case.</remarks>
/// <typeparam name="T"></typeparam>
public class ValueOrUndefined<T>
{
    private readonly T? _value;
    public bool IsUndefined { get; }

    private ValueOrUndefined(T? value, bool isUndefined)
    {
        _value = value;
        IsUndefined = isUndefined;
    }

    public static ValueOrUndefined<T?> Some(T value) => new ValueOrUndefined<T?>(value, false);
    public static ValueOrUndefined<T?> Undefined() => new ValueOrUndefined<T?>(default, true);

    /// <summary>Gets the value of the current <see cref="T:ValueOrUndefined" /> object if it has been assigned a valid underlying value.</summary>
    /// <exception cref="T:System.InvalidOperationException">The <see cref="P:ValueOrUndefined.IsUndefined" /> property is <see langword="false" />.</exception>
    /// <returns>The value of the current <see cref="T:ValueOrUndefined" /> object if the <see cref="P:ValueOrUndefined.IsUndefined" /> property is <see langword="true" />. An exception is thrown if the <see cref="P:ValueOrUndefined.IsUndefined" /> property is <see langword="false" />.</returns>
    public T? Value
    {
        get
        {
            if (IsUndefined)
                throw new InvalidOperationException("Value is undefined.");
            return _value;
        }
    }
}


namespace Maple.Json.ObjectAsPrimitiveConverter.Configuration;

/// <summary>
///     Specifies the format to use when converting objects to primitive types.
/// </summary>
public enum ObjectFormat
{
    /// <summary>
    ///     Use a dictionary to represent the object.
    /// </summary>
    Dictionary,

    /// <summary>
    ///     Use an ExpandoObject to represent the object.
    /// </summary>
    Expando
}

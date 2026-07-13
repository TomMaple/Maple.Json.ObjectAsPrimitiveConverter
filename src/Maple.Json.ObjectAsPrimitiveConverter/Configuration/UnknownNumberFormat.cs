namespace Maple.Json.ObjectAsPrimitiveConverter.Configuration;

/// <summary>
///     Specifies the behavior when encountering a number format that is not recognized.
/// </summary>
public enum UnknownNumberFormat
{
    /// <summary>
    ///     Throw an error when an unknown number format is encountered.
    /// </summary>
    Error,

    /// <summary>
    ///     Return a <see cref="JsonElement" /> when an unknown number format is encountered.
    /// </summary>
    JsonElement
}

namespace Maple.Json.ObjectAsPrimitiveConverter.Configuration;

/// <summary>
///     Specifies the desired format for floating-point numbers when converting objects to primitive types.
/// </summary>
public enum FloatFormat
{
    /// <summary>
    ///     Use the [System.Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal) (`decimal`) to represent a
    ///     floating-point number.
    /// </summary>
    Decimal,

    /// <summary>
    ///     Use the [System.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double) (`double`) to represent a
    ///     floating-point number.
    /// </summary>
    Double,

    /// <summary>
    ///     Use the [System.Single](https://learn.microsoft.com/en-us/dotnet/api/system.single) (`float`) to represent a
    ///     floating-point number.
    /// </summary>
    Single
}

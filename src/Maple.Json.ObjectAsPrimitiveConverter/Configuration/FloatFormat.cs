// SPDX-License-Identifier: MIT
/*
 * This code is a part of a Maple.Json.ObjectAsPrimitiveConverter library project.
 * https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter
 * Copyright (c) Tom Maple
 *
 * The implementation is based on the implementation provided by dbc
 * on StackOverflow: https://stackoverflow.com/a/65974452
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

namespace Maple.Json.ObjectAsPrimitiveConverter.Configuration;

/// <summary>
///     Specifies the desired format for floating-point numbers when converting objects to primitive types.
/// </summary>
public enum FloatFormat
{
    /// <summary>
    ///     Use the [System.Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal) (`decimal`)
    ///     to represent a floating-point number.
    /// </summary>
    Decimal,

    /// <summary>
    ///     Use the [System.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double) (`double`)
    ///     to represent a floating-point number.
    /// </summary>
    Double,

    /// <summary>
    ///     Use the [System.Single](https://learn.microsoft.com/en-us/dotnet/api/system.single) (`float`)
    ///     to represent a floating-point number.
    /// </summary>
    Single
}

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

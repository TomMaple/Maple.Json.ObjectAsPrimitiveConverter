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

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

using System;

namespace Maple.Json.ObjectAsPrimitiveConverter.Configuration;

/// <summary>
///     Specifies the types of date and time representations to detect when converting text properties to primitive types.
/// </summary>
/// <remarks>
///     This enumeration can be combined using a bitwise OR to detect multiple date and time types simultaneously.
/// </remarks>
[Flags]
public enum DetectDateTime
{
    /// <summary>
    ///     No date and time representations will be detected.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Detects the [System.DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset) type.
    /// </summary>
    /// <remarks>
    ///     The value needs to contain a time zone offset to be detected as
    ///     a [System.DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset).
    /// </remarks>
    DateTimeOffset = 1,

    /// <summary>
    ///     Detects the [System.DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime) type.
    /// </summary>
    /// <remarks>
    ///     The value cannot contain a time zone offset to be detected as
    ///     a [System.DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime).
    /// </remarks>
    DateTime = 2,

    /// <summary>
    ///     Detects the [System.DateOnly](https://learn.microsoft.com/en-us/dotnet/api/system.dateonly) type.
    /// </summary>
    /// <remarks>
    ///     The value must represent a date without a time component to be detected as
    ///     a [System.DateOnly](https://learn.microsoft.com/en-us/dotnet/api/system.dateonly).
    /// </remarks>
    DateOnly = 4,

    /// <summary>
    ///     Detects the [System.TimeOnly](https://learn.microsoft.com/en-us/dotnet/api/system.timeonly) type.
    /// </summary>
    /// <remarks>
    ///     The value must represent a time without a date component to be detected as
    ///     a [System.TimeOnly](https://learn.microsoft.com/en-us/dotnet/api/system.timeonly).
    /// </remarks>
    TimeOnly = 8
}

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

#pragma warning disable IDE0290
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Maple.Json.ObjectAsPrimitiveConverter.Configuration;

namespace Maple.Json.ObjectAsPrimitiveConverter;

/// <summary>
///     A JSON converter that can serialize and deserialize objects as their primitive types.
/// </summary>
/// <exception cref="JsonException">Unknown <see cref="JsonTokenType" /> is encountered.</exception>
/// <remarks>Based on the implementation from: https://stackoverflow.com/a/65974452</remarks>
public partial class ObjectAsPrimitiveConverter : JsonConverter<object>
{
    #region consts

    private static readonly SearchValues<char> DigitSearchValues = SearchValues.Create("0123456789");

    #endregion

    #region constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="ObjectAsPrimitiveConverter" /> class with the specified configuration
    ///     options.
    /// </summary>
    /// <param name="floatFormat">Specifies the default type for floating-point numbers.</param>
    /// <param name="unknownNumberFormat">Specifies the behavior when encountering a number format that is not recognized.</param>
    /// <param name="detectDateTime">Specifies whether to detect date-time values and which formats to recognize.</param>
    /// <param name="objectFormat">Specifies the format to use when converting objects to primitive types.</param>
    public ObjectAsPrimitiveConverter(
        FloatFormat floatFormat = FloatFormat.Decimal,
        UnknownNumberFormat unknownNumberFormat = UnknownNumberFormat.Error,
        DetectDateTime detectDateTime = DetectDateTime.None,
        ObjectFormat objectFormat = ObjectFormat.Dictionary)
    {
        FloatFormat = floatFormat;
        UnknownNumberFormat = unknownNumberFormat;
        DetectDateTime = detectDateTime;
        ObjectFormat = objectFormat;
    }

    #endregion

    /// <summary>
    ///     A bitwise combination of the enumeration values that specifies which date and time representations
    ///     to detect when converting text properties to primitive types.
    /// </summary>
    public DetectDateTime DetectDateTime { get; init; }

    /// <summary>
    ///     The type for floating-point numbers.
    /// </summary>
    public FloatFormat FloatFormat { get; init; }

    /// <summary>
    ///     The type to use when converting objects to primitive types.
    /// </summary>
    public ObjectFormat ObjectFormat { get; init; }

    /// <summary>
    ///     The behavior when encountering a number format that is not recognized.
    /// </summary>
    public UnknownNumberFormat UnknownNumberFormat { get; init; }

    /// <inheritdoc />
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Comment:
            case JsonTokenType.None:
            case JsonTokenType.Null:
                return null;

            case JsonTokenType.False:
                return false;

            case JsonTokenType.True:
                return true;

            case JsonTokenType.String:
                if (TryGetDateTime(ref reader, out var dtValue))
                    return dtValue;

                return reader.GetString();

            case JsonTokenType.Number:
            {
                // Extract the raw UTF-8 bytes of the JSON number
                var valueSpan = reader.HasValueSequence
                    ? reader.ValueSequence.ToArray()
                    : reader.ValueSpan;

                var textValue = Encoding.UTF8.GetString(valueSpan);

                if (textValue.Contains('.'))
                {
                    if (TryGetFloat(ref reader, out var floatValue))
                        return floatValue;
                }
                else
                {
                    if (TryGetInteger(ref reader, out var result))
                        return result;
                }

                using var doc = JsonDocument.ParseValue(ref reader);

                if (UnknownNumberFormat is UnknownNumberFormat.JsonElement)
                    return doc.RootElement.Clone();

                throw new JsonException($"Cannot parse number: ‘{doc.RootElement}’");
            }

            case JsonTokenType.StartArray:
            {
                var list = new List<object?>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    var item = Read(ref reader, typeof(object), options);
                    list.Add(item);
                }

                return list.ToArray();
            }

            case JsonTokenType.StartObject:
            {
                var dict = CreateDictionary();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    while (reader.TokenType == JsonTokenType.Comment)
                        reader.Read();

                    if (reader.TokenType == JsonTokenType.EndObject)
                        break;

                    if (reader.TokenType == JsonTokenType.Null)
                        continue;

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        using var doc = JsonDocument.ParseValue(ref reader);

                        throw new JsonException($"Cannot parse object “{doc.RootElement}”!");
                    }

                    var key = reader.GetString() ?? string.Empty;
                    reader.Read();
                    var value = Read(ref reader, typeof(object), options);
                    dict[key] = value;
                }

                return dict;
            }

            default:
                throw new JsonException($"Unknown token: {reader.TokenType}");
        }
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else if (value.GetType() == typeof(object))
        {
            writer.WriteStartObject();
            writer.WriteEndObject();
        }
        else
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }

    #region helper methods

    private IDictionary<string, object?> CreateDictionary()
    {
        return ObjectFormat == ObjectFormat.Expando
            ? new ExpandoObject()
            : new Dictionary<string, object?>();
    }

    private static bool IsDateOnly(string value)
    {
        return DateOnlyRegex().IsMatch(value);
    }

    private static bool IsDateTime(string value)
    {
        return DateTimeRegex().IsMatch(value);
    }

    private static bool IsDateTimeOffset(string value)
    {
        return DateTimeOffsetRegex().IsMatch(value);
    }

    private static bool IsTimeOnly(string value)
    {
        return TimeOnlyRegex().IsMatch(value);
    }

    private bool TryGetDateTime(ref Utf8JsonReader reader, [NotNullWhen(true)] out object? value)
    {
        value = null;

        if (reader.TokenType != JsonTokenType.String || DetectDateTime == DetectDateTime.None)
            return false;

        var stringValue = reader.GetString();
        if (stringValue is not { Length: > 2 and < 60 })
            return false;

        // Every detectable format contains at least one digit, so bail out before touching any regex or parser
        if (!stringValue.AsSpan().ContainsAny(DigitSearchValues))
            return false;

        // Try to parse the string as a DateTimeOffset, DateTime, DateOnly, or TimeOnly based on the DetectDateTime flags
        if (DetectDateTime.HasFlag(DetectDateTime.DateTimeOffset)
            && IsDateTimeOffset(stringValue)
            && DateTimeOffset.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dto))
        {
            value = dto;
            return true;
        }

        if (DetectDateTime.HasFlag(DetectDateTime.DateTime)
            && IsDateTime(stringValue)
            && DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            value = dt;
            return true;
        }

        if (DetectDateTime.HasFlag(DetectDateTime.DateOnly)
            && IsDateOnly(stringValue)
            && DateOnly.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            value = date;
            return true;
        }

        if (DetectDateTime.HasFlag(DetectDateTime.TimeOnly)
            && IsTimeOnly(stringValue)
            && TimeOnly.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
        {
            value = time;
            return true;
        }

        value = null;
        return false;
    }

    private bool TryGetFloat(ref Utf8JsonReader reader, [NotNullWhen(true)] out object? floatValue)
    {
        if (FloatFormat is FloatFormat.Decimal
            && reader.TryGetDecimal(out var dec))
        {
            floatValue = dec;
            return true;
        }

        if (FloatFormat is FloatFormat.Double
            && reader.TryGetDouble(out var dbl)
            && !double.IsInfinity(dbl) && !double.IsNaN(dbl))
        {
            floatValue = dbl;
            return true;
        }

        if (FloatFormat is FloatFormat.Single
            && reader.TryGetSingle(out var flt)
            && !float.IsInfinity(flt) && !float.IsNaN(flt))
        {
            floatValue = flt;
            return true;
        }

        floatValue = null;
        return false;
    }

    private static bool TryGetInteger(ref Utf8JsonReader reader, [NotNullWhen(true)] out object? integerValue)
    {
        if (reader.TryGetInt32(out var int32))
        {
            integerValue = int32;
            return true;
        }

        if (reader.TryGetInt64(out var int64))
        {
            integerValue = int64;
            return true;
        }

        // Only values that overflow Int64 reach this point, so the string allocation is rare.
        var valueSpan = reader.HasValueSequence
            ? reader.ValueSequence.ToArray()
            : reader.ValueSpan;

        var textValue = Encoding.UTF8.GetString(valueSpan);

        if (BigInteger.TryParse(textValue, out var bigInt))
        {
            integerValue = bigInt;
            return true;
        }

        integerValue = null;
        return false;
    }

    #endregion

    #region Regexes

    /// <summary>
    ///     Matches a text that is only a calendar date: a year (4 digits, starting with 1 or 2), a month
    ///     (numeric, e.g. "03", or a word, e.g. "Jun") and a day (1-2 digits) — e.g. "2026-03-21", "2026/3/21",
    ///     "Sat Jun 01 2024".
    /// </summary>
    [GeneratedRegex(
        @"^\s*(?:[12]\d{3}[-/][01]?\d[-/][0-3]?\d|(?:[A-Za-z]{3,}\s+)?[A-Za-z]{3,}\s+[0-3]?\d\s+[12]\d{3})\s*$",
        RegexOptions.IgnoreCase)]
    private static partial Regex DateOnlyRegex();

    /// <summary>
    ///     Matches a text that is only a clock time: hour:minute, with optional seconds, fractional seconds,
    ///     and AM/PM designator — e.g. "13:01:06", "13:01:06.0000000", "1:01 PM".
    /// </summary>
    [GeneratedRegex(@"^\s*\d{1,2}:\d{2}(?::\d{2})?(?:\.\d+)?\s*(?:AM|PM)?\s*$",
        RegexOptions.IgnoreCase)]
    private static partial Regex TimeOnlyRegex();

    /// <summary>
    ///     Matches a text that is a calendar date (see <see cref="DateOnlyRegex" />) followed by a clock time
    ///     (see <see cref="TimeOnlyRegex" />), with no timezone/offset part — e.g. "2026-03-21T13:01:06".
    /// </summary>
    [GeneratedRegex(
        @"^\s*(?:[12]\d{3}[-/][01]?\d[-/][0-3]?\d|(?:[A-Za-z]{3,}\s+)?[A-Za-z]{3,}\s+[0-3]?\d\s+[12]\d{3})[T\s]+\d{1,2}:\d{2}(?::\d{2})?(?:\.\d+)?\s*(?:AM|PM)?\s*$",
        RegexOptions.IgnoreCase)]
    private static partial Regex DateTimeRegex();

    /// <summary>
    ///     Matches a text that is a calendar date followed by a clock time followed by a timezone/offset —
    ///     "Z", or a sign and a 1-2 digit hour with an optional ":00"/":30" minute part — e.g.
    ///     "2024-06-01T12:34:56+04:00", "2024-06-01T12:34:56Z", "Sat Jun 01 2024 12:34:56 +04:00".
    /// </summary>
    [GeneratedRegex(
        @"^\s*(?:[12]\d{3}[-/][01]?\d[-/][0-3]?\d|(?:[A-Za-z]{3,}\s+)?[A-Za-z]{3,}\s+[0-3]?\d\s+[12]\d{3})[T\s]+\d{1,2}:\d{2}(?::\d{2})?(?:\.\d+)?\s*(?:AM|PM)?\s*(?:Z|[+-][01]?\d(?::?[03]0)?)\s*$",
        RegexOptions.IgnoreCase)]
    private static partial Regex DateTimeOffsetRegex();

    #endregion
}
using Maple.Json.ObjectAsPrimitiveConverter.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Numerics;
using System.Text.Json;

namespace Maple.Json.ObjectAsPrimitiveConverter.Tests.Unit;

public class ObjectAsPrimitiveConverterUnitTests
{
    #region invalid input

    [Fact]
    public void Deserialize_NoValue_ThrowsException()
    {
        // Arrange
        string? json = null;

        // Act
        var exception = Record.Exception(() => JsonSerializer.Deserialize<object>(json!, GetOptions()));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<ArgumentNullException>();
    }

    [Fact]
    public void Deserialize_EmptyValue_ThrowsException()
    {
        // Arrange
        var json = string.Empty;

        // Act
        var exception = Record.Exception(() => JsonSerializer.Deserialize<object>(json, GetOptions()));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<JsonException>();
        exception.Message.ShouldContain("The input does not contain any JSON tokens");
    }

    [Theory]
    [InlineData(".")]
    [InlineData("''")]
    [InlineData("--")]
    [InlineData("()")]
    public void Deserialize_InvalidValue_ThrowsException(string json)
    {
        // Act
        var exception = Record.Exception(() => JsonSerializer.Deserialize<object>(json, GetOptions()));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<JsonException>();
    }

    #endregion

    #region valid input

    [Fact]
    public void Deserialize_ValidJsonWithNullValue_ReturnsObjectWithNullValue()
    {
        // Arrange
        const string json = """{"Property":null}""";

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions());

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("Property");
        dictionary["Property"].ShouldBeNull();
    }

    [Fact]
    public void Deserialize_ValidJsonWithCommentAndCommentsEnabled_ReturnsObjectWithoutComment()
    {
        // Arrange
        const string json = """
                            {
                              // This is comment 1
                              "Property1": "Value",
                              // This is comment 2
                              "Property2": 123
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(commentHandling: JsonCommentHandling.Skip));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("Property1");
        dictionary["Property1"].ShouldBe("Value");
        dictionary.ShouldContainKey("Property2");
        dictionary["Property2"].ShouldBe(123);
    }

    [Fact]
    public void Read_ObjectWithTrailingCommentAndCommentsAllowed_ReturnsObject()
    {
        // Arrange

        // JsonSerializerOptions rejects ReadCommentHandling.Allow, so the converter's Comment-token
        // handling is only reachable by driving a Utf8JsonReader directly with comments allowed.
        const string json = """
                            {
                              "Property1": "Value",
                              "Property2": 123
                              // trailing comment before the closing brace
                            }
                            """;

        var converter = new ObjectAsPrimitiveConverter();
        var reader = new Utf8JsonReader(
            System.Text.Encoding.UTF8.GetBytes(json),
            new JsonReaderOptions { CommentHandling = JsonCommentHandling.Allow });

        reader.Read();

        // Act
        var result = converter.Read(ref reader, typeof(object), new JsonSerializerOptions());

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("Property1");
        dictionary["Property1"].ShouldBe("Value");
        dictionary.ShouldContainKey("Property2");
        dictionary["Property2"].ShouldBe(123);
    }

    [Fact]
    public void Deserialize_ValidJsonWithCommentsAndCommentsDisabled_ThrowsException()
    {
        // Arrange
        const string json = """
                            {
                              // This is comment 1
                              "Property1": "Value",
                              // This is comment 2
                              "Property2": 123
                            }
                            """;

        // Act
        var exception = Record.Exception(() => JsonSerializer.Deserialize<object>(json, GetOptions()));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<JsonException>();
        exception.Message.ShouldStartWith("'/' is invalid after a value.");
    }

    [Fact]
    public void Deserialize_ValidJsonObjectWithArrayWithNullValue_ReturnsDeserializedObjectWithArrayWithNullValue()
    {
        // Arrange
        const string json = """
                            {
                              "NullProperty": null,
                              "StringProperty": "Hello, World!",
                              "IntProperty": -234,
                              "DecimalProperty": 123.456,
                              "ArrayProperty": [1, null, "abc", true]
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions());

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("NullProperty");
        dictionary["NullProperty"].ShouldBeNull();
        dictionary.ShouldContainKey("StringProperty");
        dictionary["StringProperty"].ShouldBe("Hello, World!");
        dictionary.ShouldContainKey("IntProperty");
        dictionary["IntProperty"].ShouldBeOfType<int>();
        dictionary["IntProperty"].ShouldBe(-234);
        dictionary.ShouldContainKey("DecimalProperty");
        dictionary["DecimalProperty"].ShouldBeOfType<decimal>();
        dictionary["DecimalProperty"].ShouldBe(123.456m);
        dictionary.ShouldContainKey("ArrayProperty");
        dictionary["ArrayProperty"].ShouldBeOfType<object[]>();

        var array = (object[])dictionary["ArrayProperty"];
        array.Length.ShouldBe(4);
        array[0].ShouldBe(1);
        array[1].ShouldBeNull();
        array[2].ShouldBe("abc");
        array[3].ShouldBe(true);
    }

    [Fact]
    public void Deserialize_ValidJsonObjectAndDefaultSettings_ReturnsDeserializedObject()
    {
        // Arrange
        const string json = """
                            {
                              "NullProperty": null,
                              "BoolFalseProperty": false,
                              "BoolTrueProperty": true,
                              "StringProperty": "Hello, World!",
                              "DateTimeOffsetProperty": "2024-06-01T12:34:56+00:00",
                              "DateTimeProperty": "2026-03-21T13:01:06",
                              "DateProperty": "2026-03-21",
                              "TimeProperty": "13:01:06",
                              "IntProperty": -234,
                              "DecimalProperty": 123.456,
                              "ArrayProperty": [1, "abc", true]
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions());

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("NullProperty");
        dictionary["NullProperty"].ShouldBeNull();
        dictionary.ShouldContainKey("BoolFalseProperty");
        dictionary["BoolFalseProperty"].ShouldBe(false);
        dictionary.ShouldContainKey("BoolTrueProperty");
        dictionary["BoolTrueProperty"].ShouldBe(true);
        dictionary.ShouldContainKey("StringProperty");
        dictionary["StringProperty"].ShouldBe("Hello, World!");
        dictionary.ShouldContainKey("DateTimeOffsetProperty");
        dictionary["DateTimeOffsetProperty"].ShouldBe("2024-06-01T12:34:56+00:00");
        dictionary.ShouldContainKey("DateTimeProperty");
        dictionary["DateTimeProperty"].ShouldBe("2026-03-21T13:01:06");
        dictionary.ShouldContainKey("DateProperty");
        dictionary["DateProperty"].ShouldBe("2026-03-21");
        dictionary.ShouldContainKey("TimeProperty");
        dictionary["TimeProperty"].ShouldBe("13:01:06");
        dictionary.ShouldContainKey("IntProperty");
        dictionary["IntProperty"].ShouldBeOfType<int>();
        dictionary["IntProperty"].ShouldBe(-234);
        dictionary.ShouldContainKey("DecimalProperty");
        dictionary["DecimalProperty"].ShouldBeOfType<decimal>();
        dictionary["DecimalProperty"].ShouldBe(123.456m);
        dictionary.ShouldContainKey("ArrayProperty");
        dictionary["ArrayProperty"].ShouldBeOfType<object[]>();

        var array = (object[])dictionary["ArrayProperty"];
        array.Length.ShouldBe(3);
        array[0].ShouldBe(1);
        array[1].ShouldBe("abc");
        array[2].ShouldBe(true);
    }

    [Fact]
    public void Deserialize_ValidJsonObjectWithBitInteger_ReturnsDeserializedObjectWithBigInteger()
    {
        // Arrange
        const string json = """
                            {
                              "BigIntegerProperty": 123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions());

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("BigIntegerProperty");
        dictionary["BigIntegerProperty"].ShouldBeOfType<BigInteger>();
        ((BigInteger)dictionary["BigIntegerProperty"]).ToString().ShouldBe("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
    }

    [Theory]
    [InlineData("123456789012345678901234567890", typeof(BigInteger))]
    [InlineData("123.456", typeof(decimal))]
    public void Read_NumberSplitAcrossBufferSegments_ParsesUsingDecodedText(string number, Type expectedType)
    {
        // Arrange

        // When a number token straddles two buffer segments, reader.HasValueSequence is true and the
        // converter must decode reader.ValueSequence (not call ToString() on it, which yields the type name).
        var bytes = System.Text.Encoding.UTF8.GetBytes(number);
        var splitAt = bytes.Length / 2;
        var first = new BufferSegment(bytes.AsMemory(0, splitAt));
        var last = first.Append(bytes.AsMemory(splitAt));
        var sequence = new System.Buffers.ReadOnlySequence<byte>(first, 0, last, last.Memory.Length);

        var reader = new Utf8JsonReader(sequence);
        reader.Read();
        reader.HasValueSequence.ShouldBeTrue();

        // Act
        var result = new ObjectAsPrimitiveConverter().Read(ref reader, typeof(object), new JsonSerializerOptions());

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType(expectedType);
        result.ToString().ShouldBe(number);
    }

    [Theory]
    [InlineData("1e5", 100000)]
    [InlineData("2E3", 2000)]
    [InlineData("-3e2", -300)]
    [InlineData("5e-1", 0.5)]
    [InlineData("1.5e3", 1500)]
    public void Deserialize_NumberInExponentNotation_ReturnsFloatingPointValue(string number, double expected)
    {
        // Arrange

        // Exponent-notation numbers without a decimal point (e.g. "1e5") are valid JSON numbers but were
        // previously misrouted to the integer parsers, which reject exponents, and threw "Cannot parse number".
        var json = $$"""{"Property":{{number}}}""";

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions());

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("Property");
        dictionary["Property"].ShouldBeOfType<decimal>();
        dictionary["Property"].ShouldBe((decimal)expected);
    }

    #endregion

    #region FloatFormat settings

    [Fact]
    public void Deserialize_ValidJsonObjectAndFloatFormatDouble_ReturnsDeserializedObjectWithDouble()
    {
        // Arrange
        const string json = """
                            {
                              "NullProperty": null,
                              "BoolFalseProperty": false,
                              "BoolTrueProperty": true,
                              "StringProperty": "Hello, World!",
                              "DateTimeOffsetProperty": "2024-06-01T12:34:56+00:00",
                              "DateTimeProperty": "2026-03-21T13:01:06",
                              "DateProperty": "2026-03-21",
                              "TimeProperty": "13:01:06",
                              "IntProperty": -234,
                              "DecimalProperty": 123.456,
                              "ArrayProperty": [1, "abc", true]
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(floatFormat: FloatFormat.Double));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("NullProperty");
        dictionary["NullProperty"].ShouldBeNull();
        dictionary.ShouldContainKey("BoolFalseProperty");
        dictionary["BoolFalseProperty"].ShouldBe(false);
        dictionary.ShouldContainKey("BoolTrueProperty");
        dictionary["BoolTrueProperty"].ShouldBe(true);
        dictionary.ShouldContainKey("StringProperty");
        dictionary["StringProperty"].ShouldBe("Hello, World!");
        dictionary.ShouldContainKey("DateTimeOffsetProperty");
        dictionary["DateTimeOffsetProperty"].ShouldBe("2024-06-01T12:34:56+00:00");
        dictionary.ShouldContainKey("DateTimeProperty");
        dictionary["DateTimeProperty"].ShouldBe("2026-03-21T13:01:06");
        dictionary.ShouldContainKey("DateProperty");
        dictionary["DateProperty"].ShouldBe("2026-03-21");
        dictionary.ShouldContainKey("TimeProperty");
        dictionary["TimeProperty"].ShouldBe("13:01:06");
        dictionary.ShouldContainKey("IntProperty");
        dictionary["IntProperty"].ShouldBeOfType<int>();
        dictionary["IntProperty"].ShouldBe(-234);
        dictionary.ShouldContainKey("DecimalProperty");
        dictionary["DecimalProperty"].ShouldBeOfType<double>();
        dictionary["DecimalProperty"].ShouldBe(123.456);
        dictionary.ShouldContainKey("ArrayProperty");
        dictionary["ArrayProperty"].ShouldBeOfType<object[]>();

        var array = (object[])dictionary["ArrayProperty"];
        array.Length.ShouldBe(3);
        array[0].ShouldBe(1);
        array[1].ShouldBe("abc");
        array[2].ShouldBe(true);
    }

    [Fact]
    public void Deserialize_ValidJsonObjectAndFloatFormatFloat_ReturnsDeserializedObjectWithDouble()
    {
        // Arrange
        const string json = """
                            {
                              "NullProperty": null,
                              "BoolFalseProperty": false,
                              "BoolTrueProperty": true,
                              "StringProperty": "Hello, World!",
                              "DateTimeOffsetProperty": "2024-06-01T12:34:56+00:00",
                              "DateTimeProperty": "2026-03-21T13:01:06",
                              "DateProperty": "2026-03-21",
                              "TimeProperty": "13:01:06",
                              "IntProperty": -234,
                              "DecimalProperty": 123.456,
                              "ArrayProperty": [1, "abc", true]
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(floatFormat: FloatFormat.Single));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("NullProperty");
        dictionary["NullProperty"].ShouldBeNull();
        dictionary.ShouldContainKey("BoolFalseProperty");
        dictionary["BoolFalseProperty"].ShouldBe(false);
        dictionary.ShouldContainKey("BoolTrueProperty");
        dictionary["BoolTrueProperty"].ShouldBe(true);
        dictionary.ShouldContainKey("StringProperty");
        dictionary["StringProperty"].ShouldBe("Hello, World!");
        dictionary.ShouldContainKey("DateTimeOffsetProperty");
        dictionary["DateTimeOffsetProperty"].ShouldBe("2024-06-01T12:34:56+00:00");
        dictionary.ShouldContainKey("DateTimeProperty");
        dictionary["DateTimeProperty"].ShouldBe("2026-03-21T13:01:06");
        dictionary.ShouldContainKey("DateProperty");
        dictionary["DateProperty"].ShouldBe("2026-03-21");
        dictionary.ShouldContainKey("TimeProperty");
        dictionary["TimeProperty"].ShouldBe("13:01:06");
        dictionary.ShouldContainKey("IntProperty");
        dictionary["IntProperty"].ShouldBeOfType<int>();
        dictionary["IntProperty"].ShouldBe(-234);
        dictionary.ShouldContainKey("DecimalProperty");
        dictionary["DecimalProperty"].ShouldBeOfType<float>();
        dictionary["DecimalProperty"].ShouldBe(123.456f);
        dictionary.ShouldContainKey("ArrayProperty");
        dictionary["ArrayProperty"].ShouldBeOfType<object[]>();

        var array = (object[])dictionary["ArrayProperty"];
        array.Length.ShouldBe(3);
        array[0].ShouldBe(1);
        array[1].ShouldBe("abc");
        array[2].ShouldBe(true);
    }

    #endregion

    #region UnknownNumberFormat settings

    [Fact]
    public void Deserialize_ValidJsonObjectWithUnknownNumber_ThrowsException()
    {
        // Arrange
        const string json = """
                            {
                              "NullProperty": null,
                              "BoolFalseProperty": false,
                              "BoolTrueProperty": true,
                              "StringProperty": "Hello, World!",
                              "DateTimeOffsetProperty": "2024-06-01T12:34:56+00:00",
                              "DateTimeProperty": "2026-03-21T13:01:06",
                              "DateProperty": "2026-03-21",
                              "TimeProperty": "13:01:06",
                              "IntProperty": -234,
                              "DecimalProperty": 123.456,
                              "UnknownNumberProperty": 1.0e100000,
                              "ArrayProperty": [1, "abc", true]
                            }
                            """;

        // Act
        var exception = Record.Exception(() => JsonSerializer.Deserialize<object>(json, GetOptions()));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<JsonException>();
        exception.Message.ShouldBe("Cannot parse number: ‘1.0e100000’");
    }

    [Fact]
    public void Deserialize_ValidJsonObjectWithUnknownNumberAndUnknownNumberFormatJsonElement_ReturnsDeserializedObjectWithJsonElement()
    {
        // Arrange
        const string json = """
                            {
                              "NullProperty": null,
                              "BoolFalseProperty": false,
                              "BoolTrueProperty": true,
                              "StringProperty": "Hello, World!",
                              "DateTimeOffsetProperty": "2024-06-01T12:34:56+00:00",
                              "DateTimeProperty": "2026-03-21T13:01:06",
                              "DateProperty": "2026-03-21",
                              "TimeProperty": "13:01:06",
                              "IntProperty": -234,
                              "DecimalProperty": 123.456,
                              "UnknownNumberProperty": 1.0e100000,
                              "ArrayProperty": [1, "abc", true]
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(unknownNumberFormat: UnknownNumberFormat.JsonElement));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("NullProperty");
        dictionary["NullProperty"].ShouldBeNull();
        dictionary.ShouldContainKey("BoolFalseProperty");
        dictionary["BoolFalseProperty"].ShouldBe(false);
        dictionary.ShouldContainKey("BoolTrueProperty");
        dictionary["BoolTrueProperty"].ShouldBe(true);
        dictionary.ShouldContainKey("StringProperty");
        dictionary["StringProperty"].ShouldBe("Hello, World!");
        dictionary.ShouldContainKey("DateTimeOffsetProperty");
        dictionary["DateTimeOffsetProperty"].ShouldBe("2024-06-01T12:34:56+00:00");
        dictionary.ShouldContainKey("DateTimeProperty");
        dictionary["DateTimeProperty"].ShouldBe("2026-03-21T13:01:06");
        dictionary.ShouldContainKey("DateProperty");
        dictionary["DateProperty"].ShouldBe("2026-03-21");
        dictionary.ShouldContainKey("TimeProperty");
        dictionary["TimeProperty"].ShouldBe("13:01:06");
        dictionary.ShouldContainKey("IntProperty");
        dictionary["IntProperty"].ShouldBeOfType<int>();
        dictionary["IntProperty"].ShouldBe(-234);
        dictionary.ShouldContainKey("DecimalProperty");
        dictionary["DecimalProperty"].ShouldBeOfType<decimal>();
        dictionary["DecimalProperty"].ShouldBe(123.456m);
        dictionary.ShouldContainKey("UnknownNumberProperty");
        dictionary["UnknownNumberProperty"].ShouldBeOfType<JsonElement>();

        var jsonElement = (JsonElement)dictionary["UnknownNumberProperty"];
        jsonElement.ValueKind.ShouldBe(JsonValueKind.Number);
        jsonElement.ToString().ShouldBe("1.0e100000");

        dictionary.ShouldContainKey("ArrayProperty");
        dictionary["ArrayProperty"].ShouldBeOfType<object[]>();

        var array = (object[])dictionary["ArrayProperty"];
        array.Length.ShouldBe(3);
        array[0].ShouldBe(1);
        array[1].ShouldBe("abc");
        array[2].ShouldBe(true);
    }

    #endregion

    #region DetectDateTime settings

    [Fact]
    public void Deserialize_ValidJsonObjectAndDetectDateTimeNone_ReturnsDeserializedObjectWithTextValues()
    {
        // Arrange
        const string json = """
                            {
                              "NullProperty": null,
                              "BoolFalseProperty": false,
                              "BoolTrueProperty": true,
                              "StringProperty": "Hello, World!",
                              "DateTimeOffsetProperty": "2024-06-01T12:34:56+00:00",
                              "DateTimeProperty": "2026-03-21T13:01:06",
                              "DateProperty": "2026-03-21",
                              "TimeProperty": "13:01:06",
                              "IntProperty": -234,
                              "DecimalProperty": 123.456,
                              "ArrayProperty": [1, "abc", true]
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(detectDateTimeOffset: DetectDateTime.None));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("NullProperty");
        dictionary["NullProperty"].ShouldBeNull();
        dictionary.ShouldContainKey("BoolFalseProperty");
        dictionary["BoolFalseProperty"].ShouldBe(false);
        dictionary.ShouldContainKey("BoolTrueProperty");
        dictionary["BoolTrueProperty"].ShouldBe(true);
        dictionary.ShouldContainKey("StringProperty");
        dictionary["StringProperty"].ShouldBe("Hello, World!");
        dictionary.ShouldContainKey("DateTimeOffsetProperty");
        dictionary["DateTimeOffsetProperty"].ShouldBe("2024-06-01T12:34:56+00:00");
        dictionary.ShouldContainKey("DateTimeProperty");
        dictionary["DateTimeProperty"].ShouldBe("2026-03-21T13:01:06");
        dictionary.ShouldContainKey("DateProperty");
        dictionary["DateProperty"].ShouldBe("2026-03-21");
        dictionary.ShouldContainKey("TimeProperty");
        dictionary["TimeProperty"].ShouldBe("13:01:06");
        dictionary.ShouldContainKey("IntProperty");
        dictionary["IntProperty"].ShouldBeOfType<int>();
        dictionary["IntProperty"].ShouldBe(-234);
        dictionary.ShouldContainKey("DecimalProperty");
        dictionary["DecimalProperty"].ShouldBeOfType<decimal>();
        dictionary["DecimalProperty"].ShouldBe(123.456m);
        dictionary.ShouldContainKey("ArrayProperty");
        dictionary["ArrayProperty"].ShouldBeOfType<object[]>();

        var array = (object[])dictionary["ArrayProperty"];
        array.Length.ShouldBe(3);
        array[0].ShouldBe(1);
        array[1].ShouldBe("abc");
        array[2].ShouldBe(true);
    }

    [Theory]
    [InlineData("2024-06-01T12:34:56+04:00")]
    [InlineData("2024-06-01T12:34:56 +04:00")]
    [InlineData("2024-06-01T12:34:56+0400")]
    [InlineData("2024-06-01T12:34:56+04")]
    [InlineData("2024-06-01T12:34:56.000+04")]
    [InlineData("2024-06-01T12:34:56.0000000+04:00")]
    [InlineData("2024/06/01 12:34:56.0000000 +04:00")]
    [InlineData("2024/6/1 12:34:56.0000000 +04:00")]
    [InlineData("Sat Jun 01 2024 12:34:56 +04:00")]
    public void Deserialize_ValidJsonObjectAndDetectDateTimeOffset4_ReturnsDeserializedObjectWithDateTimeOffset(string dateTimeOffset)
    {
        // Arrange
        var json = $$"""
                     {
                       "NullProperty": null,
                       "BoolFalseProperty": false,
                       "BoolTrueProperty": true,
                       "StringProperty": "Hello, World!",
                       "DateTimeOffsetProperty": "{{dateTimeOffset}}",
                       "DateTimeProperty": "2026-03-21T13:01:06",
                       "DateProperty": "2026-03-21",
                       "TimeProperty": "13:01:06",
                       "IntProperty": -234,
                       "DecimalProperty": 123.456,
                       "ArrayProperty": [1, "abc", true]
                     }
                     """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(detectDateTimeOffset: DetectDateTime.DateTimeOffset));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("NullProperty");
        dictionary["NullProperty"].ShouldBeNull();
        dictionary.ShouldContainKey("BoolFalseProperty");
        dictionary["BoolFalseProperty"].ShouldBe(false);
        dictionary.ShouldContainKey("BoolTrueProperty");
        dictionary["BoolTrueProperty"].ShouldBe(true);
        dictionary.ShouldContainKey("StringProperty");
        dictionary["StringProperty"].ShouldBe("Hello, World!");
        dictionary.ShouldContainKey("DateTimeOffsetProperty");
        dictionary["DateTimeOffsetProperty"].ShouldBeOfType<DateTimeOffset>();
        dictionary["DateTimeOffsetProperty"].ShouldBe(new DateTimeOffset(2024, 6, 1, 12, 34, 56, TimeSpan.FromHours(4)));
        dictionary.ShouldContainKey("DateTimeProperty");
        dictionary["DateTimeProperty"].ShouldBe("2026-03-21T13:01:06");
        dictionary.ShouldContainKey("DateProperty");
        dictionary["DateProperty"].ShouldBe("2026-03-21");
        dictionary.ShouldContainKey("TimeProperty");
        dictionary["TimeProperty"].ShouldBe("13:01:06");
        dictionary.ShouldContainKey("IntProperty");
        dictionary["IntProperty"].ShouldBeOfType<int>();
        dictionary["IntProperty"].ShouldBe(-234);
        dictionary.ShouldContainKey("DecimalProperty");
        dictionary["DecimalProperty"].ShouldBeOfType<decimal>();
        dictionary["DecimalProperty"].ShouldBe(123.456m);
        dictionary.ShouldContainKey("ArrayProperty");
        dictionary["ArrayProperty"].ShouldBeOfType<object[]>();

        var array = (object[])dictionary["ArrayProperty"];
        array.Length.ShouldBe(3);
        array[0].ShouldBe(1);
        array[1].ShouldBe("abc");
        array[2].ShouldBe(true);
    }

    [Theory]
    [InlineData("2024-06-01T12:34:56+00:00")]
    [InlineData("2024-06-01T12:34:56 +00:00")]
    [InlineData("2024-06-01T12:34:56+0000")]
    [InlineData("2024-06-01T12:34:56Z")]
    [InlineData("2024-06-01T12:34:56.000Z")]
    [InlineData("2024-06-01T12:34:56.0000000+00:00")]
    [InlineData("2024/06/01 12:34:56.0000000 +00:00")]
    [InlineData("2024/6/1 12:34:56.0000000 +00:00")]
    [InlineData("Sat Jun 01 2024 12:34:56 +00:00")]
    public void Deserialize_ValidJsonObjectAndDetectDateTimeOffsetZ_ReturnsDeserializedObjectWithDateTimeOffset(string dateTimeOffset)
    {
        // Arrange
        var json = $$"""
                     {
                       "NullProperty": null,
                       "BoolFalseProperty": false,
                       "BoolTrueProperty": true,
                       "StringProperty": "Hello, World!",
                       "DateTimeOffsetProperty": "{{dateTimeOffset}}",
                       "DateTimeProperty": "2026-03-21T13:01:06",
                       "DateProperty": "2026-03-21",
                       "TimeProperty": "13:01:06",
                       "IntProperty": -234,
                       "DecimalProperty": 123.456,
                       "ArrayProperty": [1, "abc", true]
                     }
                     """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(detectDateTimeOffset: DetectDateTime.DateTimeOffset));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("NullProperty");
        dictionary["NullProperty"].ShouldBeNull();
        dictionary.ShouldContainKey("BoolFalseProperty");
        dictionary["BoolFalseProperty"].ShouldBe(false);
        dictionary.ShouldContainKey("BoolTrueProperty");
        dictionary["BoolTrueProperty"].ShouldBe(true);
        dictionary.ShouldContainKey("StringProperty");
        dictionary["StringProperty"].ShouldBe("Hello, World!");
        dictionary.ShouldContainKey("DateTimeOffsetProperty");
        dictionary["DateTimeOffsetProperty"].ShouldBeOfType<DateTimeOffset>();
        dictionary["DateTimeOffsetProperty"].ShouldBe(new DateTimeOffset(2024, 6, 1, 12, 34, 56, TimeSpan.Zero));
        dictionary.ShouldContainKey("DateTimeProperty");
        dictionary["DateTimeProperty"].ShouldBe("2026-03-21T13:01:06");
        dictionary.ShouldContainKey("DateProperty");
        dictionary["DateProperty"].ShouldBe("2026-03-21");
        dictionary.ShouldContainKey("TimeProperty");
        dictionary["TimeProperty"].ShouldBe("13:01:06");
        dictionary.ShouldContainKey("IntProperty");
        dictionary["IntProperty"].ShouldBeOfType<int>();
        dictionary["IntProperty"].ShouldBe(-234);
        dictionary.ShouldContainKey("DecimalProperty");
        dictionary["DecimalProperty"].ShouldBeOfType<decimal>();
        dictionary["DecimalProperty"].ShouldBe(123.456m);
        dictionary.ShouldContainKey("ArrayProperty");
        dictionary["ArrayProperty"].ShouldBeOfType<object[]>();

        var array = (object[])dictionary["ArrayProperty"];
        array.Length.ShouldBe(3);
        array[0].ShouldBe(1);
        array[1].ShouldBe("abc");
        array[2].ShouldBe(true);
    }

    [Theory]
    [InlineData("not a valid date time offset 123")]
    [InlineData("2024-02-30T12:34:56+04:00")]
    [InlineData("before 2024-06-01T12:34:56+04:00 after")]
    public void Deserialize_ValidJsonObjectAndDetectDateTimeOffsetWithInvalidValue_ReturnsDeserializedObjectWithTextValue(string dateTimeOffset)
    {
        // Arrange
        var json = $$"""
                     {
                       "DateTimeOffsetProperty": "{{dateTimeOffset}}"
                     }
                     """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(detectDateTimeOffset: DetectDateTime.DateTimeOffset));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("DateTimeOffsetProperty");
        dictionary["DateTimeOffsetProperty"].ShouldBeOfType<string>();
        dictionary["DateTimeOffsetProperty"].ShouldBe(dateTimeOffset);
    }

    [Theory]
    [InlineData("2026-03-21T13:01:06")]
    [InlineData("2026-03-21T13:01:06.0000000")]
    [InlineData("2026-03-21T13:01:06.000")]
    [InlineData("2026-03-21 13:01:06")]
    [InlineData("2026-03-21 13:01:06.00000")]
    public void Deserialize_ValidJsonObjectAndDetectDateTime_ReturnsDeserializedObjectWithDateTime(string dateTime)
    {
        // Arrange
        var json = $$"""
                     {
                       "NullProperty": null,
                       "BoolFalseProperty": false,
                       "BoolTrueProperty": true,
                       "StringProperty": "Hello, World!",
                       "DateTimeOffsetProperty": "2024-06-01T12:34:56+00:00",
                       "DateTimeProperty": "{{dateTime}}",
                       "DateProperty": "2026-03-21",
                       "TimeProperty": "13:01:06",
                       "IntProperty": -234,
                       "DecimalProperty": 123.456,
                       "ArrayProperty": [1, "abc", true]
                     }
                     """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(detectDateTimeOffset: DetectDateTime.DateTime));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("NullProperty");
        dictionary["NullProperty"].ShouldBeNull();
        dictionary.ShouldContainKey("BoolFalseProperty");
        dictionary["BoolFalseProperty"].ShouldBe(false);
        dictionary.ShouldContainKey("BoolTrueProperty");
        dictionary["BoolTrueProperty"].ShouldBe(true);
        dictionary.ShouldContainKey("StringProperty");
        dictionary["StringProperty"].ShouldBe("Hello, World!");
        dictionary.ShouldContainKey("DateTimeOffsetProperty");
        dictionary["DateTimeOffsetProperty"].ShouldBe("2024-06-01T12:34:56+00:00");
        dictionary.ShouldContainKey("DateTimeProperty");
        dictionary["DateTimeProperty"].ShouldBeOfType<DateTime>();
        dictionary["DateTimeProperty"].ShouldBe(new DateTime(2026, 3, 21, 13, 1, 6));
        dictionary.ShouldContainKey("DateProperty");
        dictionary["DateProperty"].ShouldBe("2026-03-21");
        dictionary.ShouldContainKey("TimeProperty");
        dictionary["TimeProperty"].ShouldBe("13:01:06");
        dictionary.ShouldContainKey("IntProperty");
        dictionary["IntProperty"].ShouldBeOfType<int>();
        dictionary["IntProperty"].ShouldBe(-234);
        dictionary.ShouldContainKey("DecimalProperty");
        dictionary["DecimalProperty"].ShouldBeOfType<decimal>();
        dictionary["DecimalProperty"].ShouldBe(123.456m);
        dictionary.ShouldContainKey("ArrayProperty");
        dictionary["ArrayProperty"].ShouldBeOfType<object[]>();

        var array = (object[])dictionary["ArrayProperty"];
        array.Length.ShouldBe(3);
        array[0].ShouldBe(1);
        array[1].ShouldBe("abc");
        array[2].ShouldBe(true);
    }

    [Theory]
    [InlineData("not a valid date time 123")]
    [InlineData("2026-02-30T13:01:06")]
    [InlineData("before 2026-03-21T13:01:06 after")]
    public void Deserialize_ValidJsonObjectAndDetectDateTimeWithInvalidValue_ReturnsDeserializedObjectWithTextValue(string dateTime)
    {
        // Arrange
        var json = $$"""
                     {
                       "DateTimeProperty": "{{dateTime}}"
                     }
                     """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(detectDateTimeOffset: DetectDateTime.DateTime));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("DateTimeProperty");
        dictionary["DateTimeProperty"].ShouldBeOfType<string>();
        dictionary["DateTimeProperty"].ShouldBe(dateTime);
    }

    [Theory]
    [InlineData("2026-03-21")]
    [InlineData("2026-3-21")]
    [InlineData("2026/03/21")]
    [InlineData("2026/3/21")]
    public void Deserialize_ValidJsonObjectAndDetectDateOnly_ReturnsDeserializedObjectWithDateOnly(string dateOnly)
    {
        // Arrange
        var json = $$"""
                     {
                       "NullProperty": null,
                       "BoolFalseProperty": false,
                       "BoolTrueProperty": true,
                       "StringProperty": "Hello, World!",
                       "DateTimeOffsetProperty": "2024-06-01T12:34:56+00:00",
                       "DateTimeProperty": "2026-03-21T13:01:06",
                       "DateProperty": "{{dateOnly}}",
                       "TimeProperty": "13:01:06",
                       "IntProperty": -234,
                       "DecimalProperty": 123.456,
                       "ArrayProperty": [1, "abc", true]
                     }
                     """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(detectDateTimeOffset: DetectDateTime.DateOnly));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("NullProperty");
        dictionary["NullProperty"].ShouldBeNull();
        dictionary.ShouldContainKey("BoolFalseProperty");
        dictionary["BoolFalseProperty"].ShouldBe(false);
        dictionary.ShouldContainKey("BoolTrueProperty");
        dictionary["BoolTrueProperty"].ShouldBe(true);
        dictionary.ShouldContainKey("StringProperty");
        dictionary["StringProperty"].ShouldBe("Hello, World!");
        dictionary.ShouldContainKey("DateTimeOffsetProperty");
        dictionary["DateTimeOffsetProperty"].ShouldBe("2024-06-01T12:34:56+00:00");
        dictionary.ShouldContainKey("DateTimeProperty");
        dictionary["DateTimeProperty"].ShouldBe("2026-03-21T13:01:06");
        dictionary.ShouldContainKey("DateProperty");
        dictionary["DateProperty"].ShouldBeOfType<DateOnly>();
        dictionary["DateProperty"].ShouldBe(new DateOnly(2026, 3, 21));
        dictionary.ShouldContainKey("TimeProperty");
        dictionary["TimeProperty"].ShouldBe("13:01:06");
        dictionary.ShouldContainKey("IntProperty");
        dictionary["IntProperty"].ShouldBeOfType<int>();
        dictionary["IntProperty"].ShouldBe(-234);
        dictionary.ShouldContainKey("DecimalProperty");
        dictionary["DecimalProperty"].ShouldBeOfType<decimal>();
        dictionary["DecimalProperty"].ShouldBe(123.456m);
        dictionary.ShouldContainKey("ArrayProperty");
        dictionary["ArrayProperty"].ShouldBeOfType<object[]>();

        var array = (object[])dictionary["ArrayProperty"];
        array.Length.ShouldBe(3);
        array[0].ShouldBe(1);
        array[1].ShouldBe("abc");
        array[2].ShouldBe(true);
    }

    [Theory]
    [InlineData("not a valid date 123")]
    [InlineData("2026-02-30")]
    [InlineData("before 2026-03-21 after")]
    public void Deserialize_ValidJsonObjectAndDetectDateOnlyWithInvalidValue_ReturnsDeserializedObjectWithTextValue(string dateOnly)
    {
        // Arrange
        var json = $$"""
                     {
                       "DateProperty": "{{dateOnly}}"
                     }
                     """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(detectDateTimeOffset: DetectDateTime.DateOnly));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("DateProperty");
        dictionary["DateProperty"].ShouldBeOfType<string>();
        dictionary["DateProperty"].ShouldBe(dateOnly);
    }

    [Theory]
    [InlineData("13:01:06")]
    [InlineData("13:01:06.00")]
    [InlineData("13:01:06.000")]
    [InlineData("13:01:06.00000")]
    [InlineData("13:01:06.0000000")]
    [InlineData("1:01:06 PM")]
    [InlineData("01:01:06 PM")]
    public void Deserialize_ValidJsonObjectAndDetectTimeOnly_ReturnsDeserializedObjectWithTimeOnly(string timeOnly)
    {
        // Arrange
        var json = $$"""
                     {
                       "TimeProperty": "{{timeOnly}}"
                     }
                     """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(detectDateTimeOffset: DetectDateTime.TimeOnly));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("TimeProperty");
        dictionary["TimeProperty"].ShouldBeOfType<TimeOnly>();
        dictionary["TimeProperty"].ShouldBe(new TimeOnly(13, 1, 6));
    }

    [Theory]
    [InlineData("13:01")]
    [InlineData("13:01:00")]
    [InlineData("13:01:00.00")]
    [InlineData("13:01:00.000")]
    [InlineData("13:01:00.00000")]
    [InlineData("13:01:00.0000000")]
    [InlineData("1:01 PM")]
    [InlineData("01:01 PM")]
    [InlineData("1:01:00 PM")]
    [InlineData("01:01:00 PM")]
    public void Deserialize_ValidJsonObjectAndDetectTimeOnlyWithoutSeconds_ReturnsDeserializedObjectWithTimeOnly(string timeOnly)
    {
        // Arrange
        var json = $$"""
                     {
                       "NullProperty": null,
                       "BoolFalseProperty": false,
                       "BoolTrueProperty": true,
                       "StringProperty": "Hello, World!",
                       "DateTimeOffsetProperty": "2024-06-01T12:34:56+00:00",
                       "DateTimeProperty": "2026-03-21T13:01:06",
                       "DateProperty": "2026-03-21",
                       "TimeProperty": "{{timeOnly}}",
                       "IntProperty": -234,
                       "DecimalProperty": 123.456,
                       "ArrayProperty": [1, "abc", true]
                     }
                     """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(detectDateTimeOffset: DetectDateTime.TimeOnly));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("NullProperty");
        dictionary["NullProperty"].ShouldBeNull();
        dictionary.ShouldContainKey("BoolFalseProperty");
        dictionary["BoolFalseProperty"].ShouldBe(false);
        dictionary.ShouldContainKey("BoolTrueProperty");
        dictionary["BoolTrueProperty"].ShouldBe(true);
        dictionary.ShouldContainKey("StringProperty");
        dictionary["StringProperty"].ShouldBe("Hello, World!");
        dictionary.ShouldContainKey("DateTimeOffsetProperty");
        dictionary["DateTimeOffsetProperty"].ShouldBe("2024-06-01T12:34:56+00:00");
        dictionary.ShouldContainKey("DateTimeProperty");
        dictionary["DateTimeProperty"].ShouldBe("2026-03-21T13:01:06");
        dictionary.ShouldContainKey("DateProperty");
        dictionary["DateProperty"].ShouldBe("2026-03-21");
        dictionary.ShouldContainKey("TimeProperty");
        dictionary["TimeProperty"].ShouldBeOfType<TimeOnly>();
        dictionary["TimeProperty"].ShouldBe(new TimeOnly(13, 1));
        dictionary.ShouldContainKey("IntProperty");
        dictionary["IntProperty"].ShouldBeOfType<int>();
        dictionary["IntProperty"].ShouldBe(-234);
        dictionary.ShouldContainKey("DecimalProperty");
        dictionary["DecimalProperty"].ShouldBeOfType<decimal>();
        dictionary["DecimalProperty"].ShouldBe(123.456m);
        dictionary.ShouldContainKey("ArrayProperty");
        dictionary["ArrayProperty"].ShouldBeOfType<object[]>();

        var array = (object[])dictionary["ArrayProperty"];
        array.Length.ShouldBe(3);
        array[0].ShouldBe(1);
        array[1].ShouldBe("abc");
        array[2].ShouldBe(true);
    }

    [Theory]
    [InlineData("not a valid time 123")]
    [InlineData("25:61:06")]
    [InlineData("before 13:01:06 after")]
    public void Deserialize_ValidJsonObjectAndDetectTimeOnlyWithInvalidValue_ReturnsDeserializedObjectWithTextValue(string timeOnly)
    {
        // Arrange
        var json = $$"""
                     {
                       "TimeProperty": "{{timeOnly}}"
                     }
                     """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(detectDateTimeOffset: DetectDateTime.TimeOnly));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("TimeProperty");
        dictionary["TimeProperty"].ShouldBeOfType<string>();
        dictionary["TimeProperty"].ShouldBe(timeOnly);
    }

    [Fact]
    public void Deserialize_ValidJsonObjectAndDetectDateTimeAll_ReturnsDeserializedObjectWithMappedValues()
    {
        // Arrange
        const string json = """
                            {
                              "NullProperty": null,
                              "BoolFalseProperty": false,
                              "BoolTrueProperty": true,
                              "StringProperty": "Hello, World!",
                              "DateTimeOffsetProperty": "2024-06-01T12:34:56-05:00",
                              "DateTimeProperty": "2026-03-21T13:01:06",
                              "DateProperty": "2026-03-21",
                              "TimeProperty": "13:01:06",
                              "IntProperty": -234,
                              "DecimalProperty": 123.456,
                              "ArrayProperty": [1, "abc", true]
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(detectDateTimeOffset: DetectDateTime.DateTimeOffset | DetectDateTime.DateTime | DetectDateTime.DateOnly));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("NullProperty");
        dictionary["NullProperty"].ShouldBeNull();
        dictionary.ShouldContainKey("BoolFalseProperty");
        dictionary["BoolFalseProperty"].ShouldBe(false);
        dictionary.ShouldContainKey("BoolTrueProperty");
        dictionary["BoolTrueProperty"].ShouldBe(true);
        dictionary.ShouldContainKey("StringProperty");
        dictionary["StringProperty"].ShouldBe("Hello, World!");
        dictionary.ShouldContainKey("DateTimeOffsetProperty");
        dictionary["DateTimeOffsetProperty"].ShouldBeOfType<DateTimeOffset>();
        dictionary["DateTimeOffsetProperty"].ShouldBe(new DateTimeOffset(2024, 6, 1, 12, 34, 56, TimeSpan.FromHours(-5)));
        dictionary.ShouldContainKey("DateTimeProperty");
        dictionary["DateTimeProperty"].ShouldBeOfType<DateTime>();
        dictionary["DateTimeProperty"].ShouldBe(new DateTime(2026, 3, 21, 13, 1, 6));
        dictionary.ShouldContainKey("DateProperty");
        dictionary["DateProperty"].ShouldBeOfType<DateOnly>();
        dictionary["DateProperty"].ShouldBe(new DateOnly(2026, 3, 21));
        dictionary.ShouldContainKey("TimeProperty");
        dictionary["TimeProperty"].ShouldBe("13:01:06");
        dictionary.ShouldContainKey("IntProperty");
        dictionary["IntProperty"].ShouldBeOfType<int>();
        dictionary["IntProperty"].ShouldBe(-234);
        dictionary.ShouldContainKey("DecimalProperty");
        dictionary["DecimalProperty"].ShouldBeOfType<decimal>();
        dictionary["DecimalProperty"].ShouldBe(123.456m);
        dictionary.ShouldContainKey("ArrayProperty");
        dictionary["ArrayProperty"].ShouldBeOfType<object[]>();

        var array = (object[])dictionary["ArrayProperty"];
        array.Length.ShouldBe(3);
        array[0].ShouldBe(1);
        array[1].ShouldBe("abc");
        array[2].ShouldBe(true);
    }

    [Fact]
    public void Deserialize_ValidJsonObjectAndDetectDateTimeAndTimeAll_ReturnsDeserializedObjectWithMappedValues()
    {
        // Arrange
        const string json = """
                            {
                              "NullProperty": null,
                              "BoolFalseProperty": false,
                              "BoolTrueProperty": true,
                              "StringProperty": "Hello, World!",
                              "DateTimeOffsetProperty": "2024-06-01T12:34:56-05:00",
                              "DateTimeProperty": "2026-03-21T13:01:06",
                              "DateProperty": "2026-03-21",
                              "TimeProperty": "13:01:06",
                              "IntProperty": -234,
                              "DecimalProperty": 123.456,
                              "ArrayProperty": [1, "abc", true]
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(detectDateTimeOffset: DetectDateTime.DateTimeOffset | DetectDateTime.DateTime | DetectDateTime.DateOnly | DetectDateTime.TimeOnly));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Dictionary<string, object>>();

        var dictionary = (Dictionary<string, object>)result;
        dictionary.ShouldContainKey("NullProperty");
        dictionary["NullProperty"].ShouldBeNull();
        dictionary.ShouldContainKey("BoolFalseProperty");
        dictionary["BoolFalseProperty"].ShouldBe(false);
        dictionary.ShouldContainKey("BoolTrueProperty");
        dictionary["BoolTrueProperty"].ShouldBe(true);
        dictionary.ShouldContainKey("StringProperty");
        dictionary["StringProperty"].ShouldBe("Hello, World!");
        dictionary.ShouldContainKey("DateTimeOffsetProperty");
        dictionary["DateTimeOffsetProperty"].ShouldBeOfType<DateTimeOffset>();
        dictionary["DateTimeOffsetProperty"].ShouldBe(new DateTimeOffset(2024, 6, 1, 12, 34, 56, TimeSpan.FromHours(-5)));
        dictionary.ShouldContainKey("DateTimeProperty");
        dictionary["DateTimeProperty"].ShouldBeOfType<DateTime>();
        dictionary["DateTimeProperty"].ShouldBe(new DateTime(2026, 3, 21, 13, 1, 6));
        dictionary.ShouldContainKey("DateProperty");
        dictionary["DateProperty"].ShouldBeOfType<DateOnly>();
        dictionary["DateProperty"].ShouldBe(new DateOnly(2026, 3, 21));
        dictionary.ShouldContainKey("TimeProperty");
        dictionary["TimeProperty"].ShouldBeOfType<TimeOnly>();
        dictionary["TimeProperty"].ShouldBe(new TimeOnly(13, 1, 6));
        dictionary.ShouldContainKey("IntProperty");
        dictionary["IntProperty"].ShouldBeOfType<int>();
        dictionary["IntProperty"].ShouldBe(-234);
        dictionary.ShouldContainKey("DecimalProperty");
        dictionary["DecimalProperty"].ShouldBeOfType<decimal>();
        dictionary["DecimalProperty"].ShouldBe(123.456m);
        dictionary.ShouldContainKey("ArrayProperty");
        dictionary["ArrayProperty"].ShouldBeOfType<object[]>();

        var array = (object[])dictionary["ArrayProperty"];
        array.Length.ShouldBe(3);
        array[0].ShouldBe(1);
        array[1].ShouldBe("abc");
        array[2].ShouldBe(true);
    }


    #endregion

    #region ObjectFormat settings

    [Fact]
    public void Deserialize_ValidJsonObjectAndObjectFormatExpando_ReturnsDeserializedExpandoObject()
    {
        // Arrange
        const string json = """
                            {
                              "NullProperty": null,
                              "BoolFalseProperty": false,
                              "BoolTrueProperty": true,
                              "StringProperty": "Hello, World!",
                              "DateTimeOffsetProperty": "2024-06-01T12:34:56+00:00",
                              "DateTimeProperty": "2026-03-21T13:01:06",
                              "DateProperty": "2026-03-21",
                              "TimeProperty": "13:01:06",
                              "IntProperty": -234,
                              "DecimalProperty": 123.456,
                              "ArrayProperty": [1, "abc", true]
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(objectFormat: ObjectFormat.Expando));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<ExpandoObject>();

        dynamic expandoObject = (ExpandoObject) result;
        ShouldBeNullExtensions.ShouldBeNull<object?>(expandoObject.NullProperty);
        ShouldBeTestExtensions.ShouldBe(expandoObject.BoolFalseProperty, false);
        ShouldBeTestExtensions.ShouldBe(expandoObject.BoolTrueProperty, true);
        ShouldBeTestExtensions.ShouldBe<string>(expandoObject.StringProperty, "Hello, World!");
        ShouldBeTestExtensions.ShouldBe<string>(expandoObject.DateTimeOffsetProperty, "2024-06-01T12:34:56+00:00");
        ShouldBeTestExtensions.ShouldBe<string>(expandoObject.DateTimeProperty, "2026-03-21T13:01:06");
        ShouldBeTestExtensions.ShouldBe<string>(expandoObject.DateProperty, "2026-03-21");
        ShouldBeTestExtensions.ShouldBe<string>(expandoObject.TimeProperty, "13:01:06");
        ShouldBeTestExtensions.ShouldBeOfType<int>(expandoObject.IntProperty);
        ShouldBeTestExtensions.ShouldBe<int>(expandoObject.IntProperty, -234);
        ShouldBeTestExtensions.ShouldBeOfType<decimal>(expandoObject.DecimalProperty);
        ShouldBeTestExtensions.ShouldBe<decimal>(expandoObject.DecimalProperty, 123.456m);
        ShouldBeTestExtensions.ShouldBeOfType<object[]>(expandoObject.ArrayProperty);

        var array = (object[])expandoObject.ArrayProperty;
        array.Length.ShouldBe(3);
        array[0].ShouldBe(1);
        array[1].ShouldBe("abc");
        array[2].ShouldBe(true);
    }

    [Fact]
    public void Deserialize_ValidJsonObjectAndObjectFormatExpandoAndDetectDateTimeAll_ReturnsDeserializedExpandoObjectWithMappedValues()
    {
        // Arrange
        const string json = """
                            {
                              "NullProperty": null,
                              "BoolFalseProperty": false,
                              "BoolTrueProperty": true,
                              "StringProperty": "Hello, World!",
                              "DateTimeOffsetProperty": "2024-06-01T12:34:56-05:00",
                              "DateTimeProperty": "2026-03-21T13:01:06",
                              "DateProperty": "2026-03-21",
                              "TimeProperty": "13:01:06",
                              "IntProperty": -234,
                              "DecimalProperty": 123.456,
                              "ArrayProperty": [1, "abc", true]
                            }
                            """;

        // Act
        var result = JsonSerializer.Deserialize<object>(json, GetOptions(detectDateTimeOffset: DetectDateTime.DateTimeOffset | DetectDateTime.DateTime | DetectDateTime.DateOnly | DetectDateTime.TimeOnly, objectFormat: ObjectFormat.Expando));

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<ExpandoObject>();

        dynamic expandoObject = (ExpandoObject)result;
        ShouldBeNullExtensions.ShouldBeNull<object?>(expandoObject.NullProperty);
        ShouldBeTestExtensions.ShouldBe(expandoObject.BoolFalseProperty, false);
        ShouldBeTestExtensions.ShouldBe(expandoObject.BoolTrueProperty, true);
        ShouldBeTestExtensions.ShouldBe<string>(expandoObject.StringProperty, "Hello, World!");
        ShouldBeTestExtensions.ShouldBe<DateTimeOffset>(expandoObject.DateTimeOffsetProperty, new DateTimeOffset(2024, 6, 1, 12, 34, 56, TimeSpan.FromHours(-5)));
        ShouldBeTestExtensions.ShouldBe<DateTime>(expandoObject.DateTimeProperty, new DateTime(2026, 3, 21, 13, 1, 6));
        ShouldBeTestExtensions.ShouldBe<DateOnly>(expandoObject.DateProperty, new DateOnly(2026, 3, 21));
        ShouldBeTestExtensions.ShouldBe<TimeOnly>(expandoObject.TimeProperty, new TimeOnly(13, 1, 6));
        ShouldBeTestExtensions.ShouldBe<int>(expandoObject.IntProperty, -234);
        ShouldBeTestExtensions.ShouldBe<decimal>(expandoObject.DecimalProperty, 123.456m);
        ShouldBeTestExtensions.ShouldBe<object[]>(expandoObject.ArrayProperty, new object[] { 1, "abc", true });

        var array = (object[])expandoObject.ArrayProperty;
        array.Length.ShouldBe(3);
        array[0].ShouldBe(1);
        array[1].ShouldBe("abc");
        array[2].ShouldBe(true);

        dynamic expando = result;
        ((DateTimeOffset)expando.DateTimeOffsetProperty).ShouldBe(new DateTimeOffset(2024, 6, 1, 12, 34, 56, TimeSpan.FromHours(-5)));
    }

    #endregion

    #region helper methods

    private static JsonSerializerOptions GetOptions(FloatFormat floatFormat = FloatFormat.Decimal, UnknownNumberFormat unknownNumberFormat = UnknownNumberFormat.Error, DetectDateTime detectDateTimeOffset = DetectDateTime.None, ObjectFormat objectFormat = ObjectFormat.Dictionary, JsonCommentHandling commentHandling = JsonCommentHandling.Disallow)
    {
        return new JsonSerializerOptions
        {
            ReadCommentHandling = commentHandling,
            Converters =
            {
                new ObjectAsPrimitiveConverter(floatFormat, unknownNumberFormat, detectDateTimeOffset, objectFormat)
            }
        };
    }

    #endregion

    #region helper types

    /// <summary>
    ///     A linked <see cref="System.Buffers.ReadOnlySequenceSegment{T}" /> used to build a multi-segment
    ///     <see cref="System.Buffers.ReadOnlySequence{T}" /> so that <c>Utf8JsonReader.HasValueSequence</c> is exercised.
    /// </summary>
    private sealed class BufferSegment : System.Buffers.ReadOnlySequenceSegment<byte>
    {
        public BufferSegment(ReadOnlyMemory<byte> memory) => Memory = memory;

        public BufferSegment Append(ReadOnlyMemory<byte> memory)
        {
            var segment = new BufferSegment(memory) { RunningIndex = RunningIndex + Memory.Length };
            Next = segment;
            return segment;
        }
    }

    #endregion
}
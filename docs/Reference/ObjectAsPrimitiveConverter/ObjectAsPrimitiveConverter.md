# ObjectAsPrimitiveConverter class
## Definition
Namespace: [Maple.Json.ObjectAsPrimitiveConverter](../namespace.md)<br>
Assembly: Maple.Json.ObjectAsPrimitiveConverter.dll<br>
Source: <a href="https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/src/Maple.Json.ObjectAsPrimitiveConverter/ObjectAsPrimitiveConverter.cs#L39" target="_blank">ObjectAsPrimitiveConverter.cs</a>

A JSON converter that can serialize and deserialize objects as their primitive types.

```csharp
public class ObjectAsPrimitiveConverter : JsonConverter<object>
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [JsonConverter](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter) → [JsonConverter&lt;Object&gt;](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter-1) → ObjectAsPrimitiveConverter

## Examples
```csharp
var result = JsonSerializer.Deserialize<object>(json, new JsonSerializerOptions
{
    Converters = { new ObjectAsPrimitiveConverter() }
});
```

## Remarks
When deserializing, each JSON value is converted to a CLR type as follows:

| JSON value | CLR type |
| ---------- | -------- |
| `null` | `null` |
| `true` / `false` | [Boolean](https://learn.microsoft.com/dotnet/api/system.boolean) (`bool`) |
| String | [String](https://learn.microsoft.com/dotnet/api/system.string) (`string`), or a date/time type when [DetectDateTime](ObjectAsPrimitiveConverter_DetectDateTime.md) is enabled and the value matches. |
| Integer number | The smallest of [Int32](https://learn.microsoft.com/dotnet/api/system.int32) (`int`), [Int64](https://learn.microsoft.com/dotnet/api/system.int64) (`long`), or [BigInteger](https://learn.microsoft.com/dotnet/api/system.numerics.biginteger) that can represent the value. |
| Floating-point number | `decimal`, `double`, or `float`, depending on [FloatFormat](ObjectAsPrimitiveConverter_FloatFormat.md). A value is treated as floating-point when it contains a decimal point or an exponent (for example `1e5`). |
| Number that cannot be represented | Throws a [JsonException](https://learn.microsoft.com/dotnet/api/system.text.json.jsonexception), or produces a [JsonElement](https://learn.microsoft.com/dotnet/api/system.text.json.jsonelement), depending on [UnknownNumberFormat](ObjectAsPrimitiveConverter_UnknownNumberFormat.md). |
| Array | [Object](https://learn.microsoft.com/dotnet/api/system.object)`[]` (`object[]`) whose elements are converted using these same rules. |
| Object | An `IDictionary<string, object>` or an [ExpandoObject](https://learn.microsoft.com/dotnet/api/system.dynamic.expandoobject), depending on [ObjectFormat](ObjectAsPrimitiveConverter_ObjectFormat.md), whose values are converted using the same rules. |

Comments are honoured according to the [ReadCommentHandling](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions.readcommenthandling) setting of the [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions); when skipping is enabled they are ignored and never appear in the result, including comments immediately before a closing `]` or `}`.

It is based on the implementation from: https://stackoverflow.com/a/65974452.

## Constructors
| Name | Description |
| ---- | ----------- |
| [ObjectAsPrimitiveConverter(FloatFormat, UnknownNumberFormat, DetectDateTime, ObjectFormat)](ObjectAsPrimitiveConverter_constructor.md) | Initializes a new instance of the [ObjectAsPrimitiveConverter](ObjectAsPrimitiveConverter.md) class. |

## Properties
| Name | Description |
| ---- | ----------- |
| [DetectDateTime](ObjectAsPrimitiveConverter_DetectDateTime.md) | Gets or sets whether to detect date-time values and which formats to recognize. |
| [FloatFormat](ObjectAsPrimitiveConverter_FloatFormat.md) | Gets or sets the default type for floating-point numbers. |
| [ObjectFormat](ObjectAsPrimitiveConverter_ObjectFormat.md) | Gets or sets the format to use when converting objects to primitive types. |
| [UnknownNumberFormat](ObjectAsPrimitiveConverter_UnknownNumberFormat.md) | Gets or sets the behavior when encountering a number format that is not recognized. |

## Methods
| Name | Description |
| ---- | ----------- |
| [Read(ref Utf8JsonReader, Type, JsonSerializerOptions)](ObjectAsPrimitiveConverter_Read.md) | Reads and converts the JSON to a primitive CLR type (see the [Remarks](#remarks) for the mapping). |
| [Write(Utf8JsonWriter, Object, JsonSerializerOptions)](ObjectAsPrimitiveConverter_Write.md) | Writes the value as JSON, emitting an empty object (`{}`) for a value whose runtime type is exactly [Object](https://learn.microsoft.com/dotnet/api/system.object). |

## See Also
* [Read(ref Utf8JsonReader, Type, JsonSerializerOptions)](ObjectAsPrimitiveConverter_Read.md)
* [Write(Utf8JsonWriter, Object, JsonSerializerOptions)](ObjectAsPrimitiveConverter_Write.md)
* [Maple.Json.ObjectAsPrimitiveConverter.Configuration namespace](../Configuration/namespace.md)
* [JSON serialization and deserialization in .NET - overview](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview)
* [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/api/system.text.json)
* [System.Text.Json NuGet package](https://www.nuget.org/packages/System.Text.Json)

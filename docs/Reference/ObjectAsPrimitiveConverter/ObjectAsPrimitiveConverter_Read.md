# ObjectAsPrimitiveConverter.Read Method
## Definition
Namespace: [Maple.Json.ObjectAsPrimitiveConverter](../namespace.md)<br>
Assembly: Maple.Json.ObjectAsPrimitiveConverter.dll<br>
Source: <a href="https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/src/Maple.Json.ObjectAsPrimitiveConverter/ObjectAsPrimitiveConverter.cs#L93" target="_blank">ObjectAsPrimitiveConverter.cs</a>

Reads and converts the JSON to a primitive CLR type.

```csharp
public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
```

### Parameters
`reader` [Utf8JsonReader](https://learn.microsoft.com/dotnet/api/system.text.json.utf8jsonreader)<br>
The reader positioned at the JSON value to convert.

`typeToConvert` [Type](https://learn.microsoft.com/dotnet/api/system.type)<br>
The type being converted. This converter handles [Object](https://learn.microsoft.com/dotnet/api/system.object) and ignores the value.

`options` [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions)<br>
The options in use, including the [ReadCommentHandling](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions.readcommenthandling) setting.

### Returns
[Object](https://learn.microsoft.com/dotnet/api/system.object)<br>
The converted value, or `null`, as a primitive CLR type.

## Remarks
Each JSON value is converted to a CLR type as follows:

| JSON value | CLR type |
| ---------- | -------- |
| `null` | `null` |
| `true` / `false` | [Boolean](https://learn.microsoft.com/dotnet/api/system.boolean) (`bool`) |
| String | [String](https://learn.microsoft.com/dotnet/api/system.string) (`string`), or a date/time type when [DetectDateTime](ObjectAsPrimitiveConverter_DetectDateTime.md) is enabled and the value matches. |
| Integer number | The smallest of [Int32](https://learn.microsoft.com/dotnet/api/system.int32) (`int`), [Int64](https://learn.microsoft.com/dotnet/api/system.int64) (`long`), or [BigInteger](https://learn.microsoft.com/dotnet/api/system.numerics.biginteger) that can represent the value. |
| Floating-point number | `decimal`, `double`, or `float`, depending on [FloatFormat](ObjectAsPrimitiveConverter_FloatFormat.md). A value is treated as floating-point when it contains a decimal point or an exponent (for example `1e5`). |
| Number that cannot be represented | Throws a [JsonException](https://learn.microsoft.com/dotnet/api/system.text.json.jsonexception), or produces a [JsonElement](https://learn.microsoft.com/dotnet/api/system.text.json.jsonelement), depending on [UnknownNumberFormat](ObjectAsPrimitiveConverter_UnknownNumberFormat.md). |
| Array | [Object](https://learn.microsoft.com/dotnet/api/system.object)`[]` (`object[]`) whose elements are converted using these same rules. |
| Object | An `IDictionary<string, object>` or an [ExpandoObject](https://learn.microsoft.com/dotnet/api/system.dynamic.expandoobject), depending on [ObjectFormat](ObjectAsPrimitiveConverter_ObjectFormat.md), whose values are converted using these same rules. |

Comments are honoured according to the [ReadCommentHandling](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions.readcommenthandling) setting; when skipping is enabled they are ignored and never appear in the result, including comments immediately before a closing `]` or `}`.

## Exceptions
### [JsonException](https://learn.microsoft.com/dotnet/api/system.text.json.jsonexception)<br>
An unknown [JsonTokenType](https://learn.microsoft.com/dotnet/api/system.text.json.jsontokentype) is encountered, or a number cannot be represented and [UnknownNumberFormat](ObjectAsPrimitiveConverter_UnknownNumberFormat.md) is *Error*.

## See Also
* [JsonConverter&lt;T&gt;.Read(ref Utf8JsonReader, Type, JsonSerializerOptions)](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter-1.read)
* [ObjectAsPrimitiveConverter](ObjectAsPrimitiveConverter.md)
* [Write(Utf8JsonWriter, Object, JsonSerializerOptions)](ObjectAsPrimitiveConverter_Write.md)

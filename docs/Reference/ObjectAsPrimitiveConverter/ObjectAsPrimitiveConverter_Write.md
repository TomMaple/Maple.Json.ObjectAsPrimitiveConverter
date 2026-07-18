# ObjectAsPrimitiveConverter.Write Method
## Definition
Namespace: [Maple.Json.ObjectAsPrimitiveConverter](../namespace.md)<br>
Assembly: Maple.Json.ObjectAsPrimitiveConverter.dll<br>
Source: <a href="https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/src/Maple.Json.ObjectAsPrimitiveConverter/ObjectAsPrimitiveConverter.cs#L198" target="_blank">ObjectAsPrimitiveConverter.cs</a>

Writes the value as JSON.

```csharp
public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
```

### Parameters
`writer` [Utf8JsonWriter](https://learn.microsoft.com/dotnet/api/system.text.json.utf8jsonwriter)<br>
The writer to which the value is written.

`value` [Object](https://learn.microsoft.com/dotnet/api/system.object)<br>
The value to write. May be `null`.

`options` [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions)<br>
The options in use when serializing.

## Remarks
The value is written according to its runtime type:

| Value | JSON written |
| ----- | ------------ |
| `null` | `null` |
| A value whose runtime type is exactly [Object](https://learn.microsoft.com/dotnet/api/system.object) | An empty object (`{}`) |
| Any other value | The result of serializing the value by its runtime type using the supplied [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions) |

Because the value is serialized by its runtime type, a value produced by [Read](ObjectAsPrimitiveConverter_Read.md) — such as an `IDictionary<string, object>` or an `object[]` of primitive values — round-trips back to its original JSON representation.

## See Also
* [JsonConverter&lt;T&gt;.Write(Utf8JsonWriter, T, JsonSerializerOptions)](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonconverter-1.write)
* [ObjectAsPrimitiveConverter](ObjectAsPrimitiveConverter.md)
* [Read(ref Utf8JsonReader, Type, JsonSerializerOptions)](ObjectAsPrimitiveConverter_Read.md)

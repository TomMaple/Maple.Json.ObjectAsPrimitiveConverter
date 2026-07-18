# UnknownNumberFormat Enum
## Definition
Namespace: [Maple.Json.ObjectAsPrimitiveConverter.Configuration](namespace.md)<br>
Assembly: Maple.Json.ObjectAsPrimitiveConverter.dll<br>
Source: <a href="https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/src/Maple.Json.ObjectAsPrimitiveConverter/Configuration/UnknownNumberFormat.cs#L19" target="_blank">UnknownNumberFormat.cs</a>

Specifies the behavior when encountering a number format that is not recognized.

```csharp
public enum UnknownNumberFormat
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/dotnet/api/system.enum) → UnknownNumberFormat

## Fields
| Name | Value | Description |
| ---- | ----- | ----------- |
| Error | 0 | Throw an exception when an unknown number format is detected. |
| JsonElement | 1 | Serialize an unknown number format as a [System.Text.Json.JsonElement](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement) (`JsonElement`) value. |

## See Also
* [ObjectAsPrimitiveConverter](../ObjectAsPrimitiveConverter/ObjectAsPrimitiveConverter.md)
* [Maple.Json.ObjectAsPrimitiveConverter.Configuration namespace](namespace.md)

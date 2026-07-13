# FloatFormat Enum
## Definition
Namespace: [Maple.Json.ObjectAsPrimitiveConverter.Configuration](namespace.md)<br>
Assembly: Maple.Json.ObjectAsPrimitiveConverter.dll<br>
Source: <a href="https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/src/Maple.Json.ObjectAsPrimitiveConverter/Configuration/FloatFormat.cs" target="_blank">FloatFormat.cs</a>

Specifies the desired format for floating-point numbers when converting objects to primitive types.

```csharp
public enum FloatFormat
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/dotnet/api/system.enum) → FloatFormat

## Fields
| Name | Value | Description |
| ---- | ----- | ----------- |
| Decimal | 0 | Serialize a floating-point number as a [System.Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal) (`decimal`) value. |
| Double | 1 | Serialize a floating-point number as a [System.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double) (`double`) value. |
| Single | 2 | Serialize a floating-point number as a [System.Single](https://learn.microsoft.com/en-us/dotnet/api/system.single) (`float`) value. |

## See Also
* [ObjectAsPrimitiveConverter](../ObjectAsPrimitiveConverter/ObjectAsPrimitiveConverter.md)
* [Maple.Json.ObjectAsPrimitiveConverter.Configuration namespace](namespace.md)

# ObjectFormat Enum
## Definition
Namespace: [Maple.Json.ObjectAsPrimitiveConverter.Configuration](namespace.md)<br>
Assembly: Maple.Json.ObjectAsPrimitiveConverter.dll<br>
Source: <a href="https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/src/Maple.Json.ObjectAsPrimitiveConverter/Configuration/ObjectFormat.cs#L19" target="_blank">ObjectFormat.cs</a>

Specifies the format to use when converting objects to primitive types.

```csharp
public enum ObjectFormat
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/dotnet/api/system.enum) → ObjectFormat

## Fields
| Name | Value | Description |
| ---- | ----- | ----------- |
| Dictionary | 0 | Serialize an object as a [System.Collections.Generic.IDictionary&lt;string, object&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2) value. |
| Expando| 1 | Serialize an object as a [System.Dynamic.ExpandoObject](https://learn.microsoft.com/en-us/dotnet/api/system.dynamic.expandoobject) value. |

## See Also
* [ObjectAsPrimitiveConverter](../ObjectAsPrimitiveConverter/ObjectAsPrimitiveConverter.md)
* [Maple.Json.ObjectAsPrimitiveConverter.Configuration namespace](namespace.md)

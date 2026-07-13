# ObjectAsPrimitiveConverter class
## Definition
Namespace: [Maple.Json.ObjectAsPrimitiveConverter](../namespace.md)<br>
Assembly: Maple.Json.ObjectAsPrimitiveConverter.dll<br>
Source: <a href="https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/src/Maple.Json.ObjectAsPrimitiveConverter/Maple.Json.ObjectAsPrimitiveConverter.cs" target="_blank">ObjectAsPrimitiveConverter.cs</a>

A JSON converter that can serialize and deserialize objects as their primitive types.

```csharp
public class ObjectAsPrimitiveConverter
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → ObjectAsPrimitiveConverter

## Examples
```csharp
var result = JsonSerializer.Deserialize<object>(json, new JsonSerializerOptions
{
    Converters = { new ObjectAsPrimitiveConverter() }
});
```

## Remarks
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

## See Also
* [Maple.Json.ObjectAsPrimitiveConverter.Configuration namespace](../Configuration/namespace.md)
* [JSON serialization and deserialization in .NET - overview](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview)
* [System.Text.Json](https://learn.microsoft.com/en-us/dotnet/api/system.text.json)
* [System.Text.Json NuGet package](https://www.nuget.org/packages/System.Text.Json)

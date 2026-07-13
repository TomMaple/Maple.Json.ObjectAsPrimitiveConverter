# DetectDateTime Enum
## Definition
Namespace: [Maple.Json.ObjectAsPrimitiveConverter.Configuration](namespace.md)<br>
Assembly: Maple.Json.ObjectAsPrimitiveConverter.dll<br>
Source: <a href="https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/src/Maple.Json.ObjectAsPrimitiveConverter/Configuration/DetectDateTime.cs" target="_blank">DetectDateTime.cs</a>

Specifies the types of date and time representations to detect when converting text properties to primitive types.

```csharp
public enum DetectDateTime
```

Inheritance [Object](https://learn.microsoft.com/dotnet/api/system.object) → [ValueType](https://learn.microsoft.com/dotnet/api/system.valuetype) → [Enum](https://learn.microsoft.com/dotnet/api/system.enum) → DetectDateTime

## Remarks
This enumeration can be combined using a bitwise OR to detect multiple date and time types simultaneously.

## Fields
| Name | Value | Description |
| ---- | ----- | ----------- |
| None | 0 | Do not detect date-time values. |
| DateTimeOffset | 1 | Detect date-time values with timezone and deserialize them as [System.DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset) (`DateTimeOffset`) values. |
| DateTime | 2 | Detect date-time values without timezone and deserialize them as [System.DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime) (`DateTime`) values. |
| DateOnly | 4 | Detect date values without time and deserialize them as [System.DateOnly](https://learn.microsoft.com/en-us/dotnet/api/system.dateonly) (`DateOnly`) values. |
| TimeOnly | 8 | Detect time values without date and deserialize them as [System.TimeOnly](https://learn.microsoft.com/en-us/dotnet/api/system.timeonly) (`TimeOnly`) values. |

## See Also
* [ObjectAsPrimitiveConverter](../ObjectAsPrimitiveConverter/ObjectAsPrimitiveConverter.md)
* [Maple.Json.ObjectAsPrimitiveConverter.Configuration namespace](namespace.md)

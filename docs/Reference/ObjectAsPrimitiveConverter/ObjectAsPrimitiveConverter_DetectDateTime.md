# ObjectAsPrimitiveConverter.DetectDateTime Property
## Definition
Namespace: [Maple.Json.ObjectAsPrimitiveConverter](../namespace.md)<br>
Assembly: Maple.Json.ObjectAsPrimitiveConverter.dll<br>
Source: <a href="https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/src/Maple.Json.ObjectAsPrimitiveConverter/ObjectAsPrimitiveConverter.cs#L68" target="_blank">ObjectAsPrimitiveConverter.cs</a>

Gets or sets whether to detect date-time values and which formats to recognize.

```csharp
public DetectDateTime DetectDateTime { get; init; }
```

### Property Value
[DetectDateTime](../Configuration/DetectDateTime.md)

Specifies the types of date and time representations to detect when converting text properties to primitive types.

## Remarks
This enumeration can be combined using a bitwise OR to detect multiple date and time types simultaneously.

If not specified, the category defaults to *None*.

## See Also
* [DetectDateTime](../Configuration/DetectDateTime.md) — the list of defined date-time detection options.
* [ObjectAsPrimitiveConverter](ObjectAsPrimitiveConverter.md).

# ObjectAsPrimitiveConverter.FloatFormat Property
## Definition
Namespace: [Maple.Json.ObjectAsPrimitiveConverter](../namespace.md)<br>
Assembly: Maple.Json.ObjectAsPrimitiveConverter.dll<br>
Source: <a href="https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/src/Maple.Json.ObjectAsPrimitiveConverter/ObjectAsPrimitiveConverter.cs#L73" target="_blank">ObjectAsPrimitiveConverter.cs</a>

Gets or sets the default type for floating-point numbers.

```csharp
public FloatFormat FloatFormat { get; init; }
```

### Property Value
[FloatFormat](../Configuration/FloatFormat.md)

Specifies the desired format for floating-point numbers when converting objects to primitive types.

If not specified, the category defaults to *Decimal*.

## See Also
* [FloatFormat](../Configuration/FloatFormat.md) — the list of defined floating-point formats.
* [ObjectAsPrimitiveConverter](ObjectAsPrimitiveConverter.md).

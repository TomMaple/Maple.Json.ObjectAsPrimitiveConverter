# ObjectAsPrimitiveConverter.ObjectFormat Property
## Definition
Namespace: [Maple.Json.ObjectAsPrimitiveConverter](../namespace.md)<br>
Assembly: Maple.Json.ObjectAsPrimitiveConverter.dll<br>
Source: <a href="https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/src/Maple.Json.ObjectAsPrimitiveConverter/ObjectAsPrimitiveConverter.cs#L78" target="_blank">ObjectAsPrimitiveConverter.cs</a>

Gets or sets the format to use when converting objects to primitive types.

```csharp
public DetectDateTime ObjectFormat { get; init; }
```

### Property Value
[DetectDateTime](../Configuration/DetectDateTime.md)

Specifies the format to use when converting objects to primitive types.

## Remarks
If not specified, the category defaults to *Dictionary*.

## See Also
* [ObjectFormat](../Configuration/ObjectFormat.md) — the list of defined object formats.
* [ObjectAsPrimitiveConverter](ObjectAsPrimitiveConverter.md).

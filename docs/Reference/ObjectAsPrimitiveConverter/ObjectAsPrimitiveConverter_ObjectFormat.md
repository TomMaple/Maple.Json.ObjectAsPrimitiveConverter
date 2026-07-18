# ObjectAsPrimitiveConverter.ObjectFormat Property
## Definition
Namespace: [Maple.Json.ObjectAsPrimitiveConverter](../namespace.md)<br>
Assembly: Maple.Json.ObjectAsPrimitiveConverter.dll<br>
Source: <a href="https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/src/Maple.Json.ObjectAsPrimitiveConverter/ObjectAsPrimitiveConverter.cs#L85" target="_blank">ObjectAsPrimitiveConverter.cs</a>

Gets or sets the format to use when converting objects to primitive types.

```csharp
public ObjectFormat ObjectFormat { get; init; }
```

### Property Value
[ObjectFormat](../Configuration/ObjectFormat.md)

Specifies the format to use when converting objects to primitive types.

## Remarks
If not specified, the category defaults to *Dictionary*.

## See Also
* [ObjectFormat](../Configuration/ObjectFormat.md) — the list of defined object formats.
* [ObjectAsPrimitiveConverter](ObjectAsPrimitiveConverter.md).

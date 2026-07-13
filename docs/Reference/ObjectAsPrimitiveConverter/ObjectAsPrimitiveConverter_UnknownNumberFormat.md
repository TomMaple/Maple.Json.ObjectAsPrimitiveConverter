# ObjectAsPrimitiveConverter.UnknownNumberFormat Property
## Definition
Namespace: [Maple.Json.ObjectAsPrimitiveConverter](../namespace.md)<br>
Assembly: Maple.Json.ObjectAsPrimitiveConverter.dll<br>
Source: <a href="https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/src/Maple.Json.ObjectAsPrimitiveConverter/ObjectAsPrimitiveConverter.cs#L83" target="_blank">ObjectAsPrimitiveConverter.cs</a>

Gets or sets the behavior when encountering a number format that is not recognized.

```csharp
public UnknownNumberFormat UnknownNumberFormat { get; init; }
```

### Property Value
[UnknownNumberFormat](../Configuration/UnknownNumberFormat
Specifies the behavior when encountering a number format that is not recognized.

## Remarks
If not specified, the category defaults to *Error*.

## See Also
* [UnknownNumberFormat](../Configuration/UnknownNumberFormat.md) — for the list of defined behaviours for unknown number formats.
* [ObjectAsPrimitiveConverter](ObjectAsPrimitiveConverter.md).

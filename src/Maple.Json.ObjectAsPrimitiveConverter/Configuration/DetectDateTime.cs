using System;

namespace Maple.Json.ObjectAsPrimitiveConverter.Configuration;

[Flags]
public enum DetectDateTime
{
    None = 0,
    DateTimeOffset = 1,
    DateTime = 2,
    DateOnly = 4,
    TimeOnly = 8
}

![NuGet Version](https://img.shields.io/nuget/v/Maple.Json.ObjectAsPrimitiveConverter)
![NuGet Downloads](https://img.shields.io/nuget/dt/Maple.Json.ObjectAsPrimitiveConverter)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/build-and-publish.yml)
![GitHub last commit](https://img.shields.io/github/last-commit/TomMaple/Maple.Json.ObjectAsPrimitiveConverter)


# Maple.Json.ObjectAsPrimitiveConverter
Provides the `ObjectAsPrimitiveConverter` for *System.Text.Json* that allows to serialize and deserialize objects properties using primitive types.

It is based on the implementation from: https://stackoverflow.com/a/65974452

# Give it a star ⭐
Do you like it? Show your support by giving this project a star!

# How it works
Consider the following JSON as an example:
```json
{
  "NullProperty": null,
  "StringProperty": "Hello, World!",
  "IntProperty": -234,
  "DecimalProperty": 123.456,
  "ArrayProperty": [1, null, "abc", true]
}
```

When using *System.Text.Json* to deserialize this JSON to an `object`, the result value will look like this:
![Serialization to object result](https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/.resources/standard_object.png)


When using *System.Text.Json* to deserialize this JSON to an `IDictionary<string, object>`, the result value will look like this:
![Serialization to dictionary result](https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/.resources/standard_dictionary.png)

When using *System.Text.Json* to deserialize this JSON to an `object` with the `ObjectAsPrimitiveConverter`, the result value will look like this:
![Serialization to object with converter result](https://github.com/TomMaple/Maple.Json.ObjectAsPrimitiveConverter/blob/main/.resources/converter.png)

# Quick Start
## Adding the *NuGet* package
Add the `Maple.Json.ObjectAsPrimitiveConverter` package to your project using the *NuGet* Package Manager in your IDE or the *dotnet* tool in the console:
```shellscript
dotnet add package Maple.Json.ObjectAsPrimitiveConverter
```

## Using the `ObjectAsPrimitiveConverter`
In a method to deserialize JSON to an `object`:
```csharp
var result = JsonSerializer.Deserialize<object>(json, new JsonSerializerOptions
{
    Converters = { new ObjectAsPrimitiveConverter() }
});
```

### Global settings
#### *ASP.NET Core* controller-based application
```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new ObjectAsPrimitiveConverter());
    });
```

#### *ASP.NET Core* minimal API application
```csharp
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new ObjectAsPrimitiveConverter());
});
```

#### *Azure Functions* (isolated worker model) application
```csharp
builder.Services.Configure<WorkerOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new ObjectAsPrimitiveConverter());
});
```

#### *Azure Functions* (isolated worker model) application with *ASP.NET Core* integration
```csharp
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new ObjectAsPrimitiveConverter());
});
```

## Configuration
```csharp
new ObjectAsPrimitiveConverter(floatFormat, unknownNumberFormat, detectDateTimeOffset, objectFormat);
```

### Detect date-time
| Name | Value | Description |
| ---- | ----- | ----------- |
| None | 0 | Do not detect date-time values. |
| DateTimeOffset | 1 | Detect date-time values with timezone and deserialize them as [System.DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset) (`DateTimeOffset`) values. |
| DateTime | 2 | Detect date-time values without timezone and deserialize them as [System.DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime) (`DateTime`) values. |
| DateOnly | 4 | Detect date values without time and deserialize them as [System.DateOnly](https://learn.microsoft.com/en-us/dotnet/api/system.dateonly) (`DateOnly`) values. |
| TimeOnly | 8 | Detect time values without date and deserialize them as [System.TimeOnly](https://learn.microsoft.com/en-us/dotnet/api/system.timeonly) (`TimeOnly`) values. |

### Float format
| Name | Value | Description |
| ---- | ----- | ----------- |
| Decimal | 0 | Serialize a floating-point number as a [System.Decimal](https://learn.microsoft.com/en-us/dotnet/api/system.decimal) (`decimal`) value. |
| Double | 1 | Serialize a floating-point number as a [System.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double) (`double`) value. |
| Single | 2 | Serialize a floating-point number as a [System.Single](https://learn.microsoft.com/en-us/dotnet/api/system.single) (`float`) value. |

### Object format
| Name | Value | Description |
| ---- | ----- | ----------- |
| Dictionary | 0 | Serialize an object as a [System.Collections.Generic.Dictionary&lt;string, object&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2) value. |
| Expando| 1 | Serialize an object as a [System.Dynamic.ExpandoObject](https://learn.microsoft.com/en-us/dotnet/api/system.dynamic.expandoobject) value. |

# Unknown number format
| Name | Value | Description |
| ---- | ----- | ----------- |
| Error | 0 | Throw an exception when an unknown number format is detected. |
| JsonElement | 1 | Serialize an unknown number format as a [System.Text.Json.JsonElement](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement) (`JsonElement`) value. |

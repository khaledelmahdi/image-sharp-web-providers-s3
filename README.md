# Elmahdi.Phone

[![Nuget downloads](https://img.shields.io/nuget/v/Elmahdi.ImageSharp.Web.Providers.S3)](https://www.nuget.org/packages/Elmahdi.ImageSharp.Web.Providers.S3/)
[![Test workflow]](https://github.com/khaledelmahdi/image-sharp-web-providers-s3/actions/workflows/dotnet.yml/badge.svg)
[![GitHub license](https://img.shields.io/github/license/khaledelmahdi/image-sharp-web-providers-s3)](https://github.com/khaledelmahdi/image-sharp-web-providers-s3)

Simple and common phone extensions to format and validate phone numbers.

## Installation

Available on [nuget](https://www.nuget.org/packages/Elmahdi.Phone/)

```
PM> Install-Package Elmahdi.Phone -Version 1.0.3
```

```
> dotnet add package Elmahdi.Phone --version 1.0.3
```

## Usage

### Formatting:

```c#
var phone = "0141 4 0 400 50";
phone.ToLocalPhoneNumber(); // returns 0141 404 0050
phone.ToInternationalPhoneNumber() // returns +441414040050
```

By default, these extensions are based on United Kingdom numbers.
You may override them by passing the country code to the extension.

```c#
var phone = "05 06 850 900";
phone.ToLocalPhoneNumber("EG"); // returns 050 6850900
phone.ToInternationalPhoneNumber("EG") // returns +20506850900
```

### Validating

```c#
var validPhone = "0141 4 0 400 50";
var invalidPhone = "abc";

validPhone.IsValidPhoneNumberFor(); // returns true;
invalidPhone.IsValidPhoneNumberFor(); // returns false;
```

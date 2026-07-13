# brk.Framework.Utility

A lightweight .NET utility library that provides commonly used helper classes and extension methods for everyday application development.

The package includes utilities for working with dates, strings, numbers, enums, validation, culture, and reflection, helping reduce boilerplate code across your projects.

---

## Installation

Install the package from NuGet:

```bash
dotnet add package brk.Framework.Utility
```

Or using the NuGet Package Manager:

```powershell
Install-Package brk.Framework.Utility
```

---

## Features

- 📅 Date and time conversion utilities
- 🇮🇷 Persian (Shamsi) date extensions
- 🔢 Number formatting and conversion helpers
- 🔤 String manipulation extensions
- ✅ Common string validators
- 📧 Email validation
- 📱 Mobile number validation
- 🆔 National ID validation
- 🏷️ Enum helper extensions
- 🏛️ Culture helper methods
- 🔍 Reflection-based attribute helpers

---

# Included Components

## DateTimeConverter

Provides helper methods for converting and formatting `DateTime` values.

Typical use cases:

- Convert between calendars
- Date formatting
- Date parsing
- Time conversions

---

## PersianDateTimeExtensions

Extension methods for working with the Persian (Solar Hijri) calendar.

Features include:

- Convert Gregorian to Persian date
- Convert Persian date to Gregorian
- Persian date formatting
- Persian date/time display

---

## EnumExtensions

Extension methods for working with enumerations.

Examples include:

- Get display name
- Get description attribute
- Convert enum to list
- Parse enum safely

---

## NumberExtensions

Utility methods for numeric values.

Examples:

- Formatting
- Rounding
- Percentage calculations
- Numeric conversions

---

## StringExtensions

Common string helper methods.

Examples:

- Null or empty handling
- Trimming
- Case conversion
- String formatting
- Safe string manipulation

---

## StringValidatorExtensions

Validation helpers for strings.

Examples:

- IsNumeric
- IsAlphabetic
- IsNullOrWhiteSpace
- Length validation
- Regular expression helpers

---

## AttributeHelper

Reflection utilities for reading .NET attributes.

Examples:

- Read custom attributes
- Get Display attributes
- Retrieve Description attributes
- Metadata inspection

---

## CultureHelper

Utilities related to globalization and localization.

Examples:

- Current culture
- UI culture
- Culture conversion
- Language helpers

---

## EmailValidator

Helper methods for validating email addresses.

Example:

```csharp
bool valid = EmailValidator.IsValid("john@example.com");
```

---

## MobileValidator

Provides validation methods for mobile numbers.

Supports:

- Iran
- Afghanistan

Example:

```csharp
bool valid = MobileValidator.IsValid("+989123456789");
```

---

## NationalIdValidator

Utilities for validating national identification numbers.

Currently supported:

- Iran National Code

Example:

```csharp
bool valid = NationalIdValidator.IsValid("1234567890");
```

---

# Example

```csharp
using brk.Framework.Utility;

// Email
EmailValidator.IsValid("user@example.com");

// Mobile
MobileValidator.IsValid("+989123456789");

// National ID
NationalIdValidator.IsValid("1234567890");

// Persian Date
var persianDate = DateTime.Now.ToPersianDate();

// Enum
var description = MyEnum.Active.GetDescription();
```

---

# Target Framework

- .NET 8+
- .NET 9+
- .NET 10+

---

# Why brk.Framework.Utility?

- Lightweight
- No unnecessary dependencies
- Extension-method based API
- Reusable across multiple projects
- Production-ready
- Easy to integrate

---

# Contributing

Contributions are welcome!

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Open a Pull Request

---

# License

This project is licensed under the MIT License.

---

# Repository

GitHub:
https://github.com/mustafa-barakzey/brk.Framework.Utility

---

# Author

**Mustafa Barakzey**

Senior .NET Developer

---
```
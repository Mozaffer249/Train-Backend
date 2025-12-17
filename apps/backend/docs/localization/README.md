# Localization Documentation

Internationalization (i18n) and localization (l10n) documentation.

## 📄 Documents

### [Validator Localization](./validator-localization.md)
Validation message localization:
- FluentValidation localization
- Resource file organization
- Multi-language error messages
- Custom validation messages

## 🌍 Supported Languages

Currently supported:
- **English (en-US)** - Default language
- **Arabic (ar-EG)** - Secondary language

## 📝 Resource Files

### Location
`Sudan_Train.Core/Resources/`

### Structure
```
/Resources
  /Authentication
    AuthenticationResources.resx (English)
    AuthenticationResources.ar-EG.resx (Arabic)
  /Validation
    ValidationResources.resx (English)
    ValidationResources.ar-EG.resx (Arabic)
```

### Naming Convention
- **Keys**: PascalCase (e.g., `EmailIsRequired`)
- **Files**: `ResourceName.{culture}.resx`

## 🔧 Adding New Languages

1. Create resource file: `ResourceName.{culture}.resx`
2. Add culture to supported cultures in `Program.cs`
3. Translate all keys from base resource file
4. Test with `Accept-Language` header

## 💻 Usage in Code

### Controllers
```csharp
private readonly IStringLocalizer<AuthenticationResources> _localizer;

var message = _localizer[AuthenticationResourcesKeys.EmailIsRequired];
```

### Validators
```csharp
RuleFor(x => x.Email)
    .NotEmpty()
    .WithMessage(_localizer[ValidationResourcesKeys.EmailIsRequired]);
```

## 🧪 Testing Localization

Use `Accept-Language` header in API requests:
```http
GET /api/resource
Accept-Language: ar-EG
```

## 🔗 Related Documentation

- [Development](../development/localization-refactoring.md) - Localization improvements
- [Configuration](../configuration/appsettings-guide.md) - Localization settings

# Validator Localization Guide

## ✅ What Was Implemented

All FluentValidation validators now support **English** and **Arabic** localization based on the `Accept-Language` HTTP header.

### Updated Validators:
1. ✅ **LoginCommandValidator** - Now fully localized
2. ✅ **RegisterCommandValidator** - Now fully localized
3. ✅ **RefreshTokenCommandValidator** - Already localized
4. ✅ **SendResetPasswordCodeCommandValidator** - Already localized
5. ✅ **ResetPasswordCommandValidator** - Already localized
6. ✅ **ConfirmEmailCommandValidator** - Already localized
7. ✅ **ValidateTokenQueryValidator** - Already localized

---

## 🔑 New Resource Keys Added

| Key | English | Arabic |
|-----|---------|--------|
| `MinLengthIs6` | "Minimum length is 6 characters" | "الحد الأدنى للطول هو 6 أحرف" |
| `PasswordsDoNotMatch` | "Passwords do not match" | "كلمات المرور غير متطابقة" |

---

## 🧪 How to Test

### Quick Test (cURL):

```bash
# Test English Validation
curl -X POST "http://localhost:5145/Api/V1/Authentication/Login" \
  -H "Accept-Language: en-US" \
  -H "Content-Type: application/json" \
  -d '{"userName":"","password":""}'

# Test Arabic Validation
curl -X POST "http://localhost:5145/Api/V1/Authentication/Login" \
  -H "Accept-Language: ar-EG" \
  -H "Content-Type: application/json" \
  -d '{"userName":"","password":""}'
```

### Using Test File:

1. **Start your application:**
   ```bash
   dotnet run --project Sudan_Train
   ```

2. **Open `test-validator-localization.http`** in VS Code

3. **Install REST Client Extension** (if not installed):
   - Extension ID: `humao.rest-client`

4. **Click "Send Request"** above any test case

---

## 📊 Expected Results

### English Response (Accept-Language: en-US):
```json
{
  "statusCode": 400,
  "succeeded": false,
  "message": "Validation failed",
  "errors": [
    "This field is required",
    "This field is required"
  ]
}
```

### Arabic Response (Accept-Language: ar-EG):
```json
{
  "statusCode": 400,
  "succeeded": false,
  "message": "فشل التحقق",
  "errors": [
    "هذا الحقل مطلوب",
    "هذا الحقل مطلوب"
  ]
}
```

---

## 🔍 Validation Rules by Endpoint

### 1. Login Endpoint
**POST** `/Api/V1/Authentication/Login`

| Field | Rules | Messages |
|-------|-------|----------|
| `userName` | Required | `IsRequired` |
| `password` | Required, Min 6 chars | `IsRequired`, `MinLengthIs6` |

### 2. Register Endpoint
**POST** `/Api/V1/Authentication/Register`

| Field | Rules | Messages |
|-------|-------|----------|
| `firstName` | Required, Max 100 chars | `IsRequired`, `MaxLengthIs100` |
| `lastName` | Required, Max 100 chars | `IsRequired`, `MaxLengthIs100` |
| `userName` | Required, Min 3 chars, Max 100 chars | `IsRequired`, `MinLengthIs3`, `MaxLengthIs100` |
| `email` | Required, Valid email | `IsRequired`, `InvalidFormat` |
| `password` | Required, Min 6 chars | `IsRequired`, `MinLengthIs6` |
| `confirmPassword` | Required, Must match password | `IsRequired`, `PasswordsDoNotMatch` |

### 3. RefreshToken Endpoint
**POST** `/Api/V1/Authentication/RefreshToken`

| Field | Rules | Messages |
|-------|-------|----------|
| `accessToken` | Required | `IsRequired` |
| `refreshToken` | Required | `IsRequired` |

### 4. SendResetPasswordCode Endpoint
**POST** `/Api/V1/Authentication/SendResetPasswordCode`

| Field | Rules | Messages |
|-------|-------|----------|
| `email` | Required, Valid email | `IsRequired`, `InvalidFormat` |

### 5. ResetPassword Endpoint
**POST** `/Api/V1/Authentication/ResetPassword`

| Field | Rules | Messages |
|-------|-------|----------|
| `email` | Required, Valid email | `IsRequired`, `InvalidFormat` |
| `resetCode` | Required | `IsRequired` |
| `newPassword` | Required, Min 6 chars | `IsRequired`, Min 6 |
| `confirmPassword` | Required, Must match | `IsRequired`, Must match |

### 6. ConfirmEmail Endpoint
**POST** `/Api/V1/Authentication/ConfirmEmail`

| Field | Rules | Messages |
|-------|-------|----------|
| `userId` | > 0 | `IsRequired` |
| `code` | Required | `IsRequired` |

---

## 🌐 Supported Languages

| Culture Code | Language | Status |
|-------------|----------|--------|
| `en-US` | English (US) | ✅ Supported |
| `ar-EG` | Arabic (Egypt) | ✅ Supported |

---

## 🔧 How It Works

### 1. Request Processing:
```
User Request with Accept-Language Header
         ↓
RequestLocalizationMiddleware (Program.cs Line 125)
         ↓
Sets CurrentCulture and CurrentUICulture
         ↓
FluentValidation reads IStringLocalizer
         ↓
Returns localized validation messages
```

### 2. Validator Structure:
```csharp
public class MyValidator : AbstractValidator<MyCommand>
{
    public MyValidator(IStringLocalizer<SharedResources> stringLocalizer)
    {
        RuleFor(x => x.Field)
            .NotEmpty()
            .WithMessage(stringLocalizer[SharedResourcesKeys.IsRequired]);
    }
}
```

### 3. Resource Resolution:
- `en-US` → `SharedResources.en.resx`
- `ar-EG` → `SharedResources.ar.resx`
- Default → `SharedResources.en.resx`

---

## 📝 Common Test Scenarios

### Test 1: Required Field Validation
```http
POST /Api/V1/Authentication/Login
Accept-Language: en-US

{
  "userName": "",
  "password": ""
}
```
**Expected:** "This field is required" (English)

---

### Test 2: Minimum Length Validation
```http
POST /Api/V1/Authentication/Login
Accept-Language: ar-EG

{
  "userName": "test",
  "password": "123"
}
```
**Expected:** "الحد الأدنى للطول هو 6 أحرف" (Arabic)

---

### Test 3: Email Format Validation
```http
POST /Api/V1/Authentication/Register
Accept-Language: en-US

{
  "email": "invalid-email"
}
```
**Expected:** "Invalid format" (English)

---

### Test 4: Password Mismatch Validation
```http
POST /Api/V1/Authentication/Register
Accept-Language: ar-EG

{
  "password": "Pass123",
  "confirmPassword": "Different"
}
```
**Expected:** "كلمات المرور غير متطابقة" (Arabic)

---

## ✅ Verification Checklist

- [ ] Start application: `dotnet run --project Sudan_Train`
- [ ] Test with `en-US` header - Get English messages
- [ ] Test with `ar-EG` header - Get Arabic messages
- [ ] Test different validation rules (required, min/max length, format)
- [ ] Verify no hardcoded English messages appear
- [ ] Verify no resource keys (like "IsRequired") appear in responses

---

## 🎯 Success Criteria

✅ **Validator localization is working if:**
1. Same request with `Accept-Language: en-US` returns English messages
2. Same request with `Accept-Language: ar-EG` returns Arabic messages
3. No hardcoded strings appear in error responses
4. All validation messages are properly translated
5. Message content changes based on language header

---

## 🚀 Quick Verification Command

```bash
# Test English
curl -H "Accept-Language: en-US" -H "Content-Type: application/json" \
  -d '{"userName":"","password":""}' \
  http://localhost:5145/Api/V1/Authentication/Login

# Test Arabic
curl -H "Accept-Language: ar-EG" -H "Content-Type: application/json" \
  -d '{"userName":"","password":""}' \
  http://localhost:5145/Api/V1/Authentication/Login
```

Compare the `errors` array in both responses - they should be in different languages! 🎉

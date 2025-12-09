namespace Sudan_Train.Core.Resources.Authentication
{
    public static class AuthenticationResourcesKeys
    {
        // Authentication & Authorization
        public const string UserNameIsExist = "UserNameIsExist";
        public const string EmailIsExist = "EmailIsExist";
        public const string EmailIsNotExist = "EmailIsNotExist";
        public const string PhoneNumberIsExist = "PhoneNumberIsExist";
        public const string FailedToAddUser = "FailedToAddUser";
        public const string FailedToUpdateUser = "FailedToUpdateUser";
        public const string FailedToDeleteUser = "FailedToDeleteUser";
        public const string UserNotFound = "UserNotFound";
        public const string PasswordNotCorrect = "PasswordNotCorrect";
        public const string UserIsNotActive = "UserIsNotActive";
        public const string UserRegisteredSuccessfully = "UserRegisteredSuccessfully";
        public const string WelcomeEmailSubject = "WelcomeEmailSubject";
        public const string WelcomeEmailBody = "WelcomeEmailBody";

        // Field-Specific Validation Messages
        public const string UserNameIsRequired = "UserNameIsRequired";
        public const string PasswordIsRequired = "PasswordIsRequired";
        public const string PasswordMinLength = "PasswordMinLength";
        public const string FirstNameIsRequired = "FirstNameIsRequired";
        public const string LastNameIsRequired = "LastNameIsRequired";
        public const string EmailIsRequired = "EmailIsRequired";
        public const string EmailInvalidFormat = "EmailInvalidFormat";
        public const string ConfirmPasswordIsRequired = "ConfirmPasswordIsRequired";
        public const string UserNameMinLength = "UserNameMinLength";
        public const string UserNameMaxLength = "UserNameMaxLength";
        public const string FirstNameMaxLength = "FirstNameMaxLength";
        public const string LastNameMaxLength = "LastNameMaxLength";
        public const string PasswordsDoNotMatch = "PasswordsDoNotMatch";
    }
}

namespace Sudan_Train.Core.Resources
{
    public static class SharedResourcesKeys
    {
        // Response Messages
        public const string Success = "Success";
        public const string Created = "Created";
        public const string Deleted = "Deleted";
        public const string Updated = "Updated";
        public const string NotFound = "NotFound";
        public const string BadRequest = "BadRequest";
        public const string UnAuthorized = "UnAuthorized";
        public const string UnprocessableEntity = "UnprocessableEntity";
        public const string InternalServerError = "InternalServerError";

        // Authentication & Authorization
        public const string UserNameIsExist = "UserNameIsExist";
        public const string EmailIsExist = "EmailIsExist";
        public const string EmailIsNotExist = "EmailIsNotExist";
        public const string FailedToAddUser = "FailedToAddUser";
        public const string FailedToUpdateUser = "FailedToUpdateUser";
        public const string FailedToDeleteUser = "FailedToDeleteUser";
        public const string UserNotFound = "UserNotFound";
        public const string PasswordNotCorrect = "PasswordNotCorrect";
        public const string UserIsNotActive = "UserIsNotActive";
        public const string UserRegisteredSuccessfully = "UserRegisteredSuccessfully";

        // Validation
        public const string IsRequired = "IsRequired";
        public const string IsExist = "IsExist";
        public const string IsNotExist = "IsNotExist";
        public const string MaxLengthIs100 = "MaxLengthIs100";
        public const string MaxLengthIs200 = "MaxLengthIs200";
        public const string MaxLengthIs500 = "MaxLengthIs500";
        public const string MinLengthIs3 = "MinLengthIs3";
        public const string InvalidFormat = "InvalidFormat";

        // Train Domain
        public const string TrainNotFound = "TrainNotFound";
        public const string TrainAlreadyExist = "TrainAlreadyExist";
        public const string CoachNotFound = "CoachNotFound";
        public const string SeatNotFound = "SeatNotFound";
        public const string SeatAlreadyBooked = "SeatAlreadyBooked";

        // Trip Domain
        public const string TripNotFound = "TripNotFound";
        public const string TripIsFull = "TripIsFull";
        public const string TripIsNotAvailable = "TripIsNotAvailable";

        // Booking Domain
        public const string BookingNotFound = "BookingNotFound";
        public const string BookingAlreadyExist = "BookingAlreadyExist";
        public const string BookingCancelled = "BookingCancelled";
        public const string BookingConfirmed = "BookingConfirmed";
        public const string InvalidBookingStatus = "InvalidBookingStatus";

        // Station Domain
        public const string StationNotFound = "StationNotFound";
        public const string StationAlreadyExist = "StationAlreadyExist";

        // Passenger Domain
        public const string PassengerNotFound = "PassengerNotFound";
        public const string PassengerAlreadyExist = "PassengerAlreadyExist";

        // Payment Domain
        public const string PaymentFailed = "PaymentFailed";
        public const string PaymentSuccessful = "PaymentSuccessful";
        public const string InvalidPaymentMethod = "InvalidPaymentMethod";

        // General
        public const string NoDataFound = "NoDataFound";
        public const string OperationFailed = "OperationFailed";
        public const string OperationSuccessful = "OperationSuccessful";
    }
}

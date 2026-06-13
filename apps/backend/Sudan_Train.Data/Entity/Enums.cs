namespace Sudan_Train.Data.Entity
{
    public enum BookingStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        Completed = 3
    }

    public enum PaymentMethod
    {
        Cash = 0,
        CreditCard = 1,
        DebitCard = 2,
        BankTransfer = 3,
        MobilePayment = 4
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2,
        Refunded = 3
    }

    public enum CoachClass
    {
        First = 1,
        Second = 2,
        Third = 3
    }

    public enum SeatStatus
    {
        Available = 0,
        Reserved = 1,
        Occupied = 2,
        Maintenance = 3
    }

    public enum RefundStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Completed = 3
    }

    public enum RefundMethod
    {
        Original = 0,
        BankTransfer = 1,
        Cash = 2
    }

    public enum NotificationType
    {
        BookingConfirmation = 0,
        BookingCancellation = 1,
        PaymentReceived = 2,
        TripDelay = 3,
        TripCancellation = 4,
        PromotionalOffer = 5,
        SystemAlert = 6
    }

    public enum NotificationChannel
    {
        Email = 0,
        SMS = 1,
        Push = 2,
        InApp = 3
    }

    public enum RecurrenceType
    {
        Daily = 0,
        Weekly = 1,
        Monthly = 2,
        Custom = 3
    }

    public enum PromotionType
    {
        Percentage = 0,
        FixedAmount = 1,
        BuyOneGetOne = 2
    }

    public enum TicketStatus
    {
        Issued = 0,
        Boarded = 1,
        NoShow = 2,
        Cancelled = 3,
    }

    public enum TripStatus
    {
        Scheduled = 0,
        Departed = 1,
        Arrived = 2,
        Cancelled = 3,
        Delayed = 4,
    }
}


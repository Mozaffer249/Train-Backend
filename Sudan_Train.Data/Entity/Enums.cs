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
}


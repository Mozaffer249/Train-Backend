namespace Sudan_Train.Data.AppMetaData
{
    public static class Roles
    {
        // Role Names
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        // Generic legacy role — kept for back-compat. New endpoints use the
        // specialized sub-roles below.
        public const string Staff = "Staff";
        // Specialized station-staff sub-roles. A user may have either or both.
        public const string StaffCounter = "StaffCounter";   // ticket-window cashier
        public const string StaffBoarding = "StaffBoarding"; // platform / gate agent
        public const string Customer = "Customer";
        public const string User = "User";

        // Combined Roles (used in [Authorize(Roles = ...)] attributes).
        // Legacy — kept for back-compat with existing endpoints.
        public const string AdminOrStaff = "SuperAdmin,Admin,Staff";

        // Counter-only operations (selling tickets, customer lookup).
        public const string CounterRoles = "SuperAdmin,Admin,StaffCounter";

        // Boarding-only operations (manifest, scan, mark Departed/Arrived).
        public const string BoardingRoles = "SuperAdmin,Admin,StaffBoarding";

        // Any station staff — for shared read-only surfaces (Trips, Bookings).
        public const string AnyStaff = "SuperAdmin,Admin,Staff,StaffCounter,StaffBoarding";

        // Policy Names
        public const string AdminPolicy = "AdminPolicy";
        public const string SuperAdminPolicy = "SuperAdminPolicy";
    }
}

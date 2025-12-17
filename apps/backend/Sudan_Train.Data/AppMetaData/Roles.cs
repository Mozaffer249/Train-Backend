namespace Sudan_Train.Data.AppMetaData
{
    /// <summary>
    /// Static class containing role constants for authorization.
    /// </summary>
    public static class Roles
    {
        /// <summary>
        /// Super Administrator role with full system access.
        /// Can manage admins, access all features, and perform system-wide operations.
        /// </summary>
        public const string SuperAdmin = "SuperAdmin";

        /// <summary>
        /// Administrator role for managing railway infrastructure.
        /// Can manage trains, stations, routes, trips, and fares.
        /// </summary>
        public const string Admin = "Admin";

        /// <summary>
        /// Regular user role for customers.
        /// Can search trips, make bookings, and manage their profile.
        /// </summary>
        public const string User = "User";

        /// <summary>
        /// Policy name for requiring either Admin or SuperAdmin role.
        /// Use this for general admin operations.
        /// </summary>
        public const string AdminPolicy = "RequireAdminRole";

        /// <summary>
        /// Policy name for requiring SuperAdmin role only.
        /// Use this for sensitive operations like managing admins.
        /// </summary>
        public const string SuperAdminPolicy = "RequireSuperAdminRole";

        /// <summary>
        /// Comma-separated string of admin roles for use in [Authorize(Roles = ...)] attribute.
        /// </summary>
        public const string AdminRoles = Admin + "," + SuperAdmin;
    }
}

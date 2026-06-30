namespace Sudan_Train.Data.AppMetaData
{
    /// <summary>
    /// Role privilege rules: SuperAdmin manages admins; Admin manages staff/customers only.
    /// </summary>
    public static class RoleHierarchy
    {
        public static readonly HashSet<string> PrivilegedRoles = new(StringComparer.OrdinalIgnoreCase)
        {
            Roles.SuperAdmin,
            Roles.Admin,
        };

        public static readonly string[] AllRoles =
        {
            Roles.SuperAdmin,
            Roles.Admin,
            Roles.Staff,
            Roles.StaffCounter,
            Roles.StaffBoarding,
            Roles.Customer,
            Roles.User,
        };

        public static readonly string[] AdminAssignableRoles =
        {
            Roles.Staff,
            Roles.StaffCounter,
            Roles.StaffBoarding,
            Roles.Customer,
            Roles.User,
        };

        public static bool IsSuperAdmin(IEnumerable<string> callerRoles) =>
            callerRoles.Any(r => string.Equals(r, Roles.SuperAdmin, StringComparison.OrdinalIgnoreCase));

        public static bool IsPrivilegedRole(string role) =>
            PrivilegedRoles.Contains(role);

        public static bool TargetHasPrivilegedRole(IEnumerable<string> targetRoles) =>
            targetRoles.Any(IsPrivilegedRole);

        public static bool CanManageUser(IEnumerable<string> callerRoles, IEnumerable<string> targetRoles)
        {
            if (IsSuperAdmin(callerRoles))
                return true;
            return !TargetHasPrivilegedRole(targetRoles);
        }

        public static bool CanAssignRoles(IEnumerable<string> callerRoles, IEnumerable<string> requestedRoles)
        {
            if (IsSuperAdmin(callerRoles))
                return true;
            return requestedRoles.All(r => !IsPrivilegedRole(r));
        }

        public static IEnumerable<string> GetAssignableRoles(IEnumerable<string> callerRoles) =>
            IsSuperAdmin(callerRoles) ? AllRoles : AdminAssignableRoles;
    }
}

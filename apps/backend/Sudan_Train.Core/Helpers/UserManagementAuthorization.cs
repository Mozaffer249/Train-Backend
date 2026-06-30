using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sudan_Train.Data.AppMetaData;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Infrastructure.context;

namespace Sudan_Train.Core.Helpers
{
    public static class UserManagementAuthorization
    {
        public static List<string> GetCallerRoles(IHttpContextAccessor http) =>
            http.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
            ?? new List<string>();

        public static string PrivilegedUserError =>
            "Only SuperAdmin can manage admin accounts.";

        public static string PrivilegedRoleAssignError =>
            "Only SuperAdmin can assign Admin or SuperAdmin roles.";

        public static string LastSuperAdminError =>
            "Cannot remove or disable the last SuperAdmin account.";

        public static bool CanManageTarget(IEnumerable<string> callerRoles, IEnumerable<string> targetRoles) =>
            RoleHierarchy.CanManageUser(callerRoles, targetRoles);

        public static bool CanAssignRequestedRoles(IEnumerable<string> callerRoles, IEnumerable<string> requestedRoles) =>
            RoleHierarchy.CanAssignRoles(callerRoles, requestedRoles);

        public static async Task<bool> WouldRemoveLastSuperAdminAsync(
            UserManager<User> userManager,
            ApplicationDBContext db,
            User target,
            IEnumerable<string> requestedRoles)
        {
            var current = await userManager.GetRolesAsync(target);
            if (!current.Contains(Roles.SuperAdmin))
                return false;
            if (requestedRoles.Contains(Roles.SuperAdmin))
                return false;

            return await CountSuperAdminsAsync(db) <= 1;
        }

        public static async Task<bool> IsLastSuperAdminAsync(ApplicationDBContext db, User target, UserManager<User> userManager)
        {
            var roles = await userManager.GetRolesAsync(target);
            if (!roles.Contains(Roles.SuperAdmin))
                return false;
            return await CountSuperAdminsAsync(db) <= 1;
        }

        private static async Task<int> CountSuperAdminsAsync(ApplicationDBContext db)
        {
            return await db.UserRoles
                .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur, r })
                .Where(x => x.r.Name == Roles.SuperAdmin)
                .Select(x => x.ur.UserId)
                .Distinct()
                .CountAsync();
        }
    }
}

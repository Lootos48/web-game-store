using GameStore.DomainModels.Enums;
using GameStore.PL.App_Code;
using System;
using System.Security.Claims;

namespace GameStore.PL.Util.Authorization
{
    public static class HierarchicalValidator
    {
        public static bool IsUserAdminAndOwner(string username, ClaimsPrincipal user)
        {
            if (string.IsNullOrEmpty(user.Identity.Name))
            {
                return false;
            }
            return user.Identity.Name == username && IsUserAdmin(user);
        }

        public static bool IsUserAdminAndOwner(Guid? userId, ClaimsPrincipal user)
        {
            var currentUserId = ClaimsHelper.GetUserId(user.Claims);

            if (!currentUserId.HasValue || !userId.HasValue)
            {
                return false;
            }

            return IsCurrentUserOwner(currentUserId.Value, userId.Value) && IsUserAdmin(user);
        }

        public static bool IsUserManagerOrOwner(Guid? userId, ClaimsPrincipal user)
        {
            var currentUserId = ClaimsHelper.GetUserId(user.Claims);
            var userRole = ClaimsHelper.GetUserRole(user.Claims);

            if (!currentUserId.HasValue)
            {
                return false;
            }

            if (!userId.HasValue)
            {
                return IsUserManagerOrHigher(userRole);
            }

            return IsCurrentUserOwner(userId.Value, currentUserId.Value) || IsUserManagerOrHigher(userRole);
        }

        public static bool IsUserAdminOrOwner(string username, ClaimsPrincipal user)
        {
            if (string.IsNullOrEmpty(user.Identity.Name))
            {
                return false;
            }

            return user.Identity.Name == username || IsUserAdmin(user);
        }

        public static bool IsUserAdminOrOwner(Guid? userId, ClaimsPrincipal user)
        {
            var currentUserId = ClaimsHelper.GetUserId(user.Claims);

            if (!currentUserId.HasValue || !userId.HasValue)
            {
                return false;
            }

            return IsCurrentUserOwner(currentUserId.Value, userId.Value) || IsUserAdmin(user);
        }

        public static bool IsCurrentUserOwner(Guid userId, Guid currentUserId)
        {
            return currentUserId == userId;
        }

        public static bool IsCurrentUserOwner(Guid userId, ClaimsPrincipal User)
        {
            var currentUserId = ClaimsHelper.GetUserId(User.Claims);
            if (!currentUserId.HasValue)
            {
                return false;
            }

            return currentUserId == userId;
        }

        public static bool IsUserAdmin(ClaimsPrincipal user)
        {
            return user.IsInRole(UserRoles.Administrator.ToString());
        }

        public static bool IsUserManagerOrHigher(UserRoles userRole)
        {
            return userRole >= UserRoles.Manager;
        }
    }
}

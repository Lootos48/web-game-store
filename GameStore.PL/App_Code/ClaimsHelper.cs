using GameStore.DomainModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace GameStore.PL.App_Code
{
    public static class ClaimsHelper
    {
        public static Guid? GetUserId(IEnumerable<Claim> claims)
        {
            var userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return userId;
            }

            return null;
        }

        public static UserRoles GetUserRole(IEnumerable<Claim> claims)
        {
            var userRoleClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);

            if (userRoleClaim != null && Enum.TryParse(userRoleClaim.Value, out UserRoles role))
            {
                return role;
            }

            return UserRoles.Guest;
        }

        public static string GetUserEmail(IEnumerable<Claim> claims)
        {
            var userEmailClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (userEmailClaim != null)
            {
                return userEmailClaim.Value;
            }

            return null;
        }
    }
}

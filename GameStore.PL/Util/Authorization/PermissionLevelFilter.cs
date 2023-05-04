using GameStore.DomainModels.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;

namespace GameStore.PL.Util.Authorization
{
    public class PermissionLevelFilter : IAuthorizationFilter
    {
        private readonly UserRoles _minLevel;

        public PermissionLevelFilter(UserRoles minLevel)
        {
            _minLevel = minLevel;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Claim roleClaim = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultRoleClaimType);

            if (IsAccessDenied(roleClaim))
            {
                context.Result = new ForbidResult();
            }
        }

        private bool IsAccessDenied(Claim roleClaim)
        {
            return roleClaim is null || !Enum.TryParse(roleClaim.Value, out UserRoles role) || role < _minLevel;
        }
    }
}
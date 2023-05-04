using GameStore.DomainModels.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.PL.Util.Authorization
{
    public class PermissionLevelAttribute : TypeFilterAttribute
    {
        public PermissionLevelAttribute(UserRoles minLevel) : base(typeof(PermissionLevelFilter))
        {
            Arguments = new object[] { minLevel };
        }
    }
}
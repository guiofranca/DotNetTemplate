using Microsoft.AspNetCore.Authorization;
using Template.Domain.Enums;

namespace Template.Api.v1.Attributes;

public class AuthorizeRoleAttribute : AuthorizeAttribute
{
    public AuthorizeRoleAttribute(params Roles[] roles)
    {
        var allowedRolesAsStrings = roles.Select(x => Enum.GetName(typeof(Roles), x));
        Roles = string.Join(",", allowedRolesAsStrings);
    } 
}
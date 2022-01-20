using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace EmployeeManagment.Security
{
    public class SuperAdminHandler :
        AuthorizationHandler<ManageAdminRolesAndClaimsRequirment>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirment requirement)
        {
            if (context.User.IsInRole("Super Admin"))
            {
                context.Succeed(requirement);

            }
            return Task.CompletedTask;
        }
    }
}

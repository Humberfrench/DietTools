using Microsoft.AspNetCore.Authorization;

namespace Dietcode.Api.Core.Middleware
{

    public class DebugAuthHandler : AuthorizationHandler<DebugAuthRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            DebugAuthRequirement requirement)
        {
            // Sempre aprova
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

}

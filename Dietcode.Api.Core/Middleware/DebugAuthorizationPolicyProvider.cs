namespace Dietcode.Api.Core.Middleware
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Options;
    using System.Threading.Tasks;

    public class DebugAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public DebugAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options)
        {
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName == "AllowDebug")
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new DebugAuthRequirement())
                    .Build();

                return policy;
            }

            return (await base.GetPolicyAsync(policyName))!;
        }
    }
}

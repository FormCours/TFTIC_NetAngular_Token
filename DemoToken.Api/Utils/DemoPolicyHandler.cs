using DemoToken.Toolbox.Token;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoToken.Api.Utils
{
    public class DemoRequirement : IAuthorizationRequirement {}

    public class DemoPolicyHandler : AuthorizationHandler<DemoRequirement> 
    {
        private TokenManager TokenManager { get; }

        public DemoPolicyHandler(TokenManager tokenManager)
        {
            this.TokenManager = tokenManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DemoRequirement requirement)
        {
            TokenData data = TokenManager.GetData(context.User);

            // Your code here ;)

            if (data.Username.ToLower() == "riri")
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }


            return Task.CompletedTask;
        }

    }
}

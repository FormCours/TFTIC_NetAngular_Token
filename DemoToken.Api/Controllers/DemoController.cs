using DemoToken.Toolbox.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DemoToken.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DemoController : ControllerBase
    {
        private TokenManager TokenManager { get; }

        public DemoController(TokenManager tokenManager)
        {
            this.TokenManager = tokenManager;
        }

        [HttpGet]
        [Route("/nologin")]
        [AllowAnonymous]
        public IActionResult NoLogin()
        {
            return Ok(new
            {
                message = "Hello World"
            });
        }

        [HttpGet]
        [Route("/login")]
        //[Authorize(Roles = "Admin,User")] => Pour limiter au roles "admin" ou "user"
        public IActionResult Login()
        {
            //ClaimsPrincipal cp = HttpContext.User;
            //string username = cp.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            TokenData data = TokenManager.GetData(HttpContext.User);

            return Ok(new
            {
                message = $"Hello {data.Username}"
            });
        }

        [HttpGet]
        [Route("/admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult LoginAdmin()
        {
            return Ok(new
            {
                message = "Bonjour monsieur l'administrateur :)"
            });
        }



        [HttpGet]
        [Route("/test_policy")]
        [Authorize(Policy = "DemoPolicy")]
        public IActionResult TestPolicy()
        {
            return Ok(new
            {
                message = "Test de la Policy"
            });
        }
    }
}

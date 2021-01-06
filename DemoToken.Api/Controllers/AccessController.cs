using DemoToken.Api.Models;
using DemoToken.Toolbox.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoToken.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccessController : ControllerBase
    {
        protected TokenManager TokenManager { get; }

        public AccessController(TokenManager tokenManager)
        {
            this.TokenManager = tokenManager;
        }

        [HttpGet] 
        public IActionResult Get()
        {
            return Ok("Hello World");
        }

        [HttpPost] 
        public IActionResult Login([FromBody] UserCredential userCredential)
        {
            if (userCredential?.Username == null || userCredential?.Password == null)
            {
                return BadRequest();
            }

            #region Fake Check user in Database
            int userId;
            string username, role;

            if(userCredential.Username.ToLower() == "balthazar" && userCredential.Password == "1234.")
            {
                userId = 42;
                username = userCredential.Username;
                role = "Admin";
            }
            else if (userCredential.Username.ToLower() == "riri" || userCredential.Username.ToLower() == "zaza")
            {
                userId = 12;
                username = userCredential.Username;
                role = "User";
            }
            else
            {
                return StatusCode(403);
            }
            #endregion

            string token = TokenManager.GenerateJwt(new TokenData()
            {
                UserId = userId,
                Username = username,
                Role = role
            });

            return Ok(token);
        }
    }
}

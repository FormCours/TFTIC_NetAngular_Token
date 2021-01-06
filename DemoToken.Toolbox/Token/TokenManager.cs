using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;

namespace DemoToken.Toolbox.Token
{
    public class TokenManager
    {
        private IConfiguration _config;

        public TokenManager(IConfiguration configuration)
        {
            _config = configuration;
        } 

        /// <summary>
        /// Permet de générer un token sur base de la configuration "Jwt" et des données
        /// </summary>
        /// <param name="data">Données à ajouter au token</param>
        /// <returns>Token sous format string</returns>
        public string GenerateJwt(TokenData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            // Création de la date d'expiration du token
            DateTime experationDate = DateTime.Now.AddMinutes(double.Parse(_config["Jwt:LifeTime"]));

            // Création des credentials
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            // Création des "claims" qui contient les informations du token
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Name, data.Username),
                new Claim("UserId", data.UserId.ToString()),
                new Claim(ClaimTypes.Role, data.Role)
            };

            // Génération du token via les outils JWT (package: System.IdentityModel.Tokens.Jwt)
            JwtSecurityToken token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: experationDate,
                    signingCredentials: credentials
                );

            // Renvoyer le token générer
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Permet de récuperer les données du token qui ont stocké dans un "ClaimsPrincipal"
        /// </summary>
        /// <param name="principal">Le context securité "ClaimsPrincipal"</param>
        /// <returns>Les données qui été contenu dans le token</returns>
        public TokenData GetData(ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            if (!principal.HasClaim(c => c.Type == ClaimTypes.Name)
                    && !principal.HasClaim(c => c.Type == "UserId")
                    && !principal.HasClaim(c => c.Type == ClaimTypes.Role))
                throw new SecurityTokenException("Missing claim !");

            return new TokenData()
            {
                UserId = ExtractClaim<int>(principal, "UserId"),
                Username = ExtractClaim(principal, ClaimTypes.Name),
                Role = ExtractClaim(principal, ClaimTypes.Role)
            };
        }

        /// <summary>
        /// Permet d'extrait du ClaimsPrincipal un Claim particulier en fonction de son type
        /// </summary>
        /// <param name="principal">Le context securité "ClaimsPrincipal"</param>
        /// <param name="typeClaim">La type ciblé</param>
        /// <returns>La valeur contenu par le Claim</returns>
        private string ExtractClaim(ClaimsPrincipal principal, string typeClaim)
        {
            return principal.Claims.SingleOrDefault(c => c.Type == typeClaim)?.Value;
        }

        /// <summary>
        /// Permet d'extrait du ClaimsPrincipal un Claim particulier en fonction de son type
        /// </summary>
        /// <typeparam name="T">Type de valeur retourné</typeparam>
        /// <param name="principal">Le context securité "ClaimsPrincipal"</param>
        /// <param name="typeClaim">La type ciblé</param>
        /// <returns>La valeur contenu par le Claim</returns>
        private T ExtractClaim<T>(ClaimsPrincipal principal, string typeClaim)
        {
            string valueClaim = ExtractClaim(principal, typeClaim);
            return (T)Convert.ChangeType(valueClaim, typeof(T));
        }
    }
}

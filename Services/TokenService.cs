using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Store_Dapper.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Services
{
    public class TokenService
    {
        private readonly string _privateKey;
        public TokenService(IConfiguration configuration)
        {
            _privateKey = configuration.GetSection("PrivateKey").GetSection("Secret").Value;
        }
        public string GenerateToken(Admin admin)
        {
            //cria a variavel do token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            //encripta a chave privada
            var key = Encoding.ASCII.GetBytes(_privateKey);

            //define o descritor do token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, admin.Name.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            //cria o token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //retorna o token
            return tokenHandler.WriteToken(token);
        }
    }
}

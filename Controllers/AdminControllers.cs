using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_Dapper.WebAPI.Models;
using Store_Dapper.WebAPI.Models.RepositoryInterfaces;
using Store_Dapper.WebAPI.Services;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Store_Dapper.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminControllers : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;
        private readonly TokenService _tokenService;

        public AdminControllers(IAdminRepository adminRepository, TokenService tokenService)
        {
            _adminRepository = adminRepository;
            _tokenService = tokenService;
        }

        // GET api/<AdminControllers>/5
        /// <summary>
        /// Generate a token for Admin
        /// </summary>
        /// <remarks>
        /// Example:
        /// 
        ///     name = Gisely
        ///     password = 123xl
        ///     
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <response code = '200'>Success</response>
        /// <response code = '400'>Bad Request: Exception catched</response>
        /// <response code = '404'>Not found: Admin not found</response>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{name}, {password}")]
        [ProducesResponseType(typeof(Admin), 200)]
        [ProducesErrorResponseType(typeof(BadRequestResult))]
        public IActionResult Get(string name, string password)
        {
            try
            {
                var admin = _adminRepository.GetAdmin(name, password);
                
                //checks if admin exists
                if (admin == null)
                    return NotFound(new { message = "Admin not found" });

                //cover the password
                var pass = admin.Password.Length;
                var token = _tokenService.GenerateToken(admin);

                admin.Password = "";
                
                for (int i = 0; i < pass; i++)
                {
                    admin.Password += "*";
                }

                //return token and admin
                return Accepted(new
                {
                    admin = admin,
                    token = token
                }); ;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

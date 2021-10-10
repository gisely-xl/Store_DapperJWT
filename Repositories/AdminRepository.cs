using Dapper;
using Microsoft.Extensions.Configuration;
using Store_Dapper.WebAPI.Models;
using Store_Dapper.WebAPI.Models.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly string _connectString;

        public AdminRepository(IConfiguration configuration)
        {
            _connectString = configuration.GetConnectionString("Default");
        }

        public Admin GetAdmin(string name, string password)
        {
            using var connection = new SqlConnection(_connectString);
            var getAdminQuery = $"SELECT A.* FROM Admin A WHERE A.Name = '{name}' AND A.Password = '{password}'";
            var adminDetected = connection.QueryFirstOrDefault<Admin>(getAdminQuery, new { Name = name, Password = password});
            return adminDetected;
        }

    }
}

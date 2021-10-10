using Dapper;
using Microsoft.Extensions.Configuration;
using Store_Dapper.WebAPI.Models;
using Store_Dapper.WebAPI.Models.RepositoryInterfaces;
using Store_Dapper.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly string _connectString;
        private readonly ViaCepService _viaCepService;
       
        public AddressRepository(IConfiguration configuration, ViaCepService viaCepService)
        {
            _connectString = configuration.GetConnectionString("Default");
            _viaCepService = viaCepService;
           
        }

        public Address Create(long cep)
        {
            
            var address = _viaCepService.GetAddress(cep).Result;
            
            //checks if exist that cep on api ViaCep
            if (address.Cep == null)
            {
                return address;//null
            }
            else
            {
                using var connection = new SqlConnection(_connectString);
                var createQuery = $"INSERT INTO ViaCep (Cep, Logradouro, UF, Localidade, Bairro, DDD, Complemento, SIAFI, GIA, IBGE)" +
                    $" VALUES (@Cep, @Logradouro, @UF, @Localidade, @Bairro, @DDD, @Complemento, @SIAFI, @GIA, @IBGE)";

                connection.Execute(createQuery, new
                {
                    address.Cep,
                    address.Logradouro,
                    address.UF,
                    address.Localidade,
                    address.Bairro,
                    address.DDD,
                    address.Complemento,
                    address.SIAFI,
                    address.GIA,
                    address.IBGE
                });

                return address;
            }
           
        }

        public async Task Delete(string cep)
        {
            using var connection = new SqlConnection(_connectString);
            var deletequery = $"DELETE FROM ViaCep WHERE Cep = '{cep}'";
            await connection.ExecuteAsync(deletequery, new { Cép = cep });

        }

        public Address GetAddress(string cep)
        {
            using var connection = new SqlConnection(_connectString);
            var getCepQuery = $"SELECT * FROM ViaCep WHERE Cep = '{cep}'";
            var address = connection.QueryFirstOrDefault<Address>(getCepQuery);
            return address;
        }

        public IEnumerable<Address> GetAddresses()
        {
            using var connection = new SqlConnection(_connectString);
            var getQuery = $"SELECT * FROM ViaCep";
            var adresses = SqlMapper.Query<Address>(connection, getQuery);
            return adresses;
        }

        public Address Update(string oldCep, long newCep)
        {
            
            var newAddress = _viaCepService.GetAddress(newCep).Result;

            using var connection = new SqlConnection(_connectString);
            var updateQuery = $"UPDATE ViaCep SET " +
                $"Cep=@Cep, Logradouro=@Logradouro, UF=@UF, Localidade=@Localidade, Bairro=@Bairro, DDD=@DDD, Complemento=@Complemento, SIAFI=@SIAFI, GIA=@GIA, IBGE=@IBGE " +
                $"WHERE Cep='{oldCep}'";

            connection.Execute(updateQuery, new {
                newAddress.Cep,
                newAddress.Logradouro,
                newAddress.UF,
                newAddress.Localidade,
                newAddress.Bairro,
                newAddress.DDD,
                newAddress.Complemento,
                newAddress.IBGE,
                newAddress.GIA,
                newAddress.SIAFI,
            });
            return newAddress;
        }
    }
}

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
    public class ClientRepository : IClientRepository
    {
        private readonly string _connectionString;
        private readonly IAddressRepository _addressRepository;

        public ClientRepository(IConfiguration configuration, IAddressRepository addressRepository)
        {
            _connectionString = configuration.GetConnectionString("Default");
            _addressRepository = addressRepository;
        }

        public ClientDb Create(ClientDb newClient)
        {
            //Verify the existence of the address given
            var getAddress = _addressRepository.GetAddress(newClient.Cep_C);
           
            //Create an address if the result is null which means that that address is not on database
            if (getAddress == null)
            {
                //Extracts the numbers of a string to be converted in long type
                var stringe = String.Join("", System.Text.RegularExpressions.Regex.Split(newClient.Cep_C, @"[^\d]"));
                _addressRepository.Create(Int64.Parse(stringe));
            }

            //Open dapper connection with the database and fill the fields with the values of the object Client
            using var connection = new SqlConnection(_connectionString);
            var createQuery = $"INSERT CLIENTS (Name, Contact, Cep_C) VALUES (@Name, @Contact, @Cep_C)";
            connection.Execute(createQuery, new
            {
                newClient.Name,
                newClient.Contact,
                newClient.Cep_C
            });

            //insert the info of the client address given trough address repository method
            newClient.Address = _addressRepository.GetAddress(newClient.Cep_C);

            //Format the output of the Id return to be the one associated with the info
            var queryId = $"select max(Id) as last_id from CLIENTS";
            var ids = connection.Query(queryId);
            newClient.Id = ids.ToList()[0].last_id;
            
            return newClient;
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var deleteQuery = $"DELETE FROM CLIENTS WHERE Id = {id}";
            await connection.ExecuteAsync(deleteQuery, new { Id = id });
        }

        public Client GetClient(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var getByIdQuery = $"SELECT * FROM CLIENTS WHERE Id = {id}";
            var client = connection.QueryFirstOrDefault<Client>(getByIdQuery, new { Id = id });

            var getAddress = $"SELECT A.* FROM ViaCep A JOIN CLIENTS C ON C.Cep_C = A.Cep WHERE C.Name = '{client.Name}'";
            client.Adresses = SqlMapper.Query<Address>(connection, getAddress);

            return client;
        }

        public IEnumerable<Client> GetClients()
        {
            var getQuery = "SELECT * FROM CLIENTS";
            using var connection = new SqlConnection(_connectionString);
            var clients = SqlMapper.Query<Client>(connection, getQuery);

            
            foreach (var client in clients)
            {
                var getAddress = $"SELECT A.* FROM ViaCep A JOIN CLIENTS C ON C.Cep_C = A.Cep WHERE C.Name = '{client.Name}'";
                client.Adresses = SqlMapper.Query<Address>(connection, getAddress);
            }

            return clients;
        }

        public ClientDb Update(ClientDb client)
        {
            var getAddress = _addressRepository.GetAddress(client.Cep_C);
            if (getAddress == null)
            {
                var stringe = String.Join("", System.Text.RegularExpressions.Regex.Split(client.Cep_C, @"[^\d]"));
                _addressRepository.Create(Int64.Parse(stringe));
            }

            using var connection = new SqlConnection(_connectionString);
            var updateQuery = $"UPDATE CLIENTS SET Name=@Name, Contact=@Contact, Cep_C=@Cep_C WHERE Id=@Id";
            connection.Execute(updateQuery, new
            {
                client.Id,
                client.Name,
                client.Contact,
                client.Cep_C
            });

            client.Address = _addressRepository.GetAddress(client.Cep_C);

            return client;
        }

    }
}

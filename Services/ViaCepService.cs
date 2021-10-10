using Newtonsoft.Json;
using Store_Dapper.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store_Dapper.WebAPI.Services
{
    public class ViaCepService
    {
        private readonly HttpClient _httpClient;

        public ViaCepService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Address> GetAddress(long cep)
        {
            
            _httpClient.BaseAddress = new System.Uri("https://viacep.com.br/ws/");
            var response = await _httpClient.GetAsync($"{cep}/json");
            var jsonString = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                var address = JsonConvert.DeserializeObject<Address>(jsonString);
                return address;
            }
            else
            {
                return null;
            }

        }

    }
}

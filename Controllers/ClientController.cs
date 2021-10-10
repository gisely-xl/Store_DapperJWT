using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_Dapper.WebAPI.Models;
using Store_Dapper.WebAPI.Models.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Store_Dapper.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        // GET: api/<ClientController>
        /// <summary>
        /// List all clients on database.
        /// </summary>
        /// <response code = '200'>Success</response>
        /// <response code = '400'>Bad Resquest: Exception catched</response>
        /// <response code = '404'>Not found: Clients not found</response>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientRepository.GetClients();
                if (clients == null)
                    return NotFound("Theres no Clients registered");
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<ClientController>/5
        /// <summary>
        /// Show a client by Id
        /// </summary>
        /// <remarks>
        /// Example:
        ///     Id = 1005
        /// </remarks>
        /// <param name="id">Client Id</param>
        /// <response code = '200'>Success</response>
        /// <response code = '400'>Bad Request: Exception catched</response>
        /// <response code = '404'>Not found: Client of this Id not found</response>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Client), 200)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public IActionResult Get(int id)
        {
            try
            {
                var client = _clientRepository.GetClient(id);

                if (client == null)
                    return NotFound(new { meassage = $"Client {id} not found" });

                return Ok(_clientRepository.GetClient(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/<ClientController>
        /// <summary>
        /// Create a new client.
        /// </summary>
        /// <remarks>
        /// Example:
        ///
        ///     POST 
        ///     {
        ///        "name": "Gisely",
        ///        "contact": "88996464849",
        ///        "cep": "62031-222"
        ///      } 
        /// </remarks>
        /// <param name="client">Client to be created</param>
        /// <response code = '201'>Success</response>
        /// <response code = '400'>Bad Request: Read the message</response>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Client), 200)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public IActionResult Post([FromBody] ClientDb client)
        {
            try
            {
                //Test if the client already exist on database
                //Search all clients
                var clients = _clientRepository.GetClients();
                var testExist = false;

                //for each client in the database test if have the same data
                foreach (var obj in clients)
                {
                    if (obj.Name == client.Name && obj.Contact == client.Contact && obj.Cep_C == client.Cep_C)
                    {
                        testExist = true;
                    }
                }

                //if client exist return an statement
                if (testExist == true)
                    return BadRequest("\nClient already exist.\n\tTry Update.");


                _clientRepository.Create(client);
                return CreatedAtAction(nameof(Get), new { client.Id }, client);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<ClientController>/5
        /// <summary>
        /// Update client
        /// </summary>
        /// <remarks>
        /// Example:
        ///
        ///     PUT
        ///     {  
        ///        "id": 2010
        ///        "name": "GISELY",
        ///        "contact": "88996464849",
        ///        "cep": "60010-040"
        ///      } 
        /// </remarks>
        /// 
        /// <param name="client"> Client body to Update</param>
        /// <response code = '200'> Succes</response>
        /// <response code = '400'> Bad Request: Exception catched </response>
        /// <response code = '404'> Not found: Client Id not found</response>
        /// <returns>Client updated</returns>
        [HttpPut]
        [ProducesResponseType(typeof(ClientDb), 200)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public IActionResult Put([FromBody] ClientDb client)
        {
            try
            {
                //checks if client exist
                var client_Existence = _clientRepository.GetClient(client.Id);
                if (client_Existence == null)
                    return NotFound(new { message = $"Client {client.Id} not found" });

                //checks if client contain duplicate data
                var clients = _clientRepository.GetClients();
                var testExist = false;
                foreach (var obj in clients)
                {
                    var nameTest = obj.Name.Equals(client.Name);
                    var cepTest = obj.Cep_C.Equals(client.Cep_C);
                    if (nameTest == true && cepTest == true)
                    {
                        testExist = true;
                    }
                }
                if (testExist == true)
                    return BadRequest($"\t Error to update. '{client.Name}' already own '{client.Cep_C}'");

                //returned value
                var updated = _clientRepository.Update(client);
                return Ok(updated);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        // DELETE api/<ClientController>/5
        /// <summary>
        /// Delete client by id
        /// </summary>
        /// <param name="id"> to Delete</param>
        /// <response code = '204'>Succes</response>
        /// <response code = '400'>Bad Request: Exception catched</response>
        /// <response code = '404'>Not found: Client not found to delete</response>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public IActionResult Delete(int id)
        {
            try
            {
                //checks if client exist
                var search = _clientRepository.GetClient(id);
                if (search == null)
                    return NotFound("\tClient not found. Maybe it's already deleted");

                _clientRepository.Delete(id);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
    }
}

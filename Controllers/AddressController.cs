using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_Dapper.WebAPI.Models;
using Store_Dapper.WebAPI.Models.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Store_Dapper.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IClientRepository _clientRepository;
        public AddressController(IAddressRepository addressRepository, IClientRepository clientRepository)
        {
            _addressRepository = addressRepository;
            _clientRepository = clientRepository;
        }

        // GET: api/<AddressController>
        /// <summary>
        /// List all adresses on Database.
        /// </summary>
        /// <response code = '200'>Success</response>
        /// <response code = '400'>Bad Resquest: Exception catched</response>
        /// <response code = '404'>Not found: Adresses not found</response>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Address>), 200)]
        [ProducesErrorResponseType(typeof(BadRequestResult))]
        public IActionResult Get()
        {
            try
            {
                return Ok(_addressRepository.GetAddresses());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET api/<AddressController>/5
        /// <summary>
        /// Find address from cep 
        /// </summary>
        /// <remarks>
        /// Example:
        ///     cep = 62031020
        /// </remarks>
        /// <param name="cep">Cep</param>
        /// <response code = '200'>Success</response>
        /// <response code = '400'>Bad Request: Exception catched</response>
        /// <response code = '404'>Not found: Address of this cep not found</response>
        /// <returns></returns>
        [HttpGet("{cep}")]
        [ProducesResponseType(typeof(Address), 200)]
        [ProducesErrorResponseType(typeof(BadRequestResult))]
        public IActionResult Get(long cep)
        {
            try
            {
                var cepString = String.Format(@"{0:00000\-000}", cep);
                var address = _addressRepository.GetAddress(cepString);

                if (address == null)
                    _addressRepository.Create(cep);

                address = _addressRepository.GetAddress(cepString);
                return Ok(address);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        // POST api/<AddressController>
        /// <summary>
        /// Create a new Address based on cep
        /// </summary>
        /// <remarks>
        /// Example:
        /// 
        ///     cep: 62031100
        ///    
        /// </remarks>
        /// <response code = '201'> Successfully Created</response>
        /// <response code = '404'>Not found: Cep not found on ViaCep API</response>
        /// <response code = '400'>Bad Request: Exception catched</response>
        /// <param name="cep"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Address), 201)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        [HttpPost("{cep}")]
        public IActionResult Post(long cep)
        {
            try
            {
                var cepString = String.Format(@"{0:00000\-000}", cep);

                //checks if exist on database
                var serchIfExist = _addressRepository.GetAddress(cepString);
                if (serchIfExist != null)
                {
                    return BadRequest("\tThat address already exist on Database");
                }

                //checks if exist that cep on api ViaCep
                var address = _addressRepository.Create(cep);
                if (address.Cep == null)
                    return NotFound();

                return CreatedAtAction(nameof(Get), new { address.Cep }, address);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<AddressController>/5
        /// <summary>
        /// Update an address by cep
        /// </summary>
        /// <remarks>
        /// Example:
        ///
        ///     PUT 
        ///         oCep = 60010010 --old cep
        ///         nCep = 47804112 -- new cep 
        ///     
        /// </remarks>
        /// <param name="oCep">Cep that will be modified</param>
        /// <param name="nCep">Cep that that will be put in</param>
        /// <response code = '204'>No Content: Cep updated</response>
        /// <response code = '400'> Bad Request: Read the message </response>
        /// <returns>Client updated</returns>
        [HttpPut("{oCep}")]
        [ProducesResponseType(204)]
        [ProducesErrorResponseType(typeof(BadRequestResult))]
        public IActionResult Put(long oCep, long nCep)
        {
            try
            {
                var olCepString = String.Format(@"{0:00000\-000}", oCep);
                var nwCepString = String.Format(@"{0:00000\-000}", nCep);

                //if the oldCep is attached with a client conflit in database
                var clients = _clientRepository.GetClients();
                foreach (var client in clients)
                {
                    if (client.Cep_C == olCepString)
                    {
                        return BadRequest($"The Cep: '{olCepString}' is attached with Client: {client.Id}, therefore can't be updated");
                    }
                }

                //if the old cep is not in database
                _addressRepository.GetAddress(olCepString);

                //checks if the new cep already is oon database
                var nwCepDb = _addressRepository.GetAddress(nwCepString);
                if (nwCepDb != null)
                {
                    return BadRequest($"\tCep: '{nwCepDb.Cep}' is registered already");
                }

                _addressRepository.Update(olCepString, nCep);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/<AddressController>/5
        /// <summary>
        /// Delete cep 
        /// </summary>
        /// <remarks>
        /// Example:
        ///    
        ///     DELETE
        ///         cep = 62031064
        /// 
        /// </remarks>
        /// <param name="cep">Cep to be deleted</param>
        /// <response code = '204'>No Content: Cep deleted</response>
        /// <response code = '400'>Bad Request: Read the message</response>
        /// <response code = '404'>Not found: Cep not found to be deleted</response>
        /// <returns></returns>
        [HttpDelete("{cep}")]
        public IActionResult Delete(long cep)
        {
            try
            {

                if (cep.ToString().Length != 8)
                {
                    return BadRequest($"\t The length of Cep: '{cep}' is not valid. A Cep has 8 numbers");
                }
                // the format of the string conform the long passed with the string.fomrat determinated
                var cepString = String.Format(@"{0:00000\-000}", cep);

                var address = _addressRepository.GetAddress(cepString);

                if (address == null)
                    return NotFound($"Cep: '{cepString}' doesn't exist on database");

                _addressRepository.Delete(cepString);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

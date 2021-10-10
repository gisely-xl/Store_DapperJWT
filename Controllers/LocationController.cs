using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store_Dapper.WebAPI.Models;
using Store_Dapper.WebAPI.Models.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Store_Dapper.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationRepository _locationRepository;

        public LocationController(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        // GET: api/<LocationController>
        /// <summary>
        /// List all locations on database
        /// </summary>
        /// <response code = '200'>Success</response>
        /// <response code = '400'>Bad Request: Exception catched</response>
        /// <response code = '401'>Unauthorized: Token necessary</response>
        /// <response code = '404'>Not found: Locations table empty</response>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Location), 200)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public IActionResult Get()
        {
            try
            {
                var clients = _locationRepository.GetLocations();
                if (clients == null)
                {
                    return NotFound(new { message = "\tLocations not found" });
                }
                return Ok(clients);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // GET api/<LocationController>/5
        /// <summary>
        /// Show a location by Id
        /// </summary>
        /// <remarks>
        ///     
        ///     Example:
        ///     
        ///         Id = 6
        ///         
        /// </remarks>   
        /// <param name="id">Location Id</param>
        /// <response code = '200'>Success</response>
        /// <response code = '400'>Bad Request: Exception catched</response>
        /// <response code = '401'>Unauthorized: Token necessary</response>
        /// <response code = '404'>Not found: location</response>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Location), 200)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public IActionResult Get(int id)
        {
            try
            {
                var location = _locationRepository.GetLocation(id);
                if (location == null)
                    return NotFound(new { message = $"Location {id} not found" });
                return Ok(location);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST api/<LocationController>
        /// <summary>
        /// Create a new location
        /// </summary>
        /// <remarks>
        /// Example:
        ///
        ///       POST
        ///         {
        ///            "id": 0,
        ///            "clientId": 1003,
        ///            "filmId": 5,
        ///            "loanDate": "2021-10-10T00:05:06.851Z",
        ///            "returnDate": "2021-10-10T00:05:06.851Z"
        ///        }
        /// 
        ///</remarks>
        ///<response code = '201'>Success</response>
        ///<response code = '400'>Bad Request: Read message</response>
        ///<response code = '401'>Unauthorized: Token necessary</response>
        /// <param name="location">Location to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public IActionResult Post([FromBody] LocationDb location)
        {
            try
            {
                var cLoc = _locationRepository.Create(location);
                var loc = _locationRepository.GetLocation(cLoc.Id);
                return CreatedAtAction(nameof(Get), new { loc.Id }, loc);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // PUT api/<LocationController>/5
        /// <summary>
        /// Update location
        /// </summary>
        /// <remarks>
        /// Example:
        ///
        ///       PUT
        ///         {
        ///            "id": 1,         
        ///            "clientId": 1003, 
        ///            "filmId": 5,
        ///            "loanDate": "2021-10-10T00:05:06.851Z",
        ///            "returnDate": "2021-10-10T00:05:06.851Z"
        ///        }
        ///        
        /// </remarks>
        /// <response code = '201'>Success</response>
        /// <response code = '400'>Bad Request: Exception catched</response>
        /// <response code = '401'>Unauthorized: Token necessary</response>
        /// <response code = '404'>Not found: Location to updated</response>
        /// <param name="location"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(201)]
        [ProducesErrorResponseType(typeof(BadRequestObjectResult))]
        public IActionResult Put([FromBody] LocationDb location)
        {
            try
            {
                var loc = _locationRepository.GetLocation(location.Id);
                if (loc == null)
                    return NotFound(new {message = $"Location: {location.Id} not found"});

                var locDb = _locationRepository.Update(location);
                var loca = _locationRepository.GetLocation(locDb.Id);
                return Ok(loca);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE api/<LocationController>/5
        /// <summary>
        /// Delete location by id
        /// </summary>
        /// <remarks>
        /// Example:
        ///     
        ///     Id = 6
        ///     
        /// </remarks>
        /// <response code = '204'>Success</response>
        /// <response code = '400'>Bad Request: Exception catched</response>
        /// <response code = '401'>Unauthorized: Token necessary</response>
        /// <response code = '404'>Not found: Location to delete</response>
        /// <param name="id">Location Id</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Location), 204)]
        [ProducesErrorResponseType(typeof(BadRequestResult))]
        public IActionResult Delete(int id)
        {
            try
            {
                var location = _locationRepository.GetLocation(id);
                if (location == null)
                    return BadRequest($"\t {id} is not on database");

                _locationRepository.Delete(id);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}




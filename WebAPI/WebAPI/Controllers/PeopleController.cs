using Elasticsearch.Net;

using Microsoft.AspNetCore.Mvc;

using Nest;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;

        public PeopleController()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("people");

            _elasticClient = new ElasticClient(settings);
        }

        // GET: api/<PersonsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Searching
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Person>>> Get([FromQuery] string firstName)
        {
            var searchResponse = await _elasticClient.SearchAsync<Person>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.FirstName)
                        .Query(firstName)
                    )
                )
            );

            var people = searchResponse.Documents;
            return Ok(people);
        }

        /// <summary>
        /// Aggregations
        /// </summary>
        [HttpGet("aggregations")]
        public async Task<ActionResult<string>> GetAggregations([FromQuery] string firstName)
        {
            var searchResponse = await _elasticClient.SearchAsync<Person>(s => s
                .Size(0)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.FirstName)
                        .Query(firstName)
                    )
                ).Aggregations(a => a
                    .Terms("last_names", ta => ta
                        .Field(f => f.LastName)
            )));

            var termsAggregation = searchResponse.Aggregations.Terms("last_names");
            return Ok(termsAggregation);
        }

        // POST api/<PersonsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Person person)
        {
            var asyncIndexResponse = await _elasticClient.IndexDocumentAsync(person);

            if (asyncIndexResponse.Result == Result.Created)
                return Ok();

            return BadRequest();
        }

        // PUT api/<PersonsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PersonsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

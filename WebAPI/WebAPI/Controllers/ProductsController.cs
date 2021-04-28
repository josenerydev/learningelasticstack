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
    public class ProductsController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;

        public ProductsController()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("amazon_products");

            _elasticClient = new ElasticClient(settings);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            var searchResponse = await _elasticClient.SearchAsync<Product>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .MatchAll()
                )
            );

            var products = searchResponse.Documents;
            return Ok(products);
        }

        // GET api/<ProductsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ProductsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Person person)
        {
            var client = new ElasticClient();
            var asyncIndexResponse = await client.IndexDocumentAsync(person);

            return Ok(asyncIndexResponse);
        }

        // PUT api/<ProductsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

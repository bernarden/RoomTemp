using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using RoomTemp.Models.GraphQL;

namespace RoomTemp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphQlController : ControllerBase
    {
        private readonly SensorQuery _sensorQuery;

        public GraphQlController(SensorQuery sensorQuery)
        {
            _sensorQuery = sensorQuery ?? throw new ArgumentNullException(nameof(sensorQuery));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQlQuery query)
        {
            var schema = new Schema { Query = _sensorQuery };

            var result = await new DocumentExecuter().ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.Query = query.Query;

            }).ConfigureAwait(false);

            if (result.Errors?.Count > 0)
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}
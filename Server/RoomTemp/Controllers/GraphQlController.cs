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
        private readonly SensorMutation _sensorMutation;

        public GraphQlController(SensorQuery sensorQuery, SensorMutation sensorMutation)
        {
            _sensorQuery = sensorQuery ?? throw new ArgumentNullException(nameof(sensorQuery));
            _sensorMutation = sensorMutation ?? throw new ArgumentNullException(nameof(sensorMutation));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQlQuery query)
        {
            var schema = new Schema { Query = _sensorQuery, Mutation = _sensorMutation};

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
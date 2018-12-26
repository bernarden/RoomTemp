using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RoomTemp.Models.GraphQL;

namespace RoomTemp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphQlController : ControllerBase
    {
        private readonly IDocumentExecuter _documentExecuter;
        private readonly ISchema _schema;

        public GraphQlController(IDocumentExecuter documentExecuter, ISchema schema)
        {
            _documentExecuter = documentExecuter ?? throw new ArgumentNullException(nameof(documentExecuter));
            _schema = schema ?? throw new ArgumentNullException(nameof(schema));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQlQuery query)
        {
            var extractedApiKey = ExtractApiKey();
            if (!extractedApiKey.HasValue || extractedApiKey.Value.Equals(Startup.Configuration.GetValue<Guid>("GlobalAdminKey")))
            {
                return Unauthorized();
            }

            var executionOptions = new ExecutionOptions { Schema = _schema, Query = query.Query };
            var result = await _documentExecuter.ExecuteAsync(executionOptions).ConfigureAwait(false);

            if (result.Errors?.Count > 0)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }

        private Guid? ExtractApiKey()
        {
            const string iotApiKeyParameterName = "ApiKey";
            if (!Request.Headers.ContainsKey(iotApiKeyParameterName))
            {
                return null;
            }

            var values = Request.Headers[iotApiKeyParameterName];
            if (values.Count != 1)
            {
                return null;
            }

            if (Guid.TryParse(values.First(), out var result))
            {
                return result;
            }

            return null;
        }
    }
}
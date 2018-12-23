using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json.Linq;
using Xunit;

namespace RoomTemp.Tests
{
    public class GraphQlControllerTest
    {
        private readonly HttpClient _client;

        public GraphQlControllerTest()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("LocalTests")
                .UseStartup<Startup>()
            );
            _client = server.CreateClient();
        }
        
        [Fact(Skip = "Fix it")]
        [Trait("test", "integration")]
        public async void ExecuteQuery()
        {
            // Given
            const string query = @"{
                ""query"": 
                    ""query HeroNameQuery {
                        hero {
                            name
                        }
                    }""
            }";
            var content = new StringContent(query, Encoding.UTF8, "application/json");

            // When
            var response = await _client.PostAsync("/graphql", content);

            // Then
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseString);
            var jobj = JObject.Parse(responseString);
            Assert.NotNull(jobj);
            Assert.Equal("R2-D2", (string)jobj["data"]["hero"]["name"]);
        } 
    }
}
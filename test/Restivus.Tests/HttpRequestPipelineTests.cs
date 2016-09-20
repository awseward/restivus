using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace Restivus.Tests
{
    public class HttpRequestPipelineTests
    {
        public class GitHubWebApi : IWebApi
        {
            public string BasePath { get; } = "/";
            public string Host { get; } = "api.github.com";
            public int? Port { get; }
            public string Scheme { get; } = "https";
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class GitHubUser
        {
            [JsonProperty("id")]
            int Id { get; set; }

            [JsonProperty("login")]
            string Login { get; set; }

            [JsonProperty("followers_url")]
            string FollowersUrl { get; set; }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class GitHubUserSearchResults
        {
            [JsonProperty("total_count")]
            int TotalCount { get; set; }

            [JsonProperty("incomplete_results")]
            bool IncompleteResults { get; set; }

            [JsonProperty("items")]
            List<GitHubUser> Users { get; set; }
        }

        [Fact]
        public async Task PipelineApproachWorks()
        {
            var results = await new GitHubWebApi()
                .Get("/search/users")
                .WithQueryParam("q", "dude")
                .WithQueryParam("per_page", "100")
                .WithHeader("User-Agent", "Pipelines/1.0")
                .SendAsync(
                    async response =>
                    {
                        response.EnsureSuccessStatusCode();

                        return JsonConvert.DeserializeObject<GitHubUserSearchResults>(
                            await response.Content.ReadAsStringAsync()
                        );
                    }
                );

            //var request = findMe.Run();

            //Assert.Equal(
            //    new Uri("https://api.github.com/search/users?q=awseward"),
            //    request.RequestUri
            //);

            //Assert.Equal(
            //    "world",
            //    request.Headers.GetValues("X-Hello").Single()
            //);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Restivus.Tests
{
    public class GitHubExample
    {
        static HttpClient _DefaultHttpClient { get; } = new Func<HttpClient>(() =>
        {
            var client = new HttpClient();

            // GitHub freaks out if you don't send a user agent
            client.DefaultRequestHeaders.Add("User-Agent", "Restivus.Tests");

            return client;
        }).Invoke();

        class GitHubWebApi : IWebApi
        {
            public string BasePath { get; } = "/";
            public string Host { get; } = "api.github.com";
            public int? Port { get; }
            public string Scheme { get; } = "https";
        }

        class GitHubRestClient : IRestClient
        {
            public IReadOnlyCollection<IHttpRequestMiddleware> RequestMiddlewares { get; } =
                new List<IHttpRequestMiddleware>().AsReadOnly();

            public IHttpRequestSender RequestSender { get; } = new HttpRequestSender(_DefaultHttpClient);

            public IWebApi WebApi { get; } = new GitHubWebApi();
        }

        class User
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("login")]
            public string Login { get; set; }

            [JsonProperty("html_url")]
            public string ProfileUrl { get; set; }
        }

        [Fact]
        public async Task GetUsers()
        {
            var client = new GitHubRestClient();

            var users = await client.Get().SendAsync(
                "/users",
                _DeserializeMany<User>
            );

            Assert.NotEmpty(users);
        }

        [Fact]
        public async Task CancellationIsSupported()
        {
            var tokenSource = new CancellationTokenSource();
            var client = new GitHubRestClient();

            tokenSource.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(
                () => client.Get().SendAsync(
                    "/users",
                    _DeserializeMany<User>,
                    tokenSource.Token
                )
            );
        }

        static async Task<T> _Deserialize<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        static async Task<IEnumerable<T>> _DeserializeMany<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<IEnumerable<T>>(await response.Content.ReadAsStringAsync());
        }
    }
}

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

        [Fact(Skip = "This keeps failing on AppVeyor (HTTP 403). I think GitHub doesn't like AppVeyoror something. Skipping until I can figure out why.")]
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
        public async Task Support_ThrowBased_ErrorApproach()
        {
            var client = new GitHubRestClient();

            await Assert.ThrowsAsync<HttpRequestException>(
                () => client.Get().SendAsync(
                    $"/{Guid.NewGuid().ToString()}",
                    _DeserializeMany<User>
                )
            );
        }

        [Fact(Skip = "Skip temporarily until this error can be addressed: https://ci.appveyor.com/project/datNET/restivus/build/1.0.101")]
        public async Task Support_Nonthrowing_ErrorApproaches()
        {
            var client = new GitHubRestClient();

            // This example seems a little contrived, but maybe sometime you might
            // just want to make a request and know nothing more than whether or
            // not it was successful, etc.
            var wasSuccessful = await client.Post(_AsJsonContent).SendAsync(
                $"/users",
                new { data = "garbage" },
                response => Task.FromResult(response.IsSuccessStatusCode)
            );

            Assert.False(wasSuccessful);
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

        static HttpContent _AsJsonContent(object thing)
        {
            return JsonConvert.SerializeObject(thing).AsJsonContent();
        }

        static async Task<T> _Deserialize<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        static Task<IEnumerable<T>> _DeserializeMany<T>(HttpResponseMessage response) =>
            _Deserialize<IEnumerable<T>>(response);
        }
}

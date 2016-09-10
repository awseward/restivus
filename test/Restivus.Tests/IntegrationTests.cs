using Newtonsoft.Json;
using Restivus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Restivus.Tests
{
    public class IntegrationTests
    {
        static HttpClient DefaultHttpClient { get; } = new HttpClient();

        class DummyWebApi : IWebApi
        {
            public string BasePath { get; } = "/";
            public string Host { get; } = "jsonplaceholder.typicode.com";
            public int? Port { get; }
            public string Scheme { get; } = "https";
        }

        class DummyRestClient : IRestClient
        {
            public IReadOnlyCollection<IHttpRequestMiddleware> RequestMiddlewares { get; } =
                new List<IHttpRequestMiddleware>().AsReadOnly();

            public IHttpRequestSender RequestSender { get; } = new HttpRequestSender(DefaultHttpClient);

            public IWebApi WebApi { get; } = new DummyWebApi();
        }

        class Post
        {
            [JsonProperty("userId")]
            public int UserId { get; set; }

            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("body")]
            public string Body { get; set; }
        }

        [Fact]
        public async Task CanDoBasicGet()
        {
            var client = new DummyRestClient();

            var posts = await client.SendAsync(
                HttpMethod.Get,
                "/posts",
                message => { },
                async response => JsonConvert.DeserializeObject<IEnumerable<Post>>(await response.Content.ReadAsStringAsync())
            );

            Assert.NotEmpty(posts);
        }

        [Fact]
        public async Task CanDoBasicPost()
        {
            var client = new DummyRestClient();
            var localPost = new Post
            {
                UserId = 3,
                Title = "This is a Title",
                Body = "This is a body.",
            };

            var createdPost = await client.SendAsync(
                HttpMethod.Post,
                "/posts",
                message =>
                {
                    message.Content = JsonConvert.SerializeObject(localPost).AsJsonContent();
                },
                async response => JsonConvert.DeserializeObject<Post>(await response.Content.ReadAsStringAsync())
            );

            Assert.NotEqual(localPost.Id, createdPost.Id);
            Assert.Equal(localPost.UserId, createdPost.UserId);
            Assert.Equal(localPost.Title, createdPost.Title);
            Assert.Equal(localPost.Body, createdPost.Body);
        }

        [Fact]
        public async Task CanDoBasicPut()
        {
            var client = new DummyRestClient();
            var path = "/posts/1";

            var originalPost = await client.SendAsync(
                HttpMethod.Get,
                path,
                message => { },
                async response => JsonConvert.DeserializeObject<Post>(await response.Content.ReadAsStringAsync())
            );

            var modifiedPost = new Post
            {
                Id = originalPost.Id,
                UserId = 12345,
                Title = Guid.NewGuid().ToString(),
                Body = Guid.NewGuid().ToString(),
            };

            var updatedPost = await client.SendAsync(
                HttpMethod.Put,
                path,
                message =>
                {
                    message.Content = JsonConvert.SerializeObject(modifiedPost).AsJsonContent();
                },
                async response => JsonConvert.DeserializeObject<Post>(await response.Content.ReadAsStringAsync())
            );

            Assert.    Equal( originalPost.Id,     updatedPost.Id);
            Assert. NotEqual( originalPost.UserId, updatedPost.UserId);
            Assert. NotEqual( originalPost.Title,  updatedPost.Title);
            Assert. NotEqual( originalPost.Body,   updatedPost.Body);

            Assert. Equal( modifiedPost.Id,     updatedPost.Id);
            Assert. Equal( modifiedPost.UserId, updatedPost.UserId);
            Assert. Equal( modifiedPost.Title,  updatedPost.Title);
            Assert. Equal( modifiedPost.Body,   updatedPost.Body);
        }
    }
}

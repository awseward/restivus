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
        static HttpClient _DefaultHttpClient { get; } = new HttpClient();

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
                new List<IHttpRequestMiddleware>
                {
                    new HttpRequestMiddleware(request =>
                    {
                        request.Headers.Add("X-Wing-Fighter", "Red Lobster, standing by!");
                    }),
                };

            public IHttpRequestSender RequestSender { get; } = new HttpRequestSender(_DefaultHttpClient);

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

            var posts = await client.Get().SendAsync(
                "/posts",
                _Deserialize<IEnumerable<Post>>
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

            var createdPost = await client.Post(_AsJsonContent).SendAsync(
                "/posts",
                localPost,
                _Deserialize<Post>
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

            var originalPost = await client.Get().SendAsync(
                path,
                _Deserialize<Post>
            );

            var modifiedPost = new Post
            {
                Id = originalPost.Id,
                UserId = 12345,
                Title = Guid.NewGuid().ToString(),
                Body = Guid.NewGuid().ToString(),
            };

            var updatedPost = await client.Put(_AsJsonContent).SendAsync(
                path,
                modifiedPost,
                _Deserialize<Post>
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

        static HttpContent _AsJsonContent(object thing)
        {
            return JsonConvert.SerializeObject(thing).AsJsonContent();
        }

        static async Task<T> _Deserialize<T>(HttpResponseMessage message)
        {
            return JsonConvert.DeserializeObject<T>(await message.Content.ReadAsStringAsync());
        }
    }
}

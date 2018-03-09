# Restivus [![Build status](https://ci.appveyor.com/api/projects/status/3oewdeeeuveva2vh/branch/master?svg=true)](https://ci.appveyor.com/project/datNET/restivus/branch/master) [![NuGet version](https://badge.fury.io/nu/restivus.svg)](https://badge.fury.io/nu/restivus)

A simple, basic .NET REST library

### Installation

```
Install-Package Restivus
```

### Usage

Here's a quick and basic request to GitHub's API v3 using Restivus

```cs
// ...
using Newtonsoft.Json;
using Restivus;

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

    public IHttpRequestSender RequestSender { get; } =
        new HttpRequestSender(YourApp.HttpClientInstance);

    public IWebApi WebApi { get; } = new GitHubWebApi();

    public Task<GitHubUser> GetUsers()
    {
        return this.Get().SendAsync(
            "/users",
            _DeserializeJson<IEnumerable<GitHubUser>>
        );
    }

    private async Task<T> _DeserializeJson<T>(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<T>(
            await response.Content.ReadAsStringAsync()
        );
    }
}

[JsonObject]
class GitHubUser { /* ... */ }
```

### Publishing

Mostly a reminder for myself since I publish so infrequently:

```sh
git clean -xdf

# Build in Release config in VS

nuget pack -Properties Configuration=Release src/Restivus/Restivus.csproj

nuget push Restivus.<version>.nupkg -Source https://api.nuget.org/v3/index.json -ApiKey $NUGET_API_KEY
```

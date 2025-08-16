using NUnit.Framework;
using RestSharp;
using StorySpoil.ApiTests.Helpers;
using System.Text.Json;

namespace StorySpoil.ApiTests.Tests;

public class TestBase
{
    protected static RestClient? Client;
    protected static string? CreatedStoryId;

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        var userName = Environment.GetEnvironmentVariable("STORY_USER") ?? ApiConstants.DefaultUserName;
        var password = Environment.GetEnvironmentVariable("STORY_PASS") ?? ApiConstants.DefaultPassword;

        var options = new RestClientOptions(ApiConstants.BaseUrl);
        Client = new RestClient(options);

        var loginRequest = new RestRequest(ApiConstants.UserLogin, Method.Post)
            .AddJsonBody(new { userName, password });

        var response = Client.ExecuteAsync(loginRequest).GetAwaiter().GetResult();
        Assert.That(response.IsSuccessful, Is.True, "Login failed, check credentials!");

        using var doc = JsonDocument.Parse(response.Content!);
        var token = doc.RootElement.GetProperty("accessToken").GetString();
        Assert.That(string.IsNullOrWhiteSpace(token), Is.False, "AccessToken not returned!");

        Client.Authenticator = new RestSharp.Authenticators.JwtAuthenticator(token!);
    }
}

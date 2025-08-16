using NUnit.Framework;
using RestSharp;
using System.Net;
using System.Text.Json;
using StorySpoil.ApiTests.DTOs;
using StorySpoil.ApiTests.Helpers;

namespace StorySpoil.ApiTests.Tests;

[TestFixture]
public class StorySpoilApiTests : TestBase
{
    private static string NewTitle() => $"AutoTitle {Guid.NewGuid():N}".Substring(0, 20);
    private static string NewDesc() => $"AutoDesc {Guid.NewGuid():N}";

    [Test, Order(1)]
    public void CreateStory_Succeeds()
    {
        var payload = new StoryDto { Title = NewTitle(), Description = NewDesc(), Url = "" };
        var req = new RestRequest(ApiConstants.StoryCreate, Method.Post).AddJsonBody(payload);

        var resp = Client!.ExecuteAsync(req).GetAwaiter().GetResult();
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var data = JsonSerializer.Deserialize<ApiResponseDto>(resp.Content!);
        Assert.That(data, Is.Not.Null);
        Assert.That(data!.Msg, Is.EqualTo("Successfully created!"));
        Assert.That(string.IsNullOrWhiteSpace(data!.StoryId), Is.False);

        CreatedStoryId = data.StoryId;
    }

    [Test, Order(2)]
    public void EditStory_Succeeds()
    {
        var payload = new StoryDto { Title = NewTitle(), Description = NewDesc(), Url = "" };
        var req = new RestRequest($"{ApiConstants.StoryEdit}/{CreatedStoryId}", Method.Put).AddJsonBody(payload);

        var resp = Client!.ExecuteAsync(req).GetAwaiter().GetResult();
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var data = JsonSerializer.Deserialize<ApiResponseDto>(resp.Content!);
        Assert.That(data!.Msg, Is.EqualTo("Successfully edited"));
    }

    [Test, Order(3)]
    public void GetAllStories_Succeeds()
    {
        var req = new RestRequest(ApiConstants.StoryAll, Method.Get);
        var resp = Client!.ExecuteAsync(req).GetAwaiter().GetResult();
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(resp.Content, Does.Contain("["));
    }

    [Test, Order(4)]
    public void DeleteStory_Succeeds()
    {
        var req = new RestRequest($"{ApiConstants.StoryDelete}/{CreatedStoryId}", Method.Delete);
        var resp = Client!.ExecuteAsync(req).GetAwaiter().GetResult();
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var data = JsonSerializer.Deserialize<ApiResponseDto>(resp.Content!);
        Assert.That(data!.Msg, Is.EqualTo("Deleted successfully!"));
    }

    [Test, Order(5)]
    public void CreateStory_MissingRequiredFields_Fails()
    {
        var payload = new StoryDto { Title = "", Description = "" };
        var req = new RestRequest(ApiConstants.StoryCreate, Method.Post).AddJsonBody(payload);

        var resp = Client!.ExecuteAsync(req).GetAwaiter().GetResult();
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test, Order(6)]
    public void EditStory_NonExisting_Fails()
    {
        var payload = new StoryDto { Title = NewTitle(), Description = NewDesc() };
        var req = new RestRequest($"{ApiConstants.StoryEdit}/{Guid.NewGuid()}", Method.Put).AddJsonBody(payload);

        var resp = Client!.ExecuteAsync(req).GetAwaiter().GetResult();
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var data = JsonSerializer.Deserialize<ApiResponseDto>(resp.Content!);
        Assert.That(data!.Msg, Does.Contain("No spoilers"));
    }

    [Test, Order(7)]
    public void DeleteStory_NonExisting_Fails()
    {
        var req = new RestRequest($"{ApiConstants.StoryDelete}/{Guid.NewGuid()}", Method.Delete);
        var resp = Client!.ExecuteAsync(req).GetAwaiter().GetResult();
        Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

        var data = JsonSerializer.Deserialize<ApiResponseDto>(resp.Content!);
        Assert.That(data!.Msg, Is.EqualTo("Unable to delete this story spoiler!"));
    }
}

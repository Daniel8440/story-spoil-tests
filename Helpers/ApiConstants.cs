namespace StorySpoil.ApiTests.Helpers;

public static class ApiConstants
{
    public const string BaseUrl = "https://d3s5nxhwblsjbi.cloudfront.net/api";

    // Default credentials
    public const string DefaultUserName = "daniel1551";
    public const string DefaultPassword = "2100709";

    // Endpoints
    public const string UserLogin = "/User/Authentication";
    public const string StoryCreate = "/Story/Create";
    public const string StoryEdit = "/Story/Edit";
    public const string StoryDelete = "/Story/Delete";
    public const string StoryAll = "/Story/All";
}

using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;


namespace StorySpoilerExamBobi;

    [TestFixture]
public class StorySpoilerApiTests
{
    private RestClient client;
    private static string storyId;
    private const string baseURL = "https://d3s5nxhwblsjbi.cloudfront.net";
    [OneTimeSetUp]
    public void Setup()
    {
        string token = GetJwtToken("veev", "veev123");
        var options = new RestClientOptions(baseURL)
        {
            Authenticator = new JwtAuthenticator(token)
        };
        client = new RestClient(options);
    }
    private string GetJwtToken(string username, string password)
    {
        var loginClient = new RestClient(baseURL);
        var request = new RestRequest("/api/User/Authentication", Method.Post);
        request.AddJsonBody(new { username, password });
        var response = loginClient.Execute(request);
        var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
        return json.GetProperty("accessToken").GetString() ??string.Empty;

       
    }

    //tests
    [Test, Order(1)]
    public void CreateStory_ReturnsCreated()
    {
        var story = new
        {
            title = "New Story",
            description = "Test story description",
            url = ""
        };

        var request = new RestRequest("/api/Story/Create", Method.Post);
        request.AddJsonBody(story);

        var response = client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
        storyId = json.GetProperty("storyId").GetString();
        Assert.That(storyId, Is.Not.Null.And.Not.Empty);
        Assert.That(json.GetProperty("msg").GetString(), Is.EqualTo("Successfully created!"));

    }

    [Test, Order(2)]
    public void EditStory_ReturnsOk()
    {
        var editedStory = new
        {
            title = "Edited Story",
            description = "Edited story description",
            url = ""
        };
        var request = new RestRequest($"/api/Story/Edit/{storyId}", Method.Put);
        request.AddJsonBody(editedStory);
        var response = client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
        Assert.That(json.GetProperty("msg").GetString(), Is.EqualTo("Successfully edited"));
    }
    [Test, Order(3)]
    public void GetAllStories_ShouldReturnList()
    {
        var request = new RestRequest("/api/Story/All", Method.Get);
        var response = client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var foods = JsonSerializer.Deserialize<List<object>>(response.Content);
        Assert.That(foods, Is.Not.Empty);
    }

    [Test, Order(4)]
    public void DeleteStory_ReturnsOK()
    {
        var request = new RestRequest($"/api/Story/Delete/{storyId}", Method.Delete);
        var response = client.Execute(request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
        Assert.That(json.GetProperty("msg").GetString(), Is.EqualTo("Deleted successfully!"));
    }
    [Test, Order(5)]
    public void CreateStory_WithMissingData_ReturnsBadRequest()
    {
        var missingData = new
        {
            title = "",
            description = "This story has no title",
            url = ""
        };
        var request = new RestRequest("/api/Story/Create", Method.Post);
        request.AddJsonBody(missingData);
        var response = client.Execute(request);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    [Test, Order(6)]
    public void EditNonExistingStory_ReturnsNotFound()
    {
        var nonExistingStoryId = "non-existing-id";
        var editedStory = new
        {
            title = "Edited Story",
            description = "Edited story description",
            url = ""
        };
        var request = new RestRequest($"/api/Story/Edit/{nonExistingStoryId}", Method.Put);
        request.AddJsonBody(editedStory);
        var response = client.Execute(request);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
        Assert.That(json.GetProperty("msg").GetString(), Is.EqualTo("No spoilers..."));
    }
    [Test, Order(7)]
    public void DeleteNonExistingStory_ReturnsNotFound()
    {
        var nonExistingStoryId = "non-existing-id";
        var request = new RestRequest($"/api/Story/Delete/{nonExistingStoryId}", Method.Delete);
        var response = client.Execute(request);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
        Assert.That(json.GetProperty("msg").GetString(), Is.EqualTo("Unable to delete this story spoiler!"));
    }
    [OneTimeTearDown]
    public void CleanUp()
    {
       client?.Dispose();
    }
}

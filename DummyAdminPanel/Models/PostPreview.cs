using System.Text.Json.Serialization;

namespace DummyAdminPanel.Models;

public record PostPreview(string Id, string Image, int Likes, string[] Tags, [property: JsonPropertyName("text")] string Preview,
    [property: JsonPropertyName("publishDate")] DateTime PublishDateUtc);
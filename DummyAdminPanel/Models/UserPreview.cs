using System.Text.Json.Serialization;

namespace DummyAdminPanel.Models;

public record UserPreview(string Id, string Title, string FirstName, string LastName, [property: JsonPropertyName("picture")] string PictureUrl);
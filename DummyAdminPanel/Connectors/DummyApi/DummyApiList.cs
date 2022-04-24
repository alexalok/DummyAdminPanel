using System.Text.Json.Serialization;

namespace DummyAdminPanel.Connectors.DummyApi;

public record DummyApiList<T>(T[] Data, int Total, int Page, [property: JsonPropertyName("limit")] int PageSize);
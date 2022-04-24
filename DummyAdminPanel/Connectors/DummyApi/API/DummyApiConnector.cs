using System.Text.Json;
using DummyAdminPanel.Models;
using Microsoft.Extensions.Options;

namespace DummyAdminPanel.Connectors.DummyApi.API;

public class DummyApiConnector : IDummyApiConnector
{
    readonly ILogger<DummyApiConnector> _logger;
    readonly HttpClient _http;
    readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web);
    readonly int _pageSize;

    public DummyApiConnector(ILogger<DummyApiConnector> logger, HttpClient http, IOptions<DummyApiSettings> settings)
    {
        _logger = logger;
        _http = http;
        _pageSize = settings.Value.PageSize;
    }

    public async Task<DummyApiList<UserPreview>?> TryGetAllUsers(int page)
    {
        try
        {
            var resp = await _http.GetFromJsonAsync<DummyApiList<UserPreview>>($"user?page={page}&limit={_pageSize}", _serializerOptions);
            return resp;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception when trying to fetch all users from dummy api");
            return null;
        }
    }

    public async Task<DummyApiList<PostPreview>?> TryGetPostsOfUser(string userId, int page)
    {
        try
        {
            var resp = await _http.GetFromJsonAsync<DummyApiList<PostPreview>>($"user/{userId}/post?page={page}&limit={_pageSize}", _serializerOptions);
            return resp;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception when trying to fetch posts of user {UserId} from dummy api", userId);
            return null;
        }
    }
}
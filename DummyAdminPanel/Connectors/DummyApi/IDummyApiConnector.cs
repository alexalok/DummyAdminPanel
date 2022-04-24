using DummyAdminPanel.Models;

namespace DummyAdminPanel.Connectors.DummyApi;

public interface IDummyApiConnector
{
    Task<DummyApiList<UserPreview>?> TryGetAllUsers(int page);
    Task<DummyApiList<PostPreview>?> TryGetPostsOfUser(string userId, int page);
}
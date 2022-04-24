using DummyAdminPanel.Models;

namespace DummyAdminPanel.Connectors.DummyApi.LocalStorage;

public interface IDummyApiCacher
{
    Task CachePageOfUsers(DummyApiList<UserPreview> usersPage);
    Task CachePageOfPostsOfUser(string userId, DummyApiList<PostPreview> postsPage);
}
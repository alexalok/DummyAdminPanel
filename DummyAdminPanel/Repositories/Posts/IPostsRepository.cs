using DummyAdminPanel.Models;

namespace DummyAdminPanel.Repositories.Posts;

public interface IPostsRepository
{
    Task<PagedData<PostPreview>> GetPostsOfUserPaged(string userId, int page);
}
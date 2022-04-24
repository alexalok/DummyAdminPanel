#if DEBUG

using DummyAdminPanel.Models;

namespace DummyAdminPanel.Repositories.Posts;

public class DemoPostsRepository : IPostsRepository
{
    public Task<PagedData<PostPreview>> GetPostsOfUserPaged(string userId, int page)
    {
        IEnumerable<PostPreview> posts = new PostPreview[]
        {
            new("60d21b4667d0d8992e610c85", "https://img.dummyapi.io/photo-1564694202779-bc908c327862.jpg", 43,
                new[] {"animal", "dog", "golden retriever"}, "adult Labrador retriever", DateTime.Parse("2020-05-24T14:53:17.598Z")),

            new("60d21b4967d0d8992e610c90", "https://img.dummyapi.io/photo-1510414696678-2415ad8474aa.jpg", 31, new[] {"snow", "ice", "mountain"},
                "ice caves in the wild landscape photo of ice near ...", DateTime.Parse("2020-05-24T07:44:17.738Z"))
        };
        return Task.FromResult(new PagedData<PostPreview>(0, 1, posts));
    }
}

#endif
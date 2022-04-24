using DummyAdminPanel.Connectors.DummyApi;
using DummyAdminPanel.Connectors.DummyApi.LocalStorage;
using DummyAdminPanel.Exceptions;
using DummyAdminPanel.Models;

namespace DummyAdminPanel.Repositories.Posts;

public class CachingPostsRepository : IPostsRepository
{
    readonly IDummyApiConnector _connector;
    readonly ICachingDummyApiConnector _cachingConnector;
    readonly IDummyApiCacher _cacher;

    public CachingPostsRepository(IDummyApiConnector connector, ICachingDummyApiConnector cachingConnector, IDummyApiCacher cacher)
    {
        _connector = connector;
        _cachingConnector = cachingConnector;
        _cacher = cacher;
    }

    public async Task<PagedData<PostPreview>> GetPostsOfUserPaged(string userId, int page)
    {
        var posts = await _cachingConnector.TryGetPostsOfUser(userId, page);
        if (posts == null)
        {
            posts = await _connector.TryGetPostsOfUser(userId, page);
            if (posts == null)
                throw new FailedToFetchDataException();
            await _cacher.CachePageOfPostsOfUser(userId, posts);
        }

        var totalPages = (int) Math.Ceiling((float) posts.Total / posts.PageSize);
        PagedData<PostPreview> resp = new(page, totalPages, posts.Data);
        return resp;
    }
}
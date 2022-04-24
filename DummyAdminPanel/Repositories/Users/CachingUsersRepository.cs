using DummyAdminPanel.Connectors.DummyApi;
using DummyAdminPanel.Connectors.DummyApi.LocalStorage;
using DummyAdminPanel.Exceptions;
using DummyAdminPanel.Models;

namespace DummyAdminPanel.Repositories.Users;

public class CachingUsersRepository : IUsersRepository
{
    readonly IDummyApiConnector _connector;
    readonly ICachingDummyApiConnector _cachingConnector;
    readonly IDummyApiCacher _cacher;

    public CachingUsersRepository(IDummyApiConnector connector, ICachingDummyApiConnector cachingConnector, IDummyApiCacher cacher)
    {
        _connector = connector;
        _cachingConnector = cachingConnector;
        _cacher = cacher;
    }

    public async Task<PagedData<UserPreview>> GetAllUsersPaged(int page)
    {
        var users = await _cachingConnector.TryGetAllUsers(page);
        if (users == null)
        {
            users = await _connector.TryGetAllUsers(page);
            if (users == null)
                throw new FailedToFetchDataException();
            await _cacher.CachePageOfUsers(users);
        }
        var totalPages = (int) Math.Ceiling((float) users.Total / users.PageSize);
        PagedData<UserPreview> resp = new(page, totalPages, users.Data);
        return resp;
    }
}
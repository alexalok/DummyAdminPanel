using Blazored.LocalStorage;
using DummyAdminPanel.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.Extensions.Options;

namespace DummyAdminPanel.Connectors.DummyApi.LocalStorage;

public class DummyApiLocalStorageConnector : ICachingDummyApiConnector, IDummyApiCacher
{
    readonly ILocalStorageService _localStorage;
    readonly ISystemClock _clock;
    internal readonly TimeSpan CacheLifetime;

    public DummyApiLocalStorageConnector(IOptions<DummyApiSettings> settings, ILocalStorageService localStorage, ISystemClock clock)
    {
        _localStorage = localStorage;
        _clock = clock;
        CacheLifetime = TimeSpan.FromSeconds(settings.Value.CacheLifetimeSeconds);
    }

    public async Task<DummyApiList<UserPreview>?> TryGetAllUsers(int page)
    {
        var users = await TryGetItemAsync<DummyApiList<UserPreview>>(GetCacheKeyForUsersPage(page));
        return users;
    }

    public async Task<DummyApiList<PostPreview>?> TryGetPostsOfUser(string userId, int page)
    {
        var posts = await TryGetItemAsync<DummyApiList<PostPreview>>(GetCacheKeyForUserPostsPage(userId, page));
        return posts;
    }

    public async Task CachePageOfUsers(DummyApiList<UserPreview> usersPage)
    {
        await SetItemAsync<DummyApiList<UserPreview>>(GetCacheKeyForUsersPage(usersPage.Page), usersPage);
    }

    public async Task CachePageOfPostsOfUser(string userId, DummyApiList<PostPreview> postsPage)
    {
        await SetItemAsync<DummyApiList<PostPreview>>(GetCacheKeyForUserPostsPage(userId, postsPage.Page), postsPage);
    }

    async Task<TItem?> TryGetItemAsync<TItem>(string key)
    {
        if (await HasCacheExpired())
        {
            await _localStorage.ClearAsync();
            return default;
        }

        return await _localStorage.GetItemAsync<TItem>(key);
    }

    async Task SetItemAsync<TItem>(string key, TItem item)
    {
        bool hasCacheExpired = await HasCacheExpired();
        if (hasCacheExpired)
            await _localStorage.ClearAsync();

        await _localStorage.SetItemAsync<TItem>(key, item);

        if (hasCacheExpired)
            await SetCacheExpiry(_clock.UtcNow.UtcDateTime.Add(CacheLifetime));
    }

    async Task<DateTime?> GetCacheExpiry()
    {
        var expiry = await _localStorage.GetItemAsync<DateTime?>(CacheKeyForExpiry);
        return expiry;
    }

    async Task SetCacheExpiry(DateTime expiresAtUtc)
    {
        await _localStorage.SetItemAsync<DateTime>(CacheKeyForExpiry, expiresAtUtc);
    }

    async Task<bool> HasCacheExpired()
    {
        var expiry = await GetCacheExpiry();
        return expiry == null || _clock.UtcNow.UtcDateTime > expiry.Value;
    }

    static string GetCacheKeyForUsersPage(int page) => $"/users/{page}";
    static string GetCacheKeyForUserPostsPage(string userId, int page) => $"/user/{userId}/posts/{page}";

    internal const string CacheKeyForExpiry = "expires-at-utc";
}
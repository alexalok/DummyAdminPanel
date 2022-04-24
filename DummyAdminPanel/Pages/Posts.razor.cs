using System.Diagnostics;
using DummyAdminPanel.Exceptions;
using DummyAdminPanel.Models;
using DummyAdminPanel.Repositories;
using DummyAdminPanel.Repositories.Posts;
using Microsoft.AspNetCore.Components;

namespace DummyAdminPanel.Pages;

public partial class Posts
{
    [Parameter]
    [SupplyParameterFromQuery]
    public string? UserId { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "page")]
    public int CurrentPage { get; set; } = 1;

    [Inject]
    IPostsRepository PostsRepository { get; set; } = null!;

    protected override void OnParametersSet()
    {
        if (CurrentPage == 0)
            CurrentPage = 1;
    }

    async Task<PagedData<PostPreview>?> TryLoadPostsPaged(int page)
    {
        try
        {
            Debug.Assert(UserId != null); // We won't render table unless we have a UserId.
            var posts = await PostsRepository.GetPostsOfUserPaged(UserId, page - 1);
            return posts;
        }
        catch (FailedToFetchDataException)
        {
            return null;
        }
    }

    string UrlToPage(int page) => $"/posts?userId={UserId}&page={page}";
}
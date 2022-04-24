using DummyAdminPanel.Exceptions;
using DummyAdminPanel.Models;
using DummyAdminPanel.Repositories;
using DummyAdminPanel.Repositories.Users;
using Microsoft.AspNetCore.Components;

namespace DummyAdminPanel.Pages;

public partial class Users
{
    [Parameter]
    [SupplyParameterFromQuery(Name = "page")]
    public int CurrentPage { get; set; } = 1;

    [Inject]
    IUsersRepository UsersRepository { get; set; } = null!;

    protected override void OnParametersSet()
    {
        if (CurrentPage == 0)
            CurrentPage = 1;
    }

    async Task<PagedData<UserPreview>?> TryLoadUsersForPage(int page)
    {
        try
        {
            var users = await UsersRepository.GetAllUsersPaged(page - 1);
            return users;
        }
        catch (FailedToFetchDataException)
        {
            return null;
        }
    }

    static string UrlToPage(int page) => $"/users?page={page}";
}
#if DEBUG

using DummyAdminPanel.Models;

namespace DummyAdminPanel.Repositories.Users;

public class DemoUsersRepository : IUsersRepository
{
    public Task<PagedData<UserPreview>> GetAllUsersPaged(int page)
    {
        IEnumerable<UserPreview> users = new UserPreview[]
        {
            new( "60d0fe4f5311236168a109ca", "ms", "Sara",  "Andersen", "https://randomuser.me/api/portraits/women/58.jpg"),
            new("60d0fe4f5311236168a109cb", "miss", "Edita", "Vestering", "https://randomuser.me/api/portraits/med/women/89.jpg")
        };
        return Task.FromResult(new PagedData<UserPreview>(0, 1, users));
    }
}

#endif
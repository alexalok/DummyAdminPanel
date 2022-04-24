using DummyAdminPanel.Models;

namespace DummyAdminPanel.Repositories.Users;

public interface IUsersRepository
{
    Task<PagedData<UserPreview>> GetAllUsersPaged(int page);
}
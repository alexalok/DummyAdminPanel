namespace DummyAdminPanel.Repositories;

public record PagedData<TData>(int CurrentPage, int TotalPages, IEnumerable<TData> Data);
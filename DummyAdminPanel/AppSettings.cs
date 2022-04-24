namespace DummyAdminPanel;

public class AppSettings
{
    public string UserEmail { get; set; } = null!;
    public string UserPassword { get; set; } = null!;

    public DummyApiSettings DummyApi { get; set; } = null!;
}

public class DummyApiSettings
{
    public string BaseUrl { get; set; } = null!;
    public string AppId { get; set; } = null!;
    public int PageSize { get; set; }
    public int CacheLifetimeSeconds { get; set; }
}
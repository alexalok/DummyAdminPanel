namespace DummyAdminPanel.Exceptions;

public class FailedToFetchDataException : Exception
{
    public FailedToFetchDataException() : base("Failed to fetch required data.") { }
}
namespace Template.Data.Repositories.Shared;

public static class TableName
{
    public static string Of<T>() where T : class
    {
        return typeof(T).Name switch
        {
            _ => $"{typeof(T).Name.ToLower()}s",
        };
    }
}

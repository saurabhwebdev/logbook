namespace CoreEngine.Shared.Constants;

public static class RoleConstants
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string User = "User";

    public static IReadOnlyList<string> GetAll() =>
        new[] { SuperAdmin, Admin, User };
}

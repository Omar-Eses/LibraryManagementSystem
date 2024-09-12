namespace LibraryManagementSystem.Helpers;

public static class PermissionTypes
{
    public const string CanBorrow = "CanBorrow";
    public const string CanReturn = "CanReturn";
    public const string CanAddBook = "CanAddBook";
    public const string CanGetBook = "CanGetBook";
    public const string CanEditBook = "CanEditBook";
    public const string CanDeleteBook = "CanDeleteBook";
}

public enum enumPermissionTypes
{
    CanBorrow = 1,
    CanReturn = 2,
    CanAddBook =3,
    CanGetBook =4,
    CanEditBook =5,
    CanDeleteBook = 6,
}
namespace LibraryManagementSystem.Helpers;

public static class CommonVariables
{
    public static readonly int NumberOfBooksAllowed = 3;
    public static decimal FinePerDay = 0.5m;

    public static readonly int DaysToReturn = 30;

    // define a default permissions list 
    public static List<long> DefaultPermissions = new List<long>
    {
        1, 2, 4
    };
}
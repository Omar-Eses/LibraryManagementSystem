namespace LibraryManagementSystem.Domain.Helpers;

public static class CommonVariables
{
    public static readonly int NumberOfBooksAllowed = 3;
    public static decimal FinePerDay = 0.5m;

    public static readonly int DaysToReturn = 30;

    public static readonly TimeSpan CacheExpirationTime = TimeSpan.FromHours(2);

    // define a default permissions list 
    public static List<long> DefaultPermissions = new List<long>
    {
        1, 2, 4
    };
    public const string userQueue = "LMSUserQueue";
    public const string bookQueue = "LMSBookQueue";
    public const string bRecordQueue = "LMSBRecordQueue";
    public const string exchangeName = "amq.direct";
    public const string routingKey = "LibraryManagementSystemRoutingKey";
}
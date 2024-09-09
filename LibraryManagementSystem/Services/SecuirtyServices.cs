namespace LibraryManagementSystem.Services;

public class SecuirtyServices
{
    private bool ValidateUserCredentials(string username, string password)
    {
        // This is a hardcoded check. Replace this with a database check.
        return username == "testuser" && password == "password123";
    }
}

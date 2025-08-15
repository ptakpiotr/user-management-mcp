using Microsoft.AspNetCore.Authentication;

namespace UserManagementApp.Auth;

public class ApiKeySchemeOptions : AuthenticationSchemeOptions
{
    public string[] AllowedApiKeys { get; set; } = [];
}
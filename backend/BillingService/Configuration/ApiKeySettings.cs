namespace BillingService.Configuration;

public class ApiKeySettings
{
    public const string SectionName = "Authentication:ApiKey";
    public string HeaderName { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
}
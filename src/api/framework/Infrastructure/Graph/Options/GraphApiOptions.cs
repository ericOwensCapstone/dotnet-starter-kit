namespace FSH.Framework.Infrastructure.Graph.Options;

public class GraphApiOptions
{
    public const string SectionName = "GraphApi";
    
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string B2CExtensionAppClientId { get; set; } = string.Empty;
    public string B2CDomain { get; set; } = string.Empty;
    
    public string Authority => $"https://login.microsoftonline.com/{TenantId}";
    public string[] Scopes => new[] { "https://graph.microsoft.com/.default" };
}
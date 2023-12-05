namespace Survey.API.Helpers;

public static class StaticConfigurationManager
{
    public static IConfiguration AppSettings { get; }
    static StaticConfigurationManager()
    {
        AppSettings = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
    }
}

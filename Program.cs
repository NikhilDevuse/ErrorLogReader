using ErrorLogReader.Models;
using ErrorLogReader.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var fileLocations = GetFileLocationsFromConfig(configuration);
        var moduleMappings = GetModuleMappingsFromConfig(configuration);
        var serviceProvider = ConfigureServices(connectionString);
        var fileService = serviceProvider.GetRequiredService<FileService>();
        var databaseService = serviceProvider.GetRequiredService<DatabaseService>();
        var currentDate = DateTime.Now.ToString("yyyy-MM-dd");

        foreach (var location in fileLocations)
        {
            foreach (var fileNameFormat in location.Files)
            {
                string fileName = fileNameFormat.Replace("{0}", currentDate);
                var logEntries = fileService.ReadLogFiles(location.Path, new List<string> { fileName });
                foreach (var entry in logEntries)
                {
                    string moduleName = ExtractModuleName(fileName, moduleMappings);
                    var logEntryDto = new LogEntryDto
                    {
                        Text = entry.Text,
                        Timestamp = entry.Timestamp,
                        CreatedDate = DateTime.Parse(currentDate),
                        CreatedUser = "CreatedBy",
                        UpdatedDate = DateTime.Parse(currentDate),
                        UpdatedUser = "UpdatedBy",
                        ModuleName = moduleName
                    };
                    databaseService.InsertLogEntry(logEntryDto);
                }
            }
        }
    }

    private static IServiceProvider ConfigureServices(string connectionString)
    {
        var services = new ServiceCollection();
        services.AddSingleton(new DatabaseService(connectionString));
        services.AddScoped<FileService>();
        return services.BuildServiceProvider();
    }

    private static List<FileLocation> GetFileLocationsFromConfig(IConfiguration configuration)
    {
        var fileLocations = new List<FileLocation>();
        var locations = configuration.GetSection("FileLocations");

        if (locations != null)
        {
            fileLocations = locations.GetChildren().Select(x => new FileLocation
            {
                Path = x.GetValue<string>("Path"),
                Files = x.GetSection("Files").GetChildren().Select(f => f.Value).ToList()
            }).ToList();
        }

        return fileLocations;
    }

    private static string ExtractModuleName(string fileName, Dictionary<string, List<string>> moduleMappings)
    {
        foreach (var kvp in moduleMappings)
        {
            string moduleName = kvp.Key;
            List<string> patterns = kvp.Value;

            foreach (var pattern in patterns)
            {
                if (fileName.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    return moduleName;
                }
            }
        }
        return "Other";
    }


    private static Dictionary<string, List<string>> GetModuleMappingsFromConfig(IConfiguration configuration)
    {
        var moduleMappings = new Dictionary<string, List<string>>();
        var mappings = configuration.GetSection("ModuleMappings");

        if (mappings != null)
        {
            foreach (var moduleMapping in mappings.GetChildren())
            {
                var moduleName = moduleMapping.Key;
                var patterns = moduleMapping.GetChildren().Select(p => p.Value).ToList();
                moduleMappings.Add(moduleName, patterns);
            }
        }

        return moduleMappings;
    }

}

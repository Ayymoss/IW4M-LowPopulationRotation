using System.Timers;
using Microsoft.Extensions.Logging;
using SharedLibraryCore;
using SharedLibraryCore.Interfaces;
using Timer = System.Timers.Timer;

namespace IW4MLowPopRotation;

public class Plugin : IPlugin
{
    public Plugin(IConfigurationHandlerFactory configurationHandlerFactory, ILogger<Plugin> logger)
    {
        _logger = logger;
        ConfigurationHandler =
            configurationHandlerFactory.GetConfigurationHandler<ServerConfiguration>(
                "LowPopulationSettings");
    }

    public string Name => "Low Population Rotation";
    public float Version => 20220901f;
    public string Author => "Amos";

    private readonly ILogger<Plugin> _logger;
    private readonly IConfigurationHandler<ServerConfiguration> ConfigurationHandler;
    public const int ConfigurationVersion = 2;
    private static List<string> ServersWithRotation;
    private static string RotateToMap;
    private static IManager Manager;

    public Task OnEventAsync(GameEvent gameEvent, Server server)
    {
        return Task.CompletedTask;
    }

    public async Task OnLoadAsync(IManager manager)
    {
        await ConfigurationHandler.BuildAsync();
        if (ConfigurationHandler.Configuration() == null)
        {
            Console.WriteLine($"[{Name}] Configuration not found, creating.");
            ConfigurationHandler.Set(new ServerConfiguration());
            await ConfigurationHandler.Save();
        }

        if (ConfigurationHandler.Configuration().Version < ConfigurationVersion)
        {
            Console.WriteLine($"[{Name}] Configuration version is outdated, updating.");
            ConfigurationHandler.Configuration().Version = ConfigurationVersion;
            await ConfigurationHandler.Save();
        }

        Manager = manager;
        ServersWithRotation = ConfigurationHandler.Configuration().ServersWithRotation;
        RotateToMap = ConfigurationHandler.Configuration().RotateToMap;

        var timer = new Timer();
        timer.Interval = ConfigurationHandler.Configuration().CheckInMilliseconds;
        timer.Elapsed += TimerTrigger;
        timer.AutoReset = true;
        if (ServersWithRotation.Any()) timer.Enabled = true;
    }
    
    private void TimerTrigger(object? source, ElapsedEventArgs e)
    {
        foreach (var server in Manager.GetServers())
        {
            _logger.LogDebug($"[{Name}] Checking server {server.IP}:{server.Port} for enabled rotation");
            if (!ServersWithRotation.Contains($"{server.IP}:{server.Port}")) return;
            
            _logger.LogDebug($"[{Name}] Checking server {server.IP}:{server.Port} for different map");
            if (server.CurrentMap.Name == RotateToMap) return;
            
            _logger.LogDebug($"[{Name}] Checking server {server.IP}:{server.Port} for low population");
            if (server.ClientNum <= 1) server.LoadMap(RotateToMap);
            
            _logger.LogInformation($"[{Name}] Rotating {server.IP}:{server.Port} to {RotateToMap}");
        }
    }

    public Task OnUnloadAsync()
    {
        return Task.CompletedTask;
    }

    public Task OnTickAsync(Server server)
    {
        return Task.CompletedTask;
    }
}

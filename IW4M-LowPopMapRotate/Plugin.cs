using System.Timers;
using Microsoft.Extensions.Logging;
using Serilog.Context;
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

    private async void TimerTrigger(object? source, ElapsedEventArgs e)
    {
        foreach (var server in Manager.GetServers())
        {
            using (LogContext.PushProperty("Server", server.ToString()))
            {
                _logger.LogDebug("[{Name}] Running through logic check", Name);

                if (!ServersWithRotation.Contains($"{server.IP}:{server.Port}"))
                {
                    _logger.LogDebug("[{Name}] Not in rotation list, skipping", Name);
                    continue;
                }

                if (server.CurrentMap.Name == RotateToMap)
                {
                    _logger.LogDebug("[{Name}] Already on rotation map, skipping", Name);
                    continue;
                }

                if (server.ClientNum > 1) continue;

                _logger.LogInformation("[{Name}] Rotating to {RotateToMap}", Name, RotateToMap);
                await server.LoadMap(RotateToMap);
            }
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

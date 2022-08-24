using SharedLibraryCore;
using SharedLibraryCore.Interfaces;
using Timer = System.Timers.Timer;

namespace IW4MLowPopRotation;

public class Plugin : IPlugin
{
    public Plugin(IConfigurationHandlerFactory configurationHandlerFactory)
    {
        ConfigurationHandler =
            configurationHandlerFactory.GetConfigurationHandler<ServerConfiguration>(
                "LowPopulationSettings");
    }

    public string Name => "Low Population Rotation";
    public float Version => 20220823f;
    public string Author => "Amos";
    
    private readonly ServerManager _serverManager = new();
    private readonly IConfigurationHandler<ServerConfiguration> ConfigurationHandler;
    public const int ConfigurationVersion = 1;
    public static List<long> ServersWithRotation;
    
    public Task OnEventAsync(GameEvent gameEvent, Server server)
    {
        _serverManager.EventUpdate(server);
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

        ServersWithRotation = ConfigurationHandler.Configuration().ServersWithRotation;
        if (!ServersWithRotation.Any()) await OnUnloadAsync();

        var timer = new Timer();
        timer.Interval = ConfigurationHandler.Configuration().CheckInMilliseconds;
        timer.Elapsed += _serverManager.TimerTrigger;
        timer.AutoReset = true;
        timer.Enabled = true;
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

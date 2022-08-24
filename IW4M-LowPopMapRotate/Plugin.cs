using SharedLibraryCore;
using SharedLibraryCore.Interfaces;
using Timer = System.Timers.Timer;

namespace IW4M_LowPopMapRotate;

public class Plugin : IPlugin
{
    public string Name => "IW4M Low Pop Map Rotate";
    public float Version => 20220803;
    public string Author => "Amos";
    
    private readonly ServerManager _serverManager = new();
    
    public Task OnEventAsync(GameEvent gameEvent, Server server)
    {
        _serverManager.EventUpdate(server);

        return Task.CompletedTask;
    }

    public Task OnLoadAsync(IManager manager)
    {
        var timer = new Timer();
        timer.Interval = 60_000;
        timer.Elapsed += _serverManager.TimerTrigger;
        timer.AutoReset = true;
        timer.Enabled = true;

        Console.WriteLine($"{Name} v{Version} by {Author} loaded");
        return Task.CompletedTask;
    }

    public Task OnUnloadAsync()
    {
        Console.WriteLine($"{Name} unloaded");
        return Task.CompletedTask;
    }

    public Task OnTickAsync(Server server)
    {
        return Task.CompletedTask;
    }
}

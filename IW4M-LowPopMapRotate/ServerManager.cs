using System.Timers;
using SharedLibraryCore;

namespace IW4MLowPopRotation;

public class ServerManager
{
    private Dictionary<Server, ServerInfo> Servers { get; } = new();

    public void EventUpdate(Server server)
    {
        lock (Servers)
        {
            if (!Servers.ContainsKey(server))
            {
                Servers.Add(server, new ServerInfo
                {
                    CurrentMapName = server.CurrentMap.Name,
                    ClientNum = server.ClientNum
                });
            }
            else
            {
                Servers[server].CurrentMapName = server.CurrentMap.Name;
                Servers[server].ClientNum = server.ClientNum;
            }
        }
    }

    public void TimerTrigger(object? source, ElapsedEventArgs e)
    {
        foreach (var server in Servers.Keys)
        {
            if (!Plugin.ServersWithRotation.Contains($"{server.IP}:{server.Port}")) return;
            if (Servers[server].CurrentMapName == "mp_rust") return;
            if (Servers[server].ClientNum <= 1) server.LoadMap("mp_rust");
        }
    }
}

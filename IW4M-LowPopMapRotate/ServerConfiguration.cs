using SharedLibraryCore.Interfaces;

namespace IW4MLowPopRotation;

public class ServerConfiguration : IBaseConfiguration
{
    public List<string> ServersWithRotation { get; set; } = new() {"127.0.0.1:12345", "192.168.1.2:54321", "192.168.1.2:11111"};
    public int CheckInMilliseconds { get; set; } = 60_000;
    public string RotateToMap { get; set; } = "mp_rust";
    public int Version { get; set; } = Plugin.ConfigurationVersion;

    public string Name() => "IncludedServers";
    public IBaseConfiguration Generate() => new ServerConfiguration();
}

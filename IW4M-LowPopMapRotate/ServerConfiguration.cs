using SharedLibraryCore.Interfaces;

namespace IW4MLowPopRotation;

public class ServerConfiguration : IBaseConfiguration
{
    public List<long> ServersWithRotation { get; set; } = new() {1111111111, 2222222222, 3333333333};

    public int CheckInMilliseconds { get; set; } = 60_000;
    public int Version { get; set; } = Plugin.ConfigurationVersion;

    public string Name() => "IncludedServers";
    public IBaseConfiguration Generate() => new ServerConfiguration();
}

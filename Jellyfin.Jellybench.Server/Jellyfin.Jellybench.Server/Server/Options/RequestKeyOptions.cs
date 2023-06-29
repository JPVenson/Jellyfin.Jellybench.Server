using ServiceLocator.Discovery.Option;

namespace Jellyfin.Jellybench.Server.Server.Options;

[FromConfig("RequestKey")]
public class RequestKeyOptions
{
    public string Secret { get; set; }
}
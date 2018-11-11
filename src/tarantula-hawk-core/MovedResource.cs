using System.Collections.Generic;

namespace TarantulaHawkCore
{
  public class MovedResource : IDownloadedResource
  {
    private readonly RemoteResource _remoteResource;
    private readonly string _newUri;

    public MovedResource(RemoteResource remoteResource, string newUri )
    {
      _remoteResource = remoteResource;
      _newUri = newUri;
    }

    public IEnumerable<string> ExtractLinks()
    {
      return new List<string>() { _newUri };
    }

    public void Save()
    {
    }
  }
}
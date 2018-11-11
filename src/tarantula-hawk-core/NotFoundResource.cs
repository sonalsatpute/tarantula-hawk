using System.Collections.Generic;

namespace TarantulaHawkCore
{
  public class NotFoundResource : IDownloadedResource
  {
    private RemoteResource _remoteResource;

    public NotFoundResource(RemoteResource remoteResource)
    {
      _remoteResource = remoteResource;
    }

    public IEnumerable<string> ExtractLinks()
    {
      return new List<string>();
    }

    public void Save()
    {
    }
  }
}
using System.Collections.Generic;

namespace TarantulaHawkCore
{
  public interface IDownloadedResource
  {
    IEnumerable<string> ExtractLinks();
    void Save();
  }
}
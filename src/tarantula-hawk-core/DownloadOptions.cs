using System;
using System.IO;

namespace TarantulaHawkCore
{
  public class DownloadOptions
  {
    public Uri BaseUrl { get; private set; }
    public string BaseUrlString { get; private set; }
    public string DestinationDir { get; }

    public DownloadOptions(string downloadUri, string destinationDir)
    {
      if (string.IsNullOrWhiteSpace(downloadUri) || string.IsNullOrWhiteSpace(destinationDir))
        throw new Exception("invalid parameters");

      InitializeBaseUrl(downloadUri);

      CreateDestinationDirIfNotExits(destinationDir);

      DestinationDir = destinationDir;
    }

    private static void CreateDestinationDirIfNotExits(string destinationDir)
    {
      if (!Directory.Exists(destinationDir)) Directory.CreateDirectory(destinationDir);
    }

    private void InitializeBaseUrl(string uriString)
    {
      Uri uri = new Uri(uriString);
      BaseUrlString = $"{uri.Scheme}://{uri.Authority}";
      BaseUrl = new Uri(BaseUrlString);
    }
  }
}

using System;
using System.IO;
using System.Linq;

namespace TarantulaHawkCore
{
  public class RemoteResource : IEquatable<RemoteResource>
  {
    private readonly char[] WWW = "www.".ToCharArray();

    private readonly string HTTP = "HTTP";
    private readonly string HTTPS = "HTTPS";

    private readonly DownloadOptions _options;
    private readonly Uri _url;

    public Uri Url => _url;

    public RemoteResource(DownloadOptions options, string url)
    {
      _options = options;
      string urlWithOutFragment = url.Split("?")[0].Split('#')[0].TrimEnd('/');

      _url = new Uri(urlWithOutFragment, UriKind.RelativeOrAbsolute);
      if (!Uri.TryCreate(urlWithOutFragment, UriKind.Absolute, out _url))
        _url = new Uri(new Uri(_options.BaseUrlString), urlWithOutFragment);
    }

    public bool IsAtBaseSite => string.Compare(_options.BaseUrl.Host.TrimStart(WWW), _url.Host.TrimStart(WWW), StringComparison.OrdinalIgnoreCase) == 0;

    public bool IsHttpResource => IsHttpProtocal();

    private bool IsHttpProtocal()
    {
      
      return string.Compare(_url.Scheme, HTTP, StringComparison.OrdinalIgnoreCase) == 0 ||
             string.Compare(_url.Scheme, HTTPS, StringComparison.OrdinalIgnoreCase) == 0;
    }

    public FileInfo ConvertToFileInfo()
    {
      FileInfo fileInfo = new FileInfo(Path.Combine(BuildLocalDirectoryTree(), ExtractResourceName()));
      if (string.IsNullOrWhiteSpace(fileInfo.Extension))
        fileInfo = new FileInfo(fileInfo.FullName+".html"); //TODO: hack:( just because of /meet route

      return fileInfo;
    }

    private string BuildLocalDirectoryTree()
    {
      string[] directories = _url.Segments.SkipLast(1).Select(path => path.Trim(new[] { '/' })).ToArray();
      return Path.Combine(_options.DestinationDir, Path.Combine(directories));
    }

    private string ExtractResourceName()
    {
      string name = _url.Segments.Last();
      return name.Equals("/") ? "index" : name;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;

      return Equals((RemoteResource) obj);
    }

    public bool Equals(RemoteResource other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;


      string otherLink = TrimHttpOrHttps(other.Url.OriginalString);
      string thisLink = TrimHttpOrHttps(Url.OriginalString);

      return Equals(otherLink, thisLink);
    }

    private string TrimHttpOrHttps(string uri)
    {
      const string HTTP = "http://";
      const string HTTPS = "https://";

      if (uri.StartsWith(HTTPS, StringComparison.OrdinalIgnoreCase))
        return uri.TrimStart(HTTPS.ToCharArray());

      if (uri.StartsWith(HTTP, StringComparison.OrdinalIgnoreCase))
        return uri.TrimStart(HTTPS.ToCharArray());

      return uri;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((_options != null ? _options.GetHashCode() : 0) * 397) ^ (_url != null ? _url.GetHashCode() : 0);
      }
    }
  }
}
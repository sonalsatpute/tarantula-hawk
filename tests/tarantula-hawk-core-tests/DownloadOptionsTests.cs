using TarantulaHawkCore;
using Xunit;

namespace TarantulaHawkCoreTests
{
  public class DownloadOptionsTests
  {
    [Theory]
    [InlineData("http://redgreenclean.com/", "http://redgreenclean.com")]
    [InlineData("http://github.com", "http://github.com")]
    [InlineData("https://github.com", "https://github.com")]
    [InlineData("https://github.com/", "https://github.com")]
    [InlineData("https://github.com/sonalsatpute", "https://github.com")]
    [InlineData("https://github.com/sonalsatpute/tarantula-hawk", "https://github.com")]
    [InlineData("https://github.com/sonalsatpute?tab=repositories", "https://github.com")]
    [InlineData("https://github.com?tab=repositories", "https://github.com")]
    [InlineData("https://github.com#repositories", "https://github.com")]
    public void extract_base_url(string url, string expected)
    {
      DownloadOptions options = new DownloadOptions(url, @"d:\");

      Assert.Equal(expected, options.BaseUrlString);
    }
  }
}

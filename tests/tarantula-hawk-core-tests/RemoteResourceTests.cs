using System.IO;
using TarantulaHawkCore;
using Xunit;

namespace TarantulaHawkCoreTests
{
  public class RemoteResourceTests
  {
    private readonly string BASE_URL = "http://redgreenclean.com";
    private readonly string BASE_DESTINATION_DIR = @"d:\temp";
    private readonly DownloadOptions _options;

    public RemoteResourceTests()
    {
      _options = new DownloadOptions(BASE_URL, BASE_DESTINATION_DIR);
    }

    [Theory]
    [InlineData("http://redgreenclean.com", "index.html")]
    [InlineData("http://redgreenclean.com/", "index.html")]
    [InlineData("http://redgreenclean.com/assets/css/main.css", "main.css")]
    [InlineData("http://redgreenclean.com/test.html?q=1&x=3","test.html")]
    [InlineData("http://redgreenclean.com/test.html?q=1&x=3#aidjsf","test.html")]
    [InlineData("http://redgreenclean.com#intro", "index.html")]
    [InlineData("http://redgreenclean.com/a/b/c/d","d.html")]
    [InlineData("http://redgreenclean.com/a/b/c/d/e/", "e.html")]
    [InlineData("test", "test.html")]
    [InlineData("test.html", "test.html")]
    [InlineData("/test.html", "test.html")]
    [InlineData("/test.html?q=1", "test.html")]
    [InlineData("/test.html?q=1&x=3", "test.html")]
    [InlineData("test.html?q=1&x=3", "test.html")]
    [InlineData("/../assets/i/sonal-satpute.jpg", "sonal-satpute.jpg")]
    public void remote_resource_should_have_name(string uri, string resourceName)
    {
      //RemoteResource information = new RemoteResource(_options, uri);
      RemoteResource resource = new RemoteResource(_options, uri);
      FileInfo fileInfo = resource.ConvertToFileInfo();

      Assert.Equal(resourceName, fileInfo.Name);
    }

    [Theory]
    [InlineData("http://redgreenclean.com", @"d:\temp")]
    [InlineData("http://redgreenclean.com/assets/css/main.css", @"d:\temp\assets\css")]
    [InlineData("http://redgreenclean.com/a/b/c/d/e/f/g", @"d:\temp\a\b\c\d\e\f")]
    [InlineData("http://redgreenclean.com/a/b/c/d/e/f/g/some-file.json", @"d:\temp\a\b\c\d\e\f\g")]
    [InlineData("/../assets/i/adrian-sofinet.jpg", @"d:\temp\assets\i")]
    public void remote_resource_structure_should_convert_to_local_file_info(string uri, string directoryName)
    {
      RemoteResource resource = new RemoteResource(_options, uri);
      FileInfo fileInfo = resource.ConvertToFileInfo();

      Assert.Equal(directoryName, fileInfo.DirectoryName);
    }

    [Theory]
    [InlineData("http://redgreenclean.com", true)]
    [InlineData("http://www.redgreenclean.com", true)]
    [InlineData("http://www.cleancode.com", false)]
    public void link_is_on_base_site(string uri, bool expected)
    {
      RemoteResource resource = new RemoteResource(_options, uri);
      Assert.Equal(resource.IsAtBaseSite, expected);
    }

    [Theory]
    [InlineData("http://redgreenclean.com#about", "http://redgreenclean.com")]
    [InlineData("http://redgreenclean.com?about", "http://redgreenclean.com")]
    public void link_should_not_have_fragment(string uri, string expected)
    {
      RemoteResource resource = new RemoteResource(_options, uri);
      Assert.Equal(resource.Url.OriginalString, expected);
    }

    [Theory]
    [InlineData("http://redgreenclean.com", "http://redgreenclean.com", true)]
    [InlineData("http://redgreenclean.com", "https://redgreenclean.com", true)]
    [InlineData("https://redgreenclean.com", "http://redgreenclean.com", true)]
    [InlineData("http://redgreenclean.com", "https://google.com", false)]
    public void equals_should_ignore_http_and_https(string uri1, string uri2, bool equals)
    {
      RemoteResource resource1 = new RemoteResource(_options, uri1);
      RemoteResource resource2 = new RemoteResource(_options, uri2);

      Assert.Equal(equals, resource1.Equals(resource2));
    }
  }
}
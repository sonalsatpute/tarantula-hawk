using tarantula_hawk.core;

namespace tarantula_hawk.domain.services;

internal class WebSiteDownloader : IDownloader 
{
    private readonly string _uri;
    public WebSiteDownloader(string uri) 
    {
        _uri = uri;
    }
    
    public Task<IEnumerable<Resource>> Start() => throw new NotImplementedException();
}
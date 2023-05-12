namespace tarantula_hawk.core;

public interface IDownloader
{
    Task<IEnumerable<Resource>> Start();
}

public interface IResourceDownloader : IDownloader { }

public enum ResourceType
{
    HTML,
    IMAGE,
    CSS,
    JAVASCRIPT,
    UNKNOWN
}

public record Resource(string Href, bool IsMissing, bool IsExternal, TimeSpan DownloadTime, ResourceType Type);

public interface IResourceExtractor
{
    Task<IEnumerable<Resource>> Extract(string content);
}
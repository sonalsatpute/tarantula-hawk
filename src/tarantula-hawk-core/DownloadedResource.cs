using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TarantulaHawkCore
{
  public class DownloadedResource : IDownloadedResource
  {
    private const string TEXT_HTML = "text/html";

    private readonly string _charSet;
    private readonly ResourceType _resourceType;
    private readonly byte[] _content;
    private string _textContent;

    private readonly RemoteResource _remoteResource;
    readonly LinkExtractor _extractor = new LinkExtractor();

    public DownloadedResource(RemoteResource remoteResource, string mediaType, string charSet, byte[] content)
    {
      _remoteResource = remoteResource;
      _charSet = charSet;
      _resourceType = mediaType.Equals(TEXT_HTML) ? ResourceType.Content : ResourceType.Resource;
      _content = content;
      _textContent = string.Empty;
    }

    private void SaveText(FileStream stream)
    {
      Encoding encoding = GetEncoding();

      _textContent = encoding.GetString(_content);

      using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
      {
        writer.Write(_textContent);
      }
    }

    public void Save()
    {
      try
      {
        FileInfo fileInfo = _remoteResource.ConvertToFileInfo();
        if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();

        using (FileStream stream = fileInfo.OpenWrite())
        {
          if (_resourceType == ResourceType.Content)
          {
            SaveText(stream);
          }
          else
          {
            stream.Write(_content, 0, _content.Length);
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
    }

    public IEnumerable<string> ExtractLinks()
    {
      return ResourceType.Resource == _resourceType ? new List<string>() : _extractor.GetLinks(_textContent);
    }

    private Encoding GetEncoding()
    {
        try
        {
          return Encoding.GetEncoding(_charSet);
        }
        catch (NotSupportedException)
        {
          //TODO: 
        }

      return Encoding.Default;
    }
  }
}
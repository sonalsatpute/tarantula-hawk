using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TarantulaHawkCore
{
  public class ResourceDownloader
  {
    private readonly IProgressBar _progressBar;
    private readonly HttpClient _httpClient;
    private long _activeDownloadCount;

    public ResourceDownloader(IProgressBar progressBar)
    {
      HttpClientHandler _httpClientHandler = new HttpClientHandler() { AllowAutoRedirect = false };
     _httpClient = new HttpClient(_httpClientHandler) { Timeout = TimeSpan.FromDays(1) };
      _progressBar = progressBar;
    }

    public IDownloadedResource Download(RemoteResource remoteResource)
    {
      IProgressBar progressBar = _progressBar.CreateChildDownloadProgress(remoteResource.Url.OriginalString);
      try
      {
        Interlocked.Increment(ref _activeDownloadCount);

        return DownloadResource(remoteResource, progressBar).Result;
      }
      catch (Exception ex)
      {
        if (ex is SocketException || ex is HttpRequestException)
          NoInternetConnection();

        throw;
      }
      finally
      {
        progressBar.Close();
        Interlocked.Decrement(ref _activeDownloadCount);
      }
    }

    private async Task<IDownloadedResource> DownloadResource(RemoteResource remoteResource, IProgressBar progressBar)
    {
      byte[] content;
      string mediaType;
      string charSet;

      using (HttpResponseMessage response = await _httpClient.GetAsync(remoteResource.Url, HttpCompletionOption.ResponseHeadersRead))
      {
        if (Is404(remoteResource, response))
          return new NotFoundResource(remoteResource); 

        if (IsMoved(response))
          return new MovedResource(remoteResource, response.Headers.Location.OriginalString);

        content = await ReadContent(response, progressBar);
        mediaType = response.Content.Headers.ContentType.MediaType;
        charSet = response.Content.Headers.ContentType.CharSet;
      }
      
      return new DownloadedResource(remoteResource, mediaType, charSet, content);
    }

    private bool IsMoved(HttpResponseMessage response)
    {
      return response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.MovedPermanently;
    }

    private void NoInternetConnection()
    {
      Console.WriteLine("no internet connection; please try again later.");

      Console.ReadLine();
      Environment.Exit(1);
    }

    private async Task<byte[]> ReadContent(HttpResponseMessage response, IProgressBar progressBar)
    {
      using (Stream stream = await response.Content.ReadAsStreamAsync())
      {
        using (MemoryStream mem = new MemoryStream())
        {
          int blockSize = 1024;
          byte[] blockBuffer = new byte[blockSize];
          int read;

          while ((read = stream.Read(blockBuffer, 0, blockSize)) > 0)
          {
            mem.Write(blockBuffer, 0, read);
            progressBar.ReportProgress(response.Content.Headers.ContentLength ?? 1024, read);
          }

          mem.Seek(0, SeekOrigin.Begin);
          return mem.GetBuffer();
        }
      }
    }

    private bool Is404(RemoteResource remoteResource, HttpResponseMessage response)
    {
      return !IsRequestAndResponseUrlSame(remoteResource, response) && response.StatusCode == HttpStatusCode.Found;
    }

    private bool IsRequestAndResponseUrlSame(RemoteResource remoteResource, HttpResponseMessage response)
    {
      return remoteResource.Url.Equals(response.Headers.Location);
    }

    public bool IsBusy => (Interlocked.Read(ref _activeDownloadCount) > 0);
  }
}

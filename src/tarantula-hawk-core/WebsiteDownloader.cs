using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TarantulaHawkCore
{
  public class WebsiteDownloader
  {
    private readonly SemaphoreSlim _semaphore;
    private readonly DownloadOptions _options;
    private readonly ResourceDownloader _downloader;
    private readonly IProgressBar _progressBar;

    private readonly BlockingCollection<RemoteResource> _uniqueResources = new BlockingCollection<RemoteResource>();
    private readonly BlockingCollection<RemoteResource> _downloadableResources = new BlockingCollection<RemoteResource>();
    private readonly BlockingCollection<RemoteResource> _downloadedResources = new BlockingCollection<RemoteResource>();
    
    public WebsiteDownloader(DownloadOptions options)
    {
      _options = options;
      _progressBar = new DownloadProgressBar(_options.BaseUrlString);
      _downloader = new ResourceDownloader(_progressBar);
      
      int maxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0));
      _semaphore = new SemaphoreSlim(maxDegreeOfParallelism);
    }

    public void Start()
    {
      RemoteResource root = new RemoteResource(_options, _options.BaseUrlString);
      _downloadableResources.Add(root);

      Download();

      while (IsDownloadInProgress())
      { }
      
      _progressBar.Close();
    }
    private async void Download()
    {
      foreach (RemoteResource remoteResource in _downloadableResources.GetConsumingEnumerable())
      {
        await _semaphore.WaitAsync();

        Task task = new TaskFactory().StartNew( (link) => ProcessLink((RemoteResource)link) , remoteResource);
      }
    }

    private void ProcessLink(RemoteResource link)
    {
      IDownloadedResource resource = _downloader.Download(link);
      resource.Save();

      _downloadedResources.Add(link);

      ExtractLinks(resource);
      UpdateProgressBar();

      _semaphore.Release();

      if (IsDownloadInProgress()) return;

      _downloadableResources.CompleteAdding();
    }

    private void UpdateProgressBar()
    {
      _progressBar.ReportProgress(_uniqueResources.Count, 1);
    }

    private void ExtractLinks(IDownloadedResource resource)
    {
      foreach (string url in resource.ExtractLinks())
      {
        RemoteResource remoteResource = new RemoteResource(_options, url);

        if (!IsDownloadableResource(remoteResource)) continue;

        _uniqueResources.Add(remoteResource);
        _downloadableResources.Add(remoteResource);
      }
    }

    private bool IsDownloadInProgress()
    {
      return (_downloader.IsBusy || _downloadableResources.Count > 0);
    }

    private bool IsDownloadableResource(RemoteResource remoteResource)
    {
      return remoteResource.IsHttpResource && remoteResource.IsAtBaseSite && !_uniqueResources.Contains(remoteResource);
    }
  }
}

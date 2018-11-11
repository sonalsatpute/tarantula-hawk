using System;
using ShellProgressBar;

namespace TarantulaHawkCore
{
  public class ChildDownloadProgressBar : IProgressBar
  {
    private readonly ChildProgressBar _downloadProgress;
    private const int MAX_TICKS = 100;

    private long _complitedCount;

    private ChildDownloadProgressBar(string url)
    {
      _complitedCount = 0L;

      var stepBarOptions = new ProgressBarOptions
      {
        ForegroundColor = ConsoleColor.Blue,
        BackgroundColor = ConsoleColor.DarkGray,
        ProgressCharacter = '─',

        CollapseWhenFinished = true,
      };

      _downloadProgress = _downloadProgress.Spawn(MAX_TICKS, url, stepBarOptions);
    }

    public ChildDownloadProgressBar(ProgressBar progressBar, string url)
    {
      _complitedCount = 0L;

      var stepBarOptions = new ProgressBarOptions
      {
        ForegroundColor = ConsoleColor.Green,
        BackgroundColor = ConsoleColor.DarkGray,
        ProgressCharacter = '─',

        CollapseWhenFinished = true,
      };

      _downloadProgress = progressBar.Spawn(MAX_TICKS, url, stepBarOptions);
    }

    public void ReportProgress(long totalCount, int complitedCount)
    {
      _complitedCount += complitedCount;
      int percentage = (int)((_complitedCount * MAX_TICKS) / totalCount);
      
      _downloadProgress.Tick(percentage);
    }

    public IProgressBar CreateChildDownloadProgress(string url)
    {
      return new ChildDownloadProgressBar(url);
    }

    public void Close()
    {
      _downloadProgress?.Tick(MAX_TICKS);
      _downloadProgress?.Dispose();
    }
  }
}
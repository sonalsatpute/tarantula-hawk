using System;
using ShellProgressBar;

namespace TarantulaHawkCore
{
  public class DownloadProgressBar : IProgressBar
  {
    private readonly ProgressBar _overallProgress;
    private const int MAX_TICKS = 100;
    private int _complitedCount;

    public DownloadProgressBar(string baseUri)
    {
      var overProgressOptions = new ProgressBarOptions
      {
        ForegroundColor = ConsoleColor.Yellow,
        BackgroundColor = ConsoleColor.DarkGray,
        ProgressCharacter = '─'
      };
      _overallProgress = new ProgressBar(MAX_TICKS, baseUri, overProgressOptions);
    }

    public void ReportProgress(long totalCount, int complitedCount)
    {
      _complitedCount += complitedCount;

      int percentage = (int)((_complitedCount * MAX_TICKS) / totalCount);
      _overallProgress.Tick(percentage);
    }

    public IProgressBar CreateChildDownloadProgress(string uri)
    {
      return new ChildDownloadProgressBar(_overallProgress, uri);
    }

    public void Close()
    {
      _overallProgress?.Tick(MAX_TICKS);
      _overallProgress?.Dispose();
    }
  }
}
using ShellProgressBar;

namespace TarantulaHawkCore
{
  public interface IProgressBar
  {
    void ReportProgress(long totalCount, int complitedCount);
    IProgressBar CreateChildDownloadProgress(string url);
    void Close();
  }
}
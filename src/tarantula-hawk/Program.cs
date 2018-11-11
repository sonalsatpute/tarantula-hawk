using System;
using TarantulaHawkCore;

namespace TarantulaHawk
{
  class Program
  {
    private const int BASE_URL = 0;
    private const int TARGET_FOLDER = 1;

    static void Main(string[] args)
    {
      Console.Clear();
      Console.CursorVisible = false;

      Program p = new Program();
      DownloadOptions options = p.ParseArgs(args);

      WebsiteDownloader websiteDownloader = new WebsiteDownloader(options);
      websiteDownloader.Start();

      Console.Clear();
      Console.WriteLine("Download completed, press any key to exit.");
      Console.ReadLine();
    }

    private DownloadOptions ParseArgs(string[] args)
    {
      if (args.Length != 2)
      {
        Console.WriteLine("Invalid argument(s)");
        Console.WriteLine("Usage:");
        Console.WriteLine(@"dotnet.exe .\tarantula-hawk.dll base_url targer_folder");
        Console.WriteLine("Example:");
        Console.WriteLine(@"dotnet.exe .\tarantula-hawk.dll https:\\google.com d:\temp");

        Environment.Exit(1);
      }

      return new DownloadOptions(args[BASE_URL], args[TARGET_FOLDER]);
    }
  }
}

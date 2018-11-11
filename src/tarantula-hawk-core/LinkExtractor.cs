using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TarantulaHawkCore
{
  public class LinkExtractor
  {
    const string HREF = "(?<=href=[\"\']).*?(?=[\"\'])";
    const string CONTENT = "(?<=content=[\"\'])http.*?(?=[\"\'])";
    const string SRC = "(?<=src=[\"\']).*?(?=[\"\'])";
    const string URL = "(?<=resource[\\(]).*?(?=[)'])";
    const string XML_LINK = "<link>(.+?)</link>";

    private readonly Regex _regex;

    public LinkExtractor()
    {
      _regex = new Regex($"{HREF}|{CONTENT}|{SRC}|{URL}|{XML_LINK}", RegexOptions.IgnoreCase);
    }

    public IEnumerable<string> GetLinks(string textContent)
    {
      MatchCollection mc = _regex.Matches(textContent);

      foreach (Match match in mc)
      {
        string link = match.Groups[0].Value;

        yield return link; ;
      }
    }

  }
}

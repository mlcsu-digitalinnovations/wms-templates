using Microsoft.AspNetCore.Html;

namespace NHSEI_WmsGPReferral.CustomClasses;

public class DownloadData
{
  public string Url { get; set; }
  public HtmlString Instructions { get; set; }
  public string Role { get; set; }
  public string FileVersion { get; set; }
}

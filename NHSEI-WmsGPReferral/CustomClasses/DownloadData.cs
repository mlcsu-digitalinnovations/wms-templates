using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSEI_WmsGPReferral.CustomClasses
{
    public class DownloadData
    {
        public string Url { get; set; }
        public HtmlString Instructions { get; set; }
        public string Role { get; set; }
        public string FileVersion { get; set; }
    }
}

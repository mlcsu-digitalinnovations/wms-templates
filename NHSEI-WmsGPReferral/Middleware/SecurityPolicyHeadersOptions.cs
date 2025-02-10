using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHSEI_WmsGPReferral.Middleware;

public class SecurityPolicyHeadersOptions
{
  public const string SectionKey = $"{nameof(SecurityPolicyHeadersOptions)}";

  [Required]
  public required Dictionary<string, string> Append { get; set; }
}

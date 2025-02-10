using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSEI_WmsGPReferral.Middleware;

public class SecurityPolicyHeadersMiddleware(
  RequestDelegate next,
  IOptions<SecurityPolicyHeadersOptions> options)
{
  private readonly RequestDelegate _nextDelegate = next;
  private readonly SecurityPolicyHeadersOptions _cspOptions = options.Value;

  public async Task InvokeAsync(HttpContext context)
  {
    foreach (KeyValuePair<string, string> header in _cspOptions.Append)
    {
      if (context.Response.Headers.ContainsKey(header.Key))
      {
        context.Response.Headers.Remove(header.Key);
      }

      context.Response.Headers.Append(header.Key, header.Value);
    }
    await _nextDelegate(context);
  }
}
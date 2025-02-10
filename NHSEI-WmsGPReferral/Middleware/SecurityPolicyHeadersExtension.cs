using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;

namespace NHSEI_WmsGPReferral.Middleware;

public static class SecurityPolicyHeadersExtension
{
  public static IServiceCollection AddSecurityPolicyHeaders(this IServiceCollection services)
  {
    services.AddOptions<SecurityPolicyHeadersOptions>()
      .BindConfiguration(SecurityPolicyHeadersOptions.SectionKey)
      .ValidateDataAnnotations()
      .ValidateOnStart();

    return services;
  }

  public static IApplicationBuilder UseSecurityPolicyHeaders(
    this IApplicationBuilder builder)
  {

    IOptions<SecurityPolicyHeadersOptions> options =
      builder.ApplicationServices.GetService<IOptions<SecurityPolicyHeadersOptions>>();

    bool ValidateOptions = Validator.TryValidateObject(
      options.Value,
      new ValidationContext(options.Value),
      [],
      true);

    if (ValidateOptions == false)
    {
      throw new InvalidOperationException(
       $"IOptions<{nameof(SecurityPolicyHeadersOptions)}> has not been added to the DI. Use " +
       $"{nameof(AddSecurityPolicyHeaders)} to add the service.");
    }

    return builder.UseMiddleware<SecurityPolicyHeadersMiddleware>();
  }
}
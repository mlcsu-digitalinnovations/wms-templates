using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NHSEI_WmsGPReferral.Middleware;
using NHSEI_WmsGPReferral.Services;
using Polly;
using Polly.Extensions.Http;
using System;

namespace NHSEI_WmsGPReferral;

public class Startup
{
  public Startup(IConfiguration configuration, IWebHostEnvironment env)
  {
    Configuration = configuration;
    _env = env;
  }

  public IConfiguration Configuration { get; }
  private readonly IWebHostEnvironment _env;

  // This method gets called by the runtime. Use this method to add services to the container.
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddSession(options =>
    {
      options.IdleTimeout = TimeSpan.FromSeconds(600);
      options.Cookie.HttpOnly = true;
      options.Cookie.Name = ".WmpGp.Session";
      options.Cookie.IsEssential = true;
    });

    services.AddHttpClient<INhsLookupService, NhsLookupService>(c =>
    {
      c.BaseAddress = new Uri("https://directory.spineservices.nhs.uk/ORD/2-0-0/organisations/");
    })
        .AddPolicyHandler((services, request) => HttpPolicyExtensions.HandleTransientHttpError()
        .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timespan, retryAttempt, context) =>
        {
          //log warning                    
          services.GetService<ILogger<NhsLookupService>>()?
                .LogWarning(outcome.Exception, "Delaying for {delay}ms, then making retry {retry}.",
                timespan.TotalMilliseconds,
                retryAttempt);
        }));


    services.AddHttpClient<IWmpService, WmpService>(c =>
    {
      if (_env.IsDevelopment())
      {
        //use PRE details
        c.BaseAddress = new Uri(Configuration["WmsTemplate:apiBaseAddress"]);
      }
      else
      {
        //use PROD details
        c.BaseAddress = new Uri(Configuration["WmsTemplate:apiBaseAddress"]);
      }

    })
        .AddPolicyHandler((services, request) => HttpPolicyExtensions.HandleTransientHttpError()
        .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timespan, retryAttempt, context) =>
        {
          //log warning
          services.GetService<ILogger<WmpService>>()?
                .LogWarning(outcome.Exception, "Delaying for {delay}ms, then making retry {retry}.",
                timespan.TotalMilliseconds,
                retryAttempt);
        }));

    services.AddTransient<IBlobService, BlobService>();
    services.AddControllersWithViews();
    services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
    services.AddHealthChecks();

    // Settings for security headers.
    services.AddSecurityPolicyHeaders();

  }

  // This method gets called by the runtime.
  // Use this method to configure the HTTP request pipeline.
  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
    }
    else
    {
      app.UseExceptionHandler("/Home/Error");
      // The default HSTS value is 30 days. You may want to change this for production
      // scenarios, see https://aka.ms/aspnetcore-hsts.
      app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseSession();

    app.UseAuthorization();

    app.UseSecurityPolicyHeaders();

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
      endpoints.MapHealthChecks("/health");
    });
  }
}

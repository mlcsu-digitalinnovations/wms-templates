using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSEI_WmsGPReferral.Models;
using System.Diagnostics;

namespace NHSEI_WmsGPReferral.Controllers;

public class HomeController : Controller
{
  private readonly ILogger<HomeController> _logger;

  public HomeController(ILogger<HomeController> logger)
  {
    _logger = logger;
  }

  public IActionResult Index() => View();

  public IActionResult Accessibility() => View();

  public IActionResult ContactUs() => View();

  public IActionResult Cookies() => View();

  public IActionResult Privacy() => View();

  public IActionResult TermsAndConditions() => View();

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error() => View(new ErrorViewModel
  {
    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
  });
}

using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NHSEI_WmsGPReferral.Models;
using NHSEI_WmsGPReferral.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NHSEI_WmsGPReferral.Controllers;

public class OrganisationInfoController : Controller
{
  private readonly ILogger<OrganisationInfoController> _logger;
  private readonly string _PharmacyTemplateVersion;
  private readonly INhsLookupService _nhsLookupService;
  private readonly IWmpService _wmpService;
  private readonly IBlobService _blobService;
  private const string SESSION_KEY_ODS_CODE = "OdsCode";
  private const string SESSION_KEY_ORG_NAME = "OrganisationName";
  private const string SESSION_KEY_USER_EMAIL = "UserEmail";
  private const string SESSION_KEY_CLINICAL_SYSTEM = "ClinicalSystem";
  private const string SESSION_KEY_ORGANISATION_ROLE = "Role";
  private readonly Dictionary<string, string> validRoleIds = new()
  {
    { "RO177", "Practice" },
    { "RO76", "Practice" },
    { "RO182", "Pharmacy" },
    { "RO280", "Pharmacy" }
  };

  public OrganisationInfoController(ILogger<OrganisationInfoController> logger,
      IConfiguration configuration,
      INhsLookupService nhsLookupService,
      IWmpService wmpService,
      IBlobService blobService)
  {
    _logger = logger;
    _PharmacyTemplateVersion = configuration["WmsTemplate:PharmacyTemplateVersion"];
    _nhsLookupService = nhsLookupService;
    _wmpService = wmpService;
    _blobService = blobService;
  }

  [HttpGet]
  public IActionResult Index()
  {
    OdsModel model = new();
    if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SESSION_KEY_ODS_CODE)))
    {
      model.OdsCode = HttpContext.Session.GetString(SESSION_KEY_ODS_CODE);
    }

    return View("Index", model);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> IndexAsync(OdsModel model)
  {
    if (ModelState.IsValid)
    {
      try
      {
        string responseContent = await _nhsLookupService.Get(model.OdsCode.Trim());
        string errorMessage = "";
        bool errorWithOdsCode = false;

        PracticeDataModel practiceData = new();
        switch (responseContent)
        {
          case "NotFound":
            errorMessage = "ODS code not recognised";
            ModelState.AddModelError("OdsCode", errorMessage);
            errorWithOdsCode = true;
            break;
          case "BadRequest":
            errorMessage = "Enter a valid ODS code";
            ModelState.AddModelError("OdsCode", errorMessage);
            errorWithOdsCode = true;
            break;
          default:
            JObject responseJson = JObject.Parse(responseContent);

            practiceData = new PracticeDataModel()
            {
              OdsCode =
                model.OdsCode.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Trim(),
              OrganisationName = responseJson.SelectToken("$.Organisation.Name").Value<string>(),
              OrganisationCountry = responseJson
                .SelectToken("$.Organisation.GeoLoc.Location.Country").Value<string>()
                .ToLower(System.Globalization.CultureInfo.CurrentCulture),
              OrganisationRoles = responseJson
                .SelectTokens("$.Organisation.Roles.Role[*].id").Values<string>()
            };
            break;
        }

        if (errorWithOdsCode)
        {
          // If lookup gave an error, return view with error
          return View(model);
        }
        else
        {
          // Organisation data found, check it is correctly formed
          if (practiceData.OrganisationName != null)
          {
            // Codes taken from https://digital.nhs.uk/services/organisation-data-service/guidance-for-developers/search-parameters#primaryroleid-and-nonprimaryroleid
            // RO177 applies to GPs and other prescribing centres
            // RO76 applies to "GP Practice"
            // RO182 applies to "Pharmacy"
            // RO280 applies to "Pharmacy site"
            // Roles list https://directory.spineservices.nhs.uk/ORD/2-0-0/roles


            JObject responseJson = JObject.Parse(responseContent);

            bool isGPorPharmacist = practiceData.OrganisationRoles
              .Any(value => validRoleIds.ContainsKey(value));
            bool isGPorPharmacistInEngland = practiceData.OrganisationCountry == "england";

            if (isGPorPharmacist && isGPorPharmacistInEngland)
            {
              HttpContext.Session.SetString(SESSION_KEY_ODS_CODE, practiceData.OdsCode.ToString());
              HttpContext.Session
                .SetString(SESSION_KEY_ORG_NAME, practiceData.OrganisationName.ToString());
              HttpContext.Session
                .SetString(SESSION_KEY_ORGANISATION_ROLE,
                  practiceData.OrganisationRoles.FirstOrDefault().ToString());
              //var role = HttpContext.Session.GetString(SESSION_KEY_ORGANISATION_ROLE);
              return RedirectToAction("ConfirmOds", new { odsCode = practiceData.OdsCode });
            }
            else
            {
              if (!isGPorPharmacist)
              {
                errorMessage = "ODS code does not match an NHS GP Practice or Pharmacy";
              }
              else if (!isGPorPharmacistInEngland)
              {
                errorMessage = "This service is currently open to " +
                  "GP Practices & Pharmacies in England only";
              }

              ModelState.AddModelError("OdsCode", errorMessage);
              return View(model);
            }

          }
          else
          {
            // Problem with returned data, not captured above
            errorMessage = "There was a problem with the data returned from the" +
              " organisation lookup. Please try again. If the problem persists, contact" +
              " us using the details below.";
            _logger.LogError($"{model.OdsCode} - missing data on NHS API.");
            return View("Error", GetErrorModel(errorMessage));
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError($"{model.OdsCode} - " + ex.ToString());
        return View("Error", GetErrorModel(ex.ToString()));
      }
    }
    else
    {
      return View(model);
    }
  }

  [HttpGet]
  [Route("{controller}/ConfirmPractice/{odsCode}")]
  public IActionResult ConfirmOds(string odsCode)
  {
    string sessionOdsCode = HttpContext.Session.GetString(SESSION_KEY_ODS_CODE);
    if (sessionOdsCode == odsCode)
    {
      PracticeDataModel model = new()
      {
        OdsCode = HttpContext.Session.GetString(SESSION_KEY_ODS_CODE),
        OrganisationName = HttpContext.Session.GetString(SESSION_KEY_ORG_NAME)

      };
      return View(model);
    }
    else
    {
      _logger.LogError("Session cookie didn't match model");
      return View("Error", GetErrorModel("Data did not match. Close this window and try again"));
    }

  }

  [Route("{controller}/CheckExistingRecords/{odsCode}")]
  public async Task<IActionResult> CheckExistingRecordsAsync(string odsCode)
  {
    // Check if ODS code is already in DB
    string responseContent = await _wmpService.GetPractice(odsCode, OdsOrgType());

    if (responseContent != "NotFound" && responseContent != "NoContent")
    {
      if (OdsOrgType() == "Pharmacy")
      {
        // Code found - record already in DB
        JObject responseJson = JObject.Parse(responseContent);
        string templateVersion = responseJson.SelectToken("$.templateVersion")?.Value<string>();
        string returnedEmail = responseJson.SelectToken("$.email")?.Value<string>();
        string returnedODSCode = responseJson.SelectToken("$.odsCode")?.Value<string>();
        if (templateVersion != _PharmacyTemplateVersion)
        {
          //Version has changed since Pharmacy last downloaded
          PostPharmacyWmpDataModel postWmpDataModel = new()
          {
            Email = returnedEmail,
            OdsCode = returnedODSCode,
            Name = returnedODSCode,
            TemplateVersion = templateVersion
          };

          HttpResponseMessage putResponse =
            await _wmpService.UpdatePharmacyItemAsync(postWmpDataModel);

          if (putResponse.StatusCode == HttpStatusCode.BadRequest)
          {
            // 400 error - properties are invalid
            _logger.LogError($"{postWmpDataModel.OdsCode} - WMP Service returned 400 error");
          }
          else
          {
            // 500 error - database row creation failed
            _logger.LogError($"{postWmpDataModel.OdsCode} - WMP Service returned 500 error");
          }
        }

        // Skip to downloads
        return RedirectToAction("Downloads", new { clinicalSystem = "Pharmacy" });

      }
      else // "Practice"
      {
        // Code found - record already in DB
        JObject responseJson = JObject.Parse(responseContent);
        string clinicalSystem = responseJson.SelectToken("$.systemName")?.Value<string>();

        // Skip to downloads
        return RedirectToAction("Downloads", new { clinicalSystem });
      }
    }
    else
    {
      // Not found - continue 
      return RedirectToAction("UserEmail");
    }
  }

  //Enter Your Email Address
  public IActionResult UserEmail()
  {
    UserEmailModel model = new();
    if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SESSION_KEY_USER_EMAIL)))
    {
      model.UserEmail = HttpContext.Session.GetString(SESSION_KEY_USER_EMAIL);
    }

    return View(model);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult UserEmail(UserEmailModel model)
  {
    if (ModelState.IsValid)
    {
      HttpContext.Session.SetString(SESSION_KEY_USER_EMAIL, model.UserEmail);
    }
    else
    {
      return View(model);
    }

    if (OdsOrgType() == "Pharmacy")
    {
      HttpContext.Session.SetString(SESSION_KEY_CLINICAL_SYSTEM, "");
      return RedirectToAction("ConfirmData");
    }

    return RedirectToAction("SelectClinicalSystem");
  }

  //Select GP System
  public IActionResult SelectClinicalSystem()
  {
    SelectClinicalSystemModel model = new();
    if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SESSION_KEY_CLINICAL_SYSTEM)))
    {
      SelectListItem selectedSystem = model.ClinicalSystemList
        .Where(x => x.Value == HttpContext.Session.GetString(SESSION_KEY_CLINICAL_SYSTEM)).First();
      selectedSystem.Selected = true;
    }

    return View(model);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult SelectClinicalSystem(SelectClinicalSystemModel model)
  {
    if (ModelState.IsValid)
    {
      HttpContext.Session.SetString(SESSION_KEY_CLINICAL_SYSTEM, model.SelectClinicalSystem);
    }
    else
    {
      return View(model);
    }

    return RedirectToAction("ConfirmData");
  }

  //Confirm GP System
  public IActionResult ConfirmData()
  {
    ConfirmDataModel model = new()
    {
      OdsCode = HttpContext.Session.GetString(SESSION_KEY_ODS_CODE),
      Organisation = HttpContext.Session.GetString(SESSION_KEY_ORG_NAME),
      Email = HttpContext.Session.GetString(SESSION_KEY_USER_EMAIL),
      ClinicalSystem = HttpContext.Session.GetString(SESSION_KEY_CLINICAL_SYSTEM)
    };

    return View(model);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> ConfirmDataAsync(ConfirmDataModel model)
  {
    //Org Type
    string odsOrgType = OdsOrgType();
    HttpResponseMessage responseContent;
    HttpStatusCode responseCode;

    if (ModelState.IsValid)
    {
      if (odsOrgType == "Practice")
      {
        PostPracticeWmpDataModel postWmpDataModel = new()
        {
          Email = model.Email,
          OdsCode = model.OdsCode,
          SystemName = model.ClinicalSystem
        };

        responseContent = await _wmpService.CreatePracticeItemAsync(postWmpDataModel);
        responseCode = responseContent.StatusCode;
      }
      else
      {
        PostPharmacyWmpDataModel postWmpDataModel = new()
        {
          Email = model.Email,
          OdsCode = model.OdsCode,
          Name = model.OdsCode,
          TemplateVersion = _PharmacyTemplateVersion
        };
        model.ClinicalSystem = "Pharmacy";
        responseContent = await _wmpService.CreatePharmacyItemAsync(postWmpDataModel);
        responseCode = responseContent.StatusCode;
      }

      if (responseCode == HttpStatusCode.OK)
      {
        // 200 - successful 
        return RedirectToAction("Downloads", new { clinicalSystem = model.ClinicalSystem });
      }
      else if (responseCode == HttpStatusCode.BadRequest)
      {
        // 400 error - properties are invalid
        _logger.LogError($"{model.OdsCode} - WMP Service returned 400 error");
        return View("Error", GetErrorModel("Some of your answers were in the wrong format. "));
      }
      else
      {
        // 500 error - database row creation failed
        _logger.LogError($"{model.OdsCode} - WMP Service returned 500 error");
        return View("Error", GetErrorModel("There was a problem saving your answers. "));
      }
    }
    else
    {
      // model error - shouldn't happen
      return View(model);
    }
  }

  //Downloads Page
  [Route("{controller}/Downloads/{clinicalSystem}")]
  public IActionResult Downloads(string clinicalSystem)
  {

    // remove cookies
    RemoveSessionCookies();

    DownloadsModel model = new()
    {
      ChosenClinicalSystem = clinicalSystem,
      SelectedRole = clinicalSystem == "Pharmacy" ? "pharmacy" : "practice",
      FileVersion = clinicalSystem == "Pharmacy" ? _PharmacyTemplateVersion : ""
    };

    //return results
    return View(model);
  }

  public async Task<IActionResult> Download(string id)
  {
    //Get the blob from storage and stream to client
    BlobDownloadInfo blob = await _blobService.Download(id);
    if (blob != null)
    {
      return File(blob.Content, blob.ContentType, id);
    }

    return BadRequest();
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    RemoveSessionCookies();
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }

  private ErrorViewModel GetErrorModel(string message)
  {
    return new ErrorViewModel()
    {
      RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
      Message = $"Error: {message}."
    };
  }

  private void RemoveSessionCookies()
  {
    HttpContext.Session.Remove(SESSION_KEY_ODS_CODE);
    HttpContext.Session.Remove(SESSION_KEY_ORG_NAME);
    HttpContext.Session.Remove(SESSION_KEY_USER_EMAIL);
    HttpContext.Session.Remove(SESSION_KEY_CLINICAL_SYSTEM);
  }

  private string OdsOrgType()
  {
    string role = HttpContext.Session.GetString(SESSION_KEY_ORGANISATION_ROLE);
    if (!validRoleIds.TryGetValue(role, out string orgType))
    {
      orgType = "NotFound";
    }

    return orgType;
  }
}

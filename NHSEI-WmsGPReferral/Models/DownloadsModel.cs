using Microsoft.AspNetCore.Html;
using NHSEI_WmsGPReferral.CustomClasses;
using System.Collections.Generic;

namespace NHSEI_WmsGPReferral.Models;

public class DownloadsModel
{

  public string ChosenClinicalSystem { get; set; }
  public string SelectedRole { get; set; }
  public string FileVersion { get; set; }
  public Dictionary<string, DownloadData> DownloadDictionary { get; } = new Dictionary<string, DownloadData>
      {
          {"Emis", new DownloadData {
              Url = "/OrganisationInfo/Download/NHS%20Digital%20Weight%20Management%20Programme%20-%20EMIS%20Resource%20Pack.zip",
              Instructions = new HtmlString("<p>The zip file will contain:</p>"
                  + "<ol class=\"nhsuk-list nhsuk-list--number\">"
                  + "<li>Digital Weight Management Resource Guide &ndash; this document will contain details on how to download and import the remaining items in this list to your EMIS clinical system.</li>"
                  + "<li>EMIS Referral Form &ndash; Practices <strong><u>must</u></strong> use this template to refer service users into the NHSE Digital Weight Management Programme. </li>"
                  + "<li>EMIS Search &ndash; this search will enable practices to search for eligible service users for the NHS Digital Weight Management Programme. The outcome of the search is subject to exclusion criteria such as pregnancy and unmanaged co-morbidities. </li>"
                  + "<li>EMIS Protocol &ndash; this protocol will alert practices to identify eligible service users when attending other appointment.</li>"
                  + "<li>E-Referral System Service Search – this is a guide to the search options to locate the NHSE Digital Weight Management Programme for referrals.</li>"
                  + "<li>Guidance for GP practices and service users – these guides are to explain the service to all members of the Practice and provide a reference document for patients following referral.</li>"
                  + "</ol>"), Role = "practice" }
          },
          {"SystmOne", new DownloadData {
              Url = "/OrganisationInfo/Download/NHS%20Digital%20Weight%20Management%20Programme%20-%20TPPSystmOne%20ResourcePack.zip",
              Instructions = new HtmlString("<p>The zip file will contain:</p>"
                  + "<ol class=\"nhsuk-list nhsuk-list--number\">"
                  + "<li>Digital Weight Management Resource Guide &ndash; this document will contain how to download and import the remaining items in this list to your TPP SystmOne clinical system.</li>"
                  + "<li>SystmOne Referral Form &ndash; Practices <strong><u>must</u></strong> must use this template to refer service users into the NHSE Digital Weight Management Programme.</li>"
                  + "<li>SystmOne Search &ndash; this search will enable practices to search for eligible service users for the NHS Digital Weight Management Programme. The outcome of the search is subject to exclusion criteria such as pregnancy and unmanaged co-morbidities. </li>"
                  + "<li>E-Referral System Service Search – this is a guide to the search options to locate the NHSE Digital Weight Management Programme for referrals.</li>"
                  + "<li>Guidance for GP practices and service users – these guides are to explain the service to all members of the Practice and provide a reference document for patients following referral.</li>"
                  + "</ol>"), Role = "practice" }
          },
          {"Vision", new DownloadData {
              Url = "/OrganisationInfo/Download/NHS%20Digital%20Weight%20Management%20Programme%20-%20Vision%20ResourcePack.zip",
              Instructions = new HtmlString("<p>The zip file will contain:</p>"
                  + "<ol class=\"nhsuk-list nhsuk-list--number\">"
                  + "<li>Digital Weight Management Resource Guide &ndash; this document will contain how to download and import the remaining items in this list to your Vision clinical system.</li>"
                  + "<li>Vision Referral Form &ndash; Practices <strong><u>must</u></strong> must use this template to refer service users into the NHSE Digital Weight Management Programme.</li>"
                  + "<li>Vision Search &ndash; this search will enable practices to search for eligible service users for the NHS Digital Weight Management Programme. The outcome of the search is subject to exclusion criteria such as pregnancy and unmanaged co-morbidities. </li>"
                  + "<li>Guidance for GP practices and service users – these guides are to explain the service to all members of the Practice and provide a reference document for patients following referral.</li>"
                  + "</ol>"), Role = "practice" }
          },
          {"Pharmacy", new DownloadData {
              Url = $"/OrganisationInfo/Download/NHS%20Digital%20Weight%20Management%20Programme%20-%20PharmacyTemplate",
              Instructions = new HtmlString("<p>The zip file will contain:</p>"
                  + "<ol class=\"nhsuk-list nhsuk-list--number\">"
                  + "<li><strong>User guide:</strong><br />All referrals should be submitted online <a href=\"https://pharmacy.wmp.nhs.uk/\">https://pharmacy.wmp.nhs.uk/</a>. The below provides a comprehensive guide that will support pharmacy teams completion of referrals into the NHS Digital Weight Management Programme.  </li>"
                  + "<li><strong>Template:</strong><br />This document can be used during a consultation with the patient to collect patient information required for the NHS Digital Weight Management Programme if the pharmacy team does not have access to the online form at point of consultation.</li>"
                  + "<li><strong>Example:</strong><br />This document provides a template on how to complete a referral into the NHS Digital Weight Management Programme form the pharmacy referral pathway.</li>"
                  + "</ol>"), Role = "pharmacy" }
          }
      };
}

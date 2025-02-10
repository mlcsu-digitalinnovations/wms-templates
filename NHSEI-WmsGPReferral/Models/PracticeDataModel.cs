using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHSEI_WmsGPReferral.Models;

public class PracticeDataModel : OdsModel
{
  [Display(Name = "Organisation name")]
  public string OrganisationName { get; set; }
  public string OrganisationCountry { get; set; }
  public IEnumerable<string> OrganisationRoles { get; set; }
}

// Ignore Spelling: Organisation

using System.ComponentModel.DataAnnotations;

namespace NHSEI_WmsGPReferral.Models;

public class ConfirmDataModel
{
  [Display(Name = "Confirm data fields")]

  [Required(AllowEmptyStrings = false)]
  public string OdsCode { get; set; }

  [Required(AllowEmptyStrings = false)]
  public string Organisation { get; set; }

  [Required(AllowEmptyStrings = false)]
  public string Email { get; set; }

  public string ClinicalSystem { get; set; }
}

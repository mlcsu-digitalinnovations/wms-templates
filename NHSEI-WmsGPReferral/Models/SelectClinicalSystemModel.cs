using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHSEI_WmsGPReferral.Models;

public class SelectClinicalSystemModel
{
  [Display(Name = "Select your practices GP system")]
  [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a GP system from the list below")]
  public string SelectClinicalSystem { get; set; }

  public List<SelectListItem> ClinicalSystemList { get; } =
    [
      new() { Value = "Emis", Text = "Emis" },
      new() { Value = "SystmOne", Text = "SystmOne" },
      new() { Value = "Vision", Text = "Vision"  }
    ];
}

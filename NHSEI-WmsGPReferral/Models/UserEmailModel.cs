using NHSEI_WmsGPReferral.Validations;
using System.ComponentModel.DataAnnotations;

namespace NHSEI_WmsGPReferral.Models;

public class UserEmailModel
{
  [Display(Name = "Email address")]
  [EmailValidation]
  public string UserEmail { get; set; }
}

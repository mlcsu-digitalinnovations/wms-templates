using System.ComponentModel.DataAnnotations;

namespace NHSEI_WmsGPReferral.Models;

public class OdsModel
{
  [Display(Name = "What is your organisation ODS code?")]
  [Required(ErrorMessage = "Enter a valid ODS Code")]
  //[RegularExpression(@"^[A-Z]{1}[0-9]{5}\S*$", ErrorMessage = "Enter a valid ODS Code using capital letters and without any spaces")]
  [StringLength(6, ErrorMessage = "Enter a valid ODS Code", MinimumLength = 5)]

  public string OdsCode { get; set; }
  public string Token { get; set; }
}

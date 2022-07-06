using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSEI_WmsGPReferral.Helpers;
using NHSEI_WmsGPReferral.Models;

namespace NHSEI_WmsGPReferral.Validations
{
    public class EmailValidation : ValidationAttribute
    {
        public string GetErrorMessage() =>
      $"Enter your email address";
        protected override ValidationResult? IsValid(object? value,
      ValidationContext validationContext)
        {
            UserEmailModel model = (UserEmailModel)validationContext.ObjectInstance;

            if (string.IsNullOrEmpty(model.UserEmail))
                return new ValidationResult(
                  GetErrorMessage() + " to continue with your download");

            if (!RegexUtilities.IsValidEmail(model.UserEmail))
                return new ValidationResult(
                  GetErrorMessage() + " in the correct format, like " +
                  "name@example.com");

            return ValidationResult.Success;
        }
    }
}

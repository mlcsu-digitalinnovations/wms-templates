using NHSEI_WmsGPReferral.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NHSEI_WmsGPReferral.Models
{
    public class UserEmailModel
    {
        [Display(Name = "Email address")]
        [EmailValidation]
        public string UserEmail { get; set; }
    }
}

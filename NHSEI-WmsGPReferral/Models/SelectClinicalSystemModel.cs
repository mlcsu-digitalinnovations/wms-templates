using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NHSEI_WmsGPReferral.Models
{
    public class SelectClinicalSystemModel
    {
        [Display(Name = "Select your practices GP system")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select a GP system from the list below")]
        public string SelectClinicalSystem { get; set; }

        public List<SelectListItem> ClinicalSystemList { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "Emis", Text = "Emis" },
            new SelectListItem { Value = "SystmOne", Text = "SystmOne" },
            new SelectListItem { Value = "Vision", Text = "Vision"  }
        };
    }
}

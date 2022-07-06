using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NHSEI_WmsGPReferral.Models
{
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
}

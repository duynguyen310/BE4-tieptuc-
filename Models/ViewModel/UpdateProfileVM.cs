using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BE4.Models.ViewModel
{
    public class UpdateProfileVM
    {
        [Required]
        [Phone]
        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; }

        [Required]
        [Display(Name = "Địa chỉ")]
        public string CustomerAddress { get; set; }
    }
}
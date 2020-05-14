using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bugaboo.Models
{
    public class RegisterProduct
    {
        [Required, Display(Name = "CompanyName")]
        public string CompanyName { get; set; }

        public bool IsProductRegistered { get; set; }
    }
}
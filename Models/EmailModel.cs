using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bugaboo.Models
{
    public class EmailModel
    {
        [Required, Display(Name = "Name")]
        public string FromName { get; set; }

        [Required, Display(Name = "Name"), EmailAddress]
        public string FromEmail { get; set; }
        public string Subject { get; set; }
        //[AllowHtml]
        public string Body { get; set; }
    }
}
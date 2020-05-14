using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bugaboo.ViewModels
{
    public class PersonelRoleData
    {
        public string FullName { get; set; }
        public string PersonelRole { get; set; }
        public string PersonelId { get; set; }
        public List<string> ProjectNames { get; set; }

        public List<int> ProjectIds { get; set; }
    }
}
using Bugaboo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Bugaboo.Helpers
{
    public class NameHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string GetUserName(string user)
        {
            if (!String.IsNullOrEmpty(user))
            {
                var firstName = db.Users.Find(user).FirstName;

                if (!string.IsNullOrWhiteSpace(firstName))
                    return firstName;
            }
            return "Unassigned";
        }
    }
}
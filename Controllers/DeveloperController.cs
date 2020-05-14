using Bugaboo.Helpers;
using Bugaboo.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bugaboo.Controllers
{
    [Authorize(Roles = "SuperAdministrator, ProjectManager, Administrator, Developer")]
    public class DeveloperController : Controller
    {
        private RoleHelper roleHelper = new RoleHelper();
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Developer
        public ActionResult ProjectIndex()
        {
            if (User.Identity.Name != "")
            {
                var firstName = db.Users.Find(User.Identity.GetUserId()).FirstName;

                if (!string.IsNullOrWhiteSpace(firstName))
                    ViewData.Add("FirstName", firstName);
            }
            var phelper = new ProjectHelper();
            List<Project> projects;
            if (User.IsInRole("Developer"))
                projects = phelper.ListUserProject(User.Identity.GetUserId()).ToList();
            else
                projects = db.Projects.ToList();
            return View(projects);
        }
        public ActionResult TicketsIndex()
        {

            if (User.Identity.Name != "")
            {
                var firstName = db.Users.Find(User.Identity.GetUserId()).FirstName;

                if (!string.IsNullOrWhiteSpace(firstName))
                    ViewData.Add("FirstName", firstName);
            }
            var phelper = new ProjectHelper();
            var projects = phelper.ListUserProject(User.Identity.GetUserId());
            List<Ticket> tickets = new List<Ticket>();
            foreach (var pro in projects)
            {
                foreach (var tic in pro.Tickets)
                {
                    tickets.Add(tic);
                }
            }
            return View(tickets);
        }
    }
}
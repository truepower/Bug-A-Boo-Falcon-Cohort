using Bugaboo.Helpers;
using Bugaboo.Models;
using Bugaboo.ViewModels;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bugaboo.Controllers
{
    [Authorize(Roles = "Administrator, SuperAdministrator")]
    public class AdminController : Controller
    {
        private RoleHelper roleHelper = new RoleHelper();
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult ManageRoles()
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (User.Identity.Name != "")
            {
                var firstName = db.Users.Find(User.Identity.GetUserId()).FirstName;

                if (!string.IsNullOrWhiteSpace(firstName))
                    ViewData.Add("FirstName", firstName);
            }
            var roleHelper = new RoleHelper();
            var Pm = roleHelper.UserInRole("ProjectManager");
            var emptyProjects = new List<Project>();
            foreach (var proj in db.Projects)
            {
                if (!String.IsNullOrEmpty(proj.ProjectManager))
                    continue;
                emptyProjects.Add(proj);
            }
            ViewBag.personnel = new MultiSelectList(db.Users, "Id", "Email");
            ViewBag.RoleName = new SelectList(db.Roles, "Name", "Name");
            ViewBag.Projects = new SelectList(emptyProjects, "Id", "Name", "ID");
            ViewBag.PMAddedToProject = new SelectList(Pm, "Id", "FullName");
            var personelData = new List<PersonelRoleData>();
            var personel = db.Users.ToList();
            var projectHelper = new ProjectHelper();
            foreach (var person in personel)
            {
                personelData.Add(new PersonelRoleData
                {
                    FullName = person.FullName,
                    PersonelRole = roleHelper.ListUserRoles(person.Id).FirstOrDefault() ?? "Unassign",
                    ProjectNames = projectHelper.ListUserProject(person.Id).Select(p => p.Name).ToList(),
                    ProjectIds = projectHelper.ListUserProject(person.Id).Select(p => p.Id).ToList()

            });
            }
            return View(personelData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageRoles(List<string> personnel, string roleName)
        {
            if (personnel != null)
            {
                foreach (var person in personnel)
                {
                    var personRole = roleHelper.ListUserRoles(person).FirstOrDefault();
                    if (!String.IsNullOrEmpty(personRole))
                    {
                        roleHelper.RemoveUserFromRole(person, personRole);
                    }
                    if (!String.IsNullOrEmpty(roleName))
                    roleHelper.AddUserToRole(person, roleName);
                }
            }
            return RedirectToAction("ManageRoles");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddedToProject(int projects, string PMAddedToProject)
        {
            var pj = db.Projects.Find(projects);
            var phelper = new ProjectHelper();
            phelper.AddUserToProject(PMAddedToProject, projects);
            pj.ProjectManager = PMAddedToProject;
            db.SaveChanges();

            return RedirectToAction("ManageRoles");
        }
    }
}
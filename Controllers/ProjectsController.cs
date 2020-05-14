using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Bugaboo.Helpers;
using Bugaboo.Models;
using Bugaboo.ViewModels;
using Microsoft.AspNet.Identity;

namespace Bugaboo.Controllers
{
    [Authorize(Roles = "SuperAdministrator, ProjectManager, Administrator")]
    public class ProjectsController : Controller
    {
        private RoleHelper roleHelper = new RoleHelper();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public ActionResult Index()
        {
            if (User.Identity.Name != "")
            {
                var firstName = db.Users.Find(User.Identity.GetUserId()).FirstName;

                if (!string.IsNullOrWhiteSpace(firstName))
                    ViewData.Add("FirstName", firstName);
            }
            var phelper = new ProjectHelper();
            List<Project> projects;
            if (User.IsInRole("ProjectManager"))
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
            foreach(var pro in projects)
            {
                foreach (var tic in pro.Tickets)
                {
                    tickets.Add(tic);
                }
            }
            return View(tickets);
        }
            public ActionResult ManageAssignments()
        {
            
            if (User.Identity.Name != "")
            {
                var firstName = db.Users.Find(User.Identity.GetUserId()).FirstName;

                if (!string.IsNullOrWhiteSpace(firstName))
                    ViewData.Add("FirstName", firstName);
            }
            var users = db.Users.ToList();
            ViewBag.UserIds = new MultiSelectList(users, "Id", "FullName");
            ViewBag.ProjectIds = new MultiSelectList(db.Projects, "Id", "Name");
            var projectData = new List<PersonelRoleData>();
            var projectHelper = new ProjectHelper();
            ViewData.Add("myUsers", users);
            foreach (var user in users)
            {
                var prd = new PersonelRoleData();

                prd.FullName = user.FullName;
                prd.PersonelId = user.Id;
                prd.PersonelRole = roleHelper.ListUserRoles(user.Id).FirstOrDefault() ?? "Unassign";
                prd.ProjectNames = projectHelper.ListUserProject(user.Id).Select(p => p.Name).ToList();
                prd.ProjectIds = projectHelper.ListUserProject(user.Id).Select(p => p.Id).ToList();
                projectData.Add(prd);
            }
            return View(projectData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageAssignments(List<string> userIds, List<int> projectIds)
        {
            var projectHelper = new ProjectHelper();
            if (userIds == null || projectIds == null)
                return RedirectToAction("ManageAssignments");
            foreach (var user in userIds)
            {
                foreach (var project in projectIds)
                {
                    projectHelper.AddUserToProject(user, project);
                }
            }
            return RedirectToAction("ManageAssignments");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromProject(string userId, int projectId)
        {
            var projectHelper = new ProjectHelper();
            if (userId == null || projectId == 0)
                return RedirectToAction("ManageAssignments");
           
                    projectHelper.RemoveUserFromProject(userId, projectId);
               
            return RedirectToAction("ManageAssignments");
        }
        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (User.Identity.Name != "")
            {
                var firstName = db.Users.Find(User.Identity.GetUserId()).FirstName;

                if (!string.IsNullOrWhiteSpace(firstName))
                    ViewData.Add("FirstName", firstName);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            if (User.Identity.Name != "")
            {
                var firstName = db.Users.Find(User.Identity.GetUserId()).FirstName;

                if (!string.IsNullOrWhiteSpace(firstName))
                    ViewData.Add("FirstName", firstName);
            }
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,ProjectId,Created,Updated,IsArchived")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (User.Identity.Name != "")
            {
                var firstName = db.Users.Find(User.Identity.GetUserId()).FirstName;

                if (!string.IsNullOrWhiteSpace(firstName))
                    ViewData.Add("FirstName", firstName);
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,ProjectId,Created,Updated,IsArchived")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Bugaboo.Helpers;
using Bugaboo.Models;
using Microsoft.AspNet.Identity;

namespace Bugaboo.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectHelper projectHelper = new ProjectHelper();
        // GET: Tickets
        public ActionResult Index()
        {
            if (User.Identity.Name != "")
            {
                var firstName = db.Users.Find(User.Identity.GetUserId()).FirstName;

                if (!string.IsNullOrWhiteSpace(firstName))
                    ViewData.Add("FirstName", firstName);
            }
            var tickets = db.Tickets.Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
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
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create(int? projectId)
        {
            if (User.Identity.Name != "")
            {
                var firstName = db.Users.Find(User.Identity.GetUserId()).FirstName;

                if (!string.IsNullOrWhiteSpace(firstName))
                    ViewData.Add("FirstName", firstName);
            }
            var user = User.Identity.GetUserId();
            var projects = projectHelper.ListUserProject(user);
            ViewBag.ProjectId = new SelectList(projects, "Id", "Name");
            ViewBag.SubmitterId =  new SelectList(db.Users, "Id", "FirstName");
            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            
            var ticket = new Ticket();
            if (projectId != null)
                ticket.ProjectId = (int)projectId;
            return View(ticket);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Description,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId")] Ticket ticket, int? projectId)
        {
            var user = User.Identity.GetUserId();
            var projects = projectHelper.ListUserProject(user);
            var roleHelper = new RoleHelper();
            if (ModelState.IsValid)
            {
                ticket.CreatedDate = DateTime.Now;
                ticket.SubmitterId = db.Users.Find(User.Identity.GetUserId()).Id;
                ticket.TicketStatusId = db.TicketStatus.FirstOrDefault(u => u.Name == "New").Id;
                // if (projectId != null && projectId != 0)
                //ticket.ProjectId = (int)projectId;
                
                
                var uir = db.Projects.Find(ticket.ProjectId).Users;
                foreach (var ur in uir)
                {
                    if (roleHelper.IsUserInRole(ur.Id, "ProjectManager"))
                    {
                        ticket.ProjectManager = db.Users.Find(ur.Id).FullName;
                    }
                }
                ticket.Submitter = db.Users.Find(User.Identity.GetUserId());
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           
            if (projectId == 0)
            ViewBag.ProjectId = new SelectList(projects, "Id", "Name", ticket.ProjectId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.DeveloperId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Submitter,Administrator,SuperAdministrator,ProjectManager")]
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
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            
            
           
            ViewBag.ProjectId = db.Projects.Find(ticket.ProjectId); //new SelectList(projects, "Id", "Name", ticket.ProjectId); 
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,DeveloperId,SubmitterId")] Ticket ticket)
        {
           
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var user = User.Identity.GetUserId();
            var projects = projectHelper.ListUserProject(user);
            ViewBag.ProjectId = new SelectList(projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            ticket.UpdatedDate = DateTime.Now;
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
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
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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

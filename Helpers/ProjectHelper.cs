using Bugaboo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace Bugaboo.Helpers
{
    public class ProjectHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserOnProject(string userId, int projectid)
        {
            var project = db.Projects.Find(projectid);
            var flag = project.Users.Any(u => u.Id == userId);
            return flag;
        }
        public ICollection<Project> ListUserProject(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            var projects = user.Projects.ToList();
            return projects;
        }
        public void AddUserToProject(string userId, int projectId)
        {
            if (!IsUserOnProject(userId,projectId))
            {
                var project = db.Projects.Find(projectId);
                var newUser = db.Users.Find(userId);
                project.Users.Add(newUser);
                db.SaveChanges();

            }
        }
        public void RemoveUserFromProject(string userId, int projectId)
        {
            if (IsUserOnProject(userId, projectId))
            {
                var project = db.Projects.Find(projectId);
                var newUser = db.Users.Find(userId);
                project.Users.Remove(newUser);
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();

            }
            
        }
        public ICollection<ApplicationUser> UserOnProject(int projectId)
        {
            return db.Projects.Find(projectId).Users;
        }
        public ICollection<ApplicationUser> UserNotOnProject(int projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
        }
    }
}
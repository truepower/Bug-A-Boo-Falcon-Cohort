﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Bugaboo.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AvatarPath { get; set; }

        

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{LastName},{FirstName}";
            }
        }
        
        public virtual ICollection<Project> Projects { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; }

        public virtual ICollection<TicketAttachment> Attachments { get; set; }

        public virtual ICollection<TicketHistory> Histories { get; set; }

        public ApplicationUser()
        {
            Projects = new HashSet<Project>();
            Comments = new HashSet<TicketComment>();
            Attachments = new HashSet<TicketAttachment>();
            Histories = new HashSet<TicketHistory>();
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<Bugaboo.Models.Project> Projects { get; set; }

        public System.Data.Entity.DbSet<Bugaboo.Models.Ticket> Tickets { get; set; }

        public System.Data.Entity.DbSet<Bugaboo.Models.TicketAttachment> TicketAttachments { get; set; }

        public System.Data.Entity.DbSet<Bugaboo.Models.TicketComment> TicketComments { get; set; }

        public System.Data.Entity.DbSet<Bugaboo.Models.TicketHistory> TicketHistories { get; set; }

        public System.Data.Entity.DbSet<Bugaboo.Models.TicketNotification> TicketNotifications { get; set; }

        public System.Data.Entity.DbSet<Bugaboo.Models.TicketPriority> TicketPriorities { get; set; }

        public System.Data.Entity.DbSet<Bugaboo.Models.TicketStatus> TicketStatus { get; set; }

        public System.Data.Entity.DbSet<Bugaboo.Models.TicketType> TicketTypes { get; set; }

        //public System.Data.Entity.DbSet<Bugaboo.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}
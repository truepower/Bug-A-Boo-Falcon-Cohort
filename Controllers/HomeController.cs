﻿using Bugaboo.Helpers;
using Bugaboo.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bugaboo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private NameHelper nhelper = new NameHelper();
        [AllowAnonymous]
        public ActionResult StartLogin()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult StartRegister()
        {
            return View();
        }
        [Authorize]
        public ActionResult Index()
        {
           
            ViewData.Add("FirstName", nhelper.GetUserName(User.Identity.GetUserId()));
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewData.Add("FirstName", nhelper.GetUserName(User.Identity.GetUserId()));
            ViewBag.Message = "Your contact page.";
            EmailModel model = new EmailModel();
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold> ({1})</p><p>Message:</p><p>{2}</p>";
                    var from = $"Bug-A-Boo<{model.FromEmail}>";
                    //model.Body = "This is a message from your blog site.  The name and the email of the contacting person is above.";
                    var email = new MailMessage(from, ConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = model.Subject,
                        Body = string.Format(body, model.FromName, model.FromEmail, model.Body),
                        IsBodyHtml = true
                    };
                    var svc = new PersonalEmailService();
                    await svc.SendAsync(email);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
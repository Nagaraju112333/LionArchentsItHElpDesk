using ArchentsIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using System.ComponentModel;

namespace ArchentsIT.Controllers
{
    public class AdminController : Controller
    {
        ArchnetsITHelpDesk db=new ArchnetsITHelpDesk();
        // GET: Admin
        [HttpGet]
        public ActionResult Testing()
        {
            return View();  
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Getallemployees(RaiseTicket rai )
        {

            var result=db.RaiseTickets.Where(x => x.Status== rai.Status.ToString()).ToList();
            var priarity = db.RaiseTickets.Where(x => x.priarity ==rai.priarity).ToList();
            var bothdetails=db.RaiseTickets.OrderBy(x=>x.Status==rai.Status).ThenBy(x=>x.priarity==rai.priarity).ToList();
            if (result.Count!=0)
            {
                TempData["StatusDetails"] = result;
                ViewBag.data2 = result;
                Session["data"] = result;
            }
            else if (bothdetails != null)
            {
                TempData["statusandpriarity"] = bothdetails;
            }
            else
            {

                TempData["Priaroty"] = priarity;
            }
              
            return RedirectToAction("Getallemployees", "Admin");
        }
        [HttpPost]
        public ActionResult StatusFilters(RaiseTicket tai)
        {
            return View();
        }
       

       
        [HttpGet]
        public ActionResult Getallemployees()
        {
          
            ViewBag.filter = TempData["StatusDetails"];

            ViewBag.priarity = TempData["Priaroty"];
            ViewBag.statusandpriarity = TempData["statusandpriarity"];
            Session["AdminLogin"] = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().FirstName;

            var result =db.RaiseTickets.ToList();    
            return View(result);
        }
        public class Status
        {
            public int id { get; set; }
            public string status { get; set; }
        }
        [HttpGet]
        public ActionResult Get(string id)
        {
            // RaiseTicket model = new RaiseTicket();


            List<Status> list = new List<Status>();
            list.Add(new Status { id = 101, status = "Open" });
            list.Add(new Status { id = 101, status = "Inprogress" });
            list.Add(new Status { id = 101, status = "Completed" });
            list.Add(new Status { id = 101, status = "Pending" });
            list.Add(new Status { id = 101, status = "Con't do" });
            ViewBag.CatList1 = new SelectList(list, "status", "status");
            var result = db.RaiseTickets.FirstOrDefault(x => x.TicketNo == id.ToString());
            result.UpdateDate = DateTime.Now.ToString();
            ViewBag.Updatedate = result.UpdateDate;
            return View(result);
        }
        [HttpPost]
        public ActionResult Get(RaiseTicket rai)
        {
            var result = db.RaiseTickets.FirstOrDefault(x => x.TicketNo == rai.TicketNo);

            if (rai != null)
            { 
                result.Status= rai.Status;
                result.Commant=rai.Commant;
                result.UpdateDate = rai.UpdateDate;
                db.SaveChanges();
                ViewBag.Status = rai.Status;
                ViewBag.commant = rai.Commant;
                SendVerificationLinkEmail(result.EmpEmail);
                
                TempData["message11"] = "Status Updated Successfully";
            }
            return RedirectToAction("Getallemployees", "Admin");
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailID)
        {
            
            var fromEmail = new MailAddress("Complaints.archents@outlook.com", emailID);
            var toEmail = new MailAddress(emailID);
            //var fromEmailPassword = "Nagaraju@123"; // Replace with actual password
            var fromEmailPassword = "V@11@pu6eddy$"; // Replace with actual password
            string subject = "";
            string body = "";
            subject = "Subject!"+ "Complaints Status";
            body = "<br/><br/> Hi  It Service Desk, <br/> Ticket Status " + "<b> " + ViewBag.Status + " " + " " + " " + " " + " " + ViewBag.commant + " "
            + "</a> ";


            // MailMessage mc = new MailMessage("nagaraju.bodaarchents.com@outlook.com", emailID);
            MailMessage mc = new MailMessage("Complaints.archents@outlook.com", emailID);
            mc.Subject = subject;
            mc.Body = body;
            mc.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.Timeout = 1000000;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            //NetworkCredential nc = new NetworkCredential("nagaraju.bodaarchents.com@outlook.com", "Nagaraju@123");
            NetworkCredential nc = new NetworkCredential("Complaints.archents@outlook.com", "V@11@pu6eddy$");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(mc);
        }
    }
}
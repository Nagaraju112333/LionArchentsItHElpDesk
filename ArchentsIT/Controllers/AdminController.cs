using ArchentsIT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ArchentsIT.Controllers
{
    public class AdminController : Controller
    {
        ArchentsITEntities5 db=new ArchentsITEntities5();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public class Status
        {
            public int id { get; set; }
            public string status { get; set;}
        }
        [HttpGet]
        public ActionResult Getallemployees()
        {
           
            var result=db.RaiseTickets.ToList();    
            return View(result);
        }
        [HttpGet]
        public ActionResult Get(int? id)
        {
            List<Status> list = new List<Status>();
            list.Add(new Status { id = 101, status = "Open" });
            list.Add(new Status { id = 101, status = "Inprogress" });
            list.Add(new Status { id = 101, status = "Completed" });
            list.Add(new Status { id = 101, status = "Pending" });
            list.Add(new Status { id = 101, status = "Con'do" });
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
                
                Session["message11"] = "Status Updated Successfully";
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


            //   MailMessage mc = new MailMessage("nagaraju.bodaarchents.com@outlook.com", emailID);
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
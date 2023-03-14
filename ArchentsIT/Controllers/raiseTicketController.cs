using ArchentsIT.Models;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static ArchentsIT.Controllers.AdminController;

namespace ArchentsIT.Controllers
{
    public class raiseTicketController : Controller
    {
      public class TypeOd
        {
            public int id { get; set; }
            public string Type { get; set; }
        }
        ArchnetsITHelpDesk db=new ArchnetsITHelpDesk();   
        // GET: raiseTicket
        public ActionResult Index()
        {
            return View();
        }
        public class Status
        {
            public int id { get; set; }
            public string status { get; set; }
        }
        public class prearity
        {
            public string priarity { get; set; }
        }
        [HttpGet]
        public ActionResult AddnewTicket()
        {
           // if (currentDate==10AM &&currentDate==10PM)

            ViewBag.Admin = "itsupport@archents.com";
         //   ViewBag.Admin = "itsupport@archents.com";
            List<prearity> List11 = new List<prearity>();
            List11.Add(new prearity {  priarity = "Low" });
            List11.Add(new prearity {  priarity = "Medium" });
            List11.Add(new prearity {  priarity = "High" });
            List11.Add(new prearity {  priarity = "Critical" });
            ViewBag.Priarity = new SelectList(List11, "priarity", "priarity");
            List<TypeOd> List=new List<TypeOd>();
            List.Add(new TypeOd { id = 45, Type = "New Laptop" });
            List.Add(new TypeOd { id = 45, Type = "Display Issue" });
            List.Add(new TypeOd { id = 45, Type = "Battery Issue " });
            List.Add(new TypeOd { id = 45, Type = "KeyBoard Bad Performance" });
            List.Add(new TypeOd { id = 45, Type = "Hardware Issue" });
            List.Add(new TypeOd { id = 45, Type = "SoftWare Instalation" });
            List.Add(new TypeOd { id = 45, Type = "Charger Issue" });
            
            List.Add(new TypeOd { id = 45, Type = "Others" });
            ViewBag.CatList = new SelectList(List, "Type", "Type");
            List<Status> list = new List<Status>();
            list.Add(new Status { id = 101, status = "Open" });
            list.Add(new Status { id = 101, status = "Inprogress" });
            list.Add(new Status { id = 101, status = "Completed" });
            list.Add(new Status { id = 101, status = "Pending" });
            list.Add(new Status { id = 101, status = "Unable to Process" });
            ViewBag.CatList1 = new SelectList(list, "status", "status");
            RaiseTicket r=new RaiseTicket();
            var result11 = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().EmpID;
            ViewBag.NewTicket = result11;
            Guid id= new Guid();
           // r.TicketNo =new Guid().ToString(); 
            return View();
        }
        [HttpPost]
        public ActionResult AddnewTicket(RaiseTicket ticket)
        {
                ViewBag.message = "";
                var currentDate = DateTime.Now;
                var dateonly = currentDate.TimeOfDay;
                var hour = currentDate.Hour;
                var minute = currentDate.Minute;
                var millisecond = currentDate.Millisecond;
                if (hour >= 10 && hour < 19)
                {
                    if (ticket != null)
                    {
                        var employeename = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().FirstName;
                        var EmplEmail = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().Email;
                        // it help desk email id
                        ViewBag.Admin = "itsupport@archents.com";
                        // madhu archents email
                        ViewBag.admin1 = "madhu.vallapureddy@archents.com";
                        int otpValue = new Random().Next(100000, 999999);
                        ticket.TicketNo = otpValue.ToString();
                        ticket.EmpName = employeename;
                        ticket.EmpEmail = EmplEmail;
                        ViewBag.prioriaty = ticket.priarity;
                        ticket.Status = "Open";
                        db.RaiseTickets.Add(ticket);
                        ViewBag.tiketId = ticket.TicketNo;
                        ViewBag.Reason = ticket.Issue_Type;
                        ticket.Date = DateTime.Now;
                        db.SaveChanges();
                        int EmailOtp = new Random().Next(100000, 999999);
                        SendVerificationLinkEmail(ticket.Assigned_Person);
                        if (ticket.Approval != null)
                        {
                            Approvaltolaptop(ticket.Approval);
                            approvalincc(ViewBag.admin1);
                        }
                        var message = "Registration successfully done. password " +
                             " has been sent to your email id:" + ticket.Assigned_Person;
                        /* Session["NewTicketCrated"] = "New ticket SuccessFully created";*/
                        TempData["NewTicketCrated"] = "New ticket SuccessFully created";
                    }
                    else
                    {
                        return RedirectToAction("GetEmployeeRecords", "UserRegister");
                    }
                }
                else
                {
                    var employeename = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().FirstName;
                    var EmplEmail = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().Email;
                    // it help desk email id
                    ViewBag.Admin = "itsupport@archents.com";
                    // madhu email id
                    ViewBag.admin1 = "madhu.vallapureddy@archents.com";
                    int otpValue = new Random().Next(100000, 999999);
                    ticket.TicketNo = otpValue.ToString();
                    ticket.EmpName = employeename;
                    ticket.EmpEmail = EmplEmail;
                    ViewBag.prioriaty = ticket.priarity;
                    ViewBag.Reason = ticket.Issue_Type;
                    ticket.Status = "Not Assigned";
                    db.RaiseTickets.Add(ticket);
                    ViewBag.tiketId = ticket.TicketNo;
                    ticket.Date = DateTime.Now;
                    db.SaveChanges();
                    int EmailOtp = new Random().Next(100000, 999999);
                    SendVerificationLinkEmail(ticket.Assigned_Person);
                    if (ticket.Approval != null)
                    {

                        Approvaltolaptop(ticket.Approval);
                        approvalincc(ViewBag.admin1);

                    }
                    var message = "Registration successfully done. password " +
                         " has been sent to your email id:" + ticket.Assigned_Person;
                    /* Session["NewTicketCrated"] = "New ticket SuccessFully created";*/
                    TempData["NewTicketCrated"] = "New ticket SuccessFully created";
                    /*TempData["ItNotAvailable"] = " Server Not Available  sent Tomorrow 10AM To 7PM.. ";*/
                }



                return RedirectToAction("GetEmployeeRecords", "UserRegister");


            
            return View();
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailID)
        {
            var EmployeeId= db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().EmpID;
            var employeeEmail = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().Email;
            var employeename = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().FirstName;

            // var verifyUrl = "/Account/" +"/" ;
            // var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            // var fromEmail = new MailAddress("nagaraju.bodaarchents.com@outlook.com", emailID);
            var fromEmail = new MailAddress("Complaints.archents@outlook.com", emailID);
            var toEmail = new MailAddress(emailID);
            //var fromEmailPassword = "Nagaraju@123"; 
            var fromEmailPassword = "V@11@pu6eddy$";
            string subject = "";
            string body = "";
            subject = "Subject!"+" "+ViewBag.tiketId +" "+"Is Raised";
            body = "<br/><br/> Hi  It Service Desk, <br/> Ticket no has been raised by "+"<b> "+ employeename+ " "+" "+" "+"</b>"+"with priority"+"<b>" +" "+" "+ViewBag.prioriaty+"</b>"+" "+" "+ "And Reason <b> " + ViewBag.Reason + "</b> "+" is assigned to you his" +" "+" "+" "+ " <b>Email ID:</b>" + employeeEmail+" "
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
        //approval to laptop
        public void Approvaltolaptop(string emailID)
        {
            var EmployeeId = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().EmpID;
            ViewBag.employeeID = EmployeeId;
            var employeeEmail = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().Email;
            var employeename = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().FirstName;

            // var verifyUrl = "/Account/" +"/" ;
            // var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            // var fromEmail = new MailAddress("nagaraju.bodaarchents.com@outlook.com", emailID);
            var fromEmail = new MailAddress("Complaints.archents@outlook.com", emailID);
            var toEmail = new MailAddress(emailID);
            //var fromEmailPassword = "Nagaraju@123"; 
            var fromEmailPassword = "V@11@pu6eddy$";
            string subject = "";
            string body = "";
            subject = "Approval! Needed for the laptop :" + ViewBag.tiketId;
            body = " <br/><br/> Hi, <br/>The ticket number: " + ViewBag.tiketId +" raised by "+ employeename + "("+ EmployeeId + ")  needs your approval  Please accept the request for further process  </a>";


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
        //approval for cc
        public void approvalincc(string emailID)
        {
            var EmployeeId = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().EmpID;
            ViewBag.employeeID = EmployeeId;
            var employeeEmail = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().Email;
            var employeename = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().FirstName;

            // var verifyUrl = "/Account/" +"/" ;
            // var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            // var fromEmail = new MailAddress("nagaraju.bodaarchents.com@outlook.com", emailID);
            var fromEmail = new MailAddress("Complaints.archents@outlook.com", emailID);
            var toEmail = new MailAddress(emailID);
            //var fromEmailPassword = "Nagaraju@123"; 
            var fromEmailPassword = "V@11@pu6eddy$";
            string subject = "";
            string body = "";
            subject = "Approval! Needed for the laptop :" + ViewBag.tiketId;
            body = "<br/><br/> Hi, <br/>The ticket number: " + ViewBag.tiketId + " raised by  " + employeename + "(" + EmployeeId + ")  needs your approval  Please accept the request for further process  </a>";


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
using ArchentsIT.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ArchentsIT.Controllers
{
    public class UserRegisterController : Controller
    {
        // GET: UserRegister
       ArchentsITEntities1 db=new ArchentsITEntities1();  
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult UserRegister()
        {
            return View();
        }
        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult UserRegister(UserRegister user)
        {
            bool Status = false;
            string message = "";
            //b
            // Model Validation 
            if (ModelState.IsValid)
            {
                #region //Email is already Exist 
                var isExist = IsEmailExist(user.Email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    ViewBag.IsEmailExist = "Email Is Already Exist";
                   // return View(user);
                }
                else
                {
                    user.Password = Guid.NewGuid().ToString();
                    //TempData["name"] = user.Password;

                    #endregion
                    #region Generate Activation Code 
                    //user.ActivationCode = Guid.NewGuid();
                    user.EmpID = Guid.NewGuid();
                    #endregion
                    #region  Password Hashing 
                    user.Password = Crypto.Hash(user.Password);
                    TempData["name"] = user.Password;
                    //   user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword); //
                    #endregion
                    // user.IsEmailVerified = false;
                    #region Save to Database
                    Status = true;
                    using (ArchentsITEntities1 dc = new ArchentsITEntities1())
                    {
                        user.registercount = 1;
                        dc.UserRegisters.Add(user);
                        dc.SaveChanges();

                        TempData["FirsttimeRegister"] = user.registercount;
                        //Send Email to User
                        SendVerificationLinkEmail(user.Email);
                        message = "Registration successfully done. password " +
                            " has been sent to your email id:" + user.Email; 
                        Status = true;
                        TempData["message"] = message;
                        TempData["Status"] = Status;
                        /* ViewBag.Message = message;
                         ViewBag.Status = Status;*/
                        return RedirectToAction("Login", "UserRegister");
                    }
                }
                #endregion
            }
            /* else
             {
                 message = "Invalid Request";
             }*/

            ViewBag.Message = message;
            ViewBag.Status = Status;
           return View(user);
          
        }
        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (ArchentsITEntities1 dc = new ArchentsITEntities1())
            {
                var v = dc.UserRegisters.Where(a => a.Email == emailID).FirstOrDefault();
                return v != null;
            }
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailID)
        {
           // var verifyUrl = "/Account/" +"/" ;
           // var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("nagaraju.bodaarchents.com@outlook.com", emailID);
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Nagaraju@123"; // Replace with actual password
            string subject = "";
            string body = "";
           
                subject = "Your account is successfully created!";
                body = "<br/><br/> Hello Nagaraju <br/> You have been invited for IT support protal for any it related issue please login using following password <br/> Your Password is:"+" "+""+" "
                + TempData["name"] + "" + "</a> ";
            
         
            MailMessage mc = new MailMessage("nagaraju.bodaarchents.com@outlook.com", emailID);
            mc.Subject = subject;
            mc.Body = body;
            mc.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.Timeout = 1000000;
            smtp.EnableSsl = true;

            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential nc = new NetworkCredential("nagaraju.bodaarchents.com@outlook.com", "Nagaraju@123");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(mc);

        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserRegister user)
        {
            string message = "";
            using (ArchentsITEntities1 dc = new ArchentsITEntities1())
            {
                var result = dc.UserRegisters.Where(a => a.Email == user.Email).FirstOrDefault();
                if (result != null)
                {

                    return RedirectToAction("Home", "Home");
                }
                else
                {
                    message = "Invalid credential provided";
                }


                ViewBag.Message = message;
                return View();
            }
        }
        [HttpGet]
        public ActionResult CretePassword()
        {
            return View();
        }


    }
}
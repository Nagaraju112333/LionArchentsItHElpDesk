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
using Microsoft.Ajax.Utilities;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Web.Security;
using Microsoft.AspNet.Identity;
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
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "UserRegister");
        }
        [HttpPost]
        public ActionResult Login(UserLogin user)
        {
            if (ModelState.IsValid)
            {
                string message = "";
                /* var result = db.UserRegisters.Where(a => a.Email == user.EmailID && a.Password == user.Password).FirstOrDefault();
                 if (result != null)
                 {*/
                using (ArchentsITEntities1 dc = new ArchentsITEntities1())
                {
                    var checkcrete = dc.UserRegisters.Where(x => x.Email == user.EmailID && x.Password == user.Password && x.registercount == 1).FirstOrDefault();

                    if (checkcrete != null)
                    {
                        ViewBag.data = checkcrete.Email;
                        TempData["data"] = checkcrete.Email;
                        TempData["data1"] = checkcrete;
                        return RedirectToAction("CretePassword", "UserRegister");
                    }
                    else
                    {
                        var getarecord = dc.UserRegisters.Where(a => a.Email == user.EmailID && a.Password == user.Password).FirstOrDefault();
                        if (getarecord != null)
                        {
                            FormsAuthentication.SetAuthCookie(user.EmailID, false);
                            return RedirectToAction("GetEmployeeRecords", "UserRegister");
                        }
                        else
                        {
                            message = "Invalid Username And Password";
                        }
                    }
                }
                /*  }
                      else
                      {
                          message = "Invalid credential provided";
                      }*/
                ViewBag.Message = message;
            }
            return View();
        }
        // Crete New Password
        [HttpGet]
        public ActionResult CretePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CretePassword(CreatePassword user)
        {
            var result22 = TempData["data"];
            var resul44 = TempData["data1"];
            var result = db.UserRegisters.Where(x => x.Email == result22).FirstOrDefault();
            if (result != null)
            {
                result.Password = user.NewPassword;
                result.registercount = null;
                db.SaveChanges();
                ViewBag.Successfull = "Password SuccessFully  Created Continue to Login...";
                return RedirectToAction("Login", "UserRegister");
            }
            return View();
        }
        // forgot password
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPassword password)
        {
            bool Status = false;
            string message1 = "";
            //  var result = db.UserRegisters.ToList();
            //var resltu = db.UserRegisters.FirstOrDefault(x => x.Email == .Select(x => x.Email).FirstOrDefault());
            var result = db.UserRegisters.Where(x => x.Email ==password.MobileNumberAndEmail || x.Phone_Number==password.MobileNumberAndEmail).FirstOrDefault();
            if (result != null)
            {
                var number = GetNumber(password.MobileNumberAndEmail);
                if (number)
                {
                    var otp = ViewBag.otp;
                    const string accountSid = "AC5b65569ae8d450277b340edc081cd925";
                    const string authToken = "b624391854339cf4aa75bd55ac2dabd8";
                    TwilioClient.Init(accountSid, authToken);
                    var to = new PhoneNumber("+91" + password.MobileNumberAndEmail);
                        var message = MessageResource.Create(
                        to,

                        from: new PhoneNumber("+19546377093"),
                        body: $"Verification OTP " + otp + "");
                        password.otp = otp;
                        result.Otp = otp;
                      db.SaveChanges();
                    ViewBag.message = "Otp Sent to yout mobile Number";
                }
                else
                {
                    int EmailOtp = new Random().Next(100000, 999999);
                    var Email = SendVerificationLinkEmail1(password.MobileNumberAndEmail);
                    message1 = "Registration successfully done. password " +
                            " has been sent to your email id:" + password.MobileNumberAndEmail;
                    Status = true;
                    TempData["message"] = message1;
                }
            }
            return View();
        }
        [NonAction]
        public bool SendVerificationLinkEmail1(string emailID)
        {
            int otpValue = new Random().Next(100000, 999999);
            ViewBag.otp=otpValue;
            // var verifyUrl = "/Account/" +"/" ;
            // var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress("nagaraju.bodaarchents.com@outlook.com", emailID);
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Nagaraju@123"; // Replace with actual password
            string subject = "";
            string body = "";

            subject = "OTP";
            body = "<br/><br/>  <br/> Your Password is:" + " " + "" + " "
               + otpValue + "" + "</a> ";


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
            return mc != null;
        }
        [NonAction]
        public bool GetNumber(string Number)
        {
            var getnumber = db.UserRegisters.Where(x => x.Phone_Number == Number).FirstOrDefault();
            return getnumber != null;
        }
        [HttpGet]
        public ActionResult GetEmployeeRecords()
        {

            var result11 = db.UserRegisters.Where(x => x.Email == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().EmpID;
            ViewBag.registerid = result11;
            // var result=db.UserRegisters.FirstOrDefault(x => x.Id == User.Identity.GetUserId());

            var result= db.RaiseTickets.ToList();
            if (result.Count == 0)
            {
                ViewBag.name = "No record found";
            }
            return View(result);
        }
        


    }
}
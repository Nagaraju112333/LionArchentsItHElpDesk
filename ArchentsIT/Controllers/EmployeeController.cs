using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArchentsIT.Controllers
{
    public class EmployeeController : Controller
    {
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult EmployeeRecords()
        {
            return View();
        }
    }
}
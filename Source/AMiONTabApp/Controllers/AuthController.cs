using AMiON.Helper.ConfigurationManagerHelper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMiONGraphShift.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        public ActionResult Index()
        {
            var accessToken = string.Empty; if (HttpContext.Request.Cookies.AllKeys.Contains("Auth")) accessToken = HttpContext.Request.Cookies["Auth"].Value;
            return View();
        }

        public ActionResult AmionTabSetup()
        {
            return View();
        }

        public ActionResult AmionAuthStart()
        {
            ViewBag.AuthClientId = AppSetting.AuthClientId; 
            ViewBag.Permission = ConfigurationManager.AppSettings["AuthScope"].ToString();
            ViewBag.RedirectUrl = ConfigurationManager.AppSettings["RedirectUrl"].ToString();
            return View();
        }

        public ActionResult AmionAuthEnd()
        {
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeAgent.Configuration;
using OfficeAgent.Data;
using System.Xml;
using OfficeAgent;
using System.Data;
using OfficeAgent.Cryption;

namespace CinemaWeb.Controllers
{
    public class HomeController : Controller
    {
        public DataSet dsUser = new DataSet();
        public static User UserData;

        #region LoginInfo Class Çağır

        LoginInfo _li;

        public LoginInfo UserInfo
        {
            get { return _li; }
        }

        #endregion

        //
        // GET: /Home/
        public ActionResult Index()
        {
            if (HttpContext.Application["counter"] == null)
            {
                HttpContext.Application["counter"] = 0;
            }

            HttpContext.Application["counter"] = Convert.ToInt16(HttpContext.Application["counter"]) + 1;

            ViewBag.counter = HttpContext.Application["counter"];

            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }
	}
}
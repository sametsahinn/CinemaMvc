using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using CinemaWeb.Controllers;

namespace Test.Controllers
{

    [TestClass]
    public class AccountControllerTest
    {
        [TestMethod]
        public void LoginTest()
        {
            AccountController dd = new AccountController();
            dd.Control("test", "Test"); //User Name & Pwd
            ViewResult result = dd.Control("test", "Test") as ViewResult;
            Assert.AreEqual("Hatalı kullanıcı adı veya parola !", result.TempData["error"]);
        }
    }
}

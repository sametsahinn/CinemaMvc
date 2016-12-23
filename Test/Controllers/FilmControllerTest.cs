using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using CinemaWeb.Controllers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Test.Controllers
{
    [TestClass]
    public class FilmControllerTest
    {
        FormCollection collection; 
        [TestMethod]
        public void FilmAddTest()
        {
            FilmController dd = new FilmController();
            dd.FilmDeleteAction(collection);   //FormCollection içinde FILMID değerini sakladığım button çinden verisini alıp sildiriyorum.
            ViewResult result = dd.FilmDeleteAction(collection) as ViewResult;
            Assert.AreEqual("Film Silindi.", result.TempData["success"]);
        }
    }
}

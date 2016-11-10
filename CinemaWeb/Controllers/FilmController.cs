using OfficeAgent.Cryption;
using OfficeAgent.Data;
using OfficeAgent.Object;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CinemaWeb.Controllers
{
    public class FilmController : Controller
    {
        public DataSet dsFilm = new DataSet();
        public DataSet dsFilmTyp = new DataSet();
        public DataSet dsHall = new DataSet();
               
        //
        // GET: /Film/
        public ActionResult Film()
        {
            using (DataVw dMan = new DataVw())
            {
                dsFilmTyp = dMan.ExecuteView_S("FILMTYP", "*", "", "", "");
                dsHall = dMan.ExecuteView_S("HALL", "*", "", "", "");
            }

            List<SelectListItem> filmList = new List<SelectListItem>();
            List<SelectListItem> hallList = new List<SelectListItem>();
            //List<SelectListItem> filmList = new List<SelectListItem>();
            foreach (DataRow dr in dsFilmTyp.Tables[0].Rows)
            {
                filmList.Add(new SelectListItem { Text = Convert.ToString(dr["FILMTYPNM"]), Value = dr["ID"].ToString() });
            }

            foreach (DataRow dr in dsHall.Tables[0].Rows)
            {
                hallList.Add(new SelectListItem { Text = Convert.ToString(dr["HALLNM"]), Value = dr["ID"].ToString() });
            }

            ViewBag.FilmList = filmList;
            ViewBag.HallList = hallList;

            return View();
        }

        public ActionResult FilmUpdate()
        {
            return View();
        }
        
        public ActionResult FilmDelete()
        {
            return View();
        }

        public ActionResult Hall()
        {
            return View();
        }

        public ActionResult HallUpdate()
        {
            return View();
        }

        public ActionResult HallDelete()
        {
            return View();
        }

        public ActionResult Category()
        {
            return View();
        }

        public ActionResult CategoryUpdate()
        {
            return View();
        }

        public ActionResult CategoryDelete()
        {
            return View();
        }


        //http://getbootstrap.com/javascript/
        //https://www.tutorialspoint.com/bootstrap/bootstrap_thumbnails.htm

        #region FilmAdd

        [HttpPost]
         public ActionResult FilmAdd(string txtFILMNM, string txtFILMTYPID, /*string txtHALLID,*/ string txtVISIONDATE, string txtFILMEXPLANATION, string txtFILMTIME, HttpPostedFileBase file, FormCollection collection)
        {  
            string filmID = collection["FilmList"];
            //string hallID = collection["HallList"];

            string filefo = "";
            using (DataVw dMan = new DataVw())
            {
                dsFilm = dMan.ExecuteView_S("FILM", "*", "", "", "");
            }

            if (txtFILMNM.ToString() == "" || txtVISIONDATE.ToString() == "" || filmID.ToString() == "" || txtFILMEXPLANATION.ToString() == "" || txtFILMTIME.ToString() == ""/*|| hallID.ToString() == ""*/)
            {
                Session["filmaddsuccess"] = false;
                ViewBag.addmessage = "Eksik veri girişi! Tüm Alanları Doldurunuz.";
                return Redirect("/Film/Film");
            }
            else
            {
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/images/film"), pic);
                    string pathd = "~/images/film/" + pic;
                    // file is uploaded
                    file.SaveAs(path);
                    filefo = pathd;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                }

                DataRow newrow = dsFilm.Tables[0].NewRow();
                newrow["ID"] = Guid.NewGuid();
                newrow["FILMNM"] = txtFILMNM;
                newrow["FILMTYPID"] = filmID;
                //newrow["HALLID"] = hallID;
                newrow["FILMIMG"] = filefo;
                newrow["VISIONDATE"] = Convert.ToDateTime(txtVISIONDATE);
                newrow["TIME"] = txtFILMTIME;
                newrow["EXPLANATION"] = txtFILMEXPLANATION;
                newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = null;
                //newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                newrow["NOTE"] = "En Son Kayıt İşlemi Gerçekleştirdi.";
                AgentGc data = new AgentGc();
                string veri = data.DataAdded("FILM", newrow, dsFilm.Tables[0]);
                Session["filmaddsuccess"] = true;
                ViewBag.addmessageinfo = veri;
                return Redirect("/Home/Admin");
            }
        }

        #endregion

        #region FilmTypAdd

        [HttpPost]
        public ActionResult FilmTypAdd(string txtFILMTYPNM)
        {
            //HomeController homeC = new HomeController();

            using (DataVw dMan = new DataVw())
            {
                dsFilmTyp = dMan.ExecuteView_S("FILMTYP", "*", "", "", "");
            }

            if (txtFILMTYPNM.ToString() == "")
            {
                Session["filmtypaddsuccess"] = false;
                ViewBag.addmessage = "Eksik veri girişi! Tüm Alanları Doldurunuz.";
                return Redirect("/Film/Category");
            }
            else
            {
                DataRow newrow = dsFilmTyp.Tables[0].NewRow();
                newrow["ID"] = Guid.NewGuid();
                newrow["FILMTYPNM"] = txtFILMTYPNM;
                newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = homeC.UserData.USRID.ToString();
                //newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                newrow["NOTE"] = "En Son Kayıt İşlemi Gerçekleştirdi.";
                AgentGc data = new AgentGc();
                string veri = data.DataAdded("FILMTYP", newrow, dsFilmTyp.Tables[0]);
                Session["filmtypaddsuccess"] = true;
                ViewBag.addmessageinfo = veri;
                return Redirect("/Home/Admin");
            }
        }

        #endregion

        #region HallAdd

        [HttpPost]
        public ActionResult HallAdd(string txtHALLNM)
        {
            //HomeController homeC = new HomeController();

            using (DataVw dMan = new DataVw())
            {
                dsHall = dMan.ExecuteView_S("HALL", "*", "", "", "");
            }

            if (txtHALLNM.ToString() == "")
            {
                Session["halladdsuccess"] = false;
                ViewBag.addmessage = "Eksik veri girişi! Tüm Alanları Doldurunuz.";
                return Redirect("/Film/Hall");
            }
            else
            {
                DataRow newrow = dsHall.Tables[0].NewRow();
                newrow["ID"] = Guid.NewGuid();
                newrow["HALLNM"] = txtHALLNM;
                newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = homeC.UserData.USRID.ToString();
                //newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                newrow["NOTE"] = "En Son Kayıt İşlemi Gerçekleştirdi.";
                AgentGc data = new AgentGc();
                string veri = data.DataAdded("HALL", newrow, dsHall.Tables[0]);
                Session["halladdsuccess"] = true;
                ViewBag.addmessageinfo = veri;
                return Redirect("/Home/Admin");
            }
        }

        #endregion
    }
}
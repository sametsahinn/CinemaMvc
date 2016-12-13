using OfficeAgent.Cryption;
using OfficeAgent.Data;
using OfficeAgent.Object;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CinemaWeb.Controllers
{
    public class FilmController : Controller
    {
        public DataSet dsFilm = new DataSet();
        public DataSet dsFilmTyp = new DataSet();
        public DataSet dsHall = new DataSet();
        public DataSet dsHallTime = new DataSet();
        public DataSet dsFilmInfo = new DataSet();
        public DataSet dsTicketInfo = new DataSet();
        public DataSet dsLogTable = new DataSet();
        public DataSet dsFilmD = new DataSet();
        public DataSet dsTicketPrice = new DataSet();

        #region Model

        public class FilmList
        {
            public Guid ID { get; set; }
            public string FILMNM { get; set; }
            public string FILMIMG { get; set; }
        }

        public class FilmTypList
        {
            public Guid ID { get; set; }
            public string FILMTYPNM { get; set; }
        }

        public class HallList
        {
            public Guid ID { get; set; }
            public string HALLNM { get; set; }
        }

        public class FilmTypV
        {
            public Guid ID { get; set; }
            public string FILMNM { get; set; }
            public Guid FILMTYPID { get; set; }
            public string FILMIMG { get; set; }
            public DateTime VISIONDATE { get; set; }
            public string TIME { get; set; }
            public string EXPLANATION { get; set; }
            public Guid F2ID { get; set; }
            public string FILMTYPNM { get; set; }
        }

        public class TicketInfo
        {
            public Guid ID { get; set; }
            public Guid FILMID { get; set; }
            public Guid HALLID { get; set; }
            public Guid HALLTIMEID { get; set; }
            public Guid SEATID { get; set; }
            public Guid USRID { get; set; }
            public DateTime DATETIME { get; set; }
            public Guid FID { get; set; }
            public string FILMNM { get; set; }
            public string FILMIMG { get; set; }
            //public Guid FILMTYPID { get; set; }
            public DateTime VISIONDATE { get; set; }
            public string TIME { get; set; }
            public string EXPLANATION { get; set; }
            public Guid HID { get; set; }
            public string HALLNM { get; set; }
            public Guid HTID { get; set; }
            public string HALLTIME { get; set; }
            public Guid SID { get; set; }
            public string SEATNM { get; set; }
            //public Guid SHALLID { get; set; }
            //public Boolean STATUS { get; set; }
            public Guid UID { get; set; }
            public string USRNM { get; set; }
            public string FULNM { get; set; }
            //public string PWD { get; set; }
            public string AVATAR { get; set; }
        }

        public class PriceList
        {
            public Guid ID { get; set; }
            public string PRICENM { get; set; }
            public string PRICE { get; set; }
        }
        
        #endregion

        #region Film Operations

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
        
        [HttpPost]
        public ActionResult FilmAdd(string txtFILMNM, string txtFILMTYPID, /*string txtHALLID,*/ string txtVISIONDATE, string txtFILMEXPLANATION, string txtFILMTIME, HttpPostedFileBase file, FormCollection collection)
        {
            string filmID = collection["FilmList"];
            //string hallID = collection["HallList"];

            string filefo = "";
            using (DataVw dMan = new DataVw())
            {
                dsFilm = dMan.ExecuteView_S("FILM", "*", "", "", "");
                dsLogTable = dMan.ExecuteView_S("LOGTABLE", "*", "", "", "");
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

                    //Image img = System.Drawing.Image.FromFile(pathd);
                    //Image img2 = ResizeImage(img, 190, 273);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        /////***********************************  Test Edilecek. ***********************************

                        //using (Image img = System.Drawing.Image.FromStream(ms))
                        //using (var newImage = ResizeImage(img, 190, 273))
                        //{
                        //    //Image img2 = ResizeImage(img, 190, 273);
                        //    newImage.Save(pathd, ImageFormat.Jpeg);
                        //}

                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                }

                DataRow newrow = dsFilm.Tables[0].NewRow();
                newrow["ID"] = Guid.NewGuid();
                string ID = newrow["ID"].ToString();
                newrow["FILMNM"] = txtFILMNM;
                newrow["FILMTYPID"] = filmID;
                //newrow["HALLID"] = hallID;
                newrow["FILMIMG"] = filefo;
                newrow["VISIONDATE"] = Convert.ToDateTime(txtVISIONDATE);
                newrow["TIME"] = txtFILMTIME;
                newrow["EXPLANATION"] = txtFILMEXPLANATION.Replace("'", "");
                newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = null;
                //newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                newrow["NOTE"] = "En Son Kayıt İşlemi Gerçekleştirdi.";
                AgentGc data = new AgentGc();
                string veri = data.DataAdded("FILM", newrow, dsFilm.Tables[0]);
                if (veri == "İşleminiz Tamamlandı..")
                {
                    DataRow _newrow = dsLogTable.Tables[0].NewRow();
                    _newrow["ID"] = Guid.NewGuid();
                    _newrow["LOGTABLE"] = "FILM";
                    _newrow["LOGTABLEID"] = ID;
                    _newrow["LOGIP"] = GetIp();
                    _newrow["LOGUSRID"] = Session["USRIDv"].ToString();
                    _newrow["EDATE"] = DateTime.Now;
                    //newrow["EUSRID"] = null;
                    _newrow["UDATE"] = DateTime.Now;
                    //newrow["UUSRID"] = null;
                    _newrow["NOTE"] = "Kayıt İşlemi.";
                    AgentGc _data = new AgentGc();
                    string _veri = _data.DataAdded("LOGTABLE", _newrow, dsLogTable.Tables[0]);
                    Session["filmaddsuccess"] = true;
                }
                ViewBag.addmessageinfo = veri;
                return Redirect("/Home/Admin");
            }
        }
        
        public ActionResult FilmUpdate()
        {
            using (DataVw dMan = new DataVw())
            {
                dsFilmInfo = dMan.ExecuteView_S("FILM_TYP_V", "*", "", "", "");
            }

            List<FilmTypV> FilmTypV = new List<FilmTypV>();
            foreach (DataRow dr in dsFilmInfo.Tables[0].Rows)
            {
                FilmTypV.Add(new FilmTypV
                {
                    ID = (Guid)dr["ID"],
                    FILMNM = dr["FILMNM"].ToString(),
                    FILMTYPID = (Guid)dr["FILMTYPID"],
                    FILMIMG = dr["FILMIMG"].ToString(),
                    VISIONDATE = (DateTime)dr["VISIONDATE"],
                    TIME = dr["TIME"].ToString(),
                    EXPLANATION = dr["EXPLANATION"].ToString().Substring(0, 230),
                    F2ID = (Guid)dr["F2ID"],
                    FILMTYPNM = dr["FILMTYPNM"].ToString(),
                });
            }

            ViewBag.FilmTypV = FilmTypV;

            return View();
        }
        public ActionResult FilmUpdateSelect(FormCollection collection)
        {
            string FILMID = collection["btnFilmID"];
            Session["FILMID"] = FILMID;

            return Redirect("/Film/FilmUpdateEnd");
        }

        public ActionResult FilmUpdateEnd()
        {
            string FILMID = Session["FILMID"].ToString();

            using (DataVw dMan = new DataVw())
            {
                dsFilmInfo = dMan.ExecuteView_S("FILM_TYP_V", "*", FILMID, "", "ID = ");
                dsFilmTyp = dMan.ExecuteView_S("FILMTYP", "*", "", "", "");        
            }

            List<SelectListItem> filmList = new List<SelectListItem>();
            foreach (DataRow dr in dsFilmTyp.Tables[0].Rows)
            {
                filmList.Add(new SelectListItem { Text = Convert.ToString(dr["FILMTYPNM"]), Value = dr["ID"].ToString() });
            }

            List<FilmTypV> FilmListSelect = new List<FilmTypV>();
            foreach (DataRow dr in dsFilmInfo.Tables[0].Rows)
            {
                FilmListSelect.Add(new FilmTypV
                {
                    ID = (Guid)dr["ID"],
                    FILMNM = dr["FILMNM"].ToString(),
                    FILMTYPID = (Guid)dr["FILMTYPID"],
                    FILMIMG = dr["FILMIMG"].ToString(),
                    VISIONDATE = (DateTime)dr["VISIONDATE"],
                    TIME = dr["TIME"].ToString(),
                    EXPLANATION = dr["EXPLANATION"].ToString().Substring(0, 230),
                    F2ID = (Guid)dr["F2ID"],
                    FILMTYPNM = dr["FILMTYPNM"].ToString(),
                });
            }

            ViewBag.FilmList = filmList;
            ViewBag.FilmListSelect = FilmListSelect;

            return View();
        }
        
        public ActionResult FilmUpdateEndAction(string txtFILMNM, string txtFILMTYPID, string txtVISIONDATE, string txtFILMEXPLANATION, string txtFILMTIME, HttpPostedFileBase file, FormCollection collection)
        {
            string FILMID = Session["FILMID"].ToString();
            string filmTypID = collection["FilmList"];
            string filefo = "";

            using (DataVw dMan = new DataVw())
            {
                dsFilm = dMan.ExecuteView_S("FILM", "*", FILMID, "", "ID = ");
                dsLogTable = dMan.ExecuteView_S("LOGTABLE", "*", "", "", "");
            }

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

            DataRow newrow = dsFilm.Tables[0].Rows[0];
            newrow["ID"] = FILMID;
            newrow["FILMNM"] = txtFILMNM;
            if (filmTypID == ""){
                newrow["FILMTYPID"] = newrow["FILMTYPID"];
            }else{
                newrow["FILMTYPID"] = filmTypID;
            }

            if (filefo == ""){
                newrow["FILMIMG"] = newrow["FILMIMG"];
            }else{
                newrow["FILMIMG"] = filefo;
            }            
            newrow["VISIONDATE"] = Convert.ToDateTime(txtVISIONDATE);
            newrow["TIME"] = txtFILMTIME;
            newrow["EXPLANATION"] = txtFILMEXPLANATION;
            //newrow["EDATE"] = DateTime.Now;
            //newrow["EUSRID"] = null;
            newrow["UDATE"] = DateTime.Now;
            //newrow["UUSRID"] = null;
            newrow["NOTE"] = "En Son Güncelleme İşlemi Gerçekleştirdi.";
            AgentGc data = new AgentGc();
            string veri = data.DataModified("FILM", newrow, dsFilm.Tables[0]);
            if (veri == "İşleminiz Tamamlandı..")
            {
                DataRow _newrow = dsLogTable.Tables[0].NewRow();
                _newrow["ID"] = Guid.NewGuid();
                _newrow["LOGTABLE"] = "FILM";
                _newrow["LOGTABLEID"] = FILMID;
                _newrow["LOGIP"] = GetIp();
                _newrow["LOGUSRID"] = Session["USRIDv"].ToString();
                _newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = null;
                _newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                _newrow["NOTE"] = "Güncelleme İşlemi.";
                AgentGc _data = new AgentGc();
                string _veri = _data.DataAdded("LOGTABLE", _newrow, dsLogTable.Tables[0]);
                Session["filmaddsuccess"] = true;
            }
            return Redirect("/Film/FilmUpdate");
        }
        
        public ActionResult FilmDelete()
        {
            using (DataVw dMan = new DataVw())
            {
                dsFilm = dMan.ExecuteView_S("FILM", "*", "", "", "");
            }

            List<FilmList> FilmList = new List<FilmList>();
            foreach (DataRow dr in dsFilm.Tables[0].Rows)
            {
                FilmList.Add(new FilmList { ID = (Guid)dr["ID"], FILMNM = dr["FILMNM"].ToString(), FILMIMG = dr["FILMIMG"].ToString() });
            }

            ViewBag.FilmList = FilmList;

            return View();
        }

        public ActionResult FilmDeleteAction(FormCollection collection)
        {
            string FILMID = collection["btnFilm"];

            string sql = string.Format("DELETE FROM FILM WHERE ID ='{0}'", FILMID);
            using (DataManager dMan = new DataManager())
            {
                int info = dMan.Excequte(sql);
            }

            return Redirect("/Film/FilmDelete");
        }

        #endregion

        #region Hall Operations

        public ActionResult Hall()
        {
            return View();
        }

        [HttpPost]
        public ActionResult HallAdd(string txtHALLNM)
        {
            //HomeController homeC = new HomeController();

            using (DataVw dMan = new DataVw())
            {
                dsHall = dMan.ExecuteView_S("HALL", "*", "", "", "");
                dsLogTable = dMan.ExecuteView_S("LOGTABLE", "*", "", "", "");
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
                string ID = newrow["ID"].ToString();
                newrow["HALLNM"] = txtHALLNM;
                newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = homeC.UserData.USRID.ToString();
                //newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                newrow["NOTE"] = "En Son Kayıt İşlemi Gerçekleştirdi.";
                AgentGc data = new AgentGc();
                string veri = data.DataAdded("HALL", newrow, dsHall.Tables[0]);
                if (veri == "İşleminiz Tamamlandı..")
                {
                    DataRow _newrow = dsLogTable.Tables[0].NewRow();
                    _newrow["ID"] = Guid.NewGuid();
                    _newrow["LOGTABLE"] = "HALL";
                    _newrow["LOGTABLEID"] = ID;
                    _newrow["LOGIP"] = GetIp();
                    _newrow["LOGUSRID"] = Session["USRIDv"].ToString();
                    _newrow["EDATE"] = DateTime.Now;
                    //newrow["EUSRID"] = null;
                    _newrow["UDATE"] = DateTime.Now;
                    //newrow["UUSRID"] = null;
                    _newrow["NOTE"] = "Kayıt İşlemi.";
                    AgentGc _data = new AgentGc();
                    string _veri = _data.DataAdded("LOGTABLE", _newrow, dsLogTable.Tables[0]);
                    Session["filmtypaddsuccess"] = true;
                }
                ViewBag.addmessageinfo = veri;
                return Redirect("/Home/Admin");
            }
        }

        public ActionResult HallUpdate()
        {
            using (DataVw dMan = new DataVw())
            {
                dsFilmTyp = dMan.ExecuteView_S("HALL", "*", "", "", "");
            }

            List<HallList> HallList = new List<HallList>();
            foreach (DataRow dr in dsFilmTyp.Tables[0].Rows)
            {
                HallList.Add(new HallList { ID = (Guid)dr["ID"], HALLNM = dr["HALLNM"].ToString() });
            }

            ViewBag.HallList = HallList;

            return View();
        }

        public ActionResult HallUpdateSelect(FormCollection collection)
        {
            string HALLID = collection["btnHallID"];
            Session["HALLID"] = HALLID;
            
            return Redirect("/Film/HallUpdateEnd");
        }

        public ActionResult HallUpdateEnd()
        {
            string HALLID = Session["HALLID"].ToString();

            using (DataVw dMan = new DataVw())
            {
                dsFilmTyp = dMan.ExecuteView_S("HALL", "*", HALLID, "", "ID = ");
            }

            List<HallList> HallListSelect = new List<HallList>();
            foreach (DataRow dr in dsFilmTyp.Tables[0].Rows)
            {
                HallListSelect.Add(new HallList { ID = (Guid)dr["ID"], HALLNM = dr["HALLNM"].ToString() });
            }

            ViewBag.HallListSelect = HallListSelect;

            return View();
        }

        public ActionResult HallUpdateEndAction(string txtHALLNM, FormCollection collection)
        {
            string HALLID = Session["HALLID"].ToString();

            using (DataVw dMan = new DataVw())
            {
                dsHall = dMan.ExecuteView_S("HALL", "*", HALLID, "", "ID = ");
                dsLogTable = dMan.ExecuteView_S("LOGTABLE", "*", "", "", "");
            }

            DataRow newrow = dsHall.Tables[0].Rows[0];
            newrow["ID"] = HALLID;
            newrow["HALLNM"] = txtHALLNM;
            //newrow["EDATE"] = DateTime.Now;
            //newrow["EUSRID"] = null;
            newrow["UDATE"] = DateTime.Now;
            //newrow["UUSRID"] = null;
            newrow["NOTE"] = "En Son Güncelleme İşlemi Gerçekleştirdi.";
            AgentGc data = new AgentGc();
            string veri = data.DataModified("HALL", newrow, dsHall.Tables[0]);
            if (veri == "İşleminiz Tamamlandı..")
            {
                DataRow _newrow = dsLogTable.Tables[0].NewRow();
                _newrow["ID"] = Guid.NewGuid();
                _newrow["LOGTABLE"] = "HALL";
                _newrow["LOGTABLEID"] = HALLID;
                _newrow["LOGIP"] = GetIp();
                _newrow["LOGUSRID"] = Session["USRIDv"].ToString();
                _newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = null;
                _newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                _newrow["NOTE"] = "Güncelleme İşlemi.";
                AgentGc _data = new AgentGc();
                string _veri = _data.DataAdded("LOGTABLE", _newrow, dsLogTable.Tables[0]);
                Session["filmaddsuccess"] = true;
            }
            return Redirect("/Film/HallUpdate");
        }

        public ActionResult HallDelete()
        {
            using (DataVw dMan = new DataVw())
            {
                dsHall = dMan.ExecuteView_S("HALL", "*", "", "", "");
            }

            List<HallList> HallList = new List<HallList>();
            foreach (DataRow dr in dsHall.Tables[0].Rows)
            {
                HallList.Add(new HallList { ID = (Guid)dr["ID"], HALLNM = dr["HALLNM"].ToString() });
            }

            ViewBag.HallList = HallList;

            return View();
        }

        public ActionResult HallDeleteAction(FormCollection collection)
        {
            string HALLID = collection["btnHall"];

            string sql = string.Format("DELETE FROM HALL WHERE ID ='{0}'", HALLID);
            using (DataManager dMan = new DataManager())
            {
                int info = dMan.Excequte(sql);
            }

            return Redirect("/Film/HallDelete");
        }

        #endregion

        #region Category Operations

        public ActionResult Category()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FilmTypAdd(string txtFILMTYPNM)
        {
            //HomeController homeC = new HomeController();

            using (DataVw dMan = new DataVw())
            {
                dsFilmTyp = dMan.ExecuteView_S("FILMTYP", "*", "", "", "");
                dsLogTable = dMan.ExecuteView_S("LOGTABLE", "*", "", "", "");
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
                string ID = newrow["ID"].ToString();
                newrow["FILMTYPNM"] = txtFILMTYPNM;
                newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = homeC.UserData.USRID.ToString();
                //newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                newrow["NOTE"] = "En Son Kayıt İşlemi Gerçekleştirdi.";
                AgentGc data = new AgentGc();
                string veri = data.DataAdded("FILMTYP", newrow, dsFilmTyp.Tables[0]);
                if (veri == "İşleminiz Tamamlandı..")
                {
                    DataRow _newrow = dsLogTable.Tables[0].NewRow();
                    _newrow["ID"] = Guid.NewGuid();
                    _newrow["LOGTABLE"] = "FILMTYP";
                    _newrow["LOGTABLEID"] = ID;
                    _newrow["LOGIP"] = GetIp();
                    _newrow["LOGUSRID"] = Session["USRIDv"].ToString();
                    _newrow["EDATE"] = DateTime.Now;
                    //newrow["EUSRID"] = null;
                    _newrow["UDATE"] = DateTime.Now;
                    //newrow["UUSRID"] = null;
                    _newrow["NOTE"] = "Kayıt İşlemi.";
                    AgentGc _data = new AgentGc();
                    string _veri = _data.DataAdded("LOGTABLE", _newrow, dsLogTable.Tables[0]);
                    Session["filmtypaddsuccess"] = true;
                }
                ViewBag.addmessageinfo = veri;
                return Redirect("/Home/Admin");
            }
        }

        public ActionResult CategoryUpdate()
        {
            using (DataVw dMan = new DataVw())
            {
                dsFilmTyp = dMan.ExecuteView_S("FILMTYP", "*", "", "", "");
            }

            List<FilmTypList> FilmTypList = new List<FilmTypList>();
            foreach (DataRow dr in dsFilmTyp.Tables[0].Rows)
            {
                FilmTypList.Add(new FilmTypList { ID = (Guid)dr["ID"], FILMTYPNM = dr["FILMTYPNM"].ToString() });
            }

            ViewBag.FilmTypList = FilmTypList;

            return View();
        }

        public ActionResult CategoryUpdateSelect(FormCollection collection)
        {
            string CATEGOTYID = collection["btnCategory"];
            Session["CATEGOTYID"] = CATEGOTYID;
            
            return Redirect("/Film/CategoryUpdateEnd");
        }

        public ActionResult CategoryUpdateEnd() 
        {
            string CATEGOTYID = Session["CATEGOTYID"].ToString();

            using (DataVw dMan = new DataVw())
            {
                dsFilmTyp = dMan.ExecuteView_S("FILMTYP", "*", CATEGOTYID, "", "ID = ");
            }

            List<FilmTypList> FilmTypListSelect = new List<FilmTypList>();
            foreach (DataRow dr in dsFilmTyp.Tables[0].Rows)
            {
                FilmTypListSelect.Add(new FilmTypList { ID = (Guid)dr["ID"], FILMTYPNM = dr["FILMTYPNM"].ToString() });
            }

            ViewBag.FilmTypListSelect = FilmTypListSelect;

            return View();
        }

        public ActionResult CategoryUpdateEndAction(string txtFILMTYPNM, FormCollection collection)
        {
            string C = collection["btnCatID"];
            string CATEGOTYID = Session["CATEGOTYID"].ToString();

            using (DataVw dMan = new DataVw())
            {
                dsFilmTyp = dMan.ExecuteView_S("FILMTYP", "*", CATEGOTYID, "", "ID = ");
                dsLogTable = dMan.ExecuteView_S("LOGTABLE", "*", "", "", "");
            }

            DataRow newrow = dsFilmTyp.Tables[0].Rows[0];
            newrow["ID"] = CATEGOTYID;
            newrow["FILMTYPNM"] = txtFILMTYPNM;
            //newrow["EDATE"] = DateTime.Now;
            //newrow["EUSRID"] = null;
            newrow["UDATE"] = DateTime.Now;
            //newrow["UUSRID"] = null;
            newrow["NOTE"] = "En Son Güncelleme İşlemi Gerçekleştirdi.";
            AgentGc data = new AgentGc();
            string veri = data.DataModified("FILMTYP", newrow, dsFilmTyp.Tables[0]);
            if (veri == "İşleminiz Tamamlandı..")
            {
                DataRow _newrow = dsLogTable.Tables[0].NewRow();
                _newrow["ID"] = Guid.NewGuid();
                _newrow["LOGTABLE"] = "FILMTYP";
                _newrow["LOGTABLEID"] = CATEGOTYID;
                _newrow["LOGIP"] = GetIp();
                _newrow["LOGUSRID"] = Session["USRIDv"].ToString();
                _newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = null;
                _newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                _newrow["NOTE"] = "Güncelleme İşlemi.";
                AgentGc _data = new AgentGc();
                string _veri = _data.DataAdded("LOGTABLE", _newrow, dsLogTable.Tables[0]);
                Session["filmaddsuccess"] = true;
            }
            return Redirect("/Film/CategoryUpdate");
        }

        public ActionResult CategoryDelete()
        {
            using (DataVw dMan = new DataVw())
            {
                dsFilmTyp = dMan.ExecuteView_S("FILMTYP", "*", "", "", "");
            }

            List<FilmTypList> FilmTypList = new List<FilmTypList>();
            foreach (DataRow dr in dsFilmTyp.Tables[0].Rows)
            {
                FilmTypList.Add(new FilmTypList { ID = (Guid)dr["ID"], FILMTYPNM = dr["FILMTYPNM"].ToString() });
            }

            ViewBag.FilmTypList = FilmTypList;

            return View();
        }

        public ActionResult CategoryDeleteAction(FormCollection collection)
        {
            string CATEGOTYID = collection["btnCategory"];
            //using (DataVw dMan = new DataVw())
            //{
            //    dsFilmTyp = dMan.ExecuteView_S("FILMTYP", "*", "", "", "");
            //}
            string sql = string.Format("DELETE FROM FILMTYP WHERE ID ='{0}'", CATEGOTYID);
           
            using (DataManager dMan = new DataManager())
            {
                int info = dMan.Excequte(sql);
            }
            
            //DataRow[] dr = dsFilmTyp.Tables[0].Select(string.Format("ID='{0}'", CATEGOTYID));
            //if (dr.Length == 1)
            //{
            //    DataRow drDelete = dr[0];
            //    //dr[0].Delete();

            //    AgentGc data = new AgentGc();
            //    string veri = data.DataDeleted("FILMTYP", drDelete, dsFilmTyp.Tables[0]);
            //}

            return Redirect("/Film/CategoryDelete");
        }

        #endregion

        #region PriceUpdate

        public ActionResult PriceUpdate()
        {
            using (DataVw dMan = new DataVw())
            {
                dsTicketPrice = dMan.ExecuteView_S("PRICES", "*", "", "", "");
            }
            
            List<PriceList> priceList = new List<PriceList>();
            foreach (DataRow dr in dsTicketPrice.Tables[0].Rows)
            {
                priceList.Add(new PriceList{ 
                                    ID = (Guid)dr["ID"],
                                    PRICENM = dr["PRICENM"].ToString(),
                                    PRICE = dr["PRICE"].ToString(),

                });
            }

            ViewBag.PriceList = priceList;

            return View();
        }

        public ActionResult PriceUpdateSelect(FormCollection collection)
        {
            string PRICEID = collection["btnCategory"];
            Session["PRICEID"] = PRICEID;

            return Redirect("/Film/PriceUpdateEnd");
        }

        public ActionResult PriceUpdateEnd()
        {
            string PRICEID = Session["PRICEID"].ToString();

            using (DataVw dMan = new DataVw())
            {
                dsTicketPrice = dMan.ExecuteView_S("PRICES", "*", PRICEID, "", "ID = ");
            }

            List<PriceList> priceList = new List<PriceList>();
            foreach (DataRow dr in dsTicketPrice.Tables[0].Rows)
            {
                priceList.Add(new PriceList
                {
                    ID = (Guid)dr["ID"],
                    PRICENM = dr["PRICENM"].ToString(),
                    PRICE = dr["PRICE"].ToString(),

                });
            }

            ViewBag.PriceList = priceList;

            return View();
        }

        public ActionResult PriceUpdateEndAction(string txtPRICENM, string txtPRICE, FormCollection collection)
        {
            string C = collection["btnCatID"];
            string PRICEID = Session["PRICEID"].ToString();

            using (DataVw dMan = new DataVw())
            {
                dsTicketPrice = dMan.ExecuteView_S("PRICES", "*", PRICEID, "", "ID = ");
                dsLogTable = dMan.ExecuteView_S("LOGTABLE", "*", "", "", "");
            }

            DataRow newrow = dsTicketPrice.Tables[0].Rows[0];
            newrow["ID"] = PRICEID;
            newrow["PRICENM"] = txtPRICENM;
            newrow["PRICE"] = txtPRICE;
            //newrow["EDATE"] = DateTime.Now;
            //newrow["EUSRID"] = null;
            newrow["UDATE"] = DateTime.Now;
            //newrow["UUSRID"] = null;
            newrow["NOTE"] = "En Son Güncelleme İşlemi Gerçekleştirdi.";
            AgentGc data = new AgentGc();
            string veri = data.DataModified("PRICES", newrow, dsTicketPrice.Tables[0]);
            if (veri == "İşleminiz Tamamlandı..")
            {
                DataRow _newrow = dsLogTable.Tables[0].NewRow();
                _newrow["ID"] = Guid.NewGuid();
                _newrow["LOGTABLE"] = "PRICES";
                _newrow["LOGTABLEID"] = PRICEID;
                _newrow["LOGIP"] = GetIp();
                _newrow["LOGUSRID"] = Session["USRIDv"].ToString();
                _newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = null;
                _newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                _newrow["NOTE"] = "Güncelleme İşlemi.";
                AgentGc _data = new AgentGc();
                string _veri = _data.DataAdded("LOGTABLE", _newrow, dsLogTable.Tables[0]);
                Session["filmaddsuccess"] = true;
            }
            
            return Redirect("/Film/PriceUpdate");
        }

        #endregion

        #region FilmTicketProcess

        public ActionResult FilmTicketProcess()
        {
            using (DataVw dMan = new DataVw())
            {
                dsTicketInfo = dMan.ExecuteView_S("TICKETINFO_V WHERE STATUS = 1", "*", "", "", "");
            }

            List<TicketInfo> ticketInfoList = new List<TicketInfo>();
            foreach (DataRow dr in dsTicketInfo.Tables[0].Rows)
            {
                ticketInfoList.Add(new TicketInfo { 
                        ID = (Guid)dr["ID"], 
                        FILMID = (Guid)dr["FILMID"], 
                        HALLID = (Guid)dr["HALLID"],
                        HALLTIMEID = (Guid)dr["HALLTIMEID"],
                        SEATID = (Guid)dr["SEATID"],
                        USRID = (Guid)dr["USRID"],
                        DATETIME = (DateTime)dr["DATETIME"],
                        FID = (Guid)dr["FID"],
                        FILMNM = dr["FILMNM"].ToString(),
                        FILMIMG = dr["FILMIMG"].ToString(),
                        VISIONDATE = (DateTime)dr["VISIONDATE"],
                        TIME = dr["TIME"].ToString(),
                        EXPLANATION = dr["EXPLANATION"].ToString(),
                        HID = (Guid)dr["HID"],
                        HALLNM = dr["HALLNM"].ToString(),
                        HTID = (Guid)dr["HTID"],
                        HALLTIME = dr["HALLTIME"].ToString(),
                        SID = (Guid)dr["SID"],
                        SEATNM = dr["SEATNM"].ToString(),
                        UID = (Guid)dr["UID"],
                        USRNM = dr["USRNM"].ToString(),
                        FULNM = dr["FULNM"].ToString(),
                        AVATAR = dr["AVATAR"].ToString()
                });
            }
            
            ViewBag.TicketInfoList = ticketInfoList;

            return View();
        }

        #endregion

        #region Seance

        public ActionResult Seance()
        {
            using (DataVw dMan = new DataVw())
            {
                dsFilm = dMan.ExecuteView_S("FILM", "*", "", "", "");
                dsHall = dMan.ExecuteView_S("HALL", "*", "", "", "");
                dsHallTime = dMan.ExecuteView_S("HALLTIME", "*", "", "", "");
            }

            List<SelectListItem> filmList = new List<SelectListItem>();
            List<SelectListItem> hallList = new List<SelectListItem>();
            List<SelectListItem> hallTimeList = new List<SelectListItem>();
            foreach (DataRow dr in dsFilm.Tables[0].Rows)
            {
                filmList.Add(new SelectListItem { Text = Convert.ToString(dr["FILMNM"]), Value = dr["ID"].ToString() });
            }

            foreach (DataRow dr in dsHall.Tables[0].Rows)
            {
                hallList.Add(new SelectListItem { Text = Convert.ToString(dr["HALLNM"]), Value = dr["ID"].ToString() });
            }

            foreach (DataRow dr in dsHallTime.Tables[0].Rows)
            {
                hallTimeList.Add(new SelectListItem { Text = Convert.ToString(dr["HALLTIME"]), Value = dr["ID"].ToString() });
            }

            ViewBag.FilmList = filmList;
            ViewBag.HallList = hallList;
            ViewBag.HallTimeList = hallTimeList;

            return View();
        }

        public ActionResult SeanceAdd(FormCollection collection)
        {
            string FILMID = collection["FilmList"];
            string HALLID = collection["HallList"];
            string HALLTIMEID = collection["HallTimeList"];

            using (DataVw dMan = new DataVw())
            {
                dsFilmD = dMan.ExecuteView_S("FILM_D", "*", "", "", "");
            }

            DataRow newrow = dsFilmD.Tables[0].NewRow();
            newrow["ID"] = Guid.NewGuid();
            newrow["FILMID"] = FILMID;
            newrow["HALLID"] = HALLID;
            newrow["HALLTIMEID"] = HALLTIMEID;
            newrow["EDATE"] = DateTime.Now;
            //newrow["EUSRID"] = null;
            //newrow["UDATE"] = DateTime.Now;
            //newrow["UUSRID"] = null;
            newrow["NOTE"] = "En Son Kayıt İşlemi Gerçekleştirdi.";
            AgentGc data = new AgentGc();
            string veri = data.DataAdded("FILM_D", newrow, dsFilmD.Tables[0]);

            return Redirect("/Home/Admin");
        }

        #endregion

        #region ResizeImage Func

        public static Image ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        #endregion

        #region GetIp

        public string GetIp()
        {
            var strHostName = "";
            strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            var addr = ipEntry.AddressList;
            return addr[0].ToString();
        }

        #endregion

        //http://getbootstrap.com/javascript/
        //https://www.tutorialspoint.com/bootstrap/bootstrap_thumbnails.htm
    }
}
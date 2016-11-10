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
using OfficeAgent.Object;
using System.Net.Mail;
using System.Text;

namespace CinemaWeb.Controllers
{
    public class HomeController : Controller
    {
        //public DataSet dsUser = new DataSet();
        public DataSet dsFilm = new DataSet();
        public DataSet dsFilmSoon = new DataSet();
        public string FILMID;
        Mail _Mail = new Mail();
        MailHelper _MailH = new MailHelper();

        #region LoginInfo Class Çağır

        LoginInfo _li;

        public LoginInfo UserInfo
        {
            get { return _li; }
        }

        #endregion

        #region Models

        public class FilmD 
        {
            public Guid ID { get; set; }
            public string FILMNM { get; set; }
            public Guid FILMTYPID { get; set; }
            public string FILMIMG { get; set; }
            public DateTime VISIONDATE { get; set; }
            public string TIME { get; set; }
            public string EXPLANATION { get; set; }
        }

        public class FilmInfo
        {
            public Guid FID { get; set; }
            public Guid FFILMID { get; set; }
            public Guid FHALLID { get; set; }
            public Guid FFID { get; set; }
            public Guid HID { get; set; }
            public string HALLNM { get; set; }
            public Guid H2ID { get; set; }
            public string HALLTIME { get; set; }
        }

        public class FilmSeat
        {
            public Guid ID { get; set; }
            public string SEATNM { get; set; }
            public Guid HALLID { get; set; }
            public bool STATUS { get; set; }
        }

        public class UserList
        {
            public Guid ID { get; set; }
            public string USRNM { get; set; }
            public string PWD { get; set; }
            public string FULNM { get; set; }
            public string EMAIL { get; set; }
            public string CARDNO { get; set; }
            public string CVC { get; set; }
            public string STKDAY { get; set; }
            public string STKMONTH { get; set; }
        }

        public class FilmGroup
        {
            public string Key { get; set; }
        }

        public class Group<T, K>
        {
            public K Key;
            public IEnumerable<T> Values;
        }

        #endregion

        #region Index

        //
        // GET: /Home/
        public ActionResult Index(IEnumerable<FilmD> film)
        {

            //Session["USRID"] = "";

            using (DataVw dMan = new DataVw())
            {
                dsFilm = dMan.ExecuteView_S("FILM WHERE VISIONDATE <= GETDATE() AND DAY(VISIONDATE) <= 30 AND MONTH(VISIONDATE) = MONTH(GETDATE())", "*", "", "", "");
                dsFilmSoon = dMan.ExecuteView_S("FILM WHERE VISIONDATE >= GETDATE() AND DAY(VISIONDATE) <= 30", "*", "", "", "");
            }

            FilmD s = new FilmD();
            List<FilmD> datatable = new List<FilmD>();
            foreach (DataRow dr in dsFilm.Tables[0].Rows)
            {
                datatable.Add(new FilmD {  ID = (Guid)dr["ID"],
                                           FILMNM = dr["FILMNM"].ToString(),
                                           FILMTYPID = (Guid)dr["FILMTYPID"],
                                           FILMIMG = dr["FILMIMG"].ToString(),
                                           VISIONDATE = (DateTime)dr["VISIONDATE"], 
                                           TIME = dr["TIME"].ToString(),
                                           EXPLANATION = dr["EXPLANATION"].ToString().Substring(0, 230) });
            }

            List<FilmD> filmSoon = new List<FilmD>();
            foreach (DataRow dr in dsFilmSoon.Tables[0].Rows)
            {
                filmSoon.Add(new FilmD
                {
                    ID = (Guid)dr["ID"],
                    FILMNM = dr["FILMNM"].ToString(),
                    FILMTYPID = (Guid)dr["FILMTYPID"],
                    FILMIMG = dr["FILMIMG"].ToString(),
                    VISIONDATE = (DateTime)dr["VISIONDATE"],
                    TIME = dr["TIME"].ToString(),
                    EXPLANATION = dr["EXPLANATION"].ToString().Substring(0, 230)
                });
            }

            ViewBag.FilmListD = datatable;
            ViewBag.FilmSoon = filmSoon;

            if (HttpContext.Application["counter"] == null)
            {
                HttpContext.Application["counter"] = 0;
            }

            HttpContext.Application["counter"] = Convert.ToInt16(HttpContext.Application["counter"]) + 1;

            ViewBag.counter = HttpContext.Application["counter"];

            return View(datatable);
        }

        #endregion

        public ActionResult Admin()
        {
            return View();
        }

        #region Filminfo 

        public ActionResult Filminfo()
        {
            string IdValue = Session["FILMID"].ToString();
            DataSet dsFilmD = new DataSet();            
            DataSet dsFilmInfo = new DataSet();

            using (DataVw dMan = new DataVw())
            {
                dsFilmD = dMan.ExecuteView_S("FILM", "*", IdValue, "", "ID = ");                
                dsFilmInfo = dMan.ExecuteView_S("FILMINFO", "*", IdValue, "", "FILMID = ");
            }

            FilmD s = new FilmD();
            List<FilmD> filmDe = new List<FilmD>();
            foreach (DataRow dr in dsFilmD.Tables[0].Rows)
            {
                filmDe.Add(new FilmD
                {
                    ID = (Guid)dr["ID"],
                    FILMNM = dr["FILMNM"].ToString(),
                    FILMTYPID = (Guid)dr["FILMTYPID"],
                    FILMIMG = dr["FILMIMG"].ToString(),
                    VISIONDATE = (DateTime)dr["VISIONDATE"],
                    TIME = dr["TIME"].ToString(),
                    EXPLANATION = dr["EXPLANATION"].ToString().Substring(0, 230)
                });
            }

            List<FilmInfo> filmInfo = new List<FilmInfo>();
            foreach (DataRow dr in dsFilmInfo.Tables[0].Rows)
            {
                filmInfo.Add(new FilmInfo
                {
                    FID = (Guid)dr["ID"],
                    FFILMID = (Guid)dr["FILMID"],
                    FHALLID = (Guid)dr["HALLID"],
                    FFID = (Guid)dr["FID"],
                    HID = (Guid)dr["HID"],
                    HALLNM = dr["HALLNM"].ToString(),
                    H2ID = (Guid)dr["H2ID"],
                    HALLTIME = dr["HALLTIME"].ToString()
                });
            }

            //var filmHallGroup = from flm in filmInfo
            //                    group flm by flm.HALLNM;

            //var filmHallGroup = filmInfo.GroupBy(flm => flm.HALLNM[0]);
           
            var filmHallGroup = filmInfo.GroupBy(flm => flm.HALLNM).Select(g => new FilmGroup
            {
                Key = g.Key
                //Tag = g
            });

            ViewBag.FilmListDe = filmDe;
            ViewBag.FilmListInfo = filmInfo;
            ViewBag.FilmHallGroup = filmHallGroup;


            return View(filmDe);
        } 
                                 
        [HttpPost]
        public ActionResult FilmTicket(FormCollection collection)
        {
            string filmID = collection.AllKeys[0].ToString();
            Session["FILMID"] = filmID;
            
            return Redirect("/Home/Filminfo");
        }

        #endregion

        #region FilmSeat

        public ActionResult Filmseat()
        {
            string HALLIDStr = Session["HALLIDV"].ToString();
            string HALLTIMEIDStr = Session["HALLTIMEIDV"].ToString();

            string IdValue = Session["FILMID"].ToString();
            DataSet dsFilmD = new DataSet();
            DataSet dsFilmInfo = new DataSet();
            DataSet dsFilmSeat = new DataSet();

            using (DataVw dMan = new DataVw())
            {
                dsFilmD = dMan.ExecuteView_S("FILM", "*", IdValue, "", "ID = ");
                dsFilmInfo = dMan.ExecuteView_S("FILMINFO", "*", IdValue, "", "FILMID = ");
                dsFilmSeat = dMan.ExecuteView_S("SEAT", "*", HALLIDStr, "", "HALLID = ");
            }

            FilmSeat s = new FilmSeat();
            List<FilmSeat> filmSeat = new List<FilmSeat>();
            foreach (DataRow dr in dsFilmSeat.Tables[0].Rows)
            {
                filmSeat.Add(new FilmSeat
                {
                    ID = (Guid)dr["ID"],
                    SEATNM = dr["SEATNM"].ToString(),
                    HALLID = (Guid)dr["HALLID"],
                    STATUS = Convert.ToBoolean(dr["STATUS"].ToString())
                });
            }

            List<FilmSeat> filmSeatSorted = filmSeat.OrderBy(o => o.SEATNM).ToList();
            
            //filmSeat.Sort();

            ViewBag.FilmSeatList = filmSeatSorted;

            return View(filmSeat);
        }

        [HttpPost]
        public ActionResult HallTimeSelect(FormCollection collection)
        {
            string HALLIDV = collection[0].ToString(); //Salon
            string HALLTIMEIDV = collection.AllKeys[0].ToString();  //Saat

            Session["HALLIDV"] = HALLIDV;
            Session["HALLTIMEIDV"] = HALLTIMEIDV;

            return Redirect("/Home/Filmseat");
        }

        #endregion

        #region FilmTicket 

        public ActionResult Filmticket()
        {
            string USRID = Session["USRIDv"].ToString();
            string IdValue = Session["FILMID"].ToString();
            DataSet dsFilmVal = new DataSet();
            DataSet dsUSR = new DataSet();

            using (DataVw dMan = new DataVw())
            {
                dsFilmVal = dMan.ExecuteView_S("FILM", "*", IdValue, "", "ID = ");
                dsUSR = dMan.ExecuteView_S("USR", "*", USRID, "", "ID = ");
            }

            List<FilmD> filmDe = new List<FilmD>();
            foreach (DataRow dr in dsFilmVal.Tables[0].Rows)
            {
                filmDe.Add(new FilmD
                {
                    ID = (Guid)dr["ID"],
                    FILMNM = dr["FILMNM"].ToString(),
                    FILMTYPID = (Guid)dr["FILMTYPID"],
                    FILMIMG = dr["FILMIMG"].ToString(),
                    VISIONDATE = (DateTime)dr["VISIONDATE"],
                    TIME = dr["TIME"].ToString(),
                    EXPLANATION = dr["EXPLANATION"].ToString().Substring(0, 230)
                });
            }

            List<UserList> userList = new List<UserList>();
            foreach (DataRow dr in dsUSR.Tables[0].Rows)
            {
                userList.Add(new UserList
                {
                    ID = (Guid)dr["ID"],
                    USRNM = dr["USRNM"].ToString(),
                    PWD = dr["PWD"].ToString(),
                    EMAIL = dr["EMAIL"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    CARDNO = dr["CARDNO"].ToString(),
                    CVC = dr["CVC"].ToString(),
                    STKDAY = dr["STKDAY"].ToString(),
                    STKMONTH = dr["STKMONTH"].ToString()
                });
            }

            ViewBag.SelectFilm = filmDe;
            ViewBag.LogUser = userList;

            return View();
        }

        [HttpPost]
        //[AllowAnonymous]
        public ActionResult FilmticketFunc(FormCollection collection)
        {
            if (Convert.ToBoolean(Session["IsAuthenticated"]))
            {
                string SEATID = collection[0].ToString(); //Salon
                //string HALLTIMEIDV = collection.AllKeys[0].ToString();  //Saat

                Session["SEATID"] = SEATID;

                return Redirect("/Home/Filmticket");
            }
            else
            {
                return Redirect("/Account/Login");
            }
            
        }

        [HttpPost]
        public ActionResult FilmticketFuncEnd(string txtFULNM, string txtEMAIL, string txtCARDNO, string txtCVC, string txtSTKDAY)
        {
            Session["ISSENDMAIL"] = false;
            //string ToEmailStr = collection[0].ToString();
            string ToEmailStr = txtEMAIL;
            //string FILMID = Session["FILMID"].ToString();
            string FILMNM = GetFILMNM(Session["FILMID"].ToString());
            string FILMIMG = GetFILMIMG(Session["FILMID"].ToString());
            //string HALLID = Session["HALLIDV"].ToString();
            string HALLNM = GetHALLNM(Session["HALLIDV"].ToString());
            //string HALLTIMEID = Session["HALLTIMEIDV"].ToString();
            string HALLTIME = GetHALLTIME(Session["HALLTIMEIDV"].ToString());
            //string SEATID = Session["SEATID"].ToString();
            string SEATNM = GetSEAT(Session["SEATID"].ToString());

            FILMIMG = "http://localhost:62405/images/film/" + FILMIMG.Substring(14);
            //LinkedResource inline = new LinkedResource(FILMIMG);
            //inline.ContentId = Guid.NewGuid().ToString();

            //return Content("<script language='javascript' type='text/javascript'>alert('Save Successfully');</script>");  ////Alert Mesajı Göndermek için.

            _Mail.ServerPort = 587;
            _Mail.ServerSmtp = "smtp-mail.outlook.com";
            _Mail.ServerUser = "samet.sahin.0122@hotmail.com";
            _Mail.ServerUserPwd = "***";
            _Mail.RequireAuthentication = true;
            _Mail.BodyFormat = BodyFormat.Html;

            StringBuilder mailTemplate = new StringBuilder();

            mailTemplate.Append("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            mailTemplate.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            mailTemplate.Append("<head>");
            mailTemplate.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            mailTemplate.Append("<style type=\"text/css\">");
            mailTemplate.Append(".template {");
            mailTemplate.Append("font-family: Arial;");
            mailTemplate.Append("font-size: 9.5pt;");
            mailTemplate.Append("}");
            mailTemplate.Append("</style>");
            mailTemplate.Append("</style>");
            mailTemplate.Append("</head>");
            mailTemplate.Append("<body>");
            mailTemplate.Append("<div style='margin-bottom: 10px; position: relative;min-height: 1px;padding-right: 15px;padding-left: 15px;'>");
            mailTemplate.Append("<div style='display: inline-block;display: block;height: auto;max-width: 20%;padding: 4px;line-height: 1.428571429;background-color: #ffffff;border: 1px solid #dddddd;border-radius: 4px;-webkit-transition: all 0.2s ease-in-out;transition: all 0.2s ease-in-out;'>");
            mailTemplate.Append("<img src='" + FILMIMG + "' alt='film' style='display:block;height: auto;max-width: 100%; margin-right: auto;margin-left: auto;' />");
            mailTemplate.Append("<div style='padding: 9px;color: #333333;'>");
            mailTemplate.Append("<h3 style='font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-size: 24px; margin: 0 0 10px;font-weight: 500;line-height: 1.1;' >Film : " + FILMNM + "</h3>");
            mailTemplate.Append("<p style='font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-size: 14px; margin: 0 0 10px;' >Salon : " + HALLNM + "</p>");
            mailTemplate.Append("<p style='font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-size: 14px; margin: 0 0 10px;' >Seans : " + HALLTIME + "</p>");
            mailTemplate.Append("<p style='font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-size: 14px; margin: 0 0 10px;' >Koltuk : " + SEATNM + "</p>");
            mailTemplate.Append("</div>");
            mailTemplate.Append("<div style='padding: 9px;color: #333333;'>");
            mailTemplate.Append("<p style='font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-size: 14px; margin: 0 0 10px;' >Iyi Seyirler :)</p>");
            mailTemplate.Append("<p style='font-family: Helvetica Neue,Helvetica,Arial,sans-serif; font-size: 14px; margin: 0 0 10px;' >Biletiniz Tek Kullanimliktir.</p>");
            mailTemplate.Append("</div>");
            mailTemplate.Append("</div>");
            mailTemplate.Append("</div>");
            mailTemplate.Append("</body>");
            mailTemplate.Append("</html>");

            _Mail.Body = mailTemplate.ToString(); //İçerik
            _Mail.Subject = "Biletiniz."; //Konu

            _Mail.FromEmail = "samet.sahin.0122@hotmail.com"; //Gönderen
            _Mail.ToEmail = ToEmailStr; //Alıcı

            _MailH.Send(_Mail);

            Session["ISSENDMAIL"] = true;
            return Redirect("/Home/Filmticket");
        }

        private string GetFILMNM(object FILMID)
        {
            DataSet dsFILM = new DataSet();

            using (DataVw dMan = new DataVw())
            {
                dsFILM = dMan.ExecuteView_S("FILM", "*", "", "", "");
            }

            DataView dvFILM = new DataView(dsFILM.Tables[0], "", "ID", DataViewRowState.CurrentRows);

            string FILMNM = "";

            if (dvFILM == null)
                return FILMNM;
            object result = dvFILM.Find(FILMID);

            if ((int)result == -1)
                return FILMNM;

            FILMNM = dvFILM[(int)result].Row["FILMNM"].ToString();

            return FILMNM;
        }
        
        private string GetFILMIMG(object FILMID)
        {
            DataSet dsFILM = new DataSet();

            using (DataVw dMan = new DataVw())
            {
                dsFILM = dMan.ExecuteView_S("FILM", "*", "", "", "");
            }

            DataView dvFILM = new DataView(dsFILM.Tables[0], "", "ID", DataViewRowState.CurrentRows);

            string FILMIMG = "";

            if (dvFILM == null)
                return FILMIMG;
            object result = dvFILM.Find(FILMID);

            if ((int)result == -1)
                return FILMIMG;

            FILMIMG = dvFILM[(int)result].Row["FILMIMG"].ToString();

            return FILMIMG;
        }

        private string GetHALLNM(object HALLID)
        {
            DataSet dsHALL = new DataSet();

            using (DataVw dMan = new DataVw())
            {
                dsHALL = dMan.ExecuteView_S("HALL", "*", "", "", "");
            }

            DataView dvHALL = new DataView(dsHALL.Tables[0], "", "ID", DataViewRowState.CurrentRows);

            string HALLNM = "";

            if (dvHALL == null)
                return HALLNM;
            object result = dvHALL.Find(HALLID);
            
            if ((int)result == -1)
                return HALLNM;

            HALLNM = dvHALL[(int)result].Row["HALLNM"].ToString();

            return HALLNM;
        }

        private string GetHALLTIME(object HALLTIMEID)
        {
            DataSet dsHALLTIME = new DataSet();

            using (DataVw dMan = new DataVw())
            {
                dsHALLTIME = dMan.ExecuteView_S("HALLTIME", "*", "", "", "");
            }

            DataView dvHALLTIME = new DataView(dsHALLTIME.Tables[0], "", "ID", DataViewRowState.CurrentRows);

            string HALLTIME = "";

            if (dvHALLTIME == null)
                return HALLTIME;
            object result = dvHALLTIME.Find(HALLTIMEID);

            if ((int)result == -1)
                return HALLTIME;

            HALLTIME = dvHALLTIME[(int)result].Row["HALLTIME"].ToString();

            return HALLTIME;
        }

        private string GetSEAT(object SEATID)
        {
            DataSet dsSEAT = new DataSet();

            using (DataVw dMan = new DataVw())
            {
                dsSEAT = dMan.ExecuteView_S("SEAT", "*", "", "", "");
            }

            DataView dvSEAT = new DataView(dsSEAT.Tables[0], "", "ID", DataViewRowState.CurrentRows);

            string SEATNM = "";

            if (dvSEAT == null)
                return SEATNM;
            object result = dvSEAT.Find(SEATID);

            if ((int)result == -1)
                return SEATNM;

            SEATNM = dvSEAT[(int)result].Row["SEATNM"].ToString();

            return SEATNM;
        }

        #endregion
    }
}
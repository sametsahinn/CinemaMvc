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
        public DataSet dsFilm = new DataSet();
        public User UserData;
        public string FILMID;

        #region LoginInfo Class Çağır

        LoginInfo _li;

        public LoginInfo UserInfo
        {
            get { return _li; }
        }

        #endregion

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


        //
        // GET: /Home/
        public ActionResult Index(IEnumerable<FilmD> film)
        {
            using (DataVw dMan = new DataVw())
            {
                dsFilm = dMan.ExecuteView_S("FILM", "*", "", "", "");
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
            ViewBag.FilmListD = datatable;

            if (HttpContext.Application["counter"] == null)
            {
                HttpContext.Application["counter"] = 0;
            }

            HttpContext.Application["counter"] = Convert.ToInt16(HttpContext.Application["counter"]) + 1;

            ViewBag.counter = HttpContext.Application["counter"];

            return View(datatable);
        }

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

            ViewBag.FilmListDe = filmDe;
            ViewBag.FilmListInfo = filmInfo;

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

            ViewBag.FilmSeatList = filmSeat;

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

    }
}
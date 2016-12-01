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
using System.Threading.Tasks;
using System.Net;
using Microsoft.Owin.Security;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using OfficeAgent.Object;
using System.IO;

namespace CinemaWeb.Controllers
{
    public class AccountController : Controller
    {
        public DataSet dsUser = new DataSet();
        public DataSet dsLogError = new DataSet();
        public DataSet dsLogTableFilm = new DataSet();
        public DataSet dsLogTableHall = new DataSet();
        public DataSet dsLogTableFilmTyp = new DataSet();
        public DataSet dsUserTransactions = new DataSet();
        public static User UserData;

        #region LoginInfo Class Çağır

        LoginInfo _li;

        public LoginInfo UserInfo
        {
            get { return _li; }
        }

        #endregion

        #region Model

        public class AdmUserList
        {
            public Guid ID { get; set; }
            public string IS_AC { get; set; }
            public string USRNM { get; set; }
            public string PWD { get; set; }
            public string FULNM { get; set; }
            public string EMAIL { get; set; }
            public string IS_SYSADM { get; set; }
            public string IS_ADMIN { get; set; }
            public string IS_HR { get; set; }
            public string CHNG_PWD { get; set; }
            public string AVATAR { get; set; }
            public DateTime EDATE { get; set; }
            public DateTime UDATE { get; set; }
            public string CARDNO { get; set; }
            public string CVC { get; set; }
            public string STKDAY { get; set; }
            public string STKMONTH { get; set; }
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
        
        public class Logerror
        {
            public Guid ID { get; set; }
            public string LOGINFO { get; set; }
            public string LOGIP { get; set; }
            public DateTime LOGDATE { get; set; }
            public DateTime EDATE { get; set; }
            public string EUSRID { get; set; }
            public DateTime UDATE { get; set; }
            public string UUSRID { get; set; }
            public string NOTE { get; set; }
        }

        public class LogtableFilm
        {
            public Guid ID { get; set; }
            public string LOGTABLE { get; set; }
            public Guid LOGTABLEID { get; set; }
            public string LOGIP { get; set; }
            public string FULNM { get; set; }
            public string FILMNM { get; set; }
            public string FILMIMG { get; set; }
            public DateTime EDATE { get; set; }
            public string EUSRID { get; set; }
            public DateTime UDATE { get; set; }
            public string UUSRID { get; set; }
            public string NOTE { get; set; }
        }

        public class LogtableHall
        {
            public Guid ID { get; set; }
            public string LOGTABLE { get; set; }
            public Guid LOGTABLEID { get; set; }
            public string LOGIP { get; set; }
            public string FULNM { get; set; }
            public string HALLNM { get; set; }
            public DateTime EDATE { get; set; }
            public string EUSRID { get; set; }
            public DateTime UDATE { get; set; }
            public string UUSRID { get; set; }
            public string NOTE { get; set; }
        }

        public class LogtableFilmTyp
        {
             public Guid ID { get; set; }
            public string LOGTABLE { get; set; }
            public Guid LOGTABLEID { get; set; }
            public string LOGIP { get; set; }
            public string FULNM { get; set; }
            public string FILMTYPNM { get; set; }
            public DateTime EDATE { get; set; }
            public string EUSRID { get; set; }
            public DateTime UDATE { get; set; }
            public string UUSRID { get; set; }
            public string NOTE { get; set; }
        }

        public class UserTransactions
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

        #endregion

        //
        // GET: /Account/
        public ActionResult Login()
        {
            return View();
        }

        //
        // GET: /Account/
        public ActionResult Register()
        {
            return View();
        }

        #region Manage

        //
        // GET: /Account/
        public ActionResult Manage()
        {
            DataSet dsUser = new DataSet();
            string USRID = Session["USRIDv"].ToString();
            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", USRID, "", "ID = ");
            }

            List<UserList> userList = new List<UserList>();
            foreach (DataRow dr in dsUser.Tables[0].Rows)
            {
                userList.Add(new UserList
                {
                    ID = (Guid)dr["ID"],
                    USRNM = dr["USRNM"].ToString(),
                    PWD = CryptionHelper.Decrypt(dr["PWD"].ToString(), "tb"),
                    EMAIL = dr["EMAIL"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    CARDNO = dr["CARDNO"].ToString(),
                    CVC = dr["CVC"].ToString(),
                    STKDAY = dr["STKDAY"].ToString(),
                    STKMONTH = dr["STKMONTH"].ToString()
                });
            }

            ViewBag.UserList = userList;

            return View();
        }

        #endregion

        #region Users

        //
        // GET: /Account/
        public ActionResult Users()
        {
            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", "", "", "");
            }

            List<AdmUserList> admuserList = new List<AdmUserList>();
            foreach (DataRow dr in dsUser.Tables[0].Rows)
            {
                string IS_AC, IS_SYSADM, IS_ADMIN, IS_HR, CHNG_PWD;

                if (Convert.ToBoolean(dr["IS_AC"])) { IS_AC = "Evet"; } else { IS_AC = "Hayır"; }
                if (Convert.ToBoolean(dr["IS_SYSADM"])) { IS_SYSADM = "Evet"; } else { IS_SYSADM = "Hayır"; }
                if (Convert.ToBoolean(dr["IS_ADMIN"])) { IS_ADMIN = "Evet"; } else { IS_ADMIN = "Hayır"; }
                if (Convert.ToBoolean(dr["IS_HR"])) { IS_HR = "Evet"; } else { IS_HR = "Hayır"; }
                if (Convert.ToBoolean(dr["CHNG_PWD"])) { CHNG_PWD = "Evet"; } else { CHNG_PWD = "Hayır"; }

                admuserList.Add(new AdmUserList
                {
                    ID = (Guid)dr["ID"],
                    IS_AC = IS_AC,
                    USRNM = dr["USRNM"].ToString(),
                    PWD = CryptionHelper.Decrypt(dr["PWD"].ToString(), "tb"),
                    FULNM = dr["FULNM"].ToString(),
                    EMAIL = dr["EMAIL"].ToString(),
                    IS_SYSADM = IS_SYSADM,
                    IS_ADMIN = IS_ADMIN,
                    IS_HR = IS_HR,
                    CHNG_PWD = CHNG_PWD,
                    AVATAR = dr["AVATAR"].ToString(),
                    EDATE = (DateTime)dr["EDATE"],
                    UDATE = (DateTime)dr["UDATE"],
                    CARDNO = dr["CARDNO"].ToString(),
                    CVC = dr["CVC"].ToString(),
                    STKDAY = dr["STKDAY"].ToString(),
                    STKMONTH = dr["STKMONTH"].ToString()
                });
            }

            ViewBag.UserList = admuserList;

            return View();
        }

        public ActionResult UsersActive(System.Web.Mvc.FormCollection collection)
        {
            string USRID = collection["btnUsrId"];
            int IS_AC;

            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", USRID, "", "ID = ");
            }

            DataRow newrow = dsUser.Tables[0].Rows[0];
            if (Convert.ToBoolean(newrow["IS_AC"])) { IS_AC = 0; } else{ IS_AC = 1; }
            newrow["ID"] = USRID;
            newrow["IS_AC"] = IS_AC;
            //newrow["EDATE"] = DateTime.Now;
            //newrow["EUSRID"] = null;
            newrow["UDATE"] = DateTime.Now;
            //newrow["UUSRID"] = null;
            newrow["NOTE"] = "En Son Güncelleme İşlemi Gerçekleştirdi.";
            AgentGc data = new AgentGc();
            string veri = data.DataModified("USR", newrow, dsUser.Tables[0]);

            return Redirect("/Account/Users");
        }

        #endregion

        #region Control

        [HttpPost]
        public ActionResult Control(string txtUsername, string txtPassword)
        {
            UserManager uMan = new UserManager(txtUsername, txtPassword);
            _li = uMan.CheckLogin();

            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", txtUsername, "", "USRNM =");
                dsLogError = dMan.ExecuteView_S("LOGERROR", "*", "", "", "");
            }

            if (dsUser.Tables[0].Rows.Count > 0)
            {
                DataRow row = dsUser.Tables[0].Rows[0];

                UserData = new User();
                UserData.USRID = (Guid)row["ID"];

                UserData.USRNM = Convert.ToString(row["USRNM"]);
                UserData.FULNM = Convert.ToString(row["FULNM"]);
                UserData.AVATAR = Convert.ToString(row["AVATAR"]);
                UserData.Email = Convert.ToString(row["EMAIL"]);
                UserData.IS_ADMIN = Convert.ToBoolean(row["IS_ADMIN"]);
                UserData.IS_SYSADM = Convert.ToBoolean(row["IS_SYSADM"]);
                UserData.IS_HR = Convert.ToBoolean(row["IS_HR"]);

                if (txtUsername.ToString() == row["USRNM"].ToString() && txtPassword.ToString() == CryptionHelper.Decrypt(row["PWD"].ToString(), "tb").ToString())
                {
                    Session["USRIDv"] = row["ID"].ToString();
                    Session["name"] = row["FULNM"].ToString();
                    Session["admin"] = true;
                    Session["IsAuthenticated"] = true;
                    Session["IS_SYSADM"] = row["IS_SYSADM"].ToString();
                    //await SignInAsync(user, model.RememberMe);
                    if (row["IS_SYSADM"].ToString() == "True")
                    {
                        Session["loginRoles"] = true;
                        Session["admin"] = true;
                        if (row["AVATAR"].ToString() == "")
                        {
                            Session["avatarimg"] = "~/images/avatar/nullavatar.jpg";
                        }
                        else
                        {
                            Session["avatarimg"] = row["AVATAR"].ToString();
                        }                       

                        return Redirect("/Home/Admin");
                    }
                    else
                    {
                        Session["IsAuthenticated"] = true;
                        Session["loginRoles"] = false;
                        Session["admin"] = false;
                        if (row["AVATAR"].ToString() == "")
                        {
                            Session["avatarimg"] = "~/images/avatar/nullavatar.jpg";
                        }
                        else
                        {
                            Session["avatarimg"] = row["AVATAR"].ToString();
                        } 
                    }

                    return Redirect("/Home/Index");
                }

                DataRow newrow = dsLogError.Tables[0].NewRow();
                newrow["ID"] = Guid.NewGuid();
                newrow["LOGINFO"] = "Kullanıcı adı : " + txtUsername + " " + "Parola : " + txtPassword;
                newrow["LOGIP"] = GetIp();
                newrow["LOGDATE"] = DateTime.Now;
                newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = null;
                newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                newrow["NOTE"] = "Parolaya Bağlı Hatalı Giriş.";
                AgentGc data = new AgentGc();
                string veri = data.DataAdded("LOGERROR", newrow, dsLogError.Tables[0]);

                Session["loginFailed"] = true;
                Session["IsAuthenticated"] = false;

                int loginErrorCount = Convert.ToInt32(Session["wrongpiece"]);

                Session["wrongpiece"] = loginErrorCount + 1;
                Session["wrongdate"] = DateTime.Now;
                //Session["IP"] = GetIp();

                return Redirect("/Account/Login");
            }
            else
            {
                DataRow newrow = dsLogError.Tables[0].NewRow();
                newrow["ID"] = Guid.NewGuid();
                newrow["LOGINFO"] = "Kullanıcı adı : " + txtUsername + " " + "Parola : " + txtPassword;
                newrow["LOGIP"] = GetIp();
                newrow["LOGDATE"] = DateTime.Now;
                newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = null;
                newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                newrow["NOTE"] = "Hatalı Giriş.";
                AgentGc data = new AgentGc();
                string veri = data.DataAdded("LOGERROR", newrow, dsLogError.Tables[0]);

                Session["loginFailed"] = true;
                return Redirect("/Account/Login");
            }
        }

        #endregion

        #region UserAdd

        [HttpPost]
        public ActionResult UserAdd(string txtUSRNM, string txtFULNM, string txtPWD, string txtEMAIL, string txtCARDNO, string txtCVC, string txtSTKDAY, string txtSTKMONTH, HttpPostedFileBase file)
        {
            string filefo = "";
            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", "", "", "");
            }

            //if (txtUSRNM.ToString() == "" || txtFULNM.ToString() == "" || txtPWD.ToString() == "" || txtEMAIL.ToString() == "" || txtCARDNO.ToString() == "" || txtCVC.ToString() == "" || txtSTKDAY.ToString() == "" || txtSTKMONTH.ToString() == "")
            //{
            //    Session["useraddsuccess"] = false;
            //    ViewBag.addmessage = "Eksik veri girişi! Tüm Alanları Doldurunuz.";
            //    return Redirect("/Account/Register");
            //}
            //else
            //{
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/images/avatar"), pic);
                    string pathd = "~/images/avatar/"+ pic;
                    // file is uploaded
                    file.SaveAs(path);
                    filefo = pathd;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                }

                DataRow newrow = dsUser.Tables[0].NewRow();
                newrow["ID"] = Guid.NewGuid();
                newrow["USRNM"] = txtUSRNM;
                newrow["FULNM"] = txtFULNM;
                newrow["EMAIL"] = txtEMAIL;
                newrow["PWD"] = CryptionHelper.Encrypt(txtPWD, "tb");
                newrow["IS_ADMIN"] = 1;
                newrow["IS_SYSADM"] = 0;
                newrow["IS_HR"] = 0;
                newrow["CHNG_PWD"] = 0;
                if (filefo == ""){
                    newrow["AVATAR"] = "~/images/avatar/nullavatar.jpg";
                }else{
                    newrow["AVATAR"] = filefo;
                }
                newrow["AVATAR"] = filefo;
                newrow["CARDNO"] = txtCARDNO;
                newrow["CVC"] = txtCVC;
                newrow["STKDAY"] = txtSTKDAY;
                newrow["STKMONTH"] = txtSTKMONTH;
                newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = null;
                newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                newrow["NOTE"] = "En Son Kayıt İşlemi Gerçekleştirdi.";
                AgentGc data = new AgentGc();
                string veri = data.DataAdded("USR", newrow, dsUser.Tables[0]);
                Session["useraddsuccess"] = true;
                ViewBag.addmessageinfo = veri;
                return Redirect("/Account/Login");
           // }
        }

        #endregion

        #region ManageControl

        [HttpPost]
        public ActionResult SelectUserInfo(System.Web.Mvc.FormCollection collection)
        {
            string USRID = collection.AllKeys[0].ToString();

            return Redirect("/Account/SelectUserInfoChange");
        }

        [HttpPost]
        public ActionResult SelectUserInfoChange(string txtUSRNM, string txtFULNM, string txtPWD, string txtEMAIL, string txtCARDNO, string txtCVC, string txtSTKDAY, string txtSTKMONTH, HttpPostedFileBase file, System.Web.Mvc.FormCollection collection)
        {
            DataSet dsUser = new DataSet();
            string USRID = collection.AllKeys[8].ToString();
            string filefo = "";
            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", USRID, "", "ID = ");
            }

            //if (txtUSRNM.ToString() == "" || txtFULNM.ToString() == "" || txtPWD.ToString() == "" || txtEMAIL.ToString() == "" || txtCARDNO.ToString() == "" || txtCVC.ToString() == "" || txtSTKDAY.ToString() == "" || txtSTKMONTH.ToString() == "")
            //{
            //    return Content("<script language='javascript' type='text/javascript'>alert('Eksik veri girişi! Tüm Alanları Doldurunuz.');</script>");  ////Alert Mesajı Göndermek için.
            //    //ViewBag.addmessage = "Eksik veri girişi! Tüm Alanları Doldurunuz.";
            //    //return Redirect("/Account/Manage");
            //}
            //else
            //{
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/images/avatar"), pic);
                    string pathd = "~/images/avatar/"+ pic;
                    // file is uploaded
                    file.SaveAs(path);
                    filefo = pathd;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                }

                DataRow newrow = dsUser.Tables[0].Rows[0];
                newrow["ID"] = USRID;
                newrow["USRNM"] = txtUSRNM;
                newrow["FULNM"] = txtFULNM;
                newrow["EMAIL"] = txtEMAIL;
                newrow["PWD"] = CryptionHelper.Encrypt(txtPWD, "tb");
                newrow["IS_ADMIN"] = 1;
                newrow["IS_SYSADM"] = 0;
                newrow["IS_HR"] = 0;
                newrow["CHNG_PWD"] = 0;
                if (filefo == ""){
                    newrow["AVATAR"] = "~/images/avatar/nullavatar.jpg";
                }else{
                    newrow["AVATAR"] = filefo;
                }
                newrow["CARDNO"] = txtCARDNO;
                newrow["CVC"] = txtCVC;
                newrow["STKDAY"] = txtSTKDAY;
                newrow["STKMONTH"] = txtSTKMONTH;
                //newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = null;
                newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                newrow["NOTE"] = "En Son Güncelleme İşlemi Gerçekleştirdi.";
                AgentGc data = new AgentGc();
                string veri = data.DataModified("USR", newrow, dsUser.Tables[0]);
                //return Content("<script language='javascript' type='text/javascript'>alert('" + veri + "');</script>");
                //ViewBag.addmessageinfo = veri;
                return Redirect("/Account/Manage");
            //}
            //return Redirect("/Account/Manage");
        }

        #endregion

        #region LogOff

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Abandon();
            //AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region LogError

        // GET: /Account/
        public ActionResult LogError()
        {
            using (DataVw dMan = new DataVw())
            {
                dsLogError = dMan.ExecuteView_S("LOGERROR", "*", "", "", "");
            }

            List<Logerror> logError = new List<Logerror>();
            foreach (DataRow dr in dsLogError.Tables[0].Rows)
            {
                logError.Add(new Logerror
                {
                    ID = (Guid)dr["ID"],
                    LOGINFO = dr["LOGINFO"].ToString(),
                    LOGIP = dr["LOGIP"].ToString(),
                    LOGDATE = (DateTime)dr["LOGDATE"],
                    EDATE = (DateTime)dr["EDATE"],
                    EUSRID = dr["EUSRID"].ToString(),
                    UDATE = (DateTime)dr["UDATE"],
                    UUSRID = dr["UUSRID"].ToString(),
                    NOTE = dr["NOTE"].ToString()
                });
            }

            ViewBag.LogError = logError;

            return View();
        }

        public string GetIp()
        {
            var strHostName = "";
            strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            var addr = ipEntry.AddressList;
            return addr[0].ToString();
        }

        #endregion

        #region LogTable

        // GET: /Account/
        public ActionResult LogTable()
        {
            using (DataVw dMan = new DataVw())
            {
                dsLogTableFilm = dMan.ExecuteView_S("FILM_LOG", "*", "", "", "");
                dsLogTableHall = dMan.ExecuteView_S("HALL_LOG", "*", "", "", "");
                dsLogTableFilmTyp = dMan.ExecuteView_S("FILMTYP_LOG", "*", "", "", "");
            }

            List<LogtableFilm> logtableFilm = new List<LogtableFilm>();
            foreach (DataRow dr in dsLogTableFilm.Tables[0].Rows)
            {
                logtableFilm.Add(new LogtableFilm
                {
                    ID = (Guid)dr["ID"],
                    LOGTABLE = dr["LOGTABLE"].ToString(),
                    LOGTABLEID = (Guid)dr["LOGTABLEID"],
                    LOGIP = dr["LOGIP"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    FILMNM = dr["FILMNM"].ToString(),
                    FILMIMG = dr["FILMIMG"].ToString(),
                    EDATE = (DateTime)dr["EDATE"],
                    EUSRID = dr["EUSRID"].ToString(),
                    UDATE = (DateTime)dr["UDATE"],
                    UUSRID = dr["UUSRID"].ToString(),
                    NOTE = dr["NOTE"].ToString()
                });
            }

            ViewBag.LogtableFilm = logtableFilm;

            List<LogtableHall> logtableHall = new List<LogtableHall>();
            foreach (DataRow dr in dsLogTableHall.Tables[0].Rows)
            {
                logtableHall.Add(new LogtableHall
                {
                    ID = (Guid)dr["ID"],
                    LOGTABLE = dr["LOGTABLE"].ToString(),
                    LOGTABLEID = (Guid)dr["LOGTABLEID"],
                    LOGIP = dr["LOGIP"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    HALLNM = dr["HALLNM"].ToString(),
                    EDATE = (DateTime)dr["EDATE"],
                    EUSRID = dr["EUSRID"].ToString(),
                    UDATE = (DateTime)dr["UDATE"],
                    UUSRID = dr["UUSRID"].ToString(),
                    NOTE = dr["NOTE"].ToString()
                });
            }
            ViewBag.LogtableHall = logtableHall;

            List<LogtableFilmTyp> logtableFilmTyp = new List<LogtableFilmTyp>();
            foreach (DataRow dr in dsLogTableFilmTyp.Tables[0].Rows)
            {
                logtableFilmTyp.Add(new LogtableFilmTyp
                {
                    ID = (Guid)dr["ID"],
                    LOGTABLE = dr["LOGTABLE"].ToString(),
                    LOGTABLEID = (Guid)dr["LOGTABLEID"],
                    LOGIP = dr["LOGIP"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    FILMTYPNM = dr["FILMTYPNM"].ToString(),
                    EDATE = (DateTime)dr["EDATE"],
                    EUSRID = dr["EUSRID"].ToString(),
                    UDATE = (DateTime)dr["UDATE"],
                    UUSRID = dr["UUSRID"].ToString(),
                    NOTE = dr["NOTE"].ToString()
                });
            }

            ViewBag.LogtableFilmTyp = logtableFilmTyp;

            return View();
        }

        #endregion

        #region Transactions

        //
        // GET: /Account/
        public ActionResult Transactions()
        {
            using (DataVw dMan = new DataVw())
            {
                dsUserTransactions = dMan.ExecuteView_S("TICKETINFO_V", "*", Session["USRIDv"].ToString(), "", "USRID = ");
            }

            List<UserTransactions> userTransactions = new List<UserTransactions>();
            foreach (DataRow dr in dsUserTransactions.Tables[0].Rows)
            {
                userTransactions.Add(new UserTransactions
                {
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

            ViewBag.UserTransactions = userTransactions;

            return View();
        }

        #endregion
    }
}
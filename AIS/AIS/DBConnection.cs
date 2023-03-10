using System;
using System.Collections.Generic;
using AIS.Models;
using Oracle.ManagedDataAccess.Client;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.IO;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Hosting;
using iText.Html2pdf;
using iTextSharp.tool.xml.parser;
using System.Globalization;

namespace AIS
{
    public class DBConnection
    {
        private SessionHandler sessionHandler;
        private readonly SQLParams sqlParams = new SQLParams();
        private readonly LocalIPAddress iPAddress = new LocalIPAddress();
        private readonly DateTimeHandler dtime = new DateTimeHandler();
        private readonly CAUEncodeDecode encoderDecoder = new CAUEncodeDecode();
        public ISession _session;
        public IHttpContextAccessor _httpCon;
        private OracleConnection _con;
      
       [Obsolete]
        private readonly IHostingEnvironment _env;
        

        [Obsolete]
        public DBConnection(IHttpContextAccessor httpContextAccessor, IHostingEnvironment env )
        {
            _session = httpContextAccessor.HttpContext.Session;
            _httpCon = httpContextAccessor;
            _env = env;
          
        }
        public DBConnection()
        {
           
        }

      
        private void DisposeDatabaseConnection()
        {
            try
            {
                _con.Close();
                    _con.Dispose();

            }
            catch (Exception) { }
        }
        private OracleConnection DatabaseConnection()
        {
            
            try
            {
                _con = new OracleConnection();
                OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
                ocsb.Password = "ztblaisdev";
                ocsb.UserID = "ztblaisdev";
                ocsb.DataSource = "10.1.100.222:1521/devdb18c.ztbl.com.pk";
                // connect
                _con.ConnectionString = ocsb.ConnectionString;
                _con.Open();
                return _con;

            }
            catch (Exception) { return null; }
        }
       
        public UserModel AutheticateLogin(LoginModel login)
        {
            var con = this.DatabaseConnection();
            UserModel user = new UserModel();
            user.isAlreadyLoggedIn = false;
            user.isAuthenticate = false;
            var enc_pass = getMd5Hash(login.Password);
            using (OracleCommand cmd = con.CreateCommand())
            {
                string _sql = "pkg_ais.p_get_user";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = login.PPNumber;
                cmd.Parameters.Add("enc_pass", OracleDbType.Varchar2).Value = enc_pass;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                cmd.CommandText = _sql;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    user.isAuthenticate = true;
                    user.ID = Convert.ToInt32(rdr["USERID"]);
                    user.Name = rdr["Employeefirstname"].ToString() + " " + rdr["employeelastname"].ToString();
                    user.Email = rdr["LOGIN_NAME"].ToString();
                    user.PPNumber = rdr["PPNO"].ToString();
                    if (rdr["ENTITY_ID"].ToString() != null && rdr["ENTITY_ID"].ToString() != "")
                        user.UserEntityID = Convert.ToInt32(rdr["ENTITY_ID"]);

                    user.UserLocationType = rdr["USER_LOCATION_TYPE"].ToString();
                    user.IsActive = rdr["ISACTIVE"].ToString();
                    if (rdr["DIVISIONID"].ToString() != null && rdr["DIVISIONID"].ToString() != "")
                        user.UserPostingDiv = Convert.ToInt32(rdr["DIVISIONID"]);
                    else
                        user.UserPostingDiv = 0;

                    if (rdr["DEPARTMENTID"].ToString() != null && rdr["DEPARTMENTID"].ToString() != "")
                        user.UserPostingDept = Convert.ToInt32(rdr["DEPARTMENTID"]);
                    else
                        user.UserPostingDept = 0;

                    if (rdr["ZONEID"].ToString() != null && rdr["ZONEID"].ToString() != "")
                        user.UserPostingZone = Convert.ToInt32(rdr["ZONEID"]);
                    else
                        user.UserPostingZone = 0;

                    if (rdr["BRANCHID"].ToString() != null && rdr["BRANCHID"].ToString() != "")
                        user.UserPostingBranch = Convert.ToInt32(rdr["BRANCHID"]);
                    else
                        user.UserPostingBranch = 0;

                    if (rdr["AUDIT_ZONEID"].ToString() != null && rdr["AUDIT_ZONEID"].ToString() != "")
                        user.UserPostingAuditZone = Convert.ToInt32(rdr["AUDIT_ZONEID"]);
                    else
                        user.UserPostingAuditZone = 0;

                    if (rdr["GROUP_ID"].ToString() != null && rdr["GROUP_ID"].ToString() != "")
                        user.UserGroupID = Convert.ToInt32(rdr["GROUP_ID"]);
                    else
                        user.UserGroupID = 0;

                    if (rdr["ROLE_ID"].ToString() != null && rdr["ROLE_ID"].ToString() != "")
                        user.UserRoleID = Convert.ToInt32(rdr["ROLE_ID"]);
                    else
                        user.UserRoleID = 0;

                    bool isSessionAvailable = false;
                    string _sql2 = "pkg_ais.p_get_user_id";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = login.PPNumber;
                    cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    cmd.CommandText = _sql2;
                    OracleDataReader rdr2 = cmd.ExecuteReader();
                    while (rdr2.Read())
                    {
                        if (rdr2["ID"].ToString() != null && rdr2["ID"].ToString() != "")
                        {
                            isSessionAvailable = !isSessionAvailable;
                        }
                    }

                    sessionHandler = new SessionHandler();
                    sessionHandler._httpCon = this._httpCon;
                    sessionHandler._session = this._session;
                    if (isSessionAvailable)
                    {
                        user.isAlreadyLoggedIn = true;
                    }
                    else
                    {
                        var resp = sessionHandler.SetSessionUser(user);
                        cmd.CommandText = "pkg_ais.User_SESSION";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = user.PPNumber;
                        cmd.Parameters.Add("UserRoleID", OracleDbType.Int32).Value = user.UserRoleID;
                        cmd.Parameters.Add("LocalIpAddress", OracleDbType.Varchar2).Value = iPAddress.GetLocalIpAddress();
                        cmd.Parameters.Add("SessionId", OracleDbType.Varchar2).Value = resp.SessionId;
                        cmd.Parameters.Add("UserLocationType", OracleDbType.Varchar2).Value = user.UserLocationType;
                        cmd.Parameters.Add("MACAddress", OracleDbType.Varchar2).Value = iPAddress.GetMACAddress();
                        cmd.Parameters.Add("FirstMACCardAddress", OracleDbType.Varchar2).Value = iPAddress.GetFirstMACCardAddress();
                        cmd.Parameters.Add("UserPostingDiv", OracleDbType.Int32).Value = user.UserPostingDiv;
                        cmd.Parameters.Add("UserGroupID", OracleDbType.Varchar2).Value = user.UserGroupID;
                        cmd.Parameters.Add("UserPostingDept", OracleDbType.Int32).Value = user.UserPostingDept;
                        cmd.Parameters.Add("UserPostingZone", OracleDbType.Int32).Value = user.UserPostingZone;
                        cmd.Parameters.Add("UserPostingBranch", OracleDbType.Int32).Value = user.UserPostingBranch;
                        cmd.Parameters.Add("UserPostingAuditZone", OracleDbType.Int32).Value = user.UserPostingAuditZone;
                        cmd.ExecuteReader();
                        //this.CreateAuditReport();
                    }
                }
            }
           this.DisposeDatabaseConnection();
            return user;
        }
        public bool DisposeLoginSession()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var sessionUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.Session_END";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = sessionUser.PPNumber;
                cmd.Parameters.Add("SessionId", OracleDbType.Varchar2).Value = sessionUser.SessionId;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            sessionHandler.DisposeUserSession();
            return true;
        }
        public bool IsLoginSessionExist(string PPNumber = "")
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var sessionUser = sessionHandler.GetSessionUser();

            if (PPNumber == "")
                PPNumber = sessionUser.PPNumber;
            bool isSession = false;
            if (PPNumber != null && PPNumber != "")
            {
                var con = this.DatabaseConnection();

                using (OracleCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "pkg_ais.p_get_user_session";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = PPNumber;
                    cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    OracleDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        if (rdr["ID"].ToString() != "" && rdr["ID"].ToString() != null)
                            isSession = true;
                    }
                }
               this.DisposeDatabaseConnection();
            }

            return isSession;
        }
        public bool KillExistSession(LoginModel login)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var enc_pass = getMd5Hash(login.Password);
            var con = this.DatabaseConnection();
            bool isSession = false;
            using (OracleCommand cmd = con.CreateCommand())
            {
                string _sql = "pkg_ais.p_get_user";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = login.PPNumber;
                cmd.Parameters.Add("enc_pass", OracleDbType.Varchar2).Value = enc_pass;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                cmd.CommandText = _sql;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    cmd.CommandText = "pkg_ais.Session_Kill";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = login.PPNumber;
                    cmd.ExecuteReader();
                    isSession = true;
                }
            }
           this.DisposeDatabaseConnection();
            sessionHandler.DisposeUserSession();
            return isSession;
        }
        public bool TerminateIdleSession()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            bool isTerminate = false;
            if (loggedInUser.PPNumber != null && loggedInUser.PPNumber != "")
            {
                var con = this.DatabaseConnection();
                using (OracleCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "pkg_ais.Session_Kill";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                    cmd.ExecuteReader();
                    isTerminate = true;
                }
               this.DisposeDatabaseConnection();
                sessionHandler.DisposeUserSession();
            }
            return isTerminate;
        }
        public static string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(System.Text.Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        public List<MenuModel> GetTopMenus()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<MenuModel> modelList = new List<MenuModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_GetTopMenus";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("UserRoleID", OracleDbType.Int32).Value = loggedInUser.UserRoleID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MenuModel menu = new MenuModel();
                    menu.Menu_Id = Convert.ToInt32(rdr["MENU_ID"]);
                    menu.Menu_Name = rdr["MENU_NAME"].ToString();
                    menu.Menu_Order = rdr["MENU_ORDER"].ToString();
                    menu.Menu_Description = rdr["MENU_DESCRIPTION"].ToString();
                    modelList.Add(menu);
                }
            }
           this.DisposeDatabaseConnection();
            return modelList;
        }
        public List<MenuPagesModel> GetTopMenuPages()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<MenuPagesModel> modelList = new List<MenuPagesModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_GetTopMenuPages";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("UserGroupID", OracleDbType.Int32).Value = loggedInUser.UserGroupID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MenuPagesModel menuPage = new MenuPagesModel();
                    menuPage.Id = Convert.ToInt32(rdr["ID"]);
                    menuPage.Menu_Id = Convert.ToInt32(rdr["MENU_ID"]);
                    menuPage.Page_Name = rdr["PAGE_NAME"].ToString();
                    menuPage.Page_Path = rdr["PAGE_PATH"].ToString();
                    menuPage.Page_Order = Convert.ToInt32(rdr["PAGE_ORDER"]);
                    menuPage.Status = rdr["STATUS"].ToString();
                    menuPage.Hide_Menu = Convert.ToInt32(rdr["HIDE_MENU"]);
                    modelList.Add(menuPage);
                }
            }
           this.DisposeDatabaseConnection();
            return modelList;
        }
        public List<MenuModel> GetAllTopMenus()
        {
            var con = this.DatabaseConnection();
            List<MenuModel> modelList = new List<MenuModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAllTopMenus";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MenuModel menu = new MenuModel();
                    menu.Menu_Id = Convert.ToInt32(rdr["MENU_ID"]);
                    menu.Menu_Name = rdr["MENU_NAME"].ToString();
                    menu.Menu_Order = rdr["MENU_ORDER"].ToString();
                    menu.Menu_Description = rdr["MENU_DESCRIPTION"].ToString();
                    modelList.Add(menu);
                }
            }
           this.DisposeDatabaseConnection();
            return modelList;
        }
        public List<MenuPagesModel> GetAllMenuPages(int menuId = 0)
        {
            var con = this.DatabaseConnection();

            List<MenuPagesModel> modelList = new List<MenuPagesModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_GetAllMenuPages";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("menuId", OracleDbType.Int32).Value = menuId;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MenuPagesModel menuPage = new MenuPagesModel();
                    menuPage.Id = Convert.ToInt32(rdr["ID"]);
                    menuPage.Menu_Id = Convert.ToInt32(rdr["MENU_ID"]);
                    menuPage.Page_Name = rdr["PAGE_NAME"].ToString();
                    menuPage.Page_Path = rdr["PAGE_PATH"].ToString();
                    menuPage.Page_Order = Convert.ToInt32(rdr["PAGE_ORDER"]);
                    menuPage.Status = rdr["STATUS"].ToString();
                    modelList.Add(menuPage);
                }
            }
           this.DisposeDatabaseConnection();
            return modelList;
        }
        public List<MenuPagesModel> GetAssignedMenuPages(int groupId, int menuId)
        {
            var con = this.DatabaseConnection();

            List<MenuPagesModel> modelList = new List<MenuPagesModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAssignedMenuPages";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("groupId", OracleDbType.Int32).Value = groupId;
                cmd.Parameters.Add("menuId", OracleDbType.Int32).Value = menuId;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MenuPagesModel menuPage = new MenuPagesModel();
                    menuPage.Id = Convert.ToInt32(rdr["ID"]);
                    menuPage.Menu_Id = Convert.ToInt32(rdr["MENU_ID"]);
                    menuPage.Page_Name = rdr["PAGE_NAME"].ToString();
                    menuPage.Page_Path = rdr["PAGE_PATH"].ToString();
                    menuPage.Page_Order = Convert.ToInt32(rdr["PAGE_ORDER"]);
                    menuPage.Status = rdr["STATUS"].ToString();
                    modelList.Add(menuPage);
                }
            }
           this.DisposeDatabaseConnection();
            return modelList;
        }
        public bool UpdateMenuPagesAssignment(int menuId, int pageId)
        {
            var con = this.DatabaseConnection();

             using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_updateAllMenuPages";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("menuId", OracleDbType.Int32).Value = menuId;
                cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = pageId;
                cmd.ExecuteReader();               
            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public List<GroupModel> GetGroups()
        {
            var con = this.DatabaseConnection();
            List<GroupModel> groupList = new List<GroupModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetGroups";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    GroupModel grp = new GroupModel();
                    grp.GROUP_ID = Convert.ToInt32(rdr["GROUP_ID"]);
                    grp.GROUP_NAME = rdr["GROUP_NAME"].ToString();
                    grp.GROUP_DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    grp.GROUP_CODE = Convert.ToInt32(rdr["GROUP_ID"]);
                    grp.ISACTIVE = rdr["STATUS"].ToString();
                    groupList.Add(grp);
                }
            }
           this.DisposeDatabaseConnection();
            return groupList;
        }
        public GroupModel AddGroup(GroupModel gm)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (gm.GROUP_ID == 0)
                {
                    cmd.CommandText = "pkg_ais.p_AddGroup";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("GROUP_DESCRIPTION", OracleDbType.Varchar2).Value = gm.GROUP_DESCRIPTION;
                    cmd.Parameters.Add("GROUP_NAME", OracleDbType.Varchar2).Value = gm.GROUP_NAME;
                    cmd.Parameters.Add("ISACTIVE", OracleDbType.Varchar2).Value = gm.ISACTIVE;
                    cmd.ExecuteReader();
                }
                else if (gm.GROUP_ID != 0)
                {
                    cmd.CommandText = "pkg_ais.P_Group_Update";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("GROUP_ID", OracleDbType.Varchar2).Value = gm.GROUP_ID;
                    cmd.Parameters.Add("GROUP_DESCRIPTION", OracleDbType.Varchar2).Value = gm.GROUP_DESCRIPTION;
                    cmd.Parameters.Add("GROUP_NAME", OracleDbType.Varchar2).Value = gm.GROUP_NAME;
                    cmd.Parameters.Add("ISACTIVE", OracleDbType.Varchar2).Value = gm.ISACTIVE;
                    cmd.ExecuteReader();
                }
            }
           this.DisposeDatabaseConnection();
            return gm;
        }
        public List<AuditPeriodModel> GetAuditPeriods(int dept_code = 0)
        {
            var con = this.DatabaseConnection();
            List<AuditPeriodModel> periodList = new List<AuditPeriodModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditPeriods";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    AuditPeriodModel period = new AuditPeriodModel();
                    period.AUDITPERIODID = Convert.ToInt32(rdr["AUDITPERIODID"]);
                    period.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    period.START_DATE = Convert.ToDateTime(rdr["START_DATE"]);
                    period.END_DATE = Convert.ToDateTime(rdr["END_DATE"]);
                    period.STATUS_ID = Convert.ToInt32(rdr["STATUS_ID"]);
                    periodList.Add(period);
                }
            }
           this.DisposeDatabaseConnection();
            return periodList;
        }
		
        public bool AddAuditPeriod(AuditPeriodModel periodModel)
        {
            bool result = false;
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_AddAuditPeriod";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("DESCRIPTION", OracleDbType.Varchar2).Value = periodModel.DESCRIPTION;
                cmd.Parameters.Add("START_DATE", OracleDbType.Date).Value = periodModel.START_DATE;
                cmd.Parameters.Add("END_DATE", OracleDbType.Date).Value = periodModel.END_DATE;
                cmd.Parameters.Add("STATUS_ID", OracleDbType.Int32).Value = periodModel.STATUS_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    periodModel.REMARKS_OUT = rdr["REMARKS"].ToString();
                    if (rdr["REF"].ToString() != "" && rdr["REF"].ToString() != null && rdr["REF"].ToString() == "1")
                    {
                        periodModel.IS_SUCCESS = "Yes";
                        result = true;
                    }
                }
                result = true;
            }
           this.DisposeDatabaseConnection();
            return result;
        }
        public List<AuditTeamModel> GetAuditTeams(int dept_code = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            List<AuditTeamModel> teamList = new List<AuditTeamModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditTeams";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("dept_code", OracleDbType.Int32).Value = dept_code;
                cmd.Parameters.Add("UserEntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
              
                while (rdr.Read())
                {
                    AuditTeamModel team = new AuditTeamModel();
                    team.ID = Convert.ToInt32(rdr["ID"]);
                    team.T_ID = Convert.ToInt32(rdr["T_ID"]);
                    team.CODE = rdr["T_CODE"].ToString();
                    team.NAME = rdr["TEAM_NAME"].ToString();
                    team.AUDIT_DEPARTMENT = Convert.ToInt32(rdr["PLACE_OF_POSTING"]);
                    team.TEAMMEMBER_ID = Convert.ToInt32(rdr["MEMBER_PPNO"]);
                    team.IS_TEAMLEAD = rdr["ISTEAMLEAD"].ToString();
                    team.PLACE_OF_POSTING = rdr["AUDIT_DEPARTMENT"].ToString();
                    team.EMPLOYEENAME = rdr["MEMBER_NAME"].ToString();
                    team.STATUS = rdr["STATUS"].ToString();
                    teamList.Add(team);
                }
            }
           this.DisposeDatabaseConnection();
            return teamList;
        }
        public AuditTeamModel AddAuditTeam(AuditTeamModel aTeam)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                string _sql = "pkg_ais.P_addauditteam";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("teamname", OracleDbType.Varchar2).Value = aTeam.NAME;
                cmd.Parameters.Add("TEAMMEMBER_ID", OracleDbType.Int32).Value = aTeam.TEAMMEMBER_ID;
                cmd.Parameters.Add("MAX_T_ID", OracleDbType.Int32).Value = aTeam.T_ID;
                cmd.Parameters.Add("EMPLOYEENAME", OracleDbType.Varchar2).Value = aTeam.EMPLOYEENAME;
                cmd.Parameters.Add("IS_TEAMLEAD", OracleDbType.Varchar2).Value = aTeam.IS_TEAMLEAD;
                cmd.Parameters.Add("STATUS", OracleDbType.Varchar2).Value = aTeam.STATUS;
                cmd.Parameters.Add("ENTITY_ID", OracleDbType.Varchar2).Value = loggedInUser.UserEntityID;
                cmd.CommandText = _sql;
                cmd.ExecuteReader();

            }
           this.DisposeDatabaseConnection();
            return aTeam;
        }
         public bool DeleteAuditTeam(string T_CODE)
          {
              if (T_CODE != "" && T_CODE != null)
              {
                  var con = this.DatabaseConnection();
                  using (OracleCommand cmd = con.CreateCommand())
                  {
                      cmd.CommandText = "pkg_ais.P_DeleteAuditTeam";
                      cmd.CommandType = CommandType.StoredProcedure;
                      cmd.Parameters.Clear();
                      cmd.Parameters.Add("TID", OracleDbType.Int32).Value = T_CODE;
                      cmd.ExecuteReader();

                  }
                 this.DisposeDatabaseConnection();
                  return true;
              }
              else
                  return false;
          }
        public int GetLatestTeamID()
        {
            var con = this.DatabaseConnection();
            int maxTeamId = 1;
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_MAXTEAMID";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["MAX_T_ID"].ToString() != null && rdr["MAX_T_ID"].ToString() != "")
                    {
                        maxTeamId = Convert.ToInt32(rdr["MAX_T_ID"]);
                    }
                }

            }
           this.DisposeDatabaseConnection();
            return maxTeamId;
        }
             
        public bool AddAuditCriteria(AddAuditCriteriaModel acm)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            bool isAlreadyAdded = true;
          
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_ADDAUDITCRITERIA";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITYTYPEID", OracleDbType.Int32).Value = acm.ENTITY_TYPEID;
                cmd.Parameters.Add("SIZEID", OracleDbType.Int32).Value = acm.SIZE_ID;
                cmd.Parameters.Add("RISKID", OracleDbType.Int32).Value = acm.RISK_ID;
                cmd.Parameters.Add("FREQUENCYID", OracleDbType.Int32).Value = acm.FREQUENCY_ID;
                cmd.Parameters.Add("NOOFDAYS", OracleDbType.Int32).Value = acm.NO_OF_DAYS;
                cmd.Parameters.Add("visit", OracleDbType.Varchar2).Value = acm.VISIT;
                cmd.Parameters.Add("APPROVALSTATUS", OracleDbType.Int32).Value = acm.APPROVAL_STATUS;
                cmd.Parameters.Add("AUDITPERIODID", OracleDbType.Int32).Value = acm.AUDITPERIODID;
                cmd.Parameters.Add("UserEntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = "AUDIT CRITERIA CREATED";
                cmd.Parameters.Add("CREATEDBY", OracleDbType.Int32).Value =loggedInUser.PPNumber;
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = acm.ENTITY_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["REF"].ToString() != "" && rdr["REF"].ToString() != null && rdr["REF"].ToString() == "1")
                    {
                        isAlreadyAdded = false;
                    }
                   
                }
            }
           this.DisposeDatabaseConnection();
            return !isAlreadyAdded;
        }
        public bool UpdateAuditCriteria(AddAuditCriteriaModel acm, string COMMENTS)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_UpdateAuditCriteria";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("CID", OracleDbType.Int32).Value = acm.ID;
                cmd.Parameters.Add("ENTITY_TYPEID", OracleDbType.Int32).Value = acm.ENTITY_TYPEID;
                cmd.Parameters.Add("SIZE_ID", OracleDbType.Int32).Value = acm.SIZE_ID;
                cmd.Parameters.Add("RISK_ID", OracleDbType.Int32).Value = acm.RISK_ID;
                cmd.Parameters.Add("FREQUENCY_ID", OracleDbType.Int32).Value = acm.FREQUENCY_ID;
                cmd.Parameters.Add("NO_OF_DAYS", OracleDbType.Int32).Value = acm.NO_OF_DAYS;
                cmd.Parameters.Add("VISIT", OracleDbType.Varchar2).Value = acm.VISIT;
                cmd.Parameters.Add("AUDITPERIODID", OracleDbType.Int32).Value = acm.AUDITPERIODID;
                cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = COMMENTS;
                cmd.Parameters.Add("CREATED_BY", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();               
            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public bool SetAuditCriteriaStatusReferredBack(int ID, string REMARKS)
        {
            if (REMARKS == "")
                REMARKS = "REFERRED BACK";


            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_SetAuditCriteriaStatusReferredBack";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("CID", OracleDbType.Int32).Value = ID;
                cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = REMARKS;
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public bool SetAuditCriteriaStatusApprove(int ID, string REMARKS)
        {
            if (REMARKS == "")
                REMARKS = "APPROVED";

            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_SetAuditCriteriaStatusApprove";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("CAID", OracleDbType.Int32).Value = ID;
                cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = REMARKS;
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            //EmailConfiguration email = new EmailConfiguration();
           // email.ConfigEmail();
            return true;
        }
        public string GetAuditCriteriaLogLastStatus(int Id)
        {
            var con = this.DatabaseConnection();
            string remarks = "";
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = sqlParams.GetCriteriaLatestRemarksQueryFromParams(Id);
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    remarks = rdr["remarks"].ToString();
                }
            }
           this.DisposeDatabaseConnection();
            return remarks;
        }
        public List<AuditCriteriaModel> GetPendingAuditCriterias()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            List<AuditCriteriaModel> criteriaList = new List<AuditCriteriaModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetPendingAuditCriterias";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("UserEntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditCriteriaModel acr = new AuditCriteriaModel();
                    acr.ID = Convert.ToInt32(rdr["ID"]);
                    acr.ENTITY_TYPEID = Convert.ToInt32(rdr["ENTITY_TYPEID"]);                    
                    if (rdr["ENTITY_ID"].ToString() != null && rdr["ENTITY_ID"].ToString() != "")
                        acr.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    if (rdr["SIZE_ID"].ToString() != null && rdr["SIZE_ID"].ToString() != "")
                        acr.SIZE_ID = Convert.ToInt32(rdr["SIZE_ID"]);
                    acr.RISK_ID = Convert.ToInt32(rdr["RISK_ID"]);
                    acr.FREQUENCY_ID = Convert.ToInt32(rdr["FREQUENCY_ID"]);
                    acr.NO_OF_DAYS = Convert.ToInt32(rdr["NO_OF_DAYS"]);
                    acr.APPROVAL_STATUS = Convert.ToInt32(rdr["APPROVAL_STATUS"]);
                    acr.AUDITPERIODID = Convert.ToInt32(rdr["AUDITPERIODID"]);
                    acr.PERIOD = rdr["PERIOD"].ToString();
                    acr.ENTITY = rdr["ENTITY"].ToString();
                    acr.ENTITY_NAME = rdr["NAME"].ToString();
                    acr.FREQUENCY = rdr["FREQUENCY"].ToString();
                    acr.SIZE = rdr["BRSIZE"].ToString();
                    acr.RISK = rdr["RISK"].ToString();
                    acr.VISIT = rdr["VISIT"].ToString();
                    acr.COMMENTS = rdr["REMARKS"].ToString();
                    if (rdr["no_of_entity"].ToString() != null && rdr["no_of_entity"].ToString() != "")
                        acr.ENTITIES_COUNT = Convert.ToInt32(rdr["no_of_entity"].ToString());
                    criteriaList.Add(acr);
                }
            }
           this.DisposeDatabaseConnection();
            return criteriaList;
        }
        public List<AuditCriteriaModel> GetRefferedBackAuditCriterias()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

            var con = this.DatabaseConnection();
            List<AuditCriteriaModel> criteriaList = new List<AuditCriteriaModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetRefferedBackAuditCriterias";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("UserEntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();               
                while (rdr.Read())
                {
                    AuditCriteriaModel acr = new AuditCriteriaModel();
                    acr.ID = Convert.ToInt32(rdr["ID"]);
                    acr.ENTITY_TYPEID = Convert.ToInt32(rdr["ENTITY_TYPEID"]);
                    if (rdr["SIZE_ID"].ToString() != null && rdr["SIZE_ID"].ToString() != "")
                        acr.SIZE_ID = Convert.ToInt32(rdr["SIZE_ID"]);
                    if (rdr["ENTITY_ID"].ToString() != null && rdr["ENTITY_ID"].ToString() != "")
                        acr.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    acr.RISK_ID = Convert.ToInt32(rdr["RISK_ID"]);
                    acr.FREQUENCY_ID = Convert.ToInt32(rdr["FREQUENCY_ID"]);
                    acr.NO_OF_DAYS = Convert.ToInt32(rdr["NO_OF_DAYS"]);
                    acr.APPROVAL_STATUS = Convert.ToInt32(rdr["APPROVAL_STATUS"]);
                    acr.AUDITPERIODID = Convert.ToInt32(rdr["AUDITPERIODID"]);
                    acr.PERIOD = rdr["PERIOD"].ToString();
                    acr.ENTITY = rdr["ENTITY"].ToString();
                    acr.FREQUENCY = rdr["FREQUENCY"].ToString();
                    acr.SIZE = rdr["BRSIZE"].ToString();
                    acr.RISK = rdr["RISK"].ToString();
                    acr.ENTITY_NAME = rdr["NAME"].ToString();
                    acr.VISIT = rdr["VISIT"].ToString();
                    acr.COMMENTS = rdr["REMARKS"].ToString();// this.GetAuditCriteriaLogLastStatus(acr.ID);
                    criteriaList.Add(acr);
                }
            }
           this.DisposeDatabaseConnection();
            return criteriaList;
        }
        public List<AuditCriteriaModel> GetAuditCriteriasToAuthorize()
        {
            var con = this.DatabaseConnection();
            List<AuditCriteriaModel> criteriaList = new List<AuditCriteriaModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditCriteriasToAuthorize";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();                
                while (rdr.Read())
                {
                    AuditCriteriaModel acr = new AuditCriteriaModel();
                    acr.ID = Convert.ToInt32(rdr["ID"]);
                    acr.ENTITY_TYPEID = Convert.ToInt32(rdr["ENTITY_TYPEID"]);
                    if (rdr["ENTITY_ID"].ToString() != null && rdr["ENTITY_ID"].ToString() != "")
                        acr.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    if (rdr["SIZE_ID"].ToString() != null && rdr["SIZE_ID"].ToString() != "")
                        acr.SIZE_ID = Convert.ToInt32(rdr["SIZE_ID"]);
                    acr.RISK_ID = Convert.ToInt32(rdr["RISK_ID"]);
                    acr.FREQUENCY_ID = Convert.ToInt32(rdr["FREQUENCY_ID"]);
                    acr.NO_OF_DAYS = Convert.ToInt32(rdr["NO_OF_DAYS"]);
                    acr.APPROVAL_STATUS = Convert.ToInt32(rdr["APPROVAL_STATUS"]);
                    acr.AUDITPERIODID = Convert.ToInt32(rdr["AUDITPERIODID"]);
                    acr.PERIOD = rdr["PERIOD"].ToString();
                    acr.ENTITY = rdr["ENTITY"].ToString();
                    acr.FREQUENCY = rdr["FREQUENCY"].ToString();
                    acr.SIZE = rdr["BRSIZE"].ToString();
                    acr.RISK = rdr["RISK"].ToString();
                    acr.VISIT = rdr["VISIT"].ToString();
                    acr.ENTITY_NAME = rdr["NAME"].ToString();
                    acr.COMMENTS = rdr["REMARKS"].ToString();// this.GetAuditCriteriaLogLastStatus(acr.ID);
                    acr.ENTITIES_COUNT = this.GetExpectedCountOfAuditEntitiesOnCriteria(acr.RISK_ID, acr.SIZE_ID, acr.ENTITY_TYPEID, acr.AUDITPERIODID, acr.FREQUENCY_ID);
                    criteriaList.Add(acr);
                }
            }
           this.DisposeDatabaseConnection();
            return criteriaList;
        }
        public List<AuditCriteriaModel> GetPostChangesAuditCriterias()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            List<AuditCriteriaModel> criteriaList = new List<AuditCriteriaModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {

                cmd.CommandText = "pkg_ais.P_GetPostChangesAuditCriterias";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("UserEntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    AuditCriteriaModel acr = new AuditCriteriaModel();
                    acr.ID = Convert.ToInt32(rdr["ID"]);
                    acr.ENTITY_TYPEID = Convert.ToInt32(rdr["ENTITY_TYPEID"]);
                    if (rdr["SIZE_ID"].ToString() != null && rdr["SIZE_ID"].ToString() != "")
                        acr.SIZE_ID = Convert.ToInt32(rdr["SIZE_ID"]);
                    if (rdr["ENTITY_ID"].ToString() != null && rdr["ENTITY_ID"].ToString() != "")
                        acr.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    acr.RISK_ID = Convert.ToInt32(rdr["RISK_ID"]);
                    acr.FREQUENCY_ID = Convert.ToInt32(rdr["FREQUENCY_ID"]);
                    acr.NO_OF_DAYS = Convert.ToInt32(rdr["NO_OF_DAYS"]);
                    acr.APPROVAL_STATUS = Convert.ToInt32(rdr["APPROVAL_STATUS"]);
                    acr.AUDITPERIODID = Convert.ToInt32(rdr["AUDITPERIODID"]);
                    acr.PERIOD = rdr["PERIOD"].ToString();
                    acr.ENTITY = rdr["ENTITY"].ToString();
                    acr.FREQUENCY = rdr["FREQUENCY"].ToString();
                    acr.SIZE = rdr["BRSIZE"].ToString();
                    acr.RISK = rdr["RISK"].ToString();
                    acr.VISIT = rdr["VISIT"].ToString();
                    acr.ENTITY_NAME = rdr["NAME"].ToString();
                    acr.COMMENTS = rdr["REMARKS"].ToString();// this.GetAuditCriteriaLogLastStatus(acr.ID);
                    criteriaList.Add(acr);
                }
            }
           this.DisposeDatabaseConnection();
            return criteriaList;
        }
        public List<AuditeeEntitiesModel> GetAuditeeEntitiesForOutstandingParas(int ENTITY_CODE = 0)
        {
            //Functionality Completed, no further need of this code is required as of now...
            // Once Page will be removed, this function will be removed as well
            List<AuditeeEntitiesModel> entitiesList = new List<AuditeeEntitiesModel>();
            return entitiesList;           
        }
        public string GeneratePlanForAuditCriteria(int CRITERIA_ID)
        {
            string resMsg = "";
            
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                 cmd.CommandText = "pkg_ais.Tentative_Audit_Plan";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("CRITERIA_ID", OracleDbType.Int32).Value = CRITERIA_ID;               
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    resMsg=rdr["REMARKS"].ToString();
                }

            }
           this.DisposeDatabaseConnection();
            return resMsg;
        }
        public List<RoleRespModel> GetRoleResponsibilities()
        {
            var con = this.DatabaseConnection();
            List<RoleRespModel> groupList = new List<RoleRespModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetRoleResponsibilities";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    RoleRespModel grp = new RoleRespModel();
                    grp.DESIGNATIONCODE = Convert.ToInt32(rdr["DESIGNATIONCODE"]);
                    grp.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    grp.STATUS = rdr["STATUSTYPE"].ToString();
                    groupList.Add(grp);
                }
            }
           this.DisposeDatabaseConnection();
            return groupList;
        }
        public List<UserModel> GetAllUsers(FindUserModel user)
        {
            List<UserModel> userList = new List<UserModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_get_allusers";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = user.ENTITYID;
                cmd.Parameters.Add("EMAIL", OracleDbType.Varchar2).Value = user.EMAIL;
                cmd.Parameters.Add("GROUPID", OracleDbType.Int32).Value = user.GROUPID;
                cmd.Parameters.Add("PPNUMBER", OracleDbType.Int32).Value = user.PPNUMBER;
                cmd.Parameters.Add("LOGINNAME", OracleDbType.Int32).Value = user.LOGINNAME;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserModel um = new UserModel();
                    if (rdr["USERID"].ToString() != null && rdr["USERID"].ToString() != "")
                        um.ID = Convert.ToInt32(rdr["USERID"]);
                    um.PPNumber = rdr["PPNO"].ToString();
                    um.Name = rdr["EMPLOYEEFIRSTNAME"].ToString() + " " + rdr["EMPLOYEELASTNAME"].ToString();
                    um.Email = rdr["EMAIL"].ToString();
                    if (rdr["entity_id"].ToString() != null && rdr["entity_id"].ToString() != "")
                        um.UserEntityID = Convert.ToInt32(rdr["entity_id"].ToString());

                    if (rdr["child_code"].ToString() != null && rdr["child_code"].ToString() != "")
                        um.UserEntityCode = Convert.ToInt32(rdr["child_code"].ToString());

                    if (rdr["p_type_id"].ToString() != null && rdr["p_type_id"].ToString() != "")
                        um.UserParentEntityTypeID = Convert.ToInt32(rdr["p_type_id"].ToString());

                    if (rdr["parent_id"].ToString() != null && rdr["parent_id"].ToString() != "")
                        um.UserParentEntityID = Convert.ToInt32(rdr["parent_id"].ToString());

                    if (rdr["parent_code"].ToString() != null && rdr["parent_code"].ToString() != "")
                        um.UserParentEntityCode = Convert.ToInt32(rdr["parent_code"].ToString());

                    if (rdr["c_type_id"].ToString() != null && rdr["c_type_id"].ToString() != "")
                        um.UserEntityTypeID = Convert.ToInt32(rdr["c_type_id"].ToString());

                    um.UserEntityName = rdr["c_name"].ToString();
                    um.UserParentEntityName = rdr["p_name"].ToString();
                    if (Convert.ToInt32(rdr["type_id"].ToString()) == 6)
                    {
                        if (rdr["code"].ToString() != null && rdr["code"].ToString() != "")
                        {
                            um.UserPostingBranch = Convert.ToInt32(rdr["code"]);
                        }
                        if (rdr["parent_code"].ToString() != null && rdr["parent_code"].ToString() != "")
                        {
                            um.UserPostingZone = Convert.ToInt32(rdr["parent_code"]);
                        }

                    }
                    else
                    {
                        if (rdr["code"].ToString() != null && rdr["code"].ToString() != "")
                        {
                            um.UserPostingDept = Convert.ToInt32(rdr["code"]);
                        }
                        if (rdr["parent_code"].ToString() != null && rdr["parent_code"].ToString() != "")
                        {
                            um.UserPostingDiv = Convert.ToInt32(rdr["parent_code"]);
                        }

                    }

                    if (rdr["group_id"].ToString() != null && rdr["group_id"].ToString() != "")
                    {
                        um.UserGroupID = Convert.ToInt32(rdr["group_id"]);
                        um.UserRoleID = Convert.ToInt32(rdr["group_id"]);
                    }


                    um.DivName = rdr["p_name"].ToString();
                    um.DeptName = rdr["c_name"].ToString();
                    um.ZoneName = rdr["p_name"].ToString();
                    um.BranchName = rdr["c_name"].ToString();
                    um.UserRole = rdr["group_name"].ToString();
                    um.UserGroup = rdr["group_name"].ToString();
                    um.IsActive = rdr["ISACTIVE"].ToString();
                    userList.Add(um);
                }
            }
           this.DisposeDatabaseConnection();
            return userList;

        }
        public List<AuditEntitiesModel> GetAuditEntities()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;

            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();

            List<AuditEntitiesModel> entitiesList = new List<AuditEntitiesModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditEntities";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditEntitiesModel entity = new AuditEntitiesModel();
                    entity.AUTID = Convert.ToInt32(rdr["AUTID"]);
                    entity.ENTITYCODE = rdr["ENTITYCODE"].ToString();
                    entity.ENTITYTYPEDESC = rdr["ENTITYTYPEDESC"].ToString();
                    entity.AUDITABLE = rdr["AUDITABLE"].ToString();
                    entitiesList.Add(entity);
                }
            }
           this.DisposeDatabaseConnection();
            return entitiesList;

        }
        public List<AuditeeEntitiesModel> GetAuditeeEntitiesForOldParas(int ENTITY_ID = 0)
        {
            List<AuditeeEntitiesModel> entitiesList = new List<AuditeeEntitiesModel>();
            var con = this.DatabaseConnection();
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditeeEntitiesForOldParas";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITY_ID", OracleDbType.Int32).Value = ENTITY_ID;
                cmd.Parameters.Add("UserEntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;

                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    AuditeeEntitiesModel entity = new AuditeeEntitiesModel();
                    if (rdr["ENTITY_ID"].ToString() != "" && rdr["ENTITY_ID"].ToString() != null)
                        entity.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    if (rdr["entity_code"].ToString() != "" && rdr["entity_code"].ToString() != null)
                        entity.CODE = Convert.ToInt32(rdr["entity_code"]);
                    if (rdr["entity_name"].ToString() != "" && rdr["entity_name"].ToString() != null)
                        entity.NAME = rdr["entity_name"].ToString();

                    entitiesList.Add(entity);
                }
            }
           this.DisposeDatabaseConnection();
            return entitiesList;

        }
        public List<AuditeeEntitiesModel> GetAuditeeEntities(int ENTITY_TYPE_ID = 0)
        {
            var con = this.DatabaseConnection();
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            List<AuditeeEntitiesModel> entitiesList = new List<AuditeeEntitiesModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditeeEntities";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = loggedInUser.UserEntityID; 
                cmd.Parameters.Add("TYPEID", OracleDbType.Int32).Value = ENTITY_TYPE_ID;                
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    AuditeeEntitiesModel entity = new AuditeeEntitiesModel();
                    if (rdr["ENTITY_ID"].ToString() != "" && rdr["ENTITY_ID"].ToString() != null)
                        entity.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    
                    if (rdr["name"].ToString() != "" && rdr["name"].ToString() != null)
                        entity.NAME = rdr["name"].ToString();

                    entitiesList.Add(entity);
                }
            }
           this.DisposeDatabaseConnection();
            return entitiesList;

        }
        public List<AuditeeEntitiesModel> GetAuditeeEntitiesForUpdate(int ENTITY_TYPE_ID = 0, int ENTITY_ID=0)
        {
            var con = this.DatabaseConnection();
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            List<AuditeeEntitiesModel> entitiesList = new List<AuditeeEntitiesModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetDepartments";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = ENTITY_ID;
                cmd.Parameters.Add("TYPEID", OracleDbType.Int32).Value = ENTITY_TYPE_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    AuditeeEntitiesModel entity = new AuditeeEntitiesModel();
                    if (rdr["ENTITY_ID"].ToString() != "" && rdr["ENTITY_ID"].ToString() != null)
                        entity.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);

                    if (rdr["name"].ToString() != "" && rdr["name"].ToString() != null)
                        entity.NAME = rdr["name"].ToString();

                    entitiesList.Add(entity);
                }
            }
           this.DisposeDatabaseConnection();
            return entitiesList;

        }
        public AuditEntitiesModel AddAuditEntity(AuditEntitiesModel am)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_AddAuditEntity";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("AUDITABLE", OracleDbType.Varchar2).Value = am.AUDITABLE;
                cmd.Parameters.Add("ENTITYTYPEDESC", OracleDbType.Varchar2).Value = am.ENTITYTYPEDESC;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return am;

        }
        public List<AuditSubEntitiesModel> GetAuditSubEntities()
        {
            List<AuditSubEntitiesModel> subEntitiesList = new List<AuditSubEntitiesModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditSubEntities";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditSubEntitiesModel entity = new AuditSubEntitiesModel();
                    entity.ID = Convert.ToInt32(rdr["ID"]);
                    entity.DIV_ID = Convert.ToInt32(rdr["DIV_ID"]);
                    entity.DEP_ID = Convert.ToInt32(rdr["DEP_ID"]);
                    entity.NAME = rdr["E_NAME"].ToString();
                    entity.STATUS = rdr["STATUS"].ToString();
                    subEntitiesList.Add(entity);
                }
            }
           this.DisposeDatabaseConnection();
            return subEntitiesList;

        }
        public UpdateUserModel UpdateUser(UpdateUserModel user)
        {
            var newPassword = "";
            bool setPassword = false;
            if(user.PASSWORD != "" && user.PASSWORD != null)
            {
                newPassword = getMd5Hash(user.PASSWORD);
                setPassword = !setPassword;
            }

            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.UPDATE_USERS";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNUMBER", OracleDbType.Int32).Value = user.PPNO;
                if (setPassword)
                    cmd.Parameters.Add("PASS", OracleDbType.Varchar2).Value = newPassword;
                else
                    cmd.Parameters.Add("PASS", OracleDbType.Varchar2).Value = newPassword;
                cmd.Parameters.Add("ISACTIVE", OracleDbType.Varchar2).Value = user.ISACTIVE;
                cmd.Parameters.Add("ROLEID", OracleDbType.Int32).Value = user.ROLE_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            user.PASSWORD = "";
            return user;
        }
        public bool ChangePassword(string Password, string NewPassowrd)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;

            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            var enc_pass = getMd5Hash(Password);
            bool correctPass = false;
            bool res = false;
            var enc_new_pass = getMd5Hash(NewPassowrd);
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_get_user";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PP_NO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("enc_pass", OracleDbType.Varchar2).Value = enc_pass;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["USERID"].ToString() != null && rdr["USERID"].ToString() != "")
                    {
                        correctPass = true;
                        res = true;
                    }

                }
                if (correctPass)
                {
                    cmd.CommandText = "pkg_ais.P_ChangePassword";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("PPNO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                    cmd.Parameters.Add("enc_pass", OracleDbType.Varchar2).Value = enc_new_pass;
                    cmd.ExecuteReader();
                    res = true;
                }
            }
           this.DisposeDatabaseConnection();
            return res;
        }
        public void AddGroupMenuItemsAssignment(int group_id = 0, int menu_item_id = 0)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_AddGroupMenuItemsAssignment";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("groupid", OracleDbType.Int32).Value = group_id;
                cmd.Parameters.Add("PAGEID", OracleDbType.Int32).Value = menu_item_id;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
        }
        public void RemoveGroupMenuItemsAssignment(int group_id = 0, int menu_item_id = 0)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_RemoveGroupMenuItemsAssignment";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("groupid", OracleDbType.Int32).Value = group_id;
                cmd.Parameters.Add("PAGEID", OracleDbType.Int32).Value = menu_item_id;
               cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
        }
        public List<AuditZoneModel> GetAuditZones(bool sessionCheck = true)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            List<AuditZoneModel> AZList = new List<AuditZoneModel>();
            var loggedInUser = sessionHandler.GetSessionUser();
            int entityId = 0;
            if (loggedInUser.UserGroupID != 1)
            {
                if (sessionCheck)
                    entityId =Convert.ToInt32(loggedInUser.PPNumber);              
            }
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditZones";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = entityId;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditZoneModel z = new AuditZoneModel();
                    z.ID = Convert.ToInt32(rdr["ID"]);
                    z.ZONECODE = rdr["ZONECODE"].ToString();
                    z.ZONENAME = rdr["ZONENAME"].ToString();
                    z.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    if (rdr["ISACTIVE"].ToString() == "A")
                        z.ISACTIVE = "Active";
                    else if (rdr["ISACTIVE"].ToString() == "I")
                        z.ISACTIVE = "InActive";
                    else
                        z.ISACTIVE = rdr["ISACTIVE"].ToString();

                    AZList.Add(z);
                }
            }
           this.DisposeDatabaseConnection();
            return AZList;
        }
        public List<InspectionUnitsModel> GetInspectionUnits()
        {
            var con = this.DatabaseConnection();
            List<InspectionUnitsModel> ICList = new List<InspectionUnitsModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {

                cmd.CommandText = "pkg_ais.P_GetInspectionUnits";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    InspectionUnitsModel z = new InspectionUnitsModel();
                    z.I_ID = Convert.ToInt32(rdr["I_ID"]);
                    z.I_CODE = rdr["I_CODE"].ToString();
                    z.UNIT_NAME = rdr["UNIT_NAME"].ToString();
                    z.DISCRIPTION = rdr["DISCRIPTION"].ToString();
                    if (rdr["STATUS"].ToString() == "Y")
                        z.STATUS = "Active";
                    else if (rdr["STATUS"].ToString() == "N")
                        z.STATUS = "InActive";
                    else
                        z.STATUS = rdr["ISACTIVE"].ToString();

                    ICList.Add(z);
                }
            }
           this.DisposeDatabaseConnection();
            return ICList;
        }
        public List<BranchModel> GetBranches(int zone_code = 0, bool sessionCheck = true)
        {
            var con = this.DatabaseConnection();
            List<BranchModel> branchList = new List<BranchModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetBranches";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("Zone_Id", OracleDbType.Int32).Value = zone_code;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    BranchModel br = new BranchModel();
                    br.BRANCHID = Convert.ToInt32(rdr["BRANCHID"]);
                    br.ZONEID = Convert.ToInt32(rdr["ZONEID"]);
                    br.BRANCHNAME = rdr["BRANCHNAME"].ToString();
                    br.ZONE_NAME = rdr["ZONENAME"].ToString();
                    br.BRANCHCODE = rdr["BRANCHCODE"].ToString();
                    br.BRANCH_SIZE_ID = 1;
                    br.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    br.BRANCH_SIZE = "";
                    if (rdr["ISACTIVE"].ToString() == "Y")
                        br.ISACTIVE = "Active";
                    else if (rdr["ISACTIVE"].ToString() == "N")
                        br.ISACTIVE = "InActive";
                    else
                        br.ISACTIVE = rdr["ISACTIVE"].ToString();

                    branchList.Add(br);
                }
            }
           this.DisposeDatabaseConnection();
            return branchList;
        }
        public BranchModel AddBranch(BranchModel br)
        {
            return br;
        }
        public BranchModel UpdateBranch(BranchModel br)
        {
           return br;
        }
        public List<ZoneModel> GetZones(bool sessionCheck = true)
        {
            var con = this.DatabaseConnection();
            List<ZoneModel> zoneList = new List<ZoneModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetZones";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ZoneModel z = new ZoneModel();
                    z.ZONEID = Convert.ToInt32(rdr["ZONEID"]);
                    z.ZONECODE = rdr["ZONECODE"].ToString();
                    z.ZONENAME = rdr["ZONENAME"].ToString();
                    z.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    if (rdr["ISACTIVE"].ToString() == "Y")
                        z.ISACTIVE = "Active";
                    else if (rdr["ISACTIVE"].ToString() == "N")
                        z.ISACTIVE = "InActive";
                    else
                        z.ISACTIVE = rdr["ISACTIVE"].ToString();

                    zoneList.Add(z);
                }
            }
           this.DisposeDatabaseConnection();
            return zoneList;
        }
        public List<BranchSizeModel> GetBranchSizes()
        {
            var con = this.DatabaseConnection();
            List<BranchSizeModel> brSizeList = new List<BranchSizeModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetBranchSizes";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    BranchSizeModel bs = new BranchSizeModel();
                    bs.BR_SIZE_ID = Convert.ToInt32(rdr["ENTITY_SIZE"]);
                    bs.DESCRIPTION = rdr["DESCRIPTION"].ToString();

                    brSizeList.Add(bs);
                }
            }
           this.DisposeDatabaseConnection();
            return brSizeList;
        }
        public List<ControlViolationsModel> GetControlViolations()
        {
            var con = this.DatabaseConnection();
            List<ControlViolationsModel> controlViolationList = new List<ControlViolationsModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetControlViolations";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ControlViolationsModel v = new ControlViolationsModel();
                    v.ID = Convert.ToInt32(rdr["S_GR_ID"]);
                    v.V_NAME = rdr["DESCRIPTION"].ToString();
                    if (rdr["MAX_NUMBER"].ToString() != null && rdr["MAX_NUMBER"].ToString() != "")
                        v.MAX_NUMBER = Convert.ToInt32(rdr["MAX_NUMBER"]);
                    v.STATUS = "Y";
                    controlViolationList.Add(v);
                }
            }
           this.DisposeDatabaseConnection();
            return controlViolationList;
        }
        public ControlViolationsModel AddControlViolation(ControlViolationsModel cv)
        {
            return cv;
        }
        public List<DivisionModel> GetDivisions(bool sessionCheck = true)
        {
            var con = this.DatabaseConnection();
            List<DivisionModel> divList = new List<DivisionModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = sqlParams.GetDivisionQueryFromParams();
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DivisionModel div = new DivisionModel();
                    div.DIVISIONID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    div.NAME = rdr["NAME"].ToString();
                    div.CODE = rdr["CODE"].ToString();
                    div.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    if (rdr["ACTIVE"].ToString() == "Y")
                        div.ISACTIVE = "Active";
                    else if (rdr["ACTIVE"].ToString() == "N")
                        div.ISACTIVE = "InActive";
                    else
                        div.ISACTIVE = rdr["ACTIVE"].ToString();
                    divList.Add(div);
                }
            }
           this.DisposeDatabaseConnection();
            return divList;
        }
        public DivisionModel AddDivision(DivisionModel div)
        {

            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO T_DIVISION d (d.ID,d.CODE, d.NAME, d.DESCRIPTION, d.STATUS) VALUES ( '" + div.CODE + "','" + div.CODE + "','" + div.NAME + "','" + div.DESCRIPTION + "','" + div.ISACTIVE + "')";
                OracleDataReader rdr = cmd.ExecuteReader();

            }
           this.DisposeDatabaseConnection();
            return div;
        }
        public DivisionModel UpdateDivision(DivisionModel div)
        {
           return div;
        }
        public List<DepartmentModel> GetDepartments(int div_code = 0, bool sessionCheck = true)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            List<DepartmentModel> deptList = new List<DepartmentModel>();
            var loggedInUser = sessionHandler.GetSessionUser();

          
            
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais_reports.R_GetDepartments";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("EntityId", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("PPNUM", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    DepartmentModel dept = new DepartmentModel();
                    dept.ID = Convert.ToInt32(rdr["ID"]);
                    dept.DIV_ID = Convert.ToInt32(rdr["DIVISIONID"]);
                    dept.NAME = rdr["NAME"].ToString();
                    dept.CODE = rdr["CODE"].ToString();
                    if (rdr["ISACTIVE"].ToString() == "Y")
                        dept.STATUS = "Active";
                    else if (rdr["ISACTIVE"].ToString() == "N")
                        dept.STATUS = "InActive";
                    else
                        dept.STATUS = rdr["ISACTIVE"].ToString();
                    dept.DIV_NAME = rdr["DIV_NAME"].ToString();
                    if (rdr["AUDITED_BY_DEPID"].ToString() != null && rdr["AUDITED_BY_DEPID"].ToString() != "")
                    {
                        // dept.AUDITED_BY_NAME = rdr["ADUTIED_BY"].ToString();
                        
                        dept.AUDITED_BY_DEPID = Convert.ToInt32(rdr["AUDITED_BY_DEPID"]);
                        cmd.Parameters.Clear();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sqlParams.GetDepartmentNameByIdQueryFromParams(dept.AUDITED_BY_DEPID);
                        OracleDataReader rdr2 = cmd.ExecuteReader();
                        while (rdr2.Read())
                        {
                            dept.AUDITED_BY_NAME = rdr2["NAME"].ToString();
                        }
                    }
                    deptList.Add(dept);
                }
            }
           this.DisposeDatabaseConnection();
            return deptList;
        }
        public List<SubEntitiesModel> GetSubEntities(int div_code = 0, int dept_code = 0)
        {
            var con = this.DatabaseConnection();
            List<SubEntitiesModel> entitiesList = new List<SubEntitiesModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetSubEntities";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("dept_code", OracleDbType.Varchar2).Value = dept_code;
                cmd.Parameters.Add("Div_id", OracleDbType.Varchar2).Value = div_code;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    SubEntitiesModel entity = new SubEntitiesModel();
                    entity.ID = Convert.ToInt32(rdr["ID"]);
                    entity.DIV_ID = Convert.ToInt32(rdr["DIV_ID"]);
                    entity.DEP_ID = Convert.ToInt32(rdr["DEP_ID"]);
                    entity.NAME = rdr["NAME"].ToString();
                    entity.Division_Name = rdr["DIV_NAME"].ToString();
                    entity.Department_Name = rdr["DEPT_NAME"].ToString();
                    if (rdr["STATUS"].ToString() == "Y")
                        entity.STATUS = "Active";
                    else if (rdr["STATUS"].ToString() == "N")
                        entity.STATUS = "InActive";
                    else
                        entity.STATUS = rdr["STATUS"].ToString();
                    entitiesList.Add(entity);
                }
            }
           this.DisposeDatabaseConnection();
            return entitiesList;
        }
        public SubEntitiesModel AddSubEntity(SubEntitiesModel subentity)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_AddSubEntity";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = subentity.NAME;
                cmd.Parameters.Add("DIV_ID", OracleDbType.Int32).Value = subentity.DIV_ID;
                cmd.Parameters.Add("DEP_ID", OracleDbType.Int32).Value = subentity.DEP_ID;
                cmd.Parameters.Add("STATUS", OracleDbType.Varchar2).Value = subentity.STATUS;

                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return subentity;
        }
        public SubEntitiesModel UpdateSubEntity(SubEntitiesModel subentity)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_UpdateSubEntity";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("E_id", OracleDbType.Int32).Value = subentity.ID;
                cmd.Parameters.Add("NAME", OracleDbType.Varchar2).Value = subentity.NAME;
                cmd.Parameters.Add("DIV_ID", OracleDbType.Int32).Value = subentity.DIV_ID;
                cmd.Parameters.Add("DEP_ID", OracleDbType.Int32).Value = subentity.DEP_ID;
                cmd.Parameters.Add("STATUS", OracleDbType.Varchar2).Value = subentity.STATUS;

                cmd.ExecuteReader();
             
            }
           this.DisposeDatabaseConnection();
            return subentity;
        }
        public DepartmentModel AddDepartment(DepartmentModel dept)
        {
            return dept;
        }
        public DepartmentModel UpdateDepartment(DepartmentModel dept)
        {

            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "UPDATE t_auditee_entities_maping  mp SET mp.AUDITEDBY='" + dept.AUDITED_BY_DEPID + "' WHERE mp.CODE=" + dept.CODE;
                OracleDataReader rdr = cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return dept;
        }
        public List<RiskGroupModel> GetRiskGroup()
        {
            var con = this.DatabaseConnection();
            List<RiskGroupModel> riskgroupList = new List<RiskGroupModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetRiskGroup";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
               
                while (rdr.Read())
                {
                    RiskGroupModel rgm = new RiskGroupModel();
                    rgm.GR_ID = Convert.ToInt32(rdr["GR_ID"]);
                    rgm.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    riskgroupList.Add(rgm);
                }
            }

           this.DisposeDatabaseConnection();
            return riskgroupList;
        }
        public List<RiskSubGroupModel> GetRiskSubGroup(int group_id)
        {
            var con = this.DatabaseConnection();
            List<RiskSubGroupModel> risksubgroupList = new List<RiskSubGroupModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetRiskSubGroup";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("group_id", OracleDbType.Int32).Value = group_id;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    RiskSubGroupModel rsgm = new RiskSubGroupModel();
                    rsgm.S_GR_ID = Convert.ToInt32(rdr["S_GR_ID"]);
                    rsgm.GR_ID = Convert.ToInt32(rdr["GR_ID"]);
                    rsgm.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    rsgm.GROUP_DESC = rdr["GROUP_DESC"].ToString();
                    risksubgroupList.Add(rsgm);
                }
            }
           this.DisposeDatabaseConnection();
            return risksubgroupList;
        }
        public List<RiskActivityModel> GetRiskActivities(int Sub_group_id)
        {
            var con = this.DatabaseConnection();
            List<RiskActivityModel> riskActivityList = new List<RiskActivityModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_GetRiskActivities";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("Sub_group_id", OracleDbType.Int32).Value = Sub_group_id;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    RiskActivityModel ram = new RiskActivityModel();
                    ram.S_GR_ID = Convert.ToInt32(rdr["S_GR_ID"]);
                    ram.ACTIVITY_ID = Convert.ToInt32(rdr["ACTIVITY_ID"]);
                    ram.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    ram.SUB_GROUP_DESC = rdr["SUB_GROUP_DESC"].ToString();
                    riskActivityList.Add(ram);
                }
            }
           this.DisposeDatabaseConnection();
            return riskActivityList;
        }
        public List<AuditObservationTemplateModel> GetAuditObservationTemplates(int activity_id)
        {
            List<AuditObservationTemplateModel> templateList = new List<AuditObservationTemplateModel>();           
            return templateList;
        }
        public List<AuditEmployeeModel> GetAuditEmployees(int dept_code = 0)
        {
            var con = this.DatabaseConnection();
            List<AuditEmployeeModel> empList = new List<AuditEmployeeModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditEmployees";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("dept_code", OracleDbType.Int32).Value = dept_code;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditEmployeeModel emp = new AuditEmployeeModel();
                    emp.PPNO = Convert.ToInt32(rdr["PPNO"]);
                    emp.DEPARTMENTCODE = Convert.ToInt32(rdr["DEPARTMENTCODE"]);
                    emp.RANKCODE = Convert.ToInt32(rdr["RANKCODE"]);
                    emp.DESIGNATIONCODE = Convert.ToInt32(rdr["DESIGNATIONCODE"]);

                    emp.DEPTARMENT = rdr["DEPTARMENT"].ToString();
                    emp.EMPLOYEEFIRSTNAME = rdr["EMPLOYEEFIRSTNAME"].ToString();
                    emp.EMPLOYEELASTNAME = rdr["EMPLOYEELASTNAME"].ToString();
                    emp.CURRENT_RANK = rdr["CURRENT_RANK"].ToString();
                    emp.FUN_DESIGNATION = rdr["FUN_DESIGNATION"].ToString();
                    emp.TYPE = rdr["TYPE"].ToString();
                    empList.Add(emp);
                }
            }
           this.DisposeDatabaseConnection();
            return empList;
        }
        public List<TentativePlanModel> GetTentativePlansForFields(bool sessionCheck = true)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<TentativePlanModel> tplansList = new List<TentativePlanModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                string _sql = "pkg_ais.p_get_audit_plan";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("AUDITED_BY", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                cmd.CommandText = _sql;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    TentativePlanModel tplan = new TentativePlanModel();
                    tplan.PLAN_ID = Convert.ToInt32(rdr["PLAN_ID"]);
                    tplan.CRITERIA_ID = Convert.ToInt32(rdr["CRITERIA_ID"]);
                    tplan.AUDIT_PERIOD_ID = Convert.ToInt32(rdr["AUDITPERIODID"]);
                    tplan.AUDITEDBY = Convert.ToInt32(rdr["AUDITEDBY"]);
                    tplan.BR_SIZE = rdr["AUDITEE_SIZE"].ToString();
                    tplan.RISK = rdr["AUDITEE_RISK"].ToString();
                    tplan.NO_OF_DAYS = Convert.ToInt32(rdr["NO_OF_DAYS"]);
                    tplan.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    tplan.CODE = rdr["ENTITY_CODE"].ToString();
                    tplan.ENTITY_TYPE_ID = Convert.ToInt32(rdr["ENTITY_TYPE_ID"].ToString());
                    tplan.ENTITY_NAME = rdr["AUDITEE_NAME"].ToString();
                    tplan.FREQUENCY_DESCRIPTION = rdr["FREQUENCY_DISCRIPTION"].ToString();
                    tplan.PERIOD_NAME = rdr["PERIOD_NAME"].ToString();
                    tplan.REPORTING_OFFICE = rdr["REPORTING_OFFICE"].ToString();
                    tplansList.Add(tplan);
                }
            }
           this.DisposeDatabaseConnection();
            return tplansList;
        }
        public string GetAuditOperationalStartDate(int auditPeriodId = 0, int entityCode = 0)
        {
            string result = "";
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditOperationalStartDate";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("entityCode", OracleDbType.Int32).Value = entityCode;
                cmd.Parameters.Add("auditPeriodId", OracleDbType.Int32).Value = auditPeriodId;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result = rdr["YEAR"].ToString() + "-";
                    result += rdr["MONTH"].ToString() + "-";
                    result += rdr["DAY"].ToString();
                }
            }
           this.DisposeDatabaseConnection();
            return result;
        }
        public List<AuditEngagementPlanModel> GetAuditEngagementPlans()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            List<AuditEngagementPlanModel> list = new List<AuditEngagementPlanModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditEngagementPlans";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("EntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader ardr = cmd.ExecuteReader();
                while (ardr.Read())
                {
                    AuditEngagementPlanModel eng = new AuditEngagementPlanModel();
                    eng.ENG_ID = Convert.ToInt32(ardr["eng_id"].ToString());
                    eng.TEAM_NAME = ardr["team_name"].ToString();
                    eng.ENTITY_NAME = ardr["name"].ToString();
                    eng.AUDIT_STARTDATE = Convert.ToDateTime(ardr["audit_startdate"].ToString());
                    eng.AUDIT_ENDDATE = Convert.ToDateTime(ardr["audit_enddate"].ToString());
                    eng.OP_STARTDATE = Convert.ToDateTime(ardr["op_startdate"].ToString());
                    eng.OP_ENDDATE = Convert.ToDateTime(ardr["op_enddate"].ToString());
                    eng.ENTITY_ID = Convert.ToInt32(ardr["entity_id"].ToString());
                    list.Add(eng);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<AuditEngagementPlanModel> GetRefferedBackAuditEngagementPlans()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            List<AuditEngagementPlanModel> list = new List<AuditEngagementPlanModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetRefferedBackAuditEngagementPlans";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("EntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader ardr = cmd.ExecuteReader();
                while (ardr.Read())
                {
                    AuditEngagementPlanModel eng = new AuditEngagementPlanModel();
                    eng.PLAN_ID = Convert.ToInt32(ardr["plan_id"].ToString());
                    eng.ENG_ID = Convert.ToInt32(ardr["eng_id"].ToString());
                    eng.TEAM_NAME = ardr["team_name"].ToString();
                    eng.TEAM_ID = Convert.ToInt32(ardr["team_id"].ToString());
                    eng.ENTITY_NAME = ardr["name"].ToString();
                    eng.COMMENTS = this.GetLatestCommentsOnEngagement(Convert.ToInt32(eng.ENG_ID)).ToString();
                    eng.AUDIT_STARTDATE = Convert.ToDateTime(ardr["audit_startdate"].ToString());
                    eng.AUDIT_ENDDATE = Convert.ToDateTime(ardr["audit_enddate"].ToString());
                    eng.OP_STARTDATE = Convert.ToDateTime(ardr["op_startdate"].ToString());
                    eng.OP_ENDDATE = Convert.ToDateTime(ardr["op_enddate"].ToString());

                    eng.ENTITY_ID = Convert.ToInt32(ardr["entity_id"].ToString());
                    list.Add(eng);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public AuditEngagementPlanModel AddAuditEngagementPlan(AuditEngagementPlanModel ePlan)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            ePlan.CREATED_ON = System.DateTime.Now;
            int placeofposting = Convert.ToInt32(loggedInUser.UserEntityID);
            bool isContinue = false;
           
            ePlan.CREATEDBY = Convert.ToInt32(loggedInUser.PPNumber);
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_AddAuditEngagementPlan";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PERIODID", OracleDbType.Int32).Value = ePlan.PERIOD_ID;
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = ePlan.ENTITY_ID;
                cmd.Parameters.Add("AUDIT_STARTDATE", OracleDbType.Date).Value = ePlan.AUDIT_STARTDATE;
                cmd.Parameters.Add("CREATEDBY", OracleDbType.Int32).Value = ePlan.CREATEDBY;
                cmd.Parameters.Add("AUDIT_ENDDATE", OracleDbType.Date).Value = ePlan.AUDIT_ENDDATE;
                cmd.Parameters.Add("STATUS", OracleDbType.Varchar2).Value = ePlan.STATUS;
                cmd.Parameters.Add("TEAMID", OracleDbType.Int32).Value = ePlan.TEAM_ID;
                cmd.Parameters.Add("TEAM_NAME", OracleDbType.Varchar2).Value = ePlan.TEAM_NAME;
                cmd.Parameters.Add("PLANID", OracleDbType.Int32).Value = ePlan.PLAN_ID;
                cmd.Parameters.Add("OP_STARTDATE", OracleDbType.Date).Value = ePlan.OP_STARTDATE;
                cmd.Parameters.Add("OP_ENDDATE", OracleDbType.Date).Value = ePlan.OP_ENDDATE;
                cmd.Parameters.Add("TRAVELDAY", OracleDbType.Int32).Value = ePlan.TRAVELDAY;
                cmd.Parameters.Add("RRDAY", OracleDbType.Int32).Value = ePlan.RRDAY;
                cmd.Parameters.Add("D_Day", OracleDbType.Int32).Value = ePlan.D_Day;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ePlan.REMARKS_OUT = rdr["REMARKS"].ToString();
                    if (rdr["REF"].ToString() != "" && rdr["REF"].ToString() != null && rdr["REF"].ToString() == "1")
                    {
                        isContinue = true;
                        ePlan.IS_SUCCESS = "Yes";
                    }                        
                }

                if (isContinue)
                {
                    cmd.CommandText = "pkg_ais.P_AddAuditteamtasklist";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("TEAMID", OracleDbType.Int32).Value = ePlan.TEAM_ID;
                    cmd.Parameters.Add("PLANID", OracleDbType.Int32).Value = ePlan.PLAN_ID;
                    cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = ePlan.ENTITY_ID;
                    cmd.ExecuteReader();

                }

            }
           this.DisposeDatabaseConnection();
            return ePlan;
            
        }
        public bool RefferedBackAuditEngagementPlan(int ENG_ID, string REMARKS)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_RefferedBackAuditEngagementPlan";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = REMARKS;
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public string RerecommendAuditEngagementPlan(int ENG_ID, int PLAN_ID, int ENTITY_ID, DateTime OP_START_DATE, DateTime OP_END_DATE, DateTime START_DATE, DateTime END_DATE, int TEAM_ID, string COMMENTS)
        {
            string resp = "";
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_RerecommendAuditEngagementPlan";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = ENTITY_ID;
                cmd.Parameters.Add("STARTDATE", OracleDbType.Date).Value = START_DATE;
                cmd.Parameters.Add("ENDDATE", OracleDbType.Date).Value = END_DATE;
                cmd.Parameters.Add("TEAMID", OracleDbType.Int32).Value = TEAM_ID;
                cmd.Parameters.Add("UPDATEDBY", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("PLANID", OracleDbType.Int32).Value = PLAN_ID;
                cmd.Parameters.Add("OP_STARTDATE", OracleDbType.Date).Value = OP_START_DATE;
                cmd.Parameters.Add("OP_ENDDATE", OracleDbType.Date).Value = OP_END_DATE;
                cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = COMMENTS;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    resp = rdr["REMARK"].ToString();                   
                }
            }
           this.DisposeDatabaseConnection();
            return resp;
        }
        public bool ApproveAuditEngagementPlan(int ENG_ID)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_ApproveAuditEngagementPlan";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
          
            return true;
        }

        public UserModel GetMatchedPPNumbers(string PPNO)
        {
            UserModel um = new UserModel();
           if (PPNO == "")
                return um;
            if (PPNO != null && PPNO != "")
            {
                var con = this.DatabaseConnection();

                using (OracleCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "pkg_ais.p_get_allusers";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = 0;
                    cmd.Parameters.Add("EMAIL", OracleDbType.Varchar2).Value = "";
                    cmd.Parameters.Add("GROUPID", OracleDbType.Int32).Value = 0;
                    cmd.Parameters.Add("PPNUMBER", OracleDbType.Int32).Value = PPNO;
                    cmd.Parameters.Add("LOGINNAME", OracleDbType.Varchar2).Value = "";
                    cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    OracleDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        um.ID = Convert.ToInt32(rdr["USERID"].ToString());
                        um.Name = rdr["EMPLOYEEFIRSTNAME"].ToString() + " " +rdr["EMPLOYEELASTNAME"].ToString();
                        um.PPNumber = rdr["ppno"].ToString();
                    }
                }
               this.DisposeDatabaseConnection();
            }

            return um;
        }
        public List<AuditPlanModel> GetAuditPlan(int period_id = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            List<AuditPlanModel> planList = new List<AuditPlanModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
               
                string _sql = "pkg_ais.p_get_audit_plan";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("AUDITED_BY", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                cmd.CommandText = _sql;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditPlanModel plan = new AuditPlanModel();
                    plan.PLAN_ID = Convert.ToInt32(rdr["PLAN_ID"]);
                    plan.AUDITPERIOD_ID = Convert.ToInt32(rdr["AUDITPERIOD_ID"]);
                    if (rdr["NO_OF_DAYS_AUDIT"].ToString() != null && rdr["NO_OF_DAYS_AUDIT"].ToString() != "")
                        plan.NO_OF_DAYS_AUDIT = Convert.ToInt32(rdr["NO_OF_DAYS_AUDIT"]);
                    if (rdr["AUDITZONE_ID"].ToString() != null && rdr["AUDITZONE_ID"].ToString() != "")
                        plan.AUDITZONE_ID = Convert.ToInt32(rdr["AUDITZONE_ID"]);
                    if (rdr["BRANCH_ID"].ToString() != null && rdr["BRANCH_ID"].ToString() != "")
                        plan.BRANCH_ID = Convert.ToInt32(rdr["BRANCH_ID"]);
                    if (rdr["DIVISION_ID"].ToString() != null && rdr["DIVISION_ID"].ToString() != "")
                        plan.DIVISION_ID = Convert.ToInt32(rdr["DIVISION_ID"]);
                    if (rdr["DEPARTMENT_ID"].ToString() != null && rdr["DEPARTMENT_ID"].ToString() != "")
                        plan.DEPARTMENT_ID = Convert.ToInt32(rdr["DEPARTMENT_ID"]);
                    if (rdr["PLAN_STATUS_ID"].ToString() != null && rdr["PLAN_STATUS_ID"].ToString() != "")
                        plan.PLAN_STATUS_ID = Convert.ToInt32(rdr["PLAN_STATUS_ID"]);
                    if (rdr["BRANCH_SIZE_ID"].ToString() != null && rdr["BRANCH_SIZE_ID"].ToString() != "")
                        plan.BRANCH_SIZE_ID = Convert.ToInt32(rdr["BRANCH_SIZE_ID"]);
                    if (rdr["RISK_LEVEL_ID"].ToString() != null && rdr["RISK_LEVEL_ID"].ToString() != "")
                        plan.RISK_LEVEL_ID = Convert.ToInt32(rdr["RISK_LEVEL_ID"]);
                    if (rdr["SUB_ENTITY_ID"].ToString() != null && rdr["SUB_ENTITY_ID"].ToString() != "")
                        plan.SUB_ENTITY_ID = Convert.ToInt32(rdr["SUB_ENTITY_ID"]);
                    plan.DEPARTMENT_NAME = rdr["DEPARTMENT_NAME"].ToString();
                    plan.BRANCH_NAME = rdr["BRANCH_NAME"].ToString();
                    plan.DIVISION_NAME = rdr["DIVISION_NAME"].ToString();
                    plan.AUDITZONE_NAME = rdr["AUDITZONE_NAME"].ToString();
                    planList.Add(plan);
                }
            }
           this.DisposeDatabaseConnection();
            return planList;
        }
        public List<RiskProcessDefinition> GetRiskProcessDefinition()
        {
            var con = this.DatabaseConnection();
            List<RiskProcessDefinition> pdetails = new List<RiskProcessDefinition>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetRiskProcessDefinition";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    RiskProcessDefinition proc = new RiskProcessDefinition();
                    proc.P_ID = Convert.ToInt32(rdr["T_ID"]);
                    if (rdr["ENTITY_TYPE"].ToString() != null && rdr["ENTITY_TYPE"].ToString() != "")
                        proc.RISK_ID = Convert.ToInt32(rdr["ENTITY_TYPE"]);
                    proc.P_NAME = rdr["HEADING"].ToString();
                    pdetails.Add(proc);
                }
            }
           this.DisposeDatabaseConnection();
            return pdetails;
        }
        public List<RiskProcessDetails> GetRiskProcessDetails(int procId = 0)
        {
            var con = this.DatabaseConnection();
            List<RiskProcessDetails> riskProcList = new List<RiskProcessDetails>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetRiskProcessDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("procId", OracleDbType.Int32).Value = procId;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    RiskProcessDetails pdetail = new RiskProcessDetails();
                    pdetail.ID = Convert.ToInt32(rdr["S_ID"]);
                    pdetail.P_ID = Convert.ToInt32(rdr["T_ID"]);
                    pdetail.ENTITY_TYPE = Convert.ToInt32(rdr["ENTITY_TYPE"]);
                    pdetail.TITLE = rdr["HEADING"].ToString();
                    pdetail.ACTIVE = rdr["STATUS"].ToString();
                    riskProcList.Add(pdetail);
                }
            }
           this.DisposeDatabaseConnection();
            return riskProcList;
        }
        public List<RiskProcessTransactions> GetRiskProcessTransactions(int procDetailId = 0, int transactionId = 0)
        {
            var con = this.DatabaseConnection();
            List<RiskProcessTransactions> riskTransList = new List<RiskProcessTransactions>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetRiskProcessTransactions";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("procDetailId", OracleDbType.Int32).Value = procDetailId;
                cmd.Parameters.Add("transactionId", OracleDbType.Int32).Value = transactionId;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    RiskProcessTransactions pTran = new RiskProcessTransactions();
                    pTran.ID = Convert.ToInt32(rdr["ID"]);
                    pTran.PD_ID = Convert.ToInt32(rdr["S_ID"]);
                    pTran.V_ID = Convert.ToInt32(rdr["V_ID"]);
                    if (rdr["ROLE_RESP_ID"].ToString() != null && rdr["ROLE_RESP_ID"].ToString() != "")
                        pTran.DIV_ID = Convert.ToInt32(rdr["ROLE_RESP_ID"]);
                    if (rdr["DIV_NAME"].ToString() != null && rdr["DIV_NAME"].ToString() != "")
                        pTran.DIV_NAME = rdr["DIV_NAME"].ToString();
                    if (rdr["HEADING"].ToString() != null && rdr["HEADING"].ToString() != "")
                        pTran.DESCRIPTION = rdr["HEADING"].ToString();
                    if (rdr["PROCESS_OWNER_ID"].ToString() != null && rdr["PROCESS_OWNER_ID"].ToString() != "")
                        pTran.CONTROL_OWNER = rdr["CONTROL_OWNER"].ToString();
                    pTran.RISK_WEIGHTAGE = Convert.ToInt32(rdr["RISK_ID"]);
                    pTran.RISK = this.GetRiskDescByID(pTran.RISK_WEIGHTAGE);
                    pTran.SUB_PROCESS_NAME = rdr["TITLE"].ToString();
                    pTran.PROCESS_NAME = rdr["P_NAME"].ToString();
                    pTran.VIOLATION_NAME = rdr["V_NAME"].ToString();
                    pTran.PROCESS_STATUS = rdr["STATUS"].ToString();
                    pTran.PROCESS_COMMENTS = this.GetLatestCommentsOnProcess(pTran.ID);
                    riskTransList.Add(pTran);
                }
            }
           this.DisposeDatabaseConnection();
            return riskTransList;
        }
        public List<RiskProcessTransactions> GetRiskProcessTransactionsWithStatus(int statusId)
        {
            var con = this.DatabaseConnection();
            List<RiskProcessTransactions> riskTransList = new List<RiskProcessTransactions>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_GetRiskProcessTransactionsWithStatus";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("statusId", OracleDbType.Int32).Value = statusId;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    RiskProcessTransactions pTran = new RiskProcessTransactions();
                    pTran.ID = Convert.ToInt32(rdr["ID"]);
                    pTran.PD_ID = Convert.ToInt32(rdr["S_ID"]);
                    pTran.V_ID = Convert.ToInt32(rdr["V_ID"]);
                    if (rdr["ROLE_RESP_ID"].ToString() != null && rdr["ROLE_RESP_ID"].ToString() != "")
                        pTran.DIV_ID = Convert.ToInt32(rdr["ROLE_RESP_ID"]);
                    if (rdr["DIV_NAME"].ToString() != null && rdr["DIV_NAME"].ToString() != "")
                        pTran.DIV_NAME = rdr["DIV_NAME"].ToString();
                    if (rdr["HEADING"].ToString() != null && rdr["HEADING"].ToString() != "")
                        pTran.DESCRIPTION = rdr["HEADING"].ToString();
                    if (rdr["PROCESS_OWNER_ID"].ToString() != null && rdr["PROCESS_OWNER_ID"].ToString() != "")
                        pTran.CONTROL_OWNER = rdr["CONTROL_OWNER"].ToString();
                    pTran.RISK_WEIGHTAGE = Convert.ToInt32(rdr["RISK_ID"]);
                    pTran.RISK = this.GetRiskDescByID(pTran.RISK_WEIGHTAGE);
                    pTran.SUB_PROCESS_NAME = rdr["TITLE"].ToString();
                    pTran.PROCESS_NAME = rdr["P_NAME"].ToString();
                    pTran.VIOLATION_NAME = rdr["V_NAME"].ToString();
                    pTran.PROCESS_STATUS = rdr["STATUS"].ToString();
                    pTran.PROCESS_COMMENTS = this.GetLatestCommentsOnProcess(pTran.ID);
                    riskTransList.Add(pTran);
                }
            }
           this.DisposeDatabaseConnection();
            return riskTransList;
        }
        public RiskProcessDefinition AddRiskProcess(RiskProcessDefinition proc)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_audit_checklist";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("p_name", OracleDbType.Varchar2).Value = proc.P_NAME;
                cmd.Parameters.Add("RISK_ID", OracleDbType.Int32).Value = proc.RISK_ID;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return proc;
        }
        public RiskProcessDetails AddRiskSubProcess(RiskProcessDetails subProc)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_audit_checklist_sub";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("p_ID", OracleDbType.Int32).Value = subProc.P_ID;
                cmd.Parameters.Add("TITLE", OracleDbType.Varchar2).Value = subProc.TITLE;
                cmd.Parameters.Add("ENTITY_TYPE", OracleDbType.Varchar2).Value = subProc.ENTITY_TYPE;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return subProc;
        }
        public RiskProcessTransactions AddRiskSubProcessTransaction(RiskProcessTransactions trans)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.audit_checklist_detail";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = trans.PD_ID;
                cmd.Parameters.Add("DESCRIPTION", OracleDbType.Varchar2).Value = trans.DESCRIPTION;
                cmd.Parameters.Add("V_ID", OracleDbType.Varchar2).Value = trans.V_ID;
                cmd.Parameters.Add("CONTROL_OWNER", OracleDbType.Varchar2).Value = trans.CONTROL_OWNER;
                cmd.Parameters.Add("RISK_WEIGHTAGE", OracleDbType.Varchar2).Value = trans.RISK_WEIGHTAGE;
                cmd.Parameters.Add("ACTION", OracleDbType.Varchar2).Value = trans.ACTION;
                cmd.Parameters.Add("PPNumber", OracleDbType.Varchar2).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return trans;
        }
        public bool RecommendProcessTransactionByReviewer(int T_ID, string COMMENTS)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_Recommend_Process_Transaction_By_Reviewer";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_ID", OracleDbType.Int32).Value = T_ID;
                cmd.Parameters.Add("COMMENTS", OracleDbType.Varchar2).Value = COMMENTS;
                cmd.Parameters.Add("PPNumber", OracleDbType.Varchar2).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public bool RefferedBackProcessTransactionByReviewer(int T_ID, string COMMENTS)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_RefferedBack_Process_Transaction_By_Reviewer";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_ID", OracleDbType.Int32).Value = T_ID;
                cmd.Parameters.Add("COMMENTS", OracleDbType.Varchar2).Value = COMMENTS;
                cmd.Parameters.Add("PPNumber", OracleDbType.Varchar2).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public bool RecommendProcessTransactionByAuthorizer(int T_ID, string COMMENTS)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_Recommend_Process_Transaction_By_Authorizer";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_ID", OracleDbType.Int32).Value = T_ID;
                cmd.Parameters.Add("COMMENTS", OracleDbType.Varchar2).Value = COMMENTS;
                cmd.Parameters.Add("PPNumber", OracleDbType.Varchar2).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public bool RefferedBackProcessTransactionByAuthorizer(int T_ID, string COMMENTS)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_RefferedBack_Process_Transaction_By_Authorizer";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_ID", OracleDbType.Int32).Value = T_ID;
                cmd.Parameters.Add("COMMENTS", OracleDbType.Varchar2).Value = COMMENTS;
                cmd.Parameters.Add("PPNumber", OracleDbType.Varchar2).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public List<AuditFrequencyModel> GetAuditFrequencies()
        {
            var con = this.DatabaseConnection();
            List<AuditFrequencyModel> freqList = new List<AuditFrequencyModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_GetAuditFrequencies";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditFrequencyModel freq = new AuditFrequencyModel();
                    freq.ID = Convert.ToInt32(rdr["ID"]);
                    freq.FREQUENCY_ID = Convert.ToInt32(rdr["FREQUENCY_ID"]);
                    freq.FREQUENCY_DISCRIPTION = rdr["FREQUENCY_DISCRIPTION"].ToString();
                    freq.STATUS = rdr["STATUS"].ToString();
                    freqList.Add(freq);
                }
            }
           this.DisposeDatabaseConnection();
            return freqList;
        }
        public List<RiskModel> GetRisks()
        {
            var con = this.DatabaseConnection();
            List<RiskModel> riskList = new List<RiskModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetRisks";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    RiskModel risk = new RiskModel();
                    risk.R_ID = Convert.ToInt32(rdr["R_ID"]);
                    risk.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    riskList.Add(risk);
                }
            }
           this.DisposeDatabaseConnection();
            return riskList;
        }
        public List<RiskModel> GetCOSORisks()
        {
            var con = this.DatabaseConnection();
            List<RiskModel> riskList = new List<RiskModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_GetCOSORisks";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    RiskModel risk = new RiskModel();
                    risk.R_ID = Convert.ToInt32(rdr["R_ID"]);
                    risk.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    risk.RATING = rdr["RATING"].ToString();
                    riskList.Add(risk);
                }
            }
           this.DisposeDatabaseConnection();
            return riskList;
        }     
        public List<AuditVoilationcatModel> GetAuditVoilationcats()
        {
            var con = this.DatabaseConnection();
            List<AuditVoilationcatModel> voilationList = new List<AuditVoilationcatModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditVoilationcats";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditVoilationcatModel voilationcat = new AuditVoilationcatModel();
                    voilationcat.ID = Convert.ToInt32(rdr["ID"]);
                    voilationcat.V_NAME = rdr["V_Name"].ToString();
                    voilationcat.MAX_NUMBER = Convert.ToInt32(rdr["MAX_Number"]);
                    voilationcat.STATUS = rdr["Status"].ToString();
                    voilationList.Add(voilationcat);
                }
            }
           this.DisposeDatabaseConnection();
            return voilationList;
        }
        public List<AuditSubVoilationcatModel> GetVoilationSubGroup(int group_id)
        {
            var con = this.DatabaseConnection();
            List<AuditSubVoilationcatModel> voilationsubgroupList = new List<AuditSubVoilationcatModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetVoilationSubGroup";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("group_id", OracleDbType.Int32).Value = group_id;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    AuditSubVoilationcatModel vsgm = new AuditSubVoilationcatModel();
                    vsgm.ID = Convert.ToInt32(rdr["ID"]);
                    vsgm.V_ID = Convert.ToInt32(rdr["V_ID"]);
                    vsgm.SUB_V_NAME = rdr["SUB_V_NAME"].ToString();
                    vsgm.RISK_ID = rdr["RISK_ID"].ToString();
                    vsgm.STATUS = rdr["STATUS"].ToString();

                    voilationsubgroupList.Add(vsgm);
                }
            }
           this.DisposeDatabaseConnection();
            return voilationsubgroupList;
        }
        public List<TaskListModel> GetTaskList()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            List<TaskListModel> tasklist = new List<TaskListModel>();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetTaskList";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    TaskListModel tlist = new TaskListModel();
                    tlist.ID = Convert.ToInt32(rdr["ID"]);
                    tlist.ENG_PLAN_ID = Convert.ToInt32(rdr["ENG_PLAN_ID"]);
                    tlist.TEAM_ID = Convert.ToInt32(rdr["TEAM_ID"]);
                    tlist.SEQUENCE_NO = Convert.ToInt32(rdr["SEQUENCE_NO"]);
                    tlist.TEAMMEMBER_PPNO = Convert.ToInt32(rdr["TEAMMEMBER_PPNO"]);
                    tlist.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    tlist.ENTITY_TYPE = Convert.ToInt32(rdr["ENTITY_TYPE"]);
                    tlist.ENTITY_CODE = Convert.ToInt32(rdr["ENTITY_CODE"]);
                    tlist.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    //tlist.TEAMMEMBER_PPNO = Convert.ToInt32(loggedInUser.PPNumber);
                    tlist.TEAM_NAME = rdr["T_NAME"].ToString();
                    tlist.EMP_NAME = loggedInUser.Name.ToString();
                    tlist.AUDIT_START_DATE = Convert.ToDateTime(rdr["AUDIT_START_DATE"]);
                    tlist.AUDIT_END_DATE = Convert.ToDateTime(rdr["AUDIT_END_DATE"]);
                    tlist.STATUS_ID = Convert.ToInt32(rdr["STATUS_ID"]);
                    tlist.ENG_STATUS = rdr["ENG_STATUS"].ToString();
                    tlist.ENG_NEXT_STATUS = rdr["ENG_NEXT_STATUS"].ToString();
                    tlist.ISACTIVE = rdr["ISACTIVE"].ToString();
                    tlist.AUDIT_YEAR = rdr["AUDIT_YEAR"].ToString();
                    if(rdr["OPERATION_STARTDATE"].ToString()!=null && rdr["OPERATION_STARTDATE"].ToString()!="")
                    tlist.OPERATION_STARTDATE = Convert.ToDateTime(rdr["OPERATION_STARTDATE"]);
                    if (rdr["OPERATION_ENDDATE"].ToString() != null && rdr["OPERATION_ENDDATE"].ToString() != "")
                        tlist.OPERATION_ENDDATE = Convert.ToDateTime(rdr["OPERATION_ENDDATE"]);
                    tasklist.Add(tlist);
                }
            }
           this.DisposeDatabaseConnection();
            return tasklist;
        }
        public JoiningModel GetJoiningDetails(int engId = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            JoiningModel jm = new JoiningModel();
            List<JoiningTeamModel> tjlist = new List<JoiningTeamModel>();
            var loggedInUser = sessionHandler.GetSessionUser();
            if (engId == 0)
                engId = this.GetLoggedInUserEngId();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetJoiningDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("engId", OracleDbType.Int32).Value = engId;
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                 while (rdr.Read())
                {
                    jm.ENG_PLAN_ID = engId;
                    jm.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    jm.ENTITY_CODE = Convert.ToInt32(rdr["ENTITY_CODE"]);
                    jm.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    jm.STATUS = "";
                    jm.RISK = rdr["RISK"].ToString();
                    jm.SIZE = rdr["ENT_SIZE"].ToString();
                    jm.START_DATE = Convert.ToDateTime(rdr["AUDIT_START_DATE"]);
                    jm.END_DATE = Convert.ToDateTime(rdr["AUDIT_END_DATE"]);
                    jm.AUDIT_PERIOD = rdr["AUDIT_PERIOD"].ToString();
                    jm.TEAM_NAME = rdr["TEAM_NAME"].ToString();
                    JoiningTeamModel tm = new JoiningTeamModel();
                    tm.EMP_NAME = rdr["MEMBER_NAME"].ToString();
                    tm.PP_NO = Convert.ToInt32(rdr["MEMBER_PPNO"]);
                    tm.IS_TEAM_LEAD = rdr["ISTEAMLEAD"].ToString();
                    tjlist.Add(tm);
                    jm.TEAM_DETAILS = tjlist;
                }
            }
           this.DisposeDatabaseConnection();
            return jm;
        }
        public bool AddJoiningReport(AddJoiningModel jm)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            jm.ENTEREDBY = Convert.ToInt32(loggedInUser.PPNumber);
            jm.ENTEREDDATE = System.DateTime.Now;
            bool response = false;
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_AddJoiningReport";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = jm.ENG_PLAN_ID;
                cmd.Parameters.Add("PPNO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("COMPLETION_DATE", OracleDbType.Date).Value = jm.COMPLETION_DATE;
                cmd.ExecuteReader();
                this.SetEngIdOnHold();
                response = true;
            }
           this.DisposeDatabaseConnection();
            return response;
        }
        public List<AuditChecklistModel> GetAuditChecklist()
        {
            var con = this.DatabaseConnection();
            List<AuditChecklistModel> list = new List<AuditChecklistModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {

                cmd.CommandText = "pkg_ais.P_GetAuditChecklist";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
               
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditChecklistModel chk = new AuditChecklistModel();
                    chk.T_ID = Convert.ToInt32(rdr["T_ID"]);
                    chk.HEADING = rdr["HEADING"].ToString();
                    chk.ENTITY_TYPE = Convert.ToInt32(rdr["ENTITY_TYPE"]);
                    chk.ENTITY_TYPE_NAME = rdr["ENTITY_TYPE_NAME"].ToString();
                    chk.STATUS = rdr["STATUS"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<AuditChecklistModel> GetAuditChecklistCAD()
        {
            var con = this.DatabaseConnection();
            List<AuditChecklistModel> list = new List<AuditChecklistModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {

                cmd.CommandText = "pkg_ais.P_GetAuditChecklistCAD";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditChecklistModel chk = new AuditChecklistModel();
                    chk.T_ID = Convert.ToInt32(rdr["T_ID"]);
                    chk.HEADING = rdr["HEADING"].ToString();
                    chk.ENTITY_TYPE = Convert.ToInt32(rdr["ENTITY_TYPE"]);
                    //chk.ENTITY_TYPE_NAME = rdr["ENTITY_TYPE_NAME"].ToString();
                    chk.STATUS = rdr["STATUS"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<AuditChecklistSubModel> GetAuditChecklistSub(int t_id = 0, int eng_id = 0)
        {
            var con = this.DatabaseConnection();
            List<AuditChecklistSubModel> list = new List<AuditChecklistSubModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_GetAuditChecklistSub";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("tid", OracleDbType.Int32).Value = t_id;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditChecklistSubModel chk = new AuditChecklistSubModel();
                    chk.S_ID = Convert.ToInt32(rdr["S_ID"]);
                    chk.T_ID = Convert.ToInt32(rdr["T_ID"]);
                    chk.T_NAME = rdr["T_NAME"].ToString();
                    chk.HEADING = rdr["HEADING"].ToString();
                    chk.ENTITY_TYPE = Convert.ToInt32(rdr["ENTITY_TYPE"]);
                    chk.ENTITY_TYPE_NAME = rdr["ENTITY_TYPE_NAME"].ToString();
                    chk.STATUS = "Pending";
                    /*if (eng_id != 0)
                    {
                        cmd.CommandText = "select os.statusname from t_au_observation o inner join t_au_observation_status os on o.status=os.statusid where o.subchecklist_id=" + chk.S_ID + " and o.engplanid=" + eng_id;
                        OracleDataReader rdr2 = cmd.ExecuteReader();
                        while (rdr2.Read())
                        {
                            if (rdr2["statusname"].ToString() != "" && rdr2["statusname"].ToString() != null)
                            { chk.STATUS = rdr2["statusname"].ToString(); }
                            else
                            {
                                chk.STATUS = "Pending";
                            }
                        }
                    }*/

                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<AuditChecklistDetailsModel> GetAuditChecklistDetails(int s_id = 0)
        {
            var con = this.DatabaseConnection();
            List<AuditChecklistDetailsModel> list = new List<AuditChecklistDetailsModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditChecklistDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("sid", OracleDbType.Int32).Value = s_id;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditChecklistDetailsModel chk = new AuditChecklistDetailsModel();
                    chk.ID = Convert.ToInt32(rdr["ID"]);
                    chk.S_ID = Convert.ToInt32(rdr["S_ID"]);
                    chk.S_NAME = rdr["S_NAME"].ToString();
                    chk.V_ID = Convert.ToInt32(rdr["V_ID"]);
                    chk.V_NAME = rdr["V_NAME"].ToString();
                    chk.HEADING = rdr["HEADING"].ToString();
                    chk.RISK_ID = Convert.ToInt32(rdr["RISK_ID"]);
                    chk.RISK = rdr["RISK"].ToString();
                    if (rdr["ROLE_RESP_ID"].ToString() != null && rdr["ROLE_RESP_ID"].ToString() != "")
                    {
                        chk.ROLE_RESP_ID = Convert.ToInt32(rdr["ROLE_RESP_ID"]);
                    }
                    if (rdr["PROCESS_OWNER_ID"].ToString() != null && rdr["PROCESS_OWNER_ID"].ToString() != "")
                    {
                        chk.PROCESS_OWNER_ID = Convert.ToInt32(rdr["PROCESS_OWNER_ID"]);

                    }
                    chk.STATUS = rdr["STATUS"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<GlHeadDetailsModel> GetGlheadDetails(int gl_code = 0)
        {
            int ENG_ID = this.GetLoggedInUserEngId();
            var con = this.DatabaseConnection();

            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

            List<GlHeadDetailsModel> list = new List<GlHeadDetailsModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_getglheadsummary";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                // cmd.Parameters.Add("GLSUBCODE", OracleDbType.Int32).Value = gl_code;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    GlHeadDetailsModel GlHeadDetails = new GlHeadDetailsModel();
                    GlHeadDetails.BRANCHID = Convert.ToInt32(rdr["BRANCHID"]);
                    GlHeadDetails.GL_TYPEID = Convert.ToInt32(rdr["GL_TYPEID"]);

                    GlHeadDetails.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    // GlHeadDetails.GLSUBCODE = Convert.ToInt32(rdr["GLSUBCODE"]);
                    //GlHeadDetails.GLSUBNAME = rdr["GLSUBNAME"].ToString();
                    //GlHeadDetails.DATETIME = Convert.ToDateTime(rdr["DATETIME"]);
                    GlHeadDetails.BALANCE = Convert.ToDouble(rdr["BALANCE"]);
                    if (rdr["DEBIT"].ToString() != null && rdr["DEBIT"].ToString() != "")
                        GlHeadDetails.DEBIT = Convert.ToDouble(rdr["DEBIT"]);
                    if (rdr["CREDIT"].ToString() != null && rdr["CREDIT"].ToString() != "")
                        GlHeadDetails.CREDIT = Convert.ToDouble(rdr["CREDIT"]);
                    list.Add(GlHeadDetails);
                }
            }
           this.DisposeDatabaseConnection();
            return list;

        }

        public GlHeadSubDetailsModel GetGlheadSubDetails(int gltypeid = 0)
        {
            var con = this.DatabaseConnection();
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

            GlHeadSubDetailsModel GlHeadSubDetails = new GlHeadSubDetailsModel();
            List<GlHeadSubDetailsModel> GlSubHeadList = new List<GlHeadSubDetailsModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_getglheadsum";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("gltypeid", OracleDbType.Int32).Value = gltypeid;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {


                    GlHeadSubDetailsModel GHSD = new GlHeadSubDetailsModel();

                    GHSD.GLSUBCODE = Convert.ToInt32(rdr["GLSUBCODE"]);
                    GHSD.BRANCHID = Convert.ToInt32(rdr["BRANCHID"]);

                    GHSD.GLSUBNAME = rdr["GLSUBNAME"].ToString();
                    GHSD.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    //GHSD.DATETIME = Convert.ToDateTime(rdr["DATETIME"]);
                    GHSD.BALANCE = Convert.ToDouble(rdr["BALANCE"]);
                    GHSD.DEBIT = Convert.ToDouble(rdr["DEBIT"]);
                    GHSD.CREDIT = Convert.ToDouble(rdr["CREDIT"]);
                    GlSubHeadList.Add(GHSD);
                    GlHeadSubDetails.GL_SUBDETAILS = GlSubHeadList;
                }
            }
           this.DisposeDatabaseConnection();
            return GlHeadSubDetails;

        }



        public List<LoanCaseModel> GetLoanCaseDetails(int lid = 0, string type = "")
        {
            int ENG_ID = this.GetLoggedInUserEngId();
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

            List<LoanCaseModel> list = new List<LoanCaseModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetLoanCaseDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("loantype", OracleDbType.Varchar2).Value = type;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    LoanCaseModel LoanCaseDetails = new LoanCaseModel();
                    //LoanCaseDetails.BRANCHID = Convert.ToInt32(rdr["BRANCHID"]);
                    LoanCaseDetails.CNIC = Convert.ToDouble(rdr["CNIC"]);
                    LoanCaseDetails.LOAN_CASE_NO = Convert.ToInt32(rdr["LOAN_CASE_NO"]);
                    LoanCaseDetails.CUSTOMERNAME = rdr["CUSTOMERNAME"].ToString();
                    LoanCaseDetails.FATHERNAME = rdr["FATHERNAME"].ToString();
                    LoanCaseDetails.DISBURSED_AMOUNT = Convert.ToDouble(rdr["DISBURSED_AMOUNT"]);
                    LoanCaseDetails.PRIN = Convert.ToDouble(rdr["PRIN"]);
                    LoanCaseDetails.MARKUP = Convert.ToDouble(rdr["MARKUP"]);
                    LoanCaseDetails.GLSUBCODE = Convert.ToInt32(rdr["GLSUBCODE"]);
                    // LoanCaseDetails.LOAN_DISB_ID = Convert.ToDouble(rdr["LOAN_DISB_ID"]);
                    LoanCaseDetails.DISB_DATE = Convert.ToDateTime(rdr["DISB_DATE"]);
                    LoanCaseDetails.DISB_STATUSID = Convert.ToInt32(rdr["DISB_STATUSID"]);
                    list.Add(LoanCaseDetails);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }

        public List<LoanCasedocModel> GetLoanCaseDocuments()
        {
            List<LoanCasedocModel> list = new List<LoanCasedocModel>();
            /*
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

           
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetLoanCaseDocuments";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    LoanCasedocModel LoanCaseDetails = new LoanCasedocModel();
                    LoanCaseDetails.TEAM_MEM_PPNO = Convert.ToString(rdr["TEAM_MEM_PPNO"]);
                    LoanCaseDetails.BRANCHCODE = Convert.ToString(rdr["BRANCHCODE"]);
                    LoanCaseDetails.LOAN_APP_ID = Convert.ToString(rdr["LOAN_APP_ID"]);
                    LoanCaseDetails.CNIC = Convert.ToString(rdr["CNIC"]);
                    LoanCaseDetails.LOAN_CASE_NO = Convert.ToString(rdr["LOAN_CASE_NO"]);
                    LoanCaseDetails.GLSUBCODE = Convert.ToString(rdr["GLSUBCODE"]);
                    LoanCaseDetails.CUSTOMERNAME = rdr["CUSTOMERNAME"].ToString();
                    LoanCaseDetails.LOAN_DISB_ID = Convert.ToString(rdr["LOAN_DISB_ID"]);
                    LoanCaseDetails.DOCUMENTS = rdr["DOCUMENTS"].ToString();
                    LoanCaseDetails.IMAGES = rdr["IMAGES"].ToString();

                    list.Add(LoanCaseDetails);
                }
            }
           this.DisposeDatabaseConnection();*/
            return list;
        }
        public List<GlHeadDetailsModel> GetIncomeExpenceDetails(int bid = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            int ENG_ID = this.GetLoggedInUserEngId();

            var con = this.DatabaseConnection();
            List<GlHeadDetailsModel> list = new List<GlHeadDetailsModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetIncomeExpenceDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    GlHeadDetailsModel GlHeadDetails = new GlHeadDetailsModel();
                    //GlHeadDetails.TEAM_MEM_PPNO = Convert.ToDouble(rdr["TEAM_MEM_PPNO"]);
                    GlHeadDetails.NAME = rdr["NAME"].ToString();
                    GlHeadDetails.GLSUBNAME = rdr["GLSUBNAME"].ToString();
                    GlHeadDetails.GLSUBCODE = Convert.ToInt32(rdr["GLSUBCODE"]);
                    // GlHeadDetails.DESCRIPTION = rdr["DESCRIPTION"].ToString();



                    //GlHeadDetails.DAY_END_BALANCE_DATE = Convert.ToDateTime(rdr["DAY_END_BALANCE_DATE"]);
                    // GlHeadDetails.BALANCE = Convert.ToDouble(rdr["BALANCE"]);
                    if (rdr["DEBIT"].ToString() != null && rdr["DEBIT"].ToString() != "")
                        GlHeadDetails.DEBIT = Convert.ToDouble(rdr["DEBIT"]);
                    if (rdr["CREDIT"].ToString() != null && rdr["CREDIT"].ToString() != "")
                        GlHeadDetails.CREDIT = Convert.ToDouble(rdr["CREDIT"]);
                    list.Add(GlHeadDetails);
                }
            }
           this.DisposeDatabaseConnection();
            return list;

        }



        public List<DepositAccountModel> GetDepositAccountdetails()
        {
            List<DepositAccountModel> depositacclist = new List<DepositAccountModel>();
            /*
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

            var con = this.DatabaseConnection();
            
            using (OracleCommand cmd = con.CreateCommand())
            {

                cmd.CommandText = "pkg_ais.P_GetDepositAccountdetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DepositAccountModel depositaccdetails = new DepositAccountModel();
                    depositaccdetails.NAME = rdr["NAME"].ToString();
                    depositacclist.Add(depositaccdetails);
                }
            }
           this.DisposeDatabaseConnection();*/
            return depositacclist;
        }
        public List<DepositAccountModel> GetDepositAccountSubdetails(string bname = "")
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            int ENG_ID = this.GetLoggedInUserEngId();
            var con = this.DatabaseConnection();
            List<DepositAccountModel> depositaccsublist = new List<DepositAccountModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetDepositAccountSubdetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DepositAccountModel depositaccsubdetails = new DepositAccountModel();

                    depositaccsubdetails.BRANCH_NAME = rdr["BRANCH_NAME"].ToString();
                    if (rdr["ACC_NUMBER"].ToString() != null && rdr["ACC_NUMBER"].ToString() != "")
                        depositaccsubdetails.ACC_NUMBER = Convert.ToDouble(rdr["ACC_NUMBER"]);
                    if (rdr["ACCOUNTCATEGORY"].ToString() != null && rdr["ACCOUNTCATEGORY"].ToString() != "")
                        depositaccsubdetails.ACCOUNTCATEGORY = rdr["ACCOUNTCATEGORY"].ToString();

                    if (rdr["CUSTOMERNAME"].ToString() != null && rdr["CUSTOMERNAME"].ToString() != "")
                        depositaccsubdetails.CUSTOMERNAME = rdr["CUSTOMERNAME"].ToString();
                    if (rdr["BMVS_VERIFIED"].ToString() != null && rdr["BMVS_VERIFIED"].ToString() != "")
                        depositaccsubdetails.BMVS_VERIFIED = rdr["BMVS_VERIFIED"].ToString();


                    if (rdr["OPENINGDATE"].ToString() != null && rdr["OPENINGDATE"].ToString() != "")
                    {
                        depositaccsubdetails.OPENINGDATE = Convert.ToDateTime(rdr["OPENINGDATE"]);
                    }
                    if (rdr["CNIC"].ToString() != null && rdr["CNIC"].ToString() != "")
                    {
                        depositaccsubdetails.CNIC = Convert.ToDouble(rdr["CNIC"]);
                    }
                    if (rdr["TITLE"].ToString() != null && rdr["TITLE"].ToString() != "")
                        depositaccsubdetails.TITLE = rdr["TITLE"].ToString();


                    if (rdr["ACCOCUNTSTATUS"].ToString() != null && rdr["ACCOCUNTSTATUS"].ToString() != "")
                        depositaccsubdetails.ACCOUNTSTATUS = rdr["ACCOCUNTSTATUS"].ToString();
                    if (rdr["LASTTRANSACTIONDATE"].ToString() != null && rdr["LASTTRANSACTIONDATE"].ToString() != "")
                    {
                        depositaccsubdetails.LASTTRANSACTIONDATE = Convert.ToDateTime(rdr["LASTTRANSACTIONDATE"]);
                    }
                    if (rdr["CNICEXPIRYDATE"].ToString() != null && rdr["CNICEXPIRYDATE"].ToString() != "")
                    {
                        depositaccsubdetails.CNICEXPIRYDATE = Convert.ToDateTime(rdr["CNICEXPIRYDATE"]);
                    }
                    depositaccsublist.Add(depositaccsubdetails);
                }
            }
           this.DisposeDatabaseConnection();
            return depositaccsublist;
        }

        public List<LoanCaseModel> GetBranchDesbursementAccountdetails(int bid = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;

            var loggedInUser = sessionHandler.GetSessionUser();
            int brId = Convert.ToInt32(loggedInUser.UserPostingBranch);
            List<LoanCaseModel> list = new List<LoanCaseModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetBranchDesbursementAccountdetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    LoanCaseModel LoanCaseDetails = new LoanCaseModel();
                    //  LoanCaseDetails.BRANCHID = Convert.ToInt32(rdr["BRANCHID"]);
                    LoanCaseDetails.CNIC = Convert.ToDouble(rdr["CNIC"]);
                    LoanCaseDetails.LOAN_CASE_NO = Convert.ToInt32(rdr["LOAN_CASE_NO"]);
                    LoanCaseDetails.CUSTOMERNAME = rdr["CUSTOMERNAME"].ToString();
                    LoanCaseDetails.FATHERNAME = rdr["FATHERNAME"].ToString();
                    LoanCaseDetails.DISBURSED_AMOUNT = Convert.ToDouble(rdr["DISBURSED_AMOUNT"]);
                    LoanCaseDetails.PRIN = Convert.ToDouble(rdr["PRIN"]);
                    LoanCaseDetails.MARKUP = Convert.ToDouble(rdr["MARKUP"]);
                    LoanCaseDetails.GLSUBCODE = Convert.ToInt32(rdr["GLSUBCODE"]);
                    //  LoanCaseDetails.LOAN_DISB_ID = Convert.ToDouble(rdr["LOAN_DISB_ID"]);
                    LoanCaseDetails.DISB_DATE = Convert.ToDateTime(rdr["DISB_DATE"]);
                    LoanCaseDetails.DISB_STATUSID = Convert.ToInt32(rdr["DISB_STATUSID"]);
                    list.Add(LoanCaseDetails);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }


        public bool SaveAuditObservation(ObservationModel ob)
        {
            //105400
            int addedObsId = 0;
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            if (ob.ENGPLANID == 0)
                ob.ENGPLANID = this.GetLoggedInUserEngId();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_SaveAuditObservation";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PLANID", OracleDbType.Int32).Value = ob.ENGPLANID;
                cmd.Parameters.Add("STATUS", OracleDbType.Int32).Value = ob.STATUS;
                cmd.Parameters.Add("REPLYDATE", OracleDbType.Date).Value = ob.REPLYDATE;
                cmd.Parameters.Add("ENTEREDBY", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("Severity", OracleDbType.Int32).Value = ob.SEVERITY;
                cmd.Parameters.Add("SUBCHECKLISTID", OracleDbType.Varchar2).Value = ob.SUBCHECKLIST_ID;
                cmd.Parameters.Add("CHECKLISTDETAILID", OracleDbType.Int32).Value = ob.CHECKLISTDETAIL_ID;
                cmd.Parameters.Add("VCATID", OracleDbType.Int32).Value = ob.V_CAT_ID;
                cmd.Parameters.Add("VCATNATUREID", OracleDbType.Int32).Value = ob.V_CAT_NATURE_ID;
                cmd.Parameters.Add("TEXT_DATA", OracleDbType.Clob).Value = ob.OBSERVATION_TEXT;
                cmd.Parameters.Add("NOINSTANCES", OracleDbType.Int32).Value = ob.NO_OF_INSTANCES;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["REF"].ToString() != "" && rdr["REF"].ToString() != null && rdr["REF"].ToString() == "2")
                    {
                        return false;
                    }
                    else if (rdr["REF"].ToString() != "" && rdr["REF"].ToString() != null && rdr["REF"].ToString() == "1")
                    {
                        addedObsId = Convert.ToInt32(rdr["ID"].ToString());
                    }
                }
                if (ob.RESPONSIBLE_PPNO != null)
                {
                    if (ob.RESPONSIBLE_PPNO.Count > 0 && addedObsId>0)
                    {
                        foreach (ObservationResponsiblePPNOModel pp in ob.RESPONSIBLE_PPNO)
                        {
                            cmd.CommandText = "pkg_ais.P_responibilityassigned";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("ID", OracleDbType.Int32).Value = addedObsId;
                            cmd.Parameters.Add("PPNO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                            cmd.Parameters.Add("RES_PP", OracleDbType.Int32).Value = pp.PP_NO;
                            cmd.Parameters.Add("LOANCASE", OracleDbType.Int32).Value = pp.LOAN_CASE;
                            cmd.Parameters.Add("ACCNUMBER", OracleDbType.Int32).Value = pp.ACCOUNT_NUMBER;
                            cmd.Parameters.Add("LCAMOUNT", OracleDbType.Int32).Value = pp.LC_AMOUNT;
                            cmd.Parameters.Add("ACAMOUNT", OracleDbType.Int32).Value = pp.ACC_AMOUNT;
                            cmd.ExecuteReader();
                        }
                    }

                }                  
            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public bool SaveAuditObservationCAU(ObservationModel ob)
        {

            int addedObsId = 0;
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            if (ob.ENGPLANID == 0)
                ob.ENGPLANID = this.GetLoggedInUserEngId();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_SaveAuditObservationCAD";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PLANID", OracleDbType.Int32).Value = ob.ENGPLANID;
                cmd.Parameters.Add("STATUS", OracleDbType.Int32).Value = ob.STATUS;
                cmd.Parameters.Add("REPLYDATE", OracleDbType.Date).Value = ob.REPLYDATE;
                cmd.Parameters.Add("ENTEREDBY", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("Severity", OracleDbType.Int32).Value = ob.SEVERITY;
                cmd.Parameters.Add("SUBCHECKLISTID", OracleDbType.Varchar2).Value = ob.SUBCHECKLIST_ID;
                cmd.Parameters.Add("CHECKLISTDETAILID", OracleDbType.Int32).Value = ob.CHECKLISTDETAIL_ID;              
                cmd.Parameters.Add("TEXT_DATA", OracleDbType.Clob).Value = ob.OBSERVATION_TEXT;
                cmd.Parameters.Add("BRANCHID", OracleDbType.Int32).Value = ob.BRANCH_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["REF"].ToString() != "" && rdr["REF"].ToString() != null && rdr["REF"].ToString() == "2")
                    {
                        return false;
                    }
                    else if (rdr["REF"].ToString() != "" && rdr["REF"].ToString() != null && rdr["REF"].ToString() == "1")
                    {
                        addedObsId = Convert.ToInt32(rdr["ID"].ToString());

                    }
                }
                if (ob.RESPONSIBLE_PPNO != null)
                {
                    if (ob.RESPONSIBLE_PPNO.Count > 0 && addedObsId > 0)
                    {
                        foreach (ObservationResponsiblePPNOModel pp in ob.RESPONSIBLE_PPNO)
                        {
                            cmd.CommandText = "pkg_ais.P_responibilityassigned";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("ID", OracleDbType.Int32).Value = addedObsId;
                            cmd.Parameters.Add("PPNO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                            cmd.Parameters.Add("RES_PP", OracleDbType.Int32).Value = pp.PP_NO;
                            cmd.Parameters.Add("LOANCASE", OracleDbType.Int32).Value = pp.LOAN_CASE;
                            cmd.Parameters.Add("ACCNUMBER", OracleDbType.Int32).Value = pp.ACCOUNT_NUMBER;
                            cmd.Parameters.Add("LCAMOUNT", OracleDbType.Int32).Value = pp.LC_AMOUNT;
                            cmd.Parameters.Add("ACAMOUNT", OracleDbType.Int32).Value = pp.ACC_AMOUNT;
                            cmd.ExecuteReader();
                        }
                    }

                }

            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public List<AssignedObservations> GetAssignedObservations(int ENG_ID)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<AssignedObservations> list = new List<AssignedObservations>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_GetAssignedObservations";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    AssignedObservations chk = new AssignedObservations();
                    chk.ID = Convert.ToInt32(rdr["ID"]);

                    chk.OBS_ID = Convert.ToInt32(rdr["OBS_ID"]);
                    chk.OBS_TEXT_ID = Convert.ToInt32(rdr["OBS_TEXT_ID"]);
                    chk.ASSIGNEDTO_ROLE = Convert.ToInt32(rdr["ENTITY_ID"]);
                    chk.ASSIGNEDBY = Convert.ToInt32(rdr["ASSIGNEDBY"]);
                    chk.ASSIGNED_DATE = Convert.ToDateTime(rdr["ASSIGNED_DATE"]);
                    chk.IS_ACTIVE = rdr["IS_ACTIVE"].ToString();
                    chk.REPLIED = rdr["REPLIED"].ToString();                   
                    chk.REPLY_TEXT = "";
                    chk.OBSERVATION_TEXT = "";
                    if (rdr["RESP_ID"].ToString() != null && rdr["RESP_ID"].ToString() != "")
                        chk.RESP_ID = Convert.ToInt32(rdr["RESP_ID"].ToString());
                  
                    if (rdr["VIOLATION"].ToString() != null && rdr["VIOLATION"].ToString() != "")
                        chk.VIOLATION = rdr["VIOLATION"].ToString();

                    if (rdr["NATURE"].ToString() != null && rdr["NATURE"].ToString() != "")
                        chk.NATURE = rdr["NATURE"].ToString();
                                        
                    if (rdr["Process"].ToString() != null && rdr["Process"].ToString() != "")
                        chk.PROCESS = rdr["Process"].ToString();

                    if (rdr["Sub_process"].ToString() != null && rdr["Sub_process"].ToString() != "")
                        chk.SUB_PROCESS = rdr["Sub_process"].ToString();

                    if (rdr["Check_List_Details"].ToString() != null && rdr["Check_List_Details"].ToString() != "")
                        chk.CHECKLIST_DETAIL = rdr["Check_List_Details"].ToString();

                    
                    chk.STATUS = rdr["STATUS"].ToString();
                    chk.STATUS_ID = rdr["STATUS_ID"].ToString();
                    //chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.MEMO_DATE = rdr["MEMO_DATE"].ToString();
                    chk.MEMO_REPLY_DATE = rdr["REPLYDATE"].ToString();
                    chk.MEMO_NUMBER = rdr["MEMO_NUMBER"].ToString();
                    chk.AUDIT_YEAR = rdr["AUDIT_YEAR"].ToString();
                    chk.OPERATION_STARTDATE = Convert.ToDateTime(rdr["OPERATION_STARTDATE"]);
                    chk.OPERATION_ENDDATE = Convert.ToDateTime(rdr["OPERATION_ENDDATE"]);

                    chk.RESPONSIBLE_PPNOs = this.GetObservationResponsiblePPNOs(chk.ID);
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<AssignedObservations> GetAssignedObservationsForBranch(int ENG_ID)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();        

            List<AssignedObservations> list = new List<AssignedObservations>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAssignedObservationsForBranch";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("entityid", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                
                while (rdr.Read())
                {
                    AssignedObservations chk = new AssignedObservations();
                    chk.ID = Convert.ToInt32(rdr["ID"]);
                    chk.OBS_ID = Convert.ToInt32(rdr["OBS_ID"]);
                    chk.OBS_TEXT_ID = Convert.ToInt32(rdr["OBS_TEXT_ID"]);
                    chk.ASSIGNEDTO_ROLE = Convert.ToInt32(rdr["ENTITY_ID"]);
                    chk.ASSIGNEDBY = Convert.ToInt32(rdr["ASSIGNEDBY"]);
                    chk.ASSIGNED_DATE = Convert.ToDateTime(rdr["ASSIGNED_DATE"]);
                    chk.IS_ACTIVE = rdr["IS_ACTIVE"].ToString();
                    chk.REPLIED = rdr["REPLIED"].ToString();chk.REPLY_TEXT = rdr["REPLY_TEXT"].ToString();
                    chk.OBSERVATION_TEXT = rdr["OBSERVATION_TEXT"].ToString();

                    if (rdr["VIOLATION"].ToString() != null && rdr["VIOLATION"].ToString() != "")
                        chk.VIOLATION = rdr["VIOLATION"].ToString();

                    if (rdr["NATURE"].ToString() != null && rdr["NATURE"].ToString() != "")
                        chk.NATURE = rdr["NATURE"].ToString();

                    chk.STATUS = rdr["STATUS"].ToString();
                    chk.STATUS_ID = rdr["STATUS_ID"].ToString();
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.MEMO_DATE = rdr["MEMO_DATE"].ToString();
                    chk.MEMO_REPLY_DATE = rdr["REPLYDATE"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<object> GetObservationText(int OBS_ID, int RESP_ID)
        {
            var con = this.DatabaseConnection();
            string ob_text = "";
            string ob_resp = "";

            List<object> list = new List<object>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetObservationText";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBS_ID", OracleDbType.Int32).Value = OBS_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ob_text = rdr["TEXT"].ToString();                   

                }
                list.Add(ob_text);

                cmd.CommandText = "pkg_ais.P_GetOBSERVATIONSAUDITEERESPONSE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBS_ID", OracleDbType.Int32).Value = OBS_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr2 = cmd.ExecuteReader();
               
                while (rdr2.Read())
                {
                    ob_resp = rdr2["REPLY"].ToString();
                }
                list.Add(ob_resp);
                List<AuditeeResponseEvidenceModel> modellist = new List<AuditeeResponseEvidenceModel>();
                cmd.CommandText = "pkg_ais.P_get_AUDITEE_OBSERVATION_RESPONSE_evidences";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("RESP_ID", OracleDbType.Int32).Value = RESP_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr3 = cmd.ExecuteReader();
                while (rdr3.Read())
                {
                    AuditeeResponseEvidenceModel am = new AuditeeResponseEvidenceModel();
                    am.IMAGE_NAME = rdr3["FILE_NAME"].ToString();
                    am.IMAGE_DATA = rdr3["FILE_DATA"].ToString();
                    am.SEQUENCE = Convert.ToInt32(rdr3["SEQUENCE"].ToString());
                    am.IMAGE_TYPE = rdr3["FILE_TYPE"].ToString();
                    modellist.Add(am);
                }
                list.Add(modellist);
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<ObservationResponsiblePPNOModel> GetObservationResponsiblePPNOs(int OBS_ID)
        {
            var con = this.DatabaseConnection();
            List<ObservationResponsiblePPNOModel> list = new List<ObservationResponsiblePPNOModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetObservationResponsible";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBSID", OracleDbType.Int32).Value = OBS_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ObservationResponsiblePPNOModel usr = new ObservationResponsiblePPNOModel();
                    usr.EMP_NAME = rdr["EMP_NAME"].ToString();
                    usr.PP_NO = rdr["PP_NO"].ToString();
                    usr.LOAN_CASE = rdr["LOANCASE"].ToString();
                    usr.LC_AMOUNT = rdr["LCAMOUNT"].ToString();
                    usr.ACCOUNT_NUMBER = rdr["ACCNUMBER"].ToString();
                    usr.ACC_AMOUNT = rdr["ACAMOUNT"].ToString();
                    list.Add(usr);

                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public bool ResponseAuditObservation(ObservationResponseModel ob)
        {
            int AUD_RESP_ID = 0;
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            ob.REPLIEDBY = Convert.ToInt32(loggedInUser.PPNumber);
            ob.REPLIEDDATE = System.DateTime.Now;
            ob.REMARKS = "";
            ob.SUBMITTED = "Y";
            ob.REPLY_ROLE = 0;
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_AUDITEE_OBSERVATION_RESPONSE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("AUOBSID", OracleDbType.Int32).Value = ob.AU_OBS_ID;
                cmd.Parameters.Add("REPLYDATA", OracleDbType.Clob).Value = ob.REPLY;
                cmd.Parameters.Add("REPLIEDBY", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("OBSTEXTID", OracleDbType.Int32).Value = ob.OBS_TEXT_ID;
                cmd.Parameters.Add("REPLYROLE", OracleDbType.Int32).Value = ob.REPLY_ROLE;
                cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = ob.REMARKS;
                cmd.Parameters.Add("SUBMITTED", OracleDbType.Varchar2).Value = ob.SUBMITTED;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AUD_RESP_ID = Convert.ToInt32(rdr["RESP_ID"]);
                }
                if(ob.EVIDENCE_LIST!=null)
                {
                    if (ob.EVIDENCE_LIST.Count > 0)
                    {
                        foreach ( var item in ob.EVIDENCE_LIST)
                        {
                            string fileName= AUD_RESP_ID + "_" + item.IMAGE_NAME;
                            cmd.CommandText = "pkg_ais.P_AUDITEE_OBSERVATION_RESPONSE_EVIDENCES";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("RESPID", OracleDbType.Int32).Value = AUD_RESP_ID;
                            cmd.Parameters.Add("AUOBSID", OracleDbType.Int32).Value = ob.AU_OBS_ID;
                            cmd.Parameters.Add("FILENAME", OracleDbType.Varchar2).Value = fileName;
                            cmd.Parameters.Add("FILETYPE", OracleDbType.Varchar2).Value = item.IMAGE_TYPE;
                            cmd.Parameters.Add("LENGTH", OracleDbType.Int32).Value = item.LENGTH;
                            cmd.Parameters.Add("ENTEREDBY", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                            cmd.Parameters.Add("FILEDATA", OracleDbType.Clob).Value = item.IMAGE_DATA;
                            cmd.Parameters.Add("SEQUENCE", OracleDbType.Int32).Value = (item.SEQUENCE+1);
                            cmd.Parameters.Add("TEXT_ID", OracleDbType.Int32).Value = ob.OBS_TEXT_ID;
                            cmd.ExecuteReader();
                            this.SaveImage(item.IMAGE_DATA, fileName);
                        }
                    }
                        
                }
            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public int GetLoggedInUserEngId()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            int engId = 0;
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetLoggedInUserEngId";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    engId = Convert.ToInt32(rdr["eng_plan_id"]);
                }
            }
           this.DisposeDatabaseConnection();
            return engId;
        }
        public bool SetEngIdOnHold()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            int ENG_ID = this.GetLoggedInUserEngId();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_SetEngIdOnHold";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("PPNO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public string GetLatestAuditorResponse(int obs_id = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            string response = "";
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetLatestAuditorResponse";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("obs_id", OracleDbType.Int32).Value = obs_id;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    response = rdr["Recommendation"].ToString();
                }
            }
           this.DisposeDatabaseConnection();
            return response;
        }
        public string GetLatestDepartmentalHeadResponse(int obs_id = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            string response = "";
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetLatestDepartmentalHeadResponse";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("obs_id", OracleDbType.Int32).Value = obs_id;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    response = rdr["audit_reply"].ToString();
                }
            }
           this.DisposeDatabaseConnection();
            return response;
        }
        public string GetRiskDescByID(int risk_id = 0)
        {
            var con = this.DatabaseConnection();
            string response = "";
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetRiskDescByID";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("risk_id", OracleDbType.Int32).Value = risk_id;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    response = rdr["DESCRIPTION"].ToString();
                }
            }
           this.DisposeDatabaseConnection();
            return response;
        }
        public string GetLatestCommentsOnProcess(int procId = 0)
        {
            var con = this.DatabaseConnection();
            string response = "";
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetLatestCommentsOnProcess";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("procId", OracleDbType.Int32).Value = procId;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

               
                while (rdr.Read())
                {
                    response = rdr["comments"].ToString();
                }
            }
           this.DisposeDatabaseConnection();
            return response;
        }

        public string GetLatestCommentsOnEngagement(int engId = 0)
        {
            var con = this.DatabaseConnection();
            string response = "";
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetLatestCommentsOnEngagement";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = engId;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    response = rdr["remarks"].ToString();
                }
            }
           this.DisposeDatabaseConnection();
            return response;
        }
        public string GetLatestAuditeeResponse(int obs_id = 0)
        {
            var con = this.DatabaseConnection();
            string response = "";
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetLatestAuditeeResponse";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("obs_id", OracleDbType.Int32).Value = obs_id;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    response = rdr["reply"].ToString();
                }
            }
           this.DisposeDatabaseConnection();
            return response;
        }
        public List<ManageObservations> GetManagedObservations(int ENG_ID = 0, int OBS_ID = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<ManageObservations> list = new List<ManageObservations>();

            if (loggedInUser.UserLocationType == "Z")
            {
                return this.GetManagedObservationsForBranches(ENG_ID, OBS_ID);
            }
            if (ENG_ID == 0)
                ENG_ID = this.GetLoggedInUserEngId();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetManagedObservations";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("OBSID", OracleDbType.Int32).Value = OBS_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ManageObservations chk = new ManageObservations();

                    chk.OBS_ID = Convert.ToInt32(rdr["OBS_ID"]);
                    chk.OBS_RISK_ID = Convert.ToInt32(rdr["OBS_RISK_ID"]);
                    chk.OBS_STATUS_ID = Convert.ToInt32(rdr["OBS_STATUS_ID"]);
                    chk.MEMO_NO = Convert.ToInt32(rdr["MEMO_NO"]);
                    chk.NO_OF_INSTANCES = Convert.ToInt32(rdr["NOINSTANCES"]);
                    chk.VIOLATION = rdr["VIOLATION"].ToString();
                    chk.NATURE = rdr["NATURE"].ToString();
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.OBS_STATUS = rdr["OBS_STATUS"].ToString();
                    chk.OBS_RISK = rdr["OBS_RISK"].ToString();
                    chk.PERIOD = rdr["PERIOD"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<ManageObservations> GetManagedObservationText(int ENG_ID = 0, int OBS_ID = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<ManageObservations> list = new List<ManageObservations>();

            if (loggedInUser.UserLocationType == "Z")
            {
                return this.GetManagedObservationsForBranches(ENG_ID, OBS_ID);
            }
            if (ENG_ID == 0)
                ENG_ID = this.GetLoggedInUserEngId();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetManagedObservationsText";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBSID", OracleDbType.Int32).Value = OBS_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ManageObservations chk = new ManageObservations();

                    chk.VIOLATION = rdr["VIOLATION"].ToString();
                    chk.NATURE = rdr["NATURE"].ToString();
                    chk.OBS_TEXT = rdr["OBS_TEXT"].ToString();
                    chk.OBS_REPLY = this.GetLatestAuditeeResponse(chk.OBS_ID);
                    chk.RESPONSIBLE_PPs = this.GetObservationResponsiblePPNOs(chk.OBS_ID);
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<ManageObservations> GetManagedObservationsForBranches(int ENG_ID = 0, int OBS_ID = 0)
        {

            var con = this.DatabaseConnection();
            if (ENG_ID == 0 && OBS_ID==0)
                ENG_ID = this.GetLoggedInUserEngId();

            List<ManageObservations> list = new List<ManageObservations>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetManagedObservationsForBranches";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("OBSID", OracleDbType.Int32).Value = OBS_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ManageObservations chk = new ManageObservations();

                    chk.OBS_ID = Convert.ToInt32(rdr["OBS_ID"]);
                    chk.OBS_RISK_ID = Convert.ToInt32(rdr["OBS_RISK_ID"]);
                    chk.OBS_STATUS_ID = Convert.ToInt32(rdr["OBS_STATUS_ID"]);
                    chk.MEMO_NO = Convert.ToInt32(rdr["MEMO_NO"]);
                    chk.PROCESS = rdr["PROCESS"].ToString();
                    chk.NO_OF_INSTANCES = Convert.ToInt32(rdr["NOINSTANCES"]);                  
                    chk.SUB_PROCESS = rdr["SUB_PROCESS"].ToString();
                    chk.Checklist_Details = rdr["Check_List_Detail"].ToString();
                    //chk.OBS_TEXT = rdr["OBS_TEXT"].ToString();
                    //chk.OBS_REPLY = this.GetLatestAuditeeResponse(chk.OBS_ID);
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.OBS_STATUS = rdr["OBS_STATUS"].ToString();
                    chk.OBS_RISK = rdr["OBS_RISK"].ToString();
                    chk.PERIOD = rdr["PERIOD"].ToString();
                    //chk.RESPONSIBLE_PPs = this.GetObservationResponsiblePPNOs(chk.OBS_ID);
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<ManageObservations> GetManagedObservationTextForBranches(int ENG_ID = 0, int OBS_ID = 0)
        {

            var con = this.DatabaseConnection();
            if (ENG_ID == 0 && OBS_ID == 0)
                ENG_ID = this.GetLoggedInUserEngId();

            List<ManageObservations> list = new List<ManageObservations>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetManagedObservationsForBranchesText";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBSID", OracleDbType.Int32).Value = OBS_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ManageObservations chk = new ManageObservations();

                    //chk.OBS_ID = Convert.ToInt32(rdr["OBS_ID"]);
                    chk.PROCESS = rdr["PROCESS"].ToString();
                    chk.SUB_PROCESS = rdr["SUB_PROCESS"].ToString();
                    chk.Checklist_Details = rdr["Check_List_Detail"].ToString();
                    chk.OBS_TEXT = rdr["OBS_TEXT"].ToString();
                    chk.OBS_REPLY = this.GetLatestAuditeeResponse(OBS_ID);

                    chk.RESPONSIBLE_PPs = this.GetObservationResponsiblePPNOs(chk.OBS_ID);
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<ManageObservations> GetManagedDraftObservations(int ENG_ID = 0, int OBS_ID=0)
        {
            var con = this.DatabaseConnection();
            if (ENG_ID == 0)
                ENG_ID = this.GetLoggedInUserEngId();
            List<ManageObservations> list = new List<ManageObservations>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetManagedDraftObservations";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ManageObservations chk = new ManageObservations();
                    chk.OBS_ID = Convert.ToInt32(rdr["OBS_ID"]);
                    chk.OBS_RISK_ID = Convert.ToInt32(rdr["OBS_RISK_ID"]);
                    chk.OBS_STATUS_ID = Convert.ToInt32(rdr["OBS_STATUS_ID"]);
                    chk.MEMO_NO = Convert.ToInt32(rdr["MEMO_NO"]);
                    chk.VIOLATION = rdr["VIOLATION"].ToString();
                    chk.NATURE = rdr["NATURE"].ToString();


                  
                   // chk.OBS_TEXT = rdr["OBS_TEXT"].ToString();
                   // chk.OBS_REPLY = this.GetLatestAuditeeResponse(chk.OBS_ID);
                    chk.AUD_REPLY = this.GetLatestAuditorResponse(chk.OBS_ID);
                    chk.HEAD_REPLY = this.GetLatestDepartmentalHeadResponse(chk.OBS_ID);
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.OBS_STATUS = rdr["OBS_STATUS"].ToString();
                    chk.OBS_RISK = rdr["OBS_RISK"].ToString();
                    chk.PERIOD = rdr["PERIOD"].ToString();
                   // chk.RESPONSIBLE_PPs = this.GetObservationResponsiblePPNOs(chk.OBS_ID);
                    list.Add(chk);

                }
            }
           this.DisposeDatabaseConnection();

            return list;
        }
        public List<ManageObservations> GetFinalizedDraftObservations(int ENG_ID = 0, int OBS_ID = 0)
        {
            var con = this.DatabaseConnection();
            if (ENG_ID == 0)
                ENG_ID = this.GetLoggedInUserEngId();
            List<ManageObservations> list = new List<ManageObservations>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetManagedDraftObservations";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ManageObservations chk = new ManageObservations();
                    chk.OBS_ID = Convert.ToInt32(rdr["OBS_ID"]);
                    chk.OBS_RISK_ID = Convert.ToInt32(rdr["OBS_RISK_ID"]);
                    chk.OBS_STATUS_ID = Convert.ToInt32(rdr["OBS_STATUS_ID"]);
                    chk.MEMO_NO = Convert.ToInt32(rdr["MEMO_NO"]);
                    chk.VIOLATION = rdr["VIOLATION"].ToString();
                    chk.NATURE = rdr["NATURE"].ToString();



                  
                    chk.AUD_REPLY = this.GetLatestAuditorResponse(chk.OBS_ID);
                    chk.HEAD_REPLY = this.GetLatestDepartmentalHeadResponse(chk.OBS_ID);
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.OBS_STATUS = rdr["OBS_STATUS"].ToString();
                    chk.OBS_RISK = rdr["OBS_RISK"].ToString();
                    chk.PERIOD = rdr["PERIOD"].ToString();
                    // chk.RESPONSIBLE_PPs = this.GetObservationResponsiblePPNOs(chk.OBS_ID);
                    list.Add(chk);

                }
            }
           this.DisposeDatabaseConnection();

            return list;
        }
        public List<ManageObservations> GetManagedDraftObservationsBranch(int ENG_ID = 0, int OBS_ID = 0)
        {
            var con = this.DatabaseConnection();
            if (ENG_ID == 0)
                ENG_ID = this.GetLoggedInUserEngId();
            List<ManageObservations> list = new List<ManageObservations>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetManagedDraftObservationsForBranches";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ManageObservations chk = new ManageObservations();
                    chk.OBS_ID = Convert.ToInt32(rdr["OBS_ID"]);
                    chk.OBS_RISK_ID = Convert.ToInt32(rdr["OBS_RISK_ID"]);
                    chk.OBS_STATUS_ID = Convert.ToInt32(rdr["OBS_STATUS_ID"]);
                    if(rdr["MEMO_NO"].ToString()!= null && rdr["MEMO_NO"].ToString() != "")
                        chk.MEMO_NO = Convert.ToInt32(rdr["MEMO_NO"]);                  
                    chk.PROCESS = rdr["PROCESS"].ToString();
                    chk.SUB_PROCESS = rdr["SUB_PROCESS"].ToString();
                    chk.Checklist_Details = rdr["CHECK_LIST_DETAIL"].ToString();

                    chk.AUD_REPLY = this.GetLatestAuditorResponse(chk.OBS_ID);
                    chk.HEAD_REPLY = this.GetLatestDepartmentalHeadResponse(chk.OBS_ID);
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.OBS_STATUS = rdr["OBS_STATUS"].ToString();
                    chk.OBS_RISK = rdr["OBS_RISK"].ToString();
                    chk.PERIOD = rdr["PERIOD"].ToString();
                    // chk.RESPONSIBLE_PPs = this.GetObservationResponsiblePPNOs(chk.OBS_ID);
                    list.Add(chk);

                }
            }
           this.DisposeDatabaseConnection();

            return list;
        }
        public List<ManageObservations> GetFinalizedDraftObservationsBranch(int ENG_ID = 0, int OBS_ID = 0)
        {
            var con = this.DatabaseConnection();
            if (ENG_ID == 0)
                ENG_ID = this.GetLoggedInUserEngId();
            List<ManageObservations> list = new List<ManageObservations>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetManagedDraftObservationsForBranches";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ManageObservations chk = new ManageObservations();
                    chk.OBS_ID = Convert.ToInt32(rdr["OBS_ID"]);
                    chk.OBS_RISK_ID = Convert.ToInt32(rdr["OBS_RISK_ID"]);
                    chk.OBS_STATUS_ID = Convert.ToInt32(rdr["OBS_STATUS_ID"]);
                    if (rdr["MEMO_NO"].ToString() != null && rdr["MEMO_NO"].ToString() != "")
                        chk.MEMO_NO = Convert.ToInt32(rdr["MEMO_NO"]);
                    chk.PROCESS = rdr["PROCESS"].ToString();
                    chk.SUB_PROCESS = rdr["SUB_PROCESS"].ToString();
                    chk.Checklist_Details = rdr["CHECK_LIST_DETAIL"].ToString();

                    chk.AUD_REPLY = this.GetLatestAuditorResponse(chk.OBS_ID);
                    chk.HEAD_REPLY = this.GetLatestDepartmentalHeadResponse(chk.OBS_ID);
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.OBS_STATUS = rdr["OBS_STATUS"].ToString();
                    chk.OBS_RISK = rdr["OBS_RISK"].ToString();
                    chk.PERIOD = rdr["PERIOD"].ToString();
                    // chk.RESPONSIBLE_PPs = this.GetObservationResponsiblePPNOs(chk.OBS_ID);
                    list.Add(chk);

                }
            }
           this.DisposeDatabaseConnection();

            return list;
        }
        public List<ManageObservations> GetManagedDraftObservationsText(int ENG_ID = 0, int OBS_ID = 0)
        {
            var con = this.DatabaseConnection();
            if (ENG_ID == 0)
                ENG_ID = this.GetLoggedInUserEngId();
            List<ManageObservations> list = new List<ManageObservations>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetManagedDraftObservationsText";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBSID", OracleDbType.Int32).Value = OBS_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ManageObservations chk = new ManageObservations();
                    chk.OBS_TEXT = rdr["OBS_TEXT"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();

            return list;
        }
        public List<ManageObservations> GetManagedDraftObservationsForBranches(int ENG_ID = 0)
        {
            var con = this.DatabaseConnection();
            if (ENG_ID == 0)
                ENG_ID = this.GetLoggedInUserEngId();
            List<ManageObservations> list = new List<ManageObservations>();
            List<ManageObservations> finalList = new List<ManageObservations>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetManagedDraftObservationsForBranches";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

              
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ManageObservations chk = new ManageObservations();
                    chk.OBS_ID = Convert.ToInt32(rdr["OBS_ID"]);
                    chk.OBS_RISK_ID = Convert.ToInt32(rdr["OBS_RISK_ID"]);
                    chk.OBS_STATUS_ID = Convert.ToInt32(rdr["OBS_STATUS_ID"]);
                    chk.MEMO_NO = Convert.ToInt32(rdr["MEMO_NO"]);

                    if (rdr["PROCESS"].ToString() != null && rdr["PROCESS"].ToString() != "")
                        chk.PROCESS = rdr["PROCESS"].ToString();

                    if (rdr["SUBPROCESS"].ToString() != null && rdr["SUBPROCESS"].ToString() != "")
                        chk.SUB_PROCESS = rdr["SUBPROCESS"].ToString();



                    chk.OBS_TEXT = rdr["OBS_TEXT"].ToString();
                    chk.OBS_REPLY = this.GetLatestAuditeeResponse(chk.OBS_ID);
                    chk.AUD_REPLY = this.GetLatestAuditorResponse(chk.OBS_ID);
                    chk.HEAD_REPLY = this.GetLatestDepartmentalHeadResponse(chk.OBS_ID);
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.OBS_STATUS = rdr["OBS_STATUS"].ToString();
                    chk.OBS_RISK = rdr["OBS_RISK"].ToString();
                    chk.PERIOD = rdr["PERIOD"].ToString();
                    chk.RESPONSIBLE_PPs = this.GetObservationResponsiblePPNOs(chk.OBS_ID);
                    list.Add(chk);

                }
            }
           this.DisposeDatabaseConnection();

            return list;
        }

        public List<SubCheckListStatus> GetSubChecklistStatus(int ENG_ID=0, int S_ID=0)
        {
            List<SubCheckListStatus> list = new List<SubCheckListStatus>();
            var con = this.DatabaseConnection();
            if (ENG_ID == 0)
                ENG_ID = this.GetLoggedInUserEngId();           
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_getauditeecheckklist";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PLANID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("SUBCHECKLISTID", OracleDbType.Int32).Value = S_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    SubCheckListStatus chk = new SubCheckListStatus();
                    if (rdr["S_ID"].ToString() != null && rdr["S_ID"].ToString() != "")
                        chk.S_ID = Convert.ToInt32(rdr["S_ID"].ToString());
                    if (rdr["CHECKLIST_ID"].ToString() != null && rdr["CHECKLIST_ID"].ToString() != "")
                        chk.CD_ID = Convert.ToInt32(rdr["CHECKLIST_ID"].ToString());
                    if (rdr["STATUS"].ToString() != null && rdr["STATUS"].ToString() != "")
                        chk.Status = rdr["STATUS"].ToString();
                    if (rdr["OBSID"].ToString() != null && rdr["OBSID"].ToString() != "")
                        chk.OBS_ID = Convert.ToInt32(rdr["OBSID"].ToString());
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<ManageObservations> GetViolationObservations(int ENTITY_ID = 0, int VIOLATION_ID = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();

            List<ManageObservations> list = new List<ManageObservations>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetViolationObservations";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ManageObservations chk = new ManageObservations();

                    chk.OBS_ID = Convert.ToInt32(rdr["OBS_ID"]);
                    chk.OBS_RISK_ID = Convert.ToInt32(rdr["OBS_RISK_ID"]);
                    chk.OBS_STATUS_ID = Convert.ToInt32(rdr["OBS_STATUS_ID"]);
                    chk.MEMO_NO = Convert.ToInt32(rdr["MEMO_NO"]);
                    if (rdr["PROCESS"].ToString() != null && rdr["PROCESS"].ToString() != "")
                        chk.PROCESS = rdr["PROCESS"].ToString();
                    else
                        chk.PROCESS = rdr["V_CAT_NAME"].ToString();

                    if (rdr["SUB_PROCESS"].ToString() != null && rdr["SUB_PROCESS"].ToString() != "")
                        chk.SUB_PROCESS = rdr["SUB_PROCESS"].ToString();
                    else
                        chk.SUB_PROCESS = rdr["V_CAT_NATURE_NAME"].ToString();
                    //chk.Checklist_Details = rdr["Checklist_Details"].ToString();
                    //if (rdr["VIOLATION"].ToString() != null && rdr["VIOLATION"].ToString() != "")
                    chk.VIOLATION = rdr["VIOLATION"].ToString();
                    chk.OBS_TEXT = rdr["OBS_TEXT"].ToString();
                    chk.OBS_REPLY = this.GetLatestAuditeeResponse(chk.OBS_ID);
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.OBS_STATUS = rdr["OBS_STATUS"].ToString();
                    chk.OBS_RISK = rdr["OBS_RISK"].ToString();
                    chk.RESPONSIBLE_PPs = this.GetObservationResponsiblePPNOs(chk.OBS_ID);
                    chk.PERIOD = rdr["PERIOD"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public string DropAuditObservation(int OBS_ID)
        {
            string resp = "";
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_DropAuditObservation";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBS_ID", OracleDbType.Int32).Value = OBS_ID;
                cmd.Parameters.Add("PP_NO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    resp = rdr["REMARKS"].ToString();
                    /*if (rdr["REF"].ToString() != "" && rdr["REF"].ToString() != null && rdr["REF"].ToString() == "1")
                    {
                        
                    }*/
                }
            }
            return resp;
        }
        public string SubmitAuditObservationToAuditee(int OBS_ID)
        {
            string resp = "";
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_SubmitAuditObservationToAuditee";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBS_ID", OracleDbType.Int32).Value = OBS_ID;
                cmd.Parameters.Add("PP_NO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    resp = rdr["REMARKS"].ToString();
                    /*if (rdr["REF"].ToString() != "" && rdr["REF"].ToString() != null && rdr["REF"].ToString() == "1")
                    {
                        
                    }*/
                }
            }
            return resp;
        }
        public string UpdateAuditObservationStatus(int OBS_ID, int NEW_STATUS_ID, string AUDITOR_COMMENT)
        {
            string resp = "";
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            string Remarks = "";
            if (NEW_STATUS_ID == 4)
                Remarks = "Settled";
            else if (NEW_STATUS_ID == 5)
                Remarks = "Add to Draft Report";
            else if (NEW_STATUS_ID == 8)
                Remarks = "Add to Final Report";
            else if (NEW_STATUS_ID == 9)
                Remarks = "Para settle in discussion";
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {

                cmd.CommandText = "pkg_ais.P_UpdateAuditObservationStatus";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBS_ID", OracleDbType.Int32).Value = OBS_ID;
                cmd.Parameters.Add("NEW_STATUS_ID", OracleDbType.Int32).Value = NEW_STATUS_ID;
                cmd.Parameters.Add("Remarks", OracleDbType.Varchar2).Value = Remarks;
                cmd.Parameters.Add("PP_NO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    resp = rdr["REMARKS"].ToString();
                    /*if (rdr["REF"].ToString() != "" && rdr["REF"].ToString() != null && rdr["REF"].ToString() == "1")
                    {
                        
                    }*/
                }

                if (NEW_STATUS_ID == 4 || NEW_STATUS_ID == 5)
                {
                    cmd.CommandText = "pkg_ais.AUDITOR_RESPONSE";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("OBS_ID", OracleDbType.Int32).Value = OBS_ID;
                    cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                    cmd.Parameters.Add("AUDITOR_COMMENT", OracleDbType.Varchar2).Value = AUDITOR_COMMENT;
                    cmd.Parameters.Add("status", OracleDbType.Int32).Value = NEW_STATUS_ID;
                    cmd.ExecuteReader();

                }
                else if (NEW_STATUS_ID == 8 || NEW_STATUS_ID == 9)
                {
                    cmd.CommandText = "pkg_ais.AUDITOR_REPLY";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("OBS_ID", OracleDbType.Int32).Value = OBS_ID;
                    cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                    cmd.Parameters.Add("AUDITOR_COMMENT", OracleDbType.Varchar2).Value = AUDITOR_COMMENT;
                    cmd.Parameters.Add("status", OracleDbType.Int32).Value = NEW_STATUS_ID;
                    cmd.ExecuteReader();
                }               

            }
           this.DisposeDatabaseConnection();
            return resp;
        }
        public string UpdateAuditObservationText(int OBS_ID, string OBS_TEXT, int PROCESS_ID=0, int SUBPROCESS_ID=0, int CHECKLIST_ID = 0)
        {
            string resp = "";
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();            
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {

                cmd.CommandText = "pkg_ais.P_UpdateObservation";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBSID", OracleDbType.Int32).Value = OBS_ID;
                cmd.Parameters.Add("OBTEXT", OracleDbType.Clob).Value = OBS_TEXT;
                cmd.Parameters.Add("SUBPROCESSID", OracleDbType.Int32).Value = SUBPROCESS_ID;
                cmd.Parameters.Add("CHECKLISTID", OracleDbType.Int32).Value = CHECKLIST_ID;
                cmd.Parameters.Add("PPNO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    resp = rdr["REMARKS"].ToString();
                }
            }
           this.DisposeDatabaseConnection();
            return resp;
        }
        public List<ClosingDraftTeamDetailsModel> GetClosingDraftObservations(int ENG_ID = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
           
            List<ClosingDraftTeamDetailsModel> list = new List<ClosingDraftTeamDetailsModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_GetClosingDraftObservations";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PP_NO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ClosingDraftTeamDetailsModel chk = new ClosingDraftTeamDetailsModel();
                    chk.ENG_PLAN_ID = Convert.ToInt32(ENG_ID);
                    if (rdr["TOTAL_NO_OB"].ToString() != null && rdr["TOTAL_NO_OB"].ToString() != "")
                        chk.TOTAL_NO_OB = Convert.ToInt32(rdr["TOTAL_NO_OB"]);

                    if (rdr["SUBMITTED_TO_AUDITEE"].ToString() != null && rdr["SUBMITTED_TO_AUDITEE"].ToString() != "")
                        chk.SUBMITTED_TO_AUDITEE = Convert.ToInt32(rdr["SUBMITTED_TO_AUDITEE"]);
                    if (rdr["RESOLVED"].ToString() != null && rdr["RESOLVED"].ToString() != "")
                        chk.RESOLVED = Convert.ToInt32(rdr["RESOLVED"]);
                    if (rdr["RESPONDED"].ToString() != null && rdr["RESPONDED"].ToString() != "")
                        chk.RESPONDED = Convert.ToInt32(rdr["RESPONDED"]);
                    if (rdr["DROPPED"].ToString() != null && rdr["DROPPED"].ToString() != "")
                        chk.DROPPED = Convert.ToInt32(rdr["DROPPED"]);
                    if (rdr["ADDED_TO_DRAFT"].ToString() != null && rdr["ADDED_TO_DRAFT"].ToString() != "")
                        chk.ADDED_TO_DRAFT = Convert.ToInt32(rdr["ADDED_TO_DRAFT"]);


                    chk.TEAM_MEM_PPNO = Convert.ToInt32(rdr["MEMBER_PPNO"]);
                    chk.JOINING_DATE = Convert.ToDateTime(rdr["JOINING_DATE"]);
                    chk.COMPLETION_DATE = Convert.ToDateTime(rdr["COMPLETION_DATE"]);
                    chk.ISTEAMLEAD = rdr["TEAMLEAD"].ToString();
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.MEMBER_NAME = rdr["MEMBER_NAME"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<ClosingDraftTeamDetailsModel> GetClosingDraftTeamDetails(int ENG_ID = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            if (ENG_ID == 0)
                ENG_ID = this.GetLoggedInUserEngId();
            List<ClosingDraftTeamDetailsModel> list = new List<ClosingDraftTeamDetailsModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_GetClosingDraftObservations";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PP_NO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ClosingDraftTeamDetailsModel chk = new ClosingDraftTeamDetailsModel();
                    chk.ENG_PLAN_ID = Convert.ToInt32(ENG_ID);
                    if (rdr["SUBMITTED_TO_AUDITEE"].ToString()!=null && rdr["SUBMITTED_TO_AUDITEE"].ToString() != "")
                    chk.SUBMITTED_TO_AUDITEE = Convert.ToInt32(rdr["SUBMITTED_TO_AUDITEE"]);
                    if (rdr["RESOLVED"].ToString() != null && rdr["RESOLVED"].ToString() != "")
                        chk.RESOLVED = Convert.ToInt32(rdr["RESOLVED"]);
                    if (rdr["RESPONDED"].ToString() != null && rdr["RESPONDED"].ToString() != "")
                        chk.RESPONDED = Convert.ToInt32(rdr["RESPONDED"]);
                    if (rdr["DROPPED"].ToString() != null && rdr["DROPPED"].ToString() != "")
                        chk.DROPPED = Convert.ToInt32(rdr["DROPPED"]);
                    if (rdr["ADDED_TO_DRAFT"].ToString() != null && rdr["ADDED_TO_DRAFT"].ToString() != "")
                        chk.ADDED_TO_DRAFT = Convert.ToInt32(rdr["ADDED_TO_DRAFT"]);


                    chk.TEAM_MEM_PPNO = Convert.ToInt32(rdr["MEMBER_PPNO"]);
                    chk.JOINING_DATE = Convert.ToDateTime(rdr["JOINING_DATE"]);
                    chk.COMPLETION_DATE = Convert.ToDateTime(rdr["COMPLETION_DATE"]);
                    chk.ISTEAMLEAD = rdr["TEAMLEAD"].ToString();
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.MEMBER_NAME = rdr["MEMBER_NAME"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public string CloseDraftAuditReport(int ENG_ID)
        {
            string resp = "";
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            if (ENG_ID == 0)
                ENG_ID=this.GetLoggedInUserEngId();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_Closeaudit";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("PP_NO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    resp = rdr["REMARKS"].ToString();

                }
            }
           this.DisposeDatabaseConnection();
            return resp;
        }
        public int GetExpectedCountOfAuditEntitiesOnCriteria(int RISK_ID, int SIZE_ID, int ENTITY_TYPE_ID, int PERIOD_ID, int FREQUENCY_ID)
        {
            var con = this.DatabaseConnection();
            int count = 0;
            int criteria_id = 0;
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = sqlParams.GetCriteriaIdQueryFromParams(RISK_ID, SIZE_ID, ENTITY_TYPE_ID, PERIOD_ID, FREQUENCY_ID);
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["ID"].ToString() != null && rdr["ID"].ToString() != "")
                        criteria_id = Convert.ToInt32(rdr["ID"]);
                }
                cmd.CommandText = "begin Criteria(" + criteria_id + "); end;";
                cmd.ExecuteReader();

                cmd.CommandText = sqlParams.GetCriteriaEntitiesQueryFromParams(RISK_ID, SIZE_ID, ENTITY_TYPE_ID, PERIOD_ID, FREQUENCY_ID);
                OracleDataReader rdr2 = cmd.ExecuteReader();
                while (rdr2.Read())
                {
                    if (rdr2["NO_OF_ENTITY"].ToString() != null && rdr2["NO_OF_ENTITY"].ToString() != "")
                        count = Convert.ToInt32(rdr2["NO_OF_ENTITY"]);
                }
            }
           this.DisposeDatabaseConnection();
            return count;
        }
        public bool DeletePendingCriteria(int CID=0)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_DeletePendingCriteria";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("CID", OracleDbType.Int32).Value = CID;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public bool SubmitAuditCriteriaForApproval(int PERIOD_ID)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_SubmitAuditCriteriaForApproval";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("CID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.ExecuteReader();

                /*cmd.CommandText = "pkg_ais_email.P_ADDAUDITCRITERIA";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr2 = cmd.ExecuteReader();
                while (rdr2.Read())
                {
                    if (rdr2["email_to"].ToString() != "" && rdr2["email_to"].ToString() != null)
                    {
                        email_to = rdr2["email_to"].ToString();

                    }
                    if (rdr2["email_cc"].ToString() != "" && rdr2["email_cc"].ToString() != null)
                    {
                        email_cc = rdr2["email_cc"].ToString();

                    }
                    if (rdr2["subject"].ToString() != "" && rdr2["subject"].ToString() != null)
                    {
                        email_subject = rdr2["subject"].ToString();

                    }
                    if (rdr2["email_body"].ToString() != "" && rdr2["email_body"].ToString() != null)
                    {
                        email_body = rdr2["email_body"].ToString();

                    }
                    EmailConfiguration email = new EmailConfiguration();
                    email.ConfigEmail(email_to, email_cc, email_subject, email_body);
                }*/

            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public List<COSORiskModel> GetCOSORiskForDepartment(int PERIOD_ID = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<COSORiskModel> list = new List<COSORiskModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetCOSORiskForDepartment";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PERIOD_ID", OracleDbType.Int32).Value = PERIOD_ID;
                cmd.Parameters.Add("UserEntityID", OracleDbType.Int32).Value =loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    COSORiskModel chk = new COSORiskModel();
                    chk.AUDIT_PERIOD = rdr["AUDIT_PERIOD"].ToString();
                    chk.DEPT_NAME = rdr["DEPT_NAME"].ToString();
                    chk.RATING_FACTORS = rdr["RATING_FACTORS"].ToString();
                    chk.WEIGHT_ASSIGNED = Convert.ToInt32(rdr["WEIGHT_ASSIGNED"]);
                    chk.SUB_FACTORS = Convert.ToInt32(rdr["SUB_FACTORS"]);
                    chk.MAX_SCORE = Convert.ToInt32(rdr["MAX_SCORE"]);
                    chk.FINAL_SCORE = Convert.ToInt32(rdr["FINAL_SCORE"]);
                    chk.NO_OF_OBSERVATIONS = Convert.ToInt32(rdr["NO_OF_OBSERVATIONS"]);
                    chk.WEIGHTED_AVERAGE_SCORE = Convert.ToInt32(rdr["WEIGHTED_AVERAGE_SCORE"]);
                    chk.AUDIT_RATING = rdr["AUDIT_RATING"].ToString();
                    chk.FINAL_AUDIT_RATING = rdr["FINAL_AUDIT_RATING"].ToString();
                    chk.STATUS = rdr["STATUS"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<COSORiskModel> GetCOSORiskForBranches(int PERIOD_ID = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<COSORiskModel> list = new List<COSORiskModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetCOSORiskForDepartment";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PERIOD_ID", OracleDbType.Int32).Value = PERIOD_ID;
                cmd.Parameters.Add("UserEntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    COSORiskModel chk = new COSORiskModel();
                    chk.AUDIT_PERIOD = rdr["AUDIT_PERIOD"].ToString();
                    chk.DEPT_NAME = rdr["DEPT_NAME"].ToString();
                    chk.RATING_FACTORS = rdr["RATING_FACTORS"].ToString();
                    chk.WEIGHT_ASSIGNED = Convert.ToInt32(rdr["WEIGHT_ASSIGNED"]);
                    chk.SUB_FACTORS = Convert.ToInt32(rdr["SUB_FACTORS"]);
                    chk.MAX_SCORE = Convert.ToInt32(rdr["MAX_SCORE"]);
                    chk.FINAL_SCORE = Convert.ToInt32(rdr["FINAL_SCORE"]);
                    chk.NO_OF_OBSERVATIONS = Convert.ToInt32(rdr["NO_OF_OBSERVATIONS"]);
                    chk.WEIGHTED_AVERAGE_SCORE = Convert.ToInt32(rdr["WEIGHTED_AVERAGE_SCORE"]);
                    chk.AUDIT_RATING = rdr["AUDIT_RATING"].ToString();
                    chk.FINAL_AUDIT_RATING = rdr["FINAL_AUDIT_RATING"].ToString();
                    chk.STATUS = rdr["STATUS"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public bool CAUOMAssignment(CAUOMAssignmentModel om)
        {
            string encodedMsg = encoderDecoder.Encrypt(om.CONTENTS_OF_OM);
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.T_CAU_OM";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OM_NO", OracleDbType.Varchar2).Value = om.OM_NO;
                cmd.Parameters.Add("ENCODED_MSG", OracleDbType.Clob).Value = encodedMsg;
                cmd.Parameters.Add("DIV_ID", OracleDbType.Int32).Value = om.DIV_ID;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public List<CAUOMAssignmentModel> CAUGetAssignedOMs()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();

            List<CAUOMAssignmentModel> list = new List<CAUOMAssignmentModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_CAUGetAssignedOMs";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("UserEntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    CAUOMAssignmentModel chk = new CAUOMAssignmentModel();
                    chk.ID = Convert.ToInt32(rdr["ID"]);
                    chk.DIV_ID = Convert.ToInt32(rdr["DIV_ID"]);
                    chk.STATUS = Convert.ToInt32(rdr["STATUS"]);
                    chk.OM_NO = rdr["OM_NO"].ToString();
                    chk.STATUS_DES = rdr["DISCRIPTION"].ToString();
                    chk.CONTENTS_OF_OM = encoderDecoder.Decrypt(rdr["CONTENTS_OF_OM"].ToString());
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;

        }
        public List<AuditCCQModel> GetCCQ(int ENTITY_ID = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<AuditCCQModel> list = new List<AuditCCQModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetCCQ";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear(); 
                cmd.Parameters.Add("UserEntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditCCQModel chk = new AuditCCQModel();
                    chk.ID = Convert.ToInt32(rdr["ID"]);
                    if (rdr["ENTITY_ID"].ToString() != null && rdr["ENTITY_ID"].ToString() != "")
                    {
                        chk.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                        chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    }
                    else
                    {
                        chk.ENTITY_NAME = "";

                    }

                    chk.QUESTIONS = rdr["QUESTIONS"].ToString();
                    if (rdr["CONTROL_VIOLATION_ID"].ToString() != null && rdr["CONTROL_VIOLATION_ID"].ToString() != "")
                    {
                        chk.CONTROL_VIOLATION_ID = Convert.ToInt32(rdr["CONTROL_VIOLATION_ID"]);
                        chk.CONTROL_VIOLATION = rdr["VIOLATION_NAME"].ToString();

                    }
                    else
                    {
                        chk.CONTROL_VIOLATION = "";
                    }
                    if (rdr["RISK_ID"].ToString() != null && rdr["RISK_ID"].ToString() != "")
                    {
                        chk.RISK_ID = Convert.ToInt32(rdr["RISK_ID"].ToString());
                        chk.RISK = rdr["RISK_DEF"].ToString();
                    }
                    else
                    {
                        chk.RISK = "";
                    }

                    chk.STATUS = rdr["STATUS"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public bool UpdateCCQ(AuditCCQModel ccq)
        {
            bool resp = false;
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_UpdateCCQ";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("CID", OracleDbType.Int32).Value = ccq.ID;
                cmd.Parameters.Add("QUESTIONS", OracleDbType.Varchar2).Value = ccq.QUESTIONS;
                cmd.Parameters.Add("CONTROL_VIOLATION_ID", OracleDbType.Int32).Value = ccq.CONTROL_VIOLATION_ID;
                cmd.Parameters.Add("RISK_ID", OracleDbType.Int32).Value = ccq.RISK_ID;
                cmd.Parameters.Add("STATUS", OracleDbType.Varchar2).Value = ccq.STATUS;
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();
                resp = true;
            }
           this.DisposeDatabaseConnection();
            return resp;
        }
        public List<AuditeeOldParasModel> GetAuditeeOldParasEntities()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<AuditeeOldParasModel> list = new List<AuditeeOldParasModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditeeOldParasEntities";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditeeOldParasModel chk = new AuditeeOldParasModel();
                    chk.ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    chk.ENTITY_NAME = rdr["NAME"].ToString();
                 
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<AuditeeOldParasModel> GetAuditeeOldParas(int ENTITY_ID=0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<AuditeeOldParasModel> list = new List<AuditeeOldParasModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditeeOldParas";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("EntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditeeOldParasModel chk = new AuditeeOldParasModel();
                    chk.ID = Convert.ToInt32(rdr["ID"]);
                    chk.ENTITY_CODE = Convert.ToInt32(rdr["ENTITY_CODE"]);
                    chk.TYPE_ID = Convert.ToInt32(rdr["TYPE_ID"]);
                    chk.AUDIT_PERIOD = Convert.ToInt32(rdr["AUDIT_PERIOD"]);
                    chk.PARA_NO = Convert.ToInt32(rdr["PARA_NO"]);
                    chk.AUDITEDBY = Convert.ToInt32(rdr["AUDITED_BY"]);
                    if (rdr["DATE_OF_LAST_COMPLIANCE_RECEIVED"].ToString() != null && rdr["DATE_OF_LAST_COMPLIANCE_RECEIVED"].ToString() != "")
                        chk.DATE_OF_LAST_COMPLIANCE_RECEIVED = Convert.ToDateTime(rdr["DATE_OF_LAST_COMPLIANCE_RECEIVED"]);


                    chk.GIST_OF_PARAS = rdr["GIST_OF_PARAS"].ToString();
                    chk.AUDITEE_RESPONSE = rdr["AUDITEE_RESPONSE"].ToString();
                    chk.AUDITOR_REMARKS = rdr["AUDITOR_REMARKS"].ToString();


                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.TYPE_DES = rdr["entitytypedesc"].ToString();

                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public bool AuditeeOldParaResponse(AuditeeOldParasResponseModel ob)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            bool success = false;
            var loggedInUser = sessionHandler.GetSessionUser();
            ob.REPLIEDBY = Convert.ToInt32(loggedInUser.PPNumber);
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_AuditeeOldParaResponse";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBSID", OracleDbType.Int32).Value = ob.AU_OBS_ID;
                cmd.Parameters.Add("REPLY", OracleDbType.Clob).Value = ob.REPLY;
                cmd.Parameters.Add("PPNO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();
                success = true;
            }
           this.DisposeDatabaseConnection();
            return success;
        }
        public List<OldParasModel> GetOldParas(string AUDITED_BY, string AUDIT_YEAR)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            List<OldParasModel> list = new List<OldParasModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetOldParas";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = AUDITED_BY;
                cmd.Parameters.Add("AUDITPERIOD", OracleDbType.Int32).Value = AUDIT_YEAR;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;               
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    OldParasModel chk = new OldParasModel();
                    chk.ID = Convert.ToInt32(rdr["ID"]);
                    chk.REF_P = rdr["REF_P"].ToString();
                    chk.ENTITY_ID = rdr["ENTITY_ID"].ToString();
                    chk.ENTITY_CODE = rdr["ENTITY_CODE"].ToString();
                    chk.TYPE_ID = rdr["TYPE_ID"].ToString();
                    chk.AUDIT_PERIOD = rdr["AUDIT_PERIOD"].ToString();
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.PARA_NO = rdr["PARA_NO"].ToString();
                    chk.GIST_OF_PARAS = rdr["GIST_OF_PARAS"].ToString();
                    chk.ANNEXURE = rdr["ANNEXURE"].ToString();
                    chk.AMOUNT_INVOLVED = rdr["AMOUNT_INVOLVED"].ToString();
                    chk.VOL_I_II = rdr["VOL_I_II"].ToString();
                    chk.AUDITED_BY = rdr["AUDITED_BY"].ToString();
                    chk.AUDITEDBY = rdr["AUDITEDBY"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }

        public List<OldParasModelCAD> GetOldParasManagement()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            List<OldParasModelCAD> list = new List<OldParasModelCAD>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetOldParaManagement";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    OldParasModelCAD chk = new OldParasModelCAD();
                    chk.PARA_ID = Convert.ToInt32(rdr["PARA_ID"]);
                    chk.AUDIT_PERIOD = rdr["PERIOD"].ToString();
                    chk.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"].ToString());
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.PARA_NO = rdr["PARA_NO"].ToString();
                    chk.GIST_OF_PARAS = rdr["GIST_OF_PARAS"].ToString();
                    chk.AUDITED_BY = rdr["AUDITED_BY"].ToString();
                    chk.PARA_STATUS = rdr["PARA_STATUS"].ToString();
                    if (rdr["V_CAT_ID"].ToString() != null && rdr["V_CAT_ID"].ToString() != "")
                        chk.V_CAT_ID = Convert.ToInt32(rdr["V_CAT_ID"].ToString());
                    if (rdr["V_CAT_NATURE_ID"].ToString() != null && rdr["V_CAT_NATURE_ID"].ToString() != "")
                        chk.V_CAT_NATURE_ID = Convert.ToInt32(rdr["V_CAT_NATURE_ID"].ToString());
                    if (rdr["RISK_ID"].ToString() != null && rdr["RISK_ID"].ToString() != "")
                        chk.RISK_ID = Convert.ToInt32(rdr["RISK_ID"].ToString());
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<OldParasModel> GetOldParasForResponse()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<OldParasModel> list = new List<OldParasModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetOldParasForResponse";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    OldParasModel chk = new OldParasModel();
                    chk.ID = Convert.ToInt32(rdr["ID"]);
                    chk.REF_P = rdr["REF_P"].ToString();
                    chk.ENTITY_ID = rdr["ENTITY_ID"].ToString();
                    chk.ENTITY_CODE = rdr["ENTITY_CODE"].ToString();
                    chk.TYPE_ID = rdr["TYPE_ID"].ToString();
                    chk.AUDIT_PERIOD = rdr["AUDIT_PERIOD"].ToString();
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.PARA_NO = rdr["PARA_NO"].ToString();
                    chk.GIST_OF_PARAS = rdr["GIST_OF_PARAS"].ToString();
                    chk.ANNEXURE = rdr["ANNEXURE"].ToString();
                    chk.AMOUNT_INVOLVED = rdr["AMOUNT_INVOLVED"].ToString();
                    chk.VOL_I_II = rdr["VOL_I_II"].ToString();
                    chk.AUDITED_BY = rdr["AUDITED_BY"].ToString();
                    chk.AUDITEDBY = rdr["AUDITEDBY"].ToString();
                    chk.PROCESS_DES = rdr["Process_Des"].ToString();
                    chk.SUB_PROCESS_DES = rdr["Sub_process_Des"].ToString();
                    chk.PROCESS_DETAIL_DES = rdr["Check_List_Detail_Des"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<AuditeeOldParasModel> GetOutstandingParas(string ENTITY_ID)
        {
            List<AuditeeOldParasModel> list = new List<AuditeeOldParasModel>();
            return list;
        }
        public List<OldParasModel> GetOldParasAuditYear()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            List<OldParasModel> list = new List<OldParasModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetOldParasAuditYear";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    OldParasModel chk = new OldParasModel();
                    chk.AUDIT_PERIOD = rdr["AUDIT_PERIOD"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<OldParasModel> GetOutstandingParasAuditYear()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            List<OldParasModel> list = new List<OldParasModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetOutstandingParasAuditYear";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    OldParasModel chk = new OldParasModel();
                    chk.AUDIT_PERIOD = rdr["AUDIT_PERIOD"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public bool AddOldParas(OldParasModel jm)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();

            using (OracleCommand cmd = con.CreateCommand())
            {
                List<int> PP_NOs = new List<int>();
                jm.STATUS = 1;
                jm.ENTERED_BY = loggedInUser.PPNumber;
                if (jm.RESPONSIBLE_PP_NO != "" && jm.RESPONSIBLE_PP_NO != null)
                {
                    PP_NOs = jm.RESPONSIBLE_PP_NO.Split(',').Select(int.Parse).ToList();
                }
                cmd.CommandText = "pkg_ais.P_AddOldParas";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PROCESS", OracleDbType.Int32).Value = jm.PROCESS;
                cmd.Parameters.Add("SUBPROCESS", OracleDbType.Int32).Value = jm.SUB_PROCESS;
                cmd.Parameters.Add("PROCESSDETAIL", OracleDbType.Int32).Value = jm.PROCESS_DETAIL;
                cmd.Parameters.Add("PPNO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("REPLYTEXT", OracleDbType.Clob).Value = jm.PARA_TEXT;
                cmd.Parameters.Add("PID", OracleDbType.Clob).Value = jm.ID;
                cmd.ExecuteReader();
                foreach (int pp in PP_NOs)
                {
                    cmd.CommandText = "pkg_ais.P_AddOldParasResponsibilityAssigned";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("REF_P", OracleDbType.Int32).Value = jm.ID;
                    cmd.Parameters.Add("PPNO", OracleDbType.Int32).Value = pp;
                    cmd.ExecuteReader();
                }
            }
           this.DisposeDatabaseConnection();
            return true;
        }
        public bool AddOldParasReply(int ID, string REPLY)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            bool success = false;
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_AddOldParasReply";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNO", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("REPLY", OracleDbType.Clob).Value = REPLY;
                cmd.Parameters.Add("PID", OracleDbType.Int32).Value = ID;
                cmd.ExecuteReader();
                success = true;
            }
           this.DisposeDatabaseConnection();
            return success;
        }
        public string AddOldParasCADReply(int ID, int V_CAT_ID, int V_CAT_NATURE_ID, int RISK_ID, string REPLY)
        {
            string response = "";
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_updateoldparamanagement";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PARAID", OracleDbType.Int32).Value = ID;
                cmd.Parameters.Add("VCATID", OracleDbType.Int32).Value = V_CAT_ID;
                cmd.Parameters.Add("VCATNATUREID", OracleDbType.Int32).Value = V_CAT_NATURE_ID;
                cmd.Parameters.Add("RISKID", OracleDbType.Int32).Value = RISK_ID;
                cmd.Parameters.Add("PARATEXT", OracleDbType.Clob).Value = REPLY;
                cmd.Parameters.Add("CREATEDBY", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    response = rdr["REMARKS"].ToString();
                }

            }
           this.DisposeDatabaseConnection();
            return response;
        }
        public string AddOldParasCADCompliance(OldParaComplianceModel opc)
        {
            string response = "";
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_UpdateAuditeeOldParasresponse";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("Paraid", OracleDbType.Int32).Value = opc.ParaRef;
                cmd.Parameters.Add("cdate", OracleDbType.Date).Value = opc.ComplianceDate;
                cmd.Parameters.Add("Text", OracleDbType.Clob).Value = opc.AuditeeCompliance;
                cmd.Parameters.Add("PPNO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("Remarks", OracleDbType.Clob).Value = opc.AuditorRemarks;
                cmd.Parameters.Add("imprec", OracleDbType.NVarchar2).Value = opc.CnIRecommendation;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    response = rdr["REMARKS"].ToString();
                }

            }
           this.DisposeDatabaseConnection();
            return response;
        }

        public List<ZoneWiseOldParasPerformanceModel> GetZoneWiseOldParasPerformance()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            string query = "";
            query = query + "  s.ID=" + loggedInUser.UserEntityID;


            List<ZoneWiseOldParasPerformanceModel> list = new List<ZoneWiseOldParasPerformanceModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetZoneWiseOldParasPerformance";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("UserEntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ZoneWiseOldParasPerformanceModel chk = new ZoneWiseOldParasPerformanceModel();
                    chk.ZONEID = rdr["ID"].ToString();
                    chk.ZONENAME = rdr["ZONENAME"].ToString();
                    chk.PARA_ENTERED = rdr["PARA_ENTERED"].ToString();
                    chk.PARA_PENDING = rdr["PARA_PENDING"].ToString();
                    chk.PARA_TOTAL = rdr["PARA_TOTAL"].ToString();

                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<UserWiseOldParasPerformanceModel> GetUserWiseOldParasPerformance()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
          
            List<UserWiseOldParasPerformanceModel> list = new List<UserWiseOldParasPerformanceModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetUserWiseOldParasPerformance";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("UserEntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserWiseOldParasPerformanceModel chk = new UserWiseOldParasPerformanceModel();
                    chk.AUDIT_ZONEID = rdr["AUDIT_ZONEID"].ToString();
                    chk.ZONENAME = rdr["ZONENAME"].ToString();
                    chk.PARA_ENTERED = rdr["PARA_ENTERED"].ToString();
                    chk.PPNO = rdr["PPNO"].ToString();
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public ActiveInactiveChart GetActiveInactiveChartData()
        {
            var con = this.DatabaseConnection();
            ActiveInactiveChart chk = new ActiveInactiveChart();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetActiveInactiveChartData";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {

                    if (rdr["STATUS"].ToString() == "1")
                        chk.Active_Count = rdr["TOTAL_COUNT"].ToString();
                    if (rdr["STATUS"].ToString() == "0")
                        chk.Inactive_Count = rdr["TOTAL_COUNT"].ToString();
                }
            }
           this.DisposeDatabaseConnection();
            return chk;
        }
        public List<AuditeeEntitiesModel> GetObservationEntities()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<AuditeeEntitiesModel> list = new List<AuditeeEntitiesModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_GetObservationEntities";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PP_NO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                 while (rdr.Read())
                {
                    AuditeeEntitiesModel chk = new AuditeeEntitiesModel();
                    chk.CODE = Convert.ToInt32(rdr["CODE"].ToString());
                    chk.NAME = rdr["NAME"].ToString();
                    chk.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"].ToString());
                    chk.ENG_ID = Convert.ToInt32(rdr["eng_plan_id"].ToString());
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<AuditeeEntitiesModel> GetAuditeeAssignedEntities()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<AuditeeEntitiesModel> list = new List<AuditeeEntitiesModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditeeAssignedEntities";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditeeEntitiesModel chk = new AuditeeEntitiesModel();
                    chk.CODE = Convert.ToInt32(rdr["CODE"].ToString());
                    chk.NAME = rdr["NAME"].ToString();
                    chk.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"].ToString());
                    chk.ENG_ID = Convert.ToInt32(rdr["engplanid"].ToString());
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<AuditeeEntitiesModel> GetCCQsEntities()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<AuditeeEntitiesModel> list = new List<AuditeeEntitiesModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetCCQsEntities";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditeeEntitiesModel chk = new AuditeeEntitiesModel();
                    chk.CODE = Convert.ToInt32(rdr["CODE"].ToString());
                    chk.NAME = rdr["NAME"].ToString();
                    chk.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"].ToString());
                    list.Add(chk);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<UserRelationshipModel> Getchildposting(int e_r_id = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

            if (e_r_id == 0)
                e_r_id = Convert.ToInt32(loggedInUser.UserEntityID);

            List<UserRelationshipModel> entitiesList = new List<UserRelationshipModel>();
            var con = this.DatabaseConnection();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_Getchildposting";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("erid", OracleDbType.Int32).Value = e_r_id;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserRelationshipModel entity = new UserRelationshipModel();
                    entity.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    entity.C_NAME = rdr["C_NAME"].ToString();
                    entitiesList.Add(entity);
                }
            }
           this.DisposeDatabaseConnection();
            return entitiesList;

        }
        public List<UserRelationshipModel> Getparentrepoffice(int r_id = 0)
        {

            List<UserRelationshipModel> entitiesList = new List<UserRelationshipModel>();
            var con = this.DatabaseConnection();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_Getparentrepoffice";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("rid", OracleDbType.Int32).Value = r_id;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserRelationshipModel entity = new UserRelationshipModel();
                    entity.ENTITY_REALTION_ID = Convert.ToInt32(rdr["ENTITY_REALTION_ID"]);
                    entity.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    entity.ACTIVE = rdr["ACTIVE"].ToString();
                    entity.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    entity.ENTITYTYPEDESC = rdr["ENTITYTYPEDESC"].ToString();
                    entitiesList.Add(entity);
                }
            }
           this.DisposeDatabaseConnection();
            return entitiesList;

        }
        public List<UserRelationshipModel> Getrealtionshiptype()
        {

            List<UserRelationshipModel> entitiesList = new List<UserRelationshipModel>();
            var con = this.DatabaseConnection();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_Getrealtionshiptype";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserRelationshipModel entity = new UserRelationshipModel();
                    entity.ENTITY_REALTION_ID = Convert.ToInt32(rdr["ENTITY_REALTION_ID"]);
                    entity.FIELD_NAME = rdr["FIELD_NAME"].ToString();
                    entitiesList.Add(entity);
                }
            }
           this.DisposeDatabaseConnection();
            return entitiesList;

        }
        public List<StaffPositionModel> GetStaffPosition()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

            List<StaffPositionModel> list = new List<StaffPositionModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetStaffPosition";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();


                while (rdr.Read())
                {
                    StaffPositionModel staffposition = new StaffPositionModel();
                    staffposition.PPNO = Convert.ToInt32(rdr["PPNO"]);
                    staffposition.EMPLOYEE_NAME = Convert.ToString(rdr["EMPLOYEE_NAME"]);

                    staffposition.QUALIFICATION = Convert.ToString(rdr["QUALIFICATION"]);
                    staffposition.DATE_OF_POSTING = Convert.ToDateTime(rdr["DATE_OF_POSTING"]);
                    staffposition.DESIGNATION = Convert.ToString(rdr["DESIGNATION"]);
                    staffposition.RANK_DESC = Convert.ToString(rdr["RANK_DESC"]);
                    staffposition.PLACE_OF_POSTING = Convert.ToString(rdr["PLACE_OF_POSTING"]);


                    list.Add(staffposition);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<FunctionalResponsibilityWiseParas> GetFunctionalResponsibilityWisePara(int PROCESS_ID = 0, int SUB_PROCESS_ID = 0, int PROCESS_DETAIL_ID = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

            List<FunctionalResponsibilityWiseParas> list = new List<FunctionalResponsibilityWiseParas>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetFunctionalResponsibilityWisePara";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("PROCESSID", OracleDbType.Int32).Value = PROCESS_ID;
                cmd.Parameters.Add("SUB_PROCESSID", OracleDbType.Int32).Value = SUB_PROCESS_ID;
                cmd.Parameters.Add("PROCESS_DETAILID", OracleDbType.Int32).Value = PROCESS_DETAIL_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();


                while (rdr.Read())
                {
                    FunctionalResponsibilityWiseParas para = new FunctionalResponsibilityWiseParas();
                    para.PROCESS_ID = Convert.ToInt32(rdr["PROCESS_ID"].ToString());
                    para.PROCESS = rdr["PROCESS"].ToString();
                    para.SUB_PROCESS_ID = Convert.ToInt32(rdr["SUB_PROCESS_ID"].ToString());
                    para.VIOLATION = rdr["VIOLATION"].ToString();
                    para.CHECK_LIST_DETAIL_ID = Convert.ToInt32(rdr["CHECK_LIST_DETAIL_ID"].ToString());
                    para.PERIOD = rdr["PERIOD"].ToString();
                    para.OBS_ID = Convert.ToInt32(rdr["OBS_ID"].ToString());
                    para.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    para.SUB_PROCESS = rdr["SUB_PROCESS"].ToString();
                    para.MEMO_NO = rdr["MEMO_NO"].ToString();
                    para.OBS_TEXT = rdr["OBS_TEXT"].ToString();
                    para.OBS_RISK_ID = Convert.ToInt32(rdr["OBS_RISK_ID"].ToString());
                    para.OBS_RISK = rdr["OBS_RISK"].ToString();
                    para.OBS_STATUS_ID = Convert.ToInt32(rdr["OBS_STATUS_ID"].ToString());
                    para.OBS_STATUS = rdr["OBS_STATUS"].ToString();
                    list.Add(para);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }

        
        public bool AddDivisionalHeadRemarksOnFunctionalLegacyPara(int CONCERNED_DEPT_ID = 0, string COMMENTS = "", int REF_PARA_ID = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_AddDivisionalHeadRemarksOnFunctionalLegacyPara";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("CONCERNED_DEPTID", OracleDbType.Int32).Value = CONCERNED_DEPT_ID;
                cmd.Parameters.Add("COMMENTS", OracleDbType.Varchar2).Value = COMMENTS;
                cmd.Parameters.Add("REF_PARAID", OracleDbType.Int32).Value = REF_PARA_ID;
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();
            }
           this.DisposeDatabaseConnection();
            return true;
        }

        [Obsolete]
        public void SaveImage(string base64img, string outputImgFilename = "image.jpg")
        {
            var folderPath = System.IO.Path.Combine(_env.WebRootPath, "Auditee_Evidences");
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            System.IO.File.WriteAllBytes(Path.Combine(folderPath, outputImgFilename), Convert.FromBase64String(base64img));
        }

        public string CreateAuditReport(int ENG_ID)
        {
            List<ManageObservations> list = new List<ManageObservations>();
            string filename = "";
            return filename;

            /*list = this.GetManagedObservations(ENG_ID, 0);
            var folderPath = "";
            string entityname = list[0].ENTITY_NAME;
            string period = list[0].PERIOD;
            using (MemoryStream mem = new MemoryStream())
            {
                StringBuilder sb = new StringBuilder();
                //Table For Practice
                sb.Append(@"<center><h1><u>Audit Report on " + entityname + " </u></h1><h3>" + period + "</h3><h3>Version: Draft</h3></center>");

                sb.Append(@"<br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><h1>Audit Observations</h1>");



                foreach(var item in list)
                {
                    List<object> outText = new List<object>();

                    outText=this.GetObservationText(item.OBS_ID,0);
                    sb.Append("<h3 style='margin-top:50px;'>Memo No : "+item.MEMO_NO+"</h3>");
                    sb.Append("<div style='margin-top:10px;'>"+ outText [0]+ "</div>");
                    sb.Append("<h3 style='margin-top:10px;'>Auditee Reply</h3>");
                    sb.Append("<div style='margin-top:10px;'>" + outText[1] + "</div>");

                }              

               
                string path = "";
               
                //ltTable.Text = sb.ToString();
                folderPath = System.IO.Path.Combine(_env.WebRootPath, "Audit_Reports");
                if (!System.IO.Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }
                filename = "DraftReport_" + ENG_ID + ".Pdf"; ;
                //path = Path.Combine(contentRootPath, filename + ".Pdf");
                path = Path.Combine(folderPath, filename);

                PdfWriter writer = new PdfWriter(path);
                PdfDocument pdf = new PdfDocument(writer);
                pdf.SetDefaultPageSize(iText.Kernel.Geom.PageSize.A0);                

                ConverterProperties converterProperties = new ConverterProperties();
                PdfDocument pdfDocument = new PdfDocument(writer);
                
                iText.Layout.Document document = HtmlConverter.ConvertToDocument(sb.ToString(), pdfDocument, converterProperties);



                var xmlParse = new XMLParser();
                xmlParse.Parse(new StringReader(sb.ToString()));
                xmlParse.Flush();

                document.Close();

                
            }
            return filename;
            */
        }
        public List<Glheadsummaryyearlymodel> GetGlheadDetailsyearwise(int gl_code = 0)
        {
            int ENG_ID = this.GetLoggedInUserEngId();
            var con = this.DatabaseConnection();

            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

            List<Glheadsummaryyearlymodel> list = new List<Glheadsummaryyearlymodel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.p_getglheadsummary_Yearly";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                // cmd.Parameters.Add("GLSUBCODE", OracleDbType.Int32).Value = gl_code;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Glheadsummaryyearlymodel GlHeadDetails = new Glheadsummaryyearlymodel();
                    GlHeadDetails.GLSUBCODE = Convert.ToInt32(rdr["GLSUBCODE"]);
                    GlHeadDetails.BRANCHID = Convert.ToInt32(rdr["BRANCHID"]);
                    GlHeadDetails.GLSUBNAME = rdr["GLSUBNAME"].ToString();
                    //GlHeadDetails.GL_TYPEID = Convert.ToInt32(rdr["GL_TYPEID"]);

                    //GlHeadDetails.DESCRIPTION = rdr["DESCRIPTION"].ToString();


                    //GlHeadDetails.DATETIME = Convert.ToDateTime(rdr["DATETIME"]);
                    if (rdr["BALANCE_2021"].ToString() != null && rdr["BALANCE_2021"].ToString() != "")
                        GlHeadDetails.BALANCE_2021 = Convert.ToDouble(rdr["BALANCE_2021"]);
                    if (rdr["DEBIT_2021"].ToString() != null && rdr["DEBIT_2021"].ToString() != "")
                        GlHeadDetails.DEBIT_2021 = Convert.ToDouble(rdr["DEBIT_2021"]);
                    if (rdr["CREDIT_2021"].ToString() != null && rdr["CREDIT_2021"].ToString() != "")
                        GlHeadDetails.CREDIT_2021 = Convert.ToDouble(rdr["CREDIT_2021"]);
                    if (rdr["BALANCE_2022"].ToString() != null && rdr["BALANCE_2022"].ToString() != "")
                        GlHeadDetails.BALANCE_2022 = Convert.ToDouble(rdr["BALANCE_2022"]);
                    if (rdr["DEBIT_2022"].ToString() != null && rdr["DEBIT_2022"].ToString() != "")
                        GlHeadDetails.DEBIT_2022 = Convert.ToDouble(rdr["DEBIT_2022"]);
                    if (rdr["CREDIT_2022"].ToString() != null && rdr["CREDIT_2022"].ToString() != "")
                        GlHeadDetails.CREDIT_2022 = Convert.ToDouble(rdr["CREDIT_2022"]);
                    list.Add(GlHeadDetails);
                }
            }
           this.DisposeDatabaseConnection();
            return list;

        }
        public List<DepositAccountCatModel> GetDepositCat()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            int ENG_ID = this.GetLoggedInUserEngId();

            var con = this.DatabaseConnection();
            List<DepositAccountCatModel> list = new List<DepositAccountCatModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_AIS.P_GetDepositACCOUNTCATEGORY";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    DepositAccountCatModel depcat = new DepositAccountCatModel();

                    depcat.BRANCH_NAME = rdr["BRANCH_NAME"].ToString();
                    depcat.ACCOUNTCATEGORY = rdr["ACCOUNTCATEGORY"].ToString();
                    depcat.ACCOUNTCATEGORYID = Convert.ToInt32(rdr["ACCOUNTCATEGORYID"]);

                    depcat.ACCOCUNTSTATUS = rdr["ACCOCUNTSTATUS"].ToString();

                    if (rdr["AMOUNT"].ToString() != null && rdr["AMOUNT"].ToString() != "")
                        depcat.AMOUNT = Convert.ToDouble(rdr["AMOUNT"]);

                    list.Add(depcat);
                }
            }
           this.DisposeDatabaseConnection();
            return list;

        }

        public List<DepositAccountCatDetailsModel> GetDepositAccountcatdetails(int catid = 0)

        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            int ENG_ID = this.GetLoggedInUserEngId();
            var con = this.DatabaseConnection();
            List<DepositAccountCatDetailsModel> depositaccsublist = new List<DepositAccountCatDetailsModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_AIS.P_GetDepositACCOUNTCATEGORY_details";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("catid", OracleDbType.Int32).Value = catid;

                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DepositAccountCatDetailsModel depositaccsubdetails = new DepositAccountCatDetailsModel();

                    depositaccsubdetails.BRANCH_NAME = rdr["BRANCH_NAME"].ToString();
                    if (rdr["ACC_NUMBER"].ToString() != null && rdr["ACC_NUMBER"].ToString() != "")
                        depositaccsubdetails.ACC_NUMBER = Convert.ToDouble(rdr["ACC_NUMBER"]);
                    if (rdr["ACCOUNTCATEGORY"].ToString() != null && rdr["ACCOUNTCATEGORY"].ToString() != "")
                        depositaccsubdetails.ACCOUNTCATEGORY = rdr["ACCOUNTCATEGORY"].ToString();

                    if (rdr["CUSTOMERNAME"].ToString() != null && rdr["CUSTOMERNAME"].ToString() != "")
                        depositaccsubdetails.CUSTOMERNAME = rdr["CUSTOMERNAME"].ToString();
                    if (rdr["BMVS_VERIFIED"].ToString() != null && rdr["BMVS_VERIFIED"].ToString() != "")
                        depositaccsubdetails.BMVS_VERIFIED = rdr["BMVS_VERIFIED"].ToString();


                    if (rdr["OPENINGDATE"].ToString() != null && rdr["OPENINGDATE"].ToString() != "")
                    {
                        depositaccsubdetails.OPENINGDATE = Convert.ToDateTime(rdr["OPENINGDATE"]);
                    }
                    if (rdr["CNIC"].ToString() != null && rdr["CNIC"].ToString() != "")
                    {
                        depositaccsubdetails.CNIC = Convert.ToDouble(rdr["CNIC"]);
                    }
                    if (rdr["TITLE"].ToString() != null && rdr["TITLE"].ToString() != "")
                        depositaccsubdetails.TITLE = rdr["TITLE"].ToString();


                    if (rdr["ACCOCUNTSTATUS"].ToString() != null && rdr["ACCOCUNTSTATUS"].ToString() != "")
                        depositaccsubdetails.ACCOUNTSTATUS = rdr["ACCOCUNTSTATUS"].ToString();
                    if (rdr["LASTTRANSACTIONDATE"].ToString() != null && rdr["LASTTRANSACTIONDATE"].ToString() != "")
                    {
                        depositaccsubdetails.LASTTRANSACTIONDATE = Convert.ToDateTime(rdr["LASTTRANSACTIONDATE"]);
                    }
                    if (rdr["CNICEXPIRYDATE"].ToString() != null && rdr["CNICEXPIRYDATE"].ToString() != "")
                    {
                        depositaccsubdetails.CNICEXPIRYDATE = Convert.ToDateTime(rdr["CNICEXPIRYDATE"]);
                    }
                    depositaccsublist.Add(depositaccsubdetails);
                }
            }
           this.DisposeDatabaseConnection();
            return depositaccsublist;
        }

        public List<AuditPlanEngagementModel> GetAuditPlanEngagement(int periodid)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            List<AuditPlanEngagementModel> periodList = new List<AuditPlanEngagementModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais_reports.r_audit_plan_engagement";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("Audit_Period", OracleDbType.Int32).Value = periodid;

                cmd.Parameters.Add("auditbyid", OracleDbType.Int32).Value = loggedInUser.UserEntityID;

                
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    AuditPlanEngagementModel period = new AuditPlanEngagementModel();



                    period.AUDITPERIOD = rdr["AUDITPERIOD"].ToString();
                    period.PARENT_OFFICE = rdr["PARENT_OFFICE"].ToString();
                    period.ENITIY_NAME = rdr["ENITIY_NAME"].ToString();
                    period.PARENT_OFFICE = rdr["PARENT_OFFICE"].ToString();

                    period.AUDIT_STARTDATE = Convert.ToDateTime(rdr["AUDIT_STARTDATE"]);

                    period.AUDIT_ENDDATE = Convert.ToDateTime(rdr["AUDIT_ENDDATE"]);
                    if (rdr["TRAVEL_DAY"].ToString() != null && rdr["TRAVEL_DAY"].ToString() != "")
                        period.TRAVEL_DAY = Convert.ToInt32(rdr["TRAVEL_DAY"]);
                    if (rdr["REVENUE_RECORD_DAY"].ToString() != null && rdr["REVENUE_RECORD_DAY"].ToString() != "")
                        period.REVENUE_RECORD_DAY = Convert.ToInt32(rdr["REVENUE_RECORD_DAY"]);
                    if (rdr["DISCUSSION_DAY"].ToString() != null && rdr["DISCUSSION_DAY"].ToString() != "")
                        period.DISCUSSION_DAY = Convert.ToInt32(rdr["DISCUSSION_DAY"]);

                    period.TEAM_NAME = rdr["TEAM_NAME"].ToString();
                    // period.MEMBER_NAME = rdr["MEMBER_NAME"].ToString();
                    period.STATUS = rdr["STATUS"].ToString();


                    periodList.Add(period);
                }
            }
           this.DisposeDatabaseConnection();
            return periodList;
        }

        public List<LoanSchemeModel> GetLoansScheme()
        {
            int ENG_ID = this.GetLoggedInUserEngId();
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

            List<LoanSchemeModel> list = new List<LoanSchemeModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_AIS.P_preauditinfo_loan_scheme";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;

                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    LoanSchemeModel LoanSchemeDetails = new LoanSchemeModel();

                    LoanSchemeDetails.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    //LoanSchemeDetails.DISB_STATUSID = Convert.ToInt32(rdr["DISB_STATUSID"]);
                    LoanSchemeDetails.GLSUBCODE = Convert.ToInt32(rdr["GLSUBCODE"]);
                    LoanSchemeDetails.GLSUBNAME = rdr["GLSUBNAME"].ToString();
                    LoanSchemeDetails.DISBURSED_AMOUNT = Convert.ToDouble(rdr["DISBURSED_AMOUNT"]);



                    LoanSchemeDetails.PRIN_OUT = Convert.ToDouble(rdr["PRIN_OUT"]);
                    LoanSchemeDetails.MARKUP_OUT = Convert.ToDouble(rdr["MARKUP_OUT"]);



                    list.Add(LoanSchemeDetails);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }


        public List<LoanSchemeYearlyModel> GetLoansSchemeYearly()
        {
            int ENG_ID = this.GetLoggedInUserEngId();
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

            List<LoanSchemeYearlyModel> list = new List<LoanSchemeYearlyModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_AIS.P_preauditinfo_loan_scheme_yearly";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;

                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    LoanSchemeYearlyModel LoanSchemeDetails = new LoanSchemeYearlyModel();

                    LoanSchemeDetails.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    LoanSchemeDetails.DISB_STATUSID = Convert.ToInt32(rdr["DISB_STATUSID"]);
                    LoanSchemeDetails.GLSUBCODE = Convert.ToInt32(rdr["GLSUBCODE"]);
                    LoanSchemeDetails.GLSUBNAME = rdr["GLSUBNAME"].ToString();
                    if (rdr["DISBURSED_AMOUNT_2021"].ToString() != null && rdr["DISBURSED_AMOUNT_2021"].ToString() != "")
                        LoanSchemeDetails.DISBURSED_AMOUNT_2021 = Convert.ToDouble(rdr["DISBURSED_AMOUNT_2021"]);


                    if (rdr["PRIN_OUT_2021"].ToString() != null && rdr["PRIN_OUT_2021"].ToString() != "")
                        LoanSchemeDetails.PRIN_OUT_2021 = Convert.ToDouble(rdr["PRIN_OUT_2021"]);
                    if (rdr["MARKUP_OUT_2021"].ToString() != null && rdr["MARKUP_OUT_2021"].ToString() != "")
                        LoanSchemeDetails.MARKUP_OUT_2021 = Convert.ToDouble(rdr["MARKUP_OUT_2021"]);
                    if (rdr["DISBURSED_AMOUNT_2022"].ToString() != null && rdr["DISBURSED_AMOUNT_2022"].ToString() != "")
                        LoanSchemeDetails.DISBURSED_AMOUNT_2022 = Convert.ToDouble(rdr["DISBURSED_AMOUNT_2022"]);


                    if (rdr["PRIN_OUT_2022"].ToString() != null && rdr["PRIN_OUT_2022"].ToString() != "")
                        LoanSchemeDetails.PRIN_OUT_2022 = Convert.ToDouble(rdr["PRIN_OUT_2022"]);
                    if (rdr["MARKUP_OUT_2022"].ToString() != null && rdr["MARKUP_OUT_2022"].ToString() != "")
                        LoanSchemeDetails.MARKUP_OUT_2022 = Convert.ToDouble(rdr["MARKUP_OUT_2022"]);

                    list.Add(LoanSchemeDetails);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }

        public List<FadOldParaReportModel> GetFadBranchesParas(int PROCESS_ID = 0, int SUB_PROCESS_ID = 0, int PROCESS_DETAIL_ID = 0)
        {
            List<FadOldParaReportModel> list = new List<FadOldParaReportModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "PKG_AIS_reports.r_functionalresp";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.Add("CID", OracleDbType.Int32).Value = PROCESS_ID;
                cmd.Parameters.Add("SID", OracleDbType.Int32).Value = SUB_PROCESS_ID;
                cmd.Parameters.Add("CDID", OracleDbType.Int32).Value = PROCESS_DETAIL_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    FadOldParaReportModel para = new FadOldParaReportModel();

                    para.PERIOD = Convert.ToInt32(rdr["PERIOD"].ToString());
                    para.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    para.PROCESS = rdr["PROCESS"].ToString();
                    para.SUB_PROCESS = rdr["SUB_PROCESS"].ToString();
                    para.VIOLATION = rdr["VIOLATION"].ToString();
                    para.OBS_TEXT = rdr["OBS_TEXT"].ToString();
                    para.OBS_RISK = rdr["OBS_RISK"].ToString();
                    para.OBS_STATUS = rdr["OBS_STATUS"].ToString();
                    list.Add(para);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }

        public List<JoiningCompletionReportModel> GetJoiningCompletion(int DEPT_ID, DateTime AUDIT_STARTDATE, DateTime AUDIT_ENDDATE)
        {
            List<JoiningCompletionReportModel> list = new List<JoiningCompletionReportModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_AIS_REPORTS.R_JOININGCOMPLETION";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("DEPT_ID", OracleDbType.Int32).Value = DEPT_ID;
                cmd.Parameters.Add("AUDIT_START", OracleDbType.Date).Value = AUDIT_STARTDATE;
                cmd.Parameters.Add("AUDIT_END", OracleDbType.Date).Value = AUDIT_ENDDATE;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    JoiningCompletionReportModel jc = new JoiningCompletionReportModel();

                    jc.AUDIT_BY = rdr["AUDIT_BY"].ToString();
                    jc.AUDITEE_NAME = rdr["AUDITEE_NAME"].ToString();
                    jc.TEAM_NAME = rdr["TEAM_NAME"].ToString();
                    jc.PPNO = Convert.ToInt32(rdr["PPNO"].ToString());
                    jc.NAME= rdr["NAME"].ToString();
                    jc.TEAM_LEAD = rdr["TEAM_LEAD"].ToString();
                    jc.JOINING_DATE = Convert.ToDateTime(rdr["JOINING_DATE"].ToString());
                    jc.COMPLETION_DATE = Convert.ToDateTime(rdr["COMPLETION_DATE"].ToString());
                    jc.STATUS = rdr["STATUS"].ToString();

                    list.Add(jc);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }

        public List<AuditPlanCompletionReportModel> GetauditplanCompletion(int DEPT_ID)
        {
            List<AuditPlanCompletionReportModel> list = new List<AuditPlanCompletionReportModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_AIS_REPORTS.R_AUDITPLANPROGRESS";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("DEPT_ID", OracleDbType.Int32).Value = DEPT_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    AuditPlanCompletionReportModel jac = new AuditPlanCompletionReportModel();

                    jac.AUDITNAME = rdr["AUDITNAME"].ToString();
                    jac.AUDITS = Convert.ToInt32(rdr["AUDITS"].ToString());
                    jac.ENGPLAN = Convert.ToInt32(rdr["ENGPLAN"].ToString());
                    jac.JOINING = Convert.ToInt32(rdr["JOINING"].ToString());
                    jac.COMPLETED = Convert.ToInt32(rdr["COMPLETED"].ToString());
                    jac.OBSERVATIONS = Convert.ToInt32(rdr["OBSERVATIONS"].ToString());
                    jac.HIGHRISKPARA = Convert.ToInt32(rdr["HIGHRISKPARA"].ToString());
                    jac.MEDIUMRISKPARA = Convert.ToInt32(rdr["MEDIUMRISKPARA"].ToString());
                    jac.LOWRISKPARA = Convert.ToInt32(rdr["LOWRISKPARA"].ToString());
                    list.Add(jac);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }

        public DraftReportSummaryModel GetDraftReportSummary(int ENG_ID=0, int OBS_ID=0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            List<ManageObservations> paras = new List<ManageObservations>();
            DraftReportSummaryModel list = new DraftReportSummaryModel();

            if (loggedInUser.UserLocationType == "Z")
            {
                paras = this.GetManagedObservationsForBranches(ENG_ID, OBS_ID);
            }
            else
            { 
                paras = this.GetManagedObservations(ENG_ID, OBS_ID); 
            }
            
            foreach( var p in paras)
            {
                list.Total++;
                if (p.OBS_STATUS_ID == 7)
                    list.Dropped++;
                if (p.OBS_STATUS_ID == 5)
                    list.AddtoDraft++;
                if (p.OBS_STATUS_ID == 4)
                    list.Settled++;
                if (p.OBS_RISK_ID == 3)
                    list.Low++;
                if (p.OBS_RISK_ID == 2)
                    list.Medium++;
                if (p.OBS_RISK_ID == 1)
                    list.High++;
            }         
          
            return list;
        }
        public List<CurrentAuditProgress> GetCurrentAuditProgressEntities()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            List<CurrentAuditProgress> list = new List<CurrentAuditProgress>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_AIS_REPORTS.R_GetAuditees";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PP_NO", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    CurrentAuditProgress ent = new CurrentAuditProgress();
                    ent.CODE = rdr["ENG_ID"].ToString();
                    ent.NAME = rdr["Entity_Name"].ToString();
                    list.Add(ent);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<CurrentAuditProgress> GetCurrentAuditProgress(int ENTITY_ID=0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            List<CurrentAuditProgress> list = new List<CurrentAuditProgress>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_AIS_REPORTS.R_GetAuditeesobervations";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENGID", OracleDbType.Int32).Value = ENTITY_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    CurrentAuditProgress ent = new CurrentAuditProgress();
                    //e.code, e.name as auditee, d.heading as area, count(o.id) as observation

                    ent.CODE = rdr["code"].ToString();
                    ent.NAME = rdr["auditee"].ToString();
                    ent.AREA = rdr["area"].ToString();
                    ent.OBS_COUNT = Convert.ToInt32(rdr["observation"].ToString());
                    list.Add(ent);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
        public List<CurrentActiveUsers> GetCurrentActiveUsers()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            List<CurrentActiveUsers> list = new List<CurrentActiveUsers>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_AIS_REPORTS.R_LOGINUSERS";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    CurrentActiveUsers ent = new CurrentActiveUsers();
                    //e.code, e.name as auditee, d.heading as area, count(o.id) as observation

                    ent.DEPARTMENT_NAME = rdr["DEPTNAME"].ToString();
                    ent.NAME = rdr["EMPNAME"].ToString();
                    ent.PP_NUMBER =Convert.ToInt32(rdr["PPNO"].ToString());
                    ent.LOGGED_IN_DATE = Convert.ToDateTime(rdr["LOGINDATE"].ToString());
                    ent.SESSION_TIME = rdr["SESSIONTIME"].ToString();
                    list.Add(ent);
                }
            }
           this.DisposeDatabaseConnection();
            return list;
        }
    }
}
using System;
using System.Collections.Generic;
using AIS.Models;
using Oracle.ManagedDataAccess.Client;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Data;


namespace AIS
{
    public class DBConnection
    {
        private SessionHandler sessionHandler;
        private readonly LocalIPAddress iPAddress = new LocalIPAddress();
        private readonly DateTimeHandler dtime = new DateTimeHandler();
        private readonly CAUEncodeDecode encoderDecoder = new CAUEncodeDecode();
        public ISession _session;
        public IHttpContextAccessor _httpCon;
        public DBConnection(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor.HttpContext.Session;
            _httpCon = httpContextAccessor;
        }
        public DBConnection()
        {

        }
        private OracleConnection DatabaseConnection()
        {
            try
            {
                // create connection

                OracleConnection con = new OracleConnection();
                // create connection string using builder

                OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
                ocsb.Password = "ztblaisdev";
                ocsb.UserID = "ztblaisdev";
                ocsb.DataSource = "10.1.100.222:1521/devdb18c.ztbl.com.pk";
                // connect
                con.ConnectionString = ocsb.ConnectionString;
                con.Open();

                return con;

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
                    }
                }
            }
            con.Close();
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
            con.Close();
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
                con.Close();
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
            con.Close();
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
                con.Close();
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
            con.Close();
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
            con.Close();
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
            con.Close();
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
            con.Close();
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
            con.Close();
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
            con.Close();
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
            con.Close();
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
            con.Close();
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
            con.Close();
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
            con.Close();
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
            con.Close();
            return teamList;
        }
        public AuditTeamModel AddAuditTeam(AuditTeamModel aTeam)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
           // var con = this.DatabaseConnection();
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
            con.Close();
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
                  con.Close();
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
            con.Close();
            return maxTeamId;
        }
             
        public bool AddAuditCriteria(AddAuditCriteriaModel acm)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            bool isAlreadyAdded = false;
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT a.ID FROM T_AUDIT_CRITERIA a inner join t_au_period p on p.auditperiodid = a.auditperiodid WHERE p.status_id = 2 and  a.ENTITY_TYPEID =" + acm.ENTITY_TYPEID + " and a.SIZE_ID=" + acm.SIZE_ID + " and a.RISK_ID=" + acm.RISK_ID + " and a.AUDITPERIODID = " + acm.AUDITPERIODID;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["ID"].ToString() != "" && rdr["ID"].ToString() != null)
                        isAlreadyAdded = true;
                }
                if (!isAlreadyAdded)
                {
                    cmd.CommandText = "pkg_ais.P_ADDAUDITCRITERIA";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("ENTITY_TYPEID", OracleDbType.Int32).Value = acm.ENTITY_TYPEID;
                    cmd.Parameters.Add("SIZE_ID", OracleDbType.Int32).Value = acm.SIZE_ID;
                    cmd.Parameters.Add("RISK_ID", OracleDbType.Int32).Value = acm.RISK_ID;
                    cmd.Parameters.Add("FREQUENCY_ID", OracleDbType.Int32).Value = acm.FREQUENCY_ID;
                    cmd.Parameters.Add("NO_OF_DAYS", OracleDbType.Int32).Value = acm.NO_OF_DAYS;
                    cmd.Parameters.Add("visit", OracleDbType.Varchar2).Value = acm.VISIT;
                    cmd.Parameters.Add("APPROVAL_STATUS", OracleDbType.Int32).Value = acm.APPROVAL_STATUS;
                    cmd.Parameters.Add("AUDITPERIODID", OracleDbType.Int32).Value = acm.AUDITPERIODID;
                    cmd.Parameters.Add("UserEntityID", OracleDbType.Int32).Value = loggedInUser.UserEntityID;
                    cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = "AUDIT CRITERIA CREATED";
                    cmd.Parameters.Add("CREATED_BY", OracleDbType.Int32).Value =loggedInUser.PPNumber;
                    cmd.ExecuteReader();
                }
            }
            con.Close();
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
                cmd.Parameters.Add("APPROVAL_STATUS", OracleDbType.Int32).Value = acm.APPROVAL_STATUS;
                cmd.Parameters.Add("AUDITPERIODID", OracleDbType.Int32).Value = acm.AUDITPERIODID;
                cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = COMMENTS;
                cmd.Parameters.Add("CREATED_BY", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();               
            }
            con.Close();
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
            con.Close();
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
            con.Close();
            EmailConfiguration email = new EmailConfiguration();
            email.ConfigEmail();
            return true;
        }
        public string GetAuditCriteriaLogLastStatus(int Id)
        {
            var con = this.DatabaseConnection();
            string remarks = "";
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select remarks from T_AUDIT_CRITERIA_LOG l where l.c_id=" + Id + " order by l.id desc FETCH NEXT 1 ROWS ONLY";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    remarks = rdr["remarks"].ToString();
                }
            }
            con.Close();
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
                    acr.COMMENTS = this.GetAuditCriteriaLogLastStatus(acr.ID);
                    if (rdr["no_of_entity"].ToString() != null && rdr["no_of_entity"].ToString() != "")
                        acr.ENTITIES_COUNT = Convert.ToInt32(rdr["no_of_entity"].ToString());
                    criteriaList.Add(acr);
                }
            }
            con.Close();
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
                    acr.COMMENTS = this.GetAuditCriteriaLogLastStatus(acr.ID);
                    criteriaList.Add(acr);
                }
            }
            con.Close();
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
                    acr.COMMENTS = this.GetAuditCriteriaLogLastStatus(acr.ID);
                    acr.ENTITIES_COUNT = this.GetExpectedCountOfAuditEntitiesOnCriteria(acr.RISK_ID, acr.SIZE_ID, acr.ENTITY_TYPEID, acr.AUDITPERIODID, acr.FREQUENCY_ID);
                    criteriaList.Add(acr);
                }
            }
            con.Close();
            return criteriaList;
        }
        public List<AuditCriteriaModel> GetPostChangesAuditCriterias()
        {
            var con = this.DatabaseConnection();
            List<AuditCriteriaModel> criteriaList = new List<AuditCriteriaModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {

                cmd.CommandText = "pkg_ais.P_GetPostChangesAuditCriterias";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    AuditCriteriaModel acr = new AuditCriteriaModel();
                    acr.ID = Convert.ToInt32(rdr["ID"]);
                    acr.ENTITY_TYPEID = Convert.ToInt32(rdr["ENTITY_TYPEID"]);
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
                    acr.COMMENTS = this.GetAuditCriteriaLogLastStatus(acr.ID);
                    criteriaList.Add(acr);
                }
            }
            con.Close();
            return criteriaList;
        }
        public List<AuditeeEntitiesModel> GetAuditeeEntitiesForOutstandingParas(int ENTITY_CODE = 0)
        {
            List<AuditeeEntitiesModel> entitiesList = new List<AuditeeEntitiesModel>();
            var con = this.DatabaseConnection();
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

            string whereClause = "";
            if (ENTITY_CODE != 0)
            {
                whereClause += " and e.code= " + ENTITY_CODE;
            }


            whereClause += " and enteredby=" + loggedInUser.PPNumber;
            using (OracleCommand cmd = con.CreateCommand())
            {


                cmd.CommandText = "select distinct e.name, e.code, e.entity_id from t_au_old_paras_iams f inner join t_auditee_entities e on e.code=f.branchid where 1=1 " + whereClause + " order by e.name ";
                OracleDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    AuditeeEntitiesModel entity = new AuditeeEntitiesModel();
                    entity.ENTITY_ID = Convert.ToInt32(rdr["entity_id"]);
                    entity.CODE = Convert.ToInt32(rdr["entity_code"]);
                    entity.NAME = rdr["entity_name"].ToString();
                    entitiesList.Add(entity);
                }
            }
            con.Close();
            return entitiesList;

        }
        public bool GeneratePlanForAuditCriteria(int CRITERIA_ID)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "begin Tentative_Audit_Plan(" + CRITERIA_ID + "); end;";
                cmd.ExecuteReader();

            }
            con.Close();
            return true;
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
            con.Close();
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
            con.Close();
            return userList;

        }
        public List<AuditEntitiesModel> GetAuditEntities()
        {

            List<AuditEntitiesModel> entitiesList = new List<AuditEntitiesModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetAuditEntities";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
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
            con.Close();
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
            con.Close();
            return entitiesList;

        }
        public AuditEntitiesModel AddAuditEntity(AuditEntitiesModel am)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO t_auditee_ent_types et ( et.AUTID, et.ENTITYCODE, et.ENTITYTYPEDESC, et.AUDITABLE ) VALUES ( (select COALESCE(max(p.AUTID)+1,1) from t_auditee_ent_types p) , LPAD((select COALESCE(max(p.AUTID)+1,1) from t_auditee_ent_types p),3,0), '" + am.ENTITYTYPEDESC + "', '" + am.AUDITABLE + "') ";
                cmd.ExecuteReader();

            }
            con.Close();
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
            con.Close();
            return subEntitiesList;

        }
        public UpdateUserModel UpdateUser(UpdateUserModel user)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.UPDATE_USERS";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("PPNUMBER", OracleDbType.Int32).Value = user.PPNO;
                cmd.Parameters.Add("PASSWORD", OracleDbType.Varchar2).Value = user.PASSWORD!="" && user.PASSWORD!=null?getMd5Hash(user.PASSWORD):"";
                cmd.Parameters.Add("ISACTIVE", OracleDbType.Varchar2).Value = user.ISACTIVE;
                cmd.Parameters.Add("ROLEID", OracleDbType.Int32).Value = user.ROLE_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
              cmd.ExecuteReader();
            }
            con.Close();
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
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.Parameters.Add("enc_pass", OracleDbType.Varchar2).Value = enc_pass;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["ID"].ToString() != null && rdr["ID"].ToString() != "")
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
                    cmd.Parameters.Add("enc_pass", OracleDbType.Varchar2).Value = enc_pass;
                    cmd.ExecuteReader();
                    res = true;
                }
            }
            con.Close();
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
            con.Close();
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
            con.Close();
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
            con.Close();
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
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;OracleDataReader rdr = cmd.ExecuteReader();
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
            con.Close();
            return ICList;
        }
        public List<BranchModel> GetBranches(int zone_code = 0, bool sessionCheck = true)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            List<BranchModel> branchList = new List<BranchModel>();
            var loggedInUser = sessionHandler.GetSessionUser();
            string query = "";
            if (loggedInUser.UserGroupID != 1)
            {
                if (sessionCheck)
                {
                    if (loggedInUser.UserPostingZone != 0)
                        query = query + " and z.ZONECODE=" + loggedInUser.UserPostingZone;
                    if (loggedInUser.UserPostingBranch != 0)
                        query = query + " and b.BRANCHID=" + loggedInUser.UserPostingBranch;
                }
            }

            using (OracleCommand cmd = con.CreateCommand())
            {
                if (zone_code == 0)
                    cmd.CommandText = "Select b.*, z.ZONENAME   FROM V_SERVICE_BRANCH b inner join V_SERVICE_ZONES z on b.zoneid=z.zoneid WHERE 1=1 " + query + " order by b.BRANCHNAME asc";
                else
                    cmd.CommandText = "Select b.*,  z.ZONENAME   FROM V_SERVICE_BRANCH b inner join V_SERVICE_ZONES z on b.zoneid=z.zoneid WHERE z.ZONECODE=" + zone_code + query + " order by b.BRANCHNAME asc";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    BranchModel br = new BranchModel();
                    br.BRANCHID = Convert.ToInt32(rdr["BRANCHID"]);
                    br.ZONEID = Convert.ToInt32(rdr["ZONEID"]);
                    br.BRANCHNAME = rdr["BRANCHNAME"].ToString();
                    br.ZONE_NAME = rdr["ZONENAME"].ToString();
                    br.BRANCHCODE = rdr["BRANCHCODE"].ToString();
                    br.BRANCH_SIZE_ID = 1;//Convert.ToInt32(rdr["BRANCH_SIZE_ID"]);
                    br.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    br.BRANCH_SIZE = "";//rdr["BRANCH_SIZE"].ToString();
                    if (rdr["ISACTIVE"].ToString() == "Y")
                        br.ISACTIVE = "Active";
                    else if (rdr["ISACTIVE"].ToString() == "N")
                        br.ISACTIVE = "InActive";
                    else
                        br.ISACTIVE = rdr["ISACTIVE"].ToString();

                    branchList.Add(br);
                }
            }
            con.Close();
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
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            List<ZoneModel> zoneList = new List<ZoneModel>();
            var loggedInUser = sessionHandler.GetSessionUser();
            string query = "";
            if (loggedInUser.UserGroupID != 1)
            {
                if (sessionCheck)
                {
                    if (loggedInUser.UserPostingZone != 0)
                        query = query + " and z.ZONECODE=" + loggedInUser.UserPostingZone;
                }
            }
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetZones";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                //cmd.CommandText = "Select z.* FROM V_SERVICE_ZONES z WHERE 1=1 " + query + " order by z.ZONENAME asc";
                //OracleDataReader rdr = cmd.ExecuteReader();
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
            con.Close();
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

                //cmd.CommandText = "Select bs.* FROM t_auditee_entities_size_disc bs order by bs.ENTITY_SIZE asc";
                //OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    BranchSizeModel bs = new BranchSizeModel();
                    bs.BR_SIZE_ID = Convert.ToInt32(rdr["ENTITY_SIZE"]);
                    bs.DESCRIPTION = rdr["DESCRIPTION"].ToString();

                    brSizeList.Add(bs);
                }
            }
            con.Close();
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

                //cmd.CommandText = "Select v.* FROM t_r_sub_group v WHERE v.gr_id in (1,3) order by v.S_GR_ID asc";
                //OracleDataReader rdr = cmd.ExecuteReader();
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
            con.Close();
            return controlViolationList;
        }
        public ControlViolationsModel AddControlViolation(ControlViolationsModel cv)
        {
            /*var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if(cv.ID==0)
                cmd.CommandText = "INSERT INTO t_r_sub_group cv (cv.ID,cv.V_NAME, cv.MAX_NUMBER) VALUES ( (SELECT COALESCE(max(PP.ID)+1,1) FROM t_control_violation PP), '" + cv.V_NAME + "','" + cv.MAX_NUMBER+"','"+cv.STATUS+"')";
                else
                    cmd.CommandText = "UPDATE t_control_violation cv SET cv.V_NAME = '"+cv.V_NAME+"', cv.MAX_NUMBER='"+cv.MAX_NUMBER+"', cv.STATUS= '"+cv.STATUS+"' WHERE cv.ID="+cv.ID;
                cmd.ExecuteReader();
            }
            con.Close();*/
            return cv;
        }
        public List<DivisionModel> GetDivisions(bool sessionCheck = true)
        {
            var con = this.DatabaseConnection();
            List<DivisionModel> divList = new List<DivisionModel>();
            /*if (sessionCheck)
            {
                var loggedInUser = sessionHandler.GetSessionUser();
                if (loggedInUser.UserPostingDiv != 0)
                    query = query + " and d.DIVISIONID=" + loggedInUser.UserPostingDiv;
            }*/
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select * from t_auditee_entities d WHERE d.type_id=3 order by d.CODE asc";
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
            con.Close();
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
            con.Close();
            return div;
        }
        public DivisionModel UpdateDivision(DivisionModel div)
        {
            /*
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "UPDATE T_DIVISION d SET d.CODE='" + div.CODE + "', d.NAME='" + div.NAME + "', d.DESCRIPTION='" + div.DESCRIPTION + "', d.STATUS='" + div.ISACTIVE + "' WHERE d.ID="+div.DIVISIONID; 
                OracleDataReader rdr = cmd.ExecuteReader();

            }
            con.Close();*/
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
            string query = "";
            if (loggedInUser.UserGroupID != 1)
            {
                if (sessionCheck)
                {
                    query = query + " and e.entity_id=" + loggedInUser.UserEntityID;

                }
            }
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (div_code == 0)
                    cmd.CommandText = "select mp.parent_id as DIVISIONID, mp.entity_id as ID , mp.c_name as NAME, mp.child_code as CODE ,mp.status as ISACTIVE, mp.p_name as DIV_NAME, mp.auditedby as AUDITED_BY_DEPID from t_auditee_entities e, t_auditee_entities_maping mp where e.entity_id = mp.parent_id and e.type_id IN (3) and mp.entity_id is not null" + query;
                else
                    cmd.CommandText = "select mp.parent_id as DIVISIONID, mp.entity_id as ID , mp.c_name as NAME, mp.child_code as CODE ,mp.status as ISACTIVE, mp.p_name as DIV_NAME, mp.auditedby as AUDITED_BY_DEPID from t_auditee_entities e, t_auditee_entities_maping mp where mp.entity_id is not null and e.type_id IN (3) and e.entity_id = mp.parent_id and e.entity_id = " + div_code + query;
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
                        cmd.CommandText = "Select dep.NAME FROM t_auditee_entities dep WHERE dep.ENTITY_ID = " + dept.AUDITED_BY_DEPID;
                        OracleDataReader rdr2 = cmd.ExecuteReader();
                        while (rdr2.Read())
                        {
                            dept.AUDITED_BY_NAME = rdr2["NAME"].ToString();
                        }
                    }
                    deptList.Add(dept);
                }
            }
            con.Close();
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

                /* if (div_code == 0)
                 {
                     if (dept_code == 0)
                         cmd.CommandText = "Select s.*, d.NAME as DIV_NAME, dp.NAME as DEPT_NAME FROM T_SUBENTITY s inner join T_DIVISION d on s.DIV_ID = d.DIVISIONID inner join T_DEPARTMENT dp on s.DEP_ID=dp.ID WHERE s.STATUS='Y' order by s.NAME asc";
                     else
                         cmd.CommandText = "Select s.*, d.NAME as DIV_NAME, dp.NAME as DEPT_NAME FROM T_SUBENTITY s inner join T_DIVISION d on s.DIV_ID = d.DIVISIONID inner join T_DEPARTMENT dp on s.DEP_ID=dp.ID WHERE s.STATUS='Y' and dp.ID=" + dept_code + " order by s.NAME asc";

                 }
                 else {
                     if (dept_code == 0)
                         cmd.CommandText = "Select s.*, d.NAME as DIV_NAME, dp.NAME as DEPT_NAME FROM T_SUBENTITY s inner join T_DIVISION d on s.DIV_ID = d.DIVISIONID inner join T_DEPARTMENT dp on s.DEP_ID=dp.ID WHERE d.DIVISIONID=" + div_code + " and s.STATUS='Y' order by s.NAME asc";
                     else
                         cmd.CommandText = "Select s.*, d.NAME as DIV_NAME, dp.NAME as DEPT_NAME FROM T_SUBENTITY s inner join T_DIVISION d on s.DIV_ID = d.DIVISIONID inner join T_DEPARTMENT dp on s.DEP_ID=dp.ID WHERE d.DIVISIONID=" + div_code + " and s.STATUS='Y' and dp.ID=" + dept_code + " order by s.NAME asc";

                 }

                 OracleDataReader rdr = cmd.ExecuteReader();*/
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
            con.Close();
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

                //cmd.CommandText = "INSERT INTO T_SUBENTITY d (d.ID,d.NAME, d.DIV_ID, d.DEP_ID, d.STATUS) VALUES ( (SELECT COALESCE(max(PP.ID)+1,1) FROM T_SUBENTITY PP),'" + subentity.NAME + "','" + subentity.DIV_ID + "','" + subentity.DEP_ID + "','" + subentity.STATUS + "')";
                //OracleDataReader rdr = cmd.ExecuteReader();

            }
            con.Close();
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
                // cmd.CommandText = "UPDATE T_SUBENTITY d SET d.NAME = '" + subentity.NAME + "' , d.DIV_ID='" + subentity.DIV_ID + "', d.DEP_ID='" + subentity.DEP_ID + "', d.STATUS='" + subentity.STATUS + "'  WHERE d.ID='" + subentity.ID + "'";
                //OracleDataReader rdr = cmd.ExecuteReader();

            }
            con.Close();
            return subentity;
        }
        public DepartmentModel AddDepartment(DepartmentModel dept)
        {
            /*
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO T_DEPARTMENT d (d.ID,d.CODE, d.NAME, d.DIV_ID, d.STATUS) VALUES ( '" + dept.CODE + "','" + dept.CODE + "','" + dept.NAME + "','" + dept.DIV_ID + "','" + dept.STATUS + "')";
                OracleDataReader rdr = cmd.ExecuteReader();

            }
            con.Close();*/
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
            con.Close();
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
                //cmd.CommandText = "Select rg.* FROM T_R_GROUP rg order by rg.GR_ID asc";
                //OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    RiskGroupModel rgm = new RiskGroupModel();
                    rgm.GR_ID = Convert.ToInt32(rdr["GR_ID"]);
                    rgm.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    riskgroupList.Add(rgm);
                }
            }

            con.Close();
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

                //if (group_id == 0)
                //    cmd.CommandText = "Select rsg.*, rg.DESCRIPTION as GROUP_DESC  FROM T_R_SUB_GROUP rsg inner join T_R_GROUP rg on rsg.GR_ID = rg.GR_ID order by rsg.S_GR_ID asc";
                //else
                //    cmd.CommandText = "Select rsg.*, rg.DESCRIPTION as GROUP_DESC  FROM T_R_SUB_GROUP rsg inner join T_R_GROUP rg on rsg.GR_ID = rg.GR_ID WHERE rsg.GR_ID =" + group_id + " order by rsg.S_GR_ID asc";


                //OracleDataReader rdr = cmd.ExecuteReader();
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
            con.Close();
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

                //if (Sub_group_id == 0)
                //    cmd.CommandText = "Select ra.*, rsg.DESCRIPTION as SUB_GROUP_DESC  FROM T_R_ACTIVITY ra inner join T_R_SUB_GROUP rsg on ra.S_GR_ID = rsg.S_GR_ID order by ra.ACTIVITY_ID asc";
                //else
                //    cmd.CommandText = "Select ra.*, rsg.DESCRIPTION as SUB_GROUP_DESC  FROM T_R_ACTIVITY ra inner join T_R_SUB_GROUP rsg on ra.S_GR_ID = rsg.S_GR_ID WHERE ra.S_GR_ID = " + Sub_group_id + " order by ra.ACTIVITY_ID asc";

                //OracleDataReader rdr = cmd.ExecuteReader();
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
            con.Close();
            return riskActivityList;
        }
        public List<AuditObservationTemplateModel> GetAuditObservationTemplates(int activity_id)
        {
            var con = this.DatabaseConnection();
            List<AuditObservationTemplateModel> templateList = new List<AuditObservationTemplateModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (activity_id == 0)
                    cmd.CommandText = "Select ob.* FROM T_A_OBS_TEMPLATE ob inner join T_R_ACTIVITY ra on ob.ACTIVITY_ID = ra.ACTIVITY_ID order by ob.TEMP_ID asc";
                else
                    cmd.CommandText = "Select ob.* FROM T_AP_OBS_TEMPLATE ob inner join T_R_ACTIVITY ra on ob.ACTIVITY_ID = ra.ACTIVITY_ID WHERE ra.ACTIVITY_ID = " + activity_id + " order by ob.TEMP_ID asc";

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditObservationTemplateModel temp = new AuditObservationTemplateModel();
                    temp.TEMP_ID = Convert.ToInt32(rdr["TEMP_ID"]);
                    temp.ACTIVITY_ID = Convert.ToInt32(rdr["ACTIVITY_ID"]);
                    temp.OBS_TEMPLATE = rdr["OBS_TEMPLATE"].ToString();
                    templateList.Add(temp);
                }
            }
            con.Close();
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

                //if (dept_code == 0)
                //    cmd.CommandText = "select e.* from t_audit_emp e inner join t_user u on e.ppno=u.ppno order by e.RANKCODE asc";
                //else
                //    cmd.CommandText = "select e.* from t_audit_emp e inner join t_user u on e.ppno=u.ppno WHERE u.entity_id=" + dept_code + " order by e.RANKCODE asc";

                //OracleDataReader rdr = cmd.ExecuteReader();
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
            con.Close();
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
                    tplan.ENTITY_NAME = rdr["AUDITEE_NAME"].ToString();
                    tplan.FREQUENCY_DESCRIPTION = rdr["FREQUENCY_DISCRIPTION"].ToString();
                    tplan.PERIOD_NAME = rdr["PERIOD_NAME"].ToString();
                    tplansList.Add(tplan);
                }
            }
            con.Close();
            return tplansList;


            /* sessionHandler = new SessionHandler();
             sessionHandler._httpCon = this._httpCon;
             sessionHandler._session = this._session;
             var con = this.DatabaseConnection();
             List<TentativePlanModel> tplansList = new List<TentativePlanModel>();
             var loggedInUser = sessionHandler.GetSessionUser();
             string query = "";
             if (loggedInUser.UserGroupID != 1)
             {
                 if (sessionCheck)
                 {
                     query = query + " where p.AUDITEDBY=" + loggedInUser.UserEntityID;
                 }
             }
             using (OracleCommand cmd = con.CreateCommand())
             {
                 cmd.CommandText = " select * FROM v_GET_AUDIT_PLAN p " + query;
                     //"  p.*,ap.DESCRIPTION as PERIOD_NAME from t_au_plan p inner join t_au_period ap on p.AUDITPERIODID=ap.AUDITPERIODID WHERE not EXISTS (select * from t_au_plan_eng e where e.period_id= p.auditperiodid and e.entity_code= p.entity_code and e.entity_id= p.entity_id) " + query + " order by decode(p.AUDITEE_RISK, 'High', 1, 'Medium', 2, 'Low', 3 ), p.DIVISION_ZONE_NAME asc";

                 OracleDataReader rdr = cmd.ExecuteReader();
                 while (rdr.Read())
                 {
                     TentativePlanModel tplan = new TentativePlanModel();
                     tplan.CRITERIA_ID = Convert.ToInt32(rdr["CRITERIA_ID"]);
                     tplan.AUDIT_PERIOD_ID = Convert.ToInt32(rdr["AUDITPERIODID"]);
                    // if(rdr["AUDITEDBY"].ToString()!=null && rdr["AUDITEDBY"].ToString()!="")
                     tplan.AUDITEDBY = Convert.ToInt32(rdr["AUDITEDBY"]);
                     tplan.BR_SIZE = rdr["AUDITEE_SIZE"].ToString();
                     tplan.RISK = rdr["AUDITEE_RISK"].ToString();
                     tplan.NO_OF_DAYS = Convert.ToInt32(rdr["NO_OF_DAYS"]);
                     tplan.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                     tplan.CODE = rdr["ENTITY_CODE"].ToString();
                     tplan.ENTITY_NAME = rdr["AUDITEE_NAME"].ToString();
                   // tplan.ZONE_NAME = rdr["DIVISION_ZONE_NAME"].ToString();
                     tplan.FREQUENCY_DESCRIPTION = rdr["FREQUENCY_DISCRIPTION"].ToString();
                   //  tplan.BR_NAME = rdr["AUDITEE_SIZE"].ToString();
                     tplan.PERIOD_NAME = rdr["PERIOD_NAME"].ToString();
                     tplansList.Add(tplan);
                 }
             }
             con.Close();
             return tplansList;*/
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

                //cmd.CommandText = "select EXTRACT(year FROM d.audit_enddate) as year, EXTRACT(month FROM d.audit_enddate) as month, EXTRACT(day FROM d.audit_enddate) as day  FROM T_AUDITEE_ENTITIES_AUDIT_DATES d WHERE d.ENTITY_CODE=" + entityCode + " and d.AUDIT_PERIOD_ID= " + auditPeriodId;

                //OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result = rdr["YEAR"].ToString() + "-";
                    result += rdr["MONTH"].ToString() + "-";
                    result += rdr["DAY"].ToString();
                }
            }
            con.Close();
            return result;
        }
        public AuditPlanModel AddAuditPlan(AuditPlanModel plan)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "insert into T_AU_PLAN p (p.AUDITPERIOD_ID,p.AUDIT_CONDUCTBY_DEPT,p.NO_OF_DAYS_AUDIT,p.AUDITZONE_ID, p.BRANCH_ID, p.DIVISION_ID,p.DEPARTMENT_ID,p.RISK_LEVEL_ID,p.BRANCH_SIZE_ID,p.PLAN_STATUS_ID,p.SUB_ENTITY_ID) VALUES ( " + plan.AUDITPERIOD_ID + "," + plan.AUDIT_CONDUCTBY_DEPT + ", " + plan.NO_OF_DAYS_AUDIT + ", " + plan.AUDITZONE_ID + "," + plan.BRANCH_ID + "," + plan.DIVISION_ID + "," + plan.DEPARTMENT_ID + "," + plan.RISK_LEVEL_ID + "," + plan.BRANCH_SIZE_ID + "," + plan.PLAN_STATUS_ID + ", " + plan.SUB_ENTITY_ID + ")";
                OracleDataReader rdr = cmd.ExecuteReader();
            }
            con.Close();
            return plan;
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


                //cmd.CommandText = "select e.eng_id, ee.entity_id, ee.name, e.team_name, e.audit_startdate, e.audit_enddate from t_au_plan_eng e inner join t_auditee_entities ee on e.entity_id=ee.entity_id where e.STATUS IN (1,3,7) and e.auditby_id= " + loggedInUser.UserEntityID;
                //OracleDataReader ardr = cmd.ExecuteReader();
                while (ardr.Read())
                {
                    AuditEngagementPlanModel eng = new AuditEngagementPlanModel();
                    eng.ENG_ID = Convert.ToInt32(ardr["eng_id"].ToString());
                    eng.TEAM_NAME = ardr["team_name"].ToString();
                    eng.ENTITY_NAME = ardr["name"].ToString();
                    eng.AUDIT_STARTDATE = Convert.ToDateTime(ardr["audit_startdate"].ToString());
                    eng.AUDIT_ENDDATE = Convert.ToDateTime(ardr["audit_enddate"].ToString());
                    eng.ENTITY_ID = Convert.ToInt32(ardr["entity_id"].ToString());
                    list.Add(eng);
                }
            }
            con.Close();
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

                //cmd.CommandText = "select e.eng_id, ee.entity_id, ee.name, e.team_name, tt.team_id,  e.audit_startdate, e.audit_enddate from t_au_plan_eng e inner join t_auditee_entities ee on e.entity_id=ee.entity_id inner join t_au_audit_teams tt on tt.eng_id=e.eng_id  where e.STATUS IN (2,6) and e.auditby_id= " + loggedInUser.UserEntityID;
                //OracleDataReader ardr = cmd.ExecuteReader();
                while (ardr.Read())
                {
                    AuditEngagementPlanModel eng = new AuditEngagementPlanModel();
                    eng.PLAN_ID = Convert.ToInt32(ardr["plan_id"].ToString());
                    eng.ENG_ID = Convert.ToInt32(ardr["eng_id"].ToString());
                    eng.TEAM_NAME = ardr["team_name"].ToString();
                    eng.TEAM_ID = Convert.ToInt32(ardr["team_id"].ToString());
                    eng.ENTITY_NAME = ardr["name"].ToString();
                    eng.AUDIT_STARTDATE = Convert.ToDateTime(ardr["audit_startdate"].ToString());
                    eng.AUDIT_ENDDATE = Convert.ToDateTime(ardr["audit_enddate"].ToString());
                    eng.ENTITY_ID = Convert.ToInt32(ardr["entity_id"].ToString());
                    list.Add(eng);
                }
            }
            con.Close();
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
                cmd.Parameters.Add("PERIOD_ID", OracleDbType.Int32).Value = ePlan.PERIOD_ID;
                cmd.Parameters.Add("ENTITYID", OracleDbType.Int32).Value = ePlan.ENTITY_ID;
                cmd.Parameters.Add("AUDIT_STARTDATE", OracleDbType.Date).Value = ePlan.AUDIT_STARTDATE;
                cmd.Parameters.Add("CREATEDBY", OracleDbType.Int32).Value = ePlan.CREATEDBY;
                cmd.Parameters.Add("AUDIT_ENDDATE", OracleDbType.Date).Value = ePlan.AUDIT_ENDDATE;
                cmd.Parameters.Add("STATUS", OracleDbType.Varchar2).Value = ePlan.STATUS;
                cmd.Parameters.Add("TEAM_ID", OracleDbType.Int32).Value = ePlan.TEAM_ID;
                cmd.Parameters.Add("TEAM_NAME", OracleDbType.Varchar2).Value = ePlan.TEAM_NAME;
                cmd.Parameters.Add("PLANID", OracleDbType.Int32).Value = ePlan.PLAN_ID;
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
            con.Close();
            return ePlan;
            
        }
        public bool RefferedBackAuditEngagementPlan(int ENG_ID, string REMARKS)
        {
            //P_RefferedBackAuditEngagementPlan
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
            con.Close();
            return true;
        }
        public bool RerecommendAuditEngagementPlan(int ENG_ID, int PLAN_ID, int ENTITY_ID, DateTime START_DATE, DateTime END_DATE, int TEAM_ID, string COMMENTS)
        {
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
                cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = COMMENTS;
                cmd.ExecuteReader();
            }
            con.Close();
            return true;
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
                cmd.Parameters.Add("REMARKS", OracleDbType.Varchar2).Value = "Engagement Plan Approved";
                cmd.Parameters.Add("PPNumber", OracleDbType.Int32).Value = loggedInUser.PPNumber;
                cmd.ExecuteReader();

                //cmd.CommandText = "UPDATE T_AU_PLAN_ENG a SET a.STATUS=4 WHERE a.ENG_ID = " + ENG_ID;
                //cmd.ExecuteReader();
                //cmd.CommandText = "insert into t_au_plan_eng_log l (l.ID,l.E_ID, l.STATUS_ID,l.CREATEDBY_ID, l.CREATED_ON, l.REMARKS) VALUES ( (SELECT COALESCE(max(ll.ID)+1,1) FROM t_au_plan_eng_log ll)," + ENG_ID + " ,2," + loggedInUser.PPNumber + ", to_date('" + dtime.DateTimeInDDMMYY(System.DateTime.Now) + "','dd/mm/yyyy HH:MI:SS AM'), 'Engagement Plan Approved')";
                //cmd.ExecuteReader();
            }
            con.Close();
            // EmailConfiguration email = new EmailConfiguration();
            //  email.ConfigEmail();
            return true;
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
                //if (period_id == 0)
                //    cmd.CommandText = "select p.*,d.Name as DIVISION_NAME,dp.Name as DEPARTMENT_NAME, b.branchname as BRANCH_NAME, az.ZONENAME as AUDITZONE_NAME from T_AU_PLAN p left join T_DIVISION d on p.division_id = d.DIVISIONID left join T_DEPARTMENT dp on p.department_id = dp.ID left join V_SERVICE_BRANCH b on p.Branch_Id = b.BRANCHID left join V_SERVICE_ZONES az on p.auditzone_id = az.ZONEID  order by p.PLAN_ID asc";
                //else
                //    cmd.CommandText = "select p.*,d.Name as DIVISION_NAME,dp.Name as DEPARTMENT_NAME, b.branchname as BRANCH_NAME, az.ZONENAME as AUDITZONE_NAME from T_AU_PLAN p left join T_DIVISION d on p.division_id = d.DIVISIONID left join T_DEPARTMENT dp on p.department_id = dp.ID left join V_SERVICE_BRANCH b on p.Branch_Id = b.BRANCHID left join V_SERVICE_ZONES az on p.auditzone_id = az.ZONEID where p.auditperiod_id=" + period_id + " order by p.PLAN_ID asc";

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
            con.Close();
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
                //cmd.ExecuteReader();

                //cmd.CommandText = "select * from t_audit_checklist t order by t.T_ID";

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
            con.Close();
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

                //if (procId == 0)
                //    cmd.CommandText = "select * from t_audit_checklist_sub pd order by pd.s_id asc";
                //else
                //    cmd.CommandText = "select * from t_audit_checklist_sub pd where pd.t_id = " + procId + " order by pd.s_id asc";
                //OracleDataReader rdr = cmd.ExecuteReader();
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
            con.Close();
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

                //if (procDetailId == 0)
                //{
                //    if (transactionId == 0)
                //        cmd.CommandText = "select s.description as DIV_NAME, d.name as CONTROL_OWNER, pt.*,pd.heading as TITLE , p.heading as P_NAME, vc.DESCRIPTION as V_NAME from t_audit_checklist_details pt inner join t_audit_checklist_sub pd on pt.S_ID=pd.S_ID inner join t_audit_checklist p on pd.T_ID = p.T_ID inner join t_r_sub_group vc on vc.S_GR_ID=pt.V_ID inner join t_hr_designations s on pt.role_resp_id = s.designationcode inner join T_DIVISION d on pt.process_owner_id=d.DIVISIONID order by pt.id asc";
                //    else
                //        cmd.CommandText = "select s.description as DIV_NAME, d.name as CONTROL_OWNER, pt.*,pd.heading as TITLE , p.heading as P_NAME, vc.DESCRIPTION as V_NAME from t_audit_checklist_details pt inner join t_audit_checklist_sub pd on pt.S_ID=pd.S_ID inner join t_audit_checklist p on pd.T_ID = p.T_ID inner join t_r_sub_group vc on vc.S_GR_ID=pt.V_ID inner join t_hr_designations s on pt.role_resp_id = s.designationcode inner join T_DIVISION d on pt.process_owner_id=d.DIVISIONID WHERE pt.ID=" + transactionId + " order by pt.id asc";
                //}
                //else
                //{
                //    if (transactionId == 0)
                //        cmd.CommandText = "select s.description as DIV_NAME, d.name as CONTROL_OWNER, pt.*,pd.heading as TITLE , p.heading as P_NAME, vc.DESCRIPTION as V_NAME from t_audit_checklist_details pt inner join t_audit_checklist_sub pd on pt.S_ID=pd.S_ID inner join t_audit_checklist p on pd.T_ID = p.T_ID inner join t_r_sub_group vc on vc.S_GR_ID=pt.V_ID inner join t_hr_designations s on pt.role_resp_id = s.designationcode inner join T_DIVISION d on pt.process_owner_id=d.DIVISIONID  where pt.s_id = " + procDetailId + " order by pt.Id asc";
                //    else
                //        cmd.CommandText = "select s.description as DIV_NAME, d.name as CONTROL_OWNER, pt.*,pd.heading as TITLE , p.heading as P_NAME, vc.DESCRIPTION as V_NAME from t_audit_checklist_details pt inner join t_audit_checklist_sub pd on pt.S_ID=pd.S_ID inner join t_audit_checklist p on pd.T_ID = p.T_ID inner join t_r_sub_group vc on vc.S_GR_ID=pt.V_ID inner join t_hr_designations s on pt.role_resp_id = s.designationcode inner join T_DIVISION d on pt.process_owner_id=d.DIVISIONID where pt.ID=" + transactionId + " pt.s_id = " + procDetailId + " order by pt.Id asc";

                //}
                //OracleDataReader rdr = cmd.ExecuteReader();
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
            con.Close();
            return riskTransList;
        }
        public List<RiskProcessTransactions> GetRiskProcessTransactionsWithStatus(int[] statusId)
        {
            var con = this.DatabaseConnection();
            List<RiskProcessTransactions> riskTransList = new List<RiskProcessTransactions>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                //cmd.CommandText = "pkg_ais.p_GetRiskProcessTransactionsWithStatus";
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.Clear();
                //cmd.Parameters.Add("statusId", OracleDbType.Int32).Value = statusId;
                //cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                //OracleDataReader rdr = cmd.ExecuteReader();

                if (statusId.Length == 0)
                    cmd.CommandText = "select s.description as DIV_NAME, d.name as CONTROL_OWNER, pt.*, pd.HEADING as TITLE, p.HEADING as P_NAME, vc.DESCRIPTION as V_NAME, s.status from t_audit_checklist_details pt inner join t_audit_checklist_sub pd on pt.S_ID = pd.S_ID inner join t_audit_checklist p on pd.T_ID = p.T_ID inner join t_r_sub_group vc on vc.S_GR_ID=pt.V_ID inner join t_hr_designations s on pt.role_resp_id = s.designationcode inner join T_DIVISION d on pt.process_owner_id=d.DIVISIONID inner join t_audit_checklist_details_status_mapping sm on pt.id = sm.T_ID inner join t_audit_checklist_details_status s on s.ID = sm.STATUS_ID order by pt.id asc";
                else
                    cmd.CommandText = "select s.description as DIV_NAME, d.name as CONTROL_OWNER, pt.*, pd.HEADING as TITLE, p.HEADING as P_NAME, vc.DESCRIPTION as V_NAME, s.status from t_audit_checklist_details pt inner join t_audit_checklist_sub pd on pt.S_ID = pd.S_ID inner join t_audit_checklist p on pd.T_ID = p.T_ID inner join t_r_sub_group vc on vc.S_GR_ID=pt.V_ID inner join t_hr_designations s on pt.role_resp_id = s.designationcode inner join T_DIVISION d on pt.process_owner_id=d.DIVISIONID inner join t_audit_checklist_details_status_mapping sm on pt.id = sm.T_ID inner join t_audit_checklist_details_status s on s.ID = sm.STATUS_ID and s.ID IN (" + string.Join(",", statusId) + ") order by pt.id asc";
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
            con.Close();
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

                //cmd.CommandText = "insert into t_audit_checklist p (p.T_ID,p.HEADING,p.ENTITY_TYPE, p.STATUS) VALUES ( (select COALESCE(max(pp.T_ID)+1,1) from t_audit_checklist pp),'" + proc.P_NAME + "','" + proc.RISK_ID + "', 'Y')";
                //OracleDataReader rdr = cmd.ExecuteReader();
            }
            con.Close();
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

                //cmd.CommandText = "insert into t_audit_checklist_sub p (p.S_ID,p.T_ID,p.HEADING, p.ENTITY_TYPE, p.STATUS) VALUES ( (select COALESCE(max(pp.S_ID)+1,1) from t_audit_checklist_sub pp)," + subProc.P_ID + ",'" + subProc.TITLE + "','" + subProc.ENTITY_TYPE + "','Y')";
                //OracleDataReader rdr = cmd.ExecuteReader();
            }
            con.Close();
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


                //cmd.CommandText = "insert into t_audit_checklist_details p (p.ID,p.V_ID, p.S_ID,p.HEADING,p.PROCESS_OWNER_ID,p.ROLE_RESP_ID,p.RISK_ID,p.STATUS) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_audit_checklist_details pp)," + trans.V_ID + "," + trans.PD_ID + ",'" + trans.DESCRIPTION + "','" + trans.CONTROL_OWNER + "','" + trans.ACTION + "','" + trans.RISK_WEIGHTAGE + "','Y')";
                //OracleDataReader rdr = cmd.ExecuteReader();
                //cmd.CommandText = "insert into t_audit_checklist_details_log p (p.ID,p.T_ID,p.STATUS_ID,p.USER_ID,p.COMMENTS) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_audit_checklist_details_log pp), (select max(tp.ID) from t_audit_checklist_details tp) ,'1'," + loggedInUser.PPNumber + ",'New Transaction Added')";
                //cmd.ExecuteReader();
                //cmd.CommandText = "insert into t_audit_checklist_details_status_mapping p (p.ID,p.T_ID,p.STATUS_ID) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_audit_checklist_details_status_mapping pp), (select max(tp.ID) from t_audit_checklist_details tp) ,'1')";
                //cmd.ExecuteReader();

            }
            con.Close();
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

                //cmd.CommandText = " Update t_audit_checklist_details_status_mapping tm SET tm.STATUS_ID = 3 WHERE tm.T_ID=" + T_ID;
                //OracleDataReader rdr = cmd.ExecuteReader();
                //cmd.CommandText = "insert into t_audit_checklist_details_log p (p.ID,p.T_ID,p.STATUS_ID,p.USER_ID,p.COMMENTS) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_audit_checklist_details_log pp), " + T_ID + " ,'3'," + loggedInUser.PPNumber + ",'" + COMMENTS + "')";
                //cmd.ExecuteReader();
            }
            con.Close();
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

                //cmd.CommandText = " Update t_audit_checklist_details_status_mapping tm SET tm.STATUS_ID = 2 WHERE tm.T_ID=" + T_ID;
                //OracleDataReader rdr = cmd.ExecuteReader();
                //cmd.CommandText = "insert into t_audit_checklist_details_log p (p.ID,p.T_ID,p.STATUS_ID,p.USER_ID,p.COMMENTS) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_audit_checklist_details_log pp), " + T_ID + " ,'2'," + loggedInUser.PPNumber + ",'" + COMMENTS + "')";
                //cmd.ExecuteReader();
            }
            con.Close();
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

                //cmd.CommandText = " Update t_audit_checklist_details_status_mapping tm SET tm.STATUS_ID = 5 WHERE tm.T_ID=" + T_ID;
                //OracleDataReader rdr = cmd.ExecuteReader();
                //cmd.CommandText = "insert into t_audit_checklist_details_log p (p.ID,p.T_ID,p.STATUS_ID,p.USER_ID,p.COMMENTS) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_audit_checklist_details_log pp), " + T_ID + " ,'5'," + loggedInUser.PPNumber + ",'" + COMMENTS + "')";
                //cmd.ExecuteReader();
            }
            con.Close();
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

                //cmd.CommandText = " Update t_audit_checklist_details_status_mapping tm SET tm.STATUS_ID = 4 WHERE tm.T_ID=" + T_ID;
                //OracleDataReader rdr = cmd.ExecuteReader();
                //cmd.CommandText = "insert into t_audit_checklist_details_log p (p.ID,p.T_ID,p.STATUS_ID,p.USER_ID,p.COMMENTS) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_audit_checklist_details_log pp), " + T_ID + " ,'4'," + loggedInUser.PPNumber + ",'" + COMMENTS + "')";
                //cmd.ExecuteReader();
            }
            con.Close();
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

                //cmd.CommandText = "select * from T_AUDIT_FREQUENCY F WHERE F.STATUS='Y' order by F.ID";
                //OracleDataReader rdr = cmd.ExecuteReader();
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
            con.Close();
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

                //cmd.CommandText = "select * from T_RISK R order by R.R_ID";
                //OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    RiskModel risk = new RiskModel();
                    risk.R_ID = Convert.ToInt32(rdr["R_ID"]);
                    risk.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    riskList.Add(risk);
                }
            }
            con.Close();
            return riskList;
        }
        public List<RiskModel> GetCOSORisks()
        {
            var con = this.DatabaseConnection();
            List<RiskModel> riskList = new List<RiskModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select * from T_COSO_RISK R order by R.R_ID";
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
            con.Close();
            return riskList;
        }     
        public List<AuditVoilationcatModel> GetAuditVoilationcats()
        {
            var con = this.DatabaseConnection();
            List<AuditVoilationcatModel> voilationList = new List<AuditVoilationcatModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {

                cmd.CommandText = "select * from t_control_violation V order by V.ID";
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
            con.Close();
            return voilationList;
        }
        public List<AuditSubVoilationcatModel> GetVoilationSubGroup(int group_id)
        {
            var con = this.DatabaseConnection();
            List<AuditSubVoilationcatModel> voilationsubgroupList = new List<AuditSubVoilationcatModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (group_id == 0)
                    cmd.CommandText = "select * from t_control_violation_sub S order by S.ID";
                else
                    cmd.CommandText = "select * from t_control_violation_sub S where s.v_id= " + group_id + "  order by s.v_ID, s.ID asc";

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
            con.Close();
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
                cmd.CommandText = "select t.*, (select tt.type_id from t_auditee_entities tt where tt.entity_id=t.entity_id) as ENTITY_TYPE, (select ss.description from T_AU_AUDIT_TEAM_TASKLIST_STATUS ss where ss.status_id=(t.status_id+1)) as ENG_NEXT_STATUS, ta.T_NAME, ts.DESCRIPTION as ENG_STATUS from T_AU_AUDIT_TEAM_TASKLIST t inner join T_AU_AUDIT_TEAMS ta on t.TEAM_ID=ta.TEAM_ID and t.eng_plan_id=ta.eng_id inner join T_AU_AUDIT_TEAM_TASKLIST_STATUS ts on t.STATUS_ID = ts.STATUS_ID   WHERE t.teammember_ppno = " + loggedInUser.PPNumber + " order by t.SEQUENCE_NO";

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
                    tasklist.Add(tlist);
                }
            }
            con.Close();
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
                cmd.CommandText = "select t.team_id, tm.member_name,tm.member_ppno, tm.team_name as TEAM_NAME, t.entity_id, t.entity_code, t.entity_name, t.audit_start_date, t.audit_end_date, rt.description as RISK, st.description as ENT_SIZE ,p.description as AUDIT_PERIOD, tm.isteamlead from t_au_audit_team_tasklist t inner join t_au_plan_eng pe on t.eng_plan_id = pe.eng_id inner join t_au_period p on pe.period_id=p.auditperiodid inner join t_au_team_members tm on t.teammember_ppno=tm.member_ppno inner join t_au_audit_teams audt on audt.team_id = tm.t_id  inner join t_auditee_entities_risk r on t.entity_id=r.entity_id  inner join t_risk rt on r.risk_rating=rt.r_id  inner join t_auditee_entities_size s on t.entity_id=s.entity_id  inner join t_auditee_entities_size_disc st on s.entity_size=st.entity_size where t.eng_plan_id = " + engId + " and audt.eng_id = " + engId + " and tm.member_ppno=" + loggedInUser.PPNumber;

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
            con.Close();
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
            bool alreadyJoined = false;
            bool response = false;
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = cmd.CommandText = "SELECT ID FROM T_AU_AUDIT_JOINING WHERE ENG_PLAN_ID=" + jm.ENG_PLAN_ID + " and TEAM_MEM_PPNO=" + jm.TEAM_MEM_PPNO + " and STATUS='I'";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["ID"].ToString() != null && rdr["ID"].ToString() != "")
                    {
                        alreadyJoined = true;
                    }
                }
                if (!alreadyJoined)
                {
                    this.SetEngIdOnHold();
                    response = true;
                    cmd.CommandText = cmd.CommandText = "INSERT INTO T_AU_AUDIT_JOINING al (al.ID, al.ENG_PLAN_ID, al.TEAM_MEM_PPNO,al.JOINING_DATE , al.ENTEREDBY, al.ENTEREDDATE, al.COMPLETION_DATE, al.STATUS ) VALUES ( (select COALESCE(max(acc.ID)+1,1) from T_AU_AUDIT_JOINING acc) , '" + jm.ENG_PLAN_ID + "','" + jm.TEAM_MEM_PPNO + "',to_date('" + dtime.DateTimeInDDMMYY(jm.JOINING_DATE) + "','dd/mm/yyyy HH:MI:SS AM'),'" + jm.ENTEREDBY + "',to_date('" + dtime.DateTimeInDDMMYY(jm.ENTEREDDATE) + "','dd/mm/yyyy HH:MI:SS AM'), to_date('" + dtime.DateTimeInDDMMYY(jm.COMPLETION_DATE) + "','dd/mm/yyyy HH:MI:SS AM'), 'I')";
                    cmd.ExecuteReader();
                    cmd.CommandText = cmd.CommandText = "UPDATE T_AU_AUDIT_TEAM_TASKLIST SET STATUS_ID= (select COALESCE(acc.STATUS_ID+1,1) from T_AU_AUDIT_TEAM_TASKLIST acc WHERE acc.ENG_PLAN_ID=" + jm.ENG_PLAN_ID + " and acc.TEAMMEMBER_PPNO= " + jm.TEAM_MEM_PPNO + ") WHERE ENG_PLAN_ID=" + jm.ENG_PLAN_ID + " and TEAMMEMBER_PPNO= " + jm.TEAM_MEM_PPNO;
                    cmd.ExecuteReader();
                }
            }
            con.Close();
            return response;
        }
        public List<AuditChecklistModel> GetAuditChecklist()
        {
            var con = this.DatabaseConnection();
            List<AuditChecklistModel> list = new List<AuditChecklistModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {

                cmd.CommandText = "select t.*, e.entitytypedesc as ENTITY_TYPE_NAME from t_audit_checklist t inner join t_auditee_ent_types e on t.entity_type=e.autid where t.STATUS='Y' order by t.t_id asc";

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
            con.Close();
            return list;
        }
        public List<AuditChecklistSubModel> GetAuditChecklistSub(int t_id = 0, int eng_id = 0)
        {
            var con = this.DatabaseConnection();
            List<AuditChecklistSubModel> list = new List<AuditChecklistSubModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (t_id == 0)
                    cmd.CommandText = "select t.*, p.heading as T_NAME, e.entitytypedesc as ENTITY_TYPE_NAME  from t_audit_checklist_sub t inner join t_audit_checklist p on p.t_id=t.t_id inner join t_auditee_ent_types e on t.entity_type=e.autid where t.STATUS='Y' order by t.s_id asc";
                else
                    cmd.CommandText = "select t.*, p.heading as T_NAME, e.entitytypedesc as ENTITY_TYPE_NAME from t_audit_checklist_sub t inner join t_audit_checklist p on p.t_id=t.t_id inner join t_auditee_ent_types e on t.entity_type=e.autid where t.STATUS='Y' and t.t_id=" + t_id + " order by t.s_id asc";
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
                    if (eng_id != 0)
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
                    }

                    list.Add(chk);
                }
            }
            con.Close();
            return list;
        }
        public List<AuditChecklistDetailsModel> GetAuditChecklistDetails(int s_id = 0)
        {
            var con = this.DatabaseConnection();
            List<AuditChecklistDetailsModel> list = new List<AuditChecklistDetailsModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (s_id == 0)
                    cmd.CommandText = "select t.*, p.heading as S_NAME, s.description as V_NAME, r.description as RISK from t_audit_checklist_details t inner join t_audit_checklist_sub p on p.s_id=t.s_id inner join t_r_sub_group s on s.s_gr_id=t.v_id inner join t_risk r on r.r_id=t.risk_id where t.STATUS='Y' order by t.id asc";
                else
                    cmd.CommandText = "select t.*, p.heading as S_NAME, s.description as V_NAME, r.description as RISK from t_audit_checklist_details t inner join t_audit_checklist_sub p on p.s_id=t.s_id inner join t_r_sub_group s on s.s_gr_id=t.v_id inner join t_risk r on r.r_id=t.risk_id where t.STATUS='Y' and t.s_id=" + s_id + " order by t.id asc";
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
                        // chk.ROLE_RESP = rdr["ROLE_RESP"].ToString();
                    }
                    if (rdr["PROCESS_OWNER_ID"].ToString() != null && rdr["PROCESS_OWNER_ID"].ToString() != "")
                    {
                        chk.PROCESS_OWNER_ID = Convert.ToInt32(rdr["PROCESS_OWNER_ID"]);
                        // chk.PROCESS_OWNER = rdr["PROCESS_OWNER"].ToString();

                    }
                    chk.STATUS = rdr["STATUS"].ToString();
                    list.Add(chk);
                }
            }
            con.Close();
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
                //cmd.CommandText = "select * from V_GET_GL_SUM GH order by GH.GLSUBNAME, GH.MONTHEND";
                // cmd.CommandText = "select * from V_GET_GL_SUM GH where GH.BRANCHID IN (select e.entity_id from t_au_plan_eng e where e.eng_id=" + ENG_ID + " ) order by GH.GLSUBNAME, GH.MONTHEND";
                cmd.CommandText = "select * from V_GET_GL_INCOME_EXPENDITURES t where t.team_mem_ppno = '" + loggedInUser.PPNumber + "' and t.DESCRIPTION in ('EXPENSE','INCOME') ORDER BY T.DESCRIPTION, T.datetime";

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    GlHeadDetailsModel GlHeadDetails = new GlHeadDetailsModel();
                    GlHeadDetails.BRANCHID = Convert.ToInt32(rdr["BRANCHID"]);
                    GlHeadDetails.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    GlHeadDetails.GLSUBCODE = Convert.ToInt32(rdr["GLSUBCODE"]);
                    GlHeadDetails.GLSUBNAME = rdr["GLSUBNAME"].ToString();
                    GlHeadDetails.DATETIME = Convert.ToDateTime(rdr["DATETIME"]);
                    GlHeadDetails.BALANCE = Convert.ToDouble(rdr["BALANCE"]);
                    GlHeadDetails.DEBIT = Convert.ToDouble(rdr["DEBIT"]);
                    GlHeadDetails.CREDIT = Convert.ToDouble(rdr["CREDIT"]);
                    list.Add(GlHeadDetails);
                }
            }
            con.Close();
            return list;

        }
        public GlHeadSubDetailsModel GetGlheadSubDetails(int gl_code = 0)
        {
            var con = this.DatabaseConnection();
            GlHeadSubDetailsModel GlHeadSubDetails = new GlHeadSubDetailsModel();
            List<GlHeadSubDetailsModel> GlSubHeadList = new List<GlHeadSubDetailsModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {

                cmd.CommandText = "select* from V_GET_GLHEADS_DETAILS gh where gh.GLSUBCODE= " + gl_code + " order by gh.GLSUBNAME, gh.DATETIME";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {


                    GlHeadSubDetailsModel GHSD = new GlHeadSubDetailsModel();

                    GHSD.GLCODE = Convert.ToInt32(rdr["GLSUBCODE"]);
                    GHSD.GLSUBNAME = rdr["GLSUBNAME"].ToString();
                    GHSD.DATETIME = Convert.ToDateTime(rdr["DATETIME"]);
                    GHSD.BALANCE = Convert.ToDouble(rdr["BALANCE"]);
                    GHSD.RUNNING_DR = Convert.ToDouble(rdr["RUNNING_DR"]);
                    GHSD.RUNNING_CR = Convert.ToDouble(rdr["RUNNING_CR"]);
                    GlSubHeadList.Add(GHSD);
                    GlHeadSubDetails.GL_SUBDETAILS = GlSubHeadList;
                }
            }
            con.Close();
            return GlHeadSubDetails;

        }
        public List<LoanCaseModel> GetLoanCaseDetails(int lid = 0, string type = "")
        {
            int ENG_ID = this.GetLoggedInUserEngId();

            List<LoanCaseModel> list = new List<LoanCaseModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (type.ToLower() == "live" || type.ToLower() == "")
                    cmd.CommandText = "select * from V_CUSTOMER_LOAN_LIVE lcd where lcd.BRANCHID IN (select e.entity_id from t_au_plan_eng e where e.eng_id=" + ENG_ID + " ) order by lcd.DISB_DATE ";
                else
                    cmd.CommandText = "select * from v_customer_loan_close lcd where lcd.BRANCHID IN (select e.entity_id from t_au_plan_eng e where e.eng_id=" + ENG_ID + " ) order by lcd.DISB_DATE ";

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    LoanCaseModel LoanCaseDetails = new LoanCaseModel();
                    LoanCaseDetails.BRANCHID = Convert.ToInt32(rdr["BRANCHID"]);
                    LoanCaseDetails.CNIC = Convert.ToDouble(rdr["CNIC"]);
                    LoanCaseDetails.LOAN_CASE_NO = Convert.ToInt32(rdr["LOAN_CASE_NO"]);
                    LoanCaseDetails.CUSTOMERNAME = rdr["CUSTOMERNAME"].ToString();
                    LoanCaseDetails.FATHERNAME = rdr["FATHERNAME"].ToString();
                    LoanCaseDetails.DISBURSED_AMOUNT = Convert.ToDouble(rdr["DISBURSED_AMOUNT"]);
                    LoanCaseDetails.PRIN = Convert.ToDouble(rdr["PRIN"]);
                    LoanCaseDetails.MARKUP = Convert.ToDouble(rdr["MARKUP"]);
                    LoanCaseDetails.GLSUBCODE = Convert.ToInt32(rdr["GLSUBCODE"]);
                    LoanCaseDetails.LOAN_DISB_ID = Convert.ToDouble(rdr["LOAN_DISB_ID"]);
                    LoanCaseDetails.DISB_DATE = Convert.ToDateTime(rdr["DISB_DATE"]);
                    LoanCaseDetails.DISB_STATUSID = Convert.ToInt32(rdr["DISB_STATUSID"]);
                    list.Add(LoanCaseDetails);
                }
            }
            con.Close();
            return list;
        }
        public List<LoanCasedocModel> GetLoanCaseDocuments()
        {



            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();

            List<LoanCasedocModel> list = new List<LoanCasedocModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select * from v_list_cbas_sdms s where s.team_mem_ppno ='" + loggedInUser.PPNumber + "' ";

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
            con.Close();
            return list;
        }
        public List<GlHeadDetailsModel> GetIncomeExpenceDetails(int bid = 0)
        {
            int ENG_ID = this.GetLoggedInUserEngId();

            var con = this.DatabaseConnection();
            List<GlHeadDetailsModel> list = new List<GlHeadDetailsModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                //cmd.CommandText = "select * from V_GET_GL_SUM GH where GH.BRANCHID = " + brId + " and GH.DESCRIPTION IN  ('INCOME','EXPENSE')  order by GH.DESCRIPTION, GH.MONTHEND";
                cmd.CommandText = "select * from V_GET_GL_SUM GH where GH.DESCRIPTION IN  ('INCOME','EXPENSE') and GH.BRANCHID IN (select e.entity_id from t_au_plan_eng e where e.eng_id=" + ENG_ID + " )  order by GH.DESCRIPTION, GH.MONTHEND";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    GlHeadDetailsModel GlHeadDetails = new GlHeadDetailsModel();
                    GlHeadDetails.BRANCHID = Convert.ToInt32(rdr["BRANCHID"]);
                    GlHeadDetails.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    GlHeadDetails.GLSUBCODE = Convert.ToInt32(rdr["GLSUBCODE"]);
                    GlHeadDetails.GLSUBNAME = rdr["GLSUBNAME"].ToString();
                    GlHeadDetails.DATETIME = Convert.ToDateTime(rdr["DATETIME"]);
                    GlHeadDetails.BALANCE = Convert.ToDouble(rdr["BALANCE"]);
                    GlHeadDetails.DEBIT = Convert.ToDouble(rdr["DEBIT"]);
                    GlHeadDetails.CREDIT = Convert.ToDouble(rdr["CREDIT"]);
                    list.Add(GlHeadDetails);
                }
            }
            con.Close();
            return list;

        }
        public List<DepositAccountModel> GetDepositAccountdetails()
        {

            var con = this.DatabaseConnection();
            List<DepositAccountModel> depositacclist = new List<DepositAccountModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select distinct NAME from V_GET_BRANCH_DEPOSIT_ACCOUNTS n order by  n.NAME ";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DepositAccountModel depositaccdetails = new DepositAccountModel();
                    depositaccdetails.NAME = rdr["NAME"].ToString();
                    depositacclist.Add(depositaccdetails);
                }
            }
            con.Close();
            return depositacclist;
        }
        public List<DepositAccountModel> GetDepositAccountSubdetails(string bname = "")
        {
            int ENG_ID = this.GetLoggedInUserEngId();

            var con = this.DatabaseConnection();
            List<DepositAccountModel> depositaccsublist = new List<DepositAccountModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                //cmd.CommandText = "select * from V_GET_BRANCH_DEPOSIT_ACCOUNTS d where d.NAME = '" + bname + "' order by d.OPENINGDATE ";
                cmd.CommandText = "select  * from V_GET_BRANCH_DEPOSIT_ACCOUNTS d  where UPPER(d.NAME) = (SELECT UPPER(BRANCHNAME) FROM V_SERVICE_BRANCH WHERE BRANCHID IN (select e.entity_code from t_au_plan_eng e where e.eng_id=" + ENG_ID + " ) ) order by d.OPENINGDATE";
                //cmd.CommandText = "select  * from V_GET_BRANCH_DEPOSIT_ACCOUNTS d  where d.NAME = (SELECT BRANCHNAME FROM V_SERVICE_BRANCH WHERE BRANCHID= " + brId + ") order by d.OPENINGDATE";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DepositAccountModel depositaccsubdetails = new DepositAccountModel();

                    depositaccsubdetails.NAME = rdr["NAME"].ToString();
                    if (rdr["ACC_NUMBER"].ToString() != null && rdr["ACC_NUMBER"].ToString() != "")
                        depositaccsubdetails.ACC_NUMBER = Convert.ToDouble(rdr["ACC_NUMBER"]);
                    if (rdr["ACCOUNTCATEGORY"].ToString() != null && rdr["ACCOUNTCATEGORY"].ToString() != "")
                        depositaccsubdetails.ACCOUNTCATEGORY = rdr["ACCOUNTCATEGORY"].ToString();

                    if (rdr["CUST_NAME"].ToString() != null && rdr["CUST_NAME"].ToString() != "")
                        depositaccsubdetails.CUST_NAME = rdr["CUST_NAME"].ToString();


                    if (rdr["OPENINGDATE"].ToString() != null && rdr["OPENINGDATE"].ToString() != "")
                    {
                        depositaccsubdetails.OPENINGDATE = Convert.ToDateTime(rdr["OPENINGDATE"]);
                    }
                    if (rdr["CNIC"].ToString() != null && rdr["CNIC"].ToString() != "")
                    {
                        depositaccsubdetails.CNIC = Convert.ToDouble(rdr["CNIC"]);
                    }
                    if (rdr["ACC_TITLE"].ToString() != null && rdr["ACC_TITLE"].ToString() != "")
                        depositaccsubdetails.ACC_TITLE = rdr["ACC_TITLE"].ToString();

                    if (rdr["OLDACCOUNTNO"].ToString() != null && rdr["OLDACCOUNTNO"].ToString() != "")
                        depositaccsubdetails.OLDACCOUNTNO = Convert.ToDouble(rdr["OLDACCOUNTNO"]);
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
            con.Close();
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
                //cmd.CommandText = "select * from V_CUSTOMER_LOAN_LIVE lcd where lcd.BRANCHID= " + brId + " order by lcd.DISB_DATE ";
                cmd.CommandText = "select * from V_CUSTOMER_LOAN_LIVE lcd order by lcd.DISB_DATE fetch next 50 rows only";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    LoanCaseModel LoanCaseDetails = new LoanCaseModel();
                    LoanCaseDetails.BRANCHID = Convert.ToInt32(rdr["BRANCHID"]);
                    LoanCaseDetails.CNIC = Convert.ToDouble(rdr["CNIC"]);
                    LoanCaseDetails.LOAN_CASE_NO = Convert.ToInt32(rdr["LOAN_CASE_NO"]);
                    LoanCaseDetails.CUSTOMERNAME = rdr["CUSTOMERNAME"].ToString();
                    LoanCaseDetails.FATHERNAME = rdr["FATHERNAME"].ToString();
                    LoanCaseDetails.DISBURSED_AMOUNT = Convert.ToDouble(rdr["DISBURSED_AMOUNT"]);
                    LoanCaseDetails.PRIN = Convert.ToDouble(rdr["PRIN"]);
                    LoanCaseDetails.MARKUP = Convert.ToDouble(rdr["MARKUP"]);
                    LoanCaseDetails.GLSUBCODE = Convert.ToInt32(rdr["GLSUBCODE"]);
                    LoanCaseDetails.LOAN_DISB_ID = Convert.ToDouble(rdr["LOAN_DISB_ID"]);
                    LoanCaseDetails.DISB_DATE = Convert.ToDateTime(rdr["DISB_DATE"]);
                    LoanCaseDetails.DISB_STATUSID = Convert.ToInt32(rdr["DISB_STATUSID"]);
                    list.Add(LoanCaseDetails);
                }
            }
            con.Close();
            return list;
        }
        public bool SaveAuditObservation(ObservationModel ob)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            bool alreadyAddedOb = false;
            bool success = true;
            var loggedInUser = sessionHandler.GetSessionUser();
            if (ob.ENGPLANID == 0)
                ob.ENGPLANID = this.GetLoggedInUserEngId();

            ob.ENTEREDBY = Convert.ToInt32(loggedInUser.PPNumber);
            ob.ENTEREDDATE = System.DateTime.Now;
            ob.MEMO_DATE = System.DateTime.Now;
            if (ob.SUBCHECKLIST_ID != 0)
                ob.RESPONSIBILITY_ASSIGNED = "(select cd.role_resp_id from t_audit_checklist_details cd where cd.id=" + ob.SUBCHECKLIST_ID + ")";
            else
                ob.RESPONSIBILITY_ASSIGNED = "0";

            string RiskModelQuery = "";
            if (ob.SUBCHECKLIST_ID != 0)
                RiskModelQuery = "(select cd.v_id from t_audit_checklist_details cd where cd.id=" + ob.SUBCHECKLIST_ID + ")";
            else
                RiskModelQuery = "0";

            string MemoNumberQuery = "(select COALESCE(max(ob.memo_number)+1,1) from t_au_observation ob where ob.engplanid=" + ob.ENGPLANID + ")";
            string ReplyByQuery = "(select pe.entity_id from t_au_plan_eng pe where pe.eng_id= " + ob.ENGPLANID + ")";
            string SeverityQuery = "";
            if (ob.SEVERITY != 0)
                SeverityQuery = ob.SEVERITY.ToString();
            else
                SeverityQuery = " (SELECT RISK_ID FROM t_audit_checklist_details WHERE ID = " + ob.CHECKLISTDETAIL_ID + " and S_ID=" + ob.SUBCHECKLIST_ID + ")";
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (ob.CHECKLISTDETAIL_ID != 0)
                {

                    cmd.CommandText = "SELECT o.ID FROM T_AU_OBSERVATION o WHERE o.ENGPLANID =" + ob.ENGPLANID + " and o.SUBCHECKLIST_ID=" + ob.SUBCHECKLIST_ID + " and o.CHECKLISTDETAIL_ID=" + ob.CHECKLISTDETAIL_ID;
                    OracleDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        if (rdr["ID"].ToString() != null && rdr["ID"].ToString() != "")
                        {
                            alreadyAddedOb = true;
                            success = false;
                        }
                    }
                }

                if (!alreadyAddedOb)
                {


                    cmd.CommandText = "INSERT INTO T_AU_OBSERVATION o (o.ID, o.ENGPLANID, o.STATUS, o.ENTEREDBY, o.ENTEREDDATE, o.ENTITY_ID, o.REPLYDATE, o.MEMO_DATE, o.SEVERITY, o.MEMO_NUMBER, o.RESPONSIBILITY_ASSIGNED, o.RISKMODEL_ID, o.SUBCHECKLIST_ID, o.CHECKLISTDETAIL_ID, o.V_CAT_ID, o.V_CAT_NATURE_ID) VALUES ( (select COALESCE(max(acc.ID)+1,1) from T_AU_OBSERVATION acc) , '" + ob.ENGPLANID + "','" + ob.STATUS + "','" + ob.ENTEREDBY + "',to_date('" + dtime.DateTimeInDDMMYY(ob.ENTEREDDATE) + "','dd/mm/yyyy HH:MI:SS AM')," + ReplyByQuery + ",to_date('" + dtime.DateTimeInDDMMYY(ob.REPLYDATE) + "','dd/mm/yyyy HH:MI:SS AM'), to_date('" + dtime.DateTimeInDDMMYY(ob.MEMO_DATE) + "','dd/mm/yyyy HH:MI:SS AM'), " + SeverityQuery + "," + MemoNumberQuery + "," + ob.RESPONSIBILITY_ASSIGNED + " ," + RiskModelQuery + ",'" + ob.SUBCHECKLIST_ID + "','" + ob.CHECKLISTDETAIL_ID + "','" + ob.V_CAT_ID + "','" + ob.V_CAT_NATURE_ID + "')";
                    cmd.ExecuteReader();

                    string strSQL = "INSERT INTO T_AU_OBSERVATION_TEXT ot (ot.ID, ot.OBSERVATSION_ID, ot.TEXT, ot.ENTEREDBY, ot.ENTEREDDATE ) VALUES ( (select COALESCE(max(acc.ID)+1,1) from T_AU_OBSERVATION_TEXT acc) , (select max(o.ID) from T_AU_OBSERVATION o) , :TEXT_DATA,'" + ob.ENTEREDBY + "',to_date('" + dtime.DateTimeInDDMMYY(ob.ENTEREDDATE) + "','dd/mm/yyyy HH:MI:SS AM'))";
                    OracleParameter parmData = new OracleParameter();
                    parmData.Direction = System.Data.ParameterDirection.Input;
                    parmData.OracleDbType = OracleDbType.Clob;
                    parmData.ParameterName = "TEXT_DATA";
                    parmData.Value = ob.OBSERVATION_TEXT;
                    OracleCommand cm = new OracleCommand();
                    cm.Connection = con;
                    cm.Parameters.Add(parmData);
                    cm.CommandText = strSQL;
                    cm.ExecuteNonQuery();
                }
            }
            con.Close();
            return success;
        }
        public List<AssignedObservations> GetAssignedObservations(int ENG_ID)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();

            string query = " ENTITY_ID = " + loggedInUser.UserEntityID;
            if (ENG_ID != 0)
                query += " and engplanid = " + ENG_ID;

            List<AssignedObservations> list = new List<AssignedObservations>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                //cmd.CommandText = "select  vc.v_name  as Violation, vcs.sub_v_name AS NATURE, ch.heading as Process, csb.heading as Sub_process, cd.heading as Check_List_Details,  o.Memo_Date, o.replydate, t.*, ot.text as OBSERVATION_TEXT, ot.text_plain as OBSERVATION_TEXT_PLAIN, s.statusname as STATUS, o.STATUS as STATUS_ID, e.name AS ENTITY_NAME, pe.audit_startdate as AUDIT_STARTDATE, pe.audit_enddate as AUDIT_ENDDATE from t_au_observation_assignedto t inner  join t_au_observation o on o.id = t.obs_id inner join t_au_observation_text ot on ot.id = t.obs_text_id inner join t_au_observation_status s on o.status = s.statusid inner join t_auditee_entities e on e.entity_id = t.ENTITY_ID left   join t_control_violation vc on o.v_cat_id = vc.id left join t_control_violation_sub vcs on o.v_cat_nature_id = vcs.id inner join t_au_plan_eng pe on pe.entity_id = e.entity_id left join t_audit_checklist ch on ch.t_id = o.transaction_id left join t_audit_checklist_sub csb on csb.s_id = o.subchecklist_id left join t_audit_checklist_details cd on cd.id = o.checklistdetail_id   WHERE  " + query + " order by t.OBS_ID asc";
                cmd.CommandText = "select * FROM v_get_assinged_observations WHERE " + query;
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
                    if (chk.REPLIED.ToString().ToLower() == "y")
                    {
                        cmd.CommandText = "select REPLY from t_au_observations_auditee_response where au_obs_id = " + chk.OBS_ID + " and obs_text_id= " + chk.OBS_TEXT_ID;
                        OracleDataReader rdr2 = cmd.ExecuteReader();
                        while (rdr2.Read())
                        {
                            if (rdr2["REPLY"].ToString() != "" && rdr2["REPLY"].ToString() != null)
                                chk.REPLY_TEXT = rdr2["REPLY"].ToString();
                        }
                    }
                    chk.OBSERVATION_TEXT = rdr["OBSERVATION_TEXT"].ToString();

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
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.MEMO_DATE = rdr["MEMO_DATE"].ToString();
                    chk.MEMO_REPLY_DATE = rdr["REPLYDATE"].ToString();
                    list.Add(chk);
                }
            }
            con.Close();
            return list;
        }
        public List<AssignedObservations> GetAssignedObservationsForBranch(int ENG_ID)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();

            string query = " t.ENTITY_ID = " + loggedInUser.UserEntityID;
            if (ENG_ID != 0)
                query += " and o.engplanid = " + ENG_ID;

            List<AssignedObservations> list = new List<AssignedObservations>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select  vc.v_name       as Violation, vcs.sub_v_name AS NATURE, o.Memo_Date, o.replydate, t.*, ot.text as OBSERVATION_TEXT, ot.text_plain as OBSERVATION_TEXT_PLAIN, s.statusname as STATUS, o.STATUS as STATUS_ID, e.name AS ENTITY_NAME, pe.audit_startdate as AUDIT_STARTDATE, pe.audit_enddate as AUDIT_ENDDATE from t_au_observation_assignedto t inner  join t_au_observation o on o.id = t.obs_id inner join t_au_observation_text ot on ot.id = t.obs_text_id inner join t_au_observation_status s on o.status = s.statusid inner join t_auditee_entities e on e.entity_id = t.ENTITY_ID inner   join t_control_violation vc on o.v_cat_id = vc.id inner join t_control_violation_sub vcs on o.v_cat_nature_id = vcs.id inner join t_au_plan_eng pe on pe.entity_id = e.entity_id WHERE  " + query + " order by t.OBS_ID asc";
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
                    if (chk.REPLIED.ToString().ToLower() == "y")
                    {
                        cmd.CommandText = "select REPLY from t_au_observations_auditee_response where au_obs_id = " + chk.OBS_ID + " and obs_text_id= " + chk.OBS_TEXT_ID;
                        OracleDataReader rdr2 = cmd.ExecuteReader();
                        while (rdr2.Read())
                        {
                            if (rdr2["REPLY"].ToString() != "" && rdr2["REPLY"].ToString() != null)
                                chk.REPLY_TEXT = rdr2["REPLY"].ToString();
                        }
                    }
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
            con.Close();
            return list;
        }
        public List<object> GetObservationText(int OBS_ID)
        {
            var con = this.DatabaseConnection();
            List<object> list = new List<object>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetObservationText";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBS_ID", OracleDbType.Int32).Value = OBS_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr = cmd.ExecuteReader();
                //cmd.CommandText = "select ot.text from T_AU_OBSERVATION_TEXT ot where ot.OBSERVATSION_ID=" + OBS_ID;
                //OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    object result = new object();
                    result = rdr["TEXT"];
                    list.Add(result);

                }

                cmd.CommandText = "pkg_ais.P_GetOBSERVATIONSAUDITEERESPONSE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBS_ID", OracleDbType.Int32).Value = OBS_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                OracleDataReader rdr2 = cmd.ExecuteReader();
                //cmd.CommandText = "select ot.REPLY from T_AU_OBSERVATIONS_AUDITEE_RESPONSE ot where ot.au_obs_id=" + OBS_ID;
                //OracleDataReader rdr2 = cmd.ExecuteReader();
                while (rdr2.Read())
                {
                    object result = new object();
                    result = rdr2["REPLY"];
                    list.Add(result);
                }
            }
            con.Close();
            return list;
        }
        public bool ResponseAuditObservation(ObservationResponseModel ob)
        {
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
                //cmd.CommandText = "INSERT INTO T_AU_OBSERVATIONS_AUDITEE_RESPONSE o (o.ID, o.AU_OBS_ID, o.REPLY, o.REPLIEDBY, o.REPLIEDDATE, o.OBS_TEXT_ID, o.REPLY_ROLE, o.REMARKS, o.SUBMITTED ) VALUES ( (select COALESCE(max(acc.ID)+1,1) from T_AU_OBSERVATIONS_AUDITEE_RESPONSE acc) , '" + ob.AU_OBS_ID + "','" + ob.REPLY + "','" + ob.REPLIEDBY + "',to_date('" + dtime.DateTimeInDDMMYY(ob.REPLIEDDATE) + "','dd/mm/yyyy HH:MI:SS AM')," + ob.OBS_TEXT_ID + "," + ob.REPLY_ROLE + ",'" + ob.REMARKS + "','" + ob.SUBMITTED + "')";
                //cmd.ExecuteReader();


                string strSQL = "INSERT INTO T_AU_OBSERVATIONS_AUDITEE_RESPONSE o (o.ID, o.AU_OBS_ID, o.REPLY, o.REPLIEDBY, o.REPLIEDDATE, o.OBS_TEXT_ID, o.REPLY_ROLE, o.REMARKS, o.SUBMITTED ) VALUES ( (select COALESCE(max(acc.ID)+1,1) from T_AU_OBSERVATIONS_AUDITEE_RESPONSE acc) , '" + ob.AU_OBS_ID + "',:REPLY_DATA,'" + ob.REPLIEDBY + "',to_date('" + dtime.DateTimeInDDMMYY(ob.REPLIEDDATE) + "','dd/mm/yyyy HH:MI:SS AM')," + ob.OBS_TEXT_ID + "," + ob.REPLY_ROLE + ",'" + ob.REMARKS + "','" + ob.SUBMITTED + "')";
                OracleParameter parmData = new OracleParameter();
                parmData.Direction = System.Data.ParameterDirection.Input;
                parmData.OracleDbType = OracleDbType.Clob;
                parmData.ParameterName = "REPLY_DATA";
                parmData.Value = ob.REPLY;
                OracleCommand cm = new OracleCommand();
                cm.Connection = con;
                cm.Parameters.Add(parmData);
                cm.CommandText = strSQL;
                cm.ExecuteNonQuery();


                cmd.CommandText = "UPDATE T_AU_OBSERVATION_ASSIGNEDTO SET REPLIED='Y' WHERE OBS_ID=" + ob.AU_OBS_ID + " and OBS_TEXT_ID=" + ob.OBS_TEXT_ID;
                cmd.ExecuteReader();
                cmd.CommandText = "UPDATE t_au_observation SET STATUS=3, REPLYBY=" + loggedInUser.PPNumber + ", MEMO_REPLY_DATE = to_date('" + dtime.DateTimeInDDMMYY(ob.REPLIEDDATE) + "','dd/mm/yyyy HH:MI:SS AM') WHERE ID = " + ob.AU_OBS_ID;
                cmd.ExecuteReader();
            }
            con.Close();
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


                //cmd.CommandText = "select j.eng_plan_id from t_au_audit_joining j where j.team_mem_ppno=" + loggedInUser.PPNumber + " and j.status='I'";
                //OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    engId = Convert.ToInt32(rdr["eng_plan_id"]);
                }
            }
            con.Close();
            return engId;
        }
        public bool SetEngIdOnHold()
        {
            int ENG_ID = this.GetLoggedInUserEngId();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_SetEngIdOnHold";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENG_ID", OracleDbType.Int32).Value = ENG_ID;
                cmd.ExecuteReader();


                //cmd.CommandText = "Update t_au_audit_joining j SET j.STATUS='P' where j.eng_plan_id=" + ENG_ID;
                //cmd.ExecuteReader();
                //cmd.CommandText = "Update t_au_plan_eng e SET e.STATUS=5 where e.eng_id=" + ENG_ID;
                //cmd.ExecuteReader();
            }
            con.Close();
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

                //cmd.CommandText = "select  r.reply from t_au_observations_auditor_response  r where r.au_obs_id= " + obs_id + " and r.reply_role IN ('Team Lead', 'Team Member') order by r.id desc FETCH NEXT 1 ROWS ONLY";
                //OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    response = rdr["reply"].ToString();
                }
            }
            con.Close();
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

                //cmd.CommandText = "select  r.reply from t_au_observations_auditor_response  r where r.au_obs_id= " + obs_id + " and r.reply_role IN ('Departmental Head / Incharge AZ') order by r.id desc FETCH NEXT 1 ROWS ONLY";
                //OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    response = rdr["reply"].ToString();
                }
            }
            con.Close();
            return response;
        }
        public string GetRiskDescByID(int risk_id = 0)
        {
            var con = this.DatabaseConnection();
            string response = "";
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select  r.DESCRIPTION  from T_RISK r where r.R_ID= " + risk_id;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    response = rdr["DESCRIPTION"].ToString();
                }
            }
            con.Close();
            return response;
        }
        public string GetLatestCommentsOnProcess(int procId = 0)
        {
            var con = this.DatabaseConnection();
            string response = "";
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select l.comments from t_audit_checklist_details_log l where l.t_id= " + procId + "  order by l.created_on desc FETCH NEXT 1 ROWS ONLY";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    response = rdr["comments"].ToString();
                }
            }
            con.Close();
            return response;
        }
        public string GetLatestAuditeeResponse(int obs_id = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            string response = "";
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select  r.reply from t_au_observations_auditee_response  r where r.au_obs_id= " + obs_id + "  order by r.id desc FETCH NEXT 1 ROWS ONLY";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    response = rdr["reply"].ToString();
                }
            }
            con.Close();
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
                if (OBS_ID == 0)
                {
                    cmd.CommandText = "pkg_ais.P_GetManagedObservations";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("ENG_ID", OracleDbType.Int32).Value = ENG_ID;
                    cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    //cmd.CommandText = "select c.v_name as Violation, csb.sub_v_name AS NATURE, p.description as PERIOD, o.ID as OBS_ID, aee.name as ENTITY_NAME, o.memo_number as MEMO_NO, ot.text as OBS_TEXT, o.severity as OBS_RISK_ID, r.description as OBS_RISK, o.status as OBS_STATUS_ID, ost.Statusname as OBS_STATUS from t_au_observation o inner join t_au_plan_eng e on o.engplanid = e.eng_id inner join t_au_observation_text ot on o.id = ot.observatsion_id inner join t_auditee_entities aee on e.entity_id = aee.entity_id inner join t_control_violation c on c.id = o.v_cat_id inner join t_control_violation_sub csb on csb.id = o.v_cat_nature_id inner join t_risk r on r.r_id = o.severity inner join t_au_observation_status ost on o.status = ost.statusid inner join t_au_period p on p.auditperiodid = e.period_id Where o.engplanid = " + ENG_ID + "  order by o.memo_number ";
                }

                else
                {
                    cmd.CommandText = "select c.v_name as Violation, csb.sub_v_name AS NATURE, p.description as PERIOD, o.ID as OBS_ID, aee.name as ENTITY_NAME, o.memo_number as MEMO_NO, ot.text as OBS_TEXT, o.severity as OBS_RISK_ID, r.description as OBS_RISK, o.status as OBS_STATUS_ID, ost.Statusname as OBS_STATUS from t_au_observation o inner join t_au_plan_eng e on o.engplanid = e.eng_id inner join t_au_observation_text ot on o.id = ot.observatsion_id inner join t_auditee_entities aee on e.entity_id = aee.entity_id inner join t_control_violation c on c.id = o.v_cat_id inner join t_control_violation_sub csb on csb.id = o.v_cat_nature_id inner join t_risk r on r.r_id = o.severity inner join t_au_observation_status ost on o.status = ost.statusid inner join t_au_period p on p.auditperiodid = e.period_id Where o.ID=" + OBS_ID + " order by o.memo_number";
                }

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
                    chk.OBS_TEXT = rdr["OBS_TEXT"].ToString();
                    chk.OBS_REPLY = this.GetLatestAuditeeResponse(chk.OBS_ID);
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.OBS_STATUS = rdr["OBS_STATUS"].ToString();
                    chk.OBS_RISK = rdr["OBS_RISK"].ToString();
                    chk.PERIOD = rdr["PERIOD"].ToString();
                    list.Add(chk);
                }
            }
            con.Close();
            return list;
        }
        public List<ManageObservations> GetManagedObservationsForBranches(int ENG_ID = 0, int OBS_ID = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            if (ENG_ID == 0)
                ENG_ID = this.GetLoggedInUserEngId();

            List<ManageObservations> list = new List<ManageObservations>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (OBS_ID == 0)
                {
                    cmd.CommandText = "pkg_ais.P_GetManagedObservationsForBranches";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("ENG_ID", OracleDbType.Int32).Value = ENG_ID;
                    cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    //cmd.CommandText = "select c.heading as Process, cc.heading as Sub_process, csb.heading AS Check_List_Detail, p.description as PERIOD, o.ID as OBS_ID, aee.name as ENTITY_NAME, o.memo_number as MEMO_NO, ot.text as OBS_TEXT, o.severity as OBS_RISK_ID, r.description as OBS_RISK, o.status as OBS_STATUS_ID, ost.Statusname as OBS_STATUS from t_au_observation o inner join t_au_plan_eng e on o.engplanid = e.eng_id inner join t_au_observation_text ot on o.id = ot.observatsion_id inner join t_auditee_entities aee on e.entity_id = aee.entity_id inner join t_audit_checklist_sub cc on cc.s_id = o.subchecklist_id inner join t_audit_checklist_details csb on csb.id = o.checklistdetail_id inner join t_audit_checklist c on c.t_id = cc.t_id inner join t_risk r on r.r_id = o.severity inner join t_au_observation_status ost on o.status = ost.statusid inner join t_au_period p on p.auditperiodid = e.period_id Where o.engplanid = " + ENG_ID + "  order by o.memo_number ";
                }

                else
                    cmd.CommandText = "select c.heading as Process, cc.heading as Sub_process, csb.heading AS Check_List_Detail, p.description as PERIOD, o.ID as OBS_ID, aee.name as ENTITY_NAME, o.memo_number as MEMO_NO, ot.text as OBS_TEXT, o.severity as OBS_RISK_ID, r.description as OBS_RISK, o.status as OBS_STATUS_ID, ost.Statusname as OBS_STATUS from t_au_observation o inner join t_au_plan_eng e on o.engplanid = e.eng_id inner join t_au_observation_text ot on o.id = ot.observatsion_id inner join t_auditee_entities aee on e.entity_id = aee.entity_id inner join t_audit_checklist_sub cc on cc.s_id = o.subchecklist_id inner join t_audit_checklist_details csb on csb.id = o.checklistdetail_id inner join t_audit_checklist c on c.t_id = cc.t_id inner join t_risk r on r.r_id = o.severity inner join t_au_observation_status ost on o.status = ost.statusid inner join t_au_period p on p.auditperiodid = e.period_id Where o.ID=" + OBS_ID + " order by o.memo_number";

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ManageObservations chk = new ManageObservations();

                    chk.OBS_ID = Convert.ToInt32(rdr["OBS_ID"]);
                    chk.OBS_RISK_ID = Convert.ToInt32(rdr["OBS_RISK_ID"]);
                    chk.OBS_STATUS_ID = Convert.ToInt32(rdr["OBS_STATUS_ID"]);
                    chk.MEMO_NO = Convert.ToInt32(rdr["MEMO_NO"]);
                    chk.PROCESS = rdr["PROCESS"].ToString();
                    chk.SUB_PROCESS = rdr["SUB_PROCESS"].ToString();
                    chk.Checklist_Details = rdr["Check_List_Detail"].ToString();
                    chk.OBS_TEXT = rdr["OBS_TEXT"].ToString();
                    chk.OBS_REPLY = this.GetLatestAuditeeResponse(chk.OBS_ID);
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.OBS_STATUS = rdr["OBS_STATUS"].ToString();
                    chk.OBS_RISK = rdr["OBS_RISK"].ToString();
                    chk.PERIOD = rdr["PERIOD"].ToString();
                    list.Add(chk);
                }
            }
            con.Close();
            return list;
        }
        public List<ManageObservations> GetManagedDraftObservations(int ENG_ID = 0)
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
                cmd.Parameters.Add("ENG_ID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                //cmd.CommandText = "select * from V_GET_DRAFT_AUDIT_DEPARTMENT t where t.eng_id = '" + ENG_ID + "'";

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ManageObservations chk = new ManageObservations();
                    chk.OBS_ID = Convert.ToInt32(rdr["OBS_ID"]);
                    chk.OBS_RISK_ID = Convert.ToInt32(rdr["OBS_RISK_ID"]);
                    chk.OBS_STATUS_ID = Convert.ToInt32(rdr["OBS_STATUS_ID"]);
                    chk.MEMO_NO = Convert.ToInt32(rdr["MEMO_NO"]);


                    if (rdr["VIOLATION"].ToString() != null && rdr["VIOLATION"].ToString() != "")
                        chk.VIOLATION = rdr["VIOLATION"].ToString();

                    if (rdr["NATURE"].ToString() != null && rdr["NATURE"].ToString() != "")
                        chk.NATURE = rdr["NATURE"].ToString();


                    if (rdr["PROCESS"].ToString() != null && rdr["PROCESS"].ToString() != "")
                        chk.PROCESS = rdr["PROCESS"].ToString();

                    if (rdr["SUB_PROCESS"].ToString() != null && rdr["SUB_PROCESS"].ToString() != "")
                        chk.SUB_PROCESS = rdr["SUB_PROCESS"].ToString();

                    if (rdr["CHECK_LIST_DETAILS"].ToString() != null && rdr["CHECK_LIST_DETAILS"].ToString() != "")
                        chk.Checklist_Details = rdr["CHECK_LIST_DETAILS"].ToString();



                    chk.OBS_TEXT = rdr["OBS_TEXT"].ToString();
                    chk.OBS_REPLY = this.GetLatestAuditeeResponse(chk.OBS_ID);
                    chk.AUD_REPLY = this.GetLatestAuditorResponse(chk.OBS_ID);
                    chk.HEAD_REPLY = this.GetLatestDepartmentalHeadResponse(chk.OBS_ID);
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.OBS_STATUS = rdr["OBS_STATUS"].ToString();
                    chk.OBS_RISK = rdr["OBS_RISK"].ToString();
                    chk.PERIOD = rdr["PERIOD"].ToString();
                    list.Add(chk);

                }
            }
            con.Close();

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
                cmd.CommandText = "pkg_ais.P_GetManagedDraftObservationsForBranchesh";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENG_ID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                // cmd.CommandText = "select * from V_GET_DRAFT_AUDIT_BRANCH t where t.eng_id = '" + ENG_ID + "'";

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
                    list.Add(chk);

                }
            }
            con.Close();

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

                //cmd.CommandText = "begin v_Dash_Borad_of_Divisional_Head; end;";

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
                    chk.PERIOD = rdr["PERIOD"].ToString();
                    list.Add(chk);
                }
            }
            con.Close();
            return list;
        }
        public bool DropAuditObservation(int OBS_ID)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_DropAuditObservation";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBS_ID", OracleDbType.Int32).Value = OBS_ID;


                //cmd.CommandText = "UPDATE t_au_observation SET STATUS=" + NEW_STATUS_ID + " WHERE ID = " + OBS_ID;
                cmd.ExecuteReader();
            }
            return true;
        }
        public bool SubmitAuditObservationToAuditee(int OBS_ID)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            int NEW_STATUS_ID = 2;
            var loggedInUser = sessionHandler.GetSessionUser();
            var ENG_ID = "(select o.engplanid from t_au_observation o where o.id=" + OBS_ID + ")";
            int ENTEREDBY = Convert.ToInt32(loggedInUser.PPNumber);
            DateTime ENTEREDDATE = System.DateTime.Now;
            string ReplyByQuery = "(select pe.entity_id from t_au_plan_eng pe where pe.eng_id= " + ENG_ID + ")";
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_SubmitAuditObservationToAuditee";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("OBS_ID", OracleDbType.Int32).Value = OBS_ID;
                cmd.ExecuteReader();

                //cmd.CommandText = "UPDATE t_au_observation SET STATUS=" + NEW_STATUS_ID + " WHERE ID = " + OBS_ID;
                //cmd.ExecuteReader();
                //cmd.CommandText = "INSERT INTO T_AU_OBSERVATION_ASSIGNEDTO ot (ot.ID, ot.OBS_ID, ot.OBS_TEXT_ID, ot.entity_id, ot.ASSIGNEDBY, ot.ASSIGNED_DATE, ot.IS_ACTIVE, ot.REPLIED ) VALUES ( (select COALESCE(max(acc.ID)+1,1) from T_AU_OBSERVATION_ASSIGNEDTO acc) , " + OBS_ID + ", (select tt.ID from T_AU_OBSERVATION_TEXT tt WHERE tt.OBSERVATSION_ID = " + OBS_ID + "), " + ReplyByQuery + ",'" + ENTEREDBY + "',to_date('" + dtime.DateTimeInDDMMYY(ENTEREDDATE) + "','dd/mm/yyyy HH:MI:SS AM'),'Y','N')";
                //cmd.ExecuteReader();
            }
            return true;
        }
        public bool UpdateAuditObservationStatus(int OBS_ID, int NEW_STATUS_ID, string AUDITOR_COMMENT)
        {
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
                cmd.CommandText = "UPDATE T_AU_OBSERVATIONS_AUDITEE_RESPONSE  SET REMARKS ='" + Remarks + "' WHERE AU_OBS_ID=" + OBS_ID;
                cmd.ExecuteReader();
                cmd.CommandText = "UPDATE t_au_observation SET STATUS=" + NEW_STATUS_ID + " WHERE ID = " + OBS_ID;
                cmd.ExecuteReader();
                if (NEW_STATUS_ID < 6)
                {
                    cmd.CommandText = " UPDATE t_au_audit_team_tasklist set STATUS_ID =4 WHERE ENG_PLAN_ID IN (SELECT ENGPLANID FROM t_au_observation WHERE ID = " + OBS_ID + " ) and TEAMMEMBER_PPNO= " + loggedInUser.PPNumber;
                    cmd.ExecuteReader();
                }
                string remarks = "";
                if (NEW_STATUS_ID == 5)
                    remarks = "Add To Draft";
                if (NEW_STATUS_ID == 4)
                    remarks = "Resolved at Memo Level";
                if (NEW_STATUS_ID != 8)
                {
                    if (NEW_STATUS_ID == 9)
                        cmd.CommandText = "INSERT INTO T_AU_OBSERVATIONS_AUDITOR_RESPONSE o (o.ID, o.AU_OBS_ID, o.REPLY, o.REPLIEDBY, o.REPLIEDDATE, o.OBS_TEXT_ID, o.REPLY_ROLE, o.REMARKS, o.SUBMITTED ) VALUES ( (select COALESCE(max(acc.ID)+1,1) from T_AU_OBSERVATIONS_AUDITEE_RESPONSE acc) , '" + OBS_ID + "','" + AUDITOR_COMMENT + "','" + loggedInUser.PPNumber + "',to_date('" + dtime.DateTimeInDDMMYY(DateTime.Now) + "','dd/mm/yyyy HH:MI:SS AM'),(select ot.id from t_au_observation_text ot WHERE ot.observatsion_id= " + OBS_ID + " ),'Departmental Head / Incharge AZ','" + remarks + "','Y')";
                    else
                        cmd.CommandText = "INSERT INTO T_AU_OBSERVATIONS_AUDITOR_RESPONSE o (o.ID, o.AU_OBS_ID, o.REPLY, o.REPLIEDBY, o.REPLIEDDATE, o.OBS_TEXT_ID, o.REPLY_ROLE, o.REMARKS, o.SUBMITTED ) VALUES ( (select COALESCE(max(acc.ID)+1,1) from T_AU_OBSERVATIONS_AUDITEE_RESPONSE acc) , '" + OBS_ID + "','" + AUDITOR_COMMENT + "','" + loggedInUser.PPNumber + "',to_date('" + dtime.DateTimeInDDMMYY(DateTime.Now) + "','dd/mm/yyyy HH:MI:SS AM'),(select ot.id from t_au_observation_text ot WHERE ot.observatsion_id= " + OBS_ID + " ),(select case tmm.isteamlead when 'Y' Then 'Team Lead' when 'N' then 'Team Member' end from t_au_team_members tmm where tmm.t_code IN ( select distinct tm.t_code from t_au_team_members tm where tm.t_id IN ( select t.team_id from t_au_audit_teams t where t.eng_id IN ( select o.engplanid from t_au_observation o where o.id = " + OBS_ID + "))) and tmm.member_ppno=" + loggedInUser.PPNumber + "),'" + remarks + "','Y')";
                    cmd.ExecuteReader();
                }

            }
            con.Close();
            return true;
        }
        public List<ObservationModel> GetClosingDraftObservations(int ENG_ID = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            if (ENG_ID == 0)
                ENG_ID = this.GetLoggedInUserEngId();
            List<ObservationModel> list = new List<ObservationModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                //
                cmd.CommandText = "pkg_ais.p_GetClosingDraftObservations";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENG_ID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                //cmd.CommandText = "select * from t_au_observation o where o.enteredby in (select jj.team_mem_ppno from  t_au_audit_joining jj where jj.eng_plan_id IN (" + ENG_ID + ")) and o.engplanid in (" + ENG_ID + ") and o.engplanid IN (" + ENG_ID + ") order by o.memo_number";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ObservationModel chk = new ObservationModel();
                    chk.ID = Convert.ToInt32(rdr["ID"]);
                    chk.ENGPLANID = Convert.ToInt32(rdr["ENGPLANID"]);
                    chk.STATUS = Convert.ToInt32(rdr["STATUS"]);
                    chk.ENTEREDBY = Convert.ToInt32(rdr["ENTEREDBY"]);
                    chk.REPLYBY = Convert.ToInt32(rdr["REPLYBY"]);
                    chk.SEVERITY = Convert.ToInt32(rdr["SEVERITY"]);

                    cmd.CommandText = "select tmm.isteamlead from t_au_team_members tmm where tmm.t_code IN(select distinct tm.t_code from t_au_team_members tm where tm.t_id IN (select aut.team_id  from t_au_audit_teams aut where aut.eng_id=" + chk.ENGPLANID + ")) and tmm.member_ppno = " + chk.ENTEREDBY;
                    OracleDataReader rdr2 = cmd.ExecuteReader();
                    while (rdr2.Read())
                    {
                        if (rdr2["isteamlead"].ToString() != "" && rdr2["isteamlead"].ToString() != null)
                            chk.TEAM_LEAD = rdr2["isteamlead"].ToString();
                    }

                    list.Add(chk);
                }
            }
            con.Close();
            return list;
        }
        public List<ClosingDraftTeamDetailsModel> GetClosingDraftTeamDetails(int ENG_ID = 0)
        {
            var con = this.DatabaseConnection();
            if (ENG_ID == 0)
                ENG_ID = this.GetLoggedInUserEngId();
            List<ClosingDraftTeamDetailsModel> list = new List<ClosingDraftTeamDetailsModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "pkg_ais.P_GetClosingDraftTeamDetails";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("ENG_ID", OracleDbType.Int32).Value = ENG_ID;
                cmd.Parameters.Add("T_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;


                // cmd.CommandText = "select jo.*, tm.isteamlead, tm.member_name from t_au_audit_joining jo inner join t_au_team_members tm on tm.member_ppno=jo.team_mem_ppno inner join t_au_audit_teams aut on tm.t_id=aut.team_id  where jo.eng_plan_id IN (" + ENG_ID + ") ";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ClosingDraftTeamDetailsModel chk = new ClosingDraftTeamDetailsModel();
                    chk.ID = Convert.ToInt32(rdr["ID"]);
                    chk.ENG_PLAN_ID = Convert.ToInt32(rdr["ENG_PLAN_ID"]);
                    chk.TEAM_MEM_PPNO = Convert.ToInt32(rdr["TEAM_MEM_PPNO"]);
                    chk.JOINING_DATE = Convert.ToDateTime(rdr["ENTEREDDATE"]);
                    chk.COMPLETION_DATE = Convert.ToDateTime(rdr["COMPLETION_DATE"]);
                    chk.ISTEAMLEAD = rdr["ISTEAMLEAD"].ToString();
                    chk.MEMBER_NAME = rdr["MEMBER_NAME"].ToString();
                    list.Add(chk);
                }
            }
            con.Close();
            return list;
        }
        public bool CloseDraftAuditReport(int ENG_ID)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                DateTime UpdatedDate = System.DateTime.Now;
                cmd.CommandText = "UPDATE t_au_audit_joining set STATUS ='P', LASTUPDATEDBY=" + loggedInUser.PPNumber + ", LASTUPDATEDDATE = to_date('" + dtime.DateTimeInDDMMYY(UpdatedDate) + "','dd/mm/yyyy HH:MI:SS AM')  WHERE ENG_PLAN_ID=" + ENG_ID;
                cmd.ExecuteReader();
                cmd.CommandText = "UPDATE t_au_audit_team_tasklist set STATUS_ID =5 WHERE ENG_PLAN_ID=" + ENG_ID + " and TEAMMEMBER_PPNO=" + loggedInUser.PPNumber;
                cmd.ExecuteReader();


            }
            con.Close();
            return true;
        }
        public int GetExpectedCountOfAuditEntitiesOnCriteria(int RISK_ID, int SIZE_ID, int ENTITY_TYPE_ID, int PERIOD_ID, int FREQUENCY_ID)
        {
            var con = this.DatabaseConnection();
            int count = 0;
            int criteria_id = 0;
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select c.ID from t_audit_criteria c where c.entity_typeid=" + ENTITY_TYPE_ID + " and c.auditperiodid= " + PERIOD_ID + " and c.size_id=" + SIZE_ID + " and c.risk_id=" + RISK_ID + " and c.frequency_id=" + FREQUENCY_ID;
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["ID"].ToString() != null && rdr["ID"].ToString() != "")
                        criteria_id = Convert.ToInt32(rdr["ID"]);
                }
                cmd.CommandText = "begin Criteria(" + criteria_id + "); end;";
                cmd.ExecuteReader();

                cmd.CommandText = "select c.NO_OF_ENTITY from t_audit_criteria c where c.entity_typeid=" + ENTITY_TYPE_ID + " and c.auditperiodid= " + PERIOD_ID + " and c.size_id=" + SIZE_ID + " and c.risk_id=" + RISK_ID + " and c.frequency_id=" + FREQUENCY_ID;
                OracleDataReader rdr2 = cmd.ExecuteReader();
                while (rdr2.Read())
                {
                    if (rdr2["NO_OF_ENTITY"].ToString() != null && rdr2["NO_OF_ENTITY"].ToString() != "")
                        count = Convert.ToInt32(rdr2["NO_OF_ENTITY"]);
                }

            }
            con.Close();
            return count;
        }
        public bool DeletePendingCriteria(int RISK_ID, int SIZE_ID, int ENTITY_TYPE_ID, int PERIOD_ID, int FREQUENCY_ID)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {


                cmd.CommandText = "Delete FROM t_audit_criteria c where c.entity_typeid=" + ENTITY_TYPE_ID + " and c.auditperiodid= " + PERIOD_ID + " and c.size_id=" + SIZE_ID + " and c.risk_id=" + RISK_ID + " and c.frequency_id=" + FREQUENCY_ID;
                cmd.ExecuteReader();
            }
            con.Close();
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
                cmd.CommandText = "Update t_audit_criteria c SET c.CRITERIA_SUBMITTED='Y' where c.CREATED_BY=" + loggedInUser.UserEntityID;
                cmd.ExecuteReader();
            }
            con.Close();
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
                cmd.CommandText = "select cr.*,  t.final_score, t.final_rating as final_audit_rating from T_COSO_RATING_DEPARTMENT_INHERITED_RISK cr inner join T_COSO_RATING_DEPARTMENT_FINAL t on cr.dept_name = t.dept inner join t_au_period p on cr.audit_period = p.description inner join t_auditee_entities e on e.name = cr.dept_name where cr.audit_period = t.audit_period and e.auditby_id = " + loggedInUser.UserEntityID + " and p.auditperiodid = " + PERIOD_ID + " order by cr.DEPT_NAME ASC";
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
            con.Close();
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


                // cmd.CommandText = "select cr.*,  t.final_score, t.final_rating as final_audit_rating from T_COSO_RATING_DEPARTMENT_INHERITED_RISK cr inner join T_COSO_RATING_DEPARTMENT_FINAL t on cr.dept_name = t.dept inner join t_au_period p on cr.audit_period = p.description inner join t_auditee_entities e on e.name = cr.dept_name where cr.audit_period = t.audit_period and e.auditby_id = " + loggedInUser.UserEntityID + " and p.auditperiodid = " + PERIOD_ID + " order by cr.DEPT_NAME ASC";
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
            con.Close();
            return list;
        }
        public bool CAUOMAssignment(CAUOMAssignmentModel om)
        {
            string encodedMsg = encoderDecoder.Encrypt(om.CONTENTS_OF_OM);
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                string strSQL = "INSERT INTO T_CAU_OM (ID, OM_NO, CONTENTS_OF_OM, DIV_ID, STATUS ) VALUES ( (select COALESCE(max(acc.ID)+1,1) from T_CAU_OM acc), '" + om.OM_NO + "', :ENCODED_MSG, '" + om.DIV_ID + "', 1 )";
                OracleParameter parmData = new OracleParameter();
                parmData.Direction = System.Data.ParameterDirection.Input;
                parmData.OracleDbType = OracleDbType.Clob;
                parmData.ParameterName = "ENCODED_MSG";
                parmData.Value = encodedMsg;
                OracleCommand cm = new OracleCommand();
                cm.Connection = con;
                cm.Parameters.Add(parmData);
                cm.CommandText = strSQL;
                cm.ExecuteNonQuery();
            }
            con.Close();
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

                //cmd.CommandText = "select s.*, ts.DISCRIPTION from t_cau_om s inner join t_cau_status ts on s.STATUS=ts.ID order by s.ID";
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
            con.Close();
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
                if (loggedInUser.UserGroupID != 1)
                {
                    if (ENTITY_ID == 0)
                        cmd.CommandText = "select c.*, e.name as ENTITY_NAME,  r.description as RISK_DEF, v.v_name as VIOLATION_NAME from t_au_ccq c inner join t_auditee_entities e on e.entity_id = c.entity_id inner join t_user t on t.entity_id = e.auditby_id left join t_coso_risk r on r.r_id = c.risk_id left join t_control_violation v on v.id = c.control_violation_id where t.ppno = " + loggedInUser.PPNumber + " order by c.ID";
                    else
                        cmd.CommandText = "select c.*, e.name as ENTITY_NAME,  r.description as RISK_DEF, v.v_name as VIOLATION_NAME from t_au_ccq c inner join t_auditee_entities e on e.entity_id = c.entity_id inner join t_user t on t.entity_id = e.auditby_id left join t_coso_risk r on r.r_id = c.risk_id left join t_control_violation v on v.id = c.control_violation_id where t.ppno = " + loggedInUser.PPNumber + " and e.entity_id=" + ENTITY_ID + " order by c.ID";
                }
                else
                {
                    if (ENTITY_ID == 0)
                        cmd.CommandText = "select c.*, e.name as ENTITY_NAME,  r.description as RISK_DEF, v.v_name as VIOLATION_NAME from t_au_ccq c inner join t_auditee_entities e on e.entity_id = c.entity_id  left join t_coso_risk r on r.r_id = c.risk_id left join t_control_violation v on v.id = c.control_violation_id  order by c.ID";
                    else
                        cmd.CommandText = "select c.*, e.name as ENTITY_NAME,  r.description as RISK_DEF, v.v_name as VIOLATION_NAME from t_au_ccq c inner join t_auditee_entities e on e.entity_id = c.entity_id  left join t_coso_risk r on r.r_id = c.risk_id left join t_control_violation v on v.id = c.control_violation_id where e.entity_id=" + ENTITY_ID + " order by c.ID";
                }
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
            con.Close();
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
            int risk_rating = 0;
            if (ccq.RISK_ID == 1)
                risk_rating = 3;
            else if (ccq.RISK_ID == 2)
                risk_rating = 2;
            else if (ccq.RISK_ID == 3)
                risk_rating = 1;
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "update t_au_ccq c SET c.QUESTIONS='" + ccq.QUESTIONS + "', c.CONTROL_VIOLATION_ID=" + ccq.CONTROL_VIOLATION_ID + ", c.RISK_ID=" + ccq.RISK_ID + ", c.RISK_RATING=" + risk_rating + ", c.STATUS='" + ccq.STATUS + "', c.UPDATED_BY=" + loggedInUser.PPNumber + ", c.UPDATED_DATETIME=to_date('" + dtime.DateTimeInDDMMYY(System.DateTime.Now) + "','dd/mm/yyyy HH:MI:SS AM') WHERE c.ID = " + ccq.ID;
                cmd.ExecuteReader();
                resp = true;
            }
            con.Close();
            return resp;
        }
        public List<AuditeeOldParasModel> GetAuditeeOldParas()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            string query = "";
            if (loggedInUser.UserLocationType == "H")
                query = query + "  s.ENTITY_CODE=" + loggedInUser.UserPostingDept;
            else if (loggedInUser.UserLocationType == "B")
                query = query + "  s.ENTITY_CODE=" + loggedInUser.UserPostingBranch;
            else if (loggedInUser.UserLocationType == "Z")
            {
                if (loggedInUser.UserPostingAuditZone != 0 && loggedInUser.UserPostingAuditZone != null)
                    query = query + "  s.ENTITY_CODE=" + loggedInUser.UserPostingAuditZone;
                else
                    query = query + "  s.ENTITY_CODE=" + loggedInUser.UserPostingZone;
            }

            List<AuditeeOldParasModel> list = new List<AuditeeOldParasModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select s.*, et.entitytypedesc from T_AU_OLD_PARAS s inner join t_auditee_ent_types et on s.type_id=et.autid WHERE " + query + " order by  s.AUDIT_PERIOD, s.Entity_Name, s.para_no";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditeeOldParasModel chk = new AuditeeOldParasModel();
                    chk.ID = Convert.ToInt32(rdr["ID"]);
                    chk.ENTITY_CODE = Convert.ToInt32(rdr["ENTITY_CODE"]);
                    chk.TYPE_ID = Convert.ToInt32(rdr["TYPE_ID"]);
                    chk.AUDIT_PERIOD = Convert.ToInt32(rdr["AUDIT_PERIOD"]);
                    //chk.AUDIT_PERIOD_DES =rdr["audit_period_des"].ToString();
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
            con.Close();
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
                string strSQL = "INSERT INTO T_AU_OLD_PARAS_RESPONSE o (o.ID, o.AU_OBS_ID, o.REPLY, o.REPLIEDBY, o.REPLIEDDATE, o.REMARKS, o.SUBMITTED, o.STATUS) VALUES ( (select COALESCE(max(acc.ID)+1,1) from T_AU_OLD_PARAS_RESPONSE acc) , '" + ob.AU_OBS_ID + "',:REPLY,'" + ob.REPLIEDBY + "',to_date('" + dtime.DateTimeInDDMMYY(System.DateTime.Now) + "','dd/mm/yyyy HH:MI:SS AM'), 'Un-settled','Y', 25)";
                OracleParameter parmData = new OracleParameter();
                parmData.Direction = System.Data.ParameterDirection.Input;
                parmData.OracleDbType = OracleDbType.Clob;
                parmData.ParameterName = "REPLY";
                parmData.Value = ob.REPLY;
                OracleCommand cm = new OracleCommand();
                cm.Connection = con;
                cm.Parameters.Add(parmData);
                cm.CommandText = strSQL;
                cm.ExecuteNonQuery();
                success = true;
            }
            con.Close();
            return success;
        }
        public List<OldParasModel> GetOldParas(string AUDITED_BY, string AUDIT_YEAR)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<OldParasModel> list = new List<OldParasModel>();
            string whereClause = "";
            if (AUDITED_BY != "")
                whereClause += " and entity_id=" + AUDITED_BY;

            if (AUDIT_YEAR != "" && AUDIT_YEAR != "0")
                whereClause += " and AUDIT_PERIOD=" + AUDIT_YEAR;

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select * from t_au_old_paras_fad WHERE STATUS = 0 " + whereClause + " order by ID";
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
            con.Close();
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
                cmd.CommandText = "SELECT f.*, c.heading       as Process_Des, cc.heading as Sub_process_Des, csb.heading AS Check_List_Detail_Des FROM t_au_old_paras_fad f inner join t_audit_checklist_sub cc on cc.s_id = f.sub_process inner join t_audit_checklist_details csb on csb.id = f.process_detail inner join t_audit_checklist c on c.t_id = f.process WHERE f.STATUS = 1 and f.entity_id=" + loggedInUser.UserEntityID + " order by f.ID";
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
            con.Close();
            return list;
        }
        public List<AuditeeOldParasModel> GetOutstandingParas(string ENTITY_ID)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<AuditeeOldParasModel> list = new List<AuditeeOldParasModel>();
            string whereClause = "";
            if (ENTITY_ID != "")
                whereClause += " and e.entity_id=" + ENTITY_ID;



            whereClause += " and ENTEREDBY=" + loggedInUser.PPNumber;
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select * from t_au_old_paras_iams a inner join t_auditee_entities e on e.code = a.branchid  WHERE 1=1 " + whereClause + " order by ID";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditeeOldParasModel chk = new AuditeeOldParasModel();
                    chk.ID = Convert.ToInt32(rdr["ID"]);
                    chk.ENTITY_CODE = Convert.ToInt32(rdr["ENTITY_CODE"]);
                    chk.TYPE_ID = Convert.ToInt32(rdr["TYPE_ID"]);
                    //chk.TYPE_DES = rdr["TYPE_DES"].ToString();
                    chk.AUDIT_PERIOD = Convert.ToInt32(rdr["AUDIT_PERIOD"]);
                    chk.ENTITY_NAME = rdr["ENTITY_NAME"].ToString();
                    chk.PARA_NO = Convert.ToInt32(rdr["PARA_NO"]);
                    chk.GIST_OF_PARAS = rdr["GIST_OF_PARAS"].ToString();
                    chk.AUDITEE_RESPONSE = rdr["AUDITEE_RESPONSE"].ToString();
                    chk.AUDITOR_REMARKS = rdr["AUDITOR_REMARKS"].ToString();
                    if (rdr["DATE_OF_LAST_COMPLIANCE_RECEIVED"].ToString() != "" && rdr["DATE_OF_LAST_COMPLIANCE_RECEIVED"].ToString() != null)
                        chk.DATE_OF_LAST_COMPLIANCE_RECEIVED = Convert.ToDateTime(rdr["DATE_OF_LAST_COMPLIANCE_RECEIVED"]);
                    chk.AUDITEDBY = Convert.ToInt32(rdr["AUDITED_BY"].ToString());
                    list.Add(chk);
                }
            }
            con.Close();
            return list;
        }
        public List<OldParasModel> GetOldParasAuditYear()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<OldParasModel> list = new List<OldParasModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select distinct audit_period from t_au_old_paras_fad WHERE STATUS = 0 order by audit_period";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    OldParasModel chk = new OldParasModel();
                    chk.AUDIT_PERIOD = rdr["AUDIT_PERIOD"].ToString();
                    list.Add(chk);
                }
            }
            con.Close();
            return list;
        }
        public List<OldParasModel> GetOutstandingParasAuditYear()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<OldParasModel> list = new List<OldParasModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select distinct audit_period from t_au_old_paras order by audit_period";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    OldParasModel chk = new OldParasModel();
                    chk.AUDIT_PERIOD = rdr["AUDIT_PERIOD"].ToString();
                    list.Add(chk);
                }
            }
            con.Close();
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
                string strSQL = cmd.CommandText = "UPDATE T_AU_OLD_PARAS_FAD al SET al.PROCESS = '" + jm.PROCESS + "', al.SUB_PROCESS = '" + jm.SUB_PROCESS + "', al.PROCESS_DETAIL = '" + jm.PROCESS_DETAIL + "', al.STATUS = '" + jm.STATUS + "', al.ENTERED_BY = '" + jm.ENTERED_BY + "', al.ENTERED_ON = to_date('" + dtime.DateTimeInDDMMYY(System.DateTime.Now) + "','dd/mm/yyyy hh:mi:ss') , al.PARA_TEXT =:PARA_TEXT  WHERE al.ID = " + jm.ID;
                OracleParameter parmData = new OracleParameter();
                parmData.Direction = System.Data.ParameterDirection.Input;
                parmData.OracleDbType = OracleDbType.Clob;
                parmData.ParameterName = "PARA_TEXT";
                parmData.Value = jm.PARA_TEXT;
                OracleCommand cm = new OracleCommand();
                cm.Connection = con;
                cm.Parameters.Add(parmData);
                cm.CommandText = strSQL;
                cm.ExecuteNonQuery();

                foreach (int pp in PP_NOs)
                {
                    cmd.CommandText = "INSERT INTO t_au_observation_old_paras_responibility_assigned (ID, REF_P, PP_NO, STATUS) VALUES ( (select COALESCE(max(acc.ID)+1,1) from t_au_observation_old_paras_responibility_assigned acc), " + jm.ID + ", " + pp + ", 1 ) ";
                    cmd.ExecuteReader();

                }
            }
            con.Close();
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
                string strSQL = "INSERT INTO T_AU_OLD_PARAS_RESPONSE_FAD o (o.ID, o.REF_P, o.REPLY, o.REPLIEDBY, o.REPLIEDDATE, o.REMARKS, o.SUBMITTED, o.AUDITEDBY, o.ENTITY_ID,o.C_STATUS) VALUES ( (select COALESCE(max(acc.ID)+1,1) from T_AU_OLD_PARAS_RESPONSE_FAD acc) , '" + ID + "',:REPLY,'" + loggedInUser.PPNumber + "',to_date('" + dtime.DateTimeInDDMMYY(System.DateTime.Now) + "','dd/mm/yyyy HH:MI:SS AM'), 'Response Submitted','Y',(select f.auditedby from t_au_old_paras_fad f where f.id =" + ID + "), (select f.entity_id from t_au_old_paras_fad f where f.id =" + ID + "), 3)";
                OracleParameter parmData = new OracleParameter();
                parmData.Direction = System.Data.ParameterDirection.Input;
                parmData.OracleDbType = OracleDbType.Clob;
                parmData.ParameterName = "REPLY";
                parmData.Value = REPLY;
                OracleCommand cm = new OracleCommand();
                cm.Connection = con;
                cm.Parameters.Add(parmData);
                cm.CommandText = strSQL;
                cm.ExecuteNonQuery();


                cm.CommandText = "UPDATE T_AU_OLD_PARAS_FAD al SET al.STATUS=3 WHERE al.ID = " + ID;
                cm.ExecuteReader();
                success = true;
            }
            con.Close();
            return success;
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
                cmd.CommandText = "select * from v_report_az_progress s  where 1=1 and " + query;
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
            con.Close();
            return list;
        }
        public List<UserWiseOldParasPerformanceModel> GetUserWiseOldParasPerformance()
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            string query = "";
            query = query + "  s.AUDIT_ZONEID = " + loggedInUser.UserEntityID;


            List<UserWiseOldParasPerformanceModel> list = new List<UserWiseOldParasPerformanceModel>();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select s.*, sz.zonename from v_report_az_emp_progress s inner join v_report_az_progress sz on s.audit_zoneid = sz.id  where 1=1 and " + query;
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
            con.Close();
            return list;
        }
        public ActiveInactiveChart GetActiveInactiveChartData()
        {
            var con = this.DatabaseConnection();
            ActiveInactiveChart chk = new ActiveInactiveChart();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select f.status, count(f.status) as total_count  from t_au_old_paras_fad f group by f.status";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {

                    if (rdr["STATUS"].ToString() == "1")
                        chk.Active_Count = rdr["TOTAL_COUNT"].ToString();
                    if (rdr["STATUS"].ToString() == "0")
                        chk.Inactive_Count = rdr["TOTAL_COUNT"].ToString();
                }
            }
            con.Close();
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
                if (loggedInUser.UserGroupID != 1)
                    cmd.CommandText = "select distinct t.name, t.code, t.entity_id, j.eng_plan_id from t_au_audit_joining j inner join t_au_plan_eng e on e.eng_id = j.eng_plan_id inner join t_auditee_entities t on t.entity_id = e.entity_id inner join t_au_period p on e.period_id = p.auditperiodid where p.status_id = 2 and j.team_mem_ppno = " + loggedInUser.PPNumber;
                else
                    cmd.CommandText = "select distinct t.name, t.code, t.entity_id, j.eng_plan_id from t_au_audit_joining j inner join t_au_plan_eng e on e.eng_id = j.eng_plan_id inner join t_auditee_entities t on t.entity_id = e.entity_id inner join t_au_period p on e.period_id = p.auditperiodid where p.status_id = 2";
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
            con.Close();
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
                if (loggedInUser.UserGroupID != 1)
                    cmd.CommandText = "select distinct t.name, t.code, t.entity_id, o.engplanid from t_au_observation o inner join t_au_plan_eng e on e.eng_id = o.engplanid inner join t_auditee_entities t on t.entity_id = e.entity_id inner join t_au_period p on e.period_id = p.auditperiodid inner join t_au_observation_assignedto ot on o.id=ot.obs_id where p.status_id = 2  and ot.entity_id = " + loggedInUser.UserEntityID;
                else
                    cmd.CommandText = "select distinct t.name, t.code, t.entity_id, o.engplanid from t_au_observation o inner join t_au_plan_eng e on e.eng_id = o.engplanid inner join t_auditee_entities t on t.entity_id = e.entity_id inner join t_au_period p on e.period_id = p.auditperiodid inner join t_au_observation_assignedto ot on o.id=ot.obs_id where p.status_id = 2";
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
            con.Close();
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
                if (loggedInUser.UserGroupID != 1)
                    cmd.CommandText = "select distinct e.code, e.name, e.entity_id from t_au_ccq c inner join t_auditee_entities e on e.entity_id = c.entity_id inner join t_user t on t.entity_id = e.auditby_id where t.ppno = " + loggedInUser.PPNumber;
                else
                    cmd.CommandText = "select distinct e.code, e.name, e.entity_id from t_au_ccq c inner join t_auditee_entities e on e.entity_id = c.entity_id ";
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
            con.Close();
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
                cmd.CommandText = "select distinct(r.entity_id), r.c_name, r.c_name, e.status from t_auditee_ent_relation   e, t_auditee_ent_types    t, T_AUDITEE_ENTITIES_MAPING r where t.autid = e.parent_entity_typeid and r.p_type_id = e.parent_entity_typeid and r.parent_id = " + e_r_id + " order by r.c_name";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserRelationshipModel entity = new UserRelationshipModel();
                    entity.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    entity.C_NAME = rdr["C_NAME"].ToString();
                    entitiesList.Add(entity);
                }
            }
            con.Close();
            return entitiesList;

        }
        public List<UserRelationshipModel> Getparentrepoffice(int r_id = 0)
        {

            List<UserRelationshipModel> entitiesList = new List<UserRelationshipModel>();
            var con = this.DatabaseConnection();

            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select r.entity_realtion_id, t.entitytypedesc, e.entity_id, e.description, e.active from t_auditee_entities e, t_auditee_ent_types t, t_auditee_ent_relation r where r.parent_entity_typeid = e.type_id and t.entitycode = e.type_id and r.entity_realtion_id = " + r_id + " order by e.description";
                //cmd.CommandText = "select * from ztblaisdev.t_auditee_ent_relation";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserRelationshipModel entity = new UserRelationshipModel();
                    //  entity.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);


                    //entity.ENTITYTYPEDESC = rdr["ENTITYTYPEDESC"].ToString();
                    // entity.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    // entity.ACTIVE = rdr["ACTIVE"].ToString();

                    entity.ENTITY_REALTION_ID = Convert.ToInt32(rdr["ENTITY_REALTION_ID"]);
                    entity.ENTITY_ID = Convert.ToInt32(rdr["ENTITY_ID"]);
                    entity.ACTIVE = rdr["ACTIVE"].ToString();
                    entity.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    entity.ENTITYTYPEDESC = rdr["ENTITYTYPEDESC"].ToString();




                    entitiesList.Add(entity);
                }
            }
            con.Close();
            return entitiesList;

        }
        public List<UserRelationshipModel> Getrealtionshiptype()
        {

            List<UserRelationshipModel> entitiesList = new List<UserRelationshipModel>();
            var con = this.DatabaseConnection();

            using (OracleCommand cmd = con.CreateCommand())
            {
                // cmd.CommandText = "select t.entitytypedesc, e.entity_id, e.description, e.active from t_auditee_entities e, t_auditee_ent_types t, t_auditee_ent_relation r where r.child_entity_typeid = e.type_id and t.entitycode = e.type_id and r.entity_realtion_id = 4 order by e.description";
                cmd.CommandText = "select f.entity_realtion_id,  f.parent_name||' TO '||f.chlid_name as field_name from t_auditee_ent_relation f";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserRelationshipModel entity = new UserRelationshipModel();
                    //entity.SR = Convert.ToInt32(rdr["SR"]);
                    entity.ENTITY_REALTION_ID = Convert.ToInt32(rdr["ENTITY_REALTION_ID"]);

                    entity.FIELD_NAME = rdr["FIELD_NAME"].ToString();




                    entitiesList.Add(entity);
                }
            }
            con.Close();
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
                cmd.CommandText = "select emp.ppno as PPNO, emp.employee_name as EMPLOYEE_NAME, emp.designation as DESIGNATION, emp.rank_desc as RANK_DESC, emp.place_of_posting as PLACE_OF_POSTING from T_AU_ACTIVE_EMPOLYEES emp inner join t_au_plan_eng eg on eg.entity_id = emp.entity_id  inner join t_au_audit_joining j on j.eng_plan_id = eg.eng_id where j.team_mem_ppno = '" + loggedInUser.PPNumber + "'  order by emp.rank_code";

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    StaffPositionModel staffposition = new StaffPositionModel();
                    staffposition.PPNO = Convert.ToInt32(rdr["PPNO"]);
                    staffposition.EMPLOYEE_NAME = Convert.ToString(rdr["EMPLOYEE_NAME"]);
                    staffposition.DESIGNATION = Convert.ToString(rdr["DESIGNATION"]);
                    staffposition.RANK_DESC = Convert.ToString(rdr["RANK_DESC"]);
                    staffposition.PLACE_OF_POSTING = Convert.ToString(rdr["PLACE_OF_POSTING"]);


                    list.Add(staffposition);
                }
            }
            con.Close();
            return list;
        }
        public List<FunctionalResponsibilityWiseParas> GetFunctionalResponsibilityWisePara(int PROCESS_ID = 0, int SUB_PROCESS_ID = 0, int PROCESS_DETAIL_ID = 0)
        {
            sessionHandler = new SessionHandler();
            sessionHandler._httpCon = this._httpCon;
            sessionHandler._session = this._session;
            var loggedInUser = sessionHandler.GetSessionUser();
            string whereClause = " 1=1";
            if (PROCESS_DETAIL_ID != 0)
                whereClause += " and CHECK_LIST_DETAIL_ID = " + PROCESS_DETAIL_ID;
            if (SUB_PROCESS_ID != 0)
                whereClause += " and SUB_PROCESS_ID= " + SUB_PROCESS_ID;
            if (PROCESS_ID != 0)
                whereClause += " and PROCESS_ID= " + PROCESS_ID;

            List<FunctionalResponsibilityWiseParas> list = new List<FunctionalResponsibilityWiseParas>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select * FROM v_dash_borad_of_divisional_head WHERE " + whereClause;

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
            con.Close();
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
                cmd.CommandText = "INSERT INTO t_au_old_functional_para_division_head_remarks (ID, PARA_ID, REF_P, ENTITY_ID, ENTITY_NAME, PARA_NO, CONCERNED_DEPT_ID, REMARKS, CREATED_BY) VALUES ( (select COALESCE(max(p.ID)+1,1) from t_au_old_functional_para_division_head_remarks p) , '" + REF_PARA_ID + "',(select f.REF_P from t_au_old_paras_fad f where f.id=" + REF_PARA_ID + " ),(select f.entity_id from t_au_old_paras_fad f where f.id=" + REF_PARA_ID + " ), (select f.entity_name from t_au_old_paras_fad f where f.id=" + REF_PARA_ID + " ), (select f.para_no from t_au_old_paras_fad f where f.id=" + REF_PARA_ID + " )," + CONCERNED_DEPT_ID + ", '" + COMMENTS + "', " + loggedInUser.PPNumber + ")";
                cmd.ExecuteReader();
            }
            con.Close();
            return true;
        }
    }
}

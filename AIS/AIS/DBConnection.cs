using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIS.Models;
using Oracle.ManagedDataAccess.Client;
using System.Security.Cryptography;
using System.Text;

namespace AIS
{
    
    public class DBConnection
    {
        private readonly SessionHandler sessionHandler = new SessionHandler();
        private OracleConnection DatabaseConnection()
        {
            try
            {
                // create connection
               
                OracleConnection con = new OracleConnection();
                // create connection string using builder
                
                OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
                ocsb.Password = "ztblais";
                ocsb.UserID = "ztblais";
                ocsb.DataSource = "10.1.100.222:1521/devdb18c.ztbl.com.pk";
                // connect
                con.ConnectionString = ocsb.ConnectionString;
                con.Open();
                EmailConfiguration email = new EmailConfiguration();
                email.ConfigEmail();
                return con;

            }
            catch (Exception) { return null; }
        }

        public UserModel AutheticateLogin(LoginModel login)
        {
            var con = this.DatabaseConnection();
            UserModel user = new UserModel();
            var enc_pass = getMd5Hash(login.Password);
            using (OracleCommand cmd = con.CreateCommand())
            {
                //con.Open();
                cmd.CommandText = "Select U.*, UM.* FROM v_service_user u inner join t_user_maping um on u.USERID = um.userid WHERE U.PPNO ='" + login.PPNumber + "' and u.Password ='" + enc_pass + "' and u.ISACTIVE='Y'"; 
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    
                    user.ID = Convert.ToInt32(rdr["USERID"]);
                    user.Name = rdr["LOGIN_NAME"].ToString();
                    user.Email = rdr["LOGIN_NAME"].ToString();
                    user.PPNumber = rdr["PPNO"].ToString();
                    user.UserLocationType = rdr["USER_LOCATION_TYPE"].ToString();
                    user.IsActive = rdr["ISACTIVE"].ToString();
                    if (rdr["DIVISIONID"].ToString()!=null && rdr["DIVISIONID"].ToString() != "")
                    user.UserPostingDiv = Convert.ToInt32(rdr["DIVISIONID"]);
                    if (rdr["DEPARTMENTID"].ToString() != null && rdr["DEPARTMENTID"].ToString() != "")
                        user.UserPostingDept = Convert.ToInt32(rdr["DEPARTMENTID"]);
                    if (rdr["ZONEID"].ToString() != null && rdr["ZONEID"].ToString() != "")
                        user.UserPostingZone = Convert.ToInt32(rdr["ZONEID"]);
                    if (rdr["BRANCHID"].ToString() != null && rdr["BRANCHID"].ToString() != "")
                        user.UserPostingBranch = Convert.ToInt32(rdr["BRANCHID"]);
                    if (rdr["AUDIT_ZONEID"].ToString() != null && rdr["AUDIT_ZONEID"].ToString() != "")
                        user.UserPostingAuditZone = Convert.ToInt32(rdr["AUDIT_ZONEID"]);
                    if (rdr["GROUP_ID"].ToString() != null && rdr["GROUP_ID"].ToString() != "")
                        user.UserGroupID = Convert.ToInt32(rdr["GROUP_ID"]);
                    if (rdr["ROLE_ID"].ToString() != null && rdr["ROLE_ID"].ToString() != "")
                        user.UserRoleID = Convert.ToInt32(rdr["ROLE_ID"]);

                }
            }
            con.Close();
            sessionHandler.SetSessionUser(user);
            return user;
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
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<MenuModel> modelList = new List<MenuModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select m.* from t_menu m, t_user_group_map r where m.menu_id = r.menu_id and r.role_id="+loggedInUser.UserRoleID+" ORDER BY M.MENU_ORDER ASC" +
                    "";
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
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            List<MenuPagesModel> modelList = new List<MenuPagesModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "Select * FROM T_MENU_PAGES mp inner join t_menu_pages_groupmap mpg on mp.Id=mpg.page_id WHERE mp.Status='A' and mpg.GROUP_ID= "+loggedInUser.UserGroupID+" order by mp.PAGE_ORDER asc";
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
        public List<MenuModel> GetAllTopMenus()
        {
            var con = this.DatabaseConnection();
            List<MenuModel> modelList = new List<MenuModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select m.* from  t_menu m ORDER BY M.MENU_ORDER ASC";
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
        public List<MenuPagesModel> GetAllMenuPages(int menuId=0)
        {
            var con = this.DatabaseConnection();

            List<MenuPagesModel> modelList = new List<MenuPagesModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if(menuId==0)
                cmd.CommandText = "Select * FROM T_MENU_PAGES mp WHERE mp.Status='A' order by mp.PAGE_ORDER asc";
                else
                    cmd.CommandText = "Select * FROM T_MENU_PAGES mp WHERE mp.Status='A' and mp.MENU_ID="+menuId+" order by mp.PAGE_ORDER asc";
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
        public List<MenuPagesModel> GetAssignedMenuPages(int groupId,int menuId)
        {
            var con = this.DatabaseConnection();

            List<MenuPagesModel> modelList = new List<MenuPagesModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "Select * FROM T_MENU_PAGES mp inner join t_menu_pages_groupmap mpg on mp.Id=mpg.page_id WHERE mp.Status='A' and mpg.GROUP_ID= "+groupId+" and mp.MENU_ID = "+menuId+"  order by mp.PAGE_ORDER asc";
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
        public List<GroupModel> GetGroups()
        {
            var con = this.DatabaseConnection();
            List<GroupModel> groupList = new List<GroupModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select g.* from  t_groups g WHERE g.STATUS='Y' ORDER BY g.GROUP_ID";
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
                    cmd.CommandText = "INSERT INTO t_groups g (g.ROLE_ID, g.GROUP_ID, g.DESCRIPTION, g.GROUP_NAME, g.STATUS) VALUES ( (select COALESCE(max(pr.ROLE_ID)+1,1) from t_groups pr),(select COALESCE(max(pg.GROUP_ID)+1,1) from t_groups pg), '" + gm.GROUP_DESCRIPTION+ "', '" + gm.GROUP_NAME + "', '" + gm.ISACTIVE + "')";
                else
                    cmd.CommandText = "UPDATE T_GROUPS g SET g.GROUP_NAME = '" + gm.GROUP_NAME+"', g.DESCRIPTION='"+gm.GROUP_DESCRIPTION+"', g.STATUS='"+gm.ISACTIVE+"' WHERE g.GROUP_ID="+gm.GROUP_ID;
                OracleDataReader rdr = cmd.ExecuteReader();
            }
            con.Close();
            return gm;
        }
        public List<UserModel> GetAllUsers(FindUserModel user)
        {
            string whereClause = " 1 = 1 ";
            if (user.DIVISIONID != 0)
                whereClause = whereClause+ " and e.CURRENTDIVISIONCODE =" + user.DIVISIONID;
            if (user.DEPARTMENTID != 0)
                whereClause = whereClause+ " and e.CURRENTDEPARTMENTCODE =" + user.DEPARTMENTID;
            if (user.ZONEID != 0)
                whereClause = whereClause + " and e.CURRENTZONECODE =" + user.ZONEID;
            if (user.BRANCHID != 0)
                whereClause = whereClause + " and e.CURRENTZONECODE =" + user.BRANCHID;
            if (user.EMAIL != "" && user.EMAIL != null)
                whereClause = whereClause + " and e.EMAIL like  %'" + user.EMAIL+"'%";
            if (user.GROUPID != 0)
                whereClause = whereClause + " and r.ROLE_ID =" + user.GROUPID;
            if (user.PPNUMBER != 0)
                whereClause = whereClause + " and e.PPNO =" + user.PPNUMBER;
            if (user.LOGINNAME != 0)
                whereClause = whereClause + " and e.PPNO =" + user.LOGINNAME;

            List<UserModel> userList = new List<UserModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select  u.USERID, r.group_name, rm.group_id, d.NAME as divName, dep.NAME as DeptName, z.ZONENAME, b.BRANCHNAME, e.* from v_service_employeeinfo e left join v_service_user u on e.PPNO=u.PPNO left join v_service_division d on e.CURRENTDIVISIONCODE=d.CODE left join v_service_department dep on e.CURRENTDEPARTMENTCODE=dep.ID left join v_service_zones z on e.CURRENTZONECODE=z.ZONECODE left join v_service_branch b on e.CURRENTBRANCHCODE=b.BRANCHCODE left join t_user_maping rm on e.PPNO=rm.ppno left join t_groups r on r.role_id=rm.role_id WHERE "+whereClause+" ORDER BY u.USERID";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserModel um = new UserModel();
                    if (rdr["USERID"].ToString() != null && rdr["USERID"].ToString() != "")
                        um.ID = Convert.ToInt32(rdr["USERID"]);
                    um.PPNumber = rdr["PPNO"].ToString();
                    um.Name = rdr["EMPLOYEEFIRSTNAME"].ToString()+" "+ rdr["EMPLOYEELASTNAME"].ToString();
                    um.Email = rdr["EMAIL"].ToString();
                    if(rdr["CURRENTBRANCHCODE"].ToString()!= null && rdr["CURRENTBRANCHCODE"].ToString() != "")
                    um.UserPostingBranch = Convert.ToInt32(rdr["CURRENTBRANCHCODE"]);
                    if (rdr["CURRENTDEPARTMENTCODE"].ToString() != null && rdr["CURRENTDEPARTMENTCODE"].ToString() != "")
                        um.UserPostingDept = Convert.ToInt32(rdr["CURRENTDEPARTMENTCODE"]);
                    if (rdr["CURRENTZONECODE"].ToString() != null && rdr["CURRENTZONECODE"].ToString() != "")
                        um.UserPostingZone = Convert.ToInt32(rdr["CURRENTZONECODE"]);
                    if (rdr["CURRENTDIVISIONCODE"].ToString() != null && rdr["CURRENTDIVISIONCODE"].ToString() != "")
                        um.UserPostingDiv = Convert.ToInt32(rdr["CURRENTDIVISIONCODE"]);
                    if (rdr["group_id"].ToString() != null && rdr["group_id"].ToString() != "")
                        um.UserGroupID = Convert.ToInt32(rdr["group_id"]);
                    um.DivName= rdr["divName"].ToString();
                    um.DeptName = rdr["DeptName"].ToString();
                    um.ZoneName = rdr["ZONENAME"].ToString();
                    um.BranchName = rdr["BRANCHNAME"].ToString();
                    um.UserRole = rdr["group_name"].ToString();
                    um.UserGroup = rdr["group_name"].ToString();
                    um.IsActive= rdr["STATUSTYPE"].ToString();
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
                cmd.CommandText = "SELECT * FROM  t_auditee_ent_types et where et.auditable='A'";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditEntitiesModel entity = new AuditEntitiesModel();                   
                    entity.AUTID = Convert.ToInt32(rdr["AUTID"]);
                    entity.ENTITYCODE = rdr["ENTITYCODE"].ToString();
                    entity.ENTITYTYPEDESC = rdr["ENTITYTYPEDESC"].ToString();
                    if (rdr["EFFECTIVE_FROM"].ToString() != null && rdr["EFFECTIVE_FROM"].ToString() != "")
                        entity.EFFECTIVE_FROM = Convert.ToDateTime(rdr["EFFECTIVE_FROM"].ToString());

                    if (rdr["VALIDUNTIL"].ToString() != null && rdr["VALIDUNTIL"].ToString() != "")
                        entity.VALIDUNTIL = Convert.ToDateTime(rdr["VALIDUNTIL"].ToString()); 
                    entity.ACTIVE = rdr["ACTIVE"].ToString();
                    entity.CREATED_BY = rdr["CREATED_BY"].ToString();
                    if (rdr["CREATED_ON"].ToString() != null && rdr["CREATED_ON"].ToString() != "")
                        entity.CREATED_ON = Convert.ToDateTime(rdr["CREATED_ON"].ToString());

                    if (rdr["RECORD_TIMESTAMP"].ToString() != null && rdr["RECORD_TIMESTAMP"].ToString() != "")
                        entity.RECORD_TIMESTAMP = Convert.ToDateTime(rdr["RECORD_TIMESTAMP"].ToString());
                    entity.AUDITABLE = rdr["AUDITABLE"].ToString();
                    entitiesList.Add(entity);
                }
            }
            con.Close();
            return entitiesList;

        }
        public List<AuditSubEntitiesModel> GetAuditSubEntities()
        {

            List<AuditSubEntitiesModel> subEntitiesList = new List<AuditSubEntitiesModel>();
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM  t_subentity se where se.STATUS = 'Y'";
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
            var enc_pass = getMd5Hash(user.PASSWORD);
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "UPDATE appservices.t_qar_users SET PASSWORD = '"+ enc_pass + "' WHERE USERID="+user.USER_ID+" and PPNO='"+user.PPNO+"'";
                cmd.ExecuteReader();
                if(user.ROLE_ID!=0)
                {
                    cmd.CommandText = "Select um.USERID FROM t_user_maping um WHERE um.ROLE_ID=" + user.ROLE_ID + " and um.USERID=" + user.USER_ID;
                    OracleDataReader rdr = cmd.ExecuteReader();
                    bool isAlreadyAdded = false;
                    while (rdr.Read())
                    {
                        if (rdr["USERID"].ToString() != null && rdr["USERID"].ToString() != "")
                            isAlreadyAdded = true;
                    }
                    if (!isAlreadyAdded)
                        cmd.CommandText = "INSERT INTO t_user_maping ( USERID,PPNO,GROUP_ID, ROLE_ID) VALUES ("+user.USER_ID+", '"+user.PPNO+"',"+user.ROLE_ID+", "+user.ROLE_ID+" )";
                    else
                        cmd.CommandText = "UPDATE t_user_maping SET GROUP_ID = " + user.ROLE_ID + ", ROLE_ID=" + user.ROLE_ID + " WHERE USERID=" + user.USER_ID + " and PPNO='" + user.PPNO + "'";
                    cmd.ExecuteReader();
                }
            }
            con.Close();
            user.PASSWORD = "";
            return user;
        }
        public void AddGroupMenuAssignment(int role_id = 0, int menu_id = 0, string page_ids = "")
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "Select mp.GROUP_MAP_ID FROM T_USER_GROUP_MAP mp WHERE mp.ROLE_ID=" + role_id + " and mp.MENU_ID=" + menu_id;
                OracleDataReader rdr = cmd.ExecuteReader();
                bool isAlreadyAdded = false;
                while (rdr.Read())
                {
                    if (rdr["GROUP_MAP_ID"].ToString() != null && rdr["GROUP_MAP_ID"].ToString() != "")
                        isAlreadyAdded = true;
                }
                if (!isAlreadyAdded)
                {
                    cmd.CommandText = "INSERT INTO T_USER_GROUP_MAP (GROUP_MAP_ID,ROLE_ID,MENU_ID,PAGE_IDS) VALUES ( (select COALESCE(max(p.GROUP_MAP_ID)+1,1) from T_USER_GROUP_MAP p)," + role_id + ", " + menu_id + ", '" + page_ids + "')";
                    cmd.ExecuteReader();
                }
            }
            con.Close();
        }
        public void RemoveGroupMenuAssignment(int role_id = 0, int menu_id = 0)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM T_USER_GROUP_MAP mp WHERE mp.ROLE_ID=" + role_id + " and mp.MENU_ID=" + menu_id;
                cmd.ExecuteReader();
            }
            con.Close();
        }
        public void AddGroupMenuItemsAssignment(int group_id=0, int menu_item_id=0)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "Select mp.GROUPMAP_ID FROM T_MENU_PAGES_GROUPMAP mp WHERE mp.GROUP_ID=" + group_id+ " and mp.PAGE_ID=" + menu_item_id;
                OracleDataReader rdr = cmd.ExecuteReader();
                bool isAlreadyAdded = false;
                while (rdr.Read())
                {
                    if (rdr["GROUPMAP_ID"].ToString() != null && rdr["GROUPMAP_ID"].ToString() != "")
                        isAlreadyAdded = true;
                }
                if (!isAlreadyAdded)
                {
                    cmd.CommandText = "INSERT INTO T_MENU_PAGES_GROUPMAP (GROUPMAP_ID,GROUP_ID,PAGE_ID) VALUES ( (select COALESCE(max(p.GROUPMAP_ID)+1,1) from T_MENU_PAGES_GROUPMAP p)," + group_id + ", " + menu_item_id + " )";
                    cmd.ExecuteReader();
                }
            }
            con.Close();
        }
        public void RemoveGroupMenuItemsAssignment(int group_id = 0, int menu_item_id = 0)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM  T_MENU_PAGES_GROUPMAP mp WHERE mp.GROUP_ID=" + group_id + " and mp.PAGE_ID=" + menu_item_id;
                cmd.ExecuteReader();
            }
            con.Close();
        }
        public List<AuditZoneModel> GetAuditZones()
        {
            var con = this.DatabaseConnection();
            List<AuditZoneModel> AZList = new List<AuditZoneModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "Select z.* FROM T_auditzone z order by z.ZONENAME asc";
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
                cmd.CommandText = "Select z.* FROM T_INS_UNITS z order by z.unit_name asc";
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
            con.Close();
            return ICList;
        }
        public List<BranchModel> GetBranches(int zone_code=0)
        {
            var con = this.DatabaseConnection();
            List<BranchModel> branchList = new List<BranchModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if(zone_code==0)
                    cmd.CommandText = "Select b.*, z.ZONENAME   FROM V_SERVICE_BRANCH b inner join V_SERVICE_ZONES z on b.zoneid=z.zoneid  order by b.BRANCHNAME asc";
                //cmd.CommandText = "Select b.*, s.DESCRIPTION as BRANCH_SIZE,  z.ZONENAME   FROM V_SERVICE_BRANCH b inner join T_BR_SIZE s on b.BRANCH_SIZE_ID=s.BR_SIZE_ID inner join V_SERVICE_ZONES z on b.zoneid=z.zoneid  order by b.BRANCHNAME asc";
                else
                    cmd.CommandText = "Select b.*,  z.ZONENAME   FROM V_SERVICE_BRANCH b inner join V_SERVICE_ZONES z on b.zoneid=z.zoneid WHERE z.ZONECODE=" + zone_code + " order by b.BRANCHNAME asc";
                //cmd.CommandText = "Select b.*, s.DESCRIPTION as BRANCH_SIZE,  z.ZONENAME   FROM V_SERVICE_BRANCH b inner join T_BR_SIZE s on b.BRANCH_SIZE_ID=s.BR_SIZE_ID inner join V_SERVICE_ZONES z on b.zoneid=z.zoneid WHERE z.ZONECODE="+zone_code+" order by b.BRANCHNAME asc";
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
            /*var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO V_SERVICE_BRANCH b (b.BRANCHID,b.BRANCHCODE, b.BRANCHNAME, b.ZONEID, b.ISACTIVE, b.BRANCH_SIZE_ID) VALUES ( '" + br.BRANCHCODE + "','" + br.BRANCHCODE + "','" + br.BRANCHNAME + "','" + br.ZONEID + "','" + br.ISACTIVE + "','" + br.BRANCH_SIZE_ID + "')";
                OracleDataReader rdr = cmd.ExecuteReader();

            }
            con.Close();
            return br;*/
        }
        public BranchModel UpdateBranch(BranchModel br)
        {
           /* var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "UPDATE V_SERVICE_BRANCH b SET b.BRANCHCODE='" + br.BRANCHCODE + "', b.BRANCHNAME='" + br.BRANCHNAME + "', b.ZONEID='" + br.ZONEID + "', b.BRANCH_SIZE_ID='" + br.BRANCH_SIZE_ID + "', b.ISACTIVE='" + br.ISACTIVE + "' WHERE b.BRANCHID=" + br.BRANCHID;
                OracleDataReader rdr = cmd.ExecuteReader();

            }
            con.Close();*/
            return br;
        }
        public List<ZoneModel> GetZones()
        {
            var con = this.DatabaseConnection();
            List<ZoneModel> zoneList = new List<ZoneModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "Select z.* FROM V_SERVICE_ZONES z order by z.ZONENAME asc";
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
            con.Close();
            return zoneList;
        }
        public List<BranchSizeModel> GetBranchSizes()
        {
            var con = this.DatabaseConnection();
            List<BranchSizeModel> brSizeList = new List<BranchSizeModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "Select bs.* FROM T_BR_SIZE bs order by bs.BR_SIZE_ID asc";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    BranchSizeModel bs = new BranchSizeModel();
                    bs.BR_SIZE_ID = Convert.ToInt32(rdr["BR_SIZE_ID"]);
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
                cmd.CommandText = "Select v.* FROM t_control_violation v order by v.ID asc";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ControlViolationsModel v = new ControlViolationsModel();
                    v.ID = Convert.ToInt32(rdr["ID"]);
                    v.V_NAME = rdr["V_NAME"].ToString();
                    v.MAX_NUMBER = Convert.ToInt32(rdr["MAX_NUMBER"]);
                    v.STATUS = rdr["STATUS"].ToString();
                    controlViolationList.Add(v);
                }
            }
            con.Close();
            return controlViolationList;
        }
        public ControlViolationsModel AddControlViolation(ControlViolationsModel cv)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if(cv.ID==0)
                cmd.CommandText = "INSERT INTO t_control_violation cv (cv.ID,cv.V_NAME, cv.MAX_NUMBER, cv.STATUS) VALUES ( (SELECT COALESCE(max(PP.ID)+1,1) FROM t_control_violation PP), '" + cv.V_NAME + "','" + cv.MAX_NUMBER+"','"+cv.STATUS+"')";
                else
                    cmd.CommandText = "UPDATE t_control_violation cv SET cv.V_NAME = '"+cv.V_NAME+"', cv.MAX_NUMBER='"+cv.MAX_NUMBER+"', cv.STATUS= '"+cv.STATUS+"' WHERE cv.ID="+cv.ID;
                cmd.ExecuteReader();
            }
            con.Close();
            return cv;
        }
        public List<DivisionModel> GetDivisions(bool sessionCheck=true)
        {
            var con = this.DatabaseConnection();
            List<DivisionModel> divList = new List<DivisionModel>();
            string query = "";
            /*if (sessionCheck)
            {
                var loggedInUser = sessionHandler.GetSessionUser();
                if (loggedInUser.UserPostingDiv != 0)
                    query = query + " and d.DIVISIONID=" + loggedInUser.UserPostingDiv;
            }*/
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select * from v_service_division d WHERE d.ISACTIVE='Y' "+ query + " order by d.CODE asc";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DivisionModel div = new DivisionModel();
                    div.DIVISIONID = Convert.ToInt32(rdr["DIVISIONID"]);
                    div.NAME = rdr["NAME"].ToString();
                    div.CODE = rdr["CODE"].ToString();
                    div.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    if (rdr["ISACTIVE"].ToString() == "Y")
                        div.ISACTIVE = "Active";
                    else if (rdr["ISACTIVE"].ToString() == "N")
                        div.ISACTIVE = "InActive";
                    else
                        div.ISACTIVE = rdr["ISACTIVE"].ToString();
                    divList.Add(div);
                }
            }
            con.Close();
            return divList;
        }
        public DivisionModel AddDivision(DivisionModel div)
        {
            /*
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO v_Service_Division d (d.ID,d.CODE, d.NAME, d.DESCRIPTION, d.STATUS) VALUES ( '" + div.CODE + "','" + div.CODE+"','"+div.NAME+"','"+div.DESCRIPTION+"','"+div.ISACTIVE+"')";
                OracleDataReader rdr = cmd.ExecuteReader();
               
            }
            con.Close();*/
            return div;
        }
        public DivisionModel UpdateDivision(DivisionModel div)
        {
            /*
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "UPDATE v_Service_Division d SET d.CODE='" + div.CODE + "', d.NAME='" + div.NAME + "', d.DESCRIPTION='" + div.DESCRIPTION + "', d.STATUS='" + div.ISACTIVE + "' WHERE d.ID="+div.DIVISIONID; 
                OracleDataReader rdr = cmd.ExecuteReader();

            }
            con.Close();*/
            return div;
        }       
        public List<DepartmentModel> GetDepartments(int div_code=0,bool sessionCheck=true)
        {
            var con = this.DatabaseConnection();
            List<DepartmentModel> deptList = new List<DepartmentModel>();
            var loggedInUser = sessionHandler.GetSessionUser();
            string query = "";
            /*if (sessionCheck)
            {
                if (loggedInUser.UserPostingDiv != 0)
                    query = query + " and d.DIVISIONID=" + loggedInUser.UserPostingDiv;
                if (loggedInUser.UserPostingDept != 0)
                    query = query + " and dp.ID=" + loggedInUser.UserPostingDept;
            }*/
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (div_code == 0)
                cmd.CommandText = "Select dp.*, d.NAME as DIV_NAME FROM v_service_department dp inner join v_service_division d on dp.DIVISIONID = d.DIVISIONID WHERE dp.ISACTIVE='Y' " + query + " order by dp.CODE asc";
                else
                    cmd.CommandText = "Select dp.*, d.NAME as DIV_NAME FROM v_service_department dp inner join v_service_division d on dp.DIVISIONID = d.DIVISIONID WHERE dp.DIVISIONID=" + div_code + " and dp.ISACTIVE='Y' " + query + " order by dp.CODE asc";
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
                    deptList.Add(dept);
                }
            }
            con.Close();
            return deptList;
        }
        public List<SubEntitiesModel> GetSubEntities(int div_code=0, int dept_code=0)
        {
            var con = this.DatabaseConnection();
            List<SubEntitiesModel> entitiesList = new List<SubEntitiesModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (div_code == 0)
                {
                    if (dept_code == 0)
                        cmd.CommandText = "Select s.*, d.NAME as DIV_NAME, dp.NAME as DEPT_NAME FROM T_SUBENTITY s inner join v_service_division d on s.DIV_ID = d.DIVISIONID inner join v_service_department dp on s.DEP_ID=dp.ID WHERE s.STATUS='Y' order by s.NAME asc";
                    else
                        cmd.CommandText = "Select s.*, d.NAME as DIV_NAME, dp.NAME as DEPT_NAME FROM T_SUBENTITY s inner join v_service_division d on s.DIV_ID = d.DIVISIONID inner join v_service_department dp on s.DEP_ID=dp.ID WHERE s.STATUS='Y' and dp.ID=" + dept_code+" order by s.NAME asc";

                }
                else {
                    if (dept_code == 0)
                        cmd.CommandText = "Select s.*, d.NAME as DIV_NAME, dp.NAME as DEPT_NAME FROM T_SUBENTITY s inner join v_service_division d on s.DIV_ID = d.DIVISIONID inner join v_service_department dp on s.DEP_ID=dp.ID WHERE d.DIVISIONID="+div_code+ " and s.STATUS='Y' order by s.NAME asc";
                    else
                        cmd.CommandText = "Select s.*, d.NAME as DIV_NAME, dp.NAME as DEPT_NAME FROM T_SUBENTITY s inner join v_service_division d on s.DIV_ID = d.DIVISIONID inner join v_service_department dp on s.DEP_ID=dp.ID WHERE d.DIVISIONID=" + div_code + " and s.STATUS='Y' and dp.ID=" + dept_code + " order by s.NAME asc";

                }

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
            con.Close();
            return entitiesList;
        }
        public SubEntitiesModel AddSubEntity(SubEntitiesModel subentity)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO T_SUBENTITY d (d.ID,d.NAME, d.DIV_ID, d.DEP_ID, d.STATUS) VALUES ( (SELECT COALESCE(max(PP.ID)+1,1) FROM T_SUBENTITY PP),'" + subentity.NAME + "','" + subentity.DIV_ID + "','" + subentity.DEP_ID + "','" + subentity.STATUS + "')";
                OracleDataReader rdr = cmd.ExecuteReader();

            }
            con.Close();
            return subentity;
        }
        public SubEntitiesModel UpdateSubEntity(SubEntitiesModel subentity)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "UPDATE T_SUBENTITY d SET d.NAME = '"+subentity.NAME+"' , d.DIV_ID='"+subentity.DIV_ID+"', d.DEP_ID='"+subentity.DEP_ID+"', d.STATUS='"+subentity.STATUS+"'  WHERE d.ID='"+subentity.ID+"'";
                OracleDataReader rdr = cmd.ExecuteReader();

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
                cmd.CommandText = "INSERT INTO v_Service_Department d (d.ID,d.CODE, d.NAME, d.DIV_ID, d.STATUS) VALUES ( '" + dept.CODE + "','" + dept.CODE + "','" + dept.NAME + "','" + dept.DIV_ID + "','" + dept.STATUS + "')";
                OracleDataReader rdr = cmd.ExecuteReader();

            }
            con.Close();*/
            return dept;
        }
        public DepartmentModel UpdateDepartment(DepartmentModel dept)
        {
            /*
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "UPDATE v_Service_Department d SET d.CODE='" + dept.CODE + "', d.NAME='" + dept.NAME + "', d.DIV_ID='" + dept.DIV_ID + "', d.STATUS='" + dept.STATUS + "' WHERE d.ID=" + dept.ID;
                OracleDataReader rdr = cmd.ExecuteReader();

            }
            con.Close();*/
            return dept;
        }
        public List<RiskGroupModel> GetRiskGroup()
        {
            var con = this.DatabaseConnection();
            List<RiskGroupModel> riskgroupList = new List<RiskGroupModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "Select rg.* FROM T_R_GROUP rg order by rg.GR_ID asc";
                OracleDataReader rdr = cmd.ExecuteReader();
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
                if(group_id==0)
                cmd.CommandText = "Select rsg.*, rg.DESCRIPTION as GROUP_DESC  FROM T_R_SUB_GROUP rsg inner join T_R_GROUP rg on rsg.GR_ID = rg.GR_ID order by rsg.S_GR_ID asc";
                else
                    cmd.CommandText = "Select rsg.*, rg.DESCRIPTION as GROUP_DESC  FROM T_R_SUB_GROUP rsg inner join T_R_GROUP rg on rsg.GR_ID = rg.GR_ID WHERE rsg.GR_ID ="+group_id+" order by rsg.S_GR_ID asc";


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
            con.Close();
            return risksubgroupList;
        }
        public List<RiskActivityModel> GetRiskActivities(int Sub_group_id)
        {
            var con = this.DatabaseConnection();
            List<RiskActivityModel> riskActivityList = new List<RiskActivityModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (Sub_group_id == 0) 
                    cmd.CommandText = "Select ra.*, rsg.DESCRIPTION as SUB_GROUP_DESC  FROM T_R_ACTIVITY ra inner join T_R_SUB_GROUP rsg on ra.S_GR_ID = rsg.S_GR_ID order by ra.ACTIVITY_ID asc";
                else
                    cmd.CommandText = "Select ra.*, rsg.DESCRIPTION as SUB_GROUP_DESC  FROM T_R_ACTIVITY ra inner join T_R_SUB_GROUP rsg on ra.S_GR_ID = rsg.S_GR_ID WHERE ra.S_GR_ID = "+Sub_group_id+" order by ra.ACTIVITY_ID asc";

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
        public List<AuditEmployeeModel> GetAuditEmployees(int dept_code=0)
        {
            var con = this.DatabaseConnection();
            List<AuditEmployeeModel> empList = new List<AuditEmployeeModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (dept_code == 0)
                    cmd.CommandText = "select e.* from t_audit_emp e order by e.RANKCODE asc";
                else
                    cmd.CommandText = "select e.* from t_audit_emp e WHERE e.DEPARTMENTCODE="+dept_code+" order by e.RANKCODE asc";

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
            con.Close();
            return empList;
        }
        public List<AuditTeamModel> GetAuditTeams(int dept_code = 0)
        {
            var loggedInUser = sessionHandler.GetSessionUser();
            var con = this.DatabaseConnection();
            List<AuditTeamModel> teamList = new List<AuditTeamModel>();
            string where_clause = "";
            string select_clause = "";
            if (loggedInUser.UserLocationType == "H")
            {
                where_clause = "inner join V_SERVICE_DEPARTMENT d on t.PLACE_OF_POSTING=d.ID WHERE t.PLACE_OF_POSTING = " + loggedInUser.UserPostingDept;
                select_clause = ", d.NAME as AUDIT_DEPARTMENT ";
            }
            else if (loggedInUser.UserLocationType == "B")
            { 
                where_clause = "inner join V_SERVICE_BRANCH b on t.PLACE_OF_POSTING=b.BRANCHID WHERE t.PLACE_OF_POSTING = " + loggedInUser.UserPostingBranch;
                select_clause = ", b.BRANCHNAME as AUDIT_DEPARTMENT ";
            }
            else if (loggedInUser.UserLocationType == "Z")
            {
                if (loggedInUser.UserPostingAuditZone != 0 && loggedInUser.UserPostingAuditZone != null)
                { 
                    where_clause = "left join V_SERVICE_AUDITZONE az on t.PLACE_OF_POSTING=az.ID WHERE t.PLACE_OF_POSTING = " + loggedInUser.UserPostingAuditZone;
                    select_clause = ", az.ZONENAME as AUDIT_DEPARTMENT ";
                }
                else
                {
                    where_clause = "left join V_SERVICE_ZONES z on t.PLACE_OF_POSTING=z.ZONEID WHERE t.PLACE_OF_POSTING = " + loggedInUser.UserPostingZone;
                    select_clause = ", z.ZONENAME as AUDIT_DEPARTMENT ";
                }
            }

            using (OracleCommand cmd = con.CreateCommand())
            {
                if (dept_code == 0)
                    // cmd.CommandText = "select t.*,tm.*, e.*, t.ID as TEAMID from t_ap_teamconstitute t inner join t_ap_team_members tm on t.id=tm.plan_id inner join t_audit_emp e on tm.teammember_id=e.ppno order by t.ID asc, tm.is_teamlead desc";
                    cmd.CommandText = "select t.* " + select_clause + " from t_au_team_members t " + where_clause + " order by t.T_CODE asc, t.ISTEAMLEAD desc";
                else
                    // cmd.CommandText = "select t.*,tm.*, e.*, t.ID as TEAMID from t_ap_teamconstitute t inner join t_ap_team_members tm on t.id=tm.plan_id inner join t_audit_emp e on tm.teammember_id=e.ppno WHERE t.AUDIT_DEPARTMENT=" + dept_code+ " order by t.ID asc, tm.is_teamlead desc";
                    cmd.CommandText = "select t.* "+select_clause+" from t_au_team_members t "+where_clause+" and t.PLACE_OF_POSTING="+dept_code+" order by t.T_CODE asc, t.ISTEAMLEAD desc";

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditTeamModel team = new AuditTeamModel();
                    team.ID = Convert.ToInt32(rdr["T_ID"]);
                    team.CODE = Convert.ToInt32(rdr["T_CODE"]);
                    team.NAME = rdr["TEAM_NAME"].ToString();
                    team.AUDIT_DEPARTMENT = Convert.ToInt32(rdr["PLACE_OF_POSTING"]);
                    team.TEAMMEMBER_ID = Convert.ToInt32(rdr["MEMBER_PPNO"]);
                    team.IS_TEAMLEAD = rdr["ISTEAMLEAD"].ToString();
                    team.PLACE_OF_POSTING= rdr["AUDIT_DEPARTMENT"].ToString();
                    team.EMPLOYEENAME = rdr["MEMBER_NAME"].ToString();
                    teamList.Add(team);
                }
            }
            con.Close();
            return teamList;
        }
        public List<AuditPeriodModel> GetAuditPeriods(int dept_code = 0)
        {
            var con = this.DatabaseConnection();
            List<AuditPeriodModel> periodList = new List<AuditPeriodModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (dept_code == 0)
                    cmd.CommandText = "select p.*, d.name as DEPARTMENT_NAME from T_AU_PERIOD p inner join V_SERVICE_DEPARTMENT d on p.AUDIT_CONDUCT_BY_DEPTID=d.ID order by p.ID asc";
                else
                    cmd.CommandText = "select p.*, d.name as DEPARTMENT_NAME from T_AU_PERIOD p inner join V_SERVICE_DEPARTMENT d on p.AUDIT_CONDUCT_BY_DEPTID=d.ID WHERE P.AUDIT_CONDUCT_BY_DEPTID = " + dept_code+" order by p.ID asc";

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AuditPeriodModel period = new AuditPeriodModel();
                    period.ID = Convert.ToInt32(rdr["ID"]);
                    period.AUDIT_CONDUCT_BY_DEPTID = Convert.ToInt32(rdr["AUDIT_CONDUCT_BY_DEPTID"]);
                    period.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    period.DEPARTMENT_NAME = rdr["DEPARTMENT_NAME"].ToString();
                    period.START_DATE = Convert.ToDateTime(rdr["START_DATE"]);
                    period.END_DATE = Convert.ToDateTime(rdr["END_DATE"]);
                    period.STATUS_ID = Convert.ToInt32(rdr["STATUS_ID"]);
                    periodList.Add(period);
                }
            }
            con.Close();
            return periodList;
        }
        public AuditPeriodModel AddAuditPeriod(AuditPeriodModel periodModel)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "insert into T_AU_PERIOD p (p.ID,p.DESCRIPTION,p.START_DATE,p.END_DATE,p.AUDIT_CONDUCT_BY_DEPTID,p.STATUS_ID) VALUES ( (SELECT COALESCE(max(PP.ID)+1,1) FROM T_AU_PERIOD PP),'" + periodModel.DESCRIPTION+ "', TO_DATE('"+periodModel.START_DATE+ "','dd/mm/yyyy HH:MI:SS AM'),TO_DATE('" + periodModel.END_DATE + "','dd/mm/yyyy HH:MI:SS AM')," + periodModel.AUDIT_CONDUCT_BY_DEPTID+", "+periodModel.STATUS_ID+")";
                OracleDataReader rdr = cmd.ExecuteReader();
            }
            con.Close();
            return periodModel;
        }
        public AuditPlanModel AddAuditPlan(AuditPlanModel plan)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "insert into T_AU_PLAN p (p.AUDITPERIOD_ID,p.AUDIT_CONDUCTBY_DEPT,p.NO_OF_DAYS_AUDIT,p.AUDITZONE_ID, p.BRANCH_ID, p.DIVISION_ID,p.DEPARTMENT_ID,p.RISK_LEVEL_ID,p.BRANCH_SIZE_ID,p.PLAN_STATUS_ID,p.SUB_ENTITY_ID) VALUES ( " + plan.AUDITPERIOD_ID + ","+plan.AUDIT_CONDUCTBY_DEPT + ", " + plan.NO_OF_DAYS_AUDIT + ", "+plan.AUDITZONE_ID + ","+plan.BRANCH_ID+","+plan.DIVISION_ID+","+plan.DEPARTMENT_ID+","+plan.RISK_LEVEL_ID+","+plan.BRANCH_SIZE_ID+","+plan.PLAN_STATUS_ID+ ", "+plan.SUB_ENTITY_ID+")";
                OracleDataReader rdr = cmd.ExecuteReader();
            }
            con.Close();
            return plan;
        }
        public List<AuditPlanModel> GetAuditPlan(int period_id = 0)
        {
            var con = this.DatabaseConnection();
            List<AuditPlanModel> planList = new List<AuditPlanModel>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (period_id == 0)
                    cmd.CommandText = "select p.*,d.Name as DIVISION_NAME,dp.Name as DEPARTMENT_NAME, b.branchname as BRANCH_NAME, az.ZONENAME as AUDITZONE_NAME from T_AU_PLAN p left join v_Service_Division d on p.division_id = d.DIVISIONID left join v_Service_Department dp on p.department_id = dp.ID left join V_SERVICE_BRANCH b on p.Branch_Id = b.BRANCHID left join V_SERVICE_ZONES az on p.auditzone_id = az.ZONEID order by p.PLAN_ID asc";
                else
                    cmd.CommandText = "select p.*,d.Name as DIVISION_NAME,dp.Name as DEPARTMENT_NAME, b.branchname as BRANCH_NAME, az.ZONENAME as AUDITZONE_NAME from T_AU_PLAN p left join v_Service_Division d on p.division_id = d.DIVISIONID left join v_Service_Department dp on p.department_id = dp.ID left join V_SERVICE_BRANCH b on p.Branch_Id = b.BRANCHID left join V_SERVICE_ZONES az on p.auditzone_id = az.ZONEID where p.auditperiod_id=" + period_id+ " order by p.PLAN_ID asc";

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
              cmd.CommandText = "select * from t_r_m_process pd order by pd.P_ID";

                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    RiskProcessDefinition proc = new RiskProcessDefinition();
                    proc.P_ID = Convert.ToInt32(rdr["P_ID"]);
                    if (rdr["RISK_ID"].ToString() != null && rdr["RISK_ID"].ToString() != "")
                        proc.RISK_ID = Convert.ToInt32(rdr["RISK_ID"]);
                    proc.P_NAME = rdr["P_NAME"].ToString();
                    pdetails.Add(proc);
                }
            }
            con.Close();
            return pdetails;
        }
        public List<RiskProcessDetails> GetRiskProcessDetails(int procId=0)
        {
            var con = this.DatabaseConnection();
            List<RiskProcessDetails> riskProcList = new List<RiskProcessDetails>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if(procId==0)
                    cmd.CommandText = "select * from t_r_m_process_sub pd order by pd.p_id asc";
                else
                    cmd.CommandText = "select * from t_r_m_process_sub pd where pd.p_id = "+procId+" order by pd.P_ID,pd.ID asc";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    RiskProcessDetails pdetail = new RiskProcessDetails();
                    pdetail.ID = Convert.ToInt32(rdr["ID"]);
                    pdetail.P_ID = Convert.ToInt32(rdr["P_ID"]);
                    pdetail.TITLE = rdr["TITLE"].ToString();
                    pdetail.ACTIVE = rdr["ACTIVE"].ToString();
                    riskProcList.Add(pdetail);
                }
            }
            con.Close();
            return riskProcList;
        }
        public List<RiskProcessTransactions> GetRiskProcessTransactions(int procDetailId = 0, int transactionId=0)
        {
            var con = this.DatabaseConnection();
            List<RiskProcessTransactions> riskTransList = new List<RiskProcessTransactions>();
            using (OracleCommand cmd = con.CreateCommand())
            {
                if (procDetailId == 0)
                {
                    if(transactionId==0)
                    cmd.CommandText = "select pt.*,pd.TITLE, p.P_NAME, vc.V_NAME from t_r_m_process_transaction pt inner join t_r_m_process_sub pd on pt.PD_ID=pd.ID inner join t_r_m_process p on pd.P_ID = p.P_ID inner join t_control_violation vc on vc.ID=pt.V_ID  order by pt.id asc"; 
                    else
                        cmd.CommandText = "select pt.*,pd.TITLE, p.P_NAME, vc.V_NAME from t_r_m_process_transaction pt inner join t_r_m_process_sub pd on pt.PD_ID=pd.ID inner join t_r_m_process p on pd.P_ID = p.P_ID inner join t_control_violation vc on vc.ID=pt.V_ID WHERE pt.ID=" + transactionId + " order by pt.id asc";
                }
                else
                {
                    if (transactionId == 0) 
                        cmd.CommandText = "select pt.*,pd.TITLE, p.P_NAME, vc.V_NAME from  t_r_m_process_transaction pt inner join t_r_m_process_sub pd on pt.PD_ID=pd.ID inner join t_r_m_process p on pd.P_ID = p.P_ID inner join t_control_violation vc on vc.ID=pt.V_ID where pt.pd_id = " + procDetailId + " order by pt.Id asc";
                    else
                        cmd.CommandText = "select pt.*,pd.TITLE, p.P_NAME, vc.V_NAME from t_r_m_process_transaction pt inner join t_r_m_process_sub pd on pt.PD_ID=pd.ID inner join t_r_m_process p on pd.P_ID = p.P_ID inner join t_control_violation vc on vc.ID=pt.V_ID where pt.ID=" + transactionId+" pt.pd_id = " + procDetailId + " order by pt.Id asc";

                }
                    OracleDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        RiskProcessTransactions pTran = new RiskProcessTransactions();
                        pTran.ID = Convert.ToInt32(rdr["ID"]);
                        pTran.PD_ID = Convert.ToInt32(rdr["PD_ID"]);
                        pTran.V_ID = Convert.ToInt32(rdr["V_ID"]);
                    if (rdr["DIV_ID"].ToString() != null && rdr["DIV_ID"].ToString() != "")
                            pTran.DIV_ID = Convert.ToInt32(rdr["DIV_ID"]);
                        if (rdr["DIV_NAME"].ToString() != null && rdr["DIV_NAME"].ToString() != "")
                            pTran.DIV_NAME = rdr["DIV_NAME"].ToString();
                        if (rdr["DESCRIPTION"].ToString() != null && rdr["DESCRIPTION"].ToString() != "")
                            pTran.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                        if (rdr["CONTROL_OWNER"].ToString() != null && rdr["CONTROL_OWNER"].ToString() != "")
                            pTran.CONTROL_OWNER = rdr["CONTROL_OWNER"].ToString();
                        if (rdr["RISK_MAX_NUMBER"].ToString() != null && rdr["RISK_MAX_NUMBER"].ToString() != "")
                            pTran.RISK_MAX_NUMBER = Convert.ToInt32(rdr["RISK_MAX_NUMBER"]);
                        pTran.ACTION = rdr["ACTION"].ToString();
                    pTran.RISK_WEIGHTAGE = Convert.ToInt32(rdr["RISK_WEIGHTAGE"]);
                    pTran.SUB_PROCESS_NAME = rdr["TITLE"].ToString();
                    pTran.PROCESS_NAME = rdr["P_NAME"].ToString();
                    pTran.VIOLATION_NAME = rdr["V_NAME"].ToString();
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
                if (statusId.Length == 0)
                cmd.CommandText = "select pt.*, pd.TITLE, p.P_NAME, s.status from t_r_m_process_transaction pt inner join t_r_m_process_sub pd on pt.PD_ID = pd.ID inner join t_r_m_process p on pd.ID = p.P_ID inner join t_r_m_process_transaction_status_mapping sm on pt.id = sm.T_ID inner join t_r_m_process_transaction_status s on s.ID = sm.STATUS_ID order by pt.id asc";
                else
                    cmd.CommandText = "select pt.*, pd.TITLE, p.P_NAME, s.status from t_r_m_process_transaction pt inner join t_r_m_process_sub pd on pt.PD_ID = pd.ID inner join t_r_m_process p on pd.ID = p.P_ID inner join t_r_m_process_transaction_status_mapping sm on pt.id = sm.T_ID inner join t_r_m_process_transaction_status s on s.ID = sm.STATUS_ID and s.ID IN (" + string.Join(",", statusId) + ") order by pt.id asc";


                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    RiskProcessTransactions pTran = new RiskProcessTransactions();
                    pTran.ID = Convert.ToInt32(rdr["ID"]);
                    pTran.PD_ID = Convert.ToInt32(rdr["PD_ID"]);
                    if (rdr["DIV_ID"].ToString() != null && rdr["DIV_ID"].ToString() != "")
                        pTran.DIV_ID = Convert.ToInt32(rdr["DIV_ID"]);
                    if (rdr["DIV_NAME"].ToString() != null && rdr["DIV_NAME"].ToString() != "")
                        pTran.DIV_NAME = rdr["DIV_NAME"].ToString();
                    if (rdr["DESCRIPTION"].ToString() != null && rdr["DESCRIPTION"].ToString() != "")
                        pTran.DESCRIPTION = rdr["DESCRIPTION"].ToString();
                    if (rdr["CONTROL_OWNER"].ToString() != null && rdr["CONTROL_OWNER"].ToString() != "")
                        pTran.CONTROL_OWNER = rdr["CONTROL_OWNER"].ToString();
                    if (rdr["RISK_MAX_NUMBER"].ToString() != null && rdr["RISK_MAX_NUMBER"].ToString() != "")
                        pTran.RISK_MAX_NUMBER = Convert.ToInt32(rdr["RISK_MAX_NUMBER"]);
                    pTran.ACTION = rdr["ACTION"].ToString();
                    pTran.RISK_WEIGHTAGE = Convert.ToInt32(rdr["RISK_WEIGHTAGE"]);
                    pTran.SUB_PROCESS_NAME = rdr["TITLE"].ToString();
                    pTran.PROCESS_NAME = rdr["P_NAME"].ToString();
                    pTran.PROCESS_STATUS = rdr["STATUS"].ToString();
                    riskTransList.Add(pTran);
                }
            }
            con.Close();
            return riskTransList;
        }
        public RiskProcessTransactions GetRiskProcessTransactionLastStatus(RiskProcessTransactions tr)
        {
            var con = this.DatabaseConnection();
           using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select comments from t_r_m_process_transaction_log l where l.t_id="+ tr.ID+ " order by l.created_on desc FETCH NEXT 1 ROWS ONLY";
                OracleDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    tr.PROCESS_COMMENTS = rdr["comments"].ToString();
                }
            }
            con.Close();
            return tr;
        }
        public RiskProcessDefinition AddRiskProcess(RiskProcessDefinition proc)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "insert into t_r_m_process p (p.P_ID,p.P_NAME,p.RISK_ID) VALUES ( (select COALESCE(max(pp.P_ID)+1,1) from t_r_m_process pp),'" + proc.P_NAME + "','"+proc.RISK_ID+"')";
                OracleDataReader rdr = cmd.ExecuteReader();
            }
            con.Close();
            return proc;
        }
        public RiskProcessDetails AddRiskSubProcess(RiskProcessDetails subProc)
        {
            var con = this.DatabaseConnection();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "insert into t_r_m_process_sub p (p.ID,p.P_ID,p.TITLE,p.ACTIVE) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_r_m_process_sub pp)," + subProc.P_ID + ",'" + subProc.TITLE + "','Y')";
                OracleDataReader rdr = cmd.ExecuteReader();
            }
            con.Close();
            return subProc;
        }
        public RiskProcessTransactions AddRiskSubProcessTransaction(RiskProcessTransactions trans)
        {
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "insert into t_r_m_process_transaction p (p.ID,p.V_ID, p.PD_ID,p.DESCRIPTION,p.CONTROL_OWNER,p.DIV_ID,p.DIV_NAME,p.ACTION,p.RISK_WEIGHTAGE,p.RISK_MAX_NUMBER) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_r_m_process_transaction pp)," + trans.V_ID + "," + trans.PD_ID + ",'" + trans.DESCRIPTION + "','" + trans.CONTROL_OWNER + "','" + trans.DIV_ID + "','" + trans.DIV_NAME + "','" + trans.ACTION + "','" + trans.RISK_WEIGHTAGE + "','" + trans.RISK_MAX_NUMBER + "')";
                OracleDataReader rdr = cmd.ExecuteReader();
                cmd.CommandText = "insert into t_r_m_process_transaction_log p (p.ID,p.T_ID,p.STATUS_ID,p.USER_ID,p.COMMENTS) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_r_m_process_transaction_log pp), (select max(tp.ID) from t_r_m_process_transaction tp) ,'1',"+ loggedInUser.ID+ ",'New Transaction Added')";
                cmd.ExecuteReader();
                cmd.CommandText = "insert into t_r_m_process_transaction_status_mapping p (p.ID,p.T_ID,p.STATUS_ID) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_r_m_process_transaction_status_mapping pp), (select max(tp.ID) from t_r_m_process_transaction tp) ,'1')";
                cmd.ExecuteReader();

            }
            con.Close();
            return trans;
        }
        public bool RecommendProcessTransactionByReviewer(int T_ID, string COMMENTS)
        {
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = " Update t_r_m_process_transaction_status_mapping tm SET tm.STATUS_ID = 3 WHERE tm.T_ID=" + T_ID;
                OracleDataReader rdr = cmd.ExecuteReader();
                cmd.CommandText = "insert into t_r_m_process_transaction_log p (p.ID,p.T_ID,p.STATUS_ID,p.USER_ID,p.COMMENTS) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_r_m_process_transaction_log pp), "+T_ID+" ,'3'," + loggedInUser.ID + ",'"+COMMENTS+"')";
                cmd.ExecuteReader();
            }
            con.Close();
            return true;
        }
        public bool RefferedBackProcessTransactionByReviewer(int T_ID, string COMMENTS)
        {
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = " Update t_r_m_process_transaction_status_mapping tm SET tm.STATUS_ID = 2 WHERE tm.T_ID=" + T_ID;
                OracleDataReader rdr = cmd.ExecuteReader();
                cmd.CommandText = "insert into t_r_m_process_transaction_log p (p.ID,p.T_ID,p.STATUS_ID,p.USER_ID,p.COMMENTS) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_r_m_process_transaction_log pp), " + T_ID + " ,'2'," + loggedInUser.ID + ",'" + COMMENTS + "')";
                cmd.ExecuteReader();
            }
            con.Close();
            return true;
        }
        public bool RecommendProcessTransactionByAuthorizer(int T_ID, string COMMENTS)
        {
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = " Update t_r_m_process_transaction_status_mapping tm SET tm.STATUS_ID = 5 WHERE tm.T_ID=" + T_ID;
                OracleDataReader rdr = cmd.ExecuteReader();
                cmd.CommandText = "insert into t_r_m_process_transaction_log p (p.ID,p.T_ID,p.STATUS_ID,p.USER_ID,p.COMMENTS) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_r_m_process_transaction_log pp), " + T_ID + " ,'5'," + loggedInUser.ID + ",'" + COMMENTS + "')";
                cmd.ExecuteReader();
            }
            con.Close();
            return true;
        }
        public bool RefferedBackProcessTransactionByAuthorizer(int T_ID, string COMMENTS)
        {
            var con = this.DatabaseConnection();
            var loggedInUser = sessionHandler.GetSessionUser();
            using (OracleCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = " Update t_r_m_process_transaction_status_mapping tm SET tm.STATUS_ID = 4 WHERE tm.T_ID=" + T_ID;
                OracleDataReader rdr = cmd.ExecuteReader();
                cmd.CommandText = "insert into t_r_m_process_transaction_log p (p.ID,p.T_ID,p.STATUS_ID,p.USER_ID,p.COMMENTS) VALUES ( (select COALESCE(max(pp.ID)+1,1) from t_r_m_process_transaction_log pp), " + T_ID + " ,'4'," + loggedInUser.ID + ",'" + COMMENTS + "')";
                cmd.ExecuteReader();
            }
            con.Close();
            return true;
        }
    }
}

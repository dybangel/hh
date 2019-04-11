using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class api : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string getstr = HttpContext.Current.Request.Url.Query;
         getstr = getstr.Replace("?", "");
        int start = getstr.LastIndexOf("&");
        int stop = getstr.Length - start;
         string where = getstr.Substring(0,start);
       // Response.Write("<script>console.log('where=" + where + "')</script>");
        string procedure = getstr.Substring(start,stop).Replace("&action=","").ToString();

        string action = Request["action"];
        string mysql = "";
        if ("getcount" == action)
        {
            mysql = "SELECT COUNT(*) AS wcount FROM Fooke_Udid";
        }
        else if ("adddata_fromudid" == action)
        {
            string datas = Request["datas"];
            mysql = "INSERT INTO Fooke_Udid(udid)VALUES('" + datas + "')";
        }
        else if ("adddata_fromuserid" == action)
        {
            string datas = Request["datas"];
            mysql = "INSERT INTO Fooke_Udid (udid) SELECT DeviceIdentifier FROM Fooke_User WHERE(UserID = '" + datas + "')";
        }
        else if ("devicechange" == action) {
            string olduserid = Request["olduserid"];
            string newudid = Request["newudid"];
            mysql = "select userid,strTokey from fooke_user where userid='"+ olduserid + "'";
            DataTable dt = new DataTable();
            dt=executesql(mysql);          
            string oldstrtokey=dt.Rows[0]["strtokey"].ToString();
            string jsonString = string.Empty;
            dt.Clear();

            mysql = "select userid,strTokey from fooke_user where DeviceIdentifier='" + newudid + "'";
          
            dt.Clear();
            dt = executesql(mysql);
            if (0 == dt.Rows.Count) {
                returnjson("status", "该UDID不存在，1、请确认新设备已经安装海虹 2、确认在新设备上联网打开过海虹 3、确认UDID粘贴无误");
                Response.End();
            }
           

            string newuserid= dt.Rows[0]["userid"].ToString();
            string newstrtokey = dt.Rows[0]["strtokey"].ToString();

            mysql = "update fooke_user set strtokey='" + newstrtokey + "' where userid='"+olduserid+"'";
            executesql(mysql);
          

            mysql = "update fooke_user set strtokey='" + oldstrtokey + "' where userid='" + newuserid + "'";
            executesql(mysql);
            returnjson("status","ok");
            dt.Dispose();
            Response.End();
        }
        else
        {
           mysql = "exec [" + procedure + "] " + HttpUtility.UrlDecode(where);
           
        }
        SqlConnection MyConnection = new SqlConnection("data source=localhost;initial catalog=HaiHong;password=hh123456;persist s" +
"ecurity info=True;user id=sa;workstation id=BAIHAO;packet size=4096");

        try
        {
            SqlCommand MyDataSetCommand = new SqlCommand(mysql, MyConnection);

            DataSet ds = new DataSet();
            SqlDataAdapter ada = new SqlDataAdapter(MyDataSetCommand);
            DataTable dt = new DataTable();
            ada.Fill(dt);
            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dt);

            Response.Write(jsonString);
            MyConnection.Close();
            dt.Dispose();
            ds.Dispose();
            ada.Dispose();
        }
        catch
        {
            returnjson("status", "error");
        }
        finally {
        
        }

    }
    protected void returnjson(string tag,string str) {
        Hashtable hash1 = new Hashtable();
        hash1.Add(tag, str);
        List<Hashtable> L_hash = new List<Hashtable>();
        L_hash.Add(hash1);
        Response.Write(JsonConvert.SerializeObject(L_hash));
    }
    protected DataTable executesql(string sql,string tag="") {

        SqlConnection MyConnectiondc = new SqlConnection("data source=localhost;initial catalog=HaiHong;password=hh123456;persist s" +
"ecurity info=True;user id=sa;workstation id=BAIHAO;packet size=4096");
        DataTable dt = new DataTable();
        try
        {
            SqlCommand MyDataSetCommand = new SqlCommand(sql, MyConnectiondc);
            DataSet ds = new DataSet();
            SqlDataAdapter ada = new SqlDataAdapter(MyDataSetCommand);
            ada.Fill(dt);
            if ("andwrite" == tag)
            {
                string jsonString = string.Empty;
                jsonString = JsonConvert.SerializeObject(dt);
                Response.Write(jsonString);
            }
            return dt;
        }
        catch
        {
            returnjson("status", "error");
            return dt;

        }
        finally {
           
        }


    }
}

//  Response.Write("<script>console.log('kv=" + getstr + "')</script>");
// Response.Write("<script>console.log('kvL=" + getstr.Length.ToString() + "')</script>");
// Response.Write("<script>console.log('lastindex=" + Convert.ToString(getstr.LastIndexOf("&")) + "')</script>");

// Response.Write("<script>console.log('procedure=" + procedure + "')</script>");
//  Convert.ToString(getstr.LastIndexOf("&"));


//   Response.End();

//  getstr = getstr.Replace("&action=procedure","");
// string[] split = getstr.Split('&');//?action=d&user=dyb
// foreach (string s in split)
//  {
//   Response.Write("<script>console.log('kv=" + getstr + "')</script>");
// Console.Write(a + ",");
//    }

// Response.Write("<script>console.log('getstr="+getstr+"')</script>");
// Response.End();










//   Response.Write("<script>console.log('action is:"+action+"')</script>");

/* DataTable dt = SqlHelper.ExecuteDataTable("select * from T_User where id=@id", new SqlParameter("id", id));



 Response.End();*/

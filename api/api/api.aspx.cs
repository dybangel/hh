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
        getstr = getstr.Replace("&appid=pPyZWvH3Fa6PXba10aJ009", "");//hbulider的
        getstr = getstr.Replace("&appid=FWEFASDFSFA","");//我的
        int start = getstr.LastIndexOf("&");
        int stop = getstr.Length - start;
         string where = getstr.Substring(0,start);
       // Response.Write("<script>console.log('where=" + where + "')</script>");
        string procedure = getstr.Substring(start,stop).Replace("&action=", "").ToString();

        string action = Request["action"];
        string mysql = "";
        //获取已经添加的设备数量
        if ("getcount" == action)
        {
            mysql = "SELECT COUNT(*) AS wcount FROM Fooke_Udid";
        }
        //通过UDID添加白名单
        else if ("adddata_fromudid" == action)
        {
            string datas = Request["datas"];

            //判断是否有重复记录，如果有则退出
            mysql = "SELECT COUNT(udid) AS numcount FROM Fooke_Udid WHERE (udid = '" + datas + "')";
            DataTable dt = executesql(mysql);
            if (0 < Convert.ToInt32(dt.Rows[0]["numcount"]))
            {
                returnjson("status", "UDID已经存在");
                Response.End();
            }


            mysql = "INSERT INTO Fooke_Udid(udid)VALUES('" + datas + "')";
            executesql(mysql);
            returnjson("status", "ok");
            Response.End();
        }
        //通过userid添加白名单
        else if ("adddata_fromuserid" == action)
        {
            string datas = Request["datas"];
            //判断userid是否存在
            mysql = "select count(udid) as numcount from Fooke_User where (userid='" + datas + "')";
            DataTable dt = executesql(mysql);
            if (0 == Convert.ToInt32(dt.Rows[0]["numcount"]))
            {
                returnjson("status", "邀请码存在");
                Response.End();
            }

            //判断userid对应的udid是否有重复

            mysql = "INSERT INTO Fooke_Udid (udid) SELECT DeviceIdentifier FROM Fooke_User WHERE(UserID = '" + datas + "')";
            executesql(mysql);
            returnjson("status", "ok");
            Response.End();
        }
        //更换设备
        else if ("devicechange" == action)
        {
            string olduserid = Request["olduserid"];
            string newudid = Request["newudid"];
            mysql = "select userid,strTokey from fooke_user where userid='" + olduserid + "'";
            DataTable dt = new DataTable();
            dt = executesql(mysql);
            string oldstrtokey = dt.Rows[0]["strtokey"].ToString();
            string jsonString = string.Empty;
            dt.Clear();

            mysql = "select userid,strTokey from fooke_user where DeviceIdentifier='" + newudid + "'";

            dt.Clear();
            dt = executesql(mysql);
            if (0 == dt.Rows.Count)
            {
                returnjson("status", "该UDID不存在，1、请确认新设备已经安装海虹 2、确认在新设备上联网打开过海虹 3、确认UDID粘贴无误");
                Response.End();
            }


            string newuserid = dt.Rows[0]["userid"].ToString();
            string newstrtokey = dt.Rows[0]["strtokey"].ToString();

            mysql = "update fooke_user set strtokey='" + newstrtokey + "' where userid='" + olduserid + "'";
            executesql(mysql);


            mysql = "update fooke_user set strtokey='" + oldstrtokey + "' where userid='" + newuserid + "'";
            executesql(mysql);
            returnjson("status", "ok");
            dt.Dispose();
            Response.End();
        }
        //自定义sql语句
        else if (action.IndexOf("sql_") >= 0)
        {

            mysql = Request["sql"];
            //  returnjson("status", mysql);
            //  Response.End();
            // returnjson("status", mysql);
            string jsonString = string.Empty;

            jsonString = JsonConvert.SerializeObject(executesql(mysql));
            Response.Write(jsonString);

            //  returnjson("status", "this is sql");
            Response.End();
            // executesql();
        }
        else if ("appupdate" == action)
        {
            Hashtable hash1 = new Hashtable();
            hash1.Add("status", "1.2.4");
            hash1.Add("mode", "apk");
            List<Hashtable> L_hash = new List<Hashtable>();
            L_hash.Add(hash1);
            Response.Write(JsonConvert.SerializeObject(L_hash));
            //returnjson("status", "1.1.6");
            Response.End();
        }
        else if ("ticheng" == action)
        {
            string amount = Request["amount"];
            string userid = Request["userid"];
            string username = Request["username"];
            var sql = "exec [Stored_FindUserParent] @NodeID=N'" + userid + "'";
            DataTable dt = new DataTable();
            dt = executesql(sql);
            string tuserid = "";
            string tparentid = "";
            string tnickname = "";
            string tNodeLevel = "";
            string tempstr = "";
            float ticheng = (float)(Convert.ToDouble(amount) * 0.02f);
            ticheng=(float)(Math.Round(Convert.ToDecimal(ticheng), 2, MidpointRounding.AwayFromZero));
            //returnjson("status", dt.Rows.Count.ToString());
            //    Response.End();
            for (int i = 1; i <= dt.Rows.Count - 1; i++)
            {
                tuserid = dt.Rows[i]["userid"].ToString();
                tparentid = dt.Rows[i]["parentid"].ToString();
                tnickname = dt.Rows[i]["nickname"].ToString();
                tNodeLevel = dt.Rows[i]["NodeLevel"].ToString();
                // tempstr = tempstr + "userid:" + tuserid + " parentid:" + tparentid + " nickname:" + tnickname + " NodeLevel:" + tNodeLevel;
                sql = "exec [Stored_SaveAmount] @strKey=N'" + getRandrom(14) + GetTimeStamp() + "',@UserID=N'" + tuserid + "',@Nickname=N'" + tnickname + "',@FormID=N'" + userid + "',@Formname=N'" + username + "',@Affairs=N'1',@Mode=N'任务提成',@Amount=N'" + ticheng.ToString() + "',@Remark=N'下级好友完成任务获得奖励提成" + ticheng.ToString() + "元'";
                executesql(sql);
            }

            returnjson("status", "ok");
            Response.End();

        }
        else if ("jicha" == action)
        {
            
            string amount = Request["amount"];
            string userid = Request["userid"];
            string username = Request["username"];
            var sql = "exec [Stored_FindUserParent] @NodeID=N'" + userid + "'";

            DataTable dt = new DataTable();
            //得到师傅结果集
            dt = executesql(sql);
          
              dt.Columns.Add("umbrella", typeof(string));
              dt.Columns.Add("jichaticheng", typeof(string));
            dt.Columns.Add("jichatichengamount", typeof(string));
            //循环计算出每一个师傅伞下成员数量
            DataTable dtdt = new DataTable();
            //循环前声明提成能量槽为4%，级别为0.00
            float GC = 0.04f;
            float GL = 0.00f;
            for (int i = 1; i <= dt.Rows.Count-1; i++) {
                string dtuserid = dt.Rows[i]["userid"].ToString();
                string dtsql = " WITH CTE AS (";
                dtsql += " SELECT UserID, ParentID FROM Fooke_User WHERE(UserID = "+dtuserid+")";
                dtsql += " UNION ALL";
                dtsql += " SELECT a.UserID, a.ParentID FROM Fooke_User AS a";
                dtsql += " INNER JOIN CTE AS b ON b.userID = a.ParentID)";

                dtsql += " SELECT count(*)as umbrellacount FROM Fooke_User AS a";
                dtsql += " INNER JOIN CTE AS t ON a.UserID = t.UserID";
               dtdt=executesql(dtsql);
                dt.Rows[i]["umbrella"] = dtdt.Rows[0]["umbrellacount"];
                //根据这个用户伞下的会员数计算它应该得到的提成比例
                float levelpercent=getlevelpercent(Convert.ToInt32(dt.Rows[i]["umbrella"]));
                //如果GC-levelpercent >=0,并且levelpercent〉GL说明还可以提成
                decimal cha = Convert.ToDecimal(GC) - Convert.ToDecimal(levelpercent);
                // decimal level = Convert.ToDecimal() - Convert.ToDecimal();
                if (cha >= 0 && Convert.ToDecimal(levelpercent) > Convert.ToDecimal(GL))
                {
                    decimal jichaticheng = Convert.ToDecimal(amount) * Convert.ToDecimal(levelpercent);
                     jichaticheng=(Math.Round(Convert.ToDecimal(jichaticheng), 2, MidpointRounding.AwayFromZero));
                    dt.Rows[i]["jichaticheng"] = levelpercent.ToString();
                    dt.Rows[i]["jichatichengamount"] = jichaticheng;
                    GC = (float)(cha);
                    GL = levelpercent;
                    //更新GC 更新GL
                }
                else {
                    dt.Rows[i]["jichaticheng"] = "0.00";
                }
        
                dtdt.Clear();
            }


            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(dt);
            Response.Write(jsonString);
            Response.End();
        }
        else if ("checkreg" == action) {
            string sn = Request["sn"];
            string uuid = Request["uuid"];
           // returnjson("status", "激活码异常(000)");
            //Response.End();
            //看sn存在否
        string sql = "SELECT TOP (1) fid, fsn, fuuid FROM Fooke_licence WHERE(fsn = '"+sn+"')";
            DataTable dt = new DataTable();
            dt = executesql(sql);
            if (dt.Rows.Count == 0)
            {
                returnjson("status", "激活码异常(000)");
                Response.End();
            } 
            else {
                //看激活码是否被使用过
                if (dt.Rows[0]["fuuid"] == "NULL" || dt.Rows[0]["fuuid"] == "")
                {
                    //激活码没有被使用过
                    sql = "update Fooke_licence set fuuid=" + uuid + " WHERE(fsn = '" + sn + "')";
                    executesql(sql);
                    returnjson("status", "激活成功");
                    Response.End();
                }
                else {
                    //激活码被使用过
                    if (dt.Rows[0]["fuuid"].ToString() == uuid)
                    {
                        //说明是本人
                        returnjson("status", "序列号与UUID匹配");
                        Response.End();
                    }
                    else {
                        //说明不是本人
                        returnjson("status", "不是本人" + "dt uuid is:" + dt.Rows[0]["fuuid"].ToString() + "post uuid is:" + uuid);
                        Response.End();
                    }

                }

            }   
        }
        else //执行存储过程
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
            if (0==dt.Rows.Count) {
               
                returnjson("status","NULL");
                
            }
            else {
             
                jsonString = JsonConvert.SerializeObject(dt);
                Response.Write(jsonString);
            }
            
            

           
            MyConnection.Close();
            dt.Dispose();
            ds.Dispose();
            ada.Dispose();
        }
        catch
        {
            returnjson("status", "error111");
        }
        finally {
        
        }

    }
    //随机数
 protected string getRandrom(int num)
    {

        string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";


        Random randrom = new Random((int)DateTime.Now.Ticks);

        string str = "";
        for (int i = 0; i < num; i++)
        {
            str += chars[randrom.Next(chars.Length)];
        }

        return str;

}


    public static string GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    //向前台打印json
    protected void returnjson(string tag,string str) {
        Hashtable hash1 = new Hashtable();
        hash1.Add(tag, str);
        List<Hashtable> L_hash = new List<Hashtable>();
        L_hash.Add(hash1);
        Response.Write(JsonConvert.SerializeObject(L_hash));
    }
    //根据伞下人数判断级别
    protected float getlevelpercent(int umbrellacount) {
        float levelpercent = 0f;
        if (umbrellacount >= 200 && umbrellacount < 500)
        {
            return 0.01f;//铜
        }
        else if (umbrellacount >= 500 && umbrellacount < 1000) {
            return 0.02f;//银
        }
        else if (umbrellacount >= 1000 && umbrellacount < 2000)
        {
            return 0.03f;//金
        }
        else if (umbrellacount >= 2000 )
        {
            return 0.04f;//钻石
        }
        return levelpercent;
    }
    //执行sql返回dt
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

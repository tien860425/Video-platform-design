using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using log4net;
using log4net.Config;
using Newtonsoft.Json;
using System.Threading;
using System.Messaging;
using System.IO;
namespace WebProject
{



    /// <summary>
    /// dprocess 的摘要描述
    /// </summary>
    public class dprocess : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private static readonly ILog log = LogManager.GetLogger("WebLog");
        String connStr;
        SqlConnection conn;
        SqlCommand cmmd;
        SqlDataAdapter da;
        SqlDataReader dr;
        DataSet ds;

        public void ProcessRequest(HttpContext context)
        {
            if (context.Session["Connectionstr"] == null)
            {
                connStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                context.Session["Connectionstr"] = connStr;
            }
            else
            {
                connStr = context.Session["Connectionstr"].ToString();
            }
            string qid = context.Request.Form["qid"];
            context.Response.ContentType = "text/plain";
            string usrname;
            string pwd;
            string email;
            string contact;
            string sqlstr;
            string Account;
            try
            {
                switch (qid)
                {
                    //if (name == '' || email == '' || password == '' || cpassword == '') {
                    case "1"://註冊
                        try {
                        conn_setup(true);
                        usrname = context.Request.Form["name"];
                        pwd = context.Request.Form["password"];
                        email = context.Request.Form["email"];
                        contact = context.Request.Form["contact"];
                        sqlstr = "select name from MemberAccount Where Email='" + email + "'";
                        cmmd_setup(sqlstr);
                        dr = cmmd.ExecuteReader();
                        if (dr.Read())
                        {
                            context.Response.Write("already exist...");//輸入的Email已被使用！！");
                            dr_close();
                            return;
                        }
                        dr_close();
                        sqlstr = "select membercount,alphabet,available from countReader ";
                        cmmd_setup(sqlstr);
                        dr = cmmd.ExecuteReader();
                        dr.Read();
                        bool avaiable = Convert.ToBoolean(dr["available"]);
                        while (!avaiable)
                        {
                            Thread.Sleep(10);
                            dr.Close();
                            dr = cmmd.ExecuteReader();
                            dr.Read();
                            avaiable = Convert.ToBoolean(dr["available"]);
                        }
                        int count = Convert.ToInt32(dr["memberCount"]);
                        string A = dr["alphabet"].ToString();
                        dr.Close();
                        sqlstr = "update countReader set available=0 ";
                        cmmd_setup(sqlstr);
                        cmmd.ExecuteNonQuery();
                        Account = String.Format("{0}{1:0000}", A, count);
                        count++;
                        if (count == 10000)
                        {
                            count = 1;
                            A = after(A);
                        }

                        sqlstr = "insert into MemberAccount (Account,name,Email,password,contact,RegDate) VALUES(" + getString(Account) + ",'" + usrname + "','" + email + "','" + pwd + "','" + contact + "','" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "')";
                        sqlstr = sqlstr + "   Update countReader set memberCount=" + count + ",available=1,alphabet=" + getString(A);
                        cmmd_setup(sqlstr);
                        cmmd.ExecuteNonQuery();
                        context.Session["Name"] = usrname;
                        context.Session["Account"] = Account;
                        log.Info("註冊:name=" + usrname + ",Email=" + email);
                        context.Response.Write("OK Registered...");
                        }
                        catch ( Exception ex) {
                            log.Error(ex.Message);
                        }
                        finally
                        {
                            cmmd_dispose();
                            conn_close();
                        }

                        break;
                    case "2": //登入
                        try
                        {
                            conn_setup(true);
                        email = context.Request.Form["email"];
                        pwd = context.Request.Form["password"];
                        sqlstr = "select Account, Name from MemberAccount Where Email='" + email + "' and password='" + pwd + "'";
                        cmmd_setup(sqlstr);
                        dr = cmmd.ExecuteReader();
                        string responseStr;
                        bool sucess;
                        if (dr.Read())
                        {

                            context.Session["Name"] = dr["Name"].ToString();
                            context.Session["Account"] = dr["Account"].ToString();
                            responseStr = "Successfully Logged in...";
                            log.Info("登入:Email=" + email + "; Account:" + dr["Account"].ToString());
                            sucess = true;
                        }
                        else
                        {
                            responseStr = "Email or Password is wrong...";
                            log.Info("登入失敗:Email=" + email);
                            sucess = false;
                        }
                        dr_close();
                        if (sucess)
                        {
                            sqlstr = "update MemberAccount set lastLogin= '" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "'  Where Account=" + getString(context.Session["Account"].ToString());
                            cmmd_setup(sqlstr);
                            cmmd.ExecuteNonQuery();
                        }
                        context.Response.Write(responseStr);
                        MessageQueue Mqbrocast;

                        if ((MessageQueue.Exists(@".\Private$\brocast")))
                            Mqbrocast = new MessageQueue(@".\Private$\brocast");
                        else
                            Mqbrocast = MessageQueue.Create(@".\Private$\brocast");

                        Mqbrocast.Formatter = new XmlMessageFormatter(new string[] { "System.String,mscorlib" });
                        Mqbrocast.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                        System.Messaging.Message msg = new System.Messaging.Message();
                        msg.Body = context.Session["Name"].ToString() + ";homepage;login";
                        Mqbrocast.Send(msg);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                        }
                        finally
                        {
                            cmmd_dispose();
                            conn_close();
                        }
                        break;
                    case "3": //get session
                        
                        if (context.Session["Name"] != null)
                            context.Response.Write(context.Session["Name"].ToString() + "," + context.Session["Account"].ToString());
                        else
                            context.Response.Write("null");

                        break;
                    case "4": //SendMessage for upload file info
                        
                        if (context.Session["Upload"] != null)
                        {
                            List<string> ls = (List<string>)context.Session["Upload"];
                            MessageQueue MqHandle;

                            if ((MessageQueue.Exists(@".\Private$\MqHandle")))
                                MqHandle = new MessageQueue(@".\Private$\MqHandle");
                            else
                                MqHandle = MessageQueue.Create(@".\Private$\MqHandle");

                            MqHandle.Formatter = new XmlMessageFormatter(new string[] { "System.String,mscorlib" });
                            MqHandle.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                            for (int i = 0; i < ls.Count; i++)
                            {
                                System.Messaging.Message msgo = new System.Messaging.Message();
                                msgo.Body = ls[i];
                                MqHandle.Send(msgo);
                            }
                            context.Session["Upload"] = null;
                            context.Response.Write("OK Sent...");
                            break;
                        }
                        context.Response.Write("Nothing Sent...");

                        break;
                    case "5"://登出
                        try
                        {
                            string MovieID = context.Request.Form["MovieID"];
                            string Vtype = context.Request.Form["Vtype"];
                            string PauseAt = context.Request.Form["PauseAt"];
                            string complete = context.Request.Form["complete"];
                            string username = context.Session["Name"].ToString();
                            Account = context.Session["Account"].ToString().Trim ();
                            conn_setup(true);
                            PauseAt = PauseAt.PadRight(8). Substring(0, 8); 
                            sqlstr = "Update MemberAccount set lastLogout=GETDATE() where Account=" + getString(Account);
                            if (MovieID != null)
                            {
                                sqlstr = "Update MemberAccount set lastLogout=GETDATE(), lastMovieID=" + getString(MovieID) + " where Account=" + getString(Account) +
                                    "; Update Footprint Set playcount=playcount+1, complete=" + getString(complete) + ", pauseAt=" +
                                    getString(PauseAt) + ", watchTime=GetDate() Where Account=" + getString(Account) + " and MovieID=" + getString(MovieID) + "  " +
                                    "if @@rowcount=0  " +
                                    "begin  " +
                                    "Insert into Footprint([Account],[Vtype],[playcount],[MovieID],[PauseAt],[complete],[WatchTime]) values(" +
                                    getString(Account) + ", " + getString(Vtype) + ", 1," + getString(MovieID) + ", " + getString(PauseAt) + ", " + getString(complete) +
                                    ", GETDATE()) " +
                                    "end ";
                            }
                            cmmd_setup(sqlstr);
                            cmmd.ExecuteNonQuery();
                            context.Session.Clear();
                            context.Response.Write("LogOut Success...");
                            MessageQueue Mqbrocast1;

                            if ((MessageQueue.Exists(@".\Private$\brocast")))
                                Mqbrocast1 = new MessageQueue(@".\Private$\brocast");
                            else
                                Mqbrocast1 = MessageQueue.Create(@".\Private$\brocast");

                            Mqbrocast1.Formatter = new XmlMessageFormatter(new string[] { "System.String,mscorlib" });
                            Mqbrocast1.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                            System.Messaging.Message msg1 = new System.Messaging.Message();
                            msg1.Body = username + ";homepage;logout";
                            Mqbrocast1.Send(msg1);

                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                            log.Error(ex.StackTrace);
                        }
                        finally
                        {
                            cmmd_dispose();
                            conn_close();
                        }

                        break;
                    case "51"://登出
                        try
                        {
                            String username= context.Session["Name"].ToString().Trim();
                            Account = context.Session["Account"].ToString().Trim();
                            conn_setup(true);
                            sqlstr = "Update MemberAccount set lastLogout=GETDATE() where Account=" + getString(Account);
                            cmmd_setup(sqlstr);
                            cmmd.ExecuteNonQuery();
                            context.Session.Clear();
                            context.Response.Write("LogOut Success...");
                            MessageQueue Mqbrocast1;

                            if ((MessageQueue.Exists(@".\Private$\brocast")))
                                Mqbrocast1 = new MessageQueue(@".\Private$\brocast");
                            else
                                Mqbrocast1 = MessageQueue.Create(@".\Private$\brocast");

                            Mqbrocast1.Formatter = new XmlMessageFormatter(new string[] { "System.String,mscorlib" });
                            Mqbrocast1.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                            System.Messaging.Message msg1 = new System.Messaging.Message();
                            msg1.Body = username + ";homepage;logout";
                            Mqbrocast1.Send(msg1);

                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                            log.Error(ex.StackTrace);
                        }
                        finally
                        {
                            cmmd_dispose();
                            conn_close();
                        }

                        break;
                    case "6"://leaving play.html page 
                        try
                        {
                            string MovieID = context.Request.Form["MovieID"];
                            string Vtype = context.Request.Form["Vtype"];
                            string PauseAt = context.Request.Form["PauseAt"];
                            string complete = context.Request.Form["complete"];
                            PauseAt = PauseAt.PadRight(8).Substring(0, 8).Trim();
                            Account = context.Session["Account"].ToString();
                            conn_setup(true);
                            sqlstr = "Update MemberAccount set lastLogout=GETDATE() where Account=" + getString(Account);
                            if (MovieID != null)
                            {
                                sqlstr = "Update MemberAccount set lastLogout=GETDATE(), lastMovieID=" + getString(MovieID) + " where Account=" + getString(Account) +
                                    "; Update Footprint Set playcount=playcount+1, complete=" + getString(complete) + ", pauseAt=" +
                                    getString(PauseAt) + ", watchTime=GetDate() Where Account=" + getString(Account) + " and MovieID=" + getString(MovieID) +
                                    "if @@rowcount=0  " +
                                    "begin  " +
                                    "Insert into Footprint([Account],[Vtype],[playcount],[MovieID],[PauseAt],[complete],[WatchTime]) values(" +
                                    getString(Account) + ", " + getString(Vtype) + ", 1," + getString(MovieID) + ", " + getString(PauseAt) + ", " + getString(complete) +
                                    ", GETDATE()) " +
                                    "end ";
                            }
                            cmmd_setup(sqlstr);
                            cmmd.ExecuteNonQuery();
                            context.Response.Write("Leaving Play Page...");

                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                        }
                        finally
                        {
                            cmmd_dispose();
                            conn_close();
                        }
                        break;
                    case "A"://Update 播放次數
                        try
                        {
                            string MovieID = context.Request.Form["MovieID"];
                            conn_setup(true);
                            sqlstr = "Update MovieBase set numViewed=numViewed +1 where MovieID=" + getString(MovieID);
                            cmmd_setup(sqlstr);
                            cmmd.ExecuteNonQuery();
                            context.Response.Write("Play count Update...");
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                        }
                        finally
                        {
                            cmmd_dispose();
                            conn_close();
                        }

                        break;
                    case "B"://本影片有無看過
                        try
                        {
                            string MovieID = context.Request.Form["MovieID"];
                            string Movietype = context.Request.Form["Vtype"];
                            Account = context.Session["Account"].ToString();
                            conn_setup(true);
                            sqlstr = "Select top 1 complete,pauseAt from footPrint where Vtype=" + getString(Movietype) + " and Account=" + getString(Account) + " and MovieId=" + getString(MovieID) +
                                       " order by WatchTime desc";
                            cmmd_setup(sqlstr);
                            dr = cmmd.ExecuteReader();
                            if (dr.Read())
                            {
                                if (dr["complete"].ToString() == "1")
                                    context.Response.Write("null");
                                else
                                    context.Response.Write(dr["pauseAt"].ToString());

                            }
                            else
                            {
                                context.Response.Write("null");
                            }
                            dr_close();
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                        }
                        finally
                        {
                            cmmd_dispose();
                            conn_close();
                        }
                        break;
                    case "C"://本影片評分
                        try
                        {
                            string MovieID = context.Request.Form["MovieID"];
                            string rating = context.Request.Form["rating"];
                            Account = context.Session["Account"].ToString();
                            conn_setup(true);
                            sqlstr = "update Rating set start=" + rating + ",pdate=GETDATE() where MovieID=" + getString(MovieID) + " and Account=" + getString(Account) +
                                   "if @@rowcount=0  " +
                                    "begin  " +
                                    "Insert into rating([MovieID],[Account],[star],[Pdate]) values(" +
                                    getString(MovieID) + ", " + getString(Account) + "," + rating + "," + ", GETDATE()) " +
                                    "end " + "update movieBase set evaluate=(Select 1.0*sum(star)/count('*') from rating where MovieID=" + getString(MovieID) + ") where movieID=" +
                                    getString(MovieID) + " and star is not null";

                            cmmd_setup(sqlstr);
                            cmmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                        }
                        finally
                        {
                            cmmd_dispose();
                            conn_close();
                        }
                        break;
                    case "D"://本影片評價內容
                        try
                        {
                            string MovieID = context.Request.Form["MovieID"];
                            string comment = context.Request.Form["Comment"];
                            Account = context.Session["Account"].ToString();
                            conn_setup(true);
                            sqlstr = " Insert into Comment([MovieID],[Account],[comment],[Pdate]) values(" +
                                    getString(MovieID) + ", " + getString(Account) + ",'" + comment + "', GETDATE()) ";

                            cmmd_setup(sqlstr);
                            cmmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                        }
                        finally
                        {
                            cmmd_dispose();
                            conn_close();
                        }
                        break;
                    case "E"://變更註冊資料
                        try
                        {
                            conn_setup(true);
                            string AccountNo = context.Session["Account"].ToString();
                            usrname = context.Request.Form["name"];
                            string oldpwd = context.Request.Form["oldpassword"];
                            pwd = context.Request.Form["password"];
                            email = context.Request.Form["email"];
                            contact = context.Request.Form["contact"];
                            sqlstr = "select password from MemberAccount Where account=" + getString(AccountNo);
                            cmmd_setup(sqlstr);
                            string dbpwd = cmmd.ExecuteScalar().ToString();
                            if (dbpwd != pwd)
                            {
                                context.Response.Write("wrong password...");//輸入的密碼錯誤！！");
                                dr_close();
                                conn_close();
                                return;
                            }
                            dr_close();
                            sqlstr = "select name from MemberAccount Where account <> " + getString(AccountNo) + " and  Email='" + email + "'";
                            cmmd_setup(sqlstr);
                            dr = cmmd.ExecuteReader();
                            if (dr.Read())
                            {
                                context.Response.Write("already exist...");//輸入的Email已被使用！！");
                                dr_close();
                                conn_close();
                                return;
                            }
                            dr_close();
                            if (pwd.Trim() == "")
                                sqlstr = "update  MemberAccount set name=" + getString(usrname) + ",Email=" + getString(email) + ",contact=" + getString(contact) +
                                   " Where  Account=" + getString(AccountNo);
                            else
                                sqlstr = "update  MemberAccount set name=" + getString(usrname) + ",Email=" + getString(email) + ",password=" + getString(pwd) + ",contact=" + getString(contact) +
                                   " Where  Account=" + getString(AccountNo);

                            cmmd_setup(sqlstr);
                            cmmd.ExecuteNonQuery();
                            context.Session["Name"] = usrname;
                            log.Info("更新註冊:name=" + usrname + ",Email=" + email);
                            context.Response.Write("OK Registered...");
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                        }
                        finally
                        {
                            cmmd_dispose();
                            conn_close();
                        }
                        break;

                    case "F"://更新某影片內容資料
                        //$.post("dprocess.ashx", { qid: F, mvid: mvid, title: title, type: type, intro: intro, actor: actor }, function (data) {
                        try
                        {
                            conn_setup(true);
                            string Mvid = context.Request.Form["mvid"];
                            string title = context.Request.Form["title"];
                            string type = context.Request.Form["type"];
                            string intro = context.Request.Form["intro"];
                            string actor = context.Request.Form["actor"];

                            sqlstr = "update MovieBase set Title=" + getString(title) + ", Vtype=" + getString(type) + ", Intro=" + getString(intro) + ",Actors=" + getString(actor) +
                                " where MovieID= " + getString(Mvid);
                            cmmd_setup(sqlstr);
                            cmmd.ExecuteNonQuery();
                            log.Info("更新影片資料:movieid=" + Mvid + ",片名=" +title);
                            context.Response.Write("OK Registered...");
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                        }
                        finally
                        {
                            cmmd_dispose();
                            conn_close();
                        }
                        break;
                    case "G"://刪除某影片所有資料
                        //$.post("dprocess.ashx", { qid: F, mvid: mvid, title: title, type: type, intro: intro, actor: actor }, function (data) {
                        try
                        {
                            string Mvid = context.Request.Form["mvid"];
                            string title = context.Request.Form["title"];
                            SqlTransaction trans;
                            conn_setup();
                            trans = conn.BeginTransaction();
                            cmmd_setup();
                            cmmd.Connection = conn;
                            cmmd.Transaction = trans;
                            try
                            {
                                cmmd.CommandText = "delete from footprint  where MovieID= " + getString(Mvid) + "; ";
                                cmmd.ExecuteNonQuery();
                                cmmd.CommandText = "delete from Carousel  where MovieID= " + getString(Mvid) + "; ";
                                cmmd.ExecuteNonQuery();
                                cmmd.CommandText = "delete from comment  where MovieID= " + getString(Mvid) + "; ";
                                cmmd.ExecuteNonQuery();
                                cmmd.CommandText = "delete from rating  where MovieID= " + getString(Mvid) + "; ";
                                cmmd.ExecuteNonQuery();
                                cmmd.CommandText = "update MemberAccount set lastMovieID=''  where lastMovieID= " + getString(Mvid) + "; ";
                                cmmd.ExecuteNonQuery();
                                string SaveDir="";
                                 string filename="";
                                cmmd.CommandText = "select saveDir,filename from MovieBase  where MovieID= " + getString(Mvid);
                                dr = cmmd.ExecuteReader();
                                if (dr.Read()){
                                     SaveDir=dr["SaveDir"].ToString ();
                                     filename = dr["filename"].ToString();
                                }
                                dr.Close();
                                try
                                {
                                    if (SaveDir != "" && filename != "")
                                    {
                                        File.Delete(SaveDir + filename);
                                        string[] parselist = filename.Split('.');
                                        string fileHead = "";
                                        for (int i = 0; i < parselist.Length - 1; i++)
                                        {
                                            fileHead += parselist[i] + ".";
                                        }
                                        var dir = new DirectoryInfo(SaveDir);

                                        foreach (var file in dir.EnumerateFiles(fileHead + "*"))
                                        {
                                            file.Delete();
                                        }
                                        //move to videoHandle
                                        //foreach (var file in dir.EnumerateFiles(Mvid+"_output*.mp4"))
                                        //{
                                        //    file.Delete();
                                        //}
                                        foreach (var file in dir.EnumerateFiles(Mvid + "_Seg*.mp4"))
                                        {
                                            file.Delete();
                                        }
                                        File.Delete(SaveDir + Mvid + ".mp4");
                                        File.Delete(SaveDir + Mvid + ".jpg");

                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex.Message);
                                    log.Error(ex.StackTrace);
                                }
                                cmmd.CommandText = "delete from MovieBase  where MovieID= " + getString(Mvid) ;
                                cmmd.ExecuteNonQuery();
                                trans.Commit();
                                log.Info("刪除影片資料:movieid=" + Mvid + ",片名=" + title);
                                context.Response.Write("OK Registered...");
                            }
                            catch (Exception ex)
                            {
                                log.Error(ex.Message);
                                log.Error(ex.StackTrace);
                                try
                                {
                                    trans.Rollback();
                                }
                                catch (Exception ex2)
                                {
                                    log.Error(ex2.Message);
                                }
                            }
                            //sqlstr = "delete from footprint  where MovieID= " + getString(Mvid) + "; ";
                            //sqlstr += "delete from Carousel  where MovieID= " + getString(Mvid) + "; ";
                            //sqlstr += "delete from comment  where MovieID= " + getString(Mvid) + "; ";
                            //sqlstr += "delete from rating  where MovieID= " + getString(Mvid) + "; ";
                            //sqlstr += "update MemberAccount set lastMovieID=''  where lastMovieID= " + getString(Mvid) + "; ";
                            //cmmd_setup(sqlstr);
                            //cmmd.ExecuteNonQuery();
                            //sqlstr=
                            //sqlstr = "delete from MovieBase  where MovieID= " + getString(Mvid) + "; ";
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                        }
                        finally
                        {
                            cmmd_dispose();
                            conn_close();
                        }

                        break;
                    case "H"://刪除對影片某評論
                        try
                        {
                            conn_setup(true);
                            Account = context.Session["Account"].ToString();
                            string rid = context.Request.Form["rid"];

                            sqlstr = "delete from comment where rid= " + getString(rid);
                            cmmd_setup(sqlstr);
                            cmmd.ExecuteNonQuery();
                            log.Info("會員:" + Account + "刪除評論!!");
                            context.Response.Write("OK Registered...");
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex.Message);
                        }
                        finally
                        {
                            cmmd_dispose();
                            conn_close();
                        }

                        break;

                    default:
                        context.Response.Write("Not yet Implement");
                        break;
                }

            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            finally
            {
                dr_close();
                cmmd_dispose();
                conn_close();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public string getString(string vStr)
        {
            if (vStr.IndexOf("'") >= 0)
                vStr = vStr.Replace("'", "''");
            if (vStr != "")
                return "'" + vStr + "'";
            else
                return "Null";
        }

        public string getStringEmpty(string vStr)
        {
            if (vStr.IndexOf("'") >= 0)
                vStr = vStr.Replace("'", "''");
            if (vStr != "")
                return "'" + vStr + "'";
            else
                return "";
        }
        public string after(string a)
        {
            string[] ll = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M" };
            for (int i = 0; i < ll.Length; i++)
            {
                if (a == ll[i])
                {
                    i++;
                    if (i == ll.Length)
                        return ll[0];
                    else
                        return ll[i];
                }
            }
            return "";
        }

        #region SQL Related

        private void conn_setup(bool bOpen = true)
        {
            if ((conn == null))
            {
                conn = new SqlConnection();
                conn.ConnectionString = connStr;
            }

            if (bOpen && conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }


        private void conn_close()
        {
            if ((conn != null))


                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            conn = null;
        }
        //設定da
        private void da_setup(string sSQL = "select 1")
        {
            if (string.IsNullOrEmpty(sSQL))
            {
                sSQL = "select 1";
            }

            if ((da == null))
            {
                da = new SqlDataAdapter(sSQL, conn);
            }
            else
            {
                da.SelectCommand.CommandText = sSQL;
            }
        }

        private void da_dispose()
        {


            if ((da != null))
            {

                da.Dispose();
                da = null;
            }
        }

        private void cmmd_dispose()
        {


            if ((cmmd != null))
            {

                cmmd.Dispose();
                cmmd = null;
            }
        }
        //設定cmd
        private void cmmd_setup(string sSQL = "select 1")
        {
            if (string.IsNullOrEmpty(sSQL))
            {
                sSQL = "select 1";
            }

            if ((cmmd == null))
            {
                cmmd = new SqlCommand(sSQL, conn);
            }
            else
            {
                cmmd.CommandText = sSQL;
            }
        }

        //設定ds
        private void ds_setup()
        {
            if ((ds == null))
            {
                ds = new DataSet();
            }
        }

        private void ds_dispose()
        {


            if ((ds != null))
            {

                ds.Dispose();
            }
        }
        private void dr_close()
        {


            if ((dr != null))
            {

                dr.Close();
            }
        }

        #endregion
    }
}


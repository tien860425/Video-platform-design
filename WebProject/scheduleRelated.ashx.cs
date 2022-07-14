
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using log4net;
using log4net.Config;
using System.Text;

namespace WebProject
{
    /// <summary>
    /// SendOutJSon 的摘要描述
    /// </summary>
    public class scheduleRelated : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        String connStr;
        SqlConnection conn;
        SqlCommand cmmd;
        SqlDataAdapter da;
        SqlDataReader dr;
        DataSet ds;
        string jstring;
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

            string ID = context.Request.QueryString["qid"];
            try
            {
                switch (ID)
                {

                    case "1":
                        dr_close();

                        jstring = getCarousel();

                        context.Response.ContentType = "text/plain";
                        context.Response.Write(jstring);
                        break;
                    case "2": //Admin call to modify
                        dr_close();

                        jstring = getCommercial();

                        context.Response.ContentType = "text/plain";
                        context.Response.Write(jstring);
                        break;
                    case "3": //Admin call to modify
                        dr_close();

                        jstring = Common_getCommercial();

                        context.Response.ContentType = "text/plain";
                        context.Response.Write(jstring);
                        break;
                    case "4": //佈告
                        dr_close();

                        jstring = getBulletin();

                        context.Response.ContentType = "text/plain";
                        context.Response.Write(jstring);
                        break;
                    case "5"://取得該類最新100筆
                        dr_close();
                        //["懸疑", "動漫" , "微電影","動作", "喜劇",  "愛情", "科幻","恐怖", "教學", "其他"];
                        string Movietype = context.Request.QueryString["type"];
                        jstring = getTypedMovie(Movietype);

                        context.Response.ContentType = "text/plain";
                        context.Response.Write(jstring);
                        break;
                    case "6"://取得某影片評語
                        dr_close();
                        string MovieID = context.Request.QueryString["MovieID"];
                        jstring = getMovieComment(MovieID, "");

                        context.Response.ContentType = "text/plain";
                        context.Response.Write(jstring);
                        break;
                    case "7"://取得某影片最新評語
                        dr_close();
                        string MovieID1 = context.Request.QueryString["MovieID"];
                        string timepoll = context.Request.QueryString["timepoll"];
                        jstring = getMovieComment(MovieID1, timepoll);

                        context.Response.ContentType = "text/plain";
                        context.Response.Write(jstring);
                        break;
                    case "8"://取得某會員資料
                        dr_close();
                        string AccountNo = context.Session["Account"].ToString();

                        jstring = getPersonel(AccountNo);

                        context.Response.ContentType = "text/plain";
                        context.Response.Write(jstring);
                        break;
                    case "9"://取得某會員上傳資料
                        dr_close();
                        AccountNo = context.Session["Account"].ToString();

                        jstring = getUpLoad(AccountNo);

                        context.Response.ContentType = "text/plain";
                        context.Response.Write(jstring);
                        break;
                    case "10"://取得某會員觀看記錄
                        dr_close();
                        AccountNo = context.Session["Account"].ToString();

                        jstring = getView(AccountNo);

                        context.Response.ContentType = "text/plain";
                        context.Response.Write(jstring);
                        break;
                    case "11"://取得某會員對某影片評論
                        dr_close();
                        AccountNo = context.Session["Account"].ToString();
                        string mvid = context.Request.QueryString["mvid"];

                        jstring = getComment(AccountNo,mvid);

                        context.Response.ContentType = "text/plain";
                        context.Response.Write(jstring);
                        break;

                    default:
                        context.Response.Write(null);
                        break;
                }
            }
            catch ( Exception ex){
                log.Error(ex.Message);
                context.Response.Write(null);

            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public string getCommercial(){//10天內
          try
            {
                String jsonString = "[";
                //string strSql = "select Principal as x, '[new Date('+ IIF(ShowDateStart< GETDATE(),convert(varchar,DATEPART(yy,GETDATE())) +  ',' + convert(varchar,DATEPART(mm,GETDATE())) + ',' + convert(varchar,DATEPART(dd,GETDATE())),convert(varchar,DATEPART(yy,ShowDateStart)) +  ',' + convert(varchar,DATEPART(mm,ShowDateStart)) + ',' + convert(varchar,DATEPART(dd,ShowDateStart)))+ '), new Date(' +IIF(ShowDateStop > GETDATE()+10,convert(varchar,DATEPART(yy,GETDATE()+10)) +  ',' + convert(varchar,DATEPART(mm,GETDATE()+10)) + ',' + convert(varchar,DATEPART(dd,GETDATE()+10)),convert(varchar,DATEPART(yy,ShowDateStop)) +  ',' + convert(varchar,DATEPART(mm,ShowDateStop)) + ',' + convert(varchar,DATEPART(dd,ShowDateStop)))+ ')]' as y from CommercialAd Where ShowDateStart < GETDATE()+10 and ShowDateStop > GETDATE() order by ShowDateStart, priority";
                //string strSql = "select Principal as x, IIF(ShowDateStart< GETDATE(),GETDATE(),ShowDateStart) as start,IIF(ShowDateStop > GETDATE()+10,GETDATE()+10,ShowDateStop) as stop from CommercialAd Where ShowDateStart < GETDATE()+10 and ShowDateStop > GETDATE() order by ShowDateStart, priority";
                string strSql = "select Principal as x, IIF(DATEDIFF(day,GETDATE(), ShowDateStart )>=0,DATEDIFF(day,GETDATE(), ShowDateStart) ,0) as start,IIF(DATEDIFF(day,GETDATE(), ShowDateStop)<=20,DATEDIFF(day,GETDATE(), ShowDateStop) ,20) as stop from CommercialAd Where ShowDateStart < GETDATE()+10 and ShowDateStop > GETDATE() order by ShowDateStart, priority";
                conn_setup();
            cmmd_setup(strSql);
            dr = cmmd.ExecuteReader();
            while (dr.Read())
            {
                //jsonString = jsonString + "{\"x\":" + "\"" + dr["x"].ToString() + "\",\"y\":" + dr["y"].ToString() + "},";
                jsonString = jsonString + "{\"label\":" + "\"" + dr["x"].ToString() + "\",\"y\":[" + Convert.ToInt32(dr["start"]) + "," + Convert.ToInt32(dr["stop"])+ "]},";
            }
            jsonString = jsonString.Substring(0, jsonString.Length - 1) + "]";
            dr_close();
                cmmd_dispose();
                conn_close();
            return jsonString;
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
                ds_dispose();
                da_dispose();
                conn_close();
                return "";
            }
        }

        public string Common_getCommercial()
        {//1本日
            try
            {

                string strSql = "select principal,filename,Description,MediaType,FileURL,WebURL from CommercialAd Where ShowDateStart <= GETDATE() and ShowDateStop >= GETDATE() order by ShowDateStart, priority";
                conn_setup();
                da_setup(strSql);
                ds_setup();
                da.Fill(ds, "sss");//撈出存入命名sss裡

                String jsonString = DataTableToJsonWithJsonNet(ds.Tables["sss"]);

                dr_close();
                ds_dispose();
                da_dispose();
                conn_close();
                return jsonString;
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
                ds_dispose();
                da_dispose();
                conn_close();
                return "";
            }
        }

        public string getBulletin()
        {
            try
            {
                String jsonString="";
                string strSql = "select m1content,m2content,m3content from Bulletin";
                conn_setup();
                cmmd_setup (strSql);
                dr = cmmd.ExecuteReader();
                if(dr.Read())
                    jsonString = dr["m1content"].ToString() + ";" + dr["m2content"].ToString() + ";" + dr["m3content"].ToString();
                dr_close();
                ds_dispose();
                da_dispose();
                conn_close();
                return jsonString;
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
                ds_dispose();
                da_dispose();
                conn_close();
                return "";
            }
        }
        public string getCarousel()
        {
            string returnStr = "{\"Hot\":";
            string strSql = "select  top 10 a.movieid,a.priority,b.title,b.vtype,b.intro,b.[url], b.filename from Carousel as a left join MovieBase as b on a.movieid=b.MovieID where a.Type='Hot' order by a.ShowDate desc, a.Priority ";
                 //"(select  top 3 a.movieid,a.priority,b.Title,b.Vtype,b.intro,b.[url], b.filename from Carousel as a left join MovieBase as b on a.movieid=b.MovieID where a.Type='New' order by a.ShowDate desc, a.Priority)";
            conn_setup();

            try
            {
            da_setup(strSql);
            ds_setup();
            da.Fill(ds, "sss");//撈出存入命名sss裡

            String jsonString = DataTableToJsonWithJsonNet(ds.Tables["sss"]);
            returnStr = returnStr + jsonString + ", \"New\":";
            ds_dispose();
            da_dispose();
             strSql ="select  top 10 a.movieid,a.priority,b.title,b.vtype,b.intro,b.[url], b.filename from Carousel as a left join MovieBase as b on a.movieid=b.MovieID where a.Type='New' order by a.ShowDate desc, a.Priority";
            da_setup(strSql);
            ds_setup();
            da.Fill(ds, "kkk");//撈出存入命名kkk裡

            jsonString = DataTableToJsonWithJsonNet(ds.Tables["kkk"]);
            returnStr = returnStr + jsonString + ", \"Hi\":";
            ds_dispose();
            da_dispose();
            strSql = "select  top 10 a.movieid,a.priority,b.title,b.vtype,b.intro,b.[url], b.filename from Carousel as a left join MovieBase as b on a.movieid=b.MovieID where a.Type='Hi' order by a.ShowDate desc, a.Priority";
            da_setup(strSql);
            ds_setup();
            da.Fill(ds, "ooo");//撈出存入命名ooo裡

            jsonString = DataTableToJsonWithJsonNet(ds.Tables["ooo"]);
            returnStr = returnStr + jsonString + "}";
            ds_dispose();
            da_dispose();
            conn_close();
            return returnStr;
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
                ds_dispose();
                da_dispose();
                conn_close();
                return "";
            }
        }
        public string DataTableToJsonWithJsonNet(DataTable table)
        {
            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(table);
            return jsonString;
        }


        public string getTypedMovie(string Movietype)
        {//1本日
            try
            {

                string strSql = "Select MovieID, Title, URL,Intro, Evaluate,filename  from MovieBase where Vtype=" + getString(Movietype);
                conn_setup();
                da_setup(strSql);
                ds_setup();
                da.Fill(ds, "sss");//撈出存入命名sss裡

                String jsonString = DataTableToJsonWithJsonNet(ds.Tables["sss"]);

                dr_close();
                ds_dispose();
                da_dispose();
                conn_close();
                return jsonString;
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
                ds_dispose();
                da_dispose();
                conn_close();
                return "";
            }
        }

        public string getMovieComment(string MovieID,string tmpoll="")
        {//
            try
            {
                string strSql;
                if(tmpoll=="")
                    strSql = "Select top 30 a.account,a.comment,iif(a.pdate < CONVERT(char(10), GETDATE(),110), CONVERT(char(10), a.pdate,110),CONVERT(char(8), a.pdate,108)) as wtime" +
                                ",a.pdate,b.Name from Comment a left join MemberAccount b on a.account=b.Account  where a.MovieID=" + getString(MovieID) + " order by a.pdate desc";
                else
                    strSql = "Select top 30 a.account,a.comment,iif(a.pdate < CONVERT(char(10), GETDATE(),110), CONVERT(char(10), a.pdate,110),CONVERT(char(8), a.pdate,108)) as wtime" +
                                ",a.pdate,b.Name from Comment a left join MemberAccount b on a.account=b.Account  where a.MovieID=" + getString(MovieID) + 
                                " and a.pdate > " + getString(tmpoll) + " order by a.pdate desc";

                conn_setup();
                da_setup(strSql);
                ds_setup();
                da.Fill(ds, "sss");//撈出存入命名sss裡

                String jsonString = DataTableToJsonWithJsonNet(ds.Tables["sss"]);

                dr_close();
                ds_dispose();
                da_dispose();
                conn_close();
                return jsonString;
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
                ds_dispose();
                da_dispose();
                conn_close();
                return "";
            }
        }
        public string getPersonel(string Account)
        {
            try
            {
                string strSql;
                strSql = "Select Account,Name,Password,RegDate,Contact,Email,LastLogout,a.LastMovieID,b.Title, b.Vtype,b.URL, b.filename from MemberAccount a left join MovieBase b on a.lastMovieID=b.MovieID where a.Account= " +
                       getString(Account);
                conn_setup();
                da_setup(strSql);
                ds_setup();
                da.Fill(ds, "sss");//撈出存入命名sss裡

                String jsonString = DataTableToJsonWithJsonNet(ds.Tables["sss"]);

                dr_close();
                ds_dispose();
                da_dispose();
                conn_close();
                return jsonString;
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
                ds_dispose();
                da_dispose();
                conn_close();
                return "";
            }
        }

        public string getUpLoad(string Account)
        {
            try
            {
                string strSql;
                strSql = "Select MovieID,Title,Url,Vtype,intro,Actors,UploadTime,Evaluate from MovieBase where Uploader= " +
                       getString(Account) + " order by Vtype, UploadTime desc";
                conn_setup();
                da_setup(strSql);
                ds_setup();
                da.Fill(ds, "sss");//撈出存入命名sss裡

                String jsonString = DataTableToJsonWithJsonNet(ds.Tables["sss"]);

                dr_close();
                ds_dispose();
                da_dispose();
                conn_close();
                return jsonString;
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
                ds_dispose();
                da_dispose();
                conn_close();
                return "";
            }
        }

        public string getView(string Account)
        {
            try
            {
                string strSql;
                strSql = "Select a.MovieID,b.Title,a.Vtype,c.VtypeChinese,b.URL,b.filename,b.intro,b.Actors,a.playcount,b.Evaluate,a.PauseAt,a.complete,a.WatchTime from " +
                "(select * from footprint where Account='A0003') a left join MovieBase b on a.MovieID=b.MovieID left join videoType c on a.Vtype=c.Vtype " +
                "order by a.Vtype, UploadTime desc";
                conn_setup();
                da_setup(strSql);
                ds_setup();
                da.Fill(ds, "sss");//撈出存入命名sss裡

                String jsonString = DataTableToJsonWithJsonNet(ds.Tables["sss"]);

                dr_close();
                ds_dispose();
                da_dispose();
                conn_close();
                return jsonString;
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
                ds_dispose();
                da_dispose();
                conn_close();
                return "";
            }
        }
        public string getComment(string Account,string mvid)
        {
            try
            {
                string strSql;
                strSql = "Select comment,pdate,rid from comment where MovieID=" + getString(mvid) + " and Account=" + getString(Account);
                conn_setup();
                da_setup(strSql);
                ds_setup();
                da.Fill(ds, "sss");//撈出存入命名sss裡

                String jsonString = DataTableToJsonWithJsonNet(ds.Tables["sss"]);

                dr_close();
                ds_dispose();
                da_dispose();
                conn_close();
                return jsonString;
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
                ds_dispose();
                da_dispose();
                conn_close();
                return "";
            }
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
        #endregion
    }
}
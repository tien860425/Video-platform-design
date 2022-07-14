<%@ Page ContentType="application/json" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Runtime.Serialization.Json" %>
<%@ Import Namespace="System.Runtime.Serialization" %>
<%@ Import Namespace="System.Runtime.Serialization.Json" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="log4net" %>
<%@ Import Namespace="log4net.Config" %>
<%@ Import Namespace="System.Threading" %>
[assembly: log4net.Config.XmlConfigurator(Watch = false)]

<script language="C#" runat="server">    

    // 檔案的上傳路徑
    private string Upload_Directory =  @"d:\ResourceBase\CustomUploads\";
    private static readonly int BUFFER_SIZE = 4 * 1024 * 1024;
    private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    String connStr;
    SqlConnection conn;
    SqlCommand cmmd;
    SqlDataAdapter da;
    SqlDataReader dr;
    DataSet ds;

    [DataContract]
    public class FileResponse
    {
        [DataMember]
        public string mvid;

        [DataMember]
        public string scut;
        [DataMember]
        public string name;
        [DataMember]
        public string title;
        [DataMember]
        public long size;
        [DataMember]
        public string type;
        [DataMember]
        public string Vtype;
        [DataMember]
        public string url;
        [DataMember]
        public string error;
        [DataMember]
        public string deleteUrl;
        [DataMember]
        public string path;
        [DataMember]
        public string deleteType;
    }

    [DataContract]
    private class UploaderResponse
    {
        [DataMember]
        public FileResponse[] files;
        public UploaderResponse(FileResponse[] fileResponses)
        {
            files = fileResponses;
        }
    }
    private FileResponse CreateFileResponse(string fileName, long size,string Videotype, string error,string movieid,string title,string cut,string path)
    {
        return new FileResponse()
        {
            title=title,
            mvid=movieid,
            name = Path.GetFileName(fileName),
            size = size,
            scut=cut ,
            Vtype=Videotype,
            type = String.Empty,
            url = String.Format("{0}?{1}={2}&stype={3}", Request.Url.AbsoluteUri, "file", HttpUtility.UrlEncode(Path.GetFileName(fileName)), Videotype),
            error = error,
            deleteUrl = String.Format("{0}?{1}={2}", Request.Url.AbsoluteUri, "file", HttpUtility.UrlEncode(Path.GetFileName(fileName))),
            path=path,
            deleteType = "POST"
        };
    }

    private FileResponse CreateFileResponse(string fileName, long size, string error,string title)
    {
        return new FileResponse()
        {
            title = title,
            name = Path.GetFileName(fileName),
            size = size,

            type = String.Empty,
            url = String.Format("{0}?{1}={2}", Request.Url.AbsoluteUri, "file", HttpUtility.UrlEncode(Path.GetFileName(fileName))),
            error = error,
            deleteUrl = String.Format("{0}?{1}={2}", Request.Url.AbsoluteUri, "file", HttpUtility.UrlEncode(Path.GetFileName(fileName))),
            deleteType = "POST"
        };
    }

    private void SerializeUploaderResponse(List<FileResponse> fileResponses)
    {  // 將物件序列化為 JavaScript 物件標記法 (JSON) 以及將 JSON 資料還原序列化為物件
        DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(UploaderResponse));
        // 將物件序列化為可以對應至 JavaScript 物件標記法 (JSON) 的 XML。 使用 XmlWriter 來寫入所有的物件資料，包括起始 XML 項目、內容和結尾項目。 
        Serializer.WriteObject(Response.OutputStream, new UploaderResponse(fileResponses.ToArray()));
    }

    private void FromStreamToStream(Stream source, Stream destination) // 搭配 GET method 使用
    {
        int BufferSize = source.Length >= BUFFER_SIZE ? BUFFER_SIZE : (int)source.Length;
        long BytesLeft = source.Length;
        byte[] Buffer = new byte[BufferSize];
        int BytesRead = 0;
        while (BytesLeft > 0)
        {
            BytesRead = source.Read(Buffer, 0, BytesLeft > BufferSize ? BufferSize : (int)BytesLeft);
            destination.Write(Buffer, 0, BytesRead);
            BytesLeft -= BytesRead;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        connStr = Session["Connectionstr"].ToString();
        string subName = Convert.ToString(Session["Account"]);
        Upload_Directory = Upload_Directory + subName + @"\";
        if (!Directory.Exists(Upload_Directory))  // 若目錄不存在則建立之
        {
            Directory.CreateDirectory(Upload_Directory);
        }
        string QueryFileName = Request["file"];  //從POST資料取出上傳的檔案名稱
        string stype = Request["stype"];  //從POST資料取出上傳的類別
        string FullFileName = null;     //用來放完整路徑
        string ShortFileName = null;  //存放檔名
                                      // 判斷上傳的檔名變數是否有內容 
        if (QueryFileName != null) // param specified, but maybe in wrong format (empty). else user will download json with listed files
        {
            ShortFileName = HttpUtility.UrlDecode(QueryFileName);     //取出檔名
            FullFileName = String.Format(@"{0}{1}\\{2}", Upload_Directory, stype,ShortFileName);   // 結合完整路徑與檔名~~成為完整PATH
            if (QueryFileName.Trim().Length == 0 || !File.Exists(FullFileName))  //判斷檔案是否存在
            {
                Response.StatusCode = 404;
                Response.StatusDescription = "File not found";
                Response.End();
                return;
            }
        }

        if (Request.HttpMethod.ToUpper() == "GET")   // ---- GET 的處理片段  -----------------------------------
        {
            if (FullFileName != null)
            {
                ShortFileName = HttpUtility.UrlDecode(QueryFileName);     //取出檔名
                FullFileName = String.Format(@"{0}{1}\\{2}", Upload_Directory, stype, ShortFileName);   // 結合完整路徑與檔名~~成為完整PATH
                Response.ContentType = "application/octet-stream";                   // http://www.digiblog.de/2011/04/android-and-the-download-file-headers/ :)
                Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}{1}", Path.GetFileNameWithoutExtension(ShortFileName), Path.GetExtension(ShortFileName).ToUpper()));
                using (FileStream FileReader = new FileStream(FullFileName, FileMode.Open, FileAccess.Read))
                {
                    FromStreamToStream(FileReader, Response.OutputStream);
                    Response.OutputStream.Close();
                }
                Response.End();
                return;
            }
            //else  // FullFileName == null  //一開始的已傳檔案列表
            //{
            //    //檔案列表
            //    List<FileResponse> FileResponseList = new List<FileResponse>();              
            //    string[] FileNames = Directory.GetFiles(Upload_Directory);
            //    DateTime TimeRange = DateTime.Now.AddDays(-1); //一天內

            //    foreach (string FileName in FileNames)
            //    {
            //        if (new FileInfo(FileName).CreationTime > TimeRange)
            //        {
            //            FileResponseList.Add(CreateFileResponse(FileName, new FileInfo(FileName).Length, String.Empty));
            //        }
            //    }
            //    SerializeUploaderResponse(FileResponseList);
            //}
        }  //EOF --- if (Request.HttpMethod.ToUpper() == "GET")
        else if (Request.HttpMethod.ToUpper() == "POST" && Request.QueryString["file"] == null) // ---- POST 的處理片段  -----------------------------------
        {
            try
            {
                Session["Upload"] = null;

                List<string> lstUpload = new List<string>();
                List<FileResponse> FileResponseList = new List<FileResponse>();
                for (int FileIndex = 0; FileIndex < Request.Files.Count; FileIndex++)  //利用 HttpPostedFile 方式來將檔案寫入到 Server
                {
                    HttpPostedFile hFile = Request.Files[FileIndex];
                    string ErrorMessage = String.Empty;
                    string sType = Request.Form["Vtype" + hFile.FileName];
                    string intro = Request.Form["intro" + hFile.FileName];
                    string actor = Request.Form["actor" + hFile.FileName];
                    string title = Request.Form["title" + hFile.FileName];
                    string scut = Request.Form["vcut" + hFile.FileName];
                    string ext = hFile.FileName.Split('.')[1].ToUpper();
                    intro = intro.PadRight(500).Substring(0, 500).Trim();
                    if (ext != "AVI" && ext != "MP4" && ext != "FLV" && ext != "WEBM" && ext != "WMV")
                    {
                        ErrorMessage = "Type error";
                        FileResponseList.Add(CreateFileResponse(hFile.FileName, hFile.ContentLength, ErrorMessage,title));
                        continue;
                    }

                    Upload_Directory = Upload_Directory + sType + @"\";
                    if (!Directory.Exists(Upload_Directory))  // 若目錄不存在則建立之
                    {
                        Directory.CreateDirectory(Upload_Directory);
                    }
                    string FileName = String.Format(@"{0}{1}", Upload_Directory, Path.GetFileName(hFile.FileName));

                    //if (System.IO.File.Exists(FileName))  // 檔名重複時的處理，增加序號 _yyyyMMddHHmmss.fff
                    //{
                    //    FileName = String.Format(@"{0}\{1}_{2:yyyyMMddHHmmss.fff}{3}", Upload_Directory, Path.GetFileNameWithoutExtension(FileName), DateTime.Now, Path.GetExtension(FileName));
                    //}
                    if (System.IO.File.Exists(FileName))  // 檔名重複時的處理
                    {
                        ErrorMessage = "該檔案已存在！";
                        FileResponseList.Add(CreateFileResponse(hFile.FileName, hFile.ContentLength, ErrorMessage,title));
                        continue;
                        //File.Delete(FileName);
                    }
                    hFile.SaveAs(FileName);  // 將檔案寫入 Server 端
                    string url =  "VideoBase/CustomUploads/" + subName + "/" + sType + "/";

                    string movieid = GetMovieId(sType, subName);
                    Upload_Directory = Upload_Directory.Replace('\\', '/');
                    FileResponseList.Add(CreateFileResponse(FileName, hFile.ContentLength, sType, ErrorMessage, movieid,title,scut,Upload_Directory));
                    lstUpload.Add(Upload_Directory + @";" + Path.GetFileName(hFile.FileName) + @";" + movieid + @";" + title + @";" +scut);
                    conn_setup();
                    string sqlstr = "Insert into MovieBase (MovieID,title,Uploader,UploadTime,Vtype,intro,Actors,SaveDir,URL,filename,evaluate) Values (" +
                               getString(movieid) + ", " + getString(title) + ", " + getString(subName) + ", GetDate(), " + getString(movieid.Substring(0, 1)) + ", " +
                               getString(intro) + ", " + getString(actor) + ", " + getString(Upload_Directory) + ", " + getString(url) + ", " + getString(hFile.FileName) + ", 0)";
                    cmmd_setup(sqlstr);
                    cmmd.ExecuteNonQuery();
                    conn_close();
                    cmmd_dispose();

                }
                SerializeUploaderResponse(FileResponseList);
                Session["Upload"] = lstUpload;
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
            }

        }  // EOF --- if (Request.HttpMethod.ToUpper() == "POST" && Request.QueryString["file"] == null)
           // 刪除檔案之片段  ---------------------------------------
        else if (Request.HttpMethod.ToUpper() == "POST" && Request.QueryString["file"] != null)
        {
            bool SuccessfullyDeleted = true;
            try
            {                File.Delete(FullFileName);            }
            catch
            {                SuccessfullyDeleted = false;            }
            Response.Write(String.Format("{{\"{0}\":{1}}}", ShortFileName, SuccessfullyDeleted.ToString().ToLower()));
        }
        else // 不是 GET 也不是 POST 也不是做 DELETE
        {
            Response.StatusCode = 405;
            Response.StatusDescription = "Method not allowed";
            Response.End();
            return;
        }
        Response.End();
    }


    public string getString(string vStr)
    {
        if (vStr.IndexOf("'") >= 0)
            vStr = vStr.Replace("'", "''");
        if(vStr != "")
            return  "'" + vStr + "'";
        else
            return "Null";
    }

    #region SQL Related
    private string GetMovieId(string sType,string account)
    {
        try
        {
            conn_setup();
            //string sqlstr="DECLARE  @Vtype varchar(2) , @VtypeName varchar(20), @cumulate int" +
            //               ", @available int " + 
            //               " SELECT @Vtype=Vtype, @VtypeName=VtypeName, @cumulate=cumulate,@available=available From videoType where VtypeName='" + sType + @"'"  +
            //               " if @avaiable=1 " +
            //               " begin " +
            //               "   Update videoType set available=0, cumulate=@cumulate+1 where VtypeName='" + sType + @"'" +
            //               "   Select @cumulate, @Vtype " + 
            //               " end ";
            cmmd_setup("Select Vtype,cumulate,available from videoType where VtypeName='" + sType + @"'");
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
            int count = Convert.ToInt32(dr["cumulate"]);
            string A = dr["Vtype"].ToString();
            dr.Close();
            string sqlstr = "update videoType set available=1,  cumulate= " + (count + 1).ToString() + " Where  VtypeName='" + sType + @"'";
            cmmd.CommandText = sqlstr;
            cmmd.ExecuteNonQuery();
            log.Info(sqlstr);

            string movieid = String.Format("{0}-{1}-{2:000000}", A, account, count);
            log.Info("Create MovieID:" + movieid);
            return movieid;
        }
        catch (Exception ex)
        {
            log.Error(ex.Message);
            return "";
        }
        finally
        {
            dr_close();
            cmmd_dispose();
            conn_close();
        }

    }
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

</script>
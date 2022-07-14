using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using log4net;
using log4net.Config;
using System.Data;
using System.IO;

namespace WebProject
{
    public partial class admin : System.Web.UI.Page
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        SqlConnection conn;
        SqlCommand cmmd;
        SqlDataAdapter da;
        SqlDataReader dr;
        DataSet ds;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Application["Connectionstr"] == null)
                {
                    XmlConfigurator.Configure(new System.IO.FileInfo(Server.MapPath("~/web.config")));
                    Application["connectionString"] = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;//存連線字串

                }
                
                GetDataBind();
                GetBulletin();
            }

        }

        private void GetBulletin()
        {
            String sqlStr;
            try
            {
                dr_close();
                conn_setup();
                sqlStr = "Select m1Content,m2Content,m3Content from bulletin " ;
                cmmd_setup(sqlStr);
                dr = cmmd.ExecuteReader();
                if (dr.Read())
                {
                    txtM1.Text = dr["m1Content"].ToString();
                    txtM2.Text = dr["m2Content"].ToString();
                    txtM3.Text = dr["m3Content"].ToString();

                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            finally
            {
                dr_close ();
                cmmd_dispose();
                conn_close();
            }
        }

        private void GetDataBind()
        {
            String sqlStr;
            try
            {
                conn_setup();
                sqlStr = "Select ShowDateStart,ShowDateStop,priority,filename,principal,Description,FileURL,MediaType,webURL from CommercialAd " +
                " Where ShowDateStop >= GETDATE()";
                da_setup(sqlStr);
                ds_setup();
                da.Fill(ds, "sss");//把setup的東西撈出存入命名sss裡
                GridView1.DataSource = ds.Tables["sss"].DefaultView;
                GridView1.DataBind();
                if (GridView1.Rows.Count > 0)
                {
                    GridView1.SelectedIndex = 0;
                    Display(0);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            finally
            {
                ds_dispose();
                da_dispose();
                conn_close();
            }
        }
        protected void btnStart_Click(object sender, EventArgs e)
        {
            btnEnd.Enabled = false;
            Calendar1.Visible = true;
        }

        protected void btnEnd_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            Calendar1.Visible = true;
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            if (btnEnd.Enabled == false)
            {
                btnStart.Text = Calendar1.SelectedDate.ToString("yyyy/MM/dd");
                btnEnd.Enabled = true;
                Calendar1.EnableViewState = false;
                Calendar1.Visible = false;
            }
            else
            {
                btnEnd.Text = Calendar1.SelectedDate.ToString("yyyy/MM/dd");
                btnStart.Enabled = true;
                Calendar1.EnableViewState = false;
                Calendar1.Visible = false;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //
            //string saveDir = @"D:\ResourceBase\commercial\";
            //string fileUrl = "VideoBase/Commercial/";
            //String filename = FileUpload1.FileName;  //-- User上傳的檔名（不包含 Client端的路徑！）

            //Boolean fileOK = false;
            ////string appPath = Request.PhysicalApplicationPath;
            //string fileType = "P";

            if (Convert.ToDateTime(btnStart.Text) > Convert.ToDateTime(btnEnd.Text))
            {
                UploadStatusLabel.Text = "開始日期不可晚於結束日期！";
                return;
            }
            if (txtPricipal.Text.Trim() == "")
            {
                UploadStatusLabel.Text = "委託人不可空白！";
                return;
            }
            if (FuncOP.SelectedIndex != 2)
            {
                if (txtDesciption.Text.Trim() == "")
                {
                    UploadStatusLabel.Text = "廣告詞不可空白！";
                    return;
                }
                if (txtwebUrl.Text.Trim() == "")
                {
                    UploadStatusLabel.Text = "委託人網址不可空白！";
                    return;
                }
            }
            if (FuncOP.SelectedIndex == 0)//新增
            {
                if (FileUpload1.HasFile)
                {
                    if (CheckExist())
                    {
                        UploadStatusLabel.Text = "資料已存在！！！";
                        return;

                    }
                    InsertRecord();
                    GetDataBind();
                    GridView1.SelectedIndex = 0;
                    UploadStatusLabel.Text = "新增成功!";

                }
                else
                {
                    UploadStatusLabel.Text = "請先挑選檔案之後，再來上傳";
                }
            }
            if (FuncOP.SelectedIndex == 1 && FileUpload1.HasFile)//修改
            {
                DeleteRecord();
                InsertRecord();
                GetDataBind();
                GridView1.SelectedIndex = 0;
                UploadStatusLabel.Text = "更新成功!";


            }
            if (FuncOP.SelectedIndex == 1 && !FileUpload1.HasFile)//修改
            {
                UpdateRecord();
                GetDataBind();
                GridView1.SelectedIndex = 0;
                UploadStatusLabel.Text = "更新成功!";

            }
            if (FuncOP.SelectedIndex == 2)//刪除
            {
                DeleteRecord();
                GetDataBind();
                GridView1.SelectedIndex = 0;
                UploadStatusLabel.Text = "刪除成功!";

            }
        }

        #region SQL Related

        private void conn_setup(bool bOpen = true)
        {
            if ((conn == null))
            {
                conn = new SqlConnection();
                conn.ConnectionString = Application["connectionString"].ToString();
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

        private void UpdateRecord()
        {
                String principal = GridView1.Rows[GridView1.SelectedIndex].Cells[1].Text;
                String ShowDateStart = GridView1.Rows[GridView1.SelectedIndex].Cells[2].Text;
                String ShowDateStop = GridView1.Rows[GridView1.SelectedIndex].Cells[3].Text;
                try
                {
                    string sqlStr = "Update CommercialAd set priority=" + dpPriority.SelectedValue +",ShowDateStart=" +getString(btnStart.Text) +
                        ", ShowDateStop=" + getString(btnEnd.Text) + ", Description=" +getString(txtDesciption.Text.PadRight(100).Trim()) +
                        ", webUrl=" + getString(txtwebUrl.Text.Trim()) + "  where ShowDateStart=" + getString(ShowDateStart) + "and ShowDateStop=" + getString(ShowDateStop) + "and principal= " + getString(principal);
                   conn_setup();
                   cmmd_setup(sqlStr);
                   cmmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    UploadStatusLabel.Text = "發生例外錯誤！";
                    log.Error(ex.Message);
                }
                finally
                {
                    ds_dispose();
                    da_dispose();
                    conn_close();
                }

            
        }

        private void DeleteRecord()
        {
            string fileType = "P";
            String filename ;  //-- User上傳的檔名（不包含 Client端的路徑！）
            string fileExtension ;
            Boolean fileOK = false;

            if (FuncOP.SelectedIndex == 1)//更新時有變更檔案，先刪存於server的檔案
            {
                filename = FileUpload1.FileName;  //-- User上傳的檔名（不包含 Client端的路徑！）
                fileExtension = System.IO.Path.GetExtension(filename).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".avi", ".flv" };

                for (int i = 0; i < allowedExtensions.Length; i++)
                {
                    if (allowedExtensions[i] == fileExtension)
                    {
                        fileOK = true;
                        if (i < 3)
                            fileType = "P";
                        else
                            fileType = "V";
                        break;
                    }
                }
              if (!fileOK)
                {
                    UploadStatusLabel.Text = "上傳的檔案，副檔名只能是 .jpg, .jpeg, .png, .gif, .mp4, .avi, .flv 。";
                }
           }

                //--------------------------------------------------------------------------------------------------

                if (fileOK || FuncOP.SelectedIndex == 2)
                {
                    string originfilename = ((Label)GridView1.Rows[GridView1.SelectedIndex].Cells[5].FindControl("lblfilename")).Text;
                    String principal = GridView1.Rows[GridView1.SelectedIndex].Cells[1].Text;
                    String ShowDateStart = GridView1.Rows[GridView1.SelectedIndex].Cells[2].Text;
                    String ShowDateStop = GridView1.Rows[GridView1.SelectedIndex].Cells[3].Text;
                    string saveDir = @"D:\ResourceBase\commercial\";
                    try
                    {
                        //先刪原記錄
                        if (File.Exists(saveDir + originfilename))
                        {
                            File.Delete(saveDir + originfilename);
                        }
                        string sqlStr = "Delete from CommercialAd where ShowDateStart=" + getString(ShowDateStart) + "and ShowDateStop=" + getString(ShowDateStop) + "and principal= " + getString(principal);
                        conn_setup();
                        cmmd_setup(sqlStr);
                        cmmd.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        UploadStatusLabel.Text = "發生例外錯誤！";
                        log.Error(ex.Message);
                    }
                    finally
                    {
                        ds_dispose();
                        da_dispose();
                        conn_close();
                    }

                }
 
            }
        


        private void InsertRecord()
        {
            string saveDir = @"D:\ResourceBase\commercial\";
            string fileUrl = "VideoBase/Commercial/";
            String filename = FileUpload1.FileName;  //-- User上傳的檔名（不包含 Client端的路徑！）

            Boolean fileOK = false;
            //string appPath = Request.PhysicalApplicationPath;
            string fileType = "P";
            string fileExtension = System.IO.Path.GetExtension(filename).ToLower();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".avi", ".flv" };

            for (int i = 0; i < allowedExtensions.Length; i++)
            {
                if (allowedExtensions[i] == fileExtension)
                {
                    fileOK = true;
                    if (i < 3)
                        fileType = "P";
                    else
                        fileType = "V";
                }
            }

            //--------------------------------------------------------------------------------------------------

            if (fileOK)
            {
                try
                {
                    FileUpload1.PostedFile.SaveAs(saveDir + filename);
                    //*** 寫成這樣也行！ FileUpload1.SaveAs(path + filename);
                    string sqlStr = "Insert into CommercialAd(ShowDateStart,ShowDateStop,priority,filename,principal,Description,FileURL,MediaType,webURL) Values(" +
                        getString(btnStart.Text) + "," + getString(btnEnd.Text) + "," + dpPriority.SelectedValue + "," + getString(filename) + "," +
                        getString(txtPricipal.Text.Trim()) + "," + getString(txtDesciption.Text.PadRight(100).Trim()) + "," + getString(fileUrl) + "," + getString(fileType) + "," +
                        getString(txtwebUrl.Text.Trim()) + ")";
                    conn_setup();
                    cmmd_setup(sqlStr);
                    cmmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    UploadStatusLabel.Text = "發生例外錯誤，上傳失敗！";
                    log.Error(ex.Message);
                }
                finally
                {
                    ds_dispose();
                    da_dispose();
                    conn_close();
                }

            }
            else
            {
                UploadStatusLabel.Text = "上傳的檔案，副檔名只能是 .jpg, .jpeg, .png, .gif, .mp4, .avi, .flv 。";
            }
        }
        private bool CheckExist()
        {
            bool exist = false;
            try
            {
                conn_setup();
                string sqlStr = "Select * from  CommercialAd where ShowDateStart=" + getString(btnStart.Text) + " and ShowDateStop=" + getString(btnEnd.Text) + " and  principal= " + getString(txtPricipal.Text.Trim());
                cmmd_setup(sqlStr);
                dr = cmmd.ExecuteReader();
                if (dr.Read())//已存在
                {
                    exist = true;
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            finally
            {
                ds_dispose();
                da_dispose();
                conn_close();
            }
            return exist;
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

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ////String imageurl = ((Label)GridView1.Rows[GridView1.SelectedIndex].Cells[11].FindControl("lblimageurl")).Text;
                Display(GridView1.SelectedIndex);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

        }

        protected void FuncOP_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (FuncOP.SelectedIndex)
            {
                case 0:
                    btnConfirm.Text = "資料確定與檔案上傳";
                    break;
                case 1:
                    btnConfirm.Text = "修改資料確定";
                    break;
                default:
                    btnConfirm.Text = "確定刪除該筆資料";
                    break;


            }
        }
        private void Display(int index)//顯示顯項在左邊
        {
            //ShowDateStart,ShowDateStop,priority,filename,principal,Description,FileURL,MediaType,webURL
            String principal = GridView1.Rows[GridView1.SelectedIndex].Cells[1].Text;
            String ShowDateStart = GridView1.Rows[GridView1.SelectedIndex].Cells[2].Text;
            String ShowDateStop = GridView1.Rows[GridView1.SelectedIndex].Cells[3].Text;
            String priority = GridView1.Rows[GridView1.SelectedIndex].Cells[4].Text;
            String Description = ((Label)GridView1.Rows[GridView1.SelectedIndex].Cells[6].FindControl("lbldescription")).Text;
            String webURL = ((Label)GridView1.Rows[GridView1.SelectedIndex].Cells[9].FindControl("lblWebURL")).Text;
            btnStart.Text = ShowDateStart;
            btnEnd.Text = ShowDateStop;
            dpPriority.SelectedIndex = Convert.ToInt32(priority) - 1;
            txtPricipal.Text = principal;
            txtDesciption.Text = Description;
            txtwebUrl.Text = webURL;

        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            String sqlStr;
            try
            {
                dr_close();
                conn_setup();
                sqlStr = "Update bulletin set m1Content=" + getString (txtM1.Text.Trim ()) +", m2Content=" + getString (txtM2.Text.Trim ()) +", m3Content=" + getString (txtM3.Text.Trim ()) ;
                cmmd_setup(sqlStr);
                cmmd.ExecuteNonQuery();
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

    }
}
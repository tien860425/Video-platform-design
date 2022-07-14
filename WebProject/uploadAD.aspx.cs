using System;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Messaging;
using System.Threading;

namespace WebProject
{
    public partial class uploadAD : System.Web.UI.Page
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string savePath;
        string mvid;
        string scut;

        protected void Page_Load(object sender, EventArgs e)
        {
            savePath = Request.QueryString["path"];
            mvid = Request.QueryString["mvid"];
            scut = Request.QueryString["cut"];
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //參考資料：http://msdn.microsoft.com/zh-tw/library/system.web.ui.webcontrols.fileupload.saveas.aspx
            //註解：先設定好檔案上傳的路徑，這是Web Server電腦上的硬碟「實際」目錄。
            try
            {
                if (FileUpload1.HasFile)
                {
                    //==================================================(Start)
                    string fileName = FileUpload1.FileName;  //-- User上傳的檔名（不包含 Client端的路徑！）

                    string pathToCheck = savePath + fileName;
                    string tempfileName = "";

                    if (System.IO.File.Exists(pathToCheck))
                    {
                        int my_counter = 2;
                        while (System.IO.File.Exists(pathToCheck))
                        {
                            //路徑與檔名都相同的話，目前上傳的檔名（改成 tempfileName），前面會用數字來代替。
                            tempfileName = my_counter.ToString() + "_" + fileName;
                            pathToCheck = savePath + tempfileName;
                            my_counter = my_counter + 1;
                        }
                        fileName = tempfileName;
                        Label1.Text = "抱歉，您上傳的檔名發生衝突，檔名修改如下" + "<br>" + fileName;
                    }

                    //完成檔案上傳的動作。
                    FileUpload1.SaveAs(savePath + fileName);
                    //==================================================(End)
                    Label1.Text = "上傳成功，檔名---- " + fileName;
                    MessageQueue MqHandle = new MessageQueue();
                    System.Messaging.Message mes = new System.Messaging.Message();
                    if (MessageQueue.Exists(".\\Private$\\MqHandleMerge"))
                        MqHandle = new MessageQueue(".\\Private$\\MqHandleMerge");
                    else
                        MqHandle = MessageQueue.Create(".\\Private$\\MqHandleMerge");

                    MqHandle.Formatter = new XmlMessageFormatter(new string[] { "System.String,mscorlib" });
                    MqHandle.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
                    System.Messaging.Message msgo = new System.Messaging.Message();
                    msgo.Body = savePath + ";" + fileName + ";" + mvid;
                    MqHandle.Send(msgo);
                    if (!Page.ClientScript.IsStartupScriptRegistered("updateOpener"))
                    {
                        ScriptManager.RegisterStartupScript(Page, GetType(), "updateOpener", "<script>updateOpener('" + mvid +"');</script>", false);
        }

                }
                else
                {
                    Label1.Text = "請先挑選檔案之後，再來上傳";
                }

            }
            catch (Exception ex)
            {
                log.Info(ex.Message);

            }
            //finally
            //{
            //    Thread.Sleep(1500);
            //    //Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "CloseWindow", "returnToParent();", true);
            //    Response.Write("<script language='javascript'>window.close();</script>");
            //}

        }
    }
}
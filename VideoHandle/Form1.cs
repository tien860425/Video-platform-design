using System;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Messaging;
using System.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using log4net;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

[assembly: log4net.Config.XmlConfigurator(Watch = false)]
//This will cause log4net to look for a configuration file
//called ConsoleApp.exe.config in the application base
//directory (i.e. the directory containing ConsoleApp.exe)

namespace VideoHandle
{
    public partial class Form1 : Form
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool bClose = false;
        VideoCapture capture;
        bool Pause = false;
        String connStr;
        SqlConnection conn;
        SqlCommand cmmd;
        SqlDataAdapter da;
        SqlDataReader dr;
        DataSet ds;
        int segmentTime = 60;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            Thread th = new Thread(GetMessage);
            th.Start();
        }

        private void GetMessage()
        {
            MessageQueue myQueue = new MessageQueue();
            StringBuilder sbMsg = new StringBuilder();
            System.Messaging.Message mes = new System.Messaging.Message();
            if (MessageQueue.Exists(".\\Private$\\MqHandle"))
                myQueue = new MessageQueue(".\\Private$\\MqHandle");
            else
                myQueue = MessageQueue.Create(".\\Private$\\MqHandle");

            myQueue.Formatter = new XmlMessageFormatter(new string[] { "System.String,mscorlib" });
            myQueue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);

            while (!bClose)
            {
                try
                {

                    // Retrieve next message from queue
                    mes = myQueue.Receive();
                    mes.Formatter = new System.Messaging.XmlMessageFormatter(new String[] { "System.String,mscorlib" });
                    sbMsg.Remove(0, sbMsg.Length);
                    sbMsg.Append(mes.Body);
                    log.Info("Get Message:" + sbMsg.ToString());
                    String[] parse = sbMsg.ToString().Split(';');
                    string scut = "";
                    if (parse.Length >= 5)
                        scut = parse[4];
                    if (scut == "cutone")
                        segmentTime = 30;
                    else
                        if (scut == "cutfive")
                        segmentTime = 60;
                    else
                      if (scut == "cutten")
                        segmentTime = 300;

                    Label.CheckForIllegalCrossThreadCalls = false;
                    lblOrigin.Text = parse[0] + parse[1];
                    getShortVideo(parse[0], parse[1], parse[2]);
                    getImage(parse[0], parse[1], parse[2], parse[3]);
                    //                   < option value = "cutone" > 每部一分鐘 </ option >

                    //< option value = "cutfive" > 每部五分鐘 </ option >

                    // < option value = "cutten" > 每部十分鐘 </ option >


                }
                catch (Exception ex)
                {
                    log.Error("錯誤" + ex.ToString());
                }

                finally
                {
                    // Force garbage collection
                    log.Info("Mem: " + GC.GetTotalMemory(false).ToString());
                    GC.Collect();
                    log.Info("Mem: " + GC.GetTotalMemory(true).ToString());
                }
            }

        }

        //        對於ffmpeg這個全能多媒體轉換工具，大家應該非常熟悉，就不再介紹了。

        //ffmpeg其中有三種合併功能，大家Google搜尋[ffmpeg 合併] 應該會有一大堆，這次會講講比較少人用的concat filter。

        //老實說光看官方Document的話估計就會一頭霧水。直接看例子：

        //ffmpeg -i opening.mkv -i episode.mkv -i ending.mkv -filter_complex \
        //'[0:0] [0:1] [0:2] [1:0] [1:1] [1:2] [2:0] [2:1] [2:2]
        //concat= n = 3:v= 1:a= 2[v][a1][a2]' \
        //-map '[v]' -map '[a1]' -map '[a2]' output.mkv

        //先不管分行用的\，還有windows要替換成雙引號(“)的(‘)，直接運行的話，如果三個文件resolution跟sar一致的話那還沒問題，萬一有一點不一樣，那肯定要悲劇。

        //剛巧我需要用到這功能，花了一些時間研究後做了以下修改(我只需要一條video跟一條audio，聰明的朋友自己看著上面例子修改應該也不會出問題)

        //ffmpeg.exe -i 001.mp4 -i 002.mp4 -i 003.mp4 -filter_complex "[0:v]scale=1280:720,setsar=1/1[v0];[1:v]scale=1280:720,setsar=1/1[v1];[2:v]scale=1280:720,setsar=1/1[v2];[v0][0:a] [v1][1:a] [v2][2:a] concat=n=3:v=1:a=1 [v] [a]" -map "[v]" -map "[a]" output.mkv

        //一開始輸入3個文件，然後用filter_complex濾鏡

        //[0:v] scale = 1280:720, setsar = 1 / 1[v0] 中[0:v] 代表第0個檔案的video部分，設定成1280*720，sar=1:1 ，之後賦予參數[v0]，其他的照樣

        //         [v0][0:a][v1][1:a][v2][2:a][v0][0:a] 要上面的處理過的[v0]跟第0個檔案的audio

        //      concat = n = 3:v=1:a=1 [v]
        //        [a]
        //        內n=3代表有3個檔案，v=1代表一條video軌，a=1代表一條audio軌，然後就是將video軌跟audio軌分別賦予[v] 跟[a]

        //-map “[v]” -map “[a]” 代表將[v] 跟[a]輸出到對應輸出文件。

        //output.mkv 是輸出檔案，當然也可以在之前加c:v之類的參數控制輸出檔案類型跟編碼參數。

        //p.s.留意，這方法不能套用-an或者-vn參數，因為在filter裏面就指定了一條video跟一條audio。

        private void getShortVideo(string path, string filename, string movieid)
        {
            //實例一個Process類，啟動一個獨立進程
            Process p = new Process();
            string[] pp = filename.Split('.');
            string filehead = "";
            int segCount;
            try
            {
                if (pp[pp.Length - 1].ToUpper() != "MP4")
                {
                    for (int i = 0; i < pp.Length - 1; i++)
                    {
                        filehead = filehead + pp[i];
                    }
                    p.StartInfo.FileName = @"D:\WebProject\bin\ffmpeg\ffmpeg.exe";           //設定程序名
                    p.StartInfo.Arguments = " -i \"" + path + filename + "\"  \"" + path + filehead + ".mp4\"";    //設定程式執行參數
                    p.StartInfo.UseShellExecute = false;        //關閉Shell的使用
                    //p.StartInfo.RedirectStandardInput = true;   //重定向標準輸入
                    //p.StartInfo.RedirectStandardOutput = true;  //重定向標準輸出
                    //p.StartInfo.RedirectStandardError = true;   //重定向錯誤輸出
                    p.StartInfo.CreateNoWindow = true;          //設置不顯示窗口
                    p.Start();   //啟動
                    //設定要等待相關的處理序結束的時間，這邊設定 10秒。 
                    p.WaitForExit();

                    //若應用程式在指定時間內關閉，則 value.HasExited 為 true 。
                    //若是等到指定時間到了都還沒有關閉程式，此時 value.HasExited 為 false，則進入判斷式
                    if (!p.HasExited)
                    {
                        //測試處理序是否還有回應
                        if (!p.Responding)
                        {
                            //立即停止相關處理序。意即，處理序沒回應，強制關閉
                            p.Kill();
                        }
                    }

                    if (System.IO.File.Exists(path + filehead + ".mp4"))
                        changeUploadFileExtension(movieid, filehead + ".mp4");
                    filename = filehead + ".mp4";

                }
                //Process類有一個StartInfo屬性，這個是ProcessStartInfo類，包括了一些屬性和方法，下面我們用到了他的幾個屬性：





                //seqmentation

                var ffProbe = new NReco.VideoInfo.FFProbe();
                var videoInfo = ffProbe.GetMediaInfo(path + filename);
                TimeSpan dur = videoInfo.Duration;
                double dDuration = dur.TotalSeconds;
                segCount = (int)Math.Ceiling(dDuration / (double)segmentTime);
                //Process p = new Process();
                p.StartInfo.FileName = @"D:\WebProject\bin\ffmpeg\ffmpeg.exe";           //設定程序名
                p.StartInfo.Arguments = " -i \"" + path + filename + "\" -map 0 -c copy -f segment -segment_time " + segmentTime.ToString() + " " + path + movieid + "_output_%03d.mp4";    //設定程式執行參數
                p.StartInfo.UseShellExecute = false;        //關閉Shell的使用
                                                            //p.StartInfo.RedirectStandardInput = true;   //重定向標準輸入
                                                            //p.StartInfo.RedirectStandardOutput = true;  //重定向標準輸出
                                                            //p.StartInfo.RedirectStandardError = true;   //重定向錯誤輸出
                p.StartInfo.CreateNoWindow = true;          //設置不顯示窗口
                p.Start();   //啟動
                             //設定要等待相關的處理序結束的時間，這邊設定 10秒。 
                p.WaitForExit();

                //若應用程式在指定時間內關閉，則 value.HasExited 為 true 。
                //若是等到指定時間到了都還沒有關閉程式，此時 value.HasExited 為 false，則進入判斷式
                if (!p.HasExited)
                {
                    //測試處理序是否還有回應
                    if (!p.Responding)
                    {
                        //立即停止相關處理序。意即，處理序沒回應，強制關閉
                        p.Kill();
                    }
                }




                //p.StartInfo.Arguments = " -ss 00:00:35 -i " + path + filename + " -t 00:00:30 " + "-af afade=t=out:st=25:d=5 -vf fade=t=out:st=25:d=5 " + path + "分割影片2.mp4";
                try
                {
                    var dir1 = new DirectoryInfo(path);
                    int i = 0;
                    foreach (var file in dir1.EnumerateFiles(movieid + "_output*.mp4"))
                    {


                        videoInfo = ffProbe.GetMediaInfo(path + movieid + "_output_" + i.ToString("D3") + ".mp4");
                        dur = videoInfo.Duration;
                        int partDuration = (int)dur.TotalSeconds;

                        p.StartInfo.FileName = @"D:\WebProject\bin\ffmpeg\ffmpeg.exe";           //設定程序名
                        p.StartInfo.Arguments = " -i " + path + movieid + "_output_" + i.ToString("D3") + ".mp4" + " -af afade=t=out:st=" + (partDuration - 5).ToString() + ":d=5 -vf fade=t=out:st=" + (partDuration - 5).ToString() + ":d=5 " + path + movieid + "_Seg" + i.ToString("D3") + ".mp4";   //設定程式執行參數
                        p.StartInfo.UseShellExecute = false;        //關閉Shell的使用
                        p.StartInfo.CreateNoWindow = true;          //設置不顯示窗口

                        p.Start();   //啟動
                                     //設定要等待相關的處理序結束的時間，這邊設定 10秒。 
                        p.WaitForExit();

                        //若應用程式在指定時間內關閉，則 value.HasExited 為 true 。
                        //若是等到指定時間到了都還沒有關閉程式，此時 value.HasExited 為 false，則進入判斷式
                        if (!p.HasExited)
                        {
                            //測試處理序是否還有回應
                            if (!p.Responding)
                            {
                                //立即停止相關處理序。意即，處理序沒回應，強制關閉
                                p.Kill();
                            }
                        }
                        i++;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }


                p.StartInfo.FileName = @"D:\WebProject\bin\ffmpeg\ffmpeg.exe";           //設定程序名
                p.StartInfo.Arguments = " -ss 00:00:05 -i \"" + path + filename + "\" -t 00:00:30 " + path + movieid + ".mp4";    //設定程式執行參數
                p.StartInfo.UseShellExecute = false;        //關閉Shell的使用
                //p.StartInfo.RedirectStandardInput = true;   //重定向標準輸入
                //p.StartInfo.RedirectStandardOutput = true;  //重定向標準輸出
                //p.StartInfo.RedirectStandardError = true;   //重定向錯誤輸出
                p.StartInfo.CreateNoWindow = true;          //設置不顯示窗口

                p.Start();   //啟動
                //設定要等待相關的處理序結束的時間，這邊設定 10秒。 
                p.WaitForExit();

                //若應用程式在指定時間內關閉，則 value.HasExited 為 true 。
                //若是等到指定時間到了都還沒有關閉程式，此時 value.HasExited 為 false，則進入判斷式
                if (!p.HasExited)
                {
                    //測試處理序是否還有回應
                    if (!p.Responding)
                    {
                        //立即停止相關處理序。意即，處理序沒回應，強制關閉
                        p.Kill();
                    }
                }


                conn_setup();
                cmmd_setup();
                string sqlstr = "Update movieBase set segmentCnt= " + segCount.ToString() + " where movieid= '" + movieid + "'";
                cmmd.CommandText = sqlstr;
                cmmd.ExecuteNonQuery();
                var dir = new DirectoryInfo(path);

                foreach (var file in dir.EnumerateFiles(movieid + "_output*.mp4"))
                {
                    file.Delete();
                }


            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
                log.Info(ex.StackTrace);

            }
            finally
            {
                cmmd_dispose();
                conn_close();
                if (p != null)
                {
                    p.Close();
                    p.Dispose();
                    p = null;
                }

            }
        }


        private void getImage(string path, string filename, string movieid, string title)
        {
            capture = new VideoCapture(path + filename);
            if (capture == null)
                return;
            Mat m_resize;
            Mat m;
            Mat m_gray;
            try
            {
                while (true)
                {
                    m = new Mat();
                    m = capture.QueryFrame();
                    while (m == null)
                    {
                        m = capture.QueryFrame();
                        double fps = capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);
                        if (fps == 0)
                            CvInvoke.WaitKey(2 * 1000 / 24);
                        else
                            CvInvoke.WaitKey(2 * 1000 / Convert.ToInt32(fps));

                    }
                    m_gray = m.Clone();
                    //capture.Read(m);
                    PictureBox.CheckForIllegalCrossThreadCalls = false;
                    //pictureBox1.Image = m.Bitmap;
                    //CvInvoke.Imwrite(path + movieid + ".jpg", rm);
                    CvInvoke.CvtColor(m, m_gray, ColorConversion.Bgr2Gray);
                    CvInvoke.AdaptiveThreshold(m_gray, m_gray, 255, AdaptiveThresholdType.GaussianC, ThresholdType.BinaryInv, 9, 10);

                    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();

                    CvInvoke.FindContours(m_gray, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                    if (contours.Size > 30)
                    {
                        m_resize = new Mat();

                        CvInvoke.Resize(m, m_resize, new Size(300, 200));
                        //CvInvoke.PutText(m_resize ,title,new Point(10,100),);
                        m.Dispose();

                        break;
                    }
                    double fps1 = capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);
                    if (fps1 == 0)
                        CvInvoke.WaitKey(2 * 1000 / 24);
                    else
                        CvInvoke.WaitKey(2 * 1000 / Convert.ToInt32(fps1));


                }
                capture.Stop();
                capture.Dispose();
                m_resize = PutTextInMat(m_resize, title);
                m_resize.Save(path + movieid + ".jpg");
            }
            catch (Exception ex)
            {
                log.Info(ex.Message);
            }
        }

        Mat PutTextInMat(Mat inMat, string text)
        {
            //System.Drawing.Bitmap bmp;
            int len = text.Length;
            //if (len < 12)
            //{
            //    for (int i = 0; i < 12 - len; i++)
            //    {

            //    }
            //}
            text = "片名:" + text;
            Bitmap bmp = new Bitmap(250, 50);
            Graphics g = Graphics.FromImage(bmp);
            float fsize = 16 * 12 / (text.Length);
            Font drawFont = new Font("標楷體", fsize, FontStyle.Bold);
            g.DrawString(text, drawFont, Brushes.Blue, new PointF(10, 10));
            g.Save();
            //PictureBox.CheckForIllegalCrossThreadCalls = false;
            //pictureBox2.Image = bmp;
            Image<Bgr, Byte> img = inMat.ToImage<Bgr, Byte>();
            for (int i = 0; i < 250; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    Color c = bmp.GetPixel(i, j);
                    if (c.R > 0 || c.B > 0 || c.G > 0)
                    {
                        CvInvoke.cvSet2D(img, j + 75, i + 25, new MCvScalar(c.B, c.G, c.R)); //修改對應圖元值
                    }
                }
            }
            return img.Mat;

        }

        private void changeUploadFileExtension(string movieid, string filename)
        {
            try
            {
                conn_setup();
                string sqlstr = "Update MovieBase set filename= " + getString(filename) + " Where movieid=" + getString(movieid);
                cmmd_setup(sqlstr);
                cmmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            finally
            {
                conn_close();
                cmmd_dispose();
            }

        }



        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            capture = new VideoCapture(lblOrigin.Text);

            if (capture == null)
            {
                return;
            }

            try
            {

                while (!Pause)
                {
                    Mat m = new Mat();
                    capture.Read(m);

                    if (!m.IsEmpty)
                    {
                        pictureBox1.Image = m.Bitmap;
                        Application.DoEvents();
                        double fps = capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);
                        CvInvoke.WaitKey(1000 / Convert.ToInt32(fps));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Pause = true;
        }




        private void btnClip_Click(object sender, EventArgs e)
        {

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
        //int segmentTime = 60;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var ffProbe = new NReco.VideoInfo.FFProbe();
                var videoInfo = ffProbe.GetMediaInfo("input.mp4");
                TimeSpan dur = videoInfo.Duration;
                double dDuration = dur.TotalSeconds;
                int segCount = (int)Math.Ceiling(dDuration / (double)segmentTime);
                Process p = new Process();
                p.StartInfo.FileName = @"D:\WebProject\bin\ffmpeg\ffmpeg.exe";           //設定程序名
                p.StartInfo.Arguments = " -i input.mp4 -map 0 -c copy -f segment -segment_time " + segmentTime.ToString() + " " + "output_%03d.mp4";    //設定程式執行參數
                p.StartInfo.UseShellExecute = false;        //關閉Shell的使用
                                                            //p.StartInfo.RedirectStandardInput = true;   //重定向標準輸入
                                                            //p.StartInfo.RedirectStandardOutput = true;  //重定向標準輸出
                                                            //p.StartInfo.RedirectStandardError = true;   //重定向錯誤輸出
                p.StartInfo.CreateNoWindow = true;          //設置不顯示窗口
                p.Start();   //啟動
                             //設定要等待相關的處理序結束的時間，這邊設定 10秒。 
                p.WaitForExit(1000);

                //若應用程式在指定時間內關閉，則 value.HasExited 為 true 。
                //若是等到指定時間到了都還沒有關閉程式，此時 value.HasExited 為 false，則進入判斷式
                if (!p.HasExited)
                {
                    //測試處理序是否還有回應
                    if (!p.Responding)
                    {
                        //立即停止相關處理序。意即，處理序沒回應，強制關閉
                        p.Kill();
                    }
                }




                //p.StartInfo.Arguments = " -ss 00:00:35 -i " + path + filename + " -t 00:00:30 " + "-af afade=t=out:st=25:d=5 -vf fade=t=out:st=25:d=5 " + path + "分割影片2.mp4";
                for (int i = 0; i < segCount; i++)
                {

                    videoInfo = ffProbe.GetMediaInfo("output_" + i.ToString("D3") + ".mp4");
                    dur = videoInfo.Duration;
                    int partDuration = (int)dur.TotalSeconds;

                    p.StartInfo.FileName = @"D:\WebProject\bin\ffmpeg\ffmpeg.exe";           //設定程序名
                    p.StartInfo.Arguments = " -i " + "output_" + i.ToString("D3") + ".mp4" + " -af afade=t=out:st=" + (partDuration - 5).ToString() + ":d=5 -vf fade=t=out:st=" + (partDuration - 5).ToString() + ":d=5 " + "分割影片" + i.ToString("D3") + ".mp4";   //設定程式執行參數
                    p.StartInfo.UseShellExecute = false;        //關閉Shell的使用
                    p.StartInfo.CreateNoWindow = true;          //設置不顯示窗口

                    p.Start();   //啟動
                                 //設定要等待相關的處理序結束的時間，這邊設定 10秒。 
                    p.WaitForExit(10000);

                    //若應用程式在指定時間內關閉，則 value.HasExited 為 true 。
                    //若是等到指定時間到了都還沒有關閉程式，此時 value.HasExited 為 false，則進入判斷式
                    if (!p.HasExited)
                    {
                        //測試處理序是否還有回應
                        if (!p.Responding)
                        {
                            //立即停止相關處理序。意即，處理序沒回應，強制關閉
                            p.Kill();
                        }
                    }
                }
                //                if (p != null)
                //                {
                //                    p.Close();
                //                    p.Dispose();
                //                    p = null;
                //                }

            }
            catch (Exception ex)
            {
                log.Info(ex.Message);

            }
        }
    }

}




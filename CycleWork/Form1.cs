using System;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using log4net;
using System.Configuration;
[assembly: log4net.Config.XmlConfigurator(Watch = false)]
//This will cause log4net to look for a configuration file
//called ConsoleApp.exe.config in the application base
//directory (i.e. the directory containing ConsoleApp.exe)

namespace CycleWork
{
    public partial class Form1 : Form
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //private static readonly ILog log = LogManager.GetLogger("WebLog");
        String connStr;
        SqlConnection conn;
        SqlCommand cmmd;
        SqlDataAdapter da;
        SqlDataReader dr;
        DataSet ds;
        int hour = -1;
        int day = -1;
        public Form1()
        {
            InitializeComponent();
            connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((DateTime.Now.Hour == 0 || DateTime.Now.Hour == 12) && DateTime.Now.Hour != hour)
            {
                hour = DateTime.Now.Hour;
                CreateHotVideo();
                CreateNewVideo();
                CreateHiVideo();
            }
            if ((DateTime.Now.Day == 1 || DateTime.Now.Day == 15) && DateTime.Now.Hour != 0 && DateTime.Now.Hour != 12 && DateTime.Now.Day!=day)
            {
                day = DateTime.Now.Day;
                MantainTable();
            }
        }

        private void CreateHotVideo()
        {
            string sqlstr = "Select top 5 MovieId from moviebase where uploadTime> GATEDATE()-30 order by numViewed desc";
            conn_setup();
            cmmd_setup(sqlstr);
            try
            {
                dr = cmmd.ExecuteReader();
                int count = 1;
                sqlstr = "";
                while (dr.Read())
                {
                    sqlstr = sqlstr + "Insert into Carousel(Type,movieid,Priority,ShowDate) values('Hot'," + getString(dr["MovieId"].ToString()) + "," + count.ToString() +
                        ", GETDATE());";
                    count = count + 1;
                }
                dr_close();
                if (sqlstr != "")
                {
                    cmmd_setup(sqlstr);
                    cmmd.ExecuteNonQuery();
                }
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
                conn_close();
                cmmd_dispose();
                dr_close();
            }

        }

        private void CreateNewVideo()
        {
            string sqlstr = "Select top 5 MovieId from moviebase order UploadTime desc";
            conn_setup();
            cmmd_setup(sqlstr);
            try
            {
                dr = cmmd.ExecuteReader();
                int count = 1;
                sqlstr = "";
                while (dr.Read())
                {
                    sqlstr = sqlstr + "Insert into Carousel(Type,movieid,Priority,ShowDate) values('New'," + getString(dr["MovieId"].ToString()) + "," + count.ToString() +
                        ", GETDATE());";
                    count = count + 1;
                }
                dr_close();
                if (sqlstr != "")
                {
                    cmmd_setup(sqlstr);
                    cmmd.ExecuteNonQuery();
                }
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
                conn_close();
                cmmd_dispose();
                dr_close();
            }

        }

        private void CreateHiVideo()
        {
            string sqlstr = "Select top 5 MovieId from moviebase where uploadTime> GATEDATE()-30  order by Evaluated desc";
            conn_setup();
            cmmd_setup(sqlstr);
            try
            {
                dr = cmmd.ExecuteReader();
                int count = 1;
                sqlstr = "";
                while (dr.Read())
                {
                    sqlstr = sqlstr + "Insert into Carousel(Type,movieid,Priority,ShowDate) values('Hot'," + getString(dr["MovieId"].ToString()) + "," + count.ToString() +
                        ", GETDATE());";
                    count = count + 1;
                }
                dr_close();
                if (sqlstr != "")
                {
                    cmmd_setup(sqlstr);
                    cmmd.ExecuteNonQuery();
                }
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
                conn_close();
                cmmd_dispose();
                dr_close();
            }

        }

        private void MantainTable()
        {
            string sqlstr = "Delete from Carosoul where ShowDate < GETDATE()-365;Delete from CommercialAD where ShowDateStop < GetDate();";
            conn_setup();
            cmmd_setup(sqlstr);
            try
            {
                    cmmd_setup(sqlstr);
                    cmmd.ExecuteNonQuery();
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
                conn_close();
                cmmd_dispose();
                dr_close();
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
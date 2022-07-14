using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.IO;
[assembly: log4net.Config.XmlConfigurator(Watch = false)]
namespace startService
{
    public partial class Service1 : ServiceBase
    {
        string [] processNameLst={"VideoHandleMerge", "VideoHandle","CycleWork","SignalRHubClient"};
        string[] processPathLst = { "D:\\WebProject\\bin\\VideoHandleMerge\\VideoHandleMerge.exe", "D:\\WebProject\\bin\\VideoHandle\\VideoHandle.exe", "D:\\WebProject\\bin\\CycleWork\\CycleWork.exe", "D:\\WebProject\\bin\\SignalRHubClient\\SignalRHubClient.exe" };

        public Service1()
        {
            InitializeComponent();

        }
        
        public readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool serviceStopping = false;
        protected override void OnStart(string[] args)
        {
            string path = Directory.GetCurrentDirectory();
            log.Info("Service start!");
            for (int i = 0; i < processPathLst.Length; i++)
            {
                try
                {
                    Process[] proc1 = Process.GetProcessesByName(processPathLst[i]); // 已起動
                    if (proc1.Length <= 0)
                    {
                        Process proc = new Process();
                        proc.StartInfo.FileName = processPathLst[i];
                        proc.EnableRaisingEvents = true;
                        proc.Exited += new EventHandler(OnProcessExit);
                        log.Info("起動:" +  proc.StartInfo.FileName);
                        proc.Start();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);

                }
            }
        }

        protected override void OnStop()
        {
            serviceStopping = true;
            for (int i = 0; i < processNameLst.Length; i++)
            {
                try
                {
                    Process[] proc1 = Process.GetProcessesByName(processNameLst[i]);
                    log.Info(proc1.Length.ToString());
                    log.Info(proc1.Length.ToString());
                    for (int j = 0; j < proc1.Length; j++)
                    {
                         log.Info("關閉" + proc1[j].StartInfo .FileName );
                         proc1[j].CloseMainWindow();
                         proc1[j].WaitForExit(500);

                         proc1[j].Kill();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
            }
            log.Info("Service Stop");
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            //DataRow ApRow = null/* TODO Change to default(_) if this is not a reference type */;
            try
            {
                Process proc = (Process)sender;
                log.Info("APStop-" + proc.StartInfo.FileName + "  ExitCode:" + proc.ExitCode + "  ExitTime:" + proc.ExitTime.ToString());
                //// If My.Settings.AutoMode = False Then
                //// Exit Sub
                //// End If
                //if (serviceStopping)
                //    return;
                //ApRow = GetApInfo(proc.ProcessName);

                //if (ApRow == null)
                //    return;

                //if (ApRow(3) == "手動")
                //{
                //    log.error("APMode='M' won't auto restart!!");
                //    return;
                //}
                if (!serviceStopping)
                     proc.Start();
                //log.error(proc.ProcessName + " restart!!");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

    }
}

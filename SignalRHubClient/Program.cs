using Microsoft.AspNet.SignalR.Client.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Messaging;
using log4net;

namespace SignalRHubClient
{
    class Program
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
           bool bClose = false;
            // Almost the same usage as JavaScript without generated proxy
            string connStr = ConfigurationManager.ConnectionStrings["currenturl"].ConnectionString;
            var hubConn = new HubConnection(connStr);
            //var timeHubProxy = hubConn.CreateHubProxy("PascalCasedMyDateTimeHub");
            var chatHubProxy = hubConn.CreateHubProxy("myChatHub");

            //chatHubProxy.On("appendNewMessage", delegate(string name, string message)
            //{
            //    Console.WriteLine("{0}: {1}", name, message);
            //});

            hubConn.Start().Wait();
            MessageQueue myQueue = new MessageQueue();
            StringBuilder sbMsg = new StringBuilder();
            System.Messaging.Message mes = new System.Messaging.Message();
            if (MessageQueue.Exists(".\\Private$\\brocast"))
                myQueue = new MessageQueue(".\\Private$\\brocast");
            else
                myQueue = MessageQueue.Create(".\\Private$\\brocast");

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
                    //Task<DateTime> t = timeHubProxy.Invoke<DateTime>("PascalCasedGetServerDateTime");
                    //t.Wait();
                    //Console.WriteLine((DateTime)t.Result);
                    String[] parse = sbMsg.ToString().Split(';');
                    string user = parse[0];
                    string grpname = parse[1];
                    string message = parse[2];
                    //SendGroupMessage(string callerName, string groupName, string title, string message)
                    chatHubProxy.Invoke("SendGroupMessage", user,grpname, "",message).Wait();
                    Console.WriteLine("{0}: {1}: {2}", user,grpname, message);

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
    }
}

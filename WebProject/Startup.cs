using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(WebProject.Startup))]

namespace WebProject
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            //app.MapSignalR("/myhubs", new HubConfiguration());
            // 如需如何設定應用程式的詳細資訊，請參閱  http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}

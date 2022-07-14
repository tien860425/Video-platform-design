using System;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Messaging;
[assembly: log4net.Config.XmlConfigurator(Watch = false)]

namespace WebProject
{
    public partial class fileupload : System.Web.UI.Page
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected void Page_Load(object sender, EventArgs e)
        {
        }

  
    }
}
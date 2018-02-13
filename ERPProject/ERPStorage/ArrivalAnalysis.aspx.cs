using FineUIPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPStorage
{
    public partial class ArrivalAnalysis : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(tbxDept.ClientID) + Window1.GetShowReference("../ERPQuery/WebForm1.aspx"));
        }
    }
}
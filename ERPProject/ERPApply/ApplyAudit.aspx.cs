using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPApply
{
    public partial class ApplyAudit : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }
        }

        private void DataInit()
        {
            
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {

        }

        protected void btSearch_Click(object sender, EventArgs e)
        {

        }

        protected void GridList_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {

        }
    }
}
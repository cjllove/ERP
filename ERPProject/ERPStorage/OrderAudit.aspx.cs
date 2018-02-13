using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPStorage
{
    public partial class OrderAudit : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void DataInit()
        {
            PubFunc.DdlDataGet(ddlGoodsType, "DDL_GOODS_TYPE");
            dpkOrderDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {

        }

        protected void btSearch_Click(object sender, EventArgs e)
        {

        }
    }
}
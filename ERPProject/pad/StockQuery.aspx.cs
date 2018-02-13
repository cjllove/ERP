using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using XTBase;

namespace ERPProject.pad
{
    public partial class StockQuery : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                PubFunc.DdlDataGet(ddlDEPTID, "DDL_SYS_DEPT");
                DepartmentBind.BindDDL("DDL_SYS_DEPTHASATH", UserAction.UserID);  //ddlDEPTID
                PubFunc.DdlDataGet(ddlSHSID, "DDL_DOC_SUPPLIERALL");
                USERID.Text = UserAction.UserID;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XTBase;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace ERPProject.ERPCart
{
    public partial class CartCount : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserAction != null && UserAction.UserID != null) {
                string gwcSql = string.Format("select nvl(sum(dhs),0） from dat_cart where userid = '{0}'", UserAction.UserID);
                DataTable dt1 = DbHelperOra.Query(gwcSql).Tables[0];
                string result = "0";
                if (dt1.Rows.Count > 0)
                {
                    result = dt1.Rows[0][0].ToString();
                }

                Response.ContentType = "text/plain";
                Response.Write(result);
                Response.End();
            }
                


        }
    }
}
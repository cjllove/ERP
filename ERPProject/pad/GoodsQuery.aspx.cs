using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using XTBase;
using System.Data;

namespace ERPProject.pad
{
    public partial class GoodsQuery : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                PubFunc.DdlDataGet( "DDL_GOODS_TYPE", ddlCATID0, docCATID0);

                PubFunc.DdlDataGet(ddlSUPID, "DDL_DOC_SUPPLIERNULL");
                PubFunc.DdlDataGet(ddlPASSID, "DDL_DOC_PSSNAME");
                PubFunc.DdlDataGet("DDL_GOODS_TYPE",  ddlCATID0);
                PubFunc.DdlDataGet("DDL_DOC_SUPPLIERNULL", trbSUPPLIER);
                PubFunc.DdlDataGet("DDL_PRODUCER", trbPRODUCER);

                PubFunc.DdlDataGet("DDL_UNIT", ddlUNIT, ddlUNIT_DABZ, ddlUNIT_ZHONGBZ);

                //PubFunc.DdlDataGet(ddlJX, "DDL_GOODSJX");
                //PubFunc.DdlDataGet(ddlYX, "DDL_GOODSYX");
                PubFunc.DdlDataGet("DDL_GOODS_STATUS", ddlFLAG, docFLAG);
                //PubFunc.DdlDataGet(ddlCATID, "DDL_SYS_CATLAST");
                string strSql = @"select code,'【'||code||'】'||name name,(class-1) TreeLevel,decode(islast,'Y',1,0) islast
                                    from sys_category
                                    ORDER BY code  ";
                List<CategoryTreeBean> myList = new List<CategoryTreeBean>();
                DataTable categoryTreeTable = DbHelperOra.Query(strSql).Tables[0];
                foreach (DataRow dr in categoryTreeTable.Rows)
                {
                    myList.Add(new CategoryTreeBean(dr["code"].ToString(), dr["name"].ToString(), Convert.ToInt16(dr["TreeLevel"]), Convert.ToInt16(dr["islast"]) == 1));
                }
                // 绑定到下拉列表（启用模拟树功能）
                ddlCATID.EnableSimulateTree = true;
                ddlCATID.DataTextField = "Name";
                ddlCATID.DataValueField = "Id";
                ddlCATID.DataEnableSelectField = "EnableSelect";
                ddlCATID.DataSimulateTreeLevelField = "Level";
                ddlCATID.DataSource = myList;
                ddlCATID.DataBind();
            }
        }
    }
}
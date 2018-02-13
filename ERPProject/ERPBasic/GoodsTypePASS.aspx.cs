using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using XTBase;

namespace ERPProject.ERPBasic
{
    public partial class GoodsTypePASS : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, ddlSTR2);
                cblSTR1.DataTextField = "USERNAME";
                cblSTR1.DataValueField = "USERID";
                cblSTR1.DataSource = DbHelperOra.Query("SELECT USERID,USERNAME FROM SYS_OPERUSER WHERE STATUS='01' AND USERID NOT IN ('admin','system')").Tables[0];
                cblSTR1.DataBind();

                GoodsTypeQuery();
            }
        }

        private void GoodsTypeQuery()
        {
            string sql = "SELECT CODE,NAME,f_getalluser(STR1) STR1,f_getdeptname(STR2) STR2NAME,STR2 FROM DOC_GOODSTYPE ";
            if (tgbSearch.Text.Trim() != "")
            {
                sql += string.Format(" WHERE CODE LIKE '%{0}%' OR NAME LIKE '%{0}%'", tgbSearch.Text.Trim());
            }
            int total = 0;
            GridGoodsType.DataSource = PubFunc.DbGetPage(GridGoodsType.PageIndex, GridGoodsType.PageSize, sql, ref total);
            GridGoodsType.RecordCount = total;
            GridGoodsType.DataBind();
        }

        protected void GridGoodsType_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            PubFunc.FormDataSet(FormGoodsType, GridGoodsType.Rows[e.RowIndex]);

            if (!string.IsNullOrWhiteSpace(GridGoodsType.Rows[e.RowIndex].Values[3].ToString()))
            {
                string getSelectedValue = DbHelperOra.GetSingle("select str1 from DOC_GOODSTYPE t where code='" + GridGoodsType.Rows[e.RowIndex].Values[0].ToString() + "'").ToString();
                cblSTR1.SelectedValueArray = getSelectedValue.Split(',');
            }
        }

        protected void GridGoodsType_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridGoodsType.PageIndex = e.NewPageIndex;
            GoodsTypeQuery();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ddlSTR2.SelectedValue.Length < 1)
            {
                Alert.Show("请选择默认库房", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (cblSTR1.SelectedValueArray.Length > 0)
            {
                string sql = string.Format("UPDATE DOC_GOODSTYPE SET STR1='{0}',STR2='{2}' WHERE CODE='{1}'", string.Join(",", cblSTR1.SelectedValueArray), tbxCODE.Text, ddlSTR2.SelectedValue);
                if (DbHelperOra.ExecuteSql(sql) > 0)
                {
                    Alert.Show("申领审批人保存成功", "消息提示", MessageBoxIcon.Information);
                    GoodsTypeQuery();
                }
                else
                {
                    Alert.Show("申领审批人保存失败", "错误提示", MessageBoxIcon.Error);
                }
            }
            else
            {
                Alert.Show("请选择要审批的人员", "错误提示", MessageBoxIcon.Error);
            }
        }

        protected void tgbSearch_TriggerClick(object sender, EventArgs e)
        {
            GoodsTypeQuery();
        }
    }
}
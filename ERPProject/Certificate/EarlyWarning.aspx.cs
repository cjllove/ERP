using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.Certificate
{
    public partial class EarlyWarning : PageBase
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
            PubFunc.DdlDataGet("DDL_LICENSETYPE_S", lstLICENSETYPE);
            PubFunc.DdlDataGet("DDL_DOC_SUPPLIERNULL", lstSUPID);
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now.AddDays(90);
        }

        private void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【查询日期】！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.SUPNAME,A.LICENSETYPE,A.BEGRQ,A.ENDRQ,F_GETUSERNAME(A.SHR) SHR,A.LRRQ,F_GETUSERNAME(A.LRY) LRY,A.SHRQ,A.MEMO,
                                     DECODE(A.FLAG, 'N', '新增', 'Y', '已审核', '已过期') FLAG,A.LICENSENAME,A.LICENSEID
                                FROM DOC_LICENSE_SUP A WHERE A.ISCUR='Y' ";
            string strSearch = "";

            if (lstLICENSETYPE.SelectedItem != null && lstLICENSETYPE.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.LICENSETYPE='{0}'", lstLICENSETYPE.SelectedItem.Value);
            }
            if (lstSUPID.SelectedItem != null && lstSUPID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.SUPID='{0}'", lstSUPID.SelectedItem.Value);
            }

            strSearch += string.Format(" AND A.ENDRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.ENDRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.SEQNO DESC";
            int total = 0;
            GridList.DataSource = GetDataTable(GridList.PageIndex, GridList.PageSize, strSql, ref total);
            GridList.RecordCount = total;
            GridList.DataBind();
        }

        protected void GridList_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {

        }
        protected void bntClear_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormQuery);
        }

        protected void bntSearch_Click(object sender, EventArgs e)
        {
            billSearch();
        }

        protected void btnSendMessage_Click(object sender, EventArgs e)
        {
            if (GridList.SelectedRowIndexArray.Length == 0)
            {
                Alert.Show("请选择要发送提醒的证照信息！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            GridRow row = GridList.Rows[GridList.SelectedRowIndex];
            lblMessage.Text = string.Format("供应商【{0}】，您的证照【{1}】编号【{2}】将于{3}到期，请尽快更新", row.Values[3].ToString(), row.Values[4].ToString(), row.Values[2].ToString(), row.Values[7].ToString());
            WindowMessage.Hidden = false;
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {

        }

        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }
    }
}
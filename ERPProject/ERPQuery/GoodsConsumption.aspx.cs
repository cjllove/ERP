using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Data;

namespace ERPProject.ERPQuery
{
    public partial class GoodsConsumption : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 在页面第一次加载时 
                BindDDL();
            }
        }

        private Boolean isDg()
        {
            if (Request.QueryString["dg"] == null)
            {
                return false;
            }
            else if (Request.QueryString["dg"].ToString() == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void BindDDL()
        {
            dpkDATE1.SelectedDate = DateTime.Now.AddDays(-1);
            dpkDATE2.SelectedDate = DateTime.Now;

            //PubFunc.DdlDataGet(ddlDEPTID, "DDL_SYS_DEPT"); 
            DepartmentBind.BindDDL("DDL_SYS_DEPTHASATH", UserAction.UserID, ddlDEPTID);
            //代管需要绑定代管供应商，其他绑定非代管供应商
            if (!isDg())
            {
                PubFunc.DdlDataGet("DDL_DOC_SUPPLIERNULL", ddlPSSID);
            }
            else
            {
                //TODO 增加sys_report 代管供应商
                PubFunc.DdlDataGet("DDL_DOC_SUPPLIER_DG", ddlPSSID);
            }
        }

        private string GetSearchSql()
        {
            string strSql = @"SELECT A.SEQNO    ,
                                   A.RQSJ     ,
                                   C.RULENAME BILLTYPE ,
                                   A.BILLNO   ,
                                   A.ROWNO    ,
                                   F.NAME DEPTNAME ,
                                   D.SUPID pssname,
                                   A.GDSEQ    ,
                                   F_GETHISINFO(A.GDSEQ,'GDNAME') GDNAME,
                                   F_GETHISINFO(A.GDSEQ,'GDSPEC') GDSPEC,
                                   e.name CATNAME,
                                   A.SPLB     ,
                                   A.HWID     ,
                                   A.PH       ,
                                   A.YXQZ     ,
                                   DECODE(A.KCADD,1,'增库存','减库存') KCADD,
                                   round(A.JXTAX,4)   JXTAX ,
                                   round(A.LSJ ,4)    LSJ ,
                                   round(A.HSJJ,4)    HSJJ ,
                                   round(A.BHSJJ,4)   BHSJJ ,
                                   round(A.SL,4)      SL ,
                                   round(A.LSJE ,4)    LSJE,
                                   round(A.HSJE ,4)    HSJE,
                                   round(A.BHSJE ,4)  BHSJE ,
                                   A.PZWH     ,
                                   A.RQ_SC    ,
                                   A.ZPBH     ,
                                   A.OPERGH    
                              FROM DAT_GOODSJXC A,
                                   DOC_GOODS B,
                                   (SELECT SUBSTR(RULEID,6,3) BILLTYPE, RULENAME FROM SYS_GLOBRULE WHERE RULEID LIKE 'BILL_%') C,
                                   DOC_SUPPLIER D,
                                   SYS_CATEGORY E,
                                   SYS_DEPT F
                             WHERE A.GDSEQ=B.GDSEQ(+) AND A.BILLTYPE=C.BILLTYPE AND A.PSSID=D.SUPID(+) AND A.CATID=E.CODE(+) AND A.DEPTID=F.CODE(+)";
            string strWhere = " AND A.BILLTYPE ='XSD'";
            strWhere += " AND A.RQSJ>=TO_DATE('" + dpkDATE1.Text + "','YYYY/MM/DD') AND  A.RQSJ< TO_DATE('" + dpkDATE2.Text + "','YYYY/MM/DD') +1 ";
            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " and A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
            //通过供应商区分代管和非代管
            if (isDg())
            {
                strWhere += " AND NVL(D.ISDG,'N') ='Y'";
            }
            else
            {
                strWhere += " AND NVL(D.ISDG,'N') = 'N'";
            }
            if (!PubFunc.StrIsEmpty(ddlPSSID.SelectedValue)) strWhere += " and A.PSSID = '" + ddlPSSID.SelectedValue + "'";

            if (!PubFunc.StrIsEmpty(txbGDSEQ.Text)) strWhere += " and (A.gdseq like '%" + txbGDSEQ.Text + "%' or b.zjm like '%" + txbGDSEQ.Text + "%' or b.gdname like '%" + txbGDSEQ.Text + "%')";

            strWhere += string.Format(" AND a.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            if (strWhere != " ") strSql = strSql + strWhere;
            strSql += string.Format(" ORDER BY {0} {1}", GridGoods.SortField, GridGoods.SortDirection);

            return strSql;
        }

        private void DataSearch()
        {
            if (PubFunc.StrIsEmpty(dpkDATE1.SelectedDate.ToString()) || PubFunc.StrIsEmpty(dpkDATE2.SelectedDate.ToString()))
            {
                Alert.Show("输入日期不正确,请检查！");
                return;
            }
            if (dpkDATE1.SelectedDate > dpkDATE2.SelectedDate)
            {
                Alert.Show("开始日期不能大于结束日期！");
                return;
            }

            int total = 0;

            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, GetSearchSql(), ref total);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }

        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }
        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }
        protected void btClear_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormUser);
            dpkDATE1.SelectedDate = DateTime.Now;
            dpkDATE2.SelectedDate = DateTime.Now;
        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            DataTable dtData = DbHelperOra.Query(GetSearchSql()).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("暂时没有查询到符合条件的进销存数据,无法导出！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            string[] columnNames = new string[GridGoods.Columns.Count - 1];
            for (int index = 1; index < GridGoods.Columns.Count; index++)
            {
                GridColumn column = GridGoods.Columns[index];
                if (column is FineUIPro.BoundField)
                {
                    //dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].DataType = Type.GetType("System.String");
                    dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index - 1] = column.HeaderText;
                }
            }
            //((FineUIPro.Button)sender).Enabled = false;
            XTBase.Utilities.ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), "商品进销存信息", string.Format("商品进销存信息_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));

        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;

            DataSearch();
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System.Data;
using System.Collections.Specialized;
using System.Text;
using XTBase.Utilities;

namespace ERPProject.ERPReport
{
    public partial class ConstantAbnormalReport : BillBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                billSearch();
                billSearch();

            }
        }

        private void DataInit()
        {
            //lstKSSJ.SelectedDate = DateTime.Now.AddDays(-30);
            //lstJSSJ.SelectedDate = DateTime.Now;
            //使用部门下拉表
            PubFunc.DdlDataGet("DDL_SYS_DEPT", lstDEPTID);
            PubFunc.DdlDataSql(lstDEPTID,@"select '' code, '--请选择--' name
  from dual
union all
select code, name
  from (select code, '[' || code || ']' || name name
          from sys_dept
         where type = '3'
           and flag = 'Y'
         order by code)
  ");
            try
            {
                hfdQCRQ.Text = DbHelperOra.GetSingle("select value from SYS_PARA where code = 'YH_DSHS'").ToString();
                QCQX.Text = hfdQCRQ.Text;
            }
            catch
            {
                QCQX.Text = "14";
            }
        }
        protected override void billSearch()
        {
            //if (lstKSSJ.SelectedDate == null || lstJSSJ.SelectedDate == null)
            //{
            //    Alert.Show("请输入条件【录入日期】！");
            //    return;
            //}
            //else if (lstKSSJ.SelectedDate > lstJSSJ.SelectedDate)
            //{
            //    Alert.Show("开始日期大于结束日期，请重新输入！");
            //    return;
            //}
            //string strSql = @"SELECT D.LRRQ,
            //                        F_GETDEPTNAME(D.DEPTID) DEPTID,
            //                        D.BILLNO,
            //                        C.ROWNO,
            //                        C.GDSEQ,
            //                        C.GDNAME,
            //                        C.GDSPEC,
            //                        C.STR2,
            //                        E.DSHL,
            //                        f_getunitname(F.UNIT) UNITNAME,f_getproducername(f.producer) PRODUCERNAME,f.pizno
            //                        FROM DAT_CK_COM C,DAT_CK_DOC D,DAT_GOODSDS_LOG E,DOC_GOODS F
            //                        WHERE C.SEQNO=D.SEQNO AND C.STR2=E.BARCODE AND C.GDSEQ = F.GDSEQ AND NVL(C.STR2,'#') <> '#' AND  (C.SEQNO,C.GDSEQ) IN
            //                        (
            //                        SELECT A.SEQNO,A.GDSEQ  FROM DAT_CK_COM A,DAT_CK_DOC B
            //                        WHERE  A.SEQNO=B.SEQNO AND NVL(A.STR2,'#') <> '#' ";

            string strSql = @"
                                SELECT A.GDSEQ,
                                       B.GDNAME,
                                       B.GDSPEC,
                                       F_GETUNITNAME(B.UNIT) UNITNAME,
                                       F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                       F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,
                                       F_GETDEPTNAME(A.DEPTIN) DEPTIN,
                                       A.DSHL,
                                       A.SL,
                                       B.HSJJ HSJJ,
                                       B.HSJJ*A.DSHL HSJJJE,                                       
                                       A.BARCODE,
                                       TO_CHAR(A.OUTRQ,'YYYY-MM-DD') OUTRQ,
                                       A.OUTBILLNO,
                                       FLOOR(SYSDATE - A.OUTRQ) QX
                                  FROM DAT_GOODSDS_LOG A, DOC_GOODS B
                                 WHERE A.GDSEQ = B.GDSEQ
                                   AND A.FLAG = 'N'";
            string strSearch = "";
            string strSearchOrder = " ";
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedIndex > 0)
            {
                strSearch += string.Format("  AND A.DEPTIN LIKE '%{0}%' ", lstDEPTID.SelectedItem.Value);
            }
            if (QCQX.Text.Trim() == "") QCQX.Text = "0";
            strSearch += " AND FLOOR(SYSDATE - A.OUTRQ) >=" + Convert.ToInt32(QCQX.Text.Trim());
            //strSearch += string.Format(" AND A.OUTRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstKSSJ.Text);
            //strSearch += string.Format(" AND A.OUTRQ< TO_DATE('{0}','YYYY-MM-DD') + 1", lstJSSJ.Text);
            strSearch += string.Format(" AND A.DEPTIN IN( SELECT CODE FROM SYS_DEPT WHERE F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);

            strSearchOrder += string.Format(" ORDER BY A.OUTRQ DESC");
            strSql += strSearch;
            strSql += strSearchOrder;

            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSql, ref total);
            GridList.DataSource = dt;
            GridList.RecordCount = total;
            GridList.DataBind();

        }
        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            //lstKSSJ.SelectedDate = DateTime.Now.AddDays(-30);
            //lstJSSJ.SelectedDate = DateTime.Now;
            lstDEPTID.SelectedValue = "0";
            QCQX.Text = hfdQCRQ.Text;
            GridList.Rows.Clear();
        }
        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            DataTable dtData = DbHelperOra.Query(GetSearchSql()).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string[] columnNames = new string[GridList.Columns.Count - 1];
            for (int index = 1; index < GridList.Columns.Count; index++)
            {
                GridColumn column = GridList.Columns[index];
                if (column is FineUIPro.BoundField)
                {

                    dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index - 1] = column.HeaderText;
                }
            }

            ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), "商品库存信息", string.Format("商品库存报表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));

        }
        private string GetSearchSql()
        {
            string strSql = @" select a.GDSEQ,
                                       b.gdname,
                                       b.gdspec,
                                       F_GETUNITNAME(b.unit) unitname,
                                       F_GETPRODUCERNAME(b.producer) producername,
                                       F_GETDEPTNAME(a.DEPTOUT) DEPTOUT,
                                       F_GETDEPTNAME(a.DEPTIN) DEPTIN,
                                       a.DSHL,
                                       a.SL,
                                       cast(b.HSJJ as number(8,4)) HSJJ,
                                      cast((b.HSJJ*a.DSHL) as number(8,4)) HSJJJE,  
                                       a.BARCODE,
                                       to_char(A.OUTRQ,'YYYY-MM-DD') OUTRQ,
                                       a.OUTBILLNO,
                                       floor(SYSDATE - A.OUTRQ) QX
                                  from DAT_GOODSDS_LOG a, doc_goods b
                                 WHERE a.gdseq = b.gdseq
                                   and a.FLAG = 'N'
                                   AND floor(SYSDATE - A.OUTRQ) > (select value from SYS_PARA where code='YH_DSHS')
";
            string strSearch = "";
            string strSearchOrder = " ";
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedIndex > 0)
            {
                strSearch += string.Format("  AND a.DEPTIN LIKE '%{0}%' ", lstDEPTID.SelectedItem.Value);
            }
            if (QCQX.Text.Trim() == "") QCQX.Text = "0";
            strSearch += " and floor(SYSDATE - A.OUTRQ) >=" + Convert.ToInt32(QCQX.Text.Trim());
            //strSearch += string.Format(" AND A.OUTRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstKSSJ.Text);
            //strSearch += string.Format(" AND A.OUTRQ< TO_DATE('{0}','YYYY-MM-DD') + 1", lstJSSJ.Text);
            strSearch += string.Format(" AND a.DEPTIN in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);

            strSearchOrder += string.Format(" order by floor(SYSDATE - A.OUTRQ) desc");
            strSql += strSearch;
            strSql += strSearchOrder;


            return strSql;
        }



    }
}
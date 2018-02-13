using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Data;
using Newtonsoft.Json.Linq;

namespace ERPProject.ERPQuery
{
    public partial class DeptSh : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 在页面第一次加载时 
                BindDDL();
                DataSearch();
            }
        }
        private void BindDDL()
        {
            dpkDATE1.SelectedDate = DateTime.Now.AddMonths(-3);
            dpkDATE2.SelectedDate = DateTime.Now;
            lisDATE1.SelectedDate = DateTime.Now.AddMonths(-3);
            lisDATE2.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID, ddlDEPTID);
        }

        private void DataSearch()
        {
            if (PubFunc.StrIsEmpty(dpkDATE1.SelectedDate.ToString()) || PubFunc.StrIsEmpty(dpkDATE2.SelectedDate.ToString()))
            {
                Alert.Show("【输入日期】不正确,请检查！", MessageBoxIcon.Warning);
                return;
            }
            if (dpkDATE1.SelectedDate > dpkDATE2.SelectedDate)
            {
                Alert.Show("【开始日期】不能大于【结束日期】！", MessageBoxIcon.Warning);
                return;
            }

            string strSql = @"SELECT K.*,f_getdeptname(K.DEPTID) DEPTIDNAME,NVL(F.SHJE,0) SHJE,NVL(F.SHCS,0) SHCS,NVL(F.SHPG,0) SHPG,
                        ROUND(NVL(F.SHJE,0)/DHJE,4) SLMZ,ROUND(NVL(F.SHCS,0)/DHCS,4) CSMZ,ROUND(NVL(F.SHPG,0)/DHPG,4) PGMZ,
                        NVL(G.DSPG,0) DSPG,NVL(G.DSSL,0) DSSL,
                        F_GETDEPTBL('KSPGHB','{0}','{1}',K.DEPTID,'','','','') PGHB,F_GETDEPTBL('KSPGTB','{0}','{1}',K.DEPTID,'','','','') PGTB,
                        F_GETDEPTBL('KSSLHB','{0}','{1}',K.DEPTID,'','','','') SLHB,F_GETDEPTBL('KSSLTB','{0}','{1}',K.DEPTID,'','','','') SLTB,
                        F_GETDEPTBL('KSSHHB','{0}','{1}',K.DEPTID,'','','','') SHHB,F_GETDEPTBL('KSSHTB','{0}','{1}',K.DEPTID,'','','','') SHTB
                        FROM
                        (SELECT A.DEPTID,SUM(B.DHSL*B.HSJJ) DHJE,COUNT(DISTINCT(A.SEQNO)) DHCS,COUNT(DISTINCT(B.GDSEQ)) DHPG
                        FROM DAT_SL_DOC A,DAT_SL_COM B,DOC_GOODS DG
                        WHERE A.SEQNO = B.SEQNO AND B.GDSEQ=DG.GDSEQ AND A.FLAG IN('Y','S','B','G','D','W') AND DG.ISGZ LIKE(NVL('{2}','%'))
                        AND A.SHRQ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1
                        GROUP BY A.DEPTID) K,
                        (SELECT DGJ.DEPTID,SUM(DGJ.SL*DGJ.HSJJ) SHJE,COUNT(DISTINCT(DGJ.SEQNO)) SHCS,COUNT(DISTINCT(DGJ.GDSEQ)) SHPG
                        FROM DAT_GOODSJXC DGJ,DOC_GOODS DG,SYS_DEPT SD
                        WHERE DGJ.GDSEQ=DG.GDSEQ AND DGJ.DEPTID=SD.CODE AND DG.ISGZ LIKE(NVL('{2}','%'))  AND DGJ.BILLTYPE  IN('LCD','CKD','DSC')
                        AND DGJ.RQSJ BETWEEN TO_DATE('{0}','yyyy-MM-dd') AND TO_DATE('{1}','yyyy-MM-dd') + 1 AND SD.TYPE<>1
                        GROUP BY DGJ.DEPTID) F,
                        (SELECT A.DEPTID,COUNT(DISTINCT(A.GDSEQ)) DSPG,SUM((A.DSNUM - A.NUM3)*A.NUM1) DSSL
                        FROM DOC_GOODSCFG A
                        WHERE A.Dsnum > 0 AND A.NUM1 > 0 
                        GROUP BY A.DEPTID) G
                        WHERE K.DEPTID = F.DEPTID(+) AND K.DEPTID = G.DEPTID(+)";
            string strWhere = "";
            if (lstDEPTID.SelectedValue.Length > 0) strWhere += " AND K.DEPTID = '" + lstDEPTID.SelectedValue + "'";
            if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
            string sortField = GridGoods.SortField;
            string sortDirection = GridGoods.SortDirection;
            string ss = string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, ddlISGZ.SelectedValue) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection);
            DataTable dt = DbHelperOra.QueryForTable(string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text, ddlISGZ.SelectedValue) + String.Format(" ORDER BY {0} {1}", sortField, sortDirection));
            GridGoods.DataSource = dt;
            GridGoods.DataBind();

            JObject summary = new JObject();
            hfdArray.Text = "";
            hfdArrayVal.Text = "";
            hfdArrayVal2.Text = "";
            hfdArrayVal3.Text = "";
            Decimal num1 = 0, num2 = 0, num3 = 0, num4 = 0, num5 = 0, num6 = 0, num7 = 0, num8 = 0, total = 0, total1 = 0, total2 = 0;
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                num1 += Convert.ToDecimal(dr["DHPG"]);
                num2 += Convert.ToDecimal(dr["SHPG"]);
                num3 += Convert.ToDecimal(dr["DHJE"]);
                num4 += Convert.ToDecimal(dr["SHJE"]);
                num5 += Convert.ToDecimal(dr["SHCS"]);
                num6 += Convert.ToDecimal(dr["DHCS"]);
                num7 += Convert.ToDecimal(dr["DSPG"]);
                num8 += Convert.ToDecimal(dr["DSSL"]);
                if (i < 6)
                {
                    hfdArray.Text += dr["DEPTIDNAME"] + ",";
                    hfdArrayVal.Text += dr["DHPG"] + "$" + dr["DEPTIDNAME"] + ",";
                    hfdArrayVal2.Text += dr["DHJE"] + "$" + dr["DEPTIDNAME"] + ",";
                    hfdArrayVal3.Text += dr["DHCS"] + "$" + dr["DEPTIDNAME"] + ",";
                }
                else
                {
                    total += Convert.ToDecimal(dr["DHPG"]);
                    total1 += Convert.ToDecimal(dr["DHJE"]);
                    total2 += Convert.ToDecimal(dr["DHCS"]);
                }
                i++;
            }
            if (total > 0)
            {
                hfdArray.Text += "其他,";
                hfdArrayVal.Text += total.ToString() + "$其他,";
                hfdArrayVal2.Text += total1.ToString() + "$其他,";
                hfdArrayVal3.Text += total2.ToString() + "$其他,";
            }
            hfdArray.Text = hfdArray.Text.TrimEnd(',');
            hfdArrayVal.Text = hfdArrayVal.Text.TrimEnd(',');
            hfdArrayVal2.Text = hfdArrayVal2.Text.TrimEnd(',');
            hfdArrayVal3.Text = hfdArrayVal3.Text.TrimEnd(',');
            summary.Add("DEPTIDNAME", "本页合计");
            summary.Add("DHPG", num1.ToString("F2"));
            summary.Add("SHPG", num2.ToString("F2"));
            summary.Add("DHSL", num3.ToString("F2"));
            summary.Add("SHSL", num4.ToString("F2"));
            summary.Add("SHCS", num5.ToString("F2"));
            summary.Add("DHCS", num6.ToString("F2"));
            summary.Add("DSPG", num7.ToString("F2"));
            summary.Add("DSSL", num8.ToString("F2"));
            GridGoods.SummaryData = summary;
            PageContext.RegisterStartupScript("getEcharsData();getEcharsData2();getEcharsData3();");
        }
        protected void btnSch_Click(object sender, EventArgs e)
        {
            if (PubFunc.StrIsEmpty(lisDATE1.SelectedDate.ToString()) || PubFunc.StrIsEmpty(lisDATE2.SelectedDate.ToString()))
            {
                Alert.Show("【输入日期】不正确,请检查！", MessageBoxIcon.Warning);
                return;
            }
            if (lisDATE1.SelectedDate > lisDATE2.SelectedDate)
            {
                Alert.Show("【开始日期】不能大于【结束日期】！", MessageBoxIcon.Warning);
                return;
            }
            string strSql = @"SELECT F.*,f_getdeptname(K.DEPTID) DEPTIDNAME
                        FROM
                                   (SELECT A.DEPTID,SUM(B.DHSL) DHSL,COUNT(DISTINCT(A.SEQNO)) DHCS,COUNT(DISTINCT(B.GDSEQ)) DHPG
                      FROM DAT_SL_DOC A, DAT_SL_COM B, DOC_GOODS DG
                     WHERE A.SEQNO = B.SEQNO
                       AND B.GDSEQ = DG.GDSEQ
                       AND A.FLAG IN ('Y', 'S', 'B', 'G', 'D', 'W')
                       AND A.SHRQ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
                           TO_DATE('{1}', 'yyyy-MM-dd') + 1                       
                           AND DG.ISGZ LIKE (NVL('{2}', '%'))
                           AND (DG.GDSEQ LIKE '%{3}%' OR DG.GDNAME LIKE '%{3}%' OR DG.ZJM LIKE '%{3}%') 
                           GROUP BY A.DEPTID
                   ) K,
                   (SELECT  DGJ.DEPTID, DGJ.ROWNO, DGJ.GDSEQ, DGJ.HSJJ, DGJ.HSJE,  DGJ.PZWH,DG.GDSPEC,
               DGJ.SL BZSL,DGJ.RQSJ SHRQ,DGJ.BILLNO SEQNO,DG.GDNAME,f_getunitname(DG.UNIT)UNITNAME,f_getproducername(DG.PRODUCER)PRODUCERNAME
                 
                      FROM DAT_GOODSJXC DGJ, DOC_GOODS DG, SYS_DEPT SD
                     WHERE DGJ.GDSEQ = DG.GDSEQ
                       AND DGJ.DEPTID = SD.CODE
                       AND DGJ.BILLTYPE IN ('LCD', 'CKD', 'DSC')
                       AND DGJ.RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
                           TO_DATE('{1}', 'yyyy-MM-dd') + 1
                           AND DG.ISGZ LIKE (NVL('{2}', '%'))
                           AND (DG.GDSEQ LIKE '%{3}%' OR DG.GDNAME LIKE '%{3}%' OR DG.ZJM LIKE '%{3}%') 
                       AND SD.TYPE <> 1
                     ) F,
                   (SELECT A.DEPTID,
                           COUNT(DISTINCT(A.GDSEQ)) DSPG,
                           SUM((A.DSNUM - A.NUM3) * A.NUM1) DSSL
                      FROM DOC_GOODSCFG A
                     WHERE A.DSNUM > 0
                       AND A.NUM1 > 0
                     GROUP BY A.DEPTID) G
             WHERE K.DEPTID = F.DEPTID(+)
               AND K.DEPTID = G.DEPTID(+) AND K.DEPTID='{4}'";
            string strWhere = "";
            if (ddlDEPTID.SelectedValue.Length > 1) strWhere += " AND A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
               int total = 0;
            string sortField = GridList.SortField;
            string sortDirection = GridList.SortDirection;
            string ss = string.Format(strSql, lisDATE1.Text, lisDATE2.Text, lstISGZ.SelectedValue, lisGDSEQ.Text, ddlDEPTID.SelectedValue);
            DataTable dtData = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, string.Format(strSql, lisDATE1.Text, lisDATE2.Text, lstISGZ.SelectedValue, lisGDSEQ.Text, ddlDEPTID.SelectedValue) , ref total);
            GridList.RecordCount = total;
            GridList.DataSource = dtData;
            GridList.DataBind();
            //String.Format(" ORDER BY {0} {1}", sortField, sortDirection)
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
            if (TabStrip1.ActiveTabIndex == 0)
            {
                dpkDATE1.SelectedDate = DateTime.Now.AddMonths(-3);
                dpkDATE2.SelectedDate = DateTime.Now;
                lstDEPTID.SelectedValue = "";
                ddlISGZ.SelectedValue = "";
            }
            if (TabStrip1.ActiveTabIndex == 1)
            {
                lisDATE1.SelectedDate = DateTime.Now.AddMonths(-3);
                lisDATE2.SelectedDate = DateTime.Now;
                lisGDSEQ.Text = String.Empty;
                lstISGZ.SelectedValue = "";
            }
        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            if (TabStrip1.ActiveTabIndex == 0)
            {
                if (GridGoods.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=科室排行.xls");
                Response.ContentType = "application/excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Write(PubFunc.GridToHtml(GridGoods));
                Response.End();
                btnExp.Enabled = true;
            }
            if (TabStrip1.ActiveTabIndex == 1)
            {
                if (GridList.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=科室明细排行.xls");
                Response.ContentType = "application/excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Write(PubFunc.GridToHtml(GridList));
                Response.End();
                btnExp.Enabled = true;
            }
        }

        protected void GridGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            ddlDEPTID.SelectedValue = GridGoods.DataKeys[e.RowIndex][0].ToString();
            lstISGZ.SelectedValue = ddlISGZ.SelectedValue;
            lisDATE1.SelectedDate = dpkDATE1.SelectedDate;
            lisDATE2.SelectedDate = dpkDATE2.SelectedDate;
            TabStrip1.ActiveTabIndex = 1;
            lisGDSEQ.Text = String.Empty;
            btnSch_Click(null, null);
        }

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            btnSch_Click(null, null);
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            DataSearch();
        }
    }
}
﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPPayment
{
    public partial class Settlement : BillBase
    {
        private string strDocSql = "select billno,SEQNO, LRY,F_GETUSERNAME(LRY) LRYNAME,LRRQ,SHR,F_GETUSERNAME(SHR) SHRNAME,SHRQ,F_GET_BILLTYPENAME(BILLTYPE) BILLTYPENAME,F_GETDEPTNAME(DEPTID) DEPTIDNAME,DEPTOUT,F_GETDEPTNAME(DEPTOUT) DEPTOUTNAME,decode(FLAG,'Y','已审核','G','已执行','未结算') FLAGNAME,DECODE(XSTYPE,'1','申领','申退') XSTYPENAME,PRICE_HSJE,PRICE_RTN,PRICE from DAT_JSD_BILL where seqno='{0}' order by BILLNO,SHRQ desc";
        private string strLisSQL = "SELECT SEQNO,DEPTID,SUPID,BEGRQ,ENDRQ,LRY,FLAG,decode(FLAG,'Y','已审核','G','已付款','R','驳回','未结算') FLAGNAME,LRRQ,YJJJ,MEMO FROM dat_jsd_doc WHERE SEQNO = '{0}' and DEPTID = '{1}' ORDER by SEQNO DESC";
        public Settlement()
        {
            BillType = "JSD";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                billNew();
            }
        }
        protected override void billNew()
        {
            GridGoods.DataSource = null;
            GridGoods.DataBind();
            PubFunc.FormLock(FormDoc, false);
            docSEQNO.Text = string.Empty;
            docSEQNO.Enabled = false;
            docFLAG.Enabled = false;
            docLRY.Enabled = false;
            tbxYJJJ.Enabled = false;
            btnCreate.Enabled = true;
            tbxMEMO.Text = string.Empty;
            tbxYJJJ.Text = string.Empty;
        }
        private void DataInit()
        {
            PubFunc.DdlDataGet("DDL_USER", docLRY);
            PubFunc.DdlDataGet("DDL_BILL_STATUSJSD", docFLAG);
            PubFunc.DdlDataGet("DDL_DOC_SUPID", ddlSUPID, lstSUPID);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            docJSKS.SelectedDate = DateTime.Now.AddDays(-1);
            docJSJS.SelectedDate = DateTime.Now;
            ddlSUPID.SelectedValue = Doc.DbGetSysPara("SUPPLIER");
        }

        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【结算日期】！");
                return;
            }

            string strSql = @"SELECT 
                                CUSTID,SEQNO,DEPTID,F_GETDEPTNAME(DEPTID) DEPTIDNAME,SUPID,SUPNAME,FLAG,DECODE(FLAG,'Y','已审核','G','已付款','R','驳回','未结算') FLAGNAME,GATFUNDCORP,GATFUNDBANK,GATACCNTNO,LINKMAN,LINKTEL,
                                CWLINKMAN,CWLINKTEL,BEGRQ,ENDRQ,NVL(SYJE,0) SYJE,NVL(XSJE,0) XSJE,NVL(THJE,0) THJE,NVL(JSJE,0) JSJE,FPJE,FPHM,LRY,LRRQ,SPR,SPRQ,SHR,NVL(YJJJ,0) YJJJ,JSJE,WJJE,
                                SHRQ,MEMO,STR1,STR2,STR3,NUM1,NUM2,NUM3,UPTTIME     
                                FROM DAT_JSD_DOC WHERE SEQNO IN (SELECT DISTINCT SEQNO FROM DAT_JSD_COM) ";
            string strSearch = "";
            if (lstBILLNO.Text.Length > 0)
            { strSearch += string.Format(" AND SEQNO  = '{0}'", lstBILLNO.Text); }
            if (lstSUPID.SelectedItem.Value.Length > 0)
            { strSearch += string.Format(" AND SUPID='{0}'", lstSUPID.SelectedItem.Value); }
            strSearch += string.Format(" AND ((BEGRQ BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND (TO_DATE('{1}','YYYY-MM-DD')) + 1) OR (ENDRQ BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND (TO_DATE('{1}','YYYY-MM-DD') + 1)))", lstLRRQ1.Text, lstLRRQ2.Text);
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY DEPTID,ENDRQ DESC";
            DataTable dtBill = new DataTable();
            dtBill = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataSource = dtBill;
            GridList.DataBind();
            //计算合计数量
            decimal ddslTotal = 0, bzslTotal = 0, feTotal = 0, feeTotal = 0;
            foreach (DataRow row in dtBill.Rows)
            {
                ddslTotal += Convert.ToDecimal(row["SYJE"]);
                bzslTotal += Convert.ToDecimal(row["XSJE"]);
                feeTotal += Convert.ToDecimal(row["THJE"]);
                feTotal += Convert.ToDecimal(row["YJJJ"]);
            }
            JObject summary = new JObject();
            summary.Add("FLAGNAME", "本页合计");
            summary.Add("SYJE", ddslTotal.ToString("F2"));
            summary.Add("XSJE", bzslTotal.ToString("F2"));
            summary.Add("THJE", feeTotal.ToString("F2"));
            summary.Add("YJJJ", feTotal.ToString("F2"));
            GridList.SummaryData = summary;
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].DataKeys[0].ToString());
        }

        protected override void billOpen(string strBillno)
        {
            string sql = @"SELECT T.SEQNO, T.ROWNO, T.GDSEQ, T.GDNAME,
                                              T.GDSPEC, T.BZSL, T.HSJJ, T.HSJE, 
                                              DECODE(T.ISGZ, 'Y', '是', '否') ISGZ,
                                              F_GETUNITNAME(T.UNIT) UNITNAME,
                                              F_GETPRODUCERNAME(T.PRODUCER) PRODUCERNAME
                                     FROM DAT_JSD_COM T WHERE T.SEQNO='{0}'";
            GridJSD.DataSource = DbHelperOra.Query(string.Format(sql, strBillno));
            GridJSD.DataBind();

            winJSD.Hidden = false;
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ddlSUPID.SelectedValue))
            {
                Alert.Show("请选择要结算的供应商！", "异常提醒", MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(docSEQNO.Text))
            {
                docSEQNO.Text = BillSeqGet();
            }
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"DECLARE
                                                BEGIN
                                                  INSERT INTO DAT_JSD_COM
                                                    (SEQNO,
                                                     ROWNO,
                                                     GDSEQ,
                                                     BARCODE,
                                                     GDNAME,
                                                     UNIT,
                                                     GDSPEC,
                                                     GDMODE,
                                                     BZSL,
                                                     JXTAX,
                                                     HSJJ,
                                                     HSJE,
                                                     ISGZ,
                                                     ISLOT,
                                                     PZWH,
                                                     PRODUCER,
                                                     ZPBH)
                                                    SELECT '{0}',
                                                           ROWNUM,
                                                           A.GDSEQ,
                                                           A.BARCODE,
                                                           A.GDNAME,
                                                           A.UNIT,
                                                           A.GDSPEC,
                                                           A.GDMODE,
                                                           B.BZSL,
                                                           A.JXTAX,
                                                           B.HSJJ,
                                                           B.BZSL * B.HSJJ,
                                                           A.ISGZ,
                                                           A.ISLOT,
                                                           A.PIZNO,
                                                           A.PRODUCER,
                                                           A.ZPBH
                                                      FROM DOC_GOODS A,
                                                           (SELECT J.GDSEQ, J.HSJJ, -(SUM(J.SL)) BZSL
                                                              FROM DAT_GOODSJXC J
                                                             WHERE J.PSSID = '{1}'
                                                               AND (J.BILLTYPE IN ('XSD', 'XSG', 'DSH') OR
                                                                   (J.BILLTYPE = 'XST' AND J.KCADD = '1'))
                                                               AND NVL(J.STR1,'#') = '#'
                                                               AND J.DEPTID IN
                                                                   (SELECT CODE FROM SYS_DEPT WHERE TYPE IN ('3', '4'))
                                                               AND J.RQSJ BETWEEN TO_DATE('{2}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{3}', 'YYYY-MM-DD') + 1
                                                             GROUP BY J.GDSEQ, J.HSJJ) B
                                                     WHERE A.GDSEQ = B.GDSEQ
                                                       AND B.BZSL > 0
                                                     ORDER BY A.GDSEQ;
                                                  IF SQL%ROWCOUNT = 0 THEN
                                                    RAISE_APPLICATION_ERROR(-20001, '未查询到符合条件的结算数据！');
                                                  END IF;
                                                  INSERT INTO DAT_JSD_DOC
                                                    (CUSTID,
                                                     SEQNO,
                                                     SUPID,
                                                     SUPNAME,
                                                     FLAG,
                                                     GATFUNDCORP,
                                                     BEGRQ,
                                                     ENDRQ,
                                                     XSJE,
                                                     LRY,
                                                     LRRQ,
                                                     UPTTIME,
                                                     YJJJ)
                                                    SELECT F_GETPARA('USERCODE'),
                                                           '{0}',
                                                           '{1}',
                                                           '{4}',
                                                           'G',
                                                           '{4}',
                                                           TO_DATE('{2}', 'YYYY-MM-DD'),
                                                           TO_DATE('{3}', 'YYYY-MM-DD') ,
                                                           SUM(T.BZSL * T.HSJJ),
                                                           '{5}',
                                                           SYSDATE,
                                                           SYSDATE,
                                                           SUM(T.BZSL * T.HSJJ)
                                                      FROM DAT_JSD_COM T
                                                     WHERE T.SEQNO = '{0}';
                                                    --反填结算单号
                                                    UPDATE DAT_GOODSJXC J SET J.STR1='{0}'
                                                     WHERE J.PSSID = '{1}'
                                                       AND NVL(J.STR1,'#') = '#'
                                                       AND (J.BILLTYPE IN ('XSD', 'XSG', 'DSH') OR
                                                           (J.BILLTYPE = 'XST' AND J.KCADD = '1'))
                                                       AND J.DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE TYPE IN ('3', '4'))
                                                       AND J.RQSJ BETWEEN TO_DATE('{2}', 'YYYY-MM-DD') AND
                                                           TO_DATE('{3}', 'YYYY-MM-DD') + 1 ;
                                                END;
                ", docSEQNO.Text.Trim(), ddlSUPID.SelectedValue, docJSKS.Text, docJSJS.Text, ddlSUPID.SelectedText, docLRY.SelectedValue);
            try
            {
                DbHelperOra.ExecuteSql(sbSql.ToString());

                Alert.Show("结算单据【" + docSEQNO.Text + "】生成成功!", "消息提示", MessageBoxIcon.Information);
                //if (DbHelperOra.ExecuteSqlTran(cmdList))
                //{
                //    Alert.Show("结算单据【" + docSEQNO.Text + "】生成成功!", "消息提示", MessageBoxIcon.Information);
                //}
                //else
                //{
                //    Alert.Show("结算单生成失败,请检查系统信息！", "异常提醒", MessageBoxIcon.Warning);
                //}
            }
            catch (Exception ex)
            {
                Alert.Show(ERPUtility.errorParse(ex.Message), "错误提示", MessageBoxIcon.Error);
            }
        }

        private string GetSearch()
        {
            string sup = "";
            if (!string.IsNullOrWhiteSpace(ddlSUPID.SelectedValue))
            {
                sup = " AND J.PSSID = '" + ddlSUPID.SelectedValue + "'";
            }
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT A.GDSEQ,A.BARCODE,A.GDNAME,
                                                           A.UNIT,F_GETUNITNAME(A.UNIT) UNITNAME,
                                                           A.GDSPEC,A.GDMODE, B.BZSL,
                                                           A.CATID,F_GETCATNAME(A.CATID) CATNAME,
                                                           A.JXTAX,B.HSJJ,B.BZSL * B.HSJJ HSJE,
                                                           DECODE(A.ISGZ,'Y','是','否') ISGZ,
                                                           A.ISLOT,A.PIZNO,A.PRODUCER,
                                                           F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                                           B.PSSID SUPID,
                                                           F_GETSUPNAME(B.PSSID) SUPNAME, A.PIZNO PZWH
                                                 FROM DOC_GOODS A,
                                                      (SELECT J.PSSID,J.GDSEQ, J.HSJJ, -(SUM(J.SL)) BZSL
                                                         FROM DAT_GOODSJXC J
                                                        WHERE  (J.BILLTYPE IN ('XSD', 'XSG', 'DSH') OR
                                                              (J.BILLTYPE = 'XST' AND J.KCADD = '1'))
                                                          AND J.DEPTID IN
                                                              (SELECT CODE FROM SYS_DEPT WHERE TYPE IN ('3', '4'))
                                                          AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                              TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                          {2}
                                                         AND NVL(J.STR1,'#') = '#'
                                                        GROUP BY J.PSSID,J.GDSEQ, J.HSJJ) B
                                                WHERE A.GDSEQ = B.GDSEQ
                                                  AND B.BZSL > 0
                                                ORDER BY A.SUPPLIER,A.GDNAME
                                               ", docJSKS.Text, docJSJS.Text, sup);
            return sbSql.ToString();
        }
        protected void btnSrh_Click(object sender, EventArgs e)
        {
            DataTable table = DbHelperOra.Query(GetSearch()).Tables[0];
            //查询需结算单据
            GridGoods.DataSource = table;
            GridGoods.DataBind();

            decimal bzslTotal = 0, hsjeTotal = 0, bzsl = 0, hsje = 0;
            foreach (DataRow row in table.Rows)
            {
                bzsl = 0;
                hsje = 0;
                decimal.TryParse(row["BZSL"].ToString(), out bzsl);
                decimal.TryParse(row["HSJE"].ToString(), out hsje);
                bzslTotal += bzsl;
                hsjeTotal += hsje;
            }
            JObject summary = new JObject();
            summary.Add("FLAGNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString("F2"));
            summary.Add("HSJE", hsjeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
        }

        protected void lstBILLNO_TriggerClick(object sender, EventArgs e)
        {
            billSearch();
        }

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            GridList.SortDirection = e.SortDirection;
            GridList.SortField = e.SortField;

            DataView view1 = PubFunc.GridDataGet(GridList).DefaultView;
            view1.Sort = String.Format("{0} {1}", GridList.SortField, GridList.SortDirection);
            GridList.DataSource = view1;
            GridList.DataBind();
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            if (GridList.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要打印的订单信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string billno = string.Empty;
            foreach (int index in GridList.SelectedRowIndexArray)
            {
                billno += GridList.Rows[index].DataKeys[0].ToString() + ",";
            }
            hfdSeqNoList.Text = billno.Trim(',');
            FineUIPro.MenuButton btn = sender as FineUIPro.MenuButton;
            if (btn.ID == "btnPrintPJ")
            {
                PageContext.RegisterStartupScript("PrintJSDBill('pj');");
            }
            else
            {
                PageContext.RegisterStartupScript("PrintJSDBill('ph');");
            }
        }
    }
}
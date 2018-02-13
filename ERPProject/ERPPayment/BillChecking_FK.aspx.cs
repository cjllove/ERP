using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

namespace ERPProject.ERPApply
{
    public partial class BillChecking_FK : BillBase
    {
        private string strDocSql = @"select 
                                CUSTID,SEQNO,DEPTID,F_GETDEPTNAME(DEPTID) DEPTIDNAME,SUPID,SUPNAME,FLAG,decode(FLAG,'Y','已结算','G','已付款','R','驳回','未结算') FLAGNAME,GATFUNDCORP,GATFUNDBANK,GATACCNTNO,LINKMAN,LINKTEL,
                                CWLINKMAN,CWLINKTEL,BEGRQ,ENDRQ,SYJE,XSJE,THJE,JSJE,FPJE,FPHM,LRY,LRRQ,SPR,SPRQ,SHR,YJJJ,WJJE,FPHM,
                                SHRQ,MEMO,STR1,STR2,STR3,NUM1,NUM2,NUM3,UPTTIME     
                                from dat_jsd_doc where SEQNO = '{0}' ";
        private string strLisSql = "select billno,SEQNO, LRY,F_GETUSERNAME(LRY) LRYNAME,LRRQ,SHR,F_GETUSERNAME(SHR) SHRNAME,SHRQ,F_GET_BILLTYPENAME(BILLTYPE) BILLTYPENAME,F_GETDEPTNAME(DEPTID) DEPTIDNAME,DEPTOUT,F_GETDEPTNAME(DEPTOUT) DEPTOUTNAME,decode(FLAG,'Y','已审核','G','已执行','未结算') FLAGNAME,DECODE(XSTYPE,'1','申领','申退') XSTYPENAME,PRICE_HSJE,PRICE_RTN,PRICE from DAT_JSD_BILL where seqno='{0}' order by BILLNO,SHRQ desc";
        public BillChecking_FK()
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

        private void DataInit()
        {
            PubFunc.DdlDataGet("DDL_DOC_SUPPLIERNULL", ddlSUPID);
            PubFunc.DdlDataGet("DDL_BILL_STATUSJSD", docFLAG, lstFLAG);
            PubFunc.DdlDataGet("DDL_USER", ddlFKY);
            PubFunc.DdlDataGet("DDL_SYS_DEPT", ddlDEPTID);
            ddlFKY.SelectedValue = UserAction.UserID;
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;
            dpkFKRQ.SelectedDate = DateTime.Now;
            PubFunc.FormLock(FormDoc, true);
            nbxJSJE.Enabled = true;
            tgbFPHM.Enabled = true;
            tbxMEMO.Enabled = true;
            ddlFKY.Enabled = true;
            dpkFKRQ.Enabled = true;
            //docFPJE.Enabled = true;
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                if (flag == "Y")
                {
                    highlightRows.Text += e.RowIndex.ToString() + ",";
                }
            }
        }

        private JObject GetJObject(Dictionary<string, object> dicRecord)
        {
            JObject defaultObj = new JObject();
            foreach (string key in dicRecord.Keys)
            {
                defaultObj.Add(key, dicRecord[key].ToString());
            }

            decimal hl = 0, rs = 0, jg = 0;
            decimal.TryParse(dicRecord["BZHL"].ToString(), out hl);//包装含量
            decimal.TryParse(dicRecord["BZSL"].ToString(), out rs);//订货数
            decimal.TryParse(dicRecord["HSJJ"].ToString(), out jg);//价格

            defaultObj.Remove("DHSL");
            defaultObj.Remove("HSJE");
            defaultObj.Add("DHSL", rs * hl);
            defaultObj.Add("HSJE", rs * jg);

            return defaultObj;
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
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

            string strSql = @"select 
                                CUSTID,SEQNO,DEPTID,F_GETDEPTNAME(DEPTID) DEPTIDNAME,SUPID,SUPNAME,FLAG,decode(FLAG,'Y','已结算','G','已付款','R','驳回','未结算') FLAGNAME,GATFUNDCORP,GATFUNDBANK,GATACCNTNO,LINKMAN,LINKTEL,
                                CWLINKMAN,CWLINKTEL,BEGRQ,ENDRQ,SYJE,XSJE,THJE,JSJE,FPJE,FPHM,LRY,LRRQ,SPR,SPRQ,SHR,WJJE,YJJJ,JSJE,WJJE,
                                SHRQ,MEMO,STR1,STR2,STR3,NUM1,NUM2,NUM3,UPTTIME     
                                from dat_jsd_doc where 1=1 ";
            string strSearch = "";
            if (lstBILLNO.Text.Length > 0)
            { strSearch += string.Format(" AND SEQNO  LIKE '%{0}%'", lstBILLNO.Text); }
            if (lstFLAG.SelectedValue.Length > 0)
            { strSearch += string.Format(" AND FLAG = '{0}'", lstFLAG.SelectedValue); }
            strSearch += string.Format(" AND ((BEGRQ between TO_DATE('{0}','YYYY-MM-DD') and (TO_DATE('{1}','YYYY-MM-DD')) + 1) or (ENDRQ between TO_DATE('{0}','YYYY-MM-DD') and (TO_DATE('{1}','YYYY-MM-DD') + 1)))", lstLRRQ1.Text, lstLRRQ2.Text);
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY DEPTID,ENDRQ DESC";
            highlightRows.Text = "";
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

        protected override void billAudit()
        {
            if (docSEQNO.Text.Length < 1)
            { Alert.Show("请选择需要付款的结算单!"); ;return; }
            if (docFLAG.SelectedValue != "Y")
            {
                Alert.Show("未审核单据或已付款单据,不能付款!");
                return;
            }
            //付款金额不增加控制
            if (PubFunc.StrIsEmpty(tgbFPHM.Text))
            { docFPJE.Text = "0"; }
            if (Convert.ToDecimal(docFPJE.Text) < Convert.ToDecimal(nbxJSJE.Text) && !PubFunc.StrIsEmpty(tgbFPHM.Text))
            {
                Alert.Show("发票金额不能小于实结金额!");
                return;
            }
            nbxWJJE.Text = (Convert.ToDecimal(nbbYJJJ.Text) - Convert.ToDecimal(nbxJSJE.Text)).ToString();
            DbHelperOra.ExecuteSql("update dat_jsd_doc set flag='G',JSJE=" + nbxJSJE.Text + ",WJJE=" + nbxWJJE.Text + ",FPJE=" + docFPJE.Text + ",UPTTIME =sysdate,FKY='" + ddlFKY.SelectedValue + "',FKRQ=to_date('" + dpkFKRQ.Text + "','YYYY-MM-DD'),memo='" + tbxMEMO.Text + "',FPHM ='" + tgbFPHM.Text + "' where seqno ='" + docSEQNO.Text + "'");
            //将出库单标志改为'G'
            OracleParameter[] parameters = new OracleParameter[]
            {
                 new OracleParameter("BILLNO",OracleDbType.Varchar2),
                 new OracleParameter("FPHM",OracleDbType.Varchar2)
            };
            parameters[0].Value = docSEQNO.Text;
            parameters[1].Value = tgbFPHM.Text;
            DbHelperOra.RunProcedure("P_JSD_FP", parameters);
            Alert.Show("单据【" + docSEQNO.Text + "】付款成功!");
            billOpen(docSEQNO.Text);
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[3].ToString());
        }

        protected override void billOpen(string strBillno)
        {
            //子页中表头赋值
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
            if (docFLAG.SelectedValue != "G")
            { nbxJSJE.Text = nbbYJJJ.Text; }

            //表体赋值
            DataTable dtBill = DbHelperOra.Query(string.Format(strLisSql, strBillno)).Tables[0];
            GridGoods.DataSource = dtBill;
            GridGoods.DataBind();
            TabStrip1.ActiveTabIndex = 1;
        }
        protected void GridLis_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string billno = GridGoods.Rows[e.RowIndex].Values[1].ToString();
            string url = "~/ERPPayment/Doc_CK_ComWindow.aspx?bm=" + billno + "&cx=&su=";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "出库信息:单号【" + billno + "】"));
        }

        protected void btnFP_Click(object sender, EventArgs e)
        {
            if (nbxJSJE.Text.Length < 1) return;
            string strSql = @"SELECT A.*,F_GETUSERNAME(A.LRY) LRYNAME,B.NAME FPTYPENAME,DECODE(A.FLAG,'N','未使用','已使用') FLAGNAME FROM DAT_JSD_FP A,sys_codevalue B WHERE A.FPTYPE = B.CODE AND B.type ='DAT_JS_FP' and SUPID={0} and USEJE >0 ";
            DataTable dtBill = DbHelperOra.Query(string.Format(strSql, "00002")).Tables[0];
            GridFp.DataSource = dtBill;
            GridFp.DataBind();
            decimal ddslTotal = 0, bzslTotal = 0, feeTotal = 0;
            foreach (DataRow row in dtBill.Rows)
            {
                ddslTotal += Convert.ToDecimal(row["FPJE"]);
                bzslTotal += Convert.ToDecimal(row["JSJE"]);
                feeTotal += Convert.ToDecimal(row["USEJE"]);
            }
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("lisSEQNO", "本页合计");
            summary.Add("lisFPJE", ddslTotal.ToString("F2"));
            summary.Add("lisJSJE", bzslTotal.ToString("F2"));
            summary.Add("lisUSEJE", feeTotal.ToString("F2"));
            GridFp.SummaryData = summary;
            Win_FP.Title = "发票信息 - 需结算金额：" + Convert.ToDecimal(nbxJSJE.Text).ToString("F2");
            Win_FP.Hidden = false;
        }

        protected void btnFpClose_Click(object sender, EventArgs e)
        {
            string fp = "";
            decimal sum = 0;
            int[] selections = GridFp.SelectedRowIndexArray;
            foreach (int rowIndex in selections)
            {
                fp += GridFp.DataKeys[rowIndex][0].ToString() + ",";
                sum += Convert.ToDecimal(GridFp.DataKeys[rowIndex][1]);
                if (sum >= Convert.ToDecimal(nbxJSJE.Text))
                {
                    break;
                }
            }
            if (sum < Convert.ToDecimal(nbxJSJE.Text))
            {
                Alert.Show("发票金额不能小于实结金额!");
                return;
            }
            docFPJE.Text = sum.ToString();
            tgbFPHM.Text = fp.TrimEnd(',');
            Win_FP.Hidden = true;
        }

        protected void GridFp_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string fp = "";
            decimal sum = 0;
            fp = GridFp.DataKeys[e.RowIndex][0].ToString() + ",";
            sum = Convert.ToDecimal(GridFp.DataKeys[e.RowIndex][1]);
            if (sum < Convert.ToDecimal(nbxJSJE.Text))
            {
                Alert.Show("发票金额不能小于实结金额!");
                return;
            }
            docFPJE.Text = sum.ToString();
            tgbFPHM.Text = fp.TrimEnd(',');
            Win_FP.Hidden = true;
        }
    }
}
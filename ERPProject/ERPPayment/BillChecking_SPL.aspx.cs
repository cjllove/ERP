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

namespace ERPProject.ERPApply
{
    public partial class BillChecking_SPL : BillBase
    {
        private string strDocSql = @"SELECT BILLNO,SEQNO, LRY,F_GETUSERNAME(LRY) LRYNAME,LRRQ,SHR,F_GETUSERNAME(SHR) SHRNAME,SHRQ,F_GET_BILLTYPENAME(BILLTYPE) BILLTYPENAME,BILLTYPE,
                                 F_GETDEPTNAME(DEPTID) DEPTIDNAME,DEPTOUT,F_GETDEPTNAME(DEPTOUT) DEPTOUTNAME,decode(FLAG,'Y','已审核','G','已执行','未结算') FLAGNAME,
                                DECODE(XSTYPE,'1','申领','申退') XSTYPENAME,PRICE_HSJE,PRICE_RTN,PRICE,PRICE SUPSUM from DAT_JSD_BILL where seqno='{0}' order by BILLNO,SHRQ desc";
        private string strLisSQL = @"SELECT SEQNO,DEPTID,SUPID,BEGRQ,ENDRQ,LRY,FLAG,decode(FLAG,'Y','已审核','G','已付款','R','驳回','未结算') FLAGNAME,LRRQ,YJJJ,MEMO FROM dat_jsd_doc WHERE SEQNO = '{0}' and DEPTID = '{1}' ORDER by SEQNO DESC";
        public BillChecking_SPL()
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
            tgbCKDH.Text = string.Empty;
            docSEQNO.Enabled = false;
            docFLAG.Enabled = false;
            docLRY.Enabled = false;
            tbxYJJJ.Enabled = false;
            Bill_create.Enabled = true;
            tbxMEMO.Text = string.Empty;
            tbxYJJJ.Text = string.Empty;
        }
        private void DataInit()
        {
            PubFunc.DdlDataGet("DDL_SYS_DEPT", lstDEPTOUT, docDEPTID);
            PubFunc.DdlDataGet("DDL_USER", docLRY);
            PubFunc.DdlDataGet("DDL_BILL_STATUSJSD", docFLAG);
            PubFunc.DdlDataGet("DDL_DOC_SHS", ddlSUPID);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            docJSKS.SelectedDate = DateTime.Now.AddMonths(-1);
            docJSJS.SelectedDate = DateTime.Now;
            ddlSUPID.SelectedValue = Doc.DbGetSysPara("SUPPLIER");
            if (Request.QueryString["ISGZ"] != null && Request.QueryString["ISGZ"].ToString() == "G")
            {
                ddlNUM2.Items.RemoveAt(1);


            }
            if (Request.QueryString["ISGZ"] != null && Request.QueryString["ISGZ"].ToString() == "P")
            {

                ddlNUM2.Items.RemoveAt(0);

            }
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
            string strSearch = "";
            string strSql = @"SELECT DJD.CUSTID,           
       DJD.SEQNO,
       DJD.DEPTID,
       F_GETDEPTNAME(DJD.DEPTID) DEPTIDNAME,
       DJD.SUPID,
       DJD.SUPNAME,
       DJD.FLAG,
       DECODE(DJD.FLAG, 'Y', '已审核', 'G', '已付款', 'R', '驳回', '未结算') FLAGNAME,
       DJD.GATFUNDCORP,
       DJD.GATFUNDBANK,
       DJD.GATACCNTNO,
       DJD.LINKMAN,
       DJD.LINKTEL,
       DJD.CWLINKMAN,
       DJD.CWLINKTEL,
       DJD.BEGRQ,
       DJD.ENDRQ,
       DJD.SYJE,
       DJD.XSJE,
       DJD.THJE,
       DJD.JSJE,
       DJD.FPJE,
       DJD.FPHM,
       DJD.LRY,
       DJD.LRRQ,
       DJD.SPR,
       DJD.SPRQ,
       DJD.SHR,
       DJD.YJJJ,
       DJD.JSJE,
       DJD.WJJE,
       DJD.SHRQ,
       DJD.MEMO,
       DJD.STR1,
       DJD.STR2,
       DJD.STR3,
       DJD.NUM1,
      DJD.NUM2,
       DJD.NUM3,
       DJD.UPTTIME
  FROM DAT_JSD_DOC DJD,DAT_JSD_BILL DJB WHERE DJB.BILLTYPE='XSG' AND DJD.SEQNO=DJB.SEQNO(+)  ";
           
            if (lstBILLNO.Text.Length > 0)
            { strSearch += string.Format(" AND DJD.SEQNO  = '{0}'", lstBILLNO.Text); }
            if (lstDEPTOUT.SelectedItem.Value.Length > 0)
            { strSearch += string.Format(" AND DJD.DEPTID='{0}'", lstDEPTOUT.SelectedItem.Value); }
            strSearch += string.Format(" AND ((DJD.BEGRQ between TO_DATE('{0}','YYYY-MM-DD') and (TO_DATE('{1}','YYYY-MM-DD')) + 1) or (DJD.ENDRQ between TO_DATE('{0}','YYYY-MM-DD') and (TO_DATE('{1}','YYYY-MM-DD') + 1)))", lstLRRQ1.Text, lstLRRQ2.Text);
           

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY DJD.DEPTID,DJD.ENDRQ DESC";
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
            //科室不需审核
            return;
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[3].ToString(), GridList.Rows[e.RowIndex].Values[1].ToString());
        }

        protected void billOpen(string strBillno, string DEPTID)
        {
            if (DEPTID.Length <= 0)
            {
                string sql = @"SELECT SEQNO,DEPTID,SUPID,BEGRQ,ENDRQ,LRY,FLAG,decode(FLAG,'Y','已审核','G','已付款','R','驳回','未结算') FLAGNAME,LRRQ,YJJJ,MEMO FROM dat_jsd_doc  WHERE SEQNO = '{0}' ORDER by SEQNO DESC";
                //表头进行赋值
                DataTable dtDoc = DbHelperOra.Query(string.Format(sql, strBillno)).Tables[0];
                PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
                //表体赋值
                DataTable dtBill = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
                GridGoods.DataSource = dtBill;
                GridGoods.DataBind();
                TabStrip1.ActiveTabIndex = 1;
                PubFunc.FormLock(FormDoc, true);
                Bill_create.Enabled = false;
            }
            else
            {
                //表头进行赋值
                DataTable dtDoc = DbHelperOra.Query(string.Format(strLisSQL, strBillno, DEPTID)).Tables[0];
                PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
                //表体赋值
                DataTable dtBill = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
                GridGoods.DataSource = dtBill;
                GridGoods.DataBind();
                TabStrip1.ActiveTabIndex = 1;
                PubFunc.FormLock(FormDoc, true);
                Bill_create.Enabled = false;
            }

        }
        protected void GridLis_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string billno = GridGoods.Rows[e.RowIndex].Values[1].ToString();
            string type = GridGoods.DataKeys[e.RowIndex][4].ToString();
            string url = "";
            if (type == "CKD" || type == "LCD" || type == "DSC" || type == "DST")
            {
                url = "~/ERPPayment/Doc_CK_ComWindow.aspx?bm=" + billno + "&cx=&su=";
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "出库信息:单号【" + billno + "】"));
            }
            if (type == "XSD" || type == "XSG" || type == "XST")
            {
                url = "~/ERPPayment/Doc_XS_ComWindow.aspx?bm=" + billno + "&cx=&su=" + ddlSUPID.SelectedValue;
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "销售信息:单号【" + billno + "】"));
            }
            if (type == "RKD")
            {
                url = "~/ERPPayment/Doc_RK_ComWindow.aspx?bm=" + billno + "&cx=&su=";
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "入库信息:单号【" + billno + "】"));
            }
        }

        protected void Bill_create_Click(object sender, EventArgs e)
        {
            //将选中行生成一张结算单
            int[] selections = GridGoods.SelectedRowIndexArray;
            if (selections.Count() < 1) return;
            string deptid_old = "";
            string deptid = "";
            string str_bill = "";
            decimal Hjje = 0;
            List<CommandInfo> cmdList = new List<CommandInfo>();
            foreach (int rowIndex in selections)
            {
                if (deptid_old == "")
                { deptid_old = GridGoods.DataKeys[rowIndex][1].ToString(); }
                deptid = GridGoods.DataKeys[rowIndex][1].ToString();
                if (deptid != deptid_old)
                {
                    Alert.Show("第【" + (rowIndex + 1).ToString() + "】行，单据【" + GridGoods.DataKeys[rowIndex][0].ToString() + "】结算科室与其他单据结算科室不一致,请检查！");
                    return;
                }
                str_bill += "'" + GridGoods.DataKeys[rowIndex][0].ToString() + "'" + ",";
                Hjje += decimal.Parse(GridGoods.DataKeys[rowIndex][2].ToString());
            }
            docSEQNO.Text = BillSeqGet();
            string SqlInsJsd = @"INSERT INTO DAT_JSD_DOC(CUSTID,SEQNO,DEPTID,FLAG,SUPID,SUPNAME,GATFUNDCORP,GATFUNDBANK,GATACCNTNO,LINKMAN,LINKTEL,CWLINKMAN,CWLINKTEL,BEGRQ,ENDRQ,SYJE,XSJE,THJE,YJJJ,LRY,LRRQ,UPTTIME,MEMO)
            select (select VALUE from sys_para WHERE CODE = 'USERCODE'),'{0}','{1}','N','{2}',supname,SUPNAME,GATFUNDBANK,GATACCNTNO,LINKMAN,LINKTEL,CWLINKMAN,CWLINKTEL,SYSDATE,SYSDATE,0,{3},0,{3},'{4}',sysdate,sysdate,'{5}'
            from doc_supplier where supid = '{2}'";
            string beizhu = "";
            if (PubFunc.StrIsEmpty(tbxMEMO.Text))
            {
                if (ddlNUM2.SelectedValue == "G")
                {
                    beizhu = "高值结算单";
                }
                else
                {
                    beizhu = "普通结算单";
                }
            }
            cmdList.Add(new CommandInfo(string.Format(SqlInsJsd, docSEQNO.Text, deptid_old, ddlSUPID.SelectedValue, Hjje, UserAction.UserID, beizhu), null));
            string SqlInsBill = @"INSERT INTO DAT_JSD_BILL(CUSTID,SEQNO,BILLNO,BILLTYPE,FLAG,DEPTOUT,DEPTID,XSTYPE,XSRQ,LRY,LRRQ,SHR,SHRQ,SUBNUM,PRICE_HSJE,PRICE_RTN,PRICE,MEMO)
              SELECT (select VALUE from sys_para WHERE CODE = 'USERCODE'),'{1}',SEQNO,BILLTYPE,FLAG,DEPTOUT,DEPTID,XSTYPE,XSRQ,LRY,LRRQ,SHR,SHRQ,SUBNUM,DECODE(XSTYPE,'1',SUBSUM,0),DECODE(XSTYPE,'1',0,SUBSUM),SUBSUM,'高值结算单'
              FROM DAT_XS_DOC WHERE DEPTID = '{0}' AND SEQNO IN ({2})";
            cmdList.Add(new CommandInfo(string.Format(SqlInsBill, deptid_old, docSEQNO.Text, str_bill.TrimEnd(',')), null));
            cmdList.Add(new CommandInfo("UPDATE DAT_XS_DOC SET FLAG = 'D' WHERE SEQNO IN (" + str_bill.TrimEnd(',') + ")", null));
            cmdList.Add(new CommandInfo("UPDATE DAT_GOODSJXC SET STR1='" + docSEQNO.Text + "' WHERE BILLNO IN (" + str_bill.TrimEnd(',') + ") AND NVL(STR1,'#') = '#'", null));
            cmdList.Add(new CommandInfo("UPDATE DAT_XS_DOC A SET FLAG = 'G' , STR3='" + docSEQNO.Text + "' WHERE NOT EXISTS (SELECT 1 FROM DAT_GOODSJXC B WHERE B.BILLNO = A.SEQNO AND NVL(STR1,'#') = '#') AND SEQNO IN (" + str_bill.TrimEnd(',') + ")", null));
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("结算单据【" + docSEQNO.Text + "】生成成功!");
                billOpen(docSEQNO.Text, deptid_old);
            }
            else
            {
                Alert.Show("因系统调度原因审核失败,请联系管理员！");
            }
        }
        protected override void billDel()
        {
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请选择需要删除的结算单！");
                return;
            }
            if (docFLAG.SelectedValue != "N" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("此张结算单已被审核,不能删除!");
                return;
            }
            DbHelperOra.ExecuteSql("UPDATE DAT_XS_DOC SET FLAG = 'J', STR3='#',NUM2 =0 WHERE STR3 ='" + docSEQNO.Text + "'");
            DbHelperOra.ExecuteSql("UPDATE DAT_GOODSJXC SET STR1='#' WHERE STR1 ='" + docSEQNO.Text + "'");
            DbHelperOra.ExecuteSql("DELETE FROM DAT_JSD_BILL WHERE SEQNO ='" + docSEQNO.Text + "'");
            DbHelperOra.ExecuteSql("DELETE FROM DAT_JSD_DOC WHERE SEQNO ='" + docSEQNO.Text + "'");
            Alert.Show("单据【" + docSEQNO.Text + "】删除成功！");
            billNew();
        }
        protected void btnSrh_Click(object sender, EventArgs e)
        {
            if (ddlSUPID.SelectedValue.Length < 1)
            {
                Alert.Show("请选择结算【供应商】", MessageBoxIcon.Warning);
                return;
            }
            //查询需结算单据
            string Sql = string.Format(@"SELECT A.*,DECODE(A.FLAG,'J','已登记','D','部分结算','已结算') FLAGNAME,F_GET_BILLTYPENAME(A.BILLTYPE) BILLTYPENAME,f_getdeptname(A.DEPTOUT) DEPTOUTNAME,f_getdeptname(A.DEPTID) DEPTIDNAME,
                    DECODE(XSTYPE,'1','申领','申退') XSTYPENAME,f_getusername(A.LRY) LRYNAME,f_getusername(A.SHR) SHRNAME,ABS((SELECT SUM(HSJE) FROM DAT_GOODSJXC B WHERE NVL(B.PSSID,B.SUPID) = '{2}' AND B.BILLNO = A.SEQNO AND NVL(STR1,'#') = '#')) SUPSUM
                    FROM DAT_XS_DOC A, DAT_XS_COM B
                    WHERE A.SEQNO=B.SEQNO AND　A.FLAG IN ('J','D') AND A.SHRQ BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND TO_DATE('{1}','YYYY-MM-DD') + 1  AND A.BILLTYPE='XSG'
                    AND EXISTS(SELECT 1 FROM DAT_GOODSJXC B WHERE NVL(B.PSSID,B.SUPID) = '{2}' AND B.BILLNO = A.SEQNO AND NVL(STR1,'#') = '#')", docJSKS.Text, docJSJS.Text, ddlSUPID.SelectedValue);
            string strSearch = "";
            if (docDEPTID.SelectedValue.Length > 0)
            { strSearch += string.Format(" AND A.DEPTID  LIKE '{0}'", docDEPTID.SelectedValue); }
            if (ddlNUM2.SelectedValue == "G")
            {
                strSearch += string.Format(" AND B.ISGZ  LIKE '{0}'", "Y");
            }

            else
            { strSearch += string.Format(" AND B.ISGZ  LIKE '{0}'", "N"); }
            if (tgbCKDH.Text.Length > 0)
            { strSearch += string.Format(" AND A.SEQNO LIKE '%{0}%'", tgbCKDH.Text); }
            strSearch += " ORDER BY A.DEPTID,A.SEQNO DESC";
            GridGoods.DataSource = DbHelperOra.Query(Sql + strSearch).Tables[0];
            GridGoods.DataBind();
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

        protected void GridGoods_RowSelect(object sender, GridRowSelectEventArgs e)
        {
            //计算合计数量
            int[] selects = GridGoods.SelectedRowIndexArray;
            decimal ddslTotal = 0;
            foreach (int i in selects)
            {
                ddslTotal += Convert.ToDecimal((GridGoods.DataKeys[i][2] ?? "0"));
            }
            JObject summary = new JObject();
            summary.Add("DEPTOUTNAME", "选择合计");
            summary.Add("SUBSUM", ddslTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
        }
    }
}
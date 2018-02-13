using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using FineUIPro;
using System.Collections.Specialized;
using System.Text;
using System.Collections;
using XTBase;
using Oracle.ManagedDataAccess.Client;
using Newtonsoft.Json.Linq;

namespace ERPProject.ERPApply
{
    public partial class DepartmentRefuse : BillBase
    {
        private string strDocSql = "SELECT c.*,(SELECT  '['||CODE||']'||NAME NAME  FROM SYS_DEPT where CODE=c.DEPTID) DEPNAME FROM DAT_SL_DOC c WHERE SEQNO ='{0}'";
        private string strComSql = @"SELECT A.*, F_GETUNITNAME(A.UNIT) UNITNAME,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,f_getunitname(B.UNIT) UNITSMALLNAME,DECODE(FPFLAG,'Y','已分配','F','作废','未分配') FPFLAGNAME
                        FROM DAT_SL_COM A,DOC_GOODS B WHERE SEQNO ='{0}' AND A.GDSEQ = B.GDSEQ ORDER BY ROWNO DESC";
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
            //PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, lstDEPTOUT, docDEPTOUT);
            PubFunc.DdlDataGet("DDL_SYS_DEPOT", lstDEPTOUT, docDEPTOUT);
            PubFunc.DdlDataGet("DDL_USER", lstSLR, docLRY, docSLR, docSHR);
            PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
            PubFunc.DdlDataGet("DDL_BILL_STATUSSLD", lstFLAG, docFLAG);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");

            if (docDEPTOUT.Items.Count > 0)
            {
                docDEPTOUT.SelectedIndex = 1;
            }
            if (docDEPTID.Items.Count > 0)
            {
                docDEPTID.SelectedIndex = 1;
            }
            PubFunc.DdlDataGet("DDL_SYS_DEPOT", UserAction.UserID, docDEPTOUT);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                if (flag == "M")
                {
                    highlightRows.Text += e.RowIndex.ToString() + ",";
                }
                if (flag == "N")
                {
                    highlightRowYellow.Text += e.RowIndex.ToString() + ",";
                }
                if (flag == "R")
                {
                    highRedlightRows.Text += e.RowIndex.ToString() + ",";
                }
            }
        }
        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【申领日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO,B.NAME FLAG_CN,A.FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,
                                     A.SUBNUM,F_GETUSERNAME(A.SLR) SLR,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO 
                                from DAT_SL_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' AND BILLTYPE='LYD' AND XSTYPE='1' AND A.FLAG IN('N','S','B','F') ";
            string strSearch = "";

            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedValue != null && lstDEPTID.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedValue);
            }
            if (lstDEPTOUT.SelectedItem != null && lstDEPTOUT.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND DEPTOUT='{0}'", lstDEPTOUT.SelectedItem.Value);
            }
            if (lstSLR.SelectedItem != null && lstSLR.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND SLR='{0}'", lstSLR.SelectedItem.Value);
            }

            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.XSRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC";
            highlightRows.Text = "";
            highlightRowYellow.Text = "";
            highRedlightRows.Text = "";
            DataTable table = DbHelperOra.Query(strSql).Tables[0];
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridList.SortField, GridList.SortDirection);
            GridList.DataSource = view1;
            GridList.DataBind();
        }

        protected bool billFlag(string billno, string flag)
        {
            //从数据库里判断当前单据的状态，避免并发
            if (!DbHelperOra.Exists("SELECT 1 FROM DAT_SL_DOC WHERE BILLNO = '" + billno + "' AND FLAG = '" + flag + "'"))
            {
                Alert.Show("此单据已经被操作，请刷新确认再做操作。", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());
        }

        protected override void billOpen(string strBillno)
        {
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            if (dtDoc != null && dtDoc.Rows.Count > 0)
            {
                PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
            }
            else
            {
                Alert.Show("单据信息获取失败！！！", "警告提示", MessageBoxIcon.Warning);
                return;
            }

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno)).Tables[0];
            decimal bzslTotal = 0, feeTotal = 0, dhslTotal = 0;
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["BZSL"] ?? "0");
                    feeTotal += Convert.ToDecimal(row["BZSL"] ?? "0") * Convert.ToDecimal(row["HSJJ"] ?? "0");
                    dhslTotal += Convert.ToDecimal(row["BZSL"] ?? "0") * Convert.ToDecimal(row["BZHL"] ?? "0");
                    //LoadGridRow(row, false, "OLD");
                }
                /*
                *  修 改 人 ：袁鹏    修改时间：2015-03-20
                *  信息说明：这种加载方法比LoadGridRow(row, false, "OLD")更高效
                *  研发组织：威高讯通信息科技有限公司
                */
                PubFunc.GridRowAdd(GridGoods, dtBill);
            }
            //增加合计
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            summary.Add("DHSL", dhslTotal.ToString());
            GridGoods.SummaryData = summary;
            PubFunc.FormLock(FormDoc, true, "");
            TabStrip1.ActiveTabIndex = 1;
            if (docFLAG.SelectedValue == "F")
            {
                btnCancel.Enabled = false;
            }
            else
            {
                btnCancel.Enabled = true;
            }
        }

        /// <summary>
        /// FineUIPro.Grid控件数据加载
        /// </summary>
        /// <param name="row">要加载的行数据</param>
        /// <param name="firstRow">是否插入指定行</param>
        /// <param name="flag">数据来源：NEW-从数据库中获得，用于商品新增时；OLD-从销售单据明细中获得，用于修改或审批时</param>
        private void LoadGridRow(DataRow row, bool firstRow = true, string flag = "NEW")
        {
            if (flag == "NEW")
            {
                if (!string.IsNullOrWhiteSpace(row["UNIT_SELL"].ToString()))
                {
                    //if (row["UNIT_SELL"].ToString() == "D")//出库单位为大包装时
                    //{
                    //    if (!string.IsNullOrWhiteSpace(row["NUM_DABZ"].ToString()) && row["NUM_DABZ"].ToString() != "0")
                    //    {
                    //        row["UNIT"] = row["UNIT_DABZ"];
                    //        row["UNITNAME"] = row["UNIT_DABZ_NAME"];
                    //        row["BZHL"] = row["NUM_DABZ"];
                    //        int price = 0, number = 0;
                    //        int.TryParse(row["HSJJ"].ToString(), out price);
                    //        int.TryParse(row["NUM_DABZ"].ToString(), out number);
                    //        row["HSJE"] = price * number;
                    //    }
                    //}
                    //else if (row["UNIT_SELL"].ToString() == "Z")//出库单位为中包装时
                    //{
                    //    if (!string.IsNullOrWhiteSpace(row["NUM_ZHONGBZ"].ToString()) && row["NUM_ZHONGBZ"].ToString() != "0")
                    //    {
                    //        row["UNIT"] = row["UNIT_ZHONGBZ"];
                    //        row["UNITNAME"] = row["UNIT_ZHONGBZ_NAME"];
                    //        row["BZHL"] = row["NUM_ZHONGBZ"];
                    //        int price = 0, number = 0;
                    //        int.TryParse(row["HSJJ"].ToString(), out price);
                    //        int.TryParse(row["NUM_ZHONGBZ"].ToString(), out number);
                    //        row["HSJE"] = price * number;
                    //    }
                    //}
                }
            }

            PubFunc.GridRowAdd(GridGoods, row, firstRow);
        }


        protected void trbEditorGDSEQ_TriggerClick(object sender, EventArgs e)
        {

        }
        protected override void billCancel()
        {
            if (docBILLNO.Text.Length < 1)
            {
                Alert.Show("请选择需要作废的单据!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string flag = (DbHelperOra.GetSingle(string.Format("SELECT FLAG FROM DAT_SL_DOC WHERE SEQNO = '{0}'", docBILLNO.Text)) ?? "").ToString();
            if (flag != "N" && flag != "S" && flag != "B")
            {
                Alert.Show("此单据状态不正确，不能作废！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            WindowReject.Hidden = false;
        }

        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            if (ddlReject.SelectedText == "--请选择--")
            {
                Alert.Show("请选择作废原因");
                return;
            }

            //增加待办事宜
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo("UPDATE DAT_DO_LIST SET FLAG = 'Y' WHERE PARA='" + docBILLNO.Text.Trim() + "'", null));

            string strMemo = docMEMO.Text + "；作废原因：" + ddlReject.SelectedText;
            if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
            {
                strMemo += "；详细说明：" + txaMemo.Text;
            }

            cmdList.Add(new CommandInfo(string.Format("update DAT_SL_DOC set flag='F',memo='{0}',SHR={1}，SHRQ=SYSDATE where seqno='{2}'", strMemo, "'" + UserAction.UserID + "'", docBILLNO.Text), null));
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                WindowReject.Hidden = true;
                billOpen(docBILLNO.Text);
                Alert.Show("单据作废成功!");
            }

        }
        protected override void billDel()
        {

            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (("MR").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新增』单据不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!CheckFlagForM() && !CheckFlagForR())
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }
            List<string> listSql = new List<string>();
            listSql.Add("Delete from DAT_SL_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            listSql.Add("Delete from DAT_SL_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            listSql.Add("UPDATE DAT_DO_LIST SET FLAG = 'Y' WHERE PARA='" + docBILLNO.Text.Trim() + "'");
            if (DbHelperOra.ExecuteSqlTran(listSql))
            {
                Alert.Show("单据删除成功!", "消息提示", MessageBoxIcon.Information);
                billNew();
                billSearch();
            }
            else
            {
                Alert.Show("单据删除失败!", "错误提示", MessageBoxIcon.Information);
            }
        }

        protected void btnAuditBatch_Click(object sender, EventArgs e)
        {
            int[] rowIndex = GridList.SelectedRowIndexArray;
            if (rowIndex.Length == 0)
            {
                Alert.Show("请选择要审批的科室申领信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            int count = 0;
            List<CommandInfo> sqlList = new List<CommandInfo>();
            foreach (int index in rowIndex)
            {
                if (GridList.Rows[index].Values[3].ToString() == "N")
                {
                    count += BillOper(GridList.Rows[index].Values[1].ToString(), "DECLARE");
                }
            }
            if (count > 0 && DbHelperOra.ExecuteSqlTran(sqlList))
            {
                Alert.Show("科室申领批量审批成功！", "消息提示", MessageBoxIcon.Information);
                billSearch();
            }
        }
        protected override void billCopy()
        {
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_SL_DOC WHERE SEQNO = '{0}'", docSEQNO.Text)))
            {
                Alert.Show("单据【" + docSEQNO.Text + "】不存在,请检查！");
                return;
            }
            //复制单据
            string billNew = BillSeqGet();
            DbHelperOra.ExecuteSql(string.Format(@"INSERT INTO DAT_SL_DOC(SEQNO,BILLNO,BILLTYPE,FLAG,DEPTOUT,DEPTID,CATID,SLR,XSTYPE,XSRQ,THTYPE,SUBNUM,LRY,LRRQ,STR1,MEMO)
                SELECT '{2}','{2}',BILLTYPE,'M',DEPTOUT,DEPTID,CATID,'{1}',XSTYPE,SYSDATE,THTYPE,SUBNUM,'{1}',SYSDATE,STR1,MEMO
                FROM DAT_SL_DOC WHERE SEQNO = '{0}'", docSEQNO.Text, UserAction.UserID, billNew));
            DbHelperOra.ExecuteSql(string.Format(@"INSERT INTO DAT_SL_COM(SEQNO,ROWNO,GDSEQ,BARCODE,GDNAME,UNIT,GDSPEC,GDMODE,HWID,BZHL,BZSL,DHSL,XSSL,JXTAX,HSJJ,BHSJJ,HSJE,BHSJE,LSJ,LSJE,ISGZ,ISLOT,PHID,PH,PZWH,RQ_SC,YXQZ,PRODUCER,ZPBH,STR1,MEMO)
                SELECT '{1}',ROWNO,GDSEQ,BARCODE,GDNAME,UNIT,GDSPEC,GDMODE,HWID,BZHL,BZSL,DHSL,0 XSSL,JXTAX,HSJJ,BHSJJ,HSJE,BHSJE,LSJ,LSJE,ISGZ,ISLOT,PHID,PH,PZWH,RQ_SC,YXQZ,PRODUCER,ZPBH,STR1,MEMO
                FROM DAT_SL_COM WHERE SEQNO = '{0}'", docSEQNO.Text, billNew));
            billOpen(billNew);

            Alert.Show("新单据【" + billNew + "】生成成功！");
            docMEMO.Enabled = true;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (docSEQNO.Text.Length < 1)
            {
                Alert.Show("申领单:" + docSEQNO.Text + "没有保存，不能提交!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_SL_DOC WHERE SEQNO='{0}'", docSEQNO.Text));
            if (!PubFunc.StrIsEmpty(flg) && (",M").IndexOf(flg) < 0)
            {
                Alert.Show("申领单:" + docSEQNO.Text + "不是新增单据，不能提交!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //增加待办事宜
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo(string.Format("update DAT_SL_DOC set flag='N' where seqno='{0}' and flag='M'", docBILLNO.Text), null));
            if (!DbHelperOra.Exists("select * from DAT_DO_LIST where PARA='" + docBILLNO.Text.Trim() + "'"))
            {
                cmdList.Add(new CommandInfo(" UPDATE DAT_DO_LIST SET DOUSER='" + UserAction.UserID + "',DORQ=SYSDATE,FLAG='Y' WHERE DOTYPE='" + docDEPTID.SelectedValue + "' AND PARA='" + docBILLNO.Text + "'", null));
                cmdList.Add(Doc.GETDOADD("DO_4", docDEPTID.SelectedValue, docLRY.SelectedValue, docBILLNO.Text));
            }

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("提交成功");
                billOpen(docSEQNO.Text);
            }
        }

        private bool CheckFlagForM()
        {
            if (docBILLNO.Text.Length > 0)
            {
                return Doc.getFlag(docBILLNO.Text, "M", BillType);
            }
            return true;
        }

        private bool CheckFlagForR()
        {
            if (docBILLNO.Text.Length > 0)
            {
                return Doc.getFlag(docBILLNO.Text, "R", BillType);
            }
            return true;
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            if (e.EventArgument == "CONTROLM_ENTER")
            {
                billGoods();
            }
        }
        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            GridList.SortDirection = e.SortDirection;
            GridList.SortField = e.SortField;

            DataTable table = PubFunc.GridDataGet(GridList);
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridList.SortField, GridList.SortDirection);
            GridList.DataSource = view1;
            GridList.DataBind();
        }
    }
}
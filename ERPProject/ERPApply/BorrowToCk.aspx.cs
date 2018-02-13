using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
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
    public partial class BorrowToCk : BillBase
    {
        #region Load加载
        private string strDocSql = "SELECT A.* FROM DAT_CK_DOC A WHERE A.SEQNO ='{0}' AND A.BILLTYPE = 'CKD' AND EXISTS(SELECT 1 FROM DAT_JH_COM B WHERE B.STR1 = A.SEQNO)";
        private string strComSql = @"SELECT A.*,F_GETUNITNAME(A.UNIT) UNITNAME,F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,F_GETUNITNAME(B.UNIT) UNITSMALLNAME
                                                    FROM DAT_CK_COM A, DOC_GOODS B WHERE SEQNO = '{0}' AND A.GDSEQ = B.GDSEQ ORDER BY ROWNO";
        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docDEPTID, docXSRQ }; }
        }

        public BorrowToCk()
        {
            BillType = "CKD";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ButtonHidden(btnCommit, btnCopy, btnNext, btnBef, btnCancel, btnExport, btnAddRow, btnDelRow, btnGoods, btnNew);
                DataInit();
                billNew();
            }
        }
        private void DataInit()
        {
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet("DDL_USER", docSHR, docLRY, docSLR);
            PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID, lstDEPTID, ddlDEPTID);
            //PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, docDEPTOUT, ddlDEPTOUT);
            PubFunc.DdlDataGet("DDL_SYS_DEPOT", docDEPTOUT, ddlDEPTOUT);
            PubFunc.DdlDataGet("DDL_BILL_STATUS", docFLAG);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                FineUIPro.BoundField flagcol = GridList.FindColumn("FLAGNAME") as FineUIPro.BoundField;
                if (flag == "M")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color1";
                }
                if (flag == "N")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color2";
                }
            }
        }
        #endregion
        #region 增删改
        protected override void billNew()
        {
            //原单据保存判断
            string strDept = docDEPTID.SelectedValue;
            PubFunc.FormDataClear(FormDoc);

            docFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            docSLR.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            dpkTime1.SelectedDate = DateTime.Now.AddDays(-1);
            dpkTime2.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDept;
            billLockDoc(false);
            docMEMO.Enabled = true;
            docDEPTOUT.Enabled = true;
            docSLR.Enabled = true;
            //清空Grid行
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            btnDel.Enabled = false;
            btnSave.Enabled = true;
            btnCommit.Enabled = false;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrt.Enabled = false;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
        }
        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected override void billAddRow()
        {
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新增』单据不能增行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            billLockDoc(true);
            PubFunc.GridRowAdd(GridGoods, "INIT");
        }

        protected override void billDelRow()
        {
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新增』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridGoods.SelectedCell == null)
            {
                Alert.Show("空单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            else
            {
                GridGoods.DeleteSelectedRows();
            }

        }
        protected override void billSave()
        {
            #region 数据有效性验证
            if ((",M,N").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {

                    if ((newDict[i]["BZSL"] ?? "").ToString() == "" || (newDict[i]["BZSL"] ?? "").ToString() == "0")
                    {
                        Alert.Show("请填写商品[" + newDict[i]["GDSEQ"] + "]出库数！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(newDict[i]["BZHL"].ToString()) || string.IsNullOrWhiteSpace(newDict[i]["UNIT"].ToString()))
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]包装单位信息错误，请联系管理员维护！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(newDict[i]["HSJJ"].ToString()))
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】含税进价不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(newDict[i]["UNIT"].ToString()))
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】出库包装未维护！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    goodsData.Add(newDict[i]);
                }
            }

            if (goodsData.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("商品信息不能为空", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            #endregion

            if (PubFunc.StrIsEmpty(docBILLNO.Text))
            {
                docSEQNO.Text = BillSeqGet();
                docBILLNO.Text = docSEQNO.Text;
                docBILLNO.Enabled = false;
            }
            else
            {
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'N') FROM DAT_CK_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
                if (!string.IsNullOrWhiteSpace(flg) && (",M,R,N").IndexOf(flg) < 0)
                {
                    Alert.Show("您输入的单据号存在重复信息，请重新输入或置空！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    docSEQNO.Text = docBILLNO.Text;
                    docBILLNO.Enabled = false;
                }
            }
            MyTable mtType = new MyTable("DAT_CK_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("XSTYPE", "1");
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_CK_COM");
            decimal subNum = 0;//总金额
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_CK_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_CK_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);

                //判断含税进价，为0时不能保存
                string isJiFei = string.Format("select 1 from DOC_GOODS t where iscf = 'N' and gdseq = '{0}'", mtTypeMx.ColRow["GDSEQ"].ToString());
                if (DbHelperOra.Exists(isJiFei))
                {
                    if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["HSJJ"].ToString()) || mtTypeMx.ColRow["HSJJ"].ToString() == "0")
                    {
                        Alert.Show("商品【含税进价】为0或空，无法进行【库房出库管理】操作。");
                        return;
                    }
                }
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                mtTypeMx.ColRow.Add("XSSL", mtTypeMx.ColRow["DHSL"].ToString());
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);

                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            cmdList.AddRange(mtType.InsertCommand());
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("商品出库信息保存成功！");
                billLockDoc(true);
                billOpen(docBILLNO.Text);
            }
        }
        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据");
                return;
            }
            if ((",M,N").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非[新单]不能删除!");
                return;
            }
            DbHelperOra.ExecuteSql("Delete from DAT_CK_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_CK_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql(string.Format("DELETE FROM DAT_CK_EXT WHERE BILLNO = '{0}'", docBILLNO.Text.Trim()));
            //清除反写的信息
            DbHelperOra.ExecuteSql("UPDATE DAT_JH_COM t SET STR1 = '#',NUM1 = NULL WHERE T.STR1 ='" + docBILLNO.Text.Trim() + "'");
            Alert.Show("单据删除成功!");
            billNew();
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                return;
            }
            billSearch();
        }
        #endregion
        #region 查询
        protected void tgbBILLNO_TriggerClick(object sender, EventArgs e)
        {
            billSearch();
        }

        private DataTable DataSerach()
        {
            string strSql = @"SELECT A.SEQNO,A.BILLNO,B.NAME FLAGNAME,A.FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,A.SUBSUM,
                                     A.SUBNUM,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO 
                                from DAT_CK_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' AND BILLTYPE='CKD' AND XSTYPE='1' AND EXISTS(SELECT 1 FROM DAT_JH_COM B WHERE B.STR1 = A.SEQNO)";
            string strSearch = "";


            if (tgbBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND UPPER(TRIM(A.BILLNO))  LIKE '%{0}%'", tgbBILLNO.Text.Trim().ToUpper());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (!PubFunc.StrIsEmpty(lstFLAG.SelectedValue))
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedValue);
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.XSRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC";

            return DbHelperOra.Query(strSql).Tables[0];
        }
        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【出库日期】！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("【开始日期】大于【结束日期】，请重新输入！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            GridList.DataSource = DataSerach();
            GridList.DataBind();
        }
        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());
        }

        protected override void billOpen(string strBillno)
        {
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            decimal bzslTotal = 0, feeTotal = 0, dhslTotal = 0;
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno)).Tables[0];
            Doc.GridRowAdd(GridGoods, dtBill);
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["BZSL"]);
                    feeTotal += Convert.ToDecimal(row["HSJE"]);
                    dhslTotal += Convert.ToDecimal(row["DHSL"]);
                }
            }
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            summary.Add("DHSL", dhslTotal.ToString());
            GridGoods.SummaryData = summary;
            PubFunc.FormLock(FormDoc, true, "");
            if ((",M,R").IndexOf(docFLAG.SelectedValue) > 0 && hdfOper.Text != "audit") docMEMO.Enabled = true;
            TabStrip1.ActiveTabIndex = 2;
            //增加按钮控制
            if (docFLAG.SelectedValue == "M" || docFLAG.SelectedValue == "R")
            {
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnCommit.Enabled = true;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrt.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
            }
            else if (docFLAG.SelectedValue == "N")
            {
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnCommit.Enabled = true;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnPrt.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
            }
            else
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrt.Enabled = true;
                btnPrint.Enabled = true;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
            }
        }
        private void LoadGridRow(DataRow row, bool firstRow = true, string flag = "NEW")
        {
            if (flag == "NEW")
            {
                //写入批次信息,货位
                DataTable Temp = DbHelperOra.Query(string.Format("SELECT HWID,KCSL-LOCKSL KCSL,PH,YXQZ,RQ_SC FROM DAT_GOODSSTOCK WHERE DEPTID ='{0}' AND GDSEQ = '{1}' AND KCSL >LOCKSL AND ROWNUM = 1 ORDER BY PICINO ASC", docDEPTOUT.SelectedValue, row["GDSEQ"].ToString())).Tables[0];
                if (Temp.Rows.Count > 0)
                {
                    row["HWID"] = Temp.Rows[0]["HWID"];
                    row["KCSL"] = Temp.Rows[0]["KCSL"];
                    row["PH"] = Temp.Rows[0]["PH"];
                    row["YXQZ"] = Temp.Rows[0]["YXQZ"];
                    row["RQ_SC"] = Temp.Rows[0]["RQ_SC"];
                }
                else
                {
                    //货位写入(重新取值)
                    object objHWID = DbHelperOra.GetSingle(string.Format("SELECT F_GETHWID('{0}','{1}') FROM DUAL", docDEPTOUT.SelectedValue, row["GDSEQ"].ToString()));
                    row["HWID"] = (objHWID ?? "").ToString();
                    row["KCSL"] = "0";
                }
            }
            PubFunc.GridRowAdd(GridGoods, row, firstRow);
        }
        #endregion
        #region 界面传值
        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0) return;
            PubFunc.FormLock(FormDoc, true, "");
            docMEMO.Enabled = true;
            //参数说明：cx-查询内容，bm-商品配置部门,su-供应商
            string url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTID.SelectedValue + "&cx=&su=";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            dt.Columns.Remove(dt.Columns["BZHL"]);
            dt.Columns.Remove(dt.Columns["UNIT"]);
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                dt.Columns["BZHL_SELL"].ColumnName = "BZHL";
                dt.Columns["UNIT_SELL_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNIT_SELL"].ColumnName = "UNIT";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DHS", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt.Columns.Add("SPZTSL", Type.GetType("System.Int32"));
                dt.Columns.Add("KCSL", Type.GetType("System.Double"));

                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "0";
                    row["DHSL"] = "0";
                    row["HSJE"] = "0";
                    //处理金额格式
                    decimal jingdu = 0;
                    decimal bzhl = 0;
                    if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl))
                    { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                    if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F4"); }
                    if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }
                    LoadGridRow(row, false);
                }
            }
            else
            {
                Alert.Show("系统传值错误！！！", "消息提示", MessageBoxIcon.Warning);
            }
        }
        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            bool firstRow = true;
            foreach (GridRow row in GridLot.Rows)
            {
                int rowIndex = row.RowIndex;
                System.Web.UI.WebControls.TextBox tbxNumber = (System.Web.UI.WebControls.TextBox)GridLot.Rows[rowIndex].FindControl("tbxNumber");
                if (!string.IsNullOrWhiteSpace(tbxNumber.Text) && tbxNumber.Text != "0")
                {
                    if (!PubFunc.isNumeric(tbxNumber.Text))
                    {
                        Alert.Show("请输入正确的数字信息!", "操作提示", MessageBoxIcon.Warning);
                        return;
                    }
                    string[] strCell = GridGoods.SelectedCell;
                    JObject defaultObj = Doc.GetJObject(GridGoods, strCell[0]);
                    defaultObj["PH"] = row.Values[3].ToString();
                    defaultObj["YXQZ"] = row.Values[4].ToString();
                    defaultObj["PZWH"] = row.Values[9].ToString();
                    defaultObj["RQ_SC"] = row.Values[5].ToString();
                    defaultObj["KCSL"] = row.Values[6].ToString();
                    defaultObj["BZSL"] = tbxNumber.Text;
                    if (firstRow)
                    {
                        firstRow = false;
                        PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(strCell[0], defaultObj));
                    }
                    else
                    {
                        PageContext.RegisterStartupScript(GridGoods.GetAddNewRecordReference(defaultObj));
                    }
                }
            }
            WindowLot.Hidden = true;
        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            WindowLot.Hidden = true;
        }
        #endregion
        #region Grid计算
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            if (hdfOper.Text == "audit" || (",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                return;
            }
            string[] strCell = GridGoods.SelectedCell;
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0) return;
            if (e.ColumnID == "BZSL")
            {
                if (!PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZHL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZSL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "HSJJ")))
                {
                    Alert.Show("商品信息异常，请详细检查商品信息：包装含量、价格或数量！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
                decimal hl = 0, rs = 0, jg = 0;
                decimal.TryParse((defaultObj["BZHL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["BZSL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["DHSL"] = rs * hl;
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));

                //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0, dhslTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    if (dic["BZSL"] != null)
                    {
                        if (!PubFunc.StrIsEmpty(dic["BZSL"].ToString()))
                        {
                            bzslTotal += Convert.ToDecimal(dic["BZSL"]);
                            feeTotal += Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["BZSL"]);
                            dhslTotal += Convert.ToDecimal(dic["BZHL"]) * Convert.ToDecimal(dic["BZSL"]);
                        }
                    }
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));
                summary.Add("DHSL", dhslTotal.ToString());
                GridGoods.SummaryData = summary;
            }
            else if (e.ColumnID == "PH")
            {
                String gdseq = Doc.GetGridInf(GridGoods, e.RowID, "GDSEQ");
                if (gdseq.Length < 1)
                {
                    Alert.Show("请先选择商品信息！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                String ph = Doc.GetGridInf(GridGoods, e.RowID, "PH");
                if (ph.Length < 1)
                {
                    Alert.Show("请填写批次信息！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                if (ph.ToString() == "\\")
                {
                    DataTable dtPH = Doc.GetGoodsPH_New(gdseq, docDEPTOUT.SelectedValue);
                    if (dtPH != null && dtPH.Rows.Count > 0)
                    {
                        GridLot.DataSource = dtPH;
                        GridLot.DataBind();
                        WindowLot.Hidden = false;
                    }
                    else
                    {
                        Alert.Show("此商品已无库存,请检查！", MessageBoxIcon.Warning);
                    }
                }
            }
        }
        #endregion
        #region 单据审核驳回
        protected override void billAudit()
        {
            //住院办审核
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("单据未提交或者已审核！", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            //验证科室是否盘点
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID IN('" + docDEPTOUT.SelectedValue + "','" + docDEPTID.SelectedValue + "') AND FLAG='N'"))
            {
                Alert.Show("出库或申领科室正在盘点,请检查!", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            //高值商品验证
            string strBillno = docSEQNO.Text;
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_CK_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND A.SEQNO = '{0}' AND A.GDSEQ NOT IN(SELECT GDSEQ FROM DAT_CK_EXT WHERE BILLNO = '{0}')", strBillno)))
            {
                Alert.Show("出库单:" + docSEQNO.Text + "存在未扫描条码的高值商品，请检查!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //billSave();
            if (BillOper(strBillno, "AUDIT") == 1)
            {
                billLockDoc(true);
                Alert.Show("单据【" + strBillno + "】审核成功！");
                billOpen(strBillno);
            }
        }
        protected override void billCancel()
        {
            //将选中单据驳回
            if (!string.IsNullOrWhiteSpace(docSEQNO.Text))
            {
                string flag = DbHelperOra.GetSingle("SELECT FLAG FROM DAT_CK_DOC WHERE SEQNO ='" + docSEQNO.Text + "'").ToString();
                if (!string.IsNullOrWhiteSpace(flag) && (",N").IndexOf(flag) > 0)
                {
                    WindowReject.Hidden = false;
                }
                else
                {
                    Alert.Show("不是已提交单据,不能驳回", "操作提示", MessageBoxIcon.Warning);
                    return;
                }
            }
        }
        #endregion
        #region Excel导出
        protected void btnEpt_Click(object sender, EventArgs e)
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【出库日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("【开始日期】大于【结束日期】，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.BILLNO 单据编号,
                                       F_GETDEPTNAME(A.DEPTID) 申领部门,
                                       A.XSRQ 申领日期,
                                       F_GETDEPTNAME(A.DEPTOUT) 出库部门,
                                       F_GETUSERNAME(A.LRY) 录入人,
                                       A.LRRQ 录入日期,
                                       B.ROWNO 行号,
                                       B.GDSEQ 商品编码,
                                       B.GDNAME 商品名称,
                                       B.GDSPEC 商品规格,
                                       B.PZWH 注册证号,
                                       F_GETUNITNAME(B.UNIT) 单位,
                                       F_GETPRODUCERNAME(B.PRODUCER) 生产厂家,
                                       B.BZHL 包装含量,
                                       B.BZSL 出库包装数,
                                       B.XSSL 出库数,B.HSJJ 价格,B.PH 批号,B.RQ_SC 生产日期,B.YXQZ 有效期至
                                  FROM DAT_CK_DOC A, DAT_CK_COM B
                                 WHERE A.SEQNO=B.SEQNO
                                   AND A.BILLTYPE = '" + BillType + @"'
                                   AND A.XSTYPE = '1' 
                                   AND EXISTS(SELECT 1 FROM DAT_JH_COM B WHERE B.STR1 = A.SEQNO) ";
            string strSearch = "";


            if (tgbBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND UPPER(TRIM(A.BILLNO))  LIKE '%{0}%'", tgbBILLNO.Text.Trim().ToUpper());
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }

            strSearch += string.Format(" AND A.DEPTID IN( SELECT CODE FROM SYS_DEPT WHERE F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC,B.ROWNO";

            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            if (dt == null || dt.Rows.Count == 0)
            {
                Alert.Show("暂时没有符合要求的数据，无法导出", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            XTBase.Utilities.ExcelHelper.ExportByWeb(dt, "借货转出库借货信息", string.Format("借货转出库信息_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
        #endregion

        protected void btnCommit_Click(object sender, EventArgs e)
        {
            CommitData();
        }

        protected void btnScan_Click(object sender, EventArgs e)
        {
            //越库商品不允许退货？
            if ((",M,N").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新单』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请保存单据后进行扫描追溯码操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_CK_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND SEQNO = '{0}'", docSEQNO.Text)))
            {
                Alert.Show("此单据中没有已经保存的高值商品,请检查！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            WindowScan.Hidden = false;
            ScanSearch("SHOW");
        }

        protected void ScanSearch(string type)
        {
            string sql = "";
            if (type == "SHOW")
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_CK_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.GDSEQ,A.INSTIME DESC";
            }
            else
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_CK_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.INSTIME DESC";
            }
            DataTable dtScan = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            GridSacn.DataSource = dtScan;
            GridSacn.DataBind();
            zsmScan.Text = String.Empty;
            zsmScan.Focus();
        }

        protected void zsmScan_TextChanged(object sender, EventArgs e)
        {
            if ((",M,N").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新单』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (zsmScan.Text.Length < 28) return;
            if (zsmScan.Text.Substring(0, 1) != "2")
            {
                Alert.Show("您扫描的条码不是贵重码,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_EXT WHERE ONECODE = '{0}' AND FLAG = 'Y'", zsmScan.Text)))
            {
                Alert.Show("您输入的追溯码未被使用或已退货,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }

            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_CK_EXT WHERE ONECODE = '{0}' AND BILLNO ='{1}' AND FLAG = 'Y'", zsmScan.Text, docBILLNO.Text)))
            {
                Alert.Show("您输入的追溯码已扫描使用,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            //增加判断防止同一条条码同时写入两次 BY CONGWM 2016/10/18
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_CK_EXT WHERE ONECODE = '{0}' AND BILLNO ='{1}'", zsmScan.Text, docBILLNO.Text)))
            {
                Alert.Show("您输入的追溯码已经转出库或在列表中,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            //写入数据库中
            string sSQL = string.Format(@"INSERT INTO DAT_CK_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,INSTIME,FLAG,PH,RQ_SC,YXQZ)
                    SELECT '{0}','{1}',NVL((SELECT MAX(ROWNO)+1 FROM DAT_CK_EXT WHERE BILLNO = '{1}'),1),'{2}',GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,SYSDATE,'N',PH,RQ_SC,YXQZ
                    FROM DAT_RK_EXT A
                    WHERE A.ONECODE = '{2}' ", docDEPTID.SelectedValue, docBILLNO.Text, zsmScan.Text.Trim());
            DbHelperOra.ExecuteSql(sSQL);
            ScanSearch("");
        }

        protected void zsmDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(docBILLNO.Text.Trim()))
            {
                string flag = DbHelperOra.GetSingle("SELECT FLAG FROM DAT_CK_DOC WHERE BILLNO ='" + docBILLNO.Text.Trim() + "'").ToString();

                if ((",M,N").IndexOf(flag) < 0)
                {
                    Alert.Show("非『新增』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                if (GridSacn.SelectedCell == null)
                {
                    Alert.Show("请选择您需要删除的数据！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                string onecode = (GridSacn.DataKeys[GridSacn.SelectedRowIndex][0]).ToString();
                DbHelperOra.ExecuteSql(string.Format("DELETE FROM DAT_CK_EXT WHERE ONECODE = '{0}' AND BILLNO = '{1}'", onecode, docBILLNO.Text.Trim()));
                ScanSearch("");
            }
        }
        private bool CommitData()
        {
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请保存单据后提交！", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            string flag = DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M')  FROM DAT_CK_DOC WHERE BILLNO='{0}' AND BILLTYPE = 'CKD'", docSEQNO.Text.Trim())).ToString();
            if (("M,R").IndexOf(flag) < 0)
            {
                Alert.Show("非新单，不能提交！", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            string msg = "";
            List<CommandInfo> cmdList = new List<CommandInfo>();
            string sSQL = string.Format("SELECT SUM(ABS(A.DHSL)) DHSL,A.GDSEQ,B.GDNAME FROM DAT_CK_COM A,DOC_GOODS B ,DAT_CK_DOC C WHERE A.SEQNO = C.SEQNO AND A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND C.SEQNO = '{0}' AND C.BILLTYPE = 'CKD' GROUP BY A.GDSEQ,B.GDNAME", docSEQNO.Text);
            DataTable dtCom = DbHelperOra.Query(sSQL).Tables[0];
            if (dtCom != null && dtCom.Rows.Count > 0)
            {
                foreach (DataRow dr in dtCom.Rows)
                {
                    string checkCount = DbHelperOra.GetSingle(string.Format("SELECT COUNT(*) FROM DAT_CK_EXT WHERE BILLNO='{0}' AND GDSEQ ='{1}'", docSEQNO.Text, dr["GDSEQ"].ToString())).ToString();
                    if (int.Parse(dr["DHSL"].ToString()) > int.Parse(checkCount))
                    {
                        msg += "【" + dr["GDSEQ"] + "," + dr["GDNAME"] + "】,";
                        continue;
                    }
                }

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    Alert.Show("高值商品中 " + msg + "存在追溯码未扫描", "消息提示", MessageBoxIcon.Warning);
                    return false;
                }

                string sSQL1 = "CALL P_EXE_CKD('" + docSEQNO.Text + "')";
                cmdList.Add(new CommandInfo(sSQL1, null));
            }

            string sSQL2 = string.Format("UPDATE DAT_CK_DOC SET FLAG = 'N',SPR='{0}',SPRQ=sysdate WHERE BILLTYPE = 'CKD' AND SEQNO = '{1}'", UserAction.UserID, docSEQNO.Text);
            cmdList.Add(new CommandInfo(sSQL2, null));

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                billOpen(docSEQNO.Text);
                Alert.Show("单据【" + docSEQNO.Text + "】提交成功！");
                docFLAG.SelectedValue = "N";
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(docSEQNO.Text.Trim()))
            {
                string flag = DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M')  FROM DAT_CK_DOC WHERE BILLNO='{0}'", docSEQNO.Text.Trim())).ToString();

                if (flag == "Y")
                {
                    Alert.Show("已审核单据,不能驳回", "操作提示", MessageBoxIcon.Warning);
                    return;
                }

                string strMemo = "驳回原因：" + ddlReject.SelectedText;
                if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
                {
                    strMemo += "；详细说明：" + txaMemo.Text;
                }

                string sql = "update DAT_CK_DOC set flag='R',SHRQ=sysdate,memo ='" + strMemo + "',SHR='" + UserAction.UserID + "' where seqno ='" + docSEQNO.Text + "'";
                DbHelperOra.ExecuteSql(sql);
                WindowReject.Hidden = true;
                billSearch();
                docMEMO.Text = strMemo;
                docFLAG.SelectedValue = "R";
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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //绑定借货单
            string Sql = @"SELECT B.*,f_getdeptname(A.DEPTOUT) DEPTOUTNAME,f_getdeptname(A.DEPTID) DEPTIDNAME,A.SHRQ,A.MEMO,f_getusername(A.SLR) SLRNAME,
                                f_getunitname(B.UNIT) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,C.BAR3,
                                NVL((SELECT ROUND(SUM(KCSL - LOCKSL)/F_GETBZHL(K.GDSEQ),2) FROM Dat_Goodsstock k where k.kcsl> LOCKSL and k.gdseq = b.gdseq and k.deptid = a.deptout GROUP BY GDSEQ),0) KCSL
                        FROM DAT_JH_DOC A,DAT_JH_COM B,DOC_GOODS C
                        WHERE A.SEQNO = B.SEQNO AND B.GDSEQ = C.GDSEQ AND A.FLAG = 'Y' AND NVL(B.STR1,'#') = '#'";
            string strch = "";
            if (!PubFunc.StrIsEmpty(tgbBILL.Text.Trim())) { strch += string.Format(" AND A.SEQNO LIKE '%{0}%'", tgbBILL.Text.Trim()); }
            if (!PubFunc.StrIsEmpty(tbxGDSEQ.Text.Trim())) { strch += string.Format(" AND (C.GDSEQ LIKE '%{0}%' OR C.GDNAME LIKE '%{0}%')", tbxGDSEQ.Text.Trim()); }
            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) { strch += string.Format(" AND A.DEPTID = '{0}'", ddlDEPTID.SelectedValue); }
            if (!PubFunc.StrIsEmpty(ddlDEPTOUT.SelectedValue)) { strch += string.Format(" AND A.DEPTOUT = '{0}'", ddlDEPTOUT.SelectedValue); }
            strch += string.Format(" AND A.SHRQ BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND TO_DATE('{1}','YYYY-MM-DD')+1", dpkTime1.Text, dpkTime2.Text);
            strch += " ORDER BY A.DEPTOUT,A.DEPTID,A.SEQNO DESC,B.ROWNO";

            DataTable Dt = DbHelperOra.Query(Sql + strch).Tables[0];
            GridLeft.DataSource = Dt;
            GridLeft.DataBind();
            if (Dt.Rows.Count > 0)
            {
                btnCreate.Enabled = true;
            }
            else
            {
                btnCreate.Enabled = false;
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            int[] selections = GridLeft.SelectedRowIndexArray;
            if (selections.Count() < 1)
            {
                Alert.Show("请选择需要转为出库单的商品！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            string SEQ = DbHelperOra.GetSingle("SELECT SEQ_PUBLIC.NEXTVAL FROM DUAL").ToString();
            foreach (int rowIndex in selections)
            {
                //增加库存数量的判断
                if (Convert.ToDecimal(GridLeft.DataKeys[rowIndex][2]) > Convert.ToDecimal(GridLeft.DataKeys[rowIndex][3]))
                {
                    Alert.Show("第【" + (rowIndex + 1) + "】行商品，库存数不足以出库,，请检查！");
                    return;
                }
                cmdList.Add(new CommandInfo(string.Format("UPDATE DAT_JH_COM SET NUM1 = {0} WHERE SEQNO = '{1}' AND ROWNO = {2} AND NVL(STR1，'#') = '#'", SEQ, GridLeft.DataKeys[rowIndex][0], GridLeft.DataKeys[rowIndex][1]), null));
            }
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {

                OracleParameter[] parameters ={
                                            new OracleParameter("VIN_SEQ" ,OracleDbType.Varchar2,10),
                                            new OracleParameter("VIN_OPERUSER" ,OracleDbType.Varchar2,10),
                                           };
                parameters[0].Value = SEQ;
                parameters[1].Value = UserAction.UserID;
                try
                {
                    DbHelperOra.RunProcedure("STORE.P_BowToCk", parameters);
                    Alert.Show("借条转出库单生成成功！", "消息提示", MessageBoxIcon.Information);
                    billSearch();
                    TabStrip1.ActiveTabIndex = 1;
                }
                catch (Exception err)
                {
                    throw err;
                }
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tgbBILL.Text = "";
            ddlDEPTOUT.SelectedValue = "";
            dpkTime1.SelectedDate = DateTime.Now.AddDays(-1);
            dpkTime2.SelectedDate = DateTime.Now;
            tbxGDSEQ.Text = "";
            ddlDEPTID.SelectedValue = "";
        }
    }
}
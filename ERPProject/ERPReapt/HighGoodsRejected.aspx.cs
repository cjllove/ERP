﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace ERPProject.ERPReapt
{
    public partial class HighGoodsRejected : BillBase
    {
        #region Load加载
        private string strDocSql = "SELECT * FROM DAT_TH_DOC WHERE SEQNO ='{0}'";
        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docDEPTID, docTHRQ }; }
        }

        public HighGoodsRejected()
        {
            BillType = "THD";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                billNew();
                if (Request.QueryString["oper"] != null)
                {
                    hfdOper.Text = Request.QueryString["oper"].ToString();
                    if (Request.QueryString["oper"].ToString() == "input")
                    {
                        ButtonHidden(btnAudit, btnCancel, btnScan, btnPrt, btnPrint);
                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        ButtonHidden(btnNew, btnDelRow, btnDel, btnSave, btnGoods, btnAllCommit, btnCommit);
                        // ButtonHidden(btnNew, btnDel, btnSave, btnCommit, btnPrt,btnPrint);
                    }
                }

            }
        }
        private void DataInit()
        {
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet("DDL_USER", docSHR, docLRY, docCGY);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
            PubFunc.DdlDataGet("DDL_DOC_SHS", lstPSSID, docPSSID);
            DepartmentBind.BindDDL("DDL_SYS_DEPOTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
            PubFunc.DdlDataGet("DDL_USER", lstCGY, docLRY, docSHR, docCGY);
            //PubFunc.DdlDataGet("DDL_SYS_DEPOT", lstDEPTID, docDEPTID);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
            PubFunc.DdlDataGet("DDL_BILL_FLAG_MNYR", docFLAG, lstFLAG);
            PubFunc.DdlDataGet(docTHTYPE, "DDL_RETURNREASON");

            //docDEPTOUT.Items.RemoveAt(0);
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                FineUIPro.BoundField flagcol = GridList.FindColumn("FLAG_CN") as FineUIPro.BoundField;
                if (flag == "M")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color1";
                }
                if (flag == "N")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color2";
                }
                if (flag == "R")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color3";
                }
            }
        }
        #endregion
        #region 增删改
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
        protected override void billNew()
        {
            //原单据保存判断
            string strDeptID = docDEPTID.SelectedValue;

            PubFunc.FormDataClear(FormDoc);

            docFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            docCGY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docTHRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDeptID;

            billLockDoc(false);
            docMEMO.Enabled = true;

            docCGY.Enabled = true;
            //清空Grid行
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            btnDel.Enabled = false;
            btnSave.Enabled = true;
            docPSSID.Enabled = true;
            docTHTYPE.Enabled = true;
            btnCommit.Enabled = false;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrt.Enabled = false;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            trbBARCODE.Enabled = true;

            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", "0");
            summary.Add("HSJE", "0");
            summary.Add("DHSL", "0");
            GridGoods.SummaryData = summary;
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
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "M", BillType) && !Doc.getFlag(strBillno, "R", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridGoods.SelectedCell == null)
            {
                Alert.Show("未选择任何行，无法进行【删行】操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            else
            {
                GridGoods.DeleteSelectedRows();
            }

        }
        protected override void billSave()
        {
            save();
        }

        private void save(string  flag="N")
        {
            #region 数据有效性验证
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!CheckFlag("M") && !CheckFlag("R"))
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();//.OrderBy(x => x["GDSEQ"]).ToList(); //排序这里给关掉
            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {
                    if (string.IsNullOrWhiteSpace(newDict[i]["BZHL"].ToString()) || string.IsNullOrWhiteSpace(newDict[i]["UNIT"].ToString()))
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]包装单位信息错误，请联系管理员维护！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if ((newDict[i]["BZSL"] ?? "").ToString() == "" || (newDict[i]["BZSL"] ?? "0").ToString() == "0")
                    {
                        Alert.Show("请填写商品[" + newDict[i]["GDSEQ"] + "]退货数！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    //if (((newDict[i]["HSJJ"] ?? "").ToString() == "" || (newDict[i]["HSJJ"] ?? "").ToString() == "0") && ((newDict[i]["NUM1"] ?? "").ToString() != "1"))
                    if (((newDict[i]["HSJJ"] ?? "").ToString() == "" || (newDict[i]["HSJJ"] ?? "").ToString() == "0"))
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]的【含税进价】不能为零,请维护好商品的含税进价，再次保存！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (newDict[i]["ISLOT"].ToString() == "1" || newDict[i]["ISLOT"].ToString() == "2")
                    {
                        if (string.IsNullOrWhiteSpace(newDict[i]["PH"].ToString()))
                        {
                            string[] selectedCell = GridGoods.SelectedCell;
                            PageContext.RegisterStartupScript(String.Format("F('{0}').selectCell('{1}','{2}');", GridGoods.ClientID, selectedCell[0], selectedCell[1]));
                            Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】批号不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                            return;
                        }
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
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'N') FROM DAT_TH_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
                if (!string.IsNullOrWhiteSpace(flg) && (",M,R").IndexOf(flg) < 0)
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
            MyTable mtType = new MyTable("DAT_TH_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow["FLAG"] = "M";
            mtType.ColRow["NUM1"] = "2";
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("SUPNAME", docPSSID.SelectedText);
            mtType.ColRow.Add("DEPTDH", mtType.ColRow["DEPTID"]);

            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_TH_COM");
            decimal subNum = 0;//总金额
                               //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_TH_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_TH_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            cmdList.Add(new CommandInfo("delete DAT_TH_EXT where billno='" + docBILLNO.Text + "'", null));
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);
                mtTypeMx.ColRow["BZSL"] = Math.Abs(decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString())) * -1;
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow["THSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow.Add("SSSL", mtTypeMx.ColRow["THSL"]);
                mtTypeMx.ColRow.Add("DEPTID", docDEPTID.SelectedValue);
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                cmdList.Add(mtTypeMx.Insert());
                //cmdList.Add(new CommandInfo("delete DAT_TH_EXT where onecode='" + mtTypeMx.ColRow["STR2"].ToString() + "'", null));
                cmdList.Add(new CommandInfo(String.Format(@"INSERT INTO DAT_TH_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,FLAG,PH,RQ_SC,YXQZ)
                        SELECT '{1}','{2}',B.ROWNO,A.ONECODE,A.GDSEQ,A.GDNAME,A.BARCODE,A.UNIT,A.GDSPEC,A.DEPTCUR,A.BZHL,A.FLAG,A.PH,A.RQ_SC,A.YXQZ
                          FROM DAT_RK_EXT A,DAT_TH_COM B
                         WHERE A.GDSEQ = B.GDSEQ AND A.ONECODE = '{0}' AND A.ONECODE = B.STR2 ", mtTypeMx.ColRow["STR2"].ToString(), docDEPTID.SelectedValue, docBILLNO.Text), null));
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            //     cmdList.Add(new CommandInfo("delete DAT_TH_EXT where billno='" + docBILLNO.Text + "'", null));单据编号发生了变化，这条语句无法删除DAT_TH_EXT表中已经存在的ONECODE,ONECODE是唯一键，执行会报错
            //            cmdList.Add(new CommandInfo(String.Format(@"INSERT INTO DAT_TH_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,FLAG,PH,RQ_SC,YXQZ)
            //                        SELECT '{1}','{0}',B.ROWNUM,A.ONECODE,A.GDSEQ,A.GDNAME,A.BARCODE,A.UNIT,A.GDSPEC,A.DEPTCUR,A.BZHL,A.FLAG,A.PH,A.RQ_SC,A.YXQZ
            //                          FROM DAT_RK_EXT A,DAT_TH_COM B
            //                         WHERE A.GDSEQ = B.GDSEQ AND B.SEQNO = '{0}' AND A.ONECODE = B.STR2 ", docBILLNO.Text, docDEPTID.SelectedValue), null));
            cmdList.AddRange(mtType.InsertCommand());
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                if(flag == "N")
                    Alert.Show("商品退货信息保存成功！");
                //   billOpen(docBILLNO.Text);
                billLockDoc(true);
                billOpen(docSEQNO.Text);
                OperLog("商品退货", "修改单据【" + docBILLNO.Text + "】");
                SaveSuccess = true;
            }
            else
            {
                Alert.Show("商品退货信息保存失败，请联系管理员！");
            }
        }

        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据");
                return;
            }
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非新单不能删除!");
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "M", BillType) && !Doc.getFlag(strBillno, "R", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            DbHelperOra.ExecuteSql("Delete from DAT_TH_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_TH_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_TH_EXT t WHERE T.BILLNO ='" + docBILLNO.Text.Trim() + "'");
            // DbHelperOra.ExecuteSql("Delete from DAT_RK_EXT t WHERE T.BILLNO ='" + docBILLNO.Text.Trim() + "'");
            Alert.Show("单据删除成功!");
            OperLog("商品退货", "删除单据【" + docBILLNO.Text.Trim() + "】");
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

        protected override void billCancel()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("只能驳回已提交的单据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "N", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            WindowReject.Hidden = false;
        }
        #region 查询
        protected void tgbBILLNO_TriggerClick(object sender, EventArgs e)
        {
            billSearch();
        }
        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【使用日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO, F_GETBILLFLAG(FLAG) FLAG_CN, FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,A.THRQ,F_GETSUPNAME(A.PSSID) SUPNAME,A.SUBSUM,
                                     A.SUBNUM,F_GETUSERNAME(A.CGY) CGY,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO
                                from DAT_TH_DOC A
                                WHERE NVL(A.NUM1,0) = 2 AND A.DEPTID in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '" + UserAction.UserID + "') = 'Y' ) ";
            string strSearch = "";


            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND UPPER(TRIM(A.BILLNO))  LIKE '%{0}%'", lstBILLNO.Text.Trim().ToUpper());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstCGY.SelectedItem != null && lstCGY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.CGY='{0}'", lstCGY.SelectedItem.Value);
            }

            if (lstPSSID.SelectedItem != null && lstPSSID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.PSSID='{0}'", lstPSSID.SelectedItem.Value);
            }
            //过滤新单的单据，不能审核，提交后的才能审核
            if (Request.QueryString["oper"].ToString() == "audit")
            {
                strSearch += " AND A.FLAG<>'M'";
            }
            //  strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.THRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.THRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC";
            GridList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataBind();
        }
        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());
        }

        protected override void billOpen(string strBillno)
        {
            string sql = @"SELECT A.*,F_GETUNITNAME(A.UNIT) UNITNAME,
                                  F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                  F_GETSUPNAME(A.SUPID) SUPNAME,
                                  (select NVL(SUM(KCSL), 0)
                                      from DAT_GOODSSTOCK
                                      where GDSEQ = A.GDSEQ
                                      and DEPTID = B.DEPTID) KCSL,f_getunitname(C.UNIT) UNITSMALLNAME ,DECODE(NVL(A.NUM1,0),0,'非赠品','赠品') NUM1NAME
                              FROM DAT_TH_COM A,DAT_TH_DOC B,DOC_GOODS C
                              WHERE A.SEQNO = B.SEQNO AND A.SEQNO = '{0}' AND A.GDSEQ=C.GDSEQ ORDER BY A.ROWNO";
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            decimal bzslTotal = 0, feeTotal = 0, dhslTotal = 0;
            DataTable dtBill = DbHelperOra.Query(string.Format(sql, strBillno)).Tables[0];
            Doc.GridRowAdd(GridGoods, dtBill);
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["BZSL"]);
                    feeTotal += Convert.ToDecimal(row["HSJE"]);
                    dhslTotal += Convert.ToDecimal(row["THSL"]);
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
            if ((",M,R").IndexOf(docFLAG.SelectedValue) > 0) docMEMO.Enabled = true;
            TabStrip1.ActiveTabIndex = 1;
            //增加按钮控制
            if (docFLAG.SelectedValue == "M" || docFLAG.SelectedValue == "R")
            {
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnCommit.Enabled = true;
                btnAudit.Enabled = true;
                btnCancel.Enabled = false;
                btnPrt.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
                trbBARCODE.Enabled = true;
            }
            else if (docFLAG.SelectedValue == "N")
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnAudit.Enabled = true;
                btnCommit.Enabled = false;
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
            string url = "~/ERPQuery/GoodsWindow_Stock.aspx?Deptout=" + docDEPTID.SelectedValue + "&Shs=" + docPSSID.SelectedValue;
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
            hdfZP.Text = "";
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0) return;

            //处理返回jobject
            JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
            //string cell = string.Format("[{0},{1}]", e.RowIndex, intCell[1]);
            if (e.ColumnID == "BZSL")
            {

                if (!PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZHL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZSL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "HSJJ")))
                {
                    Alert.Show("商品信息异常，请详细检查商品信息：包装含量或价格！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }

                if ((Convert.ToInt32(defaultObj["BZSL"]) > Convert.ToInt32(defaultObj["KCSL"])) ||
                    (Convert.ToInt32(defaultObj["BZSL"]) * Convert.ToInt32(defaultObj["BZHL"]) > Convert.ToInt32(defaultObj["KCSL"])))
                {
                    Alert.Show("退货数量(最小包装)不能大于库存数量！", "异常信息", MessageBoxIcon.Warning);
                    defaultObj["BZSL"] = "0";
                }

                decimal hl = 0, rs = 0, jg = 0;
                decimal.TryParse((defaultObj["BZHL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["BZSL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["THSL"] = rs * hl;
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                #region //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0, thslTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    if ((dic["BZSL"] ?? "0").ToString().Length > 0 && (dic["HSJJ"] ?? "0").ToString().Length > 0)
                    {
                        bzslTotal += Convert.ToDecimal(dic["BZSL"] ?? "0");
                        feeTotal += Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0");
                        thslTotal += Convert.ToDecimal(dic["BZHL"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0");
                    }

                    object objISFLAG5 = DbHelperOra.GetSingle(string.Format("SELECT ISFLAG5 FROM DOC_GOODS WHERE GDSEQ = '{0}'", dic["GDSEQ"]));

                    if (objISFLAG5.ToString() == "N")
                    {
                        string str = Convert.ToString(Convert.ToDecimal(dic["BZSL"] ?? "0"));
                        if (Convert.ToDecimal(dic["BZSL"]) != (int)Convert.ToDecimal(dic["BZSL"]) && Convert.ToDecimal(dic["BZHL"] ?? "0") == 1)
                        {
                            Alert.Show("当前商品不支持申领数为小数，请调整", "消息提示", MessageBoxIcon.Warning);

                        }
                    }
                }

                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));
                summary.Add("THSL", thslTotal.ToString("F2"));

                GridGoods.SummaryData = summary;
                #endregion
            }

        }
        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            dt.Columns.Remove(dt.Columns["BZHL"]);
            dt.Columns.Remove(dt.Columns["UNIT"]);
            if (dt != null && dt.Rows.Count > 0)
            {
                //dt.Columns["PIZNO"].ColumnName = "PZWH";
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
                dt.Columns.Add("NUM1", Type.GetType("System.Int32"));
                dt.Columns.Add("NUM1NAME", Type.GetType("System.String"));

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
        #endregion
        #region Grid计算
        #endregion
        #region 单据审核驳回
        protected override void billAudit()
        {
            //住院办审核
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非『已提交』单据不能审核！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //验证是否盘点
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID = '" + docDEPTID.SelectedValue + "' AND FLAG='N'"))
            {
                Alert.Show(string.Format("部门【{0}】正在盘点,请盘点后进行操作！", docDEPTID.SelectedText), "警告提示", MessageBoxIcon.Warning);
                return;
            }

            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(docBILLNO.Text, "N", BillType))
            {
                Alert.Show("此单据已经被别人操作,不能审核！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (BillOper(strBillno, "AUDIT") == 1)
                {
                    billLockDoc(true);
                    Alert.Show("单据【" + strBillno + "】审核成功！");
                    billOpen(strBillno);
                    OperLog("商品退货", "审核单据【" + strBillno + "】");
                }
            }
            catch (Exception ex)
            {
                Alert.Show(ex.Message);
            }
        }
        #endregion
        #region Excel导出
        protected void btnEpt_Click(object sender, EventArgs e)
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

            string strSql = @"SELECT A.BILLNO 单据编号,
                                       F_GETDEPTNAME(A.DEPTDH) 申领部门,
                                       A.XSRQ 申领日期,
                                       F_GETDEPTNAME(A.DEPTID) 出库部门,
                                       F_GETUSERNAME(A.SLR) 申领人,
                                       F_GETUSERNAME(A.LRY) 录入人,
                                       A.LRRQ 录入日期,
                                       B.ROWNO 行号,
                                       B.GDSEQ 商品编码,
                                       B.GDNAME 商品名称,
                                       B.GDSPEC 商品规格,
                                       B.PZWH 注册证号,
                                       F_GETUNITNAME(B.UNIT) 单位,
                                       F_GETPRODUCERNAME(B.PRODUCER) 生产厂家,
                                       B.STR2 高值条码,
                                       B.BZHL 包装含量,
                                       B.BZSL 申领包装数,
                                       B.XSSL 申领数,B.HSJJ 价格,B.PH 批号,B.RQ_SC 生产日期,B.YXQZ 有效期至
                                  FROM DAT_TH_DOC A, DAT_TH_COM B
                                 WHERE A.SEQNO=B.SEQNO
                                   AND A.BILLTYPE = '" + BillType + @"'
                                   --AND A.XSTYPE = 'G' ";
            string strSearch = "";


            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }

            strSearch += string.Format(" AND A.deptid in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
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
            XTBase.Utilities.ExcelHelper.ExportByWeb(dt, "科室出库信息", string.Format("科室申领信息_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
        }
        #endregion
        protected void btnScan_Click(object sender, EventArgs e)
        {
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
        }

        protected void zsmDelete_Click(object sender, EventArgs e)
        {
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
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
            string sSQL = string.Format("DELETE FROM DAT_GZ_EXT WHERE ONECODE = '{0}' AND BILLNO ='{1}'", onecode, docBILLNO.Text);
            DbHelperOra.ExecuteSql(sSQL);
            ScanSearch("");
            OperLog("商品退货", "修改单据【" + docBILLNO.Text + "】高值码");
            //int SelectedIndex = GridGoods.SelectedRowIndex;
            //PageContext.RegisterStartupScript(GridGoods.GetDeleteRowReference(SelectedIndex));
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

        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }

        protected void trbBARCODE_TriggerClick(object sender, EventArgs e)
        {
            //扫码出库
            if (trbBARCODE.Text.Trim().Length < Doc.LENCODE()) return;
            //if (docDEPTID.SelectedValue.Length < 1)
            //{
            //    Alert.Show("【退货科室】未维护", "提示信息", MessageBoxIcon.Warning);
            //    return;
            //}
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            for (int i = 0; i < newDict.Count; i++)
            {
                if (newDict[i]["STR2"].ToString() == trbBARCODE.Text.Trim())
                {
                    Alert.Show("扫描条码已在单据中存在！", "提示信息", MessageBoxIcon.Warning);
                    trbBARCODE.Text = "";
                    trbBARCODE.Focus();
                    return;
                }
            }
            DataTable dt = DbHelperOra.Query("SELECT A.* FROM DAT_GZ_EXT A,SYS_DEPT B WHERE A.DEPTCUR = B.CODE  AND B.TYPE = '1' AND A.ONECODE = '" + trbBARCODE.Text.Trim() + "' AND A.FLAG IN('Y','R')").Tables[0];
            if (dt.Rows.Count < 1)
            {
                Alert.Show("扫描条码未入库或已被退货!", "提示信息", MessageBoxIcon.Warning);
                trbBARCODE.Text = "";
                trbBARCODE.Focus();
                return;
            }
            string msg = "";
            DataTable dtt = DbHelperOra.Query(string.Format("SELECT ONECODE,BILLNO,DEPTID,f_getdeptname(deptid) DEPTNAME FROM DAT_TH_EXT WHERE ONECODE = '{0}'", trbBARCODE.Text.Trim())).Tables[0];
            if (dtt.Rows.Count>0)
            {
                msg = "追溯码【" + dtt.Rows[0]["ONECODE"] + "】在【[" + dtt.Rows[0]["DEPTID"] + "]" + dtt.Rows[0]["DEPTNAME"] + "】被单据【" + dtt.Rows[0]["BILLNO"] + "】使用！";
                Alert.Show(msg, "提示信息", MessageBoxIcon.Warning);
                return;
            }
            string dept = dt.Rows[0]["DEPTCUR"].ToString();
            if (docDEPTID.SelectedValue.Length > 0)
            {
                if (docDEPTID.SelectedValue != dept)
                {
                    Alert.Show("条码【" + trbBARCODE.Text.Trim() + "】不属于库房【" + docDEPTID.SelectedText + "】", "提示信息", MessageBoxIcon.Warning);
                    trbBARCODE.Text = "";
                    trbBARCODE.Focus();
                    return;
                }
            }
            else
            {
                docDEPTID.SelectedValue = dept;
            }
            docDEPTID.Enabled = false;
            String strPSSID = dt.Rows[0]["PSSID"].ToString();
            //if (strPSSID == "00001")
            //{
            //    Alert.Show("代管商品不允许退货！", MessageBoxIcon.Warning);
            //    trbBARCODE.Text = "";
            //    trbBARCODE.Focus();
            //    return;
            //}
            string gdseq = dt.Rows[0]["GDSEQ"].ToString();
            //增加商品
            //信息赋值
            DataTable dt_goods = Doc.GetGoods_His(gdseq, "", docDEPTID.SelectedValue);
            if (dt_goods != null && dt_goods.Rows.Count > 0)
            {
                string gys = DbHelperOra.GetSingle(String.Format(@"SELECT A.PSSID
                                FROM DAT_GZ_EXT A
                                WHERE A.ONECODE = '{0}'", trbBARCODE.Text.Trim()) ?? "").ToString();
                if (docPSSID.SelectedValue.Length > 0)
                {
                    if (docPSSID.SelectedValue != gys)
                    {
                        Alert.Show("条码【" + trbBARCODE.Text.Trim() + "】不属于配送商【" + docPSSID.SelectedText + "】", "提示信息", MessageBoxIcon.Warning);
                        trbBARCODE.Text = "";
                        trbBARCODE.Focus();
                        return;
                    }
                }
                else
                {
                    docPSSID.SelectedValue = gys;
                    docPSSID.Enabled = false;
                }
                dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt_goods.Columns.Add("STR2", Type.GetType("System.String"));
                DataRow dr_goods = dt_goods.Rows[0];
                //退货价格重新获取
                dr_goods["HSJJ"] = DbHelperOra.GetSingle(String.Format("SELECT HSJJ FROM (SELECT HSJJ FROM DAT_ONECODEJXC A WHERE A.ONECODE = '{0}' AND (A.BILLNO LIKE 'RKD%' OR A.BILLNO LIKE 'YKD%') ORDER BY SEQNO DESC) WHERE ROWNUM = 1", trbBARCODE.Text.Trim()));
                dr_goods["BZSL"] = "1";
                dr_goods["BZHL"] = "1";
                dr_goods["STR2"] = trbBARCODE.Text.Trim();
                dr_goods["HSJE"] = dr_goods["HSJJ"];
                dr_goods["PH"] = dt.Rows[0]["PH"];
                dr_goods["YXQZ"] = dt.Rows[0]["YXQZ"];
                dr_goods["RQ_SC"] = dt.Rows[0]["RQ_SC"];

                LoadGridRow(dr_goods, false);
            }
            else
            {
                Alert.Show(string.Format("{0}尚未配置商品【{1}】！！！", docDEPTID.SelectedText, gdseq), MessageBoxIcon.Warning);
            }
            trbBARCODE.Text = "";
            trbBARCODE.Focus();
        }
        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(docBILLNO.Text.Trim()))
            {
                string flag = DbHelperOra.GetSingle("SELECT FLAG FROM DAT_TH_DOC WHERE BILLNO ='" + docBILLNO.Text.Trim() + "'").ToString();

                if ((",N").IndexOf(flag) < 0)
                {
                    Alert.Show("只能驳回已提交的单据！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
            }


            List<CommandInfo> cmdList = new List<CommandInfo>();


            if (docBILLNO.Text.Length < 1)
            { return; }

            if (string.IsNullOrWhiteSpace(ddlReject.SelectedValue))
            {
                Alert.Show("请选择驳回原因", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (txaMemo.Text.Length > 140)
            {
                Alert.Show("驳回原因超长！", MessageBoxIcon.Warning);
                return;
            }
            string strMemo = "驳回原因:" + ddlReject.SelectedText;
            if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
            {
                strMemo += ";详细说明:" + txaMemo.Text;
            }
            if (DbHelperOra.ExecuteSql(string.Format("update DAT_TH_DOC set flag='R',memo='{0}' where seqno='{1}' and flag='N'", strMemo, docBILLNO.Text)) == 1)
            {
                WindowReject.Hidden = true;
                OperLog("商品退货", "驳回单据【" + docBILLNO.Text + "】，" + strMemo);
                billOpen(docBILLNO.Text);
                docFLAG.SelectedValue = "R";
            };
            //cmdList.Add(new CommandInfo("delete DAT_TH_EXT where BILLNO='" + docBILLNO.Text.Trim() + "'", null));
            DbHelperOra.ExecuteSqlTran(cmdList);
        }
        private bool CheckFlag(string flag)
        {
            if (docBILLNO.Text.Length > 0)
            {
                return Doc.getFlag(docBILLNO.Text, flag, BillType);
            }
            return true;
        }
        protected void btnAllCommit_Click(object sender, EventArgs e)
        {
            List<CommandInfo> sqlList = new List<CommandInfo>();
            string succeed = string.Empty;
            for (int i = 0; i < GridList.SelectedRowIndexArray.Length; i++)
            {
                int rowIndex = GridList.SelectedRowIndexArray[i];
                if (GridList.DataKeys[rowIndex][1].ToString() == "M")
                {
                    string strBILLNO = GridList.DataKeys[rowIndex][0].ToString();
                    sqlList.Add(new CommandInfo("update DAT_TH_DOC set flag='N' where BILLNO='" + strBILLNO + "' ", null));
                    if (DbHelperOra.Exists(string.Format(@"SELECT 1 
                                    FROM (SELECT A.GDSEQ,A.PH,SUM(A.THSL) SL
                                    FROM DAT_TH_COM A,DOC_GOODS B
                                    WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ IN ('Y','1') AND A.SEQNO = '{0}'
                                    GROUP BY A.GDSEQ,A.PH)
                                    WHERE (GDSEQ,PH,SL) NOT IN(SELECT GDSEQ,PH,COUNT(1) SL FROM DAT_TH_EXT WHERE BILLNO = '{0}' GROUP BY GDSEQ,PH)", strBILLNO)))
                    {
                        Alert.Show("单据【" + strBILLNO + "】高值码扫描不正确，请检查！");
                        return;
                    }

                    if (DbHelperOra.ExecuteSqlTran(sqlList))
                    {
                        succeed = succeed + "【" + strBILLNO + "】";
                    }
                }
            }
            if (succeed.Length > 0)
            {
                Alert.Show("单据" + succeed + "提交成功！");
                billSearch();
                OperLog("商品退货", "提交单据" + succeed + "");
            }
            else
            {
                Alert.Show("你选中的单据中，没有要提交的单据");
            }


        }
        private bool SaveSuccess = false;
        /// <summary>
        /// 20150510   liuz  增加提交操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCommit_Click(object sender, EventArgs e)
        {

            string strBILLNO = docBILLNO.Text;
            List<CommandInfo> sqlList = new List<CommandInfo>();
            if (strBILLNO.Length == 0)
            {
                Alert.Show("请先保存后，再次提交！");
                return;
            }
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("只有保存后单据，才能提交！");
                return;
            }
            SaveSuccess = false;
            save("Y");
            if (SaveSuccess == false)
                return;
            SaveSuccess = false;
            //提交的时候增加高值商品的扫码
            Object exis = DbHelperOra.GetSingle(string.Format(@"SELECT GDSEQ 
                FROM (SELECT A.GDSEQ,A.PH,SUM(abs(A.THSL)) SL
                FROM DAT_TH_COM A,DOC_GOODS B
                WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ IN ('Y','1') AND A.SEQNO = '{0}'
                GROUP BY A.GDSEQ,A.PH)
                WHERE (GDSEQ,PH,SL) NOT IN(SELECT GDSEQ,PH,COUNT(1) SL FROM DAT_TH_EXT WHERE BILLNO = '{0}' GROUP BY GDSEQ,PH) AND ROWNUM = 1", strBILLNO));
            if (!PubFunc.StrIsEmpty((exis ?? "").ToString()))
            {
                Alert.Show("提交失败，请检查商品【" + exis + "】高值码扫描的追溯码与退货数和批号是否不一致！");
                return;
            }
            //List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            //for (int i = 0; i < newDict.Count; i++)
            //{
            //    //增加贵重物品验证   && newDict[i]["STR1"].ToString() == ""
            //    if (DbHelperOra.Exists("select 1 from doc_goods where GDSEQ = '" + newDict[i]["GDSEQ"] + "' and ISGZ in('Y','1')") && !DbHelperOra.Exists("select 1 from DAT_TH_EXT where billno ='" + strBILLNO + "' and GDSEQ = '" + newDict[i]["GDSEQ"] + "'"))
            //    {
            //        Alert.Show("单据中存在贵重物品,请进行追溯码扫描!");
            //        return;
            //    }
            //}
            sqlList.Add(new CommandInfo("update DAT_TH_DOC set flag='N' where BILLNO='" + strBILLNO + "' ", null));


            if (DbHelperOra.ExecuteSqlTran(sqlList))
            {
                Alert.Show("此单据提交成功！");
                billSearch();
                billOpen(docBILLNO.Text);
                billLockDoc(true);
                OperLog("商品退货", "提交单据【" + strBILLNO + "】");
            }
            else
            {
                Alert.Show("此单据提交失败，请联系系统管理人员！");
            }

        }
    }
}
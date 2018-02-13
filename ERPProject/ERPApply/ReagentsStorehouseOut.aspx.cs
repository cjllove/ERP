﻿using FineUIPro;
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
    public partial class ReagentsStorehouseOut : BillBase
    {
        private string strDocSql = "SELECT A.*,STR5 DEPTGROUP,SUBNUM NUM,'' BARCODE FROM DAT_XS_DOC A WHERE XSTYPE='3' AND SEQNO ='{0}'";
        private string strComSql = @"SELECT A.*,F_GETUNITNAME(A.UNIT) UNITNAME,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,F_GETUNITNAME(C.UNIT) UNITSMALLNAME 
                                                     FROM DAT_XS_COM A,DAT_XS_DOC B,DOC_GOODS  C
                                                   WHERE A.SEQNO ='{0}' AND A.SEQNO=B.SEQNO AND B.XSTYPE='3' AND A.GDSEQ = C.GDSEQ ORDER BY A.ROWNO";

        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docDEPTID, docXSRQ }; }
        }

        public ReagentsStorehouseOut()
        {
            BillType = "XSD";
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
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;

            PubFunc.DdlDataGet(docLRY, "DDL_USER");
            //DepartmentBind.BindDDL("DDL_SYS_DEPTGROUPHASATH", UserAction.UserID, );
            PubFunc.DdlDataGet("DDL_SYS_DEPTGROUPHASATH", docDEPTGROUP, lstDEPTID);
            PubFunc.DdlDataGet(docFLAG, "DDL_BILL_STATUS");
        }

        protected override void billNew()
        {
            //原单据保存判断
            string strDept = docDEPTGROUP.SelectedValue;
            string strDept2 = docDEPTID.Text;
            PubFunc.FormDataClear(FormDoc);

            docFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            docDEPTGROUP.SelectedValue = strDept;
            docDEPTID.Text = strDept2;
            tbxNUM.Text = "0";
            billLockDoc(false);
            tbxBARCODE.Enabled = true;
            btnAudit.Enabled = false;
            btnSave.Enabled = true;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            docDEPTGROUP.Enabled = true;

            //清空Grid行
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
        }

        private JObject GetJObject(Dictionary<string, object> dicRecord)
        {
            JObject defaultObj = new JObject();
            foreach (string key in dicRecord.Keys)
            {
                if (dicRecord[key] == null)
                {
                    defaultObj.Add(key, null);
                }
                else
                {
                    defaultObj.Add(key, dicRecord[key].ToString());
                }
            }

            decimal hl = 0, rs = 0, jg = 0;
            decimal.TryParse(dicRecord["BZHL"].ToString(), out hl);//包装含量
            decimal.TryParse(dicRecord["BZSL"].ToString(), out rs);//订货数
            decimal.TryParse(dicRecord["HSJJ"].ToString(), out jg);//价格

            defaultObj.Remove("DHSL");
            defaultObj.Remove("HSJE");
            defaultObj.Add("DHSL", rs * hl);
            //defaultObj.Add("HSJE", rs * jg);

            //处理金额格式
            string jingdu = Math.Round(rs * jg, 2).ToString("F2");
            defaultObj.Add("HSJE", jingdu);

            return defaultObj;
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
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
                decimal bzslTotal = 0, feeTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    bzslTotal += Convert.ToDecimal(dic["BZSL"]);
                    feeTotal += Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["BZSL"]);
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));

                GridGoods.SummaryData = summary;
            }
            else if (e.ColumnID == "PH")
            {
                String gdseq = Doc.GetGridInf(GridGoods, e.RowID, "GDSEQ");
                if (gdseq.Length < 1)
                {
                    Alert.Show("请先选择商品信息！", "异常信息", MessageBoxIcon.Question);
                    return;
                }
                String ph = Doc.GetGridInf(GridGoods, e.RowID, "PH");
                if (ph.Length < 1 && DbHelperOra.Exists(string.Format("SELECT 1 FROM DOC_GOODS WHERE GDSEQ = '{0}' AND ISLOT = '2'", gdseq)))
                {
                    Alert.Show("请填写商品[" + gdseq + "]批次信息！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }

                if (ph.ToString() == "\\")
                {
                    DataTable dtPH = Doc.GetGoodsPH_New(gdseq, docDEPTID.Text);
                    if (dtPH != null && dtPH.Rows.Count > 0)
                    {
                        GridLot.DataSource = dtPH;
                        GridLot.DataBind();
                        WindowLot.Hidden = false;
                    }
                    else
                    {
                        Alert.Show("商品[" + gdseq + "]已无库存,请检查！", MessageBoxIcon.Warning);
                    }
                }
            }
        }

        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;
        }

        protected override void billAddRow()
        {
            if (docFLAG.SelectedValue != "M")
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
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("非『新增』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            GridGoods.DeleteSelectedRows();

        }

        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            PubFunc.FormLock(FormDoc, true, "");
            tbxBARCODE.Enabled = false;
            //参数说明：cx-查询内容，bm-商品配置部门,su-供应商
            string url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTID.Text + "&goodsType=1";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
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

            string strSql = @"SELECT A.SEQNO,A.BILLNO,B.NAME FLAG, F_GETDEPTNAME(A.STR5)  DEPTIDNAME,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,
                                     A.SUBNUM,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHRNAME,A.SHRQ,A.MEMO,'['||A.PSSID||']'||F_GETSUPNAME(A.PSSID) SUPNAME
                                from DAT_XS_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' AND BILLTYPE='XSD' AND XSTYPE='3' ";
            string strSearch = "";


            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND TRIM(UPPER(A.BILLNO)) LIKE '%{0}%'", lstBILLNO.Text.Trim().ToUpper());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.STR5='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (!string.IsNullOrWhiteSpace(ddlFLag.SelectedValue))
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", ddlFLag.SelectedValue);
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.XSRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

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
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            if (dtDoc != null && dtDoc.Rows.Count > 0)
            {
                PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
            }
            else
            {
                Alert.Show(string.Format("未在系统中查询到单据【{0}】！", strBillno), "消息提示", MessageBoxIcon.Warning);
                return;
            }

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            decimal bzslTotal = 0, feeTotal = 0;
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno)).Tables[0];
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["BZSL"]);
                    feeTotal += Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZSL"]);
                }
            }
            Doc.GridRowAdd(GridGoods, dtBill);
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
            PubFunc.FormLock(FormDoc, true, "");
            TabStrip1.ActiveTabIndex = 1;
            //判断按钮状态
            if (docFLAG.SelectedValue == "M")
            {
                btnSave.Enabled = true;
                btnAudit.Enabled = true;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
            }
            else
            {
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnPrint.Enabled = true;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
            }
        }

        protected override void billSave()
        {
            #region 数据有效性验证
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
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
                    if (newDict[i]["ISLOT"].ToString() == "1" || newDict[i]["ISLOT"].ToString() == "2")
                    {
                        if (string.IsNullOrWhiteSpace(newDict[i]["PH"].ToString()))
                        {
                            //GridGoods.SelectedCell = new int[] { i, 8 };
                            string[] selectedCell = GridGoods.SelectedCell;
                            PageContext.RegisterStartupScript(String.Format("F('{0}').selectCell('{1}','{2}');", GridGoods.ClientID, selectedCell[0], "GDSPEC"));

                            Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】批号不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    if (newDict[i]["BZSL"].ToString() == "0" || string.IsNullOrWhiteSpace(newDict[i]["BZSL"].ToString()))
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】使用数填写不正确！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (decimal.Parse(newDict[i]["NUM5"].ToString()) < decimal.Parse(newDict[i]["BZSL"].ToString()))
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】使用数大于库存数，请重新输入！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    //当商品中含有贵重商品信息，即需要赋唯一码时
                    if (newDict[i]["ISGZ"].ToString() == "Y" && newDict[i]["ONECODE"].ToString() == "")
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】为贵重商品，请先进行扫描赋码！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    //填写商品追溯码时，使用数必须为小于等于1
                    //if (newDict[i]["ONECODE"].ToString().Length > 0 && newDict[i]["BZSL"].ToString() != "1")
                    //{
                    //    Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】为贵重商品,使用数量只能为1！");
                    //    return;
                    //}
                    goodsData.Add(newDict[i]);
                }
            }

            if (goodsData.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("商品信息不能为空", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //验证单据信息
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_XS_DOC where seqno = '" + docBILLNO.Text + "'") && docBILLNO.Enabled)
            {
                Alert.Show("您输入的单号已存在,请检查!");
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
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'N') FROM DAT_XS_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
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

            MyTable mtType = new MyTable("DAT_XS_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("XSTYPE", "3");
            mtType.ColRow["STR5"] = mtType.ColRow["DEPTGROUP"];

            mtType.ColRow.Remove("DEPTGROUP");
            mtType.ColRow.Remove("NUM");
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_XS_COM");
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("DELETE DAT_XS_DOC WHERE SEQNO='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("DELETE DAT_XS_COM WHERE SEQNO='" + docBILLNO.Text + "'", null));//删除单据明细
            cmdList.AddRange(mtType.InsertCommand());
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);


                if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["BZSL"].ToString()) || mtTypeMx.ColRow["BZSL"].ToString() == "0")
                {
                    Alert.Show("商品【" + mtTypeMx.ColRow["GDSEQ"] + " | " + mtTypeMx.ColRow["GDNAME"] + "】【使用数】为0或空，无法进行【使用信息管理】操作。", "消息提示", MessageBoxIcon.Warning);
                    return;
                }

                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString());
                mtTypeMx.ColRow["XSSL"] = mtTypeMx.ColRow["DHSL"];
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                cmdList.Add(mtTypeMx.Insert());
            }

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("学科组试剂领用信息保存成功！", "消息提示", MessageBoxIcon.Information);
                OperLog("试剂出库", "修改单据【" + docBILLNO.Text + "】");
                billOpen(docBILLNO.Text);
            }
        }

        protected override void billAudit()
        {
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("本条试剂申领信息已经审核确认，不能再次审核！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            object code = DbHelperOra.GetSingle("SELECT A.ONECODE FROM DAT_XS_COM A,DAT_BARCODE_SJ B WHERE A.ONECODE = B.GDBARCODE AND B.FLAG = 'Y' AND A.SEQNO = '" + docSEQNO.Text + "' AND ROWNUM = 1");
            if ((code ?? "").ToString().Length > 0)
            {
                Alert.Show("试剂条码【" + code.ToString() + "】已被废弃或使用，请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (BillOper(docSEQNO.Text, "AUDIT") == 1)
            {
                Alert.Show("学科组试剂领用信息审核成功！", "消息提示", MessageBoxIcon.Information);
                billOpen(docSEQNO.Text);
                OperLog("试剂出库", "审核单据【" + docSEQNO.Text + "】");
            }
        }

        /// <summary>
        /// FineUIPro.Grid控件数据加载
        /// </summary>
        /// <param name="row">要加载的行数据</param>
        /// <param name="firstRow">是否插入指定行</param>
        /// <param name="flag">数据来源：NEW-从数据库中获得，用于商品新增时；OLD-从销售单据明细中获得，用于修改或审核时</param>
        private void LoadGridRow(DataRow row, bool firstRow = true, string flag = "NEW")
        {
            PubFunc.GridRowAdd(GridGoods, row, firstRow);
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            dt.Columns.Remove(dt.Columns["BZHL"]);
            dt.Columns.Remove(dt.Columns["UNIT"]);
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                dt.Columns["UNIT_SELL_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNIT_SELL"].ColumnName = "UNIT";
                dt.Columns["BZHL_SELL"].ColumnName = "BZHL";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("NUM5", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                string msg = "";
                string msg1 = "";
                string msg2 = "";
                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "0";
                    row["DHSL"] = "0";
                    row["NUM5"] = "0";
                    row["HSJE"] = "0";
                    row["HWID"] = "";
                    DataTable Temp = DbHelperOra.Query(string.Format("SELECT A.HWID, A.KCSL,A.PH,A.YXQZ,A.RQ_SC,B.ISGZ,B.GDNAME FROM DAT_GOODSSTOCK A ,DOC_GOODS B WHERE A.DEPTID ='{0}' AND A.GDSEQ = '{1}'  AND A.GDSEQ = B.GDSEQ AND A.KCSL >0 AND ROWNUM = 1 ORDER BY A.PICINO ASC", docDEPTID.Text, row["GDSEQ"].ToString())).Tables[0];
                    if (Temp.Rows.Count > 0)
                    {
                        if (Temp.Rows[0]["ISGZ"].ToString() == "Y")
                        {
                            msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                            continue;
                        }

                        row["NUM5"] = Temp.Rows[0]["KCSL"];
                        row["PH"] = Temp.Rows[0]["PH"];
                        row["YXQZ"] = Temp.Rows[0]["YXQZ"];
                        row["RQ_SC"] = Temp.Rows[0]["RQ_SC"];
                        row["HWID"] = Temp.Rows[0]["HWID"];
                    }
                    else
                    {
                        msg1 += row["GDNAME"] + ",";
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                    {
                        msg2 += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                        continue;
                    }
                    //换算价格
                    row["HSJJ"] = Math.Round(Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZHL"]), 4);
                    LoadGridRow(row, false);
                }

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】属于高值商品，请在【高值商品使用】里进行录入！", msg);
                    Alert.Show(strNostock, "消息提示", MessageBoxIcon.Warning);
                }

                if (!string.IsNullOrWhiteSpace(msg1))
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】在部门『{1}』中没有库存,不能进行录入！", msg1, docDEPTID.Text);
                    Alert.Show(strNostock, "消息提示", MessageBoxIcon.Warning);
                }

                if (!string.IsNullOrWhiteSpace(msg2))
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】【含税进价】为空,不能进行【商品退货审批】操作。", msg2);
                    Alert.Show(strNostock, "消息提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("请选择添加商品！", "消息提示", MessageBoxIcon.Warning);
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
                    defaultObj["NUM5"] = row.Values[6].ToString();
                    defaultObj["PH"] = row.Values[3].ToString();
                    defaultObj["YXQZ"] = row.Values[4].ToString();
                    defaultObj["PZWH"] = row.Values[9].ToString();
                    defaultObj["RQ_SC"] = row.Values[5].ToString();
                    defaultObj["BZSL"] = tbxNumber.Text;
                    defaultObj["HWID"] = row.Values[10].ToString();
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
        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据");
                return;
            }

            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非新单不能删除!");
                return;
            }
            DbHelperOra.ExecuteSql("DELETE FROM DAT_XS_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("DELETE FROM DAT_XS_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
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
        protected void btnClose_Click(object sender, EventArgs e)
        {
            WindowLot.Hidden = true;
        }
        protected void tbxBARCODE_TextChanged(object sender, EventArgs e)
        {
            if (docDEPTGROUP.SelectedValue.Length > 0)
            {
                //长度不够
                if (tbxBARCODE.Text.Trim().Length < Doc.LENCODE())
                    return;
            }
            else
            {
                //科室赋值
                object obj = DbHelperOra.GetSingle("SELECT sjcode FROM SYS_DEPT WHERE CODE = '" + tbxBARCODE.Text.Trim() + "' AND TYPE = '5'");
                if ((obj ?? "").ToString().Length > 0)
                {
                    docDEPTGROUP.SelectedValue = tbxBARCODE.Text.Trim();
                    docDEPTID.Text = obj.ToString();
                }
                else
                {
                    Alert.Show("请首先选择科室或扫描科室工作牌！", "提示信息", MessageBoxIcon.Warning);
                }
                tbxBARCODE.Text = "";
                tbxBARCODE.Focus();
                return;
            }
            #region 扫描试剂条码
            if (tbxBARCODE.Text.Trim().Substring(0, 1).ToUpper() == "3")
            {
                List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
                for (int i = 0; i < newDict.Count; i++)
                {
                    string barcode_old = (newDict[i]["ONECODE"] ?? "").ToString();
                    if (barcode_old == tbxBARCODE.Text)
                    {
                        Alert.Show("扫描的试剂条码已存在！", "消息提示", MessageBoxIcon.Warning);
                        tbxBARCODE.Text = "";
                        tbxBARCODE.Focus();
                        return;
                    }
                }

                DataTable barcode = DbHelperOra.Query("SELECT A.*,B.HSJJ FROM DAT_BARCODE_SJ A,DOC_GOODS B WHERE A.GDSEQ=B.GDSEQ AND A.GDBARCODE = '" + tbxBARCODE.Text + "' AND A.FLAG IN ('N','R') AND B.FLAG IN('Y','T')").Tables[0];
                if (barcode != null && barcode.Rows.Count > 0)
                {
                    string code = barcode.Rows[0]["GDSEQ"].ToString();
                    string dept = docDEPTID.Text;

                    if (!string.IsNullOrWhiteSpace(code) && code.Trim().Length >= 2)
                    {
                        DataTable dt_goods = Doc.GetGoods_His(code, "", dept);

                        if (dt_goods != null && dt_goods.Rows.Count > 0)
                        {
                            dt_goods.Columns.Add("ONECODE", Type.GetType("System.String"));
                            dt_goods.Columns.Add("BZSL", Type.GetType("System.Double"));
                            dt_goods.Columns.Add("DHSL", Type.GetType("System.Double"));
                            dt_goods.Columns.Add("NUM5", Type.GetType("System.Double"));
                            dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                            DataRow dr_goods = dt_goods.Rows[0];
                            dr_goods["BZSL"] = barcode.Rows[0]["SL"];
                            dr_goods["DHSL"] = barcode.Rows[0]["SL"];
                            object kcsl = DbHelperOra.GetSingle("SELECT SUM(KCSL-locksl) FROM DAT_GOODSSTOCK WHERE DEPTID = '" + dept + "' AND GDSEQ = '" + code + "' AND PHID = '" + barcode.Rows[0]["PH"].ToString() + "'");
                            dr_goods["NUM5"] = kcsl ?? 0;
                            dr_goods["HSJE"] = Convert.ToDecimal(barcode.Rows[0]["SL"]) * Convert.ToDecimal(barcode.Rows[0]["HSJJ"]);
                            dr_goods["PH"] = barcode.Rows[0]["PH"];
                            dr_goods["PZWH"] = barcode.Rows[0]["PZWH"];
                            dr_goods["RQ_SC"] = barcode.Rows[0]["RQ_SC"];
                            dr_goods["YXQZ"] = barcode.Rows[0]["YXQZ"];
                            dr_goods["HWID"] = barcode.Rows[0]["HWID"];
                            dr_goods["ONECODE"] = tbxBARCODE.Text;
                            PubFunc.GridRowAdd(GridGoods, dr_goods, false);
                            tbxNUM.Text = (Convert.ToInt16(tbxNUM.Text) + 1).ToString();
                            tbxBARCODE.Text = "";
                            tbxBARCODE.Focus();
                        }
                        else
                        {
                            Alert.Show(string.Format("部门【{0}】尚未配置商品【{1}】！！！", docDEPTID.Text, code), MessageBoxIcon.Warning);
                        }
                    }
                }
                else
                {
                    Alert.Show("扫描条码不存在或已被出库使用，请检查！！!");
                    tbxBARCODE.Text = "";
                    tbxBARCODE.Focus();
                }
            }
            else
            {
                Alert.Show("条码异常，请检查后重新输入。", "操作提示", MessageBoxIcon.Warning);
                tbxBARCODE.Text = "";
                tbxBARCODE.Focus();
            }
            #endregion
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
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                FineUIPro.BoundField flagcol = GridList.FindColumn("FLAG") as FineUIPro.BoundField;
                if (flag == "新单")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color1";
                }

            }
        }

        protected void docDEPTGROUP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (docDEPTGROUP.SelectedValue.Length > 0)
            {
                object obj = DbHelperOra.GetSingle("SELECT sjcode FROM SYS_DEPT WHERE CODE = '" + docDEPTGROUP.SelectedValue + "' AND TYPE = '5'");
                docDEPTID.Text = (obj ?? "").ToString();
            }
            else
            {
                docDEPTID.Text = "";
            }
        }
    }
}
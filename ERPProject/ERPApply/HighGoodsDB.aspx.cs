﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPApply
{
    public partial class HighGoodsDB : BillBase
    {
        #region Load加载
        private string strDocSql = "SELECT * FROM DAT_DB_DOC WHERE SEQNO ='{0}' AND BILLTYPE = 'DBD'";
        private string strComSql = @"SELECT A.*,
                                       F_GETUNITNAME(A.UNIT) UNITNAME,
                                       F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                       F_GETUNITNAME(B.UNIT) UNITSMALLNAME
                                          FROM DAT_DB_COM A, DOC_GOODS B
                                         WHERE SEQNO = '{0}'
                                           AND A.GDSEQ = B.GDSEQ
                                         ORDER BY ROWNO";
        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docDEPTID, docXSRQ }; }
        }

        public HighGoodsDB()
        {
            BillType = "DBD";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //屏蔽不需要的操作按钮
                ButtonHidden(btnCopy, btnNext, btnBef, btnCancel, btnExport, btnAddRow, btnGoods);
                DataInit();
                billNew();
                if (Request.QueryString["oper"] != null)
                {
                    hdfOper.Text = Request.QueryString["oper"].ToString();

                    if (hdfOper.Text == "audit")
                    {
                        ButtonHidden(btnNew, btnDel, btnSave, btnAuditBatch, btnDelRow,btnFp);
                        TabStrip1.ActiveTabIndex = 0;
                        billSearch();

                    }
                    else if (hdfOper.Text == "input")
                    {
                        ButtonHidden(btnAudit, btnFp);
                    }
                   
                }
               
            }
        }
        private void DataInit()
        {
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet("DDL_USER", docSHR, docLRY, docSLR);
            DepartmentBind.BindDDL("DDL_SYS_DEPOTRANGE", UserAction.UserID, docDEPTOUT, ddlDEPTOUT, docDEPTID, lstDEPTID);
            PubFunc.DdlDataGet("DDL_BILL_STATUSDBD", docFLAG, lstFLAG);
            docDEPTOUT.Items.RemoveAt(0);
            docDEPTID.Items.RemoveAt(0);
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
                    highlightRowRed.Text += e.RowIndex.ToString() + ",";
                }
            }
        }
        #endregion
        #region 增删改
        private JObject GetJObject(Dictionary<string, object> dicRecord)
        {
            JObject defaultObj = new JObject();
            foreach (string key in dicRecord.Keys)
            {
                defaultObj.Add(key, (dicRecord[key] ?? "").ToString());
            }

            decimal hl = 0, rs = 0, jg = 0;
            decimal.TryParse(dicRecord["BZHL"].ToString(), out hl);//包装含量
            decimal.TryParse((dicRecord["BZSL"] ?? "0").ToString(), out rs);//订货数
            decimal.TryParse(dicRecord["HSJJ"].ToString(), out jg);//价格

            defaultObj.Remove("DHSL");
            defaultObj.Remove("HSJE");
            defaultObj.Add("DHSL", rs * hl);
            defaultObj.Add("HSJE", rs * jg);

            return defaultObj;
        }
        protected override void billNew()
        {
            //原单据保存判断
            string strDeptID = docDEPTID.SelectedValue;
            string strDeptOUT = docDEPTOUT.SelectedValue;
            PubFunc.FormDataClear(FormDoc);

            docFLAG.SelectedValue = "N";
            docLRY.SelectedValue = UserAction.UserID;
            docSLR.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDeptID;
            docDEPTOUT.SelectedValue = strDeptOUT;
            billLockDoc(false);
            docMEMO.Enabled = true;
            docDEPTOUT.Enabled = true;
            docSLR.Enabled = true;
            //清空Grid行
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            btnDel.Enabled = false;
            btnSave.Enabled = true;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrt.Enabled = false;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            trbBARCODE.Enabled = true;

            if (Request.QueryString["tp"] != null && Request.QueryString["tp"].ToString().Trim().Length > 0)
            {
                docDEPTOUT.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE=" + Request.QueryString["tp"].ToString()).ToString();
            }
            else
            {
                docDEPTOUT.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE='2'").ToString();
            }

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
            if (GridGoods.SelectedRowID == null)
            {
                Alert.Show("请选择数据行删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            GridGoods.DeleteSelectedRows();
        }
        protected override void billSave()
        {
            #region 数据有效性验证
            if ((",M,R,N").IndexOf(docFLAG.SelectedValue) < 0)
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
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'N') FROM DAT_DB_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
                if (!string.IsNullOrWhiteSpace(flg) && (",M,N,R").IndexOf(flg) < 0)
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
            MyTable mtType = new MyTable("DAT_DB_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);

            mtType.ColRow.Add("XSTYPE", "1");
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_DB_COM");
            decimal subNum = 0;//总金额
            string onecode = string.Empty;
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_DB_DOC where seqno='" + docBILLNO.Text + "' and flag = 'N'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_DB_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);

                //判断含税进价，为0时不能保存
                if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["HSJJ"].ToString()) || mtTypeMx.ColRow["HSJJ"].ToString() == "0")
                {
                    Alert.Show("商品【含税进价】为0或空，无法进行【库房出库管理】操作。");
                    return;
                }
              
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["ROWNO"] = i+1;
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                mtTypeMx.ColRow.Add("XSSL", mtTypeMx.ColRow["DHSL"].ToString());
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                onecode += mtTypeMx.ColRow["STR2"].ToString() + ",";
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            //object objCount = DbHelperOra.GetSingle(@"SELECT LISTAGG(ONECODE,',') WITHIN GROUP( ORDER BY ONECODE)
            //                                                                      FROM DAT_CK_EXT WHERE BILLNO <> '" + docBILLNO.Text + @"'
            //                                                                       AND ONECODE IN ('" + onecode.Trim(',').Replace(",", "','") + "')");
            //if (objCount != null && objCount.ToString().Length > 0)
            //{
            //    Alert.Show("高值码【" + objCount.ToString() + "】重复！！！", "异常提醒", MessageBoxIcon.Warning);
            //    return;
            //}
           // 写入追溯码表
            cmdList.Add(new CommandInfo("DELETE FROM DAT_DB_EXT WHERE BILLNO='" + docBILLNO.Text + "'", null));//删除单据明细
            cmdList.Add(new CommandInfo(String.Format(@"INSERT INTO DAT_DB_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,FLAG,PH,RQ_SC,YXQZ)
                        SELECT '{1}','{0}',ROWNUM,A.ONECODE,A.GDSEQ,A.GDNAME,A.BARCODE,A.UNIT,A.GDSPEC,A.DEPTCUR,A.BZHL,A.FLAG,A.PH,A.RQ_SC,A.YXQZ
                          FROM DAT_RK_EXT A,DAT_DB_COM B
                         WHERE A.GDSEQ = B.GDSEQ AND B.SEQNO = '{0}' AND (A.ONECODE = B.STR2 OR A.STR1 = B.STR2)", docBILLNO.Text, docDEPTID.SelectedValue), null));
            cmdList.Add(mtType.Insert());
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("商品出库信息保存成功！");
                billLockDoc(true);
                billOpen(docBILLNO.Text);
                hdfRowIndex.Text = "";
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
            DbHelperOra.ExecuteSql("Delete from DAT_DB_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_DB_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_DB_EXT t WHERE T.BILLNO ='" + docBILLNO.Text.Trim() + "'");
            Alert.Show("单据删除成功!");
            OperLog("商品调拨", "删除单据【" + docBILLNO.Text.Trim() + "】-高值扫码");
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
        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【使用日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("【开始日期】大于【结束日期】，请重新输入！",MessageBoxIcon.Warning);
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO,decode(A.FLAG,'N','新单','A','已提交','W','已出库','R','已驳回','Y','已收货','未定义') FLAGNAME,A.FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,A.SUBSUM,
                                     A.SUBNUM,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,A.SHRQ,A.MEMO,F_GETUSERNAME(A.SHR) SHR
                                from DAT_DB_DOC A 
                                WHERE  BILLTYPE='DBD' AND XSTYPE='1'  ";
            string strSearch = "";


            if (tgbBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND UPPER(TRIM(A.BILLNO))  LIKE '%{0}%'", tgbBILLNO.Text.Trim().ToUpper());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (ddlDEPTOUT.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTOUT='{0}'", ddlDEPTOUT.SelectedItem.Value);
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
            if (hdfOper.Text == "audit")
            {
                strSql += " AND A.FLAG IN('A','W') ";
            }
            else
            {
                strSql += " AND A.FLAG IN('N','A') ";
            }
            strSql += " ORDER BY A.BILLNO DESC";
            highlightRows.Text = "";
            highlightRowYellow.Text = "";
            highlightRowRed.Text = "";
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
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            decimal bzslTotal = 0, feeTotal = 0, dhslTotal = 0;
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno, docDEPTOUT.SelectedValue)).Tables[0];

            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["BZSL"]);
                    feeTotal += Convert.ToDecimal(row["HSJE"]);
                    dhslTotal += Convert.ToDecimal(row["DHSL"]);
                }
                PubFunc.GridRowAdd(GridGoods, dtBill);
            }
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            summary.Add("DHSL", dhslTotal.ToString());
            GridGoods.SummaryData = summary;
            PubFunc.FormLock(FormDoc, true, "");
           
            TabStrip1.ActiveTabIndex = 1;
            //增加按钮控制
            if (docFLAG.SelectedValue == "N" || docFLAG.SelectedValue == "R")
            {
                docMEMO.Enabled = true;
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnAudit.Enabled = true;
                btnCancel.Enabled = false;
                btnPrt.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
                trbBARCODE.Enabled = true;
            }
            else if (docFLAG.SelectedValue == "A")
            {
                docMEMO.Enabled = false;
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnPrt.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
            }
            else
            {
                docMEMO.Enabled = false;
                btnDel.Enabled = false;
                btnSave.Enabled = false;
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
            string url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTOUT.SelectedValue + "&DeptIn=" + docDEPTID.SelectedValue + "&su=";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
            hdfZP.Text = "";
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
            if (docFLAG.SelectedValue != "A")
            {
                Alert.Show("单据状态非新增，不能审核！", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            //验证科室是否盘点
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID IN('" + docDEPTOUT.SelectedValue + "','" + docDEPTID.SelectedValue + "') AND FLAG='N'"))
            {
                Alert.Show("库房正在盘点,请检查!");
                return;
            }
            //string strBillno = docSEQNO.Text;
            //if (BillOper(strBillno, "AUDIT") == 1)
            //{
            //    billLockDoc(true);
            //    Alert.Show("单据【" + strBillno + "】审核成功！");
            //    billOpen(strBillno);
            //    OperLog("直接出库", "审核单据【" + docSEQNO.Text + "】");
            //}
            StringBuilder sbSql = new StringBuilder();
//            sbSql.AppendFormat(@"DECLARE
//                        BEGIN
//                                DELETE FROM DAT_DB_EXT T WHERE T.BILLNO ='{0}';
//                                INSERT INTO DAT_DB_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,FLAG,PH,RQ_SC,YXQZ)
//                                          SELECT '{3}','{0}',ROWNUM,A.STR2,A.GDSEQ,A.GDNAME,A.BARCODE,A.UNIT,A.GDSPEC,'{3}',A.BZHL,'Y',A.PH,A.RQ_SC,A.YXQZ
//                                            FROM DAT_DB_COM A WHERE A.SEQNO = '{0}';
//                                IF SQL%ROWCOUNT = 0 THEN
//                                    RAISE_APPLICATION_ERROR(-20001, '修改单据[{0}]标志不成功！');
//                                END IF;
//                                STORE.P_BILLOPER('{0}', '{1}', '{2}', 'AUDIT');
//                        END; ", docSEQNO.Text, BillType, UserAction.UserID, docDEPTID.SelectedValue);
            sbSql.AppendFormat(@"DECLARE BEGIN STORE.P_BILLOPER('{0}','{1}','{2}','AUDIT'); END;", docSEQNO.Text, BillType, UserAction.UserID, docDEPTID.SelectedValue);

            try
            {
                DbHelperOra.ExecuteSql(sbSql.ToString());
                billLockDoc(true);
                Alert.Show("单据【" + docSEQNO.Text + "】审核成功！", "消息提示", MessageBoxIcon.Information);
                OperLog("商品调拨", "审核单据【" + docSEQNO.Text + "】-高值扫码出库");
                billOpen(docSEQNO.Text);
            }
            catch (Exception ex)
            {
                Alert.Show(ERPUtility.errorParse(ex.Message), "消息提示", MessageBoxIcon.Warning);
            }
        }
        protected void btnAuditBatch_Click(object sender, EventArgs e)
        {
            int[] rowIndex = GridList.SelectedRowIndexArray;
            string billno = docBILLNO.Text;
            if (billno.Length == 0)
            {
                Alert.Show("请选择要审核的库房调拨信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
           

            if (billno.Length > 0)
            {
                string StrSql = "UPDATE DAT_DB_DOC SET FLAG = 'A' where seqno = '" + billno.TrimEnd(',') + "'";
                DbHelperOra.ExecuteSql(StrSql);
                docFLAG.SelectedValue = DbHelperOra.GetSingle("SELECT FLAG FROM DAT_DB_DOC WHERE SEQNO='" + billno.TrimEnd(',') + "'").ToString();
                if (docFLAG.SelectedValue == "A")
                {
                    OracleParameter[] parameters = new OracleParameter[]
            {
                 new OracleParameter("BILLNO",OracleDbType.Varchar2),
                 new OracleParameter("USERID",OracleDbType.Varchar2),
            };
                    parameters[0].Value = docSEQNO.Text;
                    parameters[1].Value = UserAction.UserID;
                    DbHelperOra.RunProcedure("P_EXE_DBD", parameters);
                }
                else
                {
                    Alert.Show("单据状态不正确,不能进行库存分配!");
                    return;
                }
                Alert.Show("商品调拨申请提交成功！", "消息提示", MessageBoxIcon.Information);
                billSearch();
                OperLog("商品调拨", "提交单据【" + billno.TrimEnd(',') + "】-高值扫码");
            }
        }
        protected void btnFp_Click(object sender, EventArgs e)
        {
            FP_Action();
        }
        private void FP_Action()
        {
            //进行库存分配
            if (docFLAG.SelectedValue == "A")
            {
                OracleParameter[] parameters = new OracleParameter[]
            {
                 new OracleParameter("BILLNO",OracleDbType.Varchar2),
                 new OracleParameter("USERID",OracleDbType.Varchar2),
            };
                parameters[0].Value = docSEQNO.Text;
                parameters[1].Value = UserAction.UserID;
                DbHelperOra.RunProcedure("P_EXE_DBD", parameters);
                billOpen(docSEQNO.Text);
                Alert.Show("库存分配成功!");
            }
            else
            {
                Alert.Show("单据状态不正确,不能进行库存分配!");
                return;
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
                                       F_GETDEPTNAME(A.DEPTID) 申领部门,
                                       A.XSRQ 申领日期,
                                       F_GETDEPTNAME(A.DEPTOUT) 出库部门,
                                       F_GETUSERNAME(A.SLR) 申领人,
                                       F_GETUSERNAME(A.LRY) 录入人,
                                       A.LRRQ 录入日期,
                                       B.ROWNO 行号,
                                       ''''||B.GDSEQ 商品编码,
                                       B.GDNAME 商品名称,
                                       B.GDSPEC 商品规格,
                                       B.PZWH 注册证号,
                                       F_GETUNITNAME(B.UNIT) 单位,
                                       F_GETPRODUCERNAME(B.PRODUCER) 生产厂家,
                                       B.STR2 高值条码,
                                       B.BZHL 包装含量,
                                       B.BZSL 申领包装数,
                                       B.XSSL 申领数,B.HSJJ 价格,B.PH 批号,B.RQ_SC 生产日期,B.YXQZ 有效期至
                                  FROM DAT_CK_DOC A, DAT_CK_COM B
                                 WHERE A.SEQNO=B.SEQNO
                                   AND A.BILLTYPE = '" + BillType + @"'
                                   AND A.XSTYPE = 'G' ";
            string strSearch = "";


            if (tgbBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", tgbBILLNO.Text);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (ddlDEPTOUT.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTOUT='{0}'", ddlDEPTOUT.SelectedItem.Value);
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
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_DB_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND SEQNO = '{0}'", docSEQNO.Text)))
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
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_DB_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.GDSEQ,A.INSTIME DESC";
            }
            else
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_DB_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.INSTIME DESC";
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
            string sSQL = string.Format("DELETE FROM DAT_DB_EXT WHERE ONECODE = '{0}' AND BILLNO ='{1}'", onecode, docBILLNO.Text);
            DbHelperOra.ExecuteSql(sSQL);
            ScanSearch("");
            //int SelectedIndex = GridGoods.SelectedCell[0];
            //PageContext.RegisterStartupScript(GridGoods.GetDeleteIndexReference(SelectedIndex));
        }

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            GridList.SortDirection = e.SortDirection;
            GridList.SortField = e.SortField;
            highlightRows.Text = "";
            highlightRowYellow.Text = "";
            highlightRowRed.Text = "";
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
            if (string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
            {
                Alert.Show("请选择申领库房！", "提示信息", MessageBoxIcon.Warning);
                trbBARCODE.Text = "";
                trbBARCODE.Focus();
                return;
            }
            if (docDEPTID.SelectedValue == docDEPTOUT.SelectedValue)
            {
                Alert.Show("同一库房不需要调拨！", "提示信息", MessageBoxIcon.Warning);
                trbBARCODE.Text = "";
                trbBARCODE.Focus();
                return;
            }
            //扫码出库
            if (trbBARCODE.Text.Length < Doc.LENCODE()) return;
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            for (int i = 0; i < newDict.Count; i++)
            {
                if (newDict[i]["STR2"].ToString() == trbBARCODE.Text.Trim())
                {
                    Alert.Show("扫描条码已增加到单据中！", "提示信息", MessageBoxIcon.Warning);
                    trbBARCODE.Text = "";
                    trbBARCODE.Focus();
                    return;
                }
            }
            DataTable dtCode = DbHelperOra.Query(String.Format("SELECT DEPTCUR,DEPTID,GDSEQ FROM DAT_GZ_EXT WHERE UPPER(ONECODE) = UPPER('{0}') OR UPPER(STR1) = UPPER('{0}')", trbBARCODE.Text)).Tables[0];
            if (dtCode == null || dtCode.Rows.Count < 1)
            {
                Alert.Show("系统中不存在条码【" + trbBARCODE.Text.Trim() + "】！", "提示信息", MessageBoxIcon.Warning);
                trbBARCODE.Text = "";
                trbBARCODE.Focus();
                return;
            }
            string dept = dtCode.Rows[0]["DEPTCUR"].ToString();//Doc.ONECODE_GZ(trbBARCODE.Text.Trim(), "DEPTCUR");
            string keshi = dtCode.Rows[0]["DEPTID"].ToString();
            string gdseq = dtCode.Rows[0]["GDSEQ"].ToString();

            if (docDEPTOUT.SelectedValue.Length > 0)
            {
                if (docDEPTOUT.SelectedValue != dept)
                {
                    Alert.Show("条码【" + trbBARCODE.Text.Trim() + "】不属于库房【" + docDEPTOUT.SelectedText + "】", "提示信息", MessageBoxIcon.Warning);
                    trbBARCODE.Text = "";
                    trbBARCODE.Focus();
                    return;
                }
            }
            else
            {
                docDEPTOUT.SelectedValue = dept;
                if (docDEPTOUT.Text.Length < 1)
                {
                    Alert.Show("扫描条码的库房不存在或条码已被使用！");
                    docDEPTOUT.SelectedValue = "";
                    trbBARCODE.Text = "";
                    trbBARCODE.Focus();
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(keshi))
            {
                if (docDEPTID.SelectedValue != keshi)
                {
                    Alert.Show("条码【" + trbBARCODE.Text.Trim() + "】不属于库房【" + docDEPTID.SelectedText + "】！", "提示信息", MessageBoxIcon.Warning);
                    trbBARCODE.Text = "";
                    trbBARCODE.Focus();
                    return;
                }
            }
            else
            {
                if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DOC_GOODSCFG WHERE DEPTID='{0}' AND GDSEQ='{1}' AND ISCFG='Y'", docDEPTID.SelectedValue, gdseq)))
                {
                    Alert.Show("条码【" + trbBARCODE.Text.Trim() + "】不属于库房【" + docDEPTID.SelectedText + "】！", "提示信息", MessageBoxIcon.Warning);
                    trbBARCODE.Text = "";
                    trbBARCODE.Focus();
                    return;
                }
            }
            docDEPTOUT.Enabled = false;
            docDEPTID.Enabled = false;
            //增加商品
            //信息赋值
            DataTable dt = DbHelperOra.Query("SELECT A.* FROM DAT_GZ_EXT A WHERE (UPPER(A.ONECODE) = UPPER('" + trbBARCODE.Text.Trim() + "') OR UPPER(A.STR1) = UPPER('" + trbBARCODE.Text.Trim() + "')) AND FLAG IN('Y','R')").Tables[0];
            if (dt.Rows.Count < 1)
            {
                Alert.Show("扫描条码未入库或已被使用!", "提示信息", MessageBoxIcon.Warning);
                trbBARCODE.Text = "";
                trbBARCODE.Focus();
                return;
            }
            DataTable dt_goods = Doc.GetGoods_His(gdseq, "", dept);
            int index = 0;
            if (hdfRowIndex.Text.Trim().Length > 0)
            {
                index = int.Parse(hdfRowIndex.Text);
            }
            else
            {
                index = 1;
            }

            if (dt_goods != null && dt_goods.Rows.Count > 0)
            {
                dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt_goods.Columns.Add("STR2", Type.GetType("System.String"));
                dt_goods.Columns.Add("ROWNO", Type.GetType("System.Int32"));
                DataRow dr_goods = dt_goods.Rows[0];
                dr_goods["BZSL"] = "1";
                dr_goods["BZHL"] = "1";
                dr_goods["STR2"] = trbBARCODE.Text.Trim().ToUpper();
                dr_goods["HSJE"] = dr_goods["HSJJ"];
                dr_goods["PH"] = dt.Rows[0]["PH"];
                dr_goods["YXQZ"] = dt.Rows[0]["YXQZ"];
                dr_goods["RQ_SC"] = dt.Rows[0]["RQ_SC"];
                dr_goods["ROWNO"] = index;
                LoadGridRow(dr_goods, false);
                index++;
                hdfRowIndex.Text = index.ToString();
            }
            else
            {
                Alert.Show(string.Format("{0}尚未配置商品【{1}】！！！", docDEPTID.SelectedText, gdseq), MessageBoxIcon.Warning);
            }
            trbBARCODE.Text = "";
            trbBARCODE.Focus();
        }
    }
}
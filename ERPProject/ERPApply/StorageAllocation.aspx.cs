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
    public partial class StorageAllocation : BillBase
    {
        private string strDocSql = @"SELECT A.*,F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,F_GETDEPTNAME(A.DEPTID) DEPTNAME,F_GETUSERNAME(A.LRY) LRYNAME,F_GETUSERNAME(A.SPR) SPRNAME,F_GETUSERNAME(A.SHR) SHRNAME,F_GETUSERNAME(A.SLR) SLRNAME
                        ,decode(A.FLAG,'N','新单','A','已提交','W','已出库','R','已驳回','Y','已收货','未定义') FLAGNAME
                       FROM DAT_DB_DOC A WHERE 1=1 ";
        private string strComSql = @"SELECT B.SEQNO,B.ROWNO,B.GDSEQ,B.BARCODE,B.GDNAME,B.UNIT,B.GDSPEC,B.GDMODE,B.HWID,B.BZHL,B.BZSL,B.DHSL,B.XSSL,B.JXTAX,B.HSJJ,B.BHSJJ,B.HSJE,B.BHSJE,B.LSJ,B.LSJE,B.ISGZ,B.ISLOT,B.PHID,B.PH,B.PZWH,B.RQ_SC,B.YXQZ,B.PRODUCER,B.ZPBH,B.STR1,B.STR2,B.STR3,nvl((select sum(kcsl-a.locksl) from dat_goodsstock a where a.gdseq = c.gdseq and a.deptid = '{1}'),0) NUM1,B.NUM2,B.NUM3,B.MEMO,F_GETUNITNAME(B.UNIT) UNITNAME,F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,f_getunitname(C.UNIT) UNITSMALLNAME 
FROM DAT_DB_COM B ,DOC_GOODS C WHERE SEQNO ='{0}' AND B.GDSEQ=C.GDSEQ ORDER BY ROWNO ASC";
        public override Field[] LockControl
        {
            get { return new Field[] { docSEQNO, docDEPTID, docSLR, docDEPTOUT, docXSRQ, docMEMO }; }
        }

        public StorageAllocation()
        {
            BillType = "DBD";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = Request.RawUrl;

            if (!IsPostBack)
            {
                DataInit();
                billNew();
                //屏蔽不需要的操作按钮
                ButtonHidden(btnCopy, btnNext, btnBef, btnExport, btnCancel, btnPrint);
                btnDel.Enabled = false;
                //配置模块权限
            }
        }

        private void DataInit()
        {
            //天津医科大学总医院 申请库房按照登录用户权限来加载，出库库房加载全部 c 20151024
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, lstDEPTID, docDEPTID);
            PubFunc.DdlDataGet("DDL_SYS_DEPOT", lstDEPTOUT, docDEPTOUT, ddlDeptOrder1, ddlDeptOrder2);
            PubFunc.DdlDataGet("DDL_USER", lstSLR, lstLRY, docLRY, docSLR, docSHR);
            //PubFunc.DdlDataGet("DDL_BILL_STATUSDB", lstFLAG, docFLAG);
            PubFunc.DdlDataGet("DDL_BILL_STATUSDBD", lstFLAG, docFLAG);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }

        protected override void billNew()
        {
            string strDept = docDEPTID.SelectedValue;
            string strDeptOut = docDEPTOUT.SelectedValue;
            if (PubFunc.StrIsEmpty(strDept))
            {
                if (docDEPTID.Items.Count > 2)
                    strDept = docDEPTID.Items[1].Value;
            }
            if (PubFunc.StrIsEmpty(strDeptOut))
            {
                if (docDEPTOUT.Items.Count > 2)
                    strDeptOut = docDEPTOUT.Items[1].Value;
            }

            PubFunc.FormDataClear(FormDoc);

            docFLAG.SelectedValue = "N";
            docSLR.SelectedValue = UserAction.UserID;
            docLRY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDept;

            billLockDoc(false);

            btnSave.Enabled = true;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            btnDel.Enabled = false;
            btnAudit.Enabled = false;

            GridGoods.SummaryData = null;
            docSEQNO.Enabled = true;
            docBILLNO.Text = string.Empty;
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                FineUIPro.BoundField flagcol = GridList.FindColumn("FLAG") as FineUIPro.BoundField;
                if (flag == "N")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color1";
                }
                if (flag == "A")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color2";
                }
                if (flag == "R")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color3";
                }
            }
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            string[] strCell = GridGoods.SelectedCell;
            int selectedRowIndex = GridGoods.SelectedRowIndex;
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0) return;
            if (e.ColumnID == "BZSL" || e.ColumnID == "HSJJ")
            {
                if (!PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZHL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZSL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "HSJJ")))
                    if (!PubFunc.isNumeric(newDict[e.RowIndex]["BZHL"].ToString()) || !PubFunc.isNumeric(newDict[e.RowIndex]["BZSL"].ToString()) || !PubFunc.isNumeric(newDict[e.RowIndex]["HSJJ"].ToString()))
                    {
                        Alert.Show("商品信息异常，请详细检查商品信息：包装含量或价格！", "异常信息", MessageBoxIcon.Warning);
                        return;
                    }
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
                //if (Convert.ToDecimal(Doc.GetGridInf(GridGoods, e.RowID, "BZSL")) > Convert.ToDecimal(Doc.GetGridInf(GridGoods, e.RowID, "NUM1")))
                //{

                //    defaultObj["BZSL"] = "0";
                //    PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                //    Alert.Show("申请数不能大于库存数量", "异常信息", MessageBoxIcon.Warning);
                //    return;
                //}
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
                    if (dic["BZSL"].ToString().Length > 0 && dic["HSJJ"].ToString().Length > 0)
                    {
                        bzslTotal += Convert.ToDecimal(dic["BZSL"]);
                        feeTotal += Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["BZSL"]) * Convert.ToDecimal(dic["BZHL"]);
                    }
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));
                GridGoods.SummaryData = summary;
            }
        }

        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected override void billAddRow()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非『新增』单据不能增行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;

            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0)
            {
                if (Doc.DbGetSysPara("ISAUTOAPPLY") == "Y")
                {
                    string sql = @"SELECT A.*, A.ZGKC - A.KCSL DHSL, (A.ZGKC - A.KCSL) / A.BZHL BZSL,
                                      ((A.ZGKC - A.KCSL) / A.BZHL) * A.HSJJ HSJE,A.KCSL NUM1
                                FROM (SELECT G.GDSEQ,
                                            G.BARCODE,
                                            G.GDNAME,
                                            G.GDSPEC,
                                            G.UNIT,
                                            F_GETUNITNAME(G.UNIT) UNITNAME,
                                            G.BZHL,
                                            G.JXTAX,
                                            G.HSJJ,
                                            G.ZPBH,
                                            G.PRODUCER,
                                            F_GETPRODUCERNAME(G.PRODUCER) PRODUCERNAME,
                                            PZ.HJCODE1,
                                            '自动生成' MEMO,
                                            G.ISLOT,
                                            G.ISGZ,
                                            PZ.ZDKC,
                                            PZ.ZGKC,
                                            (SELECT NVL(SUM(KCSL), 0)
                                                FROM DAT_GOODSSTOCK
                                                WHERE GDSEQ = G.GDSEQ) KCSL
                                        FROM DOC_GOODSCFG PZ, DOC_GOODS G
                                        WHERE PZ.GDSEQ = G.GDSEQ
                                        AND PZ.ZDKC > 0
                                        AND PZ.ZGKC > 0
                                        AND PZ.DEPTID = '{0}') A
                                WHERE KCSL < ZDKC";
                    DataTable dt = DbHelperOra.Query(string.Format(sql, docDEPTID.SelectedValue)).Tables[0];
                    decimal bzslTotal = 0, feeTotal = 0;
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            bzslTotal += Convert.ToDecimal(row["BZSL"]);
                            feeTotal += Convert.ToDecimal(row["BZSL"]) * Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZHL"]);
                            LoadGridRow(row, false, "OLD");
                        }
                    }
                    else
                    {
                        PubFunc.GridRowAdd(GridGoods, "INIT");
                    }
                    //增加合计
                    JObject summary = new JObject();
                    summary.Add("GDNAME", "本页合计");
                    summary.Add("BZSL", bzslTotal.ToString());
                    summary.Add("HSJE", feeTotal.ToString("F2"));
                    GridGoods.SummaryData = summary;
                }
                else
                {
                    PubFunc.GridRowAdd(GridGoods, "INIT");
                }
            }
            else
            {
                PubFunc.GridRowAdd(GridGoods, "INIT");
            }
            PubFunc.FormLock(FormDoc, true, "");
        }

        protected override void billDelRow()
        {
            if (docFLAG.SelectedValue != "N" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非『新增』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "N", BillType) && !Doc.getFlag(strBillno, "R", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridGoods.SelectedRowID == null)
            {
                Alert.Show("当前未选中单元行，无法进行操作!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //PageContext.RegisterStartupScript(GridGoods.GetDeleteRowReference(GridGoods.SelectedRowID));
            GridGoods.DeleteSelectedRows();
            PubFunc.FormLock(FormDoc, true, "");
        }

        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            string url = "~/ERPQuery/GoodsWindow_New.aspx?DeptIn=" + docDEPTID.SelectedValue + "&Deptout=" + docDEPTOUT.SelectedValue + "&GoodsState=YTSE";
            PubFunc.FormLock(FormDoc, true, "");
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "调拨商品信息查询"));
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
            string strSearch = "";
            strSearch += " AND A.DEPTID in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '" + UserAction.UserID + "') = 'Y' ) ";
            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstSLR.SelectedItem != null && lstSLR.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.SLR='{0}'", lstSLR.SelectedItem.Value);
            }
            if (lstLRY.SelectedItem != null && lstLRY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.LRY='{0}'", lstLRY.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstDEPTOUT.SelectedItem != null && lstDEPTOUT.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND DEPTOUT='{0}'", lstDEPTOUT.SelectedItem.Value);
            }
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            strDocSql = strDocSql + strSearch + " ORDER BY decode(A.FLAG,'N','1','A','2','3'), LRRQ DESC";

            GridList.DataSource = DbHelperOra.Query(strDocSql).Tables[0];
            GridList.DataBind();
        }

        protected override void billAudit()
        {
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "N", BillType))
            {
                //Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                Alert.Show("请先保存单据后再提交！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue == "N" && docSEQNO.Text.Length > 0)
            {
                billLockDoc(true);
                //验证单据准确性
                if (DbHelperOra.Exists("select 1 from dat_db_com where seqno = '" + docSEQNO.Text + "' and BZSL <= 0"))
                {
                    Alert.Show("调拨申请单据数量全部为0,不允许提交!", "提示信息", MessageBoxIcon.Warning);
                    return;
                }
                string StrSql = "UPDATE dat_db_doc set flag = 'A' where seqno = '" + docSEQNO.Text + "'";
                DbHelperOra.ExecuteSql(StrSql);
                Alert.Show("调拨申请单【" + docSEQNO.Text + "】提交成功！");
                billOpen(docSEQNO.Text);
                btnGoods.Enabled = false;
                btnDelRow.Enabled = false;
                btnDel.Enabled = false;
                btnAudit.Enabled = false;
                OperLog("商品调拨", "提交单据【" + docSEQNO.Text + "】");
            }
            else
            {
                Alert.Show("调拨申请单【" + docSEQNO.Text + "】状态不正确", "提示信息", MessageBoxIcon.Warning);
            }
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {

            string strBillno = GridList.Rows[e.RowIndex].Values[1].ToString();
            billOpen(strBillno);
            if (DbHelperOra.Exists("select 1 from DAT_DD_DOC where SEQNO = '" + strBillno + "' AND FLAG='N'"))
            { //双击打开单据信息时，如果单据状态在数据库中依然是新单状态，到货日期允许修改
                docMEMO.Enabled = true;
            }
        }

        protected override void billOpen(string strBillno)
        {
            string StrSql = strDocSql + " AND A.SEQNO = '{0}'";
            DataTable dtDoc = DbHelperOra.Query(string.Format(StrSql, strBillno)).Tables[0];
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            string deptout = DbHelperOra.GetSingle("select deptout from DAT_DB_DOC t where seqno = '" + strBillno + "'").ToString();
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno, deptout)).Tables[0];
            decimal bzslTotal = 0, feeTotal = 0;
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["BZSL"]);
                    feeTotal += Convert.ToDecimal(row["BZSL"]) * Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZHL"]);
                    //LoadGridRow(row, false, "OLD");
                }
                /*
                *  修 改 人 ：袁鹏    修改时间：2015-03-20
                *  信息说明：这种加载方法比LoadGridRow(row, false, "OLD")更高效
                *  研发组织：威高讯通信息科技有限公司
                */
                Doc.GridRowAdd(GridGoods, dtBill);
            }
            //增加合计
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
            PubFunc.FormLock(FormDoc, true, "");
            GridGoods.AllowCellEditing = false;

            if (docFLAG.SelectedValue == "N" || docFLAG.SelectedValue == "R")
            {
                btnSave.Enabled = true;
                btnDel.Enabled = true;
                btnAudit.Enabled = true;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
            }
            else
            {
                btnDel.Enabled = false;
                btnAudit.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                btnSave.Enabled = false;
            }
            TabStrip1.ActiveTabIndex = 1;
        }

        protected override void billSave()
        {
            if (DataSave())
            {
                Alert.Show("商品信息保存成功！");
                billOpen(docBILLNO.Text);
                //btnAudit.Enabled = true;
                //btnDel.Enabled = true;
                OperLog("商品调拨", "修改单据【" + docBILLNO.Text + "】");
            }
        }

        private bool DataSave()
        {
            #region 数据有效性验证
            if ((",N,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "N", BillType) && !Doc.getFlag(strBillno, "R", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return false;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {

                    if ((newDict[i]["BZSL"] ?? "").ToString() == "" || (newDict[i]["BZSL"] ?? "").ToString() == "0")
                    {
                        //Alert.Show("请填写商品[" + newDict[i]["GDSEQ"] + "]的申请数量！", "消息提示", MessageBoxIcon.Warning);
                        //return false;
                        continue;
                    }
                    //if (decimal.Parse(newDict[i]["BZSL"].ToString()) > decimal.Parse(newDict[i]["NUM1"].ToString()))
                    //{
                    //    Alert.Show("商品【" + (newDict[i]["GDSEQ"].ToString()) + "】申请数不能大于库存数量", "异常信息", MessageBoxIcon.Warning);
                    //    return false;
                    //}
                    if (string.IsNullOrWhiteSpace(newDict[i]["BZHL"].ToString()) || string.IsNullOrWhiteSpace(newDict[i]["UNIT"].ToString()))
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]包装单位信息错误，请联系管理员维护！", "消息提示", MessageBoxIcon.Warning);
                        return false;
                    }
                    goodsData.Add(newDict[i]);
                }
            }
            if (goodsData.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("商品信息不能为空", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            #endregion
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                docSEQNO.Text = BillSeqGet();
                docBILLNO.Text = docSEQNO.Text;
                docBILLNO.Enabled = false;
            }
            else
            {
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'N') FROM DAT_DB_DOC WHERE SEQNO='{0}'", docSEQNO.Text));
                if (!PubFunc.StrIsEmpty(flg) && (",N,R").IndexOf(flg) < 0)
                {
                    Alert.Show("您输入的单据号存在重复信息，请重新输入或置空！", "消息提示", MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    docBILLNO.Text = docSEQNO.Text;
                    docSEQNO.Enabled = false;
                }
            }
            if (string.IsNullOrWhiteSpace(docDEPTOUT.SelectedValue))
            {
                Alert.Show("请选择出库库房！", "提示", MessageBoxIcon.Information);
                return false;
            }
            MyTable mtType = new MyTable("DAT_DB_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow["FLAG"] = "N";//所有单据在保存时单据状态一律为新增N
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("XSTYPE", "1");
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_DB_COM");
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_DB_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_DB_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            decimal subNum = 0;//总金额
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);
                //判断含税进价，为0时不能保存
                string isJiFei = string.Format("select 1 from DOC_GOODS t where iscf = 'N' and gdseq = '{0}'", mtTypeMx.ColRow["GDSEQ"].ToString());
                if (DbHelperOra.Exists(isJiFei))
                {
                    if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["HSJJ"].ToString()) || mtTypeMx.ColRow["HSJJ"].ToString() == "0")
                    {
                        Alert.Show("商品【含税进价】为0或空，无法进行【补货管理】操作。");
                        return false;
                    }
                }
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                //mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["DHSL"].ToString()) || mtTypeMx.ColRow["DHSL"].ToString() == "0")
                {
                    mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                }
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString());
                mtTypeMx.ColRow.Add("XSSL", 0);
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                mtTypeMx.ColRow["PH"] = "";
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            cmdList.AddRange(mtType.InsertCommand());
            string flag = DbHelperOra.GetSingle("SELECT VALUE FROM SYS_PARA WHERE CODE='ISAUTOAUDIT'").ToString();
            if (flag == "Y")
            {
                //当系统设置为自动审批时，商品申领信息在保存之后即进行审核操作
                OracleParameter[] parameters = {
                                               new OracleParameter("VTASKID", OracleDbType.Varchar2,20),
                                               new OracleParameter("VPARA", OracleDbType.Varchar2,800) };
                parameters[0].Value = BillType;
                parameters[1].Value = "'" + docBILLNO.Text + "','" + BillType + "','" + UserAction.UserID + "','DECLARE'";
                cmdList.Add(new CommandInfo("P_EXECTASK", parameters, CommandType.StoredProcedure));
            }
            return DbHelperOra.ExecuteSqlTran(cmdList);
        }

        /// <summary>
        /// FineUIPro.Grid控件数据加载
        /// </summary>
        /// <param name="row">要加载的行数据</param>
        /// <param name="firstRow">是否插入指定行</param>
        /// <param name="flag">数据来源：NEW-从数据库中获得，用于商品新增时；OLD-从销售单据明细中获得，用于修改或审核时</param>
        private void LoadGridRow(DataRow row, bool firstRow = true, string flag = "NEW")
        {
            //增加库存信息
            row["NUM1"] = DbHelperOra.GetSingle("SELECT sum(kcsl-locksl) FROM dat_goodsstock WHERE GDSEQ = '" + row["GDSEQ"] + "' AND DEPTID ='" + docDEPTOUT.SelectedValue + "'") ?? "0";
            if (flag == "NEW")
            {
                if (!string.IsNullOrWhiteSpace(row["UNIT_SELL"].ToString()))
                {
                    if (row["UNIT_SELL"].ToString() == "D")//出库单位为大包装时
                    {
                        if (!string.IsNullOrWhiteSpace(row["NUM_DABZ"].ToString()) && row["NUM_DABZ"].ToString() != "0")
                        {
                            row["UNIT"] = row["UNIT_DABZ"];
                            row["UNITNAME"] = row["UNIT_DABZ_NAME"];
                            row["BZHL"] = row["NUM_DABZ"];
                            int price = 0, number = 0;
                            int.TryParse(row["HSJJ"].ToString(), out price);
                            int.TryParse(row["NUM_DABZ"].ToString(), out number);
                            row["HSJJ"] = price * number;
                            //处理库存信息
                            if (number == 0)
                            { Alert.Show("商品" + row["SEQNO"] + "大包装数量未维护,请检查!"); return; }
                            row["NUM1"] = Convert.ToInt16(DbHelperOra.GetSingle("SELECT NVL(SUM(KCSL),0) FROM dat_goodsstock WHERE GDSEQ = '" + row["GDSEQ"] + "' AND DEPTID ='" + docDEPTOUT.SelectedValue + "'")) / number;
                        }
                    }
                    else if (row["UNIT_SELL"].ToString() == "Z")//出库单位为中包装时
                    {
                        if (!string.IsNullOrWhiteSpace(row["NUM_ZHONGBZ"].ToString()) && row["NUM_ZHONGBZ"].ToString() != "0")
                        {
                            row["UNIT"] = row["UNIT_ZHONGBZ"];
                            row["UNITNAME"] = row["UNIT_ZHONGBZ_NAME"];
                            row["BZHL"] = row["NUM_ZHONGBZ"];
                            int price = 0, number = 0;
                            int.TryParse(row["HSJJ"].ToString(), out price);
                            int.TryParse(row["NUM_ZHONGBZ"].ToString(), out number);
                            row["HSJJ"] = price * number;
                            //处理库存信息
                            if (number == 0)
                            { Alert.Show("商品" + row["SEQNO"] + "中包装数量未维护,请检查!"); return; }
                            row["NUM1"] = Convert.ToInt16(DbHelperOra.GetSingle("SELECT NVL(SUM(KCSL),0) FROM dat_goodsstock WHERE GDSEQ = '" + row["GDSEQ"] + "' AND DEPTID ='" + docDEPTOUT.SelectedValue + "'")) / number;
                        }
                    }
                }
            }

            PubFunc.GridRowAdd(GridGoods, row, firstRow);
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            //dt.Columns.Remove(dt.Columns["BZHL"]);
            if (dt != null && dt.Rows.Count > 0)
            {
                //dt.Columns["PIZNO"].ColumnName = "PZWH";
                //dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                //dt.Columns["BZHL_SELL"].ColumnName = "BZHL";
                //dt.Columns["UNIT_SELL_NAME"].ColumnName = "UNITNAME";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt.Columns.Add("NUM1", Type.GetType("System.Int32"));
                string someDjbh = string.Empty;
                bool getDjbh = false;

                foreach (DataRow row in dt.Rows)
                {
                    if (row["ISGZ"].ToString().Equals("Y"))
                    { 
                        row["PH"]=DbHelperOra.GetSingle(string.Format(" SELECT drc.ph FROM dat_rk_doc drd,dat_rk_com drc WHERE drc.seqno=drd.billno(+) AND drc.gdseq='{0}' AND drd.flag='Y' AND Rownum=1  ORDER BY shrq desc",row["GDSEQ"].ToString()));
                    }
                    //判断单据编号是已经存在，如果存在提醒不让添加 liuz 20150512
                    string strDjbh = row["GDSEQ"].ToString();
                    if (IsHaveSomeDjbh(strDjbh))
                    {
                        someDjbh += strDjbh + ";";
                        getDjbh = true;
                    }
                    else
                    {
                        row["BZSL"] = "0";
                        row["DHSL"] = "0";
                        row["HSJE"] = "0";
                        LoadGridRow(row, false);
                        docMEMO.Enabled = true;
                    }
                }
                if (getDjbh)
                {
                    Alert.Show("已经存在重复的单据【" + someDjbh + "】", "消息提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("系统传值错误！！！", "消息提示", MessageBoxIcon.Warning);
            }
        }
        private bool IsHaveSomeDjbh(string strDjbh)
        {
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            int rowCount = newDict.Where(a => a["GDSEQ"].ToString() == strDjbh).Count();
            if (rowCount > 0)
                return true;
            else
                return false;
        }

        protected void trbEditorGDSEQ_TriggerClick(object sender, EventArgs e)
        {
            string code = labGDSEQ.Text;
            string dept = docDEPTID.SelectedValue;

            if (!string.IsNullOrWhiteSpace(code) && code.Trim().Length >= 2)
            {
                string sql = @"select  SP.*,F_GETUNITNAME(SP.UNIT) UNITNAME,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME, 
                                   F_GETUNITNAME(UNIT_ORDER) UNIT_ORDER_NAME,F_GETUNITNAME(UNIT_SELL) UNIT_SELL_NAME,F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,
                                   F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,PZ.HJCODE1 HWID,PIZNO PZWH
                             from  DOC_GOODS SP,DOC_GOODSCFG PZ WHERE ISDELETE='N' and sp.flag='Y' AND SP.GDSEQ=PZ.GDSEQ AND SP.GDSEQ='{0}' AND PZ.DEPTID='{1}' AND PZ.ISCFG='1'
                            and EXISTS(SELECT 1 FROM dat_goodsstock A,DOC_GOODSCFG B WHERE A.GDSEQ = B.GDSEQ AND A.DEPTID = B.DEPTID AND B.GDSEQ = SP.GDSEQ AND A.KCSL >0 AND B.DEPTID = '{2}') ";
                DataTable dt_goods = DbHelperOra.Query(string.Format(sql, code.ToUpper(), docDEPTID.SelectedValue, docDEPTOUT.SelectedValue)).Tables[0];

                if (dt_goods != null && dt_goods.Rows.Count > 0)
                {
                    dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("DHSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                    DataRow dr_goods = dt_goods.Rows[0];
                    dr_goods["BZSL"] = "0";
                    dr_goods["DHSL"] = "0";
                    dr_goods["HSJE"] = "0";
                    LoadGridRow(dr_goods);
                }
                else
                {
                    Alert.Show(string.Format("{0}尚未配置商品【{1}】！！！", docDEPTID.SelectedText, code), MessageBoxIcon.Warning);
                    PubFunc.GridRowAdd(GridGoods, "CLEAR");
                }
            }
        }

        //protected void btnClosePostBack_Click(object sender, EventArgs e)
        //{
        //    bool firstRow = true;
        //    foreach (GridRow row in GridLot.Rows)
        //    {
        //        int rowIndex = row.RowIndex;
        //        System.Web.UI.WebControls.TextBox tbxNumber = (System.Web.UI.WebControls.TextBox)GridLot.Rows[rowIndex].FindControl("tbxNumber");
        //        if (!string.IsNullOrWhiteSpace(tbxNumber.Text) && tbxNumber.Text != "0")
        //        {
        //            if (!PubFunc.isNumeric(tbxNumber.Text))
        //            {
        //                Alert.Show("请输入正确的数字信息!", "操作提示", MessageBoxIcon.Warning);
        //                return;
        //            }
        //            string[] strCell = GridGoods.SelectedCell;
        //            JObject defaultObj = Doc.GetJObject(GridGoods, strCell[0]);
        //            defaultObj["PH"] = row.Values[1].ToString();
        //            defaultObj["YXQZ"] = row.Values[2].ToString();
        //            defaultObj["PZWH"] = row.Values[4].ToString();
        //            defaultObj["RQ_SC"] = row.Values[3].ToString();
        //            defaultObj["BZSL"] = tbxNumber.Text.ToString();
        //            if (firstRow)
        //            {
        //                firstRow = false;
        //                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(strCell[0], defaultObj));
        //            }
        //            else
        //            {
        //                PageContext.RegisterStartupScript(GridGoods.GetAddNewRecordReference(defaultObj));
        //            }
        //        }
        //    }
        //    WindowLot.Hidden = true;
        //}

        protected override void billCancel()
        {
            if (docBILLNO.Text.Length < 1)
            {
                Alert.Show("请选择需要驳回的单据!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue.ToString() != "N")
            {
                Alert.Show("非『新增』不能驳回！", "消息提示", MessageBoxIcon.Warning);
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

        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            string strMemo = docMEMO.Text + "；驳回原因：" + ddlReject.SelectedText;
            if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
            {
                strMemo += "；详细说明：" + txaMemo.Text;
            }
            if (DbHelperOra.ExecuteSql(string.Format("update DAT_DB_DOC set flag='R',memo='{0}' where seqno='{1}' and flag='N'", strMemo, docBILLNO.Text)) == 1)
            {
                WindowReject.Hidden = true;
                OperLog("商品调拨", "驳回单据【" + docBILLNO.Text + "】");
                billOpen(docBILLNO.Text);
            }

        }
        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue != "R" && docFLAG.SelectedValue != "N")
            {
                Alert.Show("非『新增』单据不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "N", BillType) && !Doc.getFlag(strBillno, "R", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            List<string> listSql = new List<string>();
            listSql.Add("Delete from DAT_DB_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            listSql.Add("Delete from DAT_DB_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            if (DbHelperOra.ExecuteSqlTran(listSql))
            {
                Alert.Show("单据删除成功!", "消息提示", MessageBoxIcon.Information);
                billNew();
                billSearch();
                OperLog("商品调拨", "删除单据【" + docBILLNO.Text + "】");
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
                Alert.Show("请选择要审核的库房调拨信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string billno = "";
            foreach (int index in rowIndex)
            {
                if (GridList.Rows[index].Values[3].ToString() == "N")
                    billno += "【" + GridList.Rows[index].Values[2].ToString() + "】,";
            }

            if (billno.Length > 0)
            {
                string StrSql = "UPDATE DAT_DB_DOC SET FLAG = 'A' where seqno in (" + billno.TrimEnd(',') + ")";
                DbHelperOra.ExecuteSql(StrSql);
                Alert.Show("商品调拨申请批量提交成功！", "消息提示", MessageBoxIcon.Information);
                billSearch();
                OperLog("商品调拨", "审核单据" + billno + "！");
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
        protected void rblTYPE_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rblTYPE.SelectedValue == "XS")
            {
                dbpOrder1.Enabled = true;
                dbpOrder2.Enabled = true;
                memo.Text = "调拨量 =（备货天数+订货周期)*日均用量 - 库房库存";
            }
            else
            {
                dbpOrder1.Enabled = false;
                dbpOrder2.Enabled = false;
                memo.Text = "调拨量 = 最高库存 -库房库存";
            }
        }
        protected void btn_Auto_Click(object sender, EventArgs e)
        {
            //自动生成调拨单
            WinAuto.Hidden = false;
            dbpOrder1.SelectedDate = DateTime.Now.AddMonths(-1);
            dbpOrder2.SelectedDate = DateTime.Now;
            ddlDeptOrder1.SelectedValue = docDEPTID.SelectedValue;
            ddlDeptOrder2.SelectedValue = UserAction.UserDept;
        }
        protected void btn_Sure_Click(object sender, EventArgs e)
        {
            if (ddlDeptOrder1.SelectedValue == "" || ddlDeptOrder2.SelectedValue == "")
            {
                Alert.Show("【调出部门】或【调入部门】未维护！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (ddlDeptOrder1.SelectedValue == ddlDeptOrder2.SelectedValue)
            {
                Alert.Show("【调出部门】与【调入部门】不能相同！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            OracleParameter[] parameters ={
                                            new OracleParameter("VI_DEPTOUT" ,OracleDbType.Varchar2,20),
                                            new OracleParameter("VI_DEPTIN" ,OracleDbType.Varchar2,20),
                                            new OracleParameter("VI_TYPE" ,OracleDbType.Varchar2,20),
                                            new OracleParameter("VI_BEG" ,OracleDbType.Varchar2,10),
                                            new OracleParameter("VI_END" ,OracleDbType.Varchar2,10),
                                            new OracleParameter("VI_USER" ,OracleDbType.Varchar2,20),
                                            new OracleParameter("VO_BILLNUM",OracleDbType.Varchar2,20)
                                           };
            parameters[0].Value = ddlDeptOrder1.SelectedValue;
            parameters[1].Value = ddlDeptOrder2.SelectedValue;
            parameters[2].Value = rblTYPE.SelectedValue;
            parameters[3].Value = dbpOrder1.Text;
            parameters[4].Value = dbpOrder2.Text;
            parameters[5].Value = UserAction.UserID;

            parameters[0].Direction = ParameterDirection.Input;
            parameters[1].Direction = ParameterDirection.Input;
            parameters[2].Direction = ParameterDirection.Input;
            parameters[3].Direction = ParameterDirection.Input;
            parameters[4].Direction = ParameterDirection.Input;
            parameters[5].Direction = ParameterDirection.Input;
            parameters[6].Direction = ParameterDirection.Output;

            try
            {
                if (DbHelperOra.RunProcedure("STOREDS.P_DB_AUTO", parameters) > 0)
                {
                    Alert.Show("自动调拨单【" + parameters[6].Value + "】生成成功!", "消息提示", MessageBoxIcon.Information);
                }
                else
                {

                    Alert.Show("没有需要生成调拨数据!", "消息提示", MessageBoxIcon.Information);
                }

                WinAuto.Hidden = true;
            }
            catch (Exception err)
            {
                Alert.Show(err.Message);
            }
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            switch (e.EventArgument)
            {
                case "GoodsAdd": Window2_Close(null, null); break;

            }
        }
        protected void Window2_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            dt.Columns.Remove(dt.Columns["BZHL"]);
            dt.Columns.Remove(dt.Columns["UNIT"]);
            string msg = "";

            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                dt.Columns["UNIT_SELL_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNIT_SELL"].ColumnName = "UNIT";
                dt.Columns["BZHL_SELL"].ColumnName = "BZHL";
                dt.Columns["CKKCSL"].ColumnName = "NUM1";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt.Columns.Add("BZSL",Type.GetType("System.String"));
                string someDjbh = string.Empty;
                bool getDjbh = false;
                foreach (DataRow row in dt.Rows)
                {

                    row["MEMO"] = row["ISZS"];
                    row["DHSL"] = "0";
                    row["HSJE"] = "0";
                    row["BZSL"] = "0";
                    //row["HSJJ"].ToString();
                    if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                    {
                        msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                        Alert.Show("商品" + msg + "【含税进价】为空，不能进行【科室申领录入】操作。", "消息提示", MessageBoxIcon.Warning);
                        continue;
                    }
                    //处理金额格式
                    decimal jingdu = 0;
                    decimal bzhl = 0;
                    if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                    if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F4"); }
                    if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }
                    docMEMO.Enabled = true;
                    List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                    int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == row["GDSEQ"].ToString()).Count();
                    if (sameRowCount > 0)
                    {
                        someDjbh += "【" + row["GDNAME"].ToString() + "】";
                        getDjbh = true;
                    }
                    else
                    {
                        PubFunc.GridRowAdd(GridGoods, row, false);
                        docDEPTID.Enabled = false;
                    }
                }
                if (getDjbh)
                {
                    Alert.Show("商品名称：" + someDjbh + "申请明细中已存在", "消息提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("请先选择要加载的商品信息！", "消息提示", MessageBoxIcon.Warning);
            }
        }
    }
}
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
    public partial class StorageAcnCk : BillBase
    {
        private string strDocSql = @"SELECT A.*,F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,F_GETDEPTNAME(A.DEPTID) DEPTNAME,F_GETUSERNAME(A.LRY) LRYNAME,F_GETUSERNAME(A.SPR) SPRNAME,F_GETUSERNAME(A.SHR) SHRNAME,F_GETUSERNAME(A.SLR) SLRNAME
                        ,decode(A.FLAG,'N','新单','A','已提交','W','已出库','R','已驳回','Y','已收货','未定义') FLAGNAME
                       FROM DAT_DB_DOC A WHERE A.FLAG <> 'N' ";
        private string strComSql = @"SELECT B.*,F_GETUNITNAME(B.UNIT) UNITNAME,F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                              NVL(B.RQ_SC,TO_DATE(null)) RQ_SC,NVL(B.YXQZ,TO_DATE(null)) YXQZ,
                               (select nvl(sum(kcsl-locksl),0) from dat_goodsstock where gdseq = b.gdseq and ph like nvl(b.ph,'%') and
                            deptid in(select deptout from DAT_DB_DOC where seqno = '{0}' and kcsl>locksl)) KCSL,f_getunitname(A.UNIT) UNITSMALLNAME FROM
                             DOC_GOODS A ,DAT_DB_COM B WHERE SEQNO ='{0}' AND A.GDSEQ=B.GDSEQ ORDER BY ROWNO ASC";
        public override Field[] LockControl
        {
            get { return new Field[] { docSEQNO, docDEPTID, docSLR, docDEPTOUT, docXSRQ, docMEMO }; }
        }

        public StorageAcnCk()
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
                ButtonHidden(btnCopy, btnNext, btnBef, btnExport, btnGoods, btnDelRow, btnAddRow, btnDel, btnNew);
                //配置模块权限
            }
        }

        private void DataInit()
        {
            //天津医科大学总医院 申请库房按照登录用户权限来加载，出库库房加载全部 c 20151024
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, lstDEPTID, docDEPTID);
            PubFunc.DdlDataGet("DDL_SYS_DEPOT", lstDEPTOUT, docDEPTOUT);

            PubFunc.DdlDataGet("DDL_USER", lstSLR, lstLRY, docLRY, docSLR, docSPR);

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
            JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0) return;
            if (e.ColumnID == "XSSL")
            {
                if (!PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "XSSL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZSL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "HSJJ")))
                {
                    Alert.Show("商品信息异常，请详细检查商品信息：包装含量或价格！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                decimal hl = 0, rs = 0, jg = 0;
                decimal.TryParse((defaultObj["BZHL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["XSSL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["DHSL"] = rs * hl;
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0, xsslTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    if (dic["BZSL"].ToString().Length > 0 && dic["HSJJ"].ToString().Length > 0)
                    {
                        bzslTotal += Convert.ToDecimal(dic["BZSL"]);
                        xsslTotal += Convert.ToDecimal(dic["XSSL"]);
                        feeTotal += Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["XSSL"]);
                    }
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("XSSL", xsslTotal.ToString());
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
        protected void btnScan_Click(object sender, EventArgs e)
        {
            if ((",M,R,N,A").IndexOf(docFLAG.SelectedValue) < 0)
            {
                zsmScan.Enabled = false;
                zsmDelete.Enabled = false;
            }
            else
            {
                zsmScan.Enabled = true;
                zsmDelete.Enabled = true;
            }
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

        protected void zsmScan_TextChanged(object sender, EventArgs e)
        {
            if ((",M,R,N,A").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新单』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GZ_EXT WHERE ONECODE = '{0}'  AND FLAG IN( 'Y','R')", zsmScan.Text)))
            {
                Alert.Show("您输入的追溯码已扫描使用,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_DB_EXT WHERE ONECODE = '{0}'  AND billno='{1}'", zsmScan.Text, docBILLNO.Text)))
            {
                Alert.Show("您输入的追溯码已扫描使用,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            try {
                //写入数据库中
                OracleParameter[] parameters = new OracleParameter[]
                    {
                     new OracleParameter("V_BILLNO",OracleDbType.Varchar2),
                     new OracleParameter("V_ONECODE",OracleDbType.Varchar2),
                    };
                parameters[0].Value = docSEQNO.Text;
                parameters[1].Value = zsmScan.Text;
                DbHelperOra.RunProcedure("STOREDS.P_INS_DBD", parameters);
                billOpen(docBILLNO.Text);
                ScanSearch("");
            }
            catch (Exception ex)
            {
                Alert.Show(ERPUtility.errorParse(ex.Message), "消息提示", MessageBoxIcon.Warning);
            }

        }

        protected void zsmDelete_Click(object sender, EventArgs e)
        {
            if ((",M,R,N,A").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新增』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridSacn.SelectedRowIndexArray.Count() < 1)
            {
                Alert.Show("请选择您需要删除的数据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string barcode = "";
            foreach (int rowIndex in GridSacn.SelectedRowIndexArray)
            {
                barcode += GridSacn.DataKeys[rowIndex][0].ToString() + ",";
            }
            string onecode = barcode.Replace(",", "','");
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo(string.Format("DELETE FROM DAT_DB_EXT WHERE ONECODE in( '{0}') AND BILLNO = '{1}'", onecode, docBILLNO.Text), null));
            cmdList.Add(new CommandInfo(string.Format("UPDATE DAT_DB_COM SET STR1 = '',STR2 = '',XSSL = 0,PH = '',PHID = '',YXQZ = NULL,RQ_SC = NULL WHERE SEQNO = '{1}' AND STR1 in( '{0}')", onecode, docBILLNO.Text), null));
            DbHelperOra.ExecuteSqlTran(cmdList);
            ScanSearch("");
            billOpen(docBILLNO.Text);
            OperLog("调拨出库单", "修改单据【" + docSEQNO.Text + "】高值码");
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
            zsmScan.Text = String.Empty;
            zsmScan.Focus();
        }

        protected override void billDelRow()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非『新增』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridGoods.SelectedRowID == null)
            {
                Alert.Show("当前未选中单元行，无法进行操作!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            PageContext.RegisterStartupScript(GridGoods.GetDeleteRowReference(GridGoods.SelectedRowID));
            PubFunc.FormLock(FormDoc, true, "");
        }

        protected override void billGoods()
        {
            return;
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
            if (lstFLAG.SelectedValue != null && !string.IsNullOrWhiteSpace(lstFLAG.SelectedValue))
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedValue);
            }
            if (lstSLR.SelectedValue != null && !string.IsNullOrWhiteSpace(lstSLR.SelectedValue))
            {
                strSearch += string.Format(" AND A.SLR='{0}'", lstSLR.SelectedValue);
            }
            if (lstLRY.SelectedValue != null && !string.IsNullOrWhiteSpace(lstLRY.SelectedValue))
            {
                strSearch += string.Format(" AND A.LRY='{0}'", lstLRY.SelectedValue);
            }
            if (lstDEPTID.SelectedValue != null && !string.IsNullOrWhiteSpace(lstDEPTID.SelectedValue))
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedValue);
            }
            if (lstDEPTOUT.SelectedValue != null && !string.IsNullOrWhiteSpace(lstDEPTOUT.SelectedValue))
            {
                strSearch += string.Format(" AND DEPTOUT='{0}'", lstDEPTOUT.SelectedValue);
            }
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strDocSql = strDocSql + strSearch + " ORDER BY  DECODE(FLAG,'N','1','A','2','R','3','4'),LRRQ DESC";
            }
            GridList.DataSource = DbHelperOra.Query(strDocSql).Tables[0];
            GridList.DataBind();
        }
        protected void Bill_Audit(bool flag = true)
        {
            DataSave();
            if (docFLAG.SelectedValue == "A")
            {
                if (DbHelperOra.Exists("select 1 from dat_db_com where xssl < 1 and seqno = '" + docSEQNO.Text + "'") && flag)
                {
                    //Alert.Show("请首先进行库存分配,并确认调拨商品的批号信息不为空!", "提示信息", MessageBoxIcon.Warning);
                    PageContext.RegisterStartupScript(Confirm.GetShowReference("此调拨单存在出库数量都为0的商品，是否继续？",
                    "信息提示", MessageBoxIcon.Information, PageManager1.GetCustomEventReference(true, "Confirm_Fp", false, false), null));
                    return;
                }
                //验证库房是否盘点
                if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID IN('" + docDEPTOUT.SelectedValue + "','" + docDEPTID.SelectedValue + "') AND FLAG='N'"))
                {
                    Alert.Show("库房正在盘点,请检查!");
                    return;
                }
                billLockDoc(true);
                //验证单据准确性
                DataTable dt = DbHelperOra.QueryForTable("select dg.gdseq from dat_db_com dbc,doc_goods dg where dbc.gdseq=dg.gdseq and dg.islot='2' and nvl(dbc.ph,'#') ='#' and  dbc.seqno='" + docSEQNO.Text + "'");
                if (dt.Rows.Count > 0)
                {

                }
                if (BillOper(docSEQNO.Text, "AUDIT") == 1)
                {
                    //if (!DbHelperOra.Exists("select 1 from dat_db_com where xssl < dhsl and seqno = '" + docSEQNO.Text + "'"))
                    if (!DbHelperOra.Exists("select 1 from (select seqno,gdseq,sum(xssl) xssl,sum(dhsl) dhsl from dat_db_com  group by seqno,gdseq,gdspec) a where a.xssl < a.dhsl and a.seqno = '" + docSEQNO.Text + "'"))
                    {
                        Alert.Show("调拨出库单【" + docSEQNO.Text + "】审核成功！");
                    }
                    else
                    {
                        Alert.Show("调拨出库单【" + docSEQNO.Text + "】审核成功，库存不足商品自动生成缺货！");
                    }
                    billOpen(docSEQNO.Text);
                    OperLog("商品调拨", "审核单据【" + docSEQNO.Text + "】-调拨出库");
                }
            }
            else
            {
                Alert.Show("调拨出库单【" + docSEQNO.Text + "】状态不正确", "提示信息", MessageBoxIcon.Warning);
            }
        }
        protected override void billAudit()
        {
            Bill_Audit();
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());
        }

        protected override void billOpen(string strBillno)
        {
            string StrSql = strDocSql + " AND A.SEQNO = '{0}'";
            DataTable dtDoc = DbHelperOra.Query(string.Format(StrSql, strBillno)).Tables[0];
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno)).Tables[0];
            decimal bzslTotal = 0, xsslTotal = 0, feeTotal = 0;
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["BZSL"]);
                    xsslTotal += Convert.ToDecimal((row["XSSL"] ?? "0"));
                    feeTotal += Convert.ToDecimal((row["XSSL"] ?? "0")) * Convert.ToDecimal(row["HSJJ"]);
                    LoadGridRow(row, false, "OLD");
                }
            }
            //增加合计
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("XSSL", xsslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
            PubFunc.FormLock(FormDoc, true, "");
            GridGoods.AllowCellEditing = false;

            if (docFLAG.SelectedValue.Equals("A") && txbTHTYPE.Text == "Y")
            {
                btnFp.Enabled = true;
                btnSave.Enabled = true;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnScan.Enabled = true;
                btnPrint.Enabled = true;
                Button2.Enabled = true;
                Button1.Enabled = false;
            }
            else if (txbTHTYPE.Text == "N" && docFLAG.SelectedValue.Equals("A"))
            {
                btnFp.Enabled = true;
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = true;
                btnScan.Enabled = false;
                btnPrint.Enabled = false;
                Button2.Enabled = false;
                Button1.Enabled = false;
            }
            else if (docFLAG.SelectedValue.Equals("R"))
            {
                btnFp.Enabled = false;
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnScan.Enabled = false;
                btnPrint.Enabled = false;
                Button2.Enabled = false;
                Button1.Enabled = false;
            }
            else
            {
                btnFp.Enabled = false;
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnScan.Enabled = false;
                btnPrint.Enabled = true;
                Button2.Enabled = true;
                Button1.Enabled = true;
            }
            TabStrip1.ActiveTabIndex = 1;
        }

        protected override void billSave()
        {
            if (DataSave())
            {
                Alert.Show("商品信息保存成功！");
                billOpen(docBILLNO.Text);
                OperLog("商品调拨", "保存单据【" + docBILLNO.Text + "】-调拨出库");
            }
        }

        private bool DataSave()
        {
            #region 数据有效性验证
            if ((",A").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("调拨申请单状态不正确,不能保存！", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["HWID"]).ToList();
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
                if (!PubFunc.StrIsEmpty(flg) && (",A").IndexOf(flg) < 0)
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
            MyTable mtType = new MyTable("DAT_DB_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow["FLAG"] = "A";//所有单据在保存时单据状态一律为新增N
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("XSTYPE", "1");
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_DB_COM");
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_DB_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_DB_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细

            decimal subsum = 0;
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);

                if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["HSJE"].ToString()) || mtTypeMx.ColRow["HSJE"].ToString() == "0")
                {
                    mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                }
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                if (mtTypeMx.ColRow["XSSL"].ToString() == "")
                {
                    subsum += decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                }
                else
                {
                    subsum += decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["XSSL"].ToString());
                }
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow["SUBSUM"] = subsum;
            cmdList.AddRange(mtType.InsertCommand());
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
            row["KCSL"] = Convert.ToInt32(row["KCSL"]) / Convert.ToInt32(DbHelperOra.GetSingle("SELECT DECODE(UNIT_SELL,'D',NUM_DABZ,'Z',NUM_ZHONGBZ,1) FROM DOC_GOODS WHERE GDSEQ = '" + row["GDSEQ"] + "'"));
            //row["HSJE"] = "0";
            PubFunc.GridRowAdd(GridGoods, row, firstRow);
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["PIZNO"].ColumnName = "PZWH";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "0";
                    row["DHSL"] = "0";
                    row["HSJE"] = "0";
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
                    defaultObj["PH"] = row.Values[1].ToString();
                    defaultObj["YXQZ"] = row.Values[2].ToString();
                    defaultObj["PZWH"] = row.Values[4].ToString();
                    defaultObj["RQ_SC"] = row.Values[3].ToString();
                    defaultObj["BZSL"] = tbxNumber.Text.ToString();
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

        protected override void billCancel()
        {
            if (docBILLNO.Text.Length < 1)
            {
                Alert.Show("请选择需要驳回的单据!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue.ToString() != "A")
            {
                Alert.Show("非『新增』不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            WindowReject.Hidden = false;
        }

        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            string strMemo = "";
            if (string.IsNullOrWhiteSpace(ddlReject.SelectedValue))
            {
                Alert.Show("请选择【驳回原因】");
                return;
            }
            else
            {
                strMemo = docMEMO.Text + "；驳回原因：" + ddlReject.SelectedText;
            }

            if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
            {
                strMemo += "；详细说明：" + txaMemo.Text;
            }
            if (DbHelperOra.ExecuteSql(string.Format("update DAT_DB_DOC set flag='R',THTYPE = 'N',memo='{0}' where seqno='{1}' and flag='A'", strMemo, docBILLNO.Text)) == 1)
            {
                WindowReject.Hidden = true;
                billOpen(docBILLNO.Text);
                OperLog("商品调拨", "驳回单据【" + docBILLNO.Text + "】-调拨出库");
            }

        }
        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (("NR").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新增』单据不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<string> listSql = new List<string>();
            listSql.Add("Delete from DAT_DB_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            listSql.Add("Delete from DAT_DB_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            if (DbHelperOra.ExecuteSqlTran(listSql))
            {
                Alert.Show("单据删除成功!", "消息提示", MessageBoxIcon.Information);
                OperLog("商品调拨", "删除单据【" + docSEQNO.Text.Trim() + "】-调拨出库");
                billNew();
                billSearch();
            }
            else
            {
                Alert.Show("单据删除失败!", "错误提示", MessageBoxIcon.Information);
            }
        }
        protected void btnFp_Click(object sender, EventArgs e)
        {
            FP_Action();
        }
        private void FP_Action()
        {
            //进行库存分配
            try{
                if (docFLAG.SelectedValue == "A")
                {
                    OracleParameter[] parameters = new OracleParameter[]
                    {
                     new OracleParameter("BILLNO",OracleDbType.Varchar2),
                     new OracleParameter("USERID",OracleDbType.Varchar2),
                    };
                    parameters[0].Value = docSEQNO.Text;
                    parameters[1].Value = UserAction.UserID;
                    DbHelperOra.RunProcedure("STOREDS.P_FP_DBD", parameters);
                    billOpen(docSEQNO.Text);
                    Alert.Show("库存分配成功，可继续审核出库!");
                }
                else
                {
                    Alert.Show("单据状态不正确,不能进行库存分配!");
                    return;
                }
            }
            catch (Exception ex)
            {
                Alert.Show(ERPUtility.errorParse(ex.Message), "消息提示", MessageBoxIcon.Warning);
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

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            switch (e.EventArgument)
            {
                case "Confirm_Fp":
                    Bill_Audit(false);
                    break;
            }
        }
    }
}
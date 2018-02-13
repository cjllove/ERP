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
    public partial class ApplyForSure : BillBase
    {
        private string strDocSql = "SELECT * FROM DAT_CK_DOC WHERE SEQNO ='{0}'";
        private string strComSql = @"SELECT A.SEQNO,A.ROWNO,A.GDSEQ,A.BARCODE,A.GDNAME,A.GDSPEC,A.GDMODE,A.HWID,F_GETBZHL(A.GDSEQ)BZHL,
(A.BZSL/F_GETBZHL(A.GDSEQ))BZSL,A.DHSL,A.XSSL,A.JXTAX,(A.HSJJ*F_GETBZHL(A.GDSEQ))HSJJ,A.HSJE,
A.LSJ,A.LSJE,A.ISGZ,A.ISLOT,A.PHID,A.PH,A.PZWH,A.RQ_SC,A.YXQZ,A.NUM1,A.ISJF,A.FPSL, 
F_GETSELLUNITNAME(A.GDSEQ) UNITNAME,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,f_getunitname(B.UNIT) UNITSMALLNAME,DECODE(NVL(A.NUM1,0),0,'非赠品','赠品') NUM1NAME
                    FROM DAT_CK_COM A,DOC_GOODS B WHERE SEQNO ='{0}' AND A.GDSEQ = B.GDSEQ ORDER BY ROWNO";
        protected string SHTXD = "/grf/spxsdisdg.grf";
        protected string FDS_SHTXD = "/grf/Fds_Shtx.grf";
        public override Field[] LockControl
        {
            get { return new Field[] { docDEPTID, docSLR, docDEPTOUT, docXSRQ, docMEMO }; }
        }

        public ApplyForSure()
        {
            BillType = "LCD";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //屏蔽不需要的操作按钮
                ButtonHidden(btnNew, btnAudit, btnCopy, btnDelRow, btnDel, btnExport);
                DataInit();
                billNew();
                if (Request.QueryString["pid"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["pid"].ToString()))
                {
                    lstBILLNO.Text = Request.QueryString["pid"].ToString();
                    string date = "20" + lstBILLNO.Text.Substring(3, 2) + "-" + lstBILLNO.Text.Substring(5, 2) + "-" + lstBILLNO.Text.Substring(7, 2);
                    lstLRRQ1.SelectedDate = DateTime.Parse(date).AddDays(-1);
                    billOpen(lstBILLNO.Text);
                }
                docSTR2.Enabled = false;
                hfdCurrent.Text = UserAction.UserID;
                if (string.IsNullOrWhiteSpace(lstDEPTOUT.SelectedValue))
                {
                    lstDEPTOUT.SelectedIndex = 1;
                }
                billSearch();

                if (Request.QueryString["tp"] != null && Request.QueryString["tp"].ToString().Trim().Length > 0)
                {
                    docDEPTOUT.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE=" + Request.QueryString["tp"].ToString()).ToString();
                    lstDEPTOUT.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE=" + Request.QueryString["tp"].ToString()).ToString();
                }
                else
                {
                    docDEPTOUT.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE='2'").ToString();
                    lstDEPTOUT.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE='2'").ToString();
                }

            }
        }

        private void DataInit()
        {
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, lstDEPTOUT, docDEPTOUT);
            lstDEPTOUT.SelectedValue = UserAction.UserDept;
            docDEPTOUT.SelectedValue = UserAction.UserDept;

            PubFunc.DdlDataGet("DDL_USER", lstLRY, docLRY, docSLR);
            //PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID, docDEPTID);
            PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID, docDEPTID);
            //PubFunc.DdlDataGet("DDL_BILL_STATUSCKD", lstFLAG, docFLAG);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-7);
            lstLRRQ2.SelectedDate = DateTime.Now;

            //获取客户化GRF文件地址  By c 2016年1月21日12:18:29 At 威海509
            string grf = Doc.DbGetGrf("KSSHTXD");
            if (!string.IsNullOrWhiteSpace(grf))
            {
                SHTXD = grf;
            }
            string grf_fds = Doc.DbGetGrf("FDS_SHTXD");
            if (grf_fds.Length > 0)
            {
                FDS_SHTXD = grf_fds;
            }
            //是否启用定数标签  By c 2016年1月27日12:07:12 At 威海509
            string DSAUTO = Doc.DbGetSysPara("DSAUTO");
            if (DSAUTO == "Y")
            {
                btnPBQ.Hidden = false;
            }
        }
        public string errorParse(string error)
        {
            string value = string.Empty;
            if (error.IndexOf("ORA-") > -1)
            {
                value = error.Replace("\n", "").Substring(error.IndexOf("ORA-") + 10);
                if (value.IndexOf("ORA-") > -1)
                {
                    value = value.Substring(0, value.IndexOf("ORA-"));
                }
            }
            else
            { value = error; }
            return value;

        }
        protected override void billNew()
        {
            string strDept = docDEPTID.SelectedValue;
            string strDeptOut = docDEPTOUT.SelectedValue;
            PubFunc.FormDataClear(FormDoc);
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
            docFLAG.SelectedValue = "N";
            docSLR.SelectedValue = UserAction.UserID;
            docLRY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDept;
            docDEPTOUT.SelectedValue = strDeptOut;

            billLockDoc(false);
            GridGoods.SummaryData = null;
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());

        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                FineUIPro.BoundField flagcol = GridList.FindColumn("FLAG_CN") as FineUIPro.BoundField;
                if (row["FLAG"].ToString() == "S")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color1";
                }
            }
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            string[] strCell = GridGoods.SelectedCell;
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();

            if (e.ColumnID == "BZSL")
            {
                if (!PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZHL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "BZSL")) || !PubFunc.isNumeric(Doc.GetGridInf(GridGoods, e.RowID, "HSJJ")))
                {
                    if (Doc.GetGridInf(GridGoods, e.RowID, "BZHL") == "非赠品")
                    {
                        Alert.Show("商品信息异常，请详细检查商品信息：包装含量、价格或数量！", "异常信息", MessageBoxIcon.Warning);
                        return;
                    }
                }
                //处理返回jobject
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
                decimal hl = 0, rs = 0, jg = 0;
                decimal.TryParse((defaultObj["BZHL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["BZSL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["XSSL"] = rs * hl;
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                if (Convert.ToDecimal(Doc.GetGridInf(GridGoods, e.RowID, "BZSL")) * Convert.ToDecimal(Doc.GetGridInf(GridGoods, e.RowID, "BZHL")) > Convert.ToDecimal(Doc.GetGridInf(GridGoods, e.RowID, "DHSL")) && Doc.GetGridInf(GridGoods, e.RowID, "NUM1NAME") == "非赠品")
                {
                    Alert.Show("拣货数量不能大于申领数量!", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0, dhslTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    bzslTotal += Convert.ToDecimal(dic["BZSL"] ?? "0");
                    feeTotal += Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0");
                    dhslTotal += Convert.ToDecimal(dic["DHSL"] ?? "0");
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
                DataTable dtPH = Doc.GetGoodsPHKC(gdseq, docDEPTOUT.SelectedValue);
                if (dtPH != null && dtPH.Rows.Count > 0)
                {
                    if (dtPH.Rows.Count == 1)
                    {
                        JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
                        defaultObj["PH"] = dtPH.Rows[0]["PH"].ToString();
                        defaultObj["PZWH"] = dtPH.Rows[0]["PZWH"].ToString();
                        defaultObj["RQ_SC"] = dtPH.Rows[0]["RQ_SC"].ToString();
                        defaultObj["YXQZ"] = dtPH.Rows[0]["YXQZ"].ToString();
                        defaultObj["KCSL"] = dtPH.Rows[0]["KCSL"].ToString();
                        defaultObj["HWID"] = dtPH.Rows[0]["HWID"].ToString();
                        PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                    }
                    else
                    {
                        GridLot.DataSource = dtPH;
                        GridLot.DataBind();
                        WindowLot.Hidden = false;
                    }
                }
                else
                {
                    Alert.Show("请先维护商品批号！", MessageBoxIcon.Warning);
                }
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
            billLockDoc(true);
            PubFunc.GridRowAdd(GridGoods, "INIT");
        }

        protected override void billDelRow()
        {
            //if (docFLAG.SelectedValue != "N")
            //{
            //    Alert.Show("非新增单据不能删除！");
            //    return;
            //}
            //int rowIndex = GridGoods.SelectedRowIndex;
            //PageContext.RegisterStartupScript(GridGoods.GetDeleteRowReference(rowIndex));
        }

        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            //参数说明：cx-查询内容，bm-商品配置部门,su-供应商
            string url = "~/ERPQuery/GoodsWindow_Zp.aspx?bm=" + docDEPTOUT.SelectedValue + "_" + docDEPTID.SelectedValue + "&cx=&su=";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "赠品商品信息查询"));
        }

        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【申领日期】！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("【开始日期】大于【结束日期】，请重新输入！", "提示信息", MessageBoxIcon.Warning);
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO,A.FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,
                                                      DECODE(A.FLAG,'M','新单','N','已提交','S' ,'已分配','R' ,'已驳回','Y' ,'已出库','G' ,'已结算')  FLAG_CN,
                                                      A.SUBNUM,F_GETUSERNAME(A.SLR) SLR,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO,A.STR2,DECODE(OPER,'P','已打印','未打印') PRINT, FUNCTIME,OPERUSER,OPERTIME
                                           FROM DAT_CK_DOC A, SYS_FUNCPRNNUM B
                                       WHERE A.XSTYPE='1' AND A.BILLTYPE='LCD' AND A.SEQNO = B.FUNCNO(+) ";
            string strSearch = "";
            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (tbxSTR2.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND A.STR2 LIKE '%{0}%'", tbxSTR2.Text.Trim());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstDEPTOUT.SelectedItem != null && lstDEPTOUT.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND DEPTOUT='{0}'", lstDEPTOUT.SelectedItem.Value);
            }
            if (lstLRY.SelectedItem != null && lstLRY.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND LRY = '{0}'", lstLRY.SelectedItem.Value);
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY DECODE(A.FLAG,'M','1','N','2','S' ,'3','R' ,'4','Y' ,'5','G' ,'6','7'),A.BILLNO DESC";
            int total = 0;
            GridList.DataSource = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSql, ref total);
            GridList.RecordCount = total;
            GridList.DataBind();
        }

        protected override void billAudit()
        {
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());
        }
        protected override void billOpen(string strBillno)
        {
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            if (dtDoc == null || dtDoc.Rows.Count < 1) return;
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            decimal bzslTotal = 0, feeTotal = 0, dhslTotal = 0;
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno)).Tables[0];
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    //LoadGridRow(row, false, "OLD");
                    bzslTotal += Convert.ToDecimal(row["BZSL"] ?? "0");
                    feeTotal += Convert.ToDecimal(row["HSJJ"] ?? "0") * Convert.ToDecimal(row["BZSL"] ?? "0");
                    dhslTotal += Convert.ToDecimal(row["DHSL"] ?? "0");
                }
                Doc.GridRowAdd(GridGoods, dtBill);
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
            //按钮状态
            if (docFLAG.SelectedValue == "S")
            {
                btnSave.Enabled = true;
                btnBill.Enabled = true;
                btnPrint.Enabled = false;
                btnGoods.Enabled = true;
            }
            else
            {
                btnSave.Enabled = false;
                btnBill.Enabled = true;
                btnPrint.Enabled = true;
                btnGoods.Enabled = false;
            }
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            switch (e.EventArgument)
            {
                case "btnOk":
                    btnOk();
                    break;
                case "btnCancel":
                    break;
            }
        }

        protected override void billSave()
        {
            PageContext.RegisterStartupScript(Confirm.GetShowReference("请确认拣货出库信息正确无误，是否继续执行？", "消息提示", MessageBoxIcon.Information, PageManager1.GetCustomEventReference("btnOk"), PageManager1.GetCustomEventReference("btnCancel")));
        }

        protected void btnOk()
        {
            if (Doc.DbGetSysPara("LOCKSTOCK") == "Y")
            {
                Alert.Show("系统库存已被锁定，请等待物资管理科结存处理完毕再做审核处理！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }

            #region 数据有效性验证
            decimal subsum = 0;//总金额
            if (docFLAG.SelectedValue != "S")
            {
                Alert.Show("非『已审批』单据不能申领确定！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //验证科室是否盘点
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID IN('" + docDEPTOUT.SelectedValue + "','" + docDEPTID.SelectedValue + "') AND FLAG='N'"))
            {
                Alert.Show("出库或申领科室正在盘点,请检查!");
                return;
            }
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {
                    goodsData.Add(newDict[i]);
                    subsum = subsum + decimal.Parse(newDict[i]["HSJE"].ToString());
                }
            }

            if (goodsData.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("商品信息不能为空", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //验证单据信息
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_CK_DOC WHERE SEQNO = '" + docBILLNO.Text + "'") && docBILLNO.Enabled)
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
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'N') FROM DAT_CK_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
                if (!PubFunc.StrIsEmpty(flg) && (",N,R,S").IndexOf(flg) < 0)
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
            mtType.ColRow.Add("SUBSUM", subsum);
            mtType.ColRow.Add("XSTYPE", "1");
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_CK_COM");
            //MyTable mtTypeExt = new MyTable("DAT_CK_EXT");
            //先删除单据信息在插入
            cmdList.Add(mtType.DeleteCommand(""));//删除单据台头
            cmdList.Add(new CommandInfo("delete dat_ck_com where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            cmdList.AddRange(mtType.InsertCommand());
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow["XSSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                if (decimal.Parse(mtTypeMx.ColRow["XSSL"].ToString()) < 0 || (decimal.Parse(mtTypeMx.ColRow["XSSL"].ToString()) > decimal.Parse(mtTypeMx.ColRow["DHSL"].ToString()) && (mtTypeMx.ColRow["NUM1NAME"].ToString() == "非赠品")))
                {
                    Alert.Show("单据号中存在拣货数量小于0或拣货数量大于申领数量！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);

                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                cmdList.Add(mtTypeMx.Insert());
            }

            OracleParameter[] parameters = {
                                               new OracleParameter("VTASKID", OracleDbType.Varchar2,20),
                                               new OracleParameter("VPARA", OracleDbType.Varchar2,800) };
            parameters[0].Value = BillType;
            parameters[1].Value = "'" + docBILLNO.Text + "','" + BillType + "','" + UserAction.UserID + "','AUDIT'";
            cmdList.Add(new CommandInfo("P_EXECTASK", parameters, CommandType.StoredProcedure));

            bool flag = false;
            try { flag = DbHelperOra.ExecuteSqlTran(cmdList); }
            catch (Exception ex)
            {
                Alert.Show(errorParse(ex.Message), "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (flag)
            {
                billLockDoc(true);
                //增加待办事宜
                DbHelperOra.ExecuteSql("UPDATE DAT_DO_LIST SET FLAG = 'Y' WHERE PARA='" + docBILLNO.Text.Trim() + "'");
                if (DbHelperOra.Exists("select 1 from dat_ck_com where seqno = '" + docBILLNO.Text.Trim() + "' and gdseq in (select gdseq from doc_goods where catid0 = 1 )") == true)
                {
                    Alert.Show("商品出库确认完成！需要打印试剂条码！", "消息提示", MessageBoxIcon.Information);
                }
                else
                {
                    Alert.Show("商品出库确认完成！", "消息提示", MessageBoxIcon.Information);
                    OperLog("科室申领", "出库确认单据【" + docBILLNO.Text + "】");
                }
                billOpen(docBILLNO.Text);
            }
            else
            {
                Alert.Show("商品出库确认出错！！！", "错误提示", MessageBoxIcon.Error);
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
                dt.Columns["PIZNO"].ColumnName = "PZWH";
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
                dt.Columns.Add("KCSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt.Columns.Add("NUM1", Type.GetType("System.Int32"));
                dt.Columns.Add("NUM1NAME", Type.GetType("System.String"));
                dt.Columns.Add("XSSL", Type.GetType("System.Double"));
                string msg1 = "";
                string msg2 = "";
                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "0";
                    row["DHSL"] = "0";
                    row["KCSL"] = "0";
                    row["HSJE"] = "0";
                    row["XSSL"] = "0";
                    row["HWID"] = "";
                    row["NUM1"] = "1";
                    row["NUM1NAME"] = "赠品";
                    string sSQL = string.Format("SELECT A.HWID,A.KCSL,A.PH,A.YXQZ,A.RQ_SC,B.ISGZ,B.GDNAME FROM DAT_GOODSSTOCK A ,DOC_GOODS B WHERE A.DEPTID ='{0}' AND A.GDSEQ = '{1}'  AND A.GDSEQ = B.GDSEQ AND A.KCSL >0 AND ROWNUM = 1 ORDER BY A.PICINO ASC", docDEPTOUT.SelectedValue, row["GDSEQ"].ToString());
                    DataTable Temp = DbHelperOra.Query(sSQL).Tables[0];
                    if (Temp.Rows.Count > 0)
                    {
                        row["KCSL"] = Temp.Rows[0]["KCSL"];
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

                    if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()))
                    {
                        msg2 += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                        continue;
                    }
                    //换算价格
                    //row["HSJJ"] = Math.Round(Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZHL"]), 4);
                    //处理金额格式
                    decimal jingdu = 0;
                    decimal bzhl = 0;
                    if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                    if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F4"); }
                    if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }

                    LoadGridRow(row, false);
                }
                if (!string.IsNullOrWhiteSpace(msg1))
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】在部门『{1}』中没有库存,不能进行录入！", msg1, docDEPTOUT.SelectedText);
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
                    string[] strCell = GridGoods.SelectedCell;
                    JObject defaultObj = Doc.GetJObject(GridGoods, strCell[0]);
                    defaultObj["PH"] = row.Values[2].ToString();
                    defaultObj["YXQZ"] = row.Values[3].ToString();
                    defaultObj["PZWH"] = row.Values[5].ToString();
                    defaultObj["RQ_SC"] = row.Values[4].ToString();
                    defaultObj["BZSL"] = tbxNumber.Text;
                    defaultObj["HWID"] = row.Values[8].ToString();
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

        protected void btnScan_Click(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue != "S")
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
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_CK_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y'  AND SEQNO = '{0}'", docSEQNO.Text)))
            {
                WindowScan.Hidden = false;
                ScanSearch("SHOW");
            }
            else
            {
                if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_CK_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.CATID0 = '1'  AND SEQNO = '{0}'", docSEQNO.Text)))
                {
                    WindowScan.Hidden = false;
                    ScanSearch("SJSHOW");
                }
                else
                {
                    Alert.Show("此单据中没有已经保存的高值商品或试剂,请检查！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }

            }
        }

        protected void btExport_Click(object sender, EventArgs e)
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【申领日期】！");
                return;
            }
            if (GridList.Rows.Count == 0)
            {
                Alert.Show("明细中无数据，不能导出");
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
                                       B.BZHL 包装含量,
                                       B.BZSL 申领包装数,
                                       B.XSSL 申领数,B.HSJJ 价格,B.PH 批号,B.RQ_SC 生产日期,B.YXQZ 有效期至
                                  FROM DAT_CK_DOC A, DAT_CK_COM B
                                 WHERE A.SEQNO=B.SEQNO
                                   AND A.BILLTYPE = '" + BillType + @"'
                                   AND A.XSTYPE = '1' ";
            string strSearch = "";


            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text);
            }
            if (lstFLAG.SelectedItem != null && lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
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
                strSearch += string.Format(" AND A.DEPTOUT='{0}'", lstDEPTOUT.SelectedItem.Value);
            }

            strSearch += string.Format(" AND A.deptid in( SELECT CODE FROM SYS_DEPT WHERE TYPE <>'1' AND  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC,B.ROWNO";

            XTBase.Utilities.ExcelHelper.ExportByWeb(DbHelperOra.Query(strSql).Tables[0], "申领出库信息", string.Format("申领出库信息_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
        #region 追溯码
        protected void zsmScan_TextChanged(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue == "Y")
            {
                Alert.Show("非『新单』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //增加输入二维码验证
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GZ_EXT WHERE (ONECODE = '{0}' OR STR1 = '{0}') AND FLAG IN('Y','R') AND DEPTCUR = '{1}'", zsmScan.Text.Trim(), docDEPTOUT.SelectedValue)))
            {
                Alert.Show("您扫描的二维码已被出库或未入库审核，请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            //增加输入二维码验证
            if (!DbHelperOra.Exists(string.Format(@"SELECT *
                        FROM DAT_STOCKLOCK A,DAT_STOCKDAY B,DAT_GZ_EXT C
                        WHERE A.PICINO = B.PICINO AND A.LOCKBILLNO = '{0}'
                        AND C.ONECODE = '{1}' AND B.PH = C.PH AND B.BILLNO = C.BILLNO", docBILLNO.Text, zsmScan.Text.Trim())))
            {
                Alert.Show("您扫描的高值与已预占高值信息对应错误，请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_CK_EXT WHERE BILLNO = '{0}' AND (ONECODE = '{1}' OR STR1 = '{1}')", docBILLNO.Text, zsmScan.Text.Trim())))
            {
                Alert.Show("您扫描的二维码已被扫描，请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            //写入数据库中
            DbHelperOra.ExecuteSql(string.Format(@"INSERT INTO DAT_CK_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,INSTIME,PH,RQ_SC,YXQZ,STR1)
                    SELECT '{0}','{1}',NVL((SELECT MAX(ROWNO)+1 FROM DAT_CK_EXT WHERE BILLNO = '{1}'),1),A.ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,SYSDATE,PH,RQ_SC,YXQZ,STR1
                    FROM DAT_RK_EXT A
                    WHERE A.ONECODE = '{2}' OR A.STR1 = '{2}'", docDEPTID.SelectedValue, docBILLNO.Text, zsmScan.Text.Trim()));
            ScanSearch("");
        }

        protected void zsmDelete_Click(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue != "S")
            {
                Alert.Show("非『新单』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridSacn.SelectedCell == null)
            {
                Alert.Show("请选择您需要删除的数据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if ((Convert.ToInt32(GridSacn.SelectedRowIndex)) < 0)
            {
                Alert.Show("请选择您需要删除的数据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string onecode = (GridSacn.DataKeys[GridSacn.SelectedRowIndex][0]).ToString();
            DbHelperOra.ExecuteSql(string.Format("DELETE FROM DAT_CK_EXT WHERE ONECODE = '{0}' AND BILLNO = '{1}'", onecode, docBILLNO.Text));
            ScanSearch("");
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
                if (type == "SJSHOW")
                {
                    GoodsType.Text = "试剂";
                    sql = @"select B.GDSEQ,
                                   B.GDNAME,
                                   B.GDSPEC,
                                   f_getunitname(b.unit) UNITNAME,
                                   '1' BZHL,
                                   A.PH,
                                   '1' BZSL,
                                   A.GDBARCODE ONECODE
                              from DAT_BARCODE_SJ A, DOC_GOODS B
                             WHERE A.GDSEQ = B.GDSEQ and a.rkseqno='{0}'
                             order by onecode";
                }
                else
                {
                    sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_CK_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.INSTIME DESC";
                }

            }
            DataTable dtScan = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            //PubFunc.GridRowAdd(GridSacn, dtScan);
            GridSacn.DataSource = dtScan;
            GridSacn.DataBind();
            zsmScan.Text = String.Empty;
            zsmScan.Focus();
        }
        #endregion

        protected void GridList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
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

        protected void btnPBQ_Click(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue != "Y")
            {
                Alert.Show("单据未确认出库,不允许打印！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string sql = @"SELECT 1 FROM DAT_CK_BARCODE A,DAT_CK_COM B WHERE A.SEQNO=B.SEQNO AND A.GDSEQ=B.GDSEQ ";
            sql += " AND A.SEQNO='" + docSEQNO.Text + "'";
            if (!DbHelperOra.Exists(sql))
            {
                Alert.Show("单据中没有需要打印的标签条码！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            PageContext.RegisterStartupScript("PrintLable();");
        }
    }
}
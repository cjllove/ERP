﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text.RegularExpressions;

namespace ERPProject.ERPApply
{
    public partial class DrugConsumption : BillBase
    {
        private string strDocSql = "SELECT * FROM DAT_XS_DOC WHERE SEQNO ='{0}'";

        private string strComSql = @"SELECT A.*,F_GETUNITNAME(B.UNIT) UNITNAME,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,f_getunitname(B.UNIT) UNITSMALLNAME,
                                    (CASE WHEN A.STR1 IS NULL THEN 
                                    NVL((SELECT SUM(KCSL - LOCKSL) FROM DAT_GOODSSTOCK AA WHERE AA.DEPTID = C.DEPTID AND AA.GDSEQ = A.GDSEQ AND NVL(AA.PH,'%') = NVL(A.PH,'%')),0)
                                    ELSE A.BZSL END) KCSL 
                FROM DAT_XS_COM A,DOC_GOODS B,DAT_XS_DOC C WHERE A.SEQNO ='{0}' AND A.GDSEQ = B.GDSEQ AND A.SEQNO = C.SEQNO ORDER BY A.ROWNO";
        protected string SPXSD = "/grf/spxsd.grf";
        protected string REPORT = "/captcha/PrintReport.aspx";
        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docDEPTID, docXSRQ }; }
        }

        public DrugConsumption()
        {
            BillType = "XSD";
        }

        private void ShowPHWindow(string rowId)
        {
            String gdseq = Doc.GetGridInf(GridGoods, rowId, "GDSEQ");
            if (gdseq.Length < 1)
            {
                Alert.Show("请先选择商品信息！", "异常信息", MessageBoxIcon.Question);
                return;
            }
            String ph = Doc.GetGridInf(GridGoods, rowId, "PH");
            if (ph.Length < 1 && DbHelperOra.Exists(string.Format("SELECT 1 FROM DOC_GOODS WHERE GDSEQ = '{0}' AND ISLOT = '2'", gdseq)))
            {
                Alert.Show("请填写商品[" + gdseq + "]批次信息！", "异常信息", MessageBoxIcon.Warning);
                return;
            }

            if (ph.ToString() == "\\")
            {
                DataTable dtPH = Doc.GetGoodsPH_New(gdseq, docDEPTID.SelectedValue);
                if (dtPH != null && dtPH.Rows.Count > 0)
                {
                    hfdRowIndex.Text = GridGoods.SelectedRowIndex.ToString();
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
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;


            //当前用户是护士长，申领人下拉只显示当前科室下的用户
            if (UserAction.UserRole == "02")
            {
                string sqls = string.Format(@"select  CODE,NAME from (
SELECT '--请选择--' NAME,'' CODE  FROM dual
union all
select username name, userid code
       from sys_operuser 
       where roleid = '{0}' and dept = '{1}' and islogin = 'Y')", UserAction.UserRole, UserAction.UserDept);
                PubFunc.DdlDataSql(ddlUser, sqls);
                PubFunc.DdlDataSql(docLRY, sqls);
            }
            else
            {
                PubFunc.DdlDataGet("DDL_USER", ddlUser, docLRY);
            }

            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
            //获取客户化GRF文件地址  By c 2016年1月21日12:18:29 At 威海509
            string grf = Doc.DbGetGrf("SPXSD");
            if (!string.IsNullOrWhiteSpace(grf))
            {
                SPXSD = grf;
            }

            string report = ConfigurationManager.AppSettings["REPORT"];
            if (!string.IsNullOrWhiteSpace(report))
            {
                REPORT = report;
            }
        }
        protected override void billNew()
        {
            //原单据保存判断
            string strDept = docDEPTID.SelectedValue;
            PubFunc.FormDataClear(FormDoc);

            docFLAG.SelectedValue = "N";
            docLRY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDept;
            if (docDEPTID.SelectedValue.Length < 1)
            {
                docDEPTID.SelectedIndex = 1;
            }
            tbxNUM.Text = "0";
            billLockDoc(false);
            tbxBARCODE.Enabled = true;
            btnAudit.Enabled = false;
            btnExtraction.Enabled = true;
            btnSave.Enabled = true;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            tbxBARCODE.Focus();
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
                    DataTable dtPH = Doc.GetGoodsPH_New(gdseq, docDEPTID.SelectedValue);
                    if (dtPH != null && dtPH.Rows.Count > 0)
                    {
                        hfdRowIndex.Text = GridGoods.SelectedRowIndex.ToString();
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

        protected override void billDelRow()
        {
            if (docFLAG.SelectedValue != "N")
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
            //计算合计数量
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
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

        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            PubFunc.FormLock(FormDoc, true, "");
            string url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTID.SelectedValue + "&GoodsState=YT";
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

            string strSql = @"SELECT A.SEQNO,A.BILLNO,decode(a.flag,'N','新单','Y','已审核','G','已完结') flag,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,
                                     A.SUBNUM,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO
                                from DAT_XS_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' AND BILLTYPE='XSD' AND XSTYPE='1' ";
            string strSearch = "";


            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND TRIM(UPPER(A.BILLNO)) LIKE '%{0}%'", lstBILLNO.Text.Trim().ToUpper());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (!string.IsNullOrWhiteSpace(ddlFLag.SelectedValue))
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", ddlFLag.SelectedValue);
            }
            if (ddlUser.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND A.LRY =  '{0}'", ddlUser.SelectedValue);
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.XSRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY decode(flag,'N','1','2'),A.BILLNO DESC";
            GridList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataBind();
        }
        private bool SaveSuccess = false;
        protected override void billAudit()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("本条使用信息已经审核确认，不需要再次审核！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //验证科室是否盘点
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID = '" + docDEPTID.SelectedValue + "' AND FLAG='N'"))
            {
                Alert.Show(string.Format("部门【{0}】正在盘点,请盘点后进行操作！", docDEPTID.SelectedText), "警告提示", MessageBoxIcon.Warning);
                return;
            }
            SaveSuccess = false;
            save("Y");
            if (SaveSuccess == false)
                return;
            SaveSuccess = false;

            List<CommandInfo> cmdList = new List<CommandInfo>();

            //商品使用信息在保存之后即进行审核操作
            OracleParameter[] parameters = {
                                               new OracleParameter("VTASKID", OracleDbType.Varchar2,20),
                                               new OracleParameter("VPARA", OracleDbType.Varchar2,800) };
            parameters[0].Value = BillType;
            parameters[1].Value = "'" + docBILLNO.Text + "','" + BillType + "','" + UserAction.UserID + "','AUDIT'";
            cmdList.Add(new CommandInfo("P_EXECTASK", parameters, CommandType.StoredProcedure));
            try
            {
                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    Alert.Show("商品使用信息审核成功！", "消息提示", MessageBoxIcon.Information);
                    OperLog("使用管理", "审核单据【" + docBILLNO.Text + "】");
                    billOpen(docBILLNO.Text);
                }
            }
            catch (Exception ex)
            {
                //ex.Message
                Alert.Show(Error_Parse(ex.Message), "提示信息", MessageBoxIcon.Warning);
            }
        }

        public static string Error_Parse(string error)
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
            {
                value = error;
            }

            return value;
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
            if (docFLAG.SelectedValue == "N")
            {
                tbxBARCODE.Enabled = true;
                btnExtraction.Enabled = true;
                btnSave.Enabled = true;
                btnAudit.Enabled = true;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
            }
            else
            {
                btnExtraction.Enabled = false;
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnPrint.Enabled = true;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
            }
        }

        protected override void billSave()
        {
            save();
        }

        private void save(string flag = "N")
        {
            #region 数据有效性验证
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            decimal subsum = 0;
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {
                    //if (newDict[i]["ISLOT"].ToString() == "1" || newDict[i]["ISLOT"].ToString() == "2")
                    //{
                    //    if (string.IsNullOrWhiteSpace(newDict[i]["PH"].ToString()) || newDict[i]["PH"].ToString() == "\\")
                    //    {
                    //        //GridGoods.SelectedCell = new int[] { i, 8 };
                    //        string[] selectedCell = GridGoods.SelectedCell;
                    //        PageContext.RegisterStartupScript(String.Format("F('{0}').selectCell('{1}','{2}');", GridGoods.ClientID, selectedCell[0], "KCSL"));
                    //        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】批号不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                    //        return;
                    //    }
                    //}
                   
                    //if (newDict[i].ContainsKey("STR1")&&goodsData.Count > 0 && goodsData.Where(x => x["STR1"].ToString() == newDict[i]["STR1"].ToString()).Count() > 0)
                    //{
                    //    Alert.Show("条码[" + newDict[i]["STR1"] + "]扫描重复!", "消息提示", MessageBoxIcon.Warning);
                    //    return;
                    
                    if (!string.IsNullOrWhiteSpace(newDict[i]["STR1"].ToString()))
                    {
                        for (int k = 1 + i; k < newDict.Count; k++)
                        {
                            if ((newDict[i]["STR1"].ToString()) == (newDict[k]["STR1"].ToString()))
                            {
                                Alert.Show("商品『" + newDict[k]["GDNAME"].ToString() + "』条码『" + newDict[k]["STR1"].ToString() + "』重复，请维护！", "消息提示", MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    if (newDict[i]["BZSL"].ToString() == "0" || string.IsNullOrWhiteSpace(newDict[i]["BZSL"].ToString()))
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】使用数填写不正确！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (decimal.Parse(newDict[i]["KCSL"].ToString()) < decimal.Parse(newDict[i]["BZSL"].ToString()))
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
                    goodsData.Add(newDict[i]);
                    subsum += Convert.ToDecimal(newDict[i]["HSJJ"].ToString()) * Convert.ToDecimal(newDict[i]["BZHL"].ToString());
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
                if (!string.IsNullOrWhiteSpace(flg) && (",N,R").IndexOf(flg) < 0)
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
            mtType.ColRow.Add("XSTYPE", "1");

            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_XS_COM");
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_XS_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_XS_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            decimal subNum = 0;
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);
                if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["BZSL"].ToString()) || mtTypeMx.ColRow["BZSL"].ToString() == "0")
                {
                    Alert.Show("商品【" + mtTypeMx.ColRow["GDSEQ"] + " | " + mtTypeMx.ColRow["GDNAME"] + "】【使用数】为0或空，无法进行【使用信息管理】操作。");
                    return;
                }
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString());
                //mtTypeMx.ColRow["XSSL"] = goodsData[i]["DHSL"].ToString();
                mtTypeMx.ColRow["XSSL"] = mtTypeMx.ColRow["DHSL"];
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            cmdList.AddRange(mtType.InsertCommand());
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                if (flag == "N")
                    Alert.Show("商品使用信息保存成功！");
                OperLog("使用管理", "修改单据【" + docBILLNO.Text + "】");
                billOpen(docBILLNO.Text);
            }
            SaveSuccess = true;
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
            //dt.Columns.Remove(dt.Columns["UNIT"]);
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                dt.Columns["UNIT_SELL_NAME"].ColumnName = "UNITNAME";
                //dt.Columns["UNIT_SELL"].ColumnName = "UNIT";
                dt.Columns["BZHL_SELL"].ColumnName = "BZHL";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("KCSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt.Columns.Add("STR1", Type.GetType("System.String"));
                string msg = "";
                string msg1 = "";
                string msg2 = "";
                string msg3 = "";//
                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "0";
                    row["DHSL"] = "0";
                    row["KCSL"] = "0";
                    row["HSJE"] = "0";
                    row["HWID"] = "";
                    row["STR1"] = "";
                    row["UNITNAME"] = row["UNITSMALLNAME"];
                    row["BZHL"] = "1";
                    DataTable Temp = DbHelperOra.Query(string.Format("SELECT * FROM (SELECT A.HWID, SUM(A.KCSL) KCSL,A.PH,A.YXQZ,A.RQ_SC,B.ISGZ,B.GDNAME FROM DAT_GOODSSTOCK A ,DOC_GOODS B WHERE A.DEPTID ='{0}' AND A.GDSEQ = '{1}'  AND A.GDSEQ = B.GDSEQ AND A.KCSL >0 GROUP BY HWID,PH,YXQZ,RQ_SC,ISGZ,GDNAME ORDER BY YXQZ ASC) WHERE ROWNUM = 1", docDEPTID.SelectedValue, row["GDSEQ"].ToString())).Tables[0];

                    List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();//
                    int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == row["GDSEQ"].ToString()).Count();//

                    if (Temp.Rows.Count > 0)
                    {
                        if (Temp.Rows[0]["ISGZ"].ToString() == "Y")
                        {
                            msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                            continue;
                        }

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

                    if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                    {
                        msg2 += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                        continue;
                    }
                    if (sameRowCount > 0)//
                    {
                        msg3 += "【" + row["GDNAME"].ToString() + "】";
                        continue;
                    }
                    //换算价格
                    row["HSJJ"] = Math.Round(Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZHL"]), 4);
                    LoadGridRow(row, false);
                }

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】属于高值商品，请在【高值商品使用】里进行录入！", msg.Trim(','));
                    Alert.Show(strNostock, "消息提示", MessageBoxIcon.Warning);
                }
                if (!string.IsNullOrWhiteSpace(msg1))
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】无库存！", msg1.Trim(','));
                    Alert.Show(strNostock, "消息提示", MessageBoxIcon.Warning);
                }
                if (!string.IsNullOrWhiteSpace(msg2))
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】【含税进价】为空,不能进行【商品退货审批】操作。", msg2.Trim(','));
                    Alert.Show(strNostock, "消息提示", MessageBoxIcon.Warning);
                }
                if (!string.IsNullOrWhiteSpace(msg3))//
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】在申请明细中已存在。", msg3.Trim(','));
                    Alert.Show(strNostock, "消息提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("请选择需要添加的商品！", "消息提示", MessageBoxIcon.Warning);
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
                    defaultObj["KCSL"] = row.Values[6].ToString();
                    defaultObj["PH"] = row.Values[3].ToString();
                    defaultObj["YXQZ"] = row.Values[4].ToString();
                    defaultObj["PZWH"] = row.Values[9].ToString();
                    defaultObj["RQ_SC"] = row.Values[5].ToString();
                    defaultObj["BZSL"] = Math.Abs(Convert.ToDecimal(tbxNumber.Text));
                    defaultObj["HWID"] = row.Values[10].ToString();
                    defaultObj["HSJE"] = (Convert.ToInt16(tbxNumber.Text ?? "0") * Convert.ToDecimal(row.Values[11])).ToString();
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
            DbHelperOra.ExecuteSql("Delete from DAT_XS_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_XS_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
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
            int len = Doc.LENCODE();
            if (tbxBARCODE.Text.Trim().Length < len)
            { return; }
            DataTable dtCode = DbHelperOra.Query(string.Format("SELECT * FROM DAT_CK_BARCODE WHERE BARCODE = '{0}'", tbxBARCODE.Text)).Tables[0];
            if (dtCode == null || dtCode.Rows.Count != 1)
            {
                Alert.Show("条码信息错误，请检查！！！", "异常提示", MessageBoxIcon.Warning);
                return;
            }
            if (dtCode.Rows[0]["FLAG"].ToString() != "N")
            {
                Alert.Show("您扫描的条码状态不是【未使用】，请检查！！！", "异常提示", MessageBoxIcon.Warning);
                return;
            }
            string deptid = dtCode.Rows[0]["DEPTIN"].ToString();
            if (docDEPTID.SelectedValue == "")
            {
                docDEPTID.SelectedValue = deptid;
            }
            else
            {
                if (docDEPTID.SelectedValue != deptid)
                {
                    Alert.Show("扫描条码非[" + docDEPTID.SelectedText + "]科室条码,请检查!", "操作提示", MessageBoxIcon.Warning);
                    tbxBARCODE.Text = "";
                    tbxBARCODE.Focus();
                    return;
                }
            }

            if (tbxBARCODE.Text.Substring(0, 1) != "0")
            {
                //增加定数条码提示
                if (tbxBARCODE.Text.Substring(0, 1) == "1")
                {
                    Alert.Show("扫描定数条码为定数条码,请到【使定数条码回收】界面录入!", "异常提示", MessageBoxIcon.Warning);
                }
                else
                {
                    Alert.Show(string.Format("条码【{0}】不是非定数条码,不能在本页面进行回收扫描!", tbxBARCODE.Text), "异常提示", MessageBoxIcon.Warning);
                }
                tbxBARCODE.Text = "";
                tbxBARCODE.Focus();
                return;
            }

            //重新取得数量
            DataTable barcode = DbHelperOra.Query("select A.*,B.PIZNO,F_GETUNITNAME(B.UNIT)UNITSMALLNAME from dat_ck_barcode A,DOC_GOODS B where A.GDSEQ = B.GDSEQ AND B.FLAG IN('Y','T') AND A.BARCODE = '" + tbxBARCODE.Text + "' and A.FLAG = 'N'").Tables[0];
            if (barcode.Rows.Count < 1)
            {
                Alert.Show("扫描条码不存在或已经被回收请检查!");
                tbxBARCODE.Text = "";
                tbxBARCODE.Focus();
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
            for (int i = 0; i < newDict.Count; i++)
            {
                string barcode_old = newDict[i]["STR1"].ToString();
                if (barcode_old == tbxBARCODE.Text)
                {
                    Alert.Show("扫描定数条码已存在!");
                    tbxBARCODE.Text = "";
                    tbxBARCODE.Focus();
                    return;
                }
            }
            string code = dtCode.Rows[0]["GDSEQ"].ToString(); //tbxBARCODE.Text.Trim().Substring(13, 12);
            string dept = docDEPTID.SelectedValue;

            if (!string.IsNullOrWhiteSpace(code) && code.Trim().Length >= 2)
            {
                DataTable dt_goods = Doc.GetGoods_His(code, "", dept);

                if (dt_goods != null && dt_goods.Rows.Count > 0)
                {
                    dt_goods.Columns.Add("ONECODE", Type.GetType("System.String"));
                    dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("DHSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("KCSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                    dt_goods.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                    DataRow dr_goods = dt_goods.Rows[0];
                    dr_goods["BZSL"] = barcode.Rows[0]["DHSL"];
                    dr_goods["DHSL"] = barcode.Rows[0]["DHSL"];
                    dr_goods["KCSL"] = barcode.Rows[0]["DHSL"];
                    dr_goods["HSJE"] = Convert.ToDecimal(barcode.Rows[0]["BZSL"]) * Convert.ToDecimal(barcode.Rows[0]["HSJJ"]) ;
                    dr_goods["PH"] = barcode.Rows[0]["PH"];
                    dr_goods["PZWH"] = barcode.Rows[0]["PIZNO"];
                    dr_goods["RQ_SC"] = barcode.Rows[0]["RQ_SC"];
                    dr_goods["YXQZ"] = barcode.Rows[0]["YXQZ"];
                    //货位使用科室编码
                    dr_goods["HWID"] = dept;
                    dr_goods["STR1"] = tbxBARCODE.Text;
                    LoadGridRow(dr_goods, false);
                    tbxNUM.Text = (Convert.ToInt16(tbxNUM.Text) + 1).ToString();
                    tbxBARCODE.Text = "";
                    tbxBARCODE.Focus();
                }
                else
                {
                    Alert.Show(string.Format("{0}尚未配置商品【{1}】！！！", docDEPTID.SelectedText, code), MessageBoxIcon.Warning);
                    PubFunc.GridRowAdd(GridGoods, "CLEAR");
                }
            }
        }

        protected void btnExtraction_Click(object sender, EventArgs e)
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;

            //            string strSql = @"SELECT A.SEQNO,A.BILLNO,B.NAME FLAG_CN,A.FLAG,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,A.SUBNUM,F_GETUSERNAME(A.SLR) SLR
            //                                FROM DAT_CK_DOC A, SYS_CODEVALUE B 
            //                               WHERE A.FLAG = B.CODE AND A.FLAG = 'Y' AND B.TYPE  ='DIC_BILLSTATUS' AND A.BILLTYPE='LCD' AND A.XSTYPE='1' AND A.DEPTID='{0}' ";

            //            GridBill.DataSource = DbHelperOra.Query(string.Format(strSql, docDEPTID.SelectedValue)).Tables[0];
            //            GridBill.DataBind();
            DataSearch();
            WindowBill.Hidden = false;
        }

        protected void btnBillSave_Click(object sender, EventArgs e)
        {
            if (GridBill.SelectedRowIndexArray.Length > 0)
            {
                string msg = "";
                string strBillno = "";
                foreach (int index in GridBill.SelectedRowIndexArray)
                {
                    strBillno += GridBill.Rows[index].Values[0].ToString() + "','";
                }
                string sql = @"SELECT A.SEQNO,A.ROWNO,A.GDSEQ,A.BARCODE,A.GDNAME,A.UNIT,A.GDSPEC,A.GDMODE,'{1}' HWID,A.BZHL,A.BZSL,
                                      A.DHSL,A.XSSL,A.JXTAX,A.HSJJ,A.BHSJJ,A.HSJE,A.BHSJE,A.LSJ,A.LSJE,A.ISGZ,A.ISLOT,A.PHID,
                                      A.PZWH,A.PRODUCER,A.ZPBH,A.MEMO,F_GETUNITNAME(A.UNIT) UNITNAME,
                                      F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                      '' ONECODE,
                                      '' STR1
                                 FROM DAT_CK_COM A, DAT_CK_DOC B,DOC_GOODS C 
                                WHERE A.SEQNO = B.SEQNO AND A.GDSEQ = C.GDSEQ AND A.SEQNO IN ('{0}') AND B.FLAG='Y' AND  c.ISGZ='N'
                                ORDER BY A.SEQNO DESC, A.ROWNO ASC";
                DataTable dtBill = DbHelperOra.Query(string.Format(sql, strBillno.TrimEnd(new char[] { '\'', ',' }), docDEPTID.SelectedValue)).Tables[0];
                dtBill.Columns.Add("PH", Type.GetType("System.String"));
                dtBill.Columns.Add("YXQZ", Type.GetType("System.String"));
                dtBill.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dtBill.Columns.Add("KCSL", Type.GetType("System.String"));
                decimal bzslTotal = 0, feeTotal = 0;
                if (dtBill != null && dtBill.Rows.Count > 0)
                {
                    int rowIndex = 0;
                    foreach (DataRow row in dtBill.Rows)
                    {
                        if (row["ISGZ"].ToString() == "Y")
                        {
                            msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                            continue;
                        }

                        rowIndex++;
                        row["ROWNO"] = rowIndex;
                        row["KCSL"] = "0";
                        DataTable Temp = DbHelperOra.Query(string.Format("SELECT A.HWID,A.KCSL,A.PH,A.YXQZ,A.RQ_SC,B.ISGZ,B.GDNAME FROM DAT_GOODSSTOCK A ,DOC_GOODS B WHERE A.DEPTID ='{0}' AND A.GDSEQ = '{1}'  AND A.GDSEQ = B.GDSEQ AND A.KCSL >0 AND ROWNUM = 1 ORDER BY A.YXQZ ASC", docDEPTID.SelectedValue, row["GDSEQ"].ToString())).Tables[0];
                        if (Temp.Rows.Count > 0)
                        {
                            row["KCSL"] = Temp.Rows[0]["KCSL"];
                            row["PH"] = Temp.Rows[0]["PH"];
                            row["YXQZ"] = Temp.Rows[0]["YXQZ"];
                            row["RQ_SC"] = Temp.Rows[0]["RQ_SC"];
                        }

                        bzslTotal += Convert.ToDecimal(row["BZSL"]);
                        feeTotal += Convert.ToDecimal(row["BZSL"]) * Convert.ToDecimal(row["HSJJ"]);
                        LoadGridRow(row, false, "OLD");
                    }

                    //增加合计
                    JObject summary = new JObject();
                    summary.Add("GDNAME", "本页合计");
                    summary.Add("BZSL", bzslTotal.ToString());
                    summary.Add("HSJE", feeTotal.ToString("F2"));
                    GridGoods.SummaryData = summary;
                }

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    Alert.Show("商品" + msg + ",是高值商品,请到【高值商品使用】录入");
                    return;
                }
            }
            WindowBill.Hidden = true;
        }

        protected void GridBill_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridBill.PageIndex = e.NewPageIndex;
            DataSearch();
        }

        private void DataSearch()
        {
            int total = 0;
            string strSql = @"SELECT A.SEQNO,A.BILLNO,B.NAME FLAG_CN,A.FLAG,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,A.SUBNUM,F_GETUSERNAME(A.SLR) SLR
                                FROM DAT_CK_DOC A, SYS_CODEVALUE B 
                               WHERE A.FLAG = B.CODE AND A.FLAG = 'Y' AND B.TYPE  ='DIC_BILLSTATUS' AND A.BILLTYPE='LCD' AND A.XSTYPE='1' AND A.DEPTID='{0}' 
                            ORDER BY A.XSRQ DESC";

            //获取显示名称标记
            DataTable dtData = PubFunc.DbGetPage(GridBill.PageIndex, GridBill.PageSize, string.Format(strSql, docDEPTID.SelectedValue), ref total);
            GridBill.RecordCount = total;
            GridBill.DataSource = dtData;
            GridBill.DataBind();
        }

        protected void GridBill_Sort(object sender, GridSortEventArgs e)
        {
            GridBill.SortDirection = e.SortDirection;
            GridBill.SortField = e.SortField;
            DataSearch();
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

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            if (e.EventArgument == "GoodsAdd")
            {
                Window1_Close(null, null);
            }

            if (e.EventArgument.IndexOf("PHWindow$") >= 0)
            {
                string rowId = e.EventArgument.Split('$')[1];
                ShowPHWindow(rowId);
            }
        }
    }
}
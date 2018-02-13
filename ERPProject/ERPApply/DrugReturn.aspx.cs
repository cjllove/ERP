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
using System.Text;

namespace ERPProject.ERPApply
{
    public partial class DrugReturn : BillBase
    {
        private string strDocSql = "SELECT * FROM DAT_XS_DOC WHERE SEQNO ='{0}' AND BILLTYPE = 'XST'";
        private string strComSql = @"SELECT A.*,F_GETUNITNAME(A.UNIT) UNITNAME,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,f_getunitname(B.UNIT) UNITSMALLNAME,DECODE(NVL(A.NUM3,0),0,'非赠品','赠品') NUM3NAME 
                FROM DAT_XS_COM A,DOC_GOODS B WHERE SEQNO ='{0}' AND A.GDSEQ = B.GDSEQ  ORDER BY ROWNO";
        protected string XSTHD = "/grf/xsthd.grf";
        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docXSRQ }; }
        }

        public DrugReturn()
        {
            BillType = "XST";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //屏蔽不需要的操作按钮
                if (Request.QueryString["oper"] != null)
                {
                    if (Request.QueryString["oper"].ToString() == "input")
                    {
                        ButtonHidden(btnAudit, btnExport, btnPrint, btnCancel, btnAdt);
                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        ButtonHidden(btnExport, btnPrint, btnNew, btnDel, btnDelRow, btnGoods, btnAudit, btnCancel, btnSave, btnCommit);
                        TabStrip1.ActiveTabIndex = 0;
                    }
                    else if (Request.QueryString["oper"].ToString() == "receive")
                    {
                        ButtonHidden(btnExport, btnNew, btnDel, btnDelRow, btnGoods, btnAdt, btnSave, btnCommit);
                        TabStrip1.ActiveTabIndex = 0;
                    }
                }
                DataInit();
                billNew();
                billSearch();
            }
        }

        private void DataInit()
        {
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-7);
            lstLRRQ2.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet("DDL_USER", ddlSPR, docSHR, docLRY);

            PubFunc.DdlDataGet("DDL_SYS_DEPOT", docDEPTOUT);
            PubFunc.DdlDataGet("DDL_BILL_STATUSXST", lstTYPE, docFLAG);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");
            PubFunc.DdlDataGet(docSTR2, "DDL_RETURNREASON");
            if (Request.QueryString["oper"].ToString() == "input")
            {
                //PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
                DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
            }
            else
            {
                PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", docDEPTID, lstDEPTID);
                lstTYPE.Items.RemoveAt(1);
                docFLAG.Items.RemoveAt(1);
            }
            //获取客户化GRF文件地址  By c 2016年1月21日12:18:29 At 威海509
            string grf = Doc.DbGetGrf("XSTHD");
            if (!string.IsNullOrWhiteSpace(grf))
            {
                XSTHD = grf;
            }

        }

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
            string strDept = docDEPTID.SelectedValue;
            PubFunc.FormDataClear(FormDoc);

            docFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDept;
            if (docDEPTID.SelectedValue.Length < 1)
            {
                docDEPTID.SelectedIndex = 1;
            }
            billLockDoc(false);
            docMEMO.Enabled = true;
            docSTR2.Enabled = true;
            docDEPTID.Enabled = true;
            docDEPTOUT.Enabled = true;
            tgbSTR1.Enabled = true;
            btnGoods.Enabled = true;//x
            //清空Grid行
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            btnDel.Enabled = false;
            btnSave.Enabled = true;
            btnCommit.Enabled = false;
            btnDelRow.Enabled = true;
            btnAdt.Enabled = false;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;
            if (Request.QueryString["tp"] != null && Request.QueryString["tp"].ToString().Trim().Length > 0)
            {
                docDEPTOUT.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE=" + Request.QueryString["tp"].ToString()).ToString();
            }
            else
            {
                docDEPTOUT.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE='2'").ToString();
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", "0");
            summary.Add("HSJE", "0");
            GridGoods.SummaryData = summary;
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            string[] strCell = GridGoods.SelectedCell;
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0) return;
            if (e.ColumnID == "BZSL")
            {
                if (Doc.GetGridInf(GridGoods, e.RowID, "BZSL").Length < 1)
                {
                    Alert.Show("请填写退货数量!", "提示信息", MessageBoxIcon.Warning);
                    return;
                }
                if (Doc.GetGridInf(GridGoods, e.RowID, "HSJJ").Length < 1)
                {
                    Alert.Show("请填写含税进价!");
                    return;
                }
                if (Doc.GetGridInf(GridGoods, e.RowID, "BZHL").Length < 1)
                {
                    Alert.Show("商品信息异常，请详细检查商品信息：包装含量！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
                decimal hl = 0, rs = 0, jg = 0;
                decimal.TryParse((defaultObj["BZHL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["BZSL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["DHSL"] = rs * hl;
                if (tgbSTR1.Text.Substring(0, 2) == "DS")
                {
                    defaultObj["HSJE"] = Math.Round(rs * jg * hl, 2).ToString("F2");
                }
                else
                {
                    defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                }
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    bzslTotal += Convert.ToDecimal(dic["BZSL"]);
                    if (tgbSTR1.Text.Substring(0, 2) == "DS")
                    {
                        //feeTotal += Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["DHSL"]);
                        //定数也可以退一部分
                        feeTotal += Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["BZSL"]);
                    }
                    else
                    {
                        feeTotal += Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["BZSL"]);
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
            lstLRRQ1.SelectedDate = DateTime.Now;
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
            if (string.IsNullOrWhiteSpace(GridGoods.SelectedRowID))
            {
                Alert.Show("空单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!CheckFlag("M") && !CheckFlag("R"))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }

            GridGoods.DeleteSelectedRows();
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

            string strSql = @"SELECT A.SEQNO,A.BILLNO,B.NAME FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,A.SUBSUM,
                                     A.SUBNUM,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,F_GETUSERNAME(A.SPR) SPR,A.SPRQ,A.MEMO ,A.STR1
                                from DAT_XS_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' AND BILLTYPE='XST' AND XSTYPE='2' ";
            string strSearch = "";


            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND TRIM(UPPER(A.BILLNO))  LIKE '%{0}%'", lstBILLNO.Text.Trim().ToUpper());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (tgbXSbill.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND A.STR1  LIKE '%{0}%'", tgbXSbill.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(lstTYPE.SelectedValue))
            {
                strSearch += string.Format(" AND A.FLAG = '{0}'", lstTYPE.SelectedValue);
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.XSRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            //if (!isDg())
            //{
            //    strSearch += string.Format(" AND A.SUPID IN (SELECT SUPID FROM DOC_SUPPLIER WHERE ISDG = 'N')");
            //}
            //else
            //{
            //    strSearch += string.Format(" AND A.SUPID IN (SELECT SUPID FROM DOC_SUPPLIER WHERE ISDG = 'Y')");
            //}
            if (Request.QueryString["oper"] != null)
            {
                if (Request.QueryString["oper"].ToString() == "audit")
                {
                    //审批
                    strSearch += " AND A.FLAG <> 'M'";
                }
                else if (Request.QueryString["oper"].ToString() == "receive")
                {
                    strSearch += " AND A.FLAG <>'M' AND A.FLAG <>'N' ";
                }
            }

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
                Doc.GridRowAdd(GridGoods, dtBill);
            }
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
            PubFunc.FormLock(FormDoc, true, "");
            //按钮显示
            if (docFLAG.SelectedValue == "M" || docFLAG.SelectedValue == "R")
            {
                btnGoods.Enabled = true;//x
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnCommit.Enabled = true;
                btnDelRow.Enabled = true;
                docMEMO.Enabled = true;
                btnAdt.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = false;
            }
            else if (docFLAG.SelectedValue == "N")
            {
                btnGoods.Enabled = false;//x
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnDelRow.Enabled = false;
                btnAdt.Enabled = true;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = false;
            }
            else if (docFLAG.SelectedValue == "S")
            {
                btnGoods.Enabled = false;//x
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnDelRow.Enabled = false;
                btnAdt.Enabled = false;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnPrint.Enabled = false;
            }
            else
            {
                btnGoods.Enabled = false;//x
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnDelRow.Enabled = false;
                btnAdt.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = true;
            }
            TabStrip1.ActiveTabIndex = 1;
        }
        private bool CheckFlag(string flag)
        {
            if (docBILLNO.Text.Length > 0)
            {
                return Doc.getFlag(docBILLNO.Text, flag, BillType);
            }
            return true;
        }
        protected override void billSave()
        {
            save();
        }

        private void save(string  flagEx = "N")
        {
            #region 数据有效性验证
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!CheckFlag("M") && !CheckFlag("R"))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
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


            DataTable dt = DbHelperOra.Query(string.Format("SELECT T.GDSEQ,SUM(ABS(NVL(T.SL,0)))-SUM(ABS(NVL(T.NUM1,0))) KTSL FROM DAT_GOODSJXC T WHERE T.BILLNO='{0}' GROUP BY T.GDSEQ", tgbSTR1.Text.Trim())).Tables[0];
            Dictionary<string, string> dicKTSL = new Dictionary<string, string>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    dicKTSL.Add(row[0].ToString(), row[1].ToString());
                }
            }
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {
                    if (newDict[i]["BZSL"] == null)
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】退货数量不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(newDict[i]["BZSL"].ToString()) || newDict[i]["BZSL"].ToString() == "0")
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】退货数量不能为空或者为0！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(newDict[i]["HSJJ"].ToString()))
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】含税进价不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (newDict[i]["ISLOT"].ToString() == "2" && string.IsNullOrWhiteSpace(newDict[i]["PH"].ToString()))
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】批号不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    //if (Math.Abs(Convert.ToDecimal(newDict[i]["BZSL"].ToString())) > Math.Abs(Convert.ToDecimal(newDict[i]["NUM2"].ToString())))
                    //{
                    //    Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】退货数量大于可退货数量！！！", "消息提示", MessageBoxIcon.Warning);
                    //    return;
                    //}
                    if (Math.Abs(Convert.ToDecimal(newDict[i]["BZSL"].ToString())) > Math.Abs(Convert.ToDecimal(dicKTSL[newDict[i]["GDSEQ"].ToString()])))
                    {
                        Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】退货数量大于可退货数量！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    //dicKTSL[newDict[i]["GDSEQ"].ToString() + newDict[i]["MEMO1"].ToString()] = (Math.Abs(Convert.ToDecimal(newDict[i]["BZSL"].ToString())) - Math.Abs(Convert.ToDecimal(dicKTSL[newDict[i]["GDSEQ"].ToString() + newDict[i]["MEMO1"].ToString()]))).ToString();
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
                string flag = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_XS_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
                if (!string.IsNullOrWhiteSpace(flag) && (",M,R").IndexOf(flag) < 0)
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

            DataTable dtsave = PubFunc.GridDataGet(GridGoods);
            foreach (DataRow dr in dtsave.Rows)
            {
                if (decimal.Parse(dr["NUM2"].ToString()) < Math.Abs(decimal.Parse(dr["BZSL"].ToString())))
                {
                    Alert.Show("商品【" + dr["GDNAME"] + "】的退货数量大于可退数量！");
                    return;
                }
            }

            MyTable mtType = new MyTable("DAT_XS_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow["FLAG"] = "M";
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("XSTYPE", "2");
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_XS_COM");
            decimal subNum = 0;//总金额
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_XS_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_XS_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);

                if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["BZSL"].ToString()) || mtTypeMx.ColRow["BZSL"].ToString() == "0")
                {
                    Alert.Show("商品【" + mtTypeMx.ColRow["GDSEQ"] + " | " + mtTypeMx.ColRow["GDNAME"] + "】【退货数】为0或空，无法进行【商品退货申请】操作。");
                    return;
                }

                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);

                mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                if (tgbSTR1.Text.Substring(0, 2) == "DS")
                {
                    mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["DHSL"].ToString());
                }
                else
                {
                    mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                }
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                mtTypeMx.ColRow["XSSL"] = mtTypeMx.ColRow["DHSL"];
                //mtTypeMx.ColRow.Add("XSSL", goodsData[i]["DHSL"].ToString());
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
                if(flagEx == "N")
                    Alert.Show("商品退货信息保存成功！");
                OperLog("销售退货", "修改单据【" + docBILLNO.Text + "】");
                billLockDoc(true);
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
            //dt.Columns.Remove(dt.Columns["BZHL"]);
            string msg = "";
            string msg1 = "";
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["PIZNO"].ColumnName = "PZWH";
                //dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                //dt.Columns["UNIT_SELL_NAME"].ColumnName = "UNITNAME";
                //dt.Columns["BZHL_SELL"].ColumnName = "BZHL";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DHS", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt.Columns.Add("SPZTSL", Type.GetType("System.Int32"));
                dt.Columns.Add("KCSL", Type.GetType("System.Int32"));

                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "0";
                    row["DHSL"] = "0";
                    row["DHS"] = "0";
                    row["HSJE"] = "0";
                    row["KCSL"] = "0";
                    //查询代管库存
                    //DataTable Temp = DbHelperOra.Query(string.Format("SELECT A.KCSL,A.PH,A.YXQZ,A.RQ_SC,B.ISGZ,B.GDNAME FROM DAT_GOODSSTOCK A ,DOC_GOODS B WHERE A.DEPTID ='{0}' AND A.GDSEQ = '{1}'  AND A.GDSEQ = B.GDSEQ AND A.KCSL >0 AND ROWNUM = 1 AND A.SUPID ='00002' ORDER BY A.PICINO ASC", docDEPTID.SelectedValue, row["GDSEQ"].ToString())).Tables[0];
                    //if (Temp.Rows.Count > 0)
                    //{
                    //    row["KCSL"] = Temp.Rows[0]["KCSL"];
                    //    row["PH"] = Temp.Rows[0]["PH"];
                    //    row["YXQZ"] = Temp.Rows[0]["YXQZ"];
                    //    row["RQ_SC"] = Temp.Rows[0]["RQ_SC"];
                    //}
                    //else
                    //{
                    //    msg += row["GDNAME"] + ",";
                    //    continue;
                    //}
                    if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                    {
                        msg1 += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                        continue;
                    }
                    LoadGridRow(row, false);
                }

                //if (!string.IsNullOrWhiteSpace(msg))
                //{
                //    String strNostock = "";
                //    strNostock = string.Format("商品【{0}】在部门『{1}』中没有库存,不能进行录入！", msg, docDEPTID.SelectedText);
                //    Alert.Show(strNostock, "消息提示", MessageBoxIcon.Warning);
                //}
                if (!string.IsNullOrWhiteSpace(msg1))
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】【含税进价】为空,不能进行【商品退货审批】操作。", msg1);
                    Alert.Show(strNostock, "消息提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("系统传值错误！！！", "消息提示", MessageBoxIcon.Warning);
            }
        }

        protected void trbEditorGDSEQ_TriggerClick(object sender, EventArgs e)
        {
            string code = "";
            string dept = docDEPTID.SelectedValue;

            if (!string.IsNullOrWhiteSpace(code) && code.Trim().Length >= 2)
            {
                DataTable dt_goods = Doc.GetGoods(code, "", dept);
                if (dt_goods != null && dt_goods.Rows.Count > 0)
                {
                    dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("DHS", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("DHSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                    dt_goods.Columns.Add("SPZTSL", Type.GetType("System.Int32"));
                    DataRow dr_goods = dt_goods.Rows[0];
                    LoadGridRow(dr_goods);
                }
                else
                {
                    Alert.Show(string.Format("{0}尚未配置商品【{1}】！！！", docDEPTID.SelectedText, code), MessageBoxIcon.Warning);
                    PubFunc.GridRowAdd(GridGoods, "CLEAR");
                }
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
        protected void btnClose2_Click(object sender, EventArgs e)
        {
            winXSD.Hidden = true;
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
                Alert.Show("非新单不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (!CheckFlag("M") && !CheckFlag("R"))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }

            DbHelperOra.ExecuteSql("Delete from DAT_XS_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_XS_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_XS_EXT t WHERE T.BILLNO ='" + docBILLNO.Text.Trim() + "'");
            Alert.Show("单据【" + docBILLNO.Text + "】删除成功!");
            OperLog("销售退货", "删除单据【" + docBILLNO.Text + "】");
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
        protected override void billAudit()
        {
            if (!string.IsNullOrWhiteSpace(docSEQNO.Text))
            {



                List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();

                //判断是否已经退完
                string xsdSeqno = tgbSTR1.Text;
                string xstSeqno = docSEQNO.Text;
                string checkCountSql = @"
                    select gdseq,ph,sum(abs(sl)-nvl(num1,0))sumbzsl
                      from dat_goodsjxc  
                      where billno='{0}' 
                      group by gdseq,ph  ";
                checkCountSql = string.Format(checkCountSql,  xsdSeqno);
                DataTable checkCountDt = DbHelperOra.Query(checkCountSql).Tables[0];
                foreach (DataRow dr in checkCountDt.Rows)
                {
                    if (newDict.Where(x => x["GDSEQ"].ToString() == dr["GDSEQ"].ToString() && x["PH"].ToString() == dr["PH"].ToString()).Count() > 0)
                    {
                        Dictionary<string, object> theDict = newDict.Where(x => x["GDSEQ"].ToString() == dr["GDSEQ"].ToString() && x["PH"].ToString() == dr["PH"].ToString()).FirstOrDefault();
                        string strSUMBZSL = dr["SUMBZSL"].ToString();
                        decimal SUMBZSL = decimal.Parse(strSUMBZSL);
                        decimal thisBZSL = decimal.Parse(theDict["BZSL"].ToString());
                        if (SUMBZSL + thisBZSL < 0)
                        {
                            Alert.Show("该销售单的【" + theDict["GDSEQ"] + "】" + theDict["GDNAME"] + "已全部退完，不可再退！");
                            return;
                        }
                    }
                }


                for (int i = 0; i < newDict.Count; i++)
                {
                    string strGdseq = newDict[i]["GDSEQ"].ToString();
                    string strGdname = newDict[i]["GDNAME"].ToString();
                    if (!DbHelperOra.Exists(string.Format("select 1 from doc_goodscfg where gdseq = '{0}' and deptid = '{1}'", strGdseq, docDEPTOUT.SelectedValue)))
                    {
                        Alert.Show("第【" + (i + 1) + "】行商品【" + strGdseq + " | " + strGdname + "】在科室【" + docDEPTOUT.SelectedValue + "】未配置", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                }

                //住院办审核
                string flag = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_XS_DOC WHERE SEQNO='{0}'", docSEQNO.Text));
                if (!string.IsNullOrWhiteSpace(flag) && (",S").IndexOf(flag) < 0)
                {
                    Alert.Show("未审批或者已审核,不能审核！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                //验证是否盘点
                if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID = '" + docDEPTID.SelectedValue + "' AND FLAG='N'"))
                {
                    Alert.Show(string.Format("部门【{0}】正在盘点,请盘点后进行操作！", docDEPTID.SelectedText), "警告提示", MessageBoxIcon.Warning);
                    return;
                }
                string strBillno = docSEQNO.Text;
                
                if (BillOper(strBillno, "AUDIT") == 1)
                {
                    billLockDoc(true);
                    Alert.Show("单据【" + strBillno + "】审核成功！");
                    OperLog("销售退货", "审核单据【" + strBillno + "】");
                    billOpen(strBillno);
                    docFLAG.SelectedValue = "Y";
                }
            }
        }

        protected void btnAdt_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(docSEQNO.Text))
            {
                string flag = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_XS_DOC WHERE SEQNO='{0}'", docSEQNO.Text));
                if (!string.IsNullOrWhiteSpace(flag) && (",M,R,N").IndexOf(docFLAG.SelectedValue) < 0)
                {
                    Alert.Show("已审批或者已审核,不能审批！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                //验证是否已经退完
                string xsdSeqno = tgbSTR1.Text;
                string xstSeqno = docSEQNO.Text;
                string checkCountSql = @"
                    select gdseq,ph,sum(abs(sl)-nvl(num1,0))sumbzsl
                          from dat_goodsjxc  
                          where billno='{0}' 
                          group by gdseq,ph  ";
                checkCountSql = string.Format(checkCountSql, xsdSeqno);
                DataTable checkCountDt = DbHelperOra.Query(checkCountSql).Tables[0];
                List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
                foreach (DataRow dr in checkCountDt.Rows)
                {
                    if (newDict.Where(x => x["GDSEQ"].ToString() == dr["GDSEQ"].ToString() && x["PH"].ToString() == dr["PH"].ToString()).Count() > 0)
                    {
                        Dictionary<string, object> theDict = newDict.Where(x => x["GDSEQ"].ToString() == dr["GDSEQ"].ToString() && x["PH"].ToString() == dr["PH"].ToString()).FirstOrDefault();
                        string strSUMBZSL = dr["SUMBZSL"].ToString();
                        decimal SUMBZSL = decimal.Parse(strSUMBZSL);
                        decimal thisBZSL = decimal.Parse(theDict["BZSL"].ToString());
                        if (SUMBZSL + thisBZSL < 0)
                        {
                            Alert.Show("该销售单的【" + theDict["GDSEQ"] + "】" + theDict["GDNAME"] + "已全部退完，不可再退！");
                            return;
                        }
                    }
                }

                bool flg = true;
                if ((",M,R").IndexOf(flag) > 0)
                {
                    flg = CommitData();
                }

                if (flg)
                {
                    if (DbHelperOra.Exists("SELECT 1 FROM DAT_XS_DOC WHERE BILLTYPE = 'XST' AND SEQNO = '" + docSEQNO.Text + "'"))
                    {
                        string Sql = "UPDATE DAT_XS_DOC SET FLAG = 'S',SPR='{0}',SPRQ=sysdate WHERE BILLTYPE = 'XST' AND SEQNO = '{1}'";
                        DbHelperOra.ExecuteSql(string.Format(Sql, UserAction.UserID, docSEQNO.Text));
                        billOpen(docSEQNO.Text);
                        Alert.Show("单据【" + docSEQNO.Text + "】审批成功！");
                        OperLog("销售退货", "审批单据【" + docSEQNO.Text + "】");
                        docFLAG.SelectedValue = "S";
                    }
                    else
                    {
                        Alert.Show("请重新确认单据信息！");
                    }
                }
            }
        }

        protected override void billCancel()
        {
            //将选中单据驳回
            if ((",S").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("不是已审批单据,不能驳回", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            WindowReject.Hidden = false;
        }
        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(docSEQNO.Text.Trim()))
            {
                string flag = DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M')  FROM DAT_XS_DOC WHERE BILLNO='{0}'", docSEQNO.Text.Trim())).ToString();

                if (flag == "Y")
                {
                    Alert.Show("已审核单据,不能驳回", "操作提示", MessageBoxIcon.Warning);
                    return;
                }

                string strMemo = "";
                if (!string.IsNullOrWhiteSpace(ddlReject.SelectedValue))
                {
                    strMemo = "驳回原因：" + ddlReject.SelectedText;
                }
                else
                {
                    Alert.Show("请选择【驳回原因】");
                    return;
                }
                if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
                {
                    strMemo += ";详细说明;" + txaMemo.Text;
                }
                if (strMemo.Length > 40)
                {
                    Alert.Show("驳回备注超长！");
                    return;

                }
                List<CommandInfo> cmdList = new List<CommandInfo>();
                string sSQL = "update DAT_XS_DOC set flag='R',SHRQ=sysdate,memo ='" + strMemo + "',SHR='" + UserAction.UserID + "' where seqno ='" + docSEQNO.Text + "'";
                string sSQL1 = " DELETE  FROM DAT_XS_EXT WHERE BILLNO ='" + docSEQNO.Text + "'";
                cmdList.Add(new CommandInfo(sSQL, null));
                cmdList.Add(new CommandInfo(sSQL1, null));
                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    WindowReject.Hidden = true;
                    billSearch();
                    billOpen(docSEQNO.Text);
                    docFLAG.SelectedValue = "R";
                    OperLog("销售退货", "驳回单据【" + docSEQNO.Text + "】");
                }
            }
        }
        #region 高值退货
        protected void btnScan_Click(object sender, EventArgs e)
        {
            //越库商品不允许退货？
            //if ((",R,M").IndexOf(docFLAG.SelectedValue) < 0)
            //{
            //    Alert.Show("非『新单』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
            //    return;
            //}
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请保存单据后进行扫描追溯码操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_XS_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND SEQNO = '{0}'", docSEQNO.Text)))
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
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_XS_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.GDSEQ,A.INSTIME DESC";
            }
            else
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_XS_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.INSTIME DESC";
            }
            DataTable dtScan = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            //PubFunc.GridRowAdd(GridSacn, dtScan);
            GridSacn.DataSource = dtScan;
            GridSacn.DataBind();
            zsmScan.Text = String.Empty;
            zsmScan.Focus();
            if (docFLAG.SelectedValue == "M" || docFLAG.SelectedValue == "R")
            {
                zsmScan.Enabled = false;
                zsmDelete.Enabled = false;
            }
            else
            {
                zsmScan.Enabled = true;
                zsmDelete.Enabled = true;
            }
        }
        protected void zsmScan_TextChanged(object sender, EventArgs e)
        {
            string flag = DbHelperOra.GetSingle("SELECT FLAG FROM DAT_XS_DOC WHERE BILLNO ='" + docBILLNO.Text.Trim() + "'").ToString();
            if ((",R,M").IndexOf(docFLAG.SelectedValue) < 0)
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
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_XS_EXT WHERE ONECODE = '{0}' AND FLAG = 'Y'", zsmScan.Text)))
            {
                Alert.Show("您输入的追溯码未被使用或已退货,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }
            //写入数据库中
            string sSQL = string.Format(@"INSERT INTO DAT_XS_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,INSTIME)
                    SELECT '{0}','{1}',NVL((SELECT MAX(ROWNO)+1 FROM DAT_CK_EXT WHERE BILLNO = '{1}'),1),'{2}',GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,SYSDATE
                    FROM DAT_XS_EXT A
                    WHERE A.ONECODE = '{2}' AND ROWNO = 1", docDEPTID.SelectedValue, docBILLNO.Text, zsmScan.Text.Trim());
            DbHelperOra.ExecuteSql(sSQL);
            ScanSearch("");
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
            DbHelperOra.ExecuteSql(string.Format("DELETE FROM DAT_XS_EXT WHERE ONECODE = '{0}'", onecode));
            ScanSearch("");
        }
        #endregion
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
                if (flag == "已提交")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color2";
                }
                if (flag == "已驳回")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color3";
                }
                if (Request.QueryString["oper"].ToString() != "receive" && flag == "已审批")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color4";
                }
                if (Request.QueryString["oper"].ToString() == "receive" && flag == "已审批")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color2";
                }
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            WindowLot.Hidden = true;
        }
        private bool SaveSuccess = false;
        protected void btnCommit_Click(object sender, EventArgs e)
        {
            SaveSuccess = false;
            save("Y");
            if (SaveSuccess == false)
                return;
            SaveSuccess = false;
            CommitData();
        }

        private bool CommitData()
        {
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请保存单据后提交！", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            if (!CheckFlag("M") && !CheckFlag("R"))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return false;
            }

            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("只有保存后单据，才能提交！");
                return false;
            }
            //验证单据数量是否符合要求
            object gdseq = DbHelperOra.GetSingle(string.Format(@"SELECT A.GDSEQ
                    FROM DAT_XS_COM A,DAT_XS_COM B
                    WHERE A.SEQNO = '{0}' AND B.SEQNO = '{1}' AND A.STR1 = B.ROWNO
                    AND ABS(A.BZSL) > (B.XSSL - NVL(B.NUM1,0)) AND ROWNUM =1", docBILLNO.Text, tgbSTR1.Text));
            string msg = "";
            List<CommandInfo> cmdList = new List<CommandInfo>();

            DataTable dtCom = DbHelperOra.Query(string.Format("SELECT SUM(ABS(A.DHSL)) DHSL,A.GDSEQ,B.GDNAME FROM DAT_XS_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND SEQNO = '{0}' GROUP BY A.GDSEQ,B.GDNAME", docSEQNO.Text)).Tables[0];
            if (dtCom != null && dtCom.Rows.Count > 0)
            {
                foreach (DataRow dr in dtCom.Rows)
                {
                    string checkCount = DbHelperOra.GetSingle(string.Format("SELECT COUNT(1) FROM DAT_XS_EXT WHERE BILLNO='{0}' AND GDSEQ ='{1}'", docSEQNO.Text, dr["GDSEQ"].ToString())).ToString();
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

                string sSQL1 = "CALL P_EXE_XSD('" + docSEQNO.Text + "')";
                cmdList.Add(new CommandInfo(sSQL1, null));
            }

            string sSQL2 = string.Format("UPDATE DAT_XS_DOC SET FLAG = 'N'  WHERE BILLTYPE = 'XST' AND SEQNO = '{1}'", UserAction.UserID, docSEQNO.Text);
            cmdList.Add(new CommandInfo(sSQL2, null));

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                billOpen(docSEQNO.Text);
                Alert.Show("单据【" + docSEQNO.Text + "】提交成功！");
                OperLog("销售退货", "提交单据【" + docBILLNO.Text + "】");
                docFLAG.SelectedValue = "N";
                return true;
            }
            else
            {
                return false;
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

        protected void tgbTHDH_TriggerClick(object sender, EventArgs e)
        {
            XsdhSearch();
        }
        public void XsdhSearch(string strGdseq = "", string strPh = "")
        {
            //验证表头信息是否符合规范
            if (PubFunc.StrIsEmpty(docDEPTOUT.SelectedValue) || PubFunc.StrIsEmpty(docSTR2.SelectedValue))
            {
                Alert.Show("单据头未维护完整,请检查!!", "提示信息", MessageBoxIcon.Warning);
                return;
            }


            if (string.IsNullOrWhiteSpace(tgbSTR1.Text))
            {
                if (Request.QueryString["oper"].ToString() == "receive")
                {
                    winXSD.Hidden = true;
                }
                else if (Request.QueryString["oper"].ToString() == "audit")
                {
                    winXSD.Hidden = true;
                }
                else if (Request.QueryString["oper"].ToString() == "input")
                {
                    winXSD.Hidden = false;
                }
            }
            if (PubFunc.StrIsEmpty(tgbSTR1.Text)) return;
            if (tgbSTR1.Text.Length > 12)
            {
                //判断从GOODS表里取哪个NAME
                string sqls = "";
                if (DbHelperOra.Exists("SELECT 1 FROM SYS_PARA WHERE CODE = 'ShowName' AND VALUE = 'HIS'"))
                {
                    sqls = @"SELECT A.GDSEQ,
                           NVL(B.HISNAME,B.GDNAME) GDNAME,
                           B.GDSPEC,";

                }
                else
                {
                    sqls = @"SELECT A.GDSEQ,
                           B.GDNAME,
                           B.GDSPEC,";

                }
                sqls = sqls + @"f_getunitname(B.UNIT) UNITNAME,A.STR2 DH,
                           B.BZHL,
                           ABS(A.SL) - ABS(NVL(A.NUM1,0)) KCSL,
                           '0' SL,       
                           DECODE(A.SUPID,'00001','是','否') ISDG,
                           f_getproducername(A.SUPID) SUPNAME,
                           f_getproducername(B.PRODUCER) PRODUCERNAME,
                           A.PZWH,A.PH,A.YXQZ,A.RQ_SC,A.SEQNO,A.DEPTID,A.ROWNO
                      FROM DAT_GOODSJXC A, DOC_GOODS B, DAT_XS_COM C
                     WHERE A.GDSEQ = B.GDSEQ AND A.ROWNO=C.ROWNO AND A.BILLNO=C.SEQNO
                       AND (ABS(A.SL) - ABS(NVL(A.NUM1,0)))>0 
                       AND A.BILLNO = '{0}'";


                //同一商品同一批号合并，可退数量大于零
                //sqls = sqls + @"f_getunitname(B.UNIT) UNITNAME,
                //               B.BZHL,
                //               SUM(ABS(A.SL) - NVL(C.NUM1,0)) KCSL,
                //               '0' SL,       
                //               DECODE(NVL(C.NUM3, 0), 1, '赠品', '非赠品') NUM3NAME,
                //               DECODE(A.SUPID,'00001','是','否') ISDG,
                //               f_getproducername(A.SUPID) SUPNAME,
                //               f_getproducername(B.PRODUCER) PRODUCERNAME,
                //               A.PZWH,A.PH,A.YXQZ,A.RQ_SC,A.DEPTID,A.SUPID,B.PRODUCER
                //          FROM DAT_GOODSJXC A, DOC_GOODS B, (SELECT STR1,NUM3,ABS(SUM(XSSL)) NUM1 FROM DAT_XS_COM WHERE NVL(STR1,'#') <> '#' AND SEQNO LIKE'XST%' GROUP BY STR1,NUM3)C
                //         WHERE A.GDSEQ = B.GDSEQ
                //           AND TO_CHAR(A.SEQNO) = C.STR1(+) AND (ABS(A.SL) - NVL(C.NUM1,0))>0 
                //           AND A.BILLNO = '{0}' GROUP BY A.GDSEQ,B.GDSPEC,B.UNIT,B.BZHL,C.NUM3,A.SUPID,B.PRODUCER,A.PZWH,A.PH,A.YXQZ,A.RQ_SC,A.DEPTID,B.HISNAME,B.GDNAME";
                sqls = string.Format(sqls, tgbSTR1.Text);
                StringBuilder strSql = new StringBuilder(sqls);
                if (!string.IsNullOrWhiteSpace(trbSearch.Text))
                {
                    strSql.AppendFormat(" AND (UPPER(A.GDSEQ) LIKE '%{0}%' OR UPPER(B.GDNAME) LIKE '%{0}%' OR UPPER(B.ZJM) LIKE '%{0}%' OR B.BARCODE LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%'  OR B.HISCODE LIKE '%{0}%' OR B.HISNAME LIKE '%{0}%' OR B.STR4 LIKE '%{0}%')", trbSearch.Text.ToUpper());
                }
                DataTable dt = DbHelperOra.Query(strSql.ToString()).Tables[0];
                object obj = DbHelperOra.GetSingle(string.Format("SELECT BILLTYPE FROM DAT_XS_DOC WHERE SEQNO = '{0}'", tgbSTR1.Text));
                if (obj == null)
                {
                    Alert.Show(" 单据号不正确，无法进行销售退的操作");
                    return;
                }
                else
                {
                    if (obj.ToString() == "XSG")
                    {
                        Alert.Show("高值商品不允许进行销售退的操作");
                        return;
                    }

                }
                if (!string.IsNullOrWhiteSpace(strGdseq))
                {

                    sqls += " AND A.GDSEQ = '" + strGdseq + "'";
                }

                if (!string.IsNullOrWhiteSpace(strPh))
                {

                    sqls += string.Format(" AND A.PHID='{0}'", strPh.Trim());
                }

                //绑定数据源
                //DataTable dt = DbHelperOra.Query(string.Format(@"SELECT B.*,A.DEPTID,(ABS(B.BZSL) - NVL(B.NUM1,0)) KCSL,'0' SL,
                //           f_getunitname(B.UNIT) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,DECODE(NVL(B.NUM3,0),1,'赠品','非赠品') NUM3NAME
                //            FROM DAT_XS_DOC A,DAT_XS_COM B
                //            WHERE A.SEQNO = B.SEQNO AND B.SEQNO = '{0}' AND ABS(B.BZSL) > NVL(B.NUM1,0)", tgbSTR1.Text)).Tables[0];

                if (dt.Rows.Count < 1)
                {
                    if (!string.IsNullOrWhiteSpace(trbSearch.Text))
                    {
                        GrdRtn.DataSource = dt;
                        GrdRtn.DataBind();
                        trbSearch.Text = null;
                        return;
                    }
                    else
                    {
                        Alert.Show("单号【" + tgbSTR1.Text + "】已被退货或输入的单号不正确,请检查！", "提示信息", MessageBoxIcon.Warning);
                        return;
                    }
                }
                if (PubFunc.StrIsEmpty(docDEPTID.SelectedValue))
                {
                    docDEPTID.SelectedValue = dt.Rows[0]["DEPTID"].ToString();
                }
                else if (docDEPTID.SelectedValue != dt.Rows[0]["DEPTID"].ToString())
                {
                    Alert.Show("此单据退货科室【" + dt.Rows[0]["DEPTID"].ToString() + "】与单据头定义的科室【" + docDEPTID.SelectedValue + "】不一致，请检查！", "提示信息", MessageBoxIcon.Warning);
                    return;
                }
                GrdRtn.DataSource = dt;
                GrdRtn.DataBind();
                //增加单据显示
                Win_Rtn.Title = "退货单据信息 - " + tgbSTR1.Text;
                Win_Rtn.Hidden = false;
                PubFunc.FormLock(FormDoc, true, "");
                docMEMO.Enabled = true;
            }
        }
        protected void winSearch_Click(Object sender, EventArgs e)
        {
            string strComSql = @"SELECT A.*,F_GETUNITNAME(A.UNIT) UNITNAME,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,f_getunitname(B.UNIT) UNITSMALLNAME,DECODE(NVL(A.NUM3,0),0,'非赠品','赠品') NUM3NAME 
                FROM DAT_XS_COM A,DOC_GOODS B,DAT_XS_DOC C WHERE   A.SEQNO = C.SEQNO AND A.GDSEQ = B.GDSEQ AND C.FLAG = 'Y'AND (C.BILLTYPE='XSD'OR C.BILLTYPE='DSH') ";
            string strSearch = "";

            if (WinGOODS.Text.Length > 0)
            {
                strSearch += string.Format("  AND (UPPER(A.GDSEQ) LIKE '%{0}%' OR UPPER(A.GDNAME) LIKE '%{0}%' OR UPPER(B.ZJM) LIKE '%{0}%' OR A.BARCODE LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%'  OR B.HISCODE LIKE '%{0}%' OR B.HISNAME LIKE '%{0}%' OR A.STR4 LIKE '%{0}%')", WinGOODS.Text);
            }
            if (WinPH.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.PH LIKE '%{0}%'", WinPH.Text);
            }
            if (!string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
            {
                strSearch += string.Format(" AND C.DEPTID = '{0}'", docDEPTID.SelectedValue);
            }

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strComSql += strSearch;
            }
            GridWin.DataSource = DbHelperOra.Query(strComSql).Tables[0];
            GridWin.DataBind();
        }
        protected void WinlistRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            string strBillno = GridWin.Rows[e.RowIndex].Values[1].ToString();
            string strGdseq = GridWin.Rows[e.RowIndex].Values[2].ToString();
            string strPh = GridWin.Rows[e.RowIndex].Values[4].ToString();
            winXSD.Hidden = true;
            tgbSTR1.Text = strBillno.ToString();
            XsdhSearch(strGdseq, strPh);
        }
        protected void GodClose_Click(object sender, EventArgs e)
        {
            Win_Rtn.Hidden = true;
        }

        protected void GodSure_Click(object sender, EventArgs e)
        {
            foreach (GridRow row in GrdRtn.Rows)
            {
                int rowIndex = row.RowIndex;
                System.Web.UI.WebControls.TextBox tbxNumber = (System.Web.UI.WebControls.TextBox)GrdRtn.Rows[rowIndex].FindControl("tbxNum");
                if (!string.IsNullOrWhiteSpace(tbxNumber.Text) && tbxNumber.Text != "0")
                {

                    //校验GRIDGOODS中是否已有此商品,取唯一值  ---cjl
                    List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                    int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == row.DataKeys[0].ToString()
                    && a["PH"].ToString() == row.DataKeys[5].ToString()).Count();
                    if (sameRowCount > 0)
                    {
                        Alert.Show("商品编码为【" + row.DataKeys[0].ToString() + "】的商品同一批号已经存在，无需重复填加！");
                        return;
                    }
                    //判断从GOODS表里取哪个NAME
                    string sqls = "";
                    if (DbHelperOra.Exists("SELECT 1 FROM SYS_PARA WHERE CODE = 'ShowName' AND VALUE = 'HIS'"))
                    {
                        sqls = @"SELECT A.GDSEQ,
                           NVL(B.HISNAME,B.GDNAME) GDNAME,
                           B.GDSPEC,";

                    }
                    else
                    {
                        sqls = @"SELECT A.GDSEQ,
                           B.GDNAME,
                           B.GDSPEC,";

                    }
                    sqls = sqls + @"f_getunitname(B.UNIT) UNITNAME,A.STR2 MEMO,A.STR2 MEMO1,
                           B.BZHL,
                           ABS(A.SL) - ABS(NVL(C.NUM1,0)) KCSL,
                           '0' SL,       
                           f_getproducername(B.PRODUCER) PRODUCERNAME,A.SEQNO NUM1,
                           A.PZWH,A.PH,A.YXQZ,A.RQ_SC,A.ROWNO,A.DEPTID,C.*,'' NUM2,A.HSJJ,A.HSJE,'' BZSL,B.ISLOT,B.JXTAX,B.ISGZ,B.PIZNO PZWH,B.ZPBH,B.UNIT,B.PRODUCER
                      FROM DAT_GOODSJXC A, DOC_GOODS B, DAT_XS_COM C
                     WHERE A.GDSEQ = B.GDSEQ AND A.BILLNO = C.SEQNO AND A.ROWNO = C.ROWNO
                       AND (ABS(A.SL) - ABS(NVL(C.NUM1,0)))>0 
                       AND A.BILLNO = '{0}' AND A.SEQNO = '{1}' AND A.ROWNO='{2}'";
                    DataTable dt = DbHelperOra.Query(string.Format(sqls, tgbSTR1.Text, GrdRtn.DataKeys[rowIndex][1], GrdRtn.DataKeys[rowIndex][5])).Tables[0];
                    //DataTable dt = DbHelperOra.Query(string.Format(sqls, tgbSTR1.Text, GrdRtn.DataKeys[rowIndex][0])).Tables[0];
                    Object obj = DbHelperOra.GetSingle("SELECT DEPTOUT FROM DAT_XS_DOC A WHERE  A.SEQNO='" + tgbSTR1.Text + "'");
                    String DEPTOUT = Convert.ToString(obj ?? "0407");
                    docDEPTOUT.SelectedValue = DEPTOUT;
                    foreach (DataRow rows in dt.Rows)
                    {

                        rows["STR1"] = GrdRtn.DataKeys[rowIndex][1];
                        rows["NUM2"] = GrdRtn.DataKeys[rowIndex][2];
                        rows["BZSL"] = -Math.Abs(Convert.ToDecimal(tbxNumber.Text));
                        rows["HSJE"] = decimal.Parse(rows["HSJJ"].ToString()) * decimal.Parse(rows["BZSL"].ToString());
                        if (decimal.Parse(rows["NUM2"].ToString()) < Math.Abs(decimal.Parse(rows["BZSL"].ToString())))
                        {
                            Alert.Show("商品【" + rows["GDNAME"] + "】的退货数量大于可退数量！");
                            return;
                        }

                        //if(GrdRtn.DataKeys[rowIndex][4].ToString() == "否")
                        //{
                        rows["STR1"] = GrdRtn.DataKeys[rowIndex][1];
                        rows["NUM2"] = GrdRtn.DataKeys[rowIndex][2];
                        rows["BZSL"] = -Math.Abs(Convert.ToDecimal(tbxNumber.Text));
                        rows["HSJE"] = decimal.Parse(rows["HSJJ"].ToString()) * decimal.Parse(rows["BZSL"].ToString());
                        LoadGridRow(rows, false);
                        //}else
                        //{
                        //    Alert.Show("该商品为代管商品，无法进行退货!", "提示信息", MessageBoxIcon.Warning);
                        //}


                    }
                }
            }
            Win_Rtn.Hidden = true;
        }
        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            tgbTHDH_TriggerClick(null, null);
        }
        protected void lstBILLNO_TriggerClick(object sender, EventArgs e)
        {
            billSearch();
        }

        protected void tgbSTR1_TriggerClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(docSTR2.SelectedValue))
            {
                Alert.Show("请选择退货原因！", "异常提示", MessageBoxIcon.Warning);
                return;
            }
            dbkTime1.SelectedDate = DateTime.Now.AddDays(-1);
            dbkTime2.SelectedDate = DateTime.Now;
            WinBillno.Hidden = false;
            btnSrchBill_Click(null, null);
        }

        protected void btnSrchBill_Click(object sender, EventArgs e)
        {
            String Sql = @"SELECT DISTINCT A.SEQNO,
                                            f_getdeptname(A.DEPTID) DEPTNAME,
                                            A.SHRQ
                              FROM DAT_XS_DOC A,DAT_GOODSJXC B,DAT_XS_COM C
                             WHERE A.FLAG = 'Y' AND A.SEQNO = C.SEQNO AND B.BILLNO = C.SEQNO AND B.ROWNO = C.ROWNO
                               AND A.BILLTYPE NOT IN ('XSG', 'XST') AND (A.BILLNO LIKE '%XSD%' OR A.BILLNO LIKE '%DSH%') AND (ABS(B.SL) - ABS(NVL(B.NUM1,0)))>0
                               AND A.DEPTID = '{0}' ";
            if (tgbBillNo.Text.Trim().Length > 0)
            {
                Sql += string.Format(" AND UPPER(A.SEQNO) LIKE '%{0}%'", tgbBillNo.Text.Trim().ToUpper());
            }
            if (tgbGoods.Text.Trim().Length > 0)
            {
                Sql += string.Format(" AND A.SEQNO IN (SELECT SEQNO FROM DAT_XS_COM A, DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND (UPPER(A.GDSEQ) LIKE '%{0}%' OR UPPER(A.GDNAME) LIKE '%{0}%' OR UPPER(B.ZJM) LIKE '%{0}%'))", tgbGoods.Text.Trim().ToUpper());
            }
            Sql += string.Format(" AND A.SHRQ>=TO_DATE('{0}','YYYY-MM-DD')", dbkTime1.Text);
            Sql += string.Format(" AND A.SHRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", dbkTime2.Text);
            int total = 0;
            DataTable dtData = PubFunc.DbGetPage(GridBill.PageIndex, GridBill.PageSize, String.Format(Sql, docDEPTID.SelectedValue), ref total);
            GridBill.RecordCount = total;
            GridBill.DataSource = dtData;
            GridBill.DataBind();

        }

        protected void btnBillSure_Click(object sender, EventArgs e)
        {
            int[] selects = GridBill.SelectedRowIndexArray;
            if (selects.Count() < 1)
            {
                Alert.Show("请选择销售单号！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            String Billno = "";
            foreach (int index in selects)
            {
                Billno += GridBill.DataKeys[index][0] + ",";
            }
            tgbSTR1.Text = Billno.TrimEnd(',');
            WinBillno.Hidden = true;
            tgbTHDH_TriggerClick(null, null);
        }

        protected void GridBill_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridBill.PageIndex = e.NewPageIndex;
            btnSrchBill_Click(null, null);
        }

        protected void GridBill_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            String billNo = GridBill.DataKeys[e.RowIndex][0].ToString();
            tgbSTR1.Text = billNo;
            WinBillno.Hidden = true;
            tgbTHDH_TriggerClick(null, null);
        }
        protected void btExport_Click(object sender, EventArgs e)
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
            string strSql = @"SELECT A.SEQNO,A.BILLNO,B.NAME FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,A.SUBSUM,
                                     A.SUBNUM,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,F_GETUSERNAME(A.SPR) SPR,A.SPRQ,A.MEMO ,A.STR1
                                from DAT_XS_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' AND BILLTYPE='XST' AND XSTYPE='2' ";
            string strSearch = "";
            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND TRIM(UPPER(A.BILLNO))  LIKE '%{0}%'", lstBILLNO.Text.Trim().ToUpper());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (tgbXSbill.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND A.STR1  LIKE '%{0}%'", tgbXSbill.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(lstTYPE.SelectedValue))
            {
                strSearch += string.Format(" AND A.FLAG = '{0}'", lstTYPE.SelectedValue);
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.XSRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);
            if (Request.QueryString["oper"] != null)
            {
                if (Request.QueryString["oper"].ToString() == "audit")
                {
                    //审批
                    strSearch += " AND A.FLAG <> 'M'";
                }
                else if (Request.QueryString["oper"].ToString() == "receive")
                {
                    strSearch += " AND A.FLAG <>'M' AND A.FLAG <>'N' ";
                }
            }
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC";
            DataTable dtData = DbHelperOra.Query(strSql).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            XTBase.Utilities.ExcelHelper.ExportByWeb(DbHelperOra.Query(strExpSql()).Tables[0], "商品退货报表", string.Format("商品退货报表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
        private string strExpSql()
        {
            string strExpSql = @"SELECT A.BILLNO 单据编号,B.NAME 单据状态,F_GETDEPTNAME(A.DEPTID) 使用科室,A.XSRQ 使用日期,
                                     A.SUBNUM 明细条数,A.SUBSUM 金额合计,F_GETUSERNAME(A.LRY) 录入员,A.LRRQ 录入日期,F_GETUSERNAME(A.SPR) 审批员,A.SPRQ 审批日期,F_GETUSERNAME(A.SHR) 审核员,A.SHRQ 审核日期,A.STR1 销售单号,A.MEMO 备注
                                from DAT_XS_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' AND BILLTYPE='XST' AND XSTYPE='2' ";
            string strSearch = "";
            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND TRIM(UPPER(A.BILLNO))  LIKE '%{0}%'", lstBILLNO.Text.Trim().ToUpper());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (tgbXSbill.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND A.STR1  LIKE '%{0}%'", tgbXSbill.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(lstTYPE.SelectedValue))
            {
                strSearch += string.Format(" AND A.FLAG = '{0}'", lstTYPE.SelectedValue);
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.XSRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);
            if (Request.QueryString["oper"] != null)
            {
                if (Request.QueryString["oper"].ToString() == "audit")
                {
                    //审批
                    strSearch += " AND A.FLAG <> 'M'";
                }
                else if (Request.QueryString["oper"].ToString() == "receive")
                {
                    strSearch += " AND A.FLAG <>'M' AND A.FLAG <>'N' ";
                }
            }
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strExpSql += strSearch;
            }
            strExpSql += " ORDER BY A.BILLNO DESC";
            return strExpSql;
        }
    }
}
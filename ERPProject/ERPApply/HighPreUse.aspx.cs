﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using XTBase.Utilities;
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
    public partial class HighPreUse : BillBase
    {
        private string strDocSql = "SELECT A.* FROM DAT_CK_DOC A WHERE A.SEQNO ='{0}' AND A.XSTYPE = 'G'";
        private string strComSql = @"SELECT A.*,
                                       F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                       f_getsupname(A.SUPID) SUPNAME,
                                       f_getunitname(A.UNIT) UNITNAME,
                                       f_getunitname(B.UNIT) UNITSMALLNAME
                                  FROM DAT_CK_COM A, DOC_GOODS B
                                 WHERE A.SEQNO = '{0}'
                                   AND A.GDSEQ = B.GDSEQ
                                 ORDER BY A.ROWNO";
        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docDEPTID, docXSRQ, docSLR }; }
        }

        public HighPreUse()
        {
            BillType = "YKD";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                #region 读取客户标记，更改客户化需求
                string USERXMID = "";
                USERXMID = (DbHelperOra.GetSingle("select VALUE from sys_para WHERE CODE='USERXMID'")).ToString();
                if (USERXMID == "TJ_YKGZ") TJ_YKGZ();
                #endregion
                billNew();
            }
        }
        private void DataInit()
        {
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            dphSTR9.SelectedDate = DateTime.Now;

            PubFunc.DdlDataGet("DDL_USER", docSHR, docLRY, docSLR);
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, ddlDEPTIN);
            PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", lstDEPTID, docDEPTID);
            PubFunc.DdlDataGet(docFLAG, "DDL_BILL_STATUS");
        }

        protected override void billNew()
        {
            //原单据保存判断
            //string strDept = docDEPTID.SelectedValue;
            PubFunc.FormDataClear(FormDoc);

            docFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            //docSHR.SelectedValue = UserAction.UserID;
            docSLR.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            dphSTR9.SelectedDate = DateTime.Now;
            //docSHRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = "";

            billLockDoc(false);
            btnGoods.Enabled = true;
            docMEMO.Enabled = true;
            btnSave.Enabled = true;
            btnAudit.Enabled = false;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            docSTR1.Enabled = true;
            btnDel.Enabled = false;

            docNUM2.Enabled = true;
            docNUM3.Enabled = true;
            tbxSTR5.Enabled = true;
            tbxSTR6.Enabled = true;
            docNUM4.Enabled = true;
            tbxSTR7.Enabled = true;
            tbxSTR8.Enabled = true;
            dphSTR9.Enabled = true;
            //清空Grid行
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            //不需处理
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
                //defaultObj["DHSL"] = rs * hl;
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
                //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    if ((dic["BZSL"] ?? "").ToString().Length > 0)
                    {
                        bzslTotal += Convert.ToDecimal(dic["BZSL"]);
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
            dphSTR9.SelectedDate = DateTime.Now;
            hfdSave.Text = "";
        }

        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据");
                return;
            }
            if (docFLAG.SelectedValue != "N" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非【新单】不能删除!");
                return;
            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo("Delete from DAT_CK_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'", null));
            cmdList.Add(new CommandInfo("Delete from DAT_CK_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'", null));
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("单据【" + docBILLNO.Text + "】删除成功!");
                OperLog("跟台使用", "删除单据【" + docBILLNO.Text + "】");
                billSearch();
                billNew();
            }
            else
            {
                Alert.Show("单据删除失败!", "错误提示", MessageBoxIcon.Information);
            }
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
        }

        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            if (string.IsNullOrWhiteSpace(docSTR1.Text))
            {
                Alert.Show("【预入库单号】为空，请输入！");
                return;
            }
            docDEPTID.Enabled = false;
            docXSRQ.Enabled = false;
            docBILLNO.Enabled = false;
            docSTR1.Enabled = false;
            WindowGoods.Hidden = false;
            DataSearch();
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
                Alert.Show("[开始日期]大于[结束日期]，请重新输入！", "提示信息", MessageBoxIcon.Warning);
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO,A.FLAG,B.NAME FLAGNAME,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,
                                     A.SUBNUM,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO,A.SUBSUM
                                from DAT_CK_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' AND BILLTYPE='YKD' AND XSTYPE='G' ";
            string strSearch = "";

            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND  TRIM(UPPER(A.BILLNO))  LIKE '%{0}%'", lstBILLNO.Text.Trim().ToUpper());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
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
        private bool SaveSuccess = false;
        protected override void billAudit()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非新单不能审核！");
                return;
            }
            //验证科室是否盘点
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID ='" + docDEPTID.SelectedValue + "' AND FLAG='N'"))
            {
                Alert.Show("科室正在盘点,请检查!");
                return;
            }
            string strBillno = docSEQNO.Text;
            //save("Y");
            hfdSave.Text = "S";
            
                //if (BillOper(strBillno, "YRKAUT") == 1)
                //{
                    if (BillOper(strBillno, "YRKAUT") == 1)
                    {
                        billLockDoc(true);
                        Alert.Show("单据【" + strBillno + "】审核成功！");
                        OperLog("跟台使用", "审核单据【" + strBillno + "】");
                        hfdSave.Text = "";
                        billOpen(strBillno);
                    }
                //}
            
            //SaveSuccess = true;
            ////save("Y");
            //if (SaveSuccess == false)
            //    return;
            //SaveSuccess = false;
            
            
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
                    LoadGridRow(row, false, "OLD");
                    bzslTotal += Convert.ToDecimal(row["BZSL"]);
                    feeTotal += Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZSL"]);
                }
            }
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
            PubFunc.FormLock(FormDoc, true, "");
            TabStrip1.ActiveTabIndex = 1;
            if (docFLAG.SelectedValue == "N" || docFLAG.SelectedValue == "R")
            {
                btnSave.Enabled = true;
                btnAudit.Enabled = true;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                btnDel.Enabled = true;
                btnGoods.Enabled = true;
                btnDelRow.Enabled = true;
            }
            else
            {
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnPrint.Enabled = true;
                btnDelRow.Enabled = false;
                btnDel.Enabled = false;
                btnGoods.Enabled = false;
                btnDelRow.Enabled = false;
            }
        }
        protected override void billCancel()
        {
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请选择需要操作的单据！", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue == "N")
            {
                if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_CK_DOC SET FLAG = 'R' WHERE SEQNO = '{0}' AND FLAG = 'N'", docSEQNO.Text)) > 0)
                {
                    Alert.Show("单据【" + docSEQNO.Text + "】驳回成功！");
                    billOpen(docSEQNO.Text);
                    return;
                }
                else
                {
                    Alert.Show("请刷新界面后重试！", "操作提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("单据状态不正确，请检查！", "操作提示", MessageBoxIcon.Warning);
                return;
            }
        }

        protected override void billSave()
        {
            save();

        }

        private void save(string flag = "N")
        {
            #region 数据有效性验证
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非[新单]不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            string msg = "";
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {
                    if (int.Parse(newDict[i]["BZSL"].ToString()) > int.Parse(newDict[i]["DHSL"].ToString()))
                    {
                        Alert.Show("第" + (i + 1) + "行商品【" + newDict[i]["GDNAME"].ToString() + "】【使用数】不允许大于【预入库数量】", "提示信息", MessageBoxIcon.Warning);
                        return;
                    }
                    if (int.Parse(newDict[i]["BZSL"].ToString()) != int.Parse(newDict[i]["DHSL"].ToString()))
                    {
                        msg += "第" + (i + 1) + "行商品【" + newDict[i]["GDNAME"].ToString() + "】，";
                    }
                    if ((newDict[i]["STR1"] ?? "").ToString().Length > 0 && (newDict[i]["BZSL"] ?? "").ToString() != "1")
                    {
                        Alert.Show("第" + (i + 1) + "行商品【" + newDict[i]["GDNAME"].ToString() + "】【使用数】不允许大于【预入库数量】", "提示信息", MessageBoxIcon.Warning);
                        return;
                    }

                }
            }

            #endregion


            if (!string.IsNullOrWhiteSpace(msg))
            {
                msg += "【使用数】不等于【预入库数量】，确认保存，请选择【确定】";
                PageContext.RegisterStartupScript(Confirm.GetShowReference(msg, "消息提示", MessageBoxIcon.Information, PageManager1.GetCustomEventReference("btnOk"), PageManager1.GetCustomEventReference("btnCancel")));
            }
            else
            {
                btnOk();
            }
            SaveSuccess = true;
        }

        protected bool btnOk()
        {
            #region 数据有效性验证
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非[新单]不能保存！", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return false;
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            string msg = "";
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {
                    if (int.Parse(newDict[i]["BZSL"].ToString()) > int.Parse(newDict[i]["DHSL"].ToString()))
                    {
                        Alert.Show("第" + (i + 1) + "行商品【" + newDict[i]["GDNAME"].ToString() + "】【使用数】不允许大于【预入库数量】", "提示信息", MessageBoxIcon.Warning);
                        return false;
                    }
                    if (int.Parse(newDict[i]["BZSL"].ToString()) != int.Parse(newDict[i]["DHSL"].ToString()))
                    {
                        msg += "第" + (i + 1) + "行商品【" + newDict[i]["GDNAME"].ToString() + "】，";
                    }
                    if ((newDict[i]["STR1"] ?? "").ToString().Length > 0 && (newDict[i]["BZSL"] ?? "").ToString() != "1")
                    {
                        Alert.Show("第" + (i + 1) + "行商品【" + newDict[i]["GDNAME"].ToString() + "】【使用数】不允许大于【预入库数量】", "提示信息", MessageBoxIcon.Warning);
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

            if (PubFunc.StrIsEmpty(docBILLNO.Text))
            {
                docSEQNO.Text = BillSeqGet();
                docBILLNO.Text = docSEQNO.Text;
                docBILLNO.Enabled = false;
            }
            //else
            //{
            //    string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'N') FROM DAT_CK_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
            //    if (!string.IsNullOrWhiteSpace(flg) && (",N,R").IndexOf(flg) < 0)
            //    {
            //        Alert.Show("您输入的单据号存在重复信息，请重新输入或置空！", "消息提示", MessageBoxIcon.Warning);
            //        return false;
            //    }
            //    else
            //    {
            //        docSEQNO.Text = docBILLNO.Text;
            //        docBILLNO.Enabled = false;
            //    }
            //}
            MyTable mtType = new MyTable("DAT_CK_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("XSTYPE", "G");
            mtType.ColRow["FLAG"] = "N";
            string Deptout = (DbHelperOra.GetSingle(string.Format("SELECT DEPTID FROM DAT_YRK_DOC WHERE SEQNO IN({0})", Billno())) ?? "").ToString();
            if (PubFunc.StrIsEmpty(Deptout))
            {
                Alert.Show("您输入的预入库单号有误,请检查！", "提示信息", MessageBoxIcon.Warning);
                return false;
            }
            mtType.ColRow["DEPTOUT"] = Deptout;
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_CK_COM");
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_CK_DOC where seqno='" + docBILLNO.Text + "' AND FLAG = 'N'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_CK_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            Decimal JE = 0;
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);
                if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["BZSL"].ToString()) || mtTypeMx.ColRow["BZSL"].ToString() == "0")
                {
                    Alert.Show("商品【" + mtTypeMx.ColRow["GDSEQ"] + " | " + mtTypeMx.ColRow["GDNAME"] + "】【使用数】为0或空，无法进行【跟台商品使用】操作。");
                    return false;
                }

                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow.Add("XSSL", decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()));
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["XSSL"].ToString());
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow["ISGZ"] = "Y";
                mtTypeMx.ColRow.Add("BHSJE", 0);
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                cmdList.Add(mtTypeMx.Insert());
                JE += Convert.ToDecimal(mtTypeMx.ColRow["HSJE"]);
            }
            mtType.ColRow.Add("SUBSUM", JE);
            cmdList.AddRange(mtType.InsertCommand());

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                if (hfdSave.Text != "S")
                    Alert.Show("跟台商品信息保存成功！");
                OperLog("跟台使用", "修改单据【" + docBILLNO.Text + "】");
                billOpen(docSEQNO.Text);
                return true;
            }
            else return false;
        }
        protected override void billExport()
        {
            if (string.IsNullOrWhiteSpace(docSEQNO.Text))
            {
                Alert.Show("请先选择要导出的订单信息！");
                return;
            }
            string sql = @"SELECT A.BILLNO 单据编号,
                               F_GETDEPTNAME(A.DEPTID) 使用科室,
                               F_GETUSERNAME(A.LRY) 录入员,
                               TO_CHAR(A.XSRQ, 'YYYY-MM-DD') 录入日期,
                               A.MEMO 备注说明,
                               ''''||B.GDSEQ 商品编码,
                               B.ROWNO 行号,       
                               B.GDNAME 商品名称,
                               B.GDSPEC 商品规格,
                               B.BZSL 包装数量,
                               F_GETUNITNAME(B.UNIT) 包装单位,
                               B.HSJJ 含税进价,
                               B.HSJE 含税金额,
                               B.ZPBH 制品编号,
                               F_GETPRODUCERNAME(B.PRODUCER) 生产厂家,
                               B.PHID 批号,
                               B.YXQZ 有效期至,
                               B.PZWH 注册证号,
                               B.RQ_SC 生产日期,
                               B.MEMO 备注
                          FROM DAT_CK_DOC A, DAT_CK_COM B, DOC_GOODS G
                         WHERE B.SEQNO = '{0}'
                           AND B.GDSEQ = G.GDSEQ
                           AND A.SEQNO = B.SEQNO
                         ORDER BY ROWNO";
            DataTable dt = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            ExcelHelper.ExportByWeb(dt, string.Format("【{0}】跟台商品使用信息", docDEPTID.SelectedText), "跟台商品使用信息导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
        }

        private void LoadGridRow(DataRow row, bool firstRow = true, string flag = "NEW")
        {
            PubFunc.GridRowAdd(GridGoods, row, firstRow);
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            //dt.Columns.Remove(dt.Columns["BZHL"]);
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
                dt.Columns.Add("STR2", Type.GetType("System.String"));
                string msg = "";
                string msg1 = "";
                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "1";
                    row["DHSL"] = "1";
                    row["KCSL"] = "0";
                    row["HSJE"] = Convert.ToDecimal(row["BZSL"]) * Convert.ToDecimal(row["BZHL"]) * Convert.ToDecimal(row["HSJJ"]);
                    ////越库商品不进行库存、批号带入
                    //if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DOC_GOODS WHERE GDSEQ = '{0}' AND ISFLAG3 = 'N'", row["GDSEQ"].ToString())))
                    //{
                    DataTable dtPH = Doc.GetGoodsPHKC(row["GDSEQ"].ToString(), docDEPTID.SelectedValue);
                    if (dtPH != null && dtPH.Rows.Count > 0)
                    {
                        if (dtPH.Rows[0]["KCSL"].ToString() == "0")
                        {
                            msg += row["GDNAME"] + ",";
                            continue;
                        }
                        row["PH"] = dtPH.Rows[0]["PH"];
                        row["PZWH"] = dtPH.Rows[0]["PZWH"];
                        row["RQ_SC"] = dtPH.Rows[0]["RQ_SC"];
                        row["YXQZ"] = dtPH.Rows[0]["YXQZ"];
                        row["KCSL"] = dtPH.Rows[0]["KCSL"];
                        //row["STR2"] = (dtPH.Rows[0]["ISFLAG3"] ?? "");
                    }
                    else
                    {
                        msg += row["GDNAME"] + ",";
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                    {
                        msg1 += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                        Alert.Show("商品" + msg1 + "【含税进价】为空，不能进行【跟台商品使用】操作。", "消息提示", MessageBoxIcon.Warning);
                        continue;
                    }
                    row["HSJJ"] = Math.Round(Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZHL"]), 4);
                    LoadGridRow(row, false);
                }
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    Alert.Show(string.Format("商品【{0}】在部门『{1}』中没有库存,不能进行录入！", msg, docDEPTID.SelectedText), "消息提示", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("请选择商品信息！", "消息提示", MessageBoxIcon.Warning);
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

        #region 高值条码处理
        protected void btnScan_Click(object sender, EventArgs e)
        {
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请选择需要处理追溯码的单据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists(string.Format(@"SELECT 1 FROM DAT_CK_EXT WHERE BILLNO = '{0}'
                    UNION ALL
                    SELECT 1 FROM DAT_CK_COM A,DOC_GOODS B WHERE A.GDSEQ =B.GDSEQ AND B.ISGZ = 'Y' AND A.SEQNO = '{0}'", docSEQNO.Text)))
            {
                Alert.Show("此单据中没有已经保存的跟台商品,请检查！", "消息提示", MessageBoxIcon.Warning);
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
            //PubFunc.GridRowAdd(GridSacn, dtScan);
            GridSacn.DataSource = dtScan;
            GridSacn.DataBind();
            //对状态进行判断
            if (docFLAG.SelectedValue == "Y")
            {
                zsmScan.Enabled = false;
                zsmDelete.Enabled = false;
            }
            else
            {
                zsmScan.Enabled = true;
                zsmDelete.Enabled = true;
            }
            zsmScan.Text = String.Empty;
            zsmScan.Focus();
        }
        protected void zsmScan_TextChanged(object sender, EventArgs e)
        {
            string flag = DbHelperOra.GetSingle("SELECT FLAG FROM DAT_CK_DOC WHERE BILLNO ='" + docBILLNO.Text.Trim() + "'").ToString();
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0)
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
            string sSQL = string.Format(@"INSERT INTO DAT_XS_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,INSTIME,PH,RQ_SC,YXQZ,STR1)
                    SELECT '{0}','{1}',NVL((SELECT MAX(ROWNO)+1 FROM DAT_CK_EXT WHERE BILLNO = '{1}'),1),'{2}',GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,DEPTCUR,BZHL,SYSDATE,PH,RQ_SC,YXQZ,STR1
                    FROM DAT_CK_EXT A
                    WHERE A.ONECODE = '{2}' AND ROWNO = 1", docDEPTID.SelectedValue, docBILLNO.Text, zsmScan.Text.Trim());
            DbHelperOra.ExecuteSql(sSQL);
            ScanSearch("");
        }

        protected void zsmDelete_Click(object sender, EventArgs e)
        {
            if ((",M").IndexOf(docFLAG.SelectedValue) < 0)
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
                FineUIPro.BoundField flagcol = GridList.FindColumn("FLAGNAME") as FineUIPro.BoundField;
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
        protected String Billno()
        {
            if (docSTR1.Text.Length < 1) return "";
            string[] billnoAll = docSTR1.Text.Split(',');
            string billno = "";
            foreach (string bill in billnoAll)
            {
                billno += "'" + bill + "',";
            }
            return billno.TrimEnd(',');
        }
        private void DataSearch()
        {
            int total = 0;
            string strSearch = "";
            string sql = "";
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_YRK_EXT WHERE BILLNO IN(" + Billno() + ") AND NVL(STR1,'#') <> '#'"))
            {
                sql = String.Format(@"SELECT A.billno seqno,C.GDSEQ,C.GDNAME,C.GDSPEC,C.BZHL,C.UNIT,f_getunitname(C.UNIT) UNITNAME,1 SSSL,C.HSJJ,A.PH,
                        A.RQ_SC,A.YXQZ,C.PIZNO PZWH,C.PRODUCER,f_getproducername(C.PRODUCER) PRODUCERNAME,A.STR1,C.JXTAX,C.ISGZ
                    FROM DAT_YRK_EXT A,DOC_GOODS C
                    WHERE A.GDSEQ = C.GDSEQ AND A.BILLNO IN ({0}) AND NVL(A.STR1,'#') <>'#' AND C.FLAG IN('Y','T')
                    UNION ALL
                    SELECT A.billno,C.GDSEQ,C.GDNAME,C.GDSPEC,C.BZHL,C.UNIT,f_getunitname(C.UNIT) UNITNAME,COUNT(1) SSSL,C.HSJJ,A.PH,A.RQ_SC,A.YXQZ,C.PIZNO PZWH,C.PRODUCER,f_getproducername(C.PRODUCER) PRODUCERNAME,A.STR1,C.JXTAX,C.ISGZ
                    FROM DAT_YRK_EXT A,DOC_GOODS C
                    WHERE A.GDSEQ = C.GDSEQ AND A.BILLNO IN ({0}) AND NVL(A.STR1,'#') ='#' AND C.FLAG IN('Y','T')
                    GROUP BY C.GDSEQ,C.GDNAME,C.GDSPEC,C.BZHL,C.UNIT,C.HSJJ,A.PH,A.RQ_SC,A.YXQZ,C.PIZNO,C.PRODUCER,A.STR1,C.JXTAX,A.billno", Billno());
            }
            else
            {
                sql = string.Format(@"SELECT A.SEQNO,
                                        F_GETUNITNAME(C.UNIT) UNITNAME,
                                        F_GETPRODUCERNAME(C.PRODUCER) PRODUCERNAME,
                                        F_GETSUPNAME(b.supid) SUPNAME,
                                        F_GETSUPNAME(C.SUPPLIER) SUPPLIERNAME,B.HSJE,
                                        C.GDSEQ,C.GDNAME,C.GDSPEC,C.BZHL,C.HSJJ,C.ZPBH,'{1}' HWID,C.JXTAX,C.PIZNO PZWH,C.PRODUCER,b.supid,C.UNIT,B.SSSL,B.PH,B.RQ_SC,B.YXQZ,
                                        NVL(D.ISJF,'Y') ISJF,B.ISGZ
                            FROM DAT_YRK_DOC A, DAT_YRK_COM B , DOC_GOODS C,DOC_GOODSCFG D
                            WHERE A.SEQNO = B.SEQNO
                            AND B.GDSEQ = C.GDSEQ
                            AND D.GDSEQ=B.GDSEQ
                            AND A.DEPTID=D.DEPTID
                            AND A.SEQNO IN({0}) AND A.FLAG = 'Y' AND C.FLAG IN('Y','T')
                            AND EXISTS(SELECT 1 FROM DOC_GOODSCFG WHERE DEPTID = '{1}' AND GDSEQ = B.GDSEQ)", Billno(), docDEPTID.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(trbSearch.Text))
            {
                strSearch = string.Format(" AND (C.GDSEQ LIKE '%{0}%' OR C.GDNAME LIKE '%{0}%' OR C.ZJM LIKE '%{0}%' OR C.BARCODE LIKE '%{0}%')", trbSearch.Text.ToUpper());
            }
            //strSearch += " ORDER BY B.ROWNO";
            sql += strSearch;

            StringBuilder strSql = new StringBuilder(sql);
            DataTable dtData = PubFunc.DbGetPage(GoodsInfo.PageIndex, GoodsInfo.PageSize, strSql.ToString(), ref total);
            GoodsInfo.RecordCount = total;
            GoodsInfo.DataSource = dtData;
            GoodsInfo.DataBind();
        }

        protected void GoodsInfo_PageIndexChange(object sender, GridPageEventArgs e)
        {

        }

        protected void GoodsInfo_Sort(object sender, GridSortEventArgs e)
        {

        }

        protected void GoodsInfo_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string strCode = GoodsInfo.Rows[e.RowIndex].Values[1].ToString();
            DataTable table = PubFunc.GridDataGet(GoodsInfo);
            DataView dv = table.DefaultView;
            dv.RowFilter = "GDSEQ = '" + strCode + "'";
            DataGridBack(dv.ToTable());
        }

        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            DataSearch();
        }

        private void DataGridBack(DataTable dt)
        {
            dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
            if (dt != null && dt.Rows.Count > 0)
            {
                string someDjbh = string.Empty;
                bool getDjbh = false;

                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("STR2", Type.GetType("System.String"));
                dt.Columns.Add("STR3", Type.GetType("System.String"));
                foreach (DataRow row in dt.Rows)
                {

                    decimal jingdu = 0;
                    decimal bzhl = 0;
                    if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                    row["DHSL"] = row["SSSL"];
                    row["BZSL"] = "1";
                    row["HSJE"] = row["HSJJ"];
                    row["STR3"] = row["SEQNO"];
                    if ((row["STR1"] ?? "").ToString().Length > 0)
                    {
                        row["STR2"] = row["STR1"];
                    }
                    docMEMO.Enabled = true;
                    List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                    int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == row["GDSEQ"].ToString() && (a["STR1"] ?? "").ToString() == (row["STR1"] ?? "").ToString()).Count();
                    if (sameRowCount > 0)
                    {
                        someDjbh += "【" + row["GDNAME"].ToString() + "】";
                        getDjbh = true;
                    }
                    else
                    {
                        PubFunc.GridRowAdd(GridGoods, row, false);
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
            WindowGoods.Hidden = true;
        }
        protected void btnSure_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            if (GoodsInfo.SelectedRowIndexArray.Length == 0)
            {
                Alert.Show("请选择商品信息！", "警告提醒", MessageBoxIcon.Warning);
                return;
            }
            FineUIPro.GridRowCollection rows = new FineUIPro.GridRowCollection();
            for (int i = GoodsInfo.SelectedRowIndexArray.Length - 1; i > -1; i--)
            {
                rows.Add(GoodsInfo.Rows[GoodsInfo.SelectedRowIndexArray[i]]);
            }
            dt = GridDataGet(GoodsInfo, rows);

            DataGridBack(dt);
        }
        public static DataTable GridDataGet(FineUIPro.Grid grid, FineUIPro.GridRowCollection rows)
        {
            int rowCount = 0;
            DataTable dtGrid = new DataTable();
            foreach (GridColumn gc in grid.Columns)
            {
                if (gc is FineUIPro.BoundField)
                {
                    dtGrid.Columns.Add(new DataColumn(((FineUIPro.BoundField)(gc)).DataField.ToUpper(), typeof(string)));
                }
                else if (gc is FineUIPro.RenderField)
                {
                    dtGrid.Columns.Add(new DataColumn(((FineUIPro.RenderField)(gc)).DataField.ToUpper(), typeof(string)));
                }
                else if (gc is FineUIPro.TemplateField)
                {
                    dtGrid.Columns.Add(new DataColumn(((FineUIPro.TemplateField)(gc)).ColumnID.ToUpper(), typeof(string)));
                }
                rowCount++;
            }
            if (rowCount > 0)
            {
                foreach (GridRow gr in rows)
                {
                    DataRow row = dtGrid.NewRow();
                    foreach (GridColumn gc in grid.Columns)
                    {
                        if (gc is FineUIPro.BoundField)
                        {
                            row[((FineUIPro.BoundField)(gc)).DataField.ToUpper()] = gr.Values[gc.ColumnIndex];
                        }
                        if (gc is FineUIPro.RenderField)
                        {
                            row[((FineUIPro.RenderField)(gc)).DataField.ToUpper()] = gr.Values[gc.ColumnIndex];
                        }
                        else if (gc is FineUIPro.TemplateField)
                        {
                            row[((FineUIPro.TemplateField)(gc)).ColumnID.ToUpper()] = gr.Values[gc.ColumnIndex];
                        }
                    }
                    dtGrid.Rows.Add(row);
                }
            }
            return dtGrid;
        }

        protected void btnClose1_Click(object sender, EventArgs e)
        {
            WindowGoods.Hidden = true;
        }

        protected void docSTR1_TriggerClick(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue != "M")
            {
                return;
            }
            if (docDEPTID.SelectedValue.Length < 1)
            {
                Alert.Show("请输入【使用科室】信息！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            dbkTime1.SelectedDate = DateTime.Now.AddDays(-1);
            dbkTime2.SelectedDate = DateTime.Now;
            WinBillno.Hidden = false;
            btnSrchBill_Click(null, null);

        }

        protected void btnSrchBill_Click(object sender, EventArgs e)
        {
            if(docSTR1.Text.Length>0)
            {
                String Sql = @"SELECT DISTINCT A.SEQNO,f_getdeptname(A.DEPTID) DEPTNAME,f_getsupname(A.PSSID) PSSNAME,A.SHRQ
                            FROM DAT_YRK_DOC A,DAT_YRK_COM B,DOC_GOODSCFG C
                            WHERE A.SEQNO = B.SEQNO AND B.GDSEQ = C.GDSEQ AND A.FLAG = 'Y'AND A.SEQNO='{3}'
                            AND C.DEPTID = '{0}' AND A.SHRQ BETWEEN TO_DATE('{1}','YYYY-MM-DD') AND TO_DATE('{2}','YYYY-MM-DD') + 1";
                if (ddlDEPTIN.SelectedValue.Length > 0)
                {
                    Sql += "AND A.DEPTID = '" + ddlDEPTIN.SelectedValue + "'";
                }
                int total = 0;
                DataTable dtData = PubFunc.DbGetPage(GridBill.PageIndex, GridBill.PageSize, String.Format(Sql, docDEPTID.SelectedValue, dbkTime1.Text, dbkTime2.Text, docSTR1.Text), ref total);
                GridBill.RecordCount = total;
                GridBill.DataSource = dtData;
                GridBill.DataBind();
            }
            else
            {
                String Sql = @"SELECT DISTINCT A.SEQNO,f_getdeptname(A.DEPTID) DEPTNAME,f_getsupname(A.PSSID) PSSNAME,A.SHRQ
                            FROM DAT_YRK_DOC A,DAT_YRK_COM B,DOC_GOODSCFG C
                            WHERE A.SEQNO = B.SEQNO AND B.GDSEQ = C.GDSEQ AND A.FLAG = 'Y'
                            AND C.DEPTID = '{0}' AND A.SHRQ BETWEEN TO_DATE('{1}','YYYY-MM-DD') AND TO_DATE('{2}','YYYY-MM-DD') + 1";
                if (ddlDEPTIN.SelectedValue.Length > 0)
                {
                    Sql += " AND A.DEPTID = '" + ddlDEPTIN.SelectedValue + "'";
                }
                int total = 0;
                DataTable dtData = PubFunc.DbGetPage(GridBill.PageIndex, GridBill.PageSize, String.Format(Sql, docDEPTID.SelectedValue, dbkTime1.Text, dbkTime2.Text), ref total);
                GridBill.RecordCount = total;
                GridBill.DataSource = dtData;
                GridBill.DataBind();
            }
        }

        protected void GridBill_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            String billNo = GridBill.DataKeys[e.RowIndex][0].ToString();
            docSTR1.Text = billNo;
            WinBillno.Hidden = true;
            billGoods();
        }

        protected void GridBill_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridBill.PageIndex = e.NewPageIndex;
            btnSrchBill_Click(null, null);
        }

        protected void btnBillSure_Click(object sender, EventArgs e)
        {
            int[] selects = GridBill.SelectedRowIndexArray;
            if (selects.Count() < 1)
            {
                Alert.Show("请选择预入库单号！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            String Billno = "";
            foreach (int index in selects)
            {
                Billno += GridBill.DataKeys[index][0] + ",";
            }
            docSTR1.Text = Billno.TrimEnd(',');
            WinBillno.Hidden = true;
            billGoods();
        }
        private void TJ_YKGZ()
        {//天津医科大学总医院高值 客户化需求 2016年6月20日 17:16:25
            tbxSTR6.Label = "性  别";
            tbxSTR7.Label = "年  龄";
            tbxSTR8.Label = "身份证号";
            tbxSTR5.Required = true; 
            tbxSTR5.ShowRedStar = true;
            docNUM2.Required = true; 
            docNUM2.ShowRedStar = true;
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
    }
}
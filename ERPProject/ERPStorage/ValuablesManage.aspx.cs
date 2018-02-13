﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using XTBase.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPStorage
{
    public partial class ValuablesManage : BillBase
    {
        string strDocSql = "SELECT * FROM DAT_YRK_DOC WHERE SEQNO ='{0}'";
        string strComSql = @"SELECT A.SEQNO,A.ROWNO,A.DEPTID,A.GDSEQ,A.BARCODE,A.GDNAME,A.UNIT,A.GDSPEC,A.GDMODE,A.CDID,A.SPLB,A.CATID,A.HWID,A.BZHL,A.BZSL,
                                                    A.DDSL,A.SSSL,A.JXTAX,A.HSJJ,A.BHSJJ,A.HSJE,A.BHSJE,A.LSJ,A.LSJE,A.ISGZ,A.ISLOT,A.PHID,A.PH, A.PZWH,TO_CHAR(A.RQ_SC,'YYYY-MM-DD') RQ_SC,
                                                    TO_CHAR(A.YXQZ,'YYYY-MM-DD') YXQZ,A.KCSL,A.KCHSJE,A.SPZTSL,A.ERPAYXS,A.HLKC,A.ZPBH,A.STR1,A.STR2,A.STR3,A.NUM1,A.NUM2,
                                                    A.NUM3,A.MEMO,A.ONECODE, F_GETUNITNAME(A.UNIT) UNITNAME,F_GETUNITNAME(B.UNIT) UNITSMALLNAME,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,F_GETSUPNAME(A.SUPID) SUPNAME
                                           FROM DAT_YRK_COM A, DOC_GOODS B WHERE SEQNO = '{0}' AND A.GDSEQ = B.GDSEQ ";

        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docCGY, docPSSID, docDEPTID, docDHRQ }; }
        }

        public ValuablesManage()
        {
            BillType = "GZD";//高值商品单据
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDDL();
                billNew();

                ButtonHidden(btnCopy, btnNext, btnBef, btnExport, btnAddRow);
                if (Request.QueryString["dg"] != null)
                {
                    hfdDG.Text = Request.QueryString["dg"].ToString();
                }
                //屏蔽不需要的操作按钮
                if (Request.QueryString["oper"] != null)
                {
                    hfdOper.Text = Request.QueryString["oper"].ToString();
                    if (Request.QueryString["oper"].ToString() == "input")
                    {
                        ButtonHidden(btnAudit, btnCancel);
                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        billLockDoc(true);
                        ButtonHidden(btnNew, btnCopy, btnSave, btnAddRow, btnDelRow, btnGoods, btnCommit, btnAllCommit, btnDel);
                        TabStrip1.ActiveTabIndex = 0;
                        if (Request.QueryString["pid"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["pid"].ToString()))
                        {
                            lstBILLNO.Text = Request.QueryString["pid"].ToString();
                            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(90);
                            billOpen(lstBILLNO.Text);
                        }
                        else
                        {
                            billSearch();
                        }
                    }
                }

                hfdOneCode.Text = Doc.DbGetSysPara("ISONECODE");
                if (PubFunc.StrIsEmpty(hfdOneCode.Text))
                {
                    hfdOneCode.Text = "Y";
                }
            }
        }
        protected override void BindDDL()
        {
            PubFunc.DdlDataGet("DDL_DOC_SUPPLIER", lstPSSID, docPSSID);
            PubFunc.DdlDataGet("DDL_USER", lstLRY, docCGY, docSHR, docLRY);
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);

            PubFunc.DdlDataGet("DDL_BILL_STATUSDHD", lstFLAG, docFLAG);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");

            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            docDHRQ.SelectedDate = DateTime.Now;
        }

        protected void bind_print()
        {
            string ip = "";
            string ip_rtn = "";
            if (Context.Request.ServerVariables["HTTP_VIA"] != null)
            {
                ip = Context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            else
            {
                ip = Context.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            foreach (IPAddress ip4 in Dns.GetHostEntry(ip).AddressList)
            {
                if (ip4.AddressFamily.ToString() == "InterNetwork")
                {
                    ip_rtn = ip4.ToString();
                    break;
                }
            }
            DataTable print = new DataTable();
            print = DbHelperOra.Query("SELECT PRINTNAME FROM SYS_PRINT WHERE IP='" + ip_rtn + "' AND BILLTYPE='BILL_RKD' ORDER BY STR1").Tables[0];
            if (print.Rows.Count > 0)
            {
                print_a4.Text = print.Rows[0]["PRINTNAME"].ToString();
                if (print.Rows.Count > 1)
                { print_liu.Text = print.Rows[1]["PRINTNAME"].ToString(); }
            }
        }
        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【录入日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO,F_GETBILLFLAG(FLAG)  FLAG,A.DDBH,F_GETDEPTNAME(A.DEPTID) DEPTID,
                                     F_GETSUPNAME(A.PSSID) SUPNAME,A.DHRQ,A.SUBSUM,F_GETUSERNAME(A.CGY) CGY,
                                     F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO 
                                FROM DAT_YRK_DOC A 
                               WHERE 1=1 ";
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
            if (lstPSSID.SelectedItem != null && lstPSSID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.SUPID='{0}'", lstPSSID.SelectedItem.Value);
            }

            //过滤新单的单据，不能审核，提交后的才能审核
            if (Request.QueryString["oper"].ToString() == "audit")
            {
                strSearch += " AND A.FLAG<>'M'";
            }

            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.FLAG,A.DHRQ DESC,A.DDBH DESC,A.BILLNO ASC";
            highlightRows.Text = "";
            highlightRowYellow.Text = "";
            GridList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataBind();
        }

        protected override void billGoods()
        {
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非新单不能增加商品");
                return;
            }
            if (docBILLNO.Text.Length > 0 && docBILLNO.Text.StartsWith("TRI"))
            {
                Alert.Show("下传单据不能追加商品！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            string url = "~/ERPQuery/GoodsWindow_New.aspx?DeptIn=" + docDEPTID.SelectedValue;
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));

        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billClear();
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
                Alert.Show("单据信息获取失败！！！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (docLRY.SelectedValue == null)
            {
                docLRY.SelectedValue = UserAction.UserID;
            }

            PageContext.RegisterStartupScript(GridCom.GetRejectChangesReference());
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno)).Tables[0];
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                decimal ddslTotal = 0, bzslTotal = 0, feeTotal = 0;
                foreach (DataRow row in dtBill.Rows)
                {
                    ddslTotal += Convert.ToDecimal(string.IsNullOrWhiteSpace(row["DDSL"].ToString()) ? "0" : row["DDSL"].ToString());
                    if (!PubFunc.StrIsEmpty(Convert.ToString(row["BZSL"] ?? "0")))
                    {
                        bzslTotal += Convert.ToDecimal(row["BZSL"] ?? "0");
                        feeTotal += Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZSL"] ?? "0");
                    }
                    row["HSJE"] = Convert.ToDecimal(row["HSJE"]).ToString("F2");
                    //处理订货单位
                    //DataTable dtGoods = DbHelperOra.QueryForTable(string.Format("select F_GETUNITNAME(UNIT_DABZ) UNIT_DABZNAME,F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZNAME,UNIT_ORDER,PIZNO from doc_goods where gdseq='{0}'", row["GDSEQ"].ToString()));
                    //if (dtGoods != null && dtGoods.Rows.Count > 0)
                    //{
                    //    if ("Z".Equals(dtGoods.Rows[0]["UNIT_ORDER"].ToString()))
                    //    {
                    //        row["UNITNAME"] = dtGoods.Rows[0]["UNIT_ZHONGBZNAME"].ToString();
                    //    }
                    //    if ("D".Equals(dtGoods.Rows[0]["UNIT_ORDER"].ToString()))
                    //    {
                    //        row["UNITNAME"] = dtGoods.Rows[0]["UNIT_DABZNAME"].ToString();
                    //    }
                    //    row["PZWH"] = dtGoods.Rows[0]["PIZNO"].ToString();
                    //}

                }
                /*
                *  修 改 人 ：袁鹏    修改时间：2015-03-20
                *  信息说明：这种加载方法比LoadGridRow(row, false, "OLD")更高效
                */
                PubFunc.GridRowAdd(GridCom, dtBill);

                //计算合计数量
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("DDSL", ddslTotal.ToString());
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));
                GridCom.SummaryData = summary;
            }

            PubFunc.FormLock(FormDoc, true, "");
            if (docDHRQ.Text == "")
            {
                docDHRQ.SelectedDate = DateTime.Now;
            }
            if (docFLAG.SelectedValue == "M")
            {
                docDHRQ.Enabled = true;
            }
            TabStrip1.ActiveTabIndex = 1;
            //增加按钮控制
            if (docFLAG.SelectedValue == "M")
            {
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnCommit.Enabled = true;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
                tbxMEMO.Enabled = true;
            }
            else if (docFLAG.SelectedValue == "N")
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnPrint.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                tbxMEMO.Enabled = false;
            }
            else
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = true;
                btnPrint.Enabled = true;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                tbxMEMO.Enabled = false;
            }
        }
        protected override void billNew()
        {
            string strSup = docPSSID.SelectedValue;
            string strDept = docDEPTID.SelectedValue;
            //原单据保存判断
            PubFunc.FormDataClear(FormDoc);
            if (PubFunc.StrIsEmpty(strDept)) 
            {
                if (docDEPTID.Items.Count > 2)
                    strDept = docDEPTID.Items[1].Value;
            }
            if (PubFunc.StrIsEmpty(strSup))
            {
                strSup = Doc.DbGetSysPara("SUPPLIER");
            }
            docFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            docCGY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docDHRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDept;
            docPSSID.SelectedValue = strSup;
            billLockDoc(false);
            rblOPER.SelectedValue = "GT";
            comBZSL.Enabled = true;
            comMEMO.Enabled = true;
            GridCom.SummaryData = null;
            PageContext.RegisterStartupScript(GridCom.GetRejectChangesReference());

            btnDel.Enabled = false;
            btnSave.Enabled = true;
            btnCommit.Enabled = false;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrint.Enabled = false;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();

                if (flag == "新单")
                {
                    highlightRows.Text += e.RowIndex.ToString() + ",";
                }
                if (flag == "已提交")
                {
                    highlightRowYellow.Text += e.RowIndex.ToString() + ",";
                }
            }
        }
        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非『新增』单据不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!Doc.getFlag(docBILLNO.Text, "M", BillType))
            {
                Alert.Show("此单据正在被别人操作，不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!docBILLNO.Text.Contains(BillType) && docBILLNO.Text.Length > 0)
            {
                Alert.Show("下传单据不能删除！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<string> listSql = new List<string>();
            listSql.Add("DELETE FROM DAT_YRK_DOC T WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            listSql.Add("DELETE FROM DAT_YRK_COM T WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            listSql.Add("UPDATE DAT_DO_LIST SET FLAG = 'Y' WHERE PARA='" + docBILLNO.Text.Trim() + "'");
            if (DbHelperOra.ExecuteSqlTran(listSql))
            {
                Alert.Show("单据删除成功!", "消息提示", MessageBoxIcon.Information);
                billNew();
                billSearch();
                OperLog("跟台商品入库", "删除单据【" + docBILLNO.Text + "】");
            }
            else
            {
                Alert.Show("单据删除失败!", "错误提示", MessageBoxIcon.Information);
            }
        }
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
            sqlList.Add(new CommandInfo("UPDATE DAT_YRK_DOC SET FLAG='N' WHERE BILLNO='" + strBILLNO + "' ", null));
            //增加待办事宜
            //sqlList.Add(Doc.GETDOADD("DO_8", docDEPTID.SelectedValue, docLRY.SelectedValue, docBILLNO.Text));

            if (DbHelperOra.ExecuteSqlTran(sqlList))
            {
                Alert.Show("此单据提交成功！");
                billSearch();
                billOpen(docBILLNO.Text);
                billLockDoc(true);
                OperLog("跟台商品入库", "提交单据【" + docBILLNO.Text + "】");
            }
            else
            {
                Alert.Show("此单据提交失败，请联系系统管理人员！");
            }

        }
        protected void btnAllCommit_Click(object sender, EventArgs e)
        {
            int[] selections = GridList.SelectedRowIndexArray;
            if (GridList.SelectedRowIndexArray.Length <= 0)
            {
                Alert.Show("请选择要审核的单据信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<CommandInfo> sqlList = new List<CommandInfo>();
            string succeed = string.Empty;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridList.SelectedRowIndexArray[i];
                if (GridList.DataKeys[rowIndex][1].ToString() == "新单")
                {
                    string strBILLNO = GridList.DataKeys[rowIndex][0].ToString();
                    sqlList.Add(new CommandInfo("update DAT_YRK_DOC set flag='N' where BILLNO='" + strBILLNO + "' ", null));
                    //增加待办事宜
                    //sqlList.Add(Doc.GETDOADD("DO_8", GridList.DataKeys[rowIndex][2].ToString(), GridList.DataKeys[rowIndex][3].ToString(), strBILLNO));
                    if (DbHelperOra.ExecuteSqlTran(sqlList))
                    {
                        succeed += strBILLNO + ",";
                    }
                }
            }
            if (succeed.Length > 0)
            {
                Alert.Show("单据【" + succeed.TrimEnd(',') + "】批量提交成功！", "消息提示", MessageBoxIcon.Warning);
                billSearch();
            }
            else
            {
                Alert.Show("你选中的单据均为『非新增』单据，不能进行提交操作", "消息提示", MessageBoxIcon.Warning);
            }

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
            PubFunc.GridRowAdd(GridCom, "INIT");
        }
        protected override void billDelRow()
        {
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("非『新增』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridCom.SelectedCell == null)
            {
                Alert.Show("空单不能删行", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!docBILLNO.Text.Contains(BillType) && docBILLNO.Text.Length > 0)
            {
                Alert.Show("下传单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            int rowIndex = GridCom.SelectedRowIndex;
            if (rowIndex < 0)
                return;
            PageContext.RegisterStartupScript(GridCom.GetDeleteRowReference(rowIndex));
            List<Dictionary<string, object>> newDict = GridCom.GetNewAddedList();
            newDict.RemoveAt(rowIndex);
            UpdateSum(newDict);

        }
        protected override void billSave()
        {
            #region 数据有效性验证
            if (("MR").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!CheckFlagForM() && !CheckFlagForR())
            {
                Alert.Show("此单据已经被别人操作，请等待操作!");
                return;
            }
            if (!docBILLNO.Text.Contains(BillType) && docBILLNO.Text.Length > 0)
            {
                Alert.Show("下传单据不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> goodsData = GridCom.GetNewAddedList();
            if (goodsData.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            if (docDHRQ.SelectedDate == null)
            {
                Alert.Show("收货日期输入错误!");
                return;
            }
            string type = DbHelperOra.GetSingle(string.Format("select TYPE from SYS_DEPT where CODE='{0}'", docDEPTID.SelectedValue)).ToString();
            List<Dictionary<string, object>> newDict = new List<Dictionary<string, object>>();
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < goodsData.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(goodsData[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(goodsData[i]["GDNAME"].ToString()))
                {
                    if ((goodsData[i]["BZSL"] ?? "").ToString() == "" || (goodsData[i]["BZSL"] ?? "").ToString() == "0")
                    {
                        Alert.Show("请填写商品[" + goodsData[i]["GDSEQ"] + "]入库数！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if ((goodsData[i]["HSJJ"] ?? "").ToString() == "" || (goodsData[i]["HSJJ"] ?? "").ToString() == "0" || Convert.ToDecimal(goodsData[i]["HSJJ"]) == 0)
                    {
                        Alert.Show("商品[" + goodsData[i]["GDSEQ"] + "]的【含税进价】不能为零,请维护好商品的含税进价，再次保存！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }


                    //当入库部门为1药库、2药房时
                    if ((",1,2").IndexOf(type) > 0 && string.IsNullOrWhiteSpace(goodsData[i]["HWID"].ToString()))
                    {
                        Alert.Show("第[" + (i + 1) + "]行商品【" + goodsData[i]["GDNAME"].ToString() + "】货位不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(goodsData[i]["HWID"].ToString()))
                        {
                            goodsData[i]["HWID"] = docDEPTID.SelectedValue;
                        }
                    }

                    newDict.Add(goodsData[i]);
                }
            }

            if (newDict.Count == 0)//所有Gird行都为空行时
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
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_YRK_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
                if (!PubFunc.StrIsEmpty(flg) && (",M,R").IndexOf(flg) < 0)
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
            if (docBILLNO.Text.Length > 15)
            {
                Alert.Show("单据编号输入超长,请检查!");
                return;
            }
            MyTable mtType = new MyTable("DAT_YRK_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow["FLAG"] = "M";
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", newDict.Count);
            mtType.ColRow.Add("PPSID", docPSSID.SelectedValue);
            //mtType.ColRow.Add("SUPNAME", docSUPID.SelectedText);
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_YRK_COM");
            //取消高值写入逻辑
            //MyTable mtTypeExt = new MyTable("DAT_RK_EXT");
            MyTable mtTypePh = new MyTable("DOC_GOODSPH");
            decimal subNum = 0;//总金额
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_YRK_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_YRK_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            newDict = newDict.OrderBy(x => x["GDSEQ"]).ToList();//按照商品编码重新排序
            for (int i = 0; i < newDict.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(newDict[i]);

                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow.Add("ROWNO", i + 1);
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                mtTypeMx.ColRow.Add("SSSL", 0);
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                mtTypeMx.ColRow["DDSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());

                string codeInfo = newDict[i]["CODEINFO"].ToString();
                mtTypeMx.ColRow.Remove("CODEINFO");
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            if (PubFunc.StrIsEmpty(mtType.ColRow["NUM1"].ToString()))
            {
                mtType.ColRow.Remove("NUM1");
            }
            cmdList.AddRange(mtType.InsertCommand());

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("商品入库数据保存成功！");
                billOpen(docBILLNO.Text);
                billLockDoc(true);
                OperLog("跟台商品入库", "修改单据【" + docBILLNO.Text + "】");
            }
            else
            {
                Alert.Show("商品入库数据保存失败！", "错误提示", MessageBoxIcon.Error);
            }
        }
        protected override void billAudit()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非新单不能审核！", "警告提示", MessageBoxIcon.Warning);
                return;
            }

            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "N", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            string sql = string.Format("UPDATE DAT_YRK_DOC SET FLAG = 'Y', SHR='{0}', SHRQ=SYSDATE WHERE BILLTYPE = 'YRK'", UserAction.UserID);
            DbHelperOra.Query(sql);
            Alert.Show("单据【" + strBillno + "】审核成功！");
            OperLog("跟台商品入库", "审核单据【" + strBillno + "】");

            //if (BillOper(strBillno, "AUDIT") == 1)
            //{
            //    billLockDoc(true);
            //    billOpen(strBillno);
            //    if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_YRK_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISFLAG3 = 'Y' AND A.SEQNO = '{0}'", strBillno)))
            //    {
            //        Alert.Show("单据【" + strBillno + "】审核成功，该单据中有直送商品，已自动生成出库单！");
            //    }
            //    else
            //    {
            //        Alert.Show("单据【" + strBillno + "】审核成功！");
            //    }
            //}
            billOpen(strBillno);
        }
        protected override void billCancel()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("已提交单据才能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!docBILLNO.Text.Contains(BillType) && docBILLNO.Text.Length > 0)
            {
                Alert.Show("下传单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridCom.GetNewAddedList();
            int count = newDict.Where(a => a["MEMO"].ToString().Contains("FROM_PLATFORM")).Count();
            if (count > 0)
            {
                Alert.Show("接口下传的单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            WindowReject.Hidden = false;
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            string BZHLerror = "";
            DataTable dt = GetGoods(hfdValue.Text);
            dt.Columns.Remove(dt.Columns["BZHL"]);
            dt.Columns.Remove(dt.Columns["UNIT"]);
            DataTable dtres = new DataTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                //dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                //dt.Columns["BZHL_SELL"].ColumnName = "BZHL";
                //dt.Columns["UNIT_SELL_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                dt.Columns["BZHL_ORDER"].ColumnName = "BZHL";
                dt.Columns["UNIT_ORDER_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNIT_ORDER"].ColumnName = "UNIT";

                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("DDSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                dtres = dt.Clone();
                foreach (DataRow row in dt.Rows)
                {
                    if ((row["BZHL"] ?? "").ToString() == "")
                    {

                        BZHLerror += "【" + row["GDSEQ"] + "】";
                        continue;
                    }

                    row["DDSL"] = "0";
                    row["HSJE"] = "0.00";
                    dtres.ImportRow(row);
                }
                DataTable dtBill = LoadGridData(dtres);
                string msg = "";
                if (dtBill != null && dtBill.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        row["BZSL"] = "0";
                        row["HSJE"] = "0";
                        if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                        {
                            msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                            Alert.Show("商品" + msg + "【含税进价】为空，不能进行【商品入库管理】操作。", "消息提示", MessageBoxIcon.Warning);
                            continue;
                        }
                        //处理金额格式
                        decimal jingdu = 0;
                        decimal bzhl = 0;
                        if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                        if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F4"); }
                        if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }

                        JObject defaultObj = new JObject();
                        foreach (DataColumn col in row.Table.Columns)
                        {
                            if (col.ColumnName.ToUpper() == "RQ_SC" || col.ColumnName.ToUpper() == "YXQZ")
                            {
                                if (!PubFunc.StrIsEmpty(row[col.ColumnName].ToString()))
                                {
                                    defaultObj.Add(col.ColumnName.ToUpper(), DateTime.Parse(row[col.ColumnName].ToString()).ToString("yyyy-MM-dd"));
                                }
                            }
                            else
                            {
                                defaultObj.Add(col.ColumnName.ToUpper(), row[col.ColumnName].ToString());
                            }
                        }

                        PageContext.RegisterStartupScript(GridCom.GetAddNewRecordReference(defaultObj, true));
                    }
                }

                docBILLNO.Enabled = false;
                docCGY.Enabled = false;
                docPSSID.Enabled = false;
                docDEPTID.Enabled = false;
                rblOPER.Enabled = false;

                if (!(BZHLerror == ""))
                    Alert.Show("商品" + BZHLerror + "包装含量异常，请维护后再选择！");
            }
        }
        private DataTable LoadGridData(DataTable dt)
        {
            DataTable mydt = dt.Copy();
            foreach (DataRow row in mydt.Rows)
            {
                //处理金额格式
                decimal jingdu = 0;
                if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu)) { row["HSJJ"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["YBJ"].ToString(), out jingdu)) { row["YBJ"] = jingdu.ToString("F6"); }
                if (decimal.TryParse(row["DDSL"].ToString(), out jingdu)) { row["DDSL"] = jingdu.ToString("F0"); }
                if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = jingdu.ToString("F4"); }
            }
            return mydt;
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
            decimal.TryParse((dicRecord["BZHL"] ?? "0").ToString(), out hl);
            decimal.TryParse((dicRecord["BZSL"] ?? "0").ToString(), out rs);
            decimal.TryParse((dicRecord["HSJJ"] ?? "0").ToString(), out jg);

            defaultObj.Remove("DDSL");
            defaultObj.Remove("HSJE");
            defaultObj.Add("DDSL", rs * hl);
            defaultObj.Add("HSJE", rs * jg);

            return defaultObj;
        }

        protected void GridCom_AfterEdit(object sender, GridAfterEditEventArgs e)
        {
            string [] strCell = GridCom.SelectedCell;
            int selectedRowIndex = GridCom.SelectedRowIndex;
            List<Dictionary<string, object>> newDict = GridCom.GetNewAddedList();
            if (newDict.Count == 0) return;
            if (e.ColumnID == "BZSL" || e.ColumnID == "HSJJ")
            {
                //string cell = string.Format("[{0},{1}]", e.RowIndex, intCell[1]);
                if (!PubFunc.isNumeric(newDict[e.RowIndex]["BZHL"].ToString()) || !PubFunc.isNumeric((newDict[e.RowIndex]["BZSL"] ?? "").ToString()) || !PubFunc.isNumeric(newDict[e.RowIndex]["HSJJ"].ToString()))
                {
                    Alert.Show("商品信息异常，请详细检查商品信息：包装含量、价格或数量！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                PageContext.RegisterStartupScript(GridCom.GetUpdateCellValueReference(e.RowIndex,GetJObject(newDict[e.RowIndex])));

                if (e.RowIndex != selectedRowIndex)
                {
                    //PageContext.RegisterStartupScript(GridCom.GetUpdateCellValueReference(GetJObject(newDict[selectedRowIndex]), string.Format("[{0},{1}]", intCell[0], intCell[1])));
                    PageContext.RegisterStartupScript(GridCom.GetUpdateCellValueReference(strCell[0],GetJObject(newDict[selectedRowIndex])));
                }
                //计算合计数量
                UpdateSum(newDict);
            }
        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            Window1.Hidden = true;
        }

        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue != "N" || docBILLNO.Text.Length == 0)
            {
                Alert.Show("已提交不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!docBILLNO.Text.Contains(BillType) && docBILLNO.Text.Length > 0)
            {
                Alert.Show("下传单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (docBILLNO.Text.Length < 1)
            { return; }

            if (string.IsNullOrWhiteSpace(ddlReject.SelectedValue))
            {
                Alert.Show("请选择驳回原因", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string strMemo = "驳回原因：" + ddlReject.SelectedText;
            if (!string.IsNullOrWhiteSpace(txaMemo.Text.Trim()))
            {
                strMemo += "；详细说明：" + txaMemo.Text;
            }

            if (DbHelperOra.ExecuteSql(string.Format("update DAT_YRK_DOC set flag='R',memo='{0}' where seqno='{1}' and flag='N'", strMemo, docBILLNO.Text)) == 1)
            {
                WindowReject.Hidden = true;
                billOpen(docBILLNO.Text);
            };
        }

        protected void btExport_Click(object sender, EventArgs e)
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【录入日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.BILLNO  订单编号,
                                    F_GETDEPTNAME(A.DEPTID) 订货部门,
                                    A.DHRQ 订货日期,
                                    F_GETUSERNAME(A.CGY) 订货员,
                                    F_GETUSERNAME(A.LRY) 录入人,
                                    A.LRRQ 录入日期,
                                    B.ROWNO 行号,
                                    B.GDSEQ 商品编码,
                                    B.GDNAME 商品名称,
                                    B.GDSPEC 商品规格,
                                    B.PZWH 注册证号,
                                    F_GETUNITNAME(B.UNIT) 单位,
                                    B.BZHL 包装含量,
                                    B.BZSL  订货包装数,
                                    B.DDSL   订货数量,
                                    B.HSJJ 价格
                                FROM DAT_YRK_DOC A, DAT_YRK_COM B
                                WHERE A.SEQNO = B.SEQNO";
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
            if (lstPSSID.SelectedItem != null && lstPSSID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.SUPID='{0}'", lstPSSID.SelectedItem.Value);
            }

            if (lstPSSID.SelectedItem != null && lstPSSID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND FLAG='{0}'", lstPSSID.SelectedItem.Value);
            }

            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC,B.ROWNO";

            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            ExcelHelper.ExportByWeb(dt, string.Format("【{0}】高值跟台商品信息", docDEPTID.SelectedText), "高值跟台商品信息导出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }

        protected void GridCom_RowClick(object sender, GridRowClickEventArgs e)
        {
            //用+=选中多行的删除
            List<string> ListStr = ColorForGridGoods.Text.Split(',').ToList<string>();
            if (ListStr.Contains(e.RowIndex.ToString()))
            {
                ColorForGridGoods.Text = "";
                ListStr.Remove(e.RowIndex.ToString());
                for (int i = 0; i < ListStr.Count; i++)
                {
                    if (ListStr[i].Length > 0)
                    {
                        ColorForGridGoods.Text = ListStr[i] + ",";
                    }
                }
            }
            else
            {
                ColorForGridGoods.Text = e.RowIndex.ToString() + ",";
            }
            PageContext.RegisterStartupScript("highlightRowsForGoodsGrid();");
        }

        /// <summary>
        /// 更新汇总信息 20150510 liuz  解决带出信息更新汇总信息显示
        /// </summary>
        /// <param name="newDict"></param>
        private void UpdateSum(List<Dictionary<string, object>> newDict)
        {
            //计算合计数量
            decimal ddslTotal=0,bzslTotal = 0, feeTotal = 0;
            foreach (Dictionary<string, object> dic in newDict)
            {
                bzslTotal += Convert.ToDecimal(dic["BZSL"]);
                feeTotal += Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["BZSL"]);
                ddslTotal += Convert.ToDecimal(string.IsNullOrWhiteSpace(dic["DDSL"].ToString()) ? "0" : dic["DDSL"].ToString());
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("DDSL", ddslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridCom.SummaryData = summary;
        }

        private bool CheckFlagForM()
        {
            if (docBILLNO.Text.Length > 0)
            {
                return Doc.getFlag(docBILLNO.Text, "M", BillType);
            }
            return true;
        }

        private bool CheckFlagForR()
        {
            if (docBILLNO.Text.Length > 0)
            {
                return Doc.getFlag(docBILLNO.Text, "R", BillType);
            }
            return true;
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            if (e.EventArgument.IndexOf("GoodsAdd") >= 0)
            {
                Window1_Close(null, null);
            }
        }
    }
}
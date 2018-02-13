﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Data;
using XTBase;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using ERP3PRO;
using Oracle.ManagedDataAccess.Client;
using XTBase.Utilities;
using ERPProject;
using System.IO;

namespace ERPProject.ERPReapt
{
    public partial class HighGoodsOrder : BillBase
    {
        string strDocSql = "SELECT * FROM DAT_DDPLAN_DOC WHERE SEQNO ='{0}'";
        string strComSql = @"SELECT A.SEQNO,A.ROWNO,A.DEPTID,A.GDSEQ,A.BARCODE,A.GDNAME,A.UNIT,A.GDSPEC,A.CDID,A.SPLB,A.CATID,A.BZHL,A.BZSL,
                                                    A.DHS,A.JXTAX,A.HSJJ,A.BHSJJ,A.HSJE,A.BHSJE,A.LSJ,A.LSJE,A.ISGZ,A.ISLOT, A.PZWH,
                                                    A.KCSL,A.KCHSJE,A.SPZTSL,A.ERPAYXS,A.HLKC,A.ZPBH,A.STR1,A.STR2,A.STR3,A.NUM1,A.NUM2,A.SUPID,A.PRODUCER,
                                                    A.NUM3,A.MEMO,F_GETUNITNAME(A.UNIT) UNITNAME,F_GETUNITNAME(B.UNIT) UNITSMALLNAME,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,F_GETSUPNAME(A.SUPID) SUPNAME
                                           FROM DAT_DDPLAN_COM A, DOC_GOODS B WHERE SEQNO = '{0}' AND A.GDSEQ = B.GDSEQ order by rowno ";

        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docCGY, docDEPTID, docDHRQ }; }
        }

        public HighGoodsOrder()
        {
            BillType = "DHP";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindDDL();
                billNew();
                if (Request.QueryString["oper"] != null)
                {
                    if (Request.QueryString["oper"].ToString() == "input")
                    {
                        ButtonHidden(btnCancel, btnCopy, btnNext, btnBef, btnExport, btnAddRow, btnAudit, btnAllCommit);
                        TabStrip1.ActiveTabIndex = 1;

                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        ButtonHidden(btnCopy, btnNext, btnBef, btnExport, btnAddRow, btnNew, btnSave, btnCommit, btnDel, btnGoods, btn_Auto, btnCreate, btnDelRow);
                        LinkButton1.Hidden = true;
                        fuDocument.Hidden = true;
                        TabStrip1.ActiveTabIndex = 0;

                    }
                }
                else
                {
                    return;
                }
                //屏蔽不需要的操作按钮

                if (isGZ())
                {
                    //高值也可以备货到库房 lvj 20160414
                    //DepartmentBind.BindDDL("DDL_SYS_DEPOTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
                    PubFunc.DdlDataGet("DDL_ORDER_DEPT", docDEPTID, lstDEPTID);
                    ddlDHFS.SelectedValue = "G";
                   // btnCreate.Hidden = false;
                    LinkButton1.Hidden = true; //高值时导入不显示
                    fuDocument.Hidden = true;
                }
                else
                {
                    DepartmentBind.BindDDL("DDL_SYS_DEPOTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
                    ddlDHFS.SelectedValue = "P";
                    docSTR2.Hidden = true;
                    btn_Auto.Hidden = true;
                }
                docDEPTID.SelectedValue = UserAction.UserDept;
            }
        }
        protected bool isGZ()
        {
            if (Request.QueryString["tp"] == null)
            {
                //高值
                return true;
            }
            else if (Request.QueryString["tp"].ToString() == "D")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        protected override void BindDDL()
        {
            PubFunc.DdlDataGet("DDL_USER", lstLRY, docCGY, docSHR, docLRY, lstCZY);
            PubFunc.DdlDataGet("DDL_BILL_STATUSDHD", docFLAG);
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            docDHRQ.SelectedDate = DateTime.Now;
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

            string strSql = @"SELECT A.SEQNO,A.BILLNO,F_GETBILLFLAG(FLAG) FLAG_CN, FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,
                                     A.XDRQ,A.SUBSUM,F_GETUSERNAME(A.CGY) CGY,
                                     F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO 
                                FROM DAT_DDPLAN_DOC A 
                               WHERE 1=1  ";
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
            if (lstCZY.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND A.CGY='{0}'", lstCZY.SelectedValue);
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            //if (lstDDBH.Text.Length > 0)
            //{
            //    strSearch += string.Format(" AND A.STR1 LIKE '%{0}%'", lstDDBH.Text);
            //}
            if (isGZ())
            {
                strSearch += string.Format(" AND A.DHFS = 'G'");
            }
            else
            {
                strSearch += string.Format(" AND A.DHFS = 'P'");
            }

            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);
            if (Request.QueryString["oper"] == "audit")
            {
                strSearch += " AND A.FLAG NOT IN('M','C') ";
            }
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
          
            strSql += " ORDER BY A.BILLNO DESC,A.FLAG ASC";
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
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            string url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTID.SelectedValue + "&isGZ=N&isbd=N";
            if (isGZ())
            {
                url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTID.SelectedValue + "&isGZ=Y&isbd=N";
            }
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));

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
                decimal bzslTotal = 0, feeTotal = 0;
                foreach (DataRow row in dtBill.Rows)
                {
                    if (!PubFunc.StrIsEmpty(Convert.ToString(row["BZSL"] ?? "0")))
                    {
                        bzslTotal += Convert.ToDecimal(row["BZSL"] ?? "0");
                        feeTotal += Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZSL"] ?? "0");
                    }
                    row["HSJE"] = Convert.ToDecimal(row["HSJE"]).ToString("F2");

                }
                //PubFunc.GridRowAdd(GridCom, dtBill);
                string joBill = JsonConvert.SerializeObject(dtBill);
                PageContext.RegisterStartupScript("F('" + GridCom.ClientID + "').addNewRecords( " + joBill + " );");
                //计算合计数量
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F2"));
                GridCom.SummaryData = summary;
            }
            PubFunc.FormLock(FormDoc, true, "");
            if (docFLAG.SelectedValue == "M")
            {
                docDHRQ.Enabled = true;
            }
            TabStrip1.ActiveTabIndex = 1;
            //增加按钮控制
            if (docFLAG.SelectedValue == "N")
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
                tbxMEMO.Enabled = true;
            }
            else if (docFLAG.SelectedValue == "M")
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
            else if (docFLAG.SelectedValue == "R")
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
            string strDept = docDEPTID.SelectedValue;
            string strDHFS = ddlDHFS.SelectedValue;
            //原单据保存判断
            PubFunc.FormDataClear(FormDoc);
            if (PubFunc.StrIsEmpty(strDept))
            {
                if (docDEPTID.Items.Count > 2)
                    strDept = docDEPTID.Items[1].Value;
            }
            docFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            docCGY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docDHRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDept;
            ddlDHFS.SelectedValue = strDHFS;
            billLockDoc(false);
            comBZSL.Enabled = true;
            comMEMO.Enabled = true;
            tbxMEMO.Enabled = true;
            //comGDSEQ.Enabled = true;
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
            List<string> listSql = new List<string>();
            listSql.Add("Delete from DAT_DDPLAN_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "' AND FLAG = 'M'");
            listSql.Add("Delete from DAT_DDPLAN_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            if (DbHelperOra.ExecuteSqlTran(listSql))
            {
                Alert.Show("单据删除成功!", "消息提示", MessageBoxIcon.Information);
                billNew();
                billSearch();
            }
            else
            {
                Alert.Show("单据删除失败!", "错误提示", MessageBoxIcon.Information);
            }
        }
        protected void btnAllCommit_Click(object sender, EventArgs e)
        {
            int[] selections = GridList.SelectedRowIndexArray;
            if (GridList.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要审核的单据信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string succeed = string.Empty;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridList.SelectedRowIndexArray[i];
                if (GridList.DataKeys[rowIndex][1].ToString() == "新单")
                {
                    string strBILLNO = GridList.DataKeys[rowIndex][0].ToString();
                    if (BillOper(strBILLNO, "AUDIT") == 1)
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
            if (GridCom.GetMergedData().Count == 0)
            {
                Alert.Show("空单不能删行", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!docBILLNO.Text.Contains(BillType) && docBILLNO.Text.Length > 0)
            {
                Alert.Show("下传单据不能驳回！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string[] rowIdArray = GridCom.SelectedRowIDArray;
            JArray ja = GridCom.GetMergedData();

            foreach (string rowId in rowIdArray)
            {
                PageContext.RegisterStartupScript(GridCom.GetDeleteRowReference(rowId));
                JToken jt = ja.SelectToken("$[?(@.id == '" + rowId + "')]");
                ja.Remove(jt);
            }
            //List<Dictionary<string, object>> newDict = new List<Dictionary<string, object>>();
            //foreach (JToken jt in ja)
            //{
            //    JObject jo = (JObject)jt;
            //    JObject dataValue = jo.Value<JObject>("values");
            //    newDict.Add(JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(dataValue)));
            //}
            //PageContext.RegisterStartupScript(GridCom.GetDeleteRowReference(rowIndex));
            //List<Dictionary<string, object>> newDict = GridCom.GetNewAddedList();
            //获取RowIndex

            //newDict.RemoveAt(rowIndex);
            UpdateSum();
            //PageContext.RegisterStartupScript("F('" + GridCom.ClientID + "'). deleteSelectedRows(true)");

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
            JArray goodsData = GridCom.GetMergedData();
            //List<Dictionary<string, object>> goodsData =  GridCom.GetNewAddedList();
            if (goodsData.Count == 0)
            {
                Alert.Show("请输入商品信息!", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            if (docDHRQ.SelectedDate == null)
            {
                Alert.Show("订货日期输入错误!");
                return;
            }
            List<Dictionary<string, object>> newDict = new List<Dictionary<string, object>>();
            //判断是否有空行、批号填写是否符合要求
            foreach (JToken jt in goodsData)
            {
                JObject jo = (JObject)jt;
                JObject dataValue = jo.Value<JObject>("values");
                if (!string.IsNullOrWhiteSpace(dataValue.Value<String>("GDSEQ")) && !string.IsNullOrWhiteSpace(dataValue.Value<String>("GDNAME")))
                {
                    if ((dataValue.Value<String>("BZSL") ?? "") == "" || (dataValue.Value<String>("BZSL") ?? "").ToString() == "0")
                    {
                        Alert.Show("请填写商品[" + dataValue.Value<String>("GDSEQ") + "]订货数！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if ((dataValue.Value<String>("HSJJ") ?? "").ToString() == "" || Convert.ToDecimal(dataValue.Value<String>("HSJJ")) < 0)
                    {
                        Alert.Show("商品[" + dataValue.Value<String>("GDSEQ") + "]的【含税进价】不能小于零,请维护好商品的含税进价，再次保存！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }

                    newDict.Add(JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(dataValue)));
                }
            }
            //for (int i = 0; i < goodsData.Count; i++)
            //{
            //    if (!string.IsNullOrWhiteSpace(goodsData[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(goodsData[i]["GDNAME"].ToString()))
            //    {
            //        if ((goodsData[i]["BZSL"] ?? "").ToString() == "" || (goodsData[i]["BZSL"] ?? "").ToString() == "0")
            //        {
            //            Alert.Show("请填写商品[" + goodsData[i]["GDSEQ"] + "]订货数！", "消息提示", MessageBoxIcon.Warning);
            //            return;
            //        }
            //        if ((goodsData[i]["HSJJ"] ?? "").ToString() == "" || Convert.ToDecimal(goodsData[i]["HSJJ"]) < 0)
            //        {
            //            Alert.Show("商品[" + goodsData[i]["GDSEQ"] + "]的【含税进价】不能小于零,请维护好商品的含税进价，再次保存！", "消息提示", MessageBoxIcon.Warning);
            //            return;
            //        }
            //        newDict.Add(goodsData[i]);
            //    }
            //}

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
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_DDPLAN_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
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
            MyTable mtType = new MyTable("DAT_DDPLAN_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow["FLAG"] = "M";
            mtType.ColRow["DHRQ"] = Convert.ToDateTime(docDHRQ.Text);
            mtType.ColRow["XDRQ"] = Convert.ToDateTime(docLRRQ.Text);
            if (isGZ())
            {
                mtType.ColRow["DHFS"] = "G";
            }
            else
            {
                mtType.ColRow["DHFS"] = "P";
            }

            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", newDict.Count);
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_DDPLAN_COM");
            decimal subNum = 0;//总金额
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_DDPLAN_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_DDPLAN_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            //newDict = newDict.OrderBy(x => x["GDSEQ"]).ToList();//按照商品编码重新排序
            for (int i = 0; i < newDict.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(newDict[i]);
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                mtTypeMx.ColRow["DHS"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            cmdList.AddRange(mtType.InsertCommand());

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("商品订货数据保存成功！");
                billOpen(docBILLNO.Text);
                billLockDoc(true);
            }
            else
            {
                Alert.Show("商品订货数据保存失败！", "错误提示", MessageBoxIcon.Error);
            }
        }
      
        protected override void billAudit()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非提交单据不能审核！", "警告提示", MessageBoxIcon.Warning);
                return;
            }

            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "M", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            if (BillOper(strBillno, "AUDIT") == 1)
            {
                billLockDoc(true);
                Alert.Show("单据【" + strBillno + "】审核成功！");
                billOpen(strBillno);
            }
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
                dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                dt.Columns["BZHL_ORDER"].ColumnName = "BZHL";
                dt.Columns["UNIT_ORDER_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNIT_ORDER"].ColumnName = "UNIT";
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("DHS", Type.GetType("System.Int32"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                dtres = dt.Clone();
                foreach (DataRow row in dt.Rows)
                {
                    if ((row["BZHL"] ?? "").ToString() == "")
                    {

                        BZHLerror += "【" + row["GDSEQ"] + "】";
                        continue;
                    }
                    row["DHS"] = "0";
                    row["HSJE"] = "0.00";
                    dtres.ImportRow(row);
                }
                DataTable dtBill = LoadGridData(dtres);
                string msg = "";
                if (dtBill != null && dtBill.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        //添加商品不允许重复
                        List<Dictionary<string, object>> newDict = GridCom.GetNewAddedList();
                        for (int i = 0; i < newDict.Count; i++)
                        {
                            if (newDict[i]["GDSEQ"].ToString() == row["GDSEQ"].ToString())
                            {
                                Alert.Show("商品" + newDict[i]["GDSEQ"] + "已被添加到单据中！", "消息提示", MessageBoxIcon.Warning);
                                return;
                            }
                        }
                        row["BZSL"] = "0";
                        row["HSJE"] = "0";
                        if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                        {
                            msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】";
                            Alert.Show("商品" + msg + "【含税进价】为空，不能进行【商品入库管理】操作！", "消息提示", MessageBoxIcon.Warning);
                            return;
                        }
                        if (string.IsNullOrWhiteSpace(row["BZHL"].ToString()) || row["BZHL"].ToString() == "0")
                        {
                            msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】";
                            Alert.Show("商品" + msg + "【包装含量】维护错误！", "消息提示", MessageBoxIcon.Warning);
                            return;
                        }
                        //处理金额格式
                        decimal jingdu = 0;
                        decimal bzhl = 0;
                        if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                        if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }

                        JObject defaultObj = new JObject();
                        foreach (DataColumn col in row.Table.Columns)
                        {
                            defaultObj.Add(col.ColumnName.ToUpper(), row[col.ColumnName].ToString());
                        }
                        PageContext.RegisterStartupScript(GridCom.GetAddNewRecordReference(defaultObj, true));
                    }
                }

                docBILLNO.Enabled = false;
                docCGY.Enabled = false;
                docDEPTID.Enabled = false;
                if (!(BZHLerror == ""))
                    Alert.Show("商品" + BZHLerror + "包装含量异常，请维护后再选择！");
            }
            UpdateSum();
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
                if (decimal.TryParse(row["DHS"].ToString(), out jingdu)) { row["DHS"] = jingdu.ToString("F0"); }
                if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = jingdu.ToString("F4"); }

                //PubFunc.GridRowAdd(GridGoods, row, firstRow);
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

            defaultObj.Remove("DHS");
            defaultObj.Remove("HSJE");
            defaultObj.Add("DHS", rs * hl);
            defaultObj.Add("HSJE", rs * jg);

            return defaultObj;
        }

        protected void GridCom_AfterEdit(object sender, GridAfterEditEventArgs e)
        {

            //string[] strCell = GridCom.SelectedCell;
            string selectedRowId = e.RowID;
            if (selectedRowId == null) return;
            JArray ja = GridCom.GetMergedData();
            JObject newDictJson = (JObject)ja.SelectToken("$[?(@.id == '" + selectedRowId + "')].values");
            Dictionary<string, object> newDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(newDictJson));
            //List<Dictionary<string, object>> newDict = GridCom.GetNewAddedList();
            //if (newDict.Count == 0) return;
            if (e.ColumnID == "BZSL" || e.ColumnID == "HSJJ")
            {
                //string cell = string.Format("[{0},{1}]", e.RowIndex, intCell[1]);
                if (!PubFunc.isNumeric(newDict["BZHL"].ToString()) || !PubFunc.isNumeric((newDict["BZSL"] ?? "").ToString()) || !PubFunc.isNumeric(newDict["HSJJ"].ToString()))
                {
                    Alert.Show("商品信息异常，请详细检查商品信息：包装含量、价格或数量！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                //PageContext.RegisterStartupScript(GridCom.GetUpdateCellValueReference(GetJObject(newDict[e.RowIndex]), cell));

                //if (e.RowIndex != Convert.ToInt32(strCell[0]))
                //{
                //PageContext.RegisterStartupScript(GridCom.GetUpdateCellValueReference(GetJObject(newDict[intCell[0]]), string.Format("[{0},{1}]", intCell[0], intCell[1])));
                PageContext.RegisterStartupScript(GridCom.GetUpdateCellValueReference(selectedRowId, GetJObject(newDict)));

                //}
                //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0;
                foreach (JToken jt in ja)
                {
                    JObject jo = (JObject)jt;
                    JObject values = jo.Value<JObject>("values");
                    bzslTotal += Convert.ToDecimal(values.Value<string>("BZSL") ?? "0");
                    feeTotal += Convert.ToDecimal(values.Value<string>("HSJJ") ?? "0") * Convert.ToDecimal(values.Value<string>("BZSL") ?? "0");
                }
                //foreach (Dictionary<string, object> dic in newDict)
                //{
                //    bzslTotal += Convert.ToDecimal(dic["BZSL"] ?? "0");
                //    feeTotal += Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal(dic["BZSL"] ?? "0");
                //}
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", string.Format("{0:F2}", feeTotal));

                GridCom.SummaryData = summary;
            }
        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            Window1.Hidden = true;
        }

        protected void btExport_Click(object sender, EventArgs e)
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【录入日期】！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.BILLNO 单据编号,A.STR1 订单编号,
                                    F_GETDEPTNAME(A.DEPTID) 入库部门,
                                    A.lrrq 录入日期,
                                    F_GETUSERNAME(A.CGY) 入库人,
                                    F_GETUSERNAME(A.LRY) 录入人,
                                    A.LRRQ 录入日期,
                                    B.ROWNO 行号,
                                    B.GDSEQ 商品编码,
                                    B.GDNAME 商品名称,
                                    B.GDSPEC 商品规格,
                                    B.PZWH 注册证号,
                                    F_GETUNITNAME(B.UNIT) 单位,
                                    B.BZHL 包装含量,
                                    B.BZSL 入库包装数,
                                    B.DHS 入库数量,
                                    B.PH 批号,
                                    B.RQ_SC 生产日期,
                                    B.YXQZ 有效期至,
                                    B.HSJJ 价格
                                FROM DAT_DDPLAN_DOC A, DAT_DDPLAN_COM B
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
            if (isGZ())
            {
                strSearch += string.Format(" AND A.DHFS = 'G'");
            }
            else
            {
                strSearch += string.Format(" AND A.DHFS = 'P'");
            }
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC,B.ROWNO";

            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            ExcelHelper.ExportByWeb(dt, string.Format("【{0}】入库信息", docDEPTID.SelectedText), "入库信息导出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
        }


        private void UpdateSum()
        {
            JArray ja = GridCom.GetMergedData();
            decimal bzslTotal = 0, feeTotal = 0;
            foreach (JToken jt in ja)
            {
                JObject jo = (JObject)jt;
                JObject values = jo.Value<JObject>("values");
                bzslTotal += Convert.ToDecimal(values.Value<string>("BZSL") ?? "0");
                feeTotal += Convert.ToDecimal(values.Value<string>("HSJJ") ?? "0") * Convert.ToDecimal(values.Value<string>("BZSL") ?? "0");
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", string.Format("{0:F2}", feeTotal));
            GridCom.SummaryData = summary;
        }

        /// <summary>
        /// 更新汇总信息 20150510 liuz  解决带出信息更新汇总信息显示
        /// </summary>
        /// <param name="newDict"></param>
        private void UpdateSum(List<Dictionary<string, object>> newDict)
        {
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
        protected void btn_Auto_Click(object sender, EventArgs e)
        {
            //高值自动订货
            String Sql = @"SELECT DISTINCT A.CODE, A.NAME,DECODE(E.DHFS,'G','存在未处理完成单据','正常') TYPENAME
                      FROM SYS_DEPT A, DOC_GOODSCFG B,(SELECT SUM(KCSL - LOCKSL) KCSL,GDSEQ,DEPTID FROM DAT_GOODSSTOCK GROUP BY GDSEQ,DEPTID) C,DOC_GOODS D,(SELECT DEPTID,DHFS FROM DAT_DDPLAN_DOC WHERE (FLAG = 'N' OR FLAG = 'M') AND DHFS = 'G') E
                     WHERE A.TYPE = '3'
                       AND A.CODE = B.DEPTID AND B.DEPTID = C.DEPTID(+) AND B.GDSEQ = C.GDSEQ(+) AND B.GDSEQ = D.GDSEQ AND D.ISGZ='Y' AND B.ZDKC > NVL(C.KCSL,0) + F_GETZTKC_KS(A.CODE,B.GDSEQ)
                       AND A.CODE = E.DEPTID(+)
                       AND F_CHK_DATARANGE(A.CODE, '{0}') = 'Y'";
            DataTable dt = DbHelperOra.Query(string.Format(Sql, UserAction.UserID)).Tables[0];
            if (dt.Rows.Count < 1)
            {
                Alert.Show("无需要自动订货的科室！", "提示信息");
                return;
            }
            GridAuto.DataSource = dt;
            GridAuto.DataBind();
            WinAuto.Hidden = false;
        }

        protected void btnClose_Click1(object sender, EventArgs e)
        {
            WinAuto.Hidden = true;
        }

        protected void btnSure_Click(object sender, EventArgs e)
        {
            //生成订单
            int[] selections = GridAuto.SelectedRowIndexArray;
            String strDept = "";
            foreach (int rowIndex in selections)
            {
                strDept += GridAuto.DataKeys[rowIndex][0].ToString() + ",";
            }
            if (strDept.Length > 0)
            {
                OracleParameter[] parameters = new OracleParameter[]
                                {
                                     new OracleParameter("VIN_DEPTID",OracleDbType.Varchar2),
                                     new OracleParameter("VIN_OPERUSER",OracleDbType.Varchar2),
                                };
                parameters[0].Value = strDept;
                parameters[1].Value = UserAction.UserID;
                DbHelperOra.RunProcedure("STOREDS.P_AUTODD", parameters);
                Alert.Show("自动生成成功！");
                billSearch();
                WinAuto.Hidden = true;
            }
            else
            {
                Alert.Show("请选择科室信息！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
        }
        private bool CheckFileExt(string fileNameExt)
        {
            if (String.IsNullOrEmpty(fileNameExt))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool ImpGoods(DataTable dt)
        {
            string BZHLerror = "";
            string msg = "";
            string GDSQL = "";
            foreach (DataRow dtrow in dt.Rows)
            {
                GDSQL = "SELECT GDSEQ,GDNAME,GDSPEC,f_getunitname(UNIT) UNITNAME,BZHL," + dtrow["数量"].ToString() + " BZSL,HSJJ," + dtrow["数量"].ToString() + @" * HSJJ HSJE,F_GETSUPNAME(F_GETGOODSSUPID(GDSEQ)) SUPNAME,
                        F_GETPRODUCERNAME(PRODUCER) PRODUCERNAME,PIZNO PZWH, JXTAX, ZPBH, MEMO, BARCODE, UNIT, ISGZ, PRODUCER,
												F_GETGOODSSUPID(GDSEQ)SUPID, ISLOT
									FROM DOC_GOODS
									WHERE GDSEQ = '" + dtrow["商品编码"].ToString() + "'";
                DataTable dtBill = DbHelperOra.Query(GDSQL).Tables[0];

                foreach (DataRow row in dtBill.Rows)
                {
                    //添加商品不允许重复
                    //List<Dictionary<string, object>> newDict = GridCom.GetNewAddedList();
                    //for (int i = 0; i < newDict.Count; i++)
                    //{
                    //	if (newDict[i]["GDSEQ"].ToString() == row["GDSEQ"].ToString())
                    //	{
                    //		Alert.Show("商品" + newDict[i]["GDSEQ"] + "已被添加到单据中！", "消息提示", MessageBoxIcon.Warning);
                    //		return false;
                    //	}
                    //}				 
                    if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                    {
                        msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                        Alert.Show("商品" + msg + "【含税进价】为空，不能进行【商品入库管理】操作！", "消息提示", MessageBoxIcon.Warning);
                        return false;
                    }
                    //处理金额格式
                    decimal jingdu = 0;
                    decimal bzhl = 0;
                    if (decimal.TryParse(row["HSJJ"].ToString(), out jingdu) && decimal.TryParse(row["BZHL"].ToString(), out bzhl)) { row["HSJJ"] = Math.Round(jingdu * bzhl, 4).ToString("F4"); }
                    if (decimal.TryParse(row["HSJE"].ToString(), out jingdu)) { row["HSJE"] = Math.Round(jingdu, 2).ToString("F2"); }

                    JObject defaultObj = new JObject();
                    foreach (DataColumn col in row.Table.Columns)
                    {
                        defaultObj.Add(col.ColumnName.ToUpper(), row[col.ColumnName].ToString());
                    }
                    PageContext.RegisterStartupScript(GridCom.GetAddNewRecordReference(defaultObj, true));
                }

                docBILLNO.Enabled = false;
                docCGY.Enabled = false;
                docDEPTID.Enabled = false;
                if (!(BZHLerror == ""))
                    Alert.Show("商品" + BZHLerror + "包装含量异常，请维护后再导入！");
            }
            return true;
        }
        protected void btnSelect_Click(object sender, EventArgs e)
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            highlightRows.Text = "";
            #region 导入
            if (this.fuDocument.HasFile)
            {
                string toFilePath = "~/ERPUpload/DownloadExcel/";

                //获得文件扩展名
                string fileNameExt = Path.GetExtension(this.fuDocument.FileName).ToLower();

                if (!ValidateFileType(fileNameExt))
                {
                    Alert.Show("无效的文件类型！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }

                //验证合法的文件
                if (CheckFileExt(fileNameExt))
                {
                    //生成将要保存的随机文件名
                    string fileName = this.fuDocument.FileName.Substring(0, this.fuDocument.FileName.IndexOf(".")) + DateTime.Now.ToString("yyyyMMddHHmmss") + fileNameExt;

                    //按日期归类保存
                    string datePath = DateTime.Now.ToString("yyyyMM") + "/" + DateTime.Now.ToString("dd") + "/";
                    toFilePath += datePath;

                    //获得要保存的文件路径
                    string DownloadUrl = toFilePath + fileName;
                    //物理完整路径                    
                    string toFileFullPath = HttpContext.Current.Server.MapPath(toFilePath);

                    //检查是否有该路径,没有就创建
                    if (!Directory.Exists(toFileFullPath))
                    {
                        Directory.CreateDirectory(toFileFullPath);
                    }

                    //将要保存的完整物理文件名                
                    string serverFileName = toFileFullPath + fileName;

                    //获取保存的excel路径
                    this.fuDocument.SaveAs(serverFileName);

                    if (File.Exists(serverFileName))
                    {
                        DataTable table = new DataTable();

                        if (fileNameExt == ".xlsx")
                        {
                            table = ExcelHelper.ImportExcelxtoDt(serverFileName, 0, 1); //导入excel2007
                        }
                        else
                        {
                            table = ExcelHelper.ImportExceltoDt(serverFileName, 0, 1);//导入excel2003
                        }
                        //清空匹配表
                        DbHelperOra.ExecuteSql("DELETE FROM DAT_STOCK_IMP WHERE LRY = '" + UserAction.UserID + "'");
                        if (table != null && table.Rows.Count > 0)
                        {
                            ImpGoods(table);
                        }
                        File.Delete(serverFileName);
                    }
                }
            }
            else
            {
                Alert.Show("请选择excel文件！");
            }
            #endregion
        }
        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
            {
                Alert.Show("请选择要生成计划的科室", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            if (docSTR2.SelectedValue != "BH")
            {
                Alert.Show("订货类型【" + docSTR2.SelectedText + "】不能生成要货计划！！！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue != "M" && docFLAG.SelectedValue != "R")
            {
                Alert.Show("单据状态为【" + docFLAG.SelectedText + "】，不能生成计划！！！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;

            if (PubFunc.StrIsEmpty(docBILLNO.Text))
            {
                docSEQNO.Text = BillSeqGet();
                docBILLNO.Text = docSEQNO.Text;
                docBILLNO.Enabled = false;
            }
            else
            {
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_DDPLAN_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
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
            MyTable mtType = new MyTable("DAT_DDPLAN_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow["FLAG"] = "M";
            if (isGZ())
            {
                mtType.ColRow["DHFS"] = "G";
            }
            else
            {
                mtType.ColRow["DHFS"] = "P";
            }

            mtType.ColRow.Add("BILLTYPE", BillType);
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.AddRange(mtType.InsertCommand());

            OracleParameter[] parameters = {
                                                                                             new OracleParameter("V_DEPTID", OracleDbType.Varchar2,20),
                                                                                             new OracleParameter("V_SEQNO", OracleDbType.Varchar2,20) };
            parameters[0].Value = docDEPTID.SelectedValue;
            parameters[1].Value = docBILLNO.Text;
            cmdList.Add(new CommandInfo("P_GZBHORDER", parameters, CommandType.StoredProcedure));
            try
            {
                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    Alert.Show("高值商品备货计划生成成功！", "消息提示", MessageBoxIcon.Information);
                    billOpen(docBILLNO.Text);
                    billLockDoc(true);
                }
                else
                {
                    Alert.Show("高值商品备货计划生成失败！", "错误提示", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("ORA-20001") > -1)
                {
                    string msg = ex.Message.Substring(0, ex.Message.IndexOf("\n"));
                    Alert.Show(msg.Substring(msg.IndexOf("ORA-20001") + 10), "异常信息", MessageBoxIcon.Warning);
                }
                else
                {
                    Alert.Show("异常信息：" + ex.Message, "错误提示", MessageBoxIcon.Error);
                }
            }
        }
        protected void tgbSUPNAME_TriggerClick(object sender, EventArgs e)
        {
            rowID.Text = GridCom.SelectedRowID;
            if (rowID.Text.Length < 1) return;
            String Sql = String.Format("SELECT SUPID,f_getsupname(SUPID) SUPNAME FROM DOC_GOODSSUP WHERE GDSEQ = '{0}'", Doc.GetGridInf(GridCom, rowID.Text, "GDSEQ"));
            DataTable dt = DbHelperOra.Query(Sql).Tables[0];
            if (dt.Rows.Count < 2)
            {
                Alert.Show("只存在一个供应商，不需要选择！", MessageBoxIcon.Warning);
                return;
            }
            ddlSUPID.DataTextField = "SUPNAME";
            ddlSUPID.DataValueField = "SUPID";
            ddlSUPID.DataSource = dt;
            ddlSUPID.DataBind();
            winSup.Hidden = false;
        }

        protected void btnSupSure_Click(object sender, EventArgs e)
        {
            JObject defaultObj = Doc.GetJObject(GridCom, rowID.Text);
            defaultObj["SUPNAME"] = ddlSUPID.SelectedText;
            defaultObj["SUPID"] = ddlSUPID.SelectedValue;
            PageContext.RegisterStartupScript(GridCom.GetUpdateCellValueReference(rowID.Text, defaultObj));
            winSup.Hidden = true;
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            if (e.EventArgument == "GoodsAdd")
            {
                Window1_Close(null, null);
            }
        }

        protected void btnCommit_Click(object sender, EventArgs e)
        {
             
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("非新单不能提交！", "警告提示", MessageBoxIcon.Warning);
                return;
            }

            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "M", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.ExecuteSql("UPDATE DAT_DDPLAN_DOC SET FLAG='N' WHERE FLAG='M' AND BILLNO='" + docBILLNO.Text + "'") > 0)
            {
                billLockDoc(true);
                Alert.Show("单据【" + strBillno + "】提交成功！");
                billOpen(strBillno);

            }
            else
            {
                Alert.Show("单据【" + strBillno + "】提交失败！");

            }
        }
        protected override void billCancel()
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非提交单据不能驳回！", "警告提示", MessageBoxIcon.Warning);
                return;
            }

            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "M", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.ExecuteSql("UPDATE DAT_DDPLAN_DOC SET FLAG='R' WHERE FLAG='N' AND BILLNO='" + docBILLNO.Text + "'") > 0)
            {
                billLockDoc(true);
                Alert.Show("单据【" + strBillno + "】驳回成功！");
                billOpen(strBillno);
            }
        }
    }
}
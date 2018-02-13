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
using System.IO;

namespace ERPProject.ERPApply
{
    public partial class StoreOut : BillBase
    {
        #region Load加载
        private string strDocSql = "SELECT * FROM DAT_CK_DOC WHERE SEQNO ='{0}' AND BILLTYPE = 'CKD'";
        private string strComSql = @"SELECT A.*,B.HISCODE,
                                   F_GETUNITNAME(A.UNIT) UNITNAME,
                                   F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                   F_GETUNITNAME(B.UNIT) UNITSMALLNAME,
                                   DECODE(NVL(A.NUM1, 0), 1, '赠品', '非赠品') NUM1NAME,
                                   case
                                     when a.ph is null then
                                      (select ROUND(sum(nvl(kcsl, 0) - nvl(locksl, 0))/F_GETBZHL(A.GDSEQ),2)
                                         from dat_goodsstock c
                                        where c.gdseq = a.gdseq and c.deptid = '{1}')
                                     else
                                      (select ROUND(sum(nvl(kcsl, 0) - nvl(locksl, 0))/F_GETBZHL(A.GDSEQ),2)
                                         from dat_goodsstock c
                                        where c.gdseq = a.gdseq
                                          and c.phid = a.phid and c.deptid = '{1}')
                                   end kcsl
                              FROM DAT_CK_COM A, DOC_GOODS B
                             WHERE SEQNO = '{0}'
                               AND A.GDSEQ = B.GDSEQ
                             ORDER BY ROWNO";
        protected string Fds_Shtx = "/grf/Fds_Shtx_2.grf";
        protected string cksld = "/grf/cksld.grf";
        public override Field[] LockControl
        {
            get { return new Field[] { docBILLNO, docDEPTID, docXSRQ }; }
        }

        public StoreOut()
        {
            BillType = "CKD";
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
                        ButtonHidden(btnCopy, btnNext, btnBef, btnCancel, btnExport, btnAddRow, btnCommit, btnScan);
                        hdfOper.Text = "input";
                    }
                    else if (Request.QueryString["oper"].ToString() == "audit")
                    {
                        ButtonHidden(btnCommit, btnCopy, btnNext, btnBef, btnNew, btnScan, btnDel, btnAddRow, btnDelRow, btnGoods, btnGoodsZP, btnSave, btnExport);
                        hdfOper.Text = "audit";
                        TabStrip1.ActiveTabIndex = 0;
                    }
                }
                DataInit();
                billNew();
                hfdCurrent.Text = UserAction.UserID;
                btnTemplate.Hidden = true;
                if (docDEPTOUT.SelectedValue == "0434")
                {
                    btnTemplate.Hidden = false;
                }
            }
        }
        private void DataInit()
        {
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1);
            lstLRRQ2.SelectedDate = DateTime.Now;
            PubFunc.DdlDataGet("DDL_USER", docSHR, docLRY, docSLR);
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID, lstDEPTID, ddlDeptid);
            DepartmentBind.BindDDL("DDL_SYS_DEPOTRANGE", UserAction.UserID, docDEPTOUT, ddlDeptOrder);
            PubFunc.DdlDataGet("DDL_BILL_StoreOut", lstFLAG, docFLAG);
            PubFunc.DdlDataGet(ddlReject, "DDL_BILL_REASON");

            docDEPTOUT.Items.RemoveAt(0);
            //是否启用定数标签  By c 2016年1月27日12:07:12 At 威海509
            string DSAUTO = Doc.DbGetSysPara("DSAUTO");
            if (DSAUTO == "Y")
            {
                btnPBQ.Hidden = false;
            }

            //获取客户化GRF文件地址  By c 2016年1月21日12:18:29 At 威海509
            string grf = Doc.DbGetGrf("FDS_SHTXD");
            if (!string.IsNullOrWhiteSpace(grf))
            {
                Fds_Shtx = grf;
            }
        
           
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;

            if (row != null)
            {
                string flag = row["FLAGNAME"].ToString();
                FineUIPro.BoundField flagcol = GridList.FindColumn("FLAGNAME") as FineUIPro.BoundField;
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

            docFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            docSLR.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            docDEPTID.SelectedValue = strDeptID;
            docDEPTOUT.SelectedValue = strDeptOUT;
            billLockDoc(false);
            docMEMO.Enabled = true;
            ckbCKJSY.Enabled = true;
            docDEPTOUT.Enabled = true;
            docSLR.Enabled = true;
            //清空Grid行
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            btnUnit.Enabled = true;
            btnDel.Enabled = false;
            btnSave.Enabled = true;
            btnCommit.Enabled = false;
            btnAudit.Enabled = false;
            btnCancel.Enabled = false;
            btnPrt.Enabled = false;
            btnPrint.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            btnGoodsZP.Enabled = true;

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
            if (!CheckFlag("M") && !CheckFlag("R"))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(GridGoods.SelectedRowID))
            {
                Alert.Show("没有选中任何行，无法进行【删行】操作！", MessageBoxIcon.Warning);
                return;
            }
            GridGoods.DeleteSelectedRows();
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

        private void save(string flag = "N")
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
                    if ((newDict[i]["BZSL"] ?? "").ToString() == "" || Convert.ToInt32(newDict[i]["BZSL"] ?? "0") > Convert.ToInt32(newDict[i]["KCSL"] ?? "0"))
                    {
                        Alert.Show("第" + (i + 1) + "行商品[" + newDict[i]["GDSEQ"] + "]出库数大于库存数！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(newDict[i]["BZHL"].ToString()) || string.IsNullOrWhiteSpace(newDict[i]["UNIT"].ToString()))
                    {
                        Alert.Show("商品[" + newDict[i]["GDSEQ"] + "]包装单位信息错误，请联系管理员维护！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }

                    if ((newDict[i]["ISGZ"] ?? "").ToString() == "Y" && (newDict[i]["PH"] ?? "").ToString() == "")
                    {
                        Alert.Show("高值商品【" + newDict[i]["GDNAME"].ToString() + "】批次不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    //if (string.IsNullOrWhiteSpace(newDict[i]["BZSL"].ToString()))
                    //{
                    //    Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】出库数量不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                    //    return;
                    //}
                    //if (string.IsNullOrWhiteSpace(newDict[i]["HSJJ"].ToString()))
                    //{
                    //    Alert.Show("商品【" + newDict[i]["GDNAME"].ToString() + "】含税进价不能为空！！！", "消息提示", MessageBoxIcon.Warning);
                    //    return;
                    //}
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
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'N') FROM DAT_CK_DOC WHERE SEQNO='{0}'", docBILLNO.Text));
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
            MyTable mtType = new MyTable("DAT_CK_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow["FLAG"] = "M";
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            if (ckbCKJSY.Checked)
            {
                mtType.ColRow.Add("XSTYPE", "3");
            }
            else
            {
                mtType.ColRow.Add("XSTYPE", "1");
            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_CK_COM");
            decimal subNum = 0;//总金额
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_CK_DOC where seqno='" + docBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_CK_COM where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);

                //判断含税进价，为0时不能保存
                string isJiFei = string.Format("select 1 from DOC_GOODS t where iscf = 'N' and gdseq = '{0}'", mtTypeMx.ColRow["GDSEQ"].ToString());
                if (DbHelperOra.Exists(isJiFei))
                {
                    //if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["HSJJ"].ToString()) || mtTypeMx.ColRow["HSJJ"].ToString() == "0")
                    //{
                    //    Alert.Show("商品【含税进价】为0或空，无法进行【库房出库管理】操作。");
                    //    return;
                    //}
                }
                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                //mtTypeMx.ColRow.Add("ROWNO", i + 1);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow["DHSL"] = decimal.Parse(mtTypeMx.ColRow["BZHL"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                mtTypeMx.ColRow["HSJE"] = decimal.Parse(mtTypeMx.ColRow["HSJJ"].ToString()) * decimal.Parse(mtTypeMx.ColRow["BZSL"].ToString());
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                mtTypeMx.ColRow.Add("XSSL", mtTypeMx.ColRow["DHSL"].ToString());
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
                if(flag == "N")
                    Alert.Show("商品出库信息保存成功！", "消息提示", MessageBoxIcon.Information);
                //billLockDoc(true);
                billOpen(docBILLNO.Text);
                OperLog("直接出库", "修改单据【" + docBILLNO.Text.Trim() + "】");
            }
            SaveSuccess = true;
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
            if (!CheckFlag("M") && !CheckFlag("R"))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            DbHelperOra.ExecuteSql("Delete from DAT_CK_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_CK_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            Alert.Show("单据删除成功!");
            OperLog("直接出库", "删除单据【" + docBILLNO.Text.Trim() + "】");
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
        private string strGoodsSql = @"SELECT G.GDSEQ,G.BARCODE,G.GDNAME,G.GDSPEC,G.UNIT,CEIL(Z.SL/G.BZHL) BZSL,CEIL(Z.SL/G.BZHL)*G.BZHL DHSL,CEIL(Z.SL/G.BZHL)*G.BZHL XSSL,
                                                                 (G.HSJJ*DECODE(G.UNIT_SELL,'D',G.NUM_DABZ,'Z',G.NUM_ZHONGBZ,G.BZHL)) HSJJ,CEIL(Z.SL / G.BZHL)*DECODE(G.UNIT_SELL, 'D', G.NUM_DABZ, 'Z', G.NUM_ZHONGBZ, G.BZHL)* (G.HSJJ *
                                                                  DECODE(G.UNIT_SELL, 'D', G.NUM_DABZ, 'Z', G.NUM_ZHONGBZ, G.BZHL)) HSJE,G.ZPBH,P.HJCODE1 HWID,G.PRODUCER,
                                                                  F_GETUNITNAME(G.UNIT) UNITSMALLNAME, DECODE(G.UNIT_SELL,'D',G.NUM_DABZ,'Z',G.NUM_ZHONGBZ,G.BZHL)  BZHL,G.ISLOT,G.ISGZ,
                                                                  F_GETUNITNAME(DECODE(G.UNIT_SELL,'D',G.UNIT_DABZ,'Z',G.UNIT_ZHONGBZ,G.UNIT))  UNITNAME,Z.MEMO,
                                                                  F_GETPRODUCERNAME(G.PRODUCER) PRODUCERNAME,'' FPFLAGNAME,'' MEMO,'' PH,'' RQ_SC,'' YXQZ,G.PIZNO PZWH,G.JXTAX
                                                        FROM DOC_GOODS G,DOC_GOODSCFG P,DOC_GROUPCOM Z WHERE G.GDSEQ=P.GDSEQ AND G.GDSEQ=Z.GDSEQ AND G.flag='Y' and P.DEPTID='{0}' AND Z.GROUPID='{1}' ";

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
                Alert.Show("【开始日期】大于【结束日期】，请重新输入！", MessageBoxIcon.Warning);
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO,B.NAME FLAGNAME,A.FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,A.SUBSUM,
                                     A.SUBNUM,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,A.SHRQ,A.MEMO,F_GETUSERNAME(A.SHR) SHR,
                                 NVL((SELECT FUNCTIME FROM SYS_FUNCPRNNUM WHERE FUNCNO = A.SEQNO),0) PRINTNUM
                                from DAT_CK_DOC A, SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' AND BILLTYPE='CKD' AND XSTYPE IN ('1','3') AND A.FLAG <> 'N'";
            string strSearch = "";


            if (tgbBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND UPPER(TRIM(A.BILLNO))  LIKE '%{0}%'", tgbBILLNO.Text.Trim().ToUpper());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (!PubFunc.StrIsEmpty(lstFLAG.SelectedValue))
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedValue);
            }
            if (Request.QueryString["oper"].ToString() == "audit")
            {
                strSearch += " AND A.FLAG <> 'M'";
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch +=string.Format(" AND F_CHK_AUDIT_SJCK(A.BILLNO,'{0}') = 'Y'", UserAction.UserID);

            strSearch += string.Format(" AND A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.XSRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
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
            if (dtDoc.Rows[0]["XSTYPE"].ToString() == "3")
            {
                ckbCKJSY.Checked = true;
            }
            else
            {
                ckbCKJSY.Checked = false;
            }
            
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
            PubFunc.FormLock(FormDoc, true, "ckbCKJSY");
            if ((",M,R").IndexOf(docFLAG.SelectedValue) > 0 && hdfOper.Text != "audit") docMEMO.Enabled = true;
            TabStrip1.ActiveTabIndex = 1;
            //增加按钮控制
            if (docFLAG.SelectedValue == "M" || docFLAG.SelectedValue == "R")
            {
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnCommit.Enabled = true;
                btnAudit.Enabled = true;
                btnCancel.Enabled = false;
                btnPrt.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
                btnGoodsZP.Enabled = true;
            }
            else if (docFLAG.SelectedValue == "N")
            {
                btnUnit.Enabled = false;
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnAudit.Enabled = true;
                btnCancel.Enabled = true;
                btnPrt.Enabled = false;
                btnPrint.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                btnGoodsZP.Enabled = false;
            }
            else
            {
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnCommit.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrt.Enabled = true;
                btnPrint.Enabled = true;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                btnGoodsZP.Enabled = false;
            }
        }
        private void LoadGridRow(DataRow row, bool firstRow = true, string flag = "NEW")
        {
            if (flag == "NEW")
            {
                string sql = @"SELECT * FROM (SELECT HWID,ROUND(SUM(KCSL - LOCKSL)/F_GETBZHL(GDSEQ),2) KCSL,PH,YXQZ,RQ_SC,SUPID 
                                        FROM DAT_GOODSSTOCK WHERE DEPTID ='{0}' AND GDSEQ = '{1}' AND KCSL > LOCKSL
                                GROUP BY HWID,PH,YXQZ,RQ_SC,GDSEQ,SUPID ORDER BY (CASE WHEN NVL(YXQZ,SYSDATE+30) <=SYSDATE + 30 THEN YXQZ ELSE SYSDATE + 31 END),SUPID,YXQZ) WHERE ROWNUM=1  ";
                if (row["ISLOT"].ToString() != "2")
                {
                    sql = @"SELECT HWID, KCSL - LOCKSL KCSL, '' PH, '' YXQZ, '' RQ_SC
                              FROM (SELECT DEPTID, GDSEQ, HWID, SUM(KCSL) KCSL, SUM(LOCKSL) LOCKSL
                                      FROM DAT_GOODSSTOCK
                                     WHERE DEPTID = '{0}'
                                       AND GDSEQ = '{1}'
                                       AND KCSL > LOCKSL
                                     GROUP BY DEPTID, GDSEQ, HWID)
                              WHERE ROWNUM = 1";
                }
                //写入批次信息,货位
                DataTable Temp = DbHelperOra.Query(string.Format(sql, docDEPTOUT.SelectedValue, row["GDSEQ"].ToString())).Tables[0];
                string ss = string.Format(sql, docDEPTOUT.SelectedValue, row["GDSEQ"].ToString());
                if (Temp.Rows.Count > 0)
                {
                    row["HWID"] = Temp.Rows[0]["HWID"];
                    //row["KCSL"] = Temp.Rows[0]["KCSL"];
                    row["PH"] = Temp.Rows[0]["PH"];
                    row["YXQZ"] = Temp.Rows[0]["YXQZ"];
                    row["RQ_SC"] = Temp.Rows[0]["RQ_SC"];
                    object objKCSL = DbHelperOra.GetSingle(string.Format("SELECT SUM(KCSL-LOCKSL) FROM DAT_GOODSSTOCK WHERE GDSEQ ='{0}' AND DEPTID='{1}'AND PH='{2}'", row["GDSEQ"].ToString(), docDEPTOUT.SelectedValue, Temp.Rows[0]["PH"]));
                    row["KCSL"] = (objKCSL ?? 0).ToString();
                }
                else
                {
                    //货位写入(重新取值)
                    object objHWID = DbHelperOra.GetSingle(string.Format("SELECT F_GETHWID('{0}','{1}') FROM DUAL", docDEPTOUT.SelectedValue, row["GDSEQ"].ToString()));
                    row["HWID"] = (objHWID ?? "").ToString();
                    row["KCSL"] = "0";
                    //row["YXQZ"] = DBNull.Value;
                    //row["RQ_SC"] = DBNull.Value;
                }
            }
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
            string url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTOUT.SelectedValue + "&DeptIn=" + docDEPTID.SelectedValue + "&su=&GoodsState=YT" + "&isgz=N";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
            hdfZP.Text = "";
        }
        //追加赠品
        protected void btnGoodsZP_Click(object sender, EventArgs e)
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            if ((",M,R").IndexOf(docFLAG.SelectedValue) < 0) return;
            PubFunc.FormLock(FormDoc, true, "");
            docMEMO.Enabled = true;
            string url = "~/ERPQuery/GoodsWindow_Zp.aspx?bm=" + docDEPTOUT.SelectedValue + "_" + docDEPTID.SelectedValue + "&cx=&su=";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "赠品商品信息查询"));
            hdfZP.Text = "ZP";
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
                dt.Columns.Add("RQ_SC", Type.GetType("System.DateTime"));
                dt.Columns.Add("YXQZ", Type.GetType("System.DateTime"));
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
                    if (hdfZP.Text == "ZP")
                    {
                        row["NUM1"] = "1";
                        row["NUM1NAME"] = "赠品";
                    }
                    else
                    {
                        row["NUM1"] = "0";
                        row["NUM1NAME"] = "非赠品";
                    }
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
                    JObject defaultObj = Doc.GetJObject(GridGoods, hfdRowIndex.Text);
                    //Dictionary<string, object> newDict = GridGoods.GetNewAddedList()[Convert.ToInt16(hfdRowIndex.Text)];
                    defaultObj["PH"] = row.Values[3].ToString();
                    defaultObj["YXQZ"] = row.Values[4].ToString();
                    defaultObj["PZWH"] = row.Values[9].ToString();
                    defaultObj["RQ_SC"] = row.Values[5].ToString();
                    defaultObj["KCSL"] = row.Values[6].ToString();
                    defaultObj["HSJJ"] = row.Values[8].ToString();
                    defaultObj["BZSL"] = Math.Abs(Convert.ToDecimal(tbxNumber.Text));
                    defaultObj["HSJE"] = Convert.ToDecimal(row.Values[8].ToString()) * (Math.Abs(Convert.ToDecimal(tbxNumber.Text)));
                    defaultObj["DHSL"] = Convert.ToInt32(defaultObj["BZSL"]) * Convert.ToInt32(defaultObj["BZHL"]);
                    if (firstRow)
                    {
                        firstRow = false;
                        PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(hfdRowIndex.Text, defaultObj));
                    }
                    else
                    {
                        PageContext.RegisterStartupScript(GridGoods.GetAddNewRecordReference(defaultObj));
                    }
                }
            }
            WindowLot.Hidden = true;
        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            WindowLot.Hidden = true;
        }
        #endregion
        #region Grid计算

        private void ShowPHWindow(string rowId) {
            JObject defaultObj = Doc.GetJObject(GridGoods, rowId);
            if (defaultObj["GDSEQ"] == null || PubFunc.StrIsEmpty(defaultObj["GDSEQ"].ToString()))
            {
                Alert.Show("请先选择商品信息！", "异常信息", MessageBoxIcon.Warning);
                return;
            }
            if (defaultObj["PH"] == null || PubFunc.StrIsEmpty(defaultObj["PH"].ToString()))
            {
                Alert.Show("请填写批次信息！", "异常信息", MessageBoxIcon.Warning);
                return;
            }
            if (defaultObj["PH"].ToString() == "\\")
            {
                DataTable dtPH = Doc.GetGoodsPH_New(defaultObj["GDSEQ"].ToString(), docDEPTOUT.SelectedValue);
                if (dtPH != null && dtPH.Rows.Count > 0)
                {
                    hfdRowIndex.Text = rowId;
                    GridLot.DataSource = dtPH;
                    GridLot.DataBind();
                    WindowLot.Hidden = false;
                }
                else
                {
                    Alert.Show("此商品已无库存,请检查！", MessageBoxIcon.Warning);
                }
            }
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            if (hdfOper.Text == "audit" || (",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                return;
            }
            string[] strCell = GridGoods.SelectedCell;
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            if (newDict.Count == 0) return;

            //处理返回jobject
            JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
            #region 取消其他计算列
            if (e.ColumnID == "BZSL" || e.ColumnID == "HSJJ")
            {
                decimal hl = 0, rs = 0, jg = 0;

                decimal.TryParse((defaultObj["BZHL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["BZSL"] ?? "0").ToString(), out rs);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["DHSL"] = rs * hl;
                defaultObj["HSJE"] = Math.Round(rs * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));

                //计算合计数量
                decimal bzslTotal = 0, feeTotal = 0, dhslTotal = 0;
                foreach (Dictionary<string, object> dic in newDict)
                {
                    if (dic["BZSL"] != null)
                    {
                        if (!PubFunc.StrIsEmpty(dic["BZSL"].ToString()))
                        {
                            bzslTotal += Convert.ToDecimal(dic["BZSL"]);
                            feeTotal += Convert.ToDecimal(dic["HSJJ"]) * Convert.ToDecimal(dic["BZSL"]);
                            dhslTotal += Convert.ToDecimal(dic["BZHL"]) * Convert.ToDecimal(dic["BZSL"]);
                        }
                    }
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("BZSL", bzslTotal.ToString());
                summary.Add("HSJE", feeTotal.ToString("F4"));
                summary.Add("DHSL", dhslTotal.ToString());
                GridGoods.SummaryData = summary;
            }
            #endregion
            if (e.ColumnID == "PH")
            {
                if (defaultObj["GDSEQ"] == null || PubFunc.StrIsEmpty(defaultObj["GDSEQ"].ToString()))
                {
                    Alert.Show("请先选择商品信息！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                if (defaultObj["PH"] == null || PubFunc.StrIsEmpty(defaultObj["PH"].ToString()))
                {
                    Alert.Show("请填写批次信息！", "异常信息", MessageBoxIcon.Warning);
                    return;
                }
                if (defaultObj["PH"].ToString() == "\\")
                {
                    DataTable dtPH = Doc.GetGoodsPH_New(defaultObj["GDSEQ"].ToString(), docDEPTOUT.SelectedValue);
                    if (dtPH != null && dtPH.Rows.Count > 0)
                    {
                        hfdRowIndex.Text = e.RowID;
                        GridLot.DataSource = dtPH;
                        GridLot.DataBind();
                        WindowLot.Hidden = false;
                    }
                    else
                    {
                        Alert.Show("此商品已无库存,请检查！", MessageBoxIcon.Warning);
                    }
                }
            }
        }
        protected void trbEditorGDSEQ_TriggerClick(object sender, EventArgs e)
        {
            if (hdfOper.Text == "audit" || (",M,R").IndexOf(docFLAG.SelectedValue) < 0)
            {
                return;
            }
            string code = labGDSEQ.Text;
            //string dept = docDEPTID.SelectedValue;
            string dept = docDEPTOUT.SelectedValue;

            if (!string.IsNullOrWhiteSpace(code) && code.Trim().Length >= 2)
            {
                DataTable dt_goods = Doc.GetGoods(code, "", dept);

                if (dt_goods != null && dt_goods.Rows.Count > 0)
                {
                    dt_goods.Columns.Add("BZSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("DHS", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("DHSL", Type.GetType("System.Int32"));
                    dt_goods.Columns.Add("HSJE", Type.GetType("System.Double"));
                    dt_goods.Columns.Add("KCSL", Type.GetType("System.Double"));
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
        #endregion
        #region 单据审核驳回
        private bool SaveSuccess = false;
        protected override void billAudit()
        {

            if (Doc.DbGetSysPara("LOCKSTOCK") == "Y")
            {
                Alert.Show("系统库存已被锁定，请等待物资管理科结存处理完毕再做审核处理！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            //住院办审核
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("单据当前状态为【" + docFLAG.SelectedText + "】，不能进行审核！！！", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            //验证科室是否盘点
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_STOCK WHERE DEPTID IN('" + docDEPTOUT.SelectedValue + "','" + docDEPTID.SelectedValue + "')"))
            {
                Alert.Show("出库或申领科室正在盘点,请检查!", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            string strBillno = docSEQNO.Text;
            //判断单据是否多人操作
            if (!Doc.getFlag(strBillno, "M", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            SaveSuccess = false;
            save("Y");
            if (SaveSuccess == false)
                return;
            SaveSuccess = false;
            object obj = DbHelperOra.GetSingle("SELECT COUNT(1) FROM DAT_CK_COM A WHERE A.SEQNO='" + strBillno + "' AND EXISTS(SELECT 1 FROM  DOC_GOODS WHERE GDSEQ=A.GDSEQ AND CATID='03')");
            object objall = DbHelperOra.GetSingle("SELECT COUNT(1) FROM DAT_CK_COM WHERE SEQNO='" + strBillno + "'");
            if (!PubFunc.Equals(obj.ToString(), "0"))
            {
                if (!PubFunc.Equals(obj.ToString(), objall.ToString()))
                {
                    Alert.Show("单据【" + strBillno + "】同时存在耗材和试剂，请驳回重新申领！");
                    return;
                }
            }
          
             
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"DECLARE
                        BEGIN
                                UPDATE DAT_CK_DOC SET FLAG = 'N' WHERE SEQNO = '{0}' AND FLAG = 'M';
                                IF SQL%ROWCOUNT = 0 THEN
                                    RAISE_APPLICATION_ERROR(-20001, '修改单据[{0}]标志不成功！');
                                END IF;
                                STORE.P_BILLOPER('{0}', '{1}', '{2}', 'AUDIT');
                        END; ", docSEQNO.Text, BillType, UserAction.UserID);
            try
            {
                DbHelperOra.ExecuteSql(sbSql.ToString());
            }
            catch (Exception ex)
            {
                Alert.Show(ERPUtility.errorParse(ex.Message), "错误提示", MessageBoxIcon.Error);
                return;
            }
            billLockDoc(true);
            Alert.Show("单据【" + strBillno + "】审核成功！", "消息提示", MessageBoxIcon.Information);
            billOpen(strBillno);
            OperLog("直接出库", "审核单据【" + docSEQNO.Text + "】");
        }
        protected override void billCancel()
        {
            //将选中单据驳回
            if (!string.IsNullOrWhiteSpace(docSEQNO.Text))
            {
                string flag = DbHelperOra.GetSingle("SELECT FLAG FROM DAT_CK_DOC WHERE SEQNO ='" + docSEQNO.Text + "'").ToString();
                if (!string.IsNullOrWhiteSpace(flag) && (",N").IndexOf(flag) > 0)
                {
                    WindowReject.Hidden = false;
                }
                else
                {
                    Alert.Show("不是已提交单据,不能驳回", "操作提示", MessageBoxIcon.Warning);
                    return;
                }
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
                                       B.BZHL 包装含量,
                                       B.BZSL 申领包装数,
                                       B.XSSL 申领数,B.HSJJ 价格,B.PH 批号,B.RQ_SC 生产日期,B.YXQZ 有效期至
                                  FROM DAT_CK_DOC A, DAT_CK_COM B
                                 WHERE A.SEQNO=B.SEQNO
                                   AND A.BILLTYPE = '" + BillType + @"'
                                   AND A.XSTYPE = '1' ";
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

        protected void btnCommit_Click(object sender, EventArgs e)
        {
            CommitData();
        }
        protected void btnBatchAudit_Click(object sender, EventArgs e)
        {
            string bills = "";//天津批量审核后批量打印，记录批量打印
            if (GridList.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要审核的单据信息！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            if (Doc.DbGetSysPara("LOCKSTOCK") == "Y")
            {
                Alert.Show("系统库存已被锁定，请等待物资管理科结存处理完毕再做审核处理！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            string seqnos = string.Empty;
            string strBill = string.Empty;
            List<CommandInfo> cmdList = new List<CommandInfo>();
            StringBuilder sbSql = new StringBuilder();
            foreach (int index in GridList.SelectedRowIndexArray)
            {
                //验证科室是否盘点
                if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_STOCK WHERE DEPTID IN('" + GridList.Rows[index].DataKeys[3].ToString() + "','" + GridList.Rows[index].DataKeys[2].ToString() + "')"))
                {
                    Alert.Show("第【" + (index + 1).ToString() + "】行单据【" + GridList.Rows[index].DataKeys[0].ToString() + "】出库或申领科室正在盘点,请检查!", "警告提示", MessageBoxIcon.Warning);
                    return;
                }
                //验证单据状态是否正确
                if (GridList.Rows[index].DataKeys[1].ToString() != "M")
                {
                    Alert.Show("第【" + (index + 1).ToString() + "】行单据【" + GridList.Rows[index].DataKeys[0].ToString() + "】当前状态为【" + docFLAG.SelectedText + "】，不能进行审核", "操作提示", MessageBoxIcon.Warning);
                    return;
                }
                seqnos += GridList.Rows[index].DataKeys[0].ToString() + ",";
                //记录需要判断是否盘点的单据
                strBill = strBill + "'" + GridList.Rows[index].DataKeys[0].ToString() + "',";
                sbSql.Length = 0;
                string STRBILL = GridList.Rows[index].DataKeys[0].ToString();
                object obj = DbHelperOra.GetSingle("SELECT COUNT(1) FROM DAT_CK_COM A WHERE A.SEQNO='" + STRBILL + "' AND EXISTS(SELECT 1 FROM  DOC_GOODS WHERE GDSEQ=A.GDSEQ AND CATID='03')");
                object objall = DbHelperOra.GetSingle("SELECT COUNT(1) FROM DAT_CK_COM WHERE SEQNO='" + STRBILL + "'");
                if (!PubFunc.Equals(obj.ToString(), "0"))
                {
                    if (!PubFunc.Equals(obj.ToString(), objall.ToString()))
                    {
                        Alert.Show("单据【" + STRBILL + "】同时存在耗材和试剂，请驳回重新申领！");
                        return;
                    }
                }
             
                sbSql.AppendFormat(@"DECLARE
                        BEGIN
                                UPDATE DAT_CK_DOC SET FLAG = 'N' WHERE SEQNO = '{0}' AND FLAG = 'M';
                                IF SQL%ROWCOUNT = 0 THEN
                                    RAISE_APPLICATION_ERROR(-20001, '修改单据[{0}]标志不成功！');
                                END IF;
                                STORE.P_BILLOPER('{0}', '{1}', '{2}', 'AUDIT');
                        END; ", GridList.Rows[index].DataKeys[0].ToString(), BillType, UserAction.UserID);
                cmdList.Add(new CommandInfo(sbSql.ToString(), null));
                //记录批量审核的单据
                bills += GridList.Rows[index].DataKeys[0].ToString() + "_";
            }
            if (DbHelperOra.Exists("select 1 from  DAT_CK_DOC where deptout in (SELECT deptid FROM DAT_PD_LOCK WHERE  FLAG = 'N') AND SEQNO IN (" + strBill.TrimEnd(',') + ") "))
            {
                Alert.Show("系统库存已被锁定，请等待盘点处理完毕再做审核处理！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            try
            {
                DbHelperOra.ExecuteSqlTran(cmdList);
            }
            catch (Exception ex)
            {
                Alert.Show(ERPUtility.errorParse(ex.Message), "错误提示", MessageBoxIcon.Error);
                bills = "";//如果审核失败清楚需要打印的单据
                return;
            }
            int glselectedcount = GridList.SelectedRowIndexArray.Length;
            string[] seqnoarr = new string[glselectedcount];
            for (int i = 0; i < GridList.SelectedRowIndexArray.Length; i++)
            {
                seqnoarr[i] = GridList.Rows[GridList.SelectedRowIndexArray[i]].DataKeys[0].ToString();
            }
            billSearch();
            AfterPriSelect(seqnoarr);//传递被选中的审批单据号
            Alert.Show("单据批量审核成功！", "消息提示", MessageBoxIcon.Information);

            hfdBills.Text = bills.Trim('_');//录入需要打印的单据
            PageContext.RegisterStartupScript("PrintTXD();");//调用打印
            OperLog("库房直出批量审核", "审核单据【" + seqnos.Trim(',') + "】");
        }
        protected void AfterPriSelect(string[] arrseqno)
        {
            int[] tem = new int[arrseqno.Length];
            for (int i = 0; i < arrseqno.Length; i++)
            {

                for (int j = 0; j < GridList.Rows.Count; j++)
                {
                    if (GridList.Rows[j].DataKeys[0].ToString() == arrseqno[i].ToString())
                    {

                        tem[i] = j;
                        break;
                    }
                }
            }
            GridList.SelectedRowIndexArray = tem;
        }

        protected void btnBatchPrint_Click(object sender, EventArgs e)
        {
            if (GridList.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要打印的单据信息！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            string bills = "";
            foreach (int index in GridList.SelectedRowIndexArray)
            {
                //验证单据状态是否正确
                if ((",Y,G").IndexOf(GridList.Rows[index].DataKeys[1].ToString()) < 1)
                {
                    Alert.Show("第【" + (index + 1).ToString() + "】行单据【" + GridList.Rows[index].DataKeys[0].ToString() + "】当前状态为【" + docFLAG.SelectedText + "】，不允许打印！！！", "操作提示", MessageBoxIcon.Warning);
                    return;
                }
                bills += GridList.Rows[index].DataKeys[0].ToString() + "_";
            }

            hfdBills.Text = bills.Trim('_');
            PageContext.RegisterStartupScript("btnPrint_Bill();");
        }
        protected void btnScan_Click(object sender, EventArgs e)
        {
            //越库商品不允许退货？
            if ((",M,R,N").IndexOf(docFLAG.SelectedValue) < 0)
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
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_CK_COM A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND SEQNO = '{0}'", docSEQNO.Text)))
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
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_CK_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.GDSEQ,A.INSTIME DESC";
            }
            else
            {
                sql = "SELECT A.*,f_getunitname(A.UNIT) UNITNAME FROM DAT_CK_EXT A WHERE A.BILLNO = '{0}' ORDER BY A.INSTIME DESC";
            }
            DataTable dtScan = DbHelperOra.Query(string.Format(sql, docSEQNO.Text)).Tables[0];
            GridSacn.DataSource = dtScan;
            GridSacn.DataBind();
            zsmScan.Text = String.Empty;
            zsmScan.Focus();
        }

        protected void zsmScan_TextChanged(object sender, EventArgs e)
        {
            if ((",M,R,N").IndexOf(docFLAG.SelectedValue) < 0)
            {
                Alert.Show("非『新单』单据不允许操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            //if (zsmScan.Text.Length < Doc.LENCODE()) return;
            //if (zsmScan.Text.Substring(0, 1) != "2")
            //{
            //    Alert.Show("您扫描的条码不是贵重码,请检查！", "提示信息", MessageBoxIcon.Warning);
            //    zsmScan.Text = string.Empty;
            //    zsmScan.Focus();
            //    return;
            //}
            if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_EXT WHERE (ONECODE = '{0}' OR STR1 = '{0}') AND FLAG = 'Y'", zsmScan.Text)))
            {
                Alert.Show("您输入的追溯码未被使用或已退货,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }

            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_CK_EXT WHERE (ONECODE = '{0}' OR STR1 = '{0}') AND BILLNO ='{1}' AND FLAG = 'Y'", zsmScan.Text, docBILLNO.Text)))
            {
                Alert.Show("您输入的追溯码已扫描使用,请检查！", "提示信息", MessageBoxIcon.Warning);
                zsmScan.Text = string.Empty;
                zsmScan.Focus();
                return;
            }

            //写入数据库中
            string sSQL = string.Format(@"INSERT INTO DAT_CK_EXT(DEPTID,BILLNO,ROWNO,ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,PH,RQ_SC,YXQZ,DEPTCUR,BZHL,INSTIME,FLAG,STR1)
                    SELECT '{0}','{1}',NVL((SELECT MAX(ROWNO)+1 FROM DAT_CK_EXT WHERE BILLNO = '{1}'),1),A.ONECODE,GDSEQ,GDNAME,BARCODE,UNIT,GDSPEC,PH,RQ_SC,YXQZ,DEPTCUR,BZHL,SYSDATE,'Y',STR1
                    FROM DAT_RK_EXT A
                    WHERE A.ONECODE = '{2}' OR A.STR1 = '{2}'", docDEPTID.SelectedValue, docBILLNO.Text, zsmScan.Text.Trim());
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
            if (GridSacn.SelectedRowIndexArray.Count() < 1)
            {
                Alert.Show("请选择您需要删除的数据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string onecode = GridSacn.DataKeys[GridSacn.SelectedRowIndexArray[0]][0].ToString();
            DbHelperOra.ExecuteSql(string.Format("DELETE FROM DAT_CK_EXT WHERE ONECODE = '{0}' AND BILLNO = '{1}'", onecode, docBILLNO.Text));
            ScanSearch("");
            //int SelectedIndex = GridGoods.SelectedCell[0];
            //PageContext.RegisterStartupScript(GridGoods.GetDeleteIndexReference(SelectedIndex));
        }


        private bool CommitData()
        {
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                Alert.Show("请保存单据后提交！", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            string flag = DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M')  FROM DAT_CK_DOC WHERE BILLNO='{0}' AND BILLTYPE = 'CKD'", docSEQNO.Text.Trim())).ToString();
            if (("M,R").IndexOf(flag) < 0)
            {
                Alert.Show("非新单，不能提交！", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            if (!CheckFlag("M") && !CheckFlag("R"))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return false;
            }

            string msg = "";
            List<CommandInfo> cmdList = new List<CommandInfo>();
            string sSQL = string.Format("SELECT SUM(ABS(A.DHSL)) DHSL,A.GDSEQ,B.GDNAME FROM DAT_CK_COM A,DOC_GOODS B ,DAT_CK_DOC C WHERE A.SEQNO = C.SEQNO AND A.GDSEQ = B.GDSEQ AND B.ISGZ = 'Y' AND C.SEQNO = '{0}' AND C.BILLTYPE = 'CKD' GROUP BY A.GDSEQ,B.GDNAME", docSEQNO.Text);
            DataTable dtCom = DbHelperOra.Query(sSQL).Tables[0];
            if (dtCom != null && dtCom.Rows.Count > 0)
            {
                foreach (DataRow dr in dtCom.Rows)
                {
                    string checkCount = DbHelperOra.GetSingle(string.Format("SELECT COUNT(*) FROM DAT_CK_EXT WHERE BILLNO='{0}' AND GDSEQ ='{1}'", docSEQNO.Text, dr["GDSEQ"].ToString())).ToString();
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

                string sSQL1 = "CALL P_EXE_CKD('" + docSEQNO.Text + "')";
                cmdList.Add(new CommandInfo(sSQL1, null));
            }

            string sSQL2 = string.Format("UPDATE DAT_CK_DOC SET FLAG = 'N',SPR='{0}',SPRQ=sysdate WHERE BILLTYPE = 'CKD' AND SEQNO = '{1}'", UserAction.UserID, docSEQNO.Text);
            cmdList.Add(new CommandInfo(sSQL2, null));

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                billOpen(docSEQNO.Text);
                Alert.Show("单据【" + docSEQNO.Text + "】提交成功！");
                docFLAG.SelectedValue = "N";
                OperLog("直接出库", "提交单据【" + docSEQNO.Text + "】");
                btnUnit.Enabled = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void btnRejectSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(docSEQNO.Text.Trim()))
            {
                string flag = DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M')  FROM DAT_CK_DOC WHERE BILLNO='{0}'", docSEQNO.Text.Trim())).ToString();

                if (flag == "Y")
                {
                    Alert.Show("已审核单据,不能驳回", "操作提示", MessageBoxIcon.Warning);
                    return;
                }
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

                string sql = "update DAT_CK_DOC set flag='R',SHRQ=sysdate,memo ='" + strMemo + "',SHR='" + UserAction.UserID + "' where seqno ='" + docSEQNO.Text + "'";
                DbHelperOra.ExecuteSql(sql);
                WindowReject.Hidden = true;
                billSearch();
                docMEMO.Text = strMemo;
                docFLAG.SelectedValue = "R";
            }
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

        protected void btnUnit_Click(object sender, EventArgs e)
        {
            string[] strCell = GridGoods.SelectedRowIDArray;
            if (strCell.Length < 1)
            {
                Alert.Show("请选择行后操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            hdfSelctIndex.Text = strCell[0];
            string getgdseq = Doc.GetGridInf(GridGoods, strCell[0].ToString(), "GDSEQ");
            string strUnit = string.Format(@"SELECT 1
                                              FROM DOC_GOODS A
                                             WHERE A.GDSEQ = '{0}'
                                               AND DECODE(A.UNIT_SELL, 'D', A.UNIT_DABZ, 'Z', A.UNIT_ZHONGBZ, A.UNIT) = A.UNIT", getgdseq);
            bool isUnit = DbHelperOra.Exists(strUnit);

            if (isUnit)
            {
                Alert.Show("出库单位为【最小包装单位】，无法更改！");
                return;
            }
            string strSql = string.Format(@"SELECT GDSEQ,GDNAME, UNIT, F_GETUNITNAME(UNIT) UNITNAME, BZHL,HSJJ
                                              FROM DOC_GOODS
                                             WHERE GDSEQ = '{0}'
                                            UNION ALL
                                            SELECT A.GDSEQ,A.GDNAME,
                                                   DECODE(A.UNIT_SELL, 'D', A.UNIT_DABZ, 'Z', A.UNIT_ZHONGBZ, A.UNIT) UNIT,
                                                   F_GETUNITNAME(DECODE(A.UNIT_SELL,
                                                                        'D',
                                                                        A.UNIT_DABZ,
                                                                        'Z',
                                                                        A.UNIT_ZHONGBZ,
                                                                        A.UNIT)) UNITNAME,
                                                   DECODE(A.UNIT_SELL, 'D', A.NUM_DABZ, 'Z', A.NUM_ZHONGBZ, A.UNIT) BZHL,DECODE(A.UNIT_SELL, 'D', A.NUM_DABZ, 'Z', A.NUM_ZHONGBZ, A.UNIT)*A.HSJJ
                                              FROM DOC_GOODS A
                                             WHERE A.GDSEQ = '{0}'", getgdseq);
            GridUnit.DataSource = DbHelperOra.Query(strSql);
            GridUnit.DataBind();
            WindowUnit.Hidden = false;
        }

        protected void btnUnitSubmit_Click(object sender, EventArgs e)
        {

            int rowIndex = GridUnit.SelectedRowIndex;
            if (rowIndex < 0)
            {
                Alert.Show("未选择任何行，无法执行【确定】操作");
                return;
            }
            //int pageIndex = Convert.ToInt32(hdfSelctIndex.Text.ToString());
            JObject newDict = Doc.GetJObject(GridGoods, hdfSelctIndex.Text);

            string strUnit = newDict["UNITNAME"].ToString();
            newDict["UNIT"] = GridUnit.DataKeys[rowIndex][0].ToString();
            newDict["UNITNAME"] = GridUnit.DataKeys[rowIndex][1].ToString();
            newDict["BZHL"] = GridUnit.DataKeys[rowIndex][2].ToString();
            newDict["BZSL"] = "0";
            newDict["HSJE"] = "0";
            newDict["HSJJ"] = GridUnit.DataKeys[rowIndex][3].ToString();
            //newDict["MEMO"] += " 默认单位【"+strUnit+"】修改为【"+ GridUnit.DataKeys[rowIndex][1] + "】";

            PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(hdfSelctIndex.Text, newDict));
            WindowUnit.Hidden = true;
        }

        protected void btnUnitClose_Click(object sender, EventArgs e)
        {
            WindowUnit.Hidden = true;
        }

        protected void GridUnit_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            int rowIndex = GridUnit.SelectedRowIndex;
            if (rowIndex < 0)
            {
                Alert.Show("未选择任何行，无法执行【确定】操作");
                return;
            }
            //int pageIndex = Convert.ToInt32(hdfSelctIndex.Text.ToString());
            JObject newDict = Doc.GetJObject(GridGoods, hdfSelctIndex.Text);

            string strUnit = newDict["UNITNAME"].ToString();
            newDict["UNIT"] = GridUnit.DataKeys[rowIndex][0].ToString();
            newDict["UNITNAME"] = GridUnit.DataKeys[rowIndex][1].ToString();
            newDict["BZHL"] = GridUnit.DataKeys[rowIndex][2].ToString();
            newDict["BZSL"] = "0";
            newDict["HSJE"] = "0";
            newDict["HSJJ"] = GridUnit.DataKeys[rowIndex][3].ToString();
            //newDict["MEMO"] += " 默认单位【"+strUnit+"】修改为【"+ GridUnit.DataKeys[rowIndex][1] + "】";

            PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(hdfSelctIndex.Text, newDict));
            WindowUnit.Hidden = true;
        }

        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            if (e.EventArgument.IndexOf("GoodsAdd") >= 0)
            {
                Window1_Close(null, null);
            }
            if (e.EventArgument.IndexOf("PHWindow$") >= 0) {
                string rowId = e.EventArgument.Split('$')[1];
                ShowPHWindow(rowId);
            }
        }

        protected void btn_Auto_Click(object sender, EventArgs e)
        {
            //高值自动订货
            WinAuto.Hidden = false;
            if (dbpOrder1.Text.Length < 1 && dbpOrder2.Text.Length < 1)
            {
                dbpOrder1.SelectedDate = DateTime.Now.AddMonths(-1);
                dbpOrder2.SelectedDate = DateTime.Now;
            }
        }
        protected void rblTYPE_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rblTYPE.SelectedValue == "XS")
            {
                dbpOrder1.Enabled = true;
                dbpOrder2.Enabled = true;
                memo.Text = "出库量 = 销售期间的销售量 - 科室库存 - 在途库存";
                if (dbpOrder1.Text.Length < 1 && dbpOrder2.Text.Length < 1)
                {
                    dbpOrder1.SelectedDate = DateTime.Now.AddMonths(-1);
                    dbpOrder2.SelectedDate = DateTime.Now;
                }
            }
            else
            {
                dbpOrder1.Enabled = false;
                dbpOrder2.Enabled = false;
                memo.Text = "出库量 = 最高库存 - 库房库存 - 在途库存";
            }
        }
        protected void btnSure_Click(object sender, EventArgs e)
        {
            //生成出库单
            if (ddlDeptOrder.SelectedValue.Length < 1)
            {
                Alert.Show("请选择【出库库房】！", MessageBoxIcon.Warning);
                return;
            }
            if (ddlDeptid.SelectedValue.Length < 1)
            {
                Alert.Show("请选择【入库科室】！", MessageBoxIcon.Warning);
                return;
            }
            if (rblTYPE.SelectedValue == "XS")
            {
                if (dbpOrder1.Text.Length < 1 || dbpOrder2.Text.Length < 1)
                {
                    Alert.Show("【销售日期】未维护完全，请检查！", MessageBoxIcon.Warning);
                    return;
                }
            }
            OracleParameter[] parameters = new OracleParameter[]
                            {
                                     new OracleParameter("VIN_DEPTOUT",OracleDbType.Varchar2,20),
                                     new OracleParameter("VIN_DEPTID",OracleDbType.Varchar2,400),
                                     new OracleParameter("VIN_TYPE",OracleDbType.Varchar2,2),
                                     new OracleParameter("VIN_OPERUSER",OracleDbType.Varchar2,20),
                                     new OracleParameter("VIN_TIME1",OracleDbType.Varchar2,10),
                                     new OracleParameter("VIN_TIME2",OracleDbType.Varchar2,10),
                                     new OracleParameter("VO_BILLNO",OracleDbType.Varchar2,20)
                            };
            parameters[0].Value = ddlDeptOrder.SelectedValue;
            parameters[1].Value = ddlDeptid.SelectedValue;
            parameters[2].Value = rblTYPE.SelectedValue;
            parameters[3].Value = UserAction.UserID;
            parameters[4].Value = dbpOrder1.Text;
            parameters[5].Value = dbpOrder2.Text;
            parameters[0].Direction = ParameterDirection.Input;
            parameters[1].Direction = ParameterDirection.Input;
            parameters[2].Direction = ParameterDirection.Input;
            parameters[3].Direction = ParameterDirection.Input;
            parameters[4].Direction = ParameterDirection.Input;
            parameters[5].Direction = ParameterDirection.Input;
            parameters[6].Direction = ParameterDirection.Output;
            DbHelperOra.RunProcedure("STOREDS.P_AUTOCK", parameters);
            if (parameters[6].Value.ToString() != "#" && parameters[6].Value.ToString().Length > 0)
            {
                Alert.Show("自动生成成功！");
                billOpen(parameters[6].Value.ToString());
                WinAuto.Hidden = true;
            }
            else
            {
                Alert.Show("科室[" + ddlDeptid.SelectedText + "]无需要出库的信息！", MessageBoxIcon.Warning);
            }
        }

        protected void btn_close_Click(object sender, EventArgs e)
        {
            WinAuto.Hidden = true;
        }
        protected void btnPrint_Click(object sender, EventArgs e)
        {
            if ((",Y,G").IndexOf(docFLAG.SelectedValue) < 1)
            {
                Alert.Show("选择单据未审核,不允许打印！", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            hfdBills.Text = docBILLNO.Text; 
            PageContext.RegisterStartupScript("btnPrint_Bill();");
        }
        protected void btnSaveTemplate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
            {
                Alert.Show("请选择要保存模板的科室", "警告提醒", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
            if (newDict.Count > 0)
            {
                Window3.Hidden = false;
            }
            else
            {
                Alert.Show("保存模板之前请先添加商品明细信息！", "警告提醒", MessageBoxIcon.Warning);
            }
        }
        protected void btnLoadTemplate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
            {
                Alert.Show("请选择要载入模板的科室", "警告提醒", MessageBoxIcon.Warning);
                return;
            }

            string sql = @"SELECT GROUPID, GROUPNAME, F_GETUSERNAME(LRY) USERNAME
                                  FROM DOC_GROUPDOC WHERE FLAG='Y' AND DEPTID = '{0}'AND (TYPE='K'OR TYPE='Y') ORDER BY GROUPNAME";
            GridTemplate.DataSource = DbHelperOra.Query(string.Format(sql, docDEPTID.SelectedValue)).Tables[0];
            GridTemplate.DataBind();

            Window4.Hidden = false;
        }
        protected void btnSaveTemplateClose_Click(object sender, EventArgs e)
        {
            FineUIPro.Button btn = sender as FineUIPro.Button;
            if (btn.ID == "btnSaveTemplateClose")
            {
                if (string.IsNullOrWhiteSpace(tbsFileName.Text))
                {
                    Alert.Show("请输入模板名称！", "警告提醒", MessageBoxIcon.Warning);
                    return;
                }

                List<CommandInfo> cmdList = new List<CommandInfo>();
                MyTable doc = new MyTable("DOC_GROUPDOC");
                object maxid = DbHelperOra.GetSingle("SELECT MAX(GROUPID) FROM DOC_GROUPDOC WHERE GROUPID LIKE 'ZU%'");
                if (maxid == null || maxid.ToString() == "")
                {
                    doc.ColRow["GROUPID"] = "ZU00001";
                }
                else
                {
                    doc.ColRow["GROUPID"] = "ZU" + (100001 + int.Parse(maxid.ToString().Substring(2))).ToString().Substring(1);
                }
                doc.ColRow["GROUPNAME"] = tbsFileName.Text;
                doc.ColRow["FLAG"] = "Y";
                doc.ColRow["TYPE"] = "Y";
                doc.ColRow["DEPTID"] = docDEPTID.SelectedValue;
                doc.ColRow["CATID"] = "2";
                doc.ColRow["LRY"] = UserAction.UserID;
                doc.ColRow.Remove("LRRQ");
                List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
                for (int i = 0; i < newDict.Count; i++)
                {
                    MyTable com = new MyTable("DOC_GROUPCOM");
                    com.ColRow["GROUPID"] = doc.ColRow["GROUPID"];
                    com.ColRow["ROWNO"] = i + 1;
                    com.ColRow["GDSEQ"] = newDict[i]["GDSEQ"].ToString();
                    com.ColRow["GDNAME"] = newDict[i]["GDNAME"].ToString();
                    com.ColRow["GDSPEC"] = newDict[i]["GDSPEC"].ToString();
                    com.ColRow["BZHL"] = newDict[i]["BZHL"].ToString();
                    com.ColRow["UNIT"] = newDict[i]["UNIT"].ToString();
                    com.ColRow["SL"] = decimal.Parse(newDict[i]["BZHL"].ToString()) * decimal.Parse(newDict[i]["BZSL"].ToString());
                    com.ColRow["HSJJ"] = newDict[i]["HSJJ"].ToString();
                    com.ColRow["MEMO"] = newDict[i]["MEMO"].ToString();
                    cmdList.Add(com.Insert());
                }
                doc.ColRow["SUBNUM"] = newDict.Count;
                cmdList.Add(doc.Insert());

                DbHelperOra.ExecuteSqlTran(cmdList);
                Window3.Hidden = true;
            }
            else if (btn.ID == "btnLoadTemplateClose")
            {
                if (GridTemplate.SelectedRowIndex < 0)
                {
                    Alert.Show("请选择要加载的模板！", "警告提醒", MessageBoxIcon.Warning);
                    return;
                }

                hfdTemplateName.Text = GridTemplate.Rows[GridTemplate.SelectedRowIndex].DataKeys[1].ToString();
                StringBuilder sbSql = new StringBuilder();
                sbSql.AppendFormat(strGoodsSql, docDEPTID.SelectedValue, GridTemplate.Rows[GridTemplate.SelectedRowIndex].DataKeys[0].ToString());
                DataTable dtGoods = DbHelperOra.Query(sbSql.ToString()).Tables[0];
                if (dtGoods != null && dtGoods.Rows.Count > 0)
                {
                    foreach (DataRow row in dtGoods.Rows)
                    {
                        PubFunc.GridRowAdd(GridGoods, row, false);
                    }
                    Window2.Hidden = true;
                }
                else
                {
                    string file = GridTemplate.Rows[GridTemplate.SelectedRowIndex].DataKeys[1].ToString();
                    Alert.Show("模板【" + file + "】内容为空或模版中商品被取消配置！", "警告提醒", MessageBoxIcon.Warning);
                }
            }
        }
        protected void GridTemplate_RowCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "FileDelete")
            {
                DbHelperOra.ExecuteSql("UPDATE DOC_GROUPDOC SET FLAG='N' WHERE GROUPID='" + GridTemplate.Rows[e.RowIndex].DataKeys[0].ToString() + "'");
                GridTemplateLoad();
            }
        }
        private void GridTemplateLoad()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("TEMPLATE", typeof(string)));
            table.Columns.Add(new DataColumn("TEMPLATENAME", typeof(string)));
            table.Columns.Add(new DataColumn("USERNAME", typeof(string)));

            string strPath = AppDomain.CurrentDomain.BaseDirectory + "ERPUpload/Json/";
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            string[] dirs = Directory.GetFiles(strPath);
            foreach (string dir in dirs)
            {
                string dirName = dir.Substring(dir.LastIndexOf('/') + 1);
                if (dirName.StartsWith(docDEPTID.SelectedValue + "_"))
                {
                    string fileName = dirName.Substring(0, dirName.IndexOf('.'));
                    string[] arrDir = fileName.Split('_');
                    if (arrDir.Length < 5) continue;
                    if (arrDir[3] == "2" && arrDir[4] == docDEPTOUT.SelectedValue)
                    {
                        DataRow row = table.NewRow();
                        row[0] = dir;
                        row[1] = arrDir[2];
                        row[2] = docLRY.Items.FindByValue(arrDir[1]).Text;
                        table.Rows.Add(row);
                    }
                }
            }
            GridTemplate.DataSource = table;
            GridTemplate.DataBind();
        }
        protected void GridTemplate_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            hfdTemplateName.Text = GridTemplate.Rows[e.RowIndex].DataKeys[1].ToString();
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(strGoodsSql, docDEPTID.SelectedValue, GridTemplate.Rows[e.RowIndex].DataKeys[0].ToString());
            DataTable dtGoods = DbHelperOra.Query(sbSql.ToString()).Tables[0];
            if (dtGoods != null && dtGoods.Rows.Count > 0)
            {
                foreach (DataRow row in dtGoods.Rows)
                {
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

                    PageContext.RegisterStartupScript(GridGoods.GetAddNewRecordReference(defaultObj, true));
                }
                Window3.Hidden = true;
            }
            else
            {
                string file = GridTemplate.Rows[GridTemplate.SelectedRowIndex].DataKeys[1].ToString();
                Alert.Show("模板【" + file + "】内容为空或模版中商品被取消配置！", "警告提醒", MessageBoxIcon.Warning);
            }
        }

        protected void docDEPTOUT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (docDEPTOUT.SelectedValue == "0434")
            {
                btnTemplate.Hidden = false;
            }
            else {
                btnTemplate.Hidden = true;

            }
        }
    }
}
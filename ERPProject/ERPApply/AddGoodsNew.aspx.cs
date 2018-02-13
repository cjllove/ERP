using FineUIPro;
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

namespace ERPProject.ERPApply
{
    public partial class AddGoodsNew : BillBase
    {
        private string strDocSql = "SELECT * FROM DAT_GOODSNEW_DOC WHERE SEQNO ='{0}'";
        private string strComSql = "SELECT * FROM DAT_GOODSNEW_COM WHERE SEQNO = '{0}'";
        private string strFlagSql = @"SELECT '' CODE ,'--请选择--' NAME  FROM dual
union all ";
        public AddGoodsNew()
        {
            BillType = "SXD";//商品新增单
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                billNew();

                //屏蔽不需要的操作按钮
                if (Request.QueryString["oper"] != null)
                {
                    string oper = Request.QueryString["oper"].ToString();
                    hfdOper.Text = oper;
                    if (oper == "sq")
                    {
                        ButtonHidden(btnSaveFa, btnAudit, btnTJFA, btnCancel, btnDelete, btnPrint, btnExport, btnCopy);
                        TabStrip1.ActiveTabIndex = 1;
                        strFlagSql += @"SELECT 'M' CODE ,'新单' NAME  FROM dual
                                        union all 
                                        SELECT 'N' CODE ,'需求提交' NAME  FROM dual  
                                        union all
                                        SELECT 'Y' CODE ,'已审批' NAME  FROM dual
                                        union all
                                        SELECT 'F' CODE ,'已废弃' NAME  FROM DUAL ";

                        PubFunc.DdlDataSql(lstFLAG, strFlagSql);

                    }
                    else if (oper == "fa")
                    {
                        ButtonHidden(btnNew, btnDel, btnSave, btnTJ, btnAudit, btnAddRow, btnCancel, btnDelRow, btnExport, btnCopy);
                        btnPrint.Hidden = false;
                        TabStrip1.ActiveTabIndex = 0;
                        ddlSPR.SelectedValue = UserAction.UserID;
                        dpkSPRQ.SelectedDate = DateTime.Now;
                        billSearch();

                        strFlagSql += @"SELECT 'N' CODE ,'需求提交' NAME  FROM dual
                                        union all 
                                        SELECT 'S' CODE ,'方案提交' NAME  FROM dual
                                        union all
                                        SELECT 'Y' CODE ,'已审批' NAME  FROM dual
                                        union all
                                        SELECT 'R' CODE ,'已驳回' NAME  FROM DUAL
                                        union all
                                        SELECT 'F' CODE ,'已废弃' NAME  FROM DUAL  ";

                        PubFunc.DdlDataSql(lstFLAG, strFlagSql);
                    }
                    else if (oper == "sp")
                    {
                        ButtonHidden(btnCancel, btnNew, btnDel, btnSave, btnSaveFa, btnTJ, btnTJFA, btnAddRow, btnDelRow, btnDelete, btnPrint, btnExport, btnCopy);
                        TabStrip1.ActiveTabIndex = 0;
                        ddlSHR.SelectedValue = UserAction.UserID;
                        dpkSHRQ.SelectedDate = DateTime.Now;
                        billSearch();

                        strFlagSql += @"SELECT 'S' CODE ,'方案提交' NAME  FROM dual
                                        union all
                                        SELECT 'Y' CODE ,'已审批' NAME  FROM dual
                                        union all
                                        SELECT 'R' CODE ,'已驳回' NAME  FROM DUAL  ";

                        PubFunc.DdlDataSql(lstFLAG, strFlagSql);
                    }
                }
                JObject defaultObj = new JObject();
                defaultObj.Add("GDNAME", "");
                defaultObj.Add("GDSPEC", "");
                defaultObj.Add("UNIT", "");
                defaultObj.Add("SL", "0");
                defaultObj.Add("HSJJ", "");
                defaultObj.Add("HSJE", "");
                defaultObj.Add("PRODUCER", "");
                defaultObj.Add("MEMOGOODS", "");
                defaultObj.Add("ISNEW", "");
                defaultObj.Add("MEMOBUY", "");
                defaultObj.Add("ISPASS", "");
                defaultObj.Add("MEMOPASS", "");

                // 第一行新增一条数据
                btnAddRow.OnClientClick = GridGoods.GetAddNewRecordReference(defaultObj);
            }
        }

        private void DataInit()
        {
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;

            PubFunc.DdlDataGet(docLRY, "DDL_USER");
            PubFunc.DdlDataGet(ddlSPR, "DDL_USER");
            PubFunc.DdlDataGet(ddlSHR, "DDL_USER");
            //PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDEPTID, lstDEPTID);
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDEPTID, lstDEPTID);
            PubFunc.DdlDataGet(ddlFLAG, "DDL_ADDGOODSNEW");
            PubFunc.DdlDataGet(lstFLAG, "DDL_ADDGOODSNEW");

        }

        protected override void billNew()
        {
            //原单据保存判断
            string strDept = ddlDEPTID.SelectedValue;
            PubFunc.FormDataClear(FormDoc);

            ddlFLAG.SelectedValue = "M";
            docLRY.SelectedValue = UserAction.UserID;
            dpkLRRQ.SelectedDate = DateTime.Now;
            ddlDEPTID.SelectedValue = strDept;
            ddlDEPTID.SelectedIndex = 1;
            billLockDoc(false);
            //btnAudit.Enabled = false;
            //btnSave.Enabled = true;
            //btnPrint.Enabled = false;
            //btnDelRow.Enabled = true;

            btnAddRow.Enabled = true;
            btnDelRow.Enabled = true;
            btnSave.Enabled = true;

            ddlDEPTID.Enabled = true;
            tbxLRYTEL.Enabled = true;
            tbxMEMO.Enabled = true;
            tbxBILLNO.Enabled = false;
            //docSUPID.Enabled = true;
            //清空Grid行

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
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

            decimal rs = 0, jg = 0;
            string strSL = "0";
            string strHSJJ = "0";
            if (dicRecord["SL"] != null && !string.IsNullOrWhiteSpace(dicRecord["SL"].ToString()))
            {
                strSL = dicRecord["SL"].ToString();
            }
            if (dicRecord["HSJJ"] != null && !string.IsNullOrWhiteSpace(dicRecord["HSJJ"].ToString()))
            {
                strHSJJ = dicRecord["HSJJ"].ToString();
            }
            decimal.TryParse(strSL, out rs);//订货数
            decimal.TryParse(strHSJJ, out jg);//价格

            defaultObj.Remove("HSJE");

            //处理金额格式
            string jingdu = Math.Round(rs * jg, 2).ToString("F2");
            defaultObj.Add("HSJE", jingdu);

            return defaultObj;
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            if (e.ColumnID == "HSJJ")
            {
                JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
                decimal hl = 0, jg = 0;
                decimal.TryParse((defaultObj["SL"] ?? "0").ToString(), out hl);
                decimal.TryParse((defaultObj["HSJJ"] ?? "0").ToString(), out jg);
                defaultObj["HSJE"] = Math.Round(hl * jg, 2).ToString("F2");
                PageContext.RegisterStartupScript(GridGoods.GetUpdateCellValueReference(e.RowID, defaultObj));
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
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            if (ddlFLAG.SelectedValue != "M")
            {
                Alert.Show("非『新增』单据不能增行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            billLockDoc(true);
            //PubFunc.GridRowAdd(GridGoods, "INIT");


        }

        protected override void billDelRow()
        {
            if (ddlFLAG.SelectedValue != "M")
            {
                Alert.Show("非『新增』单据不能删行！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridGoods.SelectedRowID == null)
            {
                Alert.Show("当前没有选中行，无法进行【删行】操作", "操作警告", MessageBoxIcon.Warning);
                return;
            }
            GridGoods.DeleteSelectedRows();
        }

        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            PubFunc.FormLock(FormDoc, true, "");
            //参数说明：cx-查询内容，bm-商品配置部门,su-供应商
            string url = "~/ERPQuery/GoodsWindow_Gather.aspx?bm=" + ddlDEPTID.SelectedValue + "&cx=&su=";
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

            string strSql = @"SELECT A.SEQNO,
                               A.BILLNO,
                               --f_getbillflag(a.flag) flag,
                               a.flag,
                               decode(a.flag,'M','新单','N','需求提交','S','方案提交','Y','已审批','R','已驳回','F','已废弃') flagname,
                               F_GETDEPTNAME(A.DEPTID) DEPTID,
                               A.SUBNUM,
                               A.SUBSUM,
                               F_GETUSERNAME(A.LRY) LRY,
                               A.LRRQ,
                               F_GETUSERNAME(A.SPR) SPR,
                               A.SPRQ,
                               F_GETUSERNAME(A.SHR) SHR,
                               A.SHRQ,
                               A.LRYTEL,
                               A.MEMO
                          from DAT_GOODSNEW_DOC A
                         WHERE BILLTYPE = 'SXD'";
            string strSearch = "";


            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND TRIM(UPPER(A.BILLNO)) LIKE '%{0}%'", lstBILLNO.Text.Trim().ToUpper());
            }
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (!string.IsNullOrWhiteSpace(lstFLAG.SelectedValue))
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedValue);
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            //
            if (Request.QueryString["oper"] != null)
            {
                string oper = Request.QueryString["oper"].ToString();
                hfdOper.Text = oper;
                if (oper == "sq")
                {
                    strSearch += " AND A.FLAG IN( 'M','N','G','Y','F' )";
                }
                else if (oper == "fa")
                {
                    strSearch += " AND A.FLAG IN( 'Y', 'N','S','R' )";
                }
                else if (oper == "sp")
                {
                    strSearch += " AND A.FLAG IN( 'S', 'Y','R' )";
                }

            }
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY DECODE(A.FLAG,'M','1','R','2','N','3','S','4','5'),A.BILLNO DESC";
            highlightRowYellow.Text = "";
            highlightRowRed.Text = "";
            highlightRowGreen.Text = "";
            GridList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataBind();
        }

        protected override void billAudit()
        {
            #region 数据有效性验证
            if (ddlFLAG.SelectedValue == "Y")
            {
                Alert.Show("本条使用信息已经审核确认，不需要再次审核！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["MEMOGOODS"]).ToList();
            //if (newDict.Count == 0)
            //{
            //    Alert.Show("请输入【审批意见】", "消息提示", MessageBoxIcon.Warning);
            //    return;
            //}            
            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();

            #endregion

            MyTable mtType = new MyTable("DAT_GOODSNEW_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["FLAG"] = "Y";
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow["SHR"] = UserAction.UserID;
            mtType.ColRow["SHRQ"] = dpkSHRQ.SelectedDate;
            //mtType.ColRow.Add("SUBNUM", goodsData.Count);

            int isNum = 0;
            string strSUBSUM = "";
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                if (newDict[i]["ISPASS"] != null && newDict[i]["ISPASS"].ToString() == "R")
                {
                    if (string.IsNullOrWhiteSpace(newDict[i]["MEMOPASS"].ToString()))
                    {
                        Alert.Show("第【" + (i + 1) + "】行请填写【审批意见】", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    isNum++;
                }
                else if (newDict[i]["ISPASS"] != null && newDict[i]["ISPASS"].ToString() == "Y")
                {
                    newDict[i]["MEMOPASS"] = "";
                }
                strSUBSUM += newDict[i]["HSJE"];
                goodsData.Add(newDict[i]);

            }
            if (isNum > 0)
            {
                mtType.ColRow["FLAG"] = "R";
            }

            mtType.ColRow["SUBNUM"] = goodsData.Count;
            mtType.ColRow["SUBSUM"] = strSUBSUM;

            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_GOODSNEW_COM");
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_GOODSNEW_DOC where seqno='" + tbxBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_GOODSNEW_COM where seqno='" + tbxBILLNO.Text + "'", null));//删除单据明细
            cmdList.AddRange(mtType.InsertCommand());
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);


                if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["MEMOGOODS"].ToString()))
                {
                    Alert.Show("【需求说明】为空，无法进行【商品新增管理】操作。");
                    return;
                }

                mtTypeMx.ColRow.Add("SEQNO", tbxBILLNO.Text);
                mtTypeMx.ColRow.Add("ROWNO", i + 1);

                cmdList.Add(mtTypeMx.Insert());
            }

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("商品新增信息审核成功！");
                OperLog("商品新增管理", "修改单据【" + tbxBILLNO.Text + "】");
                billOpen(tbxBILLNO.Text);
            }

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

            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno)).Tables[0];

            if (Request.QueryString["oper"] != null)
            {
                string oper = Request.QueryString["oper"].ToString();
                hfdOper.Text = oper;
                if (oper == "fa")
                {
                    foreach (DataRow dr in dtBill.Rows)
                    {
                        if (string.IsNullOrWhiteSpace(dr["ISNEW"].ToString()))
                        {
                            dr["ISNEW"] = "Y";
                            dr["HSJJ"] = 0;
                        }
                    }
                }
                if (oper == "sp")
                {
                    foreach (DataRow dr in dtBill.Rows)
                    {
                        if (string.IsNullOrWhiteSpace(dr["ISPASS"].ToString()))
                        {
                            dr["ISPASS"] = "Y";
                        }
                    }
                }
            }

            Doc.GridRowAdd(GridGoods, dtBill);
            ////计算合计数量
            //JObject summary = new JObject();
            //summary.Add("GDNAME", "本页合计");
            //summary.Add("SL", bzslTotal.ToString());
            //summary.Add("HSJE", feeTotal.ToString("F2"));
            //GridGoods.SummaryData = summary;

            PubFunc.FormLock(FormDoc, true, "");
            TabStrip1.ActiveTabIndex = 1;
            //屏蔽不需要的操作按钮
            btnPrint.Enabled = false;
            if ((",N,R").IndexOf(ddlFLAG.SelectedValue) > 0)
            {
                btnSaveFa.Enabled = true;
                btnTJFA.Enabled = true;
                btnAddRow.Enabled = false;
                btnDelRow.Enabled = false;
                btnSave.Enabled = false;

            }
            if (ddlFLAG.SelectedValue == "M")
            {
                btnAddRow.Enabled = true;
                btnDelRow.Enabled = true;
                btnSave.Enabled = true;
            }
            else if (ddlFLAG.SelectedValue == "Y")
            {
                btnSaveFa.Enabled = false;
                btnTJFA.Enabled = false;
                btnAddRow.Enabled = false;
                btnDelRow.Enabled = false;
                btnSave.Enabled = false;
                btnTJ.Enabled = false;
                btnAudit.Enabled = false;
                btnCancel.Enabled = false;
                btnPrint.Enabled = true;
            }
            else if (ddlFLAG.SelectedValue == "S")
            {
                btnAudit.Enabled = true;
            }

            if (Request.QueryString["oper"] != null)
            {
                string oper = Request.QueryString["oper"].ToString();
                hfdOper.Text = oper;
                if (oper == "sq")
                {

                }
                else if (oper == "fa")
                {
                    ddlSPR.SelectedValue = UserAction.UserID;
                    dpkSPRQ.SelectedDate = DateTime.Now;
                }
                else if (oper == "sp")
                {
                    ddlSHR.SelectedValue = UserAction.UserID;
                    dpkSHRQ.SelectedDate = DateTime.Now;
                }
            }
        }

        protected override void billSave()
        {
            #region 数据有效性验证
            if (ddlFLAG.SelectedValue != "M")
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["MEMOGOODS"]).ToList();
            if (newDict.Count == 0)
            {
                Alert.Show("请输入【需求说明】", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["MEMOGOODS"].ToString()))
                {
                    if (newDict[i]["SL"] != null && string.IsNullOrWhiteSpace(newDict[i]["SL"].ToString()))
                    {
                        Alert.Show("请填写【数量】", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }
                    goodsData.Add(newDict[i]);
                }
            }

            if (goodsData.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("至少填写一条【需求说明】", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //验证单据信息
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_GOODSNEW_DOC where seqno = '" + tbxBILLNO.Text + "'") && tbxBILLNO.Enabled)
            {
                Alert.Show("您输入的单号已存在,请检查!");
                return;
            }


            #endregion

            if (PubFunc.StrIsEmpty(tbxBILLNO.Text))
            {
                tbxSEQNO.Text = BillSeqGet();
                tbxBILLNO.Text = tbxSEQNO.Text;
                tbxBILLNO.Enabled = false;
            }
            else
            {
                string flg = (string)DbHelperOra.GetSingle(string.Format("SELECT NVL(FLAG,'M') FROM DAT_GOODSNEW_DOC WHERE SEQNO='{0}'", tbxBILLNO.Text));
                if (!string.IsNullOrWhiteSpace(flg) && (",M,R").IndexOf(flg) < 0)
                {
                    Alert.Show("您输入的单据号存在重复信息，请重新输入或置空！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    tbxSEQNO.Text = tbxBILLNO.Text;
                    tbxBILLNO.Enabled = false;
                }
            }

            MyTable mtType = new MyTable("DAT_GOODSNEW_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = tbxBILLNO.Text;
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_GOODSNEW_COM");
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_GOODSNEW_DOC where seqno='" + tbxBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_GOODSNEW_COM where seqno='" + tbxBILLNO.Text + "'", null));//删除单据明细
            cmdList.AddRange(mtType.InsertCommand());
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);


                if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["MEMOGOODS"].ToString()))
                {
                    Alert.Show("【需求说明】为空，无法进行【商品新增管理】操作。");
                    return;
                }

                mtTypeMx.ColRow.Add("SEQNO", tbxBILLNO.Text);
                mtTypeMx.ColRow.Add("ROWNO", i + 1);

                //if (PubFunc.StrIsEmpty(mtTypeMx.ColRow["HSJE"].ToString()) || mtTypeMx.ColRow["HSJE"].ToString() == "0")
                //{
                //    string strSL = "0";
                //    string strHSJJ = "0";
                //    if (mtTypeMx.ColRow["SL"] != null && !string.IsNullOrWhiteSpace(mtTypeMx.ColRow["SL"].ToString()))
                //    {
                //        strSL = mtTypeMx.ColRow["SL"].ToString();
                //    }
                //    if (mtTypeMx.ColRow["HSJJ"] != null && !string.IsNullOrWhiteSpace(mtTypeMx.ColRow["HSJJ"].ToString()))
                //    {
                //        strSL = mtTypeMx.ColRow["HSJJ"].ToString();
                //    }
                //    mtTypeMx.ColRow["HSJE"] = decimal.Parse(strSL) * decimal.Parse(strHSJJ);
                //}

                cmdList.Add(mtTypeMx.Insert());
            }

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("商品新增信息保存成功！");
                //OperLog("商品新增管理", "修改单据【" + tbxBILLNO.Text + "】");
                billOpen(tbxBILLNO.Text);

                //billNew();
                //billLockDoc(true);
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
                string msg = "";
                string msg1 = "";
                string msg2 = "";
                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "0";
                    row["DHSL"] = "0";
                    row["KCSL"] = "0";
                    row["HSJE"] = "0";
                    row["HWID"] = "";
                    DataTable Temp = DbHelperOra.Query(string.Format("SELECT A.HWID, A.KCSL,A.PH,A.YXQZ,A.RQ_SC,B.ISGZ,B.GDNAME FROM DAT_GOODSSTOCK A ,DOC_GOODS B WHERE A.DEPTID ='{0}' AND A.GDSEQ = '{1}'  AND A.GDSEQ = B.GDSEQ AND A.KCSL >0 AND ROWNUM = 1 ORDER BY A.PICINO ASC", ddlDEPTID.SelectedValue, row["GDSEQ"].ToString())).Tables[0];
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
                    //换算价格
                    row["HSJJ"] = Math.Round(Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZHL"]), 4);
                    LoadGridRow(row, false);
                }

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】属于高值商品，请在【高值商品使用】里进行录入！", msg);
                    Alert.Show(strNostock, "消息提示", MessageBoxIcon.Warning);
                }

                if (!string.IsNullOrWhiteSpace(msg1))
                {
                    String strNostock = "";
                    strNostock = string.Format("商品【{0}】在部门『{1}』中没有库存,不能进行录入！", msg1, ddlDEPTID.SelectedText);
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

        protected override void billDel()
        {
            if (tbxBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据");
                return;
            }

            if (ddlFLAG.SelectedValue != "N")
            {
                Alert.Show("非新单不能删除!");
                return;
            }
            DbHelperOra.ExecuteSql("Delete from DAT_GOODSNEW_DOC t WHERE T.SEQNO ='" + tbxBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_GOODSNEW_COM t WHERE T.SEQNO ='" + tbxBILLNO.Text.Trim() + "'");
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
                //屏蔽不需要的操作按钮
                if (Request.QueryString["oper"] != null)
                {
                    string oper = Request.QueryString["oper"].ToString();
                    hfdOper.Text = oper;
                    if (oper == "sq")
                    {
                        if (flag == "M")
                        {
                            highlightRowGreen.Text += e.RowIndex.ToString() + ",";
                        }
                        else if (flag == "N")
                        {
                            highlightRowYellow.Text += e.RowIndex.ToString() + ",";
                        }
                    }
                    else if (oper == "fa")
                    {
                        if (flag == "N")
                        {
                            highlightRowYellow.Text += e.RowIndex.ToString() + ",";
                        }
                        else if (flag == "R")
                        {
                            highlightRowRed.Text += e.RowIndex.ToString() + ",";
                        }
                    }
                    else if (oper == "sp")
                    {
                        if (flag == "S")
                        {
                            highlightRowYellow.Text += e.RowIndex.ToString() + ",";
                        }
                        else if (flag == "R")
                        {
                            highlightRowRed.Text += e.RowIndex.ToString() + ",";
                        }
                    }

                }


            }
        }

        protected void btnTJ_Click(object sender, EventArgs e)
        {
            if (tbxSEQNO.Text.Length < 1)
            {
                Alert.Show("提交失败，请选择需要提交的单据！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_GOODSNEW_DOC SET FLAG = 'N' WHERE FLAG= 'M' AND SEQNO = '{0}'", tbxSEQNO.Text)) > 0)
            {
                Alert.Show("【商品新增】单据提交成功！");
                billOpen(tbxSEQNO.Text);
                //OperLog("在库养护", "提交单据【" + tbxBILLNO.Text + "】");
            }
            else
            {
                Alert.Show("提交失败，请检查单据状态！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {

        }

        protected void btnSaveFa_Click(object sender, EventArgs e)
        {
            #region 数据有效性验证
            if ((",N,R").IndexOf(ddlFLAG.SelectedValue) < 0)
            {
                Alert.Show("非【已提交】单据不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["MEMOGOODS"]).ToList();
            if (newDict.Count == 0)
            {
                Alert.Show("请输入【采购方案】", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.FormDataCheck(FormDoc).Length > 0) return;
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            string strSUBSUM = "";
            //判断是否有空行、批号填写是否符合要求
            for (int i = 0; i < newDict.Count; i++)
            {

                string strSL = "0";
                string strHSJJ = "0";
                if (newDict[i]["SL"] != null && !string.IsNullOrWhiteSpace(newDict[i]["SL"].ToString()))
                {
                    strSL = newDict[i]["SL"].ToString();
                }
                if (newDict[i]["HSJJ"] != null && !string.IsNullOrWhiteSpace(newDict[i]["HSJJ"].ToString()))
                {
                    strHSJJ = newDict[i]["HSJJ"].ToString();
                }
                newDict[i]["HSJE"] = decimal.Parse(strSL) * decimal.Parse(strHSJJ);

                strSUBSUM += (newDict[i]["HSJE"] ?? "0").ToString();

                goodsData.Add(newDict[i]);
            }

            if (goodsData.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("当前单据无数据，无法进行【保存】", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            #endregion


            MyTable mtType = new MyTable("DAT_GOODSNEW_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = tbxBILLNO.Text;
            mtType.ColRow["FLAG"] = "N";
            mtType.ColRow["SPR"] = UserAction.UserID;
            mtType.ColRow["SPRQ"] = dpkLRRQ.SelectedDate;
            mtType.ColRow.Add("BILLTYPE", BillType);
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("SUBSUM", strSUBSUM);
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_GOODSNEW_COM");
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("delete DAT_GOODSNEW_DOC where seqno='" + tbxBILLNO.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("delete DAT_GOODSNEW_COM where seqno='" + tbxBILLNO.Text + "'", null));//删除单据明细
            cmdList.AddRange(mtType.InsertCommand());
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);

                mtTypeMx.ColRow.Add("SEQNO", tbxBILLNO.Text);
                mtTypeMx.ColRow.Add("ROWNO", i + 1);

                if ((newDict[i]["HSJE"] ?? "0").ToString() == "0")
                {
                    string strSL = "0";
                    string strHSJJ = "0";
                    if (mtTypeMx.ColRow["SL"] != null && !string.IsNullOrWhiteSpace(mtTypeMx.ColRow["SL"].ToString()))
                    {
                        strSL = mtTypeMx.ColRow["SL"].ToString();
                    }
                    if (mtTypeMx.ColRow["HSJJ"] != null && !string.IsNullOrWhiteSpace(mtTypeMx.ColRow["HSJJ"].ToString()))
                    {
                        strHSJJ = mtTypeMx.ColRow["HSJJ"].ToString();
                    }
                    mtTypeMx.ColRow["HSJE"] = decimal.Parse(strSL) * decimal.Parse(strHSJJ);
                }
                cmdList.Add(mtTypeMx.Insert());
            }

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("商品采购方案保存成功！");
                hfdISTJFA.Text = "Y";
                //OperLog("商品新增管理", "修改单据【" + tbxBILLNO.Text + "】");
                billOpen(tbxBILLNO.Text);
                //billNew();
                //billLockDoc(true);
            }
        }

        protected void btnTJFA_Click(object sender, EventArgs e)
        {
            if (hfdISTJFA.Text != "Y")
            {
                Alert.Show("请先【保存】方案后在执行【方案提交】操作！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (ddlFLAG.SelectedValue != "N")
            {
                Alert.Show("单据状态不对，无法提交单据！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (tbxSEQNO.Text.Length < 1)
            {
                Alert.Show("提交失败，请选择需要提交的单据！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_GOODSNEW_DOC SET FLAG = 'S' WHERE FLAG= 'N' AND SEQNO = '{0}'", tbxSEQNO.Text)) > 0)
            {
                Alert.Show("【采购方案】提交成功！");
                hfdISTJFA.Text = "N";
                billOpen(tbxSEQNO.Text);
                //OperLog("在库养护", "提交单据【" + tbxBILLNO.Text + "】");
            }
            else
            {
                Alert.Show("提交失败，请检查单据状态！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (ddlFLAG.SelectedValue == "Y")
            {
                Alert.Show("本条申领信息已经审核确认，不能进行【驳回】操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_GOODSNEW_DOC SET FLAG = 'R' WHERE FLAG= 'S' AND SEQNO = '{0}'", tbxSEQNO.Text)) > 0)
            {
                Alert.Show("驳回成功！");
                billOpen(tbxSEQNO.Text);
                //OperLog("在库养护", "提交单据【" + tbxBILLNO.Text + "】");
            }
            else
            {
                Alert.Show("驳回失败，请检查单据状态！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (ddlFLAG.SelectedValue == "N")
            {
                Alert.Show("本条申领信息不是【已提交】状态，不能进行【废弃】操作！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_GOODSNEW_DOC SET FLAG = 'F' WHERE FLAG= 'N' AND SEQNO = '{0}'", tbxSEQNO.Text)) > 0)
            {
                Alert.Show("废弃成功！");
                billOpen(tbxSEQNO.Text);
                //OperLog("在库养护", "提交单据【" + tbxBILLNO.Text + "】");
            }
            else
            {
                Alert.Show("废弃失败，请检查单据状态！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
        }

    }
}
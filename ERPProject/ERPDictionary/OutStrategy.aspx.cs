﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using XTBase.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPDictionary
{
    public partial class OutStrategy : BillBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //PubFunc.DdlDataGet("DDL_SYS_DEPOT", docDEPTOUT);
                //PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, docDEPTOUT);
                PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID);
                //PubFunc.DdlDataGet(docCATID, "DDL_SYS_CATEGORY_TYPE");
                //docCATID.SelectedValue = "2";
                GridStrategy.DataSource = GridTemplate("");
                GridStrategy.DataBind();
            }
        }

        private DataTable GridTemplate(string text)
        {
            string sql = @"SELECT A.GROUPID,
                                   A.GROUPNAME,
                                   A.DEPTID,
                                   F_GETDEPTNAME(A.DEPTID) DEPTNAME,
                                   A.CATID,
                                   A.TYPE,
                                   A.FLAG,
                                   DECODE(A.FLAG,'N','已作废','正常') FLAGNAME
                              FROM DOC_GROUPDOC A
                             WHERE  F_CHK_DATARANGE(A.DEPTID, '{0}') = 'Y'";
            if (text.Trim().Length > 0)
            {
                sql += text;
            }
            string mySql = string.Format(sql, UserAction.UserID);
            return DbHelperOra.Query(mySql).Tables[0];
        }

        private void DataInit()
        {
            //docDEPTOUT.Enabled = true;
            //docCATID.Enabled = true;
            ddlTYPE.Enabled = true;
            docDEPTID.Enabled = true;
            docGROUPNAME.Enabled = true;
            PubFunc.FormDataClear(FormCond);
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            DataInit();
        }

        protected void btnDel_Click(object sender, EventArgs e)
        {
            if (GridStrategy.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要删除的模板信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string ids = "";
            foreach (int index in GridStrategy.SelectedRowIndexArray)
            {
                ids = ids + GridStrategy.Rows[index].DataKeys[2].ToString() + ",";
            }
            DbHelperOra.ExecuteSql("DELETE FROM DOC_GROUPDOC WHERE GROUPID IN ('" + ids.Trim(',').Replace(",", "','") + "')");
            DbHelperOra.ExecuteSql("DELETE FROM DOC_GROUPCOM WHERE GROUPID IN ('" + ids.Trim(',').Replace(",", "','") + "')");
            PubFunc.FormDataClear(FormCond);
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            DataQuery();
            Alert.ShowInTop("模板删除成功！", "消息提示", MessageBoxIcon.Warning);
        }

        protected void btnDelRow_Click(object sender, EventArgs e)
        {
            //string sqlDel = "DELETE FROM DOC_GROUPCOM G WHERE G.GROUPID IN ('ZU00004','ZU00003')";
            //DbHelperOra.ExecuteSql(sqlDel);
            String test = GridGoods.SelectedRowID;
            if (string.IsNullOrEmpty(test))
            {
                Alert.Show("请选择要删除的行数据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //object obj = Doc.GetJObject(GridGoods, GridGoods.SelectedRowID);
            //JObject obj = Doc.GetJObject(GridGoods, GridGoods.SelectedRowID);
            //string gdseq = obj["GDSEQ"].ToString();
            //int rowIndex = GridGoods.SelectedRowIndex;

            //if (hfdGROUPID.Text != "")
            //{
            //    string sqlDel = string.Format("DELETE FROM DOC_GROUPCOM G WHERE G.GROUPID='{0}' AND G.GDSEQ='{1}'", hfdGROUPID.Text, gdseq);
            //    DbHelperOra.ExecuteSql(sqlDel);
            //}
            //PageContext.RegisterStartupScript(GridGoods.GetDeleteRowReference(rowIndex));
            GridGoods.DeleteSelectedRows();
            //Alert.ShowInTop("删除成功！", "消息提示", MessageBoxIcon.Warning);
            //int rowIndex = GridGoods.SelectedCell[0];
            //PageContext.RegisterStartupScript(GridGoods.GetDeleteIndexReference(rowIndex));
            //PageContext.RegisterStartupScript(GridGoods.GetDeleteRowReference(GridGoods.SelectedRowID));

        }

        private string GetCode()
        {
            string groupId = string.Empty;
            int code = 0;
            groupId = DbHelperOra.GetSingle("SELECT SUBSTR(NVL(MAX(GROUPID),'ZU00000'),3) FROM DOC_GROUPDOC WHERE GROUPID LIKE 'ZU%' ").ToString();
            int.TryParse(groupId, out code);
            return "ZU" + (100001 + code).ToString().Substring(1);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(docGROUPNAME.Text))
            {
                Alert.Show("请输入模板名称！", "警告提醒", MessageBoxIcon.Warning);
                return;
            }
            if (docGROUPNAME.Text.Length > 50)
            {
                Alert.Show("输入的模板名称超过字数限制！", "警告提醒", MessageBoxIcon.Warning);
                return;
            }
            if (ddlTYPE.SelectedValue == "")
            {
                Alert.Show("请选择申请模板！", "警告提醒", MessageBoxIcon.Warning);
                return;
            }


            if (hfdGROUPID.Text == "")
            {
                hfdGROUPID.Text = GetCode();
            }

            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
            MyTable mt = new MyTable("DOC_GROUPDOC");
            mt.ColRow = PubFunc.FormDataHT(FormCond);
            //string type = ddlTYPE.SelectedValue;
            //mt.ColRow.Add("TYPE", "T");
            mt.ColRow.Add("SUBNUM", newDict.Count);
            mt.ColRow.Add("LRY", UserAction.UserID);
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DOC_GROUPCOM");
            //先删除单据信息在插入
            cmdList.Add(new CommandInfo("DELETE DOC_GROUPDOC WHERE GROUPID='" + hfdGROUPID.Text + "'", null));//删除单据台头
            cmdList.Add(new CommandInfo("DELETE DOC_GROUPCOM WHERE GROUPID='" + hfdGROUPID.Text + "'", null));//删除单据明细
            cmdList.AddRange(mt.InsertCommand());
            for (int i = 0; i < newDict.Count; i++)
            {
                int num = Convert.ToInt32(newDict[i]["SL"]);
                if (num < 1)
                {
                    Alert.Show("申请数量不能为0！", "警告提醒", MessageBoxIcon.Warning);
                    return;
                }
                mtTypeMx.ColRow = PubFunc.GridDataGet(newDict[i]);
                mtTypeMx.ColRow.Remove("ROWNO");
                mtTypeMx.ColRow.Add("ROWNO", i + 1);
                mtTypeMx.ColRow.Add("GROUPID", hfdGROUPID.Text);
                cmdList.Add(mtTypeMx.Insert());
            }
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                Alert.Show("模板信息保存成功！", "消息提示", MessageBoxIcon.Information);
                //DataInit();
                GridStrategy.DataSource = GridTemplate("");
                GridStrategy.DataBind();
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            if (hfdGROUPID.Text == "")
            {
                Alert.Show("请选择要导出的模板信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string sql = @"SELECT B.ROWNO 序号,
                                               A.DEPTID 科室编码,
                                               F_GETDEPTNAME(A.DEPTID) 科室名称,
                                              ' '|| B.GDSEQ 商品编码,
                                               B.GDNAME 商品名称,
                                               B.GDSPEC 商品规格,
                                               B.SL 数量,
                                               F_GETUNITNAME(B.UNIT) 单位,
                                               A.GROUPNAME 模版名称
                                          FROM DOC_GROUPDOC A, DOC_GROUPCOM B
                                         WHERE A.GROUPID = B.GROUPID AND A.GROUPID='{0}'";
            DataTable dt = DbHelperOra.Query(string.Format(sql, hfdGROUPID.Text)).Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                ExcelHelper.ExportByWeb(dt, docDEPTID.SelectedText + "模板信息", string.Format("{0}_模板信息.xls", docDEPTID.SelectedText.Replace("[", "").Replace(']', '_')));
            }
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            //if (docDEPTOUT.SelectedValue == "")
            //{
            //    Alert.Show("请先选择出库部门！！！", "异常提醒", MessageBoxIcon.Information);
            //    return;
            //}
            //if (docCATID.SelectedValue == "")
            //{
            //    Alert.Show("请先选择商品类别！！！", "异常提醒", MessageBoxIcon.Information);
            //    return;
            //}
            WindowImport.Hidden = false;
        }

        protected void btnGoods_Click(object sender, EventArgs e)
        {
            if (PubFunc.FormDataCheck(FormCond).Length > 1) return;
            string url = "~/ERPQuery/GoodsWindow_New.aspx?Deptout=" + docDEPTID.SelectedValue;// + "&goodsType=" + docCATID.SelectedValue;
            PubFunc.FormLock(FormCond, true, "");
            PageContext.RegisterStartupScript(winGoodsQuery.GetSaveStateReference(hfdValue.ClientID) + winGoodsQuery.GetShowReference(url, "商品信息查询"));
        }

        protected void winGoodsQuery_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            dt.Columns.Remove(dt.Columns["BZHL"]);
            dt.Columns.Remove(dt.Columns["UNIT"]);
            string msg = "";

            if (dt != null && dt.Rows.Count > 0)
            {
                //dt.Columns["PIZNO"].ColumnName = "PZWH";
                dt.Columns["UNITNAME"].ColumnName = "UNITSMALLNAME";
                dt.Columns["UNIT_SELL_NAME"].ColumnName = "UNITNAME";
                dt.Columns["UNIT_SELL"].ColumnName = "UNIT";
                dt.Columns["BZHL_SELL"].ColumnName = "BZHL";
                dt.Columns["HSJJ"].ColumnName = "HSJJ";
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("SL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Int32"));
                dt.Columns.Add("ROWNO", Type.GetType("System.Int32"));
                int rowNum = 0;
                if (hfdGROUPID.Text.Length > 0)
                {
                    string sql = "select ROWNO from doc_groupcom where rownum<2 and  groupid='" + hfdGROUPID.Text + "' order by rowno desc";
                    rowNum = Convert.ToInt32(DbHelperOra.GetSingle(sql));
                }
                foreach (DataRow row in dt.Rows)
                {
                    rowNum++;
                    row["ROWNO"] = rowNum;
                    row["SL"] = "0";
                    row["HSJE"] = "0";
                    if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                    {
                        msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                        Alert.Show("商品" + msg + "【含税进价】为空，不能保存模板。", "消息提示", MessageBoxIcon.Warning);
                        continue;
                    }
                    PubFunc.GridRowAdd(GridGoods, row, false);
                }
            }
            else
            {
                Alert.Show("系统传值错误！！！", "消息提示", MessageBoxIcon.Warning);
            }
        }

        private void DataQuery()
        {
            string text = trbSearch.Text.Trim();
            DataTable dt = new DataTable();
            if (text.Trim().Length > 0)
            {
                string strWhere = string.Format("AND (GROUPNAME LIKE '%{0}%' OR DEPTID LIKE '%{0}%')", text);
                dt = GridTemplate(strWhere);
            }
            else
            {
                dt = GridTemplate("");
                //DataRow[] drArr = dt.Select(string.Format("GROUPNAME LIKE '%{0}%' OR DEPT LIKE '%{0}%'", text));

                //dtNew = dt.Clone();//克隆dt的整个类型
                //foreach (DataRow dr in drArr)
                //{
                //    dtNew.Rows.Add(dr.ItemArray);
                //}
            }
            GridStrategy.DataSource = dt;
            GridStrategy.DataBind();
        }

        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            DataQuery();
        }

        protected void GridStrategy_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            docDEPTID.Enabled = false;
            docGROUPNAME.Enabled = false;
            ddlTYPE.Enabled = false;
            PubFunc.FormDataClear(FormCond);
            docDEPTID.SelectedValue = GridStrategy.Rows[e.RowIndex].DataKeys[0].ToString();
            docGROUPNAME.Text = GridStrategy.Rows[e.RowIndex].DataKeys[3].ToString();
            ddlTYPE.SelectedValue = GridStrategy.Rows[e.RowIndex].DataKeys[4].ToString();
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            hfdGROUPID.Text = GridStrategy.Rows[e.RowIndex].DataKeys[2].ToString();
            DataTable dt = DbHelperOra.Query("SELECT A.*,F_GETUNITNAME(A.UNIT) UNITNAME,ROWNO,F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,(A.SL*A.HSJJ) AS HSJE FROM DOC_GROUPCOM A WHERE A.GROUPID='" + GridStrategy.Rows[e.RowIndex].DataKeys[2].ToString() + "'").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                //docDEPTOUT.SelectedValue = GridStrategy.Rows[e.RowIndex].DataKeys[4].ToString();
                docDEPTID.SelectedValue = GridStrategy.Rows[e.RowIndex].DataKeys[0].ToString();
                //docCATID.SelectedValue = GridStrategy.Rows[e.RowIndex].DataKeys[1].ToString();
                docGROUPNAME.Text = GridStrategy.Rows[e.RowIndex].DataKeys[3].ToString();
                ddlTYPE.SelectedValue = GridStrategy.Rows[e.RowIndex].DataKeys[4].ToString();
                PubFunc.GridRowAdd(GridGoods, dt);
            }

        }

        TransactionScopeOption scopeOption = TransactionScopeOption.Required;
        System.Transactions.IsolationLevel isolationLevel = System.Transactions.IsolationLevel.Unspecified;
        long timeoutInMS = -1;

        public System.Transactions.IsolationLevel IsolationLevel
        {
            get { return isolationLevel; }
            set { isolationLevel = value; }
        }


        public TransactionScopeOption ScopeOption
        {
            get { return scopeOption; }
            set { scopeOption = value; }
        }

        public long TimeoutInMS
        {
            get { return timeoutInMS; }
            set { timeoutInMS = value; }
        }
        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormFile);
            fuImport.Text = "";
            string strGoodsSql = @"SELECT '{0}' GROUPID,
                                                           ROWNUM ROWNO,
                                                           G.GDSEQ,
                                                           G.GDNAME,
                                                           G.GDSPEC,
                                                           G.GDMODE,
                                                           DECODE(G.UNIT_SELL, 'D', G.NUM_DABZ, 'Z', G.NUM_ZHONGBZ, G.BZHL) BZHL,
                                                           F_GETUNITNAME(DECODE(G.UNIT_SELL,
                                                                                'D',
                                                                                G.UNIT_DABZ,
                                                                                'Z',
                                                                                G.UNIT_ZHONGBZ,
                                                                                G.UNIT)) UNIT,
                                                           G.PRODUCER,
                                                           0 SL,
                                                           G.HSJJ
                                                      FROM DOC_GOODS G, DOC_GOODSCFG P
                                                     WHERE G.GDSEQ = P.GDSEQ
                                                       AND P.DEPTID = '{1}'
                                                       AND G.GDSEQ IN ('{2}')";

            //原始数据导入系统
            if (fuImport.HasFile)
            {
                string toFilePath = "~/ERPUpload/DownloadExcel/";

                //获得文件扩展名
                string fileNameExt = Path.GetExtension(fuImport.FileName).ToLower();


                //验证合法的文件
                if (!ValidateFileType(fileNameExt))
                {
                    Alert.Show("无效的文件类型！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    //生成将要保存的随机文件名
                    string fileName = fuImport.FileName.Substring(0, fuImport.FileName.IndexOf(".")) + DateTime.Now.ToString("yyyyMMddHHmmss") + fileNameExt;

                    //按日期归类保存
                    //string datePath = DateTime.Now.ToString("yyyyMM") + "/" + DateTime.Now.ToString("dd") + "/";
                    //toFilePath += datePath;

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
                    fuImport.SaveAs(serverFileName);

                    if (File.Exists(serverFileName))
                    {
                        DataTable dtExcel = new DataTable();

                        if (fileNameExt == ".xlsx")
                        {
                            dtExcel = ExcelHelper.ImportExcelxtoDt(serverFileName, 0, 1); //导入excel2007
                        }
                        else
                        {
                            dtExcel = ExcelHelper.ImportExceltoDt(serverFileName, 0, 1);//导入excel2003
                        }

                        //if (fileNameExt == ".xlsx")
                        //{
                        //    dtExcel = YPExcelHelper.ImportExcelxtoDt(serverFileName, -1, 1); //导入excel2007
                        //}
                        //else
                        //{
                        //    dtExcel = YPExcelHelper.ImportExceltoDt(serverFileName, -1, 1);//导入excel2003
                        //}
                        if (dtExcel != null && dtExcel.Rows.Count > 0)
                        {
                            DataView dv = dtExcel.DefaultView;
                            dv.RowFilter = "数量 <> ''";
                            //1.过滤后直接获取DataTable
                            DataTable table = dv.ToTable();

                            //去掉重复行
                            DataView dvUniq = table.DefaultView;
                            DataTable dt = dvUniq.ToTable(true, new string[] { "科室编码", "模版名称", "申请模板" });
                            string Id = string.Empty;
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (string.IsNullOrWhiteSpace(dr["科室编码"].ToString()) || string.IsNullOrWhiteSpace(dr["模版名称"].ToString()) || string.IsNullOrWhiteSpace(dr["申请模板"].ToString()))
                                {
                                    continue;
                                }

                                Id = GetCode();
                                DataRow[] drArr = table.Select("科室编码 = '" + dr["科室编码"].ToString() + "' AND 模版名称 = '" + dr["模版名称"].ToString() + "' AND 申请模板 = '" + dr["申请模板"].ToString() + "'");
                                string gdseq = "";
                                foreach (DataRow row in drArr)
                                {
                                    gdseq += row["商品编码"].ToString() + ",";
                                }
                                StringBuilder sbSql = new StringBuilder();
                                sbSql.AppendFormat(strGoodsSql, Id, dr["科室编码"].ToString(), gdseq.Trim(',').Replace(",", "','"));
                                DataTable dtGoods = DbHelperOra.Query(sbSql.ToString()).Tables[0];
                                if (dtGoods != null && dtGoods.Rows.Count > 0)
                                {
                                    string sql = @"INSERT INTO DOC_GROUPCOM(GROUPID,ROWNO,GDSEQ,GDNAME,GDSPEC,GDMODE,BZHL,UNIT,PRODUCER,SL,HSJJ)
                                                 VALUES(:GROUPID,:ROWNO,:GDSEQ,:GDNAME,:GDSPEC,:GDMODE,:BZHL,:UNIT,:PRODUCER,:SL,:HSJJ)";
                                    List<CommandInfo> cmdList = new List<CommandInfo>();

                                    foreach (DataRow row in dtGoods.Rows)
                                    {
                                        DataRow[] rowArr = table.Select("商品编码='" + row["GDSEQ"].ToString() + "'");
                                        if (rowArr.Length > 0)
                                        {
                                            OracleParameter[] parameters = {
                                               new OracleParameter("GROUPID", OracleDbType.Varchar2,7),
                                               new OracleParameter("ROWNO", OracleDbType.Int16),
                                               new OracleParameter("GDSEQ", OracleDbType.Varchar2,20),
                                               new OracleParameter("GDNAME", OracleDbType.Varchar2,300),
                                               new OracleParameter("GDSPEC", OracleDbType.Varchar2,200),
                                               new OracleParameter("GDMODE", OracleDbType.Varchar2,800),
                                               new OracleParameter("BZHL", OracleDbType.Decimal),
                                               new OracleParameter("UNIT", OracleDbType.Varchar2,15),
                                               new OracleParameter("PRODUCER", OracleDbType.Varchar2,20),
                                               new OracleParameter("SL", OracleDbType.Decimal),
                                               new OracleParameter("HSJJ", OracleDbType.Decimal) };
                                            parameters[0].Value = row["GROUPID"];
                                            parameters[1].Value = row["ROWNO"];
                                            parameters[2].Value = row["GDSEQ"];
                                            parameters[3].Value = row["GDNAME"];
                                            parameters[4].Value = row["GDSPEC"];
                                            parameters[5].Value = row["GDMODE"];
                                            parameters[6].Value = row["BZHL"];
                                            parameters[7].Value = row["UNIT"];
                                            parameters[8].Value = row["PRODUCER"];
                                            parameters[9].Value = Decimal.Parse(rowArr[0]["数量"].ToString());
                                            parameters[10].Value = row["HSJJ"];
                                            cmdList.Add(new CommandInfo(sql, parameters));
                                        }
                                    }
                                    OracleParameter[] parameters_doc = {
                                               new OracleParameter("GROUPID", OracleDbType.Varchar2,7),
                                               new OracleParameter("GROUPNAME", OracleDbType.Varchar2,40),
                                               new OracleParameter("FLAG", OracleDbType.Char,1),
                                               new OracleParameter("TYPE", OracleDbType.Char,1),
                                               new OracleParameter("DEPTID", OracleDbType.Varchar2,20),
                                               new OracleParameter("LYR", OracleDbType.Varchar2,8) };
                                    parameters_doc[0].Value = Id;
                                    parameters_doc[1].Value = dr["模版名称"].ToString();
                                    parameters_doc[2].Value = "Y";
                                    switch (dr["申请模板"].ToString())
                                    {
                                        case "科室申领":
                                            parameters_doc[3].Value = "K";
                                            break;
                                        case "订货申领":
                                            parameters_doc[3].Value = "D";
                                            break;
                                        case "手术套包申领":
                                            parameters_doc[3].Value = "S";
                                            break;
                                    }
                                    parameters_doc[4].Value = dr["科室编码"].ToString();
                                    parameters_doc[5].Value = UserAction.UserID;
                                    cmdList.Add(new CommandInfo("INSERT INTO DOC_GROUPDOC (GROUPID, GROUPNAME, FLAG, TYPE, DEPTID,LRY) VALUES (:GROUPID, :GROUPNAME, :FLAG, :TYPE, :DEPTID,:LRY)", parameters_doc));

                                    WindowImport.Hidden = true;
                                    if (DbHelperOra.ExecuteSqlTran(cmdList))
                                    {
                                        Alert.Show("科室模板信息导入成功！", "消息提醒", MessageBoxIcon.Information);
                                        DataQuery();
                                    }
                                    File.Delete(serverFileName);
                                }
                                else
                                {
                                    Alert.Show("导入资料中的商品信息错误，请重新检查商品信息！", "消息提示", MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Alert.Show("请选择excel文件！", "警告提醒", MessageBoxIcon.Warning);
            }
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            if (e.EventArgument.IndexOf("GoodsAdd") >= 0)
            {
                winGoodsQuery_Close(null, null);
            }
        }
        protected void btnAdd_Bill(object sender, EventArgs e)
        {
            if (GridStrategy.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要删除的模板信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            foreach (int index in GridStrategy.SelectedRowIndexArray)
            {
                string groupId = GridStrategy.Rows[index].DataKeys[2].ToString();
                if (groupId.Trim().Length > 0)
                {
                    DbHelperOra.ExecuteSql(string.Format("UPDATE DOC_GROUPDOC SET FLAG='Y' WHERE GROUPID='{0}'", groupId));
                }
            }

            DataQuery();
        }

        protected void btnDelete_Bill(object sender, EventArgs e)
        {
            if (GridStrategy.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要删除的模板信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            foreach (int index in GridStrategy.SelectedRowIndexArray)
            {
                string groupId = GridStrategy.Rows[index].DataKeys[2].ToString();
                if (groupId.Trim().Length > 0)
                {
                    DbHelperOra.ExecuteSql(string.Format("UPDATE DOC_GROUPDOC SET FLAG='N' WHERE GROUPID='{0}'", groupId));
                }

            }
            DataQuery();
        }
    }
}
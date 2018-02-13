using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Transactions;
using System.Text.RegularExpressions;
using XTBase;
using FineUIPro;
using XTBase.Utilities;
using Oracle.ManagedDataAccess.Client;

namespace ERPProject.ERPDictionary
{
    public partial class GoodsShelf : PageBase
    {
        private static DataTable ViewTable;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }
        }

        private void DataInit()
        {
            PubFunc.DdlDataSql(ddlKFBH, "select code,'['||code||']'||name from sys_dept  where type in ('1','2')");
            PubFunc.DdlDataSql(ddllKFBH, "select code,'['||code||']'||name from sys_dept  where type in ('1','2')");
            PubFunc.DdlDataGet(ddlMANAGER, "DDL_USER");
            PubFunc.DdlDataGet(ddllMANAGER, "DDL_USER");
            PubFunc.DdlDataGet(ddlKB, "DDL_DEPOT_AREA");
            PubFunc.DdlDataGet(ddllKB, "DDL_DEPOT_AREA");
            //使用部门下拉表
            PubFunc.DdlDataGet("DDL_SYS_DEPT", lstDEPTID);
            ddlMANAGER.SelectedValue = UserAction.UserID;
            ddllMANAGER.SelectedValue = UserAction.UserID;
            ddlABC.SelectedIndex = 0;
            if (ViewTable != null)
            {
                ViewTable.Clear();
            }
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormShelf);
            hfdIsNew.Text = "Y";
            tbxHWID.Enabled = true;
            ddlMANAGER.SelectedValue = UserAction.UserID;
            ddllMANAGER.SelectedValue = UserAction.UserID;
            ddlABC.SelectedIndex = 0;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (GridShelf.SelectedRowIndexArray.Length > 0)
            {
                string Hw = "";
                for (int i = 0; i < GridShelf.SelectedRowIndexArray.Length; i++)
                {
                    Hw += GridShelf.Rows[GridShelf.SelectedRowIndexArray[i]].Values[1].ToString() + ",";
                }
                DbHelperOra.ExecuteSql("delete from DOC_HWZD where HWID in ('" + Hw.TrimEnd(',').Replace(",", "','") + "')");
                DataSearch();
            }
            else
            {
                Alert.Show("请选择要删除的货位信息！", MessageBoxIcon.Warning);
            }
        }

        protected void btSave_Click(object sender, EventArgs e)
        {

            Regex rgx = new Regex(@"^[A-Za-z0-9]+$");
            if (!rgx.IsMatch(tbxHWID.Text))
            {
                Alert.Show("请填写【货位ID】只允许输入字母或数字", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!rgx.IsMatch(tbxQYBH.Text))
            {
                Alert.Show("请填写【区域编号】只允许输入字母或数字", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!rgx.IsMatch(tbxPAI.Text))
            {
                Alert.Show("请填写【排号】只允许输入字母或数字", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!rgx.IsMatch(tbxLIE.Text))
            {
                Alert.Show("请填写【列号】只允许输入字母或数字", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (!rgx.IsMatch(tbxCENG.Text))
            {
                Alert.Show("请填写【层号】只允许输入字母或数字", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(ddlKFBH.SelectedValue))
            {
                Alert.Show("请选择所属库房！", "提示", MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(ddlHWZT.SelectedValue))
            {
                Alert.Show("请选择货位状态！", "提示", MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(ddlKB.SelectedValue))
            {
                Alert.Show("请选择库房类别！", "提示", MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(ddlMANAGER.SelectedValue))
            {
                Alert.Show("请选择管理员！", "提示", MessageBoxIcon.Information);
                return;
            }

            if (tbxHWID.Text.Trim().ToString() == "")
            {
                if (tbxQYBH.Text.Trim().ToString() == "" || tbxPAI.Text.Trim().ToString() == "" || tbxLIE.Text.Trim().ToString() == "" || tbxCENG.Text.Trim().ToString() == "")
                {
                    Alert.Show("请填写【货位编号】或将【区域编号】【排号】【列号】【层号】填写完整 ", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    tbxHWID.Text = tbxQYBH.Text.Trim().ToString() + tbxPAI.Text.Trim().ToString() + tbxLIE.Text.Trim().ToString() + tbxCENG.Text.Trim().ToString();
                }
            }

            if (PubFunc.FormDataCheck(FormShelf).Length > 1) return;
            //MyTable mtType = new MyTable("DOC_HWZD");
            //mtType.ColRow = PubFunc.FormDataHT(FormShelf);
            //mtType.ColRow.Remove("ISNEW");
            if (hfdIsNew.Text == "Y")
            {
                //新增时验证主键
                if (DbHelperOra.Exists("select 1 from DOC_HWZD where HWID = '" + tbxHWID.Text + "'"))
                {
                    Alert.Show("货位编号【" + tbxHWID.Text + "】已经存在,请检查!");
                    return;
                }
                //mtType.InsertExec();
                string sql = @"INSERT INTO DOC_HWZD(HWID,JWBH,KFBH,HWZT,MANAGER,KB,DZBQH,ABC,QYBH,PAI,LIE,CENG) 
                        values('" + tbxHWID.Text + "','" + tbsJWBH.Text + "','" + ddlKFBH.SelectedValue + "','" + ddlHWZT.SelectedValue + "','" + ddlMANAGER.SelectedValue + "','" + ddlKB.SelectedValue + "','" + tbxDZBQH.Text + "','" + ddlABC.SelectedValue + "','" + tbxQYBH.Text + "','" + tbxPAI.Text + "','" + tbxLIE.Text + "','" + tbxCENG.Text + "')";
                DbHelperOra.ExecuteSql(sql);
            }
            else
            {
                // mtType.UpdateExec("");
                DbHelperOra.ExecuteSql("UPDATE DOC_HWZD set JWBH = '" + tbsJWBH.Text + "',KFBH = '" + ddlKFBH.SelectedValue + "',HWZT='" + ddlHWZT.SelectedValue + "',MANAGER = '" + ddlMANAGER.SelectedValue + "',KB='" + ddlKB.SelectedValue + "',DZBQH='" + tbxDZBQH.Text + "',ABC ='" + ddlABC.SelectedValue + "',QYBH='" + tbxQYBH.Text + "',PAI='" + tbxPAI.Text + "',LIE='" + tbxLIE.Text + "',CENG='" + tbxCENG.Text + "' where HWID='" + tbxHWID.Text + "'");
            }

            Alert.Show("数据保存成功！");

            PubFunc.FormDataClear(FormShelf);
            hfdIsNew.Text = "Y";
            tbxHWID.Enabled = true;
            DataSearch();
        }

        private void DataSearch()
        {
            int total = 0;
            string sql = "SELECT HWID,JWBH,KFBH,KB,f_getusername(MANAGER) MANAGER,HWZT,DZBQH,ABC,DECODE(HWZT,'Y','可用','1','可用','不可用') HWZTNAME,F_GETDEPTNAME(a.KFBH) KFBHNAME,(select NAME FROM SYS_CODEVALUE c WHERE c.TYPE='DEPOT_AREA' and c.code=a.KB) KBNAME,XD,QYBH,PAI,LIE,CENG FROM DOC_HWZD A  WHERE HWID LIKE '%{0}%' ";
            string strSearch = "";
            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += " AND KFBH='" + lstDEPTID.SelectedValue + "'";
            }
            sql += strSearch;


            DataTable dtData = PubFunc.DbGetPage(GridShelf.PageIndex, GridShelf.PageSize, string.Format(sql, trbSearch.Text), ref total);
            GridShelf.RecordCount = total;
            GridShelf.DataSource = dtData;
            //GridShelf.DataSource = DbHelperOra.Query(string.Format(sql, trbSearch.Text)).Tables[0];
            GridShelf.DataBind();



        }

        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            DataSearch();
        }

        protected void GridShelf_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            hfdIsNew.Text = "N";
            string strCode = GridShelf.Rows[e.RowIndex].Values[1].ToString();
            string strSql = string.Format("SELECT * FROM DOC_HWZD WHERE HWID='{0}'", strCode);

            PubFunc.FormDataSet(FormShelf, DbHelperOra.Query(strSql).Tables[0].Rows[0]);

            tbxHWID.Enabled = false;
        }
        protected void Grid1_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridShelf.PageIndex = e.NewPageIndex;
            DataSearch();
        }

        private void myreset(DataTable dt)
        {
            dt.Clear();
            GridIns.DataBind();
            fuDocument.Reset();
        }

        private void dataSearch()
        {
            GridIns.DataSource = ViewTable;
            GridIns.DataBind();
        }

        protected void fuDocument_FileSelected(object sender, EventArgs e)
        {
            try
            {
                if (this.fuDocument.HasFile)
                {
                    string toFilePath = "~/ERPUpload/ProductShelf/";
                    string strPath = AppDomain.CurrentDomain.BaseDirectory + "ERPUpload/ProductShelf/";
                    if (!Directory.Exists(strPath))
                    {
                        Directory.CreateDirectory(strPath);
                    }
                    //获得文件扩展名
                    string fileNameExt = Path.GetExtension(this.fuDocument.FileName).ToLower();

                    if (!ValidateFileType(fileNameExt))
                    {
                        Alert.Show("无效的文件类型！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }

                    string myallname = fuDocument.ShortFileName;//ShortFileName表示整个导入文件的名称
                    string[] mystr = myallname.Split('.');
                    if ((!ValidateFileType(myallname))
                        || mystr[1].Equals("doc") || mystr[1].Equals("docx") || mystr[1].Equals("png") || mystr[1].Equals("jpg"))
                    {
                        Alert.Show("请选择excel文件导入！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }

                    //验证合法的文件
                    if (CheckFileExt(fileNameExt))
                    {
                        //生成将要保存的随机文件名
                        string fileName = this.fuDocument.ShortFileName.Substring(0, this.fuDocument.ShortFileName.IndexOf(".")) + DateTime.Now.ToString("yyyyMMddHHmmss") + fileNameExt;

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
                            DataTable dtExcel = new DataTable();

                            if (fileNameExt == ".xlsx")
                            {
                                dtExcel = ExcelHelper.ImportExcelxtoDt(serverFileName, 1, 0); //导入excel2007
                            }
                            else
                            {
                                dtExcel = ExcelHelper.ImportExceltoDt(serverFileName, 1, 0);//导入excel2003
                            }
                            ViewTable = new DataTable();
                            DataTable schema = ApiUtil.GetTableSchema("DOC_HWZD");
                            DataTable myExcelData = GetShelfFiled();
                            string[] arrFiled = new string[myExcelData.Rows.Count];
                            for (int index = 0; index < myExcelData.Rows.Count; index++)
                            {
                                DataRow mydr = myExcelData.Rows[index];
                                arrFiled[index] = mydr["TABLEZD"].ToString();

                                dtExcel.Columns[mydr["EXCELNAME"].ToString()].ColumnName = mydr["TABLEZD"].ToString();
                            }

                            DataTable table = dtExcel.DefaultView.ToTable(false, arrFiled);
                            if (table.Rows.Count == 0)
                            {
                                Alert.Show("请导入有数据的货位资料表！");
                                myreset(table);
                                return;
                            }

                            for (int i = 0; i < table.Rows.Count; i++)
                            {
                                if (table.Rows[i]["KB"] == DBNull.Value)
                                {
                                    Alert.Show("【库别】为必填项，请检查是否未填写！");
                                    myreset(table);
                                    return;
                                }
                                if (table.Rows[i]["KFBH"] == DBNull.Value)
                                {
                                    Alert.Show("【库房编号】为必填项，请检查是否未填写！");
                                    myreset(table);
                                    return;
                                }
                                if (table.Rows[i]["HWID"] == DBNull.Value)
                                {
                                    Alert.Show("【货位编号】为必填项，请检查是否未填写！");
                                    myreset(table);
                                    return;
                                }
                                //if (table.Rows[i]["KBNAME"] == DBNull.Value)
                                //{
                                //    Alert.Show("【库房类别】为必填项，请检查是否未填写！");
                                //    myreset(table);
                                //    return;
                                //}
                                if (table.Rows[i]["QYBH"] == DBNull.Value)
                                {
                                    Alert.Show("【区域编号】为必填项，请检查是否未填写！");
                                    myreset(table);
                                    return;
                                }
                                if (table.Rows[i]["PAI"] == DBNull.Value)
                                {
                                    Alert.Show("【排】为必填项，请检查是否未填写！");
                                    myreset(table);
                                    return;
                                }
                                if (table.Rows[i]["LIE"] == DBNull.Value)
                                {
                                    Alert.Show("【列】为必填项，请检查是否未填写！");
                                    myreset(table);
                                    return;
                                }

                                if (table.Rows[i]["CENG"] == DBNull.Value)
                                {
                                    Alert.Show("【层】为必填项，请检查是否未填写！");
                                    myreset(table);
                                    return;
                                }
                            }

                            ViewTable = schema;

                            ViewTable.Columns.Add("KFBHNAME", typeof(String));
                            ViewTable.Columns.Add("KBNAME", typeof(String));
                            ViewTable.Columns.Add("MANAGERNAME", typeof(String));

                            foreach (DataRow dr in table.Rows)
                            {
                                DataRow ViewTableRow = ViewTable.NewRow();
                                foreach (DataColumn dc in table.Columns)
                                {
                                    ViewTableRow[dc.ColumnName] = dr[dc.ColumnName];
                                }
                                ViewTable.Rows.Add(ViewTableRow);
                            }

                            dataSearch();

                            //Alert.Show("请双击表格行完善货位基本信息！");

                            File.Delete(serverFileName);
                        }
                    }
                }
                else
                {
                    Alert.Show("请选择excel文件！");
                }
            }
            catch (Exception ex)
            {
                Alert.Show(ex.Message);
            }
        }

        protected void GridIns_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            hfdRowIndex.Text = e.RowIndex.ToString();
            Int32 seq = e.RowIndex;
            PubFunc.FormDataSet(myFormShelf, ViewTable.Rows[seq]);
            ddllABC.SelectedValue = ViewTable.Rows[seq]["ABC"].ToString();
            ddllHWZT.SelectedValue = ViewTable.Rows[seq]["HWZT"].ToString();
            ddllKB.SelectedValue = ViewTable.Rows[seq]["KB"].ToString();
            ddllMANAGER.SelectedValue = ViewTable.Rows[seq]["MANAGER"].ToString();
            ddllKFBH.SelectedValue = ViewTable.Rows[seq]["KFBH"].ToString();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                ViewTable.Clear();
                GridIns.DataBind();
                fuDocument.Reset();
                PubFunc.FormDataClear(myFormShelf);
                // PubFunc.Form2Lock(myFormShelf, true);
            }
            catch
            {
                Alert.Show("没有数据哦！");
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (hfdRowIndex.Text == "")
            {
                Alert.Show("请先选择一行货位信息！");
                return;
            }
            if (string.IsNullOrWhiteSpace(docHWID.Text))
            {
                Alert.Show("请输入货位编号！");
                return;
            }
            //else if (string.IsNullOrWhiteSpace(docJWBH.Text))
            //{
            //    Alert.Show("请输入货架编号！");
            //    return;
            //}
            else if (string.IsNullOrWhiteSpace(ddllKFBH.SelectedValue))
            {
                Alert.Show("请选择该货架所属库房！");
                return;
            }
            //else if (string.IsNullOrWhiteSpace(ddllHWZT.SelectedValue))
            //{
            //    Alert.Show("请选择货位状态！");
            //    return;
            //}
            else if (string.IsNullOrWhiteSpace(docQYBH.Text) || string.IsNullOrWhiteSpace(docPAI.Text) || string.IsNullOrWhiteSpace(docLIE.Text) || string.IsNullOrWhiteSpace(docCENG.Text))
            {
                Alert.Show("区域编号，排号，列号，层号不得为空！");
                return;
            }
            else if (string.IsNullOrWhiteSpace(ddllKB.SelectedValue))
            {
                Alert.Show("库房类别不得为空！");
                return;
            }
            else if (string.IsNullOrWhiteSpace(ddllMANAGER.SelectedValue))
            {
                Alert.Show("管理员不得为空！");
                return;
            }

            DataRow dr = ViewTable.Rows[int.Parse(hfdRowIndex.Text)];

            dr["HWID"] = docHWID.Text;
            dr["KFBH"] = ddllKFBH.SelectedValue;
            dr["KB"] = ddllKB.SelectedValue;
            dr["MANAGER"] = ddllMANAGER.SelectedValue;

            dr["KFBHNAME"] = ddllKFBH.SelectedText;
            dr["KBNAME"] = ddllKB.SelectedText;
            dr["MANAGERNAME"] = ddllMANAGER.SelectedText;

            dr["QYBH"] = docQYBH.Text;
            dr["PAI"] = docPAI.Text;
            dr["LIE"] = docLIE.Text;
            dr["CENG"] = docCENG.Text;
            dr["DZBQH"] = docDZBQH.Text;
            dataSearch();
            Alert.Show("保存成功！");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ViewTable != null && ViewTable.Rows.Count > 0)
            {
                string sql = @"INSERT INTO DOC_HWZD(HWID,KB,KFBH,QYBH,PAI,LIE,CENG,XD,CHANG,KUAN,GAO,DZBQH,HWRL,MANAGER)
                                                 VALUES(:HWID,:KB,:KFBH,:QYBH,:PAI,:LIE,:CENG,:XD,:CHANG,:KUAN,:GAO,:DZBQH,:HWRL,:MANAGER)";
                //sql = string.Format(sql, "11");

                OracleConnection con = new OracleConnection(DbHelperOra.connectionString);
                OracleDataAdapter da = new OracleDataAdapter(sql, con);
                //在批量添加数据前的准备工作
                da.InsertCommand = new OracleCommand(sql, con);
                OracleParameter param = new OracleParameter();

                param = da.InsertCommand.Parameters.Add(new OracleParameter("HWID", OracleDbType.Varchar2, 11));
                param.SourceVersion = DataRowVersion.Current;
                param.SourceColumn = "HWID";

                param = da.InsertCommand.Parameters.Add(new OracleParameter("KB", OracleDbType.Varchar2, 10));
                param.SourceVersion = DataRowVersion.Current;
                param.SourceColumn = "KB";

                param = da.InsertCommand.Parameters.Add(new OracleParameter("KFBH", OracleDbType.Varchar2, 10));
                param.SourceVersion = DataRowVersion.Current;
                param.SourceColumn = "KFBH";

                param = da.InsertCommand.Parameters.Add(new OracleParameter("QYBH", OracleDbType.Varchar2, 10));
                param.SourceVersion = DataRowVersion.Current;
                param.SourceColumn = "QYBH";

                param = da.InsertCommand.Parameters.Add(new OracleParameter("PAI", OracleDbType.Varchar2, 3));
                param.SourceVersion = DataRowVersion.Current;
                param.SourceColumn = "PAI";


                param = da.InsertCommand.Parameters.Add(new OracleParameter("LIE", OracleDbType.Varchar2, 3));
                param.SourceVersion = DataRowVersion.Current;
                param.SourceColumn = "LIE";


                param = da.InsertCommand.Parameters.Add(new OracleParameter("CENG", OracleDbType.Varchar2, 3));
                param.SourceVersion = DataRowVersion.Current;
                param.SourceColumn = "CENG";

                param = da.InsertCommand.Parameters.Add(new OracleParameter("XD", OracleDbType.Varchar2, 10));
                param.SourceVersion = DataRowVersion.Current;
                param.SourceColumn = "XD";

                param = da.InsertCommand.Parameters.Add(new OracleParameter("CHANG", OracleDbType.Decimal));
                param.SourceVersion = DataRowVersion.Current;
                param.SourceColumn = "CHANG";


                param = da.InsertCommand.Parameters.Add(new OracleParameter("KUAN", OracleDbType.Decimal));
                param.SourceVersion = DataRowVersion.Current;
                param.SourceColumn = "KUAN";


                param = da.InsertCommand.Parameters.Add(new OracleParameter("GAO", OracleDbType.Decimal));
                param.SourceVersion = DataRowVersion.Current;
                param.SourceColumn = "GAO";


                param = da.InsertCommand.Parameters.Add(new OracleParameter("DZBQH", OracleDbType.Varchar2, 10));
                param.SourceVersion = DataRowVersion.Current;
                param.SourceColumn = "DZBQH";

                param = da.InsertCommand.Parameters.Add(new OracleParameter("HWRL", OracleDbType.Decimal));
                param.SourceVersion = DataRowVersion.Current;
                param.SourceColumn = "HWRL";


                param = da.InsertCommand.Parameters.Add(new OracleParameter("MANAGER", OracleDbType.Varchar2, 10));
                param.SourceVersion = DataRowVersion.Current;
                param.SourceColumn = "MANAGER";

                #region 新建事务

                TransactionOptions options = new TransactionOptions();
                options.IsolationLevel = isolationLevel;
                if (timeoutInMS > 0)
                    options.Timeout = new TimeSpan(timeoutInMS * 10);
                using (TransactionScope scope = new TransactionScope(scopeOption, options))
                {
                    //批量添加数据
                    try
                    {
                        con.Open();
                        DataTable updateTable = ViewTable.Copy();
                        da.Update(updateTable);

                        Alert.Show("数据导入成功！", "消息提示", MessageBoxIcon.Information);
                        //btnSave.Enabled = false;
                        dataSearch();
                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        if (ex.Message.ToString().Contains("ORA-00001"))
                        {
                            //Alert.Show(ex.Message.ToString());
                            Alert.Show("excel中导入货位与现有某货位ID重复");
                        }
                        else if (ex.Message.ToString().Contains("ORA-01400"))
                        {
                            Alert.Show("请将数据填写完整！");
                        }
                        else
                        {
                            Alert.Show("数据库错误：" + ex.Message, "异常信息", MessageBoxIcon.Warning);
                        }
                    }
                    finally
                    {
                        con.Close();
                    }
                }

                #endregion
                dataSearch();
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

        protected void btnExport_Click(object sender, EventArgs e)
        {
            if (GridShelf.Rows.Count < 1)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string strSql = @"SELECT HWID 货位ID,
                           --JWBH 货架编号,
                           F_GETDEPTNAME(a.KFBH) 所属库房,
                           (select NAME
                              FROM SYS_CODEVALUE c
                             WHERE c.TYPE = 'DEPOT_AREA'
                               and c.code = a.KB) 库房类别,
                           f_getusername(MANAGER) 管理员,
                           DZBQH 电子标签号,
                           DECODE(HWZT, 'Y', '可用', '1', '可用', '不可用') 货位状态,
                           QYBH 区域编号,
                           PAI 排号,
                           LIE 列号,
                           CENG 层号,
                           ABC ABC分类
                      FROM DOC_HWZD A
                     WHERE 1 = 1";
            string strSearch = "";
            if (!string.IsNullOrWhiteSpace(trbSearch.Text))
            {
                strSearch += string.Format(" AND HWID LIKE '%{0}%'", trbSearch.Text);
            }
            if (!string.IsNullOrWhiteSpace(lstDEPTID.SelectedValue))
            {
                strSearch += string.Format(" AND KFBH = '{0}'", lstDEPTID.SelectedValue);
            }
            strSql += strSearch;
            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            //if (GridShelf.DataSource == null)
            //{
            //    Alert.Show("没有数据,无法导出！");
            //    return;
            //}
            ExcelHelper.ExportByWeb(dt, "货位资料", "货位资料导出_" + DateTime.Now.ToString("yyyyMMddHH") + ".xls");
        }

        private DataTable GetShelfFiled()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("EXCELNAME", System.Type.GetType("System.String")));
            dt.Columns.Add(new DataColumn("TABLEZD", System.Type.GetType("System.String")));

            DataRow dr1 = dt.NewRow();
            dr1["EXCELNAME"] = "货位编号";
            dr1["TABLEZD"] = "HWID";
            dt.Rows.Add(dr1);
            DataRow dr2 = dt.NewRow();
            dr2["EXCELNAME"] = "库别";
            dr2["TABLEZD"] = "KB";
            dt.Rows.Add(dr2);
            DataRow dr8 = dt.NewRow();
            dr8["EXCELNAME"] = "库房编号";
            dr8["TABLEZD"] = "KFBH";
            dt.Rows.Add(dr8);
            DataRow dr3 = dt.NewRow();
            dr3["EXCELNAME"] = "区域编码";
            dr3["TABLEZD"] = "QYBH";
            dt.Rows.Add(dr3);
            DataRow dr4 = dt.NewRow();
            dr4["EXCELNAME"] = "排";
            dr4["TABLEZD"] = "PAI";
            dt.Rows.Add(dr4);
            DataRow dr5 = dt.NewRow();
            dr5["EXCELNAME"] = "列";
            dr5["TABLEZD"] = "LIE";
            dt.Rows.Add(dr5);
            DataRow dr6 = dt.NewRow();
            dr6["EXCELNAME"] = "层";
            dr6["TABLEZD"] = "CENG";
            dt.Rows.Add(dr6);
            DataRow dr7 = dt.NewRow();
            dr7["EXCELNAME"] = "电子标签";
            dr7["TABLEZD"] = "DZBQH";
            dt.Rows.Add(dr7);
            DataRow dr9 = dt.NewRow();
            dr9["EXCELNAME"] = "巷道";
            dr9["TABLEZD"] = "XD";
            dt.Rows.Add(dr9);
            DataRow dr10 = dt.NewRow();
            dr10["EXCELNAME"] = "长";
            dr10["TABLEZD"] = "CHANG";
            dt.Rows.Add(dr10);
            DataRow dr11 = dt.NewRow();
            dr11["EXCELNAME"] = "宽";
            dr11["TABLEZD"] = "KUAN";
            dt.Rows.Add(dr11);
            DataRow dr12 = dt.NewRow();
            dr12["EXCELNAME"] = "高";
            dr12["TABLEZD"] = "GAO";
            dt.Rows.Add(dr12);
            DataRow dr13 = dt.NewRow();
            dr13["EXCELNAME"] = "货位容量";
            dr13["TABLEZD"] = "HWRL";
            dt.Rows.Add(dr13);
            DataRow dr14 = dt.NewRow();
            dr14["EXCELNAME"] = "管理员";
            dr14["TABLEZD"] = "MANAGER";
            dt.Rows.Add(dr14);
            DataRow dr15 = dt.NewRow();
            dr15["EXCELNAME"] = "存储分类";
            dr15["TABLEZD"] = "CCFL";
            dt.Rows.Add(dr15);
            return dt;
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(lstDEPTID.SelectedValue))
            {
                Alert.Show("请先选择要打印货位的部门！", "异常提示", MessageBoxIcon.Warning);
                return;
            }
            string shelf = string.Empty;
            if (GridShelf.Rows.Count > 0 && GridShelf.SelectedRowIndexArray.Count() > 0)
            {
                foreach (int index in GridShelf.SelectedRowIndexArray)
                {
                    shelf = shelf + GridShelf.Rows[index].DataKeys[0].ToString() + ",";
                }
            }
            else
            {
                shelf = "ALL";
            }
            hfdShelf.Text = shelf.TrimEnd(',');
            PageContext.RegisterStartupScript("Print_Click();");
        }
    }
}
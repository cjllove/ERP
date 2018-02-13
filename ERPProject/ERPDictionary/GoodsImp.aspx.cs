using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Transactions;
using Oracle.ManagedDataAccess.Client;
using System.Text.RegularExpressions;
using XTBase.Utilities;

namespace ERPProject.ERPDictionary
{
    public partial class ImpGoods : BillBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PubFunc.DdlDataGet("DDL_GOODSTYPE", ddlCATID0);
                ddlCATID0.SelectedValue = "2";
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                DbHelperOra.ExecuteSql("DELETE FROM IMP_DOC_GOODS");
                GridStock.DataBind();
                fuDocument.Reset();
            }
            catch
            {
                Alert.Show("没有数据哦！");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //校验
            if (!DbHelperOra.Exists("SELECT 1 FROM IMP_DOC_GOODS WHERE IMPOPER = '" + UserAction.UserID + "' AND STR3 = 'Y'"))
            {
                Alert.Show("不存在可导入的数据，请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists("SELECT 1 FROM DOC_SUPPLIER WHERE SUPID = '00001'"))
            {
                Alert.Show("未定义医院供应商【00001】，请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            OracleParameter[] param = { new OracleParameter("V_USER", OracleDbType.Varchar2, 20),
                                                       new OracleParameter("V_TYPE", OracleDbType.Varchar2, 20),
                                                       new OracleParameter("V_CATID0", OracleDbType.Varchar2, 1)};
            param[0].Value = UserAction.UserID;
            param[1].Value = "INS";
            param[2].Value = string.IsNullOrWhiteSpace(ddlCATID0.SelectedValue) ? "2" : ddlCATID0.SelectedValue;
            try
            {
                DbHelperOra.RunProcedure("P_GOODS_IMP", param);
                Alert.Show("提交成功，商品资料信息已导入！", "提示信息", MessageBoxIcon.Information);
                GridStock.DataSource = null;
                GridStock.DataBind();
                btnSave.Enabled = false;
            }
            catch (Exception ex)
            {
                Alert.Show(ex.Message, "错误", MessageBoxIcon.Error);
            }
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
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
                        DbHelperOra.ExecuteSql("DELETE FROM IMP_DOC_GOODS WHERE IMPOPER = '" + UserAction.UserID + "'");
                        if (table != null && table.Rows.Count > 0)
                        {
                            string sql = @"INSERT INTO IMP_DOC_GOODS(GDSEQ,SEQNO,GDNAME,GDSPEC,UNIT,HSJJ,PRODUCER,PIZNO,CATID,SUPPLIER,ISGZ,ISLOT,ISJG,ZUHAO,IMPOPER,IMPRQ) 
                                           VALUES(:GDSEQ,:SEQNO,:GDNAME,:GDSPEC,:UNIT,:HSJJ,:PRODUCER,:PIZNO,:CATID,:SUPPLIER,:ISGZ,:ISLOT,:ISJG,:ZUHAO,:IMPOPER,SYSDATE)";
                            OracleConnection con = new OracleConnection(DbHelperOra.connectionString);
                            OracleDataAdapter da = new OracleDataAdapter(sql, con);
                            //在批量添加数据前的准备工作
                            da.InsertCommand = new OracleCommand(sql, con);
                            OracleParameter param = new OracleParameter();

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("GDSEQ", OracleDbType.Varchar2, 50));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "商品编码";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("SEQNO", OracleDbType.Varchar2, 20));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "流水号";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("GDNAME", OracleDbType.Varchar2, 100));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "商品名称";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("GDSPEC", OracleDbType.Varchar2, 50));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "商品规格";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("UNIT", OracleDbType.Varchar2, 50));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "单位";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("HSJJ", OracleDbType.Varchar2, 20));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "含税进价";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("PRODUCER", OracleDbType.Varchar2, 100));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "生产厂家";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("PIZNO", OracleDbType.Varchar2, 50));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "注册证号";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("CATID", OracleDbType.Varchar2, 50));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "商品类别";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("SUPPLIER", OracleDbType.Varchar2, 100));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "供应商";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("ISGZ", OracleDbType.Varchar2, 20));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "是否高值";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("ISLOT", OracleDbType.Varchar2, 20));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "是否管理批号";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("ISJG", OracleDbType.Varchar2, 20));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "是否代表品";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("ZUHAO", OracleDbType.Varchar2, 50));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "组号";

                            OracleParameter para = new OracleParameter("IMPOPER", OracleDbType.Varchar2, 20);
                            para.Value = UserAction.UserID;
                            param = da.InsertCommand.Parameters.Add(para);

                            //批量添加数据
                            try
                            {
                                con.Open();
                                da.Update(table);
                                //处理导入数据的准确性
                                OracleParameter[] parameters = new OracleParameter[]
                                {
                                     new OracleParameter("V_USER",OracleDbType.Varchar2),
                                     new OracleParameter("V_TYPE",OracleDbType.Varchar2)
                                };
                                parameters[0].Value = UserAction.UserID;
                                parameters[1].Value = "CHK";
                                DbHelperOra.RunProcedure("P_GOODS_IMP", parameters);
                                bind();
                            }
                            catch (Exception ex)
                            {
                                Alert.Show("数据库错误：" + ex.Message.ToString(), "异常信息", MessageBoxIcon.Warning);
                            }
                            finally
                            {
                                con.Close();
                            }
                        }
                        File.Delete(serverFileName);
                    }
                }
            }
            else
            {
                Alert.Show("请选择要导入的EXCEL文件！", "异常提醒", MessageBoxIcon.Warning);
            }
            #endregion
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
        protected void bind()
        {
            String Sql = @"SELECT * FROM IMP_DOC_GOODS A  WHERE  A.IMPOPER = '{0}' ORDER BY MEMO";
            int total = 0;
            GridStock.DataSource = DbHelperOra.Query(String.Format(Sql, UserAction.UserID)).Tables[0];
            DataTable dtData = PubFunc.DbGetPage(GridStock.PageIndex, GridStock.PageSize, String.Format(Sql, UserAction.UserID), ref total);
            GridStock.RecordCount = total;
            GridStock.DataSource = dtData;
            GridStock.DataBind();
            if (dtData.Rows.Count > 0)
            {
                btnSave.Enabled = true;
            }
            else
            {
                btnSave.Enabled = false;
            }
        }

        protected void GridStock_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridStock.PageIndex = e.NewPageIndex;
            highlightRows.Text = "";
            bind();
        }

        protected void GridStock_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["MEMO"].ToString();
                if (flag != "正常")
                {
                    highlightRows.Text += e.RowIndex.ToString() + ",";
                }
            }
        }
    }
}
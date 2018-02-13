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

namespace ERPProject.ERPStorage
{
    public partial class InitialStock : BillBase
    {
        //private static Int32 myindex;
        private static DataTable ViewTable;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }
        //public InitialStock()
        //{
        //    if (chkRK.Checked)
        //    {
        //        BillType = "RKD";
        //    }
        //    else
        //    {
        //        BillType = "SYD";
        //    }
        //}
        protected void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                DbHelperOra.ExecuteSql("DELETE FROM DAT_STOCK_IMP");
                ViewTable.Clear();
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
            if (!DbHelperOra.Exists("SELECT 1 FROM DAT_STOCK_IMP WHERE LRY = '" + UserAction.UserID + "' AND FLAG = 'Y'"))
            {
                Alert.Show("不存在可生成单据的数据，请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (!DbHelperOra.Exists("SELECT 1 FROM DOC_SUPPLIER WHERE SUPID = '00001'"))
            {
                Alert.Show("未定义医院供应商【00001】，请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            OracleParameter[] param = {
                                          new OracleParameter("V_USER", OracleDbType.Varchar2,20),
                                          new OracleParameter("V_TYPE",OracleDbType.Varchar2,20),
                                      };
            param[0].Value = UserAction.UserID;
            param[1].Value = ddlTYPE.SelectedValue;
            try
            {
                DbHelperOra.RunProcedure("P_STOCK_IMP", param);
                Alert.Show("提交成功，已生成对应单据信息！", "提示信息", MessageBoxIcon.Information);
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
                            string sql = @"INSERT INTO DAT_STOCK_IMP(LRY,DEPTID,GDSEQ,SL,PH,RQ_SC,YXQZ,SUPPLIER,STR1) VALUES(:LRY,:DEPTID,:GDSEQ,:SL,:PH,:RQ_SC,:YXQZ,:SUPPLIER,:STR1)";
                            OracleConnection con = new OracleConnection(DbHelperOra.connectionString);
                            OracleDataAdapter da = new OracleDataAdapter(sql, con);
                            //在批量添加数据前的准备工作
                            da.InsertCommand = new OracleCommand(sql, con);
                            OracleParameter param = new OracleParameter();

                            OracleParameter para = new OracleParameter("LRY", OracleDbType.Varchar2, 20);
                            para.Value = UserAction.UserID;
                            param = da.InsertCommand.Parameters.Add(para);

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("DEPTID", OracleDbType.Varchar2, 50));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "库房编码";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("GDSEQ", OracleDbType.Varchar2, 200));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "商品编码";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("SL", OracleDbType.Varchar2, 100));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "数量";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("PH", OracleDbType.Varchar2, 15));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "批号";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("RQ_SC", OracleDbType.Varchar2, 20));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "生产日期";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("YXQZ", OracleDbType.Varchar2, 20));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "有效期至";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("SUPPLIER", OracleDbType.Varchar2, 15));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "供应商编码";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("STR1", OracleDbType.Varchar2, 15));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "配送商编码";
                            //批量添加数据
                            try
                            {
                                con.Open();
                                da.Update(table);
                                //处理导入数据的准确性
                                OracleParameter[] parameters = new OracleParameter[]
                                {
                                     new OracleParameter("V_USER",OracleDbType.Varchar2),
                                     new OracleParameter("V_TYPE",OracleDbType.Varchar2),
                                };
                                parameters[0].Value = UserAction.UserID;
                                parameters[1].Value = "INS";
                                DbHelperOra.RunProcedure("P_STOCK_IMP", parameters);
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
                Alert.Show("请选择excel文件！");
            }
            #endregion
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                FineUIPro.BoundField flagcol = GridStock.FindColumn("MEMO") as FineUIPro.BoundField;
                if (flag == "N")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color2";
                }
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
        protected void bind()
        {
            String Sql = @"SELECT f_getdeptname(A.DEPTID) DEPTIDNAME,A.GDSEQ,A.SL,A.PH,A.RQ_SC,A.YXQZ,A.MEMO,
                                       NVL(B.HISNAME,B.GDNAME) GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,f_getproducername(A.STR1) PRODUCERNAME,f_getproducername(A.SUPPLIER) SUPPLIERNAME,
                                       B.PIZNO,B.HSJJ,A.FLAG
                                FROM DAT_STOCK_IMP A, DOC_GOODS B
                                WHERE A.GDSEQ = B.GDSEQ(+) AND A.LRY = '{0}' ORDER BY A.FLAG";
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
            bind();
        }
    }
}
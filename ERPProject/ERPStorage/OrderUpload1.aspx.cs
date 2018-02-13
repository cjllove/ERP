using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using XTBase.Utilities;

namespace ERPProject.ERPUpload
{
    public partial class OrderUpload1 : BillBase
    {
        private DataSet myDs = new DataSet();
        public static DataTable dt = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }
        }

        public void DataInit()
        {
            PubFunc.DdlDataGet("DDL_DOC_SUPPLIERNULL", ddlSUPID);
            PubFunc.DdlDataGet("DDL_SYS_DEPOT", ddlDEPTID);
            //PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", ddlDEPT);
            ddlSUPID.SelectedValue = Doc.DbGetSysPara("SUPPLIER");
            PubFunc.DdlDataGet("DDL_USER", ddlLRY, lisLRY);
            ddlLRY.SelectedValue = UserAction.UserID;
            lisLRY.SelectedValue = UserAction.UserID;
            dpkDhrq.SelectedDate = DateTime.Now;
            dpkLRRQ.SelectedDate = DateTime.Now;
            lisLRRQ.SelectedDate = DateTime.Now;
        }

        //调用存储过程将数据插入到数据表中
        protected void btnInsert_Click(object sender, EventArgs e)
        {
            if (ddlSUPID.SelectedValue == "" || ddlDEPTID.SelectedValue == "")
            {
                Alert.Show("[供应商]或[订货部门]未维护完全!", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            OracleParameter[] parameters = new OracleParameter[]
            {
                 new OracleParameter("V_SUPID",OracleDbType.Varchar2),
                 new OracleParameter("V_DEPTID",OracleDbType.Varchar2),
                 new OracleParameter("V_DATE",OracleDbType.Varchar2),
                 new OracleParameter("V_USERID",OracleDbType.Varchar2),
                 new OracleParameter("V_TYPE",OracleDbType.Varchar2),
            };
            parameters[0].Value = ddlSUPID.SelectedValue;
            parameters[1].Value = ddlDEPTID.SelectedValue;
            parameters[2].Value = dpkDhrq.Text;
            parameters[3].Value = UserAction.UserID;
            parameters[4].Value = "BILL";
            try
            {
                DbHelperOra.RunProcedure("P_Excel_GOODSORDER", parameters);
                Alert.Show("订货单生成成功!");
            }
            catch (Exception ex)
            {
                Alert.Show("因未导入数据或导入数据已被使用/r/n[" + ex.Message.ToString() + "],请检查!", "提示信息", MessageBoxIcon.Question);
            }
        }
        protected override void billClear()
        {
            DbHelperOra.Query("TRUNCATE TABLE UPLOAD_DD_TEMP");
            GridList.DataSource = null;
            GridList.DataBind();
        }
        protected void btnSelect_Click(object sender, EventArgs e)
        {
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
                        //首先清空临时表
                        DbHelperOra.Query("TRUNCATE TABLE UPLOAD_DD_TEMP");
                        if (table != null && table.Rows.Count > 0)
                        {
                            //                            string sql = @"INSERT INTO UPLOAD_DD_TEMP(CODE,NAME,GDSPEC,UNITNAME,PRICES,PROCEDURENAME,EAS_CODE,HISCODE,HISNAME,NUM_Z,UNITNAME_Z,NUM_D,UNITNAME_D,UNITNAME_C,UNITNAME_S,ORDERNUM)
                            //                                                          VALUES(:CODE,:NAME,:GDSPEC,:UNITNAME,:PRICES,:PROCEDURENAME,:EAS_CODE,:HISCODE,:HISNAME,:NUM_Z,:UNITNAME_Z,:NUM_D,:UNITNAME_D,:UNITNAME_C,:UNITNAME_S,:ORDERNUM)";
                            string sql = @"INSERT INTO UPLOAD_DD_TEMP(CODE,ORDERNUM)
                                                          VALUES(:CODE,:ORDERNUM)";
                            OracleConnection con = new OracleConnection(DbHelperOra.connectionString);
                            OracleDataAdapter da = new OracleDataAdapter(sql, con);
                            //在批量添加数据前的准备工作
                            da.InsertCommand = new OracleCommand(sql, con);
                            OracleParameter param = new OracleParameter();

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("CODE", OracleDbType.Varchar2, 20));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "ERP编码";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("NAME", OracleDbType.Varchar2, 200));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "商品名称";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("GDSPEC", OracleDbType.Varchar2, 50));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "商品规格";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("UNITNAME", OracleDbType.Varchar2, 10));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "单位";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("PRICES", OracleDbType.Varchar2, 20));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "价格";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("PROCEDURENAME", OracleDbType.Varchar2, 100));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "生产厂家";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("EAS_CODE", OracleDbType.Varchar2, 20));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "ERP编码";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("HISCODE", OracleDbType.Varchar2, 20));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "HIS编码";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("HISNAME", OracleDbType.Varchar2, 200));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "HIS名称";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("NUM_Z", OracleDbType.Varchar2, 20));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "中包装数";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("UNITNAME_Z", OracleDbType.Varchar2, 10));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "中包装名";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("NUM_D", OracleDbType.Varchar2, 20));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "大包装数";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("UNITNAME_D", OracleDbType.Varchar2, 10));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "大包装名";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("UNITNAME_S", OracleDbType.Varchar2, 10));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "订货单位";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("UNITNAME_C", OracleDbType.Varchar2, 10));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "出库单位";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("ORDERNUM", OracleDbType.Decimal, 16));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "订货数";
                            //批量添加数据
                            try
                            {
                                con.Open();
                                da.Update(table);
                                //处理不符合要求数据
                                OracleParameter[] parameters = new OracleParameter[]
                                {
                                     new OracleParameter("V_SUPID",OracleDbType.Varchar2),
                                     new OracleParameter("V_DEPTID",OracleDbType.Varchar2),
                                     new OracleParameter("V_DATE",OracleDbType.Varchar2),
                                     new OracleParameter("V_USERID",OracleDbType.Varchar2),
                                     new OracleParameter("V_TYPE",OracleDbType.Varchar2),
                                };
                                parameters[0].Value = "";
                                parameters[1].Value = "";
                                parameters[2].Value = "";
                                parameters[3].Value = "";
                                parameters[4].Value = "INS";
                                DbHelperOra.RunProcedure("P_Excel_GOODSORDER", parameters);
                                GridColumn MEMO = GridList.FindColumn("MEMOOrder");
                                MEMO.Hidden = true;
                                string mysql = @"SELECT A.ORDERNUM,A.CODE,B.GDSEQ,A.MEMO,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,B.HSJJ,
                                            f_getproducername(B.PRODUCER) PROCEDURENAME,B.BAR3,B.HISCODE,B.HISNAME
                                        FROM UPLOAD_DD_TEMP A,doc_goods B 
                                        WHERE A.FLAG = 'N' AND A.CODE = B.GDSEQ";
                                DataTable dtBill = new DataTable();
                                dtBill = DbHelperOra.Query(mysql).Tables[0];
                                GridList.DataSource = dtBill;
                                GridList.DataBind();
                                Alert.Show("数据导入成功,共导入[" + dtBill.Rows.Count.ToString() + "]条数据！", "消息提示", MessageBoxIcon.Information);
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
        }
        protected void btnError_Click(object sender, EventArgs e)
        {
            if (!DbHelperOra.Exists("SELECT 1 FROM UPLOAD_DD_TEMP WHERE FLAG ='D'"))
            {
                Alert.Show("不存在错误信息");
                return;
            }
            GridColumn MEMO = GridList.FindColumn("MEMOOrder");
            MEMO.Hidden = false;
            string mysql = @"SELECT A.ORDERNUM,A.CODE,B.GDSEQ,A.MEMO,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,B.HSJJ,
                                            f_getproducername(B.PRODUCER) PROCEDURENAME,B.BAR3,B.HISCODE,B.HISNAME
                                        FROM UPLOAD_DD_TEMP A,doc_goods B 
                                        WHERE A.FLAG = 'D' AND A.CODE = B.GDSEQ";
            DataTable dtBill = new DataTable();
            dtBill = DbHelperOra.Query(mysql).Tables[0];
            GridList.DataSource = dtBill;
            GridList.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            GridColumn MEMO = GridList.FindColumn("MEMOOrder");
            MEMO.Hidden = true;
            string mysql = @"SELECT A.ORDERNUM,A.CODE,B.GDSEQ,A.MEMO,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,B.HSJJ,
                                            f_getproducername(B.PRODUCER) PROCEDURENAME,B.BAR3,B.HISCODE,B.HISNAME
                                        FROM UPLOAD_DD_TEMP A,doc_goods B 
                                        WHERE A.FLAG = 'N' AND A.CODE = B.GDSEQ";
            DataTable dtBill = new DataTable();
            dtBill = DbHelperOra.Query(mysql).Tables[0];
            GridList.DataSource = dtBill;
            GridList.DataBind();
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
            if (this.FileUpd.HasFile)
            {
                string toFilePath = "~/ERPUpload/DownloadExcel/";

                //获得文件扩展名
                string fileNameExt = Path.GetExtension(this.FileUpd.FileName).ToLower();

                if (!ValidateFileType(fileNameExt))
                {
                    Alert.Show("无效的文件类型！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }

                //验证合法的文件
                if (CheckFileExt(fileNameExt))
                {
                    //生成将要保存的随机文件名
                    string fileName = this.FileUpd.FileName.Substring(0, this.FileUpd.FileName.IndexOf(".")) + DateTime.Now.ToString("yyyyMMddHHmmss") + fileNameExt;

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
                    this.FileUpd.SaveAs(serverFileName);

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
                        //首先清空临时表
                        DbHelperOra.Query("TRUNCATE TABLE UPLOAD_DS_TEMP");
                        if (table != null && table.Rows.Count > 0)
                        {
                            //                            string sql = @"INSERT INTO UPLOAD_DS_TEMP(DEPTID,CODE,NAME,GDSPEC,UNITNAME,PRICES,PROCEDURENAME,DSSL_CK,DSHL_CK,DSSL,DSHL)
                            //                                                          VALUES(:DEPTID,:CODE,:NAME,:GDSPEC,:UNITNAME,:PRICES,:PROCEDURENAME,:DSSL_CK,:DSHL_CK,:DSSL,:DSHL)";
                            string sql = @"INSERT INTO UPLOAD_DS_TEMP(DEPTID,CODE,DSSL_CK,DSHL_CK,DSSL,DSHL)
                                                          VALUES(:DEPTID,:CODE,:DSSL_CK,:DSHL_CK,:DSSL,:DSHL)";
                            OracleConnection con = new OracleConnection(DbHelperOra.connectionString);
                            OracleDataAdapter da = new OracleDataAdapter(sql, con);
                            //在批量添加数据前的准备工作
                            da.InsertCommand = new OracleCommand(sql, con);
                            OracleParameter param = new OracleParameter();

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("DEPTID", OracleDbType.Varchar2, 20));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "科室编码";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("DEPTNAME", OracleDbType.Varchar2, 20));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "科室名称";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("CODE", OracleDbType.Varchar2, 20));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "ERP编码";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("NAME", OracleDbType.Varchar2, 200));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "商品名称";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("GDSPEC", OracleDbType.Varchar2, 50));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "商品规格";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("UNITNAME", OracleDbType.Varchar2, 10));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "单位";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("PRICES", OracleDbType.Varchar2, 20));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "价格";

                            //param = da.InsertCommand.Parameters.Add(new OracleParameter("PROCEDURENAME", OracleDbType.Varchar2, 100));
                            //param.SourceVersion = DataRowVersion.Current;
                            //param.SourceColumn = "生产厂家";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("DSSL_CK", OracleDbType.Decimal, 10));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "参考定数数量";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("DSHL_CK", OracleDbType.Decimal, 10));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "参考定数含量";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("DSSL", OracleDbType.Decimal, 10));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "定数数量";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("DSHL", OracleDbType.Decimal, 10));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "定数含量";
                            //批量添加数据
                            try
                            {
                                con.Open();
                                da.Update(table);
                                //处理错误数据
                                OracleParameter[] parameters = new OracleParameter[]
                                {
                                     new OracleParameter("V_DEPTID",OracleDbType.Varchar2),
                                     new OracleParameter("V_USERID",OracleDbType.Varchar2),
                                     new OracleParameter("V_TYPE",OracleDbType.Varchar2),
                                     new OracleParameter("V_ZDKC",OracleDbType.Decimal),
                                     new OracleParameter("V_ZGKC",OracleDbType.Decimal),
                                };
                                parameters[0].Value = "";//ddlDEPT.SelectedValue;
                                parameters[1].Value = "";
                                parameters[2].Value = "INS";
                                parameters[3].Value = 0;
                                parameters[4].Value = 0;
                                DbHelperOra.RunProcedure("P_Excel_GOODSDS", parameters);

                                string mysql = @"SELECT A.DEPTID,F_GETDEPTNAME(A.DEPTID) DEPTNAME,B.GDNAME,B.GDSPEC,F_GETDEPTNAME(B.UNIT) UNITNAME,B.HSJJ,B.GDSEQ,
                                F_GETPRODUCERNAME(B.PRODUCER) PROCEDURENAME,A.DSHL_CK,A.DSSL_CK,A.DSHL,A.DSSL
                                FROM UPLOAD_DS_TEMP A,DOC_GOODS B 
                                WHERE A.CODE =B.GDSEQ AND A.FLAG = 'N'";
                                DataTable dtBill = new DataTable();
                                dtBill = DbHelperOra.Query(mysql).Tables[0];
                                GridDs.DataSource = dtBill;
                                GridDs.DataBind();
                                string errNum = DbHelperOra.GetSingle("SELECT COUNT(1) FROM UPLOAD_DS_TEMP WHERE FLAG = 'D'").ToString();
                                if (errNum != "0")
                                {
                                    Alert.Show("数据导入成功,成功导入[" + dtBill.Rows.Count.ToString() + "]条数据,失败[" + errNum + "]条数据！", "消息提示", MessageBoxIcon.Information);
                                }
                                else
                                {
                                    Alert.Show("数据导入成功,共导入[" + dtBill.Rows.Count.ToString() + "]条数据！", "消息提示", MessageBoxIcon.Information);
                                }
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
        }

        protected void btnINPUT_Click(object sender, EventArgs e)
        {
            if (!cbxDS.Checked && !cbxKC.Checked)
            {
                Alert.Show("请勾选操作方式!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            OracleParameter[] parameters = new OracleParameter[]
            {
                 new OracleParameter("V_DEPTID",OracleDbType.Varchar2),
                 new OracleParameter("V_USERID",OracleDbType.Varchar2),
                 new OracleParameter("V_TYPE",OracleDbType.Varchar2),
                 new OracleParameter("V_ZDKC",OracleDbType.Varchar2),
                 new OracleParameter("V_ZGKC",OracleDbType.Varchar2),
            };
            parameters[0].Value = "";//ddlDEPT.SelectedValue;
            parameters[1].Value = UserAction.UserID;
            parameters[3].Value = nbxKCXX.Text;
            parameters[4].Value = nbxKCSX.Text;
            string type = "";
            if (cbxDS.Checked && cbxKC.Checked)
            {
                type = "ALL";
            }
            if (cbxKC.Checked && !cbxDS.Checked)
            {
                type = "KC";
                if (PubFunc.StrIsEmpty(nbxKCXX.Text) || PubFunc.StrIsEmpty(nbxKCSX.Text))
                {
                    Alert.Show("[库存上限基数]或[库存下限基数]未维护完全,请检查!");
                    return;
                }
            }
            if (cbxDS.Checked && cbxKC.Checked)
            {
                type = "DS";
            }
            parameters[2].Value = type;
            try
            {
                DbHelperOra.RunProcedure("P_Excel_GOODSDS", parameters);
                if (cbxDS.Checked && cbxKC.Checked)
                {
                    Alert.Show("定数信息、库存上下限信息修改成功!");
                }
                else if (cbxDS.Checked)
                {
                    Alert.Show("定数信息修改成功!");
                }
                else if (cbxKC.Checked)
                {
                    Alert.Show("库存上下限信息修改成功!");
                }
            }
            catch (Exception ex)
            {
                Alert.Show("因未导入数据或导入数据已被使用,生成数据失败,请检查!", "提示信息", MessageBoxIcon.Question);
            }
        }

        protected void Btnclr_Click(object sender, EventArgs e)
        {
            DbHelperOra.Query("TRUNCATE TABLE UPLOAD_DS_TEMP");
            GridDs.DataSource = null;
            GridDs.DataBind();
        }
        protected void btnerr_Click(object sender, EventArgs e)
        {
            if (!DbHelperOra.Exists("SELECT 1 FROM UPLOAD_DS_TEMP A WHERE A.FLAG = 'D'"))
            {
                Alert.Show("不存在错误信息！");
                return;
            }
            GridColumn MEMO = GridDs.FindColumn("MEMODS");
            MEMO.Hidden = false;
            string mysql = @"SELECT A.DEPTID,F_GETDEPTNAME(A.DEPTID) DEPTNAME,B.GDNAME,B.GDSPEC,F_GETDEPTNAME(B.UNIT) UNITNAME,B.HSJJ,B.GDSEQ,A.DSHL_CK,A.DSSL_CK,
                F_GETPRODUCERNAME(B.PRODUCER) PROCEDURENAME,A.DSHL,A.DSSL,A.MEMO,b.num_zhongbz,b.num_dabz,f_getunitname(b.unit_zhongbz) UNITNAME_Z,f_getunitname(b.unit_dabz) UNITNAME_D,
                   DECODE(b.unit_order,'D','大包装','Z','中包装','小包装') UNITNAME_O,DECODE(b.unit_sell,'D','大包装','Z','中包装','小包装') UNITNAME_C
                FROM UPLOAD_DS_TEMP A,DOC_GOODS B 
                WHERE A.CODE =B.GDSEQ AND A.FLAG = 'D'";
            DataTable dtBill = new DataTable();
            dtBill = DbHelperOra.Query(mysql).Tables[0];
            GridDs.DataSource = dtBill;
            GridDs.DataBind();
        }

        protected void btnSearchDS_Click(object sender, EventArgs e)
        {
            GridColumn MEMO = GridDs.FindColumn("MEMODS");
            MEMO.Hidden = true;
            string mysql = @"SELECT A.DEPTID,F_GETDEPTNAME(A.DEPTID) DEPTNAME,B.GDNAME,B.GDSPEC,F_GETDEPTNAME(B.UNIT) UNITNAME,B.HSJJ,B.GDSEQ,A.DSHL_CK,A.DSSL_CK,
                F_GETPRODUCERNAME(B.PRODUCER) PROCEDURENAME,A.DSHL,A.DSSL,A.MEMO,b.num_zhongbz,b.num_dabz,f_getunitname(b.unit_zhongbz) UNITNAME_Z,f_getunitname(b.unit_dabz) UNITNAME_D,
                   DECODE(b.unit_order,'D','大包装','Z','中包装','小包装') UNITNAME_O,DECODE(b.unit_sell,'D','大包装','Z','中包装','小包装') UNITNAME_C
                FROM UPLOAD_DS_TEMP A,DOC_GOODS B 
                WHERE A.CODE =B.GDSEQ AND A.FLAG = 'N'";
            DataTable dtBill = new DataTable();
            dtBill = DbHelperOra.Query(mysql).Tables[0];
            GridDs.DataSource = dtBill;
            GridDs.DataBind();
        }
        #region 数据分析
        protected void btnJCXX_Click(object sender, EventArgs e)
        {
            //原始数据导入系统
            if (this.fpdExcelJC.HasFile)
            {
                string toFilePath = "~/ERPUpload/DownloadExcel/";

                //获得文件扩展名
                string fileNameExt = Path.GetExtension(this.fpdExcelJC.FileName).ToLower();

                if (!ValidateFileType(fileNameExt))
                {
                    Alert.Show("无效的文件类型！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }

                //验证合法的文件
                if (CheckFileExt(fileNameExt))
                {
                    //生成将要保存的随机文件名
                    string fileName = this.fpdExcelJC.FileName.Substring(0, this.fpdExcelJC.FileName.IndexOf(".")) + DateTime.Now.ToString("yyyyMMddHHmmss") + fileNameExt;

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
                    this.fpdExcelJC.SaveAs(serverFileName);

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
                        //首先清空临时表
                        DbHelperOra.Query("TRUNCATE TABLE TEMP_EXCEL_JCXX");
                        if (table != null && table.Rows.Count > 0)
                        {
                            string sql = @"INSERT INTO TEMP_EXCEL_JCXX(HISCODE,GDSEQ,HSJJ,BAR3,HISNAME)
                                                          VALUES(:HISCODE,:GDSEQ,:HSJJ,:BAR3,:HISNAME)";
                            OracleConnection con = new OracleConnection(DbHelperOra.connectionString);
                            OracleDataAdapter da = new OracleDataAdapter(sql, con);
                            //在批量添加数据前的准备工作
                            da.InsertCommand = new OracleCommand(sql, con);
                            OracleParameter param = new OracleParameter();

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("HISCODE", OracleDbType.Varchar2, 10));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "HIS编码";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("GDSEQ", OracleDbType.Varchar2, 50));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "ERP编码";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("HSJJ", OracleDbType.Decimal, 15));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "价格";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("BAR3", OracleDbType.Varchar2, 50));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "ERP编码";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("HISNAME", OracleDbType.Varchar2, 10));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "商品名称";
                            //批量添加数据
                            try
                            {
                                con.Open();
                                da.Update(table);
                                OracleParameter[] parameters = new OracleParameter[]
                                {
                                     new OracleParameter("V_BILL",OracleDbType.Varchar2),
                                     new OracleParameter("V_TYPE",OracleDbType.Varchar2),
                                };
                                parameters[0].Value = "";
                                parameters[1].Value = "JCXX";
                                DbHelperOra.RunProcedure("P_ExcelGOODS", parameters);
                                btnGoodsSed_Click(null, null);
                                Alert.Show("Excel信息导入成功,请查看导入信息!");
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
        }
        protected void btnSrchDh_Click(object sender, EventArgs e)
        {
            ColumnHid(true, false);
            //生成查询SQL语句
            string Sql = @"SELECT B.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,B.HSJJ,f_getproducername(B.PRODUCER) PROCEDURENAME,B.PIZNO,B.BAR3,A.HISCODE,B.HISNAME,
               b.num_zhongbz,b.num_dabz,f_getunitname(b.unit_zhongbz) UNITNAME_Z,f_getunitname(b.unit_dabz) UNITNAME_D,
               DECODE(b.unit_order,'D','大包装','Z','中包装','小包装') UNITNAME_O,DECODE(b.unit_sell,'D','大包装','Z','中包装','小包装') UNITNAME_C
                ,A.DHL ORDERNUM,A.SL
                FROM (SELECT HISCODE,GDSEQ,DHL,SL FROM TEMP_EXCEL_DS WHERE FLAG = 'N' GROUP BY HISCODE,GDSEQ,DHL,SL) A,DOC_GOODS B
                WHERE A.GDSEQ = B.GDSEQ";
            string streach = "";
            if (!PubFunc.StrIsEmpty(trbSearch.Text))
            {
                streach += string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%')", trbSearch.Text);
            }
            streach += " ORDER BY B.GDSEQ";
            GridIns.DataSource = DbHelperOra.Query(Sql + streach);
            GridIns.DataBind();
        }
        protected void btnSrchDs_Click(object sender, EventArgs e)
        {
            ColumnHid(false, true);
            //生成查询SQL语句
            //            string Sql = @"SELECT f_getdeptname(A.DEPTID) DEPTIDNAME,B.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,B.HSJJ,f_getproducername(B.PRODUCER) PROCEDURENAME,B.PIZNO,B.BAR3,A.HISCODE,A.HISNAME
            //                ,ceil((A.SL/(TO_DATE((SELECT MAX(YWRQ) FROM UPLOAD_EXCEL_GOODS),'YYYY-MM-DD')- TO_DATE((SELECT MIN(YWRQ) FROM UPLOAD_EXCEL_GOODS),'YYYY-MM-DD')+1)*NVL((SELECT VALUE FROM SYS_PARA WHERE CODE = 'DSDAY'),7))/DECODE(NVL(DECODE(B.UNIT_SELL,'D',B.NUM_DABZ,'Z',B.NUM_ZHONGBZ,B.BZHL),1),0,1,NVL(DECODE(B.UNIT_SELL,'D',B.NUM_DABZ,'Z',B.NUM_ZHONGBZ,B.BZHL),1))) DSHL
            //                ,DECODE(NVL(DECODE(B.UNIT_SELL,'D',B.NUM_DABZ,'Z',B.NUM_ZHONGBZ,B.BZHL),1),0,1,NVL(DECODE(B.UNIT_SELL,'D',B.NUM_DABZ,'Z',B.NUM_ZHONGBZ,B.BZHL),1)) DSSL
            //                ,b.num_zhongbz,b.num_dabz,f_getunitname(b.unit_zhongbz) UNITNAME_Z,f_getunitname(b.unit_dabz) UNITNAME_D,
            //               DECODE(b.unit_order,'D','大包装','Z','中包装','小包装') UNITNAME_O,DECODE(b.unit_sell,'D','大包装','Z','中包装','小包装') UNITNAME_C                
            //                FROM (SELECT HISCODE,HISNAME,GDSEQ,deptid,SUM(NVL(SL,0)) SL FROM UPLOAD_EXCEL_GOODS WHERE FLAG = 'N' GROUP BY HISCODE,HISNAME,deptid,GDSEQ) A,DOC_GOODS B
            //                WHERE A.GDSEQ = B.GDSEQ";
            string Sql = @"SELECT f_getdeptname(A.DEPTID) DEPTIDNAME,B.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,B.HSJJ,f_getproducername(B.PRODUCER) PROCEDURENAME,B.BAR3,B.PIZNO,
                   A.HISCODE,B.HISNAME,A.SL,A.DSHL,A.DSSL,b.num_zhongbz,b.num_dabz,f_getunitname(b.unit_zhongbz) UNITNAME_Z,f_getunitname(b.unit_dabz) UNITNAME_D,
                   DECODE(b.unit_order,'D','大包装','Z','中包装','小包装') UNITNAME_O,DECODE(b.unit_sell,'D','大包装','Z','中包装','小包装') UNITNAME_C,A.SL
                    FROM TEMP_EXCEL_DS A,DOC_GOODS B
                    WHERE A.GDSEQ = B.GDSEQ";
            string streach = "";
            if (!PubFunc.StrIsEmpty(trbSearch.Text))
            {
                streach += string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%')", trbSearch.Text);
            }
            streach += " ORDER BY A.DEPTID,B.GDSEQ";
            GridIns.DataSource = DbHelperOra.Query(Sql + streach);
            GridIns.DataBind();
        }
        private void ColumnHid(bool DS, bool DH)
        {
            GridColumn MEMO = GridIns.FindColumn("MEMO");
            MEMO.Hidden = true;

            GridColumn DEPTIDNAME = GridIns.FindColumn("DEPTIDNAME");
            GridColumn DSSL = GridIns.FindColumn("DSSL");
            GridColumn DSHL = GridIns.FindColumn("DSHL");
            GridColumn NUM_ZHONGBZ = GridIns.FindColumn("NUM_ZHONGBZ");
            GridColumn UNITNAME_Z = GridIns.FindColumn("UNITNAME_Z");
            GridColumn NUM_DABZ = GridIns.FindColumn("NUM_DABZ");
            GridColumn UNITNAME_D = GridIns.FindColumn("UNITNAME_D");
            GridColumn UNITNAME_O = GridIns.FindColumn("UNITNAME_O");
            GridColumn UNITNAME_C = GridIns.FindColumn("UNITNAME_C");
            GridColumn ORDERNUM = GridIns.FindColumn("ORDERNUM");

            DEPTIDNAME.Hidden = DS;
            DSSL.Hidden = DS;
            DSHL.Hidden = DS;
            UNITNAME_Z.Hidden = DS;
            NUM_ZHONGBZ.Hidden = DS;
            NUM_DABZ.Hidden = DS;
            UNITNAME_D.Hidden = DS;
            UNITNAME_O.Hidden = DS;
            UNITNAME_C.Hidden = DS;
            ORDERNUM.Hidden = DH;
        }
        protected void ExlIn_Click(object sender, EventArgs e)
        {
            //原始数据导入系统
            if (this.fueExl.HasFile)
            {
                string toFilePath = "~/ERPUpload/DownloadExcel/";

                //获得文件扩展名
                string fileNameExt = Path.GetExtension(this.fueExl.FileName).ToLower();

                if (!ValidateFileType(fileNameExt))
                {
                    Alert.Show("无效的文件类型！", "消息提示", MessageBoxIcon.Warning);
                    return;
                }

                //验证合法的文件
                if (CheckFileExt(fileNameExt))
                {
                    //生成将要保存的随机文件名
                    string fileName = this.fueExl.FileName.Substring(0, this.fueExl.FileName.IndexOf(".")) + DateTime.Now.ToString("yyyyMMddHHmmss") + fileNameExt;

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
                    this.fueExl.SaveAs(serverFileName);

                    if (File.Exists(serverFileName))
                    {
                        DataTable table = new DataTable();

                        if (fileNameExt == ".xlsx")
                        {
                            table = ExcelHelper.ImportExcelxtoDt(serverFileName); //导入excel2007
                        }
                        else
                        {
                            table = ExcelHelper.ImportExceltoDt(serverFileName);//导入excel2003
                        }
                        //首先清空临时表
                        DbHelperOra.Query("TRUNCATE TABLE UPLOAD_EXCEL_GOODS");
                        DbHelperOra.Query("TRUNCATE TABLE TEMP_EXCEL_DS");
                        if (table != null && table.Rows.Count > 0)
                        {
                            string sql = @"INSERT INTO UPLOAD_EXCEL_GOODS(DEPTID,HISCODE,HISNAME,HISSPEC,HISUNIT,PRODUCER,SL,YWRQ)
                                                          VALUES(:DEPTID,:HISCODE,:HISNAME,:HISSPEC,:HISUNIT,:PRODUCER,:SL,:YWRQ)";
                            OracleConnection con = new OracleConnection(DbHelperOra.connectionString);
                            OracleDataAdapter da = new OracleDataAdapter(sql, con);
                            //在批量添加数据前的准备工作
                            da.InsertCommand = new OracleCommand(sql, con);
                            OracleParameter param = new OracleParameter();

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("DEPTID", OracleDbType.Varchar2, 10));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "科室编码";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("HISCODE", OracleDbType.Varchar2, 50));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "HIS编码";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("HISNAME", OracleDbType.Varchar2, 100));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "HIS名称";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("HISSPEC", OracleDbType.Varchar2, 50));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "HIS规格";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("HISUNIT", OracleDbType.Varchar2, 10));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "HIS单位";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("PRODUCER", OracleDbType.Varchar2, 100));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "生产厂家";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("SL", OracleDbType.Decimal));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "数量";

                            param = da.InsertCommand.Parameters.Add(new OracleParameter("YWRQ", OracleDbType.Varchar2, 10));
                            param.SourceVersion = DataRowVersion.Current;
                            param.SourceColumn = "业务日期";
                            //批量添加数据
                            try
                            {
                                con.Open();
                                da.Update(table);
                                OracleParameter[] parameters = new OracleParameter[]
                                {
                                     new OracleParameter("V_BILL",OracleDbType.Varchar2),
                                     new OracleParameter("V_TYPE",OracleDbType.Varchar2),
                                };
                                parameters[0].Value = "";
                                parameters[1].Value = "1";
                                DbHelperOra.RunProcedure("P_ExcelGOODS", parameters);
                                Streach(false);
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
        }
        private void Streach(bool errShow)
        {
            string mysql = "";
            if (errShow)
            {
                mysql = @"SELECT A.*,F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,B.GDNAME,B.GDSPEC,F_GETUNITNAME(B.UNIT) UNITNAME,B.HSJJ,F_GETPRODUCERNAME(B.PRODUCER) PROCEDURENAME
                ,B.PIZNO,B.BAR3,B.NUM_ZHONGBZ,B.NUM_DABZ,F_GETUNITNAME(B.UNIT_ZHONGBZ) UNITNAME_Z,F_GETUNITNAME(B.UNIT_DABZ) UNITNAME_D,
                 DECODE(B.UNIT_ORDER,'D','大包装','Z','中包装','小包装') UNITNAME_O,DECODE(B.UNIT_SELL,'D','大包装','Z','中包装','小包装') UNITNAME_C
                FROM TEMP_EXCEL_DS A,DOC_GOODS B
                WHERE A.GDSEQ = B.GDSEQ(+) AND A.FLAG = 'D' ORDER BY A.GDSEQ,A.DEPTID";
                DataTable dtBill = new DataTable();
                dtBill = DbHelperOra.Query(mysql).Tables[0];
                GridIns.DataSource = dtBill;
                GridIns.DataBind();
                //Alert.Show("共导入[" + dtBill.Rows.Count.ToString() + "]条失败数据！", "消息提示", MessageBoxIcon.Information);
            }
            else
            {
                mysql = @"SELECT A.*,F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,B.GDNAME,B.GDSPEC,F_GETUNITNAME(B.UNIT) UNITNAME,B.HSJJ,F_GETPRODUCERNAME(B.PRODUCER) PROCEDURENAME
                ,B.PIZNO,B.BAR3,B.NUM_ZHONGBZ,B.NUM_DABZ,F_GETUNITNAME(B.UNIT_ZHONGBZ) UNITNAME_Z,F_GETUNITNAME(B.UNIT_DABZ) UNITNAME_D,
                 DECODE(B.UNIT_ORDER,'D','大包装','Z','中包装','小包装') UNITNAME_O,DECODE(B.UNIT_SELL,'D','大包装','Z','中包装','小包装') UNITNAME_C
                FROM TEMP_EXCEL_DS A,DOC_GOODS B
                WHERE A.GDSEQ = B.GDSEQ(+) AND A.FLAG = 'N' ORDER BY A.GDSEQ,A.DEPTID";
                DataTable dtBill = new DataTable();
                dtBill = DbHelperOra.Query(mysql).Tables[0];
                GridIns.DataSource = dtBill;
                GridIns.DataBind();
                string numDel = DbHelperOra.GetSingle("SELECT COUNT(1) FROM UPLOAD_EXCEL_GOODS WHERE FLAG = 'D'").ToString();
                Alert.Show("数据导入成功,共导入[" + dtBill.Rows.Count.ToString() + "]条数据,有[" + numDel + "]条数据因Excel未维护完全导入失败！", "消息提示", MessageBoxIcon.Information);
            }
        }
        protected void ErrSrch_Click(object sender, EventArgs e)
        {
            if (!DbHelperOra.Exists("SELECT 1 FROM TEMP_EXCEL_DS A WHERE A.FLAG = 'D'"))
            {
                Alert.Show("不存在错误信息");
                return;
            }
            GridColumn MEMO = GridIns.FindColumn("MEMO");
            MEMO.Hidden = false;
            Streach(true);
        }
        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            GridColumn DEPTIDNAME = GridIns.FindColumn("DEPTIDNAME");
            if (DEPTIDNAME.Hidden)
            {
                //订货查询
                btnSrchDh_Click(null, null);
            }
            else
            {
                btnSrchDs_Click(null, null);
            }
        }
        protected void btnDS_Click(object sender, EventArgs e)
        {
            //            string Sql = @"SELECT A.DEPTID 科室编码,f_getdeptname(A.DEPTID) 科室名称,B.GDSEQ ERP编码,B.GDNAME 商品名称,B.GDSPEC 商品规格,f_getunitname(B.UNIT) 单位,B.HSJJ 价格,f_getproducername(B.PRODUCER) 生产厂家
            //                ,NVL(DECODE(B.UNIT_SELL,'D',B.NUM_DABZ,'Z',B.NUM_ZHONGBZ,B.BZHL),1) 参考定数数量
            //                ,ceil((A.SL/(TO_DATE((SELECT MAX(YWRQ) FROM UPLOAD_EXCEL_GOODS),'YYYY-MM-DD')- TO_DATE((SELECT MIN(YWRQ) FROM UPLOAD_EXCEL_GOODS),'YYYY-MM-DD')+1)*NVL((SELECT VALUE FROM SYS_PARA WHERE CODE = 'DSDAY'),7))/NVL(DECODE(B.UNIT_SELL,'D',B.NUM_DABZ,'Z',B.NUM_ZHONGBZ,B.BZHL),1)) 参考定数含量
            //                ,'' 定数数量,'' 定数含量
            //                FROM (SELECT HISCODE,HISNAME,GDSEQ,deptid,SUM(NVL(SL,0)) SL FROM UPLOAD_EXCEL_GOODS WHERE FLAG = 'N' GROUP BY HISCODE,HISNAME,deptid,GDSEQ) A,DOC_GOODS B
            //                WHERE A.GDSEQ = B.GDSEQ";
            string Sql = @"SELECT A.DEPTID 科室编码,f_getdeptname(A.DEPTID) 科室名称,B.GDSEQ ERP编码,B.GDNAME 商品名称,B.GDSPEC 商品规格,f_getunitname(B.UNIT) 单位,B.HSJJ 价格,f_getproducername(B.PRODUCER) 生产厂家
                ,A.SL 日均用量,A.DSSL 参考定数数量,A.DSHL 参考定数含量,'' 定数数量,'' 定数含量
                FROM TEMP_EXCEL_DS A,DOC_GOODS B
                WHERE A.GDSEQ = B.GDSEQ";
            string streach = "";
            if (!PubFunc.StrIsEmpty(trbSearch.Text))
            {
                streach += string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%')", trbSearch.Text);
            }
            streach += " ORDER BY A.DEPTID,B.GDSEQ";

            DataTable dt = DbHelperOra.Query(Sql + streach).Tables[0];

            if (dt == null || dt.Rows.Count == 0)
            {
                Alert.Show("暂时没有符合要求的数据，无法导出", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            ExcelHelper.ExportByWeb(dt, "科室定数信息", string.Format("科室定数信息_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
            btnDS.Enabled = true;
            btnSrchDs_Click(null, null);
        }
        protected void btnDH_Click(object sender, EventArgs e)
        {
            string Sql = @"SELECT B.GDSEQ ERP编码,B.GDNAME 商品名称,B.GDSPEC 商品规格,f_getunitname(B.UNIT) 单位,B.HSJJ 价格,f_getproducername(B.PRODUCER) 生产厂家,B.BAR3 ERP编码,A.HISCODE HIS编码,B.HISNAME HIS名称
                ,A.SL 日均用量,A.DHL 订货数
                FROM (SELECT HISCODE,DHL,GDSEQ,SL FROM TEMP_EXCEL_DS WHERE FLAG = 'N' GROUP BY HISCODE,DHL,GDSEQ,SL) A,DOC_GOODS B
                WHERE A.GDSEQ = B.GDSEQ";
            string streach = "";
            if (!PubFunc.StrIsEmpty(trbSearch.Text))
            {
                streach += string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%')", trbSearch.Text);
            }
            streach += " ORDER BY B.GDSEQ";

            DataTable dt = DbHelperOra.Query(Sql + streach).Tables[0];

            if (dt == null || dt.Rows.Count == 0)
            {
                Alert.Show("暂时没有符合要求的数据，无法导出", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            ExcelHelper.ExportByWeb(dt, "订货信息", string.Format("订货信息_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
            btnDH.Enabled = true;
            btnSrchDh_Click(null, null);
        }
        #endregion

        protected void cbxKC_CheckedChanged(object sender, CheckedEventArgs e)
        {
            if (cbxKC.Checked)
            {
                nbxKCXX.Enabled = true;
                nbxKCSX.Enabled = true;
            }
            else
            {
                nbxKCXX.Enabled = false;
                nbxKCSX.Enabled = false;
            }
        }
        protected void bntClr_Click(object sender, EventArgs e)
        {
            DbHelperOra.Query("TRUNCATE TABLE UPLOAD_DD_TEMP");
            GridList.DataSource = null;
            GridList.DataBind();
        }

        protected void bunClear_Click(object sender, EventArgs e)
        {
            DbHelperOra.Query("TRUNCATE TABLE UPLOAD_EXCEL_GOODS");
            GridIns.DataSource = null;
            GridIns.DataBind();
        }

        protected void btnRecy_Click(object sender, EventArgs e)
        {
            //重新计算定数
            DbHelperOra.ExecuteSql("UPDATE UPLOAD_EXCEL_GOODS SET FLAG = 'N'");
            OracleParameter[] parameters = new OracleParameter[]
                                {
                                     new OracleParameter("V_BILL",OracleDbType.Varchar2),
                                     new OracleParameter("V_TYPE",OracleDbType.Varchar2),
                                };
            parameters[0].Value = "";
            parameters[1].Value = "1";
            DbHelperOra.RunProcedure("P_ExcelGOODS", parameters);
            Streach(false);
        }

        protected void BtnGoodsErr_Click(object sender, EventArgs e)
        {
            if (!DbHelperOra.Exists("SELECT 1 FROM TEMP_EXCEL_JCXX D WHERE FLAG = 'D'"))
            {
                Alert.Show("不存在错误信息!");
                return;
            }
            ColumnHid(true, true);
            GridColumn MEMO = GridIns.FindColumn("MEMO");
            MEMO.Hidden = false;
            string mysql = @"SELECT A.GDSEQ,B.GDNAME,B.GDSPEC,F_GETUNITNAME(B.UNIT) UNITNAME,A.HSJJ,F_GETPRODUCERNAME(B.PRODUCER) PROCEDURENAME
                    ,B.PIZNO,B.BAR3,A.HISCODE,A.HISNAME,A.MEMO
                    FROM TEMP_EXCEL_JCXX A,DOC_GOODS B
                    WHERE A.GDSEQ = B.GDSEQ(+) AND A.FLAG = 'D' ORDER BY A.GDSEQ";
            DataTable dtBill = new DataTable();
            dtBill = DbHelperOra.Query(mysql).Tables[0];
            GridIns.DataSource = dtBill;
            GridIns.DataBind();
        }

        protected void btnGoodsSed_Click(object sender, EventArgs e)
        {
            ColumnHid(true, true);
            GridColumn MEMO = GridIns.FindColumn("MEMO");
            MEMO.Hidden = true;
            string mysql = @"SELECT A.GDSEQ,B.GDNAME,B.GDSPEC,F_GETUNITNAME(B.UNIT) UNITNAME,A.HSJJ,F_GETPRODUCERNAME(B.PRODUCER) PROCEDURENAME
                    ,B.PIZNO,B.BAR3,A.HISCODE,A.HISNAME,A.MEMO
                    FROM TEMP_EXCEL_JCXX A,DOC_GOODS B
                    WHERE A.GDSEQ = B.GDSEQ(+) AND A.FLAG = 'N' ORDER BY A.GDSEQ";
            DataTable dtBill = new DataTable();
            dtBill = DbHelperOra.Query(mysql).Tables[0];
            GridIns.DataSource = dtBill;
            GridIns.DataBind();
        }

        protected void btnInssys_Click(object sender, EventArgs e)
        {
            OracleParameter[] parameters = new OracleParameter[]
                                {
                                     new OracleParameter("V_BILL",OracleDbType.Varchar2),
                                     new OracleParameter("V_TYPE",OracleDbType.Varchar2),
                                };
            if (CbxAll.Checked)
            {
                parameters[0].Value = "1";
            }
            else
            {
                parameters[0].Value = "0";
            }

            parameters[1].Value = "JCXX_INS";
            DbHelperOra.RunProcedure("P_ExcelGOODS", parameters);
            Alert.Show("导入系统成功");
        }

        protected void btnExp_Click(object sender, EventArgs e)
        {
            string Sql = @"SELECT '' 医院名称,'' HIS编码,'' 商品名称,'' 规格,'' 单位,'' 价格,'' 生产厂家,'' 注册证号,'' ERP编码,'' ERP编码,
                    '' 商品状态,'' 匹配模式 
                    FROM DUAL";
            DataTable dt = DbHelperOra.Query(Sql).Tables[0];
            ExcelHelper.ExportByWeb(dt, "商品匹配信息", string.Format("商品匹配样表.xls", DateTime.Now.ToString("yyyyMMdd")));
            btnExp.Enabled = true;
        }
    }
}
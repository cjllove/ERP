using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using FineUIPro;
using System.Configuration;
using System.Text;

namespace ERPProject
{
    //ymh 2015-02-07
    //解决Grid加载速度,行选中等问题
    public class Grid
    {
        public static bool saveDoc(MyTable MyTb, string tableName, string billNo)
        {
            List<CommandInfo> cmdList = new List<CommandInfo>();
            try
            {
                MyTb.ColRow.Add("SUBSUM", DbHelperOra.GetSingle(string.Format("SELECT SUM(HSJE) FROM {0} WHERE SEQNO = '{1}'", tableName.Substring(0, tableName.Length - 3) + "COM", billNo)));
                MyTb.ColRow.Add("SUBNUM", DbHelperOra.GetSingle(string.Format("SELECT COUNT(1) FROM {0} WHERE SEQNO = '{1}'", tableName.Substring(0, tableName.Length - 3) + "COM", billNo)));
            }
            catch
            {
            }

            cmdList.Add(new CommandInfo(string.Format("DELETE {0} WHERE SEQNO='{1}'", tableName, billNo), null));//删除单据台头
            cmdList.AddRange(MyTb.InsertCommand());
            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //商品信息转换为编码信息
        public static string strGdseq(string tableName, string billNo, string Goods)
        {
            DataTable dt = DbHelperOra.Query(string.Format(@"SELECT A.GDSEQ FROM DOC_GOODS A,{3} B 
            WHERE A.GDSEQ = B.GDSEQ AND B.SEQNO = '{4}' AND (A.GDSEQ LIKE '%{0}%' OR A.GDNAME LIKE '%{0}%' OR A.ZJM LIKE '%{1}%' OR A.ZJM LIKE '%{2}%')", Goods, Goods.ToUpper(), Goods.ToLower(), tableName, billNo)).Tables[0];
            string where = "";
            foreach (DataRow dr in dt.Rows)
            {
                where += "'" + dr["GDSEQ"] + "',";
            }
            return "GDSEQ IN (" + where.TrimEnd(',') + ")";
        }
        /// <summary>
        /// 处理分页拼接数据
        /// </summary>
        /// <param name="Billno">单据编号</param>
        /// <param name="strWhere">筛选条件</param>
        /// <param name="TableName">处理数据单据体</param>
        /// <param name="pageIndex">分页</param>
        /// <param name="pageSize">当前分页数</param>
        /// <param name="newDict">新增数据</param>
        public static bool PageInsert(string Billno, string strWhere, string TableName, int pageIndex, int pageSize, List<Dictionary<string, object>> newDict)
        {
            string connectionString = "";
            if (strWhere.Length < 1)
            {
                strWhere = " 1=1";
            }
            else
            {
                strWhere = strGdseq(TableName, Billno, strWhere);
            }

            DataTable Res = new DataTable();
            if (pageIndex > 1)
            {
                string strSql = string.Format("SELECT ERP.*,ROWNUM ERPNO FROM {0} ERP WHERE ERP.SEQNO = '{1}' AND ROWNUM <= ({2} - 1)*{3} AND {4}", TableName, Billno, pageIndex, pageSize, strWhere);
                Res = DbHelperOra.Query(strSql).Tables[0];
            }
            else
            {
                Res = DbHelperOra.Query(string.Format("SELECT ERP.*,ROWNUM ERPNO FROM {0} ERP WHERE 1=2", TableName)).Tables[0];
            }

            for (int i = 0; i < newDict.Count; i++)
            {
                DataRow newrow = Res.NewRow();
                newrow["ERPNO"] = Res.Rows.Count + 1;
                newrow["SEQNO"] = Billno;
                foreach (DataColumn dtcol in Res.Columns)
                {
                    try
                    {
                        if (newDict[0].ContainsKey(dtcol.ToString()))
                        {
                            if (newDict[i][dtcol.ToString()] == null || string.IsNullOrWhiteSpace(newDict[i][dtcol.ToString()].ToString()))
                            {
                                newrow[dtcol] = DBNull.Value;
                            }
                            else
                            {
                                newrow[dtcol] = newDict[i][dtcol.ToString()];
                            }
                        }

                    }
                    catch
                    { }
                }
                Res.Rows.Add(newrow);
            }
            //第二部分数据
            string strSql2 = string.Format("SELECT * FROM (SELECT ERP.*,ROWNO ERPNO FROM {0} ERP WHERE SEQNO = '{1}' AND {4}) WHERE ERPNO > ({2} + 1)*{3}", TableName, Billno, pageIndex, pageSize, strWhere);
            DataTable dt2 = DbHelperOra.Query(strSql2).Tables[0];
            foreach (DataRow dr in dt2.Rows)
            {
                dr["ERPNO"] = Res.Rows.Count + 1;
                Res.Rows.Add(dr.ItemArray);
            }
            //第三部分数据
            string strSql3 = string.Format("SELECT ERP.*,ROWNO ERPNO FROM {0} ERP WHERE SEQNO = '{1}' AND NOT EXISTS (SELECT 1 FROM {0} WHERE SEQNO = ERP.SEQNO AND ROWNO = ERP.ROWNO AND {4})", TableName, Billno, pageIndex, pageSize, strWhere);
            DataTable dt3 = DbHelperOra.Query(strSql3).Tables[0];
            foreach (DataRow dr in dt3.Rows)
            {
                dr["ERPNO"] = Res.Rows.Count + 1;
                Res.Rows.Add(dr.ItemArray);
            }
            try
            {
                Res.Columns.Remove("ROWNO");
                Res.Columns["ERPNO"].ColumnName = "ROWNO";
            }
            catch
            {
                Res.Columns.Remove("ERPNO");
            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo(string.Format("DELETE {0} WHERE SEQNO='{1}'", TableName, Billno), null));
            //cmdList.Add(new CommandInfo(string.Format("DELETE {0} WHERE SEQNO='{1}'", TableName.Substring(0, TableName.Length - 3) + "DOC", Billno), null));
            DbHelperOra.ExecuteSqlTran(cmdList);

            if (ConfigurationManager.ConnectionStrings["OracleConnString"] != null)
            {
                connectionString = ConfigurationManager.ConnectionStrings["OracleConnString"].ConnectionString;
            }
            else
            {
                throw new Exception(string.Format("从配置文件webconfig中找不到 {0} 的链接字符串配置项!", "OracleConnString"));
            }
            if (Res.Rows.Count < 1)
                return false;
            #region Oracle.ManagedDataAccess.dll没有bulk方法,替换掉
            //using (OracleConnection connection = new OracleConnection(connectionString))
            //{
            //    using (OracleBulkCopy orabulkcopy = new OracleBulkCopy(connectionString, OracleBulkCopyOptions.UseInternalTransaction))
            //    {
            //        try
            //        {
            //            orabulkcopy.DestinationTableName = TableName;
            //            for (int i = 0; i < Res.Columns.Count; i++)
            //            {
            //                orabulkcopy.ColumnMappings.Add(Res.Columns[i].ColumnName, Res.Columns[i].ColumnName);
            //            }
            //            orabulkcopy.WriteToServer(Res);
            //        }
            //        catch (System.Exception ex)
            //        {
            //            return false;
            //        }
            //    }
            //}

            DataTable tableSchema = ApiClientUtil.GetTableSchema(TableName);
            string columnNames = "";
            string columnValues = "";
            string sql = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(" BEGIN ");
            foreach (DataRow dr in Res.Rows)
            {
                columnNames = "";
                columnValues = "";
                foreach (DataColumn dc in tableSchema.Columns)
                {
                    string colType = dc.DataType.ToString();
                    string colName = dc.ColumnName;
                    columnNames += colName + ",";
                    if (dr[colName] == null)
                    {
                        columnValues += "null,";
                    }
                    else if ("System.String".Equals(colType))
                    {
                        columnValues += "'" + dr[colName].ToString() + "',";
                    }
                    else if ("System.DateTime".Equals(colType))
                    {
                        columnValues += "TO_DATE('" + dr[colName].ToString() + "','mm/dd/yyyy hh24:mi:ss'),";
                    }
                    else if ("System.Decimal".Equals(colType))
                    {
                        if (!string.IsNullOrWhiteSpace(dr[colName].ToString()))
                        {
                            columnValues += dr[colName].ToString() + ",";

                        }else
                        {
                            columnValues += 0 + ",";
                        }
                    }
                    else if ("System.Int32".Equals(colType))
                    {
                        if (!string.IsNullOrWhiteSpace(dr[colName].ToString()))
                        {
                            columnValues += dr[colName].ToString() + ",";

                        }
                        else
                        {
                            columnValues += 0 + ",";
                        }
                    }
                    else
                    {
                        columnValues += "'" + dr[colName].ToString() + "',";
                    }
                }
                sql = "INSERT INTO " + TableName + " (";
                sql += columnNames.TrimEnd(',');
                sql += ") VALUES (";
                sql += columnValues.TrimEnd(',') + ");";

                builder.Append(sql);
            }
            builder.Append(" END; ");

            List<CommandInfo> cmdList2 = new List<CommandInfo>();
            cmdList2.Add(new CommandInfo(builder.ToString(), null));
            try
            {
                bool oraResult = DbHelperOra.ExecuteSqlTran(cmdList2);
                if (!oraResult)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            #endregion
            return true;
        }
        /// <summary>
        /// 通过TABLE定义JOBJECT
        /// </summary>
        public static List<JObject> GetJObject(DataTable dtGoods, FineUIPro.Grid GoodsAdd)
        {
            List<JObject> Lobj = new List<JObject>();
            foreach (DataRow rows in dtGoods.Rows)
            {
                JObject Obj = new JObject();
                foreach (GridColumn gc in GoodsAdd.AllColumns)
                {

                    string Columns = "";
                    if (gc is FineUIPro.RenderField)
                    {
                        Columns = ((FineUIPro.RenderField)(gc)).DataField.ToUpper();
                    }
                    if (gc is FineUIPro.BoundField)
                    {

                        Columns = ((FineUIPro.BoundField)(gc)).DataField.ToUpper();
                    }
                    if (Columns.Length > 0)
                    {
                        try
                        {
                            Obj.Add(Columns, rows[Columns].ToString());
                        }
                        catch
                        {
                            Obj.Add(Columns, "");
                        }
                    }
                }
                Lobj.Add(Obj);
            }

            return Lobj;
        }

        /// <summary>
        /// 通过字符串，解析得到JOBJECT
        /// </summary>
        public static JObject GetJObject(string goods, FineUIPro.Grid GoodsAdd)
        {
            JObject Obj = new JObject();
            List<JObject> Lobj = new List<JObject>();
            if (GoodsAdd != null)
            {
                DataTable table = new DataTable();
                DataRow dr;
                string[] goodsRows = goods.Split('♂');
                foreach (string rowValue in goodsRows)
                {
                    dr = table.NewRow();
                    string[] row = rowValue.Split('♀');
                    if (row.Length != table.Columns.Count) continue;
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        dr[i] = row[i] == "★" ? "" : row[i];
                    }
                    table.Rows.Add(dr);
                }
                foreach (DataRow rows in table.Rows)
                {

                    foreach (GridColumn gc in GoodsAdd.AllColumns)
                    {
                        try
                        {
                            Obj.Add(gc.ProductName, rows[gc.ProductName].ToString());
                        }
                        catch
                        { }

                    }

                }
            }
            return Obj;
        }
        /// <summary>
        /// 批量加载数据
        /// </summary>
        public static void GridRowAdd(FineUIPro.Grid grid, DataTable table)
        {
            JArray defaultObj = new JArray();
            defaultObj = JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(table));
            PageContext.RegisterStartupScript(getAddNewRecordReference(grid, defaultObj, true));

        }

        public static string getAddNewRecordReference(FineUIPro.Grid grid, JArray ja, Boolean last)
        {
            string result = "F('" + grid.ClientID + "').f_addNewRecord(" + JsonConvert.SerializeObject(ja) + "," + last.ToString().ToLower() + ");";
            return result;
        }   
        /// <summary>
        /// 新增数据是否为最后一行
        /// </summary>
        public static Boolean AppendToEnd()
        {
            return true;
        }
        #region 增删改
        /// <summary>
        /// 获取DataTable中的某行
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="rowID">列ID</param>
        /// <param name="dt">临时表（包含数据)表结构要和数据库表一致</param>
        /// <returns></returns>
        public static DataRow FindRowByID(int rowID, DataTable DataSource)
        {
            foreach (DataRow row in DataSource.Rows)
            {
                if (Convert.ToInt32(row["ROWNO"]) == rowID)
                {
                    return row;
                }
            }
            return null;
        }
        /// <summary>
        /// 根据Rowno删行
        /// </summary>
        public static DataTable DeleteRowByID(int rowID, DataTable DataSource)
        {
            DataRow found = FindRowByID(rowID, DataSource);
            if (found != null)
            {
                DataSource.Rows.Remove(found);
            }
            return DataSource;
        }
        /// <summary>
        /// 更新整个table
        /// </summary>
        public static DataTable UpdateTable(DataTable DataSource, Dictionary<int, Dictionary<string, object>> modifiedDict, string[] Columns, int RowID, int rowIndex)
        {
            DataTable Rtn_table = DataSource;
            for (int i = 0; i < DataSource.Rows.Count; i++)
            {
                if (Convert.ToInt32(DataSource.Rows[i]["ROWNO"]) == RowID)
                {
                    foreach (string Col in Columns)
                    {
                        if (Col == "SEQNO") continue;
                        if (modifiedDict[rowIndex].ContainsKey(Col))
                            Rtn_table.Rows[i][Col] = modifiedDict[rowIndex][Col];
                    }
                    break;
                }
            }
            return Rtn_table;
        }
        /// <summary>
        /// 更新整行
        /// </summary>
        public static DataRow CreateNewData(DataRow RowSource, Dictionary<string, object> rowDict, string[] Columns, string Billno)
        {
            foreach (string RowName in Columns)
            {
                if (RowName == "SEQNO")
                { RowSource["SEQNO"] = Billno; continue; }
                RowSource[RowName] = rowDict[RowName].ToString();
            }
            return RowSource;
        }
        /// <summary>
        /// 批量数据插入
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="TableName">Oracle里的表名</param>
        /// <param name="dt">临时表（包含数据)表结构要和数据库表一致</param>
        /// <returns></returns>
        public static string ExecuteSqlTranWithSqlBulkCopy(string connectionString, string TableName, DataTable dt, string[] Columns)
        {
            #region Oracle.ManagedDataAccess.dll没有bulk方法,替换掉
            //增加数据验证
            //using (OracleConnection connection = new OracleConnection(connectionString))
            //{
            //    using (OracleBulkCopy orabulkcopy = new OracleBulkCopy(connectionString, OracleBulkCopyOptions.UseInternalTransaction))
            //    {
            //        try
            //        {
            //            orabulkcopy.DestinationTableName = TableName;
            //            for (int i = 0; i < dt.Columns.Count; i++)
            //            {
            //                orabulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
            //            }
            //            orabulkcopy.WriteToServer(dt);
            //            return "";
            //        }
            //        catch (System.Exception ex)
            //        {
            //            return "FALSE";
            //        }
            //    }
            //}
            #endregion

            DataTable tableSchema = ApiClientUtil.GetTableSchema(TableName);
            string columnNames = "";
            string columnValues = "";
            string sql = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(" BEGIN ");
            foreach (DataRow dr in dt.Rows)
            {
                columnNames = "";
                columnValues = "";
                foreach (DataColumn dc in tableSchema.Columns)
                {
                    string colType = dc.DataType.ToString();
                    string colName = dc.ColumnName;
                    columnNames += colName + ",";
                    if (dr[colName] == null)
                    {
                        columnValues += "null,";
                    }
                    else if ("System.String".Equals(colType))
                    {
                        columnValues += "'" + dr[colName].ToString() + "',";
                    }
                    else if ("System.DateTime".Equals(colType))
                    {
                        columnValues += "TO_DATE('" + dr[colName].ToString() + "','mm/dd/yyyy hh24:mi:ss'),";
                    }
                    else if ("System.Decimal".Equals(colType))
                    {
                        columnValues += dr[colName].ToString() + ",";
                    }
                    else if ("System.Int32".Equals(colType))
                    {
                        columnValues += dr[colName].ToString() + ",";
                    }
                    else
                    {
                        columnValues += "'" + dr[colName].ToString() + "',";
                    }
                }
                sql = "INSERT INTO " + TableName + " (";
                sql += columnNames.TrimEnd(',');
                sql += ") VALUES (";
                sql += columnValues.TrimEnd(',') + ");";

                builder.Append(sql);
            }
            builder.Append(" END; ");

            List<CommandInfo> cmdList = new List<CommandInfo>();
            cmdList.Add(new CommandInfo(builder.ToString(), null));
            try
            {
                bool oraResult = DbHelperOra.ExecuteSqlTran(cmdList);
                if (oraResult)
                {
                    return "";
                }
                else
                {
                    return "FALSE";
                }
            }
            catch
            {
                return "FALSE";
            }
        }
        #endregion
        /// <summary>
        /// 由临时表写入正式表中
        /// </summary>
        public static int TempToTable(string Billno, string TABLENAME)
        {
            OracleParameter[] parameters = new OracleParameter[]
                {
                     new OracleParameter("V_BILLNO",OracleDbType.Varchar2),
                     new OracleParameter("V_TABLENAME",OracleDbType.Varchar2),
                };
            parameters[0].Value = Billno;
            parameters[1].Value = TABLENAME;
            DbHelperOra.RunProcedure("P_INS_TABLE", parameters);
            return 1;
        }
        /// <summary>
        /// 验证编码与名称是否匹配
        /// </summary>
        public static Boolean GdseqToGdname(string Gdseq, string Gdname)
        {
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DOC_GOODS WHERE GDSEQ ='{0}' AND (GDNAME ='{1}' OR HISNAME = '{1}')", Gdseq, Gdname)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 验证编码
        /// </summary>
        public static Boolean CheckGdseq(string Gdseq)
        {
            if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DOC_GOODS WHERE GDSEQ ='{0}' AND FLAG = 'Y'", Gdseq)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 检验table是否符合规范
        /// </summary>
        public static string CheckTable(DataTable Goods, string[] Columns)
        {
            for (int i = 0; i < Goods.Rows.Count; i++)
            {
                foreach (string Col in Columns)
                {
                    try
                    {
                        if (PubFunc.StrIsEmpty(Goods.Rows[i][Col].ToString()))
                            return "商品[" + Goods.Rows[i]["GDSEQ"].ToString() + "]信息维护不完整,请检查!";
                        if (Col == "GDSEQ")
                        {
                            if (!CheckGdseq(Goods.Rows[i]["GDSEQ"].ToString()))
                                return "商品[" + Goods.Rows[i]["GDSEQ"].ToString() + "]信息维护不正确,请检查!";
                        }

                    }
                    catch
                    {
                        return "商品[" + Goods.Rows[i]["GDSEQ"].ToString() + "]信息维护不完整,请检查!";
                    }
                }
            }
            return "";
        }
        public static string ColName(string Columns)
        {
            switch (Columns)
            {
                case "GDSEQ": return "商品编码";
                case "BZHL": return "包装含量";
                case "BZSL": return "数量";
            }
            return Columns;
        }

        public string ClientID { get; set; }
    }
}
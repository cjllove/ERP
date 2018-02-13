using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using XTBase.Utilities;
using System.Collections;
using System.Data;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;
using System.Web.Services;
using System.Net;
using System.Web.Services.Description;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Xml;
using System.Configuration;
using System.Web.SessionState;
using XTBase;
using System.Text;

/*
*  编 写 人：鞠怡
*  编写时间：2014-10-07
*  功能说明：封装接口调用操作
*/
namespace ERPProject
{

    public class ApiUtil
    {

        //--------------字段转换逻辑，暂不使用，在数据库里转换-------------
        public static Hashtable GoodsColNameTable = new Hashtable();
        public static Hashtable UnitsColNameTable = new Hashtable();
        public static Hashtable CategoryColNameTable = new Hashtable();


        //public ApiUtil()
        //{

        //}

        static ApiUtil()
        {

            ///商品列名字典  ERP,EAS
            GoodsColNameTable.Add("GDSEQ", "SYO_SEINO");//商品编码 +GDID
            GoodsColNameTable.Add("GDNAME", "SYO_NAME"); //商品名称
            GoodsColNameTable.Add("GDSPEC", "SYO_KKK"); //规格型号
            //GoodsColNameTable.Add("??", "BZHL");//缺省单位包装含量
            GoodsColNameTable.Add("UNIT", "SYO_NYTNICD");//缺省单位
            //GoodsColNameTable.Add("", "SUPPLIER");//供应商 威高？
            //GoodsColNameTable.Add("FACTORY", "PRODUCER");//制造商 威高？
            GoodsColNameTable.Add("JXTAX", "SALETAX");//进项税率 //TODO需要转换
            //GoodsColNameTable.Add("CPDL", "CATID0");//产品大类
            GoodsColNameTable.Add("CATID", "SYO_SKBCD");//基本分类编码
            GoodsColNameTable.Add("UNIT_ZHONGBZ", "ZBZDW");//中包装计量单位编码
            GoodsColNameTable.Add("NUM_ZHONGBZ", "ZBZXS");//中包装系数
            GoodsColNameTable.Add("NUM_DABZ", "DBZXS");//大包装系数
            GoodsColNameTable.Add("PIZNO", "SYO_SYONIN");//注册证号

            ///单位列名字典
            UnitsColNameTable.Add("CODE", "FNUMBER"); //单位编码
            UnitsColNameTable.Add("NAME", "FNAME"); //单位名称
            UnitsColNameTable.Add("FLAG", "FISDISABLED"); //TODO 需要转换 状态

            ///分类列名字典
            CategoryColNameTable.Add("CODE", "FNUMBER");
            CategoryColNameTable.Add("NAME", "FNAME");
            CategoryColNameTable.Add("CLASS", "FLEVEL");
            CategoryColNameTable.Add("SJCODE", "FPARENTNUM");

        }

        #region 判断时间字符串是否合法
        public static Boolean isTimeValid(String time)
        {
            DateTime dt;
            try
            {
                dt = Convert.ToDateTime(time);
            }
            catch (Exception e)
            {
                //Console.Write(e.StackTrace);
                return false;
            }
            return true;
        }
        #endregion

        #region 判断字符串是否为空
        public static Boolean isNull(String item)
        {
            if (item == null)
            {
                return true;
            }
            else if (item.Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region EAS-ERP表名转换
        /// <summary>
        /// EAS-ERP表名转换
        /// </summary>
        /// <param name="OldColName"></param>
        /// <returns></returns>
        private static String getNewColName(String OldColName, Hashtable dictionary)
        {

            return dictionary.ContainsKey(OldColName) ? Convert.ToString(dictionary[OldColName]) : "";
        }


        private static String getOldColName(String newColName, Hashtable dictionary)
        {
            return dictionary.ContainsKey(newColName) ? Convert.ToString(dictionary[newColName]) : "";
        }

        #endregion

        #region 得到表结构
        public static DataTable GetTableSchema(String tableName)
        {
            return DbHelperOra.Query("select * from " + tableName + " where 1=2").Tables[0];
        }
        #endregion

        #region 批量插入数据
        public static int BulkInsert(String tableName, DataTable dResult)
        {
            int oraResult = 0;
            //OracleConnection conn = new OracleConnection(DbHelperOra.connectionString);
            //OracleBulkCopy bulkCopy = new OracleBulkCopy(DbHelperOra.connectionString, OracleBulkCopyOptions.UseInternalTransaction);   //用其它源的数据有效批量加载Oracle表中
            ////conn.BeginTransaction();
            ////OracleBulkCopy bulkCopy = new OracleBulkCopy(connOrcleString, OracleBulkCopyOptions.Default);
            //bulkCopy.BatchSize = 100000;
            //bulkCopy.BulkCopyTimeout = 260;
            //bulkCopy.DestinationTableName = tableName;    //服务器上目标表的名称
            //bulkCopy.BatchSize = dResult.Rows.Count;   //每一批次中的行数
            //try
            //{
            //    conn.Open();
            //    if (dResult != null && dResult.Rows.Count != 0)
            //    {
            //        bulkCopy.WriteToServer(dResult);   //将提供的数据源中的所有行复制到目标表中
            //        oraResult = 1;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;

            //}
            //finally
            //{
            //    conn.Close();
            //    if (bulkCopy != null)
            //        bulkCopy.Close();
            //}
            DataTable tableSchema = ApiClientUtil.GetTableSchema(tableName);
            string columnNames = "";
            string columnValues = "";
            string sql = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(" BEGIN ");
            foreach (DataRow dr in dResult.Rows)
            {
                columnNames = "";
                columnValues = "";
                foreach (DataColumn dc in tableSchema.Columns)
                {
                    string colType = dc.DataType.ToString();
                    string colName = dc.ColumnName;
                    columnNames += colName + ",";
                    if (dr[colName] == null || "".Equals(dr[colName].ToString()))
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
                sql = "INSERT INTO " + tableName + " (";
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
                bool oraResultB = DbHelperOra.ExecuteSqlTran(cmdList);
                if (oraResultB)
                {
                    oraResult = 1;
                }
            }
            catch
            {
                oraResult = 0;
            }
            return oraResult;
        }
        #endregion

        #region 执行存储过程
        public static int runProcedure(String procedureName)
        {
            int oraResult = 0;
            OracleConnection conn = new OracleConnection(DbHelperOra.connectionString);
            OracleCommand command = new OracleCommand(procedureName);
            command.Connection = conn;
            command.CommandType = CommandType.StoredProcedure;
            try
            {
                conn.Open();
                command.ExecuteNonQuery();
                oraResult = 1;
            }
            catch (Exception ex)
            {
                oraResult = -1;
                throw ex;
            }
            finally
            {
                conn.Close();

            }
            return oraResult;
        }

        public static int runProcedure(String procedureName, params OracleParameter[] parameters)
        {
            int oraResult = 0;
            OracleConnection conn = new OracleConnection(DbHelperOra.connectionString);
            OracleCommand command = new OracleCommand(procedureName);
            command.Connection = conn;
            command.CommandType = CommandType.StoredProcedure;
            foreach (OracleParameter param in parameters)
            {
                command.Parameters.Add(param);
            }
            try
            {
                conn.Open();
                command.ExecuteNonQuery();
                oraResult = 1;
            }
            catch (Exception ex)
            {
                oraResult = -1;
                throw ex;
            }
            finally
            {
                conn.Close();

            }
            return oraResult;
        }
        #endregion

        #region Json 字符串 转换为 DataTable数据集合
        /// <summary> 
        /// Json 字符串 转换为 DataTable数据集合 
        /// </summary> 
        /// <param name="json"></param> 
        /// <returns></returns> 
        public static DataTable ToDataTable(String json, Hashtable colDictionary, String tableName)
        {
            DataTable dataTable = new DataTable(); //实例化 
            DataTable result;

            dataTable = GetTableSchema(tableName);

            try
            {
                JArray ja = JArray.Parse(json);
                if (ja.Count > 0)
                {
                    foreach (JObject jo in ja)
                    {
                        if (jo.Count == 0)
                        {
                            result = dataTable;
                            return result;
                        }

                        DataRow dataRow = dataTable.NewRow();
                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            String newColName = dataTable.Columns[i].ColumnName;
                            Type type = dataTable.Columns[i].DataType;
                            if (jo.Property(getOldColName(newColName, colDictionary)) == null || jo.Property(getOldColName(newColName, colDictionary)).ToString() == "" || isNull(jo.Value<String>(getOldColName(newColName, colDictionary)))) //没有此键值
                            {
                                try
                                {
                                    dataRow[newColName] = Convert.ChangeType("", type);
                                }
                                catch
                                {
                                    if (type != typeof(DateTime))
                                    {
                                        dataRow[newColName] = Convert.ChangeType(0, type);
                                    }
                                    else
                                    {
                                        dataRow[newColName] = DateTime.Now;
                                    }

                                }
                            }
                            else
                            {

                                dataRow[newColName] = Convert.ChangeType(jo.Value<String>(getOldColName(newColName, colDictionary)), type);

                            }
                        }
                        if (tableName.Equals("DOC_GOODS") || tableName.Equals("TEST_GOODS"))
                        {
                            dataRow["GDID"] = "TEST." + jo.Value<String>("SYO_SEINO");
                        }
                        else if (tableName.Equals("DOC_GOODSUNIT") || tableName.Equals("TEST_UNITS"))
                        {
                            dataRow["FLAG"] = jo.Value<String>("FISDISABLED").Equals("0") ? "Y" : "N";
                            dataRow["MEMO"] = "FROM_EAS2";
                            dataRow["ISDELETE"] = "N";
                        }
                        else if (tableName.Equals("SYS_CATEGORY") || tableName.Equals("TEST_CATEGORY"))
                        {
                            dataRow["FLAG"] = jo.Value<String>("FDELETEDSTATUS").Equals("1") ? "Y" : "N";
                            dataRow["ISLAST"] = jo.Value<String>("FLEVEL").Equals("3") ? "Y" : "N";

                        }

                        dataTable.Rows.Add(dataRow);

                    }
                }
            }
            catch (Exception e)
            {
            }

            result = dataTable;
            return result;
        }
        #endregion

        #region 拿到appsettings 里对应key 的的值
        public static string GetConfigCont(String key)
        {
            try
            {
                return ConfigurationManager.AppSettings.Get(key);

            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region 拿到INF_TABLE_DEF/缓存infTableDef 里对应key的值

        public static String INF_TABLE_CACHE_NAME = "INF_TABLE_DEF";

        public static String GetTableConfig(String key)
        {
            //创建Hashtable infTableDef空实例
            Hashtable infTableDef = new Hashtable();
            String result = "";
            //判断缓存是否存在
            if (!ApiUtil.isCacheExist(INF_TABLE_CACHE_NAME))
            {
                //从数据库中取值，更新缓存
                RefreshTableConfig();

            }
            infTableDef = (Hashtable)CacheHelper.GetCache(INF_TABLE_CACHE_NAME);
            result = infTableDef[key].ToString();

            return result;
        }

        public static void RefreshTableConfig()
        {
            Hashtable infTableDef = new Hashtable();
            String sql = "select * from INF_TABLE_DEF where flag = 'Y'";
            DataTable dt = DbHelperOra.Query(sql).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                infTableDef.Add(dr["INFKEY"].ToString(), dr["INFVALUE"].ToString());
            }
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            InfTableCacheCallBack = new System.Web.Caching.CacheItemRemovedCallback(ApiUtil.RemovedCallback);
            objCache.Add(
                INF_TABLE_CACHE_NAME,
                infTableDef,
                null,
                DateTime.Now.Add(TimeSpan.FromMinutes(30)),
                TimeSpan.Zero,
                System.Web.Caching.CacheItemPriority.Default,
                InfTableCacheCallBack);
            InfTableCacheRemoved = false;
            //两小时失效
            //CacheHelper.SetCache(INF_TABLE_CACHE_NAME, infTableDef, TimeSpan.FromHours(2));
        }

        private static System.Web.Caching.CacheItemRemovedCallback InfTableCacheCallBack = null;
        public static Boolean InfTableCacheRemoved = false;
        public static System.Web.Caching.CacheItemRemovedReason InfTableCacheRemovedReason;
        private static void RemovedCallback(String k, Object v, System.Web.Caching.CacheItemRemovedReason r)
        {
            InfTableCacheRemoved = true;
            InfTableCacheRemovedReason = r;
            ApiUtil.RefreshTableConfig();
        }
        #endregion

        #region 生成随机uuid
        public static String renderUUID()
        {
            return System.Guid.NewGuid().ToString();
        }
        #endregion

        #region 判断cache 是否存在
        public static Boolean isCacheExist(String key)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            if (objCache[key] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region 得到缓存中的uuid
        public static String getUserUUID(String userName)
        {
            Hashtable ht = (Hashtable)XTBase.Utilities.CacheHelper.GetCache("userInfo");
            if (ht.ContainsKey(userName))
            {
                return ht[userName].ToString();
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 判断数据库中登陆信息是否正确
        public static Boolean isUserInfoCorrect(String username, String password, String token)
        {
            String sql = "select CODE,PASSWD,TOKEN from doc_customer where code = '" + username + "' and flag='Y'";
            try
            {
                DataTable result = DbHelperOra.Query(sql).Tables[0];
                if (result.Rows.Count > 0 && result.Rows[0]["PASSWD"].Equals(password) && result.Rows[0]["TOKEN"].Equals(token))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 登陆成功生成返回字符串
        public static String renderLoginSuccess(String uuid)
        {
            JObject jo = new JObject();
            jo.Add("uuid", uuid);
            jo.Add("result", "success");
            return jo.ToString();
        }
        #endregion

        #region 登陆失败返回字符串
        public static String renderLoginFail()
        {
            JObject jo = new JObject();
            jo.Add("result", "fail");
            return jo.ToString();
        }
        #endregion

        #region 登陆成功更新doc_customer
        public static void updateCustomer(String username, String uuid, String ip)
        {
            String sql = "update doc_customer set logid='{0}',logtime=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),logip='{2}' where code='{3}'";
            sql = String.Format(sql, uuid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ip, username);
            try
            {
                DbHelperOra.ExecuteSql(sql);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 返回登出字符串
        public static String renderLogout(Boolean result)
        {
            JObject jo = new JObject();
            jo.Add("result", result ? "success" : "fail");
            return jo.ToString();
        }
        #endregion

        #region 调用接口时判断是否登陆
        public static Boolean isLogin(String username, String uuid)
        {
            Hashtable ht = (Hashtable)XTBase.Utilities.CacheHelper.GetCache("userInfo");
            if (ht.ContainsKey(username))
            {
                return ht[username].ToString().Equals(uuid);
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 返回未登陆字符串
        public static String renderNotLogin()
        {
            JObject jo = new JObject();
            jo.Add("result", "fail");
            jo.Add("reason", "not login");
            return jo.ToString();
        }
        #endregion

        #region 包装返回结果
        public static String renderResultJson(JToken result, String reason, JToken data)
        {
            JObject jo = new JObject();
            jo.Add("result", result);
            jo.Add("reason", reason);
            jo.Add("data", data);
            jo.Add("time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return jo.ToString();
        }
        #endregion


        #region 两整数除以，保留精度(默认4位)
        public static decimal divide(Decimal divisor, Decimal dividend, int precision = 6)
        {
            String strDivisor = divisor.ToString("F" + precision);
            Decimal dDivisor = Convert.ToDecimal(strDivisor);
            String strDivdend = dividend.ToString("F" + precision);
            Decimal dDividend = Convert.ToDecimal(strDivdend);
            decimal result = dDivisor / dDividend;
            String strResult = result.ToString("F" + precision);
            result = Convert.ToDecimal(strResult);
            return Math.Round(result, precision);

        }

        public static float fdivide(long divisor, long dividend, int precision = 4)
        {

            Single fDividend = Convert.ToSingle(dividend);
            float result = divisor / fDividend;
            String strResult = result.ToString("F" + precision);
            result = Convert.ToSingle(strResult);
            return result;
        }
        #endregion
    }

    public class ApiService
    {

        private static String USERNAME;//= "37110045";//"37110169";
        private static String PASSWORD;//= "123";//"123";
        private static String SLNNAME;//= "eas";
        private static String DCNAME;//= "a24";
        private static String LANGUAGE;//= "l2";
        private static int DBTYPE;//= 2;
        private static String loginURL;
        private static String webServiceURL;
        private static String webServiceURLNEW;
        private static Boolean useDynamicURL;
        private static String loginServiceName = "EASLoginProxyService";
        private static String webServiceName = "WSPlatformERPFacadeSrvProxyService";

        public static String ERRORCODE_NOTLOGIN = "-1";
        public static String ERRORCODE_TIMENOTVALID = "-2";

        private static String USERNAME_WITHCUST;
        private static String PASSWORD_WITHCUST;

        private static String SESSION_ID;
        static ApiService()
        {
            USERNAME = ApiUtil.GetConfigCont("ERP_USERNAME");
            PASSWORD = ApiUtil.GetConfigCont("ERP_PASSWORD");
            SLNNAME = ApiUtil.GetConfigCont("ERP_SLNNAME");
            DCNAME = ApiUtil.GetConfigCont("ERP_DCNAME");
            LANGUAGE = ApiUtil.GetConfigCont("ERP_LANGUAGE");
            DBTYPE = Convert.ToInt16(ApiUtil.GetConfigCont("ERP_DBTYPE"));

            USERNAME_WITHCUST = "";
            PASSWORD_WITHCUST = "";

            String strUseDynamicURL = ApiUtil.GetConfigCont("ERP_USEDYNAMICURL");
            if (!ApiUtil.isNull(strUseDynamicURL))
            {
                useDynamicURL = "1".Equals(strUseDynamicURL);
            }
            else
            {
                useDynamicURL = false;
            }

        }


        #region 取webService URL
        private static Boolean getWebServiceURL()
        {
            Hashtable ht = new Hashtable();
            //String url = "";
            //String login = "";
            //String className = "";
            if (ApiUtil.isCacheExist("webServicePara"))
            {
                webServiceURL = ((Hashtable)XTBase.Utilities.CacheHelper.GetCache("webServicePara"))["webServiceURL"].ToString();
                loginURL = ((Hashtable)XTBase.Utilities.CacheHelper.GetCache("webServicePara"))["loginURL"].ToString();
                webServiceName = ((Hashtable)XTBase.Utilities.CacheHelper.GetCache("webServicePara"))["webServiceName"].ToString();
                webServiceURLNEW = ((Hashtable)XTBase.Utilities.CacheHelper.GetCache("webServicePara"))["webServiceURLNEW"].ToString();
                return true;
            }

            //if (!ApiUtil.isNull(url) && !ApiUtil.isNull(login) && !ApiUtil.isNull(className))
            //{
            //    webServiceURL = url;
            //}
            if (ApiUtil.isNull(webServiceURL) || ApiUtil.isNull(loginURL) || ApiUtil.isNull(webServiceName) || ApiUtil.isNull(webServiceURLNEW))
            {
                DataTable dt = new DataTable();
                String sql = "select code,value,str1,str2,str3 from sys_para where code = 'WEBSERVICEURL'";
                dt = DbHelperOra.Query(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    loginURL = Convert.ToString(dt.Rows[0]["str1"]);
                    webServiceURL = Convert.ToString(dt.Rows[0]["value"]);
                    webServiceName = Convert.ToString(dt.Rows[0]["STR2"]);
                    webServiceURLNEW = Convert.ToString(dt.Rows[0]["str3"]);
                    ht.Add("webServiceURL", webServiceURL);
                    ht.Add("loginURL", loginURL);
                    ht.Add("webServiceName", webServiceName);
                    ht.Add("webServiceURLNEW", webServiceURLNEW);
                    XTBase.Utilities.CacheHelper.RemoveOneCache("webServicePara");
                    XTBase.Utilities.CacheHelper.SetCache("webServicePara", ht, TimeSpan.FromHours(2));
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 通用单据接口
        /// <summary>
        /// 通用单据接口
        /// </summary>
        /// <param name="billType">
        /// String billType 单据类型，目前标准支持的单据类型如下：
        /// 001 采购申请单  002 采购订单  003 销售订单  004	调拨订单   005	库存调拨单
        /// 006	采购入库    007	销售出库  008 其他入库  009	其他出库   010	应收单
        /// 011	应付单
        /// </param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static String commonBillImp(String billType, String content)
        {
            String result = "";
            if (login())
            {
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("billType", billType);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("billDataJSON", content);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("billStatus", 1);
                    pars.Add(parsValue);
                    #region 测试用
                    // webServiceURL = "http://192.168.1.158:56898/ormrpc/services/WSSCMWebServiceFacade";
                    //webServiceURL = "http://localhost:9123/ormrpc/services/WSSCMWebServiceFacade"; 
                    //webServiceURL = "http://172.16.3.38:6888/ormrpc/services/WSSCMWebServiceFacade";
                    #endregion
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURLNEW, "importBill", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion

        #region 登陆
        private static Boolean login()
        {
            SESSION_ID = "";
            Boolean bResult = false;
            try
            {

                if (useDynamicURL)
                {
                    getWebServiceURL();
                    #region 测试用
                    //loginURL = "http://localhost:9123/ormrpc/services/EASLogin";
                    //loginURL = "http://localhost:56898/ormrpc/services/EASLogin";
                    #endregion
                    dynamic result = DynamicServiceBind.InvokeWebService(loginURL, loginServiceName, "login", new object[] { USERNAME, PASSWORD, SLNNAME, DCNAME, LANGUAGE, DBTYPE });
                    bResult = result.sessionId == null ? false : true;

                    if (bResult)
                    {
                        SESSION_ID = result.sessionId;
                    }

                }
                else
                {
                    EasLogin.EASLoginProxyService login = new EasLogin.EASLoginProxyService();
                    EasLogin.WSContext result = login.login(USERNAME, PASSWORD, SLNNAME, DCNAME, LANGUAGE, DBTYPE);
                    bResult = result.sessionId == null ? false : true;
                    if (bResult)
                    {
                        SESSION_ID = result.sessionId;
                    }
                }
            }
            catch (Exception e)
            {
                //Console.Write(e.StackTrace);
                return false;
            }

            return bResult;
        }
        #endregion

        #region 登出
        private static Boolean logout()
        {
            #region do nothing
            //Boolean bResult = false;
            //try
            //{
            //    if (useDynamicURL)
            //    {
            //        getWebServiceURL();
            //        bResult = (Boolean)DynamicServiceBind.InvokeWebService(loginURL, loginServiceName, "logout", new object[] { USERNAME, SLNNAME, DCNAME, LANGUAGE });
            //    }
            //    else
            //    {
            //        EasLogin.EASLoginProxyService login = new EasLogin.EASLoginProxyService();
            //        bResult = login.logout(USERNAME, SLNNAME, DCNAME, LANGUAGE);
            //    }

            //}
            //catch (Exception e)
            //{
            //    //Console.Write(e.StackTrace);
            //    return false;
            //}

            //return bResult;
            #endregion
            return false;
        }
        #endregion



        #region 根据时间戳取物料
        /// <summary>
        /// 根据时间戳取物料 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String getMaterials(String time)
        {
            String result = "";
            if (login())
            {
                if (!ApiUtil.isTimeValid(time))
                {
                    return ERRORCODE_TIMENOTVALID;
                }
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, webServiceName, "exportMaterials", new object[] { time }));
                }
                else
                {
                    EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    result = eas.exportMaterials(time);
                }

                //JObject jo = new JObject();
                //jo = JsonConvert.DeserializeObject<JObject>(result);
                //Console.WriteLine(jo.Value<String>("time"));
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion

        #region 取全部物料信息
        /// <summary>
        /// 取全部物料信息
        /// </summary>
        /// <returns></returns>
        public static String getMaterials()
        {
            String result = "";
            if (login())
            {
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, webServiceName, "exportMaterials", new object[] { "1970-01-01" }));
                }
                else
                {
                    EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    result = eas.exportMaterials("1970-01-01");
                }

            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion

        #region 根据时间戳取单位信息
        /// <summary>
        /// 根据时间戳取单位信息
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String getUnits(String time)
        {
            String result = "";
            if (login())
            {
                if (!ApiUtil.isTimeValid(time))
                {
                    return ERRORCODE_TIMENOTVALID;
                }
                if (useDynamicURL)
                {
                    getWebServiceURL();

                    //result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, webServiceName, "exportUnits", new object[] { time }));
                    #region 测试用
                    //webServiceURL = "http://localhost:9123/ormrpc/services/WSPlatformERPFacade";
                    #endregion
                    //Hashtable pars = new Hashtable();
                    //pars["after"] = time;
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("after", time);
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "exportUnits", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;

                }
                else
                {

                    EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    #region 测试用，已废弃
                    //SessionIdRender wsa = new SessionIdRender();
                    //wsa.sessionId = SESSION_ID;
                    ////wsa.Namespaces.Add("ns1", "http://login.webservice.bos.kingdee.com");
                    //wsa.Namespaces.Add("", "http://login.webservice.bos.kingdee.com");

                    //eas.SessionId = wsa;
                    #endregion
                    result = eas.exportUnits(time);
                }
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion

        #region 取全部单位信息
        /// <summary>
        /// 取全部单位信息
        /// </summary>
        /// <returns></returns>
        public static String getUnits()
        {
            String result = "";
            if (login()) //TODO ///---------------------TEST-----------------------------////CHANGE THIS
            {
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    //result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, webServiceName, "exportUnits", new object[] { "1970-01-01" }));
                    //Hashtable pars = new Hashtable();
                    //pars["after"] = "1970-01-01";

                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("after", "1970-01-01");
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "exportUnits", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    result = eas.exportUnits("1970-01-01");
                }
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion

        #region 根据时间戳获取商品分类
        /// <summary>
        /// 根据时间戳获取商品分类
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static String getMaterialGroups(String time)
        {
            String result = "";
            if (login())
            {
                if (!ApiUtil.isTimeValid(time))
                {
                    return ERRORCODE_TIMENOTVALID;
                }
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    //result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, webServiceName, "exportMaterialGroups", new object[] { time }));
                    //Hashtable pars = new Hashtable();
                    //pars["after"] = time;
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("after", time);
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "exportMaterialGroups", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    result = eas.exportMaterialGroups(time);
                }

                //JObject jo = new JObject();
                //jo = JsonConvert.DeserializeObject<JObject>(result);
                //Console.WriteLine(jo.Value<String>("time"));
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion

        #region 获取全部商品分类
        /// <summary>
        /// 获取全部商品分类
        /// </summary>
        /// <returns></returns>
        public static String getMaterialGroups()
        {
            String result = "";
            if (login())
            {
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    //result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, webServiceName, "exportMaterialGroups", new object[] { "1970-01-01" }));
                    //Hashtable pars = new Hashtable();
                    //pars["after"] = "1970-01-01";
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("after", "1970-01-01");
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "exportMaterialGroups", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    result = eas.exportMaterialGroups("1970-01-01");
                }

            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion

        #region 分页取物料
        public static String getMaterials(int begin, int end, String time, String type)
        {
            String result = "";
            if (login())
            {
                if (!ApiUtil.isTimeValid(time))
                {
                    return ERRORCODE_TIMENOTVALID;
                }
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    //result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, webServiceName, "exportMaterials", new object[] { begin, end, time, type }));
                    //Hashtable pars = new Hashtable();
                    //pars["begin"] = begin;
                    //pars["end"] = end;
                    //pars["after"] = time;
                    //pars["type"] = type;
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("begin", begin);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("end", end);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("after", time);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("type", type);
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "exportMaterials", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    result = eas.exportMaterials(begin, end, time, type);
                }

            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion

        #region 分页分类型取物料分类
        public static String getMaterialGroups(int begin, int end, String time, String type)
        {
            String result = "";
            if (login())
            {
                if (!ApiUtil.isTimeValid(time))
                {
                    return ERRORCODE_TIMENOTVALID;
                }
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    //result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, webServiceName, "exportMaterialGroups", new object[] { begin, end, time, type }));
                    //Hashtable pars = new Hashtable();
                    //pars["begin"] = begin;
                    //pars["end"] = end;
                    //pars["after"] = time;
                    //pars["type"] = type;
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("begin", begin);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("end", end);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("after", time);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("type", type);
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "exportMaterialGroups", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    result = eas.exportMaterialGroups(begin, end, time, type);
                }

            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion

        #region 取count
        /// <summary>
        /// 取数据数量
        /// </summary>
        /// <param name="type">UNITS,GOODS,CATEGORY</param>
        /// <param name="time">时间戳</param>
        /// <param name="materialType">物料类型</param>
        /// <returns></returns>
        public static Int64 getCount(String type, String time, String materialType)
        {
            Int64 result = -1;
            if (login())
            {
                if (!ApiUtil.isTimeValid(time))
                {
                    return -1;
                }
                String sResult = "0";
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    //sResult = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, webServiceName, "count", new object[] { type, time, materialType }));
                    //Hashtable pars = new Hashtable();
                    //pars["type"] = type;
                    //pars["after"] = time;
                    //pars["materialType"] = materialType;
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("type", type);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("after", time);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("materialType", materialType);
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "count", pars, SESSION_ID);
                    sResult = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    sResult = eas.count(type, time, materialType);
                }

                try
                {
                    result = Convert.ToInt64(sResult);
                }
                catch
                {

                }
            }
            logout();
            return result;
        }
        #endregion

        #region 单据相关

        /// <summary>
        /// 订单上传
        /// </summary>
        /// <param name="content">
        //* 输入数据格式
        // * {"head":[
        // * 	{
        // * 		"number":"",   //单据号
        // *      "bizDate":"yyyy-MM-dd", //单据日期
        // *      "reqtype":"WG/ZD" //需求类型
        // *      
        // * 	}],
        // *  "body":[
        // *   {
        // *   	"qty":"", //数量
        // *   	"number":"" //商品编码
        // *   }
        // *  ]
        // * }
        /// </param>
        /// <returns></returns>
        public static String putStockTransferBill(String content)
        {
            String result = "";
            if (login())
            {
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    //result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, webServiceName, "importStockTransferBill", new object[] { content }));
                    //Hashtable pars = new Hashtable();
                    //pars["content"] = content;
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("content", content);
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "importStockTransferBill", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    result = eas.importStockTransferBill(content);
                }

                //JObject jo = new JObject();
                //jo = JsonConvert.DeserializeObject<JObject>(result);
                //Console.WriteLine(jo.Value<String>("time"));
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }

        public static String putSaleIssueBill(String content)
        {
            String result = "";
            if (login())
            {
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    //result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, webServiceName, "importSaleIssueBill", new object[] { content }));
                    //Hashtable pars = new Hashtable();
                    //pars["content"] = content;
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("content", content);
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "importSaleIssueBill", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    result = eas.importSaleIssueBill(content);
                }

                //JObject jo = new JObject();
                //jo = JsonConvert.DeserializeObject<JObject>(result);
                //Console.WriteLine(jo.Value<String>("time"));
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }


        /// <summary>
        /// 入库单下传
        /// </summary>
        /// <param name="number">订单流水号</param>
        /// <param name="mat">商品编号</param>
        /// <returns></returns>
        public static String queryState(String number, String mat)
        {
            String result = "";
            if (login())
            {
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    //result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, webServiceName, "queryState", new object[] { number, mat }));
                    //Hashtable pars = new Hashtable();
                    //pars["number"] = number;
                    //pars["mat"] = mat;
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("number", number);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("mat", mat);
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "queryState", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    result = eas.queryState(number, mat);
                }

                //JObject jo = new JObject();
                //jo = JsonConvert.DeserializeObject<JObject>(result);
                //Console.WriteLine(jo.Value<String>("time"));
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }

        /// <summary>
        /// 入库单下传
        /// </summary>
        /// <param name="number">订单流水号</param>
        /// <returns>
        ///      * result = 
        //* {
        //* 		"number":number,
        //* 		"entries":[{
        //* 			"factory":factory,
        //* 			"ERPstate":ERPstate,
        //* 			"matial":matial.number
        //* 		}],
        //* 		
        //* 		"inStore":[{
        //* 			"number":pur number,
        //* 			"entries":[{
        //* 				"matial":matial.number,
        //* 				"lot":"WG/ZD"+lot,
        //* 				"exp":enddate,
        //* 				"qty":qty
        //* 			}]
        //* 			
        //* 		}]
        //* }
        /// </returns>
        public static String queryState(String number)
        {
            String result = "";
            if (login())
            {
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    //result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, webServiceName, "queryState", new object[] { number, "" }));
                    //Hashtable pars = new Hashtable();
                    //pars["number"] = number;
                    //pars["mat"] = "";
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("number", number);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("mat", "");
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "queryState", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    result = eas.queryState(number, "");
                }

                //JObject jo = new JObject();
                //jo = JsonConvert.DeserializeObject<JObject>(result);
                //Console.WriteLine(jo.Value<String>("time"));
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }


        #endregion

        #region billStatus
        public static String queryBillStatus(String number)
        {
            String result = "";
            if (login())
            {
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    //result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, webServiceName, "queryState", new object[] { number, "" }));
                    //Hashtable pars = new Hashtable();
                    //pars["number"] = number;
                    //pars["mat"] = "";\
                    #region 测试用
                    //webServiceURL = "http://localhost:56898/ormrpc/services/WSPlatformERPFacade";
                    #endregion
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("number", number);
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "getDhdStatus", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    //EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    //result = eas.getDhdStatus(number);
                }
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion

        #region getXCK   EAS取入库单和所有单据方法
        public static String getXCK(String number, bool isAllStatus)
        {
            String result = "";
            if (login())
            {
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    #region 测试用
                    //webServiceURL = "http://localhost:56898/ormrpc/services/WSPlatformERPFacade";
                    #endregion
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("number", number);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("isAllStatus", isAllStatus);
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "getXCK", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    // EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    //result = eas.getDhdStatus(number);
                }
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion

        #region getEASDHDBILL   EAS取调拨单方法
        public static String getEASDHDBILL(String[] paramArray)
        {
            String result = "";
            if (login())
            {
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    #region 测试用
                    //webServiceURL = "http://localhost:56898/ormrpc/services/WSPlatformERPFacade";
                    #endregion
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("beginDate", paramArray[0]);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("endDate", paramArray[1]);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("dno", paramArray[2]);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("wareHouse", DbHelperOra.GetSingle(string.Format("select WAREHOUSE from doc_customer where code='{0}'", paramArray[3])).ToString());
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "getDHD", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    // EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    //result = eas.getDhdStatus(number);
                }
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion

        #region getEASDRKBILL   EAS取调拨入库单方法
        public static String getEASDRKBILL(String[] paramArray)
        {
            String result = "";
            if (login())
            {
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    #region 测试用
                    //webServiceURL = "http://localhost:56898/ormrpc/services/WSPlatformERPFacade";
                    #endregion
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("beginDate", paramArray[0]);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("endDate", paramArray[1]);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("dno", paramArray[2]);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("wareHouse", DbHelperOra.GetSingle(string.Format("select WAREHOUSE from doc_customer where code='{0}'", paramArray[3])).ToString());
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "getDRK", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    // EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    //result = eas.getDhdStatus(number);
                }
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion

        #region getDDPurAllStatus   EAS取采购单和之后单据方法
        public static String getDDPurAllStatus(String number)
        {
            String result = "";
            if (login())
            {
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    #region 测试用
                    //webServiceURL = "http://localhost:56898/ormrpc/services/WSPlatformERPFacade";
                    #endregion
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("number", number);
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "getDDPurAllStatus", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    // EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    //result = eas.getDhdStatus(number);
                }
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion

        #region getInventory  查询EAS商品库存接口
        public static String getInventory(String warehouse, String materials)
        {
            String result = "";
            if (login())
            {
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    #region 测试用
                    //webServiceURL = "http://192.168.1.102:56898/ormrpc/services/WSPlatformERPFacade";
                    //webServiceURL = "http://localhost:8080/ormrpc/services/WSPlatformERPFacade";
                    #endregion
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("warehouse", warehouse);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("materials", materials);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("location", "");
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "getInventory", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                }
                else
                {
                    getWebServiceURL();
                    webServiceURL = ApiUtil.GetConfigCont("ERP_WEBSERVICEURL");
                    List<KeyValuePair<String, object>> pars = new List<KeyValuePair<string, object>>();
                    KeyValuePair<string, object> parsValue = new KeyValuePair<string, object>("warehouse", warehouse);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("materials", materials);
                    pars.Add(parsValue);
                    parsValue = new KeyValuePair<string, object>("location", "");
                    pars.Add(parsValue);
                    XmlDocument doc = WebSvcCaller.QuerySoapWebService(webServiceURL, "getInventory", pars, SESSION_ID);
                    result = doc.SelectSingleNode("root").InnerXml;
                    //EasAPI.WSPlatformERPFacadeSrvProxyService eas = new EasAPI.WSPlatformERPFacadeSrvProxyService();
                    //result = eas.getInventory(warehouse,materials,"");
                }
            }
            else
            {
                result = ERRORCODE_NOTLOGIN;
            }
            logout();
            return result;
        }
        #endregion
    }



    public class Api
    {
        private DateTime workTime = new DateTime();//EAS 服务器回传时间
        private String workType; //类别
        private String orgWorkType;
        private int execCount = 0; //执行条目数
        private TimeSpan exectime { set; get; } //执行总时间
        private TimeSpan tableTime; //转datatable时间
        private TimeSpan queryTime; //调用WebService时间

        public DateTime paraTime { get; set; } //传参时间
        private DateTime startTime = new DateTime();//运行开始时间

        private int defaultStep = 10000; //默认分页数
        private int currentStep = 0; //当前位置
        private Int64 total = 0;

        public Int64 count = 0;
        //private DateTime startWorkTime = new DateTime();
        public Boolean isError;
        private String errorDetail;
        private DataTable ddFlagS;//已上传未确认订单
        private DataTable ddFlagY;//未上传订单

        private DataTable ckTable;
        private DataTable thTable;

        private String ddBillNo;
        private int successCount = 0;

        public System.Web.HttpContext context { set; get; }

        private void init()
        {
            this.isError = false;
            this.startTime = DateTime.Now;
            this.errorDetail = "";
        }


        public Api(String type)
        {
            this.orgWorkType = this.workType = type;
            this.workTime = new DateTime();
            this.paraTime = new DateTime();
            //init();
        }

        public int getExecCount()
        {
            return this.execCount;
        }

        public String getWorkType()
        {
            return this.workType;
        }

        public TimeSpan getExectime()
        {
            return this.exectime;
        }

        public Int64 getTotal()
        {
            return this.total;
        }

        private Boolean export(String result, Hashtable ColNameTable)
        {
            if (result.LongCount() <= 2)
            {
                //TODO 处理错误类型
                this.isError = true;
                this.total = -1;
                this.errorDetail = "未返回数据！";
                return false;
            }


            DataTable dResult = new DataTable();
            DataTable tableSchema = ApiUtil.GetTableSchema(this.workType);
            JObject jo = JsonConvert.DeserializeObject<JObject>(result);
            if (JsonConvert.SerializeObject(jo.Value<JArray>("result")).Length <= 2)
            {
                this.isError = true;
                this.total = -1;
                this.errorDetail = "没有需要下传的数据";
                return false;
            }
            DateTime dtime = DateTime.Now;
            //dResult = ApiUtil.ToDataTable(JsonConvert.SerializeObject(jo.GetValue("result")), ColNameTable,this.workType);
            if (this.orgWorkType.Equals("DOC_GOODSUNIT"))
            {
                dResult = ApiUtil.ToDataTable(JsonConvert.SerializeObject(jo.GetValue("result")), ColNameTable, this.workType);
            }
            else if (this.orgWorkType.Equals("DOC_GOODS"))
            {
                dResult = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(jo.GetValue("result")));
                dResult.Columns.Add("STR9", typeof(String));
                foreach (DataRow dr in dResult.Rows)
                {
                    dr["STR9"] = dr["SYO_SEINO"].ToString() + dr["SALEORG"].ToString();
                }
            }
            else
            {
                dResult = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(jo.GetValue("result")));
            }
            this.tableTime = DateTime.Now - dtime;
            // dResult.Columns["FSTATUS"].SetOrdinal(12);
            //   int oraResult = DbHelperOra.ExecuteSqlTranWithSqlBulkCopy(DbHelperOra.ConnString(), this.workType, dResult);
            //BCP copy

            foreach (DataColumn dc in tableSchema.Columns)
            {
                String name = dc.ColumnName;
                int index = tableSchema.Columns.IndexOf(dc);
                dResult.Columns[name].SetOrdinal(index);
            }
            int oraResult = ApiUtil.BulkInsert(this.workType, dResult);

            if (oraResult > 0)
            {
                this.execCount += dResult.Rows.Count;

            }
            else
            {
                this.total = -1;
                this.isError = true;
            }
            this.total = this.execCount;
            this.workTime = Convert.ToDateTime(jo.GetValue("time"));
            return true;


        }

        private Boolean execute()
        {
            this.execCount = 0;
            String result;
            String delsql;
            //TODO 根据type执行接口方法
            switch (this.workType)
            {
                case "DOC_GOODS":

                    //test 
                    this.workType = "INF_GOODS_EAS";
                    delsql = "delete from " + this.workType;
                    DbHelperOra.ExecuteSql(delsql);
                    //
                    DateTime dt = DateTime.Now;
                    this.total = ApiService.getCount("GOODS", this.paraTime.ToString("yyyy-MM-dd HH:mm:ss"), "");
                    this.queryTime = DateTime.Now - dt;
                    if (this.total > 0)
                    {
                        for (int i = 0; i <= this.total; i += this.defaultStep)
                        {
                            result = ApiService.getMaterials(i, this.defaultStep, this.paraTime.ToString("yyyy-MM-dd HH:mm:ss"), "");

                            export(result, ApiUtil.GoodsColNameTable);
                        }
                        //---------------------------------------------------for test---------------------------------------------
                        if (ApiUtil.runProcedure("INFTEMP.CONV_GOODS") <= 0)
                        {
                            this.isError = true;
                            this.errorDetail = "执行存储过程时发生未知错误";
                            this.total = -1;
                        }
                        //--------------------------------------------------------------------------------------------------------
                    }
                    else if (total < 0)
                    {
                        this.isError = true;
                        this.errorDetail = "登录失败";
                        save();
                        return false;
                    }
                    else
                    {
                        this.total = -1;
                        this.isError = true;
                        this.errorDetail = "没有需要下传的物料";
                        save();
                        return false;
                    }

                    // result = ApiUtil.getMaterials(this.currentStep, this.defaultStep, this.paraTime.ToString("yyyy-MM-dd HH:mm:ss"), "");

                    //if (this.paraTime == new DateTime(1970, 1, 1, 0, 0, 0, 0))
                    //{
                    //    result = ApiUtil.getMaterials();
                    //}
                    //else
                    //{
                    //    result = ApiUtil.getMaterials(this.paraTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    //}


                    save();
                    break;

                case "DOC_GOODSUNIT":

                    this.workType = "INF_UNITS_EAS";
                    delsql = "delete from " + this.workType;
                    DbHelperOra.ExecuteSql(delsql);



                    result = ApiService.getUnits(this.paraTime.ToString("yyyy-MM-dd HH:mm:ss"));

                    export(result, ApiUtil.UnitsColNameTable);

                    //---------------------------------------run procedure
                    //int r = DbHelperOra.RunProcedure("INFTEMP.CONV_UNITS",null);


                    int r = ApiUtil.runProcedure("INFTEMP.CONV_UNITS");
                    if (r <= 0)
                    {
                        this.isError = true;
                        this.errorDetail = "执行存储过程时发生未知错误";
                        this.total = -1;
                    }
                    //-----------------------------------------
                    save();

                    break;

                case "SYS_CATEGORY":
                    //test 
                    this.workType = "INF_CATEGORY_EAS";
                    delsql = "delete from " + this.workType;
                    DbHelperOra.ExecuteSql(delsql);

                    result = ApiService.getMaterialGroups(this.paraTime.ToString("yyyy-MM-dd HH:mm:ss"));

                    export(result, ApiUtil.CategoryColNameTable);
                    if (ApiUtil.runProcedure("INFTEMP.CONV_CATEGORY") <= 0)
                    {
                        this.isError = true;
                        this.errorDetail = "执行存储过程时发生未知错误";
                        this.total = -1;

                        break;
                    }
                    save();
                    break;

                case "DAT_DD_DOC":
                    this.workTime = DateTime.Now;
                    JArray ddInputContent = new JArray();
                    if (queryDD() != null)
                    {

                        foreach (DataRow dr in this.ddFlagY.Rows)
                        {
                            Boolean thisError = false;
                            // String[] paramArray = new String[dr.ItemArray.Length];


                            JObject jo = new JObject();
                            JArray head = new JArray();
                            JArray body = new JArray();
                            String seqno = Convert.ToString(dr["SEQNO"]);
                            DateTime bizDate = Convert.ToDateTime(dr["XDRQ"]);
                            String sql = "select * from dat_dd_com where seqno = '" + seqno + "' and custid = '" + dr["CUSTID"].ToString() + "'";
                            DataTable dResult = DbHelperOra.Query(sql).Tables[0];

                            JObject joHead = new JObject();
                            joHead.Add("number", seqno + '_' + dr["CUSTID"].ToString());
                            joHead.Add("bizDate", bizDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            joHead.Add("reqtype", "WG"); //-----------------------------TODO CHANGE THIS
                            joHead.Add("custId", dr["CUSTID"].ToString()); // newly added

                            String customerSql = "select CODE,COSTCENTER,USERCODE,WAREHOUSE,SALEPERSON,STORAGEUNIT,COMPANYUNIT,STOCKER,DCBM,DRBM,XSZ,SALEBM from doc_customer where code = '" + dr["CUSTID"].ToString() + "'";
                            DataTable customerInfo = DbHelperOra.Query(customerSql).Tables[0];
                            joHead.Add("costcenter", customerInfo.Rows[0]["COSTCENTER"].ToString());
                            joHead.Add("user", customerInfo.Rows[0]["USERCODE"].ToString());
                            joHead.Add("warehouse", customerInfo.Rows[0]["WAREHOUSE"].ToString());
                            joHead.Add("saleperson", customerInfo.Rows[0]["SALEPERSON"].ToString());
                            joHead.Add("storageUnit", customerInfo.Rows[0]["STORAGEUNIT"].ToString());
                            joHead.Add("companyUnit", customerInfo.Rows[0]["COMPANYUNIT"].ToString());
                            joHead.Add("stocker", customerInfo.Rows[0]["STOCKER"].ToString());
                            joHead.Add("dcbm", customerInfo.Rows[0]["DCBM"].ToString());
                            joHead.Add("drbm", customerInfo.Rows[0]["DRBM"].ToString());
                            joHead.Add("xsz", customerInfo.Rows[0]["XSZ"].ToString());
                            joHead.Add("salebm", customerInfo.Rows[0]["SALEBM"].ToString());
                            joHead.Add("memo", dr["MEMO"].ToString());
                            head.Add(joHead);
                            for (int j = 0; j < dResult.Rows.Count; j++)
                            {
                                JObject joBody = new JObject();
                                joBody.Add("qty", JToken.FromObject(dResult.Rows[j]["DHS"]));
                                joBody.Add("memo", JToken.FromObject(dResult.Rows[j]["MEMO"]));
                                String easGDSEQ = getEasGDSEQ(dResult.Rows[j]["GDSEQ"].ToString());
                                if (easGDSEQ == null)
                                {
                                    this.isError = true;
                                    thisError = true;
                                    this.errorDetail += "SEQNO[" + dResult.Rows[j]["SEQNO"].ToString() + "]GDSEQ[" + dResult.Rows[j]["GDSEQ"].ToString() + "] 不是 EAS 商品. ";
                                    updateDD(dResult.Rows[j]["SEQNO"].ToString(), "E", dr["CUSTID"].ToString());
                                    break;
                                }
                                joBody.Add("number", easGDSEQ);
                                body.Add(joBody);
                            }
                            jo.Add("body", body);
                            jo.Add("head", head);
                            if (!thisError)
                            {
                                ddInputContent.Add(jo);
                                //result = ApiService.putStockTransferBill(jo.ToString());
                                //JObject jResult = JObject.Parse(result);
                                //if (jResult.Value<String>("result").Equals("success"))
                                //{
                                //    updateDD(dr["SEQNO"].ToString(), "S");
                                //}
                                //else
                                //{
                                //    this.isError = true;
                                //    this.errorDetail += " SEQNO[" + dr["SEQNO"].ToString() + "]" + jResult.Value<String>("result").ToString() + " " + jResult.Value<String>("faildetail");
                                //}
                            }

                        }
                        result = ApiService.putStockTransferBill(ddInputContent.ToString());
                        if ("-1".Equals(result))
                        {
                            this.isError = true;
                            this.total = -1;
                            this.errorDetail = "登录失败";
                            save();
                            break;
                        }
                        JArray jResult = JArray.Parse(result);
                        int successCount = 0;
                        foreach (JToken jt in jResult)
                        {
                            if (jt.Value<String>("result").Equals("success") && !ApiUtil.isNull(jt.Value<String>("data")))
                            {
                                String tempNumber = jt.Value<String>("data").Split('_')[0];
                                String custId = jt.Value<String>("data").Split('_')[1];
                                updateDD(tempNumber, "Y", custId);
                                this.errorDetail += " SEQNO[" + jt.Value<String>("data") + "]" + jt.Value<String>("result").ToString() + ". ";
                                successCount++;
                            }
                            else
                            {
                                this.isError = true;
                                String failDetail = jt.Value<String>("faildetail");
                                if (failDetail.IndexOf("bill_existed") > 0)
                                {
                                    String tempNumber = jt.Value<String>("data").Split('_')[0];
                                    String custId = jt.Value<String>("data").Split('_')[1];
                                    updateDD(tempNumber, "Y", custId);
                                }
                                this.errorDetail += " SEQNO[" + jt.Value<String>("data") + "]" + jt.Value<String>("result").ToString() + " " + jt.Value<String>("faildetail");
                            }
                        }
                        String successText = "已成功执行 " + successCount + "/" + ddFlagY.Rows.Count + " 条  ";
                        if (successCount > 0)
                        {
                            this.isError = false;
                        }
                        this.errorDetail = successText + this.errorDetail;
                        this.execCount = successCount;
                        this.total = ddFlagY.Rows.Count;
                    }
                    else
                    {
                        this.isError = true;
                        this.errorDetail = "没有需要上传的数据";
                        this.total = -1;
                    }
                    save();
                    break;
                case "DAT_CK_DOC":
                    this.workTime = DateTime.Now;
                    JArray ckInputContent = new JArray();
                    if (queryCK() != null)
                    {

                        foreach (DataRow dr in this.ckTable.Rows)
                        {
                            Boolean thisError = false;
                            // String[] paramArray = new String[dr.ItemArray.Length];


                            JObject jo = new JObject();
                            JArray head = new JArray();
                            JArray body = new JArray();
                            String seqno = Convert.ToString(dr["SEQNO"]);
                            DateTime bizDate = Convert.ToDateTime(dr["SHRQ"].ToString());
                            String sql = "select * from dat_ck_com where seqno = '" + seqno + "' and custid = '" + dr["CUSTID"].ToString() + "'";
                            DataTable dResult = DbHelperOra.Query(sql).Tables[0];

                            JObject joHead = new JObject();
                            joHead.Add("number", seqno + '_' + dr["CUSTID"].ToString());
                            joHead.Add("bizDate", bizDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            joHead.Add("reqtype", "WG"); //-----------------------------TODO CHANGE THIS
                            joHead.Add("custId", Convert.ToString(dr["CUSTID"])); // newly added
                            String customerSql = "select CODE,COSTCENTER,USERCODE,WAREHOUSE,SALEPERSON,STORAGEUNIT,COMPANYUNIT,STOCKER,DCBM,DRBM,XSZ,SALEBM from doc_customer where code = '" + dr["CUSTID"].ToString() + "'";
                            DataTable customerInfo = DbHelperOra.Query(customerSql).Tables[0];
                            joHead.Add("costcenter", customerInfo.Rows[0]["COSTCENTER"].ToString());
                            joHead.Add("user", customerInfo.Rows[0]["USERCODE"].ToString());
                            joHead.Add("warehouse", customerInfo.Rows[0]["WAREHOUSE"].ToString());
                            joHead.Add("saleperson", customerInfo.Rows[0]["SALEPERSON"].ToString());
                            joHead.Add("storageUnit", customerInfo.Rows[0]["STORAGEUNIT"].ToString());
                            joHead.Add("companyUnit", customerInfo.Rows[0]["COMPANYUNIT"].ToString());
                            joHead.Add("stocker", customerInfo.Rows[0]["STOCKER"].ToString());
                            joHead.Add("dcbm", customerInfo.Rows[0]["DCBM"].ToString());
                            joHead.Add("drbm", customerInfo.Rows[0]["DRBM"].ToString());
                            joHead.Add("xsz", customerInfo.Rows[0]["XSZ"].ToString());
                            joHead.Add("salebm", customerInfo.Rows[0]["SALEBM"].ToString());
                            head.Add(joHead);
                            for (int j = 0; j < dResult.Rows.Count; j++)
                            {
                                JObject joBody = new JObject();
                                joBody.Add("qty", JToken.FromObject(dResult.Rows[j]["XSSL"]));//实收数量
                                String easGDSEQ = getEasGDSEQ(dResult.Rows[j]["GDSEQ"].ToString());

                                if (easGDSEQ == null)
                                {
                                    this.isError = true;
                                    thisError = true;
                                    this.errorDetail += "SEQNO[" + dResult.Rows[j]["SEQNO"].ToString() + "]GDSEQ[" + dResult.Rows[j]["GDSEQ"].ToString() + "] 不是 EAS 商品. ";
                                    updateCK(dResult.Rows[j]["SEQNO"].ToString(), "E", dr["CUSTID"].ToString());
                                    break;
                                }
                                String ph = dResult.Rows[j]["PHID"].ToString();
                                String price = dResult.Rows[j]["HSJJ"].ToString();
                                joBody.Add("lot", ph);
                                joBody.Add("price", price);
                                joBody.Add("number", easGDSEQ);
                                body.Add(joBody);
                            }
                            jo.Add("body", body);
                            jo.Add("head", head);
                            if (!thisError)
                            {
                                ckInputContent.Add(jo);
                            }

                        }
                        result = ApiService.putSaleIssueBill(ckInputContent.ToString());
                        if ("-1".Equals(result))
                        {
                            this.isError = true;
                            this.total = -1;
                            this.errorDetail = "登录失败";
                            save();
                            break;
                        }

                        JArray jResult = JArray.Parse(result);
                        int successCount = 0;
                        foreach (JToken jt in jResult)
                        {
                            if (jt.Value<String>("result").Equals("success") && !ApiUtil.isNull(jt.Value<String>("data")))
                            {
                                String tempNumber = jt.Value<String>("data").Split('_')[0];
                                String custId = jt.Value<String>("data").Split('_')[1];
                                updateCK(tempNumber, "Y", custId);
                                this.errorDetail += " SEQNO[" + jt.Value<String>("data") + "]" + jt.Value<String>("result").ToString() + ". ";
                                successCount++;//here
                            }
                            else
                            {
                                this.isError = true;
                                String failDetail = jt.Value<String>("faildetail");
                                if (failDetail.IndexOf("bill_existed") > 0)
                                {
                                    String tempNumber = jt.Value<String>("data").Split('_')[0];
                                    String custId = jt.Value<String>("data").Split('_')[1];
                                    updateCK(tempNumber, "Y", custId);
                                }
                                this.errorDetail += " SEQNO[" + jt.Value<String>("data") + "]" + jt.Value<String>("result").ToString() + " " + jt.Value<String>("faildetail");
                            }
                        }
                        String successText = "已成功执行 " + successCount + "/" + ckTable.Rows.Count + " 条  ";
                        if (successCount > 0)
                        {
                            this.isError = false;
                        }
                        this.errorDetail = successText + this.errorDetail;
                        this.execCount = successCount;
                        this.total = ckTable.Rows.Count;
                    }
                    else
                    {
                        this.isError = true;
                        this.errorDetail = "没有需要上传的数据";
                        this.total = -1;
                    }
                    save();
                    break;
                case "DAT_QTCK_DOC":
                    this.workTime = DateTime.Now;

                    BillBeanController bbc = new BillBeanController();
                    String queryResult = bbc.renderQT();
                    this.errorDetail = bbc.errorDetail;
                    if (String.IsNullOrWhiteSpace(this.errorDetail))
                    {
                        this.isError = false;
                    }
                    else
                    {
                        this.isError = true;
                    }
                    result = ApiService.commonBillImp("009", queryResult);

                    #region 解析返回值
                    if ("-1".Equals(result))
                    {
                        this.isError = true;
                        this.total = -1;
                        this.errorDetail = "登录失败";
                        save();
                        break;
                    }
                    int successCountDD = 0;
                    try
                    {
                        JObject jResult = JObject.Parse(result);
                        int status = jResult.Value<int>("status");
                        if (status == 0)
                        {
                            successCountDD = bbc.total;
                            this.total = bbc.total;
                            updateCK(bbc.resultDT, "Y");
                        }
                        else
                        {
                            this.isError = true;
                            this.total = -1;
                            JArray message = jResult.Value<JArray>("billErrors");
                            successCountDD = bbc.total - message.Count;
                            this.errorDetail += JsonConvert.SerializeObject(message);

                            DataTable resultDT = bbc.resultDT.Copy();
                            DataTable resultTemp = resultDT.Copy();
                            foreach (JToken jt in message)
                            {
                                JObject joError = (JObject)jt;

                                foreach (DataRow drResult in resultDT.Rows)
                                {
                                    JArray errorMsgs = joError.Value<JArray>("errorMsgs");
                                    String msg1 = errorMsgs[0].ToString();
                                    //TODO change this
                                    if (msg1.IndexOf("已经存在，不能重复") < 0 && (joError.Value<String>("number") + "_" + custId).Equals(drResult["SEQNO"].ToString()))
                                    {
                                        resultTemp.Rows.Remove(drResult);
                                    }
                                }

                            }
                            updateCK(resultTemp, "Y");
                        }
                        this.errorDetail = "已成功执行" + successCountDD + "/" + bbc.total + "条。" + this.errorDetail;
                    }
                    catch (Exception ex)
                    {
                        this.errorDetail = "解析返回值时发生了错误。" + ex.Message + "\n\r" + ex.StackTrace;
                        this.isError = true;
                        this.total = -1;
                    }
                    #endregion
                    save();
                    break;
                case "DAT_TH_DOC":
                    this.workTime = DateTime.Now;
                    JArray thInputContent = new JArray();
                    if (queryTH() != null)
                    {

                        foreach (DataRow dr in this.thTable.Rows)
                        {
                            Boolean thisError = false;
                            // String[] paramArray = new String[dr.ItemArray.Length];


                            JObject jo = new JObject();
                            JArray head = new JArray();
                            JArray body = new JArray();
                            String seqno = Convert.ToString(dr["SEQNO"]);
                            DateTime bizDate = Convert.ToDateTime(dr["SHRQ"].ToString());
                            String sql = "select * from dat_th_com where seqno = '" + seqno + "' and custid='" + dr["CUSTID"].ToString() + "'";
                            DataTable dResult = DbHelperOra.Query(sql).Tables[0];

                            JObject joHead = new JObject();
                            joHead.Add("number", seqno + '_' + dr["CUSTID"].ToString());
                            joHead.Add("bizDate", bizDate.ToString("yyyy-MM-dd HH:mm:ss"));
                            joHead.Add("reqtype", "WG"); //-----------------------------TODO CHANGE THIS
                            joHead.Add("custId", Convert.ToString(dr["CUSTID"])); // newly added
                            joHead.Add("isRevert", true); //反向调拨单标志
                            String customerSql = "select CODE,COSTCENTER,USERCODE,WAREHOUSE,SALEPERSON,STORAGEUNIT,COMPANYUNIT,STOCKER,DCBM,DRBM,XSZ,SALEBM from doc_customer where code = '" + dr["CUSTID"].ToString() + "'";
                            DataTable customerInfo = DbHelperOra.Query(customerSql).Tables[0];
                            joHead.Add("costcenter", customerInfo.Rows[0]["COSTCENTER"].ToString());
                            joHead.Add("user", customerInfo.Rows[0]["USERCODE"].ToString());
                            joHead.Add("warehouse", customerInfo.Rows[0]["WAREHOUSE"].ToString());
                            joHead.Add("saleperson", customerInfo.Rows[0]["SALEPERSON"].ToString());
                            joHead.Add("storageUnit", customerInfo.Rows[0]["STORAGEUNIT"].ToString());
                            joHead.Add("companyUnit", customerInfo.Rows[0]["COMPANYUNIT"].ToString());
                            joHead.Add("stocker", customerInfo.Rows[0]["STOCKER"].ToString());
                            joHead.Add("dcbm", customerInfo.Rows[0]["DCBM"].ToString());
                            joHead.Add("drbm", customerInfo.Rows[0]["DRBM"].ToString());
                            joHead.Add("xsz", customerInfo.Rows[0]["XSZ"].ToString());
                            joHead.Add("salebm", customerInfo.Rows[0]["SALEBM"].ToString());
                            head.Add(joHead);
                            for (int j = 0; j < dResult.Rows.Count; j++)
                            {
                                JObject joBody = new JObject();
                                joBody.Add("qty", JToken.FromObject(dResult.Rows[j]["SSSL"]));//数量
                                String easGDSEQ = getEasGDSEQ(dResult.Rows[j]["GDSEQ"].ToString());

                                if (easGDSEQ == null)
                                {
                                    this.isError = true;
                                    thisError = true;
                                    this.errorDetail += "SEQNO[" + dResult.Rows[j]["SEQNO"].ToString() + "]GDSEQ[" + dResult.Rows[j]["GDSEQ"].ToString() + "] 不是 EAS 商品. ";
                                    updateTH(dResult.Rows[j]["SEQNO"].ToString(), "E", dr["CUSTID"].ToString());
                                    break;
                                }
                                String ph = dResult.Rows[j]["PHID"].ToString();

                                joBody.Add("lot", ph);
                                //String price = dResult.Rows[j]["LSJ"].ToString();
                                //joBody.Add("price", price); //------------------EAS不接收price?---------------------
                                joBody.Add("number", easGDSEQ);
                                body.Add(joBody);
                            }
                            jo.Add("body", body);
                            jo.Add("head", head);
                            if (!thisError)
                            {
                                thInputContent.Add(jo);
                            }

                        }
                        result = ApiService.putStockTransferBill(thInputContent.ToString());//HERE
                        if ("-1".Equals(result))
                        {
                            this.isError = true;
                            this.total = -1;
                            this.errorDetail = "登录失败";
                            save();
                            break;
                        }

                        JArray jResult = JArray.Parse(result);
                        int successCount = 0;
                        foreach (JToken jt in jResult)
                        {

                            if (jt.Value<String>("result").Equals("success") && !ApiUtil.isNull(jt.Value<String>("data")))
                            {
                                String tempNumber = jt.Value<String>("data").Split('_')[0];
                                String custId = jt.Value<String>("data").Split('_')[1];
                                updateTH(tempNumber, "Y", custId);
                                this.errorDetail += " SEQNO[" + jt.Value<String>("data") + "]" + jt.Value<String>("result").ToString() + ". ";
                                successCount++;
                            }
                            else
                            {
                                this.isError = true;
                                String failDetail = jt.Value<String>("faildetail");
                                if (failDetail.IndexOf("bill_existed") > 0)
                                {
                                    String tempNumber = jt.Value<String>("data").Split('_')[0];
                                    String custId = jt.Value<String>("data").Split('_')[1];
                                    updateTH(tempNumber, "Y", custId);
                                }
                                this.errorDetail += " SEQNO[" + jt.Value<String>("data") + "]" + jt.Value<String>("result").ToString() + " " + jt.Value<String>("faildetail");
                            }
                        }
                        String successText = "已成功执行 " + successCount + "/" + thTable.Rows.Count + " 条  ";
                        if (successCount > 0)
                        {
                            this.isError = false;
                        }
                        this.errorDetail = successText + this.errorDetail;
                        this.execCount = successCount;
                        this.total = thTable.Rows.Count;
                    }
                    else
                    {
                        this.isError = true;
                        this.errorDetail = "没有需要上传的数据";
                        this.total = -1;
                    }
                    save();
                    break;
                case "DAT_RK_DOC":
                    this.workTime = DateTime.Now;
                    this.ddFlagS = new DataTable();
                    if (queryRK())
                    {
                        this.total = this.ddFlagS.Rows.Count;
                        try
                        {
                            CacheHelper.SetCache("DATAUPDATE_CURRENTCOUNT_RK", 0);
                        }
                        catch (Exception ex)
                        {
                            //do nothing
                        }

                        String strSeqnoArray = "";
                        successCount = 0;
                        Boolean rkContinue = true;
                        foreach (DataRow dr in this.ddFlagS.Rows)
                        { //已上传未确认订单
                            strSeqnoArray += "'" + dr["SEQNO"].ToString() + "_" + dr["CUSTID"].ToString() + "'";
                            //if (ddFlagS.Rows.IndexOf(dr) != ddFlagS.Rows.Count - 1)
                            //{
                            strSeqnoArray += ",";
                            //}
                            if ((this.ddFlagS.Rows.IndexOf(dr) % 23 == 0 && this.ddFlagS.Rows.IndexOf(dr) != 0) || this.ddFlagS.Rows.IndexOf(dr) == this.ddFlagS.Rows.Count - 1)
                            {
                                strSeqnoArray = strSeqnoArray.TrimEnd(',');
                                if (!ApiUtil.isNull(strSeqnoArray))
                                {
                                    result = ApiService.queryState(strSeqnoArray);
                                    if ("-1".Equals(result))
                                    {
                                        this.isError = true;
                                        this.total = -1;
                                        this.errorDetail = "登录失败";
                                        save();
                                        rkContinue = false;
                                        break;
                                    }
                                    foreach (JToken jt in JArray.Parse(result))
                                    {
                                        insertRK(jt);
                                    }
                                    strSeqnoArray = "";
                                    try
                                    {
                                        CacheHelper.SetCache("DATAUPDATE_CURRENTCOUNT_RK", this.ddFlagS.Rows.IndexOf(dr));
                                        //UseSession.SetSession(context,"DATAUPDATE_CURRENTCOUNT_RK", this.ddFlagS.Rows.IndexOf(dr));
                                        //System.Web.HttpContext.Current.Session["DATAUPDATE_CURRENTCOUNT_RK"] = this.ddFlagS.Rows.IndexOf(dr);
                                    }
                                    catch (Exception ex)
                                    {
                                        //do nothing
                                    }
                                    //this.isError = false;
                                    //String successText = "已成功执行 " + successCount + "/" + JArray.Parse(result).Count + " 条. ";
                                    //this.errorDetail = successText + this.errorDetail;
                                }
                            }
                        }
                        if (!rkContinue)
                        {
                            break;
                        }

                        try
                        {
                            CacheHelper.SetCache("DATAUPDATE_CURRENTCOUNT_RK", ddFlagS.Rows.Count);
                            //UseSession.SetSession(context,"DATAUPDATE_CURRENTCOUNT_RK", this.ddFlagS.Rows.IndexOf(dr));
                            //System.Web.HttpContext.Current.Session["DATAUPDATE_CURRENTCOUNT_RK"] = this.ddFlagS.Rows.IndexOf(dr);
                        }
                        catch (Exception ex)
                        {
                            //do nothing
                        }
                        #region
                        //result = ApiService.queryState(strSeqnoArray);
                        //if ("-1".Equals(result))
                        //{
                        //    this.isError = true;
                        //    this.total = -1;
                        //    this.errorDetail = "登录失败";
                        //    save();
                        //    break;
                        //}
                        //foreach (JToken jt in JArray.Parse(result))
                        //{
                        //    insertRK(jt);
                        //}
                        //this.isError = false;
                        #endregion
                        this.isError = false;
                        String successText = "已成功执行 " + successCount + "/" + ddFlagS.Rows.Count + " 条. ";
                        this.errorDetail = successText + this.errorDetail;



                        //result = ApiService.queryState("ERP20141006");
                        //insertRK(result, "ERP20141006");
                    }
                    else
                    {
                        this.total = -1;
                        this.isError = true;
                        this.errorDetail = "没有符合条件的数据";
                    }
                    save();
                    break;

                case "DD_CONFIRM":
                    //在某一订单的物料全部审核入库时，需要执行此方法
                    //处理插入的入库单，调用 recordState ,flag = 'S'  => 'F' 
                    if (ApiUtil.isNull(this.ddBillNo))
                    {
                        return false;
                    }
                    else
                    {
                        try
                        {
                            //result = ApiService.recordState(this.ddBillNo, "");//标记状态已完成

                            //if (JsonConvert.DeserializeObject<JObject>(result).Value<String>("result").Equals("success"))
                            //{
                            //    updateDD(ddBillNo, "F");
                            //}
                            //else
                            //{
                            //    this.isError = true;
                            //}
                        }
                        catch
                        {
                            this.isError = true;
                        }
                        save();
                    }
                    break;
                case "DAT_BILLSTATUS_LOG":
                    //入 = 订单号+custId 出 =string 
                    try
                    {
                        handleBillStatus();
                    }
                    catch (Exception ex)
                    {
                        this.errorDetail = ex.Message;
                        this.isError = true;
                    }
                    save();
                    break;
                case "MessageSend":
                    //发送定时短信
                    MessageOperTimer();
                    save();
                    break;
                default:
                    return false;
            }

            return true;
        }

        public Boolean exec()
        {
            init();
            return execute();
        }

        private String custId = "";
        public Boolean exec(String billNo, String custId)
        {
            init();
            this.ddBillNo = billNo;
            this.custId = custId;
            return execute();
        }

        public Boolean exec(String billNo)
        {
            init();
            this.ddBillNo = billNo;
            return execute();
        }



        public Boolean exec(DateTime date)
        {
            init();
            this.paraTime = date;
            return execute();
        }

        #region 订单状态相关
        public Boolean handleBillStatus()
        {
            Boolean result = false;
            String billno = this.ddBillNo;
            String custId = this.custId;
            //TODO 处理订单状态 写入到dat_billstatus_log中

            return result;
        }
        #endregion

        #region  查询订单 flag = 'Y'
        /// <summary>
        /// 查询订单 flag = 'Y'
        /// </summary>
        /// <returns></returns>
        private DataTable queryDD()
        {
            this.ddFlagY = new DataTable();
            JObject jo = new JObject();
            //

            String sql = "select * from dat_dd_doc where flag = 'Y' and issend='N' AND SUPID IN (SELECT SUPID FROM DOC_SUPPLIER WHERE ISDG='N')";


            //            String sql = @"select * from dat_dd_doc ddmain where exists(
            //                    select * from
            //                    ( 
            //                    select distinct(ddc.gdseq),ddc.seqno,sum(dhs) as sumdhs  from dat_dd_com ddc 
            //                    group by ddc.gdseq,ddc.seqno
            //                    ) dd
            //                    left join dat_rk_doc rkmain on dd.seqno = rkmain.ddbh
            //                    left join (
            //                         select distinct(gdseq),sum(sssl) sumsssl,seqno from dat_rk_com rkcom
            //
            //                           group by gdseq,seqno
            //                    ) rk on dd.gdseq = rk.gdseq and rkmain.seqno = rk.seqno
            //                    where dd.seqno = ddmain.seqno and (dd.sumdhs <> rk.sumsssl or rk.sumsssl is null)
            //                ) and flag = 'Y' and issend = 'N' and isend = 'N'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            int mCount = Convert.ToInt16(mResult.Rows.Count);
            if (mCount > 0)
            {
                this.ddFlagY = mResult;
                return mResult;

                //JArray head = new JArray();
                //JArray body = new JArray();
                //for (int i = 0; i < mCount; i++)
                //{
                //    String seqno = Convert.ToString(mResult.Rows[i]["SEQNO"]);
                //    DateTime bizDate = Convert.ToDateTime(mResult.Rows[i]["XDRQ"]);
                //    sql = "select * from dat_dd_com where seqno = '" + seqno + "'";
                //    DataTable dResult = DbHelperOra.Query(sql).Tables[0];

                //    JObject joHead = new JObject();
                //    joHead.Add("number", JToken.FromObject(seqno));
                //    joHead.Add("bizDate", bizDate.ToString("yyyy-MM-dd HH:mm:ss"));
                //    joHead.Add("reqtype", "WG"); //-----------------------------TODO CHANGE THIS
                //    head.Add(joHead);
                //    for (int j = 0; j < dResult.Rows.Count; j++)
                //    {
                //        JObject joBody = new JObject();
                //        joBody.Add("qty", JToken.FromObject(dResult.Rows[j]["DHS"]));
                //        joBody.Add("number", JToken.FromObject(dResult.Rows[j]["GDSEQ"]));
                //        body.Add(joBody);
                //    }

                //}
                //jo.Add("body", body);
                //jo.Add("head", head);

            }
            else
            {
                return null;
            }
        }
        #endregion

        //G 已执行 
        #region 查询出库单
        private DataTable queryCK()
        {
            this.ckTable = new DataTable();
            JObject jo = new JObject();
            String sql = @"select * from dat_ck_doc c,doc_customer t where c.custid=t.code and t.settlementway<>'NZJ' and c.flag = 'G' and c.issend='N' and nvl(c.num2,0)=0
                           union
                           select * from dat_ck_doc c,doc_customer t where c.custid=t.code and c.num2=1 and c.flag = 'G' and c.issend='N'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            int mCount = Convert.ToInt16(mResult.Rows.Count);
            if (mCount > 0)
            {
                this.ckTable = mResult;
                return mResult;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 查询退货单
        private DataTable queryTH()
        {
            this.thTable = new DataTable();
            JObject jo = new JObject();
            String sql = "select * from dat_th_doc where flag= 'Y' and issend='N'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            int mCount = Convert.ToInt16(mResult.Rows.Count);
            if (mCount > 0)
            {
                this.thTable = mResult;
                return mResult;
            }
            else
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 得到EAS物料编码
        /// </summary>
        /// <param name="gdseq"></param>
        /// <returns></returns>
        private String getEasGDSEQ(String gdseq)
        {
            String result = null;
            String sql = "select BAR3 FROM DOC_GOODS WHERE GDSEQ = '" + gdseq + "'";
            DataTable dt = DbHelperOra.Query(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0][0].ToString();
            }
            return result;
        }

        private String getERPGDSEQ(String material, String custId)
        {
            String sql = "select a.gdseq from doc_goods a,doc_customer c,doc_custrange r ,doc_goodsgroup gr where a.flag='Y' and c.code = '" + custId + "' and gr.eascode='" + material + "' and a.gdseq = gr.gdseq and gr.groupid=r.catid and c.code=r.custid";
            //String sql = "select a.gdseq from doc_goods a,doc_customer c,doc_custrange r where a.flag='Y' and c.code = '" + custId + "' and a.bar3='"+material+"' and a.catid0=r.catid and c.code=r.custid";
            String result = null;
            DataTable dt = DbHelperOra.Query(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                result = dt.Rows[0][0].ToString();
            }
            return result;
        }

        #region 更新订单
        /// <summary>
        /// 批量更新订单
        /// </summary>
        /// <param name="dt">保存订单记录的datatable</param>
        /// <param name="flag">flag值，S 已上传/F已确认</param>
        /// <returns></returns>
        private Boolean updateDD(DataTable dt, String flag)
        {
            String seqNoOld = "";
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + dr["CUSTID"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            String sql = "update DAT_DD_DOC set issend = '" + flag + "' where  seqno||custid in (" + seqNoOld + ") ";
            try
            {
                DbHelperOra.ExecuteSql(sql);
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="billno">订单号</param>
        /// <param name="flag">flag值，S 已上传/F已确认</param>
        /// <returns></returns>
        private Boolean updateDD(String billno, String flag, String custId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SEQNO", typeof(String));
            dt.Columns.Add("CUSTID", typeof(String));
            DataRow dr = dt.NewRow();
            dr["SEQNO"] = billno;
            dr["CUSTID"] = custId;
            dt.Rows.Add(dr);
            return updateDD(dt, flag);
        }

        private Boolean updateCK(DataTable dt, String flag)
        {
            String seqNoOld = "";
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + dr["CUSTID"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            String sql = "update DAT_CK_DOC set issend = '" + flag + "' where  seqno||CUSTID in (" + seqNoOld + ")";
            try
            {
                DbHelperOra.ExecuteSql(sql);
                return true;
            }
            catch
            {
                return false;
            }

        }

        private Boolean updateCK(String billno, String flag, String custId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SEQNO", typeof(String));
            dt.Columns.Add("CUSTID", typeof(String));
            DataRow dr = dt.NewRow();
            dr["SEQNO"] = billno;
            dr["CUSTID"] = custId;
            dt.Rows.Add(dr);
            return updateCK(dt, flag);
        }

        private Boolean updateTH(DataTable dt, String flag)
        {
            String seqNoOld = "";
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + dr["CUSTID"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            String sql = "update DAT_TH_DOC set issend = '" + flag + "' where  seqno||CUSTID in (" + seqNoOld + ") ";
            try
            {
                DbHelperOra.ExecuteSql(sql);
                return true;
            }
            catch
            {
                return false;
            }

        }

        private Boolean updateTH(String billno, String flag, String custId)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SEQNO", typeof(String));
            dt.Columns.Add("CUSTID", typeof(String));
            DataRow dr = dt.NewRow();
            dr["SEQNO"] = billno;
            dr["CUSTID"] = custId;
            dt.Rows.Add(dr);
            return updateTH(dt, flag);
        }
        #endregion

        #region 查询已上传未确认订单 flag = 'S'
        /// <summary>
        /// 查询已上传未确认订单
        /// </summary>
        /// <returns></returns>
        private Boolean queryRK()
        {
            JObject jo = new JObject();
            this.ddFlagS = new DataTable();
            //只查询 XX 天前的订单记录，更早的记录不再使用
            //XX来自Web.Config的 ERP_BILLTIMEOUT
            //String sql = "select * from dat_dd_doc where flag = 'Y' and issend = 'Y' and isend ='N' and xdrq>to_date('"+DateTime.Now+"','yyyy/mm/dd hh24:mi:ss')-"+ApiUtil.GetConfigCont("ERP_BILLTIMEOUT"); //已上传未确认订单
            String sql = @"select * from dat_dd_doc ddmain where exists(
                            select * from
                            ( 
                            select distinct(ddc.gdseq),ddc.seqno,ddc.custid,sum(dhs) as sumdhs  from dat_dd_com ddc 
                            group by ddc.gdseq,ddc.seqno,ddc.custid
                            ) dd
                            left join dat_rk_doc rkmain on dd.seqno = rkmain.ddbh and dd.custid = rkmain.custid
                            left join (
                                 select distinct(gdseq),sum(sssl) sumsssl,seqno,custid from dat_rk_com rkcom

                                   group by gdseq,seqno,custid
                            ) rk on dd.gdseq = rk.gdseq and rkmain.seqno = rk.seqno and rkmain.custid = rk.custid
                            where dd.seqno = ddmain.seqno and (dd.sumdhs <> rk.sumsssl or rk.sumsssl is null)
                        ) and flag = 'Y' and issend = 'Y' and isend = 'N' and xdrq>to_date('" + DateTime.Now + "','yyyy/mm/dd hh24:mi:ss')-" + ApiUtil.GetConfigCont("ERP_BILLTIMEOUT");
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            int mCount = Convert.ToInt16(mResult.Rows.Count);
            if (mCount > 0)
            {
                this.ddFlagS = mResult;

            }
            else
            {
                return false;
            }
            return true;

        }
        #endregion

        #region 生成入库单
        private Boolean insertRK(JToken data)
        {
            List<CommandInfo> list = new List<CommandInfo>();

            if (JsonConvert.SerializeObject(data).Length <= 2)
            {
                return false;
            }
            if (data.Value<String>("result").Equals("success"))
            {
                JObject jo = data.Value<JObject>("data");
                String number = jo.Value<String>("number").Split('_')[0];
                String custId = "";
                if (jo.Value<String>("number").Split('_').Length > 1)
                {
                    custId = jo.Value<String>("number").Split('_')[1];
                }
                if (!ApiUtil.isNull(Convert.ToString(custId)))
                {



                    JArray entries = jo.Value<JArray>("entries");
                    JArray inStore = jo.Value<JArray>("inStore");

                    foreach (JObject entry in entries)
                    {
                        Boolean ERPstate = entry.Value<Boolean>("ERPstate");//ERP向eas确认状态
                        if (true) //不判断确认状态                                                                                        
                        { //---------------------------------------------------------change this for test -----------------------------------------
                            foreach (JObject rk in inStore)
                            {
                                JArray rEntries = rk.Value<JArray>("entries");
                                Boolean needHandle = false;
                                Decimal qtyTotal = 0;
                                Decimal priceTotal = 0;
                                Decimal decSubNum = 0;
                                foreach (JObject rEntry in rEntries)
                                {
                                    if (rEntry.Value<String>("material").Equals(entry.Value<String>("material")))
                                    {
                                        needHandle = true;
                                        qtyTotal += rEntry.Value<Decimal>("qty");
                                        decSubNum++;
                                        priceTotal += rEntry.Value<Decimal>("qty") * rEntry.Value<Decimal>("price");
                                    }
                                }
                                if (needHandle)
                                {
                                    String billNo = rk.Value<String>("number");
                                    String billType = "RKD";
                                    String flag = "N";//单据状态(N新单Y已审核C已取消G已执行)
                                    String supId = "WG";//供应商id （威高）----------------------------------临时-------------------------------------
                                    String supName = "威高"; //供应商名称 （威高）
                                    DateTime dhrq = Convert.ToDateTime(rk.Value<String>("bizdate")); //到货日期
                                    Decimal subNum = decSubNum; //明细条数
                                    Decimal subSum = priceTotal;//明细价格 
                                    DateTime lrrq = DateTime.Now; //录入日期
                                    String lry = "admin"; //录入人


                                    String exist = "select count(1) from dat_rk_doc where seqno = '" + billNo + "' and custid = '" + custId + "'";
                                    Boolean billExists = DbHelperOra.Exists(exist);
                                    if (!billExists)
                                    {
                                        String ddbh = number;//订单编号
                                        Boolean hasError = false;
                                        String sql = @"insert into dat_rk_doc(seqno,billno,billType,flag,supid,supname,subnum,subsum,lry,lrrq,dhrq,ddbh,custid) values('" + billNo + "','" + billNo + "','" + billType + "','" + flag + "','" + supId + "','" + supName + "'," + subNum + "," + subSum + ",'" + lry + "',to_date('" + lrrq + "','yyyy/mm/dd hh24:mi:ss'),to_date('" + dhrq + "','yyyy/mm/dd hh24:mi:ss'),'" + ddbh + "','" + custId + "'）";
                                        list.Add(new CommandInfo(sql, null));
                                        //try
                                        //{
                                        //    DbHelperOra.ExecuteSql(sql);
                                        //}
                                        //catch (Exception ex)
                                        //{
                                        //    hasError = true;
                                        //    errorDetail += "SEQNO[" + number + "] FAIL " + ex.Message + "  ";
                                        //}

                                        if (!hasError)
                                        {

                                            foreach (JObject rEntry in rEntries)
                                            {
                                                //String rNumber = rEntry.Value<String>("number");
                                                String rNumber = billNo;
                                                int rowNum = rEntries.IndexOf(rEntry) + 1;
                                                String gdseq = getERPGDSEQ(rEntry.Value<String>("material"), custId);
                                                if (gdseq != null)
                                                {
                                                    //String gdName = rEntry.Value<String>("")
                                                    String gdName = "";
                                                    String unit = "";
                                                    String gdspec = "";
                                                    String gdmode = "";
                                                    String catid = "";
                                                    Decimal bzhl = 1;
                                                    Decimal ddsl = 0;


                                                    Decimal sssl = rEntry.Value<Decimal>("qty");
                                                    Decimal bzsl = 0;
                                                    Decimal jxtax = 0;
                                                    Decimal price = 0;
                                                    Decimal hsjj = 0;
                                                    Decimal bhsjj = 0;
                                                    DataTable good = getGoods(gdseq);
                                                    Decimal hsje = 0;
                                                    Decimal bhsje = 0;
                                                    String islot = "2";
                                                    String yxqz = rEntry.Value<String>("exp");
                                                    String rq_sc = rEntry.Value<String>("mfg");
                                                    String phid = rEntry.Value<String>("lot");
                                                    if (good.Rows.Count > 0)
                                                    {
                                                        try
                                                        {
                                                            String ddslSql = "select c.DHS,c.bzhl from dat_dd_doc d left join dat_dd_com c on d.seqno = c.seqno and d.custid = c.custid where d.seqno = '" + ddbh + "' and d.custid = '" + custId + "' and c.gdseq = '" + gdseq + "'";
                                                            DataTable ddslTable = DbHelperOra.Query(ddslSql).Tables[0];
                                                            ddsl = Convert.ToDecimal(ddslTable.Rows[0]["DHS"].ToString());
                                                            bzhl = Convert.ToDecimal(ddslTable.Rows[0]["BZHL"].ToString());


                                                            gdName = Convert.ToString(good.Rows[0]["GDNAME"]);
                                                            unit = Convert.ToString(good.Rows[0]["UNIT"]);
                                                            gdspec = Convert.ToString(good.Rows[0]["GDSPEC"]);
                                                            gdmode = Convert.ToString(good.Rows[0]["GDMODE"]);
                                                            catid = Convert.ToString(good.Rows[0]["CATID"]);
                                                            //bzhl = Convert.ToDecimal(good.Rows[0]["BZHL"]);

                                                            bzsl = ApiUtil.divide(sssl, bzhl);// sssl / bzhl;
                                                            if (ddsl > sssl)
                                                            {
                                                                ddsl = sssl;
                                                            }

                                                            //sssl = bzhl * bzsl;
                                                            //ddsl = bzhl * sssl;
                                                            jxtax = Convert.ToDecimal(good.Rows[0]["JXTAX"]);
                                                            price = rEntry.Value<Decimal>("price");
                                                            hsjj = rEntry.Value<Decimal>("price");
                                                            bhsjj = 0;
                                                            hsje = hsjj * sssl;
                                                            bhsje = bhsjj * sssl;
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            this.isError = true;
                                                            this.errorDetail += "错误信息：" + ex.Message;
                                                            hasError = true;
                                                        }
                                                    }
                                                    String rSql = @"insert into dat_rk_com(seqno,rowno,gdseq,gdname,unit,gdspec,gdmode,catid,bzhl,ddsl,sssl,jxtax,hsjj,bhsjj,hsje,bhsje,islot,yxqz,rq_sc,custid,phid,ph,bzsl)
                                                        values('" + rNumber + "','" + rowNum + "','" + gdseq + "','" + gdName + "','" + unit + "','" + gdspec + "','" + gdmode + "','" + catid + "'," + bzhl + "," + ddsl + "," + sssl + "," + jxtax + "," + hsjj + "," + bhsjj + "," + hsje + "," + bhsje + "," + islot + ",to_date('" + yxqz + "','yyyy-mm-dd'),to_date('" + rq_sc + "','yyyy-mm-dd'),'" + custId + "','" + phid + "','" + phid + "'," + bzsl + ")";
                                                    list.Add(new CommandInfo(rSql, null));
                                                    //DbHelperOra.ExecuteSql(rSql);

                                                }
                                                else
                                                { //gdseq ==null
                                                    this.isError = true;
                                                    this.errorDetail += "EASMAT[" + rEntry.Value<String>("material") + "] 物料未同步！";
                                                    hasError = true;
                                                }
                                            }
                                            if (!hasError) {
                                                try
                                                {
                                                    DbHelperOra.ExecuteSqlTran(list);
                                                }
                                                catch (Exception ex)
                                                {
                                                    hasError = true;
                                                    this.isError = true;
                                                    this.errorDetail += ex.Message;
                                                }
                                            }
                                            if (!hasError)
                                            {
                                               
                                                //更改ddsl
                                                String rkNumber = rk.Value<String>("number");
                                                String chkSql = @"SELECT * FROM (                           
                                                SELECT RKD.SEQNO,RKD.DDBH,RKC.GDSEQ,DDC.DHS,SUM(RKC.SSSL) AS SUMSSSL,rkd.custid FROM DAT_RK_DOC RKD
                                                LEFT JOIN
                                                DAT_RK_COM RKC ON RKD.SEQNO = RKC.SEQNO and rkd.custid = rkc.custid
 
                                                LEFT JOIN DAT_DD_DOC DDD ON RKD.DDBH = DDD.SEQNO and rkd.custid = ddd.custid
                                                LEFT JOIN DAT_DD_COM DDC ON DDD.SEQNO = DDC.SEQNO AND RKC.GDSEQ = DDC.GDSEQ and ddd.custid = ddc.custid

                                                GROUP BY RKD.SEQNO,RKD.DDBH,DDC.DHS,RKC.GDSEQ,rkd.custid
                                                )WHERE SUMSSSL<DHS AND SEQNO = '{0}' and custid = '" + custId + "'";
                                                DataTable chkDt = new DataTable();
                                                try
                                                {
                                                    chkDt = DbHelperOra.Query(String.Format(chkSql, rkNumber)).Tables[0];
                                                    if (chkDt.Rows.Count > 0)
                                                    {
                                                        foreach (DataRow dr in chkDt.Rows)
                                                        {
                                                            Decimal minusCount = Convert.ToDecimal(dr["DHS"].ToString()) - Convert.ToDecimal(dr["SUMSSSL"].ToString());
                                                            String chkGdseq = dr["GDSEQ"].ToString();
                                                            String chkUpdateSql = @"UPDATE DAT_RK_COM C SET c.ddsl = c.ddsl+{0}
                                                                                    WHERE C.SEQNO = '{1}' and c.gdseq='{2}' and c.custid='" + custId + @"' AND C.ROWNO = (
                                                                                    SELECT max(rowno) as rowno from dat_rk_com where seqno = c.seqno and gdseq = c.gdseq 
                                                                                    )";
                                                            DbHelperOra.ExecuteSql(String.Format(chkUpdateSql, minusCount, rkNumber, chkGdseq));
                                                        }
                                                    }
                                                    this.errorDetail += "DDBH[" + number + "],SEQNO[" + rkNumber + "] SUCCESS. ";
                                                    this.successCount++;
                                                }
                                                catch (Exception ex)
                                                {
                                                    this.isError = true;
                                                    this.errorDetail += "SEQNO[" + number + "] FAIL \n\r" + ex.Message + "\n\r" + ex.StackTrace;
                                                }

                                            }
                                        }
                                        //
                                    }
                                    else
                                    { //bill exist
                                        this.isError = true;
                                        if (!this.errorDetail.Contains(billNo))
                                        {
                                            this.errorDetail += "SEQNO[" + billNo + "] 入库单号重复";
                                        }
                                    }
                                }


                            }
                        }//----------------------------------------------------change this for test---------------------------------------------
                    }

                    return true;


                }
                else
                {

                    this.isError = true;
                    this.errorDetail += " SEQNO[" + number + "]" + "未分配CUSTID. ";
                    return false;
                }

            }
            else
            {
                String number = data.Value<String>("data");
                this.isError = true;
                this.errorDetail += " SEQNO[" + number + "]" + data.Value<String>("result") + " " + data.Value<String>("faildetail") + ". ";
                return false;
            }
        }
        #endregion



        #region 保存日志 更新记录
        private void save()
        {

            //MyTable log = new MyTable("DAT_INFDATA_LOG");
            //MyTable def = new MyTable("DAT_INFDATA_DEF");

            //TODO save record to dat_infdata_def and dat_infdata_log
            this.exectime = DateTime.Now - this.startTime;
            string strIsError = this.isError ? "FAIL" : "SUCCESS";

            //string sqlLog = "insert into dat_infdata_log(execrq,pararq,execcount,exectime,execmemo) values (to_date('" + DateTime.Now + "','yyyy/mm/dd hh24:mi:ss'),to_date('" + this.paraTime + "','yyyy/mm/dd hh24:mi:ss')," + this.execCount + "," + this.exectime.TotalSeconds + ",'" + this.orgWorkType + " " + strIsError + " " + this.errorDetail + "')";

            string sqlLog = "insert into dat_infdata_log(execrq,pararq,execcount,exectime,execmemo) values (to_date('" + DateTime.Now + "','yyyy/mm/dd hh24:mi:ss'),to_date('" + this.paraTime + "','yyyy/mm/dd hh24:mi:ss')," + this.execCount + "," + this.exectime.TotalSeconds + ",:memo)";
            string sqlDef = @"update dat_infdata_def set execrq=to_date('" + this.workTime + "','yyyy/mm/dd hh24:mi:ss'),execcount = " + this.execCount + ",exectime = " + this.exectime.TotalSeconds + ",execmemo=:memo where inftype = '" + this.orgWorkType + "'";
            if (this.isError)
            {
                sqlDef = @"update dat_infdata_def set execcount = " + this.execCount + ",exectime = " + this.exectime.TotalSeconds + ",execmemo=:memo where inftype = '" + this.orgWorkType + "'";
            }
            List<CommandInfo> list = new List<CommandInfo>();

            OracleParameter[] param = {
                                          new OracleParameter("memo", OracleDbType.Clob)
                                      };
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = this.orgWorkType + " " + strIsError + " " + this.errorDetail;

            list.Add(new CommandInfo(sqlLog, param));
            list.Add(new CommandInfo(sqlDef, param));
            try
            {
                DbHelperOra.ExecuteSqlTran(list);
            }
            catch
            {
                //
            }


            //DbHelperOra.ExecuteSql(sqlLog);

            // string sqlDef = @"update dat_infdata_def set execrq=to_date('" + this.workTime + "','yyyy/mm/dd hh24:mi:ss'),execcount = " + this.execCount + ",exectime = " + this.exectime.TotalSeconds + ",execmemo='" + this.orgWorkType + " " + strIsError + " " + this.errorDetail + "' where inftype = '" + this.orgWorkType + "'";

            // DbHelperOra.ExecuteSql(sqlDef);

        }
        public void save(String errorMsg)
        {
            this.exectime = DateTime.Now - this.startTime;
            string strIsError = "FAIL";
            this.total = -1;
            //string sqlLog = "insert into dat_infdata_log(execrq,pararq,execcount,exectime,execmemo) values (to_date('" + DateTime.Now + "','yyyy/mm/dd hh24:mi:ss'),to_date('" + this.paraTime + "','yyyy/mm/dd hh24:mi:ss')," + this.execCount + "," + this.exectime.TotalSeconds + ",'" + this.orgWorkType + " " + strIsError + " " + errorMsg.Replace("'","\"") + "')";
            //DbHelperOra.ExecuteSql(sqlLog);
            //不更改上次执行时间
            //string sqlDef = @"update dat_infdata_def set execcount = " + this.execCount + ",exectime = " + this.exectime.TotalSeconds + ",execmemo='" + this.orgWorkType + " " + strIsError + " " + errorMsg.Replace("'", "\"") + "' where inftype = '" + this.orgWorkType + "'";
            //DbHelperOra.ExecuteSql(sqlDef);

            string sqlLog = "insert into dat_infdata_log(execrq,pararq,execcount,exectime,execmemo) values (to_date('" + DateTime.Now + "','yyyy/mm/dd hh24:mi:ss'),to_date('" + this.paraTime + "','yyyy/mm/dd hh24:mi:ss')," + this.execCount + "," + this.exectime.TotalSeconds + ",:memo)";
            string sqlDef = @"update dat_infdata_def set execrq=to_date('" + this.workTime + "','yyyy/mm/dd hh24:mi:ss'),execcount = " + this.execCount + ",exectime = " + this.exectime.TotalSeconds + ",execmemo=:memo where inftype = '" + this.orgWorkType + "'";
            if (this.isError)
            {
                sqlDef = @"update dat_infdata_def set execcount = " + this.execCount + ",exectime = " + this.exectime.TotalSeconds + ",execmemo=:memo where inftype = '" + this.orgWorkType + "'";
            }
            List<CommandInfo> list = new List<CommandInfo>();

            OracleParameter[] param = {
                                          new OracleParameter("memo", OracleDbType.Clob)
                                      };
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = this.orgWorkType + " " + strIsError + " " + errorMsg.Replace("'", "\"");

            list.Add(new CommandInfo(sqlLog, param));
            list.Add(new CommandInfo(sqlDef, param));
            try
            {
                DbHelperOra.ExecuteSqlTran(list);
            }
            catch
            {
                //
            }

        }
        #endregion

        #region 检查、设置单据编号 (暂时不用)
        public string BillSeqGet(String strBillType)    //检查、设置单据编号
        {
            return PubFunc.DbGetRule("BILL_" + strBillType, "N");
        }
        #endregion

        #region 根据gdseq取物料信息
        private DataTable getGoods(String id)
        {
            String sql = "select * from doc_goods where gdseq = '" + id + "'";
            DataTable dt = DbHelperOra.Query(sql).Tables[0];
            return dt;
        }
        #endregion

        public void setCurrParaTime(String type)
        {
            String sql = "select execrq from dat_infdata_def where inftype='" + type + "'";
            try
            {
                DataTable dt = DbHelperOra.Query(sql).Tables[0];
                this.paraTime = Convert.ToDateTime(dt.Rows[0]["EXECRQ"].ToString());
            }
            catch
            {
                this.paraTime = new DateTime();
            }

            //this.paraTime 
        }

        #region 短信发送相关方法
        //定时器调用发送定时短信
        public static void MessageOperTimer()
        {
            string strSql = @"select * from dat_msgtel_list
                              where flag='N' and isTiming='1' and timingsj<to_date('{0}','yyyy-mm-dd hh24:mi:ss') 
                              order by CLASS DESC";
            DataTable dt = DbHelperOra.Query(string.Format(strSql, DateTime.Now.ToString())).Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    MessageOper(dr);
                }
            }
        }
        //处理短信
        public static void MessageOper(DataRow dr)
        {
            //先发送短信
            string sendSj = DateTime.Now.ToString();
            object sendState = null;
            sendState = SendMessage(dr["sendDoc"].ToString(), dr["getTel"].ToString());

            //处理返回值
            string retSj = DateTime.Now.ToString();
            string strState = "";
            string strPh = "";
            string strFlag = "S";//S-发送成功；E-发送不成功
            if (sendState != null)
            {
                System.Reflection.FieldInfo fieldInfoId = sendState.GetType().GetField("Id");
                int intId = (int)fieldInfoId.GetValue(sendState);
                strPh = intId.ToString();
                System.Reflection.FieldInfo fieldInfoState = sendState.GetType().GetField("State");
                int intState = (int)fieldInfoState.GetValue(sendState); ;
                System.Reflection.FieldInfo fieldInfoFailPhone = sendState.GetType().GetField("FailPhone");
                string strFailPhone = (string)fieldInfoFailPhone.GetValue(sendState);

                switch (intState)
                {
                    case 0:
                        strState = "帐户处于禁止使用状态";
                        break;
                    case 1:
                        strState = "发送短信成功";
                        break;
                    case -1:
                        strState = "发送失败";
                        break;
                    case -2:
                        strState = "帐户信息错误";
                        break;
                    case -3:
                        strState = "机构代码、用户名或密码错误";
                        break;
                    case -4:
                        strState = "不是普通帐户";
                        break;
                    case -5:
                        strState = "发送短信内容为空";
                        break;
                    case -6:
                        strState = "短信内容过长";
                        break;
                    case -7:
                        strState = "发送号码为空";
                        break;
                    case -8:
                        strState = "余额不足";
                        //如果余额不足，应提醒系统管理员

                        break;
                    case -9:
                        strState = "接收数据失败";
                        break;
                    case -10:
                        strState = "号码错误";
                        break;
                    case -11:
                        strState = "发送时间格式错误";
                        break;
                    case -12:
                        strState = "定时时间必须大于当前时间";
                        break;
                    case -13://如是失败-13，返回strFailPhone是包含的关键字
                        strState = "内容含关键字" + strFailPhone;
                        break;
                    case -14:
                        strState = "信息内容格式与限定格式不符";
                        break;
                    case -15:
                        strState = "信息没带签名";
                        break;
                    case -30:
                        strState = "非绑定IP";
                        break;
                    case -100:
                        strState = "客户端获取状态失败";
                        break;
                    default:
                        strState = "发送失败,不明原因";
                        break;
                }

                //如果strFailPhone不是空值，说明该批次有部分接收电话发送失败
                if (intState != -13 && !String.IsNullOrWhiteSpace(strFailPhone))
                {
                    strState = strFailPhone + "发送失败";
                }
            }
            else
            {
                strState = "发送失败,连接不到短信接口";
                strFlag = "E";
            }

            //短信保存到数据库
            string strSql = @"update dat_msgtel_list set flag='{0}',sendsj=to_date('{1}','yyyy/mm/dd hh24:mi:ss'),retmsg='{2}',retsj=to_date('{3}','yyyy/mm/dd hh24:mi:ss'),retph='{4}'
                               where seqno='{5}'";
            strSql = string.Format(strSql, strFlag, sendSj, strState, retSj, strPh, dr["seqno"].ToString());
            DbHelperOra.ExecuteSql(strSql);
        }

        //发送短信
        public static object SendMessage(string sendDoc, string getTel)
        {
            int Id = 300;//机构代码
            try
            {
                Id = Convert.ToInt32(ApiClientUtil.GetConfigCont("MESSAGE_ID"));
            }
            catch
            { }
            string Name = ApiClientUtil.GetConfigCont("MESSAGE_NAME");//账户名
            string Psw = ApiClientUtil.GetConfigCont("MESSAGE_PWD");//密码
            string webServiceURL = ApiClientUtil.GetConfigCont("MESSAGE_WEBSERVICEURL");//webService地址
            long Timestamp = GetTimeStamp();
            object result = null;
            try
            {
                result = DynamicServiceBind.InvokeWebService(webServiceURL, "SendMessage", new object[] { Id, Name, Psw, sendDoc, getTel, Timestamp });
            }
            catch (Exception e)
            {
            }
            return result;
        }

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        #endregion
    }

    public class ApiPlatformService
    {
        public String errorDetail = "";
        public ApiPlatformService() { }
        /// <summary>
        /// 根据key查sql查结果返回json
        /// </summary>
        /// <param name="key"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public JArray Query(String key, params String[] paramArray)
        {
            //String sql = String.Format(ApiUtil.GetConfigCont(key), paramArray);
            String sql = String.Format(ApiUtil.GetTableConfig(key), paramArray);
            DataTable dt = DbHelperOra.Query(sql).Tables[0];
            JArray ja = JArray.FromObject(dt);
            return ja;
        }

        public int Execute(String key, params String[] paramArray)
        {
            //String sql = String.Format(ApiUtil.GetConfigCont(key), paramArray);
            String sql = String.Format(ApiUtil.GetTableConfig(key), paramArray);
            int result = -1;
            try
            {
                result = DbHelperOra.ExecuteSql(sql);
            }
            catch (Exception e)
            {
                this.errorDetail = e.Message;
            }
            return result;
        }

        public Boolean BulkExecute(String key, params String[] paramArray)
        {
            //JArray result = new JArray();
            JArray ja = JArray.Parse(paramArray[2]);
            List<CommandInfo> lci = new List<CommandInfo>();
            foreach (JToken jt in ja)
            {
                //String sql = String.Format(ApiUtil.GetConfigCont(key), JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt)));
                String sql = String.Format(ApiUtil.GetTableConfig(key), JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt)));
                CommandInfo ci = new CommandInfo(sql, null);
                lci.Add(ci);
            }
            try
            {
                Boolean result = DbHelperOra.ExecuteSqlTran(lci);
                return result;
            }
            catch (Exception e)
            {
                this.errorDetail = e.Message;
                return false;
            }
            //TODO RETURN 
        }

        public Boolean BulkExecuteBill(String key, params String[] paramArray)
        {

            Boolean result = false;
            JObject jo = JObject.Parse(paramArray[2]);
            JArray heads = jo.Value<JArray>("heads");
            JArray bodies = jo.Value<JArray>("bodies");
            JArray exts = jo.Value<JArray>("exts");
            List<CommandInfo> lci = new List<CommandInfo>();
            String bodyName = key.Split('_')[0] + "_" + key.Split('_')[1] + "_COM";
            String extName = key.Split('_')[0] + "_" + key.Split('_')[1] + "_EXT";
            foreach (JToken jt in heads)
            {
                String custid = JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt))[0];
                String chkExistSql = "select count(1) from dat_" + key.Split('_')[1] + "_doc where seqno = '" + JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt))[2] + "' and custid='" + custid + "'";
                if ("DAT_DDPUR_DOC".Equals(key)) 
                {
                    chkExistSql = "select count(1) from dat_" + key.Split('_')[1] + "_doc where seqno = '" + JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt))[2] + "' and wareHouse='" + custid + "'";
                }
                if (!DbHelperOra.Exists(chkExistSql))
                {
                    String[] tempArray = JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt));
                    //String sql = String.Format(ApiUtil.GetConfigCont(key), JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt)));

                    String sql = String.Format(ApiUtil.GetTableConfig(key), tempArray);
                    CommandInfo ci = new CommandInfo(sql, null);
                    lci.Add(ci);
                }
                else if ("DAT_DD_DOC".Equals(key))
                {  //订单重复上传则 更新订单完结状态
                    String[] paraTemp = JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt));
                    String sql = String.Format("update dat_dd_doc set isend = '{6}',flag='{5}' where seqno = '{2}' and custId = '{0}'", paraTemp);
                    CommandInfo ci = new CommandInfo(sql, null);
                    lci.Add(ci);
                }


            }
            foreach (JToken jt in bodies)
            {
                String chkExistSql = "select count(1) from dat_" + key.Split('_')[1] + "_com t where seqno = '" + JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt))[2] + "' and custid = '" + JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt))[0] + "' ";
                if ("DAT_DDPUR_DOC".Equals(key))
                {
                    chkExistSql = "select count(1) from dat_" + key.Split('_')[1] + "_com where seqno = '" + JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt))[2] + "' and wareHouse='" + JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt))[0] + "'";
                }
                if (!DbHelperOra.Exists(chkExistSql))
                {
                    String[] tempArray = JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt));
                    String sql = "";
                    if ("DAT_DD_DOC".Equals(key) && tempArray.Length < 33)
                    {
                        String[] newArray = new String[33];
                        for (int i = 0; i < tempArray.Length; i++)
                        {
                            newArray[i] = tempArray[i];
                        }
                        newArray[31] = "N";
                        sql = String.Format(ApiUtil.GetTableConfig(bodyName), newArray);
                    }
                    else
                    {
                        sql = String.Format(ApiUtil.GetTableConfig(bodyName), tempArray);
                    }

                    //String sql = String.Format(ApiUtil.GetConfigCont(bodyName), JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt)));

                    CommandInfo ci = new CommandInfo(sql, null);
                    lci.Add(ci);
                }
            }
            //当有附表（即唯一码信息） By c 20150416
            if(exts!=null && exts.Count > 0)
            {
                foreach (JToken jt in exts)
                {
                    string chkExistSql = "select count(1) from dat_" + key.Split('_')[1] + "_ext t where BILLNO = '" + JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject(jt))[3] + "' and custid = '" + JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject(jt))[0] + "' and ROWNO='" + JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject(jt))[4] + "' and ONECODE='" + JsonConvert.DeserializeObject<string[]>(JsonConvert.SerializeObject(jt))[5] + "'";
                    if (!DbHelperOra.Exists(chkExistSql))
                    {
                        String sql = String.Format(ApiUtil.GetTableConfig(extName), JsonConvert.DeserializeObject<String[]>(JsonConvert.SerializeObject(jt)));

                        CommandInfo ci = new CommandInfo(sql, null);
                        lci.Add(ci);
                    }
                }
            }
            try
            {
                result = DbHelperOra.ExecuteSqlTran(lci);

            }
            catch (Exception e)
            {
                this.errorDetail = e.Message;
            }
            return result;
        }

        private JArray bodies;
        private JArray billexts;
        /// <summary>
        /// 订单查询方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public JObject QueryBill(String key, params String[] paramArray)
        {

            //String sql = String.Format(ApiUtil.GetConfigCont(key), paramArray);
            String sql = String.Format(ApiUtil.GetTableConfig(key), paramArray);
            DataTable dt = DbHelperOra.Query(sql).Tables[0];
            JObject jo = new JObject();

            if (dt.Rows.Count > 0)
            {
                String type = key.Split('_')[1];
                JArray heads = JArray.FromObject(dt);
                jo.Add("heads", heads);
                bodies = new JArray();
                billexts = new JArray();
                foreach (DataRow dr in dt.Rows)
                {
                    String seqno = dr["SEQNO"].ToString();
                    String custId = dr["CUSTID"].ToString();
                    getBillBody(type, seqno, custId);
                }
                jo.Add("bodies", bodies);
                jo.Add("billexts", billexts);
            }

            return jo;
        }

        /// <summary>
        /// 查询单据的表体
        /// </summary>
        /// <param name="type">单据类型</param>
        /// <param name="seqno"></param>
        /// <returns></returns>
        private void getBillBody(String type, String seqno, String custId)
        {
            JArray ja = new JArray();
            String sql = "select * from dat_" + type + "_com where seqno = '" + seqno + "' and custId = '" + custId + "'";
            if ("RK".Equals(type.ToUpper())) {
                sql = @"select SEQNO,ROWNO,DEPTID,GDSEQ,BARCODE,GDNAME,UNIT,GDSPEC,GDMODE,CDID,SPLB,CATID,HWID,BZHL,BZSL,DDSL,SSSL,JXTAX,HSJJ,BHSJJ,HSJE,BHSJE,
                               LSJ,LSJE,ISGZ,ISLOT,PHID,PH,PZWH,RQ_SC,YXQZ,KCSL,KCHSJE,SPZTSL,ERPAYXS,HLKC,ZPBH,STR1,STR2,STR3,NUM1,NUM2,NUM3,MEMO,CUSTID  
                          from dat_rk_com where seqno = '" + seqno + "' and custId ='" + custId + "'";
            }
            DataTable dt = DbHelperOra.Query(sql).Tables[0];

            ja = JArray.FromObject(dt);
            foreach (JToken jt in ja)
            {
                this.bodies.Add(jt);
            }
            //添加附表信息 c 20140517
            JArray jaExt = new JArray();
            string sqlExt = "select * from dat_" + type + "_ext where billno = '" + seqno + "' and custId = '" + custId + "'";
            DataTable dtExt = DbHelperOra.Query(sqlExt).Tables[0];
            if(dtExt !=null && dtExt.Rows.Count > 0)
            {
                jaExt = JArray.FromObject(dtExt);
                foreach (JToken jt in jaExt)
                {
                    this.billexts.Add(jt);
                }
            }

            //foreach (DataRow dr in dt.Rows)
            //{
            //    JObject jo = new JObject();
            //    jo = JObject.FromObject(dr);
            //    this.bodies.Add(jo.Value<JArray>("Table")[0]);
            //}
        }

        #region 保存证照图片
        /// <summary>
        /// 保存证照图片
        /// </summary>
        /// <param name="key"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public Boolean SaveCertPic(String key, byte[] bytes, params String[] paramArray)
        {
            try
            {
                String rootPath = ApiUtil.GetConfigCont("UPLOADDIR");//AppDomain.CurrentDomain.BaseDirectory;
                String containPath = "CertPic";
                String monthPath = "\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd");
                String fileName = paramArray[2] + "_" + DateTime.Now.ToString("ddHHmmssfff") + ".jpg";
                String path = rootPath + containPath + monthPath;
                String httpPath = containPath + monthPath;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                System.IO.Stream stream = new System.IO.MemoryStream(bytes);
                System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                image.Save(path + "\\" + fileName, System.Drawing.Imaging.ImageFormat.Jpeg);

                String sql = String.Format(ApiUtil.GetTableConfig(key), paramArray[2], "~/" + ("ERPUpload\\" + httpPath + "\\" + fileName).Replace("\\", "/"));
                int count = DbHelperOra.ExecuteSql(sql);
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    this.errorDetail = "没找到对应的记录[" + paramArray[2] + "]";
                    return false;
                }

            }
            catch (Exception ex)
            {
                this.errorDetail = ex.Message;
                return false;
            }
        }
        #endregion

        #region 保存商品图片
        /// <summary>
        /// 保存证照图片
        /// </summary>
        /// <param name="key"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>

        public String SaveGoodsPic(String key, byte[] bytes, params String[] paramArray)
        {
            //string seqn = "1";//DateTime.Now.ToString("ddHHmmssfff")
            try
            {
                String rootPath = ApiUtil.GetConfigCont("UPLOADDIR");//AppDomain.CurrentDomain.BaseDirectory;
                String containPath = "GoodsPic";
                String monthPath = "\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd");
                String fileName = paramArray[2] + "_" + DateTime.Now.ToString("ddHHmmssfff") + ".jpg";
                String path = rootPath + containPath + monthPath;
                String httpPath = containPath + monthPath;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                System.IO.Stream stream = new System.IO.MemoryStream(bytes);
                System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
                image.Save(path + "\\" + fileName, System.Drawing.Imaging.ImageFormat.Jpeg);

                return ("ERPUpload\\" + httpPath + "\\" + fileName).Replace("\\", "/");
                //String sql = String.Format(ApiUtil.GetTableConfig(key), paramArray[2], "~/" + ("ERPUpload\\" + httpPath + "\\" + fileName).Replace("\\", "/"));
                //int count = DbHelperOra.ExecuteSql(sql);
                //if (count > 0)
                //{
                //    return true;
                //}
                //else
                //{
                //    this.errorDetail = "没找到对应的记录[" + paramArray[2] + "]";
                //    return false;
                //}

            }
            catch (Exception ex)
            {
                this.errorDetail = ex.Message;
                return "没有路径！";
            }
        }
        #endregion


        /// <summary>
        /// 供应商单据查询方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public JObject QuerySupBill(String key, params String[] paramArray)
        {
            String sql = String.Format(ApiUtil.GetTableConfig(key), paramArray);
            DataTable dt = DbHelperOra.Query(sql).Tables[0];
            JObject jo = new JObject();
            if (dt.Rows.Count > 0)
            {
                String type = key.Split('_')[1];
                JArray heads = JArray.FromObject(dt);
                jo.Add("heads", heads);
                bodies = new JArray();
                foreach (DataRow dr in dt.Rows)
                {
                    String seqno = dr["SEQNO"].ToString();
                    String custId = dr["CUSTID"].ToString();
                    String supId = dr["SUPID"].ToString();
                    getSupBillBody(type, seqno, custId, supId);
                }
                jo.Add("bodies", bodies);
            }
            return jo;
        }

        public String QueryEASInventory(String warehouse, String materials) {
            return ApiService.getInventory(warehouse, materials);
        }

        /// <summary>
        /// 查询供应商单据的表体
        /// </summary>
        /// <param name="type">单据类型</param>
        /// <param name="seqno"></param>
        /// <returns></returns>
        private void getSupBillBody(String type, String seqno, String custId, String supId)
        {
            JArray ja = new JArray();
            String sql = "select * from sup_" + type + "_com where seqno = '" + seqno + "' and custId = '" + custId + "' and supId = '" + supId + "'";
            DataTable dt = DbHelperOra.Query(sql).Tables[0];

            ja = JArray.FromObject(dt);
            foreach (JToken jt in ja)
            {
                this.bodies.Add(jt);
            }
        }

        /// <summary>
        /// EAS 调拨单查询方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public String QueryEASDHDBill(String[] paramArray)
        {
            return ApiService.getEASDHDBILL(paramArray);
        }

        /// <summary>
        /// EAS 调拨入库单查询方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public String QueryEASDRKBill(String[] paramArray)
        {
            return ApiService.getEASDRKBILL(paramArray);
        }
    }

    public class UseSession : System.Web.SessionState.IRequiresSessionState
    {
        static public int GetSessionCount(System.Web.HttpContext context)
        {
            return context.Session.Count;
            // 说明：如果不继承IRequiresSessionState接口的话，此时会抛出异常。  
        }

        static public void SetTimeout(System.Web.HttpContext context, int timeout)
        {
            context.Session.Timeout = 120;
        }

        static public void SetSession(System.Web.HttpContext context, String key, object value)
        {
            context.Session[key] = value;
        }

        static public object GetSession(System.Web.HttpContext context, String key)
        {
            return context.Session[key];
        }
    }

}
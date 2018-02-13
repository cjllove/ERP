using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using XTBase;
using XTBase.Utilities;
using System.Collections;
using System.Data;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Text;

/*
*  编 写 人：鞠怡
*  编写时间：2014-10-07
*  功能说明：封装接口调用操作
*/
namespace ERPProject
{
    public class ApiClientUtil
    {

        #region //--------------字段转换逻辑，暂不使用，在数据库里转换-------------
        //public static Hashtable GoodsColNameTable = new Hashtable();
        //public static Hashtable UnitsColNameTable = new Hashtable();
        //public static Hashtable CategoryColNameTable = new Hashtable();


        //public ApiUtil()
        //{

        //}

        //static ApiUtil()
        //{

        //    /////商品列名字典  ERP,EAS
        //    //GoodsColNameTable.Add("GDSEQ", "SYO_SEINO");//商品编码 +GDID
        //    //GoodsColNameTable.Add("GDNAME", "SYO_NAME"); //商品名称
        //    //GoodsColNameTable.Add("GDSPEC", "SYO_KKK"); //规格型号
        //    ////GoodsColNameTable.Add("??", "BZHL");//缺省单位包装含量
        //    //GoodsColNameTable.Add("UNIT", "SYO_NYTNICD");//缺省单位
        //    ////GoodsColNameTable.Add("", "SUPPLIER");//供应商 威高？
        //    ////GoodsColNameTable.Add("FACTORY", "PRODUCER");//制造商 威高？
        //    //GoodsColNameTable.Add("JXTAX", "SALETAX");//进项税率 //TODO需要转换
        //    ////GoodsColNameTable.Add("CPDL", "CATID0");//产品大类
        //    //GoodsColNameTable.Add("CATID", "SYO_SKBCD");//基本分类编码
        //    //GoodsColNameTable.Add("UNIT_ZHONGBZ", "ZBZDW");//中包装计量单位编码
        //    //GoodsColNameTable.Add("NUM_ZHONGBZ", "ZBZXS");//中包装系数
        //    //GoodsColNameTable.Add("NUM_DABZ", "DBZXS");//大包装系数
        //    //GoodsColNameTable.Add("PIZNO", "SYO_SYONIN");//注册证号

        //    /////单位列名字典
        //    //UnitsColNameTable.Add("CODE", "FNUMBER"); //单位编码
        //    //UnitsColNameTable.Add("NAME", "FNAME"); //单位名称
        //    //UnitsColNameTable.Add("FLAG", "FISDISABLED"); //TODO 需要转换 状态

        //    /////分类列名字典
        //    //CategoryColNameTable.Add("CODE", "FNUMBER");
        //    //CategoryColNameTable.Add("NAME", "FNAME");
        //    //CategoryColNameTable.Add("CLASS", "FLEVEL");
        //    //CategoryColNameTable.Add("SJCODE", "FPARENTNUM");

        //}
        #endregion

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
                Console.Write(e.StackTrace);
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
                        columnValues += "TO_DATE('" + dr[colName].ToString() + "','yyyy/mm/dd hh24:mi:ss'),";
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
        public static String GetConfigCont(String key)
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

        #region 下传图片并保存
        public static String DownloadSavePic(String picUrl)
        {
            string imgpath = "";
            if (!String.IsNullOrEmpty(picUrl))
            {
                imgpath = picUrl;
            }
            else
            {
                return "ERROR";
            }
            imgpath = GetConfigCont("PIC_PATH") + imgpath + "?id=";
            Random seed = new Random();
            WebRequest webreq = WebRequest.Create(imgpath + seed.NextDouble());
            WebResponse webres = webreq.GetResponse();
            Stream stream = webres.GetResponseStream();
            System.Drawing.Image image;
            image = System.Drawing.Image.FromStream(stream);
            stream.Close();
            System.IO.MemoryStream _ImageMem = new System.IO.MemoryStream();
            image.Save(_ImageMem, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] imgBytes = _ImageMem.ToArray();
            _ImageMem.Close();
            if (!String.IsNullOrEmpty(GetConfigCont("PICWEBSERVICEURL")))
            {
                imgpath = ApiClientService.execPicUpload(imgBytes);
            }
            else
            {
                imgpath = SavePic(imgBytes);
            }
            return imgpath;
        }
        #endregion

        #region 下传图片并保存
        public static String SavePic(byte[] imgBytes)
        {
            String rootPath = ApiUtil.GetConfigCont("DOWNDIR");
            String containPath = "GoodsPic";
            String monthPath = "\\" + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd");
            String fileName = DateTime.Now.ToString("ddHHmmssfff") + ".jpg";
            String path = rootPath + containPath + monthPath;
            String httpPath = containPath + monthPath + "\\" + fileName;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            System.IO.Stream stream = new System.IO.MemoryStream(imgBytes);
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
            image.Save(path + "\\" + fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            return httpPath;
        }
        #endregion

    }

    public class ApiClientService
    {
        private static String loginUuid;
        private static String username;
        private static String password;
        private static String token;
        private static String webServiceURL;
        private static Boolean useDynamicURL;

        static ApiClientService()
        {
            String strUseDynamicURL = ApiClientUtil.GetConfigCont("USEDYNAMICURL");
            if (!ApiClientUtil.isNull(strUseDynamicURL))
            {
                useDynamicURL = "1".Equals(strUseDynamicURL);
            }
            else
            {
                useDynamicURL = false;
            }

            billJa = new JArray();
        }

        public static List<KeyValuePair<String, String>> getLoginInfo()
        {
            List<KeyValuePair<String, String>> list = new List<KeyValuePair<string, string>>();
            getCustomerInfo();
            getWebServiceURL();
            list.Add(new KeyValuePair<string, string>("webServiceURL", webServiceURL));
            list.Add(new KeyValuePair<string, string>("USERNAME", username));
            list.Add(new KeyValuePair<string, string>("PASSWORD", password));
            list.Add(new KeyValuePair<string, string>("TOKEN", token));
            list.Add(new KeyValuePair<string, string>("useDynamicURL", useDynamicURL.ToString()));
            return list;
        }

        public static Boolean testLogin()
        {
            return login();
        }

        #region 取用户信息
        ///不再使用Web.Config中的用户信息
        //static ApiClientService()
        //{
        //    username = ApiClientUtil.GetConfigCont("CUSTOMER_CODE");
        //    password = ApiClientUtil.GetConfigCont("CUSTOMER_PASSWD");
        //    token = ApiClientUtil.GetConfigCont("CUSTOMER_TOKEN");
        //}

        /// <summary>
        /// 取用户信息，缓存 ->静态变量
        /// 缓存中没有，数据库 ->静态变量->缓存
        /// 如需更改用户信息设置，需要在更改之后清空缓存
        /// </summary>
        /// <returns></returns>
        private static Boolean getCustomerInfo()
        {
            Hashtable ht = (Hashtable)CacheHelper.GetCache("customerUserInfo");
            if (ht != null && ht.ContainsKey("username") && ht.ContainsKey("password") && ht.ContainsKey("token"))
            {
                username = ht["username"].ToString();
                password = ht["password"].ToString();
                token = ht["token"].ToString();
            }
            if (username == null)
            {
                DataTable dt = new DataTable();
                String sql = "select code,value,str1,str2 from sys_para where code = 'USERCODE'";
                dt = DbHelperOra.Query(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    username = dt.Rows[0]["value"].ToString();
                    password = dt.Rows[0]["str1"].ToString();
                    token = dt.Rows[0]["str2"].ToString();
                    CacheHelper.RemoveOneCache("customerUserInfo");
                    ht = new Hashtable();
                    ht.Clear();
                    ht.Add("username", username);
                    ht.Add("password", password);
                    ht.Add("token", token);
                    CacheHelper.SetCache("customerUserInfo", ht, TimeSpan.FromHours(2));
                }
                //username = "37.11.0169";
                return true;
            }
            else
            {
                //username = "37.11.0169";
                return false;
            }
            
        }
        #endregion

        #region 取webService URL
        private static Boolean getWebServiceURL()
        {
            String url = Convert.ToString(CacheHelper.GetCache("webServiceURL"));
            if (!ApiClientUtil.isNull(url))
            {
                webServiceURL = url;
                //webServiceURL = "http://localhost:2732/WebService/PlatformWebService.asmx";
            }
            if (ApiClientUtil.isNull(webServiceURL))
            {
                DataTable dt = new DataTable();
                String sql = "select code,value from sys_para where code = 'WEBSERVICEURL'";
                dt = DbHelperOra.Query(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    webServiceURL = Convert.ToString(dt.Rows[0]["value"]);
                    //webServiceURL = "http://localhost:2732/WebService/PlatformWebService.asmx";
                    CacheHelper.RemoveOneCache("webServiceURL");
                    CacheHelper.SetCache("webServiceURL", webServiceURL, TimeSpan.FromHours(2));
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string getWebServiceURL(string key)
        {
            String url = Convert.ToString(CacheHelper.GetCache("webServiceURL"+key));
            if (ApiClientUtil.isNull(url))
            {
                DataTable dt = new DataTable();
                String sql = "select webserviceurl from dat_infdata_def where inftype='{0}'";
                dt = DbHelperOra.Query(string.Format(sql,key)).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    url = Convert.ToString(dt.Rows[0]["webserviceurl"]);
                    CacheHelper.RemoveOneCache("webServiceURL" + key);
                    CacheHelper.SetCache("webServiceURL" + key, url, TimeSpan.FromHours(2));
                }
                else
                {
                    sql = "select value from sys_para where code = 'WEBSERVICEURL'";
                    url = (String)DbHelperOra.GetSingle(sql);
                    CacheHelper.RemoveOneCache("webServiceURL" + key);
                    CacheHelper.SetCache("webServiceURL" + key, url, TimeSpan.FromHours(2));
                }
            }
            //url = "http://localhost:2732/WebService/PlatformWebService.asmx";
            //url = "http://jkpt.yutuyun.com.cn/webService/PlatformWebService.asmx";
            return url;
        }
        #endregion

        #region login
        private static Boolean login()
        {
            JObject jo = new JObject();
            try
            {
                String result = "";
                getCustomerInfo();
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    //webServiceURL = "http://localhost:3789/WeiGo/WebService/PlatformWebService.asmx";
                    //webServiceURL = "http://localhost:2732/WebService/PlatformWebService.asmx";
                    //图片接口
                    //webServiceURL = "http://localhost:3899/WeiGo/WebService/PlatformWebService.asmx";
                    result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, "Login", new object[] { username, password, token }));
                }
                else
                {
                    //YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                    //result = pws.Login(username, password, token);
                }

                jo = JsonConvert.DeserializeObject<JObject>(result);
                String strResult = jo["result"].ToString();
                if (strResult.Equals("fail"))
                {
                    return false;
                }
                else
                {
                    ApiClientService.loginUuid = jo["data"].ToString();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static Boolean login(string key)
        {
            JObject jo = new JObject();
            try
            {
                String result = "";
                getCustomerInfo();
                if (useDynamicURL)
                {
                    result = Convert.ToString(DynamicServiceBind.InvokeWebService(getWebServiceURL(key), "Login", new object[] { username, password, token }));
                }
                else
                {
                    //YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                    //result = pws.Login(username, password, token);
                }

                jo = JsonConvert.DeserializeObject<JObject>(result);
                String strResult = jo["result"].ToString();
                if (strResult.Equals("fail"))
                {
                    return false;
                }
                else
                {
                    ApiClientService.loginUuid = jo["data"].ToString();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region logout
        private static Boolean logout()
        {

            JObject jo = new JObject();
            try
            {
                getCustomerInfo();
                String result = "";
                if (useDynamicURL)
                {
                    getWebServiceURL();
                    result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, "Logout", new object[] { username, password, token }));
                }
                else
                {
                    //YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                    //result = pws.Logout(username, password, token);
                }

                jo = JsonConvert.DeserializeObject<JObject>(result);
                String strResult = jo["result"].ToString();
                if (strResult.Equals("fail"))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private static Boolean logout(string key)
        {

            JObject jo = new JObject();
            try
            {
                getCustomerInfo();
                String result = "";
                if (useDynamicURL)
                {
                    result = Convert.ToString(DynamicServiceBind.InvokeWebService(getWebServiceURL(key), "Logout", new object[] { username, password, token }));
                }
                else
                {
                    //YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                    //result = pws.Logout(username, password, token);
                }

                jo = JsonConvert.DeserializeObject<JObject>(result);
                String strResult = jo["result"].ToString();
                if (strResult.Equals("fail"))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion



        #region 执行接口通用方法
        /// <summary>
        /// 执行接口通用方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public static JObject query(String key, params String[] paramArray)
        {
            JObject jo = new JObject();
            try
            {
                if (login(key))
                {
                    String result = "";
                    String[] paramResult;
                    if (paramArray != null)
                    {
                        paramResult = new String[paramArray.Length + 2];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                        for (int i = 2; i < paramArray.Length + 2; i++)
                        {
                            paramResult[i] = paramArray[i - 2];
                        }
                    }
                    else
                    {
                        paramResult = new String[2];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                    }
                    if (useDynamicURL)
                    {
                        result = Convert.ToString(DynamicServiceBind.InvokeWebService(getWebServiceURL(key), "CommonApi", new object[] { key, paramResult }));
                    }
                    else
                    {
                        //YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                        //result = pws.CommonApi(key, paramResult);
                    }
                    logout(key);
                    return JsonConvert.DeserializeObject<JObject>(result);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion


        #region 获取数据中心证照图片地址
        public static byte[] getUploadPic(String key, params String[] paramArray)
        {
            JObject jo = new JObject();
            try
            {
                if (login())
                {
                    byte[] result = null;
                    String[] paramResult;
                    if (paramArray != null)
                    {
                        paramResult = new String[paramArray.Length + 2];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                        for (int i = 2; i < paramArray.Length + 2; i++)
                        {
                            paramResult[i] = paramArray[i - 2];
                        }
                    }
                    else
                    {
                        paramResult = new String[2];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                    }
                    if (useDynamicURL)
                    {
                        getWebServiceURL();
                        //webServiceURL = "http://localhost:3791/WeiGo/WebService/PlatformWebService.asmx";
                        result = (byte[])(DynamicServiceBind.InvokeWebService(webServiceURL, "getCertPicUpload", new object[] { key, paramResult }));
                    }
                    logout();
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion 

        #region 获取数据中心商品图片地址
        public static byte[] getUploadgoodsPic(String key, params String[] paramArray)
        {
            JObject jo = new JObject();
            try
            {
                if (login())
                {
                    byte[] result = null;
                    String[] paramResult;
                    if (paramArray != null)
                    {
                        paramResult = new String[paramArray.Length + 2];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                        for (int i = 2; i < paramArray.Length + 2; i++)
                        {
                            paramResult[i] = paramArray[i - 2];
                        }
                    }
                    else
                    {
                        paramResult = new String[2];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                    }
                    if (useDynamicURL)
                    {
                        getWebServiceURL();
                        //webServiceURL = "http://localhost:3791/WeiGo/WebService/PlatformWebService.asmx";
                        result = (byte[])(DynamicServiceBind.InvokeWebService(webServiceURL, "getGoodsPicUpload", new object[] { key, paramResult }));
                    }
                    logout();
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static byte[][] getGoodsPics(String key, params String[] paramArray)
        {
            JObject jo = new JObject();
            try
            {
                if (login())
                {
                    byte[][] result = null;
                    String[] paramResult;
                    if (paramArray != null)
                    {
                        paramResult = new String[paramArray.Length + 2];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                        for (int i = 2; i < paramArray.Length + 2; i++)
                        {
                            paramResult[i] = paramArray[i - 2];
                        }
                    }
                    else
                    {
                        paramResult = new String[2];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                    }
                    if (useDynamicURL)
                    {
                        getWebServiceURL();
                        //图片
                        //webServiceURL = "http://localhost:3899/WeiGo/WebService/PlatformWebService.asmx";
                        result = (byte[][])(DynamicServiceBind.InvokeWebService(webServiceURL, "getGoodsPics", new object[] { key, paramResult }));
                    }
                    logout();
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion 


        #region 执行订单接口通用方法
        /// <summary>
        /// 执行接口通用方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public static JObject queryBill(String key, params String[] paramArray)
        {
            JObject jo = new JObject();
            try
            {
                if (login(key))
                {
                    String[] paramResult;
                    if (paramArray != null)
                    {
                        paramResult = new String[paramArray.Length + 2];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                        for (int i = 2; i < paramArray.Length + 2; i++)
                        {
                            paramResult[i] = paramArray[i - 2];
                        }
                    }
                    else
                    {
                        paramResult = new String[2];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                    }
                    String result = "";
                    if (useDynamicURL)
                    {
                        result = Convert.ToString(DynamicServiceBind.InvokeWebService(getWebServiceURL(key), "CommonBillApi", new object[] { key, paramResult }));
                    }
                    else
                    {
                        //YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                        //result = pws.CommonBillApi(key, paramResult);
                    }
                    logout(key);
                    return JsonConvert.DeserializeObject<JObject>(result);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 执行接口插入更新等通用方法
        /// <summary>
        /// 执行接口插入等通用方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public static JObject exec(String key, params String[] paramArray)
        {
            JObject jo = new JObject();
            try
            {
                if (login(key))
                {
                    String result = "";
                    String[] paramResult;
                    if (paramArray != null)
                    {
                        paramResult = new String[paramArray.Length + 2];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                        for (int i = 2; i < paramArray.Length + 2; i++)
                        {
                            paramResult[i] = paramArray[i - 2];
                        }
                    }
                    else
                    {
                        paramResult = new String[2];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                    }
                    if (useDynamicURL)
                    {
                        result = Convert.ToString(DynamicServiceBind.InvokeWebService(getWebServiceURL(key), "CommonExecApi", new object[] { key, paramResult }));
                    }
                    else
                    {
                        //YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                        //result = pws.CommonExecApi(key, paramResult);
                    }
                    logout(key);
                    return JsonConvert.DeserializeObject<JObject>(result);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static JObject execBulk(String key)
        {
            JObject jo = new JObject();
            try
            {
                if (login())
                {
                    String result = "";
                    if (billJa.Count > 0)
                    {
                        String[] paramResult = new String[3];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                        paramResult[2] = JsonConvert.SerializeObject(billJa);
                        if (useDynamicURL)
                        {
                            getWebServiceURL();
                            //webServiceURL = "http://localhost:3794/WeiGo/WebService/PlatformWebService.asmx";
                            result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, "BulkExecApi", new object[] { key, paramResult }));
                        }
                        else
                        {
                            //YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                            //result = pws.BulkExecApi(key, paramResult);
                        }
                        jo = JObject.Parse(result);
                    }
                    return jo;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static JObject execBulk(String key,JArray ja)
        {
            JObject jo = new JObject();
            try
            {
                if (login(key))
                {
                    String result = "";
                    if (ja.Count > 0)
                    {
                        String[] paramResult = new String[3];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                        paramResult[2] = JsonConvert.SerializeObject(ja);
                        if (useDynamicURL)
                        {
                            result = Convert.ToString(DynamicServiceBind.InvokeWebService(getWebServiceURL(key), "BulkExecApi", new object[] { key, paramResult }));
                        }
                        else
                        {
                            //YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                            //result = pws.BulkExecApi(key, paramResult);
                        }
                        jo = JObject.Parse(result);
                    }
                    return jo;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static JObject execRenderedBill(String key)
        {
            JObject jo = new JObject();
            try
            {
                if (login(key))
                {
                    String result = "";
                    JObject paramJo = new JObject();
                    if (billJa.Count > 0)
                    {
                        paramJo.Add("heads", billJa);
                        paramJo.Add("bodies", billBodyJa);
                        //当单据有附表(即有唯一码信息)时  c 20150416
                        if (billExtJa != null && billExtJa.Count > 0)
                        {
                            paramJo.Add("exts", billExtJa);
                        }
                        String[] paramResult = new String[3];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                        paramResult[2] = JsonConvert.SerializeObject(paramJo);
                        if (useDynamicURL)
                        {
                            result = Convert.ToString(DynamicServiceBind.InvokeWebService(getWebServiceURL(key), "BulkExecBillApi", new object[] { key, paramResult }));
                        }
                        else
                        {
                            //YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                            //result = pws.BulkExecBillApi(key, paramResult);
                        }
                        jo = JObject.Parse(result);
                    }
                    return jo;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static JObject execRenderedBill(String key, JArray ja, JArray jaBody, JArray jaExt)
        {
            JObject jo = new JObject();
            try
            {
                if (login(key))
                {
                    String result = "";
                    JObject paramJo = new JObject();
                    if (ja.Count > 0)
                    {
                        paramJo.Add("heads", ja);
                        paramJo.Add("bodies", jaBody);
                        //当单据有附表(即有唯一码信息)时  c 20150416
                        if (jaExt != null && jaExt.Count > 0)
                        {
                            paramJo.Add("exts", jaExt);
                        }
                        String[] paramResult = new String[3];
                        paramResult[0] = username;
                        paramResult[1] = loginUuid;
                        paramResult[2] = JsonConvert.SerializeObject(paramJo);
                        if (useDynamicURL)
                        {
                            result = Convert.ToString(DynamicServiceBind.InvokeWebService(getWebServiceURL(key), "BulkExecBillApi", new object[] { key, paramResult }));
                        }
                        else
                        {
                            //YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                            //result = pws.BulkExecBillApi(key, paramResult);
                        }
                        jo = JObject.Parse(result);
                    }
                    return jo;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static JArray billJa { get; set; }
        public static JArray billBodyJa { get; set; }
        public static JArray billExtJa { get; set; }

        public static String[] renderParams(params String[] paramArray)
        {
            getCustomerInfo();
            String[] paramResult;
            if (paramArray != null)
            {
                paramResult = new String[paramArray.Length + 2];
                paramResult[0] = username;
                paramResult[1] = loginUuid;
                for (int i = 2; i < paramArray.Length + 2; i++)
                {
                    paramResult[i] = paramArray[i - 2];
                }
            }
            else
            {
                paramResult = new String[2];
                paramResult[0] = username;
                paramResult[1] = loginUuid;
            }
            return paramResult;
        }

        public static void renderBillParams(params String[] paramArray)
        {
            billJa.Add(JArray.FromObject(renderParams(paramArray)));
        }

        public static void renderBillBodyParams(params String[] paramArray)
        {
            billBodyJa.Add(JArray.FromObject(renderParams(paramArray)));
        }

        public static void renderBillExtParams(params String[] paramArray)
        {
            billExtJa.Add(JArray.FromObject(renderParams(paramArray)));
        }


        #endregion

        #region 库存查询接口
        public static JObject queryInventory(String warehouse, String materials)
        {
            JObject jo = new JObject();
            try
            {
                if (login())
                {
                    String result = "";
                    if (useDynamicURL)
                    {
                        getWebServiceURL();
                        result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, "QueryInventory", new object[] { username, loginUuid, warehouse, materials }));
                    }
                    //else
                    //{
                    //    YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                    //    result = pws.QueryInventory(username, loginUuid, warehouse, materials);
                    //}
                    logout();
                    return JsonConvert.DeserializeObject<JObject>(result);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 库存分页查询接口
        public static JObject queryInventoryByPage(String warehouse, int pagenum, int pageindex)
        {
            JObject jo = new JObject();
            try
            {
                if (login("D_INVENTORY"))
                {
                    String result = "";
                    if (useDynamicURL)
                    {
                        result = Convert.ToString(DynamicServiceBind.InvokeWebService(getWebServiceURL("D_INVENTORY"), "QueryInventoryByPage", new object[] { username, loginUuid, warehouse, pagenum, pageindex }));
                    }
                    //else
                    //{
                    //    YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                    //    result = pws.QueryInventory(username, loginUuid, warehouse, materials);
                    //}
                    logout("D_INVENTORY");
                    return JsonConvert.DeserializeObject<JObject>(result);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 执行EAS单据查询接口方法
        /// <summary>
        /// 执行EAS单据查询接口方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="billno"></param>
        /// <returns></returns>
        public static String QueryEASBill(String type, String[] paramArray)
        {
            try
            {
                if (login())
                {
                    String result = "";
                    if (useDynamicURL)
                    {
                        getWebServiceURL();
                        #region 测试用
                        //webServiceURL = "http://localhost:3799/WeiGo/WebService/PlatformWebService.asmx";
                        //webServiceURL = "http://localhost/WeiGo/WebService/PlatformWebService.asmx";
                        //测试用
                        //webServiceURL = "http://localhost:3787/WeiGo/WebService/PlatformWebService.asmx";

                        #endregion
                        result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, "QueryEASBillApi", new object[] { username, loginUuid, type, paramArray }));
                    }
                    else
                    {
                        //YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                        //result = pws.QueryBillStatus(billno, custid, loginUuid);
                    }
                    logout();
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 执行图片文件上传ERPwebService
        public static string execPicUpload(byte[] bytes)
        {
            string result = "";
            try
            {
                result = Convert.ToString(DynamicServiceBind.InvokeWebService(ApiClientUtil.GetConfigCont("PICWEBSERVICEURL"), "GoodsPicUploadRetUrl", new object[] { bytes }));
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }
        #endregion

        #region 发送短信方法
        /// <summary>
        /// 发送短信方法
        /// </summary>
        public static string SendMessage(String sendDoc, String getTel, String isTiming, String TimingSj, String sendUser, String sendCust, String Class)
        {
            string Name = ApiClientUtil.GetConfigCont("MESSAGE_NAME");//账户名
            string Psw = ApiClientUtil.GetConfigCont("MESSAGE_PWD");//密码
            string webServiceURL = ApiClientUtil.GetConfigCont("MESSAGE_WEBSERVICEURL");//webService地址
            string USEDYNAMICURL = ApiClientUtil.GetConfigCont("USEDYNAMICURL");//是否动态调用WebService
            string result = null;
            try
            {
                if ("1".Equals(USEDYNAMICURL))
                {
                    result = Convert.ToString(DynamicServiceBind.InvokeWebService(webServiceURL, "SendMessage", new object[] { Name, Psw, sendDoc, getTel, isTiming, TimingSj, sendUser, sendCust, Class }));
                }
                else
                {
                    //YPTWebService.PlatformWebServiceSoapClient pws = new YPTWebService.PlatformWebServiceSoapClient();
                    //result = pws.SendMessage(Name, Psw, sendDoc, getTel, isTiming, TimingSj, sendUser, sendCust, Class);
                }
            }
            catch (Exception e)
            {
            }
            return result;
        }
        #endregion
    }


    public class ApiClient
    {
        public DateTime workTime { get; set; }//EAS 服务器回传时间
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

        //public Int64 count = 0;
        //private DateTime startWorkTime = new DateTime();
        public Boolean isError;
        private String errorDetail;
        private String errorType = "LOG";
        private DataTable ddFlagS;//已上传未确认订单
        private DataTable ddFlagY;//未上传订单
        private String ddBillNo;
        private DataTable ckTable;
        private DataTable thTable;
        private DataTable syTable;
        private int billTimeOut = 60;
        private void init()
        {
            this.isError = false;
            this.startTime = DateTime.Now;
        }

        ApiClient()
        {
            this.workTime = new DateTime();
            this.paraTime = new DateTime();
        }

        public ApiClient(String type)
        {
            this.orgWorkType = this.workType = type;
            this.workTime = new DateTime();
            this.paraTime = new DateTime();
            try
            {
                billTimeOut = Convert.ToInt32(ApiUtil.GetConfigCont("BILLTIMEOUT"));
            }
            catch
            { }
        }

        public void setBillTimeOut(int days)
        {
            this.billTimeOut = days;
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

        private Boolean export(string tableName, JObject result)
        {
            DbHelperOra.ExecuteSql("TRUNCATE TABLE " + tableName);
            DataTable tableSchema = ApiClientUtil.GetTableSchema(tableName);
            JArray ja = result.Value<JArray>("data");
            string columnNames = "";
            string columnValues = "";
            string sql = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(" BEGIN ");
            foreach (JToken jt in ja)
            {
                JObject joHead = (JObject)jt;
                columnNames = "";
                columnValues = "";
                foreach (DataColumn dc in tableSchema.Columns)
                {
                    string colType = dc.DataType.ToString();
                    string colName = dc.ColumnName;
                    columnNames += colName + ",";
                    if (joHead.Value<String>(colName) == null)
                    {
                        columnValues += "null,";
                    }
                    else if ("System.String".Equals(colType))
                    {
                        columnValues += "'" + joHead.Value<String>(colName) + "',";
                    }
                    else if ("System.DateTime".Equals(colType))
                    {
                        columnValues += "TO_DATE('" + joHead.Value<String>(colName) + "','mm/dd/yyyy hh24:mi:ss'),";
                    }
                    else if ("System.Decimal".Equals(colType))
                    {
                        columnValues += joHead.Value<String>(colName) + ",";
                    }
                    else if ("System.Int32".Equals(colType))
                    {
                        columnValues += joHead.Value<String>(colName) + ",";
                    }
                    else
                    {
                        columnValues += "'" + joHead.Value<String>(colName) + "',";
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
            bool oraResult = DbHelperOra.ExecuteSqlTran(cmdList);
            //int oraResult = DbHelperOra.ExecuteSql(builder.ToString());
            if (oraResult)
            {
                this.execCount += ja.Count;
            }
            else
            {
                this.total = -1;
                this.isError = true;
            }
            this.total = this.execCount;
            this.workTime = Convert.ToDateTime(result.GetValue("time"));
            this.errorDetail = result.GetValue("reason").ToString();
            return true;
        }

        //判断接口是否正在执行方法
        private Boolean checkExecing()
        {
            OracleConnection conn = null;
            OracleTransaction trans = null;
            try
            {
                conn = new OracleConnection(DbHelperOra.connectionString);
                conn.Open();
                trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = trans;
                cmd.CommandText = "select EXECING from dat_infdata_def where INFTYPE='" + this.workType + "' for update";
                String Flag = (String)cmd.ExecuteScalar();
                if ("Y".Equals(Flag))
                {
                    trans.Rollback();
                    this.isError = true;
                    this.errorDetail = "该接口正在执行中";
                    this.total = -1;
                    this.errorType = "LOG";
                    save(this.errorDetail);
                }
                else
                {
                    cmd.CommandText = "update dat_infdata_def set EXECING='Y' where INFTYPE='" + this.workType + "'";
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                trans.Rollback();
                this.isError = true;

            }
            finally
            {
                conn.Close();
            }

            if (this.isError)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private Boolean execute()
        {
            //判断接口是否正在执行
            if (checkExecing())
            {
                return false;
            }
            this.execCount = 0;
            JObject result;
            String[] workTypeArray = this.workType.Split('_');
            switch (this.workType)
            {
                case "DOC_GOODS":
                case "DOC_GOODSUNIT":
                case "SYS_CATEGORY":
                case "DOC_GOODSJX":
                case "DOC_GOODSPH":
                case "DOC_SUPPLIER":
                case "DOC_SUPPLIERYPT":
                case "DOC_PRODUCER":
                case "DOC_GOODSYX":
                case "DOC_GOODSPICTURE":
                case "DOC_GOODSLIC":
                case "DOC_SUPLIC":
                case "DOC_GOODSGRANT":
                case "DOC_GOODSSUP":
                    this.workType = "INF_" + this.orgWorkType;//workTypeArray[1] + "_PLATFORM";
                    result = ApiClientService.query(this.orgWorkType, this.paraTime.ToString("yyyy/MM/dd HH:mm:ss"));
                    if (result != null)
                    {
                        if (result.Value<JArray>("data") != null)
                        {
                            if (result.Value<JArray>("data").Count == 0)
                            {
                                this.total = -1;
                                this.isError = true;
                                this.errorDetail = "无数据获取";
                                this.errorType = "LOG";
                            }
                            else
                            {
                                export(this.workType, result);
                                if (ApiClientUtil.runProcedure("INFTEMP.CONV_" + workTypeArray[1] + "_PLATFORM") <= 0)
                                {
                                    this.isError = true;
                                    this.errorType = "ERR";
                                    this.errorDetail = "执行存储过程时发生未知错误";
                                }
                            }
                        }
                        else
                        {
                            this.total = -1;
                            this.isError = true;
                            this.errorType = "LOG";
                            this.errorDetail = "无数据获取";
                        }

                    }
                    else
                    {
                        this.total = -1;
                        this.isError = true;
                        this.errorType = "ERR";
                        this.errorDetail = "用户信息不合法，登陆失败";
                    }
                    save();
                    break;

                #region 威海中心医院普耗商品资料下传
                case "DOC_GOODSP":
                    this.workType = "INF_DOC_GOODS";
                    result = ApiClientService.query(this.orgWorkType, this.paraTime.ToString("yyyy/MM/dd HH:mm:ss"));
                    if (result != null)
                    {
                        if (result.Value<JArray>("data") != null)
                        {
                            if (result.Value<JArray>("data").Count == 0)
                            {
                                this.total = -1;
                                this.isError = true;
                                this.errorDetail = "无数据获取";
                                this.errorType = "LOG";
                            }
                            else
                            {
                                export(this.workType, result);
                                if (ApiClientUtil.runProcedure("INFTEMP.CONV_GOODS_PLATFORM") <= 0)
                                {
                                    this.isError = true;
                                    this.errorType = "ERR";
                                    this.errorDetail = "执行存储过程时发生未知错误";
                                }
                            }
                        }
                        else
                        {
                            this.total = -1;
                            this.isError = true;
                            this.errorType = "LOG";
                            this.errorDetail = "无数据获取";
                        }
                    }
                    else
                    {
                        this.total = -1;
                        this.isError = true;
                        this.errorType = "ERR";
                        this.errorDetail = "用户信息不合法，登陆失败";
                    }
                    save();
                    break;
                #endregion

                #region DOC_GOODSPICTUREDOWN  商品图片下传
                case "DOC_GOODSPICTUREDOWN":
                    string picPath = "";
                    string str3 = "Y";
                    string sqlGoodsPic = @"select GDSEQ,ROWNO,PICPATH from doc_goodspicture
                                            WHERE STR3='N' AND STR2='N'
                                            ORDER BY GDSEQ,ROWNO";
                    DataTable dtGoodsPic = DbHelperOra.Query(sqlGoodsPic).Tables[0];
                    if (dtGoodsPic != null && dtGoodsPic.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtGoodsPic.Rows)
                        {
                            try
                            {
                                picPath = ApiClientUtil.DownloadSavePic(dr["PICPATH"].ToString());
                            }
                            catch
                            {
                                str3 = "E";
                            }

                            if(!String.IsNullOrEmpty(picPath)&&"ERROR".Equals(picPath))
                            {
                                str3 = "E";
                            }
                            sqlGoodsPic = "update doc_goodspicture set STR3='{3}',STR1='{2}' WHERE GDSEQ='{0}' AND ROWNO='{1}'";
                            sqlGoodsPic = string.Format(sqlGoodsPic, dr["GDSEQ"].ToString(), dr["ROWNO"].ToString(), picPath,str3);
                            DbHelperOra.ExecuteSql(sqlGoodsPic);
                        }
                        this.total = this.execCount = dtGoodsPic.Rows.Count;
                    }
                    else
                    {
                        this.errorDetail = "没有需要下传的数据";
                        this.isError = true;
                        this.total = -1;
                        this.errorType = "LOG";
                    }
                    save();
                    break;
                #endregion

                #region DOC_GOODSLICDOWN  商品证照下传
                case "DOC_GOODSLICDOWN":
                    string picPathLic = "";
                    string isDown = "Y";
                    string sqlGoodsLic = @"select guid,imgpath from doc_goodslic where isdown='N'";
                    DataTable dtGoodsLic = DbHelperOra.Query(sqlGoodsLic).Tables[0];
                    if (dtGoodsLic != null && dtGoodsLic.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtGoodsLic.Rows)
                        {
                            picPathLic = ApiClientUtil.DownloadSavePic(dr["imgpath"].ToString());
                            if (!String.IsNullOrEmpty(picPathLic) && "ERROR".Equals(picPathLic))
                            {
                                isDown = "E";
                            }
                            sqlGoodsPic = "update doc_goodslic set imgpaths='{1}',downtime=sysdate,isdown='{2}' WHERE guid='{0}'";
                            sqlGoodsPic = string.Format(sqlGoodsPic, dr["guid"].ToString(), picPathLic, isDown);
                            DbHelperOra.ExecuteSql(sqlGoodsPic);
                        }
                        this.total = this.execCount = dtGoodsLic.Rows.Count;
                    }
                    else
                    {
                        this.errorDetail = "没有需要下传的数据";
                        this.isError = true;
                        this.total = -1;
                        this.errorType = "LOG";
                    }
                    save();
                    break;
                #endregion

                #region DAT_RK_DOC
                case "DAT_RK_DOC":
                    //--------------delete ---------------------------------
                    String delsqlrk = "truncate table INF_DAT_RK_DOC";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlrk);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    delsqlrk = "truncate table INF_DAT_RK_COM";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlrk);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    delsqlrk = "truncate table INF_DAT_RK_EXT";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlrk);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    //------------------------------------------------------

                    result = ApiClientService.queryBill(this.orgWorkType, null);
                    if (result != null)
                    {

                        if (JsonConvert.SerializeObject(result.Value<JObject>("data")).Length > 2)
                        {
                            JObject resultData = result.Value<JObject>("data");
                            try
                            {
                                insertBill(workTypeArray[1], result);
                            }
                            catch (Exception e)
                            {
                                this.errorDetail += "插入数据时发生错误" + e.Message + "|" + e.StackTrace;
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                                save();
                                break;
                            }

                            if (ApiClientUtil.runProcedure("INFTEMP.CONV_" + workTypeArray[1] + "_PLATFORM") <= 0)
                            {
                                this.errorDetail += "执行存储过程时发生未知错误";
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                            }
                            else
                            {
                                string SEQNOS = "";
                                foreach (JToken jt in resultData.Value<JArray>("heads"))
                                {
                                    this.errorDetail += "SEQNO[" + jt.Value<String>("SEQNO") + "] SUCCESS. ";
                                    SEQNOS += "'" + jt.Value<String>("SEQNO") + "',";
                                }
                                SEQNOS = SEQNOS.TrimEnd(',');
                                ApiClientService.exec(this.orgWorkType + "_STATUS", SEQNOS);

                                this.total = this.execCount = resultData.Value<JArray>("heads").Count;
                            }
                        }
                        else
                        {
                            this.errorDetail = "没有需要下传的数据";
                            this.isError = true;
                            this.total = -1;
                            this.errorType = "LOG";
                        }

                    }
                    else
                    {
                        this.errorDetail = "用户信息不合法，登陆失败";
                        this.isError = true;
                        this.total = -1;
                        this.errorType = "ERR";
                    }

                    save();
                    break;
                #endregion

                #region DAT_RK_DOCB办公用品入库单下传
                case "DAT_RK_DOCB":
                    //--------------delete ---------------------------------
                    String delsqlrkB = "truncate table INF_DAT_RK_DOC";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlrkB);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    delsqlrkB = "truncate table INF_DAT_RK_COM";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlrkB);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    delsqlrkB = "truncate table INF_DAT_RK_EXT";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlrkB);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    //------------------------------------------------------

                    result = ApiClientService.queryBill(this.orgWorkType, null);
                    if (result != null)
                    {

                        if (JsonConvert.SerializeObject(result.Value<JObject>("data")).Length > 2)
                        {
                            JObject resultData = result.Value<JObject>("data");
                            try
                            {
                                insertBill(workTypeArray[1], result);
                            }
                            catch (Exception e)
                            {
                                this.errorDetail += "插入数据时发生错误" + e.Message + "|" + e.StackTrace;
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                                save();
                                break;
                            }

                            if (ApiClientUtil.runProcedure("INFTEMP.CONV_" + workTypeArray[1] + "_PLATFORM") <= 0)
                            {
                                this.errorDetail += "执行存储过程时发生未知错误";
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                            }
                            else
                            {
                                string SEQNOS = "";
                                foreach (JToken jt in resultData.Value<JArray>("heads"))
                                {
                                    this.errorDetail += "SEQNO[" + jt.Value<String>("SEQNO") + "] SUCCESS. ";
                                    SEQNOS += "'" + jt.Value<String>("SEQNO") + "',";
                                }
                                SEQNOS = SEQNOS.TrimEnd(',');
                                ApiClientService.exec(this.orgWorkType + "_STATUS", SEQNOS);

                                this.total = this.execCount = resultData.Value<JArray>("heads").Count;
                            }
                        }
                        else
                        {
                            this.errorDetail = "没有需要下传的数据";
                            this.isError = true;
                            this.total = -1;
                            this.errorType = "LOG";
                        }

                    }
                    else
                    {
                        this.errorDetail = "用户信息不合法，登陆失败";
                        this.isError = true;
                        this.total = -1;
                        this.errorType = "ERR";
                    }

                    save();
                    break;
                #endregion

                #region DAT_CK_DOC  出库单下传作为ERP入库单
                case "DAT_CK_DOC":
                    //--------------delete ---------------------------------
                    String delsqlck = "truncate table INF_DAT_CK_DOC";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlck);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    delsqlck = "truncate table INF_DAT_CK_COM";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlck);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    delsqlck = "truncate table INF_DAT_CK_EXT";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlck);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    //------------------------------------------------------

                    result = ApiClientService.queryBill(this.orgWorkType, null);
                    if (result != null)
                    {
                        if (JsonConvert.SerializeObject(result.Value<JObject>("data")).Length > 2)
                        {
                            JObject resultData = result.Value<JObject>("data");
                            try
                            {
                                insertBill(workTypeArray[1], result);
                            }
                            catch (Exception e)
                            {
                                this.errorDetail += "插入数据时发生错误" + e.Message + "|" + e.StackTrace;
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                                save();
                                break;
                            }

                            if (ApiClientUtil.runProcedure("INFTEMP.CONV_CK_PLATFORM") <= 0)
                            {
                                this.errorDetail += "执行存储过程时发生未知错误";
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                            }
                            else
                            {
                                string SEQNOS = "";
                                foreach (JToken jt in resultData.Value<JArray>("heads"))
                                {
                                    this.errorDetail += "SEQNO[" + jt.Value<String>("SEQNO") + "] SUCCESS. ";
                                    SEQNOS += "'" + jt.Value<String>("SEQNO") + "',";
                                }
                                SEQNOS = SEQNOS.TrimEnd(',');
                                ApiClientService.exec(this.orgWorkType + "_STATUS", SEQNOS);

                                this.total = this.execCount = resultData.Value<JArray>("heads").Count;
                            }
                        }
                        else
                        {
                            this.errorDetail = "没有需要下传的数据";
                            this.isError = true;
                            this.total = -1;
                            this.errorType = "LOG";
                        }

                    }
                    else
                    {
                        this.errorDetail = "用户信息不合法，登陆失败";
                        this.isError = true;
                        this.total = -1;
                        this.errorType = "ERR";
                    }

                    save();
                    break;
                #endregion

                #region DAT_CKERP_DOC  ERP出库单下传作为ERP入库单
                case "DAT_CKERP_DOC":
                    //--------------delete ---------------------------------
                    String delsqlckERP = "truncate table INF_DAT_CKERP_DOC";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlckERP);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    delsqlck = "truncate table INF_DAT_CKERP_COM";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlck);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    //------------------------------------------------------

                    result = ApiClientService.queryBill(this.orgWorkType, null);
                    if (result != null)
                    {
                        if (JsonConvert.SerializeObject(result.Value<JObject>("data")).Length > 2)
                        {
                            JObject resultData = result.Value<JObject>("data");
                            try
                            {
                                insertBill(workTypeArray[1], result);
                            }
                            catch (Exception e)
                            {
                                this.errorDetail += "插入数据时发生错误" + e.Message + "|" + e.StackTrace;
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                                save();
                                break;
                            }

                            if (ApiClientUtil.runProcedure("INFTEMP.CONV_CKERP_PLATFORM") <= 0)
                            {
                                this.errorDetail += "执行存储过程时发生未知错误";
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                            }
                            else
                            {
                                string SEQNOS = "";
                                foreach (JToken jt in resultData.Value<JArray>("heads"))
                                {
                                    this.errorDetail += "SEQNO[" + jt.Value<String>("SEQNO") + "] SUCCESS. ";
                                    SEQNOS += "'" + jt.Value<String>("SEQNO") + "',";
                                }
                                SEQNOS = SEQNOS.TrimEnd(',');
                                ApiClientService.exec(this.orgWorkType + "_STATUS", SEQNOS);

                                this.total = this.execCount = resultData.Value<JArray>("heads").Count;
                            }
                        }
                        else
                        {
                            this.errorDetail = "没有需要下传的数据";
                            this.isError = true;
                            this.total = -1;
                            this.errorType = "LOG";
                        }

                    }
                    else
                    {
                        this.errorDetail = "用户信息不合法，登陆失败";
                        this.isError = true;
                        this.total = -1;
                        this.errorType = "ERR";
                    }

                    save();
                    break;
                #endregion

                #region DAT_FP_DOC  发票下传
                case "DAT_FP_DOC":
                    //--------------delete ---------------------------------
                    String delsqlfp = "truncate table INF_DAT_FP_DOC";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlfp);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    delsqlfp = "truncate table INF_DAT_FP_COM";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlfp);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    //------------------------------------------------------

                    result = ApiClientService.queryBill(this.orgWorkType, null);
                    if (result != null)
                    {
                        if (JsonConvert.SerializeObject(result.Value<JObject>("data")).Length > 2)
                        {
                            JObject resultData = result.Value<JObject>("data");
                            try
                            {
                                insertBill(workTypeArray[1], result);
                            }
                            catch (Exception e)
                            {
                                this.errorDetail += "插入数据时发生错误" + e.Message + "|" + e.StackTrace;
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                                save();
                                break;
                            }

                            if (ApiClientUtil.runProcedure("INFTEMP.CONV_FP_PLATFORM") <= 0)
                            {
                                this.errorDetail += "执行存储过程时发生未知错误";
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                            }
                            else
                            {
                                string SEQNOS = "";
                                foreach (JToken jt in resultData.Value<JArray>("heads"))
                                {
                                    this.errorDetail += "SEQNO[" + jt.Value<String>("SEQNO") + "] SUCCESS. ";
                                    SEQNOS += "'" + jt.Value<String>("SEQNO") + "',";
                                }
                                SEQNOS = SEQNOS.TrimEnd(',');
                                ApiClientService.exec(this.orgWorkType + "_STATUS", SEQNOS);

                                this.total = this.execCount = resultData.Value<JArray>("heads").Count;
                            }
                        }
                        else
                        {
                            this.errorDetail = "没有需要下传的数据";
                            this.isError = true;
                            this.total = -1;
                            this.errorType = "LOG";
                        }

                    }
                    else
                    {
                        this.errorDetail = "用户信息不合法，登陆失败";
                        this.isError = true;
                        this.total = -1;
                        this.errorType = "ERR";
                    }

                    save();
                    break;
                #endregion

                #region 入库单上传
                case "DAT_RK_DOCUP":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    DataTable rkDt = queryRKD();
                    JArray rkBillJa = new JArray();
                    JArray rkBillBodyJa = new JArray();
                    JArray rkBillExtJa = new JArray();
                    foreach (DataRow dr in rkDt.Rows)
                    {
                        DataTable dtBody = queryRKCom(dr["SEQNO"].ToString());
                        if (dtBody != null && dtBody.Rows.Count > 0)
                        {
                            String[] paramArray = new String[dr.ItemArray.Length];
                            for (int i = 0; i < dr.ItemArray.Length; i++)
                            {
                                paramArray[i] = dr.ItemArray[i].ToString();
                            }
                            rkBillJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                            foreach (DataRow drBody in dtBody.Rows)
                            {
                                String[] paramArrayBody = new String[drBody.ItemArray.Length];
                                for (int j = 0; j < drBody.ItemArray.Length; j++)
                                {
                                    paramArrayBody[j] = drBody.ItemArray[j].ToString();
                                }
                                rkBillBodyJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArrayBody)));
                            }
                        }

                        DataTable dtExt = queryRKExt(dr["SEQNO"].ToString());
                        if (dtExt != null && dtExt.Rows.Count > 0)
                        {
                            foreach (DataRow drExt in dtExt.Rows)
                            {
                                String[] paramArrayExt = new String[drExt.ItemArray.Length];
                                for (int j = 0; j < drExt.ItemArray.Length; j++)
                                {
                                    paramArrayExt[j] = drExt.ItemArray[j].ToString();
                                }
                                rkBillExtJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArrayExt)));
                            }
                        }
                    }
                    if (rkBillJa.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill("DAT_RK_DOCUP", rkBillJa, rkBillBodyJa, rkBillExtJa);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updateRK(rkDt, "Y");
                                this.execCount = rkBillJa.Count;
                                this.total = rkBillJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                    this.errorType = "ERR";
                                }

                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion                

                #region 高值跟台入库单上传
                case "DAT_RK_DOCUPY":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    DataTable yrkDt = queryYRKD();
                    JArray yrkBillJa = new JArray();
                    JArray yrkBillBodyJa = new JArray();
                    JArray yrkBillExtJa = new JArray();
                    foreach (DataRow dr in yrkDt.Rows)
                    {
                        DataTable dtBody = queryYRKCom(dr["SEQNO"].ToString());
                        if (dtBody != null && dtBody.Rows.Count > 0)
                        {
                            String[] paramArray = new String[dr.ItemArray.Length];
                            for (int i = 0; i < dr.ItemArray.Length; i++)
                            {
                                paramArray[i] = dr.ItemArray[i].ToString();
                            }
                            yrkBillJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                            foreach (DataRow drBody in dtBody.Rows)
                            {
                                String[] paramArrayBody = new String[drBody.ItemArray.Length];
                                for (int j = 0; j < drBody.ItemArray.Length; j++)
                                {
                                    paramArrayBody[j] = drBody.ItemArray[j].ToString();
                                }
                                yrkBillBodyJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArrayBody)));
                            }
                        }
                    }
                    if (yrkBillJa.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill("DAT_RK_DOCUPY", yrkBillJa, yrkBillBodyJa, yrkBillExtJa);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updateYRK(yrkDt, "Y");
                                this.execCount = yrkBillJa.Count;
                                this.total = yrkBillJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                    this.errorType = "ERR";
                                }

                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion                

                #region DAT_TH_DOCB 办公用品退货下传
                case "DAT_TH_DOCB":
                    //--------------delete ---------------------------------
                    String delsqlthb = "truncate table INF_DAT_TH_DOC";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlthb);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    delsqlthb = "truncate table INF_DAT_TH_COM";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlthb);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }

                    //------------------------------------------------------

                    result = ApiClientService.queryBill(this.orgWorkType, null);
                    if (result != null)
                    {
                        if (JsonConvert.SerializeObject(result.Value<JObject>("data")).Length > 2)
                        {
                            JObject resultData = result.Value<JObject>("data");
                            try
                            {
                                insertBill(workTypeArray[1], result);
                            }
                            catch (Exception e)
                            {
                                this.errorDetail += "插入数据时发生错误" + e.Message + "|" + e.StackTrace;
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                                save();
                                break;
                            }

                            if (ApiClientUtil.runProcedure("INFTEMP.CONV_" + workTypeArray[1] + "_PLATFORM") <= 0)
                            {
                                this.errorDetail += "执行存储过程时发生未知错误";
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                            }
                            else
                            {
                                string SEQNOS = "";
                                foreach (JToken jt in resultData.Value<JArray>("heads"))
                                {
                                    this.errorDetail += "SEQNO[" + jt.Value<String>("SEQNO") + "] SUCCESS. ";
                                    SEQNOS += "'" + jt.Value<String>("SEQNO") + "',";
                                }
                                SEQNOS = SEQNOS.TrimEnd(',');
                                ApiClientService.exec(this.orgWorkType + "_STATUS", SEQNOS);
                                this.total = this.execCount = resultData.Value<JArray>("heads").Count;
                            }
                        }
                        else
                        {
                            this.errorDetail = "没有需要下传的数据";
                            this.isError = true;
                            this.total = -1;
                            this.errorType = "LOG";
                        }

                    }
                    else
                    {
                        this.errorDetail = "用户信息不合法，登陆失败";
                        this.isError = true;
                        this.total = -1;
                        this.errorType = "ERR";
                    }

                    save();
                    break;
                #endregion

                #region 
                case "DD_CONFIRM":
                    //在某一订单的物料全部审核入库时，需要执行此方法
                    //处理插入的入库单，调用 recordState ,flag = 'S'  => 'F' 
                    if (ApiClientUtil.isNull(this.ddBillNo))
                    {
                        return false;
                    }
                    else
                    {
                        try
                        {
                            //TODO 标记状态已完成，
                            //result = ApiClientService.query(this.orgWorkType,this.ddBillNo);//标记状态已完成

                            //if (result.Value<String>("result").Equals("success"))
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
                #endregion

                #region 订单上传
                case "DAT_DD_DOC":
                    //上传订单
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    this.errorType = "LOG";
                    DataTable dt = queryDD();
                    ApiClientService.billJa = new JArray();
                    ApiClientService.billBodyJa = new JArray();
                    ApiClientService.billExtJa = new JArray();
                    foreach (DataRow dr in dt.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        ApiClientService.renderBillParams(paramArray);
                        DataTable dtBody = queryDDCom(dr["SEQNO"].ToString());
                        foreach (DataRow drBody in dtBody.Rows)
                        {
                            String[] paramArrayBody = new String[drBody.ItemArray.Length];
                            for (int j = 0; j < drBody.ItemArray.Length; j++)
                            {
                                paramArrayBody[j] = drBody.ItemArray[j].ToString();
                            }
                            ApiClientService.renderBillBodyParams(paramArrayBody);
                        }
                        //当有附表信息时要上传附表信息 c 20150416
                        DataTable dtExt = queryDDExt(dr["SEQNO"].ToString());
                        if (dtExt != null && dtExt.Rows.Count > 0)
                        {
                            foreach (DataRow drExt in dtExt.Rows)
                            {
                                String[] paramArrayExt = new String[drExt.ItemArray.Length];
                                for (int j = 0; j < drExt.ItemArray.Length; j++)
                                {
                                    paramArrayExt[j] = drExt.ItemArray[j].ToString();
                                }
                                ApiClientService.renderBillExtParams(paramArrayExt);
                            }
                        }
                    }
                    if (ApiClientService.billJa.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill(this.orgWorkType);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updateDD(ddFlagY, "Y");
                                this.execCount = ApiClientService.billJa.Count;
                                this.total = ApiClientService.billJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                    this.errorType = "ERR";
                                }
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }

                    save();
                    break;
                #endregion

                #region 订单上传采购平台
                case "DAT_DD_DOCC":
                    //上传订单
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    this.errorType = "LOG";
                    DataTable dtC = queryDDC();
                    ApiClientService.billJa = new JArray();
                    ApiClientService.billBodyJa = new JArray();
                    ApiClientService.billExtJa = new JArray();
                    foreach (DataRow dr in dtC.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        ApiClientService.renderBillParams(paramArray);
                        DataTable dtBody = queryDDCCom(dr["SEQNO"].ToString());
                        foreach (DataRow drBody in dtBody.Rows)
                        {
                            String[] paramArrayBody = new String[drBody.ItemArray.Length];
                            for (int j = 0; j < drBody.ItemArray.Length; j++)
                            {
                                paramArrayBody[j] = drBody.ItemArray[j].ToString();
                            }
                            ApiClientService.renderBillBodyParams(paramArrayBody);
                        }
                        //当有附表信息时要上传附表信息 c 20150416
                        DataTable dtExt = queryDDExt(dr["SEQNO"].ToString());
                        if (dtExt != null && dtExt.Rows.Count > 0)
                        {
                            foreach (DataRow drExt in dtExt.Rows)
                            {
                                String[] paramArrayExt = new String[drExt.ItemArray.Length];
                                for (int j = 0; j < drExt.ItemArray.Length; j++)
                                {
                                    paramArrayExt[j] = drExt.ItemArray[j].ToString();
                                }
                                ApiClientService.renderBillExtParams(paramArrayExt);
                            }
                        }
                    }
                    if (ApiClientService.billJa.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill(this.orgWorkType);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updateDD(ddFlagY, "Y");
                                this.execCount = ApiClientService.billJa.Count;
                                this.total = ApiClientService.billJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                    this.errorType = "ERR";
                                }
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }

                    save();
                    break;
                #endregion

                #region 办公用品订单上传
                case "DAT_DD_DOCB":
                    //上传订单
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    this.errorType = "LOG";
                    DataTable ddDt = queryDD();
                    JArray ddHeadJa = new JArray();
                    JArray ddBodyJa = new JArray();

                    foreach (DataRow dr in ddDt.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        ddHeadJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                        DataTable dtBody = queryDDBCom(dr["SEQNO"].ToString());
                        foreach (DataRow drBody in dtBody.Rows)
                        {
                            String[] paramArrayBody = new String[drBody.ItemArray.Length];
                            for (int j = 0; j < drBody.ItemArray.Length; j++)
                            {
                                paramArrayBody[j] = drBody.ItemArray[j].ToString();
                            }
                            ddBodyJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArrayBody)));
                        }
                    }
                    if (ddHeadJa.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill(this.orgWorkType, ddHeadJa, ddBodyJa, null);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updateDD(ddDt, "Y");
                                this.execCount = ddHeadJa.Count;
                                this.total = ddHeadJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                    this.errorType = "ERR";
                                }
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }

                    save();
                    break;
                #endregion

                #region 销售单上传
		            case "DAT_XS_DOC_OLD":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    DataTable xsDt = queryXS();
                    JArray xsBillJa = new JArray();
                    JArray xsBillBodyJa = new JArray();
                    JArray xsBillExtJa = new JArray();
                    foreach (DataRow dr in xsDt.Rows)
                    {
                        DataTable dtBody = queryXSCom(dr["SEQNO"].ToString());
                        if(dtBody!=null&&dtBody.Rows.Count>0)
                        {
                            String[] paramArray = new String[dr.ItemArray.Length];
                            for (int i = 0; i < dr.ItemArray.Length; i++)
                            {
                                paramArray[i] = dr.ItemArray[i].ToString();
                            }
                            //ApiClientService.renderBillParams(paramArray);
                            xsBillJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                            foreach (DataRow drBody in dtBody.Rows)
                            {
                                String[] paramArrayBody = new String[drBody.ItemArray.Length];
                                for (int j = 0; j < drBody.ItemArray.Length; j++)
                                {
                                    paramArrayBody[j] = drBody.ItemArray[j].ToString();
                                }
                                //ApiClientService.renderBillBodyParams(paramArrayBody);
                                xsBillBodyJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArrayBody)));
                            }
                        }
                    }
                    if (xsBillJa.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill("DAT_CK_DOC", xsBillJa, xsBillBodyJa, null);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updateXS(ckTable, "Y");
                                this.execCount = xsBillJa.Count;
                                this.total = xsBillJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                    this.errorType = "ERR";
                                }

                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
	                #endregion                
                
                #region 销售单上传
                case "DAT_XS_DOCUP":
                this.workTime = DateTime.Now;
                this.errorDetail = "";
                int intCount = queryXSUPCount();//查询记录数后分页取每页200单
                for (int ip = 0; ip <= intCount / 200;ip++ )
                {
                    DataTable xsUPDt = queryXSUP();
                    JArray xsUPBillJa = new JArray();
                    JArray xsUPBillBodyJa = new JArray();
                    foreach (DataRow dr in xsUPDt.Rows)
                    {
                        DataTable dtBody = queryXSComUP(dr["SEQNO"].ToString());
                        if (dtBody != null && dtBody.Rows.Count > 0)
                        {
                            String[] paramArray = new String[dr.ItemArray.Length];
                            for (int i = 0; i < dr.ItemArray.Length; i++)
                            {
                                paramArray[i] = dr.ItemArray[i].ToString();
                            }
                            xsUPBillJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                            foreach (DataRow drBody in dtBody.Rows)
                            {
                                String[] paramArrayBody = new String[drBody.ItemArray.Length];
                                for (int j = 0; j < drBody.ItemArray.Length; j++)
                                {
                                    paramArrayBody[j] = drBody.ItemArray[j].ToString();
                                }
                                xsUPBillBodyJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArrayBody)));
                            }
                        }
                    }
                    if (xsUPBillJa.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill("DAT_XS_DOCUP", xsUPBillJa, xsUPBillBodyJa, null);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updateXS(xsUPDt, "Y");
                                this.execCount = xsUPBillJa.Count;
                                this.total = xsUPBillJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                    this.errorType = "ERR";
                                }

                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                }
 
                save();
                break;
                #endregion                

                #region 结算单上传
                case "DAT_JSD_DOC":
                this.workTime = DateTime.Now;
                this.errorDetail = "";
                DataTable jsDt = queryJSD();
                JArray jsBillJa = new JArray();
                JArray jsBillBodyJa = new JArray();
                foreach (DataRow dr in jsDt.Rows)
                {
                    String[] paramArray = new String[dr.ItemArray.Length];
                    for (int i = 0; i < dr.ItemArray.Length; i++)
                    {
                        paramArray[i] = dr.ItemArray[i].ToString();
                    }
                    jsBillJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                    DataTable dtBody = queryJSDCom(dr["SEQNO"].ToString());
                    foreach (DataRow drBody in dtBody.Rows)
                    {
                        String[] paramArrayBody = new String[drBody.ItemArray.Length];
                        for (int j = 0; j < drBody.ItemArray.Length; j++)
                        {
                            paramArrayBody[j] = drBody.ItemArray[j].ToString();
                        }
                        jsBillBodyJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArrayBody)));
                    }
                }
                if (jsBillJa.Count > 0)
                {
                    result = ApiClientService.execRenderedBill(this.orgWorkType, jsBillJa, jsBillBodyJa, null);
                    if (result != null)
                    {
                        if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                        {
                            updateJSD(jsDt, "G");
                            this.execCount = jsBillJa.Count;
                            this.total = jsBillJa.Count;
                        }
                        else
                        {
                            this.isError = true;
                            this.errorType = "ERR";
                            this.total = -1;
                            if (result.GetValue("reason") != null)
                            {
                                this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                            }
                        }
                    }
                    else
                    {
                        this.errorDetail = "用户信息不合法，登陆失败";
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                    }
                }
                else
                {
                    this.errorDetail = "没有符合条件的数据";
                    this.errorType = "LOG";
                    this.isError = true;
                    this.total = -1;
                }
                save();
                break;
                #endregion

                #region 文登中心医院结算单上传
                case "DAT_JSD_DOCC":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    DataTable jsCDt = queryJSD();
                    JArray jsCBillJa = new JArray();
                    JArray jsCBillBodyJa = new JArray();
                    foreach (DataRow dr in jsCDt.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        jsCBillJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                        DataTable dtBody = queryJSDComC(dr["SEQNO"].ToString());
                        foreach (DataRow drBody in dtBody.Rows)
                        {
                            String[] paramArrayBody = new String[drBody.ItemArray.Length];
                            for (int j = 0; j < drBody.ItemArray.Length; j++)
                            {
                                paramArrayBody[j] = drBody.ItemArray[j].ToString();
                            }
                            jsCBillBodyJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArrayBody)));
                        }
                    }
                    if (jsCBillJa.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill(this.orgWorkType, jsCBillJa, jsCBillBodyJa, null);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updateJSD(jsCDt, "G");
                                this.execCount = jsCBillJa.Count;
                                this.total = jsCBillJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.errorType = "ERR";
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                }
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region 文登中心批号调整单上传
                case "DAT_PHTZ_DOCC":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    DataTable phtzDt = queryPHTZ();
                    JArray phtzBillJa = new JArray();
                    JArray phtzBillBodyJa = new JArray();
                    foreach (DataRow dr in phtzDt.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        phtzBillJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                        DataTable dtBody = queryPHTZCom(dr["SEQNO"].ToString());
                        foreach (DataRow drBody in dtBody.Rows)
                        {
                            String[] paramArrayBody = new String[drBody.ItemArray.Length];
                            for (int j = 0; j < drBody.ItemArray.Length; j++)
                            {
                                paramArrayBody[j] = drBody.ItemArray[j].ToString();
                            }
                            phtzBillBodyJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArrayBody)));
                        }
                    }
                    if (phtzBillJa.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill(this.orgWorkType, phtzBillJa, phtzBillBodyJa, null);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updatePHTZ(phtzDt, "Y");
                                this.execCount = phtzBillJa.Count;
                                this.total = phtzBillJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.errorType = "ERR";
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                }
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region 退货上传
                case "DAT_TH_DOC":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    DataTable thDt = queryTH();
                    JArray thBillJa = new JArray();
                    JArray thBillBodyJa = new JArray();
                    JArray thBillExtJa = new JArray();
                    foreach (DataRow dr in thDt.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        //ApiClientService.renderBillParams(paramArray);
                        thBillJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                        DataTable dtBody = queryTHCom(dr["SEQNO"].ToString());
                        foreach (DataRow drBody in dtBody.Rows)
                        {
                            String[] paramArrayBody = new String[drBody.ItemArray.Length];
                            for (int j = 0; j < drBody.ItemArray.Length; j++)
                            {
                                paramArrayBody[j] = drBody.ItemArray[j].ToString();
                            }
                            //ApiClientService.renderBillBodyParams(paramArrayBody);
                            thBillBodyJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArrayBody)));
                        }
                    }
                    if (thBillJa.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill(this.orgWorkType, thBillJa, thBillBodyJa, null);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updateTH(thTable, "Y");
                                this.execCount = thBillJa.Count;
                                this.total = thBillJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.errorType = "ERR";
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                }

                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region 退货上传
                case "DAT_TH_DOCC":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    DataTable thDtC = queryTHC();
                    JArray thCBillJa = new JArray();
                    JArray thCBillBodyJa = new JArray();
                    JArray thCBillExtJa = new JArray();
                    foreach (DataRow dr in thDtC.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        //ApiClientService.renderBillParams(paramArray);
                        thCBillJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                        DataTable dtBody = queryTHComC(dr["SEQNO"].ToString());
                        foreach (DataRow drBody in dtBody.Rows)
                        {
                            String[] paramArrayBody = new String[drBody.ItemArray.Length];
                            for (int j = 0; j < drBody.ItemArray.Length; j++)
                            {
                                paramArrayBody[j] = drBody.ItemArray[j].ToString();
                            }
                            //ApiClientService.renderBillBodyParams(paramArrayBody);
                            thCBillBodyJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArrayBody)));
                        }
                    }
                    if (thCBillJa.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill(this.orgWorkType, thCBillJa, thCBillBodyJa, null);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updateTH(thTable, "Y");
                                this.execCount = thCBillJa.Count;
                                this.total = thCBillJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.errorType = "ERR";
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                }

                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region 采购计划上传
                case "DAT_DDPLAN_DOC":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    DataTable cgDt = queryCGPLAN();
                    JArray cgBillJa = new JArray();
                    JArray cgBillBodyJa = new JArray();
                    foreach (DataRow dr in cgDt.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        cgBillJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                        DataTable dtBody = queryCGPLANCom(dr["SEQNO"].ToString());
                        foreach (DataRow drBody in dtBody.Rows)
                        {
                            String[] paramArrayBody = new String[drBody.ItemArray.Length];
                            for (int j = 0; j < drBody.ItemArray.Length; j++)
                            {
                                paramArrayBody[j] = drBody.ItemArray[j].ToString();
                            }
                            cgBillBodyJa.Add(JArray.FromObject(ApiClientService.renderParams(paramArrayBody)));
                        }
                    }
                    if (cgBillJa.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill(this.orgWorkType, cgBillJa, cgBillBodyJa, null);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updateCGPLAN(cgDt, "Y");
                                this.execCount = cgBillJa.Count;
                                this.total = cgBillJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.errorType = "ERR";
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                }

                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region DAT_SY_DOC 办公用品损益单下传
                case "DAT_SY_DOCB":
                    //--------------delete ---------------------------------
                    String delsqlSY = "truncate table INF_DAT_SY_DOC";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlSY);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    delsqlthb = "truncate table INF_DAT_SY_COM";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlthb);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }

                    //------------------------------------------------------

                    result = ApiClientService.queryBill(this.orgWorkType, null);
                    if (result != null)
                    {
                        if (JsonConvert.SerializeObject(result.Value<JObject>("data")).Length > 2)
                        {
                            JObject resultData = result.Value<JObject>("data");
                            try
                            {
                                insertBill(workTypeArray[1], result);
                            }
                            catch (Exception e)
                            {
                                this.errorDetail += "插入数据时发生错误" + e.Message + "|" + e.StackTrace;
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                                save();
                                break;
                            }

                            if (ApiClientUtil.runProcedure("INFTEMP.CONV_" + workTypeArray[1] + "_PLATFORM") <= 0)
                            {
                                this.errorDetail += "执行存储过程时发生未知错误";
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                            }
                            else
                            {
                                string SEQNOS = "";
                                foreach (JToken jt in resultData.Value<JArray>("heads"))
                                {
                                    this.errorDetail += "SEQNO[" + jt.Value<String>("SEQNO") + "] SUCCESS. ";
                                    SEQNOS += "'" + jt.Value<String>("SEQNO") + "',";
                                }
                                SEQNOS = SEQNOS.TrimEnd(',');
                                ApiClientService.exec(this.orgWorkType + "_STATUS", SEQNOS);
                                this.total = this.execCount = resultData.Value<JArray>("heads").Count;
                            }
                        }
                        else
                        {
                            this.errorDetail = "没有需要下传的数据";
                            this.isError = true;
                            this.total = -1;
                            this.errorType = "LOG";
                        }

                    }
                    else
                    {
                        this.errorDetail = "用户信息不合法，登陆失败";
                        this.isError = true;
                        this.total = -1;
                        this.errorType = "ERR";
                    }

                    save();
                    break;
                #endregion

                #region DAT_SY_DOCC 文登中心医院损益单下传
                case "DAT_SY_DOCC":
                    //--------------delete ---------------------------------
                    String delsqlSYC = "truncate table INF_DAT_SY_DOC";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlSYC);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }
                    delsqlSYC = "truncate table INF_DAT_SY_COM";
                    try
                    {
                        DbHelperOra.ExecuteSql(delsqlSYC);
                    }
                    catch (Exception e)
                    {
                        this.errorType = "ERR";
                        this.isError = true;
                        this.total = -1;
                        save(e.Message + "|" + e.StackTrace);
                        break;
                    }

                    //------------------------------------------------------

                    result = ApiClientService.queryBill(this.orgWorkType, null);
                    if (result != null)
                    {
                        if (JsonConvert.SerializeObject(result.Value<JObject>("data")).Length > 2)
                        {
                            JObject resultData = result.Value<JObject>("data");
                            try
                            {
                                insertBill(workTypeArray[1], result);
                            }
                            catch (Exception e)
                            {
                                this.errorDetail += "插入数据时发生错误" + e.Message + "|" + e.StackTrace;
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                                save();
                                break;
                            }

                            if (ApiClientUtil.runProcedure("INFTEMP.CONV_" + workTypeArray[1] + "C_PLATFORM") <= 0)
                            {
                                this.errorDetail += "执行存储过程时发生未知错误";
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "ERR";
                            }
                            else
                            {
                                string SEQNOS = "";
                                foreach (JToken jt in resultData.Value<JArray>("heads"))
                                {
                                    this.errorDetail += "SEQNO[" + jt.Value<String>("SEQNO") + "] SUCCESS. ";
                                    SEQNOS += "'" + jt.Value<String>("SEQNO") + "',";
                                }
                                SEQNOS = SEQNOS.TrimEnd(',');
                                ApiClientService.exec(this.orgWorkType + "_STATUS", SEQNOS);
                                this.total = this.execCount = resultData.Value<JArray>("heads").Count;
                            }
                        }
                        else
                        {
                            this.errorDetail = "没有需要下传的数据";
                            this.isError = true;
                            this.total = -1;
                            this.errorType = "LOG";
                        }

                    }
                    else
                    {
                        this.errorDetail = "用户信息不合法，登陆失败";
                        this.isError = true;
                        this.total = -1;
                        this.errorType = "ERR";
                    }

                    save();
                    break;
                #endregion


                #region 损益单上传
                case "DAT_SY_DOC_OLD":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    DataTable syDt = querySY();
                    ApiClientService.billJa = new JArray();
                    ApiClientService.billBodyJa = new JArray();
                    ApiClientService.billExtJa = new JArray();
                    foreach (DataRow dr in syDt.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        ApiClientService.renderBillParams(paramArray);
                        DataTable dtBody = querySYCom(dr["SEQNO"].ToString());
                        foreach (DataRow drBody in dtBody.Rows)
                        {
                            String[] paramArrayBody = new String[drBody.ItemArray.Length];
                            for (int j = 0; j < drBody.ItemArray.Length; j++)
                            {
                                paramArrayBody[j] = drBody.ItemArray[j].ToString();
                            }
                            ApiClientService.renderBillBodyParams(paramArrayBody);
                        }
                    }
                    if (ApiClientService.billJa.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill(this.orgWorkType);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updateSY(syTable, "Y");
                                this.execCount = ApiClientService.billJa.Count;
                                this.total = ApiClientService.billJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                    this.errorType = "ERR";
                                }

                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                            break;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break; 
                #endregion
                #region 系统日志上传
                case "SYS_OPERLOG":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";

                    DataTable t_sys_operlog = querySys_operlog();
                    if (t_sys_operlog==null)
                    {
                        this.errorDetail = "昨天的数据已提交过";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                        break;
                    }
                    ApiClientService.billJa = new JArray();
                    foreach (DataRow dr in t_sys_operlog.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString().Replace("'", "").Replace(",", "|");
                        }
                        ApiClientService.renderBillParams(paramArray);
                    }
                    if (ApiClientService.billJa.Count > 0)
                    {
                        JObject operLogResult = ApiClientService.execBulk("SYS_OPERLOG");
                        if (operLogResult != null)
                        {
                            if (operLogResult.Value<Boolean>("data") && "success".Equals(operLogResult.Value<String>("result")))
                            {
                                this.execCount = ApiClientService.billJa.Count;
                                this.total = ApiClientService.billJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                this.errorDetail += operLogResult.Value<String>("reason");
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                            break;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region 数据库空间日志上传
                case "SYS_SPACELOG":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";

                    DataTable t_sys_spacelog = querySys_spacelog();
                    if (t_sys_spacelog == null)
                    {
                        this.errorDetail = "昨天的数据已提交过";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                        break;
                    }
                    ApiClientService.billJa = new JArray();
                    foreach (DataRow dr in t_sys_spacelog.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        ApiClientService.renderBillParams(paramArray);
                    }
                    if (ApiClientService.billJa.Count > 0)
                    {
                        JObject operLogResult = ApiClientService.execBulk("SYS_SPACELOG");
                        if (operLogResult != null)
                        {
                            if (operLogResult.Value<Boolean>("data") && "success".Equals(operLogResult.Value<String>("result")))
                            {
                                this.execCount = ApiClientService.billJa.Count;
                                this.total = ApiClientService.billJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                this.errorDetail += operLogResult.Value<String>("reason");
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                            break;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region 订单到货信息上传
                case "DAT_DD_DHTIME":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";

                    DataTable t_dat_dd_dhtime = queryDAT_DD_DHTIME();
                    if (t_dat_dd_dhtime == null)
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                        break;
                    }
                    ApiClientService.billJa = new JArray();
                    foreach (DataRow dr in t_dat_dd_dhtime.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        ApiClientService.renderBillParams(paramArray);
                    }
                    if (ApiClientService.billJa.Count > 0)
                    {
                        JObject operLogResult = ApiClientService.execBulk("DAT_DD_DHTIME");
                        if (operLogResult != null)
                        {
                            if (operLogResult.Value<Boolean>("data") && "success".Equals(operLogResult.Value<String>("result")))
                            {
                                this.execCount = ApiClientService.billJa.Count;
                                this.total = ApiClientService.billJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                this.errorDetail += operLogResult.Value<String>("reason");
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                            break;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region 唯一码跟踪报表上传
                case "DAT_ONECODEJXC":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";

                    DataTable t_dat_onecodejxc = queryDat_onecodejxc();
                    if (t_dat_onecodejxc == null)
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                        break;
                    }
                    ApiClientService.billJa = new JArray();
                    foreach (DataRow dr in t_dat_onecodejxc.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        ApiClientService.renderBillParams(paramArray);
                    }
                    if (ApiClientService.billJa.Count > 0)
                    {
                        JObject operLogResult = ApiClientService.execBulk("DAT_ONECODEJXC");
                        if (operLogResult != null)
                        {
                            if (operLogResult.Value<Boolean>("data") && "success".Equals(operLogResult.Value<String>("result")))
                            {
                                updateDat_onecodejxc(t_dat_onecodejxc, "Y");
                                this.execCount = ApiClientService.billJa.Count;
                                this.total = ApiClientService.billJa.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                this.errorDetail += operLogResult.Value<String>("reason");
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                            break;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region EAS库存下传
                case "D_INVENTORY":
                    String warehouse = ApiUtil.GetConfigCont("QUERYINVENTORY_WAREHOUSE");
                    //先查询EAS库存的行数
                    JObject joCli = ApiClientService.queryInventoryByPage(warehouse, 0, 1);
                    int countRow = 0;
                    if ("success".Equals(joCli.Value<String>("result")))
                    {
                        try
                        {
                            countRow = Convert.ToInt32(joCli.Value<String>("data"));
                        }
                        catch
                        { }
                    }
                    //判断今天的库存有没有下传
                    //int sumCount = Convert.ToInt32(DbHelperOra.GetSingle("SELECT count(1) FROM EAS_STOCK where to_char(RQ,'yyyy-mm-dd')=to_char(sysdate,'yyyy-mm-dd')"));
                    //if (sumCount == countRow)
                    //{
                    //    this.errorDetail = "今天数据已下传";
                    //    this.errorType = "LOG";
                    //    this.isError = true;
                    //    this.total = -1;
                    //    save();
                    //    break;
                    //}
                    //else
                    //{
                        //DbHelperOra.ExecuteSql("delete from EAS_STOCK where to_char(RQ,'yyyy-mm-dd')=to_char(sysdate,'yyyy-mm-dd')");
                        DbHelperOra.ExecuteSql("TRUNCATE TABLE EAS_STOCK_COMPARE ");//是否保留几天的日库存？
                    //}
                    int pageNum = 100;
                    int sumPage = countRow % pageNum > 0 ? countRow / pageNum + 1 : countRow / pageNum;
                    for (int i = 1; i <= sumPage; i++)
                    {
                        joCli = ApiClientService.queryInventoryByPage(warehouse, pageNum, i);
                        JObject jo = new JObject();
                        if ("success".Equals(joCli.Value<String>("result")))
                        {
                            foreach (JToken jt in joCli.Value<JArray>("data"))
                            {
                                insertInventory(jt);
                            }
                        }
                    }

                    save();
                    break;
                #endregion

                #region EAS订单下传
                case "D_DAT_DD_DOC":
                    string custId = DbHelperOra.GetSingle("select value from sys_para where code='USERCODE'").ToString();
                    String[] paramArrayDD = { DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), "", custId };
                    //根据条件查询eas上调拨单
                    string resultDD = ApiClientService.QueryEASBill("DHD", paramArrayDD);
                    JArray jaResult = JArray.Parse(resultDD);//多单据数组
                    foreach (JToken jt in jaResult)
                    {
                        JObject jResult = (JObject)jt;//单单据对象
                        if ("success".Equals(jResult.Value<String>("result")))
                        {
                            JArray ja = jResult.Value<JArray>("data");
                            if (ja.Count > 0)
                            {
                                saveDDDocCom(ja);//把下传的单据保存到用于比较的临时表DAT_DD_DOC_COMPARE,DAT_DD_COM_COMPARE
                            }
                        }
                    }
                    save();
                    break;
                #endregion

                #region EAS入库单下传
                case "D_DAT_RK_DOC":
                    string custIdRK = DbHelperOra.GetSingle("select value from sys_para where code='USERCODE'").ToString();
                    String[] paramArrayRK = { DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), "", custIdRK };
                    //根据条件查询eas上入库单
                    string resultRK = ApiClientService.QueryEASBill("DRK", paramArrayRK);
                    JArray jaResultRK = JArray.Parse(resultRK);//多单据数组
                    foreach (JToken jt in jaResultRK)
                    {
                        JObject jResult = (JObject)jt;//单单据对象
                        if ("success".Equals(jResult.Value<String>("result")))
                        {
                            JArray ja = jResult.Value<JArray>("data");
                            if (ja.Count > 0)
                            {
                                saveRKDocCom(ja);//把下传的单据保存到用于比较的临时表DAT_RK_DOC_COMPARE,DAT_RK_COM_COMPARE
                            }
                        }
                    }
                    save();
                    break;
                #endregion

                #region EAS退货单下传
                case "D_DAT_TH_DOC":
                    string custIdTH = DbHelperOra.GetSingle("select value from sys_para where code='USERCODE'").ToString();
                    String[] paramArrayTH = { DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), "", custIdTH };
                    //根据条件查询eas上退货单
                    string resultTH = ApiClientService.QueryEASBill("THD", paramArrayTH);
                    JArray jaResultTH = JArray.Parse(resultTH);//多单据数组
                    foreach (JToken jt in jaResultTH)
                    {
                        JObject jResult = (JObject)jt;//单单据对象
                        if ("success".Equals(jResult.Value<String>("result")))
                        {
                            JArray ja = jResult.Value<JArray>("data");
                            if (ja.Count > 0)
                            {
                                saveTHDocCom(ja);//把下传的单据保存到用于比较的临时表DAT_TH_DOC_COMPARE,DAT_TH_COM_COMPARE
                            }
                        }
                    }
                    save();
                    break;
                #endregion

                #region EAS销售单下传
                case "D_DAT_XS_DOC":
                    string custIdXS = DbHelperOra.GetSingle("select value from sys_para where code='USERCODE'").ToString();
                    String[] paramArrayXS = { DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), "", custIdXS };
                    //根据条件查询eas上销售单
                    string resultXS = ApiClientService.QueryEASBill("XCK", paramArrayXS);
                    JArray jaResultXS = JArray.Parse(resultXS);//多单据数组
                    foreach (JToken jt in jaResultXS)
                    {
                        JObject jResult = (JObject)jt;//单单据对象
                        if ("success".Equals(jResult.Value<String>("result")))
                        {
                            JArray ja = jResult.Value<JArray>("data");
                            if (ja.Count > 0)
                            {
                                saveXSDocCom(ja);//把下传的单据保存到用于比较的临时表DAT_XS_DOC_COMPARE,DAT_XS_COM_COMPARE
                            }
                        }
                    }
                    save();
                    break;
                #endregion

                #region ERP库存上传
                case "DAT_STOCKDAY":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    string custIdStock = DbHelperOra.GetSingle("select value from sys_para where code='USERCODE'").ToString();
                    //先查询库存的行数
                    int countRowStock = Convert.ToInt32(DbHelperOra.GetSingle("select count(1) from DAT_GOODSSTOCK"));

                    int pageNumStock = 100;
                    int sumPageStock = countRowStock % pageNumStock > 0 ? countRowStock / pageNumStock + 1 : countRowStock / pageNumStock;
                    for (int j = 1; j <= sumPageStock; j++)
                    {
                        DataTable t_dat_stockday = queryDat_StockDay(pageNumStock,j);
                        if (t_dat_stockday == null)
                        {
                            this.errorDetail = "没有符合条件的数据";
                            this.errorType = "LOG";
                            this.isError = true;
                            this.total = -1;
                            break;
                        }
                        JArray jaStockDay = new JArray();
                        foreach (DataRow dr in t_dat_stockday.Rows)
                        {
                            String[] paramArray = new String[dr.ItemArray.Length];
                            for (int i = 0; i < dr.ItemArray.Length; i++)
                            {
                                paramArray[i] = dr.ItemArray[i].ToString();
                            }
                            jaStockDay.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                        }
                        if (jaStockDay.Count > 0)
                        {
                            JObject operLogResult = ApiClientService.execBulk("DAT_STOCKDAY", jaStockDay);
                            if (operLogResult != null)
                            {
                                if (operLogResult.Value<Boolean>("data") && "success".Equals(operLogResult.Value<String>("result")))
                                {
                                    this.execCount = jaStockDay.Count;
                                    this.total = jaStockDay.Count;
                                }
                                else
                                {
                                    this.isError = true;
                                    this.total = -1;
                                    this.errorDetail += operLogResult.Value<String>("reason");
                                }
                            }
                            else
                            {
                                this.errorDetail = "用户信息不合法，登陆失败";
                                this.errorType = "ERR";
                                this.isError = true;
                                this.total = -1;
                                break;
                            }
                        }
                        else
                        {
                            this.errorDetail = "没有符合条件的数据";
                            this.errorType = "LOG";
                            this.isError = true;
                            this.total = -1;
                        }
                    }

                    save();
                    break;
                #endregion

                #region 商品资料信息上传
                case "DOC_GOODS_UP":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";

                    DataTable t_dat_dd_goods = queryDOC_GOODS();
                    if (t_dat_dd_goods == null)
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                        break;
                    }
                    JArray jaDOC_GOODS = new JArray();
                    foreach (DataRow dr in t_dat_dd_goods.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        //ApiClientService.renderBillParams(paramArray);
                        jaDOC_GOODS.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                    }
                    if (jaDOC_GOODS.Count > 0)
                    {
                        JObject operLogResult = ApiClientService.execBulk("DOC_GOODS_UP",jaDOC_GOODS);
                        if (operLogResult != null)
                        {
                            if (operLogResult.Value<Boolean>("data") && "success".Equals(operLogResult.Value<String>("result")))
                            {
                                this.execCount = jaDOC_GOODS.Count;
                                this.total = jaDOC_GOODS.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                this.errorDetail += operLogResult.Value<String>("reason");
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                            break;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region 商品进销存信息上传
                case "DOC_GOODSJXC_UP":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";

                    DataTable t_dat_dd_goodsjxc = queryDOC_GOODSJXC();
                    if (t_dat_dd_goodsjxc == null)
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                        break;
                    }
                    JArray jaDOC_GOODSJXC = new JArray();
                    foreach (DataRow dr in t_dat_dd_goodsjxc.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        jaDOC_GOODSJXC.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                    }
                    if (jaDOC_GOODSJXC.Count > 0)
                    {
                        JObject operLogResult = ApiClientService.execBulk("DOC_GOODSJXC_UP", jaDOC_GOODSJXC);
                        if (operLogResult != null)
                        {
                            if (operLogResult.Value<Boolean>("data") && "success".Equals(operLogResult.Value<String>("result")))
                            {
                                this.execCount = jaDOC_GOODSJXC.Count;
                                this.total = jaDOC_GOODSJXC.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                this.errorDetail += operLogResult.Value<String>("reason");
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                            break;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region 供应商资料上传
                case "DOC_SUPPLIER_UP":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";

                    DataTable t_doc_supplier= queryDOC_SUPPLIER();
                    if (t_doc_supplier == null)
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                        break;
                    }
                    JArray jaDOC_SUPPLIER = new JArray();
                    foreach (DataRow dr in t_doc_supplier.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString().Replace(',', '，').Replace('\'', ' ');
                        }
                        jaDOC_SUPPLIER.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                    }
                    if (jaDOC_SUPPLIER.Count > 0)
                    {
                        JObject operLogResult = ApiClientService.execBulk("DOC_SUPPLIER_UP", jaDOC_SUPPLIER);
                        if (operLogResult != null)
                        {
                            if (operLogResult.Value<Boolean>("data") && "success".Equals(operLogResult.Value<String>("result")))
                            {
                                this.execCount = jaDOC_SUPPLIER.Count;
                                this.total = jaDOC_SUPPLIER.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                this.errorDetail += operLogResult.Value<String>("reason");
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                            break;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region 销售单上传
                case "DAT_XS_DOC":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";

                    DataTable t_view_xs_com = queryVIEW_XS_COM();
                    DataTable t_js_seqno = queryXSSeqno();
                    if (t_view_xs_com == null)
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                        break;
                    }
                    JArray jaXSCOM = new JArray();
                    foreach (DataRow dr in t_view_xs_com.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        jaXSCOM.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                    }
                    if (jaXSCOM.Count > 0)
                    {
                        JObject operLogResult = ApiClientService.execBulk("DAT_XS_DOC", jaXSCOM);
                        if (operLogResult != null)
                        {
                            if (operLogResult.Value<Boolean>("data") && "success".Equals(operLogResult.Value<String>("result")))
                            {
                                this.execCount = jaXSCOM.Count;
                                this.total = jaXSCOM.Count;
                                string seqnos = "";
                                foreach (DataRow dr in t_js_seqno.Rows)
                                {
                                    seqnos += "'" + dr["seqno"] + "',";
                                }
                                seqnos = seqnos.TrimEnd(',');
                                updateJSDFlag(seqnos);
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                this.errorDetail += operLogResult.Value<String>("reason");
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                            break;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region 文登中心医院销售单上传
                case "DAT_XS_DOCC":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";

                    DataTable t_view_xs_comC = queryVIEW_XS_COMC();
                    DataTable t_js_seqnoC = queryXSSeqno();
                    if (t_view_xs_comC == null)
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                        break;
                    }
                    JArray jaXSCOMC = new JArray();
                    foreach (DataRow dr in t_view_xs_comC.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        jaXSCOMC.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                    }
                    if (jaXSCOMC.Count > 0)
                    {
                        JObject operLogResult = ApiClientService.execBulk("DAT_XS_DOC", jaXSCOMC);
                        if (operLogResult != null)
                        {
                            if (operLogResult.Value<Boolean>("data") && "success".Equals(operLogResult.Value<String>("result")))
                            {
                                this.execCount = jaXSCOMC.Count;
                                this.total = jaXSCOMC.Count;
                                string seqnos = "";
                                foreach (DataRow dr in t_js_seqnoC.Rows)
                                {
                                    seqnos += "'" + dr["seqno"] + "',";
                                }
                                seqnos = seqnos.TrimEnd(',');
                                updateJSDFlag(seqnos);
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                this.errorDetail += operLogResult.Value<String>("reason");
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                            break;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region 科室资料上传
                case "SYS_DEPT_UP":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";

                    DataTable t_sys_dept = querySYS_DEPT();
                    if (t_sys_dept == null)
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                        break;
                    }
                    JArray jaSYS_DEPT = new JArray();
                    foreach (DataRow dr in t_sys_dept.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString().Replace(',', '，').Replace('\'', ' ');
                        }
                        jaSYS_DEPT.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                    }
                    if (jaSYS_DEPT.Count > 0)
                    {
                        JObject operLogResult = ApiClientService.execBulk("SYS_DEPT_UP", jaSYS_DEPT);
                        if (operLogResult != null)
                        {
                            if (operLogResult.Value<Boolean>("data") && "success".Equals(operLogResult.Value<String>("result")))
                            {
                                this.execCount = jaSYS_DEPT.Count;
                                this.total = jaSYS_DEPT.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                this.errorDetail += operLogResult.Value<String>("reason");
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                            break;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region 入库单驳回信息上传
                case "DAT_RKDREJECT":
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";

                    DataTable t_dat_rkd_reject = queryDAT_RKDREJECT();
                    if (t_dat_rkd_reject == null)
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                        break;
                    }
                    JArray jaDAT_RKDREJECT = new JArray();
                    foreach (DataRow dr in t_dat_rkd_reject.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        jaDAT_RKDREJECT.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                    }
                    if (jaDAT_RKDREJECT.Count > 0)
                    {
                        JObject operLogResult = ApiClientService.execBulk("DAT_RKDREJECT", jaDAT_RKDREJECT);
                        if (operLogResult != null)
                        {
                            if (operLogResult.Value<Boolean>("data") && "success".Equals(operLogResult.Value<String>("result")))
                            {
                                this.execCount = jaDAT_RKDREJECT.Count;
                                this.total = jaDAT_RKDREJECT.Count;
                                string seqnosRKDREJECT = "";
                                foreach (DataRow dr in t_dat_rkd_reject.Rows)
                                {
                                    seqnosRKDREJECT += "'" + dr["seqno"] + "',";
                                }
                                seqnosRKDREJECT = seqnosRKDREJECT.TrimEnd(',');
                                updateRKDREJECT(seqnosRKDREJECT);
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                this.errorDetail += operLogResult.Value<String>("reason");
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                            break;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }
                    save();
                    break;
                #endregion

                #region 订单状态下传
                case "DAT_DDSTATUS":
                    this.workType = "INF_" + this.orgWorkType;
                    DataTable t_dat_ddstatus = queryDAT_DDSTATUS();
                    if (t_dat_ddstatus == null || t_dat_ddstatus.Rows.Count < 1)
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                        save();
                        break;
                    }
                    string seqnosDDSTATUS = "";
                    foreach (DataRow dr in t_dat_ddstatus.Rows)
                    {
                        seqnosDDSTATUS += "'" + dr["seqno"] + "',";
                    }
                    seqnosDDSTATUS = seqnosDDSTATUS.TrimEnd(',');

                    JObject resultDDSTATUS = null;
                    if (!String.IsNullOrEmpty(seqnosDDSTATUS))
                    {
                        resultDDSTATUS = ApiClientService.query(this.orgWorkType, seqnosDDSTATUS);
                        if (resultDDSTATUS != null)
                        {
                            if (resultDDSTATUS.Value<JArray>("data") != null)
                            {
                                if (resultDDSTATUS.Value<JArray>("data").Count == 0)
                                {
                                    this.total = -1;
                                    this.isError = true;
                                    this.errorDetail = "无数据获取";
                                    this.errorType = "LOG";
                                }
                                else
                                {
                                    export(this.workType, resultDDSTATUS);
                                    if (ApiClientUtil.runProcedure("INFTEMP.CONV_" + workTypeArray[1] + "_PLATFORM") <= 0)
                                    {
                                        this.isError = true;
                                        this.errorType = "ERR";
                                        this.errorDetail = "执行存储过程时发生未知错误";
                                    }
                                }
                            }
                            else
                            {
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "LOG";
                                this.errorDetail = "无数据获取";
                            }
                        }
                        else
                        {
                            this.total = -1;
                            this.isError = true;
                            this.errorType = "ERR";
                            this.errorDetail = "用户信息不合法，登陆失败";
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }

                    save();
                    break;
                #endregion

                #region 采配单状态下传
                case "DAT_CPDSTATUS":
                    this.workType = "INF_" + this.orgWorkType;
                    DataTable t_dat_cpdstatus = queryDAT_CPDSTATUS();
                    if (t_dat_cpdstatus == null || t_dat_cpdstatus.Rows.Count < 1)
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                        save();
                        break;
                    }
                    string seqnosCPDSTATUS = "";
                    foreach (DataRow dr in t_dat_cpdstatus.Rows)
                    {
                        seqnosCPDSTATUS += "'" + dr["seqno"] + "',";
                    }
                    seqnosCPDSTATUS = seqnosCPDSTATUS.TrimEnd(',');

                    JObject resultCPDSTATUS = null;
                    if (!String.IsNullOrEmpty(seqnosCPDSTATUS))
                    {
                        resultCPDSTATUS = ApiClientService.query(this.orgWorkType, seqnosCPDSTATUS);
                        if (resultCPDSTATUS != null)
                        {
                            if (resultCPDSTATUS.Value<JArray>("data") != null)
                            {
                                if (resultCPDSTATUS.Value<JArray>("data").Count == 0)
                                {
                                    this.total = -1;
                                    this.isError = true;
                                    this.errorDetail = "无数据获取";
                                    this.errorType = "LOG";
                                }
                                else
                                {
                                    export(this.workType, resultCPDSTATUS);
                                    if (ApiClientUtil.runProcedure("INFTEMP.CONV_" + workTypeArray[1] + "_PLATFORM") <= 0)
                                    {
                                        this.isError = true;
                                        this.errorType = "ERR";
                                        this.errorDetail = "执行存储过程时发生未知错误";
                                    }
                                }
                            }
                            else
                            {
                                this.total = -1;
                                this.isError = true;
                                this.errorType = "LOG";
                                this.errorDetail = "无数据获取";
                            }
                        }
                        else
                        {
                            this.total = -1;
                            this.isError = true;
                            this.errorType = "ERR";
                            this.errorDetail = "用户信息不合法，登陆失败";
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }

                    save();
                    break;
                #endregion

                #region ERP订单执行情况预警数据上午下传
                case "DAT_DD_REMINDAM":
                    this.workType = "INF_DAT_DD_REMIND";
                    result = ApiClientService.query(this.orgWorkType, this.paraTime.ToString("yyyy/MM/dd HH:mm:ss"));
                    if (result != null)
                    {
                        if (result.Value<JArray>("data") != null)
                        {
                            if (result.Value<JArray>("data").Count == 0)
                            {
                                this.total = -1;
                                this.isError = true;
                                this.errorDetail = "无数据获取";
                                this.errorType = "LOG";
                            }
                            else
                            {
                                export(this.workType, result);
                                if (ApiClientUtil.runProcedure("INFTEMP.CONV_DD_REMIND_PLATFORM") <= 0)
                                {
                                    this.isError = true;
                                    this.errorType = "ERR";
                                    this.errorDetail = "执行存储过程时发生未知错误";
                                }
                            }
                        }
                        else
                        {
                            this.total = -1;
                            this.isError = true;
                            this.errorType = "LOG";
                            this.errorDetail = "无数据获取";
                        }
                    }
                    else
                    {
                        this.total = -1;
                        this.isError = true;
                        this.errorType = "ERR";
                        this.errorDetail = "用户信息不合法，登陆失败";
                    }
                    save();
                    break;
                #endregion

                #region ERP订单执行情况预警数据下午下传
                case "DAT_DD_REMINDPM":
                    this.workType = "INF_DAT_DD_REMIND";
                    result = ApiClientService.query(this.orgWorkType, this.paraTime.ToString("yyyy/MM/dd HH:mm:ss"));
                    if (result != null)
                    {
                        if (result.Value<JArray>("data") != null)
                        {
                            if (result.Value<JArray>("data").Count == 0)
                            {
                                this.total = -1;
                                this.isError = true;
                                this.errorDetail = "无数据获取";
                                this.errorType = "LOG";
                            }
                            else
                            {
                                export(this.workType, result);
                                if (ApiClientUtil.runProcedure("INFTEMP.CONV_DD_REMIND_PLATFORM") <= 0)
                                {
                                    this.isError = true;
                                    this.errorType = "ERR";
                                    this.errorDetail = "执行存储过程时发生未知错误";
                                }
                            }
                        }
                        else
                        {
                            this.total = -1;
                            this.isError = true;
                            this.errorType = "LOG";
                            this.errorDetail = "无数据获取";
                        }
                    }
                    else
                    {
                        this.total = -1;
                        this.isError = true;
                        this.errorType = "ERR";
                        this.errorDetail = "用户信息不合法，登陆失败";
                    }
                    save();
                    break;
                #endregion

                #region 订单执行情况预警短信重发
                case "DDREMIND_SEND":

                    DataTable t_dd_remind = queryDAT_DD_REMIND();
                    if (t_dd_remind == null || t_dd_remind.Rows.Count < 1)
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                        save();
                        break;
                    }
                    string sendState = "";
                    foreach (DataRow dr in t_dd_remind.Rows)
                    {
                        sendState = ApiClientService.SendMessage(dr["MSGCONTENT"].ToString(), dr["STAFFTEL"].ToString(), "", "", "自动程序", "ERP", "9");
                        //处理返回值
                        string strFlag = "E";//Y-发送成功；E-发送不成功
                        if (!String.IsNullOrEmpty(sendState))
                        {
                            strFlag = "S";
                        }

                        //更新标志位
                        updateDAT_DD_REMIND(strFlag, dr["SEQNO"].ToString());
                    }

                    save();
                    break;
                #endregion

                #region 商品资料调整单上传
                case "DAT_TZGOODS_DOC":
                    //上传订单
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    this.errorType = "LOG";
                    DataTable dtTZGOODS = queryTZGOODS();
                    JArray jArrayTZGOODS = new JArray();
                    JArray jArrayTZGOODSBody = new JArray();
                    foreach (DataRow dr in dtTZGOODS.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        jArrayTZGOODS.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                        DataTable dtBody = queryTZGOODSCom(dr["SEQNO"].ToString());
                        foreach (DataRow drBody in dtBody.Rows)
                        {
                            String[] paramArrayBody = new String[drBody.ItemArray.Length];
                            for (int j = 0; j < drBody.ItemArray.Length; j++)
                            {
                                paramArrayBody[j] = drBody.ItemArray[j].ToString();
                            }
                            jArrayTZGOODSBody.Add(JArray.FromObject(ApiClientService.renderParams(paramArrayBody)));
                        }
                    }
                    if (jArrayTZGOODS.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill(this.orgWorkType, jArrayTZGOODS, jArrayTZGOODSBody, null);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updateDD(ddFlagY, "Y");
                                this.execCount = jArrayTZGOODS.Count;
                                this.total = jArrayTZGOODS.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                    this.errorType = "ERR";
                                }
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }

                    save();
                    break;
                #endregion

                #region 供应商资料调整单上传
                case "DAT_TZSUP_DOC":
                    //上传订单
                    this.workTime = DateTime.Now;
                    this.errorDetail = "";
                    this.errorType = "LOG";
                    DataTable dtTZSUP = queryTZSUP();
                    JArray jArrayTZSUP = new JArray();
                    JArray jArrayTZSUPBody = new JArray();
                    foreach (DataRow dr in dtTZSUP.Rows)
                    {
                        String[] paramArray = new String[dr.ItemArray.Length];
                        for (int i = 0; i < dr.ItemArray.Length; i++)
                        {
                            paramArray[i] = dr.ItemArray[i].ToString();
                        }
                        jArrayTZSUP.Add(JArray.FromObject(ApiClientService.renderParams(paramArray)));
                        DataTable dtBody = queryTZSUPCom(dr["SEQNO"].ToString());
                        foreach (DataRow drBody in dtBody.Rows)
                        {
                            String[] paramArrayBody = new String[drBody.ItemArray.Length];
                            for (int j = 0; j < drBody.ItemArray.Length; j++)
                            {
                                paramArrayBody[j] = drBody.ItemArray[j].ToString();
                            }
                            jArrayTZSUPBody.Add(JArray.FromObject(ApiClientService.renderParams(paramArrayBody)));
                        }
                    }
                    if (jArrayTZSUP.Count > 0)
                    {
                        result = ApiClientService.execRenderedBill(this.orgWorkType, jArrayTZSUP, jArrayTZSUPBody, null);
                        if (result != null)
                        {
                            if (result.Value<Boolean>("data") && "success".Equals(result.Value<String>("result")))
                            {
                                updateDD(ddFlagY, "Y");
                                this.execCount = jArrayTZSUP.Count;
                                this.total = jArrayTZSUP.Count;
                            }
                            else
                            {
                                this.isError = true;
                                this.total = -1;
                                if (result.GetValue("reason") != null)
                                {
                                    this.errorDetail += Convert.ToString(result.GetValue("reason")) + ". ";
                                    this.errorType = "ERR";
                                }
                            }
                        }
                        else
                        {
                            this.errorDetail = "用户信息不合法，登陆失败";
                            this.errorType = "ERR";
                            this.isError = true;
                            this.total = -1;
                        }
                    }
                    else
                    {
                        this.errorDetail = "没有符合条件的数据";
                        this.errorType = "LOG";
                        this.isError = true;
                        this.total = -1;
                    }

                    save();
                    break;
                #endregion

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

        #region 保存日志 更新记录
        private void save()
        {

            //MyTable log = new MyTable("DAT_INFDATA_LOG");
            //MyTable def = new MyTable("DAT_INFDATA_DEF");

            //TODO save record to dat_infdata_def and dat_infdata_log
            //this.exectime = DateTime.Now - this.startTime;
            //string strIsError = this.isError ? "FAIL" : "SUCCESS";

            //string sqlLog = "insert into dat_infdata_log(execrq,pararq,execcount,exectime,execmemo) values (to_date('" + DateTime.Now + "','yyyy/mm/dd hh24:mi:ss'),to_date('" + this.paraTime + "','yyyy/mm/dd hh24:mi:ss')," + this.execCount + "," + this.exectime.TotalSeconds + ",'" + this.orgWorkType + " " + strIsError + " " + this.errorDetail + "')";
            //DbHelperOra.ExecuteSql(sqlLog);

            //string sqlDef = @"update dat_infdata_def set execrq=to_date('" + this.workTime + "','yyyy/mm/dd hh24:mi:ss'),execcount = " + this.execCount + ",exectime = " + this.exectime.TotalSeconds + ",execmemo='" + this.orgWorkType + " " + strIsError + " " + this.errorDetail + "' where inftype = '" + this.orgWorkType + "'";
            //if (this.isError)
            //{
            //    sqlDef = @"update dat_infdata_def set execcount = " + this.execCount + ",exectime = " + this.exectime.TotalSeconds + ",execmemo='" + this.orgWorkType + " " + strIsError + " " + this.errorDetail + "' where inftype = '" + this.orgWorkType + "'";
            //}
            //DbHelperOra.ExecuteSql(sqlDef);
            this.exectime = DateTime.Now - this.startTime;
            string strIsError = this.isError ? "FAIL" : "SUCCESS";

            //string sqlLog = "insert into dat_infdata_log(execrq,pararq,execcount,exectime,execmemo) values (to_date('" + DateTime.Now + "','yyyy/mm/dd hh24:mi:ss'),to_date('" + this.paraTime + "','yyyy/mm/dd hh24:mi:ss')," + this.execCount + "," + this.exectime.TotalSeconds + ",'" + this.orgWorkType + " " + strIsError + " " + this.errorDetail + "')";
            string errMsg = this.orgWorkType + " " + strIsError + " " + this.errorDetail;
            string sqlLog = "insert into dat_infdata_log(execrq,pararq,execcount,exectime,execmemo,memo,errortype) values (to_date('" + DateTime.Now + "','yyyy/mm/dd hh24:mi:ss'),to_date('" + this.paraTime + "','yyyy/mm/dd hh24:mi:ss')," + this.execCount + "," + this.exectime.TotalSeconds + ",:execmemo,:memo,'"+this.errorType+"')";
            string sqlDef = @"update dat_infdata_def set EXECING='N',execrq=to_date('" + this.workTime + "','yyyy/mm/dd hh24:mi:ss'),execcount = " + this.execCount + ",exectime = " + this.exectime.TotalSeconds + ",execmemo=:execmemo,memo=:memo where inftype = '" + this.orgWorkType + "'";
            if (this.isError)
            {
                sqlDef = @"update dat_infdata_def set EXECING='N',execcount = " + this.execCount + ",exectime = " + this.exectime.TotalSeconds + ",execmemo=:execmemo,memo=:memo where inftype = '" + this.orgWorkType + "'";
            }
            List<CommandInfo> list = new List<CommandInfo>();

            OracleParameter[] param = {
                                          new OracleParameter("execmemo", OracleDbType.Clob,ParameterDirection.Input),
                                          new OracleParameter("memo", OracleDbType.Varchar2,100,ParameterDirection.Input)
                                      };
            param[0].Value = errMsg;
            param[1].Value = errMsg.Length > 50 ? errMsg.Substring(0, 50) : errMsg;

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

            //MyLog.getLogger().Info(this.orgWorkType + " | " + strIsError + " | " + this.execCount + "条已执行 | " + "执行时间 " + this.exectime.TotalSeconds + " 秒 | 服务器时间" + this.workTime + "\r\n" + this.errorDetail);

        }

        public void save(String errorMsg)
        {
            //this.exectime = DateTime.Now - this.startTime;
            //string strIsError = "FAIL";
            //this.total = -1;
            //// string sqlLog = "insert into dat_infdata_log(execrq,pararq,execcount,exectime,execmemo) values (to_date('" + DateTime.Now + "','yyyy/mm/dd hh24:mi:ss'),to_date('" + this.paraTime + "','yyyy/mm/dd hh24:mi:ss')," + this.execCount + "," + this.exectime.TotalSeconds + ",'" + this.orgWorkType + " " + strIsError + " " + errorMsg + "')";
            //string sqlLog = "insert into dat_infdata_log(execrq,pararq,execcount,exectime,execmemo) values (to_date('" + DateTime.Now + "','yyyy/mm/dd hh24:mi:ss'),to_date('" + this.paraTime + "','yyyy/mm/dd hh24:mi:ss')," + this.execCount + "," + this.exectime.TotalSeconds + ",'" + this.orgWorkType + " " + strIsError + " " + errorMsg.Replace("'", "\"") + "')";

            //DbHelperOra.ExecuteSql(sqlLog);
            ////不更改上次执行时间
            //string sqlDef = @"update dat_infdata_def set execcount = " + this.execCount + ",exectime = " + this.exectime.TotalSeconds + ",execmemo='" + this.orgWorkType + " " + strIsError + " " + errorMsg.Replace("'", "\"") + "' where inftype = '" + this.orgWorkType + "'";
            //DbHelperOra.ExecuteSql(sqlDef);


            this.exectime = DateTime.Now - this.startTime;
            string strIsError = "FAIL";
            this.total = -1;
            string errMsg = this.orgWorkType + " " + strIsError + " " + errorMsg.Replace("'", "\"");
            string sqlLog = "insert into dat_infdata_log(execrq,pararq,execcount,exectime,execmemo,memo,errortype) values (to_date('" + DateTime.Now + "','yyyy/mm/dd hh24:mi:ss'),to_date('" + this.paraTime + "','yyyy/mm/dd hh24:mi:ss')," + this.execCount + "," + this.exectime.TotalSeconds + ",:execmemo,:memo,'" + this.errorType + "')";
            string sqlDef = @"update dat_infdata_def set EXECING='N',execrq=to_date('" + this.workTime + "','yyyy/mm/dd hh24:mi:ss'),execcount = " + this.execCount + ",exectime = " + this.exectime.TotalSeconds + ",execmemo=:execmemo,memo=:memo where inftype = '" + this.orgWorkType + "'";
            if (this.isError)
            {
                sqlDef = @"update dat_infdata_def set EXECING='N',execcount = " + this.execCount + ",exectime = " + this.exectime.TotalSeconds + ",execmemo=:execmemo,memo=:memo where inftype = '" + this.orgWorkType + "'";
            }
            List<CommandInfo> list = new List<CommandInfo>();

            OracleParameter[] param = {
                                          new OracleParameter("execmemo", OracleDbType.Clob),
                                          new OracleParameter("memo", OracleDbType.Varchar2)
                                      };
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = errMsg;
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = errMsg.Length > 50 ? errMsg.Substring(0, 50) : errMsg;

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

            //MyLog.getLogger().Info(this.orgWorkType + " | " + strIsError + " | " + this.execCount + "条已执行 | " + "执行时间 " + this.exectime.TotalSeconds + " 秒 | 服务器时间" + this.workTime + "\r\n" + this.errorDetail);
        }
        #endregion

        #region 查询已上传未确认订单 //flag = 'S'
        /// <summary>
        /// 查询已上传未确认订单
        /// </summary>
        /// <returns></returns>
        private Boolean queryRK()
        {
            JObject jo = new JObject();
            this.ddFlagS = new DataTable();
            //String sql = "select * from dat_dd_doc where flag = 'Y' and ISSEND = 'Y' and isend='N' and xdrq>to_date('"+DateTime.Now+"','yyyy/mm/dd hh24:mi:ss')-"+ApiClientUtil.GetConfigCont("BILLTIMEOUT"); //已上传未确认订单
            String sql = @"select * from dat_dd_doc ddmain where exists(
                            select * from
                            ( 
                            select distinct(ddc.gdseq),ddc.seqno,sum(dhs) as sumdhs  from dat_dd_com ddc 
                            group by ddc.gdseq,ddc.seqno
                            ) dd
                            left join dat_rk_doc rkmain on dd.seqno = rkmain.ddbh
                            left join (
                                 select distinct(gdseq),sum(sssl) sumsssl,seqno from dat_rk_com rkcom

                                   group by gdseq,seqno
                            ) rk on dd.gdseq = rk.gdseq and rkmain.seqno = rk.seqno
                            where dd.seqno = ddmain.seqno and (dd.sumdhs <> rk.sumsssl or rk.sumsssl is null)
                        ) and flag = 'Y' and issend = 'Y' and isend = 'N' and xdrq>to_date('" + DateTime.Now + "','yyyy/mm/dd hh24:mi:ss')-" + billTimeOut;

            DataTable mResult = null;
            try
            {
                mResult = DbHelperOra.Query(sql).Tables[0];
            }
            catch (Exception e)
            {
                this.errorType = "ERR";
                this.isError = true;
                this.total = -1;
                save(e.Message + "|" + e.StackTrace);
                return false;
            }
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
        private Boolean insertBill(String type, JObject data)
        {

            if (data != null)
            {
                this.workTime = Convert.ToDateTime(data.GetValue("time"));

                if (data.GetValue("result").ToString().Equals("success"))
                {
                    String headName = "DAT_" + type + "_DOC";
                    String bodyName = "DAT_" + type + "_COM";
                    JObject result = data.Value<JObject>("data");
                    JArray heads = result.Value<JArray>("heads");
                    if (heads != null)
                    {
                        this.workType = "INF_DAT_" + type + "_DOC";
                        exportBill("INF_DAT_" + type + "_DOC", heads);
                        JArray bodies = result.Value<JArray>("bodies");
                        this.workType = "INF_DAT_" + type + "_COM";
                        exportBill("INF_DAT_" + type + "_COM", bodies);

                        JArray exts = result.Value<JArray>("billexts");
                        if (exts != null&&exts.Count!=0)
                        {
                            this.workType = "INF_DAT_" + type + "_EXT";
                            exportBill("INF_DAT_" + type + "_EXT", exts);
                        }
                    }
                }
                else
                {
                    this.isError = true;
                    this.errorType = "ERR";
                    this.errorDetail += data.GetValue("reason").ToString();
                }
            }

            return !this.isError;
        }


        //private Boolean exportBill(JArray result)
        //{
        //    DataTable dResult = new DataTable();
        //    DataTable tableSchema = ApiClientUtil.GetTableSchema(this.workType);
        //    dResult = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(result));
        //    foreach (DataColumn dc in tableSchema.Columns)
        //    {
        //        String name = dc.ColumnName;
        //        int index = tableSchema.Columns.IndexOf(dc);
        //        dResult.Columns[name].SetOrdinal(index);
        //    }
        //    dResult.Columns.Remove("CUSTID");
        //    int oraResult = ApiClientUtil.BulkInsert(this.workType, dResult);
        //    if (oraResult > 0)
        //    {
        //        if (this.workType.Split('_')[2].Equals("DOC"))
        //        {
        //            this.execCount += dResult.Rows.Count;
        //        }
        //    }
        //    else
        //    {
        //        this.isError = true;
        //        this.errorDetail += " 插入数据时发生未知错误 ";
        //        this.errorType = "ERR";
        //    }

        //    return true;
        //}
        private Boolean exportBill(string tableName, JArray result)
        {
            DbHelperOra.ExecuteSql("TRUNCATE TABLE " + tableName);
            DataTable tableSchema = ApiClientUtil.GetTableSchema(tableName);
            string columnNames = "";
            string columnValues = "";
            string sql = "";
            StringBuilder builder = new StringBuilder();
            builder.Append(" BEGIN ");
            foreach (JToken jt in result)
            {
                JObject joHead = (JObject)jt;
                columnNames = "";
                columnValues = "";
                foreach (DataColumn dc in tableSchema.Columns)
                {
                    string colType = dc.DataType.ToString();
                    string colName = dc.ColumnName;
                    columnNames += colName + ",";
                    if (joHead.Value<String>(colName) == null || "".Equals(joHead.Value<String>(colName)))
                    {
                        columnValues += "null,";
                    }
                    else if ("System.String".Equals(colType))
                    {
                        columnValues += "'" + joHead.Value<String>(colName) + "',";
                    }
                    else if ("System.DateTime".Equals(colType))
                    {
                        columnValues += "TO_DATE('" + joHead.Value<String>(colName) + "','mm/dd/yyyy hh24:mi:ss'),";
                    }
                    else if ("System.Decimal".Equals(colType))
                    {
                        columnValues += joHead.Value<String>(colName) + ",";
                    }
                    else if ("System.Int32".Equals(colType))
                    {
                        columnValues += joHead.Value<String>(colName) + ",";
                    }
                    else
                    {
                        columnValues += "'" + joHead.Value<String>(colName) + "',";
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
            bool oraResult = DbHelperOra.ExecuteSqlTran(cmdList);
            if (oraResult)
            {
                if (tableName.Split('_')[3].Equals("DOC"))
                {
                    this.execCount += result.Count;
                }
            }
            else
            {
                this.isError = true;
                this.errorDetail += " 插入数据时发生未知错误 ";
                this.errorType = "ERR";
            }

            return true;
        }
        #endregion

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
                seqNoOld += "'" + dr["SEQNO"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            if (seqNoOld.Length > 0)
            {
                String sql = "update DAT_DD_DOC set ISSEND = '" + flag + "' where  seqno in (" + seqNoOld + ")";
                DbHelperOra.ExecuteSql(sql);
            }

            return true;
        }

        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="billno">订单号</param>
        /// <param name="flag">flag值，S 已上传/F已确认</param>
        /// <returns></returns>
        private Boolean updateDD(String billno, String flag)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SEQNO", typeof(String));
            DataRow dr = dt.NewRow();
            dr["SEQNO"] = billno;
            dt.Rows.Add(dr);
            return updateDD(dt, flag);
        }
        #endregion

        #region 更新退货单
        /// <summary>
        /// 批量更新订单
        /// </summary>
        /// <param name="dt">保存订单记录的datatable</param>
        /// <param name="flag">flag值</param>
        /// <returns></returns>
        private Boolean updateTH(DataTable dt, String flag)
        {
            String seqNoOld = "";
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            if (seqNoOld.Length > 0)
            {
                String sql = "update DAT_TH_DOC set ISSEND = '" + flag + "' where  seqno in (" + seqNoOld + ")";
                DbHelperOra.ExecuteSql(sql);
            }

            return true;
        }

        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="billno">订单号</param>
        /// <param name="flag">flag值</param>
        /// <returns></returns>
        private Boolean updateTH(String billno, String flag)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SEQNO", typeof(String));
            DataRow dr = dt.NewRow();
            dr["SEQNO"] = billno;
            dt.Rows.Add(dr);
            return updateTH(dt, flag);
        }
        #endregion

        #region 更新结算单
        /// <summary>
        /// 批量更新结算单
        /// </summary>
        /// <param name="dt">保存订单记录的datatable</param>
        /// <param name="flag">flag值</param>
        /// <returns></returns>
        private Boolean updateJSD(DataTable dt, String flag)
        {
            String seqNoOld = "";
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            if (seqNoOld.Length > 0)
            {
                String sql = "update DAT_JSD_DOC set FLAG = '" + flag + "' where  seqno in (" + seqNoOld + ")";
                DbHelperOra.ExecuteSql(sql);
            }

            return true;
        }

        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="billno">订单号</param>
        /// <param name="flag">flag值</param>
        /// <returns></returns>
        private Boolean updateJSD(String billno, String flag)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SEQNO", typeof(String));
            DataRow dr = dt.NewRow();
            dr["SEQNO"] = billno;
            dt.Rows.Add(dr);
            return updateJSD(dt, flag);
        }
        #endregion

        #region 更新批号调整单
        /// <summary>
        /// 更新批号调整单
        /// </summary>
        /// <param name="dt">保存订单记录的datatable</param>
        /// <param name="flag">flag值</param>
        /// <returns></returns>
        private Boolean updatePHTZ(DataTable dt, String flag)
        {
            String seqNoOld = "";
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            if (seqNoOld.Length > 0)
            {
                String sql = "update DAT_PHTZ_DOC set ISSEND = '" + flag + "' where  seqno in (" + seqNoOld + ")";
                DbHelperOra.ExecuteSql(sql);
            }

            return true;
        }

        /// <summary>
        /// 更新批号调整单
        /// </summary>
        /// <param name="billno">订单号</param>
        /// <param name="flag">flag值</param>
        /// <returns></returns>
        private Boolean updatePHTZ(String billno, String flag)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SEQNO", typeof(String));
            DataRow dr = dt.NewRow();
            dr["SEQNO"] = billno;
            dt.Rows.Add(dr);
            return updatePHTZ(dt, flag);
        }
        #endregion

        #region 更新损益单
        /// <summary>
        /// 批量更新订单
        /// </summary>
        /// <param name="dt">保存订单记录的datatable</param>
        /// <param name="flag">flag值</param>
        /// <returns></returns>
        private Boolean updateSY(DataTable dt, String flag)
        {
            String seqNoOld = "";
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            if (seqNoOld.Length > 0)
            {
                String sql = "update DAT_SY_DOC set ISSEND = '" + flag + "' where  seqno in (" + seqNoOld + ")";
                DbHelperOra.ExecuteSql(sql);
            }

            return true;
        }

        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="billno">订单号</param>
        /// <param name="flag">flag值</param>
        /// <returns></returns>
        private Boolean updateSY(String billno, String flag)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SEQNO", typeof(String));
            DataRow dr = dt.NewRow();
            dr["SEQNO"] = billno;
            dt.Rows.Add(dr);
            return updateSY(dt, flag);
        }
        #endregion

        #region 更新出库单
        private Boolean updateCK(DataTable dt, String flag)
        {
            String seqNoOld = "";
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            if (seqNoOld.Length > 0)
            {
                String sql = "update DAT_CK_DOC set ISSEND = '" + flag + "' where  seqno in (" + seqNoOld + ")";
                DbHelperOra.ExecuteSql(sql);
            }

            return true;
        }
        #endregion

        #region 更新唯一码跟踪报表
        /// <summary>
        /// 更新唯一码跟踪报表
        /// </summary>
        /// <param name="dt">保存订单记录的datatable</param>
        /// <param name="flag">flag值</param>
        /// <returns></returns>
        private Boolean updateDat_onecodejxc(DataTable dt, String flag)
        {
            String seqNoOld = "";
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            if (seqNoOld.Length > 0)
            {
                String sql = "update DAT_ONECODEJXC set ISSEND = '" + flag + "' where  seqno in (" + seqNoOld + ")";
                DbHelperOra.ExecuteSql(sql);
            }

            return true;
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
            //JArray ja = new JArray();
            String sql = @"select seqno, billno, billtype, flag, isend, issend, isauto, deptdh, deptid, pssid, pssname, nvl(xdrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')), dhfs,  nvl(subnum,0), nvl(subsum,0), F_GETUSERNAME(cgy) cgy,F_GETUSERNAME(lry) lry, nvl(lrrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),  shr, nvl(shrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),memo, nvl(dhrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),DHLX,ISYX,SUBNUM,SUBSUM,F_GETUSERNAME(cgy),F_GETUSERNAME(lry),STR1,STR3,STR4,STR5  
                            from dat_dd_doc 
                            where ( flag = 'Y' OR FLAG = 'G') and issend = 'N' and pssid in (select SUPID from doc_supplier WHERE ISSEND='Y')";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            this.ddFlagY = mResult;
            return mResult;
        }

        private DataTable queryDDCom(String seqno)
        {
            String sql = "select seqno, rowno,  gdseq, barcode, gdname, F_GETUNITNAME(unit) unit, gdspec, supid,catid,  nvl(bzhl,1), nvl(bzsl,0), nvl(dhs,0), nvl(dhsl,0), nvl(jxtax,0), nvl(hsjj,0), nvl(bhsjj,0),nvl(hsje,0), nvl(bhsje,0), nvl(lsj,0), nvl(lsje,0), isgz, islot,phid,ph,pzwh,nvl(rq_sc,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),nvl(yxqz,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),F_GETPRODUCERNAME(producer) producer,memo,nvl(num2,0),nvl(firsttime, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')),nvl(lasttime, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')),UNITSMALL from V_DD_COM where seqno = '" + seqno + "'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        private DataTable queryDDC()
        {
            this.ddFlagY = new DataTable();
            //JArray ja = new JArray();
            String sql = @"select seqno, billno, billtype, flag, isend, issend, isauto, deptdh, deptid, pssid, pssname, nvl(xdrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')), dhfs,  nvl(subnum,0), nvl(subsum,0), cgy, lry, nvl(lrrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),  shr, nvl(shrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),memo, nvl(dhrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),DHLX,ISYX,SUBNUM,SUBSUM  
                            from dat_dd_doc 
                            where ( flag = 'Y' OR FLAG = 'G') and issend = 'N' and pssid in (select SUPID from doc_supplier WHERE ISSEND='Y')";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            this.ddFlagY = mResult;
            return mResult;
        }

        private DataTable queryDDCCom(String seqno)
        {
            String sql = "select seqno, rowno,  F_GETEASCODEBYGDSEQ(gdseq) gdseq, barcode, gdname, F_GETUNITNAME(unit) unit, gdspec, supid,catid,  nvl(bzhl,1), nvl(bzsl,0), nvl(dhs,0), nvl(dhsl,0), nvl(jxtax,0), nvl(hsjj,0), nvl(bhsjj,0),nvl(hsje,0), nvl(bhsje,0), nvl(lsj,0), nvl(lsje,0), isgz, islot,phid,ph,pzwh,nvl(rq_sc,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),nvl(yxqz,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),producer,memo,nvl(num2,0),nvl(firsttime, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')),nvl(lasttime, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')) from dat_dd_com where seqno = '" + seqno + "'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        //办公用品订单表体
        private DataTable queryDDBCom(String seqno)
        {
            String sql = "select seqno, rowno,  gdseq, barcode, gdname, unit, gdspec, supid,catid,  nvl(bzhl,1), nvl(bzsl,0), nvl(dhs,0), nvl(dhsl,0), nvl(jxtax,0), nvl(hsjj,0), nvl(bhsjj,0),nvl(hsje,0), nvl(bhsje,0), nvl(lsj,0), nvl(lsje,0), isgz, islot,phid,ph,pzwh,nvl(rq_sc,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),nvl(yxqz,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),producer,memo,nvl(num2,0),nvl(firsttime, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')),nvl(lasttime, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')) from dat_dd_com where seqno = '" + seqno + "'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        private DataTable queryDDExt(string seqno)
        {
            String sql = "select deptid, billno, rowno, onecode, gdseq, gdname, barcode, unit, gdspec, ph, nvl(rq_sc, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')), nvl(yxqz, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')), nvl(kpxq, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')),deptcur,billck,billsy, flag,nvl(operdate, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')), operuser, optdate,patient,optid,optdoctor,str1,str2,str3,str4,str5,bzhl,nvl(instime, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')) from dat_dd_ext where BILLNO = '" + seqno + "'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        #endregion

        #region  查询入库单
        private DataTable queryRKD()
        {
            String sql = @"select seqno,subnum,subsum,shr,shrq,nvl(num2,0),nvl(num3,0) from dat_rk_doc
                            where flag='Y' and issend='N' and substr(seqno,1,3)='GCK'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        private DataTable queryRKCom(String seqno)
        {
            String sql = "select seqno,rowno,deptid,gdseq,gdname,unit,gdspec,bzhl,bzsl,ddsl,sssl,hsjj,hsje from dat_rk_com where seqno = '" + seqno + "'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        private DataTable queryRKExt(string seqno)
        {
            String sql = "select deptid, billno, rowno, onecode, gdseq, gdname, barcode, unit, gdspec, ph, nvl(rq_sc, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')), nvl(yxqz, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')), nvl(kpxq, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')),deptcur,billck,billsy, flag,nvl(operdate, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')), operuser, optdate,patient,optid,optdoctor,str1,str2,str3,str4,str5,bzhl,nvl(instime, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')) from dat_rk_ext where BILLNO = '" + seqno + "'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        private Boolean updateRK(DataTable dt, String flag)
        {
            String seqNoOld = "";
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            if (seqNoOld.Length > 0)
            {
                String sql = "update DAT_RK_DOC set ISSEND = '" + flag + "' where  seqno in (" + seqNoOld + ")";
                DbHelperOra.ExecuteSql(sql);
            }

            return true;
        }
        #endregion

        #region  查询预入库单
        private DataTable queryYRKD()
        {
            String sql = @"select seqno,subnum,subsum,shr,shrq from dat_yrk_doc
                            where flag='G' and issend='N'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        private DataTable queryYRKCom(String seqno)
        {
            String sql = "select seqno,rowno,deptid,gdseq,gdname,unit,gdspec,bzhl,bzsl,ddsl,sssl,hsjj,hsje from dat_yrk_com where seqno = '" + seqno + "'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        private Boolean updateYRK(DataTable dt, String flag)
        {
            String seqNoOld = "";
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            if (seqNoOld.Length > 0)
            {
                String sql = "update DAT_YRK_DOC set ISSEND = '" + flag + "' where  seqno in (" + seqNoOld + ")";
                DbHelperOra.ExecuteSql(sql);
            }

            return true;
        }
        #endregion

        #region 查询出库单
        private DataTable queryCK()
        {
            this.ckTable = new DataTable();
            String sql = "select seqno,billno,billtype,flag,deptout,deptid, slr, xstype,nvl(xsrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),thtype, nvl(subnum,0), lry,lrrq,memo, issend,SHRQ,nvl(num2,0) from dat_ck_doc where flag='G' and issend='N' and shr is not null and shrq is not null";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            this.ckTable = mResult;
            return mResult;
        }
        private DataTable queryCKCom(String seqno)
        {
            String sql = "select seqno,  rowno, gdseq,  barcode, gdname, unit, gdspec,  hwid,nvl( bzhl,1), nvl(bzsl,0), nvl(dhsl,0), nvl(xssl,0), nvl(jxtax,0), nvl(hsjj,0), nvl(bhsjj,0), nvl(hsje,0), nvl(bhsje,0), isgz, islot, phid, ph,pzwh, producer, zpbh, str1, memo,'' from dat_ck_com where seqno='" + seqno + "'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        #endregion

        #region 销售单操作
        private DataTable queryXS()
        {
            this.ckTable = new DataTable();
            String sql = "select seqno,billno,billtype,flag,nvl(f_getpara('DEFDEPT'),deptid) deptout,deptid, lry slr, xstype,nvl(xsrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),'' thtype,(select count(1) from VIEW_XS_COM where seqno=billno) subnum, lry,lrrq,memo, issend,SHRQ,nvl(num2,0) from dat_xs_doc where flag='G' and issend='N' and shr is not null and shrq is not null ";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            this.ckTable = mResult;
            return mResult;
        }
        private DataTable queryXSCom(String seqno)
        {
            String sql = @"select B.seqno,rownum ROWNO,B.gdseq,B.barcode,B.gdname,B.unit,B.gdspec,B.hwid,nvl(B.bzhl,1),round(nvl(B.bzsl,0),4),nvl(B.dhsl,0),nvl(B.xssl,0),nvl(B.jxtax,0),nvl(B.hsjj,0),nvl(B.bhsjj,0),round(nvl(B.hsje,0),4),nvl(B.bhsje,0),B.isgz,B.islot,B.phid,B.ph,B.pzwh,B.producer,B.zpbh,B.str1,B.memo,B.rowno STR2
                            from VIEW_XS_COM B where seqno='" + seqno + "' and nvl(B.xssl,0)<>0";
            //String sql = @"select B.seqno,B.rowno,B.gdseq,B.barcode,B.gdname,B.unit,B.gdspec,B.hwid,nvl(B.bzhl,1),nvl(B.bzsl,0),nvl(B.dhsl,0),nvl(B.xssl,0),nvl(B.jxtax,0),nvl(B.hsjj,0),nvl(B.bhsjj,0),nvl(B.hsje,0),nvl(B.bhsje,0),B.isgz,B.islot,B.phid,B.ph,B.pzwh,B.producer,B.zpbh,B.str1,B.memo 
            //                from dat_xs_COM B,DAT_GOODSJXC C
             //               where B.SEQNO = C.BILLNO AND B.GDSEQ = C.GDSEQ AND B.ROWNO = C.ROWNO AND C.SUPID = '00002' and b.seqno='"+seqno+"'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        private int queryXSUPCount()
        {
            String sql = @"select count(*) num from dat_xs_doc
                            where flag='Y' AND ISSEND='N' ";
            int mResult = Convert.ToInt32(DbHelperOra.GetSingle(sql));
            return mResult;
        }

        private DataTable queryXSUP()
        {
            String sql = @"select SEQNO,BILLNO,BILLTYPE,FLAG,DEPTOUT,DEPTID,CATID,XSTYPE,XSRQ,SUBNUM,NVL(SUBSUM,0) SUBSUM,
                            LRY,LRRQ,SPR,nvl(SPRQ,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')) SPRQ,SHR,nvl(SHRQ,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')) SHRQ from dat_xs_doc
                            where flag='Y' AND ISSEND='N' AND ROWNUM<=200";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        private DataTable queryXSComUP(String seqno)
        {
            String sql = @"select SEQNO,ROWNO,GDSEQ,GDNAME,UNIT,GDSPEC,BZHL,BZSL,DHSL,XSSL,JXTAX,HSJJ,
                            HSJE,ISGZ,ISLOT,PHID,PH,PZWH,nvl(RQ_SC,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')) RQ_SC,nvl(YXQZ,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')) YXQZ,PRODUCER from dat_xs_com
                            WHERE SEQNO='" + seqno+"'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        private Boolean updateXS(DataTable dt, String flag)
        {
            String seqNoOld = "";
            int intCount = 0;
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
                intCount++;
                if (intCount > 200)
                {
                    if (seqNoOld.Length > 0)
                    {
                        String sql = "update DAT_XS_DOC set ISSEND = '" + flag + "' where  seqno in (" + seqNoOld.TrimEnd(',') + ")";
                        DbHelperOra.ExecuteSql(sql);
                        seqNoOld = "";
                        intCount = 0;
                    }
                }
            }

            if (seqNoOld.Length > 0)
            {
                String sql = "update DAT_XS_DOC set ISSEND = '" + flag + "' where  seqno in (" + seqNoOld + ")";
                DbHelperOra.ExecuteSql(sql);
            }

            return true;
        }
        #endregion

        #region 采购计划操作
        private DataTable queryCGPLAN()
        {
            String sql = @"SELECT SEQNO,BILLTYPE,BEGINTIME,ENDTIME,DEPTID,PSSID,XDRQ,SUBNUM,SUBSUM,CGY,
                            MEMO,LRY,LRRQ,SHR,SHRQ,nvl(NUM1,0) FROM v_ddplan_doc_inf WHERE ISSEND='N' ";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        private DataTable queryCGPLANCom(String seqno)
        {
            String sql = @"SELECT SEQNO,ROWNO,GDSEQ,GDNAME,F_GETUNITNAME(unit) UNIT,GDSPEC,SUPID,nvl(BZHL,1),nvl(BZSL,0),nvl(DHS,0),nvl(JXTAX,0),nvl(HSJJ,0),
                            nvl(HSJE,0),PZWH,PRODUCER,MINIUNIT FROM v_ddplan_com_inf where seqno='" + seqno + "'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        private Boolean updateCGPLAN(DataTable dt, String flag)
        {
            String seqNoOld = "";
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            if (seqNoOld.Length > 0)
            {
                String sql = "update DAT_DDPLAN_DOC set ISSEND = '" + flag + "' where  seqno in (" + seqNoOld + ")";
                DbHelperOra.ExecuteSql(sql);
            }

            return true;
        }
        #endregion

        #region 查询退货单
        private DataTable queryTH()
        {
            this.thTable = new DataTable();
            String sql = "select seqno, billno, billtype, flag,thtype, rkdh, deptdh, deptid, pssid,pssname,  nvl(thrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),  nvl(subnum,0), nvl(subsum,0), cgy, lry,  nvl(lrrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')), nvl( shrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),  memo,F_GETUSERNAME(cgy),F_GETUSERNAME(lry) from dat_th_doc where flag='Y' and issend='N' and shr is not null and shrq is not null and pssid in (select SUPID from doc_supplier WHERE ISSEND = 'Y')";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            this.thTable = mResult;
            return mResult;
        }
        private DataTable queryTHCom(String seqno)
        {
            String sql = "select seqno, rowno, deptid, gdseq, barcode, gdname,F_GETUNITNAME(unit) unit, gdspec, gdmode,supth supid, cdid,  kctype,  catid, hwid, nvl(bzhl,1), nvl(bzsl,0), nvl(spzt,0), nvl(thsl,0), nvl(sssl,0), nvl(jxtax,0), nvl(hsjj,0), nvl(bhsjj,0), nvl(hsje,0), nvl(bhsje,0), nvl(lsj,0), nvl(lsje,0), islot, phid,    ph,     pzwh,  nvl(rq_sc,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),  nvl(yxqz,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),  nvl(kcsl,0), nvl(kchsje,0), nvl(spztsl,0), nvl(ERPayxs,0), nvl(hlkc,0), zpbh, producer,  memo,unitname from V_TH_COM where seqno='" + seqno + "'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        private DataTable queryTHC()
        {
            this.thTable = new DataTable();
            String sql = @"select seqno,
                               billno,
                               billtype,
                               flag,
                               thtype,
                               rkdh,
                               deptdh,
                               deptid,
                               pssid,
                               pssname,
                               nvl(thrq, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')),
                               nvl(subnum, 0),
                               nvl(subsum, 0),
                               cgy,
                               lry,
                               nvl(lrrq, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')),
                               nvl(shrq, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')),
                               memo,
                               F_GETUSERNAME(cgy),
                               F_GETUSERNAME(lry)
                          from dat_th_doc d
                         where flag = 'Y'
                           and issend = 'N'
                           and shr is not null
                           and shrq is not null
                           and pssid in (select SUPID from doc_supplier WHERE ISSEND = 'Y')
                           and not EXISTS (
                                 SELECT * FROM DAT_TH_COM C,DOC_GOODS G
                              WHERE C.GDSEQ=G.GDSEQ AND G.ISGZ='Y' AND C.SEQNO=d.seqno
                           )";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            this.thTable = mResult;
            return mResult;
        }
        private DataTable queryTHComC(String seqno)
        {
            String sql = "select seqno, rowno, deptid, F_GETEASCODEBYGDSEQ(gdseq) gdseq, barcode, gdname,F_GETUNITNAME(unit) unit, gdspec, gdmode,supth supid, cdid,  kctype,  catid, hwid, nvl(bzhl,1), nvl(bzsl,0), nvl(spzt,0), nvl(thsl,0), nvl(sssl,0), nvl(jxtax,0), nvl(hsjj,0), nvl(bhsjj,0), nvl(hsje,0), nvl(bhsje,0), nvl(lsj,0), nvl(lsje,0), islot, phid,    ph,     pzwh,  nvl(rq_sc,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),  nvl(yxqz,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),  nvl(kcsl,0), nvl(kchsje,0), nvl(spztsl,0), nvl(ERPayxs,0), nvl(hlkc,0), zpbh, producer,  memo,unitname from V_TH_COM where seqno='" + seqno + "'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        #endregion

        #region 查询结算单
        private DataTable queryJSD()
        {
            String sql = @"SELECT SEQNO,
                                               CUSTID,
                                               SUPID,
                                               NVL(BEGRQ, TO_DATE('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')),
                                               ENDRQ,
                                               LRY,
                                               F_GETUSERNAME(LRY),
                                               SPRQ,
                                               YJJJ
                                          FROM DAT_JSD_DOC
                                         WHERE FLAG = 'Y'
                                           AND SUPID IN (SELECT SUPID
                                                           FROM DOC_SUPPLIER
                                                          WHERE FLAG = 'Y'
                                                            AND ISSEND = 'Y')";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        private DataTable queryJSDCom(String seqno)
        {
            String sql = @"SELECT str1,billno,billtype,rownum rowno,gdseq,gdname,F_GETUNITNAME(unit) unit,gdspec,bzhl,bzsl,
                            xssl,jxtax,hsjj,hsje,phid,ph,pzwh,rq_sc,yxqz,supid,rqsj,catid0,producer,KCSL,rowno xsrowno,KCSL_OLD,STR2 
                            FROM VIEW_XS_COM WHERE STR1='" + seqno + "' ORDER BY BILLNO,ROWNO";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        private DataTable queryJSDComC(String seqno)
        {
            String sql = @"SELECT str1,billno,billtype,rownum rowno,F_GETEASCODEBYGDSEQ(gdseq) gdseq,gdname,F_GETUNITNAME(unit) unit,gdspec,bzhl,bzsl,
                            xssl,jxtax,hsjj,hsje,phid,ph,pzwh,rq_sc,yxqz,supid,rqsj,catid0,producer,KCSL,rowno xsrowno,KCSL_OLD 
                            FROM VIEW_XS_COM WHERE STR1='" + seqno + "' ORDER BY BILLNO,ROWNO";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        #endregion

        #region 查询结算单
        private DataTable queryPHTZ()
        {
            String sql = @"SELECT SEQNO,BILLNO,BILLTYPE,FLAG,DEPTID,LRY,LRRQ,SHR,SHRQ,STR1,STR2,STR3,NUM1,NUM2,NUM3,MEMO,SPR,SPRQ,SUBSUM,SUBNUM,TPRQ,PSSID
  FROM DAT_PHTZ_DOC WHERE FLAG = 'Y' AND ISSEND='N'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        private DataTable queryPHTZCom(String seqno)
        {
            String sql = @"SELECT SEQNO,ROWNO,DEPTID,GDSEQ,GDNAME,UNIT,GDSPEC,HWID,BZHL,KCSL,SL,JXTAX,HSJJ,HSJE,PHID,PH,PZWH,RQ_SC,YXQZ,NEWPH,NEWYXQZ,PRODUCER FROM DAT_PHTZ_COM 
                            WHERE SEQNO='" + seqno + "' ORDER BY SEQNO,ROWNO";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        #endregion

        #region 查询损益单
        private DataTable querySY()
        {
            this.thTable = new DataTable();
            String sql = "select seqno, billno, billtype, flag, deptid, catid, kctype, sytype, scth, nvl(syrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')), nvl(subnum,0), nvl(subsum,0), cgy, lry,  nvl(lrrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')), spr, nvl(sprq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),  shr, nvl(shrq,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')),str1, str2, memo from dat_sy_doc where flag='Y' and issend='N' and shr is not null and shrq is not null and (str3 is null or str3 in (select supid from doc_supplier where isdg='N')) ";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            this.syTable = mResult;
            return mResult;
        }
        private DataTable querySYCom(String seqno)
        {
            String sql = "select seqno, rowno, hwid, gdseq, barcode, gdname, unit, gdspec, gdmode, nvl(bzhl,1), nvl(kcsl,0), nvl(kchsje,0), nvl(bzsl,0), nvl(sysl,0), nvl(jxtax,0), nvl(hsjj,0), nvl(bhsjj,0), nvl(hsje,0), nvl(bhsje,0), nvl(lsj,0),   nvl(lsje,0), isgz, islot,     phid,   ph,   pzwh, nvl(rq_sc,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')), nvl(yxqz,to_date('0001/01/01 00:00:00','yyyy/mm/dd hh24:mi:ss')), zpbh,   nvl(syhsje,0), nvl(sybhsje,0) from dat_sy_com where seqno='" + seqno + "'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        #endregion

        #region 查询系统操作日志
        private DataTable querySys_operlog()
        {
            String sql = "select to_char(EXECRQ,'yyyy-mm-dd') from dat_infdata_def where inftype='SYS_OPERLOG'";
            String result = (String)DbHelperOra.GetSingle(sql);
            DataTable mResult = null;
            //先判断当前时间减1天后是否大于上次上传时间
            if(DateTime.Compare(DateTime.Now.AddDays(-1),Convert.ToDateTime(result))>0)
            {
                sql = "select SEQNO,RQSJ,USERID,STATION,FUNCID,MEMO,TYPE,(select VALUE from sys_para where code='USERCODE') CUSTID  from sys_operlog where to_char(rqsj,'yyyy-mm-dd')=to_char(sysdate-1,'yyyy-mm-dd')";
                mResult = DbHelperOra.Query(sql).Tables[0];
            }
            return mResult;
        }
        #endregion

        #region 查询数据空间日志
        private DataTable querySys_spacelog()
        {
            String sql = "select to_char(EXECRQ,'yyyy-mm-dd') from dat_infdata_def where inftype='SYS_SPACELOG'";
            String result = (String)DbHelperOra.GetSingle(sql);
            DataTable mResult = null;
            //先判断当前时间减1天后是否大于上次上传时间
            if (DateTime.Compare(DateTime.Now.AddDays(-1), Convert.ToDateTime(result)) > 0)
            {
                sql = "select * from sys_spacelog where to_char(LOGDATE,'yyyy-mm-dd')=to_char(sysdate-1,'yyyy-mm-dd')";
                mResult = DbHelperOra.Query(sql).Tables[0];
            }
            return mResult;
        }
        #endregion

        #region 查询订单到货信息
        private DataTable queryDAT_DD_DHTIME()
        {
            String sql = @"select to_char(EXECRQ,'yyyy-MM-dd HH24:mm:ss') from dat_infdata_def where inftype='DAT_DD_DHTIME'";
            String result = (String)DbHelperOra.GetSingle(sql);
            DataTable mResult = null;
            sql = @"select (SELECT VALUE FROM SYS_PARA WHERE CODE='USERCODE') CUSTID,SEQNO,ROWNO,DHSL,FIRSTTIME,LASTTIME 
                    from DAT_DD_COM where LASTTIME is not null and LASTTIME>to_date('"+result+"','yyyy-MM-dd HH24:mi:ss')";
            mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        #endregion

        #region 查询唯一码跟踪报表
        private DataTable queryDat_onecodejxc()
        {
            String sql = "SELECT SEQNO,ONECODE,CREATETIME,GDSEQ,DEPT,OPERTYEP,PRINTTIME,PRINTNUM,BILLNO,BILLTIME,BILLUSER,CUSTID,DOCTOR,OPTID,OPTTABLE,OPTDATE,SUPID,HSJJ,PHID,PH,PZWH,DEPTID,STR1,STR2,STR3,STR4,STR5,SL,nvl(NUM2,0),nvl(NUM3,0),ISSEND FROM DAT_ONECODEJXC WHERE ISSEND='N'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        #endregion

        #region 分页查询库存
        private DataTable queryDat_StockDay(int pageNum, int pageIndex)
        {
            String sql = @"select * from
                            (select t.*,ROWNUM rn from
                            (select PICINO,PICITYPE,FLAG,GDSEQ,F_GETUNITNAME(UNIT),BZHL,CATID,HWID,DEPTID,SUPID,ZPBH,JXTAX,XXTAX,BILLNO,BILLTYPE,JHRQ,PHID,PH,RQ_SC,YXQZ,ISZP,YSL,YHSJE,YBHSJE,KCSL,KCLSJ,KCHSJJ,KCBHSJJ,KCHSJE,KCBHSJE,STR1,STR2,STR3,nvl(NUM1,0),nvl(NUM2,0),nvl(NUM3,0),MEMO from dat_goodsstock where 0=0 order by picino) t)
                            where rn>" + (pageIndex-1)*pageNum+" and rn<="+pageIndex*pageNum;
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        #endregion

        #region 查询商品基本资料信息
        private DataTable queryDOC_GOODS()
        {
            String sql = @"select to_char(EXECRQ,'yyyy-MM-dd HH24:mi:ss') from dat_infdata_def where inftype='DOC_GOODS_UP'";
            String result = (String)DbHelperOra.GetSingle(sql);
            DataTable mResult = null;
            sql = @"select g.GDSEQ,g.GDNAME,g.GDSPEC,nvl(g.BZHL,0),F_GETUNITNAME(g.UNIT) UNIT,
                    NVL(PIZNO,'无'),SUPPLIER,F_GETSUPNAME(SUPPLIER) SUPNAME,f_getproducername(PRODUCER) PRODUCER,JXTAX,XXTAX,HSJJ,ISGZ,F_GETUNITNAME(g.UNIT_DABZ) UNIT_DABZ,
                    F_GETUNITNAME(g.UNIT_ZHONGBZ) UNIT_ZHONGBZ,nvl(NUM_DABZ,0),nvl(NUM_ZHONGBZ,0),
                    DECODE(CATID0,'1','药品','3','医疗器械','其他') CATID0,ISLOT,DECODE(FLAG,'Y','2','T','3','S','4','E','5'),NVL(BARCODE,'0'),ISJG,BAR1,BAR2
                    from doc_goods g 
                    where FLAG<>'N' AND ISFLAG7='N' AND g.upttime>to_date('" + result + "','yyyy-MM-dd HH24:mi:ss')";
            mResult = DbHelperOra.Query(sql).Tables[0];//,G.ISJG,G.BAR1,G.BAR2
            return mResult;
        }
        #endregion

        #region 查询商品进销存信息
        private DataTable queryDOC_GOODSJXC()
        {
            String sql = @"select to_char(EXECRQ,'yyyy-MM-dd HH24:mi:ss') from dat_infdata_def where inftype='DOC_GOODSJXC_UP'";
            String result = (String)DbHelperOra.GetSingle(sql);
            DataTable mResult = null;
            sql = @"SELECT RQSJ,BILLNO,KCADD,SL,HSJJ,HSJE,SUPID,SUPNAME,GDSEQ,GDNAME,GDSPEC,UNITNAME,PRODUCERNAME,PIZNO,KCSL,HSJJ_NOW FROM V_GOODSJXC WHERE RQSJ>to_date('" + result + "','yyyy-MM-dd HH24:mi:ss')";
            mResult = DbHelperOra.Query(sql).Tables[0];//,G.ISJG,G.BAR1,G.BAR2
            return mResult;
        }
        #endregion

        #region 查询驳回的入库单
        private DataTable queryDAT_RKDREJECT()
        {
            DataTable mResult = null;
            string sql = @"SELECT SEQNO FROM DAT_RK_DOC
                            WHERE FLAG='R' AND ISSEND='N'
                            UNION
                            SELECT SEQNO FROM DAT_YRK_DOC
                            WHERE FLAG='R' AND ISSEND = 'N'";
            mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        #endregion

        #region 查询已发送状态的订单
        private DataTable queryDAT_DDSTATUS()
        {
            DataTable mResult = null;
            string sql = @"SELECT SEQNO FROM DAT_DD_DOC WHERE ISSEND IN ('Y','E')";
            mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        #endregion

        #region 查询因未受理而预计的采配单
        private DataTable queryDAT_CPDSTATUS()
        {
            DataTable mResult = null;
            string sql = @"SELECT SEQNO FROM DAT_DD_REMIND WHERE BILLFLAG in ('S','010')";
            mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        #endregion

        #region 查询商品资料调整单
        private DataTable queryTZGOODS()
        {
            String sql = @"SELECT SEQNO,BILLNO,BILLTYPE,FLAG,XGR,XGTYPE,nvl(XGRQ, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')),SUBNUM,LRY,LRRQ,SPR,SPRQ,SHR,SHRQ,STR1,STR2,STR3,NVL(NUM1,0),NVL(NUM2,0),NVL(NUM3,0),MEMO 
                            FROM DAT_TZGOODS_DOC
                            WHERE FLAG='Y' AND STR1='N'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        private DataTable queryTZGOODSCom(String seqno)
        {
            String sql = @"SELECT SEQNO,ROWNO,GDSEQ,GDNAME,BARCODE,ZJM,NAMEJC,NAMEEN,GDSPEC,GDMODE,F_GETUNITNAME(UNIT),CATID,JX,YX,PIZNO,BAR1,BAR2,SUPPLIER,
PRODUCER,ZPBH,PPID,CDID,JXTAX,XXTAX,BHSJJ,HSJJ,LSJ,YBJ,HSID,NVL(HSJ,0),MANAGER,nvl(ENDRQ, to_date('0001/01/01 00:00:00', 'yyyy/mm/dd hh24:mi:ss')),ISLOT,ISJB,ISFZ,ISGZ,ISIN,ISJG,
ISDM,ISYNZJ,ISFLAG1,ISFLAG2,ISFLAG3,ISFLAG4,ISFLAG5,ISFLAG6,ISFLAG7,ISFLAG8,STR3,NVL(NUM2,0),F_GETUNITNAME(UNIT_DABZ),F_GETUNITNAME(UNIT_ZHONGBZ),
BARCODE_DABZ,BARCODE_ZHONGBZ,NVL(NUM_DABZ,0),NVL(NUM_ZHONGBZ,0),UNIT_ORDER,UNIT_SELL,HISCODE,HISNAME,CATID0,NVL(KPYXQ,0),NVL(MJYXQ,0),MEMO,STR1,STR2,STR5,STR6 FROM DAT_TZGOODS_COM where seqno = '" + seqno + "'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        private Boolean updateTZGOODS(DataTable dt, String flag)
        {
            String seqNoOld = "";
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            if (seqNoOld.Length > 0)
            {
                String sql = "update DAT_TZGOODS_DOC set STR1 = '" + flag + "' where  seqno in (" + seqNoOld + ")";
                DbHelperOra.ExecuteSql(sql);
            }

            return true;
        }
        #endregion

        #region 查询商品资料调整单
        private DataTable queryTZSUP()
        {
            String sql = @"SELECT SEQNO,BILLNO,BILLTYPE,FLAG,XGR,XGTYPE,XGRQ,SUBNUM,LRY,LRRQ,SPR,SPRQ,SHR,SHRQ,STR1,STR2,STR3,NVL(NUM1,0),NVL(NUM2,0),NVL(NUM3,0),MEMO 
                            FROM DAT_TZSUP_DOC
                            WHERE FLAG='Y' AND STR1='N'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        private DataTable queryTZSUPCom(String seqno)
        {
            String sql = "select SEQNO,ROWNO,SUPID,SUPNAME from DAT_TZSUP_COM where seqno = '" + seqno + "'";
            DataTable mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        private Boolean updateTZSUP(DataTable dt, String flag)
        {
            String seqNoOld = "";
            foreach (DataRow dr in dt.Rows)
            {
                seqNoOld += "'" + dr["SEQNO"].ToString() + "'";
                if (dt.Rows.IndexOf(dr) != dt.Rows.Count - 1)
                {
                    seqNoOld += ",";
                }
            }
            if (seqNoOld.Length > 0)
            {
                String sql = "update DAT_TZSUP_DOC set STR1 = '" + flag + "' where  seqno in (" + seqNoOld + ")";
                DbHelperOra.ExecuteSql(sql);
            }

            return true;
        }
        #endregion

        #region 查询供应商资料信息
        private DataTable queryDOC_SUPPLIER()
        {
            String sql = @"select to_char(EXECRQ,'yyyy-MM-dd HH24:mi:ss') from dat_infdata_def where inftype='DOC_SUPPLIER_UP'";
            String result = (String)DbHelperOra.GetSingle(sql);
            DataTable mResult = null;
            sql = @"select supid,supname from doc_supplier 
                    where issupplier='Y' and ISSEND='Y' and  upttime>to_date('" + result + "','yyyy-MM-dd HH24:mi:ss')";
            mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        #endregion

        #region 查询科室资料信息
        private DataTable querySYS_DEPT()
        {
            String sql = "";//@"select to_char(EXECRQ,'yyyy-MM-dd HH24:mm:ss') from dat_infdata_def where inftype='SYS_DEPT_UP'";
            //String result = (String)DbHelperOra.GetSingle(sql);
            DataTable mResult = null;
            sql = @"select CODE,NAME,TYPE,FLAG,CLASS,SJCODE,ISLAST,STOCK,ISXH,ISORDER,
ISDELETE,MANAGER,DHZQ1,DHZQ2,DHZQ3,DHZQ4,DHZQ5,DHZQ6,DHZQ7,NVL(KCZQ,0),STR5,NUM1 from sys_dept";
            mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        #endregion

        #region 查询商品供应关系表
        private DataTable queryDOC_GOODS_SUP()
        {
            String sql = @"select to_char(EXECRQ,'yyyy-MM-dd HH24:mm:ss') from dat_infdata_def where inftype='DOC_GOODSSUP'";
            String result = (String)DbHelperOra.GetSingle(sql);
            DataTable mResult = null;
            sql = @"select gdseq,supid,F_GETSUPPLIERNAME(supid) supname,(select VALUE from sys_para WHERE CODE='USERCODE') custID,
                    (select VALUE from sys_para WHERE CODE='USERNAME') custname,pssid,F_GETSUPPLIERNAME(pssid) pssname,ordersort  
                    from doc_goodssup 
                    where upttime>to_date('" + result + "','yyyy-MM-dd HH24:mi:ss')";
            mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        #endregion

        #region 查询销售明细视图VIEW_XS_COM
        private DataTable queryVIEW_XS_COM()
        {
            DataTable mResult = null;
            string sql = @"select billno,billtype,rowno,gdseq,gdname,unit,gdspec,bzhl,bzsl,
                            xssl,jxtax,hsjj,hsje,phid,ph,pzwh,rq_sc,yxqz,str1,supid,rqsj,catid0 from VIEW_XS_COM";
            mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        private DataTable queryXSSeqno()
        {
            DataTable mResult = null;
            string sql = @"select distinct str1 seqno from VIEW_XS_COM";
            mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        private void updateJSDFlag(string seqnos)
        {
            string sql = @"update dat_jsd_doc set flag='G'
                            where flag='Y' and seqno in ("+seqnos+")";
            DbHelperOra.ExecuteSql(sql);
        }

        private DataTable queryVIEW_XS_COMC()
        {
            DataTable mResult = null;
            string sql = @"select billno,billtype,rowno,F_GETEASCODEBYGDSEQ(gdseq) gdseq,gdname,unit,gdspec,bzhl,bzsl,
                            xssl,jxtax,hsjj,hsje,phid,ph,pzwh,rq_sc,yxqz,str1,supid,rqsj,catid0 from VIEW_XS_COM";
            mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }

        private void updateRKDREJECT(string seqnos)
        {
            string sql = @"update dat_rk_doc set issend='Y'
                            where flag='R' and seqno in (" + seqnos + ")";
            DbHelperOra.ExecuteSql(sql);
            sql = @"update dat_yrk_doc set issend='Y'
                            where flag='R' and seqno in (" + seqnos + ")";
            //string sql = @"BEGIN 
            //                DELETE FROM DAT_RK_EXT WHERE BILLNO IN ({0});
            //                DELETE FROM DAT_RK_COM WHERE SEQNO IN ({0});
            //                DELETE FROM DAT_RK_DOC WHERE SEQNO IN ({0});
            //               END;";
            sql = string.Format(sql,seqnos);
            DbHelperOra.ExecuteSql(sql);
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

        #region 插入eas库存信息
        private Boolean insertInventory(JToken data)
        {
            List<CommandInfo> list = new List<CommandInfo>();

            if (JsonConvert.SerializeObject(data).Length <= 2)
            {
                return false;
            }

            String gdseq = data.Value<String>("MATERIALNUMBER");
            String phid = data.Value<String>("FLOT");
            String qty = data.Value<String>("FBASEQTY");
            String unit = data.Value<String>("UNITNUMBER");

            String sql = @"insert into EAS_STOCK_COMPARE (SEQNO,RQ,GDSEQ,UNIT,PHID,SL) values (SEQ_EAS_STOCK.Nextval,SYSDATE,F_GETGDSEQBYEASCODE('" + gdseq + "'),'" + unit + "','" + phid + "'," + qty + "）";
            list.Add(new CommandInfo(sql, null));

            try
            {
                DbHelperOra.ExecuteSqlTran(list);
            }
            catch (Exception ex)
            {
                this.isError = true;
                this.errorDetail += ex.Message;
            }

            return true;
        }
        #endregion

        #region 保存EAS下传订单
        private void saveDDDocCom(JArray ja)
        {
            DataTable dResultDOC = ApiUtil.GetTableSchema("DAT_DD_DOC_COMPARE");
            DataTable dResultCOM = ApiUtil.GetTableSchema("DAT_DD_COM_COMPARE");
            String strDDBH = "";
            String strCustId = "";
            String SEQNO = "";
            foreach (JToken jt in ja)
            {
                //表头
                JObject jo = (JObject)jt;
                String billType = jo.Value<String>("BILLTYPE");

                String convSqlDOC = @"select * from dat_download_rule where type = '" + billType + "' and doccom='D'";
                String convSqlCOM = @"select * from dat_download_rule where type='" + billType + "' and doccom='C'";
                DataTable convertTableDOC = DbHelperOra.Query(convSqlDOC).Tables[0];
                DataTable convertTableCOM = DbHelperOra.Query(convSqlCOM).Tables[0];
                DataRow docRow = dResultDOC.NewRow();

                SEQNO = jo.Value<String>("FNUMBER");
                strDDBH = SEQNO.Split('_')[0];
                strCustId = SEQNO.Split('_')[1];

                //判断FLAG<>4的单据不要
                String strFlag = jo.Value<String>("FBASESTATUS");
                //if (!"4".Equals(strFlag))
                //{
                //    continue;
                //}

                foreach (DataRow dr in convertTableDOC.Rows)
                {
                    String colTarget = dr["COLTARGET"].ToString();
                    String colName = dr["COLNAME"].ToString();
                    Boolean isConv = "Y".Equals(dr["ISCONV"].ToString());
                    String convRule = dr["CONVRULE"].ToString();
                    if (!isConv)
                    {
                        if (String.IsNullOrWhiteSpace(jo.Value<String>(colName)))
                        {
                            docRow[colTarget] = DBNull.Value;
                        }
                        else
                        {
                            docRow[colTarget] = jo.Value<String>(colName);
                        }
                    }
                    else
                    {
                        docRow[colTarget] = convRule;
                    }
                }

                int intSubNum = 0;//表头条数
                decimal decSubSum = 0;//表头总金额

                //表体
                JArray jacom = jo.Value<JArray>("ENTRIES");
                foreach (JToken jtcom in jacom)
                {
                    JObject jocom = (JObject)jtcom;
                    DataRow comRow = dResultCOM.NewRow();
                    foreach (DataRow dr in convertTableCOM.Rows)
                    {
                        String colTarget = dr["COLTARGET"].ToString();
                        String colName = dr["COLNAME"].ToString();
                        Boolean isConv = "Y".Equals(dr["ISCONV"].ToString());
                        String convRule = dr["CONVRULE"].ToString();

                        if (!isConv)
                        {
                            if (String.IsNullOrWhiteSpace(jocom.Value<String>(colName)))
                            {
                                comRow[colTarget] = DBNull.Value;
                            }
                            else
                            {
                                comRow[colTarget] = jocom.Value<String>(colName);
                            }
                        }
                        else
                        {
                            comRow[colTarget] = convRule;
                        }
                    }
                    //计算表头中条数和总金额
                    intSubNum++;
                    // decSubSum = decSubSum + Convert.ToDecimal(comRow["HSJJ"]) * Convert.ToDecimal(comRow["DHS"]);

                    comRow["SEQNO"] = strDDBH;
                    comRow["ROWNO"] = jacom.IndexOf(jtcom) + 1;
                    DataTable goodsDt = getERPGoodsTable(jocom.Value<String>("MATERIALNUMBER"));
                    if (goodsDt.Rows.Count > 0)
                    {
                        comRow["GDSEQ"] = goodsDt.Rows[0]["GDSEQ"];
                        comRow["UNIT"] = goodsDt.Rows[0]["UNIT"];
                        comRow["GDNAME"] = goodsDt.Rows[0]["GDNAME"];
                        comRow["GDSPEC"] = goodsDt.Rows[0]["GDSPEC"];
                        //comRow["GDMODE"] = goodsDt.Rows[0]["GDMODE"];
                        comRow["CATID"] = goodsDt.Rows[0]["CATID"];
                        //comRow["JXTAX"] = goodsDt.Rows[0]["JXTAX"];
                        //comRow["PZWH"] = goodsDt.Rows[0]["PZWH"];
                        comRow["ZPBH"] = goodsDt.Rows[0]["ZPBH"];
                        comRow["BZHL"] = (goodsDt.Rows[0]["BZHL"] == null ? 0 : goodsDt.Rows[0]["BZHL"]);
                        //comRow["ISGZ"] = goodsDt.Rows[0]["ISGZ"];//
                        comRow["ISLOT"] = goodsDt.Rows[0]["ISLOT"];
                        comRow["BHSJJ"] = goodsDt.Rows[0]["BHSJJ"];
                        //comRow["DDBH"] = strDDBH;
                        comRow["ISGZ"] = "";
                    }

                    dResultCOM.Rows.Add(comRow);
                }
                docRow["SEQNO"] = strDDBH;
                docRow["BILLNO"] = strDDBH;
                docRow["SUBNUM"] = intSubNum;//
                docRow["SUBSUM"] = decSubSum;
                dResultDOC.Rows.Add(docRow);
            }
            bulkInsert("DAT_DD_DOC_COMPARE", dResultDOC, strDDBH, strCustId);
            bulkInsert("DAT_DD_COM_COMPARE", dResultCOM, strDDBH, strCustId);
        }
        #endregion

        #region 保存EAS下传退货单
        private void saveTHDocCom(JArray ja)
        {
            DataTable dResultDOC = ApiUtil.GetTableSchema("DAT_TH_DOC_COMPARE");
            DataTable dResultCOM = ApiUtil.GetTableSchema("DAT_TH_COM_COMPARE");
            String strDDBH = "";
            String strCustId = "";
            String SEQNO = "";
            foreach (JToken jt in ja)
            {
                //表头
                JObject jo = (JObject)jt;
                String billType = "THD";

                String convSqlDOC = @"select * from dat_download_rule where type = '" + billType + "' and doccom='D'";
                String convSqlCOM = @"select * from dat_download_rule where type='" + billType + "' and doccom='C'";
                DataTable convertTableDOC = DbHelperOra.Query(convSqlDOC).Tables[0];
                DataTable convertTableCOM = DbHelperOra.Query(convSqlCOM).Tables[0];
                DataRow docRow = dResultDOC.NewRow();

                SEQNO = jo.Value<String>("FNUMBER");
                strDDBH = SEQNO.Split('_')[0];
                strCustId = SEQNO.Split('_')[1];

                //判断FLAG<>4的单据不要
                String strFlag = jo.Value<String>("FBASESTATUS");
                //if (!"4".Equals(strFlag))
                //{
                //    continue;
                //}

                foreach (DataRow dr in convertTableDOC.Rows)
                {
                    String colTarget = dr["COLTARGET"].ToString();
                    String colName = dr["COLNAME"].ToString();
                    Boolean isConv = "Y".Equals(dr["ISCONV"].ToString());
                    String convRule = dr["CONVRULE"].ToString();
                    if (!isConv)
                    {
                        if (String.IsNullOrWhiteSpace(jo.Value<String>(colName)))
                        {
                            docRow[colTarget] = DBNull.Value;
                        }
                        else
                        {
                            docRow[colTarget] = jo.Value<String>(colName);
                        }
                    }
                    else
                    {
                        docRow[colTarget] = convRule;
                    }
                }

                int intSubNum = 0;//表头条数
                decimal decSubSum = 0;//表头总金额

                //表体
                JArray jacom = jo.Value<JArray>("ENTRIES");
                foreach (JToken jtcom in jacom)
                {
                    JObject jocom = (JObject)jtcom;
                    DataRow comRow = dResultCOM.NewRow();
                    foreach (DataRow dr in convertTableCOM.Rows)
                    {
                        String colTarget = dr["COLTARGET"].ToString();
                        String colName = dr["COLNAME"].ToString();
                        Boolean isConv = "Y".Equals(dr["ISCONV"].ToString());
                        String convRule = dr["CONVRULE"].ToString();

                        if (!isConv)
                        {
                            if (String.IsNullOrWhiteSpace(jocom.Value<String>(colName)))
                            {
                                comRow[colTarget] = DBNull.Value;
                            }
                            else
                            {
                                comRow[colTarget] = jocom.Value<String>(colName);
                            }
                        }
                        else
                        {
                            comRow[colTarget] = convRule;
                        }
                    }
                    //计算表头中条数和总金额
                    intSubNum++;
                    // decSubSum = decSubSum + Convert.ToDecimal(comRow["HSJJ"]) * Convert.ToDecimal(comRow["DHS"]);

                    comRow["SEQNO"] = strDDBH;
                    comRow["ROWNO"] = jacom.IndexOf(jtcom) + 1;
                    DataTable goodsDt = getERPGoodsTable(jocom.Value<String>("MATERIALNUMBER"));
                    if (goodsDt.Rows.Count > 0)
                    {
                        comRow["GDSEQ"] = goodsDt.Rows[0]["GDSEQ"];
                        comRow["UNIT"] = goodsDt.Rows[0]["UNIT"];
                        comRow["GDNAME"] = goodsDt.Rows[0]["GDNAME"];
                        comRow["GDSPEC"] = goodsDt.Rows[0]["GDSPEC"];
                        //comRow["GDMODE"] = goodsDt.Rows[0]["GDMODE"];
                        comRow["CATID"] = goodsDt.Rows[0]["CATID"];
                        //comRow["JXTAX"] = goodsDt.Rows[0]["JXTAX"];
                        //comRow["PZWH"] = goodsDt.Rows[0]["PZWH"];
                        comRow["ZPBH"] = goodsDt.Rows[0]["ZPBH"];
                        comRow["BZHL"] = (goodsDt.Rows[0]["BZHL"] == null ? 0 : goodsDt.Rows[0]["BZHL"]);
                        //comRow["ISGZ"] = goodsDt.Rows[0]["ISGZ"];//
                        comRow["ISLOT"] = goodsDt.Rows[0]["ISLOT"];
                        comRow["BHSJJ"] = goodsDt.Rows[0]["BHSJJ"];
                        //comRow["DDBH"] = strDDBH;
                        //comRow["ISGZ"] = "";
                    }

                    dResultCOM.Rows.Add(comRow);
                }
                docRow["SEQNO"] = strDDBH;
                docRow["BILLNO"] = strDDBH;
                docRow["SUBNUM"] = intSubNum;//
                docRow["SUBSUM"] = decSubSum;
                dResultDOC.Rows.Add(docRow);
            }
            bulkInsert("DAT_TH_DOC_COMPARE", dResultDOC, strDDBH, strCustId);
            bulkInsert("DAT_TH_COM_COMPARE", dResultCOM, strDDBH, strCustId);
        }
        #endregion

        #region 保存EAS下传入库单
        private string saveRKDocCom(JArray ja)
        {
            DataTable dResultDOC = ApiUtil.GetTableSchema("DAT_RK_DOC_COMPARE");
            DataTable dResultCOM = ApiUtil.GetTableSchema("DAT_RK_COM_COMPARE");
            String strDDBH = "";
            String strCustId = "";
            String SEQNO = "";
            foreach (JToken jt in ja)
            {
                //表头
                JObject jo = (JObject)jt;
                String billType = jo.Value<String>("BILLTYPE");

                String convSqlDOC = @"select * from dat_download_rule where type = '" + billType + "' and doccom='D'";
                String convSqlCOM = @"select * from dat_download_rule where type='" + billType + "' and doccom='C'";
                DataTable convertTableDOC = DbHelperOra.Query(convSqlDOC).Tables[0];
                DataTable convertTableCOM = DbHelperOra.Query(convSqlCOM).Tables[0];
                DataRow docRow = dResultDOC.NewRow();

                SEQNO = jo.Value<String>("FNUMBER");
                //String SOURCENUMBER = jo.Value<String>("SOURCENUMBER");
                //strDDBH = SOURCENUMBER.Split('_')[0];
                //strCustId = SOURCENUMBER.Split('_')[1];
                ////strCustId = getCustIdByEasCustId(strCustId);
                //docRow["DDBH"] = strDDBH;
                //docRow["CUSTID"] = strCustId;

                //判断FLAG<>4的单据不要
                String strFlag = jo.Value<String>("FBASESTATUS");
                if (!"4".Equals(strFlag))
                {
                    continue;
                }

                foreach (DataRow dr in convertTableDOC.Rows)
                {
                    String colTarget = dr["COLTARGET"].ToString();
                    String colName = dr["COLNAME"].ToString();
                    Boolean isConv = "Y".Equals(dr["ISCONV"].ToString());
                    String convRule = dr["CONVRULE"].ToString();
                    if (!isConv)
                    {
                        if (String.IsNullOrWhiteSpace(jo.Value<String>(colName)))
                        {
                            docRow[colTarget] = DBNull.Value;
                        }
                        else
                        {
                            docRow[colTarget] = jo.Value<String>(colName);
                        }
                    }
                    else
                    {
                        docRow[colTarget] = convRule;
                    }
                }

                int intSubNum = 0;//表头条数
                decimal decSubSum = 0;//表头总金额

                //表体
                JArray jacom = jo.Value<JArray>("ENTRIES");
                foreach (JToken jtcom in jacom)
                {
                    JObject jocom = (JObject)jtcom;
                    DataRow comRow = dResultCOM.NewRow();
                    foreach (DataRow dr in convertTableCOM.Rows)
                    {
                        String colTarget = dr["COLTARGET"].ToString();
                        String colName = dr["COLNAME"].ToString();
                        Boolean isConv = "Y".Equals(dr["ISCONV"].ToString());
                        String convRule = dr["CONVRULE"].ToString();

                        if (!isConv)
                        {
                            if (String.IsNullOrWhiteSpace(jocom.Value<String>(colName)))
                            {
                                comRow[colTarget] = DBNull.Value;
                            }
                            else
                            {
                                comRow[colTarget] = jocom.Value<String>(colName);
                            }
                        }
                        else
                        {
                            comRow[colTarget] = convRule;
                        }
                    }
                    //计算表头中条数和总金额
                    intSubNum++;
                    //decSubSum = decSubSum + Convert.ToDecimal(comRow["HSJJ"]) * Convert.ToDecimal(comRow["SSSL"]);

                    //comRow["CUSTID"] = strCustId;
                    comRow["SEQNO"] = SEQNO;
                    comRow["ROWNO"] = jacom.IndexOf(jtcom) + 1;
                    DataTable goodsDt = getERPGoodsTable(jocom.Value<String>("MATERIALNUMBER"));
                    if (goodsDt.Rows.Count > 0)
                    {
                        comRow["GDSEQ"] = goodsDt.Rows[0]["GDSEQ"];
                        comRow["UNIT"] = goodsDt.Rows[0]["UNIT"];
                        comRow["GDNAME"] = goodsDt.Rows[0]["GDNAME"];
                        comRow["GDSPEC"] = goodsDt.Rows[0]["GDSPEC"];
                        comRow["GDMODE"] = goodsDt.Rows[0]["GDMODE"];
                        comRow["CATID"] = goodsDt.Rows[0]["CATID"];
                        //comRow["JXTAX"] = goodsDt.Rows[0]["JXTAX"];
                        //comRow["PZWH"] = goodsDt.Rows[0]["PZWH"];
                        comRow["ZPBH"] = goodsDt.Rows[0]["ZPBH"];
                        comRow["BZHL"] = (goodsDt.Rows[0]["BZHL"] == null ? 0 : goodsDt.Rows[0]["BZHL"]);
                        //comRow["ISGZ"] = goodsDt.Rows[0]["ISGZ"];//
                        comRow["ISLOT"] = goodsDt.Rows[0]["ISLOT"];
                        comRow["BHSJJ"] = goodsDt.Rows[0]["BHSJJ"];
                        //comRow["DDBH"] = strDDBH;
                        comRow["ISGZ"] = "";
                    }

                    dResultCOM.Rows.Add(comRow);
                }

                docRow["SUBNUM"] = intSubNum;//
                docRow["SUBSUM"] = decSubSum;
                dResultDOC.Rows.Add(docRow);
            }
            bulkInsert("DAT_RK_DOC_COMPARE", dResultDOC, SEQNO, strCustId);
            bulkInsert("DAT_RK_COM_COMPARE", dResultCOM, SEQNO, strCustId);
            return SEQNO;
        }
        #endregion

        #region 保存EAS下传销售单
        private void saveXSDocCom(JArray ja)
        {
            DataTable dResultDOC = ApiUtil.GetTableSchema("DAT_XS_DOC_COMPARE");
            DataTable dResultCOM = ApiUtil.GetTableSchema("DAT_XS_COM_COMPARE");
            String strDDBH = "";
            String strCustId = "";
            String SEQNO = "";
            foreach (JToken jt in ja)
            {
                //表头
                JObject jo = (JObject)jt;
                String billType = "XSD";

                String convSqlDOC = @"select * from dat_download_rule where type = '" + billType + "' and doccom='D'";
                String convSqlCOM = @"select * from dat_download_rule where type='" + billType + "' and doccom='C'";
                DataTable convertTableDOC = DbHelperOra.Query(convSqlDOC).Tables[0];
                DataTable convertTableCOM = DbHelperOra.Query(convSqlCOM).Tables[0];
                DataRow docRow = dResultDOC.NewRow();

                SEQNO = jo.Value<String>("FNUMBER");
                strDDBH = SEQNO.Split('_')[0];
                strCustId = SEQNO.Split('_')[1];

                //判断FLAG<>4的单据不要
                String strFlag = jo.Value<String>("FBASESTATUS");
                //if (!"4".Equals(strFlag))
                //{
                //    continue;
                //}

                foreach (DataRow dr in convertTableDOC.Rows)
                {
                    String colTarget = dr["COLTARGET"].ToString();
                    String colName = dr["COLNAME"].ToString();
                    Boolean isConv = "Y".Equals(dr["ISCONV"].ToString());
                    String convRule = dr["CONVRULE"].ToString();
                    if (!isConv)
                    {
                        if (String.IsNullOrWhiteSpace(jo.Value<String>(colName)))
                        {
                            docRow[colTarget] = DBNull.Value;
                        }
                        else
                        {
                            docRow[colTarget] = jo.Value<String>(colName);
                        }
                    }
                    else
                    {
                        docRow[colTarget] = convRule;
                    }
                }

                int intSubNum = 0;//表头条数
                decimal decSubSum = 0;//表头总金额

                //表体
                JArray jacom = jo.Value<JArray>("ENTRIES");
                foreach (JToken jtcom in jacom)
                {
                    JObject jocom = (JObject)jtcom;
                    DataRow comRow = dResultCOM.NewRow();
                    foreach (DataRow dr in convertTableCOM.Rows)
                    {
                        String colTarget = dr["COLTARGET"].ToString();
                        String colName = dr["COLNAME"].ToString();
                        Boolean isConv = "Y".Equals(dr["ISCONV"].ToString());
                        String convRule = dr["CONVRULE"].ToString();

                        if (!isConv)
                        {
                            if (String.IsNullOrWhiteSpace(jocom.Value<String>(colName)))
                            {
                                comRow[colTarget] = DBNull.Value;
                            }
                            else
                            {
                                comRow[colTarget] = jocom.Value<String>(colName);
                            }
                        }
                        else
                        {
                            comRow[colTarget] = convRule;
                        }
                    }
                    //计算表头中条数和总金额
                    intSubNum++;
                    // decSubSum = decSubSum + Convert.ToDecimal(comRow["HSJJ"]) * Convert.ToDecimal(comRow["DHS"]);

                    comRow["SEQNO"] = strDDBH;
                    comRow["ROWNO"] = jacom.IndexOf(jtcom) + 1;
                    DataTable goodsDt = getERPGoodsTable(jocom.Value<String>("MATERIALNUMBER"));
                    if (goodsDt.Rows.Count > 0)
                    {
                        comRow["GDSEQ"] = goodsDt.Rows[0]["GDSEQ"];
                        comRow["UNIT"] = goodsDt.Rows[0]["UNIT"];
                        comRow["GDNAME"] = goodsDt.Rows[0]["GDNAME"];
                        comRow["GDSPEC"] = goodsDt.Rows[0]["GDSPEC"];
                        //comRow["GDMODE"] = goodsDt.Rows[0]["GDMODE"];
                        comRow["CATID"] = goodsDt.Rows[0]["CATID"];
                        //comRow["JXTAX"] = goodsDt.Rows[0]["JXTAX"];
                        //comRow["PZWH"] = goodsDt.Rows[0]["PZWH"];
                        comRow["ZPBH"] = goodsDt.Rows[0]["ZPBH"];
                        comRow["BZHL"] = (goodsDt.Rows[0]["BZHL"] == null ? 0 : goodsDt.Rows[0]["BZHL"]);
                        //comRow["ISGZ"] = goodsDt.Rows[0]["ISGZ"];//
                        comRow["ISLOT"] = goodsDt.Rows[0]["ISLOT"];
                        comRow["BHSJJ"] = goodsDt.Rows[0]["BHSJJ"];
                        //comRow["DDBH"] = strDDBH;
                        //comRow["ISGZ"] = "";
                    }

                    dResultCOM.Rows.Add(comRow);
                }
                docRow["SEQNO"] = strDDBH;
                docRow["BILLNO"] = strDDBH;
                docRow["SUBNUM"] = intSubNum;//
                docRow["SUBSUM"] = decSubSum;
                dResultDOC.Rows.Add(docRow);
            }
            bulkInsert("DAT_XS_DOC_COMPARE", dResultDOC, strDDBH, strCustId);
            bulkInsert("DAT_XS_COM_COMPARE", dResultCOM, strDDBH, strCustId);
        }
        #endregion

        private DataTable getERPGoodsTable(String material)
        {
            String sql = "select a.* from doc_goods a where a.flag='Y' and a.BAR3='" + material + "'";
            DataTable dt = DbHelperOra.Query(sql).Tables[0];
            return dt;
        }

        /// <summary>
        /// 批量插入数据。 先清临时表，再bulkcopy到临时表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name="procedureName"></param>
        private void bulkInsert(String tableName, DataTable dt, String strDDBH, String strCustId)
        {
            String sql = "delete from " + tableName + " where SEQNO='" + strDDBH + "'";
            DbHelperOra.ExecuteSql(sql);
            ApiUtil.BulkInsert(tableName, dt);
        }

        #region 查询需要重发的短信
        private DataTable queryDAT_DD_REMIND()
        {
            DataTable mResult = null;
            string sql = @"SELECT SEQNO,STAFFTEL,MSGCONTENT FROM DAT_DD_REMIND WHERE ISSENDR='Y'";
            mResult = DbHelperOra.Query(sql).Tables[0];
            return mResult;
        }
        #endregion

        #region 更新重发短信的标志位和重发时间
        private bool updateDAT_DD_REMIND(string issendr, string seqno)
        {
            string sql = @"UPDATE DAT_DD_REMIND SET ISSENDR='{0}',SENDTIMER=SYSDATE
                            WHERE SEQNO='{1}'";
            sql = string.Format(sql, issendr, seqno);
            DbHelperOra.ExecuteSql(sql);
            return true;
        }
        #endregion

    }

}
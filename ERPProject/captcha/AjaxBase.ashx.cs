﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Data;
using System.IO;
using System.Text;
using System.Collections;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using Newtonsoft.Json;
using System.Web.Services;
using System.Text.RegularExpressions;
using XTBase.Utilities;

namespace ERPProject
{

    public class AjaxBase : IHttpHandler
    {
        public string url { get; set; }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {

            if (context.Request["callback"] != null && !string.IsNullOrEmpty(context.Request["callback"]))
            {
                string rStatus;
                string retStr = "";
                object cachekey = null;
                HttpRequest Request = HttpContext.Current.Request;
                string callback = Request["callback"];
                HttpResponse Response = HttpContext.Current.Response;
                Response.ContentType = "text/html";
                Response.Charset = "utf-8";//GB2312 "utf-8"
                ArrayList arlStr = new ArrayList();              

                try
                {   //接受 
                    if (Request["slable"].Length > 0) arlStr.Add(Request["slable"]);
                    if (Request["slable"].ToUpper().Contains("LOGIN"))
                    {
                        #region 解析串
                        string sql = "select MEMO,value from sys_para WHERE CODE='USERNAME'";
                        DataTable dt=DbHelperOra.Query(sql).Tables[0];
                        string wname = "";
                        string username="";
                        string usercount="";
                        DateTime date=DateTime.Now;

                        if(dt!=null&&dt.Rows.Count>0){
                          string strCode =dt.Rows[0][0].ToString().Replace("-", "");
                          username = strCode.Substring(5, Convert.ToInt32(strCode.Substring(0, 1)));  //获取用户名
                          usercount = strCode.Substring(9 + username.Length, Convert.ToInt32(strCode.Substring(strCode.Length - 1, 1))); // 获取用户数
                          date =Convert.ToDateTime(strCode.Substring(16 + username.Length + usercount.Length, 8));  //获取有效期
                          wname = dt.Rows[0][1].ToString();
                        }
                        #endregion

                        #region 获取真实用户数
                        string sql1 = "select count(userid) from user_list";
                        int ucount = Convert.ToInt32(DbHelperOra.GetSingle(sql1));
                        #endregion

                        if (wname != username || Convert.ToInt32(usercount) != ucount || date < DateTime.Now)
                        {
                            Response.Write(callback + "('" + "{\"rStatus\":\"" + "2001" + "\",\"retdata\":\"不符合条件\"}" + "')");
                            return;
                        }
                       
                    }


                    if (Request["var1"].Length > 0) arlStr.Add(Request["var1"]);
                    if (Request["var2"].Length > 0) arlStr.Add(Request["var2"]);
                    if (Request["var3"].Length > 0) arlStr.Add(Request["var3"]);
                    if (Request["var4"].Length > 0) arlStr.Add(Request["var4"]);
                    if (Request["var5"].Length > 0) arlStr.Add(Request["var5"]);
                    if (Request["var6"].Length > 0) arlStr.Add(Request["var6"]);
                    if (Request["var7"].Length > 0) arlStr.Add(Request["var7"]);
                    if (Request["var8"].Length > 0) arlStr.Add(Request["var8"]);
                    if (Request["var9"].Length > 0) arlStr.Add(Request["var9"]);
                    if (Request["var10"].Length > 0) arlStr.Add(Request["var10"]);
                    if (Request["var11"].Length > 0) arlStr.Add(Request["var11"]);
                    if (Request["var12"].Length > 0) arlStr.Add(Request["var12"]);
                    if (Request["var13"].Length > 0) arlStr.Add(Request["var13"]);
                    if (Request["var14"].Length > 0) arlStr.Add(Request["var14"]);
                    if (Request["var15"].Length > 0) arlStr.Add(Request["var15"]);
                    if (Request["var16"].Length > 0) arlStr.Add(Request["var16"]);
                    if (Request["var17"].Length > 0) arlStr.Add(Request["var17"]);
                    if (Request["var18"].Length > 0) arlStr.Add(Request["var18"]);
                    if (Request["var19"].Length > 0) arlStr.Add(Request["var19"]);
                    if (Request["var20"].Length > 0) arlStr.Add(Request["var20"]);
                    if (Request["var21"].Length > 0) arlStr.Add(Request["var21"]);
                    if (Request["var22"].Length > 0) arlStr.Add(Request["var22"]);
                    if (Request["var23"].Length > 0) arlStr.Add(Request["var23"]);
                    if (Request["var24"].Length > 0) arlStr.Add(Request["var24"]);
                    if (Request["var25"].Length > 0) arlStr.Add(Request["var25"]);
                    if (Request["var26"].Length > 0) arlStr.Add(Request["var26"]);
                    if (Request["var27"].Length > 0) arlStr.Add(Request["var27"]);
                    if (Request["var28"].Length > 0) arlStr.Add(Request["var28"]);
                    if (Request["var29"].Length > 0) arlStr.Add(Request["var29"]);
                    if (Request["var30"].Length > 0) arlStr.Add(Request["var30"]);
                }
                catch (Exception)
                {
                    //throw new Exception(e.Message);
                }

                retStr = main(arlStr);
                if (retStr.IndexOf("ERR:") >= 0 || retStr.IndexOf("ORA-") >= 0)
                {
                    rStatus = "2001";
                }
                else
                {
                    rStatus = "0000";
                    cachekey =  CacheHelper.GetCache(Request["skey"]);

                }
                if (callback == "?") //无回调函数
                {
                    if (string.IsNullOrEmpty(Request["skey"]))
                    {
                        Response.Write(callback + "('" + "{\"rStatus\":\"" + rStatus + "\",\"sessionkey\":\"" + cachekey + "\"}" + "')");
                    }
                    else
                    {
                        if (retStr == "OK")
                        {
                            Response.Write(callback + "('" + "{\"rStatus\":\"" + rStatus + "\",\"retdata\":\"" + retStr + "\"}" + "')");
                        }
                        else
                        {
                            // ---- chaged to reutrn directly
                            Response.Write( "{\"rStatus\":\"" + rStatus + "\",\"retdata\":" + retStr + "}" );
                        }
                    }
                }
                else //指定回调函数--myajax，把回JSON串
                {
                    if (string.IsNullOrEmpty(Request["skey"]))
                    {
                        Response.Write(callback + "('" + "{\"rStatus\":\"" + rStatus + "\",\"sessionkey\":\"" + cachekey + "\"}" + "')");
                    }
                    else
                    {
                        //Response.Write(callback + "(" + "{\"rStatus\":\"" + rStatus + "\",\"retdata\":\"" + retStr + "\"}" + ")");
                        if (retStr == "OK")
                        {
                            Response.Write(callback + "('" + "{\"rStatus\":\"" + rStatus + "\",\"retdata\":\"" + retStr + "\"}" + "')");
                        }
                        else
                        {
                            Response.Write(callback + "('" + "{\"rStatus\":\"" + rStatus + "\",\"retdata\":" + retStr + "}" + "')");
                            //Response.Write(callback + "('" + "{\"rStatus\":\"" + rStatus + "\",\"retdata\":\"" + retStr + "\"}" + "')");
                        }
                    }
                }
                Response.End();

            }
        }

        public static string main(ArrayList arlStr)
        {
            string ret = "";//正确返回：【OK】,错误：【ERR:XXX】,其它返回JSON串
            if (arlStr.Count < 1)
            {
                return "ERR:" + "SQL参数必须为：【CONN/SQLGET_???/SQLEXEC_???/PROC_???/EXT_???】，请使用正确的调用方法！";

            }
            switch ((string)arlStr[0])
            {
                case "CONN":
                    ret = "";//DbHelperOra.ConnDb();
                    if (ret == "") { ret = "OK"; }
                    break;
                case "SQLGET":     //执行通用取数SQL得到数据
                    ret = DbHelperOra.SQL_GetData(arlStr);
                    break;
                case "SQLEXEC":     //执行通用SQL更新数据库,单条INSERT或UPDATE,返回ok/errors
                    ret = DbHelperOra.SQL_ExecSQL(arlStr);
                    break;
                case "PROC":        //执行通用过程,返回ok/errors
                    ret = DbHelperOra.SQL_ExecProc(arlStr);
                    break;
                case "EXT":         //其它扩展应用,返回ok/errors
                    ret = AjaxExt.Exec(arlStr);
                    break;
                default:
                    ret = "ERR:" + "SQL参数必须为：【CONN/SQLGET_???/SQLEXEC_???/PROC_???/EXT_???】，请使用正确的调用方法！";
                    break;
            }
            return ret;
        }



        //public string Login(string strPara)
        //{
        //    JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

        //    LoginPara LoginPara = jsonSerializer.Deserialize<LoginPara>(strPara);

        //    if (LoginPara.usergh == "admin" && LoginPara.userpwd == "admin")
        //    {
        //        return "1";
        //    }
        //    else
        //    {
        //        return "错误的用户名或密码！";
        //    } 
        //}

        //public string GegGoods(string strPara, ref string retPara)
        //{
        //    Hashtable ht = MyJson.Json2HT(strPara);
        //    //Hashtable goods=DbHelperOra.GetArrayHT("SELECT CODE,NAME,UNIT,LSJ FROM DOC_GOODS WHERE CODE like '"+ht["gdid"].ToString()+"'");
        //    //retPara = MyJson.HT2Json(goods);
        //    DataTable goods = DbHelperOra.Query("select * from mk_member WHERE MEMLOGINID='jerry'");
        //    //"SELECT CODE,NAME,UNIT,LSJ FROM DOC_GOODS WHERE CODE like '" + ht["gdid"].ToString() + "' and rownum < 6 ");
        //    retPara = MyJson.DataTable2Json(goods);
        //    return "1";
        //}

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using XTBase;
using System.Data;
using System.Collections;
using System.Web.Hosting;

namespace ERPProject
{
    public class Global : System.Web.HttpApplication
    {
        public static void RefreshTableConfig(string key)
        {
            try
            {
                Hashtable infTableDef = new Hashtable();
                String sql = "select * from sys_report where seqno = '" + key + "'";
                DataTable dt = DbHelperOra.Query(sql).Tables[0];
                string sqlSpec = dt.Rows[0].Field<string>("SELECTSQL");
                DataTable dataDt = DbHelperOra.Query(sqlSpec).Tables[0];
                //foreach (DataRow dr in dt.Rows)
                //{
                //    infTableDef.Add(dr["INFKEY"].ToString(), dr["INFVALUE"].ToString());
                //}
                System.Web.Caching.Cache objCache = HttpRuntime.Cache;
                InfTableCacheCallBack = new System.Web.Caching.CacheItemRemovedCallback(RemovedCallback);
                objCache.Add(
                    key,
                    dataDt,
                    null,
                    DateTime.Now.Add(TimeSpan.FromMinutes(30)),
                    TimeSpan.Zero,
                    System.Web.Caching.CacheItemPriority.Default,
                    InfTableCacheCallBack);
                //InfTableCacheRemoved = false;
            }
            catch (Exception ex) {
                //LogHelper.WriteLog("CacheError_"+key, ex);
                //PubFunc.OperLog("缓存管理", "00", "admin", "CacheError_" + key + "_" + ex.Message);
            }
            //两小时失效
            //CacheHelper.SetCache(INF_TABLE_CACHE_NAME, infTableDef, TimeSpan.FromHours(2));
        }

        private static System.Web.Caching.CacheItemRemovedCallback InfTableCacheCallBack = null;
        //public static Boolean InfTableCacheRemoved = false;
        //public static System.Web.Caching.CacheItemRemovedReason InfTableCacheRemovedReason;
        private static void RemovedCallback(String k, Object v, System.Web.Caching.CacheItemRemovedReason r)
        {
            //InfTableCacheRemoved = true;
            //InfTableCacheRemovedReason = r;
            //LogHelper.WriteLog("CacheRemoved_"+k+"_" + r);
            //PubFunc.OperLog("缓存管理", "00", "admin", "CacheRemoved_" + k + "_" + r);
            RefreshTableConfig(k);
        }


        protected void Application_Start(object sender, EventArgs e)
        {
            HostingEnvironment.RegisterVirtualPathProvider(new XTFramework.MyVirtualPathProvider());
            try
            {
                DataTable dt = DbHelperOra.Query("select SEQNO from sys_report where type = 'DDLLIST' and flag='A'").Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    string seqno = dr.Field<string>("SEQNO");
                    //DataTable dtSpec = DbHelperOra.Query(sql).Tables[0];
                    RefreshTableConfig(seqno);
                }
            }
            catch(Exception ex) {
                // LogHelper.WriteLog("CacheListError", ex);
                //PubFunc.OperLog("缓存管理", "00", "admin", "CacheListError_" + ex.Message);
            }
            
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}
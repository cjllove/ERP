﻿using XTBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace ERPProject.captcha
{
    /// <summary>
    /// GetModGoods 的摘要说明
    /// </summary>
    public class GetModGoods : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string GDSEQ = context.Request.QueryString["GDSEQ"];
            String Sql = @"SELECT '新商品' TYPE,A.GDSEQ,A.GDNAME,A.PRODUCER PRODUCERNAME,A.UPTTIME,A.PIZNO,A.GDSPEC,A.BZHL,f_getunitname(A.UNIT) UNIT,f_getunitname(A.UNIT_DABZ) UNIT_DABZ,A.NUM_DABZ
                    FROM INF_DOC_GOODS_HIS A WHERE A.GDSEQ = '{0}'
                    UNION ALL
                    SELECT '原商品',A.GDSEQ,A.GDNAME,f_getsuppliername(A.PRODUCER) PRODUCERNAME,A.UPTTIME,A.PIZNO,A.GDSPEC,A.BZHL,f_getunitname(A.UNIT) UNIT,f_getunitname(A.UNIT_DABZ) UNIT_DABZ,A.NUM_DABZ
                    FROM DOC_GOODS A WHERE A.GDSEQ = '{0}'";
            DataTable Dt = DbHelperOra.Query(String.Format(Sql, GDSEQ)).Tables[0];

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            javaScriptSerializer.MaxJsonLength = Int32.MaxValue; //取得最大数值
            ArrayList arrayList = new ArrayList();
            foreach (DataRow dataRow in Dt.Rows)

            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>(); //实例化一个参数集合
                foreach (DataColumn dataColumn in Dt.Columns)
                {
                    dictionary.Add(dataColumn.ColumnName, dataRow[dataColumn.ColumnName].ToString());
                }
                arrayList.Add(dictionary); //ArrayList集合中添加键值
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(javaScriptSerializer.Serialize(arrayList));
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System.Data;
using System.Collections.Specialized;
using System.Text;
using XTBase.Utilities;
using System.Collections;
using Newtonsoft.Json.Converters;

 
 
namespace FineUIPro.Examples.grid
{
    /// <summary>
    /// grid_rowexpander_grid_data 的摘要说明
    /// </summary>
    public class grid_rowexpander_grid_data : IHttpHandler
    {
 
        public void ProcessRequest(HttpContext context)
        {
            string rowId = context.Request.QueryString["id"];
           // int rowIdInt = Convert.ToInt32(rowId);
            string rowname = context.Request.QueryString["name"];
            string sbegrq = context.Request.QueryString["begrq"];
            string sendrq = context.Request.QueryString["endrq"];
            DateTime begrq = Convert.ToDateTime(sbegrq);
            DateTime endrq = Convert.ToDateTime(sendrq);
            JArray ja = new JArray();

            DataTable dt = DbHelperOra.QueryForTable("select code,name from sys_category where TYPE='"+rowname+"' and flag='Y' ");
            if (dt.Rows.Count > 0)
            {

                
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataTable dts = DbHelperOra.QueryForTable(string.Format(@"select
F_GET_CAT_PIE('SYSL','{0}',TO_DATE('{1}','yyyy-MM-dd'),TO_DATE('{2}','yyyy-MM-dd'),'C')SYSL,
F_GET_CAT_PIE('SLZB','{0}',TO_DATE('{1}','yyyy-MM-dd'),TO_DATE('{2}','yyyy-MM-dd'),'C')SLZB,
       F_GET_CAT_PIE('HBZZSL','{0}',TO_DATE('{1}','yyyy-MM-dd'),TO_DATE('{2}','yyyy-MM-dd'),'C')HBZZSL,
F_GET_CAT_PIE('TBZZSL','{0}',TO_DATE('{1}','yyyy-MM-dd'),TO_DATE('{2}','yyyy-MM-dd'),'C')TBZZSL,
F_GET_CAT_PIE('SYJE','{0}',TO_DATE('{1}','yyyy-MM-dd'),TO_DATE('{2}','yyyy-MM-dd'),'C')SYJE ,
                  F_GET_CAT_PIE('JEZB','{0}',TO_DATE('{1}','yyyy-MM-dd'),TO_DATE('{2}','yyyy-MM-dd'),'C')JEZB,
F_GET_CAT_PIE('HBZZJE','{0}',TO_DATE('{1}','yyyy-MM-dd'),TO_DATE('{2}','yyyy-MM-dd'),'C')HBZZJE,
F_GET_CAT_PIE('TBZZJE','{0}',TO_DATE('{1}','yyyy-MM-dd'),TO_DATE('{2}','yyyy-MM-dd'),'C')TBZZJE
from dual ", dt.Rows[i][0].ToString(), begrq.ToShortDateString(), endrq.ToShortDateString()));
                    JArray jaItem = new JArray();
                    jaItem.Add(dt.Rows[i][1].ToString());                 
                    for (int j = 0; j < 8; j++)
                    {
                        jaItem.Add(dts.Rows[0][j].ToString());
                    }
                        ja.Add(jaItem);
                }
                
                
            }
           
            context.Response.ContentType = "text/plain";
            context.Response.Write(ja.ToString(Newtonsoft.Json.Formatting.None));
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
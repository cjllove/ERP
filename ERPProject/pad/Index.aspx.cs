using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XTBase;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FineUIPro;
using System.Configuration;

namespace ERPProject.pad
{
    public partial class Index : PageBase
    {
        public Index() {
            ISCHECK = false;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                if (Request.QueryString["oper"] != null)
                {
                    if (Request.QueryString["oper"].ToString() == "menu")
                    {
                        string result = menuInit();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();

                    }
                }
                else {
                    lblSysName.Text = ConfigurationManager.AppSettings["APPNAME"];
                    lblWelcome.Text = "欢迎您，" + UserAction.UserName;
                }
            }
        }
        private string menuInit() {
            string result = "";
            DataTable dt = new DataTable();
            string sql = "select * from sys_function WHERE SYSTEM = 'TAB' AND RUNWHAT<>'#' AND RUNWHAT IS NOT NULL order by itemsort";
            dt = DbHelperOra.Query(sql).Tables[0];
            foreach (DataRow dr in dt.Rows) {
                if (dr["FUNICO"] != null && dr["FUNICO"].ToString().IndexOf("icon-") == -1) {
                    foreach (string icon in Enum.GetNames(typeof(IconFont)))
                    {
                        IconFont iconType = (IconFont)Enum.Parse(typeof(IconFont), icon);
                        if (icon == dr["FUNICO"].ToString())
                        {
                            string iconName = IconFontHelper.GetName(iconType);
                            dr["FUNICO"] = iconName;
                            //btn.IconFont = iconType;
                        }
                    }
                }
            }
            string data = JsonConvert.SerializeObject(dt);
            
            JObject jo = new JObject();
            jo.Add("result", "success");
            jo.Add("data", data);
            result = JsonConvert.SerializeObject(jo);
            return result;
        }
    }
}
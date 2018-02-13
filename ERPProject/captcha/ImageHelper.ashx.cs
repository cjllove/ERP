using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPProject.captcha
{
    /// <summary>
    /// ImageHelper 的摘要说明
    /// </summary>
    public class ImageHelper : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string result = "";
             List<String> ls = new List<string>();
            HttpRequest Request = HttpContext.Current.Request;
            String gdseqs = Request["gdseqs"];
            if (!string.IsNullOrWhiteSpace(gdseqs)) {
                string[] sa = gdseqs.Split(';');
                ls = WeiGo.ImageHelper.GetImgs(sa);
                foreach (string s in ls) {
                    
                    result += s + ";";
                }
                result = result.TrimEnd(';');
            }
            
            context.Response.ContentType = "text/plain";
            context.Response.Write(result);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace ERPProject.captcha
{
    public partial class ImageServer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)  
        {
            String key = Request.QueryString["img"];
            if (!string.IsNullOrWhiteSpace(key)) {
                byte[] img = (byte[])XTBase.Utilities.CacheHelper.GetCache(key);
                Response.ContentType = "application/binary;";
                //这个地方图片可以从数据库中读取二进制图片  
                //byte[] img = DBHelper.ReadImg();  
                //byte[] img = File.ReadAllBytes(Server.MapPath("img") + @"/testImg.jpg");
                try
                {
                    Response.BinaryWrite(img);
                }
                catch { }
                Response.Flush();
                Response.End();  
            }
           
        }  
    }
}



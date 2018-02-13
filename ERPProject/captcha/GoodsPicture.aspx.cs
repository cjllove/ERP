using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using XTBase;

namespace ERPProject.captcha
{
    public partial class GoodsPicture : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    String picPath = Request["picPath"];
            //    if (!string.IsNullOrWhiteSpace(picPath))
            //    {
            //        String[] paramSup = new String[1] { picPath };
            //        byte[] img = ApiClientService.getUploadgoodsPic("getGoodsPicUpload", paramSup);
            //        Response.Clear();
            //        Response.ContentType = "application/binary;";
            //        Response.BinaryWrite(img);
            //        Response.Flush();
            //        Response.End();
            //    }
            //}

            String picPath = Request["picPath"];
            if (!string.IsNullOrWhiteSpace(picPath))
            {
                String[] paramSup = new String[1] { picPath };
                DbHelperOra.ExecuteSql("INSERT INTO A_TEST(ROWNO,BEGIN,MEMO) SELECT 1,SYSDATE,'" + picPath + "' FROM DUAL");
                byte[] b_logoImg = ApiClientService.getUploadgoodsPic("getGoodsPicUpload", paramSup);
                DbHelperOra.ExecuteSql("INSERT INTO A_TEST(ROWNO,BEGIN,MEMO) SELECT 2,SYSDATE,'" + picPath + "' FROM DUAL");
                if (b_logoImg == null)
                {
                    for (int i = 0; i < 10; i++)
                    {

                        DbHelperOra.ExecuteSql("INSERT INTO A_TEST(ROWNO,BEGIN,MEMO) SELECT " + 3 + i + ",SYSDATE,'" + picPath + "' FROM DUAL");
                        b_logoImg = ApiClientService.getUploadgoodsPic("getGoodsPicUpload", paramSup);
                        DbHelperOra.ExecuteSql("INSERT INTO A_TEST(ROWNO,BEGIN,MEMO) SELECT " + 4 + i + ",SYSDATE,'" + picPath + "' FROM DUAL");
                        if (b_logoImg != null)
                        {
                            break;
                        }
                    }
                }
                if (b_logoImg != null)
                {
                    if (b_logoImg.Length > 0)
                    {
                        MemoryStream ms = new MemoryStream(b_logoImg);
                        Response.Clear();
                        Response.ContentType = "image/gif";
                        Response.OutputStream.Write(b_logoImg, 0, b_logoImg.Length);
                        Response.End();
                    }
                }
            }
        }
    }
}
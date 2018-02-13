using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace ERPProject.captcha
{
    public partial class CertificatePic : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String seqno = Request.QueryString["seqno"];
            String rowno = Request.QueryString["rowno"];
            if (!string.IsNullOrWhiteSpace(seqno) && !string.IsNullOrWhiteSpace(rowno))
            {
                String[] paramSup = new String[2] { seqno, rowno };
                byte[] img = ApiClientService.getUploadPic("SUP_LICENSE_TEMPPICTURE", paramSup);
                Response.ContentType = "application/binary;";
                Response.BinaryWrite(img);
                Response.Flush();
                Response.End();
            }
        }
    }
}
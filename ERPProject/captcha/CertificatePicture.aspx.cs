﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace ERPProject.captcha
{
    public partial class CertificatePicture : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String seqno = Request["seqno"];
            String rowno = Request["rowno"];
            if (!string.IsNullOrWhiteSpace(seqno) && !string.IsNullOrWhiteSpace(rowno))
            {
                String[] paramSup = new String[2] { seqno, rowno };
                byte[] img = ApiClientService.getUploadPic("SUP_LICENSE_PICTURE", paramSup);
                Response.ContentType = "application/binary;";
                Response.BinaryWrite(img);
                Response.Flush();
                Response.End();
            }
        }
    }
}
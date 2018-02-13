﻿using Newtonsoft.Json;
using XTBase;
using System;
using System.Data;

namespace ERPProject.ERPQuery
{
    public partial class TraceBackFm : System.Web.UI.Page
    {
        public string USERNAME = "高值条码";
        public string GDNAME = "高值条码";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["onecode"] != null)
                {
                    USERNAME = Request.QueryString["onecode"];
                    GDNAME = DbHelperOra.GetSingle("SELECT GDNAME FROM DAT_GZ_EXT WHERE ONECODE = '" + USERNAME + "'").ToString();
                    versionDataBind(Request.QueryString["onecode"].ToString());
                }
            }
        }
        private void versionDataBind(String Onecode)
        {
            string strSql = @"SELECT A.BILLNO,A.GDSEQ,A.INSTIME,f_getusername(B.SHR) USERNAME,
                            '科室【'||f_getdeptname(B.DEPTID)||'】给患者【'||B.CUSTID||'】使用，系统操作人：'||f_getusername(B.SHR)||'，使用单：'||A.BILLNO MEMO
                            FROM DAT_XS_EXT A,DAT_XS_DOC B
                            WHERE A.ONECODE = '{0}' AND A.BILLNO = B.SEQNO AND B.FLAG IN('Y','G','J')
                            UNION ALL
                            SELECT A.BILLNO,A.GDSEQ,A.INSTIME,f_getusername(B.SHR) USERNAME,
                            '库房【'||f_getdeptname(B.DEPTOUT)||'】 ，出库至【'||f_getdeptname(B.DEPTID)||'】 ，出库人：'||f_getusername(B.SHR)||'，出库单：'||A.BILLNO MEMO
                            FROM DAT_CK_EXT A,DAT_CK_DOC B
                            WHERE A.ONECODE = '{0}' AND A.BILLNO = B.SEQNO AND B.FLAG IN('Y')
                            UNION ALL
                            SELECT A.BILLNO,A.GDSEQ,A.INSTIME,f_getusername(B.SHR) USERNAME,
                            '配送商【'||f_getsuppliername( B.PSSID)||'】，送货至【'||f_getdeptname(B.DEPTID)||'】，签收人：'||f_getusername(B.SHR)||'，入库单：'||A.BILLNO MEMO
                            FROM DAT_RK_EXT A,DAT_RK_DOC B
                            WHERE A.ONECODE = '{0}' AND A.BILLNO = B.SEQNO AND B.FLAG IN('Y','G')
                            UNION ALL
                            SELECT A.BILLNO,A.GDSEQ,A.INSTIME,f_getusername(B.SHR) USERNAME,
                            '库房【'||f_getdeptname(B.DEPTID)||'】，向配送商【'|| f_getsuppliername( B.PSSID) ||'】订货，采购员：'||f_getusername(B.CGY)||'，订货单：'||A.BILLNO MEMO
                            FROM DAT_DD_EXT A,DAT_DD_DOC B
                            WHERE A.ONECODE = '{0}' AND A.BILLNO = B.SEQNO AND B.FLAG IN('Y','G')";
            RepeaterVersion.DataSource = DbHelperOra.Query(String.Format(strSql, Onecode)).Tables[0];
            RepeaterVersion.DataBind();
        }
    }
}
using System;
using FineUIPro;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XTBase;

namespace ERPProject.CertificateInput
{
    public partial class EarlyWarningInput : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }
        }

        private void DataInit()
        {
            DataSearch();
        }


        private void DataSearch()
        {
            string expirenum = "";
            if (nbxExpire.Text.Length <= 0)
            {
                expirenum = "30";
                nbxExpire.Text = "30";
            }
            else
            {
                expirenum = nbxExpire.Text;
            }
            string sql = string.Format(@"select seqno,lictype,begrq,endrq,docid,memo,warning,
                        DECODE(SIGN(WARNING),
                                1， '已过期' || WARNING || '天',0, '今天过期',
                                -1,
                                '还有' || ABS(WARNING) || '天到期')  tishi
                        FROM (select seqno,
                                '商品证照' lictype,
                                begrq,
                                endrq,
                                docid,
                                memo,
                                (trunc(sysdate) - trunc(endrq)) warning

                            from doc_license_goods
                            where (trunc(sysdate) - trunc(endrq)) >= -{0}
                        union all
                        select seqno,
                                '机构证照' lictype,
                                begrq,
                                endrq,
                                docid,
                                memo,
                                (trunc(sysdate) - trunc(endrq)) warning
                            from doc_license_supplier
                            where (trunc(sysdate) - trunc(endrq)) >= -{0}
                        union all
                        select seqno,
                                '客户关系证照' lictype,
                                begrq,
                                endrq,
                                docid,
                                memo,
                                (trunc(sysdate) - trunc(endrq)) warning
                            from doc_license_relationsup
                            where (trunc(sysdate) - trunc(endrq)) >= -{0}
                        union all
                        select grantid seqno,
                                '授权证照' lictype,
                                beginsqrq begrq,
                                endsqrq endrq,
                                docid,
                                memo,
                                (trunc(sysdate) - trunc(endsqrq)) warning
                            from doc_license_grant
                            where (trunc(sysdate) - trunc(endsqrq)) >= -{0})", Convert.ToInt32(expirenum));

            string query = "";
            StringBuilder strSql = new StringBuilder(sql);
            if (ddlIsWarn.SelectedValue.Equals("ALLExpire"))
            {
                
            }
            if(tbxDOCID.Text.Length>0)
            {
                ddlIsWarn.SelectedValue = "ALLExpire";
                query = tbxDOCID.Text.Trim();
                strSql.AppendFormat("where docid LIKE '%{0}%'", query);
            }
            if(ddlIsWarn.SelectedValue.Equals("ISExpire"))
            {
                strSql.AppendFormat("where warning >= 0");
            }
            if (ddlIsWarn.SelectedValue.Equals("NOExpire"))
            {
                strSql.AppendFormat("where warning < 0");
            }

            int total = 0;
            DataTable dtData = PubFunc.DbGetPage(GridCertype.PageIndex, GridCertype.PageSize, strSql.ToString(), ref total);
            GridCertype.RecordCount = total;
            GridCertype.DataSource = dtData;
            GridCertype.DataBind();
        }

        protected void GridCertype_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                int warnint = Convert.ToInt32(row["WARNING"].ToString());
                if (warnint < 0 && warnint >= -30)
                {
                    e.CellCssClasses[1] = "color2";       //单列加颜色(如果只想将单元格的字体变色，只需要将样式表里的background-color的属性去掉，加上color，然后alpha设置透明度即可)
                }
                if (warnint >= 0)
                {
                    e.CellCssClasses[1] = "color1";       //单列加颜色
                }
            }
        }

        protected void GridCertype_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridCertype.PageIndex = e.NewPageIndex;
            DataSearch();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }
    }
}
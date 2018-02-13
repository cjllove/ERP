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
using System.Text.RegularExpressions;
using XTBase;

namespace SPDProject.CertificateInput
{
    public partial class SupplierHistory : PageBase
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
            HistoryDataSearch();
        }
        private void HistoryDataSearch()
        {
            string strSQL = "SELECT SUPID,SUPNAME,decode(flag,'Y','已审核','N','已保存','S','已提交','R','已驳回','待录入')flag, ";
            string strISBEGIN = @"";
            string strSQLBEG = @"FROM(select ta.supid,ta.supname,(select flag from doc_license_log where supid=ta.supid and rownum=1) flag,  ";
            string strSQLEND = @"from (select a.supid, a.supname, b.code, b.name
                                    from doc_supplier a, doc_license b where a.issupplier = 'Y' and b.objuser='SUP_LIC') ta, doc_license_log tb
                            where ta.supid = tb.supid(+) and ta.name = tb.licensename(+)";
            string strsql = "group by ta.supid, ta.supname order by ta.supid, ta.supname)";


            StringBuilder sb = new StringBuilder();
            StringBuilder sb1 = new StringBuilder();
            string strSQLAFTER = @"select distinct 'sum(decode(tb.licensename,''' || tb.name || ''',tb.picnum, 0))" + "\"" + "' || tb.name ||'" + "\"" + "'from  doc_license tb where tb.objuser='SUP_LIC'";
            strISBEGIN = @"select 'DECODE(SIGN(" + "\"" + "'|| tb.name ||'" + "\"" + "), 0，''未上传'',''已上传'') " + "\"" + "' || tb.name ||'" + "\"" + "'from doc_license tb where tb.objuser='SUP_LIC'";
            DataTable mydtable = DbHelperOra.Query(strISBEGIN).Tables[0];
            if (mydtable.Rows.Count > 0)
            {
                foreach (DataRow dr in mydtable.Rows)
                {
                    sb1.Append(dr[0].ToString());
                    sb1.Append(",");
                }
            }
            else
            {
                Alert.Show("请先维护供应商证照再上传证照图片！");
                return;
            }
            sb1 = sb1.Remove(sb1.Length - 1, 1);
            DataTable dtafter = DbHelperOra.Query(strSQLAFTER).Tables[0];
            if (dtafter.Rows.Count > 0)
            {
                foreach (DataRow dr in dtafter.Rows)
                {
                    sb.Append(dr[0].ToString());
                    sb.Append(",");
                    FineUIPro.BoundField bf;
                    bf = new FineUIPro.BoundField();
                    Match M = Regex.Match(dr[0].ToString(), "\".*\"");
                    bf = new FineUIPro.BoundField();
                    bf.ColumnID = M.ToString().Replace("\"", "");
                    bf.DataField = M.ToString().Replace("\"", "");
                    bf.HeaderText = M.ToString().Replace("\"", "");
                    bf.TextAlign = FineUIPro.TextAlign.Center;
                    bf.Width = 96;
                    GridLIS.Columns.Add(bf);
                }
            }

            if (!string.IsNullOrWhiteSpace(txtName.Text))
            {
                strSQLEND = strSQLEND + "  and (ta.supid like '%" + txtName.Text + "%' or ta.supname like  '%" + txtName.Text + "%')";
            }
            

            strSQL = strSQL + sb1 + strSQLBEG + sb.ToString().TrimEnd(',') + strSQLEND + strsql;

            if (sb.ToString().Length == 0)
            {
                strSQL = "select 1 from dual where 1=2 ";
            }
            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridLIS.PageIndex, GridLIS.PageSize, strSQL, ref total);
            GridLIS.RecordCount = total;
            GridLIS.DataSource = dt;
            GridLIS.DataBind();


            if (dt.Rows.Count > 0)
            {
                DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    T.SEQNO,
                                                    T.LICENSEID,
                                                    T.LICENSENAME,
                                                    T.SUPNAME,
                                                    T.SUPID,
                                                    T.OPERTIME,
                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.SUPID, T.SUPNAME,T.OPERTIME,T.FLAG,T.PICNUM
                                                        FROM DOC_LICENSE_LOG T
                                                        WHERE SUPID = '" + dt.Rows[0][0].ToString() + "' AND T.ISCUR='N') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC'").Tables[0];
                GridCertype.DataSource = lisDT;
                GridCertype.DataBind();
            }
        }

        protected void GridLIS_RowClick(object sender, GridRowClickEventArgs e)
        {

        }

        protected void GridLIS_PageIndexChange(object sender, GridPageEventArgs e)
        {

        }

        protected void GridLIS_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {

        }

        protected void GridCertype_RowCommand(object sender, GridCommandEventArgs e)
        {

        }

        protected void GridLicense_RowClick(object sender, GridRowClickEventArgs e)
        {

        }

        protected void btnLeft_Click(object sender, EventArgs e)
        {

        }

        protected void btnRight_Click(object sender, EventArgs e)
        {

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

        }
    }
}
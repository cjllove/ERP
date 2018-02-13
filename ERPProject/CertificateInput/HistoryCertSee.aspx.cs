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
    public partial class HistoryCertSee : PageBase
    {
        private static int picindex = 0;
        //用于判断是否保存并新增上级授权
        private static string picnum = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HistoryDataSearch();
                TabStrip1.ActiveTabIndex = 0;
            }
        }

        private void HistoryDataSearch()
        {
            string strSQL = "SELECT GDSEQ,GDNAME,decode(flag,'Y','已审核','N','已保存','S','已提交','R','已驳回','待录入')flag, ";
            string strISBEGIN = @"";
            string strSQLBEG = @"FROM(select ta.gdseq,ta.gdname,(select flag from doc_license_log where gdseq=ta.gdseq and rownum=1) flag, ";
            string strSQLEND = @"from (select a.gdseq, a.gdname, b.code, b.name
                                    from doc_goods a, doc_license b where b.objuser='GOODS_LIC') ta, doc_license_log tb
                            where ta.gdseq = tb.gdseq(+) and ta.name = tb.licensename(+) and tb.iscur='N'";
            string strsql = "group by ta.gdseq, ta.gdname order by ta.gdseq, ta.gdname)";

            StringBuilder sb = new StringBuilder();
            StringBuilder sb1 = new StringBuilder();
            string strSQLAFTER = @"select distinct 'sum(decode(tb.licensename,''' || tb.name || ''',tb.picnum, 0))" + "\"" + "' || tb.name ||'" + "\"" + "'from  doc_license tb where tb.objuser='GOODS_LIC'";
            strISBEGIN = @"select 'DECODE(SIGN(" + "\"" + "'|| tb.name ||'" + "\"" + "), 0，''未上传'',''已上传'') " + "\"" + "' || tb.name ||'" + "\"" + "'from doc_license tb where tb.objuser='GOODS_LIC'";
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
                Alert.Show("请先维护商品证照再上传证照图片！");
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
                    bf.Width = 120;
                    GridLIS.Columns.Add(bf);
                }
            }


            if (!string.IsNullOrWhiteSpace(txtName.Text))
            {
                strSQLEND = strSQLEND + "  and (ta.gdseq='" + txtName.Text + "' or ta.gdname = '" + txtName.Text + "')";
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
                                                    T.GDNAME,
                                                    T.GDSEQ,
                                                    T.OPERTIME,
                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.GDSEQ, T.GDNAME,T.OPERTIME,T.FLAG,T.PICNUM
                                                        FROM DOC_LICENSE_LOG T
                                                        WHERE GDSEQ = '" + dt.Rows[0][0].ToString() + "' AND T.ISCUR='N') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];
                GridCertype.DataSource = lisDT;
                GridCertype.DataBind();
            }

        }

        protected void GridLIS_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridLIS.PageIndex = e.NewPageIndex;
            HistoryDataSearch();
        }

        protected void GridLIS_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            TabStrip1.ActiveTabIndex = 1;
            string gdname = GridLIS.Rows[e.RowIndex].DataKeys[2].ToString();
            string code = GridLIS.Rows[e.RowIndex].DataKeys[1].ToString();
            docGDNAME.Text = gdname;
            hfdGDSEQ.Text = code;
            DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    T.SEQNO,
                                                    T.LICENSEID,
                                                    T.LICENSENAME,
                                                    T.GDNAME,
                                                    T.GDSEQ,
                                                    T.OPERTIME,
                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.GDSEQ, T.GDNAME,T.OPERTIME,T.FLAG,T.PICNUM
                                                        FROM DOC_LICENSE_LOG T
                                                        WHERE GDSEQ = '" + code + "' AND T.ISCUR='N') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];

            docLISNAME.Text = lisDT.Rows[0]["LICENSENAME"].ToString();
            GridLicense.DataSource = lisDT;
            GridLicense.DataBind();

            if (DbHelperOra.Exists("select 1 from doc_license_log where gdseq='" + code + "'"))
            {
                DataTable mydt = DbHelperOra.Query("select seqno,licenseid,memo from doc_license_log where gdseq='" + code + "' and iscur='N'").Tables[0];
                string mypicnum = DbHelperOra.GetSingle("select count(1) from doc_license_img where gdseq='" + code + "' and licenseid='" + mydt.Rows[0]["licenseid"].ToString() + "' and iscur='N'").ToString();
                string seqno = mydt.Rows[0]["seqno"].ToString();
                DataTable licensedt = DbHelperOra.Query("select trunc(begrq) begrq,trunc(endrq) endrq,docid,memo from doc_license_goods where seqno='" + seqno + "'").Tables[0];
                if (licensedt.Rows[0][1].ToString().Equals("2099/1/1 0:00:00"))
                {
                    ischk.Checked = true;
                    dpkENDRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][1].ToString());
                    dpkBEGRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][0].ToString());
                    dpkENDRQ.Enabled = false;
                }
                else
                {
                    dpkENDRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][1].ToString());
                    dpkBEGRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][0].ToString());
                }
                docMEMO.Text = mydt.Rows[0]["memo"].ToString();
                docDOCID.Text = licensedt.Rows[0]["DOCID"].ToString();

                DataTable picDt = DbHelperOra.Query(string.Format("select imgpath,rowno from DOC_LICENSE_IMG where seqno='{0}' and rownum=1", seqno)).Tables[0];
                string picpath = picDt.Rows[0]["imgpath"].ToString();
                string rowno = picDt.Rows[0]["rowno"].ToString();
                picpath = picpath.Substring(1, picpath.Length - 1);
                imglbl.Text = seqno + "/" + rowno + picpath;
                imgBMPPATH.ImageUrl = ApiUtil.GetConfigCont("LIS_PICURL") + picpath;
                picindex = 1;
                btnLeft.Enabled = false;
                picnum = mypicnum;
                if (Convert.ToInt32(picnum) == 1)
                {
                    btnRight.Enabled = false;
                }
            }
        }

        protected void GridLIS_RowClick(object sender, GridRowClickEventArgs e)
        {
            string code = GridLIS.Rows[e.RowIndex].DataKeys[1].ToString();
            DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    T.SEQNO,
                                                    T.LICENSEID,
                                                    T.LICENSENAME,
                                                    T.GDNAME,
                                                    T.GDSEQ,
                                                    T.OPERTIME,
                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.GDSEQ, T.GDNAME,T.OPERTIME,T.FLAG,T.PICNUM
                                                        FROM DOC_LICENSE_LOG T
                                                        WHERE GDSEQ = '" + code + "' AND T.ISCUR='N') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];
            GridCertype.DataSource = lisDT;
            GridCertype.DataBind();
        }

        protected void GridLicense_RowClick(object sender, GridRowClickEventArgs e)
        {
            string LISID = GridLicense.Rows[e.RowIndex].DataKeys[4].ToString();
            string LISNAME = GridLicense.Rows[e.RowIndex].DataKeys[5].ToString();
            hfdLISID.Text = LISID;
            string HSEQNO = "";
            docLISNAME.Text = LISNAME;
            try
            {
                HSEQNO = GridLicense.Rows[e.RowIndex].DataKeys[0].ToString();
                hfdHSEQNO.Text = HSEQNO;
                hfdGDSEQ.Text = GridLicense.Rows[e.RowIndex].DataKeys[1].ToString();
                DataTable licensedt = DbHelperOra.Query("select trunc(begrq) begrq,trunc(endrq) endrq,docid,memo from doc_license_goods where seqno='" + HSEQNO + "' and licenseid='" + LISID + "'").Tables[0];
                if (licensedt.Rows[0][1].ToString().Equals("2099/1/1 0:00:00"))
                {
                    ischk.Checked = true;
                    dpkENDRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][1].ToString());
                    dpkBEGRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][0].ToString());
                    dpkENDRQ.Enabled = false;
                }
                else
                {
                    dpkENDRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][1].ToString());
                    dpkBEGRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][0].ToString());
                    ischk.Checked = false;
                    dpkENDRQ.Enabled = true;
                }
                docDOCID.Text = licensedt.Rows[0]["DOCID"].ToString();
                docMEMO.Text = licensedt.Rows[0]["MEMO"].ToString();
            }
            catch (Exception)
            {
            }
        }

        protected void GridCertype_RowCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "seepic")
            {
                try
                {
                    string LISID = GridCertype.Rows[e.RowIndex].DataKeys[4].ToString();
                    string LISNAME = GridCertype.Rows[e.RowIndex].DataKeys[5].ToString();
                    string HSEQNO = "";
                    HSEQNO = GridCertype.Rows[e.RowIndex].DataKeys[0].ToString();
                    Window1.Hidden = false;
                    string picnum = DbHelperOra.GetSingle("select count(1) from DOC_LICENSE_IMG where seqno='" + HSEQNO + "' and licenseid='" + LISID + "'").ToString();
                    string url = "~/CertificateInput/ShowLisPicWindow.aspx?bm=" + HSEQNO + "&xc=" + picnum + "&cc=" + LISID + "";
                    PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdSEQNO.ClientID) + Window1.GetShowReference(url, "证照图片展示"));
                }
                catch (Exception)
                {
                    Alert.Show("暂无图片展示,请上传图片后再查看！");
                }
            }
        }

        protected void btnLeft_Click(object sender, EventArgs e)
        {
            if (picindex == 1)
            {
                Alert.Show("已经是第一张证照图片！");
            }
            else
            {
                btnRight.Enabled = true;
                picindex--;
                DataTable mydt = DbHelperOra.Query("select seqno,licenseid,memo from doc_license_log where gdseq='" + hfdGDSEQ.Text + "'").Tables[0];
                string seqno = mydt.Rows[0]["seqno"].ToString();
                string licenseid = mydt.Rows[0]["licenseid"].ToString();
                DataTable picDt = DbHelperOra.Query(string.Format("select imgpath,rowno from DOC_LICENSE_IMG where seqno='{0}' and licenseid='{1}' and rowno={2} AND ISCUR='N'", seqno, licenseid, picindex)).Tables[0];
                string picpath = picDt.Rows[0]["imgpath"].ToString();
                string rowno = picDt.Rows[0]["rowno"].ToString();
                picpath = picpath.Substring(1, picpath.Length - 1);
                imglbl.Text = seqno + "/" + rowno + picpath;
                imgBMPPATH.ImageUrl = ApiUtil.GetConfigCont("LIS_PICURL") + picpath;
                if (picindex == 1)
                {
                    btnLeft.Enabled = false;
                }
            }
        }

        protected void btnRight_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(picnum) > 1)
            {
                picindex++;
                if (picindex > Convert.ToInt32(picnum))
                {
                    Alert.Show("证照图片到底了！");
                    picindex--;
                }
                else
                {
                    btnLeft.Enabled = true;
                    DataTable mydt = DbHelperOra.Query("select seqno,licenseid,memo from doc_license_log where gdseq='" + hfdGDSEQ.Text + "' AND ISCUR='N'").Tables[0];
                    string seqno = mydt.Rows[0]["seqno"].ToString();
                    string licenseid = mydt.Rows[0]["licenseid"].ToString();
                    DataTable picDt = DbHelperOra.Query(string.Format("select imgpath,rowno from DOC_LICENSE_IMG where seqno='{0}' and licenseid='{1}' and rowno={2} AND ISCUR='N'", seqno, licenseid, picindex)).Tables[0];
                    string picpath = picDt.Rows[0]["imgpath"].ToString();
                    string rowno = picDt.Rows[0]["rowno"].ToString();
                    picpath = picpath.Substring(1, picpath.Length - 1);
                    imglbl.Text = seqno + "/" + rowno + picpath;
                    imgBMPPATH.ImageUrl = ApiUtil.GetConfigCont("LIS_PICURL") + picpath;
                    if (picindex == Convert.ToInt32(picnum))
                    {
                        btnRight.Enabled = false;
                    }
                }
            }
            if (Convert.ToInt32(picnum) == 1)
            {
                Alert.Show("证照图片到底了！");
            }
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {

        }
    }
}
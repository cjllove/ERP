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


//作者：曹国稳（讯通信息科技有限公司）

namespace ERPProject.CertificateInput
{
    public partial class SupplierCerts : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();

                //屏蔽不需要的操作按钮
                if (Request.QueryString["oper"] != null)
                {
                    if (Request.QueryString["oper"].ToString() == "gl")
                    {
                        ButtonHidden(btnAudit,btnReject,mybtnAudit,mybtnReject);
                        DataSearch();
                        lbfchCert.Hidden = true;
                    }
                    else if (Request.QueryString["oper"].ToString() == "sh")
                    {
                        ButtonHidden(btnHistory,btnRollBack,btnSave,btnSubmit);
                        chkisLR.Hidden = true;
                        chknoLR.Hidden = true;
                        DataSearch1();
                        lbfchCert.Hidden = true;
                    }
                    else if (Request.QueryString["oper"].ToString() == "change")
                    {
                        ButtonHidden(btnAudit, btnReject, mybtnAudit, mybtnReject);
                        chkisLR.Hidden = true;
                        chknoLR.Hidden = true;
                        DataSearch2();
                        lbfchCert.Hidden = false;
                    }
                    else
                    {
                        ButtonHidden(btnHistory, btnRollBack, btnSave, btnSubmit);
                        chkisLR.Hidden = true;
                        chknoLR.Hidden = true;
                        DataSearch3();
                        lbfchCert.Hidden = true;
                    }
                }

            }
        }

        private void DataInit()
        {
            //默认第一个tab页面
            TabStrip1.ActiveTabIndex = 0;
            //默认绑定供应商证照类别表
            DataTable MYDT = DbHelperOra.Query("SELECT CODE LISID,NAME LISNAME,ISNEED FROM DOC_LICENSE WHERE OBJUSER='SUP_LIC'").Tables[0];
            GridLicense.DataSource = MYDT;
            GridLicense.DataBind();
            DateTime tmCur = DateTime.Now;

            if (tmCur.Hour < 6 || tmCur.Hour > 18)
            {//晚上
                lblTime.Text = @"晚上好！";
            }
            else if (tmCur.Hour >= 6 && tmCur.Hour < 12)
            {//上午
                lblTime.Text = @"上午好！";
            }
            else
            {//下午
                lblTime.Text = @"下午好！";
            }
        }

        private void DataSearch()
        {
            //REL_LIC 表示客户关系证照信息（包含承诺书，合同等）
            //GOODS_LIC 表示商品证照信息
            //SUP_LIC 表示供应商证照信息
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
            if(mydtable.Rows.Count > 0)
            {
                foreach(DataRow dr in mydtable.Rows)
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
            sb1 = sb1.Remove(sb1.Length - 1,1);
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
            if (!string.IsNullOrWhiteSpace(ddlFLAG.SelectedValue))
            {
                strSQLEND = strSQLEND + "  and flag in (select flag from doc_license_log where supid=ta.supid and flag='" + ddlFLAG.SelectedValue + "' and iscur='Y')";
            }
            if (chkisLR.Checked)
            {
                strSQLEND = strSQLEND + "  and flag in (select flag from doc_license_log where supid=ta.supid and iscur='Y')";
            }
            if (chknoLR.Checked)
            {
                strSQLEND = strSQLEND + "  and flag not in (select flag from doc_license_log where supid=ta.supid and iscur='Y')";
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
                                                    B.ISNEED,
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
                                                        WHERE SUPID = '" + dt.Rows[0][0].ToString() + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC'").Tables[0];
                GridCertype.DataSource = lisDT;
                GridCertype.DataBind();
            }
        }
        
        private void initSear(string mystrseq)
        {
            string strSQL = "SELECT SUPID,SUPNAME,decode(flag,'Y','已审核','N','已保存','S','已提交','R','已驳回','待录入')flag, ";
            string strISBEGIN = @"";
            string strSQLBEG = @"FROM(select ta.supid,ta.supname,(select flag from doc_license_log where supid=ta.supid and rownum=1 AND ISCUR='Y') flag,  ";
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
            if (!string.IsNullOrWhiteSpace(ddlFLAG.SelectedValue))
            {
                strSQLEND = strSQLEND + "  and flag in (select flag from doc_license_log where supid=ta.supid and flag='" + ddlFLAG.SelectedValue + "' AND ISCUR='Y')";
            }

            strSQLEND = strSQLEND + mystrseq;

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
                                                    B.ISNEED,
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
                                                        WHERE SUPID = '" + dt.Rows[0][0].ToString() + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC'").Tables[0];
                GridCertype.DataSource = lisDT;
                GridCertype.DataBind();
            }
            else
            {
                GridCertype.DataSource = null;
                GridCertype.DataBind();
                GridLicense.DataSource = null;
                GridLicense.DataBind();
            }
        }

        //用户证照审核
        private void DataSearch1()
        {
           string strsql=  "  and flag in (select flag from doc_license_log where supid=ta.supid and flag='S' and iscur='Y' and ischange='N')";
           initSear(strsql);
        }

        //用于证照变更申请
        private void DataSearch2()
        {
            string strsql = "  and flag in (select flag from doc_license_log where supid=ta.supid and flag<>'S' and iscur='Y')";
            initSear(strsql);
        }

        //用于证照变更审核
        private void DataSearch3()
        {
            string strsql = "  and flag in (select flag from doc_license_log where supid=ta.supid and flag='S' and iscur='Y' and ischange='Y')";
            initSear(strsql);
        }

        protected void GridLIS_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridLIS.PageIndex = e.NewPageIndex;
            DataSearch();
        }

        protected void chkisLR_CheckedChanged(object sender, CheckedEventArgs e)
        {
            if (chkisLR.Checked)
            {
                chknoLR.Checked = false;
            }
        }

        protected void chknoLR_CheckedChanged(object sender, CheckedEventArgs e)
        {
            if (chknoLR.Checked)
            {
                chkisLR.Checked = false;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["oper"].ToString() == "gl")
            {
                DataSearch();
            }
            else if (Request.QueryString["oper"].ToString() == "sh")
            {
                DataSearch1();
            }
            else if(Request.QueryString["oper"].ToString() == "change")
            {
                DataSearch2();
            }
            else
            {
                DataSearch3();
            }
        }

        private void HistoryDataSearch()
        {

        }

        protected void btnHistory_Click(object sender, EventArgs e)
        {
            HistoryDataSearch();
        }

        protected void mybtnAudit_Click(object sender, EventArgs e)
        {
            int[] selections = GridLIS.SelectedRowIndexArray;
            if (GridLIS.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要审核的机构证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<CommandInfo> liscmd = new List<CommandInfo>();
            string succeed = string.Empty;
            bool ishave = false;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLIS.SelectedRowIndexArray[i];
                string flag = DbHelperOra.GetSingle("SELECT FLAG FROM DOC_LICENSE_LOG WHERE SUPID='" + GridLIS.DataKeys[rowIndex][1].ToString() + "' AND ROWNUM=1 AND ISCUR='Y'").ToString();
                if (flag.Equals("Y"))
                {
                    Alert.Show("已审核，无需重复审核!");
                    return;
                }
                
                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_SUPPLIER WHERE SUPID='" + GridLIS.DataKeys[rowIndex][1].ToString() + "' AND ISCUR='Y'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where SUPID='" + GridLIS.DataKeys[rowIndex][1].ToString() + "' AND ISCUR='Y'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo("update doc_license_log set flag='Y' where SUPID='" + GridLIS.DataKeys[rowIndex][1].ToString() + "' AND ISCUR='Y'", null));
                        liscmd.Add(new CommandInfo("update doc_license_supplier set flag='Y' where SUPID='" + GridLIS.DataKeys[rowIndex][1].ToString() + "' AND ISCUR='Y'", null));
                    }
                    ishave = true;
                }
                else
                {
                    ishave = false;
                }
            }
            if (ishave)
            {
                if (DbHelperOra.ExecuteSqlTran(liscmd))
                {
                    Alert.Show("审核成功！", "消息提示", MessageBoxIcon.Warning);
                    if (Request.QueryString["oper"].ToString() == "gl")
                    {
                        DataSearch();
                    }
                    else if (Request.QueryString["oper"].ToString() == "sh")
                    {
                        DataSearch1();
                        GridLicense.DataSource = null;
                        GridLicense.DataBind();
                        docLISNAME.Text = "";
                        docDOCID.Text = "";
                        docMEMO.Text = "";
                        docSUPNAME.Text = "";
                        dpkBEGRQ.Text="";
                        dpkENDRQ.Text = "";
                        ischk.Checked = false;
                        dpkENDRQ.Enabled=true;
                    }
                    else if (Request.QueryString["oper"].ToString() == "change")
                    {
                        DataSearch2();
                    }
                    else
                    {
                        DataSearch3();
                    }
                }
                else
                {
                    Alert.Show("操作失败！", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("未录入无须做审核操作！", "消息提示", MessageBoxIcon.Warning);
            }
        }
        protected void mybtnReject_Click(object sender, EventArgs e)
        {
            int[] selections = GridLIS.SelectedRowIndexArray;
            if (GridLIS.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要审核的商品证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<CommandInfo> liscmd = new List<CommandInfo>();
            string succeed = string.Empty;
            bool ishave = false;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLIS.SelectedRowIndexArray[i];
                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_SUPPLIER WHERE SUPID='" + GridLIS.DataKeys[rowIndex][1].ToString() + "' AND ISCUR='Y'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where SUPID='" + GridLIS.DataKeys[rowIndex][1].ToString() + "' AND ISCUR='Y'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo("update doc_license_log set flag='R' where SUPID='" + GridLIS.DataKeys[rowIndex][1].ToString() + "' AND ISCUR='Y'", null));
                        liscmd.Add(new CommandInfo("update doc_license_SUPPLIER set flag='R' where SUPID='" + GridLIS.DataKeys[rowIndex][1].ToString() + "' AND ISCUR='Y'", null));
                    }
                    ishave = true;
                }
                else
                {
                    ishave = false;
                }
            }
            if (ishave)
            {
                if (DbHelperOra.ExecuteSqlTran(liscmd))
                {
                    Alert.Show("所选证照信息已被批量驳回！", "消息提示", MessageBoxIcon.Warning);
                    if (Request.QueryString["oper"].ToString() == "gl")
                    {
                        DataSearch();
                    }
                    else if (Request.QueryString["oper"].ToString() == "sh")
                    {
                        DataSearch1();
                    }
                    else if (Request.QueryString["oper"].ToString() == "change")
                    {
                        DataSearch2();
                    }
                    else
                    {
                        DataSearch3();
                    }
                }
                else
                {
                    Alert.Show("操作失败！", MessageBoxIcon.Warning);
                }
            }
            else
            {
                Alert.Show("未录入无须驳回操作！", "消息提示", MessageBoxIcon.Warning);
            }
        }

        protected void GridLIS_RowClick(object sender, GridRowClickEventArgs e)
        {
            string code = GridLIS.Rows[e.RowIndex].DataKeys[1].ToString();
            DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    B.ISNEED,
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
                                                        WHERE SUPID = '" + code + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC'").Tables[0];
            GridCertype.DataSource = lisDT;
            GridCertype.DataBind();
        }

        protected void GridLIS_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            TabStrip1.ActiveTabIndex = 1;
            string supname = GridLIS.Rows[e.RowIndex].DataKeys[2].ToString();
            string code = GridLIS.Rows[e.RowIndex].DataKeys[1].ToString();
            docSUPNAME.Text = supname;
            hfdSUPID.Text = code;

            string myseq = "";
            if (DbHelperOra.Exists("select 1 from doc_license_log where supid='" + code + "' AND ISCUR='Y'"))
            {
                DataTable mydt = DbHelperOra.Query("select seqno,licenseid from doc_license_log where supid='" + code + "' AND ISCUR='Y'").Tables[0];
                string seqno = mydt.Rows[0]["seqno"].ToString();
                myseq = seqno + "|" + code + "|" + hfdLISID.Text + "|" + code;
            }
            else
            {
                //这个地方需要修改一下seqno，需要从数据库中查询一下
                string seqnostr = DbHelperOra.GetSingle("SELECT SEQ_LICENSE_PIC.NEXTVAL FROM DUAL").ToString();
                string seqno = "LIS" + hfdSUPID.Text + seqnostr;
                hfdSEQNO.Text = seqno;
                myseq = hfdSEQNO.Text + "|" + hfdSUPID.Text + "|" + hfdLISID.Text + "|" + code;
            }
            PageContext.RegisterStartupScript("initUpload('" + myseq + "');");


            DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    B.ISNEED,
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
                                                        WHERE supid = '" + code + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC'").Tables[0];

            docLISNAME.Text = lisDT.Rows[0]["LICENSENAME"].ToString();
            GridLicense.DataSource = lisDT;
            GridLicense.DataBind();

            if (DbHelperOra.Exists("select 1 from doc_license_log where supid='" + code + "' AND ISCUR='Y'"))
            {
                DataTable mydt = DbHelperOra.Query("select seqno,licenseid,memo from doc_license_log where supid='" + code + "' AND ISCUR='Y'").Tables[0];
                string seqno = mydt.Rows[0]["seqno"].ToString();
                DataTable licensedt = DbHelperOra.Query("select trunc(begrq) begrq,trunc(endrq) endrq from doc_license_supplier where seqno='" + seqno + "' AND ISCUR='Y'").Tables[0];
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
            }
            else
            {
                docLISNAME.Text = "";
                docMEMO.Text = "";
                dpkBEGRQ.Text = "";
                dpkENDRQ.Text = "";
                ischk.Checked = false;
                dpkENDRQ.Enabled = true;
            }

            DataTable cdt = DbHelperOra.Query("select flag from doc_license_log where supid='" + code + "' AND ISCUR='Y'").Tables[0];
            if (cdt.Rows.Count > 0)
            {
                if (cdt.Rows[0][0].ToString().Equals("Y"))
                {
                    btnSave.Enabled = false;
                    btnSubmit.Enabled = false;
                    btnRollBack.Enabled = false;
                    btnReject.Enabled = false;
                    btnAudit.Enabled = false;
                }
                else if (cdt.Rows[0][0].ToString().Equals("S"))
                {
                    btnSave.Enabled = false;
                }
                else if (cdt.Rows[0][0].ToString().Equals("N") || cdt.Rows[0][0].ToString().Equals("R"))
                {
                    btnSave.Enabled = true;
                    btnSubmit.Enabled = true;
                    btnRollBack.Enabled = true;
                    btnAudit.Enabled = true;
                    btnReject.Enabled = true;
                }
            }
            else
            {
                btnSave.Enabled = true;
                btnSubmit.Enabled = true;
                btnRollBack.Enabled = true;
                btnAudit.Enabled = true;
                btnReject.Enabled = true;
            }
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            string picurl = @"";
            if (e.EventArgument.IndexOf("mysave") >= 0)
            {
                JArray jaResult = JsonConvert.DeserializeObject<JArray>(hfdURL.Text);

                foreach (JToken jt in jaResult)
                {
                    picurl += jt["_raw"].ToString() + ",";
                }

                string seqno = "";
                DataTable LISDT = DbHelperOra.Query("SELECT SEQNO FROM DOC_LICENSE_LOG WHERE SUPID='" + hfdSUPID.Text + "' AND ISCUR='Y'").Tables[0];
                if (LISDT.Rows.Count > 0)
                {
                    seqno = LISDT.Rows[0][0].ToString();
                }
                else
                {
                    seqno = hfdSEQNO.Text;
                }

                //换证操作后，保存换证后的数据
                if (hfdisChange.Text.Equals("changec"))
                {
                    if (picurl.Length <= 0)
                    {
                        Alert.Show("请上传更换的新证！");
                        return;
                    }
                    picurl = picurl.Substring(0, picurl.Length - 1);
                    string[] arrays = picurl.Split(',');

                    //如果证照保存后想再次换证，直接删除掉已保存证照，再换上新证照
                    DataTable chkdt = DbHelperOra.Query("SELECT FLAG FROM DOC_LICENSE_LOG WHERE SEQNO='" + seqno + "' AND LICENSEID='" + hfdLISID.Text + "' AND ISCUR='Y'").Tables[0];
                    if (chkdt.Rows[0][0].ToString().Equals("N") || chkdt.Rows[0][0].ToString().Equals("R"))
                    {
                        List<CommandInfo> delCmd = new List<CommandInfo>();
                        delCmd.Add(new CommandInfo("delete from doc_license_supplier where seqno='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        delCmd.Add(new CommandInfo("delete from doc_license_log where seqno='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        delCmd.Add(new CommandInfo("delete from doc_license_img where seqno='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        DbHelperOra.ExecuteSqlTran(delCmd);
                    }
                    else
                    {
                        List<CommandInfo> liscommand = new List<CommandInfo>();
                        liscommand.Add(new CommandInfo("update doc_license_supplier set iscur='N' where seqno='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        liscommand.Add(new CommandInfo("update doc_license_log set iscur='N' where SEQNO='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        liscommand.Add(new CommandInfo("UPDATE DOC_LICENSE_IMG SET ISCUR='N' where SEQNO='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        DbHelperOra.ExecuteSqlTran(liscommand);
                    }

                    //换证之后，更新新录入的证照流水，其他证照流水保持不变。
                    string seqnostr = DbHelperOra.GetSingle("SELECT SEQ_LICENSE_PIC.NEXTVAL FROM DUAL").ToString();
                    string myseqno = "LIS" + hfdSUPID.Text + seqnostr;

                    //换证之后，将ISCHANGE字段表示成Y，表示已换证。新录入就是N.
                    List<CommandInfo> liscmd = new List<CommandInfo>();
                    liscmd.Add(new CommandInfo(@"insert into DOC_LICENSE_SUPPLIER(SEQNO,SUPID,SUPNAME,FLAG,LICENSETYPE,LICENSENAME,BEGRQ,ENDRQ,LRY,LRRQ,UPTTIME,LICENSEID,ISCUR,ISCHANGE) values('" + myseqno + "','" + hfdSUPID.Text + "','" + docSUPNAME.Text + "','N','SUP_LIC','" + docLISNAME.Text + "',to_date('" + dpkBEGRQ.Text + "','yyyy-mm-dd'),to_date('" + dpkENDRQ.Text + "','yyyy-mm-dd'),'" + UserAction.UserID + "',sysdate,sysdate,'" + hfdLISID.Text + "','Y','Y')", null));
                    liscmd.Add(new CommandInfo(@"insert into doc_license_log(SEQNO,LICENSEID,LICENSENAME,FLAG,LICTYPE,SUPNAME,SUPID,PICNUM,OPERTIME,MEMO,ISCUR,ISCHANGE)VALUES('" + myseqno + "','" + hfdLISID.Text + "','" + docLISNAME.Text + "','N','SUP_LIC','" + docSUPNAME.Text + "','" + hfdSUPID.Text + "','" + arrays.Length + "',SYSDATE,'" + docMEMO.Text + "','Y','Y')", null));
                    for (int i = 1; i < arrays.Length + 1; i++)
                    {
                        liscmd.Add(new CommandInfo(@"insert into doc_license_img(SEQNO,ROWNO,IMGPATH,UPTTIME,LICENSEID,ISCUR,STR1)values('" + myseqno + "','" + i + "','" + arrays[i - 1] + "',sysdate,'" + hfdLISID.Text + "','Y','" + hfdSUPID.Text + "')", null));
                    }
                    DataTable changeDT = DbHelperOra.Query("SELECT COUNT(1) FROM DOC_LICENSE_LOG WHERE SUPID='" + hfdSUPID.Text + "' AND ISCUR='Y' AND LICENSEID<>'" + hfdLISID.Text + "'").Tables[0];
                    if (changeDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo(@"update DOC_LICENSE_SUPPLIER set seqno='" + myseqno + "' WHERE SUPID='" + hfdSUPID.Text + "' and iscur='Y'", null));
                        liscmd.Add(new CommandInfo(@"update doc_license_log set seqno='" + myseqno + "' WHERE SUPID='" + hfdSUPID.Text + "' and iscur='Y'", null));
                        liscmd.Add(new CommandInfo(@"update doc_license_img set seqno='" + myseqno + "' where SUPID='" + hfdSUPID.Text + "' and iscur='Y'", null));
                    }

                    if (DbHelperOra.ExecuteSqlTran(liscmd))
                    {
                        Alert.Show("证照上传成功！");
                        btnSubmit.Enabled = true;

                        string supid = hfdSUPID.Text;
                        //保存成功之后重新绑定证照基本信息
                        DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                                    B.NAME,
                                                                    B.ISNEED,
                                                                    T.SEQNO,
                                                                    T.LICENSEID,
                                                                    T.LICENSENAME,
                                                                    T.SUPNAME,
                                                                    T.SUPID,
                                                                    T.OPERTIME,
                                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
                                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
                                                                FROM (SELECT T.SEQNO,T.SUPID,T.SUPNAME,T.LICENSEID, T.LICENSENAME,T.OPERTIME,T.FLAG,T.PICNUM
                                                                        FROM DOC_LICENSE_LOG T
                                                                        WHERE SUPID = '" + supid + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC' ").Tables[0];
                        GridLicense.DataSource = lisDT;
                        GridLicense.DataBind();

                        DataSearch();
                    }
                    else
                    {
                        Alert.Show("证照上传失败！");
                    }
                }
                else
                {
                    DataTable goodsDT = DbHelperOra.Query("SELECT COUNT(1)ccount FROM DOC_LICENSE_SUPPLIER WHERE SUPID='" + hfdSUPID.Text + "' AND LICENSEID='" + hfdLISID.Text + "' and iscur='Y'").Tables[0];
                     if (Convert.ToInt32(goodsDT.Rows[0][0].ToString()) > 0)
                     {
                         List<CommandInfo> delpicmd = new List<CommandInfo>();
                         //执行update操作
                         if (picurl.Length<=0)
                         {
                             delpicmd.Add(new CommandInfo("update doc_license_supplier set docid='" + docDOCID.Text + "',memo='" + docMEMO.Text + "',begrq=to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),endrq=to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd') where supid='" + hfdSUPID.Text + "' and licenseid='" + hfdLISID.Text + "'", null));
                         }
                         else
                         {
                             picurl = picurl.Substring(0, picurl.Length - 1);
                             string[] arrays = picurl.Split(',');
                             //删除图片，然后换上新图片
                             DbHelperOra.ExecuteSql("delete from doc_license_img where supid='" + hfdSUPID.Text + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'");
                             delpicmd.Add(new CommandInfo("update doc_license_supplier set docid='" + docDOCID.Text + "',memo='" + docMEMO.Text + "',begrq=to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),endrq=to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd') where supid='" + hfdSUPID.Text + "' and licenseid='" + hfdLISID.Text + "'", null));
                             delpicmd.Add(new CommandInfo("update doc_license_log set picnum='" + arrays.Length + "' where supid='" + hfdSUPID.Text + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                             for (int i = 1; i < arrays.Length + 1; i++)
                             {
                                 delpicmd.Add(new CommandInfo(@"insert into doc_license_img(SEQNO,ROWNO,IMGPATH,UPTTIME,LICENSEID,SUPID,ISCUR)values('" + seqno + "','" + i + "','" + arrays[i - 1] + "',sysdate,'" + hfdLISID.Text + "','" + hfdSUPID.Text + "','Y')", null));
                             }
                         }
                         if (DbHelperOra.ExecuteSqlTran(delpicmd))
                         {
                             Alert.Show("证照上传成功！");
                             btnSubmit.Enabled = true;

                             string supid = hfdSUPID.Text;
                             //保存成功之后重新绑定证照基本信息
                             DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                                    B.NAME,
                                                                    B.ISNEED,
                                                                    T.SEQNO,
                                                                    T.LICENSEID,
                                                                    T.LICENSENAME,
                                                                    T.SUPNAME,
                                                                    T.SUPID,
                                                                    T.OPERTIME,
                                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
                                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
                                                                FROM (SELECT T.SEQNO,T.SUPID,T.SUPNAME,T.LICENSEID, T.LICENSENAME,T.OPERTIME,T.FLAG,T.PICNUM
                                                                        FROM DOC_LICENSE_LOG T
                                                                        WHERE SUPID = '" + supid + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC' ").Tables[0];
                             GridLicense.DataSource = lisDT;
                             GridLicense.DataBind();
                             DataSearch();
                         }
                     }
                    else
                     {
                         if (picurl.Length <= 0)
                         {
                             Alert.Show("请上传证照图片！");
                             return;
                         }
                         picurl = picurl.Substring(0, picurl.Length - 1);
                         string[] arrays = picurl.Split(',');

                         List<CommandInfo> liscmd = new List<CommandInfo>();
                         liscmd.Add(new CommandInfo(@"insert into doc_license_supplier(SEQNO,SUPID,SUPNAME,FLAG,LICENSETYPE,LICENSENAME,LICENSEID,BEGRQ,ENDRQ,LRY,LRRQ,UPTTIME,ISCUR,DOCID,MEMO,ISCHANGE)values('" + seqno + "','" + hfdSUPID.Text + "','" + docSUPNAME.Text + "','N','SUP_LIC','" + docLISNAME.Text + "','" + hfdLISID.Text + "',to_date('" + dpkBEGRQ.Text + "','yyyy-mm-dd'),to_date('" + dpkENDRQ.Text + "','yyyy-mm-dd'),'" + UserAction.UserID + "',sysdate,sysdate,'Y','" + docDOCID.Text + "','" + docMEMO.Text + "','N')", null));
                         liscmd.Add(new CommandInfo(@"insert into doc_license_log(SEQNO,LICENSEID,LICENSENAME,FLAG,LICTYPE,SUPNAME,SUPID,PICNUM,OPERTIME,MEMO,ISCUR,ISCHANGE)VALUES('" + seqno + "','" + hfdLISID.Text + "','" + docLISNAME.Text + "','N','SUP_LIC','" + docSUPNAME.Text + "','" + hfdSUPID.Text + "','" + arrays.Length + "',SYSDATE,'" + docMEMO.Text + "','Y','N')", null));
                         for (int i = 1; i < arrays.Length + 1; i++)
                         {
                             liscmd.Add(new CommandInfo(@"insert into doc_license_img(SEQNO,ROWNO,IMGPATH,UPTTIME,LICENSEID,SUPID,ISCUR)values('" + seqno + "','" + i + "','" + arrays[i - 1] + "',sysdate,'" + hfdLISID.Text + "','" + hfdSUPID.Text + "','Y')", null));
                         }
                         if (DbHelperOra.ExecuteSqlTran(liscmd))
                         {
                             Alert.Show("证照上传成功！");
                             btnSubmit.Enabled = true;

                             string supid = hfdSUPID.Text;
                             //保存成功之后重新绑定证照基本信息
                             DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                                    B.NAME,
                                                                    B.ISNEED,
                                                                    T.SEQNO,
                                                                    T.LICENSEID,
                                                                    T.LICENSENAME,
                                                                    T.SUPNAME,
                                                                    T.SUPID,
                                                                    T.OPERTIME,
                                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
                                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
                                                                FROM (SELECT T.SEQNO,T.SUPID,T.SUPNAME,T.LICENSEID, T.LICENSENAME,T.OPERTIME,T.FLAG,T.PICNUM
                                                                        FROM DOC_LICENSE_LOG T
                                                                        WHERE SUPID = '" + supid + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC' ").Tables[0];
                             GridLicense.DataSource = lisDT;
                             GridLicense.DataBind();
                             DataSearch();
                         }
                         else
                         {
                             Alert.Show("证照上传失败！");
                         }
                     }
                   
                }
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (dpkBEGRQ.Text.Length <= 0)
            {
                Alert.Show("请为 注册日期* 提供有效值！");
                return;
            }
            if (dpkENDRQ.Text.Length <= 0)
            {
                Alert.Show("请为 到期日期* 提供有效值！");
                return;
            }


            PageContext.RegisterStartupScript("save();");
        }

        protected void ischk_CheckedChanged(object sender, CheckedEventArgs e)
        {
            if(ischk.Checked)
            {
                if (string.IsNullOrWhiteSpace(dpkENDRQ.Text))
                {
                    dpkENDRQ.Text = "2099-01-01";
                    dpkENDRQ.Enabled = false;
                }
                else
                {
                    if (hfdFLAG.Text.Equals("已保存") || hfdFLAG.Text.Equals("已驳回"))
                    {
                        dpkENDRQ.Text = "";
                        dpkENDRQ.Enabled = true;
                    }
                }
            }
            else
            {
                dpkENDRQ.Text = "";
                dpkENDRQ.Enabled = true;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int[] selections = GridLicense.SelectedRowIndexArray;
            if (GridLicense.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要提交的机构证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            //查询证照类别表，绑定是否必须
            DataTable typLicDT = DbHelperOra.Query("SELECT CODE,NAME,ISNEED FROM DOC_LICENSE WHERE OBJUSER='SUP_LIC'").Tables[0];
            for (int i = 0; i < typLicDT.Rows.Count; i++)
            {
                DataTable supplierLicDT = DbHelperOra.Query("SELECT LICENSEID FROM DOC_LICENSE_SUPPLIER WHERE SUPID='" + hfdSUPID.Text + "' AND LICENSEID='" + typLicDT.Rows[i]["CODE"].ToString() + "' AND ISCUR='Y'").Tables[0];
                if (typLicDT.Rows[i][2].ToString().Equals("Y") && supplierLicDT.Rows.Count <= 0)
                {
                    Alert.Show("【" + typLicDT.Rows[i][1].ToString() + "】" + "是必须要记录的，请把必须的证照补充完整！");
                    return;
                }
            }

            List<CommandInfo> liscmd = new List<CommandInfo>();
            string succeed = string.Empty;
            bool ishave = false;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLicense.SelectedRowIndexArray[i];

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_SUPPLIER WHERE SUPID='" + hfdSUPID.Text + "' AND ISCUR='Y'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where SUPID='" + hfdSUPID.Text + "' AND ISCUR='Y'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo("update doc_license_log set flag='S' where SUPID='" + hfdSUPID.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
                        liscmd.Add(new CommandInfo("update DOC_LICENSE_SUPPLIER set flag='S' where SUPID='" + hfdSUPID.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
                    }
                    ishave = true;
                }
                else
                {
                    ishave = false;
                }
            }
            if (ishave)
            {
                if (DbHelperOra.ExecuteSqlTran(liscmd))
                {
                    Alert.Show("提交成功！");
                    //btnSubmit.Enabled = false;
                    string supcode = hfdSUPID.Text;
                    //保存成功之后重新绑定证照基本信息
                    DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    B.ISNEED,
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
                                                        WHERE SUPID = '" + hfdSUPID.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC'").Tables[0];

                    GridLicense.DataSource = lisDT;
                    GridLicense.DataBind();
                    DataSearch();
                    btnSave.Enabled = false;
                    btnSubmit.Enabled = false;
                    btnRollBack.Enabled = true;
                    btnAudit.Enabled = true;
                    btnReject.Enabled = true;
                }
                else
                {
                    Alert.Show("操作失败！", MessageBoxIcon.Warning);
                }
            }
        }

        protected void btnRollBack_Click(object sender, EventArgs e)
        {
//            DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_SUPPLIER WHERE SUPID='" + hfdSUPID.Text + "'").Tables[0];
//            if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
//            {
//                List<CommandInfo> liscmd = new List<CommandInfo>();
//                liscmd.Add(new CommandInfo("update doc_license_log set flag='N' where SUPID='" + hfdSUPID.Text + "'", null));
//                liscmd.Add(new CommandInfo("update doc_license_SUPPLIER set flag='N' where SUPID='" + hfdSUPID.Text + "'", null));
//                if (DbHelperOra.ExecuteSqlTran(liscmd))
//                {
//                    Alert.Show("【'" + docSUPNAME.Text + "'】证照信息已成功撤回！");
//                }
//                else
//                {
//                    Alert.Show("撤回失败！");
//                }


//                string supcode = hfdSUPID.Text;
//                //保存成功之后重新绑定证照基本信息
//            DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
//                                                    B.NAME,
//                                                    T.SEQNO,
//                                                    T.LICENSEID,
//                                                    T.LICENSENAME,
//                                                    T.SUPNAME,
//                                                    T.SUPID,
//                                                    T.OPERTIME,
//                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
//                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
//                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.SUPID, T.SUPNAME,T.OPERTIME,T.FLAG,T.PICNUM
//                                                        FROM DOC_LICENSE_LOG T
//                                                        WHERE SUPID = '" + hfdSUPID.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC'").Tables[0];

//                GridLicense.DataSource = lisDT;
//                GridLicense.DataBind();
//                DataSearch();
            //btnSave.Enabled = true;
            //btnSubmit.Enabled = false;
            //btnRollBack.Enabled = false;
            //btnAudit.Enabled = false;
            //btnReject.Enabled = false;
//            }
//            else
//            {
//                Alert.Show("没有待撤回的机构证照信息！");
//            }


            int[] selections = GridLicense.SelectedRowIndexArray;
            if (GridLicense.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要提交的机构证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<CommandInfo> liscmd = new List<CommandInfo>();
            string succeed = string.Empty;
            bool ishave = false;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLicense.SelectedRowIndexArray[i];

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_SUPPLIER WHERE SUPID='" + hfdSUPID.Text + "' AND ISCUR='Y'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where SUPID='" + hfdSUPID.Text + "' AND ISCUR='Y'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo("update doc_license_log set flag='N' where SUPID='" + hfdSUPID.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
                        liscmd.Add(new CommandInfo("update DOC_LICENSE_SUPPLIER set flag='N' where SUPID='" + hfdSUPID.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
                    }
                    ishave = true;
                }
                else
                {
                    ishave = false;
                }
            }
            if (ishave)
            {
                if (DbHelperOra.ExecuteSqlTran(liscmd))
                {
                    Alert.Show("【'" + docSUPNAME.Text + "'】证照信息已成功撤回！");
                    //btnSubmit.Enabled = false;
                    string supcode = hfdSUPID.Text;
                    //保存成功之后重新绑定证照基本信息
                    DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    B.ISNEED,
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
                                                        WHERE SUPID = '" + hfdSUPID.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC'").Tables[0];
                    GridLicense.DataSource = lisDT;
                    GridLicense.DataBind();
                    DataSearch();
                    btnSave.Enabled = true;
                    btnSubmit.Enabled = false;
                    btnRollBack.Enabled = false;
                    btnAudit.Enabled = false;
                    btnReject.Enabled = false;
                }
                else
                {
                    Alert.Show("操作失败！", MessageBoxIcon.Warning);
                }
            }
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
//            DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_SUPPLIER WHERE SUPID='" + hfdSUPID.Text + "'").Tables[0];
//            if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
//            {
//                List<CommandInfo> liscmd = new List<CommandInfo>();
//                liscmd.Add(new CommandInfo("update doc_license_log set flag='Y' where SUPID='" + hfdSUPID.Text + "'", null));
//                liscmd.Add(new CommandInfo("update doc_license_SUPPLIER set flag='Y' where SUPID='" + hfdSUPID.Text + "'", null));
//                if (DbHelperOra.ExecuteSqlTran(liscmd))
//                {
//                    Alert.Show("审核成功！");
//                    btnSubmit.Enabled = false;
//                }
//                else
//                {
//                    Alert.Show("审核失败！");
//                }

//                string supcode = hfdSUPID.Text;
//                //保存成功之后重新绑定证照基本信息
//            DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
//                                                    B.NAME,
//                                                    T.SEQNO,
//                                                    T.LICENSEID,
//                                                    T.LICENSENAME,
//                                                    T.SUPNAME,
//                                                    T.SUPID,
//                                                    T.OPERTIME,
//                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
//                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
//                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.SUPID, T.SUPNAME,T.OPERTIME,T.FLAG,T.PICNUM
//                                                        FROM DOC_LICENSE_LOG T
//                                                        WHERE SUPID = '" + hfdSUPID.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC'").Tables[0];

//                GridLicense.DataSource = lisDT;
//                GridLicense.DataBind();
//                DataSearch();
            //btnSave.Enabled = false;
            //btnSubmit.Enabled = false;
            //btnRollBack.Enabled = false;
            //btnAudit.Enabled = false;
            //btnReject.Enabled = false;
//            }
//            else
//            {
//                Alert.Show("没有待审核的机构证照信息！");
//            }


            int[] selections = GridLicense.SelectedRowIndexArray;
            if (GridLicense.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要提交的机构证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<CommandInfo> liscmd = new List<CommandInfo>();
            string succeed = string.Empty;
            bool ishave = false;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLicense.SelectedRowIndexArray[i];

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_SUPPLIER WHERE SUPID='" + hfdSUPID.Text + "' AND ISCUR='Y'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where SUPID='" + hfdSUPID.Text + "' AND ISCUR='Y'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo("update doc_license_log set flag='Y' where SUPID='" + hfdSUPID.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
                        liscmd.Add(new CommandInfo("update DOC_LICENSE_SUPPLIER set flag='Y' where SUPID='" + hfdSUPID.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
                    }
                    ishave = true;
                }
                else
                {
                    ishave = false;
                }
            }
            if (ishave)
            {
                if (DbHelperOra.ExecuteSqlTran(liscmd))
                {
                    Alert.Show("审核成功！");
                    //btnSubmit.Enabled = false;
                    string supcode = hfdSUPID.Text;
                    //保存成功之后重新绑定证照基本信息
                    DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    B.ISNEED,
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
                                                        WHERE SUPID = '" + hfdSUPID.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC'").Tables[0];
                    GridLicense.DataSource = lisDT;
                    GridLicense.DataBind();
                    if (Request.QueryString["oper"].ToString() == "gl")
                    {
                        DataSearch();
                    }
                    else if (Request.QueryString["oper"].ToString() == "sh")
                    {
                        DataSearch1();
                        docLISNAME.Text = "";
                        docDOCID.Text = "";
                        docMEMO.Text = "";
                        docSUPNAME.Text = "";
                        dpkBEGRQ.Text = "";
                        dpkENDRQ.Text = "";
                        ischk.Checked = false;
                        dpkENDRQ.Enabled = true;
                    }
                    else if (Request.QueryString["oper"].ToString() == "change")
                    {
                        DataSearch2();
                    }
                    else
                    {
                        DataSearch3();
                    }
                    btnSave.Enabled = false;
                    btnSubmit.Enabled = false;
                    btnRollBack.Enabled = false;
                    btnAudit.Enabled = false;
                    btnReject.Enabled = false;
                }
                else
                {
                    Alert.Show("操作失败！", MessageBoxIcon.Warning);
                }
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
//            DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_SUPPLIER WHERE SUPID='" + hfdSUPID.Text + "'").Tables[0];
//            if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
//            {
//                List<CommandInfo> liscmd = new List<CommandInfo>();
//                liscmd.Add(new CommandInfo("update doc_license_log set flag='R' where SUPID='" + hfdSUPID.Text + "'", null));
//                liscmd.Add(new CommandInfo("update doc_license_SUPPLIER set flag='R' where SUPID='" + hfdSUPID.Text + "'", null));
//                if (DbHelperOra.ExecuteSqlTran(liscmd))
//                {
//                    Alert.Show("驳回成功！");
//                    btnSubmit.Enabled = false;
//                }
//                else
//                {
//                    Alert.Show("驳回失败！");
//                }


//                string supcode = hfdSUPID.Text;
//                //保存成功之后重新绑定证照基本信息
//            DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
//                                                    B.NAME,
//                                                    T.SEQNO,
//                                                    T.LICENSEID,
//                                                    T.LICENSENAME,
//                                                    T.SUPNAME,
//                                                    T.SUPID,
//                                                    T.OPERTIME,
//                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
//                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
//                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.SUPID, T.SUPNAME,T.OPERTIME,T.FLAG,T.PICNUM
//                                                        FROM DOC_LICENSE_LOG T
//                                                        WHERE SUPID = '" + hfdSUPID.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC'").Tables[0];

//                GridLicense.DataSource = lisDT;
//                GridLicense.DataBind();
//                DataSearch();
            //btnSave.Enabled = true;
            //btnSubmit.Enabled = false;
            //btnRollBack.Enabled = false;
            //btnAudit.Enabled = false;
            //btnReject.Enabled = false;
//            }
//            else
//            {
//                Alert.Show("没有待驳回的证照信息！");
//            }


            int[] selections = GridLicense.SelectedRowIndexArray;
            if (GridLicense.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要提交的机构证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<CommandInfo> liscmd = new List<CommandInfo>();
            string succeed = string.Empty;
            bool ishave = false;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLicense.SelectedRowIndexArray[i];

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_SUPPLIER WHERE SUPID='" + hfdSUPID.Text + "' AND ISCUR='Y'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where SUPID='" + hfdSUPID.Text + "' AND ISCUR='Y'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo("update doc_license_log set flag='R' where SUPID='" + hfdSUPID.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
                        liscmd.Add(new CommandInfo("update DOC_LICENSE_SUPPLIER set flag='R' where SUPID='" + hfdSUPID.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
                    }
                    ishave = true;
                }
                else
                {
                    ishave = false;
                }
            }
            if (ishave)
            {
                if (DbHelperOra.ExecuteSqlTran(liscmd))
                {
                    Alert.Show("审核成功！");
                    //btnSubmit.Enabled = false;
                    string supcode = hfdSUPID.Text;
                    //保存成功之后重新绑定证照基本信息
                    DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    B.ISNEED,
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
                                                        WHERE SUPID = '" + hfdSUPID.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC'").Tables[0];
                    GridLicense.DataSource = lisDT;
                    GridLicense.DataBind();
                    if (Request.QueryString["oper"].ToString() == "gl")
                    {
                        DataSearch();
                    }
                    else if (Request.QueryString["oper"].ToString() == "sh")
                    {
                        DataSearch1();
                    }
                    else if (Request.QueryString["oper"].ToString() == "change")
                    {
                        DataSearch2();
                    }
                    else
                    {
                        DataSearch3();
                    }
                    btnSave.Enabled = true;
                    btnSubmit.Enabled = false;
                    btnRollBack.Enabled = false;
                    btnAudit.Enabled = false;
                    btnReject.Enabled = false;
                }
                else
                {
                    Alert.Show("操作失败！", MessageBoxIcon.Warning);
                }
            }

        }

        protected void GridLicense_RowClick(object sender, GridRowClickEventArgs e)
        {
            string LISID = GridLicense.Rows[e.RowIndex].DataKeys[4].ToString();
            string LISNAME = GridLicense.Rows[e.RowIndex].DataKeys[5].ToString();
            hfdLISID.Text = LISID;
            string HSEQNO = "";
            docLISNAME.Text = LISNAME;
            string flag = GridLicense.Rows[e.RowIndex].DataKeys[7].ToString();
            if (flag.Equals("已保存"))
            {
                btnSave.Enabled = true;
                btnSubmit.Enabled = true;
                btnRollBack.Enabled = false;
                btnAudit.Enabled = false;
                btnReject.Enabled = false;
            }
            else if (flag.Equals("已提交"))
            {
                btnSave.Enabled = false;
                btnSubmit.Enabled = false;
                btnRollBack.Enabled = true;
                btnAudit.Enabled = true;
                btnReject.Enabled = true;
            }
            else if (flag.Equals("已审核"))
            {
                btnSave.Enabled = false;
                btnSubmit.Enabled = false;
                btnRollBack.Enabled = false;
                btnAudit.Enabled = false;
                btnReject.Enabled = false;
            }
            else
            {
                btnSave.Enabled = true;
                btnSubmit.Enabled = false;
                btnRollBack.Enabled = false;
                btnAudit.Enabled = false;
                btnReject.Enabled = false;
            }


            DataTable SUDT = DbHelperOra.Query("SELECT COUNT(1)mycount FROM DOC_LICENSE_LOG WHERE SUPID='" + hfdSUPID.Text + "' AND LICENSEID='" + LISID + "'").Tables[0];
            if(Convert.ToInt32(SUDT.Rows[0][0].ToString())>0)
            {
                HSEQNO = GridLicense.Rows[e.RowIndex].DataKeys[0].ToString();
                hfdHSEQNO.Text = HSEQNO;
                hfdSUPID.Text = GridLicense.Rows[e.RowIndex].DataKeys[1].ToString();
                hfdFLAG.Text = GridLicense.Rows[e.RowIndex].DataKeys[7].ToString();
                DataTable licensedt = DbHelperOra.Query("select trunc(begrq) begrq,trunc(endrq) endrq,docid,memo from doc_license_supplier where seqno='" + HSEQNO + "' and licenseid='" + LISID + "' AND ISCUR='Y'").Tables[0];
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
            else
            {
                DataTable dt = DbHelperOra.Query("select flag from doc_license_log where supid = '" + hfdSUPID.Text + "' AND ISCUR='Y' and rownum=1").Tables[0];
                if (dt.Rows.Count>0)
                {
                    if(dt.Rows[0][0].ToString().Equals("Y") || dt.Rows[0][0].ToString().Equals("S"))
                    {

                    }
                    else
                    {
                        hfdHSEQNO.Text = @"";
                        hfdLISID.Text = LISID;
                        hfdLISNAME.Text = LISNAME;
                        btnSave.Enabled = true;
                        dpkBEGRQ.Text = "";
                        dpkENDRQ.Text = @"";
                        dpkENDRQ.Enabled = true;
                        ischk.Checked = false;
                        docDOCID.Text = "";
                        docMEMO.Text = "";
                    }
                }

                PageContext.RegisterStartupScript("clearUpload();");

                string myseq = "";
                if (DbHelperOra.Exists("select 1 from doc_license_log where gdseq='" + hfdSUPID.Text + "' AND ISCUR='Y'"))
                {
                    DataTable mydt = DbHelperOra.Query("select seqno,licenseid from doc_license_log where gdseq='" + hfdSUPID.Text + "' AND ISCUR='Y'").Tables[0];
                    string seqno = mydt.Rows[0]["seqno"].ToString();
                    myseq = seqno + "|" + hfdSUPID.Text + "|" + LISID + "|" + hfdSUPID.Text;
                }
                else
                {
                    string seqnostr = DbHelperOra.GetSingle("SELECT SEQ_LICENSE_PIC.NEXTVAL FROM DUAL").ToString();
                    string seqno = "LIS" + hfdSUPID.Text + seqnostr;
                    myseq = seqno + "|" + hfdSUPID.Text + "|" + LISID + "|" + hfdSUPID.Text;
                    hfdSEQNO.Text = seqno;
                }
                PageContext.RegisterStartupScript("initUpload1('" + myseq + "');");
            }
        }

        protected void GridLicense_RowCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "htpic")
            {
                try
                {
                    string LISID = GridLicense.Rows[e.RowIndex].DataKeys[4].ToString();
                    string LISNAME = GridLicense.Rows[e.RowIndex].DataKeys[5].ToString();
                    string HSEQNO = "";
                    HSEQNO = GridLicense.Rows[e.RowIndex].DataKeys[0].ToString();

                    string picnum = DbHelperOra.GetSingle("select count(1) from DOC_LICENSE_IMG where seqno='" + HSEQNO + "' and licenseid='" + LISID + "' AND ISCUR='Y'").ToString();
                    string url = "~/CertificateInput/ShowLisPicWindow.aspx?bm=" + HSEQNO + "&xc=" + picnum + "&cc=" + LISID + "";
                    PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdSEQNO.ClientID) + Window1.GetShowReference(url, "证照图片展示"));
                }
                catch (Exception)
                {
                    Alert.Show("暂无图片展示,请上传图片后再查看！");
                }
            }
            if (e.CommandName == "changeCert")
            {
                string supid = hfdSUPID.Text;
                DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    B.ISNEED,
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
                                                        WHERE SUPID = '" + supid + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'SUP_LIC'").Tables[0];


                string opertime = lisDT.Rows[e.RowIndex]["OPERTIME"].ToString();
                string flag = lisDT.Rows[e.RowIndex]["FLAG"].ToString();
                if (string.IsNullOrWhiteSpace(opertime))
                {
                    Alert.Show("您还没有上传证照信息，不用执行换证操作！");
                    return;
                }
                if (flag.Equals("已提交"))
                {
                    Alert.Show("已提交的证照正在等待审核，不允许更换！");
                    return;
                }

                string LISID = GridLicense.Rows[e.RowIndex].DataKeys[4].ToString();
                string LISNAME = GridLicense.Rows[e.RowIndex].DataKeys[5].ToString();
                hfdHSEQNO.Text = @"";
                hfdLISID.Text = LISID;
                hfdLISNAME.Text = LISNAME;
                dpkBEGRQ.Text = "";
                dpkENDRQ.Text = @"";
                dpkENDRQ.Enabled = true;
                ischk.Checked = false;
                btnSave.Enabled = true;
                btnSubmit.Enabled = false;
                btnAudit.Enabled = false;
                btnRollBack.Enabled = false;
                hfdisChange.Text = "changec";
                docDOCID.Text = "";
                docMEMO.Text = "";
                PageContext.RegisterStartupScript("clearUpload();");
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
                    string picnum = DbHelperOra.GetSingle("select count(1) from DOC_LICENSE_IMG where seqno='" + HSEQNO + "' and licenseid='" + LISID + "' AND ISCUR='Y'").ToString();
                    string url = "~/CertificateInput/ShowLisPicWindow.aspx?bm=" + HSEQNO + "&xc=" + picnum + "&cc=" + LISID + "";
                    PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdSEQNO.ClientID) + Window1.GetShowReference(url, "证照图片展示"));
                }
                catch (Exception)
                {
                    Alert.Show("暂无图片展示,请上传图片后再查看！");
                }
            }
        }

        protected void GridCertype_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string isneed = row["ISNEED"].ToString();
                if (isneed.Equals("Y"))
                {
                    e.CellCssClasses[1] = "color2";       //单列加颜色(如果只想将单元格的字体变色，只需要将样式表里的background-color的属性去掉，加上color，然后alpha设置透明度即可)
                }
            }
        }

        protected void GridLicense_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string isneed = row["ISNEED"].ToString();
                if (isneed.Equals("Y"))
                {
                    e.CellCssClasses[1] = "color2";       //单列加颜色(如果只想将单元格的字体变色，只需要将样式表里的background-color的属性去掉，加上color，然后alpha设置透明度即可)
                }
            }
        }

    }
}
﻿using System;
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

namespace ERPProject.CertificateInput
{
    public partial class GoodCertsInput : PageBase
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
                        ButtonHidden(btnAudit, btnReject, mybtnAudit, mybtnReject);
                        DataSearch();
                    }
                    else if (Request.QueryString["oper"].ToString() == "sh")
                    {
                        ButtonHidden(btnHistory, btnRollBack, btnSave, btnSubmit);
                        chkisLR.Hidden = true;
                        chknoLR.Hidden = true;
                        DataSearch1();
                    }
                    else if (Request.QueryString["oper"].ToString() == "change")
                    {
                        ButtonHidden(btnAudit, btnReject, mybtnAudit, mybtnReject);
                        chkisLR.Hidden = true;
                        chknoLR.Hidden = true;
                        DataSearch2();
                    }
                    else
                    {
                        ButtonHidden(btnHistory, btnRollBack, btnSave, btnSubmit);
                        chkisLR.Hidden = true;
                        chknoLR.Hidden = true;
                        DataSearch3();
                    }
                }
                else
                {
                    DataSearch();
                }
            }
        }

        private void DataInit()
        {
            //默认第一个tab页面
            TabStrip1.ActiveTabIndex = 0;
            //默认绑定商品证照类别表
            DataTable MYDT = DbHelperOra.Query("SELECT CODE LISID,NAME LISNAME,ISNEED FROM DOC_LICENSE WHERE OBJUSER='GOODS_LIC'").Tables[0];
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
            string strSQL = "SELECT GDSEQ,GDNAME,decode(flag,'Y','已审核','N','已保存','S','已提交','R','已驳回','待录入')flag, ";
            string strISBEGIN = @"";
            string strSQLBEG = @"FROM(select ta.gdseq,ta.gdname,(select flag from doc_license_log where gdseq=ta.gdseq AND ISCUR='Y' and rownum=1) flag, ";
            string strSQLEND = @"from (select a.gdseq, a.gdname, b.code, b.name
                                    from doc_goods a, doc_license b where b.objuser='GOODS_LIC') ta, doc_license_log tb
                            where ta.gdseq = tb.gdseq(+) and ta.name = tb.licensename(+)";
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
                strSQLEND = strSQLEND + "  and (ta.gdseq like '%" + txtName.Text + "%' or ta.gdname like '%" + txtName.Text + "%')";
            }
            //if(!string.IsNullOrWhiteSpace(ddlFLAG.SelectedValue))
            //{
            //    strSQLEND = strSQLEND + "  and flag in (select flag from doc_license_log where gdseq=ta.gdseq and flag='"+ddlFLAG.SelectedValue+"')";
            //}
            if (chkisLR.Checked)
            {
                strSQLEND = strSQLEND + "  and flag in (select flag from doc_license_log where gdseq=ta.gdseq and iscur='Y')";
            }
            if (chknoLR.Checked)
            {
                strSQLEND = strSQLEND + "  and flag not in (select flag from doc_license_log where gdseq=ta.gdseq and iscur='Y')";
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
                                                    T.GDNAME,
                                                    T.GDSEQ,
                                                    T.OPERTIME,
                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.GDSEQ, T.GDNAME,T.OPERTIME,T.FLAG,T.PICNUM
                                                        FROM DOC_LICENSE_LOG T
                                                        WHERE GDSEQ = '" + dt.Rows[0][0].ToString() + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];
                GridCertype.DataSource = lisDT;
                GridCertype.DataBind();
            }

        }

        private void DataSearch1()
        {
            string sql = "  and flag in (select flag from doc_license_log where gdseq=ta.gdseq and flag='S' and iscur='Y' AND ISCHANGE='N')";
            dsearch(sql);
        }

        private void dsearch(string mystrsql)
        {
            string strSQL = "SELECT GDSEQ,GDNAME,decode(flag,'Y','已审核','N','已保存','S','已提交','R','已驳回','待录入')flag, ";
            string strISBEGIN = @"";
            string strSQLBEG = @"FROM(select ta.gdseq,ta.gdname,(select flag from doc_license_log where gdseq=ta.gdseq and rownum=1) flag, ";
            string strSQLEND = @"from (select a.gdseq, a.gdname, b.code, b.name
                                    from doc_goods a, doc_license b where b.objuser='GOODS_LIC') ta, doc_license_log tb
                            where ta.gdseq = tb.gdseq(+) and ta.name = tb.licensename(+)";
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
                strSQLEND = strSQLEND + "  and (ta.gdseq like '%" + txtName.Text + "%' or ta.gdname like '%" + txtName.Text + "%')";
            }

            strSQLEND = strSQLEND + mystrsql;

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
                                                    T.GDNAME,
                                                    T.GDSEQ,
                                                    T.OPERTIME,
                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.GDSEQ, T.GDNAME,T.OPERTIME,T.FLAG,T.PICNUM
                                                        FROM DOC_LICENSE_LOG T
                                                        WHERE GDSEQ = '" + dt.Rows[0][0].ToString() + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];
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

        private void DataSearch2()
        {
            string sql = "  and flag in (select flag from doc_license_log where gdseq=ta.gdseq and flag<>'S' AND ISCUR='Y')";
            dsearch(sql);
        }

        private void DataSearch3()
        {
            string sql = "  and flag in (select flag from doc_license_log where gdseq=ta.gdseq and flag='S' and ischange='Y' AND ISCUR='Y')";
            dsearch(sql);
        }

        protected void GridLIS_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridLIS.PageIndex = e.NewPageIndex;
            DataSearch();
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
                DataTable LISDT = DbHelperOra.Query("SELECT SEQNO FROM DOC_LICENSE_LOG WHERE GDSEQ='" + hfdGDSEQ.Text + "' AND ISCUR='Y'").Tables[0];
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
                    if(picurl.Length<=0)
                    {
                        Alert.Show("请上传更换的新证！");
                        return;
                    }
                    //上传的图片张数(这部分是肯定存在图片的)
                    picurl = picurl.Substring(0, picurl.Length - 1);
                    string[] arrays = picurl.Split(',');
                    //////////

                    //如果证照是保存或撤回状态，直接删除掉已保存证照，再换上新证照
                    DataTable chkdt = DbHelperOra.Query("SELECT FLAG FROM DOC_LICENSE_LOG WHERE SEQNO='" + seqno + "' AND LICENSEID='" + hfdLISID.Text + "' AND ISCUR='Y'").Tables[0];
                    if (chkdt.Rows[0][0].ToString().Equals("N") || chkdt.Rows[0][0].ToString().Equals("R"))
                    {
                        List<CommandInfo> delCmd = new List<CommandInfo>();
                        delCmd.Add(new CommandInfo("delete from doc_license_goods where seqno='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        delCmd.Add(new CommandInfo("delete from doc_license_log where seqno='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        delCmd.Add(new CommandInfo("delete from doc_license_img where seqno='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        DbHelperOra.ExecuteSqlTran(delCmd);
                    }
                    else
                    {
                        //如果证照状态不是N或R的话，直接将现有证照变成老证照,ISCUR='N'
                        List<CommandInfo> liscommand = new List<CommandInfo>();
                        liscommand.Add(new CommandInfo("update doc_license_goods set iscur='N',ISCHANGE='N' where seqno='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        liscommand.Add(new CommandInfo("update doc_license_log set iscur='N',ISCHANGE='N' where SEQNO='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        liscommand.Add(new CommandInfo("UPDATE DOC_LICENSE_IMG SET ISCUR='N',ISCHANGE='N' where SEQNO='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        DbHelperOra.ExecuteSqlTran(liscommand);
                    }

                    //换证之后，更新新录入的证照流水，其他证照流水保持不变。插入本次换证的新证照
                    string seqnostr = DbHelperOra.GetSingle("SELECT SEQ_LICENSE_PIC.NEXTVAL FROM DUAL").ToString();
                    string myseqno = "LIS" + hfdGDSEQ.Text + seqnostr;

                    List<CommandInfo> liscmd = new List<CommandInfo>();
                    liscmd.Add(new CommandInfo(@"insert into doc_license_goods(SEQNO,GDSEQ,GDNAME,FLAG,LICENSETYPE,LICENSENAME,BEGRQ,ENDRQ,LRY,LRRQ,UPTTIME,LICENSEID,ISCUR,ISCHANGE) values('" + myseqno + "','" + hfdGDSEQ.Text + "','" + docGDNAME.Text + "','N','GOODS_LIC','" + docLISNAME.Text + "',to_date('" + dpkBEGRQ.Text + "','yyyy-mm-dd'),to_date('" + dpkENDRQ.Text + "','yyyy-mm-dd'),'" + UserAction.UserID + "',sysdate,sysdate,'" + hfdLISID.Text + "','Y','Y')", null));
                    liscmd.Add(new CommandInfo(@"insert into doc_license_log(SEQNO,LICENSEID,LICENSENAME,FLAG,LICTYPE,GDNAME,GDSEQ,PICNUM,OPERTIME,MEMO,ISCUR,ISCHANGE)VALUES('" + myseqno + "','" + hfdLISID.Text + "','" + docLISNAME.Text + "','N','GOODS_LIC','" + docGDNAME.Text + "','" + hfdGDSEQ.Text + "','" + arrays.Length + "',SYSDATE,'" + docMEMO.Text + "','Y','Y')", null));
                    for (int i = 1; i < arrays.Length + 1; i++)
                    {
                        liscmd.Add(new CommandInfo(@"insert into doc_license_img(SEQNO,ROWNO,IMGPATH,UPTTIME,LICENSEID,ISCUR,GDSEQ)values('" + myseqno + "','" + i + "','" + arrays[i - 1] + "',sysdate,'" + hfdLISID.Text + "','Y','" + hfdGDSEQ.Text + "')", null));
                    }
                    DataTable changeDT = DbHelperOra.Query("SELECT COUNT(1) FROM DOC_LICENSE_LOG WHERE GDSEQ='" + hfdGDSEQ.Text + "' AND ISCUR='Y' AND LICENSEID<>'" + hfdLISID.Text + "'").Tables[0];
                    if (changeDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo(@"update doc_license_goods set seqno='" + myseqno + "' WHERE gdseq='" + hfdGDSEQ.Text + "' and iscur='Y'", null));
                        liscmd.Add(new CommandInfo(@"update doc_license_log set seqno='" + myseqno + "' WHERE gdseq='" + hfdGDSEQ.Text + "' and iscur='Y'", null));
                        liscmd.Add(new CommandInfo(@"update doc_license_img set seqno='" + myseqno + "' where gdseq='" + hfdGDSEQ.Text + "' and iscur='Y'", null));
                    }

                    if (DbHelperOra.ExecuteSqlTran(liscmd))
                    {
                        Alert.Show("证照上传成功！");
                        btnSubmit.Enabled = true;

                        string gdseq = hfdGDSEQ.Text;
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
                                                                FROM (SELECT T.SEQNO,T.SUPID,T.SUPNAME,T.LICENSEID, T.LICENSENAME, T.GDSEQ, T.GDNAME,T.OPERTIME,T.FLAG,T.PICNUM
                                                                        FROM DOC_LICENSE_LOG T
                                                                        WHERE GDSEQ = '" + gdseq + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];
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
                    DataTable goodsDT = DbHelperOra.Query("SELECT COUNT(1)ccount FROM DOC_LICENSE_GOODS WHERE GDSEQ='" + hfdGDSEQ.Text + "' AND LICENSEID='" + hfdLISID.Text + "' and iscur='Y'").Tables[0];
                    if(Convert.ToInt32(goodsDT.Rows[0][0].ToString())>0)
                    {
                        List<CommandInfo> delpicmd = new List<CommandInfo>();
                        //执行update操作
                        if (picurl.Length<=0)
                        {
                            delpicmd.Add(new CommandInfo("update doc_license_goods set docid='" + docDOCID.Text + "',memo='" + docMEMO.Text + "',begrq=to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),endrq=to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd') where gdseq='" + hfdGDSEQ.Text + "' and licenseid='" + hfdLISID.Text + "'", null));
                        }
                        else
                        {
                            picurl = picurl.Substring(0, picurl.Length - 1);
                            string[] arrays = picurl.Split(',');
                            //删除图片，然后换上新图片
                            DbHelperOra.ExecuteSql("delete from doc_license_img where gdseq='" + hfdGDSEQ.Text + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'");
                            delpicmd.Add(new CommandInfo("update doc_license_goods set docid='" + docDOCID.Text + "',memo='" + docMEMO.Text + "',begrq=to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),endrq=to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd') where gdseq='" + hfdGDSEQ.Text + "' and licenseid='" + hfdLISID.Text + "'", null));
                            delpicmd.Add(new CommandInfo("update doc_license_log set picnum='" + arrays.Length + "' where gdseq='" + hfdGDSEQ.Text + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                            for (int i = 1; i < arrays.Length + 1; i++)
                            {
                                delpicmd.Add(new CommandInfo(@"insert into doc_license_img(SEQNO,ROWNO,IMGPATH,UPTTIME,LICENSEID,GDSEQ,ISCUR)values('" + seqno + "','" + i + "','" + arrays[i - 1] + "',sysdate,'" + hfdLISID.Text + "','" + hfdGDSEQ.Text + "','Y')", null));
                            }
                        }

                        if (DbHelperOra.ExecuteSqlTran(delpicmd))
                        {
                            Alert.Show("保存成功！");
                            btnSubmit.Enabled = true;

                            string gdseq = hfdGDSEQ.Text;
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
                                                                FROM (SELECT T.SEQNO,T.SUPID,T.SUPNAME,T.LICENSEID, T.LICENSENAME, T.GDSEQ, T.GDNAME,T.OPERTIME,T.FLAG,T.PICNUM
                                                                        FROM DOC_LICENSE_LOG T
                                                                        WHERE GDSEQ = '" + gdseq + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC' ").Tables[0];
                            GridLicense.DataSource = lisDT;
                            GridLicense.DataBind();
                            DataSearch();
                        }
                    }
                    else
                    {
                        if(picurl.Length<=0)
                        {
                            Alert.Show("请上传证照图片！");
                            return;
                        }
                        picurl = picurl.Substring(0, picurl.Length - 1);
                        string[] arrays = picurl.Split(',');
                        //保存证照信息
                        List<CommandInfo> liscmd = new List<CommandInfo>();
                        liscmd.Add(new CommandInfo(@"insert into doc_license_goods(SEQNO,GDSEQ,GDNAME,FLAG,LICENSETYPE,LICENSENAME,BEGRQ,ENDRQ,LRY,LRRQ,UPTTIME,LICENSEID,ISCUR,DOCID,MEMO) values('" + seqno + "','" + hfdGDSEQ.Text + "','" + docGDNAME.Text + "','N','GOODS_LIC','" + docLISNAME.Text + "',to_date('" + dpkBEGRQ.Text + "','yyyy-mm-dd'),to_date('" + dpkENDRQ.Text + "','yyyy-mm-dd'),'" + UserAction.UserID + "',sysdate,sysdate,'" + hfdLISID.Text + "','Y','" + docDOCID.Text + "','" + docMEMO.Text + "')", null));
                        liscmd.Add(new CommandInfo(@"insert into doc_license_log(SEQNO,LICENSEID,LICENSENAME,FLAG,LICTYPE,GDNAME,GDSEQ,PICNUM,OPERTIME,MEMO,ISCUR,DOCID)VALUES('" + seqno + "','" + hfdLISID.Text + "','" + docLISNAME.Text + "','N','GOODS_LIC','" + docGDNAME.Text + "','" + hfdGDSEQ.Text + "','" + arrays.Length + "',SYSDATE,'" + docMEMO.Text + "','Y','" + docDOCID.Text + "')", null));
                        for (int i = 1; i < arrays.Length + 1; i++)
                        {
                            liscmd.Add(new CommandInfo(@"insert into doc_license_img(SEQNO,ROWNO,IMGPATH,UPTTIME,LICENSEID,GDSEQ,ISCUR)values('" + seqno + "','" + i + "','" + arrays[i - 1] + "',sysdate,'" + hfdLISID.Text + "','" + hfdGDSEQ.Text + "','Y')", null));
                        }
                        if (DbHelperOra.ExecuteSqlTran(liscmd))
                        {
                            Alert.Show("证照上传成功！");
                            btnSubmit.Enabled = true;

                            string gdseq = hfdGDSEQ.Text;
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
                                                                FROM (SELECT T.SEQNO,T.SUPID,T.SUPNAME,T.LICENSEID, T.LICENSENAME, T.GDSEQ, T.GDNAME,T.OPERTIME,T.FLAG,T.PICNUM
                                                                        FROM DOC_LICENSE_LOG T
                                                                        WHERE GDSEQ = '" + gdseq + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC' ").Tables[0];
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int[] selections = GridLicense.SelectedRowIndexArray;
            if (GridLicense.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要提交的商品证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            //查询证照类别表，绑定是否必须
            DataTable typLicDT = DbHelperOra.Query("SELECT CODE,NAME,ISNEED FROM DOC_LICENSE WHERE OBJUSER='GOODS_LIC'").Tables[0];
            for(int i=0;i<typLicDT.Rows.Count;i++)
            {
                DataTable goodsLicDT = DbHelperOra.Query("SELECT LICENSEID FROM DOC_LICENSE_GOODS WHERE GDSEQ='" + hfdGDSEQ.Text + "' AND LICENSEID='" + typLicDT.Rows[i]["CODE"].ToString() + "' AND ISCUR='Y'").Tables[0];
                if(typLicDT.Rows[i][2].ToString().Equals("Y") && goodsLicDT.Rows.Count<=0)
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

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_GOODS WHERE GDSEQ='" + hfdGDSEQ.Text + "'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where GDSEQ='" + hfdGDSEQ.Text + "'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo("update doc_license_log set flag='S' where gdseq='" + hfdGDSEQ.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
                        liscmd.Add(new CommandInfo("update doc_license_goods set flag='S' where gdseq='" + hfdGDSEQ.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
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
                    string supcode = hfdGDSEQ.Text;
                    //保存成功之后重新绑定证照基本信息
                    DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    B.ISNEED,
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
                                                        WHERE GDSEQ = '" + hfdGDSEQ.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];

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

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            //string mystr = hfdValue.Text;
            //JArray ja = JsonConvert.DeserializeObject<JArray>(mystr);
            //bindProduct(ja);
        }

        protected void ischk_CheckedChanged(object sender, CheckedEventArgs e)
        {
            dpkENDRQ.Text = "2099-01-01";
            dpkENDRQ.Enabled = false;
        }
        protected void GridLIS_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            TabStrip1.ActiveTabIndex = 1;
            string gdname = GridLIS.Rows[e.RowIndex].DataKeys[2].ToString();
            string code = GridLIS.Rows[e.RowIndex].DataKeys[1].ToString();
            docGDNAME.Text = gdname;
            hfdGDSEQ.Text = code;

            if (Request.QueryString["oper"].ToString() == "gl")
            {
                changea.Hidden = true;
            }
            else if (Request.QueryString["oper"].ToString() == "sh")
            {
                changea.Hidden = true;
            }
            else if (Request.QueryString["oper"].ToString() == "change")
            {
                changea.Hidden = false;
            }
            else
            {
                changea.Hidden = true;
            }

            string myseq = "";
            if (DbHelperOra.Exists("select 1 from doc_license_log where gdseq='" + code + "'"))
            {
                DataTable mydt = DbHelperOra.Query("select seqno,licenseid from doc_license_log where gdseq='" + code + "'").Tables[0];
                string seqno = mydt.Rows[0]["seqno"].ToString();
                myseq = seqno + "|" + code + "|" + hfdLISID.Text + "|" + code;
            }
            else
            {
                //这个地方需要修改一下seqno，需要从数据库中查询一下
                string seqnostr = DbHelperOra.GetSingle("SELECT SEQ_LICENSE_PIC.NEXTVAL FROM DUAL").ToString();
                string seqno = "LIS" + hfdGDSEQ.Text + seqnostr;
                hfdSEQNO.Text = seqno;
                myseq = hfdSEQNO.Text + "|" + hfdGDSEQ.Text + "|" + hfdLISID.Text + "|" + code;
            }

            PageContext.RegisterStartupScript("initUpload('" + myseq + "');");

            DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    B.ISNEED,
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
                                                        WHERE GDSEQ = '" + code + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];

            docLISNAME.Text = lisDT.Rows[0]["LICENSENAME"].ToString();
            GridLicense.DataSource = lisDT;
            GridLicense.DataBind();

            if (DbHelperOra.Exists("select 1 from doc_license_log where gdseq='" + code + "'"))
            {
                DataTable mydt = DbHelperOra.Query("select seqno,licenseid,memo from doc_license_log where gdseq='" + code + "'").Tables[0];
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
            }

            DataTable cdt = DbHelperOra.Query("select flag from doc_license_log where gdseq='" + code + "'").Tables[0];
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
            else if (Request.QueryString["oper"].ToString() == "change")
            {
                DataSearch2();
            }
            else
            {
                DataSearch3();
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            //            DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_GOODS WHERE GDSEQ='" + hfdGDSEQ.Text + "'").Tables[0];
            //            if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
            //            {
            //                List<CommandInfo> liscmd = new List<CommandInfo>();
            //                liscmd.Add(new CommandInfo("update doc_license_log set flag='R' where gdseq='" + hfdGDSEQ.Text + "'", null));
            //                liscmd.Add(new CommandInfo("update doc_license_goods set flag='R' where gdseq='" + hfdGDSEQ.Text + "'", null));
            //                if (DbHelperOra.ExecuteSqlTran(liscmd))
            //                {
            //                    Alert.Show("驳回成功！");
            //                    btnSubmit.Enabled = false;
            //                }
            //                else
            //                {
            //                    Alert.Show("驳回失败！");
            //                }


            //                string supcode = hfdGDSEQ.Text;
            //                //保存成功之后重新绑定证照基本信息
            //            DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
            //                                                    B.NAME,
            //                                                    T.SEQNO,
            //                                                    T.LICENSEID,
            //                                                    T.LICENSENAME,
            //                                                    T.GDNAME,
            //                                                    T.GDSEQ,
            //                                                    T.OPERTIME,
            //                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
            //                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
            //                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.GDSEQ, T.GDNAME,T.OPERTIME,T.FLAG,T.PICNUM
            //                                                        FROM DOC_LICENSE_LOG T
            //                                                        WHERE GDSEQ = '" + hfdGDSEQ.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];

            //            GridLicense.DataSource = lisDT;
            //            GridLicense.DataBind();
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
                Alert.Show("请选择要提交的商品证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<CommandInfo> liscmd = new List<CommandInfo>();
            string succeed = string.Empty;
            bool ishave = false;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLicense.SelectedRowIndexArray[i];

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_GOODS WHERE GDSEQ='" + hfdGDSEQ.Text + "'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where GDSEQ='" + hfdGDSEQ.Text + "'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo("update doc_license_log set flag='R' where gdseq='" + hfdGDSEQ.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
                        liscmd.Add(new CommandInfo("update doc_license_goods set flag='R' where gdseq='" + hfdGDSEQ.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
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
                    Alert.Show("证照信息被驳回！");
                    //btnSubmit.Enabled = false;
                    string supcode = hfdGDSEQ.Text;
                    //保存成功之后重新绑定证照基本信息
                    DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    B.ISNEED,
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
                                                        WHERE GDSEQ = '" + hfdGDSEQ.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];

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

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            //            DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_GOODS WHERE GDSEQ='" + hfdGDSEQ.Text + "'").Tables[0];
            //            if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
            //            {
            //                List<CommandInfo> liscmd = new List<CommandInfo>();
            //                liscmd.Add(new CommandInfo("update doc_license_log set flag='Y' where gdseq='" + hfdGDSEQ.Text + "'", null));
            //                liscmd.Add(new CommandInfo("update doc_license_goods set flag='Y' where gdseq='" + hfdGDSEQ.Text + "'", null));
            //                if (DbHelperOra.ExecuteSqlTran(liscmd))
            //                {
            //                    Alert.Show("审核成功！");
            //                    btnSubmit.Enabled = false;
            //                }
            //                else
            //                {
            //                    Alert.Show("审核失败！");
            //                }


            //                string supcode = hfdGDSEQ.Text;
            //                //保存成功之后重新绑定证照基本信息
            //            DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
            //                                                    B.NAME,
            //                                                    T.SEQNO,
            //                                                    T.LICENSEID,
            //                                                    T.LICENSENAME,
            //                                                    T.GDNAME,
            //                                                    T.GDSEQ,
            //                                                    T.OPERTIME,
            //                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
            //                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
            //                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.GDSEQ, T.GDNAME,T.OPERTIME,T.FLAG,T.PICNUM
            //                                                        FROM DOC_LICENSE_LOG T
            //                                                        WHERE GDSEQ = '" + hfdGDSEQ.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];

            //            GridLicense.DataSource = lisDT;
            //            GridLicense.DataBind();
            //                DataSearch();
            //                btnSave.Enabled = false;
            //                btnSubmit.Enabled = false;
            //                btnRollBack.Enabled = false;
            //                btnAudit.Enabled = false;
            //                btnReject.Enabled = false;
            //            }
            //            else
            //            {
            //                Alert.Show("没有待审核的商品证照信息！");
            //            }



            int[] selections = GridLicense.SelectedRowIndexArray;
            if (GridLicense.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要提交的商品证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<CommandInfo> liscmd = new List<CommandInfo>();
            string succeed = string.Empty;
            bool ishave = false;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLicense.SelectedRowIndexArray[i];

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_GOODS WHERE GDSEQ='" + hfdGDSEQ.Text + "'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where GDSEQ='" + hfdGDSEQ.Text + "'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo("update doc_license_log set flag='Y' where gdseq='" + hfdGDSEQ.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
                        liscmd.Add(new CommandInfo("update doc_license_goods set flag='Y' where gdseq='" + hfdGDSEQ.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
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
                    string supcode = hfdGDSEQ.Text;
                    //保存成功之后重新绑定证照基本信息
                    DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    B.ISNEED,
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
                                                        WHERE GDSEQ = '" + hfdGDSEQ.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];

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

                    string picnum = DbHelperOra.GetSingle("select count(1) from DOC_LICENSE_IMG where seqno='" + HSEQNO + "' and licenseid='" + LISID + "'").ToString();
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
                string gdseq = hfdGDSEQ.Text;
                DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    B.ISNEED,
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
                                                        WHERE GDSEQ = '" + gdseq + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];


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
                PageContext.RegisterStartupScript("clearUpload();");


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

            DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1)mycount FROM DOC_LICENSE_LOG WHERE GDSEQ='" + hfdGDSEQ.Text + "' AND LICENSEID='" + LISID + "'").Tables[0];
            if(Convert.ToInt32(gdDT.Rows[0][0].ToString())>0)
                {
                    HSEQNO = GridLicense.Rows[e.RowIndex].DataKeys[0].ToString();
                    hfdHSEQNO.Text = HSEQNO;
                    //hfdGDSEQ.Text = GridLicense.Rows[e.RowIndex].DataKeys[1].ToString();
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
                
            else
            {

                DataTable dt = DbHelperOra.Query("select flag from doc_license_log where gdseq = '" + hfdGDSEQ.Text + "' and rownum=1 AND ISCUR='Y' AND lictype='GOODS_LIC'").Tables[0];
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString().Equals("Y") || dt.Rows[0][0].ToString().Equals("S"))
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
                else
                {
                    dpkBEGRQ.Text = "";
                    dpkENDRQ.Text = @"";
                    dpkENDRQ.Enabled = true;
                    ischk.Checked = false;
                    docDOCID.Text = "";
                    docMEMO.Text = "";
                }

                PageContext.RegisterStartupScript("clearUpload();");

                string myseq = "";
                if (DbHelperOra.Exists("select 1 from doc_license_log where gdseq='" + hfdGDSEQ.Text + "'"))
                {
                    DataTable mydt = DbHelperOra.Query("select seqno,licenseid from doc_license_log where gdseq='" + hfdGDSEQ.Text + "'").Tables[0];
                    string seqno = mydt.Rows[0]["seqno"].ToString();
                    myseq = seqno + "|" + hfdGDSEQ.Text + "|" + LISID + "|" + hfdGDSEQ.Text;
                }
                else
                {
                    string seqnostr = DbHelperOra.GetSingle("SELECT SEQ_LICENSE_PIC.NEXTVAL FROM DUAL").ToString();
                    string seqno = "LIS" + hfdGDSEQ.Text + seqnostr;
                    myseq = seqno + "|" + hfdGDSEQ.Text + "|" + LISID + "|" + hfdGDSEQ.Text;
                    hfdSEQNO.Text = seqno;
                }



                PageContext.RegisterStartupScript("initUpload1('" + myseq + "');");

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
                                                    T.GDNAME,
                                                    T.GDSEQ,
                                                    T.OPERTIME,
                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.GDSEQ, T.GDNAME,T.OPERTIME,T.FLAG,T.PICNUM
                                                        FROM DOC_LICENSE_LOG T
                                                        WHERE GDSEQ = '" + code + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];
            GridCertype.DataSource = lisDT;
            GridCertype.DataBind();
        }

        protected void btnRollBack_Click(object sender, EventArgs e)
        {
            //            DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_GOODS WHERE GDSEQ='" + hfdGDSEQ.Text + "'").Tables[0];
            //            if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
            //            {
            //                List<CommandInfo> liscmd = new List<CommandInfo>();
            //                liscmd.Add(new CommandInfo("update doc_license_log set flag='N' where gdseq='" + hfdGDSEQ.Text + "'", null));
            //                liscmd.Add(new CommandInfo("update doc_license_goods set flag='N' where gdseq='" + hfdGDSEQ.Text + "'", null));
            //                if (DbHelperOra.ExecuteSqlTran(liscmd))
            //                {
            //                    Alert.Show("【'" + docGDNAME.Text+ "'】证照信息已成功撤回！");
            //                }
            //                else
            //                {
            //                    Alert.Show("撤回失败！");
            //                }


            //                string supcode = hfdGDSEQ.Text;
            //                //保存成功之后重新绑定证照基本信息
            //                DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
            //                                                    B.NAME,
            //                                                    T.SEQNO,
            //                                                    T.LICENSEID,
            //                                                    T.LICENSENAME,
            //                                                    T.GDNAME,
            //                                                    T.GDSEQ,
            //                                                    T.OPERTIME,
            //                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
            //                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
            //                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.GDSEQ, T.GDNAME,T.OPERTIME,T.FLAG,T.PICNUM
            //                                                        FROM DOC_LICENSE_LOG T
            //                                                        WHERE GDSEQ = '" + hfdGDSEQ.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];

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
            //                Alert.Show("没有待撤回的商品证照信息！");
            //            }




            int[] selections = GridLicense.SelectedRowIndexArray;
            if (GridLicense.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要提交的商品证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<CommandInfo> liscmd = new List<CommandInfo>();
            string succeed = string.Empty;
            bool ishave = false;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLicense.SelectedRowIndexArray[i];

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_GOODS WHERE GDSEQ='" + hfdGDSEQ.Text + "'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where GDSEQ='" + hfdGDSEQ.Text + "'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo("update doc_license_log set flag='N' where gdseq='" + hfdGDSEQ.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
                        liscmd.Add(new CommandInfo("update doc_license_goods set flag='N' where gdseq='" + hfdGDSEQ.Text + "' AND LICENSEID='" + GridLicense.DataKeys[rowIndex][3].ToString() + "' AND ISCUR='Y'", null));
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
                    Alert.Show("【'" + docGDNAME.Text + "'】证照信息已成功撤回！");
                    //btnSubmit.Enabled = false;
                    string supcode = hfdGDSEQ.Text;
                    //保存成功之后重新绑定证照基本信息
                    DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
                                                    B.NAME,
                                                    B.ISNEED,
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
                                                        WHERE GDSEQ = '" + hfdGDSEQ.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];

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

        protected void mybtnAudit_Click(object sender, EventArgs e)
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
                string flag = DbHelperOra.GetSingle("SELECT FLAG FROM DOC_LICENSE_LOG WHERE GDSEQ='" + GridLIS.DataKeys[rowIndex][1].ToString() + "' AND ROWNUM=1").ToString();
                if (flag.Equals("Y"))
                {
                    Alert.Show("已审核，无需重复审核!");
                    return;
                }

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_GOODS WHERE GDSEQ='" + GridLIS.DataKeys[rowIndex][1].ToString() + "'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where gdseq='" + GridLIS.DataKeys[rowIndex][1].ToString() + "'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo("update doc_license_log set flag='Y' where gdseq='" + GridLIS.DataKeys[rowIndex][1].ToString() + "'", null));
                        liscmd.Add(new CommandInfo("update doc_license_goods set flag='Y' where gdseq='" + GridLIS.DataKeys[rowIndex][1].ToString() + "'", null));
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
                        docGDNAME.Text = "";
                        dpkBEGRQ.Text = "";
                        dpkENDRQ.Text = "";
                        PageContext.RegisterStartupScript("clearUpload();");
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
                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM DOC_LICENSE_GOODS WHERE GDSEQ='" + GridLIS.DataKeys[rowIndex][1].ToString() + "'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where gdseq='" + GridLIS.DataKeys[rowIndex][1].ToString() + "'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo("update doc_license_log set flag='R' where gdseq='" + GridLIS.DataKeys[rowIndex][1].ToString() + "'", null));
                        liscmd.Add(new CommandInfo("update doc_license_goods set flag='R' where gdseq='" + GridLIS.DataKeys[rowIndex][1].ToString() + "'", null));
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
                    Alert.Show("所选商品证照信息已被驳回！", "消息提示", MessageBoxIcon.Warning);
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
                        docGDNAME.Text = "";
                        dpkBEGRQ.Text = "";
                        dpkENDRQ.Text = "";
                        PageContext.RegisterStartupScript("clearUpload();");
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
                                                    B.NAME,B.ISNEED,
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
                                                        WHERE GDSEQ = '" + dt.Rows[0][0].ToString() + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'GOODS_LIC'").Tables[0];
                GridCertype.DataSource = lisDT;
                GridCertype.DataBind();
            }

        }

        protected void btnHistory_Click(object sender, EventArgs e)
        {
            HistoryDataSearch();
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
    }
}
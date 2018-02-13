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

//         作者：曹国稳（讯通信息科技有限公司）

namespace ERPProject.CertificateInput
{
    public partial class SupCustRelative : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                Init();
                //屏蔽不需要的操作按钮
                if (Request.QueryString["oper"] != null)
                {
                    if (Request.QueryString["oper"].ToString() == "gl")
                    {
                        ButtonHidden(btnAudit, btnReject,mybtnAudit,mybtnReject);
                        DataSearch();
                        lbfChCerts.Hidden = true;
                    }
                    else if (Request.QueryString["oper"].ToString() == "sh")
                    {
                        ButtonHidden(btnHistory, btnRollBack, btnSave, btnSubmit);
                        chkisLR.Hidden = true;
                        chknoLR.Hidden = true;
                        DataSearch1();
                        lbfChCerts.Hidden = true;
                    }
                    else if (Request.QueryString["oper"].ToString() == "change")
                    {
                        ButtonHidden(btnAudit, btnReject, mybtnAudit, mybtnReject);
                        chkisLR.Hidden = true;
                        chknoLR.Hidden = true;
                        DataSearch2();
                        lbfChCerts.Hidden = false;
                    }
                    else
                    {
                        ButtonHidden(btnHistory, btnRollBack, btnSave, btnSubmit);
                        chkisLR.Hidden = true;
                        chknoLR.Hidden = true;
                        DataSearch3();
                        lbfChCerts.Hidden = true;
                    }
                }
            }
        }

        private void Init()
        {
            //默认第一个tab页面
            TabStrip1.ActiveTabIndex = 0;
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
            string strbeg = @"SELECT SUPPLIER,SUPID,SUPNAME,PICNUM,HTPICNUM,FLAG,FLAG1 FROM(";
            string strSQLEND = @"SELECT '供应商' SUPPLIER,
                                  T.SUPID,
                                  T.SUPNAME,
                                  '【' || nvl((SELECT PICNUM FROM DOC_LICENSE_LOG WHERE SUPID=T.SUPID and lictype='REL_LIC' and picnum is not null and iscur='Y'), 0) || '】' PICNUM,
                                  '【' || nvl((SELECT HTPICNUM FROM DOC_LICENSE_LOG WHERE SUPID=T.SUPID and lictype='REL_LIC' and HTPICNUM is not null and iscur='Y'), 0) || '】' HTPICNUM,
                                      nvl(DECODE((SELECT FLAG FROM DOC_LICENSE_LOG WHERE LICENSEID='ht001' AND SUPID=T.SUPID and lictype='REL_LIC' and iscur='Y'),
                                              'N',
                                              '已保存',
                                              'S',
                                              '已提交',
                                              'Y',
                                              '已审核',
                                              'R',
                                              '已拒绝'),
                                      '待录入')FLAG1,
                                      nvl(DECODE((SELECT FLAG FROM DOC_LICENSE_LOG WHERE LICENSEID='cn001' AND SUPID=T.SUPID and iscur='Y'),
                                              'N',
                                              '已保存',
                                              'S',
                                              '已提交',
                                              'Y',
                                              '已审核',
                                              'R',
                                              '已拒绝'),
                                      '待录入')FLAG
                              FROM (SELECT DISTINCT SUPID,SUPNAME FROM DOC_LICENSE_LOG WHERE LICTYPE='REL_LIC')A, DOC_SUPPLIER T
                              WHERE T.SUPID = A.SUPID(+) 
                              AND (T.ISSUPPLIER = 'Y' OR T.ISPSS='Y')";
            string strend = @")";
            string strSQL = @"";
            if (!string.IsNullOrWhiteSpace(txtName.Text))
            {
                strSQLEND = strSQLEND + "  and (a.supid like '%" + txtName.Text + "%' or a.supname like  '%" + txtName.Text + "%')";
            }
            if (!string.IsNullOrWhiteSpace(ddlFLAG.SelectedValue))
            {
                strSQLEND = strSQLEND + " and A.flag in (select flag from doc_license_log where supid=T.supid and flag='" + ddlFLAG.SelectedValue + "')";
            }
            if (chkisLR.Checked)
            {
                //已上传表示合同书和承诺书都完全上传了的
                strend = strend + "where picnum <> '【0】' or htpicnum <>  '【0】'";
            }
            if (chknoLR.Checked)
            {
                //未上传指的是合同书和承诺书都没有上传或则只是部分上传
                strend = strend + @"where (picnum = '【0】' and htpicnum =  '【0】') or (picnum = '【0】' and htpicnum <>  '【0】')
                                   or (picnum <> '【0】' and htpicnum =  '【0】')";
            }

            strSQL = strbeg + strSQLEND + strend;

            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridLIS.PageIndex, GridLIS.PageSize, strSQL, ref total);
            GridLIS.RecordCount = total;
            GridLIS.DataSource = dt;
            GridLIS.DataBind();

            //默认情况下，单击供应商的时候上面显示图片上传情况，下面证照图片展示
            if (dt.Rows.Count > 0)
            {
                DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
         CASE
           when B.CODE = 'cn001' then
            B.NAME ||
            decode(T.PICNUM, '', '【' || 0 || '】', '【' || T.PICNUM || '】') || '张'
           when B.CODE = 'ht001' then
           B.NAME || decode(T.HTPICNUM, '', '【' || 0 || '】', '【' || T.HTPICNUM || '】') || '张'
         END CASE,
         T.SEQNO,
         T.LICENSEID,
         T.LICENSENAME,
         T.SUPNAME,
         T.SUPID,
         T.OPERTIME,
         nvl(decode(T.FLAG,
                    'N',
                    '已保存',
                    'S',
                    '已提交',
                    'Y',
                    '已审核',
                    'R',
                    '已驳回'),
             '待录入') FLAG
    FROM (SELECT T.SEQNO,
                 T.SUPID,
                 T.SUPNAME,
                 T.LICENSEID,
                 T.LICENSENAME,
                 T.OPERTIME,
                 T.FLAG,
                 T.PICNUM,
                 T.HTPICNUM
            FROM DOC_LICENSE_LOG T
           WHERE SUPID = '" + dt.Rows[0]["SUPID"].ToString() + "'AND T.ISCUR = 'Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'REL_LIC'").Tables[0];
                GridCertype.DataSource = lisDT;
                GridCertype.DataBind();
            }
        }

        private void initSear(string mystrsql)
        {
            string strbeg = @"SELECT SUPPLIER,SUPID,SUPNAME,PICNUM,HTPICNUM,FLAG,FLAG1 FROM(";
            string strSQLEND = @"SELECT '供应商' SUPPLIER,
                                  T.SUPID,
                                  T.SUPNAME,
                                  '【' || nvl((SELECT PICNUM FROM DOC_LICENSE_LOG WHERE SUPID=T.SUPID and lictype='REL_LIC' and picnum is not null and iscur='Y'), 0) || '】' PICNUM,
                                  '【' || nvl((SELECT HTPICNUM FROM DOC_LICENSE_LOG WHERE SUPID=T.SUPID and lictype='REL_LIC' and HTPICNUM is not null and iscur='Y'), 0) || '】' HTPICNUM,
                                      nvl(DECODE((SELECT FLAG FROM DOC_LICENSE_LOG WHERE LICENSEID='ht001' AND SUPID=T.SUPID and lictype='REL_LIC' and iscur='Y'),
                                              'N',
                                              '已保存',
                                              'S',
                                              '已提交',
                                              'Y',
                                              '已审核',
                                              'R',
                                              '已拒绝'),
                                      '待录入')FLAG1,
                                      nvl(DECODE((SELECT FLAG FROM DOC_LICENSE_LOG WHERE LICENSEID='cn001' AND SUPID=T.SUPID and iscur='Y'),
                                              'N',
                                              '已保存',
                                              'S',
                                              '已提交',
                                              'Y',
                                              '已审核',
                                              'R',
                                              '已拒绝'),
                                      '待录入')FLAG
                              FROM (SELECT DISTINCT SUPID,SUPNAME,flag FROM DOC_LICENSE_LOG WHERE LICTYPE='REL_LIC')A, DOC_SUPPLIER T
                              WHERE T.SUPID = A.SUPID(+) 
                              AND T.ISSUPPLIER = 'Y'";
            string strend = @")";
            string strSQL = @"";
            if (!string.IsNullOrWhiteSpace(txtName.Text))
            {
                strSQLEND = strSQLEND + "  and (a.supid like '%" + txtName.Text + "%' or a.supname like  '%" + txtName.Text + "%')";
            }

            strSQLEND = strSQLEND + mystrsql;

            strend = strend + "where picnum <> '【0】' or htpicnum <>  '【0】'";

            strSQL = strbeg + strSQLEND + strend;

            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridLIS.PageIndex, GridLIS.PageSize, strSQL, ref total);
            GridLIS.RecordCount = total;
            GridLIS.DataSource = dt;
            GridLIS.DataBind();

            //默认情况下，单击供应商的时候上面显示图片上传情况，下面证照图片展示
            if (dt.Rows.Count > 0)
            {
                DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
         CASE
           when B.CODE = 'cn001' then
            B.NAME ||
            decode(T.PICNUM, '', '【' || 0 || '】', '【' || T.PICNUM || '】') || '张'
           when B.CODE = 'ht001' then
           B.NAME || decode(T.HTPICNUM, '', '【' || 0 || '】', '【' || T.HTPICNUM || '】') || '张'
         END CASE,
         T.SEQNO,
         T.LICENSEID,
         T.LICENSENAME,
         T.SUPNAME,
         T.SUPID,
         T.OPERTIME,
         nvl(decode(T.FLAG,
                    'N',
                    '已保存',
                    'S',
                    '已提交',
                    'Y',
                    '已审核',
                    'R',
                    '已驳回'),
             '待录入') FLAG
    FROM (SELECT T.SEQNO,
                 T.SUPID,
                 T.SUPNAME,
                 T.LICENSEID,
                 T.LICENSENAME,
                 T.OPERTIME,
                 T.FLAG,
                 T.PICNUM,
                 T.HTPICNUM
            FROM DOC_LICENSE_LOG T
           WHERE SUPID = '" + dt.Rows[0]["SUPID"].ToString() + "'AND T.ISCUR = 'Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'REL_LIC'").Tables[0];
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


        private void DataSearch1()
        {
            string mysql = " and A.flag in (select flag from doc_license_log where supid=T.supid and LICTYPE='REL_LIC' and flag='S' AND ISCUR='Y' and ischange='N')";
            initSear(mysql);
        }

        private void DataSearch2()
        {
            string mysql = " and A.flag in (select flag from doc_license_log where supid=T.supid and LICTYPE='REL_LIC' and flag<>'S' AND ISCUR='Y')";
            initSear(mysql);
        }

        private void DataSearch3()
        {
            string mysql = " and A.flag in (select flag from doc_license_log where supid=T.supid and LICTYPE='REL_LIC' and flag='S' AND ISCUR='Y' and ischange='Y')";
            initSear(mysql);
        }

        protected void GridLIS_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
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
                        delCmd.Add(new CommandInfo("delete from doc_license_relationsup where seqno='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        delCmd.Add(new CommandInfo("delete from doc_license_log where seqno='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        delCmd.Add(new CommandInfo("delete from doc_license_img where seqno='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        DbHelperOra.ExecuteSqlTran(delCmd);
                    }
                    else
                    {
                        List<CommandInfo> liscommand = new List<CommandInfo>();
                        liscommand.Add(new CommandInfo("update doc_license_relationsup set iscur='N' where seqno='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        liscommand.Add(new CommandInfo("update doc_license_log set iscur='N' where SEQNO='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        liscommand.Add(new CommandInfo("UPDATE DOC_LICENSE_IMG SET ISCUR='N' where SEQNO='" + seqno + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                        DbHelperOra.ExecuteSqlTran(liscommand);
                    }

                    //换证之后，更新新录入的证照流水，其他证照流水保持不变。
                    string seqnostr = DbHelperOra.GetSingle("SELECT SEQ_LICENSE_PIC.NEXTVAL FROM DUAL").ToString();
                    string myseqno = "LIS" + hfdSUPID.Text + seqnostr;

                    List<CommandInfo> liscmd = new List<CommandInfo>();
                    string strlic = "";
                    if (docLISNAME.Text.Equals("承诺书"))
                    {
                        strlic = "cn001";
                        liscmd.Add(new CommandInfo(@"insert into doc_license_log(SEQNO,LICENSEID,LICENSENAME,FLAG,LICTYPE,SUPNAME,SUPID,PICNUM,OPERTIME,MEMO,ISCUR,ISCHANGE)VALUES('" + myseqno + "','" + hfdLISID.Text + "','" + docLISNAME.Text + "','N','REL_LIC','" + docSUPNAME.Text + "','" + hfdSUPID.Text + "','" + arrays.Length + "',SYSDATE,'" + docMEMO.Text + "','Y','Y')", null));
                    }
                    else
                    {
                        strlic = "ht001";
                        liscmd.Add(new CommandInfo(@"insert into doc_license_log(SEQNO,LICENSEID,LICENSENAME,FLAG,LICTYPE,SUPNAME,SUPID,HTPICNUM,OPERTIME,MEMO,ISCUR,ISCHANGE)VALUES('" + myseqno + "','" + hfdLISID.Text + "','" + docLISNAME.Text + "','N','REL_LIC','" + docSUPNAME.Text + "','" + hfdSUPID.Text + "','" + arrays.Length + "',SYSDATE,'" + docMEMO.Text + "','Y','Y')", null));
                    }

                    liscmd.Add(new CommandInfo(@"INSERT INTO DOC_LICENSE_RELATIONSUP(SEQNO,SUPID,SUPNAME,FLAG,LICENSETYPE,LICENSENAME,LICENSEID,BEGRQ,ENDRQ,ISCUR,LRY,LRRQ,MEMO,DOCID,ISCHANGE)
                                                VALUES('" + myseqno + "','" + hfdSUPID.Text + "','" + docSUPNAME.Text + "','N','REL_LIC','" + docLISNAME.Text + "','" + strlic + "',to_date('" + dpkBEGRQ.Text + "','yyyy-mm-dd'),to_date('" + dpkENDRQ.Text + "','yyyy-mm-dd'),'Y','" + UserAction.UserID + "',SYSDATE,'" + docMEMO.Text + "','"+docDOCID.Text+"','Y')", null));

                    for (int i = 1; i < arrays.Length + 1; i++)
                    {
                        liscmd.Add(new CommandInfo(@"insert into doc_license_img(SEQNO,ROWNO,IMGPATH,UPTTIME,LICENSEID,ISCUR,STR1)values('" + myseqno + "','" + i + "','" + arrays[i - 1] + "',sysdate,'" + hfdLISID.Text + "','Y','" + hfdSUPID.Text + "')", null));
                    }
                    DataTable changeDT = DbHelperOra.Query("SELECT COUNT(1) FROM DOC_LICENSE_LOG WHERE SUPID='" + hfdSUPID.Text + "' AND ISCUR='Y' AND LICENSEID<>'" + hfdLISID.Text + "'").Tables[0];
                    if (changeDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo(@"update doc_license_relationsup set seqno='" + myseqno + "' WHERE SUPID='" + hfdSUPID.Text + "' and iscur='Y'", null));
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
         CASE
           when B.CODE = 'cn001' then
            B.NAME ||
            decode(T.PICNUM, '', '【' || 0 || '】', '【' || T.PICNUM || '】') || '张'
           when B.CODE = 'ht001' then
          B.NAME || decode(T.HTPICNUM, '', '【' || 0 || '】', '【' || T.HTPICNUM || '】') || '张'
         END CASE,
         T.SEQNO,
         T.LICENSEID,
         T.LICENSENAME,
         T.SUPNAME,
         T.SUPID,
         T.OPERTIME,
         nvl(decode(T.FLAG,
                    'N',
                    '已保存',
                    'S',
                    '已提交',
                    'Y',
                    '已审核',
                    'R',
                    '已驳回'),
             '待录入') FLAG
    FROM (SELECT T.SEQNO,
                 T.SUPID,
                 T.SUPNAME,
                 T.LICENSEID,
                 T.LICENSENAME,
                 T.OPERTIME,
                 T.FLAG,
                 T.PICNUM,
                 T.HTPICNUM
            FROM DOC_LICENSE_LOG T
           WHERE SUPID = '" + supid + "'AND T.ISCUR = 'Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'REL_LIC'").Tables[0];

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
                    DataTable goodsDT = DbHelperOra.Query("SELECT COUNT(1)ccount FROM DOC_LICENSE_RELATIONSUP WHERE SUPID='" + hfdSUPID.Text + "' AND LICENSEID='" + hfdLISID.Text + "' and iscur='Y'").Tables[0];
                     if (Convert.ToInt32(goodsDT.Rows[0][0].ToString()) > 0)
                     {
                         List<CommandInfo> delpicmd = new List<CommandInfo>();
                         //执行update操作
                         if (picurl.Length <= 0)
                         {
                             delpicmd.Add(new CommandInfo("update DOC_LICENSE_RELATIONSUP set docid='" + docDOCID.Text + "',memo='" + docMEMO.Text + "',begrq=to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),endrq=to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd') where supid='" + hfdSUPID.Text + "' and licenseid='" + hfdLISID.Text + "'", null));
                         }
                         else
                         {
                             picurl = picurl.Substring(0, picurl.Length - 1);
                             string[] arrays = picurl.Split(',');
                             //删除图片，然后换上新图片
                             DbHelperOra.ExecuteSql("delete from doc_license_img where supid='" + hfdSUPID.Text + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'");
                             delpicmd.Add(new CommandInfo("update DOC_LICENSE_RELATIONSUP set docid='" + docDOCID.Text + "',memo='" + docMEMO.Text + "',begrq=to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),endrq=to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd') where supid='" + hfdSUPID.Text + "' and licenseid='" + hfdLISID.Text + "'", null));
                             if (hfdLISID.Text.Equals("cn001"))
                             {
                                 delpicmd.Add(new CommandInfo("update doc_license_log set picnum='" + arrays.Length + "' where supid='" + hfdSUPID.Text + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                             }
                             else
                             {
                                 delpicmd.Add(new CommandInfo("update doc_license_log set htpicnum='" + arrays.Length + "' where supid='" + hfdSUPID.Text + "' and licenseid='" + hfdLISID.Text + "' and iscur='Y'", null));
                             }
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
         CASE
           when B.CODE = 'cn001' then
            B.NAME ||
            decode(T.PICNUM, '', '【' || 0 || '】', '【' || T.PICNUM || '】') || '张'
           when B.CODE = 'ht001' then
          B.NAME || decode(T.HTPICNUM, '', '【' || 0 || '】', '【' || T.HTPICNUM || '】') || '张'
         END CASE,
         T.SEQNO,
         T.LICENSEID,
         T.LICENSENAME,
         T.SUPNAME,
         T.SUPID,
         T.OPERTIME,
         nvl(decode(T.FLAG,
                    'N',
                    '已保存',
                    'S',
                    '已提交',
                    'Y',
                    '已审核',
                    'R',
                    '已驳回'),
             '待录入') FLAG
    FROM (SELECT T.SEQNO,
                 T.SUPID,
                 T.SUPNAME,
                 T.LICENSEID,
                 T.LICENSENAME,
                 T.OPERTIME,
                 T.FLAG,
                 T.PICNUM,
                 T.HTPICNUM
            FROM DOC_LICENSE_LOG T
           WHERE SUPID = '" + supid + "'AND T.ISCUR = 'Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'REL_LIC'").Tables[0];

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
                         if (picurl.Length <= 0)
                         {
                             Alert.Show("请上传证照图片！");
                             return;
                         }
                         picurl = picurl.Substring(0, picurl.Length - 1);
                         string[] arrays = picurl.Split(',');

                         string licname = @"";
                         string lisid = "";//证照编号
                         List<CommandInfo> liscmd = new List<CommandInfo>();

                         if (docLISNAME.Text.Equals("承诺书"))
                         {
                             lisid = "cn001";
                             licname = "承诺书";
                             liscmd.Add(new CommandInfo(@"insert into doc_license_log(SEQNO,LICENSEID,LICENSENAME,FLAG,LICTYPE,SUPNAME,SUPID,PICNUM,OPERTIME,MEMO,ISCUR,ISCHANGE)VALUES('" + seqno + "','" + hfdLISID.Text + "','承诺书','N','REL_LIC','" + docSUPNAME.Text + "','" + hfdSUPID.Text + "','" + arrays.Length + "',SYSDATE,'" + docMEMO.Text + "','Y','N')", null));
                         }
                         else
                         {
                             lisid = "ht001";
                             licname = @"合同";
                             liscmd.Add(new CommandInfo(@"insert into doc_license_log(SEQNO,LICENSEID,LICENSENAME,FLAG,LICTYPE,SUPNAME,SUPID,HTPICNUM,OPERTIME,MEMO,ISCUR,ISCHANGE)
                                               VALUES('" + seqno + "','" + hfdLISID.Text + "','合同书','N','REL_LIC','" + docSUPNAME.Text + "','" + hfdSUPID.Text + "','" + arrays.Length + "',SYSDATE,'" + docMEMO.Text + "','Y','N')", null));
                         }
                         liscmd.Add(new CommandInfo(@"INSERT INTO DOC_LICENSE_RELATIONSUP(SEQNO,SUPID,SUPNAME,FLAG,LICENSETYPE,LICENSENAME,LICENSEID,BEGRQ,ENDRQ,ISCUR,LRY,LRRQ,MEMO,DOCID,ISCHANGE)
                                                VALUES('" + seqno + "','" + hfdSUPID.Text + "','" + docSUPNAME.Text + "','N','REL_LIC','" + licname + "','" + lisid + "',to_date('" + dpkBEGRQ.Text + "','yyyy-mm-dd'),to_date('" + dpkENDRQ.Text + "','yyyy-mm-dd'),'Y','" + UserAction.UserID + "',SYSDATE,'" + docMEMO.Text + "','" + docDOCID.Text + "','N')", null));
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
         CASE
           when B.CODE = 'cn001' then
            B.NAME ||
            decode(T.PICNUM, '', '【' || 0 || '】', '【' || T.PICNUM || '】') || '张'
           when B.CODE = 'ht001' then
          B.NAME || decode(T.HTPICNUM, '', '【' || 0 || '】', '【' || T.HTPICNUM || '】') || '张'
         END CASE,
         T.SEQNO,
         T.LICENSEID,
         T.LICENSENAME,
         T.SUPNAME,
         T.SUPID,
         T.OPERTIME,
         nvl(decode(T.FLAG,
                    'N',
                    '已保存',
                    'S',
                    '已提交',
                    'Y',
                    '已审核',
                    'R',
                    '已驳回'),
             '待录入') FLAG
    FROM (SELECT T.SEQNO,
                 T.SUPID,
                 T.SUPNAME,
                 T.LICENSEID,
                 T.LICENSENAME,
                 T.OPERTIME,
                 T.FLAG,
                 T.PICNUM,
                 T.HTPICNUM
            FROM DOC_LICENSE_LOG T
           WHERE SUPID = '" + supid + "'AND T.ISCUR = 'Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'REL_LIC'").Tables[0];

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
            else if (Request.QueryString["oper"].ToString() == "change")
            {
                DataSearch2();
            }
            else
            {
                DataSearch3();
            }
        }

        protected void btnHistory_Click(object sender, EventArgs e)
        {

        }

        protected void GridLIS_RowClick(object sender, GridRowClickEventArgs e)
        {
            string code = GridLIS.Rows[e.RowIndex].DataKeys[1].ToString();
            DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
         CASE
           when B.CODE = 'cn001' then
            B.NAME ||
            decode(T.PICNUM, '', '【' || 0 || '】', '【' || T.PICNUM || '】') || '张'
           when B.CODE = 'ht001' then
           B.NAME || decode(T.HTPICNUM, '', '【' || 0 || '】', '【' || T.HTPICNUM || '】') || '张'
         END CASE,
         T.SEQNO,
         T.LICENSEID,
         T.LICENSENAME,
         T.SUPNAME,
         T.SUPID,
         T.OPERTIME,
         nvl(decode(T.FLAG,
                    'N',
                    '已保存',
                    'S',
                    '已提交',
                    'Y',
                    '已审核',
                    'R',
                    '已驳回'),
             '待录入') FLAG
    FROM (SELECT T.SEQNO,
                 T.SUPID,
                 T.SUPNAME,
                 T.LICENSEID,
                 T.LICENSENAME,
                 T.OPERTIME,
                 T.FLAG,
                 T.PICNUM,
                 T.HTPICNUM
            FROM DOC_LICENSE_LOG T
           WHERE SUPID = '" + code + "'AND T.ISCUR = 'Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'REL_LIC'").Tables[0];
           
            GridCertype.DataSource = lisDT;
            GridCertype.DataBind();
        }

        protected void GridLIS_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            TabStrip1.ActiveTabIndex = 1;
            string code = GridLIS.Rows[e.RowIndex].DataKeys[1].ToString();
            string supname = GridLIS.Rows[e.RowIndex].DataKeys[2].ToString();
            docSUPNAME.Text = supname;
            hfdSUPID.Text = code;

            string myseq = "";
            if (DbHelperOra.Exists("select 1 from doc_license_log where supid='" + code + "'"))
            {
                DataTable mydt = DbHelperOra.Query("select seqno,licenseid from doc_license_log where supid='" + code + "'").Tables[0];
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
            PageContext.RegisterStartupScript("initUpload('" + "aaa" + "');");//调用图片选择控件，其中的aaa是随意填写的，不为空。

            DataTable lisDT = DbHelperOra.Query(@"SELECT B.CODE,
         CASE
           when B.CODE = 'cn001' then
            B.NAME ||
            decode(T.PICNUM, '', '【' || 0 || '】', '【' || T.PICNUM || '】') || '张'
           when B.CODE = 'ht001' then
          B.NAME || decode(T.HTPICNUM, '', '【' || 0 || '】', '【' || T.HTPICNUM || '】') || '张'
         END CASE,
         T.SEQNO,
         T.LICENSEID,
         T.LICENSENAME,
         T.SUPNAME,
         T.SUPID,
         T.OPERTIME,
         nvl(decode(T.FLAG,
                    'N',
                    '已保存',
                    'S',
                    '已提交',
                    'Y',
                    '已审核',
                    'R',
                    '已驳回'),
             '待录入') FLAG
    FROM (SELECT T.SEQNO,
                 T.SUPID,
                 T.SUPNAME,
                 T.LICENSEID,
                 T.LICENSENAME,
                 T.OPERTIME,
                 T.FLAG,
                 T.PICNUM,
                 T.HTPICNUM
            FROM DOC_LICENSE_LOG T
           WHERE SUPID = '" + code + "'AND T.ISCUR = 'Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'REL_LIC'").Tables[0];
            
            docLISNAME.Text = lisDT.Rows[0]["LICENSENAME"].ToString();
            GridLicense.DataSource = lisDT;
            GridLicense.DataBind();


            if (DbHelperOra.Exists("select 1 from doc_license_log where supid='" + code + "'"))
            {
                DataTable mydt = DbHelperOra.Query("select seqno,licenseid,memo from doc_license_log where supid='" + code + "'").Tables[0];
                string seqno = mydt.Rows[0]["seqno"].ToString();
                DataTable licensedt = DbHelperOra.Query("select trunc(begrq) begrq,trunc(endrq) endrq,DOCID,MEMO from doc_license_relationsup where seqno='" + seqno + "'").Tables[0];
                if (licensedt.Rows[0][1].ToString().Equals("2099/1/1 0:00:00"))
                {
                    ischk.Checked = true;
                    dpkENDRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][1].ToString());
                    dpkBEGRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][0].ToString());
                    dpkENDRQ.Enabled = false;
                }
                else
                {
                    ischk.Checked = false;
                    dpkENDRQ.Enabled = true;
                    dpkENDRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][1].ToString());
                    dpkBEGRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][0].ToString());
                }
                docMEMO.Text = mydt.Rows[0]["memo"].ToString();
                docDOCID.Text = licensedt.Rows[0]["docid"].ToString();
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


            DataTable cdt = DbHelperOra.Query("select flag from doc_license_log where supid='" + code + "'").Tables[0];
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

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int[] selections = GridLicense.SelectedRowIndexArray;
            if (GridLicense.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要提交的客户关系证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            //查询证照类别表，绑定是否必须
            DataTable typLicDT = DbHelperOra.Query("SELECT CODE,NAME,ISNEED FROM DOC_LICENSE WHERE OBJUSER='REL_LIC'").Tables[0];
            for (int i = 0; i < typLicDT.Rows.Count; i++)
            {
                DataTable supplierLicDT = DbHelperOra.Query("SELECT LICENSEID FROM doc_license_relationsup WHERE SUPID='" + hfdSUPID.Text + "' AND LICENSEID='" + typLicDT.Rows[i]["CODE"].ToString() + "' AND ISCUR='Y'").Tables[0];
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

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM doc_license_relationsup WHERE SUPID='" + GridLicense.DataKeys[rowIndex][1].ToString() + "' AND ISCUR='Y'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where SUPID='" + GridLicense.DataKeys[rowIndex][1].ToString() + "' AND ISCUR='Y'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        if (GridLicense.DataKeys[rowIndex][8].ToString().Substring(0, 3).Equals("合同书"))
                        {
                            liscmd.Add(new CommandInfo("update doc_license_log set flag='S' where SEQNO='" + hfdHSEQNO.Text + "' and licenseid='ht001' and iscur='Y'", null));
                        }
                        else
                        {
                            liscmd.Add(new CommandInfo("update doc_license_log set flag='S' where SEQNO='" + hfdHSEQNO.Text + "' and licenseid='cn001' AND ISCUR='Y'", null));
                        }
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
         CASE when B.CODE = 'cn001' then
       B.NAME || decode(T.PICNUM, '', '【' || 0 || '】', '【' || T.PICNUM || '】') || '张' when B.CODE = 'ht001' then
       B.NAME || decode(T.HTPICNUM, '', '【' || 0 || '】', '【' || T.HTPICNUM || '】') || '张' END CASE,
                                                    B.NAME,
                                                    T.SEQNO,
                                                    T.LICENSEID,
                                                    T.LICENSENAME,
                                                    T.SUPNAME,
                                                    T.SUPID,
                                                    T.OPERTIME,
                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.SUPID, T.SUPNAME,T.OPERTIME,T.FLAG,T.PICNUM,T.HTPICNUM
                                                        FROM DOC_LICENSE_LOG T
                                                        WHERE SUPID = '" + hfdSUPID.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'REL_LIC'").Tables[0];

                    GridLicense.DataSource = lisDT;
                    GridLicense.DataBind();
                    DataSearch();
                    //btnSave.Enabled = false;
                    //btnSubmit.Enabled = false;
                    //btnRollBack.Enabled = true;
                    //btnAudit.Enabled = true;
                    //btnReject.Enabled = true;
                }
                else
                {
                    Alert.Show("操作失败！", MessageBoxIcon.Warning);
                }
            }
        }

        protected void btnRollBack_Click(object sender, EventArgs e)
        {

            int[] selections = GridLicense.SelectedRowIndexArray;
            if (GridLicense.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要审核的客户关系证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<CommandInfo> liscmd = new List<CommandInfo>();
            string succeed = string.Empty;
            bool ishave = false;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLicense.SelectedRowIndexArray[i];

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM doc_license_relationsup WHERE SUPID='" + GridLicense.DataKeys[rowIndex][1].ToString() + "'  AND ISCUR='Y'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where SUPID='" + GridLicense.DataKeys[rowIndex][1].ToString() + "' AND ISCUR='Y'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        if (GridLicense.DataKeys[rowIndex][8].ToString().Substring(0, 3).Equals("合同书"))
                        {
                            liscmd.Add(new CommandInfo("update doc_license_log set flag='N' where SEQNO='" + hfdHSEQNO.Text + "' and licenseid='ht001' AND ISCUR='Y'", null));
                        }
                        else
                        {
                            liscmd.Add(new CommandInfo("update doc_license_log set flag='N' where SEQNO='" + hfdHSEQNO.Text + "' and licenseid='cn001' AND ISCUR='Y'", null));
                        }
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
         CASE when B.CODE = 'cn001' then
       B.NAME || decode(T.PICNUM, '', '【' || 0 || '】', '【' || T.PICNUM || '】') || '张' when B.CODE = 'ht001' then
       B.NAME || decode(T.HTPICNUM, '', '【' || 0 || '】', '【' || T.HTPICNUM || '】') || '张' END CASE,
                                                    B.NAME,
                                                    T.SEQNO,
                                                    T.LICENSEID,
                                                    T.LICENSENAME,
                                                    T.SUPNAME,
                                                    T.SUPID,
                                                    T.OPERTIME,
                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.SUPID, T.SUPNAME,T.OPERTIME,T.FLAG,T.PICNUM,T.HTPICNUM
                                                        FROM DOC_LICENSE_LOG T
                                                        WHERE SUPID = '" + hfdSUPID.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'REL_LIC'").Tables[0];

                    GridLicense.DataSource = lisDT;
                    GridLicense.DataBind();
                    DataSearch();
                    //btnSave.Enabled = false;
                    //btnSubmit.Enabled = false;
                    //btnRollBack.Enabled = true;
                    //btnAudit.Enabled = true;
                    //btnReject.Enabled = true;
                }
                else
                {
                    Alert.Show("操作失败！", MessageBoxIcon.Warning);
                }
            }
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            int[] selections = GridLicense.SelectedRowIndexArray;
            if (GridLicense.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要审核的客户关系证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<CommandInfo> liscmd = new List<CommandInfo>();
            string succeed = string.Empty;
            bool ishave = false;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLicense.SelectedRowIndexArray[i];

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM doc_license_relationsup WHERE SUPID='" + GridLicense.DataKeys[rowIndex][1].ToString() + "'and iscur='Y'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    string seqno = DbHelperOra.GetSingle("select seqno from doc_license_log where supid = '" + GridLicense.DataKeys[rowIndex][1].ToString() + "' and iscur='Y' AND ROWNUM=1").ToString();
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where SUPID='" + GridLicense.DataKeys[rowIndex][1].ToString() + "' and iscur='Y'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        if (certDT.Rows.Count == selections.Length)
                        {
                            liscmd.Add(new CommandInfo("update doc_license_log set flag='Y' where SEQNO='" + seqno + "' and iscur='Y'", null));
                        }
                        else if (GridLicense.DataKeys[rowIndex][8].ToString().Substring(0, 3).Equals("合同书"))
                        {
                            liscmd.Add(new CommandInfo("update doc_license_log set flag='Y' where SEQNO='" + seqno + "' and licenseid='ht001' and iscur='Y'", null));
                        }
                        else
                        {
                            liscmd.Add(new CommandInfo("update doc_license_log set flag='Y' where SEQNO='" + seqno + "' and licenseid='cn001' and iscur='Y'", null));
                        }
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
         CASE when B.CODE = 'cn001' then
       B.NAME || decode(T.PICNUM, '', '【' || 0 || '】', '【' || T.PICNUM || '】') || '张' when B.CODE = 'ht001' then
       B.NAME || decode(T.HTPICNUM, '', '【' || 0 || '】', '【' || T.HTPICNUM || '】') || '张' END CASE,
                                                    B.NAME,
                                                    T.SEQNO,
                                                    T.LICENSEID,
                                                    T.LICENSENAME,
                                                    T.SUPNAME,
                                                    T.SUPID,
                                                    T.OPERTIME,
                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.SUPID, T.SUPNAME,T.OPERTIME,T.FLAG,T.PICNUM,T.HTPICNUM
                                                        FROM DOC_LICENSE_LOG T
                                                        WHERE SUPID = '" + hfdSUPID.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'REL_LIC'").Tables[0];

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
                    //btnSave.Enabled = false;
                    //btnSubmit.Enabled = false;
                    //btnRollBack.Enabled = true;
                    //btnAudit.Enabled = true;
                    //btnReject.Enabled = true;
                }
                else
                {
                    Alert.Show("操作失败！", MessageBoxIcon.Warning);
                }
            }

        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            int[] selections = GridLicense.SelectedRowIndexArray;
            if (GridLicense.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要审核的客户关系证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<CommandInfo> liscmd = new List<CommandInfo>();
            string succeed = string.Empty;
            bool ishave = false;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLicense.SelectedRowIndexArray[i];

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM doc_license_relationsup WHERE SUPID='" + GridLicense.DataKeys[rowIndex][1].ToString() + "'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    string seqno = DbHelperOra.GetSingle("select seqno from doc_license_log where supid = '" + GridLicense.DataKeys[rowIndex][1].ToString() + "' and iscur='Y' AND ROWNUM=1").ToString();
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where SUPID='" + GridLicense.DataKeys[rowIndex][1].ToString() + "'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        if (certDT.Rows.Count == selections.Length)
                        {
                            liscmd.Add(new CommandInfo("update doc_license_log set flag='Y' where SEQNO='" + seqno + "' and iscur='Y'", null));
                        }
                        else if (GridLicense.DataKeys[rowIndex][8].ToString().Substring(0, 3).Equals("合同书"))
                        {
                            liscmd.Add(new CommandInfo("update doc_license_log set flag='R' where SEQNO='" + hfdHSEQNO.Text + "' and licenseid='ht001'", null));
                        }
                        else
                        {
                            liscmd.Add(new CommandInfo("update doc_license_log set flag='R' where SEQNO='" + hfdHSEQNO.Text + "' and licenseid='cn001'", null));
                        }
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
         CASE when B.CODE = 'cn001' then
       B.NAME || decode(T.PICNUM, '', '【' || 0 || '】', '【' || T.PICNUM || '】') || '张' when B.CODE = 'ht001' then
       B.NAME || decode(T.HTPICNUM, '', '【' || 0 || '】', '【' || T.HTPICNUM || '】') || '张' END CASE,
                                                    B.NAME,
                                                    T.SEQNO,
                                                    T.LICENSEID,
                                                    T.LICENSENAME,
                                                    T.SUPNAME,
                                                    T.SUPID,
                                                    T.OPERTIME,
                                                    nvl(decode(T.FLAG,'N','已保存','S','已提交','Y','已审核','R','已驳回'),'待录入') FLAG,
                                                    decode(T.PICNUM,'','【'||0||'】','【'||T.PICNUM||'】')PICNUM
                                                FROM (SELECT T.SEQNO, T.LICENSEID, T.LICENSENAME, T.SUPID, T.SUPNAME,T.OPERTIME,T.FLAG,T.PICNUM,T.HTPICNUM
                                                        FROM DOC_LICENSE_LOG T
                                                        WHERE SUPID = '" + hfdSUPID.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'REL_LIC'").Tables[0];

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
                    //btnSave.Enabled = false;
                    //btnSubmit.Enabled = false;
                    //btnRollBack.Enabled = true;
                    //btnAudit.Enabled = true;
                    //btnReject.Enabled = true;
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
            string LISNAME = GridLicense.Rows[e.RowIndex].DataKeys[5].ToString().Substring(0,3);
            hfdLISID.Text = LISID;
            string HSEQNO = "";
            docLISNAME.Text = LISNAME;

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
                                                        WHERE supid = '" + hfdSUPID.Text + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'REL_LIC' and b.code='" + LISID + "'").Tables[0];
            if(lisDT.Rows[0]["SEQNO"].ToString().Length <= 0)
            {
                //Alert.Show("您还没有上传！");
                DataTable dt = DbHelperOra.Query("select flag from doc_license_log where supid = '" + hfdSUPID.Text + "' and rownum=1 and iscur='Y'").Tables[0];
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
                        btnSubmit.Enabled = false;
                        btnRollBack.Enabled = false;
                        dpkBEGRQ.Text = "";
                        dpkENDRQ.Text = @"";
                        dpkENDRQ.Enabled = true;
                        ischk.Checked = false;
                        docMEMO.Text = @"";
                        docDOCID.Text = "";
                    }
                }
                else
                {
                    btnSave.Enabled = true;
                    btnSubmit.Enabled = false;
                    btnRollBack.Enabled = false;
                    btnSave.Enabled = true;
                    btnSubmit.Enabled = false;
                    btnRollBack.Enabled = false;
                    dpkBEGRQ.Text = "";
                    dpkENDRQ.Text = @"";
                    dpkENDRQ.Enabled = true;
                    ischk.Checked = false;
                    docMEMO.Text = @"";
                    docDOCID.Text = "";
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
            else
            {
                //没有填写任何证照数据的时候，单击进行证照录入和已经录入证照后的查看信息。
                HSEQNO = GridLicense.Rows[e.RowIndex].DataKeys[0].ToString();
                hfdHSEQNO.Text = HSEQNO;
                hfdSUPID.Text = GridLicense.Rows[e.RowIndex].DataKeys[1].ToString();
                hfdFLAG.Text = GridLicense.Rows[e.RowIndex].DataKeys[7].ToString();
                DataTable licensedt = DbHelperOra.Query("select trunc(begrq) begrq,trunc(endrq) endrq,docid,memo from doc_license_relationsup where seqno='" + HSEQNO + "' and licenseid='" + LISID + "' AND ISCUR='Y'").Tables[0];
                if (licensedt.Rows[0][1].ToString().Equals("2099/1/1 0:00:00"))
                {
                    ischk.Checked = true;
                    dpkENDRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][1].ToString());
                    dpkBEGRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][0].ToString());
                    dpkENDRQ.Enabled = false;
                }
                else
                {
                    ischk.Checked = false;
                    dpkENDRQ.Enabled = true;
                    dpkENDRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][1].ToString());
                    dpkBEGRQ.SelectedDate = Convert.ToDateTime(licensedt.Rows[0][0].ToString());
                }
                docDOCID.Text = licensedt.Rows[0]["DOCID"].ToString();
                docMEMO.Text = licensedt.Rows[0]["MEMO"].ToString();
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
                    string strcase = GridLicense.Rows[e.RowIndex].DataKeys[8].ToString();
                    string HSEQNO = "";
                    string strlic = "";
                    HSEQNO = GridLicense.Rows[e.RowIndex].DataKeys[0].ToString();

                    if(strcase.Substring(0,3).Equals("合同书"))
                    {
                        strlic = "ht001";
                    }
                    else
                    {
                        strlic = "cn001";
                    }

                    //弹出图片展示框  （SEQNO, ROWNO, ISCUR, LICENSEID证照图片表主键）
                    Window1.Hidden = false;
                    string picnum = DbHelperOra.GetSingle("select count(1) from DOC_LICENSE_IMG where seqno='" + HSEQNO + "' and licenseid='" + LISID + "' AND ISCUR='Y'").ToString();
                    string url = "~/CertificateInput/ShowLisPicWindow.aspx?bm=" + HSEQNO + "&xc=" + picnum + "&cc=" + strlic + "";
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
                                                        WHERE SUPID = '" + supid + "' AND T.ISCUR='Y') T,DOC_LICENSE B WHERE B.CODE = T.LICENSEID(+) AND B.OBJUSER = 'REL_LIC'").Tables[0];


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
                docMEMO.Text = "";
                docDOCID.Text = "";
                hfdisChange.Text = "changec";
                PageContext.RegisterStartupScript("clearUpload();");
            }
        }

        protected void ischk_CheckedChanged(object sender, CheckedEventArgs e)
        {
            if (ischk.Checked)
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

        protected void GridCertype_RowCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "seepic")
            {
                try
                {
                    string LISID = GridCertype.Rows[e.RowIndex].DataKeys[4].ToString();
                    string LISNAME = GridCertype.Rows[e.RowIndex].DataKeys[5].ToString();
                    string strcase = GridCertype.Rows[e.RowIndex].DataKeys[8].ToString();
                    string HSEQNO = "";
                    string strlic = "";
                    HSEQNO = GridCertype.Rows[e.RowIndex].DataKeys[0].ToString();

                    if (strcase.Substring(0, 3).Equals("合同书"))
                    {
                        strlic = "ht001";
                    }
                    else
                    {
                        strlic = "cn001";
                    }

                    //弹出图片展示框  （SEQNO, ROWNO, ISCUR, LICENSEID证照图片表主键）
                    Window1.Hidden = false;
                    string picnum = DbHelperOra.GetSingle("select count(1) from DOC_LICENSE_IMG where seqno='" + HSEQNO + "' and licenseid='" + LISID + "'").ToString();
                    string url = "~/CertificateInput/ShowLisPicWindow.aspx?bm=" + HSEQNO + "&xc=" + picnum + "&cc=" + strlic + "";
                    PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdSEQNO.ClientID) + Window1.GetShowReference(url, "证照图片展示"));
                }
                catch (Exception)
                {
                    Alert.Show("暂无图片展示,请上传图片后再查看！");
                }
            }
        }

        protected void GridLIS_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                if (flag.Equals("待录入"))
                {
                    e.CellCssClasses[5] = "color2";       //单列加颜色(如果只想将单元格的字体变色，只需要将样式表里的background-color的属性去掉，加上color，然后alpha设置透明度即可)
                }
                else
                {
                    e.CellCssClasses[5] = "color1";       //单列加颜色
                }
                string flag1 = row["FLAG1"].ToString();
                if (flag1.Equals("待录入"))
                {
                    e.CellCssClasses[7] = "color2";     
                    
                }
                else
                {
                    e.CellCssClasses[7] = "color1";       
                }
            }
        }

        protected void mybtnAudit_Click(object sender, EventArgs e)
        {
            int[] selections = GridLIS.SelectedRowIndexArray;
            if (GridLIS.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要审核的客户关系证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<CommandInfo> liscmd = new List<CommandInfo>();
            string succeed = string.Empty;
            bool ishave = false;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLIS.SelectedRowIndexArray[i];

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM doc_license_relationsup WHERE SUPID='" + GridLIS.DataKeys[rowIndex][1].ToString() + "' AND ISCUR='Y'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    string seqno = DbHelperOra.GetSingle("select seqno from doc_license_log where supid = '" + GridLIS.DataKeys[rowIndex][1].ToString() + "' and iscur='Y' AND ROWNUM=1").ToString();
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where SUPID='" + GridLIS.DataKeys[rowIndex][1].ToString() + "' AND ISCUR='Y'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo("update doc_license_log set flag='Y' where SEQNO='" + seqno + "' AND ISCUR='Y'", null));        
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
        }

        protected void mybtnReject_Click(object sender, EventArgs e)
        {
            int[] selections = GridLIS.SelectedRowIndexArray;
            if (GridLIS.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择要审核的客户关系证照信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            List<CommandInfo> liscmd = new List<CommandInfo>();
            string succeed = string.Empty;
            bool ishave = false;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLIS.SelectedRowIndexArray[i];

                DataTable gdDT = DbHelperOra.Query("SELECT COUNT(1) MYCOUNT FROM doc_license_relationsup WHERE SUPID='" + GridLIS.DataKeys[rowIndex][1].ToString() + "'").Tables[0];
                if (Convert.ToDecimal(gdDT.Rows[0][0].ToString()) > 0)
                {
                    string seqno = DbHelperOra.GetSingle("select seqno from doc_license_log where supid = '" + GridLIS.DataKeys[rowIndex][1].ToString() + "' and iscur='Y' AND ROWNUM=1").ToString();
                    DataTable certDT = DbHelperOra.Query("select count(1) from doc_license_log where SUPID='" + GridLIS.DataKeys[rowIndex][1].ToString() + "'").Tables[0];
                    if (certDT.Rows.Count > 0)
                    {
                        liscmd.Add(new CommandInfo("update doc_license_log set flag='R' where SEQNO='" + seqno + "' AND ISCUR='Y'", null));
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
                        DataSearch1();
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
        }

    }
}
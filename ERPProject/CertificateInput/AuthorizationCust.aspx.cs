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
using XTBase;

namespace ERPProject.CertificateInput
{
    
    public partial class AuthorizationCust : PageBase
    {
        private static int picindex = 0;
        //用于判断是否保存并新增上级授权
        private static string picnum = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                DataInit();
                //屏蔽不需要的操作按钮
                if (Request.QueryString["oper"] != null)
                {
                    if (Request.QueryString["oper"].ToString() == "gl")
                    {
                        ButtonHidden(btnAudit1, btnReject1, btnMyAudit, btnMyReject);
                        DataSearch();
                    }
                    else if (Request.QueryString["oper"].ToString() == "sh")
                    {
                        ButtonHidden(btnSumbit, btnRollBack, btnSave, btnMySumbit, btnSumbit1, btnRollBack1, btnClear, btnSaveAndNew, btnNewAndAdd, btnNew, btnMyRollBack);
                        TabStrip1.ActiveTabIndex = 0;
                        DataSearch1();
                    }
                    else if(Request.QueryString["oper"].ToString() == "change")
                    {
                        ButtonHidden(btnAudit1, btnReject1, btnMyAudit, btnMyReject, btnNew, btnNewAndAdd, btnClear, btnMySumbit, btnMyReject, btnMyRollBack);
                        TabStrip1.ActiveTabIndex = 0;
                        DataSearch2();
                    }
                    else
                    {
                        ButtonHidden(btnSumbit, btnRollBack, btnSave, btnMySumbit, btnSumbit1, btnRollBack1, btnClear, btnSaveAndNew, btnNewAndAdd, btnNew, btnMyRollBack);
                        TabStrip1.ActiveTabIndex = 0;
                        DataSearch3();
                    }
                }
            }
        }

        private void DataInit()
        {
            TabStrip1.ActiveTabIndex = 0;
            trgAGENT.Enabled = false;
        }


        private void DataSearch()
        {
            string sql = string.Format(@"SELECT T.GRANTID,(SELECT COUNT(1) FROM DOC_LICENSE_GRANT WHERE GRANTID=T.GRANTID)SQLEVEL,
                            T.SUPID,
                            (SELECT SUPNAME FROM DOC_SUPPLIER WHERE SUPID = T.SUPID) SUPNAME,
                            T.SUPTOID,
                            DECODE(T.FLAG,'N','已保存','S','已提交','Y','已审批','R','已驳回') FLAG,
                            T.SQREGION,
                            TRUNC(T.BEGINSQRQ) BEGINSQRQ,
                            TRUNC(T.ENDSQRQ) ENDSQRQ,
                            T.OPERTIME,T.MEMO
                        FROM DOC_LICENSE_GRANTDOC T WHERE T.ISCUR='Y'");
            string query = "";
            StringBuilder strSql = new StringBuilder(sql);

            if (!string.IsNullOrWhiteSpace(tbxSUPNAME.Text))
            {
                query = tbxSUPNAME.Text.Trim();
                strSql.AppendFormat(" AND T.supid LIKE '%{0}%' or (SELECT SUPNAME FROM DOC_SUPPLIER WHERE SUPID = T.SUPID) like '%{0}%'", query);
            }
            int total = 0;
            DataTable dtData = PubFunc.DbGetPage(GridLIS.PageIndex, GridLIS.PageSize, strSql.ToString(), ref total);
            GridLIS.RecordCount = total;
            GridLIS.DataSource = dtData;
            GridLIS.DataBind();
        }


        private void DataSearch1()
        {
            string sql = string.Format(@"SELECT T.GRANTID,(SELECT COUNT(1) FROM DOC_LICENSE_GRANT WHERE GRANTID=T.GRANTID)SQLEVEL,
                            T.SUPID,
                            (SELECT SUPNAME FROM DOC_SUPPLIER WHERE SUPID = T.SUPID) SUPNAME,
                            T.SUPTOID,
                            DECODE(T.FLAG,'N','已保存','S','已提交','Y','已审批','R','已驳回') FLAG,
                            T.SQREGION,
                            TRUNC(T.BEGINSQRQ) BEGINSQRQ,
                            TRUNC(T.ENDSQRQ) ENDSQRQ,
                            T.OPERTIME,T.MEMO
                        FROM DOC_LICENSE_GRANTDOC T WHERE T.ISCUR='Y' AND T.FLAG='S' AND ISCHANGE='N'");
            string query = "";
            StringBuilder strSql = new StringBuilder(sql);

            if (!string.IsNullOrWhiteSpace(tbxSUPNAME.Text))
            {
                query = tbxSUPNAME.Text.Trim();
                strSql.AppendFormat(" AND T.supid LIKE '%{0}%' or (SELECT SUPNAME FROM DOC_SUPPLIER WHERE SUPID = T.SUPID) like '%{0}%'", query);
            }
            int total = 0;
            DataTable dtData = PubFunc.DbGetPage(GridLIS.PageIndex, GridLIS.PageSize, strSql.ToString(), ref total);
            GridLIS.RecordCount = total;
            GridLIS.DataSource = dtData;
            GridLIS.DataBind();
        }

        //授权书变更申请
        private void DataSearch2()
        {
            string sql = string.Format(@"SELECT T.GRANTID,(SELECT COUNT(1) FROM DOC_LICENSE_GRANT WHERE GRANTID=T.GRANTID)SQLEVEL,
                            T.SUPID,
                            (SELECT SUPNAME FROM DOC_SUPPLIER WHERE SUPID = T.SUPID) SUPNAME,
                            T.SUPTOID,
                            DECODE(T.FLAG,'N','已保存','S','已提交','Y','已审批','R','已驳回') FLAG,
                            T.SQREGION,
                            TRUNC(T.BEGINSQRQ) BEGINSQRQ,
                            TRUNC(T.ENDSQRQ) ENDSQRQ,
                            T.OPERTIME,T.MEMO
                        FROM DOC_LICENSE_GRANTDOC T WHERE T.ISCUR='Y' AND T.FLAG<>'S'");
            string query = "";
            StringBuilder strSql = new StringBuilder(sql);

            if (!string.IsNullOrWhiteSpace(tbxSUPNAME.Text))
            {
                query = tbxSUPNAME.Text.Trim();
                strSql.AppendFormat(" AND T.supid LIKE '%{0}%' or (SELECT SUPNAME FROM DOC_SUPPLIER WHERE SUPID = T.SUPID) like '%{0}%'", query);
            }
            int total = 0;
            DataTable dtData = PubFunc.DbGetPage(GridLIS.PageIndex, GridLIS.PageSize, strSql.ToString(), ref total);
            GridLIS.RecordCount = total;
            GridLIS.DataSource = dtData;
            GridLIS.DataBind();
        }

        //授权书变更审核
        private void DataSearch3()
        {
            string sql = string.Format(@"SELECT T.GRANTID,(SELECT COUNT(1) FROM DOC_LICENSE_GRANT WHERE GRANTID=T.GRANTID)SQLEVEL,
                            T.SUPID,
                            (SELECT SUPNAME FROM DOC_SUPPLIER WHERE SUPID = T.SUPID) SUPNAME,
                            T.SUPTOID,
                            DECODE(T.FLAG,'N','已保存','S','已提交','Y','已审批','R','已驳回') FLAG,
                            T.SQREGION,
                            TRUNC(T.BEGINSQRQ) BEGINSQRQ,
                            TRUNC(T.ENDSQRQ) ENDSQRQ,
                            T.OPERTIME,T.MEMO
                        FROM DOC_LICENSE_GRANTDOC T WHERE T.ISCUR='Y' AND T.FLAG='S' AND ISCHANGE='Y'");
            string query = "";
            StringBuilder strSql = new StringBuilder(sql);

            if (!string.IsNullOrWhiteSpace(tbxSUPNAME.Text))
            {
                query = tbxSUPNAME.Text.Trim();
                strSql.AppendFormat(" AND T.supid LIKE '%{0}%' or (SELECT SUPNAME FROM DOC_SUPPLIER WHERE SUPID = T.SUPID) like '%{0}%'", query);
            }
            int total = 0;
            DataTable dtData = PubFunc.DbGetPage(GridLIS.PageIndex, GridLIS.PageSize, strSql.ToString(), ref total);
            GridLIS.RecordCount = total;
            GridLIS.DataSource = dtData;
            GridLIS.DataBind();
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            string picurl = @"";
            if (e.EventArgument.IndexOf("mysave") >= 0)
            {
                //获取上传的授权书图片并和授权书相关数据一起保存
                JArray jaResult = JsonConvert.DeserializeObject<JArray>(hfdURL.Text);
                foreach (JToken jt in jaResult)
                {
                    picurl += jt["_raw"].ToString() + ",";
                }
                if (picurl.Length <= 0)
                {
                    Alert.Show("请传入授权书证照图片！");
                    return;
                }
                picurl = picurl.Substring(0, picurl.Length - 1);
                string[] arrays = picurl.Split(',');

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < GridList.SelectedRowIndexArray.Length; i++)
                {
                    sb.Append(GridList.SelectedRowIndexArray[i].ToString() + ",");
                }
                sb = sb.Remove(sb.Length - 1, 1);
                string seqnostr = hfdseqno.Text;

                List<CommandInfo> liscmd = new List<CommandInfo>();
                string sqtype = "";
                if (chkisLR.Checked)
                {
                    sqtype = "厂家直接授权";
                }
                else
                {
                    sqtype = "代理商授权";
                }

                //判断是否为同一层级关系
                decimal isinput = Convert.ToDecimal(DbHelperOra.GetSingle(string.Format(@"select count(1) from 
                         doc_license_grant where producer='{0}' and (suptoid='{1}' or supid='{2}') and iscur='Y'", hfdSupid.Text, hfdagent.Text, hfdbesup.Text)).ToString());
                string myseq = "";
                if (isinput > 0)
                {
                    string sqseqno = DbHelperOra.GetSingle(string.Format(@"select grantid from doc_license_grant where producer = '{0}' and (suptoid='{1}' or supid='{2}') and iscur='Y'", hfdSupid.Text, hfdagent.Text, hfdbesup.Text)).ToString();
                    myseq = sqseqno;
                }
                else
                {
                    myseq = seqnostr;
                    for (int i = 0; i < GridList.SelectedRowIndexArray.Length; i++)
                    {
                        int rowIndex = GridList.SelectedRowIndexArray[i];
                        string gdseq = GridList.DataKeys[rowIndex][0].ToString();
                        string gdname = GridList.DataKeys[rowIndex][1].ToString();
                        string gdspec = GridList.DataKeys[rowIndex][2].ToString();
                        liscmd.Add(new CommandInfo(@"insert into doc_license_grantgoods(GRANTID,FLAG,GDSEQ,GDNAME,SPEC,SUPPLIER,UPTIME,SUPBETO)
                                    values('" + myseq + "','N','" + gdseq + "','" + gdname + "','" + gdspec + "','" + trgSUPID.Text + "',sysdate,'" + hfdbesup.Text + "')", null));
                    }

                    //根据流水号判断是否为首级授权，然后将首级授权插入到doc_license_grantdoc中
                    if (sqtype.Equals("厂家直接授权"))
                    {
                        //如果是厂家直接授权，则生产商和授权方都是生产商
                        liscmd.Add(new CommandInfo(@"insert into doc_license_grantdoc(GRANTID,PRODUCER,SUPID,SUPTOID,SUPAGENT,FLAG,SQREGION,BEGINSQRQ,ENDSQRQ,OPERTIME,LRY,MEMO,GOODSINDEX,DOCID,SQTYPE,ISCUR,ISCHANGE)
                                    values('" + myseq + "','" + hfdSupid.Text + "','" + hfdSupid.Text + "','" + hfdbesup.Text + "','" + hfdagent.Text + "','N','" + tbxSQREGION.Text + "',to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd'),SYSDATE,'" + UserAction.UserID + "','" + tbxMEMO.Text + "','" + sb.ToString() + "','" + tbxDOCID.Text + "','" + sqtype + "','Y','N')", null));
                    }
                    else
                    {
                        //如果是代理商授权，则授权方是代理商
                        liscmd.Add(new CommandInfo(@"insert into doc_license_grantdoc(GRANTID,PRODUCER,SUPID,SUPTOID,SUPAGENT,FLAG,SQREGION,BEGINSQRQ,ENDSQRQ,OPERTIME,LRY,MEMO,GOODSINDEX,DOCID,SQTYPE,ISCUR,ISCHANGE)
                                    values('" + myseq + "','" + hfdSupid.Text + "','" + hfdagent.Text + "','" + hfdbesup.Text + "','" + hfdagent.Text + "','N','" + tbxSQREGION.Text + "',to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd'),SYSDATE,'" + UserAction.UserID + "','" + tbxMEMO.Text + "','" + sb.ToString() + "','" + tbxDOCID.Text + "','" + sqtype + "','Y','N')", null));
                    }
                }

                if (sqtype.Equals("厂家直接授权"))
                {
                    //如果是厂家直接授权，则生产商和授权方都是生产商
                    liscmd.Add(new CommandInfo(@"insert into doc_license_grant(GRANTID,PRODUCER,SUPID,SUPTOID,SUPAGENT,FLAG,SQREGION,BEGINSQRQ,ENDSQRQ,OPERTIME,LRY,MEMO,GOODSINDEX,DOCID,SQTYPE,ISCUR,ISCHANGE)
                                    values('" + myseq + "','" + hfdSupid.Text + "','" + hfdSupid.Text + "','" + hfdbesup.Text + "','" + hfdagent.Text + "','N','" + tbxSQREGION.Text + "',to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd'),SYSDATE,'" + UserAction.UserID + "','" + tbxMEMO.Text + "','" + sb.ToString() + "','" + tbxDOCID.Text + "','" + sqtype + "','Y','N')", null));
                }
                else
                {
                    //如果是代理商授权，则授权方是代理商
                    liscmd.Add(new CommandInfo(@"insert into doc_license_grant(GRANTID,PRODUCER,SUPID,SUPTOID,SUPAGENT,FLAG,SQREGION,BEGINSQRQ,ENDSQRQ,OPERTIME,LRY,MEMO,GOODSINDEX,DOCID,SQTYPE,ISCUR,ISCHANGE)
                                    values('" + myseq + "','" + hfdSupid.Text + "','" + hfdagent.Text + "','" + hfdbesup.Text + "','" + hfdagent.Text + "','N','" + tbxSQREGION.Text + "',to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd'),SYSDATE,'" + UserAction.UserID + "','" + tbxMEMO.Text + "','" + sb.ToString() + "','" + tbxDOCID.Text + "','" + sqtype + "','Y','N')", null));
                }

                for (int i = 1; i < arrays.Length + 1; i++)
                {
                    liscmd.Add(new CommandInfo(@"insert into DOC_LICENSE_SQIMG(SEQNO,ROWNO,IMGPATH,UPTTIME,LICENSEID,ISCUR,STR1,STR2)values('" + myseq + "','" + i + "','" + arrays[i - 1] + "',sysdate,'" + "SQ" + "','Y','" + hfdSupid.Text + "','" + hfdbesup.Text + "')", null));
                }

                if (DbHelperOra.ExecuteSqlTran(liscmd))
                {
                    Alert.Show("证照上传成功！");
                    btnSumbit.Enabled = true;
                    btnSaveAndNew.Enabled = true;
                    flagLbl.Text = "当前状态:已保存";
                    DataSearch();
                }
                else
                {
                    Alert.Show("证照上传失败！");
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (trgSUPID1.Text.Length <= 0)
            {
                Alert.Show("请选择被授权供应商！");
                return;
            }
            if (dpkBEGRQ.Text.Length <= 0)
            {
                Alert.Show("请为 授权日期* 提供有效值！");
                return;
            }
            if (dpkENDRQ.Text.Length <= 0)
            {
                Alert.Show("请为 截止日期* 提供有效值！");
                return;
            }
            DateTime dtBEGRQ = Convert.ToDateTime(dpkBEGRQ.Text);
            DateTime dtENDRQ = Convert.ToDateTime(dpkENDRQ.Text);
            if (dtBEGRQ > dtENDRQ)
            {
                Alert.Show("授权日期不能大于截止日期！");
                return;
            }
            if (GridList.SelectedRowIndexArray.Length <= 0)
            {
                Alert.Show("必须选择授权的商品！");
                return;
            }

            PageContext.RegisterStartupScript("save();");
        }

        protected void btnSumbit_Click(object sender, EventArgs e)
        {
            if (hfdseqno.Text.Length <= 0 || hfdbesup.Text.Length <= 0)
            {
                Alert.Show("请先录入授权信息再执行授权【提交】！");
                return;
            }
            Int32 isinput = Convert.ToInt32(DbHelperOra.GetSingle(string.Format("select count(1) from doc_license_grant t where grantid='{0}' and suptoid='{1}' and iscur='Y'", hfdseqno.Text, hfdbesup.Text)).ToString());
            if(isinput==0)
            {
                Alert.Show("请先录入授权信息再执行授权【提交】！");
                return;
            }
            List<CommandInfo> liscmd = new List<CommandInfo>();
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grant set flag='S' where grantid='{0}' and suptoid='{1}' and iscur='Y'", hfdseqno.Text, hfdbesup.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantgoods set flag='S' where grantid='{0}' and iscur='Y'", hfdseqno.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantdoc set flag='S' where grantid='{0}' and iscur='Y'", hfdseqno.Text), null));
            if (DbHelperOra.ExecuteSqlTran(liscmd))
            {
                Alert.Show("提交成功！");
                flagLbl.Text = "当前状态:已提交";
                btnRollBack.Enabled = true;
                btnSave.Enabled = false;
                btnSumbit.Enabled = true;
                btnNewAndAdd.Enabled = true;
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
                Alert.Show("提交失败！");
            }
        }

        protected void GridLIS_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridLIS.PageIndex = e.NewPageIndex;
            DataSearch();
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            string mystr = hfdValue.Text;
            JArray ja = JsonConvert.DeserializeObject<JArray>(mystr);
            bindProduct(ja);
        }

        protected void trgSUPID_TriggerClick(object sender, EventArgs e)
        {
            string url = "~/CertificateInput/SupplierList.aspx?bm=producer";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "生产厂商"));
        }

        //生产厂家选择后绑定相关数据
        private void bindProduct(JArray ja)
        {
            string codes = "";
            string names = "";
            foreach (JToken jt in ja)
            {
                JArray detailJa = (JArray)jt;
                codes += detailJa[0].ToString() + ";";
                names += detailJa[1].ToString() + ";";
            }
            names = names.TrimEnd(';');
            codes = codes.TrimEnd(';');

            trgSUPID.Text = names;
            hfdSupid.Text = codes;

            string seqnostr = DbHelperOra.GetSingle("SELECT SEQ_SQ.NEXTVAL FROM DUAL").ToString();
            string seqno = "SQ" + codes + seqnostr;
            hfdseqno.Text = seqno;
            string myseq = hfdseqno.Text + "|" + codes + "|" + names;

            PageContext.RegisterStartupScript("initUpload('" + myseq + "');");

            DataTable dt = DbHelperOra.Query(@"SELECT T.GDSEQ,(SELECT GDNAME FROM DOC_GOODS WHERE GDSEQ=T.GDSEQ) GDNAME,
                                      (SELECT GDSPEC FROM DOC_GOODS WHERE GDSEQ=T.GDSEQ) GDSPEC,
                                       (SELECT PRODUCER FROM DOC_GOODS WHERE GDSEQ=T.GDSEQ) PRODUCER
                                        FROM DOC_GOODSSUP T WHERE SUPID='" + codes + "' ").Tables[0];
            GridList.DataSource = dt;
            GridList.DataBind();

            //选择相关厂商之后，绑定该厂商下的商品
            int total = 0;
            //授权商品不给定分页暂将分页查询数据注释掉
            string prosql = string.Format(@"SELECT GDSEQ,GDNAME,GDSPEC,F_GETSUPNAME(PRODUCER) PRODUCER FROM DOC_GOODS WHERE PRODUCER='{0}'", codes);
            //DataTable prodt = DbHelperOra.Query(prosql).Tables[0];
            //GridList.DataSource = prodt;
            //GridList.DataBind();
            DataTable prodt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, prosql, ref total);
            GridList.RecordCount = total;
            GridList.DataSource = prodt;
            GridList.DataBind();
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

        protected void GridLIS_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string grantid = GridLIS.Rows[e.RowIndex].DataKeys[0].ToString();
            hfdseqnoa.Text = grantid;
            initPicUrl(grantid);
            DataTable dt = DbHelperOra.Query(@"SELECT t.GRANTID,t.PRODUCER,F_GETSUPNAME(T.PRODUCER)PRODUCERNAME,t.SUPID,F_GETSUPNAME(t.SUPID) SUPNAME,nvl(F_GETSUPNAME(SUPAGENT),'厂家直接授权')SUPAGENT,DOCID,MEMO,
                                              t.SUPTOID,F_GETSUPNAME(t.SUPTOID) SUPTONAME,
                                              decode(t.FLAG,'Y','已审核','N','已保存','S','已提交','R','已驳回')flag
                                              ,t.SQREGION,t.BEGINSQRQ,t.ENDSQRQ,T.MEMO FROM DOC_LICENSE_GRANT t 
                                              WHERE t.GRANTID='" + grantid + "' AND ISCUR='Y' order by opertime asc").Tables[0];
            TabStrip1.ActiveTabIndex = 1;
            Panel5.Hidden = true;
            Panel4.Hidden = false;
            GridGrant.DataSource = dt;
            GridGrant.DataBind();
            string flag = dt.Rows[0]["FLAG"].ToString();
            if (flag.Equals("已保存"))
            {
                Label1.Hidden = false;
                Label2.Hidden = true;
                Label1.Text = "当前状态:已保存";
                btnMyRollBack.Enabled = false;
                btnMySumbit.Enabled = true;
            }
            else if (flag.Equals("已提交"))
            {
                Label1.Hidden = false;
                Label2.Hidden = true;
                Label1.Text = "当前状态:已提交";
                btnMyRollBack.Enabled = true;
                btnMySumbit.Enabled = false;
            }
            else if (flag.Equals("已审核"))
            {
                Label1.Hidden = false;
                Label2.Hidden = true;
                Label1.Text = "当前状态:已审核";
            }
            else
            {
                Label1.Hidden = true;
                Label2.Hidden = false;
                Label2.Text = "当前状态:已驳回";
            }
            string suptoid = dt.Rows[0]["SUPTOID"].ToString();
            string pnum = DbHelperOra.GetSingle("select count(1) from doc_license_sqimg where seqno='" + grantid + "' and str2='" + suptoid + "' and iscur='Y'").ToString();
            lblPicNum.Text = "共有" + pnum + "张授权书";

            DataTable goodsDT = DbHelperOra.Query(@"select t.gdseq,t.gdname,t.gdspec,F_GETSUPNAME(t.producer)producer from doc_goods t where t.producer='" + dt.Rows[0]["PRODUCER"].ToString() + "' ").Tables[0];
            GridGoods.DataSource = goodsDT;
            GridGoods.DataBind();

            DataTable selectGoodsDT = DbHelperOra.Query("SELECT GOODSINDEX FROM DOC_LICENSE_GRANT WHERE GRANTID='" + grantid + "' AND ISCUR='Y'").Tables[0];
            string GOODSINDEX = selectGoodsDT.Rows[0][0].ToString();
            string[] arrays = GOODSINDEX.Split(',');
            int[] numbers = new int[arrays.Length];
            for(int i=0;i<arrays.Length;i++)
            {
               numbers[i] = Convert.ToInt32(arrays[i]);
            }

            GridGoods.SelectedRowIndexArray = numbers;
        }

        protected void btnSumbit1_Click(object sender, EventArgs e)
        {
            int[] selections = GridLIS.SelectedRowIndexArray;
            if (GridLIS.SelectedRowIndexArray.Length <= 0)
            {
                Alert.Show("请选择要提交的授权信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<CommandInfo> sqlList = new List<CommandInfo>();
            string succeed = string.Empty;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLIS.SelectedRowIndexArray[i];
                string strBILLNO = GridLIS.DataKeys[rowIndex][0].ToString();
                sqlList.Add(new CommandInfo(@"update doc_license_grant set flag='S' where grantid='" + strBILLNO + "' AND ISCUR='Y'", null));
                sqlList.Add(new CommandInfo(@"update doc_license_grantgoods set flag='S' where grantid='" + strBILLNO + "' AND ISCUR='Y'", null));
                sqlList.Add(new CommandInfo(@"update doc_license_grantdoc set flag='S' where grantid='" + strBILLNO + "' AND ISCUR='Y'", null));
                if (DbHelperOra.ExecuteSqlTran(sqlList))
                {
                    succeed += strBILLNO + ",";
                }
            }
            if (succeed.Length > 0)
            {
                Alert.Show("提交成功！");
                DataSearch();
            }
            else
            {
                Alert.Show("提交失败!", "消息提示", MessageBoxIcon.Warning);
            }
        }

        protected void btnDelete1_Click(object sender, EventArgs e)
        {
            int[] selections = GridLIS.SelectedRowIndexArray;
            if (GridLIS.SelectedRowIndexArray.Length <= 0)
            {
                Alert.Show("请选择要删除的授权信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<CommandInfo> sqlList = new List<CommandInfo>();
            string succeed = string.Empty;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLIS.SelectedRowIndexArray[i];
                string strBILLNO = GridLIS.DataKeys[rowIndex][0].ToString();
                sqlList.Add(new CommandInfo(@"delete from DOC_LICENSE_GRANT where grantid='" + strBILLNO + "' AND ISCUR='Y'", null));
                sqlList.Add(new CommandInfo(@"delete from DOC_LICENSE_GRANTGOODS where grantid='" + strBILLNO + "' AND ISCUR='Y'", null));
                sqlList.Add(new CommandInfo(@"delete from DOC_LICENSE_SQIMG where seqno='" + strBILLNO + "' AND ISCUR='Y'", null));
                if (DbHelperOra.ExecuteSqlTran(sqlList))
                {
                    succeed += strBILLNO + ",";
                }
            }
            if (succeed.Length > 0)
            {
                Alert.Show("删除成功！");
                DataSearch();
            }
            else
            {
                Alert.Show("删除失败!", "消息提示", MessageBoxIcon.Warning);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            trgSUPID.Text = @"";
            dpkENDRQ.Text = @"";
            dpkBEGRQ.Text = @"";
            tbxSQREGION.Text = @"";
            tbxMEMO.Text = @"";
            trgSUPID1.Text = "";
            tbxDOCID.Text = "";
            hfdagent.Text = "";
            hfdbesup.Text = "";
            hfdSupid.Text = "";
            hfdseqno.Text = "";
            trgAGENT.Text = "";
            GridList.DataSource = null;
            GridList.DataBind();
            PageContext.RegisterStartupScript("clearUpload();");
            btnSave.Enabled = true;
            btnSumbit.Enabled = false;
            btnRollBack.Enabled = false;
            Panel4.Hidden = true;
            Panel5.Hidden = false;
        }

        //protected void btnAudit_Click(object sender, EventArgs e)
        //{
        //    if (hfdseqno.Text.Length <= 0 || hfdbesup.Text.Length <= 0)
        //    {
        //        Alert.Show("请先录入授权信息再进行【审核】操作！");
        //        return;
        //    }
        //    Int32 isinput = Convert.ToInt32(DbHelperOra.GetSingle(string.Format("select count(1) from doc_license_grant t where grantid='{0}' and suptoid='{1}' AND ISCUR='Y'", hfdseqno.Text, hfdbesup.Text)).ToString());
        //    if (isinput == 0)
        //    {
        //        Alert.Show("请先录入授权信息再进行【审核】操作！");
        //        return;
        //    }
        //    List<CommandInfo> liscmd = new List<CommandInfo>();
        //    liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grant set flag='Y' where grantid='{0}' and suptoid='{1}' AND ISCUR='Y'", hfdseqno.Text, hfdbesup.Text), null));
        //    liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantgoods set flag='Y' where grantid='{0}' AND ISCUR='Y'", hfdseqno.Text), null));
        //    if (DbHelperOra.ExecuteSqlTran(liscmd))
        //    {
        //        Alert.Show("审核成功！");
        //    }
        //    else
        //    {
        //        Alert.Show("审核失败！");
        //    }
        //}

        //protected void btnReject_Click(object sender, EventArgs e)
        //{
        //    if (hfdseqno.Text.Length <= 0 || hfdbesup.Text.Length <= 0)
        //    {
        //        Alert.Show("请先录入授权信息再执行【驳回】操作！");
        //        return;
        //    }
        //    Int32 isinput = Convert.ToInt32(DbHelperOra.GetSingle(string.Format("select count(1) from doc_license_grant t where grantid='{0}' and suptoid='{1}' AND ISCUR='Y'", hfdseqno.Text, hfdbesup.Text)).ToString());
        //    if (isinput == 0)
        //    {
        //        Alert.Show("请先录入授权信息再执行【驳回】操作！");
        //        return;
        //    }
        //    List<CommandInfo> liscmd = new List<CommandInfo>();
        //    liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grant set flag='R' where grantid='{0}' and suptoid='{1}' AND ISCUR='Y'", hfdseqno.Text, hfdbesup.Text), null));
        //    liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantgoods set flag='R' where grantid='{0}' AND ISCUR='Y'", hfdseqno.Text), null));
        //    if (DbHelperOra.ExecuteSqlTran(liscmd))
        //    {
        //        Alert.Show("授权书已被驳回！");
        //    }
        //    else
        //    {
        //        Alert.Show("操作失败！");
        //    }
        //}

        protected void trgSUPID1_TriggerClick(object sender, EventArgs e)
        {
            string url = "~/CertificateInput/SupplierList.aspx?bm=supplier";
            PageContext.RegisterStartupScript(Window3.GetSaveStateReference(hfdValue2.ClientID) + Window3.GetShowReference(url, "被授权机构"));
        }

        protected void btnRollBack_Click(object sender, EventArgs e)
        {
            if (hfdseqno.Text.Length <= 0 || hfdbesup.Text.Length <= 0)
            {
                Alert.Show("请先录入授权信息再进行【撤回】！");
                return;
            }
            Int32 isinput = Convert.ToInt32(DbHelperOra.GetSingle(string.Format("select count(1) from doc_license_grant t where grantid='{0}' and suptoid='{1}' AND ISCUR='Y'", hfdseqno.Text, hfdbesup.Text)).ToString());
            if (isinput == 0)
            {
                Alert.Show("请先录入授权信息再进行【撤回】！");
                return;
            }
            List<CommandInfo> liscmd = new List<CommandInfo>();
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grant set flag='N' where grantid='{0}' and suptoid='{1}' AND ISCUR='Y'", hfdseqno.Text, hfdbesup.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantgoods set flag='N' where grantid='{0}' AND ISCUR='Y'", hfdseqno.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantdoc set flag='N' where grantid='{0}' AND ISCUR='Y'", hfdseqno.Text), null));
            if (DbHelperOra.ExecuteSqlTran(liscmd))
            {
                Alert.Show("撤回成功！");
                flagLbl.Text = "当前状态:已撤回";
                btnSave.Enabled = true;
                btnSumbit.Enabled = false;
                btnRollBack.Enabled = false;
                btnNewAndAdd.Enabled = true;
                DataSearch();
            }
            else
            {
                Alert.Show("撤回失败！");
            }
        }

        protected void chkisLR_CheckedChanged(object sender, CheckedEventArgs e)
        {
            if (chkisLR.Checked)
            {
                chknoLR.Checked = false;
                trgAGENT.Enabled = false;
                if (trgAGENT.Text.Length > 0)
                {
                    trgAGENT.Text = "";
                }
            }
            else
            {
                chknoLR.Checked = true;
                trgAGENT.Enabled = true;
            }
        }

        protected void chknoLR_CheckedChanged(object sender, CheckedEventArgs e)
        {
            if (chknoLR.Checked)
            {
                chkisLR.Checked = false;
                trgAGENT.Enabled = true;
            }
            else
            {
                chkisLR.Checked = true;
                trgAGENT.Enabled = false;
            }
        }

        protected void trgAGENT_TriggerClick(object sender, EventArgs e)
        {
            string url = "~/CertificateInput/SupplierList.aspx?bm=agent";
            PageContext.RegisterStartupScript(Window2.GetSaveStateReference(hfdValue1.ClientID) + Window2.GetShowReference(url, "上级代理商"));
        }

        protected void Window2_Close(object sender, WindowCloseEventArgs e)
        {
            string mystr = hfdValue1.Text;
            JArray ja = JsonConvert.DeserializeObject<JArray>(mystr);
            bindProduct1(ja);
        }

        //上级代理商选择后绑定相关数据
        private void bindProduct1(JArray ja)
        {
            string codes = "";
            string names = "";
            foreach (JToken jt in ja)
            {
                JArray detailJa = (JArray)jt;
                codes += detailJa[0].ToString() + ";";
                names += detailJa[1].ToString() + ";";
            }
            names = names.TrimEnd(';');
            codes = codes.TrimEnd(';');

            trgAGENT.Text = names;
            hfdagent.Text = codes;
        }

        protected void Window3_Close(object sender, WindowCloseEventArgs e)
        {
            string mystr = hfdValue2.Text;
            JArray ja = JsonConvert.DeserializeObject<JArray>(mystr);
            bindProduct2(ja);
        }

        //被授权机构选择后绑定相关数据
        private void bindProduct2(JArray ja)
        {
            string codes = "";
            string names = "";
            foreach (JToken jt in ja)
            {
                JArray detailJa = (JArray)jt;
                codes += detailJa[0].ToString() + ";";
                names += detailJa[1].ToString() + ";";
            }
            names = names.TrimEnd(';');
            codes = codes.TrimEnd(';');

            trgSUPID1.Text = names;
            hfdbesup.Text = codes;
        }

        private void datasearch()
        {
            int total = 0;
            string prosql = string.Format(@"SELECT GDSEQ,GDNAME,GDSPEC,F_GETSUPNAME(PRODUCER) PRODUCER FROM DOC_GOODS WHERE PRODUCER='{0}'", hfdSupid.Text);
            DataTable  prodt= PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, prosql, ref total);
            GridList.RecordCount = total;
            GridList.DataSource = prodt;
            GridList.DataBind();
        }

        protected void GridList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            datasearch();
        }

        private void datasearch1()
        {
            int total = 0;
            string prosql = string.Format(@"SELECT GDSEQ,GDNAME,GDSPEC,F_GETSUPNAME(PRODUCER) PRODUCER FROM DOC_GOODS WHERE PRODUCER='{0}'", hfdSupid.Text);
            DataTable prodt = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, prosql, ref total);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = prodt;
            GridGoods.DataBind();
        }

        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            datasearch1();
        }

        protected void GridGrant_RowCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "sqedit")
            {
                //授权相关信息更新或换证
                string grantid = GridGrant.Rows[e.RowIndex].DataKeys[0].ToString();
                string suptoid = GridGrant.Rows[e.RowIndex].DataKeys[1].ToString();
                string url = "";
                if (Request.QueryString["oper"].ToString() == "gl")
                {
                    url = "~/CertificateInput/SQEditWindow.aspx?bm=" + grantid + "&xc=" + suptoid + "&xg=" + "gl";
                }
                else if (Request.QueryString["oper"].ToString() == "gl")
                {
                    url = "~/CertificateInput/SQEditWindow.aspx?bm=" + grantid + "&xc=" + suptoid + "&xg=" + "sh";
                }
                else if (Request.QueryString["oper"].ToString() == "change")
                {
                    url = "~/CertificateInput/SQEditWindow.aspx?bm=" + grantid + "&xc=" + suptoid + "&xg=" + "change";
                }
                else
                {
                    url = "~/CertificateInput/SQEditWindow.aspx?bm=" + grantid + "&xc=" + suptoid + "&xg=" + "chkAudit";
                }

                PageContext.RegisterStartupScript(Window4.GetSaveStateReference(hfdValue3.ClientID) + Window4.GetShowReference(url, "编辑"));
            }
        }

        protected void GridGrant_RowClick(object sender, GridRowClickEventArgs e)
        {
            string grantid = GridGrant.Rows[e.RowIndex].DataKeys[0].ToString();
            string suptoid = GridGrant.Rows[e.RowIndex].DataKeys[1].ToString();
            hfdBesupper.Text = suptoid;
            hfdGRANTID.Text = grantid;

            DataTable picDt = DbHelperOra.Query(string.Format("select imgpath,rowno from DOC_LICENSE_SQIMG where seqno='{0}' and str2='" + suptoid + "' and rowno='" + 1 + "' AND ISCUR='Y' order by upttime,rowno asc", grantid)).Tables[0];
            string picpath = picDt.Rows[0][0].ToString();
            string rowno = picDt.Rows[0]["rowno"].ToString();
            imglbl.Text = rowno + "/" + picpath;
            picpath = picpath.Substring(1, picpath.Length - 1);
            imgBMPPATH.ImageUrl = ApiUtil.GetConfigCont("LIS_PICURL") + picpath;
            picindex = 1;
            string pnum = DbHelperOra.GetSingle("select count(1) from doc_license_sqimg where seqno='" + grantid + "' and str2='" + suptoid+ "' AND ISCUR='Y'").ToString();
            picnum = pnum;
            lblPicNum.Text = "共有" + pnum + "张授权书";
            btnLeft.Enabled = false;
            if (Convert.ToInt32(picnum) == 1)
            {
                btnRight.Enabled = false;
            }
            else
            {
                btnRight.Enabled = true;
            }
        }

        private void initPicUrl(string grantid)
        {
            DataTable picDt = DbHelperOra.Query(string.Format("select imgpath,rowno,str2 from DOC_LICENSE_SQIMG where seqno='{0}' AND ISCUR='Y' order by upttime,rowno asc", grantid)).Tables[0];
            string picpath = picDt.Rows[0][0].ToString();
            string rowno = picDt.Rows[0]["rowno"].ToString();
            imglbl.Text = rowno + "/" + picpath;
            picpath = picpath.Substring(1, picpath.Length - 1);
            imgBMPPATH.ImageUrl = ApiUtil.GetConfigCont("LIS_PICURL") + picpath;
            picindex = 1;
            hfdGRANTID.Text = grantid;
            hfdBesupper.Text = picDt.Rows[0]["str2"].ToString();

            string pnum = DbHelperOra.GetSingle("select count(1) from doc_license_sqimg where seqno='" + grantid + "' and ISCUR='Y' AND str2='" + picDt.Rows[0]["str2"].ToString()+ "'").ToString();
            picnum = pnum;

            btnLeft.Enabled = false;
            if (Convert.ToInt32(picnum) == 1)
            {
                btnRight.Enabled = false;
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
                string seqno = hfdGRANTID.Text;
                DataTable picDt = DbHelperOra.Query(string.Format("select imgpath,rowno from DOC_LICENSE_SQIMG where seqno='{0}' and str2='{1}' and rowno={2} AND ISCUR='Y'", seqno, hfdBesupper.Text, picindex)).Tables[0];
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
                    string seqno = hfdGRANTID.Text;
                    DataTable picDt = DbHelperOra.Query(string.Format("select imgpath,rowno from DOC_LICENSE_SQIMG where seqno='{0}' and str2='{1}' and rowno={2} AND ISCUR='Y'", seqno, hfdBesupper.Text, picindex)).Tables[0];
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
            if(Convert.ToInt32(picnum) == 1)
            {
                Alert.Show("证照图片到底了！");
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            trgSUPID.Text = @"";
            dpkENDRQ.Text = @"";
            dpkBEGRQ.Text = @"";
            tbxSQREGION.Text = @"";
            tbxMEMO.Text = @"";
            trgSUPID1.Text = "";
            tbxDOCID.Text = "";
            hfdagent.Text = "";
            hfdbesup.Text = "";
            hfdSupid.Text = "";
            hfdseqno.Text = "";
            trgAGENT.Text = "";
            GridList.DataSource = null;
            GridList.DataBind();
            PageContext.RegisterStartupScript("clearUpload();");
            btnSave.Enabled = true;
            btnSumbit.Enabled = true;
            Panel4.Hidden = true;
            Panel5.Hidden = false;
        }

        protected void GridGrant_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string SUPAG = row["SUPAGENT"].ToString();
                FineUIPro.BoundField SUPAGENT = GridGrant.FindColumn("SUPAGENT") as FineUIPro.BoundField;
                if (SUPAG == "厂家直接授权")
                {
                    e.CellCssClasses[SUPAGENT.ColumnIndex] = "color1";       //单列加颜色
                }
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (hfdseqno.Text.Length <= 0 || hfdbesup.Text.Length <= 0)
            {
                Alert.Show("请先录入授权信息再进行【删除】操作！");
                return;
            }
            Int32 isinput = Convert.ToInt32(DbHelperOra.GetSingle(string.Format("select count(1) from doc_license_grant t where grantid='{0}' and suptoid='{1}' and iscur='Y'", hfdseqno.Text, hfdbesup.Text)).ToString());
            if (isinput == 0)
            {
                Alert.Show("请先录入授权信息再进行【删除】操作！");
                return;
            }
            int cc = DbHelperOra.ExecuteSql(string.Format(@"delete from doc_license_grant where grantid='{0}' and suptoid='{1}' and iscur='Y'", hfdseqno.Text, hfdbesup.Text));
            decimal sqcount = Convert.ToDecimal(DbHelperOra.GetSingle("select count(1) from doc_licnese_grant where grantid='" + hfdseqno.Text + "' and iscur='Y'"));
            if(sqcount <= 0)
            {
                int cc1 = DbHelperOra.ExecuteSql("delete from doc_license_grantgoods where grantid='"+hfdseqno.Text+"' and iscur='Y'");
                if (cc1 > 0)
                {
                    Alert.Show("删除成功!");
                }
                else
                {
                    Alert.Show("删除失败！");
                }
            }
            else
            {
                if(cc >0)
                {
                    Alert.Show("删除成功!");
                }
                else
                {
                    Alert.Show("删除失败！");
                }
            }
        }

        protected void btnSaveAndNew_Click(object sender, EventArgs e)
        {
            DataTable goodsdt = DbHelperOra.Query(string.Format("SELECT GDSEQ,GDNAME,SPEC,F_GETSUPNAME(SUPPLIER) PRODUCER FROM doc_license_grantgoods WHERE GRANTID='{0}' and iscur='Y'", hfdseqno.Text)).Tables[0];
            if(goodsdt.Rows.Count<=0)
            {
                Alert.Show("请先保存再新增上级授权！");
                return;
            }
            GridList.DataSource = goodsdt;
            GridList.DataBind();
            DataTable selectGoodsDT = DbHelperOra.Query("SELECT GOODSINDEX FROM DOC_LICENSE_GRANT WHERE GRANTID='" + hfdseqno.Text + "' AND ISCUR='Y'").Tables[0];
            if(selectGoodsDT.Rows.Count>0)
            {
                string GOODSINDEX = selectGoodsDT.Rows[0][0].ToString();
                string[] arrays = GOODSINDEX.Split(',');
                int[] numbers = new int[arrays.Length];
                for (int i = 0; i < arrays.Length; i++)
                {
                    numbers[i] = Convert.ToInt32(arrays[i]);
                }
                GridList.SelectedRowIndexArray = numbers;
            }

            //保存并新增上级授权，厂商不变
            dpkENDRQ.Text = @"";
            dpkBEGRQ.Text = @"";
            tbxSQREGION.Text = @"";
            tbxMEMO.Text = @"";
            trgSUPID1.Text = "";
            tbxDOCID.Text = "";
            trgAGENT.Text = "";
            PageContext.RegisterStartupScript("clearUpload();");
            btnSave.Enabled = true;
            btnSumbit.Enabled = true;
            Panel4.Hidden = true;
            Panel5.Hidden = false;
        }

        protected void btnMySumbit_Click(object sender, EventArgs e)
        {
            List<CommandInfo> liscmd = new List<CommandInfo>();
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grant set flag='S' where grantid='{0}' and iscur='Y'", hfdseqnoa.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantgoods set flag='S' where grantid='{0}' and iscur='Y'", hfdseqnoa.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantdoc set flag='S' where grantid='{0}' and iscur='Y'", hfdseqnoa.Text), null));
            if (DbHelperOra.ExecuteSqlTran(liscmd))
            {
                Alert.Show("提交成功！");
                Label1.Text = "当前状态:已提交";
                btnMyRollBack.Enabled = true;
                btnMySumbit.Enabled = false;
                btnMyRollBack.Enabled = true;
            }
            else
            {
                Alert.Show("提交失败！");
            }
        }

        protected void btnMyRollBack_Click(object sender, EventArgs e)
        {
            List<CommandInfo> liscmd = new List<CommandInfo>();
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grant set flag='N' where grantid='{0}' and iscur='Y'", hfdseqnoa.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantgoods set flag='N' where grantid='{0}' and iscur='Y'", hfdseqnoa.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantdoc set flag='N' where grantid='{0}' and iscur='Y'", hfdseqnoa.Text), null));
            if (DbHelperOra.ExecuteSqlTran(liscmd))
            {
                Alert.Show("撤回成功！");
                Label1.Text = "当前状态:已撤回";
            }
            else
            {
                Alert.Show("撤回失败！");
            }
        }

        protected void btnMyDelete_Click(object sender, EventArgs e)
        {
            List<CommandInfo> liscmd = new List<CommandInfo>();
            liscmd.Add(new CommandInfo(string.Format("delete from doc_license_grant where grantid='{0}' and iscur='Y'", hfdseqnoa.Text), null));
            liscmd.Add(new CommandInfo(string.Format("delete from doc_license_grantgoods where grantid='{0}' and iscur='Y'", hfdseqnoa.Text), null));
            liscmd.Add(new CommandInfo(string.Format("delete from doc_license_sqimg where seqno='{0}' and iscur='Y'", hfdseqnoa.Text), null));
            if(DbHelperOra.ExecuteSqlTran(liscmd))
            {
                Alert.Show("删除成功！");
                TabStrip1.ActiveTabIndex = 0;
                DataSearch();
            }
        }

        protected void btnMyAudit_Click(object sender, EventArgs e)
        {
            List<CommandInfo> liscmd = new List<CommandInfo>();
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grant set flag='Y' where grantid='{0}' and iscur='Y'", hfdseqnoa.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantgoods set flag='Y' where grantid='{0}' and iscur='Y'", hfdseqnoa.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantdoc set flag='Y' where grantid='{0}' and iscur='Y'", hfdseqnoa.Text), null));
            if (DbHelperOra.ExecuteSqlTran(liscmd))
            {
                Alert.Show("审核成功！");
                Label1.Text = "当前状态:已审核";
                btnMyAudit.Enabled = false;
                btnMyReject.Enabled = false;
            }
            else
            {
                Alert.Show("审核失败！");
            }
        }

        protected void btnMyReject_Click(object sender, EventArgs e)
        {
            Window5.Hidden = false;
        }

        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            List<CommandInfo> liscmd = new List<CommandInfo>();
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grant set flag='R',memo='{0}' where grantid='{1}' and iscur='Y'", txtREJECTMEMO.Text, hfdseqnoa.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantgoods set flag='R' where grantid='{0}' and iscur='Y'", hfdseqnoa.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantdoc set flag='R',memo='{0}' where grantid='{1}' and iscur='Y'", txtREJECTMEMO.Text, hfdseqnoa.Text), null));
            if (DbHelperOra.ExecuteSqlTran(liscmd))
            {
                Alert.Show("驳回成功");
                Label2.Text = "当前状态:已驳回";
                Label1.Hidden = true;
                Label2.Hidden = false;
                Window5.Hidden = true;
                txtREJECTMEMO.Text = "";
                DataSearch1();
                btnMyAudit.Enabled = false;
                btnMyReject.Enabled = false;
            }
            else
            {
                Alert.Show("驳回失败！");
            }
        }

        protected void GridLIS_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string SUPAG = row["FLAG"].ToString();
                FineUIPro.BoundField SUPAGENT = GridGrant.FindColumn("FLAG") as FineUIPro.BoundField;
                if (SUPAG == "已驳回")
                {
                    e.CellCssClasses[2] = "color4";       //单列加颜色
                }
            }
        }

        protected void btnRollBack1_Click(object sender, EventArgs e)
        {
            int[] selections = GridLIS.SelectedRowIndexArray;
            if (GridLIS.SelectedRowIndexArray.Length <= 0)
            {
                Alert.Show("请选择要撤回的授权信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<CommandInfo> sqlList = new List<CommandInfo>();
            string succeed = string.Empty;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLIS.SelectedRowIndexArray[i];
                string strBILLNO = GridLIS.DataKeys[rowIndex][0].ToString();
                string flag = GridLIS.DataKeys[rowIndex][3].ToString();
                if (flag.Equals("已保存"))
                {
                    Alert.Show("当前是保存状态，无须撤回操作！");
                    return;
                }
                if (flag.Equals("已审核"))
                {
                    Alert.Show("当前是已审核状态，无法撤回！");
                    return;
                }
                if (flag.Equals("已驳回"))
                {
                    Alert.Show("当前是已驳回状态，无法撤回！");
                    return;
                }
                sqlList.Add(new CommandInfo(@"update doc_license_grant set flag='N' where grantid='" + strBILLNO + "' AND ISCUR='Y'", null));
                sqlList.Add(new CommandInfo(@"update doc_license_grantgoods set flag='N' where grantid='" + strBILLNO + "' AND ISCUR='Y'", null));
                sqlList.Add(new CommandInfo(@"update doc_license_grantdoc set flag='N' where grantid='" + strBILLNO + "' AND ISCUR='Y'", null));
                if (DbHelperOra.ExecuteSqlTran(sqlList))
                {
                    succeed += strBILLNO + ",";
                }
            }
            if (succeed.Length > 0)
            {
                Alert.Show("撤回成功！");
                DataSearch();
            }
            else
            {
                Alert.Show("撤回失败!", "消息提示", MessageBoxIcon.Warning);
            }
        }

        protected void btnAudit1_Click(object sender, EventArgs e)
        {
            int[] selections = GridLIS.SelectedRowIndexArray;
            if (GridLIS.SelectedRowIndexArray.Length <= 0)
            {
                Alert.Show("请选择要审核的授权信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<CommandInfo> sqlList = new List<CommandInfo>();
            string succeed = string.Empty;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLIS.SelectedRowIndexArray[i];
                string strBILLNO = GridLIS.DataKeys[rowIndex][0].ToString();
                string flag = GridLIS.DataKeys[rowIndex][3].ToString();
                if (flag.Equals("已保存"))
                {
                    Alert.Show("当前是保存状态，无法审核！");
                    return;
                }
                if (flag.Equals("已审核"))
                {
                    Alert.Show("当前是已审核状态，无须重复审核！");
                    return;
                }
                if (flag.Equals("已驳回"))
                {
                    Alert.Show("当前是已驳回状态，无法审核！");
                    return;
                }
                sqlList.Add(new CommandInfo(@"update doc_license_grant set flag='Y' where grantid='" + strBILLNO + "' AND ISCUR='Y'", null));
                sqlList.Add(new CommandInfo(@"update doc_license_grantgoods set flag='Y' where grantid='" + strBILLNO + "' AND ISCUR='Y'", null));
                sqlList.Add(new CommandInfo(@"update doc_license_grantdoc set flag='Y' where grantid='" + strBILLNO + "' AND ISCUR='Y'", null));
                if (DbHelperOra.ExecuteSqlTran(sqlList))
                {
                    succeed += strBILLNO + ",";
                }
            }
            if (succeed.Length > 0)
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
                Alert.Show("审核失败!", "消息提示", MessageBoxIcon.Warning);
            }
        }

        protected void btnReject1_Click(object sender, EventArgs e)
        {
            int[] selections = GridLIS.SelectedRowIndexArray;
            if (GridLIS.SelectedRowIndexArray.Length <= 0)
            {
                Alert.Show("请选择要驳回的授权信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            Window6.Hidden = false;
        }

        protected void btnClosePostBack1_Click(object sender, EventArgs e)
        {
            int[] selections = GridLIS.SelectedRowIndexArray;
            if (GridLIS.SelectedRowIndexArray.Length <= 0)
            {
                Alert.Show("请选择要驳回的授权信息！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<CommandInfo> sqlList = new List<CommandInfo>();
            string succeed = string.Empty;
            for (int i = 0; i < selections.Length; i++)
            {
                int rowIndex = GridLIS.SelectedRowIndexArray[i];
                string strBILLNO = GridLIS.DataKeys[rowIndex][0].ToString();
                string flag = GridLIS.DataKeys[rowIndex][3].ToString();
                if (flag.Equals("已保存"))
                {
                    Alert.Show("当前是保存状态，无法驳回！");
                    return;
                }
                if (flag.Equals("已审核"))
                {
                    Alert.Show("当前是已审核状态，无法驳回！");
                    return;
                }
                if (flag.Equals("已驳回"))
                {
                    Alert.Show("当前是已驳回状态，无须重复驳回！");
                    return;
                }
                sqlList.Add(new CommandInfo(@"update doc_license_grant set flag='R',memo='" + txtRejectMEMOE.Text + "' where grantid='" + strBILLNO + "' AND ISCUR='Y'", null));
                sqlList.Add(new CommandInfo(@"update doc_license_grantgoods set flag='R' where grantid='" + strBILLNO + "' AND ISCUR='Y'", null));
                sqlList.Add(new CommandInfo(@"update doc_license_grantdoc set flag='R',memo='" + txtRejectMEMOE.Text + "' where grantid='" + strBILLNO + "' AND ISCUR='Y'", null));
                if (DbHelperOra.ExecuteSqlTran(sqlList))
                {
                    succeed += strBILLNO + ",";
                }
            }
            if (succeed.Length > 0)
            {
                Alert.Show("成功驳回！");
                txtRejectMEMOE.Text = "";
                Window6.Hidden = true;
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
                Alert.Show("驳回失败!", "消息提示", MessageBoxIcon.Warning);
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            Window5.Hidden = true;
            txtREJECTMEMO.Text = "";
        }

        protected void btnClose1_Click(object sender, EventArgs e)
        {
            Window6.Hidden = true;
            txtRejectMEMOE.Text = "";
        }

        protected void btnNewAndAdd_Click(object sender, EventArgs e)
        {
            DataTable goodsdt = DbHelperOra.Query(string.Format("SELECT GDSEQ,GDNAME,SPEC,F_GETSUPNAME(SUPPLIER) PRODUCER FROM doc_license_grantgoods WHERE GRANTID='{0}' and iscur='Y'", hfdseqnoa.Text)).Tables[0];
            GridList.DataSource = goodsdt;
            GridList.DataBind();
            DataTable selectGoodsDT = DbHelperOra.Query("SELECT GOODSINDEX,F_GETSUPNAME(PRODUCER)PRODUCERNAME,PRODUCER FROM DOC_LICENSE_GRANT WHERE GRANTID='" + hfdseqnoa.Text + "' AND ISCUR='Y'").Tables[0];
            if (selectGoodsDT.Rows.Count > 0)
            {
                string GOODSINDEX = selectGoodsDT.Rows[0][0].ToString();
                string[] arrays = GOODSINDEX.Split(',');
                int[] numbers = new int[arrays.Length];
                for (int i = 0; i < arrays.Length; i++)
                {
                    numbers[i] = Convert.ToInt32(arrays[i]);
                }
                GridList.SelectedRowIndexArray = numbers;
            }
            trgSUPID.Text = selectGoodsDT.Rows[0]["PRODUCERNAME"].ToString();
            hfdSupid.Text = selectGoodsDT.Rows[0]["PRODUCER"].ToString(); ;
            dpkENDRQ.Text = @"";
            dpkBEGRQ.Text = @"";
            tbxSQREGION.Text = @"";
            tbxMEMO.Text = @"";
            trgSUPID1.Text = "";
            tbxDOCID.Text = "";
            hfdagent.Text = "";
            hfdbesup.Text = "";
            hfdseqno.Text = "";
            trgAGENT.Text = "";
            btnSave.Enabled = true;
            btnSumbit.Enabled = false;
            btnRollBack.Enabled = true;
            btnNewAndAdd.Enabled = false;
            Panel4.Hidden = true;
            Panel5.Hidden = false;
            hfdseqno.Text = hfdseqnoa.Text;
            PageContext.RegisterStartupScript("initUpload('" + hfdseqnoa.Text + "');");
        }

        protected void btnSearchGoods_Click(object sender, EventArgs e)
        {
            int total = 0;
            //授权商品不给定分页暂将分页查询数据注释掉
            string prosql = string.Format(@"SELECT GDSEQ,GDNAME,GDSPEC,F_GETSUPNAME(PRODUCER) PRODUCER FROM DOC_GOODS WHERE PRODUCER='{0}' and (gdseq like '%{1}%' or gdname like '%{1}%')", hfdSupid.Text, tbxSearchGoods.Text);
            DataTable prodt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, prosql, ref total);
            GridList.RecordCount = total;
            GridList.DataSource = prodt;
            GridList.DataBind();
        }

     
    }
}
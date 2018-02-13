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
    public partial class SQEditWindow : PageBase
    {
        private static string mygrantid = "";
        private static string mysuptoid = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //默认情况下换证标识就是N，当点击换证按钮的时候，isChange才是Y
                isChange.Text = "N";
                if (Request.QueryString["bm"] != null && Request.QueryString["bm"].ToString() != "")
                {
                    hfdGrantid.Text = Request.QueryString["bm"].ToString();
                    mygrantid = Request.QueryString["bm"].ToString();
                }
                if (Request.QueryString["xc"] != null && Request.QueryString["xc"].ToString() != "")
                {
                    hfdSuptoid.Text = Request.QueryString["xc"].ToString();
                    hfdbesup.Text = Request.QueryString["xc"].ToString();
                    mysuptoid = Request.QueryString["xc"].ToString();
                }
                if (Request.QueryString["xg"] != null && Request.QueryString["xg"].ToString() != "")
                {
                   if (Request.QueryString["xg"].ToString().Equals("gl"))
                   {
                       btnChange.Hidden = true;
                       btnSave.Hidden = false;
                       btnDelete.Hidden = false;
                       btnSumbit.Hidden = false;
                   }
                   else if (Request.QueryString["xg"].ToString().Equals("sh"))
                   {
                       btnChange.Hidden = true;
                       btnSave.Hidden = true;
                       btnDelete.Hidden = true;
                       btnSumbit.Hidden = true;
                   }
                   else if(Request.QueryString["xg"].ToString().Equals("change"))
                   {
                       btnChange.Hidden = false;
                       btnSave.Hidden = false;
                       btnDelete.Hidden = true;
                       btnSumbit.Hidden = false;
                   }
                   else
                   {
                       btnChange.Hidden = true;
                       btnSave.Hidden = true;
                       btnDelete.Hidden = true;
                       btnSumbit.Hidden = true;
                   }
                }
                init();
            }
        }

        private void init()
        {
            DataTable dtt = DbHelperOra.Query(@"SELECT PRODUCER,F_GETSUPNAME(PRODUCER)PRODUCERNAME,SUPID,F_GETSUPNAME(SUPID)SUPNAME,SUPTOID,F_GETSUPNAME(SUPTOID)SUPTONAME,FLAG,SQREGION,BEGINSQRQ,ENDSQRQ,
                                                DECODE(FLAG,'N','已保存','S','已提交','Y','已审批','R','已驳回') FLAGNAME,DOCID,SUPAGENT,GOODSINDEX,F_GETSUPNAME(SUPAGENT)SUPAGENTNAME,MEMO,SQTYPE FROM DOC_LICENSE_GRANT T WHERE GRANTID='" + hfdGrantid.Text + "' AND SUPTOID='" + hfdbesup.Text + "'").Tables[0];
            string producername = dtt.Rows[0]["PRODUCERNAME"].ToString();
            string producer = dtt.Rows[0]["PRODUCER"].ToString();
            string supname = dtt.Rows[0]["SUPNAME"].ToString();
            string suptoid = dtt.Rows[0]["SUPTOID"].ToString();
            string suptoname = dtt.Rows[0]["SUPTONAME"].ToString();
            string flag = dtt.Rows[0]["FLAG"].ToString();
            string sqregion = dtt.Rows[0]["SQREGION"].ToString();
            string beginsqrq = dtt.Rows[0]["BEGINSQRQ"].ToString();
            string endsqrq = dtt.Rows[0]["ENDSQRQ"].ToString();
            string docid = dtt.Rows[0]["DOCID"].ToString();
            string supagentname = dtt.Rows[0]["SUPAGENTNAME"].ToString();
            string supagent = dtt.Rows[0]["SUPAGENT"].ToString();
            string memo = dtt.Rows[0]["MEMO"].ToString();
            string sqtype = dtt.Rows[0]["SQTYPE"].ToString();
            string goodsindex = dtt.Rows[0]["GOODSINDEX"].ToString();
            string flagname = dtt.Rows[0]["FLAGNAME"].ToString();
            string supid = dtt.Rows[0]["SUPID"].ToString();

            if(flag.Equals("R"))
            {
                flagLbl1.Hidden = false;
                flagLbl.Hidden = true;
                flagLbl1.Text = "当前状态:" + flagname;
            }
            else
            {
                flagLbl.Text = "当前状态:" + flagname;
                if(flag.Equals("N"))
                {
                    btnSave.Enabled = true;
                    btnDelete.Enabled = true;
                    btnChange.Enabled = true;
                    btnSumbit.Enabled = false;
                }
                else if(flag.Equals("Y"))
                {
                    btnSave.Enabled = false;
                    btnDelete.Enabled = false;
                    btnSumbit.Enabled = false;
                    btnChange.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                    btnDelete.Enabled = false;
                    btnChange.Enabled = false;
                    btnSumbit.Enabled = false;
                }
            }

            hfdGoodsIndex.Text = goodsindex;
            hfdagent.Text = supagent;
            hfdbesup.Text = suptoid;
            hfdsupid.Text = supid;

            trgPRODUCER.Text = producername;
            trgAGENT.Text = supagentname;
            trgSUPID1.Text = suptoname;
            dpkBEGRQ.SelectedDate = Convert.ToDateTime(beginsqrq);
            dpkENDRQ.SelectedDate = Convert.ToDateTime(endsqrq);
            tbxSQREGION.Text = sqregion;
            tbxDOCID.Text = docid;
            tbxMEMO.Text = memo;


            //带出该授权书的授权商品
            string prosql = string.Format(@"SELECT GDSEQ,GDNAME,GDSPEC,F_GETSUPNAME(PRODUCER) PRODUCER FROM DOC_GOODS WHERE PRODUCER='{0}'", producer);
            DataTable prodt = DbHelperOra.Query(prosql).Tables[0];
            GridList.DataSource = prodt;
            GridList.DataBind();

            DataTable selectGoodsDT = DbHelperOra.Query("SELECT GOODSINDEX FROM DOC_LICENSE_GRANT WHERE GRANTID='" + hfdGrantid.Text + "'").Tables[0];
            string GOODSINDEX = selectGoodsDT.Rows[0][0].ToString();
            string[] arrays = GOODSINDEX.Split(',');
            int[] numbers = new int[arrays.Length];
            for (int i = 0; i < arrays.Length; i++)
            {
                numbers[i] = Convert.ToInt32(arrays[i]);
            }
            GridList.SelectedRowIndexArray = numbers;

            if (sqtype.Equals("厂家直接授权"))
            {
                trgAGENT.Enabled = false;
                chkisLR.Checked = true;
                chknoLR.Checked = false;
            }
            else
            {
                chknoLR.Checked = true;
                chkisLR.Checked = false;
            }
            //如果处于授权链条中而不是末尾，则不允许修改并置灰
            //decimal count = Convert.ToDecimal(DbHelperOra.GetSingle("SELECT COUNT(SUPAGENT) FROM DOC_LICENSE_GRANT WHERE GRANTID='" + hfdGrantid.Text + "' AND SUPAGENT = '" + suptoid + "'").ToString());
            //if(count>0)
            //{
            //    trgSUPID1.Enabled = false;
            //}
            PageContext.RegisterStartupScript("initUpload('" + "aaa" + "');");//调用图片选择控件，其中的aaa是随意填写的，不为空。
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

        //换证
        protected void btnChange_Click(object sender, EventArgs e)
        {
            //授权换证提供商品更改，新的授权开始日期及截止日期。目前思路是授权和被授权机构置灰不允许在换证的时候进行更改
            chkisLR.Enabled = false;
            chknoLR.Enabled = false;
            trgSUPID1.Enabled = false;
            trgAGENT.Enabled = false;
            dpkBEGRQ.Text = "";
            dpkENDRQ.Text = "";
            tbxDOCID.Text = "";
            tbxMEMO.Text = "";
            tbxSQREGION.Text = "";
            isChange.Text = "Y";
            btnSave.Enabled = true;
            btnSumbit.Enabled = true;
            DataTable dtt = DbHelperOra.Query(@"SELECT PRODUCER,F_GETSUPNAME(PRODUCER)PRODUCERNAME,SUPID,F_GETSUPNAME(SUPID)SUPNAME,SUPTOID,F_GETSUPNAME(SUPTOID)SUPTONAME,FLAG,SQREGION,BEGINSQRQ,ENDSQRQ,
                                                DECODE(FLAG,'N','已保存','S','已提交','Y','已审批','R','已驳回') FLAGNAME,DOCID,SUPAGENT,GOODSINDEX,F_GETSUPNAME(SUPAGENT)SUPAGENTNAME,MEMO,SQTYPE FROM DOC_LICENSE_GRANT T WHERE GRANTID='" + hfdGrantid.Text + "' AND SUPTOID='" + hfdbesup.Text + "'").Tables[0];
            string producer = dtt.Rows[0]["PRODUCER"].ToString();
            string prosql = string.Format(@"SELECT GDSEQ,GDNAME,GDSPEC,F_GETSUPNAME(PRODUCER) PRODUCER FROM DOC_GOODS WHERE PRODUCER='{0}'", producer);
            DataTable prodt = DbHelperOra.Query(prosql).Tables[0];
            GridList.DataSource = prodt;
            GridList.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //非换证直接保存的情况
            if(isChange.Text.Equals("N"))
            {
                string sqtype = "";
                if (chkisLR.Checked)
                {
                    sqtype = "厂家直接授权";
                }
                else
                {
                    sqtype = "代理商授权";
                }

                DateTime beginsqrq = Convert.ToDateTime(dpkBEGRQ.Text);
                DateTime endsqrq = Convert.ToDateTime(dpkENDRQ.Text);

                List<CommandInfo> liscmd = new List<CommandInfo>();
                string sb = string.Empty;
                for (int i = 0; i < GridList.SelectedRowIndexArray.Length; i++)
                {
                    sb = sb + GridList.SelectedRowIndexArray[i].ToString() + ",";
                }
                sb = sb.Remove(sb.Length - 1, 1);

                if (!sb.Equals(hfdGoodsIndex.Text))
                {
                    for (int i = 0; i < GridList.SelectedRowIndexArray.Length; i++)
                    {
                        int rowIndex = GridList.SelectedRowIndexArray[i];
                        string gdseq = GridList.DataKeys[rowIndex][0].ToString();
                        string gdname = GridList.DataKeys[rowIndex][1].ToString();
                        string gdspec = GridList.DataKeys[rowIndex][2].ToString();
                        liscmd.Add(new CommandInfo(@"insert into doc_license_grantgoods(GRANTID,FLAG,GDSEQ,GDNAME,SPEC,SUPPLIER,UPTIME,SUPBETO,ISCUR)
                                    values('" + hfdGrantid.Text + "','N','" + gdseq + "','" + gdname + "','" + gdspec + "','" + trgPRODUCER.Text + "',sysdate,'" + hfdbesup.Text + "','Y')", null));
                    }
                    liscmd.Add(new CommandInfo("UPDATE DOC_LICENSE_GRANT SET FLAG='N',SQTYPE='" + sqtype + "',SUPAGENT='" + hfdagent.Text + "',SUPTOID='" + hfdbesup.Text + "',BEGINSQRQ=to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),ENDSQRQ=to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd'),SQREGION='" + tbxSQREGION.Text + "',DOCID='" + tbxDOCID.Text + "',MEMO='" + tbxMEMO.Text + "',GOODSINDEX='" + sb.ToString() + "' where grantid='" + hfdGrantid.Text + "' and suptoid='" + hfdSuptoid.Text + "'", null));
                }
                else
                {
                    liscmd.Add(new CommandInfo("UPDATE DOC_LICENSE_GRANT SET FLAG='N',SQTYPE='" + sqtype + "',SUPAGENT='" + hfdagent.Text + "',SUPTOID='" + hfdbesup.Text + "',BEGINSQRQ=to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),ENDSQRQ=to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd'),SQREGION='" + tbxSQREGION.Text + "',DOCID='" + tbxDOCID.Text + "',MEMO='" + tbxMEMO.Text + "' where grantid='" + hfdGrantid.Text + "' and suptoid='" + hfdSuptoid.Text + "'", null));
                }
                if (DbHelperOra.ExecuteSqlTran(liscmd))
                {
                    Alert.Show("保存成功！");
                    flagLbl1.Hidden = true;
                    flagLbl.Hidden = false;
                    flagLbl.Text = "当前状态:已保存";
                    PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference());
                }
                else
                {
                    Alert.Show("保存失败！");
                }
            }
            else //执行换证操作后保存
            {
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
                string seqnostr = hfdGrantid.Text;

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

                DataTable dtt = DbHelperOra.Query(@"SELECT PRODUCER,F_GETSUPNAME(PRODUCER)PRODUCERNAME,SUPID,F_GETSUPNAME(SUPID)SUPNAME,SUPTOID,F_GETSUPNAME(SUPTOID)SUPTONAME,FLAG,SQREGION,BEGINSQRQ,ENDSQRQ,
                                                DECODE(FLAG,'N','已保存','S','已提交','Y','已审批','R','已驳回') FLAGNAME,DOCID,SUPAGENT,GOODSINDEX,F_GETSUPNAME(SUPAGENT)SUPAGENTNAME,MEMO,SQTYPE FROM DOC_LICENSE_GRANT T WHERE GRANTID='" + hfdGrantid.Text + "' AND SUPTOID='" + hfdbesup.Text + "'").Tables[0];
                string supid = dtt.Rows[0]["SUPID"].ToString();//授权方
                string producer = dtt.Rows[0]["PRODUCER"].ToString(); //生产厂商
                string suptoid = dtt.Rows[0]["SUPTOID"].ToString(); //被授权方
                string flag = dtt.Rows[0]["FLAG"].ToString(); //状态
                if(flag.Equals("N") || flag.Equals("R"))
                {
                    //如果证照是保存或撤回状态，直接删除掉已保存证照，再换上新证照
                    List<CommandInfo> delCmd = new List<CommandInfo>();
                    delCmd.Add(new CommandInfo("delete from doc_license_grant where grantid='" + hfdGrantid.Text + "' and suptoid='" + hfdbesup.Text + "' and iscur='Y'", null));
                    delCmd.Add(new CommandInfo("delete from doc_license_grantgoods where grantid='" + hfdGrantid.Text + "' and iscur='Y'", null));
                    delCmd.Add(new CommandInfo("delete from doc_license_sqimg where seqno='" + hfdGrantid.Text + "' and str2='" + hfdbesup.Text + "' and iscur='Y'", null));
                    DbHelperOra.ExecuteSqlTran(delCmd);
                }
                else
                {
                    //如果证照状态不是N或R的话，直接将现有证照变成老证照,ISCUR='N'（将状态直接变成N）
                    List<CommandInfo> liscommand = new List<CommandInfo>();
                    liscommand.Add(new CommandInfo("update doc_license_grant set iscur='N' where grantid='" + hfdGrantid.Text + "' and suptoid='" + hfdbesup.Text + "' and iscur='Y'", null));
                    liscommand.Add(new CommandInfo("update doc_license_grantgoods set iscur='N' where grantid='" + hfdGrantid.Text + "' and supbeto='" + hfdbesup.Text + "' and iscur='Y'", null));
                    liscommand.Add(new CommandInfo("update doc_license_sqimg SET ISCUR='N' where seqno='" + hfdGrantid.Text + "' and str2='" + hfdbesup.Text + "' and iscur='Y'", null));
                    DbHelperOra.ExecuteSqlTran(liscommand);
                }

                //换证之后，更新新录入的证照流水，其他老流水保持不变。插入本次换证的新证照
                string seqnostrnew = DbHelperOra.GetSingle("SELECT SEQ_LICENSE_PIC.NEXTVAL FROM DUAL").ToString();
                string myseq = "LIS" + producer + seqnostrnew;

                for (int i = 0; i < GridList.SelectedRowIndexArray.Length; i++)
                {
                    int rowIndex = GridList.SelectedRowIndexArray[i];
                    string gdseq = GridList.DataKeys[rowIndex][0].ToString();
                    string gdname = GridList.DataKeys[rowIndex][1].ToString();
                    string gdspec = GridList.DataKeys[rowIndex][2].ToString();
                    liscmd.Add(new CommandInfo(@"insert into doc_license_grantgoods(GRANTID,FLAG,GDSEQ,GDNAME,SPEC,SUPPLIER,UPTIME,SUPBETO,ISCUR)
                                    values('" + myseq + "','N','" + gdseq + "','" + gdname + "','" + gdspec + "','" + producer + "',sysdate,'" + hfdbesup.Text + "','Y')", null));
                }

                if (sqtype.Equals("厂家直接授权"))
                {
                    //如果是厂家直接授权，则生产商和授权方都是生产商
                    liscmd.Add(new CommandInfo(@"insert into doc_license_grant(GRANTID,PRODUCER,SUPID,SUPTOID,SUPAGENT,FLAG,SQREGION,BEGINSQRQ,ENDSQRQ,OPERTIME,LRY,MEMO,GOODSINDEX,DOCID,SQTYPE,ISCUR,ISCHANGE)
                                    values('" + myseq + "','" + producer + "','" + supid + "','" + hfdbesup.Text + "','" + hfdagent.Text + "','N','" + tbxSQREGION.Text + "',to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd'),SYSDATE,'" + UserAction.UserID + "','" + tbxMEMO.Text + "','" + sb.ToString() + "','" + tbxDOCID.Text + "','" + sqtype + "','Y','Y')", null));
                }
                else
                {
                    //如果是代理商授权，则授权方是代理商
                    liscmd.Add(new CommandInfo(@"insert into doc_license_grant(GRANTID,PRODUCER,SUPID,SUPTOID,SUPAGENT,FLAG,SQREGION,BEGINSQRQ,ENDSQRQ,OPERTIME,LRY,MEMO,GOODSINDEX,DOCID,SQTYPE,ISCUR,ISCHANGE)
                                    values('" + myseq + "','" + producer + "','" + hfdagent.Text + "','" + hfdbesup.Text + "','" + hfdagent.Text + "','N','" + tbxSQREGION.Text + "',to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd'),SYSDATE,'" + UserAction.UserID + "','" + tbxMEMO.Text + "','" + sb.ToString() + "','" + tbxDOCID.Text + "','" + sqtype + "','Y','Y')", null));
                }

                //授权链中有一个换证，那么将整个层级授权的流水更改成换证后的新流水,并且将层级中其他证照一并更换流水及授权商品、状态
                liscmd.Add(new CommandInfo("update doc_license_grant set grantid='" + myseq + "',flag='N',goodsindex='" + sb.ToString() + "' where grantid='" + hfdGrantid.Text + "' and iscur='Y'", null));
                liscmd.Add(new CommandInfo("update doc_license_grantgoods set grantid='" + myseq + "',flag='N' where grantid='" + hfdGrantid.Text + "' and iscur='Y'", null));
                liscmd.Add(new CommandInfo("update doc_license_sqimg set seqno='" + myseq + "' where seqno='" + hfdGrantid.Text + "' and iscur='Y'", null));
                
                //换证更新首级代理商
                DataTable chkddt = DbHelperOra.Query("select supid,suptoid from doc_license_grantdoc where grantid='"+hfdGrantid.Text+"'").Tables[0];
                if (chkddt.Rows[0][0].ToString().Equals(hfdbesup.Text) && chkddt.Rows[0][1].ToString().Equals(hfdsupid.Text))
                {
                    liscmd.Add(new CommandInfo("update doc_license_grantdoc set sqregion='"+tbxSQREGION.Text+"',beginsqrq=to_date('" + dpkBEGRQ.Text + "','yyyy/mm/dd'),endsqrq=to_date('" + dpkENDRQ.Text + "','yyyy/mm/dd'),grantid='" + myseq + "',flag='N',ischange='Y',goodsindex='" + sb.ToString() + "' where grantid='" + hfdGrantid.Text + "' and iscur='Y'", null));
                }
                else
                {
                    liscmd.Add(new CommandInfo("update doc_license_grantdoc set grantid='" + myseq + "',flag='N',ischange='Y' where grantid='" + hfdGrantid.Text + "' and iscur='Y'", null));
                }

                for (int i = 1; i < arrays.Length + 1; i++)
                {
                    liscmd.Add(new CommandInfo(@"insert into DOC_LICENSE_SQIMG(SEQNO,ROWNO,IMGPATH,UPTTIME,LICENSEID,ISCUR,STR1,STR2)values('" + myseq + "','" + i + "','" + arrays[i - 1] + "',sysdate,'" + "SQ" + "','Y','" + producer + "','" + hfdbesup.Text + "')", null));
                }

                if (DbHelperOra.ExecuteSqlTran(liscmd))
                {
                    Alert.Show("证照上传成功！");
                    flagLbl.Text = "当前状态:已保存";
                    //DataSearch();
                    PageContext.RegisterStartupScript(ActiveWindow.GetHideRefreshReference());
                }
                else
                {
                    Alert.Show("证照上传失败！");
                }
            }
        }
          
        private void DataSearch()
        {

        }

        protected void btnSumbit_Click(object sender, EventArgs e)
        {
            List<CommandInfo> liscmd = new List<CommandInfo>();
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grant set flag='S' where grantid='{0}' and suptoid='{1}'", hfdGrantid.Text, hfdbesup.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantgoods set flag='S' where grantid='{0}'", hfdGrantid.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantdoc set flag='S' where grantid='{0}'", hfdGrantid.Text), null));
            if (DbHelperOra.ExecuteSqlTran(liscmd))
            {
                Alert.Show("提交成功！");
                flagLbl.Text = "当前状态:已提交";
            }
            else
            {
                Alert.Show("提交失败！");
            }
        }

        protected void btnRollBack_Click(object sender, EventArgs e)
        {
            List<CommandInfo> liscmd = new List<CommandInfo>();
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grant set flag='N' where grantid='{0}' and suptoid='{1}'", hfdGrantid.Text, hfdbesup.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantgoods set flag='N' where grantid='{0}'", hfdGrantid.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantdoc set flag='N' where grantid='{0}'", hfdGrantid.Text), null));
            if (DbHelperOra.ExecuteSqlTran(liscmd))
            {
                Alert.Show("授权证照已成功撤回！");
                flagLbl.Text = "当前状态:已撤回";
            }
            else
            {
                Alert.Show("撤回失败！");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int cc = DbHelperOra.ExecuteSql(string.Format(@"delete from doc_license_grant where grantid='{0}' and suptoid='{1}' and iscur='Y'", hfdGrantid.Text, hfdbesup.Text));
            decimal sqcount = Convert.ToDecimal(DbHelperOra.GetSingle("select count(1) from doc_liense_grant where grantid='" + hfdGrantid.Text + "' and iscur='Y'"));
            if (sqcount <= 0)
            {
                int cc1 = DbHelperOra.ExecuteSql("delete from doc_license_grantgoods where grantid='" + hfdGrantid.Text + "'");
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
                if (cc > 0)
                {
                    Alert.Show("删除成功!");
                }
                else
                {
                    Alert.Show("删除失败！");
                }
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

        protected void trgSUPID1_TriggerClick(object sender, EventArgs e)
        {
            string url = "~/CertificateInput/SupplierList.aspx?bm=supplier";
            PageContext.RegisterStartupScript(Window3.GetSaveStateReference(hfdValue2.ClientID) + Window3.GetShowReference(url, "被授权机构"));
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

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            List<CommandInfo> liscmd = new List<CommandInfo>();
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grant set flag='Y' where grantid='{0}' and suptoid='{1}'", hfdGrantid.Text, hfdbesup.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantgoods set flag='Y' where grantid='{0}'", hfdGrantid.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantdoc set flag='Y' where grantid='{0}'", hfdGrantid.Text), null));
            if (DbHelperOra.ExecuteSqlTran(liscmd))
            {
                Alert.Show("审核成功！");
                flagLbl.Text = "当前状态:已审核";
                btnAudit.Enabled = false;
                btnReject.Enabled = false;
                btnSave.Enabled = false;
                btnSumbit.Enabled = false;
                btnDelete.Enabled = false;
                btnRollBack.Enabled = false;
            }
            else
            {
                Alert.Show("审核失败！");
            }
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            List<CommandInfo> liscmd = new List<CommandInfo>();
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grant set flag='R' where grantid='{0}' and suptoid='{1}'", hfdGrantid.Text, hfdbesup.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantgoods set flag='R' where grantid='{0}'", hfdGrantid.Text), null));
            liscmd.Add(new CommandInfo(string.Format(@"update doc_license_grantdoc set flag='R' where grantid='{0}'", hfdGrantid.Text), null));
            if (DbHelperOra.ExecuteSqlTran(liscmd))
            {
                Alert.Show("授权书被驳回！");
                flagLbl.Text = "当前状态:已驳回";
            }
            else
            {
                Alert.Show("操作失败！");
            }
        }
    }
}
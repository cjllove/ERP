using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XTBase;
using FineUIPro;
using System.Web.Caching;
using System.Data;
using System.IO;
using Newtonsoft.Json;

namespace ERPProject.ERPCart
{
    public partial class GoodsSearch : PageBase//System.Web.UI.Page
    {
        private int intPageIndex = 1;
        public string strPage = "1";
        // public string strPath = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["page"] != null)
            {
                strPage = Request.QueryString["page"];
                try
                {
                    intPageIndex = Convert.ToInt32(strPage);
                }
                catch
                {
                    intPageIndex = 1;
                }
            }

            if (!IsPostBack)
            {
                Path.Text = ApiUtil.GetConfigCont("PIC_PATH");

                PubFunc.DdlDataGet("DDL_SYS_DEPOT", ddlDEPTOUT);
                Object obj;
                if (HttpContext.Current.Request.Cookies["DEPTOUT"] == null)
                {
                    obj = DbHelperOra.GetSingle("select nvl((SELECT A.STOCK FROM SYS_DEPT A WHERE A.CODE = '" + UserAction.UserDept + "'),(select value from sys_para where code = 'DEFDEPT')) from dual");
                    HttpCookie cookie = new HttpCookie("DEPTOUT");
                    cookie.Value = obj.ToString();
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }
                else
                {
                    obj = HttpContext.Current.Request.Cookies["DEPTOUT"].Value;
                }
                ddlDEPTOUT.SelectedValue = obj.ToString();
            }
        }
        protected void RepeaterCat_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            System.Web.UI.WebControls.HiddenField hdfCode = e.Item.FindControl("hfdCode") as System.Web.UI.WebControls.HiddenField;
            Repeater repeaterCat = e.Item.FindControl("RepeaterCatN") as Repeater;
            Object obj;
            if (HttpContext.Current.Request.Cookies["DEPTOUT"] == null)
            {
                obj = "%";
            }
            else
            {
                obj = HttpContext.Current.Request.Cookies["DEPTOUT"].Value;
            }
            string strSqlCat = String.Format(@"SELECT CODE, NAME
                              FROM DOC_GOODSTYPE
                             WHERE ISDELETE = 'N'
                             AND EXISTS(SELECT 1 FROM SYS_DEPT WHERE TYPE IN ('3','4') AND CODE = '{1}')
                             AND CODE IN(SELECT DISTINCT A.CATID0 FROM DOC_GOODS A,DOC_GOODSCFG B,DOC_GOODSCFG C 
                                WHERE A.FLAG = 'Y' AND A.GDSEQ = B.GDSEQ AND A.GDSEQ = C.GDSEQ AND C.DEPTID LIKE NVL('{0}','%') AND B.DEPTID = '{1}')  ORDER BY CODE", obj.ToString(), UserAction.UserDept);

            repeaterCat.DataSource = DbHelperOra.Query(strSqlCat).Tables[0];
            repeaterCat.DataBind();
        }

        private void ItemDataBind()
        {


        }

        protected void ddlDEPTOUT_SelectedIndexChanged(object sender, EventArgs e)
        {
            HttpCookie cookie = new HttpCookie("DEPTOUT");
            cookie.Value = ddlDEPTOUT.SelectedValue;
            HttpContext.Current.Response.Cookies.Add(cookie);
            ItemDataBind();
            PageContext.RegisterStartupScript("$('#" + btnPostBack.ClientID + "').click();");
        }

        protected void BtnIns_Click(object sender, EventArgs e)
        {
            //载入模板
            GridTemplateLoad();
            Window2.Hidden = false;
        }
        private void GridTemplateLoad()
        {
            string sql = @"SELECT T.GROUPID, T.GROUPNAME, F_GETUSERNAME(T.LRY) USERNAME
                                    FROM DOC_GROUPDOC T
                                  WHERE F_CHK_DATARANGE(T.DEPTID, '{0}') = 'Y' AND T.FLAG='Y' AND T.TYPE='K' ";
            DataTable table = DbHelperOra.Query(string.Format(sql, UserAction.UserID)).Tables[0];
            GridTemplate.DataSource = table;
            GridTemplate.DataBind();
        }
        protected void GridTemplate_RowCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "FileDelete")
            {
                string gid = GridTemplate.Rows[e.RowIndex].DataKeys[0].ToString();
                DbHelperOra.ExecuteSql(string.Format("UPDATE DOC_GROUPDOC SET FLAG='N' WHERE GROUPID='{0}'", gid));
                GridTemplateLoad();
            }
        }
        protected void InsGoods()
        {
            if (string.IsNullOrWhiteSpace(ddlDEPTOUT.SelectedValue))
            {
                Alert.Show("请选择库房！", "警告提醒", MessageBoxIcon.Warning);
                return;
            }
            if (GridTemplate.SelectedRowIndex < 0)
            {
                Alert.Show("请选择要加载的模板！", "警告提醒", MessageBoxIcon.Warning);
                return;
            }
            string gid = GridTemplate.Rows[GridTemplate.SelectedRowIndex].DataKeys[0].ToString();
            if (!string.IsNullOrWhiteSpace(gid))
            {
                string sql = @"SELECT G.GDSEQ,A.DEPTID,B.SL
                                          FROM DOC_GOODS G, DOC_GOODSCFG P, DOC_GROUPDOC A, DOC_GROUPCOM B
                                         WHERE G.GDSEQ = P.GDSEQ
                                           AND A.GROUPID = B.GROUPID
                                           AND A.DEPTID = P.DEPTID
                                           AND G.GDSEQ = B.GDSEQ
                                           AND G.FLAG = 'Y'
                                           AND P.ISCFG IN ('Y', '1')
                                           AND A.GROUPID = '{0}'
                                           AND A.DEPTID='{1}' ";
                DataTable dtGoods = DbHelperOra.Query(string.Format(sql, gid,UserAction.UserDept)).Tables[0];
                List<CommandInfo> cmdList = new List<CommandInfo>();
                if (dtGoods != null && dtGoods.Rows.Count == 0)
                {
                    Alert.Show("当前模板非本科室模板，请得新选择！");
                    return;
                }
                
                foreach (DataRow row in dtGoods.Rows)
                {
                    //写入表中
                    if (DbHelperOra.Exists(String.Format("SELECT 1 FROM dat_cart WHERE USERID = '{0}' AND GDSEQ = '{1}'", UserAction.UserID, row["GDSEQ"])))
                    {
                        cmdList.Add(new CommandInfo(String.Format("UPDATE dat_cart SET dhs = NVL(DHS,0) + {0} WHERE USERID = '{1}' AND GDSEQ = '{2}'", row["SL"], UserAction.UserID, row["GDSEQ"]), null));
                    }
                    else
                    {
                        cmdList.Add(new CommandInfo(String.Format("insert into dat_cart (seqno,userid,gdseq,dhs,deptid) values (seq_cart.nextval,'{0}','{1}',{2},'{3}')", UserAction.UserID, row["GDSEQ"], row["SL"], ddlDEPTOUT.SelectedValue), null));
                    }
                }

                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    Window2.Hidden = true;
                    //Alert.Show("载入模板成功！", "消息提示", MessageBoxIcon.Information);
                    PageContext.RegisterStartupScript("TemplateRefresh()");
                }
            }
        }

        protected void btnLoadTemplateClose_Click(object sender, EventArgs e)
        {
            InsGoods();
        }

        protected void GridTemplate_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            InsGoods();
        }

        protected void btnPostBack_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "123", "$('#thirth').rlAccordion('mix', {childNum: 0});$('#iframeGoods')[0].contentWindow.document.location.reload(true); ", true);
        }

        protected void RepeaterCat_PreRender(object sender, EventArgs e)
        {
            string strSqlCat = @"SELECT 'A' CODE,'普通商品' NAME FROM DUAL";
            RepeaterCat.DataSource = DbHelperOra.Query(strSqlCat).Tables[0];
            RepeaterCat.DataBind();
        }
    }
}
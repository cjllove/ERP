using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPBasic
{
    public partial class SysCategory : PageBase
    {
        static string strTable = "SYS_CATEGORY";
        string SqlTree = @"select CODE,'【'||CODE||'】'||NAME NAME,BYCODE,BYNAME,TYPE,FLAG,CLASS,SJCODE,ISLAST,MANAGER
                            from @TABLE
                            where SJCODE='@CODE'  
                            order by code ";
        string SqlMx = "SELECT * FROM @TABLE WHERE CODE like '@CODE' order by code";
        protected void Page_Load(object sender, EventArgs e)
        {
            strTable = Request.QueryString["RULEID"];
            if (PubFunc.StrIsEmpty(strTable)) strTable = "SYS_CATEGORY";

            SqlTree = SqlTree.Replace("@TABLE", strTable);
            SqlMx = SqlMx.Replace("@TABLE", strTable);
            tbxRule.Text = DbHelperOra.GetSingle("select RULEDEF from sys_globrule where ruleid='" + strTable + "'").ToString();
            if (!IsPostBack)
            {
                InitDdl();
                InitTree();
            }
        }

        private void InitTree()
        {
            DataTable dtMenu = DbHelperOra.Query(SqlTree.Replace("@CODE", "0")).Tables[0];
            TreeDic.EnableSingleClickExpand = true;
            TreeDic.Nodes.Clear();
            foreach (DataRow dr in dtMenu.Rows)
            {
                FineUIPro.TreeNode node = new FineUIPro.TreeNode();
                node.Text = dr["name"].ToString();
                node.NodeID = dr["code"].ToString();
                node.EnableClickEvent = true;
                node.EnableExpandEvent = true; 
                TreeDic.Nodes.Add(node);
                TreeSubGet(node.Nodes, dr);
            }
            //加载界面不直接显示数据
            //PubFunc.GridDataSet(GridMx, DbHelperOra.Query(SqlMx.Replace("@CODE", "%")).Tables[0]);
        }
        private void TreeSubGet(FineUIPro.TreeNodeCollection TreeNodes, DataRow Menu)
        {
            string MenuSqlSub = SqlTree.Replace("@CODE", Menu["code"].ToString());
            DataTable dtMenu = DbHelperOra.Query(MenuSqlSub).Tables[0];

            foreach (DataRow dr in dtMenu.Rows)
            {
                FineUIPro.TreeNode node = new FineUIPro.TreeNode();
                node.Text = dr["name"].ToString();
                node.NodeID = dr["code"].ToString();
                node.EnableClickEvent = true;
                node.EnableExpandEvent = true; 
                TreeNodes.Add(node);
                TreeSubGet(node.Nodes, dr);
            }
        }
        private void InitDdl()
        {
            PubFunc.DdlDataGet(ddlManager, "DDL_USER");
            PubFunc.DdlDataGet(ddlType, "DDL_" + strTable + "_TYPE");

            string strsql = @"select  CODE,NAME from (
                                    SELECT '0' CODE ,'无' NAME  FROM dual
                                    union all
                                    select code,name from @TABLE where islast='N'
                                    )
                                    ORDER BY DECODE(CODE,'',99,0) DESC ,CODE ASC";
            strsql = strsql.Replace("@TABLE", strTable);

            PubFunc.DdlDataSql(ddlSJCODE, strsql);

            DataTable dtDDLdata = (DataTable)ddlSJCODE.DataSource;
            ddlSearch.DataTextField = dtDDLdata.Columns[1].ToString();
            ddlSearch.DataValueField = dtDDLdata.Columns[0].ToString();
            ddlSearch.DataSource = dtDDLdata;
            ddlSearch.DataBind();
        }
        private void InitFrom(String strCode)
        {
            if (strCode == "")
            {
                PubFunc.FormDataClear(FormMx);
                ddlSJCODE.SelectedValue = "";
                tbxCode.Enabled = true;
            }
            else
            {
                PubFunc.FormDataSet(FormMx, DbHelperOra.Query(SqlMx.Replace("@CODE", strCode)).Tables[0].Rows[0]);
                tbxCode.Enabled = false;
            }
        }
        protected void TreeDic_NodeCommand(object sender, FineUIPro.TreeCommandEventArgs e)
        {
            InitFrom(e.Node.NodeID);
        }
        protected void TreeDic_NodeExpand(object sender, FineUIPro.TreeNodeEventArgs e)
        {
            TreeDic.SelectedNodeID = e.Node.NodeID;
            InitFrom(e.Node.NodeID);
        }
        private void DataAdd()
        {
            string ls_parent = TreeDic.SelectedNodeID;
            string ls_ddlManager = ddlManager.SelectedValue;
            int li_maxLen = CodeLen("99");
            object obj=DbHelperOra.GetSingle("select  nvl(islast,'Y')   from " + strTable + " where code='" + ls_parent + "'");
            if (obj != null)
            {
                if ((obj.ToString()).Equals("Y"))
                {
                    Alert.Show("【" + ls_parent + "】已是末级，不能增加子级！");
                    return;
                }
                if (ls_parent.Length == li_maxLen)
                {
                    Alert.Show("【" + ls_parent + "】已是末级，不能增加子级！");
                    return;
                }
            }
           
            InitDdl();
            InitFrom("");

            if (PubFunc.StrIsEmpty(ls_parent))
            {
                ddlSJCODE.SelectedValue = "0";
                tbxCLASS.Text = "1";
            }
            else
            {
                tbxCLASS.Text = DbHelperOra.GetSingle("select  nvl(CLASS,0) + 1  from " + strTable + " where code='" + ls_parent + "'").ToString();
                ddlSJCODE.SelectedValue = ls_parent;
                ddlType.SelectedValue = DbHelperOra.GetSingle("select TYPE from " + strTable + " where code='" + ls_parent + "'").ToString();
            }
            //ddlType.Enabled = (tbxCLASS.Text == "1");

            ddlManager.SelectedValue = ls_ddlManager;
            cbxFlag.Checked = true;
            tbxCode.Enabled = true;
            tbxCode.Text = CodeGet(ddlSJCODE.SelectedValue, tbxCLASS.Text);
            cbxIsLast.Checked = (li_maxLen == tbxCode.Text.Length);
        }
        //增加
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            DataAdd();
        }
        //字典类别删除
        protected void btnDel_Click(object sender, EventArgs e)
        {
            MyTable mtTable = new MyTable(strTable, "CODE");
            mtTable.ColRow["CODE"] = PubFunc.FormDataGet(FormMx, "CODE");
            if (PubFunc.StrIsEmpty(mtTable.ColRow["CODE"].ToString()))
            {
                Alert.Show("请选择要删除的数据！");
                return;
            }
            else if (DbHelperOra.Exists("select 1 from " + strTable + " where SJCODE='" + mtTable.ColRow["CODE"].ToString() + "'"))
            {
                Alert.Show("数据【" + mtTable.ColRow["CODE"].ToString() + "】有下级，不能删除！");
                return;
            }
            mtTable.DeleteExec("");
            InitDdl();
            InitTree();
            InitFrom("");
            tbxCode.Enabled = true;
            Alert.Show("数据【" + mtTable.ColRow["CODE"] + "】删除成功！");
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (PubFunc.FormDataCheck(FormMx).Length > 0) return;
            //判断此类别是否存在
            if ((DbHelperOra.Exists("select 1 from SYS_CATEGORY where code = '" + tbxCode.Text + "'")) && (tbxCode.Enabled))
            {
                Alert.Show("此商品类别编码已经存在,请检查!");
                return;
            }

            string ls_codelabel;
            MyTable mtTable = new MyTable(strTable, "CODE");
            mtTable.ColRow = PubFunc.FormDataHT(FormMx);
            ls_codelabel = PubFunc.FormLabelGet(FormMx, "CODE");
            if (PubFunc.StrIsEmpty(mtTable.ColRow["CODE"].ToString()))
            {
                Alert.Show("【" + ls_codelabel + "】不能为空！");
                return;
            }
            else if (mtTable.ColRow["CODE"].ToString().Length != CodeLen(tbxCLASS.Text))
            {
                Alert.Show("【" + ls_codelabel + "】长度应该为【" + CodeLen(tbxCLASS.Text).ToString() + "】，请修改！");
                return;
            }
            if (tbxCode.Enabled)  //新增
            {
                mtTable.InsertExec();
            }
            else
            {
                mtTable.UpdateExec("");
            }
            Alert.Show("数据保存成功！");
            tbxCode.Enabled = false;
            InitTree();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string strCode = ddlSearch.SelectedValue;
            string strSql;
            if (strCode == null || strCode == "" || strCode == "0")
            {
                strSql = "select * from " + strTable + " order by  code";
            }
            else
            {
                strSql = "select * from " + strTable + " start with code='" + strCode + "' connect by prior code = SJCODE order by  code";
            }

            PubFunc.GridDataSet(GridMx, DbHelperOra.Query(strSql).Tables[0]);
        }
        protected void btnExp_Click(object sender, EventArgs e)
        {
            if (GridMx.Rows.Count < 1)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=信息导出.xls");
            Response.ContentType = "application/excel";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Write(PubFunc.GridToHtml(GridMx));
            Response.End();

            btnExp.Enabled = true;
        }

        private int CodeLen(string strLEVELS)
        {//得到编码的长度，如果传入99则取最大长度。
            int li_levels = Convert.ToInt32(strLEVELS);
            int li_len = 0;
            string[] ls_rule = tbxRule.Text.Split('-');
            if (li_levels == 99) { li_levels = ls_rule.Length; }
            for (int i = 0; i < li_levels; i++)
            {
                li_len = li_len + Convert.ToInt32(ls_rule[i]);
            }

            return li_len;
        }
        private string CodeGet(string strParent, string strLEVELS)
        {
            string strCode;
            string strSql;
            int li_len;
            if (strLEVELS == "0")
            {
                li_len = Convert.ToInt32(tbxRule.Text.Split('-')[0]);
                strSql = "select nvl(max(code),substr('00000',1," + li_len.ToString() + ")) from " + strTable + " where CLASS=" + strLEVELS + "";
            }
            else
            {
                li_len = Convert.ToInt32(strLEVELS);
                li_len = Convert.ToInt32(tbxRule.Text.Split('-')[li_len - 1]);
                strSql = "select nvl(max(code),'" + strParent + "'||substr('00000',1," + li_len.ToString() + ")) from " + strTable + " where SJCODE='" + strParent + "'";
            }
            strCode = DbHelperOra.GetSingle(strSql).ToString();
            if (PubFunc.isNumeric(strCode))
            {
                strCode = (Convert.ToInt32("1" + strCode) + 1).ToString();
                strCode = strCode.Substring(1, strCode.Length - 1);
                return strCode;
            }
            else
            {
                return "";
            } 
        }
    }
}
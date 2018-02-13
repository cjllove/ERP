﻿using FineUIPro;
using XTBase;
using XTBase.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPBasic
{
    public partial class BasicInfo : PageBase
    {
        static string strTable = "SYS_DEPT";
        string SqlTree = @"select CODE,'【'||CODE||'】'||NAME NAME,BYCODE,BYNAME,TYPE,FLAG,CLASS,SJCODE,ISLAST,MANAGER,ISORDER,SHZQ,KCZQ
                            from @TABLE
                            where SJCODE='@CODE'  
                            order by code ";
        string SqlMx = "SELECT * FROM @TABLE WHERE CODE like '@CODE' order by code";
        protected void Page_Load(object sender, EventArgs e)
        {
            strTable = Request.QueryString["RULEID"];
            if (PubFunc.StrIsEmpty(strTable)) strTable = "SYS_DEPT";

            SqlTree = SqlTree.Replace("@TABLE", strTable);
            SqlMx = SqlMx.Replace("@TABLE", strTable);
            //tbxRule.Text = DbHelperOra.GetSingle("select RULEDEF from sys_globrule where ruleid='" + strTable + "'").ToString();
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

                TreeNodes.Add(node);
                TreeSubGet(node.Nodes, dr);
            }
        }
        private void InitDdl()
        {
            PubFunc.DdlDataGet(ddlManager, "DDL_USER");
            PubFunc.DdlDataGet(ddlType, "DDL_" + strTable + "_TYPE");
            PubFunc.DdlDataGet("DDL_USER", ddlSTR4);
            PubFunc.DdlDataGet(ddlSTR3, "DAT_LX");
            PubFunc.DdlDataGet(ddlHOUSE, "DAT_LOUDONG");
            PubFunc.DdlDataGet(ddlFLOOR, "DDL_LOUCENG");
            //PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE_ORDER", UserAction.UserID, ddlSTOCK);
            string sqlDepotrange = @"
select '0' code,'无' name from dual
union all
SELECT CODE, NAME
  FROM(SELECT '' CODE， '--请选择--' NAME
          FROM DUAL
        UNION ALL
        SELECT CODE, '[' || CODE || ']' || NAME NAME
          FROM SYS_DEPT
         WHERE  TYPE IN('1', '2') AND ISORDER = 'Y' AND F_CHK_DATARANGE(CODE, '{0}') = 'Y')
 ORDER BY CODE DESC       ";
            PubFunc.DdlDataSql(ddlSTOCK, string.Format(sqlDepotrange, UserAction.UserID));

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
            int li_maxLen = CodeLen("99");
            if (DbHelperOra.Exists("SELECT 1 FROM SYS_DEPT WHERE CODE = '" + ls_parent + "' AND ISLAST = 'Y'"))
            {
                Alert.Show("【" + ls_parent + "】已是末级，不能增加子级！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            InitDdl();
            InitFrom("");

            if (PubFunc.StrIsEmpty(ls_parent))
            {
                ddlSJCODE.SelectedValue = "0";
                ddlISORDER.SelectedValue = "Y";
                tbxCLASS.Text = "1";
                ddlType.SelectedValue = "1";
            }
            else
            {
                tbxCLASS.Text = DbHelperOra.GetSingle("select  nvl(CLASS,0) + 1  from " + strTable + " where code='" + ls_parent + "'").ToString();
                ddlSJCODE.SelectedValue = ls_parent;
                ddlType.SelectedValue = "3";
                ddlISORDER.SelectedValue = "N";
            }
            //ddlType.Enabled = (tbxCLASS.Text == "1");

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
            TreeDic.SelectedNodeID = null;//解决删除后点击增加会报错的问题
            Alert.Show("数据【" + mtTable.ColRow["CODE"] + "】删除成功！");
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (PubFunc.FormDataCheck(FormMx).Length > 0) return;
            //if (tbxCode.Text.Length != ddlSJCODE.SelectedValue.Length + 2 && tbxCode.Text.Length > 2)
            //{
            //    Alert.Show("编码规则不正确，请检查！", "提示信息", MessageBoxIcon.Warning);
            //    return;
            //}
            //增加控制避免主键重复
            if ((DbHelperOra.Exists("select 1 from SYS_DEPT where code = '" + tbxCode.Text + "'")) && (tbxCode.Enabled))
            {
                Alert.Show("你输入的编码已存在,请检查!");
                return;
            }

            if (string.IsNullOrWhiteSpace(ddlSJCODE.SelectedValue))
            {
                Alert.Show("请填选上级！", "提示", MessageBoxIcon.Information);
                return;
            }
            if (!CheckTel(tbxSTR6.Text))
            {
                Alert.Show("联系电话请保证格式（固话XXX-XXX）(手机1XXX)！");
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
            if (tbxCode.Enabled)  //新增
            {
                mtTable.InsertExec();
            }
            else
            {
                mtTable.UpdateExec("");
            }
            //写入日志
            OperLog("管理架构定义", "修改资料【" + tbxCode.Text + "】");
            Alert.Show("数据保存成功！");
            ERPUtility.CacheClear("SYS_DEPT");
            tbxCode.Enabled = false;
            InitTree();
        }
        protected bool CheckTel(string tel)
        {
            bool returnbool = false;
            if (!string.IsNullOrEmpty(tel))
            {
                returnbool = System.Text.RegularExpressions.Regex.IsMatch(tel, @"^[1][0-9]{10}");
                if (!returnbool)
                {
                    returnbool = System.Text.RegularExpressions.Regex.IsMatch(tel, @"^[0,2-9]{1}[0-9]+[-]?[0-9]+?$");
                }
                return returnbool;
            }
            else
            {
                return true;
            }

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
            //Response.ClearContent();
            //Response.AddHeader("content-disposition", "attachment; filename=信息导出.xls");
            //Response.ContentType = "application/excel";
            //Response.Write(PubFunc.GridToHtml(GridMx));
            //Response.End();
            //btnExp.Enabled = true;

            string strCode = ddlSearch.SelectedValue;
            string strSql = "select CODE 编码, NAME 名称, FLAG 状态, TYPE 类别, CLASS 级次, ISLAST 末级, STR1 HIS编码, MEMO 备注 from " + strTable;
            if (strCode != null & strCode != "" & strCode != "0")
            {
                strSql += " start with code='" + strCode + "' connect by prior code = SJCODE";
            }

            strSql += " order by  code";

            DataTable dt = DbHelperOra.Query(strSql).Tables[0];

            ExcelHelper.ExportByWeb(dt, "管理架构定义导出", "管理架构定义导出_" + DateTime.Now.ToString("yyyyMMddHH") + ".xls");

        }

        private int CodeLen(string strLEVELS)
        {//得到编码的长度，如果传入99则取最大长度。
            int li_levels = Convert.ToInt32(strLEVELS);
            int li_len = 0;
            //string[] ls_rule = tbxRule.Text.Split('-');
            //if (li_levels == 99) { li_levels = ls_rule.Length; }
            //for (int i = 0; i < li_levels; i++)
            //{
            //    li_len = li_len + Convert.ToInt32(ls_rule[i]);
            //}

            return li_len;
        }
        private string CodeGet(string strParent, string strLEVELS)
        {
            string strSql = "SELECT MAX(CODE) FROM SYS_DEPT WHERE SJCODE = '" + strParent + "'";
            string strCode = (DbHelperOra.GetSingle(strSql) ?? "").ToString();
            if (strCode.Length > 1)
            {
                String str = "0" + (Convert.ToInt16(strCode.Substring(strCode.Length - 2, 2)) + 1).ToString();
                strCode = strCode.Substring(0, strCode.Length - 2) + (str).Substring(str.Length - 2, 2);
                return strCode;
            }
            else
            {
                return strParent+"01" ;
            }
        }
    }
}
using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPDictionary
{
    public partial class Efficacy : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetEfficacy();
            }
        }

        protected void GridEfficacy_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            hfdIsNew.Text = "N";
            tbxCode.Enabled = false;
            tbxCode.Text = PubFunc.GridDataGet(GridEfficacy, e.RowIndex)["Code"].ToString();
            tbxName.Text = PubFunc.GridDataGet(GridEfficacy, e.RowIndex)["Name"].ToString();
            ckbFlag.Checked = PubFunc.GridDataGet(GridEfficacy, e.RowIndex)["FLAG"].ToString() == "Y" ? true : false;
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormEfficacy);
            hfdIsNew.Text = "Y";
            tbxCode.Enabled = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (PubFunc.FormDataCheck(FormEfficacy).Length > 0) return;
            int out_i = 0;
            if (!int.TryParse(tbxCode.Text, out out_i))
            {
                Alert.Show("请输入正确的药效编码!");
                return;
            }
            if (out_i < 1)
            {
                Alert.Show("请输入正确的药效编码!");
                return;
            }
            MyTable mtType = new MyTable("DOC_GOODSYX");
            mtType.ColRow = PubFunc.FormDataHT(FormEfficacy);
            if (hfdIsNew.Text == "Y" || hfdIsNew.Text == "")
            {
                if (DbHelperOra.Exists(string.Format("select 1 from DOC_GOODSYX where code='{0}'", tbxCode.Text)))
                {
                    Alert.Show(string.Format("您输入的药效编码【{0}】已存在，请重新输入！", tbxCode.Text), MessageBoxIcon.Warning);
                    return;
                }
                mtType.InsertExec();
            }
            else
            {
                if (ckbFlag.Checked)
                {
                    DbHelperOra.ExecuteSql("UPDATE DOC_GOODSYX SET flag='Y',ISDELETE='N',name='" + tbxName.Text.Trim() + "' WHERE CODE='" + tbxCode.Text + "'");
                }
                else
                {
                    DbHelperOra.ExecuteSql("UPDATE DOC_GOODSYX SET flag='Y',ISDELETE='Y',name='" + tbxName.Text.Trim() + "' WHERE CODE='" + tbxCode.Text + "'");
                }
            }
            OperLog("药效资料", "修改药效【" + tbxName.Text.Trim() + "】");
            Alert.Show("数据保存成功！");
            GetEfficacy();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int[] rows = GridEfficacy.SelectedRowIndexArray;
            if (rows.Length > 0)
            {
                string code = "";
                foreach (int index in rows)
                {
                    code += PubFunc.GridDataGet(GridEfficacy, index)["Code"].ToString() + ",";
                }
                // DbHelperOra.ExecuteSql("UPDATE DOC_GOODSYX SET ISDELETE='Y' WHERE CODE IN ('" + code.TrimEnd(',').Replace(",", "','") + "')");
                DbHelperOra.ExecuteSql("delete DOC_GOODSYX WHERE CODE IN ('" + code.TrimEnd(',').Replace(",", "','") + "')");
                OperLog("药效资料", "修改药效【" + code.TrimEnd(',') + "】");
                Alert.Show("药效信息删除成功！");
                GetEfficacy();
            }
            else
            {
                Alert.Show("请选择要删除的药效信息！");
            }
        }

        public void GetEfficacy()
        {
            GridEfficacy.DataSource = DbHelperOra.Query("select CODE,NAME,FLAG from  DOC_GOODSYX WHERE ISDELETE='N'");
            GridEfficacy.DataBind();
        }
    }
}
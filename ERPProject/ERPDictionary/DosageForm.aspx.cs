using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPDictionary
{
    public partial class DosageForm : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetDosage();
            }
        }

        protected void GridDosage_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            hfdIsNew.Text = "N";
            tbxCode.Enabled = false;
            tbxCode.Text = PubFunc.GridDataGet(GridDosage, e.RowIndex)["Code"].ToString();
            tbxName.Text = PubFunc.GridDataGet(GridDosage, e.RowIndex)["Name"].ToString();
            ckbFlag.Checked = PubFunc.GridDataGet(GridDosage, e.RowIndex)["FLAG"].ToString() == "Y" ? true : false;
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormDosage);
            hfdIsNew.Text = "Y";
            tbxCode.Enabled = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (PubFunc.FormDataCheck(FormDosage).Length > 0) return;
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
            MyTable mtType = new MyTable("DOC_GOODSJX");
            mtType.ColRow = PubFunc.FormDataHT(FormDosage);
            if (hfdIsNew.Text == "Y")
            {
                if (DbHelperOra.Exists(string.Format("select 1 from DOC_GOODSJX where code='{0}'", tbxCode.Text)))
                {
                    Alert.Show(string.Format("您输入的剂型编码【{0}】已存在，请重新输入！", tbxCode.Text), MessageBoxIcon.Warning);
                    return;
                }
                mtType.InsertExec();
            }
            else
            {
                if (ckbFlag.Checked)
                {
                    DbHelperOra.ExecuteSql("UPDATE DOC_GOODSJX SET flag='Y',name='" + tbxName.Text.Trim() + "' WHERE CODE='" + tbxCode.Text + "'");
                }
                else
                {
                    DbHelperOra.ExecuteSql("UPDATE DOC_GOODSJX SET flag='N',name='" + tbxName.Text.Trim() + "' WHERE CODE='" + tbxCode.Text + "'");
                }
            }
            OperLog("剂型资料", "修改剂型【" + tbxName.Text.Trim() + "】");
            Alert.Show("数据保存成功！");
            GetDosage();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int[] rows = GridDosage.SelectedRowIndexArray;
            if (rows.Length > 0)
            {
                string code = "";
                foreach (int index in rows)
                {
                    code += PubFunc.GridDataGet(GridDosage, index)["Code"].ToString() + ",";
                }
                //DbHelperOra.ExecuteSql("UPDATE DOC_GOODSJX SET ISDELETE='Y' WHERE CODE IN ('" + code.TrimEnd(',').Replace(",", "','") + "')");
                DbHelperOra.ExecuteSql("delete DOC_GOODSJX WHERE CODE IN ('" + code.TrimEnd(',').Replace(",", "','") + "')");
                OperLog("剂型资料", "删除剂型【" + code.TrimEnd(',') + "】");
                Alert.Show("商品剂型信息删除成功！");
                GetDosage();
            }
            else
            {
                Alert.Show("请选择要删除的商品剂型信息！");
            }
        }

        public void GetDosage()
        {
            GridDosage.DataSource = DbHelperOra.Query("select CODE,NAME,FLAG from  DOC_GOODSJX WHERE ISDELETE='N'");
            GridDosage.DataBind();
        }

    }
}
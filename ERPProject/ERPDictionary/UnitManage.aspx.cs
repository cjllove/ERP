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
    public partial class UnitManage : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();

                //先暂时锁定这些功能
                btnNew.Enabled = false;
                btnDelete.Enabled = false;
                btnSave.Enabled = false;
                ckbFlag.Enabled = false;
                tbxCode.Enabled = false;
                tbxName.Enabled = false;
            }
        }

        private void LoadData()
        {
            GridUnit.DataSource = GetUnit();
            GridUnit.DataBind();
        }

        protected void GridUnit_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            hfdIsNew.Text = "N";
            tbxCode.Enabled = false;
            tbxCode.Text = PubFunc.GridDataGet(GridUnit, e.RowIndex)["Code"].ToString();
            tbxName.Text = PubFunc.GridDataGet(GridUnit, e.RowIndex)["Name"].ToString();
            ckbFlag.Checked = PubFunc.GridDataGet(GridUnit, e.RowIndex)["FLAG"].ToString() == "Y" ? true : false;
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            tbxCode.Enabled = true;
            hfdIsNew.Text = "Y";
            PubFunc.FormDataClear(FormUnit);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (PubFunc.FormDataCheck(FormUnit).Length > 1)
            {
                return; //存在为空的非空列则返回！
            }
            if (tbxCode.Text.Trim() == "" || tbxName.Text.Trim() == "")
            {
                Alert.Show("请维护单位编码和单位名称!");
                return;
            }
            if (tbxCode.Text.Length > 15 || tbxName.Text.Length > 30)
            {
                Alert.Show("单位编码或单位名称超长!");
                return;
            }
            try
            {
                int k = Convert.ToInt16(tbxCode.Text);
                if (k < 0)
                {
                    Alert.Show("单位编码只能输入大于0的数字!");
                    return;
                }
            }
            catch
            {
                Alert.Show("单位编码只能输入数字!");
                return;
            }
            if (tbxCode.Enabled != false)
            {
                if (DbHelperOra.Exists(string.Format("select 1 from DOC_GOODSUNIT where CODE='{0}'", tbxCode.Text.Trim())))
                {
                    Alert.Show(string.Format("您输入的单位编码【{0}】已存在，请重新输入！", tbxCode.Text), MessageBoxIcon.Warning);
                    return;
                }
            }

            MyTable mtType = new MyTable("DOC_GOODSUNIT");
            mtType.ColRow = PubFunc.FormDataHT(FormUnit);
            if (hfdIsNew.Text == "Y")
            {
                mtType.InsertExec();
            }
            else
            {
                if (ckbFlag.Checked)
                {
                    DbHelperOra.ExecuteSql("update DOC_GOODSUNIT set name = '" + tbxName.Text.Trim() + "',flag='Y' where code = '" + tbxCode.Text.Trim() + "'");
                }
                else
                {
                    DbHelperOra.ExecuteSql("update DOC_GOODSUNIT set name = '" + tbxName.Text.Trim() + "',flag='N' where code ='" + tbxCode.Text.Trim() + "'");
                }
                OperLog("单位资料", "修改单位【" + tbxName.Text.Trim() + "】");
            }

            Alert.Show("数据保存成功！");
            LoadData();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int[] rows = GridUnit.SelectedRowIndexArray;
            string code = "";
            foreach (int index in rows)
            {
                code += PubFunc.GridDataGet(GridUnit, index)["Code"].ToString() + ",";
            }
            if (code.Trim() == "")
            {
                Alert.Show("请选择需要删除的信息");
                return;
            }

            // DbHelperOra.ExecuteSql("UPDATE DOC_GOODSUNIT SET ISDELETE='Y' WHERE CODE IN ('" + code.TrimEnd(',').Replace(",", "','") + "')");
            DbHelperOra.ExecuteSql("DELETE DOC_GOODSUNIT WHERE CODE IN ('" + code.TrimEnd(',').Replace(",", "','") + "')");
            OperLog("单位资料", "删除单位【" + code.TrimEnd(',') + "】");
            Alert.Show("单位信息删除成功！");
            LoadData();
        }

        public DataTable GetUnit(NameValueCollection nvc = null)
        {
            string strSearch = "";
            if (nvc != null)
            {
                foreach (string key in nvc)
                {
                    string condition = nvc[key];
                    if (!string.IsNullOrEmpty(condition))
                    {
                        switch (key.ToUpper())
                        {
                            case "CODE":
                                strSearch += string.Format(" AND CODE='{0}'", condition);
                                break;
                            case "NAME":
                                strSearch += string.Format(" AND NAME LIKE '%{0}%'", condition);
                                break;
                            case "FLAG":
                                strSearch += string.Format(" AND FLAG='{0}'", condition);
                                break;
                            case "STARTCODE":
                                strSearch += string.Format(" AND CODE >= '{0}'", condition);
                                break;
                            case "ENDCODE":
                                strSearch += string.Format(" AND CODE <= '{0}'", condition);
                                break;
                        }
                    }
                }
            }
            string strSql = @"select CODE,NAME,FLAG from  DOC_GOODSUNIT WHERE ISDELETE='N'";
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            return DbHelperOra.Query(strSql).Tables[0];
        }

    }
}
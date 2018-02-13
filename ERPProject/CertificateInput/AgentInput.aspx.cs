using FineUIPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using XTBase;
using System.Text.RegularExpressions;
using XTBase.Utilities;

namespace SPDProject.CertificateInput
{
    public partial class AgentInput : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                dataSearch();
            }
        }

        private void dataSearch()
        {
            string query = "%";
            if (!string.IsNullOrWhiteSpace(tbxSUPPNAME.Text))
            {
                query = tbxSUPPNAME.Text.Trim();
            }
            int total = 0;
            string sql = string.Format(@"select SUPID,SUPNAME,DECODE(FLAG, 'Y', '审核通过', 'N', '未审核') flag,DECODE(ISAGENT,'Y','代理商') ISAGENT,TEL,FAX,ZZADDR,EMAIL,LOGINADDR,URL,LEADER,DECODE(ISAGENT, 'Y', '配送商', '') ISAGENTN
                                          from DOC_SUPPLIER WHERE (SUPID like '%{0}%' or SUPNAME like '%{0}%') AND ISAGENT='Y'", query);
            if (!string.IsNullOrWhiteSpace(ddlFLAG.SelectedValue))
            {
                sql += string.Format(" and FLAG = '{0}'", ddlFLAG.SelectedValue);
            }
            DataTable dtData = PubFunc.DbGetPage(GridSupplier.PageIndex, GridSupplier.PageSize, sql, ref total);
            GridSupplier.RecordCount = total;
            GridSupplier.DataSource = dtData;
            GridSupplier.DataBind();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            hfdIsNew.Text = "Y";
            PubFunc.FormDataClear(FormProducer);
            btnSave.Enabled = true;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int[] rows = GridSupplier.SelectedRowIndexArray;
            if (rows.Length > 0)
            {
                string code = "";
                foreach (int index in rows)
                {
                    code += PubFunc.GridDataGet(GridSupplier, index)["SUPID"].ToString() + ",";
                }
                code = code.TrimEnd(',').Replace(",", "','");
                DbHelperOra.ExecuteSql("DELETE FROM DOC_SUPPLIER WHERE SUPID IN ('" + code + "')");
                Alert.Show("供应商【" + code + "】删除成功！");
                dataSearch();
            }
            else
            {
                Alert.Show("请选择要删除的供应商！");
            }
        }

        private void saveExec()
        {
            try
            {
                if (PubFunc.FormDataCheck(FormProducer).Length > 0) return;//存在为空的非空列则返回！
                //增加自动生成的编码
                if (hfdIsNew.Text == "Y" && tbxSUPID.Text.Trim().Length < 1)
                {
                    DataTable dt = DbHelperOra.QueryForTable("SELECT F_GET_SUPCODE FROM DUAL");
                    if (dt.Rows.Count > 0)
                    {
                        tbxSUPID.Text = dt.Rows[0][0].ToString();
                    }
                    if (string.IsNullOrEmpty(tbxSUPID.Text))//当导入供应商时，SUPID再次新增时，超过50次尝试后无法自动生成
                    {
                        string getstring = DbHelperOra.GetSingle("SELECT SUPID FROM(SELECT SUPID FROM DOC_SUPPLIER ORDER BY SUPID DESC) WHERE ROWNUM=1  ").ToString();
                        int totalcount = 0;
                        int zerocount = 0;
                        string newstring = "";
                        MatchCollection ms = Regex.Matches(getstring, @"\d+");
                        foreach (Match m in ms)
                        {
                            if (!string.IsNullOrEmpty(m.Value))
                            {
                                totalcount = m.Value.Length;//总位数
                                zerocount = totalcount - Int32.Parse(m.Value).ToString().Length;//0的位数
                                newstring = (Int32.Parse(m.Value) + 1).ToString();
                                tbxSUPID.Text = getstring.Replace(m.Value, "") + m.Value.Substring(0, zerocount) + newstring;
                            }
                        }
                    }
                }
                MyTable mtType = new MyTable("DOC_SUPPLIER");

                if ((DbHelperOra.Exists("select 1 from DOC_SUPPLIER where SUPID = '" + tbxSUPID.Text + "'")) && (tbxSUPID.Enabled))
                {
                    Alert.Show("此供应商编码已经存在,请重新输入!", "提示信息", MessageBoxIcon.Warning);
                    return;
                }
                mtType.ColRow = PubFunc.FormDataHT(FormProducer);
                mtType.ColRow.Add("SUPCAT", "03");
                mtType.ColRow.Add("ISAGENT", 'Y');
                mtType.ColRow.Add("FLAG", 'N');

                if (hfdIsNew.Text == "" || hfdIsNew.Text == "Y")
                {
                    mtType.InsertExec();
                    Alert.Show("数据保存成功！");
                    btnSave.Enabled = false;
                }
                else
                {
                    mtType.UpdateExec("");
                    Alert.Show("数据更新成功！");
                }
               
            }
            catch (Exception)
            {
                Alert.Show("请确认您的供应商名称是否重复了！");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            saveExec();
            PubFunc.FormDataClear(FormProducer);
            btnNew_Click(null, null);
            dataSearch();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }

        protected void GridSupplier_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridSupplier.PageIndex = e.NewPageIndex;
            dataSearch();
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {
            int[] rows = GridSupplier.SelectedRowIndexArray;
            if (rows.Length > 0)
            {
                string code = "";
                foreach (int index in rows)
                {
                    code += PubFunc.GridDataGet(GridSupplier, index)["SUPID"].ToString() + ",";
                }
                code = code.TrimEnd(',').Replace(",", "','");
                DbHelperOra.ExecuteSql("UPDATE DOC_SUPPLIER SET FLAG='Y' WHERE SUPID IN ('" + code + "')");
                Alert.Show("代理商【" + code + "】审核成功！");
                dataSearch();
            }
            else
            {
                Alert.Show("请选择要审核的代理商！");
            }
        }

        protected void GridSupplier_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            string strCode = GridSupplier.DataKeys[e.RowIndex][0].ToString();
            hfdsavecount.Text = strCode;
            cxsj(strCode);
        }

        protected void cxsj(String strCode)
        {
            string strSql = string.Format("select * from DOC_SUPPLIER where SUPID='{0}'", strCode);

            DataTable dtProducer = DbHelperOra.Query(strSql).Tables[0];
            if (dtProducer.Rows.Count > 0)
            {

                PubFunc.FormDataSet(FormProducer, dtProducer.Rows[0]);
            }

            if (ddlFLAG.SelectedValue == "Y")
            {
                PubFunc.FormLock(FormProducer, true);
                btnDelete.Enabled = false;
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                
            }
            else
            {
                PubFunc.FormLock(FormProducer, false);
                btnAudit.Enabled = true;
                btnDelete.Enabled = true;
                tbxSUPID.Enabled = false;
                btnSave.Enabled = true;
                hfdIsNew.Text = "N";
            }
        }
    }
}
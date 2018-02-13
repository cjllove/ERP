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

namespace ERPProject.ERPDictionary
{
    public partial class Suppliers : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //PubFunc.DdlDataGet("DDL_DOC_FIRMNATURE",docCORPKID);
                PubFunc.DdlDataSql(docCORPKID,@"select  CODE,NAME from (
SELECT '0' CODE,'--请选择--' NAME  FROM dual
union all
SELECT code,NAME FROM SYS_CODEVALUE where type='DOC_FIRMNATURE'
)
ORDER BY NAME,code  ");
                PubFunc.DdlDataSql(lstFLAG, @"SELECT '' CODE ,'--请选择--' NAME  FROM dual
                                                union all
                                                SELECT 'N' CODE ,'未审核' NAME  FROM dual
                                                union all
                                                SELECT 'Y' CODE ,'审核通过' NAME  FROM dual
                                                ");
                dataSearch();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }
        private void dataSearch()
        {
            string query = "%";
            if (!string.IsNullOrWhiteSpace(tgbSearch.Text))
            {
                query = tgbSearch.Text.Trim();
            }
            int total = 0;
            string sql = string.Format(@"select SUPID,
       SUPNAME,
       TEL,
       LINKMAN,
       LOGINADDR,
       DECODE(FLAG, 'Y', '审核通过', 'N', '未审核') flag,
       DECODE(ISDG, 'Y', '是', 'N', '否') ISDG,
       DECODE(STR1, 'Y', '是', '否') ISBD,
       DECODE(ISSUPPLIER, 'Y', '供应商', '')|| DECODE(ISPRODUCER, 'Y', '生产商', '')|| DECODE(ISPSS, 'Y', '配送商', '')  SUPNAMETYPE
       
  from DOC_SUPPLIER
 WHERE (SUPID like '%{0}%' or SUPNAME like '%{0}%')", query);
            if (!string.IsNullOrWhiteSpace(lstFLAG.SelectedValue))
            {
                sql += string.Format(" and FLAG = '{0}'", lstFLAG.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(lstSTR1.SelectedValue))
            {
                sql += string.Format(" and str1 = '{0}'", lstSTR1.SelectedValue);
            }
            if (!string.IsNullOrEmpty(lstSuppType.SelectedValue))
            {
                if (lstSuppType.SelectedValue.Equals("X"))
                {//供应商
                    sql += " AND ISSUPPLIER = 'Y' ";
                }
                else if (lstSuppType.SelectedValue.Equals("Y"))
                {//配送商
                    sql += " AND ISPSS = 'Y'";
                }
                else if (lstSuppType.SelectedValue.Equals("Z"))
                {//生产商
                    sql += " AND ISPRODUCER ='Y' ";
                }
            }
            DataTable dtData = PubFunc.DbGetPage(GridSupplier.PageIndex, GridSupplier.PageSize, sql, ref total);
            GridSupplier.RecordCount = total;
            GridSupplier.DataSource = dtData;
            GridSupplier.DataBind();
        }

        protected void GridSupplier_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
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
                tbxSUPPWD.Text = "";
                if (dtProducer.Rows[0]["STR1"].ToString() == "Y")
                {
                    chkSTR1.Checked=true;
                }
                else
                {
                    chkSTR1.Checked = false;
                }
            }

            if (ddlFLAG.SelectedValue == "Y")
            {
                PubFunc.FormLock(FormProducer, true);
                btnDelete.Enabled = false;
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnUnAudit.Enabled = true;
            }
            else
            {
                PubFunc.FormLock(FormProducer, false);
                btnUnAudit.Enabled = false;
                btnAudit.Enabled = true;
                btnDelete.Enabled = true;
                tbxSUPID.Enabled = false;
                btnSave.Enabled = true;
                hfdIsNew.Text = "N";
            }
            if (strCode == "00001" || strCode == "00002")
            {
                chkISDG.Enabled = false;
            }
            else
            {
                chkISDG.Enabled = true;
            }
            ddlFLAG.Enabled = false;
            chkSTR1.Enabled = false;
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            PubFunc.FormDataClear(FormProducer);
            hfdIsNew.Text = "Y";
            tbxSUPID.Enabled = true;
            ddlFLAG.SelectedValue = "N";
            PubFunc.FormLock(FormProducer, false);
            ddlFLAG.Enabled = false;
            ddlFLAG.SelectedValue = "N";
            chkSTR1.Checked = true;
            chkISDG.Checked = true;
            chkSTR1.Enabled = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            saveExec();            
            PubFunc.FormDataClear(FormProducer);
            btnNew_Click(null, null);
            dataSearch();
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
                        string getstring=DbHelperOra.GetSingle("SELECT SUPID FROM(SELECT SUPID FROM DOC_SUPPLIER ORDER BY SUPID DESC) WHERE ROWNUM=1  ").ToString();
                        int totalcount = 0;
                        int zerocount = 0;
                        string newstring = "";
                        MatchCollection ms = Regex.Matches(getstring, @"\d+");
                        foreach (Match m in ms)
                        {
                            if(!string.IsNullOrEmpty(m.Value))
                            {
                                totalcount = m.Value.Length;//总位数
                                zerocount = totalcount - Int32.Parse(m.Value).ToString().Length;//0的位数
                                newstring=(Int32.Parse(m.Value) + 1).ToString();
                                tbxSUPID.Text =getstring.Replace(m.Value,"")+m.Value.Substring(0,zerocount)+ newstring;
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
                if (string.IsNullOrEmpty(hfdsavecount.Text))
                {
                    if (DbHelperOra.Exists("select 1 from DOC_SUPPLIER where SUPNAME = '" +  tbxSUPNAME.Text + "'"))
                    {
                        Alert.Show("此供应商名称已经存在,请重新输入!", "提示信息", MessageBoxIcon.Warning);
                        return;
                    }
                }
                if (!chkISDG.Checked && chkSTR1.Checked)
                {
                    Alert.Show("本地供应商必须为代管供应商！", "提示信息", MessageBoxIcon.Warning);
                    chkISDG.Checked = true;
                    return;
                }
                if (chkISSUPPLIER.Checked && chkISPSS.Checked)
                {
                    Alert.Show("【供应商】标记与【配送商】标记不能同时维护！", "提示信息", MessageBoxIcon.Warning);
                    return;
                }
                mtType.ColRow = PubFunc.FormDataHT(FormProducer);
                //mtType.ColRow.Add("FLAG", "N");
                mtType.ColRow.Add("SUPCAT", "03");

                if (hfdIsNew.Text == "" || hfdIsNew.Text == "Y")
                {
                    mtType.ColRow["ISSEND"] = chkSTR1.Checked?"Y":"N";
                    mtType.InsertExec();
                    Alert.Show("数据保存成功！");
                    btnSave.Enabled = false;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(tbxSUPPWD.Text.Trim()))
                    {
                        mtType.ColRow.Remove("SUPPWD");
                    }
                    else
                    {
                        mtType.ColRow["SUPPWD"] = PasswordUtil.CreateDbPassword(tbxSUPPWD.Text.Trim());
                    }
                    mtType.UpdateExec("");
                    Alert.Show("数据更新成功！");
                }
                OperLog("供应商资料", "修改供应商【" + tbxSUPID.Text + "】");
            }
            catch (Exception)
            {
                Alert.Show("请确认您的供应商名称是否重复了！");
            }
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
        protected void Grid1_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridSupplier.PageIndex = e.NewPageIndex;
            dataSearch();
        }

        protected void btnAudit_Click(object sender, EventArgs e)
        {

            int[] rows = GridSupplier.SelectedRowIndexArray;
            if (rows.Length > 0 && !string.IsNullOrEmpty(tbxSUPID.Text))
            {
                if (PubFunc.FormDataCheck(FormProducer).Length > 1)
                {

                    return; //存在为空的非空列则返回！
                }
                else
                {
                    saveExec();
                    string strFLAG = ddlFLAG.SelectedValue.ToString();
                    if (strFLAG == "Y")
                    {
                        Alert.Show("已经审核通过，无需再审！");
                    }
                    else
                    {
                        string strSUPID = "";
                        foreach (int index in rows)
                        {

                            strSUPID += PubFunc.GridDataGet(GridSupplier, index)["SUPID"].ToString() + ",";
                        }
                        strSUPID = strSUPID.TrimEnd(',').Replace(",", "','");
                        DbHelperOra.ExecuteSql("UPDATE DOC_SUPPLIER SET FLAG = 'Y' WHERE SUPID IN ('" + strSUPID + "')");
                        Alert.Show("供应商资料 审核通过！");
                        OperLog("供应商资料", "审核供应商【" + strSUPID + "】");
                        ERPUtility.CacheClear("DOC_SUPPLIER");

                    }

                }

            }
            else
            {
                Alert.Show("请选择需要审核的供应商！");
                return;
            }
            String strSUPID1 = tbxSUPID.Text.ToString();
            cxsj(strSUPID1);
            PubFunc.FormLock(FormProducer, true);
            dataSearch();
        }

        protected void tbxSUPNAME_TextChanged(object sender, EventArgs e)
        {
            tbxSTR2.Text = PinYinUtil.GetCodstring(tbxSUPNAME.Text);
        }

        protected void btnUnAudit_Click(object sender, EventArgs e)
        {
            if (ddlFLAG.SelectedValue == "N")
            {
                Alert.Show("【未审核】供应商，无法进行【反审核】操作");
                return;
            }
            else
            {
                String strSUPID = tbxSUPID.Text.ToString();
                if (string.IsNullOrEmpty(tbxSUPID.Text))
                {
                    Alert.Show("未选择供应商，无法进行【反审核】操作");
                    return;
                }
                if (DbHelperOra.Exists("select 1 from DAT_GOODSSTOCK t where pssid = '" + strSUPID + "' and t.kcsl > 0"))
                {
                    Alert.Show("当前供应商下还存在有库存的商品，无法执行【反审核】操作。");
                    return;
                }
                if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_DD_DOC WHERE PSSID = '{0}' AND (FLAG ='N' OR (isend = 'N' AND FLAG = 'Y'))", strSUPID)))
                {
                    Alert.Show("当前供应商下还存在有在途中的商品，无法执行【反审核】操作。");
                    return;
                }
                DbHelperOra.ExecuteSql("UPDATE DOC_SUPPLIER SET FLAG = 'N' WHERE SUPID IN ('" + strSUPID + "')");
                Alert.Show("【反审核】成功");
                OperLog("供应商资料", "反审核供应商【" + strSUPID + "】");
                ERPUtility.CacheClear("DOC_SUPPLIER");
                cxsj(strSUPID);
                dataSearch();
            }

        }

        protected void btExp_Click(object sender, EventArgs e)
        {
            if (GridSupplier.Rows.Count < 1)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }

            string query = "%";
            if (!string.IsNullOrWhiteSpace(tgbSearch.Text))
            {
                query = tgbSearch.Text.Trim();
            }
            string sql = string.Format(@"select SUPID 供应商编码,
                                       SUPNAME 供应商名称,
                                       TEL 公司电话,
                                       LINKMAN 联系人,
                                       LOGINADDR 注册地址,
                                       DECODE(FLAG, 'Y', '审核通过', 'N', '未审核') 状态,
                                       DECODE(ISDG, 'Y', '代管', 'N', '不代管') 是否代管
                                  from DOC_SUPPLIER
                                 WHERE  (SUPID like '%{0}%' or SUPNAME like '%{0}%')", query);
            if (!string.IsNullOrWhiteSpace(lstFLAG.SelectedValue))
            {
                sql += string.Format(" and FLAG = '{0}'", lstFLAG.SelectedValue);
            }
         
            if (!string.IsNullOrWhiteSpace(lstSTR1.SelectedValue))
            {
                sql += string.Format(" and STR1 = '{0}'", lstSTR1.SelectedValue);
            }
            DataTable dt = DbHelperOra.Query(sql).Tables[0];

            ExcelHelper.ExportByWeb(dt, "供应商资料导出", "供应商资料导出_" + DateTime.Now.ToString("yyyyMMddHH") + ".xls");
        }

        protected void chkSTR1_CheckedChanged(object sender, CheckedEventArgs e)
        {
            if (chkSTR1.Checked)
                chkISDG.Checked = true;
        }
       
    }
}
using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.IO;
using XTBase;
using XTBase.Utilities;
using System.Text.RegularExpressions;

namespace ERPProject.ERPApply
{
    public partial class DepartmentApplyAuto : BillBase
    {
        public override Field[] LockControl
        {
            get { return new Field[] { }; }
        }

        public DepartmentApplyAuto()
        {
            BillType = "LYD";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string path = Request.RawUrl;
            if (!IsPostBack)
            {
                DataInit();
                billNew();
            }
        }

        private void DataInit()
        {
            PubFunc.DdlDataGet("DDL_SYS_DEPOT", ddlDEPTIN);
            //PubFunc.DdlDataGet("DDL_SYS_DEPTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDEPTIN);
            dpkLRRQ1.SelectedDate = DateTime.Now.AddDays(-7);
            dpkLRRQ2.SelectedDate = DateTime.Now.AddDays(-1);
        }
        protected Boolean OPER()
        {
            if (ddlDEPTIN.SelectedValue.Length < 1) return true;
            if (DbHelperOra.Exists("SELECT 1 FROM SYS_DEPT WHERE ISORDER = 'Y' AND CODE = '" + ddlDEPTIN.SelectedValue + "'"))
            {
                return true;
            }
            return false;
        }
        private bool ISDecimal(string GDSEQ)
        {
            object objISFLAG5 = DbHelperOra.GetSingle(string.Format("SELECT ISFLAG5 FROM DOC_GOODS WHERE GDSEQ = '{0}'", GDSEQ));
            if (objISFLAG5 != null)
            {
                if ( objISFLAG5.ToString() == "N")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else return false;
        }
        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            dpkLRRQ1.SelectedDate = DateTime.Now.AddDays(-7);
            dpkLRRQ2.SelectedDate = DateTime.Now.AddDays(-1);
        }
        protected override void billSearch()
        {
            if (dpkLRRQ1.SelectedDate == null || dpkLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【起始日期】！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            else if (dpkLRRQ1.SelectedDate > dpkLRRQ2.SelectedDate)
            {
                Alert.Show("【起始日期】大于【终止日期】，请重新输入！", "提示信息", MessageBoxIcon.Warning);
                return;
            }

            string strSql = @"SELECT A.SEQ,
                                       A.GDSEQ,
                                       A.FLAG,
                                       F_GETDEPTNAME(A.DEPTSY) DEPTID,
                                       A.GDNAME,
                                       A.GDSPEC,
                                       A.SL XHSL,
                                       A.SL SLSL,
                                       A.HSJJ,
                                       A.PRODUCER,
F_GETSUPNAME(A.PRODUCER) PRODUCER_CN,
                                       A.DEPTSY,A.DJRQ
                                  from INF_PH_USE A,DOC_GOODS B
                                 WHERE A.FLAG = 'N' AND A.GDSEQ=B.GDSEQ(+) ";
            string strSearch = "";
            if (ddlDEPTIN.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTSY = '{0}'", ddlDEPTIN.Text);
            }
            if (ddlISGZ.SelectedItem != null && ddlISGZ.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND B.ISGZ = '{0}'", ddlISGZ.SelectedItem.Value);
            }
            if (!string.IsNullOrWhiteSpace(tbxGDSEQ.Text))
            {
                strSearch += string.Format(" AND (UPPER(B.GDSEQ) LIKE '%{0}%' OR UPPER(B.GDNAME) LIKE '%{0}%' OR UPPER(B.ZJM) LIKE '%{0}%')", tbxGDSEQ.Text.Trim().ToUpper());
            }

            strSearch += string.Format(" AND A.DEPTSY in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.GETTIME>=TO_DATE('{0}','YYYY-MM-DD')", dpkLRRQ1.Text);
            strSearch += string.Format(" AND A.GETTIME <TO_DATE('{0}','YYYY-MM-DD') + 1", dpkLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }

            strSql += "order by " + GridGoods.SortField + " " + GridGoods.SortDirection;
            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, strSql, ref total);
            GridGoods.DataSource = dt;
            GridGoods.RecordCount = total;
            GridGoods.DataBind();
        }
        private bool DataSave(string stats_flag)
        {
            #region 数据有效性验证
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
            if (newDict.Count == 0)
            {
                Alert.Show("请输入药品信息", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()) && !string.IsNullOrWhiteSpace(newDict[i]["GDNAME"].ToString()))
                {
                    bool strISFLAG5 = ISDecimal(newDict[i]["GDSEQ"].ToString());
                    if (strISFLAG5)
                    {
                        string str = Convert.ToString(Convert.ToDecimal(newDict[i]["BZSL"] ?? "0"));
                        if (Convert.ToDecimal(newDict[i]["BZSL"]) != (int)Convert.ToDecimal(newDict[i]["BZSL"]))
                        {
                            Alert.Show("第【" + (i + 1) + "】行【" + newDict[i]["GDNAME"] + "】药品不支持申领数为小数，请调整", "消息提示", MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                    if (!DbHelperOra.Exists(string.Format("SELECT 1 FROM DOC_GOODSCFG WHERE GDSEQ='{0}' AND DEPTID='{1}'", newDict[i]["GDSEQ"].ToString(), newDict[i]["DEPTSY"].ToString())))
                    {
                        Alert.Show("第【" + (i + 1) + "】行【" + newDict[i]["GDNAME"] + "】药品没有配置到科室!", "消息提示", MessageBoxIcon.Warning);
                        return false;

                    }
                    decimal dec = Convert.ToDecimal(string.IsNullOrWhiteSpace(newDict[i]["BZSL"].ToString()) ? "0" : newDict[i]["BZSL"].ToString());
                    if (!string.IsNullOrWhiteSpace(newDict[i]["BZSL"].ToString()) && dec > 0 && dec < 10000)
                    {
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(newDict[i]["BZSL"].ToString()))
                        {
                            Alert.Show(string.Format("第【{0}】行药品【{1}】的申领数为空", i + 1, newDict[i]["GDNAME"].ToString()), "消息提示", MessageBoxIcon.Warning);
                            return false;
                        }
                        if (dec <= 0)
                        {
                            Alert.Show(string.Format("第【{0}】行药品【{1}】的申领数小于等于0", i + 1, newDict[i]["GDNAME"].ToString()), "消息提示", MessageBoxIcon.Warning);
                            return false;
                        }
                        if (dec >= 10000)
                        {
                            Alert.Show(string.Format("第【{0}】行药品【{1}】的申领数大于等于10000", i + 1, newDict[i]["GDNAME"].ToString()), "消息提示", MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
            }

            if (goodsData.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("药品信息不能为空", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            else return true;
            #endregion
        }
        private static DataTable newDt = new DataTable();

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;
            DataTable table = PubFunc.GridDataGet(GridGoods);
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            GridGoods.DataSource = view1;
            GridGoods.DataBind();
        }
        protected void GridGoods_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            billSearch();
        }

        protected void comGDSEQ_Click(object sender, EventArgs e)
        {
            Alert.Show(e.ToString());
        }
        protected void btnAut_Click(object sender, EventArgs e)
        {
            if (GridGoods.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择需要生成申领单的商品!", MessageBoxIcon.Question);
                return;
            }
            int[] index = GridGoods.SelectedRowIndexArray;
            if (index.Count() < 1)
            {
                Alert.Show("请选择需要生成申领单的商品!", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            string strBill = "";
            string strSEQ = "";
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            List<CommandInfo> cmdList = new List<CommandInfo>();
            foreach (int i in index)
            {
                System.Web.UI.WebControls.TextBox tbxNumber = (System.Web.UI.WebControls.TextBox)GridGoods.Rows[i].FindControl("tbxNum");
                //if (!string.IsNullOrWhiteSpace(GridGoods.DataKeys[i][0].ToString()))
                //{
                //    strBill = strBill + "'" + GridGoods.DataKeys[i][0] + "',";
                //}
                Regex regex = new Regex(@"^[0-9]*$");
                Match maa = regex.Match(tbxNumber.Text);
                if (!maa.Success)
                {
                    Alert.Show("【申领数量】请输入数字类型！");
                    return;
                }
                if (!string.IsNullOrWhiteSpace(GridGoods.DataKeys[i][3].ToString()) && !string.IsNullOrWhiteSpace(GridGoods.DataKeys[i][4].ToString()))
                {
                    bool strISFLAG5 = ISDecimal(GridGoods.DataKeys[i][3].ToString());
                    if (strISFLAG5)
                    {
                        string str = Convert.ToString(Convert.ToDecimal(tbxNumber.Text ?? "0"));
                        if (Convert.ToDecimal(tbxNumber.Text) != (int)Convert.ToDecimal(tbxNumber.Text))
                        {
                            Alert.Show("第【" + (i + 1) + "】行【" + GridGoods.DataKeys[i][4] + "】商品不支持申领数为小数，请调整", "消息提示", MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    decimal dec = Convert.ToDecimal(string.IsNullOrWhiteSpace(tbxNumber.Text) ? "0" : tbxNumber.Text);
                    if (!string.IsNullOrWhiteSpace(tbxNumber.Text) && dec > 0 && dec < 1000000)
                    {
                        strBill = strBill + GridGoods.DataKeys[i][0] + ",";
                        cmdList.Add(new CommandInfo("UPDATE INF_PH_USE SET SLSL ='" + tbxNumber.Text + "' WHERE SEQ='" + GridGoods.DataKeys[i][0] + "'", null));
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(GridGoods.DataKeys[i][2].ToString()))
                        {
                            Alert.Show(string.Format("第【{0}】行商品【{1}】的申领数为空", i + 1, GridGoods.DataKeys[i][4].ToString()), "消息提示", MessageBoxIcon.Warning);
                            return;
                        }
                        if (dec < 0)
                        {
                            Alert.Show(string.Format("第【{0}】行商品【{1}】的申领数小于等于0", i + 1, GridGoods.DataKeys[i][4].ToString()), "消息提示", MessageBoxIcon.Warning);
                            return;
                        }
                        if (dec >= 1000000)
                        {
                            Alert.Show(string.Format("第【{0}】行商品【{1}】的申领数大于等于1000000", i + 1, GridGoods.DataKeys[i][4].ToString()), "消息提示", MessageBoxIcon.Warning);
                            return;
                        }
                    }
                }
            }
            DbHelperOra.ExecuteSqlTran(cmdList);
            if (!string.IsNullOrWhiteSpace(strBill.TrimEnd(',')))
            {
                string slr = UserAction.UserID;
                OracleParameter[] parameters ={
                                            new OracleParameter("VIN_BILLNO" ,OracleDbType.Varchar2,2000),
                                            new OracleParameter("VIN_OPERUSER" ,OracleDbType.Varchar2,20),
                                            new OracleParameter("VO_BILLNUM",OracleDbType.Varchar2,2000)
                                           };
                parameters[0].Value = strBill.TrimEnd(',').Replace("'", "");
                parameters[1].Value = UserAction.UserID;

                parameters[0].Direction = ParameterDirection.Input;
                parameters[1].Direction = ParameterDirection.Input;
                parameters[2].Direction = ParameterDirection.Output;
                try
                {
                    DbHelperOra.RunProcedure("P_LCD", parameters);
                    if (parameters[2].Value.ToString() == "F" || parameters[2].Value.ToString() == "null")
                    {
                        Alert.Show("生成申领单失败！", "消息提示", MessageBoxIcon.Information);
                    }
                    else
                    {
                        //List<CommandInfo> cmdList = new List<CommandInfo>();
                        Alert.Show("成功生成申领单【" + parameters[2].Value.ToString().TrimEnd(',') + "】", "消息提示", MessageBoxIcon.Information);
                        billSearch();
                    }
                }
                catch (Exception ex)
                {
                    Alert.Show(Error_Parse(ex.Message));
                    return;
                }
            }
        }
        public static string Error_Parse(string error)
        {
            string value = string.Empty;
            if (error.IndexOf("ORA-") > -1)
            {
                value = error.Replace("\n", "").Substring(error.IndexOf("ORA-") + 10);
                if (value.IndexOf("ORA-") > -1)
                {
                    value = value.Substring(0, value.IndexOf("ORA-"));
                }
            }
            else
            {
                value = error;
            }

            return value;
        }
   }
}
﻿using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using XTBase.Utilities;

namespace ERPProject.ERPEntrust
{
    public partial class SupplierEntrust : PageBase
    {
        private static DataTable dtTotal = new DataTable();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }
        }

        private void DataInit()
        {
            dtTotal.Rows.Clear();
        }

        private DataTable getDGSupplier()
        {
            DataTable dt = new DataTable();
            DateTime newDate = new DateTime();
            try
            {
                String tgbSearchText = "";
                if (!String.IsNullOrWhiteSpace(tgbSearch.Text))
                {
                    tgbSearchText = tgbSearch.Text;
                }
                JObject result = ApiClientService.query("DOC_SUPPLIER_DG", newDate.ToString("yyyy/MM/dd HH:mm:ss"), tgbSearchText);
                if ("success".Equals(result.Value<String>("result")))
                {
                    JArray ja = result.Value<JArray>("data");
                    String serJa = JsonConvert.SerializeObject(ja);
                    dt = JsonConvert.DeserializeObject<DataTable>(serJa);
                    hfdGridSupplier.Text = serJa;
                }
                else
                {
                    String reason = result.Value<String>("reason");
                    Exception ex = new Exception(reason);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Alert.Show(ex.Message, "获取供应商信息时报错", MessageBoxIcon.Error);
            }
            return dt;
        }

        private void dataSearch(string type = "left")
        {
            int total = 0;
            string msg = "";
            NameValueCollection nvc = new NameValueCollection();
            if (type == "left")
            {
                //if (ddlCATID.SelectedValue.Length > 0) nvc.Add("CATID", ddlCATID.SelectedValue);
                //if (tgbSearch.Text.Length > 0) nvc.Add("CX", tgbSearch.Text);
                //if (ddlDept.SelectedValue.Length > 0) nvc.Add("LEFT", ddlDept.SelectedValue);

                //GridSupplier.DataSource = GetSupplierList(GridSupplier.PageIndex, GridSupplier.PageSize, nvc, ref total, ref msg);
                //GridSupplier.DataBind();
                //GridSupplier.RecordCount = total;
                dtTotal = getDGSupplier();
                DataTable dt = SplitDataTable(dtTotal, GridSupplier.PageIndex, GridSupplier.PageSize);
                GridSupplier.DataSource = dt;
                GridSupplier.DataBind();
                GridSupplier.RecordCount = dtTotal.Rows.Count;
                hfdGridSupDisabled.Text = "";

                DataTable dtLocal = DbHelperOra.Query("select * from doc_supplier").Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (DataRow drLocal in dtLocal.Rows)
                    {
                        if (dr["SUPID"].ToString().Equals(drLocal["SUPID"].ToString()))
                        {
                            int index = dt.Rows.IndexOf(dr);
                            hfdGridSupDisabled.Text += index.ToString() + ",";
                            break;
                        }
                    }
                }
                //PageContext.RegisterStartupScript("setDisableClass();");

            }
            else if (type == "right")
            {
                if (trbSearch.Text.Length > 0) nvc.Add("CX", trbSearch.Text);
                //if (ddlDept.SelectedValue.Length > 0) nvc.Add("RIGHT", ddlDept.SelectedValue);
                DataTable dt = GetSupplierList(GridCFGSupplier.PageIndex, GridCFGSupplier.PageSize, nvc, ref total, ref msg);
                GridCFGSupplier.DataSource = dt;//GetSupplierList(GridCFGSupplier.PageIndex, GridCFGSupplier.PageSize, nvc, ref total, ref msg);
                GridCFGSupplier.DataBind();
                GridCFGSupplier.RecordCount = total;


                foreach (DataRow dr in dt.Rows)
                {
                    if (DbHelperOra.Exists("SELECT 1 FROM DAT_GOODSSTOCK WHERE KCSL > 0 AND SUPID = '" + dr["SUPID"] + "'"))
                    {
                        int index = dt.Rows.IndexOf(dr);
                        hfdGridSupDisabled1.Text += index.ToString() + ",";
                    }
                }
                PageContext.RegisterStartupScript("setDisableYiyuan();");
                //setDisableYiyuan
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
            dataSearch("right");
        }


        //数字验证
        private static bool number_test(NumberBox test)
        {
            int tmp;
            if (!int.TryParse(test.Text, out tmp))
            {
                return false;
            }
            if (tmp < 0)
            {
                return false;
            }
            return true;
        }
        //ymh 验证时间
        public static bool IsDate(string StrSource)
        {
            return Regex.IsMatch(StrSource, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
        }
        //处理字段空值
        private decimal rtn_empt(NumberBox empt)
        {
            if (empt.Text.Trim().Length > 0)
            {
                decimal rtn = Math.Round(Convert.ToDecimal(empt.Text.Trim()), 4);
                return rtn;
            }
            return 0;
        }

        protected void GridSupplier_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridSupplier.PageIndex = e.NewPageIndex;
            dataSearch();
        }

        protected void GridCFGSupplier_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridCFGSupplier.PageIndex = e.NewPageIndex;
            dataSearch("right");
        }

        protected void btnAddRight_Click(object sender, EventArgs e)
        {

            int[] selectArray = GridSupplier.SelectedRowIndexArray;
            if (selectArray.Length > 0)
            {
                String strData = hfdGridSupplier.Text;
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(strData);
                List<CommandInfo> cmdList = new List<CommandInfo>();
                for (int i = 0; i < selectArray.Length; i++)
                {

                    MyTable mySupplierCFG = new MyTable("DOC_SUPPLIER");
                    Hashtable ht = new Hashtable(mySupplierCFG.ColRow);
                    foreach (DictionaryEntry de in ht)
                    {
                        if (dt.Columns.Contains(de.Key.ToString()))
                        {
                            mySupplierCFG.ColRow[de.Key.ToString()] = dt.Rows[selectArray[i]][de.Key.ToString()];
                        }

                    }
                    #region not use
                    //mySupplierCFG.ColRow["SUPID"] =  dt.Rows[selectArray[i]]["SUPID"];
                    //mySupplierCFG.ColRow["SUPNAME"] = dt.Rows[selectArray[i]]["SUPNAME"];
                    //mySupplierCFG.ColRow["SUPENAME"] = dt.Rows[selectArray[i]]["SUPENAME"];
                    //mySupplierCFG.ColRow["SUPSIMNAME"] = dt.Rows[selectArray[i]]["SUPSIMNAME"];
                    //mySupplierCFG.ColRow["SUPSIMID"] = dt.Rows[selectArray[i]]["SUPSIMID"];
                    //mySupplierCFG.ColRow["FLAG"] = dt.Rows[selectArray[i]]["FLAG"];
                    //mySupplierCFG.ColRow["SUBJECT"] = dt.Rows[selectArray[i]]["SUBJECT"];
                    //mySupplierCFG.ColRow["SUPCAT"] = dt.Rows[selectArray[i]]["SUPCAT"];
                    //mySupplierCFG.ColRow["REGID"] = dt.Rows[selectArray[i]]["REGID"];
                    //mySupplierCFG.ColRow["CORPKID"] = dt.Rows[selectArray[i]]["CORPKID"];
                    //mySupplierCFG.ColRow["CORPTYPE"] = dt.Rows[selectArray[i]]["CORPTYPE"];
                    //mySupplierCFG.ColRow["YYZZNO"] = dt.Rows[selectArray[i]]["YYZZNO"];
                    //mySupplierCFG.ColRow["LOGINRQ"] = dt.Rows[selectArray[i]]["LOGINRQ"];
                    //mySupplierCFG.ColRow["LOGINLABEL"] = dt.Rows[selectArray[i]]["LOGINLABEL"];
                    //mySupplierCFG.ColRow["LOGINFUND"] = dt.Rows[selectArray[i]]["LOGINFUND"];
                    //mySupplierCFG.ColRow["JYGM"] = dt.Rows[selectArray[i]]["JYGM"];
                    //mySupplierCFG.ColRow["JYFW"] = dt.Rows[selectArray[i]]["JYFW"];
                    //mySupplierCFG.ColRow["TAXPAYER"] = dt.Rows[selectArray[i]]["TAXPAYER"];
                    //mySupplierCFG.ColRow["TAXRATE"] = dt.Rows[selectArray[i]]["TAXRATE"];
                    //mySupplierCFG.ColRow["TAXNO"] = dt.Rows[selectArray[i]]["TAXNO"];
                    //mySupplierCFG.ColRow["BANK"] = dt.Rows[selectArray[i]]["BANK"];
                    //mySupplierCFG.ColRow["ACCNTNO"] = dt.Rows[selectArray[i]]["ACCNTNO"];
                    //mySupplierCFG.ColRow["LOGINADDR"] = dt.Rows[selectArray[i]]["LOGINADDR"];
                    //mySupplierCFG.ColRow["LEADER"] = dt.Rows[selectArray[i]]["LEADER"];
                    //mySupplierCFG.ColRow["LEADERIDCARD"] = dt.Rows[selectArray[i]]["LEADERIDCARD"];



                    //mySupplierCFG.ColRow["TEL"] = dt.Rows[selectArray[i]]["TEL"];
                    //mySupplierCFG.ColRow["FAX"] = dt.Rows[selectArray[i]]["FAX"];
                    //mySupplierCFG.ColRow["TELSERVICE"] = dt.Rows[selectArray[i]]["TELSERVICE"];
                    //mySupplierCFG.ColRow["ZIP"] = dt.Rows[selectArray[i]]["ZIP"];
                    //mySupplierCFG.ColRow["EMAIL"] = dt.Rows[selectArray[i]]["EMAIL"];
                    //mySupplierCFG.ColRow["URL"] = dt.Rows[selectArray[i]]["URL"];
                    //mySupplierCFG.ColRow["ISGATHERING"] = dt.Rows[selectArray[i]]["ISGATHERING"];
                    //mySupplierCFG.ColRow["GATFUNDCORP"] = dt.Rows[selectArray[i]]["GATFUNDCORP"];
                    //mySupplierCFG.ColRow["GATFUNDBANK"] = dt.Rows[selectArray[i]]["GATFUNDBANK"];
                    //mySupplierCFG.ColRow["GATACCNTNO"] = dt.Rows[selectArray[i]]["GATACCNTNO"];
                    //mySupplierCFG.ColRow["ZZADDR"] = dt.Rows[selectArray[i]]["ZZADDR"];
                    //mySupplierCFG.ColRow["LINKMAN"] = dt.Rows[selectArray[i]]["LINKMAN"];
                    //mySupplierCFG.ColRow["LINKMANDUTY"] = dt.Rows[selectArray[i]]["LINKMANDUTY"];
                    //mySupplierCFG.ColRow["LINKTEL"] = dt.Rows[selectArray[i]]["LINKTEL"];
                    //mySupplierCFG.ColRow["LINKFAX"] = dt.Rows[selectArray[i]]["LINKFAX"];
                    //mySupplierCFG.ColRow["LINKEMAIL"] = dt.Rows[selectArray[i]]["LINKEMAIL"];
                    //mySupplierCFG.ColRow["CWLINKMAN"] = dt.Rows[selectArray[i]]["CWLINKMAN"];
                    //mySupplierCFG.ColRow["CWLINKDUTY"] = dt.Rows[selectArray[i]]["CWLINKDUTY"];
                    //mySupplierCFG.ColRow["CWLINKTEL"] = dt.Rows[selectArray[i]]["CWLINKTEL"];
                    //mySupplierCFG.ColRow["CWLINKFAX"] = dt.Rows[selectArray[i]]["CWLINKFAX"];
                    //mySupplierCFG.ColRow["CWLINKEMAIL"] = dt.Rows[selectArray[i]]["CWLINKEMAIL"];
                    //mySupplierCFG.ColRow["BUYERID"] = dt.Rows[selectArray[i]]["BUYERID"];
                    //mySupplierCFG.ColRow["APPLYDEPT"] = dt.Rows[selectArray[i]]["APPLYDEPT"];
                    //mySupplierCFG.ColRow["MANAGER"] = dt.Rows[selectArray[i]]["MANAGER"];
                    //mySupplierCFG.ColRow["CRERQ"] = dt.Rows[selectArray[i]]["CRERQ"];

                    //mySupplierCFG.ColRow["ZZRQ"] = dt.Rows[selectArray[i]]["ZZRQ"];
                    //mySupplierCFG.ColRow["BUYERID"] = dt.Rows[selectArray[i]]["BUYERID"];
                    //mySupplierCFG.ColRow["APPLYDEPT"] = dt.Rows[selectArray[i]]["APPLYDEPT"];
                    //mySupplierCFG.ColRow["MANAGER"] = dt.Rows[selectArray[i]]["MANAGER"];
                    //mySupplierCFG.ColRow["CRERQ"] = dt.Rows[selectArray[i]]["CRERQ"];
                    #endregion
                    mySupplierCFG.ColRow["ISDG"] = "Y";
                    cmdList.Add(mySupplierCFG.Insert());
                }
                //List<CommandInfo> cmdList = new List<CommandInfo>();
                //MyTable mySupplierCFG = new MyTable("DOC_SUPPLIER_CUST");
                //for (int i = 0; i < selectArray.Length; i++)
                //{
                //    //if (GridSupplier.Rows[selectArray[i]].Values[7].ToString() != "Y")
                //    //{
                //    //    Alert.Show(string.Format("供应商【{0}】状态信息错误！", GridSupplier.Rows[selectArray[i]].Values[2].ToString()), "消息提示", MessageBoxIcon.Warning);
                //    //    return;
                //    //}
                //    mySupplierCFG.ColRow.Clear();
                //    mySupplierCFG.ColRow.Add("SUPID", GridSupplier.Rows[selectArray[i]].Values[0]);
                //    //mySupplierCFG.ColRow.Add("ISCFG", "1");
                //    cmdList.Add(mySupplierCFG.Insert());
                //}
                DbHelperOra.ExecuteSqlTran(cmdList);
                CacheHelper.RemoveOneCache("DDL_DOC_SUPPLIER_DG");
                hfdGridSupDisabled1.Text = "";
                dataSearch();
                Alert.Show("本地代管供应商信息添加成功，您需要【刷新】页面后才能进行【代管补货】操作!");
                dataSearch("right");
            }
            else
            {
                Alert.Show("请选择要添加的供应商信息！");
                GridSupplier.Focus();
            }
        }

        protected void btnAddLeft_Click(object sender, EventArgs e)
        {
            int[] selectArray = GridCFGSupplier.SelectedRowIndexArray;
            if (selectArray.Length > 0)
            {
                List<CommandInfo> cmdList = new List<CommandInfo>();
                for (int i = 0; i < selectArray.Length; i++)
                {
                    cmdList.Add(new CommandInfo("delete DOC_SUPPLIER where SUPID='" + GridCFGSupplier.Rows[selectArray[i]].Values[0].ToString() + "'", null));
                }
                DbHelperOra.ExecuteSqlTran(cmdList);
                CacheHelper.RemoveOneCache("DDL_DOC_SUPPLIER_DG");
                hfdGridSupDisabled1.Text = "";
                dataSearch();
                Alert.Show("本地代管供应商信息移除成功，您需要【刷新】页面后才能进行【代管补货】操作!");
                dataSearch("right");
            }
        }

        /// <summary>
        /// 获取商品数据信息
        /// </summary>
        /// <param name="pageNum">第几页</param>
        /// <param name="pageSize">每页显示天数</param>
        /// <param name="nvc">查询条件</param>
        /// <param name="total">总的条目数</param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public DataTable GetSupplierList(int pageNum, int pageSize, NameValueCollection nvc, ref int total, ref string errMsg)
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
                            case "CX":
                                strSearch += string.Format(" AND （SUPNAME LIKE '%{0}%' OR SUPID LIKE '%{0}%' OR SUPSIMNAME LIKE '%{0}%' OR SUPENAME LIKE '%{0}%'）", condition);
                                break;
                            //case "CATID":
                            //    strSearch += string.Format(" AND CATID LIKE '{0}%'", condition);
                            //    break;
                            case "RIGHT":
                                strSearch += string.Format(" AND SUPID IN (SELECT SUPID FROM DOC_SUPPLIER_CUST WHERE CUSTID = '{0}')", condition);
                                break;
                            case "LEFT":
                                strSearch += string.Format(" AND SUPID NOT IN (SELECT SUPID FROM DOC_SUPPLIER_CUST WHERE CUSTID = '{0}')", condition);
                                break;
                        }
                    }
                }
            }
            StringBuilder strSql = new StringBuilder(@"SELECT * FROM DOC_SUPPLIER WHERE FLAG = 'Y' and isdg='Y'");
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql.Append(strSearch);
            }
            strSql.Append(" order by SUPID");
            return GetDataTable(pageNum, pageSize, strSql, ref total);
        }

        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            //if (ddlDept.SelectedItem.Value.Length < 1) { Alert.Show("请先选择配置部门"); return; }
            dataSearch();
            dataSearch("right");
        }

        protected void cbxTB_CheckedChanged(object sender, CheckedEventArgs e)
        {
            //不允许修改选中状态
            //cbxTB.Checked = true;
        }

        /// <summary>
        /// 根据索引和pagesize返回记录
        /// </summary>
        /// <param name="dt">记录集 DataTable</param>
        /// <param name="PageIndex">当前页</param>
        /// <param name="pagesize">一页的记录数</param>
        /// <returns></returns>
        public static DataTable SplitDataTable(DataTable dt, int PageIndex, int PageSize)
        {
            if (PageIndex == 0)
                return dt;
            DataTable newdt = dt.Clone();
            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;

            if (rowbegin >= dt.Rows.Count)
                return newdt;

            if (rowend > dt.Rows.Count)
                rowend = dt.Rows.Count;
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }

            return newdt;
        }
    }
}
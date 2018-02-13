using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Data;
using Newtonsoft.Json.Linq;
using XTBase;
using XTBase.Utilities;

namespace ERPProject.ERPStorage
{
    public partial class OrderEntry : PageBase
    {
        private DataTable dtOrders;
        private bool AppendToEnd = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                hfdCacheKey.Text = UserAction.UserID + "_OrderGoods";
                CacheHelper.RemoveOneCache(hfdCacheKey.Text);
                DataInit();
            }
            //else
            //{
            //    if (GetRequestEventArgument() == "GoodsInfo")
            //    {
            //        GridGoods.DataSource = GetDataTableGoods();
            //        GridGoods.DataBind();
            //    }
            //}

        }

        protected DataTable GetDataTable1()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Class", typeof(string)));
            table.Columns.Add(new DataColumn("Month1", typeof(string)));
            table.Columns.Add(new DataColumn("Month2", typeof(string)));
            table.Columns.Add(new DataColumn("Month3", typeof(string)));
            table.Columns.Add(new DataColumn("Month4", typeof(string)));
            table.Columns.Add(new DataColumn("Month5", typeof(string)));
            table.Columns.Add(new DataColumn("Month6", typeof(string)));
            table.Columns.Add(new DataColumn("Month7", typeof(string)));

            DataRow row = table.NewRow();

            row[0] = "采购";
            row[1] = "0.000";
            row[2] = "0.000";
            row[3] = "0.000";
            row[4] = "0.000";
            row[5] = "0.000";
            row[6] = "0.000";
            row[7] = "0.000";
            table.Rows.Add(row);

            row = table.NewRow();
            row[0] = "出库";
            row[1] = "0.000";
            row[2] = "0.000";
            row[3] = "0.000";
            row[4] = "0.000";
            row[5] = "0.000";
            row[6] = "0.000";
            row[7] = "0.000";
            table.Rows.Add(row);

            return table;
        }

        protected DataTable GetDataTable2()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Class", typeof(string)));
            table.Columns.Add(new DataColumn("DayInfo", typeof(string)));
            table.Columns.Add(new DataColumn("WeekInfo", typeof(string)));
            table.Columns.Add(new DataColumn("MonthInfo", typeof(string)));

            DataRow row = table.NewRow();

            row[0] = "自动订货";
            row[1] = "定数补充";
            row[2] = "不进行操作";
            row[3] = "不进行操作";
            table.Rows.Add(row);

            row = table.NewRow();
            row[0] = "订货点";
            row[1] = "100";
            row[2] = "0.000";
            row[3] = "0.000";
            table.Rows.Add(row);

            row = table.NewRow();
            row[0] = "订货量";
            row[1] = "300";
            row[2] = "0.000";
            row[3] = "0.000";
            table.Rows.Add(row);

            return table;
        }

        protected DataTable GetDataTable3()
        {
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("Storeing", typeof(string)));
            table.Columns.Add(new DataColumn("Stored", typeof(string)));
            table.Columns.Add(new DataColumn("Coefficient", typeof(string)));
            table.Columns.Add(new DataColumn("PurchaseDay", typeof(string)));
            table.Columns.Add(new DataColumn("PurchaseSum", typeof(string)));

            DataRow row = table.NewRow();

            row[0] = "501";
            row[1] = "623.000";
            row[2] = "1";
            row[3] = "2014-08-08";
            row[4] = "348";
            table.Rows.Add(row);

            return table;
        }

        protected DataTable GetDataTableGoods()
        {
            DataTable table = new DataTable();
            if (CacheHelper.GetCache(hfdCacheKey.Text) == null)
            {
                table.Columns.Add(new DataColumn("GDSEQ", typeof(string)));
                table.Columns.Add(new DataColumn("NAME", typeof(string)));
                table.Columns.Add(new DataColumn("GDSPEC", typeof(string)));
                table.Columns.Add(new DataColumn("UNIT", typeof(string)));
                table.Columns.Add(new DataColumn("BZHL", typeof(decimal)));
                table.Columns.Add(new DataColumn("ZPBH", typeof(string)));
                table.Columns.Add(new DataColumn("SPLB", typeof(string)));
                table.Columns.Add(new DataColumn("CDID", typeof(string)));
                table.Columns.Add(new DataColumn("SUPID", typeof(string)));
                table.Columns.Add(new DataColumn("HSJJ", typeof(decimal)));
                table.Columns.Add(new DataColumn("DHS", typeof(decimal)));
                table.Columns.Add(new DataColumn("HSJE", typeof(decimal)));
                table.Columns.Add(new DataColumn("KCSL", typeof(decimal)));
                table.Columns.Add(new DataColumn("LOT", typeof(string)));
                table.Columns.Add(new DataColumn("BZRQ", typeof(string)));

                CacheHelper.SetCache(hfdCacheKey.Text, table);
            }
            else
            {
                table = CacheHelper.GetCache(hfdCacheKey.Text) as DataTable;
            }

            return table;
        }

        private void DataInit()
        {
            // 新增数据初始值
            JObject defaultObj = new JObject();
            defaultObj.Add("GDSEQ", "");
            defaultObj.Add("NAME", "");
            defaultObj.Add("GDSPEC", "");
            defaultObj.Add("UNIT", "");
            defaultObj.Add("BZHL", "");
            defaultObj.Add("ZPBH", "");
            defaultObj.Add("SPLB", "");
            defaultObj.Add("CDID", "");
            defaultObj.Add("SUPID", "");
            defaultObj.Add("HSJJ", "");
            defaultObj.Add("DHS", "");
            defaultObj.Add("HSJE", "");
            defaultObj.Add("KCSL", "");
            defaultObj.Add("CKBM", "");
            defaultObj.Add("LOT", "");
            defaultObj.Add("BZRQ", "");

            // 在第一行新增一条数据
            //btnAddRow.OnClientClick = GridGoods.GetAddNewRecordReference(defaultObj, AppendToEnd);

            // 删除选中行按钮
            //btnDelete.OnClientClick = Grid1.GetNoSelectionAlertReference("请至少选择一项！") + deleteScript;


            PubFunc.DdlDataGet(ddlGoodsType, "DDL_GOODS_TYPE");
            dpkOrderDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            BindGrid();
            dtOrders = GetDataTableGoods();//DbHelperOra.Query("select GDSEQ,NAME, GDSPEC, UNIT, BZHL, ZPBH, SPLB, CDID, SUPID, HSJJ, DHS, HSJE, KCSL, CKBM, LOT, BZRQ from DAT_DD_COM where 1=2").Tables[0];
            GridGoods.DataSource = dtOrders;
            GridGoods.DataBind();
        }

        private void BindGrid()
        {
            Grid1.DataSource = GetDataTable1();
            Grid1.DataBind();
            Grid2.DataSource = GetDataTable2();
            Grid2.DataBind();
            Grid3.DataSource = GetDataTable3();
            Grid3.DataBind();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {

        }
        private void dataSearch()
        {
            //            string strSearch, strDept, strUserStatus;
            //            strSearch = txtSearch.Text == "" ? "%" : txtSearch.Text;
            //            strDept = ddlDept.SelectedValue;
            //            strDept = strDept == null || strDept == "" ? "%" : strDept;
            //            strUserStatus = ddlEmpStatus.SelectedValue == "" ? "%" : ddlEmpStatus.SelectedValue;
            //            string strSql = @"select t.userid,t.usergh,t.username,d.name Dept,t.usertype,t.intotime,t.tel,t.asim,
            //                                     decode(t.status, '0', '在勤', '1', '晋升', '2', '调动', '3', '离职') Status
            //                                from  sys_operuser t, sys_dept d
            //                                where t.usergh<>'admin' and t.dept = d.code(+) and nvl(t.dept,'%') like '" + strDept + @"' and t.username like '%" + strSearch + @"%'
            //                                      and t.STATUS like '" + strUserStatus + @"'
            //                                order by t.username";
            //            dtData = DbHelperOra.Query(strSql).Tables[0];
            //GridGoods.DataSource = dtData;
            //GridGoods.DataBind();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }
        protected void btnAddRow_Click(object sender, EventArgs e)
        {
            dtOrders = GetDataTableGoods();
            DataRow row = dtOrders.NewRow();
            row[0] = "";
            row[1] = "";
            row[2] = "";
            row[3] = "";
            row[4] = "0";
            row[5] = "";
            row[6] = "";
            row[7] = "";
            row[8] = "";
            row[9] = "0";
            row[10] = "0";
            row[11] = "0";
            row[12] = "0";
            row[13] = "";
            row[14] = "";
            dtOrders.Rows.Add(row);
            CacheHelper.SetCache(hfdCacheKey.Text, dtOrders);

            GridGoods.DataSource = dtOrders;
            GridGoods.DataBind();
        }
        protected void btnDelRow_Click(object sender, EventArgs e)
        {

        }
        protected void GridGoods_RowCommand(object sender, FineUIPro.GridCommandEventArgs e)
        {
            string strUserID = GridGoods.Rows[e.RowIndex].Values[0].ToString();
            string strUserName = GridGoods.Rows[e.RowIndex].Values[1].ToString();
            if (e.CommandName == "Delete")
            {
                //DbHelperOra.ExecuteSql("UPDATE SYS_OPERUSER SET FLAG='-1' WHERE USERGH='" + strUserID + "'");
                dataSearch();
            }
            else
            {
                FineUIPro.PageContext.RegisterStartupScript(Window1.GetShowReference("~/AppFrame/UserEdith.aspx?id=" + strUserID, "用户【" + strUserName + "】信息修改"));
                Window1.Hidden = false;
            }
        }
        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            dtOrders = GetDataTableGoods();
            string strValue = hfdValue.Text;
            string[] goodsRows = strValue.Split(';');
            foreach (string rowValue in goodsRows)
            {
                string[] goods = rowValue.Split('_');
                DataRow row = dtOrders.NewRow();
                row[0] = goods[0];
                row[1] = goods[1];
                row[2] = goods[2];
                row[3] = goods[3];
                row[4] = goods[4];
                row[5] = goods[5];
                row[6] = goods[6];
                row[7] = goods[7];
                row[8] = goods[8];
                row[9] = "10.5";
                row[10] = "0";
                row[11] = "0";
                row[12] = "0";
                row[13] = "";
                row[14] = "";
                dtOrders.Rows.Add(row);
            }

            GridGoods.DataSource = dtOrders;
            GridGoods.DataBind();
        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {

        }

        protected void btnAuto_Click(object sender, EventArgs e)
        {

        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            UpdateAllUserInputData();
            DataTable dt = dtOrders;
        }

        private void UpdateAllUserInputData()
        {
            // 更新所有行的用户输入数据
            foreach (GridRow row in GridGoods.Rows)
            {
                int rowIndex = row.RowIndex;
                System.Web.UI.WebControls.TextBox tbxNumber = (System.Web.UI.WebControls.TextBox)GridGoods.Rows[rowIndex].FindControl("tbxNumber");

                int rowDataId = Convert.ToInt32(GridGoods.DataKeys[rowIndex][0]);

                int quantity = 0;
                try
                {
                    quantity = Convert.ToInt32(tbxNumber.Text);
                }
                catch (Exception)
                {
                    // ...
                }

                SetDataRow(rowDataId, quantity);
            }
        }

        private void SetDataRow(int dataId, int quantity)
        {
            DataRow row = FindDataRowById(dataId);
            row["DHS"] = quantity;
            row["HSJE"] = Convert.ToDouble(row["HSJJ"]) * quantity;
        }

        private DataRow FindDataRowById(int dataId)
        {
            dtOrders = GetDataTableGoods();

            foreach (DataRow row in dtOrders.Rows)
            {
                if (Convert.ToInt32(row["GDSEQ"]) == dataId)
                {
                    return row;
                }
            }
            return null;
        }



        protected void btSearch_Click(object sender, EventArgs e)
        {
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference("~/ERPQuery/GoodsWindow.aspx", "商品信息查询"));
        }

        private void UpdateDataRow(Dictionary<string, object> rowDict, DataRow rowData)
        {
            // 姓名
            if (rowDict.ContainsKey("GDSEQ"))
            {
                rowData["GDSEQ"] = rowDict["GDSEQ"];
            }
            // 性别
            if (rowDict.ContainsKey("NAME"))
            {
                rowData["NAME"] = rowDict["NAME"];
            }
            // 入学年份
            if (rowDict.ContainsKey("GDSPEC"))
            {
                rowData["GDSPEC"] = rowDict["GDSPEC"];
            }
            // 入学日期
            if (rowDict.ContainsKey("UNIT"))
            {
                rowData["UNIT"] = rowDict["UNIT"];
            }
            // 是否在校
            if (rowDict.ContainsKey("BZHL"))
            {
                rowData["BZHL"] = rowDict["BZHL"];
            }
            // 所学专业
            if (rowDict.ContainsKey("ZPBH"))
            {
                rowData["ZPBH"] = rowDict["ZPBH"];
            }
        }

        protected string GetXiaoji(object priceobj, object numberobj)
        {
            float price = Convert.ToSingle(priceobj);
            int number = Convert.ToInt32(numberobj);

            return String.Format("{0:F}", price * number);
        }

    }
}
using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPApply
{
    public partial class ReturnsForSure : PageBase
    {
        private bool AppendToEnd = true;
        private string strDocSql = "SELECT * FROM DAT_CK_DOC WHERE SEQNO ='{0}'";
        private string strComSql = "SELECT * FROM DAT_CK_COM WHERE SEQNO ='{0}'";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }
        }

        private void DataInit()
        {
            // 新增数据初始值
            JObject defaultObj = new JObject();
            defaultObj.Add("GDSEQ", "");
            defaultObj.Add("BARCODE", "");
            defaultObj.Add("NAME", "");
            defaultObj.Add("GDSPEC", "");
            defaultObj.Add("UNIT", "");
            defaultObj.Add("BZHL", "");
            defaultObj.Add("DHSL", "");
            defaultObj.Add("XSSL", "");
            defaultObj.Add("JXTAX", "");
            defaultObj.Add("HSJJ", "");
            defaultObj.Add("HSJE", "");
            defaultObj.Add("ZPBH", "");
            defaultObj.Add("CDID", "");
            defaultObj.Add("HWID", "");
            defaultObj.Add("PH", "");
            defaultObj.Add("PZWH", "");
            defaultObj.Add("RQ_SC", "");
            defaultObj.Add("YXQZ", "");
            defaultObj.Add("MEMO", "");

            // 第一行新增一条数据
            btnAddRow.OnClientClick = GridGoods.GetAddNewRecordReference(defaultObj, AppendToEnd);

            docXSRQ.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            if (e.ColumnID == "BARCODE")
            {
                PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference("~/ERPQuery/GoodsWindow.aspx", "商品信息查询"));
            }
        }

        private void btnClear_Click()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        private void bntSearch_Click()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("请输入条件【录入日期】！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO,B.NAME FLAG,A.DDBH,F_GETDEPTNAME(A.DEPTID) DEPTID,A.SUPNAME,A.DHRQ,
                                     A.SUBSUM,F_GETUSERNAME(A.CGY) CGY,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,A.SHR,A.SHRQ,A.MEMO 
                                from DAT_RK_DOC A,
                                     SYS_CODEVALUE B 
                                WHERE A.FLAG = B.CODE AND B.TYPE  ='DIC_BILLSTATUS' ";
            string strSearch = "";


            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text);
            }
            if (lstFLAG.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstSLR.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.SLR='{0}'", lstSLR.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstDEPTOUT.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND DEPTOUT='{0}'", lstDEPTOUT.SelectedItem.Value);
            }

            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }

            GridList.DataSource = DbHelperOra.Query(strSql).Tables[0];
            GridList.DataBind();
        }

        protected void btnBill_Click(object sender, EventArgs e)
        {
            switch (((FineUIPro.Button)sender).ID.ToString())
            {
                case "bntClear":
                    btnClear_Click();
                    break;
                case "bntSearch":
                    bntSearch_Click();
                    break;
                default:
                    Alert.Show("按钮【" + ((FineUIPro.Button)sender).Text.ToString() + "】的方法【" + ((FineUIPro.Button)sender).ID.ToString() + "】未定义！");
                    break;
            }

        }

        protected void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridGoods.Rows[e.RowIndex].Values[1].ToString());
        }

        private void billOpen(string strBillno)
        {
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);
            decimal bzslTotal = 0, feeTotal = 0;
            if (dtDoc != null && dtDoc.Rows.Count > 0)
            {
                foreach (DataRow row in dtDoc.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["BZSL"]);
                    feeTotal += Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZSL"]);
                }
            }
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("BZSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;

            DataTable dtCom  = DbHelperOra.Query(string.Format(strComSql, strBillno)).Tables[0];
            GridGoods.DataSource = dtCom;
            GridGoods.DataBind();
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            bool firstRow = true;
            string deleteScript = GridGoods.GetDeleteSelectedReference();
            string strValue = hfdValue.Text;
            string[] goodsRows = strValue.Split(';');
            foreach (string rowValue in goodsRows)
            {
                string[] goods = rowValue.Split('_');
                JObject defaultObj = new JObject();
                defaultObj.Add("GDSEQ", goods[0]);
                defaultObj.Add("BARCODE", "");
                defaultObj.Add("NAME", goods[1]);
                defaultObj.Add("GDSPEC", goods[2]);
                defaultObj.Add("UNIT", goods[3]);
                defaultObj.Add("BZHL", goods[0]);
                defaultObj.Add("DHSL", "");
                defaultObj.Add("XSSL", "");
                defaultObj.Add("JXTAX", "");
                defaultObj.Add("HSJJ", "");
                defaultObj.Add("HSJE", "");
                defaultObj.Add("ZPBH", "");
                defaultObj.Add("CDID", "");
                defaultObj.Add("HWID", "");
                defaultObj.Add("PH", "");
                defaultObj.Add("PZWH", "");
                defaultObj.Add("RQ_SC", "");
                defaultObj.Add("YXQZ", "");
                defaultObj.Add("MEMO", "");
                if (firstRow)
                {
                    PageContext.RegisterStartupScript(deleteScript + GridGoods.GetAddNewRecordReference(defaultObj, AppendToEnd));
                }
                else
                {
                    PageContext.RegisterStartupScript(GridGoods.GetAddNewRecordReference(defaultObj, AppendToEnd));
                }
                firstRow = false;
            }
        }
    }
}
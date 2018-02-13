using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System.Data;
using System.Collections.Specialized;
using System.Text;
using XTBase.Utilities;
using System.Collections;
using Newtonsoft.Json.Converters;
using System.Threading;

namespace ERPProject.ERPQuery
{
    public partial class CatFX : BillBase
    {
        public string text = "商品使用类别分析(数量)";
        public string text2 = "商品使用类别分析(金额)";
        public string subtext = "";
        public string sqlsl = @"SELECT SUM(SL)ENDSL,CATID FROM (
SELECT SUM(DXC.DHSL)SL,(SELECT CATID FROM DOC_GOODS WHERE GDSEQ=DXC.GDSEQ)CATID
FROM DAT_GOODSJXC DGJ,DAT_XS_COM DXC
 WHERE DGJ.BILLTYPE='XSD'
 AND DGJ.BILLNO=DXC.SEQNO 
 AND TRUNC(DGJ.RQSJ,'DD')>TRUNC(SYSDATE-15,'DD')
 GROUP BY DXC.GDSEQ)
 WHERE ROWNUM<=10
 GROUP BY CATID";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                billSearch();
            }
        }
        private void DataInit()
        {

            dpkBEGRQ.SelectedDate = DateTime.Now.AddDays(-30);
            dpkENDRQ.SelectedDate = DateTime.Now;
            DateTime begrq = Convert.ToDateTime(dpkBEGRQ.Text);
            DateTime endrq = Convert.ToDateTime(dpkENDRQ.Text);
            lstCAT.DataValueField = "CODE";
            lstCAT.DataTextField = "NAME";
            DataTable dt = DbHelperOra.QueryForTable("SELECT ''CODE,'--请选择--'NAME FROM DUAL UNION ALL SELECT code,NAME FROM doc_goodstype WHERE FLAG = 'Y'");
            lstCAT.DataSource = dt;
            lstCAT.DataBind();
           

        }
        protected override void billSearch()
        {
            if (PubFunc.StrIsEmpty(dpkBEGRQ.SelectedDate.ToString()) || PubFunc.StrIsEmpty(dpkENDRQ.SelectedDate.ToString()))
            {
                Alert.Show("【输入日期】不正确,请检查！", MessageBoxIcon.Warning);
                return;
            }
            if (dpkBEGRQ.SelectedDate > dpkENDRQ.SelectedDate)
            {
                Alert.Show("【开始日期】不能大于【结束日期】！", MessageBoxIcon.Warning);
                return;
            }
            DateTime begrq = Convert.ToDateTime(dpkBEGRQ.Text);
            DateTime endrq = Convert.ToDateTime(dpkENDRQ.Text);
            string strSql = string.Format(@"select code,name CATEGORY,F_GET_CAT_PIE('SYSL',CODE,TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),'Z')SYSL,F_GET_CAT_PIE('SLZB',CODE,TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),'Z')SLZB,
       TO_NUMBER(F_GET_CAT_PIE('SYJE',CODE,TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),'Z'))SYJE, F_GET_CAT_PIE('JEZB',CODE,TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),'Z')JEZB, F_GET_CAT_PIE('HBZZSL',CODE,TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),'Z')HBZZSL ,
                  F_GET_CAT_PIE('TBZZSL',CODE,TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),'Z')TBZZSL, F_GET_CAT_PIE('HBZZJE',CODE,TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),'Z')HBZZJE,F_GET_CAT_PIE('TBZZJE',CODE,TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),'Z')TBZZJE
   from doc_goodstype WHERE FLAG = 'Y' ", begrq.ToShortDateString(), endrq.ToShortDateString());
            string strSearch = "";
            if (!string.IsNullOrEmpty(lstCAT.SelectedValue))
            {
                strSearch += " AND CODE='"+lstCAT.SelectedValue+"' ";
            }
            strSql += strSearch;
            int total = 0;
            highyellowlight.Text = ",";
            highredlight.Text = ",";
            string sortField = GridList.SortField;
            string sortDirection = GridList.SortDirection;
            //DataTable dt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSql + String.Format(" ORDER BY code"), ref total);
            DataTable dt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSql + String.Format(" ORDER BY {0} {1}", sortField, sortDirection), ref total);
            GridList.RecordCount = total;
            GridList.DataSource = dt;
            GridList.DataBind();
            DataTable dtinit;
          //  DataTable dtinit = DbHelperOra.QueryForTable("select code,name from doc_goodstype where  flag='Y'" + String.Format(" ORDER BY {0} {1}", "CODE", sortDirection));
            if (!string.IsNullOrEmpty(lstCAT.SelectedValue))
            {
                dtinit = DbHelperOra.QueryForTable("select code,name from sys_category where  type ='" + lstCAT.SelectedValue + "' and  flag='Y'" + String.Format(" ORDER BY {0} {1}", "CODE", sortDirection));

            }
            else {
                dtinit = DbHelperOra.QueryForTable("select code,name from doc_goodstype where   flag='Y'" + String.Format(" ORDER BY {0} {1}", "CODE", sortDirection));

            }


            init_pie(dtinit, begrq, endrq);
        }
        protected void GridList_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch();
        }
        protected override void billClear()
        {
            DataInit();

        }
        protected void lstGDSEQ_TriggerClick(object sender, EventArgs e)
        {
            //查询信息统一触发
            billSearch();
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {


        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            if (GridList.Rows.Count < 1)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }

            DateTime begrq = Convert.ToDateTime(dpkBEGRQ.Text);
            DateTime endrq = Convert.ToDateTime(dpkENDRQ.Text);
            string strSql = string.Format(@"select decode(type, '2', code, type)code,name CATEGORY,F_GET_CAT_PIE('SYSL',decode(type,'2',code,type),TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),decode(type,'2','C','Z'))使用数量,F_GET_CAT_PIE('SLZB',decode(type,'2',code,type),TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),decode(type,'2','C','Z'))数量占比,
                     F_GET_CAT_PIE('HBZZSL',decode(type,'2',code,type),TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),decode(type,'2','C','Z'))环比增长数量 ,
                     F_GET_CAT_PIE('TBZZSL',decode(type,'2',code,type),TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),decode(type,'2','C','Z'))同比增长数量,
                     F_GET_CAT_PIE('SYJE',decode(type,'2',code,type),TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),decode(type,'2','C','Z'))使用金额, F_GET_CAT_PIE('JEZB',decode(type,'2',code,type),TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),decode(type,'2','C','Z'))金额占比,
                     F_GET_CAT_PIE('HBZZJE',decode(type,'2',code,type),TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),decode(type,'2','C','Z'))环比增长金额,F_GET_CAT_PIE('TBZZJE',decode(type,'2',code,type),TO_DATE('{0}','yyyy-MM-dd'),TO_DATE('{1}','yyyy-MM-dd'),decode(type,'2','C','Z'))同比增长金额
             from sys_category where sjcode='0' and flag='Y'", begrq.ToShortDateString(), endrq.ToShortDateString());

            highyellowlight.Text = ",";
            highredlight.Text = ",";
            DataTable dt = DbHelperOra.QueryForTable(strSql);

            ExcelHelper.ExportByWeb(dt, "商品使用类别分析导出", "商品使用类别分析导出_" + DateTime.Now.ToString("yyyyMMddHH") + ".xls");
        }
        protected void init_pie(DataTable dt, DateTime begrq, DateTime endrq)
        {
            string sbeg = "[";
            string send = "]";
            string scon = "";
            string dbeg = "{";
            string dend = "}";
            string dcon = "";
            string sysl = "";
            string syje = "";
            string je = "";
            string data1 = "";
            string data2 = "";
            string data22 = "";
            foreach (DataRow dr in dt.Rows)
            {
                Object obj = DbHelperOra.GetSingle(string.Format("select F_GET_CAT_PIE('SYSL','{0}',TO_DATE('{1}','yyyy-MM-dd'),TO_DATE('{2}','yyyy-MM-dd'),'Z')SYSL from dual", dr[0].ToString(), begrq.ToShortDateString(), endrq.ToShortDateString())).ToString();
                sysl = Convert.ToString(obj ?? "0");
                if (sysl != "0")
                {
                    scon = scon + "'" + dr[1].ToString() + "',";
                    dcon = dcon + dbeg + "value:" + sysl + ",name:" + "'" + dr[1].ToString() + "'" + dend + ",";
                    syje = DbHelperOra.GetSingle(string.Format("select F_GET_CAT_PIE('SYJE','{0}',TO_DATE('{1}','yyyy-MM-dd'),TO_DATE('{2}','yyyy-MM-dd'),'Z')SYSL from dual", dr[0].ToString(), begrq.ToShortDateString(), endrq.ToShortDateString())).ToString();
                    je = je + dbeg + "value:" + syje + ",name:" + "'" + dr[1].ToString() + "'" + dend + ",";
                }
            }
            if (scon == "")
            {
                data1 = sbeg + send;
                data2 = "[" +"0"+ "]";
                data22 = "[" + "0" + "]";

            }
            else
            {
             data1 = sbeg + scon.Substring(0, scon.Length - 1) + send;
             data2 = "[" + dcon.Substring(0, dcon.Length - 1) + "]";
             data22 = "[" + je.Substring(0, je.Length - 1) + "]";
            }
            
            PageContext.RegisterStartupScript("showpie2('" + text2 + "','" + subtext + "'," + data1 + "," + data22 + ");");
            PageContext.RegisterStartupScript("showpie('" + text + "','" + subtext + "'," + data1 + "," + data2 + ");");
        }

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            billSearch();
        }

     
    
    }
}
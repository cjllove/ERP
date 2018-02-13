using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using System.Data;
using Newtonsoft.Json.Linq;
using XTBase.Utilities;
using System.Text;

namespace ERPProject.ERPQuery
{
    public partial class SupGoodsSearch : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 在页面第一次加载时 
                BindDDL();
            }
        }
 

        private void BindDDL()
        {
            dpkDATE1.SelectedDate = DateTime.Now.AddMonths(-1);
            dpkDATE2.SelectedDate = DateTime.Now;
           
            PubFunc.DdlDataGet("DDL_DOC_SUPID", ddlSUPID);
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            
            GridGoods.SummaryData = summary;
        }

        private string GetSearchSql()
        {
            string strSql = @"SELECT kf.SUPID,kf.SUPNAME,KF.PSSNAME,
       NVL(kf.QCKCSL, 0)+NVL(ks.QCKCSL, 0) ZQCSL,
       NVL(kf.QCKCHSJE, 0)+NVL(ks.QCKCHSJE, 0) ZQCHSJE,
       NVL(kf.TJJE, 0) +NVL(ks.TJJE, 0) ZTJ,
       NVL(kf.CGRK, 0) + NVL(ks.YKDRK, 0) CGRK,
       NVL(kf.CGRKHSJE, 0) +NVL(ks.YKDRKHSJE, 0) CGRKHSJE,
       NVL(kf.KFSY, 0) + NVL(ks.KSSY, 0) ZSY,
       NVL(kf.KFSYHSJE, 0) + NVL(ks.KSSYHSJE, 0) ZSYHSJE,
       NVL(ks.KSXH, 0)-NVL(KS.XSTRK,0) KSXH,
       NVL(ks.KSXHHSJE, 0)-NVL(KS.XSTRKHSJE,0) KSXHHSJE,
       NVL(kf.THCK, 0) THCK,
       NVL(kf.THCKHSJE, 0) THCKHSJE,
       NVL(kf.KCSL, 0)+NVL(KS.KCSL,0) KFKCSL,
       NVL(kf.KCHSJE, 0)+NVL(KS.KCHSJE,0) KFKCHSJE,
       --NVL(ks.KSXT, 0) KSXT,
       --NVL(ks.KSXTHSJE, 0) KSXTHSJE,
       NVL(kf.QMKCSL, 0)+NVL(KS.QMKCSL,0) QMKCSL,
       NVL(kf.QMKCHSJE, 0)+NVL(KS.QMKCHSJE,0) QMKCHSJE
       --NVL(ks.QMKCSL, 0) KSKCSL,
       --NVL(ks.QMKCSLHSJE, 0) KSKCSLHSJE
FROM (WITH TA AS
 (SELECT SUPID,PSSID,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{0}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCSL,
                    0)) QCKCSL,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{0}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCHSJE,
                    0)) QCKCHSJE,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{2}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCSL,
                    0)) KCSL,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{2}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCHSJE,
                    0)) KCHSJE
    FROM V_STOCKALL T
   WHERE (trunc(t.rq) = (to_date('{0}', 'YYYY-MM-DD')) or
         trunc(t.rq) = (to_date('{2}', 'YYYY-MM-DD'))) 
    and T.deptid in (SELECT distinct code
                        FROM sys_dept
                       where TYPE = '1'
                      /*AND F_CHK_DATARANGE(CODE, 'admin') = 'Y'*/
                      ) 
   GROUP BY SUPID,PSSID),
TB AS
 (SELECT SUPID,PSSID,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'RKD', SL, 0), 0)) CGRK,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'RKD', HSJE, 0), 0)) CGRKHSJE,
         SUM(DECODE(A.KCADD,
                    '1',
                    DECODE(A.BILLTYPE, 'LTD', SL, 'XST', SL, 'DST', SL, 0),
                    0)) KSTH,
         SUM(DECODE(A.KCADD,
                    '1',
                    DECODE(A.BILLTYPE,'LTD',HSJE,'XST', HSJE,'DST',HSJE, 0),
                    0)) KSTHHSJE,
         SUM(DECODE(A.BILLTYPE, 'SYD', SL, 0)) KFSY,
         SUM(DECODE(A.BILLTYPE, 'SYD', HSJE, 0)) KFSYHSJE,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'DBD', SL, 0), 0)) DBRK, --调拨入库
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'DBD', HSJE, 0), 0)) DBRKHSJE, --调拨入库               
         (SUM(DECODE(A.KCADD,
                        '-1',
                        DECODE(A.BILLTYPE, 'LCD', SL, 'CKD', SL, 'DSC', SL, 0),
                        0))) KFCK,
         (SUM(DECODE(A.KCADD,
                        '-1',
                        DECODE(A.BILLTYPE,'LCD',HSJE,'CKD', HSJE, 'DSC', HSJE, 0),
                        0))) KFCKHSJE,
         (SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE, 'THD', SL, 0), 0))) THCK,
         (SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE, 'THD', HSJE, 0), 0))) THCKHSJE,
         (SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE, 'DBD', SL, 0), 0))) DBCK, --调拨出库
         (SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE, 'DBD', HSJE, 0), 0))) DBCKHSJE, --调拨出库              
         SUM(DECODE(A.KCADD, '0', DECODE(A.BILLTYPE, 'TJD', HSJE, 0), 0)) TJJE --调价金额  
    FROM DAT_GOODSJXC A
   WHERE to_char(A.rqsj, 'yyyy-mm-DD')>= '{1}'
        AND to_char(A.rqsj, 'yyyy-mm-DD')<= '{2}'
     and A.deptid in (SELECT distinct code FROM sys_dept where TYPE = '1')
   GROUP BY SUPID,PSSID)


SELECT DG.SUPID,NVL(TA.PSSID,TB.PSSID)PSSID,
decode(F_GETSUPPLIERNAME(TB.PSSID),
              '',
              F_GETSUPPLIERNAME(TA.PSSID),
              F_GETSUPPLIERNAME(TB.PSSID))PSSNAME,
       (SELECT SUPNAME FROM DOC_SUPPLIER where SUPID=DG.SUPID) SUPNAME,
       NVL(TA.QCKCSL, 0) QCKCSL,
       NVL(TA.QCKCHSJE, 0) QCKCHSJE,
       NVL(TB.CGRK, 0) CGRK,
       NVL(TB.CGRKHSJE, 0) CGRKHSJE,
       NVL(TB.KFSY, 0) KFSY,
       NVL(TB.KFSYHSJE, 0) KFSYHSJE,
       NVL(TB.THCK, 0) THCK,
       NVL(TB.THCKHSJE, 0) THCKHSJE,
       NVL(TB.KFCK, 0) KFCK,
       NVL(TB.KFCKHSJE, 0) KFCKHSJE,
       NVL(TB.TJJE, 0) TJJE, 
       NVL(TA.QCKCSL, 0)+((NVL(TB.CGRK, 0) + NVL(TB.KSTH, 0) + NVL(TB.KFSY, 0) + NVL(TB.DBRK, 0)) +
       (NVL(TB.KFCK, 0) + NVL(TB.THCK, 0) + NVL(TB.DBCK, 0))) QMKCSL,
       NVL(TB.TJJE, 0)+NVL(TA.QCKCHSJE, 0)+((NVL(TB.CGRKHSJE, 0) + NVL(TB.KSTHHSJE, 0) + NVL(TB.KFSYHSJE, 0) + NVL(TB.DBRKHSJE, 0)) +
       (NVL(TB.KFCKHSJE, 0) + NVL(TB.THCKHSJE, 0) + NVL(TB.DBCKHSJE, 0))) QMKCHSJE,
       NVL(TA.KCSL, 0) KCSL,NVL(TA.KCHSJE, 0) KCHSJE

  FROM TA full join TB on ta.SUPID = tb.SUPID AND TA.PSSID=TB.PSSID , DOC_SUPPLIER DG
 where (TA.SUPID = DG.SUPID 
    or TB.SUPID = DG.SUPID)) KF,
(WITH TA AS
 (SELECT SUPID,PSSID,
        SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{0}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCSL,
                    0)) QCKCSL,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{0}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCHSJE,
                    0)) QCKCHSJE,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{2}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCSL,
                    0)) KCSL,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{2}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCHSJE,
                    0)) KCHSJE
    FROM V_STOCKALL T
   WHERE (trunc(t.rq) = (to_date('{0}', 'YYYY-MM-DD')) or
         trunc(t.rq) = (to_date('{2}', 'YYYY-MM-DD')))
     and T.deptid in (SELECT distinct code
                        FROM sys_dept
                       where TYPE = '3'
                      /*AND F_CHK_DATARANGE(CODE, 'admin') = 'Y'*/
                      )
   GROUP BY SUPID,PSSID),
TB as
 (SELECT SUPID,PSSID,
         SUM(DECODE(A.KCADD,
                    '1',
                    DECODE(A.BILLTYPE, 'LCD', SL, 'CKD', SL, 'DSC', SL, 0),
                    0)) KSSL,
         SUM(DECODE(A.KCADD,
                    '1',
                    DECODE(A.BILLTYPE, 'LCD', HSJE, 'CKD', HSJE, 'DSC', HSJE, 0),
                    0)) KSSLHSJE,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'DBD', SL, 0), 0)) DBRK,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'DBD', HSJE, 0), 0)) DBRKHSJE,
         SUM(DECODE(A.BILLTYPE, 'SYD', SL, 0)) KSSY,
         SUM(DECODE(A.BILLTYPE, 'SYD', HSJE, 0)) KSSYHSJE,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'XST', SL, 0), 0)) XSTRK,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'XST', HSJE, 0), 0)) XSTRKHSJE,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'YKD', SL, 0), 0)) YKDRK,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'YKD', HSJE, 0), 0)) YKDRKHSJE,
         (SUM(DECODE(A.KCADD,
                        '-1',
                        DECODE(A.BILLTYPE, 'XSD', SL, 'DSH', SL, 'XSG', SL, 0),
                        0))) KSXH,
         (SUM(DECODE(A.KCADD,
                        '-1',
                        DECODE(A.BILLTYPE,'XSD', HSJE,'DSH',HSJE,'XSG',HSJE,0),
                        0))) KSXHHSJE,
         (SUM(DECODE(A.KCADD,
                        '-1',
                        DECODE(A.BILLTYPE, 'LTD', SL, 'DST', SL, 'XST', SL, 0),
                        0))) KSTH,
         SUM(DECODE(A.KCADD,
                        '-1',
                        DECODE(A.BILLTYPE,'LTD', HSJE,'DST',HSJE,'XST',HSJE,0),
                        0)) KSTHHSJE,
         (SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE, 'DBD', SL, 0), 0))) DBCK,
         (SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE, 'DBD', HSJE, 0), 0))) DBCKHSJE,
         SUM(DECODE(A.KCADD, '0', DECODE(A.BILLTYPE, 'TJD', HSJE, 0), 0)) TJJE --调价金额 
    FROM DAT_GOODSJXC A
   WHERE to_char(A.rqsj, 'yyyy-mm-DD')>= '{1}'
        AND to_char(A.rqsj, 'yyyy-mm-DD')<= '{2}'
     and A.deptid in (SELECT distinct code FROM sys_dept where TYPE = '3')
   GROUP BY SUPID,PSSID)


SELECT DG.SUPID,NVL(TA.PSSID,TB.PSSID)PSSID,
             decode(F_GETSUPPLIERNAME(TB.PSSID),
              '',
              F_GETSUPPLIERNAME(TA.PSSID),
              F_GETSUPPLIERNAME(TB.PSSID))PSSNAME,
       (SELECT SUPNAME FROM DOC_SUPPLIER where SUPID=DG.SUPID) SUPNAME,
       NVL(TA.QCKCSL, 0) QCKCSL,
       NVL(TA.QCKCHSJE, 0) QCKCHSJE,
       NVL(TB.KSXH, 0) KSXH,
       NVL(TB.KSXHHSJE, 0) KSXHHSJE,
       NVL(TB.KSSY, 0) KSSY,
       NVL(TB.KSSYHSJE, 0) KSSYHSJE,
       NVL(TB.XSTRK, 0) KSXT,
       NVL(TB.XSTRKHSJE, 0) KSXTHSJE,
       NVL(TB.YKDRK, 0) YKDRK,
       NVL(TB.YKDRKHSJE, 0) YKDRKHSJE,
       NVL(TB.TJJE, 0) TJJE,
       NVL(TB.XSTRK,0) XSTRK,
       NVL(TB.XSTRKHSJE,0)XSTRKHSJE,
       NVL(TA.QCKCSL, 0)+(NVL(TB.KSSL, 0) +NVL(TB.DBRK, 0) + NVL(TB.KSSY, 0)+NVL(TB.XSTRK, 0)+NVL(TB.YKDRK, 0)) +
      (NVL(TB.KSXH, 0) +NVL(TB.KSTH, 0) +NVL(TB.DBCK, 0)) QMKCSL,
       NVL(TB.TJJE, 0)+NVL(TA.QCKCHSJE, 0)+
       (NVL(TB.KSSLHSJE, 0) +NVL(TB.DBRKHSJE, 0) + NVL(TB.KSSYHSJE, 0)+NVL(TB.XSTRKHSJE, 0)+NVL(TB.YKDRKHSJE, 0)) +
       (NVL(TB.KSXHHSJE, 0) +NVL(TB.KSTHHSJE, 0) +NVL(TB.DBCKHSJE, 0)) QMKCHSJE, 
      NVL(TA.KCSL, 0) KCSL,NVL(TA.KCHSJE, 0) KCHSJE
FROM TA full join TB on ta.SUPID = tb.SUPID AND TA.PSSID=TB.PSSID,DOC_SUPPLIER DG
 where (TA.SUPID = DG.SUPID 
    or TB.SUPID = DG.SUPID)) ks 
where kf.SUPID=ks.SUPID(+) AND kf.PSSID=ks.PSSID(+)";
            string strWhere = " ";
            
            if (!PubFunc.StrIsEmpty(ddlSUPID.SelectedValue)) strWhere += " and KF.SUPID = '" + ddlSUPID.SelectedValue + "'";
            //DateTime time = DateTime.Parse(dpkDATE1.Text);
            String time1 = DateTime.Parse(dpkDATE1.Text).AddDays(-1).ToString("yyyyMMdd");
            //if (!PubFunc.StrIsEmpty(txbGDSEQ.Text)) strWhere += " and (TA.gdseq = '" + txbGDSEQ.Text + "' OR TB.GDSEQ='" + txbGDSEQ.Text + "')";
            
            if (strWhere != " ") strSql = strSql + strWhere;
            strSql += string.Format(" ORDER BY {0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            strSql = string.Format(strSql, time1, dpkDATE1.Text, dpkDATE2.Text);
            return strSql;
        }

        private void DataSearch()
        {
            int total = 0;

            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, GetSearchSql(), ref total);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
            //计算合计数量
            if (dtData != null && dtData.Rows.Count > 0)
            {
                decimal ZQCSLTotal = 0, ZQCHSJETotal = 0, ZTJTotal = 0, CGRKTotal = 0, CGRKHSJETotal = 0,THCKTotal = 0, THCKHSJETotal = 0;
                decimal KSXHTotal = 0, KSXHHSJETotal = 0, ZSYTotal = 0, ZSYHSJETotal = 0, KFKCSLTotal = 0, KFKCHSJETotal = 0,QMKCSLTotal = 0, QMKCHSJETotal = 0;


                foreach (DataRow row in dtData.Rows)
                {
                    THCKTotal+= Convert.ToDecimal(row["THCK"] ?? "0");
                    THCKHSJETotal+= Convert.ToDecimal(row["THCKHSJE"] ?? "0");
                    ZQCSLTotal += Convert.ToDecimal(row["ZQCSL"] ?? "0");
                    ZQCHSJETotal+= Convert.ToDecimal(row["ZQCHSJE"] ?? "0");
                    CGRKTotal+= Convert.ToDecimal(row["CGRK"] ?? "0");
                    CGRKHSJETotal+= Convert.ToDecimal(row["CGRKHSJE"] ?? "0");
                    
                    KSXHTotal+= Convert.ToDecimal(row["KSXH"] ?? "0");
                    KSXHHSJETotal+= Convert.ToDecimal(row["KSXHHSJE"] ?? "0");
                    KFKCSLTotal += Convert.ToDecimal(row["KFKCSL"] ?? "0");
                    KFKCHSJETotal += Convert.ToDecimal(row["KFKCHSJE"] ?? "0");
                    ZSYTotal += Convert.ToDecimal(row["ZSY"] ?? "0");
                    ZSYHSJETotal += Convert.ToDecimal(row["ZSYHSJE"] ?? "0");
                    ZTJTotal += Convert.ToDecimal(row["ZTJ"] ?? "0");
                    QMKCSLTotal += Convert.ToDecimal(row["QMKCSL"] ?? "0");
                    QMKCHSJETotal += Convert.ToDecimal(row["QMKCHSJE"] ?? "0");
                }
                JObject summary = new JObject();

                summary.Add("GDNAME", "本页合计");
                summary.Add("THCK", THCKTotal);
                summary.Add("THCKHSJE", THCKHSJETotal.ToString("F2"));   
                summary.Add("ZQCSL", ZQCSLTotal);
                summary.Add("ZQCHSJE", ZQCHSJETotal.ToString("F2"));
                summary.Add("CGRK", CGRKTotal);
                summary.Add("CGRKHSJE", CGRKHSJETotal.ToString("F2"));
                
                summary.Add("KSXH", KSXHTotal);
                summary.Add("KSXHHSJE", KSXHHSJETotal.ToString("F2"));
                summary.Add("KFKCSL", KFKCSLTotal);
                summary.Add("KFKCHSJE", KFKCHSJETotal.ToString("F2"));
                summary.Add("ZSY", ZSYTotal);
                summary.Add("ZSYHSJE", ZSYHSJETotal.ToString("F2"));
                summary.Add("ZTJ", ZTJTotal);
                summary.Add("QMKCSL", QMKCSLTotal);
                summary.Add("QMKCHSJE", QMKCHSJETotal.ToString("F2"));
                GridGoods.SummaryData = summary;
            }
        }

        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }
        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }
        protected void btClear_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormUser);
            dpkDATE1.SelectedDate = DateTime.Now.AddMonths(-1);
            dpkDATE2.SelectedDate = DateTime.Now;
        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            
            if (GridGoods.Rows.Count <= 0)
            {
                Alert.Show("请先选择要导出的商品进销存明细！");
                return;
            }
            string strSql = @"SELECT kf.SUPID 供应商编码,kf.SUPNAME 供应商名称,kf.pssname 配送商名称,
       NVL(kf.QCKCSL, 0)+NVL(ks.QCKCSL, 0) 期初数量,
       NVL(kf.QCKCHSJE, 0)+NVL(ks.QCKCHSJE, 0) 期初金额,   
       NVL(kf.KFSY, 0) + NVL(ks.KSSY, 0) 损益数量,
       NVL(kf.KFSYHSJE, 0) + NVL(ks.KSSYHSJE, 0) 损益金额,
       NVL(kf.CGRK, 0) + NVL(ks.YKDRK, 0) 采购数量,
       NVL(kf.CGRKHSJE, 0) +NVL(ks.YKDRKHSJE, 0) 采购金额,
       NVL(ks.KSXH, 0)-NVL(KS.XSTRK,0) 使用数量,
       NVL(ks.KSXHHSJE, 0)-NVL(KS.XSTRKHSJE,0) 使用金额,
       NVL(kf.THCK, 0) 退货数量,
       NVL(kf.THCKHSJE, 0) 退货金额,
       NVL(kf.TJJE, 0) +NVL(ks.TJJE, 0) 调价金额,
        NVL(kf.KCSL, 0)+NVL(KS.KCSL,0) 库存数量,
       NVL(kf.KCHSJE, 0)+NVL(KS.KCHSJE,0) 库存金额,
       NVL(kf.QMKCSL, 0)+NVL(KS.QMKCSL,0) 期末数量,
       NVL(kf.QMKCHSJE, 0)+NVL(KS.QMKCHSJE,0) 期末金额
       --NVL(ks.KSXT, 0) KSXT,
       --NVL(ks.KSXTHSJE, 0) KSXTHSJE,
       --NVL(kf.QMKCSL, 0) KFKCSL,
       --NVL(kf.QMKCSLHSJE, 0) KFKCSLHSJE,
       --NVL(ks.QMKCSL, 0) KSKCSL,
       --NVL(ks.QMKCSLHSJE, 0) KSKCSLHSJE
FROM (WITH TA AS
 (SELECT SUPID,PSSID,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{0}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCSL,
                    0)) QCKCSL,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{0}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCHSJE,
                    0)) QCKCHSJE,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{2}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCSL,
                    0)) KCSL,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{2}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCHSJE,
                    0)) KCHSJE
    FROM V_STOCKALL T
   WHERE (trunc(t.rq) = (to_date('{0}', 'YYYY-MM-DD')) or
         trunc(t.rq) = (to_date('{2}', 'YYYY-MM-DD')))
     and T.deptid in (SELECT distinct code
                        FROM sys_dept
                       where TYPE = '1'
                      /*AND F_CHK_DATARANGE(CODE, 'admin') = 'Y'*/
                      )
   GROUP BY SUPID,PSSID),
TB AS
 (SELECT SUPID,PSSID,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'RKD', SL, 0), 0)) CGRK,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'RKD', HSJE, 0), 0)) CGRKHSJE,
         SUM(DECODE(A.KCADD,
                    '1',
                    DECODE(A.BILLTYPE, 'LTD', SL, 'XST', SL, 'DST', SL, 0),
                    0)) KSTH,
         SUM(DECODE(A.KCADD,
                    '1',
                    DECODE(A.BILLTYPE,'LTD',HSJE,'XST', HSJE,'DST',HSJE, 0),
                    0)) KSTHHSJE,
         SUM(DECODE(A.BILLTYPE, 'SYD', SL, 0)) KFSY,
         SUM(DECODE(A.BILLTYPE, 'SYD', HSJE, 0)) KFSYHSJE,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'DBD', SL, 0), 0)) DBRK, --调拨入库
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'DBD', HSJE, 0), 0)) DBRKHSJE, --调拨入库               
         (SUM(DECODE(A.KCADD,
                        '-1',
                        DECODE(A.BILLTYPE, 'LCD', SL, 'CKD', SL, 'DSC', SL, 0),
                        0))) KFCK,
         (SUM(DECODE(A.KCADD,
                        '-1',
                        DECODE(A.BILLTYPE,'LCD',HSJE,'CKD', HSJE, 'DSC', HSJE, 0),
                        0))) KFCKHSJE,
         (SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE, 'THD', SL, 0), 0))) THCK,
         (SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE, 'THD', HSJE, 0), 0))) THCKHSJE,
         (SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE, 'DBD', SL, 0), 0))) DBCK, --调拨出库
         (SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE, 'DBD', HSJE, 0), 0))) DBCKHSJE, --调拨出库              
         SUM(DECODE(A.KCADD, '0', DECODE(A.BILLTYPE, 'TJD', HSJE, 0), 0)) TJJE --调价金额  
    FROM DAT_GOODSJXC A
   WHERE to_char(A.rqsj, 'yyyy-mm-DD')>= '{1}'
        AND to_char(A.rqsj, 'yyyy-mm-DD')<= '{2}'
     and A.deptid in (SELECT distinct code FROM sys_dept where TYPE = '1')
   GROUP BY SUPID,PSSID)


SELECT DG.SUPID,NVL(TA.PSSID,TB.PSSID)PSSID,
       decode(F_GETSUPPLIERNAME(TB.PSSID),
              '',
              F_GETSUPPLIERNAME(TA.PSSID),
              F_GETSUPPLIERNAME(TB.PSSID))PSSNAME,
       (SELECT SUPNAME FROM DOC_SUPPLIER where SUPID=DG.SUPID) SUPNAME,
       NVL(TA.QCKCSL, 0) QCKCSL,
       NVL(TA.QCKCHSJE, 0) QCKCHSJE,
       NVL(TB.CGRK, 0) CGRK,
       NVL(TB.CGRKHSJE, 0) CGRKHSJE,
       NVL(TB.KFSY, 0) KFSY,
       NVL(TB.KFSYHSJE, 0) KFSYHSJE,
       NVL(TB.THCK, 0) THCK,
       NVL(TB.THCKHSJE, 0) THCKHSJE,
       NVL(TB.KFCK, 0) KFCK,
       NVL(TB.KFCKHSJE, 0) KFCKHSJE,
       NVL(TB.TJJE, 0) TJJE, 
       NVL(TA.QCKCSL, 0)+((NVL(TB.CGRK, 0) + NVL(TB.KSTH, 0) + NVL(TB.KFSY, 0) + NVL(TB.DBRK, 0)) +
       (NVL(TB.KFCK, 0) + NVL(TB.THCK, 0) + NVL(TB.DBCK, 0))) QMKCSL,
       NVL(TB.TJJE, 0)+NVL(TA.QCKCHSJE, 0)+((NVL(TB.CGRKHSJE, 0) + NVL(TB.KSTHHSJE, 0) + NVL(TB.KFSYHSJE, 0) + NVL(TB.DBRKHSJE, 0)) +
       (NVL(TB.KFCKHSJE, 0) + NVL(TB.THCKHSJE, 0) + NVL(TB.DBCKHSJE, 0))) QMKCHSJE,
       NVL(TA.KCSL, 0) KCSL,NVL(TA.KCHSJE, 0) KCHSJE

  FROM TA full join TB on ta.SUPID = tb.SUPID AND TA.PSSID=TB.PSSID, DOC_SUPPLIER DG
 where (TA.SUPID = DG.SUPID 
    or TB.SUPID = DG.SUPID)) KF,
(WITH TA AS
 (SELECT SUPID,PSSID,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{0}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCSL,
                    0)) QCKCSL,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{0}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCHSJE,
                    0)) QCKCHSJE,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{2}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCSL,
                    0)) KCSL,
         SUM(DECODE(TO_CHAR(T.RQ, 'YYYY-MM-DD'),
                    TO_CHAR(TO_DATE('{2}', 'yyyy-MM-dd'), 'YYYY-MM-DD'),
                    T.KCHSJE,
                    0)) KCHSJE
    FROM V_STOCKALL T
   WHERE (trunc(t.rq) = (to_date('{0}', 'YYYY-MM-DD')) or
         trunc(t.rq) = (to_date('{2}', 'YYYY-MM-DD')))
     and T.deptid in (SELECT distinct code
                        FROM sys_dept
                       where TYPE = '3'
                      /*AND F_CHK_DATARANGE(CODE, 'admin') = 'Y'*/
                      )
   GROUP BY SUPID,PSSID),
TB as
 (SELECT SUPID,PSSID,
         SUM(DECODE(A.KCADD,
                    '1',
                    DECODE(A.BILLTYPE, 'LCD', SL, 'CKD', SL, 'DSC', SL, 0),
                    0)) KSSL,
         SUM(DECODE(A.KCADD,
                    '1',
                    DECODE(A.BILLTYPE, 'LCD', HSJE, 'CKD', HSJE, 'DSC', HSJE, 0),
                    0)) KSSLHSJE,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'DBD', SL, 0), 0)) DBRK,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'DBD', HSJE, 0), 0)) DBRKHSJE,
         SUM(DECODE(A.BILLTYPE, 'SYD', SL, 0)) KSSY,
         SUM(DECODE(A.BILLTYPE, 'SYD', HSJE, 0)) KSSYHSJE,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'XST', SL, 0), 0)) XSTRK,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'XST', HSJE, 0), 0)) XSTRKHSJE,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'YKD', SL, 0), 0)) YKDRK,
         SUM(DECODE(A.KCADD, '1', DECODE(A.BILLTYPE, 'YKD', HSJE, 0), 0)) YKDRKHSJE,
         (SUM(DECODE(A.KCADD,
                        '-1',
                        DECODE(A.BILLTYPE, 'XSD', SL, 'DSH', SL, 'XSG', SL, 0),
                        0))) KSXH,
         (SUM(DECODE(A.KCADD,
                        '-1',
                        DECODE(A.BILLTYPE,'XSD', HSJE,'DSH',HSJE,'XSG',HSJE,0),
                        0))) KSXHHSJE,
         (SUM(DECODE(A.KCADD,
                        '-1',
                        DECODE(A.BILLTYPE, 'LTD', SL, 'DST', SL, 'XST', SL, 0),
                        0))) KSTH,
         SUM(DECODE(A.KCADD,
                        '-1',
                        DECODE(A.BILLTYPE,'LTD', HSJE,'DST',HSJE,'XST',HSJE,0),
                        0)) KSTHHSJE,
         (SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE, 'DBD', SL, 0), 0))) DBCK,
         (SUM(DECODE(A.KCADD, '-1', DECODE(A.BILLTYPE, 'DBD', HSJE, 0), 0))) DBCKHSJE,
         SUM(DECODE(A.KCADD, '0', DECODE(A.BILLTYPE, 'TJD', HSJE, 0), 0)) TJJE --调价金额 
    FROM DAT_GOODSJXC A
   WHERE to_char(A.rqsj, 'yyyy-mm-DD')>= '{1}'
        AND to_char(A.rqsj, 'yyyy-mm-DD')<= '{2}'
     and A.deptid in (SELECT distinct code FROM sys_dept where TYPE = '3')
   GROUP BY SUPID,PSSID)


SELECT DG.SUPID,NVL(TA.PSSID,TB.PSSID)PSSID,
       decode(F_GETSUPPLIERNAME(TB.PSSID),
              '',
              F_GETSUPPLIERNAME(TA.PSSID),
              F_GETSUPPLIERNAME(TB.PSSID))PSSNAME,
       (SELECT SUPNAME FROM DOC_SUPPLIER where SUPID=DG.SUPID) SUPNAME,
       NVL(TA.QCKCSL, 0) QCKCSL,
       NVL(TA.QCKCHSJE, 0) QCKCHSJE,
       NVL(TB.KSXH, 0) KSXH,
       NVL(TB.KSXHHSJE, 0) KSXHHSJE,
       NVL(TB.KSSY, 0) KSSY,
       NVL(TB.KSSYHSJE, 0) KSSYHSJE,
       NVL(TB.XSTRK, 0) KSXT,
       NVL(TB.XSTRKHSJE, 0) KSXTHSJE,
       NVL(TB.YKDRK, 0) YKDRK,
       NVL(TB.YKDRKHSJE, 0) YKDRKHSJE,
       NVL(TB.TJJE, 0) TJJE,
       NVL(TB.XSTRK,0) XSTRK,
       NVL(TB.XSTRKHSJE,0)XSTRKHSJE,
       NVL(TA.QCKCSL, 0)+(NVL(TB.KSSL, 0) +NVL(TB.DBRK, 0) + NVL(TB.KSSY, 0)+NVL(TB.XSTRK, 0)+NVL(TB.YKDRK, 0)) +
      (NVL(TB.KSXH, 0) +NVL(TB.KSTH, 0) +NVL(TB.DBCK, 0)) QMKCSL,
       NVL(TB.TJJE, 0)+NVL(TA.QCKCHSJE, 0)+
       (NVL(TB.KSSLHSJE, 0) +NVL(TB.DBRKHSJE, 0) + NVL(TB.KSSYHSJE, 0)+NVL(TB.XSTRKHSJE, 0)+NVL(TB.YKDRKHSJE, 0)) +
       (NVL(TB.KSXHHSJE, 0) +NVL(TB.KSTHHSJE, 0) +NVL(TB.DBCKHSJE, 0)) QMKCHSJE, 
      NVL(TA.KCSL, 0) KCSL,NVL(TA.KCHSJE, 0) KCHSJE
FROM TA full join TB on ta.SUPID = tb.SUPID AND TA.PSSID=TB.PSSID,DOC_SUPPLIER DG
 where (TA.SUPID = DG.SUPID 
    or TB.SUPID = DG.SUPID)) ks 
where kf.SUPID=ks.SUPID(+)AND kf.PSSID=ks.PSSID(+)";
            string strWhere = " ";
            
            if (!PubFunc.StrIsEmpty(ddlSUPID.SelectedValue)) strWhere += " and KF.SUPID = '" + ddlSUPID.SelectedValue + "'";
            //DateTime time = DateTime.Parse(dpkDATE1.Text);
            String time1 = DateTime.Parse(dpkDATE1.Text).AddDays(-1).ToString("yyyyMMdd");
            //if (!PubFunc.StrIsEmpty(txbGDSEQ.Text)) strWhere += " and (TA.gdseq = '" + txbGDSEQ.Text + "' OR TB.GDSEQ='" + txbGDSEQ.Text + "')";
            
            if (strWhere != " ") strSql = strSql + strWhere;
            strSql += string.Format(" ORDER BY {0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            strSql = string.Format(strSql, time1, dpkDATE1.Text, dpkDATE2.Text);
            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            ExcelHelper.ExportByWeb(dt, "供应商进销存信息", "供应商进销存信息_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;
            DataSearch();
        }

    }



}
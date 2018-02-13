﻿using FineUIPro;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPWorkbench
{
    public partial class IndexBusiness : PageBase
    {
        static string xq;
        protected string MyMemo = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    xq = DbHelperOra.GetSingle("select value from SYS_PARA where code = 'YXQZDAYS'").ToString();
                }
                catch
                {
                    xq = "30";
                }
                GridLoadData();
                btnSearch();
                GridGoodsList.EmptyText = String.Format("<img src=\"{0}\" alt=\"No Data Found!\"/>", ResolveUrl("~/res/images/no_data_found.png"));
                YsSearch();
            }
        }
        private void GridLoadData()
        {

            try
            {

                string sql = @"SELECT A.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,B.HSJJ,A.ZGKC,A.ZDKC,DECODE(A.NUM1,0,'否',DECODE(A.DSNUM,0,'否','是')) ISDS,
                               NVL((SELECT SUM(KCSL)FROM DAT_GOODSSTOCK K WHERE KCSL > 0 AND K.DEPTID = A.DEPTID AND K.GDSEQ = A.GDSEQ),0) KCSL
                          FROM DOC_GOODSCFG A,DOC_GOODS B
                         WHERE A.GDSEQ = B.GDSEQ
                         AND A.DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE F_CHK_DATARANGE(CODE, '{0}') = 'Y')";
                int total = 0;
                DataTable dt = PubFunc.DbGetPage(GridGoodsList.PageIndex, GridGoodsList.PageSize, String.Format(sql, UserAction.UserID), ref total);
                GridGoodsList.RecordCount = total;
                GridGoodsList.DataSource = dt;
                GridGoodsList.DataBind();
            }
            catch (Exception ex)
            {
                //有异常也不抛出,防止影响系统主界面运行 c 20150414
                return;
            }

            //GridToDoList.DataSource = dtDaiBan;
            //GridToDoList.DataBind();
            //GridBillStatus.DataSource = dtDD;
            // GridBillStatus.DataBind();

            //if (dtYuJing != null && dtYuJing.Rows.Count > 0)
            //{
            //    List<object> list = new List<object>();
            //    foreach (DataRow row in dtYuJing.Rows)
            //    {
            //if (row["TOTAL"].ToString() == "0") continue;
            //FineUIPro.HyperLink link = new FineUIPro.HyperLink();
            //link.ID = row["DOTYPE"].ToString();
            //link.Text = string.Format(row["INSTRUCTIONS"].ToString(), row["TOTAL"].ToString());
            //link.NavigateUrl = "javascript:" + mainTabStrip.GetAddTabReference(row["FUNCID"].ToString(), row["RUNWHAT"].ToString(), row["FUNCNAME"].ToString(), "~/extjs/res/ext-theme-classic/images/tree/leaf.gif", true);
            //FineUIPro.LinkButton btnLink = new FineUIPro.LinkButton();
            //btnLink.ID = "btn_" + row["SEQNO"].ToString();
            //btnLink.ShowLabel = false;
            //btnLink.Label = row["SEQNO"].ToString() + "," + row["FUNCID"].ToString() + "," + row["RUNWHAT"].ToString() + "," + row["FUNCNAME"].ToString() + "," + row["EXECTYPE"].ToString();
            //if (row["DOTYPE"].ToString() == "DO_2")
            //{
            //    list.Add(new
            //    {
            //        PARA = row["SEQNO"].ToString() + "," + row["FUNCID"].ToString() + "," + row["RUNWHAT"].ToString() + "," + row["FUNCNAME"].ToString() + "," + row["EXECTYPE"].ToString(),
            //        INSTRUCTIONS = "部门『" + row["DEPT"].ToString() + "』" + string.Format(row["INSTRUCTIONS"].ToString(), row["TOTAL"].ToString())
            //    });
            //btnLink.Text = "部门『" + row["DEPT"].ToString() + "』" + string.Format(row["INSTRUCTIONS"].ToString(), row["TOTAL"].ToString());
            //}
            //else
            //{
            //    if (row["DOTYPE"].ToString() == "ksjxq")
            //    {
            //        string[] total_number = row["TOTAL"].ToString().Split(';');
            //        list.Add(new
            //        {
            //            PARA = row["SEQNO"].ToString() + "," + row["FUNCID"].ToString() + "," + row["RUNWHAT"].ToString() + "," + row["FUNCNAME"].ToString() + "," + row["EXECTYPE"].ToString(),
            //            INSTRUCTIONS = string.Format(row["INSTRUCTIONS"].ToString(), total_number[0], total_number[1])
            //        });
            //    }
            //    else
            //    if (row["DOTYPE"].ToString() != "DO_1")
            //    {
            //        list.Add(new
            //        {
            //            PARA = row["SEQNO"].ToString() + "," + row["FUNCID"].ToString() + "," + row["RUNWHAT"].ToString() + "," + row["FUNCNAME"].ToString() + "," + row["EXECTYPE"].ToString(),
            //            INSTRUCTIONS = string.Format(row["INSTRUCTIONS"].ToString(), row["TOTAL"].ToString())
            //        });
            //        // btnLink.Text = string.Format(row["INSTRUCTIONS"].ToString(), row["TOTAL"].ToString());
            //    }
            //}

            ////btnLink.OnClientClick = mainTabStrip.GetAddTabReference(row["FUNCID"].ToString(), row["RUNWHAT"].ToString(), row["FUNCNAME"].ToString(), "~/extjs/res/ext-theme-classic/images/tree/leaf.gif", true);
            //btnLink.Click += new EventHandler(btnLink_Click);
            //btnLink.EnablePostBack = true;
            //Panel3.Items.Add(btnLink);
            //}
            //GridYuJing.DataSource = list;
            //GridYuJing.DataBind();
            //}
        }
        public string getPersonNamesString(string PersonIDs)
        {
            string retval = string.Empty;
            string strSQL = string.Empty;
            if (PersonIDs.Length > 0)
            {
                strSQL = "SELECT userName From sys_operuser where userid in ('" + PersonIDs.Replace(",", "','") + "')";
                DataTable dtuser = DbHelperOra.Query(strSQL).Tables[0];
                if (dtuser.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtuser.Rows)
                    {
                        retval += dr["UserName"].ToString() + ",";
                    }
                }
            }
            return retval.TrimEnd(',');
        }
        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            GridList.SortDirection = e.SortDirection;
            GridList.SortField = e.SortField;

            DataTable table = PubFunc.GridDataGet(GridList);
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridList.SortField, GridList.SortDirection);
            GridList.DataSource = view1;
            GridList.DataBind();
        }
        protected void btnSearch()
        {
            string strSQL = @"select seqno,
                                                   isprivate,
                                                   (select username from sys_operuser where userid = m.lrr) as lrName,
                                                   lrrq,
                                                   title,
                                                   Memo,
                                                   waringRQ,
                                                   lookper,
                                                   (select username from sys_operuser where userid = m.closeper) as closename,
                                                   closerq,
                                                   (select mu.flag from SYS_MYMEMO_USER mu where mu.seqno = m.seqno and mu.lookper='" + UserAction.UserID + @"' and mu.isprivate='S') as mystatus,
                                                   case
                                                     when flag = 'N' then
                                                      '新备忘'
                                                     when flag = 'C' then
                                                      '已取消'
                                                     else
                                                      '已完成'
                                                   end as status
                                              from sys_mymemo m
                                             where (lrr = '" + UserAction.UserID + @"' or
                                                   LookPer || ',' like '%" + UserAction.UserID + @",%')
                                               and waringRQ =
                                                   to_date('" + DateTime.Now.ToString("yyyy-MM-dd") + @"', 'YYYY-MM-DD')
                                             order by lrrq desc";
            int total = 0;
            DataTable dt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSQL, ref total);
            GridList.DataSource = dt;
            GridList.RecordCount = total;
            GridList.DataBind();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["MYSTATUS"].ToString() == "N" || row["MYSTATUS"].ToString() == "Y")
                    {
                        hfdSeqNo.Text = row["SEQNO"].ToString();
                        MyMemo = row["MEMO"].ToString();
                        WindowMemo.Title = "系统公告通知 -【 " + row["TITLE"].ToString() + "】";
                        WindowMemo.Hidden = false;
                        break;
                    }
                }
            }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            GridLoadData();
            btnSearch();
        }

        protected void GridGoodsList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoodsList.PageIndex = e.NewPageIndex;
            GridLoadData();
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            DbHelperOra.ExecuteSql(string.Format("UPDATE SYS_MYMEMO_USER SET FLAG='C',LOOKRQ=SYSDATE WHERE SEQNO='{0}' AND LOOKPER='{1}'", hfdSeqNo.Text, UserAction.UserID));
            WindowMemo.Hidden = true;
        }
        protected void YsSearch()
        {
            string currentMonth = DateTime.Now.ToString("MM");
            string startTime = string.Empty;
            string endTime = string.Empty;
            string ACCOUNTDAY = Doc.DbGetSysPara("ACCOUNTDAY");
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            if (ACCOUNTDAY == "31")
            {
                startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + currentMonth + "-01");
                endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + currentMonth + "-01").AddMonths(1).AddDays(-1);
            }
            else
            {
                startDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + currentMonth + "-" + ACCOUNTDAY).AddMonths(-1);
                endDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy") + "-" + currentMonth + "-" + ACCOUNTDAY);

            }
            string strSql = string.Format(@"SELECT A.DEPTID,TO_CHAR(A.RQSJ, 'YYYY-MM') Monthly, SUM(JSJE + THJE) TOTAL,SUM(NVL(B.YSJE, 0)) YSTOTAL
                                                    FROM VIEW_JS A,
                                                        (SELECT to_char(A.YSRQ, 'YYYY-MM') Monthly, SUM(B.HSJE) YSJE
                                                            FROM DAT_YS_DOC A, DAT_YS_COM B
                                                            WHERE A.SEQNO = B.SEQNO
                                                            AND A.FLAG = 'Y' AND A.DEPTID = '{2}'
                                                            AND A.YSRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                TO_DATE('{1}', 'YYYY-MM-DD')
                                                            GROUP BY to_char(A.YSRQ, 'YYYY-MM')) B
                                                    WHERE A.DEPTID = '{2}' AND RQSJ BETWEEN TO_DATE('{0}', 'yyyy-MM-dd') AND
                                                        TO_DATE('{1}', 'yyyy-MM-dd') + 1
                                                        AND TO_CHAR(A.RQSJ, 'YYYY-MM') = B.Monthly(+)
                                                    GROUP BY a.deptid,TO_CHAR(A.RQSJ, 'YYYY-MM')
                                                ", startDate.AddMonths(-2).ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), UserAction.UserDept);
            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            String ysje, xsje;
            if (dt != null && dt.Rows.Count > 0)
            {
                ysje = dt.Rows[0]["YSTOTAL"].ToString();
                xsje = dt.Rows[0]["TOTAL"].ToString();
            }
            else
            {
                ysje = "0";
                xsje = "0";
            }
            PageContext.RegisterStartupScript("reloaddata(" + ysje + "," + xsje + ");");
        }

        protected void GridGoodsList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;

            decimal upLine = Convert.ToDecimal(row["ZGKC"]);
            decimal bottomLine = Convert.ToDecimal(row["ZDKC"]);
            decimal kcsl = Convert.ToDecimal(row["KCSL"]);

            if (kcsl > upLine && upLine != 0 && upLine != 0)
            {
                e.RowCssClass = "color-yellow";
            }
            else if (kcsl < bottomLine)
            {
                e.RowCssClass = "color-red";
            }
        }
    }
}
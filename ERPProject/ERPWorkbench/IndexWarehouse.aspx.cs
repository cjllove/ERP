﻿using FineUIPro;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPWorkbench
{
    #region 新工作台后代代码
    public partial class IndexWarehouse : PageBase
    {
        protected string MyMemo = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GridLoadData();
                btnSearch();
                GridToDoList.EmptyText = String.Format("<img src=\"{0}\" alt=\"No Data Found!\"/>", ResolveUrl("~/res/images/no_data_found.png"));
                Search();

                //PanelDelivery.EnableIFrame = true;
                //PanelDelivery.IFrameUrl = "/ERPBasic/DataDeptJdt.aspx?user=" + UserAction.UserID;
            }
        }
        private void GridLoadData()
        {
            string sql_daiban = @"SELECT *
                                              FROM V_TODOLIST
                                             WHERE F_CHK_ROLELIST(ROLELIST, '{0}') = 'Y'
                                               AND PARA IN (SELECT L.SEQNO
                                                              FROM DAT_SL_DOC L
                                                             WHERE F_CHK_AUDIT(NVL(L.CATID, '2'), '{1}') = 'Y')";

            string sql_dd = @"SELECT *
                                  FROM (SELECT SEQNO,
                                               BILLNO,
                                               BILLTYPE,
                                               DECODE(FLAG,
                                                      'M',
                                                      '新单',
                                                      'N',
                                                      '已提交',
                                                      'Y',
                                                      '已审核',
                                                      'R',
                                                      '已驳回',
                                                      'G',
                                                      '已结算') FLAG,
                                               XDRQ DJRQ
                                          FROM DAT_DD_DOC
                                         WHERE flag <> 'M'
                                           and F_CHK_DATARANGE(DEPTDH, '{0}') = 'Y'
                                         ORDER BY SEQNO DESC)
                                 WHERE ROWNUM < 18
                                UNION
                                SELECT *
                                  FROM (SELECT SEQNO,
                                               BILLNO,
                                               BILLTYPE,
                                               DECODE(FLAG,
                                                      'M',
                                                      '新单',
                                                      'N',
                                                      '已提交',
                                                      'S',
                                                      '已审批',
                                                      'B',
                                                      '部分出库',
                                                      'Y',
                                                      '全部出库',
                                                      'R',
                                                      '已驳回',
                                                      'G',
                                                      '已结算') FLAG,
                                               XSRQ DJRQ
                                          FROM DAT_SL_DOC
                                         WHERE flag <> 'M'
                                           AND F_CHK_DATARANGE(DEPTID, '{0}') = 'Y'
                                           AND F_CHK_FUNCRANGE('6101','{0}') ='Y'
                                         ORDER BY SEQNO DESC)
                                 WHERE ROWNUM < 18";

            string sql_yujing = @"SELECT B.SEQNO,C.NAME INSTRUCTIONS,F.FUNCID,F.FUNCNAME,A.DOTYPE,
                                         A.EXECTYPE,F.RUNWHAT,B.PARA TOTAL,F_GETDEPTNAME(B.DEPTID) DEPT
                                    FROM DAT_DO_TYPE A,DAT_DO_LIST B,
                                         SYS_FUNCTION F,
                                         (SELECT T.CODE, T.NAME
                                            FROM SYS_CODEVALUE T
                                            WHERE TYPE LIKE 'DIC_DOTYPE') C
                                    WHERE A.DOTYPE IN (SELECT DOTYPE FROM DAT_DO_TYPE WHERE EXECTYPE='0')
                                    AND A.DOTYPE=B.DOTYPE
                                    AND B.FLAG='N'
                                    AND A.DOTYPE = C.CODE(+)
                                    AND A.FUNCID = F.FUNCID
                                    AND F_CHK_ROLELIST(B.ROLELIST, '{0}') = 'Y'";
            DataTable dtDaiBan = new DataTable();
            DataTable dtDD = new DataTable();
            DataTable dtYuJing = new DataTable();
            try
            {
                dtDaiBan = DbHelperOra.Query(string.Format("select * from (" + sql_daiban + ") where rownum<10 order by scrq desc", UserAction.UserRole, UserAction.UserID)).Tables[0];
                dtDD = DbHelperOra.Query(string.Format(sql_dd, UserAction.UserID)).Tables[0];
                dtYuJing = DbHelperOra.Query(string.Format(sql_yujing, UserAction.UserRole)).Tables[0];
            }
            catch (Exception ex)
            {
                //有异常也不抛出,防止影响系统主界面运行 c 20150414
                return;
            }

            GridToDoList.DataSource = dtDaiBan;
            GridToDoList.DataBind();
            //GridBillStatus.DataSource = dtDD;
            // GridBillStatus.DataBind();

            if (dtYuJing != null && dtYuJing.Rows.Count > 0)
            {
                List<object> list = new List<object>();
                foreach (DataRow row in dtYuJing.Rows)
                {
                    //if (row["TOTAL"].ToString() == "0") continue;
                    //FineUIPro.HyperLink link = new FineUIPro.HyperLink();
                    //link.ID = row["DOTYPE"].ToString();
                    //link.Text = string.Format(row["INSTRUCTIONS"].ToString(), row["TOTAL"].ToString());
                    //link.NavigateUrl = "javascript:" + mainTabStrip.GetAddTabReference(row["FUNCID"].ToString(), row["RUNWHAT"].ToString(), row["FUNCNAME"].ToString(), "~/extjs/res/ext-theme-classic/images/tree/leaf.gif", true);
                    //FineUIPro.LinkButton btnLink = new FineUIPro.LinkButton();
                    //btnLink.ID = "btn_" + row["SEQNO"].ToString();
                    //btnLink.ShowLabel = false;
                    //btnLink.Label = row["SEQNO"].ToString() + "," + row["FUNCID"].ToString() + "," + row["RUNWHAT"].ToString() + "," + row["FUNCNAME"].ToString() + "," + row["EXECTYPE"].ToString();
                    if (row["DOTYPE"].ToString() == "DO_2")
                    {
                        list.Add(new
                        {
                            PARA = row["SEQNO"].ToString() + "," + row["FUNCID"].ToString() + "," + row["RUNWHAT"].ToString() + "," + row["FUNCNAME"].ToString() + "," + row["EXECTYPE"].ToString(),
                            INSTRUCTIONS = "部门『" + row["DEPT"].ToString() + "』" + string.Format(row["INSTRUCTIONS"].ToString(), row["TOTAL"].ToString())
                        });
                        //btnLink.Text = "部门『" + row["DEPT"].ToString() + "』" + string.Format(row["INSTRUCTIONS"].ToString(), row["TOTAL"].ToString());
                    }
                    else
                    {
                        if (row["DOTYPE"].ToString() == "DO_1")
                        {
                            string[] total_number = row["TOTAL"].ToString().Split(';');
                            list.Add(new
                            {
                                PARA = row["SEQNO"].ToString() + "," + row["FUNCID"].ToString() + "," + row["RUNWHAT"].ToString() + "," + row["FUNCNAME"].ToString() + "," + row["EXECTYPE"].ToString(),
                                INSTRUCTIONS = string.Format(row["INSTRUCTIONS"].ToString(), total_number[0] == "" ? "0" : total_number[0], total_number[1] == "" ? "0" : total_number[1])
                            });
                        }
                        else
                        {
                            list.Add(new
                            {
                                PARA = row["SEQNO"].ToString() + "," + row["FUNCID"].ToString() + "," + row["RUNWHAT"].ToString() + "," + row["FUNCNAME"].ToString() + "," + row["EXECTYPE"].ToString(),
                                INSTRUCTIONS = string.Format(row["INSTRUCTIONS"].ToString(), row["TOTAL"].ToString())
                            });
                            // btnLink.Text = string.Format(row["INSTRUCTIONS"].ToString(), row["TOTAL"].ToString());
                        }

                    }

                    ////btnLink.OnClientClick = mainTabStrip.GetAddTabReference(row["FUNCID"].ToString(), row["RUNWHAT"].ToString(), row["FUNCNAME"].ToString(), "~/extjs/res/ext-theme-classic/images/tree/leaf.gif", true);
                    //btnLink.Click += new EventHandler(btnLink_Click);
                    //btnLink.EnablePostBack = true;
                    //Panel3.Items.Add(btnLink);
                }
                GridYuJing.DataSource = list;
                GridYuJing.DataBind();
            }
        }
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            GridLoadData();
            btnSearch();
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
        protected void GridList_RowCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "TODOLINK")
            {
                GridRow row = GridToDoList.Rows[e.RowIndex];
                string url = row.Values[3].ToString();
                if (url.IndexOf('?') > 0)
                {
                    url += "&pid=" + row.Values[4].ToString();
                }
                else
                {
                    url += "?pid=" + row.Values[4].ToString();
                }
                url = url.Substring(1);
                PageContext.RegisterStartupScript("openTODOLINK('" + row.Values[1].ToString() + "','" + url + "','" + row.Values[2].ToString() + "')");
                //PageContext.RegisterStartupScript(mainTabStrip.GetRemoveTabReference(row.Values[1].ToString()) + mainTabStrip.GetAddTabReference(row.Values[1].ToString(), url, row.Values[2].ToString(), "~/extjs/res/ext-theme-classic/images/tree/leaf.gif", true));
            }
            else if (e.CommandName == "YUJING")
            {
                GridRow row = GridYuJing.Rows[e.RowIndex];
                string[] para = row.DataKeys[0].ToString().Split(',');
                if (para.Length != 5) return;
                if (para[4] == "0")
                {
                    OracleParameter[] parameters_db = {
                                               new OracleParameter("V_SEQNO", OracleDbType.Varchar2),
                                               new OracleParameter("V_USERID", OracleDbType.Varchar2) };
                    parameters_db[0].Value = para[0];
                    parameters_db[1].Value = UserAction.UserID;
                    DbHelperOra.RunProcedure("MAINT.P_DO_EXEC", parameters_db);
                }
                string url = para[2].Substring(1);
                PageContext.RegisterStartupScript("openTODOLINK('" + para[1] + "','" + url + "','" + para[3] + "')");
            }
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
        protected void btnClose_Click(object sender, EventArgs e)
        {
            DbHelperOra.ExecuteSql(string.Format("UPDATE SYS_MYMEMO_USER SET FLAG='C',LOOKRQ=SYSDATE WHERE SEQNO='{0}' AND LOOKPER='{1}'", hfdSeqNo.Text, UserAction.UserID));
            WindowMemo.Hidden = true;
        }
        protected void Search()
        {
            String wcl = "";
            String sql = @"SELECT round(SUM(DECODE(NVL(SB.DEPTID,'#'),'#',0,1))/COUNT(1),4)*100
                        FROM
                        (SELECT A.CODE
                        FROM SYS_DEPT A
                        WHERE DECODE(to_char(TO_DATE(sysdate,'yyyy-MM-dd'),'day'),'星期一',A.DHZQ1,'星期二',A.DHZQ2,'星期三',A.DHZQ3,'星期四',A.DHZQ4,'星期五',A.DHZQ5,'星期六',A.DHZQ6,A.DHZQ7) <> 'N')
                        SA,
                        (
                        SELECT DISTINCT B.DEPTID
                        FROM DAT_GOODSJXC B
                        WHERE B.RQSJ > TO_DATE(sysdate,'yyyy-MM-dd')
                        ) SB
                        WHERE SA.CODE = SB.DEPTID(+)";
            try
            {
                wcl = DbHelperOra.GetSingle(sql).ToString() + "";
            }
            catch
            {
                wcl = "0.00";
            }
            hfdWcl.Text = wcl;
            PageContext.RegisterStartupScript("getEcharsData();");
        }

    }
    #endregion

}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System.Data;
using System.Collections.Specialized;
using System.Text;

namespace ERPProject.ERPApply
{
    public partial class DepartmentsItemsCollect : BillBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                billSearch();
            }
        }
        protected override void billSearch()
        {
            //string strSql = @"SELECT A.GDSEQ,A.GDNAME,A.GDSPEC, 
            //    F_GETUNITNAME(A.UNIT) UNITNAME, 
            //    F_GETDEPTNAME(B.DEPTID) DEPTIDNAME,
            //    B.DEPTID,B.ZDKC,B.ZGKC,B.DSNUM, B.NUM1,B.NUM2,B.NUM3,C.OPERUSER, 
            //    TO_CHAR(C.OPERDATE,'YYYY-MM-DD') OPERDATE,C.DEFSL，
            //    F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,PIZNO 
            //    FROM DOC_GOODS A, DOC_GOODSCFG B, DOC_MYGOODS C 
            //    WHERE A.GDSEQ = B.GDSEQ 
            //    AND A.GDSEQ = C.GDSEQ 
            //    AND B.GDSEQ=C.GDSEQ AND B.DEPTID=C.DEPTID";
            string strSql = @"SELECT A.GDSEQ,A.GDNAME,A.GDSPEC, 
                F_GETUNITNAME(A.UNIT) UNITNAME, 
                F_GETDEPTNAME(B.DEPTID) DEPTIDNAME,
                B.DEPTID,B.ZDKC,B.ZGKC,B.DSNUM, B.NUM1,B.NUM2,B.NUM3,f_getusername(C.OPERUSER) OPERUSER, 
                TO_CHAR(C.OPERDATE,'YYYY-MM-DD') OPERDATE,C.DEFSL,PIZNO,f_getproducername(A.PRODUCER) PRODUCERNAME
                FROM DOC_GOODS A, DOC_GOODSCFG B, DOC_MYGOODS C 
                WHERE A.GDSEQ = B.GDSEQ 
                AND A.GDSEQ = C.GDSEQ 
                AND B.GDSEQ=C.GDSEQ AND B.DEPTID=C.DEPTID";
            string strSearch = "";

            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND B.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }

            if (tgbGDSEQ.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND (A.GDSEQ   LIKE '%{0}%' OR A.GDNAME LIKE '%{0}%') ", tgbGDSEQ.Text.Trim());
            }

            if (rblRange.SelectedValue == "1")
            {
                strSearch += string.Format(" AND  C.OPERUSER='{0}'", UserAction.UserID);
            }

            strSql += strSearch;

            //int total = 0;
            //DataTable dt = PubFunc.DbGetPage(GridList.PageIndex, GridList.PageSize, strSql, ref total);
            //DataTable dt
            //GridList.DataSource = dt;
            //GridList.RecordCount = total;
            //GridList.DataBind();

            PageContext.RegisterStartupScript(GridList.GetRejectChangesReference());
            DataTable dtBill = DbHelperOra.Query(strSql).Tables[0];
            PubFunc.GridRowAdd(GridList, dtBill);
        }
        private void DataInit()
        {
            //使用部门下拉表
            PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", lstDEPTID);
        }
        protected void tgbGDSEQ_TriggerClick(object sender, EventArgs e)
        {
            billSearch();
        }
        protected override void billDelRow()
        {
            if (GridList.Rows.Count == 0) return;
            if (GridList.SelectedCell == null) return;
            string GETdeptid = "";
            string GETgdseq = "";
            List<Dictionary<string, object>> newDict = GridList.GetNewAddedList();
            int rowIndex = GridList.SelectedRowIndex;
            if ((GridList.DataKeys.Count - 1) < rowIndex)
            {

                Alert.Show("该商品未收藏");
                return;
            }
            else
            {
                GETdeptid = GridList.DataKeys[rowIndex][0].ToString();
                GETgdseq = GridList.DataKeys[rowIndex][1].ToString();
                if (DbHelperOra.Exists("select 1 from DOC_MYGOODS where DEPTID='" + GETdeptid + "' and GDSEQ ='" + GETgdseq + "'"))
                {
                    string Sql = string.Empty;
                    Sql = "DELETE FROM DOC_MYGOODS WHERE DEPTID = '" + GETdeptid + "' and GDSEQ = '" + GETgdseq + "'";
                    DbHelperOra.ExecuteSql(Sql);
                    Alert.Show("已取消收藏");
                    PageContext.RegisterStartupScript(GridList.GetDeleteRowReference(rowIndex));
                    //billSearch();
                }
                else
                {
                    Alert.Show("未保存商品无需删除");
                    return;
                }
            }
        }
        protected override void billGoods()
        {
            if (lstDEPTID.SelectedItem == null || lstDEPTID.SelectedIndex == 0)
            {
                Alert.Show("请选择当前科室！");
                return;
            }
            string url = "~/ERPQuery/GoodsWindow.aspx?bm=" + lstDEPTID.SelectedValue + "&cx=&su=";
            PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "商品信息查询"));
        }
        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            DataTable dt = GetGoods(hfdValue.Text);
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns.Add("DEPTIDNAME", Type.GetType("System.String"));
                dt.Columns.Add("ZDKC", Type.GetType("System.Int32"));
                dt.Columns.Add("ZGKC", Type.GetType("System.Int32"));
                dt.Columns.Add("DSNUM", Type.GetType("System.Int32"));
                dt.Columns.Add("NUM1", Type.GetType("System.Int32"));
                dt.Columns.Add("NUM2", Type.GetType("System.Int32"));
                dt.Columns.Add("NUM3", Type.GetType("System.Int32"));
                dt.Columns.Add("OPERUSER", Type.GetType("System.String"));
                string sqlDS = @"select SP.GDSEQ,f_getdeptname(PZ.DEPTID) DEPTIDname,PZ.DEPTID,PZ.ZDKC,PZ.ZGKC,PZ.DSNUM,PZ.NUM1,PZ.NUM2,PZ.NUM3
                                            from DOC_GOODS SP,DOC_GOODSCFG PZ
                                            WHERE ISDELETE = 'N' and sp.flag = 'Y' AND SP.GDSEQ = PZ.GDSEQ(+) AND PZ.GDSEQ = '{0}' AND PZ.DEPTID = '{1}'";
                foreach (DataRow row in dt.Rows)
                {
                    DataTable dtDS = DbHelperOra.Query(string.Format(sqlDS, row["GDSEQ"], lstDEPTID.SelectedValue)).Tables[0];
                    if (dtDS != null && dtDS.Rows.Count > 0)
                    {
                        row["DEPTIDNAME"] = lstDEPTID.SelectedText;
                        row["DEPTID"] = lstDEPTID.SelectedValue;
                        row["ZDKC"] = dtDS.Rows[0]["ZDKC"];
                        row["ZGKC"] = dtDS.Rows[0]["ZGKC"];
                        row["DSNUM"] = dtDS.Rows[0]["DSNUM"];
                        row["NUM1"] = dtDS.Rows[0]["NUM1"];
                        row["NUM2"] = dtDS.Rows[0]["NUM2"];
                        row["NUM3"] = dtDS.Rows[0]["NUM3"];
                        row["OPERUSER"] = UserAction.UserID;
                    }
                    PubFunc.GridRowAdd(GridList, row, false);
                }
            }
            else
            {
                Alert.Show("系统传值错误！！！", "消息提示", MessageBoxIcon.Warning);
            }
        }
        protected override void billSave()
        {
            List<Dictionary<string, object>> newDict = GridList.GetNewAddedList();
            MyTable mtTypeMx = new MyTable("doc_mygoods");
            List<CommandInfo> cmdList = new List<CommandInfo>();
            string Sql = string.Empty;
            string GETdeptid = "";
            string GETgdseq = "";
            for (int i = 0; i < newDict.Count; i++)
            {
                GETdeptid = newDict[i]["DEPTID"].ToString();
                GETgdseq = newDict[i]["GDSEQ"].ToString();
                if (DbHelperOra.Exists("select 1 from DOC_MYGOODS where DEPTID='" + GETdeptid + "' and GDSEQ ='" + GETgdseq + "'"))
                {
                    Sql = "DELETE FROM DOC_MYGOODS WHERE DEPTID = '" + GETdeptid + "' and GDSEQ = '" + GETgdseq + "'";
                    cmdList.Add(new CommandInfo(Sql, null));
                }
                mtTypeMx.ColRow = PubFunc.GridDataGet(newDict[i]);
                mtTypeMx.ColRow.Remove("DEPTIDNAME");
                mtTypeMx.ColRow.Remove("GDNAME");
                mtTypeMx.ColRow.Remove("GDSPEC");
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("BZHL");
                mtTypeMx.ColRow.Remove("ZDKC");
                mtTypeMx.ColRow.Remove("ZGKC");
                mtTypeMx.ColRow.Remove("DSNUM");
                mtTypeMx.ColRow.Remove("NUM1");
                mtTypeMx.ColRow.Remove("NUM2");
                mtTypeMx.ColRow.Remove("NUM3");
                mtTypeMx.ColRow.Remove("OPERDATE");
                cmdList.Add(mtTypeMx.Insert());
            }
            if (DbHelperOra.ExecuteSqlTran(cmdList))
                Alert.Show("保存成功！");
            else
            {
                Alert.Show("保存失败！");
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            if (GridList.Rows.Count < 1)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }

            string strSql = @"SELECT 
                              F_GETDEPTNAME(B.DEPTID) 管理科室,
                              A.GDSEQ 商品编码,
                              A.GDNAME 商品名称,
                              A.GDSPEC 商品规格,
                              C.DEFSL 默认数量,
                              F_GETUNITNAME(A.UNIT) 单位,
                              f_getproducername(A.PRODUCER) 生产商,
                              PIZNO 注册证号,
                              B.DSNUM 定数数量,
                              B.NUM1 定数含量,
                              B.NUM3 代收定数,
                              f_getusername(C.OPERUSER) 操作人,
                              TO_CHAR(C.OPERDATE, 'YYYY-MM-DD') 操作时间
                          FROM DOC_GOODS A, DOC_GOODSCFG B, DOC_MYGOODS C
                         WHERE A.GDSEQ = B.GDSEQ
                           AND A.GDSEQ = C.GDSEQ
                           AND B.GDSEQ = C.GDSEQ
                           AND B.DEPTID = C.DEPTID";
            string strSearch = "";

            if (lstDEPTID.SelectedItem != null && lstDEPTID.SelectedItem.Value.Length > 0)
            {
                strSearch += string.Format(" AND B.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }

            if (tgbGDSEQ.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND (A.GDSEQ   LIKE '%{0}%' OR A.GDNAME LIKE '%{0}%') ", tgbGDSEQ.Text.Trim());
            }

            if (rblRange.SelectedValue == "1")
            {
                strSearch += string.Format(" AND  C.OPERUSER='{0}'", UserAction.UserID);
            }

            strSql += strSearch;

            DataTable dt = DbHelperOra.Query(strSql).Tables[0];

            XTBase.Utilities.ExcelHelper.ExportByWeb(dt, "科室商品收藏导出", "科室商品收藏导出_" + DateTime.Now.ToString("yyyyMMddHH") + ".xls");
        }

    }
}
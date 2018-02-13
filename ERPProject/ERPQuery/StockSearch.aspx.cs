using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections.Specialized;
using FineUIPro;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ERPProject.ERPQuery
{
    public partial class StockSearch : PageBase
    {
        public StockSearch()
        {
            ISCHECK = false;
        }

        private string listSql = @"SELECT F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,A.DEPTID,F_GETSUPNAME(A.SUPID) SUPID,F_GETCATNAME(A.CATID) CATID,A.LOCKSL,A.PICINO,
                                   A.GDSEQ,F_GETHISINFO(A.GDSEQ,'GDNAME') GDNAME,F_GETHISINFO(A.GDSEQ,'GDSPEC') GDSPEC,C.NAME UNIT,B.BZHL,B.BAR3,f_getproducername(b.producer) producername,b.pizno,
                                   A.HWID,A.KCSL,B.HSJJ,A.KCSL*B.HSJJ HSJE,A.ZPBH,A.BILLNO,A.PHID,A.RQ_SC,A.YXQZ,decode(A.SUPID,'00002','非代管','代管') ISDG,decode(B.ISGZ,'Y','是','否') ISGZ,f_getsuppliername(A.PSSID) PSSID,
                                B.ISFLAG7,DECODE(B.ISFLAG7,'Y','本地','下传') ISFLAG7_CN,
                               DECODE(NVL(D.ISCFG, 'Y'), 'Y', '正常', '1', '正常', '停用') ISCFG_CN,
                               DECODE(NVL(D.IERP, 'Y'), 'Y', '是', '否') IERP_CN,
                               DECODE(NVL(D.ISJF, 'Y'), 'Y', '是', '否') ISJF_CN
                              FROM DAT_GOODSSTOCK A,DOC_GOODS B,DOC_GOODSUNIT C, DOC_GOODSCFG D
                             WHERE A.GDSEQ=B.GDSEQ AND A.GDSEQ = D.GDSEQ AND A.DEPTID = D.DEPTID AND B.UNIT=C.CODE(+)";

        private string listSqlNoPh = @"SELECT F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                               A.DEPTID,
                               --F_GETSUPNAME(A.SUPID) SUPID,
                              F_GETCATNAME(B.CATID) CATID,
                               A.LOCKSL,
                               A.GDSEQ,
                               F_GETHISINFO(A.GDSEQ, 'GDNAME') GDNAME,
                               F_GETHISINFO(A.GDSEQ, 'GDSPEC') GDSPEC,
                               C.NAME UNIT,
                               B.BZHL,
                               B.BAR3,
                               f_getproducername(b.producer) producername,
                               b.pizno,
                               --A.HWID,
                               A.KCSL,
                               B.HSJJ,
                               A.KCSL * B.HSJJ HSJE,
                               --A.ZPBH,
                               --A.BILLNO,
                               A.PHID,
                               A.RQ_SC,
                               A.YXQZ,
                               --decode(A.SUPID, '00002', '非代管', '代管') ISDG,
                               decode(B.ISGZ, 'Y', '是', '否') ISGZ,
                               --f_getsuppliername(A.PSSID) PSSID,
                               B.ISFLAG7,
                               DECODE(B.ISFLAG7, 'Y', '本地', '下传') ISFLAG7_CN,
                               DECODE(NVL(D.ISCFG, 'Y'), 'Y', '正常', '1', '正常', '停用') ISCFG_CN,
                               DECODE(NVL(D.IERP, 'Y'), 'Y', '是', '否') IERP_CN,
                               DECODE(NVL(D.ISJF, 'Y'), 'Y', '是', '否') ISJF_CN
                        FROM (
                        SELECT GDSEQ,DEPTID,SUM(KCSL) KCSL, SUM(LOCKSL) LOCKSL, PHID,RQ_SC,YXQZ FROM DAT_GOODSSTOCK 
                            GROUP BY GDSEQ,DEPTID, PHID,RQ_SC,YXQZ) A, DOC_GOODS B, DOC_GOODSUNIT C, DOC_GOODSCFG D
                         WHERE A.GDSEQ = B.GDSEQ AND A.GDSEQ = D.GDSEQ AND A.DEPTID = D.DEPTID
                           AND B.UNIT = C.CODE(+)";
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (Request.QueryString.Get("osid") == null)
                {
                    // 在页面第一次加载时 
                    BindDDL();
                    hfdISNOPH.Text = chkISPHALL.Checked.ToString();
                    ISNOPH(false);
                }
                else
                {
                    if (Request.QueryString.Get("osid") == "querylist")
                    {
                        string result = listSearchForPad();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();
                    }
                }
            }
        }
        protected void ISNOPH(bool flag)
        {
            if (flag)
            {

                GridColumn LOCKSL = GridGoods.FindColumn("LOCKSL");
                LOCKSL.Hidden = false;

                //GridColumn PHID = GridGoods.FindColumn("PHID");
                //PHID.Hidden = false;
                //GridColumn RQ_SC = GridGoods.FindColumn("RQ_SC");
                //RQ_SC.Hidden = false;
                //GridColumn YXQZ = GridGoods.FindColumn("YXQZ");
                //YXQZ.Hidden = false;
                GridColumn PIZNO = GridGoods.FindColumn("PIZNO");
                PIZNO.Hidden = false;
                GridColumn PSSID = GridGoods.FindColumn("PSSID");
                PSSID.Hidden = false;
                GridColumn SUPID = GridGoods.FindColumn("SUPID");
                SUPID.Hidden = false;
                GridColumn HWID = GridGoods.FindColumn("HWID");
                HWID.Hidden = false;
                GridColumn BILLNO = GridGoods.FindColumn("BILLNO");
                BILLNO.Hidden = false;

                ddlSHSID.Enabled = true;
                tbxHWID.Enabled = true;
                tbxPHID.Enabled = true;
            }
            else
            {
                //GridColumn LOCKSL = GridGoods.FindColumn("LOCKSL");
                //LOCKSL.Hidden = true;

                //GridColumn PHID = GridGoods.FindColumn("PHID");
                //PHID.Hidden = true;
                //GridColumn RQ_SC = GridGoods.FindColumn("RQ_SC");
                //RQ_SC.Hidden = true;
                //GridColumn YXQZ = GridGoods.FindColumn("YXQZ");
                //YXQZ.Hidden = true;
                GridColumn PIZNO = GridGoods.FindColumn("PIZNO");
                PIZNO.Hidden = true;
                GridColumn PSSID = GridGoods.FindColumn("PSSID");
                PSSID.Hidden = true;
                GridColumn SUPID = GridGoods.FindColumn("SUPID");
                SUPID.Hidden = true;
                GridColumn HWID = GridGoods.FindColumn("HWID");
                HWID.Hidden = true;
                GridColumn BILLNO = GridGoods.FindColumn("BILLNO");
                BILLNO.Hidden = true;

                ddlSHSID.Enabled = false;
                tbxHWID.Enabled = false;
                tbxPHID.Enabled = true;

                ddlSHSID.SelectedValue = "";
                tbxHWID.Text = "";
                //tbxPHID.Text = "";
            }


        }


        private Boolean isDg()
        {
            if (Request.QueryString["dg"] == null)
            {
                return false;
            }
            else if (Request.QueryString["dg"].ToString() == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void BindDDL()
        {
            // 绑定到下拉列表（启用模拟树功能）
            DepartmentBind.BindDDL("DDL_SYS_DEPTHASATH", UserAction.UserID, ddlDEPTID);
            PubFunc.DdlDataGet(ddlSHSID, "DDL_DOC_SHS");
            ddlDEPTID.SelectedIndex = 0;
            ddlSHSID.SelectedIndex = 0;
            USERID.Text = UserAction.UserID;
        }
        private string strExpSql()
        {
            string strSql = "";
            strSql = @"SELECT A.DEPTID||'-'||F_GETDEPTNAME(A.DEPTID) 科室名称,
                              F_GETCATNAME(B.CATID) 分类,
                               ' '||A.GDSEQ 商品编码,
                               F_GETHISINFO(A.GDSEQ, 'GDNAME') 商品名称,
                               F_GETHISINFO(A.GDSEQ, 'GDSPEC') 商品规格,
                               C.NAME 单位,
                               B.BZHL 包装含量,
                               --B.BAR3,
                               f_getproducername(b.producer) 生产厂家,
                               b.pizno 批准文号,
                               A.KCSL 库存数量,
                               B.HSJJ 含税进价,
                               A.KCSL * B.HSJJ 含税金额,
                               --decode(A.SUPID, '00002', '非代管', '代管') ISDG,
                               decode(B.ISGZ, 'Y', '是', '否') 是否高值,
                               A.PHID 批号,
                               A.RQ_SC 生产日期,
                               A.YXQZ 有效期至,
                               ";
            if (!chkISPHALL.Checked)
            {
                strSql = strSql + @"F_GETSUPNAME(A.SUPID) 供应商,
                               A.HWID 货位ID,
                               A.ZPBH 制品编号,
                               A.BILLNO 单据编码,
                               f_getsuppliername(A.PSSID) 配送商,";
            }
            strSql += @" --B.ISFLAG7,
                DECODE(B.ISFLAG7, 'Y', '本地', '下传') 本地或下传,
                DECODE(NVL(D.ISCFG, 'Y'), 'Y', '正常', '1', '正常', '停用') 状态,
                DECODE(NVL(D.IERP, 'Y'), 'Y', '是', '否') 是否盘点,
                DECODE(NVL(D.ISJF, 'Y'), 'Y', '是', '否') 计费商品";
            if (chkISPHALL.Checked)
            {
                strSql += @" FROM (
                            SELECT GDSEQ,DEPTID,SUM(KCSL) KCSL, SUM(LOCKSL) LOCKSL, PHID,RQ_SC,YXQZ FROM DAT_GOODSSTOCK 
                                GROUP BY GDSEQ,DEPTID, PHID,RQ_SC,YXQZ) A, DOC_GOODS B, DOC_GOODSUNIT C, DOC_GOODSCFG D
                         WHERE A.GDSEQ = B.GDSEQ AND A.GDSEQ = D.GDSEQ AND A.DEPTID = D.DEPTID
                           AND B.UNIT = C.CODE(+)";
            }
            else
            {
                strSql += @" FROM DAT_GOODSSTOCK A,DOC_GOODS B,DOC_GOODSUNIT C, DOC_GOODSCFG D
                             WHERE A.GDSEQ=B.GDSEQ AND A.GDSEQ = D.GDSEQ AND A.DEPTID = D.DEPTID AND B.UNIT=C.CODE(+)";
            }

            string strWhere = " ";
            if (ddlAll.SelectedValue.Length > 0) strWhere += " and A.KCSL > 0";

            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " and A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";

            if (!PubFunc.StrIsEmpty(ddlISDG.SelectedValue))
            {
                strWhere += " AND (A.GDSEQ,A.SUPID) IN (select GDSEQ,SUPID from doc_goodssup a where a.type = '" + ddlISDG.SelectedValue + "')";
            }

            //if (!PubFunc.StrIsEmpty(ddlSUPID.SelectedValue)) strWhere += " and A.SUPID = '" + ddlSUPID.SelectedValue + "'";

            if (!PubFunc.StrIsEmpty(tbxHWID.Text.Trim())) strWhere += " and A.HWID = '" + tbxHWID.Text.Trim() + "'";

            if (!PubFunc.StrIsEmpty(tbxGOODS.Text)) strWhere += " and (A.gdseq like '%" + tbxGOODS.Text + "%' or b.zjm like '%" + tbxGOODS.Text + "%' or b.gdname like '%" + tbxGOODS.Text + "%' or b.hisname like '%" + tbxGOODS.Text + "%' or b.hiscode like '%" + tbxGOODS.Text + "%')";

            if (!PubFunc.StrIsEmpty(tbxPHID.Text)) strWhere += " and A.PHID = '" + tbxPHID.Text + "'";

            //if (!PubFunc.StrIsEmpty(tbxBILLNO.Text)) strWhere += " and A.BILLNO like '%" + tbxBILLNO.Text + "%'";

            if (ddlSHSID.SelectedValue.Length > 0) strWhere += " and A.PSSID = '" + ddlSHSID.SelectedValue + "'";

            if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere += " and B.ISGZ = '" + ddlISGZ.SelectedValue + "'";

            if (!string.IsNullOrWhiteSpace(ddlISFLAG7.SelectedValue))
            {
                strWhere += string.Format(" AND B.ISFLAG7 = '{0}'", ddlISFLAG7.SelectedValue);
            }

            strWhere += string.Format(" AND a.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            if (strWhere != " ") strSql = strSql + strWhere;

            strSql += string.Format(" ORDER BY {0} {1}", GridGoods.SortField, GridGoods.SortDirection);

            return strSql;
        }
        private string GetSearchSql()
        {
            string strSql = "";
            if (chkISPHALL.Checked)
            {
                ISNOPH(false);
                strSql = listSqlNoPh;
            }
            else
            {
                ISNOPH(true);
                strSql = listSql;
            }

            string strWhere = " ";
            if (ddlAll.SelectedValue.Length > 0) strWhere += " and A.KCSL > 0";

            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " and A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";

            if (!PubFunc.StrIsEmpty(ddlISDG.SelectedValue))
            {
                strWhere += " AND (A.GDSEQ) IN (select GDSEQ from doc_goodssup a where a.type = '" + ddlISDG.SelectedValue + "')";
            }

            //if (!PubFunc.StrIsEmpty(ddlSUPID.SelectedValue)) strWhere += " and A.SUPID = '" + ddlSUPID.SelectedValue + "'";

            if (!PubFunc.StrIsEmpty(tbxHWID.Text.Trim())) strWhere += " and A.HWID = '" + tbxHWID.Text.Trim() + "'";

            if (!PubFunc.StrIsEmpty(tbxGOODS.Text)) strWhere += " and (A.gdseq like '%" + tbxGOODS.Text + "%' or b.zjm like '%" + tbxGOODS.Text + "%' or b.gdname like '%" + tbxGOODS.Text + "%' or b.hisname like '%" + tbxGOODS.Text + "%' or b.hiscode like '%" + tbxGOODS.Text + "%')";

            if (!PubFunc.StrIsEmpty(tbxPHID.Text)) strWhere += " and A.PHID = '" + tbxPHID.Text + "'";

            //if (!PubFunc.StrIsEmpty(tbxBILLNO.Text)) strWhere += " and A.BILLNO like '%" + tbxBILLNO.Text + "%'";

            if (ddlSHSID.SelectedValue.Length > 0) strWhere += " and A.PSSID = '" + ddlSHSID.SelectedValue + "'";

            if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere += " and B.ISGZ = '" + ddlISGZ.SelectedValue + "'";

            if (!string.IsNullOrWhiteSpace(ddlISFLAG7.SelectedValue))
            {
                strWhere += string.Format(" AND B.ISFLAG7 = '{0}'", ddlISFLAG7.SelectedValue);
            }

            strWhere += string.Format(" AND a.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            if (strWhere != " ") strSql = strSql + strWhere;

            strSql += string.Format(" ORDER BY {0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            return strSql;
        }
        private string GetGoodsListForPad()
        {
            string strSql = listSql;
            string strWhere = " ";
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("All"))) strWhere += " and A.KCSL > 0";
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("DEPTID")))
                strWhere += " and A.DEPTID = '" + Request.QueryString.Get("DEPTID") + "'";
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("ISDG")))
            {
                strWhere += " AND (A.GDSEQ,A.SUPID) IN (select GDSEQ,SUPID from doc_goodssup a where a.type = '" + Request.QueryString.Get("ISDG") + "')";
            }

            //if (!PubFunc.StrIsEmpty(ddlSUPID.SelectedValue)) strWhere += " and A.SUPID = '" + ddlSUPID.SelectedValue + "'";
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("HWID")))
                strWhere += " and A.HWID = '" + Request.QueryString.Get("HWID") + "'";
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("GOODS")))

                strWhere += " and (A.gdseq like '%" + Request.QueryString.Get("GOODS") + "%' or b.zjm like '%" + Request.QueryString.Get("GOODS") + "%' or b.gdname like '%" + Request.QueryString.Get("GOODS") + "%' or b.hisname like '%" + Request.QueryString.Get("GOODS") + "%' or b.hiscode like '%" + Request.QueryString.Get("GOODS") + "%')";
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("PHID")))
                strWhere += " and A.PHID = '" + Request.QueryString.Get("PHID") + "'";

            //if (!PubFunc.StrIsEmpty(tbxBILLNO.Text)) strWhere += " and A.BILLNO like '%" + tbxBILLNO.Text + "%'";
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("SHSID")))
                strWhere += " and A.SUPID = '" + Request.QueryString.Get("SHSID") + "'";

            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("ISGZ")))
                strWhere += " and B.ISGZ = '" + Request.QueryString.Get("ISGZ") + "'";

            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("ISFLAG7")))
            {
                strWhere += string.Format(" AND B.ISFLAG7 = '{0}'", Request.QueryString.Get("ISFLAG7"));
            }

            strWhere += string.Format(" AND a.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            if (strWhere != " ") strSql = strSql + strWhere;

            strSql += " ORDER BY GDSEQ";
            return strSql;
        }

        private string listSearchForPad()
        {
            string pagesize = Request.QueryString.Get("pagesize");
            string pageindex = Request.QueryString.Get("pageindex");
            int total = 0;
            JObject jo = new JObject();
            DataTable dtData = PubFunc.DbGetPage(Convert.ToInt16(pageindex) - 1, Convert.ToInt16(pagesize), GetGoodsListForPad(), ref total);
            jo.Add("result", "success");
            jo.Add("data", JsonConvert.SerializeObject(dtData));
            jo.Add("total", total);
            int totalPage = total / Convert.ToInt16(pagesize);
            if (total % Convert.ToInt16(pagesize) > 0)
            {
                totalPage = totalPage + 1;
            }
            jo.Add("totalpage", totalPage);
            string result = JsonConvert.SerializeObject(jo);
            return result;
        }
        private void DataSearch()
        {
            int total = 0;

            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, GetSearchSql(), ref total);
            OutputSummaryData(dtData);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
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
            BindDDL();
        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            DataTable dtData = DbHelperOra.Query(GetSearchSql()).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }


            XTBase.Utilities.ExcelHelper.ExportByWeb(DbHelperOra.Query(strExpSql()).Tables[0], "商品库存报表", string.Format("商品库存报表_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));


        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;

            DataSearch();
        }
        private void OutputSummaryData(DataTable source)
        {
            decimal HSJJTotal = 0, Total = 0, HSJETotal = 0;
            foreach (DataRow row in source.Rows)
            {
                HSJJTotal += Convert.ToInt32(row["KCSL"]);
                Total += Convert.ToInt32(row["LOCKSL"]);
                HSJETotal += Convert.ToDecimal(row["HSJE"]);
            }
            JObject summary = new JObject();
            summary.Add("GDNAME", "全部合计");
            summary.Add("KCSL", HSJJTotal.ToString("F2"));
            summary.Add("LOCKSL", Total.ToString("F2"));
            summary.Add("HSJE", HSJETotal.ToString("F2"));
            GridGoods.SummaryData = summary;
        }

        protected void GridGoods_RowCommand(object sender, GridCommandEventArgs e)
        {
            if (GridGoods.DataKeys[e.RowIndex][1].ToString() == "0")
            {
                return;
            }
            if ("FuncAction".Equals(e.CommandName))
            {
                hdfGdseq.Text = GridGoods.DataKeys[e.RowIndex][0].ToString();
                hdfDept.Text = GridGoods.DataKeys[e.RowIndex][2].ToString();
                hdfPhid.Text = (GridGoods.DataKeys[e.RowIndex][3] ?? "").ToString();
                hdfPicino.Text = (GridGoods.DataKeys[e.RowIndex][4] ?? "").ToString();
                GridYzSch();
            }
        }
        private void GridYzSch()
        {
            WinYz.Hidden = false;
            string strSql = "";
            if (chkISPHALL.Checked)
            {
                strSql = @"select a.lockbillno,a.lockrowno,b.gdseq, b.gdspec,b.gdname,f_getunitname(b.unit) unitname,abs(a.lockkcsl) lockkcsl,f_getdeptname(a.deptid) deptidname,
                       b.hsjj,b.hsjj*abs(a.lockkcsl) hsje,b.zpbh,a.phid,b.bar3,f_getproducername(b.producer) PRODUCERNAME,b.pizno
                from dat_stocklock a,doc_goods b
                where a.gdseq = b.gdseq and a.lockflag = 'Y' AND b.gdseq = '" + hdfGdseq.Text + "' and a.deptid = '" + hdfDept.Text + "' and a.phid = '" + hdfPhid.Text + "'";
            }
            else
            {
                strSql = @"select a.lockbillno,a.lockrowno,b.gdseq, b.gdspec,b.gdname,f_getunitname(b.unit) unitname,abs(a.lockkcsl) lockkcsl,f_getdeptname(a.deptid) deptidname,
                       b.hsjj,b.hsjj*abs(a.lockkcsl) hsje,b.zpbh,a.phid,b.bar3,f_getproducername(b.producer) PRODUCERNAME,b.pizno
                from dat_stocklock a,doc_goods b
                where a.gdseq = b.gdseq and a.lockflag = 'Y' AND a.picino = '" + hdfPicino.Text + "'";
            }
            int total = 0;
            GridYz.DataSource = PubFunc.DbGetPage(GridYz.PageIndex, GridYz.PageSize, strSql, ref total);
            GridYz.RecordCount = total;
            GridYz.DataBind();
        }

        protected void GridYz_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridYz.PageIndex = e.NewPageIndex;
            GridYzSch();
        }

        protected void chkISPHALL_CheckedChanged(object sender, CheckedEventArgs e)
        {
            DataSearch();
            if (chkISPHALL.Checked)
            {
                hfdISNOPH.Text = "Y";
            }
            else
            {
                hfdISNOPH.Text = "N";
            }

        }
    }
}
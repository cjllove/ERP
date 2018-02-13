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


namespace ERPProject.ERPQuery
{
    public partial class GoodsJshz : PageBase
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
            dpkTIME1.SelectedDate = DateTime.Now.AddMonths(-1);
            dpkTIME2.SelectedDate = DateTime.Now;
            dpkdata1.SelectedDate = DateTime.Now.AddMonths(-1);
            dpkdata2.SelectedDate = DateTime.Now;
            lisData1.SelectedDate = DateTime.Now.AddMonths(-1);
            lisData2.SelectedDate = DateTime.Now;
            dpktim1.SelectedDate = DateTime.Now.AddMonths(-1);
            dpktim2.SelectedDate = DateTime.Now;
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, ddlDEPTID, ddlDEPT, lisDEPT);
            PubFunc.DdlDataGet("DDL_DOC_SUPPLIERNULL", ddlSUPERID, ddlSUP);
            ddlSUPERID.SelectedValue = Doc.DbGetSysPara("SUPPLIER");
            ddlSUP.SelectedValue = Doc.DbGetSysPara("SUPPLIER");
        }

        private void DataSearch()
        {
            if (PubFunc.StrIsEmpty(dpkDATE1.SelectedDate.ToString()) || PubFunc.StrIsEmpty(dpkDATE2.SelectedDate.ToString()))
            {
                Alert.Show("输入日期不正确,请检查！");
                return;
            }
            if (dpkDATE1.SelectedDate > dpkDATE2.SelectedDate)
            {
                Alert.Show("开始日期不能大于结束日期！");
                return;
            }
            //            string strSql = @"select a.deptid,f_getdeptname(A.DEPTID) DEPTIDNAME,sum(decode(exp_form,'办公耗材',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) bghc,sum(decode(exp_form,'办公用品',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) bgyp,sum(decode(exp_form,'被服材料',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) bfcl
            //                        ,sum(decode(exp_form,'低值易耗',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) dzyh,sum(decode(exp_form,'电工材料',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) dgcl,sum(decode(exp_form,'非收费其他',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) fsfqt
            //                        ,sum(decode(exp_form,'木工材料',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) mgcl,sum(decode(exp_form,'设备材料',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) sbcl,sum(decode(exp_form,'试剂',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) sj
            //                        ,sum(decode(exp_form,'收费其他',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) sfqt,sum(decode(exp_form,'维修材料',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) wxcl,sum(decode(exp_form,'卫生材料',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) wscl
            //                        ,sum(decode(exp_form,'卫生用品',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) wsyp,sum(decode(exp_form,'印刷品',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) ysp,sum(decode(exp_form,'植入性材料',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) zrxcl
            //                        ,sum(decode(nvl(exp_form,'#'),'#',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) qt
            //                        ,sum(decode(nvl(exp_form,'#'),'#',decode(a.billtype,'DST',-b.hsje,b.hsje),0)) qt
            //                        from dat_ck_doc a,dat_ck_com b,doc_goods c,(select distinct exp_code,exp_form from temp_goods_his) d
            //                        where a.seqno = b.seqno and a.flag in('Y','G') and a.SHRQ between to_date('{0}','yyyy-mm-dd') and to_date('{1}','yyyy-mm-dd')
            //                        and b.gdseq = c.gdseq and c.hiscode = d.exp_code(+)";
            string strSql = @"select a.deptid,f_getdeptname(A.DEPTID) DEPTIDNAME,sum(decode(exp_form,'办公耗材',b.hsje,0)) bghc,sum(decode(exp_form,'办公用品',b.hsje,0)) bgyp,sum(decode(exp_form,'被服材料',b.hsje,0)) bfcl
                        ,sum(decode(exp_form,'低值易耗',b.hsje,0)) dzyh,sum(decode(exp_form,'电工材料',b.hsje,0)) dgcl,sum(decode(exp_form,'非收费其他',b.hsje,0)) fsfqt
                        ,sum(decode(exp_form,'木工材料',b.hsje,0)) mgcl,sum(decode(exp_form,'设备材料',b.hsje,0)) sbcl,sum(decode(exp_form,'试剂',b.hsje,0)) sj
                        ,sum(decode(exp_form,'收费其他',b.hsje,0)) sfqt,sum(decode(exp_form,'维修材料',b.hsje,0)) wxcl,sum(decode(exp_form,'卫生材料',b.hsje,0)) wscl
                        ,sum(decode(exp_form,'卫生用品',b.hsje,0)) wsyp,sum(decode(exp_form,'印刷品',b.hsje,0)) ysp,sum(decode(exp_form,'植入性材料',b.hsje,0)) zrxcl
                        ,sum(decode(nvl(exp_form,'#'),'#',b.hsje,0)) qt
                        ,sum(decode(nvl(exp_form,'#'),'办公用品',b.hsje,'低值易耗',b.hsje,'非收费其他',b.hsje,'收费其他',b.hsje,'植入性材料',b.hsje,'试剂',b.hsje,0)) hj
                        from dat_xs_doc a,dat_xs_com b,doc_goods c,(select distinct exp_code,exp_form from temp_goods_his) d
                        where a.seqno = b.seqno and a.flag in('Y','G') and a.SHRQ between to_date('{0}','yyyy-mm-dd') and to_date('{1}','yyyy-mm-dd')
                        and b.gdseq = c.gdseq and c.hiscode = d.exp_code(+)";
            string strWhere = "";
            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " and a.deptid = '" + ddlDEPTID.SelectedValue + "'";
            if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
            strSql += " group by a.deptid order by a.deptid";
            DataTable dtBill = new DataTable();
            dtBill = DbHelperOra.QueryForTable(string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text));
            GridGoods.DataSource = dtBill;
            GridGoods.DataBind();
            time1.Text = dpkDATE1.Text;
            time2.Text = dpkDATE2.Text;
            //计算合计
            decimal ld_DZYH = 0, ld_FSFQT = 0, ld_SFQT = 0, ld_ZRXCL = 0, ld_HJ = 0;
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    ld_DZYH += Convert.ToDecimal(row["DZYH"]);
                    ld_FSFQT += Convert.ToDecimal(row["FSFQT"]);
                    ld_SFQT += Convert.ToDecimal(row["SFQT"]);
                    ld_ZRXCL += Convert.ToDecimal(row["ZRXCL"]);
                    ld_HJ += Convert.ToDecimal(row["HJ"]);
                }
            }
            JObject summary = new JObject();
            summary.Add("DEPTIDNAME", "本页合计");
            summary.Add("DZYH", ld_DZYH.ToString("F2"));
            summary.Add("FSFQT", ld_FSFQT.ToString("F2"));
            summary.Add("SFQT", ld_SFQT.ToString("F2"));
            summary.Add("ZRXCL", ld_ZRXCL.ToString("F2"));
            summary.Add("HJ", ld_HJ.ToString("F2"));
            GridGoods.SummaryData = summary;
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
            if (TabStrip1.ActiveTabIndex == 0)
            {
                PubFunc.FormDataClear(FormUser);
                dpkDATE1.SelectedDate = DateTime.Now.AddMonths(-1);
                dpkDATE2.SelectedDate = DateTime.Now;
            }
            if (TabStrip1.ActiveTabIndex == 1)
            {
                dpkTIME1.SelectedDate = DateTime.Now.AddMonths(-1);
                dpkTIME2.SelectedDate = DateTime.Now;
            }
            if (TabStrip1.ActiveTabIndex == 2)
            {
                dpkdata1.SelectedDate = DateTime.Now.AddMonths(-1);
                dpkdata2.SelectedDate = DateTime.Now;
            }
            if (TabStrip1.ActiveTabIndex == 3)
            {
                tbxGDSEQ.Text = string.Empty;
                lisData1.SelectedDate = DateTime.Now.AddMonths(-1);
                lisData2.SelectedDate = DateTime.Now;
            }
            if (TabStrip1.ActiveTabIndex == 3)
            {
                tbxGoods.Text = string.Empty;
                dpktim1.SelectedDate = DateTime.Now.AddMonths(-1);
                dpktim2.SelectedDate = DateTime.Now;
            }
        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            if (TabStrip1.ActiveTabIndex == 0)
            {
                //科室分类
                if (GridGoods.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
                string strSql = @"select f_getdeptname(A.DEPTID) 科室名称
                        ,sum(decode(exp_form,'低值易耗',b.hsje,0)) 低值易耗,sum(decode(exp_form,'非收费其他',b.hsje,0)) 非收费其他
                        ,sum(decode(exp_form,'试剂',b.hsje,0)) 试剂
                        ,sum(decode(exp_form,'收费其他',b.hsje,0)) 收费其他
                        ,sum(decode(exp_form,'植入性材料',b.hsje,0)) 植入性材料
                        ,sum(decode(nvl(exp_form,'#'),'办公用品',b.hsje,'低值易耗',b.hsje,'非收费其他',b.hsje,'收费其他',b.hsje,'植入性材料',b.hsje,'试剂',b.hsje,0)) 合计
                        from dat_xs_doc a,dat_xs_com b,doc_goods c,(select distinct exp_code,exp_form from temp_goods_his) d
                        where a.seqno = b.seqno and a.flag in('Y','G') and a.SHRQ between to_date('{0}','yyyy-mm-dd') and to_date('{1}','yyyy-mm-dd')
                        and b.gdseq = c.gdseq and c.hiscode = d.exp_code(+)";
                string strWhere = "";
                if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " and a.deptid = '" + ddlDEPTID.SelectedValue + "'";
                if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
                strSql += " group by a.deptid order by a.deptid";
                DataTable dtBill = new DataTable();
                dtBill = DbHelperOra.QueryForTable(string.Format(strSql, dpkDATE1.Text, dpkDATE2.Text));
                if (dtBill == null || dtBill.Rows.Count == 0)
                {
                    Alert.Show("暂时没有符合要求的数据，无法导出", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                XTBase.Utilities.ExcelHelper.ExportByWeb(dtBill, "威海市妇幼保健院医用耗材来源情况汇总表", string.Format("科室分类结算信息_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));

                btnExp.Enabled = true;
            }
            if (TabStrip1.ActiveTabIndex == 1)
            {
                if (Gridlist.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
                string strSql = @"select F_GETPARA('SUPPER') 供应商名称,sum(decode(exp_form,'办公用品',b.xssl,0)) 办公用品数量,sum(decode(exp_form,'办公用品',b.hsje,0)) 办公用品
                        ,sum(decode(exp_form,'低值易耗',b.xssl,0)) 低值易耗数量,sum(decode(exp_form,'低值易耗',b.hsje,0)) 低值易耗
                        ,sum(decode(exp_form,'非收费其他',b.xssl,0)) 非收费其他数量,sum(decode(exp_form,'非收费其他',b.hsje,0)) 非收费其他
                        ,sum(decode(exp_form,'试剂',b.xssl,0)) 试剂其他,sum(decode(exp_form,'试剂',b.hsje,0)) 试剂
                        ,sum(decode(exp_form,'收费其他',b.xssl,0)) 收费其他数量,sum(decode(exp_form,'收费其他',b.hsje,0)) 收费其他
                       ,sum(decode(exp_form,'植入性材料',b.xssl,0)) 植入性材料数量,sum(decode(exp_form,'植入性材料',b.hsje,0)) 植入性材料
                       ,sum(decode(nvl(exp_form,'#'),'办公用品',b.xssl,'低值易耗',b.xssl,'非收费其他',b.xssl,'收费其他',b.xssl,'植入性材料',b.xssl,'试剂',b.xssl,0)) 合计数量
                        ,sum(decode(nvl(exp_form,'#'),'办公用品',b.hsje,'低值易耗',b.hsje,'非收费其他',b.hsje,'收费其他',b.hsje,'植入性材料',b.hsje,'试剂',b.hsje,0)) 合计金额
                        from dat_xs_doc a,dat_xs_com b,doc_goods c,(select distinct exp_code,exp_form from temp_goods_his) d
                        where a.seqno = b.seqno and a.flag in('Y','G') and a.SHRQ between to_date('{0}','yyyy-mm-dd') and to_date('{1}','yyyy-mm-dd')
                        and b.gdseq = c.gdseq and c.hiscode = d.exp_code(+)";
                DataTable dtBill = new DataTable();
                dtBill = DbHelperOra.QueryForTable(string.Format(strSql, dpkTIME1.Text, dpkTIME2.Text));
                if (dtBill == null || dtBill.Rows.Count == 0)
                {
                    Alert.Show("暂时没有符合要求的数据，无法导出", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                XTBase.Utilities.ExcelHelper.ExportByWeb(dtBill, "威海市妇幼保健院医用耗材来源情况汇总表", string.Format("供应商消耗商品金额汇总_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
                //Response.ClearContent();
                //Response.AddHeader("content-disposition", "attachment; filename=供应商消耗商品金额汇总.xls");
                //Response.ContentType = "application/excel";
                //Response.ContentEncoding = System.Text.Encoding.UTF8;
                //Response.Write(PubFunc.GridToHtml(Gridlist));
                //Response.End();
                BtnEpt.Enabled = true;
            }
            if (TabStrip1.ActiveTabIndex == 2)
            {
                if (Gridlist2.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
                string strSql = @"select F_GETPARA('SUPPER') 供应商名称,sum(hsje) 合计金额,sum(decode(a.billtype,'DSH',b.bzhl,b.xssl)) 合计数量
                from dat_xs_doc a,dat_xs_com b
                where a.seqno = b.seqno and a.flag in('Y','G') and a.SHRQ between to_date('{0}','yyyy-mm-dd') and to_date('{1}','yyyy-mm-dd')";
                DataTable dtBill = new DataTable();
                dtBill = DbHelperOra.QueryForTable(string.Format(strSql, dpkdata1.Text, dpkdata2.Text));
                if (dtBill == null || dtBill.Rows.Count == 0)
                {
                    Alert.Show("暂时没有符合要求的数据，无法导出", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                XTBase.Utilities.ExcelHelper.ExportByWeb(dtBill, "威海市妇幼保健院医用耗材来源情况汇总表", string.Format("供应商消耗商品金额汇总_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
                //Response.ClearContent();
                //Response.AddHeader("content-disposition", "attachment; filename=供应商消耗商品金额汇总.xls");
                //Response.ContentType = "application/excel";
                //Response.ContentEncoding = System.Text.Encoding.UTF8;
                //Response.Write(PubFunc.GridToHtml(Gridlist2));
                //Response.End();
                btnexl.Enabled = true;
            }
            if (TabStrip1.ActiveTabIndex == 3)
            {
                if (Grdlist.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=单品入库明细.xls");
                Response.ContentType = "application/excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Write(PubFunc.GridToHtml(Grdlist));
                Response.End();
                btnexl.Enabled = true;
            }
            if (TabStrip1.ActiveTabIndex == 4)
            {
                if (Gridlist4.Rows.Count < 1)
                {
                    Alert.Show("没有数据,无法导出！");
                    return;
                }
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment; filename=单品入库汇总.xls");
                Response.ContentType = "application/excel";
                Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Write(PubFunc.GridToHtml(Gridlist4));
                Response.End();
                btnexl.Enabled = true;
            }
        }

        protected void BtnSearch_Click(object sender, EventArgs e)
        {
            if (PubFunc.StrIsEmpty(dpkTIME1.SelectedDate.ToString()) || PubFunc.StrIsEmpty(dpkTIME2.SelectedDate.ToString()))
            {
                Alert.Show("输入日期不正确,请检查！");
                return;
            }
            if (dpkTIME1.SelectedDate > dpkTIME2.SelectedDate)
            {
                Alert.Show("开始日期不能大于结束日期！");
                return;
            }
            string strSql = @"select F_GETPARA('SUPPER') SUPNAME,sum(decode(exp_form,'办公耗材',b.hsje,0)) bghc,sum(decode(exp_form,'办公用品',b.hsje,0)) bgyp,sum(decode(exp_form,'被服材料',b.hsje,0)) bfcl
                        ,sum(decode(exp_form,'低值易耗',b.hsje,0)) dzyh,sum(decode(exp_form,'电工材料',b.hsje,0)) dgcl,sum(decode(exp_form,'非收费其他',b.hsje,0)) fsfqt
                        ,sum(decode(exp_form,'木工材料',b.hsje,0)) mgcl,sum(decode(exp_form,'设备材料',b.hsje,0)) sbcl,sum(decode(exp_form,'试剂',b.hsje,0)) sj
                        ,sum(decode(exp_form,'收费其他',b.hsje,0)) sfqt,sum(decode(exp_form,'维修材料',b.hsje,0)) wxcl,sum(decode(exp_form,'卫生材料',b.hsje,0)) wscl
                        ,sum(decode(exp_form,'卫生用品',b.hsje,0)) wsyp,sum(decode(exp_form,'印刷品',b.hsje,0)) ysp,sum(decode(exp_form,'植入性材料',b.hsje,0)) zrxcl
                        ,sum(decode(nvl(exp_form,'#'),'#',b.hsje,0)) qt
                        ,sum(decode(nvl(exp_form,'#'),'办公用品',b.hsje,'低值易耗',b.hsje,'非收费其他',b.hsje,'收费其他',b.hsje,'植入性材料',b.hsje,'试剂',b.hsje,0)) hj
                        --数量
                        ,sum(decode(exp_form,'办公用品',b.xssl,0)) bgypsl,sum(decode(exp_form,'低值易耗',b.xssl,0)) dzyhsl,sum(decode(exp_form,'非收费其他',b.xssl,0)) fsfqtsl
                        ,sum(decode(exp_form,'试剂',b.xssl,0)) sjsl,sum(decode(exp_form,'收费其他',b.xssl,0)) sfqtsl,sum(decode(exp_form,'植入性材料',b.xssl,0)) zrxclsl
                        ,sum(decode(nvl(exp_form,'#'),'办公用品',b.xssl,'低值易耗',b.xssl,'非收费其他',b.xssl,'收费其他',b.xssl,'植入性材料',b.xssl,'试剂',b.xssl,0)) hjsl
                        from dat_xs_doc a,dat_xs_com b,doc_goods c,(select distinct exp_code,exp_form from temp_goods_his) d
                        where a.seqno = b.seqno and a.flag in('Y','G') and a.SHRQ between to_date('{0}','yyyy-mm-dd') and to_date('{1}','yyyy-mm-dd')
                        and b.gdseq = c.gdseq and c.hiscode = d.exp_code(+)";
            Gridlist.DataSource = DbHelperOra.QueryForTable(string.Format(strSql, dpkTIME1.Text, dpkTIME2.Text));
            Gridlist.DataBind();
            HIDtime1.Text = dpkTIME1.Text;
            HIDtime2.Text = dpkTIME2.Text;
        }

        protected void btnSrch_Click(object sender, EventArgs e)
        {
            if (PubFunc.StrIsEmpty(dpkdata1.SelectedDate.ToString()) || PubFunc.StrIsEmpty(dpkdata2.SelectedDate.ToString()))
            {
                Alert.Show("输入日期不正确,请检查！");
                return;
            }
            if (dpkdata1.SelectedDate > dpkdata2.SelectedDate)
            {
                Alert.Show("开始日期不能大于结束日期！");
                return;
            }
            string strSql = @"select sum(hsje) hjje,F_GETPARA('SUPPER') SUPNAME,sum(decode(a.billtype,'DSH',b.bzhl,b.xssl)) hjsl
                from dat_xs_doc a,dat_xs_com b
                where a.seqno = b.seqno and a.flag in('Y','G') and a.SHRQ between to_date('{0}','yyyy-mm-dd') and to_date('{1}','yyyy-mm-dd')";
            Gridlist2.DataSource = DbHelperOra.QueryForTable(string.Format(strSql, dpkdata1.Text, dpkdata2.Text));
            Gridlist2.DataBind();
            hiddata1.Text = dpkTIME1.Text;
            hiddata2.Text = dpkTIME2.Text;
        }

        protected void btnlis_Click(object sender, EventArgs e)
        {
            string strSql = @"SELECT B.*,F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,F_GETUNITNAME(B.UNIT) UNITNAME,F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME
                            FROM DAT_XS_DOC A,DAT_XS_COM B
                            WHERE A.SEQNO = B.SEQNO AND A.FLAG IN('Y','G') AND A.SHRQ BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND TO_DATE('{1}','YYYY-MM-DD')";
            string strWhere = "";
            if (!PubFunc.StrIsEmpty(tbxGDSEQ.Text.Trim())) strWhere += string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%')", tbxGDSEQ.Text.Trim());
            if (!PubFunc.StrIsEmpty(ddlDEPT.SelectedValue)) strWhere += string.Format(" AND (a.deptid = '{0}')", ddlDEPT.SelectedValue);
            if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
            strSql += " order by a.deptid，b.gdname";
            DataTable dtBill = DbHelperOra.QueryForTable(string.Format(strSql, lisData1.Text, lisData2.Text));
            Grdlist.DataSource = dtBill;
            Grdlist.DataBind();
            //合计
            decimal sumsl = 0, sumhjje = 0;
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    sumsl += Convert.ToDecimal(row["XSSL"]);
                    sumhjje += Convert.ToDecimal(row["HSJE"]);
                }
            }
            JObject summary = new JObject();
            summary.Add("GDNAME3", "本页合计");
            summary.Add("XSSL3", sumsl.ToString("F0"));
            summary.Add("HSJE3", sumhjje.ToString("F2"));
            Grdlist.SummaryData = summary;
        }

        protected void btnSch_Click(object sender, EventArgs e)
        {
            string strSql = @"SELECT B.GDSEQ,B.GDNAME,B.GDSPEC,B.UNIT,F_GETUNITNAME(B.UNIT) UNITNAME,B.HSJJ,SUM(B.HSJE) HSJE,F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,B.PZWH,SUM(B.XSSL) SL
                        FROM DAT_XS_DOC A,DAT_XS_COM B
                        WHERE A.SEQNO = B.SEQNO AND A.FLAG IN('Y','G') AND A.SHRQ BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND TO_DATE('{1}','YYYY-MM-DD')";
            string strWhere = "";
            if (!PubFunc.StrIsEmpty(tbxGoods.Text.Trim())) strWhere += string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%')", tbxGoods.Text.Trim());
            if (!PubFunc.StrIsEmpty(lisDEPT.SelectedValue)) strWhere += string.Format(" AND (a.deptid = '{0}')", lisDEPT.SelectedValue);
            if (strWhere.Trim().Length > 0) strSql = strSql + strWhere;
            strSql += " group by B.gdseq,B.gdname,B.gdspec,B.unit,b.hsjj,b.producer,B.PZWH order by B.gdname";
            DataTable dtBill = DbHelperOra.QueryForTable(string.Format(strSql, dpktim1.Text, dpktim2.Text));
            Gridlist4.DataSource = dtBill;
            Gridlist4.DataBind();
            //合计
            decimal sumsl = 0, sumhjje = 0;
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    sumsl += Convert.ToDecimal(row["SL"]);
                    sumhjje += Convert.ToDecimal(row["HSJE"]);
                }
            }
            JObject summary = new JObject();
            summary.Add("GDNAME4", "本页合计");
            summary.Add("SL4", sumsl.ToString("F0"));
            summary.Add("HSJE4", sumhjje.ToString("F2"));
            Gridlist4.SummaryData = summary;
        }
    }
}
﻿using XTBase;
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
    public partial class GoodsInvoicing : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 在页面第一次加载时 
                BindDDL();
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
            dpkDATE1.SelectedDate = DateTime.Now;
            dpkDATE2.SelectedDate = DateTime.Now;

            //PubFunc.DdlDataGet(ddlDEPTID, "DDL_SYS_DEPT"); 
            DepartmentBind.BindDDL("DDL_SYS_DEPTHASATH", UserAction.UserID, ddlDEPTID);
            // PubFunc.DdlDataGet(ddlSUPID, "DDL_DOC_SUPPLIERNULL");
            //代管需要绑定代管供应商，其他绑定非代管供应商
            //if (!isDg())
            //{
            //    PubFunc.DdlDataGet("DDL_DOC_SUPPLIERNULL", ddlSUPID);
            //}
            //else
            //{
            //    //TODO 增加sys_report 代管供应商
            //    PubFunc.DdlDataGet("DDL_DOC_SUPPLIER_DG", ddlSUPID);
            //}DDL_DOC_SUPPSCS
            PubFunc.DdlDataGet("DDL_DOC_SUPID", ddlSUPID);
            //PbFunc.DdlDataGet("DDL_PRODUCER", ddlSCSID);
            PubFunc.DdlDataGet("DDL_DOC_PSSNAMEADDZG", ddlSCSID);
            //PubFunc.DdlDataGet("DDL_BILLTYPE", ddlBillType);
            GetBillType();
            //PubFunc.DdlDataGet("DDL_GOODS_TYPE", srhCATID0);
            //ddlBillType.Items.Remove(new FineUIPro.ListItem() { Text = "商品编码", Value = "DOC_GOODS" });

            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("SL", "");
            summary.Add("LSJE", "");
            summary.Add("HSJE", "");
            summary.Add("BHSJE", "");
            GridGoods.SummaryData = summary;
        }

        private string GetSearchSql()
        {
            string strSql = @"SELECT f_getunitname(B.Unit) UNIT,
                                   A.RQSJ     ,
                                   C.RULENAME BILLTYPE ,
                                   A.BILLNO   ,
                                   A.ROWNO    ,
                                   F.NAME DEPTNAME ,
                                   d.supname supname,
                                   F_GETSUPNAME(A.PSSID)PSSNAME,
                                   A.GDSEQ    ,
                                   F_GETHISINFO(A.GDSEQ,'GDNAME') GDNAME,
                                   F_GETHISINFO(A.GDSEQ,'GDSPEC') GDSPEC,
                                   e.name CATNAME,
                                   --DECODE(B.ISFLAG3,'0','非直送商品','1','直送商品','N','非直送商品','Y','直送商品')    SPLB ,
                                   DECODE(B.ISFLAG3,'0','否','1','是','N','否','Y','是') ISFLAG3 ,
                                   A.HWID     ,
                                   A.PH       ,
                                   A.YXQZ     ,
                                   DECODE(A.KCADD,1,'增库存',-1,'减库存','其他') KCADD,
                                   round(A.JXTAX,4)   JXTAX ,
                                   round(A.LSJ ,4)    LSJ ,
                                   round(A.HSJJ,4)    HSJJ ,
                                   round(A.BHSJJ,4)   BHSJJ ,
                                   sum(round(A.SL,4))      SL ,
                                   round(A.LSJE ,4)    LSJE,
                                   sum(round(A.HSJE ,4))    HSJE,
                                   sum(round(A.BHSJE ,4))  BHSJE ,
                                   A.PZWH     ,
                                   A.RQ_SC    ,
                                   A.ZPBH     ,
                                   F_GETSUPNAME(A.SUPID) SUPID,
                                   F_GETUSERNAME(A.OPERGH) OPERGH ,decode(A.SUPID,'00002','非代管','代管') ISDG ,
                                   decode(B.ISGZ,'Y','是','否') ISGZ,
                                   DECODE(B.ISFLAG7,'Y','是','否') ISFLAG7  
                              FROM DAT_GOODSJXC A,
                                   DOC_GOODS B,
                                   (SELECT SUBSTR(RULEID,6,3) BILLTYPE, RULENAME FROM SYS_GLOBRULE WHERE RULEID LIKE 'BILL_%') C,
                                   DOC_SUPPLIER D,
                                   SYS_CATEGORY E,
                                   SYS_DEPT F
                             WHERE A.GDSEQ=B.GDSEQ(+) AND A.BILLTYPE=C.BILLTYPE AND A.SUPID=D.SUPID(+) AND A.CATID=E.CODE(+) AND A.DEPTID=F.CODE(+) ";
            string strWhere = " ";
            strWhere += " AND A.RQSJ>=TO_DATE('" + dpkDATE1.Text + "','YYYY/MM/DD') AND  A.RQSJ< TO_DATE('" + dpkDATE2.Text + "','YYYY/MM/DD') +1 ";
            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " and A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
            ////通过供应商区分代管和非代管
            //if (isDg())
            //{
            //    strWhere += " AND A.SUPID IN (select SUPID from doc_supplier WHERE ISDG='Y')";
            //}
            //else
            //{
            //    if (chkISDG.Checked)
            //    {
            //        strWhere += " AND A.SUPID IN (select SUPID from doc_supplier)";
            //    }
            //    else
            //    {
            //        strWhere += " AND A.SUPID IN (select SUPID from doc_supplier WHERE ISDG='N')";
            //    }
            //    //strWhere += " AND A.SUPID IN (select SUPID from doc_supplier WHERE ISDG='N')";
            //}
            if (!PubFunc.StrIsEmpty(ddlSUPID.SelectedValue)) strWhere += " and A.SUPID = '" + ddlSUPID.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(ddlSCSID.SelectedValue)) strWhere += " and A.PSSID = '" + ddlSCSID.SelectedValue + "'";
            //-----------------------------------------------------------------------------------------------------
            //if (!PubFunc.StrIsEmpty(ddlBillType.SelectedValue))
            //{
            //    strWhere += " and A.BILLTYPE = '" + ddlBillType.SelectedValue.Substring(ddlBillType.SelectedValue.IndexOf('_') + 1) + "'";
            //}

            //if (!PubFunc.StrIsEmpty(txbGDSEQ.Text)) strWhere += " and A.gdseq = '" + txbGDSEQ.Text + "'";
            if (!PubFunc.StrIsEmpty(txbGDSEQ.Text)) strWhere += " AND (A.GDSEQ LIKE UPPER('%" + txbGDSEQ.Text + "%') OR B.ZJM LIKE UPPER('%" + txbGDSEQ.Text + "%') OR B.GDNAME LIKE '%" + txbGDSEQ.Text + "%' OR B.GDSPEC LIKE '%" + txbGDSEQ.Text + "%' OR A.PH LIKE '%" + txbGDSEQ.Text + "%')";
            if (!PubFunc.StrIsEmpty(tbxBILLNO.Text)) strWhere += " and a.billno like UPPER( '%" + tbxBILLNO.Text + "%')";
            if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere += " and B.ISGZ = '" + ddlISGZ.SelectedValue + "'";
            //if (tgbPH.Text.Trim().Length > 0) strWhere += " and A.PH LIKE '%" + tgbPH.Text.Trim() + "%'";
            //if (tgbHWID.Text.Trim().Length > 0) strWhere += " and A.HWID LIKE '%" + tgbHWID.Text.Trim() + "%'";
            //if (srhCATID0.SelectedValue.Length > 0) strWhere += " and B.CATID0 = '" + srhCATID0.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(ddlISADD.SelectedValue)) strWhere += " and A.KCADD = '" + ddlISADD.SelectedValue + "'";
            if (!string.IsNullOrWhiteSpace(ddlISFLAG7.SelectedValue))
            {
                strWhere += string.Format(" AND B.ISFLAG7 = '{0}'", ddlISFLAG7.SelectedValue);
            }
            strWhere += string.Format(" AND a.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);

            string[] billTypes = cblBillType.SelectedValueArray;
            if (billTypes.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" and A.BILLTYPE in('");
                foreach (string item in billTypes)
                {
                    sb.Append(item.Substring(item.IndexOf('_') + 1) + "','");
                }
                string temp = sb.ToString();
                temp = temp.Substring(0, temp.Length - 2);
                temp = temp + ")";
                strWhere += temp;
            }
            if (strWhere != " ") strSql = strSql + strWhere;

            //同一商品同一批号，来自同一入库单的合并
            strSql += string.Format(" group by B.Unit,A.RQSJ,C.RULENAME,A.BILLNO，A.BILLTYPE,A.ROWNO,F.NAME,d.supname, A.GDSEQ, e.name, B.ISFLAG3, A.HWID, A.SL,A.PH, A.YXQZ, A.KCADD, A.JXTAX, A.LSJ, A.PZWH,A.RQ_SC, A.ZPBH, A.SUPID,A.PSSID, A.OPERGH, B.ISFLAG7, a.str2,A.HSJJ, A.BHSJJ, A.LSJE, B.ISGZ");

            if (GridGoods.SortField == "GDNAME" || GridGoods.SortField == "GDSPEC")
            {
                    strSql += string.Format(" ORDER BY {0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            }
            
            else if (GridGoods.SortField == "ISFLAG3" || GridGoods.SortField == "ISGZ" || GridGoods.SortField == "ISFLAG7")
            {
                strSql += string.Format(" ORDER BY B.{0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            }
            else 
            {
                strSql += string.Format(" ORDER BY A.{0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            }

            return strSql;
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

            int total = 0;

            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, GetSearchSql(), ref total);
            DataTable dtAll = DbHelperOra.QueryForTable(GetSearchSql());
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
            //计算合计数量
            if (dtData != null && dtData.Rows.Count > 0)
            {
                decimal slTotal = 0, lsjeTotal = 0, hsjeTotal = 0, bhsjeTotal = 0,slAll=0,lsjeAll=0,hsjeAll=0,bhsjeAll=0;
                foreach (DataRow row in dtData.Rows)
                {
                    slTotal += Convert.ToDecimal(row["SL"] ?? "0");
                    lsjeTotal += Convert.ToDecimal(row["LSJE"] ?? "0");
                    hsjeTotal += Convert.ToDecimal(row["HSJE"] ?? "0");
                    bhsjeTotal += Convert.ToDecimal(row["BHSJE"] ?? "0");
                }
                foreach (DataRow dr in dtAll.Rows)
                {
                    slAll += Convert.ToDecimal(dr["SL"] ?? "0");
                    lsjeAll += Convert.ToDecimal(dr["LSJE"] ?? "0");
                    hsjeAll += Convert.ToDecimal(dr["HSJE"] ?? "0");
                    bhsjeAll += Convert.ToDecimal(dr["BHSJE"] ?? "0");
                }
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计</br>总合计");
                summary.Add("SL", slTotal+"</br>"+slAll);
                summary.Add("LSJE", lsjeTotal.ToString("F2")+"</br>"+lsjeTotal.ToString("F2"));
                summary.Add("HSJE", hsjeTotal.ToString("F2")+"</br>"+hsjeAll.ToString("F2"));
                summary.Add("BHSJE", bhsjeTotal.ToString("F2")+"</br>"+bhsjeAll.ToString("F2"));
                GridGoods.SummaryData = summary;
            }
            else
            {
                JObject summary = new JObject();
                summary.Add("GDNAME", "本页合计");
                summary.Add("SL", 0);
                summary.Add("LSJE", 0);
                summary.Add("HSJE", 0);
                summary.Add("BHSJE", 0);
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
            dpkDATE1.SelectedDate = DateTime.Now;
            dpkDATE2.SelectedDate = DateTime.Now;
        }
        protected void btExport_Click(object sender, EventArgs e)
        {
            DataTable dtData = DbHelperOra.Query(GetSearchSql()).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("暂时没有查询到符合条件的进销存数据,无法导出！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            //string[] columnNames = new string[GridGoods.Columns.Count - 1];
            //List<String> columnNames = new List<string>();
            //for (int index = 1; index < GridGoods.Columns.Count; index++)
            //{
            //    GridColumn column = GridGoods.Columns[index];
            //    if (column is FineUIPro.BoundField && column.Hidden == false)
            //    {
            //        //dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].DataType = Type.GetType("System.String");
            //        dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
            //        columnNames.Add(column.HeaderText);
            //    }
            //}
            ////原列名中没有SEQNO字段，导出数据漏行，增加SEQNO后正常
            //ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames.ToArray()), "商品进销存信息", string.Format("商品进销存信息_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
            string strSql = @"SELECT 
                                   to_char(A.RQSJ,'yyyy/MM/dd HH24:mi:ss') 时间,
                                   C.RULENAME 单据类型 ,
                                   A.BILLNO 单据编号,
                                   ' '||A.GDSEQ 商品编码,
                                   F_GETHISINFO(A.GDSEQ,'GDNAME') 商品名称,
                                   F_GETHISINFO(A.GDSEQ,'GDSPEC') 规格容量,
                                  -- A.ROWNO    ,
                                   sum(round(A.SL,4))数量,
                                   f_getunitname(B.Unit) 单位,
                                   round(A.HSJJ,4)进价,
                                   sum(round(A.HSJE ,4))含税金额,
                                   F.NAME 科室 ,
                                   F_GETSUPNAME(A.SUPID) 供应商,
                                   F_GETSUPNAME(A.PSSID) 配送商,
                                   A.HWID 货位,
                                   A.PH 批号,
                                   A.RQ_SC 生产日期,
                                   A.YXQZ 效期,
                                   DECODE(A.KCADD,1,'增库存',-1,'减库存','其他') 操作,
                                   DECODE(B.ISFLAG3,'0','否','1','是','N','否','Y','是') 是否直送 ,
                                   --DECODE(B.ISFLAG3,'0','非直送商品','1','直送商品','N','非直送商品','Y','直送商品')    SPLB ,
                                   A.PZWH 注册证号,
                                   F_GETUSERNAME(A.OPERGH) 操作员 ,
                                   e.name 商品类别,
                                   decode(B.ISGZ,'Y','是','否')高值,
                                   DECODE(B.ISFLAG7,'Y','是','否') 本地  
                                   
                              FROM DAT_GOODSJXC A,
                                   DOC_GOODS B,
                                   (SELECT SUBSTR(RULEID,6,3) BILLTYPE, RULENAME FROM SYS_GLOBRULE WHERE RULEID LIKE 'BILL_%') C,
                                   DOC_SUPPLIER D,
                                   SYS_CATEGORY E,
                                   SYS_DEPT F
                             WHERE A.GDSEQ=B.GDSEQ(+) AND A.BILLTYPE=C.BILLTYPE AND A.SUPID=D.SUPID(+) AND A.CATID=E.CODE(+) AND A.DEPTID=F.CODE(+) ";
            string strWhere = " ";
            strWhere += " AND A.RQSJ>=TO_DATE('" + dpkDATE1.Text + "','YYYY/MM/DD') AND  A.RQSJ< TO_DATE('" + dpkDATE2.Text + "','YYYY/MM/DD') +1 ";
            if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " and A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(ddlSUPID.SelectedValue)) strWhere += " and A.SUPID = '" + ddlSUPID.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(ddlSCSID.SelectedValue)) strWhere += " and A.PSSID = '" + ddlSCSID.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(txbGDSEQ.Text)) strWhere += " AND (A.GDSEQ LIKE UPPER('%" + txbGDSEQ.Text + "%') OR B.ZJM LIKE UPPER('%" + txbGDSEQ.Text + "%') OR B.GDNAME LIKE '%" + txbGDSEQ.Text + "%' OR B.GDSPEC LIKE '%" + txbGDSEQ.Text + "%' OR A.PH LIKE '%" + txbGDSEQ.Text + "%')";
            if (!PubFunc.StrIsEmpty(tbxBILLNO.Text)) strWhere += " and a.billno like UPPER( '%" + tbxBILLNO.Text + "%')";
            if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere += " and B.ISGZ = '" + ddlISGZ.SelectedValue + "'";
            if (!PubFunc.StrIsEmpty(ddlISADD.SelectedValue)) strWhere += " and A.KCADD = '" + ddlISADD.SelectedValue + "'";
            if (!string.IsNullOrWhiteSpace(ddlISFLAG7.SelectedValue))
            {
                strWhere += string.Format(" AND B.ISFLAG7 = '{0}'", ddlISFLAG7.SelectedValue);
            }
            strWhere += string.Format(" AND a.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);

            string[] billTypes = cblBillType.SelectedValueArray;
            if (billTypes.Length > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(" and A.BILLTYPE in('");
                foreach (string item in billTypes)
                {
                    sb.Append(item.Substring(item.IndexOf('_') + 1) + "','");
                }
                string temp = sb.ToString();
                temp = temp.Substring(0, temp.Length - 2);
                temp = temp + ")";
                strWhere += temp;
            }
            if (strWhere != " ") strSql = strSql + strWhere;
            strSql += string.Format(" group by B.Unit,A.RQSJ,C.RULENAME,A.BILLNO，A.BILLTYPE,A.ROWNO,F.NAME,d.supname, A.GDSEQ, e.name, B.ISFLAG3, A.HWID, A.SL,A.PH, A.YXQZ, A.KCADD, A.JXTAX, A.LSJ, A.PZWH,A.RQ_SC, A.ZPBH, A.SUPID,A.PSSID, A.OPERGH, B.ISFLAG7, a.str2,A.HSJJ, A.BHSJJ, A.LSJE, B.ISGZ");

            if (GridGoods.SortField == "GDNAME" || GridGoods.SortField == "GDSPEC")
            {
                strSql += string.Format(" ORDER BY {0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            }
            if (GridGoods.SortField == "ISFLAG3" || GridGoods.SortField == "ISGZ" || GridGoods.SortField == "ISFLAG7")
            {
                strSql += string.Format(" ORDER BY B.{0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            }
            else
            {
                strSql += string.Format(" ORDER BY A.{0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            }
            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            ExcelHelper.ExportByWeb(dt, "商品进销存信息", "商品进销存信息_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
            //20160106 lvj
            //if (GridGoods.Rows.Count <= 0)
            //{
            //    Alert.Show("请先选择要导出的商品进销存明细！");
            //    return;
            //}
            //string strSql = @"SELECT --A.ROWNO 行号,
            //            A.RQSJ 时间,
            //            C.RULENAME 单据类型,
            //            A.SEQNO 单据编号,
            //            A.GDSEQ 商品编码,
            //            F_GETHISINFO(A.GDSEQ,'GDNAME') 商品名称,
            //            F_GETHISINFO(A.GDSEQ,'GDSPEC') 规格容量,
            //            round(A.SL,4) 数量,
            //            f_getunitname(B.Unit) 单位,
            //            round(A.HSJJ,4) 进价,
            //            F.NAME 科室,
            //            d.supname 供应商,
            //            A.HWID 货位,
            //            A.PH 批号,
            //            A.YXQZ 效期,
            //            DECODE(A.KCADD,1,'增库存',-1,'减库存','其他') 操作,
            //            DECODE(B.ISFLAG3,'0','非直送商品','1','直送商品','N','非直送商品','Y','直送商品') 直送商品 , 
            //                       --A.BILLNO   ,
            //                       --round(A.JXTAX,4)   JXTAX ,
            //                       --round(A.LSJ ,4)    LSJ ,
            //                       round(A.BHSJJ,4) 不含税进价 ,
            //                       round(A.LSJE ,4) 离手金额,
            //                       round(A.HSJE ,4) 含税金额,
            //                       round(A.BHSJE ,4) 不含税金额 ,
            //                       A.PZWH 批准文号,
            //                       A.RQ_SC 生产日期,
            //                       A.ZPBH 制品编号,
            //                       e.name 商品类别,
            //                       --F_GETSUPNAME(A.SUPID) SUPID,
            //                       --F_GETUSERNAME(A.OPERGH) OPERGH ,decode(A.SUPID,'00002','非代管','代管') ISDG ,
            //                       decode(B.ISGZ,'Y','是','否') 是否高值  
            //                  FROM DAT_GOODSJXC A,
            //                       DOC_GOODS B,
            //                       (SELECT SUBSTR(RULEID,6,3) BILLTYPE, RULENAME FROM SYS_GLOBRULE WHERE RULEID LIKE 'BILL_%') C,
            //                       DOC_SUPPLIER D,
            //                       SYS_CATEGORY E,
            //                       SYS_DEPT F
            //                 WHERE A.GDSEQ=B.GDSEQ(+) AND A.BILLTYPE=C.BILLTYPE AND A.SUPID=D.SUPID(+) AND A.CATID=E.CODE(+) AND A.DEPTID=F.CODE(+)";
            //string strWhere = " ";
            //strWhere += " AND A.RQSJ>=TO_DATE('" + dpkDATE1.Text + "','YYYY/MM/DD') AND  A.RQSJ< TO_DATE('" + dpkDATE2.Text + "','YYYY/MM/DD') +1 ";
            //if (!PubFunc.StrIsEmpty(ddlDEPTID.SelectedValue)) strWhere += " and A.DEPTID = '" + ddlDEPTID.SelectedValue + "'";
            //if (!PubFunc.StrIsEmpty(ddlSUPID.SelectedValue)) strWhere += " and A.SUPID = '" + ddlSUPID.SelectedValue + "'";
            //if (!PubFunc.StrIsEmpty(ddlBillType.SelectedValue))
            //{
            //    strWhere += " and A.BILLTYPE = '" + ddlBillType.SelectedValue.Substring(ddlBillType.SelectedValue.IndexOf('_') + 1) + "'";
            //}

            ////if (!PubFunc.StrIsEmpty(txbGDSEQ.Text)) strWhere += " and A.gdseq = '" + txbGDSEQ.Text + "'";
            //if (!PubFunc.StrIsEmpty(txbGDSEQ.Text)) strWhere += " and (A.gdseq like UPPER('%" + txbGDSEQ.Text + "%') or b.zjm like UPPER('%" + txbGDSEQ.Text + "%') or b.gdname like UPPER('%" + txbGDSEQ.Text + "%'))";
            //if (!PubFunc.StrIsEmpty(tbxBILLNO.Text)) strWhere += " and a.billno like '%" + tbxBILLNO.Text + "%'";
            //if (!PubFunc.StrIsEmpty(ddlISGZ.SelectedValue)) strWhere += " and B.ISGZ = '" + ddlISGZ.SelectedValue + "'";
            //if (tgbPH.Text.Trim().Length > 0) strWhere += " and A.PH LIKE '%" + tgbPH.Text.Trim() + "%'";
            //if (tgbHWID.Text.Trim().Length > 0) strWhere += " and A.HWID LIKE '%" + tgbHWID.Text.Trim() + "%'";
            //if (srhCATID0.SelectedValue.Length > 0) strWhere += " and B.CATID0 = '" + srhCATID0.SelectedValue + "'";
            //strWhere += string.Format(" AND a.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);

            //if (strWhere != " ") strSql = strSql + strWhere;
            //if (GridGoods.SortField == "GDNAME" || GridGoods.SortField == "GDSPEC")
            //{
            //    strSql += string.Format(" ORDER BY {0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            //}
            //else
            //{
            //    strSql += string.Format(" ORDER BY A.{0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            //}

            //DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            //ExcelHelper.ExportByWeb(dt, "商品进销存信息", "商品进销存信息_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;

            DataSearch();
        }
        protected void GetBillType()
        {
            string strSql = @"SELECT RULENAME, RULEID
                                      FROM SYS_GLOBRULE
                                     WHERE ISRULE = 'N'
                                       AND RULEID IN ('BILL_DSC',
                                                      'BILL_DSH',
                                                      'BILL_DST',
                                                      'BILL_GTD',
                                                      'BILL_XSG',
                                                      'BILL_YKD',
                                                      'BILL_KSD',
                                                      'BILL_LTD',
                                                      'BILL_LCD',
                                                      'BILL_XSD',
                                                      'BILL_CKD',
                                                      'BILL_DBD',
                                                      'BILL_SYD',
                                                      'BILL_THD',
                                                      'BILL_RKD',
                                                      'BILL_XST',
                                                      'BILL_TJD'
                                                           )
                                     ORDER BY RULENAME";
            DataTable dt = DbHelperOra.Query(strSql).Tables[0];

            cblBillType.DataTextField = "RULENAME";
            cblBillType.DataValueField = "RULEID";
            cblBillType.DataSource = dt;
            cblBillType.DataBind();
        }
        protected void rblAutoPostBack_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataSearch();
        }
    }
    public class Temp
    {
        public Temp(string id, string name)
        {
            _id = id;
            _name = name;
        }
        private string _id;
        private string _name;

        public string Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }
    }


}
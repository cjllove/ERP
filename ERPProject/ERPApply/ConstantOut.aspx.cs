﻿using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace ERPProject.ERPApply
{
    public partial class ConstantOut : BillBase
    {
        private string strDocSql = "SELECT * FROM DAT_CK_DOC WHERE BILLTYPE = 'DSC' AND SEQNO ='{0}'";
        private string strComSql = "SELECT D.*, B.HISCODE,F_GETUNITNAME(D.UNIT) UNITNAME, F_GETPRODUCERNAME(D.PRODUCER) PRODUCERNAME,D.FPSL / D.BZHL CH, D.FPSL* D.HSJJ CHJE FROM DAT_CK_COM D, DOC_GOODS B WHERE D.SEQNO = '{0}' AND B.GDSEQ = D.GDSEQ ORDER BY ROWNO";

        protected string strUSERXMID = DbHelperOra.GetSingle("SELECT VALUE FROM SYS_PARA WHERE CODE = 'USERXMID'").ToString();
        protected string DS_SHTXD = "/grf/Ds_Shtx_2.grf";
        public ConstantOut()
        {
            BillType = "DSC";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //屏蔽不需要的操作按钮
                ButtonHidden(btnCopy, btnNext, btnBef, btnExport, btnAddRow);
                hfdCurrent.Text = UserAction.UserID;
                DataInit();
                billNew();
            }
        }

        private void DataInit()
        {
            //PubFunc.DdlDataGet("DDL_SYS_DEPOT", lstDEPTOUT, docDEPTOUT);
            DepartmentBind.BindDDL("DDL_SYS_DEPOTRANGE", UserAction.UserID, lstDEPTOUT, docDEPTOUT);
            DepartmentBind.BindDDL("DDL_SYS_DEPTRANGE", UserAction.UserID, lstDEPTID, docDEPTID);
            PubFunc.DdlDataGet("DDL_USER", lstSLR, docLRY, docSLR);

            //PubFunc.DdlDataGet("DDL_BILL_STATUSCKD", lstFLAG, docFLAG);
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;

            //获取客户化GRF文件地址  By c 2016年1月21日12:18:29 At 威海509
            string grf = Doc.DbGetGrf("DS_SHTXD");
            if (!string.IsNullOrWhiteSpace(grf))
            {
                DS_SHTXD = grf;
            }
        }

        protected override void billNew()
        {
            string strDeptout = docDEPTOUT.SelectedValue;
            string strDeptid = docDEPTID.SelectedValue;
            PubFunc.FormDataClear(FormDoc);

            docFLAG.SelectedValue = "M";
            docSLR.SelectedValue = UserAction.UserID;
            docLRY.SelectedValue = UserAction.UserID;
            docLRRQ.SelectedDate = DateTime.Now;
            docXSRQ.SelectedDate = DateTime.Now;
            docSHRQ.SelectedDate = DateTime.Now;

            docDEPTOUT.SelectedValue = strDeptout;
            docDEPTID.SelectedValue = strDeptid;
            docDEPTOUT.Enabled = true;
            docSLR.Enabled = true;
            docDEPTID.Enabled = true;
            docXSRQ.Enabled = true;
            docMEMO.Enabled = true;
            //改变按钮状态
            BtnPrintJh.Enabled = false;
            btnPrint.Enabled = false;
            btnPrintBQ.Enabled = false;
            btnDel.Enabled = false;
            btnSave.Enabled = true;
            btnAudit.Enabled = false;
            btnDelRow.Enabled = true;
            btnGoods.Enabled = true;
            btnAudit2.Enabled = false;
            btnRtn.Enabled = false;
            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());

            if (Request.QueryString["tp"] != null && Request.QueryString["tp"].ToString().Trim().Length > 0)
            {
                docDEPTOUT.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE=" + Request.QueryString["tp"].ToString()).ToString();
            }
            else
            {
                docDEPTOUT.SelectedValue = DbHelperOra.GetSingle("SELECT T.STR2 FROM DOC_GOODSTYPE T WHERE T.CODE='2'").ToString();
            }
            //处理 本页合计
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("CH", "0");
            summary.Add("CHJE", "0");

            GridGoods.SummaryData = summary;
        }
        private void DataSearch2()
        {
            string sql = "";
            if (DbHelperOra.Exists("SELECT 1 FROM SYS_PARA WHERE CODE = 'ShowName' AND VALUE = 'HIS'"))
            {
                sql = @"SELECT  SP.GDSEQ,SP.GDID,SP.BARCODE,SP.ZJM,SP.YCODE,SP.NAMEJC,SP.NAMEEN,SP.GDMODE,SP.STRUCT,SP.BZHL,SP.UNIT,SP.FLAG,SP.CATID,SP.JX,SP.YX,SP.PIZNO,SP.BAR1,SP.BAR2,SP.BAR3,SP.DEPTID,SP.SUPPLIER,SP.LOGINLABEL,SP.PRODUCER,SP.ZPBH,SP.PPID,SP.CDID,SP.JXTAX,SP.XXTAX,SP.BHSJJ,SP.HSJJ,SP.LSJ,SP.YBJ,SP.HSID,SP.HSJ,SP.JHZQ,SP.ZDKC,SP.HLKC,SP.ZGKC,SP.SPZT,SP.DAYXS,SP.MANAGER,SP.INPER,SP.INRQ,SP.BEGRQ,SP.ENDRQ,SP.UPTRQ,SP.UPTUSER,SP.MEMO,DISABLEORG,SP.ISLOT,SP.ISJB,SP.ISFZ,SP.ISGZ,SP.ISIN,SP.ISJG,SP.ISDM,SP.ISCF,SP.ISYNZJ,SP.ISFLAG1,nvl(SP.STR3,SP.GDSPEC) GDSPEC,SP.UNIT_DABZ,SP.UNIT_ZHONGBZ,SP.BARCODE_DABZ,SP.NUM_DABZ,SP.NUM_ZHONGBZ,SP.UNIT_ORDER,SP.UNIT_SELL,SP.HISCODE,nvl(SP.HISNAME,SP.GDNAME) GDNAME,SP.CATID0,
                                        F_GETUNITNAME(UNIT) UNITNAME,SP.PRODUCER,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME,(nvl(PZ.DSNUM,0) - nvl(PZ.NUM3,0)- nvl(PZ.DSPOOL,0)) sum_num,PZ.DSNUM,nvl(PZ.NUM1,0) NUM_XS,nvl(PZ.NUM3,0) NUM_DS,
                                       F_GETUNITNAME(UNIT_ORDER) UNIT_ORDER_NAME,F_GETUNITNAME(UNIT_SELL) UNIT_SELL_NAME,F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,NVL(PZ.ISJF,'Y') ISJF,
                                       F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,f_gethwid('{1}',SP.GDSEQ) HWID,NVL((SELECT wmsys.wm_concat(gdseq) FROM VIEW_DS WHERE GDSEQ_OLD = SP.GDSEQ AND DEPTID = '{0}'),'不存在') DT";
            }
            else
            {
                sql = @"SELECT  SP.GDSEQ,SP.GDID,SP.BARCODE,SP.ZJM,SP.YCODE,SP.NAMEJC,SP.NAMEEN,SP.GDMODE,SP.STRUCT,SP.BZHL,SP.UNIT,SP.FLAG,SP.CATID,SP.JX,SP.YX,SP.PIZNO,SP.BAR1,SP.BAR2,SP.BAR3,SP.DEPTID,SP.SUPPLIER,SP.LOGINLABEL,SP.PRODUCER,SP.ZPBH,SP.PPID,SP.CDID,SP.JXTAX,SP.XXTAX,SP.BHSJJ,SP.HSJJ,SP.LSJ,SP.YBJ,SP.HSID,SP.HSJ,SP.JHZQ,SP.ZDKC,SP.HLKC,SP.ZGKC,SP.SPZT,SP.DAYXS,SP.MANAGER,SP.INPER,SP.INRQ,SP.BEGRQ,SP.ENDRQ,SP.UPTRQ,SP.UPTUSER,SP.MEMO,DISABLEORG,SP.ISLOT,SP.ISJB,SP.ISFZ,SP.ISGZ,SP.ISIN,SP.ISJG,SP.ISDM,SP.ISCF,SP.ISYNZJ,SP.ISFLAG1,nvl(SP.STR3,SP.GDSPEC) GDSPEC,SP.UNIT_DABZ,SP.UNIT_ZHONGBZ,SP.BARCODE_DABZ,SP.NUM_DABZ,SP.NUM_ZHONGBZ,SP.UNIT_ORDER,SP.UNIT_SELL,SP.HISCODE,SP.GDNAME,SP.CATID0,
                                        F_GETUNITNAME(UNIT) UNITNAME,SP.PRODUCER,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME,(nvl(PZ.DSNUM,0) - nvl(PZ.NUM3,0)- nvl(PZ.DSPOOL,0)) sum_num,PZ.DSNUM,nvl(PZ.NUM1,0) NUM_XS,nvl(PZ.NUM3,0) NUM_DS,
                                       F_GETUNITNAME(UNIT_ORDER) UNIT_ORDER_NAME,F_GETUNITNAME(UNIT_SELL) UNIT_SELL_NAME,F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,NVL(PZ.ISJF,'Y') ISJF,
                                       F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,f_gethwid('{1}',SP.GDSEQ) HWID,NVL((SELECT wmsys.wm_concat(gdseq) FROM VIEW_DS WHERE GDSEQ_OLD = SP.GDSEQ AND DEPTID = '{1}'),'不存在') DT";
            }
            StringBuilder strSql = new StringBuilder(string.Format(sql, docDEPTID.SelectedValue, docDEPTOUT.SelectedValue));
            if (!string.IsNullOrWhiteSpace(docDEPTOUT.SelectedValue))
            {
                strSql.AppendFormat(" ,(select nvl(sum(KCSL -LOCKSL),0) from DAT_GOODSSTOCK a where a.gdseq = SP.GDSEQ and a.deptid = '{0}') KCSL,floor((select nvl(sum(KCSL - LOCKSL),0) from DAT_GOODSSTOCK a where a.gdseq = SP.GDSEQ and a.deptid = '{0}')/PZ.NUM1) SL", docDEPTOUT.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
            {
                strSql.AppendFormat(" from  DOC_GOODS SP,DOC_GOODSCFG PZ WHERE SP.FLAG IN('Y','T') AND ISDELETE='N' AND SP.GDSEQ=PZ.GDSEQ AND PZ.DSNUM > 0 AND nvl(PZ.NUM3,0) + nvl(PZ.DSPOOL,0) <= nvl(PZ.DSNUM,0) and nvl(PZ.NUM1,0) > 0 AND PZ.DEPTID='{0}' AND PZ.ISCFG IN ('1','Y') and (nvl(PZ.DSNUM, 0) - nvl(PZ.NUM3, 0) - nvl(PZ.DSPOOL, 0))>0 AND EXISTS(SELECT 1 FROM DOC_GOODSSUP GS WHERE GS.GDSEQ=SP.GDSEQ AND GS.SUPID IS NOT NULL) ", docDEPTID.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(docDEPTOUT.SelectedValue))
            {
                strSql.AppendFormat(" AND EXISTS(SELECT 1 FROM DOC_GOODSCFG PZ WHERE PZ.GDSEQ=SP.GDSEQ AND PZ.DEPTID ='{0}') ", docDEPTOUT.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(trbSearch.Text))
            {
                strSql.AppendFormat(" AND (SP.GDSEQ LIKE '%{0}%' OR SP.GDNAME LIKE '%{0}%' OR SP.ZJM LIKE '%{0}%' OR SP.BARCODE LIKE '%{0}%')", trbSearch.Text.Trim().ToUpper());
            }
            strSql.Append("    ORDER BY SP.GDNAME,KCSL");
            GridCom.DataSource = DbHelperOra.Query(strSql.ToString()).Tables[0];
            GridCom.DataBind();
        }
        protected void GridList_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                FineUIPro.BoundField flagcol = GridList.FindColumn("FLAG") as FineUIPro.BoundField;
                if (flag == "新单")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color1";
                }
                else if (flag == "已提交" || flag == "已分配")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color2";
                }
                else if (flag == "缺货中")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color3";
                }
            }
        }
        private JObject GetJObject(Dictionary<string, object> dicRecord)
        {
            JObject defaultObj = new JObject();
            defaultObj.Add("GDSEQ", dicRecord["GDSEQ"].ToString());
            defaultObj.Add("BARCODE", dicRecord["BARCODE"].ToString());
            defaultObj.Add("GDNAME", dicRecord["GDNAME"].ToString());
            defaultObj.Add("GDSPEC", dicRecord["GDSPEC"].ToString());
            defaultObj.Add("UNIT", dicRecord["UNIT"].ToString());
            defaultObj.Add("UNITNAME", dicRecord["UNITNAME"].ToString());
            defaultObj.Add("BZHL", dicRecord["BZHL"].ToString());
            defaultObj.Add("BZSL", (dicRecord["BZSL"] ?? "0").ToString());

            defaultObj.Add("HWID", dicRecord["HWID"].ToString());
            defaultObj.Add("PH", dicRecord["PH"].ToString());
            defaultObj.Add("PZWH", dicRecord["PZWH"].ToString());
            defaultObj.Add("RQ_SC", dicRecord["RQ_SC"].ToString());
            defaultObj.Add("YXQZ", dicRecord["YXQZ"].ToString());
            defaultObj.Add("JXTAX", dicRecord["JXTAX"].ToString());
            defaultObj.Add("PRODUCER", dicRecord["PRODUCER"].ToString());
            defaultObj.Add("PRODUCERNAME", dicRecord["PRODUCERNAME"].ToString());

            decimal hl = 0, rs = 0, jg = 0;
            decimal.TryParse((dicRecord["BZHL"] ?? "0").ToString(), out hl);
            decimal.TryParse((dicRecord["BZSL"] ?? "0").ToString(), out rs);
            decimal.TryParse((dicRecord["HSJJ"] ?? "0").ToString(), out jg);

            defaultObj.Add("DHSL", rs * hl);
            defaultObj.Add("HSJJ", (dicRecord["HSJJ"] ?? "0").ToString());
            defaultObj.Add("HSJE", rs * jg);
            defaultObj.Add("ZPBH", (dicRecord["ZPBH"] ?? "0").ToString());
            defaultObj.Add("MEMO", (dicRecord["MEMO"] ?? "0").ToString());
            //defaultObj.Add("ISLOT", dicRecord["ISLOT"].ToString());
            return defaultObj;
        }
        protected void GridGoods_AfterEdit(object sender, FineUIPro.GridAfterEditEventArgs e)
        {
            //int[] intCell = GridGoods.SelectedCell;
            //List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList();
            //if (newDict.Count == 0) return;
            //if (e.ColumnID == "BZSL" || e.ColumnID == "HSJJ")
            //{
            //    string cell = string.Format("[{0},{1}]", e.RowIndex, intCell[1]);
            //    PageContext.RegisterStartupScript(GridGoods.GetSetCellReference(GetJObject(newDict[e.RowIndex]), cell));

            //    if (e.RowIndex != intCell[0])
            //    {
            //        PageContext.RegisterStartupScript(GridGoods.GetSetCellReference(GetJObject(newDict[intCell[0]]), string.Format("[{0},{1}]", intCell[0], intCell[1])));
            //    }

            //    //计算合计数量
            //    decimal bzslTotal = 0, feeTotal = 0;
            //    foreach (Dictionary<string, object> dic in newDict)
            //    {
            //        bzslTotal += Convert.ToDecimal(dic["CH"] ?? "0");
            //        feeTotal += Convert.ToDecimal(dic["HSJJ"] ?? "0") * Convert.ToDecimal(dic["BZHL"] ?? "0") * Convert.ToDecimal(dic["CH"] ?? "0");
            //    }
            //    JObject summary = new JObject();
            //    summary.Add("GDNAME", "本页合计");
            //    summary.Add("CH", bzslTotal.ToString());
            //    summary.Add("CHJE", feeTotal.ToString("F6"));

            //    GridGoods.SummaryData = summary;
            //}
        }

        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlist);
            lstLRRQ1.SelectedDate = DateTime.Now;
            lstLRRQ2.SelectedDate = DateTime.Now;
        }
        protected override void billAddRow()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;

            string strBillno = docSEQNO.Text;
            // 新增数据初始值
            JObject defaultObj = new JObject();
            defaultObj.Add("GDSEQ", "");
            defaultObj.Add("BARCODE", "");
            defaultObj.Add("GDNAME", "");
            defaultObj.Add("GDSPEC", "");
            defaultObj.Add("UNIT", "");
            defaultObj.Add("UNITNAME", "");
            defaultObj.Add("BZHL", "");
            defaultObj.Add("BZSL", "");
            defaultObj.Add("DHSL", "");
            defaultObj.Add("XSSL", "");
            defaultObj.Add("JXTAX", "");
            defaultObj.Add("HSJJ", "");
            defaultObj.Add("HSJE", "");
            defaultObj.Add("ZPBH", "");
            defaultObj.Add("PRODUCER", "");
            defaultObj.Add("PRODUCERNAME", "");
            defaultObj.Add("HWID", "");
            defaultObj.Add("PH", "");
            defaultObj.Add("PZWH", "");
            defaultObj.Add("RQ_SC", "");
            defaultObj.Add("YXQZ", "");
            defaultObj.Add("MEMO", "");
            PubFunc.FormLock(FormDoc, true, "");
            //trbEditorGDSEQ.Enabled = true;
            PageContext.RegisterStartupScript(GridGoods.GetAddNewRecordReference(defaultObj, true));
        }

        protected override void billDelRow()
        {
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("非新增单据不能删除！");
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "M", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            if (GridGoods.SelectedCell == null) return;
            GridGoods.DeleteSelectedRows();
            //int rowIndex = GridGoods.SelectedRowIndex;
            //PageContext.RegisterStartupScript(GridGoods.GetDeleteRowReference(rowIndex));
            PubFunc.FormLock(FormDoc, true, "");
        }

        protected override void billGoods()
        {
            if (PubFunc.FormDataCheck(FormDoc).Length > 1) return;
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("非新单据不允许操作");
                return;
            }
            //参数说明：cx - 查询内容，bm - 商品配置部门,su - 供应商
            //string url = "~/ERPApply/Contant_GoodsWindow_His.aspx?bm=" + docDEPTID.SelectedValue + "&cx=" + docDEPTOUT.SelectedValue + "&su=";
            //PageContext.RegisterStartupScript(Window1.GetSaveStateReference(hfdValue.ClientID) + Window1.GetShowReference(url, "可出定数商品信息查询"));
            PubFunc.FormLock(FormDoc, true, "");
            docMEMO.Enabled = true;
            Window1.Hidden = false;
            DataSearch2();
        }
        protected void GridCom_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {

            string GoodsInfo = GetRowValue(GridGoods.Rows[e.RowIndex]);
            FineUIPro.PageContext.RegisterStartupScript(FineUIPro.ActiveWindow.GetWriteBackValueReference(GoodsInfo) + FineUIPro.ActiveWindow.GetHidePostBackReference());
        }
        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            btnPostBack_Click(null, null);
            Window1.Hidden = true;

        }
        protected void btnPostBack_Click(object sender, EventArgs e)
        {
            if (GridCom.SelectedRowIndexArray.Length < 1)
            {
                Alert.ShowInTop("没有选择请选择需要添加的定数！！", "消息提示！");
                return;
            }
            string billno = "";
            foreach (int index in GridCom.SelectedRowIndexArray)
            {
                billno += GridCom.Rows[index].DataKeys[0].ToString() + ",";
            }
            billno = billno.TrimEnd(',').Replace(",", "','");
            string sql = @"SELECT A.GDSEQ,
                                    A.GDNAME,
                                    A.GDSPEC,
									 A.UNIT,
                                    f_getunitname(A.UNIT) UNITNAME,
                                    b.num1 BZHL,
                                    1 BZSL,
                                     ROUND(A.HSJJ,4),
                                    '' STR2,
                                    (B.NUM1 *  a.hsjj) CHJE,
                                    (B.NUM1 * a.hsjj) HSJE,A.PRODUCER,
                                    f_getsupname(A.PRODUCER) PRODUCERNAME,
                                    F_GETHWID('{2}',A.GDSEQ) HWID,
                                    ''PH,
                                 (NVL(B.DSNUM, 0) - NVL(B.NUM3, 0) - NVL(B.DSPOOL, 0)) CH,
                                 B.DSNUM,
                                    A.PIZNO PZWH,
                                    ''RQ_SC,
                                    ''YXQZ,
                                    B.NUM1 DHSL,A.JXTAX,A.HSJJ
                                FROM DOC_GOODS A, DOC_GOODSCFG B 
                                WHERE B.GDSEQ = A.GDSEQ
                                AND B.GDSEQ IN ('{0}')
                                AND B.DEPTID = '{1}'";
            DataTable dt = DbHelperOra.Query(string.Format(sql, billno, docDEPTID.SelectedValue, docDEPTOUT.SelectedValue)).Tables[0];
            dt = GetNewDT(dt);
            foreach (DataRow row in dt.Rows)
            {
                List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == row["GDSEQ"].ToString()).Count();
                if (sameRowCount > 0)
                {
                    Alert.Show("商品编码为【" + row["GDSEQ"].ToString() + "】的商品已经存在，无需重复填加！");
                    return;
                }
            }

            PubFunc.GridRowAdd(GridGoods, dt);

            GridCom.SelectedRowIndexArray = new int[] { };
        }
        //通过DB实现GRID中多条显示CJL
        private DataTable GetNewDT(DataTable dt)
        {
            DataTable newdt = dt.Copy();
            foreach (DataRow dr in dt.Rows)
            {
                int countch = int.Parse(dr["CH"].ToString());
                for (int i = 0; i < countch; i++)
                {
                    if (i > 0)
                    {
                        newdt.Rows.Add(dr.ItemArray);
                    }
                }
            }
            foreach (DataRow dr in newdt.Rows)
            {
                dr["CH"] = "0";
                dr["CHJE"] = "0";
            }
            DataView dv = newdt.DefaultView;
            dv.Sort = "GDSEQ DESC";
            return dv.ToTable();
        }
        private string GetRowValue(GridRow row)
        {
            string strValue = "";
            for (int i = 0; i < GridGoods.Columns.Count; i++)
            {
                strValue += row.Values[i].ToString() == "" ? "★♀" : row.Values[i].ToString() + "♀";
            }
            return strValue.TrimEnd('♀');
        }
        protected void GridCom_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridCom.PageIndex = e.NewPageIndex;
            DataSearch2();
        }
        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            DataSearch2();

        }
        protected override void billSearch()
        {
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                Alert.Show("输入条件录入日期不正确！");
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                Alert.Show("开始日期大于结束日期，请重新输入！");
                return;
            }

            string strSql = @"SELECT A.SEQNO,A.BILLNO,DECODE(A.FLAG,'M','新单','N','已提交','S','已分配','D','调拨中','W','调拨完成','B','缺货中','已出库') FLAG,F_GETDEPTNAME(A.DEPTID) DEPTID,A.XSRQ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUT,A.SUBSUM,
                                     A.SUBNUM,F_GETUSERNAME(A.SLR) SLR,F_GETUSERNAME(A.LRY) LRY,A.LRRQ,F_GETUSERNAME(A.SHR) SHR,A.SHRQ,A.MEMO,
                                    (SELECT SUM(FPSL*HSJJ) FROM DAT_CK_COM B WHERE B.SEQNO = A.SEQNO) CHJE,(SELECT COUNT(1) FROM DAT_CK_COM B WHERE B.SEQNO = A.SEQNO) CHSL,
                                     NVL((SELECT FUNCTIME FROM SYS_FUNCPRNNUM WHERE FUNCNO = A.SEQNO),0) PRINTNUM
                                from DAT_CK_DOC A
                                WHERE A.BILLTYPE='DSC' AND A.XSTYPE='1' ";
            string strSearch = "";


            if (lstBILLNO.Text.Length > 0)
            {
                strSearch += string.Format(" AND A.BILLNO  LIKE '%{0}%'", lstBILLNO.Text);
            }
            if (lstFLAG.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND A.FLAG='{0}'", lstFLAG.SelectedItem.Value);
            }
            if (lstSLR.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND A.SLR='{0}'", lstSLR.SelectedItem.Value);
            }
            if (lstDEPTID.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND A.DEPTID='{0}'", lstDEPTID.SelectedItem.Value);
            }
            if (lstDEPTOUT.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND DEPTOUT='{0}'", lstDEPTOUT.SelectedItem.Value);
            }
            if (tbxGDSEQ.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND EXISTS (SELECT 1 FROM DAT_CK_COM C,DOC_GOODS B WHERE A.SEQNO=C.SEQNO AND C.GDSEQ = B.GDSEQ AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%'))", tbxGDSEQ.Text.Trim().ToUpper());
            }
            strSearch += string.Format(" AND deptid in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            strSearch += string.Format(" AND A.LRRQ>=TO_DATE('{0}','YYYY-MM-DD')", lstLRRQ1.Text);
            strSearch += string.Format(" AND A.LRRQ <TO_DATE('{0}','YYYY-MM-DD') + 1", lstLRRQ2.Text);

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql += strSearch;
            }
            strSql += " ORDER BY A.BILLNO DESC";
            highlightRows.Text = "";
            highlightyel.Text = "";

            DataTable table = DbHelperOra.Query(strSql).Tables[0];
            string sortField = GridList.SortField;
            string sortDirection = GridList.SortDirection;
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", sortField, sortDirection);

            GridList.DataSource = view1;
            GridList.DataBind();
        }
        private bool SaveSuccess = false;
        protected override void billAudit()
        {
            if (Doc.DbGetSysPara("LOCKSTOCK") == "Y")
            {
                Alert.Show("系统库存已被锁定，请等待物资管理科结存处理完毕再做审核处理！", "消息提醒", MessageBoxIcon.Warning);
                return;
            }
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("非新单不能提交！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "M", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            SaveSuccess = false;
            save("Y");
            if (SaveSuccess == false)
                return;
            SaveSuccess = false;
            //增加定数是否已经被提交出库判断
            if (DbHelperOra.Exists(String.Format(@"SELECT 1 FROM
                    (SELECT GDSEQ,COUNT(1) SL
                    FROM DAT_CK_COM A
                    WHERE A.SEQNO = '{0}'
                    GROUP BY GDSEQ) A,DOC_GOODSCFG B
                    WHERE A.GDSEQ = B.GDSEQ AND B.DEPTID = '{1}'
                    AND A.SL > B.DSNUM - B.DSPOOL - B.NUM3", strBillno, docDEPTID.SelectedValue)))
            {
                Alert.Show("单据【" + strBillno + "】中部分定数已经被出库，请检查！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            

            if (BillOper(strBillno, "DECLARE") == 1)
            {
                Alert.Show("单据【" + strBillno + "】提交成功！");
                OperLog("定数出库", "提交单据【" + docBILLNO.Text + "】");
                billOpen(strBillno);
            }
            //else
            //{
            //    Alert.Show("单据【" + strBillno + "】提交失败,请刷新后重试！", "错误提示", MessageBoxIcon.Error);
            //    billOpen(strBillno);
            //}
        }

        protected override void listRow_DoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].Values[1].ToString());
        }

        protected override void billOpen(string strBillno)
        {
            DataTable dtDoc = DbHelperOra.Query(string.Format(strDocSql, strBillno)).Tables[0];
            PubFunc.FormDataSet(FormDoc, dtDoc.Rows[0]);

            PageContext.RegisterStartupScript(GridGoods.GetRejectChangesReference());
            string str = string.Format(strComSql, strBillno);
            int i = 0;
            DataTable dtBill = DbHelperOra.Query(string.Format(strComSql, strBillno)).Tables[0];
            decimal bzslTotal = 0, feeTotal = 0;
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    //LoadGridRow(row, false, "DSC");
                    bzslTotal += Convert.ToDecimal(row["CH"] ?? "0");
                    feeTotal += Convert.ToDecimal(row["HSJJ"] ?? "0") * Convert.ToDecimal(row["BZHL"] ?? "0") * Convert.ToDecimal(row["CH"] ?? "0");
                }
                /*
               *  修 改 人 ：袁鹏    修改时间：2015-04-13
               *  信息说明：这种加载方法比LoadGridRow(row, false, "DSC")更高效
               *  研发组织：威高讯通信息科技有限公司
               */
                PubFunc.GridRowAdd(GridGoods, dtBill);
            }
            //trbEditorGDSEQ.Enabled = false;
            PubFunc.FormLock(FormDoc, true, "");
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("CH", bzslTotal.ToString());
            summary.Add("CHJE", feeTotal.ToString("F2"));
            GridGoods.SummaryData = summary;
            TabStrip1.ActiveTabIndex = 1;
            //根据状态屏蔽按钮
            if (docFLAG.SelectedValue == "M")
            {
                docMEMO.Enabled = true;
                BtnPrintJh.Enabled = false;
                btnPrint.Enabled = false;
                btnPrintBQ.Enabled = false;
                btnDel.Enabled = true;
                btnSave.Enabled = true;
                btnAudit.Enabled = true;
                btnDelRow.Enabled = true;
                btnGoods.Enabled = true;
                btnAudit2.Enabled = false;
                btnRtn.Enabled = false;
            }
            else if (docFLAG.SelectedValue == "Y")
            {
                BtnPrintJh.Enabled = true;
                btnPrint.Enabled = true;
                btnPrintBQ.Enabled = true;
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                btnAudit2.Enabled = false;
                btnRtn.Enabled = false;
            }
            else if (docFLAG.SelectedValue == "N")
            {
                BtnPrintJh.Enabled = false;
                btnPrint.Enabled = false;
                btnPrintBQ.Enabled = false;
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                btnAudit2.Enabled = false;
                btnRtn.Enabled = true;
            }
            else
            {
                BtnPrintJh.Enabled = false;
                btnPrint.Enabled = false;
                btnPrintBQ.Enabled = false;
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnAudit.Enabled = false;
                btnDelRow.Enabled = false;
                btnGoods.Enabled = false;
                btnAudit2.Enabled = true;
                btnRtn.Enabled = false;
            }
        }

        protected override void billSave()
        {
            save();
        }

        private void save(string flag ="N")
        {
            #region 数据有效性验证
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("非新单不能保存！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "M", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().ToList();
            if (newDict.Count == 0)
            {
                Alert.Show("请输入商品信息", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<Dictionary<string, object>> goodsData = new List<Dictionary<string, object>>();
            //判断是否有空行
            for (int i = 0; i < newDict.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(newDict[i]["GDSEQ"].ToString()))
                {
                    goodsData.Add(newDict[i]);
                }
            }

            if (goodsData.Count == 0)//所有Gird行都为空行时
            {
                Alert.Show("商品信息不能为空", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            //验证单据信息
            if (DbHelperOra.Exists("SELECT 1 FROM dat_ck_doc where seqno = '" + docBILLNO.Text + "'") && docBILLNO.Enabled)
            {
                Alert.Show("您输入的单号已存在,请检查!");
                return;
            }
            #endregion

            if (PubFunc.StrIsEmpty(docBILLNO.Text))
            {
                docSEQNO.Text = BillSeqGet();
                //处理单号前缀
                docSEQNO.Text = "DSC" + docSEQNO.Text.Substring(3, docSEQNO.Text.Length - 3);
                docBILLNO.Text = docSEQNO.Text;
                docBILLNO.Enabled = false;
            }
            MyTable mtType = new MyTable("DAT_CK_DOC");
            mtType.ColRow = PubFunc.FormDataHT(FormDoc);
            mtType.ColRow["SEQNO"] = docBILLNO.Text;
            mtType.ColRow.Add("BILLTYPE", "DSC");
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("XSTYPE", "1");
            mtType.ColRow.Remove("SHRQ");
            decimal subNum = 0;//总金额
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtTypeMx = new MyTable("DAT_CK_COM");
            //先删除单据信息在插入
            cmdList.Add(mtType.DeleteCommand(""));//删除单据台头
            cmdList.Add(new CommandInfo("delete dat_ck_com where seqno='" + docBILLNO.Text + "'", null));//删除单据明细
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);

                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PHID", mtTypeMx.ColRow["PH"]);
                mtTypeMx.ColRow.Add("XSSL", mtTypeMx.ColRow["BZHL"]);
                mtTypeMx.ColRow.Add("BHSJJ", 0);
                mtTypeMx.ColRow.Add("BHSJE", 0);
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                subNum = subNum + decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBSUM", subNum);
            cmdList.AddRange(mtType.InsertCommand());
            DbHelperOra.ExecuteSqlTran(cmdList);
            if(flag == "N")
                Alert.Show("定数出库信息保存成功！");
            OperLog("定数出库", "修改单据【" + docBILLNO.Text + "】");
            billOpen(docBILLNO.Text);
            btnDel.Enabled = true;
            billLockDoc(true);
            SaveSuccess = true;
        }

        /// <summary>
        /// FineUIPro.Grid控件数据加载
        /// </summary>
        /// <param name="row">要加载的行数据</param>
        /// <param name="firstRow">是否插入指定行</param>
        /// <param name="flag">数据来源：NEW-从数据库中获得，用于商品新增时；OLD-从销售单据明细中获得，用于修改或审核时</param>
        private void LoadGridRow(DataRow row, bool firstRow = true, string flag = "NEW")
        {
            decimal price = 0, number = 0;
            decimal.TryParse(row["HSJJ"].ToString(), out price);
            decimal.TryParse(row["BZHL"].ToString(), out number);
            row["HSJE"] = (price * number).ToString();
            JObject defaultObj = new JObject();
            defaultObj.Add("GDSEQ", row["GDSEQ"].ToString());
            defaultObj.Add("BARCODE", row["BARCODE"].ToString());
            defaultObj.Add("GDNAME", row["GDNAME"].ToString());
            defaultObj.Add("GDSPEC", row["GDSPEC"].ToString());
            defaultObj.Add("UNIT", row["UNIT"].ToString());
            defaultObj.Add("UNITNAME", row["UNITNAME"].ToString());
            defaultObj.Add("BZSL", row["BZSL"].ToString());
            defaultObj.Add("CH", row["CH"].ToString());
            defaultObj.Add("BZHL", row["BZHL"].ToString());
            defaultObj.Add("DHSL", "1");
            defaultObj.Add("JXTAX", row["JXTAX"].ToString());
            defaultObj.Add("HSJJ", row["HSJJ"].ToString());
            defaultObj.Add("HSJE", row["HSJE"].ToString());
            defaultObj.Add("CHJE", row["CHJE"].ToString());
            defaultObj.Add("ZPBH", row["ZPBH"].ToString());
            defaultObj.Add("PRODUCER", row["PRODUCER"].ToString());
            defaultObj.Add("PRODUCERNAME", row["PRODUCERNAME"].ToString());
            defaultObj.Add("HWID", row["HWID"].ToString());
            defaultObj.Add("PH", row["PH"].ToString());
            defaultObj.Add("PZWH", row["PZWH"].ToString());
            defaultObj.Add("RQ_SC", row["RQ_SC"].ToString());
            defaultObj.Add("YXQZ", row["YXQZ"].ToString());
            defaultObj.Add("MEMO", row["MEMO"].ToString());
            defaultObj.Add("ISLOT", row["ISLOT"].ToString());
            if (flag == "DSC")
            { defaultObj.Add("STR2", row["STR2"].ToString()); }
            if (firstRow)
            {
                int rowIndex = GridGoods.SelectedRowIndex;
                string deleteScript = GridGoods.GetDeleteRowReference(rowIndex);
                PageContext.RegisterStartupScript(deleteScript + GridGoods.GetAddNewRecordReference(defaultObj, rowIndex));
            }
            else
            {
                PageContext.RegisterStartupScript(GridGoods.GetAddNewRecordReference(defaultObj, true));
            }
        }

        protected void Window1_Close(object sender, WindowCloseEventArgs e)
        {
            string err = "";
            string msg = "";
            DataTable dt = GetGoods(hfdValue.Text);
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns["PIZNO"].ColumnName = "PZWH";
                dt.Columns.Add("PH", Type.GetType("System.String"));
                dt.Columns.Add("RQ_SC", Type.GetType("System.String"));
                dt.Columns.Add("YXQZ", Type.GetType("System.String"));
                dt.Columns.Add("MEMO", Type.GetType("System.String"));
                dt.Columns.Add("BZSL", Type.GetType("System.Int32"));
                dt.Columns.Add("CH", Type.GetType("System.Int32"));
                dt.Columns.Add("DHSL", Type.GetType("System.Int32"));
                dt.Columns.Add("HSJE", Type.GetType("System.Double"));
                dt.Columns.Add("CHJE", Type.GetType("System.Double"));
                foreach (DataRow row in dt.Rows)
                {
                    row["BZSL"] = "1";
                    row["CH"] = "0";
                    row["BZHL"] = row["NUM_XS"];
                    row["DHSL"] = row["BZHL"];
                    row["HSJE"] = Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZHL"]);
                    row["CHJE"] = Convert.ToDecimal(row["HSJJ"]) * Convert.ToDecimal(row["BZHL"]);
                    int k = 0;
                    //数量重新复制为应出数量，不考虑库存
                    k = Convert.ToInt32(row["sum_num"]);
                    //if (DbHelperOra.Exists(string.Format("SELECT 1 FROM VIEW_DS WHERE GDSEQ_OLD = '{0}'", row["GDSEQ"])))
                    //{
                    //    //数量重新复制为应出数量，不考虑库存
                    //    k = Convert.ToInt32(row["sum_num"]);
                    //}
                    //else
                    //{
                    //    if (Convert.ToInt32(row["sum_num"]) < Convert.ToInt32(row["SL"]))
                    //    { k = Convert.ToInt32(row["sum_num"]); }
                    //    else { k = Convert.ToInt32(row["SL"]); }
                    //}
                    if (k < 1)
                    {
                        err = err + "【" + row["GDNAME"] + "】";
                    }

                    //重复的定数商品不再添加 YuaPeng 2010621
                    List<Dictionary<string, object>> newDict = GridGoods.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
                    int sameRowCount = newDict.Where(a => a["GDSEQ"].ToString() == row["GDSEQ"].ToString()).Count();
                    if (sameRowCount <= 0)
                    {
                        for (int i = 0; i < k; i++)
                        {
                            if (string.IsNullOrWhiteSpace(row["HSJJ"].ToString()) || row["HSJJ"].ToString() == "0")
                            {
                                msg += "【" + row["GDSEQ"].ToString() + " | " + row["GDNAME"].ToString() + "】,";
                                Alert.Show("商品" + msg + "【含税进价】为空");
                                continue;
                            }
                            LoadGridRow(row, false);
                        }
                    }
                }
            }
            else
            {
                Alert.Show("系统传值错误！！！", "消息提示", MessageBoxIcon.Warning);
            }
            if (err.Length > 0)
            {
                Alert.Show("商品" + err + "添加失败,请检查商品价格！");
            }
        }
        protected override void billDel()
        {
            if (docBILLNO.Text.Trim() == "")
            {
                Alert.Show("请选择需要删除的单据");
                return;
            }
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("非新单不能删除!");
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "M", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }

            DbHelperOra.ExecuteSql("Delete from DAT_CK_DOC t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            DbHelperOra.ExecuteSql("Delete from DAT_CK_COM t WHERE T.SEQNO ='" + docBILLNO.Text.Trim() + "'");
            Alert.Show("单据删除成功!");
            OperLog("定数出库", "删除单据【" + docBILLNO.Text + "】");
            billNew();
            if (lstLRRQ1.SelectedDate == null || lstLRRQ2.SelectedDate == null)
            {
                return;
            }
            else if (lstLRRQ1.SelectedDate > lstLRRQ2.SelectedDate)
            {
                return;
            }
            billSearch();
        }

        protected void GridList_Sort(object sender, GridSortEventArgs e)
        {
            //highlightRows.Text = "";
            //GridList.SortDirection = e.SortDirection;
            //GridList.SortField = e.SortField;

            //DataTable table = PubFunc.GridDataGet(GridList);
            //DataView view1 = table.DefaultView;
            //view1.Sort = String.Format("{0} {1}", GridList.SortField, GridList.SortDirection);
            //GridList.DataSource = view1;
            //GridList.DataBind();
            billSearch();
        }

        protected void btnAudit2_Click(object sender, EventArgs e)
        {
            if (docFLAG.SelectedValue != "S")
            {
                Alert.Show("未库存分配的单据不允许审核！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            string strBillno = docSEQNO.Text;
            if (!Doc.getFlag(strBillno, "S", BillType))
            {
                Alert.Show("此单据已被其他人处理，请刷新页面更新单据状态！", "警告提示", MessageBoxIcon.Warning);
                return;
            }
            //验证库房是否盘点
            if (DbHelperOra.Exists("SELECT 1 FROM DAT_PD_LOCK WHERE DEPTID IN('" + docDEPTOUT.SelectedValue + "','" + docDEPTID.SelectedValue + "') AND FLAG='N'"))
            {
                Alert.Show("出库库房或申领科室正在盘点,请检查!");
                return;
            }
            if (BillOper(strBillno, "AUDIT") == 1)
            {
                Alert.Show("单据【" + strBillno + "】审核成功！");
                OperLog("定数出库", "审核单据【" + strBillno + "】");
                billOpen(strBillno);
            }
            //else
            //{
            //    Alert.Show("单据【" + strBillno + "】审核失败,请检查！", "错误提示", MessageBoxIcon.Error);
            //    billOpen(strBillno);
            //}
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            Window1.Hidden = true;
        }

        protected void btnCmt_Click(object sender, EventArgs e)
        {
            //批号提交单据
            if (GridList.SelectedRowIDArray.Length < 1)
            {
                Alert.Show("请选择需要提交的单据！", MessageBoxIcon.Warning); return;
            }
            String billno = "", flag = "", sed = "", err = "";
            foreach (int indx in GridList.SelectedRowIndexArray)
            {
                flag = GridList.DataKeys[indx][1].ToString();
                if (flag == "新单")
                {
                    billno = GridList.DataKeys[indx][0].ToString();
                    //增加定数是否已经被提交出库判断
                    if (DbHelperOra.Exists(String.Format(@"SELECT 1 FROM
                    (SELECT GDSEQ,B.DEPTID,COUNT(1) SL
                    FROM DAT_CK_COM A,DAT_CK_DOC B
                    WHERE A.SEQNO = '{0}' AND A.SEQNO = B.SEQNO
                    GROUP BY GDSEQ,DEPTID) A,DOC_GOODSCFG B
                    WHERE A.GDSEQ = B.GDSEQ AND A.DEPTID = B.DEPTID
                    AND A.SL > B.DSNUM - B.DSPOOL - B.NUM3", billno)))
                    {
                        Alert.Show("单据【" + billno + "】中部分定数已经被出库，请检查！", "提示信息", MessageBoxIcon.Warning);
                        return;
                    }

                    if (BillOper(billno, "DECLARE") == 1)
                    {
                        sed += "【" + billno + "】";
                        OperLog("定数出库", "提交单据【" + billno + "】");
                    }
                    else
                    {
                        err += "【" + billno + "】";
                    }
                }
            }
            if (err.Length > 0 && sed.Length > 0)
            {
                Alert.Show("单据" + sed + "提交成功！单据" + err + "提交失败！", MessageBoxIcon.Warning);
            }
            else if (sed.Length > 0)
            {
                Alert.Show("单据" + sed + "提交成功!");
            }
            else
            {
                Alert.Show("单据" + err + "提交失败！", MessageBoxIcon.Warning);
            }
            billSearch();
        }

        protected void btnRtn_Click(object sender, EventArgs e)
        {
            if (docBILLNO.Text.Length < 1)
            {
                Alert.Show("请选择需要回撤的单据！", MessageBoxIcon.Warning);
                return;
            }
            Object flag = DbHelperOra.GetSingle(String.Format("SELECT FLAG FROM DAT_CK_DOC WHERE SEQNO = '{0}'", docBILLNO.Text));
            if ((flag ?? "").ToString() != "N")
            {
                Alert.Show("单据状态【" + flag + "】不正确，不允许回撤操作！", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.ExecuteSql(String.Format("UPDATE DAT_CK_DOC SET FLAG = 'M',SPR = '{1}',SPRQ = SYSDATE WHERE SEQNO ='{0}'", docBILLNO.Text, UserAction.UserID)) > 0)
            {
                Alert.Show("单据回撤成功！");
                OperLog("定数出库", "回撤【" + docBILLNO.Text + "】");
                billOpen(docBILLNO.Text);
                return;
            }
            else
            {
                Alert.Show("单据回撤失败，请联系管理员！", MessageBoxIcon.Warning);
                return;
            }
        }
    }
}
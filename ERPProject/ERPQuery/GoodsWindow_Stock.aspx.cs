using FineUIPro;
using XTBase;
using System;
using System.Collections.Specialized;
using System.Data;
using System.Text;

namespace ERPProject.ERPQuery
{
    public partial class GoodsWindow_Stock : PageBase
    {
        public GoodsWindow_Stock()
        {
            ISCHECK = false;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //ymh重新定义传递参数

                if (Request.QueryString["Deptout"] != null && Request.QueryString["Deptout"].ToString() != "")
                {
                    //出库部门，必须定义，否则无出库库存数
                    hfdDeptOut.Text = Request.QueryString["Deptout"].ToString();
                }

                if (Request.QueryString["DeptIn"] != null && Request.QueryString["DeptIn"].ToString() != "")
                {
                    //入库部门
                    hfdDeptIn.Text = Request.QueryString["DeptIn"].ToString();
                }

                if (Request.QueryString["Sup"] != null && Request.QueryString["Sup"].ToString() != "")
                {
                    //供应商
                    hfdSupplier.Text = Request.QueryString["Sup"].ToString();
                }

                if (Request.QueryString["Pss"] != null && Request.QueryString["Pss"].ToString() != "")
                {
                    //配送商
                    hfdProvider.Text = Request.QueryString["Pss"].ToString();
                }

                if (Request.QueryString["Pde"] != null && Request.QueryString["Pde"].ToString() != "")
                {
                    //生产商
                    hfdProducer.Text = Request.QueryString["Pde"].ToString();
                }

                if (Request.QueryString["Shs"] != null && Request.QueryString["Shs"].ToString() != "")
                {
                    //送货商
                    hfdShs.Text = Request.QueryString["Shs"].ToString();
                }

                if (Request.QueryString["goodsType"] != null && Request.QueryString["goodsType"].ToString() != "")
                {
                    //商品类型
                    hfdGoodsType.Text = Request.QueryString["goodsType"].ToString();
                }

                if (Request.QueryString["isGZ"] != null && Request.QueryString["isGZ"].ToString() != "")
                {
                    //高值
                    hfdisGZ.Text = Request.QueryString["isGZ"].ToString();
                }

                if (Request.QueryString["isdg"] != null && Request.QueryString["isdg"].ToString() != "")
                {
                    //是否代管
                    hfdISDG.Text = Request.QueryString["isdg"].ToString();
                }
                if (Request.QueryString["isSum"] != null && Request.QueryString["isSum"].ToString() != "")
                {
                    //库存汇总
                    hfdSum.Text = Request.QueryString["isSum"].ToString();
                }
                DataSearch();
                btnClose.OnClientClick = ActiveWindow.GetHideReference();
                BillBase.Grid_Goods = GridGoods;
            }
        }
        private String GetSql()
        {
            string sql = "";
            #region 语句
            //使用his名称、规格,SP.GDNAME,SP.GDSPEC
            sql = @"SELECT SP.GDSEQ,
                           SP.GDID,
                           SP.BARCODE,
                           SP.ZJM,
                           SP.YCODE,
                           SP.NAMEJC,
                           SP.NAMEEN,
                           SP.GDMODE,
                           SP.STRUCT,
                           SP.BZHL,
                           SP.UNIT,
                           SP.FLAG,
                           SP.CATID,
                           SP.JX,
                           SP.YX,
                           SP.PIZNO,
                           SP.BAR1,
                           SP.BAR2,
                           SP.BAR3,
                           SP.DEPTID,
                           --SP.SUPPLIER,
                           SP.LOGINLABEL,
                           SP.PRODUCER,
                           SP.ZPBH,
                           SP.PPID,
                           SP.CDID,
                           SP.JXTAX,
                           SP.XXTAX,
                           SP.BHSJJ,
                           SP.HSJJ,
                           SP.LSJ,
                           SP.YBJ,
                           SP.HSID,
                           SP.HSJ,
                           SP.JHZQ,
                           SP.ZDKC,
                           SP.HLKC,
                           SP.ZGKC,
                           SP.SPZT,
                           SP.DAYXS,
                           SP.MANAGER,
                           SP.INPER,
                           SP.INRQ,
                           SP.BEGRQ,
                           SP.ENDRQ,
                           SP.UPTRQ,
                           SP.UPTUSER,
                           SP.MEMO,
                           DISABLEORG,
                           SP.ISLOT,
                           SP.ISJB,
                           SP.ISFZ,
                           SP.ISGZ,
                           SP.ISIN,
                           SP.ISJG,
                           SP.ISDM,
                           SP.ISCF,
                           SP.ISYNZJ,
                           SP.ISFLAG1,
                           NVL(SP.STR3, SP.GDSPEC) GDSPEC,
                           SP.UNIT_DABZ,
                           SP.UNIT_ZHONGBZ,
                           SP.BARCODE_DABZ,
                           SP.NUM_DABZ,
                           SP.NUM_ZHONGBZ,
                           SP.HISCODE,
                           NVL(SP.HISNAME, SP.GDNAME) GDNAME,
                           SP.CATID0,
                           F_GETUNITNAME(UNIT) UNITNAME,
                           F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,
                           F_GETSUPNAME(DP.SUPID) SUPNAME,
                           F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,
                           F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,
                           NVL(PZ.HJCODE1, PZ.DEPTID) HWID,
                           DECODE(SP.UNIT_ORDER,
                                  'D',
                                  SP.UNIT_DABZ,
                                  'Z',
                                  SP.UNIT_ZHONGBZ,
                                  SP.UNIT) UNIT_ORDER,
                           DECODE(SP.UNIT_SELL,
                                  'D',
                                  SP.UNIT_DABZ,
                                  'Z',
                                  SP.UNIT_ZHONGBZ,
                                  SP.UNIT) UNIT_SELL,
                           F_GETUNITNAME(DECODE(SP.UNIT_SELL,
                                                'D',
                                                SP.UNIT_DABZ,
                                                'Z',
                                                SP.UNIT_ZHONGBZ,
                                                SP.UNIT)) UNIT_SELL_NAME,
                           F_GETUNITNAME(DECODE(SP.UNIT_ORDER,
                                                'D',
                                                SP.UNIT_DABZ,
                                                'Z',
                                                SP.UNIT_ZHONGBZ,
                                                SP.UNIT)) UNIT_ORDER_NAME,
                           DECODE(SP.UNIT_ORDER, 'D', SP.NUM_DABZ, 'Z', SP.NUM_ZHONGBZ, SP.BZHL) BZHL_ORDER,
                           DECODE(SP.UNIT_SELL, 'D', SP.NUM_DABZ, 'Z', SP.NUM_ZHONGBZ, SP.BZHL) BZHL_SELL,
                           SP.STR0,PZ.ISJF,SP.PIZNO PZWH,DP.SUPID,SK.PH,SK.YXQZ,SK.RQ_SC,SK.KCSL
                      FROM DOC_GOODS SP, DOC_GOODSCFG PZ,DOC_GOODSSUP DP,({0}) SK
                     WHERE SP.ISDELETE = 'N'
                       AND SP.FLAG = 'Y'                       
                       AND SP.GDSEQ = PZ.GDSEQ AND SP.GDSEQ = DP.GDSEQ
                       AND SP.GDSEQ = SK.GDSEQ AND PZ.DEPTID = SK.DEPTID";
            String SqlSum = "";
            if (hfdSum.Text.Length > 0)
            {
                //库存汇总信息
                SqlSum = String.Format(@"SELECT DEPTID,GDSEQ,SUM(KCSL) KCSL,'' PH,'' YXQZ,'' RQ_SC
                        FROM DAT_GOODSSTOCK
                        WHERE KCSL > 0 AND DEPTID = '{0}'
                        GROUP BY DEPTID,GDSEQ", hfdDeptOut.Text);
            }
            else
            {
                //库存明细
                SqlSum = String.Format(@"SELECT DEPTID,GDSEQ,SUM(KCSL) KCSL,PH,YXQZ,RQ_SC
                        FROM DAT_GOODSSTOCK
                        WHERE KCSL > 0 AND DEPTID = '{0}'
                        GROUP BY DEPTID,GDSEQ,PH,YXQZ,RQ_SC", hfdDeptOut.Text);
            }
            sql = String.Format(sql, SqlSum);
            #endregion
            StringBuilder strSql = new StringBuilder(sql);
            if (hfdDeptOut.Text.Trim().Length > 0)
            {
                strSql.AppendFormat(" AND PZ.DEPTID='{0}' AND PZ.ISCFG in('1','Y')", hfdDeptOut.Text);
            }
            if (Request.QueryString["isbd"] != null)
            {
                //是否是本地商品
                strSql.AppendFormat(" AND ISFLAG7='" + Request.QueryString["isbd"].ToString() + "'");
            }
            if (!string.IsNullOrWhiteSpace(hfdSearch.Text))
            {
                strSql.AppendFormat(" AND (SP.GDSEQ LIKE '%{0}%' OR SP.GDNAME LIKE '%{0}%' OR SP.ZJM LIKE '%{0}%' OR SP.BARCODE LIKE '%{0}%' OR SP.HISCODE LIKE '%{0}%' OR SP.HISNAME LIKE '%{0}%' OR SP.STR4 LIKE '%{0}%')", hfdSearch.Text.ToUpper());
            }

            if (!string.IsNullOrWhiteSpace(hfdDeptIn.Text))
            {
                if (hfdDeptOut.Text.Trim().Length > 0)
                {
                    strSql.AppendFormat(" AND EXISTS (SELECT GDSEQ FROM DOC_GOODSCFG WHERE DEPTID = '{0}' AND PZ.ISCFG IN ('1','Y') AND GDSEQ = PZ.GDSEQ)", hfdDeptIn.Text);
                }
                else
                {
                    strSql.AppendFormat(" AND EXISTS (SELECT GDSEQ FROM DOC_GOODSCFG WHERE DEPTID = '{0}' AND ISCFG IN ('1','Y') AND GDSEQ = SP.GDSEQ)", hfdDeptIn.Text);
                }
            }

            if (!string.IsNullOrWhiteSpace(hfdSupplier.Text))
            {
                //供应商
                strSql.AppendFormat(" AND DP.SUPID = '{0}'", hfdSupplier.Text);
            }
            if (!string.IsNullOrWhiteSpace(hfdProvider.Text))
            {
                //配送商
                strSql.AppendFormat(" DP.PSSID = '{0}'", hfdProvider.Text);
            }
            if (hfdShs.Text.Length > 0)
            {
                //送货商
                strSql.AppendFormat(" AND NVL(DP.PSSID,DP.SUPID) = '{0}'", hfdShs.Text);
            }
            if (hfdGoodsType.Text.Length > 0)
            {
                strSql.AppendFormat(" AND SP.CATID0 = '{0}'", hfdGoodsType.Text);
            }
            if (hfdISDG.Text == "Y")
            {
                //代管
                strSql.AppendFormat(" AND DP.TYPE IN ='1'");
            }
            else if (hfdISDG.Text == "N")
            {
                //托管或直送
                strSql.AppendFormat(" AND DP.TYPE IN ('0','Z')");
            }
            if (hfdisGZ.Text.Length > 0)
            {
                if (hfdisGZ.Text == "Y")
                    strSql.AppendFormat(" AND SP.ISGZ = 'Y'");
                else
                    strSql.AppendFormat(" AND SP.ISGZ = 'N'");
            }
            strSql.AppendFormat(" ORDER BY SP.{0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            return strSql.ToString();
        }
        private void DataSearch()
        {
            int total = 0;
            //获取显示名称标记
            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, GetSql(), ref total);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }

        protected void GridGoods_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            string GoodsInfo = GetRowValue(GridGoods.Rows[e.RowIndex]);
            FineUIPro.PageContext.RegisterStartupScript(FineUIPro.ActiveWindow.GetWriteBackValueReference(GoodsInfo) + FineUIPro.ActiveWindow.GetHidePostBackReference());
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

        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            string GoodsInfo = "";
            int[] row = GridGoods.SelectedRowIndexArray;
            foreach (int index in row)
            {
                GoodsInfo += GetRowValue(GridGoods.Rows[index]) + "♂";
            }
            FineUIPro.PageContext.RegisterStartupScript(FineUIPro.ActiveWindow.GetWriteBackValueReference(GoodsInfo.TrimEnd(';')) + FineUIPro.ActiveWindow.GetHidePostBackReference());
        }
        protected void btnPostBack_Click(object sender, EventArgs e)
        {
            string GoodsInfo = "";
            int[] row = GridGoods.SelectedRowIndexArray;
            foreach (int index in row)
            {
                GoodsInfo += GetRowValue(GridGoods.Rows[index]) + "♂";
            }
            GridGoods.SelectedRowIndexArray = new int[] { };
            FineUIPro.PageContext.RegisterStartupScript("PosGoods('" + GoodsInfo.TrimEnd(';') + "')");
        }

        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            hfdSearch.Text = trbSearch.Text.Trim();
            DataSearch();
        }

        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;
            DataSearch();
        }
        #region 联想
        public DataTable GetGoods(int pageNum, int pageSize, NameValueCollection nvc, ref int total, ref string errMsg)
        {
            hfdSearch.Text = Doc.filterSql(trbSearch.Text);
            return GetDataTable(pageNum, pageSize, GetSql(), ref total);
        }
        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            //if (e.EventArgument.IndexOf("trbSearch_change_") >= 0)
            //{
            //    SetGoods();
            //}
            //if (e.EventArgument.IndexOf("Grid2_bind_") >= 0)
            //{
            //    SetGoods();
            //}
        }
        //protected void Grid2_PageIndexChange(object sender, GridPageEventArgs e)
        //{
        //    Grid2.PageIndex = e.NewPageIndex;
        //    SetGoods();
        //}
        //protected void SetGoods()
        //{
        //    int total = 0;
        //    string msg = "";
        //    NameValueCollection nvc = new NameValueCollection();
        //    DataTable dtData = GetGoods(Grid2.PageIndex, Grid2.PageSize, nvc, ref total, ref msg);
        //    Grid2.RecordCount = total;
        //    Grid2.DataSource = dtData;
        //    Grid2.DataBind();
        //}
        #endregion
    }
}
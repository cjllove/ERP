using XTBase;
using System;
using System.Data;
using FineUIPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace ERPProject.ERPQuery
{
    public partial class GoodsQuery : PageBase
    {
        private string goodsListSql = @"SELECT  SP.*,
                                      F_GETUNITNAME(SP.UNIT) UNITNAME,
                                     f_getcatid0name(SP.CATID0) CATID0NAME,--E.NAME CATID0NAME,
                                      F_GETSUPNAME(GS.SUPID) SUPNAME,F_GETSUPNAME(GS.PSSID) PSSNAME,DECODE(GS.TYPE,'0','托管','1','代管','Z','直供','') TYPE,
                                      F_GETCATNAME(SP.CATID) CATIDNAME,DECODE(GS.STR3,'R','入库结','C','出库结','X','销售结','') STR3NAME,
                                      F_GETUNITNAME(DECODE(SP.UNIT_SELL,'D',SP.UNIT_DABZ,'Z',SP.UNIT_ZHONGBZ,SP.UNIT)) UNITSELLNAME,
                                       DECODE(SP.UNIT_ORDER,'D',SP.NUM_DABZ,'Z',SP.NUM_ZHONGBZ,SP.BZHL) NUMORDER,
                                       F_GETUNITNAME(DECODE(SP.UNIT_ORDER,'D',SP.UNIT_DABZ,'Z',SP.UNIT_ZHONGBZ,SP.UNIT)) UNITORDERNAME,
                                       DECODE(SP.UNIT_SELL,'D',SP.NUM_DABZ,'Z',SP.NUM_ZHONGBZ,SP.BZHL) NUMSELL,
                                      F_GETGOODSJX(SP.JX) JXNAME,
                                      F_GETGOODSYX(SP.YX) YXNAME,
                                      S.NAME FLAGNAME,
                                      DECODE(SP.ISFLAG7,'Y','本地','下传') ISFLAG7_CN,
                                      F.SUPNAME CD,
                                      F_GETGOODSKFCFG(SP.GDSEQ) KFCFG
                                  FROM DOC_GOODS SP,
                                       SYS_CATEGORY B,
                                       DOC_GOODSTYPE E,
                                       DOC_SUPPLIER F,
                                       doc_goodssup GS,
                                        (SELECT CODE, NAME FROM SYS_CODEVALUE WHERE TYPE = 'GOODS_STATUS') S
                                 WHERE SP.ISDELETE = 'N'
                                   AND SP.CATID = B.CODE(+)
                                   AND SP.FLAG = S.CODE
                                   AND B.TYPE = E.CODE(+)
                                   AND SP.PRODUCER = F.SUPID(+)
                                   AND SP.GDSEQ=GS.GDSEQ(+)";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString.Get("osid") == null)
                {
                    DataInit();
                }
                else {
                    if (Request.QueryString.Get("osid") == "querylist")
                    {
                        string result = listSearchForPad();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();
                    }
                    else if (Request.QueryString.Get("osid") == "query")
                    {
                        string result = detailSearchForPad();
                        Response.ContentType = "text/plain";
                        Response.Write(result);
                        Response.End();
                    }

                }
                
            }
        }

        private void DataInit()
        {
            PubFunc.DdlDataGet(ddlCATID0, "DDL_GOODS_TYPE");
            PubFunc.DdlDataGet(ddlSUPID, "DDL_DOC_SUPPLIERNULL");
            PubFunc.DdlDataGet(ddlPASSID, "DDL_DOC_PSSNAME");
        }

        private string detailSearchForPad() {
            string gdseq = Request.QueryString.Get("gdseq");
            JObject jo = new JObject();
            
            if (string.IsNullOrWhiteSpace(gdseq)) {
                jo.Add("result", "fail");
                jo.Add("data","商品编码不能为空");
                return JsonConvert.SerializeObject(jo);
            }
            DataTable dt = DbHelperOra.Query("select * from doc_goods where gdseq='" + gdseq + "'").Tables[0];
            DataTable dtPic = DbHelperOra.Query(string.Format(@"
                select * from (
                select gdseq,rowno,gdpict,str1,decode(flag,'N','新增','Y','已发布','E','已撤回') flag,PICPATH, ROWNUM HANGNO from doc_goodspicture where gdseq = '{0}')
                order by rowno", gdseq)).Tables[0];
            jo.Add("result", "success");
            jo.Add("data", JsonConvert.SerializeObject(dt));
            jo.Add("picdata", JsonConvert.SerializeObject(dtPic));
            string result = JsonConvert.SerializeObject(jo);
            return result;
        }

        private string listSearchForPad() {
            string pagesize = Request.QueryString.Get("pagesize");
            string pageindex = Request.QueryString.Get("pageindex");
            int total = 0;
            JObject jo = new JObject();
            DataTable dtData = PubFunc.DbGetPage(Convert.ToInt16(pageindex)-1, Convert.ToInt16(pagesize), GetGoodsListForPad(), ref total);
            jo.Add("result", "success");
            jo.Add("data", JsonConvert.SerializeObject(dtData));
            jo.Add("total", total);
            int totalPage = total / Convert.ToInt16(pagesize);
            if (total % Convert.ToInt16(pagesize) > 0) {
                totalPage = totalPage + 1;
            }
            jo.Add("totalpage", totalPage);
            string result = JsonConvert.SerializeObject(jo);
            return result;
        }
        private string GetGoodsListForPad() {
            string strSearch = "";
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("xSearch")))
            {
                strSearch += string.Format(" AND (SP.GDSEQ LIKE '%{0}%' OR SP.GDNAME LIKE '%{0}%' OR SP.BAR3 LIKE '%{0}%' OR HISCODE LIKE '%{0}%' OR ZJM LIKE '%{0}%' OR STR4 LIKE '%{0}%')", Request.QueryString.Get("xSearch"));
                
            }
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("CATID0")))
            {
                strSearch += string.Format(" AND SP.CATID0='{0}'", Request.QueryString.Get("CATID0"));
            }

            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("SUPID")))
            {
                strSearch += string.Format(" AND GS.SUPID='{0}'", Request.QueryString.Get("SUPID"));
            }

            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("PASSID")))
            {
                strSearch += string.Format(" AND GS.PSSID='{0}'", Request.QueryString.Get("PASSID"));
            }
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("FLAG")))
            {
                strSearch += string.Format(" AND SP.FLAG='{0}'", Request.QueryString.Get("FLAG"));
            }
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("ODE")))
            {
                strSearch += string.Format(" AND GS.STR3='{0}'", Request.QueryString.Get("ODE"));
            }
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("PRO")))
            {
                strSearch += string.Format(" AND F.SUPNAME LIKE '%{0}%'", Request.QueryString.Get("PRO"));
            }
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("dsMode")))
            {
                strSearch += string.Format(" AND GS.TYPE = '{0}'", Request.QueryString.Get("dsMode"));
            }
            if (!string.IsNullOrWhiteSpace(Request.QueryString.Get("ISFLAG7")))
            {
                strSearch += string.Format(" AND SP.ISFLAG7 = '{0}'", Request.QueryString.Get("ISFLAG7"));
            }
            string strGoods = goodsListSql;
            return strGoods + strSearch;
        }

        private void DataSearch()
        {
            int total = 0;
            DataTable dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, GetGoodsList(), ref total);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }

        protected void GridGoods_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }
        public String GetGoodsList()
        {
            string strSearch = "";
            string strGoods = goodsListSql;
            if (trbxSearch.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND (SP.GDSEQ LIKE '%{0}%' OR SP.GDNAME LIKE '%{0}%' OR SP.BAR3 LIKE '%{0}%' OR HISCODE LIKE '%{0}%' OR ZJM LIKE '%{0}%' OR STR4 LIKE '%{0}%'", trbxSearch.Text);
                strSearch += string.Format(" OR SP.GDSEQ LIKE '%{0}%' OR SP.GDNAME LIKE '%{0}%' OR SP.BAR3 LIKE '%{0}%' OR HISCODE LIKE '%{0}%' OR ZJM LIKE '%{0}%' OR STR4 LIKE '%{0}%')", trbxSearch.Text.ToUpper());
            }
            if (ddlCATID0.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND SP.CATID0='{0}'", ddlCATID0.SelectedValue);
            }
            if (ddlSUPID.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND GS.SUPID='{0}'", ddlSUPID.SelectedValue);
            }
            if (ddlPASSID.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND GS.PSSID='{0}'", ddlPASSID.SelectedValue);
            }
            if (ddlFLAG.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND SP.FLAG = '{0}'", ddlFLAG.SelectedValue);
            }
            if (JSMODE.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND GS.STR3 = '{0}'", JSMODE.SelectedValue);
            }
            if (ddlSTR6.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND SP.STR6='{0}' ",ddlSTR6.SelectedValue);
            }
            if (tgbPRO.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND F.SUPNAME LIKE '%{0}%'", tgbPRO.Text.Trim());
            }
            if (GoodsMode.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND GS.TYPE = '{0}'", GoodsMode.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(ddlISFLAG7.SelectedValue))
            {
                strSearch += string.Format(" AND SP.ISFLAG7 = '{0}'", ddlISFLAG7.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(ddlISGZ.SelectedValue))
            {
                strSearch += string.Format(" AND SP.ISGZ = '{0}'", ddlISGZ.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(ddlISTY.SelectedValue))
            {
                strSearch += string.Format(" AND SP.FLAG = '{0}'", ddlISTY.SelectedValue);
            }
            return strGoods + strSearch;
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            if (GridGoods.Rows.Count < 1)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string strSearch = "";
            string strGoods = @"SELECT 
                               ' '||SP.GDID 商品编码,
                               SP.BAR3 ERP编码,
                               SP.GDNAME 商品名称,
                               S.NAME 状态,
                               E.NAME 商品种类,
                               F_GETCATNAME(SP.CATID) 商品类别,
                               SP.GDSPEC 规格容量,
                               F_GETUNITNAME(SP.UNIT) 最小单位,
                               SP.HSJJ 含税进价,
                               SP.LSJ 售价,
                               F_GETUNITNAME(DECODE(SP.UNIT_SELL, 'D', SP.UNIT_DABZ, 'Z', SP.UNIT_ZHONGBZ, SP.UNIT)) 订货单位,
                               DECODE(SP.UNIT_ORDER, 'D', SP.NUM_DABZ, 'Z', SP.NUM_ZHONGBZ, SP.BZHL) 订货单位含量,
                               F_GETUNITNAME(DECODE(SP.UNIT_ORDER, 'D', SP.UNIT_DABZ, 'Z', SP.UNIT_ZHONGBZ, SP.UNIT)) 出库单位,
                               DECODE(SP.UNIT_SELL, 'D', SP.NUM_DABZ, 'Z', SP.NUM_ZHONGBZ, SP.BZHL) 出库单位含量,
                               DECODE(GS.STR3,'R','入库结','C','出库结','X','销售结','') 结算模式,
                               F.SUPNAME 产地,
                               F_GETSUPNAME(GS.PSSID) 配送商,
                               F_GETSUPNAME(GS.SUPID) 供应商,
                               SP.PIZNO 注册证号,
                               ' '||SP.HISCODE HIS编码,
                               SP.HISNAME HIS名称,
                               SP.STR3 HIS规格
                          FROM DOC_GOODS SP,
                                       SYS_CATEGORY B,
                                       DOC_GOODSTYPE E,
                                       DOC_SUPPLIER F,
                                       doc_goodssup GS,
                                        (SELECT CODE, NAME FROM SYS_CODEVALUE WHERE TYPE = 'GOODS_STATUS') S
                                 WHERE SP.ISDELETE = 'N'
                                   AND SP.CATID = B.CODE(+)
                                   AND SP.FLAG = S.CODE
                                   AND B.TYPE = E.CODE(+)
                                   AND SP.PRODUCER = F.SUPID(+)
                                   AND SP.GDSEQ=GS.GDSEQ(+)";

            if (trbxSearch.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND (SP.GDSEQ LIKE '%{0}%' OR SP.GDNAME LIKE '%{0}%' OR SP.BAR3 LIKE '%{0}%' OR HISCODE LIKE '%{0}%' OR ZJM LIKE '%{0}%' OR STR4 LIKE '%{0}%')", trbxSearch.Text);
            }
            if (ddlCATID0.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND SP.CATID0='{0}'", ddlCATID0.SelectedValue);
            }
            if (ddlSUPID.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND GS.SUPID='{0}'", ddlSUPID.SelectedValue);
            }
            if (ddlPASSID.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND GS.PSSID='{0}'", ddlPASSID.SelectedValue);
            }
            if (ddlFLAG.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND SP.FLAG = '{0}'", ddlFLAG.SelectedValue);
            }
            if (JSMODE.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND GS.STR3 = '{0}'", JSMODE.SelectedValue);
            }
            if (tgbPRO.Text.Trim().Length > 0)
            {
                strSearch += string.Format(" AND F.SUPNAME LIKE '%{0}%'", tgbPRO.Text.Trim());
            }
            if (GoodsMode.SelectedValue.Length > 0)
            {
                strSearch += string.Format(" AND GS.TYPE = '{0}'", GoodsMode.SelectedValue);
            }
            strGoods += strSearch;
            DataTable dt = DbHelperOra.Query(strGoods).Tables[0];

            XTBase.Utilities.ExcelHelper.ExportByWeb(dt, "商品资料导出", "商品资料导出_" + DateTime.Now.ToString("yyyyMMddHH") + ".xls");
        }
    }
}
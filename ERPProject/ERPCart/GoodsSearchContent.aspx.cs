using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XTBase;
using System.Data;
using System.IO;

namespace ERPProject.ERPCart
{
    public partial class GoodsSearchContent : PageBase
    {
        private int intPageIndex = 1;
        public string strPage = "1";
        public string strPath = "";
        public string strLastPage = "";
        public string strNextPage = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["page"] != null)
            {
                strPage = Request.QueryString["page"];
                try
                {
                    intPageIndex = Convert.ToInt32(strPage);
                }
                catch
                {
                    intPageIndex = 1;
                }
            }

            strPath = ApiUtil.GetConfigCont("PIC_PATH");
            if (!IsPostBack)
            {
                goodsDataBind();
            }
        }

        private void goodsDataBind()
        {
            strNextPage = "?page=" + (intPageIndex + 1).ToString();
            strLastPage = "?page=" + (intPageIndex - 1).ToString();
            string strWhere = "";
            if (DbHelperOra.Exists("SELECT 1 FROM SYS_DEPT WHERE TYPE IN ('3','4') AND CODE = '" + UserAction.UserDept + "'"))
            {
                Object obj;
                if (HttpContext.Current.Request.Cookies["DEPTOUT"] == null)
                {
                    obj = DbHelperOra.GetSingle("select nvl((SELECT A.STOCK FROM SYS_DEPT A WHERE A.CODE = '" + UserAction.UserDept + "'),(select value from sys_para where code = 'DEFDEPT')) from dual");
                }
                else
                {
                    obj = HttpContext.Current.Request.Cookies["DEPTOUT"].Value;
                }
                strWhere = " GDSEQ IN(SELECT A.GDSEQ FROM DOC_GOODSCFG A,DOC_GOODSCFG B ,doc_goods C WHERE A.GDSEQ = B.GDSEQ AND A.GDSEQ=C.GDSEQ AND C.FLAG='Y' AND A.ISCFG IN ('Y','1') AND A.DEPTID = '" + UserAction.UserDept + "' AND B.DEPTID = '" + obj.ToString() + "') ";
            }
            else
            {
                strWhere = " 1=2";
            }

            if (Request.QueryString["k"] != null && !"".Equals(Request.QueryString["k"]))
            {
                strWhere += " and (gdname like '%" + Request.QueryString["k"].Trim() + "%' OR GDSEQ like '%" + Request.QueryString["k"].Trim() + "%' OR ZJM like '%" + Request.QueryString["k"].Trim().ToUpper() + "%')";
                strNextPage = strNextPage + "&k=" + Request.QueryString["k"].Trim();
                strLastPage = strLastPage + "&k=" + Request.QueryString["k"].Trim();
            }
            if (Request.QueryString["catid"] != null && !"".Equals(Request.QueryString["catid"]))
            {
                strNextPage = strNextPage + "&catid=" + Request.QueryString["catid"].Trim();
                strLastPage = strLastPage + "&catid=" + Request.QueryString["catid"].Trim();

                string catIds = Request.QueryString["catid"].Trim(',');
                if (!String.IsNullOrWhiteSpace(catIds))
                {
                    if (catIds.IndexOf(",") > 0)
                    {
                        string[] catIdsArray = catIds.Split(',');
                        catIds = "";
                        foreach (string catId in catIdsArray)
                        {
                            catIds += "'" + catId + "',";
                        }
                        catIds = catIds.Trim(',');
                    }
                    else
                    {
                        catIds = "'" + catIds + "'";
                    }
                    strWhere += " and CATID0 IN (" + catIds + ")";
                }
            }
            //取得有效行
            object rowno = DbHelperOra.GetSingle("SELECT MIN(HANGHAO) FROM ( SELECT ROWNUM HANGHAO FROM doc_goodspicture WHERE flag='Y' and nvl(str2,'N') = 'N' ORDER BY ROWNUM)");
            int pageNum = AspNetPager1.PageSize;
            string strSqlCount = @"select count(1) from 
                                    (select g.GDSEQ,g.GDNAME,g.GDSPEC,g.BZHL,g.UNIT,g.CATID0,g.PIZNO,p.picpath,ZJM from doc_goods g
                                    left join  (SELECT GDSEQ, MAX(PICPATH) PICPATH
                                       FROM DOC_GOODSPICTURE
                                      WHERE FLAG = 'Y'
                                        AND NVL(STR2, 'N') = 'N'
                                      GROUP BY GDSEQ) p on g.gdseq=p.gdseq and rownum = 1  where g.flag = 'Y') where {0}";
            strSqlCount = string.Format(strSqlCount, strWhere);
            AspNetPager1.RecordCount = Convert.ToInt32(DbHelperOra.GetSingle(strSqlCount));

            this.LiteralNum.Text = AspNetPager1.RecordCount.ToString();
            this.LiteralPageNum.Text = (Math.Ceiling(AspNetPager1.RecordCount / 10.0)).ToString();
            if (AspNetPager1.RecordCount >= 0 && AspNetPager1.RecordCount < 10)
            {
                strLastPage = "<a title='上一页' href='javascript:void(0);' class='sort_page_arrow'>&lt;</a>";
                strNextPage = "<a hreCeilingf='javascript:void(0);' title='下一页' class='sort_page_arrow'>&gt;</a>";
            }
            else
            {
                if (intPageIndex == 1)
                {
                    strLastPage = "<a title='上一页' href='javascript:void(0);' class='sort_page_arrow'>&lt;</a>";
                    strNextPage = "<a href='" + strNextPage + "' title='下一页' class='sort_page_arrow'>&gt;</a>";
                }
                else if (intPageIndex == (AspNetPager1.RecordCount / pageNum + 1))
                {
                    strLastPage = "<a title='上一页' href='" + strLastPage + "' class='sort_page_arrow'>&lt;</a>";
                    strNextPage = "<a href='javascript:void(0);' title='下一页' class='sort_page_arrow'>&gt;</a>";
                }
                else
                {
                    strLastPage = "<a title='上一页' href='" + strLastPage + "' class='sort_page_arrow'>&lt;</a>";
                    strNextPage = "<a href='" + strNextPage + "' title='下一页' class='sort_page_arrow'>&gt;</a>";
                }
            }

            this.LiteralLastPage.Text = strLastPage;
            this.LiteralNextPage.Text = strNextPage;
            string strSqlGood = @"select * from
                            (select t.*,ROWNUM rn from
                            (select GDSEQ,GDNAME,GDSPEC,BZHL,UNIT,UNITNAME,CATID0,PIZNO,HSJJ,producername,picpath,flag,isflag5 from 

                            (select g.GDSEQ,";

            //判断系统默认设置显示的商品名
            if(PubFunc.DbGetPara("ShowName") == "HIS")
            {
                strSqlGood += "NVL(g.HISNAME,g.GDNAME) GDNAME,NVL(g.STR3,g.GDSPEC) GDSPEC,";
            }
            else
            {
                strSqlGood += "g.GDNAME,g.GDSPEC,";
            }

            strSqlGood  += @"g.isflag5,g.BZHL,
                                    --g.UNIT,
                                    DECODE(g.UNIT_SELL, 'D', g.UNIT_DABZ, 'Z', g.UNIT_ZHONGBZ, g.UNIT) UNIT,
                                                                        --u.NAME UNITNAME,
                                    F_GETUNITNAME(DECODE(g.UNIT_SELL, 'D', g.UNIT_DABZ, 'Z', g.UNIT_ZHONGBZ, g.UNIT)) UNITNAME, 
                                    g.CATID0,
                                    g.PIZNO,
                                    --g.HSJJ,
                                    g.hsjj * (DECODE(g.UNIT_SELL, 'D', g.NUM_DABZ, 'Z', g.NUM_ZHONGBZ, g.BZHL)) as HSJJ,
                                    f_getproducername(g.producer) producername,
                                    p.picpath,ZJM,g.flag from doc_goods g
                                    left join (SELECT GDSEQ, MAX(PICPATH)PICPATH
                                      FROM DOC_GOODSPICTURE
                                     WHERE FLAG = 'Y'
                                       AND NVL(STR2, 'N') = 'N'
                                     GROUP BY GDSEQ) p
                            on g.gdseq = p.gdseq
                          left join doc_goodsunit u
                            on g.unit = u.code)
                            where {0}) t
 where t.flag = 'Y') where rn> " + (intPageIndex - 1) * pageNum + " and rn<= " + intPageIndex * pageNum;

            strSqlGood = string.Format(strSqlGood, strWhere);
            DataTable dt = DbHelperOra.Query(strSqlGood).Tables[0];

            DataTable dtnew = dt.Clone();
            foreach (DataRow row in dt.Rows)
            {
                string path = row["PICPATH"].ToString();
                if (!string.IsNullOrWhiteSpace(path))
                {
                    if (File.Exists(Request.PhysicalApplicationPath + (path.Substring(2)).Replace("/", @"\")))
                    {
                        row["PICPATH"] = "http://" + Request.Url.Authority + path.Substring(1);
                    }
                    else
                    {
                        row["PICPATH"] = ApiUtil.GetConfigCont("PIC_PATH") + "/captcha/GetPictures.aspx?picpath=" + path;
                    }
                }
                else
                {
                    row["PICPATH"] = "/res/images/noPic.jpg";
                }
                
                dtnew.Rows.Add(row.ItemArray);
            }
            RepeaterGoods.DataSource = dtnew;
            RepeaterGoods.DataBind();
        }
        protected void AspNetPager1_PageChanged(object sender, EventArgs e)
        {

        }
    }
}
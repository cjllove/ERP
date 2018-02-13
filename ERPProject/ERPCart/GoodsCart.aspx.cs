using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using XTBase;
using System.Data;
using FineUIPro;
using System.IO;

namespace ERPProject.ERPCart
{
    public partial class GoodsCart : PageBase
    {
        public string strPath = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            strPath = ApiUtil.GetConfigCont("PIC_PATH");
            if (Request.QueryString["oper"] != null && "del".Equals(Request.QueryString["oper"]))
            {
                if (Request.QueryString["seqno"] != null)
                {
                    string seqnos = Request.QueryString["seqno"].Trim(',');
                    string seqnosTemp = "";
                    if (seqnos.IndexOf(",") > 0)
                    {
                        string[] seqnosArray = seqnos.Split(',');
                        foreach (String seqno in seqnosArray)
                        {
                            seqnosTemp += "'" + seqno + "',";
                        }
                        seqnosTemp = seqnosTemp.Trim(',');
                    }
                    else
                    {
                        seqnosTemp += "'" + seqnos + "'";
                    }
                    del(seqnosTemp);
                }
            }
            if (!IsPostBack)
            {
                Path.Text = strPath;

                
            }
        }

        private void del(string seqno) {
            DbHelperOra.ExecuteSql(string.Format("delete from dat_cart where seqno in ({0}) and userid='{1}'", seqno, UserAction.UserID));
        }
        

        private void goodsDataBind()
        {
            string strSqlGoodSum = @"SELECT sum(c.dhs) dhs,sum((g.hsjj * (DECODE(g.UNIT_SELL, 'D', g.NUM_DABZ, 'Z', g.NUM_ZHONGBZ, g.BZHL)) * c.dhs )) as JE,COUNT(1) DPS
                                    FROM DAT_CART c
                                    left join doc_goods g on c.gdseq=g.gdseq
                                    where c.USERID='{0}'";
            DataTable dt = DbHelperOra.Query(string.Format(strSqlGoodSum, UserAction.UserID)).Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                this.LiteralDhs.Text = dt.Rows[0]["DPS"].ToString();
                this.LiteralJe.Text = dt.Rows[0]["JE"].ToString();
            }

            string strSqlGood = @"SELECT c.seqno,c.userid,c.gdseq,c.dhs,g.GDSEQ,";

            //判断系统默认设置显示的商品名
            if (PubFunc.DbGetPara("ShowName") == "HIS")
            {
                strSqlGood += "NVL(g.HISNAME,g.GDNAME) GDNAME,NVL(g.STR3,g.GDSPEC) GDSPEC,";
            }
            else
            {
                strSqlGood += "g.GDNAME,g.GDSPEC,";
            }

            strSqlGood += @"g.isflag5,g.BZHL,f_getunitname(g.UNIT) UNIT,g.CATID,g.PIZNO,c.deptid,f_getproducername(g.producer) producername,
                                    --g.hsjj as price,
                                    --(g.hsjj)*c.dhs as JE,
                                    p.picpath,
                                    g.hsjj * (DECODE(g.UNIT_SELL, 'D', g.NUM_DABZ, 'Z', g.NUM_ZHONGBZ, g.BZHL)) as price,
                                    (g.hsjj * (DECODE(g.UNIT_SELL, 'D', g.NUM_DABZ, 'Z', g.NUM_ZHONGBZ, g.BZHL)) * c.dhs ) JE,
                                                                        F_GETUNITNAME(DECODE(g.UNIT_SELL,
                                                                            'D',
                                                                            g.UNIT_DABZ,
                                                                            'Z',
                                                                            g.UNIT_ZHONGBZ,
                                                                            g.UNIT)) UNIT_SELL_NAME,
                                                                            DECODE(g.UNIT_SELL, 'D', g.NUM_DABZ, 'Z', g.NUM_ZHONGBZ, g.BZHL) BZHL_SELL
                                                                        FROM DAT_CART c
                                                                        left join doc_goods g on c.gdseq=g.gdseq
                                                                        left join (select  a.*,Row_Number()  OVER (partition by gdseq ORDER BY rownum  ) HANGHAO
                                        from doc_goodspicture a) p
                                    on c.gdseq=p.gdseq and p.HANGHAO=1 and p.flag='Y'and nvl(p.str2,'N') = 'N'
                                    where c.USERID='{0}'";
            strSqlGood = string.Format(strSqlGood, UserAction.UserID);
            DataTable dtInfo = DbHelperOra.Query(strSqlGood).Tables[0];
            DataTable dtnew = dtInfo.Clone();
            foreach (DataRow row in dtInfo.Rows)
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
        protected string GetXiaoji(object priceobj, object numberobj)
        {
            float price = Convert.ToSingle(priceobj);
            int number = Convert.ToInt32(numberobj);
            return String.Format("{0:F}", price * number);
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {
            if (e.EventArgument == "dataBind") {
                goodsDataBind();
            }
        }

        protected void RepeaterGoods_PreRender(object sender, EventArgs e)
        {
            goodsDataBind();
        }

        protected void test3_Click(object sender, EventArgs e)
        {
            //PageContext.RegisterStartupScript("console.log(111222333);");
            //do nothing
            //ClientScript.RegisterStartupScript(ClientScript.GetType(), "myscript ", "<script type=\"text/javascript\">console.log(21231223);</script> ");
            ScriptManager.RegisterStartupScript(this, this.GetType(),"123", "BindBox();", true);
        }


    }
}
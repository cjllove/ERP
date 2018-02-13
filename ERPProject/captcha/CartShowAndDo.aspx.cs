using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System.Web;

namespace ERPProject.captcha
{
    public partial class CartShowAndDo : BillBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String gdseq = "";
            decimal dhs = 0;
            String userId = "";
            String result = "";
            if (Request.QueryString["dhs"] != null && !"".Equals(Request.QueryString["dhs"]))
            {
                userId = UserAction.UserID;
                gdseq = Request.QueryString["gdseq"];
                try
                {
                    dhs = Convert.ToDecimal(Request.QueryString["dhs"]);
                }
                catch
                {
                    result = "获取参数时发生错误";
                }
                string operType = "add";
                if (Request.QueryString["oper"] != null && "update".Equals(Request.QueryString["oper"]))
                {
                    operType = "update";
                }


                result = addCart(gdseq, dhs.ToString(), userId, operType);
            }
            else if (Request.QueryString["gdseq"] != null && !"".Equals(Request.QueryString["gdseq"]))
            {
                gdseq = Request.QueryString["gdseq"];
                result = getGoodsData(gdseq);
            }
            else if (Request.QueryString["oper"] != null && "submit".Equals(Request.QueryString["oper"]))
            {
                result = submitCart();
            }
            else if (Request.QueryString["oper"] != null && "sum".Equals(Request.QueryString["oper"]))
            {
                result = sumCart();
            }
            else
            {
                result = "获取参数时发生错误";
            }

            Response.ContentType = "text/plain";
            Response.Write(result);
            Response.End();
        }

        private string getGoodsData(string gdseq)
        {
            DataTable picTable = DbHelperOra.Query(String.Format("select rowno,picpath from doc_goodspicture where gdseq='{0}' and nvl(str2,'N') = 'N' order by rowno", gdseq)).Tables[0];
            string strGoodsSql = @"select GDSEQ,GDNAME,GDSPEC,BZHL,UNIT,u.name UNITNAME,CATID,c.name CATNAME,PIZNO,BAR3 EASCODE,SUPPLIER SUPID,s.supname SUPNAME,PRODUCER,p.supname PRODUCERNAME,JXTAX,HSJJ+2 JJ from doc_goods g
                                left join doc_goodsunit u on g.unit=u.code
                                left join sys_category c on g.catid=c.code
                                left join doc_supplier s on g.supplier=s.supid
                                left join doc_supplier p on g.PRODUCER=p.str2
                                where gdseq='{0}'";
            DataTable goodsInfo = DbHelperOra.Query(String.Format(strGoodsSql, gdseq)).Tables[0];
            DataTable cartTable = DbHelperOra.Query(String.Format("select dhs from dat_cart where userid='{0}' and gdseq='{1}'", UserAction.UserID, gdseq)).Tables[0];
            JObject jo = new JObject();
            JArray ja = new JArray();
            jo.Add("gdseq", goodsInfo.Rows[0]["GDSEQ"].ToString());
            jo.Add("gdname", goodsInfo.Rows[0]["GDNAME"].ToString());
            jo.Add("gdspec", goodsInfo.Rows[0]["GDSPEC"].ToString());
            jo.Add("bzhl", goodsInfo.Rows[0]["BZHL"].ToString());
            jo.Add("unit", goodsInfo.Rows[0]["UNIT"].ToString());
            jo.Add("unitname", goodsInfo.Rows[0]["UNITNAME"].ToString());
            jo.Add("catid", goodsInfo.Rows[0]["CATID"].ToString());
            jo.Add("catname", goodsInfo.Rows[0]["CATNAME"].ToString());
            jo.Add("eascode", goodsInfo.Rows[0]["EASCODE"].ToString());
            jo.Add("pizno", goodsInfo.Rows[0]["PIZNO"].ToString());
            jo.Add("supid", goodsInfo.Rows[0]["SUPID"].ToString());
            jo.Add("supname", goodsInfo.Rows[0]["SUPNAME"].ToString());
            jo.Add("producer", goodsInfo.Rows[0]["PRODUCER"].ToString());
            jo.Add("producername", goodsInfo.Rows[0]["PRODUCERNAME"].ToString());
            jo.Add("jxtax", goodsInfo.Rows[0]["JXTAX"].ToString());
            jo.Add("jj", goodsInfo.Rows[0]["JJ"].ToString());
            foreach (DataRow dr in picTable.Rows)
            {
                JObject joPath = new JObject();
                joPath.Add("rowno", Convert.ToInt16(dr["ROWNO"]));
                if (dr["PICPATH"] != null && !"".Equals(dr["PICPATH"].ToString()))
                {
                    string strPICPATH = ApiUtil.GetConfigCont("CLOUD_URL_PREFIX") + dr["PICPATH"].ToString().Substring(2);
                    joPath.Add("path", strPICPATH);
                }
                ja.Add(joPath);
            }
            jo.Add("picpath", ja);
            if (cartTable != null && cartTable.Rows.Count > 0)
            {
                jo.Add("dhs", cartTable.Rows[0]["dhs"].ToString());
            }

            return JsonConvert.SerializeObject(jo);
        }

        private string addCart(string gdseq, string dhs, string userId, string operType)
        {
            JObject jo = new JObject();
            Object obj;
            if (HttpContext.Current.Request.Cookies["DEPTOUT"] == null)
            {
                obj = DbHelperOra.GetSingle("select nvl((SELECT A.STOCK FROM SYS_DEPT A WHERE A.CODE = '" + UserAction.UserDept + "'),(select value from sys_para where code = 'DEFDEPT')) from dual");
            }
            else
            {
                obj = HttpContext.Current.Request.Cookies["DEPTOUT"].Value;
            }
            int result = 0;
            string sql = "select count(1) from  dat_cart where userid='{0}' and gdseq='{1}' and deptid = '{2}'";
            sql = string.Format(sql, userId, gdseq, obj.ToString());
            result = Convert.ToInt32(DbHelperOra.GetSingle(sql));
            if (result > 0)
            {
                if ("update".Equals(operType))
                {
                    sql = "update dat_cart set dhs={2} where userid='{0}' and gdseq='{1}' and deptid = '{3}'";
                }
                else
                {
                    sql = "update dat_cart set dhs=dhs+{2} where userid='{0}' and gdseq='{1}' and deptid = '{3}'";
                }
            }
            else
            {
                sql = "insert into dat_cart (seqno,userid,gdseq,dhs,deptid) values (seq_cart.nextval,'{0}','{1}',{2},'{3}')";
            }

            sql = string.Format(sql, userId, gdseq, dhs, obj.ToString());
            result = DbHelperOra.ExecuteSql(sql);
            if (result > 0)
            {
                jo.Add("result", "success");
            }
            else
            {
                jo.Add("result", "false");
            }
            return JsonConvert.SerializeObject(jo);
        }

        private string submitCart()
        {
            string seqnosTemp = "";
            if (Request.QueryString["seqno"] != null)
            {
                string seqnos = Request.QueryString["seqno"].Trim(',');
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
            }

            JObject jo = new JObject();
            try
            {
                string seqno = BillSeqGet("DHD");
                string custid = "10009";
                OracleParameter[] param = { new OracleParameter("VS_USERID", OracleDbType.Varchar2, 20),
                                          new OracleParameter("VS_SEQNO", OracleDbType.Varchar2, 20),
                                          new OracleParameter("VS_CUSTID", OracleDbType.Varchar2, 20),
                                          new OracleParameter("VS_CARTSEQNOS", OracleDbType.Varchar2, 2000)};
                param[0].Direction = ParameterDirection.Input;
                param[0].Value = UserAction.UserID;
                param[1].Direction = ParameterDirection.Input;
                param[1].Value = seqno;
                param[2].Direction = ParameterDirection.Input;
                param[2].Value = custid;
                param[3].Direction = ParameterDirection.Input;
                param[3].Value = seqnosTemp;

                int result = DbHelperOra.RunProcedure("INFTEMP.P_SAVECARTTODD", param);

                jo.Add("result", "success");
                jo.Add("seqno", seqno);
                jo.Add("custid", custid);
            }
            catch (Exception ex)
            {
                jo.Add("result", "false");
            }
            return JsonConvert.SerializeObject(jo);
        }

        private string sumCart()
        {
            string seqnosTemp = "";
            if (Request.QueryString["seqno"] != null)
            {
                string seqnos = Request.QueryString["seqno"].Trim(',');
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
            }

            JObject jo = new JObject();
            try
            {
                string strSqlGoodSum = "";
                DataTable dt = new DataTable();
                if (seqnosTemp != "")
                {
                    strSqlGoodSum = @"SELECT nvl(sum(c.dhs),0) dhs,nvl(sum((g.hsjj+2)*c.dhs),0) as JE
                                    FROM DAT_CART c
                                    left join doc_goods g on c.gdseq=g.gdseq
                                    where c.USERID='{0}' and c.seqno in ({1})";
                    dt = DbHelperOra.Query(string.Format(strSqlGoodSum, UserAction.UserID, seqnosTemp)).Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        jo.Add("result", "success");
                        jo.Add("dhs", dt.Rows[0]["dhs"].ToString());
                        jo.Add("je", dt.Rows[0]["JE"].ToString());
                    }
                }
                else {
                    strSqlGoodSum= @"SELECT nvl(count(c.gdseq),0) dhs
                                    FROM DAT_CART c
                                    where c.USERID='{0}'";
                    dt = DbHelperOra.Query(string.Format(strSqlGoodSum, UserAction.UserID)).Tables[0];
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        jo.Add("result", "success");
                        jo.Add("dhs", dt.Rows[0]["dhs"].ToString());
                    }
                }
               
            }
            catch (Exception ex)
            {
                jo.Add("result", "false");
                jo.Add("dhs", "0");
                jo.Add("je", "0");
            }
            return JsonConvert.SerializeObject(jo);
        }
    }
}
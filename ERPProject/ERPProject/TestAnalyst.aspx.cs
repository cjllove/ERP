using FineUIPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPProject
{
    public partial class TestAnalyst : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                DataSearch();
            }
        }

        private void DataInit()
        {
            PubFunc.DdlDataGet(ddlXMBH, "DDL_PROJECT");
            PubFunc.DdlDataGet("DDL_USER", ddlCREUSER, ddlDOUSER);
            PubFunc.DdlDataGet(ddlQTYPE, "DDL_QUESTION_TYPE");
            ddlXMBH.SelectedIndex = 0;

            dpkCRETIME1.SelectedDate = DateTime.Now.AddDays(-15);
            dpkCRETIME2.SelectedDate = DateTime.Now;
        }

        private void DataSearch(bool IsTotal = false)
        {
            string sql = @"SELECT TO_CHAR(CRETIME,'MM-DD') DAYS, SUM(DECODE(STATUS, '5', '1', '4', '1', '0')) RESOLVED, SUM(1) TOTAL
                             FROM PRO_QUESTION
                            WHERE 1=1 ";
            if (ddlXMBH.SelectedValue != null && PubFunc.StrIsEmpty(ddlXMBH.SelectedValue))
            {
                sql += string.Format(" AND XMBH='{0}'", ddlXMBH.SelectedValue);
            }
            if (dpkCRETIME1.SelectedDate != null && dpkCRETIME2.SelectedDate != null)
            {
                sql += string.Format(" AND CRETIME BETWEEN TO_DATE('{0}','yyyy-MM-dd HH:mi:ss') AND TO_DATE('{1}','yyyy-MM-dd HH:mi:ss')", string.Format("{0:yyyy-MM-dd}", dpkCRETIME1.SelectedDate), string.Format("{0:yyyy-MM-dd}", dpkCRETIME2.SelectedDate));
            }
            else
            {
                sql += " AND CRETIME BETWEEN SYSDATE - 15 AND SYSDATE";
            }
            if (dpkDOTIME1.SelectedDate != null && dpkDOTIME2.SelectedDate != null)
            {
                sql += string.Format(" AND DOTIME BETWEEN TO_DATE('{0}','yyyy-MM-dd HH:mi:ss') AND TO_DATE('{1}','yyyy-MM-dd HH:mi:ss')", string.Format("{0:yyyy-MM-dd}", dpkDOTIME1.SelectedDate), string.Format("{0:yyyy-MM-dd}", dpkDOTIME2.SelectedDate));
            }
            if (ddlCREUSER.SelectedValue != null && !PubFunc.StrIsEmpty(ddlCREUSER.SelectedValue))
            {
                sql += string.Format(" AND CREUSER='{0}'", ddlCREUSER.SelectedValue);
            }
            if (ddlDOUSER.SelectedValue != null && !PubFunc.StrIsEmpty(ddlDOUSER.SelectedValue))
            {
                sql += string.Format(" AND DOUSER='{0}'", ddlDOUSER.SelectedValue);
            }
            sql += " GROUP BY CRETIME ORDER BY CRETIME";
            DataTable dtData = DbHelperOra.Query(sql).Tables[0];

            if (dtData != null && dtData.Rows.Count > 0)
            {
                string strLabels = "", strResolved = "", strTotal = "";
                int Resolved = 0, Total = 0;
                foreach (DataRow row in dtData.Rows)
                {
                    strLabels += "'" + row["DAYS"].ToString() + "',";
                    if (IsTotal)
                    {
                        if (Total == 0)
                        {
                            string date = "SYSDATE-15";
                            if (dpkCRETIME2.SelectedDate != null)
                            {
                                date = string.Format("TO_DATE('{0}','yyyy-MM-dd')", string.Format("{0:yyyy-MM-dd}", dpkCRETIME2.SelectedDate));
                            }
                            DataTable dtOld = DbHelperOra.Query(string.Format("SELECT SUM(DECODE(STATUS, '5', '1', '4', '1', '0')) RESOLVED, SUM(1) TOTAL FROM PRO_QUESTION WHERE CRETIME < {0}", date)).Tables[0];
                            Resolved = int.Parse(dtOld.Rows[0]["RESOLVED"].ToString());
                            Total = int.Parse(dtOld.Rows[0]["TOTAL"].ToString());
                        }
                        Resolved += int.Parse(row["RESOLVED"].ToString());
                        Total += int.Parse(row["TOTAL"].ToString());
                        strResolved += "'" + Resolved.ToString() + "',";
                        strTotal += "'" + Total.ToString() + "',";
                    }
                    else
                    {
                        strResolved += "'" + row["RESOLVED"].ToString() + "',";
                        strTotal += "'" + row["TOTAL"].ToString() + "',";
                    }
                }

                DataRow[] rows = dtData.Select("", "TOTAL DESC");

                int end = 100, scale_space = 10;
                //if (total > 100)
                //{
                //    end = (int)Math.Ceiling(Convert.ToDouble(total / 10)) * 10;
                //}
                scale_space = (int)Math.Ceiling(Convert.ToDouble(Total / 10));
                end = scale_space * 10;

                var data = new
                {
                    render = IsTotal ? "canvasDivTotal" : "canvasDiv",
                    title = string.Format("{0}测试BUG分析", ddlXMBH.SelectedText),
                    subtitle = IsTotal ? "累计BUG测试情况(BUG单位：个)" : "每天BUG测试情况(BUG单位：个)",
                    footnote = string.Format("测试项目：{0}", ddlXMBH.SelectedText),
                    end_scale = end,
                    scale_space = scale_space,
                    lbl = (JArray)JsonConvert.DeserializeObject("[" + strLabels.Substring(0, strLabels.Length - 1) + "]"),
                    dataSource = new List<object>() { new { name = "测试BUG数",color="#0d8ecf",line_width=2, value = (JArray)JsonConvert.DeserializeObject("["+strTotal.Substring(0, strTotal.Length - 1)+"]") }, 
                        new { name = "解决BUG数", color="#ef7707",line_width=2, value = (JArray)JsonConvert.DeserializeObject("["+strResolved.Substring(0, strResolved.Length - 1)+"]") } }
                };

                //var dataChar = new
                //{
                //    title = string.Format("{0}测试BUG分析", ddlXMBH.SelectedText),
                //    subtitle = "每天BUG测试情况(BUG单位：个)",
                //    footnote = string.Format("测试项目：{0}", ddlXMBH.SelectedText),
                //    end_scale = end,
                //    scale_space = scale_space,
                //    labels = (JArray)JsonConvert.DeserializeObject("[" + strLabels.Substring(0, strLabels.Length - 1) + "]"),
                //    dataSource = new List<object>() { new { name = "测试BUG数",color="#0d8ecf",value = (JArray)JsonConvert.DeserializeObject("["+strTotal.Substring(0, strTotal.Length - 1)+"]") }, 
                //        new { name = "解决BUG数", color="#ef7707",value = (JArray)JsonConvert.DeserializeObject("["+strResolved.Substring(0, strResolved.Length - 1)+"]") } }
                //};

                PageContext.RegisterStartupScript(string.Format("DrawLin({0});", JsonConvert.SerializeObject(data)));
            }
        }

        protected void btSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }

        private DataTable ChartData()
        {
            string sql = @"SELECT *
                            FROM (SELECT F_GETUSERNAME(NVL(DOUSER, 'system')) STAFF,
                                        SUM(1) TOTAL,
                                        0 FLG
                                    FROM PRO_QUESTION
                                    WHERE 1 = 1
                                    GROUP BY DOUSER
                                UNION
                                SELECT F_GETUSERNAME(NVL(CREUSER, 'system')) STAFF, SUM(1) TOTAL, 1 FLG
                                    FROM PRO_QUESTION
                                    WHERE 1 = 1
                                    GROUP BY CREUSER)
                            ORDER BY FLG";
            DataTable dtData = DbHelperOra.Query(sql).Tables[0];
            return dtData;
        }

        public Color GetRandomColor()
        {
            Random randomNum_1 = new Random(Guid.NewGuid().GetHashCode());
            System.Threading.Thread.Sleep(randomNum_1.Next(9));
            int int_Red = randomNum_1.Next(255);

            Random randomNum_2 = new Random((int)DateTime.Now.Ticks);
            int int_Green = randomNum_2.Next(255);

            Random randomNum_3 = new Random(Guid.NewGuid().GetHashCode());

            int int_Blue = randomNum_3.Next(255);
            int_Blue = (int_Red + int_Green > 380) ? int_Red + int_Green - 380 : int_Blue;
            int_Blue = (int_Blue > 255) ? 255 : int_Blue;


            return GetDarkerColor(System.Drawing.Color.FromArgb(int_Red, int_Green, int_Blue));
        }

        //获取加深颜色
        public Color GetDarkerColor(Color color)
        {
            const int max = 255;
            int increase = new Random(Guid.NewGuid().GetHashCode()).Next(30, 255); //还可以根据需要调整此处的值


            int r = Math.Abs(Math.Min(color.R - increase, max));
            int g = Math.Abs(Math.Min(color.G - increase, max));
            int b = Math.Abs(Math.Min(color.B - increase, max));


            return Color.FromArgb(r, g, b);
        }

        protected void TabStrip1_TabIndexChanged(object sender, EventArgs e)
        {
            if (TabStrip1.ActiveTabIndex == 1 && string.IsNullOrWhiteSpace(hfdSecond.Text))
            {
                hfdSecond.Text = "已加载";
                DataSearch(true);
            }
            if (TabStrip1.ActiveTabIndex == 2 && string.IsNullOrWhiteSpace(hfdThird.Text))
            {
                hfdThird.Text = "已加载";
                DataTable dtData = ChartData();
                if (dtData != null && dtData.Rows.Count > 0)
                {
                    List<object> list1 = new List<object>();
                    List<object> list2 = new List<object>();
                    foreach (DataRow row in dtData.Rows)
                    {
                        if (row["FLG"].ToString() == "1")
                            list1.Add(new { name = row["STAFF"].ToString(), value = int.Parse(row["TOTAL"].ToString()), color = ColorTranslator.ToHtml(GetRandomColor()) });
                        else if (row["FLG"].ToString() == "0")
                            list2.Add(new { name = row["STAFF"].ToString(), value = int.Parse(row["TOTAL"].ToString()), color = ColorTranslator.ToHtml(GetRandomColor()) });
                    }

                    var dataChar1 = new
                    {
                        render = "canvasPie1",
                        title = string.Format("{0}测试BUG分析", ddlXMBH.SelectedText),
                        text = "BUG数",
                        footnote = string.Format("测试项目：{0}", ddlXMBH.SelectedText),
                        dataSource = list1
                    };
                    var dataChar2 = new
                    {
                        render = "canvasPie2",
                        title = string.Format("{0}解决BUG分析", ddlXMBH.SelectedText),
                        text = "解决数",
                        footnote = string.Format("测试项目：{0}", ddlXMBH.SelectedText),
                        dataSource = list2
                    };
                    PageContext.RegisterStartupScript(string.Format("DrawPie({0});DrawPie({1});", JsonConvert.SerializeObject(dataChar1), JsonConvert.SerializeObject(dataChar2)));
                }
            }
            else if (TabStrip1.ActiveTabIndex == 3 && string.IsNullOrWhiteSpace(hfdFourth.Text))
            {
                hfdFourth.Text = "已加载";
                DataTable dtData = ChartData();
                if (dtData != null && dtData.Rows.Count > 0)
                {
                    List<object> list = new List<object>();
                    foreach (DataRow row in dtData.Rows)
                    {
                        list.Add(new { name = row["STAFF"].ToString(), value = int.Parse(row["TOTAL"].ToString()), color = ColorTranslator.ToHtml(GetRandomColor()) });
                    }
                    DataRow[] rows = dtData.Select("", "TOTAL DESC");
                    int total = int.Parse(rows[0]["TOTAL"].ToString());
                    int end = 100, scale_space = 10;
                    scale_space = (int)Math.Ceiling(Convert.ToDouble(total / 10));
                    end = scale_space * 10;

                    var dataChar = new
                    {
                        title = string.Format("{0}测试BUG分析", ddlXMBH.SelectedText),
                        subtitle = "每天BUG测试情况(BUG单位：个)",
                        footnote = string.Format("测试项目：{0}", ddlXMBH.SelectedText),
                        end_scale = end,
                        scale_space = scale_space,
                        dataSource = list
                    };
                    PageContext.RegisterStartupScript(string.Format("DrawChar({0});", JsonConvert.SerializeObject(dataChar)));
                }
            }
        }
    }
}
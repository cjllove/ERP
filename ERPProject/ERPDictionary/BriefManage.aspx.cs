using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;


namespace ERPProject.ERPDictionary
{
    public partial class BriefManage : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                DataQuery();
            }
        }

        private void DataInit()
        {
            ddlUsers.DataSource = DbHelperOra.QueryForTable("SELECT USERID,USERNAME FROM SYS_OPERUSER WHERE ISDELETE='N' ORDER BY USERID ASC");
            ddlUsers.DataTextField = "USERNAME";
            ddlUsers.DataValueField = "USERID";
            ddlUsers.DataBind();
            hfdIsNew.Text = "Y";
        }

        private void DataQuery()
        {
            String sql = @"select DBD.CODE BRCODE,
                                       DBD.NAME BRNAME,
                                       DBD.FLAG,
                                       DECODE(DBD.FLAG, 'Y', '启用', 'N', '不启用','未维护') FLAGNAME,
                                     F_JOINUSERID(DBD.CODE) USERID,
                                     TO_CHAR(TIMEUP,'YYYY-MM-DD HH24:MI:SS')TIMEUP,
                                     MEMO,STR1,STR2,STR3                              
                          from DOC_BRIEF_DOC DBD
                         where  1=1
                           ORDER BY DBD.CODE";
            int total = 0;
            if (!PubFunc.StrIsEmpty(tgbSearch.Text))
            {
                sql += string.Format(" AND (DOTYPE like '%{0}%' or DONAME like '%{0}%')", tgbSearch.Text);
            }
            DataTable dt = PubFunc.DbGetPage(GridToBrief.PageIndex, GridToBrief.PageSize, sql, ref total);

                GridToBrief.DataSource = dt;
            GridToBrief.RecordCount = total;
            GridToBrief.DataBind();
        }

        protected void GridToBrief_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            hfdIsNew.Text = "N";
            string strcode = GridToBrief.Rows[e.RowIndex].Values[0].ToString();
            DataTable dt = DbHelperOra.Query("select * from DOC_BRIEF_DOC where code='" + strcode + "'").Tables[0];
           PubFunc.FormDataSet(FormBR, dt.Rows[0]);
           PubFunc.FormDataSet(FormCON,dt.Rows[0]);
           labSHOW.Text = "";
        }

        protected void GridToBrief_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridToBrief.PageIndex = e.NewPageIndex;
            DataQuery();
        }

        protected void tgbSearch_TriggerClick(object sender, EventArgs e)
        {
            DataQuery();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            hfdIsNew.Text = "Y";
             ddlFLAG.SelectedValue = "N";         
            tbxMEMO.Text = "";
            PubFunc.FormDataClear(FormBR);
            PubFunc.FormDataClear(FormCON);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (GridToBrief.SelectedRowIndexArray.Length > 0)
            {
                string ID = "";
                for (int i = 0; i < GridToBrief.SelectedRowIndexArray.Length; i++)
                {
                    ID += GridToBrief.Rows[GridToBrief.SelectedRowIndexArray[i]].Values[0].ToString() + ",";
                }
                DbHelperOra.ExecuteSql("delete from DOC_BRIEF_DOC where CODE in ('" + ID.TrimEnd(',').Replace(",", "','") + "')");
                DataQuery();
            }
            else
            {
                Alert.Show("请选择要删除的信息！", MessageBoxIcon.Warning);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

            if (tbxCODE.Text.Trim() == "" || tbxCODE.Text.Trim() == null)
            {
                Alert.Show("简报编码不能为空！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            if (tbxNAME.Text.Trim() == "" || tbxNAME.Text.Trim() == null)
            {
                Alert.Show("简报名称不能为空！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (trbESQL.Text.Trim() == "" || trbESQL.Text.Trim() == null)
            {
                Alert.Show("简报SQL不能为空！","消息提示",MessageBoxIcon.Warning);
                return;
            }
            if (trbBRCONTENT.Text.Trim() == "" || trbBRCONTENT.Text.Trim()==null)
            {
                Alert.Show("简报内容不能为空！","消息提示",MessageBoxIcon.Warning);
                return;
            }
                CheckSQL();            
                string strcode = tbxCODE.Text.ToString().Trim();

            //int num = 0;
            //switch (lstTIMEOA.SelectedValue)
            //{
            //    case "DAY":
            //        num = 1;
            //        break;
            //    case "MONTH":
            //        num = 30;
            //        break;
            //    case "YEAR":
            //        num = 365;
            //        break;
            //    default:
            //        num = 0;
            //        break;


            //}
            //Regex regex = new Regex(@"[-][ ]?[0-9]\d*");
            //trbESQL.Text = regex.Replace(trbESQL.Text.ToString(), "-" + num.ToString());
            string strAFTER = trbESQL.Text.ToString().Replace("'", "''");
            //if (strAFTER.IndexOf("/r") < 0 || strAFTER.IndexOf("/n") < 0)
            //{
            //    strAFTER = strAFTER.Replace("/r", " ");
            //    strAFTER = strAFTER.Replace("/n", " ");
            //}
            List<CommandInfo> cmdlist = new List<CommandInfo>();
            cmdlist.Add(new CommandInfo(string.Format("DELETE FROM DOC_BRIEF_DOC WHERE CODE ='{0}'",strcode), null));
            cmdlist.Add(new CommandInfo(string.Format("INSERT INTO DOC_BRIEF_DOC(CODE,NAME,FLAG,TIMEOA,ESQL,ISTIME,BRCONTENT,MEMO,STR1,STR2,STR3)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", strcode, tbxNAME.Text, ddlFLAG.SelectedValue, "", strAFTER, ddlISTIME.SelectedValue, trbBRCONTENT.Text, tbxMEMO.Text.Trim(), lstSTR1.SelectedValue, tbxSTR2.Text.Trim(), lstSTR1.SelectedText), null));
            //DataTable dtuser = DbHelperOra.QueryForTable(string.Format("SELECT USERID FROM DOC_BRIEF_COM WHERE CODE='{0}'",strcode));
            //if (dtuser.Rows.Count > 0)
            //{
            //    string sresult=getSendMes(strcode);
            //    foreach(DataRow dr in dtuser.Rows)
            //    {
            //     cmdlist.Add(new CommandInfo(string.Format("UPDATE DOC_BRIEF_COM SET SENDMES='{0}' WHERE CODE='{1}' AND USERID='{2}'",sresult,strcode,dr[0].ToString()),null));
            //    }
            //}

             DbHelperOra.ExecuteSqlTran(cmdlist);           
            Alert.Show("简报保存成功！");
            DataQuery();
            
        }

        protected void btnTest_Click(object sender, EventArgs e)
        {
           DataTable dtget = DbHelperOra.QueryForTable(string.Format("SELECT ESQL,BRCONTENT FROM DOC_BRIEF_DOC WHERE  CODE='{0}'",tbxCODE.Text));
            string strResult = "";

            if (dtget.Rows.Count>0)
            {
                DataTable dtSQL = DbHelperOra.QueryForTable(dtget.Rows[0][0].ToString());
                Regex regex = new Regex(@"[:：][1-9]\d*");
                strResult = dtget.Rows[0][1].ToString();
                MatchCollection mac = regex.Matches(strResult);
                for (int i = 0; i < mac.Count;i++ )
                {
                    strResult = strResult.Replace(mac[i].ToString(),dtSQL.Rows[0][i].ToString());
                }
                
            }
            labSHOW.Text = "【预览结果】:"+strResult+"";
        }
        protected void CheckSQL()
        {
            try
            {
                DbHelperOra.ExecuteSql(trbESQL.Text.Trim());
            }
            catch (Exception ex)
            {
                Alert.Show(Error_Parse(ex.Message));
                return ;
            }
            DataTable dt = DbHelperOra.QueryForTable(trbESQL.Text);
            Regex regex = new Regex(@"[:：][1-9]\d*");
            if (!regex.Matches(trbBRCONTENT.Text).Count.ToString().Equals(dt.Columns.Count.ToString()))
            {
                Alert.Show("SQL数量不匹配");
                return ;
            }
        }
        public static string Error_Parse(string error)
        {
            string value = string.Empty;
            if (error.IndexOf("ORA-") > -1)
            {
                value = error.Replace("\n", "").Substring(error.IndexOf("ORA-") + 10);
                if (value.IndexOf("ORA-") > -1)
                {
                    value = value.Substring(0, value.IndexOf("ORA-"));
                }
            }
            else
            {
                value = error;
            }

            return value;
        }

        protected string getSendMes(string strcode)
        {
            DataTable dtget = DbHelperOra.QueryForTable(string.Format("SELECT ESQL,BRCONTENT FROM DOC_BRIEF_DOC WHERE  CODE='{0}'", strcode));
            string strResult = "";

            if (dtget.Rows.Count > 0)
            {
                DataTable dtSQL = DbHelperOra.QueryForTable(dtget.Rows[0][0].ToString());
                Regex regex = new Regex(@"[:：][1-9]\d*");
                strResult = dtget.Rows[0][1].ToString();
                MatchCollection mac = regex.Matches(strResult);
                for (int i = 0; i < mac.Count; i++)
                {
                    strResult = strResult.Replace(mac[i].ToString(), dtSQL.Rows[0][i].ToString());
                }

            }
            return strResult;
        }

        private void dataSearch(string type = "left")
        {
            int total = 0;

            if (type == "left")
            {
                String Sql = @"SELECT * FROM DOC_BRIEF_DOC
                                WHERE FLAG='Y'";

                if (tgbSearch.Text.Trim().Length > 0)
                    Sql += string.Format(" AND （CODE LIKE '%{0}%' OR NAME LIKE '%{0}%' OR STR3 LIKE '%{0}%'）", tgbSearch.Text.Trim());
                if (ddlUsers.SelectedValue != null && ddlUsers.SelectedValue.Length > 0)
                {
                    Sql += string.Format(" AND CODE NOT IN (SELECT CODE FROM DOC_BRIEF_COM WHERE USERID= '{0}')", ddlUsers.SelectedValue);
                }

                Sql += "  ORDER BY STR1,STR2";
                DataTable dtData = PubFunc.DbGetPage(GridALLBrief.PageIndex, GridALLBrief.PageSize, Sql, ref total);
                GridALLBrief.RecordCount = total;
                GridALLBrief.DataSource = dtData;
                GridALLBrief.DataBind();
            }
            else if (type == "right")
            {
                String Sql = @"SELECT C.CODE,D.NAME,D.STR3,C.STR2,C.USERID FROM DOC_BRIEF_COM C,DOC_BRIEF_DOC D
                                WHERE C.CODE = D.CODE";

                if (ddlUsers.SelectedValue != null && ddlUsers.SelectedValue.Length > 0)
                {
                    Sql += string.Format(" AND C.USERID = '{0}'", ddlUsers.SelectedValue);
                }
                Sql += "  ORDER BY STR2";
                DataTable dtData = PubFunc.DbGetPage(GridCFGBrief.PageIndex, GridCFGBrief.PageSize, Sql, ref total);
                GridCFGBrief.RecordCount = total;
                GridCFGBrief.DataSource = dtData;
                GridCFGBrief.DataBind();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }

        protected void GridCFGBrief_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            string CODE = GridCFGBrief.DataKeys[e.RowIndex][0].ToString();
            string USERID = GridCFGBrief.DataKeys[e.RowIndex][1].ToString();

            string strSql = string.Format("SELECT CODE,USERID,STR2 FROM DOC_BRIEF_COM WHERE CODE='{0}' AND USERID='{1}'",CODE,USERID);
            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            PubFunc.FormDataSet(FormConfig, dt.Rows[0]);
        }

        protected void ddlUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataSearch();
            dataSearch("right");
            tbsCODE.Text = "";
            tbsNAME.Text = "";
            tbsSTR2.Text = "";
        }
        protected void GridALLBrief_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridALLBrief.PageIndex = e.NewPageIndex;
            dataSearch();
        }

        protected void GridCFGBrief_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridCFGBrief.PageIndex = e.NewPageIndex;
            dataSearch("right");
        }

        protected void btnAddRight_Click(object sender, EventArgs e)
        {
            if (ddlUsers.SelectedValue.Length > 0)
            {
                int[] selectArray = GridALLBrief.SelectedRowIndexArray;
                if (selectArray.Length > 0)
                {
                    List<CommandInfo> cmdList = new List<CommandInfo>();
                    for (int i = 0; i < selectArray.Length; i++)
                    {
                        string CODE = GridALLBrief.Rows[selectArray[i]].Values[1].ToString();
                        string STR1 = GridALLBrief.Rows[selectArray[i]].Values[4].ToString();
                        string STR2 = GridALLBrief.Rows[selectArray[i]].Values[5].ToString();

                        if (DbHelperOra.Exists(string.Format(@"SELECT 1 FROM DOC_BRIEF_COM WHERE CODE='{0}' AND USERID='{1}'", CODE, ddlUsers.SelectedValue)))
                        {
                            Alert.Show("简报【" + CODE + "】已被配置此用户", "消息提示", MessageBoxIcon.Warning);
                            return;
                        }

                        string Sql = @"INSERT INTO DOC_BRIEF_COM
                                        (CODE,USERID,STR2)
                                        VALUES
                                        ('{0}','{1}','{2}')";
                        Sql = string.Format(Sql,CODE, ddlUsers.SelectedValue,STR1+STR2);
                        cmdList.Add(new CommandInfo(Sql, null));
                    }
                    DbHelperOra.ExecuteSqlTran(cmdList);
                    dataSearch("left");
                    dataSearch("right");
                }
                else
                {
                    Alert.Show("请选择要进行配置的简报信息！");
                }
            }
            else
            {
                Alert.Show("请选择要进行配置的用户！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
        }

        protected void btnAddLeft_Click(object sender, EventArgs e)
        {
            int[] selectArray = GridCFGBrief.SelectedRowIndexArray;
            string err = "";
            if (selectArray.Length > 0)
            {
                List<CommandInfo> cmdList = new List<CommandInfo>();
                for (int i = 0; i < selectArray.Length; i++)
                {
                    string CODE = GridCFGBrief.DataKeys[selectArray[i]][0].ToString();
                    cmdList.Add(new CommandInfo("DELETE DOC_BRIEF_COM WHERE CODE='" + CODE + "' AND USERID = '" + ddlUsers.SelectedValue + "'", null));
                }
                DbHelperOra.ExecuteSqlTran(cmdList);

                Alert.Show("简报配置取消成功！");

                tbsCODE.Text = "";
                tbsNAME.Text = "";
                tbsSTR2.Text = "";
                dataSearch("left");
                dataSearch("right");
            }
        }

        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ddlUsers.SelectedValue))
            {
                Alert.Show("请先选择用户!", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            dataSearch("right");
        }
        //protected void lstTIMEOA_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    int num = 0;
        //    switch (lstTIMEOA.Text)
        //    {
        //        case "DAY":
        //            num = 1;
        //            break;
        //        case "MONTH":
        //            num = 30;
        //            break;
        //        case "YEAR":
        //            num = 365;
        //            break;
        //        default:
        //            num = 0;
        //            break;


        //    }
        //    string strAFTER = trbESQL.Text.ToString().Trim();
        //      Regex regex = new Regex(@"[-][ ]?[1-9]\d*");
        //         trbESQL.Text= regex.Replace(strAFTER,"-"+num.ToString().Replace("'",""));
        //}
    }
}
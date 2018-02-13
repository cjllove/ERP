using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using FineUIPro;
using System.Collections.Specialized;
using System.Text;
using System.Collections;
using XTBase;
using Oracle.ManagedDataAccess.Client;
using System.IO;

namespace ERPProject.ERPDictionary
{
    public partial class PictureDistribute : PageBase
    {
        private static DataTable dtData;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private void dataSearch()
        {
            //string strSql = string.Format("select p.gdseq,p.rowno,decode(p.flag,'N','新增','Y','已发布','E','已撤回') flag,p.gdpict,g.gdname,g.gdspec,g.cdid,g.pizno from doc_goods g,doc_goodspicture p where p.gdseq = g.gdseq and (p.gdseq ='{0}' or g.barcode like '%{0}%' or g.ZJM like '%{0}%' or g.gdname like '%{0}%')", trbSearch.Text);
            //DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            //gridPic.DataSource = dt;
            //gridPic.DataBind();

            int total = 0;
            string msg = "";
            NameValueCollection nvc = new NameValueCollection();
            if (trbSearch.Text.Length > 0) nvc.Add("CX", trbSearch.Text);

            dtData = GetGoodsList(gridPic.PageIndex, gridPic.PageSize, nvc, ref total, ref msg);
            gridPic.RecordCount = total;
            gridPic.DataSource = dtData;
            gridPic.DataBind();
        }

        public DataTable GetGoodsList(int pageNum, int pageSize, NameValueCollection nvc, ref int total, ref string errMsg)
        {
            string strSearch = "";
            if (nvc != null)
            {
                foreach (string key in nvc)
                {
                    string condition = nvc[key];
                    if (!string.IsNullOrEmpty(condition))
                    {
                        switch (key.ToUpper())
                        {
                            case "CX":
                                strSearch += string.Format(" and (p.gdseq like '%{0}%' or g.barcode like '%{0}%' or g.ZJM like '%{0}%' or g.gdname like '%{0}%')", condition.ToUpper());
                                break;
                        }
                    }
                }
            }
            string strSql = @"select p.gdseq,p.rowno,decode(p.flag,'N','新增','Y','已发布','E','已撤回') flag,p.gdpict,p.str1,g.gdname,g.ZJM,g.gdspec,F_GETPRODUCERNAME(g.PRODUCER) PRODUCERNAME,g.pizno from doc_goods g,doc_goodspicture p where p.gdseq = g.gdseq ";
            string strwhere = "";
            if (cbxNonPic.Checked && !cbxNonPic1.Checked)
            {
                strwhere = " and p.flag='Y' ";
            }
            else if (!cbxNonPic.Checked && cbxNonPic1.Checked)
            {
                strwhere = " and p.flag='E' ";
            }
            else if (cbxNonPic.Checked && cbxNonPic1.Checked)
            {
                strwhere = " and p.flag in('E','Y') ";
                }
            else
            {
                strwhere = " and p.flag in('E','Y','N') ";
            }
            
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql = strSql + strSearch;
            }
            else
            {
                strSql = strSql + "";
            }
            strSql += strwhere;
            return PubFunc.DbGetPage(pageNum, pageSize, strSql, ref total);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }

        //图片批量发布和撤销操作
        private void picDistribute(string flag, string msg)
        {
            int[] selections = gridPic.SelectedRowIndexArray;
            bool isflag = true;
            if (selections.Length == 0)
            {
                Alert.Show("请选择要操作的单据！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            List<CommandInfo> cmdList = new List<CommandInfo>();
            for (int i = 0; i < selections.Length; i++)
            {
                int rowNum = gridPic.SelectedRowIndexArray[i];
                if (gridPic.DataKeys[rowNum][2].ToString().Equals("新增") && msg.Equals("图片撤回"))
                {
                    isflag = false;
                }
                else
                {
                    cmdList.Add(new CommandInfo("update doc_goodspicture set flag='" + flag + "' where rowno='" + gridPic.DataKeys[rowNum][1].ToString() + "' and gdseq='" + gridPic.DataKeys[rowNum][0].ToString() + "'", null));
                }

            }
            if (isflag)
            {
                if (DbHelperOra.ExecuteSqlTran(cmdList))
                {
                    Alert.Show("" + msg + "成功！");
                    dataSearch();
                }
                else
                {
                    Alert.Show("" + msg + "失败！");
                    dataSearch();
                }
            }
            else
            {
                Alert.Show("您不能选择【新增】状态的商品图片，执行撤回操作！");
            }


        }
        private int ComRow()
        {
            return gridPic.SelectedRowIndexArray.Length > 0 ? gridPic.SelectedRowIndexArray[0] : -1;
        }

        //图片发布
        protected void btnDistribute_Click(object sender, EventArgs e)
        {
            picDistribute("Y", "图片发布");
        }

        //图片撤回
        protected void btnBack_Click(object sender, EventArgs e)
        {
            picDistribute("E", "图片撤回");
        }

        protected void gridPic_PageIndexChange(object sender, GridPageEventArgs e)
        {
            gridPic.PageIndex = e.NewPageIndex;
            dataSearch();
        }

        protected void gridPic_RowClick(object sender, GridRowClickEventArgs e)
        {
            string gdseq = gridPic.Rows[e.RowIndex].DataKeys[0].ToString();
            string rowno = gridPic.Rows[e.RowIndex].DataKeys[1].ToString();
            DataTable picDt = DbHelperOra.Query(string.Format("select picpath from doc_goodspicture where gdseq='{0}' and rowno='{1}'", gdseq, rowno)).Tables[0];
            imgBMPPATH.ImageUrl = ApiUtil.GetConfigCont("CLOUD_URL_PREFIX") + picDt.Rows[0][0].ToString();
            lnkImage.NavigateUrl = ApiUtil.GetConfigCont("CLOUD_URL_PREFIX") + picDt.Rows[0][0].ToString();
        }
    }
}
using FineUIPro;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ERPProject.Certificate
{
    public partial class GoodsCertificate : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }
        }

        private void DataInit()
        {
            TabStrip1.ActiveTabIndex = 0;
        }

        private void billOpen(string strSeq)
        {
            TabStrip1.ActiveTabIndex = 1;
            String prefix = ApiClientUtil.GetConfigCont("LICENSE_PATH");
            gridPic.DataSource = getPicData(strSeq);
            gridPic.DataBind();
            imgBMPPATH.ImageUrl = ApiUtil.GetConfigCont("LICENSE_PATH") + "/captcha/CertificatePicture.aspx?seqno=" + strSeq + "&rowno=1";
            //DataTable mydt = gridPic_path1(strSeq);
            //if (mydt.Rows.Count > 0)
            //{
            //    imgBMPPATH.ImageUrl = prefix + mydt.Rows[0]["PICPATH"].ToString().Replace("~", "");
            //}
            //else
            //{
            //    Alert.Show("不要频繁点击，稍后片刻！");
            //}
        }

        private DataTable gridPic_path1(string lsno)
        {
            DataTable picpathDT = new DataTable();
            try
            {
                String[] paramSup1 = new String[1] { lsno };
                JObject result = ApiClientService.query("LICENSE_TPICTURE", paramSup1);
                if ("success".Equals(result.Value<String>("result")))
                {
                    JArray ja = result.Value<JArray>("data");
                    String serJa = JsonConvert.SerializeObject(ja);
                    picpathDT = JsonConvert.DeserializeObject<DataTable>(serJa);
                }
                else
                {
                    String reason = result.Value<String>("reason");
                    Exception ex = new Exception(reason);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Alert.Show(ex.Message, "获取图片路径时报错", MessageBoxIcon.Error);
            }
            return picpathDT;
        }

        private DataTable getPicData(string lsno)
        {
            DataTable picdt = new DataTable();
            try
            {
                String[] paramSup1 = new String[1] { lsno };
                JObject result = ApiClientService.query("LICENSE_PIC", paramSup1);
                if ("success".Equals(result.Value<String>("result")))
                {
                    JArray ja = result.Value<JArray>("data");
                    String serJa = JsonConvert.SerializeObject(ja);
                    picdt = JsonConvert.DeserializeObject<DataTable>(serJa);
                }
                else
                {
                    String reason = result.Value<String>("reason");
                    Exception ex = new Exception(reason);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Alert.Show(ex.Message, "获取检验报告信息时报错", MessageBoxIcon.Error);
            }
            return picdt;
        }

        private DataTable getDGSupplier()
        {
            DataTable dt = new DataTable();
            try
            {
                //String[] paramSup = new String[7] { tbxSEQNO.Text, lstLICENSEID.Text, llstFLAG.SelectedValue, lstLICENSETYPE.SelectedValue, "", lstLRRQ1.SelectedDate.ToString(), lstLRRQ2.SelectedDate.ToString() };
                //String[] paramSup1 = new String[2] { docSearch.Text, docFLAG.SelectedValue };
                JObject result = ApiClientService.query("SUP_LICENSE_GOODSDOC", docFLAG.SelectedValue, docSearch.Text);
                if ("success".Equals(result.Value<String>("result")))
                {
                    JArray ja = result.Value<JArray>("data");
                    String serJa = JsonConvert.SerializeObject(ja);
                    dt = JsonConvert.DeserializeObject<DataTable>(serJa);
                }
                else
                {
                    String reason = result.Value<String>("reason");
                    Exception ex = new Exception(reason);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Alert.Show(ex.Message, "获取供应商信息时报错", MessageBoxIcon.Error);
            }
            return dt;
        }

        private DataTable getDGSupplier1()
        {
            DataTable dt = new DataTable();
            try
            {
                //String[] paramSup = new String[7] { tbxSEQNO.Text, lstLICENSEID.Text, llstFLAG.SelectedValue, lstLICENSETYPE.SelectedValue, "", lstLRRQ1.SelectedDate.ToString(), lstLRRQ2.SelectedDate.ToString() };
                //String[] paramSup1 = new String[2] { docSearch.Text, docFLAG.SelectedValue };
                JObject result = ApiClientService.query("SUP_LICENSE_GOODSD", docSearch.Text);
                if ("success".Equals(result.Value<String>("result")))
                {
                    JArray ja = result.Value<JArray>("data");
                    String serJa = JsonConvert.SerializeObject(ja);
                    dt = JsonConvert.DeserializeObject<DataTable>(serJa);
                }
                else
                {
                    String reason = result.Value<String>("reason");
                    Exception ex = new Exception(reason);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Alert.Show(ex.Message, "获取供应商信息时报错", MessageBoxIcon.Error);
            }
            return dt;
        }

        protected void GridList_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            billOpen(GridList.Rows[e.RowIndex].DataKeys[0].ToString());
        }

        protected void btnSearch_Click(object sender,EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(docFLAG.SelectedValue))
            {
                billSearch1();
            }
            else
            {
                billSearch();
            }
        }

        private DataTable gridPic_path(string seqno, string rowno)
        {
            DataTable picpathDT = new DataTable();
            try
            {
                String[] paramSup1 = new String[2] { seqno, rowno };
                JObject result = ApiClientService.query("LICENSE_PICTURE", paramSup1);
                if ("success".Equals(result.Value<String>("result")))
                {
                    JArray ja = result.Value<JArray>("data");
                    String serJa = JsonConvert.SerializeObject(ja);
                    picpathDT = JsonConvert.DeserializeObject<DataTable>(serJa);
                }
                else
                {
                    String reason = result.Value<String>("reason");
                    Exception ex = new Exception(reason);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Alert.Show(ex.Message, "获取图片路径时报错", MessageBoxIcon.Error);
            }
            return picpathDT;
        }
        protected void gridPic_RowClick(object sender, GridRowClickEventArgs e)
        {
            TabStrip1.ActiveTabIndex = 1;
            String prefix = ApiClientUtil.GetConfigCont("LICENSE_PATH");
            string seqno = gridPic.Rows[e.RowIndex].DataKeys[0].ToString();
            string rowno = gridPic.Rows[e.RowIndex].DataKeys[1].ToString();
            imgBMPPATH.ImageUrl = ApiUtil.GetConfigCont("LICENSE_PATH") + "/captcha/CertificatePicture.aspx?seqno=" + seqno + "&rowno=" + rowno;
            //DataTable dt = gridPic_path(seqno, rowno);
            //if (dt.Rows.Count > 0)
            //{
            //    imgBMPPATH.ImageUrl = prefix + dt.Rows[0]["PICPATH"].ToString().Replace("~", "");
            //}
            //else
            //{
            //    Alert.Show("不要频繁点击，稍后片刻！");
            //}
        }
        private void billSearch()
        {
            GridList.DataSource = getDGSupplier();
            GridList.DataBind();
        }

        private void billSearch1()
        {
            GridList.DataSource = getDGSupplier1();
            GridList.DataBind();
        }

        protected void GridList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridList.PageIndex = e.NewPageIndex;
            billSearch1();
        }

    }
}
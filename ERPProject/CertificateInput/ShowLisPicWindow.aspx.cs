using System;
using FineUIPro;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using XTBase;

namespace ERPProject.CertificateInput
{
    public partial class ShowLisPicWindow : PageBase
    {
        private static int picindex = 0;
        private static string picnum = "";
        private static string myseqno = "";
        private static string lisid = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                if (Request.QueryString["bm"] != null && Request.QueryString["bm"].ToString() != "")
                {
                    seqnohfd.Text = Request.QueryString["bm"].ToString();
                    myseqno = Request.QueryString["bm"].ToString();
                }
                if (Request.QueryString["xc"] != null && Request.QueryString["xc"].ToString() != "")
                {
                    hfdpicnum.Text = Request.QueryString["xc"].ToString();
                    picnum = Request.QueryString["xc"].ToString();
                }
                if (Request.QueryString["cc"] != null && Request.QueryString["cc"].ToString() != "")
                {
                    hfdlisid.Text = Request.QueryString["cc"].ToString();
                    lisid = Request.QueryString["cc"].ToString();
                }
                Init();
            }
        }

        private void Init()
        {
            //SEQNO, ROWNO, ISCUR, LICENSEID证照图片表主键
            string gdseq = seqnohfd.Text;
            DataTable picDt = DbHelperOra.Query(string.Format("select imgpath,rowno from DOC_LICENSE_IMG where seqno='{0}' and rownum=1", gdseq)).Tables[0];
            string picpath = picDt.Rows[0]["imgpath"].ToString();
            string rowno = picDt.Rows[0]["rowno"].ToString();
            picpath = picpath.Substring(1, picpath.Length - 1);
            //imglbl.Text = seqnohfd.Text + "/" + rowno + picpath;
            imgBMPPATH.ImageUrl = ApiUtil.GetConfigCont("LIS_PICURL") + picpath;
            picindex = 1;
            btnLeft.Enabled = false;
            if (Convert.ToInt32(picnum) == 1)
            {
                btnRight.Enabled = false;
            }
        }

        protected void prePageBtn_Click(object sender, EventArgs e)
        {
            if(picindex==1)
            {
                //Alert.Show("已经是第一张证照图片！");
                btnLeft.Enabled = false;
            }
            else
            {
                btnRight.Enabled=true;
                picindex--;
                string seqno = myseqno;
                DataTable picDt = DbHelperOra.Query(string.Format("select imgpath,rowno from DOC_LICENSE_IMG where seqno='{0}' and licenseid='{1}' and rowno={2}", seqno, lisid, picindex)).Tables[0];
                string picpath = picDt.Rows[0]["imgpath"].ToString();
                string rowno = picDt.Rows[0]["rowno"].ToString();
                picpath = picpath.Substring(1, picpath.Length - 1);
                //imglbl.Text = seqno + "/" + rowno + picpath;
                imgBMPPATH.ImageUrl = ApiUtil.GetConfigCont("LIS_PICURL") + picpath;
                if (picindex == 1)
                {
                    btnLeft.Enabled = false;
                }
            }
        }

        protected void nextPageBtn_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(picnum) > 1)
            {
                picindex++;
                if (picindex > Convert.ToInt32(picnum))
                {
                    Alert.Show("证照图片到底了！");
                    picindex--;
                }
                else
                {
                    btnLeft.Enabled = true;
                    string seqno = myseqno;
                    DataTable picDt = DbHelperOra.Query(string.Format("select imgpath,rowno from DOC_LICENSE_IMG where seqno='{0}' and licenseid='{1}' and rowno={2}", seqno, lisid, picindex)).Tables[0];
                    string picpath = picDt.Rows[0]["imgpath"].ToString();
                    string rowno = picDt.Rows[0]["rowno"].ToString();
                    picpath = picpath.Substring(1, picpath.Length - 1);
                    //imglbl.Text = seqno + "/" + rowno + picpath;
                    imgBMPPATH.ImageUrl = ApiUtil.GetConfigCont("LIS_PICURL") + picpath;

                    if (picindex == Convert.ToInt32(picnum))
                    {
                        btnRight.Enabled = false;
                    }
                }

            }
            if(Convert.ToInt32(picnum) == 1)
            {
                Alert.Show("证照图片到底了！");
            }

        }

        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {

        }


    }
}
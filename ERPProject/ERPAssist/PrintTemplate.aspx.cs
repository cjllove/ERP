using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using XTFramework;
using System.Data;
using System.Collections.Specialized;
using System.Text;
namespace ERPProject.ERPAssist
{
    public partial class PrintTemplate : PageBase
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
            //string HeaderString = PubFunc.DbGetPara("SUPPER");
            //tbxHeader.Text = HeaderString + "院院内物流管理系统配送条码回收簿";
            PubFunc.DdlDataGet("DDL_SYS_DEPTDEF", lstDEPTID);
            lstDEPTID.SelectedIndex = 0;
            numCoding.Text = "10";
            NumberBox1.Text = "1";

            lblCPUID.Text = SerialNumber.GetCpuId();
        }
        protected void lstDEPTID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDEPTID.SelectedValue.Length == 0)
            {
                Alert.Show("请选择科室！");
                return;    
            }
            if(lstISGZ.SelectedItem != null && lstISGZ.SelectedItem.Value=="Y")
            { 
            Object obj = DbHelperOra.GetSingle("SELECT NUM2 FROM SYS_DEPT A WHERE  A.CODE='" + lstDEPTID.SelectedValue + "'");
            int initNum = Convert.ToInt32(obj ?? "0");
            NumberBox1.Text = initNum == 1?"1" :( initNum+1).ToString();
            numCoding.Text = initNum == 1 ? "10" : (initNum + 10).ToString();
            }
            if (lstISGZ.SelectedItem != null && lstISGZ.SelectedItem.Value == "N")
            {
                Object obj = DbHelperOra.GetSingle("SELECT NUM3 FROM SYS_DEPT A WHERE  A.CODE='" + lstDEPTID.SelectedValue + "'");
                int initNum = Convert.ToInt32(obj ?? "0");
                NumberBox1.Text = (initNum + 1).ToString();
                numCoding.Text = (initNum + 10).ToString();
            }
        }
        protected void btnBarcode_click(object sender, EventArgs e)
        {
            if (lstISGZ.SelectedItem != null && lstISGZ.SelectedItem.Value == "Y")
            {
                Object obj = DbHelperOra.GetSingle("SELECT NUM2 FROM SYS_DEPT A WHERE  A.CODE='" + lstDEPTID.SelectedValue + "'");
                int initNum = Convert.ToInt32(obj ?? "0");
                NumberBox1.Text =(initNum + 1).ToString();
                numCoding.Text = (initNum + 10).ToString();
            }
            if (lstISGZ.SelectedItem != null && lstISGZ.SelectedItem.Value == "N")
            {
                Object obj = DbHelperOra.GetSingle("SELECT NUM3 FROM SYS_DEPT A WHERE  A.CODE='" + lstDEPTID.SelectedValue + "'");
                int initNum = Convert.ToInt32(obj ?? "0");
                NumberBox1.Text = (initNum + 1).ToString();
                numCoding.Text = (initNum + 10).ToString();
            }
        }
        protected void lstISGZ_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDEPTID.SelectedValue.Length == 0)
            {
                numCoding.Text = "10";
                NumberBox1.Text = "1";
            }
            if (lstDEPTID.SelectedItem != null)
            {
                if (lstISGZ.SelectedItem != null && lstISGZ.SelectedItem.Value == "Y")
                {
                    Object obj = DbHelperOra.GetSingle("SELECT NUM2 FROM SYS_DEPT A WHERE  A.CODE='" + lstDEPTID.SelectedValue + "'");
                    int initNum = Convert.ToInt32(obj ?? "0");
                    NumberBox1.Text = initNum == 1 ? "1" : (initNum + 1).ToString();
                    numCoding.Text = initNum == 1 ? "10" : (initNum + 10).ToString();
                }
                if (lstISGZ.SelectedItem != null && lstISGZ.SelectedItem.Value == "N")
                {
                    Object obj = DbHelperOra.GetSingle("SELECT NUM3 FROM SYS_DEPT A WHERE  A.CODE='" + lstDEPTID.SelectedValue + "'");
                    int initNum = Convert.ToInt32(obj ?? "0");
                    NumberBox1.Text =(initNum + 1).ToString();
                    numCoding.Text = (initNum + 10).ToString();
                }
            }

        }
    }
        
}
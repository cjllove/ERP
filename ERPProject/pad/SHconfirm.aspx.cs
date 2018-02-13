﻿using FineUIPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.pad
{
    public partial class SHconfirm : PageBase
    {
        public JObject MyUser;
        public SHconfirm()
        {
            MyUser = new JObject();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MyUser = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(UserAction));
                bindData();
            }
        }
        private void bindData()
        {
            PubFunc.DdlDataGet("DDL_SYS_DEPOTRANGE", UserAction.UserID, docDEPTID, lstDEPTID);
            PubFunc.DdlDataGet("DDL_USERALL", lstLRY, docCGY, docSHR, docLRY);
            PubFunc.DdlDataGet("DDL_BILL_STATUSDHD", lstFLAG, docFLAG);
            PubFunc.DdlDataGet("DDL_DOC_SHS", lstSUPID, docPSSID);
            docDEPTID.SelectedValue = UserAction.UserDept;
        }
        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {

        }

    }
}
using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Specialized;
using System.Collections;

namespace ERPProject.ERPStorage
{
    public partial class LibraryMaintain : BillBase
    {
        private string strComSql = "select t.*,f_getunitname(t.unit) unitname, decode(t.isgz,'N','否','Y','是','未定义') isgzname,decode(t.islot,'0','不进行','1','只有入库','2','全部','未定义') ISLOT, f_getproducername(producer) producername,t.str1 hwid from DAT_YH_COM t where 1=1";
        private string strDocSql = @"select t.*,decode(t.flag,'N','已提交','Y','已审核','M','新单','未定义') flagname, f_getdeptname(t.deptid) DEPTNAME,
       f_getusername(t.YHY) YHYNAME,
       f_getusername(t.LRY) LRYNAME,
       f_getusername(t.SPR) SPRNAME,
       f_getusername(t.SHR) SHRNAME from DAT_YH_DOC t where 1=1";
        public LibraryMaintain()
        {
            BillType = "YHD";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                billNew();
            }
        }
        private void DataInit()
        {
            PubFunc.DdlDataGet("DDL_SYS_DEPOT", lstDEPTID, docDEPTID, ddlDEPTID);
            string strSql = @"SELECT CODE,NAME FROM (SELECT '' CODE ,'--请选择--' NAME  FROM dual
                                            union all
                                            SELECT 'M' CODE ,'新单' NAME  FROM dual
                                            union all
                                            SELECT 'N' CODE ,'已提交' NAME  FROM dual
                                            union all
                                            SELECT 'Y' CODE ,'已审核' NAME  FROM dual)";
            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            docFLAG.DataTextField = "NAME";
            docFLAG.DataValueField = "CODE";
            docFLAG.DataSource = dt;
            docFLAG.DataBind();
            lstFLAG.DataTextField = "NAME";
            lstFLAG.DataValueField = "CODE";
            lstFLAG.DataSource = dt;
            lstFLAG.DataBind();

            string strSql1 = @"SELECT CODE,NAME FROM (
                                            SELECT '0' CODE ,'正常' NAME  FROM dual
                                            union all
                                            SELECT '1' CODE ,'破损' NAME  FROM dual
                                            union all
                                            SELECT '2' CODE ,'失效' NAME  FROM dual
                                            union all
                                            SELECT '3' CODE ,'发霉' NAME  FROM dual)";
            PubFunc.DdlDataSql(comYHTYPE, strSql1);
            docDEPTID.SelectedValue = "01";
            PubFunc.DdlDataGet("DDL_USER", ddlLRY, ddlSHR, ddlYHY, lstYHY);
            dpkDATE1.SelectedDate = DateTime.Now.AddDays(-1);
            dpkDATE2.SelectedDate = DateTime.Now;
            billNew();
        }

        private void dataSearch()
        {
            int total = 0;
            string msg = "";
            NameValueCollection nvc = new NameValueCollection();
            if (string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
            {
                Alert.Show("请选择库房进行查询操作", "警告提醒", MessageBoxIcon.Warning);
                return;
            }
            if (tbxGDSEQ.Text.Length > 0) nvc.Add("GDSEQ", tbxGDSEQ.Text);
            if (tbxPRODUCER.Text.Length > 0) nvc.Add("PRODUCER", tbxPRODUCER.Text);
            if (docDEPTID.SelectedValue.Length > 0) nvc.Add("DEPTID", docDEPTID.SelectedValue);
            if (tbxHWID.Text.Length > 0) nvc.Add("HWID", tbxHWID.Text);

            DataTable dt = DbHelperOra.Query(GetSql(nvc)).Tables[0];
            GridLeft.DataSource = dt;
            GridLeft.DataBind();
            docDEPTID.Enabled = false;
            if (dt.Rows.Count > 0)
            {
                btnCreate.Enabled = true;
            }
            else
            {
                btnCreate.Enabled = false;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(fmForm);
            GridLeft.DataSource = null;
            GridLeft.DataBind();
            docDEPTID.Enabled = true;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(docDEPTID.SelectedValue))
            {
                Alert.Show("没有选择【库房】，不能生成养护单。");
                return;
            }
            if (GridLeft.SelectedRowIndexArray.Length < 1)
            {
                Alert.Show("请选择需要生成的单据信息！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            string strSeqNo = BillSeqGet();
            List<CommandInfo> cmdList = new List<CommandInfo>();
            MyTable mtType = new MyTable("DAT_YH_DOC");
            mtType.ColRow["SEQNO"] = strSeqNo;
            mtType.ColRow["BILLNO"] = strSeqNo;
            mtType.ColRow["BILLTYPE"] = BillType.ToString();
            mtType.ColRow["FLAG"] = "M";
            mtType.ColRow["DEPTID"] = docDEPTID.SelectedValue;
            mtType.ColRow["YHRQ"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            mtType.ColRow["SUBNUM"] = GridLeft.SelectedRowIndexArray.Length;
            mtType.ColRow["YHY"] = UserAction.UserID;
            mtType.ColRow["LRY"] = UserAction.UserID;
            mtType.ColRow["LRRQ"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            MyTable mtTypeMx = new MyTable("DAT_YH_COM");
            //先删除单据信息在插入
            int i = 0;
            decimal decTotal = 0;
            foreach (int index in GridLeft.SelectedRowIndexArray)
            {
                Hashtable row = PubFunc.GridDataGet(GridLeft.Rows[index]);
                mtTypeMx.ColRow["SEQNO"] = strSeqNo;
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow["GDSEQ"] = row["GDSEQ"];
                mtTypeMx.ColRow["GDID"] = row["GDID"];
                mtTypeMx.ColRow["GDNAME"] = row["GDNAME"];
                mtTypeMx.ColRow["GDSPEC"] = row["GDSPEC"];
                mtTypeMx.ColRow["UNIT"] = row["UNIT"];
                mtTypeMx.ColRow["ISGZ"] = row["ISGZ"];
                mtTypeMx.ColRow["ISLOT"] = row["ISLOT"];
                mtTypeMx.ColRow["HSJJ"] = row["HSJJ"];
                mtTypeMx.ColRow["KCSL"] = row["KCSL"];
                mtTypeMx.ColRow["HSJE"] = row["HSJE"];
                mtTypeMx.ColRow["STR1"] = row["HWID"];
                mtTypeMx.ColRow["PHID"] = row["PHID"];
                mtTypeMx.ColRow["YXQZ"] = row["YXQZ"];
                mtTypeMx.ColRow["RQ_SC"] = row["RQ_SC"];
                mtTypeMx.ColRow["HWID"] = row["HWID"];
                mtTypeMx.ColRow["GDMODE"] = row["GDMODE"];
                mtTypeMx.ColRow["BZHL"] = row["BZHL"];
                mtTypeMx.ColRow["PIZNO"] = row["PIZNO"];
                mtTypeMx.ColRow["PRODUCER"] = row["PRODUCER"];
                mtTypeMx.ColRow["ZPBH"] = row["ZPBH"];
                mtTypeMx.ColRow["MEMO"] = row["MEMO"];
                mtTypeMx.ColRow["PICINO"] = row["PICINO"];

                decTotal += decimal.Parse(row["HSJE"].ToString());
                i++;
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow["SUBSUM"] = decTotal;
            cmdList.Add(mtType.Insert());

            if (DbHelperOra.ExecuteSqlTran(cmdList))
            {
                //PubFunc.FormDataClear(fmForm);
                Alert.Show("商品养护信息保存成功,已生成养护单【" + strSeqNo + "】！", "消息提示", MessageBoxIcon.Information);
                OperLog("在库养护", "生成单据【" + strSeqNo + "】");
            }
            //docDEPTID.Enabled = true;
            TabStrip1.ActiveTabIndex = 1;
            btnSear_Click(null, null);
        }

        protected override void billOpen(string strBillno)
        {
            //表头进行赋值
            DataTable dtDoc = DbHelperOra.Query(strDocSql + string.Format(" AND SEQNO='{0}'", strBillno)).Tables[0];
            PubFunc.FormDataSet(Formlis, dtDoc.Rows[0]);
            PubFunc.FormLock(Formlis, true);
            string strFlag = DbHelperOra.GetSingle(string.Format(@"SELECT flag FROM DAT_YH_DOC WHERE SEQNO = '{0}'", strBillno)).ToString();
            hfdOper.Text = strFlag;
            if (strFlag == "M")
            {
                tbxMEMO.Enabled = false;
                btnDelRow.Enabled = true;
                btnDel.Enabled = true;
                btnSave.Enabled = false;
                btnTJ.Enabled = true;
                btnAudit.Enabled = false;
                btnPrint.Enabled = false;
            }
            else if (strFlag == "N")
            {
                tbxMEMO.Enabled = true;
                btnDelRow.Enabled = false;
                btnDel.Enabled = false;
                btnSave.Enabled = true;
                btnTJ.Enabled = false;
                btnAudit.Enabled = true;
                btnPrint.Enabled = true;
            }
            else
            {
                tbxMEMO.Enabled = false;
                btnDelRow.Enabled = false;
                btnDel.Enabled = false;
                btnSave.Enabled = false;
                btnTJ.Enabled = false;
                btnAudit.Enabled = false;
                btnPrint.Enabled = false;
            }
            //表体赋值
            PageContext.RegisterStartupScript(GridLis.GetRejectChangesReference());
            decimal bzslTotal = 0, feeTotal = 0;
            string sql = strComSql + string.Format(" AND SEQNO='{0}'", strBillno);
            DataTable dtBill = DbHelperOra.Query(sql).Tables[0];
            if (dtBill != null && dtBill.Rows.Count > 0)
            {
                foreach (DataRow row in dtBill.Rows)
                {
                    bzslTotal += Convert.ToDecimal(row["KCSL"] ?? "0");
                    feeTotal += Convert.ToDecimal(row["HSJE"] ?? "0");
                }
                Doc.GridRowAdd(GridLis, dtBill);
            }
            //计算合计数量
            JObject summary = new JObject();
            summary.Add("GDNAME", "本页合计");
            summary.Add("KCSL", bzslTotal.ToString());
            summary.Add("HSJE", feeTotal.ToString("F2"));
            GridLis.SummaryData = summary;

            TabStrip1.ActiveTabIndex = 2;


        }
        protected void btnClearField_Click(object sender, EventArgs e)
        {
            lstSEQNO.Text = "";
            lstFLAG.SelectedValue = "";
            dpkDATE1.SelectedDate = DateTime.Now.AddDays(-1);
            dpkDATE2.SelectedDate = DateTime.Now;
            lstYHY.SelectedValue = "";
            lstDEPTID.SelectedValue = "";
            //GridDoc.DataBind();
        }

        protected override void billNew()
        {
            PubFunc.FormDataClear(Formlis);
            ddlYHY.SelectedValue = UserAction.UserID;
            ddlLRY.SelectedValue = UserAction.UserID;
            dpkYHRQ.SelectedDate = DateTime.Now;
            dpkLRRQ.SelectedDate = DateTime.Now;

            PubFunc.FormLock(Formlis, false);
            ddlLRY.Enabled = false;
            dpkLRRQ.Enabled = false;
            ddlSHR.Enabled = false;
            dpkSHRQ.Enabled = false;
            docFLAG.Enabled = false;

            GridLis.SummaryData = null;
            PageContext.RegisterStartupScript(GridLis.GetRejectChangesReference());
        }

        protected override void billClear()
        {
            PubFunc.FormDataClear(Formlis);
            GridDoc.DataSource = null;
            GridDoc.DataBind();
        }

        protected override void billDel()
        {
            if (string.IsNullOrWhiteSpace(docBILLNO.Text))
            {
                Alert.Show("单据为空，无法执行【删除操作】");
                return;
            }
            string strFLAG = docFLAG.SelectedValue;
            if (strFLAG != "M")
            {
                Alert.Show("单据号【" + docBILLNO.Text + "】状态不正确，无法执行【删除】操作。");
                return;
            }
            DbHelperOra.Exists(string.Format("delete from dat_yh_doc where seqno = '{0}'", docBILLNO.Text));
            DbHelperOra.Exists(string.Format("delete from dat_yh_com where seqno = '{0}'", docBILLNO.Text));
            Alert.Show("单据号【" + docBILLNO.Text + "】删除成功");
            OperLog("在库养护", "删除单据【" + docBILLNO.Text + "】");
            //btnSear_Click(null, null);
            PubFunc.FormDataClear(Formlis);

            GridLis.DataSource = null;
            GridLis.DataBind();

            btnSear_Click(null, null);
        }

        protected override void billDelRow()
        {
            if (string.IsNullOrWhiteSpace(docBILLNO.Text))
            {
                Alert.Show("空单据，无法进行【删行】操作");
                return;
            }
            if (docFLAG.SelectedValue != "M")
            {
                Alert.Show("非新单，不能进行【删行】操作！");
                return;
            }
            if (GridLis.SelectedCell == null) return;
            int rowIndex = GridLis.SelectedRowIndex;
            if (rowIndex < 0) return;

            PageContext.RegisterStartupScript(Confirm.GetShowReference("确认要对第【" + (rowIndex + 1) + "】行进行【删行】的操作吗？", "警告提示",
                MessageBoxIcon.Information, PageManager1.GetCustomEventReference("billDelRow_Ok"),
                PageManager1.GetCustomEventReference("billDelRow_Cancel"))); //,true

            //20150510 liuz 
            List<Dictionary<string, object>> newDict = GridLis.GetNewAddedList();
            newDict.RemoveAt(rowIndex);
        }
        private void billDelRow_mes()
        {
            int SelectedIndex = GridLis.SelectedRowIndex;
            PageContext.RegisterStartupScript(GridLis.GetDeleteRowReference(SelectedIndex));
        }

        protected override void billSave()
        {
            if (DataSave())
            {
                PubFunc.FormDataClear(fmForm);
                docDEPTID.Enabled = true;

                Alert.Show("商品养护信息保存成功,单据号【" + docBILLNO.Text + "】", "消息提示", MessageBoxIcon.Information);
                OperLog("在库养护", "修改单据【" + docBILLNO.Text + "】");
            }
        }
        protected override void billAudit()
        {
            if (string.IsNullOrWhiteSpace(docBILLNO.Text))
            {
                Alert.Show("空单据，无法进行【审核】操作");
                return;
            }
            if (DataSave("Y"))
            {
                Alert.Show("商品养护信息审核成功！", "消息提示", MessageBoxIcon.Information);
                billOpen(docBILLNO.Text);
                btnSear_Click(null, null);
                OperLog("在库养护", "审核单据【" + docBILLNO.Text + "】");
            }
        }

        protected override void billExport()
        {
            //string sql = strComSql + string.Format(" AND SEQNO='{0}'", docSEQNO.Text);
            //DataTable dtData = DbHelperOra.Query(sql).Tables[0];
            //if (dtData == null || dtData.Rows.Count == 0)
            //{
            //    Alert.Show("没有数据,无法导出！");
            //    return;
            //}
            //string[] columnNames = new string[GridLis.Columns.Count - 1];
            //for (int index = 1; index < GridLis.Columns.Count; index++)
            //{
            //    GridColumn column = GridLis.Columns[index];
            //    if (column is FineUIPro.BoundField)
            //    {
            //        dtData.Columns[((FineUIPro.RenderField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
            //        columnNames[index - 1] = column.HeaderText;
            //    }
            //}

            //ExcelHelper.ExportByWeb(dtData, ddlDEPTID.SelectedText + "在库保养信息", string.Format("在库保养信息_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));

            if (!DbHelperOra.Exists(string.Format("select 1 from dat_yh_doc where seqno = '{0}'", docBILLNO.Text)))
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string strSql = @"SELECT 
                                                  --A.SEQNO 养护单号,
                                                  --decode(A.YHTYPE,'0','正常','1','破损','2','失效','3','发霉','未定义') 养护标准,
                                                  '' 养护标准,
                                                  A.REASON 原因说明,
                                                  A.GDSEQ 商品编码,
                                                  A.GDNAME 商品名称,
                                                  A.GDSPEC 商品规格, 
                                                  f_getunitname(A.UNIT) 单位, 
                                                  decode(A.ISGZ,'N','否','Y','是','未定义') 是否贵重,
                                                  A.ISLOT 批号管理,
                                                  A.HSJJ 含税进价,
                                                  A.KCSL 库存数量,
                                                  A.HSJE 含税金额,
                                                  A.PHID 批号,
                                                A.STR1 货位,
                                                  A.YXQZ 有效期至,
                                                  A.RQ_SC 生产日期,
                                                  A.BZHL 包装含量,
                                                  f_getproducername(A.PRODUCER) 生产厂家,
                                                  A.ZPBH 制品编号,
                                                  A.PICINO 批次编号 
                                                FROM DAT_YH_COM A,DAT_YH_DOC B
                                                WHERE A.SEQNO=B.SEQNO AND A.SEQNO = '" + docBILLNO.Text + "'";
            //string strSearch = "";

            //if (!string.IsNullOrWhiteSpace(dpkDATE1.Text))
            //{
            //    strSearch += string.Format(" AND TRUNC(B.YHRQ) >= TO_DATE('{0}','YYYY-MM-DD')", dpkDATE1.Text);
            //}
            //if (!string.IsNullOrWhiteSpace(dpkDATE2.Text))
            //{
            //    strSearch += string.Format(" AND TRUNC(B.YHRQ) <= TO_DATE('{0}','YYYY-MM-DD')", dpkDATE2.Text);
            //}

            //if (!string.IsNullOrWhiteSpace(ddlDEPTID.SelectedValue))
            //{
            //    strSearch += string.Format(" and A.deptid = '{0}'", ddlDEPTID.SelectedValue);
            //}
            //if (!string.IsNullOrWhiteSpace(ddlYHY.Text))
            //{
            //    strSearch += string.Format(" and A.yhy = '{0}'", ddlYHY.Text);
            //}
            //if (!string.IsNullOrWhiteSpace(strSearch))
            //{
            //    strSql += strSearch;
            //}
            //strSql += " ORDER BY A.SEQNO DESC";
            DataTable dt = DbHelperOra.Query(strSql).Tables[0];
            XTBase.Utilities.ExcelHelper.ExportByWeb(dt, "在库养护信息", "在库养护信息报表_" + DateTime.Now.ToString("yyyyMMddHH") + ".xls");
        }

        protected bool DataSave(string flag = "N")
        {
            if (docFLAG.SelectedValue != "N")
            {
                Alert.Show("非[已提交]不能保存！", "消息提示", MessageBoxIcon.Warning);
                return false;
            }
            List<Dictionary<string, object>> goodsData = GridLis.GetNewAddedList().OrderBy(x => x["GDSEQ"]).ToList();
            if (goodsData.Count < 1)
            {
                Alert.Show("请添加要生成养护单的商品信息！", "提示信息", MessageBoxIcon.Warning);
                return false;
            }

            List<CommandInfo> cmdList = new List<CommandInfo>();
            if (PubFunc.StrIsEmpty(docSEQNO.Text))
            {
                docSEQNO.Text = BillSeqGet();
                docBILLNO.Text = docSEQNO.Text;
            }
            else
            {
                if (docBILLNO.Text.Length > 16)
                {
                    Alert.Show("单据编号长度不能大于16，请检查！", "消息提示", MessageBoxIcon.Warning);
                    return false;
                }
                docSEQNO.Text = docBILLNO.Text;
            }
            cmdList.Add(new CommandInfo("delete from dat_yh_doc where seqno='" + docSEQNO.Text + "'", null));
            cmdList.Add(new CommandInfo("delete from dat_yh_com where seqno='" + docSEQNO.Text + "'", null));

            MyTable mtType = new MyTable("DAT_YH_DOC");
            mtType.ColRow = PubFunc.FormDataHT(Formlis);
            mtType.ColRow.Add("BILLTYPE", BillType);
            decimal decTotal = 0;
            MyTable mtTypeMx = new MyTable("DAT_YH_COM");
            //先删除单据信息在插入
            for (int i = 0; i < goodsData.Count; i++)
            {
                mtTypeMx.ColRow = PubFunc.GridDataGet(goodsData[i]);
                if (mtTypeMx.ColRow["REASON"].ToString().Length > 80)
                {
                    Alert.Show("第【" + i + 1 + "】行【原因说明】字段超出规定长度");
                    return false;
                }

                mtTypeMx.ColRow.Add("SEQNO", docBILLNO.Text);
                mtTypeMx.ColRow["ROWNO"] = i + 1;
                mtTypeMx.ColRow.Add("PH", mtTypeMx.ColRow["PHID"]);
                decTotal += decimal.Parse(mtTypeMx.ColRow["HSJE"].ToString());
                mtTypeMx.ColRow.Remove("UNITNAME");
                mtTypeMx.ColRow.Remove("PRODUCERNAME");
                cmdList.Add(mtTypeMx.Insert());
            }
            mtType.ColRow.Add("SUBNUM", goodsData.Count);
            mtType.ColRow.Add("SUBSUM", decTotal);
            if (flag == "Y")
            {
                mtType.ColRow["FLAG"] = "Y";
                //mtType.ColRow["SHR"] = ddlSHR.SelectedValue;
                mtType.ColRow["SHR"] = UserAction.UserID;
                mtType.ColRow["SHRQ"] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
            }
            cmdList.Add(mtType.Insert());
            return DbHelperOra.ExecuteSqlTran(cmdList);
        }

        protected void btnSear_Click(object sender, EventArgs e)
        {
            string strSearch = "";
            if (!string.IsNullOrWhiteSpace(lstSEQNO.Text))
            {
                strSearch += string.Format(" and seqno like '%{0}%'", lstSEQNO.Text);
            }
            if (!string.IsNullOrWhiteSpace(lstFLAG.SelectedValue))
            {
                strSearch += string.Format(" and flag = '{0}'", lstFLAG.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(dpkDATE1.Text))
            {
                strSearch += string.Format(" AND TRUNC(YHRQ) >= TO_DATE('{0}','YYYY-MM-DD')", dpkDATE1.Text);
            }
            if (!string.IsNullOrWhiteSpace(dpkDATE2.Text))
            {
                strSearch += string.Format(" AND TRUNC(YHRQ) <= TO_DATE('{0}','YYYY-MM-DD')", dpkDATE2.Text);
            }

            if (!string.IsNullOrWhiteSpace(lstDEPTID.SelectedValue))
            {
                strSearch += string.Format(" and deptid = '{0}'", lstDEPTID.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(lstYHY.SelectedValue))
            {
                strSearch += string.Format(" and yhy = '{0}'", lstYHY.SelectedValue);
            }
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strDocSql += strSearch;
            }
            strDocSql += " ORDER BY LRRQ DESC";
            DataTable dtBill = new DataTable();
            dtBill = DbHelperOra.Query(strDocSql).Tables[0];
            GridDoc.DataSource = dtBill;
            GridDoc.DataBind();
        }

        /// <summary>
        /// 获取商品数据信息
        /// </summary>
        /// <param name="pageNum">第几页</param>
        /// <param name="pageSize">每页显示天数</param>
        /// <param name="nvc">查询条件</param>
        /// <param name="total">总的条目数</param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        //        public DataTable GetGoodsList(int pageNum, int pageSize, NameValueCollection nvc, ref int total, ref string errMsg)
        //        {
        //            string strSearch = "";
        //            if (nvc != null)
        //            {
        //                foreach (string key in nvc)
        //                {
        //                    string condition = nvc[key];
        //                    if (!string.IsNullOrEmpty(condition))
        //                    {
        //                        switch (key.ToUpper())
        //                        {
        //                            case "GDSEQ":
        //                                strSearch += string.Format(" AND （b.GDNAME LIKE '%{0}%' OR b.GDSEQ LIKE '%{0}%' OR b.ZJM LIKE '%{0}%' OR b.NAMEJC LIKE '%{0}%' OR .BAR3 LIKE '%{0}%'）", condition.ToUpper());
        //                                break;
        //                            case "PRODUCER":
        //                                strSearch += string.Format(" AND a.PRODUCER='{0}'", condition);
        //                                break;
        //                            case "DEPTID":
        //                                strSearch += string.Format(" AND a.DEPTID='{0}'", condition);
        //                                break;
        //                            case "HWID":
        //                                strSearch += string.Format(" AND a.HWID='{0}'", condition);
        //                                break;
        //                        }
        //                    }
        //                }
        //            }
        //            StringBuilder strSql = new StringBuilder(@"SELECT A.PHID,
        //                                                       A.HWID,
        //                                                       A.YXQZ,
        //                                                       f_getdeptname(a.deptid) DEPTNAME,
        //                                                       F_GETUNITNAME(a.UNIT) UNITNAME,       
        //                                                       F_GETSUPNAME(a.SUPID) SUPNAME,
        //                                                       F_GETUNITNAME(a.UNIT) UNITSMALLNAME,
        //                                                       A.KCSL,
        //                                                       (B.HSJJ * B.BZHL * A.KCSL) HSJE,
        //                                                       A.RQ_SC,
        //                                                       F_GETPRODUCERNAME(b.PRODUCER) PRODUCERNAME,
        //                                                        A.PICINO,
        //                                                       B.*
        //                                                  FROM DAT_GOODSSTOCK A, DOC_GOODS B
        //                                                 WHERE A.GDSEQ = B.GDSEQ AND A.KCSL > 0");
        //            if (!string.IsNullOrWhiteSpace(strSearch))
        //            {
        //                strSql.Append(strSearch);
        //            }
        //            strSql.Append(" order by a.deptid,a.gdseq");
        //            return GetDataTable(pageNum, pageSize, strSql, ref total);
        //        }
        public string GetSql(NameValueCollection nvc)
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
                            case "GDSEQ":
                                strSearch += string.Format(" AND (b.GDNAME LIKE '%{0}%' OR b.GDSEQ LIKE '%{0}%' OR b.ZJM LIKE '%{0}%' OR b.NAMEJC LIKE '%{0}%' OR b.BAR3 LIKE '%{0}%')", condition.ToUpper());
                                break;
                            case "PRODUCER":
                                strSearch += string.Format(" AND F_GETPRODUCERNAME(b.PRODUCER) like '%{0}%'", condition);
                                break;
                            case "DEPTID":
                                strSearch += string.Format(" AND a.DEPTID='{0}'", condition);
                                break;
                            case "HWID":
                                strSearch += string.Format(" AND a.HWID like '%{0}%'", condition);
                                break;
                        }
                    }
                }
            }
            StringBuilder strSql = new StringBuilder(@"SELECT A.PHID,
                                                       A.HWID,
                                                       A.YXQZ,
                                                       f_getdeptname(a.deptid) DEPTNAME,
                                                       F_GETUNITNAME(a.UNIT) UNITNAME,       
                                                       F_GETSUPNAME(a.SUPID) SUPNAME,
                                                       F_GETUNITNAME(a.UNIT) UNITSMALLNAME,
                                                       A.KCSL,
                                                       (B.HSJJ * B.BZHL * A.KCSL) HSJE,
                                                       A.RQ_SC,
                                                       F_GETPRODUCERNAME(b.PRODUCER) PRODUCERNAME,
                                                       A.PICINO,
                                                       decode(B.isgz,'N','否','Y','是','未定义') ISGZNAME, 
                                                       B.*
                                                  FROM DAT_GOODSSTOCK A, DOC_GOODS B
                                                 WHERE A.GDSEQ = B.GDSEQ AND A.KCSL > 0");
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql.Append(strSearch);
            }
            strSql.Append(" order by a.deptid,a.gdseq");
            return strSql.ToString();
        }

        protected void GridLeft_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridLeft.PageIndex = e.NewPageIndex;
            dataSearch();
        }

        protected void GridDoc_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            billOpen(GridDoc.Rows[e.RowIndex].Values[1].ToString());
        }

        protected void PageManager1_CustomEvent(object sender, CustomEventArgs e)
        {

            switch (e.EventArgument)
            {
                //删行
                case "billDelRow_Ok":
                    billDelRow_mes();
                    break;
                case "billDelRow_Cancel":
                    break;
            }
        }

        protected void GridDoc_RowDataBound(object sender, GridRowEventArgs e)
        {
            DataRowView row = e.DataItem as DataRowView;
            if (row != null)
            {
                string flag = row["FLAG"].ToString();
                FineUIPro.BoundField flagcol = GridDoc.FindColumn("FLAG") as FineUIPro.BoundField;
                if (flag == "N")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color1";
                }
                if (flag == "M")
                {
                    e.CellAttributes[flagcol.ColumnIndex]["data-color"] = "color2";
                }
            }
        }

        protected void btnTJ_Click(object sender, EventArgs e)
        {
            if (docSEQNO.Text.Length < 1)
            {
                Alert.Show("提交失败，请选择需要提交的单据！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
            if (DbHelperOra.ExecuteSql(string.Format("UPDATE DAT_YH_DOC SET FLAG = 'N' WHERE FLAG= 'M' AND SEQNO = '{0}'", docSEQNO.Text)) > 0)
            {
                Alert.Show("单据提交成功！");
                billOpen(docSEQNO.Text);
                OperLog("在库养护", "提交单据【" + docBILLNO.Text + "】");
            }
            else
            {
                Alert.Show("提交失败，请检查单据状态！", "提示信息", MessageBoxIcon.Warning);
                return;
            }
        }
    }
}
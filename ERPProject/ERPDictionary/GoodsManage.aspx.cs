﻿using System;
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

namespace ERPProject.ERPDictionary
{
    public partial class GoodsManage : PageBase
    {
        private static DataTable dtData;
        private static bool FLAG = true;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                //btnNew.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void DataInit()
        {
            if (!ApiClientUtil.isNull(Request.QueryString["hideks"]))
            {
                if (Request.QueryString["hideks"].Equals("1"))
                {
                    ManageKeshi.Hidden = true;
                }
            }
            PubFunc.DdlDataGet("DDL_GOODS_TYPE", srhCATID0, ddlCATID0);
            PubFunc.DdlDataGet("DDL_DOC_SUPPLIERNULL", trbSUPPLIER);
            PubFunc.DdlDataGet("DDL_PRODUCER", trbPRODUCER);

            PubFunc.DdlDataGet("DDL_UNIT", ddlUNIT, ddlUNIT_DABZ, ddlUNIT_ZHONGBZ);

            PubFunc.DdlDataGet("DDL_GOODS_STATUS", ddlFLAG, srhFLAG);
            //PubFunc.DdlDataGet(ddlCATID, "DDL_SYS_CATLAST");
            string strSql = @"select code,'【'||code||'】'||name name,(class-1) TreeLevel,decode(islast,'Y',1,0) islast
                                    from sys_category
                                    ORDER BY code  ";
            List<CategoryTreeBean> myList = new List<CategoryTreeBean>();
            DataTable categoryTreeTable = DbHelperOra.Query(strSql).Tables[0];
            foreach (DataRow dr in categoryTreeTable.Rows)
            {
                myList.Add(new CategoryTreeBean(dr["code"].ToString(), dr["name"].ToString(), Convert.ToInt16(dr["TreeLevel"]), Convert.ToInt16(dr["islast"]) == 1));
            }
            // 绑定到下拉列表（启用模拟树功能）
            ddlCATID.EnableSimulateTree = true;
            ddlCATID.DataTextField = "Name";
            ddlCATID.DataValueField = "Id";
            ddlCATID.DataEnableSelectField = "EnableSelect";
            ddlCATID.DataSimulateTreeLevelField = "Level";
            ddlCATID.DataSource = myList;
            ddlCATID.DataBind();

            string Sql = @"select code,'【'||code||'】'||name name,(class-1) TreeLevel,decode(islast,'Y',1,0) islast
                                    from SYS_CATUSER
                                    ORDER BY code  ";
            List<CategoryTreeBean> mList = new List<CategoryTreeBean>();
            DataTable TreeTable = DbHelperOra.Query(Sql).Tables[0];
            foreach (DataRow dr in TreeTable.Rows)
            {
                mList.Add(new CategoryTreeBean(dr["code"].ToString(), dr["name"].ToString(), Convert.ToInt16(dr["TreeLevel"]), Convert.ToInt16(dr["islast"]) == 1));
            }
            // 绑定到下拉列表（启用模拟树功能）
            ddlCATUSER.EnableSimulateTree = true;
            ddlCATUSER.DataTextField = "Name";
            ddlCATUSER.DataValueField = "Id";
            ddlCATUSER.DataEnableSelectField = "EnableSelect";
            ddlCATUSER.DataSimulateTreeLevelField = "Level";
            ddlCATUSER.DataSource = mList;
            ddlCATUSER.DataBind();

            hfdIsNew.Text = "Y";
            hfdConfig.Text = "";
            dpkINRQ.SelectedDate = DateTime.Now;
            ddlFLAG.SelectedValue = "N";
            tbxBZHL.Text = "1";
            ddlCATID0.SelectedValue = "2";
            //ckbISFLAG7.Checked = true;
        }
        protected void trbPRODUCER_TriggerClick(object sender, EventArgs e)
        {
            dataSearch();
        }
        private void dataSearch()
        {
            int total = 0;
            string msg = "";
            NameValueCollection nvc = new NameValueCollection();
            if (srhCATID0.SelectedValue.Length > 0) nvc.Add("CATID0", srhCATID0.SelectedValue);
            //if (srhPRODUCER.SelectedValue.Length > 0) nvc.Add("PRODUCER", srhPRODUCER.SelectedValue);
            if (srhFLAG.SelectedValue.Length > 0) nvc.Add("FLAG", srhFLAG.SelectedValue);
            if (trbSearch.Text.Length > 0) nvc.Add("CX", trbSearch.Text.Trim());

            dtData = GetGoodsList(GridGoods.PageIndex, GridGoods.PageSize, nvc, ref total, ref msg);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();
        }

        protected void ddlCATID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strCATID0, strCATID, strSQL;
            strCATID = ((FineUIPro.DropDownList)sender).SelectedValue;
            if (PubFunc.StrIsEmpty(strCATID)) return;

            strSQL = "select type from sys_category where code='" + strCATID + "'";
            strCATID0 = DbHelperOra.GetSingle(strSQL).ToString();

            ddlCATID0.SelectedValue = strCATID0;
        }
        protected void GridDept_Selected(object sender, EventArgs e)
        {
            //if (TabStripMain.ActiveTabIndex != 2) return;
            string seq = tbxGDID.Text;
            if (PubFunc.StrIsEmpty(seq) || GridDept.Rows.Count > 0) return;

            //string strSql = @"SELECT B.CODE CODE,B.NAME NAME,nvl(A.FLAG,0)||'/'||nvl(Store,0) OLD,
            //                         nvl(A.FLAG,0) FLAG,nvl(Store,0) Store,nvl(A.FLAG,0)||'/'||nvl(Store,0) NEW
            //                    FROM (select deptid,TO_NUMBER(DECODE(ISCFG,'Y','1','1','1','0')) FLAG,TO_NUMBER(DECODE(SPLB,'Y','1','1','1','0')) Store FROM DOC_GOODSCFG where gdseq='{0}') A,
            //                         SYS_DEPT B 
            //                   WHERE B.CODE =A.DEPTID(+) ";//B.ISLAST='Y' AND 
            string strSql = @"SELECT A.CODE,NVL(B.ISCFG,'N') FLAG,A.NAME
                FROM SYS_DEPT A ,(SELECT DEPTID,ISCFG FROM DOC_GOODSCFG WHERE GDSEQ = '{0}') B
                WHERE A.CODE = B.DEPTID(+) ORDER BY NVL(B.ISCFG,'N') DESC";
            DataTable dtCFG = DbHelperOra.Query(string.Format(strSql, seq)).Tables[0];
            GridDept.DataSource = dtCFG;
            GridDept.DataBind();
            List<int> selectRow = new List<int>();
            for (int i = 0; i < dtCFG.Rows.Count; i++)
            {
                if (dtCFG.Rows[i]["FLAG"].ToString() != "N")
                {
                    selectRow.Add(i);
                }
            }
            GridDept.SelectedRowIndexArray = selectRow.ToArray();
            //新增状态商品不允许配置科室  王阿磊2015年8月26日 22:29:52
            if (ddlFLAG.SelectedText == "新增")
            {
                GridDept.Enabled = false;
            }
            else
            {
                GridDept.Enabled = true;
            }

        }
        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            dataSearch();
        }
        protected void btnNew_Click(object sender, EventArgs e)
        {
            PubFunc.FormDataClear(FormMain);
            PubFunc.FormDataClear(FormAssist);
            hfdIsNew.Text = "Y";
            hfdConfig.Text = "";

            dpkINRQ.SelectedDate = DateTime.Now;
            ddlFLAG.SelectedValue = "N";
            tbxBZHL.Text = "1";
            //ddlCATID.SelectedValue = "BG";
            //ddlCATID0.SelectedValue = "5";
            ddlUNIT_ORDER.SelectedValue = "X";
            ddlUNIT_SELL.SelectedValue = "X";
            btn_state(true);
            //清空部门配置信息
            GridDept.DataSource = null;
            GridDept.DataBind();
            //新增状态商品不允许配置科室  王阿磊2015年8月26日 22:29:52
            if (ddlFLAG.SelectedText == "新增")
            {
                GridDept.Enabled = false;
            }
            else
            {
                GridDept.Enabled = true;
            }
        }
        protected void btn_state(bool Flag)
        {
            //按钮状态
            tbxGDID.Enabled = Flag;
            tbsGDNAME.Enabled = Flag;
            tbxGDSPEC.Enabled = Flag;
            nbbBARCODE.Enabled = Flag;
            ddlUNIT.Enabled = Flag;
            nbbHSJJ.Enabled = Flag;
            nbbBHSJJ.Enabled = Flag;
            trbPRODUCER.Enabled = Flag;
            tbxPIZNO.Enabled = Flag;
            ddlUNIT_DABZ.Enabled = Flag;
            nbbBARCODE_DABZ.Enabled = Flag;
            ddlUNIT_ZHONGBZ.Enabled = Flag;
            nbbNUM_ZHONGBZ.Enabled = Flag;
            nbbBARCODE_ZHONGBZ.Enabled = Flag;
            nbbNUM_DABZ.Enabled = Flag;
            ddlCATID.Enabled = Flag;
            ddlISFLAG7.Enabled = Flag;
        }
        protected void btnDelete_Click(object sender, EventArgs e)
        {

        }

        private void DataCheck(DataRow row, FineUIPro.Form form, ref string msg)
        {
            foreach (FineUIPro.FormRow fr in form.Rows)
            {
                if (fr.Hidden) continue;
                foreach (Field cl in fr.Items)
                {
                    if (cl is FineUIPro.TextBox)
                    {
                        FineUIPro.TextBox tbx = (FineUIPro.TextBox)cl;
                        if (tbx.Text != row[tbx.ID.Substring(3)].ToString())
                        {
                            msg += string.Format("字段【{0}】由【{1}】变更为【{2}】；", tbx.Text + "_" + tbx.ID.Substring(3), row[tbx.ID.Substring(3)].ToString(), tbx.Text);
                        }
                    }
                    else if (cl is FineUIPro.DropDownList)
                    {
                        FineUIPro.DropDownList ddl = (FineUIPro.DropDownList)cl;
                        if (ddl.SelectedValue != row[ddl.ID.Substring(3)].ToString())
                        {
                            msg += string.Format("字段【{0}】由【{1}】变更为【{2}】；", ddl.Text + "_" + ddl.ID.Substring(3), row[ddl.ID.Substring(3)].ToString(), ddl.SelectedValue);
                        }
                    }
                    else if (cl is FineUIPro.NumberBox)
                    {
                        FineUIPro.NumberBox nbb = (FineUIPro.NumberBox)cl;
                        if (nbb.Text != row[nbb.ID.Substring(3)].ToString())
                        {
                            msg += string.Format("字段【{0}】由【{1}】变更为【{2}】；", nbb.Text + "_" + nbb.ID.Substring(3), row[nbb.ID.Substring(3)].ToString(), nbb.Text);
                        }
                    }
                    else if (cl is FineUIPro.DatePicker)
                    {
                        FineUIPro.DatePicker dpk = (FineUIPro.DatePicker)cl;
                        if (dpk.Text != row[dpk.ID.Substring(3)].ToString())
                        {
                            msg += string.Format("字段【{0}】由【{1}】变更为【{2}】；", dpk.Text + "_" + dpk.ID.Substring(3), row[dpk.ID.Substring(3)].ToString(), dpk.Text);
                        }
                    }
                    else if (cl is FineUIPro.CheckBox)
                    {
                        FineUIPro.CheckBox ckb = (FineUIPro.CheckBox)cl;
                        //row[ckb.ID.Substring(3)].ToString()
                        string strValue = ckb.Checked ? "Y" : "N";
                        if (row[ckb.ID.Substring(3)].ToString() != strValue)
                        {
                            msg += string.Format("字段【{0}】由【{1}】变更为【{2}】；", ckb.Text + "_" + ckb.ID.Substring(3), row[ckb.ID.Substring(3)].ToString(), strValue);
                        }
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (PubFunc.FormDataCheck(FormMain).Length > 0) return;
            //增加自动生成的编码
            if (hfdIsNew.Text == "Y" && tbxGDID.Text.Trim().Length < 1)
            {
                string catid = string.IsNullOrWhiteSpace(ddlCATID0.SelectedValue) ? "2" : ddlCATID0.SelectedValue;
                tbxGDID.Text = DbHelperOra.GetSingle("SELECT F_GET_BGGDSEQ('" + catid + "') FROM DUAL").ToString();
            }

            if (tbxGDID.Text.Trim().Length < 1)
            {
                Alert.Show("输入商品编码不符合规范!", "操作提示", MessageBoxIcon.Warning);
                return;
            }

            if ((",N,Y").IndexOf(ddlFLAG.SelectedValue) < 1)
            {
                Alert.Show("当前商品为【" + ddlFLAG.SelectedText + "】状态，不允许进行编辑！", "操作提示", MessageBoxIcon.Warning);
                return;
            }

            //增加单位的验证
            if (ddlUNIT_ORDER.SelectedValue == "D" && PubFunc.StrIsEmpty(nbbNUM_DABZ.Text))
            {
                Alert.Show("订货单位为大包装,请维护大包装数量!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (ddlUNIT_ORDER.SelectedValue == "D" && PubFunc.StrIsEmpty(ddlUNIT_DABZ.SelectedValue))
            {
                Alert.Show("订货单位为大包装,请维护大包装单位!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (ddlUNIT_ORDER.SelectedValue == "Z" && PubFunc.StrIsEmpty(nbbNUM_ZHONGBZ.Text))
            {
                Alert.Show("订货单位为中包装,请维护中包装数量!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (ddlUNIT_ORDER.SelectedValue == "Z" && PubFunc.StrIsEmpty(ddlUNIT_ZHONGBZ.SelectedValue))
            {
                Alert.Show("订货单位为中包装,请维护中包装单位!", "操作提示", MessageBoxIcon.Warning);
                return;
            }

            if (ddlUNIT_SELL.SelectedValue == "D" && PubFunc.StrIsEmpty(nbbNUM_DABZ.Text))
            {
                Alert.Show("出库单位为大包装,请维护大包装数量!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (ddlUNIT_SELL.SelectedValue == "D" && PubFunc.StrIsEmpty(ddlUNIT_DABZ.SelectedValue))
            {
                Alert.Show("出库单位为大包装,请维护大包装单位!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (ddlUNIT_SELL.SelectedValue == "Z" && PubFunc.StrIsEmpty(nbbNUM_ZHONGBZ.Text))
            {
                Alert.Show("出库单位为中包装,请维护中包装数量!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (ddlUNIT_SELL.SelectedValue == "Z" && PubFunc.StrIsEmpty(ddlUNIT_ZHONGBZ.SelectedValue))
            {
                Alert.Show("出库单位为中包装,请维护中包装单位!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (!PubFunc.StrIsEmpty(nbbHSJJ.Text))
            {
                if (Convert.ToDecimal(nbbHSJJ.Text) < 0)
                {
                    Alert.Show("含税进价不能小于0", "操作提示", MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                return;
            }
            //if (PubFunc.StrIsEmpty(trbSUPPLIER.SelectedValue))
            //{
            //    Alert.Show("缺省默认供应商不能为空", "操作提示", MessageBoxIcon.Warning);
            //    return;

            //}
            //如果是高值商品，订货单位和出库单位要相等
            if (ckbISGZ.Checked)
            {
                if (ddlUNIT_ORDER.SelectedValue != "X" || ddlUNIT_SELL.SelectedValue != "X")
                {
                    Alert.Show("高值商品，订货单位和出库单位必须为【最小包装单位】不一致，请调整!", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                //不允许是定数商品
                if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DOC_GOODSCFG WHERE NVL(NUM1,0)> 0 AND NVL(DSNUM,0) > 0 AND GDSEQ = '{0}'", tbxGDID.Text)))
                {
                    Alert.Show("商品为定数商品，不允许设置为高值", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                //必须无库存时可设置为高值商品
                if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_GOODSSTOCK A,DOC_GOODS B WHERE A.KCSL > 0 AND A.GDSEQ = '{0}' AND A.GDSEQ =B.GDSEQ AND B.ISGZ = 'N'", tbxGDID.Text)))
                {
                    Alert.Show("商品存在未使用库存，不允许设置为高值商品", "消息提示", MessageBoxIcon.Warning);
                    return;
                }
            }
            //if (ckbISFLAG3.Checked)
            //{
            //    object Supname = DbHelperOra.GetSingle("select f_getsupname(SUPID) from doc_goodssup WHERE GDSEQ = '" + tbxGDID.Text.Trim() + "'");
            //    if (!PubFunc.StrIsEmpty((Supname ?? "").ToString()))
            //    {
            //        Alert.Show("该商品被设置为【直送商品】但它当前为【" + Supname + "】供应商的库存品，无法执行操作。", "操作提示", MessageBoxIcon.Warning);
            //        return;
            //    }
            //}
            if (ckbISFLAG3.Checked)
            {

                DataTable dt = DbHelperOra.QueryForTable(string.Format(@"SELECT SD.CODE FROM DOC_GOODSCFG DGC ,SYS_DEPT SD WHERE DGC.DEPTID=SD.CODE AND DGC.GDSEQ='{0}' AND SD.TYPE='1' AND (SELECT F_GETKCGDSEQ(SD.CODE,DGC.GDSEQ,'') FROM DUAL)>0", tbxGDID.Text.Trim()));
                if (dt.Rows.Count > 0)
                {
                    string deptname = "";
                    foreach (DataRow dr in dt.Rows)
                    {
                        deptname += dr["CODE"].ToString() + ",";
                    }
                    Alert.Show("商品在库房【" + deptname.Substring(0, deptname.Length - 1) + "】有库存，无法被设置为【直送商品】。", "操作提示", MessageBoxIcon.Warning);
                    return;
                }


            }

            if (ckbISFLAG3.Checked)
            {


                if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DOC_GOODSCFG WHERE GDSEQ = '{0}' AND NUM1 >0 AND DSNUM>0", tbxGDID.Text.Trim())))
                {
                    Alert.Show("该商品被设置为定数,不允许设置为直送商品！", "操作提示", MessageBoxIcon.Warning);
                    return;
                }
            }
            hfdGDSEQ.Text = tbxGDID.Text;
            MyTable mtType = new MyTable("DOC_GOODS");
            mtType.ColRow = PubFunc.FormDataHT(FormMain);
            Hashtable htAssist = PubFunc.FormDataHT(FormAssist);
            foreach (string key in htAssist.Keys)
            {
                mtType.ColRow.Add(key, htAssist[key]);
            }

            //数据处理 
            mtType.ColRow.Remove("ISNEW");
            if (hfdIsNew.Text == "Y")
            {
                if (DbHelperOra.Exists(string.Format("select 1 from doc_goods where GDSEQ='{0}'", tbxGDID.Text.Trim())))
                {
                    Alert.Show(string.Format("您输入的商品编码【{0}】已存在，请重新输入！", tbxGDID.Text), MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(trbSUPPLIER.SelectedValue))
                {
                    //trbSUPPLIER.SelectedValue = "00002";
                    mtType.ColRow["SUPPLIER"] = "00002";
                }
                mtType.ColRow["GDSEQ"] = mtType.ColRow["GDID"];
                mtType.ColRow["ISFLAG7"] = ddlISFLAG7.SelectedValue;
                mtType.InsertExec();
            }
            else
            {
                if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DAT_RK_DOC A,DAT_RK_COM B WHERE A.SEQNO=B.SEQNO AND A.FLAG IN ('M','N','R') AND ISGZ='{0}' AND B.GDNAME='{1}'", ckbISGZ.Checked ? "N" : "Y", tbsGDNAME.Text)))
                {
                    Alert.Show(string.Format("商品【{0}】还有入库单未处理，请处理完入库单之后再做修改！", tbsGDNAME.Text), "警告提示", MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(trbSUPPLIER.SelectedValue))
                {
                    //trbSUPPLIER.SelectedValue = "00002";
                    mtType.ColRow["SUPPLIER"] = "00002";
                }

                string msg = "";
                DataTable dt = DbHelperOra.Query(string.Format("SELECT * FROM DOC_GOODS WHERE GDSEQ = '{0}'", mtType.ColRow["GDSEQ"])).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataCheck(dt.Rows[0], FormMain, ref msg);
                    DataCheck(dt.Rows[0], FormAssist, ref msg);
                }
                List<CommandInfo> cmdList = new List<CommandInfo>();
                //if (dt.Rows[0]["SUPPLIER"].ToString() != trbSUPPLIER.SelectedValue && !string.IsNullOrWhiteSpace(trbSUPPLIER.SelectedValue) && trbSUPPLIER.SelectedValue != "00001")
                //{
                //    string sup_sql = @"declare
                //                                  ln_num number;
                //                                begin
                //                                  select count(1) into ln_num from doc_goodssup where gdseq = '{0}';
                //                                  if ln_num > 0 then
                //                                    update doc_goodssup s
                //                                       set s.supid = '{1}', s.supname = '{2}'
                //                                     where s.gdseq = '{0}' ;
                //                                  else
                //                                    insert into doc_goodssup
                //                                      (custid, gdseq, supid, supname,str3)
                //                                    values
                //                                      (f_getpara('USERCODE'), '{0}', '{1}', '{2}',SUBSTR(nvl(f_getpara('FDSMODE'),'XSJ'),1,1));
                //                                  end if;
                //                                end;";
                //    cmdList.Add(new CommandInfo(string.Format(sup_sql, hfdGDSEQ.Text, trbSUPPLIER.SelectedValue, trbSUPPLIER.SelectedText), null));
                //}
                if (!string.IsNullOrEmpty(msg))
                {
                    //Alert.Show(DateTime.Now + "', '" + UserAction.UserID + "', 'USER', 'GoodsManage.aspx', '商品资料更改："+msg );
                    //return;
                    string sql = "INSERT INTO sys_operlog (SEQNO, RQSJ, USERID, STATION, FUNCID, MEMO, TYPE) VALUES (seq_operlog.nextval, SYSDATE, '" + UserAction.UserID + "', 'USER', '商品资料管理', '商品【" + mtType.ColRow["GDSEQ"] + "】资料更新：" + msg + "','LOG')";
                    cmdList.Add(new CommandInfo(sql, null));
                }
                //if (!string.IsNullOrWhiteSpace(trbSUPPLIER.SelectedValue))
                //{ 
                //    cmdList.Add(new CommandInfo(string.Format("update doc_goods set supplier='{0}' where gdseq='{1}'", trbSUPPLIER.SelectedValue,hfdGDSEQ.Text), null));
                //}
                mtType.ColRow["UPTTIME"] = DateTime.Now;
                cmdList.Add(mtType.Update(""));
                DbHelperOra.ExecuteSqlTran(cmdList);
                DataTable dtBack = DbHelperOra.Query("select * from doc_goods where gdseq='" + tbxGDID.Text + "'").Tables[0];
                PubFunc.FormDataSet(FormMain, dtBack.Rows[0]);
                PubFunc.FormDataSet(FormAssist, dtBack.Rows[0]);

            }
            //更新部门配置信息
            string cfg = "";
            int[] selections = GridDept.SelectedRowIndexArray;
            foreach (int rowIndex in selections)
            {

                cfg += string.Concat(GridDept.DataKeys[rowIndex][0].ToString(), ",");
            }
            if (!string.IsNullOrEmpty(cfg))
            {
                OracleParameter[] parameters =
                {
                    new OracleParameter("V_GDSEQ",OracleDbType.Varchar2,20),
                    new OracleParameter("V_CFGLIST",OracleDbType.Varchar2,4000),
                    new OracleParameter("V_USERID",OracleDbType.Varchar2,20)
                };
                parameters[0].Value = hfdGDSEQ.Text;
                parameters[1].Value = cfg.TrimEnd(',');
                parameters[2].Value = UserAction.UserID;
                try
                {
                    DbHelperOra.RunProcedure("P_GOODSMANAGE", parameters);
                }
                catch (Exception ex)
                {
                    Alert.Show(Error_Parse(ex.Message), "提示信息", MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                OracleParameter[] parameters =
               {
                    new OracleParameter("V_GDSEQ",OracleDbType.Varchar2,20),
                    new OracleParameter("V_CFGLIST",OracleDbType.Varchar2,2000),
                    new OracleParameter("V_USERID",OracleDbType.Varchar2,20)
                };
                parameters[0].Value = hfdGDSEQ.Text;
                parameters[1].Value = "NULL";
                parameters[2].Value = UserAction.UserID;
                try
                {
                    DbHelperOra.RunProcedure("P_GOODSMANAGE", parameters);
                }
                catch (Exception ex)
                {

                    Alert.Show(Error_Parse(ex.Message), "提示信息", MessageBoxIcon.Warning);
                    return;
                }
            }
            Alert.Show("数据保存成功！");

            hfdIsNew.Text = "N";
            btExp.Enabled = false;
            ddlUNIT.Enabled = false;
            //nbbHSJJ.Enabled = true;
            ddlCATID.Enabled = false;

            //保存后，界面的锁定 lvj 20160615
            ddlISFLAG7.Enabled = false;
            tbxGDID.Enabled = false;
            tbxGDSPEC.Enabled = false;
            ddlUNIT.Enabled = false;
            nbbBHSJJ.Enabled = false;
            trbPRODUCER.Enabled = false;
            tbxPIZNO.Enabled = false;
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
        protected void btExp_Click(object sender, EventArgs e)
        {
            int total = 0;
            string msg = "";
            NameValueCollection nvc = new NameValueCollection();
            if (srhCATID0.SelectedValue.Length > 0) nvc.Add("CATID0", srhCATID0.SelectedValue);
            //if (srhPRODUCER.SelectedValue.Length > 0) nvc.Add("PRODUCER", srhPRODUCER.SelectedValue);
            if (srhFLAG.SelectedValue.Length > 0) nvc.Add("FLAG", srhFLAG.SelectedValue);
            if (trbSearch.Text.Length > 0) nvc.Add("CX", trbSearch.Text.Trim());

            DataTable dtExt = GetGoodsList(0, 0, nvc, ref total, ref msg);
            if (dtExt == null || dtExt.Rows.Count < 1)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string[] columnNames = new string[GridGoods.Columns.Count - 1];
            for (int index = 1; index < GridGoods.Columns.Count; index++)
            {
                GridColumn column = GridGoods.Columns[index];
                if (column is FineUIPro.BoundField)
                {
                    dtExt.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index - 1] = column.HeaderText;
                }
            }
            XTBase.Utilities.ExcelHelper.ExportByWeb(dtExt.DefaultView.ToTable(true, columnNames), "商品信息", string.Format("商品信息导出_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
        protected void Window1_Close(object sender, EventArgs e)
        {
            dataSearch();
        }

        protected void GridGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            TabStripMain.ActiveTabIndex = 0;
            string seq = GridGoods.Rows[e.RowIndex].Values[1].ToString();

            DataTable dt = DbHelperOra.Query("select * from doc_goods where gdseq='" + seq.Trim() + "'").Tables[0];
            PubFunc.FormDataSet(FormMain, dt.Rows[0]);
            PubFunc.FormDataSet(FormAssist, dt.Rows[0]);
            GridDept.DataSource = null;//清除商品科室配置
            GridDept.DataBind();
            GridDept_Selected(null, null);//双击即绑定GridDept数据，而不是在点开标签页绑定
            hfdIsNew.Text = "N";
            hfdConfig.Text = "";
            ddlISFLAG7.Enabled = false;
            if (string.IsNullOrWhiteSpace(ddlISFLAG7.SelectedValue) || ddlISFLAG7.SelectedValue == "Y")
            {
                if (!string.IsNullOrWhiteSpace(ddlFLAG.SelectedValue) && ddlFLAG.SelectedValue != "N")
                {

                    tbsGDNAME.Enabled = false;
                    nbbBARCODE.Enabled = false;
                    ddlUNIT.Enabled = false;
                }
                else
                {
                    tbsGDNAME.Enabled = true;
                    nbbBARCODE.Enabled = true;
                    ddlUNIT.Enabled = true;
                }
                tbxGDID.Enabled = false;
                ddlCATID.Enabled = false;
                ddlUNIT_DABZ.Enabled = true;
                nbbBARCODE_DABZ.Enabled = true;
                ddlUNIT_ZHONGBZ.Enabled = true;
                nbbNUM_ZHONGBZ.Enabled = true;
                nbbBARCODE_ZHONGBZ.Enabled = true;
                nbbNUM_DABZ.Enabled = true;
                ddlCATID.Enabled = true;
                //新增状态商品不允许配置科室  王阿磊2015年8月26日 22:29:52
                if (ddlFLAG.SelectedText == "新增")
                {
                    GridDept.Enabled = false;
                }
                else
                {
                    GridDept.Enabled = true;
                }
            }
            else
            {
                btn_state(false);
                nbbHSJJ.Enabled = true;
                ddlUNIT_DABZ.Enabled = true;
                nbbBARCODE_DABZ.Enabled = true;
                ddlUNIT_ZHONGBZ.Enabled = true;
                nbbNUM_ZHONGBZ.Enabled = true;
                nbbBARCODE_ZHONGBZ.Enabled = true;
                nbbNUM_DABZ.Enabled = true;
                ddlCATID.Enabled = true;
            }
        }

        public DataTable GetGoods(NameValueCollection nvc)
        {
            int total = 0;
            string strMsg = "";
            return GetGoodsList(0, 0, nvc, ref total, ref strMsg);
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
                            case "SEQ":
                                strSearch += string.Format(" AND sp.GDSEQ='{0}'", condition);
                                break;
                            case "CATID0":
                                strSearch += string.Format(" AND sp.CATID0='{0}'", condition);
                                break;
                            case "FLAG":
                                strSearch += string.Format(" AND sp.FLAG='{0}'", condition);
                                break;
                            case "CX":
                                strSearch += string.Format(" AND (sp.GDSEQ LIKE '%{0}%' OR sp.GDNAME LIKE '%{0}%' OR sp.HISCODE LIKE '%{0}%' OR sp.HISNAME LIKE '%{0}%' OR sp.BARCODE  LIKE '%{0}%' OR  sp.BAR3 LIKE '%{0}%' OR sp.ZJM  LIKE '%{0}%' OR F_GETPRODUCERNAME(SP.PRODUCER) LIKE '%{0}%')", condition.ToUpper());
                                break;
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(srhISFLAG7.SelectedValue))
            {
                strSearch += string.Format(" AND sp.ISFLAG7 = '{0}'", srhISFLAG7.SelectedValue);
            }
            string strSql = @"SELECT ' '||SP.GDSEQ GDSEQ,SP.GDID,F_GETHISINFO（SP.GDSEQ，'GDNAME') GDNAME,SP.BARCODE,E.NAME CATID0NAME,B.NAME CATID0NAME_F,F_GETHISINFO（SP.GDSEQ，'GDSPEC') GDSPEC,D.NAME UNITNAME,SP.BZHL,
                               ROUND(SP.HSJJ,4) HSJJ,ROUND(SP.LSJ,4) LSJ,C.SUPNAME,SP.ZPBH,S.NAME FLAG_CN,SP.PIZNO,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,decode(sp.isflag7,'Y','是','否') ISNEW_CN
                          from DOC_GOODS SP,
                               SYS_CATEGORY B,
                               DOC_SUPPLIER C,
                               DOC_GOODSUNIT D,
                               doc_goodstype e,
                               (SELECT CODE, NAME FROM SYS_CODEVALUE WHERE TYPE = 'GOODS_STATUS') S
                         WHERE SP.CATID=B.CODE(+) AND SP.FLAG=S.CODE AND SP.SUPPLIER=C.SUPID(+) AND SP.UNIT = D.CODE(+) and SP.CATID0 = e.code(+) ";

            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql = strSql + strSearch + " order by SP.gdseq";
            }
            else
            {
                strSql = strSql + " order by SP.gdseq";
            }

            return PubFunc.DbGetPage(pageNum, pageSize, strSql, ref total);
        }

        protected void GridGoods_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            dataSearch();
        }

        protected void tbsGDNAME_TextChanged(object sender, EventArgs e)
        {
            tbxNAMEJC.Text = tbsGDNAME.Text;
            //tbxZJM.Text = XTBase.Utilities.PinYinUtil.GetCodstring(tbsGDNAME.Text);
            tbxZJM.Text = Doc.GetPinYinFirst(tbsGDNAME.Text);
        }

        protected void GridDept_RowCommand(object sender, GridCommandEventArgs e)
        {
            int columnIndex = e.ColumnIndex;
            if (e.CommandName == "Store")
            {
                columnIndex = columnIndex - 1;
            }

            FineUIPro.CheckBoxField checkField1 = (FineUIPro.CheckBoxField)GridDept.FindColumn(columnIndex);
            FineUIPro.CheckBoxField checkField2 = (FineUIPro.CheckBoxField)GridDept.FindColumn(columnIndex + 1);
            string IsCFG = checkField1.GetCheckedState(e.RowIndex) ? "1" : "0";
            string SPLB = checkField2.GetCheckedState(e.RowIndex) ? "1" : "0";
            GridDept.Rows[e.RowIndex].Values[4] = IsCFG + "/" + SPLB;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }

        protected void tbxHISNAME_TextChanged(object sender, EventArgs e)
        {
            tbxSTR4.Text = XTBase.Utilities.PinYinUtil.GetCodstring(tbxHISNAME.Text);
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            if (e.SortField == "FLAG")
            {
                for (int rowIndex = 0; rowIndex < GridDept.Rows.Count; rowIndex++)
                {

                    FineUIPro.CheckBoxField cbxEnable = GridDept.FindColumn("cbxEnable") as FineUIPro.CheckBoxField;
                    cbxEnable.SetCheckedState(rowIndex, FLAG);
                    FineUIPro.CheckBoxField checkField1 = (FineUIPro.CheckBoxField)GridDept.FindColumn("cbxEnable");
                    FineUIPro.CheckBoxField checkField2 = (FineUIPro.CheckBoxField)GridDept.FindColumn("cbxStore");
                    string IsCFG = checkField1.GetCheckedState(rowIndex) ? "1" : "0";
                    string SPLB = checkField2.GetCheckedState(rowIndex) ? "1" : "0";
                    GridDept.Rows[rowIndex].Values[4] = IsCFG + "/" + SPLB;

                }
                FLAG = !FLAG;
            }


        }
    }
}
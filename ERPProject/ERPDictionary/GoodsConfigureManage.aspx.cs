﻿using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using XTBase.Utilities;
using Newtonsoft.Json.Linq;

namespace ERPProject.ERPDictionary
{
    public partial class GoodsConfigureManage : PageBase
    {
        private static string strDEPT = "select CODE,NAME, ('['||CODE||']'||NAME) DEPTNAME, TYPE DEPTTYPE from sys_dept where flag = 'Y' order by code";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
            }
        }

        private void DataInit()
        {
            PubFunc.DdlDataGet(ddlGoodsType, "DDL_GOODS_TYPE");
            //PubFunc.DdlDataGet(ddlDept, "DDL_SYS_DEPT");
            //绑定下拉树
            string strSql = @"select code,name,TreeLevel,islast from(SELECT '' CODE,'--请选择--' NAME,0 TreeLevel,0 islast
          FROM DUAL
        UNION ALL 
select code,'['||code||']'||name||'-'||ZJM name,(class-1) TreeLevel,decode(islast,'Y',1,0) islast
                                    from SYS_DEPT)
                                    ORDER BY code  ";
            List<CategoryTreeBean> myList = new List<CategoryTreeBean>();
            DataTable categoryTreeTable = DbHelperOra.Query(strSql).Tables[0];
            foreach (DataRow dr in categoryTreeTable.Rows)
            {
                myList.Add(new CategoryTreeBean(dr["code"].ToString(), dr["name"].ToString(), Convert.ToInt16(dr["TreeLevel"]), true));
            }
            ddlDept.EnableSimulateTree = true;
            ddlDept.DataTextField = "Name";
            ddlDept.DataValueField = "Id";
            ddlDept.DataEnableSelectField = "EnableSelect";
            ddlDept.DataSimulateTreeLevelField = "Level";
            ddlDept.DataSource = myList;
            ddlDept.DataBind();

            ddlDeptFrom.EnableSimulateTree = true;
            ddlDeptFrom.DataTextField = "Name";
            ddlDeptFrom.DataValueField = "Id";
            ddlDeptFrom.DataEnableSelectField = "EnableSelect";
            ddlDeptFrom.DataSimulateTreeLevelField = "Level";
            ddlDeptFrom.DataSource = myList;
            ddlDeptFrom.DataBind();

            ddlDeptTo.EnableSimulateTree = true;
            ddlDeptTo.DataTextField = "Name";
            ddlDeptTo.DataValueField = "Id";
            ddlDeptTo.DataEnableSelectField = "EnableSelect";
            ddlDeptTo.DataSimulateTreeLevelField = "Level";
            ddlDeptTo.DataSource = myList;
            ddlDeptTo.DataBind();
            PubFunc.DdlDataGet(ddlStoreType, "DDL_SYS_DEPOT");

            //string dptSql = "select CODE,NAME, ('['||CODE||']'||NAME) DEPTNAME from sys_dept where flag = 'Y' order by code";
            GridDepartment.DataSource = DbHelperOra.Query(strDEPT).Tables[0];
            GridDepartment.DataBind();

            nbxDSNUM.Enabled = false;
            NubNUM1.Enabled = false;
        }

        private void dataSearch(string type = "left")
        {
            int total = 0;
            string msg = "";
            NameValueCollection nvc = new NameValueCollection();
            if (type == "left")
            {
                if (ddlGoodsType.SelectedValue.Length > 0) nvc.Add("CATID", ddlGoodsType.SelectedValue);
                if (tgbSearch.Text.Length > 0) nvc.Add("CX", tgbSearch.Text);
                if (ddlStoreType.SelectedValue.Length > 0) nvc.Add("DEPTOUT", ddlStoreType.SelectedValue);
                if (ddlDept.SelectedValue.Length > 0) nvc.Add("LEFT", ddlDept.SelectedValue);
                
                GridGoods.DataSource = GetGoodsList(GridGoods.PageIndex, GridGoods.PageSize, nvc, ref total, ref msg);
                GridGoods.DataBind();
                GridGoods.RecordCount = total;
            }
            else if (type == "right")
            {
                string sortField = GridCFGGoods.SortField;
                string sortDirection = GridCFGGoods.SortDirection;

                if (trbSearch.Text.Length > 0) nvc.Add("CX", trbSearch.Text);
                if (ddlDept.SelectedValue.Length > 0) nvc.Add("RIGHT", ddlDept.SelectedValue);

                DataTable table = GetGoodsList(GridCFGGoods.PageIndex, GridCFGGoods.PageSize, nvc, ref total, ref msg);
                DataView view1 = table.DefaultView;
                view1.Sort = String.Format("{0} {1}", sortField, sortDirection);

                GridCFGGoods.DataSource = view1;
                GridCFGGoods.DataBind();
                GridCFGGoods.RecordCount = total;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }

        protected void GridCFGGoods_RowDoubleClick(object sender, FineUIPro.GridRowClickEventArgs e)
        {
            string seq = GridCFGGoods.Rows[e.RowIndex].Values[0].ToString();
            DataTable dt = DbHelperOra.Query(string.Format("select PZ.*,SP.GDNAME,decode(nvl(pz.iscfg,'Y'),'Y','Y','1','Y','N') ISCFGID from DOC_GOODSCFG PZ,DOC_GOODS SP where PZ.GDSEQ=SP.GDSEQ AND PZ.GDSEQ='{0}' AND PZ.DEPTID ='{1}'", seq, ddlDept.SelectedValue)).Tables[0];
            PubFunc.FormDataSet(FormConfig, dt.Rows[0]);
            if (DbHelperOra.Exists("SELECT 1 FROM SYS_DEPT WHERE CODE = '" + ddlDept.SelectedValue + "' AND TYPE IN('1','2')") && cbxTB.Checked)
            {
                trbHJCODE1.Enabled = true;
            }
            else
            {
                trbHJCODE1.Enabled = false;
            }
        }
        //数字验证
        private static bool number_test(NumberBox test)
        {
            int tmp;
            if (!int.TryParse(test.Text, out tmp))
            {
                return false;
            }
            if (tmp < 0)
            {
                return false;
            }
            return true;
        }
        //ymh 验证时间
        public static bool IsDate(string StrSource)
        {
            return Regex.IsMatch(StrSource, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (PubFunc.FormDataCheck(FormConfig).Length > 1) return;
            if (tbnZDKC.Text.Length > 0 && tbnZGKC.Text.Length > 0)
            {
                if (Convert.ToDecimal(tbnZDKC.Text) > Convert.ToDecimal(tbnZGKC.Text))
                {
                    Alert.Show("最低库存不能大于最高库存");
                    return;
                }
            }
            //填写定数数量，定数系数必须填写
            //if (nbxDSNUM.Text.Length > 0)
            //{
            //    if (NubNUM1.Text.Length < 1)
            //    {
            //        Alert.Show("定数数量不为0时,定数系数必须填写!");
            //        return;
            //    }
            //    if (Convert.ToDecimal(nbxDSNUM.Text) > 0 && Convert.ToDecimal(NubNUM1.Text) < 1)
            //    {
            //        Alert.Show("定数数量不为0时,定数系数必须填写!");
            //        return;
            //    }
            //}
            MyTable mtType = new MyTable("DOC_GOODSCFG");
            mtType.ColRow = PubFunc.FormDataHT(FormConfig);

            mtType.ColRow.Remove("GDNAME");
            string strISCFG = "";
            if (chkISCFGID.Checked)
            {
                strISCFG = "Y";
            }
            else
            {
                strISCFG = "N";
            }
            mtType.ColRow.Add("ISCFG", strISCFG);
            mtType.UpdateExec();
            //同步修改库存表中存在库存信息
            //if (DbHelperOra.Exists("SELECT 1 FROM SYS_DEPT WHERE CODE = '" + ddlDept.SelectedValue + "' AND TYPE IN('1','2')") && cbxTB.Checked)
            //{
            if (trbHJCODE1.Text.Trim().Length < 1)
            {
                Alert.Show("请维护库房商品【" + tbxGDSEQ.Text + "】的货位信息!", "操作提示", MessageBoxIcon.Warning);
                return;
            }
            if (cbxTB.Checked)
            {
                DbHelperOra.ExecuteSql("UPDATE DAT_GOODSSTOCK SET HWID = '" + trbHJCODE1.Text + "' WHERE GDSEQ = '" + tbxGDSEQ.Text + "' AND DEPTID = '" + ddlDept.SelectedValue + "'");

            }
            OperLog("商品配置", "修改科室【" + ddlDept.SelectedValue + "】商品【" + tbxGDSEQ.Text + "】");
            //}
            //else
            //{
            //    Alert.Show("这个是科室，直接把科室编码冻结");
            //}
            dataSearch();
            dataSearch("right");
            Alert.Show("配置信息保存成功！");
            //PubFunc.FormDataClear(FormConfig);
            //cbxTB.Checked = true;
        }
        //处理字段空值
        private decimal rtn_empt(NumberBox empt)
        {
            if (empt.Text.Trim().Length > 0)
            {
                decimal rtn = Math.Round(Convert.ToDecimal(empt.Text.Trim()), 4);
                return rtn;
            }
            return 0;
        }

        protected void ddlDept_SelectedIndexChanged(object sender, EventArgs e)
        {



            if (ddlDept.SelectedValue != null && !PubFunc.StrIsEmpty(ddlDept.SelectedValue))
            {
                //if (ddlDept.SelectedValue.Equals("01"))
                //{
                //    ExportBtn.Enabled = true;
                //}
                //else
                //{
                //    ExportBtn.Enabled = false;
                //}
                string type = DbHelperOra.GetSingle(string.Format("SELECT DECODE(TYPE,'1','Y','2','Y','N') KF_FLAG FROM SYS_DEPT WHERE CODE='{0}'", ddlDept.SelectedValue)).ToString();
                if (type == "Y")
                {
                    nbxDSNUM.Text = "";
                    NubNUM1.Text = "";
                    nbxDSNUM.Enabled = false;
                    NubNUM1.Enabled = false;
                }
                else
                {
                    //nbxDSNUM.Enabled = true;
                    //NubNUM1.Enabled = true;
                }
                dataSearch();
                dataSearch("right");
            }
        }

        protected void GridGoods_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            dataSearch();
        }

        protected void GridCFGGoods_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridCFGGoods.PageIndex = e.NewPageIndex;
            dataSearch("right");
        }

        protected void btnAddRight_Click(object sender, EventArgs e)
        {
            if (ddlDept.SelectedValue.Length > 0)
            {
                int[] selectArray = GridGoods.SelectedRowIndexArray;
                if (selectArray.Length > 0)
                {
                    List<CommandInfo> cmdList = new List<CommandInfo>();
                    MyTable myGoodsCFG = new MyTable("DOC_GOODSCFG");
                    for (int i = 0; i < selectArray.Length; i++)
                    {
                        Object OBJ = DbHelperOra.GetSingle(String.Format("SELECT DEPTID FROM DOC_GOODSCFG A,SYS_DEPT B WHERE A.DEPTID = B.CODE AND B.TYPE = '1' AND A.GDSEQ = '{0}'", GridGoods.Rows[selectArray[i]].Values[2].ToString()));
                        if ((OBJ ?? "").ToString().Length > 0)
                        {
                            Alert.Show(string.Format("商品【{0}】已配置到库房【{1}】！", GridGoods.Rows[selectArray[i]].Values[2].ToString(), OBJ), "消息提示", MessageBoxIcon.Warning);
                            return;
                        }
                        if (GridGoods.Rows[selectArray[i]].Values[8].ToString() != "Y"&& GridGoods.Rows[selectArray[i]].Values[8].ToString() != "T")
                        {
                            Alert.Show(string.Format("商品【{0}】状态信息错误！", GridGoods.Rows[selectArray[i]].Values[2].ToString()), "消息提示", MessageBoxIcon.Warning);
                            return;
                        }
                        myGoodsCFG.ColRow.Clear();
                        myGoodsCFG.ColRow.Add("GDSEQ", GridGoods.Rows[selectArray[i]].Values[0]);
                        myGoodsCFG.ColRow.Add("DEPTID", ddlDept.SelectedValue);
                        myGoodsCFG.ColRow.Add("ISCFG", "Y");
                        myGoodsCFG.ColRow.Add("NUM1", "0");
                        myGoodsCFG.ColRow.Add("NUM3", "0");
                        myGoodsCFG.ColRow.Add("DSNUM", "0");
                        if (DbHelperOra.Exists("SELECT 1 FROM SYS_DEPT WHERE CODE = '" + ddlDept.SelectedValue + "' AND TYPE IN('1','2')"))
                        {
                            trbHJCODE1.Enabled = true;
                        }
                        else
                        {
                            myGoodsCFG.ColRow.Add("HJCODE1", ddlDept.SelectedValue);
                            trbHJCODE1.Enabled = false;
                        }
                        cmdList.Add(myGoodsCFG.Insert());
                        OperLog("商品配置", "修改科室【" + ddlDept.SelectedValue + "】商品【" + GridGoods.Rows[selectArray[i]].Values[0] + "】");
                    }

                    DbHelperOra.ExecuteSqlTran(cmdList);
                    dataSearch();
                    dataSearch("right");
                }
                else
                {
                    Alert.Show("请选择要进行配置的商品信息！");
                    GridGoods.Focus();
                }
            }
            else
            {
                Alert.Show("请选择要进行配置的部门！");
                ddlDept.Focus();
            }
        }

        protected void btnAddLeft_Click(object sender, EventArgs e)
        {
            int[] selectArray = GridCFGGoods.SelectedRowIndexArray;
            if (selectArray.Length > 0)
            {
                List<CommandInfo> cmdList = new List<CommandInfo>();
                for (int i = 0; i < selectArray.Length; i++)
                {

                    string sql = "";
                    sql += string.Format("select sum(kcsl) kcsl from dat_goodsstock where gdseq='{0}' and deptid='{1}'", GridCFGGoods.Rows[selectArray[i]].Values[0].ToString(), ddlDept.SelectedValue);
                    DataTable dt=DbHelperOra.Query(sql).Tables[0];
                    if (dt.Rows[0]["KCSL"].ToString() != "")
                    {
                        if (int.Parse(dt.Rows[0]["KCSL"].ToString()) > 0)
                        {
                            Alert.Show("该商品在该科室有库存，不能取消配置");
                        }
                        else
                        {
                            cmdList.Add(new CommandInfo("delete DOC_GOODSCFG where GDSEQ='" + GridCFGGoods.Rows[selectArray[i]].Values[0].ToString() + "' and DEPTID='" + ddlDept.SelectedValue + "'", null));
                            OperLog("商品配置", "修改科室【" + ddlDept.SelectedValue + "】商品【" + GridCFGGoods.Rows[selectArray[i]].Values[0] + "】");
                        }
                    }
                    else
                    {
                        cmdList.Add(new CommandInfo("delete DOC_GOODSCFG where GDSEQ='" + GridCFGGoods.Rows[selectArray[i]].Values[0].ToString() + "' and DEPTID='" + ddlDept.SelectedValue + "'", null));
                        OperLog("商品配置", "修改科室【" + ddlDept.SelectedValue + "】商品【" + GridCFGGoods.Rows[selectArray[i]].Values[0] + "】");
                    }
                }
                DbHelperOra.ExecuteSqlTran(cmdList);
                dataSearch();
                dataSearch("right");
            }
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
                            case "CX":
                                strSearch += string.Format(" AND （GDNAME LIKE '%{0}%' OR GDSEQ LIKE '%{0}%' OR NAMEJC LIKE '%{0}%' OR BAR3 LIKE '%{0}%'）", condition);
                                break;
                            case "CATID":
                                strSearch += string.Format(" AND CATID0='{0}'", condition);
                                break;
                            case "RIGHT":
                                strSearch += string.Format(" AND GDSEQ IN (SELECT PZ.GDSEQ FROM DOC_GOODSCFG PZ WHERE PZ.DEPTID = '{0}')", condition);
                                break;
                            case "DEPTOUT":
                                strSearch += string.Format(" AND GDSEQ  IN (SELECT GDSEQ FROM DOC_GOODSCFG WHERE DEPTID = '{0}')", condition);
                                break;
                            case "LEFT":
                                strSearch += string.Format(" AND GDSEQ NOT IN (SELECT GDSEQ FROM DOC_GOODSCFG WHERE DEPTID = '{0}') order by GDSEQ,GDNAME", condition);
                                break;
                        }
                    }
                }
            }
            StringBuilder strSql = new StringBuilder(string.Format(@"select G.GDSEQ,
G.GDID,G.GDNAME,G.BARCODE,G.ZJM,G.YCODE,G.NAMEJC,G.NAMEEN,G.GDSPEC,G.GDMODE,G.STRUCT,G.BZHL,G.UNIT,G.FLAG,G.CATID,G.JX,G.YX,G.PIZNO,G.BAR1,G.BAR2,G.BAR3,G.DEPTID,
G.SUPPLIER,G.LOGINLABEL,G.PRODUCER,G.ZPBH,G.PPID,G.CDID,G.JXTAX,G.XXTAX,G.BHSJJ,G.HSJJ,G.LSJ,G.YBJ,G.HSID,G.HSJ,G.JHZQ,G.ZDKC,
G.HLKC,G.ZGKC,G.SPZT,G.DAYXS,G.MANAGER,G.INPER,G.INRQ,G.BEGRQ,G.ENDRQ,G.UPTRQ,G.UPTUSER,G.MEMO,G.DISABLEORG,G.VERSION,G.LENGTH,G.WIDTH,G.HEIGH,
G.WEIGHTGROSS,G.WEIGHTNET,G.VOLUME,G.ISLOT,G.ISJB,G.ISFZ,decode(G.ISGZ,'Y','是','否') ISGZ ,G.ISIN,G.ISJG,G.ISDM,G.ISCF,G.ISYNZJ,G.ISFLAG1,G.ISFLAG2,G.ISFLAG3,G.ISFLAG4,G.ISFLAG5,G.ISFLAG6,G.ISFLAG7,G.ISFLAG8,G.ISFLAG9,G.STR0,G.STR1,G.STR2,
G.STR3,G.STR4,G.STR5,G.STR6,G.STR7,G.STR8,G.STR9,G.NUM1,G.NUM2,G.NUM3,G.NUM4,G.NUM5,G.ISDELETE,G.UNIT_DABZ,G.UNIT_ZHONGBZ,G.BARCODE_DABZ,G.BARCODE_ZHONGBZ,
G.NUM_DABZ,G.NUM_ZHONGBZ,G.UNIT_ORDER,G.UNIT_SELL,G.HISCODE,G.HISNAME,G.CATID0,G.UPTTIME,G.KPYXQ,G.MJYXQ,G.CATUSER,F_GETHISINFO(G.GDNAME,'GDNAME') GETHISINFOGDNAME,
                                                       F_GETHISINFO(G.GDSPEC,'GDSPEC') GETHISINFOGDSPEC,D.NAME UNITNAME,S.NAME FLAG_CN,  
(SELECT DECODE( NVL(F.ISCFG,'Y'),'Y','正常','1','正常','停用') FROM DOC_GOODSCFG F WHERE G.GDSEQ = F.GDSEQ AND F.DEPTID = '{0}') ISCFG_CN,
(SELECT DECODE( NVL(F.IERP,'Y'),'Y','是','否') FROM DOC_GOODSCFG F WHERE G.GDSEQ = F.GDSEQ AND F.DEPTID = '{0}') IERP,
(SELECT DECODE( NVL(F.ISJF,'Y'),'Y','是','否') FROM DOC_GOODSCFG F WHERE G.GDSEQ = F.GDSEQ AND F.DEPTID = '{0}') ISJF,
                                                       (SELECT PZ.Hjcode1 FROM DOC_GOODSCFG PZ WHERE PZ.DEPTID = '{0}' and G.gdseq = pz.gdseq) Hjcode1
                                                        from  DOC_GOODS G,DOC_GOODSUNIT D,
                                                              (SELECT CODE, NAME FROM SYS_CODEVALUE WHERE TYPE = 'GOODS_STATUS') S 
                                                        where G.ISDELETE='N' AND G.FLAG=S.CODE(+) AND G.UNIT=D.CODE(+) AND G.FLAG IN('Y','T') ", ddlDept.SelectedValue));
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql.Append(strSearch);
            }
            //strSql.Append(" order by GDSEQ,GDNAME");
            return GetDataTable(pageNum, pageSize, strSql, ref total);
        }

        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            if (ddlDept.SelectedItem.Value.Length < 1) { Alert.Show("请先选择配置部门"); return; }
            dataSearch();
            dataSearch("right");
        }

        protected void cbxTB_CheckedChanged(object sender, CheckedEventArgs e)
        {
            //不允许修改选中状态
            //cbxTB.Checked = true;
        }

        protected void ExportBtn_Click(object sender, EventArgs e)
        {
            string strSearch = "";

            if (GridCFGGoods == null)
            {
                Alert.Show("没有数据,无法导出！");
                return;
            }
            string sql = @"select PZZ.DEPTID 管理部门,' '||G.gdseq 商品编码,g.gdname 商品名称,S.NAME 状态,g.gdspec 规格容量,D.NAME 单位,pzz.hjcode1 拣零货位  from DOC_GOODS G,DOC_GOODSUNIT D,doc_goodscfg pzz,
                                    (SELECT CODE, NAME FROM SYS_CODEVALUE WHERE TYPE = 'GOODS_STATUS') S 
                                    where G.ISDELETE='N' AND G.FLAG=S.CODE(+) AND G.UNIT=D.CODE(+)AND G.GDSEQ = PZZ.GDSEQ(+)";

            strSearch += string.Format("AND PZZ.DEPTID='{0}'", ddlDept.SelectedValue);
            sql += strSearch;

            DataTable dt = DbHelperOra.Query(sql).Tables[0];

            ExcelHelper.ExportByWeb(dt, "商品配置信息", "商品配置导出_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");

            ExportBtn.Enabled = true;

        }

        protected void gridGoodsCfg_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {

        }

        private bool CheckFileExt(string fileNameExt)
        {
            if (String.IsNullOrEmpty(fileNameExt))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void billSearch()
        {
            DataTable dtBill = DbHelperOra.Query("select * from TEMP_GOODSPZ").Tables[0];
            gridGoodsCfg.DataSource = dtBill;
            gridGoodsCfg.DataBind();
        }

        protected void gridGoodsCfg_PageIndexChange(object sender, GridPageEventArgs e)
        {
            gridGoodsCfg.PageIndex = e.NewPageIndex;
            billSearch();
        }

        protected void btnHWUpload_Click(object sender, EventArgs e)
        {
            DataTable dtBill = DbHelperOra.Query("select * from TEMP_GOODSPZ").Tables[0];
            List<CommandInfo> cmdList = new List<CommandInfo>();
            if (dtBill.Rows.Count == 0)
            {
                if (this.fuDocument.HasFile)
                {
                    string toFilePath = "~/ERPUpload/products/";

                    //获得文件扩展名
                    string fileNameExt = Path.GetExtension(this.fuDocument.FileName).ToLower();

                    if (!ValidateFileType(fileNameExt))
                    {
                        Alert.Show("无效的文件类型！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }

                    string myallname = fuDocument.ShortFileName;//ShortFileName表示整个导入文件的名称
                    string[] mystr = myallname.Split('.');
                    if ((!ValidateFileType(myallname))
                        || mystr[1].Equals("doc") || mystr[1].Equals("docx") || mystr[1].Equals("png") || mystr[1].Equals("jpg"))
                    {
                        Alert.Show("请选择excel文件导入！", "消息提示", MessageBoxIcon.Warning);
                        return;
                    }

                    //验证合法的文件
                    if (CheckFileExt(fileNameExt))
                    {
                        //生成将要保存的随机文件名
                        string fileName = this.fuDocument.FileName.Substring(0, this.fuDocument.FileName.IndexOf(".")) + DateTime.Now.ToString("yyyyMMddHHmmss") + fileNameExt;

                        //按日期归类保存
                        string datePath = DateTime.Now.ToString("yyyyMM") + "/" + DateTime.Now.ToString("dd") + "/";
                        toFilePath += datePath;

                        //获得要保存的文件路径
                        string DownloadUrl = toFilePath + fileName;
                        //物理完整路径                    
                        string toFileFullPath = HttpContext.Current.Server.MapPath(toFilePath);

                        //检查是否有该路径,没有就创建
                        if (!Directory.Exists(toFileFullPath))
                        {
                            Directory.CreateDirectory(toFileFullPath);
                        }

                        //将要保存的完整物理文件名                
                        string serverFileName = toFileFullPath + fileName;

                        //获取保存的excel路径
                        this.fuDocument.SaveAs(serverFileName);

                        if (File.Exists(serverFileName))
                        {
                            DataTable table = new DataTable();

                            if (fileNameExt == ".xlsx")
                            {
                                table = ExcelHelper.ImportExcelxtoDt(serverFileName, 0, 1); //导入excel2007
                            }
                            else
                            {
                                table = ExcelHelper.ImportExceltoDt(serverFileName, 0, 1);//导入excel2003
                            }

                            string str = "";

                            for (int i = 0; i < table.Rows.Count; i++)
                            {
                                int count = Convert.ToInt32(DbHelperOra.GetSingle(string.Format("select COUNT(1) FROM DOC_HWZD where HWID='{0}'", table.Rows[i]["拣零货位"].ToString())));
                                if (count <= 0)
                                {
                                    //gDT.Rows[i]["STR1"] = "Y";
                                    int j = i + 1;
                                    str = str + j + ",";
                                }
                            }
                            if (str.Length > 0)
                            {
                                Alert.Show("拣零货位在" + str + "行发生了错误，请先确认货位是否和配置的货位编号匹配！");
                                // Alert.Show(table.Rows[i]["HJCODE1"].ToString());
                                return;
                            }
                            else
                            {
                                //首先清空临时表
                                DbHelperOra.Query("TRUNCATE TABLE TEMP_GOODSPZ");
                                if (table != null && table.Rows.Count > 0)
                                {
                                    string sql = @"INSERT INTO TEMP_GOODSPZ(DEPTID,GDSEQ,GDNAME,FLAG,GDSPEC,UNIT,HJCODE1)
                                                          VALUES(:DEPTID,:GDSEQ,:GDNAME,:FLAG,:GDSPEC,:UNIT,:HJCODE1)";
                                    OracleConnection con = new OracleConnection(DbHelperOra.connectionString);
                                    OracleDataAdapter da = new OracleDataAdapter(sql, con);
                                    //在批量添加数据前的准备工作
                                    da.InsertCommand = new OracleCommand(sql, con);
                                    OracleParameter param = new OracleParameter();
                                    param = da.InsertCommand.Parameters.Add(new OracleParameter("DEPTID", OracleDbType.Varchar2, 15));
                                    param.SourceVersion = DataRowVersion.Current;
                                    param.SourceColumn = "管理部门";

                                    param = da.InsertCommand.Parameters.Add(new OracleParameter("GDSEQ", OracleDbType.Varchar2, 100));
                                    param.SourceVersion = DataRowVersion.Current;
                                    param.SourceColumn = "商品编码";

                                    param = da.InsertCommand.Parameters.Add(new OracleParameter("GDNAME", OracleDbType.Varchar2, 100));
                                    param.SourceVersion = DataRowVersion.Current;
                                    param.SourceColumn = "商品名称";

                                    param = da.InsertCommand.Parameters.Add(new OracleParameter("FLAG", OracleDbType.Char, 10));
                                    param.SourceVersion = DataRowVersion.Current;
                                    param.SourceColumn = "状态";

                                    param = da.InsertCommand.Parameters.Add(new OracleParameter("GDSPEC", OracleDbType.Varchar2, 100));
                                    param.SourceVersion = DataRowVersion.Current;
                                    param.SourceColumn = "规格容量";

                                    param = da.InsertCommand.Parameters.Add(new OracleParameter("UNIT", OracleDbType.Varchar2, 20));
                                    param.SourceVersion = DataRowVersion.Current;
                                    param.SourceColumn = "单位";

                                    param = da.InsertCommand.Parameters.Add(new OracleParameter("HJCODE1", OracleDbType.Varchar2, 50));
                                    param.SourceVersion = DataRowVersion.Current;
                                    param.SourceColumn = "拣零货位";

                                    //批量添加数据
                                    try
                                    {
                                        con.Open();
                                        da.Update(table);
                                        DataTable updateTable = table.Copy();
                                        DataTable gDT = DbHelperOra.Query("select * from TEMP_GOODSPZ").Tables[0];
                                        gridGoodsCfg.DataSource = gDT;
                                        gridGoodsCfg.DataBind();
                                        Alert.Show("数据导入成功,共导入[" + gDT.Rows.Count.ToString() + "]条数据！", "消息提示", MessageBoxIcon.Information);
                                    }
                                    catch (Exception ex)
                                    {
                                        Alert.Show("数据库错误：" + ex.Message.ToString(), "异常信息", MessageBoxIcon.Warning);
                                    }
                                    finally
                                    {
                                        con.Close();
                                    }
                                }
                            }
                            File.Delete(serverFileName);
                        }
                    }
                }
                else
                {
                    Alert.Show("请选择excel文件！");
                }
            }
            else
            {
                Alert.Show("有数据，导入前请先清除原始数据！", "提示", MessageBoxIcon.Information);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            int count = DbHelperOra.ExecuteSql(@"UPDATE DOC_GOODSCFG TA
   SET HJCODE1 =
       (SELECT HJCODE1
          FROM TEMP_GOODSPZ TB
         WHERE TA.GDSEQ = tb.GDSEQ
           and TA.deptid = tb.deptid)
 where ((TA.GDSEQ, TA.DEPTID) IN (SELECT GDSEQ, DEPTID FROM TEMP_GOODSPZ))
 and (TA.DEPTID in ( select code from sys_dept where type='3'  ))");
            if (count > 0)
            {
                Alert.Show("更新成功！", "提示", MessageBoxIcon.Information);
                billSearch();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            int count = DbHelperOra.ExecuteSql("delete from TEMP_GOODSPZ");
            if (count > 0)
            {
                Alert.Show("删除成功！", "提示", MessageBoxIcon.Information);
                billSearch();
            }
            fuDocument.Reset();
        }

        private void HwidQuery()
        {
            string code = tbxGDSEQ.Text;
            string dept = ddlDept.SelectedValue;
            string sql = @"SELECT HWID,
                               JWBH,
                               KFBH,
                               KB,
                               f_getusername(MANAGER) MANAGER,
                               HWZT,
                               DZBQH,      
                               F_GETDEPTNAME(a.KFBH) KFBHNAME,
                               (select NAME
                                  FROM SYS_CODEVALUE c
                                 WHERE c.TYPE = 'DEPOT_AREA'
                                   and c.code = a.KB) KBNAME,
                                QYBH,PAI,LIE,CENG
                          FROM DOC_HWZD A   where kfbh = '{0}' and hwzt = 'Y'";
            if (!string.IsNullOrWhiteSpace(trbSearch1.Text.Trim()))
            {
                sql += string.Format(" and (HWID like '%{0}%' or JWBH like '%{0}%' or QYBH like '%{0}%')", trbSearch1.Text.Trim());
            }

            int total = 0;
            DataTable dt_huowei = PubFunc.DbGetPage(HwList.PageIndex, HwList.PageSize, string.Format(sql, dept), ref total);
            HwList.RecordCount = total;
            HwList.DataSource = dt_huowei;
            HwList.DataBind();
        }

        protected void trbHJCODE1_TriggerClick(object sender, EventArgs e)
        {
            //string sup = docSUPID.SelectedValue;
            if (DbHelperOra.Exists("SELECT 1 FROM SYS_DEPT WHERE CODE = '" + ddlDept.SelectedValue + "' AND TYPE IN('1','2')") && cbxTB.Checked)
            {
                if (!string.IsNullOrWhiteSpace(tbxGDSEQ.Text))
                {
                    Window1.Hidden = false;
                    HwidQuery();
                }
                else
                {
                    Alert.Show("当前未选择商品，无法进行【拆零货位】配置操作。", "操作提示", MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                Window1.Hidden = true;
            }
        }

        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            int[] selections = HwList.SelectedRowIndexArray;
            foreach (int rowIndex in selections)
            {
                trbHJCODE1.Text = HwList.DataKeys[rowIndex][0].ToString();
            }
            Window1.Hidden = true;
        }
        protected void btnCopy_Click(object sender, EventArgs e)
        {
            winCopy.Hidden = false;
        }
        protected void HwList_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            trbHJCODE1.Text = HwList.DataKeys[e.RowIndex][0].ToString();
            Window1.Hidden = true;
        }

        protected void HwList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            HwList.PageIndex = e.NewPageIndex;
            HwidQuery();
        }

        protected void HwList_Sort(object sender, GridSortEventArgs e)
        {

        }

        protected void trbSearch1_TriggerClick(object sender, EventArgs e)
        {
            HwidQuery();
        }

        protected void btnGoodsConfig_Click(object sender, EventArgs e)
        {
            int[] deptArray = GridDepartment.SelectedRowIndexArray;
            string strGoods = "";
            string strGoodsTR = "";
            if (deptArray.Length > 0)
            {
                int[] selectArray = GridGoodsConfig.SelectedRowIndexArray;
                //if (selectArray.Length > 0)
                //{
                    List<CommandInfo> cmdList = new List<CommandInfo>();
                    MyTable myGoodsCFG = new MyTable("DOC_GOODSCFG");
                    for (int j = 0; j < selectArray.Length; j++)
                    {
                        cmdList.Add(new CommandInfo(string.Format("delete doc_goodscfg where gdseq='{0}'", GridGoodsConfig.Rows[selectArray[j]].Values[1].ToString()), null));

                    }
                    DataTable dtcfggrid = DbHelperOra.QueryForTable(string.Format(@"SELECT DEPTID FROM DOC_GOODSCFG WHERE GDSEQ='{0}'", GridGoodsConfig.Rows[GridGoodsConfig.SelectedRowIndex].Values[1].ToString()));
                    bool isexist = false;    
                if (dtcfggrid.Rows.Count > 0)
                    {
                        DataTable canceldt = PubFunc.GridDataGet(GridDepartment, GridDepartment.SelectedRowIndexArray);
                        canceldt.Columns.Remove("DEPTNAME");
                        canceldt.Columns.Remove("DEPTTYPE");
                        foreach (DataRow dr in dtcfggrid.Rows)
                        {
                            foreach (DataRow drs in canceldt.Rows)
                            {
                                if (dr["DEPTID"].ToString().Equals(drs["CODE"].ToString()))
                                {
                                    isexist = true;
                                    break;
                                }
                                else
                                {
                                    isexist = false;
                                }
                            }
                            if (!isexist)
                            {
                                DataTable dtcfgdb = DbHelperOra.QueryForTable(string.Format(@"SELECT DGC.DEPTID,SD.NAME DEPTNAME FROM DOC_GOODSCFG DGC ,SYS_DEPT SD WHERE DGC.DEPTID=SD.CODE AND (SELECT f_getkcgdseq(SD.CODE,DGC.GDSEQ,'') FROM DUAL)>0 AND SD.CODE='{0}' AND DGC.GDSEQ='{1}'", dr["DEPTID"].ToString(), GridGoodsConfig.Rows[GridGoodsConfig.SelectedRowIndex].Values[1].ToString()));
                                if (dtcfgdb.Rows.Count > 0)
                                {
                                    foreach (DataRow dre in dtcfgdb.Rows)
                                    {
                                        Alert.Show("科室【" + dre["DEPTNAME"].ToString() + "】中此商品尚有库存，不能取消配置");
                                        return;
                                    }

                                }
                            }
                        }
                        
                      
                      

                        for (int i = 0; i < deptArray.Length; i++)
                        {
                            string strDEPT = GridDepartment.Rows[deptArray[i]].Values[0].ToString();
                            string strTYPE = GridDepartment.Rows[deptArray[i]].Values[2].ToString();
                            string strInsertSql = "";
                            for (int j = 0; j < selectArray.Length; j++)
                            {
                                
                                strGoods = "'" + GridGoodsConfig.Rows[selectArray[j]].Values[1].ToString() + "',";
                                strGoodsTR = strGoods.TrimEnd(',');
                                if (strTYPE.Equals("3"))
                                {
                                    strInsertSql = string.Format(@"INSERT INTO doc_goodscfg
  (GDSEQ, DEPTID, HJCODE1) VALUES({0}, '{1}', '{2}')", strGoodsTR, strDEPT, strDEPT);
                                }
                                else
                                {
                                    strInsertSql = string.Format(@"INSERT INTO doc_goodscfg
  (GDSEQ, DEPTID, HJCODE1) VALUES({0}, '{1}', '{2}')", strGoodsTR, strDEPT, "");
                                }



                                cmdList.Add(new CommandInfo(strInsertSql, null));

                            }

                            #region


                            //string strSqls = String.Format("SELECT 1 FROM DOC_GOODSCFG A,SYS_DEPT B WHERE A.DEPTID = B.CODE AND B.TYPE = '1' AND A.GDSEQ = '{0}' AND  DEPTID = '{1}'", GridGoodsConfig.Rows[selectArray[j]].Values[1].ToString(), GridDepartment.Rows[selectArray[i]].Values[0].ToString());
                            //    if ( !DbHelperOra.Exists(strSqls))
                            //    {
                            //        //Alert.Show(string.Format("商品【{0}】已配置到库房【{1}】！", GridGoods.Rows[selectArray[i]].Values[2].ToString(), OBJ), "消息提示", MessageBoxIcon.Warning);
                            //        //return;

                            //        myGoodsCFG.ColRow.Clear();
                            //        myGoodsCFG.ColRow.Add("GDSEQ", GridGoodsConfig.Rows[selectArray[j]].Values[1]);
                            //        myGoodsCFG.ColRow.Add("DEPTID", GridDepartment.Rows[selectArray[j]].Values[0]);
                            //        myGoodsCFG.ColRow.Add("ISCFG", "Y");
                            //        myGoodsCFG.ColRow.Add("NUM1", "0");
                            //        myGoodsCFG.ColRow.Add("NUM3", "0");
                            //        myGoodsCFG.ColRow.Add("DSNUM", "0");
                            //        if (DbHelperOra.Exists("SELECT 1 FROM SYS_DEPT WHERE CODE = '" + GridDepartment.Rows[selectArray[i]].Values[0].ToString() + "' AND TYPE IN('1','2')"))
                            //        {
                            //            trbHJCODE1.Enabled = true;
                            //        }
                            //        else
                            //        {
                            //            myGoodsCFG.ColRow.Add("HJCODE1", GridDepartment.Rows[selectArray[i]].Values[0].ToString());
                            //            trbHJCODE1.Enabled = false;
                            //        }
                            //        cmdList.Add(myGoodsCFG.Insert());
                            //        OperLog("商品配置", "修改科室【" + GridDepartment.Rows[selectArray[i]].Values[0] + "】商品【" + GridGoodsConfig.Rows[selectArray[j]].Values[1] + "】");
                            //    }
                            //if (GridGoods.Rows[selectArray[i]].Values[8].ToString() != "Y")
                            //{
                            //    //Alert.Show(string.Format("商品【{0}】状态信息错误！", GridGoods.Rows[selectArray[i]].Values[2].ToString()), "消息提示", MessageBoxIcon.Warning);
                            //    return;
                            //}
                            #endregion


                        }
                    }
                    if (DbHelperOra.ExecuteSqlTran(cmdList))
                    {
                        Alert.Show("保存成功");
                    }
                //}
                //else
                //{
                //    Alert.Show("请选择要进行配置的商品信息！");
                //    GridGoods.Focus();
                //}
            }
            else
            {
                Alert.Show("请选择要进行配置的部门！");
                ddlDept.Focus();
            }
        }

        protected void GridGoodsConfig_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridGoodsConfig.PageIndex = e.NewPageIndex;
            btnGoodScreen_Click(null,null);
            
        }

        protected void GridGoodsConfig_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            //JObject defaultObj = Doc.GetJObject(GridGoodsConfig, e.RowID);
            string strGDSEQ = GridGoodsConfig.Rows[e.RowIndex].DataKeys[0].ToString();
            //string strGDSEQ = (defaultObj["GDSEQ"] ?? "").ToString();

            string dptSql = string.Format(@"SELECT ROWNO,CODE,NAME FROM (
       SELECT GDSEQ, ROWNUM ROWNO,B.CODE,B.NAME FROM DOC_GOODSCFG A, SYS_DEPT B WHERE A.DEPTID = B.CODE ORDER BY CODE
       ) WHERE GDSEQ = '{0}'", strGDSEQ);
            DataTable dt = DbHelperOra.Query(dptSql).Tables[0];
            DataTable dtDEPT = DbHelperOra.Query(strDEPT).Tables[0];

            List<int> sel = new List<int>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < dtDEPT.Rows.Count; i++)
                    {
                        if (dtDEPT.Rows[i]["CODE"].ToString() == dr["CODE"].ToString())
                        {
                            sel.Add(i);
                        }
                    }
                }
            }
            GridDepartment.SelectedRowIndexArray = sel.ToArray();
        }

        protected void btnGoodScreen_Click(object sender, EventArgs e)
        {
            int total = 0;
            string msg = "";
            NameValueCollection nvc = new NameValueCollection();
            if (trbGoodsSearch.Text.Length > 0) nvc.Add("CX", trbGoodsSearch.Text);
            GridGoodsConfig.DataSource = GetGoodsList(GridGoodsConfig.PageIndex, GridGoodsConfig.PageSize, nvc, ref total, ref msg);
            GridGoodsConfig.DataBind();
            GridGoodsConfig.RecordCount = total;

        }
        private void dataSearchConfig()
        {

        }

        protected void btnDeptScreen_Click(object sender, EventArgs e)
        {
            
        }

        protected void GridGoodsConfig_RowClick(object sender, GridRowClickEventArgs e)
        {
            //GridDepartment.SelectedRowIndexArray = new int[] { };
        }

        protected void btnClearDept_Click(object sender, EventArgs e)
        {
            GridDepartment.SelectedRowIndexArray = new int[] { };
        }

        protected void btnClearGoods_Click(object sender, EventArgs e)
        {
            GridGoodsConfig.SelectedRowIndexArray = new int[] { };
        }

        protected void GridCFGGoods_Sort(object sender, GridSortEventArgs e)
        {
            dataSearch("right");
        }
        protected void btnCopyClosePost_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ddlDeptFrom.SelectedValue) && string.IsNullOrWhiteSpace(ddlDeptFrom.SelectedValue))
            {
                Alert.Show("请选择科室信息！！！", "异常提醒", MessageBoxIcon.Warning);
                return;
            }
            List<CommandInfo> cmdlist = new List<CommandInfo>();
            foreach (ListItem lst in ddlDeptTo.SelectedItemArray)
            {

                string sql = @"INSERT INTO DOC_GOODSCFG
                                                (GDSEQ,
                                                   DEPTID,
                                                   ISCFG,
                                                   SPLB,
                                                   BEGRQ,
                                                   ENDRQ,
                                                   DAYSL,
                                                   HLKC,
                                                   HJCODE1,
                                                   HJCODE2,
                                                   HJCODE3,
                                                   ISAUTO,
                                                   ISDH,
                                                   ZDKC,
                                                   ZGKC,
                                                   DHXS,
                                                   NUM1,
                                                   NUM2,
                                                   NUM3,
                                                   MEMO,
                                                   DSNUM,
                                                   DSPOOL,
                                                   DHDAY,
                                                   ISJF,
                                                   IERP)
                                              SELECT GDSEQ,
                                                     '{1}',
                                                     ISCFG,
                                                     SPLB,
                                                     BEGRQ,
                                                     ENDRQ,
                                                     DAYSL,
                                                     HLKC,
                                                     '{1}',
                                                     HJCODE2,
                                                     HJCODE3,
                                                     ISAUTO,
                                                     ISDH,
                                                     ZDKC,
                                                     ZGKC,
                                                     DHXS,
                                                     NUM1,
                                                     NUM2,
                                                     NUM3,
                                                     MEMO,
                                                     DSNUM,
                                                     DSPOOL,
                                                     DHDAY,
                                                     ISJF,
                                                     IERP
                                                FROM DOC_GOODSCFG
                                               WHERE DEPTID = '{0}' AND GDSEQ NOT IN
                                                     (SELECT GDSEQ FROM DOC_GOODSCFG WHERE DEPTID = '{1}')";
                sql = string.Format(sql, ddlDeptFrom.SelectedValue, lst.Value);
                cmdlist.Add(new CommandInfo(sql,null));
            }
            if (DbHelperOra.ExecuteSqlTran(cmdlist))
            {
                Alert.Show("科室配置信息复制成功！", "消息提示", MessageBoxIcon.Warning);
                winCopy.Hidden = true;
            }
        }
    }
}
﻿using XTBase;
using FineUIPro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;

namespace ERPProject.ERPApply
{
    public partial class ConstantSetup : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInit();
                //ButtonHidden(bntSave);
            }
        }

        private void DataInit()
        {
            PubFunc.DdlDataGet(ddlGoodsType, "DDL_GOODS_TYPE");
            PubFunc.DdlDataGet(ddlDEPTID, "DDL_SYS_DEPTDEF");
            //DepartmentBind.BindDDL("DDL_SYS_DEPTHASATH", UserAction.UserID, ddlDEPTID);

            string strSql = @"
                        SELECT '--请选择--' NAME,'%' CODE,0 TreeLevel,1 islast  FROM dual
                              union all
                              select '【'||code||'】'||name name,code,(class-1) TreeLevel,decode(islast,'Y',1,0) islast
                                          from sys_category
                         ORDER BY code ";
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
        }

        private void DataSearch()
        {
            string strSearch = @"select  cfg.deptid,
       G.GDSEQ,
       G.GDNAME,
       G.GDSPEC,
       G.ZDKC,
       G.ZGKC,
       G.DAYXS,
       a.name UNITNAME,
       G.CATID0,
       b.name UNITNAME_ORDER,gt.name as typename,
       '[' || nvl(dept.code, '商品未配置') || ']' || dept.name as deptname,
       '[' || cat.code || ']' || cat.name as catname,
       sup.supname PRODUCER,
       cfg.num2 as dsls,
       cfg.num3 as DAISHOU,
       cfg.num1 as num1,
       cfg.dsnum,
       cfg.num1 as dsHanL
  from DOC_GOODS     G,
       doc_goodsunit a,
       doc_goodsunit b,
       doc_goodscfg  cfg,
       doc_goodstype gt,
       sys_dept      dept,
       sys_category  cat,
       doc_supplier  sup
 where g.ISDELETE = 'N'
   and g.producer = sup.supid(+)
   and G.UNIT = a.code(+)
   and g.catid0 = gt.code(+)
   and cfg.deptid = dept.code(+)
   and g.catid = cat.code(+)
   and dept.type <> '1'
   and decode(G.UNIT_ORDER,
              'D',
              NVL(UNIT_DABZ, G.UNIT),
              'Z',
              NVL(G.UNIT_ZHONGBZ, G.UNIT),
              G.UNIT) = b.code(+)
   and g.gdseq = cfg.gdseq(+)
                                      and cfg.deptid like '{0}'
                                      and g.catid0 like '{1}'
                                      and g.catid like '{2}'
                                      and (g.gdseq like '{3}' or g.gdname like '{3}')  ";
            //strSearch += string.Format(" AND cfg.deptid in( select code FROM SYS_DEPT where type <>'1' and  F_CHK_DATARANGE(CODE, '{0}') = 'Y' )", UserAction.UserID);
            StringBuilder strSql = new StringBuilder("");
            String deptId = "%";
            String catId0 = "%";
            String catId = "%";
            String gdseq = "%";
            if (!string.IsNullOrWhiteSpace(ddlDEPTID.SelectedValue))
            {
                deptId = ddlDEPTID.SelectedValue;
            }
            if (!string.IsNullOrWhiteSpace(ddlGoodsType.SelectedValue))
            {
                catId0 = ddlGoodsType.SelectedValue;
            }
            if (!string.IsNullOrWhiteSpace(ddlCATID.SelectedValue))
            {
                catId = ddlCATID.SelectedValue;
            }
            if (!string.IsNullOrWhiteSpace(tbxGoodsName.Text))
            {
                gdseq = "%" + tbxGoodsName.Text + "%";
            }
            if (!string.IsNullOrWhiteSpace(strSearch))
            {
                strSql.Append(string.Format(strSearch, deptId, catId0, catId, gdseq));
            }

            //strSql.Append(" order by cfg.deptid(+),g.GDSEQ,g.GDNAME");
            int total = 0;
            GridGoods.DataSource = GetDataTable(GridGoods.PageIndex, GridGoods.PageSize, strSql, ref total);
            GridGoods.RecordCount = total;
            GridGoods.DataBind();
        }

        protected void GridGoods_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            DataSearch();
        }

        protected void bntSearch_Click(object sender, EventArgs e)
        {
            DataSearch();
        }

        protected void bntSave_Click(object sender, EventArgs e)
        {
            //JObject defaultObj = Doc.GetJObject(GridGoods, e.RowID);
            String querySql = "select COUNT(1) from doc_goodscfg WHERE GDSEQ='{0}' AND DEPTID='{1}'";
            String insSql = "insert into doc_goodscfg (GDSEQ,DEPTID,NUM1,DSNUM,ISCFG,HJCODE1) values ('{0}','{1}',{2},{3},'Y','{1}')";
            String uptSql = "update doc_goodscfg set NUM1={2},DSNUM={3},ISCFG='Y' where GDSEQ='{0}' and DEPTID='{1}'";
            List<CommandInfo> lci = new List<CommandInfo>();

            JArray ja = GridGoods.GetModifiedData();
            

            foreach (JToken jt in ja)
            {
                //取ID 
                String res = (jt.SelectToken(string.Format("$.{0}", "index")) ?? "").ToString();
                String rowid = (jt.SelectToken(string.Format("$.{0}", "id")) ?? "").ToString();
                JObject defaultObj = Doc.GetJObject(GridGoods, rowid);
               
                object[] keys = GridGoods.DataKeys[Convert.ToInt16(res)];
                int num1 = Convert.ToInt32(defaultObj["DSHANL"]);
                int dsnum = Convert.ToInt32(defaultObj["DSNUM"]);
                String gdseq = keys[0].ToString();
                String deptId = "";

                if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DOC_GOODS WHERE GDSEQ = '{0}' AND ISGZ='Y'", gdseq)))
                {
                    Alert.Show(string.Format("商品【{0}】为高值商品，不允许设置为定数", keys[4].ToString()), "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                if (DbHelperOra.Exists(string.Format("SELECT 1 FROM DOC_GOODS WHERE GDSEQ = '{0}' AND ISFLAG3='Y'", gdseq)))
                {
                    Alert.Show(string.Format("商品【{0}】为直送商品，不允许设置为定数", keys[4].ToString()), "消息提示", MessageBoxIcon.Warning);
                    return;
                }
                if (!string.IsNullOrWhiteSpace((String)keys[1]))
                {
                    deptId = keys[1].ToString();
                }
                else if (!string.IsNullOrWhiteSpace(ddlDEPTID.SelectedValue))
                {
                    deptId = ddlDEPTID.SelectedValue;
                }
                else
                {
                    Alert.Show("新增配置需要选择库房！");
                    return;
                }
                //if (string.IsNullOrEmpty(keys[3].ToString()))
                //{
                //    Alert.Show("定数数量不能为空！");
                //    return;
                //}
                //if (string.IsNullOrEmpty(keys[2].ToString()))
                //{
                //    Alert.Show("定数含量不能为空！");
                //    return;
                //}
                
                //int num1 = Convert.ToInt32(keys[2]);
                //int dsnum = Convert.ToInt32(keys[3]);
                //JArray theJa = JArray.FromObject(jt);
                //JObject jo = JObject.FromObject(theJa[2]);
                //if (!string.IsNullOrWhiteSpace(jo.Value<String>("DSNUM")))
                //{
                //    dsnum = Convert.ToInt32(jo.Value<String>("DSNUM"));

                //}
                //if (!string.IsNullOrWhiteSpace(jo.Value<String>("DSHANL")))
                //{
                //    num1 = Convert.ToInt32(jo.Value<String>("DSHANL"));
                //}


                if (DbHelperOra.Exists(String.Format(querySql, gdseq, deptId)))
                {
                    lci.Add(new CommandInfo(String.Format(uptSql, gdseq, deptId, num1, dsnum), null));
                }
                else
                {
                    lci.Add(new CommandInfo(String.Format(insSql, gdseq, deptId, num1, dsnum), null));
                }
                OperLog("定数设置", "修改商品【" + gdseq + "】");
            }
            if (lci.Count==0)
            {
                Alert.Show("信息无需保存。");
            }
            else
            {
                DbHelperOra.ExecuteSqlTran(lci);
                Alert.Show("信息保存成功。");

            }
           
            DataSearch();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            StringBuilder strbuilder = new StringBuilder();
            strbuilder.Append(" SELECT '[' || nvl(dept.code, '商品未配置') || ']' || dept.name 库房,  gt.name 大类, '[' || cat.code || ']' || cat.name 商品分类,");
            strbuilder.Append(" G.GDSEQ 商品编码,  G.GDNAME 商品名称,  G.GDSPEC 规格容量,  F_GETPRODUCERNAME(G.PRODUCER) 生产厂家,cfg.dsnum 定数数量, cfg.num1  定数含量, a.name 单位  FROM  DOC_GOODS G,");
            strbuilder.Append("  doc_goodsunit a, doc_goodsunit b,doc_goodscfg  cfg, doc_goodstype gt,   (select * from sys_dept where type<>'1') dept,sys_category  cat ");
            strbuilder.Append("  where g.ISDELETE = 'N' and G.UNIT=a.code(+) and g.catid0 = gt.code(+)  and cfg.deptid = dept.code(+)  and g.catid = cat.code(+) ");
            strbuilder.Append("and decode(G.UNIT_ORDER,'D',NVL(UNIT_DABZ,G.UNIT),'Z',NVL(G.UNIT_ZHONGBZ,G.UNIT),G.UNIT) = b.code(+)");
            strbuilder.Append("and g.gdseq = cfg.gdseq (+)  and  cfg.deptid(+) like '{0}'  and g.catid0 like '{1}' and g.catid like '{2}' and (g.gdseq like '{3}' or g.gdname like '{3}') ");


            StringBuilder strSql = new StringBuilder("");
            String deptId = "%";
            String catId0 = "%";
            String catId = "%";
            String gdseq = "%";
            if (!string.IsNullOrWhiteSpace(ddlDEPTID.SelectedValue))
            {
                deptId = ddlDEPTID.SelectedValue;
            }
            if (!string.IsNullOrWhiteSpace(ddlGoodsType.SelectedValue))
            {
                catId0 = ddlGoodsType.SelectedValue;
            }
            if (!string.IsNullOrWhiteSpace(ddlCATID.SelectedValue))
            {
                catId = ddlCATID.SelectedValue;
            }
            if (!string.IsNullOrWhiteSpace(tbxGoodsName.Text))
            {
                gdseq = "%" + tbxGoodsName.Text + "%";
            }
            if (!string.IsNullOrWhiteSpace(strbuilder.ToString()))
            {
                strSql.Append(string.Format(strbuilder.ToString(), deptId, catId0, catId, gdseq));
            }

            strSql.Append(" order by cfg.deptid(+),g.GDSEQ,g.GDNAME");

            DataTable table = DbHelperOra.Query(strSql.ToString()).Tables[0];

            try
            {
                XTBase.Utilities.ExcelHelper.ExportByWeb(table, "定数批量设置", string.Format("定数批量设置_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
            }
            catch (Exception ex)
            {

                Alert.Show(ex.Message);
            }
        }

        protected void GridGoods_Sort(object sender, GridSortEventArgs e)
        {
            GridGoods.SortDirection = e.SortDirection;
            GridGoods.SortField = e.SortField;

            DataTable table = PubFunc.GridDataGet(GridGoods);
            DataView view1 = table.DefaultView;
            view1.Sort = String.Format("{0} {1}", GridGoods.SortField, GridGoods.SortDirection);
            GridGoods.DataSource = view1;
            GridGoods.DataBind();
        }
    }
}
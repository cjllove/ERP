using FineUIPro;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERPProject.ERPEntrust
{
    public partial class GoodsSupplier : PageBase
    {
        //在dataSearch函数、btnSave_Click函数中一起使用
        StringBuilder builder = new StringBuilder();
        private static DataTable dtData;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataInt();


                dataSearch();
            }
        }
        private void DataInt()
        {
            //PubFunc.DdlDataGet("DDL_UNIT", ddlUNIT, ddlUNIT_DABZ, ddlUNIT_ZHONGBZ);

        }
        public string getOper()
        {
            return Request.QueryString["oper"].ToString();
        }
        private void dataSearch()
        {

            //builder.Append("SELECT TA.GDSEQ,TA.GDNAME,TA.GDSPEC,TB.NAME CATID0NAME,F_GETCATNAME(TA.CATID) CATIDNAME,f_getunitname(TA.UNIT) UNITNAME,");
            //builder.Append("f_getproducername(TA.PRODUCER) producername,f_getsupname(TC.SUPID) SUPNAME,'请选择新供应商' NEWSUPNAME,ta.supplier newsupid,TA.PIZNO,(CASEWHEN TA.FLAG = 'Y' THEN");
            //builder.Append("'正常'when TA.flag = 'T' THEN'停购'when TA.flag = 'S' THEN'停销'WHEN TA.FLAG = 'N' THEN'新品'when TA.flag = 'E' THEN");
            //builder.Append("'淘汰'END) FLAGFROM DOC_GOODS TA, DOC_GOODSTYPE TB, DOC_GOODSSUP TC WHERE TA.CATID0 = TB.CODE AND TA.GDSEQ = TC.GDSEQ(+)");

           
            builder.Append(@"SELECT TA.GDSEQ,  TA.GDNAME, TA.GDSPEC, TB.NAME CATID0NAME, F_GETCATNAME(TA.CATID) CATIDNAME,  
            f_getunitname(TA.UNIT) UNITNAME, f_getproducername(TA.PRODUCER) producername, f_getsupname(TC.SUPID) SUPNAME,  '请选择新供应商' NEWSUPNAME, ta.supplier newsupid, TA.PIZNO,(CASE WHEN TA.FLAG = 'Y' THEN '正常'when TA.flag = 'T' THEN  '停购' when TA.flag = 'S' THEN   '停销' WHEN TA.FLAG = 'N' THEN  '新品'   when TA.flag = 'E' THEN  '淘汰'  END )FLAG FROM DOC_GOODS TA ,DOC_GOODSTYPE TB ,DOC_GOODSSUP TC WHERE TA.CATID0=TB.CODE AND TA.GDSEQ=TC.GDSEQ(+)");

            if (trbSearch.Text.Length > 0)
            { builder.Append(" and (gdseq like '%" + trbSearch.Text + "%' or gdname like '%" + trbSearch.Text + "%' or  zjm like '%" + trbSearch.Text + "%')"); }
            int total = 0;
            dtData = PubFunc.DbGetPage(GridGoods.PageIndex, GridGoods.PageSize, builder.ToString(), ref total);
            GridGoods.RecordCount = total;
            GridGoods.DataSource = dtData;
            GridGoods.DataBind();

            StringBuilder builderstr = new StringBuilder();

            foreach (DataRow dr in dtData.Rows)
            {


                foreach (DataColumn column in dtData.Columns)
                {
                    string datafile = column.ColumnName;
                    builderstr.Append(datafile + "@" + dr[column].ToString() + "♂");
                }

                builderstr.Append("★");



            }
            if (builderstr.ToString().Length > 0)
            { hdfgood.Text = builderstr.ToString(); }
        }

        private void Get_SupName()
        {

            StringBuilder builder = new StringBuilder();
            builder.Append("select supid,supname from doc_supplier where flag='Y' AND issupplier='Y'");
            if (TriggerBox1.Text.Length > 0)
            {
                builder.Append("and (supid like '%" + TriggerBox1.Text + "%' or supname like '%" + TriggerBox1.Text + "%'    or  str2  like  '%" + TriggerBox1.Text + "%')");

            }


            int total = 0;
            dtData = PubFunc.DbGetPage(GridSu.PageIndex, GridSu.PageSize, builder.ToString(), ref total);
            GridSu.RecordCount = total;
            GridSu.DataSource = dtData;
            GridSu.DataBind();

        }






        protected void GridGoods_PageIndexChange(object sender, FineUIPro.GridPageEventArgs e)
        {
            GridGoods.PageIndex = e.NewPageIndex;
            dataSearch();
        }



        protected void trbSearch_TriggerClick(object sender, EventArgs e)
        {
            dataSearch();
        }

        protected void GridSu_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GridSu.PageIndex = e.NewPageIndex;
            Get_SupName();
        }
        private DataTable Get_ModifyTable()
        {
            DataTable table = new DataTable();


            if (GridGoods != null)
            {
                foreach (GridColumn gc in GridGoods.Columns)
                {
                    if (gc is FineUIPro.RenderField)
                    {
                        table.Columns.Add(new DataColumn(((FineUIPro.RenderField)(gc)).DataField.ToUpper(), typeof(string)));
                    }
                }

                string[] goodsRows = this.hfdValue.Text.Split('★');
                foreach (string rowValue in goodsRows)
                {
                    DataRow dr = table.NewRow();
                    string[] column = rowValue.TrimEnd('♂').Split('♂');

                    if (column.Length != table.Columns.Count) continue;
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        string[] columnvalue = column[i].Split('@');
                        dr[columnvalue[0]] = columnvalue[1];

                    }


                    table.Rows.Add(dr);
                }
            }
            return table;
        }
        protected void GridSu_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            if (e.RowIndex == -1)
                return;




            StringBuilder builder = new StringBuilder();
            foreach (GridColumn gc in GridGoods.Columns)
            {
                if (gc is FineUIPro.RenderField)
                {
                    string datafile = ((FineUIPro.RenderField)(gc)).DataField.ToUpper();
                    if (datafile == "NEWSUPNAME")
                    { builder.Append(datafile.Trim() + "@" + GridSu.Rows[GridSu.SelectedRowIndex].DataKeys[1].ToString().Trim() + "♂"); }
                    else if (datafile == "NEWSUPID")
                    { builder.Append(datafile.Trim() + "@" + GridSu.Rows[GridSu.SelectedRowIndex].DataKeys[0].ToString().Trim() + "♂"); }

                    else
                    {

                        builder.Append(datafile.Trim() + "@" + GridGoods.Rows[Convert.ToInt32(this.Rowid.Text)].Values[gc.ColumnIndex].ToString().Trim() + "♂");
                    }




                }

            }
            if (builder.Length > 0)
            {
                builder.Append("★");
                this.hfdValue.Text = this.hfdValue.Text + builder.ToString().Trim();
            }


            DataTable dt = Get_Table();
            dt.Rows[Convert.ToInt32(this.Rowid.Text)]["NEWSUPNAME"] = GridSu.Rows[GridSu.SelectedRowIndex].DataKeys[1].ToString();
            dt.Rows[Convert.ToInt32(this.Rowid.Text)]["NEWSUPID"] = GridSu.Rows[GridSu.SelectedRowIndex].DataKeys[0].ToString();




            GridGoods.DataSource = dt;
            GridGoods.DataBind();
            GridGoods.SelectedRowIndex = Convert.ToInt32(this.Rowid.Text);
            StringBuilder builder1 = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {


                foreach (DataColumn column in dt.Columns)
                {
                    string datafile = column.ColumnName;
                    builder1.Append(datafile + "@" + dr[column].ToString() + "♂");
                }

                builder1.Append("★");



            }
            if (builder1.ToString().Length > 0)
            { hdfgood.Text = builder1.ToString(); }
            this.WindowSup.Hidden = true;
        }

        private DataTable Get_Table()
        {
            DataTable table = new DataTable();


            if (GridGoods != null)
            {


                string[] goodsRows = this.hdfgood.Text.Split('★');

                foreach (string column in goodsRows[0].TrimEnd('♂').Split('♂'))
                {
                    string[] columnvalue = column.Split('@');
                    table.Columns.Add(new DataColumn(columnvalue[0], typeof(string)));

                }


                foreach (string rowValue in goodsRows)
                {

                    DataRow dr = table.NewRow();
                    string[] column = rowValue.TrimEnd('♂').Split('♂');

                    if (column.Length != table.Columns.Count) continue;
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        string[] columnvalue = column[i].Split('@');
                        dr[columnvalue[0]] = columnvalue[1];

                    }


                    table.Rows.Add(dr);
                }
            }
            return table;
        }

        protected void btnClosePostBack_Click(object sender, EventArgs e)
        {
            if (GridSu.SelectedRowIndex == -1)
                return;
            StringBuilder builder = new StringBuilder();
            foreach (GridColumn gc in GridGoods.Columns)
            {
                if (gc is FineUIPro.RenderField)
                {
                    string datafile = ((FineUIPro.RenderField)(gc)).DataField.ToUpper();
                    if (datafile == "NEWSUPNAME")
                    { builder.Append(datafile.Trim() + "@" + GridSu.Rows[GridSu.SelectedRowIndex].DataKeys[1].ToString().Trim() + "♂"); }
                    else if (datafile == "NEWSUPID")
                    { builder.Append(datafile.Trim() + "@" + GridSu.Rows[GridSu.SelectedRowIndex].DataKeys[0].ToString().Trim() + "♂"); }

                    else
                    {

                        builder.Append(datafile.Trim() + "@" + GridGoods.Rows[Convert.ToInt32(this.Rowid.Text)].Values[gc.ColumnIndex].ToString().Trim() + "♂");
                    }




                }

            }
            if (builder.Length > 0)
            {
                builder.Append("★");
                this.hfdValue.Text = this.hfdValue.Text + builder.ToString().Trim();
            }


            DataTable dt = Get_Table();
            dt.Rows[Convert.ToInt32(this.Rowid.Text)]["NEWSUPNAME"] = GridSu.Rows[GridSu.SelectedRowIndex].DataKeys[1].ToString();
            dt.Rows[Convert.ToInt32(this.Rowid.Text)]["NEWSUPID"] = GridSu.Rows[GridSu.SelectedRowIndex].DataKeys[0].ToString();




            GridGoods.DataSource = dt;
            GridGoods.DataBind();
            GridGoods.SelectedRowIndex = Convert.ToInt32(this.Rowid.Text);
            StringBuilder builder1 = new StringBuilder();
            foreach (DataRow dr in dt.Rows)
            {


                foreach (DataColumn column in dt.Columns)
                {
                    string datafile = column.ColumnName;
                    builder1.Append(datafile + "@" + dr[column].ToString() + "♂");
                }

                builder1.Append("★");



            }
            if (builder1.ToString().Length > 0)
            { hdfgood.Text = builder1.ToString(); }
            this.WindowSup.Hidden = true;

        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            this.WindowSup.Hidden = true;

        }

        protected void GridGoods_RowDoubleClick(object sender, GridRowClickEventArgs e)
        {
            this.WindowSup.Hidden = false;
            this.Rowid.Text = this.GridGoods.SelectedRowIndex.ToString();
            Get_SupName();
        }

        protected void TriggerBox1_TriggerClick(object sender, EventArgs e)
        {
            Get_SupName();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Up_Source();

        }

        private void Up_Source()
        {
            DataTable dt = Get_ModifyTable();
            DataView dataview = new DataView(dt);

            string uptSql = string.Empty;
            List<CommandInfo> lci = new List<CommandInfo>();
            foreach (DataRowView row in dataview)
            {
                String gdseq = row["gdseq"].ToString();
                String newsupplier = row["NEWSUPID"].ToString();
                if (DbHelperOra.Exists("select 1 from DOC_GOODSSUP where gdseq='" + gdseq + "'"))
                {
                    uptSql = "update DOC_GOODSSUP set SUPID='"+newsupplier+"',OPERUSER='"+UserAction.UserID+"' where gdseq='"+gdseq+"'";
                    lci.Add(new CommandInfo(uptSql, null));
                }
                else
                {
                    uptSql = "INSERT INTO DOC_GOODSSUP (CUSTID,GDSEQ, SUPID,OPERUSER) VALUES ('0','" + gdseq + "','" + newsupplier + "','" + UserAction.UserID + "')";
                    lci.Add(new CommandInfo(uptSql, null));
                }
            }
            DbHelperOra.ExecuteSqlTran(lci);
            dataSearch();
            this.hfdValue.Text = "";
            Alert.Show("数据更新成功！");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            dataSearch();
        }
    }
}
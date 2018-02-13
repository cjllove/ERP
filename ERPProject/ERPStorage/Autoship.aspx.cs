using FineUIPro;
using Oracle.ManagedDataAccess.Client;
using XTBase;
using XTBase.Utilities;
using System;
using System.Data;

namespace ERPProject.ERPStorage
{
    public partial class Autoship : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //PubFunc.DdlDataGet("DDL_SYS_DEPOT", docDEPTID);
                PubFunc.DdlDataSql(docDEPTID, "SELECT CODE, '[' || CODE || ']' || NAME NAME   FROM SYS_DEPT WHERE TYPE = 1 OR ISORDER='Y' ORDER BY CODE");
                lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1).AddMonths(-1);
                lstLRRQ2.SelectedDate = DateTime.Now.AddDays(-1);
            }
        }

        protected void BtnAuto_Click(object sender, EventArgs e)
        {

        }

        protected void rblTYPE_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rblTYPE.SelectedValue == "XS")
            {
                lstLRRQ1.Enabled = true;
                lstLRRQ2.Enabled = true;
                GridColumn DSSL = GridList.FindColumn("DSSL");
                DSSL.Hidden = true;
                GridColumn DHDAY = GridList.FindColumn("DHDAY");
                DHDAY.Hidden = false;
                GridColumn DAYSL = GridList.FindColumn("DAYSL");
                DAYSL.Hidden = false;
                GridColumn ZGKC = GridList.FindColumn("ZGKC");
                ZGKC.Hidden = true;
                GridColumn ZDKC = GridList.FindColumn("ZDKC");
                ZDKC.Hidden = true;
                GridColumn DAYUSE = GridList.FindColumn("DAYUSE");
                DAYUSE.Hidden = false;
            }
            else if (rblTYPE.SelectedValue == "DS")
            {
                lstLRRQ1.Enabled = true;
                lstLRRQ2.Enabled = true;
                GridColumn DSSL = GridList.FindColumn("DSSL");
                DSSL.Hidden = false;
                GridColumn DHDAY = GridList.FindColumn("DHDAY");
                DHDAY.Hidden = true;
                GridColumn DAYSL = GridList.FindColumn("DAYSL");
                DAYSL.Hidden = true;
                GridColumn ZGKC = GridList.FindColumn("ZGKC");
                ZGKC.Hidden = true;
                GridColumn ZDKC = GridList.FindColumn("ZDKC");
                ZDKC.Hidden = true;
                GridColumn DAYUSE = GridList.FindColumn("DAYUSE");
                DAYUSE.Hidden = false;
            }
            else
            {
                lstLRRQ1.Enabled = false;
                lstLRRQ2.Enabled = false;
                GridColumn DSSL = GridList.FindColumn("DSSL");
                DSSL.Hidden = true;
                GridColumn DHDAY = GridList.FindColumn("DHDAY");
                DHDAY.Hidden = true;
                GridColumn DAYSL = GridList.FindColumn("DAYSL");
                DAYSL.Hidden = true;
                GridColumn ZGKC = GridList.FindColumn("ZGKC");
                ZGKC.Hidden = false;
                GridColumn ZDKC = GridList.FindColumn("ZDKC");
                ZDKC.Hidden = false;
                GridColumn DAYUSE = GridList.FindColumn("DAYUSE");
                DAYUSE.Hidden = true;
            }
            bntSearch_Click(null, null);
        }
        protected void bntClear_Click(object sender, EventArgs e)
        {
            rblTYPE.SelectedValue = "DS";
            lstLRRQ1.SelectedDate = DateTime.Now.AddDays(-1).AddMonths(-1);
            lstLRRQ2.SelectedDate = DateTime.Now.AddDays(-1);
        }
        protected void btnAutoOrder_Click(object sender, EventArgs e)
        {
            string lsRET;
            DateTime dtrq1 = Convert.ToDateTime(lstLRRQ1.Text);
            DateTime dtrq2 = Convert.ToDateTime(lstLRRQ2.Text);
            string dept = docDEPTID.SelectedValue;
            if (PubFunc.StrIsEmpty(dept))
            {
                Alert.Show("请选择[" + docDEPTID.Label + "]！");
                return;
            }

            if (rblTYPE.SelectedValue == "XS" || rblTYPE.SelectedValue == "DS")
            {
                lsRET = PubFunc.isDateTwoValid(dtrq1, dtrq2);
                if (lsRET.Length > 1)
                {
                    Alert.Show(lsRET);
                    return;
                }
            }

            OracleParameter[] parameters ={
                                            new OracleParameter("VI_DEPT" ,OracleDbType.Varchar2,20),
                                            new OracleParameter("VI_TYPE" ,OracleDbType.Varchar2,20),
                                            new OracleParameter("VI_BEG" ,OracleDbType.Varchar2,10),
                                            new OracleParameter("VI_END" ,OracleDbType.Varchar2,10),
                                            new OracleParameter("VI_USER" ,OracleDbType.Varchar2,20),
                                            new OracleParameter("VO_BILLNUM",OracleDbType.Double)
                                           };
            parameters[0].Value = dept;
            parameters[1].Value = rblTYPE.SelectedValue;
            parameters[2].Value = lstLRRQ1.Text;
            parameters[3].Value = lstLRRQ2.Text;
            parameters[4].Value = UserAction.UserID;

            parameters[0].Direction = ParameterDirection.Input;
            parameters[1].Direction = ParameterDirection.Input;
            parameters[2].Direction = ParameterDirection.Input;
            parameters[3].Direction = ParameterDirection.Input;
            parameters[4].Direction = ParameterDirection.Input;
            parameters[5].Direction = ParameterDirection.Output;

            try
            {
                DbHelperOra.RunProcedure("STORE.P_DD_AUTO", parameters);

                Alert.Show("自动订货生成成功，单品数：【" + parameters[5].Value.ToString() + "】", "消息提示", MessageBoxIcon.Information);
                OperLog("自动订货", "生成自动订货单据单据");
            }
            catch (Exception err)
            {
                throw err;
            }
        }
        protected void bntSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = DbHelperOra.Query(GetSql()).Tables[0];
            GridList.DataSource = dt;
            GridList.DataBind();
            btnAutoOrder.Enabled = true;
        }
        protected string GetSql()
        {
            string Sql = "";
            if (rblTYPE.SelectedValue == "DS")
            {
                Sql = @"SELECT A.* FROM (select cfg.deptid,
                                   G.GDSEQ,
                                   G.GDNAME,
                                   G.GDSPEC,
                                   a.name UNITNAME,
                                   G.BAR3,G.HISCODE,G.HISNAME,G.HSJJ,
                                   G.BZHL,--DECODE(G.UNIT_ORDER,'D','大包装','Z','中包装','小包装') ORDERBZ,
                                   dept.name as deptname,
                                   F_GETZTKC(cfg.deptid,cfg.gdseq) ZTKC,
                                   F_GETCURKC(cfg.deptid,cfg.gdseq) KCSL,
                                   cfg.DHXS,
                                   --(CASE WHEN NVL(cfg.DAYSL,0) < 1 THEN 7 ELSE cfg.DAYSL END) DAYSL,
                                   --(CASE WHEN NVL(cfg.DHDAY,0) < 1 THEN 7 ELSE cfg.DHDAY END) DHDAY,
                                   --cfg.ZDKC,
                                   --cfg.ZGKC,
                                   F_GETJYDH('{0}',G.GDSEQ,'DAY','{1}','{2}') DAYUSE,
                                   F_GETJYDH('{0}',G.GDSEQ,'DS','{1}','{2}') DHSL,
                                   (SELECT SUM(A.DSNUM*A.NUM1) FROM DOC_GOODSCFG A,SYS_DEPT B
                                         WHERE A.DEPTID = B.CODE AND A.GDSEQ = G.GDSEQ AND B.TYPE <> '1' AND NVL(A.DSNUM,0) > 0 AND NVL(A.NUM1,0) > 0) DSSL
                              from DOC_GOODS     G,
                                   doc_goodsunit A,
                                   doc_goodscfg  cfg,
                                   sys_dept      dept
                             where G.ISDELETE = 'N'
                               and G.UNIT = A.code(+)
                               and cfg.deptid = dept.code(+)
                               and cfg.ISAUTO = 'Y'
                               and cfg.gdseq ＝g.gdseq 
                               and cfg.deptid = '{0}'
                               and G.isflag7 = 'N'
                               and G.gdseq in (select cfg.gdseq from doc_goodssup B where cfg.DEPTID = '{0}' AND cfg.GDSEQ = B.GDSEQ  AND B.STR1 = 'N') 
                               ) A
                               WHERE DHSL > 0";
            }
            else if (rblTYPE.SelectedValue == "XS")
            {
                Sql = @"SELECT A.* FROM (select cfg.deptid,
                                   G.GDSEQ,
                                   G.GDNAME,
                                   G.GDSPEC,
                                   a.name UNITNAME,
                                   G.BAR3,G.HISCODE,G.HISNAME,G.HSJJ,
                                   G.BZHL,--DECODE(G.UNIT_ORDER,'D','大包装','Z','中包装','小包装') ORDERBZ,
                                   dept.name as deptname,
                                   F_GETZTKC(cfg.deptid,cfg.gdseq) ZTKC,
                                   F_GETCURKC(cfg.deptid,cfg.gdseq) KCSL,
                                   cfg.DHXS,
                                   (CASE WHEN NVL(cfg.DAYSL,0) < 1 THEN 7 ELSE cfg.DAYSL END) DAYSL,
                                   (CASE WHEN NVL(cfg.DHDAY,0) < 1 THEN 7 ELSE cfg.DHDAY END) DHDAY,
                                   cfg.ZDKC,
                                   cfg.ZGKC,
                                   F_GETJYDH('{0}',G.GDSEQ,'DAY','{1}','{2}') DAYUSE,
                                   F_GETJYDH('{0}',G.GDSEQ,'XS','{1}','{2}') DHSL
                              from DOC_GOODS     G,
                                   doc_goodsunit A,
                                   doc_goodscfg  cfg,
                                   sys_dept      dept
                             where G.ISDELETE = 'N'
                               and G.UNIT = A.code(+)
                               and cfg.deptid = dept.code(+)
                               and cfg.ISAUTO = 'Y'
                               and cfg.gdseq ＝g.gdseq 
                               and cfg.deptid = '{0}'
                               and G.isflag7 = 'N'
                               and G.gdseq in (select cfg.gdseq from doc_goodssup B where cfg.DEPTID = '{0}' AND cfg.GDSEQ = B.GDSEQ  AND B.STR1 = 'N') 
                               ) A
                               WHERE DHSL > 0";
            }
            else
            {
                Sql = @"SELECT A.* FROM (select cfg.deptid,
                                   G.GDSEQ,
                                   G.GDNAME,
                                   G.GDSPEC,
                                   a.name UNITNAME,
                                   G.BAR3,G.HISCODE,G.HISNAME,G.HSJJ,
                                   G.BZHL,--DECODE(G.UNIT_ORDER,'D','大包装','Z','中包装','小包装') ORDERBZ,
                                   dept.name as deptname,
                                   F_GETZTKC(cfg.deptid,cfg.gdseq) ZTKC,
                                   F_GETCURKC(cfg.deptid,cfg.gdseq) KCSL,
                                   cfg.DHXS,
                                   (CASE WHEN NVL(cfg.DAYSL,0) < 1 THEN 7 ELSE cfg.DAYSL END) DAYSL,
                                   (CASE WHEN NVL(cfg.DHDAY,0) < 1 THEN 7 ELSE cfg.DHDAY END) DHDAY,
                                   cfg.ZDKC,
                                   cfg.ZGKC,
                                   F_GETJYDH('{0}',G.GDSEQ,'DAY','{1}','{2}') DAYUSE,
                                   F_GETJYDH('{0}',G.GDSEQ,'KC','{1}','{2}') DHSL
                              from DOC_GOODS     G,
                                   doc_goodsunit A,
                                   doc_goodscfg  cfg,
                                   sys_dept      dept
                             where G.ISDELETE = 'N'
                               and G.UNIT = A.code(+)
                               and cfg.deptid = dept.code(+)
                               and cfg.ISAUTO = 'Y'
                               and cfg.gdseq ＝g.gdseq 
                               and cfg.deptid = '{0}'
                               and G.isflag7 = 'N'
                               and G.gdseq in (select cfg.gdseq from doc_goodssup B where cfg.DEPTID = '{0}' AND cfg.GDSEQ = B.GDSEQ  AND B.STR1 = 'N')  
                               ) A
                               WHERE DHSL > 0";
            }
            return string.Format(Sql, docDEPTID.SelectedValue, lstLRRQ1.Text, lstLRRQ2.Text); ;
        }
        protected void btExp_Click(object sender, EventArgs e)
        {

            DataTable dt = DbHelperOra.Query(GetExcelSql()).Tables[0];
            if (dt.Rows.Count < 1) return;
            ExcelHelper.ExportByWeb(dt, string.Format("【{0}】建议订货导出", docDEPTID.SelectedText), "建议订货导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls");
        }
        protected string GetExcelSql()
        {
            string Sql = "";
            if (rblTYPE.SelectedValue == "DS")
            {
                Sql = @"SELECT A.* FROM (select
                                   G.GDSEQ 商品编码,
                                   G.GDNAME 商品名称,
                                   G.GDSPEC 规格容量,
                                   a.name 包装单位,
                                   G.HSJJ 含税进价,
                                   F_GETCURKC(cfg.deptid,cfg.gdseq) 当前库存,F_GETZTKC(cfg.deptid,cfg.gdseq) 在途库存,
                                   F_GETJYDH('{0}',G.GDSEQ,'DAY','{1}','{2}') 日均用量,
                                   (SELECT SUM(A.DSNUM*A.NUM1) FROM DOC_GOODSCFG A,SYS_DEPT B
                                         WHERE A.DEPTID = B.CODE AND A.GDSEQ = G.GDSEQ AND B.TYPE <> '1' AND NVL(A.DSNUM,0) > 0 AND NVL(A.NUM1,0) > 0) 定数量,
                                   F_GETJYDH('{0}',G.GDSEQ,'DS','{1}','{2}') 建议订货量,
                                   G.BAR3 ERP编码,G.HISCODE HIS编码,G.HISNAME HIS名称
                              from DOC_GOODS     G,
                                   doc_goodsunit A,
                                   doc_goodscfg  cfg,
                                   sys_dept      dept
                             where G.ISDELETE = 'N'
                               and G.UNIT = A.code(+)
                               and cfg.deptid = dept.code(+)
                               and cfg.ISAUTO = 'Y'
                               and cfg.gdseq ＝g.gdseq 
                               and cfg.deptid = '{0}') A
                               WHERE 建议订货量 > 0";
            }
            else if (rblTYPE.SelectedValue == "XS")
            {
                Sql = @"SELECT A.* FROM (select
                                   G.GDSEQ 商品编码,
                                   G.GDNAME 商品名称,
                                   G.GDSPEC 规格容量,
                                   a.name 包装单位,
                                   G.HSJJ 含税进价,
                                   F_GETCURKC(cfg.deptid,cfg.gdseq) 当前库存,F_GETZTKC(cfg.deptid,cfg.gdseq) 在途库存,
                                   (CASE WHEN NVL(cfg.DAYSL,0) < 1 THEN 7 ELSE cfg.DAYSL END) 备货天数,
                                   (CASE WHEN NVL(cfg.DHDAY,0) < 1 THEN 7 ELSE cfg.DHDAY END) 送货天数,
                                   F_GETJYDH('{0}',G.GDSEQ,'DAY','{1}','{2}') 日均用量,
                                   F_GETJYDH('{0}',G.GDSEQ,'XS','{1}','{2}') 建议订货量,
                                   G.BAR3 ERP编码,G.HISCODE HIS编码,G.HISNAME HIS名称
                              from DOC_GOODS     G,
                                   doc_goodsunit A,
                                   doc_goodscfg  cfg,
                                   sys_dept      dept
                             where G.ISDELETE = 'N'
                               and G.UNIT = A.code(+)
                               and cfg.deptid = dept.code(+)
                               and cfg.ISAUTO = 'Y'
                               and cfg.gdseq ＝g.gdseq 
                               and cfg.deptid = '{0}') A
                               WHERE 建议订货量 > 0";
            }
            else
            {
                Sql = @"SELECT A.* FROM (select
                                   G.GDSEQ 商品编码,
                                   G.GDNAME 商品名称,
                                   G.GDSPEC 规格容量,
                                   a.name 包装单位,
                                   G.HSJJ 含税进价,
                                   F_GETCURKC(cfg.deptid,cfg.gdseq) 当前库存,F_GETZTKC(cfg.deptid,cfg.gdseq) 在途库存,
                                   cfg.ZGKC 最高库存,cfg.ZDKC 最低库存,
                                   --F_GETJYDH('{0}',G.GDSEQ,'DAY','{1}','{2}') 日均用量,
                                   F_GETJYDH('{0}',G.GDSEQ,'KC','{1}','{2}') 建议订货量,
                                   G.BAR3 ERP编码,G.HISCODE HIS编码,G.HISNAME HIS名称
                              from DOC_GOODS     G,
                                   doc_goodsunit A,
                                   doc_goodscfg  cfg,
                                   sys_dept      dept
                             where G.ISDELETE = 'N'
                               and G.UNIT = A.code(+)
                               and cfg.deptid = dept.code(+)
                               and cfg.ISAUTO = 'Y'
                               and cfg.gdseq ＝g.gdseq 
                               and cfg.deptid = '{0}') A
                               WHERE 建议订货量 > 0";
            }
            return string.Format(Sql, docDEPTID.SelectedValue, lstLRRQ1.Text, lstLRRQ2.Text); ;
        }
    }
}
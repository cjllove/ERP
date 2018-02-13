﻿using XTBase;
using XTBase.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace ERPProject.SpdReport
{
    public partial class PrintReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string method = Request["Method"].ToString();
            if (!string.IsNullOrWhiteSpace(method))
            {
                MethodInfo methodInfo = this.GetType().GetMethod(method);
                methodInfo.Invoke(this, null);
            }
        }

        public void GetPrintData()
        {
            string rid = Request["rid"] == null ? "" : Request["rid"].ToString();
            string para = Request["pa"] == null ? "" : Request["pa"].ToString();

            Report Report = new Report(rid);
            string strMenuSql1 = Report.RepSQL;

            string[] strCond = para.Split('_');


            for (int i = 0; i < Report.RepCond.Count; i++)
            {
                if (Report.RepCond[i].DATATYPE == "DATE")
                {
                    strMenuSql1 = strMenuSql1.Replace(":" + Report.RepCond[i].COLID, "TO_DATE('" + strCond[i] + "','YYYY-MM-DD')");
                }
                else if (Report.RepCond[i].DATATYPE == "NUMBER")
                {
                    strMenuSql1 = strMenuSql1.Replace(":" + Report.RepCond[i].COLID, strCond[i] == "" ? "0" : strCond[i]);
                }
                else
                {
                    int li_pos = strMenuSql1.IndexOf(":" + Report.RepCond[i].COLID);
                    if (strMenuSql1.Substring(0, li_pos).TrimEnd().ToUpper().EndsWith("LIKE"))
                    {
                        strMenuSql1 = strMenuSql1.Replace(":" + Report.RepCond[i].COLID, strCond[i] == "" ? "'%'" : "'%" + strCond[i].ToUpper() + "%'");
                    }
                    else
                    {
                        strMenuSql1 = strMenuSql1.Replace(":" + Report.RepCond[i].COLID, strCond[i] == "" ? "'%'" : "'" + strCond[i] + "'");
                    }
                }
            }
            OracleXMLReportData.GenDetailData(this, strMenuSql1);
        }

        public void GetOrderData()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();

            string title = "订货单";
            string sql = @"select a.seqno,b.pzwh,
                               F_GETDEPTNAME(a.deptid) deptidname,
                               to_char(A.XDRQ, 'YYYY-MM-DD') XDRQ,
                               b.gdseq,
                               b.gdname,
                               b.unit,
                               b.gdspec,
                               b.bzhl,
                               b.hsjj,
                               B.PZWH,
                               F_GETPRODUCERNAME(b.producer) producername,
                               a.pssname,
                               b.rowno,
                               to_char(sysdate, 'YYYY-MM-DD') printtime,
                               F_GETUNITNAME(b.unit) unitname,
                               HSJE,
                               F_GETUSERNAME(a.lry) ZDY,
                               B.BZSL,
                               F_GETPARA('SUPPER') || '{1}' || DECODE(C.ISGZ, 'Y',' 高值','') DT,
                               --F_GETPARA('SUPPER')KH,
                               F_GETSUPNAME(A.PSSID)PSSNAME,
                               C.ISGZ,
                               (SELECT SUM(HSJE) FROM dat_dd_com WHERE SEQNO = A.SEQNO) sumhjje,
                                F_GETBILLCOUNT(A.SEQNO,'DHD',10) SUBNUM
                          from dat_dd_doc a, dat_dd_com b,DOC_GOODS C
                         where a.seqno = b.seqno AND B.GDSEQ = C.GDSEQ(+)
                           AND A.SEQNO IN ('{0}') order by a.seqno,c.isgz,b.rowno";
            //OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
            //OracleXMLReportData.GenDetailData(this, string.Format(sql, osid, title));
            sql = string.Format(sql, osid.Replace("_", "','"), title);
            OracleXMLReportData.GenDetailData(this, sql);
        }
        public void GoodsRtn()
        {
            //退货至EAS
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"SELECT A.SEQNO,F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,A.THRQ,B.GDSEQ,B.GDNAME,B.UNIT,B.GDSPEC,B.BZHL,B.HSJJ,F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,B.ROWNO,TO_CHAR(SYSDATE,'YYYY-MM-DD') PRINTTIME
            //        ,F_GETUNITNAME(B.UNIT) UNITNAME,HSJE,F_GETUSERNAME(A.LRY) ZDY,B.BZSL,B.BZSL,F_GETPARA('USERNAME')||'采购退货单' DT,F_GETPARA('SUPPER')KH,B.THSL,B.PH,F_GETHIS_CODE(B.GDSEQ) HIS_CODE
            //         ,F_GETSUPNAME(A.PSSID) PSSNAME,(SELECT SUM(HSJE) FROM DAT_TH_COM WHERE SEQNO = '{0}') SUMHJJE,B.HSJJ||'元/'||F_GETUNITNAME(B.UNIT) V_HSJJ
            //            FROM DAT_TH_DOC A,DAT_TH_COM B WHERE A.SEQNO = B.SEQNO AND A.SEQNO='{0}'";
            //F_GETPARA('SUPPER') 改成了讯通的名字，根据测试需求改的。
            string sql = @"SELECT A.SEQNO,F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,A.THRQ,B.GDSEQ,B.GDNAME,B.UNIT,B.GDSPEC,B.BZHL,B.PZWH,(B.HSJJ/B.BZHL) HSJJ,F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,B.ROWNO,TO_CHAR(SYSDATE,'YYYY-MM-DD') PRINTTIME
                    ,F_GETUNITNAME(C.UNIT) UNITNAME,HSJE,F_GETUSERNAME(A.LRY) ZDY,(B.BZSL*B.BZHL)BZSL, F_GETPARA('SUPPER')||'采购退货单'||DECODE(C.ISGZ,'Y','（高值）') DT,C.ISGZ,F_GETPARA('SUPPER')KH,B.THSL,B.PH,F_GETHIS_CODE(B.GDSEQ) HIS_CODE
                     ,F_GETSUPNAME(A.PSSID) PSSNAME,(SELECT SUM(HSJE) FROM DAT_TH_COM WHERE SEQNO = '{0}') SUBSUM,B.HSJJ,B.HSJE
                        FROM DAT_TH_DOC A,DAT_TH_COM B,DOC_GOODS C WHERE A.SEQNO = B.SEQNO AND A.SEQNO='{0}'AND B.GDSEQ=C.GDSEQ";

            OracleXMLReportData.GenDetailData(this, string.Format(sql, osid));
        }
        public void GetStorageData()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string flag = Request["flag"] == null ? "ph" : Request["flag"].ToString();
            string title = "医疗物资质量验收入库单";

            if (flag.ToLower() == "pj")
            {
                title = "设备维修（配件）入库单";
            }
            //F_GETPARA('USERNAME')
            string sql = @"SELECT A.SEQNO,
                               F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                               TO_CHAR(SYSDATE, 'yyyy-MM-dd') PRINTDATE,
                               TO_CHAR(A.DHRQ, 'yyyy-MM-dd') DHRQ,
f_getusername(A.LRY) LRY,
                               --A.DHRQ,
                               B.ROWNO,
                               B.GDSEQ,
                               B.GDNAME,
                               C.LEADER,
                               B.BARCODE,
                               B.GDSPEC,
                               B.PZWH,
                               B.PH,
                               B.YXQZ,
                               B.SSSL BZSL,
                               F_GETUNITNAME(B.UNIT) UNITNAME,
                               B.HSJJ,
                               B.HSJE,
                               B.HWID,
                               --F_GETUSERNAME(A.LRY) LRYNAME,
                               SUM(NVL(B.HSJJ * B.SSSL, 0)) OVER(PARTITION BY A.SEQNO) SUMHJJE,
                               C.SUPNAME,
                               C.TEL TELEPHONE,
                               F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                               A.PSSID,
                               f_getsupname(a.pssid) pssname,
                               D.ISGZ,
                               F_GETPARA('SUPPER') || '{1}' || DECODE(D.ISGZ, 'Y', '(高值)') || DECODE(A.PSSID, '00001', '代管') DT，
                               F_GETBILLCOUNT(A.SEQNO,'RKD',10) SUBNUM
                          FROM DAT_RK_DOC A, DAT_RK_COM B, DOC_SUPPLIER C, DOC_GOODS D
                         WHERE A.SEQNO = B.SEQNO
                           AND A.PSSID = C.SUPID AND B.GDSEQ = D.GDSEQ
                           AND A.SEQNO IN ('{0}') --AND C.ISSUPPLIER='Y'
                         ORDER BY B.SEQNO,a.pssid,d.isgz,B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid.Replace("_", "','"), title), false);
        }
        public void GetYSJD()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();

            string sql = @"SELECT A.SEQNO,F_GETDEPTNAME(A.DEPTID) DEPTID,A.DHRQ,ROWNUM ROWNO,GDSEQ,GDNAME,BARCODE,GDSPEC,PH PZWH,YXQZ,SSSL,f_getunitname(UNIT) UNIT,HSJJ,HSJE,HWID,TO_CHAR(SYSDATE,'yyyy-MM-dd') PRINTDATE,
                                  F_GETUSERNAME(A.LRY) JHY,SUM(NVL(B.HSJJ*B.SSSL, 0)) OVER(PARTITION BY A.SEQNO ORDER BY A.SEQNO) AMOUNT,B.PH,A.INVOICENUMBER,f_getsuppliername(pssid) pssname,
                                 (SELECT F_GETPRODUCERNAME(G.PRODUCER) FROM DOC_GOODS G WHERE G.GDSEQ=B.GDSEQ) PRODUCER,F_GETPARA('USERNAME')||'入库上架单' HISTROY
                            FROM  DAT_RK_DOC A, DAT_RK_COM B WHERE A.SEQNO = B.SEQNO AND A.SEQNO='{0}'";

            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetYSRKSJDData()
        {
            string user = Request["user"] == null ? "" : Request["user"].ToString();
            string begrq = Request["beg"] == null ? "" : Request["beg"].ToString();
            string endrq = Request["end"] == null ? "" : Request["end"].ToString();
            string flag = Request["fg"] == null ? "" : Request["fg"].ToString();
            string gys = Request["gys"] == null ? "" : Request["gys"].ToString();
            string supid = "";
            if (!string.IsNullOrWhiteSpace(gys))
            {
                supid = " AND A.PSSID ='" + gys + "'";
            }
            string sql = @"SELECT F_GETSUPNAME(A.PSSID) PSSNAME,
                                           (SELECT MIN(SHRQ)
                                              FROM DAT_RK_DOC
                                             WHERE A.SHRQ BETWEEN TO_DATE('{1}', 'YYYY-MM-DD') AND
                                                   TO_DATE('{2}', 'YYYY-MM-DD') + 1) BEGRQ,
                                           (SELECT MAX(SHRQ)
                                              FROM DAT_RK_DOC
                                             WHERE A.SHRQ BETWEEN TO_DATE('{1}', 'YYYY-MM-DD') AND
                                                   TO_DATE('{2}', 'YYYY-MM-DD') + 1) ENDRQ,
                                           F_GETUSERNAME('{0}') JHY,
                                           GDSEQ,
                                           GDNAME,
                                           GDSPEC,
                                           PZWH,
                                           YXQZ,PH,
                                           SSSL,
                                           F_GETUNITNAME(B.UNIT) UNIT,
                                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCER,
                                           HWID,
                                           F_GETPARA('SUPPER')|| '医疗物资质量验收入库上架表' HISTROY
                                      FROM DAT_RK_DOC A, DAT_RK_COM B
                                     WHERE A.SEQNO = B.SEQNO
                                       AND A.SHRQ BETWEEN TO_DATE('{1}', 'YYYY-MM-DD') AND
                                               TO_DATE('{2}', 'YYYY-MM-DD') + 1
                                       AND A.FLAG LIKE '%{3}%' {4}
                                     ORDER BY B.SEQNO, B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, user, begrq, endrq, flag, supid), false);
        }
        public void GetOnecodeData()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.DEPTID,A.ONECODE,A.GDSEQ,A.GDNAME,UNIT,A.PH,TO_CHAR(A.YXQZ,'YYYY-MM-DD')||'(效期)' YXQ,A.GDSPEC,F_GETDEPTNAME(A.DEPTID) DEPTIDNAME ,b.SUPID,F_GETPRODUCERNAME(b.SUPID) PRODUCENAME
                        ,SYSDATE printdate ,A.GDNAME||' - '||GDSPEC NAMESPEC 
                            from DAT_RK_EXT A,DAT_RK_DOC B WHERE A.BILLNO = B.SEQNO AND A.BILLNO='{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetOnecodeZSM()
        {
            //追溯码
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.DEPTID,
                       A.ONECODE,
                       A.GDSEQ,
                       A.GDNAME,
                       f_getunitname(A.UNIT) UNIT,
                       A.PH,
                       TO_CHAR(A.YXQZ, 'YYYY-MM-DD') XQ,
                       A.GDSPEC,
                       F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                       F_GETSUPNAME(D.SUPID) SUPNAME,
                       SYSDATE printdate,
                       A.GDNAME || ' - ' || A.GDSPEC NAMESPEC,
                       f_getproducername(C.PRODUCER) PRODUCERNAME
                  from DAT_CK_EXT A, DAT_CK_DOC B, DOC_GOODS C,DAT_GZ_EXT D
                 WHERE A.BILLNO = B.SEQNO
                   AND B.SEQNO=D.BILLNO
                   AND A.BILLNO = '{0}'
                   AND A.GDSEQ = C.GDSEQ
                   AND A.GDSEQ=D.GDSEQ AND A.ROWNO=D.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetDSData()
        {
            //定数条码
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select a.*,
                           to_char(OUTRQ, 'YYYY-MM-DD') DATE_ck,
                           nvl(c.HISNAME,c.gdname) gdname,
                           c.gdspec,
                           nvl(c.HISNAME,c.gdname) || ' - ' || c.gdspec gd,
                           F_GETPRODUCERNAME(c.producer) producername,
                           F_GETDEPTNAME(a.deptin) deptidname,
                           (F_GETBILLGDNO(SEQNO,ROWNO,a.gdseq,A.DEPTIN)) numb
                      from DAT_GOODSDS_LOG a, doc_goods c
                     where a.gdseq = c.gdseq  AND A.SEQNO IN ('{0}') ORDER BY A.SEQNO,ROWNO";
            osid = osid.Replace(",", "','");//B.SEQNO,B.ROWNO
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetStockOutData()
        {
            //定数条码
            string deptid = Request["deptid"] == null ? "" : Request["deptid"].ToString();
            string gdseq = Request["gdseq"] == null ? "" : Request["gdseq"].ToString();
            string begrq = Request["b"] == null ? "" : Request["b"].ToString();
            string endrq = Request["e"] == null ? "" : Request["e"].ToString();
            string dhlx = Request["dh"] == null ? "" : Request["dh"].ToString();
            string sql = @"SELECT SD.NAME DEPTNAME,B.GDSEQ,B.GDNAME,B.GDSPEC,F_GETUNITNAME(B.UNIT) UNIT,B.HSJJ,SUM(B.XSSL) SL,
                                             SUM(B.HSJE) JE,G.HISCODE,F_GETPARA('SUPPER')||'科室出库单汇总' DT
                                       FROM DAT_CK_DOC A, DAT_CK_COM B, SYS_DEPT SD, DOC_GOODS G
                                      WHERE A.SEQNO = B.SEQNO AND B.GDSEQ = G.GDSEQ AND A.DEPTID = SD.CODE  AND A.FLAG='Y' ";

            string strWhere = " ";
            if (!PubFunc.StrIsEmpty(deptid)) strWhere += " and A.DEPTID='" + deptid + "'";
            if (!PubFunc.StrIsEmpty(gdseq)) strWhere += " and (G.GDSEQ like '%" + gdseq + "%' or g.zjm like '%" + gdseq + "%' or g.gdname like '%" + gdseq + "%')";
            if (string.IsNullOrWhiteSpace(begrq))
            {
                begrq = DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (string.IsNullOrWhiteSpace(endrq))
            {
                endrq = DateTime.Now.ToString("yyyy-MM-dd");
            }

            strWhere += string.Format(" and A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD') and A.XSRQ < TO_DATE('{1}','YYYY-MM-DD')+1 ", begrq, endrq);
            if (!string.IsNullOrWhiteSpace(dhlx))
            {
                strWhere += string.Format(" and g.catid0='{0}' ", dhlx);
            }

            sql = sql + strWhere;

            sql += " GROUP BY SD.NAME, B.GDSEQ, B.GDNAME, B.GDSPEC, B.UNIT, B.HSJJ, G.HISCODE ORDER BY B.GDNAME";
            OracleReportData.GenNodeXmlData(this, sql, false);
        }
        public void GetStockOutData1()
        {
            //定数条码
            string gdseq = Request["gdseq"] == null ? "" : Request["gdseq"].ToString();
            string begrq = Request["b"] == null ? "" : Request["b"].ToString();
            string endrq = Request["e"] == null ? "" : Request["e"].ToString();
            string dhxl = Request["d"] == null ? "" : Request["d"].ToString();
            //string sql = @" SELECT B.GDSEQ,
            //                                       B.GDNAME,
            //                                       B.GDSPEC,
            //                                       F_GETUNITNAME(B.UNIT) UNIT,
            //                                       C.NAME CATTYPE,
            //                                       B.HSJJ,B.HISCODE,
            //                                       DECODE(B.ISCF,'Y','是','否') ISSF,
            //                                       F_GETPRODUCERNAME(B.PRODUCER) PRODUCER,
            //                                       F_GETPARA('B.SUPPER')||'科室出库单汇总' DT
            //                                       (-SUM(DECODE(A.BILLTYPE,'XST',DECODE(A.KCADD,'1',A.SL,0),A.SL))) SL,
            //                                       (-SUM(DECODE(A.BILLTYPE,'XST',DECODE(A.KCADD,'1',A.SL,0),A.SL) * A.HSJJ)) JE
            //                                  FROM DAT_GOODSJXC A, DOC_GOODS B, SYS_CATEGORY C
            //                                 WHERE A.GDSEQ = B.GDSEQ
            //                                   AND B.CATID = C.CODE
            //                                   AND A.BILLTYPE IN ('DSH','XSD','XSG','XST')
            //                                   AND A.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND TO_DATE('{1}', 'YYYY-MM-DD') + 1
            //                                   AND B.CATID0='{2}'
            //                                   AND A.DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE TYPE IN ('3', '4'))";

            //string strWhere = " ";
            //if (!PubFunc.StrIsEmpty(gdseq)) strWhere += " and (G.GDSEQ like '%" + gdseq + "%' or g.zjm like '%" + gdseq + "%' or g.gdname like '%" + gdseq + "%')";
            //if (string.IsNullOrWhiteSpace(begrq))
            //{
            //    begrq = DateTime.Now.ToString("yyyy-MM-dd");
            //}
            //if (string.IsNullOrWhiteSpace(endrq))
            //{
            //    endrq = DateTime.Now.ToString("yyyy-MM-dd");
            //}
            //strWhere += string.Format(" and A.XSRQ>=TO_DATE('{0}','YYYY-MM-DD') and A.XSRQ < TO_DATE('{1}','YYYY-MM-DD')+1 ", begrq, endrq);

            //sql = sql + strWhere;

            //sql += " GROUP BY B.GDSEQ, B.GDNAME, B.GDSPEC, B.UNIT, B.HSJJ, G.HISCODE ORDER BY B.GDNAME";

            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT B.GDSEQ,
                                                   B.GDNAME,
                                                   B.GDSPEC,
                                                   F_GETUNITNAME(B.UNIT) UNIT,
                                                   C.NAME CATTYPE,
                                                   B.HSJJ,B.HISCODE,
                                                   DECODE(B.ISCF,'Y','是','否') ISSF,
                                                   F_GETPRODUCERNAME(B.PRODUCER) PRODUCER,
                                                   F_GETPARA('B.SUPPER')||'科室出库单汇总' DT,
                                                   (-SUM(DECODE(A.BILLTYPE,'XST',DECODE(A.KCADD,'1',A.SL,0),A.SL))) SL,
                                                   (-SUM(DECODE(A.BILLTYPE,'XST',DECODE(A.KCADD,'1',A.SL,0),A.SL) * A.HSJJ)) JE
                                              FROM DAT_GOODSJXC A, DOC_GOODS B, SYS_CATEGORY C
                                             WHERE A.GDSEQ = B.GDSEQ
                                               AND B.CATID = C.CODE
                                               AND A.BILLTYPE IN ('DSH','XSD','XSG','XST')
                                               AND A.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                               AND B.CATID0='{2}'
                                               AND A.DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE TYPE IN ('3', '4')) ", begrq, endrq, dhxl);

            if (!PubFunc.StrIsEmpty(gdseq))
            {
                sbSql.AppendFormat(" AND (A.GDSEQ='%{0}%' OR B.GDNAME LIKE '%{0}%'  OR B.GDSPEC LIKE '%{0}%' OR UPPER(B.ZJM) LIKE UPPER('%{0}%'))", gdseq);
            }
            sbSql.Append(@"   GROUP BY B.GDSEQ,
                                                        B.GDNAME,
                                                        B.GDSPEC,B.ISCF,
                                                        B.UNIT,
                                                        B.HSJJ,B.HISCODE,
                                                        C.NAME,
                                                        B.PRODUCER
                                             ORDER BY B.GDNAME,B.GDSPEC");
            OracleReportData.GenNodeXmlData(this, sbSql.ToString(), false);
        }
        public void GetStockOutData2()
        {

            string beg = Request["b"] == null ? "" : Request["b"].ToString();
            string end = Request["e"] == null ? "" : Request["e"].ToString();
            string sql = @"SELECT DEPTID,
                                                   F_GETDEPTNAME(DEPTID) DEPTNAME,ROWNO,
                                                    HC0101,
                                                   HC0102,
                                                   HC0103,
                                                    HC0104,
                                                    HC0105,
                                                    HC0106,
                                                    HC0107,
                                                    HC0108,
                                                    HC0109,
                                                    HC0110,
                                                    HC0111, TOTALJE,
                                                    TO_DATE('{1}','YYYY-MM-DD') ZBRQ,
                                                   '{2}'||'医疗物资科室领用汇总' DT
                                              FROM TEMP_KSYDBB WHERE BEGRQ='{0}' AND ENDRQ='{1}' ORDER BY ROWNO";

            OracleReportData.GenNodeXmlData(this, string.Format(sql, beg, end, string.Format("{0:yyyy年MM月}", Convert.ToDateTime(end))), false);
        }
        public void GetDSDataBill()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            /*string sql = @"SELECT A.SEQNO,
	                           F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
	                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
	                           A.XSRQ,
	                           B.GDSEQ,
	                           B.GDNAME,
	                           B.GDSPEC,
	                           F_GETPRINTINF(B.GDSEQ, 'SL', B.BZHL) BZHL, 
	                           --B.BZHL,
	                           F_GETPRINTINF(B.GDSEQ, 'HSJJ', 0) HSJJ, 
	                           --B.HSJJ,
	                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
	                           B.ROWNO,
	                           B.STR2,
	                           TO_CHAR(A.SHRQ, 'YYYY-MM-DD') DJRQ,
	                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
	                           F_GETPRINTINF(B.GDSEQ, 'UNITNAME', 0) UNITNAME,
	                           --f_getunitname(B.UNIT) UNITNAME,
	                           B.HSJE,
	                           F_GETUSERNAME(A.LRY) ZDY,
	                           F_GETPARA('SUPPER') || '随货同行单(定数)' DT,
	                           B.PH,
	                           B.YXQZ,
	                           (SELECT SUM(HSJE) FROM DAT_CK_COM WHERE SEQNO = '{0}' AND NVL(FPSL,0) > 0) SUMHJJE,
	                           F_GETPRINTINF(B.GDSEQ, 'HSJJ', 0) || '元/' || F_GETPRINTINF(B.GDSEQ, 'UNITNAME', 0) V_HSJJ,
	                           (select CODE_HIS from sys_dept where code = A.DEPTID) KJCODE,--天津，会计编码
	                           F_GETHWID(A.DEPTOUT,B.GDSEQ) HWID ,b.FPSL / b.BZHL CH     
                          FROM DAT_CK_DOC A, DAT_CK_COM B, DAT_GOODSJXC C
                         WHERE A.SEQNO = B.SEQNO 
	                        AND B.SEQNO = C.BILLNO AND C.KCADD = 1
	                        AND B.ROWNO = C.ROWNO
	                        and (b.FPSL / b.BZHL) >0 
	                        AND A.SEQNO = '{0}'
	                        ORDER BY B.SEQNO,B.ROWNO";*/
            string sql = @"SELECT A.SEQNO,
                                   F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                   F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                   A.XSRQ,
                                   B.GDSEQ,
                                   B.GDNAME,
                                   B.GDSPEC,
                                   SUM(C.SL) BZHL,
                                   --F_GETPRINTINF(B.GDSEQ, 'HSJJ', 0) HSJJ,
                                   B.HSJJ,
                                   F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                   B.ROWNO,
                                   B.STR2,
                                   TO_CHAR(A.SHRQ, 'YYYY-MM-DD') DJRQ,
                                   TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                                   --F_GETPRINTINF(B.GDSEQ, 'UNITNAME', 0) UNITNAME,
                                   f_getunitname(B.UNIT) UNITNAME,
                                   B.HSJJ*SUM(C.SL) HSJE,
                                   F_GETUSERNAME(A.LRY) ZDY,
                                   F_GETPARA('SUPPER') || '随货同行单(定数)' DT,
                                   C.PH,
                                   B.YXQZ,
                                   (SELECT SUM(HSJE)
                                      FROM DAT_CK_COM
                                     WHERE SEQNO = '{0}'
                                       AND NVL(FPSL, 0) > 0) SUMHJJE,
                                   B.HSJJ|| '元/' || f_getunitname(B.UNIT) V_HSJJ,
                                   (select CODE_HIS from sys_dept where code = A.DEPTID) KJCODE, --天津，会计编码
                                   F_GETHWID(A.DEPTOUT, B.GDSEQ) HWID,
                                   b.FPSL / b.BZHL CH
                              FROM DAT_CK_DOC A, DAT_CK_COM B, DAT_GOODSJXC C
                             WHERE A.SEQNO = B.SEQNO
                               AND B.SEQNO = C.BILLNO
                               AND C.KCADD = 1
                               AND B.ROWNO = C.ROWNO
                               and(b.FPSL / b.BZHL) > 0
                               AND A.SEQNO = '{0}'
                             GROUP BY A.SEQNO,
                                      A.DEPTOUT,
                                      A.DEPTID,
                                      A.XSRQ,
                                      B.GDSEQ,
                                      B.GDNAME,
                                      B.GDSPEC,
                                      B.PRODUCER,
                                      B.ROWNO,
                                      A.LRY,
                                      B.STR2,
                                      A.SHRQ,
                                      C.PH,
                                      B.YXQZ,
                                      b.FPSL,
                                      b.BZHL,B.HSJJ,B.HSJE,B.UNIT
                             ORDER BY A.SEQNO, B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        //菏泽中医医院 lvj 20190928
        public void GetDSXSDNDG()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT A.SEQNO,D.HISCODE,
	                           F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
	                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
	                           A.XSRQ,
	                           B.GDSEQ,
	                           B.GDNAME,
	                           B.GDSPEC,
	                           F_GETPRINTINF(B.GDSEQ, 'SL',C.SL) BZHL, 
	                           --B.BZHL,
	                           F_GETPRINTINF(B.GDSEQ, 'HSJJ', 0) HSJJ, 
	                           --B.HSJJ,
	                           --F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
	                           B.ROWNO,
	                           B.STR2,
	                           TO_CHAR(A.SHRQ, 'YYYY-MM-DD') DJRQ,
	                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
	                           F_GETPRINTINF(B.GDSEQ, 'UNITNAME', 0) UNITNAME,
	                           --f_getunitname(B.UNIT) UNITNAME,
	                           C.HSJE HSJE,
	                           F_GETUSERNAME(A.LRY) ZDY,
	                           F_GETPARA('SUPPER') || '随货同行单(定数)' DT,
	                           C.PH,
                               B.PZWH,
                               C.PSSID,
                               F_GETSUPNAME(C.PSSID) PSSNAME,
	                           B.YXQZ,
	                           --(SELECT SUM(HSJE) FROM DAT_CK_COM WHERE SEQNO = '{0}' AND NVL(FPSL,0) > 0) SUMHJJE,
(SELECT SUM(B.HSJE) FROM DAT_CK_COM A,DAT_GOODSJXC B 
                                WHERE A.SEQNO = '{0}' 
                                AND A.ROWNO = B.ROWNO
                                AND A.SEQNO=B.BILLNO
                                AND B.KCADD = 1
                                AND B.SUPID <> '00001'
                                AND NVL(A.FPSL,0) > 0) SUMHJJE,
	                           F_GETPRINTINF(B.GDSEQ, 'HSJJ', 0) || '元/' || F_GETPRINTINF(B.GDSEQ, 'UNITNAME', 0) V_HSJJ,
	                           (select CODE_HIS from sys_dept where code = A.DEPTID) KJCODE,--天津，会计编码
	                           F_GETHWID(A.DEPTOUT,B.GDSEQ) HWID ,b.FPSL / b.BZHL CH     
                          FROM DAT_CK_DOC A, DAT_CK_COM B, DOC_GOODS D,
                               (SELECT BILLNO,ROWNO,PH,YXQZ,SUM(SL) SL,SUM(HSJE) HSJE ,PSSID
                               FROM DAT_GOODSJXC 
                               WHERE KCADD = 1 AND PSSID <> '00001'
                               GROUP BY BILLNO,ROWNO,PH,YXQZ,PSSID) C
                         WHERE A.SEQNO = B.SEQNO 
                            AND B.GDSEQ = D.GDSEQ
	                        AND B.SEQNO = C.BILLNO 
	                        AND B.ROWNO = C.ROWNO
	                        and (b.FPSL / b.BZHL) >0 
	                        AND A.SEQNO = '{0}'
                            --GROUP BY A.SEQNO,A.DEPTOUT,A.DEPTID,A.XSRQ,A.SHRQ,A.LRY, 
                              --B.SEQNO,B.GDSEQ, B.GDNAME, B.GDSPEC,B.PRODUCER,B.ROWNO,B.STR2,B.PH,B.YXQZ,b.FPSL,b.BZHL,
                              --C.BILLNO,C.SUPID,C.PH,D.HISCODE
	                        ORDER BY B.SEQNO,C.PSSID,B.ROWNO", osid);
            OracleReportData.GenNodeXmlData(this, sql.ToString(), false);
        }
        public void GetDSXSDYDG()
        {
            //F_GETPARA('SUPPER')
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT A.SEQNO,D.HISCODE,
	                           F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
	                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
	                           A.XSRQ,
	                           B.GDSEQ,
	                           B.GDNAME,
	                           B.GDSPEC,
	                           F_GETPRINTINF(B.GDSEQ, 'SL', C.SL) BZHL, 
	                           --B.BZHL,
	                           --F_GETPRINTINF(B.GDSEQ, 'HSJJ', 0) HSJJ, 
                               --0 HSJJ,
	                           B.HSJJ,
	                           --F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
	                           B.ROWNO,
	                           B.STR2,
	                           TO_CHAR(A.SHRQ, 'YYYY-MM-DD') DJRQ,
	                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
	                           F_GETPRINTINF(B.GDSEQ, 'UNITNAME', 0) UNITNAME,
	                           --f_getunitname(B.UNIT) UNITNAME,
	                           C.HSJE,
                               --0 HSJE,
	                           F_GETUSERNAME(A.LRY) ZDY,
	                           F_GETPARA('SUPPER') || '随货同行单(定数代管)' DT,
	                           C.PH,
                               C.PSSID,
                               f_getsupname(C.PSSID) PSSNAME,
                               B.PZWH,
	                           B.YXQZ,
	                           (SELECT SUM(HSJE) FROM DAT_CK_COM WHERE SEQNO = '{0}' AND NVL(FPSL,0) > 0) SUMHJJE,
	                           F_GETPRINTINF(B.GDSEQ, 'HSJJ', 0) || '元/' || F_GETPRINTINF(B.GDSEQ, 'UNITNAME', 0) V_HSJJ,
	                           (select CODE_HIS from sys_dept where code = A.DEPTID) KJCODE,--天津，会计编码
	                           F_GETHWID(A.DEPTOUT,B.GDSEQ) HWID ,b.FPSL / b.BZHL CH     
                          FROM DAT_CK_DOC A, DAT_CK_COM B, DOC_GOODS D,(SELECT BILLNO, ROWNO, PH, YXQZ, SUM(SL) SL, SUM(HSJE) HSJE, PSSID
                                       FROM DAT_GOODSJXC
                                      WHERE KCADD = 1
                                        AND PSSID = '00001'
                                   GROUP BY BILLNO, ROWNO, PH, YXQZ, PSSID) C
                         WHERE A.SEQNO = B.SEQNO 
	                        AND B.SEQNO = C.BILLNO
                            AND B.GDSEQ = D.GDSEQ
	                        AND B.ROWNO = C.ROWNO
	                        and (b.FPSL / b.BZHL) >0 
	                        AND A.SEQNO = '{0}'
	                        ORDER BY B.SEQNO,c.pssid,B.ROWNO", osid);
            OracleReportData.GenNodeXmlData(this, sql.ToString(), false);
        }
        public void GetDSXSD()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT A.SEQNO,D.HISCODE,
	                           F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
	                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
	                           A.XSRQ,
	                           B.GDSEQ,
	                           B.GDNAME,
	                           B.GDSPEC,
	                           F_GETPRINTINF(B.GDSEQ, 'SL',C.SL) BZHL, 
	                           --B.BZHL,
	                           F_GETPRINTINF(B.GDSEQ, 'HSJJ', 0) HSJJ, 
	                           --B.HSJJ,
	                           --F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
	                           B.ROWNO,
	                           B.STR2,
	                           TO_CHAR(A.SHRQ, 'YYYY-MM-DD') DJRQ,
	                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
	                           F_GETPRINTINF(B.GDSEQ, 'UNITNAME', 0) UNITNAME,
	                           --f_getunitname(B.UNIT) UNITNAME,
	                           C.HSJE HSJE,
	                           F_GETUSERNAME(A.LRY) ZDY,
	                           F_GETPARA('SUPPER') || '医用耗材出库单（定数）'||DECODE(C.PSSID,'00001','(代管)') DT,
	                           C.PH,
                               B.PZWH,
                               C.PSSID,
                               F_GETSUPNAME(C.PSSID) PSSNAME,
	                           B.YXQZ,
	                           --(SELECT SUM(HSJE) FROM DAT_CK_COM WHERE SEQNO = '{0}' AND NVL(FPSL,0) > 0) SUMHJJE,
                (SELECT SUM(B.HSJE) FROM DAT_CK_COM A,DAT_GOODSJXC B 
                                WHERE A.SEQNO = '{0}' 
                                AND A.ROWNO = B.ROWNO
                                AND A.SEQNO=B.BILLNO
                                AND B.KCADD = 1
                                --AND B.SUPID <> '00001'
                                AND NVL(A.FPSL,0) > 0) SUMHJJE,
	                           F_GETPRINTINF(B.GDSEQ, 'HSJJ', 0) || '元/' || F_GETPRINTINF(B.GDSEQ, 'UNITNAME', 0) V_HSJJ,
	                           (select CODE_HIS from sys_dept where code = A.DEPTID) KJCODE,--天津，会计编码
	                           F_GETHWID(A.DEPTOUT,B.GDSEQ) HWID ,b.FPSL / b.BZHL CH     
                          FROM DAT_CK_DOC A, DAT_CK_COM B, DOC_GOODS D,
                               (SELECT BILLNO,ROWNO,PH,YXQZ,SUM(SL) SL,SUM(HSJE) HSJE ,PSSID
                               FROM DAT_GOODSJXC 
                               WHERE KCADD = 1 --AND PSSID <> '00001'
                               GROUP BY BILLNO,ROWNO,PH,YXQZ,PSSID) C
                         WHERE A.SEQNO = B.SEQNO 
                            AND B.GDSEQ = D.GDSEQ
	                        AND B.SEQNO = C.BILLNO 
	                        AND B.ROWNO = C.ROWNO
	                        and (b.FPSL / b.BZHL) >0 
	                        AND A.SEQNO = '{0}'
                            --GROUP BY A.SEQNO,A.DEPTOUT,A.DEPTID,A.XSRQ,A.SHRQ,A.LRY, 
                              --B.SEQNO,B.GDSEQ, B.GDNAME, B.GDSPEC,B.PRODUCER,B.ROWNO,B.STR2,B.PH,B.YXQZ,b.FPSL,b.BZHL,
                              --C.BILLNO,C.SUPID,C.PH,D.HISCODE
	                        ORDER BY B.SEQNO,C.PSSID,B.ROWNO", osid);
            OracleReportData.GenNodeXmlData(this, sql.ToString(), false);
        }
        public void GetDSData_echo()
        {
            //定数条码重打
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select a.*,to_char(OUTRQ,'YYYY-MM-DD') DATE_ck,nvl(c.HISNAME,c.gdname) gdname,c.gdspec,nvl(c.HISNAME,c.gdname)||' - '||c.gdspec gd,F_GETPRODUCERNAME(c.producer) producername,F_GETDEPTNAME(a.deptin) deptidname,
--(rowno||'/'||(select count(1) from DAT_GOODSDS_LOG b where b.seqno =a.seqno and b.gdseq=a.gdseq)||'/'||(select DSNUM from DOC_GOODSCFG d where d.GDSEQ = a.GDSEQ and d.DEPTID = a.DEPTIN)) numb  
F_GETBILLGDNO(SEQNO,ROWNO,a.gdseq,A.DEPTIN) numb
                        from DAT_GOODSDS_LOG a,doc_goods c where a.gdseq = c.gdseq  AND A.BARCODE in ({0})";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetData_echo()
        {
            //非定数条码重打
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.*,GDNAME||' - '||GDSPEC NAMESPEC,TO_CHAR(INS_TIME,'YYYY-MM-DD') TI,F_GETPRODUCERNAME(A.PRODUCER) producername,F_GETDEPTNAME(a.DEPTIN) dept,F_GETDEPTNAME(a.DEPTOUT) deptOUT
                        FROM dat_ck_barcode A  where A.BARCODE in ({0})";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetData_F()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.*,A.GDNAME || ' - ' || A.GDSPEC NAMESPEC,
                                             TO_CHAR(A.INS_TIME, 'YYYY-MM-DD') TI,
                                             F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                             F_GETDEPTNAME(A.DEPTIN) DEPT,
                                             F_GETDEPTNAME(A.DEPTOUT) DEPTOUT
                                   FROM DAT_CK_BARCODE A
                                 WHERE A.SEQNO IN ('{0}')
                                 ORDER BY A.SEQNO";

            //string sql = @"SELECT A.*,A.GDNAME || ' - ' || A.GDSPEC NAMESPEC,
            //                                 TO_CHAR(A.INS_TIME, 'YYYY-MM-DD') TI,
            //                                 F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
            //                                 F_GETDEPTNAME(A.DEPTIN) DEPT,
            //                                 F_GETDEPTNAME(A.DEPTOUT) DEPTOUT
            //                       FROM DAT_CK_BARCODE A,DAT_CK_COM B
            //                     WHERE A.SEQNO=B.SEQNO AND A.GDSEQ=B.GDSEQ AND A.SEQNO IN ('{0}')
            //                     ORDER BY A.SEQNO, B.ROWNO";
            osid = osid.Replace(",", "','");
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetPDData()
        {
            //盘点
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,
                                               (CASE
                                                 WHEN A.MEMO IS NOT NULL THEN
                                                  DECODE(C.TYPE,
                                                         '3',
                                                         C.NAME,
                                                         '4',
                                                         C.NAME,
                                                         C.NAME || '（' || A.MEMO || '）')
                                                 ELSE
                                                  C.NAME
                                               END) DEPTIDNAME,
                                               A.PDPLAN,
                                               A.PDRQ,
                                               F_GETUSERNAME(A.LRY) LRYNAME,
                                               A.SUBNUM,
                                               B.HSJJ,
                                               B.ROWNO,
                                               B.GDSEQ,
                                               B.GDNAME,
                                               F_GETUNITNAME(B.UNIT) UNITNAME,
                                               B.GDSPEC,
                                               B.CDID,
                                               B.BZHL,
                                               B.PH,
                                               B.RQ_SC,
                                               B.KCSL,
                                               B.KCSL * B.HSJJ KCJE,
                                               B.BHSJJ KSKCSL,
                                               B.BHSJJ * B.HSJJ KSKCJE,
                                               B.PZWH,
                                               B.HWID,
                                               B.CATID,
                                               TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                                               YXQZ,
                                               F_GETDEPTNAME(F_GETPARA('DEFDEPT')) ||
                                               SUBSTR(TO_CHAR(A.PDRQ, 'YYYY-MM-DD'), 0, 4) || '年' ||
                                               SUBSTR(TO_CHAR(A.PDRQ, 'YYYY-MM-DD'), 6, 2) || '月份盘点表' HISTROY
                                          FROM DAT_PD_DOC A, DAT_PD_COM B, SYS_DEPT C
                                         WHERE A.SEQNO = B.SEQNO
                                           AND A.DEPTID = C.CODE
                                           AND A.SEQNO = '{0}'
                                         ORDER BY B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetData_Fcksld()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString().Replace("_", "','");
            string title = "随货同行单（非定数直出）";

            string sql = @"SELECT A.STR2 SEQNO,D.HISCODE,
                                           F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                           A.XSRQ,
                                           B.GDSEQ,
                                           B.GDNAME,
                                           B.UNIT,
                                           B.GDSPEC,
                                           B.BZHL,
                                           B.HSJJ,
                                           --F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                           B.ROWNO,
                                           TO_CHAR(A.XSRQ, 'YYYY-MM-DD') DJRQ,
                                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                                           F_GETUNITNAME(B.UNIT) UNITNAME,
                                           --B.HSJE,
                                           ABS(C.HSJE) HSJE,
                                           F_GETUSERNAME(A.LRY) ZDY,
                                           B.HSJJ || '元/' || F_GETUNITNAME(B.UNIT) V_HSJJ,
                                           C.PH,
                                           C.YXQZ,
                                           ABS(C.SL) XSSL,
                                           ABS(C.SL) / B.BZHL BZSL,
                                           C.PSSID,
                                           F_GETSUPNAME(C.PSSID) PSSNAME,
                                           B.PZWH,
                                           F_GETPARA('SUPPER') || '{1}' DT,
                                           B.HWID,
                                           --B.STR2,
                                           A.SEQNO CKDH,
                                           (select CODE_HIS from sys_dept where code = A.DEPTID) KJCODE,
                                             (SELECT SUM(JXC.HSJE) FROM DAT_CK_COM COM,DAT_GOODSJXC JXC
                                WHERE  COM.ROWNO = JXC.ROWNO
                                AND COM.SEQNO=JXC.BILLNO
                                AND JXC.KCADD = 1
                                AND JXC.SUPID <> '00001'
                                AND JXC.BILLNO=A.SEQNO) SUMHJJE
                                      FROM DAT_CK_DOC A, DAT_CK_COM B, DOC_GOODS D,
                                           (SELECT BILLNO,ROWNO,PSSID,PH,YXQZ,SUM(SL) SL,SUM(HSJE) HSJE 
                                      FROM DAT_GOODSJXC WHERE KCADD=1 AND BILLNO in ('{0}') AND PSSID <> '00001' 
                                      GROUP BY BILLNO,ROWNO,PSSID,PH,YXQZ) C
                                     WHERE A.SEQNO = B.SEQNO         
                                       AND B.GDSEQ=D.GDSEQ
                                       AND B.SEQNO = C.BILLNO
                                       AND B.ROWNO = C.ROWNO
                                       AND A.SEQNO in( '{0}')
                                       --AND A.SEQNO IN (SELECT STR2 FROM DAT_CK_DOC WHERE SEQNO in( '{0}'))
                                     ORDER BY B.SEQNO, C.PSSID"; ;

            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid, title), false);
        }
        //lvj 20170214 库房出库管理，高值直出（区分代管非代管）
        public void GetData_FcksldDG()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string title = "随货同行单（非定数直出代管）";

            string sql = @"SELECT A.STR2 SEQNO,D.HISCODE,
                                           F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                           A.XSRQ,
                                           B.GDSEQ,
                                           B.GDNAME,
                                           B.UNIT,
                                           B.GDSPEC,
                                           B.BZHL,
                                           B.HSJJ,
                                           --F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                           B.ROWNO,
                                           TO_CHAR(A.XSRQ, 'YYYY-MM-DD') DJRQ,
                                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                                           F_GETUNITNAME(B.UNIT) UNITNAME,
                                           --B.HSJE,
                                           ABS(C.HSJE) HSJE,
                                           F_GETUSERNAME(A.LRY) ZDY,
                                           B.HSJJ || '元/' || F_GETUNITNAME(B.UNIT) V_HSJJ,
                                           C.PH,
                                           C.YXQZ,
                                           ABS(C.SL) XSSL,
                                           ABS(C.SL) / B.BZHL BZSL,
                                           C.PSSID,
                                           F_GETSUPNAME(C.PSSID) PSSNAME,
                                           B.PZWH,
                                           F_GETPARA('SUPPER') || '{1}' DT,
                                           B.HWID,
                                           --B.STR2,
                                           '{0}' CKDH,
                                           (select CODE_HIS from sys_dept where code = A.DEPTID) KJCODE,
                                            (SELECT SUM(JXC.HSJE) FROM DAT_CK_COM COM,DAT_GOODSJXC JXC
                                WHERE  COM.ROWNO = JXC.ROWNO
                                AND COM.SEQNO=JXC.BILLNO
                                AND JXC.KCADD = 1
                                AND JXC.SUPID ='00001'
                                AND JXC.BILLNO=A.SEQNO) SUMHJJE
                                      FROM DAT_CK_DOC A, DAT_CK_COM B, DOC_GOODS D,
                                           (SELECT BILLNO,ROWNO,PSSID,PH,YXQZ,SUM(SL) SL,SUM(HSJE) HSJE 
                                      FROM DAT_GOODSJXC WHERE KCADD=1 AND BILLNO='{0}' AND PSSID = '00001' 
                                      GROUP BY BILLNO,ROWNO,PSSID,PH,YXQZ) C
                                     WHERE A.SEQNO = B.SEQNO AND B.GDSEQ=D.GDSEQ
                                       AND B.SEQNO = C.BILLNO
                                       AND B.ROWNO = C.ROWNO
                                       AND A.SEQNO = '{0}'
                                       --AND A.SEQNO IN (SELECT STR2 FROM DAT_CK_DOC WHERE SEQNO = '{0}')
                                     ORDER BY B.SEQNO, C.PSSID";

            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid, title), false);
        }
        public void GetData_FcksldN()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString().Replace("_", "','");
            string title = "随货同行单（非定数直出）";
            //F_GETPARA('SUPPER')
            string sql = @"SELECT A.STR2 SEQNO,D.HISCODE,
                                           F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                           A.XSRQ,
                                           B.GDSEQ,
                                           B.GDNAME,
                                           B.UNIT,
                                           B.GDSPEC,
                                           B.BZHL,
                                           B.HSJJ,
                                           --F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                           B.ROWNO,
                                           D.PIZNO,
                                           TO_CHAR(A.XSRQ, 'YYYY-MM-DD') DJRQ,
                                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                                           F_GETUNITNAME(B.UNIT) UNITNAME,
                                           --B.HSJE,
                                           ABS(C.HSJE) HSJE,
                                           F_GETUSERNAME(A.LRY) ZDY,
                                           B.HSJJ || '元/' || F_GETUNITNAME(B.UNIT) V_HSJJ,
                                           C.PH,
                                           C.YXQZ,
                                           ABS(C.SL) XSSL,
                                           ABS(C.SL) / B.BZHL BZSL,
                                           C.PSSID,
                                           F_GETSUPNAME(C.PSSID) PSSNAME,
                                           B.PZWH,
                                            F_GETPARA('SUPPER')|| '{1}'||decode(c.pssid, '00001', '代管', '') DT,
                                           B.HWID,
                                           B.STR2,
                                           B.SEQNO CKDH,
                                           (select CODE_HIS from sys_dept where code = A.DEPTID) KJCODE,
                                             A.SUBSUM,F_GETBILLCOUNT(A.SEQNO,'CKD',10) SUBNUM
                                      FROM DAT_CK_DOC A, DAT_CK_COM B, DOC_GOODS D,
                                           (SELECT BILLNO,ROWNO,PSSID,PH,YXQZ,SUM(SL) SL,SUM(HSJE) HSJE 
                                      FROM DAT_GOODSJXC WHERE KCADD=1 AND BILLNO in( '{0}')
                                      GROUP BY BILLNO,ROWNO,PSSID,PH,YXQZ) C
                                     WHERE A.SEQNO = B.SEQNO         
                                       AND B.GDSEQ=D.GDSEQ
                                       AND B.SEQNO = C.BILLNO
                                       AND B.ROWNO = C.ROWNO
                                       AND A.SEQNO in( '{0}')
                                       --AND A.SEQNO IN (SELECT STR2 FROM DAT_CK_DOC WHERE SEQNO in( '{0}'))
                                     ORDER BY B.SEQNO, C.PSSID,B.ROWNO"; ;
            //非代管不需要显示。string sql = @"SELECT A.STR2 SEQNO,D.HISCODE,
            //                               F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
            //                               F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
            //                               A.XSRQ,
            //                               B.GDSEQ,
            //                               B.GDNAME,
            //                               B.UNIT,
            //                               B.GDSPEC,
            //                               B.BZHL,
            //                               B.HSJJ,
            //                               --F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
            //                               B.ROWNO,
            //                               TO_CHAR(A.XSRQ, 'YYYY-MM-DD') DJRQ,
            //                               TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
            //                               F_GETUNITNAME(B.UNIT) UNITNAME,
            //                               --B.HSJE,
            //                               ABS(C.HSJE) HSJE,
            //                               F_GETUSERNAME(A.LRY) ZDY,
            //                               B.HSJJ || '元/' || F_GETUNITNAME(B.UNIT) V_HSJJ,
            //                               C.PH,
            //                               C.YXQZ,
            //                               ABS(C.SL) XSSL,
            //                               ABS(C.SL) / B.BZHL BZSL,
            //                               C.PSSID,
            //                               F_GETSUPNAME(C.PSSID) PSSNAME,
            //                               B.PZWH,
            //                                '山东威高讯通信息科技有限公司'|| '{1}'||decode(c.pssid, '00001', '代管', '非代管') DT,
            //                               B.HWID,
            //                               --B.STR2,
            //                               B.SEQNO CKDH,
            //                               (select CODE_HIS from sys_dept where code = A.DEPTID) KJCODE,
            //                                 A.SUBSUM
            //                          FROM DAT_CK_DOC A, DAT_CK_COM B, DOC_GOODS D,
            //                               (SELECT BILLNO,ROWNO,PSSID,PH,YXQZ,SUM(SL) SL,SUM(HSJE) HSJE 
            //                          FROM DAT_GOODSJXC WHERE KCADD=1 AND BILLNO in( '{0}')
            //                          GROUP BY BILLNO,ROWNO,PSSID,PH,YXQZ) C
            //                         WHERE A.SEQNO = B.SEQNO         
            //                           AND B.GDSEQ=D.GDSEQ
            //                           AND B.SEQNO = C.BILLNO
            //                           AND B.ROWNO = C.ROWNO
            //                           AND A.SEQNO in( '{0}')
            //                           --AND A.SEQNO IN (SELECT STR2 FROM DAT_CK_DOC WHERE SEQNO in( '{0}'))
            //                         ORDER BY B.SEQNO, C.PSSID,B.ROWNO"; ;

            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid, title), false);
        }
        public void GoodsReturn()
        {
            //科室退货
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select F_GETPARA('SUPPER')cusname,a.seqno,F_GETDEPTNAME(a.deptout) deptoutname,F_GETDEPTNAME(a.deptid) deptidname,to_char(a.xsrq,'YYYY-MM-DD') XSRQNAME,b.gdseq,b.gdname,b.unit,b.gdspec
            ,b.bzhl,b.hsjj,F_GETPRODUCERNAME(b.producer) producername,b.rowno,to_char(a.shrq,'YYYY-MM-DD') DJRQ,to_char(sysdate,'YYYY-MM-DD') printtime,F_GETUNITNAME(b.unit) unitname,-b.BZSL*b.hsjj HSJE,F_GETUSERNAME(a.lry) ZDY
            ,B.PH,B.YXQZ,B.XSSL,-B.BZSL BZSL,F_GETPARA('USERNAME')||'科室退货单' DT,B.HWID
            ,(SELECT SUM(-b.BZSL*b.hsjj) FROM dat_ck_com WHERE SEQNO = '{0}') sumhjje
            from dat_ck_doc a,dat_ck_com b where a.seqno = b.seqno and a.SEQNO = '{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGoodsNew()
        {
            //商品新增
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select 
       a.seqno,
       F_GETDEPTNAME(a.deptid) deptidname,
       a.lrytel,       
       F_GETUSERNAME(a.lry) ZDY,
       F_GETUSERNAME(a.spr) TAY,
       b.MEMOGOODS,
       b.sl,     
       decode(b.isnew,'Y','新增商品','已有商品') isnew, 
       b.gdname,
       b.unit,
       b.gdspec,
       b.hsjj,
       b.producer,
       b.rowno,
       to_char(sysdate, 'YYYY-MM-DD') printtime,
       to_char(a.lrrq, 'YYYY-MM-DD') lrrqtime,
       b.unit,
       b.SL * b.hsjj HSJE,
       
       F_GETPARA('SUPPER')|| '商品新增单' DT,
       (SELECT SUM(SL * hsjj) FROM dat_goodsnew_com WHERE SEQNO = '{0}' and ispass <> 'N' group by seqno) sumhjje
       
       ,a.subsum
       ,b.ispass
  from dat_goodsnew_doc a, dat_goodsnew_com b
 where a.seqno = b.seqno
   and a.SEQNO = '{0}'
   and b.ispass <> 'N'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetXSData()
        {
            //销售单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string flag = Request["flag"] == null ? "ph" : Request["flag"].ToString();
            string title = "医用耗材销售单";
            if (flag == "pj")
            {
                title = "设备维修（配件）出库单";
            }
            string sql = @"select A.SEQNO,
                                               A.DEPTID,
                                               F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                               A.LRY,
                                               F_GETUSERNAME(A.LRY) LRYNAME,
                                               B.ROWNO,
                                               B.GDSEQ,
                                               B.GDNAME,
                                               B.GDSPEC,
                                               B.PRODUCER,
                                               F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                               B.UNIT,
                                               F_GETUNITNAME(B.UNIT) UNITNAME,
                                               --B.BZSL,
                                               ABS(SL) BZSL,
                                               B.BZHL,
                                               B.HSJJ,
                                               ABS(CHSJE) HSJE,
                                               C.PH,
                                               B.RQ_SC,
                                               B.ZPBH,
                                               '合格' QUALITY,
                                               F_GETPARA('SUPPER')|| '{1}' DT,
                                               C.YXQZ,
                                               B.PZWH,
                                               F_GETPARA('SUPPER') cusname,
                                               TO_CHAR(A.XSRQ, 'YYYY-MM-DD') XSRQ,
                                               TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                                               --f_getsupname(G.SUPID) SUPNAME,
                                               f_getsupname(C.PSSID) PSSNAME,
                                               B.STR1,
                                               '合格' QUALITY,
                                               --F_GETPRINTINF(B.GDSEQ,'HSJJ',B.HSJJ) || '元/' || F_GETUNITNAME(B.UNIT) V_HSJJ,
                                               B.HSJJ|| '元/' ||F_GETUNITNAME(B.UNIT) V_HSJJ,
                                               --(SELECT PH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PHCK,
                                               --(SELECT PZWH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PZWHCK,
                                               F_GETHISTYPE(B.GDSEQ) LB,
                                               B.NUM1,
                                               (SELECT SUM(HSJE) FROM DAT_XS_COM WHERE SEQNO = '{0}') sumhjje
                                          from DAT_XS_DOC A, DAT_XS_COM B,(SELECT BILLNO, ROWNO, SUM(SL) SL, SUM(HSJE) CHSJE, PSSID,PH,YXQZ
                                                                             FROM DAT_GOODSJXC
                                                                             WHERE BILLNO = '{0}'
                                                                             AND KCADD = -1 GROUP BY BILLNO, ROWNO, PSSID,PH,YXQZ) C
                                         WHERE A.SEQNO = B.SEQNO
                                           AND B.ROWNO = C.ROWNO
                                           and B.SEQNO = C.BILLNO
                                           and A.SEQNO = '{0}'
                                        ORDER BY A.SEQNO,C.PSSID";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid, title), false);
        }
        public void GetXSDataBill()
        {
            //销售单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"select A.SEQNO,A.DEPTID,F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,A.LRY,F_GETUSERNAME(A.LRY) LRYNAME, ROWNO,GDNAME,GDSPEC,PRODUCER,F_GETPRODUCERNAME(PRODUCER) PRODUCERNAME
            //    ,UNIT,F_GETUNITNAME(UNIT) UNITNAME,BZSL,BZHL,HSJJ,HSJE,PH,RQ_SC,ZPBH,'合格' QUALITY,F_GETPARA('USERNAME')||'医用耗材入库单' DT,yxqz,PZWH,F_GETPARA('SUPPER') cusname
            //    ,TO_CHAR(XSRQ,'YYYY-MM-DD') XSRQ,TO_CHAR(SYSDATE,'YYYY-MM-DD') PRINTDATE,B.STR1,'合格' QUALITY,HSJJ||'元/'||F_GETUNITNAME(UNIT) V_HSJJ,
            //    (SELECT PH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PHCK,(SELECT PZWH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PZWHCK,F_GETHISTYPE(B.GDSEQ) LB,B.NUM1
            //    ,(SELECT SUM(HSJE) FROM DAT_XS_COM WHERE SEQNO = '{0}') sumhjje
            //    from DAT_XS_DOC A,DAT_XS_COM B WHERE A.SEQNO=B.SEQNO and a.SEQNO = '{0}'";   F_GETPARA('SUPPER')
            string sql = @"SELECT A.BILLNO,
                                               F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                               F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                               A.XSRQ,
                                               B.GDSEQ,
                                               D.BAR3 EASCODE,
                                               B.GDNAME,
                                               B.GDSPEC,
                                               ABS(C.SL) XSSL,
                                               B.HSJJ,
                                               --C.SUPID SUPPLIER,
                                               C.PSSID,
                                               --F_GETSUPNAME(C.SUPID) SUPNAME,
                                               f_getsupname(C.PSSID) PSSNAME,
                                               F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                               B.ROWNO,
                                               B.STR1 BARCODE,
                                               F_GETUNITNAME(B.UNIT) UNITNAME,
                                               ABS(C.CHSJE) HSJE,
                                               F_GETUSERNAME(A.LRY) ZDY,
                                              F_GETPARA('SUPPER') || '医疗物资销售单（定数）'||DECODE(C.PSSID,'00001','代管') DT,
                                               C.PH,
                                               C.YXQZ,
                                               --ABS(SUM(B.HSJJ * C.SL) OVER(PARTITION BY C.PSSID)) SUMHJJE
                                               (SELECT SUM(HSJE) FROM DAT_XS_COM WHERE SEQNO = '{0}') SUMHJJE,
                                               B.PZWH
                                          FROM DAT_XS_DOC A, DAT_XS_COM B, 
                                                DOC_GOODS D,
                                                (SELECT BILLNO, ROWNO, SUM(SL) SL, SUM(HSJE) CHSJE, PSSID, PH, YXQZ,GDSEQ
                                                 FROM DAT_GOODSJXC
                                                WHERE BILLNO IN ('{0}')
                                                AND KCADD = -1
                                                GROUP BY BILLNO, ROWNO, PSSID, PH, YXQZ,GDSEQ) C
                                         WHERE A.SEQNO = B.SEQNO
                                           AND B.SEQNO = C.BILLNO
                                           AND B.ROWNO = C.ROWNO
                                           AND B.GDSEQ = C.GDSEQ
                                           AND C.GDSEQ = D.GDSEQ
                                           AND A.FLAG = 'Y'
                                           AND A.SEQNO IN ('{0}')
                                         ORDER BY PSSNAME,ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGZXSDataBill()
        {
            //销售单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //F_GETPARA('SUPPER')
            string sql = @"SELECT A.BILLNO,A.STR9 NOTEID,
                                               F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                               A.XSRQ,
                                               --C.SUPID SUPPLIER,
                                               B.GDSEQ,
                                               B.GDNAME,
                                               B.GDSPEC,
                                               ABS(C.SL) XSSL,
                                               B.HSJJ,
                                               A.CUSTID PATIENT,
                                               A.STR7 PATIENTID,
                                               --F_GETSUPNAME(C.SUPID) SUPNAME,
                                               --F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                               B.ROWNO,
                                               B.ONECODE,
                                               ABS(C.HSJE) HSJE,
                                               C.PH,
                                               C.YXQZ,
                                               TO_CHAR(A.SHRQ, 'YYYY-MM-DD') DJRQ,
                                               SYSDATE PRINTDATE,
                                               F_GETUNITNAME(B.UNIT) UNITNAME,
                                               F_GETUSERNAME(A.LRY) ZDY,
                                              F_GETPARA('SUPPER') || '医疗物资销售单（高值）'||DECODE(C.PSSID,'00001','代管','') DT,
                                               SUM(C.HSJJ*ABS(C.SL)) OVER(PARTITION BY A.SEQNO) SUBSUM,
                                               B.PZWH,
                                               C.PSSID,
                                               F_GETSUPNAME(C.PSSID) PSSNAME
                                          FROM DAT_XS_DOC A, DAT_XS_COM B, (SELECT BILLNO, ROWNO, PH, YXQZ,PSSID,SUM(SL) SL, SUM(HSJE) HSJE,HSJJ
                                                            FROM DAT_GOODSJXC
                                                            GROUP BY BILLNO, ROWNO, PH, YXQZ,PSSID,HSJJ) C
                                         WHERE A.SEQNO = B.SEQNO
                                           AND B.SEQNO = C.BILLNO
                                           AND B.ROWNO = C.ROWNO
                                           AND A.SEQNO = '{0}'
                                         ORDER BY A.SEQNO, C.PSSID,B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetKDDataBill()
        {
            //销售单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"select A.SEQNO,A.DEPTID,F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,A.LRY,F_GETUSERNAME(A.LRY) LRYNAME, ROWNO,GDNAME,GDSPEC,PRODUCER,F_GETPRODUCERNAME(PRODUCER) PRODUCERNAME
            //    ,UNIT,F_GETUNITNAME(UNIT) UNITNAME,BZSL,BZHL,HSJJ,HSJE,PH,RQ_SC,ZPBH,'合格' QUALITY,F_GETPARA('USERNAME')||'医用耗材入库单' DT,yxqz,PZWH,F_GETPARA('SUPPER') cusname
            //    ,TO_CHAR(XSRQ,'YYYY-MM-DD') XSRQ,TO_CHAR(SYSDATE,'YYYY-MM-DD') PRINTDATE,B.STR1,'合格' QUALITY,HSJJ||'元/'||F_GETUNITNAME(UNIT) V_HSJJ,
            //    (SELECT PH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PHCK,(SELECT PZWH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PZWHCK,F_GETHISTYPE(B.GDSEQ) LB,B.NUM1
            //    ,(SELECT SUM(HSJE) FROM DAT_XS_COM WHERE SEQNO = '{0}') sumhjje
            //    from DAT_XS_DOC A,DAT_XS_COM B WHERE A.SEQNO=B.SEQNO and a.SEQNO = '{0}'";
            string sql = @"select A.SEQNO,
                           A.DEPTID,
                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                           A.LRY,
                           F_GETUSERNAME(A.LRY) LRYNAME,
                           ROWNO,
                           GDNAME,
                           GDSPEC,
                           PRODUCER,
                           F_GETPRODUCERNAME(PRODUCER) PRODUCERNAME,
                          -- F_GETPRINTINF(B.GDSEQ,'UNITNAME',0) UNITNAME,
                           F_GETUNITNAME(UNIT) UNITNAME,
                           BZSL,
                           F_GETPRINTINF(B.GDSEQ,'SL',B.BZHL) BZHL,--BZHL,
                           --F_GETPRINTINF(B.GDSEQ,'HSJJ',0) HSJJ,
                           HSJJ,
                           HSJE,
                           PH,
                           RQ_SC,
                           ZPBH,
                           '合格' QUALITY,
                           F_GETPARA('SUPPER')|| '科室商品调拨单' DT,
                           yxqz,
                           PZWH,
                           F_GETPARA('SUPPER') cusname,
                           TO_CHAR(XSRQ, 'YYYY-MM-DD') XSRQ,
                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                           B.STR1,
                           '合格' QUALITY,
                           HSJJ || '元/' || F_GETUNITNAME(UNIT) V_HSJJ,
                           B.PH PHCK,
                           B.PZWH PZWHCK,
                           F_GETHISTYPE(B.GDSEQ) LB,
                           B.NUM1,
                           (SELECT SUM(HSJE) FROM DAT_KD_COM WHERE SEQNO = '{0}') sumhjje
                      from DAT_KD_DOC A, DAT_KD_COM B
                     WHERE A.SEQNO = B.SEQNO and a.SEQNO = '{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }

        public void GetXSDNDG()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"select 
            //A.SEQNO,B.GDSEQ,A.DEPTID,F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,A.LRY,F_GETUSERNAME(A.LRY) LRYNAME, ROWNO,GDNAME,GDSPEC,PRODUCER,F_GETPRODUCERNAME(PRODUCER) PRODUCERNAME,UNIT,F_GETUNITNAME(UNIT) UNITNAME,BZSL,BZHL,
            //(case F_GETISDG(a.SEQNO, b.gdseq, b.ROWNO, a.deptid) when '代管' then 0 else HSJJ end) HSJJ,
            //(case F_GETISDG(a.SEQNO, b.gdseq, b.ROWNO, a.deptid) when '代管' then 0 else HSJE end) HSJE,
            //PH,RQ_SC,ZPBH,'合格' QUALITY,F_GETPARA('USERNAME')||'随货同行单' DT,yxqz,PZWH,F_GETPARA('SUPPER') cusname,TO_CHAR(XSRQ,'YYYY-MM-DD') XSRQ,TO_CHAR(SYSDATE,'YYYY-MM-DD') PRINTDATE,B.STR1,'合格' QUALITY,B.SEQNO,
            //    (SELECT PH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PHCK,(SELECT PZWH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PZWHCK,F_GETHISTYPE(B.GDSEQ) LB,B.NUM1,F_GETISDG(a.SEQNO, b.gdseq, b.ROWNO, a.deptid) ISDG,f_getdeptHWID(A.DEPTID,B.GDSEQ) HWID
            //    from DAT_XS_DOC A,DAT_XS_COM B WHERE A.SEQNO=B.SEQNO and a.SEQNO IN (SELECT STR2 FROM DAT_CK_DOC C WHERE C.SEQNO IN ({0})) AND F_GETISDG(a.SEQNO, b.gdseq, b.ROWNO, a.deptid) not in ('代管','高值')";

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"select A.SEQNO,
                                       B.GDSEQ,
(SELECT HISCODE FROM DOC_GOODS WHERE B.GDSEQ = GDSEQ) HISCODE,
                                       A.DEPTOUT,
                                       F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                       A.DEPTID,
                                       F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                       A.LRY,
                                       F_GETUSERNAME(A.LRY) LRYNAME,
                                       B.ROWNO,
                                       GDNAME,
                                       GDSPEC,
                                       PRODUCER,
                                       F_GETPRODUCERNAME(PRODUCER) PRODUCERNAME,
                                       --UNIT,
                                       F_GETPRINTINF(B.GDSEQ, 'UNITNAME', 0) UNITNAME,
                                       --ROUND(C.SL/B.BZHL,2) BZSL,
                                       F_GETPRINTINF(B.GDSEQ, 'SL',SUM(C.SL)) BZSL,
                                       F_GETPRINTINF(B.GDSEQ, 'HSJJ', 0) HSJJ,
                                       SUM(C.HSJE) HSJE,
                                       C.PH,
                                       C.RQ_SC,
                                       C.ZPBH,
                                       '合格' QUALITY,
                                       F_GETPARA('SUPPER') || '随货同行单' DT,
                                       C.yxqz,
                                       C.PZWH,
                                       F_GETPARA('SUPPER') cusname,
                                       TO_CHAR(XSRQ, 'YYYY-MM-DD') XSRQ,
                                       TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:mi:ss') PRINTDATE,
                                       B.STR1,
                                       '合格' QUALITY,
                                       B.SEQNO,
                                       (SELECT PH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PHCK,
                                       (SELECT PZWH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PZWHCK,
                                       F_GETHISTYPE(B.GDSEQ) LB,
                                       B.NUM1,
                                       F_GETISDG(a.SEQNO, b.gdseq, b.ROWNO, a.deptid) ISDG,
                                       --JXC.HWID
                                       f_gethwid(a.deptout,b.gdseq) hwid,
       (SELECT DECODE(ISJF,'Y','是','否') FROM DOC_GOODSCFG WHERE GDSEQ = B.GDSEQ AND DEPTID = A.DEPTID) ISJF
                                  from DAT_CK_DOC A,
                                       DAT_CK_COM B,
                                       DAT_GOODSJXC C
                                 WHERE A.SEQNO = B.SEQNO
                                   AND B.SEQNO = C.BILLNO
                                   AND B.ROWNO = C.ROWNO
                                   AND B.GDSEQ = C.GDSEQ
                                   AND C.KCADD = '1'
                                   AND C.PSSID IN (SELECT NVL(S.PSSID, S.SUPID)
                                                              FROM DOC_GOODSSUP S
                                                             WHERE S.TYPE IN ('0', 'Z') and S.GDSEQ=C.GDSEQ)
                                   and a.SEQNO IN ({0})                                   
                                    GROUP BY A.SEQNO,A.DEPTOUT,A.DEPTID,A.XSRQ,A.SHRQ,A.LRY, 
                              B.SEQNO,B.GDSEQ, B.GDNAME, B.GDSPEC,B.PRODUCER,B.ROWNO,B.STR2,B.PH,B.YXQZ,b.FPSL,b.BZHL,B.STR1,B.NUM1,
                              C.BILLNO,C.SUPID,C.PH,C.RQ_SC,C.ZPBH,C.yxqz,C.PZWH", osid);
            OracleReportData.GenNodeXmlData(this, sql.ToString(), false);
        }
        public void GetGZDataBill_Rtn()
        {
            //高值退货
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.GDSEQ, A.ROWNO,
                                   C.GDNAME,
                                   C.GDSPEC,
                                   A.STR2,
                                   B.SEQNO,
                                   TO_CHAR(THRQ, 'YYYY-MM-DD') THRQ,
                                   f_getdeptname(B.DEPTID) DEPTIDNAME,
                                   A.THSL,
                                   A.HSJJ,
                                   A.HSJE,
                                   A.PH,
                                   A.YXQZ,
                                   f_getusername(B.LRY) LRYNAME,
                                   C.HISCODE,
                                   A.PZWH,
                                   (SELECT SUM(HSJE) FROM DAT_TH_COM WHERE SEQNO = '{0}') SUMHJJE,
                                   F_GETUNITNAME(A.UNIT) UNITNAME,
                                   F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
                                   F_GETSUPNAME(B.PSSID) PSSNAME,
                                   f_getunitname(C.UNIT) UNITSMALLNAME,
                                   F_GETPARA('SUPPER')|| '销售退货单（高值）'||DECODE(B.PSSID,'00001','(代管)','') DT
                              FROM DAT_TH_COM A, DAT_TH_DOC B, DOC_GOODS C
                             WHERE A.SEQNO = B.SEQNO
                               AND A.SEQNO = '{0}'
                               AND A.GDSEQ = C.GDSEQ
                             ORDER BY A.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetXSDYDG()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"select 
            //A.SEQNO,B.GDSEQ,A.DEPTID,F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,A.LRY,F_GETUSERNAME(A.LRY) LRYNAME, ROWNO,GDNAME,GDSPEC,PRODUCER,F_GETPRODUCERNAME(PRODUCER) PRODUCERNAME,UNIT,F_GETUNITNAME(UNIT) UNITNAME,BZSL,BZHL,
            //(case F_GETISDG(a.SEQNO, b.gdseq, b.ROWNO, a.deptid) when '代管' then 0 else HSJJ end) HSJJ,
            //(case F_GETISDG(a.SEQNO, b.gdseq, b.ROWNO, a.deptid) when '代管' then 0 else HSJE end) HSJE,
            //PH,RQ_SC,ZPBH,'合格' QUALITY,F_GETPARA('USERNAME')||'随货同行单' DT,yxqz,PZWH,F_GETPARA('SUPPER') cusname,TO_CHAR(XSRQ,'YYYY-MM-DD') XSRQ,TO_CHAR(SYSDATE,'YYYY-MM-DD') PRINTDATE,B.STR1,'合格' QUALITY,B.SEQNO,
            //    (SELECT PH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PHCK,(SELECT PZWH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PZWHCK,F_GETHISTYPE(B.GDSEQ) LB,B.NUM1,F_GETISDG(a.SEQNO, b.gdseq, b.ROWNO, a.deptid) ISDG,f_getdeptHWID(A.DEPTID,B.GDSEQ) HWID
            //    from DAT_CK_DOC A,DAT_CK_COM B WHERE A.SEQNO=B.SEQNO and a.SEQNO IN ({0}) AND F_GETISDG(a.SEQNO, b.gdseq, b.ROWNO, a.deptid)IN('代管','高值')";
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"select A.SEQNO,
                                       B.GDSEQ,
                                        (SELECT HISCODE FROM DOC_GOODS WHERE B.GDSEQ = GDSEQ) HISCODE,
                                        A.DEPTOUT,
                                       F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                       A.DEPTID,
                                       F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                       A.LRY,
                                       F_GETUSERNAME(A.LRY) LRYNAME,
                                       B.ROWNO,
                                       GDNAME,
                                       GDSPEC,
                                       PRODUCER,
                                       F_GETPRODUCERNAME(PRODUCER) PRODUCERNAME,
                                       UNIT,
                                       F_GETUNITNAME(UNIT) UNITNAME,
                                       ROUND(JXC.SL/BZHL,2) BZSL,
                                       BZHL,
                                       (case B.ISGZ
                                         when 'N' then
                                          0
                                         else
                                          JXC.HSJJ
                                       end) HSJJ,
                                       (case B.ISGZ
                                         when 'N' then
                                          0
                                         else
                                          JXC.HSJE
                                       end) HSJE,
                                       JXC.HSJJ BGHSJJ,
                                       JXC.HSJE BGHSJE, 
                                       JXC.PH,
                                       JXC.RQ_SC,
                                       ZPBH,
                                       '合格' QUALITY,
                                       F_GETPARA('SUPPER') || '随货同行单(代管)' DT,
                                       JXC.yxqz,
                                       PZWH,
                                       F_GETPARA('SUPPER') cusname,
                                       TO_CHAR(XSRQ, 'YYYY-MM-DD') XSRQ,
                                       TO_CHAR(SYSDATE, 'YYYY-MM-DD HH24:mi:ss') PRINTDATE,
                                       B.STR1,
                                       '合格' QUALITY,
                                       B.SEQNO,
                                       (SELECT PH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PHCK,
                                       (SELECT PZWH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PZWHCK,
                                       F_GETHISTYPE(B.GDSEQ) LB,
                                       B.NUM1,
                                       F_GETISDG(a.SEQNO, b.gdseq, b.ROWNO, a.deptid) ISDG,
                                       --JXC.HWID
                                        f_gethwid(a.deptout,b.gdseq) hwid,
       (SELECT DECODE(ISJF,'Y','是','否') FROM DOC_GOODSCFG WHERE GDSEQ = B.GDSEQ AND DEPTID = A.DEPTID) ISJF
                                  from DAT_CK_DOC A,
                                       DAT_CK_COM B,
                                       (SELECT J.BILLNO,
                                               J.ROWNO,
                                               J.HWID,
                                               J.SL,
                                               J.HSJJ,
                                               J.HSJE,
                                               J.PH,
                                               J.YXQZ,
                                               J.RQ_SC,
                                               J.GDSEQ
                                          FROM DAT_GOODSJXC J
                                         WHERE J.BILLNO IN ({0})
                                           AND J.KCADD = '1'
                                           AND (J.PSSID = '00001' OR
                                               J.PSSID IN (SELECT NVL(S.PSSID, S.SUPID)
                                                              FROM DOC_GOODSSUP S
                                                             WHERE S.TYPE IN ('1') and S.GDSEQ=J.GDSEQ))) JXC
                                 WHERE A.SEQNO = B.SEQNO
                                   AND B.SEQNO = JXC.BILLNO
                                   AND B.ROWNO = JXC.ROWNO
                                   AND B.GDSEQ = JXC.GDSEQ
                                   and a.SEQNO IN ({0})", osid);
            OracleReportData.GenNodeXmlData(this, sql.ToString(), false);
        }
        public void GetXSDataBill_Rtn()
        {
            //销售退货单,非代管
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select A.SEQNO,
                           C.HISCODE,
                           A.DEPTID,
                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                           a.deptout,
                           F_GETDEPTNAME(A.deptout) DEPTOUTNAME,
                           A.LRY,
                           F_GETUSERNAME(A.LRY) LRYNAME,
                           B.ROWNO,
                           B.GDSEQ,
                           B.GDNAME,
                           B.GDSPEC,
                           B.PRODUCER,
                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                           B.UNIT,
                           F_GETUNITNAME(B.UNIT) UNITNAME,
                           --B.BZSL,
                           -NVL(F_GETXSSL(A.DEPTID, B.SEQNO, B.ROWNO, 'SUP'),F_GETXSSL(A.DEPTID, B.SEQNO, B.ROWNO, '')) BZSL,
                           B.BZHL,
                           B.HSJJ,
                           --B.HSJE,
                           -NVL(F_GETXSSL(A.DEPTID, B.SEQNO, B.ROWNO, 'SUP'),F_GETXSSL(A.DEPTID, B.SEQNO, B.ROWNO, '')) * B.HSJJ HSJE,
                           B.PH,
                           B.RQ_SC,
                           B.ZPBH,
                           '合格' QUALITY,
                           F_GETPARA('SUPPER')|| '销售退货单'||DECODE(D.PSSID,'00001',' 代管','') DT,
                           B.yxqz,
                           B.PZWH,
                           F_GETPARA('SUPPER') cusname,
                           TO_CHAR(XSRQ, 'YYYY-MM-DD') XSRQ,
                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                           B.STR1,
                           '合格' QUALITY,
                           B.HSJJ || '元/' || F_GETUNITNAME(B.UNIT) V_HSJJ,
                           (SELECT PH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PHCK,
                           (SELECT PZWH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PZWHCK,
                           F_GETHISTYPE(B.GDSEQ) LB,
                           B.NUM1,F_GETSUPNAME(D.PSSID)PSSNAME,
                           f_getcatname( C.catid) catname,--DECODE(d.pssid,'00001','代管','非代管') PSSID,
                           (SELECT SUM(HSJE) FROM DAT_GOODSJXC WHERE BILLNO = '{0}'  AND KCADD = '-1') sumhjje
                                    from DAT_XS_DOC A, DAT_XS_COM B ,DOC_GOODS C ,DAT_GOODSJXC D
                     WHERE A.SEQNO = B.SEQNO
                           and b.gdseq = c.gdseq
                           AND A.SEQNO = D.BILLNO
                           AND B.ROWNO = D.ROWNO
                           AND D.KCADD = '-1'
                           AND a.SEQNO = '{0}' order by d.pssid ";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
  public void GetXSDataBill_RtnGZ()
        {
            //销售退货单,非代管退至科室
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select A.SEQNO,
                           C.HISCODE,
                           A.DEPTID,
                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                           a.deptout,
                           F_GETDEPTNAME(A.deptout) DEPTOUTNAME,
                           A.LRY,
                           F_GETUSERNAME(A.LRY) LRYNAME,
                           B.ROWNO,
                           B.GDSEQ,
                           B.GDNAME,
                           B.GDSPEC,
                           B.PRODUCER,
                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                           B.UNIT,
                           F_GETUNITNAME(B.UNIT) UNITNAME,
                           --B.BZSL,
                           -D.SL BZSL,
                           B.BZHL,
                           B.HSJJ,
                           --B.HSJE,
                           -D.SL * B.HSJJ HSJE,
                           D.PH,
                           D.RQ_SC,
                           B.ZPBH,
                           '合格' QUALITY,
                           F_GETPARA('SUPPER')|| '销售退货单'||DECODE(D.PSSID,'00001',' 代管','') DT,
                           D.YXQZ,
                           D.PZWH,
                           F_GETPARA('SUPPER') cusname,
                           TO_CHAR(XSRQ, 'YYYY-MM-DD') XSRQ,
                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                           B.STR1,
                           '合格' QUALITY,
                           B.HSJJ || '元/' || F_GETUNITNAME(B.UNIT) V_HSJJ,
                           (SELECT PH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PHCK,
                           (SELECT PZWH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PZWHCK,
                           F_GETHISTYPE(B.GDSEQ) LB,
                           B.NUM1,F_GETSUPNAME(D.PSSID)PSSNAME,
                           f_getcatname( C.catid) catname,--DECODE(d.pssid,'00001','代管','非代管') PSSID,
                           (SELECT SUM(HSJE) FROM DAT_GOODSJXC WHERE BILLNO = '{0}'  AND KCADD = '1') sumhjje
                                    from DAT_XS_DOC A, DAT_XS_COM B ,DOC_GOODS C ,DAT_GOODSJXC D
                     WHERE A.SEQNO = B.SEQNO
                           and b.gdseq = c.gdseq
                           AND A.SEQNO = D.BILLNO
                           AND B.ROWNO = D.ROWNO
                           AND D.KCADD = '1'
                           AND a.SEQNO = '{0}' order by d.pssid ";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetXSDataBill_RtnDG()
        {
            //销售退货单,代管
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();

            string sql = @"select A.SEQNO,
                           C.HISCODE,
                           A.DEPTID,
                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                           a.deptout,
                           F_GETDEPTNAME(A.deptout) deptoutDNAME,
                           A.LRY,
                           F_GETUSERNAME(A.LRY) LRYNAME,
                           B.ROWNO,
                           B.GDSEQ,
                           B.GDNAME,
                           B.GDSPEC,
                           B.PRODUCER,
                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                           B.UNIT,
                           F_GETUNITNAME(B.UNIT) UNITNAME,
                           --B.BZSL,
                           -F_GETXSSL(A.DEPTID, B.SEQNO, B.ROWNO, 'YY') BZSL,
                           B.BZHL,
                           B.HSJJ,
                           --B.HSJE,
                           -F_GETXSSL(A.DEPTID, B.SEQNO, B.ROWNO, 'YY') * B.HSJJ HSJE,
                           B.PH,
                           B.RQ_SC,
                           B.ZPBH,
                           '合格' QUALITY,
                           F_GETPARA('SUPPER')|| '销售退货单(代管)' DT,
                           B.yxqz,
                           B.PZWH,
                           F_GETPARA('SUPPER') cusname,
                           TO_CHAR(XSRQ, 'YYYY-MM-DD') XSRQ,
                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                           B.STR1,F_GETSUPNAME(D.PSSID)PSSNAME,
                           '合格' QUALITY,
                           B.HSJJ || '元/' || F_GETUNITNAME(B.UNIT) V_HSJJ,
                           (SELECT PH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PHCK,
                           (SELECT PZWH FROM DAT_CK_COM C WHERE B.STR1 = C.STR2) PZWHCK,
                           F_GETHISTYPE(B.GDSEQ) LB,
                           B.NUM1,
                           f_getcatname( C.catid) catname,
                           (SELECT SUM(HSJE) FROM DAT_GOODSJXC WHERE BILLNO = '{0}' AND KCADD = '-1') sumhjje
                                    from DAT_XS_DOC A, DAT_XS_COM B ,DOC_GOODS C ,DAT_GOODSJXC D
                     WHERE A.SEQNO = B.SEQNO
                           and b.gdseq = c.gdseq
                           AND A.SEQNO = D.BILLNO
                           AND B.ROWNO = D.ROWNO
                           AND D.KCADD = '-1'
                           AND D.PSSID = '00001'
                           AND a.SEQNO = '{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);


        }
        public void GetData_JHDG()
        {
            //非定数拣货单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,
                       F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                       F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                       A.XSRQ,
                       B.GDSEQ,
                       B.GDNAME,
                       B.UNIT,
                       B.GDSPEC,
                       C.SL,A.SUBSUM,
                       B.HSJJ,
                       F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                       F_GETSUPNAME(C.PSSID)PSSNAME,
                       B.ROWNO,
                       TO_CHAR(A.SHRQ, 'YYYY-MM-DD') DJRQ,
                       TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                       F_GETUNITNAME(B.UNIT) UNITNAME,
                       C.HSJJ*C.SL HSJE,
                       F_GETUSERNAME(A.LRY) ZDY,
                       B.PH,
                       B.YXQZ,
                       --C.SL XSSL,
                       B.BZSL,
                       F_GETPARA('SUPPER')|| '拣货单' DT,
       
                       F_GETHWID(A.DEPTOUT, B.GDSEQ) HWID
                  FROM DAT_CK_DOC A, DAT_CK_COM B,DAT_GOODSJXC C
                 WHERE A.SEQNO = B.SEQNO AND A.BILLNO=C.BILLNO AND C.KCADD=1 AND B.ROWNO=C.ROWNO
                   AND A.SEQNO = '{0}' AND C.PSSID='00001'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetData_JHFDG()
        {
            //非定数拣货单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,
                       F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                       F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                       A.XSRQ,
                       B.GDSEQ,
                       B.GDNAME,
                       B.UNIT,
                       B.GDSPEC,
                       B.BZHL,
                       B.HSJJ,
                       F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                       F_GETSUPNAME(C.PSSID)PSSNAME,
                       B.ROWNO,
                       TO_CHAR(A.SHRQ, 'YYYY-MM-DD') DJRQ,
                       TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                       F_GETUNITNAME(B.UNIT) UNITNAME,
                       C.HSJJ*C.SL HSJE,
                       F_GETUSERNAME(A.LRY) ZDY,
                       B.PH,
                       B.YXQZ,
                       C.SL,A.SUBSUM,
                       B.BZSL,
                       F_GETPARA('SUPPER')|| '拣货单' DT,       
                       F_GETHWID(A.DEPTOUT, B.GDSEQ) HWID
                  FROM DAT_CK_DOC A, DAT_CK_COM B,DAT_GOODSJXC C
                 WHERE A.SEQNO = B.SEQNO AND A.BILLNO=C.BILLNO AND C.KCADD=1 AND B.ROWNO=C.ROWNO
                   AND A.SEQNO = '{0}' AND C.PSSID<>'00001'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetData_DsJh()
        {
            //定数退货单F_GETPARA('SUPPER')
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,
                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                           B.GDSEQ,
                           B.GDNAME,
                           B.GDSPEC,
                           F_GETPRINTINF(B.GDSEQ,'SL',B.BZHL) BZHL,--B.BZHL,
                           --F_GETPRINTINF(B.GDSEQ,'HSJJ',0) HSJJ,
                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                           B.ROWNO,
                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                           F_GETPRINTINF(B.GDSEQ,'UNITNAME',0) UNITNAME,
                           HSJE,
                           F_GETUSERNAME(A.LRY) ZDY,
                           B.BZSL,
                           F_GETPARA('SUPPER') || '拣货单(定数)' DT,
                           F_GETPARA('SUPPER')KH,
                           PH,
                           B.STR2,
                           B.YXQZ,
                           B.PZWH,
                           A.XSRQ,
                           --F_GETDEPTNAME(F_GETPARA('DEFDEPT')) DEPTINNAME,
                           F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                           (SELECT SUM(HSJE) FROM DAT_CK_COM WHERE SEQNO = '{0}') SUMHJJE,
                           --F_GETPRINTINF(B.GDSEQ,'HSJJ',0) || '元/' || F_GETPRINTINF(B.GDSEQ,'UNITNAME',0) V_HSJJ
                          B.HSJJ,B.HWID HWIDNAME
                      FROM DAT_CK_DOC A, DAT_CK_COM B
                     WHERE A.SEQNO = B.SEQNO AND A.SEQNO = '{0}'AND B.FPSL>0";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGoodsRtn_Ds()
        {
            //定数退货单 F_GETPARA('SUPPER')
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,
                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                           B.GDSEQ,
                           B.GDNAME,
                           B.GDSPEC,
                           --F_GETPRINTINF(B.GDSEQ,'SL',B.BZHL) BZHL,
                           B.BZHL,
                           --F_GETPRINTINF(B.GDSEQ,'HSJJ',0) HSJJ,
                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                           B.ROWNO,
                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTTIME,
                           --F_GETPRINTINF(B.GDSEQ,'UNITNAME',0) UNITNAME,
                           F_GETUNITNAME(B.UNIT) UNITNAME,
                           HSJE,
                           F_GETUSERNAME(A.LRY) ZDY,
                           B.BZSL,
                           F_GETPARA('SUPPER') || '科室退货单(定数)'||DECODE(C.PSSID,'00001','代管','') DT,
                           F_GETPARA('SUPPER')KH,
                           C.PH,
                           B.STR2,
                           A.XSRQ,
                           --F_GETDEPTNAME(F_GETPARA('DEFDEPT')) DEPTINNAME,
                           F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                           (SELECT SUM(HSJE) FROM DAT_CK_COM WHERE SEQNO = '{0}') SUMHJJE,
                           --F_GETPRINTINF(B.GDSEQ,'HSJJ',0) || '元/' || F_GETPRINTINF(B.GDSEQ,'UNITNAME',0) V_HSJJ
                          B.HSJJ V_HSJJ,
                          F_GETSUPNAME(C.PSSID) PSSNAME,
                          C.SL SL,
                          C.CHSJE,
                          B.PZWH
                      FROM DAT_CK_DOC A, DAT_CK_COM B,
                          (SELECT BILLNO,ROWNO,SUM(SL) SL,SUM(HSJE) CHSJE,PSSID,PH FROM DAT_GOODSJXC WHERE BILLNO='{0}' AND KCADD=-1 GROUP BY BILLNO,ROWNO,PSSID,PH) C
                     WHERE A.SEQNO = B.SEQNO AND A.SEQNO = '{0}' AND B.SEQNO=C.BILLNO AND B.ROWNO=C.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGoodsGT()
        {
            //跟台商品使用
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,A.STR1,f_getusername(A.SLR) SLRNAME,
                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                           B.GDSEQ,
                           B.GDNAME,
                           B.GDSPEC,
                           F_GETPRINTINF(B.GDSEQ,'SL',B.BZHL) BZHL, b.bzsl,--B.BZHL,
                           F_GETPRINTINF(B.GDSEQ,'HSJJ',0) HSJJ,
                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                           F_GETSUPNAME(C.PSSID)PSSNAME,
                           B.ROWNO,
                           TO_CHAR(SYSDATE, 'YYYY-MM-DD hh24:mi:ss') PRINTTIME,
                           F_GETPRINTINF(B.GDSEQ,'UNITNAME',0) UNITNAME,
                           B.HSJE,
                           F_GETUSERNAME(A.LRY) ZDY,
                           B.BZSL,
                           F_GETPRINTINF(B.GDSEQ,'SL',B.BZHL) BZHL,--B.BZHL,
                           F_GETPARA('SUPPER')|| '跟台商品使用单' DT,
                           F_GETPARA('SUPPER')KH,
                           C.PH,C.PZWH,DHSL,
                           TO_CHAR(B.YXQZ, 'YYYY-MM-DD') YXQZ,
                           TO_CHAR(B.RQ_SC, 'YYYY-MM-DD') RQ_SC,
                           B.STR2,
                           TO_CHAR(A.XSRQ, 'YYYY-MM-DD') XSRQ,
                           --F_GETDEPTNAME(F_GETPARA('DEFDEPT')) DEPTINNAME,
                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                           (SELECT SUM(HSJE) FROM DAT_CK_COM WHERE SEQNO = '{0}') SUMHJJE,
                           --F_GETPRINTINF(B.GDSEQ,'HSJJ',0) || '元/' || F_GETPRINTINF(B.GDSEQ,'UNITNAME',0) V_HSJJ
                            F_GETPRINTINF(B.GDSEQ,'HSJJ',0) V_HSJJ
                      FROM DAT_CK_DOC A, DAT_CK_COM B,DAT_GOODSJXC C
                     WHERE A.SEQNO = B.SEQNO AND A.SEQNO = '{0}'AND A.SEQNO=C.BILLNO AND B.ROWNO=C.ROWNO AND C.KCADD='-1'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void getYSBill()
        {
            //月度预算管理
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,
                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                           B.GDSEQ,
                           B.GDNAME,
                           B.GDSPEC,
                           B.XHSL,
                           B.YSSL,
                           F_GETPRINTINF(B.GDSEQ,'SL',B.BZHL) BZHL,--B.BZHL,
                           F_GETPRINTINF(B.GDSEQ,'HSJJ',0) HSJJ,
                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                           B.ROWNO,
                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTTIME,
                           F_GETPRINTINF(B.GDSEQ,'UNITNAME',0) UNITNAME,
                           HSJE,
                           F_GETUSERNAME(A.LRY) ZDY,
                           B.BZSL,
                           F_GETPRINTINF(B.GDSEQ,'SL',B.BZHL) BZHL,--B.BZHL,
                           F_GETPARA('SUPPER')|| '月度预算审批' DT,
                           F_GETPARA('SUPPER')KH,
                           B.STR2,
                           TO_CHAR(A.YSRQ, 'YYYY-MM') YSRQ,
                           TO_CHAR(A.LRRQ, 'YYYY-MM-DD') LRRQ,
                           --F_GETDEPTNAME(F_GETPARA('DEFDEPT')) DEPTINNAME,
                           (SELECT SUM(HSJE) FROM DAT_YS_COM WHERE SEQNO = '{0}') SUMHJJE,
                           F_GETPRINTINF(B.GDSEQ,'HSJJ',0) || '元/' || F_GETPRINTINF(B.GDSEQ,'UNITNAME',0) V_HSJJ
                      FROM DAT_YS_DOC A, DAT_YS_COM B
                     WHERE A.SEQNO = B.SEQNO AND A.SEQNO = '{0}'      ";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void getJHBill()
        {
            //月度预算管理
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,B.DHS,B.PZWH,to_char(A.XDRQ,'YYYY-MM') YSRQ,B.PH,
                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                           B.GDSEQ,
                           B.GDNAME,
                           B.GDSPEC,
                           F_GETPRINTINF(B.GDSEQ,'SL',B.BZHL) BZHL,--B.BZHL,
                           F_GETPRINTINF(B.GDSEQ,'HSJJ',0) HSJJ,
                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                           B.ROWNO,
                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTTIME,
                           F_GETPRINTINF(B.GDSEQ,'UNITNAME',0) UNITNAME,
                           HSJE,
                           F_GETUSERNAME(A.LRY) ZDY,
                           B.DHS BZSL,A.XDRQ YSRQ,
                           F_GETPRINTINF(B.GDSEQ,'SL',B.BZHL) BZHL,--B.BZHL,
                           F_GETPARA('SUPPER')|| '采购计划单' DT,
                           F_GETPARA('SUPPER')KH,
                           B.STR2,
                           TO_CHAR(A.LRRQ, 'YYYY-MM-DD') LRRQ,
                           --F_GETDEPTNAME(F_GETPARA('DEFDEPT')) DEPTINNAME,
                           (SELECT SUM(HSJE) FROM DAT_DDPLAN_COM WHERE SEQNO = '{0}') SUMHJJE,
                           --F_GETPRINTINF(B.GDSEQ,'HSJJ',0) || '元/' || F_GETPRINTINF(B.GDSEQ,'UNITNAME',0) V_HSJJ
                           F_GETPRINTINF(B.GDSEQ,'HSJJ',0) V_HSJJ
                      FROM DAT_DDPLAN_DOC A, DAT_DDPLAN_COM B
                     WHERE A.SEQNO = B.SEQNO AND A.SEQNO = '{0}'      ";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGoodsDb()
        {
            //随货通行单只有在已出库的状态下才能打印，已收货之后就不能打印
            //调拨随货通行单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"select a.seqno,F_GETDEPTNAME(a.deptout) deptoutname,F_GETDEPTNAME(a.deptid) deptidname,a.xsrq,b.gdseq,b.gdname,b.unit,b.gdspec,b.bzhl,B.PZWH,b.hsjj
            //    ,F_GETPRODUCERNAME(b.producer) producername,b.rowno,to_char(a.xsrq,'YYYY-MM-DD') DJRQ,to_char(sysdate,'YYYY-MM-DD') printdate,F_GETUNITNAME(b.unit) unitname,HSJE
            //    ,F_GETUSERNAME(a.lry) ZDY,B.PH,B.YXQZ,B.XSSL,B.BZSL,F_GETPARA('SUPPER')||'调拨单' DT
            //    ,(SELECT SUM(HSJE) FROM dat_db_com WHERE SEQNO = '{0}') sumhjje
            //    from dat_db_doc a,dat_db_com b where a.seqno = b.seqno and B.XSSL>0 AND a.SEQNO = '{0}'";
            string sql = @"SELECT M.SEQNO,M.DEPTOUTNAME,M.DEPTIDNAME,M.XSRQ,M.GDSEQ,M.GDNAME,M.UNIT,M.GDSPEC,M.BZHL,M.PZWH,M.HSJJ,
                            M.PRODUCERNAME,M.DJRQ,M.PRINTDATE,M.UNITNAME,SUM(M.HSJE)HSJE,M.DT,M.PH,M.ZDY,M.YXQZ,SUM(M.XSSL)XSSL,SUM(M.BZSL)BZSL,M.PSSNAME,
                            M.ISGZ,M.SUMHJJE
                            FROM(select a.seqno,
                                           F_GETDEPTNAME(a.deptout) deptoutname,
                                           F_GETDEPTNAME(a.deptid) deptidname,
                                           a.xsrq,
                                           b.gdseq,
                                           b.gdname,
                                           b.unit,
                                           b.gdspec,
                                           b.bzhl,
                                           B.PZWH,
                                           b.hsjj,
                                           F_GETPRODUCERNAME(b.producer) producername,
                                           b.rowno,
                                           to_char(a.xsrq, 'YYYY-MM-DD') DJRQ,
                                           to_char(sysdate, 'YYYY-MM-DD') printdate,
                                           F_GETUNITNAME(b.unit) unitname,
                                            B.XSSL*B.HSJJ HSJE,
                                           F_GETUSERNAME(a.lry) ZDY,
                                           B.PH,
                                           B.YXQZ,
                                           B.XSSL,
                                           B.BZSL,
                                           F_GETSUPNAME(C.PSSID) PSSNAME,
                                           b.ISGZ,
                                           F_GETPARA('SUPPER') || '调拨单' || DECODE(b.ISGZ, 'Y', '(高值)') ||
                                           DECODE(C.SUPID, '00001', '(代管)') DT,
                                           (SELECT SUM(HSJE) FROM dat_db_com WHERE SEQNO = '{0}' and xssl>0) sumhjje
                                      from dat_db_doc a,
                                           dat_db_com b,
                                           (select lk.lockbillno, lk.lockrowno, kc.pssid, kc.supid
                                              from dat_stocklock lk, dat_goodsstock kc
                                             where lk.picino = kc.picino
                                             GROUP BY LK.LOCKBILLNO, LK.LOCKROWNO, KC.PSSID, KC.SUPID) C
                                     where a.seqno = b.seqno
                                       AND B.SEQNO = C.LOCKBILLNO
                                       AND B.ROWNO = C.LOCKROWNO
                                       and B.XSSL > 0
                                       AND a.SEQNO = '{0}'
                                     ORDER BY B.ISGZ, C.PSSID, B.ROWNO) M
                                         GROUP BY M.SEQNO,M.DEPTOUTNAME,M.DEPTIDNAME,M.XSRQ,M.GDSEQ,M.GDNAME,M.UNIT,M.GDSPEC,M.BZHL,M.PZWH,M.HSJJ,
                                        M.PRODUCERNAME,M.DJRQ,M.PRINTDATE,M.DT,M.UNITNAME,M.PH,M.ZDY,M.YXQZ,M.PSSNAME,
                                        M.ISGZ,M.SUMHJJE
                                        ";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGoodsDbckjhd()
        {
            //调拨拣货单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"select a.seqno,F_GETDEPTNAME(a.deptout) deptoutname,F_GETDEPTNAME(a.deptid) deptidname,a.xsrq,b.gdseq,b.gdname,b.unit,b.gdspec,b.bzhl,B.PZWH,b.hsjj
            //    ,F_GETPRODUCERNAME(b.producer) producername,b.rowno,to_char(a.xsrq,'YYYY-MM-DD') DJRQ,to_char(sysdate,'YYYY-MM-DD') printdate,F_GETUNITNAME(b.unit) unitname,HSJE
            //    ,F_GETUSERNAME(a.lry) ZDY,B.PH,B.YXQZ,B.XSSL,B.BZSL,F_GETPARA('SUPPER')||'调拨单' DT
            //    ,(SELECT SUM(HSJE) FROM dat_db_com WHERE SEQNO = '{0}') sumhjje
            //    from dat_db_doc a,dat_db_com b where a.seqno = b.seqno and B.XSSL>0 AND a.SEQNO = '{0}'";
            string sql = @"select a.seqno,
                                           F_GETDEPTNAME(a.deptout) deptoutname,
                                           F_GETDEPTNAME(a.deptid) deptidname,
                                           a.xsrq,
                                           b.gdseq,
                                           b.gdname,
                                           b.unit,
                                           b.gdspec,
                                           b.bzhl,
                                           B.PZWH,
                                           b.hsjj,
                                           F_GETPRODUCERNAME(b.producer) producername,
                                           b.rowno,
                                           to_char(a.xsrq, 'YYYY-MM-DD') DJRQ,
                                           to_char(sysdate, 'YYYY-MM-DD') printdate,
                                           F_GETUNITNAME(b.unit) unitname,
                                           B.HSJE,
                                           F_GETUSERNAME(a.lry) ZDY,
                                           B.PH,
                                           B.YXQZ,
                                           B.XSSL,
                                           B.BZSL,
                                           F_GETSUPNAME(C.PSSID) PSSNAME,
                                           b.ISGZ,
                                           F_GETPARA('SUPPER') || '调拨单' || DECODE(b.ISGZ, 'Y', '(高值)') ||
                                           DECODE(C.PSSID, '00001', '(代管)') DT,
                                           (SELECT SUM(HSJE) FROM dat_db_com WHERE SEQNO = '{0}') sumhjje
                                      from dat_db_doc a, dat_db_com b, DAT_GOODSJXC C--dat_stocklock C
                                     where a.seqno = b.seqno
                                       --AND B.SEQNO = C.LOCKBILLNO
                                       --AND B.ROWNO = C.LOCKROWNO
                                       AND B.SEQNO=C.BILLNO
                                       AND B.ROWNO=C.ROWNO
                                       AND C.KCADD='1'
                                       and B.XSSL > 0
                                       AND a.SEQNO = '{0}'
                                     ORDER BY B.ISGZ, C.PSSID, B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }

        public void GetGoodsDbrksjd()
        {
            //调拨入库上架单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"select a.seqno,F_GETDEPTNAME(a.deptout) deptoutname,F_GETDEPTNAME(a.deptid) deptidname,a.xsrq,b.gdseq,b.gdname,b.unit,b.gdspec,b.bzhl,B.PZWH,b.hsjj
            //    ,F_GETPRODUCERNAME(b.producer) producername,b.rowno,to_char(a.xsrq,'YYYY-MM-DD') DJRQ,to_char(sysdate,'YYYY-MM-DD') printdate,F_GETUNITNAME(b.unit) unitname,HSJE
            //    ,F_GETUSERNAME(a.lry) ZDY,B.PH,B.YXQZ,B.XSSL,B.BZSL,F_GETPARA('SUPPER')||'调拨单' DT
            //    ,(SELECT SUM(HSJE) FROM dat_db_com WHERE SEQNO = '{0}') sumhjje
            //    from dat_db_doc a,dat_db_com b where a.seqno = b.seqno and B.XSSL>0 AND a.SEQNO = '{0}'";
            string sql = @"select a.seqno,
                                   F_GETDEPTNAME(a.deptout) deptoutname,
                                   F_GETDEPTNAME(a.deptid) deptidname,
                                   a.xsrq,
                                   b.gdseq,
                                   b.gdname,
                                   b.unit,
                                   b.gdspec,
                                   b.bzhl,
                                   B.PZWH,
                                   b.hsjj,
                                   F_GETPRODUCERNAME(b.producer) producername,
                                   b.rowno,
                                   to_char(a.xsrq, 'YYYY-MM-DD') DJRQ,
                                   to_char(sysdate, 'YYYY-MM-DD') printdate,
                                   F_GETUNITNAME(b.unit) unitname,
                                   ABS(C.HSJE)HSJE,
                                   F_GETUSERNAME(a.lry) ZDY,
                                   B.PH,
                                   B.YXQZ,
                                   B.XSSL,
                                   B.BZSL,
                                   F_GETSUPNAME(C.PSSID) PSSNAME,b.ISGZ,
                                   F_GETPARA('SUPPER') || '入库上架单'||DECODE(b.ISGZ,'Y','(高值)')||DECODE(C.PSSID,'00001','(代管)') DT,
                            (SELECT SUM(HSJE) FROM dat_db_com WHERE SEQNO = '{0}') sumhjje
                              from dat_db_doc a, dat_db_com b,DAT_GOODSJXC C
                             where a.seqno = b.seqno AND B.SEQNO=C.BILLNO AND B.ROWNO=C.ROWNO AND C.KCADD=-1
                               and B.XSSL > 0
                               AND a.SEQNO = '{0}' ORDER BY B.ISGZ,C.PSSID,B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGoodsDbjhd()
        {
            //调拨出库拣货单 F_GETPARA('SUPPER')
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"select DISTINCT a.seqno,F_GETDEPTNAME(a.deptout) deptoutname,F_GETDEPTNAME(a.deptid) deptidname,a.xsrq,DHSL,b.gdseq,b.gdname,b.unit,b.gdspec,b.bzhl,b.hsjj,b.pzwh,F_GETPRODUCERNAME(b.producer) producername,b.rowno,to_char(a.shrq,'YYYY-MM-DD') DJRQ,to_char(sysdate,'YYYY-MM-DD') printdate,F_GETUNITNAME(b.unit) unitname,b.XSSL*b.hsjj HSJE,F_GETUSERNAME(a.lry) ZDY,B.PH,B.YXQZ,B.XSSL,B.BZSL,F_GETPARA('SUPPER')||'拣货单' DT,B.HWID,(select sum(hsje)from dat_db_com where seqno=a.seqno)sumhjje
            //    from dat_db_doc a,dat_db_com b ,DAT_GOODSJXC T where a.seqno = b.seqno  AND A.SEQNO=T.BILLNO AND a.SEQNO = '{0}' AND XSSL >0 AND T.PSSID<>'00001' ORDER BY B.HWID";
            string sql = @"select DISTINCT a.seqno,
                                    F_GETDEPTNAME(a.deptout) deptoutname,
                                    F_GETDEPTNAME(a.deptid) deptidname,
                                    a.xsrq,
                                    DHSL,
                                    b.gdseq,
                                    b.gdname,
                                    b.unit,
                                    b.gdspec,
                                    b.bzhl,
                                    b.hsjj,
                                    b.pzwh,
                                    F_GETPRODUCERNAME(b.producer) producername,
                                    b.rowno,
                                    to_char(a.shrq, 'YYYY-MM-DD') DJRQ,
                                    to_char(sysdate, 'YYYY-MM-DD') printdate,
                                    F_GETUNITNAME(b.unit) unitname,
                                    b.XSSL * b.hsjj HSJE,
                                    F_GETUSERNAME(a.lry) ZDY,
                                    B.PH,
                                    B.YXQZ,
                                    B.XSSL,
                                    B.BZSL,
                                    F_GETPARA('SUPPER') || '拣货单'||nvl(b.str1, '(高值)')||DECODE(T.PSSID,'00001','(代管)') DT,
                                    B.HWID,
                                    (select sum(hsje) from dat_db_com where seqno = a.seqno) sumhjje,
                                    d.hiscode,
                                    A.SUBSUM,
                                    F_GETSUPNAME(T.PSSID) PSSNAME,
                                    T.PSSID,
                                    b.isgz,
                                    b.rowno,
                                    B.STR1 BARCODE
                      from dat_db_doc a, dat_db_com b, DAT_GOODSJXC T, doc_goods d
                     where a.seqno = b.seqno and b.gdseq = d.gdseq
                       AND B.SEQNO = T.BILLNO AND B.ROWNO=T.ROWNO
                       AND a.SEQNO = '{0}'
                       AND XSSL > 0
                     ORDER BY T.PSSID,B.ISGZ,b.rowno";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGoodsDbjh()
        {
            //调拨出库拣货单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"select DISTINCT a.seqno,F_GETDEPTNAME(a.deptout) deptoutname,F_GETDEPTNAME(a.deptid) deptidname,a.xsrq,DHSL,b.gdseq,b.gdname,b.unit,b.gdspec,b.bzhl,b.hsjj,b.pzwh,F_GETPRODUCERNAME(b.producer) producername,b.rowno,to_char(a.shrq,'YYYY-MM-DD') DJRQ,to_char(sysdate,'YYYY-MM-DD') printdate,F_GETUNITNAME(b.unit) unitname,b.XSSL*b.hsjj HSJE,F_GETUSERNAME(a.lry) ZDY,B.PH,B.YXQZ,B.XSSL,B.BZSL,F_GETPARA('SUPPER')||'拣货单' DT,B.HWID,(select sum(hsje)from dat_db_com where seqno=a.seqno)sumhjje
            //    from dat_db_doc a,dat_db_com b ,DAT_GOODSJXC T where a.seqno = b.seqno  AND A.SEQNO=T.BILLNO AND a.SEQNO = '{0}' AND XSSL >0 AND T.PSSID<>'00001' ORDER BY B.HWID";
            string sql = @"select DISTINCT a.seqno,
                            F_GETDEPTNAME(a.deptout) deptoutname,
                            F_GETDEPTNAME(a.deptid) deptidname,
                            a.xsrq,
                            B.BZSL DHSL,
                            b.gdseq,
                            b.gdname,
                            b.unit,
                            b.gdspec,
                            b.bzhl,
                            b.hsjj,
                            b.pzwh,
                            F_GETPRODUCERNAME(b.producer) producername,
                            b.rowno,
                            to_char(a.shrq, 'YYYY-MM-DD') DJRQ,
                            to_char(sysdate, 'YYYY-MM-DD') printdate,
                            F_GETUNITNAME(b.unit) unitname,
                            b.XSSL * b.hsjj HSJE,
                            F_GETUSERNAME(a.lry) ZDY,
                            B.PH,
                            B.YXQZ,
                            B.XSSL,
                            F_GETPARA('SUPPER') || '拣货单' ||
                            nvl2(b.str1, '(高值)', '(非高值)') DT,
                            B.HWID,
                            (select sum(hsje) from dat_db_com where seqno = a.seqno) sumhjje,
                            c.hiscode,
                            A.SUBSUM,
                            b.isgz,
                            b.rowno,
                            B.STR1 BARCODE
              from dat_db_doc a, dat_db_com b, doc_goods c
             where a.seqno = b.seqno
               and b.gdseq = c.gdseq
               AND a.SEQNO = '{0}'
             ORDER BY a.seqno, b.rowno";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGoodsDbjhDG()
        {
            //调拨出库拣货单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"select DISTINCT a.seqno,F_GETDEPTNAME(a.deptout) deptoutname,F_GETDEPTNAME(a.deptid) deptidname,a.xsrq,DHSL,b.gdseq,b.gdname,b.unit,b.gdspec,b.bzhl,b.hsjj,b.pzwh,F_GETPRODUCERNAME(b.producer) producername,b.rowno,to_char(a.shrq,'YYYY-MM-DD') DJRQ,to_char(sysdate,'YYYY-MM-DD') printdate,F_GETUNITNAME(b.unit) unitname,b.XSSL*b.hsjj HSJE,F_GETUSERNAME(a.lry) ZDY,B.PH,B.YXQZ,B.XSSL,B.BZSL,F_GETPARA('SUPPER')||'拣货单' DT,B.HWID,(select sum(hsje)from dat_db_com where seqno=a.seqno)sumhjje
            //    from dat_db_doc a,dat_db_com b ,DAT_GOODSJXC T where a.seqno = b.seqno  AND A.SEQNO=T.BILLNO AND a.SEQNO = '{0}' AND XSSL >0 AND T.PSSID='00001' ORDER BY B.HWID";
            string sql = @"select DISTINCT a.seqno,
                                        F_GETDEPTNAME(a.deptout) deptoutname,
                                        F_GETDEPTNAME(a.deptid) deptidname,
                                        a.xsrq,
                                        DHSL,
                                        b.gdseq,
                                        b.gdname,
                                        b.unit,
                                        b.gdspec,
                                        b.bzhl,
                                        b.hsjj,
                                        b.pzwh,
                                        F_GETPRODUCERNAME(b.producer) producername,
                                        b.rowno,
                                        to_char(a.shrq, 'YYYY-MM-DD') DJRQ,
                                        to_char(sysdate, 'YYYY-MM-DD') printdate,
                                        F_GETUNITNAME(b.unit) unitname,
                                        b.XSSL * b.hsjj HSJE,
                                        F_GETUSERNAME(a.lry) ZDY,
                                        B.PH,
                                        B.YXQZ,
                                        B.XSSL SL,
                                        B.BZSL,
                                        F_GETPARA('SUPPER') || '拣货单'||nvl2(b.str1, '(高值)') DT,
                                        B.HWID,
                                        (select sum(hsje) from dat_db_com where seqno = a.seqno) sumhjje,
                                        d.hiscode,
                                        A.SUBSUM,
                                        F_GETSUPNAME(T.PSSID) PSSNAME,T.PSSID,b.isgz,b.rowno,
                                        B.STR1 BARCODE
                          from dat_db_doc a, dat_db_com b, DAT_GOODSJXC T, doc_goods d
                         where a.seqno = b.seqno and b.gdseq = d.gdseq
                           AND B.SEQNO = T.BILLNO AND B.ROWNO=T.ROWNO
                           AND a.SEQNO = '{0}'
                           AND XSSL > 0
                           AND T.PSSID = '00001'
                         ORDER BY T.PSSID,B.ISGZ,b.rowno";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGoodsDbqh()
        {
            //调拨出库拣货单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select a.seqno,ROWNUM ROWNO,
                               F_GETDEPTNAME(a.deptout) deptoutname,
                               F_GETDEPTNAME(a.deptid) deptidname,
                               a.xsrq,
                               b.gdseq,
                               b.gdname,
                               b.unit,
                               b.gdspec,
                               b.bzhl,
                               b.hsjj,
                               F_GETPRODUCERNAME(b.producer) producername,
                               to_char(a.shrq, 'YYYY-MM-DD') DJRQ,
                               to_char(sysdate, 'YYYY-MM-DD') printdate,
                               F_GETUNITNAME(b.unit) unitname,
                               (K.SUMDH-K.SUMXS) * b.hsjj HSJE,
                               F_GETUSERNAME(a.lry) ZDY,
                               F_GETPARA('SUPPER') || '缺货单' DT,K.SUMDH DHSL,K.SUMDH - K.SUMXS XSSL
                          from dat_db_doc a,
                               DOC_GOODS b,
                               (SELECT SUM(XSSL) SUMXS, SUM(DHSL)/ COUNT(1) SUMDH, GDSEQ, SEQNO
                                  FROM dat_db_com
                                 WHERE SEQNO = '{0}'
                                 GROUP BY GDSEQ, SEQNO) K
                         where a.seqno = K.seqno
                           AND B.GDSEQ = K.GDSEQ
                           and a.SEQNO = '{0}'
                           AND K.SUMXS < K.SUMDH
                         ORDER BY B.GDNAME";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGoodsDbrksj()
        {
            //调拨入库上架单  F_GETPARA('SUPPER')
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select DISTINCT a.seqno,F_GETDEPTNAME(a.deptout) deptoutname,F_GETDEPTNAME(a.deptid) deptidname,a.xsrq,b.gdseq,b.gdname,b.unit,b.gdspec,b.bzhl,b.hsjj,F_GETPRODUCERNAME(b.producer) producername,b.rowno,to_char(a.shrq,'YYYY-MM-DD') DJRQ,to_char(sysdate,'YYYY-MM-DD') printdate,F_GETUNITNAME(b.unit) unitname,b.XSSL*b.hsjj HSJE,F_GETUSERNAME(a.lry) ZDY,
                   B.PH,B.YXQZ,B.XSSL sl,B.BZSL,B.PZWH,F_GETPARA('SUPPER')||'入库上架单'||nvl2(b.str1,'(高值)','(非高值)') DT,F_GETHWID(a.deptid,b.gdseq) HWID,A.SUBSUM,F_GETSUPNAME(T.PSSID)PSSNAME,B.STR1 BARCODE,d.hiscode
                from dat_db_doc a,dat_db_com b,DAT_GOODSJXC T,doc_goods d where a.seqno = b.seqno and b.gdseq=d.gdseq AND A.SEQNO=T.BILLNO and b.xssl > 0 and a.SEQNO = '{0}' AND T.PSSID<>'00001'AND B.ROWNO=T.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGoodsDbrksjDG()
        {
            //调拨入库上架单 F_GETPARA('SUPPER')
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select DISTINCT a.seqno,F_GETDEPTNAME(a.deptout) deptoutname,F_GETDEPTNAME(a.deptid) deptidname,a.xsrq,b.gdseq,b.gdname,b.unit,b.gdspec,b.pzwh,b.bzhl,b.hsjj,A.SUBSUM,F_GETSUPNAME(T.PSSID)PSSNAME,B.STR1 BARCODE,
                          F_GETPRODUCERNAME(b.producer) producername,b.rowno,to_char(a.shrq,'YYYY-MM-DD') DJRQ,to_char(sysdate,'YYYY-MM-DD') printdate,F_GETUNITNAME(b.unit) unitname,b.XSSL*b.hsjj HSJE,F_GETUSERNAME(a.lry) ZDY,B.PH,B.YXQZ,B.XSSL SL,B.BZSL,F_GETPARA('SUPPER')||'入库上架单'||nvl2(b.str1,'(高值)','(非高值)') DT,F_GETHWID(a.deptid,b.gdseq) HWID,d.hiscode
                from dat_db_doc a,dat_db_com b,DAT_GOODSJXC T,doc_goods d where a.seqno = b.seqno and b.gdseq=d.gdseq AND A.SEQNO=T.BILLNO and b.xssl > 0 and a.SEQNO = '{0}' AND T.PSSID='00001'AND B.ROWNO=T.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGoodsJsks()
        {
            //结算报表打印,妇幼专用
            string time1 = Request["time1"] == null ? "" : Request["time1"].ToString();
            string time2 = Request["time2"] == null ? "" : Request["time2"].ToString();
            string sql = @"select a.deptid,f_getdeptname(A.DEPTID) DEPTIDNAME,sum(decode(exp_form,'办公耗材',b.hsje,0)) bghc,sum(decode(exp_form,'办公用品',b.hsje,0)) bgyp,sum(decode(exp_form,'被服材料',b.hsje,0)) bfcl
                        ,sum(decode(exp_form,'低值易耗',b.hsje,0)) dzyh,sum(decode(exp_form,'电工材料',b.hsje,0)) dgcl,sum(decode(exp_form,'非收费其他',b.hsje,0)) fsfqt
                        ,sum(decode(exp_form,'木工材料',b.hsje,0)) mgcl,sum(decode(exp_form,'设备材料',b.hsje,0)) sbcl,sum(decode(exp_form,'试剂',b.hsje,0)) sj
                        ,sum(decode(exp_form,'收费其他',b.hsje,0)) sfqt,sum(decode(exp_form,'维修材料',b.hsje,0)) wxcl,sum(decode(exp_form,'卫生材料',b.hsje,0)) wscl
                        ,sum(decode(exp_form,'卫生用品',b.hsje,0)) wsyp,sum(decode(exp_form,'印刷品',b.hsje,0)) ysp,sum(decode(exp_form,'植入性材料',b.hsje,0)) zrxcl
                        ,sum(decode(nvl(exp_form,'#'),'#',b.hsje,0)) qt,TO_CHAR(SYSDATE,'YYYY')||'年'||TO_CHAR(SYSDATE,'MM')||'月'||'科室领用卫生材料、低值易耗明细' DT,'{0}' time1,'{1}' time2
                        ,sum(decode(nvl(exp_form,'#'),'办公用品',b.hsje,'低值易耗',b.hsje,'非收费其他',b.hsje,'收费其他',b.hsje,'植入性材料',b.hsje,'试剂',b.hsje,0)) hj
                        from dat_xs_doc a,dat_xs_com b,doc_goods c,(select distinct exp_code,exp_form from temp_goods_his) d
                        where a.seqno = b.seqno and a.flag in('Y','G') and a.SHRQ between to_date('{0}','yyyy-mm-dd') and to_date('{1}','yyyy-mm-dd')
                        and b.gdseq = c.gdseq and c.hiscode = d.exp_code(+) group by a.deptid order by deptid";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, time1, time2), false);
        }
        public void GetGoodsJsgys()
        {
            //结算报表打印,妇幼专用
            string time1 = Request["time1"] == null ? "" : Request["time1"].ToString();
            string time2 = Request["time2"] == null ? "" : Request["time2"].ToString();
            string sql = @"select F_GETPARA('SUPPER') SUPNAME,sum(decode(exp_form,'办公耗材',b.hsje,0)) bghc,sum(decode(exp_form,'办公用品',b.hsje,0)) bgyp,sum(decode(exp_form,'被服材料',b.hsje,0)) bfcl
                        ,sum(decode(exp_form,'低值易耗',b.hsje,0)) dzyh,sum(decode(exp_form,'电工材料',b.hsje,0)) dgcl,sum(decode(exp_form,'非收费其他',b.hsje,0)) fsfqt
                        ,sum(decode(exp_form,'木工材料',b.hsje,0)) mgcl,sum(decode(exp_form,'设备材料',b.hsje,0)) sbcl,sum(decode(exp_form,'试剂',b.hsje,0)) sj
                        ,sum(decode(exp_form,'收费其他',b.hsje,0)) sfqt,sum(decode(exp_form,'维修材料',b.hsje,0)) wxcl,sum(decode(exp_form,'卫生材料',b.hsje,0)) wscl
                        ,sum(decode(exp_form,'卫生用品',b.hsje,0)) wsyp,sum(decode(exp_form,'印刷品',b.hsje,0)) ysp,sum(decode(exp_form,'植入性材料',b.hsje,0)) zrxcl
                        ,sum(decode(nvl(exp_form,'#'),'#',b.hsje,0)) qt,'{0}' time1,'{1}' time2
                        ,sum(decode(nvl(exp_form,'#'),'办公用品',b.hsje,'低值易耗',b.hsje,'非收费其他',b.hsje,'收费其他',b.hsje,'植入性材料',b.hsje,'试剂',b.hsje,0)) hj
                        from dat_xs_doc a,dat_xs_com b,doc_goods c,(select distinct exp_code,exp_form from temp_goods_his) d
                        where a.seqno = b.seqno and a.flag in('Y','G') and a.SHRQ between to_date('{0}','yyyy-mm-dd') and to_date('{1}','yyyy-mm-dd')
                        and b.gdseq = c.gdseq and c.hiscode = d.exp_code(+)";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, time1, time2), false);
        }
        public void GetGoodsSuppe()
        {
            //结算报表打印,妇幼专用
            string time1 = Request["time1"] == null ? "" : Request["time1"].ToString();
            string time2 = Request["time2"] == null ? "" : Request["time2"].ToString();
            string sql = @"select sum(hsje) hjje,F_GETPARA('SUPPER') SUPNAME,sum(b.bzhl) hjsl,'耗材来源情况汇总' DT,'{0}' time1,'{1}' time2,sysdate printdate
                from dat_xs_doc a,dat_xs_com b where a.seqno = b.seqno and a.SHRQ between to_date('{0}','yyyy-mm-dd') and to_date('{1}','yyyy-mm-dd')";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, time1, time2), false);
        }
        public void Mod_Ds()
        {
            //定数调整单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.BILLNO,f_getdeptname(A.DEPTID) DEPTIDNAME,A.STR1 REASON,SYSDATE PRINTDATE,A.SHRQ,f_getusername(A.LRY) LRYNAME,B.*,f_getunitname(B.UNIT) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,F_GETPARA('USERNAME')||'定数调整单' DT
                FROM DAT_DT_DOC A,DAT_DT_COM B WHERE A.SEQNO= B.SEQNO AND A.BILLNO = '{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void Mod_Gz()
        {
            //高值调整单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.BILLNO,f_getdeptname(A.DEPTID) DEPTIDNAME,A.STR1 REASON,SYSDATE PRINTDATE,A.SHRQ,f_getusername(A.LRY) LRYNAME,B.*,f_getunitname(B.UNIT) UNITNAME,f_getproducername(B.PRODUCER) PRODUCERNAME,F_GETPARA('USERNAME')||'高值调整单' DT
                FROM DAT_DT_DOC A,DAT_DT_COM B WHERE A.SEQNO= B.SEQNO AND A.BILLNO = '{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void Mod_JSD()
        {
            //结算单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.*,f_getdeptname(A.DEPTID) DEPTIDNAME,F_GETPARA('USERNAME')||'结算单' DT,sysdate printdate,f_getusername(a.lry) lryname,f_getusername(a.spr) sprname
                FROM DAT_JSD_DOC A WHERE A.SEQNO = '{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }

        public void GetData_GZ()
        {
            //高值条码打印
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"SELECT A.BILLNO,A.GDSEQ,NVL(B.HISNAME, B.GDNAME) GDNAME,NVL(B.STR3, B.GDSPEC) GDSPEC,A.BZHL||f_getunitname(B.UNIT) BZALL,f_getdeptname(A.DEPTCUR) DEPTCURNAME,SYSDATE PRINTDATE,
            //    B.HSJJ,f_getproducername(B.PRODUCER) PRODUCERNAME,A.INSTIME,A.BZHL,A.ONECODE,A.FLAG,DECODE(A.FLAG,'T','已退货','Y','已使用','未使用') FLAGNAME
            //     FROM DAT_DD_EXT A,DOC_GOODS B
            //     WHERE A.GDSEQ = B.GDSEQ AND A.ONECODE in ({0})";
            /*string sql = @"SELECT A.ONECODE,
                                              C.GDNAME,
                                              C.GDSPEC,
                                              A.PH,'高值重打' TYPE,TO_CHAR(A.YXQZ, 'yyyy/MM/dd') XQ,
                                              --TO_CHAR(A.RQ_SC, 'yy/MM/dd') || '~' || TO_CHAR(A.YXQZ, 'yy/MM/dd') XQ,
                                              C.GDNAME || ' - ' || C.GDSPEC GD,
                                              F_GETPRODUCERNAME(C.PRODUCER) PRODUCERNAME,
                                              B.SUPNAME,
                                              F_GETPARA('SUPPER')USERNAME
                                         FROM DAT_GZ_EXT A,
                                              (SELECT S.SUPID, S.SUPNAME, G.GDSEQ
                                                 FROM DOC_SUPPLIER S, DOC_GOODSSUP G
                                                WHERE S.SUPID = G.SUPID
                                                  AND S.ISSUPPLIER = 'Y') B,
                                              DOC_GOODS C
                                        WHERE A.GDSEQ = C.GDSEQ
                                          AND B.GDSEQ = C.GDSEQ AND A.ONECODE in ({0})";
                                          这样写的话如果一个商品有多个供应商是不是就都打出来了？ by congwm */
            string sql = @"SELECT A.ONECODE,
                                   C.GDNAME,
                                   C.GDSPEC,
                                   A.PH,
                                   '高值重打' TYPE,
                                   TO_CHAR(A.YXQZ, 'yyyy/MM/dd') XQ,
                                   --TO_CHAR(A.RQ_SC, 'yy/MM/dd') || '~' || TO_CHAR(A.YXQZ, 'yy/MM/dd') XQ,
                                   C.GDNAME || ' - ' || C.GDSPEC GD,
                                   F_GETPRODUCERNAME(C.PRODUCER) PRODUCERNAME,
                                   F_GETSUPNAME(A.SUPID) SUPNAME,
                                   F_GETPARA('SUPPER')USERNAME
                              FROM DAT_GZ_EXT A, DAT_RK_EXT B,
                                   DOC_GOODS C
                             WHERE A.GDSEQ = C.GDSEQ
                               AND B.GDSEQ = C.GDSEQ
                               --AND B.BILLNO = D.SEQNO
                               AND A.ONECODE = B.ONECODE
                               AND A.ONECODE in ({0})";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GoodsReturnNDG()
        {
            //科室退货非代管
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string SHBM = Request["SHBM"] == null ? "" : Request["SHBM"].ToString();
            string sql = @"select F_GETPARA('SUPPER')cusname,a.seqno,F_GETDEPTNAME(a.deptout) deptoutname,F_GETDEPTNAME(a.deptid) deptidname,to_char(a.xsrq, 'YYYY-MM-DD') XSRQNAME,
                b.gdseq,b.gdname,b.unit,b.gdspec,b.bzhl,b.pzwh,
                (case F_GETISDG('{0}', b.gdseq, b.ph, '{1}')when '代管' then 0 else b.hsjj/b.bzhl end) HSJJ,
                F_GETPRODUCERNAME(b.producer) producername,b.rowno,to_char(a.shrq, 'YYYY-MM-DD') DJRQ,to_char(sysdate, 'YYYY-MM-DD') printtime,F_GETUNITNAME(b.unit) unitname,
                (case F_GETISDG('{0}', b.gdseq, b.ph, '{1}') when '代管' then 0 else -b.BZSL * b.hsjj end) HSJE,b.str2 onecode,
                F_GETUSERNAME(a.lry) ZDY,B.PH,f_getisdg('{0}', b.gdseq, b.ph, '{1}') isdg,B.YXQZ,B.XSSL,-(B.BZSL*B.BZHL) BZSL,F_GETPARA('SUPPER')|| '科室退货单' DT,B.HWID
                ,-(select sum(hsje) sumhsje from dat_ck_com where seqno = '{0}' group by seqno) sumhjje
                from dat_ck_doc a, dat_ck_com b
                where a.seqno = b.seqno and a.SEQNO = '{0}' and f_getisdg('{0}', b.gdseq, b.ph, '{1}')  <>'代管'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid, SHBM), false);
        }
        public void GoodsReturnYDG()
        {
            //科室退货代管
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string SHBM = Request["SHBM"] == null ? "" : Request["SHBM"].ToString();
            string sql = @"select F_GETPARA('SUPPER')cusname,a.seqno,F_GETDEPTNAME(a.deptout) deptoutname,F_GETDEPTNAME(a.deptid) deptidname,to_char(a.xsrq, 'YYYY-MM-DD') XSRQNAME,
                b.gdseq,b.gdname,b.unit,b.gdspec,b.bzhl,b.pzwh,
                (case F_GETISDG('{0}', b.gdseq, b.ph, '{1}')when '代管' then 0 else b.hsjj end) HSJJ,
                F_GETPRODUCERNAME(b.producer) producername,b.rowno,to_char(a.shrq, 'YYYY-MM-DD') DJRQ,to_char(sysdate, 'YYYY-MM-DD') printtime,F_GETUNITNAME(b.unit) unitname,
                (case F_GETISDG('{0}', b.gdseq, b.ph, '{1}') when '代管' then 0 else -b.BZSL * b.hsjj end) HSJE,b.str2 onecode,
                F_GETUSERNAME(a.lry) ZDY,B.PH,f_getisdg('{0}', b.gdseq, b.ph, '{1}') isdg,B.YXQZ,B.XSSL,-B.BZSL BZSL,F_GETPARA('SUPPER')|| '科室退货单' DT,B.HWID
                from dat_ck_doc a, dat_ck_com b
                where a.seqno = b.seqno and a.SEQNO = '{0}' and f_getisdg('{0}', b.gdseq, b.ph, '{1}')  ='代管'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid, SHBM), false);
        }
        public void GoodsOrderGz()
        {
            //订货高值条码打印
            string billno = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"SELECT A.BILLNO,A.GDSEQ,B.GDNAME,B.GDSPEC,A.BZHL||f_getunitname(B.UNIT) BZALL,f_getdeptname(A.DEPTCUR) DEPTCURNAME,SYSDATE PRINTDATE,
            //    B.HSJJ,f_getproducername(B.PRODUCER) PRODUCERNAME,A.INSTIME,A.BZHL,A.ONECODE,A.FLAG,DECODE(A.FLAG,'T','已退货','Y','已使用','未使用') FLAGNAME
            //     FROM DAT_DD_EXT A,DOC_GOODS B
            //     WHERE A.GDSEQ = B.GDSEQ AND A.BILLNO = '{0}' AND A.FLAG <>'D' ";
            string sql = @"SELECT A.ONECODE,
                                               NVL(C.HISNAME, C.GDNAME) GDNAME,
                                               NVL(C.STR3, C.GDSPEC) GDSPEC,
                                               A.PH,'高值' TYPE,TO_CHAR(A.YXQZ,'yyyy/MM/dd') XQ,
                                               --TO_CHAR(A.RQ_SC,'yy/MM/dd')||'~'||TO_CHAR(A.YXQZ,'yy/MM/dd') XQ,
                                               NVL(C.HISNAME, C.GDNAME) || ' - ' || NVL(C.STR3, C.GDSPEC) GD,
                                               F_GETPRODUCERNAME(C.PRODUCER) PRODUCERNAME,
                                               B.SUPNAME,
                                               F_GETPARA('SUPPER')USERNAME
                                          FROM DAT_RK_EXT A,
                                               (SELECT S.SUPID, S.SUPNAME, G.GDSEQ
                                                  FROM DOC_SUPPLIER S, DOC_GOODSSUP G
                                                 WHERE S.SUPID = G.SUPID
                                                   AND S.ISSUPPLIER = 'Y') B,
                                               DOC_GOODS C
                                         WHERE A.GDSEQ = C.GDSEQ AND B.GDSEQ = C.GDSEQ
                                             AND A.BILLNO = '{0}' AND A.FLAG <>'D' ";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, billno), false);
        }
        public void GoodsRkGz()
        {
            //入库高值条码打印
            string billno = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"SELECT A.BILLNO,A.GDSEQ,B.GDNAME,B.GDSPEC,A.BZHL||f_getunitname(B.UNIT) BZALL,f_getdeptname(A.DEPTCUR) DEPTCURNAME,SYSDATE PRINTDATE,
            //    B.HSJJ,f_getproducername(B.PRODUCER) PRODUCERNAME,A.INSTIME,A.BZHL,A.ONECODE,A.FLAG,DECODE(A.FLAG,'T','已退货','Y','已使用','未使用') FLAGNAME,PH,YXQZ,
            //    RQ_SC,f_getunitname(B.UNIT) UNITNAME
            //     FROM DAT_RK_EXT A,DOC_GOODS B
            //     WHERE A.GDSEQ = B.GDSEQ AND A.BILLNO = '{0}'";
            string sql = @"SELECT A.ONECODE,
                                        NVL(C.HISNAME, C.GDNAME) GDNAME,
                                        NVL(C.STR3, C.GDSPEC) GDSPEC,
                                        A.PH,
                                        '高值' TYPE,
                                        TO_CHAR(A.YXQZ, 'yyyy/MM/dd') XQ,
                                        NVL(C.HISNAME, C.GDNAME) || ' - ' || NVL(C.STR3, C.GDSPEC) GD,
                                        F_GETPRODUCERNAME(C.PRODUCER) PRODUCERNAME,
                                        F_GETSUPNAME(NVL(B.SUPID,'')) SUPNAME,
                                        F_GETPARA('USERNAME') USERNAME
                                    FROM DAT_RK_EXT A, DAT_GZ_EXT B, DOC_GOODS C
                                    WHERE A.GDSEQ = C.GDSEQ
                                    AND A.ONECODE = B.ONECODE
                                   AND A.BILLNO = '{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, billno), false);
        }


        public void GoodsYRkGz()
        {
            //入库高值条码打印
            string billno = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"SELECT A.BILLNO,A.GDSEQ,B.GDNAME,B.GDSPEC,A.BZHL||f_getunitname(B.UNIT) BZALL,f_getdeptname(A.DEPTCUR) DEPTCURNAME,SYSDATE PRINTDATE,
            //    B.HSJJ,f_getproducername(B.PRODUCER) PRODUCERNAME,A.INSTIME,A.BZHL,A.ONECODE,A.FLAG,DECODE(A.FLAG,'T','已退货','Y','已使用','未使用') FLAGNAME,PH,YXQZ,
            //    RQ_SC,f_getunitname(B.UNIT) UNITNAME
            //     FROM DAT_RK_EXT A,DOC_GOODS B
            //     WHERE A.GDSEQ = B.GDSEQ AND A.BILLNO = '{0}'";
            string sql = @"SELECT A.ONECODE,
                                               NVL(C.HISNAME, C.GDNAME) GDNAME,
                                               NVL(C.STR3, C.GDSPEC) GDSPEC,
                                               A.PH,'跟台' TYPE,TO_CHAR(A.YXQZ,'yyyy/MM/dd') XQ,
                                               --TO_CHAR(A.RQ_SC,'yy/MM/dd')||'~'||TO_CHAR(A.YXQZ,'yy/MM/dd') XQ,
                                               NVL(C.HISNAME, C.GDNAME) || ' - ' || NVL(C.STR3, C.GDSPEC) GD,
                                               F_GETPRODUCERNAME(C.PRODUCER) PRODUCERNAME,
                                               B.SUPNAME,
                                               F_GETPARA('SUPPER')USERNAME
                                          FROM DAT_YRK_EXT A,
                                               (SELECT S.SUPID, S.SUPNAME, G.GDSEQ
                                                  FROM DOC_SUPPLIER S, DOC_GOODSSUP G
                                                 WHERE S.SUPID = G.SUPID
                                                   AND S.ISSUPPLIER = 'Y') B,
                                               DOC_GOODS C
                                         WHERE A.GDSEQ = C.GDSEQ
                                           AND B.GDSEQ = C.GDSEQ AND A.BILLNO = '{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, billno), false);
        }
        public void GoodsCkGz()
        {
            //订货高值条码打印
            string billno = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"SELECT A.BILLNO,A.GDSEQ,B.GDNAME,B.GDSPEC,A.BZHL||f_getunitname(B.UNIT) BZALL,f_getdeptname(A.DEPTCUR) DEPTCURNAME,SYSDATE PRINTDATE,
            //    B.HSJJ,f_getproducername(B.PRODUCER) PRODUCERNAME,A.INSTIME,A.BZHL,A.ONECODE,A.FLAG,DECODE(A.FLAG,'T','已退货','Y','已使用','未使用') FLAGNAME
            //     FROM DAT_CK_EXT A,DOC_GOODS B
            //     WHERE A.GDSEQ = B.GDSEQ AND A.BILLNO = '{0}' AND A.FLAG <>'D' ";
            string sql = @"SELECT A.ONECODE,
                                               NVL(C.HISNAME, C.GDNAME) GDNAME,
                                               NVL(C.STR3, C.GDSPEC) GDSPEC,
                                               A.PH,'高值' TYPE,TO_CHAR(A.YXQZ,'yyyy/MM/dd') XQ,
                                               --TO_CHAR(A.RQ_SC,'yy/MM/dd')||'~'||TO_CHAR(A.YXQZ,'yy/MM/dd') XQ,
                                               NVL(C.HISNAME, C.GDNAME) || ' - ' || NVL(C.STR3, C.GDSPEC) GD,
                                               F_GETPRODUCERNAME(C.PRODUCER) PRODUCERNAME,
                                               B.SUPNAME,
                                               F_GETPARA('SUPPER')USERNAME
                                          FROM DAT_CK_EXT A,
                                               (SELECT S.SUPID, S.SUPNAME, G.GDSEQ
                                                  FROM DOC_SUPPLIER S, DOC_GOODSSUP G
                                                 WHERE S.SUPID = G.SUPID
                                                   AND S.ISSUPPLIER = 'Y') B,
                                               DOC_GOODS C
                                         WHERE A.GDSEQ = C.GDSEQ
                                           AND B.GDSEQ = C.GDSEQ AND A.BILLNO = '{0}' AND A.FLAG <>'D' ";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, billno), false);
        }
        public void GoodsXsGz()
        {
            //订货高值条码打印
            string billno = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"SELECT A.BILLNO,A.GDSEQ,B.GDNAME,B.GDSPEC,A.BZHL||f_getunitname(B.UNIT) BZALL,f_getdeptname(A.DEPTCUR) DEPTCURNAME,SYSDATE PRINTDATE,
            //    B.HSJJ,f_getproducername(B.PRODUCER) PRODUCERNAME,A.INSTIME,A.BZHL,A.ONECODE,A.FLAG,DECODE(A.FLAG,'T','已退货','Y','已使用','未使用') FLAGNAME
            //     FROM DAT_XS_EXT A,DOC_GOODS B
            //     WHERE A.GDSEQ = B.GDSEQ AND A.BILLNO = '{0}' AND A.FLAG <>'D' ";
            string sql = @"SELECT A.ONECODE,
                                               NVL(C.HISNAME, C.GDNAME) GDNAME,
                                               NVL(C.STR3, C.GDSPEC) GDSPEC,
                                               A.PH,'高值' TYPE,TO_CHAR(A.YXQZ,'yyyy/MM/dd') XQ,
                                               --TO_CHAR(A.RQ_SC,'yy/MM/dd')||'~'||TO_CHAR(A.YXQZ,'yy/MM/dd') XQ,
                                               NVL(C.HISNAME, C.GDNAME) || ' - ' || NVL(C.STR3, C.GDSPEC) GD,
                                               F_GETPRODUCERNAME(C.PRODUCER) PRODUCERNAME,
                                               B.SUPNAME,
                                               F_GETPARA('SUPPER')USERNAME
                                          FROM DAT_XS_EXT A,
                                               (SELECT S.SUPID, S.SUPNAME, G.GDSEQ
                                                  FROM DOC_SUPPLIER S, DOC_GOODSSUP G
                                                 WHERE S.SUPID = G.SUPID
                                                   AND S.ISSUPPLIER = 'Y') B,
                                               DOC_GOODS C
                                         WHERE A.GDSEQ = C.GDSEQ
                                           AND B.GDSEQ = C.GDSEQ AND A.BILLNO = '{0}' AND A.FLAG <>'D' ";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, billno), false);
        }
        public void GoodsSY()
        {
            //损益打印
            string billno = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT B.*,a.deptid,a.subsum,f_getusername(A.LRY) LRYNAME,A.SHRQ,f_getusername(A.SHR) SHRNAME,F_GETPARA('SUPPER')|| '损溢单' DT,
                DECODE(A.KCTYPE,'A','溢余','损耗') KCTYPENAME,SYSDATE PRINTDATE,f_getunitname(B.UNIT) UNITNAME,(SELECT SUM(HSJE) FROM DAT_SY_COM WHERE SEQNO = '{0}') SUMHJJE,
                f_getproducername((SELECT C.PRODUCER FROM DOC_GOODS C WHERE C.GDSEQ = B.GDSEQ)) PRODUCERNAME,f_getdeptname(a.deptid) deptidname
                FROM DAT_SY_DOC A,DAT_SY_COM B
                WHERE A.SEQNO = B.SEQNO AND A.SEQNO = '{0}' ";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, billno), false);
        }
        /// <summary>
        /// 获取在库养护单数据
        /// </summary>
        public void GetZkyhData()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.BILLNO,F_GETPARA('SUPPER') || '常规养护检查记录表' TITLE,F_GETDEPTNAME(A.DEPTID) DEPTID,
                                             TO_CHAR(A.YHRQ, 'YYYY-MM-DD') YHRQ,A.SUBNUM,F_GETUSERNAME(A.YHY) YHY,A.MEMO,B.ROWNO,B.GDSEQ,B.GDNAME,B.GDSPEC,B.STR1,
                                             F_GETUNITNAME(B.UNIT) UNIT,DECODE(B.YHTYPE,'0','正常','1','破损','2','失效','发霉') YHTYPE,B.REASON,B.PHID,TO_CHAR(B.YXQZ, 'YYYY-MM-DD') YXQZ,F_GETPRODUCERNAME(B.PRODUCER) PRODUCER
                                   FROM DAT_YH_DOC A, DAT_YH_COM B WHERE A.SEQNO = B.SEQNO AND A.SEQNO = '{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetData_Jhd()
        {
            //科室借货单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,A.XSRQ,B.GDSEQ,B.GDNAME,B.UNIT,B.GDSPEC,B.BZHL,HSJJ,F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME
                    ,B.ROWNO,TO_CHAR(A.XSRQ,'YYYY-MM-DD') DJRQ,TO_CHAR(SYSDATE,'YYYY-MM-DD') PRINTDATE,F_GETUNITNAME(B.UNIT) UNITNAME,HSJE,F_GETUSERNAME(A.LRY) LRYNAME,B.HSJJ||'元/'||F_GETUNITNAME(B.UNIT) V_HSJJ
                    ,B.PH,B.YXQZ,B.XSSL,B.BZSL SL,F_GETPARA('SUPPER')||'医用耗材借货单' DT,B.HWID
                    ,(SELECT SUM(HSJE) FROM DAT_JH_COM WHERE SEQNO = '{0}') SUMHJJE
                    FROM DAT_JH_DOC A,DAT_JH_COM B WHERE A.SEQNO = B.SEQNO AND A.SEQNO = '{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetBarCode()
        {
            string title = "条码回收板", flag = "HP";
            if (Request["isgz"] != null && Request["isgz"] == "Y")
            {
                title = "（高值）条码回收板";
                flag = "JG";
            }

            string sql = @"SELECT NAME DEPT,
                                       F_GETPARA('SUPPER')|| '" + title + @"' TITLE,
                                       '{0}' NO,
                                       1 CEL1,
                                       11 CEL2
                                  FROM SYS_DEPT
                                 WHERE CODE LIKE '{1}'
                                   AND TYPE IN ('3', '4')
                                   AND ISLAST = 'Y'
                                UNION ALL
                                SELECT NAME DEPT,
                                       F_GETPARA('SUPPER')|| '" + title + @"' TITLE,
                                       '{0}' NO,
                                       2 CEL1,
                                       12 CEL2
                                  FROM SYS_DEPT
                                 WHERE CODE LIKE '{1}'
                                   AND TYPE IN ('3', '4')
                                   AND ISLAST = 'Y'
                                UNION ALL
                                SELECT NAME DEPT,
                                       F_GETPARA('SUPPER')|| '" + title + @"' TITLE,
                                       '{0}' NO,
                                       3 CEL1,
                                       13 CEL2
                                  FROM SYS_DEPT
                                 WHERE CODE LIKE '{1}'
                                   AND TYPE IN ('3', '4')
                                   AND ISLAST = 'Y'
                                UNION ALL
                                SELECT NAME DEPT,
                                       F_GETPARA('SUPPER')|| '" + title + @"' TITLE,
                                       '{0}' NO,
                                       4 CEL1,
                                       14 CEL2
                                  FROM SYS_DEPT
                                 WHERE CODE LIKE '{1}'
                                   AND TYPE IN ('3', '4')
                                   AND ISLAST = 'Y'
                                UNION ALL
                                SELECT NAME DEPT,
                                       F_GETPARA('SUPPER')|| '" + title + @"' TITLE,
                                       '{0}' NO,
                                       5 CEL1,
                                       15 CEL2
                                  FROM SYS_DEPT
                                 WHERE CODE LIKE '{1}'
                                   AND TYPE IN ('3', '4')
                                   AND ISLAST = 'Y'
                                UNION ALL
                                SELECT NAME DEPT,
                                       F_GETPARA('SUPPER')|| '" + title + @"' TITLE,
                                       '{0}' NO,
                                       6 CEL1,
                                       16 CEL2
                                  FROM SYS_DEPT
                                 WHERE CODE LIKE '{1}'
                                   AND TYPE IN ('3', '4')
                                   AND ISLAST = 'Y'
                                UNION ALL
                                SELECT NAME DEPT,
                                       F_GETPARA('SUPPER')|| '" + title + @"' TITLE,
                                       '{0}' NO,
                                       7 CEL1,
                                       17 CEL2
                                  FROM SYS_DEPT
                                 WHERE CODE LIKE '{1}'
                                   AND TYPE IN ('3', '4')
                                   AND ISLAST = 'Y'
                                UNION ALL
                                SELECT NAME DEPT,
                                       F_GETPARA('SUPPER')|| '" + title + @"' TITLE,
                                       '{0}' NO,
                                       8 CEL1,
                                       18 CEL2
                                  FROM SYS_DEPT
                                 WHERE CODE LIKE '{1}'
                                   AND TYPE IN ('3', '4')
                                   AND ISLAST = 'Y'
                                UNION ALL
                                SELECT NAME DEPT,
                                       F_GETPARA('SUPPER')|| '" + title + @"' TITLE,
                                       '{0}' NO,
                                       9 CEL1,
                                       19 CEL2
                                  FROM SYS_DEPT
                                 WHERE CODE LIKE '{1}'
                                   AND TYPE IN ('3', '4')
                                   AND ISLAST = 'Y'
                                UNION ALL
                                SELECT NAME DEPT,
                                       F_GETPARA('SUPPER')|| '" + title + @"' TITLE,
                                       '{0}' NO,
                                       10 CEL1,
                                       20 CEL2
                                  FROM SYS_DEPT
                                 WHERE CODE LIKE '{1}'
                                   AND TYPE IN ('3', '4')
                                   AND ISLAST = 'Y'";
            string dept = Request["d"] == null ? "%" : Request["d"].ToString();
            int num = Request["n"] == null ? 10 : int.Parse(Request["n"].ToString());
            string strSql = "";
            for (int i = 0; i < num; i++)
            {
                strSql += string.Format(sql, flag + DateTime.Now.ToString("yyMMddfffff") + (101 + i).ToString().Substring(1), dept == "" ? "%" : dept) + " UNION ALL ";
            }
            sql = string.Format("SELECT * FROM ({0}) ORDER BY DEPT,NO,CEL1", strSql.Substring(0, strSql.Length - 10));
            OracleReportData.GenNodeXmlData(this, sql, false);
        }

        public void GetGZBarCode()
        {
            string title = "条码回收板", flag = "HP";
            if (Request["isgz"] != null && Request["isgz"] == "Y")
            {
                title = "（高值）条码回收板";
                flag = "JG";
            }

            string sql = @"SELECT NAME DEPT, NAME || '{0}' TITLE,
                                              '{1}' || TO_CHAR(SYSDATE, 'YYMM') ||
                                              SUBSTR(TO_CHAR(10000 + {2}), 2) NO
                                          FROM SYS_DEPT
                                         WHERE TYPE IN ('3', '4') AND CODE='{3}' ";
            string dept = Request["d"] == null ? "%" : Request["d"].ToString();
            int num = Request["n"] == null ? 10 : int.Parse(Request["n"].ToString());
            int num1 = Request["m"] == null ? 1 : int.Parse(Request["m"].ToString());
            string strSql = "";
            for (int i = num1; i <= num; i++)
            {
                strSql += string.Format(sql, title, flag, i, dept == "" ? "%" : dept) + " UNION ALL ";
            }
            sql = string.Format("SELECT * FROM ({0}) ORDER BY DEPT,NO", strSql.Substring(0, strSql.Length - 10));
            if (Request["isgz"] != null && Request["isgz"] == "Y")
            {
                DbHelperOra.ExecuteSql("UPDATE SYS_DEPT SET NUM2=" + num + " WHERE CODE='" + dept + "'");
            }
            else
                DbHelperOra.ExecuteSql("UPDATE SYS_DEPT SET NUM3=" + num + " WHERE CODE='" + dept + "'");
            OracleReportData.GenNodeXmlData(this, sql, false);

        }

        public void GetGoodsStock()
        {
            //商品库存信息
            //天津现场提出需求
            //所需字段及顺序：序号、存货地点、商品编码、商品名称、商品规格、单位、含税进价、含税金额、批号、货位、库存数
            string QBKC = Request["QBKC"] == null ? "" : Request["QBKC"].ToString();
            string KSKF = Request["KSKF"] == null ? "" : Request["KSKF"].ToString();
            string GOODS = Request["GOODS"] == null ? "" : Request["GOODS"].ToString();
            string SCCJ = Request["SCCJ"] == null ? "" : Request["SCCJ"].ToString();
            string HWID = Request["HWID"] == null ? "" : Request["HWID"].ToString();
            string PHID = Request["PHID"] == null ? "" : Request["PHID"].ToString();
            string RKDH = Request["RKDH"] == null ? "" : Request["RKDH"].ToString();
            string ISDG = Request["ISDG"] == null ? "" : Request["ISDG"].ToString();
            string USER = Request["USER"] == null ? "" : Request["USER"].ToString();

            //string strSql = @"SELECT F_GETDEPTNAME(A.DEPTID) DEPTID,F_GETSUPNAME(A.SUPID) SUPID,F_GETCATNAME(A.CATID) CATID,
            //                       A.GDSEQ,F_GETHISINFO(A.GDSEQ,'GDNAME') GDNAME,F_GETHISINFO(A.GDSEQ,'GDSPEC') GDSPEC,C.NAME UNIT,B.BZHL,B.BAR3,f_getproducername(b.producer) producername,b.pizno,
            //                       A.HWID,A.KCSL,B.HSJJ,A.KCSL*B.HSJJ HSJE,A.ZPBH,A.BILLNO,A.PHID,A.RQ_SC,A.YXQZ,decode(A.SUPID,'00002','非代管','代管') ISDG
            //                  FROM DAT_GOODSSTOCK A,DOC_GOODS B,DOC_GOODSUNIT C
            //                 WHERE A.GDSEQ=B.GDSEQ AND B.UNIT=C.CODE(+)";
            string strSql = @"SELECT F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,A.DEPTID,F_GETSUPNAME(A.SUPID) SUPID,F_GETCATNAME(A.CATID) CATID,A.LOCKSL,
                                   A.GDSEQ,F_GETHISINFO(A.GDSEQ,'GDNAME') GDNAME,F_GETHISINFO(A.GDSEQ,'GDSPEC') GDSPEC,C.NAME UNIT,B.BZHL,B.BAR3,f_getproducername(b.producer) producername,b.pizno,
                                   A.HWID,A.KCSL,B.HSJJ,A.KCSL*B.HSJJ HSJE,A.ZPBH,A.BILLNO,A.PHID,A.RQ_SC,A.YXQZ,decode(A.SUPID,'00002','非代管','代管') ISDG,decode(B.ISGZ,'Y','是','否') ISGZ,f_getsuppliername(A.PSSID) PSSID,
                                B.ISFLAG7,DECODE(B.ISFLAG7,'Y','本地','下传') ISFLAG7_CN,
                               DECODE(NVL(D.ISCFG, 'Y'), 'Y', '正常', '1', '正常', '停用') ISCFG_CN,
                               DECODE(NVL(D.ISPD, 'Y'), 'Y', '是', '否') ISPD_CN,
                               DECODE(NVL(D.ISJF, 'Y'), 'Y', '是', '否') ISJF_CN
                              FROM DAT_GOODSSTOCK A,DOC_GOODS B,DOC_GOODSUNIT C, DOC_GOODSCFG D
                             WHERE A.GDSEQ=B.GDSEQ AND A.GDSEQ = D.GDSEQ AND A.DEPTID = D.DEPTID AND B.UNIT=C.CODE(+)";
            string strWhere = " ";
            if (QBKC == "true") strWhere += " and A.KCSL > 0";
            if (KSKF != "") strWhere += " and A.DEPTID = '" + KSKF.Trim() + "'";
            if (GOODS != "") strWhere += " and (A.gdseq like '%" + GOODS.ToUpper() + "%' or b.zjm like '%" + GOODS.ToUpper() + "%' or b.gdname like '%" + GOODS.ToUpper() + "%')";
            if (SCCJ != "") strWhere += " and A.SUPID = '" + SCCJ.Trim() + "'";
            if (HWID != "") strWhere += " and A.HWID = '" + HWID.Trim() + "'";
            if (PHID != "") strWhere += " and A.PHID = '" + PHID.Trim() + "'";
            if (RKDH != "") strWhere += " and A.BILLNO like '%" + RKDH.Trim() + "%'";

            if (ISDG != "")
            {
                strWhere += " AND A.SUPID IN (select SUPID from doc_supplier WHERE ISDG = '" + ISDG + "')";
            }
            else
            {
                strWhere += " AND A.SUPID IN (select SUPID from doc_supplier)";
            }
            strWhere += string.Format(" AND a.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' ) AND A.KCSL > 0 ", USER);

            if (strWhere != " ") strSql = strSql + strWhere;
            strSql += "  ORDER BY GDNAME ASC";
            OracleReportData.GenNodeXmlData(this, strSql, false);
        }
        public void GetGoodsStock_NoPh()
        {
            //商品库存信息
            //天津现场提出需求
            //所需字段及顺序：序号、存货地点、商品编码、商品名称、商品规格、单位、含税进价、含税金额、批号、货位、库存数
            string QBKC = Request["QBKC"] == null ? "" : Request["QBKC"].ToString();
            string KSKF = Request["KSKF"] == null ? "" : Request["KSKF"].ToString();
            string GOODS = Request["GOODS"] == null ? "" : Request["GOODS"].ToString();
            string RKDH = Request["RKDH"] == null ? "" : Request["RKDH"].ToString();
            string ISDG = Request["ISDG"] == null ? "" : Request["ISDG"].ToString();
            string USER = Request["USER"] == null ? "" : Request["USER"].ToString();

            //string strSql = @"SELECT F_GETDEPTNAME(A.DEPTID) DEPTID,F_GETSUPNAME(A.SUPID) SUPID,F_GETCATNAME(A.CATID) CATID,
            //                       A.GDSEQ,F_GETHISINFO(A.GDSEQ,'GDNAME') GDNAME,F_GETHISINFO(A.GDSEQ,'GDSPEC') GDSPEC,C.NAME UNIT,B.BZHL,B.BAR3,f_getproducername(b.producer) producername,b.pizno,
            //                       A.HWID,A.KCSL,B.HSJJ,A.KCSL*B.HSJJ HSJE,A.ZPBH,A.BILLNO,A.PHID,A.RQ_SC,A.YXQZ,decode(A.SUPID,'00002','非代管','代管') ISDG
            //                  FROM DAT_GOODSSTOCK A,DOC_GOODS B,DOC_GOODSUNIT C
            //                 WHERE A.GDSEQ=B.GDSEQ AND B.UNIT=C.CODE(+)";
            string strSql = @"SELECT F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                               A.DEPTID,
                               --F_GETSUPNAME(A.SUPID) SUPID,
                              F_GETCATNAME(B.CATID) CATID,
                               A.LOCKSL,
                               A.GDSEQ,
                               F_GETHISINFO(A.GDSEQ, 'GDNAME') GDNAME,
                               F_GETHISINFO(A.GDSEQ, 'GDSPEC') GDSPEC,
                               C.NAME UNIT,
                               B.BZHL,
                               B.BAR3,
                               f_getproducername(b.producer) producername,
                               b.pizno,
                               --A.HWID,
                               A.KCSL,
                               B.HSJJ,
                               A.KCSL * B.HSJJ HSJE,
                               --A.ZPBH,
                               --A.BILLNO,
                               A.PHID,
                               A.RQ_SC,
                               A.YXQZ,
                               --decode(A.SUPID, '00002', '非代管', '代管') ISDG,
                               decode(B.ISGZ, 'Y', '是', '否') ISGZ,
                               --f_getsuppliername(A.PSSID) PSSID,
                               B.ISFLAG7,
                               DECODE(B.ISFLAG7, 'Y', '本地', '下传') ISFLAG7_CN,
                               DECODE(NVL(D.ISCFG, 'Y'), 'Y', '正常', '1', '正常', '停用') ISCFG_CN,
                               DECODE(NVL(D.ISPD, 'Y'), 'Y', '是', '否') ISPD_CN,
                               DECODE(NVL(D.ISJF, 'Y'), 'Y', '是', '否') ISJF_CN
                        FROM (
                        SELECT GDSEQ,DEPTID,SUM(KCSL) KCSL, SUM(LOCKSL) LOCKSL, PHID,RQ_SC,YXQZ FROM DAT_GOODSSTOCK GROUP BY GDSEQ,DEPTID, PHID,RQ_SC,YXQZ) A, DOC_GOODS B, DOC_GOODSUNIT C, DOC_GOODSCFG D
                         WHERE A.GDSEQ = B.GDSEQ AND A.GDSEQ = D.GDSEQ AND A.DEPTID = D.DEPTID
                           AND B.UNIT = C.CODE(+)";
            string strWhere = " ";
            if (QBKC == "true") strWhere += " and A.KCSL > 0";
            if (KSKF != "") strWhere += " and A.DEPTID = '" + KSKF.Trim() + "'";
            if (GOODS != "") strWhere += " and (A.gdseq like '%" + GOODS.ToUpper() + "%' or b.zjm like '%" + GOODS.ToUpper() + "%' or b.gdname like '%" + GOODS.ToUpper() + "%')";

            if (RKDH != "") strWhere += " and A.BILLNO like '%" + RKDH.Trim() + "%'";

            //if (ISDG != "")
            //{
            //    strWhere += " AND A.SUPID IN (select SUPID from doc_supplier WHERE ISDG = '" + ISDG + "')";
            //}
            //else
            //{
            //    strWhere += " AND A.SUPID IN (select SUPID from doc_supplier)";
            //}
            strWhere += string.Format(" AND a.deptid in( select code FROM SYS_DEPT where F_CHK_DATARANGE(CODE, '{0}') = 'Y' ) AND A.KCSL > 0 ", USER);

            if (strWhere != " ") strSql = strSql + strWhere;
            strSql += "  ORDER BY GDNAME ASC";
            OracleReportData.GenNodeXmlData(this, strSql, false);
        }

        public void GoodsYRK()
        {
            //跟台预入库打印 F_GETPARA('SUPPER')
            string billno = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select F_GETPARA('SUPPER') || '医用物资跟台预入库单' DT,
                               a.seqno,
                               --f_getproducername(a.pssid) pss,
                               f_getsupname(a.pssid) pss,
                               a.dhrq,
                               f_getusername(a.shr) ysy,
                               f_getusername(a.lry) dhy,
                               to_char(sysdate, 'YYYY-MM-DD') printtime,
                               b.rowno,
                               F_GETDEPTNAME(a.deptid) deptidname,
                               b.gdseq,
                               b.gdname,       
                               b.gdspec,
                               f_getunitname(b.unit) unitname,
                               b.bzsl,
                               b.ph,
                               b.yxqz,
                               b.rq_sc,       
                               F_GETPRODUCERNAME(c.producer) producername,
                               b.pzwh,
                               a.memo,
                               b.hsjj,
                               b.bzsl * b.bzhl * b.hsjj HSJE,
                               (SELECT TEL FROM DOC_SUPPLIER D WHERE ISSUPPLIER = 'Y' AND B.SUPID = D.SUPID ) TEL,
                               (SELECT SUM(HSJE) FROM dat_yrk_com WHERE SEQNO = '{0}') sumhjje,
                              (SELECT SUM(bzsl) FROM dat_yrk_com WHERE SEQNO = '{0}') SUMSL
                          from dat_yrk_doc a, dat_yrk_com b, doc_goods c
                         where a.seqno = b.seqno and b.gdseq = c.gdseq AND a.seqno = '{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, billno), false);
        }
        public void GoodsSJ()
        {
            //试剂条码打印
            string billno = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.GDBARCODE,B.GDNAME,B.GDSPEC,B.PRODUCER,f_getproducername(B.PRODUCER) PRODUCERNAME
                 FROM dat_barcode_sj A,DOC_GOODS B
                 WHERE A.GDSEQ = B.GDSEQ(+) AND A.RKSEQNO = '{0}'  ";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, billno), false);
        }

        public void GETGoodsSJ()
        {
            //定数条码重打
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.GDBARCODE,B.GDNAME,B.GDSPEC,B.PRODUCER,f_getproducername(B.PRODUCER) PRODUCERNAME
                 FROM dat_barcode_sj A,DOC_GOODS B
                 WHERE A.GDSEQ = B.GDSEQ(+)  AND A.GDBARCODE in ({0})";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }

        public void GetSJCKD()
        {
            //销售单
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select A.SEQNO,
                                   F_GETDEPTNAME(A.DEPTID) DEPTIDOUTNAME,
                                   c.name DEPTIDINNAME,
                                   ROWNO,
                                   GDNAME,
                                   GDSPEC,
                                   F_GETPRINTINF(B.GDSEQ, 'UNITNAME', 0) UNITNAME,
                                   F_GETPRINTINF(B.GDSEQ, 'HSJJ', 0) HSJJ,
                                   F_GETPRINTINF(B.GDSEQ, 'HSJJ', 0) || '元/' ||
                                   F_GETPRINTINF(B.GDSEQ, 'UNITNAME', 0) V_HSJJ,
                                   HSJE,
                                   B.PH PHCK,
                                   yxqz,
                                   B.PZWH PZWHCK,
                                   F_GETPRODUCERNAME(PRODUCER) PRODUCERNAME,
                                   BZSL,
                                   F_GETPRINTINF(B.GDSEQ, 'SL', B.BZHL) BZHL,
                                   (SELECT SUM(HSJE) FROM DAT_XS_COM WHERE SEQNO = '{0}') sumhjje,
                                   f_getusername(A.SHR) SHY
                              from DAT_XS_DOC A, DAT_XS_COM B, SYS_DEPTGROUP C
                             WHERE A.SEQNO = B.SEQNO
                               AND A.STR5 = C.CODE(+)
                               and a.SEQNO = '{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void StockOutAll()
        {
            //汇总拣货
            string DEPTOUT = Request["DEPTOUT"] == null ? "" : Request["DEPTOUT"].ToString();
            string DEPTID = Request["DEPTID"] == null ? "" : Request["DEPTID"].ToString();
            string XL = Request["XL"] == null ? "" : Request["XL"].ToString();
            string GOODS = Request["GOODS"] == null ? "" : Request["GOODS"].ToString();
            string BC = Request["BC"] == null ? "" : Request["BC"].ToString();
            string USERID = Request["USERID"] == null ? "" : Request["USERID"].ToString();
            string LRRQ1 = Request["LRRQ1"] == null ? DateTime.Now.ToString("yyyy-MM-dd") : Request["LRRQ1"].ToString();
            string LRRQ2 = Request["LRRQ2"] == null ? DateTime.Now.ToString("yyyy-MM-dd") : Request["LRRQ2"].ToString();

            //string strSql = string.Format(@"SELECT F_GETPARA('SUPPER') || ' 拣货单' DT,
            //f_getdeptname( A.DEPTOUT) DEPTOUT,B.GDSEQ,B.GDNAME,B.GDSPEC,A.SL,F_GETBZHL(B.GDSEQ) BZHL, FLOOR(A.SL/F_GETBZHL(B.GDSEQ)) BZSL,
            //                B.HSJJ*F_GETBZHL(B.GDSEQ) HSJJ,A.SL*B.HSJJ JE,B.PRODUCER,B.PIZNO
            //                ,F_GETSELLUNITNAME(B.GDSEQ) UNITNAME,F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME，LRYNAME
            //                ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,F_GETHWID(A.DEPTOUT,B.GDSEQ) HWID
            //                FROM (SELECT A.DEPTOUT,F_GETUSERNAME(A.LRY) LRYNAME,B.GDSEQ,SUM(DECODE(A.BILLTYPE,'DSC',FPSL,'LCD',XSSL)) SL
            //                FROM DAT_CK_DOC A,DAT_CK_COM B,SYS_DEPT C
            //                WHERE A.SEQNO = B.SEQNO AND A.FLAG = 'S' AND B.XSSL > 0 AND A.DEPTID = C.CODE AND A.STR4>=TO_DATE('{0}','YYYY-MM-DD') AND A.STR4 <TO_DATE('{1}','YYYY-MM-DD') + 1", LRRQ1, LRRQ2);
            string strSql = string.Format(@"SELECT F_GETPARA('SUPPER') || ' 拣货单' DT,
            f_getdeptname( A.DEPTOUT) DEPTOUT,B.GDSEQ,B.GDNAME,B.GDSPEC,A.SL,F_GETBZHL(B.GDSEQ) BZHL, FLOOR(A.SL/F_GETBZHL(B.GDSEQ)) BZSL,
                            B.HSJJ*F_GETBZHL(B.GDSEQ) HSJJ,A.SL*B.HSJJ JE,B.PRODUCER,B.PIZNO
                            ,F_GETSELLUNITNAME(B.GDSEQ) UNITNAME,F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME，LRYNAME
                            ,F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,F_GETHWID(A.DEPTOUT,B.GDSEQ) HWID,A.ONECODE
                            FROM (SELECT A.DEPTOUT,F_GETUSERNAME(A.LRY) LRYNAME,B.GDSEQ,(case B.ISGZ
                                      when 'N' then
                                        SUM(DECODE(A.BILLTYPE, 'DSC', FPSL, 'LCD', XSSL))
                                         else
                                          1
                                       end) SL,D.ONECODE
                            FROM DAT_CK_DOC A,DAT_CK_COM B,SYS_DEPT C,DAT_CK_EXT D
                            WHERE A.SEQNO = B.SEQNO AND B.SEQNO=D.BILLNO(+) AND B.ROWNO=D.ROWNO(+)
                        AND A.FLAG = 'S' AND B.XSSL > 0 AND A.DEPTID = C.CODE 
                        AND A.STR4>=TO_DATE('{0}','YYYY-MM-DD') AND A.STR4 <TO_DATE('{1}','YYYY-MM-DD') + 1", LRRQ1, LRRQ2);
            //string strSql = string.Format(@"SELECT F_GETPARA('SUPPER') || '拣货单' DT,
            //                                                           B.GDSEQ,
            //                                                           B.GDNAME,
            //                                                           B.GDSPEC,
            //                                                           A.SL,
            //                                                           A.DQKCSL,
            //                                                           A.ZKCSL,
            //                                                           B.HSJJ * F_GETBZHL(B.GDSEQ) HSJJ,
            //                                                           F_GETSELLUNITNAME(B.GDSEQ) UNITNAME,
            //                                                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME，LRYNAME,
            //                                                           F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
            //                                                           F_GETHWID(A.DEPTOUT, B.GDSEQ) HWID
            //                                                      FROM (SELECT A.DEPTOUT,
            //                                                                   F_GETUSERNAME(A.LRY) LRYNAME,
            //                                                                   B.GDSEQ,
            //                                                                   F_GETSTOCK(A.DEPTOUT, B.GDSEQ) DQKCSL,
            //                                                                   F_GETSTOCK(A.DEPTOUT, B.GDSEQ, 'Y') ZKCSL,
            //                                                                   SUM(B.XSSL) SL
            //                                                              FROM DAT_CK_DOC A, DAT_CK_COM B, SYS_DEPT C
            //                                                             WHERE A.SEQNO = B.SEQNO
            //                                                               AND A.FLAG = 'S'
            //                                                               AND B.XSSL > 0
            //                                                               AND A.DEPTID = C.CODE
            //                                                               AND A.STR4 >= TO_DATE('{0}', 'YYYY-MM-DD')
            //                                                               AND A.STR4 < TO_DATE('{1}', 'YYYY-MM-DD') + 1 ", LRRQ1, LRRQ2);
            if (!PubFunc.StrIsEmpty(DEPTOUT))
            {
                strSql += " AND A.DEPTOUT = '" + DEPTOUT + "'";
            }
            if (!PubFunc.StrIsEmpty(DEPTID))
            {
                strSql += " AND A.DEPTID = '" + DEPTID + "'";
            }
            if (!PubFunc.StrIsEmpty(XL))
            {
                strSql += " AND C.STR3 = '" + XL + "'";
            }
            if (BC.Trim().Length > 0)
            {
                strSql += " AND A.NUM1 = " + BC.Trim() + "";
            }
            strSql += " GROUP BY A.DEPTOUT,A.LRY，B.GDSEQ,D.ONECODE,B.ISGZ) A,DOC_GOODS B WHERE A.GDSEQ = B.GDSEQ and (A.SL / F_GETBZHL(B.GDSEQ))>0";
            if (GOODS.Trim().Length > 0)
            {
                strSql += string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME LIKE '%{0}%' OR B.ZJM LIKE '%{0}%' OR B.BARCODE LIKE '%{0}%' OR B.HISCODE LIKE '%{0}%' OR B.HISNAME LIKE '%{0}%' OR B.STR4 LIKE '%{0}%')", GOODS.Trim().ToUpper());
            }
            strSql += " ORDER BY DEPTOUT,GDSEQ";


            OracleReportData.GenNodeXmlData(this, strSql, false);
        }
        public void StockOutDateAll()
        {
            //汇总拣货
            string DEPTOUT = Request["DEPTOUT"] == null ? "" : Request["DEPTOUT"].ToString();
            string DEPTID = Request["DEPTID"] == null ? "" : Request["DEPTID"].ToString();
            string GOODS = Request["GOODS"] == null ? "" : Request["GOODS"].ToString();
            string FLAG = Request["FLAG"] == null ? "%" : Request["FLAG"].ToString();
            string BILLNO = Request["BN"] == null ? "" : Request["BN"].ToString();
            string date1 = Request["D1"] == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : Request["D1"].ToString();
            string date2 = Request["D2"] == null ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : Request["D2"].ToString();

            string strsql = @"SELECT A.*,SYSDATE DYRQ,ROWNUM FROM (SELECT A.DEPTOUT,B.GDSEQ,B.GDNAME,B.GDSPEC,B.HSJJ,SUM(B.HSJE) JE,B.PRODUCER,B.UNIT,B.BZHL,SUM(B.BZSL) SL,
                                                 F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                                 F_GETUNITNAME(B.UNIT) UNITNAME,F_GETHWID(A.DEPTOUT,B.GDSEQ) HWID
                                       FROM DAT_CK_DOC A,DAT_CK_COM B,SYS_DEPT C,DOC_GOODS D
                                     WHERE A.SEQNO = B.SEQNO AND A.DEPTID = C.CODE AND B.GDSEQ = D.GDSEQ
                                         AND A.BILLTYPE = 'LCD' ";
            string strWHERE = "";
            if (!string.IsNullOrWhiteSpace(BILLNO))
            {
                strWHERE += " AND A.SEQNO IN ('" + BILLNO.Replace(",", "','") + "') ";
            }
            else
            {
                if (!PubFunc.StrIsEmpty(FLAG))
                {
                    strWHERE += "AND A.FLAG = '" + FLAG + "'";
                }
                else
                {
                    strWHERE += "AND A.FLAG IN ('S','Y')";
                }
                if (DEPTOUT.Length > 0)
                {
                    strWHERE += "AND A.DEPTOUT = '" + DEPTOUT + "'";
                }
                if (DEPTID.Length > 0)
                {
                    strWHERE += " AND A.DEPTID = '" + DEPTID + "'";
                }
                if (GOODS.Length > 0)
                    strWHERE += " AND ( D.GDNAME like '%" + GOODS + "%' or D.GDSEQ like '%" + GOODS + "%'or D.ZJM like '%" + GOODS + "%' or D.ZJM like '%" + GOODS + "%' or D.ZJM like '%" + GOODS + "%')";

                strWHERE += string.Format(" AND A.LRRQ BETWEEN TO_DATE('{0}', 'yyyy-mm-dd hh24:mi:ss') AND TO_DATE('{1}', 'yyyy-mm-dd hh24:mi:ss') + 1", date1, date2);
            }
            strWHERE += " GROUP BY A.DEPTOUT,B.GDSEQ,B.GDNAME,B.GDSPEC,B.HSJJ,B.PRODUCER,B.UNIT,B.BZHL ORDER BY PRODUCERNAME, A.DEPTOUT, B.GDNAME) A";

            OracleReportData.GenNodeXmlData(this, strsql + strWHERE, false);
        }
        public void KFBCJHPrinting()
        {
            //销售单非代管
            string osid = Request["djbh"] == null ? "" : Request["djbh"].ToString();

            string sql = @"select F_GETPARA('SUPPER') || '出库单' DT,TA.*
  from (SELECT
         f_getdeptname(B.DEPTOUT) DEPTOUT,
         f_getdeptname(B.DEPTID) DEPTID,
         B.SHRQ,
         A.SEQNO,
         A.GDSEQ,
         A.GDNAME,
         A.BARCODE,
         decode(SUBSTR(A.SEQNO, 0, 2), 'DS', A.XSSL, A.BZSL) BZSL,
         A.JXTAX,
         A.HSJJ,
         A.HSJE,
         A.ZPBH,
         A.HWID,
         A.PZWH,
         A.RQ_SC,
         A.MEMO,
         F_GETUNITNAME(A.UNIT) UNITNAME,
         F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
         A.GDSPEC
          FROM DAT_CK_COM A, DAT_CK_DOC B
         WHERE A.SEQNO = B.SEQNO
           AND A.SEQNO in ({0})
        UNION ALL
        SELECT f_getdeptname(B.DEPTOUT) DEPTOUT,
               f_getdeptname(B.DEPTID) DEPTID,
               B.SHRQ,
               A.SEQNO,
               A.GDSEQ,
               A.GDNAME,
               A.BARCODE,
               decode(SUBSTR(A.SEQNO, 0, 3),
                      'LYD',
                      A.BZSL,
                      'DSC',
                      (A.BZSL * A.BZHL),
                      A.BZSL) BZSL,
               A.JXTAX,
               A.HSJJ,
               A.HSJE,
               A.ZPBH,
               A.HWID,
               A.PZWH,
               A.RQ_SC,
               A.MEMO,
               F_GETUNITNAME(A.UNIT) UNITNAME,
               F_GETPRODUCERNAME(A.PRODUCER) PRODUCERNAME,
               A.GDSPEC
          FROM DAT_SL_COM A, DAT_SL_DOC B
         WHERE A.SEQNO = B.SEQNO
            and decode(SUBSTR(A.SEQNO, 0, 3),
                      'LYD',
                      A.BZSL,
                      'DSC',
                      (A.BZSL * A.BZHL),
                      A.BZSL) >0
           AND A.SEQNO in ({0})) TA
 order by seqno asc";

            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetValuable()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select a.seqno,
                               F_GETDEPTNAME(a.deptout) deptoutname,
                               F_GETDEPTNAME(a.deptid) deptidname,
                               a.xsrq,
                               b.gdseq,
                               b.gdname,
                               b.unit,
                               b.gdspec,
                               b.bzhl,
                               b.hsjj,
                               F_GETPRODUCERNAME(b.producer) producername,
                               b.rowno,
                               b.str2,
                               to_char(a.shrq, 'YYYY-MM-DD') DJRQ,
                               to_char(sysdate, 'YYYY-MM-DD') printdate,
                               F_GETUNITNAME(b.unit) unitname,
                               b.bzhl * b.hsjj HSJE,
                               F_GETUSERNAME(a.lry) ZDY,
                               F_GETPARA('SUPPER') || '植入材料使用登记表' DT,
                               b.ph,
                               b.yxqz,
                               '12.22' sumhjje,
                               '' V_HSJJ
                          from dat_sl_doc a, dat_sl_com b
                         where a.seqno = b.seqno  AND A.SEQNO='{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }

        public void GetKSSLData()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,f_getdeptname(A.DEPTOUT) DEPTOUT,f_getdeptname(A.DEPTID) DEPTID,DECODE(A.BILLTYPE,'LYD','物资请领','GBD','高值备货') OPER,B.PH,B.PZWH,
                           B.GDSEQ,
                           B.GDNAME,
                           B.GDSPEC,
                           F_GETPRINTINF(B.GDSEQ,'SL',B.BZHL) BZHL,--B.BZHL,
                           B.HSJJ,
                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                           B.ROWNO,
                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTTIME,
                           F_GETPRINTINF(B.GDSEQ,'UNITNAME',0) UNITNAME,
                           HSJE,
                           F_GETUSERNAME(A.LRY) ZDY,
                           B.BZSL,
                           F_GETPRINTINF(B.GDSEQ,'SL',B.BZHL) BZHL,--B.BZHL,
                           F_GETPARA('SUPPER')|| '科室申领单' DT,
                           F_GETPARA('SUPPER')KH,
                           TO_CHAR(A.LRRQ, 'YYYY-MM-DD') LRRQ,
                           --F_GETDEPTNAME(F_GETPARA('DEFDEPT')) DEPTINNAME,
                           (SELECT SUM(HSJE) FROM DAT_SL_COM WHERE SEQNO = '{0}') SUMHJJE,
                           F_GETPRINTINF(B.GDSEQ,'HSJJ',0) || '元/' || F_GETPRINTINF(B.GDSEQ,'UNITNAME',0) V_HSJJ
                      FROM DAT_SL_DOC A, DAT_SL_COM B
                     WHERE A.SEQNO = B.SEQNO AND A.SEQNO = '{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetDSDataBillAll()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            if (osid.IndexOf('\'') < 0)
            {
                osid = osid.Replace(",", "','");
            }
            if (string.IsNullOrWhiteSpace(osid))
            {
                osid = "''";
            }
            //string sql = @"SELECT A.SEQNO,
            //                                  F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
            //                                  F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
            //                                  to_char(A.XSRQ, 'yyyy-MM-dd') XSRQ,
            //                                  B.GDSEQ,
            //                                  B.GDNAME,
            //                                  B.GDSPEC,
            //                                  C.SL BZHL,
            //                                  B.HSJJ,
            //                                  F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
            //                                  B.ROWNO,
            //                                  B.STR2,
            //                                  TO_CHAR(A.SHRQ, 'YYYY-MM-DD') DJRQ,
            //                                  TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
            //                                  f_getunitname(B.UNIT) UNITNAME,
            //                                  C.HSJE HSJE,
            //                                  F_GETUSERNAME(A.LRY) ZDY,
            //                                  F_GETPARA('SUPPER') || '随货同行单(定数)' DT,
            //                                  C.PH,
            //                                  C.YXQZ,
            //                                  A.subsum SUMHJJE,
            //                                  B.HSJJ V_HSJJ,
            //                                  F_GETHWID(A.DEPTOUT, B.GDSEQ) HWID,
            //                                  b.FPSL / b.BZHL CH
            //                             FROM DAT_CK_DOC A, DAT_CK_COM B, DAT_GOODSJXC C
            //                            WHERE A.SEQNO = B.SEQNO
            //                              AND B.SEQNO = C.BILLNO AND C.KCADD = 1
            //                              AND B.ROWNO = C.ROWNO
            //                              and (b.FPSL / b.BZHL) > 0
            //                              AND A.SEQNO in ({0})
            //                            ORDER BY B.SEQNO,B.ROWNO";
            string sql = @"SELECT A.SEQNO,
                                              F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                              F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                              to_char(A.XSRQ, 'yyyy-MM-dd') XSRQ,
                                              B.GDSEQ,
                                              B.GDNAME,
                                              B.GDSPEC,
                                              C.SL BZHL,
                                              B.HSJJ,
                                              F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                              B.ROWNO,
                                              B.STR2,
                                              TO_CHAR(A.SHRQ, 'YYYY-MM-DD') DJRQ,
                                              TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                                              f_getunitname(B.UNIT) UNITNAME,
                                              C.HSJE HSJE,
                                              F_GETUSERNAME(A.LRY) ZDY,
                                              F_GETPARA('SUPPER') || '随货同行单(定数)' DT,
                                              C.PH,
                                              C.YXQZ,
                                              (SELECT SUM(HSJE) FROM DAT_CK_COM WHERE SEQNO={0} AND STR2 IS NOT NULL) SUMHJJE,
                                              B.HSJJ V_HSJJ,
                                              F_GETHWID(A.DEPTOUT, B.GDSEQ) HWID,
                                              b.FPSL / b.BZHL CH,
                                              C.PSSID,
                                              f_getsupname(C.PSSID) PSSNAME,
                                              B.PZWH,F_GETBILLCOUNT(A.SEQNO,'CKD',5) SUBNUM,f_getpssbillcount(A.SEQNO,C.PSSID,'',5) PSSNUM
                                         FROM DAT_CK_DOC A, DAT_CK_COM B, (SELECT BILLNO, ROWNO, PH, YXQZ, SUM(SL) SL, SUM(HSJE) HSJE, PSSID
                                                               FROM DAT_GOODSJXC WHERE KCADD = 1 AND BILLNO in ({0})
                                                               GROUP BY BILLNO, ROWNO, PH, YXQZ, PSSID) C
                                        WHERE A.SEQNO = B.SEQNO
                                          AND B.SEQNO = C.BILLNO
                                          AND B.ROWNO = C.ROWNO
                                          and (b.FPSL / b.BZHL) > 0
                                          AND A.SEQNO in ({0})
                                        ORDER BY B.SEQNO,C.PSSID";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetData_FcksldAll()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"SELECT A.SEQNO,F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,A.XSRQ,B.GDSEQ,B.GDNAME,B.UNIT,B.GDSPEC,B.BZHL,B.HSJJ,F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME
            //        ,B.ROWNO,TO_CHAR(A.XSRQ,'YYYY-MM-DD') DJRQ,TO_CHAR(SYSDATE,'YYYY-MM-DD') PRINTDATE,F_GETUNITNAME(B.UNIT) UNITNAME,B.HSJE,F_GETUSERNAME(A.LRY) ZDY,B.HSJJ||'元/'||F_GETUNITNAME(B.UNIT) V_HSJJ
            //        ,B.PH,B.YXQZ,B.XSSL,B.BZSL,F_GETPARA('SUPPER')||'随货同行单(非定数)' DT,B.HWID,B.STR2,A.subsum SUMHJJE
            //        FROM DAT_CK_DOC A,DAT_CK_COM B 
            //      WHERE A.SEQNO = B.SEQNO AND A.SEQNO in ({0}) 
            //        order by seqno";
            //string title = "医疗物资销售单(非定数)";
            //F_GETPARA('SUPPER')
            if (osid.IndexOf('\'') < 0)
            {
                osid = "'" + osid.Replace(",", "','") + "'";
            }
            string sql = @"SELECT  SEQNO,CKSEQNO,DEPTOUTNAME,DEPTIDNAME,XSRQ,GDSEQ,GDNAME,UNIT,GDSPEC,BZHL,HSJJ,PRODUCERNAME,ROWNO,DJRQ,PRINTDATE,UNITNAME,sum(HSJE)HSJE,ZDY,V_HSJJ,PH,YXQZ,PZWH,PSSNAME,SUM(XSSL)XSSL,SUM(BZSL)BZSL,PSSID,DT,HWID,STR2,SUMHJJE
                                ,F_GETBILLCOUNT(CKSEQNO,'CKD',5) SUBNUM                                  
                                FROM (
                                        SELECT '' SEQNO,
                                               A.SEQNO CKSEQNO,
                                               F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                               F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                               A.XSRQ,
                                               B.GDSEQ,
                                               B.GDNAME,
                                               B.UNIT,
                                               B.GDSPEC,
                                               B.BZHL,
                                               B.HSJJ,
                                               F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                               B.ROWNO,
                                               TO_CHAR(A.XSRQ, 'YYYY-MM-DD') DJRQ,
                                               TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                                               F_GETUNITNAME(B.UNIT) UNITNAME,
                                               B.HSJJ*ABS(C.SL) HSJE,
                                               F_GETUSERNAME(A.LRY) ZDY,
                                               B.HSJJ || '元/' || F_GETUNITNAME(B.UNIT) V_HSJJ,
                                               C.PH,C.STR2 CSTR,
                                               C.YXQZ,B.PZWH,F_GETSUPNAME(C.PSSID) PSSNAME,
                                               ABS(C.SL) XSSL,
                                               ABS(C.SL) / B.BZHL BZSL,
                                               DECODE(C.PSSID, '00001', '代管','非代管') PSSID,
                                               F_GETPARA('SUPPER') || '随货同行单(非定数)' ||
                                               DECODE(C.PSSID, '00001', '代管') DT,
                                               --F_GETPARA('SUPPER') || '随货同行单(非定数)' DT,
                                               B.HWID,
                                               B.STR2,
                                               A.SUBSUM SUMHJJE
                                          FROM DAT_CK_DOC A, DAT_CK_COM B, DAT_GOODSJXC C
                                         WHERE A.SEQNO = B.SEQNO
                                           AND B.SEQNO = C.BILLNO
                                           AND B.ROWNO = C.ROWNO
                                           AND C.KCADD = 1
                                           AND B.ISGZ = 'N'
                                          -- AND F_GETJSMODE(A.DEPTID) = 'N'
                                           AND A.SEQNO IN ({0}))
      GROUP BY SEQNO,CKSEQNO,DEPTOUTNAME,DEPTIDNAME,XSRQ,GDSEQ,GDNAME,UNIT,GDSPEC,BZHL,HSJJ,PRODUCERNAME,ROWNO,DJRQ,PRINTDATE,UNITNAME,ZDY,V_HSJJ,PH,YXQZ,PZWH,PSSNAME,PSSID,DT,HWID,STR2,SUMHJJE
                                 ORDER BY CKSEQNO,PSSNAME,ROWNO";

            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetData_GZcksldAll()
        {

            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            if (osid.IndexOf('\'') < 0)
            {
                osid = "'" + osid.Replace(",", "','") + "'";
            }
            string title = "随货同行单（高值）";
            string sql = "SELECT * FROM DUAL";


            DataTable dtBill = DbHelperOra.Query(@"SELECT C.ISGZ,F_GETJSMODE(A.DEPTID) JSMODE  
                                            FROM DAT_CK_DOC A, DAT_CK_COM B, DOC_GOODS C 
                                            WHERE A.SEQNO = B.SEQNO AND B.GDSEQ = C.GDSEQ AND A.SEQNO  IN (" + osid + ") ").Tables[0];
            object objMode = DbHelperOra.GetSingle("SELECT F_GETJSMODE(DEPTID) FROM DAT_CK_DOC WHERE SEQNO IN (" + osid + ")");
            foreach (DataRow row in dtBill.Rows)
            {
                if (!string.IsNullOrWhiteSpace(row["ISGZ"].ToString()) && row["ISGZ"].ToString() == "Y")
                {
                    //sql = @"SELECT A.SEQNO,
                    //                           F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                    //                           F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                    //                           A.XSRQ,
                    //                           B.GDSEQ,
                    //                           B.GDNAME,
                    //                           B.UNIT,
                    //                           B.GDSPEC,
                    //                           B.BZHL,
                    //                           B.HSJJ,
                    //                           F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                    //                           B.ROWNO,
                    //                           TO_CHAR(A.XSRQ, 'YYYY-MM-DD') DJRQ,
                    //                           TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                    //                           F_GETUNITNAME(B.UNIT) UNITNAME,
                    //                           1 * B.HSJJ V_HSJE,
                    //                           F_GETUSERNAME(A.LRY) ZDY,
                    //                           B.HSJJ || '元/' || F_GETUNITNAME(B.UNIT) V_HSJJ,
                    //                           C.PH,
                    //                           C.YXQZ,
                    //                           1 XSSL,
                    //                           1 / B.BZHL BZSL,
                    //                           DECODE(C.PSSID,'00001','代管','非代管') PSSID,
                    //                           F_GETPARA('SUPPER') || '{1}' || DECODE(C.PSSID,'00001','代管') DT,
                    //                           --F_GETPARA('SUPPER') || '随货同行单(非定数)' DT,
                    //                           B.HWID,
                    //                           B.STR2,C.ONECODE,
                    //                           (select CODE_HIS from sys_dept where code = A.DEPTID) KJCODE,--天津医院，会计编码
                    //                           SUM(B.HSJJ * B.XSSL) OVER(PARTITION BY A.SEQNO) SUMHJJE
                    //                      FROM DAT_CK_DOC A, DAT_CK_COM B,
                    //                      (
                    //                      SELECT A.BILLNO CKSEQNO,B.*
                    //                           FROM DAT_CK_EXT A, DAT_GZ_EXT B
                    //                           WHERE A.ONECODE = B.ONECODE AND A.GDSEQ = B.GDSEQ AND A.PH = B.PH
                    //                      ) C
                    //                     WHERE A.SEQNO = B.SEQNO
                    //                       AND A.SEQNO = C.CKSEQNO
                    //                       AND B.GDSEQ = C.GDSEQ
                    //                       AND A.SEQNO IN ({0})
                    //                     ORDER BY B.PSSID,A.SEQNO,B.ROWNO";
                    sql = @"SELECT A.SEQNO,
                                               F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                               F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                               A.XSRQ,
                                               B.GDSEQ,
                                               B.GDNAME,
                                               B.UNIT,
                                               B.GDSPEC,
                                               B.BZHL,
                                               B.HSJJ,
                                               F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                               B.ROWNO,
                                               TO_CHAR(A.XSRQ, 'YYYY-MM-DD') DJRQ,
                                               TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                                               F_GETUNITNAME(B.UNIT) UNITNAME,
                                               1 * B.HSJJ V_HSJE,
                                               F_GETUSERNAME(A.LRY) ZDY,
                                               B.HSJJ || '元/' || F_GETUNITNAME(B.UNIT) V_HSJJ,
                                               C.PH,
                                               C.YXQZ,
                                               B.PZWH,
                                               C.PSSID,
                                               F_GETSUPNAME(C.PSSID) PSSNAME,
                                               1 XSSL,
                                               1 / B.BZHL BZSL,
                                               DECODE(C.PSSID,'00001','代管','非代管') PSSID,
                                               F_GETPARA('SUPPER') || '{1}' || DECODE(C.PSSID,'00001','代管') DT,
                                               --F_GETPARA('SUPPER') || '随货同行单(非定数)' DT,
                                               B.HWID,
                                               B.STR2,C.ONECODE,
                                               (select CODE_HIS from sys_dept where code = A.DEPTID) KJCODE,--天津医院，会计编码
                                               --SUM(B.HSJJ * 1) OVER(PARTITION BY A.SEQNO) SUMHJJE
                                               A.SUBSUM SUMHJJE,F_GETBILLCOUNT(A.SEQNO,'CKD',10) SUBNUM
                                          FROM DAT_CK_DOC A, DAT_CK_COM B,
                                          (
                                          SELECT A.BILLNO CKSEQNO,B.*
                                               FROM DAT_CK_EXT A, DAT_GZ_EXT B
                                               WHERE A.ONECODE = B.ONECODE AND A.GDSEQ = B.GDSEQ AND A.PH = B.PH
                                          ) C
                                         WHERE A.SEQNO = B.SEQNO
                                           AND A.SEQNO = C.CKSEQNO
                                           AND B.GDSEQ = C.GDSEQ
                                           AND A.SEQNO IN ({0})
                                         ORDER BY A.SEQNO,C.PSSID,B.ROWNO";
                }
            }
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid, title), false);
        }

        public void EidtPrintNum()
        {
            string osid = Request["seqno"] == null ? "" : Request["seqno"].ToString();
            string user = Request["user"] == null ? "" : Request["user"].ToString();
            string oper = Request["oper"] == null ? "" : Request["oper"].ToString();

            if (!string.IsNullOrWhiteSpace(osid) && !string.IsNullOrWhiteSpace(osid) && !string.IsNullOrWhiteSpace(osid))
            {
                List<CommandInfo> cmdList = new List<CommandInfo>();
                string[] osids = osid.Replace("'", "").Replace(',', '_').Split('_');
                foreach (string bill in osids)
                {
                    string type = bill.Substring(0, 3);

                    StringBuilder builder = new StringBuilder();
                    builder.Append(" DECLARE ");
                    builder.Append(" LN_NUM NUMBER;");
                    builder.Append(" BEGIN ");
                    builder.Append(" SELECT COUNT(*) INTO LN_NUM");
                    builder.Append(" FROM SYS_FUNCPRNNUM ");
                    builder.AppendFormat(" WHERE FUNCNO = '{0}' AND FUNCID = '{1}' AND OPER='{2}';", bill, type, oper);
                    builder.Append(" IF LN_NUM > 0 THEN ");
                    builder.AppendFormat(" UPDATE SYS_FUNCPRNNUM SET FUNCTIME = FUNCTIME+1,OPERUSER='{2}',OPERTIME = SYSDATE WHERE FUNCNO = '{0}' AND FUNCID = '{1}' AND OPER='{3}'; ", bill, type, user, oper);
                    builder.Append(" ELSE ");
                    builder.AppendFormat(" INSERT INTO SYS_FUNCPRNNUM(OPER,FUNCID,FUNCNO,FUNCTIME,OPERUSER,OPERTIME) VALUES('{0}','{1}','{2}',1,'{3}',SYSDATE); ", oper, type, bill, user);
                    builder.Append(" END IF; ");
                    builder.Append(" EXCEPTION ");
                    builder.Append(" WHEN NO_DATA_FOUND THEN ");
                    builder.Append(" NULL;");
                    builder.Append("END ;");
                    cmdList.Add(new CommandInfo(builder.ToString(), null));
                }
                DbHelperOra.ExecuteSqlTran(cmdList);
            }
            else
            {
                PubFunc.OperLog("LOG", "PrintReport.EidtPrintNum", user, string.Format("【科室申领拣货】页面传递过来的参数值为：user=[{0}],osid=[{1}],oper=[{2}]", user, osid, oper));
            }
        }
        public void GetSYDataBill()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string lry = Request["user"] == null ? "" : Request["user"].ToString();
            string sql = @"select a.seqno,
                                           F_GETDEPTNAME(a.deptout) deptoutname,
                                           F_GETDEPTNAME(a.deptid) DEPTIDNAME,
                                           a.xsrq,
                                           b.gdseq,
                                           b.gdname,
                                           b.unit,
                                           b.gdspec,
                                           b.bzsl,
                                           b.hsjj,g.bar2,
                                           F_GETPRODUCERNAME(b.producer) producername,
                                           b.rowno,
                                           ds.supname,
                                           to_char(a.shrq, 'YYYY-MM-DD') DJRQ,
                                           to_char(sysdate, 'YYYY-MM-DD') printdate,
                                           F_GETUNITNAME(b.unit) unitname,
                                           b.bzhl * b.hsjj HSJE,
                                           F_GETUSERNAME(a.lry) ZDY,
                                           F_GETPARA('SUPPER') || '手术材料使用登记表' DT,
                                           b.ph,
                                           b.yxqz,
                                           a.custid,
                                           a.str6,
                                            DECODE(G.ISFLAG1,'N','否','是') SFZR,
                                           f_getusername('{1}') LRY,
                                           (SELECT SUM(HSJE) FROM dat_xs_com WHERE SEQNO = '{0}') SUMHSJE
                                      from dat_xs_doc a,
                                           dat_xs_com b,
                                           doc_goods g,
                                           (select * from doc_supplier where issupplier = 'Y') ds
                                     where a.seqno = b.seqno
                                       and b.gdseq = g.gdseq
                                       and g.supplier = ds.supid(+)  AND A.SEQNO='{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid, lry), false);
        }

        public void GetGZSYMX()
        {
            string GDSEQ = Request["GDSEQ"] == null ? "" : Request["GDSEQ"].ToString();
            string DEPTID = Request["DEPTID"] == null ? "" : Request["DEPTID"].ToString();
            string NAME = Request["NAME"] == null ? "" : Request["NAME"].ToString();
            string ZYH = Request["ZYH"] == null ? "" : Request["ZYH"].ToString();
            string DATE1 = Request["DATE1"] == null ? "" : Request["DATE1"].ToString();
            string DATE2 = Request["DATE2"] == null ? "" : Request["DATE2"].ToString();
            string user = Request["user"] == null ? "" : Request["user"].ToString();
            string strSql = @"SELECT C.GDSEQ,
                               C.GDNAME,
                               E.ZJM,
                               C.GDSPEC,
                               C.UNIT,
                               f_getunitname(C.UNIT) UNITNAME,
                               C.HSJJ,
                               ABS(D.SL) SL,
                               ABS(D.HSJE) HSJE,
                               E.PPID,
                               D.SUPID,
                               f_getsuppliername(D.SUPID) SUPPLIERNAME,
                               A.DEPTID,
                               f_getdeptname(A.DEPTID) DEPTNAME,
                               A.CUSTID,
                               A.STR7
                          from DAT_XS_DOC A, SYS_CODEVALUE B,DAT_XS_COM C,DAT_GOODSJXC D,DOC_GOODS E
                         WHERE A.FLAG = B.CODE
                           AND A.SEQNO=C.SEQNO 
                           AND C.SEQNO = D.BILLNO 
                           AND C.ROWNO = D.ROWNO
                           AND C.GDSEQ=E.GDSEQ
                           AND B.TYPE = 'DIC_BILLSTATUS'
                           AND A.BILLTYPE = 'XSG'
                           AND A.XSTYPE = '1'
                           AND A.FLAG='J' ";
            strSql += string.Format(@"AND A.deptid in (select code
                                            FROM SYS_DEPT
                                           where type <> '1'
                                             and F_CHK_DATARANGE(CODE, '{0}') = 'Y')", user);
            if (!PubFunc.StrIsEmpty(GDSEQ.Trim()))
                strSql += " and (C.GDSEQ like '" + GDSEQ.Trim() + "' OR C.GDSEQ like '" + GDSEQ.Trim().ToUpper() + "' OR C.GDSEQ like '" + GDSEQ.Trim().ToLower()
                       + "' OR C.GDNAME like '" + GDSEQ.Trim() + "' OR C.GDNAME like '" + GDSEQ.Trim().ToUpper() + "' OR C.GDNAME like '" + GDSEQ.Trim().ToLower()
                       + "' OR E.ZJM like '" + GDSEQ.Trim() + "' OR E.ZJM like '" + GDSEQ.Trim().ToUpper() + "' OR E.ZJM like '" + GDSEQ.Trim().ToLower() + "')";
            if (!PubFunc.StrIsEmpty(DEPTID.Trim())) strSql += " and A.DEPTID = '" + DEPTID + "'";
            if (!PubFunc.StrIsEmpty(NAME.Trim())) strSql += " and A.DEPTID LIKE '" + NAME + "'";
            if (!PubFunc.StrIsEmpty(ZYH.Trim())) strSql += " and A.DEPTID LIKE '" + ZYH.Trim() + "'";
            strSql += string.Format(@" AND A.XSRQ >= TO_DATE('" + DATE1.Trim() + "', 'YYYY-MM-DD')");
            strSql += string.Format(@" AND A.XSRQ < TO_DATE('" + DATE2.Trim() + "', 'YYYY-MM-DD') + 1");
            strSql += string.Format(@" ORDER BY C.GDSEQ DESC");
            OracleReportData.GenNodeXmlData(this, string.Format(strSql), false);
        }
        public void GetGZSYHZ()
        {
            string GDSEQ = Request["GDSEQ"] == null ? "" : Request["GDSEQ"].ToString();
            string DEPTID = Request["DEPTID"] == null ? "" : Request["DEPTID"].ToString();
            string DATE1 = Request["DATE1"] == null ? "" : Request["DATE1"].ToString();
            string DATE2 = Request["DATE2"] == null ? "" : Request["DATE2"].ToString();
            string user = Request["user"] == null ? "" : Request["user"].ToString();
            string strSql = @"SELECT C.GDSEQ,
                               C.GDNAME,
                               E.ZJM,
                               C.GDSPEC,
                               C.UNIT,
                               f_getunitname(C.UNIT) UNITNAME,
                               C.HSJJ,
                               SUM(ABS(D.SL) )SL,
                               SUM(ABS(D.HSJE) )HSJE,
                               E.PPID,
                               D.SUPID,
                               f_getsuppliername(D.SUPID) SUPPLIERNAME,
                               A.DEPTID,
                               f_getdeptname(A.DEPTID) DEPTNAME
                          from DAT_XS_DOC A, SYS_CODEVALUE B,DAT_XS_COM C,DAT_GOODSJXC D,DOC_GOODS E
                         WHERE A.FLAG = B.CODE
                           AND A.SEQNO=C.SEQNO 
                           AND C.SEQNO = D.BILLNO 
                           AND C.ROWNO = D.ROWNO
                           AND C.GDSEQ=E.GDSEQ
                           AND B.TYPE = 'DIC_BILLSTATUS'
                           AND A.BILLTYPE = 'XSG'
                           AND A.XSTYPE = '1'
                           AND A.FLAG='J' ";
            strSql += string.Format(@"AND A.deptid in (select code
                                            FROM SYS_DEPT
                                           where type <> '1'
                                             and F_CHK_DATARANGE(CODE, '{0}') = 'Y')", user);
            if (!PubFunc.StrIsEmpty(GDSEQ.Trim()))
                strSql += " and (C.GDSEQ like '" + GDSEQ.Trim() + "' OR C.GDSEQ like '" + GDSEQ.Trim().ToUpper() + "' OR C.GDSEQ like '" + GDSEQ.Trim().ToLower()
                       + "' OR C.GDNAME like '" + GDSEQ.Trim() + "' OR C.GDNAME like '" + GDSEQ.Trim().ToUpper() + "' OR C.GDNAME like '" + GDSEQ.Trim().ToLower()
                       + "' OR E.ZJM like '" + GDSEQ.Trim() + "' OR E.ZJM like '" + GDSEQ.Trim().ToUpper() + "' OR E.ZJM like '" + GDSEQ.Trim().ToLower() + "')";
            if (!PubFunc.StrIsEmpty(DEPTID)) strSql += " and A.DEPTID = '" + DEPTID + "'";
            strSql += string.Format(@" AND A.XSRQ >= TO_DATE('" + DATE1.Trim() + "', 'YYYY-MM-DD')");
            strSql += string.Format(@" AND A.XSRQ < TO_DATE('" + DATE2.Trim() + "', 'YYYY-MM-DD') + 1");
            strSql += string.Format(@" GROUP BY C.GDSEQ,C.GDNAME,E.ZJM,C.GDSPEC,C.UNIT,f_getunitname(C.UNIT),C.HSJJ,E.PPID,D.SUPID, f_getsuppliername(D.SUPID),A.DEPTID,f_getdeptname(A.DEPTID)");
            strSql += string.Format(@" ORDER BY C.GDSEQ DESC");
            OracleReportData.GenNodeXmlData(this, string.Format(strSql), false);
        }
        public void GetGZYRKSH()
        {
            string GDSEQ = Request["GDSEQ"] == null ? "" : Request["GDSEQ"].ToString();
            string DEPTID = Request["DEPTID"] == null ? "" : Request["DEPTID"].ToString();
            string RKDNO = Request["RKDNO"] == null ? "" : Request["RKDNO"].ToString();
            string BILLNO = Request["BILLNO"] == null ? "" : Request["BILLNO"].ToString();
            string DATE1 = Request["DATE1"] == null ? "" : Request["DATE1"].ToString();
            string DATE2 = Request["DATE2"] == null ? "" : Request["DATE2"].ToString();
            string user = Request["user"] == null ? "" : Request["user"].ToString();
            string strSql = @"SELECT A.SEQNO 预入库单号,
                                       A.ROWNO 行号,
                                       A.GDSEQ 商品编号,
                                       A.GDNAME 商品名称,
                                       A.GDSPEC 商品规格,
                                       F_GETUNITNAME(A.UNIT) 单位,
                                       F_GETDEPTNAME(A.HWID) 收货库房,
                                       A.BZHL 包装含量,
                                       A.BZSL 包装数量,
                                       A.SSSL 入库数量,
                                       (A.SSSL-D.BZSL) 供应商带回数量,
                                       A.HSJJ 含税进价,
                                       A.HSJE 含税金额,
                                       A.PH 批号,
                                       TO_CHAR(A.RQ_SC, 'YYYY-MM-DD') 生产日期,
                                       TO_CHAR(A.YXQZ, 'YYYY-MM-DD') 有效期至,
                                       F_GETSUPNAME(C.PRODUCER) 生产商,
                                       C.HISCODE HIS编码,
                                       D.SEQNO 跟台使用单号,
                                       F_GETDEPTNAME(E.DEPTID) 使用科室,
                                       D.BZSL 使用数量,
                                       TO_CHAR(E.XSRQ, 'YYYY-MM-DD') 使用日期
                                  FROM DAT_YRK_COM A,
                                       DAT_YRK_DOC B,
                                       DOC_GOODS   C,
                                       DAT_CK_COM  D,
                                       DAT_CK_DOC  E
                                 WHERE A.SEQNO = B.SEQNO
                                   AND A.GDSEQ = C.GDSEQ
                                   AND D.SEQNO = E.SEQNO(+)
                                   AND A.SEQNO = D.STR3(+)
                                   AND A.GDSEQ = D.GDSEQ(+) ";
            strSql += string.Format(@"AND E.deptid in (select code
                                            FROM SYS_DEPT
                                           where type <> '1'
                                             and F_CHK_DATARANGE(CODE, '{0}') = 'Y')", user);
            if (!PubFunc.StrIsEmpty(RKDNO.Trim())) strSql += " and A.SEQNO LIKE '%" + RKDNO.Trim() + "%'";
            if (!PubFunc.StrIsEmpty(BILLNO.Trim())) strSql += " and D.SEQNO LIKE '%" + BILLNO.Trim() + "%'";
            if (!PubFunc.StrIsEmpty(GDSEQ.Trim()))
                strSql += " and (A.GDSEQ like '%" + GDSEQ.Trim() + "%' OR A.GDSEQ like '%" + GDSEQ.Trim().ToUpper() + "%' OR A.GDSEQ like '%" + GDSEQ.Trim().ToLower()
                       + "%' OR A.GDNAME like '%" + GDSEQ.Trim() + "%' OR A.GDNAME like '%" + GDSEQ.Trim().Trim().ToUpper() + "%' OR A.GDNAME like '%" + GDSEQ.Trim().ToLower()
                       + "%' OR C.ZJM like '%" + GDSEQ.Trim() + "%' OR C.ZJM like '%" + GDSEQ.Trim().ToUpper() + "%' OR C.ZJM like '%" + GDSEQ.Trim().ToLower() + "%')";
            if (!PubFunc.StrIsEmpty(DEPTID))
            {
                strSql += " and E.DEPTID = '" + DEPTID + "'";
            }
            strSql += string.Format(@" AND E.XSRQ >= TO_DATE('" + DATE1.Trim() + "', 'YYYY-MM-DD')");
            strSql += string.Format(@" AND E.XSRQ < TO_DATE('" + DATE2.Trim() + "', 'YYYY-MM-DD') + 1");
            strSql += string.Format(@" ORDER BY A.SEQNO, A.ROWID");
            OracleReportData.GenNodeXmlData(this, string.Format(strSql), false);
        }
        public void InventoryAllocation()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            //string sql = @"SELECT   A.*,A.XSSL*A.HSJJ HSJE,B.SUMPH,NVL(B.SUMSL,0) SUMSL,A.XSSL - NVL(B.SL,0) DCSL,SYSDATE PRINTDATE
            //                    FROM (SELECT Q.DEPTID,Q.DEPTOUT,Q.GDSEQ,Q.GDNAME,Q.GDSPEC,f_getdeptname(Q.DEPTID) DEPTIDNAME,f_getunitname(Q.UNIT) UNITNAME,f_getproducername(Q.PRODUCER) PRODUCERNAME,Q.HSJJ,SUM(Q.XSSL) XSSL
            //                          FROM V_KCSP Q
            //                          WHERE Q.SEQNO IN(SELECT A.COLUMN_VALUE FROM TABLE(f_split('{0}')) A)
            //                          GROUP BY Q.DEPTID,Q.DEPTOUT,Q.GDSEQ,Q.GDNAME,Q.GDSPEC,Q.UNIT,Q.PRODUCER,Q.HSJJ) A,
            //                        (SELECT K.DEPTID,K.GDSEQ,TO_CHAR(wmsys.wm_concat(K.PHID)) SUMPH,TO_CHAR(wmsys.wm_concat(ABS(K.LOCKKCSL))) SUMSL,SUM(ABS(K.LOCKKCSL)) SL
            //                         FROM (SELECT K.DEPTID,K.GDSEQ,K.PHID,SUM(ABS(K.LOCKKCSL)) LOCKKCSL
            //                              FROM DAT_STOCKLOCK K
            //                             WHERE K.LOCKBILLNO IN(SELECT A.COLUMN_VALUE FROM TABLE(f_split('{0}')) A)
            //                             GROUP BY K.DEPTID,K.GDSEQ,K.PHID) K 
            //                         GROUP BY K.DEPTID,K.GDSEQ) B
            //                    WHERE A.DEPTOUT = B.DEPTID(+) AND A.GDSEQ = B.GDSEQ(+)
            //                    ORDER BY A.DEPTIDNAME";

            //根据商品资料的出库包装来打印
            string sql = @"SELECT   F_GETPARA('SUPPER')|| '预出库拣货单' DT,A.*,A.XSSL*A.HSJJ HSJE,B.SUMPH,NVL(B.SUMSL,0) SUMSL,A.XSSL - NVL(B.SL,0) DCSL,SYSDATE PRINTDATE,(select SUM(HSJE) FROM V_KCSP WHERE SEQNO IN(SELECT A.COLUMN_VALUE FROM TABLE(f_split('{0}')) A)) sumhjje
                                FROM (SELECT Q.FPTYPE,Q.DEPTID,Q.DEPTOUT,Q.GDSEQ,Q.GDNAME,Q.GDSPEC,f_getdeptname(Q.DEPTID) DEPTIDNAME,F_GETUNITNAME(Q.UNIT) UNITNAME,f_getproducername(Q.PRODUCER) PRODUCERNAME,Q.HSJJ,SUM(Q.XSSL) XSSL,Q.SEQNO
                                      FROM V_KCSP Q
                                      WHERE Q.FPTYPE='1' AND Q.SEQNO IN(SELECT A.COLUMN_VALUE FROM TABLE(f_split('{0}')) A)
                                      GROUP BY Q.DEPTID,Q.DEPTOUT,Q.GDSEQ,Q.GDNAME,Q.GDSPEC,Q.UNIT,Q.PRODUCER,Q.HSJJ,Q.FPTYPE,Q.SEQNO
                                        UNION ALL
                                      SELECT Q.FPTYPE,Q.DEPTID,Q.DEPTOUT,Q.GDSEQ,Q.GDNAME,Q.GDSPEC,f_getdeptname(Q.DEPTID) DEPTIDNAME,f_getunitname(B.UNIT) UNITNAME,f_getproducername(Q.PRODUCER) PRODUCERNAME,Q.HSJJ,SUM(Q.XSSL)/Q.BZHL XSSL,Q.SEQNO
                                      FROM V_KCSP Q,DAT_SL_COM B
                                      WHERE Q.SEQNO = B.SEQNO AND B.GDSEQ = Q.GDSEQ AND Q.FPTYPE='0' AND Q.SEQNO IN(SELECT A.COLUMN_VALUE FROM TABLE(f_split('{0}')) A)
                                      GROUP BY Q.DEPTID,Q.DEPTOUT,Q.GDSEQ,Q.GDNAME,Q.GDSPEC,Q.UNIT,Q.PRODUCER,Q.HSJJ,Q.FPTYPE,Q.BZHL,B.UNIT,Q.SEQNO
                                        ) A,
                                    (SELECT K.DEPTID,K.GDSEQ,TO_CHAR(wmsys.wm_concat(K.PHID)) SUMPH,TO_CHAR(wmsys.wm_concat(ABS(K.LOCKKCSL))) SUMSL,SUM(ABS(K.LOCKKCSL)) SL,K.LOCKBILLNO
                                     FROM (SELECT K.DEPTID,K.GDSEQ,K.PHID,SUM(ABS(K.LOCKKCSL)) LOCKKCSL,K.LOCKBILLNO
                                          FROM DAT_STOCKLOCK K
                                         WHERE K.LOCKBILLNO IN(SELECT A.COLUMN_VALUE FROM TABLE(f_split('{0}')) A)
                                         GROUP BY K.DEPTID,K.GDSEQ,K.PHID,K.LOCKBILLNO) K 
                                     GROUP BY K.DEPTID,K.GDSEQ,K.LOCKBILLNO) B
                                WHERE A.DEPTOUT = B.DEPTID(+) AND A.GDSEQ = B.GDSEQ(+) AND A.SEQNO=B.LOCKBILLNO(+)
                                ORDER BY A.DEPTIDNAME";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }

        public void GetGoodsDX_MX()
        {
            //商品动销明细     
            string GDSEQ = Request["gdseq"] == null ? "" : Request["gdseq"].ToString();
            string DEPTID = Request["deptid"] == null ? "" : Request["deptid"].ToString();
            string isgz = Request["isgz"] == null ? "" : Request["isgz"].ToString();
            string dpkTime1 = Request["dpktime1"] == null ? "" : Request["dpktime1"].ToString();
            string dpkTime2 = Request["dpktime2"] == null ? "" : Request["dpktime2"].ToString();
            string UserID = Request["userid"] == null ? "" : Request["userid"].ToString();
            string strSql = @"SELECT t.*,rownum from (
SELECT A.RQSJ,f_getdeptname(A.DEPTID) DEPTNAME,B.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,B.HSJJ,f_getproducername(B.PRODUCER) PRODUCERNAME,
                            A.SL,A.HSJE,A.PH,A.RQ_SC,A.YXQZ,'{0}'||' 至 '||'{1}'||'商品动销明细' DT
                    FROM DAT_GOODSJXC A,DOC_GOODS B
                    WHERE A.RQSJ BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND TO_DATE('{1}','YYYY-MM-DD')  + 1
                    AND A.GDSEQ = B.GDSEQ AND F_CHK_DATARANGE(A.DEPTID, '{2}') = 'Y'";
            string strWhere = " ";
            if (GDSEQ != "") strWhere += string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME  LIKE '%{0}%' OR B.ZJM  LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%')", GDSEQ.ToUpper());
            if (DEPTID != "") strWhere += string.Format(" AND A.DEPTID = '{0}'", DEPTID.Trim());
            if (isgz != "") strWhere += String.Format(" AND B.ISGZ = '{0}'", isgz.Trim());

            strSql = string.Format(strSql + strWhere + "order by rqsj desc ) t  ", dpkTime1, dpkTime2, UserID);

            OracleReportData.GenNodeXmlData(this, strSql, false);
        }

        public void GetGoodsDX_HZ()
        {
            //商品动销明细     
            string GDSEQ = Request["gdseq"] == null ? "" : Request["gdseq"].ToString();
            string DEPTID = Request["deptid"] == null ? "" : Request["deptid"].ToString();
            string isgz = Request["isgz"] == null ? "" : Request["isgz"].ToString();
            string dpkTime1 = Request["dpktime1"] == null ? "" : Request["dpktime1"].ToString();
            string dpkTime2 = Request["dpktime2"] == null ? "" : Request["dpktime2"].ToString();
            string UserID = Request["userid"] == null ? "" : Request["userid"].ToString();
            string strSql = @"select t.*, rownum from (
SELECT A.DEPTID,f_getdeptname(A.DEPTID) DEPTNAME,B.GDSEQ,B.GDNAME,B.GDSPEC,f_getunitname(B.UNIT) UNITNAME,B.HSJJ,f_getproducername(B.PRODUCER) PRODUCERNAME,
                           SUM(DECODE(A.KCADD,'1',A.SL,0)) RKSL,SUM(DECODE(A.KCADD,'1',A.HSJE,0)) RKJE,
                           SUM(DECODE(A.KCADD,'-1',DECODE(A.BILLTYPE,'XST',0，'LTD',0,'DST',0,'THD',0,A.SL),0)) CKSL,SUM(DECODE(A.KCADD,'-1',DECODE(A.BILLTYPE,'XST',0，'LTD',0,'DST',0,'THD',0,A.HSJE),0)) CKJE,
                           SUM(DECODE(A.KCADD,'-1',DECODE(A.BILLTYPE,'XST',A.SL，'LTD',A.SL,'DST',A.SL,'THD',A.SL,0),0)) THSL,SUM(DECODE(A.KCADD,'-1',DECODE(A.BILLTYPE,'XST',A.HSJE，'LTD',A.HSJE,'DST',A.HSJE,'THD',A.HSJE,0),0)) THJE,
                           NVL((SELECT SUM(KCSL) FROM DAT_GOODSSTOCK WHERE DEPTID = A.DEPTID AND GDSEQ = B.GDSEQ),0) KCSL,
                           NVL((SELECT SUM(KCSL) FROM DAT_GOODSSTOCK WHERE DEPTID = A.DEPTID AND GDSEQ = B.GDSEQ),0)*B.HSJJ KCJE,
                           '{0}'||' 至 '||'{1}'||'商品动销汇总' DT
                    FROM DAT_GOODSJXC A,DOC_GOODS B
                    WHERE A.RQSJ BETWEEN TO_DATE('{0}','YYYY-MM-DD') AND TO_DATE('{1}','YYYY-MM-DD') + 1
                    AND A.GDSEQ = B.GDSEQ AND F_CHK_DATARANGE(A.DEPTID, '{2}') = 'Y'";
            string strWhere = " ";
            if (GDSEQ != "") strWhere += string.Format(" AND (B.GDSEQ LIKE '%{0}%' OR B.GDNAME  LIKE '%{0}%' OR B.ZJM  LIKE '%{0}%' OR B.BAR3 LIKE '%{0}%')", GDSEQ.ToUpper());
            if (DEPTID != "") strWhere += string.Format(" AND A.DEPTID = '{0}'", DEPTID.Trim());
            if (isgz != "") strWhere += String.Format(" AND B.ISGZ = '{0}'", isgz.Trim());

            strSql = string.Format(strSql + strWhere + " GROUP BY A.DEPTID,B.GDSEQ,B.GDNAME,B.GDSPEC,B.UNIT,B.HSJJ,B.PRODUCER ) t  ", dpkTime1, dpkTime2, UserID);

            OracleReportData.GenNodeXmlData(this, strSql, false);
        }

        public void GetCKDData()
        {
            string date = Request["d"] == null ? "" : Request["d"].ToString().Replace(",", "','");
            string sql = @"SELECT  B.JSBILLNO SEQNO,A.SEQNO BILLNO,
                                               F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                               F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                               A.XSRQ,
                                               B.GDSEQ,
                                               B.GDNAME,
                                               B.GDSPEC,
                                               B.XSSL,
                                               B.HSJJ,
                                               B.PSSID SUPPLIER,
                                               F_GETSUPNAME(B.PSSID) SUPNAME,
                                               F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                               B.ROWNO,
                                               B.STR1,
                                               SYSDATE PRINTDATE,
                                               F_GETUNITNAME(B.UNIT) UNITNAME,
                                               HSJE,
                                               F_GETUSERNAME(A.LRY) ZDY,
                                               F_GETPARA('SUPPER') || '科室使用出库单' DT,
                                               B.PH,
                                               B.YXQZ,
                                               SUM(B.HSJJ * B.XSSL) OVER(PARTITION BY A.SEQNO) SUMHJJE,
                                               F_GETPRINTINF(B.GDSEQ, 'HSJJ', 0) || '元/' ||
                                               F_GETPRINTINF(B.GDSEQ, 'UNITNAME', 0) V_HSJJ,
                                               F_GETHWID(A.DEPTOUT, B.GDSEQ) HWID
                                          FROM DAT_JZ_DOC A, DAT_JZ_COM B
                                         WHERE A.SEQNO = B.SEQNO AND A.FLAG='Y' AND A.SEQNO IN ('{0}') 
                                          ORDER BY A.SEQNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, date), false);
        }

        public void GetCKHData()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString().Replace(",", "','");
            string sql = @"SELECT B.JSBILLNO SEQNO,
                                               A.BILLNO,
                                               F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                               F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                               A.STR1 BEGRQ,
                                               A.STR2 ENDRQ,
                                               B.GDSEQ,
                                               B.GDNAME,
                                               B.GDSPEC,
                                               B.XSSL,
                                               B.HSJJ,
                                               B.PSSID SUPPLIER,
                                               F_GETSUPNAME(B.PSSID) SUPNAME,
                                               F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                               B.ROWNO,
                                               B.STR1 BARCODE,
                                               F_GETUNITNAME(B.UNIT) UNITNAME,
                                               HSJE,
                                               F_GETUSERNAME(A.LRY) ZDY,
                                               F_GETPARA('SUPPER') || '科室使用出库单' DT,
                                               B.PH,
                                               B.YXQZ,
                                               SUM(B.HSJJ * B.XSSL) OVER(PARTITION BY A.BILLNO) SUMHJJE
                                          FROM DAT_JZ_DOC A, DAT_JZ_COM B
                                         WHERE A.SEQNO = B.SEQNO
                                           AND A.FLAG = 'Y'
                                           AND A.SEQNO IN ('{0}')
                                         ORDER BY B.SEQNO DESC,B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }

        public void GetCKDData_GYS()
        {
            string gys = Request["g"] == null ? "" : Request["g"].ToString();
            string date = Request["rq"] == null ? "" : Request["rq"].ToString();
            string[] rq = date.Split(',');
            string sql = @"SELECT AB.*, CD.SUMHJJE
                                    FROM (SELECT B.JSBILLNO BILLNO,
                                                 F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                                                 F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                                 '{0}~{1}' XSRQ,
                                                 B.GDSEQ,
                                                 B.GDNAME,
                                                 B.GDSPEC,
                                                 B.XSSL,
                                                 B.HSJJ,
                                                 B.PSSID SUPPLIER,
                                                 F_GETSUPNAME(B.PSSID) SUPNAME,
                                                 F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                                 B.ROWNO,
                                                 B.STR1,
                                                 TO_CHAR(A.SHRQ, 'YYYY-MM-DD') DJRQ,
                                                 SYSDATE PRINTDATE,
                                                 F_GETUNITNAME(B.UNIT) UNITNAME,
                                                 HSJE,
                                                 F_GETUSERNAME(A.LRY) ZDY,
                                                 F_GETPARA('SUPPER') || '出库单' DT,
                                                 B.PH,
                                                 B.YXQZ
                                            FROM DAT_JZ_DOC A, DAT_JZ_COM B, DAT_XS_DOC C
                                           WHERE A.SEQNO = B.SEQNO
                                             AND C.SEQNO = B.JSBILLNO  {2}
                                             AND C.XSRQ >= TO_DATE('{0}', 'YYYY-MM-DD')
                                             AND C.XSRQ < TO_DATE('{1}', 'YYYY-MM-DD')
                                           ORDER BY B.PSSID, A.DEPTID, A.SEQNO, B.ROWNO) AB,
                                         (SELECT B.PSSID, SUM(B.HSJJ * B.HSJE) SUMHJJE
                                            FROM DAT_XS_DOC A, DAT_JZ_COM B
                                           WHERE A.SEQNO = B.JSBILLNO {2}
                                             AND A.XSRQ >= TO_DATE('{0}', 'YYYY-MM-DD')
                                             AND A.XSRQ < TO_DATE('{1}', 'YYYY-MM-DD')
                                           GROUP BY B.PSSID) CD
                                   WHERE AB.SUPPLIER = CD.PSSID";
            string gys_w = "";
            if (!string.IsNullOrWhiteSpace(gys))
            {
                gys_w = " AND B.PSSID='" + gys + "'";
            }
            OracleReportData.GenNodeXmlData(this, string.Format(sql, rq[0], rq[1], gys_w), false);
        }

        public void GetShelf()
        {
            string shelf = Request["hw"] == null ? "ALL" : Request["hw"].ToString();
            string dept = Request["bm"] == null ? "" : Request["bm"].ToString();
            string dhlx = Request["dh"] == null ? "" : Request["dh"].ToString();
            string gdseq = Request["gd"] == null ? "" : Request["gd"].ToString();
            string order = "";
            if (gdseq.Trim().Length > 0)
            {
                gdseq = gdseq.Replace(",", "','");
                order += " AND A.GDSEQ IN ('" + gdseq + "') ";
            }
            order += "ORDER BY B.HJCODE1";
            if (dhlx == "6")
            {
                string sql = @"SELECT A.GDSEQ,
                                            A.GDNAME,
                                            A.GDSPEC,
                                            A.LOGINLABEL,
                                            F_GETUNITNAME(A.UNIT) UNIT,
                                            F_GETPRODUCERNAME(A.PRODUCER) PRODUCER,
                                            B.HJCODE1,
                                            (select SUM(KCSL) KCSL
                                                from DAT_GOODSSTOCK
                                                WHERE DEPTID = B.DEPTID
                                                AND GDSEQ = B.GDSEQ) KCSL
                                        FROM DOC_GOODS A, DOC_GOODSCFG B
                                        WHERE A.GDSEQ = B.GDSEQ
                                        AND B.DEPTID = '{0}'
                                        AND INSTR('{1}', B.HJCODE1) > 0
                                        AND A.CATID0 = {2}
                                        ";
                if (shelf.ToUpper() == "ALL")
                {
                    sql = @"SELECT A.GDSEQ,
                                           A.GDNAME,
                                           A.GDSPEC,
                                           A.LOGINLABEL,
                                           F_GETUNITNAME(A.UNIT) UNIT,
                                           F_GETPRODUCERNAME(A.PRODUCER) PRODUCER,
                                           B.HJCODE1,
                                           (select SUM(KCSL) KCSL
                                              from DAT_GOODSSTOCK
                                             WHERE DEPTID = B.DEPTID
                                               AND GDSEQ = B.GDSEQ) KCSL
                                      FROM DOC_GOODS A, DOC_GOODSCFG B
                                     WHERE A.GDSEQ = B.GDSEQ
                                       AND B.DEPTID = '{0}'
                                       AND A.CATID0 = {1} ";
                    sql += order;
                    OracleReportData.GenNodeXmlData(this, string.Format(sql, dept, dhlx), false);
                }
                else
                {
                    sql += order;
                    OracleReportData.GenNodeXmlData(this, string.Format(sql, dept, shelf, dhlx), false);
                }
            }
            else
            {
                string sql = @"SELECT A.GDSEQ, A.GDNAME, A.GDSPEC, A.LOGINLABEL,  F_GETUNITNAME(A.UNIT) UNIT, F_GETPRODUCERNAME(A.PRODUCER) PRODUCER, B.HJCODE1, B.ZDKC, B.ZGKC
                                    FROM DOC_GOODS A, DOC_GOODSCFG B WHERE A.GDSEQ = B.GDSEQ AND B.DEPTID = '{0}' AND INSTR('{1}',B.HJCODE1)>0 AND A.CATID0={2} ";
                if (shelf.ToUpper() == "ALL")
                {
                    sql = @"SELECT A.GDSEQ, A.GDNAME, A.GDSPEC, A.LOGINLABEL,  F_GETUNITNAME(A.UNIT) UNIT, F_GETPRODUCERNAME(A.PRODUCER) PRODUCER, B.HJCODE1, B.ZDKC, B.ZGKC
                              FROM DOC_GOODS A, DOC_GOODSCFG B WHERE A.GDSEQ = B.GDSEQ AND B.HJCODE1<> B.DEPTID AND B.DEPTID = '{0}' AND A.CATID0={1} ";
                    sql += order;
                    OracleReportData.GenNodeXmlData(this, string.Format(sql, dept, dhlx), false);
                }
                else
                {
                    sql += order;
                    OracleReportData.GenNodeXmlData(this, string.Format(sql, dept, shelf, dhlx), false);
                }
            }

        }

        public void GetKSZRCLData()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,
                                               F_GETDEPTNAME(A.DEPTID) DEPTINNAME,
                                               B.GDSEQ,
                                               B.GDNAME,
                                               B.GDSPEC,
                                               F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                               B.ROWNO,
                                               B.BZSL,A.OPERNAME,
                                               F_GETPARA('SUPPER')|| '植入材料使用申请表' DT,
                                               TO_CHAR(A.XSRQ, 'YYYY-MM-DD') SLRQ,
                                               A.PATIENT,
                                               A.PATIENTID
                                          FROM DAT_SL_DOC A, DAT_SL_COM B
                                         WHERE A.SEQNO = B.SEQNO AND A.SEQNO='{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }

        public void GetJSDData()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string flag = Request["flag"] == null ? "ph" : Request["flag"].ToString();
            string title = "医用材料结算入库单";
            if (flag == "pj")
            {
                title = "设备维修（配件）结算单";
            }

            string sql = @"SELECT A.SEQNO, B.ROWNO, A.BEGRQ, A.ENDRQ, C.LEADER,
                                              --F_GETUSERNAME(A.LRY) LRYNAME,
                                              B.GDSEQ,B.GDNAME, B.GDSPEC,B.BZSL, B.HSJJ, B.HSJE,
                                              F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                              F_GETUNITNAME(B.UNIT) UNIT,C.TEL TELEPHONE,
                                              F_GETPARA('SUPPER')||
                                              SUBSTR(TO_CHAR(A.ENDRQ, 'YYYY-MM-DD'), 0, 4) || '年' ||
                                              SUBSTR(TO_CHAR(A.ENDRQ, 'YYYY-MM-DD'), 6, 2) || '月{1}' DT,
                                              C.SUPNAME CUSTNAME,
                                              SUM(B.HSJJ * B.BZSL) OVER(PARTITION BY A.SEQNO) SUMHJJE
                                    FROM DAT_JSD_DOC A, DAT_JSD_COM B,DOC_SUPPLIER C
                                  WHERE A.SEQNO = B.SEQNO AND A.SUPID = C.SUPID
                                      AND A.SEQNO IN ('{0}') AND C.ISSUPPLIER = 'Y'
                                   ORDER BY A.SEQNO DESC, B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid.Replace(",", "','"), title), false);
        }

        public void GetDeptStockOutHZ()
        {
            string deptid = Request["deptid"] == null ? "" : Request["deptid"].ToString();
            string isgz = Request["isgz"] == null ? "" : Request["isgz"].ToString();
            string begrq = Request["b"] == null ? DateTime.Now.ToString("yyyy-MM-dd") : Request["b"].ToString();
            string endrq = Request["e"] == null ? DateTime.Now.ToString("yyyy-MM-dd") : Request["e"].ToString();
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT D.NAME DEPTNAME,
                                                   B.GDSEQ,
                                                   B.GDNAME,
                                                   B.GDSPEC,
                                                   F_GETUNITNAME(B.UNIT) UNIT,
                                                   C.NAME CATID,
                                                   B.HSJJ,
                                                   '' ISSF,
                                                   F_GETPRODUCERNAME(B.PRODUCER) PRODUCER,
                                                   SUM(A.SL) SL,
                                                   SUM(A.SL * A.HSJJ) JE,
                                                   SUBSTR('{1}', 0, 4) || '年' ||
                                                   SUBSTR('{1}', 6, 2) ||  '月医用物资科室领用明细表' DT
                                              FROM DAT_GOODSJXC A, DOC_GOODS B, SYS_CATEGORY C, SYS_DEPT D
                                             WHERE A.GDSEQ = B.GDSEQ
                                               AND A.DEPTID = D.CODE
                                               AND B.CATID = C.CODE
                                               AND A.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                               AND A.DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE TYPE IN ('3', '4'))
                                               AND B.FLAG = 'Y' ", begrq, endrq);
            if (!string.IsNullOrWhiteSpace(deptid))
            {
                sbSql.AppendFormat(" AND A.DEPTID='{0}'", deptid);
            }
            if (!string.IsNullOrWhiteSpace(isgz))
            {
                sbSql.AppendFormat(" AND B.ISGZ='{0}'", isgz);
            }
            sbSql.Append(@"   GROUP BY D.NAME,
                                                      B.GDSEQ,
                                                      B.GDNAME,
                                                      B.GDSPEC,
                                                      B.UNIT,
                                                      B.HSJJ,
                                                      C.NAME,
                                                      B.PRODUCER
                                             ORDER BY D.NAME, B.GDNAME");
            OracleReportData.GenNodeXmlData(this, sbSql.ToString(), false);
        }

        public void GetDeptStockOutMX()
        {
            string deptid = Request["deptid"] == null ? "" : Request["deptid"].ToString();
            string gdseq = Request["gdseq"] == null ? "" : Request["gdseq"].ToString();
            string begrq = Request["b"] == null ? DateTime.Now.ToString("yyyy-MM-dd") : Request["b"].ToString();
            string endrq = Request["e"] == null ? DateTime.Now.ToString("yyyy-MM-dd") : Request["e"].ToString();
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT D.NAME DEPTNAME,
                                                   B.GDSEQ,
                                                   B.GDNAME,
                                                   B.GDSPEC,
                                                   F_GETUNITNAME(B.UNIT) UNIT,
                                                   C.NAME CATID,
                                                   B.HSJJ,B.HSID,
                                                   DECODE(B.ISCF,'Y','是','否') ISSF,
                                                   F_GETPRODUCERNAME(B.PRODUCER) PRODUCER,
                                                   (-SUM(DECODE(A.BILLTYPE,'XST',DECODE(A.KCADD,'1',A.SL,0),A.SL))) SL,
                                                   (-SUM(DECODE(A.BILLTYPE,'XST',DECODE(A.KCADD,'1',A.SL,0),A.SL) * A.HSJJ)) JE,
                                                   SUBSTR('{1}', 0, 4) || '年' ||
                                                   SUBSTR('{1}', 6, 2) || '月医用物资科室领用明细表' DT
                                              FROM DAT_GOODSJXC A, DOC_GOODS B, SYS_CATEGORY C, SYS_DEPT D
                                             WHERE A.GDSEQ = B.GDSEQ
                                               AND A.DEPTID = D.CODE
                                               AND B.CATID = C.CODE
                                               AND A.BILLTYPE IN ('DSH','XSD','XSG','XST')
                                               AND A.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                               AND A.DEPTID IN (SELECT CODE FROM SYS_DEPT WHERE TYPE IN ('3', '4'))
                                                ", begrq, endrq);
            if (!string.IsNullOrWhiteSpace(deptid))
            {
                sbSql.AppendFormat(" AND A.DEPTID='{0}'", deptid);
            }
            if (!string.IsNullOrWhiteSpace(gdseq))
            {
                sbSql.AppendFormat(" AND A.GDSEQ='{0}'", deptid);
            }
            sbSql.Append(@"   GROUP BY D.NAME,
                                                        B.GDSEQ,
                                                        B.GDNAME,
                                                        B.GDSPEC,B.ISCF,
                                                        B.UNIT,C.NAME,
                                                        B.HSJJ,B.HSID,
                                                        C.NAME,
                                                        B.PRODUCER
                                             ORDER BY D.NAME, B.GDNAME");
            OracleReportData.GenNodeXmlData(this, sbSql.ToString(), false);
        }

        public void GoodsOrderSum()
        {
            string begrq = Request["beg"] == null ? "" : Request["beg"].ToString();
            string endrq = Request["end"] == null ? "" : Request["end"].ToString();
            string dept = Request["bm"] == null ? "" : Request["bm"].ToString();
            string gys = Request["gys"] == null ? "" : Request["gys"].ToString();
            string isgz = Request["isgz"] == null ? "" : Request["isgz"].ToString();
            string user = Request["user"] == null ? "admin" : Request["user"].ToString();
            string detp_s = "", gys_s = "";
            if (!string.IsNullOrWhiteSpace(dept))
            {
                detp_s = " AND A.DEPTID = '" + dept + "'";
            }
            if (!string.IsNullOrWhiteSpace(gys))
            {
                gys_s = "  AND A.PSSID = '" + gys + "'";
            }
            string sql = @"SELECT A.SEQNO DDBH,
                                              F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                                              '{1}' BEGRQ,'{2}' ENDRQ,
                                              B.GDSEQ,
                                              B.GDNAME,
                                              B.UNIT,
                                              B.GDSPEC,
                                              B.BZHL,
                                              B.HSJJ,
                                              F_GETSUPNAME(B.SUPID) SUPNAME,
                                              F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                              TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTTIME,
                                              F_GETUNITNAME(B.UNIT) UNITNAME,
                                              B.BZSL * B.HSJJ HSJE,
                                              F_GETUSERNAME('{0}') ZDY,
                                              B.BZSL,
                                              F_GETPARA('SUPPER') || '采购订单' DT,
                                              F_GETPARA('SUPPER')KH,
                                              SUM(NVL(B.HSJJ * B.BZSL, 0)) OVER() SUBSUM
                                    FROM DAT_DD_DOC A, DAT_DD_COM B
                                   WHERE A.SEQNO = B.SEQNO AND A.FLAG IN ('Y','G')
                                       AND B.ISGZ LIKE '%{5}%'
                                       AND A.SHRQ BETWEEN TO_DATE('{1}', 'YYYY-MM-DD') AND
                                              TO_DATE('{2}', 'YYYY-MM-DD') + 1 {3} {4}
                                      ORDER BY A.SEQNO,B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, user, begrq, endrq, detp_s, gys_s, isgz), false);
        }

        public void GetTJDData()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString().Replace(",", "','");
            string sql = @"SELECT A.SEQNO,
                                              A.TJREASON,
                                              A.TJRQ,
                                              B.GDSEQ,
                                              B.GDNAME,
                                              B.GDSPEC,
                                              B.PIZNO PZWH,
                                              B.HSJJ YHSJJ,
                                              B.XHSJJ,
                                              F_GETCATNAME(B.CATID) CATNAME,
                                              F_GETSUPNAME(B.SUPID) SUPNAME,
                                              F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                              B.ROWNO,
                                              F_GETUNITNAME(B.UNIT) UNITNAME,
                                              F_GETUSERNAME(A.LRY) LRYNAME,
                                              F_GETPARA('SUPPER') || '商品调价单' DT
                                    FROM DAT_TJ_DOC A, DAT_TJ_COM B
                                  WHERE A.SEQNO = B.SEQNO
                                      AND A.FLAG = 'Y'
                                      AND A.SEQNO IN ('{0}')
                                   ORDER BY B.SEQNO DESC, B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }

        public void GetYDBB()
        {
            string beg = Request["beg"] == null ? "" : Request["beg"].ToString();
            string end = Request["end"] == null ? "" : Request["end"].ToString();
            string sup = Request["sup"] == null ? "" : Request["sup"].ToString();
            string sql = @"SELECT  '物资管理科' DEPT,
                                               '医疗物资库房'||SUBSTR('{1}', 0, 4) || '年' ||
                                               SUBSTR('{1}', 6, 2) || '月月报明细表' DT,
                                               ROWNO,
                                               BEGRQ,
                                               ENDRQ,
                                               SUPID,
                                               CATEGORYID,
                                               CATEGORYNAME,
                                               QCJE,
                                               BYRKJE,
                                               PYJE,
                                               BNLJRKJE,
                                               BYCKJE,
                                               PKJE,
                                               BSJE,
                                               BNLJCKJE,
                                               QMJE
                                          FROM TEMP_YYYDBB
                                         WHERE BEGRQ = '{0}' AND ENDRQ = '{1}'";
            if (!string.IsNullOrWhiteSpace(sup))
            {
                sql += " AND SUPID = '" + sup + "'";
            }
            sql += " ORDER BY ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, beg, end), false);
        }

        public void GetSupEvaluate()
        {
            string beg = Request["beg"] == null ? DateTime.Now.ToString("yyyy-MM-dd") : Request["beg"].ToString();
            string end = Request["end"] == null ? DateTime.Now.ToString("yyyy-MM-dd") : Request["end"].ToString();
            string sup = Request["sup"] == null ? "" : Request["sup"].ToString();

            string sql = "";
            if (!string.IsNullOrWhiteSpace(sup))
            {
                sql += " AND A.PSSID = '" + sup + "'";
            }

            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT A.SEQNO DDBH,
                                               B.DHS DHSL,
                                               A.DHRQ GDDHRQ,
                                               B.GDSEQ,
                                               B.GDNAME,
                                               B.UNIT,
                                               B.GDSPEC,
                                               B.HSJJ,
                                               F_GETSUPNAME(B.SUPID) SUPNAME,
                                               F_GETPRODUCERNAME(B.PRODUCER) PRODUCERNAME,
                                               F_GETUSERNAME(A.LRY) ZDY,
                                               '供应商医疗物资供货量化评分表' DT,
                                               C.RKSL,
                                               C.RKRQ SJDHRQ,
                                               C.PH,
                                               C.YXQZ
                                      FROM DAT_DD_DOC A,
                                               DAT_DD_COM B,
                                               (SELECT RA.DDBH,
                                                       RB.GDSEQ,
                                                       SUM(RB.SSSL) RKSL,
                                                       LISTAGG(TO_CHAR(RA.SHRQ, 'YYYY-MM-DD'), ',') WITHIN GROUP(ORDER BY RA.SHRQ) RKRQ,
                                                       LISTAGG(RB.PH, ',') WITHIN GROUP(ORDER BY RB.PH) PH,
                                                       LISTAGG(TO_CHAR(RB.YXQZ, 'YYYY-MM-DD'), ',') WITHIN GROUP(ORDER BY RB.YXQZ) YXQZ
                                                  FROM DAT_RK_DOC RA, DAT_RK_COM RB
                                                 WHERE RA.SEQNO = RB.SEQNO {2}
                                                     AND RA.FLAG IN ('Y', 'G')
                                                     AND RA.SHRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                            TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                 GROUP BY RA.DDBH, RB.GDSEQ) C
                                     WHERE A.SEQNO = B.SEQNO AND A.FLAG IN ('Y', 'C')
                                       AND B.SEQNO = C.DDBH(+)
                                       AND B.GDSEQ = C.GDSEQ(+) {2}
                                       AND A.SHRQ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                    ORDER BY A.PSSID, A.BILLNO", beg, end, sql);
            OracleReportData.GenNodeXmlData(this, string.Format(sql, beg, end), false);
        }

        public void GetZrcldjb()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();

            string[] billnos = osid.Split(',');
            string seq2 = "";
            if (billnos.Length == 2)
            {
                seq2 = billnos[1];
            }
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"select '{0}' SEQNO1,'{1}' SEQNO2,
                                                        F_GETPARA('SUPPER') || '植入材料使用登记表' DT
                                                from dual;", billnos[0], seq2);

            OracleReportData.GenNodeXmlData(this, sbSql.ToString(), false);
        }
        public void GetJCData()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,
                                               F_GETDEPTNAME(A.DEPTID) ||
                                               DECODE(A.MEMO, NULL, '', '（' || A.MEMO || '）') DEPTIDNAME,
                                               A.JCRQ,
                                               F_GETUSERNAME(A.LRY) LRYNAME,
                                               A.SUBNUM,
                                               B.ROWNO,
                                               B.GDSEQ,
                                               B.GDNAME,
                                               F_GETUNITNAME(B.UNIT) UNITNAME,
                                               B.GDSPEC,
                                               B.HSJJ,
                                               NVL(B.QCSL, 0) QCSL,
                                               NVL(B.QCJE, 0) QCJE,
                                               NVL(B.RKSL, 0) RKSL,
                                               NVL(B.RKJE, 0) RKJE,
                                               NVL(B.CKSL, 0) CKSL,
                                               NVL(B.CKJE, 0) CKJE,
                                               NVL(B.KFJCSL, 0) KFJCSL,
                                               NVL(B.KFJCJE, 0) KFJCJE,
                                               NVL(B.KFPDSL, 0) KFPDSL,
                                               NVL(B.KFPDJE, 0) KFPDJE,
                                               NVL(B.KSJCSL, 0) KSJCSL,
                                               NVL(B.KSJCJE, 0) KSJCJE,
                                               NVL(B.KSPDSL, 0) KSPDSL,
                                               NVL(B.KSPDJE, 0) KSPDJE,
                                               NVL(B.CYSL, 0) CYSL,
                                               NVL(B.CYJE, 0) CYJE,
                                               B.HWID,
                                               B.CATID,
                                               SUBSTR(TO_CHAR(A.JCRQ, 'YYYY-MM-DD'), 0, 4) || '年' ||
                                               SUBSTR(TO_CHAR(A.JCRQ, 'YYYY-MM-DD'), 6, 2) || '月医疗物资库房月度结存表' HISTROY
                                          FROM DAT_JC_DOC A, DAT_JC_COM B
                                         WHERE A.SEQNO = B.SEQNO
                                           AND a.SEQNO = b.SEQNO
                                           and a.SEQNO = '{0}'
                                         ORDER BY B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGoodsCJGT()
        {
            //跟台商品使用
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select a.seqno,
                                           F_GETDEPTNAME(a.deptout) deptoutname,
                                           F_GETDEPTNAME(a.deptid) DEPTIDNAME,
                                           a.xsrq,
                                           b.gdseq,
                                           b.gdname,
                                           b.unit,
                                           b.gdspec,
                                           b.bzsl,
                                           b.hsjj,
                                           g.bar2,
                                           F_GETPRODUCERNAME(b.producer) producername,
                                           b.rowno,
                                           ds.supname,
                                           to_char(a.shrq, 'YYYY-MM-DD') DJRQ,
                                           to_char(sysdate, 'YYYY-MM-DD') printdate,
                                           F_GETUNITNAME(b.unit) unitname,
                                           b.bzhl * b.hsjj HSJE,
                                           F_GETUSERNAME(a.lry) ZDY,
                                           F_GETPARA('SUPPER') || '植入材料使用登记表' DT,
                                           b.ph,
                                           b.yxqz,
                                           c.patient custid,
                                           c.str6
                                      from dat_ck_doc a,
                                           dat_ck_com b,
                                           dat_ck_ext c,
                                           doc_goods g,
                                           (select * from doc_supplier where issupplier = 'Y') ds
                                     where a.seqno = b.seqno
                                       and b.gdseq = g.gdseq and b.seqno=c.billno and b.rowno=c.rowno
                                       and g.supplier = ds.supid(+)
                                       AND A.SEQNO = '{0}' ";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetData_GZCKJH()
        {
            //F_GETPARA('SUPPER')
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select DISTINCT a.seqno,
                                F_GETDEPTNAME(a.deptout) deptoutname,
                                F_GETDEPTNAME(a.deptid) deptidname,
                                a.xsrq,
                                DHSL,
                                b.gdseq,
                                b.gdname,
                                b.unit,
                                b.gdspec,
                                b.bzhl,
                                b.hsjj,
                                b.pzwh,
                                F_GETPRODUCERNAME(b.producer) producername,
                                b.rowno,
                                to_char(a.shrq, 'YYYY-MM-DD') DJRQ,
                                to_char(sysdate, 'YYYY-MM-DD') printdate,
                                F_GETUNITNAME(b.unit) unitname,
                                b.XSSL * b.hsjj HSJE,
                                F_GETUSERNAME(a.lry) ZDY,
                                B.PH,
                                B.YXQZ,
                                B.XSSL SL,
                                B.BZSL,
                                F_GETPARA('SUPPER') || '拣货单' || nvl(b.str1, '(高值)') DT,
                                B.HWID,
                                (select sum(hsje) from dat_db_com where seqno = a.seqno) sumhjje,
                                d.hiscode,
                                A.SUBSUM,
                                F_GETSUPNAME(T.PSSID) PSSNAME,
                                T.PSSID,
                                b.isgz,
                                b.rowno,
                                B.STR1 BARCODE
                  from dat_ck_doc a, dat_ck_com b, DAT_GOODSJXC T, doc_goods d
                 where a.seqno = b.seqno
                   and b.gdseq = d.gdseq
                   AND B.SEQNO = T.BILLNO
                   AND B.ROWNO = T.ROWNO
                   AND a.SEQNO = '{0}'
                   AND XSSL > 0
                 ORDER BY T.PSSID, B.ISGZ, b.rowno
                ";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetData_GZSMCKD()
        {
            //F_GETPARA('SUPPER')
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,
                               F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                               F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                               A.XSRQ, B.GDSEQ, B.GDNAME, B.UNIT, B.GDSPEC, B.BZHL, D.HSJJ,
                               F_GETPRODUCERNAME(D.PRODUCER) PRODUCERNAME,
                               B.ROWNO, TO_CHAR(A.XSRQ, 'YYYY-MM-DD') DJRQ,
                               TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                               F_GETUNITNAME(B.UNIT) UNITNAME,
                               D.HSJJ * 1 HSJE, F_GETUSERNAME(A.LRY) ZDY,
                               D.HSJJ V_HSJJ,
                               B.PH,  B.YXQZ, 
                               1 SL,
                               D.PIZNO PZWH,
                               F_GETSUPNAME(C.PSSID) PSSNAME,
                               F_GETPARA('SUPPER') || '随货同行单 高值'||DECODE(C.PSSID,'00001',' 代管','')  DT, 
                               A.SUBSUM,
                               B.ONECODE BARCODE ,D.HISCODE,
                               F_GETBILLCOUNT(A.SEQNO,'CKD',10) SUBNUM
                               FROM DAT_CK_DOC A, DAT_CK_EXT B, DAT_GZ_EXT C,DOC_GOODS D
                               WHERE A.SEQNO = B.BILLNO 
                               AND B.GDSEQ = D.GDSEQ
                               AND B.ONECODE = C.ONECODE
                               AND A.SEQNO = '{0}' 
                                         ORDER BY B.BILLNO,C.PSSID,B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetData_GZCK()
        {
            //F_GETPARA('SUPPER')
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,
                               F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                               F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                               A.XSRQ, B.GDSEQ, B.GDNAME, B.UNIT, B.GDSPEC, B.BZHL, D.HSJJ,
                               F_GETPRODUCERNAME(D.PRODUCER) PRODUCERNAME,
                               B.ROWNO, TO_CHAR(A.XSRQ, 'YYYY-MM-DD') DJRQ,
                               TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                               F_GETUNITNAME(B.UNIT) UNITNAME,
                               D.HSJJ * 1 HSJE, F_GETUSERNAME(A.LRY) ZDY,
                               D.HSJJ V_HSJJ,
                               B.PH,  B.YXQZ, 
                               1 SL,
                               D.PIZNO PZWH,
                               F_GETSUPNAME(C.PSSID) PSSNAME,
                               F_GETPARA('SUPPER') || '随货同行单 高值'  DT, 
                               A.SUBSUM,
                               B.ONECODE BARCODE ,D.HISCODE
                               FROM DAT_CK_DOC A, DAT_CK_EXT B, DAT_GZ_EXT C,DOC_GOODS D
                               WHERE A.SEQNO = B.BILLNO 
                               AND B.GDSEQ = D.GDSEQ
                               AND B.ONECODE = C.ONECODE
                               AND C.PSSID <> '00001'
                               AND A.SEQNO = '{0}' 
                                         ORDER BY B.BILLNO,B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetData_GZCKDG()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,
                               F_GETDEPTNAME(A.DEPTOUT) DEPTOUTNAME,
                               F_GETDEPTNAME(A.DEPTID) DEPTIDNAME,
                               A.XSRQ, B.GDSEQ, B.GDNAME, B.UNIT, B.GDSPEC, B.BZHL, D.HSJJ,
                               F_GETPRODUCERNAME(D.PRODUCER) PRODUCERNAME,
                               B.ROWNO, TO_CHAR(A.XSRQ, 'YYYY-MM-DD') DJRQ,
                               TO_CHAR(SYSDATE, 'YYYY-MM-DD') PRINTDATE,
                               F_GETUNITNAME(B.UNIT) UNITNAME,
                               D.HSJJ * 1 HSJE, F_GETUSERNAME(A.LRY) ZDY,
                               D.HSJJ V_HSJJ,
                               B.PH,  B.YXQZ, 
                               1 SL,A.SUBSUM,
                               D.PIZNO PZWH,F_GETSUPNAME(C.PSSID) PSSNAME,
                               F_GETPARA('SUPPER') || '随货同行单 高值' DT, 
                               -(select SUM(B.HSJE) SUMHJJE
                                      from dat_ck_com A, DAT_GOODSJXC B
                                     where A.seqno = '{0}'
                                       AND A.SEQNO = B.BILLNO
                                       AND B.KCADD = -1
                                       AND A.ROWNO = B.ROWNO
                                       AND B.pssid = '00001') SUMHJJE,
                               B.ONECODE BARCODE ,D.HISCODE
                               FROM DAT_CK_DOC A, DAT_CK_EXT B, DAT_GZ_EXT C,DOC_GOODS D
                               WHERE A.SEQNO = B.BILLNO 
                               AND B.GDSEQ = D.GDSEQ
                               AND B.ONECODE = C.ONECODE
                               AND C.PSSID = '00001'
                               AND A.SEQNO = '{0}' 
                                         ORDER BY B.BILLNO,B.ROWNO";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetStockGysData()
        {
            string gys = Request["gys"] == null ? "" : Request["gys"].ToString();
            string begrq = Request["b"] == null ? "" : Request["b"].ToString();
            string endrq = Request["e"] == null ? "" : Request["e"].ToString();
            string isgz = Request["isgz"] == null ? "" : Request["isgz"].ToString();
            string user = Request["u"] == null ? "" : Request["u"].ToString();
            string all = Request["a"] == null ? "" : Request["a"].ToString();
            string dhlx = Request["dh"] == null ? "" : Request["dh"].ToString();

            if (string.IsNullOrWhiteSpace(gys))
            {
                gys = "LIKE '%'";
            }
            else
            {
                gys = " = '" + gys + "'";
            }

            string qcb, qmb;
            if (begrq == DateTime.Now.ToString("yyyy-MM-dd"))
            {
                qcb = @"(SELECT A.SUPID, SUM(A.KCSL) QCSL, SUM(A.KCSL * A.KCHSJJ) QCJE
                                 FROM DAT_GOODSSTOCK A
                               GROUP BY A.SUPID) QC";
            }
            else
            {
                qcb = string.Format(@"(SELECT A.SUPID, SUM(A.KCSL) QCSL, SUM(A.KCSL * B.HSJJ) QCJE
                                                    FROM DAT_STOCKDAY A, DOC_GOODS B
                                                  WHERE A.GDSEQ = B.GDSEQ
                                                      AND TO_CHAR(A.RQ, 'YYYY-MM-DD') = '{0}'
                                                  GROUP BY A.SUPID) QC", begrq);

            }
            if (endrq == DateTime.Now.ToString("yyyy-MM-dd"))
            {
                qmb = string.Format(@"  (SELECT G.PSSID, SUM(G.KCSL) KCSL, SUM(G.KCSL * A.HSJJ) KCJE
                                                              FROM DAT_GOODSSTOCK G, DOC_GOODS A
                                                             WHERE G.GDSEQ = A.GDSEQ
                                                               AND G.PSSID {0}
                                                               AND G.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('3', '4')
                                                                                   AND ISLAST = 'Y')
                                                               AND A.ISGZ LIKE '%{1}%'
                                                             GROUP BY G.PSSID) KSKC,
                                                           (SELECT G.PSSID, SUM(G.KCSL) KCSL, SUM(G.KCSL * A.HSJJ) KCJE
                                                              FROM DAT_GOODSSTOCK G, DOC_GOODS A
                                                             WHERE G.GDSEQ = A.GDSEQ
                                                               AND G.PSSID {0}
                                                               AND G.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('1', '2')
                                                                                   AND ISLAST = 'Y')
                                                               AND A.ISGZ LIKE '%{1}%'
                                                             GROUP BY G.PSSID) KFKC", gys, isgz);
            }
            else
            {
                qmb = string.Format(@"  (SELECT G.PSSID, SUM(G.KCSL) KCSL, SUM(G.KCSL * A.HSJJ) KCJE
                                                              FROM DAT_STOCKDAY G, DOC_GOODS A
                                                             WHERE G.GDSEQ = A.GDSEQ
                                                               AND G.PSSID {0}
                                                               AND TO_CHAR(G.RQ,'YYYY-MM-DD')='{2}'
                                                               AND G.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('3', '4')
                                                                                   AND ISLAST = 'Y')
                                                               AND A.ISGZ LIKE '%{1}%'
                                                             GROUP BY G.PSSID) KSKC,
                                                           (SELECT G.PSSID, SUM(G.KCSL) KCSL, SUM(G.KCSL * A.HSJJ) KCJE
                                                              FROM DAT_STOCKDAY G, DOC_GOODS A
                                                             WHERE G.GDSEQ = A.GDSEQ
                                                               AND G.PSSID {0}
                                                               AND TO_CHAR(G.RQ,'YYYY-MM-DD')='{2}'
                                                               AND G.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('1', '2')
                                                                                   AND ISLAST = 'Y')
                                                               AND A.ISGZ LIKE '%{1}%'
                                                             GROUP BY G.PSSID) KFKC", gys, isgz, endrq);
            }

            StringBuilder sbSql = new StringBuilder();
            //昌吉州人民医院期初数据处理 By c 2016年5月3日09:53:21
            if (begrq == "2016-03-01")
            {
                sbSql.AppendFormat(@"SELECT S.SUPID,
                                                           S.SUPNAME,F_GETUSERNAME('{4}') ZDY,
                                                           F_GETPARA('SUPPER') || '医疗物资供应商进销存汇总表' DT,
                                                           NVL(QC.QCSL,0) QCSL,
                                                           NVL(QC.QCJE,0) QCJE,
                                                           NVL(KF.RKSL, 0)-NVL(QC.QCSL,0) RKSL,
                                                           NVL(KF.RKJE, 0)-NVL(QC.QCJE,0) RKJE,
                                                           NVL(ABS(KF.SHSL), 0) SHSL,
                                                           NVL(ABS(KF.SHJE), 0) SHJE,
                                                           NVL(ABS(KF.CKSL), 0) CKSL,
                                                           NVL(ABS(KF.CKJE), 0) CKJE,
                                                           NVL(ABS(SY.SYSL), 0) SYSL,
                                                           NVL(ABS(SY.SYJE), 0) SYJE,
                                                           NVL(KSKC.KCSL, 0) KSKCSL,
                                                           NVL(KSKC.KCJE, 0) KSKCJE,
                                                           NVL(KFKC.KCSL, 0) KFKCSL,
                                                           NVL(KFKC.KCJE, 0) KFKCJE
                                                      FROM (SELECT * FROM DOC_SUPPLIER WHERE ISSUPPLIER = 'Y') S,
                                                           (SELECT J.PSSID,
                                                                   SUM(DECODE(J.BILLTYPE, 'RKD', J.SL, 'THD', J.SL, 0)) RKSL,
                                                                   SUM(DECODE(J.BILLTYPE, 'RKD', J.HSJE, 'THD', J.HSJE, 0)) RKJE,
                                                                   SUM(DECODE(J.BILLTYPE, 'SYD', J.SL, 0)) SHSL,
                                                                   SUM(DECODE(J.BILLTYPE, 'SYD', J.SL * J.HSJJ, 0)) SHJE,          
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'LCD',
                                                                              J.SL,
                                                                              'DSC',
                                                                              J.SL,
                                                                              'CKD',
                                                                              J.SL,
                                                                              'LTD',
                                                                              J.SL,
                                                                              'DST',
                                                                              J.SL,
                                                                              'XST',
                                                                              J.SL,
                                                                              0)) CKSL,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'LCD',
                                                                              J.SL*J.HSJJ,
                                                                              'DSC',
                                                                              J.SL*J.HSJJ,
                                                                              'CKD',
                                                                              J.SL*J.HSJJ,
                                                                              'LTD',
                                                                              J.SL*J.HSJJ,
                                                                              'DST',
                                                                              J.SL*J.HSJJ,
                                                                              'XST',
                                                                              J.SL*J.HSJJ,
                                                                              0)) CKJE
                                                              FROM DAT_GOODSJXC J ,DOC_GOODS G
                                                             WHERE J.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('1', '2')
                                                                                   AND ISLAST = 'Y')
                                                               AND J.PSSID {2}
AND J.GDSEQ=G.GDSEQ
AND G.CATID0='{7}'
                                                               AND J.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%')
                                                               AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                             GROUP BY J.PSSID) KF,
                                                           (SELECT J.PSSID,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'XSD',
                                                                              J.SL,
                                                                              'XSG',
                                                                              J.SL,
                                                                              'DSH',
                                                                              J.SL,
                                                                              'SYD',
                                                                              J.SL,
                                                                              'XST',
                                                                              DECODE(J.KCADD,'1',J.SL,0),
                                                                              0)) SYSL,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'XSD',
                                                                              J.SL*J.HSJJ,
                                                                              'XSG',
                                                                              J.SL*J.HSJJ,
                                                                              'DSH',
                                                                              J.SL*J.HSJJ,
                                                                              'SYD',
                                                                              J.SL*J.HSJJ,
                                                                              'XST',
                                                                              DECODE(J.KCADD,'1',J.SL*J.HSJJ,0),
                                                                              0)) SYJE
                                                              FROM DAT_GOODSJXC J
                                                             WHERE J.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('3', '4')
                                                                                   AND ISLAST = 'Y')
                                                               AND J.PSSID {2}
                                                               AND J.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%')
                                                               AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                             GROUP BY J.PSSID) SY,
                                                           {5},{6}
            ", begrq, endrq, gys, isgz, user, qcb, qmb, dhlx);
            }
            else
            {
                sbSql.AppendFormat(@"SELECT S.SUPID,
                                                           S.SUPNAME,F_GETUSERNAME('{4}') ZDY,
                                                           F_GETPARA('SUPPER') || '医疗物资供应商进销存汇总表' DT,
                                                           NVL(KF.RKSL, 0) RKSL,
                                                           NVL(KF.RKJE, 0) RKJE,
                                                           NVL(ABS(KF.SHSL), 0) SHSL,
                                                           NVL(ABS(KF.SHJE), 0) SHJE,
                                                           NVL(ABS(KF.CKSL), 0) CKSL,
                                                           NVL(ABS(KF.CKJE), 0) CKJE,
                                                           NVL(ABS(SY.SYSL), 0) SYSL,
                                                           NVL(ABS(SY.SYJE), 0) SYJE,
                                                           NVL(KSKC.KCSL, 0) KSKCSL,
                                                           NVL(KSKC.KCJE, 0) KSKCJE,
                                                           NVL(KFKC.KCSL, 0) KFKCSL,
                                                           NVL(KFKC.KCJE, 0) KFKCJE
                                                      FROM (SELECT * FROM DOC_SUPPLIER WHERE ISSUPPLIER = 'Y') S,
                                                           (SELECT J.PSSID,
                                                                   SUM(DECODE(J.BILLTYPE, 'RKD', J.SL, 'THD', J.SL, 0)) RKSL,
                                                                   SUM(DECODE(J.BILLTYPE, 'RKD', J.HSJE, 'THD', J.HSJE, 0)) RKJE,
                                                                   SUM(DECODE(J.BILLTYPE, 'SYD', J.SL, 0)) SHSL,
                                                                   SUM(DECODE(J.BILLTYPE, 'SYD', J.SL * J.HSJJ, 0)) SHJE,          
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'LCD',
                                                                              J.SL,
                                                                              'DSC',
                                                                              J.SL,
                                                                              'CKD',
                                                                              J.SL,
                                                                              'LTD',
                                                                              J.SL,
                                                                              'DST',
                                                                              J.SL,
                                                                              'XST',
                                                                              J.SL,
                                                                              0)) CKSL,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'LCD',
                                                                              J.SL*J.HSJJ,
                                                                              'DSC',
                                                                              J.SL*J.HSJJ,
                                                                              'CKD',
                                                                              J.SL*J.HSJJ,
                                                                              'LTD',
                                                                              J.SL*J.HSJJ,
                                                                              'DST',
                                                                              J.SL*J.HSJJ,
                                                                              'XST',
                                                                              J.SL*J.HSJJ,
                                                                              0)) CKJE
                                                              FROM DAT_GOODSJXC J ,DOC_GOODS G
                                                             WHERE J.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('1', '2')
                                                                                   AND ISLAST = 'Y')
                                                               AND J.PSSID {2}
                                                               AND J.GDSEQ=G.GDSEQ
                                                               AND G.CATID0='{7}'
                                                               AND J.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%')
                                                               AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                             GROUP BY J.PSSID) KF,
                                                           (SELECT J.PSSID,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'XSD',
                                                                              J.SL,
                                                                              'XSG',
                                                                              J.SL,
                                                                              'DSH',
                                                                              J.SL,
                                                                              'SYD',
                                                                              J.SL,
                                                                              'XST',
                                                                              DECODE(J.KCADD,'1',J.SL,0),
                                                                              0)) SYSL,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'XSD',
                                                                              J.SL*J.HSJJ,
                                                                              'XSG',
                                                                              J.SL*J.HSJJ,
                                                                              'DSH',
                                                                              J.SL*J.HSJJ,
                                                                              'SYD',
                                                                              J.SL*J.HSJJ,
                                                                              'XST',
                                                                              DECODE(J.KCADD,'1',J.SL*J.HSJJ,0),
                                                                              0)) SYJE
                                                              FROM DAT_GOODSJXC J
                                                             WHERE J.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('3', '4')
                                                                                   AND ISLAST = 'Y')
                                                               AND J.PSSID {2}
                                                               AND J.GDSEQ IN
                                                                   (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%')
                                                               AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                             GROUP BY J.PSSID) SY,
                                                           {5},{6}
            ", begrq, endrq, gys, isgz, user, qcb, qmb, dhlx);
            }
            if (string.IsNullOrWhiteSpace(Request["gys"].ToString()))
            {
                sbSql.Append(@" WHERE S.SUPID = SY.PSSID(+)
                                              AND S.SUPID = QC.SUPID(+)
                                              AND S.SUPID = KSKC.PSSID(+)
                                              AND S.SUPID = KFKC.PSSID(+)
                                              AND S.SUPID = KF.PSSID ");
                if (all.ToLower() == "false")
                {
                    sbSql.Append(@" AND (NVL(KF.RKSL, 0)>0 OR
                                                       NVL(ABS(KF.SHSL), 0)>0 OR
                                                       NVL(ABS(KF.CKSL), 0)>0 OR
                                                       NVL(ABS(SY.SYSL), 0)>0 OR
                                                       NVL(KSKC.KCSL, 0)>0 OR
                                                       NVL(KFKC.KCSL, 0) >0) ");
                }
                sbSql.Append("  ORDER BY S.SUPNAME");
            }
            else
            {
                sbSql.AppendFormat(@" WHERE S.SUPID = SY.PSSID
                                                        AND S.SUPID = QC.SUPID(+)
                                                        AND S.SUPID = KSKC.PSSID
                                                        AND S.SUPID = KFKC.PSSID
                                                        AND S.SUPID = KF.PSSID
                                                        AND S.SUPID = '{0}'
                                                      ORDER BY S.SUPNAME", Request["gys"].ToString());
            }
            OracleReportData.GenNodeXmlData(this, sbSql.ToString(), false);
        }

        public void PrintNum()
        {
            string osid = Request["seqno"] == null ? "" : Request["seqno"].ToString();
            string user = Request["user"] == null ? "" : Request["user"].ToString();
            string oper = Request["oper"] == null ? "" : Request["oper"].ToString();

            if (!string.IsNullOrWhiteSpace(osid) && !string.IsNullOrWhiteSpace(osid) && !string.IsNullOrWhiteSpace(osid))
            {
                string[] billNos = osid.Replace("'", "").Split(',');
                StringBuilder builder = new StringBuilder();
                foreach (string bill in billNos)
                {
                    builder.Length = 0;
                    string type = bill.Substring(0, 3);
                    builder.Append(" DECLARE ");
                    builder.Append(" LN_NUM NUMBER;");
                    builder.Append(" BEGIN ");
                    builder.Append(" SELECT COUNT(*) INTO LN_NUM");
                    builder.Append(" FROM SYS_FUNCPRNNUM ");
                    builder.AppendFormat(" WHERE FUNCNO = '{0}' AND FUNCID = '{1}' AND OPER='{2}';", bill, type, oper);
                    builder.Append(" IF LN_NUM > 0 THEN ");
                    builder.AppendFormat(" UPDATE SYS_FUNCPRNNUM SET FUNCTIME = FUNCTIME+1,OPERUSER='{2}',OPERTIME = SYSDATE WHERE FUNCNO = '{0}' AND FUNCID = '{1}' AND OPER='{3}'; ", bill, type, user, oper);
                    builder.Append(" ELSE ");
                    builder.AppendFormat(" INSERT INTO SYS_FUNCPRNNUM(OPER,FUNCID,FUNCNO,FUNCTIME,OPERUSER,OPERTIME) VALUES('{0}','{1}','{2}',1,'{3}',SYSDATE); ", oper, type, bill, user);
                    builder.Append(" END IF; ");
                    builder.Append(" EXCEPTION ");
                    builder.Append(" WHEN NO_DATA_FOUND THEN ");
                    builder.Append(" NULL;");
                    builder.Append("END ;");
                    DbHelperOra.ExecuteSql(builder.ToString());
                }
            }
            else
            {
                PubFunc.OperLog("LOG", "PrintReport.EidtPrintNum", user, string.Format("页面传递过来的参数值为：user=[{0}],osid=[{1}],oper=[{2}]", user, osid, oper));
            }
        }


        public void HighGoodsOder()
        {
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT a.seqno,
F_GETUSERNAME(a.lry) lry,
b.rowno,
b.gdseq,
b.gdname,
b.gdspec,
F_GETDEPTNAME(a.deptid) deptidname,
F_GETUNITNAME(b.unit) UNITNAME,
a.XDRQ,
sysdate,
b.bzhl,
(b.dhs/b.bzhl) dhs,
b.hsjj,
a.SUBSUM,
b.hsjj*（b.DHS/b.bzhl) HSJE,
F_GETSUPNAME(b.supid) SUPNAME, 
F_GETPRODUCERNAME(b.producer) producername,
DECODE(a.DHFS,'P','普耗','G','高值') || '计划生成单' DT
from DAT_DDPLAN_DOC a,DAT_DDPLAN_COM b
where a.seqno = b.seqno
   and a.SEQNO = '{0}'";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);

        }
        public void GetGoodsNewCJ()
        {
            //商品新增
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"SELECT A.SEQNO,
                                           B.ROWNO,
                                           B.GDNAME,
                                           B.SL BZSL,
                                           B.HSJJ,
                                           B.HSJE,
                                           B.MEMOGOODS,
                                           A.LRRQ XSRQ, f_getdeptname(A.DEPTID) DEPTIDNAME,F_GETPARA('SUPPER')|| '临时物资申请单' DT
                                      FROM DAT_GOODSNEW_DOC A, DAT_GOODSNEW_COM B
                                     WHERE A.SEQNO = B.SEQNO
                                       AND A.SEQNO = '{0}' ";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GetGysHzData()
        {
            string gys = Request["gys"] == null ? "" : Request["gys"].ToString();
            string b = Request["b"] == null ? "" : Request["b"].ToString();
            string e = Request["e"] == null ? "" : Request["e"].ToString();
            string isgz = Request["isgz"] == null ? "" : Request["isgz"].ToString();
            string all = Request["a"] == null ? "" : Request["a"].ToString();
            if (string.IsNullOrWhiteSpace(gys))
            {
                gys = "  AND J.PSSID LIKE '%'";
            }
            else
            {
                gys = "  AND J.PSSID = '" + gys + "'";
            }
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT S.SUPID,
                                                           S.SUPNAME,
                                                           NVL(KF.RKSL, 0) RKSL,
                                                           NVL(KF.RKJE, 0) RKJE,
                                                           NVL(ABS(SY.SYSL), 0) SYSL,
                                                           NVL(ABS(SY.SYJE), 0) SYJE,
                                                           FP.FPS,
                                                           FP.INVOICESUM FPJE,
                                                           NVL(ABS(SY.SYJE), 0) - NVL(FP.INVOICESUM, 0) WFPJE,
                                                           '' MEMO, 
                                                           SUBSTR('{1}', 0, 4) || '年' || SUBSTR('{1}', 6, 2) || '月医疗物资供应商汇总表' DT
                                                      FROM (SELECT * FROM DOC_SUPPLIER WHERE ISSUPPLIER = 'Y') S,
                                                           (SELECT J.PSSID,
                                                                   SUM(DECODE(J.BILLTYPE, 'RKD', J.SL, 'THD', J.SL, 0)) RKSL,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'RKD',
                                                                              J.SL * J.HSJJ,
                                                                              'THD',
                                                                              J.SL * J.HSJJ,
                                                                              0)) RKJE
                                                              FROM DAT_GOODSJXC J
                                                             WHERE J.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('1', '2')
                                                                                   AND ISLAST = 'Y')
                                                               AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                               {2}
                                                               AND J.GDSEQ IN (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%')
                                                             GROUP BY J.PSSID) KF,
                                                           (SELECT J.PSSID,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'XSD',
                                                                              J.SL,
                                                                              'XSG',
                                                                              J.SL,
                                                                              'DSH',
                                                                              J.SL,
                                                                              'SYD',
                                                                              J.SL,
                                                                              'XST',
                                                                              DECODE(J.KCADD, '1', J.SL, 0),
                                                                              0)) SYSL,
                                                                   SUM(DECODE(J.BILLTYPE,
                                                                              'XSD',
                                                                              J.SL * J.HSJJ,
                                                                              'XSG',
                                                                              J.SL * J.HSJJ,
                                                                              'DSH',
                                                                              J.SL * J.HSJJ,
                                                                              'SYD',
                                                                              J.SL * J.HSJJ,
                                                                              'XST',
                                                                              DECODE(J.KCADD, '1', J.SL * J.HSJJ, 0),
                                                                              0)) SYJE
                                                              FROM DAT_GOODSJXC J
                                                             WHERE J.DEPTID IN (SELECT CODE
                                                                                  FROM SYS_DEPT
                                                                                 WHERE TYPE IN ('3', '4')
                                                                                   AND ISLAST = 'Y')
                                                               AND J.RQSJ BETWEEN TO_DATE('{0}', 'YYYY-MM-DD') AND
                                                                   TO_DATE('{1}', 'YYYY-MM-DD') + 1
                                                               {2}
                                                               AND J.GDSEQ IN (SELECT GDSEQ FROM DOC_GOODS WHERE ISGZ LIKE '%{3}%')
                                                             GROUP BY J.PSSID) SY,
                                                           (SELECT ORG SUPID,
                                                                   COUNT(1) FPS,
                                                                   SUM(INVOICESUM) INVOICESUM,
                                                                   LISTAGG(INVOICENO, ',') WITHIN GROUP(ORDER BY ORG) INVOICENO
                                                              FROM DAT_FP_DOC
                                                             WHERE TO_CHAR(BEGRQ, 'YYYY-MM-DD') = '{0}'
                                                               AND TO_CHAR(ENDRQ, 'YYYY-MM-DD') = '{1}'
                                                             GROUP BY ORG) FP
                                                     WHERE S.SUPID = SY.PSSID(+)
                                                       AND S.SUPID = KF.PSSID(+)
                                                       AND S.SUPID = FP.SUPID(+) ", b, e, gys, isgz);
            if (all.ToLower() == "false")
            {
                sbSql.Append(@" AND (NVL(KF.RKSL, 0)>0 OR
                                                   NVL(KF.RKJE, 0)>0 OR
                                                   NVL(ABS(SY.SYSL), 0)>0 OR
                                                   NVL(ABS(SY.SYJE), 0)>0) ");
            }
            sbSql.Append(@" ORDER BY S.SUPNAME");
            OracleReportData.GenNodeXmlData(this, sbSql.ToString(), false);
        }
        public void GetGysrkData()
        {
            string gys = Request["gys"] == null ? "" : Request["gys"].ToString();
            string b = Request["b"] == null ? "" : Request["b"].ToString();
            string e = Request["e"] == null ? "" : Request["e"].ToString();
            string deptout = Request["deptout"] == null ? "" : Request["deptout"].ToString();

            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"SELECT SUPID,SUPNAME, NVL(SUM(DZYHJE), 0) DZYHJE,
                                               NVL(SUM(FSCLJE), 0) FSCLJE,
                                               NVL(SUM(HYCLJE), 0) HYCLJE,
                                               NVL(SUM(KQCLJE), 0) KQCLJE,
                                               (NVL(SUM(TOTALJE), 0) - NVL(SUM(DZYHJE), 0) - NVL(SUM(FSCLJE), 0) -
                                               NVL(SUM(HYCLJE), 0) - NVL(SUM(KQCLJE), 0)) MOREJE,
                                               NVL(SUM(TOTALJE), 0) TOTALJE
                                          FROM (SELECT SUPID,SUPNAME, CASE
                                                         WHEN CATID LIKE ('212302%') THEN
                                                          HSJE
                                                       END DZYHJE,
                                                       CASE
                                                         WHEN CATID = '21230105' THEN
                                                          HSJE
                                                       END FSCLJE,
                                                       CASE
                                                         WHEN CATID = '21230109' THEN
                                                          HSJE
                                                       END HYCLJE,
                                                       CASE
                                                         WHEN CATID = '21230112' THEN
                                                          HSJE
                                                       END KQCLJE,
                                                       CASE
                                                         WHEN CATID LIKE ('2123%') THEN
                                                          HSJE
                                                       END TOTALJE
                                                  FROM (SELECT DGJ.HSJE, DGJ.SUPID,DS.SUPNAME, DGJ.CATID
                                                          FROM DAT_GOODSJXC DGJ, DOC_GOODS DG, DOC_SUPPLIER DS
                                                         WHERE DGJ.GDSEQ = DG.GDSEQ(+)
                                                           AND DGJ.SUPID = DS.SUPID(+)
                                                           AND DS.ISSUPPLIER = 'Y'
                                                           AND DGJ.SL > 0
                                                           AND DGJ.BILLTYPE = 'RKD'
                                                           AND TRUNC(DGJ.RQSJ,'dd') BETWEEN TRUNC(TO_DATE('{0}','YYYY-MM-DD HH24:MI:SS'),'dd') AND TRUNC(TO_DATE('{1}','YYYY-MM-DD HH24:MI:SS'),'dd')
                                                           AND DGJ.SUPID LIKE NVL('{2}','%')
                                                           AND DGJ.DEPTID LIKE NVL('{3}','%') ))
                                         ", b, e, gys, deptout);

            sbSql.Append(@" GROUP BY SUPID,SUPNAME ORDER BY SUPNAME");
            OracleReportData.GenNodeXmlData(this, sbSql.ToString(), false);
        }
        public void GoodsReturnNGZ()
        {
            //科室退货非高值
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select F_GETPARA('SUPPER')cusname,
                                               a.seqno,
                                               F_GETDEPTNAME(a.deptout) deptoutname,
                                               F_GETDEPTNAME(a.deptid) deptidname,
                                               to_char(a.xsrq, 'YYYY-MM-DD') XSRQNAME,
                                               b.gdseq,
                                               b.gdname,
                                               b.unit,
                                               b.gdspec,
                                               (c.hsjj) HSJJ,
                                               b.rowno,
                                               to_char(a.shrq, 'YYYY-MM-DD') DJRQ,
                                               to_char(sysdate, 'YYYY-MM-DD') printtime,
                                               F_GETUNITNAME(b.unit) unitname,
                                               -abs(c.hsje) HSJE,
                                               b.str2 onecode,
                                               F_GETUSERNAME(a.lry) ZDY,
                                               c.PH,
                                               c.YXQZ,
                                               B.XSSL,
                                               -abs(c.sl) bzsl,
                                               c.pssid,
                                               f_getsupname(c.pssid) pssname,
                                               b.pzwh,
                                               F_GETPARA('SUPPER')|| '科室退货单'||DECODE(C.PSSID,'00001','代管','') DT,
                                               B.HWID,
                                               - (select sum(hsje) sumhsje
                                                   from dat_ck_com
                                                  where seqno = '{0}' and isgz = 'N' 
                                                  group by seqno) sumhjje
                                          from dat_ck_doc a,
                                               dat_ck_com b,
                                               (select billno,rowno,ph,yxqz,pssid,hsjj,sum(sl) sl,sum(hsje) hsje
                                                  from dat_goodsjxc where billno='{0}' and kcadd=1 
                                                       group by billno,rowno,ph,yxqz,pssid,hsjj ) c
                                         where a.seqno = b.seqno and b.rowno=c.rowno and b.isgz='N'
                                           and a.SEQNO = '{0}' order by b.seqno,c.pssid,b.rowno";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
        public void GoodsReturnYGZ()
        {
            //科室退货高值
            string osid = Request["osid"] == null ? "" : Request["osid"].ToString();
            string sql = @"select F_GETPARA('SUPPER')cusname,
                                                       a.seqno,
                                                       F_GETDEPTNAME(a.deptout) deptoutname,
                                                       F_GETDEPTNAME(a.deptid) deptidname,
                                                       to_char(a.xsrq, 'YYYY-MM-DD') XSRQNAME,
                                                       b.gdseq,
                                                       b.gdname,
                                                       b.unit,
                                                       b.gdspec,
                                                       b.bzhl,
                                                       (c.hsjj) HSJJ,
                                                       b.rowno,
                                                       to_char(a.shrq, 'YYYY-MM-DD') DJRQ,
                                                       to_char(sysdate, 'YYYY-MM-DD') printtime,
                                                       F_GETUNITNAME(b.unit) unitname,
                                                       -abs(c.hsje) HSJE,
                                                       b.str2 onecode,
                                                       F_GETUSERNAME(a.lry) ZDY,
                                                       c.PH,
                                                       c.YXQZ,
                                                       C.PSSID,
                                                       F_GETSUPNAME(C.PSSID) PSSNAME,
                                                       B.PZWH,
                                                       -B.BZSL BZSL,
                                                       -abs(c.sl) bzsl,
                                                       F_GETPARA('SUPPER')|| '科室退货单(高值)'||DECODE(C.PSSID,'00001','代管','') DT,
                                                       B.HWID,
                                                       - (select sum(hsje) sumhsje
                                                           from dat_ck_com
                                                          where seqno = '{0}' and isgz = 'Y' 
                                                          group by seqno) sumhjje
                                                  from dat_ck_doc a,
                                                       dat_ck_com b,
                                                       (select billno,rowno,ph,yxqz,pssid,hsjj,sum(sl) sl,sum(hsje) hsje
                                                          from dat_goodsjxc
                                                         where billno = '{0}'
                                                           and kcadd = 1
                                                         group by billno, rowno, ph, yxqz, pssid, hsjj) c
                                                 where a.seqno = b.seqno
                                                   and b.rowno = c.rowno
                                                   and b.isgz = 'Y'
                                                   and a.SEQNO = '{0}' order by b.seqno,c.pssid,b.rowno";
            OracleReportData.GenNodeXmlData(this, string.Format(sql, osid), false);
        }
    }
}
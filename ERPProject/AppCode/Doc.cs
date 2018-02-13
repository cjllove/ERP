using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Data;
using System.Collections;
using XTBase;
using System.Net;
using FineUIPro;
using Oracle.ManagedDataAccess.Client;
using System.Text.RegularExpressions;
using XTBase.Utilities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ERPProject
{
    public class Doc
    {
        public static DataTable GetGoods(string strId, string supId, string deptId)
        {
            string sql = @"SELECT  SP.*,F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME,SP.BZHL,
                                   F_GETUNITNAME(DECODE(UNIT_ORDER,'D',UNIT_DABZ,'Z',UNIT_ZHONGBZ,UNIT)) UNIT_ORDER_NAME,F_GETUNITNAME(DECODE(UNIT_SELL,'D',UNIT_DABZ,'Z',UNIT_ZHONGBZ,UNIT)) UNIT_SELL_NAME,F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,
                                   F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,PZ.HJCODE1 HWID,'' PH,SP.PIZNO PZWH,'' RQ_SC,'' YXQZ,PZ.NUM1 DS_NUM
                             FROM  DOC_GOODS SP,DOC_GOODSCFG PZ WHERE ISDELETE='N' AND (SP.GDSEQ = '{0}' OR SP.BARCODE = '{0}' AND UPPER(SP.ZJM) LIKE '%{0}%') 
                              AND  SP.GDSEQ=PZ.GDSEQ AND SP.FLAG='Y' AND SP.SUPPLIER LIKE '%{1}%' AND PZ.DEPTID='{2}' ";
            return DbHelperOra.Query(string.Format(sql, strId.ToUpper(), supId, deptId)).Tables[0];
        }
        //ymh 提取是非否库存品信息（N为库存品），区分是否显示HIS信息
        public static DataTable GetGoods_KCP(string strId, string supId, string deptId)
        {
            string sql = "";
            if (DbHelperOra.Exists("SELECT 1 FROM SYS_PARA WHERE CODE = 'ShowName' AND VALUE = 'HIS'"))
            {
                sql = @"select  SP.GDSEQ,SP.GDID,SP.BARCODE,SP.ZJM,SP.YCODE,SP.NAMEJC,SP.NAMEEN,SP.GDMODE,SP.STRUCT,SP.BZHL,SP.UNIT,SP.FLAG,SP.CATID,SP.JX,SP.YX,SP.PIZNO,SP.BAR1,SP.BAR2,SP.BAR3,SP.DEPTID,SP.SUPPLIER,SP.LOGINLABEL,SP.PRODUCER,SP.ZPBH,SP.PPID,SP.CDID,SP.JXTAX,SP.XXTAX,SP.BHSJJ,SP.HSJJ,SP.LSJ,SP.YBJ,SP.HSID,SP.HSJ,SP.JHZQ,SP.ZDKC,
                                    SP.HLKC,SP.ZGKC,SP.SPZT,SP.DAYXS,SP.MANAGER,SP.INPER,SP.INRQ,SP.BEGRQ,SP.ENDRQ,SP.UPTRQ,SP.UPTUSER,SP.MEMO,DISABLEORG,SP.ISLOT,SP.ISJB,SP.ISFZ,SP.ISGZ,SP.ISIN,SP.ISJG,SP.ISDM,SP.ISCF,SP.ISYNZJ,SP.ISFLAG1,nvl(SP.STR3,SP.GDSPEC) GDSPEC,SP.UNIT_DABZ,SP.UNIT_ZHONGBZ,SP.BARCODE_DABZ,SP.NUM_DABZ,SP.NUM_ZHONGBZ,SP.UNIT_ORDER,SP.UNIT_SELL,SP.HISCODE,nvl(SP.HISNAME,SP.GDNAME) GDNAME,SP.CATID0,SP.STR1,
                                   F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME,SP.ISFLAG3,
                                   F_GETUNITNAME(DECODE(UNIT_ORDER,'D',UNIT_DABZ,'Z',UNIT_ZHONGBZ,UNIT)) UNIT_ORDER_NAME,F_GETUNITNAME(DECODE(UNIT_SELL,'D',UNIT_DABZ,'Z',UNIT_ZHONGBZ,UNIT)) UNIT_SELL_NAME,F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,
                                   F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,PZ.HJCODE1 HWID,'' PH,SP.PIZNO PZWH,'' RQ_SC,'' YXQZ,PZ.NUM1 DS_NUM,PZ.DSNUM
                             from  DOC_GOODS SP,DOC_GOODSCFG PZ WHERE ISDELETE='N' AND (sp.gdseq = '{0}' or sp.barcode = '{0}' and upper(sp.ZJM) like '%{0}%') 
                              and  sp.gdseq=pz.gdseq and sp.flag='Y' and sp.supplier like '%{1}%' and pz.deptid='{2}' and sp.isflag3 = 'N' ";
            }
            else
            {
                sql = @"select  SP.*,F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME,SP.BZHL,
                                   F_GETUNITNAME(DECODE(UNIT_ORDER,'D',UNIT_DABZ,'Z',UNIT_ZHONGBZ,UNIT)) UNIT_ORDER_NAME,F_GETUNITNAME(DECODE(UNIT_SELL,'D',UNIT_DABZ,'Z',UNIT_ZHONGBZ,UNIT)) UNIT_SELL_NAME,F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,
                                   F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,PZ.HJCODE1 HWID,'' PH,SP.PIZNO PZWH,'' RQ_SC,'' YXQZ,PZ.NUM1 DS_NUM
                             from  DOC_GOODS SP,DOC_GOODSCFG PZ WHERE ISDELETE='N' AND (sp.gdseq = '{0}' or sp.barcode = '{0}' and upper(sp.ZJM) like '%{0}%') 
                              and  sp.gdseq=pz.gdseq and sp.flag='Y' and sp.supplier like '%{1}%' and pz.deptid='{2}' and sp.isflag3 = 'N' ";
            }
            return DbHelperOra.Query(string.Format(sql, strId.ToUpper(), supId, deptId)).Tables[0];
        }
        public static DataTable GetGoods_His(string strId, string supId, string deptId)
        {
            string sql = @"select  SP.GDSEQ,SP.GDID,SP.BARCODE,SP.ZJM,SP.YCODE,SP.NAMEJC,SP.NAMEEN,SP.GDMODE,SP.STRUCT,SP.BZHL,SP.UNIT,SP.FLAG,SP.CATID,SP.JX,SP.YX,SP.PIZNO,SP.BAR1,SP.BAR2,SP.BAR3,SP.DEPTID,SP.SUPPLIER,SP.LOGINLABEL,SP.PRODUCER,SP.ZPBH,SP.PPID,SP.CDID,SP.JXTAX,SP.XXTAX,SP.BHSJJ,SP.HSJJ,SP.LSJ,SP.YBJ,SP.HSID,SP.HSJ,SP.JHZQ,SP.ZDKC,
                                    SP.HLKC,SP.ZGKC,SP.SPZT,SP.DAYXS,SP.MANAGER,SP.INPER,SP.INRQ,SP.BEGRQ,SP.ENDRQ,SP.UPTRQ,SP.UPTUSER,SP.MEMO,DISABLEORG,SP.ISLOT,SP.ISJB,SP.ISFZ,SP.ISGZ,SP.ISIN,SP.ISJG,SP.ISDM,SP.ISCF,SP.ISYNZJ,SP.ISFLAG1,nvl(SP.STR3,SP.GDSPEC) GDSPEC,SP.UNIT_DABZ,SP.UNIT_ZHONGBZ,SP.BARCODE_DABZ,SP.NUM_DABZ,SP.NUM_ZHONGBZ,SP.UNIT_ORDER,SP.UNIT_SELL,SP.HISCODE,nvl(SP.HISNAME,SP.GDNAME) GDNAME,SP.CATID0,SP.STR1,
                                   F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME,SP.ISFLAG3,
                                   F_GETUNITNAME(DECODE(UNIT_ORDER,'D',UNIT_DABZ,'Z',UNIT_ZHONGBZ,UNIT)) UNIT_ORDER_NAME,F_GETUNITNAME(DECODE(UNIT_SELL,'D',UNIT_DABZ,'Z',UNIT_ZHONGBZ,UNIT)) UNIT_SELL_NAME,F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,
                                   F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,NVL(PZ.HJCODE1,PZ.DEPTID) HWID,'' PH,SP.PIZNO PZWH,'' RQ_SC,'' YXQZ,PZ.NUM1 DS_NUM,PZ.DSNUM
                             from  DOC_GOODS SP,DOC_GOODSCFG PZ WHERE ISDELETE='N' AND (sp.gdseq = '{0}' or sp.barcode = '{0}' and upper(sp.ZJM) like '%{0}%') 
                              and  sp.gdseq=pz.gdseq and sp.supplier like '%{1}%' and pz.deptid='{2}' ";

            return DbHelperOra.Query(string.Format(sql, strId.ToUpper(), supId, deptId)).Tables[0];
        }
        //得到所有商品信息，区分是否需要使用HIS名称显示
        public static DataTable GetGoods_Gather(string strId, string supId, string deptId)
        {
            string sql = "";
            if (DbHelperOra.Exists("SELECT 1 FROM SYS_PARA WHERE CODE = 'ShowName' AND VALUE = 'HIS'"))
            {
                sql = @"select  SP.GDSEQ,SP.GDID,SP.BARCODE,SP.ZJM,SP.YCODE,SP.NAMEJC,SP.NAMEEN,SP.GDMODE,SP.STRUCT,SP.BZHL,SP.UNIT,SP.FLAG,SP.CATID,SP.JX,SP.YX,SP.PIZNO,SP.BAR1,SP.BAR2,SP.BAR3,SP.DEPTID,SP.SUPPLIER,SP.LOGINLABEL,SP.PRODUCER,SP.ZPBH,SP.PPID,SP.CDID,SP.JXTAX,SP.XXTAX,SP.BHSJJ,SP.HSJJ,SP.LSJ,SP.YBJ,SP.HSID,SP.HSJ,SP.JHZQ,SP.ZDKC,
                                    SP.HLKC,SP.ZGKC,SP.SPZT,SP.DAYXS,SP.MANAGER,SP.INPER,SP.INRQ,SP.BEGRQ,SP.ENDRQ,SP.UPTRQ,SP.UPTUSER,SP.MEMO,DISABLEORG,SP.ISLOT,SP.ISJB,SP.ISFZ,SP.ISGZ,SP.ISIN,SP.ISJG,SP.ISDM,SP.ISCF,SP.ISYNZJ,SP.ISFLAG1,nvl(SP.STR3,SP.GDSPEC) GDSPEC,SP.UNIT_DABZ,SP.UNIT_ZHONGBZ,SP.BARCODE_DABZ,SP.NUM_DABZ,SP.NUM_ZHONGBZ,SP.UNIT_ORDER,SP.UNIT_SELL,SP.HISCODE,nvl(SP.HISNAME,SP.GDNAME) GDNAME,SP.CATID0,SP.STR1,
                                   F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME,SP.ISFLAG3,
                                   F_GETUNITNAME(DECODE(UNIT_ORDER,'D',UNIT_DABZ,'Z',UNIT_ZHONGBZ,UNIT)) UNIT_ORDER_NAME,F_GETUNITNAME(DECODE(UNIT_SELL,'D',UNIT_DABZ,'Z',UNIT_ZHONGBZ,UNIT)) UNIT_SELL_NAME,F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,
                                   F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,PZ.HJCODE1 HWID,'' PH,SP.PIZNO PZWH,'' RQ_SC,'' YXQZ,PZ.NUM1 DS_NUM,PZ.DSNUM
                             from  DOC_GOODS SP,DOC_GOODSCFG PZ WHERE ISDELETE='N' AND (sp.gdseq = '{0}' or sp.barcode = '{0}' and upper(sp.ZJM) like '%{0}%') 
                              and  sp.gdseq=pz.gdseq and sp.flag='Y' and sp.supplier like '%{1}%' and pz.deptid='{2}' ";
            }
            else
            {
                sql = @"select  SP.*,F_GETUNITNAME(UNIT) UNITNAME,F_GETPRODUCERNAME(SP.PRODUCER) PRODUCERNAME,F_GETSUPNAME(SUPPLIER) SUPPLIERNAME,SP.BZHL,
                                   F_GETUNITNAME(DECODE(UNIT_ORDER,'D',UNIT_DABZ,'Z',UNIT_ZHONGBZ,UNIT)) UNIT_ORDER_NAME,F_GETUNITNAME(DECODE(UNIT_SELL,'D',UNIT_DABZ,'Z',UNIT_ZHONGBZ,UNIT)) UNIT_SELL_NAME,F_GETUNITNAME(UNIT_DABZ) UNIT_DABZ_NAME,
                                   F_GETUNITNAME(UNIT_ZHONGBZ) UNIT_ZHONGBZ_NAME,PZ.HJCODE1 HWID,'' PH,SP.PIZNO PZWH,'' RQ_SC,'' YXQZ,PZ.NUM1 DS_NUM
                             from  DOC_GOODS SP,DOC_GOODSCFG PZ WHERE ISDELETE='N' AND (sp.gdseq = '{0}' or sp.barcode = '{0}' and upper(sp.ZJM) like '%{0}%') 
                              and  sp.gdseq=pz.gdseq and sp.flag='Y' and sp.supplier like '%{1}%' and pz.deptid='{2}' ";
            }
            return DbHelperOra.Query(string.Format(sql, strId.ToUpper(), supId, deptId)).Tables[0];
        }
        public static DataRow GetGoodsCfg(string strId, string strDept)
        {
            DataTable dtGoods = DbHelperOra.Query(string.Format("select A.* from doc_goodscfg A where GDSEQ='{0}' and  DEPTID='{1}'", strId, strDept)).Tables[0];
            if (dtGoods.Rows.Count <= 0) return null;
            return dtGoods.Rows[0];
        }

        public static DataTable GetGoodsPHList(string strId)
        {
            //得到批号列表
            return DbHelperOra.Query(string.Format(@"select * from(select A.*, B.PIZNO,
                                                       '' SL,
                                                       (select NVL(SUM(KCSL), 0)
                                                          from DAT_GOODSSTOCK
                                                         where GDSEQ = '{0}'
                                                         and phid = a.phid
                                                           AND SUPID IN (SELECT SUPID FROM DOC_SUPPLIER WHERE ISDG = 'N')
                                                           ) KCSL
                                                  from DOC_GOODSPH A, DOC_GOODS B
                                                 where A.GDSEQ = '{0}' and A.GDSEQ = B.GDSEQ)
                                                 where kcsl>0.01", strId)).Tables[0];
        }
        public static DataTable GetGoodsPHList_DG(string strId)
        {
            //得到批号列表
            return DbHelperOra.Query(string.Format(@"select * from(select A.*,
                                                       '' SL,
                                                       (select NVL(SUM(KCSL), 0)
                                                          from DAT_GOODSSTOCK
                                                         where GDSEQ = '{0}'
                                                         and phid = a.phid
                                                           AND SUPID IN (SELECT SUPID FROM DOC_SUPPLIER WHERE ISDG = 'Y')
                                                           ) KCSL
                                                  from DOC_GOODSPH A
                                                 where GDSEQ = '{0}')
                                                 where kcsl>0.01", strId)).Tables[0];
        }
        public static DataTable GetGoodsPH_New(string gdseq, string deptid, bool supidflag = false)
        {
            //得到批号列表
            string Sql = @"SELECT A.HWID, A.GDSEQ,B.GDNAME,B.UNIT,A.PH,A.YXQZ, SUM(A.KCSL-A.LOCKSL) KCSL,ROUND(SUM(A.KCSL - A.LOCKSL)/F_GETBZHL(A.GDSEQ),2) CKKCSL,B.PIZNO,A.RQ_SC,'' SL,--DECODE(GS.TYPE,'0','托管','1','代管','Z','直供','') TYPE,
                            DECODE(B.ISFLAG3,'Y','直送','N','非直送','')TYPE,B.HSJJ,B.PIZNO PZWH
                FROM DAT_GOODSSTOCK A,DOC_GOODS B,DOC_GOODSSUP GS
                WHERE A.GDSEQ = B.GDSEQ AND B.GDSEQ=GS.GDSEQ(+) AND GS.FLAG='Y' AND A.DEPTID = '{0}' AND A.GDSEQ='{1}' AND A.KCSL > A.LOCKSL AND GS.ORDERSORT = 'Y'";
            if (supidflag)
            {
                Sql += " AND A.SUPID='00002' ";
            }
            Sql += @" GROUP BY A.HWID, A.GDSEQ, B.GDNAME, B.UNIT, A.PH, A.YXQZ, B.PIZNO,A.RQ_SC, GS.TYPE,B.HSJJ,B.ISFLAG3
                        ORDER BY A.YXQZ";
            return DbHelperOra.Query(string.Format(Sql, deptid, gdseq)).Tables[0];
        }
        /// <summary>
        /// 部门批号库存
        /// </summary>
        /// <param name="strId"></param>
        /// <returns></returns>
        public static DataTable GetGoodsPHKC(string strId, string deptId, string supId = "")
        {
            //string sql = @"SELECT PH.PHID,
            //               PH.GDSEQ,
            //               PH.PH,
            //               G.pizno PZWH,
            //               DG.RQ_SC,
            //               DG.YXQZ,'' SL,G.HISNAME GDNAME,G.PIZNO,
            //                     (SELECT NVL(SUM(KC.KCSL), 0)
            //                        FROM DAT_GOODSSTOCK KC
            //                       WHERE KC.GDSEQ = PH.GDSEQ
            //                         AND KC.DEPTID = DG.DEPTID
            //                         AND KC.PHID = PH.PHID
            //                         AND kc.SUPID IN (SELECT SUPID FROM DOC_SUPPLIER WHERE ISDG = 'N' AND SUPID='00002')) KCSL,G.ISFLAG3,G.ISGZ,'非代管' TYPE,dg.hwid
            //                FROM DOC_GOODSPH PH, DAT_GOODSSTOCK DG,DOC_GOODS G
            //                WHERE PH.PHID = DG.PHID AND PH.GDSEQ=G.GDSEQ AND PH.GDSEQ = DG.GDSEQ
            //                  AND PH.GDSEQ = '{0}' AND DG.DEPTID = '{1}' 
            //                  AND DG.KCSL>0
            //                  AND DG.SUPID IN (SELECT SUPID FROM DOC_SUPPLIER WHERE ISDG = 'N'";
            string sql = @"SELECT DG.PHID,
                           DG.GDSEQ,
                           DG.PH,
                           G.pizno PZWH,
                           DG.RQ_SC,
                           DG.YXQZ,'' SL,GDNAME,G.PIZNO,
                                 (SELECT NVL(SUM(KC.KCSL -KC.LOCKSL), 0)
                                    FROM DAT_GOODSSTOCK KC
                                   WHERE KC.GDSEQ = DG.GDSEQ
                                     AND KC.DEPTID = DG.DEPTID
                                     AND NVL(KC.PHID,'#') = NVL(DG.PHID,'#')) KCSL,G.ISFLAG3,G.ISGZ,'非代管' TYPE,dg.hwid
                            FROM DAT_GOODSSTOCK DG,DOC_GOODS G
                            WHERE DG.GDSEQ=G.GDSEQ AND G.GDSEQ = DG.GDSEQ
                              AND DG.GDSEQ = '{0}' AND DG.DEPTID = '{1}' 
                              AND DG.KCSL>0
                              AND NVL(DG.PSSID,DG.SUPID) IN (SELECT NVL(PSSID,SUPID) FROM DOC_GOODSSUP WHERE TYPE = '0')";
            if (!string.IsNullOrWhiteSpace(supId))
            {
                sql += " AND NVL(DG.PSSID,DG.SUPID)='{2}'  ORDER BY DG.YXQZ";
                return DbHelperOra.Query(string.Format("select * from ({0}) where KCSL> 0 ", string.Format(sql, strId, deptId, supId))).Tables[0];
            }
            else
            {
                sql += "  ORDER BY DG.YXQZ";
                return DbHelperOra.Query(string.Format("select * from ({0}) where KCSL> 0 ", string.Format(sql, strId, deptId))).Tables[0];
            }
        }

        public static DataTable GetGoodsPHKC_DG(string strId, string deptId, string supId = "")
        {
            //string sql = @"SELECT PH.PHID,
            //               PH.GDSEQ,
            //               PH.PH,
            //                G.pizno PZWH,
            //               DG.RQ_SC,
            //               DG.YXQZ,'' SL,nvl(G.HISNAME,G.GDNAME) GDNAME,
            //                     (SELECT NVL(SUM(KC.KCSL), 0)
            //                        FROM DAT_GOODSSTOCK KC
            //                       WHERE KC.GDSEQ = PH.GDSEQ
            //                         AND KC.DEPTID = DG.DEPTID
            //                         AND KC.PHID = PH.PHID
            //                         AND KC.SUPID = DG.SUPID
            //                         AND kc.SUPID IN (SELECT SUPID FROM DOC_SUPPLIER WHERE ISDG = 'Y' AND SUPID='{2}')) KCSL,'代管' TYPE,dg.hwid
            //                FROM DOC_GOODSPH PH, DAT_GOODSSTOCK DG,DOC_GOODS G
            //                WHERE PH.PHID = DG.PHID AND PH.GDSEQ=G.GDSEQ AND PH.GDSEQ = DG.GDSEQ
            //                  AND PH.GDSEQ = '{0}' AND DG.DEPTID = '{1}'
            //                  AND DG.SUPID IN (SELECT SUPID FROM DOC_SUPPLIER WHERE ISDG = 'Y'";
            string sql = @"SELECT DG.PHID,
                           DG.GDSEQ,
                           DG.PH,
                           G.pizno PZWH,
                           DG.RQ_SC,
                           DG.YXQZ,'' SL,GDNAME,G.PIZNO,
                                 (SELECT NVL(SUM(KC.KCSL - KC.LOCKSL), 0)
                                    FROM DAT_GOODSSTOCK KC
                                   WHERE KC.GDSEQ = DG.GDSEQ
                                     AND KC.DEPTID = DG.DEPTID
                                     AND NVL(KC.PHID,'#') = NVL(DG.PHID,'#')
                                     AND kc.SUPID IN (SELECT SUPID FROM DOC_SUPPLIER WHERE ISDG = 'Y' AND SUPID='{2}')) KCSL,'代管' TYPE,dg.hwid
                            FROM DAT_GOODSSTOCK DG,DOC_GOODS G
                            WHERE DG.GDSEQ=G.GDSEQ AND G.GDSEQ = DG.GDSEQ
                              AND DG.GDSEQ = '{0}' AND DG.DEPTID = '{1}' 
                              AND DG.KCSL>0
                              AND NVL(DG.PSSID,DG.SUPID) IN (SELECT NVL(PSSID,SUPID) FROM DOC_GOODSSUP WHERE TYPE IN ('1','Z'))";
            if (!string.IsNullOrWhiteSpace(supId))
            {
                sql += " AND NVL(DG.PSSID,DG.SUPID)='{2}'";
            }

            sql += "  ORDER BY DG.YXQZ";
            return DbHelperOra.Query(string.Format("select * from ({0}) where KCSL> 0 ", string.Format(sql, strId, deptId, supId))).Tables[0];
        }

        public static DataRow GetGoodsPH(string strPH)
        {//得到某个具体的批号
            DataTable dtGoods = DbHelperOra.Query(string.Format("select * from DOC_GOODSPH where PH='{0}'", strPH)).Tables[0];
            if (dtGoods.Rows.Count <= 0) return null;
            return dtGoods.Rows[0];
        }

        public static DataTable DdlDataGet(string strDDL, string paras, params FineUIPro.DropDownList[] ddls)
        {
            DataTable dtDDLdata = new DataTable();
            string strddlSql = (DbHelperOra.GetSingle(string.Format("SELECT  SELECTSQL FROM SYS_REPORT WHERE SEQNO ='{0}'", strDDL)) ?? "").ToString();
            if (string.IsNullOrEmpty(strddlSql)) { return null; }
            string[] args = paras.TrimEnd(',').Split(',');
            dtDDLdata = DbHelperOra.Query(string.Format(strddlSql, args)).Tables[0];

            if (dtDDLdata == null) return null;
            foreach (FineUIPro.DropDownList ddl in ddls)
            {
                ddl.DataTextField = dtDDLdata.Columns[1].ToString();
                ddl.DataValueField = dtDDLdata.Columns[0].ToString();
                ddl.DataSource = dtDDLdata;
                ddl.DataBind();
            }
            return dtDDLdata;
        }

        public static string DbGetSysPara(string para)
        {
            return (DbHelperOra.GetSingle(string.Format("SELECT VALUE FROM SYS_PARA WHERE CODE='{0}'", para)) ?? "").ToString();
        }
        public static string DbGetGrf(string para)
        {
            object obj = DbHelperOra.GetSingle(string.Format("SELECT NVL(XMGRF,DEFGRF) FROM SYS_PRINTDEF WHERE PRINTCODE='{0}'", para));
            if (obj == null)
            {
                return "";
            }
            return obj.ToString();
        }
        public static string GetIp()
        {
            string ip = "";
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            {
                //ip = Context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
            }
            if (null == ip || ip == String.Empty)
            {
                //ip = Context.Request.ServerVariables["REMOTE_ADDR"].ToString();
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            if (null == ip || ip == String.Empty)
            {
                ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            }
            if (null == ip || ip == String.Empty)
            {
                ip = "0.0.0.0";
            }
            return ip;
        }
        /// <summary>
        /// FineUIPro.Grid数据导出
        /// </summary>
        /// <param name="sql">获取数据的SQL语句</param>
        /// <param name="grid"></param>
        /// <param name="columnIndex">从第几列开始导出</param>
        /// <param name="fileName">导出的文件名</param>
        /// <param name="headerText">表头文本</param>
        public static void GridExport(string sql, FineUIPro.Grid grid, int columnIndex, string fileName, string headerText)
        {
            DataTable dtData = DbHelperOra.Query(sql).Tables[0];
            if (dtData == null || dtData.Rows.Count == 0)
            {
                Alert.Show("暂时没有查询到符合条件的数据,无法导出！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (columnIndex < 1) columnIndex = 1;

            //ArrayList arrColumName = new ArrayList();
            string[] columnNames = new string[grid.Columns.Count - 1];
            for (int index = columnIndex - 1; index < grid.Columns.Count; index++)
            {
                GridColumn column = grid.Columns[index];
                if (column is FineUIPro.BoundField)
                {
                    //dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].DataType = Type.GetType("System.String");
                    dtData.Columns[((FineUIPro.BoundField)(column)).DataField.ToUpper()].ColumnName = column.HeaderText;
                    columnNames[index - 1] = column.HeaderText;
                    //arrColumName.Add(column.HeaderText);
                }
            }
            ExcelHelper.ExportByWeb(dtData.DefaultView.ToTable(true, columnNames), headerText, string.Format("{0}_{1}.xls", fileName, DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        public static CommandInfo GETDOADD(string doType, string dept, string user, string para)
        {
            //增加待办事宜
            OracleParameter[] parameters_db = {
                                               new OracleParameter("V_DOTYPE", OracleDbType.Varchar2),
                                               new OracleParameter("V_DEPT", OracleDbType.Varchar2),
                                               new OracleParameter("V_USERID", OracleDbType.Varchar2),
                                               new OracleParameter("V_PARA", OracleDbType.Varchar2) };
            parameters_db[0].Value = doType;
            parameters_db[1].Value = dept;
            parameters_db[2].Value = user;
            parameters_db[3].Value = para;
            return new CommandInfo("MAINT.P_DO_ADD", parameters_db, CommandType.StoredProcedure);

        }
        public static string ONECODE(string onecode, string type, string table = "DAT_GOODSDS_LOG")
        {
            string results = "";
            object res = DbHelperOra.GetSingle(string.Format("SELECT {0} FROM {1} WHERE BARCODE = '{2}'", type, table, onecode));
            results = (res ?? "").ToString();
            return results;
        }

        public static bool getFlag(string strDjbh, string strFlag, string BillType)
        {
            if (strDjbh.Length == 0)
            {
                return true;
            }
            string getState = DbHelperOra.GetSingle("select F_GETBILLTYPE('" + BillType + "','" + strDjbh + "') FROM dual").ToString();
            if (getState == strFlag && !getState.Contains("错误"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 过滤SQL语句,防止注入
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static string filterSql(string strSql)
        {
            strSql = strSql.ToLower().Trim();
            strSql = strSql.Replace("exec", "");
            strSql = strSql.Replace("delete", "");
            strSql = strSql.Replace("master", "");
            strSql = strSql.Replace("truncate", "");
            strSql = strSql.Replace("declare", "");
            strSql = strSql.Replace("create", "");
            strSql = strSql.Replace("xp_", "no");
            strSql = strSql.Replace("insert", "");
            strSql = strSql.Replace("update", "");
            strSql = strSql.Replace("mid", "");
            strSql = strSql.Replace("count", "");
            strSql = strSql.Replace("chr", "");
            strSql = strSql.Replace("select", "");
            strSql = strSql.Replace(";", string.Empty);
            strSql = strSql.Replace("'", string.Empty);
            strSql = strSql.Replace("&", string.Empty);
            strSql = strSql.Replace("%", string.Empty);
            strSql = strSql.Replace("-", string.Empty);
            strSql = strSql.Replace(" <", string.Empty);
            strSql = strSql.Replace(">", string.Empty);
            strSql = strSql.Replace("%", string.Empty);
            strSql = strSql.Replace(".", string.Empty);
            strSql = strSql.Replace(",", string.Empty);
            strSql = strSql.Replace("|", string.Empty);
            strSql = strSql.Replace("\"\"", string.Empty);
            strSql = strSql.Replace("&", string.Empty);

            return strSql;
        }
        /// <summary>
        /// 得到高值条码对应信息
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static string ONECODE_GZ(string onecode, string type)
        {
            string results = "";
            object res = DbHelperOra.GetSingle(string.Format("SELECT {0} FROM DAT_GZ_EXT WHERE ONECODE = '{1}' OR STR1 = '{1}'", type, onecode));
            results = (res ?? "").ToString();
            return results;
        }
        /// <summary>
        /// 得到条码对应位数
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static int LENCODE()
        {
            string results = "";
            object res = DbHelperOra.GetSingle("SELECT VALUE FROM SYS_PARA WHERE CODE = 'CODELENGTH'");
            results = (res ?? "0").ToString();
            return Convert.ToInt16(results);
        }
        /// <summary>
        /// 得到非定数条码对应信息
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static string GetInfoByCode(string code, string type, string table)
        {
            string results = "";
            object res = DbHelperOra.GetSingle(string.Format("SELECT {0} FROM {1} WHERE ONECODE = '{2}'", type, table, code));
            results = (res ?? "").ToString();
            return results;
        }
        /// <summary>
        /// 得到Grid 相应单元格数据
        /// </summary>
        /// <param name="grid">指定Grid</param>
        /// <param name="rowId">行ID</param>
        /// <param name="colID">列ID</param>
        /// <returns></returns>
        public static String GetGridInf(FineUIPro.Grid grid, string rowId, string colID)
        {
            JToken jtdata = grid.GetMergedData().Where(a => a["id"].ToString() == rowId).ToList()[0];
            String res = (jtdata.SelectToken(string.Format("$..values.{0}", colID)) ?? "").ToString();
            return res;
        }
        /// <summary>
        /// 得到Grid 更新列数据
        /// </summary>
        /// <param name="grid">指定Grid</param>
        /// <param name="rowId">行ID</param>
        /// <returns></returns>
        public static JObject GetJObject(FineUIPro.Grid grid, string rowId)
        {
            JObject defaultObj = new JObject();
            try
            {
                foreach (JToken key in grid.GetMergedData().Where(a => a["id"].ToString() == rowId).ToList()[0].SelectToken("$.values").ToList())
                {
                    defaultObj.Add(((JProperty)(key)).Name, ((JProperty)(key)).Value);
                }
            }
            catch { }
            return defaultObj;
        }
        /// <summary>
        /// 批量加载Grid数据
        /// </summary>
        /// <returns></returns>
        public static void GridRowAdd(FineUIPro.Grid grid, DataTable table)
        {
            JArray defaultObj = new JArray();
            defaultObj = JsonConvert.DeserializeObject<JArray>(JsonConvert.SerializeObject(table));
            PageContext.RegisterStartupScript("F('" + grid.ClientID + "').addNewRecords(" + defaultObj + ",'true');");
        }

        #region 汉字转化为拼音首字母
        /// <summary> 
        /// 汉字转化为拼音首字母
        /// </summary> 
        /// <param name="str">汉字</param> 
        /// <returns>首字母</returns> 
        public static string GetPinYinFirst(string str)
        {
            if (str.Length < 1) return "";
            String _Temp = null;
            for (int i = 0; i < str.Length; i++)
                _Temp = _Temp + GetOneIndex(str.Substring(i, 1));
            return _Temp.ToUpper();
        }
        //得到单个字符的首字母
        private static String GetOneIndex(String OneIndexTxt)
        {
            if (Convert.ToChar(OneIndexTxt) >= 0 && Convert.ToChar(OneIndexTxt) < 256)
                return OneIndexTxt;
            else
            {
                Encoding gb2312 = Encoding.GetEncoding("gb2312");
                byte[] unicodeBytes = Encoding.Unicode.GetBytes(OneIndexTxt);
                byte[] gb2312Bytes = Encoding.Convert(Encoding.Unicode, gb2312, unicodeBytes);
                return GetX(Convert.ToInt32(
             String.Format("{0:D2}", Convert.ToInt16(gb2312Bytes[0]) - 160)
             + String.Format("{0:D2}", Convert.ToInt16(gb2312Bytes[1]) - 160)
             ));
            }
        }

        //根据区位得到首字母
        private static String GetX(int GBCode)
        {
            if (GBCode >= 1601 && GBCode < 1637) return "A";
            if (GBCode >= 1637 && GBCode < 1833) return "B";
            if (GBCode >= 1833 && GBCode < 2078) return "C";
            if (GBCode >= 2078 && GBCode < 2274) return "D";
            if (GBCode >= 2274 && GBCode < 2302) return "E";
            if (GBCode >= 2302 && GBCode < 2433) return "F";
            if (GBCode >= 2433 && GBCode < 2594) return "G";
            if (GBCode >= 2594 && GBCode < 2787) return "H";
            if (GBCode >= 2787 && GBCode < 3106) return "J";
            if (GBCode >= 3106 && GBCode < 3212) return "K";
            if (GBCode >= 3212 && GBCode < 3472) return "L";
            if (GBCode >= 3472 && GBCode < 3635) return "M";
            if (GBCode >= 3635 && GBCode < 3722) return "N";
            if (GBCode >= 3722 && GBCode < 3730) return "O";
            if (GBCode >= 3730 && GBCode < 3858) return "P";
            if (GBCode >= 3858 && GBCode < 4027) return "Q";
            if (GBCode >= 4027 && GBCode < 4086) return "R";
            if (GBCode >= 4086 && GBCode < 4390) return "S";
            if (GBCode >= 4390 && GBCode < 4558) return "T";
            if (GBCode >= 4558 && GBCode < 4684) return "W";
            if (GBCode >= 4684 && GBCode < 4925) return "X";
            if (GBCode >= 4925 && GBCode < 5249) return "Y";
            if (GBCode >= 5249 && GBCode <= 5589) return "Z";
            if (GBCode >= 5601 && GBCode <= 8794)
            {
                String CodeData = "cjwgnspgcenegypbtwxzdxykygtpjnmjqmbsgzscyjsyyfpggbzgydywjkgaljswkbjqhyjwpdzlsgmr"
                 + "ybywwccgznkydgttngjeyekzydcjnmcylqlypyqbqrpzslwbdgkjfyxjwcltbncxjjjjcxdtqsqzycdxxhgckbphffss"
                 + "pybgmxjbbyglbhlssmzmpjhsojnghdzcdklgjhsgqzhxqgkezzwymcscjnyetxadzpmdssmzjjqjyzcjjfwqjbdzbjgd"
                 + "nzcbwhgxhqkmwfbpbqdtjjzkqhylcgxfptyjyyzpsjlfchmqshgmmxsxjpkdcmbbqbefsjwhwwgckpylqbgldlcctnma"
                 + "eddksjngkcsgxlhzaybdbtsdkdylhgymylcxpycjndqjwxqxfyyfjlejbzrwccqhqcsbzkymgplbmcrqcflnymyqmsqt"
                 + "rbcjthztqfrxchxmcjcjlxqgjmshzkbswxemdlckfsydsglycjjssjnqbjctyhbftdcyjdgwyghqfrxwckqkxebpdjpx"
                 + "jqsrmebwgjlbjslyysmdxlclqkxlhtjrjjmbjhxhwywcbhtrxxglhjhfbmgykldyxzpplggpmtcbbajjzyljtyanjgbj"
                 + "flqgdzyqcaxbkclecjsznslyzhlxlzcghbxzhznytdsbcjkdlzayffydlabbgqszkggldndnyskjshdlxxbcghxyggdj"
                 + "mmzngmmccgwzszxsjbznmlzdthcqydbdllscddnlkjyhjsycjlkohqasdhnhcsgaehdaashtcplcpqybsdmpjlpcjaql"
                 + "cdhjjasprchngjnlhlyyqyhwzpnccgwwmzffjqqqqxxaclbhkdjxdgmmydjxzllsygxgkjrywzwyclzmcsjzldbndcfc"
                 + "xyhlschycjqppqagmnyxpfrkssbjlyxyjjglnscmhcwwmnzjjlhmhchsyppttxrycsxbyhcsmxjsxnbwgpxxtaybgajc"
                 + "xlypdccwqocwkccsbnhcpdyznbcyytyckskybsqkkytqqxfcwchcwkelcqbsqyjqcclmthsywhmktlkjlychwheqjhtj"
                 + "hppqpqscfymmcmgbmhglgsllysdllljpchmjhwljcyhzjxhdxjlhxrswlwzjcbxmhzqxsdzpmgfcsglsdymjshxpjxom"
                 + "yqknmyblrthbcftpmgyxlchlhlzylxgsssscclsldclepbhshxyyfhbmgdfycnjqwlqhjjcywjztejjdhfblqxtqkwhd"
                 + "chqxagtlxljxmsljhdzkzjecxjcjnmbbjcsfywkbjzghysdcpqyrsljpclpwxsdwejbjcbcnaytmgmbapclyqbclzxcb"
                 + "nmsggfnzjjbzsfqyndxhpcqkzczwalsbccjxpozgwkybsgxfcfcdkhjbstlqfsgdslqwzkxtmhsbgzhjcrglyjbpmljs"
                 + "xlcjqqhzmjczydjwbmjklddpmjegxyhylxhlqyqhkycwcjmyhxnatjhyccxzpcqlbzwwwtwbqcmlbmynjcccxbbsnzzl"
                 + "jpljxyztzlgcldcklyrzzgqtgjhhgjljaxfgfjzslcfdqzlclgjdjcsnclljpjqdcclcjxmyzftsxgcgsbrzxjqqcczh"
                 + "gyjdjqqlzxjyldlbcyamcstylbdjbyregklzdzhldszchznwczcllwjqjjjkdgjcolbbzppglghtgzcygezmycnqcycy"
                 + "hbhgxkamtxyxnbskyzzgjzlqjdfcjxdygjqjjpmgwgjjjpkjsbgbmmcjssclpqpdxcdyykypcjddyygywchjrtgcnyql"
                 + "dkljczzgzccjgdyksgpzmdlcphnjafyzdjcnmwescsglbtzcgmsdllyxqsxsbljsbbsgghfjlwpmzjnlyywdqshzxtyy"
                 + "whmcyhywdbxbtlmswyyfsbjcbdxxlhjhfpsxzqhfzmqcztqcxzxrdkdjhnnyzqqfnqdmmgnydxmjgdhcdycbffallztd"
                 + "ltfkmxqzdngeqdbdczjdxbzgsqqddjcmbkxffxmkdmcsychzcmljdjynhprsjmkmpcklgdbqtfzswtfgglyplljzhgjj"
                 + "gypzltcsmcnbtjbhfkdhbyzgkpbbymtdlsxsbnpdkleycjnycdykzddhqgsdzsctarlltkzlgecllkjljjaqnbdggghf"
                 + "jtzqjsecshalqfmmgjnlyjbbtmlycxdcjpldlpcqdhsycbzsckbzmsljflhrbjsnbrgjhxpdgdjybzgdlgcsezgxlblg"
                 + "yxtwmabchecmwyjyzlljjshlgndjlslygkdzpzxjyyzlpcxszfgwyydlyhcljscmbjhblyjlycblydpdqysxktbytdkd"
                 + "xjypcnrjmfdjgklccjbctbjddbblblcdqrppxjcglzcshltoljnmdddlngkaqakgjgyhheznmshrphqqjchgmfprxcjg"
                 + "dychghlyrzqlcngjnzsqdkqjymszswlcfqjqxgbggxmdjwlmcrnfkkfsyyljbmqammmycctbshcptxxzzsmphfshmclm"
                 + "ldjfyqxsdyjdjjzzhqpdszglssjbckbxyqzjsgpsxjzqznqtbdkwxjkhhgflbcsmdldgdzdblzkycqnncsybzbfglzzx"
                 + "swmsccmqnjqsbdqsjtxxmbldxcclzshzcxrqjgjylxzfjphymzqqydfqjjlcznzjcdgzygcdxmzysctlkphtxhtlbjxj"
                 + "lxscdqccbbqjfqzfsltjbtkqbsxjjljchczdbzjdczjccprnlqcgpfczlclcxzdmxmphgsgzgszzqjxlwtjpfsyaslcj"
                 + "btckwcwmytcsjjljcqlwzmalbxyfbpnlschtgjwejjxxglljstgshjqlzfkcgnndszfdeqfhbsaqdgylbxmmygszldyd"
                 + "jmjjrgbjgkgdhgkblgkbdmbylxwcxyttybkmrjjzxqjbhlmhmjjzmqasldcyxyqdlqcafywyxqhz";
                String _gbcode = GBCode.ToString();
                int pos = (Convert.ToInt16(_gbcode.Substring(0, 2)) - 56) * 94 + Convert.ToInt16(_gbcode.Substring(_gbcode.Length - 2, 2));
                return CodeData.Substring(pos - 1, 1);
            }
            return " ";
        }
        #endregion

        #region 获取Sys_Para值
        /// <summary>
        /// 获取Sys_Para值
        /// </summary>
        /// <param name="key"></param>
        public static string GetSysPara(string key)
        {
            string result = "";
            DataTable dt = (DataTable)CacheHelper.GetCache("syspara");
            if (dt == null || dt.Rows.Count == 0)
            {
                string sql = "select * from sys_para where flag='Y' and  isdelete='N'";
                dt = DbHelperOra.Query(sql).Tables[0];

                CacheHelper.SetCache("syspara", dt);
            }
            DataRow[] dr = dt.Select("CODE='" + key + "'");
            if (dr.Length > 0)
            {
                result = dr[0]["VALUE"].ToString();
            }
            return result;
        }
        #endregion


        #region 多表头处理
        public static String GetGridTableHtml(FineUIPro.Grid grid)
        {
            StringBuilder sb = new StringBuilder();

            MultiHeaderTable mht = new MultiHeaderTable();
            mht.ResolveMultiHeaderTable(grid.Columns);
            sb.Append("<meta http-equiv=\"content-type\" content=\"application/excel; charset=UTF-8\"/>");
            sb.Append("<table cellspacing=\"0\" rules=\"all\" border=\"1\" style=\"border-collapse:collapse;\">");

            foreach (List<object[]> rows in mht.MultiTable)
            {
                sb.Append("<tr>");
                foreach (object[] cell in rows)
                {
                    int rowspan = Convert.ToInt32(cell[0]);
                    int colspan = Convert.ToInt32(cell[1]);
                    GridColumn column = cell[2] as GridColumn;

                    sb.AppendFormat("<th{0}{1}{2}>{3}</th>",
                        rowspan != 1 ? " rowspan=\"" + rowspan + "\"" : "",
                        colspan != 1 ? " colspan=\"" + colspan + "\"" : "",
                        colspan != 1 ? " style=\"text-align:center;\"" : "",
                        column.HeaderText);
                }
                sb.Append("</tr>");
            }

            foreach (GridRow row in grid.Rows)
            {
                sb.Append("<tr>");

                foreach (GridColumn column in mht.Columns)
                {
                    string html = row.Values[column.ColumnIndex].ToString();

                    if (column.ColumnID == "tfNumber")
                    {
                        html = (row.FindControl("spanNumber") as System.Web.UI.HtmlControls.HtmlGenericControl).InnerText;
                    }
                    else if (column.ColumnID == "tfNumber1")
                    {
                        html = (row.FindControl("spanNumber1") as System.Web.UI.HtmlControls.HtmlGenericControl).InnerText;
                    }
                    else if (column.ColumnID == "tfGender")
                    {
                        html = (row.FindControl("labGender") as FineUIPro.Label).Text;
                    }
                    sb.AppendFormat("<td>{0}</td>", html);
                }

                sb.Append("</tr>");
            }

            sb.Append("</table>");

            return sb.ToString();
        }
        public static String GetGridTableHtml(FineUIPro.Grid grid, DataTable dataSource)
        {
            StringBuilder sb = new StringBuilder();

            MultiHeaderTable mht = new MultiHeaderTable();
            mht.ResolveMultiHeaderTable(grid.Columns);
            sb.Append("<meta http-equiv=\"content-type\" content=\"application/excel; charset=UTF-8\"/>");
            sb.Append("<table cellspacing=\"0\" rules=\"all\" border=\"1\" style=\"border-collapse:collapse;\">");

            foreach (List<object[]> rows in mht.MultiTable)
            {
                sb.Append("<tr>");
                foreach (object[] cell in rows)
                {
                    int rowspan = Convert.ToInt32(cell[0]);
                    int colspan = Convert.ToInt32(cell[1]);
                    GridColumn column = cell[2] as GridColumn;

                    sb.AppendFormat("<th{0}{1}{2}>{3}</th>",
                        rowspan != 1 ? " rowspan=\"" + rowspan + "\"" : "",
                        colspan != 1 ? " colspan=\"" + colspan + "\"" : "",
                        colspan != 1 ? " style=\"text-align:center;\"" : "",
                        column.HeaderText);
                }
                sb.Append("</tr>");
            }

            foreach (DataRow row in dataSource.Rows)
            {
                sb.Append("<tr>");

                for (int i = 0; i < dataSource.Columns.Count; i++)
                {
                    sb.AppendFormat("<td>{0}</td>", row[i]);
                }

                sb.Append("</tr>");
            }

            sb.Append("</table>");
            return sb.ToString();
        }
        #endregion
        #region 处理多表头的类
        /// <summary>
        /// 处理多表头的类
        /// </summary>
        public class MultiHeaderTable
        {
            // 包含 rowspan，colspan 的多表头，方便生成 HTML 的 table 标签
            public List<List<object[]>> MultiTable = new List<List<object[]>>();
            // 最终渲染的列数组
            public List<GridColumn> Columns = new List<GridColumn>();


            public void ResolveMultiHeaderTable(GridColumnCollection columns)
            {
                List<object[]> row = new List<object[]>();
                foreach (GridColumn column in columns)
                {
                    object[] cell = new object[4];
                    cell[0] = 1;    // rowspan
                    cell[1] = 1;    // colspan
                    cell[2] = column;
                    cell[3] = null;

                    row.Add(cell);
                }

                ResolveMultiTable(row, 0);

                ResolveColumns(row);
            }

            private void ResolveColumns(List<object[]> row)
            {
                foreach (object[] cell in row)
                {
                    GroupField groupField = cell[2] as GroupField;
                    if (groupField != null && groupField.Columns.Count > 0)
                    {
                        List<object[]> subrow = new List<object[]>();
                        foreach (GridColumn column in groupField.Columns)
                        {
                            subrow.Add(new object[]
                            {
                        1,
                        1,
                        column,
                        groupField
                            });
                        }

                        ResolveColumns(subrow);
                    }
                    else
                    {
                        Columns.Add(cell[2] as GridColumn);
                    }
                }

            }

            private void ResolveMultiTable(List<object[]> row, int level)
            {
                List<object[]> nextrow = new List<object[]>();

                foreach (object[] cell in row)
                {
                    GroupField groupField = cell[2] as GroupField;
                    if (groupField != null && groupField.Columns.Count > 0)
                    {
                        // 如果当前列包含子列，则更改当前列的 colspan，以及增加父列（向上递归）的colspan
                        cell[1] = Convert.ToInt32(groupField.Columns.Count);
                        PlusColspan(level - 1, cell[3] as GridColumn, groupField.Columns.Count - 1);

                        foreach (GridColumn column in groupField.Columns)
                        {
                            nextrow.Add(new object[]
                            {
                        1,
                        1,
                        column,
                        groupField
                            });
                        }
                    }
                }

                MultiTable.Add(row);

                // 如果当前下一行，则增加上一行（向上递归）中没有子列的列的 rowspan
                if (nextrow.Count > 0)
                {
                    PlusRowspan(level);

                    ResolveMultiTable(nextrow, level + 1);
                }
            }

            private void PlusRowspan(int level)
            {
                if (level < 0)
                {
                    return;
                }

                foreach (object[] cells in MultiTable[level])
                {
                    GroupField groupField = cells[2] as GroupField;
                    if (groupField != null && groupField.Columns.Count > 0)
                    {
                        // ...
                    }
                    else
                    {
                        cells[0] = Convert.ToInt32(cells[0]) + 1;
                    }
                }

                PlusRowspan(level - 1);
            }

            private void PlusColspan(int level, GridColumn parent, int plusCount)
            {
                if (level < 0)
                {
                    return;
                }

                foreach (object[] cells in MultiTable[level])
                {
                    GridColumn column = cells[2] as GridColumn;
                    if (column == parent)
                    {
                        cells[1] = Convert.ToInt32(cells[1]) + plusCount;

                        PlusColspan(level - 1, cells[3] as GridColumn, plusCount);
                    }
                }
            }

        }
        #endregion


    }
}
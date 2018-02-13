using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XTBase;
using FineUIPro;
using System.Collections;
using System.Data;
namespace ERPProject
{
    public class DepartmentBind
    {
        private static Hashtable htKeyValue = new Hashtable();
        static DepartmentBind()
        {
            String deptOutSql = @"SELECT CODE, NAME
                                  FROM (SELECT '' CODE， '--请选择--' NAME
                                          FROM DUAL
                                        UNION ALL
                                        SELECT CODE, '['||CODE||']'||NAME NAME
                                          FROM SYS_DEPT
                                         WHERE TYPE = '1'
                                            AND F_CHK_DATARANGE(CODE, '{0}') = 'Y'
                                            ORDER BY CODE )
                                 ORDER BY CODE DESC ";
            //String deptInSql = @"SELECT CODE, NAME
            //                      FROM (SELECT '' CODE, '--请选择--' NAME
            //                              FROM DUAL
            //                            UNION ALL
            //                            SELECT CODE, '['||CODE||']'||NAME NAME
            //                              FROM SYS_DEPT
            //                             WHERE TYPE IN ('3','4')
            //                               AND F_CHK_DATARANGE(CODE, '{0}') = 'Y'
            //                             ORDER BY CODE)
            //                     ORDER BY NAME";
            String deptInSql = @"SELECT CODE, NAME
                                  FROM (SELECT '' CODE, '--请选择--' NAME
                                          FROM DUAL
                                        UNION ALL
                                        SELECT CODE, '['||CODE||']'||NAME NAME
                                          FROM SYS_DEPT
                                         WHERE TYPE IN ('3','4')
                                           AND F_CHK_DATARANGE(CODE, '{0}') = 'Y'
                                         ORDER BY CODE)
                                 ORDER BY NAME";
            String deptSql = @"SELECT CODE, NAME
                                  FROM (SELECT '' CODE, '--请选择--' NAME
                                          FROM DUAL
                                        UNION ALL
                                        SELECT CODE, '['||CODE||']'||NAME NAME
                                          FROM SYS_DEPT
                                         WHERE TYPE = '3'
                                           AND F_CHK_DATARANGE(CODE, '{0}') = 'Y'
                                         ORDER BY CODE)
                                 ORDER BY NAME";
            String deptallSql = @"SELECT CODE, NAME
                                  FROM (SELECT '' CODE, '--请选择--' NAME
                                          FROM DUAL
                                        UNION ALL
                                        SELECT CODE, '['||CODE||']'||NAME NAME
                                          FROM SYS_DEPT
                                         WHERE  F_CHK_DATARANGE(CODE, '{0}') = 'Y'
                                         ORDER BY CODE)
                                 ORDER BY NAME";
            String deptHasAth = @"SELECT CODE, NAME
                                  FROM (SELECT '' CODE, '--请选择--' NAME
                                          FROM DUAL
                                        UNION ALL
                                        SELECT CODE, '['||CODE||']'||NAME NAME
                                          FROM SYS_DEPT
                                          where F_CHK_DATARANGE(CODE, '{0}') = 'Y'
                                         ORDER BY CODE)
                                 ORDER BY NAME";
            String deptGroupHasAth = @"SELECT CODE, NAME
                                  FROM (SELECT '' CODE, '--请选择--' NAME
                                          FROM DUAL
                                        UNION ALL
                                        SELECT CODE, '['||CODE||']'||NAME NAME
                                          FROM SYS_DEPTGROUP
                                         ORDER BY CODE)
                                 ORDER BY NAME";
            String WorkbenchDDL = @"SELECT CODE, NAME
                                  FROM (SELECT '' CODE, '--请选择--' NAME
                                          FROM DUAL
                                        UNION ALL
                                        SELECT ID, INDEXNAME
                                          FROM SYS_DO_MODE)
                                 ORDER BY DECODE(CODE, '', 99, 0) DESC, CODE ASC ";
            htKeyValue.Add("DDL_SYS_DEPOTRANGE", deptOutSql);
            htKeyValue.Add("DDL_SYS_DEPTRANGE", deptInSql);
            htKeyValue.Add("DDL_SYS_DEPTALLRANGE", deptallSql);
            //权限科室
            htKeyValue.Add("DDL_SYS_DEPARTMENTRANGE", deptSql);
            htKeyValue.Add("DDL_SYS_DEPTHASATH", deptHasAth);
            htKeyValue.Add("DDL_SYS_DEPTGROUPHASATH", deptGroupHasAth);
            //工作台选择
            htKeyValue.Add("DDL_SYS_WORKBENCHDDL", WorkbenchDDL);
        }

        public static Boolean BindDDL(String key, String userId, params FineUIPro.DropDownList[] ddls)
        {
            Boolean result = false;
            try
            {
                String value = (String)htKeyValue[key];
                if (!String.IsNullOrWhiteSpace(value))
                {

                    DataTable dt = DbHelperOra.Query(String.Format(value, userId)).Tables[0];
                    foreach (FineUIPro.DropDownList ddl in ddls)
                    {
                        ddl.DataTextField = "NAME";
                        ddl.DataValueField = "CODE";
                        ddl.DataSource = dt;
                        ddl.DataBind();
                    }
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}
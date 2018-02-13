/*********************************************************************************
* 系 统 名 ：威高讯通院内物流管理系统HSC  V3.0
* 程 序 ID ：MenuHelper.cs
* 功能概要 ：系统自定义菜单
*
* 开发组织 ：威高讯通信息科技有限公司 产品研发部
*                      Copyright(C) 2015 WEGO System Service Co.Ltd
*                                                        All rights reserved.
*********************************************************************************
*   本程序为公司机密信息，未经公司书面许可授权，不得向第三方披露程序的相关信息。
*                                                   威高讯通信息科技有限公司
*********************************************************************************
* VERSION   DATE        BY             CHANGE/COMMENT
* 0.00      2015/04/08  c          新作成
*********************************************************************************/
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ERPProject
{
    public class MenuHelper
    {
        private static string strMenuSql = @"select funcid,funcname,treeid,decode(runwhat,'#','',runwhat) runwhat,funico,itemsort,treelevel,micohelp
                                from sys_function 
                               where funtype='FUN'
                                 and (instr('@ROLELIST','00',1)>0 or instr('@ROLELIST','admin',1)>0 or funcid in (select funcid from sys_rolefunc where instr('@ROLELIST',ROLEID,1)>0) )
                               order by itemsort ";

        private static List<MenuData> _menus;

        public static List<MenuData> Menus
        {
            get
            {
                if (_menus == null || _menus.Count == 0)
                {
                    InitMenus();
                }
                return _menus;
            }
        }

        public static void Reload()
        {
            _menus = null;
        }

        private static void InitMenus()
        {
            _menus = new List<MenuData>();

            List<MenuData> dbMenus = new List<MenuData>();
            DataTable dtMenu = DbHelperOra.Query(strMenuSql.Replace("@treeid", "00").Replace("@ROLELIST", "00")).Tables[0];
            if (dtMenu != null && dtMenu.Rows.Count > 0)
            {
                foreach (DataRow row in dtMenu.Rows)
                {
                    dbMenus.Add(new MenuData()
                    {
                        ID = row["funcid"].ToString(),
                        Name = row["funcname"].ToString(),
                        ImageUrl = row["funico"].ToString(),
                        NavigateUrl = row["runwhat"].ToString(),
                        SortIndex = int.Parse(row["itemsort"].ToString()),
                        ParentId = row["treeid"].ToString(),
                        TreeLevel = int.Parse(row["treelevel"].ToString()),
                        MicoHelp = row["micohelp"].ToString()
                    });
                }
            }

            ResolveMenuCollection(dbMenus, "00", 0);
        }

        private static int ResolveMenuCollection(List<MenuData> dbMenus, string parentMenuId, int level)
        {
            int count = 0;

            foreach (var menu in dbMenus.Where(m => m.ParentId == parentMenuId))
            {
                count++;

                _menus.Add(menu);
                menu.TreeLevel = level;
                menu.IsTreeLeaf = true;
                menu.Enabled = true;

                level++;
                int childCount = ResolveMenuCollection(dbMenus, menu.ID, level);
                if (childCount != 0)
                {
                    menu.IsTreeLeaf = false;
                }
                level--;
            }

            return count;
        }
    }
}
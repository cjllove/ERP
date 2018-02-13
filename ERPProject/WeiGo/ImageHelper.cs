using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPProject.WeiGo
{
    public class ImageHelper
    {
  
        /// <summary>
        /// 得到图片KEY
        /// </summary>
        /// <param name="gdseqs">商品编码“;”隔开</param>
        /// <param name="isList">是否是列表页面，默认true，true时有缓存（15分钟）</param>
        /// <param name="isSmall">是否缩略图，默认true</param>
        /// <returns></returns>
        public static List<String> GetImgs(String[] gdseqs,bool isList = true,bool isSmall = true)
        {
            String [] para = new String[gdseqs.Length+1];
            if (isSmall)
            {
                para[0] = "Y";
            }
            else {
                para[0] = "N";
            }
            List<String> keys = new List<string>();
            int countToGet = 0;
            for (int i = 0; i < gdseqs.Length; i++) {
                
                para[i + 1] = gdseqs[i];
                String key = "img_" + gdseqs[i];
                keys.Add(key);
                if(!isList){
                    key = "img_" + getGUID();
                }
                if (!ApiUtil.isCacheExist(key) && key != "img_") {
                    countToGet++;
                }
            }

           //如果有图片在本次缓存里没有，就调webservice 待优化
            if (countToGet > 0)
            {
                byte[][] lb = ApiClientService.getGoodsPics("DOC_GOODSPICTURE", para);
                int j = 0;
                foreach (byte[] b in lb)
                {
                    String key = keys[j];
                    if (!ApiUtil.isCacheExist(key) && key != "img_")
                    {
                        try
                        {
                            XTBase.Utilities.CacheHelper.SetCache(key, b, TimeSpan.FromMinutes(15));
                        }
                        catch { }

                    }
                    j++;
                }
            }
            
            return keys;
        }


        private static string getGUID()
        {
            System.Guid guid = new Guid();
            guid = Guid.NewGuid();
            string str = guid.ToString();
            return str;
        }
    }
}
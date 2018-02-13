using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPProject.captcha
{
    public class FileHelper
    {
        public FileHelper()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }
        /// <summary>
        /// 获取上传目录
        /// </summary>
        /// <returns></returns>
        public static string GetUploadPath()
        {
            //string path = HttpContext.Current.Server.MapPath("~/");
            //string dirname = GetDirName();
            //string uploadDir = path + "\\" + dirname;
            string path = ApiUtil.GetConfigCont("UPLOADDIR");
            string uploadDir = path+"SupCert";
            CreateDir(uploadDir);
            return uploadDir;
        }
        /// <summary>
        /// 获取临时目录
        /// </summary>
        /// <returns></returns>
        public static string GetTempPath()
        {
            //string path = HttpContext.Current.Server.MapPath("~/");
            //string dirname = GetTempDirName();
            //string uploadDir = path + "\\" + dirname;
            string path = ApiUtil.GetConfigCont("UPLOADDIR");
            string uploadDir = path + "SupCert";
            CreateDir(uploadDir);
            return uploadDir;
        }
        private static string GetDirName()
        {
            return System.Configuration.ConfigurationManager.AppSettings["UPLOADDIR"];//uploaddir
        }
        private static string GetTempDirName()
        {
            return System.Configuration.ConfigurationManager.AppSettings["UPLOADDIR"];//tempdir
        }
        public static void CreateDir(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
    }
}
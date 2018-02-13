using FineUIPro;
using Newtonsoft.Json.Linq;
using XTBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace ERPProject.captcha
{
    /// <summary>
    /// LicenseUploadPictures 的摘要说明
    /// </summary>
    public class LicenseUploadPictures : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            //owner 为传递的参数，用于向数据库存值，应为商品id等唯一编码
            string owner = context.Request.Form["owner"];
            string[] arrays = owner.Split('|'); 
            

            if (context.Request.Files.Count == 0)
            {
                ResponseError(context);
                return;
            }

            if (String.IsNullOrEmpty(owner))
            {
                ResponseError(context);
                return;
            }
            System.Guid guid = new Guid();
            guid = Guid.NewGuid();

            string rowno = guid.ToString();

            HttpPostedFile postedFile = context.Request.Files[0];
            // 文件名完整路径
            string fileName = postedFile.FileName;
            // 文件名保存的服务器路径
            string savedFileName = GetSavedFileName(fileName);
            //路径加入 年/月日 防止同目录文件数溢出
            string monthPath = GetMonthPath();
            //最终完整的路径
            string finalPath = "~/ERPupload/LicensesPic/" + monthPath;
            //最终完整的路径+文件名
            string finalPathFile = finalPath + savedFileName;
            if (!Directory.Exists(context.Server.MapPath(finalPath)))
            {
                Directory.CreateDirectory(context.Server.MapPath(finalPath));
            }

            //E:\威高项目\公版ERP\ERP3_Pro\ERPProject\ERPupload\LicensesPic\2017\1\10\

            postedFile.SaveAs(context.Server.MapPath(finalPathFile));

            string shortFileName = GetFileName(fileName);
            string fileType = GetFileType(fileName);
            int fileSize = postedFile.ContentLength;

            #region 向数据库存储值
            //JObject fileObj = new JObject();
            //string fileId = Guid.NewGuid().ToString();

            //fileObj.Add("rowno", rowno);
            //fileObj.Add("name", shortFileName);
            //fileObj.Add("picname", savedFileName);
            //fileObj.Add("type", fileType);
            //fileObj.Add("path", finalPathFile);
            //fileObj.Add("size", fileSize);
            //fileObj.Add("id", fileId);

            //if (arrays[0].Contains("SQ"))
            //{
            //    SQSaveToDatabase1(context, owner, fileObj);
            //}
            //else
            //{
            //    SaveToDatabase(context, owner, fileObj);
            //}
            
            #endregion

            context.Response.Write(finalPathFile);
        }

        private JObject GetFileObject(JArray source, string fileId)
        {
            for (int i = 0, count = source.Count; i < count; i++)
            {
                JObject item = source[i] as JObject;

                if (item.Value<string>("id") == fileId)
                {
                    return item;
                }
            }
            return null;
        }

        //授权图片上传进行保存
        private void SQSaveToDatabase1(HttpContext context, string owner, JObject fileObj)
        {
            //写你自己的sql语句执行即可
            //上传商品图片时，owner可为商品id
            List<CommandInfo> cmdList = new List<CommandInfo>();
            string[] arrays = owner.Split('|');

            decimal i = Convert.ToDecimal(DbHelperOra.GetSingle("select count(1) c from doc_license_img where seqno='" + arrays[0] + "'"));
            MyTable mtType = new MyTable("doc_license_img");
            mtType.ColRow["SEQNO"] = arrays[0];
            mtType.ColRow["ROWNO"] = i + 1;
            mtType.ColRow["FLAG"] = "N";
            mtType.ColRow["IMGPATH"] = fileObj["path"];
            mtType.ColRow["UPTTIME"] = System.DateTime.Now;
            mtType.ColRow["SUPID"] = arrays[1];
            cmdList.Add(mtType.Insert());
            DbHelperOra.ExecuteSqlTran(cmdList);
        }

        //TODO 保存到数据库
        private void SaveToDatabase(HttpContext context, string owner, JObject fileObj)
        {
            //写你自己的sql语句执行即可
            //上传商品图片时，owner可为商品id
            List<CommandInfo> cmdList = new List<CommandInfo>();
            string[] arrays = owner.Split('|');

            decimal i = Convert.ToDecimal(DbHelperOra.GetSingle("select count(1) c from doc_license_img where seqno='" + arrays[0] + "'"));
            MyTable mtType = new MyTable("doc_license_img");
            mtType.ColRow["SEQNO"] = arrays[0];
            mtType.ColRow["ROWNO"] = i+1;
            mtType.ColRow["FLAG"] = "N";
            mtType.ColRow["IMGPATH"] = fileObj["path"];
            mtType.ColRow["GDSEQ"] = arrays[3];
            mtType.ColRow["UPTTIME"] = System.DateTime.Now;
            mtType.ColRow["SUPID"] = arrays[1];
            mtType.ColRow["LICENSEID"] = arrays[2];
            cmdList.Add(mtType.Insert());
            DbHelperOra.ExecuteSqlTran(cmdList);
            int picn = Convert.ToInt32(DbHelperOra.GetSingle("select count(1) from doc_license_img where seqno='" + arrays[0] + "' and licenseid='" + arrays[2] + "'"));
            DbHelperOra.ExecuteSql(string.Format("update doc_license_log set picnum = {0} where seqno='{1}' and licenseid='{2}'", picn, arrays[0], arrays[2]));
        }

        private string GetFileType(string fileName)
        {
            string fileType = String.Empty;
            int lastDotIndex = fileName.LastIndexOf(".");
            if (lastDotIndex >= 0)
            {
                fileType = fileName.Substring(lastDotIndex + 1).ToLower();
            }

            return fileType;
        }


        private string GetFileName(string fileName)
        {
            string shortFileName = fileName;
            int lastSlashIndex = shortFileName.LastIndexOf("\\");
            if (lastSlashIndex >= 0)
            {
                shortFileName = shortFileName.Substring(lastSlashIndex + 1);
            }

            return shortFileName;
        }

        private string GetSavedFileName(string fileName)
        {
            fileName = fileName.Replace(":", "_").Replace(" ", "_").Replace("\\", "_").Replace("/", "_");
            fileName = DateTime.Now.Ticks.ToString() + "_" + fileName;

            return fileName;
        }

        private string GetMonthPath()
        {
            string result = "";
            DateTime date = DateTime.Now;
            string year = date.Year.ToString();
            string month = date.Month.ToString();
            string day = date.Day.ToString();
            result = year + "/" + month + "/" + day + "/";
            return result;
        }

        private void ResponseError(HttpContext context)
        {
            // 出错了
            context.Response.StatusCode = 500;
            context.Response.Write("No file");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
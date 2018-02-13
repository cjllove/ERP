using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XTBase;
using System.Data;
using FineUIPro;

namespace ERPProject.captcha
{
    /// <summary>
    /// PictureServer 的摘要说明
    /// </summary>
    public class PictureServer : IHttpHandler
    {
        private Boolean result = false;
        private String message = "";
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            HttpRequest Request = HttpContext.Current.Request;
            String licenseid = Request["licenseid"];
            UploadFile(context, licenseid);
            JObject jo = new JObject();
            jo.Add("success", result);
            jo.Add("message", message);
            context.Response.Write(JsonConvert.SerializeObject(jo));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public Boolean UploadFile(HttpContext context, string licenseid)
        {
            message = "";
            int rowno = 1;//存放图片的行数
            string pic_path = "", seqn = "";
            result = false;
            context.Response.CacheControl = "no-cache";
            string s_rpath = FileHelper.GetUploadPath();//@"E:\My Documents\Visual Studio 2008\WebSites\SWFUpload\demos\applicationdemo.net";

            List<CommandInfo> cmdList = new List<CommandInfo>();

            string Datedir = DateTime.Now.ToString("yy-MM-dd");
            string updir = s_rpath + "\\" + Datedir;
            FileHelper.CreateDir(updir);
            string extname = string.Empty;
            string fullname = string.Empty;
            string filename = string.Empty;

            if (context.Request.Files.Count > 0)
            {
                try
                {
                    for (int j = 0; j < context.Request.Files.Count; j++)
                    {
                        HttpPostedFile uploadFile = context.Request.Files[j];
                        int offset = Convert.ToInt32(context.Request["chunk"]); //当前分块
                        int total = Convert.ToInt32(context.Request["chunks"]);//总的分块数量
                        string name = context.Request["name"];
                        //文件没有分块
                        if (total == 1)
                        {
                            if (uploadFile.ContentLength > 0)
                            {
                                if (!System.IO.Directory.Exists(updir))
                                {
                                    System.IO.Directory.CreateDirectory(updir);
                                }
                                extname = System.IO.Path.GetExtension(uploadFile.FileName);
                                fullname = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                                filename = uploadFile.FileName;

                                uploadFile.SaveAs(string.Format("{0}\\{1}", updir, filename));
                                result = true;
                            }
                        }
                        else
                        {
                            //文件 分成多块上传
                            fullname = WriteTempFile(uploadFile, offset);
                            if (total - offset == 1 || total == 0)
                            {
                                //如果是最后一个分块文件 ，则把文件从临时文件夹中移到上传文件夹中
                                System.IO.FileInfo fi = new System.IO.FileInfo(fullname);
                                string oldFullName = string.Format("{0}\\{1}", updir, uploadFile.FileName);
                                    string mypath = "";
                                    System.IO.FileInfo oldFi = new System.IO.FileInfo(oldFullName);
                                    if (oldFi.Exists)
                                    {
                                        //文件名存在则删除旧文件 
                                        oldFi.Delete();
                                    }
                                    string pathMain = ApiUtil.GetConfigCont("UPLOADDIR");
                                    //fi.MoveTo(oldFullName);
                                    string spath = "SupCert\\" + Datedir + "\\";
                                    //string spath = oldFullName.Substring(10);
                                    DataTable dt = DbHelperOra.Query("select max(rowno) from DOC_GOODSPICTURE where gdseq='" + licenseid + "' nvl(str2,'N') = 'N'").Tables[0];
                                    if (dt != null && dt.Rows.Count != 0)
                                    {
                                        string oldnum = dt.Rows[0][0].ToString();
                                        if (!String.IsNullOrWhiteSpace(oldnum))
                                        {
                                            rowno = Convert.ToInt32(oldnum) + 1;
                                            mypath = "~/" + ("ERPUpload\\" + spath).Replace("\\", "/");
                                            pic_path = mypath;//pathMain + "SupCert\\" + Datedir + "\\";//mypath.Substring(0, oldFullName.LastIndexOf("\\") + 1);
                                            seqn = licenseid + "_" + rowno;
                                            pic_path = pic_path + seqn + ".jpg";
                                        }
                                        else
                                        {
                                            mypath = "~/" + ("ERPUpload\\" + spath).Replace("\\", "/");
                                            pic_path = mypath;//.Substring(0, oldFullName.LastIndexOf("\\") + 1);
                                            seqn = licenseid + "_" + rowno;
                                            pic_path = pic_path + seqn + ".jpg";
                                        }
                                    }
                                    string thePicPath = pathMain + spath + seqn + ".jpg";
                                    System.IO.FileInfo oldFi1 = new System.IO.FileInfo(thePicPath);
                                    if (oldFi1.Exists)
                                    {
                                        //文件名存在则删除旧文件 
                                        oldFi1.Delete();
                                    }
                                    fi.MoveTo(thePicPath);
                                    //E:\UPLOAD\SupCert\15-04-17\1-120Q0092042.jpg   --->oldFullName
                                    //E:\\UPLOAD\\SupCert\15-05-03\授权设计.xlsx
                                    cmdList.Add(new CommandInfo(string.Format("update doc_goods set ISFLAG4 = 'Y' where gdseq='{0}'", licenseid), null)); 
                                    cmdList.Add(new CommandInfo("insert into DOC_GOODSPICTURE(GDSEQ,ROWNO,GDPICT,PICPATH,FLAG,STR1) values('" + licenseid + "','" + rowno + "','" + seqn + "','" + pic_path + "','N','" + seqn + "')", null));
                                    result = true;
                                    DbHelperOra.ExecuteSqlTran(cmdList);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    message = ex.ToString();
                    //context.Response.Write("Message" + ex.ToString());
                }
            }
            return result;
        }

        /// <summary>
        /// 保存临时文件 
        /// </summary>
        /// <param name="uploadFile"></param>
        /// <param name="chunk"></param>
        /// <returns></returns>
        private string WriteTempFile(HttpPostedFile uploadFile, int chunk)
        {

            string tempDir = FileHelper.GetTempPath();
            if (!System.IO.Directory.Exists(tempDir))
            {
                System.IO.Directory.CreateDirectory(tempDir);
            }
            string fullName = string.Format("{0}\\{1}.part", tempDir, uploadFile.FileName);
            if (chunk == 0)
            {
                //如果是第一个分块，则直接保存
                uploadFile.SaveAs(fullName);
            }
            else
            {
                //如果是其他分块文件 ，则原来的分块文件，读取流，然后文件最后写入相应的字节
                System.IO.FileStream fs = new System.IO.FileStream(fullName, System.IO.FileMode.Append);
                if (uploadFile.ContentLength > 0)
                {
                    int FileLen = uploadFile.ContentLength;
                    byte[] input = new byte[FileLen];

                    // Initialize the stream.
                    System.IO.Stream MyStream = uploadFile.InputStream;

                    // Read the file into the byte array.
                    MyStream.Read(input, 0, FileLen);

                    fs.Write(input, 0, FileLen);
                    fs.Close();
                }
            }
            return fullName;
        }
    }
}
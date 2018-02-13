using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using XTBase;
using XTBase.Utilities;
using XTFramework;

namespace ERPProject
{
    public partial class LoginHis : System.Web.UI.Page
    {
        protected string Huserid = "";//登录名
        protected string Hoperid = "";//操作人
        protected string Hdept = "";//科室编码
        protected string Hdeptname = "";//科室名称
        protected string Hpatient = "";//病人ID
        protected string Hpatientname = "";//
        protected string Hvisit = "";//住院次数
        protected string Hdoctor = "";//医生ID
        protected string Hdoctorname = "";//医生姓名
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (HISLogin())
                {
                    
                    string mURL = string.Format("~/ERPApply/DrugConsumptionHIS.aspx?Hdept={0}&Hoperid={1}&Hpatient={2}&Hvisit={3}&Hdoctor={4}&Hdoctorname={5}",Hdept.ToString(),Hoperid.ToString(),Hpatient.ToString(),Hvisit.ToString(),Hdoctor.ToString(),Hdoctorname.ToString());
                    Response.Redirect(mURL, true);
                }
            }
            Response.End();
        }

        private bool HISLogin()
        {
            string userName = string.Empty;
            string password = string.Empty;
            if (Request.QueryString["la"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["la"].ToString()))
            {
                userName = Request.QueryString["la"].ToString();
                Huserid = userName;
            }
            else
            {
                return false;
            }
            if (Request.QueryString["pw"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["pw"].ToString()))
            {
                password = Request.QueryString["pw"].ToString();
            }
            else
            {
                return false;
            }
            if (Request.QueryString["operator_no"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["operator_no"].ToString()))
            {
                Hoperid = Request.QueryString["operator_no"].ToString();
            }
            else
            {
                return false;
            }
            if (Request.QueryString["PERFORMED_BY"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["PERFORMED_BY"].ToString()))
            {
                Hdept = Request.QueryString["PERFORMED_BY"].ToString();
            }
            else
            {
                return false;
            }
            //if (Request.QueryString["Hdeptname"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["Hdeptname"].ToString()))
            //{
            //    Hdeptname = Request.QueryString["Hdeptname"].ToString();
            //}
            //else
            //{
            //    return false;
            //}
            if (Request.QueryString["patient_id"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["patient_id"].ToString()))
            {
                Hpatient = Request.QueryString["patient_id"].ToString();
            }
            else
            {
                return false;
            }
            if (Request.QueryString["visit_id"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["visit_id"].ToString()))
            {
                Hvisit = Request.QueryString["visit_id"].ToString();
            }
            else
            {
                return false;
            }
            if (Request.QueryString["doctor_user"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["doctor_user"].ToString()))
            {
                Hdoctor = Request.QueryString["doctor_user"].ToString();
            }
            else
            {
                return false;
            }
            if (Request.QueryString["order_doctor"] != null && !string.IsNullOrWhiteSpace(Request.QueryString["order_doctor"].ToString()))
            {
                Hdoctorname = Request.QueryString["order_doctor"].ToString();
            }
            else
            {
                return false;
            }
            LoginInfo login = UserLogin(userName);

            if (login != null)
            {
                bool isEncrypt = EncryptionUtil.ComparePasswords(login.UserPwd, password);
                if (isEncrypt)
                {
                    if (!login.Enabled)
                    {
                        return false;
                    }
                    else if (login.UserStatus != "01")
                    {
                        return false;
                    }
                    else
                    {
                        // 登录成功 
                        HttpCookie myCookie = new HttpCookie("YUAN_" + Request.Url.Authority, userName + "@" + password + "@N@N");
                        myCookie.Expires = System.DateTime.Now.AddMinutes(600);
                        Response.Cookies.Add(myCookie);

                        UserInfo user = new UserInfo()
                        {
                            UserDept = login.UserDept,
                            UserID = login.UserID,
                            UserName = login.UserName,
                            UserPwd = login.UserPwd,
                            UserRole = login.UserRole,
                            Enabled = login.Enabled
                        };

                        DateTime expiration = DateTime.Now.AddMinutes(120);
                        CreateFormsAuthenticationTicket(user.UserID, JsonConvert.SerializeObject(user), false, expiration);

                        return true;
                    }
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }

        /// <summary> 
        /// 获取登录用户信息 
        /// </summary> 
        /// <param name="account">登录账号</param> 
        /// <returns></returns> 
        public LoginInfo UserLogin(string account)
        {
            LoginInfo model = new LoginInfo();
            string query = string.Format(@"SELECT USERID, USERGH, USERPWD, USERNAME, ROLEID, DEPT, ISLOGIN,STATUS
                                                            FROM SYS_OPERUSER
                                                          WHERE ISDELETE = 'N'
                                                              AND (TRIM(UPPER(USERID)) = UPPER('{0}') OR UPPER(USERGH) = UPPER('{0}') OR
                                                                       TRIM(UPPER(USERNAME)) = UPPER('{0}'))", account);
            DataTable dtUser = new DataTable();
            string str = ConfigurationManager.ConnectionStrings["OracleConnString"].ConnectionString;
            dtUser = DbHelperOra.Query(query).Tables[0];
            if (dtUser != null && dtUser.Rows.Count > 0)
            {
                model.Enabled = dtUser.Rows[0]["ISLOGIN"].ToString() == "Y" ? true : false;
                model.UserDept = dtUser.Rows[0]["DEPT"].ToString();
                model.UserID = dtUser.Rows[0]["USERID"].ToString();
                model.UserStatus = dtUser.Rows[0]["STATUS"].ToString();
                model.UserName = dtUser.Rows[0]["USERNAME"].ToString();
                model.UserPwd = dtUser.Rows[0]["USERPWD"].ToString();
                model.UserRole = dtUser.Rows[0]["ROLEID"].ToString();
            }
            else
            {
                return null;
            }
            return model;
        }

        /// <summary> 
        /// 创建表单验证的票证并存储在客户端Cookie中 
        /// </summary> 
        /// <param name="userName">当前登录用户名</param> 
        /// <param name="userData">当前登录用户的登录信息</param> 
        /// <param name="isPersistent">是否跨浏览器会话保存票证</param> 
        /// <param name="expiration">过期时间</param> 
        public void CreateFormsAuthenticationTicket(string userName, string userData, bool isPersistent, DateTime expiration)
        {
            // 创建Forms身份验证票据 
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
            userName, // 与票证关联的用户名 
            DateTime.Now, // 票证发出时间 
            expiration, // 票证过期时间 
            isPersistent, // 如果票证将存储在持久性 Cookie 中（跨浏览器会话保存），则为 true；否则为 false。 
            userData // 存储在票证中的用户数据 
            );

            // 对Forms身份验证票据进行加密，然后保存到客户端Cookie中 
            string hashTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hashTicket);
            cookie.HttpOnly = true;
            // 1. 关闭浏览器即删除（Session Cookie）：DateTime.MinValue 
            // 2. 指定时间后删除：大于 DateTime.Now 的某个值 
            // 3. 删除Cookie：小于 DateTime.Now 的某个值 
            if (isPersistent)
            {
                cookie.Expires = expiration;
            }
            else
            {
                cookie.Expires = DateTime.MinValue;
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}
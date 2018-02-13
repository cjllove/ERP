using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FineUIPro;
using XTBase;
using XTBase.Utilities;
using System.Data;
using System.Web.Security;
using Newtonsoft.Json;

namespace ERPProject.pad
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                sysName.Text = ConfigurationManager.AppSettings["APPNAME"];
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string userName = tbxUserName.Text.Trim();
            string password = tbxPassword.Text.Trim();

            if (PubFunc.StrIsEmpty(userName))
            {
                Alert.Show("用户名不能为空，请输入用户名！", "消息提示", MessageBoxIcon.Warning);
                return;
            }
            if (PubFunc.StrIsEmpty(password))
            {
                Alert.Show("密码不能为空，请输入登录密码！", "消息提示", MessageBoxIcon.Warning);
                return;
            }

            UserInfo user = myLogin(userName);

            if (user != null)
            {
                bool isEncrypt = false;
                if (hfdISPASSWORD.Text == "Y")
                {
                    isEncrypt = (user.UserPwd == password);
                }
                else
                {
                    isEncrypt = EncryptionUtil.ComparePasswords(user.UserPwd, password);
                }
                if (isEncrypt)
                {
                    if (!user.Enabled)
                    {
                        Alert.Show("用户未启用，请联系管理员！");
                    }
                    else
                    {
                        // 登录成功 
                        //logger.Info(String.Format("登录成功：用户“{0}”", user.Name)); 
                        SetCookie(userName, user.UserPwd, "Y", "Y", 600);
                        //SetCookie(userName, user.UserPwd, rememberme.Checked ? "Y" : "N", rememberme.Checked ? "Y" : "N", 600); 
                        LoginSuccess(user);

                        return;
                    }
                }
                else
                {
                    //logger.Warn(String.Format("登录失败：用户“{0}”密码错误", userName)); 
                    Alert.Show("用户名或密码错误！");
                    return;
                }

            }
            else
            {
                //logger.Warn(String.Format("登录失败：用户“{0}”不存在", userName)); 
                Alert.Show("用户名或密码错误！");
                return;
            }
        }
        public UserInfo myLogin(string account)
        {
            UserInfo model = new UserInfo();
            string query = string.Format(@"select USERID,USERGH,USERPWD,USERNAME,ROLEID,DEPT,ISLOGIN from SYS_OPERUSER 
where ISDELETE='N' and (USERID = '{0}' or USERGH = '{0}' or USERNAME = '{0}')", account);
            DataTable dtUser = new DataTable();
            string str = ConfigurationManager.ConnectionStrings["OracleConnString"].ConnectionString;
            dtUser = DbHelperOra.Query(query).Tables[0];
            if (dtUser != null && dtUser.Rows.Count > 0)
            {
                model.Enabled = dtUser.Rows[0]["ISLOGIN"].ToString() == "Y" ? true : false;
                model.UserDept = dtUser.Rows[0]["DEPT"].ToString();
                model.UserID = dtUser.Rows[0]["USERID"].ToString();
                //model.UserGH = dtUser.Rows[0]["USERGH"].ToString();
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

        public void SetCookie(string userName, string password, string saveUser, string savePwd, int minutesCount)
        {
            HttpCookie myCookie = new HttpCookie("YUAN_" + Request.Url.Authority, userName + "@" + password + "@" + saveUser + "@" + savePwd);
            myCookie.Expires = System.DateTime.Now.AddMinutes(minutesCount);
            Response.Cookies.Add(myCookie);
        }

        private void LoginSuccess(UserInfo user)
        {
            //RegisterOnlineUser(user); 

            bool isPersistent = false;
            DateTime expiration = DateTime.Now.AddMinutes(120);
            CreateFormsAuthenticationTicket(user.UserID, JsonConvert.SerializeObject(user), isPersistent, expiration);

            // 重定向到登陆后首页 
            Response.Redirect("/pad/index.aspx", false);
        }

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
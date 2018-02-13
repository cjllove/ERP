using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using XTBase;

namespace ERPProject.WeiGo
{
    public class Dat_MsgTel_List
    {        
        private string _seqno;
        private string _rqsj;
        private string _msgtype;
        private string _istiming;
        private string _timingsj;
        private string _flag;
        private string _senduser;
        private string _sendcust;
        private string _sendsj;
        private string _senddoc;
        private string _gettel;
        private string _retmsg;
        private string _retsj;
        private string _retph;
        private string _class;

        public string seqno
        {
            set { _seqno = value; }
            get { return _seqno; }
        }
        public string rqsj
        {
            set { _rqsj = value; }
            get { return _rqsj; }
        }
        public string msgType
        {
            set { _msgtype = value; }
            get { return _msgtype; }
        }
        public string isTiming
        {
            set { _istiming = value; }
            get { return _istiming; }
        }
        public string timingSj
        {
            set { _timingsj = value; }
            get { return _timingsj; }
        }
        public string flag
        {
            set { _flag = value; }
            get { return _flag; }
        }
        public string sendUser
        {
            set { _senduser = value; }
            get { return _senduser; }
        }
        public string sendCust
        {
            set { _sendcust = value; }
            get { return _sendcust; }
        }
        public string sendSj
        {
            set { _sendsj = value; }
            get { return _sendsj; }
        }
        public string sendDoc
        {
            set { _senddoc = value; }
            get { return _senddoc; }
        }
        public string getTel
        {
            set { _gettel = value; }
            get { return _gettel; }
        }
        public string retMsg
        {
            set { _retmsg = value; }
            get { return _retmsg; }
        }
        public string retSj
        {
            set { _retsj = value; }
            get { return _retsj; }
        }
        public string retPh
        {
            set { _retph = value; }
            get { return _retph; }
        }
        public string Class
        {
            set { _class = value; }
            get { return _class; }
        }

        public Dat_MsgTel_List()
        {
            _class = "1";
            _msgtype = "1";
        }

        public Dat_MsgTel_List(DataRow dr)
        {
            _seqno = dr["seqno"].ToString();
            _rqsj = dr["rqsj"].ToString();
            _msgtype = dr["msgtype"].ToString();
            _istiming = dr["istiming"].ToString();
            _timingsj = dr["timingsj"].ToString();
            _flag = dr["flag"].ToString();
            _senduser = dr["senduser"].ToString();
            _sendcust = dr["sendcust"].ToString();
            _sendsj = dr["sendsj"].ToString();
            _senddoc = dr["senddoc"].ToString();
            _gettel = dr["gettel"].ToString();
            _retmsg = dr["retmsg"].ToString();
            _retsj = dr["retsj"].ToString();
            _retph = dr["retph"].ToString();
            _class = dr["class"].ToString();
        }

        public static int runProcedure(String procedureName, params OracleParameter[] parameters)
        {
            int oraResult = 0;
            OracleConnection conn = new OracleConnection(DbHelperOra.connectionString);
            OracleCommand command = new OracleCommand(procedureName);
            command.Connection = conn;
            command.CommandType = CommandType.StoredProcedure;
            foreach (OracleParameter param in parameters)
            {
                command.Parameters.Add(param);
            }
            try
            {
                conn.Open();
                command.ExecuteNonQuery();
                oraResult = 1;
            }
            catch (Exception ex)
            {
                oraResult = -1;
                throw ex;
            }
            finally
            {
                conn.Close();

            }
            return oraResult;
        }
        
        public static string AddDat_MsgTel_List(Dat_MsgTel_List singleDat_MsgTel_List)
        {
            string procname = "MSGTEL.AddDat_MsgTel_List";
            OracleParameter[] prams = {
		    new OracleParameter("vs_seqno",OracleDbType.Varchar2,16),
            new OracleParameter("vs_rqsj",OracleDbType.Varchar2,30),
            new OracleParameter("vs_msgType",OracleDbType.Char,1),
            new OracleParameter("vs_isTiming",OracleDbType.Char,1),
            new OracleParameter("vs_timingSj",OracleDbType.Varchar2,50),
            new OracleParameter("vs_flag",OracleDbType.Char,1),
            new OracleParameter("vs_sendUser",OracleDbType.Varchar2,50),
            new OracleParameter("vs_sendCust",OracleDbType.Varchar2,50),
            new OracleParameter("vs_sendSj",OracleDbType.Varchar2,50),
            new OracleParameter("vs_sendDoc",OracleDbType.Varchar2,500),
            new OracleParameter("vs_getTel",OracleDbType.Varchar2,2000),
            new OracleParameter("vs_retMsg",OracleDbType.Varchar2,2000),
            new OracleParameter("vs_retSj",OracleDbType.Varchar2,50),
            new OracleParameter("vs_retPh",OracleDbType.Varchar2,50),
            new OracleParameter("vs_class",OracleDbType.Char,1)
                                };

            prams[0].Value = singleDat_MsgTel_List.seqno;
            prams[1].Value = singleDat_MsgTel_List.rqsj;
            prams[2].Value = singleDat_MsgTel_List.msgType;
            prams[3].Value = singleDat_MsgTel_List.isTiming;
            prams[4].Value = singleDat_MsgTel_List.timingSj;
            prams[5].Value = singleDat_MsgTel_List.flag;
            prams[6].Value = singleDat_MsgTel_List.sendUser;
            prams[7].Value = singleDat_MsgTel_List.sendCust;
            prams[8].Value = singleDat_MsgTel_List.sendSj;
            prams[9].Value = singleDat_MsgTel_List.sendDoc;
            prams[10].Value = singleDat_MsgTel_List.getTel;
            prams[11].Value = singleDat_MsgTel_List.retMsg;
            prams[12].Value = singleDat_MsgTel_List.retSj;
            prams[13].Value = singleDat_MsgTel_List.retPh;
            prams[14].Value = singleDat_MsgTel_List.Class;
            prams[0].Direction = ParameterDirection.InputOutput;

            runProcedure(procname, prams);

            return prams[0].Value.ToString();
        }

        public static Dat_MsgTel_List GetDat_MsgTel_ListBySeqno(string seqno)
        {
            Dat_MsgTel_List dat_MsgTel_List = null;
            DataTable dt = DbHelperOra.Query(string.Format("select * from dat_msgtel_list where seqno='{0}'", seqno)).Tables[0];
            if(dt != null&&dt.Rows.Count > 0)
            {
                dat_MsgTel_List = new Dat_MsgTel_List(dt.Rows[0]);        
            }
            return dat_MsgTel_List;
        }

        public static string GetDat_MsgTel_ListJsonBySeqno(string seqno)
        {
            string Dat_MsgTel_ListJson = "";
            DataTable dt = DbHelperOra.Query(string.Format("select * from dat_msgtel_list where seqno='{0}'", seqno)).Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                Dat_MsgTel_ListJson = JsonConvert.SerializeObject(dt);
            }
            return Dat_MsgTel_ListJson;
        }

        public static bool MessageCancel(string seqno)
        {
            bool bFlag = false;
            int result = DbHelperOra.ExecuteSql(string.Format("update dat_msgtel_list set flag='I' where seqno='{0}'", seqno));
            if(result>0)
            {
                bFlag = true;
            }
            return bFlag;
        }
        public static string MessageOper(String sendDoc, String getTel, String isTiming, String timingSj, String sendUser, String sendCust, String Class)
        {
            JObject retJObject = new JObject();
            //验证字段有效性
            if (!String.IsNullOrWhiteSpace(sendDoc))
            {
                if (sendDoc.Length > 500)
                {
                    retJObject.Add("state", "短信内容不能超过500个字符");
                    return retJObject.ToString();
                }
                else
                { 
                //可以过滤关键字
                }
            }
            else
            {
                retJObject.Add("state", "发送短信内容为空");
                return retJObject.ToString();
            }

            if (!String.IsNullOrWhiteSpace(getTel))
            {
                if (getTel.Length > 1200)
                {
                    retJObject.Add("state", "接收短信电话号码不能超过100个号码");
                    return retJObject.ToString();
                }
                else
                {
                    //可以验证电话号码有效性
                }
            }
            else
            {
                retJObject.Add("state", "接收短信电话号码为空");
                return retJObject.ToString();
            }

            if (!String.IsNullOrWhiteSpace(isTiming)&&"1".Equals(isTiming))
            {
                isTiming = "1";
                if (!String.IsNullOrWhiteSpace(timingSj))
                {
                    try
                    {
                        if (Convert.ToDateTime(timingSj)<DateTime.Now)
                        {
                            retJObject.Add("state", "定时时间必须大于当前时间");
                            return retJObject.ToString();
                        }
                        timingSj = Convert.ToDateTime(timingSj).ToString("yyyy-MM-dd HH:mm:ss");//转换时有问题
                    }
                    catch
                    {
                        retJObject.Add("state", "发送时间格式错误");
                        return retJObject.ToString();
                    }
                }
                else
                {
                    retJObject.Add("state", "定时发送时间为空");
                    return retJObject.ToString();
                }
            }
            else
            {
                isTiming = "0";
                timingSj = null;
            }

            if(String.IsNullOrWhiteSpace(sendUser))
            {
                retJObject.Add("state", "发送人为空");
                return retJObject.ToString();
            }
            if (String.IsNullOrWhiteSpace(sendCust))
            {
                retJObject.Add("state", "发送用户为空");
                return retJObject.ToString();
            }
            if (String.IsNullOrWhiteSpace(Class))
            {
                Class = "1";
            }

            Dat_MsgTel_List dat_MsgTel_List = new Dat_MsgTel_List();
            dat_MsgTel_List.sendDoc = sendDoc;
            dat_MsgTel_List.getTel = getTel;
            dat_MsgTel_List.sendUser = sendUser;
            dat_MsgTel_List.sendCust = sendCust;
            dat_MsgTel_List.rqsj = DateTime.Now.ToString();
            dat_MsgTel_List.isTiming = isTiming;
            dat_MsgTel_List.timingSj = timingSj;
            dat_MsgTel_List.Class = Class;
            dat_MsgTel_List.flag = "N";

            string result = "短信已存入数据库，等待发送";
            if ("1".Equals(isTiming))//如果是定时发送，直接保存信息至数据库
            {
                string seqno = Dat_MsgTel_List.AddDat_MsgTel_List(dat_MsgTel_List);

                retJObject.Add("seqno", seqno);
                retJObject.Add("state", result);
                result = retJObject.ToString();
            }
            else
            {
                result = MessageOper(dat_MsgTel_List);
            }
            return result;
        }
        public static string MessageOper(string seqno)
        {
            return MessageOper(GetDat_MsgTel_ListBySeqno(seqno));
        }
        public static string MessageOper(Dat_MsgTel_List singleDat_MsgTel_List)
        { 
            //先发送短信
            singleDat_MsgTel_List.sendSj = DateTime.Now.ToString();
            object sendState = null;
            //if ("0".Equals(singleDat_MsgTel_List.isTiming))
            //{
                sendState = SendMessage(singleDat_MsgTel_List.sendDoc, singleDat_MsgTel_List.getTel);
            //}
            //else
            //{
            //    sendState = SendMessageTimer(singleDat_MsgTel_List.sendDoc, singleDat_MsgTel_List.getTel,singleDat_MsgTel_List.timingSj);
            //}

            //处理返回值
            string strState = "";
            string strPh = "";
            string strFlag = "S";//S-发送成功；E-发送不成功
            if (sendState != null)
            {
                System.Reflection.FieldInfo fieldInfoId = sendState.GetType().GetField("Id");
                int intId = (int)fieldInfoId.GetValue(sendState);
                strPh = intId.ToString();
                System.Reflection.FieldInfo fieldInfoState = sendState.GetType().GetField("State");
                int intState = (int)fieldInfoState.GetValue(sendState); ;
                System.Reflection.FieldInfo fieldInfoFailPhone = sendState.GetType().GetField("FailPhone");
                string strFailPhone = (string)fieldInfoFailPhone.GetValue(sendState);

                switch (intState)
                {
                    case 0:
                        strState = "帐户处于禁止使用状态";
                        break;
                    case 1:
                        strState = "发送短信成功";
                        break;
                    case -1:
                        strState = "发送失败";
                        break;
                    case -2:
                        strState = "帐户信息错误";
                        break;
                    case -3:
                        strState = "机构代码、用户名或密码错误";
                        break;
                    case -4:
                        strState = "不是普通帐户";
                        break;
                    case -5:
                        strState = "发送短信内容为空";
                        break;
                    case -6:
                        strState = "短信内容过长";
                        break;
                    case -7:
                        strState = "发送号码为空";
                        break;
                    case -8:
                        strState = "余额不足";
                        //如果余额不足，应提醒系统管理员

                        break;
                    case -9:
                        strState = "接收数据失败";
                        break;
                    case -10:
                        strState = "号码错误";
                        break;
                    case -11:
                        strState = "发送时间格式错误";
                        break;
                    case -12:
                        strState = "定时时间必须大于当前时间";
                        break;
                    case -13://如是失败-13，返回strFailPhone是包含的关键字
                        strState = "内容含关键字" + strFailPhone;
                        break;
                    case -14:
                        strState = "信息内容格式与限定格式不符";
                        break;
                    case -15:
                        strState = "信息没带签名";
                        break;
                    case -30:
                        strState = "非绑定IP";
                        break;
                    case -100:
                        strState = "客户端获取状态失败";
                        break;
                    default:
                        strState = "发送失败,不明原因";
                        break;
                }

                //如果strFailPhone不是空值，说明该批次有部分接收电话发送失败
                if (intState!=-13&&!String.IsNullOrWhiteSpace(strFailPhone))
                {
                    strState = strFailPhone + "发送失败";
                }
            }
            else
            {
                strState = "发送失败,连接不到短信接口";
                strFlag = "E";
            }

            singleDat_MsgTel_List.retMsg = strState;
            singleDat_MsgTel_List.retSj = DateTime.Now.ToString();
            singleDat_MsgTel_List.retPh = strPh;
            singleDat_MsgTel_List.flag = strFlag;
            //短信保存到数据库
            //AddDat_MsgTel_List(singleDat_MsgTel_List);
            string seqno = Dat_MsgTel_List.AddDat_MsgTel_List(singleDat_MsgTel_List);
            JObject jo = new JObject();
            jo.Add("seqno", seqno);
            jo.Add("state", strState);

            return jo.ToString();
        }

        //定时器调用发送定时短信
        public static void MessageOperTimer()
        {
            Dat_MsgTel_List dat_MsgTel_List = null;
            string strSql = @"select * from dat_msgtel_list
                              where flag='N' and isTiming='1' and timingsj<to_date('{0}','yyyy-mm-dd hh24:mi:ss') 
                              order by CLASS DESC";
            DataTable dt = DbHelperOra.Query(string.Format(strSql, DateTime.Now.ToString())).Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    dat_MsgTel_List = new Dat_MsgTel_List(dr);
                    MessageOper(dat_MsgTel_List);
                }
            }
        }

        //发送短信
        public static object SendMessage(string sendDoc, string getTel)
        {
            int Id = 300;//机构代码
            try
            {
                Id = Convert.ToInt32(ApiClientUtil.GetConfigCont("MESSAGE_ID"));
            }
            catch
            { }
            string Name = ApiClientUtil.GetConfigCont("MESSAGE_NAME");//账户名
            string Psw = ApiClientUtil.GetConfigCont("MESSAGE_PWD");//密码
            string webServiceURL = ApiClientUtil.GetConfigCont("MESSAGE_WEBSERVICEURL");//webService地址
            long Timestamp = GetTimeStamp();
            //SendMessage(int Id, string Name, string Psw, string Message, string Phone, int Timestamp)
            object result = null;
            try
            {
                //Message.ServiceSoap. service= new Message.ServiceSoap();
                //Messages.ServiceSoapClient ser = new Messages.ServiceSoapClient();

                //Messages.SendState c= ser.SendMessage(Id,Name,Psw,sendDoc,getTel,Timestamp);
                //result = c.Id.ToString() + "|" + c.State + "|" + c.FailPhone;

                result = DynamicServiceBind.InvokeWebService(webServiceURL, "SendMessage", new object[] { Id, Name, Psw, sendDoc, getTel, Timestamp });
            }
            catch (Exception e)
            {
  
            }
            return result;
        }
        //定时发送短信
        public static object SendMessageTimer(string sendDoc, string getTel, string sendDatetime)
        {
            int Id = 300;//机构代码
            try
            {
                Id = Convert.ToInt32(ApiClientUtil.GetConfigCont("MESSAGE_ID"));
            }
            catch
            { }
            string Name = ApiClientUtil.GetConfigCont("MESSAGE_NAME");//账户名
            string Psw = ApiClientUtil.GetConfigCont("MESSAGE_PWD");//密码
            string webServiceURL = ApiClientUtil.GetConfigCont("MESSAGE_WEBSERVICEURL");//webService地址
            long Timestamp = GetTimeStamp();
            //SendTimer(int Id, string Name, string Psw, string Message, string Phone,String DateTime, long Timestamp)
            object result = "";
            try
            {
                result = DynamicServiceBind.InvokeWebService(webServiceURL, "SendTimer", new object[] { Id, Name, Psw, sendDoc, getTel, sendDatetime, Timestamp });
            }
            catch (Exception e)
            {
 
            }
            return result;
        }
        //获取帐户短信可用数量
        public static int GetBalance()
        {
            int Id = 300;//机构代码
            try
            {
                Id = Convert.ToInt32(ApiClientUtil.GetConfigCont("MESSAGE_ID"));
            }
            catch
            { }
            string Name = ApiClientUtil.GetConfigCont("MESSAGE_NAME");//账户名
            string Psw = ApiClientUtil.GetConfigCont("MESSAGE_PWD");//密码
            string webServiceURL = ApiClientUtil.GetConfigCont("MESSAGE_WEBSERVICEURL");//webService地址

            //GetBalance(int Id, string Name, string Psw)
            int Balance = -1;
            object BalanceState = null;
            try
            {
                BalanceState = DynamicServiceBind.InvokeWebService(webServiceURL, "GetBalance", new object[] { Id, Name, Psw });
            }
            catch (Exception e)
            {
                throw e;
            }

            if (BalanceState != null)
            {
                System.Reflection.FieldInfo fieldInfoBalance = BalanceState.GetType().GetField("Balance");
                Balance = (int)fieldInfoBalance.GetValue(BalanceState);
            }

            return Balance;
        }

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        } 
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Services.Description;
using System.CodeDom;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace ERPProject
{
    /// <summary>
    ///动态调用WebService
    ///由于EAS需要SOAP头验证，已在数据中心端废弃,只用于不需要SessionId的接口调用，比如登陆
    /// </summary>
    /// 
    //[System.Web.Services.Protocols.SoapDocumentService(RoutingStyle = System.Web.Services.Protocols.SoapServiceRoutingStyle.RequestElement)]
    public class DynamicServiceBind
    {
        /// <summary>
        /// 获取WebService类型
        /// </summary>
        /// <param name="wsUrl">WebService地址</param>
        /// <returns></returns>
        private static string GetWsClassName(string wsUrl)
        {
            string[] parts = wsUrl.Split('/');
            string[] pps = parts[parts.Length - 1].Split('.');

            return pps[0];
        }




        /// <summary>
        /// 调用WebService
        /// </summary>
        /// <param name="wsUrl">WebService地址</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="args">参数列表</param>
        /// <returns></returns>
        public static object InvokeWebService( string wsUrl, string methodName, object[] args)
        {
            return InvokeWebService(wsUrl, null, methodName, args);
        }
        

        /// <summary>
        /// 调用WebService
        /// </summary>
        /// <param name="wsUrl">WebService地址</param>
        /// <param name="className">类名</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="args">参数列表</param>
        /// <returns></returns>
        public static object InvokeWebService(string wsUrl, string className, string methodName, object[] args)
        {
            string @namespace = "ERPProject.WebService.DynamicWebCalling";
            if ((className == null) || (className == ""))
            {
                className = GetWsClassName(wsUrl);
            }

            try
            {
                //获取WSDL
                WebClient wc = new WebClient();
                Stream stream = wc.OpenRead(wsUrl + "?wsdl");
                ServiceDescription sd = ServiceDescription.Read(stream);
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace(@namespace);

                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider csc = new CSharpCodeProvider();
                ICodeCompiler icc = csc.CreateCompiler();

                //设定编译参数
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;
                cplist.GenerateInMemory = true;
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");

                //编译代理类
                CompilerResults cr = icc.CompileAssemblyFromDom(cplist, ccu);
                if (true == cr.Errors.HasErrors)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                    {
                        sb.Append(ce.ToString());
                        sb.Append(System.Environment.NewLine);
                    }
                    throw new Exception(sb.ToString());
                }

                //生成代理实例，并调用方法
                System.Reflection.Assembly assembly = cr.CompiledAssembly;
                Type t = assembly.GetType(@namespace + "." + className, true, true);
                object obj = Activator.CreateInstance(t);
                Type[] types = new Type[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    types[i] = args[i].GetType();
                }
                
                #region soapheader信息
                //System.Reflection.FieldInfo[] arry = t.GetFields();

                //System.Reflection.FieldInfo fieldHeader = null;
                ////soapheader 对象值
                //object objHeader = null;
                //if (soapHeader != null)
                //{
                //    fieldHeader = t.GetField(soapHeader.ClassName);

                //    Type tHeader = assembly.GetType(@namespace + "." + soapHeader.ClassName);
                //    objHeader = Activator.CreateInstance(tHeader);

                //    foreach (KeyValuePair<string, object> property in soapHeader.Properties)
                //    {
                //        System.Reflection.FieldInfo[] arry1 = tHeader.GetFields();
                //        int ts = arry1.Length;
                //        System.Reflection.FieldInfo f = tHeader.GetField(property.Key);
                //        if (f != null)
                //        {
                //            f.SetValue(objHeader, property.Value);
                //        }
                //    }
                //}

                //if (soapHeader != null && fieldHeader != null)
                //{
                //    //设置Soap头
                //    fieldHeader.SetValue(obj, objHeader);
                //}

                #endregion


                System.Reflection.MethodInfo mi = t.GetMethod(methodName,types);

                return mi.Invoke(obj, args);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException.Message, new Exception(ex.InnerException.StackTrace));
            }
        }


        /// <summary>
        /// SOAP头
        /// </summary>
        public class SoapHeader
        {
            /// <summary>
            /// 构造一个SOAP头
            /// </summary>
            public SoapHeader()
            {
                this.Properties = new Dictionary<string, object>();
            }

            /// <summary>
            /// 构造一个SOAP头
            /// </summary>
            /// <param name="className">SOAP头的类名</param>
            public SoapHeader(string className)
            {
                this.ClassName = className;
                this.Properties = new Dictionary<string, object>();
            }

            /// <summary>
            /// 构造一个SOAP头
            /// </summary>
            /// <param name="className">SOAP头的类名</param>
            /// <param name="properties">SOAP头的类属性名及属性值</param>
            public SoapHeader(string className, Dictionary<string, object> properties)
            {
                this.ClassName = className;
                this.Properties = properties;
            }

            /// <summary>
            /// SOAP头的类名
            /// </summary>
            public string ClassName
            {
                get;
                set;
            }

            /// <summary>
            /// SOAP头的类属性名及属性值
            /// </summary>
            public Dictionary<string, object> Properties
            {
                get;
                set;
            }

            /// <summary>
            /// 为SOAP头增加一个属性及值
            /// </summary>
            /// <param name="name">SOAP头的类属性名</param>
            /// <param name="value">SOAP头的类属性值</param>
            public void AddProperty(string name, object value)
            {
                if (this.Properties == null)
                {
                    this.Properties = new Dictionary<string, object>();
                }
                Properties.Add(name, value);
            }
        }



    }

   
    public class SessionIdRender : System.Web.Services.Protocols.SoapHeader
    {
        public SessionIdRender()
        {
            Namespaces = new XmlSerializerNamespaces();

            sessionId = String.Empty;
        }
         
        //[SoapElement]
        [XmlText]
        public String sessionId { get; set; }

        //[XmlElement(Namespace = "http://login.webservice.bos.kingdee.com")]
        //public string sessionId
        //{
        //    get;
        //    set;
        //}

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Namespaces { get; set; }
    }
}
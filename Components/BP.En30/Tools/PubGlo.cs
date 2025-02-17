﻿using BP.DA;
using BP.En;
using BP.Sys;
using BP.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BP.Difference;
using System.Collections;

namespace BP.Tools
{
    public class PubGlo
    {
        #region 表达式替换
        /// <summary>
        /// 表达式替换
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string DealExp(string exp, Entity en)
        {
            //替换字符
            exp = exp.Replace("~", "'");

            if (exp.Contains("@") == false)
                return exp;

            //首先替换加; 的。
            exp = exp.Replace("@WebUser.No;", WebUser.No);
            exp = exp.Replace("@WebUser.Name;", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_DeptName;", WebUser.DeptName);
            exp = exp.Replace("@WebUser.FK_Dept;", WebUser.DeptNo);


            // 替换没有 ; 的 .
            exp = exp.Replace("@WebUser.No", WebUser.No);
            exp = exp.Replace("@WebUser.Name", WebUser.Name);
            exp = exp.Replace("@WebUser.FK_DeptName", WebUser.DeptName);
            exp = exp.Replace("@WebUser.FK_Dept", WebUser.DeptNo);

            if (exp.Contains("@") == false)
                return exp;

            //增加对新规则的支持. @MyField; 格式.
            if (en != null)
            {
                Attrs attrs = en.EnMap.Attrs;
                Row row = en.Row;
                //特殊判断.
                if (row.ContainsKey("OID") == true)
                    exp = exp.Replace("@WorkID", row["OID"].ToString());

                if (exp.Contains("@") == false)
                    return exp;

                foreach (string key in row.Keys)
                {
                    //值为空或者null不替换
                    if (row[key] == null || row[key].Equals("") == true)
                        continue;

                    if (exp.Contains("@" + key))
                    {
                        Attr attr = attrs.GetAttrByKeyOfEn(key);

                        //@hongyan.
                        if (exp.Contains("@" + key))
                        {
                            if (row.ContainsKey(key + "Text") == true)
                                exp = exp.Replace("@" + key, row[key + "Text"].ToString());
                            else if (row.ContainsKey(key + "T") == true)
                                exp = exp.Replace("@" + key, row[key + "T"].ToString());
                            else
                                exp = exp.Replace("@" + key, row[key].ToString());
                        }
                    }

                    //不包含@则返回SQL语句
                    if (exp.Contains("@") == false)
                        return exp;
                }
            }

            if (exp.Contains("@") && BP.Difference.SystemConfig.isBSsystem == true)
            {
                /*如果是bs*/
                foreach (string key in HttpContextHelper.RequestParamKeys)
                {
                    if (string.IsNullOrEmpty(key))
                        continue;
                    exp = exp.Replace("@" + key, HttpContextHelper.RequestParams(key));
                }
            }

            exp = exp.Replace("~", "'");
            return exp;
        }
        #endregion 表达式替换

        #region http请求
        /// <summary>
        /// Http Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            try
            {
                HttpWebRequest request;
                // 创建一个HTTP请求
                request = (HttpWebRequest)WebRequest.Create(url);
                // request.Method="get";
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string responseText = myreader.ReadToEnd();
                myreader.Close();
                response.Close();
                return responseText;
            }
            catch (Exception ex)
            {
                //url请求失败
                return ex.Message;
            }
        }
        public static string HttpPostConnect_Data(string dbSrNo, string exp, Hashtable ht, string requestMethod)
        {  //返回值
            //用户输入的webAPI地址
            string apiUrl = exp;
            if (apiUrl.Contains("@WebApiHost"))//可以替换配置文件中配置的webapi地址
                apiUrl = apiUrl.Replace("@WebApiHost", BP.Difference.SystemConfig.AppSettings["WebApiHost"]);

            var mysrc = new SFDBSrc(dbSrNo);
            //拼接路径.
            apiUrl = mysrc.ConnString + apiUrl;

            #region 解析路径变量 /{xxx}/{yyy} ? xxx=xxx
            if (apiUrl.Contains("{") == true)
            {
                if (ht != null)
                {
                    foreach (string key in ht.Keys)
                    {
                        apiUrl = apiUrl.Replace("{" + key + "}", ht[key].ToString());
                    }
                }
            }
            #endregion

            string[] strs = apiUrl.Split('?');
            //api接口地址
            string apiHost = strs[0];
            //api参数
            string apiParams = "";
            if (strs.Length == 2)
                apiParams = strs[1];
            return BP.Tools.PubGlo.HttpPostConnect(apiHost, apiParams, requestMethod);
        }
        /// <summary>
        /// httppost方式发送数据
        /// </summary>
        /// <param name="url">要提交的url</param>
        /// <param name="postDataStr"></param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="encode">text code.</param>
        /// <returns>成功：返回读取内容；失败：0</returns>
        public static string HttpPostConnect(string serverUrl, string postData, string requestMethod = "POST", bool isJsonModel = false)
        {
            //serverUrl = serverUrl.Replace("//", "/");
            //serverUrl = serverUrl.Replace("//", "/");

            var dataArray = Encoding.UTF8.GetBytes(postData);
            //创建请求
            var request = (HttpWebRequest)HttpWebRequest.Create(serverUrl);
            request.Method = requestMethod;
            request.ContentLength = dataArray.Length;
            //设置上传服务的数据格式  设置之后不好使
            //request.ContentType = "application/x-www-form-urlencoded";
            //请求的身份验证信息为默认
            request.Credentials = CredentialCache.DefaultCredentials;

            if (isJsonModel == true)
                request.ContentType = "application/json";
            else
                request.ContentType = "application/x-www-form-urlencoded";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //请求超时时间
            request.Timeout = 10000;
            //创建输入流
            Stream dataStream;
            if (requestMethod.ToLower().Equals("get") == false)
            {
                try
                {
                    dataStream = request.GetRequestStream();
                    //发送请求
                    dataStream.Write(dataArray, 0, dataArray.Length);
                    dataStream.Close();
                }
                catch (Exception)
                {
                    return "0";//连接服务器失败
                }
            }

            HttpWebResponse res;
            try
            {
                res = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                res = (HttpWebResponse)ex.Response;
            }
            StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
            //读取返回消息
            string data = sr.ReadToEnd();
            sr.Close();
            return data;
        }
        public static string HttpPostConnect(string serverUrl, Hashtable headerMap, string postData)
        {
            var dataArray = Encoding.UTF8.GetBytes(postData);
            //创建请求
            var request = (HttpWebRequest)HttpWebRequest.Create(serverUrl);
            request.Method = "POST";
            request.ContentLength = dataArray.Length;
            //设置上传服务的数据格式  设置之后不好使
            //request.ContentType = "application/x-www-form-urlencoded";
            //请求的身份验证信息为默认
            request.Credentials = CredentialCache.DefaultCredentials;
            // request.ContentType = "application/x-www-form-urlencoded";
            request.ContentType = "application/json";
            //  request.ContentType = "application/x-www-form-urlencoded";

            //设置请求头
            if (headerMap.Count > 0)
            {
                foreach (string key in headerMap.Keys)
                {
                    request.Headers.Add(key, headerMap[key].ToString());
                }
            }

            //请求超时时间
            request.Timeout = 10000;
            //创建输入流
            Stream dataStream;
            try
            {
                dataStream = request.GetRequestStream();
            }
            catch (Exception)
            {
                return "0";//连接服务器失败
            }
            //发送请求
            dataStream.Write(dataArray, 0, dataArray.Length);
            dataStream.Close();

            HttpWebResponse res;

            res = (HttpWebResponse)request.GetResponse();

            //}
            //catch (WebException ex)
            //{
            //    throw new Exception(ex.Message);
            //    //res = (HttpWebResponse)ex.Response;
            //}
            StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8);
            //读取返回消息
            string data = sr.ReadToEnd();
            sr.Close();
            return data;
        }
    }
    #endregion http请求
}

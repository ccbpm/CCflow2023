﻿using System.Net;
using System.IO;
using System.Collections.Generic;
using BP.Difference;


namespace BP.WF.HttpHandler
{
    public class HttpHandlerGlo
    {
        #region 转化格式  chen
        public static void DownloadFile(string filepath, string tempName)
        {
            //if (String.Compare("firefox", HttpContextHelper.RequestBrowser, StringComparison.OrdinalIgnoreCase)!=0)
            //    tempName = HttpUtility.UrlEncode(tempName);

            HttpContextHelper.ResponseWriteFile(filepath, tempName);
        }

        /// <summary>
        /// 从别的网站服务器上下载文件
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="tempName"></param>
        public static void DownloadHttpFile(string filepath, string tempName)
        {
            List<byte> byteList = new List<byte>();

            //打开网络连接
            string filePth = filepath.Replace("\\", "/").Trim();
            if (filepath.IndexOf("/") == 0)
            {
                filepath = filepath.Remove(1, filepath.Length - 1);
            }
            if (!SystemConfig.AttachWebSite.Trim().EndsWith("/"))
            {
                filepath =  BP.Difference.SystemConfig.AttachWebSite.Trim() + "/" + filepath;
            }
            else
            {
                filepath =  BP.Difference.SystemConfig.AttachWebSite.Trim() + filepath;
            }

            HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(filepath);
            //向服务器请求,获得服务器的回应数据流
            using (Stream myStream = myRequest.GetResponse().GetResponseStream())
            {
                //定义一个字节数据
                byte[] btContent = new byte[512];
                int intSize = 0;
                intSize = myStream.Read(btContent, 0, 512);
                while (intSize > 0)
                {
                    if (intSize == 512)
                        byteList.AddRange(btContent);
                    else
                    {
                        byte[] btContent2 = new byte[intSize];
                        for (int i = 0; i < btContent2.Length; i++)
                        {
                            btContent2[i] = btContent[i];
                        }
                        byteList.AddRange(btContent2);
                    }
                    intSize = myStream.Read(btContent, 0, 512);
                }

                myStream.Close();
                HttpContextHelper.ResponseWriteFile(byteList.ToArray(), tempName);
            }
        }
        public static void OpenWordDoc(string filepath, string tempName)
        {
            HttpContextHelper.ResponseWriteFile(filepath, tempName, "application/ms-msword");
        }
        public static void OpenWordDocV2(string filepath, string tempName)
        {
            HttpContextHelper.ResponseWriteFile(filepath, tempName);
        }
        #endregion
    }
}

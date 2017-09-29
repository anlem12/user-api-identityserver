using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace UserInfo.API.Core
{
    /// <summary>
    /// Request
    /// </summary>
    public  class LJLJRequest
    {
        /// <summary>
        /// Http Get
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static string RequestGet(string strUrl)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.ContinueTimeout = 60000;
                var rsp = (HttpWebResponse)request.GetResponseAsync().Result;
                using (StreamReader reader = new StreamReader(rsp.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetUserIp(Controller controller)
        {
            return controller.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}

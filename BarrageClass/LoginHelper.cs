using System;
using System.IO;
using System.Net;

namespace BarrageClass
{
    class LoginHelper
    {

        public static bool Login(String _targetUrl,String _queryString) {
            String loginResult = HttpGet(_targetUrl, _queryString);

            //Console.WriteLine("[get-result]="+loginResult);

            if (loginResult.Equals("success"))
            {
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Http Get 请求
        /// </summary>
        /// <param name="url">请求的网址</param>
        /// <param name="query">GET请求的参数，如：key1=value1&key2=value2</param>
        /// <returns></returns>
        private static string HttpGet(String url,string query) {
            url += "?" + query;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);
            string result = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            return result;
        }
    }
}

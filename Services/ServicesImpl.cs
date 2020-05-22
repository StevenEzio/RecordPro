﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RecordPRO.Services
{
    public class ServicesImpl:IServices
    {
        public static String Md5(string s)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();
            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }
            return ret.PadLeft(32, '0');
        }

        public string ExtractKeywords(string content)
        {
            string x_appid = "5ec7de18";
            string api_key = "ca9a83f10cd4e0a9449a0c50ed1e9023";


            string param = "{\"type\":\"dependent\"}";

            System.Text.Encoding encode = System.Text.Encoding.ASCII;
            byte[] bytedata = encode.GetBytes(param);
            string x_param = Convert.ToBase64String(bytedata);

            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string curTime = Convert.ToInt64(ts.TotalSeconds).ToString();

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string result = string.Format("{0}{1}{2}", api_key, curTime, x_param);
            string x_checksum = Md5(result);

            string cc = HttpUtility.UrlEncode(content, Encoding.UTF8);
            string data = "text=" + cc;

            String Url = "http://ltpapi.xfyun.cn/v1/ke";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers["X-Appid"] = x_appid;
            request.Headers["X-CurTime"] = curTime;
            request.Headers["X-Param"] = x_param;
            request.Headers["X-Checksum"] = x_checksum;

            request.ContentLength = Encoding.UTF8.GetByteCount(data);
            Stream requestStream = request.GetRequestStream();
            StreamWriter streamWriter = new StreamWriter(requestStream, Encoding.GetEncoding("gb2312"));
            streamWriter.Write(data);
            streamWriter.Close();

            string htmlStr = string.Empty;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream responseStream = response.GetResponseStream();
            using (StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8")))
            {
                htmlStr = reader.ReadToEnd();
            }
            responseStream.Close();
            JObject jresponse = JObject.Parse(htmlStr);

            var keywords = "";
            foreach(var i in jresponse["data"]["ke"])
            {
                keywords += i.Value<string>("word") + "/";
            }
            return keywords;
        }
    }
}

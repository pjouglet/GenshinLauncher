﻿using Launcher.Model;
using Newtonsoft.Json;
using Launcher.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Common
{
    public static class ServerInfoGetter
    {
        public static string scheme = "https";

        public static async Task<string> HttpGet(string url, Dictionary<string, string> dic = null)
        {
            HttpResponseMessage response;
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };
            #region 参数添加
            StringBuilder builder = new StringBuilder();
            builder.Append(url);
            if (dic != null)
            {

                if (dic.Count > 0)
                {
                    builder.Append("?");
                    int i = 0;
                    foreach (var item in dic)
                    {
                        if (i > 0)
                            builder.Append("&");
                        builder.AppendFormat("{0}={1}", item.Key, item.Value);
                        i++;
                    }
                }
            }
            #endregion
            try
            {

                response = await new HttpClient(handler).GetAsync(new Uri(builder.ToString()));
            }
            catch (Exception e)
            {
                return null;
            }
            string result = await response.Content.ReadAsStringAsync();

            return result;
        }

        class REPDT
        {
            public class Status
            {
                /// <summary>
                /// 
                /// </summary>
                public int playerCount { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string version { get; set; }
            }

            public class Root
            {
                /// <summary>
                /// 
                /// </summary>
                public int retcode { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public Status status { get; set; }
            }

        }


        public static async Task<ServerInfo> GetAsync(string ip)
        {

            var Url = $"{scheme}://{ip}/status/server";
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var r = await HttpGet(url: Url);

            sw.Stop();
            REPDT.Root dt;
            try
            {
                dt = JsonConvert.DeserializeObject<REPDT.Root>(r);
                if (dt==null)
                {
                    return new ServerInfo();
                }

            }
            catch
            {
                dt = new REPDT.Root();
            }

            var SI = new ServerInfo();
            if (dt.status==null)
            {
                return new ServerInfo();
            }
            SI.ver = dt.status.version;
            SI.players = dt.status.playerCount.ToString();
            SI.timeout = sw.ElapsedMilliseconds.ToString();


            return SI;



        }

        internal static async Task<List<AnnounceMentItem>> GetAnnounceAsync(string ip)
        {

            var Url = $"{scheme}://{ip}/glannouncement/list";
            Stopwatch sw = new Stopwatch();

            var r = await HttpGet(url: Url);

            List<AnnounceMentItem> ret;
            try
            {
                ret = JsonConvert.DeserializeObject<List<AnnounceMentItem>>(r);

            }
            catch (Exception ex)
            {
                ret = new List<AnnounceMentItem>();
            }


            return ret;



        }


    }

}

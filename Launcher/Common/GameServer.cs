using Launcher.Model;
using Newtonsoft.Json;
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
        public static string Scheme = "https";

        public static async Task<string?> HttpGet(string url, Dictionary<string, string> dic)
        {
            HttpResponseMessage response;
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = delegate { return true; };
            #region 参数添加
            StringBuilder builder = new StringBuilder();
            builder.Append(url);
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
            #endregion
            try
            {

                response = await new HttpClient(handler).GetAsync(new Uri(builder.ToString()));
            }
            catch (Exception)
            {
                return null;
            }
            string result = await response.Content.ReadAsStringAsync();

            return result;
        }

        class Repdt
        {
            public class Status
            {
                public int PlayerCount { get; set; } = 0;

                public string Version { get; set; } = string.Empty;
            }

            public class Root
            {
                public int Retcode { get; set; }

                public Status? Status { get; set; }
            }

        }


        public static async Task<ServerInfo> GetAsync(string ip)
        {

            var url = $"{Scheme}://{ip}/status/server";
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var r = await HttpGet(url, new Dictionary<string, string>());
            if(r is null)
                return new ServerInfo();

            sw.Stop();
            Repdt.Root? dt;
            try
            {
                dt = JsonConvert.DeserializeObject<Repdt.Root>(r);
                if (dt is null)
                    return new ServerInfo();

            }
            catch
            {
                dt = new Repdt.Root();
            }

            ServerInfo serverInfo = new ServerInfo();
            if (dt.Status == null)
            {
                return new ServerInfo();
            }
            serverInfo.ver = dt.Status.Version;
            serverInfo.players = dt.Status.PlayerCount.ToString();
            serverInfo.timeout = sw.ElapsedMilliseconds.ToString();


            return serverInfo;



        }

        internal static async Task<List<AnnounceMentItem>> GetAnnounceAsync(string ip)
        {
            var url = $"{Scheme}://{ip}/glannouncement/list";
            var r = await HttpGet(url, new Dictionary<string, string>());
            if (r is null)
                return new List<AnnounceMentItem>();

            List<AnnounceMentItem>? ret;
            try
            {
                ret = JsonConvert.DeserializeObject<List<AnnounceMentItem>>(r);
            }
            catch (Exception)
            {
                ret = new List<AnnounceMentItem>();
            }

            return ret ?? new List<AnnounceMentItem>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetLibrary.SimpleHttpNet;

namespace TestHttpNet
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleHttpNet net = new SimpleHttpNet();

            HttpQuery http_query_get = new HttpQuery("https://partner.steam-api.com/ISteamMicroTxn/GetUserInfo/v2");
            http_query_get.time_out = 10000;
            http_query_get.AddParam("format", "json");
            http_query_get.AddParam("key", "02FF47D90BB148544A207A3A7A81F546");
            http_query_get.AddParam("appid", "550650");
            http_query_get.AddParam("steamid", "76561198026095706");
   
            string response = string.Empty;
            net.Request(HttpNetRequestType.GET, http_query_get, out response);
            Console.WriteLine(response);

            HttpQuery http_query_post = new HttpQuery("https://httpbin.org/post");
            http_query_post.time_out = 10000;
            http_query_post.AddParam("id", "101");
            http_query_post.AddParam("name", "Alex");

            net.Request(HttpNetRequestType.POST, http_query_post, out response);
            Console.WriteLine(response);

            net.FileDownload("http://httpbin.org/image/png", "image.png");

            return;
        }
    }
}

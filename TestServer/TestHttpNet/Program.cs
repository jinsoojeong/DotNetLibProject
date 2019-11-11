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

            // Get 방식 http call
            HttpQuery http_query_get = new HttpQuery("url");
            http_query_get.time_out = 10000;
            http_query_get.AddParam("format", "json");
            http_query_get.AddParam("param1", "");
            http_query_get.AddParam("param2", "");
   
            string response = string.Empty;
            net.Request(HttpNetRequestType.GET, http_query_get, out response);
            Console.WriteLine(response);

            // Post 방식 Http call
            HttpQuery http_query_post = new HttpQuery("url");
            http_query_post.time_out = 10000;
            http_query_post.AddParam("param1", "");
            http_query_post.AddParam("param2", "");

            net.Request(HttpNetRequestType.POST, http_query_post, out response);
            Console.WriteLine(response);

            // 이미지 다운 예제
            net.FileDownload("url", "image.png");

            return;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace NetLibrary.SimpleHttpNet
{
    public enum HttpNetRequestType
    {
        GET,
        POST
    }

    public class HttpQuery
    {
        List<KeyValuePair<string, string>> params_;
        public int time_out { get; set; } = 10000;
        public string url { get; private set; }

        public HttpQuery(string url)
        {
            this.url = url;
            params_ = new List<KeyValuePair<string, string>>();
        }
        
        public void AddParam(string key, string value)
        {
            params_.Add(new KeyValuePair<string, string>(key, value));
        }

        public int GetCountParam()
        {
            return params_.Count();
        }

        internal IEnumerable<KeyValuePair<string, string>> IteratorParam()
        {
            foreach (KeyValuePair<string, string> itor in params_)
            {
                yield return itor;
            }
        }

        internal string GenerateGetParam()
        {
            string data = "?";

            int i = 0;
            foreach (var param in params_)
            {
                data += param.Key + "=" + param.Value;

                if (i != params_.Count - 1)
                    data += "&";

                ++i;
            }

            return data;
        }

        internal string GeneratePostParam()
        {
            string data = "{ ";

            int i = 0;
            foreach (var itor in params_)
            {
                data += " \" "+ itor.Key + "\": \" " + itor.Value + "\" ";

                if (i != params_.Count - 1)
                    data += ", ";

                ++i;
            }

            data += " }";

            return data;
        }
    }

    public class SimpleHttpNet
    {
        public SimpleHttpNet() {}

        public int Request(HttpNetRequestType type, HttpQuery http_query, out string response)
        {
            response = string.Empty;
            HttpWebRequest request = null;
            if (type == HttpNetRequestType.GET)
            {
                string request_url = http_query.url + "/";
                if (http_query.GetCountParam() != 0)
                {
                    request_url += http_query.GenerateGetParam();
                }

                request = (HttpWebRequest)WebRequest.Create(request_url);
                request.Method = "GET";
                request.Headers.Add("Authorization", "BASIC SGVsbG8=");
            }
            else if (type == HttpNetRequestType.POST)
            {
                request = (HttpWebRequest)WebRequest.Create(http_query.url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = http_query.time_out;

                // POST할 데이타를 Request Stream에 쓴다
                if (http_query.GetCountParam() != 0)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(http_query.GeneratePostParam());
                    request.ContentLength = bytes.Length; // 바이트수 지정
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }

            if (request == null)
                return -1;

            if (http_query.time_out == 0)
                request.Timeout = System.Threading.Timeout.Infinite;
            else
                request.Timeout = http_query.time_out;

            // Response 처리
            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            {
                HttpStatusCode status = resp.StatusCode;
                if (resp.StatusCode != HttpStatusCode.OK)
                    return (int)resp.StatusCode;

                Stream respStream = resp.GetResponseStream();
                using (StreamReader sr = new StreamReader(respStream))
                {
                    response = sr.ReadToEnd();
                }
            }

            return (int)HttpStatusCode.OK;
        }

        public int FileDownload(string url, string filename)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            // Response 바이너리 데이타 처리. 이미지 파일로 저장.                        
            using (WebResponse resp = request.GetResponse())
            {
                var buff = new byte[1024];
                int pos = 0;
                int count;

                using (Stream stream = resp.GetResponseStream())
                {
                    using (var fs = new FileStream(filename, FileMode.Create))
                    {
                        do
                        {
                            count = stream.Read(buff, pos, buff.Length);
                            fs.Write(buff, 0, count);
                        } while (count > 0);
                    }
                }
            }

            return (int)HttpStatusCode.OK;
        }
    }
}


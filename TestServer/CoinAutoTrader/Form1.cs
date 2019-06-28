using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using Newtonsoft.Json.Linq;
using NetLibrary.SimpleHttpNet;

namespace CoinAutoTrader
{
    using CoinAutoTrader.Source;

    public partial class Form1 : Form
    {
        List<string> markets;
        SimpleHttpNet net;

        public Form1()
        {
            InitializeComponent();

            markets = new List<string>();
            net = new SimpleHttpNet();

            SelectMarket();
        }
                
        private void button2_Click(object sender, EventArgs e)
        {
            Ticker ticker = SelectTicker("krw-btc");
            BaseCandle = 
        }

        // 종목조회
        private void SelectMarket()
        {
            HttpQuery http_query_get = new HttpQuery("https://api.upbit.com/v1/market/all");
            http_query_get.time_out = 10000;

            string response = string.Empty;
            net.Request(HttpNetRequestType.GET, http_query_get, out response);
            Console.WriteLine(response);

            markets.Clear();

            JArray ja = JArray.Parse(response);
            foreach (JObject jo in ja)
            {
                markets.Add(jo["market"].ToString());
                DataGrid1.Rows.Add(jo["korean_name"], jo["market"], jo["english_name"]);
            }
        }

        // ticker
        private Ticker SelectTicker(string market)
        {
            HttpQuery http_query_get = new HttpQuery("https://api.upbit.com/v1/ticker");
            http_query_get.time_out = 10000;
            http_query_get.AddParam("markets", market);

            string response = string.Empty;
            net.Request(HttpNetRequestType.GET, http_query_get, out response);
            Console.WriteLine(response);

            JArray ja = JArray.Parse(response);
            JToken jo = ja.First();

            Ticker ticker = new Ticker();
            ticker.market = jo["market"].ToString();
            ticker.trade_date = jo["trade_date"].ToString();
            ticker.trade_time = jo["trade_time"].ToString();
            ticker.trade_date_kst = jo["trade_date_kst"].ToString();
            ticker.trade_time_kst = jo["trade_time_kst"].ToString();
            ticker.trade_timestamp = Convert.ToInt64(jo["trade_timestamp"].ToString());
            ticker.opening_price = Convert.ToInt64(jo["opening_price"].ToString());
            ticker.high_price = Convert.ToInt64(jo["high_price"].ToString());
            ticker.low_price = Convert.ToInt64(jo["low_price"].ToString());
            ticker.trade_price = Convert.ToInt64(jo["trade_price"].ToString());
            ticker.prev_closing_price = Convert.ToInt64(jo["prev_closing_price"].ToString());
            ticker.change = jo["change"].ToString();
            ticker.change_price = Convert.ToInt64(jo["change_price"].ToString());
            ticker.change_rate = Convert.ToDouble(jo["change_rate"].ToString());
            ticker.signed_change_price = Convert.ToInt64(jo["signed_change_price"].ToString());
            ticker.signed_change_rate = Convert.ToDouble(jo["signed_change_rate"].ToString());
            ticker.trade_volume = Convert.ToDouble(jo["trade_volume"].ToString());
            ticker.acc_trade_price = Convert.ToDouble(jo["acc_trade_price"].ToString());
            ticker.acc_trade_price_24h = Convert.ToDouble(jo["acc_trade_price_24h"].ToString());
            ticker.acc_trade_volume = Convert.ToDouble(jo["acc_trade_volume"].ToString());
            ticker.acc_trade_volume_24h = Convert.ToDouble(jo["acc_trade_volume_24h"].ToString());
            ticker.highest_52_week_price = Convert.ToInt64(jo["highest_52_week_price"].ToString());
            ticker.highest_52_week_date = jo["highest_52_week_date"].ToString();
            ticker.lowest_52_week_price = Convert.ToInt64(jo["lowest_52_week_price"].ToString());
            ticker.lowest_52_week_date = jo["lowest_52_week_date"].ToString();
            ticker.timestamp = Convert.ToInt64(jo["timestamp"].ToString());

            return ticker;
        }

        private void SelectCandle(CandleType type, string market, string to = null, Int32? count = null)
        {
            string url = "https://api.upbit.com/v1/candles";
            switch (type)
            {
                case CandleType.Min:
                    url += "/minutes/1";
                    break;
                case CandleType.Day:
                    url += "/days";
                    break;
                case CandleType.Week:
                    url += "/weeks";
                    break;
                case CandleType.Month:
                    url += "/months";
                    break;
            }

            HttpQuery http_query_get = new HttpQuery(url);
            http_query_get.time_out = 10000;
            http_query_get.AddParam("markets", market);

            if (to != null)
                http_query_get.AddParam("to", to);

            if (count != null)
                http_query_get.AddParam("count", count.ToString());

            string response = string.Empty;
            net.Request(HttpNetRequestType.GET, http_query_get, out response);

            BaseCandle candle = null;
            switch (type)
            {
                case CandleType.Min: candle = new MinCandle(); break;
                case CandleType.Day: candle = new DayCandle(); break;
                case CandleType.Week: candle = new WeekCandle(); break;
                case CandleType.Month: candle = new MonthCandle(); break;
            }


        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;
using NetLibrary.SimpleHttpNet;

namespace CoinAutoTrader.Source
{
    class QuotationApi
    {
        SimpleHttpNet net;

        public QuotationApi()
        {
            net = new SimpleHttpNet();
        }

        // 종목조회
        public List<string> SelectMarket()
        {
            List<string> markets = new List<string>();

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
            }

            return markets;
        }

        // ticker
        public Ticker SelectTicker(string market)
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

        public MinCandle SelectCandleMin(string market, int unit, string to = null, Int32? count = null)
        {
            string url = "https://api.upbit.com/v1/candles/minutes/" + unit.ToString();

            HttpQuery http_query_get = new HttpQuery(url);
            http_query_get.time_out = 10000;
            http_query_get.AddParam("market", market);
            if (to != null) http_query_get.AddParam("to", to);
            if (count != null) http_query_get.AddParam("count", count.ToString());

            string response = string.Empty;
            net.Request(HttpNetRequestType.GET, http_query_get, out response);

            JArray ja = JArray.Parse(response);
            JToken jt = ja.First();

            MinCandle candle = new MinCandle();
            candle.market = jt["market"].ToString();
            candle.candle_date_time_utc = jt["candle_date_time_utc"].ToString();
            candle.candle_date_time_kst = jt["candle_date_time_kst"].ToString();
            candle.opening_price = Convert.ToInt64(jt["opening_price"].ToString());
            candle.high_price = Convert.ToInt64(jt["high_price"].ToString());
            candle.low_price = Convert.ToInt64(jt["low_price"].ToString());
            candle.trade_price = Convert.ToInt64(jt["trade_price"].ToString());
            candle.timestamp = Convert.ToInt64(jt["timestamp"].ToString());
            candle.candle_acc_trade_price = Convert.ToDouble(jt["candle_acc_trade_price"].ToString());
            candle.candle_acc_trade_volume = Convert.ToDouble(jt["candle_acc_trade_volume"].ToString());

            candle.unit = Convert.ToInt64(jt["unit"].ToString());

            return candle;
        }

        public DayCandle SelectCandleDay(string market, string to = null, Int32? count = null)
        {
            string url = "https://api.upbit.com/v1/candles/days";

            HttpQuery http_query_get = new HttpQuery(url);
            http_query_get.time_out = 10000;
            http_query_get.AddParam("market", market);
            if (to != null) http_query_get.AddParam("to", to);
            if (count != null) http_query_get.AddParam("count", count.ToString());

            string response = string.Empty;
            net.Request(HttpNetRequestType.GET, http_query_get, out response);

            JArray ja = JArray.Parse(response);
            JToken jt = ja.First();

            DayCandle candle = new DayCandle();
            candle.market = jt["market"].ToString();
            candle.candle_date_time_utc = jt["candle_date_time_utc"].ToString();
            candle.candle_date_time_kst = jt["candle_date_time_kst"].ToString();
            candle.opening_price = Convert.ToInt64(jt["opening_price"].ToString());
            candle.high_price = Convert.ToInt64(jt["high_price"].ToString());
            candle.low_price = Convert.ToInt64(jt["low_price"].ToString());
            candle.trade_price = Convert.ToInt64(jt["trade_price"].ToString());
            candle.timestamp = Convert.ToInt64(jt["timestamp"].ToString());
            candle.candle_acc_trade_price = Convert.ToDouble(jt["candle_acc_trade_price"].ToString());
            candle.candle_acc_trade_volume = Convert.ToDouble(jt["candle_acc_trade_volume"].ToString());

            candle.prev_closing_price = Convert.ToInt64(jt["prev_closing_price"].ToString());
            candle.change_price = Convert.ToInt64(jt["change_price"].ToString());
            candle.change_rate = Convert.ToDouble(jt["change_rate"].ToString());

            return candle;
        }

        public WeekCandle SelectCandleWeek(string market, string to = null, Int32? count = null)
        {
            string url = "https://api.upbit.com/v1/candles/weeks";

            HttpQuery http_query_get = new HttpQuery(url);
            http_query_get.time_out = 10000;
            http_query_get.AddParam("market", market);
            if (to != null) http_query_get.AddParam("to", to);
            if (count != null) http_query_get.AddParam("count", count.ToString());

            string response = string.Empty;
            net.Request(HttpNetRequestType.GET, http_query_get, out response);

            JArray ja = JArray.Parse(response);
            JToken jt = ja.First();

            WeekCandle candle = new WeekCandle();
            candle.market = jt["market"].ToString();
            candle.candle_date_time_utc = jt["candle_date_time_utc"].ToString();
            candle.candle_date_time_kst = jt["candle_date_time_kst"].ToString();
            candle.opening_price = Convert.ToInt64(jt["opening_price"].ToString());
            candle.high_price = Convert.ToInt64(jt["high_price"].ToString());
            candle.low_price = Convert.ToInt64(jt["low_price"].ToString());
            candle.trade_price = Convert.ToInt64(jt["trade_price"].ToString());
            candle.timestamp = Convert.ToInt64(jt["timestamp"].ToString());
            candle.candle_acc_trade_price = Convert.ToDouble(jt["candle_acc_trade_price"].ToString());
            candle.candle_acc_trade_volume = Convert.ToDouble(jt["candle_acc_trade_volume"].ToString());

            candle.first_day_of_period = jt["first_day_of_period"].ToString();

            return candle;
        }

        public MonthCandle SelectCandleMonth(string market, string to = null, Int32? count = null)
        {
            string url = "https://api.upbit.com/v1/candles/months";

            HttpQuery http_query_get = new HttpQuery(url);
            http_query_get.time_out = 10000;
            http_query_get.AddParam("market", market);
            if (to != null) http_query_get.AddParam("to", to);
            if (count != null) http_query_get.AddParam("count", count.ToString());

            string response = string.Empty;
            net.Request(HttpNetRequestType.GET, http_query_get, out response);

            JArray ja = JArray.Parse(response);
            JToken jt = ja.First();

            MonthCandle candle = new MonthCandle();
            candle.market = jt["market"].ToString();
            candle.candle_date_time_utc = jt["candle_date_time_utc"].ToString();
            candle.candle_date_time_kst = jt["candle_date_time_kst"].ToString();
            candle.opening_price = Convert.ToInt64(jt["opening_price"].ToString());
            candle.high_price = Convert.ToInt64(jt["high_price"].ToString());
            candle.low_price = Convert.ToInt64(jt["low_price"].ToString());
            candle.trade_price = Convert.ToInt64(jt["trade_price"].ToString());
            candle.timestamp = Convert.ToInt64(jt["timestamp"].ToString());
            candle.candle_acc_trade_price = Convert.ToDouble(jt["candle_acc_trade_price"].ToString());
            candle.candle_acc_trade_volume = Convert.ToDouble(jt["candle_acc_trade_volume"].ToString());

            candle.first_day_of_period = jt["first_day_of_period"].ToString();

            return candle;
        }

        //Trades
        public Trades SelectTrades(string market, string to = null, Int32? count = null, string cursor = null)
        {
            string url = "https://api.upbit.com/v1/trades/ticks";

            HttpQuery http_query_get = new HttpQuery(url);
            http_query_get.time_out = 10000;
            http_query_get.AddParam("market", market);
            if (to != null) http_query_get.AddParam("to", to);
            if (count != null) http_query_get.AddParam("count", count.ToString());
            if (cursor != null) http_query_get.AddParam("cursor", cursor.ToString());

            string response = string.Empty;
            net.Request(HttpNetRequestType.GET, http_query_get, out response);

            JArray ja = JArray.Parse(response);
            JToken jt = ja.First();

            Trades trades = new Trades();
            trades.market = jt["market"].ToString();
            trades.trade_date_utc = jt["trade_date_utc"].ToString();
            trades.trade_time_utc = jt["trade_time_utc"].ToString();
            trades.timestamp = Convert.ToInt64(jt["timestamp"].ToString());
            trades.trade_price = Convert.ToInt64(jt["trade_price"].ToString());
            trades.trade_volume = Convert.ToDouble(jt["trade_volume"].ToString());
            trades.prev_closing_price = Convert.ToInt64(jt["prev_closing_price"].ToString());
            trades.change_price = Convert.ToInt64(jt["change_price"].ToString());
            trades.ask_bid = jt["ask_bid"].ToString();

            return trades;
        }

        //Orderbook
        public OrderBook SelectOrderbook(string market, string to = null, Int32? count = null, string cursor = null)
        {
            string url = "https://api.upbit.com/v1/orderbook";

            HttpQuery http_query_get = new HttpQuery(url);
            http_query_get.time_out = 10000;
            http_query_get.AddParam("market", market);
            if (to != null) http_query_get.AddParam("to", to);
            if (count != null) http_query_get.AddParam("count", count.ToString());
            if (cursor != null) http_query_get.AddParam("cursor", cursor.ToString());

            string response = string.Empty;
            net.Request(HttpNetRequestType.GET, http_query_get, out response);

            JArray ja = JArray.Parse(response);
            JToken jt = ja.First();

            OrderBook order_book = new OrderBook();
            order_book.market = jt["market"].ToString();
            order_book.timestamp = Convert.ToInt64(jt["timestamp"].ToString());
            order_book.total_ask_size = Convert.ToInt64(jt["total_ask_size"].ToString());
            order_book.total_bid_size = Convert.ToInt64(jt["total_bid_size"].ToString());

            if (jt["orderbook_units"].Count() > 0)
            {
                Unit unit = new Unit();

                unit.ask_price = Convert.ToInt64(jt["ask_price"].ToString());
                unit.bid_price = Convert.ToInt64(jt["bid_price"].ToString());
                unit.ask_size = Convert.ToDouble(jt["ask_size"].ToString());
                unit.bid_size = Convert.ToDouble(jt["bid_size"].ToString());

                order_book.units.Add(unit);
            }

            return order_book;
        }
    }
}

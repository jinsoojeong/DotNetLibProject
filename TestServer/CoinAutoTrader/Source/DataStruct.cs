using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinAutoTrader.Source
{
    public struct Ticker
    {
        public string market;
        public string trade_date;
        public string trade_time;
        public string trade_date_kst;
        public string trade_time_kst;
        public Int64 trade_timestamp;
        public Int64 opening_price;
        public Int64 high_price;
        public Int64 low_price;
        public Int64 trade_price;
        public Int64 prev_closing_price;
        public string change;
        public Int64 change_price;
        public double change_rate;
        public Int64 signed_change_price;
        public double signed_change_rate;
        public double trade_volume;
        public double acc_trade_price;
        public double acc_trade_price_24h;
        public double acc_trade_volume;
        public double acc_trade_volume_24h;
        public Int64 highest_52_week_price;
        public string highest_52_week_date;
        public Int64 lowest_52_week_price;
        public string lowest_52_week_date;
        public Int64 timestamp;
    }

    public class BaseCandle
    {
        public string market;
        public string candle_date_time_utc;
        public string candle_date_time_kst;
        public Int64 opening_price;
        public Int64 high_price;
        public Int64 low_price;
        public Int64 trade_price;
        public Int64 timestamp;
        public double candle_acc_trade_price;
        public double candle_acc_trade_volume;
    }

    public class MinCandle : BaseCandle
    {
        public Int64 unit;
    }

    public class DayCandle : BaseCandle
    {
        public Int64 prev_closing_price;
        public Int64 change_price;
        public double change_rate;
    }

    public class WeekCandle : BaseCandle
    {
        public string first_day_of_period;
    }

    public class MonthCandle : BaseCandle
    {
        public string first_day_of_period;
    }

    public class Trades
    {
        public string market;
        public string trade_date_utc;
        public string trade_time_utc;
        public Int64 timestamp;
        public Int64 trade_price;
        public double trade_volume;
        public Int64 prev_closing_price;
        public Int64 chane_price;
        public string ask_bid;
    }

    public class Unit
    {
        public Int64 ask_price;
        public Int64 bid_price;
        public double ask_size;
        public double bid_size;
    }

    public class OrderBook
    {
        public OrderBook()
        {
            units = new List<Unit>();
        }

        public string market;
        public Int64 timestamp;
        public double total_ask_size;
        public double total_bid_size;
        public List<Unit> units;
    }
}

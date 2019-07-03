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
        QuotationApi quotation_api;
        ExchangeApi exchange_api;

        public Form1()
        {
            InitializeComponent();

            quotation_api = new QuotationApi();
            exchange_api = new ExchangeApi();

            List<string> markets = quotation_api.SelectMarket();
            foreach (string name in markets)
            {
                DataGrid1.Rows.Add(name, "", "");
            }
        }
                
        private void button2_Click(object sender, EventArgs e)
        {
            Ticker ticker = quotation_api.SelectTicker("krw-btc");
            MinCandle min_candle = quotation_api.SelectCandleMin("krw-btc", 1);
            DayCandle day_candle = quotation_api.SelectCandleDay("krw-btc");
            WeekCandle week_candle = quotation_api.SelectCandleWeek("krw-btc");
            MonthCandle month_candle = quotation_api.SelectCandleMonth("krw-btc");
            Trades trades = quotation_api.SelectTrades("krw-btc");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            exchange_api.SelectAccounts();
        }
    }
}

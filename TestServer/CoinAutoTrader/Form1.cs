﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Collections.Concurrent;

using Newtonsoft.Json.Linq;
using NetLibrary.SimpleHttpNet;

namespace CoinAutoTrader
{
    using CoinAutoTrader.Source;

    public partial class Form1 : Form
    {
        object ticker_lock;

        Ticker current_ticker_;

        ConcurrentQueue<string> logs;
        QuotationApi quotation_api;
        ExchangeApi exchange_api;

        int ticker_interval = 1000;
        bool shutdown = false;
        Thread ticker_thread = null;

        string market = "krw-btc";

        public Form1()
        {
            InitializeComponent();

            ticker_lock = new object();

            logs = new ConcurrentQueue<string>();
            quotation_api = new QuotationApi();
            exchange_api = new ExchangeApi();

            //List<string> markets = quotation_api.SelectMarket();

            timer1.Start();
        }

        private void TickerThread()
        {
            while (!shutdown)
            {
                Ticker ticker = quotation_api.SelectTicker("krw-btc");
                
                if (ticker.timestamp != current_ticker_.timestamp)
                {
                    lock (ticker_lock)
                    {
                        current_ticker_ = ticker;
                    }

                    string s = "trade_price : " + current_ticker_.trade_price.ToString() + 
                        " opening_price : " + current_ticker_.opening_price.ToString() + 
                        " high_price : " + current_ticker_.high_price + 
                        " low_price : " + current_ticker_.low_price + 
                        " time : " + current_ticker_.timestamp;

                    Log(s);
                }

                Thread.Sleep(ticker_interval);
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

        private void button3_Click(object sender, EventArgs e)
        {
            shutdown = false;
            ticker_thread = new Thread(TickerThread);
            ticker_thread.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            shutdown = true;
            ticker_thread.Join();
        }

        private void ReportLog(string log)
        {
            textBox1.AppendText(log + Environment.NewLine);
        }

        private void Log(string txt)
        {
            logs.Enqueue("[" + DateTime.Now + "] " + txt);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string log;
            while (logs.TryDequeue(out log))
            {
                ReportLog(log);
            }
        }
    }
}

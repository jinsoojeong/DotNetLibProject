using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NetLibrary.SimpleHttpNet;
using JWT;
using System.Security.Cryptography;

using System.IdentityModel.Tokens.Jwt;

namespace CoinAutoTrader.Source
{
    class ExchangeApi
    {
        SimpleHttpNet net;

        private string access_key = "pPoBmx2GHFF50xFKqtKYObS7mF3A91C0MBkASgDC";
        private string secret_key = "98rJ5UdFxSbWtKYfvbhkzhUYUxtLx6tZxDGuLoQq";

        public ExchangeApi()
        {
            net = new SimpleHttpNet();
        }

        private string GenerateAuthorizationToken(Dictionary<string, string> param = null)
        {
            string token;
            JwtPayload pay_load = new JwtPayload();
            pay_load.Add("access_key", access_key);
            pay_load.Add("nonce", Guid.NewGuid().ToString());

            if (param != null)
            {
                // parameter가 Dictionary<string, string> 일 경우
                StringBuilder builder = new StringBuilder();
                foreach (KeyValuePair<string, string> pair in param)
                {
                    builder.Append(pair.Key).Append("=").Append(pair.Value).Append("&");
                }

                string queryString = builder.ToString().TrimEnd('&');

                SHA512 sha512 = SHA512.Create();
                byte[] queryHashByteArray = sha512.ComputeHash(Encoding.UTF8.GetBytes(queryString));
                string queryHash = BitConverter.ToString(queryHashByteArray).Replace("-", "").ToLower();

                pay_load.Add("query_hash", queryHash);
                pay_load.Add("query_hash_alg", "SHA512");
            }

            byte[] keyBytes = Encoding.Default.GetBytes(secret_key);
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes);
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, "HS256");
            var header = new JwtHeader(credentials);
            var secToken = new JwtSecurityToken(header, pay_load);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(secToken);
            token = "Bearer " + jwtToken;

            return token;
        }

        public List<Account> SelectAccounts()
        {
            HttpQuery http_query_get = new HttpQuery("https://api.upbit.com/v1/accounts");
            http_query_get.AddHeader("Authorization", GenerateAuthorizationToken());

            string response = string.Empty;
            net.Request(HttpNetRequestType.GET, http_query_get, out response);

            JArray ja = JArray.Parse(response);

            if (ja.Count == 0)
                return null;

            List<Account> accounts = new List<Account>();

            foreach (JToken jt in ja)
            {
                Account account = new Account();

                account.currency = jt["currency"].ToString();
                account.balance = jt["balance"].ToString();
                account.locked = jt["locked"].ToString();
                account.avg_buy_price = jt["avg_buy_price"].ToString();
                account.avg_buy_price_modified = Convert.ToBoolean(jt["avg_buy_price_modified"].ToString());
                account.unit_currency = jt["unit_currency"].ToString();
                account.avg_krw_buy_price = jt["avg_krw_buy_price"].ToString();
                account.modified = Convert.ToBoolean(jt["modified"].ToString());
                accounts.Add(account);
            }

            return accounts;
        }

        public Chance SelectChance(string market)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("market", market);

            HttpQuery http_query_get = new HttpQuery("https://api.upbit.com/v1/orders/chance");
            http_query_get.AddHeader("Authorization", GenerateAuthorizationToken(param));

            string response = string.Empty;
            net.Request(HttpNetRequestType.GET, http_query_get, out response);

            JArray ja = JArray.Parse(response);
            JToken jt = ja.First();

            Chance chance = new Chance();

            chance.bid_fee = jt["bid_fee"].ToString();
            chance.ask_fee = jt["ask_fee"].ToString();

            JToken jt_market = jt["market"];
            chance.market.id = jt_market["id"].ToString();
            chance.market.name = jt_market["name"].ToString();

            foreach (string type in jt_market["order_types"])
            {
                chance.market.order_types.Add(type.ToString());
            }

            foreach (string side in jt_market["order_sides"])
            {
                chance.market.order_sides.Add(side.ToString());
            }

            //chance.market.bid.currency;
            //chance.market.bid.price_unit;
            //chance.market.bid.min_total;
            //  "bid": {
            //    "currency": "KRW",
            //    "price_unit": null,
            //    "min_total": 1000
            //  },
            //  "ask": {
            //    "currency": "BTC",
            //    "price_unit": null,
            //    "min_total": 1000
            //  },
            //  "max_total": "100000000.0",
            //  "state": "active",
            //},
            //"bid_account": {
            //  "currency": "KRW",
            //  "balance": "0.0",
            //  "locked": "0.0",
            //  "avg_buy_price": "0",
            //  "avg_buy_price_modified": false,
            //  "unit_currency": "KRW",
            //  "avg_krw_buy_price": "0",
            //  "modified": false
            //},
            //"ask_account": {
            //  "currency": "BTC",
            //  "balance": "10.0",
            //  "locked": "0.0",
            //  "avg_buy_price": "8042000",
            //  "avg_buy_price_modified": false,
            //  "unit_currency": "KRW",
            //  "avg_krw_buy_price": "8042000",
            //  "modified": false,
            //}

            return chance;
        }
    }
}

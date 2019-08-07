using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinAutoTrader.Source
{
    // 보유 자산 리스트 GET
    public class Account
    {
        public string currency;
        public string balance;
        public string locked;
        public string avg_buy_price;
        public bool avg_buy_price_modified;
        public string unit_currency;
        public string avg_krw_buy_price;
        public bool modified;
    }

    // 마켓별 주문 가능 정보를 확인 GET
    public class Chance
    {
        public class Market
        {
            public class Bid
            {
                public string currency;
                public string price_unit;
                public Int64 min_total;
            }

            public class Ask
            {
                public string currency;
                public string price_unit;
                public Int64 min_total;
            }

            public string id;
            public string name;
            public List<string> order_types;
            public List<string> order_sides;
            public Bid bid;
            public Ask ask;
            public string max_total;
            public string state;
        }

        public class BidAccount
        {
            public string currency;
            public string balance;
            public string locked;
            public string avg_buy_price;
            public bool avg_buy_price_modified;
            public string unit_currency;
            public string avg_krw_buy_price;
            public bool modified;
        }

        public class AskAccount
        {
            public string currency;
            public string balance;
            public string locked;
            public string avg_buy_price;
            public bool avg_buy_price_modified;
            public string unit_currency;
            public string avg_krw_buy_price;
            public bool modified;
        }

        public Chance()
        {
            market = new Market();
            bid_account = new BidAccount();
            ask_account = new AskAccount();
        }

        public string bid_fee;
        public string ask_fee;
        public Market market;
        public BidAccount bid_account;
        public AskAccount ask_account;
    }

    //주문 UUID 를 통해 개별 주문건을 조회 GET
    public class Order
    {
        public class Trade
        {
            public string market; // 마켓의 유일 키
            public string uuid; // 체결의 고유 아이디
            public string price; // 체결 가격
            public string volume; // 체결 양
            public string funds; // 체결된 총 가격
            public string side; // 체결 종류
        }

        public Order()
        {
            trades = new List<Trade>();
        }

        string uuid; // 주문의 고유 아이디
        string side; // 주문 종류
        string ord_type; // 주문 방식
        string price; // 주문 당시 화폐 가격
        string state; // 주문 상태
        string market; // 마켓의 유일키
        string created_at; // 주문 생성 시간
        string volume; // 사용자가 입력한 주문 양
        string remaining_volume; // 체결 후 남은 주문 양
        string reserved_fee; // 수수료로 예약된 비용
        string remaininge_fee; // 남은 수수료
        string locked; // 거래에 사용중인 비용
        string executed_volume; // 쳬결된 양
        Int64 trade_count; // 해당 주문에 걸린 체결 수
        List<Trade> trades; // 체결s
    }

    // 주문 리스트를 조회 GET
    public class Orders
    {
        string uuid; // 주문의 고유 아이디
        string side; // 주문 종류
        string ord_type; // 주문 방식
        string price; // 주문 당시 화폐 가격
        string state; // 주문 상태
        string market; // 마켓의 유일키
        string created_at; // 주문 생성 시간
        string volume; // 사용자가 입력한 주문 양
        string remaining_volume; // 체결 후 남은 주문 양
        string reserved_fee; // 수수료로 예약된 비용
        string remaininge_fee; // 남은 수수료
        string locked; // 거래에 사용중인 비용
        string executed_volume; // 쳬결된 양
        Int64 trade_count; // 해당 주문에 걸린 체결 수
    }

    
}

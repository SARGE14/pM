using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Net;
using System.Numerics;


namespace pM
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        readonly INotificationManager notificationManager;
        readonly WebClient webClient;
        public string url;
        public string coin;
        public string exchange;
        public string imageName;
        public string currencyPair;
        public string priceBidAsk;
        public string errorExh;
        public string alertPair;
        public bool firstUpDown;
        public bool alive;
        public double oldPrice;
        public double newPrice;
        public double percentUpDown = 1.09f;
        public int timerload = 15;
        public string toastMesLine1;
        public string toastMesLine2;
        public float priceAlertBuyLive;
        public float priceAlertSellHit;
        public float priceAlertSellLive;
        public float priceAlertBuyHit;
        RootObject json;
        public class RootObject
        {
            public double best_bid { get; set; }
            public double best_ask { get; set; }
            public double ask { get; set; }
            public double bid { get; set; }
            public Ticker ticker { get; set; }
        }
        public class Ticker
        {
            public double Buy { get; set; }
            public double Sell { get; set; }
        }
        public MainPage()
        {
            InitializeComponent();
            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                //  ShowNotification(evtData.Title, evtData.Message);
            };
            webClient = new WebClient();
            messageText.Text = "";
            firstUpDown = true;
            errorText.Text = "Соединение...";
            PriceGet();
        }
        private async void UpdateTimer()
        {
            while (updateSwitch.IsToggled)
            {
                PriceGet();
                //  errorText.Text = notificationNumber++.ToString();
                await Task.Delay(30000);
            }
        }

        private void UpdateSwitchToggled(object sender, EventArgs e)
        {
            if (updateSwitch.IsToggled)
                UpdateTimer();
        }

        /*   void ShowNotification(string title, string message)
           {
               Device.BeginInvokeOnMainThread(() =>
               {
                   //   messageText.Text = $"{title}\n{message}";
               });
           }*/
        private void WebTest()
        {
            using (WebClient wc = webClient)
            {
                string webPage = null;

                try
                {
                    webPage = wc.DownloadString(@url);

                    errorExh = "";
                    errorText.Text = "";
                    json = null;
                    try
                    {
                        json = JsonConvert.DeserializeObject<RootObject>(webPage);
                    }
                    catch
                    {
                        errorText.Text = "API " + exchange + " недоступно";
                        url = null;
                        json = null;
                    }
                }
                catch (WebException)
                {
                    errorExh = exchange;
                    errorText.Text = "API " + exchange + " недоступно";
                    url = null;
                }
            }
        }
        private void OnButtonClicked(object sender, EventArgs e)
        {
            PriceGet();
            /* if (updateSwitch.IsToggled)
                 UpdateTimer();*/
        }

        private void PriceGet()
        {
            string urlExchange;
            messageText.Text = "";
            /*Livecoin*/
            exchange = "Livecoin";
            urlExchange = "https://api.livecoin.net/exchange/ticker?currencyPair=";
            currencyPair = "BTC/USD";
            url = urlExchange + currencyPair;
            WebTest();
            if (errorExh != "Livecoin" && json != null)
            {
                currencyPair = "BTC/USD";
                url = urlExchange + currencyPair;
                WebTest();

                oldPrice = double.Parse(priceBtc.Text);
                newPrice = json.best_ask;
                PriceUpDown();
                priceBtc.Text = newPrice.ToString("F");


                currencyPair = "ETH/USD";
                url = urlExchange + currencyPair;
                WebTest();

                oldPrice = double.Parse(priceEth.Text);
                newPrice = json.best_ask;
                PriceUpDown();
                priceEth.Text = newPrice.ToString("F");

                currencyPair = "PLBT/BTC";
                url = urlExchange + currencyPair;
                WebTest();

                priceBidAsk = "продажи";
                alertPair = currencyPair;
                oldPrice = double.Parse(priceBtcSell_livecoin.Text);
                newPrice = json.best_ask;
                PriceUpDown();
                priceBtcSell_livecoin.Text = newPrice.ToString("F8");

                priceBidAsk = "покупки";
                oldPrice = double.Parse(priceBtcBuy_livecoin.Text);
                newPrice = json.best_bid;
                PriceUpDown();
                priceBtcBuy_livecoin.Text = newPrice.ToString("F8");
            }


            /*HitBTC*/
            exchange = "HitBTC";
            urlExchange = "https://api.hitbtc.com/api/2/public/ticker/";
            currencyPair = "PLBTBTC";
            url = urlExchange + currencyPair;
            WebTest();
            if (errorExh != "HitBTC" && json != null)
            {
                alertPair = "PLBT/BTC";
                priceBidAsk = "продажи";
                oldPrice = double.Parse(priceBtcSell_hitbtc.Text);
                newPrice = json.ask;
                PriceUpDown();
                priceBtcSell_hitbtc.Text = newPrice.ToString("F8");

                priceBidAsk = "покупки";
                oldPrice = double.Parse(priceBtcBuy_hitbtc.Text);
                newPrice = json.bid;
                PriceUpDown();
                priceBtcBuy_hitbtc.Text = newPrice.ToString("F8");
            }
            /*YoBit*/
            exchange = "YoBit";
            urlExchange = "https://yobit.net/api/2/";
            currencyPair = "plbt_btc/ticker";
            url = urlExchange + currencyPair;
            WebTest();
            if (errorExh != "YoBit" && json != null)
            {
                oldPrice = double.Parse(priceBtcSell_yobit.Text);
                newPrice = json.ticker.Sell;

                alertPair = "PLBT/BTC";
                priceBidAsk = "продажи";
                PriceUpDown();
                priceBtcSell_yobit.Text = newPrice.ToString("F8");
                oldPrice = double.Parse(priceBtcBuy_yobit.Text);
                newPrice = json.ticker.Buy;

                priceBidAsk = "покупки";
                PriceUpDown();
                priceBtcBuy_yobit.Text = newPrice.ToString("F8");

            }
            firstUpDown = false;
            PriceAlert();
        }

        private void PriceUpDown()
        {
            double calcPriceUp;
            double calcPriceDown;
            if (!firstUpDown)
            {
                calcPriceUp = newPrice / oldPrice;
                calcPriceDown = oldPrice / newPrice;
                if (percentUpDown < calcPriceUp)
                {
                    /// цена вверх
                    calcPriceUp = (calcPriceUp - 1f) * 100f;
                    messageText.Text = "Цена на " + exchange + " вверх на " + calcPriceUp.ToString("F") + "%";
                    toastMesLine1 = "Старая цена: " + oldPrice.ToString("F8");
                    toastMesLine2 = "Новая цена: " + newPrice.ToString("F8");
                    ToastPrice();
                }
                if (percentUpDown < calcPriceDown)
                {
                    /// цена вниз
                    calcPriceDown = (calcPriceDown - 1f) * 100f;
                    messageText.Text = "Цена на " + exchange + " вниз на " + calcPriceDown.ToString("F") + "%";
                    toastMesLine1 = "Старая цена: " + oldPrice.ToString("F8");
                    toastMesLine2 = "Новая цена: " + newPrice.ToString("F8");
                    ToastPrice();
                }
            }
        }
        private void PriceAlert()
        {
            float result;
            priceAlertBuyLive = float.Parse(priceBtcBuy_livecoin.Text);
            priceAlertSellLive = float.Parse(priceBtcSell_livecoin.Text);
            priceAlertBuyHit = float.Parse(priceBtcBuy_hitbtc.Text);
            priceAlertSellHit = float.Parse(priceBtcSell_hitbtc.Text);

            if (priceAlertBuyLive > priceAlertSellHit)
            {
                result = (priceAlertBuyLive / priceAlertSellHit - 1f) * 100f;
                if (result > 3f)

                    PriceAlertsHit(result);
            }
            if (priceAlertBuyHit > priceAlertSellLive)
            {
                result = (priceAlertBuyHit / priceAlertSellLive - 1f) * 100f;
                if (result > 3f)
                    PriceAlertsLive(result);
            }

        }
        private void PriceAlertsHit(double result)
        {
            messageText.Text = "На HitBTC дешевле на " + result.ToString("F") + "%";
            toastMesLine1 = "HitBTC цена: " + priceAlertSellHit.ToString("F8");
            toastMesLine2 = "Livecoin цена: " + priceAlertBuyLive.ToString("F8");
            //      imageName = "Resources/hitbtc.png";
            ToastPrice();
        }
        private void PriceAlertsLive(double result)
        {
            messageText.Text = coin + "На Livecoin дешевле на " + result.ToString("F") + "%";
            toastMesLine1 = "Livecoin цена: " + priceAlertSellLive.ToString("F8");
            toastMesLine2 = "HitBTC цена:   " + priceAlertBuyHit.ToString("F8");
            //  imageName = "Resources/livecoin.png";
            ToastPrice();
        }
        private void ToastPrice()
        {
            string title = messageText.Text;
            string message = $"{toastMesLine1}\n{toastMesLine2}";
            notificationManager.ScheduleNotification(title, message);
        }
    }
}

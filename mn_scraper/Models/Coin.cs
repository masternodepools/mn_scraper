using Newtonsoft.Json;
using System;

namespace MnScraper.Models
{
    public class Coin
    {
        private void mapCoinData(string item, int row, int col)
        {
            switch (col)
            {
                case 0:
                    if (row == 1) Price = item;
                    else PriceInBitCoin = item;
                    break;
                case 1:
                    if (row == 1) Volume = item;
                    else VolumeInBitCoin = item;
                    break;
                case 2:
                    if (row == 1) MarketCap = item;
                    else MarketCapInBitCoin = item;
                    break;
                case 3:
                    if (row == 1) Change = item;
                    break;
                default:
                    Console.WriteLine("default case");
                    break;
            }
        }
        public void SetPriceStats(string[,] coinData, int rows, int cols)
        {
            for (int row = 1; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    mapCoinData(coinData[row, col], row, col);
                }
            }
        }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("priceInBitCoin")]
        public string PriceInBitCoin { get; set; }

        [JsonProperty("volume")]
        public string Volume { get; set; }

        [JsonProperty("volumeInBitCoin")]
        public string VolumeInBitCoin { get; set; }

        [JsonProperty("marketCap")]
        public string MarketCap { get; set; }

        [JsonProperty("marketCapInBitCoin")]
        public string MarketCapInBitCoin { get; set; }

        [JsonProperty("change")]
        public string Change { get; set; }

        [JsonProperty("rewardPerDay")]
        public string RewardPerDay { get; set; }

        [JsonProperty("numerOfMasterNodes")]
        public string NumberOfMasterNodes { get; set; }

        [JsonProperty("requiredAmount")]
        public string RequiredAmount { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
    }
}

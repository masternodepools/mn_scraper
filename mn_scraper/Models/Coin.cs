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

        public string Symbol { get; set; }
        public string Price { get; set; }
        public string PriceInBitCoin { get; set; }
        public string Volume { get; set; }
        public string VolumeInBitCoin { get; set; }
        public string MarketCap { get; set; }
        public string MarketCapInBitCoin { get; set; }
        public string Change { get; set; }
        public string RewardPerDay { get; set; }
        public string NumberOfMasterNodes { get; set; }
        public string RequiredAmount { get; set; }
    }
}
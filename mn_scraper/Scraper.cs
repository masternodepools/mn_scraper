using HtmlAgilityPack;
using MnScraper.Models;
using MnScraper.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MnScraper
{
    public class Scraper
    {

        private readonly string[] _urls;
        private readonly ScraperDbService _scraperDbService;

        private const int ROWS = 3;
        private const int COLS = 4;
        private const int SYMBOL_INDEX = 38;

        public Scraper(string[] urls, AwsSettings awsSettings)
        {
            this._urls = urls;
            this._scraperDbService = new ScraperDbService(awsSettings);
        }

        private async Task<string> GetHtmlAsync(string url)
        {

            using (HttpClient httpClient = new HttpClient())
            {
                string res = await httpClient.GetStringAsync(url);

                return res;
            }

        }

        private string[,] ParsePriceStats(HtmlNode nodes)
        {
            string[,] coinTable = new string[ROWS, COLS];

            int loopIndex = 0;
            int col = 0;
            int row = 0;
            foreach (HtmlNode node in nodes.Descendants())
            {
                if (node.Name == "tr" ||
                    node.NodeType != HtmlNodeType.Element ||
                    node.GetDirectInnerText() == null)
                {
                    continue;
                }

                row = loopIndex > 0 && loopIndex % 4 == 0 ? row + 1 : row;
                col = loopIndex % 4 == 0 ? 0 : col + 1;

                coinTable[row, col] = node.GetDirectInnerText();
                loopIndex++;
            }

            return coinTable;

        }

        private string[,] GetPriceStats(HtmlNode node)
        {
            if (String.IsNullOrEmpty(node.InnerHtml.Trim()))
            {
                return null;
            }
            else if (node.InnerHtml.Contains("PRICE STATS"))
            {
                var sibling = node.NextSibling;
                while (sibling != null)
                {
                    if (sibling.Name != "table")
                    {
                        sibling = sibling.NextSibling;
                        continue;
                    }
                    return ParsePriceStats(sibling);
                }
            }
            return null;
        }

        private string GetSibblingOfSearchString(HtmlNode node, string searchString)
        {
            if (node.GetDirectInnerText() != searchString)
            {
                return "";
            }
            var childNodes = node.ParentNode.ChildNodes.Descendants();

            foreach (var childNode in childNodes)
            {
                string text = childNode.GetDirectInnerText();
                if (text != searchString)
                {
                    return childNode.GetDirectInnerText();
                }
            }
            return "";
        }


        private Coin ParseHtmlAndGetData(IEnumerable<HtmlNode> nodes)
        {
            string[,] priceStatsTable = null;
            Coin coinData = new Coin();

            foreach (var node in nodes)
            {
                if (priceStatsTable == null)
                {
                    priceStatsTable = GetPriceStats(node);
                }
                if (String.IsNullOrEmpty(coinData.RewardPerDay))
                {
                    coinData.RewardPerDay = GetSibblingOfSearchString(node, "Paid rewards for masternodes:");
                }
                if (String.IsNullOrEmpty(coinData.NumberOfMasterNodes))
                {
                    coinData.NumberOfMasterNodes = GetSibblingOfSearchString(node, "Active masternodes:");

                }
                if (String.IsNullOrEmpty(coinData.RequiredAmount))
                {
                    coinData.RequiredAmount = GetSibblingOfSearchString(node, "Required coins for masternode:");
                }
                else
                {
                    break;
                }
            }
            coinData.SetPriceStats(priceStatsTable, ROWS, COLS);
            return coinData;
        }

        public async Task RunScraper()
        {
            foreach (string url in _urls)
            {
                var html = await GetHtmlAsync(url);

                HtmlDocument htmlDocument = new HtmlDocument();

                htmlDocument.LoadHtml(html);

                var body = htmlDocument.DocumentNode.SelectSingleNode("//body");
                Coin coinData = ParseHtmlAndGetData(body.Descendants());

                coinData.Symbol = url.Substring(SYMBOL_INDEX);

                Int32 timestamp = (Int32)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                coinData.Timestamp = timestamp.ToString();                
                
                await _scraperDbService.SaveCoinData(coinData);
            }
        }
    }
}

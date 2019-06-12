using HtmlAgilityPack;
using MnScraper.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MnScraper
{
    public class Scraper
    {

        private string[] _urls;
        private const string REWARD_PER_DAY_SEARCH = "Paid rewards for masternodes:";
        private const string NUMER_OF_MASTER_NODES_SEARCH = "Active masternodes:";
        private const string REQUIRED_AMOUNT_SEARCH = "Required coins for masternode:";
        public Scraper(string[] urls)
        {
            this._urls = urls;
        }

        private async Task<string> GetHtmlAsync(string url)
        {

            using (HttpClient httpClient = new HttpClient())
            {
                string res = await httpClient.GetStringAsync(url);

                return res;
            }

        }

        private Coin ParseCoinStats(HtmlNode nodes)
        {
            int rows = 3;
            int cols = 4;
            string[,] coinTable = new string[rows, cols];

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

            return new Coin(coinTable, rows, cols);

        }

        private Coin GetCoinStats(HtmlNode node)
        {
            if (String.IsNullOrEmpty(node.GetDirectInnerText()))
            {
                return null;
            }
            else if (node.GetDirectInnerText().Contains("PRICE STATS"))
            {
                var sibling = node.NextSibling;
                while (sibling != null)
                {
                    if (sibling.Name != "table")
                    {
                        sibling = sibling.NextSibling;
                        continue;
                    }

                    return ParseCoinStats(sibling);
                }
            }

            return null;
        }

        private string getMasterNodeStats(HtmlNode node, string searchString)
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


        private (MasterNode, Coin) ParseHtmlAndGetData(IEnumerable<HtmlNode> nodes)
        {
            string rewardPerDay = "";
            string numberOfMasterNodes = "";
            string requiredAmount = "";
            Coin coinData = null;

            foreach (var node in nodes)
            {
                if (coinData == null)
                {
                    coinData = GetCoinStats(node);
                }
                if (String.IsNullOrEmpty(rewardPerDay))
                {
                    rewardPerDay = getMasterNodeStats(node, REWARD_PER_DAY_SEARCH);
                }
                if (String.IsNullOrEmpty(numberOfMasterNodes))
                {
                    numberOfMasterNodes = getMasterNodeStats(node, NUMER_OF_MASTER_NODES_SEARCH);
                }
                if (String.IsNullOrEmpty(requiredAmount))
                {
                    requiredAmount = getMasterNodeStats(node, REQUIRED_AMOUNT_SEARCH);
                }
                else
                {
                    break;
                }
            }

            MasterNode masterNodeData = new MasterNode(rewardPerDay, numberOfMasterNodes, requiredAmount);
            return (masterNodeData, coinData);
        }

        public async Task RunScraper()
        {
            List<Tuple<MasterNode, Coin>> scraperResult = new List<Tuple<MasterNode, Coin>>();                         

            foreach (string url in _urls)
            {
                var html = await GetHtmlAsync(url);

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var body = htmlDocument.DocumentNode.SelectSingleNode("//body");

                (MasterNode masterNodeData, Coin coinData) = ParseHtmlAndGetData(body.Descendants());

                scraperResult.Add(new Tuple<MasterNode, Coin>(masterNodeData, coinData));

                Console.WriteLine("scraperResult = {0}", scraperResult);
                Console.WriteLine("Price = {0}, PriceInBitCoin = {1}, Volume = {2}, VolumeInBitCoin = {3}, MarketCap = {4}, MarketCapInBitCoin = {5}, Change = {6}", coinData.Price, coinData.PriceInBitCoin, coinData.Volume, coinData.VolumeInBitCoin, coinData.MarketCap, coinData.MarketCapInBitCoin, coinData.Change);
                Console.WriteLine("rewardPerDay = {0}, numberOfMasteNodes = {1}, requierdAmount = {2}", masterNodeData.RewardPerDay, masterNodeData.NumberOfMasterNodes, masterNodeData.RequiredAmount);
            }
        }
    }
}

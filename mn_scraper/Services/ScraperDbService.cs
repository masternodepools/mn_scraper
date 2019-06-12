using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using MnScraper.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MnScraper.Services
{
    public class ScraperDbService
    {
        private Table _coinTable;



        public ScraperDbService()
        {            
            var client = new AmazonDynamoDBClient();
            _coinTable = Table.LoadTable(client, "CoinData");
        }

        public async Task SaveCoinData(Coin coinData)
        {
            var doc = Document.FromJson(JsonConvert.SerializeObject(coinData));
            await _coinTable.UpdateItemAsync(doc);
        }
    }
}
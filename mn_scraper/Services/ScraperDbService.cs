using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using MnScraper.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MnScraper.Services
{
    public class ScraperDbService
    {
        private Table _coinTable;



        public ScraperDbService(AwsSettings settings)
        {

            var region = RegionEndpoint.GetBySystemName(settings.Region);
            var client = new AmazonDynamoDBClient(
                settings.AccessId,
                settings.AccessSecret,
                region);

            _coinTable = Table.LoadTable(client, "CoinData");
        }

        public async Task SaveCoinData(Coin coinData)
        {
            var doc = Document.FromJson(JsonConvert.SerializeObject(coinData));
            Console.WriteLine("DOC: {0}", doc.ToJsonPretty());
            Console.WriteLine("updating dynamoDb...");
            try
            {
                await _coinTable.UpdateItemAsync(doc);
                Console.WriteLine("Updated OK");
            }
            catch(AmazonDynamoDBException exception)
            {
                Console.WriteLine("Exception {0}", exception);
            }            
        }
    }
}
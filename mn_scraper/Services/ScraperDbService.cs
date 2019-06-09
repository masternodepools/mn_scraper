using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mn_scraper.Services
{
    public class ScraperDbService
    {
        private Table _scraperTable;
    }
}
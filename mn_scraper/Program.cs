using Microsoft.Extensions.Configuration;
using MnScraper.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MnScraper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Start of main");
            string[] urls = new string[] { "https://masternodes.online/currencies/SAPP" };


            var config = GetConfiguration();

            var awsSettings = config
                .GetSection("AwsSettings")
                .Get<AwsSettings>();

            Scraper scraper = new Scraper(urls, awsSettings);

            Console.WriteLine("run scraper");

            //var doc = Document.FromJson(JsonConvert.SerializeObject(coin));

            //Console.WriteLine("doc = {0}", doc.ToJson());

            await scraper.RunScraper();

            Console.WriteLine("done scraper");
        }


        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }
    }
}

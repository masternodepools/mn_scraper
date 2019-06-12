using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using MnScraper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MnScraper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string[] urls = new string[] { "https://masternodes.online/currencies/SAPP/" };


            var config = GetConfiguration();

            var awsSettings = config
                .GetSection("AwsSettings")
                .Get<AwsSettings>();

            Scraper scraper = new Scraper(urls);
            Console.WriteLine("run scraper");
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

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace mn_scraper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string[] urls = new string[] { "https://masternodes.online/currencies/SAPP/" };

            Scraper scraper = new Scraper(urls);
            Console.WriteLine("run scraper");
            await scraper.RunScraper();
            Console.WriteLine("done scraper");








        }

    }
}

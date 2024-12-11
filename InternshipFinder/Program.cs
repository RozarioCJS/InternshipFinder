using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace InternshipFinder
{
    public  class Program
    {
        static async Task Main()
        {
            Console.Title = "Internship Finder";
            Console.Write("Normal mode or Top 5(N/T): ");
            string mode = Console.ReadLine().ToUpper();

            if(mode == "T")
            {
                await ParallelScraping();
            }
            else
            {
                Console.Write("Please enter a url from graduates24: ");

                string url = Console.ReadLine();
                string html = await Scraper.GetPageAsync(url);
                ParseHtml(html);

                Console.Write("Would you like to try again?(Y/N): ");
                string answer = Console.ReadLine().ToUpper();
                if (answer == "Y")
                {
                    Console.Write("Keep previous results?(Y/N): ");
                    string kr = Console.ReadLine().ToUpper();
                    if (kr == "Y")
                    {
                        await Main();
                    }
                    else
                    {
                        Console.Clear();
                        await Main();
                    }
                }
                else
                {
                    Environment.Exit(0);
                }
            }
           
            Console.ReadLine();
        }
        //Parse HMTL to extract required data
        public static void ParseHtml(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            var nodesh = document.DocumentNode.SelectNodes("//h4");
            bool found = false;
            string KeyWord;
            Console.WriteLine();
            Console.Write("Please enter a term you would like to search for:");
            KeyWord = Console.ReadLine();
            Console.WriteLine();

            foreach (var node in nodesh)
            {
                if (node.InnerText.Contains(KeyWord))
                {
                    found = true;
                    HtmlNode firstChild = node.SelectSingleNode(".//a[@href]");

                    Console.WriteLine(node.InnerText);
                    Console.WriteLine(firstChild.GetAttributeValue("href", string.Empty));
                    Console.WriteLine();
                }
            }
            if (found == false)
            {
                Console.WriteLine("No Results");
            }
        }
        //Top 5 pages scraped
        public static async Task ParallelScraping()
        {
            string[] urls = { "https://www.graduates24.com/internshipprogrammes", "https://www.graduates24.com/internshipprogrammes?page=2", "https://www.graduates24.com/internshipprogrammes?page=3", "https://www.graduates24.com/internshipprogrammes?page=4", "https://www.graduates24.com/internshipprogrammes?page=5" };
            var tasks = new Task<string>[urls.Length];
            for(int i=0; i<urls.Length; i++)
            {
                tasks[i] = Scraper.GetPageAsync(urls[i]);
            }
            var results = await Task.WhenAll(tasks);
            ParseHtmls(results);
        }
        //Parse multiple pages for specific data
        public static void ParseHtmls(string[] html)
        {
            HtmlDocument document = new HtmlDocument();

            bool found = false;
            string KeyWord = "IT";
            foreach (var result in html)
            {
                document.LoadHtml(result);
                var nodesh = document.DocumentNode.SelectNodes("//h4");
                foreach (var node in nodesh)
                {
                    if (node.InnerText.Contains(KeyWord))
                    {
                        found = true;
                        HtmlNode firstChild = node.SelectSingleNode(".//a[@href]");

                        Console.WriteLine(node.InnerText);
                        Console.WriteLine(firstChild.GetAttributeValue("href", string.Empty));
                        Console.WriteLine();
                    }
                }
            }
            if (found == false)
            {
                Console.WriteLine("No Results");
            }
        }
    }
    //Fetching the page's HTML
    public class Scraper
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task<string> GetPageAsync(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}

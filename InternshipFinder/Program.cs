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
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            string url = "https://www.graduates24.com/internshipprogrammes?page=1";
            string html = await GetPageAsync(url);
            ParseHtml(html);
            Console.ReadLine();
        }

        //Fetching the page's HTML
        public static async Task<string> GetPageAsync(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        //Parse HMTL to extract required data
        public static void ParseHtml(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            var nodesh = document.DocumentNode.SelectNodes("//h4");
            bool found = false;
            foreach (var node in nodesh)
            {
                if (node.InnerText.Contains("IT"))
                {
                    found = true;
                    HtmlNode firstChild = node.SelectSingleNode(".//a[@href]");

                    Console.WriteLine(node.InnerText);
                    Console.WriteLine(firstChild.GetAttributeValue("href", string.Empty));
                    Console.WriteLine();
                }
            }
            if(found == false)
            {
                Console.WriteLine("No Results");
            }           
        }
    }
}

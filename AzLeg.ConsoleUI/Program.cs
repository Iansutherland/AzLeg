using AzLeg.ConsoleUI.Entities;
using System;

namespace AzLeg.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            try {
                using (var context = new AzLegContext())
                {
                    //base address used in scraper's httpclient
                    var scraper = new Scraper("https://www.azleg.gov/");

                    //retrieve page html
                    var stuff = scraper.retrieveURL("ars/1/00104.htm").Result;

                    Console.WriteLine("------------results----------------\n" + stuff + "\n------------results----------------");

                    Console.WriteLine("---------------------------------------\n");
                    //citation extract
                    var citation = scraper.ParseCitation(stuff);
                    Console.WriteLine($"Citation Parsed: {citation}");

                    //article content extract
                    var articleContent = scraper.ParseArticleContent(stuff);
                    var articleString = "";
                    articleContent.ForEach(x => articleString += x + "\n");
                    Console.WriteLine($"Article Content:\n{articleString}");


                    Console.ReadLine();
                }
            }
            catch( Exception exception)
            {
                Console.WriteLine("\tERROR: " + exception.Message);
            }
        }
    }
}

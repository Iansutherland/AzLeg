using AzLeg.ConsoleUI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AzLeg.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using(var context = new AzLegContext())
                {
                    //add two Titles that have been repealed
                    var title2 = new LegTitle();
                    title2.Title = 2;
                    title2.UrlAzLeg = "https://www.azleg.gov/arsDetail/?title=2";
                    var title24 = new LegTitle();
                    title24.Title = 24;
                    title24.UrlAzLeg = "https://www.azleg.gov/arsDetail/?title=24";
                    context.LegTitles.AddRange(new LegTitle[] { title2, title24 });

                    //base address used in scraper's httpclient
                    var scraper = new Scraper("https://www.azleg.gov/");

                    //all  the file names of the title page html files
                    var fileList = Directory.GetFiles(".\\Scratch\\TitlePages");

                    foreach (string url in fileList)
                    {
                        string fileContents = File.ReadAllText(url);

                        //parse for Title
                        LegTitle legTitle = scraper.ParseTitleContent(fileContents);
                        //Get title back from db so you have the DB PK for the FKs in Chapters
                        LegTitle titleFromDB = context.LegTitles.Where(x => x.Title == legTitle.Title).FirstOrDefault();

                        //get title chapters
                        var legChapterList = scraper.ParseChapterContent(fileContents, titleFromDB.Id);
                        //context.LegChapters.AddRange(legChapterList);



                        //context.SaveChanges();

                        Console.WriteLine($"finished processing title content in file {url}");
                    }
                }
            }
            catch( Exception exception)
            {
                Console.WriteLine("\tERROR: " + exception.Message);
            }
        }
    }
}

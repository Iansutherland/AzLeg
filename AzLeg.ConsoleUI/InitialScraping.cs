using AzLeg.ConsoleUI.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AzLeg.ConsoleUI
{
    class InitialScraping
    {
        public void ScrapeAndDump()
        {
            using (var context = new AzLegContext())
            {
                //base address used in scraper's httpclient
                var scraper = new Scraper("https://www.azleg.gov/");

                //all  the file names of the title page html files
                var fileList = Directory.GetFiles(".\\Scratch\\TitlePages");

                foreach (string url in fileList)
                {
                    string fileContents = File.ReadAllText(url);

                    LegTitle legTitle = scraper.ParseTitleContent(fileContents);

                    //add title to db
                    context.LegTitles.Add(legTitle);

                    LegTitle titleFromDB = context.LegTitles.Where(x => x.Title == legTitle.Title).FirstOrDefault();

                    //get title chapters
                    var legChapterList = scraper.ParseChapterContent(fileContents, titleFromDB.Id);

                    context.LegChapters.AddRange(legChapterList);

                    //add Chapter Articles

                    Console.WriteLine($"finished processing title content in file {url}");
                }

                //add two Titles that have been repealed
                //var title2 = new LegTitle();
                //title2.Title = 2;
                //title2.UrlAzLeg = "https://www.azleg.gov/arsDetail/?title=2";
                //var title24 = new LegTitle();
                //title24.Title = 24;
                //title24.UrlAzLeg = "https://www.azleg.gov/arsDetail/?title=24";
                //context.LegTitles.AddRange(new LegTitle[] { title2, title24 });

                context.SaveChanges();
            }
        }
    }
}

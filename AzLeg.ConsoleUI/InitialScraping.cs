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
        public static void ScrapeAndDump()
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

                    //parse file for title info
                    LegTitle legTitle = scraper.ParseTitleContent(fileContents);

                    //add title to db
                    context.LegTitles.Add(legTitle);

                    Console.WriteLine($"Finished adding Title to context: {url}");
                }
                //add two titles with no info on website
                AddRepealedTitles(context);

                context.SaveChanges();
                Console.WriteLine("Saved Titles to DB");

                foreach(string url in fileList)
                {
                    string fileContents = File.ReadAllText(url);

                    //parse file for title info
                    LegTitle legTitle = scraper.ParseTitleContent(fileContents);

                    //for use when legTitles have already been saved
                    LegTitle titleFromDB = context.LegTitles.Where(x => x.Title == legTitle.Title).FirstOrDefault();

                    //get title chapters
                    var legChapterList = scraper.ParseChapterContent(fileContents, titleFromDB.Id);

                    context.LegChapters.AddRange(legChapterList);

                    Console.WriteLine($"finished adding Chapter to context: {url}");
                    context.SaveChanges();
                }
                //add weird missing chapter -- can't figure out why the regex fails to match this one
                context.LegChapters
                    .Add(new LegChapter {
                        Chapter = "35.1", 
                        Heading = "HOSPITAL AND COMMUNITY HEALTH CENTER MERGERS AND OTHER TRANSACTIONS", 
                        TitleId = 1
                        });

                context.SaveChanges();
                Console.WriteLine("Saved Chapters to DB");

                foreach(string url in fileList)
                {
                    string fileContents = File.ReadAllText(url);
                    var linkList = scraper.ParseArticleLinks(fileContents);
                    foreach(var link in linkList)
                    {
                        //get title number from link
                        int titleNum;
                        //get title ID from number in DB
                        context.LegTitles.Where(x => x.Title == titleNum).FirstOrDefault();

                        //get chapter number
                        string chapterNum;
                        //get chapter ID from number in DB
                        context.LegChapters.Where(x => x.Chapter == chapterNum).FirstOrDefault();

                        //get article num
                        string articleNum;

                        //ensure savespot for new files
                        string saveDir = @".\Scratch\TitlePages";
                        if (!Directory.Exists(saveDir))
                        {
                            Directory.CreateDirectory(saveDir);
                        }

                        //build save file name
                        var fileName = saveDir + $"Title{titleNum}Chapter{chapterNum}Article{articleNum}";

                        string articlePage = "";
                        //check that this hasn't been downloaded
                        if (!File.Exists(fileName))
                        {
                            //retrieve html
                            articlePage = scraper.retrieveURL(link).Result;

                            //save to saveDir
                            File.WriteAllText(fileName, articlePage);
                        }
                        

                        var legArticle = scraper.ParseArticle(articlePage);
                    }
                }
            }
        }

        private static void AddRepealedTitles(AzLegContext context)
        {
            //add two Titles that have been repealed and aren't present on the site
            var title2 = new LegTitle
            {
                Title = 2,
                UrlAzLeg = "https://www.azleg.gov/arsDetail/?title=2"
            };
            var title24 = new LegTitle
            {
                Title = 24,
                UrlAzLeg = "https://www.azleg.gov/arsDetail/?title=24"
            };
            context.LegTitles.AddRange(new LegTitle[] { title2, title24 });
        }
    }
}

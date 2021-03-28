using AzLeg.ConsoleUI.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AzLeg.ConsoleUI
{
    class InitialScraping
    {
        public static void ScrapeAndDump()
        {
            Random rand = new Random();
            using (var context = new AzLegContext())
            {
                //base address used in scraper's httpclient
                var scraper = new Scraper("https://www.azleg.gov/");

                //all  the file names of the title page html files
                var fileList = Directory.GetFiles(".\\Scratch\\TitlePages");
                //don't run these again because they've already been loaded
                if (true)
                {
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
                }

                foreach(string url in fileList)
                {
                    string fileContents = File.ReadAllText(url);
                    List<string> chapterSectionList = scraper.ParseChapterSections(fileContents);
                    chapterSectionList.Remove("");
                    foreach(string chapterSection in chapterSectionList)
                    {
                        var articleSectionList = scraper.ParseArticleSections(chapterSection);
                        articleSectionList.Remove("");
                        foreach (var article in articleSectionList)
                        {
                            Console.WriteLine($"article section processing");
                            //get article links
                            var articleLinkList = scraper.ParseArticleLinks(article);

                            foreach(string link in articleLinkList)
                            {
                                //get title number from link
                                int titleNum = scraper.ParseArticleForTitle(link);
                                //get title ID from number in DB
                                var tempTitle = context.LegTitles.Where(x => x.Title == titleNum).FirstOrDefault();

                                //get chapter number
                                string chapterNum = scraper.ParseChapterFormat(chapterSection);
                                //get chapter ID from number in DB
                                var tempChapter = context.LegChapters.Where(x => x.Chapter == chapterNum).FirstOrDefault();

                                string articleNum = scraper.ParseArticleForArticle(link);

                                //get all the article files and save and/or parse
                                //ensure savespot for new files
                                string saveDir = @".\Scratch\ArticlePages\";
                                if (!Directory.Exists(saveDir))
                                {
                                    Directory.CreateDirectory(saveDir);
                                }

                                //build save file name
                                var fileName = saveDir + $"T{titleNum}C{chapterNum.Replace(" ", "")}A{articleNum}.txt";

                                string articlePage = "";

                                //bool to track if download happened
                                bool downloadedFile = false;
                                //check that this hasn't been downloaded
                                if (!File.Exists(fileName))
                                {
                                    //retrieve html
                                    articlePage = scraper.retrieveURL(link).Result;
                                    downloadedFile = true;
                                    Console.WriteLine($"retrieving: {link}");

                                    //save to saveDir
                                    File.WriteAllText(fileName, articlePage);
                                }
                                else
                                {
                                    articlePage = File.ReadAllText(fileName);
                                    Console.WriteLine($"Opening file: {fileName} of link: {link}");
                                }

                                var legArticle = scraper.ParseArticle(articlePage);
                                legArticle.UrlAzLeg = link;
                                legArticle.TitleId = tempTitle.Id;
                                legArticle.ChapterId = tempChapter.Id;

                                if(legArticle.UrlAzLeg.Length >= 2083)
                                {
                                    var a = 1;
                                }
                                if(legArticle.Heading.Length >= 50)
                                {
                                    var a = 1;
                                }

                                if(legArticle.Citation.Length >= 20)
                                {
                                    var a = 1;
                                }

                                if (downloadedFile)
                                {
                                    int variability = rand.Next(1, 5);
                                    Console.WriteLine($"thread sleep: {2+variability}s");
                                    Thread.Sleep(2000 + (variability*1000));
                                }
                            }

                            context.SaveChanges();
                        }
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

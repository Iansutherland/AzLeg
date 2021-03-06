using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AzLeg.ConsoleUI.Entities;
using AzLeg.Data.Models;

namespace AzLeg.ConsoleUI
{
    class Scraper : IDisposable
    {
        private HttpClient httpClient;

        public Scraper(string baseAddress)
        {
            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new Uri(baseAddress);
        }
        /// <summary>
        /// Get the source of the given URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> retrieveURL(string url)
        {
            var response = await this.httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception($"response code from {this.httpClient.BaseAddress}/{url} returned code {response.StatusCode}");
            }
        }

        /// <summary>
        /// Citations in AzLeg look like 1-203 where '1' is the Title, '2' is the Chapter, and '3' is the Article number (two digits)
        /// </summary>
        /// <returns></returns>
        private CitationFormat ParseCitation(string content)
        {
            Regex citationPattern = new Regex(@"(\d{1})-(\d{1})(\d{1,2})");
            var matchesFound = citationPattern.Match(content);
            var citation = new CitationFormat();
            if(matchesFound.Groups.Count > 1)
            {
                citation.Title = Int32.Parse(matchesFound.Groups[1].Value);
                citation.Chapter = Int32.Parse(matchesFound.Groups[2].Value);
                citation.Article = Int32.Parse(matchesFound.Groups[3].Value);
            }

            return citation;
        }

        /// <summary>
        /// Get the Article Title from the page content
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string ParseArticleTitle(string content)
        {
            Regex titlePattern = new Regex(@"<u>(.*)</u>");
            var matcheFound = titlePattern.Match(content);
            if (matcheFound.Success)
            {
                return matcheFound.Value.Replace("<u>", "").Replace("</u>", "");
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Get the legislation verbiage from the given string
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private List<string> ParseArticleContent(string content)
        {
            Regex htmlParagraph = new Regex(@"<p>(.*)</p>");
            var matchesFound = htmlParagraph.Matches(content);
            var list = new List<string>();
            if (matchesFound.Count> 1)
            {
                foreach(Group group in matchesFound)
                {
                    list.Add(group.Value.Replace("<p>", "").Replace("</p>", ""));
                }
            }

            list.RemoveAt(0);

            return list;
        }

        public LegArticle ParseArticle(string content)
        {
            var article = new LegArticle();
            article.Citation = this.ParseCitation(content).ToString();

            article.Heading = this.ParseArticleTitle(content);

            article.Content = String.Join(" ", this.ParseArticleContent(content));

            return article;
        }

        /// <summary>
        /// Get the Title content from the detail page (/arsDetail/?title=1)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public LegTitle ParseTitleContent(string content)
        {
            var title = new LegTitle();
            Regex titlePattern = new Regex(@"Title (\d{1,2}) - ((\w*\s|\w*)*)");
            var match = titlePattern.Match(content);
            if (match.Success)
            {
                title.Title = Int32.Parse(match.Groups[1].Value);
                title.Heading = match.Groups[2].Value;
                title.UrlAzLeg = $@"https://www.azleg.gov/arsDetail?title={title.Title}";
            }

            return title;
        }

        /// <summary>
        /// Get the Chapter Content from the detail page (/arsDetail/?title=1)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<LegChapter> ParseChapterContent(string content, int titleNum)
        {
            var legChapterList = new List<LegChapter>();

            //Get the chapter number and set
            Regex chapterPattern = new Regex("Chapter (\\d{1,2}|\\d{1,2}\\.\\d)<\\/a><div class=\"five-sixth\">(\\w( \\w|\\w|'|-|,)*)<\\/div>");
            var matchesFound = chapterPattern.Matches(content);
            if(matchesFound.Count > 0)
            {
                foreach(Match match in matchesFound)
                {
                    var tempLegChapter = new LegChapter();
                    tempLegChapter.Chapter = match.Groups[1].Value;
                    tempLegChapter.Heading = match.Groups[2].Value;
                    tempLegChapter.TitleId = titleNum;
                    legChapterList.Add(tempLegChapter);
                }
            }

            return legChapterList;
        }

        /// <summary>
        /// Run to get all filelinks in FileLinks.txt
        /// </summary>
        public void GetAllTitleHTML()
        {
            //retrieve page html
            var titleLinkList = File.ReadAllLines(".\\Scratch\\FileLinks.txt").ToList();

            foreach (var url in titleLinkList)
            {
                //get the title page contents
                var titlePageContents = this.retrieveURL(url).Result;

                //make scratch directory so we don't have to call the server for this request again later
                string urlBit = url.Substring(url.IndexOf('=') + 1);
                var saveSpot = $@"D:\Ian\Development\AzLeg\AzLeg.ConsoleUI\Scratch\{"Title" + urlBit}.txt";
                File.WriteAllText(saveSpot, titlePageContents);
                Console.WriteLine($"Finished file at {saveSpot}");

                //var legTitle = scraper.ParseTitleContent(titlePageContents);
            }
            Console.WriteLine("\tdone");
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
        }
    }
}

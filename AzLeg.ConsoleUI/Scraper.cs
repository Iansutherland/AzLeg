using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        public CitationFormat ParseCitation(string content)
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
        /// Get the legislation verbiage from the given string
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<string> ParseArticleContent(string content)
        {
            Regex htmlParagraph = new Regex(@"<p>(.*)</p>");
            var matchesFound = htmlParagraph.Match(content);
            var list = new List<string>();
            if (matchesFound.Groups.Count > 1)
            {
                foreach(Group group in matchesFound.Groups)
                {

                    list.Add(group.Value);
                }
            }

            list.RemoveAt(0);

            return list;
        }

        /// <summary>
        /// Get the Title content from the detail page (/arsDetail/?title=1)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string ParseTitleContent(string content)
        {
            return "";
        }

        /// <summary>
        /// Get the Chapter Content from the detail page (/arsDetail/?title=1)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string ParseChapterContent(string content)
        {
            return "";
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace AzLeg.ConsoleUI.Entities
{
    public partial class LegArticle
    {
        public LegArticle()
        {
            Article2ArticleFromArticleNavigations = new HashSet<Article2Article>();
            Article2ArticleToArticleNavigations = new HashSet<Article2Article>();
        }

        public int Id { get; set; }
        public int TitleId { get; set; }
        public int ChapterId { get; set; }
        public int Article { get; set; }
        public string Content { get; set; }
        public string Citation { get; set; }
        public string UrlAzLeg { get; set; }
        public DateTime? CreateDate { get; set; }

        public virtual LegChapter Chapter { get; set; }
        public virtual LegTitle Title { get; set; }
        public virtual ICollection<Article2Article> Article2ArticleFromArticleNavigations { get; set; }
        public virtual ICollection<Article2Article> Article2ArticleToArticleNavigations { get; set; }
    }
}

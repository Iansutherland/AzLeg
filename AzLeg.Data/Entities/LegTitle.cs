using System;
using System.Collections.Generic;

#nullable disable

namespace AzLeg.ConsoleUI.Entities
{
    public partial class LegTitle
    {
        public LegTitle()
        {
            LegArticles = new HashSet<LegArticle>();
            LegChapters = new HashSet<LegChapter>();
        }

        public int Id { get; set; }
        public string Heading { get; set; }
        public string UrlAzLeg { get; set; }
        public int Title { get; set; }

        public virtual ICollection<LegArticle> LegArticles { get; set; }
        public virtual ICollection<LegChapter> LegChapters { get; set; }
    }
}

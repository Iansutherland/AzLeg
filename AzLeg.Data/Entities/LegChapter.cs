using System;
using System.Collections.Generic;

#nullable disable

namespace AzLeg.ConsoleUI.Entities
{
    public partial class LegChapter
    {
        public LegChapter()
        {
            LegArticles = new HashSet<LegArticle>();
        }

        public int Id { get; set; }
        public int TitleId { get; set; }
        public string Chapter { get; set; }
        public string Heading { get; set; }

        public virtual LegTitle Title { get; set; }
        public virtual ICollection<LegArticle> LegArticles { get; set; }
    }
}

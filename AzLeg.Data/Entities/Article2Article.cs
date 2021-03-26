using System;
using System.Collections.Generic;

#nullable disable

namespace AzLeg.ConsoleUI.Entities
{
    public partial class Article2Article
    {
        public int Id { get; set; }
        public int FromArticle { get; set; }
        public int ToArticle { get; set; }
        public bool? ReferencedInText { get; set; }

        public virtual LegArticle FromArticleNavigation { get; set; }
        public virtual LegArticle ToArticleNavigation { get; set; }
    }
}

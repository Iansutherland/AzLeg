using System;
using System.Collections.Generic;
using System.Text;

namespace AzLeg.Data.Models
{
    public class CitationFormat
    {
        public int Title { get; set; }
        public int Chapter { get; set; }
        public int Article { get; set; }

        public override string ToString()
        {
            return $"{Title}-{Chapter}{Article.ToString().PadLeft(2, '0')}";
        }
    }
}

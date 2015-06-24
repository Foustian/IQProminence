using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prominence.Model
{
    public class OnlineNewsProminenceInput
    {
        public string IQSeqID { get; set; }

        public List<double> Sentiments { get; set; }

        public string HeadLine { get; set; }

        public string SearchTerm { get; set; }

        public Int32 TotalMention { get; set; }

        public bool IsParagraphSearch { get; set; }

        public float SentimentLowThreshold { get; set; }

        public float SentimentHighThreshold { get; set; }
    }
}

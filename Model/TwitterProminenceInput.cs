using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prominence.Model
{
    public class TwitterProminenceInput
    {
        public string Tweet_ID { get; set; }

        public List<double> Sentiments { get; set; }

        public Int32 KloutScore { get; set; }

        public float SentimentLowThreshold { get; set; }

        public float SentimentHighThreshold { get; set; }
    }
}

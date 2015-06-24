using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prominence.Model
{
    public class SocialMediaProminenceInput
    {
        public string IQSeqID { get; set; }

        public List<double> Sentiments { get; set; }

        public string ProgramType { get; set; }

        public float SentimentLowThreshold { get; set; }

        public float SentimentHighThreshold { get; set; }
    }
}

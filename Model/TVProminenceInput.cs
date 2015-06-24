using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prominence.Model
{
    public class TVProminenceInput
    {
        public Guid VideoGuid { get; set; }

        public List<double> Sentiments { get; set; }

        public string Dma_Num { get; set; }

        public string ProgramType { get; set; }

        public float SentimentLowThreshold { get; set; }

        public float SentimentHighThreshold { get; set; }

        public string Station_Affiliate { get; set; }

        public DateTime Air_DateTime { get; set; }
    }
}

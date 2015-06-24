using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Prominence
{
    internal static class ProminenceMultipliers
    {
        internal struct TVDMAMultiplier
        {
            internal static KeyValuePair<int, double> Top10 = new KeyValuePair<int, double>(10, 1.25);
            internal static KeyValuePair<int, double> Top20 = new KeyValuePair<int, double>(20, 1.10);
            internal static KeyValuePair<int, double> Top50 = new KeyValuePair<int, double>(50, 0.90);
            internal static KeyValuePair<int, double> Top100 = new KeyValuePair<int, double>(100, 0.80);
            internal static double Rest = 0.75;
        }

        internal struct TVSentimentMultiplier
        {
            internal static double InRange = 1;
            internal static double OutRange = 1.25;
        }

        internal static Dictionary<string, double> TVProgramTypeMultiplier = new Dictionary<string, double>()
            {
                {"News",1.25},
                {"Sports",1.00}
            };

        internal static double TVRestProgramTypeMultiplier = 0.90;

        internal static KeyValuePair<List<string>, double> TVPrimeTimeProgramTypeMultiplier = new KeyValuePair<List<string>, double>
            (new List<string>() { 
                "abc",
                "nbc",
                "cw",
                "fox",
                "ion",
                "cbs",
                "univision",
                "pbs"
            }, 1.20);

        internal struct SMSentimentMultiplier
        {
            internal static double InRange = 1;
            internal static double OutRange = 1.25;
        }

        internal static Dictionary<string, double> SMProgramTypeMultiplier = new Dictionary<string, double>()
            {
                {"Blog",1.20},
                {"Forum",1.10}
            };

        internal static double SMRestProgramTypeMultiplier = 1.00;

        internal struct TwitterSentimentMultiplier
        {
            internal static double InRange = 1;
            internal static double OutRange = 1.25;
        }

        internal struct TwitterKloutMultiplier
        {
            internal static KeyValuePair<int, double> GT90 = new KeyValuePair<int, double>(90, 1.20);
            internal static KeyValuePair<int, double> GT65 = new KeyValuePair<int, double>(65, 1.10);
            internal static double Rest = 1.00;
        }

        internal struct NewsSentimentMultiplier
        {
            internal static double InRange = 1;
            internal static double OutRange = 1.25;
        }

        internal struct NewsLeadParagraph
        {
            internal static double True = 1.25;
            internal static double False = 1;
        }

        internal struct NewsHeadlineMultiplier
        {
            internal static double InHeadLine = 1.25;
            internal static double NotInHeadLine = 1;
        }


        internal struct NewsMentionMultiplier
        {
            internal static KeyValuePair<int, double> One = new KeyValuePair<int, double>(1, 1.00);
            internal static KeyValuePair<int, double> Two = new KeyValuePair<int, double>(2, 1.10);
            internal static KeyValuePair<int, double> Three = new KeyValuePair<int, double>(3, 1.15);
            internal static KeyValuePair<int, double> Four = new KeyValuePair<int, double>(4, 1.20);
            internal static double GTFive = 1.25;
        }

        internal static double PQRestMultiplier = 1.25;

        internal struct ProQuestHeadlineMultiplier
        {
            internal static double InHeadLine = 1.25;
            internal static double NotInHeadLine = 1;
        }

        internal struct ProQuestLeadParagraph
        {
            internal static double True = 1.15;
            internal static double False = 1;
        }

        internal struct ProQuestSentimentMultiplier
        {
            internal static double InRange = 1;
            internal static double OutRange = 1.25;
        }

        internal struct ProQuestMentionMultiplier
        {
            internal static KeyValuePair<int, double> One = new KeyValuePair<int, double>(1, 1.00);
            internal static KeyValuePair<int, double> Two = new KeyValuePair<int, double>(2, 1.10);
            internal static KeyValuePair<int, double> Three = new KeyValuePair<int, double>(3, 1.15);
            internal static KeyValuePair<int, double> Four = new KeyValuePair<int, double>(4, 1.20);
            internal static double GTFive = 1.25;
        }
    }
}

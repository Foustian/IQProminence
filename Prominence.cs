using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Prominence.Utility;
using Prominence.Model;

namespace Prominence
{
    public static class Prominence
    {
        public static double CalculateTVProminence(string p_DmaNum, List<double> p_SentimentValues, float p_TVSentimentLowThreshold, float p_TVSentimentHighThreshold, string p_ProgramType, string p_Station_Affil, DateTime p_Air_DateTime, string PromineceLogLocation = "", Boolean IsLogProminence = true)
        {
            try
            {

                double DmaMultiplier = 0;
                double SentimentMultiplier = 0;
                double ProgramTypeMultiplier = 0;

                Utility.CommonFunction.LogInfo(string.Format("Calculate TV Prominence Input : \n" + " DmaNum : {0}\n ProgramType : {1}\n Sentiment Values : {2}\n TV Low Threshold : {3}\n  TV High Threshold : {4}\n Station Affiliate : {5}\n Air Datetime : {6}", p_DmaNum, p_ProgramType, string.Join(", ", p_SentimentValues), p_TVSentimentLowThreshold, p_TVSentimentHighThreshold, p_Station_Affil, p_Air_DateTime), IsLogProminence, PromineceLogLocation);

                short DmaNumber = Convert.ToInt16(p_DmaNum);

                if (DmaNumber <= ProminenceMultipliers.TVDMAMultiplier.Top10.Key)
                {
                    DmaMultiplier = ProminenceMultipliers.TVDMAMultiplier.Top10.Value;
                }
                else if (DmaNumber <= ProminenceMultipliers.TVDMAMultiplier.Top20.Key)
                {
                    DmaMultiplier = ProminenceMultipliers.TVDMAMultiplier.Top20.Value;
                }
                else if (DmaNumber <= ProminenceMultipliers.TVDMAMultiplier.Top50.Key)
                {
                    DmaMultiplier = ProminenceMultipliers.TVDMAMultiplier.Top50.Value;
                }
                else if (DmaNumber <= ProminenceMultipliers.TVDMAMultiplier.Top100.Key)
                {
                    DmaMultiplier = ProminenceMultipliers.TVDMAMultiplier.Top100.Value;
                }
                else
                {
                    DmaMultiplier = ProminenceMultipliers.TVDMAMultiplier.Rest;
                }


                Utility.CommonFunction.LogInfo("DMA  - Value: \"" + p_DmaNum + "\" Multiplier: \"" + DmaMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                SentimentMultiplier = ProminenceMultipliers.TVSentimentMultiplier.InRange;

                foreach (double sentiment in p_SentimentValues)
                {
                    if (sentiment < p_TVSentimentLowThreshold || sentiment > p_TVSentimentHighThreshold)
                    {
                        SentimentMultiplier = ProminenceMultipliers.TVSentimentMultiplier.OutRange;
                        break;
                    }
                }

                Utility.CommonFunction.LogInfo("Sentiments - Value: \"" + string.Join(",", p_SentimentValues) + "\" Multiplier: \"" + SentimentMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                if (!ProminenceMultipliers.TVProgramTypeMultiplier.TryGetValue(p_ProgramType, out ProgramTypeMultiplier))
                {
                    TimeSpan range_start = new TimeSpan(20, 0, 0);
                    if (ProminenceMultipliers.TVPrimeTimeProgramTypeMultiplier.Key.Contains(p_Station_Affil.ToLower()) && p_Air_DateTime.TimeOfDay >= range_start)
                    {
                        ProgramTypeMultiplier = ProminenceMultipliers.TVPrimeTimeProgramTypeMultiplier.Value;
                        Utility.CommonFunction.LogInfo("ProgramType - Station Affil - Value: \"" + p_Station_Affil + "\", Air Time - Value: " + p_Air_DateTime + ", Multiplier: \"" + ProgramTypeMultiplier + "\"", IsLogProminence, PromineceLogLocation);
                    }
                    else
                    {
                        ProgramTypeMultiplier = ProminenceMultipliers.TVRestProgramTypeMultiplier;
                        Utility.CommonFunction.LogInfo("ProgramType - Value: \"" + p_ProgramType + "\" Multiplier: \"" + ProgramTypeMultiplier + "\"", IsLogProminence, PromineceLogLocation);
                    }
                }
                else
                {
                    Utility.CommonFunction.LogInfo("ProgramType - Value: \"" + p_ProgramType + "\" Multiplier: \"" + ProgramTypeMultiplier + "\"", IsLogProminence, PromineceLogLocation);
                }



                return (DmaMultiplier * SentimentMultiplier * ProgramTypeMultiplier);
            }
            catch (Exception ex)
            {
                CommonFunction.LogInfo("Error On Calculate TV Prominence :" + ex.Message);
                throw;
            }
        }

        public static double CalculateOnlineNewsProminence(bool p_IsSearchTermInHeadline, List<double> p_SentimentValues, float p_NewsSentimentLowThreshold, float p_NewsSentimentHighThreshold, int p_TotalMention, bool p_ParagraphSearch, string PromineceLogLocation = "", Boolean IsLogProminence = true)
        {
            try
            {
                double HeadlineMultiplier = 0;
                double SentimentMultiplier = 0;
                double MentionMultiplier = 0;
                double LeadParagraphMultiplier = 0;

                Utility.CommonFunction.LogInfo(string.Format("Calculate Online News Prominence Input : \n" + " IsSearchTermInHeadline : {0}\n Sentiment Values : {1}\n News Low Threshold : {2}\n  News High Threshold : {3}\n  Total Mention : {4}\n  Paragraph Search? : {5}", p_IsSearchTermInHeadline, string.Join(", ", p_SentimentValues), p_NewsSentimentLowThreshold, p_NewsSentimentHighThreshold, p_TotalMention, p_ParagraphSearch), IsLogProminence, PromineceLogLocation);

                if (p_ParagraphSearch)
                {
                    LeadParagraphMultiplier = ProminenceMultipliers.NewsLeadParagraph.True;
                }
                else
                {
                    LeadParagraphMultiplier = ProminenceMultipliers.NewsLeadParagraph.False;
                }

                Utility.CommonFunction.LogInfo("Paragraph Search - Value: \"" + p_ParagraphSearch + "\" Multiplier: \"" + LeadParagraphMultiplier + "\"", IsLogProminence, PromineceLogLocation);


                SentimentMultiplier = ProminenceMultipliers.NewsSentimentMultiplier.InRange;

                foreach (double sentiment in p_SentimentValues)
                {
                    if (sentiment > 0 && (sentiment < p_NewsSentimentLowThreshold || sentiment > p_NewsSentimentHighThreshold))
                    {
                        SentimentMultiplier = ProminenceMultipliers.NewsSentimentMultiplier.OutRange;
                        break;
                    }
                }

                Utility.CommonFunction.LogInfo("Sentiments - Value: \"" + string.Join(",", p_SentimentValues) + "\", Multiplier: \"" + SentimentMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                if (p_IsSearchTermInHeadline)
                {
                    HeadlineMultiplier = ProminenceMultipliers.NewsHeadlineMultiplier.InHeadLine; 
                }
                else
                {
                    HeadlineMultiplier = ProminenceMultipliers.NewsHeadlineMultiplier.NotInHeadLine; 
                }

                Utility.CommonFunction.LogInfo("IsSearchTermInHeadLine - Value: \"" + p_IsSearchTermInHeadline + "\" Multiplier: \"" + HeadlineMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                if (p_TotalMention == ProminenceMultipliers.NewsMentionMultiplier.One.Key)
                {
                    MentionMultiplier = ProminenceMultipliers.NewsMentionMultiplier.One.Value;
                }
                else if (p_TotalMention == ProminenceMultipliers.NewsMentionMultiplier.Two.Key)
                {
                    MentionMultiplier = ProminenceMultipliers.NewsMentionMultiplier.Two.Value;
                }
                else if (p_TotalMention == ProminenceMultipliers.NewsMentionMultiplier.Three.Key)
                {
                    MentionMultiplier = ProminenceMultipliers.NewsMentionMultiplier.Three.Value;
                }
                else if (p_TotalMention == ProminenceMultipliers.NewsMentionMultiplier.Four.Key)
                {
                    MentionMultiplier = ProminenceMultipliers.NewsMentionMultiplier.Four.Value;
                }
                else
                {
                    MentionMultiplier = ProminenceMultipliers.NewsMentionMultiplier.GTFive;
                }

                Utility.CommonFunction.LogInfo("Mention - Value: \"" + p_TotalMention + "\" Multiplier: \"" + MentionMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                return (HeadlineMultiplier * SentimentMultiplier * LeadParagraphMultiplier * MentionMultiplier);
            }
            catch (Exception ex)
            {
                CommonFunction.LogInfo("Error On Calculate Online News Prominence :" + ex.Message);
                throw;
            }
        }

        public static double CalculateSocialMediaProminence(List<double> p_SentimentValues, float p_SMSentimentLowThreshold, float p_SMSentimentHighThreshold, string p_ProgramType, string PromineceLogLocation = "", Boolean IsLogProminence = true)
        {
            try
            {
                double SentimentMultiplier = 0;
                double ProgramTypeMultiplier = 0;

                Utility.CommonFunction.LogInfo(string.Format("Calculate Social Media Prominence Input : \n ProgramType : {0}\n Sentiment Values : {1}\n TV Low Threshold : {2}\n  TV High Threshold : {3}", p_ProgramType, string.Join(", ", p_SentimentValues), p_SMSentimentLowThreshold, p_SMSentimentHighThreshold), IsLogProminence, PromineceLogLocation);

                SentimentMultiplier = ProminenceMultipliers.SMSentimentMultiplier.InRange;

                foreach (double sentiment in p_SentimentValues)
                {
                    if (sentiment < p_SMSentimentLowThreshold || sentiment > p_SMSentimentHighThreshold)
                    {
                        SentimentMultiplier = ProminenceMultipliers.SMSentimentMultiplier.OutRange;
                        break;
                    }
                }

                Utility.CommonFunction.LogInfo("Sentiments - Value: \"" + string.Join(",", p_SentimentValues) + "\", Multiplier: \"" + SentimentMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                if (!ProminenceMultipliers.SMProgramTypeMultiplier.TryGetValue(p_ProgramType, out ProgramTypeMultiplier))
                {
                    ProgramTypeMultiplier = ProminenceMultipliers.SMRestProgramTypeMultiplier;
                }

                Utility.CommonFunction.LogInfo("ProgramType - Value: \"" + p_ProgramType + "\" Multiplier :\"" + ProgramTypeMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                return (SentimentMultiplier * ProgramTypeMultiplier);
            }
            catch (Exception ex)
            {
                CommonFunction.LogInfo("Error On Calculate Social Media Prominence :" + ex.Message);
                throw;
            }
        }

        public static double CalculateTwitterProminence(Int64 p_KloutScore, List<double> p_SentimentValues, float p_TWSentimentLowThreshold, float p_TWSentimentHighThreshold, string PromineceLogLocation = "", Boolean IsLogProminence = true)
        {
            try
            {
                Utility.CommonFunction.LogInfo(string.Format("Calculate Twitter Prominence Input : \n Klout Score : {0}\n Sentiment Values : {1}\n TV Low Threshold : {2}\n  TV High Threshold : {3}", p_KloutScore, string.Join(", ", p_SentimentValues), p_TWSentimentLowThreshold, p_TWSentimentHighThreshold), IsLogProminence, PromineceLogLocation);

                double SentimentMultiplier = 0;
                double KloutScoreMultiplier = 0;

                if (p_KloutScore > ProminenceMultipliers.TwitterKloutMultiplier.GT90.Key)
                {
                    KloutScoreMultiplier = ProminenceMultipliers.TwitterKloutMultiplier.GT90.Value;
                }
                else if (p_KloutScore > ProminenceMultipliers.TwitterKloutMultiplier.GT65.Key)
                {
                    KloutScoreMultiplier = ProminenceMultipliers.TwitterKloutMultiplier.GT65.Value;
                }
                else
                {
                    KloutScoreMultiplier = ProminenceMultipliers.TwitterKloutMultiplier.Rest;
                }

                Utility.CommonFunction.LogInfo("Klout Score - Value: \"" + p_KloutScore + "\" Multiplier: \"" + KloutScoreMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                SentimentMultiplier = ProminenceMultipliers.TwitterSentimentMultiplier.InRange;

                foreach (double sentiment in p_SentimentValues)
                {
                    if (sentiment < p_TWSentimentLowThreshold || sentiment > p_TWSentimentHighThreshold)
                    {
                        SentimentMultiplier = ProminenceMultipliers.TwitterSentimentMultiplier.OutRange;
                        break;
                    }
                }

                Utility.CommonFunction.LogInfo("Sentiments - Value: \"" + string.Join(",", p_SentimentValues) + "\", Multiplier: \"" + SentimentMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                return (SentimentMultiplier * KloutScoreMultiplier);
            }
            catch (Exception ex)
            {
                CommonFunction.LogInfo("Error On Calculate Twitter Prominence :" + ex.Message);
                throw;
            }
        }

        public static double CalculateProQuestProminence(bool p_IsSearchTermInHeadline, List<double> p_SentimentValues, float p_PQSentimentLowThreshold, float p_PQSentimentHighThreshold, int p_TotalMentions, bool p_ParagraphSearch, string ProminenceLogLocation = "", Boolean IsLogProminence = true)
        {
            try
            {
                double PQRestMultiplier = ProminenceMultipliers.PQRestMultiplier;
                double HeadlineMultiplier = 0;
                double SentimentMultiplier = 0;
                double MentionMultiplier = 0;
                double LeadParagraphMultiplier = 0;

                Utility.CommonFunction.LogInfo(string.Format("Calculate ProQuest Prominence Input : \n" + " IsSearchTermInHeadline : {0}\n Sentiment Values : {1}\n PQ Low Threshold : {2}\n  PQ High Threshold : {3}\n  Total Mention : {4}\n  Paragraph Search? : {5}", p_IsSearchTermInHeadline, string.Join(", ", p_SentimentValues), p_PQSentimentLowThreshold, p_PQSentimentHighThreshold, p_TotalMentions, p_ParagraphSearch), IsLogProminence, ProminenceLogLocation);

                if (p_ParagraphSearch)
                {
                    LeadParagraphMultiplier = ProminenceMultipliers.ProQuestLeadParagraph.True;
                }
                else
                {
                    LeadParagraphMultiplier = ProminenceMultipliers.ProQuestLeadParagraph.False;
                }

                Utility.CommonFunction.LogInfo("Paragraph Search - Value: \"" + p_ParagraphSearch + "\" Multiplier: \"" + LeadParagraphMultiplier + "\"", IsLogProminence, ProminenceLogLocation);


                SentimentMultiplier = ProminenceMultipliers.ProQuestSentimentMultiplier.InRange;

                foreach (double sentiment in p_SentimentValues)
                {
                    if (sentiment > 0 && (sentiment < p_PQSentimentLowThreshold || sentiment > p_PQSentimentHighThreshold))
                    {
                        SentimentMultiplier = ProminenceMultipliers.ProQuestSentimentMultiplier.OutRange;
                        break;
                    }
                }

                Utility.CommonFunction.LogInfo("Sentiments - Value: \"" + string.Join(",", p_SentimentValues) + "\", Multiplier: \"" + SentimentMultiplier + "\"", IsLogProminence, ProminenceLogLocation);

                if (p_IsSearchTermInHeadline)
                {
                    HeadlineMultiplier = ProminenceMultipliers.ProQuestHeadlineMultiplier.InHeadLine;
                }
                else
                {
                    HeadlineMultiplier = ProminenceMultipliers.ProQuestHeadlineMultiplier.NotInHeadLine;
                }

                Utility.CommonFunction.LogInfo("IsSearchTermInHeadLine - Value: \"" + p_IsSearchTermInHeadline + "\" Multiplier: \"" + HeadlineMultiplier + "\"", IsLogProminence, ProminenceLogLocation);

                if (p_TotalMentions == ProminenceMultipliers.ProQuestMentionMultiplier.One.Key)
                {
                    MentionMultiplier = ProminenceMultipliers.ProQuestMentionMultiplier.One.Value;
                }
                else if (p_TotalMentions == ProminenceMultipliers.ProQuestMentionMultiplier.Two.Key)
                {
                    MentionMultiplier = ProminenceMultipliers.ProQuestMentionMultiplier.Two.Value;
                }
                else if (p_TotalMentions == ProminenceMultipliers.ProQuestMentionMultiplier.Three.Key)
                {
                    MentionMultiplier = ProminenceMultipliers.ProQuestMentionMultiplier.Three.Value;
                }
                else if (p_TotalMentions == ProminenceMultipliers.ProQuestMentionMultiplier.Four.Key)
                {
                    MentionMultiplier = ProminenceMultipliers.ProQuestMentionMultiplier.Four.Value;
                }
                else
                {
                    MentionMultiplier = ProminenceMultipliers.ProQuestMentionMultiplier.GTFive;
                }

                Utility.CommonFunction.LogInfo("Mention - Value: \"" + p_TotalMentions + "\" Multiplier: \"" + MentionMultiplier + "\"", IsLogProminence, ProminenceLogLocation);

                return (PQRestMultiplier * HeadlineMultiplier * SentimentMultiplier * LeadParagraphMultiplier * MentionMultiplier);
            }
            catch (Exception ex)
            {
                CommonFunction.LogInfo("Error On Calculate Online News Prominence :" + ex.Message);
                throw;
            }
        }

        public static List<ProminenceOutput> CalculateTVProminence(List<TVProminenceInput> p_TVProminenceInputList, string PromineceLogLocation = "", Boolean IsLogProminence = true)
        {
            try
            {
                List<ProminenceOutput> lstOfProminenceOutput = new List<ProminenceOutput>();

                foreach (TVProminenceInput input in p_TVProminenceInputList)
                {
                    ProminenceOutput prominenceOutput = new ProminenceOutput();


                    double DmaMultiplier = 0;
                    double SentimentMultiplier = 0;
                    double ProgramTypeMultiplier = 0;

                    Utility.CommonFunction.LogInfo(string.Format("Calculate TV Prominence Input : \n" + " DmaNum : {0}\n ProgramType : {1}\n Sentiment Values : {2}\n TV Low Threshold : {3}\n TV High Threshold : {4}\n Station Affiliate : {5}\n Air Datetime : {6}", input.Dma_Num, input.ProgramType, string.Join(", ", input.Sentiments), input.SentimentLowThreshold, input.SentimentHighThreshold, input.Station_Affiliate, input.Air_DateTime), IsLogProminence, PromineceLogLocation);

                    short DmaNumber = Convert.ToInt16(input.Dma_Num);

                    if (DmaNumber <= ProminenceMultipliers.TVDMAMultiplier.Top10.Key)
                    {
                        DmaMultiplier = ProminenceMultipliers.TVDMAMultiplier.Top10.Value;
                    }
                    else if (DmaNumber <= ProminenceMultipliers.TVDMAMultiplier.Top20.Key)
                    {
                        DmaMultiplier = ProminenceMultipliers.TVDMAMultiplier.Top20.Value;
                    }
                    else if (DmaNumber <= ProminenceMultipliers.TVDMAMultiplier.Top50.Key)
                    {
                        DmaMultiplier = ProminenceMultipliers.TVDMAMultiplier.Top50.Value;
                    }
                    else if (DmaNumber <= ProminenceMultipliers.TVDMAMultiplier.Top100.Key)
                    {
                        DmaMultiplier = ProminenceMultipliers.TVDMAMultiplier.Top100.Value;
                    }
                    else
                    {
                        DmaMultiplier = ProminenceMultipliers.TVDMAMultiplier.Rest;
                    }


                    Utility.CommonFunction.LogInfo("DMA  - Value: \"" + input.Dma_Num + "\" Multiplier: \"" + DmaMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                    SentimentMultiplier = ProminenceMultipliers.TVSentimentMultiplier.InRange;

                    foreach (double sentiment in input.Sentiments)
                    {
                        if (sentiment < input.SentimentLowThreshold || sentiment > input.SentimentHighThreshold)
                        {
                            SentimentMultiplier = ProminenceMultipliers.TVSentimentMultiplier.OutRange;
                            break;
                        }
                    }

                    Utility.CommonFunction.LogInfo("Sentiments - Value: \"" + string.Join(",", input.Sentiments) + "\" Multiplier: \"" + SentimentMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                    if (!ProminenceMultipliers.TVProgramTypeMultiplier.TryGetValue(input.ProgramType, out ProgramTypeMultiplier))
                    {
                        TimeSpan range_start = new TimeSpan(20, 0, 0);
                        TimeSpan range_end = new TimeSpan(0, 0, 0);
                        TimeSpan air_time = input.Air_DateTime.TimeOfDay;

                        bool IsInMidNightRange = air_time == range_start || air_time == range_start || (air_time > range_start && air_time > range_end);

                        if (ProminenceMultipliers.TVPrimeTimeProgramTypeMultiplier.Key.Contains(input.Station_Affiliate.ToLower()) && IsInMidNightRange)
                        {
                            ProgramTypeMultiplier = ProminenceMultipliers.TVPrimeTimeProgramTypeMultiplier.Value;
                            Utility.CommonFunction.LogInfo("ProgramType - Station Affil - Value: \"" + input.Station_Affiliate + "\", Air Time - Value: " + input.Air_DateTime + ", Multiplier: \"" + ProgramTypeMultiplier + "\"", IsLogProminence, PromineceLogLocation);
                        }
                        else
                        {
                            ProgramTypeMultiplier = ProminenceMultipliers.TVRestProgramTypeMultiplier;
                            Utility.CommonFunction.LogInfo("ProgramType - Value: \"" + input.ProgramType + "\" Multiplier: \"" + ProgramTypeMultiplier + "\"", IsLogProminence, PromineceLogLocation);
                        }
                    }
                    else
                    {
                        Utility.CommonFunction.LogInfo("ProgramType - Value: \"" + input.ProgramType + "\" Multiplier: \"" + ProgramTypeMultiplier + "\"", IsLogProminence, PromineceLogLocation);
                    }

                    prominenceOutput.ID = Convert.ToString(input.VideoGuid);
                    prominenceOutput.ProminenceMultiPlier = (DmaMultiplier * SentimentMultiplier * ProgramTypeMultiplier);

                    lstOfProminenceOutput.Add(prominenceOutput);

                }

                return lstOfProminenceOutput;


            }
            catch (Exception ex)
            {
                CommonFunction.LogInfo("Error On Calculate TV Prominence :" + ex.Message);
                throw;
            }
        }

        public static List<ProminenceOutput> CalculateOnlineNewsProminence(List<OnlineNewsProminenceInput> p_OnlineNewsProminenceInputList, string PromineceLogLocation = "", Boolean IsLogProminence = true)
        {
            try
            {
                List<ProminenceOutput> lstOfProminenceOutput = new List<ProminenceOutput>();

                foreach (OnlineNewsProminenceInput input in p_OnlineNewsProminenceInputList)
                {
                    double HeadlineMultiplier = 0;
                    double SentimentMultiplier = 0;
                    double MentionMultiplier = 0;
                    double LeadParagraphMultiplier = 0;

                    ProminenceOutput prominenceOutput = new ProminenceOutput();

                    Utility.CommonFunction.LogInfo(string.Format("Calculate Online News Prominence Input : \n" + " HeadLine : {0}\n SearchTerm : {1}\n Sentiment Values : {2}\n News Low Threshold : {3}\n  News High Threshold : {4}\n  Total Mention : {5}\n  Paragraph Search? : {6}", input.HeadLine, input.SearchTerm, string.Join(", ", input.Sentiments), input.SentimentLowThreshold, input.SentimentHighThreshold, input.TotalMention, input.IsParagraphSearch), IsLogProminence, PromineceLogLocation);

                    if (input.IsParagraphSearch)
                    {
                        LeadParagraphMultiplier = ProminenceMultipliers.NewsLeadParagraph.True;
                    }
                    else
                    {
                        LeadParagraphMultiplier = ProminenceMultipliers.NewsLeadParagraph.False;
                    }

                    Utility.CommonFunction.LogInfo("Paragraph Search - Value: \"" + input.IsParagraphSearch + "\" Multiplier: \"" + LeadParagraphMultiplier + "\"", IsLogProminence, PromineceLogLocation);


                    SentimentMultiplier = ProminenceMultipliers.NewsSentimentMultiplier.InRange;

                    foreach (double sentiment in input.Sentiments)
                    {
                        if (sentiment < input.SentimentLowThreshold || sentiment > input.SentimentHighThreshold)
                        {
                            SentimentMultiplier = ProminenceMultipliers.NewsSentimentMultiplier.OutRange;
                            break;
                        }
                    }

                    Utility.CommonFunction.LogInfo("Sentiments - Value: \"" + string.Join(",", input.Sentiments) + "\", Multiplier: \"" + SentimentMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                    HeadlineMultiplier = ProminenceMultipliers.NewsHeadlineMultiplier.NotInHeadLine;
                    if (!string.IsNullOrEmpty(input.HeadLine) && !string.IsNullOrEmpty(input.SearchTerm))
                    {
                        // replace all special characters to space'
                        string _SearchTerm = Regex.Replace(input.SearchTerm, "[^0-9a-zA-Z]+", " ");

                        // replace multiple spaces to a signle space.
                        _SearchTerm = Regex.Replace(_SearchTerm, @"\s{2,}", " ");

                        foreach (string substring in _SearchTerm.Split(' '))
                        {
                            // match a word from string
                            if (Regex.IsMatch(input.HeadLine, "^.*(\b" + substring + "\b)?.*$"))
                            {
                                HeadlineMultiplier = ProminenceMultipliers.NewsHeadlineMultiplier.InHeadLine;
                                break;
                            }
                        }
                    }

                    Utility.CommonFunction.LogInfo("HeadLine - Value: \"" + input.HeadLine + "\" SearchTerm - Value: \"" + input.SearchTerm + "\" Multiplier: \"" + HeadlineMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                    if (input.TotalMention == ProminenceMultipliers.NewsMentionMultiplier.One.Key)
                    {
                        MentionMultiplier = ProminenceMultipliers.NewsMentionMultiplier.One.Value;
                    }
                    else if (input.TotalMention == ProminenceMultipliers.NewsMentionMultiplier.Two.Key)
                    {
                        MentionMultiplier = ProminenceMultipliers.NewsMentionMultiplier.Two.Value;
                    }
                    else if (input.TotalMention == ProminenceMultipliers.NewsMentionMultiplier.Three.Key)
                    {
                        MentionMultiplier = ProminenceMultipliers.NewsMentionMultiplier.Three.Value;
                    }
                    else if (input.TotalMention == ProminenceMultipliers.NewsMentionMultiplier.Four.Key)
                    {
                        MentionMultiplier = ProminenceMultipliers.NewsMentionMultiplier.Four.Value;
                    }
                    else
                    {
                        MentionMultiplier = ProminenceMultipliers.NewsMentionMultiplier.GTFive;
                    }

                    Utility.CommonFunction.LogInfo("Mention - Value: \"" + input.TotalMention + "\" Multiplier: \"" + MentionMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                    prominenceOutput.ID = input.IQSeqID;
                    prominenceOutput.ProminenceMultiPlier = (HeadlineMultiplier * SentimentMultiplier * LeadParagraphMultiplier);

                    lstOfProminenceOutput.Add(prominenceOutput);
                }

                return lstOfProminenceOutput;
            }
            catch (Exception ex)
            {
                CommonFunction.LogInfo("Error On Calculate Online News Prominence :" + ex.Message);
                throw;
            }
        }

        public static List<ProminenceOutput> CalculateSocialMediaProminence(List<SocialMediaProminenceInput> p_SocialMediaProminenceInputList, string PromineceLogLocation = "", Boolean IsLogProminence = true)
        {
            try
            {
                List<ProminenceOutput> lstOfProminenceOutput = new List<ProminenceOutput>();

                foreach (SocialMediaProminenceInput input in p_SocialMediaProminenceInputList)
                {
                    double SentimentMultiplier = 0;
                    double ProgramTypeMultiplier = 0;

                    ProminenceOutput prominenceOutput = new ProminenceOutput();

                    Utility.CommonFunction.LogInfo(string.Format("Calculate Social Media Prominence Input : \n ProgramType : {0}\n Sentiment Values : {1}\n TV Low Threshold : {2}\n  TV High Threshold : {3}", input.ProgramType, string.Join(", ", input.Sentiments), input.SentimentLowThreshold, input.SentimentHighThreshold), IsLogProminence, PromineceLogLocation);

                    SentimentMultiplier = ProminenceMultipliers.SMSentimentMultiplier.InRange;

                    foreach (double sentiment in input.Sentiments)
                    {
                        if (sentiment < input.SentimentLowThreshold || sentiment > input.SentimentHighThreshold)
                        {
                            SentimentMultiplier = ProminenceMultipliers.SMSentimentMultiplier.OutRange;
                            break;
                        }
                    }

                    Utility.CommonFunction.LogInfo("Sentiments - Value: \"" + string.Join(",", input.Sentiments) + "\", Multiplier: \"" + SentimentMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                    if (!ProminenceMultipliers.SMProgramTypeMultiplier.TryGetValue(input.ProgramType, out ProgramTypeMultiplier))
                    {
                        ProgramTypeMultiplier = ProminenceMultipliers.SMRestProgramTypeMultiplier;
                    }

                    Utility.CommonFunction.LogInfo("ProgramType - Value: \"" + input.ProgramType + "\" Multiplier :\"" + ProgramTypeMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                    prominenceOutput.ID = input.IQSeqID;
                    prominenceOutput.ProminenceMultiPlier = (SentimentMultiplier * ProgramTypeMultiplier);


                    lstOfProminenceOutput.Add(prominenceOutput);

                }

                return lstOfProminenceOutput;
            }
            catch (Exception ex)
            {
                CommonFunction.LogInfo("Error On Calculate Social Media Prominence :" + ex.Message);
                throw;
            }
        }

        public static List<ProminenceOutput> CalculateTwitterProminence(List<TwitterProminenceInput> p_TwitterProminenceInputList, string PromineceLogLocation = "", Boolean IsLogProminence = true)
        {
            try
            {
                List<ProminenceOutput> lstOfProminenceOutput = new List<ProminenceOutput>();

                foreach (TwitterProminenceInput input in p_TwitterProminenceInputList)
                {
                    double SentimentMultiplier = 0;
                    double KloutScoreMultiplier = 0;

                    ProminenceOutput prominenceOutput = new ProminenceOutput();

                    Utility.CommonFunction.LogInfo(string.Format("Calculate Twitter Prominence Input : \n Klout Score : {0}\n Sentiment Values : {1}\n TV Low Threshold : {2}\n  TV High Threshold : {3}", input.KloutScore, string.Join(", ", input.Sentiments), input.SentimentLowThreshold, input.SentimentHighThreshold), IsLogProminence, PromineceLogLocation);

                    if (input.KloutScore > ProminenceMultipliers.TwitterKloutMultiplier.GT90.Key)
                    {
                        KloutScoreMultiplier = ProminenceMultipliers.TwitterKloutMultiplier.GT90.Value;
                    }
                    else if (input.KloutScore > ProminenceMultipliers.TwitterKloutMultiplier.GT65.Key)
                    {
                        KloutScoreMultiplier = ProminenceMultipliers.TwitterKloutMultiplier.GT65.Value;
                    }
                    else
                    {
                        KloutScoreMultiplier = ProminenceMultipliers.TwitterKloutMultiplier.Rest;
                    }

                    Utility.CommonFunction.LogInfo("Klout Score - Value: \"" + input.KloutScore + "\" Multiplier: \"" + KloutScoreMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                    SentimentMultiplier = ProminenceMultipliers.TwitterSentimentMultiplier.InRange;

                    foreach (double sentiment in input.Sentiments)
                    {
                        if (sentiment < input.SentimentLowThreshold || sentiment > input.SentimentHighThreshold)
                        {
                            SentimentMultiplier = ProminenceMultipliers.TwitterSentimentMultiplier.OutRange;
                            break;
                        }
                    }

                    Utility.CommonFunction.LogInfo("Sentiments - Value: \"" + string.Join(",", input.Sentiments) + "\", Multiplier: \"" + SentimentMultiplier + "\"", IsLogProminence, PromineceLogLocation);

                    prominenceOutput.ID = input.Tweet_ID;
                    prominenceOutput.ProminenceMultiPlier = (SentimentMultiplier * KloutScoreMultiplier);
                }

                return lstOfProminenceOutput;
            }
            catch (Exception ex)
            {
                CommonFunction.LogInfo("Error On Calculate Twitter Prominence :" + ex.Message);
                throw;
            }
        }

        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}

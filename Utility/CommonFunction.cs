using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace Prominence.Utility
{
    internal static class CommonFunction
    {
        static CommonFunction()
        {

        }

        internal static void LogInfo(string LogMessage, bool IsProminenceLogging = false, string ProminenceLogFileLocation = "")
        {
            try
            {
                if (ConfigurationManager.AppSettings["IsProminenceLogging"] != null && Convert.ToBoolean(ConfigurationManager.AppSettings["IsProminenceLogging"], System.Globalization.CultureInfo.CurrentCulture) == true)
                {
                    string path = ConfigurationManager.AppSettings["ProminenceLogFileLocation"] + "LOG_" + DateTime.Today.ToString("MMddyyyy", System.Globalization.CultureInfo.CurrentCulture) + ".csv";

                    if (!Directory.Exists(ConfigurationManager.AppSettings["ProminenceLogFileLocation"]))
                    {
                        Directory.CreateDirectory(ConfigurationManager.AppSettings["ProminenceLogFileLocation"]);
                    }

                    if (!File.Exists(path))
                    {
                        File.Create(path).Close();
                    }
                    using (StreamWriter w = File.AppendText(path))
                    {
                        w.WriteLine(DateTime.Now.ToString() + " , [INFO] ,\"" + LogMessage + "\"");
                    }
                }
                else if (ConfigurationManager.AppSettings["IsProminenceLogging"] == null && IsProminenceLogging == true && !string.IsNullOrEmpty(ProminenceLogFileLocation))
                {
                    string path = ProminenceLogFileLocation + "LOG_" + DateTime.Today.ToString("MMddyyyy", System.Globalization.CultureInfo.CurrentCulture) + ".csv";

                    if (!Directory.Exists(ProminenceLogFileLocation))
                    {
                        Directory.CreateDirectory(ProminenceLogFileLocation);
                    }

                    if (!File.Exists(path))
                    {
                        File.Create(path).Close();
                    }
                    using (StreamWriter w = File.AppendText(path))
                    {
                        w.WriteLine(DateTime.Now.ToString() + " , [INFO] ,\"" + LogMessage + "\"");
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}

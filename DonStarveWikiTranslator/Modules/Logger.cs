using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonStarveWikiTranslator.Modules
{
    internal class Logger
    {
        private static readonly string LogDirectory = @"d:\Code\DonStarveWikiTranslator\DonStarveWikiTranslator\Logs";

        public static void Log(string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            
            // Log to console (useful for debugging)
            Console.WriteLine(logMessage);

            try
            {
                // Ensure Logs directory exists
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }

                // Append to log file named by date
                string fileName = $"log_{DateTime.Now:yyyyMMdd}.txt";
                string filePath = Path.Combine(LogDirectory, fileName);

                File.AppendAllText(filePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Fallback to console if file logging fails
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }
    }
}

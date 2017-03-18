using Crankdata.Models;
using DownloadFCCFMStationData.AppCode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadFCCFMStationData
{
    class Program
    {
        static void Main(string[] args)
        {
            bool saveToFS = false;
            string dbConnectionString = ConfigurationManager.AppSettings["dbConnectionString"];
            string dbName = ConfigurationManager.AppSettings["dbName"];

            string saveToFileSystem = ConfigurationManager.AppSettings["SaveToFileSystem"];
            string saveFolderPath = ConfigurationManager.AppSettings["SaveFolderPath"];

            if (!String.IsNullOrEmpty(saveToFileSystem))
            {
                Boolean.TryParse(saveToFileSystem, out saveToFS);
            }

            string fccFMStationSearchURL = ConfigurationManager.AppSettings["FCCFMStationSearchURL"];
            string fccFMStationCoverageMapURL = ConfigurationManager.AppSettings["FCCFMStationCoverageMapURL"];


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                CrankdataContext dbContext = new CrankdataContext(dbConnectionString, dbName);

                FCCFMStationInfoDownload fccFMStationInfoDownload = new FCCFMStationInfoDownload(dbContext, 
                                                                        fccFMStationSearchURL, 
                                                                        fccFMStationCoverageMapURL,
                                                                        saveToFS, 
                                                                        saveFolderPath);
                                                           

                fccFMStationInfoDownload.Process();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured. Error={0}", ex.Message);
            }




            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            Console.WriteLine("Total time to process \"{0}\"", elapsedTime);
        }
    }
}

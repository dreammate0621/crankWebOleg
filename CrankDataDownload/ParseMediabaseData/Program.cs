using System;
using Crankdata.Models;
using System.Configuration;
using ParseMediabaseData.AppCode;
using ParseMediabaseData.AppCode.Utils;
using System.Diagnostics;

namespace ParseMediabaseData
{
    class Program
    {
        static void Main(string[] args)
        {
            string dbConnectionString = ConfigurationManager.AppSettings["dbConnectionString"];
            string dbName = ConfigurationManager.AppSettings["dbName"];
          
            string songsCsvFolderPath = ConfigurationManager.AppSettings["songscsvfolder"];
            string songsArchiveFolderPath = ConfigurationManager.AppSettings["songsarchivefolder"];
            string stationsCsvFolderPath = ConfigurationManager.AppSettings["stationscsvfolder"];
            string stationsArchiveFolderPath = ConfigurationManager.AppSettings["stationsarchviefolder"];
            string mediabaseScript = ConfigurationManager.AppSettings["mediabasescript"];

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                MediabaseDownload mbDownload = new MediabaseDownload(mediabaseScript);

                //Cleanup old files
                FileHelper.CleanUpFiles(songsCsvFolderPath, "*.csv");
                FileHelper.CleanUpFiles(stationsCsvFolderPath, "*.csv");

                //Begin Mediabase data download
                mbDownload.StartMediabaseDownload();

                CrankdataContext dbContext = new CrankdataContext(dbConnectionString, dbName);
                ParseSongStats parseSongStats = new ParseSongStats(dbContext, songsCsvFolderPath, songsArchiveFolderPath);
                parseSongStats.Process();

                ParseStationStats parseStationStats = new ParseStationStats(dbContext, stationsCsvFolderPath, stationsArchiveFolderPath);
                parseStationStats.Process();
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

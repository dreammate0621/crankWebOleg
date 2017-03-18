using System;
using System.Diagnostics;
using Crankdata.Models;
using System.Configuration;
using DownloadSongKickData.AppCode;

using NLog;

namespace DownloadSongKickData
{
    class Program
    {
        private static Logger logger;


        static void Main(string[] args)
        {
            logger = LogManager.GetCurrentClassLogger();
            string dbConnectionString = ConfigurationManager.AppSettings["dbConnectionString"];
            string dbName = ConfigurationManager.AppSettings["dbName"];

            string songKickAPIKey = ConfigurationManager.AppSettings["SongKickAPIKey"];
            string songKickArtistSearchURL = ConfigurationManager.AppSettings["SongKickArtistSearchURL"];
            string songKickArtistCalendarURL = ConfigurationManager.AppSettings["SongKickArtistCalendarURL"];
            string songKickVenueDetailsURL = ConfigurationManager.AppSettings["SongKickVenueDetailsURL"];
            string songKickImageURL = ConfigurationManager.AppSettings["SongKickImageURL"]; 


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            logger.Info("Starting SongKick data download...");
            
            try
            {
                CrankdataContext dbContext = new CrankdataContext(dbConnectionString, dbName);

                SongKickApiDownload songKickDownload = new SongKickApiDownload(dbContext, 
                                                                               songKickAPIKey, 
                                                                               songKickArtistSearchURL, 
                                                                               songKickArtistCalendarURL,
                                                                               songKickVenueDetailsURL);
                
                songKickDownload.Process();
            }
            catch(Exception ex)
            {
                logger.Fatal("Exception occured. Error={0}", ex.Message);
            }




            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            logger.Info("SongKick data download complete. Total time to process \"{0}\"", elapsedTime);
        }
    }
}

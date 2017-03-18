using Crankdata.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadArtistDetailFrom7Digital
{
    class Program
    {
        private static Logger logger;
        static void Main(string[] args)
        {

            logger = LogManager.GetCurrentClassLogger();
            string dbConnectionString = ConfigurationManager.AppSettings["dbConnectionString"];
            string dbName = ConfigurationManager.AppSettings["dbName"];
            bool saveImagesToFS = false;
            string saveImageToFileSystem = ConfigurationManager.AppSettings["SaveImageToFileSystem"];
            string imageSaveFolderPath = ConfigurationManager.AppSettings["ImageSaveFolderPath"];

            if (!String.IsNullOrEmpty(saveImageToFileSystem))
            {
                Boolean.TryParse(saveImageToFileSystem, out saveImagesToFS);
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            logger.Info("Starting 7digital data download...");

            try
            {
                CrankdataContext dbContext = new CrankdataContext(dbConnectionString, dbName);

                DownloadArtistDetail sevenDigitalDownload = new DownloadArtistDetail(dbContext, imageSaveFolderPath,saveImagesToFS);

                sevenDigitalDownload.Process().Wait();
            }
            catch (Exception ex)
            {
                logger.Fatal("Exception occured. Error={0}", ex.Message);
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

            logger.Info("7 digital data download complete. Total time to process \"{0}\"", elapsedTime);

        }
    }
}

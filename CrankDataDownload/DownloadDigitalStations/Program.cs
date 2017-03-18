using Crankdata.Models;
using DownloadDigitalStations.AppCode;
using System;
using System.Configuration;
using System.Diagnostics;

namespace DownloadDigitalStations
{
    class Program
    {
        static void Main(string[] args)
        {
            bool saveImagesToFS = false;

            string dbConnectionString = ConfigurationManager.AppSettings["dbConnectionString"];
            string dbName = ConfigurationManager.AppSettings["dbName"];

            string UserAgent = ConfigurationManager.AppSettings["UserAgent"];
            string saveImageToFileSystem = ConfigurationManager.AppSettings["SaveImageToFileSystem"];
            string imageSaveFolderPath = ConfigurationManager.AppSettings["ImageSaveFolderPath"];

            if (!String.IsNullOrEmpty(saveImageToFileSystem))
            {
                Boolean.TryParse(saveImageToFileSystem, out saveImagesToFS);
            }

            string proMuscicDownloadURL = ConfigurationManager.AppSettings["ProMuscicDownloadURL"];


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                CrankdataContext dbContext = new CrankdataContext(dbConnectionString, dbName);

                ProMusicImageDownload proMusicImageDownload = new ProMusicImageDownload(dbContext,
                                                                                        proMuscicDownloadURL,
                                                                                        UserAgent,
                                                                                        saveImagesToFS,
                                                                                        imageSaveFolderPath);
                proMusicImageDownload.Process().Wait();
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

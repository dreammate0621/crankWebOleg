using Crankdata.Models;
using DownloadSongKickArtistImage.AppCode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadSongKickArtistImage
{
    class Program
    {
        static void Main(string[] args)
        {
            bool saveImagesToFS = false;

            string dbConnectionString = ConfigurationManager.AppSettings["dbConnectionString"];
            string dbName = ConfigurationManager.AppSettings["dbName"];

            string saveImageToFileSystem = ConfigurationManager.AppSettings["SaveImageToFileSystem"];
            string imageSaveFolderPath = ConfigurationManager.AppSettings["ImageSaveFolderPath"];

            if (!String.IsNullOrEmpty(saveImageToFileSystem))
            {
                Boolean.TryParse(saveImageToFileSystem, out saveImagesToFS);
            }
            
            string songKickImageURL = ConfigurationManager.AppSettings["SongKickImageURL"];


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                CrankdataContext dbContext = new CrankdataContext(dbConnectionString, dbName);

                SongKickImageDownload songKickImageDownload = new SongKickImageDownload(dbContext,
                                                                                        songKickImageURL,
                                                                                        saveImagesToFS,
                                                                                        imageSaveFolderPath);
                songKickImageDownload.Process();
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

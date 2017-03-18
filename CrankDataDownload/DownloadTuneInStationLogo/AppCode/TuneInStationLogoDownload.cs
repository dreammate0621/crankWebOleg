using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Network;
using AngleSharp.Network.Default;
using Crankdata.Models;
using MongoDB.Driver;
using DownloadTuneInStationLogo.AppCode.Models;
using System.Collections.Generic;
using AngleSharp.Dom;
using AngleSharp.Dom.Navigator;
using System.Text.RegularExpressions;

namespace DownloadTuneInStationLogo.AppCode
{
    public class TuneInStationLogoDownload
    {
        #region private vaiables
        private string tuneInStartURL;
        private CrankdataContext dbContext;
        IMongoCollection<Station> stations = null;
        IMongoCollection<StationImage> stationImages = null;
        private bool saveImageToFileSystem;
        string imageSaveFolderPath;
        string UserAgent = String.Empty;

        IBrowsingContext bContext = null;

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        Regex stationInfoMatch = new Regex(@"^(.*?)\s+-\s+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Regex stationNoLogo = new Regex(@"s0q.png$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.


                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SongKickImageDownload() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        public TuneInStationLogoDownload(CrankdataContext dbContext,
                                    string tuneInStartURL,
                                    string userAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36",
                                    bool saveImageToFileSystem = false,
                                    string imageSaveFolderPath = "")
        {
            this.dbContext = dbContext;
            this.tuneInStartURL = tuneInStartURL;
            this.UserAgent = userAgent;
            this.saveImageToFileSystem = saveImageToFileSystem;
            this.imageSaveFolderPath = imageSaveFolderPath;

            Init();
        }

        private void Init()
        {
            //TODO: Initialize objects here    

            //Get MongoDB collections
            stations = dbContext.Stations;
            stationImages = dbContext.StationImages;


            //Initialize the browsing context
            var config = Configuration.Default.WithDefaultLoader().WithCookies();

            var httpRequester = config.Services.OfType<HttpRequester>().FirstOrDefault() as HttpRequester;

            //Chnage user agent
            httpRequester.Headers["User-Agent"] = UserAgent;

            //Set timeout to 10 mins
            httpRequester.Timeout = new TimeSpan(0, 10, 0);

            //Create BrowsingContext
            bContext = BrowsingContext.New(config);


            if (String.IsNullOrEmpty(imageSaveFolderPath))
            {
                imageSaveFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
            }

            if (saveImageToFileSystem && !Directory.Exists(imageSaveFolderPath))
            {
                Directory.CreateDirectory(imageSaveFolderPath);
            }
        }

        public async Task Process()
        {
            await DownloadTuenInStationImages();
        }

        private async Task DownloadTuenInStationImages()
        {

            Url startUrl = new Url(tuneInStartURL);

            var document = await bContext.OpenAsync(startUrl).ConfigureAwait(false);

            //Console.WriteLine(document.ToHtml());

            //Extract State elements
            var states = document.QuerySelectorAll(".link a");
            foreach(IHtmlAnchorElement stateLink in states )
            {
                Console.WriteLine($"Processing state {stateLink.TextContent} - {stateLink.Href}");
                ProcessState(stateLink);
            }

        }

        private void ProcessState(IHtmlAnchorElement stateLink)
        {
            //Process state link

            var sTask = stateLink.NavigateAsync();
            sTask.Wait();
            var sDocument = sTask.Result;
           // Console.WriteLine(sDocument.ToHtml());

            var regions = sDocument.QuerySelectorAll(".link a");
            foreach (IHtmlAnchorElement regionLink in regions)
            {
                Console.WriteLine($"Processing region {regionLink.InnerHtml} - {regionLink.Href}");
                ProcessRegion(regionLink);
            }

        }

        private void ProcessRegion(IHtmlAnchorElement regionLink)
        {
            var sTask = regionLink.NavigateAsync();
            sTask.Wait();
            var sDocument = sTask.Result;
           // Console.WriteLine(sDocument.ToHtml());
            var stations = sDocument.QuerySelectorAll(".station a");
            foreach (IHtmlAnchorElement stationLink in stations)
            {
                ProcessStation(stationLink);
            }


        }

        private void ProcessStation(IHtmlAnchorElement stationLink)
        {
            string stationCallCodeFull = string.Empty;
            string stationCallCode = string.Empty;
            string stationTitle = string.Empty;
            byte[] stationLogo = null;


            Console.WriteLine("Station Name = {0}", stationLink.Href);

            var title = stationLink.QuerySelector(".title");
            var imgElement = stationLink.QuerySelector(".background-image") as IHtmlImageElement;

            Console.WriteLine("{0} - {1}", title.TextContent, imgElement.Source);

            //Skip if the logo in empty
            if (!stationNoLogo.IsMatch(imgElement.Source))
            {

                //Load station details
                var sTask = stationLink.NavigateAsync();
                sTask.Wait();
                var sDocument = sTask.Result;

                Console.WriteLine("Station Title = {0}", sDocument.Title);

                //Extract Station Call Code

                Match rMatch = stationInfoMatch.Match(sDocument.Title);
                if (rMatch.Success)
                {
                    //We have match, extract CALL code
                    stationCallCodeFull = rMatch.Groups[1].Value;
                    stationCallCode = stationCallCodeFull.Contains("-FM") ? stationCallCodeFull.Substring(0, stationCallCodeFull.IndexOf('-')) : stationCallCodeFull;

                    stationTitle = sDocument.QuerySelector(".ribbon h1")?.TextContent;

                    Console.WriteLine("Call Code Full:{0}, Call Code:{1}, Title: {2}, Logo URL: {3}", stationCallCodeFull, stationCallCode, stationTitle, imgElement.Source);
                    //Find the station in DB, if exist download Logo, otherwise skip
                    Station dbStation = FindStationInDb(stationCallCode);
                    if(dbStation != null)
                    {
                        Console.WriteLine("Downloading logo for station {0} : {1}", stationCallCode, imgElement.Source);
                        var sLogoDownloadTask = DownloadStationLogoAsync(imgElement.Source);
                        sLogoDownloadTask.Wait();
                        stationLogo = sLogoDownloadTask.Result;

                        if (stationLogo != null)
                        {
                            //Time to party...!!!
                            //Save logo to database

                            StationThumbnail stationThumbnail = new StationThumbnail();
                            stationThumbnail.Name = stationCallCodeFull;
                            stationThumbnail.CallCode = stationCallCode;

                            List<Thumbnail> sThumbnails = new List<Thumbnail>();
                            Thumbnail sThumb = new Thumbnail();
                            sThumb.Source = "TuneIn";
                            sThumb.Size = "large";
                            sThumb.data = stationLogo;
                            sThumb.Width = 145;
                            sThumb.Height = 145;
                            sThumb.MimeType = "image/png";

                            sThumbnails.Add(sThumb);

                            stationThumbnail.Thumbnails = sThumbnails;

                            //Insert station thumb nail
                            SaveStationImageThumbnailsToDb(dbStation, stationThumbnail);

                        }
                    }
                    else
                    {
                        Console.WriteLine("Unable to find station with call code {0} in the database", stationCallCode);
                    }
                }
                else
                {
                    Console.WriteLine("Skipping station {0}, unable to extract Call code information.", sDocument.Title);
                }
            }
            else
            {
                Console.WriteLine("Station {0} doesn't have logo, skipping...", title);
            }
        }

        private Station FindStationInDb(string callCode)
        {
            var filter = Builders<Station>.Filter.Eq(s => s.Callcode, callCode);

            Station station = stations.Find(filter).FirstOrDefault();

            return station;
        }
        private void SaveStationImageThumbnailsToDb(Station station, StationThumbnail stationThumbnail)
        {
            try
            {
                UpdateOptions updateOptions = new UpdateOptions { IsUpsert = true };

                //Insert the image
                StationImage sImage = new StationImage();
                sImage.StationId = station.Id;

                foreach (var sThumbnail in stationThumbnail.Thumbnails)
                {
                    var sImageThumbnail = new ImageThumbnail();
                    sImageThumbnail.Size = sThumbnail.Size;
                    sImageThumbnail.Width = sThumbnail.Width;
                    sImageThumbnail.Height = sThumbnail.Height;
                    sImageThumbnail.Source = sThumbnail.Source;
                    sImageThumbnail.data = sThumbnail.data;
                    sImageThumbnail.MimeType = sThumbnail.MimeType;

                    sImage.Images.Add(sImageThumbnail);
                }

                var filterSI = Builders<StationImage>.Filter.Eq(sI => sI.StationId, sImage.StationId);

                //If exist replace, else insert
                stationImages.ReplaceOne(filterSI, sImage, updateOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while saving image thumbnails of station \"{0}\" to database.\r\nError={1}", stationThumbnail.Name, ex.ToString());
            }
        }

        private async Task<byte[]> DownloadStationLogoAsync(string stationLogoUrl)
        {
            byte[] imageData = null;

            try
            {

                var request = new DocumentRequest(new Url(stationLogoUrl));

                var loader = bContext.Loader;

                if (loader != null)
                {
                    var download = loader.DownloadAsync(request);

                    using (var response = await download.Task.ConfigureAwait(false))
                    {
                        if (response != null)
                        {
                            using (var imgReader = new MemoryStream())
                            {
                                await response.Content.CopyToAsync(imgReader);
                                imageData = imgReader.ToArray();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("No content returned for URL \"{0}\". Error = {1}", stationLogoUrl, ex.ToString());
            }

            return imageData;
        }


        private void SaveImageToFile(byte[] imageData, string FileName)
        {
            if (saveImageToFileSystem)
            {
                try
                {
                    string cleanFileName = CleanFileName(FileName);
                    string FilePath = Path.Combine(imageSaveFolderPath, String.Format("{0}.png", cleanFileName));

                    using (Stream fileStream = File.Open(FilePath, FileMode.Create))
                    using (BinaryWriter imgWriter = new BinaryWriter(fileStream))
                    {
                        imgWriter.Write(imageData);
                        imgWriter.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured while saving image thumbnails to file \"{0}\".\r\nError={1}", FileName, ex.ToString());
                }
            }
        }

        private string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

    }
}

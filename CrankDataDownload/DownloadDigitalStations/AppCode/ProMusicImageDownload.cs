using System;
using System.Net.Http;
using DownloadSongKickArtistImage.AppCode.Models;
using Crankdata.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Linq;
using AngleSharp;
using AngleSharp.Extensions;
using AngleSharp.Network.Default;
using AngleSharp.Dom.Html;
using AngleSharp.Network;
using System.Collections.Generic;
using DownloadDigitalStations.AppCode.Models;
using AngleSharp.Dom;

namespace DownloadDigitalStations.AppCode
{
    public class ProMusicImageDownload : IDisposable
    {
        #region private vaiables
        private string proMuscicDownloadURL;
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

        public ProMusicImageDownload(CrankdataContext dbContext,
                                    string proMuscicDownloadURL,
                                    string userAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36",
                                    bool saveImageToFileSystem = false,
                                    string imageSaveFolderPath = "")
        {
            this.dbContext = dbContext;
            this.proMuscicDownloadURL = proMuscicDownloadURL;
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

            // pass pro music continents page url
            await GetContinentsFromProMusic(proMuscicDownloadURL).ContinueWith(async (c) =>
          {
              var continents = c.Result;

              WriteInfoToConsole($"continents are :-");

              // Print continents name
              for (int j = 0; j < continents.Count; j++)
              {
                  WriteTextToConsole($"{(j + 1)}-{ continents[j].Name}");
              }

              foreach (var continent in continents)
              {
                  // Getting countires of continent
                  await GetContinentCountries(continent.Href, continent.Name).ContinueWith(cc =>
                  {
                      // print countries name
                      for (int j = 0; j < cc.Result.Count; j++)
                      {
                          WriteTextToConsole($"{(j + 1)}-{ cc.Result[j].Name}");

                      }

                  });
              }
          });

            // Exit
            Exit();
        }

        #region Get continents  from pro-music
        // Get all continents from pro music 
        private async Task<List<Continents>> GetContinentsFromProMusic(string url)
        {

            var continentsList = new List<Continents>();

            try
            {

                WriteInfoToConsole("Start Getting Continents From Pro Music");
                Url proMusicUrl = new Url(url);
                // download Html
                var document = await bContext.OpenAsync(proMusicUrl);

                // write out HTML to console
                // WriteText(document.ToHtml());

                //Extract listing elements
                var listing = document.QuerySelector("#listing");
                if (listing != null)
                {
                    // Getting all the Anchor tag under #listing
                    var continents = listing.QuerySelectorAll("a");

                    foreach (var continent in continents)
                    {
                        // ignor for column4 
                        if (continent.QuerySelector("div") == null)
                        {
                            if (continent.TagName == "A")
                            {
                                var anchorElement = continent as IHtmlAnchorElement;
                                continentsList.Add(new Continents() { Name = anchorElement.TextContent, Href = anchorElement.Href });
                            }
                        }
                    }
                }
            }

            catch (Exception e)
            {

                WriteErrorTextToConsole(e.StackTrace);
            }

            return continentsList;
        }

        #endregion

        #region Contries 
        // Get continent countries 

        private async Task<List<Countries>> GetContinentCountries(string url, string continent)
        {
            var countriesList = new List<Countries>();
            WriteInfoToConsole($"Start Getting countries of {continent}");
            try
            {
                var document = await bContext.OpenAsync(new Url(url));
                //Extract listing elements
                var listings = document.QuerySelectorAll("#listing");

                if (listings.Length > 0)
                {
                    var countries = listings[0].QuerySelectorAll("a");

                    foreach (var country in countries)
                    {
                        if (country.TagName == "A")
                        {
                            var anchorElement = country as IHtmlAnchorElement;
                            countriesList.Add(new Countries() { Name = anchorElement.TextContent, Id = anchorElement.Id });

                            // Get all stations for countrys
                            await DownloadProMusisStationImages(listings[2], anchorElement.Id);
                        }
                    }

                    WriteTextToConsole($"{countries.Count()} countries in  {continent}");
                }

            }

            catch (Exception e)
            {
                WriteErrorTextToConsole(e.StackTrace);
            }
            return countriesList;
        }

        #endregion

        #region Download image form  pro music
        private async Task DownloadProMusisStationImages(IElement stationsWarpper, string country)
        {

            try
            {
                if (stationsWarpper != null)
                {
                    var filterCountry = $".allcat{country}";
                    //Extract stations node
                    var availableStations = stationsWarpper.QuerySelectorAll(filterCountry);

                    foreach (var station in availableStations)
                    {
                        //  Console.WriteLine("{0}", station.ToHtml());
                        string stationName = string.Empty;
                        string stationLogoUrl = string.Empty;
                        byte[] stationImg = null;

                        //Loop through children and get the station info and link
                        foreach (var child in station.Children)
                        {
                            switch (child.TagName)
                            {
                                case "IMG":
                                    var imgElement = child as IHtmlImageElement;
                                    stationLogoUrl = imgElement.Source;
                                    break;
                                case "H1":
                                    stationName = child.TextContent;
                                    break;
                            }
                        }

                        if (stationName != string.Empty && stationLogoUrl != string.Empty)
                        {
                            // Console.WriteLine($"{stationName} {stationLogoUrl}");
                            stationImg = await DownloadStationImageAsync(stationLogoUrl);

                            if (stationImg != null)
                            {
                                StationThumbnail stationThumbnail = new StationThumbnail();
                                stationThumbnail.Name = stationName;

                                List<Thumbnail> sThumbnails = new List<Thumbnail>();
                                Thumbnail sThumb = new Thumbnail();
                                sThumb.Source = "Music Pro";
                                sThumb.Size = "normal";
                                sThumb.data = stationImg;
                                sThumb.Width = 38;
                                sThumb.Height = 38;
                                sThumb.MimeType = "image/jpg";

                                sThumbnails.Add(sThumb);

                                stationThumbnail.Thumbnails = sThumbnails;

                                //Insert station thumb nail
                                await SaveStationImageThumbnailsToDb(stationThumbnail, country);
                            }
                        }
                    }

                    WriteTextToConsole($"{availableStations.Count()} station in {country}");
                }
            }

            catch (Exception e)
            {
                WriteErrorTextToConsole(e.StackTrace);
            }
        }
        #endregion

        #region Save image functions
        private async Task SaveStationImageThumbnailsToDb(StationThumbnail stationThumbnail, string country)
        {

            try
            {
                UpdateOptions updateOptions = new UpdateOptions { IsUpsert = true };
                var filter = Builders<Station>.Filter.Eq(s => s.Name, stationThumbnail.Name);

                Station station = stations.Find(filter).FirstOrDefault();

                if (station == null)
                {
                    //Insert new station into DB
                    station = new Station();
                    station.Name = stationThumbnail.Name;
                    station.Callcode = stationThumbnail.Name;
                    station.SType = "Digital";
                    station.Countries.Add(country);
                    //Insert the new record
                    await stations.InsertOneAsync(station);
                }

                else
                {
                     station.Countries.Add(country);
                    //update station after add new country 
                    await stations.ReplaceOneAsync(filter, station);
                }
                if (station.Id != null)
                {
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
                    await stationImages.ReplaceOneAsync(filterSI, sImage, updateOptions);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while saving image thumbnails of station \"{0}\" to database.\r\nError={1}", stationThumbnail.Name, ex.ToString());
            }
        }

        private async Task<byte[]> DownloadStationImageAsync(string stationImageUrl)
        {
            byte[] imageData = null;


            try
            {

                var request = new DocumentRequest(new Url(stationImageUrl));

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
                Console.WriteLine("No content returned for URL \"{0}\". Error = {1}", stationImageUrl, ex.ToString());
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

        #endregion

        #region Console function

        private void WriteInfoToConsole(string txt)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(txt);

        }

        private void WriteTextToConsole(string txt)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(txt);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void WriteErrorTextToConsole(string txt)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(txt);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void Exit()
        {
            Console.WriteLine("PRSS ANY KEY FOR EXIT");
            Console.ReadLine();
        }
        #endregion
    }
}

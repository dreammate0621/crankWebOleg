using System;
using System.Net.Http;
using Crankdata.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Linq;
using CsvHelper;
using DownloadFCCFMStationData.Models;
using MongoDB.Driver.GeoJsonObjectModel;

namespace DownloadFCCFMStationData.AppCode
{
    public class FCCFMStationInfoDownload: IDisposable
    {
        #region Constants
        const string UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36";
        #endregion

        #region private vaiables
        private HttpClient httpClient;
        private string fccFMStationSearchURL;
        private string fccFMStationCoverageMapURL;
        private CrankdataContext dbContext;
        IMongoCollection<Station> stations = null;
        private bool saveToFileSystem;
        string saveFolderPath;

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
                    if (httpClient != null)
                    {
                        httpClient.Dispose();
                        httpClient = null;
                    }
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

        public FCCFMStationInfoDownload(CrankdataContext dbContext,
                                   string fccFMStationSearchURL,
                                   string fccFMStationCoverageMapURL,

                                   bool saveToFileSystem = false,
                                   string saveFolderPath = "")
        {
            this.dbContext = dbContext;
            this.fccFMStationSearchURL = fccFMStationSearchURL;
            this.fccFMStationCoverageMapURL = fccFMStationCoverageMapURL;
            this.saveToFileSystem = saveToFileSystem;
            this.saveFolderPath = saveFolderPath;
            Init();
        }

        private void Init()
        {
            //TODO: Initialize objects here    

            //Get MongoDB collections
            stations = dbContext.Stations;

            //Initialize the HttpClient 
            httpClient = new HttpClient();

            //Change User Agent string
            httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);


            if(String.IsNullOrEmpty(saveFolderPath))
            {
                saveFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "savefolder");
            }

            if (saveToFileSystem && !Directory.Exists(saveFolderPath))
            {
                Directory.CreateDirectory(saveFolderPath);
            }
        }

        public void Process()
        {
            DownloadFCCFMStatationData();
        }

        private void DownloadFCCFMStatationData()
        {
            try
            {
                Console.WriteLine("Begin FCC FM Station data download");

                string FCCFMStationInfo = GetURLContentString(fccFMStationSearchURL);

                 SaveToFile(FCCFMStationInfo, "FCCFMStationInfo");
                //ParseFMStationData(new StreamReader(@"C:\Temp\FCCData\FCCFMStation\FCCFMStationInfo.txt").ReadToEnd());
                ParseFMStationData(FCCFMStationInfo);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while querying FCC for FM station info using URL \"{0}\".\r\nError={1}", fccFMStationSearchURL, ex.ToString());
            }
        }

        
        private string GetURLContentString(string urlPath)
        {
            string returnData = null;

            try
            {
                Task<HttpResponseMessage> requestTask = httpClient.GetAsync(urlPath);
                //Wait until the request is complete
                requestTask.Wait();

                HttpResponseMessage response = requestTask.Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //We are in business, parse the response
                    //Read the data content 
                    var contentReaderTask = response.Content.ReadAsStringAsync();

                    //Wait until read is complete
                    contentReaderTask.Wait();
                    if (contentReaderTask.IsCompleted)
                    {
                        //Extract the response
                        if (contentReaderTask.Result != null)
                        {
                            returnData = contentReaderTask.Result;
                        }
                        else
                        {
                            Console.WriteLine("No content returned for URL \"{0}\"", urlPath);
                        }
                    }
                }
                else if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    Console.WriteLine("No content returned for URL \"{0}\"", urlPath);
                }

                //Clean up

                response.Dispose();
                requestTask.Dispose();
            }

            catch (HttpRequestException ex)
            {
                Console.WriteLine("Failed during HTTP request to \"{0}\", Error={1}", urlPath, ex.Message);
                throw;
            }

            return returnData;
        }


        private void SaveToFile(string data, string FileName)
        {
            if (saveToFileSystem)
            {
                try
                {
                    string cleanFileName = CleanFileName(FileName);
                    string FilePath = Path.Combine(saveFolderPath, String.Format("{0}.txt", cleanFileName));

                    using (Stream fileStream = File.Open(FilePath, FileMode.Create))
                    using (StreamWriter fileWriter = new StreamWriter(fileStream))
                    {
                        fileWriter.Write(data);
                        fileWriter.Close();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error occured while saving data to file \"{0}\".\r\nError={1}", FileName, ex.ToString());
                }
            }
        }

        private string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        private void ParseFMStationData(string fccFMStationInfo)
        {
            int stationIndex = 0;
            using (var sReader = new StringReader(fccFMStationInfo))
            using(var csvReader = new CsvReader(sReader))
            {
                csvReader.Configuration.Delimiter = "|";
                csvReader.Configuration.IgnoreBlankLines = true;
                csvReader.Configuration.HasHeaderRecord = false;
                csvReader.Configuration.SkipEmptyRecords = true;
                csvReader.Configuration.TrimFields = true;
                csvReader.Configuration.RegisterClassMap<FCCFMStationInfoMap>();

                var allStationRec = csvReader.GetRecords<FCCFMStationInfo>().ToList();

                Console.WriteLine("Found {0} FM stations", allStationRec.Count);

                foreach (var fmStation in allStationRec)
                {
                    fmStation.Frequency = fmStation.Frequency.Replace("  MHz","");
                    fmStation.FileNumber = fmStation.FileNumber.Replace(" ", "");

                    Console.WriteLine("{0}, Processing station \"{1}\", {2}, {3}, {4}", ++stationIndex, fmStation.Call, fmStation.Frequency, fmStation.City, fmStation.State, fmStation.FileNumber);

                    UpdateStaionInfoInDb(fmStation);
                }
            }

        }

        private void GetCoverageMap(FCCFMStationInfo fccStationInfo)
        {

            string stationCoverageMapURL = string.Format(fccFMStationCoverageMapURL,
                                                         fccStationInfo.FCCAppId,
                                                         fccStationInfo.Call,
                                                         fccStationInfo.Frequency,
                                                         fccStationInfo.City,
                                                         fccStationInfo.State,
                                                         fccStationInfo.FileNumber);

            Console.WriteLine("CoverageMapURL = {0}", stationCoverageMapURL);

            //Download the coverage map
            string fmCoverageMap = GetURLContentString(stationCoverageMapURL);

            if(!String.IsNullOrEmpty(fmCoverageMap))
            {
                fccStationInfo.CoverageMap = fmCoverageMap;

                //Save the file if configured
                SaveToFile(fmCoverageMap, string.Format("{0}_{1}_{2}_{3}", fccStationInfo.Call, fccStationInfo.FCCAppId, fccStationInfo.City, fccStationInfo.State));
            }
        }

        private void UpdateStaionInfoInDb(FCCFMStationInfo fccStationInfo)
        {
            //Check whether station exist in the DB
            try
            {
                string fmCallcode = fccStationInfo.Call;
                if(fmCallcode.Contains("-FM"))
                {
                    fmCallcode = fmCallcode.Replace("-FM", "");
                }

                var filter = Builders<Station>.Filter.Eq(s => s.Callcode, fmCallcode);
                Station fmStation = stations.Find(filter).FirstOrDefault();
                if(fmStation == null)
                {
                    //Insert new station into DB
                    GetCoverageMap(fccStationInfo);
                    Station newStation = new Station();
                    newStation.Name = fccStationInfo.Call;
                    newStation.Callcode = fmCallcode;
                    newStation.SType = fccStationInfo.Service;
                    newStation.Frequency = fccStationInfo.Frequency;
                    newStation.Channel = fccStationInfo.Channel;
                    newStation.Class = fccStationInfo.Class;
                    newStation.Status = fccStationInfo.Status;
                    newStation.City = fccStationInfo.City;
                    newStation.State = fccStationInfo.State;
                    newStation.Country = fccStationInfo.Country;
                    newStation.FileNumber = fccStationInfo.FileNumber;
                    newStation.FacilityID = fccStationInfo.FacilityID;
                    newStation.LatDirection = fccStationInfo.LatDirection;
                    newStation.LatDegrees = fccStationInfo.LatDegrees;
                    newStation.LatMinutes = fccStationInfo.LatMinutes;
                    newStation.LatSeconds = fccStationInfo.LatSeconds;
                    newStation.LngDirection = fccStationInfo.LngDirection;
                    newStation.LngDegrees = fccStationInfo.LngDegrees;
                    newStation.LngMinutes = fccStationInfo.LngMinutes;
                    newStation.LngSeconds = fccStationInfo.LngSeconds;
                    newStation.Licensee = fccStationInfo.Licensee;
                    newStation.CoverageMap = fccStationInfo.CoverageMap;
                    newStation.FCCAppId = fccStationInfo.FCCAppId;
                    newStation.LngLat = new GeoJson2DGeographicCoordinates(ConvertDegreeMinutesSecondsToDecimal(fccStationInfo.LngDirection, fccStationInfo.LngDegrees, fccStationInfo.LngMinutes, fccStationInfo.LngSeconds),
                                                                           ConvertDegreeMinutesSecondsToDecimal(fccStationInfo.LatDirection, fccStationInfo.LatDegrees, fccStationInfo.LatMinutes, fccStationInfo.LatSeconds));

                    //Insert the new record
                    stations.InsertOne(newStation);
                }
                else
                {
                    //Station exist, check whether FCC information already exist
                    if(!fmStation.FCCAppId.HasValue)
                    {
                        //Update
                        GetCoverageMap(fccStationInfo);
                        fmStation.Frequency = fccStationInfo.Frequency;
                        fmStation.Channel = fccStationInfo.Channel;
                        fmStation.Class = fccStationInfo.Class;
                        fmStation.Status = fccStationInfo.Status;
                        fmStation.City = fccStationInfo.City;
                        fmStation.State = fccStationInfo.State;
                        fmStation.Country = fccStationInfo.Country;
                        fmStation.FileNumber = fccStationInfo.FileNumber;
                        fmStation.FacilityID = fccStationInfo.FacilityID;
                        fmStation.LatDirection = fccStationInfo.LatDirection;
                        fmStation.LatDegrees = fccStationInfo.LatDegrees;
                        fmStation.LatMinutes = fccStationInfo.LatMinutes;
                        fmStation.LatSeconds = fccStationInfo.LatSeconds;
                        fmStation.LngDirection = fccStationInfo.LngDirection;
                        fmStation.LngDegrees = fccStationInfo.LngDegrees;
                        fmStation.LngMinutes = fccStationInfo.LngMinutes;
                        fmStation.LngSeconds = fccStationInfo.LngSeconds;
                        fmStation.Licensee = fccStationInfo.Licensee;
                        fmStation.CoverageMap = fccStationInfo.CoverageMap;
                        fmStation.FCCAppId = fccStationInfo.FCCAppId;
                        fmStation.LngLat = new GeoJson2DGeographicCoordinates(ConvertDegreeMinutesSecondsToDecimal(fccStationInfo.LngDirection, fccStationInfo.LngDegrees, fccStationInfo.LngMinutes, fccStationInfo.LngSeconds),
                                                                                         ConvertDegreeMinutesSecondsToDecimal(fccStationInfo.LatDirection, fccStationInfo.LatDegrees, fccStationInfo.LatMinutes, fccStationInfo.LatSeconds));

                        var sFilter = Builders<Station>.Filter.Eq(s => s.Id, fmStation.Id);

                        FindOneAndReplaceOptions<Station> options = new FindOneAndReplaceOptions<Station>() { IsUpsert = true };
                        stations.FindOneAndReplace(sFilter, fmStation, options);
                    }
                }
                  
            }
            catch (Exception ex)
            {

                Console.WriteLine("Unable to read FM station informaition from database. Error={0}", ex.ToString());
                throw;
            }
        }

        private double ConvertDegreeMinutesSecondsToDecimal(string direction, int degrees, int minutes, float seconds)
        {
            double retValue = degrees + minutes / 60.0 + seconds / 3600.0;

            return (direction == "N" || direction == "E") ? retValue : -retValue;
        }
    }
}

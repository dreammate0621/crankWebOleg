using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using DownloadSongKickData.AppCode.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;

namespace DownloadSongKickData.AppCode
{
    public class SongKickApi : IDisposable
    {
        private static Logger logger;

        bool disposed = false;

        private string songKickAPIKey;
        private string songKickArtistCalendarURL;
        private string songKickArtistSearchURL;
        private string songKickVenueDetailsURL;

        private HttpClient skHttpClient = new HttpClient();
        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();

        public SongKickApi(string songKickAPIKey,
                           string songKickArtistSearchURL,
                           string songKickArtistCalendarURL,
                           string songKickVenueDetailsURL)
        {
            logger = LogManager.GetCurrentClassLogger();

            this.songKickAPIKey = songKickAPIKey;
            this.songKickArtistSearchURL = songKickArtistSearchURL;
            this.songKickArtistCalendarURL = songKickArtistCalendarURL;
            this.songKickVenueDetailsURL = songKickVenueDetailsURL;

            Init();
        }

        private void Init()
        {
            //Setup JsonSerializerSetting
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if(disposing)
            {
                if(skHttpClient != null)
                {
                    skHttpClient.Dispose();
                    skHttpClient = null;
                }
            }

            disposed = true;
        }

        ~SongKickApi()
        {
            Dispose(false);
        }

        private string BuildArtistSearchURL(string artistName)
        {
            return string.Format(songKickArtistSearchURL, artistName, songKickAPIKey);
        }

        private string BuildArtistEventURL(string artistId, int page=1)
        {
            string eventUrl= string.Format(songKickArtistCalendarURL, artistId, songKickAPIKey);
            eventUrl += String.Format("&page={0}", page);
            return eventUrl;
        }

        private string BuildVenueDetailsURL(int skVenueId)
        {
            return string.Format(songKickVenueDetailsURL, skVenueId, songKickAPIKey);
        }

        public List<SongKickArtist> SearchSongKickArtist(string artistName )
        {
            List<SongKickArtist> searchResult = null;
            SongKickArtistResultsPage songKickArtistResultsPage = null;

            string artistSearchURL = BuildArtistSearchURL(artistName);

            try
            {
                string searchResponse = GetURLContentString(artistSearchURL);
                if(!string.IsNullOrEmpty(searchResponse))
                {
                    //Parse the response
                    try
                    {
                        songKickArtistResultsPage = JsonConvert.DeserializeObject<SongKickArtistResultsPage>(searchResponse, jsonSerializerSettings);

                        if(songKickArtistResultsPage != null)
                        {
                            searchResult = songKickArtistResultsPage.ResultsPage.Results.Artist;
                        }
                    }
                    catch(Exception ex)
                    {
                        logger.Error("Unable to deserialize Artist search result. Error={0}", ex.Message);
                    }
                }
                else
                {
                    logger.Info("Empty response. No artist found");
                }
            }
           
            catch(HttpRequestException ex)
            {
                logger.Error("Failed while searching arist \"{0}\", Error={1}", artistName, ex.Message);
                throw;
            }

            return searchResult;
        }

        private string GetURLContentString(string urlPath)
        {
            string returnData = null;

            try
            {
                Task<HttpResponseMessage> requestTask = skHttpClient.GetAsync(urlPath);
                //Wait until the request is complete
                requestTask.Wait();

                HttpResponseMessage skResponse = requestTask.Result;

                if (skResponse.StatusCode == HttpStatusCode.OK)
                {
                    //We are in business, parse the response
                    //Read the data content 
                    var contentReaderTask = skResponse.Content.ReadAsStringAsync();

                    //Wait until read is complete
                    contentReaderTask.Wait();
                    if (contentReaderTask.IsCompleted)
                    {
                        //Extract the response
                        string requestResponse = contentReaderTask.Result;
                        if(requestResponse != null)
                        {
                            returnData = requestResponse;
                        }
                        else
                        {
                            logger.Info("No content returned for URL \"{0}\"", urlPath);
                        }
                    }
                }
                else if (skResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    logger.Info("No content returned for URL \"{0}\"", urlPath);
                }

                //Clean up

                skResponse.Dispose();
            }

            catch (HttpRequestException ex)
            {
                logger.Error("Failed during HTTP request to \"{0}\", Error={1}", urlPath, ex.Message);
                throw;
            }

            return returnData;
        }

        public SongKickEventResult GetArtistEvents(string artistId, int page = 1 )
        {
            SongKickEventResultsPage eventResultPage = null;
            SongKickEventResult eventResult = null;

            string eventsURL = BuildArtistEventURL(artistId,page);

            try
            {
                string response = GetURLContentString(eventsURL);
                if (!string.IsNullOrEmpty(response))
                {
                    //Parse the response
                    try
                    {
                        eventResultPage = JsonConvert.DeserializeObject<SongKickEventResultsPage>(response, jsonSerializerSettings);

                        if (eventResultPage != null)
                        {
                            eventResult = eventResultPage.ResultsPage;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Unable to deserialize Artist events result. Error={0}", ex.Message);
                    }
                }
                else
                {
                    logger.Info("Empty response. No events found");
                }
            }

            catch (HttpRequestException ex)
            {
                logger.Error("Failed while searching events for arist \"{0}\", Error={1}", artistId, ex.Message);
                throw;
            }

            return eventResult;
        }

        public SongKickVenueResults GetSKVenueDetails(int skVenueId)
        {
            SongKickVenueResults venueResult = null;
            SongKickVenueResultsPage venueResultsPage = null;

            string venueDetailURL = BuildVenueDetailsURL(skVenueId);

            try
            {
                string venueResponse = GetURLContentString(venueDetailURL);
                if (!string.IsNullOrEmpty(venueDetailURL))
                {
                    //Parse the response
                    try
                    {
                        venueResultsPage = JsonConvert.DeserializeObject<SongKickVenueResultsPage>(venueResponse, jsonSerializerSettings);

                        if (venueResultsPage != null)
                        {
                            venueResult = venueResultsPage.ResultsPage;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Unable to deserialize venue detail search result. Error={0}", ex.Message);
                    }
                }
                else
                {
                    logger.Info("Empty response. No venue found");
                }
            }

            catch (HttpRequestException ex)
            {
                logger.Error("Failed while quering Venue details for id \"{0}\", Error={1}", skVenueId, ex.Message);
                throw;
            }

            return venueResult;
        }
    }
}

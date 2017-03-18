using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DownloadArtistDetailFrom7Digital
{
    public class Api : IDisposable
    {
        bool disposed = false;
        private static Logger logger;
        private string oauth_consumer_key;
        private string oauth_consumer_secret;
        private string hostUrl;
        private HttpClient digitalHttpClient;
        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
        const string UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36";

        public Api()
        {
            logger = LogManager.GetCurrentClassLogger();

            oauth_consumer_key = ConfigurationManager.AppSettings["oauth_consumer_key"];
            oauth_consumer_secret = ConfigurationManager.AppSettings["oauth_consumer_secret"];
            hostUrl = ConfigurationManager.AppSettings["hostUrl"];

            digitalHttpClient = new HttpClient();

            //Setup JsonSerializerSetting
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
            jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        private string BulidURLForSearchArtistByName(string name)
        {
            var url = hostUrl + "/search?q={0}&sort=score%20desc&oauth_consumer_key={1}&pagesize=2&&country=ww";
            return string.Format(url, name, oauth_consumer_key);
        }

        private string BulidURLForSearchArtistById(string id, string country = "ww")
        {
            var url = hostUrl + "/details?artistid={0}&country={1}&oauth_consumer_key={2}";

            return string.Format(url, id, country, oauth_consumer_key);
        }

        private string BuildArtitImageUrl(string url, int size = 150)
        {
            var sizeStartIndex = url.IndexOf("_");
            // add different image size to url 
            return url.Remove((sizeStartIndex + 1), 3).Insert((sizeStartIndex + 1), size.ToString());
        }

        public DigitalArtistSearchResult GetArtistByName(string name)
        {
            DigitalArtistSearchResult artists = null;

            var url = BulidURLForSearchArtistByName(name);
            // call 7 digital api to get artist id 
            var searchResultString = GetResponseRawString(url);

            if (!string.IsNullOrEmpty(searchResultString))
            {
                try
                {
                    artists = JsonConvert.DeserializeObject<DigitalArtistSearchResult>(searchResultString, jsonSerializerSettings);
                }
                catch (Exception ex)
                {
                    logger.Error("Unable to deserialize Artist search result. Error={0}", ex.Message);
                }
            }

            else
            {
                logger.Info("Empty response. No artist found");
            }

            return artists;
        }

        public DigitalArtistDetailResult GetArtistById(string id)
        {
            DigitalArtistDetailResult artist = null;

            var url = BulidURLForSearchArtistById(id);
            // call 7 digital api to get artist detail from id 
            var searchResultString = GetResponseRawString(url);

            if (!string.IsNullOrEmpty(searchResultString))
            {
                try
                {
                    artist = JsonConvert.DeserializeObject<DigitalArtistDetailResult>(searchResultString, jsonSerializerSettings);
                }
                catch (Exception ex)
                {
                    logger.Error("Unable to deserialize Artist search result. Error={0}", ex.Message);
                }
            }

            else
            {
                logger.Info("Empty response. No artist found");
            }

            return artist;
        }

        private string GetResponseRawString(string urlPath)
        {
            // clear header and add application/json
            digitalHttpClient.DefaultRequestHeaders.Clear();
            digitalHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string returnData = null;

            try
            {
                Task<HttpResponseMessage> requestTask = digitalHttpClient.GetAsync(urlPath);
                //Wait until the request is complete
                requestTask.Wait();

                HttpResponseMessage digitalResponse = requestTask.Result;

                if (digitalResponse.StatusCode == HttpStatusCode.OK)
                {
                    //We are in business, parse the response
                    //Read the data content 
                    var contentReaderTask = digitalResponse.Content.ReadAsStringAsync();

                    //Wait until read is complete
                    contentReaderTask.Wait();
                    if (contentReaderTask.IsCompleted)
                    {
                        //Extract the response
                        string requestResponse = contentReaderTask.Result;
                        if (requestResponse != null)
                        {
                            returnData = requestResponse;
                        }
                        else
                        {
                            logger.Info("No content returned for URL \"{0}\"", urlPath);
                        }
                    }
                }
                else if (digitalResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    logger.Info("No content returned for URL \"{0}\"", urlPath);
                }

                //Clean up

                digitalResponse.Dispose();
            }

            catch (HttpRequestException ex)
            {
                logger.Error("Failed during HTTP request to \"{0}\", Error={1}", urlPath, ex.Message);
                throw;
            }

            return returnData;
        }

        // Download Artist image from 7digital
        public async Task<byte[]> DownloadArtistImage(string artistImageUrl, int size = 150)
        {
            //Clear header andChange User Agent string
            digitalHttpClient.DefaultRequestHeaders.Clear();
            digitalHttpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);

            byte[] imageData = null;
            try
            {
                var imageUrl = BuildArtitImageUrl(artistImageUrl, size);

                HttpResponseMessage requestTask = await digitalHttpClient.GetAsync(imageUrl);

                HttpResponseMessage skResponse = requestTask;

                if (skResponse.StatusCode == HttpStatusCode.OK)
                {
                    //We are in business, parse the response
                    //Read the data content 
                    var contentReader = await skResponse.Content.ReadAsByteArrayAsync();

                    //Extract the response
                    imageData = contentReader;

                }
                else if (skResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    logger.Error("No content returned for URL \"{0}\"", artistImageUrl);
                }
            }
            catch (HttpRequestException ex)
            {
                logger.Error("Failed during HTTP request to \"{0}\", Error={1}", artistImageUrl, ex.Message);
                throw;
            }

            return imageData;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                if (digitalHttpClient != null)
                {
                    digitalHttpClient.Dispose();
                    digitalHttpClient = null;
                }
            }

            disposed = true;
        }

        ~Api()
        {
            Dispose(false);
        }
    }
}

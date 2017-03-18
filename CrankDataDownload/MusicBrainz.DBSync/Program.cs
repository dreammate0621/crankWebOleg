#region Using Statements

using Crankdata.Models;
using MongoDB.Driver;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using MusicBrainz = Hqub.MusicBrainz.API.Entities;
using System.Threading;
using MusicDBSync.Logging;
using MongoDB.Bson;

#endregion

namespace MusicDBSync
{
    class Program
    {
        static string dbConnectionString = ConfigurationManager.AppSettings["dbConnectionString"];
        static string dbName = ConfigurationManager.AppSettings["dbName"];
        static int SLEEP_SECONDS = Convert.ToInt32(ConfigurationManager.AppSettings["musicBrainzWSCallIntervalSeconds"]);
        static int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["crankDbFetchPageSize"]);

        #region Main Program

        static void Main(string[] args)
        {
            int totalPageNumber = GetTotalPageNumber() + 1;

            Log.Instance.Info("Starting Music Brainz DB Syncing...");

            for (int pageNumber = 1; pageNumber <= totalPageNumber; pageNumber++)
            {
                
                try
                {
                    CrankdataContext dbContext = new CrankdataContext(dbConnectionString, dbName);
                   // var filter = Builders<Artist>.Filter.Empty;
                    var artists = dbContext.Artists;

                    FindOptions<Artist> options = new FindOptions<Artist>
                    {
                        Limit = pageSize,
                        Skip = (pageNumber > 0 ? ((pageNumber - 1) * pageSize) : 0),
                        Sort = Builders<Artist>.Sort.Ascending(a => a.Title)
                    };

                    var cursor = artists.FindAsync<Artist>(new BsonDocument(), options);

                   // filter = Builders<Artist>.Filter.Eq(a => a.Title, "SHINEDOWN");

                    foreach (Artist dbArtist in cursor.Result.ToList())
                    {
                        try
                        {
                            Log.Instance.Info("--------------------------------------Processing artist : Title= \"{0}\", ID={1}, Mbid={2}--------------------------------------", dbArtist.Title, dbArtist.Id, dbArtist.Mbid);
                            MusicBrainz.Artist musicBrainzArtist = null;

                            if (!IfCrankArtistMBidNull(dbArtist))
                            {
                                Log.Instance.Info("Fetching details (search by MBId) from Music Brainz DB");
                                musicBrainzArtist = GetMusicBrainzArtistByMbid(dbArtist.Mbid.ToString()).Result;
                                Log.Instance.Info("Successfully fetched details (search by MBId) from Music Brainz DB");
                            }
                            else
                            {
                                Log.Instance.Info("Fetching details (search by title) from Music Brainz DB");
                                musicBrainzArtist = GetMusicBrainzArtistByTitle(dbArtist.Title).Result;
                                Log.Instance.Info("Successfully fetched details (search by title) from Music Brainz DB");
                            }
                            
                            if (musicBrainzArtist != null)
                            {
                                var newArtist = GetMappedArtistInformation(musicBrainzArtist, dbArtist);
                                UpdateArtist(newArtist, dbContext);
                                Thread.Sleep(SLEEP_SECONDS * 1000);
                            }
                            Log.Instance.Info("Completed processing artist : Title= \"{0}\", ID={1}, Mbid={2}.\r\n", dbArtist.Title, dbArtist.Id, dbArtist.Mbid);
                        }
                        catch (Exception ex)
                        {
                            Log.Instance.Error("Error occured while processing artist : Title={0}\".\r\nError={1}", dbArtist.Title, ex.ToString());
                        }
                    }

                    int percentComplete = (pageNumber / totalPageNumber) * 100;

                    Log.Instance.Info("Music Brainz DB Syncing. Completed " + percentComplete.ToString() + " %...");
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("Exception occured. Error={0}", ex.Message);
                }
            }
            Log.Instance.Info("Completed 100% Music Brainz DB Syncing...");
        }

        static int GetTotalPageNumber()
        {
            CrankdataContext dbContext = new CrankdataContext(dbConnectionString, dbName);
            var filter = Builders<Artist>.Filter.Empty;
            var artists = dbContext.Artists;

            return artists.Find(filter).ToList().Count / pageSize;
        }

        #endregion

        #region CrankDb Methods

        /// <summary>
        /// Check if MBid is null for an artist
        /// </summary>
        /// <param name="inputArtist"></param>
        /// <returns></returns>
        static bool IfCrankArtistMBidNull(Artist inputArtist)
        {
            if(inputArtist.Mbid != null && inputArtist.Mbid.ToString() != "00000000-0000-0000-0000-000000000000")
            {
                return false;
            }
            Log.Instance.Info("Artist MBId missing in Crank DB for Artist : Title={0}, ID={1}, MBId={2}", inputArtist.Title, inputArtist.Id, inputArtist.Mbid);
            return true;
        }

        /// <summary>
        /// Update crank db artist
        /// </summary>
        /// <param name="inputArtist"></param>
        /// <param name="dbContext"></param>
        static void UpdateArtist(Artist crankArtist, CrankdataContext dbContext)
        {
            var aFilter = Builders<Artist>.Filter.Eq(a => a.Id, crankArtist.Id);
            var artists = dbContext.Artists;

            Log.Instance.Info("Currently updating CrankDb for artist...", crankArtist.Title, crankArtist.Id, crankArtist.Mbid);

            //Update below fields
            var aUpdate = Builders<Artist>.Update
                        .Set(a => a.LastName, crankArtist.LastName)
                        .Set(a => a.SType, crankArtist.SType)
                        .Set(a => a.SortName, crankArtist.SortName)
                        .Set(a => a.Gender, crankArtist.Gender)
                        .Set(a => a.IPICode, crankArtist.IPICode)
                        .Set(a => a.Location, crankArtist.Location);

            artists.UpdateOne(aFilter, aUpdate);
            Log.Instance.Info("Successfully updated CrankDb for artist...", crankArtist.Title, crankArtist.Id, crankArtist.Mbid);
        }

        /// <summary>
        /// Map the properties from Music Brainz to Crank db artist
        /// </summary>
        /// <param name="musicBrainzArtist"></param>
        /// <param name="dbArtist"></param>
        /// <returns></returns>
        static Artist GetMappedArtistInformation(MusicBrainz.Artist musicBrainzArtist, Artist crankArtist)
        {
            crankArtist.LastName = musicBrainzArtist.Name;
            crankArtist.SType = musicBrainzArtist.Type;
            crankArtist.SortName = musicBrainzArtist.SortName;
            crankArtist.Gender = musicBrainzArtist.Gender;
            crankArtist.IPICode = musicBrainzArtist.IPI;
            crankArtist.Location = musicBrainzArtist.Country;

            return crankArtist;
        }

        #endregion

        #region MusicBrainz WebService Call Methods

        /// <summary>
        /// Get the Music Braniz artist queried by MB id
        /// </summary>
        /// <param name="mbid"></param>
        /// <returns></returns>
        private static async Task<MusicBrainz.Artist> GetMusicBrainzArtistByMbid(string mbid)
        {
            return await MusicBrainz.Artist.GetAsync(mbid);
        }

        /// <summary>
        /// Get the Music Braniz artist queried by title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private static async Task<MusicBrainz.Artist> GetMusicBrainzArtistByTitle(string title)
        {
            var musicBrainzArtistList = await MusicBrainz.Artist.SearchAsync(title);

            if(musicBrainzArtistList != null && musicBrainzArtistList.Items != null && musicBrainzArtistList.Items.Count > 0)
            {
                return musicBrainzArtistList.Items.OrderByDescending(x => x.Score).FirstOrDefault();
            }
            return null;
        }

        #endregion
    }
}

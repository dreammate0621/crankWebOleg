using Crankdata.Models;
using MongoDB.Driver;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DownloadSongKickData.AppCode.Models;
using System.Collections.Generic;
using System.Collections;
using MongoDB.Bson;
using System.Linq;
using MongoDB.Driver.GeoJsonObjectModel;
using NLog;

namespace DownloadSongKickData.AppCode
{
    public class SongKickApiDownload
    {
        private static Logger logger;

        //MongoDB context and collections
        CrankdataContext dbContext = null;
        IMongoCollection<Artist> artists = null;
        IMongoCollection<Song> songs = null;
        IMongoCollection<Event> events = null;
        IMongoCollection<Venue> venues = null;
        IMongoCollection<MetroArea> metroarea = null;

        string songKickAPIKey = string.Empty;
        string songKickArtistSearchURL = string.Empty;
        string songKickArtistCalendarURL = string.Empty;
        string songKickVenueDetailsURL = string.Empty;

        UpdateOptions eventUpdateOptions = new UpdateOptions { IsUpsert = true };


        private SongKickApi songKickApiObject;


        //Dcitionary for venue lookup
        Dictionary<string, Venue> venueLookupbyName = new Dictionary<string, Venue>();
        Dictionary<int, Venue> venueLookupbySKId = new Dictionary<int, Venue>();


        //Dictionary for MetroArea lookup
        Dictionary<int, MetroArea> metroAreaLookupBySKId = new Dictionary<int, MetroArea>();
        Dictionary<string, MetroArea> metroAreaLookupByName = new Dictionary<string, MetroArea>();


        //Dictionary for aritst lookup
        Dictionary<int, Artist> artistSKIdLookup = new Dictionary<int, Artist>();

        public SongKickApiDownload(CrankdataContext dbContext,
                                   string songKickAPIKey,
                                   string songKickArtistSearchURL,
                                   string songKickArtistCalendarURL,
                                   string songKickVenueDetailsURL)
        {
            logger = LogManager.GetCurrentClassLogger();

            this.dbContext = dbContext;
            this.songKickAPIKey = songKickAPIKey;
            this.songKickArtistSearchURL = songKickArtistSearchURL;
            this.songKickArtistCalendarURL = songKickArtistCalendarURL;
            this.songKickVenueDetailsURL = songKickVenueDetailsURL;
            Init();
        }

        private void Init()
        {
            songKickApiObject = new SongKickApi(songKickAPIKey,
                                                songKickArtistSearchURL,
                                                songKickArtistCalendarURL,
                                                songKickVenueDetailsURL);
            artists = dbContext.Artists;
            songs = dbContext.Songs;
            events = dbContext.Events;
            venues = dbContext.Venues;
            metroarea = dbContext.MetroAreas;
        }

        public void Process()
        {

            ProcessArtistAndEvents();
        }

        private void ProcessArtistEvents(Artist artist)
        {


            SongKickEventResult artistEventResult = null;
            int currentPage = 1;
            int totalEntries = 0;
            int entriesPerPage = 0;
            int eventCount = 0;

            int totalPages = 1;

            bool moreEventLeft = true;

            string skArtistID;

            if (!Guid.Empty.Equals(artist.Mbid))
            {
                skArtistID = string.Format("mbid:{0}", artist.Mbid);
            }
            else
            {
                skArtistID = artist.ArtistSKId.ToString();
            }

            while (moreEventLeft)
            {
                artistEventResult = songKickApiObject.GetArtistEvents(skArtistID, currentPage);

                if (artistEventResult != null)
                {
                    if (currentPage == 1)
                    {
                        if (artistEventResult.TotalEntries > 0)
                        {
                            totalEntries = artistEventResult.TotalEntries;
                            entriesPerPage = artistEventResult.PerPage;

                            totalPages = (int)Math.Ceiling(((double)totalEntries / entriesPerPage));
                            logger.Info("{0} events found found for aritst \"{1}\". Total results page:{2}", totalEntries, artist.Title, totalPages);

                        }
                        else
                        {
                            logger.Info("No events found for artist \"{0}\"", artist.Title);
                            moreEventLeft = false;
                        }
                    }

                    if (artistEventResult.Results.Event != null)
                    {
                        foreach (var artistEvent in artistEventResult.Results.Event)
                        {
                            try
                            {
                                eventCount++;
                                logger.Info("Processing event# {0} for artist \"{1}\", Event = \"{2}\"", eventCount, artist.Title, artistEvent.DisplayName);

                                //If event exist, then update, else insert new
                                Event newEvent = new Event();
                                newEvent.Name = artistEvent.DisplayName;
                                newEvent.EventSKId = artistEvent.Id;
                                newEvent.AgeRestriction = artistEvent.AgeRestriction;
                                newEvent.EventType = artistEvent.Type;
                                newEvent.Popularity = artistEvent.Popularity;
                                newEvent.StartDateTime = artistEvent.Start.DateTime;
                                newEvent.StartDate = artistEvent.Start.Date;
                                newEvent.StartTime = artistEvent.Start.Time;
                                newEvent.Status = artistEvent.Status;

                                newEvent.City = artistEvent.Location.City;
                                if (artistEvent.Location.Lng.HasValue && artistEvent.Location.Lat.HasValue)
                                {
                                    newEvent.LngLat = new GeoJson2DCoordinates(artistEvent.Location.Lng.Value, artistEvent.Location.Lat.Value);
                                }

                                //Verify we have Venue, if not create one

                                Venue eventVenue = LookupVenue(artistEvent.Venue);

                                if (eventVenue == null)
                                {
                                    eventVenue = InsertVenueIntoDb(artistEvent.Venue);
                                }

                                newEvent.EventVenue = eventVenue;
                                newEvent.Performance = new List<Performance>();
                                //Add performance
                                foreach (var perf in artistEvent.Performance)
                                {
                                    Performance newEventPerf = new Performance();
                                    newEventPerf.PerfSKId = perf.Id;
                                    newEventPerf.Billing = perf.Billing;
                                    newEventPerf.BillingIndex = perf.billingIndex;
                                    newEventPerf.ArtistSKId = perf.Artist.Id;
                                    newEventPerf.ArtistName = perf.Artist.DisplayName;

                                    Artist dbArtist = GetArtistBySKId(newEventPerf.ArtistSKId);
                                    if (dbArtist != null)
                                    {
                                        newEventPerf.ArtistId = dbArtist.Id;
                                    }

                                    newEvent.Performance.Add(newEventPerf);
                                }

                                //Insert the Event into DB
                                InsertEventinDb(newEvent);

                            }
                            catch (Exception ex)
                            {
                                logger.Error("Error occured while processing artist \"{0}\" event \"{1}\".\r\nERROR={2}", artist.Title, artistEvent.DisplayName, ex.ToString());
                            }
                        }
                    }

                    if (currentPage < totalPages)
                    {
                        //We have more pages 
                        currentPage++;
                    }
                    else
                    {
                        //Done processing
                        moreEventLeft = false;
                    }
                }
                else
                {
                    //No result
                    moreEventLeft = false;
                }
            }

        }

        public void ProcessArtistAndEvents()
        {
            //This is where all action happens
            //Go through current songs, find the Artist, If Artist doens't has SongKick ID, get the id
            //Once the ID is found, get all the events
            //Parse each event, add/update the events
            //Find all artists from database

            int artistIndex = 0;
            var filter = Builders<Artist>.Filter.Empty;

            // filter = Builders<Artist>.Filter.Eq(a => a.Title, "BLUE OCTOBER");

            foreach (Artist dbArtist in artists.Find(filter).ToList())
            {

                try
                {
                    logger.Info("Processing artist {0}: Name = \"{1}\", ID = {2}, SKId={3}, Mbid={4}", ++artistIndex, dbArtist.Title, dbArtist.Id, dbArtist.ArtistSKId?.ToString(), dbArtist.Mbid);

                    if (!dbArtist.ArtistSKId.HasValue)
                    {
                        FindArtistSKInfo(dbArtist);

                        //If we have Artist SK id, update the record in database
                        if (dbArtist.ArtistSKId.HasValue)
                        {
                            //Update the record
                            logger.Info("Aritst : \"{0}\", SKID : {1}, MBID={2}", dbArtist.Title, dbArtist.ArtistSKId, dbArtist.Mbid);
                            UpdateArtistSKInfoInDb(dbArtist);
                        }
                    }

                    //If we have ArtistID at this time, download the concert informaiton
                    if (dbArtist.ArtistSKId.HasValue)
                    {
                        ProcessArtistEvents(dbArtist);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Error occured while processing artist \"{0}\".\r\nError={1}", dbArtist.Title, ex.ToString());
                }
            }

        }

        private void FindArtistSKInfo(Artist dbArtist)
        {

            try
            {
                //Search artist info 
                List<SongKickArtist> artistResult = songKickApiObject.SearchSongKickArtist(dbArtist.Title);

                if (artistResult != null)
                {
                    logger.Info("Search entry found for aritst \"{0}\"", dbArtist.Title);

                    //Update the SK ID, SK DisplayName and MBIB of the first record if exist
                    //Use the first record
                    if (artistResult.Count > 0)
                    {
                        SongKickArtist foundArtist = artistResult[0];
                        if (foundArtist != null)
                        {
                            dbArtist.ArtistSKId = foundArtist.Id;
                            dbArtist.ArtistSKDisplayName = foundArtist.DisplayName;
                            ArtistIdentifier aIdentifier = foundArtist.Identifier[0];
                            if (aIdentifier != null)
                            {
                                if (!String.IsNullOrEmpty(aIdentifier.Mbid))
                                {
                                    dbArtist.Mbid = new Guid(aIdentifier.Mbid);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occurred while quering SongKick artist infomration for arist \"{0}\". Error={1}", dbArtist.Title, ex.ToString());
            }
        }

        private void UpdateArtistSKInfoInDb(Artist dbArtist)
        {
            var aFilter = Builders<Artist>.Filter.Eq(a => a.Id, dbArtist.Id);

            var aUpdate = Builders<Artist>.Update.Set(a => a.ArtistSKId, dbArtist.ArtistSKId)
                                                 .Set(a => a.ArtistSKDisplayName, dbArtist.ArtistSKDisplayName)
                                                 .Set(a => a.Mbid, dbArtist.Mbid);
            artists.UpdateOne(aFilter, aUpdate);
        }

        private Artist GetArtistBySKId(int artistSKId)
        {
            Artist retArtist = null;

            if (!artistSKIdLookup.ContainsKey(artistSKId))
            {
                //Search in DB
                var artistFilter = Builders<Artist>.Filter.Eq(a => a.ArtistSKId, artistSKId);
                var dbAritst = artists.Find(artistFilter).FirstOrDefault();

                if (dbAritst != null)
                {
                    retArtist = dbAritst;
                    artistSKIdLookup[dbAritst.ArtistSKId.Value] = retArtist;
                }
            }
            else
            {
                retArtist = artistSKIdLookup[artistSKId];
            }

            return retArtist;

        }

        private bool EventExistinDb(int eventSKId)
        {
            var eventFilter = Builders<Event>.Filter.Eq(e => e.EventSKId, eventSKId);
            return events.Find(eventFilter).Count() >= 1 ? true : false;
        }

        private void InsertEventinDb(Event newEvent)
        {
            //If exist update, else insert. Use SkId
            var eventFilter = Builders<Event>.Filter.Eq(e => e.EventSKId, newEvent.EventSKId);
            events.ReplaceOne(eventFilter, newEvent, eventUpdateOptions);
        }

        private Venue LookupVenue(SKVenue skVenue)
        {
            Venue retVenue = null;
            //Look at the cache
            if (skVenue.Id.HasValue && venueLookupbySKId.ContainsKey(skVenue.Id.Value))
            {
                //We have the venue
                retVenue = venueLookupbySKId[skVenue.Id.Value];
            }
            //Don't lookup by name.
            //else if (venueLookupbyName.ContainsKey(skVenue.DisplayName))
            //{
            //    retVenue = venueLookupbyName[skVenue.DisplayName];
            //}
            else
            {
                var venueFilter = Builders<Venue>.Filter.Eq(v => v.VenueSKId, skVenue.Id);
                Venue dbVenue = venues.Find(venueFilter).FirstOrDefault();

                //if (dbVenue == null)
                //{
                //    //Lookup by name
                //    venueFilter = Builders<Venue>.Filter.Eq(v => v.Name, skVenue.DisplayName);
                //    dbVenue = venues.Find(venueFilter).FirstOrDefault();

                //}

                if (dbVenue != null)
                {
                    //Add this into the lookup
                    if (dbVenue.VenueSKId.HasValue)
                    {
                        venueLookupbySKId[dbVenue.VenueSKId.Value] = dbVenue;
                    }

                    //venueLookupbyName[dbVenue.Name] = dbVenue;

                    retVenue = dbVenue;
                }
            }

            return retVenue;
        }

        private Venue InsertVenueIntoDb(SKVenue skVenue)
        {
            Venue newVenue = new Venue();

            //Get venue details form SongKick
            SongKickVenueResults skVenueDetailResult = null;

            if (skVenue.Id.HasValue)
            {
                skVenueDetailResult = songKickApiObject.GetSKVenueDetails(skVenue.Id.Value);
            }
            SKVenue venueDetails = null;

            if (skVenueDetailResult != null)
            {
                venueDetails = skVenueDetailResult.Results.Venue;

                newVenue.Name = venueDetails.DisplayName;
                newVenue.VenueSKId = venueDetails.Id;
                newVenue.Phone = venueDetails.Phone;
                newVenue.Street = venueDetails.Street;
                newVenue.Capacity = venueDetails.Capacity;
                newVenue.Description = venueDetails.Description;
                newVenue.Website = venueDetails.Website;
                newVenue.City = venueDetails.City?.DisplayName;
                newVenue.State = venueDetails.City?.State?.DisplayName;
                newVenue.Country = venueDetails.City?.Country?.DisplayName;

                if (venueDetails.Lat.HasValue && venueDetails.Lng.HasValue)
                {
                    newVenue.LngLat = new GeoJson2DCoordinates(venueDetails.Lng.Value, venueDetails.Lat.Value);
                }
            }
            else
            {
                newVenue.Name = skVenue.DisplayName;
                newVenue.VenueSKId = skVenue.Id;

                if (skVenue.Lat.HasValue && skVenue.Lng.HasValue)
                {
                    newVenue.LngLat = new GeoJson2DCoordinates(skVenue.Lng.Value, skVenue.Lat.Value);
                }
            }

            MetroArea venueMetroArea = LookupMetroArea(skVenue.MetroArea);
            if (venueMetroArea == null)
            {
                venueMetroArea = InsertMetroAreaIntoDb(skVenue.MetroArea);
            }

            newVenue.MetroArea = venueMetroArea;

            venues.InsertOne(newVenue);

            //Add this into lookups
            venueLookupbyName[newVenue.Name] = newVenue;
            if (newVenue.VenueSKId.HasValue)
            {
                venueLookupbySKId[newVenue.VenueSKId.Value] = newVenue;
            }

            return newVenue;
        }


        private MetroArea InsertMetroAreaIntoDb(SKMetroArea skMetroArea)
        {
            MetroArea newMetroArea = new MetroArea();

            newMetroArea.Name = skMetroArea.DisplayName;
            newMetroArea.MetroSKId = skMetroArea.Id;
            newMetroArea.State = skMetroArea.State?.DisplayName;
            newMetroArea.Country = skMetroArea.Country?.DisplayName;

            metroarea.InsertOne(newMetroArea);

            //Add this into lookups
           // metroAreaLookupByName[newMetroArea.Name] = newMetroArea;
            if (newMetroArea.MetroSKId.HasValue)
            {
                metroAreaLookupBySKId[newMetroArea.MetroSKId.Value] = newMetroArea;
            }

            return newMetroArea;
        }

        private MetroArea LookupMetroArea(SKMetroArea skMetroArea)
        {
            MetroArea retMetroArea = null;
            //Look at the cache
            if (metroAreaLookupBySKId.ContainsKey(skMetroArea.Id))
            {
                //We have the venue
                retMetroArea = metroAreaLookupBySKId[skMetroArea.Id];
            }
            //else if (metroAreaLookupByName.ContainsKey(skMetroArea.DisplayName))
            //{
            //    retMetroArea = metroAreaLookupByName[skMetroArea.DisplayName];
            //}
            else
            {
                var metroAreaFilter = Builders<MetroArea>.Filter.Eq(ma => ma.MetroSKId, skMetroArea.Id);
                MetroArea dbMetroArea = metroarea.Find(metroAreaFilter).FirstOrDefault();

                //if (dbMetroArea == null)
                //{
                //    //Lookup by name
                //    metroAreaFilter = Builders<MetroArea>.Filter.Eq(ma => ma.Name, skMetroArea.DisplayName);
                //    dbMetroArea = metroarea.Find(metroAreaFilter).FirstOrDefault();

                //}

                if (dbMetroArea != null)
                {
                    //Add this into the lookup
                    if (dbMetroArea.MetroSKId.HasValue)
                    {
                        metroAreaLookupBySKId[dbMetroArea.MetroSKId.Value] = dbMetroArea;
                    }

                    //metroAreaLookupByName[dbMetroArea.Name] = dbMetroArea;

                    retMetroArea = dbMetroArea;
                }
            }

            return retMetroArea;
        }
    }
}
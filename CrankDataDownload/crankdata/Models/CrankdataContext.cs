using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    public class CrankdataContext
    {
        string dbConnectionString = string.Empty;
        string dbName = string.Empty;

        MongoClient mongoClient = null;
        IMongoDatabase mongoDb;
        public CrankdataContext(string dbConnectionString = "mongodb://localhost:27017", string dbName = "crankdb")
        {
            this.dbConnectionString = dbConnectionString;
            this.dbName = dbName;

            //Call Init()
            Init();
        }

        private void Init()
        {
            mongoClient = new MongoClient(this.dbConnectionString);
            mongoDb = mongoClient.GetDatabase(dbName);
        }

        //Songs Collection
        public IMongoCollection<Song> Songs
        {
            get
            {
                return mongoDb.GetCollection<Song>("songs");
            }
        }

        //Artists collection
        public IMongoCollection<Artist> Artists
        {
            get
            {
                return mongoDb.GetCollection<Artist>("artists");
            }
        }

        public IMongoCollection<ArtistSummary> ArtistSummaries
        {
            get
            {
                return mongoDb.GetCollection<ArtistSummary>("artists");
            }
        }

        //Station collection
        public IMongoCollection<Station> Stations
        {
            get
            {
                return mongoDb.GetCollection<Station>("stations");
            }
        }

        //Station collection
        public IMongoCollection<StationSummary> StationSummaries
        {
            get
            {
                return mongoDb.GetCollection<StationSummary>("stations");
            }
        }

        //User collection
        public IMongoCollection<User> Users
        {
            get
            {
                return mongoDb.GetCollection<User>("users");
            }
        }

        public IMongoCollection<UserDetail> UserDetails
        {
            get
            {
                return mongoDb.GetCollection<UserDetail>("users");
            }
        }

        public IMongoCollection<UserSummary> UserSummaries
        {
            get
            {
                return mongoDb.GetCollection<UserSummary>("users");
            }
        }

        //Country collection
        public IMongoCollection<Country> Countries
        {
            get
            {
                return mongoDb.GetCollection<Country>("countries");
            }
        }

        //Event Collection
        public IMongoCollection<Event> Events
        {
            get
            {
                return mongoDb.GetCollection<Event>("events");
            }
        }

        //Event Collection
        public IMongoCollection<EventSummary> EventSummaries
        {
            get
            {
                return mongoDb.GetCollection<EventSummary>("events");
            }
        }
        //MetroArea collection
        public IMongoCollection<MetroArea> MetroAreas
        {
            get
            {
                return mongoDb.GetCollection<MetroArea>("metroarea");
            }
        }

        //MetroArea collection
        public IMongoCollection<MetroAreaMarketMap> MetroAreaMarketMaps
        {
            get
            {
                return mongoDb.GetCollection<MetroAreaMarketMap>("metroareamarketmap");
            }
        }
        //MetroArea collection
        public IMongoCollection<MetroAreaSummary> MetroAreaSummaries
        {
            get
            {
                return mongoDb.GetCollection<MetroAreaSummary>("metroarea");
            }
        }

        public IMongoCollection<Venue> Venues
        {
            get
            {
                return mongoDb.GetCollection<Venue>("venues");
            }
        }

        public IMongoCollection<VenueSummary> VenueSummaries
        {
            get
            {
                return mongoDb.GetCollection<VenueSummary>("venues");
            }
        }

        public IMongoCollection<VenueImage> VenueImages
        {
            get
            {
                return mongoDb.GetCollection<VenueImage>("venueimages");
            }
        }

        public IMongoCollection<ArtistImage> ArtistImages
        {
            get
            {
                return mongoDb.GetCollection<ArtistImage>("artistimages");
            }
        }

        public IMongoCollection<StationImage> StationImages
        {
            get
            {
                return mongoDb.GetCollection<StationImage>("stationimages");
            }
        }

        public IMongoCollection<UserImage> UserImages
        {
            get
            {
                return mongoDb.GetCollection<UserImage>("userimages");
            }
        }

        public IMongoCollection<Invitation> Invitations
        {
            get
            {
                return mongoDb.GetCollection<Invitation>("invitations");
            }
        }

        public IMongoCollection<Module> Modules
        {
            get
            {
                return mongoDb.GetCollection<Module>("modules");
            }
        }

        public IMongoCollection<ModuleImage> ModuleImages
        {
            get
            {
                return mongoDb.GetCollection<ModuleImage>("modules");
            }
        }

        public IMongoCollection<UserPwd> UserPwds
        {
            get
            {
                return mongoDb.GetCollection<UserPwd>("userpwds");
            }
        }

        public IMongoCollection<Company> Companies
        {
            get
            {
                return mongoDb.GetCollection<Company>("companies");
            }
        }

        public IMongoCollection<CompanyImage> CompanyImages
        {
            get
            {
                return mongoDb.GetCollection<CompanyImage>("companyimages");
            }
        }

        public IMongoCollection<CompanySummary> CompanySummaries
        {
            get
            {
                return mongoDb.GetCollection<CompanySummary>("companies");
            }
        }

        public IMongoCollection<CompanySummary> WorldCityBorder
        {
            get
            {
                return mongoDb.GetCollection<CompanySummary>("WorldCityBorder");
            }
        }

        public IMongoCollection<CompanySummary> WorldPlacepoint
        {
            get
            {
                return mongoDb.GetCollection<CompanySummary>("WorldPlacepoint");
            }
        }

        public IMongoCollection<CompanySummary> WorldStateBorder
        {
            get
            {
                return mongoDb.GetCollection<CompanySummary>("WorldStateBorder");
            }
        }

        public IMongoCollection<CompanySummary> WorldUrbanBorder
        {
            get
            {
                return mongoDb.GetCollection<CompanySummary>("WorldUrbanBorder");
            }
        }

        public IMongoCollection<CompanySummary> WorldUSCityBorder
        {
            get
            {
                return mongoDb.GetCollection<CompanySummary>("WorldUSCityBorder");
            }
        }

        public IMongoCollection<CompanySummary> WorldWorldBorder
        {
            get
            {
                return mongoDb.GetCollection<CompanySummary>("WorldWorldBorder");
            }
        }

        public IMongoCollection<CompanySummary> WorldZipCodeBorder
        {
            get
            {
                return mongoDb.GetCollection<CompanySummary>("WorldZipCodeBorder");
            }
        }

        public IMongoCollection<CompanySummary> ScraperAgency
        {
            get
            {
                return mongoDb.GetCollection<CompanySummary>("ScraperAgency");
            }

        }

        public IMongoCollection<CompanySummary> ScraperMarket
        {
            get
            {
                return mongoDb.GetCollection<CompanySummary>("ScraperMarket");
            }

        }

        public IMongoCollection<EventExtras> EventExtras
        {
            get
            {
                return mongoDb.GetCollection<EventExtras>("eventextras");
            }
        }

        // api access keys collection
        public IMongoCollection<APIAccessKey> APIAccessKeys
        {
            get
            {
                return mongoDb.GetCollection<APIAccessKey>("apiaccesskeys");
            }
        }

        public void RenameCollection(string oldName, string newName)
        {
            try
            {
                var renameOptions = new RenameCollectionOptions() { DropTarget = true };
                mongoDb.RenameCollection(oldName, newName, renameOptions);
            }
            catch
            {

            }
        }

    }
}

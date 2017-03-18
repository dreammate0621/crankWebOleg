using Crankdata.Models;
using MongoDB.Driver;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadArtistDetailFrom7Digital
{
    public class DownloadArtistDetail
    {

        #region private variables
        const string hugeAvatar = "huge_avatar";
        const string largeAvatar = "large_avatar";
        const string mediumAvatar = "medium_avatar";
        private static Logger logger;
        private bool saveImageToFileSystem;
        string imageSaveFolderPath;

        //MongoDB context and collections
        CrankdataContext dbContext = null;
        IMongoCollection<Artist> artists = null;
        IMongoCollection<ArtistImage> artistimages = null;

        Api sevenDigitalApiObject = null;
        #endregion
        public DownloadArtistDetail(CrankdataContext dbContext, string imageSaveFolderPath, bool saveImageOnFileSystem = false)
        {
            logger = LogManager.GetCurrentClassLogger();
            saveImageToFileSystem = saveImageOnFileSystem;
            this.imageSaveFolderPath = imageSaveFolderPath;
            this.dbContext = dbContext;

            Init();
        }

        private void Init()
        {
            sevenDigitalApiObject = new Api();
            artists = dbContext.Artists;
            artistimages = dbContext.ArtistImages;

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
            // start processing on availble artists
            await ProcessArtist();
        }

        //Get Bio from 7digital
        private async Task ProcessArtist()
        {

            var filter = Builders<Artist>.Filter.Empty;

            foreach (Artist dbArtist in artists.Find(filter).ToList())
            {
                try
                {
                    // search aritst by name to get 7digital artist id
                    var digitalArtist = sevenDigitalApiObject.GetArtistByName(dbArtist.ArtistSKDisplayName);

                    // if artist found 
                    if (digitalArtist != null && digitalArtist.SearchResults != null && digitalArtist.SearchResults.SearchResult.Count > 0)
                    {
                        // get artist detail from 7 digital by artist id
                        var digitalArtistDetail = sevenDigitalApiObject.GetArtistById(digitalArtist.SearchResults.SearchResult[0].Artist.Id.ToString());

                        if (digitalArtistDetail != null)
                        {

                            // if artist Bio found 
                            if (!string.IsNullOrEmpty(digitalArtistDetail.Artist.Bio.Text))
                            {
                                // add Bio to artist 
                                dbArtist.Bio = digitalArtistDetail.Artist.Bio.Text;
                                await SaveArtistBioToDatabase(dbArtist);
                            }

                            //Check whether arstist already have thumbmnails downloaded
                            var filterBuilder = Builders<ArtistImage>.Filter;
                            var artistImageFilter = filterBuilder.Eq(aI => aI.ArtistId, dbArtist.Id) & filterBuilder.Size(aI => aI.Images, 4);

                            if (artistimages.Count(artistImageFilter) == 1)
                            {
                                //Images exists
                                Console.WriteLine("All thumbnail images exist for  artist \"{0}\", skipping download", dbArtist.Title);

                            }

                            else
                            {
                                // download artist images from 7digital and save to database
                                var artistThumbnail = await DownloadArtistThumbnail(dbArtist, digitalArtistDetail.Artist.Image);

                                if (artistThumbnail != null)
                                {
                                    await SaveArtistImageThumbnailsToDb(dbArtist, artistThumbnail);
                                }
                            }

                        }

                    }
                }
                catch (Exception e)
                {
                    logger.Error("Processing on {0} for artist detail failed,Error is {1}", dbArtist.ArtistSKDisplayName, e.Message);
                }
            }
        }

        private async Task<ArtistThumbnail> DownloadArtistThumbnail(Artist dbArtist, string imageUrl)
        {
            ArtistThumbnail artistThumbnail = new ArtistThumbnail();
            artistThumbnail.Id = dbArtist.ArtistSKId.Value;
            artistThumbnail.Title = dbArtist.Title;
            byte[] thumbImage;

            try
            {
                //Download artist larger avatar
                thumbImage = await sevenDigitalApiObject.DownloadArtistImage(imageUrl, 150);

                if (thumbImage != null)
                {
                    Thumbnail thumbnail = new Thumbnail();
                    thumbnail.Size = "large";
                    thumbnail.Source = "7digital";
                    thumbnail.Height = 140;
                    thumbnail.Width = 140;
                    thumbnail.data = thumbImage;

                    artistThumbnail.Thumbnails.Add(thumbnail);

                    SaveImageToFile(thumbImage, string.Format("{0}_{1}_{2}", artistThumbnail.Id, dbArtist.ArtistSKDisplayName, thumbnail.Size));
                }

                //Download artist huge avatar
                thumbImage = await sevenDigitalApiObject.DownloadArtistImage(imageUrl, 300);

                if (thumbImage != null)
                {
                    Thumbnail thumbnail = new Thumbnail();
                    thumbnail.Size = "huge";
                    thumbnail.Source = "7digital";
                    thumbnail.Height = 300;
                    thumbnail.Width = 300;
                    thumbnail.data = thumbImage;

                    artistThumbnail.Thumbnails.Add(thumbnail);

                    SaveImageToFile(thumbImage, string.Format("{0}_{1}_{2}", artistThumbnail.Id, dbArtist.ArtistSKDisplayName, thumbnail.Size));
                }
            }
            catch (Exception ex)
            {

                logger.Error("Error occured while downloading image thumbnails from 7digital \"{0}\".\r\nError={1}", imageUrl, ex.ToString());

                throw;
            }
            return artistThumbnail;
        }

        // save artist bio in database 

        private async Task SaveArtistBioToDatabase(Artist artist)
        {
            try
            {
                var filter = Builders<Artist>.Filter.Where(a => a.Id == artist.Id);

                await dbContext.Artists.ReplaceOneAsync(filter, artist);
            }
            catch (Exception e)
            {
                logger.Error("Error is occurred {0} while saving Bio For {1}", e.Message, artist.ArtistSKDisplayName);

            }

        }

        // save Thumbnail to database

        private async Task SaveArtistImageThumbnailsToDb(Artist dbArtist, ArtistThumbnail artistThumbnails)
        {


            ArtistImage aImage = new ArtistImage();
            aImage.ArtistId = dbArtist.Id;

            ImageThumbnail aImageThubnail;

            try
            {

                foreach (var aThumbnail in artistThumbnails.Thumbnails)
                {
                    aImageThubnail = new ImageThumbnail();
                    aImageThubnail.Size = aThumbnail.Size;
                    aImageThubnail.Width = aThumbnail.Width;
                    aImageThubnail.Height = aThumbnail.Height;
                    aImageThubnail.Source = aThumbnail.Source;
                    aImageThubnail.data = aThumbnail.data;

                    aImage.Images.Add(aImageThubnail);
                }

                var filter = Builders<ArtistImage>.Filter.Eq(aI => aI.ArtistId, dbArtist.Id);

                //If exist replace, else insert
                await artistimages.ReplaceOneAsync(filter, aImage);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while saving image thumbnails of artist \"{0}\" to database.\r\nError={1}", dbArtist.Title, ex.ToString());

            }
        }

        // save file to file system
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
                    logger.Error("Error occured while saving image thumbnails to file \"{0}\".\r\nError={1}", FileName, ex.ToString());
                }
            }
        }

        private string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

    }
}

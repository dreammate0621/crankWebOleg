using System;
using System.Net.Http;
using DownloadSongKickArtistImage.AppCode.Models;
using Crankdata.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;
using System.IO;
using System.Linq;

namespace DownloadSongKickArtistImage.AppCode
{
    public class SongKickImageDownload: IDisposable
    {
        #region Image avatars end point
        const string hugeAvatar = "huge_avatar";
        const string largeAvatar = "large_avatar";
        const string mediumAvatar = "medium_avatar";
        const string avatar = "avatar";
        const string UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36";
        #endregion

        #region private vaiables
        private HttpClient skHttpClient;
        private string songKickImageURL;
        private CrankdataContext dbContext;
        IMongoCollection<Artist> artists = null;
        IMongoCollection<ArtistImage> artistimages = null;
        private bool saveImageToFileSystem;
        string imageSaveFolderPath;

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
                    if (skHttpClient != null)
                    {
                        skHttpClient.Dispose();
                        skHttpClient = null;
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




        public SongKickImageDownload(CrankdataContext dbContext,
                                   string songKickImageURL,
                                   bool saveImageToFileSystem = false,
                                   string imageSaveFolderPath = "")
        {
            this.dbContext = dbContext;
            this.songKickImageURL = songKickImageURL;
            this.saveImageToFileSystem = saveImageToFileSystem;
            this.imageSaveFolderPath = imageSaveFolderPath;
            Init();
        }

        private void Init()
        {
            //TODO: Initialize objects here    

            //Get MongoDB collections
            artists = dbContext.Artists;
            artistimages = dbContext.ArtistImages;


            //Initialize the HttpClient 
            skHttpClient = new HttpClient();

            //Change User Agent string
            skHttpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);


            if(String.IsNullOrEmpty(imageSaveFolderPath))
            {
                imageSaveFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
            }

            if (saveImageToFileSystem && !Directory.Exists(imageSaveFolderPath))
            {
                Directory.CreateDirectory(imageSaveFolderPath);
            }
        }

        public void Process()
        {
            DownloadArtistImages();
        }

        private void DownloadArtistImages()
        {
            int artistIndex = 0;
            var filter = Builders<Artist>.Filter.Empty;
            var projection = Builders<Artist>.Projection;
            var artistFields = projection.Include(a => a.Title).Include(a => a.ArtistSKId).Include(a => a.ArtistSKDisplayName);
               

            // filter = Builders<Artist>.Filter.Eq(a => a.Title, "BLUE OCTOBER");

            foreach (ArtistMinInfo dbArtist in artists.Find(filter).Project<ArtistMinInfo>(artistFields).ToList()) 
            {

                try
                {
                    Console.WriteLine("Processing artist {0}: Name = \"{1}\", ID = {2}, SKId={3}", ++artistIndex, dbArtist.Title, dbArtist.Id, dbArtist.ArtistSKId?.ToString());

                    //Check whether arstist already have thumbmnails downloaded

                    var filterBuilder = Builders<ArtistImage>.Filter;
                    var artistImageFilter = filterBuilder.Eq(aI => aI.ArtistId, dbArtist.Id) & filterBuilder.Size(aI => aI.Images, 4 );

                   if (artistimages.Count(artistImageFilter) == 1)
                    {
                        //Images exists
                        Console.WriteLine("All thumbnail images exist for  artist \"{0}\", skipping download", dbArtist.Title);

                    }
                    else
                    {
                        //If we have ArtistID at this time, download the concert informaiton
                        if (dbArtist.ArtistSKId.HasValue)
                        {
                            DownlaodAndProcessArtistImage(dbArtist);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured while processing artist \"{0}\".\r\nError={1}", dbArtist.Title, ex.ToString());
                }
            }
        }

        private void DownlaodAndProcessArtistImage(ArtistMinInfo dbArtist)
        {
            //Build Image url
            //Download all images
            //Save all iamges to database

            try
            {
                ArtistThumbnail artistThumbnails = DownloadArtistThumbnail(dbArtist);

                if(artistThumbnails != null)
                {
                    SaveArtistImageThumbnailsToDb(dbArtist,artistThumbnails);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error occured while downloading image thumbnails of artist \"{0}\".\r\nError={1}", dbArtist.Title, ex.ToString());
            }
        }

        private void SaveArtistImageThumbnailsToDb(ArtistMinInfo dbArtist, ArtistThumbnail artistThumbnails)
        {

            
            ArtistImage aImage = new ArtistImage();
            aImage.ArtistId = dbArtist.Id;

            ImageThumbnail aImageThubnail;
            UpdateOptions updateOptions = new UpdateOptions { IsUpsert = true };

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
                artistimages.ReplaceOne(filter, aImage, updateOptions);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured while saving image thumbnails of artist \"{0}\" to database.\r\nError={1}", dbArtist.Title, ex.ToString());

            }
        }

        private Uri BuildArtitImageUrl(string artistSKId, string avatar)
        {
            return  new Uri(string.Format(songKickImageURL, artistSKId, avatar));
        }

        private ArtistThumbnail DownloadArtistThumbnail(ArtistMinInfo dbArtist)
        {
            ArtistThumbnail artistThumbnail = new ArtistThumbnail();
            artistThumbnail.Id = dbArtist.ArtistSKId.Value;
            artistThumbnail.Title = dbArtist.Title;
            byte[] thumbImage;

            //Download artist huge avatar
            Uri artistImageUrl = BuildArtitImageUrl(artistThumbnail.Id.ToString(), hugeAvatar);
            thumbImage = DownloadArtistImage(artistImageUrl);

            if(thumbImage != null)
            {
                Thumbnail thumbnail = new Thumbnail();
                thumbnail.Size = "huge";
                thumbnail.Source = "SongKick";
                thumbnail.Height = 300;
                thumbnail.Width = 300;
                thumbnail.data = thumbImage;

                artistThumbnail.Thumbnails.Add(thumbnail);
                SaveImageToFile(thumbImage, string.Format("{0}_{1}_{2}", artistThumbnail.Id, dbArtist.ArtistSKDisplayName, thumbnail.Size));
            }


            artistImageUrl = BuildArtitImageUrl(artistThumbnail.Id.ToString(), largeAvatar);
            thumbImage = DownloadArtistImage(artistImageUrl);

            if (thumbImage != null)
            {
                Thumbnail thumbnail = new Thumbnail();
                thumbnail.Size = "large";
                thumbnail.Source = "SongKick";
                thumbnail.Height = 140;
                thumbnail.Width = 140;
                thumbnail.data = thumbImage;

                artistThumbnail.Thumbnails.Add(thumbnail);
                SaveImageToFile(thumbImage, string.Format("{0}_{1}_{2}", artistThumbnail.Id, dbArtist.ArtistSKDisplayName, thumbnail.Size));
            }

            artistImageUrl = BuildArtitImageUrl(artistThumbnail.Id.ToString(), avatar);
            thumbImage = DownloadArtistImage(artistImageUrl);

            if (thumbImage != null)
            {
                Thumbnail thumbnail = new Thumbnail();
                thumbnail.Size = "normal";
                thumbnail.Source = "SongKick";
                thumbnail.Height = 50;
                thumbnail.Width = 50;
                thumbnail.data = thumbImage;

                artistThumbnail.Thumbnails.Add(thumbnail);
                SaveImageToFile(thumbImage, string.Format("{0}_{1}_{2}", artistThumbnail.Id, dbArtist.ArtistSKDisplayName, thumbnail.Size));
            }

            artistImageUrl = BuildArtitImageUrl(artistThumbnail.Id.ToString(), mediumAvatar);
            thumbImage = DownloadArtistImage(artistImageUrl);

            if (thumbImage != null)
            {
                Thumbnail thumbnail = new Thumbnail();
                thumbnail.Size = "small";
                thumbnail.Source = "SongKick";
                thumbnail.Height = 31;
                thumbnail.Width = 31;
                thumbnail.data = thumbImage;

                artistThumbnail.Thumbnails.Add(thumbnail);
                SaveImageToFile(thumbImage, string.Format("{0}_{1}_{2}", artistThumbnail.Id, dbArtist.ArtistSKDisplayName, thumbnail.Size));
            }
            return artistThumbnail;



        }

        private byte[] DownloadArtistImage(Uri artistImageUrl)
        {
            byte[] imageData = null;

            Task<HttpResponseMessage> requestTask = skHttpClient.GetAsync(artistImageUrl);
            //Wait until the request is complete
            requestTask.Wait();

            HttpResponseMessage skResponse = requestTask.Result;

            if (skResponse.StatusCode == HttpStatusCode.OK)
            {
                //We are in business, parse the response
                //Read the data content 
                var contentReaderTask = skResponse.Content.ReadAsByteArrayAsync();

                //Wait until read is complete
                contentReaderTask.Wait();
                if (contentReaderTask.IsCompleted)
                {
                    //Extract the response
                    imageData = contentReaderTask.Result;
                }
            }
            else if (skResponse.StatusCode == HttpStatusCode.NoContent)
            {
                Console.WriteLine("No content returned for URL \"{0}\"", artistImageUrl.AbsolutePath);
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
                catch(Exception ex)
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

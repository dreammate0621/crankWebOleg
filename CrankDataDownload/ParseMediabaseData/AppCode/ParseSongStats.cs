using Crankdata.Models;
using CsvHelper;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using ParseMediabaseData.AppCode.Utils;
using ParseMediabaseData.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseMediabaseData.AppCode
{
    public class ParseSongStats
    {
        CrankdataContext dbContext = null;
        IMongoCollection<Song> songs = null;
        IMongoCollection<Artist> artists = null;

        Dictionary<string, Song> parsedSongs = new Dictionary<string, Song>();
        Dictionary<string, Artist> foundArtists = new Dictionary<string, Artist>();

        string csvFolderPath = null;
        string csvArchivePath = null;

        string songCollectionName = "songs";

        public ParseSongStats( CrankdataContext dbContext, string csvFolderPath, string csvArchivePath)
        {
            this.dbContext = dbContext;
            this.csvFolderPath = csvFolderPath;
            this.csvArchivePath = csvArchivePath;
            Init();
        }

        private void Init()
        {
            songs = dbContext.Songs;
            artists = dbContext.Artists;
        }

        public void Process()
        {
            //Verify whether the csvFolderPath exists
            if(!Directory.Exists(csvFolderPath))
            {
                throw new DirectoryNotFoundException(string.Format("Invalid CSV folder path: {0}", csvFolderPath));
            }

            //Enumerate all files in the folder
            var csvFiles = Directory.EnumerateFiles(csvFolderPath, "*.csv").ToList<string>();

            if(csvFiles.Count <= 0)
            {
                Console.WriteLine("No files found in the folder {0}. Verify file exists before running the app", csvFolderPath);
            }

            foreach (var csvFile in csvFiles)
            {
                ProcessSongsStatsFromFile(csvFile);
            }

            //All files have been parsed
            SaveArtistsToDb();
            UpdateArtistsId();
            RenameSongCollection();
            SaveSongStatsToDb();

            //Archive processed files
            FileHelper.ArchiveFiles(csvFolderPath, csvArchivePath, "*.csv", true);
        }

        private void RenameSongCollection()
        {
            string archiveSongCollection = String.Format("{0}_{1}", songCollectionName,DateTime.Now.ToString(("yyyyMMdd")));
            dbContext.RenameCollection(songCollectionName, archiveSongCollection);
        }

        private void UpdateArtistsId()
        {
            foreach(string songArtistKey in parsedSongs.Keys)
            {
                Artist artist = foundArtists[songArtistKey];

                Song song = parsedSongs[songArtistKey];
                song.ArtistId = artist.Id;
            }
        }

        private void SaveArtistsToDb()
        {
            if (foundArtists.Count > 0)
            {
                try
                {
                    foreach(var artist in foundArtists.Values)
                    {
                        var filter = Builders<Artist>.Filter.Eq(a => a.Title, artist.Title);

                        var artistInDb = artists.Find(filter).SingleOrDefault();

                        if(artistInDb == null)
                        {
                            //Insert Artist
                            artists.InsertOne(artist);
                        }
                        else
                        {
                            artist.Id = artistInDb.Id;
                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Unable to write Artists informaition to database. Error={0}", ex.ToString());
                    throw;
                }
            }
        }

        private void SaveSongStatsToDb()
        {
            if (parsedSongs.Count > 0)
            {
                try
                {
                    //Insert Artist ID before inserting the song data
                    songs.InsertMany(parsedSongs.Values);
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Unable to write the Songs stats to database. Error={0}", ex.ToString());
                    throw;
                }
            }
        }

        private void ProcessSongsStatsFromFile(string csvFile)
        {
            int parsedInt;
            //Extract file name without extension
            string SongFormatFromFileName = Path.GetFileNameWithoutExtension(csvFile);
            string parsedSongFormat = string.Empty;

            try
            {


                //Make sure the file exist
                if (File.Exists(csvFile))
                {
                    using (var sReader = new StreamReader(csvFile))
                    using (var csvParser = new CsvParser(sReader))
                    {
                        csvParser.Configuration.HasHeaderRecord = true;
                        csvParser.Configuration.IgnoreBlankLines = true;
                        csvParser.Configuration.IgnoreHeaderWhiteSpace = true;
                        csvParser.Configuration.IsHeaderCaseSensitive = false;
                        csvParser.Configuration.IgnoreReadingExceptions = true;
                        csvParser.Configuration.TrimHeaders = true;
                        csvParser.Configuration.TrimFields = true;


                        var row = csvParser.Read();

                        if (row != null)
                        {
                            //Read first row and extract the Format from the CSV
                            if (csvParser.RawRow == 1)
                            {
                                //Extract Chart type
                                parsedSongFormat = row.GetValue(2).ToString();
                                if (!String.IsNullOrEmpty(parsedSongFormat))
                                {
                                    parsedSongFormat = SongFormatFromFileName;
                                }
                            }

                            //Read second empty row
                            row = csvParser.Read();

                            //Reached 
                            if (row != null && csvParser.RawRow == 2)
                            {
                                //Now construct CsvReader to parse the rest of the CSV
                                using (var csvReader = new CsvReader(csvParser))
                                {
                                    var records = csvReader.GetRecords<ArtistSpin>().ToList();

                                    foreach (var artistspin in records)
                                    {
                                        Console.WriteLine("Processing song: {0}, artist:{1}", artistspin.Title, artistspin.Artist);
                                        string OrigTitle = artistspin.Title;

                                        Song songData = null;

                                        string songTitle;
                                        SortedSet<string> subArtists;
                                        SortedSet<string> labels;
                                        string songAritstKey = string.Format("{0}:{1}", artistspin.Artist, artistspin.Title);

                                        //Verify whether this song already exists
                                        if (parsedSongs.ContainsKey(songAritstKey))
                                        {
                                            songData = parsedSongs[songAritstKey];
                                        }
                                        else
                                        {
                                            songData = new Song();
                                            //Copy all data from ArtistSpin to Song class
                                            ParseTitle(artistspin.Title, out songTitle, out subArtists);
                                            ParseLabels(artistspin.Label, out labels);

                                            songData.Title = songTitle;
                                            songData.OrigLabels = artistspin.Label;
                                            songData.OrigTitle = artistspin.Title;
                                            songData.Artist = artistspin.Artist;
                                            songData.Charts = parsedSongFormat;
                                            songData.SubArtists = subArtists;
                                            songData.Labels = labels;

                                            //Add song stats to parsedSongs collection
                                            parsedSongs[songAritstKey] = songData;

                                            //Create Artist object
                                            Artist artist = new Artist();
                                            artist.Title = songData.Artist;

                                            //Add new Artist to foundArtist collecction
                                            foundArtists[songAritstKey] = artist;

                                        }


                                        //Assign song stats based on the format
                                        SongStats songStats = new SongStats();
                                        songStats.Format = parsedSongFormat;
                                        songStats.SpinStat.OverNightSpins = artistspin.OVN;
                                        songStats.SpinStat.PeakSpins = artistspin.Peak;
                                        songStats.SpinStat.Spins = artistspin.TW;
                                        songStats.SpinStat.AMSpins = artistspin.AMD;
                                        songStats.SpinStat.MidSpins = artistspin.MID;
                                        songStats.SpinStat.EveningSpins = artistspin.EVE;
                                        songStats.SpinStat.StationsOn = artistspin.StationsOn;
                                        songStats.SpinStat.NewStations = artistspin.NewStations;
                                        songStats.SpinStat.Rank = artistspin.RankTW;
                                        songStats.SpinStat.PeakRank = (Int32.TryParse(artistspin.PeakRank, out parsedInt)) ? parsedInt : -1;
                                        songStats.SpinStat.RankLastWeek = (Int32.TryParse(artistspin.RankLW, out parsedInt)) ? parsedInt : -1;

                                        songData.Stats.Add(songStats);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid file path {0}", csvFile);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception occured while reading file {0}. Error={1}", csvFile, ex.ToString());
            }
        }

        private void ParseTitle(string Title, out string outTitle, out SortedSet<string> subArtists)
        {
            outTitle = Title;
            subArtists = new SortedSet<string>();
            int markerPosition = Title.IndexOf(" f/");

            if (markerPosition >= 0)
            {
                //Sub artist exists, extract
                outTitle = Title.Substring(0, markerPosition);
                subArtists.Add(Title.Substring(markerPosition + 3));
            }
        }

        private void ParseLabels(string Labels, out SortedSet<string> outLabels)
        {
            outLabels = new SortedSet<string>();
            foreach (var label in Labels.Split(new char[] { '/' }).ToArray())
            {
                outLabels.Add(label);
            }
        }
    }
}

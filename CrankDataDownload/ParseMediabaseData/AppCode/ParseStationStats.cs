using Crankdata.Models;
using CsvHelper;
using MongoDB.Driver;
using ParseMediabaseData.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ParseMediabaseData.AppCode.Utils;

namespace ParseMediabaseData.AppCode
{
    public class ParseStationStats
    {
        CrankdataContext dbContext = null;
        IMongoCollection<Station> stations = null;

        Dictionary<string, Station> parsedStation = new Dictionary<string, Station>();

        UpdateOptions eventUpdateOptions = new UpdateOptions { IsUpsert = true };

        string csvFolderPath = null;
        string csvArchivePath = null;


        public ParseStationStats( CrankdataContext dbContext, string csvFolderPath, string csvArchivePath)
        {
            this.dbContext = dbContext;
            this.csvFolderPath = csvFolderPath;
            this.csvArchivePath = csvArchivePath;
            Init();
        }

        private void Init()
        {
            stations = dbContext.Stations;
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
                ProcessStationStatsFromFile(csvFile);
            }

            //All files have been parsed

            SaveStationsToDb();

            //Archive processed files
            FileHelper.ArchiveFiles(csvFolderPath, csvArchivePath, "*.csv", true);
               
        }

        private void SaveStationsToDb()
        {
            if (parsedStation.Count > 0)
            {
                try
                {
                    foreach(var station in parsedStation.Values)
                    {
                        var filter = Builders<Station>.Filter.Eq(s => s.Callcode, station.Callcode);
                        
                        //Upsert Station data
                        stations.ReplaceOne(filter, station, eventUpdateOptions);
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Unable to write station informaition to database. Error={0}", ex.ToString());
                    throw;
                }
            }
        }

     
        private void ProcessStationStatsFromFile(string csvFile)
        {
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
                            //Read second empty row
                            row = csvParser.Read();
                            row = csvParser.Read();
                            row = csvParser.Read();

                            //Reached 
                            if (row != null && csvParser.RawRow == 4)
                            {
                                //Now construct CsvReader to parse the rest of the CSV
                                using (var csvReader = new CsvReader(csvParser))
                                {
                                    var records = csvReader.GetRecords<StationRank>().ToList();

                                    foreach (var stationRank in records)
                                    {
                                        Console.WriteLine("Processing station: {0}, Format:{1}", stationRank.Station, stationRank.Format);
                                       
                                        Station stationData = null;


                                        string stationName = stationRank.Station;

                                        //Verify whether this song already exists
                                        if (!parsedStation.ContainsKey(stationName))
                                        {
                                            stationData = new Station();
                                            //Copy all data from ArtistSpin to Song class

                                            stationData.Name = stationRank.Station;
                                            stationData.Rank = stationRank.Rank;
                                            stationData.Market = stationRank.Market;
                                            stationData.Format = stationRank.Format;
                                            stationData.AQH = stationRank.AQH;
                                            stationData.Owner = stationRank.Owner;
                                            stationData.Phone = stationRank.Phone;
                                            stationData.Callcode = stationRank.Station;
                                            ParseStationCallCodeType(stationRank.Station, stationRank.Market, stationData);
                                            stationData.FirstMonitored = stationRank.FirstMonitored;

                                            //Add station stats to parsedStation collection
                                            parsedStation[stationName] = stationData;
                                        }

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

        private void ParseStationCallCodeType(string StationName, string Market, Station StationData )
        {
            StationData.Callcode = StationName;
            StationData.SType = "FM";
            if(Market.StartsWith(">"))
            {
                StationData.SType = "Digital";
                StationData.Market = Market.Replace(">", "").Trim();
            }
            else if(StationName.Contains("-"))
            {
                string[] stationComponents = StationName.Split(new[] { '-' });
                if(stationComponents.Length == 2)
                {
                    StationData.Callcode = stationComponents[0];
                    StationData.SType = stationComponents[1];
                }
            }
        }
    }
}

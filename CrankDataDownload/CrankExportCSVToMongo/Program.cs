using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrankExportCSVToMongo.Models;
using MongoDB.Driver;
using System.IO;
using System.Globalization;
using MongoDB.Driver.GeoJsonObjectModel;
using System.Data.Spatial;
using CsvHelper;

namespace CrankExportCSVToMongo
{


    class Program
    {

        public static double GetLatitude(string wkt)
        {
            if (wkt.Contains("MULTIPOINT"))
            {
                string Multipoint = wkt.Split(';')[1].Replace("MULTIPOINT(", " ").Replace(")", " ").Replace('\"', ' ').TrimStart().TrimEnd();
                return double.Parse(Multipoint.Split(' ')[1]);

            }
            else
            {
                string Multipoint = wkt.Split(';')[1].Replace("POINT(", " ").Replace(")", " ").Replace('\"', ' ').TrimStart().TrimEnd();
                return double.Parse(Multipoint.Split(' ')[1]);

            }


        }


        public static double GetLongitude(string wkt)
        {
            if (wkt.Contains("MULTIPOINT"))
            {
                string Multipoint = wkt.Split(';')[1].Replace("MULTIPOINT(", " ").Replace(")", " ").Replace('\"', ' ').TrimStart().TrimEnd();
                return double.Parse(Multipoint.Split(' ')[0]);

            }
            else
            {
                string Multipoint = wkt.Split(';')[1].Replace("POINT(", " ").Replace(")", " ").Replace('\"', ' ').TrimStart().TrimEnd();
                return double.Parse(Multipoint.Split(' ')[0]);

            }

        }


        private static CsvReader ReadCSVContent(string path)
        {
            var csvRow = System.IO.File.ReadAllLines(path);
            var textReader = new StreamReader(path);
            CsvReader csv = new CsvReader(textReader);
            return csv;
        }
        static List<T> ParseCSVToTable<T>(string path)
        {
            List<T> list = new List<T>();
            list = ReadCSVContent(path).GetRecords<T>().ToList();
            return list;
        }


        public static GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates> GetMultiPolyGon(string text)
        {
            var polylist = text.Replace("MULTIPOLYGON(((", " ").TrimStart().Split(')');

            List<string> modifiedPolyList = new List<string>();

            foreach (var mpl in polylist)
            {
                if (mpl != "")
                {
                    modifiedPolyList.Add(mpl.Replace(",((", " ").TrimStart().TrimEnd());
                }

            }


            List<GeoJsonPolygonCoordinates<GeoJson2DGeographicCoordinates>> geojsonlist = new List<GeoJsonPolygonCoordinates<GeoJson2DGeographicCoordinates>>();
            foreach (var mpl in modifiedPolyList)
            {
                var polygon = mpl.Replace(",(", " ").TrimStart().TrimEnd().Split(',');
                List<GeoJson2DGeographicCoordinates> linearRing = new List<MongoDB.Driver.GeoJsonObjectModel.GeoJson2DGeographicCoordinates>();
                foreach (var p in polygon)
                {

                    var points = p.Split(' ');
                    if (points.Count() == 2)
                    {
                        double lat = double.Parse(points[1]);
                        double lang = double.Parse(points[0]);
                        linearRing.Add(new GeoJson2DGeographicCoordinates(lang, lat));
                    }

                }

                if (linearRing.Count() > 2)
                {
                    GeoJsonLinearRingCoordinates<GeoJson2DGeographicCoordinates> geolinerring = new GeoJsonLinearRingCoordinates<GeoJson2DGeographicCoordinates>(linearRing);
                    GeoJsonPolygonCoordinates<GeoJson2DGeographicCoordinates> poly = new GeoJsonPolygonCoordinates<GeoJson2DGeographicCoordinates>(geolinerring);
                    geojsonlist.Add(poly);
                }



            }

            GeoJsonMultiPolygonCoordinates<GeoJson2DGeographicCoordinates> multiPolyCords = new GeoJsonMultiPolygonCoordinates<GeoJson2DGeographicCoordinates>(geojsonlist);
            GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates> mulitpoly = new GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates>(multiPolyCords);

            return mulitpoly;

        }

        /// <summary>
        /// world_cityborder, world_placepoint,  world_uscityborder,
        /// scrape_market, spatial_ref_sysscrape_agency,         
        /// world_countyborder, 
        /// world_stateborder, world_urbanborder, 
        /// world_worldborder, 
        /// world_zipcodeborder, 
        /// </summary>
        /// <param name="args"></param>

        static void Main(string[] args)
        {

            //var poly = "MULTIPOLYGON(((-121.793060751481 36.9651509144984,-121.795166751481 36.9712529144984,-121.771658751481 36.9712529144984,-121.768661751481 36.9697499144984,-121.765763751481 36.9693539144984,-121.765061751481 36.9704519144984,-121.764863751481 36.9723509144984,-121.764260751481 36.9726479144984,-121.755458751481 36.9728549144984,-121.750859751481 36.9755549144984,-121.742363751481 36.9760499144984,-121.742858751481 36.9739529144984,-121.745432751481 36.9705869144984,-121.749230751481 36.9671399144984,-121.747592751481 36.9645029144984,-121.745054751481 36.9551159144984,-121.739960751481 36.9546479144984,-121.736558751481 36.9580499144984,-121.731257751481 36.9614519144984,-121.729655751481 36.9579509144984,-121.721456751481 36.9510479144984,-121.717460751481 36.9462509144984,-121.731959751481 36.9322559144984,-121.733759751481 36.9269549144984,-121.731761751481 36.9210509144984,-121.732481751481 36.9199259144984,-121.733300751481 36.9183329144984,-121.735559751481 36.9171539144984,-121.738160751481 36.9169559144984,-121.743263751481 36.9151559144984,-121.745558751481 36.9110519144984,-121.745576751481 36.9091889144984,-121.743929751481 36.9091799144984,-121.736045751481 36.9084419144984,-121.736162751481 36.9042569144984,-121.735559751481 36.8983529144984,-121.747655751481 36.8953559144984,-121.749014751481 36.8955179144984,-121.749995751481 36.8967059144984,-121.751057751481 36.9009539144984,-121.754162751481 36.9029519144984,-121.757186751481 36.8997659144984,-121.760138751481 36.8945909144984,-121.766132751481 36.8875889144984,-121.772459751481 36.8837549144984,-121.774160751481 36.8942579144984,-121.776653751481 36.8983889144984,-121.777661751481 36.9000539144984,-121.780163751481 36.9040499144984,-121.782404751481 36.9076589144984,-121.785365751481 36.9120509144984,-121.786859751481 36.9143549144984,-121.790360751481 36.9196559144984,-121.793465751481 36.9224549144984,-121.797758751481 36.9227519144984,-121.796966751481 36.9244529144984,-121.799666751481 36.9262529144984,-121.801763751481 36.9282509144984,-121.806866751481 36.9348569144984,-121.808063751481 36.9375569144984,-121.809566751481 36.9438569144984,-121.804661751481 36.9433529144984,-121.798964751481 36.9444509144984,-121.791566751481 36.9504539144984,-121.787057751481 36.9489509144984,-121.784663751481 36.9471509144984,-121.783943751481 36.9445949144984,-121.777265751481 36.9408509144984,-121.775762751481 36.9408509144984,-121.774565751481 36.9417509144984,-121.773458751481 36.9414539144984,-121.770263751481 36.9390509144984,-121.769462751481 36.9409499144984,-121.766762751481 36.9457559144984,-121.775159751481 36.9519569144984,-121.791665751481 36.9627479144984,-121.793060751481 36.9651509144984)),((-121.746863751481 36.8815499144984,-121.747655751481 36.8953559144984,-121.718756751481 36.9006569144984,-121.735658751481 36.8973539144984,-121.734821751481 36.8866079144984,-121.730951751481 36.8835749144984,-121.730159751481 36.8841509144984,-121.730357751481 36.8827559144984,-121.738745751481 36.8803979144984,-121.745855751481 36.8791199144984,-121.746026751481 36.8786519144984,-121.744136751481 36.8674919144984,-121.740257751481 36.8661509144984,-121.738853751481 36.8690489144984,-121.737917751481 36.8723879144984,-121.737674751481 36.8766449144984,-121.738106751481 36.8777339144984,-121.734137751481 36.8793089144984,-121.724462751481 36.8799569144984,-121.722842751481 36.8763659144984,-121.722248751481 36.8652869144984,-121.722455751481 36.8639549144984,-121.728548751481 36.8614349144984,-121.731905751481 36.8604989144984,-121.743965751481 36.8594549144984,-121.749698751481 36.8574029144984,-121.753361751481 36.8548559144984,-121.754171751481 36.8550809144984,-121.755755751481 36.8572589144984,-121.754855751481 36.8595989144984,-121.752389751481 36.8627579144984,-121.746791751481 36.8677529144984,-121.745351751481 36.8734679144984,-121.746863751481 36.8815499144984)),((-121.795166751481 36.9712529144984,-121.796660751481 36.9699479144984,-121.798766751481 36.9701549144984,-121.801862751481 36.9721529144984,-121.802366751481 36.9746549144984,-121.801160751481 36.9771479144984,-121.802960751481 36.9820529144984,-121.805165751481 36.9833489144984,-121.802564751481 36.9890549144984,-121.807262751481 36.9885509144984,-121.806767751481 36.9903509144984,-121.804958751481 36.9920519144984,-121.804463751481 36.9932489144984,-121.805966751481 37.0009529144984,-121.809467751481 37.0076489144984,-121.806560751481 37.0087469144984,-121.803959751481 37.0064519144984,-121.799162751481 36.9982529144984,-121.798658751481 36.9957509144984,-121.801961751481 36.9853469144984,-121.801061751481 36.9806489144984,-121.799261751481 36.9786509144984,-121.796966751481 36.9736559144984,-121.795166751481 36.9712529144984)),((-121.731257751481 36.9614519144984,-121.733561751481 36.9646559144984,-121.726658751481 36.9696509144984,-121.726757751481 36.9727559144984,-121.729862751481 36.9764549144984,-121.726856751481 36.9783539144984,-121.723157751481 36.9774539144984,-121.721429751481 36.9751769144984,-121.720673751481 36.9731339144984,-121.719665751481 36.9679229144984,-121.722149751481 36.9651239144984,-121.724327751481 36.9665819144984,-121.731257751481 36.9614519144984)))";                      
            //connection string change here
            var connectionstring = "mongodb://localhost";

            var mongoclient = new MongoClient(connectionstring);
            try
            {

            }
            catch
            {

            }
            string databaseName = "TestDb";
            //Mongo db name change here
            var db = mongoclient.GetDatabase("");
            string BasePath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())); ;
            string csvpath = BasePath + "\\CSV\\world_cityborder.csv";

            #region world_cityborder
            Console.WriteLine("Parsing Cityborder csv to Cityborder poco..");
            List<world_cityborder> wcblist = ParseCSVToTable<world_cityborder>(csvpath);
            Console.WriteLine("Parsing Done");

            dynamic collection = db.GetCollection<world_cityborder>("WorldCityBorder");
            Console.WriteLine("Inserting world_cityborder data...");
            collection.InsertMany(wcblist);
            Console.WriteLine("world_cityborder data inserted.\n");
            #endregion

            #region world_placepoint
            csvpath = BasePath + "\\CSV\\world_placepoint.csv";
            Console.WriteLine("Parsing world_placepoint csv to world_placepoint poco..");
            List<world_placepoint> wpp = ParseCSVToTable<world_placepoint>(csvpath);
            Console.WriteLine("Parsing Done");
            collection = db.GetCollection<world_placepoint>("WorldPlacepoint");
            Console.WriteLine("Inserting world_placepoint data...");
            collection.InsertMany(wpp);
            Console.WriteLine("world_placepoint data inserted.\n");

            #endregion

            #region world_uscityborder
            csvpath = BasePath + "\\CSV\\world_uscityborder.csv";
            Console.WriteLine("Parsing world_uscityborder csv to world_uscityborder poco..");
            List<world_uscityborder> wucb = ParseCSVToTable<world_uscityborder>(csvpath);
            Console.WriteLine("Parsing Done");
            collection = db.GetCollection<world_uscityborder>("WorldUSCityBorder");
            Console.WriteLine("Inserting world_uscityborder data...");
            collection.InsertMany(wucb);
            Console.WriteLine("world_uscityborder data inserted.\n");
            #endregion

            #region world_stateborder
            for (int i = 0; i <= 5; i++)
            {
                csvpath = BasePath + "\\CSV\\world_stateborder_" + i + ".csv";
                Console.WriteLine(i + ") Parsing world_stateborder csv to world_stateborder poco..");
                List<world_stateborder> wsb = ParseCSVToTable<world_stateborder>(csvpath);
                Console.WriteLine("Parsing Done");
                collection = db.GetCollection<world_stateborder>("WorldStateBorder");
                Console.WriteLine("Inserting world_stateborder data...");
                collection.InsertMany(wsb);
                Console.WriteLine("world_stateborder data inserted.");
            }

            #endregion

            #region world_urbanborder
            csvpath = BasePath + "\\CSV\\Urban.csv";
            Console.WriteLine("Parsing world_urbanborder csv to world_urbanborder poco..");
            List<world_urbanborder> wubb = ParseCSVToTable<world_urbanborder>(csvpath);
            Console.WriteLine("Parsing Done");
            collection = db.GetCollection<world_urbanborder>("WorldUrbanBorder");
            Console.WriteLine("Inserting world_urbanborder data...");
            collection.InsertMany(wubb);
            Console.WriteLine("world_urbanborder data inserted.\n");
            #endregion

            #region world_worldborder
            csvpath = BasePath + "\\CSV\\world_worldborder.csv";
            Console.WriteLine("Parsing world_worldborder csv to world_worldborder poco..");
            List<world_worldborder> wwb = ParseCSVToTable<world_worldborder>(csvpath);
            Console.WriteLine("Parsing Done");
            collection = db.GetCollection<world_worldborder>("WorldWorldBorder");
            Console.WriteLine("Inserting world_worldborder data...");
            collection.InsertMany(wwb);
            Console.WriteLine("world_worldborder data inserted.");
            #endregion

            #region world_zipcodeborder
            for (int i = 1; i <= 9; i++)
            {
                csvpath = BasePath + "\\CSV\\world_zipcodeborder_" + i + ".csv";
                Console.WriteLine(i + ") Parsing world_zipcodeborder csv to world_zipcodeborder poco..");
                List<world_zipcodeborder> wzcb = ParseCSVToTable<world_zipcodeborder>(csvpath);
                Console.WriteLine("Parsing Done");
                collection = db.GetCollection<world_zipcodeborder>("WorldZipCodeBorder");
                Console.WriteLine("Inserting world_zipcodeborder data...");
                collection.InsertMany(wzcb);
                Console.WriteLine("world_zipcodeborder data inserted.");

            }

            #endregion

            #region scraper_agency
            csvpath = BasePath + "\\CSV\\scraper_agency.csv";
            Console.WriteLine("Parsing scraper_agency csv to scraper_agency poco..");
            List<scraper_agency> sa = ParseCSVToTable<scraper_agency>(csvpath);
            Console.WriteLine("Parsing Done");
            collection = db.GetCollection<scraper_agency>("ScraperAgency");
            Console.WriteLine("Inserting scraper_agency data...");
            collection.InsertMany(sa);
            Console.WriteLine("scraper_agency data inserted.\n");
            #endregion

            #region scraper_market
            csvpath = BasePath + "\\CSV\\scraper_market.csv";
            Console.WriteLine("Parsing scraper_market csv to scraper_market poco..");
            List<scraper_market> sm = ParseCSVToTable<scraper_market>(csvpath);
            Console.WriteLine("Parsing Done");
            collection = db.GetCollection<scraper_market>("ScraperMarket");
            Console.WriteLine("Inserting scraper_market data...");
            collection.InsertMany(sm);
            Console.WriteLine("scraper_market data inserted.\n");
            #endregion

            #region spatial_ref_sys
            csvpath = BasePath + "\\CSV\\spatial_ref_sys.csv";
            Console.WriteLine("Parsing spatial_ref_sys csv to spatial_ref_sys poco..");
            List<spatial_ref_sys> srs = ParseCSVToTable<spatial_ref_sys>(csvpath);
            Console.WriteLine("Parsing Done");
            collection = db.GetCollection<spatial_ref_sys>("SpatialRefSys");
            Console.WriteLine("Inserting spatial_ref_sys data...");
            collection.InsertMany(srs);
            Console.WriteLine("spatial_ref_sys data inserted.\n");
            #endregion


            Console.WriteLine("Press any key for exit...");
            Console.ReadKey();
        }

    }
}

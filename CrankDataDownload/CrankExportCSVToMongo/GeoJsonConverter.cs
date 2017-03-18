using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Newtonsoft.Json;


namespace CrankExportCSVToMongo
{
  public  class GeoConverter
    {
        private const string GEOMETRYCOLLECTION = "GeometryCollection";
        private const string LINESTRING = "LineString";
        private const string MULTILINESTRING = "MultiLineString";
        private const string MULTIPOINT = "MultiPoint";
        private const string MULTIPOLYGON = "MultiPolygon";
        private const string POINT = "Point";
        private const string POLYGON = "Polygon";

        static public string ToGeoJSON(Dictionary<string, string> geos)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartObject();
                writer.WritePropertyName("type");
                writer.WriteValue("FeatureCollection");
                WriteFeatures(writer, geos);
                writer.WriteEndObject();

                return sb.ToString();
            }
        }

        static private void WriteFeatures(JsonWriter writer, Dictionary<string, string> geos)
        {
            writer.WritePropertyName("features");
            writer.WriteStartArray();
            foreach (KeyValuePair<string, string> geo in geos)
            {
                WriteFeature(writer, geo.Key, geo.Value);
            }
            writer.WriteEndArray();
        }

        static private void WriteFeature(JsonWriter writer, string geoCode, string WKT)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue("Feature");
            writer.WritePropertyName("id");
            writer.WriteValue(geoCode);
            WriteFeatureProperties(writer, geoCode);
            writer.WritePropertyName("geometry");
            WriteFeatureGeometry(writer, WKT);
            writer.WriteEndObject();
        }

        static private void WriteFeatureProperties(JsonWriter writer, string geoCode)
        {
            writer.WritePropertyName("properties");
            writer.WriteStartObject();
            writer.WritePropertyName("hc-key");
            writer.WriteValue(geoCode);
            writer.WriteEndObject();
        }

        static private void WriteFeatureGeometry(JsonWriter writer, string WKT)
        {
            string geometryType = GetGeometryType(WKT);

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue(geometryType);
            if (geometryType == GEOMETRYCOLLECTION)
            {
                WriteGeometries(writer, WKT);
            }
            else
            {
                WriteGeometryCoordinates(writer, geometryType, WKT);
            }
            writer.WriteEndObject();
        }

        static private void WriteGeometries(JsonWriter writer, string WKT) { }

        static private void WriteGeometryCoordinates(JsonWriter writer, string geometryType, string WKT)
        {
            writer.WritePropertyName("coordinates");
            switch (geometryType)
            {
                case LINESTRING:
                    WriteLineStringCoordinates(writer, WKT);
                    break;
                case MULTILINESTRING:
                    WriteMultiLineStringCoordinates(writer, WKT);
                    break;
                case MULTIPOINT:
                    WriteMultiPointCoordinates(writer, WKT);
                    break;
                case MULTIPOLYGON:
                    WriteMultiPolygonCoordinates(writer, WKT);
                    break;
                case POINT:
                    WritePointCoordinates(writer, WKT);
                    break;
                case POLYGON:
                    WritePolygonCoordinates(writer, WKT);
                    break;
            }
        }

        static private void WriteLineStringCoordinates(JsonWriter writer, string WKT)
        {
            string lineStringCoordinatesString = GetCoordinatesGroupString(WKT).First();//check if list larger than 1?EXCEPTION?
            List<Tuple<double, double>> lineStringCoordinates = GetCoordinatesList(lineStringCoordinatesString);

            WriteMultipleCoordinates(writer, lineStringCoordinates);
        }

        static private void WriteMultiLineStringCoordinates(JsonWriter writer, string WKT)
        {
            List<string> multiLineStringCoordinatesStringList = GetCoordinatesGroupString(WKT);
            List<Tuple<double, double>> lineStringCoordinates = new List<Tuple<double, double>>();

            writer.WriteStartArray();
            foreach (string lineStringCoordinatesString in multiLineStringCoordinatesStringList)
            {
                lineStringCoordinates = GetCoordinatesList(lineStringCoordinatesString);
                WriteMultipleCoordinates(writer, lineStringCoordinates);
            }
            writer.WriteEndArray();
        }

        static private void WriteMultiPointCoordinates(JsonWriter writer, string WKT)
        {
            string homogenizedWKT = WKT.Replace("(", "").Replace(")", "");//for whatever reason WKT representation of multipoint geometries come in two formats!!
            string MultiPointCoordinatesString = GetCoordinatesGroupString(homogenizedWKT).First();//check if list larger than 1?EXCEPTION?
            List<Tuple<double, double>> multiPointCoordinates = GetCoordinatesList(MultiPointCoordinatesString);

            WriteMultipleCoordinates(writer, multiPointCoordinates);
        }

        static private void WriteMultiPolygonCoordinates(JsonWriter writer, string WKT)
        {
            Regex test = new Regex("[(][(]([0-9, ]+)([(),]+)([0-9, ]+)[)][)]");
            MatchCollection testMatches = test.Matches(WKT);

            writer.WriteStartArray();
            foreach (Match m in testMatches)
            {
                Console.WriteLine("match = " + m.Value);
                WritePolygonCoordinates(writer, m.Value);
            }
            writer.WriteEndArray();
        }

        static private void WritePointCoordinates(JsonWriter writer, string WKT)
        {
            string pointCoordinatesString = GetCoordinatesGroupString(WKT).First();//check if list larger than 1?EXCEPTION?
            Tuple<double, double> pointCoordinates = GetCoordinatesList(pointCoordinatesString).First();//check if list larger than 1?EXCEPTION?

            WriteCoordinates(writer, pointCoordinates);
        }

        static private void WritePolygonCoordinates(JsonWriter writer, string WKT)
        {
            List<string> coordinatesGroupList = GetCoordinatesGroupString(WKT);
            List<Tuple<double, double>> polygonCoordinates = new List<Tuple<double, double>>();

            writer.WriteStartArray();
            foreach (string coordinatesGroup in coordinatesGroupList)
            {
                polygonCoordinates = GetCoordinatesList(coordinatesGroup);
                WriteMultipleCoordinates(writer, polygonCoordinates);
            }
            writer.WriteEndArray();
        }

        static public List<string> GetCoordinatesGroupString(string WKT)
        {
            Regex coordinateGroupPattern = new Regex("[0-9.]+ [0-9., ]+");
            MatchCollection coordinateGroupMatch = coordinateGroupPattern.Matches(WKT);
            List<string> value = new List<string>();

            if (coordinateGroupMatch.Count != 0)
            {
                foreach (Match m in coordinateGroupMatch)
                {
                    value.Add(m.Value);
                }
            }
            else
            {
                throw new Exception("Error: No coordinates found");
            }

            return value;

        }

        static private void WriteMultipleCoordinates(JsonWriter writer, List<Tuple<double, double>> coordinatesList)
        {//does this for each loop need to wrapped in array????

            writer.WriteStartArray();
            foreach (Tuple<double, double> coordinates in coordinatesList)
            {
                WriteCoordinates(writer, coordinates);
            }
            writer.WriteEndArray();
        }

        static private void WriteCoordinates(JsonWriter writer, Tuple<double, double> coordinates)
        {
            writer.WriteStartArray();
            writer.WriteValue(coordinates.Item1);
            writer.WriteValue(coordinates.Item2);
            writer.WriteEndArray();
        }

        static public List<Tuple<double, double>> GetCoordinatesList(string coordinateGroup)
        {
            List<string> coordinatesList = new List<string>();
            if (coordinateGroup.IndexOf(',') != -1)
            {
                List<string> inputList = new List<string>(coordinateGroup.Split(','));
                foreach (string coordinatesString in inputList.ToList())
                {
                    coordinatesList.Add(coordinatesString.Trim());
                }
            }
            else
            {
                coordinatesList.Add(coordinateGroup);
            }

            return ConvertCoordinateString(coordinatesList);
        }

        static public List<Tuple<double, double>> ConvertCoordinateString(List<string> coordinateString)
        {
            List<Tuple<double, double>> coordinatesList = new List<Tuple<double, double>>();
            double x = 0;
            double y = 0;

            foreach (string coordinates in coordinateString)
            {
                string[] coordinatesArray = coordinates.Split(null);
                if (coordinatesArray.Length == 2)
                {

                    if (Double.TryParse(coordinatesArray[0], out x) && Double.TryParse(coordinatesArray[1], out y))
                    {
                        coordinatesList.Add(new Tuple<double, double>(x, y));
                    }
                }
                else
                {
                    Console.WriteLine("catastroiphic failure");
                }
            }
            return coordinatesList;
        }

        static private string GetGeometryType(string WKT)
        {
            Regex GeometryTypePattern = new Regex("^[A-Z]+");
            Match GeometryTypeMatch = GeometryTypePattern.Match(WKT);

            return GeometryTypeMatch.Success ? ConvertGeometryType(GeometryTypeMatch.Value) : "";
        }

        static private string ConvertGeometryType(string GeometryType)
        {
            string value = "";
            switch (GeometryType)
            {
                case "GEOMETRYCOLLECTION":
                    value = GEOMETRYCOLLECTION;
                    break;
                case "LINESTRING":
                    value = LINESTRING;
                    break;
                case "MULTILINESTRING":
                    value = MULTILINESTRING;
                    break;
                case "MULTIPOINT":
                    value = MULTIPOINT;
                    break;
                case "MULTIPOLYGON":
                    value = MULTIPOLYGON;
                    break;
                case "POINT":
                    value = POINT;
                    break;
                case "POLYGON":
                    value = POLYGON;
                    break;
            }
            return value;
        }
    }
}
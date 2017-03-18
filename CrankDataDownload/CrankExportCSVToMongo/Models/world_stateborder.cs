using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace CrankExportCSVToMongo.Models
{
    /// <summary>
    /// "id","state_name","state_fips","sub_region","state_abbr","pop2000","pop2007","pop00_sqmi","pop07_sqmi","white","black","ameri_es",
    /// "asian","hawn_pi","other","mult_race","hispanic","males","females","age_under5",
    /// "age_5_17","age_18_21","age_22_29","age_30_39","age_40_49","age_50_64","age_65_up",
    /// "med_age","med_age_m","med_age_f","households","ave_hh_sz","hsehld_1_m","hsehld_1_f",
    /// "marhh_chd","marhh_no_c","mhh_child","fhh_child","families","ave_fam_sz","hse_units",
    /// "vacant","owner_occ","renter_occ","no_farms97","avg_size97","crop_acr97","avg_sale97","sqmi","oid","geom"
    /// </summary>
    public class world_stateborder
    {
        [BsonId]
        [BsonElement("Id")]
        public int id { get; set; }

        [BsonElement("StateName")]
        public string state_name { get; set; }

        [BsonElement("StateFips")]
        public string state_fips { get; set; }

        [BsonElement("SubRegion")]
        public string sub_region { get; set; }

        [BsonElement("StateAbbr")]
        public string state_abbr { get; set; }

        [BsonElement("Pop2000")]
        public int pop2000 { get; set; }
        [BsonElement("Pop200")]
        public int pop2007 { get; set; }

        [BsonElement("Pop00Sqmi")]
        public double pop00_sqmi { get; set; }


        [BsonElement("Pop07Sqmi")]
        public double pop07_sqmi { get; set; }

        [BsonElement("White")]
        public int white { get; set; }

        [BsonElement("Black")]
        public int black { get; set; }

        [BsonElement("AmeriEs")]
        public int ameri_es { get; set; }

        [BsonElement("Asian")]
        public int asian { get; set; }

        [BsonElement("HawnPi")]
        public int hawn_pi { get; set; }

        [BsonElement("Other")]
        public int other { get; set; }

        [BsonElement("MultRace")]
        public int mult_race { get; set; }

        [BsonElement("HisPanic")]
        public int hispanic { get; set; }

        [BsonElement("Males")]
        public int males { get; set; }

        [BsonElement("Females")]
        public int females { get; set; }

        [BsonElement("AgeUnder5")]
        public int age_under5 { get; set; }

        [BsonElement("Age_5_17")]
        public int age_5_17 { get; set; }


        [BsonElement("Age_18_21")]
        public int age_18_21 { get; set; }

        [BsonElement("Age_22_29")]
        public int age_22_29 { get; set; }

        [BsonElement("Age_30_39")]
        public int age_30_39 { get; set; }

        [BsonElement("Age_40_49")]
        public int age_40_49 { get; set; }

        [BsonElement("Age_50_64")]
        public int age_50_64 { get; set; }

        [BsonElement("Age_65_Up")]
        public int age_65_up { get; set; }

        [BsonElement("MedAge")]
        public double med_age { get; set; }

        [BsonElement("MedAgeM")]
        public double med_age_m { get; set; }

        [BsonElement("MedAgeF")]
        public double med_age_f { get; set; }

        [BsonElement("Households")]
        public int households { get; set; }

        [BsonElement("AveHhSz")]
        public double ave_hh_sz { get; set; }

        [BsonElement("Hsehld1M")]
        public int hsehld_1_m { get; set; }

        [BsonElement("Hsehld1f")]
        public int hsehld_1_f { get; set; }

        [BsonElement("MarhhChd")]
        public int marhh_chd { get; set; }

        [BsonElement("MarhhNoC")]
        public int marhh_no_c { get; set; }

        [BsonElement("MhhChild")]
        public int mhh_child { get; set; }

        [BsonElement("FhhChild")]
        public int fhh_child { get; set; }

        [BsonElement("Families")]
        public int families { get; set; }

        [BsonElement("AveFamSz")]
        public double ave_fam_sz { get; set; }

        [BsonElement("HseUnits")]
        public int hse_units { get; set; }

        [BsonElement("Vacant")]
        public int vacant { get; set; }

        [BsonElement("OwnerOcc")]
        public int owner_occ { get; set; }

        [BsonElement("RenterOcc")]
        public int renter_occ { get; set; }

        [BsonElement("NoFarms97")]
        public double no_farms97 { get; set; }

        [BsonElement("AvgSize97")]
        public double avg_size97 { get; set; }

        [BsonElement("CropAcr97")]
        public double crop_acr97 { get; set; }
        [BsonElement("AvgSale97")]
        public double avg_sale97 { get; set; }

        [BsonElement("Sqmi")]
        public int sqmi { get; set; }

        [BsonElement("Oid")]
        public int oid { get; set; }
        [BsonIgnore]
        public string st_astext { get; set; }



        [BsonElement("Geom")]
        public GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates> geom { set { value = null; } get { return Program.GetMultiPolyGon(st_astext); } }



    }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace Crankdata.Models
{
    /// <summary>
    /// "id","state_name","state_fips","sub_region","state_abbr","pop2000","pop2007","pop00_sqmi","pop07_sqmi","white","black","ameri_es",
    /// "asian","hawn_pi","other","mult_race","hispanic","males","females","age_under5",
    /// "age_5_17","age_18_21","age_22_29","age_30_39","age_40_49","age_50_64","age_65_up",
    /// "med_age","med_age_m","med_age_f","households","ave_hh_sz","hsehld_1_m","hsehld_1_f",
    /// "marhh_chd","marhh_no_c","mhh_child","fhh_child","families","ave_fam_sz","hse_units",
    /// "vacant","owner_occ","renter_occ","no_farms97","avg_size97","crop_acr97","avg_sale97","sqmi","oid","geom"
    /// </summary>
    public class WorldStateBorder
    {
        [BsonId]
        public int Id { get; set; }


        public string StateName { get; set; }


        public string StateFips { get; set; }


        public string SubRegion { get; set; }


        public string StateAbbr { get; set; }


        public int Pop2000 { get; set; }


        public int Pop200 { get; set; }


        public double Pop00Sqmi { get; set; }



        public double Pop07Sqmi { get; set; }


        public int White { get; set; }


        public int Black { get; set; }


        public int AmeriEs { get; set; }


        public int Asian { get; set; }


        public int HawnPi { get; set; }


        public int Other { get; set; }


        public int MultRace { get; set; }


        public int HisPanic { get; set; }


        public int Males { get; set; }


        public int Females { get; set; }


        public int AgeUnder5 { get; set; }


        public int Age_5_17 { get; set; }



        public int Age_18_21 { get; set; }


        public int Age_22_29 { get; set; }


        public int Age_30_39 { get; set; }


        public int Age_40_49 { get; set; }


        public int Age_50_64 { get; set; }


        public int Age_65_Up { get; set; }


        public double MedAge { get; set; }


        public double MedAgeM { get; set; }


        public double MedAgeF { get; set; }


        public int Households { get; set; }


        public double AveHhSz { get; set; }


        public int Hsehld1M { get; set; }


        public int Hsehld1f { get; set; }


        public int MarhhChd { get; set; }


        public int MarhhNoC { get; set; }


        public int MhhChild { get; set; }


        public int FhhChild { get; set; }


        public int Families { get; set; }


        public double AveFamSz { get; set; }


        public int HseUnits { get; set; }


        public int Vacant { get; set; }


        public int OwnerOcc { get; set; }


        public int RenterOcc { get; set; }


        public double NoFarms97 { get; set; }


        public double AvgSize97 { get; set; }


        public double CropAcr97 { get; set; }

        public double AvgSale97 { get; set; }


        public int Sqmi { get; set; }


        public int Oid { get; set; }





        public GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates> Geom { set { value = null; } get { return Program.GetMultiPolyGon(st_astext); } }



    }
}

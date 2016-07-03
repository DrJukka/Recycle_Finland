using System;
using System.Collections.Generic;


namespace RecycleFinland.Engine
{
    public class JLYConstants
    {
        public const string laji_id = "laji_id";
        public const string paikka_id = "paikka_id";
        public const string lajitarkennus = "lajitarkennus";
        public const string nimi = "nimi";
        public const string osoite = "osoite";
        public const string pnro = "pnro";
        public const string paikkakunta = "paikkakunta";
        public const string lat = "lat";
        public const string lng = "lng";
        public const string etaisyys = "etaisyys";
        public const string yllapitaja = "yllapitaja";
        public const string miehitys = "miehitys";
        public const string aukiolo = "aukiolo";
        public const string yhteys = "yhteys";
        public const string rateplus = "rateplus";
        public const string rateminus = "rateminus";

        public static Dictionary<int, string> materialTypes = new Dictionary<int, string>
        {
        {0, "All"},
        {103, "Paper"},
        {106, "Metals"},
        {107, "Glass packaging"},
        {104, "Carton packaging"},
        {111, "Plastic packaging"},
        {109, "Electrical equipment"},
        {116, "Lamps"},
        {110, "Batteries"},
        {115, "Automotive batteries"},
        {113, "Textiles"},
        {117, "Wood"},
        {119, "Construction waste"},
        {108, "Hazardous waste"},
        {101, "Garden waste"},
        {102, "Energy waste"},
        {100, "Mixed waste"},
        {114, "Other waste"}
        };
    }
}
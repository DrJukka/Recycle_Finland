package com.drjukka.recyclefinland;

import java.util.Collections;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.Map;

/**
 * Created by juksilve on 29.1.2016.
 */
public class JLYConstants {
    public static String laji_id = "laji_id";
    public static String  paikka_id = "paikka_id";
    public static String  lajitarkennus = "lajitarkennus";
    public static String  nimi = "nimi";
    public static String  osoite = "osoite";
    public static String  pnro = "pnro";
    public static String  paikkakunta = "paikkakunta";
    public static String  lat = "lat";
    public static String  lng = "lng";
    public static String  etaisyys = "etaisyys";
    public static String  yllapitaja = "yllapitaja";
    public static String  miehitys = "miehitys";
    public static String  aukiolo = "aukiolo";
    public static String  yhteys = "yhteys";
    public static String  rateplus = "rateplus";
    public static String  rateminus = "rateminus";


    public static final Map<Integer, String> materialTypes = createMap();

    private static Map<Integer, String> createMap() {
        Map<Integer, String> result = new LinkedHashMap<Integer, String>();
        result.put(0, "All");
        result.put(103, "Paper");
        result.put(106, "Metals");
        result.put(107, "Glass packaging");
        result.put(104, "Carton packaging");
        result.put(111, "Plastic packaging");
        result.put(109, "Electrical equipment");
        result.put(116, "Lamps");
        result.put(110, "Batteries");
        result.put(115, "Automotive batteries");
        result.put(113, "Textiles");
        result.put(117, "Wood");
        result.put(119, "Construction waste");
        result.put(108, "Hazardous waste");
        result.put(101, "Garden waste");
        result.put(102, "Energy waste");
        result.put(100, "Mixed waste");
        result.put(114, "Other waste");
        return Collections.unmodifiableMap(result);
    }



}

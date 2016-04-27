//
//  JLYConstants.swift
//  Recycle Finland
//
//  Created by Jukka Silvennoinen on 04/02/16.
//  Copyright © 2016 Jukka Silvennoinen. All rights reserved.
//

import Foundation

public struct JLYConstants {
    static let laji_id = "laji_id";
    static let paikka_id = "paikka_id";
    static let lajitarkennus = "lajitarkennus";
    static let nimi = "nimi";
    static let osoite = "osoite";
    static let pnro = "pnro";
    static let paikkakunta = "paikkakunta";
    static let lat = "lat";
    static let lng = "lng";
    static let etaisyys = "etaisyys";
    static let yllapitaja = "yllapitaja";
    static let miehitys = "miehitys";
    static let aukiolo = "aukiolo";
    static let yhteys = "yhteys";
    static let rateplus = "rateplus";
    static let rateminus = "rateminus";
    
    static let materialTypeeOrder : [Int] = [0,103,106,107,104,111,109,116,110,115,113,117,119,108,101,102,100,114]
    
    static let materialTypes = [0 : "All",103: "Paper",106: "Metals", 107: "Glass packaging",104: "Carton packaging",111: "Plastic packaging",109: "Electrical equipment",116: "Lamps",110: "Batteries",115: "Automotive batteries",113: "Textiles",117: "Wood",119: "Construction waste",108: "Hazardous waste",101: "Garden waste",102: "Energy waste",100: "Mixed waste",114: "Other waste"]
}

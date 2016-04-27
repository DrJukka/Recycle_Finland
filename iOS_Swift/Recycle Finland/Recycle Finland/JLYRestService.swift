//
//  JLYRestService.swift
//  Recycle Finland
//
//  Created by Jukka Silvennoinen on 05/02/16.
//  Copyright © 2016 Jukka Silvennoinen. All rights reserved.
//

import Foundation
import UIKit
import MapKit

protocol JLYRestServiceDelegate: class {
    func SearchFinished(array : [JLYServiceItem])
}

class JLYRestService : NSObject, NSXMLParserDelegate  {

    private var parser :NSXMLParser?
    private var array = [JLYServiceItem]()
    
    weak var delegate:JLYRestServiceDelegate?

    func findNearby(point : CLLocationCoordinate2D,type : Int,radius : Int,limit : Int) {
    
        var UrlToUse = "http://kierratys.info/2.0/genxml.php?lat=" + String(point.latitude) + "&lng=" + String(point.longitude);
    
        if (limit != 0) {
            UrlToUse += "&limit=" + String(limit);
        }
    
        if (radius != 0) {
            UrlToUse +=  "&radius=" + String(radius);
        }
    
        if (type != 0) {
            UrlToUse +=  "&type_id=" + String(type);
        }
    
        print("UrlToUse : " + UrlToUse);
    
        array.removeAll()
        
        let url = NSURL(string: UrlToUse)
        
        let task = NSURLSession.sharedSession().dataTaskWithURL(url!) {(data, response, error) in
            
            if data == nil{
                //return the empty array
                self.delegate?.SearchFinished(self.array)
                return;
            }
            
            //print(NSString(data: data!, encoding: NSUTF8StringEncoding))
            
            self.parser = NSXMLParser(data: data!)
            self.parser!.delegate = self
            self.parser!.parse()
        }
        
        task.resume()
    }
    
    @objc func parserDidStartDocument(parser: NSXMLParser)
    {
        print("parserDidStartDocument : ")
    }
    
    @objc func parser(parser: NSXMLParser, didStartElement elementName: String, namespaceURI: String?, qualifiedName qName: String?, attributes attributeDict: [String : String])
    {
        //print("Start - elementName : " + elementName)
    
        if (elementName as NSString).isEqualToString("marker")
        {
            if let latitude = attributeDict[JLYConstants.lat]{
                if let longitude = attributeDict[JLYConstants.lng]{
                    if let locationId = attributeDict[JLYConstants.paikka_id]{
                        if let title = attributeDict[JLYConstants.nimi]{
            
                            let newItem =  JLYServiceItem(title : title, coordinate: CLLocationCoordinate2D(latitude: Double(latitude)!, longitude: Double(longitude)!),locationId : locationId)
                            
                            for attrib in attributeDict {
                            
                                switch(attrib.0)
                                {
                                case JLYConstants.laji_id:  // = "laji_id";
                                    if let materialType = Int(attrib.1) {
                                        newItem.materialTypes.append(materialType)
                                    }
                                    break
                                case JLYConstants.lajitarkennus: // = "lajitarkennus";
                                    break
                                case JLYConstants.osoite: // = "osoite";
                                    newItem.address = attrib.1;
                                    break
                                case JLYConstants.pnro: // = "pnro";
                                    newItem.postalCode = attrib.1;
                                    break
                                case JLYConstants.paikkakunta: // = "paikkakunta";
                                    newItem.city = attrib.1;
                                    break
                                case JLYConstants.etaisyys: // = "etaisyys";
                                    newItem.distance = attrib.1;
                                    break
                                case JLYConstants.yllapitaja: // = "yllapitaja";
                                    newItem.oper = attrib.1;
                                    break
                                case JLYConstants.miehitys: // = "miehitys";
                                    if let manned = Int(attrib.1) {
                                        newItem.manned = manned
                                    }
                                    break
                                case JLYConstants.aukiolo: // = "aukiolo";
                                    newItem.openTimes = attrib.1;
                                    break
                                case JLYConstants.yhteys: // = "yhteys";
                                    newItem.contact = attrib.1;
                                    break
                                default:
                                    break;
                                }
                            }
                            
                            array.append(newItem)
                        }
                    }
                }
             }
        }
    }

    
    @objc func parserDidEndDocument(parser: NSXMLParser)
    {
        print("parserDidEndDocument : ")
        delegate?.SearchFinished(array)
    }
}
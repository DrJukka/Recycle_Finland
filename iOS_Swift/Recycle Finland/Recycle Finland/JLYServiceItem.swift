//
//  JLYServiceItem.swift
//  Recycle Finland
//
//  Created by Jukka Silvennoinen on 03/02/16.
//  Copyright © 2016 Jukka Silvennoinen. All rights reserved.
//

import Foundation
import MapKit
import AddressBook

class JLYServiceItem: NSObject, MKAnnotation {
    

    let title: String?
    var subtitle: String?
    let coordinate: CLLocationCoordinate2D // required
    let locationId : String
    var materialTypes = [Int]()
    var address : String?
    var city : String?
    var distance : String?
    var oper : String?
    var manned : Int = 0
    var openTimes: String?
    var contact: String?
    var postalCode : String?
    
    
    init(title: String, coordinate: CLLocationCoordinate2D, locationId : String) {
        self.title = title
        self.coordinate = coordinate
        self.locationId = locationId
        self.subtitle = "";
        super.init()
    }
    
    func fullAddress() -> String?{
        
        var retString : String?
        
        if address?.characters.count > 0 {
            retString = address
        }
        
        if postalCode?.characters.count > 0 {
            
            if retString?.characters.count > 0 {
                retString! += " " + postalCode!
            }else{
                retString = postalCode
            }

        }
        if city?.characters.count > 0 {
            
            if retString?.characters.count > 0 {
                retString! += " " + city!
            }else{
                retString = city
            }
            
        }
        
        return retString
    }
    
    // MARK: - MapKit related methods
    
    // pinColor for disciplines: Sculpture, Plaque, Mural, Monument, other
    func pinColor() -> MKPinAnnotationColor  {
         return .Red
    }
    
    // annotation callout opens this mapItem in Maps app for navigation
    //functionality defined in ViewControllerMapView.swift
    func mapItem() -> MKMapItem {
           
        let addressDict = [String(kABPersonAddressStreetKey): self.subtitle as! AnyObject]
        let placemark = MKPlacemark(coordinate: self.coordinate, addressDictionary: addressDict)
        
        let mapItem = MKMapItem(placemark: placemark)
        mapItem.name = self.title
        
        return mapItem
    }
}

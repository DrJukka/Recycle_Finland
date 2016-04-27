//
//  ViewController.swift
//  Recycle Finland
//
//  Created by Jukka Silvennoinen on 03/02/16.
//  Copyright © 2016 Jukka Silvennoinen. All rights reserved.
//

import UIKit
import MapKit

class ViewController: EngagementViewController, TypesOverlayDelegate, DetailsOverlayDelegate, JLYRestServiceDelegate ,MKMapViewDelegate, UITextFieldDelegate, CLLocationManagerDelegate{
    
    @IBOutlet weak var searchTextBox: UITextField!
    
    @IBOutlet weak var mapView: MKMapView!
   
    var userLocationButton : UIButton?
    var userLocation : CLLocation?
    
    var locationManager = CLLocationManager()
    var restService  = JLYRestService()
    var recycleArray = [JLYServiceItem]()
    var currentItem = 0
    
    var lastOrientationLandscape = false;
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        locationManager.delegate = self
        mapView.delegate = self
        restService.delegate = self
        TypesOverlay.shared.delegate = self
        DetailsOverlay.shared.delegate = self
        
        EngagementAgent.shared().startActivity("MainActivity",extras: nil)
        
        let tap = UITapGestureRecognizer(target: self, action: Selector("handleMapTap:"))
        mapView.addGestureRecognizer(tap)

        searchTextBox.delegate = self

        lastOrientationLandscape = UIDeviceOrientationIsLandscape(UIDevice.currentDevice().orientation)
        NSNotificationCenter.defaultCenter().addObserver(self, selector: "rotated", name: UIDeviceOrientationDidChangeNotification, object: nil)

        let initialLocation = CLLocation(latitude: 60.1708, longitude: 24.9375)
        let initialDistance : CLLocationDistance = 5000;
        
        centerMap(initialLocation,distance : initialDistance)
        
        let image = UIImage(named: "listButtonImage") as UIImage?
        let button   = UIButton(type: UIButtonType.System) as UIButton
        button.frame = CGRectMake(8, 8, 48, 48)
        button.setImage(image, forState: .Normal)
        button.addTarget(self, action: "showTypesView:", forControlEvents: UIControlEvents.TouchUpInside)
        mapView.addSubview(button)
        
        let image2 = UIImage(named: "refreshButtonImage") as UIImage?
        let button2   = UIButton(type: UIButtonType.System) as UIButton
        button2.frame = CGRectMake(8, 56, 48, 48)
        button2.setImage(image2, forState: .Normal)
        button2.addTarget(self, action: "reFreshData:", forControlEvents: UIControlEvents.TouchUpInside)
        mapView.addSubview(button2)
        
        startSearch()
        
        locationManager.desiredAccuracy = kCLLocationAccuracyBest
        if CLLocationManager.authorizationStatus() == .NotDetermined {
            if #available(iOS 8.0, *) {
                print("requesting authorization !")
                locationManager.requestWhenInUseAuthorization()
            } else {
                // Fallback on earlier versions
            }
            
        }else{
            locationManager.startUpdatingLocation()
            mapView.showsUserLocation = true;
            
        }
    }

    func rotated()
    {
        dispatch_async(dispatch_get_main_queue(), {
            
            if(UIDeviceOrientationIsLandscape(UIDevice.currentDevice().orientation) && !self.lastOrientationLandscape)
            {
                print("landscape")
                self.lastOrientationLandscape = true;
                self.updateOverlays()
            }
            
            if(UIDeviceOrientationIsPortrait(UIDevice.currentDevice().orientation) && self.lastOrientationLandscape)
            {
                print("Portrait")
                self.lastOrientationLandscape = false;
                self.updateOverlays()
            }
            
      /*      if(self.userLocationButton == nil){
                //move point to right top of the screen
                self.userLocationButton!.frame = CGRectMake((self.view.bounds.width - 56), 0, 48, 48)
            }*/
        })
    }
    
    func updateOverlays(){
    
        if(ProgressCircle.shared.isShowing){
            ProgressCircle.shared.showOverlay(self.view, isLandscape:  UIDeviceOrientationIsLandscape(UIDevice.currentDevice().orientation))
        }
        
        if(AboutOverlay.shared.isShowing){
            AboutOverlay.shared.showOverlay(self.view, isLandscape:  UIDeviceOrientationIsLandscape(UIDevice.currentDevice().orientation))
        }
        
        if(TypesOverlay.shared.isShowing){
            TypesOverlay.shared.showOverlay(self.view, isLandscape:  UIDeviceOrientationIsLandscape(UIDevice.currentDevice().orientation))
        }
        
        if(DetailsOverlay.shared.isShowing){
            DetailsOverlay.shared.showOverlay(self.view, isLandscape:  UIDeviceOrientationIsLandscape(UIDevice.currentDevice().orientation))
        }
    }
 

    
// MARK overlay delegates
    
    func selectedTypeChanged(selectedType : Int){
        
        EngagementAgent.shared().sendSessionEvent("TypeSelected", extras: nil)
        
        print("selected type : " + String(selectedType) + "==" + JLYConstants.materialTypes[selectedType]!);
        startSearch()
    }
    
    func getRecyclePlace(which : WhichItem) -> JLYServiceItem?{
     
        EngagementAgent.shared().sendSessionEvent("Show_POI", extras: nil)
        
        if (recycleArray.count <= 0)
        {
            return nil;
        }
        
        if (which == WhichItem.next)
        {
            currentItem++;
        }
        else if(which == WhichItem.previous)
        {
            currentItem--;
        }
        
        if (currentItem < 0)
        {
            currentItem = (recycleArray.count - 1);
        }
        
        if (recycleArray.count <= currentItem)
        {
            currentItem = 0;
        }
        
        let selectedItem = recycleArray[currentItem];
        
        mapView.setCenterCoordinate(selectedItem.coordinate, animated: false)
        mapView.selectAnnotation(selectedItem, animated: true)
        
        return selectedItem
    }
    
    func startSearch(){
        
        ProgressCircle.shared.showOverlay(self.view, isLandscape:  UIDeviceOrientationIsLandscape(UIDevice.currentDevice().orientation))
        
        
        EngagementAgent.shared().startJob("Nearby", extras: nil)
        restService.findNearby(mapView.centerCoordinate,type : TypesOverlay.shared.selectedType,radius : 0,limit : 0);
    }
    
    func SearchFinished(array : [JLYServiceItem])
    {
        EngagementAgent.shared().endJob("Nearby")
        
        dispatch_async(dispatch_get_main_queue(), {
            print("SearchFinished with " + String(array.count) + " results")
            ProgressCircle.shared.hideOverlayView()
        
            
            let annotationsToRemove = self.mapView.annotations.filter { $0 !== self.mapView.userLocation }
            self.mapView.removeAnnotations( annotationsToRemove )
            
            self.currentItem = 0
            self.recycleArray = array
            self.mapView.addAnnotations(self.recycleArray)
            
            if(self.recycleArray.count <= 0){
                self.showToast("No results found");
            }
        })
    }
    
    //MARK, general helpers

    func showToast(message : String){
        
        let toastLabel = UILabel(frame: CGRectMake(self.view.frame.size.width/2 - 150, self.view.frame.size.height-100, 300, 35))
        toastLabel.backgroundColor = UIColor.blackColor()
        toastLabel.textColor = UIColor.whiteColor()
        toastLabel.textAlignment = NSTextAlignment.Center;
        self.view.addSubview(toastLabel)
        toastLabel.text = message
        toastLabel.alpha = 1.0
        toastLabel.layer.cornerRadius = 10;
        toastLabel.clipsToBounds  =  true
        UIView.animateKeyframesWithDuration(4.0, delay: 0.1, options: UIViewKeyframeAnimationOptions.CalculationModeLinear, animations: {
            toastLabel.alpha = 0.0
            },
        completion: { finished in
                //print("toast finished !!!")
        })
    }
    
// MARK UI Button actions
    @IBAction func showAboutView(sender: AnyObject) {
        searchTextBox.resignFirstResponder()
        EngagementAgent.shared().startActivity("AboutActivity",extras: nil)
        
        AboutOverlay.shared.showOverlay(self.view, isLandscape:  UIDeviceOrientationIsLandscape(UIDevice.currentDevice().orientation))
    }
    
    @IBAction func searchAddress(sender: AnyObject) {
        startGeoSearch()
    }
    
    func gotoUserLocation(sender:UIButton!){
        EngagementAgent.shared().sendSessionEvent("locate_me", extras: nil)
        mapView.setCenterCoordinate(userLocation!.coordinate, animated: false)
        startSearch();
    }
    

    func startGeoSearch(){
        searchTextBox.resignFirstResponder()
        print("do startGeoSearch")
 
        EngagementAgent.shared().startJob("SearchAddress",extras: nil)
        
        CLGeocoder().geocodeAddressString(searchTextBox.text!, completionHandler:{(placemarks: [CLPlacemark]?, error: NSError?) -> Void in
            
                dispatch_async(dispatch_get_main_queue(), {
                
                    
                    if error != nil {
                        self.showToast("Error with geocoding")
                        print("geocodeAddressStringfailed with error" + error!.localizedDescription)
                        return
                    }
                
                    EngagementAgent.shared().endJob("SearchAddress")
                    
                    if placemarks?.count > 0 {
                        for pm : CLPlacemark in placemarks! {
                            if (pm.country?.lowercaseString == "finland" || pm.country?.lowercaseString == "suomi" ) {
                                self.mapView.setCenterCoordinate(pm.location!.coordinate, animated: false)
                                self.startSearch()
                                return;
                            }
                        }
                    }
                    //if we indeed got here, we did not find suitable results
                    self.showToast("No results found")
                })
            })
        
    }

    func showTypesView(sender:UIButton!)
    {   searchTextBox.resignFirstResponder()
        EngagementAgent.shared().startActivity("TypesActivity",extras: nil)
        
        TypesOverlay.shared.showOverlay(self.view, isLandscape:  UIDeviceOrientationIsLandscape(UIDevice.currentDevice().orientation))
    }
 
    func reFreshData(sender:UIButton!)
    {
        EngagementAgent.shared().sendSessionEvent("Refresh", extras: nil)
        searchTextBox.resignFirstResponder()
        startSearch()
    }
    
    // Mark UITextFieldDelegate
    
    func textFieldShouldEndEditing(textField: UITextField) -> Bool
    {
        return true
    }
    
    func textFieldShouldReturn(textField: UITextField) -> Bool
    {
        startGeoSearch()
        return true
    }
    
    
    //Mark Map related functions
    
    
    func handleMapTap(sender: UITapGestureRecognizer? = nil) {
       searchTextBox.resignFirstResponder()
    }
    
    func centerMap(location: CLLocation, distance : CLLocationDistance) {
        let coordinateRegion = MKCoordinateRegionMakeWithDistance(location.coordinate, distance * 2.0, distance * 2.0)
        mapView.setRegion(coordinateRegion, animated: true)
    }
    
    // MARK CLLocationManagerDelegate
    
    func locationManager(manager: CLLocationManager, didUpdateLocations locations: [CLLocation]){
        
        userLocation = locations[0] as CLLocation
        
        dispatch_async(dispatch_get_main_queue(), {
            //We'll create & add the button when we get first location
            if(self.userLocationButton == nil){
                let image3 = UIImage(named: "mylocButtonImage") as UIImage?
                self.userLocationButton   = UIButton(type: UIButtonType.System) as UIButton
                self.userLocationButton!.frame = CGRectMake((self.view.bounds.width - 56), 0, 48, 48)
                self.userLocationButton!.setImage(image3, forState: .Normal)
                self.userLocationButton!.addTarget(self, action: "gotoUserLocation:", forControlEvents: UIControlEvents.TouchUpInside)
                self.mapView.addSubview(self.userLocationButton!)
            }
        })
    }
    
    func locationManager(manager: CLLocationManager, didChangeAuthorizationStatus status: CLAuthorizationStatus){
        locationManager.startUpdatingLocation()
        mapView.showsUserLocation = true;
    }
    
    // MARK MKMapViewDelegate
    
    func mapView(mapView: MKMapView, viewForAnnotation annotation: MKAnnotation) -> MKAnnotationView? {
        if let annotation = annotation as? JLYServiceItem {
              
            let identifier = "pin"
            var view: MKPinAnnotationView
            if let dequeuedView = mapView.dequeueReusableAnnotationViewWithIdentifier(identifier) as? MKPinAnnotationView { // 2
                dequeuedView.annotation = annotation
                view = dequeuedView
            } else {
                // 3
                view = MKPinAnnotationView(annotation: annotation, reuseIdentifier: identifier)
                view.canShowCallout = true
                view.calloutOffset = CGPoint(x: -5, y: 5)
                view.rightCalloutAccessoryView = UIButton(type: .DetailDisclosure) as UIView
            }
            
            view.pinColor = annotation.pinColor()
            
            return view
        }
        return nil
    }
    
    func mapView(mapView: MKMapView, annotationView view: MKAnnotationView, calloutAccessoryControlTapped control: UIControl) {
        
        EngagementAgent.shared().startActivity("DetailsActivity",extras: nil)
        
        let location = view.annotation as! JLYServiceItem
        currentItem = recycleArray.indexOf(location)!
        DetailsOverlay.shared.showOverlay(self.view, isLandscape:  UIDeviceOrientationIsLandscape(UIDevice.currentDevice().orientation))
    }
}


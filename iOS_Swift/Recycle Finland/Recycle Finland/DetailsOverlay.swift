//
//  DetailsOverlay.swift
//  Recycle Finland
//
//  Created by Jukka Silvennoinen on 04/02/16.
//  Copyright © 2016 Jukka Silvennoinen. All rights reserved.
//

import Foundation
import UIKit
import MapKit

enum WhichItem {
    case current, next, previous
}

protocol DetailsOverlayDelegate: class {
    func getRecyclePlace(which : WhichItem) -> JLYServiceItem?
}

public class DetailsOverlay : UIViewController,  UITableViewDataSource, UITableViewDelegate{
    
    var overlayView : UIView!
    var navigationBar : UINavigationBar!
    var isShowing : Bool = false
    weak var delegate:DetailsOverlayDelegate?
    
    var currentItem : JLYServiceItem?
    var tableView : UITableView!
    var detailArray = [DetailItem]()
    
    class var shared: DetailsOverlay {
        struct Static {
            static let instance: DetailsOverlay = DetailsOverlay()
        }
        return Static.instance
    }
    
    public func showOverlay(view: UIView,isLandscape : Bool) {
        
        // my iPhone 4 with OS7.1 gives there wrong way, but my iPhone 6 with OS 9.2 gives it right, so fixing it here
        let vHeight = ((isLandscape && (view.frame.height > view.frame.width )) ? view.frame.width : view.frame.height)
        let vWidth = ((isLandscape && (view.frame.height > view.frame.width )) ? view.frame.height : view.frame.width)
        
        if(overlayView == nil){
        
            overlayView = UIView()
            
            let swipeRight = UISwipeGestureRecognizer(target: self, action: "respondToSwipeGesture:")
            swipeRight.direction = UISwipeGestureRecognizerDirection.Right
            overlayView.addGestureRecognizer(swipeRight)
            
            let swipeLeft = UISwipeGestureRecognizer(target: self, action: "respondToSwipeGesture:")
            swipeLeft.direction = UISwipeGestureRecognizerDirection.Left
            overlayView.addGestureRecognizer(swipeLeft)
            
            navigationBar = UINavigationBar();
            navigationBar.backgroundColor = UIColor.whiteColor()
            
            // Create a navigation item with a title
            let navigationItem = UINavigationItem()
            
            let leftButton =  UIBarButtonItem(title: "Back", style:   UIBarButtonItemStyle.Done, target: self, action: "backButtonAction:")
            navigationItem.leftBarButtonItem = leftButton
            navigationBar.items = [navigationItem]
            
            // Make the navigation bar a subview of the current view controller
            overlayView.addSubview(navigationBar)
            
            tableView = UITableView();
            tableView.backgroundColor = UIColor(red: 0xFF, green: 0xFF, blue: 0xFF, alpha: 0.0)
            tableView.dataSource = self
            tableView.delegate = self
            tableView.allowsSelection = false
            overlayView.addSubview(tableView)
        }
        
        overlayView.frame = CGRectMake(0,0,vWidth,vHeight)
        overlayView.backgroundColor = UIColor(red: 0xFF, green: 0xFF, blue: 0xFF, alpha: 0.7)
        
        navigationBar.frame = CGRectMake(0, 0, overlayView.frame.size.width, 60) // Offset by 20 pixels vertically to take the status bar into account

        tableView.frame = CGRectMake(0, navigationBar.frame.height, overlayView.frame.size.width, (overlayView.frame.size.height - navigationBar.frame.height))

        view.addSubview(overlayView)
        isShowing = true
        showItem(delegate?.getRecyclePlace(WhichItem.current))
    }
    
    
    func respondToSwipeGesture(gesture: UIGestureRecognizer) {
        
        if let swipeGesture = gesture as? UISwipeGestureRecognizer {

            switch swipeGesture.direction {
            case UISwipeGestureRecognizerDirection.Right:
                print("Swiped right")
                showItem(delegate?.getRecyclePlace(WhichItem.next))
                break
            case UISwipeGestureRecognizerDirection.Left:
                print("Swiped left")
                showItem(delegate?.getRecyclePlace(WhichItem.previous))
                break
            default:
                break
            }
        }
    }
    
    func showItem(item : JLYServiceItem?){
        
        currentItem = item
        
        navigationBar.topItem?.title = currentItem?.title
      
        detailArray.removeAll()
        
        if let contactText = currentItem?.contact{
            detailArray.append(DetailItem(image: "contactButtonImage" , text : contactText))
        }
        
        if let addresText = currentItem?.fullAddress(){
            detailArray.append(DetailItem(image: "mailButtonImage" , text : addresText))
            detailArray.append(DetailItem(buttontext : "Navigate to"))
        }
        
        if let openText = currentItem?.openTimes{
            detailArray.append(DetailItem(image:  "timeButtonImage" , text : openText))
        }
        
        for typeInt in (currentItem?.materialTypes)! {
            if let materialName = JLYConstants.materialTypes[typeInt]{
                detailArray.append(DetailItem(text : materialName))
            }
        }
        
        tableView.reloadData()
    }
    
    public func tableView(tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return detailArray.count
    }
    
    public func tableView(tableView: UITableView, cellForRowAtIndexPath indexPath: NSIndexPath) -> UITableViewCell {
        
        let myCell = UITableViewCell(style: UITableViewCellStyle.Default, reuseIdentifier: "myIdentifier")
        myCell.backgroundColor = UIColor(red: 0xFF, green: 0xFF, blue: 0xFF, alpha: 0.0)
        
        if detailArray[indexPath.row].isButton {
            
            let button : UIButton = UIButton(type: UIButtonType.System) as UIButton
            button.frame = CGRectMake(0,0, tableView.frame.width - 40, 40)
          
            let cellHeight: CGFloat = 44.0
            button.center = CGPoint(x: view.bounds.width / 2.0, y: cellHeight / 2.0)
        
            button.addTarget(self, action: "navigateToButtonAction:", forControlEvents: UIControlEvents.TouchUpInside)
            button.setTitle(detailArray[indexPath.row].text, forState: UIControlState.Normal)
            button.titleLabel!.font =  UIFont(name: "...", size: 25)
            
            myCell.addSubview(button)
            
        } else {
        
            myCell.textLabel?.text = detailArray[indexPath.row].text
            myCell.textLabel?.textAlignment = .Left
            myCell.imageView?.image = detailArray[indexPath.row].icon
        }
        
        return myCell
    }
    
    func navigateToButtonAction(sender:UIButton!)
    {
        if let mapitem = currentItem?.mapItem() {
            
            EngagementAgent.shared().sendSessionEvent("NavigateTo", extras: nil)
            
            let launchOptions = [MKLaunchOptionsDirectionsModeKey: MKLaunchOptionsDirectionsModeDriving]
            mapitem.openInMapsWithLaunchOptions(launchOptions)
        }
    }
    
    func  backButtonAction(sender: UIBarButtonItem){
        print("back button pressed !!")
        hideOverlayView()
    }
    
    public func hideOverlayView() {
        EngagementAgent.shared().startActivity("MainActivity",extras: nil)
        
        isShowing = false
        overlayView.removeFromSuperview()
    }
}


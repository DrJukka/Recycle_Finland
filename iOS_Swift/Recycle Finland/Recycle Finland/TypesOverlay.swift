//
//  TypesOverlay.swift
//  Recycle Finland
//
//  Created by Jukka Silvennoinen on 04/02/16.
//  Copyright © 2016 Jukka Silvennoinen. All rights reserved.
//

import Foundation
import UIKit

protocol TypesOverlayDelegate: class {
    func selectedTypeChanged(selectedType : Int)
}

public class TypesOverlay : UIViewController,  UITableViewDataSource, UITableViewDelegate {
    

    private var overlayView : UIView!
    private var navigationBar : UINavigationBar!
    private var tableView : UITableView!
  
    var selectedType : Int = 0;
    var isShowing : Bool = false
    weak var delegate:TypesOverlayDelegate?
    
    class var shared: TypesOverlay {
        struct Static {
            static let instance: TypesOverlay = TypesOverlay()
        }
        return Static.instance
    }
    
    
    public func showOverlay(view: UIView,isLandscape : Bool) {
        
        // my iPhone 4 with OS7.1 gives there wrong way, but my iPhone 6 with OS 9.2 gives it right, so fixing it here
        let vHeight = ((isLandscape && (view.frame.height > view.frame.width )) ? view.frame.width : view.frame.height)
        let vWidth = ((isLandscape && (view.frame.height > view.frame.width )) ? view.frame.height : view.frame.width)
        
        if(overlayView == nil){
            overlayView = UIView()
            navigationBar = UINavigationBar();
            
            navigationBar.backgroundColor = UIColor.whiteColor()
            
            // Create a navigation item with a title
            let navigationItem = UINavigationItem()
            navigationItem.title = "Select type"
            
            // Create left and right button for navigation item
            let leftButton =  UIBarButtonItem(title: "Back", style:   UIBarButtonItemStyle.Done, target: self, action: "backButtonAction:")
            
            // Create two buttons for the navigation item
            navigationItem.leftBarButtonItem = leftButton
            
            // Assign the navigation item to the navigation bar
            navigationBar.items = [navigationItem]
            
            // Make the navigation bar a subview of the current view controller
            overlayView.addSubview(navigationBar)
            
            tableView = UITableView();
            tableView.backgroundColor = UIColor(red: 0xFF, green: 0xFF, blue: 0xFF, alpha: 0.0)
            tableView.dataSource = self
            tableView.delegate = self
        
            overlayView.addSubview(tableView)
            
        }
        
        overlayView.frame = CGRectMake(0,0,vWidth,vHeight)
        
        overlayView.backgroundColor = UIColor(red: 0xFF, green: 0xFF, blue: 0xFF, alpha: 0.7)
    
        navigationBar.frame = CGRectMake(0, 0, overlayView.frame.size.width, 60) // Offset by 20 pixels vertically to take the status bar into account
        
        tableView.frame = CGRectMake(0, navigationBar.frame.height, overlayView.frame.size.width, (overlayView.frame.size.height - navigationBar.frame.height))
        
        view.addSubview(overlayView)
        isShowing = true
        
        if let selectIndex = JLYConstants.materialTypeeOrder.indexOf(selectedType){
            let rowToSelect:NSIndexPath = NSIndexPath(forRow: selectIndex, inSection: 0);
            tableView.selectRowAtIndexPath(rowToSelect, animated: true, scrollPosition: UITableViewScrollPosition.None);
        }
    }
    
    public func tableView(tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return JLYConstants.materialTypeeOrder.count
    }
    
    public func tableView(tableView: UITableView, cellForRowAtIndexPath indexPath: NSIndexPath) -> UITableViewCell {
       
        let myCell = UITableViewCell(style: UITableViewCellStyle.Default, reuseIdentifier: "myIdentifier")
        myCell.backgroundColor = UIColor(red: 0xFF, green: 0xFF, blue: 0xFF, alpha: 0.0)
        myCell.textLabel?.text = JLYConstants.materialTypes[JLYConstants.materialTypeeOrder[indexPath.row]]
        myCell.textLabel?.textAlignment = .Center
       
        return myCell
    }

    public func tableView(tableView: UITableView, didSelectRowAtIndexPath indexPath: NSIndexPath) {
        
        let typeIndex : Int = tableView.indexPathForSelectedRow!.row
        
        if(typeIndex >= 0 && typeIndex < JLYConstants.materialTypeeOrder.count){
            
            selectedType = JLYConstants.materialTypeeOrder[typeIndex]
            delegate?.selectedTypeChanged(selectedType)
            
        }else{
            print("index out of range " + "\(typeIndex)")
        }
    }
    
    
    func  backButtonAction(sender: UIBarButtonItem){
        hideOverlayView()
    }
    
    public func hideOverlayView() {
        EngagementAgent.shared().startActivity("MainActivity",extras: nil)
        
        isShowing = false
        overlayView.removeFromSuperview()
    }
}

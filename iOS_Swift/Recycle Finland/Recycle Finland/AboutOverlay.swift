//
//  AboutOverlay.swift
//  Recycle Finland
//
//  Created by Jukka Silvennoinen on 03/02/16.
//  Copyright © 2016 Jukka Silvennoinen. All rights reserved.
//

import Foundation
import UIKit

// http://stackoverflow.com/questions/27960556/loading-an-overlay-when-running-long-tasks-in-ios

public class AboutOverlay : UIViewController{
    
    var overlayView : UIView!
    var navigationBar : UINavigationBar!
    var aboutLabel : UILabel!
    var isShowing : Bool = false
    
    class var shared: AboutOverlay {
        struct Static {
            static let instance: AboutOverlay = AboutOverlay()
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
            navigationItem.title = "Recycle Finland"
        
            // Create left and right button for navigation item
            let leftButton =  UIBarButtonItem(title: "Back", style:   UIBarButtonItemStyle.Done, target: self, action: "backButtonAction:")
        
            // Create two buttons for the navigation item
            navigationItem.leftBarButtonItem = leftButton
        
            // Assign the navigation item to the navigation bar
            navigationBar.items = [navigationItem]
        
            // Make the navigation bar a subview of the current view controller
            overlayView.addSubview(navigationBar)
            
            aboutLabel = UILabel()
            
            aboutLabel.lineBreakMode = .ByWordWrapping // or NSLineBreakMode.ByWordWrapping
            aboutLabel.numberOfLines = 0
            aboutLabel.textAlignment = NSTextAlignment.Center
            aboutLabel.text = "Recycling data provided by JLY (www.jly.fi/), mobile application implemented by Dr.Jukka (www.DrJukka.com), all rights reserved.\n\r\n\r Version: 1.01 March 22th 2016.";
            
            overlayView.addSubview(aboutLabel)
            
        }
    
        overlayView.frame = CGRectMake(0,0,vWidth,vHeight)
        
        overlayView.backgroundColor = UIColor(red: 0xFF, green: 0xFF, blue: 0xFF, alpha: 0.7)
        //overlayView.center = view.center
        
        navigationBar.frame = CGRectMake(0, 0, overlayView.frame.size.width, 60) // Offset by 20 pixels vertically to take the status bar into account

        aboutLabel.frame = CGRectMake(0, navigationBar.frame.height, overlayView.frame.size.width, (overlayView.frame.size.height - navigationBar.frame.height))
        aboutLabel.center = overlayView.center
        
        view.addSubview(overlayView)
        isShowing = true
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
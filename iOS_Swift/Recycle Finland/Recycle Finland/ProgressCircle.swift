//
//  ProgressCircle.swift
//  Recycle Finland
//
//  Created by Jukka Silvennoinen on 03/02/16.
//  Copyright © 2016 Jukka Silvennoinen. All rights reserved.
//

import Foundation
import UIKit

// http://stackoverflow.com/questions/27960556/loading-an-overlay-when-running-long-tasks-in-ios

public class ProgressCircle{
    
    var overlayView : UIView!
    var activityIndicator : UIActivityIndicatorView!
    var isShowing : Bool = false
    
    class var shared: ProgressCircle {
        struct Static {
            static let instance: ProgressCircle = ProgressCircle()
        }
        return Static.instance
    }
    
    public func showOverlay(view: UIView,isLandscape : Bool) {
        
        // my iPhone 4 with OS7.1 gives there wrong way, but my iPhone 6 with OS 9.2 gives it right, so fixing it here
        //let vHeight = ((isLandscape && (view.frame.height > view.frame.width )) ? view.frame.width : view.frame.height)
       // let vWidth = ((isLandscape && (view.frame.height > view.frame.width )) ? view.frame.height : view.frame.width)
        
        if(overlayView == nil){
            overlayView = UIView()
            activityIndicator = UIActivityIndicatorView()
            
            overlayView.frame = CGRectMake(0, 0, 80, 80)
            overlayView.backgroundColor = UIColor(red: 0x00, green: 0xff, blue: 0x00, alpha: 0.5)
            overlayView.clipsToBounds = true
            overlayView.layer.cornerRadius = 10
            
            activityIndicator.frame = CGRectMake(0, 0, 40, 40)
            activityIndicator.activityIndicatorViewStyle = .WhiteLarge
            activityIndicator.center = CGPointMake(overlayView.bounds.width / 2, overlayView.bounds.height / 2)
            overlayView.addSubview(activityIndicator)
        }
        
        overlayView.center = view.center
        
        view.addSubview(overlayView)
        activityIndicator.startAnimating()
    }


    
    public func hideOverlayView() {
        isShowing = false
        overlayView.removeFromSuperview()
        activityIndicator.stopAnimating()
    }
}

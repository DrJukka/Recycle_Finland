//
//  DetailControl.swift
//  Recycle Finland
//
//  Created by Jukka Silvennoinen on 08/02/16.
//  Copyright © 2016 Jukka Silvennoinen. All rights reserved.
//

import Foundation
import UIKit

public class DetailItem: NSObject{
    
    var icon : UIImage?
    var text : String!
    var isButton : Bool = false
    
    public init(image : String, text :  String) {
        self.icon = UIImage(named: image)
        self.text = text
    }
    
    public init(text :  String) {
        self.text = text
    }
    
    public init(buttontext : String) {
        self.text = buttontext
        self.isButton = true
    }
}

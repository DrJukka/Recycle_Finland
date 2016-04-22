package com.drjukka.recyclefinland;

import android.app.Application;
import android.content.Context;

/**
 * Created by juksilve on 2.2.2016.
 */
public class MyApp extends Application {

    private EngagementAPI mEngagementAPI;

    public EngagementAPI getEngagementAPI(){
        if(mEngagementAPI == null){
            mEngagementAPI = new EngagementAPI(this);
        }
        return mEngagementAPI;
    }

}
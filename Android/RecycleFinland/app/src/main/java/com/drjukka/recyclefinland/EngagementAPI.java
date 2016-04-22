package com.drjukka.recyclefinland;

import android.app.Activity;
import android.content.Context;
import android.os.Bundle;
import android.util.Log;

import com.microsoft.azure.engagement.EngagementAgent;
import com.microsoft.azure.engagement.EngagementConfiguration;
import com.microsoft.azure.engagement.reach.EngagementReachAgent;

import java.lang.reflect.Array;
import java.util.ArrayList;

/**
 * Created by juksilve on 2.2.2016.
 */

// Server API Key : AIzaSyCuUjmwLDX0BvdAoNjU-gYxd1pvmqrYZhI
// Sender ID : 1030014324257

//https://azure.microsoft.com/en-us/documentation/articles/mobile-engagement-android-integrate-engagement-reach/
// https://azure.microsoft.com/en-us/documentation/articles/mobile-engagement-android-get-started/
//Enable your app to receive GCM push notifications

public class EngagementAPI {

    final private String TAG = "EngagementAPI";
    final private Context mContext;
    private final EngagementAgent mAgent;
    final private String mConnectionString = "ADD_YOUR_VALUE_HERE";

    private boolean mIsActivityActive = false;
    final private ArrayList<String> mStartedJobs = new ArrayList<>();

    public EngagementAPI(Context context){
        this.mContext = context;
        mAgent = EngagementAgent.getInstance(this.mContext);
    }

    public void setEnabled(boolean enabled){
        mAgent.setEnabled(enabled);
    }

    public void init(){
        EngagementConfiguration engagementConfiguration = new EngagementConfiguration();
        engagementConfiguration.setConnectionString(mConnectionString);
        engagementConfiguration.setLazyAreaLocationReport(true);
        Log.d(TAG, "**** Engagement init : ");
        mAgent.init(engagementConfiguration);
    }

    public void close(){

        for(String name : mStartedJobs){
            mAgent.endJob(name);
        }

        if(mIsActivityActive){
            mIsActivityActive = false;
            mAgent.endActivity();
        }
        Log.d(TAG, "**** Engagement closed : ");
    }

    public void startActivity(Activity activity, String name){
        if(mIsActivityActive){
            stopActivity();
        }
        mIsActivityActive = true;
        Log.d(TAG,"**** Starting  activity : " + name);
        mAgent.startActivity(activity, name, null);
    }

    public void stopActivity(){
        mIsActivityActive = false;
        Log.d(TAG,"**** Stopping activity");
        mAgent.endActivity();
    }

    public void startJob(final String name, final Bundle extras) {
        endJob(name);
        mStartedJobs.add(name);
        Log.d(TAG, "**** starting job : " + name);
        mAgent.startJob(name, extras);
    }

    public void endJob(final String name) {

        if(mStartedJobs.contains(name)){
            mStartedJobs.remove(name);
            Log.d(TAG,"**** ending job : " + name);
            mAgent.endJob(name);
        }
    }
    public void sendSessionEvent(final String name, final Bundle extras) {
        Log.d(TAG,"**** sending session event : " + name);
        mAgent.sendSessionEvent(name, extras);
    }
}

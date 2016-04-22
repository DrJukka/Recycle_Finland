package com.drjukka.recyclefinland;

import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import com.google.android.gms.maps.model.LatLng;

/**
 * Created by juksilve on 31.1.2016.
 */
public class RecycleSettings {

    final private String zoomKey = "zoonLevel";
    final private String typeKey = "selectedType";
    final  private String latKey = "latitude";
    final  private String lonKey = "longitude";
    final  private Double latValue = 60.1708;
    final  private Double lonValue = 24.9375;
    final  private float zoomValue = 12;
    final  private int typeValue = 0;
    final  private Activity context;

    private LatLng mLocation;
    private float mZoonLevel;
    private int mSelectedType;

    public RecycleSettings(Activity context){
        this.context = context;
        loadSettings();
    }

    public void storeSettings(LatLng location, float zoonLevel, int selectedType){
        SharedPreferences sharedPref = context.getPreferences(Context.MODE_PRIVATE);
        SharedPreferences.Editor editor = sharedPref.edit();
        editor.putFloat(zoomKey, zoonLevel);
        editor.putInt(typeKey, selectedType);

        putDouble(editor, latKey, location.latitude);
        putDouble(editor, lonKey, location.longitude);

        editor.commit();
    }

    public void loadSettings(){
        SharedPreferences sharedPref = context.getPreferences(Context.MODE_PRIVATE);

        Double latTmp= getDouble(sharedPref,latKey,latValue);
        Double lonTmp= getDouble(sharedPref,lonKey,lonValue);
        mLocation = new LatLng(latTmp,lonTmp);

        mZoonLevel = sharedPref.getFloat(zoomKey, zoomValue);
        mSelectedType = sharedPref.getInt(typeKey, typeValue);
    }

    public LatLng getLocation(){ return mLocation;}
    public float getZoonLevel(){ return mZoonLevel;}
    public int getSelectedType(){ return mSelectedType;}

    private Editor putDouble(final Editor edit, final String key, final double value) {
        return edit.putLong(key, Double.doubleToRawLongBits(value));
    }

    private double getDouble(final SharedPreferences prefs, final String key, final double defaultValue) {
        if ( !prefs.contains(key))
            return defaultValue;

        return Double.longBitsToDouble(prefs.getLong(key, Double.doubleToLongBits(defaultValue)));
    }
}



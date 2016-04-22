package com.drjukka.recyclefinland;

import com.google.android.gms.maps.model.LatLng;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Dictionary;
import java.util.HashMap;
import java.util.Map;

/**
 * Created by juksilve on 29.1.2016.
 */
public class JLYServiceItem {

    private String mLocationId;
    private ArrayList<Integer> mMaterialTypes = new ArrayList<Integer>();
    private String mName;
    private String mAddress;
    private String mPostalCode;
    private String mCity;
    private LatLng mGeoPoint;
    private String mDistance;
    private String mOperator;
    private int mManned;
    private String mOpenTimes;
    private String mContact;

    public String getDisplayName() { return mName;  }
    public LatLng getLocation() { return mGeoPoint;  }
    public String getLocationId() { return mLocationId;  }
    public ArrayList<Integer> getMatrialTypes() { return mMaterialTypes;  }

    public String getAddress() { return mAddress;  }
    public String getPostalCode() { return mPostalCode;  }
    public String getCity() { return mCity;  }

    public String getDistance() { return mDistance;  }
    public String getOperator() { return mOperator;  }
    public int getManned() { return mManned;  }
    public String getOpenTimes() { return mOpenTimes;  }
    public String getContact() { return mContact; }

    private JLYServiceItem(String locationId)
    {
        mLocationId = locationId;
    }

    public void updateItem(Map<String, String> data)
    {
        try{
            String laji_idString = data.get(JLYConstants.laji_id);
            if(laji_idString != null) {
                int laji_idInt = Integer.parseInt(laji_idString);
                mMaterialTypes.add(laji_idInt);
            }
        }catch (Exception ex){}

        mName = data.get(JLYConstants.nimi);
        mAddress = data.get(JLYConstants.osoite);
        mPostalCode = data.get(JLYConstants.pnro);
        mCity = data.get(JLYConstants.paikkakunta);

        try{
            String latString = data.get(JLYConstants.lat);
            String lngString = data.get(JLYConstants.lng);
            if (latString != null && lngString != null)
            {
                Double latDouble = Double.parseDouble(latString);
                Double lngDouble = Double.parseDouble(lngString);
                mGeoPoint = new LatLng(latDouble, lngDouble);
            }
        }catch (Exception ex){}

        mDistance = data.get(JLYConstants.etaisyys);
        mOperator = data.get(JLYConstants.yllapitaja);

        try{
            String miehitysString = data.get(JLYConstants.miehitys);
            if(miehitysString != null){
                mManned = Integer.parseInt(miehitysString);
            }
        }catch (Exception ex){}

        mOpenTimes = data.get(JLYConstants.aukiolo);
        mContact = data.get(JLYConstants.yhteys);
    }

    static public JLYServiceItem createServiceItem(Map<String, String> data)
    {
        JLYServiceItem tmpItem = new JLYServiceItem(data.get(JLYConstants.paikka_id));
        tmpItem.updateItem(data);
        return tmpItem;
    }
}

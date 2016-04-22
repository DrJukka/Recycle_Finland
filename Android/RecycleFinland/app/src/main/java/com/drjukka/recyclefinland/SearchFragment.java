package com.drjukka.recyclefinland;


import android.app.Activity;
import android.location.Address;
import android.location.Geocoder;
import android.support.v4.app.Fragment;
import android.os.Bundle;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.TextView;

import com.google.android.gms.maps.model.LatLng;

import java.util.List;
import java.util.Locale;

/**
 * Created by juksilve on 28.1.2016.
 */
public class SearchFragment extends Fragment {

    static public String TAG = "SearchFragment";
    private EngagementAPI mEngagement;

    public interface SearchCallback{
        LatLng getMapCenter();
        void showAboutView();
        void startSearchWithLocation(LatLng point, boolean addGeoMarker);
    }

    private SearchCallback mCallback;
    private View mView;
    private EditText mDisplay_text;
    private Geocoder mGeocoder;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        mEngagement = ((MyApp)getActivity().getApplicationContext()).getEngagementAPI();

        mView=inflater.inflate(R.layout.search_fragment, container,false);
        mDisplay_text=(EditText) mView.findViewById(R.id.editText1);

        ImageButton aboutButton = (ImageButton) mView.findViewById(R.id.aboutButton);
        aboutButton.setOnClickListener(new View.OnClickListener()   {
            public void onClick(View v)  {
                mCallback.showAboutView();
            }
        });

        Button searchButton= (Button) mView.findViewById(R.id.searchButton);
        searchButton.setOnClickListener(new View.OnClickListener()   {
            public void onClick(View v)  {
                startGeoCoding();
            }
        });

        mGeocoder = new Geocoder(getActivity().getApplicationContext(), Locale.ENGLISH);

        return mView;
    }

    @Override
    public void onAttach(Activity activity) {
        super.onAttach(activity);

        // This makes sure that the container activity has implemented
        // the callback interface. If not, it throws an exception
        try {
            mCallback = (SearchCallback) activity;
        } catch (ClassCastException e) {
            throw new ClassCastException(activity.toString()+ " must implement SearchCallback");
        }
    }

    private void startGeoCoding(){

        if(mDisplay_text != null && mDisplay_text.getText() != null){
            String searchText = mDisplay_text.getText().toString();
            if(searchText.length() > 0){

                Bundle tmpBundle = new Bundle();
                tmpBundle.putString("SearchText",searchText);
                mEngagement.sendSessionEvent("SeachAddress",tmpBundle);

                Log.d(TAG,"Geocoder searchText: " + searchText);
                try {
                    List<Address> addresses = mGeocoder.getFromLocationName(searchText, 1);
                    Log.d(TAG,"Geocoder done ");
                    if(addresses != null &&addresses.size() > 0){
                        Address useAddress = addresses.get(0);

                        Log.d(TAG,"Geocoder found " + useAddress.getCountryName());

                        mCallback.startSearchWithLocation(new LatLng(useAddress.getLatitude(), useAddress.getLongitude()), true);
                    }
                }catch (Exception ex){
                    Log.d(TAG,"Geocoder error : " + ex.toString());
                }
            }
        }
    }
}

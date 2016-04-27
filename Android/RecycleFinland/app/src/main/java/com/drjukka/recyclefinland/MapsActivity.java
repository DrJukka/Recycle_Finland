package com.drjukka.recyclefinland;

import android.Manifest;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.content.res.Configuration;
import android.hardware.SensorManager;
import android.net.Uri;
import android.os.Build;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.os.Bundle;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.util.Log;
import android.view.OrientationEventListener;
import android.view.View;
import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.widget.LinearLayout;
import android.app.AlertDialog;
import android.widget.Toast;

import com.google.android.gms.maps.model.LatLng;

import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

@TargetApi(18)
@SuppressLint("NewApi")

public class MapsActivity extends FragmentActivity // EngagementFragmentActivity
implements SearchFragment.SearchCallback ,MyMapFragment.MapsCallback, TypesFragment.TypesCallback
        , JLYRestService.JLYRestServiceCallback,DetailsFragment.DetailsCallback {

    private final String TAG = "MapsActivity";
    private static final int PERMISSION_REQUEST_FINE_LOCATION = 1;
    private static final int PERMISSION_WRITE_EXTERNAL_STORAGE= 2;

    private EngagementAPI mEngagement;

    private FragmentManager mManager = null;
    private AboutFragment mAbout = null;
    private TypesFragment mTypes = null;
    private DetailsFragment mDetails = null;
    private MyMapFragment mMap = null;
    private SearchFragment mSearch = null;

    private LinearLayout mProgressSpinner;
    private JLYRestService mJLYRestService;
    private RecycleSettings mSettings;
    private ArrayList<JLYServiceItem> mArray;
    private int mSelectedIndex;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        mEngagement = ((MyApp)getApplicationContext()).getEngagementAPI();

        mSettings = new RecycleSettings(this);
        mProgressSpinner = (LinearLayout) findViewById(R.id.linlaHeaderProgress);

        mJLYRestService = new JLYRestService();
        mSearch = new SearchFragment();
        mMap = new MyMapFragment();
        mAbout = new AboutFragment();

        mTypes = new TypesFragment();
        mTypes.setSelectedType(mSettings.getSelectedType());

        mDetails = new DetailsFragment();
        mManager = getSupportFragmentManager();//create an instance of fragment manager

        FragmentTransaction transaction = mManager.beginTransaction();//create an instance of Fragment-transaction
        transaction.add(R.id.search_container, mSearch, "Search");
        transaction.add(R.id.maps_container, mMap, "Map");
        transaction.commit();

        //API is not available before Marchmellow
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            //we need to get user to grant access to the ACCESS_COARSE_LOCATION, beacon scanning requires this
            if (this.checkSelfPermission(Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
                // so if its not granted yet, we do need the request it
                final AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.setTitle("This app needs location access");
                builder.setMessage("Please grant location access so this app can locate nearby recycling places.");
                builder.setPositiveButton(android.R.string.ok, null);
                builder.setOnDismissListener(new DialogInterface.OnDismissListener() {
                    @Override
                    public void onDismiss(DialogInterface dialog) {
                        requestPermissions(new String[]{Manifest.permission.ACCESS_COARSE_LOCATION}, PERMISSION_REQUEST_FINE_LOCATION);

                        if (checkSelfPermission(android.Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED) {
                            requestPermissions(new String[]{android.Manifest.permission.WRITE_EXTERNAL_STORAGE}, PERMISSION_WRITE_EXTERNAL_STORAGE);
                        }
                    }
                });
                builder.show();
            }
        }
    }

    @Override
    protected void onPause() {
        super.onPause();
        removeTopFragments();
        if (mSettings != null && mMap != null && mTypes != null) {
            mSettings.storeSettings(mMap.getMapCenter(), mMap.getMapZoomLevel(), mTypes.getSelectedType());
        }
        mEngagement.close();
    }

    @Override
    public void onResume() {
        super.onResume();

        mEngagement.init();
        mEngagement.startActivity(this, "MainActivity");
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
    }

    @Override
    public void onRequestPermissionsResult(int requestCode,String permissions[], int[] grantResults) {
        switch (requestCode) {
            case PERMISSION_REQUEST_FINE_LOCATION: {
                if (grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    //"coarse location permission granted");
                } else {
                    final AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.setTitle("Functionality limited");
                    builder.setMessage("Since location access has not been granted, this app will not be able to find your location.");
                    builder.setPositiveButton(android.R.string.ok, null);
                    builder.setOnDismissListener(new DialogInterface.OnDismissListener() {

                        @Override
                        public void onDismiss(DialogInterface dialog) {
                        }

                    });
                    builder.show();
                }
                return;
            }
            case PERMISSION_WRITE_EXTERNAL_STORAGE:{
                //we'll just lose some big picture stuff, thus lets not worry user about it..
                break;
            }
        }
    }

    @Override
    public void onBackPressed() {

        if (!removeTopFragments()) {
            // we did not have any fragments to remove, so we'll exit.
            super.onBackPressed();
        }
    }

    public boolean removeTopFragments() {

        //lets get back to main activity
        mEngagement.startActivity(this, "MainActivity");

        List<Fragment> fragments = mManager.getFragments();
        for (Fragment fragment : fragments) {
            if (fragment != null) {
                if (fragment == mAbout || fragment == mTypes || fragment == mDetails) {
                    FragmentTransaction transaction = mManager.beginTransaction();//create an instance of Fragment-transaction
                    transaction.remove(fragment);
                    transaction.commit();
                    return true;
                }
            }
        }


        return false;
    }


    @Override
    public void showTypesList() {

        mEngagement.startActivity(this,"TypesSelectionView");

        FragmentTransaction transaction = mManager.beginTransaction();//create an instance of Fragment-transaction
        transaction.add(R.id.types_container, mTypes, "Types");
        transaction.commit();
    }

    @Override
    public RecycleSettings getSettings() {
        return mSettings;
    }

    @Override
    public LatLng getMapCenter() {
        return mMap != null ? mMap.getMapCenter() : null;
    }

    @Override
    public void showAboutView() {

        mEngagement.startActivity(this,"AboutView");

        FragmentTransaction transaction = mManager.beginTransaction();//create an instance of Fragment-transaction
        transaction.add(R.id.about_container, mAbout, "About");
        transaction.commit();
    }

    @Override
    public void showDetails(String locationID) {

        if (mArray == null || mArray.size() <= 0) {
            return;
        }

        for(int i = 0; i < mArray.size(); i++) {

            if(locationID.equalsIgnoreCase(mArray.get(i).getLocationId())){
                mSelectedIndex = i;
                mEngagement.startActivity(this,"DetailsView");

                FragmentTransaction transaction = mManager.beginTransaction();//create an instance of Fragment-transaction
                transaction.add(R.id.details_container, mDetails, "Details");
                transaction.commit();
                return;
            }
        }
    }

    @Override
    public void materialTypeSelected(int type) {
        startSearchWithLocation(getMapCenter(), false);
    }

    @Override
    public void startSearchWithLocation(LatLng point, boolean addGeoMarker) {
        if (point == null || mMap == null) {
            return;
        }

        if (addGeoMarker) {
            mMap.addGeoMarker(point);
        }

        Bundle tmpBundle = new Bundle();
        tmpBundle.putDouble("latitude", point.latitude);
        tmpBundle.putDouble("longitude", point.longitude);
        tmpBundle.putInt("type", mTypes.getSelectedType());

        mEngagement.startJob("FindNearby",tmpBundle);

        mProgressSpinner.setVisibility(View.VISIBLE);
        mJLYRestService.findNearby(point.latitude, point.longitude, mTypes.getSelectedType(), 0, 0, this);
    }

    @Override
    public void searchFinished(final ArrayList<JLYServiceItem> array) {

        mEngagement.endJob("FindNearby");

        if (mMap == null) {
            return;
        }

        mArray = array;
        mSelectedIndex =0;

        runOnUiThread(new Thread(new Runnable() {
            public void run() {
                mProgressSpinner.setVisibility(View.GONE);
                mMap.addMarkers(array);
            }
        }));
    }

    @Override
    public void startNavigation(final JLYServiceItem destiny){

        if(destiny == null || destiny.getLocation() == null){
            return;
        }

        Bundle extras = new Bundle();
        extras.putString("DisplayName", destiny.getDisplayName());
        extras.putString("LocationId", destiny.getLocationId());

        sendSessionEvent("NavigateTo", extras);

        try {

            String uri = "google.navigation:q=%f, %f";
            Intent navIntent = new Intent(Intent.ACTION_VIEW, Uri.parse(String.format(Locale.US, uri, destiny.getLocation().latitude, destiny.getLocation().longitude)));
            startActivity(navIntent);
        }catch (Exception ex) {
            Toast.makeText(this, "Navigation failed, Please make sure Google Navigation is installed.", Toast.LENGTH_LONG).show();
        }
    }

    @Override
    public JLYServiceItem getRecyclePlace(WhichItem item) {
        if (mArray == null || mArray.size() <= 0) {
            return null;
        }

        if (item == WhichItem.next) {
            mSelectedIndex++;
        } else if (item == WhichItem.previous){
            mSelectedIndex--;
        }//else its the current one

        if (mSelectedIndex < 0) {
            mSelectedIndex = (mArray.size() - 1);
        }

        if (mArray.size() <= mSelectedIndex) {
            mSelectedIndex = 0;
        }

        JLYServiceItem selectedItem = mArray.get(mSelectedIndex);

        Bundle extras = new Bundle();
        extras.putString("DisplayName", selectedItem.getDisplayName());
        extras.putString("LocationId", selectedItem.getLocationId());
        sendSessionEvent("Show_POI", extras);

        //center the map to the selected recycling place
        final String locationId = selectedItem.getLocationId();
        runOnUiThread(new Thread(new Runnable() {
            public void run() {
                if (mMap == null) {
                    mMap.selectRecyclePlaceMarker(locationId);
                }
            }
        }));

        return selectedItem;
    }


    public void sendSessionEvent(final String name, final Bundle extras) {
        mEngagement.sendSessionEvent(name, extras);
    }
}

package com.drjukka.recyclefinland;


    import android.app.Activity;
    import android.location.Location;
    import android.os.Bundle;
    import android.support.v4.app.Fragment;
    import android.support.v4.app.FragmentManager;
    import android.util.Log;
    import android.view.LayoutInflater;
    import android.view.View;
    import android.view.ViewGroup;
    import android.widget.ImageButton;

    import com.google.android.gms.maps.CameraUpdateFactory;
    import com.google.android.gms.maps.GoogleMap;
    import com.google.android.gms.maps.SupportMapFragment;
    import com.google.android.gms.maps.model.CircleOptions;
    import com.google.android.gms.maps.model.LatLng;
    import com.google.android.gms.maps.model.Marker;
    import com.google.android.gms.maps.model.MarkerOptions;

    import java.util.ArrayList;

/**
 * Created by juksilve on 28.1.2016.
 */
    public class MyMapFragment extends SupportMapFragment implements GoogleMap.OnMyLocationChangeListener, GoogleMap.OnMyLocationButtonClickListener, GoogleMap.OnInfoWindowClickListener {


    public interface MapsCallback {
        void startSearchWithLocation(LatLng point, boolean addGeoMarker);
        void showDetails(String locationID);
        void showTypesList();
        RecycleSettings getSettings();
    }
    private EngagementAPI mEngagement;

    private final String TAG = "MyMapFragment";
    private MapsCallback mCallback;
    private View mView;
    private SupportMapFragment mFragment;
    private GoogleMap mMap;
    private LatLng mGeoPoint;
    private LatLng mCurrentLocation;
    private ImageButton mRefreshButton;
    private ImageButton mShowListButton;

    final private ArrayList<Marker> mCurrentMarkers = new ArrayList<>();

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        super.onCreateView(inflater, container, savedInstanceState);

        mEngagement = ((MyApp)getActivity().getApplicationContext()).getEngagementAPI();

        mView = inflater.inflate(R.layout.activity_maps, null, false);
        FragmentManager fm = getChildFragmentManager();
        mFragment = (SupportMapFragment) fm.findFragmentById(R.id.map);
        if (mFragment == null) {
            mFragment = SupportMapFragment.newInstance();
            fm.beginTransaction().replace(R.id.map, mFragment).commit();
        }

        mShowListButton= (ImageButton) mView.findViewById(R.id.showListButton);
        mShowListButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                mCallback.showTypesList();
            }
        });

        mRefreshButton= (ImageButton) mView.findViewById(R.id.refreshButton);
        mRefreshButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {

                mEngagement.sendSessionEvent("Refresh_data",null);
                //do-refresh for current location
                mCallback.startSearchWithLocation(getMapCenter(), false);
            }
        });

        return mView;
    }

    @Override
    public void onAttach(Activity activity) {
        super.onAttach(activity);

        // This makes sure that the container activity has implemented
        // the callback interface. If not, it throws an exception
        try {
            mCallback = (MapsCallback) activity;
        } catch (ClassCastException e) {
            throw new ClassCastException(activity.toString()+ " must implement MapsCallback");
        }
    }

    @Override
    public void onResume() {
        super.onResume();
        if (mMap == null) {
            mMap = mFragment.getMap();
            if (mMap != null) {
                mMap.setOnInfoWindowClickListener(this);
                //this should be read from settings!!
                mMap.moveCamera( CameraUpdateFactory.newLatLngZoom(mCallback.getSettings().getLocation() ,mCallback.getSettings().getZoonLevel()) );
                mMap.setOnMyLocationChangeListener(this);//
                mMap.setOnMyLocationButtonClickListener(this);
                try {
                    mMap.setMyLocationEnabled(true);
                } catch (SecurityException ex) {
                    Log.d(TAG, "setMyLocationEnabled exception : " + ex.toString());
                }

                mCallback.startSearchWithLocation(getMapCenter(), false);
            }else{
                mRefreshButton.setVisibility(View.GONE);
                mShowListButton.setVisibility(View.GONE);
            }
        }
    }

    public void selectRecyclePlaceMarker(final String locationId){

        for(Marker marker : mCurrentMarkers){
            if(marker != null && marker.getSnippet().equalsIgnoreCase(locationId)){
                mMap.moveCamera(CameraUpdateFactory.newLatLng(marker.getPosition()));
                marker.showInfoWindow();
                return;
            }
        }
    }

    public void addGeoMarker(LatLng point){
        if(mMap == null || point == null){
            return ;
        }
        setMapCenter(point);
        mGeoPoint = point;
        mMap.addCircle(new CircleOptions()
                .center(mGeoPoint)
                .radius(50));
    }

    public void addMarkers(ArrayList<JLYServiceItem> array){
        if(mMap == null || array == null){
            return ;
        }

        mCurrentMarkers.clear();
        mMap.clear();

        for(JLYServiceItem item  :array) {

            if(item != null) {
                mCurrentMarkers.add(mMap.addMarker(new MarkerOptions()
                        .position(item.getLocation())
                        .title(item.getDisplayName())
                        .snippet(item.getLocationId())));
            }
        }

        if(mGeoPoint != null) {
            mMap.addCircle(new CircleOptions()
                    .center(mGeoPoint)
                    .radius(50));
        }
    }

    public void setMapCenter(LatLng point){
        if(mMap == null){
            return ;
        }
        mMap.moveCamera(CameraUpdateFactory.newLatLng(point));
    }
    public float getMapZoomLevel(){
        return mMap != null ? mMap.getCameraPosition().zoom : null;
    }

    public LatLng getMapCenter(){
        return mMap != null ? mMap.getCameraPosition().target : null;
    }

    @Override
    public void onDetach() {
        super.onDetach();

        try {
            java.lang.reflect.Field childFragmentManager = Fragment.class.getDeclaredField("mChildFragmentManager");
            childFragmentManager.setAccessible(true);
            childFragmentManager.set(this, null);

        } catch (NoSuchFieldException e) {
            throw new RuntimeException(e);
        } catch (IllegalAccessException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public boolean onMyLocationButtonClick(){

        mEngagement.sendSessionEvent("locate_me",null);

        Log.d(TAG, "onMyLocationButtonClick clicked ");
        mCallback.startSearchWithLocation(mCurrentLocation, false);
        return false;
    }

    @Override
    public void onMyLocationChange(Location location) {
        mCurrentLocation = new LatLng(location.getLatitude(), location.getLongitude());
    }

    @Override
    public void onInfoWindowClick(Marker marker) {
        mCallback.showDetails(marker.getSnippet());
    }
}
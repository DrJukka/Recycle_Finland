package com.drjukka.recyclefinland;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.view.GestureDetector;
import android.view.LayoutInflater;
import android.view.MotionEvent;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.gms.maps.model.LatLng;

import java.util.ArrayList;
import java.util.Locale;


/**
 * Created by juksilve on 29.1.2016.
 */
public class DetailsFragment extends Fragment {

    private final String TAG = "DetailsFragment";

    public interface DetailsCallback {
        enum WhichItem{
            current,
            next,
            previous
        };
        JLYServiceItem getRecyclePlace(WhichItem item);
        void startNavigation(final JLYServiceItem destiny);
    }

    final int SWIPE_MIN_DISTANCE = 120;
    final int SWIPE_MAX_OFF_PATH = 250;
    final int SWIPE_THRESHOLD_VELOCITY = 200;

    private View mView;

    private JLYServiceItem mSelectedItem;
    private DetailsCallback mCallback;
    private TextView mDetailsTitle;
    private TextView mDetailsContact;
    private TextView mDetailsAddress;
    private TextView mDetailsOpenTimes;

    private LinearLayout mContactLayout;
    private LinearLayout mAddressLayout;
    private LinearLayout mOpenTimesLayout;

    private Button mNaviButton;

    private ListView mTypesList;
    private ArrayAdapter<String> mTypesAdapter;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        mView = inflater.inflate(R.layout.details_fragment, container, false);
        mDetailsTitle = ((TextView) mView.findViewById(R.id.detailsTitle));
        mDetailsContact = ((TextView) mView.findViewById(R.id.detailsContact));
        mDetailsAddress = ((TextView) mView.findViewById(R.id.detailsAddress));
        mDetailsOpenTimes = ((TextView) mView.findViewById(R.id.detailsOpenTimes));

        mContactLayout = ((LinearLayout) mView.findViewById(R.id.ContactLayout));
        mAddressLayout = ((LinearLayout) mView.findViewById(R.id.AddressLayout));
        mOpenTimesLayout = ((LinearLayout) mView.findViewById(R.id.OpenTimesLayout));

        mNaviButton= ((Button) mView.findViewById(R.id.navigateToButton));
        mNaviButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                //do-refresh for current location
                startNavigation();
            }
        });

        mTypesList = (ListView) mView.findViewById(R.id.detailsTypes);
        ArrayList<String> itemArray = new ArrayList<String>();
        mTypesAdapter = new ArrayAdapter<String>(getActivity(),R.layout.detailtypes_listitem, itemArray);

        ShowItem(mCallback.getRecyclePlace(DetailsCallback.WhichItem.current));

        final GestureDetector gesture = new GestureDetector(getActivity(),
                new GestureDetector.SimpleOnGestureListener() {

                    @Override
                    public boolean onDown(MotionEvent e) {
                        return true;
                    }

                    @Override
                    public boolean onFling(MotionEvent e1, MotionEvent e2, float velocityX,float velocityY) {
                        try {
                            if (Math.abs(e1.getY() - e2.getY()) > SWIPE_MAX_OFF_PATH)
                                return false;
                            if (e1.getX() - e2.getX() > SWIPE_MIN_DISTANCE && Math.abs(velocityX) > SWIPE_THRESHOLD_VELOCITY) {
                                ShowItem(mCallback.getRecyclePlace(DetailsCallback.WhichItem.previous));
                            } else if (e2.getX() - e1.getX() > SWIPE_MIN_DISTANCE && Math.abs(velocityX) > SWIPE_THRESHOLD_VELOCITY) {
                                ShowItem(mCallback.getRecyclePlace(DetailsCallback.WhichItem.next));
                            }
                        } catch (Exception e) {
                            // nothing
                        }
                        return super.onFling(e1, e2, velocityX, velocityY);
                    }
                });

        mView.setOnTouchListener(new View.OnTouchListener() {
            @Override
            public boolean onTouch(View v, MotionEvent event) {
                return gesture.onTouchEvent(event);
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
            mCallback = (DetailsCallback) activity;
        } catch (ClassCastException e) {
            throw new ClassCastException(activity.toString()+ " must implement DetailsCallback");
        }
    }

    private void startNavigation() {

        if (mSelectedItem == null) {
            return;
        }

        mCallback.startNavigation(mSelectedItem);
    }

    private void ShowItem(JLYServiceItem item) {
        if (item == null) {
            return;
        }
        mSelectedItem = item;

        mDetailsTitle.setText(item.getDisplayName());

        mDetailsContact.setText(item.getContact());
        if(mDetailsContact.getText().length() <= 0) {
            mContactLayout.setVisibility(View.GONE);
        }else{
            mContactLayout.setVisibility(View.VISIBLE);
        }

        mDetailsAddress.setText(item.getAddress() + ", " + item.getPostalCode() + " " + item.getCity());
        if(mDetailsAddress.getText().length() <= 0) {
            mAddressLayout.setVisibility(View.GONE);
        }else{
            mAddressLayout.setVisibility(View.VISIBLE);
        }

        if(item.getLocation() == null){
            mNaviButton.setVisibility(View.GONE);
        }else{
            mNaviButton.setVisibility(View.VISIBLE);
        }

        mDetailsOpenTimes.setText(item.getOpenTimes());
        if(mDetailsOpenTimes.getText().length() <= 0) {
            mOpenTimesLayout.setVisibility(View.GONE);
        }else{
            mOpenTimesLayout.setVisibility(View.VISIBLE);
        }
        mTypesAdapter.clear();
        ArrayList<Integer> typesList = item.getMatrialTypes();
        if(typesList != null){
            for(int type: typesList){
                if(JLYConstants.materialTypes.containsKey(type)) {
                    mTypesAdapter.add(JLYConstants.materialTypes.get(type));
                }
            }
        }

        mTypesList.setAdapter(mTypesAdapter);
    }
}
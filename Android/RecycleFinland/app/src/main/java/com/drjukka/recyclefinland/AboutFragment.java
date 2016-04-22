package com.drjukka.recyclefinland;

import android.app.Activity;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.TextView;

import com.google.android.gms.maps.model.LatLng;

/**
 * Created by juksilve on 29.1.2016.
 */
public class AboutFragment extends Fragment {

    private View mView;
    private final String mAboutTitile = "RECYCLE FINLAND";
    private final String mAboutText1 = "Recycling data provided by JLY (www.jly.fi/), mobile application implemented by Dr.Jukka (www.DrJukka.com)";
    private final String mAboutText2 = "All rights reserved.\r\n Version: 1.01 February 4th 2016.";

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,Bundle savedInstanceState) {

        mView=inflater.inflate(R.layout.about_fragment, container,false);
        ((TextView) mView.findViewById(R.id.aboutTitle)).setText(mAboutTitile);
        ((TextView) mView.findViewById(R.id.aboutText1)).setText(mAboutText1);
        ((TextView) mView.findViewById(R.id.aboutText2)).setText(mAboutText2);
        return mView;
    }
}

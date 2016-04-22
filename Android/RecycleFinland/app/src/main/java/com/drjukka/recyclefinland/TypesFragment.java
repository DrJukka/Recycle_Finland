package com.drjukka.recyclefinland;

import android.app.Activity;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.Map;

/**
 * Created by juksilve on 29.1.2016.
 */
public class TypesFragment extends Fragment implements TypesListAdapter.SelectionHighlighter {

    public interface TypesCallback{
        void materialTypeSelected(int type);
    }

    private TypesCallback mCallback;
    private View mView;
    private ArrayList<TypesItem> mListItems = null;
    private TypesListAdapter mAdapter = null;
    private ListView mListView = null;
    private int mSelectedType = 0;
    private int mSelectedIndex = 0;

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {

        mView = inflater.inflate(R.layout.types_fragment, container, false);

        if(mListItems == null){
            mListItems = new ArrayList<>();

            Iterator it = JLYConstants.materialTypes.entrySet().iterator();
            while (it.hasNext()) {
                Map.Entry pair = (Map.Entry)it.next();
                if(mSelectedType == (int)pair.getKey()){
                    mSelectedIndex = mListItems.size();
                }
                mListItems.add(new TypesItem((int)pair.getKey(), (String)pair.getValue()));
            }
        }

        mListView = (ListView) mView.findViewById(R.id.list);
        mAdapter = new TypesListAdapter(getActivity(),mListItems,this);

        mListView.setAdapter(mAdapter);
        if(mSelectedIndex >= 0 && mListItems.size() > mSelectedIndex) {
            mListView.setItemChecked(mSelectedIndex, true);
        }

        mListView.setChoiceMode(ListView.CHOICE_MODE_SINGLE);
        mListView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                // ListView Clicked item value
                mSelectedIndex = position;
                mListView.setItemChecked(position, true);
                TypesItem itemValue = (TypesItem) mListView.getItemAtPosition(position);
                mSelectedType = itemValue.getID();
                mCallback.materialTypeSelected(mSelectedType);
            }
        });

        return mView;
    }

    public void  setSelectedType(int type){
        mSelectedType = type;
    }

    public int getSelectedType(){
        return mSelectedType;
    }

    @Override
    public void onAttach(Activity activity) {
        super.onAttach(activity);

        // This makes sure that the container activity has implemented
        // the callback interface. If not, it throws an exception
        try {
            mCallback = (TypesCallback) activity;
        } catch (ClassCastException e) {
            throw new ClassCastException(activity.toString()
                    + " must implement SearchCallback");
        }
    }

    @Override
    public boolean isSelected(int index) {
        return (mSelectedIndex == index);
    }
}
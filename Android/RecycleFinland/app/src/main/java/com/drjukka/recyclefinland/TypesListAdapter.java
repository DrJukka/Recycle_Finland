package com.drjukka.recyclefinland;

import android.app.Activity;
import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

import java.util.ArrayList;

/**
 * Created by juksilve on 29.1.2016.
 */
public class TypesListAdapter extends ArrayAdapter<TypesItem> {

    public interface SelectionHighlighter{
        boolean isSelected(int index);
    }
    private final Activity mContext;
    private final ArrayList<TypesItem> mArray;
    private final SelectionHighlighter mCallback;

    public TypesListAdapter(Activity context, ArrayList<TypesItem> array,SelectionHighlighter callback) {
        super(context, R.layout.typeslistitem_layout, array);
        this.mContext = context;
        this.mArray  = array;
        this.mCallback = callback;
    }

    public View getView(int position,View view,ViewGroup parent) {
        LayoutInflater inflater=mContext.getLayoutInflater();
        View rowView=inflater.inflate(R.layout.typeslistitem_layout, null,true);

        TextView secondLine = (TextView) rowView.findViewById(R.id.firstLine);
        secondLine.setText(mArray.get(position).getName());
        if(this.mCallback.isSelected(position)) {
            secondLine.setBackgroundColor(mContext.getResources().getColor(R.color.selectedTypeBackground));
        }
        return rowView;
    };
}
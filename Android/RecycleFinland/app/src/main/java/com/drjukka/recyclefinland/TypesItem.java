package com.drjukka.recyclefinland;

/**
 * Created by juksilve on 29.1.2016.
 */
public class TypesItem {
    private final int mId;
    private final String mName;

    public TypesItem(int id, String name) {
        this.mId = id;
        this.mName = name;
    }

    public int getID(){return mId;}
    public String getName(){return mName;}
}

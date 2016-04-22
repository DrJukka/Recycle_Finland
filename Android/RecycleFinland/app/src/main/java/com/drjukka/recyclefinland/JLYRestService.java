package com.drjukka.recyclefinland;

import android.os.AsyncTask;
import android.os.Build;
import android.support.annotation.NonNull;
import android.util.Log;
import android.util.Xml;

import com.google.android.gms.maps.model.LatLng;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;
import org.xmlpull.v1.XmlPullParserFactory;

import java.io.BufferedInputStream;
import java.io.BufferedReader;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.Locale;
import java.util.Map;
import java.util.Set;

import javax.net.ssl.HttpsURLConnection;

/**
 * Created by juksilve on 29.1.2016.
 */
public class JLYRestService {

    private final String TAG = "JLYRestService";
    private final String mServiceUrl = "http://kierratys.info/2.0/genxml.php?lat=%f&lng=%f";

    public interface JLYRestServiceCallback {
        void searchFinished(final ArrayList<JLYServiceItem> array);
    }

    public void findNearby(double latitude, double longitude, int type, int radius, int limit,final  JLYRestServiceCallback callback) {

        String UrlToUse = String.format(Locale.US, mServiceUrl, latitude, longitude);

        if (limit != 0) {
            UrlToUse = UrlToUse + "&limit=" + limit;
        }

        if (radius != 0) {
            UrlToUse = UrlToUse + "&radius=" + radius;
        }

        if (type != 0) {
            UrlToUse = UrlToUse + "&type_id=" + type;
        }

        Log.d(TAG,"UrlToUse : " + UrlToUse);

        fetchXML(UrlToUse,callback);
    }

    private void fetchXML(final String url,final JLYRestServiceCallback callback){
        Thread thread = new Thread(new Runnable(){
            @Override
            public void run() {
                try {
                    URL connectionUrl = new URL(url);
                    HttpURLConnection conn = (HttpURLConnection)connectionUrl.openConnection();

                    conn.setReadTimeout(25000 /* milliseconds */);
                    conn.setConnectTimeout(35000 /* milliseconds */);
                    conn.setRequestMethod("GET");
                    conn.setDoInput(true);
                    conn.connect();

                    InputStream stream = conn.getInputStream();

                    XmlPullParser myparser = Xml.newPullParser();
                    myparser.setFeature(XmlPullParser.FEATURE_PROCESS_NAMESPACES, false);
                    myparser.setInput(stream, null);

                    ArrayList<JLYServiceItem> array = parseMarker(myparser);
                    stream.close();

                    callback.searchFinished(array);
                }
                catch (Exception e) {
                    e.printStackTrace();
                    callback.searchFinished(null);
                }
            }
        });
        thread.start();
    }

    // for debugging, allows printing the whole received XML
    private String readInputStreamToString(HttpURLConnection connection) {
        String result = null;
        StringBuffer sb = new StringBuffer();
        InputStream is = null;

        try {
            is = new BufferedInputStream(connection.getInputStream());
            BufferedReader br = new BufferedReader(new InputStreamReader(is));
            String inputLine = "";
            while ((inputLine = br.readLine()) != null) {
                sb.append(inputLine);
            }
            result = sb.toString();
        }
        catch (Exception e) {
            Log.i(TAG, "Error reading InputStream");
            result = null;
        }
        finally {
            if (is != null) {
                try {
                    is.close();
                }
                catch (IOException e) {
                    Log.i(TAG, "Error closing InputStream");
                }
            }
        }

        return result;
    }

    private ArrayList<JLYServiceItem> parseMarker(XmlPullParser parser)
    {
        ArrayList<JLYServiceItem> items = new ArrayList<JLYServiceItem>();
        Map<String, String> tmp = new HashMap<>();
        try {
            int eventType;

             Log.d(TAG, "**************** Start ****************");

             while((eventType = parser.getEventType())!=XmlPullParser.END_DOCUMENT){
                if(eventType==XmlPullParser.START_TAG) {
                    if("marker".equals(parser.getName())) {
                        tmp.clear();

                        for(int i= 0; i < parser.getAttributeCount(); i++) {
                            tmp.put(parser.getAttributeName(i),parser.getAttributeValue(i));
                        }

                        addMarker(tmp, items);
                    }
                }
                parser.next();
            }
             Log.d(TAG, "**************** Done ****************");
        }
        catch(XmlPullParserException ex) {
            Log.d(TAG, "parser XmlPullParserException : " + ex.toString());
        }catch(IOException ex) {
            Log.d(TAG, "parser IOException : " + ex.toString());
        }

        return items;
    }
    private void addMarker(Map<String, String> data, ArrayList<JLYServiceItem> list)
    {
        String locationId;

        if (data.containsKey(JLYConstants.paikka_id))
        {
            locationId = data.get(JLYConstants.paikka_id);
            for (JLYServiceItem item : list)
            {
                if (item.getLocationId() == locationId)
                {
                    item.updateItem(data);
                    return;
                }
            }

            JLYServiceItem item = JLYServiceItem.createServiceItem(data);
            Log.d(TAG,"Found place  : " + item.getDisplayName());
            list.add(item);
        }
    }
}

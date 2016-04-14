using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RecycleFinland.Engine
{
    public delegate void JLYResults(List<JLYServiceItem> items, Exception error);

    public class JLYRestService
    {
        public JLYResults JLYResults;

        private WebRequest _request;
        private static JLYRestService _instance = new JLYRestService();

        public static JLYRestService Instance
        {
            get { return _instance; }
        }

        public void FindNearby(double latitude, double longitude, int type, int radius, int limit)
        {
            Cancel();

            string latStr = latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lonStr = longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

            string UrlToUse = "http://kierratys.info/2.0/genxml.php?lat=" + latStr + "&lng=" + lonStr;

            
            if (limit != 0)
            {
                UrlToUse = UrlToUse + "&limit=" + limit;
            }

            if (radius != 0)
            {
                UrlToUse = UrlToUse + "&radius=" + radius;
            }

            if (type != 0)
            {
                UrlToUse = UrlToUse + "&type_id=" + type;
            }

           // System.Diagnostics.Debug.WriteLine("Sending request " + UrlToUse);
            SendRequestAsync(UrlToUse);
        }

        public void Cancel()
        {
            WebRequest tmpReg = _request;
            _request = null;
            if (tmpReg != null)
            {
                tmpReg.Abort();
            }
        }

        private void SendRequestAsync(String Url)
        {
            if (JLYResults == null)
            {
                System.Diagnostics.Debug.WriteLine("No callback defined !!");
                return;
            }

            _request = WebRequest.Create(Url);
            _request.BeginGetResponse(
                (IAsyncResult ar) =>
                {
                    WebRequest tmpReg = _request;
                    if (tmpReg == null)
                    {
                        return;
                    }

                    List<JLYServiceItem> items = null;
                    WebResponse response = null;
                    HttpWebResponse webResponse = null;
                    HttpStatusCode? statusCode = null;
                    Exception error = null;

                    try
                    {
                        response = tmpReg.EndGetResponse(ar);
                        webResponse = response as HttpWebResponse;
                        if (webResponse != null)
                        {
                            statusCode = webResponse.StatusCode;
                        }
                    }
                    catch (WebException ex)
                    {
                        error = ex;
                        if (ex.Response != null)
                        {
                            webResponse = (HttpWebResponse)ex.Response;
                            statusCode = webResponse.StatusCode;
                        }
                    }

                    string result = null;
                    if (response != null)
                    {

                        using (Stream responseStream = response.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(responseStream))
                            {
                                result = reader.ReadToEnd();
                                if (!string.IsNullOrEmpty(result))
                                {
                                    try
                                    {
                                      //  System.Diagnostics.Debug.WriteLine("got result " + result);
                                        items = parseMarker(result);
                                    }
                                    catch (Exception ex)
                                    {
                                        error = ex;
                                    }
                                }
                            }
                        }
                    }

                    _request = null;
                    JLYResults(items, error);
                },
                _request);
        }

        private List<JLYServiceItem> parseMarker(string xmlString)
        {
            List<JLYServiceItem> items = new List<JLYServiceItem>();

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
            {
                Dictionary<string, string> Tmp = new Dictionary<string, string>();

            //    System.Diagnostics.Debug.WriteLine("**************** Start ****************");

                while (reader.ReadToFollowing("marker"))
                {
                    Tmp.Clear();
                    if (reader.MoveToFirstAttribute())
                    {
                        Tmp.Add(reader.Name, reader.Value);
                        while (reader.MoveToNextAttribute())
                        {
                            Tmp.Add(reader.Name, reader.Value);
                        }
                    }
                    AddMarker(Tmp, items);
                }

              //  System.Diagnostics.Debug.WriteLine("**************** Done ****************");
            }

            return items;
        }
        private void AddMarker(Dictionary<string, string> data, List<JLYServiceItem> list)
        {
            String locationId;
            if (data.TryGetValue(JLYConstants.paikka_id, out locationId))
            {
                foreach (JLYServiceItem item in list)
                {
                    if (item.LocationId == locationId)
                    {
                        item.UpdateItem(data);
                        return;
                    }
                }

                list.Add(JLYServiceItem.CreateServiceItem(data));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace RecycleFinland.Engine
{
    public class JLYServiceItem
    {
        private string _locationId;
        private List<int> _materialTypes = new List<int>();
        private string _name;
        private string _address;
        private string _postalCode;
        private string _city;
        private Geopoint _geoPoint;
        private string _distance;
        private string _operator;
        private int _manned;
        private string _openTimes;
        private string _contact;
        private Point _anchorPoint;
        private Uri _iconUri;
        public string DisplayName { get { return _name; } }
        public Geopoint Location { get { return _geoPoint; } }
        public Point NormalizedAnchorPoint { get { return _anchorPoint; } }
        public Uri ImageSourceUri { get { return _iconUri; } }
        public string LocationId { get { return _locationId; } }
        public List<int> MatrialTypes { get { return _materialTypes; } }
        
        public string Address { get { return _address; } }
        public string PostalCode { get { return _postalCode; } }
        public string City { get { return _city; } }
        
        public string Distance { get { return _distance; } }
        public string Operator { get { return _operator; } }
        public int Manned { get { return _manned; } }
        public string OpenTimes { get { return _openTimes; } }
        public string Contact { get { return _contact; } }

        private JLYServiceItem(String locationId)
        {
            _locationId = locationId;
            _anchorPoint = new Point(0.5, 1.0);
            _iconUri = new Uri("ms-appx:///Assets/mappin.png");
        }

        public void UpdateItem(Dictionary<string, string> data)
        {
            String laji_idString;
            if (data.TryGetValue(JLYConstants.laji_id, out laji_idString))
            {
                int laji_idInt;
                if (Int32.TryParse(laji_idString, out laji_idInt))
                {
                    _materialTypes.Add(laji_idInt);
                }
            }

            data.TryGetValue(JLYConstants.nimi, out _name);
            data.TryGetValue(JLYConstants.osoite, out _address);
            data.TryGetValue(JLYConstants.pnro, out _postalCode);
            data.TryGetValue(JLYConstants.paikkakunta, out _city);

            String latString;
            String lngString;
            if (data.TryGetValue(JLYConstants.lat, out latString)
             && data.TryGetValue(JLYConstants.lng, out lngString))
            {
                Double latDouble;
                Double lngDouble;
                if (Double.TryParse(latString, out latDouble)
                && Double.TryParse(lngString, out lngDouble))
                {
                    _geoPoint = new Geopoint(new BasicGeoposition
                    {
                        Latitude = latDouble,
                        Longitude = lngDouble
                    });
                }
            }

            data.TryGetValue(JLYConstants.etaisyys, out _distance);
            data.TryGetValue(JLYConstants.yllapitaja, out _operator);

            String miehitysString;
            if (data.TryGetValue(JLYConstants.miehitys, out miehitysString))
            {
                Int32.TryParse(miehitysString, out _manned);
            }

            data.TryGetValue(JLYConstants.aukiolo, out _openTimes);
            data.TryGetValue(JLYConstants.yhteys, out _contact);

            /*public const string lajitarkennus = "lajitarkennus";
            public const string rateplus = "rateplus";
            public const string rateminus = "rateminus";
            */
        }

        static public JLYServiceItem CreateServiceItem(Dictionary<string, string> data)
        {
            JLYServiceItem tmpItem = new JLYServiceItem(data[JLYConstants.paikka_id]);
            tmpItem.UpdateItem(data);
            return tmpItem;
        }
    }
}


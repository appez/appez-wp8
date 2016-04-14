using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Windows.Media;
using System.Device.Location;
using System.Collections;
using appez.model;
using appez.constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Phone.Maps.Controls;
using System.ComponentModel;
using System.Windows.Shapes;
using Windows.Devices.Geolocation;
using System.Windows.Input;
using System.Threading.Tasks;
using appez.listeners;
using appez.model.map;
using appez.exceptions;

namespace appez.utility.uicontrols.map
{
    public partial class SmartMapView : PhoneApplicationPage
    {
        #region variables
        MapLayer markerLayer = null;
        List<PunchLocation> punchLocation = null;
        Geolocator myGeolocator = null;
        GeoCoordinate currentLocation = null;
        internal SmartCoActionListener smartCoActionListener;
        bool isShowDirection = false;
        SolidColorBrush[] PushPinColor =
        {
            new SolidColorBrush(Colors.Blue),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Red),
            new SolidColorBrush(Colors.Yellow)
        };
        #endregion

        public SmartMapView()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Handles page navigation event, parse information comes in query string.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                // Retrieve parameter from query string.
                IDictionary<string, string> parameters = this.NavigationContext.QueryString;
                isShowDirection = Convert.ToBoolean(parameters["showDirection"]);
                string mapData = parameters["mapData"];
                Dictionary<String, Object> json = JsonConvert.DeserializeObject<Dictionary<string, Object>>(mapData);

                // Convert location information in json to punchLocation list.
                if (json.ContainsKey(CommMessageConstants.MMI_REQUEST_PROP_LOCATIONS))
                    this.punchLocation = ParseLocationInformation((JArray)json[CommMessageConstants.MMI_REQUEST_PROP_LOCATIONS]);

                AttachHardwareButtonHandlers();
            }
            catch (NullReferenceException)
            {
                throw new MobiletException(ExceptionTypes.MALFORMED_URL_EXCEPTION);
            }
        }

        /// <summary>
        /// Handles SmartMap loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SmartMap_Loaded(object sender, RoutedEventArgs e)
        {
            SessionData.GetInstance().SetIsMapBusy(false);
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "ApplicationID";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "AuthenticationToken";
            // Set current location on map.
            await this.SetCurrentLocation();
            // Add punch location on map.
            this.AddPunchLocationItem();
            // recenter map according to punch location and current location.
            this.RecenterMap();
            UIUtility uiUtility = new UIUtility();
            uiUtility.HideIndicator();

        }

        /// <summary>
        /// Sets current position on map.
        /// </summary>
        public async Task SetCurrentLocation()
        {
            try
            {
                // Get current location.
                myGeolocator = new Geolocator();
                Geoposition myGeoposition = null;

                myGeoposition = await myGeolocator.GetGeopositionAsync();

                Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
                currentLocation = this.ConvertGeocoordinate(myGeocoordinate);
                // set center of map to current location.
                mapView.Center = currentLocation;
                Grid currentLocationMark = CreateCurrentLocationMark();
                MapOverlay overlay = new MapOverlay
                {
                    GeoCoordinate = mapView.Center,
                    Content = currentLocationMark
                };
                MapLayer layer = new MapLayer();
                layer.Add(overlay);

                mapView.Layers.Add(layer);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Location is disabled in phone settings.");
            }

        }

        /// <summary>
        /// Creates current location mark.
        /// </summary>
        /// <returns>Current location mark</returns>
        private Grid CreateCurrentLocationMark()
        {
            Polygon polygon = new Polygon();
            polygon.Fill = new SolidColorBrush(Colors.Black);
            polygon.Width = 34;
            polygon.Height = 34;
            polygon.Points.Add(new Point(0, 17));
            polygon.Points.Add(new Point(17, 0));
            polygon.Points.Add(new Point(34, 17));
            polygon.Points.Add(new Point(17, 34));

            Ellipse ellipse = new Ellipse();
            ellipse.Fill = new SolidColorBrush(Colors.Red);
            ellipse.Width = 14;
            ellipse.Height = 14;

            Grid grid = new Grid();
            grid.Children.Add(polygon);
            grid.Children.Add(ellipse);

            return grid;

        }

        /// <summary>
        /// Sets map view according to punch locations available.
        /// </summary>
        public void RecenterMap()
        {
            if (punchLocation != null && punchLocation.Count > 0)
            {
                float maxLatitude = punchLocation.Max<PunchLocation>(PunchLocation => PunchLocation.Latitude);
                float maxLongitude = punchLocation.Max<PunchLocation>(PunchLocation => PunchLocation.Longitude);
                float minLatitude = punchLocation.Min<PunchLocation>(PunchLocation => PunchLocation.Latitude);
                float minLongitude = punchLocation.Min<PunchLocation>(PunchLocation => PunchLocation.Longitude);
                GeoCoordinate[] boundingGeoCoordinate = { new GeoCoordinate(maxLatitude, maxLongitude), new GeoCoordinate(minLatitude, minLongitude), this.currentLocation };

                mapView.SetView(LocationRectangle.CreateBoundingRectangle(boundingGeoCoordinate), MapAnimationKind.Parabolic);
            }

        }

        /// <summary>
        /// Converts Geocoordinate to GeoCoordinate.
        /// </summary>
        /// <param name="geocoordinate">Geocoordinate</param>
        /// <returns>GeoCoordinate</returns>
        public GeoCoordinate ConvertGeocoordinate(Geocoordinate geocoordinate)
        {
            return new GeoCoordinate
                (
                geocoordinate.Latitude,
                geocoordinate.Longitude,
                geocoordinate.Altitude ?? Double.NaN,
                geocoordinate.Accuracy,
                geocoordinate.AltitudeAccuracy ?? Double.NaN,
                geocoordinate.Speed ?? Double.NaN,
                geocoordinate.Heading ?? Double.NaN
                );
        }

        /// <summary>
        /// Adds punch location to map view.
        /// </summary>
        public void AddPunchLocationItem()
        {
            if (markerLayer == null)
            {
                markerLayer = new MapLayer();
                if (punchLocation != null && punchLocation.Count > 0)
                {
                    foreach (PunchLocation location in punchLocation)
                    {
                        StackPanel punchLocationMark = CreatePunchLocationMark(location);
                        MapOverlay pin = new MapOverlay();
                        pin.Content = punchLocationMark;
                        pin.GeoCoordinate = new GeoCoordinate(location.Latitude, location.Longitude);

                        markerLayer.Add(pin);
                    }
                }
                mapView.Layers.Add(markerLayer);
            }
        }

        /// <summary>
        /// Creates punch location mark.
        /// </summary>
        /// <param name="location">PunchLocation</param>
        /// <returns>Punch location marker</returns>
        private StackPanel CreatePunchLocationMark(PunchLocation location)
        {

            StackPanel stackpanel = new StackPanel();
            if (location != null)
            {
                stackpanel.Orientation = System.Windows.Controls.Orientation.Vertical;

                Grid grid = new Grid();
                grid.Background = new SolidColorBrush(GetColorFromHexString(location.LocationMarker));
                grid.MinHeight = 31;
                grid.MinWidth = 29;
                ContentPresenter contentpresenter = new ContentPresenter();
                contentpresenter.Margin = new Thickness(4);
                contentpresenter.Content = location.LocationTitle + "\n" + location.LocationDescription;

                grid.Children.Add(contentpresenter);


                Polygon polygon = new Polygon();
                polygon.Points.Add(new Point(0, 0));
                polygon.Points.Add(new Point(0, 29));
                polygon.Points.Add(new Point(29, 0));
                polygon.Fill = new SolidColorBrush(GetColorFromHexString(location.LocationMarker));

                stackpanel.Children.Add(grid);
                stackpanel.Children.Add(polygon);
                stackpanel.Tag = location.Latitude + "," + location.Longitude;
                if (isShowDirection)
                {
                    stackpanel.MouseLeftButtonUp += new MouseButtonEventHandler(Marker_Click);
                }
            }
            return stackpanel;

        }

        /// <summary>
        /// Event handler for clicking a map marker. 
        /// Initiates reverse geocode query.
        /// </summary>
        private void Marker_Click(object sender, EventArgs e)
        {
            string location = (sender as StackPanel).Tag.ToString();

            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(string.Format("/{0};component/utility/uicontrols/map/SmartMapListRoute.xaml?location={1}&currentLocation={2}", AppUtility.GetAssemblyName(), location, currentLocation), UriKind.Relative));

        }

        /// <summary>
        /// Parse Json punch information to list of punch information.
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns></returns>
        public List<PunchInformation> ParsePunchInformation(JArray mapData)
        {
            List<PunchInformation> punchInfo = new List<PunchInformation>();
            if (mapData != null && mapData.Count > 0)
            {
                foreach (dynamic item in mapData)
                {
                    PunchInformation location = new PunchInformation();
                    location.Title = (string)item[CommMessageConstants.MMI_REQUEST_PROP_LOC_TITLE];
                    location.PunchType = Convert.ToInt16(item[CommMessageConstants.MMI_REQUEST_PROP_LOC_MARKER]);
                    punchInfo.Add(location);
                }
            }
            return punchInfo;
        }

        /// <summary>
        /// parse Json punch location information to list of punch location.
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns></returns>
        public List<PunchLocation> ParseLocationInformation(JArray mapData)
        {

            List<PunchLocation> locationInfo = new List<PunchLocation>();
            if (mapData != null && mapData.Count > 0)
            {
                foreach (dynamic item in mapData)
                {
                    PunchLocation location = new PunchLocation();
                    location.LocationTitle = (string)item[CommMessageConstants.MMI_REQUEST_PROP_LOC_TITLE];
                    location.LocationDescription = (string)item[CommMessageConstants.MMI_REQUEST_PROP_LOC_DESCRIPTION];
                    location.Latitude = float.Parse((string)item[CommMessageConstants.MMI_REQUEST_PROP_LOC_LATITUDE]);
                    location.Longitude = float.Parse((string)item[CommMessageConstants.MMI_REQUEST_PROP_LOC_LONGITUDE]);
                    location.LocationMarker = (string)item[CommMessageConstants.MMI_REQUEST_PROP_LOC_MARKER];
                    locationInfo.Add(location);
                }
            }
            return locationInfo;
        }
        /// <summary>
        /// Convert hexcode string into Color.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected static Color GetColorFromHexString(string s)
        {
            // remove artifacts
            s = s.Trim().TrimStart('#');

            // only 8 (with alpha channel) or 6 symbols are allowed
            if (s.Length != 8 && s.Length != 6)
                throw new ArgumentException("Unknown string format!");

            int startParseIndex = 0;
            bool alphaChannelExists = s.Length == 8; // check if alpha canal exists            

            // read alpha channel value
            byte a = 255;
            if (alphaChannelExists)
            {
                a = System.Convert.ToByte(s.Substring(0, 2), 16);
                startParseIndex += 2;
            }

            // read r value
            byte r = System.Convert.ToByte(s.Substring(startParseIndex, 2), 16);
            startParseIndex += 2;
            // read g value
            byte g = System.Convert.ToByte(s.Substring(startParseIndex, 2), 16);
            startParseIndex += 2;
            // read b value
            byte b = System.Convert.ToByte(s.Substring(startParseIndex, 2), 16);

            return Color.FromArgb(a, r, g, b);
        }
        /// <summary>
        /// Adds hardware back key press event to current page.
        /// </summary>
        void AttachHardwareButtonHandlers()
        {
            PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (frame != null)
            {
                PhoneApplicationPage page = frame.Content as PhoneApplicationPage;

                if (page != null)
                {
                    page.BackKeyPress += new EventHandler<CancelEventArgs>(Page_BackKeyPress);

                }
            }
        }
        /// <summary>
        /// Handle hardware back key press event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Page_BackKeyPress(object sender, CancelEventArgs e)
        {

            try
            {
                smartCoActionListener.OnSuccessCoAction(null);
                this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while invoking backbutton into view: " + ex.Message);
            }


        }

    }
}


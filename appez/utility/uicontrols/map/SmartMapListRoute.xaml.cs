using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Device.Location;
using Microsoft.Phone.Maps.Services;
using Windows.Devices.Geolocation;
using System.ComponentModel;
using Microsoft.Phone.Maps.Controls;
using appez.exceptions;
using System.Reflection;
using System.Threading;

namespace appez.utility.uicontrols.map
{
    public partial class SmartMapListRoute : PhoneApplicationPage
    {
        #region variables
        RouteQuery routeQuery = null;
        GeoCoordinate currentLocation = null;
        GeoCoordinate targetLocation = null;
        List<GeoCoordinate> routeCoordinates = new List<GeoCoordinate>();
        #endregion
        UIUtility uiUtility = null;
        /// <summary>
        /// Initialize SmartMapListRoute.
        /// </summary>
        public SmartMapListRoute()
        {
            InitializeComponent();
            uiUtility = new UIUtility();
            uiUtility.ShowProgressBarWithMessage("Loading...");
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
                IDictionary<string, string> parameters = this.NavigationContext.QueryString;

                targetLocation = new GeoCoordinate(Convert.ToDouble(parameters["location"].Split(',')[0]), Convert.ToDouble(parameters["location"].Split(',')[1]));
                currentLocation = new GeoCoordinate(Convert.ToDouble(parameters["currentLocation"].Split(',')[0]), Convert.ToDouble(parameters["currentLocation"].Split(',')[1]));
                routeCoordinates.Add(currentLocation);
                routeCoordinates.Add(targetLocation);

                AttachHardwareButtonHandlers();

                GetRouteDirection();

            }
            catch (NullReferenceException ex)
            {
                System.Diagnostics.Debug.WriteLine("Error occurred when parsing query string of SmartMapListRoute :" + ex.Message.ToString());
                throw new MobiletException(ExceptionTypes.UNKNOWN_CURRENT_LOCATION_EXCEPTION);
            }
        }

        /// <summary>
        /// Initiate route query and invoke QueryAsync.
        /// </summary>
        public void GetRouteDirection()
        {
            routeQuery = new RouteQuery();
            routeQuery.Waypoints = routeCoordinates;
            routeQuery.QueryCompleted += RouteQuery_QueryCompleted;
            routeQuery.QueryAsync();
            
        }

        /// <summary>
        /// Handles route query completed event and show route information to LongListSelector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RouteQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            List<string> routeList = new List<string>();
            if (!e.Cancelled)
            {
                
                try
                {
                    Route route = e.Result;
                
                    MapRoute mapRoute = new MapRoute(route);
                    // Iterate through all route and set them to RouteLLS.
                    foreach (RouteLeg leg in route.Legs)
                    {
                        foreach (RouteManeuver maneuver in leg.Maneuvers)
                        {
                            routeList.Add(maneuver.InstructionText);
                        }
                    }

                    RouteLLS.ItemsSource = routeList;
                }
                catch (TargetInvocationException)
                {
                    Thread.Sleep(1000); // waiting for  completing the query
                    RouteQuery_QueryCompleted(sender, e);
                }

            }
            else
            {
                // If no route found.
                routeList.Add("No route found.");
                RouteLLS.ItemsSource = routeList;
            }
            routeQuery.Dispose();
            uiUtility.HideIndicator();
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
                this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while invoking backbutton into view: " + ex.Message);
            }


        }
    }
}
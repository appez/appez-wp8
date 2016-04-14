using appez.constants;
using appez.exceptions;
using appez.listeners;
using appez.model;
using appez.utility;
using appez.utility.uicontrols.map;
using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Windows;

namespace appez.services
{
    /// <summary>
    /// Allows the web layer to show maps in the appez powered
    /// application.
    /// </summary>
    public class MapService : SmartService, SmartCoActionListener
    {
        #region variables
        private SmartEvent smartEvent = null;
        private SmartServiceListener smartServiceListener = null;
        #endregion
        /// <summary>
        /// Creates the instance of MapService
        /// </summary>
        /// <param name="listener">SmartServiceListener</param>
        public MapService(SmartServiceListener listener)
        {
            this.smartServiceListener = listener;
        }

        public override void ShutDown()
        {
            smartServiceListener = null;
        }

        public override void PerformAction(SmartEvent smartEvent)
        {
            this.smartEvent = smartEvent;

            try
            {
                switch (smartEvent.GetServiceOperationId())
                {
                    case CoEvents.CO_SHOW_MAP_ONLY:
                        (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(string.Format("/{0};component/utility/uicontrols/map/SmartMapView.xaml?showDirection={1}&mapData={2}", AppUtility.GetAssemblyName(), false, smartEvent.SmartEventRequest.ServiceRequestData.ToString().Replace("#","%23")), UriKind.Relative));
                        break;

                    case CoEvents.CO_SHOW_MAP_N_DIR:
                        (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(string.Format("/{0};component/utility/uicontrols/map/SmartMapView.xaml?showDirection={1}&mapData={2}", AppUtility.GetAssemblyName(), true, smartEvent.SmartEventRequest.ServiceRequestData.ToString().Replace("#", "%23")), UriKind.Relative));
                        break;
                }
                // TODO: need to handle map with direction.
                //Navigate to map screen.
                
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigated += MapService_Navigated;
            }
            catch (Exception ex)
            {
                OnErrorCoAction(ExceptionTypes.UNKNOWN_EXCEPTION, ex.Message.ToString());
            }
        }

        void MapService_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (e.Content is SmartMapView)
            {
                (e.Content as SmartMapView).smartCoActionListener = this;
            }
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigated -= MapService_Navigated;

        }


        /// <summary>
        /// Sends the map show completion notification to the web layer. This event
        /// is triggered when the container has shown the map and user goes back from
        /// the native map screen.
        /// </summary>
        /// <param name="mapsData"></param>
        public void OnSuccessCoAction(String mapsData)
        {
            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete=true;
            smEventResponse.ServiceResponse=mapsData;
            smEventResponse.ExceptionType=0;
            smEventResponse.ExceptionMessage=null;
            smartEvent.SmartEventResponse = smEventResponse;
            smartServiceListener.OnCompleteServiceWithSuccess(smartEvent);

        }

        /// <summary>
        /// Sends the map show error notification to the web layer.
        /// </summary>
        /// <param name="exceptionType">Unique ID corresponding to the error in showing map</param>
        /// <param name="exceptionMessage">Message describing the nature of problem with showing map</param>
        public void OnErrorCoAction(int exceptionType, String exceptionMessage)
        {
            SmartEventResponse smEventResponse = new SmartEventResponse();
            smEventResponse.IsOperationComplete=false;
            smEventResponse.ServiceResponse=null;
            smEventResponse.ExceptionType=exceptionType;
            smEventResponse.ExceptionMessage=exceptionMessage;
            smartEvent.SmartEventResponse=smEventResponse;
            smartServiceListener.OnCompleteServiceWithError(smartEvent);
        }

    }
}

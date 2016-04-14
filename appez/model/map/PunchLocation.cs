using System;

namespace appez.model.map
{
    /// <summary>
    /// Model contain details about punch location.
    /// </summary>
    public class PunchLocation
    {
        /// <summary>
        /// Title of the location to be marked. This gets shown in the info-bubble
        /// marker of the location.
        /// </summary>
        public string LocationTitle { get; set; }
        /// <summary>
        /// Description of the location to be marked. This gets shown in the
        /// info-bubble marker of the location.
        /// </summary>
        public string LocationDescription { get; set; }
        /// <summary>
        /// Latitude of the location.
        /// </summary>
        public float Latitude { get; set; }
        /// <summary>
        /// Longitude of the location.
        /// </summary>
        public float Longitude { get; set; }
        /// <summary>
        /// Punch type of location.
        /// </summary>
        public string LocationMarker { get; set; }

    }
}

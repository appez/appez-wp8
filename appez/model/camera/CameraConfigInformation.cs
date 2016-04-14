
using Newtonsoft.Json;
namespace appez.model.camera
{
    /// <summary>
    /// Model bean defining parameters for the Camera
    /// service. Its properties are user configurable via the web layer API for
    /// Camera Service.
    /// </summary>
    public class CameraConfigInformation
    {
        // properties names are kept as is as in json for converting to json object.

        /// <summary>
        /// Defines the source of image. Accpetable sources are device camera and
        // image gallery
        /// </summary>
        [JsonIgnore]
        public int source { set; get; }
        /// <summary>
        /// the compression level that needs to be achieve, the higher the
        /// value of this parameter should be.
        /// </summary>
        public int imageCompressionLevel { set; get; }
        /// <summary>
        /// Indicates the format in which the image data should be returned to the
        /// web layer. Based on user selection, it can be either a saved image URL or
        /// Base64 encoded image data
        /// </summary>
        public string imageReturnType { set; get; }
        /// <summary>
        /// Defines the image encoding format. Currently supported formats are JPEG
        /// and PNG
        /// </summary>
        public string imageEncoding { set; get; }
        /// <summary>
        /// Defines the filter effect to be applied on the image. If no effect needs
        /// to be applied, then this parameter needs to be kept STANDARD. Effects
        /// that can be applied include SEPIA and MONOCHROME
        /// </summary>
        public string imageFilter { set; get; }
        /// <summary>
        /// User defined property indicating the camera direction that user wants to
        /// capture image from
        /// </summary>
        public string cameraDirection { set; get; }

    }
}


namespace appez.model.fileupload
{
    /// <summary>
    /// Model bean that indicates the property of file. This is used
    /// in scenarios involving the image upload.
    /// </summary>
    class FileInfoBean
    {
        /// <summary>
        /// Indicates the name of the file
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Indicates the name of the file
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// Indicates the Base64 content of the file
        /// </summary>
        public string FileData { get; set; }
    }
}

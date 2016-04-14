using appez.constants;
using appez.exceptions;
using appez.model;
using appez.model.camera;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace appez.utility
{
    /// <summary>
    /// Utility class to process image.
    /// Conversion to B/W, sepia, image compression is currently supported.
    /// </summary>
    public class ImageUtility
    {
        #region variables
        CameraConfigInformation cameraConfiginformation;
        byte[] imageData;
        static WriteableBitmap wbSource;
        int[] srcPxls;
        #endregion

        /// <summary>
        /// Apply filter(Sepia, Monochrome) to image and reduce image quality according to camera config information.
        /// </summary>
        /// <param name="imageStream">Image stream to be processed</param>
        /// <param name="configInfo">Camera config information</param>
        /// <returns>Processed image in byte[]</returns>
        public byte[] ProcessImage(Stream imageStream, CameraConfigInformation configInfo)
        {
            try
            {
                this.cameraConfiginformation = configInfo;
                if (wbSource == null)
                {
                    // Create source bitmap.
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(imageStream);
                    wbSource = new WriteableBitmap(bitmap);
                    bitmap.UriSource = null;
                    bitmap = null;


                }
                else
                {
                    wbSource.SetSource(imageStream);
                }
                GC.Collect();

                srcPxls = wbSource.Pixels;

                switch (cameraConfiginformation.imageFilter)
                {
                    case SmartConstants.IMAGE_FORMAT_SEPIA:
                        for (int i = 0; i < srcPxls.Length; i++)
                        {
                            srcPxls[i] = ColorToSepia(srcPxls[i]);
                        }
                        break;
                    case SmartConstants.IMAGE_FORMAT_MONOCHROME:
                        for (int i = 0; i < srcPxls.Length; i++)
                        {
                            srcPxls[i] = ColorToGray(srcPxls[i]);
                        }
                        break;
                    default:
                        goto reduceQuality;
                        
                }
                // Copy pixel arrray to writeable bitmap.
                srcPxls.CopyTo(wbSource.Pixels, 0);
                srcPxls = null;
                wbSource.Invalidate();

            reduceQuality:
                using (MemoryStream targetStream = new MemoryStream())
                {

                    // reduce quality of image.
                    wbSource.SaveJpeg(targetStream, wbSource.PixelWidth, wbSource.PixelHeight, 0, cameraConfiginformation.imageCompressionLevel);
                    imageData = targetStream.ToArray();

                    wbSource = null;
                    GC.Collect();
                    return imageData;

                }
            }
            catch (OutOfMemoryException)
            {
                throw new MobiletException(ExceptionTypes.MEMORY_NOT_AVAILABLE);
            }

        }
        /// <summary>
        /// Convert pixel to sepia.
        /// </summary>
        /// <param name="color">pixel to be modified</param>
        /// <returns>modified pixel</returns>
        internal int ColorToSepia(int color)
        {
            int sepia = 0;
            int outputRed, outputGreen, outputBlue;
            // get a,r,g,b value from pixel.
            int a = color >> 24;
            int r = (color & 0x00ff0000) >> 16;
            int g = (color & 0x0000ff00) >> 8;
            int b = (color & 0x000000ff);
            //for sepia http://stackoverflow.com/questions/9448478/what-is-wrong-with-this-sepia-tone-conversion-algorithm

            outputRed = (byte)Math.Min(255, (int)((.393 * r) + (.769 * g) + (.189 * b))); //red
            outputGreen = (byte)Math.Min(255, (int)((.349 * r) + (.686 * g) + (.168 * b))); //green
            outputBlue = (byte)Math.Min(255, (int)((.272 * r) + (.534 * g) + (.131 * b))); //blue

            sepia = ((a & 0xFF) << 24) | ((outputRed & 0xFF) << 16) | ((outputGreen & 0xFF) << 8) | (outputBlue & 0xFF);

            return sepia;
        }
        /// <summary>
        /// convert pixel to grayscale.
        /// </summary>
        /// <param name="color">pixel to be modified</param>
        /// <returns>modified pixel</returns>
        internal int ColorToGray(int color)
        {
            int gray = 0;
            int outputRGB;
            // get a,r,g,b value from pixel.
            int a = color >> 24;
            int r = (color & 0x00ff0000) >> 16;
            int g = (color & 0x0000ff00) >> 8;
            int b = (color & 0x000000ff);
            //for grayScale http://tech.pro/tutorial/660/csharp-tutorial-convert-a-color-image-to-grayscale
            outputRGB = (byte)((.3 * r) + (.59 * g) + (.11 * b)); //gray

            gray = ((a & 0xFF) << 24) | ((outputRGB & 0xFF) << 16) | ((outputRGB & 0xFF) << 8) | (outputRGB & 0xFF);

            return gray;
        }

    }
}

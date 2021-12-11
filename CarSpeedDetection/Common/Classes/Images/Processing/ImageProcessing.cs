using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarSpeedDetection.Common.Extensions;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace CarSpeedDetection.Common.Classes.Images.Processing
{
    internal static class ImageProcessing
    {
        /// <summary>
        /// Video Capture object
        /// </summary>
        public static VideoCapture Capture { get; set; }

        /// <summary>
        /// Background substractor object
        /// </summary>
        public static IBackgroundSubtractor BackgroundSubtractor { get; set; }

        /// <summary>
        /// Returns foreground mask of frame
        /// </summary>
        /// <param name="frame">Frame that the background subtraction will be applied to </param>
        /// <returns></returns>
        public static Mat GetForegroundMask(Mat frame)
        {
            Mat smoothFrame = new Mat();
            CvInvoke.GaussianBlur(frame, smoothFrame, new Size(3, 3), 1);
            Mat foregroundMask = new Mat();
            BackgroundSubtractor.Apply(smoothFrame, foregroundMask);

            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            CvInvoke.Threshold(foregroundMask, foregroundMask, 200, 255, ThresholdType.Binary);
            CvInvoke.MorphologyEx(foregroundMask, foregroundMask, MorphOp.Open, 
                kernel, new Point(-1, -1), 5, BorderType.Default, new MCvScalar(0));

            return foregroundMask;
        }

        /// <summary>
        /// Return the contour of the moving object
        /// </summary>
        /// <param name="foregroundMask">mask of the frame</param>
        /// <param name="contours">contours container</param>
        public static void GetContoursFromMask(Mat foregroundMask, out VectorOfVectorOfPoint contours)
        {
            contours = new VectorOfVectorOfPoint();
            if (foregroundMask == null) return;

            CvInvoke.FindContours(foregroundMask, contours, null, RetrType.External,
                ChainApproxMethod.ChainApproxSimple);
        }

        /// <summary>
        /// Draw object's contours and centroid
        /// </summary>
        /// <param name="frame">frame to draw geometry</param>
        /// <param name="contours">object's countours</param>
        /// <param name="centroid">centroid of moving object</param>
        public static void DrawObjectGeometry(Mat frame, VectorOfVectorOfPoint contours, out Point centroid)
        {
            centroid = new Point();
            for (var i = 0; i < contours.Size; i++)
            {
                var boundingRectangle = CvInvoke.BoundingRectangle(contours[i]);

                if (boundingRectangle.Width < 300)
                    continue;

                CvInvoke.Rectangle(frame, boundingRectangle, new MCvScalar(255, 0, 0), 4);
                centroid = new Point(boundingRectangle.X + boundingRectangle.Width / 2, boundingRectangle.Y + boundingRectangle.Height / 2);
                CvInvoke.Circle(frame, centroid, 3, new MCvScalar(255,  0, 0), 4);
                break;
            }
        }
    }
}
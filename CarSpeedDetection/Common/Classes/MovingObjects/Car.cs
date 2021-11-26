using System;
using System.Collections.Generic;
using System.Drawing;
using CarSpeedDetection.Common.Classes.Motion;
using Emgu.CV;
using Emgu.CV.Util;

namespace CarSpeedDetection.Common.Classes.MovingObjects
{
    internal class Car : MovingObject
    {
        /// <summary>
        /// Calculates car speed
        /// </summary>
        /// <param name="centroids">centroid of the blob's rectangle</param>
        /// <param name="timeElapsed">time in frame</param>
        /// <returns></returns>
        public double CalculateSpeed(List<Point> centroids, double timeElapsed)
        {
            double pixelSizeInches = GetPixelSize();
            double speed = 0;

             var pixelDifferenceFromFrame = Math.Abs(centroids[0].X - centroids[centroids.Count-1].X); 
             var distanceInches = pixelDifferenceFromFrame * pixelSizeInches; 
             speed = distanceInches / timeElapsed;

             return speed;
        }

        /// <summary>
        /// Returns Pixel size based of real world car mesuasurements and car pixel width
        /// </summary>
        /// <returns></returns>
        public override double GetPixelSize()
        {
            const double carWidthInches = 177;
            const double carWidthPixels = 253;
            return carWidthInches / carWidthPixels;
        }
    }
}

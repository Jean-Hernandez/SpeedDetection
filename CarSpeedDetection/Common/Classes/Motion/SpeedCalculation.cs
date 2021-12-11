using System;
using System.Collections.Generic;
using System.Drawing;

namespace CarSpeedDetection.Common.Classes.Motion
{
    internal class SpeedCalculation : IMovingObject
    {
        public static readonly List<Point> Centroids = new List<Point>();

        /// <summary>
        /// Calculates car speed
        /// </summary>
        /// <param name="timeElapsed">time in frame</param>
        /// <returns></returns>
        public double  CalculateSpeed(double timeElapsed)
        {
            double pixelSizeInches = GetPixelSize();
            double maxSpeed = 0;
            for (int i = 0; i < Centroids.Count - 1; i++)
            {
                var pixelDifference = Math.Abs(Centroids[i].X - Centroids[i+1].X);
                var pixelDifferenceInches = pixelSizeInches * pixelDifference;
                var speed = pixelDifferenceInches / timeElapsed;

                if (speed > maxSpeed)
                    maxSpeed = speed;
            }

            return maxSpeed;
        }

        /// <summary>
        /// Returns Pixel size based of real world car mesuasurements and car pixel width
        /// </summary>
        /// <returns></returns>
        public double GetPixelSize()
        {
            const double carWidthInches = 177;
            const double carWidthPixels = 400;
            return carWidthInches / carWidthPixels;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CarSpeedDetection.Common.Classes.Images.Processing;
using CarSpeedDetection.Common.Classes.Motion;
using CarSpeedDetection.Common.Classes.MovingObjects;
using CarSpeedDetection.Common.Extensions;
using CarSpeedDetection.Properties;
using DirectShowLib;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace CarSpeedDetection.Windows
{
    public partial class SpeedDetectionDialog : Form
    {
        public SpeedDetectionDialog()
        {
            InitializeComponent();
            CenterToScreen();
        }

        private List<Point> Centroids = new List<Point>();

        private void MenuItem_Browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            
            if (DialogResult.OK != of.ShowDialog())
                return;

            ImageProcessing.Capture = new VideoCapture(of.FileName);
            ImageProcessing.BackgroundSubtractor = new BackgroundSubtractorMOG2();

        }

        private void selectDefaultCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageProcessing.Capture = new VideoCapture(0);
            ImageProcessing.BackgroundSubtractor = new BackgroundSubtractorMOG2();
        }

        private void Btn_Play_Click(object sender, EventArgs e)
        {
            Application.Idle += Process;
        } 

        private void Btn_Pause_Click(object sender, EventArgs e)
        {
            Application.Idle -= Process;
        }

        private void Process(object sender, EventArgs e)
        {
            if (ImageProcessing.Capture == null)
                return;

            Mat frame = ImageProcessing.Capture.QueryFrame();

            if (frame == null)
            {
                var car = new Car();
                Watch.Stopwatch.Stop();
                var timeElapsed = Watch.Stopwatch.Elapsed.TotalSeconds;
                var carSpeed = car.CalculateSpeed(Centroids, timeElapsed);
                
                label_Speed.Text = carSpeed + Resources.Car_CalculateSpeed_mph;
                return;
            }

            Mat foregroundMask = ImageProcessing.GetForegroundMask(frame);

            ImageProcessing.GetContoursFromMask(foregroundMask, out VectorOfVectorOfPoint contours);

            var rectangle = ImageProcessing.GetRectangleFromContours(contours);

            if (!rectangle.IsEmpty && rectangle.Width > 250)
            {
                var centroid = new Point((rectangle.Height / 2), (rectangle.Width / 2));
                 Centroids.Add(centroid);
            }
                 
             
            CvInvoke.Rectangle(frame, rectangle, new MCvScalar(255, 0, 0));
            CvInvoke.Resize(frame, frame, new Size(imageBox1.Width, imageBox1.Height));

            imageBox1.Image = frame;
        }
    }
}

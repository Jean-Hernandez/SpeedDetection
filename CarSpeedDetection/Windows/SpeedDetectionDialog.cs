using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CarSpeedDetection.Common.Classes.Images.Processing;
using CarSpeedDetection.Common.Classes.Motion;
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
            Watch.Stopwatch.Start();
            if (ImageProcessing.Capture == null || ImageProcessing.BackgroundSubtractor == null)
                return;

            Mat frame = ImageProcessing.Capture.QueryFrame();

            if (frame == null)
                return;

            Mat foregroundMask = ImageProcessing.GetForegroundMask(frame);

            ImageProcessing.GetContoursFromMask(foregroundMask, out VectorOfVectorOfPoint contours);

            ImageProcessing.DrawObjectGeometry(frame, contours, out Point centroid);

            if(!centroid.IsEmpty)
                SpeedCalculation.Centroids.Add(centroid);

            SpeedCalculation calculator = new SpeedCalculation();
            label_Speed.Text = calculator.CalculateSpeed(Watch.Stopwatch.Elapsed.TotalSeconds) + Resources.Car_CalculateSpeed_mph;

            CvInvoke.Resize(frame, frame, new Size(imageBox1.Width, imageBox1.Height));

            imageBox1.Image = frame;
        }
    }
}

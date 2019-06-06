using System;
using System.Diagnostics;
using System.Windows.Forms;
using Sobolev.Capstone.Connectors;
using Sobolev.Capstone.Streams;
using Sobolev.Capstone.Commands;
using Sobolev.Capstone.Enumerations;
using System.ComponentModel;
using Sobolev.Capstone.PreferencesStorage;

namespace EBRDriver
{
    public partial class MainWindow : Form
    {
        private bool useMindData = false;
        private bool useGazeData = false;

        //HttpConnector httpConnector = HttpConnector.ConnectorObject;
        private MotionCommand RobotDriver = null;
        //private MindStream MS = null;

        public MainWindow()
        {
            InitializeComponent();

            HttpConnector.ConnectionStateChanged += HttpConnector_ConnectionStateChnged;
            Text = $"{HttpConnector.IsRaspberryAvailable}";

            if (HttpConnector.IsRaspberryAvailable)
            {
                SshConnector.Connect(HttpConnector.RaspberryIPAdderss);
                // Debug.WriteLine($"{SshConnector.SshReceiver.IsConnected}=========");
            }

            // --- videoStream1.NetworkConnector = httpConnector;
            videoStream1.EmptyImage = videoStream1.Image;
            videoStream1.MovementDirectionChanged += VideoStream1_MovementDirectionChanged;
            videoStream1.UserPresenceChanged += VideoStream1_UserPresenceChanged;
            videoStream1.StartReadingPicture();

            //MS = new MindStream();

            //MS.BrainWaveChanged += MS_BrainWaveChanged;
            //MS.StartReadingBrainWaveData();

            RobotDriver = new MotionCommand();
            //AppPrferences.Data = "raspberry soda";
        }

        private void VideoStream1_UserPresenceChanged(object sender, bool e)
        {
            if (!e) RobotDriver.Stop();
        }

        private void VideoStream1_MovementDirectionChanged(object sender, MotionDirectionChangedArgs e)
        {
            //Text = $"{MS.BrainDataPacket.NoDevice} --- ";

            //if (!MS.BrainDataPacket.NoDevice)
            //{
            //    if (MS.BrainDataPacket.AttentionLevel < BrainActivityValues.AttentionThresholdValue)
            //    {
            //        videoStream1.StreamAvailable = false;
            //        RobotDriver.Stop();
            //        return;
            //    }
            //}

            
            switch(e.NewMovementArea)
            {
                case MoveGoalRectangles.BackRect:
                    //Debug.WriteLine("MoveGoalRectangles.BackRect");
                    if (videoStream1.StreamAvailable) RobotDriver.GoBack();
                    break;                
                case MoveGoalRectangles.ForwardRect:
                    //Debug.WriteLine("MoveGoalRectangles.ForwardRect");
                    if (videoStream1.StreamAvailable) RobotDriver.GoForward();
                    break;
                case MoveGoalRectangles.LeftRect:
                    //Debug.WriteLine("MoveGoalRectangles.LeftRect");
                    if (videoStream1.StreamAvailable) RobotDriver.TurnLeft();
                    break;
                case MoveGoalRectangles.RightRect:
                    //Debug.WriteLine("MoveGoalRectangles.RightRect");
                    if (videoStream1.StreamAvailable) RobotDriver.TurnRight();
                    break;
                case MoveGoalRectangles.StopRect:
                    //Debug.WriteLine("MoveGoalRectangles.StopRect");
                    if (videoStream1.StreamAvailable) RobotDriver.Stop();
                    break;
                case MoveGoalRectangles.StartRect:
                    //Debug.WriteLine("------------------- Inside stop!!!");
                    videoStream1.StreamAvailable = !videoStream1.StreamAvailable;
                    if (!videoStream1.StreamAvailable) RobotDriver.Stop();
                    break;
            }
        }

        private void MS_BrainWaveChanged(object sender, BrainWaveEventArgs e)
        {
            RobotDriver.Stop();

            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    Text = $"att level is very low!! => {e.AttentionLevel}, med = {e.MeditationLevel}, poor = {e.PoorSignalLevel}";
                }));
            }
            else
                Text = $"att = {e.AttentionLevel}, med = {e.MeditationLevel}, poor = {e.PoorSignalLevel}";
        }

        private void HttpConnector_ConnectionStateChnged(object sender, ConnectionStatusEventArgs e)
        {
            /*try
            {
                Invoke(new Action(() =>
                    {
                        Text += $" - {e.IsRaspberryAvailable}/{HttpConnector.RaspberryIPAdderssAsync()}";
                    }));
            }
            catch
            {
                return;
            }*/
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            RobotDriver.Stop();
        }
    }
}

using Sobolev.Capstone.Connectors;
using Sobolev.Capstone.Enumerations;
using Sobolev.Capstone.GazeInteractionsData;
using Sobolev.Capstone.PreferencesStorage;
using Sobolev.Capstone.Streams;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
//using System.Reflection;

namespace Sobolev.Capstone.Commands
{
    #region Changing of movement direction event's args class
    /// <summary>
    /// Definition of data for using in 
    /// event EventHandler MovementDirectionChanged of GazeCommanderPictureBox class
    /// </summary>
    public sealed class MotionDirectionChangedArgs : EventArgs
    {
        /// <summary>
        /// New direction of robot after gaze direction was changed
        /// </summary>
        public MoveGoalRectangles NewMovementArea = MoveGoalRectangles.Passive;

        /// <summary>
        /// Point on the screen where fixation of the user's gaze was detected
        /// </summary>
        public Point GazePoint = new Point();

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="newMovementArea">New direction of the gaze</param>
        /// <param name="gazePoint">New point of the gaze's fixation</param>
        public MotionDirectionChangedArgs(MoveGoalRectangles newMovementArea, Point gazePoint)
        {
            GazePoint = gazePoint;
            NewMovementArea = newMovementArea;
        }
    }
    #endregion

    #region GazeCommanderPictureBox : PictureBox
    public sealed partial class GazeCommanderPictureBox : PictureBox
    {
        #region stream properties, fields and methods

        private readonly int IntervalMs = InternalStorageProvider.CurrentPreferneces.PictureUpdatingIntervalMS;

        private Uri PictureServerUri = null;
        public Timer streamTimer = null;                
        private WebClient PictureGetter = null;
        private byte[] PictBytes = null;
        private MemoryStream PictMemoryStream = new MemoryStream();
        private Image RawImage = null;
        public Image EmptyImage = null;
        private bool CurrentUserPresenceState = true;

        public void StartReadingPicture()
        {
            if (HttpConnector.IsRaspberryAvailable)
            {
                streamTimer.Enabled = true;
                streamTimer.Start();
            }

            //streamTimer.Enabled = true;
            //streamTimer.Start();
        }

        public void StopPicture()
        {
            streamTimer.Enabled = false;
            streamTimer.Stop();
        }

        private MoveGoalRectangles CurrentGoalRectangle;
        private void GetNewPictureFromServer(object sender, EventArgs e)
        {
            // if there is no user 
            if (CurrentUserPresenceState!=gazeStream.IsUserPresent)
            {
                CurrentUserPresenceState = gazeStream.IsUserPresent;
                UserPresenceChanged?.Invoke(gazeStream, CurrentUserPresenceState);
            }

            if (!gazeStream.IsUserPresent)
            {
                if (EmptyImage != null) Image = EmptyImage;
                // stop the robot
                // set current state as paSSIVE
                LastState = MoveGoalRectangles.Passive;
                CurrentGoalArea = MoveGoalRectangles.Passive;
                CurrentGazePoint = new Point(0, 0);
                return;
            }

            try
            {                
                CurrentGazePoint = PointToClient(gazeStream.GazePoint);
                CurrentGoalRectangle = GazeRectangle.TestGazePoint(CurrentGazePoint);

                // change state only if gaze point was moved to new control area
                if (CurrentGoalRectangle != LastState)
                {
                    if (CurrentGoalRectangle != MoveGoalRectangles.Passive)
                    {
                        LastState = CurrentGoalRectangle;

                        MovementDirectionChanged?.Invoke(this,
                            new MotionDirectionChangedArgs(
                                GazeRectangle.TestGazePoint(CurrentGazePoint), CurrentGazePoint));
                        //Debug.WriteLine($"MovementDirectionChanged fired at {DateTime.Now.ToLongTimeString()} !!!");
                    }
                }

                DownloadRawPicture();
                
                DecorateRawPicture();

                OutputProcessedPicture();                
            }
            catch
            {
                return;
            }
        }

        //private SizeF sizeTxt;
        private void DownloadRawPicture()
        {
            // get picture from web-server and put it to the memory stream
            PictureGetter = new WebClient();
            PictBytes = PictureGetter.DownloadData(PictureServerUri);
            PictMemoryStream = new MemoryStream(PictBytes);

            // get image from stream bytes
            RawImage = Image.FromStream(PictMemoryStream);

            // get drawable surface of picture and draw directly to its bytes
            // Canvas is processed image
            Canvas = Graphics.FromImage(RawImage);
            Canvas.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            SizeF sizeTxt = Canvas.MeasureString(TextTriggerStart, TextFont);
            StartTextSize = (Convert.ToInt32(sizeTxt.Width) / 2, Convert.ToInt32(sizeTxt.Height) / 2);
            sizeTxt = Canvas.MeasureString(TextTriggerStop, TextFont);
            StopTextSize = (Convert.ToInt32(sizeTxt.Width) / 2, Convert.ToInt32(sizeTxt.Height) / 2);
        }

        private void DecorateRawPicture()
        {
            DrawControlRects();
            DrawDirectionText();
            if (StreamAvailable)
                DrawDirectionalFigures[LastState]();
            else
                StartSign();
        }

        private void OutputProcessedPicture()
        {
            Image = RawImage;
            RawImage = null;
            PictMemoryStream.Dispose();
        }


        #endregion --------------------------------------------------------------------------------------

        #region picture decoration properties, fields and methods

        private Graphics Canvas;

        private readonly Color StartColor = Color.FromName(InternalStorageProvider.CurrentPreferneces.StartColor);
        private readonly Color StopColor = Color.FromName(InternalStorageProvider.CurrentPreferneces.StopColor);
        private readonly Color RightLeftColor = Color.FromName(InternalStorageProvider.CurrentPreferneces.RightLeftColor);
        private readonly Color BaсkColor = Color.FromName(InternalStorageProvider.CurrentPreferneces.BaсkColor);
        private readonly Color ForwardColor = Color.FromName(InternalStorageProvider.CurrentPreferneces.ForwardColor);
        private readonly Color StartStopTriggerColor = Color.FromName(InternalStorageProvider.CurrentPreferneces.StartStopTriggerColor);

        private readonly SolidBrush StartBrush = null;
        private readonly SolidBrush ForwardBrush = null;
        private readonly SolidBrush StopBrush = null;
        private readonly SolidBrush BackBrush = null;
        private readonly SolidBrush RightLeftBrush = null;
        private readonly SolidBrush StartStopTriggerBrush = null;

        private readonly int TransparancyLevel = InternalStorageProvider.CurrentPreferneces.TransparancyLevel;

        private readonly Font TextFont = new Font(InternalStorageProvider.CurrentPreferneces.PictureFontName, 
            InternalStorageProvider.CurrentPreferneces.PictureFontSize);

        private readonly Point[] GazeCursorPoints = new Point[3];
        private readonly int TextShifht = InternalStorageProvider.CurrentPreferneces.TextShifht;
        private readonly int PointShift = InternalStorageProvider.CurrentPreferneces.PointShift;

        private readonly string TextTriggerStop = InternalStorageProvider.CurrentPreferneces.TextTriggerStop;
        private readonly string TextTriggerStart = InternalStorageProvider.CurrentPreferneces.TextTriggerStart;

        private (int w, int h) StartTextSize = (0, 0);
        private (int w, int h) StopTextSize = (0, 0);        

        private void DrawControlRects()
        {
            if (StreamAvailable)
            {
                // orange with stop
                Canvas.FillRectangle(StartBrush, GazeRectangle.StartArea);
                Canvas.DrawString(TextTriggerStop, TextFont, StartStopTriggerBrush,
                    GazeRectangle.StartArea.Left + StartTextSize.w - 20,
                    GazeRectangle.StartArea.Top + StartTextSize.h);

                Canvas.FillRectangle(new SolidBrush(Color.FromArgb(TransparancyLevel, StopColor)),
                GazeRectangle.StopArea);

                Canvas.FillRectangle(new SolidBrush(Color.FromArgb(TransparancyLevel, RightLeftColor)),
                    GazeRectangle.LeftArea);

                Canvas.FillRectangle(new SolidBrush(Color.FromArgb(TransparancyLevel, RightLeftColor)),
                    GazeRectangle.RightArea);

                Canvas.FillRectangle(
                    new SolidBrush(Color.FromArgb(TransparancyLevel, BaсkColor)),
                    GazeRectangle.BackArea);

                Canvas.FillRectangle(
                    new SolidBrush(Color.FromArgb(TransparancyLevel, ForwardColor)),
                    GazeRectangle.ForwardArea);
            }
            else
            {
                // red with start
                Canvas.FillRectangle(StopBrush, GazeRectangle.StartArea);
                Canvas.DrawString(TextTriggerStart, TextFont, StartStopTriggerBrush,
                    GazeRectangle.StartArea.Left + StopTextSize.w - 20,
                    GazeRectangle.StartArea.Top + StopTextSize.h);
            }
        }

        private int textWidth;
        private int textHeight;
        private string text;
        private void DrawDirectionText()
        {
            text = $"{EnumStrings.MoveDirectionNames[LastState].ToUpper()}";
            textWidth = Convert.ToInt32(Canvas.MeasureString(text, TextFont).Width);
            textHeight = Convert.ToInt32(Canvas.MeasureString(text, TextFont).Height);


            if (StreamAvailable)
            {
                switch (LastState)
                {
                    case MoveGoalRectangles.LeftRect:
                        Canvas.DrawString(text, TextFont, RightLeftBrush,
                            CurrentGazePoint.X + PointShift + TextShifht,
                            CurrentGazePoint.Y - textHeight / 2);
                        break;
                    case MoveGoalRectangles.RightRect:
                        Canvas.DrawString(text, TextFont, RightLeftBrush,
                            CurrentGazePoint.X - PointShift - TextShifht - textWidth,
                            CurrentGazePoint.Y - textHeight / 2);
                        break;
                    case MoveGoalRectangles.ForwardRect:
                        Canvas.DrawString(text, TextFont, ForwardBrush,
                            CurrentGazePoint.X - textWidth / 2,
                            CurrentGazePoint.Y + PointShift + TextShifht);
                        break;
                    case MoveGoalRectangles.StopRect:
                        Canvas.DrawString(text, TextFont, StopBrush,
                            CurrentGazePoint.X - textWidth / 2,
                            CurrentGazePoint.Y + PointShift + TextShifht);
                        break;
                    case MoveGoalRectangles.BackRect:
                        Canvas.DrawString(text, TextFont, BackBrush,
                            CurrentGazePoint.X - textWidth / 2,
                            CurrentGazePoint.Y - PointShift - TextShifht - textHeight);
                        break;
                    case MoveGoalRectangles.StartRect:
                        Canvas.DrawString(text,
                            TextFont,
                            new SolidBrush(Color.Black),
                            CurrentGazePoint.X - textWidth / 2,
                            CurrentGazePoint.Y + PointShift + TextShifht);
                        break;
                    default:
                        Canvas.DrawString(text,
                            TextFont,
                            new SolidBrush(Color.Black),
                            CurrentGazePoint.X - textWidth / 2,
                            CurrentGazePoint.Y + PointShift + TextShifht);
                        break;
                }
            }            
        }

        private void LeftSign()
        {
            GazeCursorPoints[0] = new Point(CurrentGazePoint.X - PointShift, CurrentGazePoint.Y);
            GazeCursorPoints[1] = new Point(CurrentGazePoint.X + PointShift, CurrentGazePoint.Y - PointShift);
            GazeCursorPoints[2] = new Point(CurrentGazePoint.X + PointShift, CurrentGazePoint.Y + PointShift);
            Canvas.FillPolygon(RightLeftBrush, GazeCursorPoints);
        }

        private void RightSign()
        {
            GazeCursorPoints[0] = new Point(CurrentGazePoint.X + PointShift, CurrentGazePoint.Y);
            GazeCursorPoints[1] = new Point(CurrentGazePoint.X - PointShift, CurrentGazePoint.Y - PointShift);
            GazeCursorPoints[2] = new Point(CurrentGazePoint.X - PointShift, CurrentGazePoint.Y + PointShift);
            Canvas.FillPolygon(RightLeftBrush, GazeCursorPoints);
        }

        private void ForwardSign()
        {
            GazeCursorPoints[0] = new Point(CurrentGazePoint.X, CurrentGazePoint.Y - PointShift);
            GazeCursorPoints[1] = new Point(CurrentGazePoint.X - PointShift, CurrentGazePoint.Y + PointShift);
            GazeCursorPoints[2] = new Point(CurrentGazePoint.X + PointShift, CurrentGazePoint.Y + PointShift);
            Canvas.FillPolygon(ForwardBrush, GazeCursorPoints);
        }

        private void StartSign() => PassiveSign();

        private void BackSign()
        {
            GazeCursorPoints[0] = new Point(CurrentGazePoint.X, CurrentGazePoint.Y + PointShift);
            GazeCursorPoints[1] = new Point(CurrentGazePoint.X - PointShift, CurrentGazePoint.Y - PointShift);
            GazeCursorPoints[2] = new Point(CurrentGazePoint.X + PointShift, CurrentGazePoint.Y - PointShift);
            Canvas.FillPolygon(BackBrush, GazeCursorPoints);
        }

        private void StopSign() =>
            Canvas.FillRectangle(StopBrush, CurrentGazePoint.X - PointShift, CurrentGazePoint.Y - PointShift,
                2 * PointShift, 2 * PointShift);

        private void PassiveSign() =>   
            Canvas.FillEllipse(new SolidBrush(Color.FromArgb(100, Color.Red)), 
                CurrentGazePoint.X - 20, CurrentGazePoint.Y - 20, 40, 40);        

        #endregion ------------------------------------------------------------------------------------

        #region network status detection

        private void NetworkConnectionStateChanged(object sender, ConnectionStatusEventArgs e)
        {
            if (!HttpConnector.IsRaspberryAvailable)
            {
                Image = null;
                if (streamTimer.Enabled)
                {
                    StopPicture();
                }
            }
            else
            {
                // exeption - Uri can not ba parsed - SOMETIMES!!! UNDER UNCLEAR CIRCUMSTANCES DURING NORMAL FUNCTIONING
                PictureServerUri = new Uri($"http://{HttpConnector.RaspberryIPAdderss}/html/cam_pic.php");
            }
        }

        #endregion ------------------------------------------------------------------------------------------

        #region gaze controlling

        private Point CurrentGazePoint;
        //private readonly int StopStartTimeDwellMS = 100;
        private MoveGoalRectangles LastState = MoveGoalRectangles.Passive;
        private GazeStream gazeStream = null;
        public bool StreamAvailable { get; set; } = false;

        #endregion

        #region interaction with mind wave state
        public void CheckMindState(int MindActivityValue)
        {
            if (MindActivityValue < BrainActivityValues.AttentionThresholdValue)
            {
                StreamAvailable = false;
            }
            else
            {
                StreamAvailable = true;
            }
        }
        #endregion

        #region detection of the gaze direction and sending commands to  Raspberry PI

        public event EventHandler<MotionDirectionChangedArgs> MovementDirectionChanged;
        private MoveGoalRectangles CurrentGoalArea = MoveGoalRectangles.Passive;
        private Dictionary<MoveGoalRectangles, Action> DrawDirectionalFigures;

        public event EventHandler<bool> UserPresenceChanged;

        #endregion --------------------------------------------------------------------------------------------

        public GazeCommanderPictureBox() // constructor
        {
            InitializeComponent();

            DrawDirectionalFigures = new Dictionary<MoveGoalRectangles, Action>
            {
                { MoveGoalRectangles.BackRect, BackSign },
                { MoveGoalRectangles.ForwardRect, ForwardSign },
                { MoveGoalRectangles.StartRect, StartSign },
                { MoveGoalRectangles.Passive, PassiveSign },
                { MoveGoalRectangles.StopRect, StopSign },
                { MoveGoalRectangles.LeftRect, LeftSign },
                { MoveGoalRectangles.RightRect, RightSign },
            };

            ForwardBrush = new SolidBrush(ForwardColor);
            StopBrush = new SolidBrush(StopColor);
            BackBrush = new SolidBrush(BaсkColor);
            RightLeftBrush = new SolidBrush(RightLeftColor);
            StartBrush = new SolidBrush(StartColor);
            StartStopTriggerBrush = new SolidBrush(StartStopTriggerColor);

            if (HttpConnector.IsRaspberryAvailable)  // !!!!!!!!!!! exception if there is  no rasp working in LAN
            {
                PictureServerUri = new Uri($"http://{HttpConnector.RaspberryIPAdderss}/html/cam_pic.php");
                HttpConnector.ConnectionStateChanged += NetworkConnectionStateChanged;
            }

            streamTimer = new Timer();
            streamTimer.Interval = IntervalMs;
            streamTimer.Tick += GetNewPictureFromServer;

            gazeStream = new GazeStream(ClientRectangle);
        }
    }
    #endregion
}
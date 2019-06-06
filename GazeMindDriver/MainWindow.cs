using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sobolev.Capstone.Commands;
using Sobolev.Capstone.Connectors;
using  Sobolev.Capstone.Streams;
using  Sobolev.Capstone.PreferencesStorage;

namespace GazeMindDriver
{
    public partial class MainWindow : Form
    {
        private readonly Size FixedSize = new Size(InternalStorageProvider.CurrentPreferneces.PictureWidth, InternalStorageProvider.CurrentPreferneces.PictureHeight);

        private MotionCommand RobotDriver;
        private GazeCommanderPictureBox VideoGazeBox;
        private MindStream MindData;

        public MainWindow()
        {
            InitializeComponent();
            InitializeHttpConnection();
            InitializeMindWaveDeviceConnection();
            InitializeGazeBox();
        }


        #region Local initializations
        private void InitializeGazeBox()
        {
            VideoGazeBox = new GazeCommanderPictureBox();
            VideoGazeBox.Location = new Point(10, 10);
            VideoGazeBox.Image = emptyPictBox.Image;
            VideoGazeBox.EmptyImage = emptyPictBox.Image;
            VideoGazeBox.MaximumSize = FixedSize;
            VideoGazeBox.MinimumSize = FixedSize;
            VideoGazeBox.Size = FixedSize;
            VideoGazeBox.Visible = true;
            VideoGazeBox.TabIndex = 0;
            VideoGazeBox.TabStop = false;
            VideoGazeBox.Parent = this;
        }

        private void InitializeHttpConnection()
        {
            HttpConnector.ConnectionStateChanged += HttpConnector_ConnectionStateChanged;
            if (HttpConnector.IsRaspberryAvailable)
            {
                SshConnector.Connect(HttpConnector.RaspberryIPAdderss);
            }
        }

        private void InitializeMindWaveDeviceConnection()
        {
            MindData = new MindStream();
            MindData.BrainWaveChanged += MindData_BrainWaveChanged;
        }

        
        #endregion

        #region Event handlers
        private void HttpConnector_ConnectionStateChanged(object sender, ConnectionStatusEventArgs e)
        {
            // state of connection was changed
        }

        private void MindData_BrainWaveChanged(object sender, BrainWaveEventArgs e)
        {
            // when brain activity goes below predefined threshold values
        }
        #endregion
    }
}

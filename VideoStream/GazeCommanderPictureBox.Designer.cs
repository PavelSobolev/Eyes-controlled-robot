using System.Drawing;
using Sobolev.Capstone.GazeInteractionsData;
using Sobolev.Capstone.Streams;

namespace Sobolev.Capstone.Commands
{
    partial class GazeCommanderPictureBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Size size = new Size(GazeRectangle.Width, GazeRectangle.Height);

            ClientSize = size;
            MinimumSize = size;
            MaximumSize = size;
        }
        #endregion
    }
}

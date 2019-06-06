namespace EBRDriver
{
    partial class MainWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.videoStream1 = new Sobolev.Capstone.Commands.GazeCommanderPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.videoStream1)).BeginInit();
            this.SuspendLayout();
            // 
            // videoStream1
            // 
            this.videoStream1.Image = ((System.Drawing.Image)(resources.GetObject("videoStream1.Image")));
            this.videoStream1.Location = new System.Drawing.Point(12, 12);
            this.videoStream1.MaximumSize = new System.Drawing.Size(1024, 768);
            this.videoStream1.MinimumSize = new System.Drawing.Size(1024, 768);
            this.videoStream1.Name = "videoStream1";
            this.videoStream1.Size = new System.Drawing.Size(1024, 768);
            this.videoStream1.TabIndex = 0;
            this.videoStream1.TabStop = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ClientSize = new System.Drawing.Size(1071, 794);
            this.Controls.Add(this.videoStream1);
            this.Name = "MainWindow";
            this.Text = "EBRDriver";
            ((System.ComponentModel.ISupportInitialize)(this.videoStream1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Sobolev.Capstone.Commands.GazeCommanderPictureBox videoStream1;
    }
}


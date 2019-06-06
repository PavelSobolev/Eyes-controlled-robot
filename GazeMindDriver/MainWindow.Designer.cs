namespace GazeMindDriver
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
            this.emptyPictBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.emptyPictBox)).BeginInit();
            this.SuspendLayout();
            // 
            // emptyPictBox
            // 
            this.emptyPictBox.ErrorImage = global::GazeMindDriver.Properties.Resources.white;
            this.emptyPictBox.Image = global::GazeMindDriver.Properties.Resources.white;
            this.emptyPictBox.InitialImage = global::GazeMindDriver.Properties.Resources.white;
            this.emptyPictBox.Location = new System.Drawing.Point(18, 18);
            this.emptyPictBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.emptyPictBox.Name = "emptyPictBox";
            this.emptyPictBox.Size = new System.Drawing.Size(432, 315);
            this.emptyPictBox.TabIndex = 0;
            this.emptyPictBox.TabStop = false;
            this.emptyPictBox.Visible = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 592);
            this.Controls.Add(this.emptyPictBox);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainWindow";
            this.Text = "Eye tracking and mind wave data robot controller";
            ((System.ComponentModel.ISupportInitialize)(this.emptyPictBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox emptyPictBox;
    }
}


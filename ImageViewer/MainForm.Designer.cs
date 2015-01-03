namespace ImageViewer
{
    partial class MainForm
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
            this.ImageHolder = new System.Windows.Forms.PictureBox();
            this.ImageNameBox = new System.Windows.Forms.Label();
            this.StatusMessagesBox = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ImageHolder)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageHolder
            // 
            this.ImageHolder.Location = new System.Drawing.Point(0, 0);
            this.ImageHolder.Name = "ImageHolder";
            this.ImageHolder.Size = new System.Drawing.Size(1067, 600);
            this.ImageHolder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ImageHolder.TabIndex = 0;
            this.ImageHolder.TabStop = false;
            // 
            // ImageNameBox
            // 
            this.ImageNameBox.AutoSize = true;
            this.ImageNameBox.Location = new System.Drawing.Point(12, 9);
            this.ImageNameBox.Name = "ImageNameBox";
            this.ImageNameBox.Size = new System.Drawing.Size(88, 13);
            this.ImageNameBox.TabIndex = 1;
            this.ImageNameBox.Text = "[ImageNameBox]";
            // 
            // StatusMessagesBox
            // 
            this.StatusMessagesBox.AutoSize = true;
            this.StatusMessagesBox.Location = new System.Drawing.Point(12, 22);
            this.StatusMessagesBox.Name = "StatusMessagesBox";
            this.StatusMessagesBox.Size = new System.Drawing.Size(109, 13);
            this.StatusMessagesBox.TabIndex = 1;
            this.StatusMessagesBox.Text = "[StatusMessagesBox]";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 600);
            this.Controls.Add(this.StatusMessagesBox);
            this.Controls.Add(this.ImageNameBox);
            this.Controls.Add(this.ImageHolder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.ImageHolder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ImageHolder;
        private System.Windows.Forms.Label ImageNameBox;
        private System.Windows.Forms.Label StatusMessagesBox;
    }
}


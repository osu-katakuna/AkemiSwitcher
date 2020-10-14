namespace AkemiSwitcher
{
    partial class AkemiSwitcherUpdate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AkemiSwitcherUpdate));
            this.logoImg = new System.Windows.Forms.PictureBox();
            this.statusText = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.logoImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressBar)).BeginInit();
            this.SuspendLayout();
            // 
            // logoImg
            // 
            this.logoImg.BackColor = System.Drawing.Color.Transparent;
            this.logoImg.Image = global::AkemiSwitcher.Properties.Resources.logoSmall;
            this.logoImg.Location = new System.Drawing.Point(102, 58);
            this.logoImg.Name = "logoImg";
            this.logoImg.Size = new System.Drawing.Size(195, 195);
            this.logoImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.logoImg.TabIndex = 0;
            this.logoImg.TabStop = false;
            // 
            // statusText
            // 
            this.statusText.AutoSize = true;
            this.statusText.BackColor = System.Drawing.Color.Transparent;
            this.statusText.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusText.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.statusText.Location = new System.Drawing.Point(34, 307);
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(111, 19);
            this.statusText.TabIndex = 2;
            this.statusText.Text = "Please Wait...";
            // 
            // progressBar
            // 
            this.progressBar.BackColor = System.Drawing.Color.Transparent;
            this.progressBar.Location = new System.Drawing.Point(38, 290);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(327, 14);
            this.progressBar.TabIndex = 3;
            this.progressBar.TabStop = false;
            // 
            // AkemiSwitcherUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::AkemiSwitcher.Properties.Resources.UpdateBG;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.statusText);
            this.Controls.Add(this.logoImg);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AkemiSwitcherUpdate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AkemiSwitcher Updater";
#if UPDATABLE
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.AkemiSwitcherUpdate_Load);
#endif
            ((System.ComponentModel.ISupportInitialize)(this.logoImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.progressBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox logoImg;
        private System.Windows.Forms.Label statusText;
        private System.Windows.Forms.PictureBox progressBar;
    }
}
namespace Server_UdpClient
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbServerStatistics = new System.Windows.Forms.TextBox();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.btnAdditionalInfo = new System.Windows.Forms.Button();
            this.btnHistory = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbServerStatistics
            // 
            this.tbServerStatistics.Location = new System.Drawing.Point(12, 12);
            this.tbServerStatistics.Multiline = true;
            this.tbServerStatistics.Name = "tbServerStatistics";
            this.tbServerStatistics.PlaceholderText = "Server statistics";
            this.tbServerStatistics.ReadOnly = true;
            this.tbServerStatistics.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbServerStatistics.Size = new System.Drawing.Size(322, 397);
            this.tbServerStatistics.TabIndex = 0;
            // 
            // btnStartServer
            // 
            this.btnStartServer.Location = new System.Drawing.Point(12, 415);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(322, 23);
            this.btnStartServer.TabIndex = 1;
            this.btnStartServer.Text = "Start server";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // btnAdditionalInfo
            // 
            this.btnAdditionalInfo.Location = new System.Drawing.Point(12, 458);
            this.btnAdditionalInfo.Name = "btnAdditionalInfo";
            this.btnAdditionalInfo.Size = new System.Drawing.Size(155, 23);
            this.btnAdditionalInfo.TabIndex = 2;
            this.btnAdditionalInfo.Text = "Additional Information";
            this.btnAdditionalInfo.UseVisualStyleBackColor = true;
            this.btnAdditionalInfo.Click += new System.EventHandler(this.btnAdditionalInfo_Click);
            // 
            // btnHistory
            // 
            this.btnHistory.Location = new System.Drawing.Point(179, 458);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(155, 23);
            this.btnHistory.TabIndex = 3;
            this.btnHistory.Text = "History (default 1 day)...";
            this.btnHistory.UseVisualStyleBackColor = true;
            this.btnHistory.Click += new System.EventHandler(this.btnHistory_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 496);
            this.Controls.Add(this.btnHistory);
            this.Controls.Add(this.btnAdditionalInfo);
            this.Controls.Add(this.btnStartServer);
            this.Controls.Add(this.tbServerStatistics);
            this.MaximumSize = new System.Drawing.Size(362, 535);
            this.MinimumSize = new System.Drawing.Size(362, 535);
            this.Name = "Form1";
            this.Text = "Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_LoadAsync);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox tbServerStatistics;
        private Button btnStartServer;
        private Button btnAdditionalInfo;
        private Button btnHistory;
    }
}
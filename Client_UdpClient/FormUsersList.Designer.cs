namespace Client_UdpClient
{
    partial class FormUsersList
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
            this.dgvUsersList = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsersList)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvUsersList
            // 
            this.dgvUsersList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsersList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUsersList.Location = new System.Drawing.Point(5, 5);
            this.dgvUsersList.Name = "dgvUsersList";
            this.dgvUsersList.RowTemplate.Height = 25;
            this.dgvUsersList.Size = new System.Drawing.Size(310, 404);
            this.dgvUsersList.TabIndex = 0;
            // 
            // FormUsersList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 414);
            this.Controls.Add(this.dgvUsersList);
            this.MaximumSize = new System.Drawing.Size(336, 453);
            this.MinimumSize = new System.Drawing.Size(336, 453);
            this.Name = "FormUsersList";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "Chat Users List";
            this.Load += new System.EventHandler(this.FormUsersList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsersList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridView dgvUsersList;
    }
}
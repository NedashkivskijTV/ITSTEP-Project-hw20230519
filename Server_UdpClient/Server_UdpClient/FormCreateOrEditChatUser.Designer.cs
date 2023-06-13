namespace Server_UdpClient
{
    partial class FormCreateOrEditChatUser
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbChatId = new System.Windows.Forms.ComboBox();
            this.cbUserId = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 217);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(181, 217);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Chat name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "User Login";
            // 
            // cbChatId
            // 
            this.cbChatId.FormattingEnabled = true;
            this.cbChatId.Location = new System.Drawing.Point(12, 39);
            this.cbChatId.Name = "cbChatId";
            this.cbChatId.Size = new System.Drawing.Size(244, 23);
            this.cbChatId.TabIndex = 4;
            this.cbChatId.SelectedIndexChanged += new System.EventHandler(this.cbChatId_SelectedIndexChanged);
            // 
            // cbUserId
            // 
            this.cbUserId.FormattingEnabled = true;
            this.cbUserId.Location = new System.Drawing.Point(12, 107);
            this.cbUserId.Name = "cbUserId";
            this.cbUserId.Size = new System.Drawing.Size(244, 23);
            this.cbUserId.TabIndex = 5;
            // 
            // FormCreateOrEditChatUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 257);
            this.Controls.Add(this.cbUserId);
            this.Controls.Add(this.cbChatId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximumSize = new System.Drawing.Size(284, 296);
            this.MinimumSize = new System.Drawing.Size(284, 296);
            this.Name = "FormCreateOrEditChatUser";
            this.Text = "Create Or Edit User in Chat";
            this.Load += new System.EventHandler(this.FormCreateOrEditChatUser_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnOK;
        private Button btnCancel;
        private Label label1;
        private Label label2;
        private ComboBox cbChatId;
        private ComboBox cbUserId;
    }
}
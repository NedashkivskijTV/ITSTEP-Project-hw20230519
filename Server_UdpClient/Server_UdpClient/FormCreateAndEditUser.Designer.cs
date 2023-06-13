namespace Server_UdpClient
{
    partial class FormCreateAndEditUser
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
            this.tbUserLogin = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.cbChatList = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCreateUser = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbRepeatPassword = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbBlackList = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbChatsAvailable = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // tbUserLogin
            // 
            this.tbUserLogin.Location = new System.Drawing.Point(12, 34);
            this.tbUserLogin.Name = "tbUserLogin";
            this.tbUserLogin.PlaceholderText = "Enter user login";
            this.tbUserLogin.Size = new System.Drawing.Size(285, 23);
            this.tbUserLogin.TabIndex = 0;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(12, 96);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.PlaceholderText = "Enter Password";
            this.tbPassword.Size = new System.Drawing.Size(285, 23);
            this.tbPassword.TabIndex = 1;
            // 
            // cbChatList
            // 
            this.cbChatList.FormattingEnabled = true;
            this.cbChatList.Location = new System.Drawing.Point(12, 230);
            this.cbChatList.Name = "cbChatList";
            this.cbChatList.Size = new System.Drawing.Size(285, 23);
            this.cbChatList.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "User login";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 212);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(140, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Chats created by the user";
            // 
            // btnCreateUser
            // 
            this.btnCreateUser.Location = new System.Drawing.Point(12, 413);
            this.btnCreateUser.Name = "btnCreateUser";
            this.btnCreateUser.Size = new System.Drawing.Size(75, 23);
            this.btnCreateUser.TabIndex = 6;
            this.btnCreateUser.Text = "OK";
            this.btnCreateUser.UseVisualStyleBackColor = true;
            this.btnCreateUser.Click += new System.EventHandler(this.btnCreateUser_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(222, 413);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tbRepeatPassword
            // 
            this.tbRepeatPassword.Location = new System.Drawing.Point(12, 161);
            this.tbRepeatPassword.Name = "tbRepeatPassword";
            this.tbRepeatPassword.PasswordChar = '*';
            this.tbRepeatPassword.PlaceholderText = "Repeat Password";
            this.tbRepeatPassword.Size = new System.Drawing.Size(285, 23);
            this.tbRepeatPassword.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Password";
            // 
            // cbBlackList
            // 
            this.cbBlackList.FormattingEnabled = true;
            this.cbBlackList.Location = new System.Drawing.Point(12, 353);
            this.cbBlackList.Name = "cbBlackList";
            this.cbBlackList.Size = new System.Drawing.Size(285, 23);
            this.cbBlackList.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 335);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 15);
            this.label5.TabIndex = 11;
            this.label5.Text = "Black list";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 275);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(189, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "Chats to which the user has access";
            // 
            // cbChatsAvailable
            // 
            this.cbChatsAvailable.FormattingEnabled = true;
            this.cbChatsAvailable.Location = new System.Drawing.Point(12, 293);
            this.cbChatsAvailable.Name = "cbChatsAvailable";
            this.cbChatsAvailable.Size = new System.Drawing.Size(285, 23);
            this.cbChatsAvailable.TabIndex = 12;
            // 
            // FormCreateAndEditUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 458);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbChatsAvailable);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbBlackList);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbRepeatPassword);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreateUser);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbChatList);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbUserLogin);
            this.MaximumSize = new System.Drawing.Size(325, 497);
            this.MinimumSize = new System.Drawing.Size(325, 497);
            this.Name = "FormCreateAndEditUser";
            this.Text = "Create Or Edit User";
            this.Load += new System.EventHandler(this.FormCreateAndEditUser_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox tbUserLogin;
        private TextBox tbPassword;
        private ComboBox cbChatList;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button btnCreateUser;
        private Button btnCancel;
        private TextBox tbRepeatPassword;
        private Label label4;
        private ComboBox cbBlackList;
        private Label label5;
        private Label label6;
        private ComboBox cbChatsAvailable;
    }
}
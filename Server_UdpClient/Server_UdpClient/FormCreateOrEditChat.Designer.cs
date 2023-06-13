namespace Server_UdpClient
{
    partial class FormCreateOrEditChat
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
            this.tbChatName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbUserCreator = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbAllUsers = new System.Windows.Forms.ComboBox();
            this.dgvChatMembers = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.btnAddUserToChatList = new System.Windows.Forms.Button();
            this.btnRemoveUserFromChatList = new System.Windows.Forms.Button();
            this.btnCreateOrUpdate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChatMembers)).BeginInit();
            this.SuspendLayout();
            // 
            // tbChatName
            // 
            this.tbChatName.Location = new System.Drawing.Point(12, 37);
            this.tbChatName.Name = "tbChatName";
            this.tbChatName.PlaceholderText = "Enter chat name";
            this.tbChatName.Size = new System.Drawing.Size(240, 23);
            this.tbChatName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Chat name";
            // 
            // cbUserCreator
            // 
            this.cbUserCreator.FormattingEnabled = true;
            this.cbUserCreator.Location = new System.Drawing.Point(12, 105);
            this.cbUserCreator.Name = "cbUserCreator";
            this.cbUserCreator.Size = new System.Drawing.Size(240, 23);
            this.cbUserCreator.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "User - creator";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Users list";
            // 
            // cbAllUsers
            // 
            this.cbAllUsers.FormattingEnabled = true;
            this.cbAllUsers.Location = new System.Drawing.Point(12, 178);
            this.cbAllUsers.Name = "cbAllUsers";
            this.cbAllUsers.Size = new System.Drawing.Size(240, 23);
            this.cbAllUsers.TabIndex = 4;
            // 
            // dgvChatMembers
            // 
            this.dgvChatMembers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvChatMembers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChatMembers.Location = new System.Drawing.Point(288, 37);
            this.dgvChatMembers.Name = "dgvChatMembers";
            this.dgvChatMembers.RowTemplate.Height = 25;
            this.dgvChatMembers.Size = new System.Drawing.Size(240, 164);
            this.dgvChatMembers.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(288, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "Chat members";
            // 
            // btnAddUserToChatList
            // 
            this.btnAddUserToChatList.Location = new System.Drawing.Point(12, 230);
            this.btnAddUserToChatList.Name = "btnAddUserToChatList";
            this.btnAddUserToChatList.Size = new System.Drawing.Size(240, 23);
            this.btnAddUserToChatList.TabIndex = 8;
            this.btnAddUserToChatList.Text = "Add User to chat list";
            this.btnAddUserToChatList.UseVisualStyleBackColor = true;
            this.btnAddUserToChatList.Click += new System.EventHandler(this.btnAddUserToChatList_Click);
            // 
            // btnRemoveUserFromChatList
            // 
            this.btnRemoveUserFromChatList.Location = new System.Drawing.Point(288, 230);
            this.btnRemoveUserFromChatList.Name = "btnRemoveUserFromChatList";
            this.btnRemoveUserFromChatList.Size = new System.Drawing.Size(240, 23);
            this.btnRemoveUserFromChatList.TabIndex = 9;
            this.btnRemoveUserFromChatList.Text = "Remove User from chat list";
            this.btnRemoveUserFromChatList.UseVisualStyleBackColor = true;
            this.btnRemoveUserFromChatList.Click += new System.EventHandler(this.btnRemoveUserFromChatList_Click);
            // 
            // btnCreateOrUpdate
            // 
            this.btnCreateOrUpdate.Location = new System.Drawing.Point(12, 291);
            this.btnCreateOrUpdate.Name = "btnCreateOrUpdate";
            this.btnCreateOrUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnCreateOrUpdate.TabIndex = 10;
            this.btnCreateOrUpdate.Text = "OK";
            this.btnCreateOrUpdate.UseVisualStyleBackColor = true;
            this.btnCreateOrUpdate.Click += new System.EventHandler(this.btnCreateOrUpdate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(453, 291);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormCreateOrEditChat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 326);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreateOrUpdate);
            this.Controls.Add(this.btnRemoveUserFromChatList);
            this.Controls.Add(this.btnAddUserToChatList);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dgvChatMembers);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbAllUsers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbUserCreator);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbChatName);
            this.MaximumSize = new System.Drawing.Size(556, 365);
            this.MinimumSize = new System.Drawing.Size(556, 365);
            this.Name = "FormCreateOrEditChat";
            this.Text = "Create Or Edit Chat";
            this.Load += new System.EventHandler(this.FormCreateOrEditChat_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChatMembers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox tbChatName;
        private Label label1;
        private ComboBox cbUserCreator;
        private Label label2;
        private Label label3;
        private ComboBox cbAllUsers;
        private DataGridView dgvChatMembers;
        private Label label4;
        private Button btnAddUserToChatList;
        private Button btnRemoveUserFromChatList;
        private Button btnCreateOrUpdate;
        private Button btnCancel;
    }
}
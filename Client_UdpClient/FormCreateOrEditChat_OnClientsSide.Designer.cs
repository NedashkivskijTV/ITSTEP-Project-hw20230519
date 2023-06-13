namespace Client_UdpClient
{
    partial class FormCreateOrEditChat_OnClientsSide
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbChatName = new System.Windows.Forms.TextBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvUsersChatMembers = new System.Windows.Forms.DataGridView();
            this.dgvUsersNotInTheChat = new System.Windows.Forms.DataGridView();
            this.btnLeaveChat = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsersChatMembers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsersNotInTheChat)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Chat name";
            // 
            // tbChatName
            // 
            this.tbChatName.Location = new System.Drawing.Point(12, 36);
            this.tbChatName.Name = "tbChatName";
            this.tbChatName.PlaceholderText = "Enter chat name";
            this.tbChatName.Size = new System.Drawing.Size(526, 23);
            this.tbChatName.TabIndex = 2;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(262, 190);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(25, 23);
            this.btnRemove.TabIndex = 27;
            this.btnRemove.Text = "<";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(262, 125);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(25, 23);
            this.btnAdd.TabIndex = 26;
            this.btnAdd.Text = ">";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(463, 260);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 25;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 260);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 24;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(298, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 15);
            this.label3.TabIndex = 23;
            this.label3.Text = "Chat members";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 15);
            this.label2.TabIndex = 22;
            this.label2.Text = "Users list";
            // 
            // dgvUsersChatMembers
            // 
            this.dgvUsersChatMembers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsersChatMembers.Location = new System.Drawing.Point(298, 93);
            this.dgvUsersChatMembers.Name = "dgvUsersChatMembers";
            this.dgvUsersChatMembers.RowTemplate.Height = 25;
            this.dgvUsersChatMembers.Size = new System.Drawing.Size(240, 150);
            this.dgvUsersChatMembers.TabIndex = 21;
            // 
            // dgvUsersNotInTheChat
            // 
            this.dgvUsersNotInTheChat.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsersNotInTheChat.Location = new System.Drawing.Point(12, 93);
            this.dgvUsersNotInTheChat.Name = "dgvUsersNotInTheChat";
            this.dgvUsersNotInTheChat.RowTemplate.Height = 25;
            this.dgvUsersNotInTheChat.Size = new System.Drawing.Size(240, 150);
            this.dgvUsersNotInTheChat.TabIndex = 20;
            // 
            // btnLeaveChat
            // 
            this.btnLeaveChat.Location = new System.Drawing.Point(298, 260);
            this.btnLeaveChat.Name = "btnLeaveChat";
            this.btnLeaveChat.Size = new System.Drawing.Size(75, 23);
            this.btnLeaveChat.TabIndex = 28;
            this.btnLeaveChat.Text = "Leave";
            this.btnLeaveChat.UseVisualStyleBackColor = true;
            this.btnLeaveChat.Click += new System.EventHandler(this.btnLeaveChat_Click);
            // 
            // FormCreateOrEditChat_OnClientsSide
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 290);
            this.Controls.Add(this.btnLeaveChat);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgvUsersChatMembers);
            this.Controls.Add(this.dgvUsersNotInTheChat);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbChatName);
            this.MaximumSize = new System.Drawing.Size(567, 329);
            this.MinimumSize = new System.Drawing.Size(567, 329);
            this.Name = "FormCreateOrEditChat_OnClientsSide";
            this.Text = "Create Or Edit Chat";
            this.Load += new System.EventHandler(this.FormCreateOrEditChat_OnClientsSide_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsersChatMembers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsersNotInTheChat)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox tbChatName;
        private Button btnRemove;
        private Button btnAdd;
        private Button btnCancel;
        private Button btnOK;
        private Label label3;
        private Label label2;
        private DataGridView dgvUsersChatMembers;
        private DataGridView dgvUsersNotInTheChat;
        private Button btnLeaveChat;
    }
}
namespace Server_UdpClient
{
    partial class FormAdditionalInfo
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
            this.dgvAdditionalInfo = new System.Windows.Forms.DataGridView();
            this.lbAdditionalInfo = new System.Windows.Forms.Label();
            this.gbTables = new System.Windows.Forms.GroupBox();
            this.btnChatUsers = new System.Windows.Forms.Button();
            this.btnBlackLists = new System.Windows.Forms.Button();
            this.btnMessages = new System.Windows.Forms.Button();
            this.btnChats = new System.Windows.Forms.Button();
            this.btnUsersTable = new System.Windows.Forms.Button();
            this.gbActions = new System.Windows.Forms.GroupBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnShowAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAdditionalInfo)).BeginInit();
            this.gbTables.SuspendLayout();
            this.gbActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvAdditionalInfo
            // 
            this.dgvAdditionalInfo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAdditionalInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAdditionalInfo.Location = new System.Drawing.Point(6, 45);
            this.dgvAdditionalInfo.Name = "dgvAdditionalInfo";
            this.dgvAdditionalInfo.RowTemplate.Height = 25;
            this.dgvAdditionalInfo.Size = new System.Drawing.Size(587, 291);
            this.dgvAdditionalInfo.TabIndex = 0;
            // 
            // lbAdditionalInfo
            // 
            this.lbAdditionalInfo.AutoSize = true;
            this.lbAdditionalInfo.Location = new System.Drawing.Point(236, 19);
            this.lbAdditionalInfo.Name = "lbAdditionalInfo";
            this.lbAdditionalInfo.Size = new System.Drawing.Size(128, 15);
            this.lbAdditionalInfo.TabIndex = 1;
            this.lbAdditionalInfo.Text = "Additional Information";
            // 
            // gbTables
            // 
            this.gbTables.Controls.Add(this.btnChatUsers);
            this.gbTables.Controls.Add(this.btnBlackLists);
            this.gbTables.Controls.Add(this.btnMessages);
            this.gbTables.Controls.Add(this.btnChats);
            this.gbTables.Controls.Add(this.btnUsersTable);
            this.gbTables.Location = new System.Drawing.Point(12, 12);
            this.gbTables.Name = "gbTables";
            this.gbTables.Size = new System.Drawing.Size(171, 426);
            this.gbTables.TabIndex = 2;
            this.gbTables.TabStop = false;
            this.gbTables.Text = "Tables";
            // 
            // btnChatUsers
            // 
            this.btnChatUsers.Location = new System.Drawing.Point(6, 271);
            this.btnChatUsers.Name = "btnChatUsers";
            this.btnChatUsers.Size = new System.Drawing.Size(159, 23);
            this.btnChatUsers.TabIndex = 4;
            this.btnChatUsers.Text = "Chat Users";
            this.btnChatUsers.UseVisualStyleBackColor = true;
            this.btnChatUsers.Click += new System.EventHandler(this.btnChatUsers_Click);
            // 
            // btnBlackLists
            // 
            this.btnBlackLists.Location = new System.Drawing.Point(6, 221);
            this.btnBlackLists.Name = "btnBlackLists";
            this.btnBlackLists.Size = new System.Drawing.Size(159, 23);
            this.btnBlackLists.TabIndex = 3;
            this.btnBlackLists.Text = "Black Lists";
            this.btnBlackLists.UseVisualStyleBackColor = true;
            this.btnBlackLists.Click += new System.EventHandler(this.btnBlackLists_Click);
            // 
            // btnMessages
            // 
            this.btnMessages.Location = new System.Drawing.Point(6, 176);
            this.btnMessages.Name = "btnMessages";
            this.btnMessages.Size = new System.Drawing.Size(159, 23);
            this.btnMessages.TabIndex = 2;
            this.btnMessages.Text = "Messages";
            this.btnMessages.UseVisualStyleBackColor = true;
            this.btnMessages.Click += new System.EventHandler(this.btnMessages_Click);
            // 
            // btnChats
            // 
            this.btnChats.Location = new System.Drawing.Point(6, 132);
            this.btnChats.Name = "btnChats";
            this.btnChats.Size = new System.Drawing.Size(159, 23);
            this.btnChats.TabIndex = 1;
            this.btnChats.Text = "Chats";
            this.btnChats.UseVisualStyleBackColor = true;
            this.btnChats.Click += new System.EventHandler(this.btnChats_Click);
            // 
            // btnUsersTable
            // 
            this.btnUsersTable.Location = new System.Drawing.Point(6, 85);
            this.btnUsersTable.Name = "btnUsersTable";
            this.btnUsersTable.Size = new System.Drawing.Size(159, 23);
            this.btnUsersTable.TabIndex = 0;
            this.btnUsersTable.Text = "Users";
            this.btnUsersTable.UseVisualStyleBackColor = true;
            this.btnUsersTable.Click += new System.EventHandler(this.btnUsersTable_Click);
            // 
            // gbActions
            // 
            this.gbActions.Controls.Add(this.btnDelete);
            this.gbActions.Controls.Add(this.btnUpdate);
            this.gbActions.Controls.Add(this.btnCreate);
            this.gbActions.Controls.Add(this.btnShowAll);
            this.gbActions.Controls.Add(this.lbAdditionalInfo);
            this.gbActions.Controls.Add(this.dgvAdditionalInfo);
            this.gbActions.Location = new System.Drawing.Point(189, 12);
            this.gbActions.Name = "gbActions";
            this.gbActions.Size = new System.Drawing.Size(599, 426);
            this.gbActions.TabIndex = 3;
            this.gbActions.TabStop = false;
            this.gbActions.Text = "Actions";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(489, 375);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(110, 23);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(326, 375);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(110, 23);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(171, 375);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(110, 23);
            this.btnCreate.TabIndex = 3;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_ClickAsync);
            // 
            // btnShowAll
            // 
            this.btnShowAll.Location = new System.Drawing.Point(6, 375);
            this.btnShowAll.Name = "btnShowAll";
            this.btnShowAll.Size = new System.Drawing.Size(110, 23);
            this.btnShowAll.TabIndex = 2;
            this.btnShowAll.Text = "Show";
            this.btnShowAll.UseVisualStyleBackColor = true;
            this.btnShowAll.Click += new System.EventHandler(this.btnShowAll_Click);
            // 
            // FormAdditionalInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.gbActions);
            this.Controls.Add(this.gbTables);
            this.MaximumSize = new System.Drawing.Size(816, 489);
            this.MinimumSize = new System.Drawing.Size(816, 489);
            this.Name = "FormAdditionalInfo";
            this.Text = "FormAdditionalInfo";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAdditionalInfo)).EndInit();
            this.gbTables.ResumeLayout(false);
            this.gbActions.ResumeLayout(false);
            this.gbActions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridView dgvAdditionalInfo;
        private Label lbAdditionalInfo;
        private GroupBox gbTables;
        private Button btnUsersTable;
        private GroupBox gbActions;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnCreate;
        private Button btnShowAll;
        private Button btnChats;
        private Button btnChatUsers;
        private Button btnBlackLists;
        private Button btnMessages;
    }
}
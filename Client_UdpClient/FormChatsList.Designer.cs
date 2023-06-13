namespace Client_UdpClient
{
    partial class FormChatsList
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDeleteChat = new System.Windows.Forms.Button();
            this.btnEditeChat = new System.Windows.Forms.Button();
            this.btnCreateChat = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dgvChatsList = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChatsList)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnDeleteChat);
            this.panel1.Controls.Add(this.btnEditeChat);
            this.panel1.Controls.Add(this.btnCreateChat);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnSaveChanges);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(5, 388);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(493, 61);
            this.panel1.TabIndex = 0;
            // 
            // btnDeleteChat
            // 
            this.btnDeleteChat.Location = new System.Drawing.Point(297, 26);
            this.btnDeleteChat.Name = "btnDeleteChat";
            this.btnDeleteChat.Size = new System.Drawing.Size(89, 23);
            this.btnDeleteChat.TabIndex = 4;
            this.btnDeleteChat.Text = "Delete Chat";
            this.btnDeleteChat.UseVisualStyleBackColor = true;
            this.btnDeleteChat.Click += new System.EventHandler(this.btnDeleteChat_Click);
            // 
            // btnEditeChat
            // 
            this.btnEditeChat.Location = new System.Drawing.Point(202, 26);
            this.btnEditeChat.Name = "btnEditeChat";
            this.btnEditeChat.Size = new System.Drawing.Size(89, 23);
            this.btnEditeChat.TabIndex = 3;
            this.btnEditeChat.Text = "Edite Chat";
            this.btnEditeChat.UseVisualStyleBackColor = true;
            this.btnEditeChat.Click += new System.EventHandler(this.btnEditeChat_Click);
            // 
            // btnCreateChat
            // 
            this.btnCreateChat.Location = new System.Drawing.Point(107, 26);
            this.btnCreateChat.Name = "btnCreateChat";
            this.btnCreateChat.Size = new System.Drawing.Size(89, 23);
            this.btnCreateChat.TabIndex = 2;
            this.btnCreateChat.Text = "Create Chat";
            this.btnCreateChat.UseVisualStyleBackColor = true;
            this.btnCreateChat.Click += new System.EventHandler(this.btnCreateChat_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(392, 26);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.Location = new System.Drawing.Point(12, 26);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(89, 23);
            this.btnSaveChanges.TabIndex = 0;
            this.btnSaveChanges.Text = "Save changes";
            this.btnSaveChanges.UseVisualStyleBackColor = true;
            this.btnSaveChanges.Click += new System.EventHandler(this.btnSaveChanges_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dgvChatsList);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(5, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(493, 383);
            this.panel2.TabIndex = 1;
            // 
            // dgvChatsList
            // 
            this.dgvChatsList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChatsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvChatsList.Location = new System.Drawing.Point(0, 0);
            this.dgvChatsList.Name = "dgvChatsList";
            this.dgvChatsList.RowTemplate.Height = 25;
            this.dgvChatsList.Size = new System.Drawing.Size(493, 383);
            this.dgvChatsList.TabIndex = 0;
            // 
            // FormChatsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 454);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MaximumSize = new System.Drawing.Size(519, 493);
            this.MinimumSize = new System.Drawing.Size(519, 493);
            this.Name = "FormChatsList";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "Chats List";
            this.Load += new System.EventHandler(this.FormChatsList_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChatsList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Button btnDeleteChat;
        private Button btnEditeChat;
        private Button btnCreateChat;
        private Button btnCancel;
        private Button btnSaveChanges;
        private Panel panel2;
        private DataGridView dgvChatsList;
    }
}
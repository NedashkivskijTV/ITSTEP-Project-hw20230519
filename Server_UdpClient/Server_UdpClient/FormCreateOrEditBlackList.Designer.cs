namespace Server_UdpClient
{
    partial class FormCreateOrEditBlackList
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
            this.cbUserCreator = new System.Windows.Forms.ComboBox();
            this.lbUserCreator = new System.Windows.Forms.Label();
            this.dgvUsersNotBlackListMembers = new System.Windows.Forms.DataGridView();
            this.dgvUsersBlackListMembers = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsersNotBlackListMembers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsersBlackListMembers)).BeginInit();
            this.SuspendLayout();
            // 
            // cbUserCreator
            // 
            this.cbUserCreator.FormattingEnabled = true;
            this.cbUserCreator.Location = new System.Drawing.Point(12, 36);
            this.cbUserCreator.Name = "cbUserCreator";
            this.cbUserCreator.Size = new System.Drawing.Size(526, 23);
            this.cbUserCreator.TabIndex = 0;
            // 
            // lbUserCreator
            // 
            this.lbUserCreator.AutoSize = true;
            this.lbUserCreator.Location = new System.Drawing.Point(12, 18);
            this.lbUserCreator.Name = "lbUserCreator";
            this.lbUserCreator.Size = new System.Drawing.Size(127, 15);
            this.lbUserCreator.TabIndex = 1;
            this.lbUserCreator.Text = "User - black list creator";
            // 
            // dgvUsersNotBlackListMembers
            // 
            this.dgvUsersNotBlackListMembers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsersNotBlackListMembers.Location = new System.Drawing.Point(12, 113);
            this.dgvUsersNotBlackListMembers.Name = "dgvUsersNotBlackListMembers";
            this.dgvUsersNotBlackListMembers.RowTemplate.Height = 25;
            this.dgvUsersNotBlackListMembers.Size = new System.Drawing.Size(240, 150);
            this.dgvUsersNotBlackListMembers.TabIndex = 2;
            // 
            // dgvUsersBlackListMembers
            // 
            this.dgvUsersBlackListMembers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsersBlackListMembers.Location = new System.Drawing.Point(298, 113);
            this.dgvUsersBlackListMembers.Name = "dgvUsersBlackListMembers";
            this.dgvUsersBlackListMembers.RowTemplate.Height = 25;
            this.dgvUsersBlackListMembers.Size = new System.Drawing.Size(240, 150);
            this.dgvUsersBlackListMembers.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Users list";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(298, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Users in black list";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 300);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(463, 300);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(262, 145);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(25, 23);
            this.btnAdd.TabIndex = 8;
            this.btnAdd.Text = ">";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(262, 210);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(25, 23);
            this.btnRemove.TabIndex = 9;
            this.btnRemove.Text = "<";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // FormCreateOrEditBlackList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 339);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgvUsersBlackListMembers);
            this.Controls.Add(this.dgvUsersNotBlackListMembers);
            this.Controls.Add(this.lbUserCreator);
            this.Controls.Add(this.cbUserCreator);
            this.MaximumSize = new System.Drawing.Size(566, 378);
            this.MinimumSize = new System.Drawing.Size(566, 378);
            this.Name = "FormCreateOrEditBlackList";
            this.Text = "Create Or Edit Users BlackList";
            this.Load += new System.EventHandler(this.FormCreateOrEditBlackList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsersNotBlackListMembers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsersBlackListMembers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComboBox cbUserCreator;
        private Label lbUserCreator;
        private DataGridView dgvUsersNotBlackListMembers;
        private DataGridView dgvUsersBlackListMembers;
        private Label label2;
        private Label label3;
        private Button btnOK;
        private Button btnCancel;
        private Button btnAdd;
        private Button btnRemove;
    }
}
namespace Client_UdpClient
{
    partial class FormBlackListEditor
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
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvUsersBlackListMembers = new System.Windows.Forms.DataGridView();
            this.dgvUsersNotBlackListMembers = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsersBlackListMembers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsersNotBlackListMembers)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(260, 131);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(25, 23);
            this.btnRemove.TabIndex = 19;
            this.btnRemove.Text = "<";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(260, 66);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(25, 23);
            this.btnAdd.TabIndex = 18;
            this.btnAdd.Text = ">";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(461, 201);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(10, 201);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(296, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 15);
            this.label3.TabIndex = 15;
            this.label3.Text = "Users in black list";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 15);
            this.label2.TabIndex = 14;
            this.label2.Text = "Users list";
            // 
            // dgvUsersBlackListMembers
            // 
            this.dgvUsersBlackListMembers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsersBlackListMembers.Location = new System.Drawing.Point(296, 34);
            this.dgvUsersBlackListMembers.Name = "dgvUsersBlackListMembers";
            this.dgvUsersBlackListMembers.RowTemplate.Height = 25;
            this.dgvUsersBlackListMembers.Size = new System.Drawing.Size(240, 150);
            this.dgvUsersBlackListMembers.TabIndex = 13;
            // 
            // dgvUsersNotBlackListMembers
            // 
            this.dgvUsersNotBlackListMembers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUsersNotBlackListMembers.Location = new System.Drawing.Point(10, 34);
            this.dgvUsersNotBlackListMembers.Name = "dgvUsersNotBlackListMembers";
            this.dgvUsersNotBlackListMembers.RowTemplate.Height = 25;
            this.dgvUsersNotBlackListMembers.Size = new System.Drawing.Size(240, 150);
            this.dgvUsersNotBlackListMembers.TabIndex = 12;
            // 
            // FormBlackListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 233);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgvUsersBlackListMembers);
            this.Controls.Add(this.dgvUsersNotBlackListMembers);
            this.MaximumSize = new System.Drawing.Size(564, 272);
            this.MinimumSize = new System.Drawing.Size(564, 272);
            this.Name = "FormBlackListEditor";
            this.Text = "Black List Editor";
            this.Load += new System.EventHandler(this.FormBlackListEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsersBlackListMembers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsersNotBlackListMembers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnRemove;
        private Button btnAdd;
        private Button btnCancel;
        private Button btnOK;
        private Label label3;
        private Label label2;
        private DataGridView dgvUsersBlackListMembers;
        private DataGridView dgvUsersNotBlackListMembers;
    }
}
namespace Server_UdpClient
{
    partial class FormCreateOrEditMessage
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
            this.cbMessageCreator = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbChat = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbSystemInfo = new System.Windows.Forms.ComboBox();
            this.tbMessage = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePickerCreateonDate = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.dateTimePickerCreateonTime = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // cbMessageCreator
            // 
            this.cbMessageCreator.FormattingEnabled = true;
            this.cbMessageCreator.Location = new System.Drawing.Point(12, 27);
            this.cbMessageCreator.Name = "cbMessageCreator";
            this.cbMessageCreator.Size = new System.Drawing.Size(201, 23);
            this.cbMessageCreator.TabIndex = 0;
            this.cbMessageCreator.SelectedIndexChanged += new System.EventHandler(this.cbMessageCreator_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "User - message creator";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Chat";
            // 
            // cbChat
            // 
            this.cbChat.FormattingEnabled = true;
            this.cbChat.Location = new System.Drawing.Point(12, 91);
            this.cbChat.Name = "cbChat";
            this.cbChat.Size = new System.Drawing.Size(201, 23);
            this.cbChat.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(253, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "System info";
            // 
            // cbSystemInfo
            // 
            this.cbSystemInfo.FormattingEnabled = true;
            this.cbSystemInfo.Location = new System.Drawing.Point(253, 91);
            this.cbSystemInfo.Name = "cbSystemInfo";
            this.cbSystemInfo.Size = new System.Drawing.Size(201, 23);
            this.cbSystemInfo.TabIndex = 4;
            // 
            // tbMessage
            // 
            this.tbMessage.Location = new System.Drawing.Point(12, 154);
            this.tbMessage.Multiline = true;
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.PlaceholderText = "Enter your message";
            this.tbMessage.Size = new System.Drawing.Size(442, 87);
            this.tbMessage.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "Message";
            // 
            // dateTimePickerCreateonDate
            // 
            this.dateTimePickerCreateonDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerCreateonDate.Location = new System.Drawing.Point(254, 27);
            this.dateTimePickerCreateonDate.Name = "dateTimePickerCreateonDate";
            this.dateTimePickerCreateonDate.Size = new System.Drawing.Size(103, 23);
            this.dateTimePickerCreateonDate.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(254, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "Creation date";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 278);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(379, 278);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(375, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 15);
            this.label6.TabIndex = 14;
            this.label6.Text = "Creation time";
            // 
            // dateTimePickerCreateonTime
            // 
            this.dateTimePickerCreateonTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerCreateonTime.Location = new System.Drawing.Point(375, 27);
            this.dateTimePickerCreateonTime.Name = "dateTimePickerCreateonTime";
            this.dateTimePickerCreateonTime.ShowUpDown = true;
            this.dateTimePickerCreateonTime.Size = new System.Drawing.Size(79, 23);
            this.dateTimePickerCreateonTime.TabIndex = 13;
            // 
            // FormCreateOrEditMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 313);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dateTimePickerCreateonTime);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dateTimePickerCreateonDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbMessage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbSystemInfo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbChat);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbMessageCreator);
            this.MaximumSize = new System.Drawing.Size(483, 352);
            this.MinimumSize = new System.Drawing.Size(483, 352);
            this.Name = "FormCreateOrEditMessage";
            this.Text = "Create Or Edit Message";
            this.Load += new System.EventHandler(this.FormCreateOrEditMessage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComboBox cbMessageCreator;
        private Label label1;
        private Label label2;
        private ComboBox cbChat;
        private Label label3;
        private ComboBox cbSystemInfo;
        private TextBox tbMessage;
        private Label label4;
        private DateTimePicker dateTimePickerCreateonDate;
        private Label label5;
        private Button btnOK;
        private Button btnCancel;
        private Label label6;
        private DateTimePicker dateTimePickerCreateonTime;
    }
}
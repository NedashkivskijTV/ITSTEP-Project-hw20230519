namespace Client_UdpClient
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.tbChatMessages = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbUserMessage = new System.Windows.Forms.TextBox();
            this.btnStartChat = new System.Windows.Forms.Button();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.cbChatsList = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.additionalActionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.historyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.for1DayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.for1WeekToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.for1MonthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.for3MonthsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.chatContactsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blackListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chatEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 145);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Messages";
            // 
            // tbChatMessages
            // 
            this.tbChatMessages.Location = new System.Drawing.Point(12, 163);
            this.tbChatMessages.Multiline = true;
            this.tbChatMessages.Name = "tbChatMessages";
            this.tbChatMessages.PlaceholderText = "Chat messages";
            this.tbChatMessages.ReadOnly = true;
            this.tbChatMessages.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbChatMessages.Size = new System.Drawing.Size(327, 254);
            this.tbChatMessages.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "User name";
            // 
            // tbUserName
            // 
            this.tbUserName.Location = new System.Drawing.Point(12, 49);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.PlaceholderText = "Enter user name";
            this.tbUserName.Size = new System.Drawing.Size(219, 23);
            this.tbUserName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 429);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "User message";
            // 
            // tbUserMessage
            // 
            this.tbUserMessage.Location = new System.Drawing.Point(12, 447);
            this.tbUserMessage.Multiline = true;
            this.tbUserMessage.Name = "tbUserMessage";
            this.tbUserMessage.PlaceholderText = "Enter message";
            this.tbUserMessage.Size = new System.Drawing.Size(327, 80);
            this.tbUserMessage.TabIndex = 5;
            this.tbUserMessage.TextChanged += new System.EventHandler(this.tbUserMessage_TextChanged);
            // 
            // btnStartChat
            // 
            this.btnStartChat.Location = new System.Drawing.Point(237, 49);
            this.btnStartChat.Name = "btnStartChat";
            this.btnStartChat.Size = new System.Drawing.Size(102, 23);
            this.btnStartChat.TabIndex = 6;
            this.btnStartChat.Text = "Start Chat";
            this.btnStartChat.UseVisualStyleBackColor = true;
            this.btnStartChat.Click += new System.EventHandler(this.btnStartChat_Click);
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Location = new System.Drawing.Point(237, 536);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(102, 23);
            this.btnSendMessage.TabIndex = 7;
            this.btnSendMessage.Text = "Send message";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // cbChatsList
            // 
            this.cbChatsList.FormattingEnabled = true;
            this.cbChatsList.Location = new System.Drawing.Point(12, 106);
            this.cbChatsList.Name = "cbChatsList";
            this.cbChatsList.Size = new System.Drawing.Size(327, 23);
            this.cbChatsList.TabIndex = 8;
            this.cbChatsList.SelectedIndexChanged += new System.EventHandler(this.cbChatsList_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Chats List";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.additionalActionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(351, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // additionalActionsToolStripMenuItem
            // 
            this.additionalActionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.historyToolStripMenuItem,
            this.chatContactsToolStripMenuItem,
            this.blackListToolStripMenuItem,
            this.chatEditorToolStripMenuItem});
            this.additionalActionsToolStripMenuItem.Name = "additionalActionsToolStripMenuItem";
            this.additionalActionsToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.additionalActionsToolStripMenuItem.Text = "Menu";
            // 
            // historyToolStripMenuItem
            // 
            this.historyToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.for1DayToolStripMenuItem,
            this.for1WeekToolStripMenuItem,
            this.for1MonthToolStripMenuItem,
            this.for3MonthsToolStripMenuItem,
            this.yearToolStripMenuItem,
            this.allToolStripMenuItem1});
            this.historyToolStripMenuItem.Name = "historyToolStripMenuItem";
            this.historyToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.historyToolStripMenuItem.Text = "Chat History";
            // 
            // for1DayToolStripMenuItem
            // 
            this.for1DayToolStripMenuItem.Name = "for1DayToolStripMenuItem";
            this.for1DayToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.for1DayToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.for1DayToolStripMenuItem.Text = "1 day";
            this.for1DayToolStripMenuItem.Click += new System.EventHandler(this.for1DayToolStripMenuItem_Click);
            // 
            // for1WeekToolStripMenuItem
            // 
            this.for1WeekToolStripMenuItem.Name = "for1WeekToolStripMenuItem";
            this.for1WeekToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.for1WeekToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.for1WeekToolStripMenuItem.Text = "1 week";
            this.for1WeekToolStripMenuItem.Click += new System.EventHandler(this.for1WeekToolStripMenuItem_Click);
            // 
            // for1MonthToolStripMenuItem
            // 
            this.for1MonthToolStripMenuItem.Name = "for1MonthToolStripMenuItem";
            this.for1MonthToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.for1MonthToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.for1MonthToolStripMenuItem.Text = "1 month";
            this.for1MonthToolStripMenuItem.Click += new System.EventHandler(this.for1MonthToolStripMenuItem_Click);
            // 
            // for3MonthsToolStripMenuItem
            // 
            this.for3MonthsToolStripMenuItem.Name = "for3MonthsToolStripMenuItem";
            this.for3MonthsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.for3MonthsToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.for3MonthsToolStripMenuItem.Text = "3 months";
            this.for3MonthsToolStripMenuItem.Click += new System.EventHandler(this.for3MonthsToolStripMenuItem_Click);
            // 
            // yearToolStripMenuItem
            // 
            this.yearToolStripMenuItem.Name = "yearToolStripMenuItem";
            this.yearToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.yearToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.yearToolStripMenuItem.Text = "1 year";
            this.yearToolStripMenuItem.Click += new System.EventHandler(this.yearToolStripMenuItem_Click);
            // 
            // allToolStripMenuItem1
            // 
            this.allToolStripMenuItem1.Name = "allToolStripMenuItem1";
            this.allToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.allToolStripMenuItem1.Size = new System.Drawing.Size(167, 22);
            this.allToolStripMenuItem1.Text = "all";
            this.allToolStripMenuItem1.Click += new System.EventHandler(this.allToolStripMenuItem1_Click);
            // 
            // chatContactsToolStripMenuItem
            // 
            this.chatContactsToolStripMenuItem.Name = "chatContactsToolStripMenuItem";
            this.chatContactsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.chatContactsToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.chatContactsToolStripMenuItem.Text = "Chat contacts";
            this.chatContactsToolStripMenuItem.Click += new System.EventHandler(this.chatContactsToolStripMenuItem_Click);
            // 
            // blackListToolStripMenuItem
            // 
            this.blackListToolStripMenuItem.Name = "blackListToolStripMenuItem";
            this.blackListToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.blackListToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.blackListToolStripMenuItem.Text = "Black list";
            this.blackListToolStripMenuItem.Click += new System.EventHandler(this.blackListToolStripMenuItem_Click);
            // 
            // chatEditorToolStripMenuItem
            // 
            this.chatEditorToolStripMenuItem.Name = "chatEditorToolStripMenuItem";
            this.chatEditorToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.chatEditorToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.chatEditorToolStripMenuItem.Text = "Chat Editor";
            this.chatEditorToolStripMenuItem.Click += new System.EventHandler(this.chatEditorToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 568);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbChatsList);
            this.Controls.Add(this.btnSendMessage);
            this.Controls.Add(this.btnStartChat);
            this.Controls.Add(this.tbUserMessage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbUserName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbChatMessages);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximumSize = new System.Drawing.Size(367, 607);
            this.MinimumSize = new System.Drawing.Size(367, 607);
            this.Name = "Form1";
            this.Text = "Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TextBox tbChatMessages;
        private Label label2;
        private Label label3;
        private TextBox tbUserMessage;
        private Button btnStartChat;
        private Button btnSendMessage;
        private TextBox tbUserName;
        private ComboBox cbChatsList;
        private Label label4;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem additionalActionsToolStripMenuItem;
        private ToolStripMenuItem historyToolStripMenuItem;
        private ToolStripMenuItem for1DayToolStripMenuItem;
        private ToolStripMenuItem for1WeekToolStripMenuItem;
        private ToolStripMenuItem for1MonthToolStripMenuItem;
        private ToolStripMenuItem for3MonthsToolStripMenuItem;
        private ToolStripMenuItem yearToolStripMenuItem;
        private ToolStripMenuItem chatContactsToolStripMenuItem;
        private ToolStripMenuItem blackListToolStripMenuItem;
        private ToolStripMenuItem chatEditorToolStripMenuItem;
        private ToolStripMenuItem allToolStripMenuItem1;
    }
}
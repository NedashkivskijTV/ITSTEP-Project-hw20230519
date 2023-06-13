namespace Client_UdpClient
{
    partial class FormAuthorizationRegistration
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
            this.btnAuthoriz = new System.Windows.Forms.Button();
            this.btnRegistration = new System.Windows.Forms.Button();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbRepeatPassword = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnAuthoriz
            // 
            this.btnAuthoriz.Location = new System.Drawing.Point(12, 151);
            this.btnAuthoriz.Name = "btnAuthoriz";
            this.btnAuthoriz.Size = new System.Drawing.Size(75, 23);
            this.btnAuthoriz.TabIndex = 0;
            this.btnAuthoriz.Text = "Authorize";
            this.btnAuthoriz.UseVisualStyleBackColor = true;
            this.btnAuthoriz.Click += new System.EventHandler(this.btnAuthoriz_Click);
            // 
            // btnRegistration
            // 
            this.btnRegistration.Location = new System.Drawing.Point(109, 151);
            this.btnRegistration.Name = "btnRegistration";
            this.btnRegistration.Size = new System.Drawing.Size(75, 23);
            this.btnRegistration.TabIndex = 1;
            this.btnRegistration.Text = "Register";
            this.btnRegistration.UseVisualStyleBackColor = true;
            this.btnRegistration.Click += new System.EventHandler(this.btnRegistration_Click);
            // 
            // tbLogin
            // 
            this.tbLogin.Location = new System.Drawing.Point(12, 23);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.PlaceholderText = "Enter login";
            this.tbLogin.Size = new System.Drawing.Size(266, 23);
            this.tbLogin.TabIndex = 2;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(12, 64);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PlaceholderText = "Enter Password";
            this.tbPassword.Size = new System.Drawing.Size(266, 23);
            this.tbPassword.TabIndex = 3;
            this.tbPassword.UseSystemPasswordChar = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(203, 151);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tbRepeatPassword
            // 
            this.tbRepeatPassword.Location = new System.Drawing.Point(12, 107);
            this.tbRepeatPassword.Name = "tbRepeatPassword";
            this.tbRepeatPassword.PlaceholderText = "Repeat Password";
            this.tbRepeatPassword.Size = new System.Drawing.Size(266, 23);
            this.tbRepeatPassword.TabIndex = 5;
            this.tbRepeatPassword.UseSystemPasswordChar = true;
            // 
            // FormAuthorizationRegistration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 197);
            this.Controls.Add(this.tbRepeatPassword);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbLogin);
            this.Controls.Add(this.btnRegistration);
            this.Controls.Add(this.btnAuthoriz);
            this.MaximumSize = new System.Drawing.Size(304, 236);
            this.MinimumSize = new System.Drawing.Size(304, 236);
            this.Name = "FormAuthorizationRegistration";
            this.Text = "Authorization / Registration";
            this.Load += new System.EventHandler(this.FormAuthorizationRegistration_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnAuthoriz;
        private Button btnRegistration;
        private TextBox tbLogin;
        private TextBox tbPassword;
        private Button btnCancel;
        private TextBox tbRepeatPassword;
    }
}
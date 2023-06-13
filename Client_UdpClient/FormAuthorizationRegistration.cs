using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChatLibrary;

namespace Client_UdpClient
{
    // Форма для отримання даних по авторизації / реєстрації користувача
    public partial class FormAuthorizationRegistration : Form
    {
        // Модель для передачі даних між формами
        ParaLoginPass paraLoginPass;
        
        // Прапорець на відкриття поля для повторного введення пароля
        bool isAuthorization;


        public FormAuthorizationRegistration(ParaLoginPass paraLoginPass, bool isAuthorization)
        {
            InitializeComponent();

            this.paraLoginPass = paraLoginPass;
            this.isAuthorization = isAuthorization;

            tbLogin.Select();
        }

        private void btnAuthoriz_Click(object sender, EventArgs e)
        {
            if (tbLogin.Text.Trim().Length > 0 && tbPassword.Text.Trim().Length > 0) // перевірка заповненості текстових полів
            {
                paraLoginPass.Login = tbLogin.Text;
                paraLoginPass.Pass = tbPassword.Text;
                this.DialogResult = DialogResult.OK;
            }
            else 
            {
                MessageBox.Show(SystemInfo.SystemInfo_MessageAllFieldsMustBeСompleted); // All fields of the form must be filled out - Всі поля форми повинні бути заповнені
            }
        }

        private void btnRegistration_Click(object sender, EventArgs e)
        {
            if(isAuthorization) // відкриття поля для повторного введення пароля
            {
                tbRepeatPassword.Show();
                isAuthorization = false;
                return;
            }

            // Визначення заповненості полів / еквівалентрості паролів
            if(tbLogin.Text.Length > 0 && tbRepeatPassword.Text.Length > 0 && tbPassword.Text.Equals(tbRepeatPassword.Text)) 
            {
                paraLoginPass.Login = tbLogin.Text;
                paraLoginPass.Pass = tbPassword.Text;
                this.DialogResult = DialogResult.Yes;
            }
            else if(tbLogin.Text.Length == 0 || tbRepeatPassword.Text.Length == 0)
            {
                MessageBox.Show(SystemInfo.SystemInfo_MessageAllFieldsMustBeСompleted); // All fields of the form must be filled out - Всі поля форми повинні бути заповнені
            }
            else
            {
                MessageBox.Show(SystemInfo.SystemInfo_MessageEnteredDifferentPasswords); // Введено різні паролі
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void FormAuthorizationRegistration_Load(object sender, EventArgs e)
        {
            // Первинне завантаження даних з моделі
            tbLogin.Text = paraLoginPass.Login;
            tbPassword.Text = paraLoginPass.Pass;

            // Приховання / візуалізація поля повторного введення пароля
            if (isAuthorization)
            {
                tbRepeatPassword.Hide();
                //btnRegistration.Hide();
            }
        }
    }
}

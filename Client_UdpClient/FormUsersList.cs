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
    public partial class FormUsersList : Form
    {
        
        List<ModelUsersLoginAndId> usersList;
        int currentChatCreatoreId;

        public FormUsersList(List<ModelUsersLoginAndId> usersList, int currentChatCreatoreId)
        {
            InitializeComponent();

            this.usersList = usersList;
            this.currentChatCreatoreId = currentChatCreatoreId;

            // Налаштування відображення інф у елементі dgvAdditionalInfo - одним суцільним рядком
            dgvUsersList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // відображення інф у елементі датаГрідВюв - вписати всі колонки у вікно
            dgvUsersList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void FormUsersList_Load(object sender, EventArgs e)
        {
            var users = usersList
                .Select(u => new {Login = u.Login, Status = u.Status, Creator = u.Id == currentChatCreatoreId ? $"{SystemInfo.SystemInfo_Creator_creator}" : "" })
                .ToList();

            dgvUsersList.DataSource = null;
            dgvUsersList.DataSource = users;
        }
    }
}

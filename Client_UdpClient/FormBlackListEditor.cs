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
    public partial class FormBlackListEditor : Form
    {
        List<ModelUsersLoginAndId> usersBlackList;
        List<ModelUsersLoginAndId> usersList;
        int userId;

        public FormBlackListEditor(List<ModelUsersLoginAndId> usersBlackList, List<ModelUsersLoginAndId> usersList, int userId)
        {
            InitializeComponent();

            this.usersBlackList = usersBlackList;
            this.usersList = usersList;
            this.userId = userId;

            // Налаштування відображення інф у елементах dgv - одним суцільним рядком
            dgvUsersBlackListMembers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsersNotBlackListMembers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // відображення інф у елементі датаГрідВюв - вписати всі колонки у вікно
            dgvUsersBlackListMembers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsersNotBlackListMembers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        private void FormBlackListEditor_Load(object sender, EventArgs e)
        {
            UpdateDgvBlackListMembers();
        }

        private void UpdateDgvBlackListMembers()
        {
            dgvUsersBlackListMembers.DataSource = null;
            dgvUsersBlackListMembers.DataSource = usersBlackList
                .Select(u => new { Id = u.Id, Login = u.Login, Status = u.Status })
                .ToList();

            dgvUsersNotBlackListMembers.DataSource = null;
            dgvUsersNotBlackListMembers.DataSource = usersList
                .Where(u => u.Id != this.userId)
                .Where(u => usersBlackList.Where(b => b.Id == u.Id).Count() == 0)
                .Select(u => new { Id = u.Id, Login = u.Login, Status = u.Status })
                .ToList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dgvUsersNotBlackListMembers.SelectedRows.Count > 0)
            {
                List<ModelUsersLoginAndId> usersNotInBlackList = usersList
                    .Where(u => u.Id != this.userId)
                    .Where(u => usersBlackList.Where(b => b.Id == u.Id).Count() == 0)
                    .ToList();

                int userId = int.Parse(dgvUsersNotBlackListMembers.SelectedRows[0].Cells[0].Value.ToString());
                ModelUsersLoginAndId user = usersNotInBlackList.Find(u => u.Id == userId);

                usersBlackList.Add(user);

                UpdateDgvBlackListMembers();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvUsersBlackListMembers.SelectedRows.Count > 0)
            {
                int userId = int.Parse(dgvUsersBlackListMembers.SelectedRows[0].Cells[0].Value.ToString());
                ModelUsersLoginAndId user = usersBlackList.Find(u => u.Id == userId);

                usersBlackList.Remove(user);

                UpdateDgvBlackListMembers();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}

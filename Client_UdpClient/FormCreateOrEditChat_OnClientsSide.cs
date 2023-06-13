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
    public partial class FormCreateOrEditChat_OnClientsSide : Form
    {

        ModelChatsNameAndId chat;
        List<ModelUsersLoginAndId> usersList;
        int userId;

        List<ModelUsersLoginAndId> usersChatMembers;

        public FormCreateOrEditChat_OnClientsSide(ModelChatsNameAndId chat, List<ModelUsersLoginAndId> usersList, int userId)
        {
            InitializeComponent();

            this.chat = chat;
            this.usersList = new List<ModelUsersLoginAndId>(usersList);
            this.usersList.Add(new ModelUsersLoginAndId(0, SystemUsers.SystemUser_All));
            this.userId = userId;

            usersChatMembers = chat.UsersInChat;

            // Налаштування відображення інф у елементі dgv - одним суцільним рядком
            dgvUsersChatMembers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsersNotInTheChat.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // відображення інф у елементі датаГрідВюв - вписати всі колонки у вікно
            dgvUsersChatMembers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsersNotInTheChat.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Відключення можливості вносити зміни у чат, якщо користувач не є власником чата
            if(chat.ChatCreatorId != userId)
            {
                tbChatName.Enabled = false;
                btnAdd.Enabled = false;
                btnRemove.Enabled = false;
                btnLeaveChat.Hide();
            }
            else
            {
                btnLeaveChat.Hide();
            }
        }

        private void FormCreateOrEditChat_OnClientsSide_Load(object sender, EventArgs e)
        {
            if(chat.ChatName.Length > 0)
            {
                tbChatName.Text = chat.ChatName;
            }

            UpdateDgvElements();
        }

        private void UpdateDgvElements()
        {
            dgvUsersChatMembers.DataSource = null;
            dgvUsersChatMembers.DataSource = usersChatMembers
                .Select(u => new { Login = u.Login, Status = u.Login == SystemUsers.SystemUser_All ? "" : u.Status })
                .ToList();

            dgvUsersNotInTheChat.DataSource = null;
            dgvUsersNotInTheChat.DataSource = usersList
                .Where(u => usersChatMembers.Where(x => x.Login == u.Login).Count() == 0)
                .Select(u => new { Login = u.Login, Status = u.Login == SystemUsers.SystemUser_All ? "" : u.Status })
                .ToList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dgvUsersNotInTheChat.SelectedRows.Count > 0)
            {
                List<ModelUsersLoginAndId> usersNotInChat = usersList
                    .Where(u => usersChatMembers.Where(x => x.Login == u.Login).Count() == 0)
                    .ToList();

                string userlogin = dgvUsersNotInTheChat.SelectedRows[0].Cells[0].Value.ToString();
                
                ModelUsersLoginAndId user = usersNotInChat.Find(u => u.Login == userlogin);

                usersChatMembers.Add(user);

                UpdateDgvElements();
            }

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvUsersChatMembers.SelectedRows.Count > 0)
            {
                string userLogin = dgvUsersChatMembers.SelectedRows[0].Cells[0].Value.ToString();
                ModelUsersLoginAndId user = usersChatMembers.Find(u => u.Login == userLogin);

                usersChatMembers.Remove(user);

                UpdateDgvElements();
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(tbChatName.Text.Length > 0)
            {
                chat.ChatName = tbChatName.Text;
                chat.UsersInChat = usersChatMembers;
                chat.AmountUsersInChat = usersChatMembers.Count();

                // Якщо список учасників чату містить системного користувача all
                // - користувачі видаляються, лишається тільки all
                if (usersChatMembers.Select(u => u.Login).ToList().Contains(SystemUsers.SystemUser_All))
                {
                    usersChatMembers.Clear();
                    usersChatMembers.Add(new ModelUsersLoginAndId(0, SystemUsers.SystemUser_All));

                    chat.AmountUsersInChat = usersList.Count() - 1; // Під час ініціалізації даних додається системний користувач all - потрібно відняти його від загальної кількості користувачів
                }
                else if (!usersChatMembers.Select(u => u.Id).ToList().Contains(chat.ChatCreatorId))
                {
                    ModelUsersLoginAndId user = usersList.Find(u => u.Id == chat.ChatCreatorId);
                    usersChatMembers.Add(user);
                    chat.AmountUsersInChat = usersChatMembers.Count();
                }

                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(SystemInfo.SystemInfo_MessageChatNameFieldMustBeFilled);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnLeaveChat_Click(object sender, EventArgs e)
        {
            // TODO
        }
    }
}

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

namespace Server_UdpClient
{
    public partial class FormCreateOrEditChat : Form
    {

        Chat chat;
        int userCreatorId;
        List<User> usersAll;
        List<string> userInChatLogin;

        public FormCreateOrEditChat(Chat chat, int userCreatorId = -1)
        {
            InitializeComponent();

            this.chat = chat;
            this.userCreatorId = userCreatorId;

            // Налаштування відображення інф у елементі dgvAdditionalInfo - одним суцільним рядком
            dgvChatMembers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void btnAddUserToChatList_Click(object sender, EventArgs e)
        {
            // Оновлення компонента ДатаГрідВюв після додавання користувача до учасників чата
            userInChatLogin.Add((cbAllUsers.SelectedItem as User).Login);

            UpdateChatMembersElements();
        }

        private void btnRemoveUserFromChatList_Click(object sender, EventArgs e)
        {
            if (dgvChatMembers.SelectedRows.Count > 0)
            {
                string login = dgvChatMembers.SelectedRows[0].Cells[0].Value.ToString();

                userInChatLogin.Remove(login);
                UpdateChatMembersElements();
            }
        }

        private void UpdateChatMembersElements()
        {
            var userInChatLoginLinqCollection = userInChatLogin.Select(u => new { Login = u }).ToList();
            dgvChatMembers.DataSource = null;
            dgvChatMembers.DataSource = userInChatLoginLinqCollection;

            cbAllUsers.DataSource = null;
            cbAllUsers.DisplayMember = nameof(User.Login);
            cbAllUsers.ValueMember = nameof(User.Id);
            var usersNotInChat = usersAll.Where(u => !userInChatLogin.Contains(u.Login)).ToList();
            cbAllUsers.DataSource = usersNotInChat;
        }

        private void btnCreateOrUpdate_Click(object sender, EventArgs e)
        {
            if(cbUserCreator.SelectedIndex != -1 && tbChatName.Text.Length > 0)
            {
                // Модифікація списку учасників чату
                // - У разі наявності користувача ALL усі решта користувачів видаляються
                // (ALL - системний користувач, означає, що доступ до чату мають усі)
                // - У разі відсутності у списку учаників чату користувача, що його створив - останній додається автоматично
                // (власник чату завжди має до нього доступ)
                if (userInChatLogin.Contains(SystemUsers.SystemUser_All))
                {
                    userInChatLogin = new List<string> { SystemUsers.SystemUser_All };
                }
                else if(!userInChatLogin.Contains((cbUserCreator.SelectedItem as User).Login))
                {
                    string login = (cbUserCreator.SelectedItem as User).Login;

                    userInChatLogin.Add(login);
                    UpdateChatMembersElements();
                }

                chat.ChatName = tbChatName.Text;
                chat.CreatorId = usersAll.FirstOrDefault(u => u.Login.Equals((cbUserCreator.SelectedItem as User).Login)).Id;

                // Наповнення колекції користувачів чату відповідними елементами
                // та передача її (колекції) у модель
                List<ChatUser> chatUsers = new List<ChatUser>();
                foreach (string userLogin in userInChatLogin)
                {
                    ChatUser userChatMember = new ChatUser();
                    var temp = usersAll
                        .FirstOrDefault(u => u.Login == userLogin);
                    userChatMember.ChatUserId = temp.Id;
                    chatUsers.Add(userChatMember);
                }
                chat.ChatUsers = chatUsers;
                
                this.DialogResult = DialogResult.OK;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void FormCreateOrEditChat_Load(object sender, EventArgs e)
        {
            using(ChatUdp01Context context = new ChatUdp01Context())
            {
                tbChatName.Text = chat.ChatName;

                // отримання списку усіх користувачів
                usersAll = context.Users.ToList();
                // отримання списку користувачів даного чату
                var chatUsers = context.ChatUsers
                    .Where(c => c.ChatId == chat.Id)
                    .Select(cu => new { cu.ChatUserNavigation.Login })
                    .ToList();

                // Перенесення даних з колекції "невизначеного типу" до колекції рядків
                userInChatLogin = new List<string>();
                foreach (var user in chatUsers)
                {
                    userInChatLogin.Add(user.Login);
                }
                var userInChatLoginLinqCollection = userInChatLogin.Select(u => new {Login = u}).ToList();

                // Заповнення комбоБокс користувачів - автор чату
                cbUserCreator.DataSource = null;
                cbUserCreator.DisplayMember = nameof(User.Login);
                cbUserCreator.ValueMember = nameof(User.Id);

                if (userCreatorId == -1)
                {
                    cbUserCreator.DataSource = usersAll;
                    if (chat.CreatorId != 0)
                    {
                        int index = 0;
                        foreach (User user in usersAll)
                        {
                            if (user.Id == chat.CreatorId)
                            {
                                break;
                            }
                            index++;
                        }
                        cbUserCreator.SelectedIndex = index;
                    }
                }
                else
                {
                    cbUserCreator.DataSource = usersAll.Where(u => u.Id == userCreatorId).ToList();
                }

                // Заповнення комбоБокс користувачів - учасників чату
                cbAllUsers.DataSource = null;
                cbAllUsers.DisplayMember = nameof(User.Login);
                cbAllUsers.ValueMember = nameof(User.Id);
                var usersNotInChat = usersAll.Where(u => !userInChatLogin.Contains(u.Login)).ToList();
                cbAllUsers.DataSource = usersNotInChat;

                // Заповнення елемента dgvChatMembers - список учасників чату
                dgvChatMembers.DataSource = null;

                dgvChatMembers.DataSource = userInChatLoginLinqCollection; // -------------------------------------------
            }
        }

    }
}

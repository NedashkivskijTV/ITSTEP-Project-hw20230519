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
    public partial class FormCreateAndEditUser : Form
    {
        User user;

        string oldUserName;

        public FormCreateAndEditUser(User user)
        {
            InitializeComponent();
            
            this.user = user;
            this.oldUserName = new string(user.Login);

            InitData();
        }

        private void InitData()
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                tbUserLogin.Text = user.Login;
                tbPassword.Text = user.Passvord;
                tbRepeatPassword.Text = user.Passvord;
                
                // Відображення списку чатів користувача ---------------------------------------------------------
                cbChatList.DataSource = null;
                cbChatList.DisplayMember = nameof(Chat.ChatName);
                cbChatList.ValueMember = nameof(Chat.Id);
                cbChatList.DataSource = context.Chats.Where(c => c.CreatorId == user.Id).ToList();

                // Відображення чатів до яких корристувач має доступ --------------------------------------------
                if(user.Login != null)
                {
                    cbChatsAvailable.DataSource = null;
                    cbChatsAvailable.DisplayMember = nameof(Chat.ChatName);
                    cbChatsAvailable.ValueMember = nameof(Chat.Id);
                    var chats = context.Chats
                        .Select(ch => new {
                            ch.Id,
                            ch.ChatName,
                            Creator = ch.Creator.Login,
                            ChatUsers = ((ch.ChatUsers as List<ChatUser>)
                            .Where(c => c.ChatUserNavigation.Login.ToLower() == SystemUsers.SystemUser_All || c.ChatUserNavigation.Login.ToLower() == user.Login.ToLower())
                            .ToList()
                            .Count)
                        })
                        .Where(ch2 => ch2.ChatUsers > 0)
                        .ToList();
                    cbChatsAvailable.DataSource = chats;
                }

                // Відображення чорного списку користувача ------------------------------------------------------
                cbBlackList.DataSource = null;

                var bl = context.BlackLists
                    .Where(b => b.CreatorId == user.Id)
                    .Select(b => new { Id = b.BlackUserId, blackUserLogin = (b.BlackUser as User).Login })
                    .ToList();

                cbBlackList.DisplayMember = "blackUserLogin";
                cbBlackList.ValueMember = "Id";

                cbBlackList.DataSource = bl;
            }
        }

        private void btnCreateUser_Click(object sender, EventArgs e)
        {
            if (tbUserLogin.Text.Length > 0 && tbPassword.Text.Length > 0 && tbRepeatPassword.Text.Length > 0)
            {
                if (!tbPassword.Text.Equals(tbRepeatPassword.Text))
                {
                    MessageBox.Show("Введено різні паролі");
                    return;
                }

                if(!tbUserLogin.Text.Equals(oldUserName) && IsLoginExists())
                {
                    MessageBox.Show("Логін вже існує, спробуйте інший");
                    return;
                }

                user.Login = tbUserLogin.Text;
                user.Passvord = tbPassword.Text;
            }
            else
            {
                MessageBox.Show("Текстові поля мають бути заповнені");
            }
            this.DialogResult = DialogResult.OK;
        }

        // Перевірка наявності введеного у текстБокс логіна у БД
        private bool IsLoginExists()
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                var userName = context.Users.FirstOrDefault(u => u.Login == tbUserLogin.Text);
                return userName != null;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void FormCreateAndEditUser_Load(object sender, EventArgs e)
        {
            //
        }
    }
}

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
    public partial class FormCreateOrEditChatUser : Form
    {

        ChatUser chatUser;

        public FormCreateOrEditChatUser(ChatUser chatUser)
        {
            InitializeComponent();

            this.chatUser = chatUser;
        }

        private void FormCreateOrEditChatUser_Load(object sender, EventArgs e)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                // Заповнення текстБокс наявних чатів
                cbChatId.DataSource = null;
                var chatsAll = context.Chats.Select(c => new { Id = c.Id, ChatName = $"{c.ChatName} - creator: {c.Creator.Login}"}).ToList();

                cbChatId.DisplayMember = "ChatName";
                cbChatId.ValueMember = "Id";
                cbChatId.DataSource = chatsAll;

                // Визначення обраного за замовчуванням чату при редагуванні даних
                if(chatUser.ChatId != 0)
                {
                    int index = 0;
                    foreach (var item in chatsAll)
                    {
                        if("" + item.Id == "" + chatUser.ChatId)
                        {
                            break;
                        }
                        index++;
                    }

                    cbChatId.SelectedIndex = index;
                }

                // Заповнення текстБокс користувачів - учасник чату
                cbUserId.DataSource = null;
                cbUserId.DisplayMember = nameof(User.Login);
                cbUserId.ValueMember = nameof(User.Id);
                var usersAll = context.Users.ToList();

                cbUserId.DataSource = usersAll;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cbChatId.SelectedIndex != -1 && cbUserId.SelectedIndex != -1)
            {
                chatUser.ChatId = int.Parse("" + cbChatId.SelectedValue);
                chatUser.ChatUserId = int.Parse("" + cbUserId.SelectedValue);
            
                this.DialogResult = DialogResult.OK;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void cbChatId_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                // Заповнення текстБокс користувачів - учасник чату при зміні обраного чату
                // У списку вибору користувачів відображатимуться лише користувачі, які відсутні
                // серед учасників чату
                cbUserId.DataSource = null;
                cbUserId.DisplayMember = nameof(User.Login);
                cbUserId.ValueMember = nameof(User.Id);
                var usersAll = context.Users.ToList();
                var usersInChatLogins = context.ChatUsers
                    .Where(c => ("" + c.ChatId) == ("" + cbChatId.SelectedValue))
                    .ToList()
                    .Select(cu => new {cu.ChatUserNavigation.Login})
                    .ToList();
                List<string> usersInChatloginstList = new List<string>();
                foreach (var user in usersInChatLogins)
                {
                    usersInChatloginstList.Add(user.Login);
                }
                var usersNotInChat = usersAll.Where(u => !usersInChatloginstList.Contains(u.Login)).ToList();
                cbUserId.DataSource = usersNotInChat;
            }
        }
    }
}

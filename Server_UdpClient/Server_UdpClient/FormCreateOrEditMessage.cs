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
    public partial class FormCreateOrEditMessage : Form
    {

        ChatLibrary.Message message;

        List<User> usersAll;
        List<Chat> chatsAll;

        public FormCreateOrEditMessage(ChatLibrary.Message message)
        {
            InitializeComponent();

            this.message = message;

            usersAll = new List<User>();
            chatsAll = new List<Chat>();
        }

        private void FormCreateOrEditMessage_Load(object sender, EventArgs e)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                usersAll = context.Users.ToList();
                chatsAll = context.Chats.ToList();

                cbMessageCreator.DataSource = null;
                cbMessageCreator.DisplayMember = nameof(User.Login);
                cbMessageCreator.ValueMember = nameof(User.Id);
                cbMessageCreator.DataSource = usersAll;

                cbMessageCreator_SelectedIndexChanged(null, null);

                cbSystemInfo.DataSource = null;
                cbSystemInfo.DisplayMember = "Name";
                cbSystemInfo.ValueMember = "Name";
                cbSystemInfo.DataSource = SystemInfo.SystemInfoCollection();

                // Встановлення обраних елементів у комбобоксах
                int indexMessageCreator = 0;
                int indexSystemInfo = 0;

                // Встановлення значень відповідних візуальних елементів
                // при редагуванні повідомлення
                if(message.Id != 0)
                {
                    indexMessageCreator = usersAll.Select(x => x.Id).ToList().IndexOf(message.CreatorUserId);

                    indexSystemInfo = SystemInfo.SystemInfoCollection().IndexOf(message.SystemInfo);

                    tbMessage.Text = message.Body;

                    // Встановлення дєйтПікерів дати та часу у відповідності
                    // до дати створення повідомлення, що редагується
                    dateTimePickerCreateonDate.Value = message.SendingTime.Date;
                    dateTimePickerCreateonTime.Value = message.SendingTime.Date;
                    dateTimePickerCreateonTime.Value += message.SendingTime.TimeOfDay;
                }
                cbMessageCreator.SelectedIndex = indexMessageCreator;

                cbSystemInfo.SelectedIndex = indexSystemInfo;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(cbMessageCreator.SelectedIndex != -1 && cbChat.SelectedIndex != -1 && cbSystemInfo.SelectedIndex != -1 && tbMessage.Text.Length > 0)
            {
                message.CreatorUserId = int.Parse("" + cbMessageCreator.SelectedValue);
                message.ChatsId = int.Parse("" + cbChat.SelectedValue);

                message.SystemInfo = SystemInfo.SystemInfoCollection()[cbSystemInfo.SelectedIndex];

                // Отримання поточних дати та часу з 2 dateTimePicker та передача інф до моделі
                DateTime myDate = dateTimePickerCreateonDate.Value.Date + dateTimePickerCreateonTime.Value.TimeOfDay;
                message.SendingTime = myDate;

                message.Body = tbMessage.Text;

                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("All fields must be filled");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void cbMessageCreator_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Заповнення текстБокс чатів - при зміні обраного користувача
            // У списку вибору чатів відображатимуться лише чати, до яких має доступ 
            // вищеобраний користувач

            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                int userId = int.Parse(cbMessageCreator.SelectedValue.ToString());

                User selectedUser = usersAll.Find(u => u.Id == userId);

                var chats = context.Chats
                    .Select(ch => new {
                        ch.Id,
                        ch.ChatName,
                        Creator = ch.Creator.Login,
                        ChatUsers = ((ch.ChatUsers as List<ChatUser>)
                        .Where(c => c.ChatUserNavigation.Login.ToLower() == SystemUsers.SystemUser_All || c.ChatUserNavigation.Login.ToLower() == selectedUser.Login.ToLower())
                        .ToList()
                        .Count)
                    })
                    .Where(ch2 => ch2.ChatUsers > 0)
                    .ToList();

                cbChat.DataSource = null;
                cbChat.DisplayMember = nameof(Chat.ChatName);
                cbChat.ValueMember = nameof(Chat.Id);
                cbChat.DataSource = chats;
                
                int indexChat = 0;
                if (message.Id != 0)
                {
                    indexChat = chats.Select(x => x.Id).ToList().IndexOf(message.ChatsId);
                }
                cbChat.SelectedIndex = indexChat;
            }
        }
    }
}

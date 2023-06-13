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
using Microsoft.EntityFrameworkCore;

namespace Server_UdpClient
{
    public partial class FormAdditionalInfo : Form
    {
        List<string> usersNamesList;

        string tableName = "";
        int RegisteredUsers = -1;

        public FormAdditionalInfo(List<string> usersNamesList)
        {
            InitializeComponent();
            this.usersNamesList = usersNamesList;

            Text = "Server Additional Information";

            // Налаштування відображення інф у елементі dgvAdditionalInfo - одним суцільним рядком
            dgvAdditionalInfo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            btnUsersTable_Click(null, null);
            btnShowAll_Click(null, null);
        }

        private void btnUsersTable_Click(object sender, EventArgs e)
        {
            lbAdditionalInfo.Text = "Additional Information - table Users";
            tableName = "Users";
            // відображення інф у елементі датаГрідВюв - вписати всі колонки у вікно
            dgvAdditionalInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ActionButtonsTextChange();
            btnShowAll_Click(null, null);
        }
        private void btnChats_Click(object sender, EventArgs e)
        {
            lbAdditionalInfo.Text = "Additional Information - table Chats";
            tableName = "Chats";
            // відображення інф у елементі датаГрідВюв - вписати всі колонки у вікно
            dgvAdditionalInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ActionButtonsTextChange();
            btnShowAll_Click(null, null);
        }
        private void btnMessages_Click(object sender, EventArgs e)
        {
            lbAdditionalInfo.Text = "Additional Information - table Messages";
            tableName = "Messages";

            // відображення інф у елементі датаГрідВюв - розтягнути усі колонки (ширше вікна)
            dgvAdditionalInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            ActionButtonsTextChange();
            btnShowAll_Click(null, null);
        }
        private void btnBlackLists_Click(object sender, EventArgs e)
        {
            lbAdditionalInfo.Text = "Additional Information - table BlackLists";
            tableName = "BlackLists";
            // відображення інф у елементі датаГрідВюв - вписати всі колонки у вікно
            dgvAdditionalInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ActionButtonsTextChange();
            btnShowAll_Click(null, null);
        }

        private void btnChatUsers_Click(object sender, EventArgs e)
        {
            lbAdditionalInfo.Text = "Additional Information - table ChatUsers";
            tableName = "ChatUsers";
            // відображення інф у елементі датаГрідВюв - вписати всі колонки у вікно
            dgvAdditionalInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ActionButtonsTextChange();
            btnShowAll_Click(null, null);
        }

        private void ActionButtonsTextChange()
        {
            btnShowAll.Text = "Show" + " " + tableName;
            btnCreate.Text = "Create" + " " + tableName;
            btnUpdate.Text = "Update" + " " + tableName;
            btnDelete.Text = "Delete" + " " + tableName;
        }

        private async void btnShowAll_Click(object sender, EventArgs e)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                switch (tableName)
                {
                    case "Users":
                        {
                            await context.Users.LoadAsync();
 
                            var users = context.Users
                                 .Select(u => new { u.Id, u.Login, u.Passvord, System = u.IsSystem.ToString() == "" ? "" : SystemUsers.SystemUser_System })
                                .ToList();

                            var usersStatus = users
                                .Select(u => new { u.Id, u.Login, u.Passvord, u.System, Status = usersNamesList.Contains(u.Login) ? "ON LINE" : "off line" })
                                .ToList();

                            dgvAdditionalInfo.DataSource = null;

                            dgvAdditionalInfo.DataSource = usersStatus;

                            // Визначення загальної кількості користувачів (без системних, як server, all...)
                            var users2 = users.Where(u => !u.System.Equals(SystemUsers.SystemUser_System)).Count();

                            RegisteredUsers = users2;

                            break;
                        }
                    case "Chats":
                        {
                            await context.Chats.LoadAsync();
                            var chats = context.Chats
                                .Select(ch => new { ch.Id, ch.ChatName, Creator = ch.Creator.Login, 
                                    ChatUsers = ((ch.ChatUsers as List<ChatUser>)
                                    .Where(c => c.ChatUserNavigation.Login.ToLower() != SystemUsers.SystemUser_All)
                                    .ToList()
                                    .Count) != ch.ChatUsers.Count ? RegisteredUsers : ch.ChatUsers.Count
                                })
                                .ToList();
                            dgvAdditionalInfo.DataSource = null;
                            dgvAdditionalInfo.DataSource = chats;
                            break;
                        }
                    case "Messages":
                        {
                            await context.Messages.LoadAsync();
                            var messages = context.Messages
                                .Select(m => new { m.Id, m.CreatorUserId, m.CreatorUser.Login, m.ChatsId, m.SendingTime, m.SystemInfo, m.Body })
                                .ToList();
                            dgvAdditionalInfo.DataSource = null;
                            dgvAdditionalInfo.DataSource = messages;
                            dgvAdditionalInfo.AutoResizeColumns();
                            break;
                        }
                    case "BlackLists":
                        {
                            await context.BlackLists.LoadAsync();
                            var blackLists = context.BlackLists
                                .Select(b => new { b.Id, b.CreatorId, b.BlackUserId, b.BlackUser.Login })
                                .ToList();
                            dgvAdditionalInfo.DataSource = null;
                            dgvAdditionalInfo.DataSource = blackLists;
                            break;
                        }
                    case "ChatUsers":
                        {
                            await context.ChatUsers.LoadAsync();
                            var chatUsers = context.ChatUsers
                                .Select(cu => new { cu.Id, cu.ChatId, cu.Chat.ChatName, cu.ChatUserId, cu.ChatUserNavigation.Login })
                                .ToList();
                            dgvAdditionalInfo.DataSource = null;
                            dgvAdditionalInfo.DataSource = chatUsers;
                            break;
                        }
                }
            }
        }

        private async void btnCreate_ClickAsync(object sender, EventArgs e)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                switch (tableName)
                {
                    case "Users":
                        {
                            User user = new User();
                            FormCreateAndEditUser formCreateAndEditUser = new FormCreateAndEditUser(user);
                            if(formCreateAndEditUser.ShowDialog() == DialogResult.OK)
                            {
                                var userName = context.Users.FirstOrDefault(u => u.Login == user.Login);
                                if(userName != null)
                                {
                                    MessageBox.Show("Логін вже існує, спробуйте інший");
                                }
                                else
                                {
                                    context.Entry<User>(user).State = EntityState.Added;
                                    await context.SaveChangesAsync();
                                    btnShowAll_Click(null, null);
                                }
                            }
                            break;
                        }
                    case "Chats":
                        {
                            Chat chat = new Chat();
                            FormCreateOrEditChat formCreateOrEditChat = new FormCreateOrEditChat(chat);
                            if(formCreateOrEditChat.ShowDialog() == DialogResult.OK)
                            {
                                // Збереження у окремій колекції списку учасників чату
                                List<ChatUser> usersInChat = chat.ChatUsers.Select(c => c as ChatUser).ToList();

                                // Додавання чату
                                context.Entry<Chat>(chat).State = EntityState.Added;
                                // Додавання учасників чату
                                foreach (ChatUser item in usersInChat)
                                {
                                    item.ChatId = chat.Id;
                                    context.Entry<ChatUser>(item).State = EntityState.Added;
                                }
                                await context.SaveChangesAsync();
                                btnShowAll_Click(null, null);
                            }

                            break;
                        }
                    case "Messages":
                        {
                            ChatLibrary.Message message = new ChatLibrary.Message();
                            FormCreateOrEditMessage formCreateOrEditMessage = new FormCreateOrEditMessage(message);
                            if(formCreateOrEditMessage.ShowDialog() == DialogResult.OK)
                            {
                                context.Entry<ChatLibrary.Message>(message).State = EntityState.Added;
                                await context.SaveChangesAsync();
                                btnShowAll_Click(null, null);
                            }
                            break;
                        }
                    case "BlackLists":
                        {
                            BlackList blackList = new BlackList();
                            FormCreateOrEditBlackList formCreateOrEditBlackList = new FormCreateOrEditBlackList(blackList);
                            if(formCreateOrEditBlackList.ShowDialog() == DialogResult.OK)
                            {
                                btnShowAll_Click(null, null);
                            }
                            break;
                        }
                    case "ChatUsers":
                        {
                            ChatUser chatUser = new ChatUser();
                            FormCreateOrEditChatUser formCreateOrEditChatUser = new FormCreateOrEditChatUser(chatUser);
                            if(formCreateOrEditChatUser.ShowDialog() == DialogResult.OK)
                            {
                                context.Entry<ChatUser>(chatUser).State = EntityState.Added;
                                await context.SaveChangesAsync();
                                btnShowAll_Click(null, null);
                            }
                            break;
                        }
                }
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                switch (tableName)
                {
                    case "Users":
                        {
                            if(dgvAdditionalInfo.SelectedRows.Count > 0)
                            {
                                if (int.TryParse(dgvAdditionalInfo.SelectedRows[0].Cells[0].Value.ToString(), out int id))
                                {
                                    User? user = await context.Users.FindAsync(id);
                                    if(user != null)
                                    {
                                        FormCreateAndEditUser form = new FormCreateAndEditUser(user);
                                        if(form.ShowDialog() == DialogResult.OK)
                                        {
                                                context.Entry<User>(user).State = EntityState.Modified;
                                                await context.SaveChangesAsync();
                                                btnShowAll_Click(null, null);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "Chats":
                        {
                            if (dgvAdditionalInfo.SelectedRows.Count > 0)
                            {
                                if (int.TryParse(dgvAdditionalInfo.SelectedRows[0].Cells[0].Value.ToString(), out int id))
                                {
                                    Chat? chat = await context.Chats.FindAsync(id);
                                    if (chat != null)
                                    {
                                        FormCreateOrEditChat form = new FormCreateOrEditChat(chat);
                                        if (form.ShowDialog() == DialogResult.OK)
                                        {

                                            // Видалення користувачів чату
                                            List<ChatUser> usersInChatOld  = context.ChatUsers.Where(c => c.ChatId == chat.Id).ToList();
                                            foreach (ChatUser user in usersInChatOld)
                                            {
                                                context.Entry<ChatUser>(user).State = EntityState.Deleted;
                                            }
                                            await context.SaveChangesAsync();

                                            // Додавання чату
                                            context.Entry<Chat>(chat).State = EntityState.Modified;
                                            await context.SaveChangesAsync();
                                            btnShowAll_Click(null, null);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "Messages":
                        {
                            if (dgvAdditionalInfo.SelectedRows.Count > 0)
                            {
                                if (int.TryParse(dgvAdditionalInfo.SelectedRows[0].Cells[0].Value.ToString(), out int id))
                                {
                                    ChatLibrary.Message? message = await context.Messages.FindAsync(id);
                                    if (message != null)
                                    {
                                        FormCreateOrEditMessage form = new FormCreateOrEditMessage(message);
                                        if (form.ShowDialog() == DialogResult.OK)
                                        {
                                            context.Entry<ChatLibrary.Message>(message).State = EntityState.Modified;
                                            await context.SaveChangesAsync();
                                            btnShowAll_Click(null, null);
                                        }
                                    }
                                }
                            }

                            break;
                        }
                    case "BlackLists":
                        {
                            // 
                            if (dgvAdditionalInfo.SelectedRows.Count > 0)
                            {
                                if (int.TryParse(dgvAdditionalInfo.SelectedRows[0].Cells[0].Value.ToString(), out int id))
                                {
                                    BlackList? blackList = await context.BlackLists.FindAsync(id);
                                    if (blackList != null)
                                    {
                                        FormCreateOrEditBlackList form = new FormCreateOrEditBlackList(blackList);
                                        if (form.ShowDialog() == DialogResult.OK)
                                        {
                                            btnShowAll_Click(null, null);
                                        }
                                    }
                                }
                            }

                            break;
                        }
                    case "ChatUsers":
                        {
                            // 
                            if (dgvAdditionalInfo.SelectedRows.Count > 0)
                            {
                                if (int.TryParse(dgvAdditionalInfo.SelectedRows[0].Cells[0].Value.ToString(), out int id))
                                {
                                    ChatUser? chatUser = await context.ChatUsers.FindAsync(id);
                                    if (chatUser != null)
                                    {
                                        FormCreateOrEditChatUser form = new FormCreateOrEditChatUser(chatUser);
                                        if (form.ShowDialog() == DialogResult.OK)
                                        {
                                            context.Entry<ChatUser>(chatUser).State = EntityState.Modified;
                                            await context.SaveChangesAsync();
                                            btnShowAll_Click(null, null);
                                        }
                                    }
                                }
                            }

                            break;
                        }
                }
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                switch (tableName)
                {
                    case "Users":
                        {
                            if (dgvAdditionalInfo.SelectedRows.Count > 0)
                            {
                                if (int.TryParse(dgvAdditionalInfo.SelectedRows[0].Cells[0].Value.ToString(), out int id))
                                {
                                    User? user = await context.Users.FindAsync(id);
                                    if (user != null)
                                    {
                                        var result = MessageBox.Show($"Бажаєте видалити користувача {user.Login} ?", "Delete user", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                                        if(result == DialogResult.OK)
                                        {
                                            context.Entry<User>(user).State = EntityState.Deleted;
                                            await context.SaveChangesAsync();
                                            btnShowAll_Click(null, null);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "Chats":
                        {
                            // 
                            if (dgvAdditionalInfo.SelectedRows.Count > 0)
                            {
                                if (int.TryParse(dgvAdditionalInfo.SelectedRows[0].Cells[0].Value.ToString(), out int id))
                                {
                                    Chat? chat = await context.Chats.FindAsync(id);
                                    if (chat != null)
                                    {
                                        var result = MessageBox.Show($"Бажаєте видалити чат {chat.ChatName} ?", "Delete chat", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                                        if (result == DialogResult.OK)
                                        {
                                            context.Entry<Chat>(chat).State = EntityState.Deleted;
                                            await context.SaveChangesAsync();
                                            btnShowAll_Click(null, null);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "Messages":
                        {
                            // 
                            if (dgvAdditionalInfo.SelectedRows.Count > 0)
                            {
                                if (int.TryParse(dgvAdditionalInfo.SelectedRows[0].Cells[0].Value.ToString(), out int id))
                                {
                                    ChatLibrary.Message? message = await context.Messages.FindAsync(id);
                                    if (message != null)
                                    {
                                        var result = MessageBox.Show($"Бажаєте видалити повідомленння {message.Id} ?", "Delete message", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                                        if (result == DialogResult.OK)
                                        {
                                            context.Entry<ChatLibrary.Message>(message).State = EntityState.Deleted;
                                            await context.SaveChangesAsync();
                                            btnShowAll_Click(null, null);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "BlackLists":
                        {
                            // 
                            if (dgvAdditionalInfo.SelectedRows.Count > 0)
                            {
                                if (int.TryParse(dgvAdditionalInfo.SelectedRows[0].Cells[0].Value.ToString(), out int id))
                                {
                                    BlackList? blackList = await context.BlackLists.FindAsync(id);
                                    if (blackList != null)
                                    {
                                        var result = MessageBox.Show($"Бажаєте видалити чорний список {blackList.Id} ?", "Delete BlackList", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                                        if (result == DialogResult.OK)
                                        {
                                            context.Entry<BlackList>(blackList).State = EntityState.Deleted;
                                            await context.SaveChangesAsync();
                                            btnShowAll_Click(null, null);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case "ChatUsers":
                        {
                            // 
                            if (dgvAdditionalInfo.SelectedRows.Count > 0)
                            {
                                if (int.TryParse(dgvAdditionalInfo.SelectedRows[0].Cells[0].Value.ToString(), out int id))
                                {
                                    ChatUser? chatUser = await context.ChatUsers.FindAsync(id);
                                    Chat? chat = await context.Chats.FindAsync(chatUser.ChatId);
                                    if (chatUser != null)
                                    {
                                        var result = MessageBox.Show($"Бажаєте видалити участика з чату {chat.ChatName} ?", 
                                            "Delete user from chat", 
                                            MessageBoxButtons.OKCancel, 
                                            MessageBoxIcon.Question);
                                        if (result == DialogResult.OK)
                                        {
                                            context.Entry<ChatUser>(chatUser).State = EntityState.Deleted;
                                            await context.SaveChangesAsync();
                                            btnShowAll_Click(null, null);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                }
            }
        }
    }
}

// 02 03
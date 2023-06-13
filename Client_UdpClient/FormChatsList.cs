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
    public partial class FormChatsList : Form
    {
        List<ModelChatsNameAndId> usersChatList;
        List<ModelUsersLoginAndId> usersList;
        int userId;
        string userLogin;
        int chatIdTemp = -1;

        public FormChatsList(List<ModelChatsNameAndId> usersChatList, List<ModelUsersLoginAndId> usersList, int userId, string userLogin)
        {
            InitializeComponent();

            this.usersChatList = usersChatList;
            this.usersList = usersList;
            this.userId = userId;
            this.userLogin = userLogin;

            // Налаштування відображення інф у елементі dgvChatsList - одним суцільним рядком
            dgvChatsList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // відображення інф у елементі датаГрідВюв - вписати всі колонки у вікно
            dgvChatsList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void FormChatsList_Load(object sender, EventArgs e)
        {
            UpdateChatsList();
        }

        private void UpdateChatsList()
        {
            dgvChatsList.DataSource = null;
            dgvChatsList.DataSource = usersChatList
                .Select(c => new {
                    Id = c.Id, 
                    ChatName = c.ChatName, 
                    Creator = new String(c.ChatCreatorId == userId ? SystemInfo.SystemInfo_Creator_creator: "" ), 
                    UsersInChat = c.AmountUsersInChat
                })
                .ToList();
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnCreateChat_Click(object sender, EventArgs e)
        {
            ModelChatsNameAndId chat = new ModelChatsNameAndId();
            chat.ChatCreatorId = userId;
            chat.Id = chatIdTemp--;
            FormCreateOrEditChat_OnClientsSide formCreateOrEditChat = new FormCreateOrEditChat_OnClientsSide(chat, usersList, userId);
            if (formCreateOrEditChat.ShowDialog() == DialogResult.OK)
            {
                usersChatList.Add(chat);

                UpdateChatsList();
            }
        }

        private void btnEditeChat_Click(object sender, EventArgs e)
        {
            if (dgvChatsList.SelectedRows.Count > 0)
            {
                if (int.TryParse(dgvChatsList.SelectedRows[0].Cells[0].Value.ToString(), out int chatId))
                {
                    ModelChatsNameAndId chat = usersChatList.Find(c => c.Id == chatId);
                    if(chat.ChatCreatorId != userId)
                    {
                        MessageBox.Show(SystemInfo.SystemInfo_MessageUserCanEditOnlyChatsCreatedByHimself);
                    }
                    int chatPos = 0;
                    if (chat != null)
                    {
                        chatPos = usersChatList.IndexOf(chat);
                        FormCreateOrEditChat_OnClientsSide form = new FormCreateOrEditChat_OnClientsSide(chat, usersList, userId);
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            usersChatList[chatPos] = chat;
                            UpdateChatsList();
                        }
                    }
                }
            }

        }

        private void btnDeleteChat_Click(object sender, EventArgs e)
        {
            if (dgvChatsList.SelectedRows.Count > 0)
            {
                if (int.TryParse(dgvChatsList.SelectedRows[0].Cells[0].Value.ToString(), out int chatId))
                {
                    ModelChatsNameAndId chat = usersChatList.Find(c => c.Id == chatId);
                    if (chat.ChatCreatorId != userId)
                    {
                        MessageBox.Show(SystemInfo.SystemInfo_MessageUserCanEditOnlyChatsCreatedByHimself);
                    }
                    else
                    {
                        int chatPos = 0;
                        if (chat != null)
                        {
                            chatPos = usersChatList.IndexOf(chat);
                            var result = MessageBox.Show(SystemInfo.SystemInfo_MessageDoYouWantToDeleteTheChat + usersChatList[chatPos].ChatName, 
                                SystemInfo.SystemInfo_MessageDeleteChat, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (result == DialogResult.OK)
                            {
                                usersChatList.RemoveAt(chatPos);
                                UpdateChatsList();
                            }
                        }
                    }
                }
            }
        }
    }
}

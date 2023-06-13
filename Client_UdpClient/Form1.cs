using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChatLibrary;
using Microsoft.EntityFrameworkCore;

namespace Client_UdpClient
{
    public partial class Form1 : Form
    {
        int remotePort = 4569; // or 11000
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        // ����� ��� ��������� �� �������
        Socket listeningSocket = null;

        ItemCreator? creator;

        UserForSending? user;

        // ����� ��� ��������� ������� ���������� ��䳿 �� ���� ����� cbChatsList
        // - ��� ����������� ����� ��������� 2 ����, ��� ����� ��������������� ���
        // ������� ��������� �������������� ����
        bool isCbChatsListInitComplete = false;

        // ����� ��� ���������� ������� ��������� ����
        // - ��������������� ��� ������� �� ���������� ����������� ������ (cbChatsList_SelectedIndexChanged)
        // ������������ ��� ����������� ����
        // - ��� ��������� � ��������� ����� ������������� �� ����� ������ ����
        int selectedChatIndexInCB = 0;

        public Form1()
        {
            InitializeComponent();

            // ������� ������������ ����������
            btnSendMessage.Enabled = false;
            tbUserMessage.Enabled = false;
            cbChatsList.Enabled = false;
            tbUserName.Text = "USER";
            menuStrip1.Enabled = false;
            tbUserName.Select();

            InitData();
        }

        private void InitData()
        {
            creator = new ItemCreator();
            user = new UserForSending();

        }

        // ���������� �� ������ ��������/³�'�������
        private async void btnStartChat_Click(object sender, EventArgs e)
        {
            if (listeningSocket != null)
            {
                // ³��������� �� �������
                Form1_FormClosing(sender, null);

                // ������������ ���������� - ³�'������� �����
                btnStartChat.Text = "Start Chat";
                tbUserName.Enabled = true;
                tbUserMessage.Text = "";
                tbUserMessage.Enabled = false;
                btnSendMessage.Enabled = false;
            }
            else
            {
                // ϳ��������� �� �������
                try
                {
                    // ���� ���� ��� �������� ����� ����������� ����� - ����� �������� ������
                    if (tbUserName.Text.Trim().Length == 0)
                    {
                        MessageBox.Show(SystemInfo.SystemInfo_MessageNeedUserLogin); // ������� ������ ���� �����������
                        return;
                    }

                    // ����� �����������/���������

                    // ��������� ��'����-����� ��� �������� ����� �� ������� (�������� �� ���������)
                    ParaLoginPass paraLoginPass = new ParaLoginPass(tbUserName.Text, "");
                    // ��������� ��� ������������ ����� � ������ ����������� / ���������
                    bool isAuthorization = true;

                    // ���� ��������� / ����������� - ��������� ���������� ����� �����/������
                    bool isTryAgain = false; // ��������� ������� ����� �� �������� ����������� / ���������
                    DialogResult isNextTry; // ��������� ��������� ���������� ���� �� ���������� ��� �������� ������ ����������� / ���������
                    do
                    {
                        isTryAgain = false;

                        // ��������� / ������ ����� ���������
                        FormAuthorizationRegistration form = new FormAuthorizationRegistration(paraLoginPass, isAuthorization);
                        // ��������� ����������� ��� ����������� ������� ����� ��� ���������/�����������
                        MessageForSending clientMessageAuthorizeRegister = null;
                        var result = form.ShowDialog();
                        // ³���������� �� ���� ����� �����������, ��������� � ��� �����������/���������
                        tbUserName.Text = paraLoginPass.Login;
                        if (result == DialogResult.OK)
                        {
                            // �����������
                            clientMessageAuthorizeRegister = creator.CreateMessageForSending(
                                SystemInfo.SystemInfo_EmptyMessageBody,
                                SystemInfo.SystemInfo_AuthorizationUser, 0, 0);
                        }
                        else if (result == DialogResult.Yes)
                        {
                            // ���������
                            clientMessageAuthorizeRegister = creator.CreateMessageForSending(
                                SystemInfo.SystemInfo_EmptyMessageBody,
                                SystemInfo.SystemInfo_RegistrationUser, 0, 0);
                        }
                        else
                        {
                            return;
                        }
                        clientMessageAuthorizeRegister.CreatorUserLogin = paraLoginPass.Login;
                        clientMessageAuthorizeRegister.HashBody = paraLoginPass.Pass;

                        // ����������� ������
                        listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                        // �������� ��������� ������ �����������
                        // (�볺�� ��������� ����� �� ���������� ��������� ������������ � �������� ���� �����)
                        string messageFromServerAboutAuthorizeRegisterJson = ""; // ������� ������� �� ����� �볺���
                                                                                 //string message = $"#client-newUser#####{tbUserName.Text}";
                        string message = ""; // ����� �볺���

                        // ��������� �����������
                        // ���������� �����������
                        message = JsonSerializer.Serialize<MessageForSending>(clientMessageAuthorizeRegister);

                        // ��������� � �������� �����
                        byte[] data = Encoding.Default.GetBytes(message);
                        EndPoint remoteEndpoint = new IPEndPoint(ipAddress, remotePort);
                        // ³������� ������
                        listeningSocket.SendTo(data, remoteEndpoint);

                        // ��������� �����������-������ �� �������
                        messageFromServerAboutAuthorizeRegisterJson = MessageReceiver();

                        // ������������ ����������� � ��'��� ���� MessageForSending
                        MessageForSending messageForSendingFormatReceived = JsonSerializer.Deserialize<MessageForSending>(messageFromServerAboutAuthorizeRegisterJson);

                        // �������� ����������� ������ ������ � ���, ����
                        // - �� ������ ��� ������������� ����������, ���� �������� �� ��������� �볺�� ��� ��������� (���� ��� ����)
                        // - ������� ���� ��� ������ (��� �����������)
                        if (messageForSendingFormatReceived.SystemInfo.Equals(SystemInfo.SystemInfo_AuthorizUserFalse) ||
                            messageForSendingFormatReceived.SystemInfo.Equals(SystemInfo.SystemInfo_RegistrationUserFalse))
                        {
                            // ����� ����������� ��� ����������� ��� ��������� �������
                            string messageToUserAboutAuthorizeOrRegisterError =
                                messageForSendingFormatReceived.SystemInfo.Equals(SystemInfo.SystemInfo_AuthorizUserFalse) ?
                                SystemInfo.SystemInfo_MessageIncorrectLoginOrPassword :
                                SystemInfo.SystemInfo_MessageLoginAlreadyExists;
                            // ��������� ����������� ��� ������� � ���������� ��� � ������� ���������� / ���������
                            isNextTry = MessageBox.Show(messageToUserAboutAuthorizeOrRegisterError +
                                " " +
                                SystemInfo.SystemInfo_MessageWantTryAgain, "Authorize/Register", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (isNextTry != DialogResult.OK)
                            {
                                return;
                            }
                            else
                            {
                                isTryAgain = true;
                            }
                        }
                        else if (messageForSendingFormatReceived.SystemInfo.Equals(SystemInfo.SystemInfo_AuthorizUserTrue))
                        {
                            // ����������� ����� UserForSending user
                            // - ������ �������� ��� ��� ���� ������ ����������� �� ������ ���������� �� �������
                            // ��������� ������������� ��'���� UserForSending
                            string userForSendingJson = messageForSendingFormatReceived.SystemBody;

                            // ������������ ��'���� UserForSending �� ����������� ����� ���������� user
                            user = JsonSerializer.Deserialize<UserForSending>(userForSendingJson);

                            InitComboBoxChatList();
                        }
                        else if (messageForSendingFormatReceived.SystemInfo.Equals(SystemInfo.SystemInfo_RegistrationUserFalse))
                        {
                            // ��������� ����������� ��� ������� ��������� � ���������� ��� � ������� ���������� / ���������
                            isNextTry = MessageBox.Show(SystemInfo.SystemInfo_MessageLoginAlreadyInUse +
                                " " +
                                SystemInfo.SystemInfo_MessageWantTryAgain, "Authorize/Register", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                            if (isNextTry != DialogResult.OK)
                            {
                                return;
                            }
                            else
                            {
                                isTryAgain = true;
                            }
                        }
                        else if (messageForSendingFormatReceived.SystemInfo.Equals(SystemInfo.SystemInfo_RegistrationUserTrue))
                        {
                            // ����� ��������� - ��������������� �� �����������
                            MessageBox.Show(SystemInfo.SystemInfo_MessageLogInToEnterTheChat,
                                "Authorize/Register", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            isTryAgain = true;
                        }
                    } while (isTryAgain);


                    //������ ��������� ��������������� ���������� �� ������� -������ ����
                    Task.Run(() =>
                    {
                        Listen();
                    });

                    // ������������ ���������� - ��������� �����
                    btnStartChat.Text = "Disconnect";
                    tbUserName.Enabled = false;
                    tbUserMessage.Enabled = true;
                    cbChatsList.Enabled = true;
                    menuStrip1.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void InitComboBoxChatList()
        {
            isCbChatsListInitComplete = false;

            cbChatsList.DataSource = null;
            cbChatsList.DisplayMember = "Name";
            cbChatsList.ValueMember = nameof(ModelChatsNameAndId.Id);
            var chats = user.ChatsList
                .Select(c => new { Id = c.Id, Name = new String((c.ChatCreatorId == user.UserId ? SystemInfo.SystemInfo_Creator_Star + "  " : "   ") + c.ChatName) })
                .ToList();
            int index = 0, i = 0;

            foreach (var chat in chats)
            {
                if (chat.Name.Contains(user.CurrentChatName))
                {
                    index = i;
                }
                i++;
            }

            cbChatsList.DataSource = chats;
            cbChatsList.SelectedIndex = index;

            selectedChatIndexInCB = int.Parse("" + cbChatsList.SelectedValue);

            isCbChatsListInitComplete = true;
        }


        // ��������������� ����������� �� �������� ����������
        private void Listen()
        {
            try
            {
                // ���� ���������������
                while (true)
                {
                    // ��������� ����������� - MessageForSending � ������������� ������
                    string messageReceived = MessageReceiver();

                    // ������������ ���������� �����������
                    MessageForSending messageForSendingReceived = JsonSerializer.Deserialize<MessageForSending>(messageReceived);
                    // ����� - ���� SystemInfo ������ ���, �� ���� ����������� user-users, server-users, server-user - �����
                    string message = "";
                    if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UserMessage) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UserDisconnect) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_ServerMessage)
                        ))
                    {
                        // ��������� ������ �����������
                        message = messageForSendingReceived.GetFullMessageLong();
                    }
                    // �������� ���������� �� ������ ����
                    else if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_ChatIsConnected) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_ChatNotConnected)
                        ))
                    {
                        if (messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_ChatNotConnected))
                        {
                            // ��������� ����������� ��� ������� ���������� �� ����
                            MessageBox.Show(SystemInfo.SystemInfo_MessageFailedToConnectToChat, "Chat connecting error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            // ����������� ����� UserForSending user
                            // - ������ �������� ��� ��� ���� ������ ����������� �� ������ ���������� �� �������
                            // ��������� ������������� ��'���� UserForSending
                            string userForSendingJson = messageForSendingReceived.SystemBody;

                            // ������������ ��'���� UserForSending �� ����������� ����� ���������� user
                            user = JsonSerializer.Deserialize<UserForSending>(userForSendingJson);

                            // ����������� ������ ��������� ���� � ��������� ��������� ����
                            InitComboBoxChatList();

                            // ������� ���������� ���������� - ���� ���������� ����
                            tbChatMessages.BeginInvoke(new Action<string>(TbChatMessagesUpdate), "");
                        }
                    }
                    // ������������ ����� ����
                    else if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_NewChatHistoryStart) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_NewChatHistoryContinue)
                        ))
                    {
                        // ��������� ����� ���� � ������ �����
                        string chatHistory = messageForSendingReceived.SystemBody;

                        // ��������� ���������� ���������� � ������������ ����� ����
                        if (messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_NewChatHistoryStart))
                        {
                            tbChatMessages.BeginInvoke(new Action<string>(TbChatMessagesUpdate), chatHistory);
                        }
                        else
                        {
                            tbChatMessages.BeginInvoke(new Action<string>(AddTextToTbChatMessages), chatHistory);
                        }
                    }
                    // ������������ ��������� ��� ��� �������� ��� �� ��������� ������ ������������ ����
                    else if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UserAllInfoIsUpToDate) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_SendingTheCurrentUserAllInfo)
                        ))
                    {
                        // ��������� ��������� ��� ��� ��������� �����������
                        if (messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_SendingTheCurrentUserAllInfo))
                        {
                            // ����������� ����� UserForSending user
                            // - ������ �������� ��� ��� ���� ������ ����������� �� ������ ���������� �� �������
                            // ��������� ������������� ��'���� UserForSending
                            string userForSendingJson = messageForSendingReceived.SystemBody;

                            // ������������ ��'���� UserForSending �� ����������� ����� ���������� user
                            user = JsonSerializer.Deserialize<UserForSending>(userForSendingJson);

                            // ����������� ��������� �������� - ���������� ����
                            InitComboBoxChatList();
                        }

                        // ������������ ����� ��� ��������� ������ ������������ ���� - ���������� �� Listen
                        int userCreator = user.ChatsList.Find(c => c.Id == user.CurrentChatId).ChatCreatorId;

                        List<ModelUsersLoginAndId> usersList = user.ChatsList.Find(c => c.Id == user.CurrentChatId).UsersInChat;
                        FormUsersList form = new FormUsersList(usersList, userCreator);
                        form.ShowDialog();
                    }
                    // ������������ ��������� ��� ��� �볺��� �� ��������� ��������� ������� ������ �����������
                    else if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UserAllInfoIsUpToDateForBlackList) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForBlackList)
                        ))
                    {
                        // ��������� ��������� ��� ��� ��������� �����������
                        if (messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForBlackList))
                        {
                            // ����������� ����� UserForSending user
                            // - ������ �������� ��� ��� ���� ������ ����������� �� ������ ���������� �� �������
                            // ��������� ������������� ��'���� UserForSending
                            string userForSendingJson = messageForSendingReceived.SystemBody;

                            // ������������ ��'���� UserForSending �� ����������� ����� ���������� user
                            user = JsonSerializer.Deserialize<UserForSending>(userForSendingJson);

                            // ����������� ��������� �������� - ���������� ����
                            InitComboBoxChatList();
                        }

                        // ������������ ����� ��������� ������� ������ 
                        List<ModelUsersLoginAndId> usersBlackList = user.BlackList;
                        List<ModelUsersLoginAndId> usersList = user.ChatsList.Find(c => c.ChatName == SystemInfo.SystemInfo_GroupChat).UsersInChat;

                        FormBlackListEditor form = new FormBlackListEditor(usersBlackList, usersList, user.UserId);
                        if(form.ShowDialog() == DialogResult.OK)
                        {
                            // ���������� ����������� �� ��������� ������� ������ ����������� ... 
                            // ��������� ����������� ��� �������� ������� MessageForSending
                            MessageForSending messageForSendingBlackListUpdate = new MessageForSending(
                                SystemInfo.SystemInfo_EmptyMessageBody, 
                                SystemInfo.SystemInfo_UpdateUserBlackListInDb,
                                user.UserId, user.UserLogin, user.CurrentChatId, user.CurrentChatName);
                            
                            // �������� ��������� SystemBody �� HashBody
                            // ���������� �������� ������ ������ ��� �������� �� ������ - ��� ���������� ���
                            string usersBlackListJson = JsonSerializer.Serialize<List<ModelUsersLoginAndId>>(usersBlackList);

                            // �������� ����������� �������� ������ ������ �� ����������� 
                            messageForSendingBlackListUpdate.SystemBody = usersBlackListJson;

                            // ³������� �����������
                            MessageSender(messageForSendingBlackListUpdate);
                        }
                    }
                    // ������������ ��������� ��� ��� �볺��� �� ��������� ���-��������� �����������
                    else if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UserAllInfoIsUpToDateForChatList) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForChatList)
                        ))
                    {
                        // ��������� ��������� ��� ��� ��������� �����������
                        if (messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForChatList))
                        {
                            // ����������� ����� UserForSending user
                            // - ������ �������� ��� ��� ���� ������ ����������� �� ������ ���������� �� �������
                            // ��������� ������������� ��'���� UserForSending
                            string userForSendingJson = messageForSendingReceived.SystemBody;

                            // ������������ ��'���� UserForSending �� ����������� ����� ���������� user
                            user = JsonSerializer.Deserialize<UserForSending>(userForSendingJson);

                            // ����������� ��������� �������� - ���������� ����
                            InitComboBoxChatList();
                        }

                        // ������������ ����� ���-���������
                        List<ModelChatsNameAndId> usersChatList = user.ChatsList;
                        List<ModelUsersLoginAndId> usersList = user.ChatsList.Find(c => c.ChatName == SystemInfo.SystemInfo_GroupChat).UsersInChat;

                        FormChatsList form = new FormChatsList(usersChatList, usersList, user.UserId, user.UserLogin);
                        if(form.ShowDialog() == DialogResult.OK)
                        {
                            // ���������� ����������� �� ��������� ������ ����  
                            // ��������� ����������� ��� �������� ������� MessageForSending
                            MessageForSending messageForSendingChatListUpdate = new MessageForSending(
                                SystemInfo.SystemInfo_EmptyMessageBody, 
                                SystemInfo.SystemInfo_UpdateUserChatListInDb,
                                user.UserId, user.UserLogin, user.CurrentChatId, user.CurrentChatName);
                            
                            // �������� ��������� SystemBody �� HashBody
                            // ���������� �������� ������ ���� ��� �������� �� ������ - ��� ���������� ���
                            string usersChatListJson = JsonSerializer.Serialize<List<ModelChatsNameAndId>>(usersChatList);

                            // �������� ����������� �������� ������ ���� �� ����������� 
                            messageForSendingChatListUpdate.SystemBody = usersChatListJson;

                            // ³������� �����������
                            MessageSender(messageForSendingChatListUpdate);
                        }

                    }
                    // ������� � ��� ��������� ����������� ��� �������������� �� ���������� ��������� ���� ���� ��������� ��������� ���� ���
                    // ��������� � ����� ������������ ��������� ����
                    else if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_DeletedCurrentChat) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_RemovedFromCurrentChatUsers)
                        ))
                    {
                        // ��������� ������������� ��'���� UserForSending
                        string userForSendingJson = messageForSendingReceived.SystemBody;

                        // ���������� ����������� ��� �������������� �� ����������� ��������� ����
                        MessageBox.Show(messageForSendingReceived.GetFullMessageLong());
                        messageForSendingReceived.Body = "";

                        // ������������ ��'���� UserForSending �� ����������� ����� ���������� user
                        user = JsonSerializer.Deserialize<UserForSending>(userForSendingJson);

                        // ����������� ������ ��������� ���� � ��������� ��������� ����
                        InitComboBoxChatList();

                        // ������� ���������� ���������� - ���� ���������� ����
                        tbChatMessages.BeginInvoke(new Action<string>(TbChatMessagesUpdate), "");
                    }
                    // ��������� ����� ��� �볺��� - �� ��������� ����� ��
                    else if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UpdatedUserInformation)
                        ))
                    {
                        // ����������� ����� UserForSending user
                        // - ������ �������� ��� ��� ���� ������ ����������� �� ������ ���������� �� �������
                        // ��������� ������������� ��'���� UserForSending
                        string userForSendingJson = messageForSendingReceived.SystemBody;

                        // ������������ ��'���� UserForSending �� ����������� ����� ���������� user
                        user = JsonSerializer.Deserialize<UserForSending>(userForSendingJson);

                        // ����������� ��������� �������� - ���������� ����
                        InitComboBoxChatList();
                    }

                    // ��������� ����������� � ��������� ���������
                    if (message.Length > 0)
                    {
                        tbChatMessages.BeginInvoke(new Action<string>(AddTextToTbChatMessages), message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " - " + user.UserLogin); ;
            }
            finally
            {
                SocketClose();
            }
        }

        // �������� ��������� ����������
        private string MessageReceiver()
        {
            StringBuilder builder = new StringBuilder();
            int len = 0;
            byte[] data = new byte[8193024]; // 1024, 4096, 8192, 16384, 1024000, 1048576, 8192000, 8193024, 8388608
            EndPoint remoteIP = new IPEndPoint(IPAddress.Any, 0);
            do
            {
                len = listeningSocket.ReceiveFrom(data, ref remoteIP);
                builder.AppendLine(Encoding.Default.GetString(data, 0, len));
            } while (listeningSocket.Available > 0);

            return builder.ToString();
        }

        // ³���������� ����������� � ���������� ���������
        private void AddTextToTbChatMessages(string str)
        {
            StringBuilder builder = new StringBuilder(tbChatMessages.Text);
            builder.Append(str.Trim());
            builder.AppendLine(SystemInfo.SystemInfo_Separator);
            tbChatMessages.Text = builder.ToString();
            tbChatMessages.SelectionStart = tbChatMessages.Text.Length; // ���������� ������� � ����� ������
            tbChatMessages.ScrollToCaret(); // ������� ���� �� ������� ������� (� ����� ��� �� ���� ������)

            // ������� ������ �� ��������� ������� - ������� � ���� �������� ������ �����������
            tbUserMessage.Select();
        }

        // ����� �������� ���������� ����������
        private void TbChatMessagesUpdate(string str)
        {
            tbChatMessages.Text = str;
            tbChatMessages.SelectionStart = tbChatMessages.Text.Length; // ���������� ������� � ����� ������
            tbChatMessages.ScrollToCaret(); // ������� ���� �� ������� ������� (� ����� ��� �� ���� ������)

            // ������� ������ �� ��������� ������� - ������� � ���� �������� ������ �����������
            tbUserMessage.Select();
        }


        // �������� ��������� �� ����������� ���������� �� �볺��� �� �������
        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            // ��������� ����������� ��� �������� �볺��� 
            MessageForSending messageFromClientToClients = new MessageForSending(
                tbUserMessage.Text + SystemInfo.SystemInfo_Separator, SystemInfo.SystemInfo_UserMessage, user.UserId, user.UserLogin, user.CurrentChatId, user.CurrentChatName);

            MessageSender(messageFromClientToClients);

            tbUserMessage.Clear();
        }

        // ³���������� ����������
        private void MessageSender(MessageForSending message)
        {
            // ���������� ����������� 
            string messageFromClientToServerJson = JsonSerializer.Serialize<MessageForSending>(message);

            // ³������� ����������� �� ������ �������
            byte[] data = Encoding.Default.GetBytes(messageFromClientToServerJson);
            EndPoint remoteEndpoint = new IPEndPoint(ipAddress, remotePort);
            listeningSocket.SendTo(data, remoteEndpoint);
        }

        // �������� �������� �'������� �� �������� �� ������ ����������� ��� ����� �볺��� � ���� 
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ���������� ��������� ������ � �������, ���� �볺�� �� ������� �� ����
            if (listeningSocket == null)
            {
                return;
            }

            // ��������� ����������� (MessageForSending) �볺��-������ ��� ���������� �볺���
            MessageForSending messageForSendingUserDisconnect = creator.CreateMessageForSending(
                SystemInfo.SystemInfo_EmptyMessageBody,
                SystemInfo.SystemInfo_UserDisconnect,
                user.UserId,
                user.CurrentChatId);

            // ���������� �����������
            string messageForSendingUserDisconnectJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingUserDisconnect);

            // ³������� ����������� ��� ���������� �볺���
            byte[] data = Encoding.Default.GetBytes(messageForSendingUserDisconnectJson);
            EndPoint remoteEndpoint = new IPEndPoint(ipAddress, remotePort);
            listeningSocket.SendTo(data, remoteEndpoint);

            // �������� �'�������
            SocketClose();
        }

        // ���� ���������� - ������ �������� ���������� � ���������
        // �� ����������� ���������� ��� �������� ����������
        private void tbUserMessage_TextChanged(object sender, EventArgs e)
        {
            if (tbUserMessage.Text.Length == 0)
            {
                btnSendMessage.Enabled = false;
            }
            else
            {
                btnSendMessage.Enabled = true;
            }
        }

        // �������� �'�������
        private void SocketClose()
        {
            if (listeningSocket != null)
            {
                listeningSocket.Shutdown(SocketShutdown.Both);
                listeningSocket.Close();
                listeningSocket = null;
            }
        }

        private void cbChatsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ������ �� ����������� ��䳿 �� ���������� ������� ������������ ���������� cbChatsList,
            // � ����� �� ������� ������ ��� ���������� ��������� �� ��� ������������ ������� (���, �� �����
            // ���������� �� ����� ������ ����������)
            if (isCbChatsListInitComplete && (selectedChatIndexInCB != int.Parse("" + cbChatsList.SelectedValue)))
            {
                selectedChatIndexInCB = int.Parse("" + cbChatsList.SelectedValue);

                // ��������� ����������� ��� �������� ������� MessageForSending
                MessageForSending messageForSendingForChangeChat = creator.CreateMessageForSending(
                    SystemInfo.SystemInfo_EmptyMessageBody,
                    SystemInfo.SystemInfo_ConnectChat, user.UserId, int.Parse("" + cbChatsList.SelectedValue));

                // ������� - �������� ��������� SystemBody �� HashBody
                // ���������� �����������

                // ³������� �����������
                MessageSender(messageForSendingForChangeChat);
            }

        }

        private void for1DayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime startDate = DateTime.Today;
            DownloadChatHistory(startDate);
        }

        private void for1WeekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime startDate = DateTime.Today.AddDays(-7);
            DownloadChatHistory(startDate);
        }

        private void for1MonthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime startDate = DateTime.Today.AddDays(-31);
            DownloadChatHistory(startDate);
        }

        private void for3MonthsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime startDate = DateTime.Today.AddDays(-93);
            DownloadChatHistory(startDate);
        }

        private void yearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime startDate = DateTime.Today.AddDays(-366);
            DownloadChatHistory(startDate);
        }

        private void allToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DateTime startDate = new DateTime(2000, 1, 1);
            DownloadChatHistory(startDate);
        }

        private void DownloadChatHistory(DateTime date)
        {
            // �������� ���������� ����������
            tbChatMessages.BeginInvoke(new Action<string>(TbChatMessagesUpdate), "");

            // ��������� ����������� ��� �������� ������� MessageForSending
            MessageForSending messageGettingHistory = new MessageForSending(
                SystemInfo.SystemInfo_EmptyMessageBody,
                SystemInfo.SystemInfo_GetChatHistory,
                user.UserId, user.UserLogin, user.CurrentChatId, user.CurrentChatName);

            // ������� - �������� ��������� SystemBody �� HashBody
            string dateHistoryStart = JsonSerializer.Serialize<DateTime>(date);
            // �������� ������������� ��������� �� �����
            messageGettingHistory.SystemBody = dateHistoryStart;

            // ³������� �����������
            MessageSender(messageGettingHistory);
        }

        private void chatContactsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ����� �� ������� �� ������� ��������� ������ ������������ ����
            // ��������� ����������� ��� �������� ������� MessageForSending
            MessageForSending messageForSendingToGetUserActual = new MessageForSending(
                SystemInfo.SystemInfo_EmptyMessageBody, 
                SystemInfo.SystemInfo_UpdateUserAllInfo, 
                user.UserId, user.UserLogin, user.CurrentChatId, user.CurrentChatName);
            
            // ��������� �������� ��'���� UserForSending? user
            // - ������ ��������� ��� ����������� ��'���� � ��������� �� ������ � �������
            // ���������� ��'��� ��� ����������� ��� ����������� 
            string userHesh = user.GetHashCode().ToString();

            // �������� ��������� SystemBody �� HashBody
            messageForSendingToGetUserActual.HashBody = userHesh;

            // ³������� �����������
            MessageSender(messageForSendingToGetUserActual);
        }

        private void blackListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ����� �� ������� �� ������� ��������� ������� ������ �����������
            // ��������� ����������� MessageForSending ��� �������� ������� 
            MessageForSending messageForSendingToGetUserActual = new MessageForSending(
                SystemInfo.SystemInfo_EmptyMessageBody,
                SystemInfo.SystemInfo_UpdateUserAllInfoForBlackList,
                user.UserId, user.UserLogin, user.CurrentChatId, user.CurrentChatName);

            // ��������� �������� ��'���� UserForSending? user
            // - ������ ��������� ��� ����������� ��'���� � ��������� �� ������ � �������
            // ���������� ��'��� ��� ����������� ��� ����������� 
            string userHesh = user.GetHashCode().ToString();

            // �������� ��������� SystemBody �� HashBody
            messageForSendingToGetUserActual.HashBody = userHesh;

            // ³������� �����������
            MessageSender(messageForSendingToGetUserActual);
        }

        private void chatEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ����� �� ������� �� ������� ��������� ����� ����������� ��� ������� ���-��������� 
            // ��������� ����������� MessageForSending ��� �������� ������� 
            MessageForSending messageForSendingToGetUserActual = new MessageForSending(
                SystemInfo.SystemInfo_EmptyMessageBody,
                SystemInfo.SystemInfo_UpdateUserAllInfoForChatEditor,
                user.UserId, user.UserLogin, user.CurrentChatId, user.CurrentChatName);

            // ��������� �������� ��'���� UserForSending? user
            // - ������ ��������� ��� ����������� ��'���� � ��������� �� ������ � �������
            // ���������� ��'��� ��� ����������� ��� ����������� 
            string userHesh = user.GetHashCode().ToString();

            // �������� ��������� �� HashBody
            messageForSendingToGetUserActual.HashBody = userHesh;

            // ³������� �����������
            MessageSender(messageForSendingToGetUserActual);
        }

    }
}

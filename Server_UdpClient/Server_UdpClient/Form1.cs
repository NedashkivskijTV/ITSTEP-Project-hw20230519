using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using ChatLibrary;
using Microsoft.EntityFrameworkCore;

namespace Server_UdpClient
{
    public partial class Form1 : Form
    {
        int localPort = 4569; // or 11000
        string multicastGroupIp = "0.0.0.0";

        // �������� ������� ����� - ���������� �볺���
        List<IPEndPoint> clientsList = new List<IPEndPoint>();
        // �������� ����� ���������� ������������
        List<string> usersNamesList = new List<string>();
        // �������� Id ����, �� ������ ������

        // �������� ��'���� UserForSending, �� ������ ��� ���, �� ���������3 �������� ��������
        List<ModelUserAllInfo> usersAllInfoList = new List<ModelUserAllInfo>();

        // ���������� � ������ "server"
        User? server;

        // ��'����, �� ���������������� ��� ��������� ������� �볺��� �� �������
        // (������������� � ��� ��������� Id ���������� �� Id ����)
        User? userTemp;
        Chat? serverSystemChat;

        // �������� ���
        Chat? groupChat;

        // ��'��� ��� �����䳿 � ��
        DatabaseService? databaseService;
        // ��'���, �� ������� ������� ��
        ItemCreator? creator;


        // �� ��� ���������� - ������������� ��� �����, �������� �� ��� ������, ���������� ��� ��������� ������
        List<ChatLibrary.Message> messagesList = new List<ChatLibrary.Message>();
        // ʳ������ ���������� � �� �� ������ ������������ �� ������ ������� 
        int messagesListCount = 0;

        // �� ��� ������������ - ������������� ��� �����, �������� �� ��� ������, ���������� ��� ��������� ������
        List<User> usersList = new List<User>();
        // �� ��� ���� (� ��� ������������) - ������������� ��� �����, �������� �� ��� ������, ���������� ��� ��������� ������
        List<Chat> chatsList = new List<Chat>();

        // ��������� ���� ��� ������������ �������� ����� (�� ������������� - 1 ����)
        DateTime startDateToLoadChatHistory;

        public Form1()
        {
            InitializeComponent();

            InitData();

            InitSystemDataInDb();
        }

        // ����������� ����� �� ��'����
        private void InitData()
        {
            databaseService = new DatabaseService();
            creator = new ItemCreator();
            startDateToLoadChatHistory = DateTime.Today;
        }

        // ����������� (��� ��������� � ��� ���������) ��������� ��'���� �� (������������, ����)
        private void InitSystemDataInDb()
        {
            server = databaseService.InitServer();
            userTemp = databaseService.InitOrCreateUser(SystemUsers.SystemUser_UserTemp);
            groupChat = databaseService.InitGroupChat();
            serverSystemChat = databaseService.InitOrCreateChat(SystemInfo.SystemInfo_ServerSystemChat, SystemUsers.SystemUser_Server);
        }

        // �������� ������� ������ - �������
        private void btnStartServer_Click(object sender, EventArgs e)
        {
            Task.Run(Listener);

            btnStartServer.Enabled = false;
        }

        // ��������������� ����������� �볺����� ����������
        private void Listener()
        {
            UdpClient listener = new UdpClient(new IPEndPoint(IPAddress.Parse(multicastGroupIp), localPort));
            IPEndPoint remoteEP = null;

            Text = "Server was started !";

            // ������������ ����� ����
            LoadChatHistoryToServerStatistic();

            // ��������� ����������� =============================================================================================
            ChatLibrary.Message messageServerStarted = creator.CreateMessage(
                $"{SystemInfo.SystemInfo_MessageServerStarted} {DateTime.Now.ToString()}",
                SystemInfo.SystemInfo_ServerStarted,
                server.Id, groupChat.Id);
            // ³������� �����������
            // ³���������� ����������� � ��� ���������� �������
            tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageServerStarted));
            // ��������� ����������� �� �������� ���������� (�� ������� ������ �������� ���������� �������������, ������������� � ��)
            messagesList.Add(messageServerStarted);
            // ���������� ����������� �� �� !!!!!!!
            databaseService.SaveMessageToDb(messageServerStarted);

            //int hash = messageServerStarted.Body.GetHashCode();
            //MessageBox.Show("" + messageServerStarted.Body + " - " + hash);

            // ���� ���������������
            while (true)
            {
                // ��������� �����������
                byte[] buffer = listener.Receive(ref remoteEP);
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(Encoding.Default.GetString(buffer));

                // ������������ ���������� �� �볺��� ����������� � ������ �����
                string clientMessage = builder.ToString();

                // ������������ ����������� � ��'��� ���� MessageForSending
                MessageForSending messageForSendingFormatReceived = JsonSerializer.Deserialize<MessageForSending>(clientMessage);

                // ������������� MessageForSending � ��'��� Message
                ChatLibrary.Message messageReceived = creator.ConvertMessageForSendingToMessage(messageForSendingFormatReceived);

                // �������� ����������� ��'���� Message - Id ����������� �� ���� ����� ���� ��������� !!!
                // (� ��� ��������� ������������� �������� �������� ��������� ��'����)
                ControlOfMessageContent(messageReceived);


                // ��������� ����������� �� �������� ���������� (�� ������� ������ �������� ���������� �������������, ������������� � ��)
                messagesList.Add(messageReceived);

                // ���������� ���������� ����������� �� ��
                databaseService.SaveMessageToDb(messageReceived);

                // ��������� ���������� ����������� (�� �볺��� �� �������)

                tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageReceived));


                // �������� ������� ������ �볺��� �� ���������� (�������� ���� �� ������)
                if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_AuthorizationUser))
                {
                    // ����� �� �� �� �������� ����� �� ������ - ����������� �����������
                    if (!databaseService.AuthorizationUser(messageForSendingFormatReceived.CreatorUserLogin, messageForSendingFormatReceived.HashBody)) // ������� �����������
                    {
                        // ������� ��������ֲ�
                        // ��������� ����������� ��� �������� �볺��� ��� ������� ���������� �� ���� (������� ���� ��� ������)
                        ChatLibrary.Message messageWrongAuthorize = creator.CreateMessage(
                            SystemInfo.SystemInfo_EmptyMessageBody,
                            SystemInfo.SystemInfo_AuthorizUserFalse, server.Id, serverSystemChat.Id);

                        // ������������� � ������ ��� �������� - MessageForSending
                        MessageForSending messageForSendingWrongAuthorize = creator.ConvertMessageToMessageForSending(messageWrongAuthorize, server.Login, serverSystemChat.ChatName);

                        // ���������� �����������
                        string messageForSendingWrongAuthorizeJson = JsonSerializer.Serialize(messageForSendingWrongAuthorize);

                        // ³������� �����������
                        SendMessagesForUser(messageForSendingWrongAuthorizeJson, remoteEP);

                        // �������� ����������� �� �������� ����������
                        messagesList.Add(messageWrongAuthorize);

                        // ���������� ����������� �� ��
                        databaseService.SaveMessageToDb(messageWrongAuthorize);

                        // ��������� ���������� ����������� (�� ������� �� �볺���)
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageWrongAuthorize));
                    }
                    else
                    {
                        // ����� ��������ֲ� - ���� �� ������ �������� - ���������� ����������,
                        // � ���� ������������� ��'��� UserForSending, ���� ������� ������
                        // (�����������) ����������� �� ������ ��� ������� ������� ��� ����� ���
                        // � �.�. ��������
                        // - ����, �� ��� ������� (��� ��� ��� ������ Id, ����� ����, �������� �������� ����),
                        // - ���� �� ���� �� �� ������ (��� ��� ��� ������ Id, ����� ����, �������� �������� ����),
                        // - �����, �� ������� �� ������� ������ (��� ��� ����� ������ ����, Id, ������)

                        // ��������� ����� ����������� (������) �� �������� ���������� �����
                        usersNamesList.Add(messageForSendingFormatReceived.CreatorUserLogin.Trim()); // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                        // ��������� ������ ����� ����������� (������) �� �������� ���������� ������������
                        clientsList.Add(remoteEP); // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                        // ��������� ����������� from server to client ��� ���� (�����������) ����� ���������� �� ����
                        ChatLibrary.Message messageToUserAuthorizUserTrue = creator.CreateMessage(SystemInfo.SystemInfo_EmptyMessageBody, SystemInfo.SystemInfo_AuthorizUserTrue, server.Id, serverSystemChat.Id);

                        // ������������� ����������� (������-�볺��, �� �����������) � ������ ��� �������� MessageForSending
                        MessageForSending messageForSendingToUserAuthorizUserTrue = creator.ConvertMessageToMessageForSending(messageToUserAuthorizUserTrue, server.Login, serverSystemChat.ChatName);

                        // ���������� ��'���� ModelUserAllInfo - ��'���, �� ������ ����������� ������� ��� ��� �������� ���� ����������� 
                        // � �.�. � ����������� ��� � ������������ �������� � ����� - � ���� �������� 
                        // - userNew.chatsList.UsersInChat - ��� �����
                        // - userNew.blackList - ���� ����
                        //CheckingThePresenceOfUsersInTheNetwork(userNew);
                        ModelUserAllInfo userNew = databaseService.CreateUserForSending(
                            messageForSendingFormatReceived.CreatorUserLogin,
                            remoteEP,
                            groupChat.Id,
                            groupChat.ChatName,
                            usersNamesList);

                        //��������� ��� ��� ����������� �� �������� ���������� ������������ (�������� ��'���� ModelUserAllInfo)
                        // - ������ ��������������� ���� ��������, �� ������ ���� �������� ����� ����� ��� �������������� ������ ��������
                        usersAllInfoList.Add(userNew);

                        // ������������� ��'���� ModelUserAllInfo � UserForSending - ������� ������������ �� �볺��� (���� ����������� �� ������ ��� ����� �������� ���)
                        UserForSending userNewForSending = creator.ConvertModelUserAllInfoToUserForSending(userNew);

                        // ���������� ��'���� UserForSending
                        string userNewForSendingJson = JsonSerializer.Serialize<UserForSending>(userNewForSending);

                        // �������� ������������� ��'���� UserForSending �� ��'���� MessageForSending 
                        messageForSendingToUserAuthorizUserTrue.SystemBody = userNewForSendingJson;

                        // ���������� ��'���� MessageForSending ��� ����������� �볺���
                        string messageUserAuthorizTrueJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingToUserAuthorizUserTrue);

                        // ³������� �����������
                        SendMessagesForUser(messageUserAuthorizTrueJson, remoteEP);

                        // �������� ����������� �� �������� ����������
                        messagesList.Add(messageToUserAuthorizUserTrue);

                        // ���������� ������������ ����������� �� ��
                        databaseService.SaveMessageToDb(messageToUserAuthorizUserTrue);

                        // ³���������� ������������ ����������� (��� ����� ���������� �볺��� �� ����) �� ����� ���������� �������
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageToUserAuthorizUserTrue));

                        // ³���������� � �������� ��� ����������� ��� ��������� �볺���
                        // - ��������� ����������� ��� ��������� �볺��� -----------------------------------------------------------------------------------
                        ChatLibrary.Message messageUserAuthorizeTrueForAll = creator.CreateMessage(
                            $"{SystemInfo.SystemInfo_MessageJoinedTheChat}{messageForSendingFormatReceived.CreatorUserLogin}" +
                            $"{SystemInfo.SystemInfo_Separator}" +
                            $"{SystemInfo.SystemInfo_MessageNumberOfChatParticipants} {UsersInChat(userNew.CurrentChatId)}" +
                            $"{SystemInfo.SystemInfo_Separator}",
                            SystemInfo.SystemInfo_ServerMessage, server.Id, groupChat.Id);

                        // ������������� � ������ ��� �������� MessageForSending
                        MessageForSending messageForSendingUserAuthorizeTrueForAll = creator.ConvertMessageToMessageForSending(messageUserAuthorizeTrueForAll, server.Login, groupChat.ChatName);

                        // C��������� ����������� ��� ��������� �볺���
                        string messageForSendingUserAuthorizeTrueForAllJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingUserAuthorizeTrueForAll);

                        // ³������� ����������� ��� ��������� �볺��� � �������� ���
                        //SendMessagesAllUsers($"{DateTime.Now}\r\n�� ���� ��������� :{clientMessage.Substring(20)}" + $"�볺��� � ���: {clientsList.Count}\r\n") ;
                        SendMessagesAllUsers(messageForSendingUserAuthorizeTrueForAllJson);

                        // �������� ����������� �� �������� ����������
                        messagesList.Add(messageUserAuthorizeTrueForAll);

                        // ���������� �� �� ����������� ��� ��������� �볺��� � �������� ���
                        databaseService.SaveMessageToDb(messageUserAuthorizeTrueForAll);

                        // ��������� � ���� ���������� ���� ����������� ��� ��������� �볺���
                        //tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), $"{DateTime.Now}\r\n�� ���� ��������� :{clientMessage.Substring(20)}" + $"�볺��� � ���: {clientsList.Count}\r\n");
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageUserAuthorizeTrueForAll));
                    }
                }
                // �������� ������� ���������� ��� ��������� �볺���
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_RegistrationUser))
                {
                    string systemBodyAboutUserRegistration = SystemInfo.SystemInfo_RegistrationUserTrue; // ������������ ��� ����� ���������
                    if (!databaseService.RegistrationUser(messageForSendingFormatReceived.CreatorUserLogin, messageForSendingFormatReceived.HashBody))
                    {
                        // ������� ��������� (���� ��� ���������������)
                        systemBodyAboutUserRegistration = SystemInfo.SystemInfo_RegistrationUserFalse;
                    }

                    // ��������� ����������� ��� �������� �볺��� ��� ��������� ��������� 
                    ChatLibrary.Message messageWrongRegistration = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody,
                        systemBodyAboutUserRegistration,
                        server.Id, serverSystemChat.Id);

                    // ������������� � ������ ��� �������� - MessageForSending
                    MessageForSending messageForSendingWrongRegistration = creator.ConvertMessageToMessageForSending(
                        messageWrongRegistration, server.Login, serverSystemChat.ChatName);

                    // ���������� �����������
                    string messageForSendingWrongRegistrationJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingWrongRegistration);

                    // ³������� �����������
                    SendMessagesForUser(messageForSendingWrongRegistrationJson, remoteEP);

                    // �������� ����������� �� �������� ����������
                    messagesList.Add(messageWrongRegistration);

                    // ���������� ����������� �� ��
                    databaseService.SaveMessageToDb(messageWrongRegistration);

                    // ��������� ���������� ����������� (�� ������� �� �볺���)
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageWrongRegistration));
                }
                // �������� ������� ���������� ����������� �볺���
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UserMessage))
                {
                    // �������� ���������� �볺���� ����������� ��� ���������� �볺���� � ��������� � ���������� ���
                    SendMessagesAllUsers(clientMessage, messageReceived.ChatsId, messageReceived.CreatorUserId); // �������� � �������� � ���������� ���
                }
                // �������� ������� ������ �� ���������� �볺��� �� ������ (��������� � ���������� ����)
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_ConnectChat))
                {

                    // ���������� ��������� �������� � �������� ������������ List<ModelUserAllInfo> usersAllInfoList
                    // � ����� ���������� ��������� ��� �� �������������� �� ������ ����
                    int posUserName = clientsList.IndexOf(remoteEP); // ���������� ��������� � �������� ������� ����� (���������� ������)

                    // ��� ��� ��������� ���
                    int oldChatId = usersAllInfoList[posUserName].CurrentChatId;
                    string oldChatName = usersAllInfoList[posUserName].CurrentChatName;
                    
                    // ����������, �� ���� �� ����� ������ ��� �� ����� ���� ����������� �볺��
                    Chat newChat = databaseService.InitChat(messageReceived.ChatsId);

                    // ���������� ����������� ��� �����/������� ���������� �� ����
                    string messageAboutConnectionToChat = SystemInfo.SystemInfo_ChatIsConnected; // ����������� ��� ����� ���������� �� ����
                    if (newChat == null)
                    {
                        messageAboutConnectionToChat = SystemInfo.SystemInfo_ChatNotConnected; // � ��� ��������� ���� - ����������� ��� ������� ���������� �� ����
                    }
                    // ��������� ����������� from server to client ��� ���� (�����������) ���������� �� ����
                    ChatLibrary.Message messageToUserConnectingToChat = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody,
                        messageAboutConnectionToChat, server.Id, serverSystemChat.Id);

                    // ������������� ����������� (������-�볺��, �� �����������) � ������ ��� �������� MessageForSending
                    MessageForSending messageForSendingToUserConnectingToChat = creator.ConvertMessageToMessageForSending(
                        messageToUserConnectingToChat, server.Login, serverSystemChat.ChatName);

                    // ���������� ��'���� ModelUserAllInfo - ��'���, �� ������ ����������� ������� ��� ��� �������� ���� ����������� 
                    ModelUserAllInfo userAllInfo = databaseService.CreateUserForSending(
                        usersAllInfoList[posUserName].UserLogin,
                        usersAllInfoList[posUserName].UsersIPEndPoint,
                        (newChat == null ? oldChatId : newChat.Id),
                        (newChat == null ? oldChatName : newChat.ChatName),
                        usersNamesList);

                    // ��������� (����� �� ������������� ��'���) ��� ��� ����������� �� �������� ���������� ������������ (�������� ��'���� ModelUserAllInfo)
                    usersAllInfoList[posUserName] = userAllInfo;

                    // ������������� ��'���� ModelUserAllInfo � UserForSending - ������� ������������ �� �볺��� (���� ����������� �� ������ ��� ����� �������� ���)
                    UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);

                    // ���������� ��'���� UserForSending
                    string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);

                    // �������� ������������� ��'���� UserForSending �� ��'���� MessageForSending 
                    messageForSendingToUserConnectingToChat.SystemBody = userAllInfoForSendingJson;

                    // ���������� ��'���� MessageForSending ��� ����������� �볺���
                    string messageForSendingToUserConnectingToChatJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingToUserConnectingToChat);

                    // ³������� ����������� �����������, �� ����������� �� ���� - ����������� �볺��� ��� �����/������� ���������� �� ������ ����
                    SendMessagesForUser(messageForSendingToUserConnectingToChatJson, remoteEP);

                    // �������� ����������� �� �������� ����������
                    messagesList.Add(messageToUserConnectingToChat);

                    // ���������� ������������ ����������� �� ��
                    databaseService.SaveMessageToDb(messageToUserConnectingToChat);

                    // ³���������� ������������ ����������� (��� ����� ���������� �볺��� �� ����) �� ����� ���������� �������
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageToUserConnectingToChat));

                    // ����������� � ��������� ��� ��� ����� ����������� - ������ ���������� ���� ������ �볺��� � ����
                    if (newChat != null)
                    {
                        // ³������� ����������� ��� ��'������� �볺��� - �� ����
                        // ��������� ����������� ��� �������� � ��� 
                        ChatLibrary.Message messageUserOutFromChat = creator.CreateMessage(
                            $"{SystemInfo.SystemInfo_MessageUserLeftTheChat}{usersAllInfoList[posUserName].UserLogin}" +
                            $"{SystemInfo.SystemInfo_Separator}" +
                            $"{SystemInfo.SystemInfo_MessageNumberOfChatParticipants}{UsersInChat(oldChatId)}", // -1
                            SystemInfo.SystemInfo_UserDisconnect, server.Id, oldChatId);

                        // ������������� � ������ ��� �������� MessageForSending
                        MessageForSending messageeForSendingUserOutFromChat = creator.ConvertMessageToMessageForSending(
                            messageUserOutFromChat,
                            server.Login, oldChatName);

                        // ���������� �����������
                        string messageeForSendingUserOutFromChatJson = JsonSerializer.Serialize<MessageForSending>(messageeForSendingUserOutFromChat);

                        // ³������� ����������� - ������������� ���� ����������� �볺��� ��� ����� ���������� �� ������ ����
                        // - � ��� � ����� ��������� �볺�� �� ������ ������ (id ���� ��������� � ����������)
                        SendMessagesAllUsers(messageeForSendingUserOutFromChatJson, oldChatId);

                        // �������� ����������� �� �������� ����������
                        messagesList.Add(messageUserOutFromChat);

                        // ���������� ����������� �� ��
                        databaseService.SaveMessageToDb(messageUserOutFromChat);

                        // ��������� ������������ ����������� 
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageUserOutFromChat));

                        // ³���������� � ����� ��� ����������� ��� ��������� �볺���
                        // - ��������� ����������� ��� ��������� �볺��� 
                        ChatLibrary.Message messageUserConnectedToChat = creator.CreateMessage(
                            $"{SystemInfo.SystemInfo_MessageJoinedTheChat} {usersAllInfoList[posUserName].UserLogin}" +
                            $"{SystemInfo.SystemInfo_Separator}" +
                            $"{SystemInfo.SystemInfo_MessageNumberOfChatParticipants} {UsersInChat(newChat.Id)}", // +1
                            SystemInfo.SystemInfo_ServerMessage, server.Id, newChat.Id);

                        // ������������� � ������ ��� �������� MessageForSending
                        MessageForSending messageForSendingUserConnectedToChat = creator.ConvertMessageToMessageForSending(
                            messageUserConnectedToChat,
                            server.Login, newChat.ChatName);

                        // - ���������� ����������� ��� ��������� �볺���
                        string messageForSendingUserConnectedToChatJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingUserConnectedToChat);

                        // -  �������� ����������� ��� ��������� �볺��� � ����� ���
                        SendMessagesAllUsers(messageForSendingUserConnectedToChatJson, newChat.Id);

                        // �������� ����������� �� �������� ����������
                        messagesList.Add(messageUserConnectedToChat);

                        // - ���������� �� �� ����������� ��� ��������� �볺��� � ����� ���
                        databaseService.SaveMessageToDb(messageUserConnectedToChat);

                        // - ��������� � ���� ���������� ���� ����������� ��� ��������� �볺���
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageUserConnectedToChat));
                    }
                    
                }
                // �������� ������� ������ �볺��� �� ��������� ����� ����
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_GetChatHistory))
                {
                    // ��������� �������� ���� ��� �������� ����� ����
                    DateTime dateStartHistory = JsonSerializer.Deserialize<DateTime>(messageForSendingFormatReceived.SystemBody);

                    // �������� ����� ����
                    List<ChatLibrary.Message> messages = databaseService.GetChatHisrory(messageReceived.ChatsId, dateStartHistory);
                    //MessageBox.Show("TEST - messages - " + messages.Count);

                    List<ChatLibrary.Message> subMessagesList;
                    bool firstPackage = true;
                    int stepElement = SystemInfo.SystemInfo_NumberOfMessagesInTheSendingPackage; // 40
                    for (int i = 0; i < messages.Count; i += stepElement )
                    {
                        if(i + stepElement > messages.Count)
                        {
                            stepElement = messages.Count - i;
                        }
                        subMessagesList = messages.GetRange(i, stepElement);

                        // ���������� ����� ���� � ������ �����
                        string chatHistory = GetHistoryString(subMessagesList, messageReceived.CreatorUserId);

                        // ���������� ��������� systemInfo � ��������� �� ������� ������
                        // - ����������� ��� ������� �� ���������
                        string systemInfo = SystemInfo.SystemInfo_NewChatHistoryContinue;
                        if (firstPackage)
                        {
                            systemInfo = SystemInfo.SystemInfo_NewChatHistoryStart;
                            firstPackage = false;
                        }

                        // ��������� ����������� ��� �������� �볺��� 
                        ChatLibrary.Message messageChatHistory = creator.CreateMessage(
                            SystemInfo.SystemInfo_EmptyMessageBody,
                            systemInfo, server.Id, messageReceived.ChatsId);

                        // ������������� � ������ ��� �������� MessageForSending
                        MessageForSending messageForSendingChatHistory = creator.ConvertMessageToMessageForSending(
                            messageChatHistory, server.Login, messageForSendingFormatReceived.ChatsName);

                        // ������� - �������� ��������� SystemBody �� HashBody
                        messageForSendingChatHistory.SystemBody = chatHistory;

                        // ���������� �����������
                        string messageForSendingChatHistoryJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingChatHistory);

                        // ³������� �����������
                        SendMessagesForUser(messageForSendingChatHistoryJson, remoteEP);

                        // �������� ����������� �� �������� ����������
                        messagesList.Add(messageChatHistory);

                        // ���������� ����������� �� ��
                        databaseService.SaveMessageToDb(messageChatHistory);

                        // ��������� ������������ ����������� � ���������� �������
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageChatHistory));
                    }
                }
                // �������� ������� ������ �� ����������� ������ ������������ ����
                // - ������� ������ �볺��� �� ��������� ����� ��� ����
                // - �������� ��'���� � ��� ������������� ����� �� ���� �볺��� (��������� ���-����)
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UpdateUserAllInfo))
                {
                    // ���������� ��������� �������� � �������� ������������ List<ModelUserAllInfo> usersAllInfoList
                    // � ����� ���������� ��������� ��� �� �������� ����������� �����
                    int posUserName = clientsList.IndexOf(remoteEP); // ���������� ��������� � �������� ������� ����� (���������� ������)

                    // ���������� ��'���� ModelUserAllInfo - ��'���, �� ������ ����������� ������� ��� ��� �������� ���� ����������� 
                    ModelUserAllInfo userAllInfo = databaseService.CreateUserForSending(
                        usersAllInfoList[posUserName].UserLogin,
                        usersAllInfoList[posUserName].UsersIPEndPoint,
                        usersAllInfoList[posUserName].CurrentChatId,
                        usersAllInfoList[posUserName].CurrentChatName,
                        usersNamesList);

                    // ���������� ����������� ��'���� ModelUserAllInfo userAllInfo � ���� ����������� �� �볺�� 
                    // - ��������� ���-����
                    UserForSending userForSendingAllInfo = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);
                    string userAllInfoOnServer = userForSendingAllInfo.GetHashCode().ToString();

                    // ��������� ����������� from server to client ��� ����������� ����� ��� ���������� ����������� ��'���� UserForSending
                    ChatLibrary.Message messageToUserLoadContactsList = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody,
                        SystemInfo.SystemInfo_UserAllInfoIsUpToDate, server.Id, serverSystemChat.Id);

                    // ������������� ����������� (������-�볺��) � ������ ��� �������� MessageForSending
                    MessageForSending messageForSendingToUserLoadContactsList = creator.ConvertMessageToMessageForSending(
                        messageToUserLoadContactsList, server.Login, serverSystemChat.ChatName);
                    
                    // ����������� ����� � ���, ���� ���������� �� �볺�� �� ���������, � ����� ���������� ��������� ���
                    if (messageForSendingFormatReceived.HashBody != userAllInfoOnServer)
                    {
                        messageToUserLoadContactsList.SystemInfo = SystemInfo.SystemInfo_SendingTheCurrentUserAllInfo;
                        messageForSendingToUserLoadContactsList.SystemInfo = SystemInfo.SystemInfo_SendingTheCurrentUserAllInfo;

                        // ��������� (����� �� ������������� ��'���) ��� ��� ����������� �� �������� ���������� ������������ (�������� ��'���� ModelUserAllInfo)
                        usersAllInfoList[posUserName] = userAllInfo;

                        // ������������� ��'���� ModelUserAllInfo � UserForSending - ������� ������������ �� �볺��� (���� ����������� �� ������ ��� ����� �������� ���)
                        UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);
                        
                        // ���������� ��'���� UserForSending
                        string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);

                        // �������� ������������� ��'���� UserForSending �� ��'���� MessageForSending 
                        messageForSendingToUserLoadContactsList.SystemBody = userAllInfoForSendingJson;
                    }

                    // ���������� ��'���� MessageForSending ��� ����������� �볺���
                    string messageForSendingToUserLoadContactsListJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingToUserLoadContactsList);

                    // ³������� ����������� �����������, �� ����������� �� ���� - ����������� �볺��� ��� �����/������� ���������� �� ������ ����
                    SendMessagesForUser(messageForSendingToUserLoadContactsListJson, remoteEP);

                    // �������� ����������� �� �������� ����������
                    messagesList.Add(messageToUserLoadContactsList);

                    // ���������� ������������ ����������� �� ��
                    databaseService.SaveMessageToDb(messageToUserLoadContactsList);

                    // ³���������� ������������ ����������� (��� ����� ���������� �볺��� �� ����) �� ����� ���������� �������
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageToUserLoadContactsList));
                }
                // �������� ������� ������ �� ����������� ������� ������
                // - ������� ������ �볺��� �� ��������� ����� ��� ����
                // - �������� ��'���� � ��� ������������� ����� �� ���� �볺��� (��������� ���-����)
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UpdateUserAllInfoForBlackList))
                {
                    // ���������� ��������� �������� � �������� ������������ List<ModelUserAllInfo> usersAllInfoList
                    // � ����� ���������� ��������� ��� �� �������� ����������� �����
                    int posUserName = clientsList.IndexOf(remoteEP); // ���������� ��������� � �������� ������� ����� (���������� ������)

                    // ���������� ��'���� ModelUserAllInfo - ��'���, �� ������ ����������� ������� ��� ��� �������� ���� ����������� 
                    ModelUserAllInfo userAllInfo = databaseService.CreateUserForSending(
                        usersAllInfoList[posUserName].UserLogin,
                        usersAllInfoList[posUserName].UsersIPEndPoint,
                        usersAllInfoList[posUserName].CurrentChatId,
                        usersAllInfoList[posUserName].CurrentChatName,
                        usersNamesList);

                    // ���������� ����������� ��'���� ModelUserAllInfo userAllInfo � ���� ����������� �� �볺�� 
                    // - ��������� ���-����
                    UserForSending userForSendingAllInfo = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);
                    string userAllInfoOnServer = userForSendingAllInfo.GetHashCode().ToString();

                    // ��������� ����������� from server to client ��� ����������� ����� ��� ���������� ����������� ��'���� UserForSending
                    ChatLibrary.Message messageToUserLoadBlackList = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody,
                        SystemInfo.SystemInfo_UserAllInfoIsUpToDateForBlackList, server.Id, serverSystemChat.Id);

                    // ������������� ����������� (������-�볺��) � ������ ��� �������� MessageForSending
                    MessageForSending messageForSendingToUserLoadBlackList = creator.ConvertMessageToMessageForSending(
                        messageToUserLoadBlackList, server.Login, serverSystemChat.ChatName);
                    
                    // ����������� ����� � ���, ���� ���������� �� �볺�� �� ���������, � ����� ���������� ��������� ���
                    if (messageForSendingFormatReceived.HashBody != userAllInfoOnServer)
                    {
                        messageToUserLoadBlackList.SystemInfo = SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForBlackList;
                        messageForSendingToUserLoadBlackList.SystemInfo = SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForBlackList;

                        // ��������� (����� �� ������������� ��'���) ��� ��� ����������� �� �������� ���������� ������������ (�������� ��'���� ModelUserAllInfo)
                        usersAllInfoList[posUserName] = userAllInfo;

                        // ������������� ��'���� ModelUserAllInfo � UserForSending - ������� ������������ �� �볺��� (���� ����������� �� ������ ��� ����� �������� ���)
                        UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);
                        
                        // ���������� ��'���� UserForSending
                        string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);

                        // �������� ������������� ��'���� UserForSending �� ��'���� MessageForSending 
                        messageForSendingToUserLoadBlackList.SystemBody = userAllInfoForSendingJson;
                    }

                    // ���������� ��'���� MessageForSending ��� ����������� �볺���
                    string messageForSendingToUserLoadBlackListJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingToUserLoadBlackList);

                    // ³������� ����������� �����������, �� ����������� �� ���� - ����������� �볺��� ��� �����/������� ���������� �� ������ ����
                    SendMessagesForUser(messageForSendingToUserLoadBlackListJson, remoteEP);

                    // �������� ����������� �� �������� ����������
                    messagesList.Add(messageToUserLoadBlackList);

                    // ���������� ������������ ����������� �� ��
                    databaseService.SaveMessageToDb(messageToUserLoadBlackList);

                    // ³���������� ������������ ����������� (��� ����� ���������� �볺��� �� ����) �� ����� ���������� �������
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageToUserLoadBlackList));
                }
                // �������� ������� ������ �� ��������� ������� ������ �볺��� � ��
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UpdateUserBlackListInDb))
                {
                    // ���������� ��������� �������� � �������� ������������ List<ModelUserAllInfo> usersAllInfoList
                    // � ����� ���������� ��������� ��� �� �������� �볺��� ���������� userAllInfo ���� ����������� ������� ������
                    int posUserName = clientsList.IndexOf(remoteEP); // ���������� ��������� � �������� ������� ����� (���������� ������)

                    // ��������� �������� List<ModelUsersLoginAndId> usersBlackList - ����������� ������ ������
                    string usersBlackListJson = messageForSendingFormatReceived.SystemBody;
                    
                    // ������������ ��������
                    List<ModelUsersLoginAndId> usersBlackList = new List<ModelUsersLoginAndId>(); 
                    if(usersBlackListJson.Length > 0)
                    {
                        usersBlackList = JsonSerializer.Deserialize<List<ModelUsersLoginAndId>>(usersBlackListJson); 
                    }

                    // ��������� ����� � ��
                    databaseService.UpdateUsersBlackListAsync(usersBlackList, messageReceived.CreatorUserId);

                    // ��������� �� ��������� ��'���� ModelUserAllInfo
                    ModelUserAllInfo userAllInfo = databaseService.CreateUserForSending(
                        usersAllInfoList[posUserName].UserLogin,
                        remoteEP,
                        usersAllInfoList[posUserName].CurrentChatId,
                        usersAllInfoList[posUserName].CurrentChatName,
                        usersNamesList);
                    // ��������� (����� �� ������������� ��'���) ��� ��� ����������� �� �������� ���������� ������������ (�������� ��'���� ModelUserAllInfo)
                    usersAllInfoList[posUserName] = userAllInfo;

                    // ³������� ���������� ModelUserAllInfo �볺���
                    // ��������� ����������� ��� �������� �볺��� 
                    ChatLibrary.Message messageNewUserAllInfo = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody, 
                        SystemInfo.SystemInfo_UpdatedUserInformation, server.Id, serverSystemChat.Id);

                    // ������������� � ������ ��� �������� MessageForSending
                    MessageForSending messageForSendingNewUserAllInfo = creator.ConvertMessageToMessageForSending(
                        messageNewUserAllInfo, server.Login, serverSystemChat.ChatName
                        );

                    // �������� ��������� �� SystemBody 
                    // ������������� ��'���� ModelUserAllInfo � UserForSending - ������� ������������ �� �볺��� 
                    UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);
                    // ���������� ��'���� UserForSending
                    string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);
                    // �������� ������������� ��'���� UserForSending �� ��'���� MessageForSending
                    messageForSendingNewUserAllInfo.SystemBody = userAllInfoForSendingJson;

                    // ���������� �����������
                    string messageForSendingNewUserAllInfoJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingNewUserAllInfo);

                    // ³������� �����������
                    SendMessagesForUser(messageForSendingNewUserAllInfoJson, remoteEP);

                    // �������� ����������� �� �������� ����������
                    messagesList.Add(messageNewUserAllInfo);

                    // ���������� ����������� �� ��
                    databaseService.SaveMessageToDb(messageNewUserAllInfo);

                    // ��������� ������������ ����������� 
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageNewUserAllInfo));


                }
                // �������� ������� ������ �� ������������ �볺���� ��������� ����
                // - ������� ������ �볺��� �� ��������� ����� ��� ����
                // - �������� ��'���� UserForSending � ��� ������������� ����� �� ���� �볺��� (��������� ���-����)
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UpdateUserAllInfoForChatEditor))
                {
                    // ���������� ��������� �������� � �������� ������������ List<ModelUserAllInfo> usersAllInfoList
                    // � ����� ���������� ��������� ��� �� �������� ����������� �����
                    int posUserName = clientsList.IndexOf(remoteEP); // ���������� ��������� � �������� ������� ����� (���������� ������)

                    // ���������� ��'���� ModelUserAllInfo - ��'���, �� ������ ����������� ������� ��� ��� �������� ���� ����������� 
                    ModelUserAllInfo userAllInfo = databaseService.CreateUserForSending(
                        usersAllInfoList[posUserName].UserLogin,
                        usersAllInfoList[posUserName].UsersIPEndPoint,
                        usersAllInfoList[posUserName].CurrentChatId,
                        usersAllInfoList[posUserName].CurrentChatName,
                        usersNamesList);

                    // ���������� ����������� ��'���� ModelUserAllInfo userAllInfo � ���� ����������� �� �볺�� 
                    // - ��������� ���-����
                    UserForSending userForSendingAllInfo = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);
                    string userAllInfoOnServer = userForSendingAllInfo.GetHashCode().ToString();

                    // ��������� ����������� from server to client ��� ����������� ����� ��� ���������� ����������� ��'���� UserForSending
                    ChatLibrary.Message messageToUserLoadChatList = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody,
                        SystemInfo.SystemInfo_UserAllInfoIsUpToDateForChatList, server.Id, serverSystemChat.Id);

                    // ������������� ����������� (������-�볺��) � ������ ��� �������� MessageForSending
                    MessageForSending messageForSendingToUserLoadChatList = creator.ConvertMessageToMessageForSending(
                        messageToUserLoadChatList, server.Login, serverSystemChat.ChatName);

                    // ����������� ����� � ���, ���� ���������� �� �볺�� �� ���������, � ����� ���������� ��������� ���
                    if (messageForSendingFormatReceived.HashBody != userAllInfoOnServer)
                    {
                        messageToUserLoadChatList.SystemInfo = SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForChatList;
                        messageForSendingToUserLoadChatList.SystemInfo = SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForChatList;

                        // ��������� (����� �� ������������� ��'���) ��� ��� ����������� �� �������� ���������� ������������ (�������� ��'���� ModelUserAllInfo)
                        usersAllInfoList[posUserName] = userAllInfo;

                        // ������������� ��'���� ModelUserAllInfo � UserForSending - ������� ������������ �� �볺��� (���� ����������� �� ������ ��� ����� �������� ���)
                        UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);

                        // ���������� ��'���� UserForSending
                        string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);

                        // �������� ������������� ��'���� UserForSending �� ��'���� MessageForSending 
                        messageForSendingToUserLoadChatList.SystemBody = userAllInfoForSendingJson;
                    }

                    // ���������� ��'���� MessageForSending ��� ����������� �볺���
                    string messageForSendingToUserLoadChatListJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingToUserLoadChatList);

                    // ³������� ����������� �����������, �� ����������� �� ���� - ����������� �볺��� ��� �����/������� ���������� �� ������ ����
                    SendMessagesForUser(messageForSendingToUserLoadChatListJson, remoteEP);

                    // �������� ����������� �� �������� ����������
                    messagesList.Add(messageToUserLoadChatList);

                    // ���������� ������������ ����������� �� ��
                    databaseService.SaveMessageToDb(messageToUserLoadChatList);

                    // ³���������� ������������ ����������� (��� ����� ���������� �볺��� �� ����) �� ����� ���������� �������
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageToUserLoadChatList));
                }
                // �������� ������� ������ �� ���������/�����������/��������� ���� �����������
                // - ������ �� ����������� ��'��� List<ModelChatsNameAndId> (������ ������ ������������)
                // - ������� ��� � ��
                // - ��������� ����������� �볺��� (���� �������� �����), �� ������ �������� ��� � ��'��� UserForSending
                // - ������� ���������� ��� ���������� �볺��� (����������� ��� ������� ��'��� ModelUserForSending)
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UpdateUserChatListInDb))
                {
                    // ���������� ��������� �������� � �������� ������������ List<ModelUserAllInfo> usersAllInfoList
                    // � ����� ���������� ��������� ��� �� �������� �볺��� ���������� userAllInfo ���� ����������� �������� ����
                    int posUserName = clientsList.IndexOf(remoteEP); // ���������� ��������� � �������� ������� ����� (���������� ������)

                    // ��������� ����������� �������� List<ModelChatsNameAndId> usersChatListJson - ����������� ������ ����
                    string usersChatListJson = messageForSendingFormatReceived.SystemBody;
                    
                    // ������������ ��������
                    List<ModelChatsNameAndId> usersChatList = new List<ModelChatsNameAndId>(); 
                    if(usersChatListJson.Length > 0)
                    {
                        usersChatList = JsonSerializer.Deserialize<List<ModelChatsNameAndId>>(usersChatListJson); 
                    }

                    // ��������� ����� � ��
                    databaseService.UpdateUsersChatListAsync(usersChatList, messageReceived.CreatorUserId);

                    Thread.Sleep(1000);
                    Chat chat = databaseService.InitChat(usersAllInfoList[posUserName].CurrentChatId);
                                        
                    string s = "" + chat; // ����� ���������� ��'��� Chat � ������ ����� - ������� ����� �������,  �� ������ � ��� ��������� ���� �� �������� Id � �� - ��������� ���� ��������� ���� � ������ ���� ���������� ��� �� ����� ����������
                    
                    int currentChatId = s.Length == 0 ? groupChat.Id : chat.Id;
                    string currentChatName = s.Length == 0 ? groupChat.ChatName : chat.ChatName;

                    ModelUserAllInfo userAllInfo = databaseService.CreateUserForSending(
                        usersAllInfoList[posUserName].UserLogin,
                        usersAllInfoList[posUserName].UsersIPEndPoint,
                        currentChatId,
                        currentChatName,
                        usersNamesList);

                    // ��������� (����� �� ������������� ��'���) ��� ��� ����������� �� �������� ���������� ������������ (�������� ��'���� ModelUserAllInfo)
                    usersAllInfoList[posUserName] = userAllInfo;

                    // � ��� ��������� � �� ���� �� ����� �볺�� ��� ���������� �����
                    // - ����������� �������� �������������� �� ���������� ��������� ����
                    // - ������ - �������������� ������� ��� ��� �볺���
                    string systemInfo = chat == null ? SystemInfo.SystemInfo_ChatIsConnected : SystemInfo.SystemInfo_UpdatedUserInformation;

                    // ³������� ���������� ModelUserAllInfo �볺���
                    // ��������� ����������� ��� �������� �볺��� 
                    ChatLibrary.Message messageNewUserAllInfo = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody,
                        systemInfo, 
                        server.Id, serverSystemChat.Id);

                    // ������������� � ������ ��� �������� MessageForSending
                    MessageForSending messageForSendingNewUserAllInfo = creator.ConvertMessageToMessageForSending(
                        messageNewUserAllInfo, server.Login, serverSystemChat.ChatName);

                    // �������� ��������� �� SystemBody 
                    // ������������� ��'���� ModelUserAllInfo � UserForSending - ������� ������������ �� �볺��� 
                    UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);
                    // ���������� ��'���� UserForSending
                    string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);
                    // �������� ������������� ��'���� UserForSending �� ��'���� MessageForSending
                    messageForSendingNewUserAllInfo.SystemBody = userAllInfoForSendingJson;

                    // ���������� �����������
                    string messageForSendingNewUserAllInfoJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingNewUserAllInfo);

                    // ³������� �����������
                    SendMessagesForUser(messageForSendingNewUserAllInfoJson, remoteEP);

                    // �������� ����������� �� �������� ����������
                    messagesList.Add(messageNewUserAllInfo);

                    // ���������� ����������� �� ��
                    databaseService.SaveMessageToDb(messageNewUserAllInfo);

                    // ��������� ������������ ����������� 
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageNewUserAllInfo));

                    // ���� ��� ��� �볺���, �� �������� �� ������� (������������� ��'��� UserForSending-����� ��� ��� �볺���)
                    UpdateUsersInfoAfterUsingChatEditor(posUserName);
                }
                // �������� ������� ������ �� ���������� �볺���
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UserDisconnect))
                {
                    // ���������� ��������� �������� � �������� ������������ List<ModelUserAllInfo> usersAllInfoList
                    // � ����� ���������� ���������
                    int posUserName = clientsList.IndexOf(remoteEP); // ���������� ��������� � �������� ������� ����� (���������� ������)

                    ModelUserAllInfo userToRemove = usersAllInfoList[posUserName];
                    string userNameToRemove = usersNamesList[posUserName]; // ��������� ����� � ��������� ����� (�� ����� ������������ ��������)

                    // ³������� ����������� ��� ��'������� �볺��� - � (�������� ���) � ���, �� ���������� �볺�� �� ������ ������
                    // ��������� ����������� ��� �������� �볺��� 
                    ChatLibrary.Message messageUserOutFromChat = creator.CreateMessage(
                        $"{SystemInfo.SystemInfo_MessageUserLeftTheChat}{userToRemove.UserLogin}" +
                        $"{SystemInfo.SystemInfo_Separator}" +
                        $"{SystemInfo.SystemInfo_MessageNumberOfChatParticipants}{UsersInChat(userToRemove.CurrentChatId) - 1}", // -1
                        SystemInfo.SystemInfo_UserDisconnect, server.Id, messageReceived.ChatsId);

                    // ������������� � ������ ��� �������� MessageForSending
                    MessageForSending messageeForSendingUserOutFromChat = creator.ConvertMessageToMessageForSending(messageUserOutFromChat, server.Login, messageForSendingFormatReceived.ChatsName);

                    // ��������� ����� ��� �����������, �� ����������� � ��������
                    clientsList.Remove(remoteEP); // ��������� ������ �����
                    usersNamesList.Remove(userNameToRemove); // ��������� ����� � �������� �����
                    usersAllInfoList.Remove(userToRemove); // ��������� � �������� ������������

                    // ���������� �����������
                    string messageeForSendingUserOutFromChatJson = JsonSerializer.Serialize<MessageForSending>(messageeForSendingUserOutFromChat);

                    // ³������� �����������
                    // - � ��� � ����� ��������� �볺�� �� ������ ������ (id ���� ��������� � ����������)
                    SendMessagesAllUsers(messageeForSendingUserOutFromChatJson, messageReceived.ChatsId);

                    // �������� ����������� �� �������� ����������
                    messagesList.Add(messageUserOutFromChat);

                    // ���������� ����������� �� ��
                    databaseService.SaveMessageToDb(messageUserOutFromChat);

                    // ��������� ������������ ����������� 
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageUserOutFromChat));
                }
            }

        }

        // ϳ��������� ����������� �� ������ ����
        private void UserConnectingToANewChat(int posUserName, 
            int newChatId, 
            int oldChatId, string oldChatName, 
            string systemInfo_AboutChatConnecting, 
            ModelUserAllInfo userAllInfo_New, 
            string accompanyingMessage)
        {

            // ����������, �� ���� �� ����� ������ ��� �� ����� ���� ����������� �볺��
            Chat newChat = databaseService.InitChat(newChatId);

            // ���������� ����������� ��� �����/������� ���������� �� ����
            string messageAboutConnectionToChat = systemInfo_AboutChatConnecting; // ����������� ��� ����� ���������� �� ����
            if (newChat == null)
            {
                messageAboutConnectionToChat = SystemInfo.SystemInfo_ChatNotConnected; // � ��� ��������� ���� - ����������� ��� ������� ���������� �� ����
            }

            // ��������� ����������� from server to client ��� ���� (�����������) ���������� �� ����
            ChatLibrary.Message messageToUserConnectingToChat = creator.CreateMessage(
                SystemInfo.SystemInfo_EmptyMessageBody,
                messageAboutConnectionToChat, server.Id, serverSystemChat.Id);

            // ������������� ����������� (������-�볺��, �� �����������) � ������ ��� �������� MessageForSending
            MessageForSending messageForSendingToUserConnectingToChat = creator.ConvertMessageToMessageForSending(
                messageToUserConnectingToChat, server.Login, serverSystemChat.ChatName);

            // ���������� ��'���� ModelUserAllInfo - ��'���, �� ������ ����������� ������� ��� ��� �������� ���� ����������� 
            ModelUserAllInfo userAllInfo = userAllInfo_New;
            if(userAllInfo == null)
            {
                userAllInfo = databaseService.CreateUserForSending(
                    usersAllInfoList[posUserName].UserLogin,
                    usersAllInfoList[posUserName].UsersIPEndPoint,
                    (newChat == null ? oldChatId : newChat.Id),
                    (newChat == null ? oldChatName : newChat.ChatName),
                    usersNamesList);
            }

            // ��������� (����� �� ������������� ��'���) ��� ��� ����������� �� �������� ���������� ������������ (�������� ��'���� ModelUserAllInfo)
            usersAllInfoList[posUserName] = userAllInfo;

            // ������������� ��'���� ModelUserAllInfo � UserForSending - ������� ������������ �� �볺��� (���� ����������� �� ������ ��� ����� �������� ���)
            UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);

            // ���������� ��'���� UserForSending
            string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);

            // �������� ������������� ��'���� UserForSending �� ��'���� MessageForSending 
            messageForSendingToUserConnectingToChat.SystemBody = userAllInfoForSendingJson;
            // �������� �� ����������� ��������� ��� - ��������� ��� ������� ��������������
            // (��������� ����������� � ����� ������������ ����... - �������� ���������� ������)
            messageForSendingToUserConnectingToChat.Body = accompanyingMessage;

            // ���������� ��'���� MessageForSending ��� ����������� �볺���
            string messageForSendingToUserConnectingToChatJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingToUserConnectingToChat);

            // ³������� ����������� �����������, �� ����������� �� ���� - ����������� �볺��� ��� �����/������� ���������� �� ������ ����
            SendMessagesForUser(messageForSendingToUserConnectingToChatJson, usersAllInfoList[posUserName].UsersIPEndPoint);

            // �������� ����������� �� �������� ����������
            messagesList.Add(messageToUserConnectingToChat);

            // ���������� ������������ ����������� �� ��
            databaseService.SaveMessageToDb(messageToUserConnectingToChat);

            // ³���������� ������������ ����������� (��� ����� ���������� �볺��� �� ����) �� ����� ���������� �������
            tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageToUserConnectingToChat));

            // ����������� � ��������� ��� ��� ����� ����������� - ������ ���������� ���� ������ �볺��� � ����
            if (newChat != null)
            {
                // ³���������� � ����� ��� ����������� ��� ��������� �볺���
                // - ��������� ����������� ��� ��������� �볺��� 
                Thread.Sleep(3000);
                ChatLibrary.Message messageUserConnectedToChat = creator.CreateMessage(
                    $"{SystemInfo.SystemInfo_MessageJoinedTheChat} {usersAllInfoList[posUserName].UserLogin}" +
                    $"{SystemInfo.SystemInfo_Separator}" +
                    $"{SystemInfo.SystemInfo_MessageNumberOfChatParticipants} {UsersInChat(newChat.Id)}", // +1
                    SystemInfo.SystemInfo_ServerMessage, server.Id, newChat.Id);

                // ������������� � ������ ��� �������� MessageForSending
                MessageForSending messageForSendingUserConnectedToChat = creator.ConvertMessageToMessageForSending(
                    messageUserConnectedToChat,
                    server.Login, newChat.ChatName);

                // - ���������� ����������� ��� ��������� �볺���
                string messageForSendingUserConnectedToChatJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingUserConnectedToChat);

                // -  �������� ����������� ��� ��������� �볺��� � ����� ���
                SendMessagesAllUsers(messageForSendingUserConnectedToChatJson, newChat.Id);

                // �������� ����������� �� �������� ����������
                messagesList.Add(messageUserConnectedToChat);

                // - ���������� �� �� ����������� ��� ��������� �볺��� � ����� ���
                databaseService.SaveMessageToDb(messageUserConnectedToChat);

                // - ��������� � ���� ���������� ���� ����������� ��� ��������� �볺���
                tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageUserConnectedToChat));
            }

        }


        // ��������� ����� ���������� ������������ ���� ������������ ����� � ��� ���-��������� (������/�������� ����, ������/�������� ������������ ����)
        private void UpdateUsersInfoAfterUsingChatEditor(int posUserName)
        {
            for (int i = 0; i < usersAllInfoList.Count(); i++)
            {
                // ������� �� ���������� �������� � ������� ���������� �� �볺���, �� ������ ���� � �������� ����
                // - ������� ������ ������ ���������� � ������ �����
                if(i == posUserName)
                {
                    continue;
                }

                // ������� ������� ��������� ����
                // ���������� ��������/��������� ����, �� ����� �� ����� ������ ���������� �볺�� - ���� ���� ��������
                // - � ��� �������� �볺��� ���� ������������� �� ���������� ��������� ����
                Chat chat = databaseService.InitChat(usersAllInfoList[i].CurrentChatId);
                string stringFromChat = "" + chat; // ����� ���������� ��'��� Chat � ������ ����� - ������� ����� �������, �� ������ � ��� ��������� ���� �� �������� Id � �� - ��������� ���� ��������� ���� � ������ ���� ���������� ��� �� ����� ����������

                // ������� ������� ��������� �볺��� � ����� ������������ ���� (��� ��� �� �����������)
                ChatUser chatUser = databaseService.InitChatUser(usersAllInfoList[i].UserId, usersAllInfoList[i].CurrentChatId);
                string stringFromChatUser = "" + chatUser; 

                int currentChatId = (stringFromChat.Length == 0 || stringFromChatUser.Length == 0 ? groupChat.Id : chat.Id);
                string currentChatName = (stringFromChat.Length == 0 || stringFromChatUser.Length == 0 ? groupChat.ChatName : chat.ChatName);

                // ��������� �������� ����� ��� ��� �볺���
                ModelUserAllInfo userAllInfo_New = databaseService.CreateUserForSending(
                    usersAllInfoList[i].UserLogin,
                    usersAllInfoList[i].UsersIPEndPoint,
                    currentChatId,
                    currentChatName,
                    usersNamesList);

                // �������� - ����� ����������� �볺��� ��� ���� ����������
                // (����������/���������� �� ����, �������������� �� ���������� ��������� ����)
                List<string> messagesForClientConnectionChange = new List<string>();

                // ������� ������� ²��������� �� ����
                foreach (var chatFromListOld in usersAllInfoList[i].ChatsList)
                {
                    if(userAllInfo_New.ChatsList.Where(c => c.Id == chatFromListOld.Id).Count() == 0)
                    {
                        // ����������� ��� �볺��� "��� ��������� �� ���� chatFromListOld.ChatName"
                        messagesForClientConnectionChange.Add($"{SystemInfo.SystemInfo_MessageYouHaveBeenDisconnectedFromTheChat} - {chatFromListOld.ChatName}"); // You have been disconnected from the chat
                    }
                }

                // ������� ������� ϲ��������� �� ����
                foreach (var chatFromListNew in userAllInfo_New.ChatsList)
                {
                    if(usersAllInfoList[i].ChatsList.Where(c => c.Id == chatFromListNew.Id).Count() == 0)
                    {
                        // ����������� ��� �볺��� "��� ��������� �� ���� chatFromListNew.ChatName"
                        messagesForClientConnectionChange.Add($"{SystemInfo.SystemInfo_MessageYouHaveBeenConnectedToTheChat} - {chatFromListNew.ChatName}"); // You have been connected to the chat
                    }
                }

                // �������������� �볺��� �� ��������� ���� � ��� �������� ���� � ����� �� ���������
                if(stringFromChat.Length == 0 || stringFromChatUser.Length == 0)
                {
                    // �������������� �볺��� �� ���������� ��������� ���� - ��� ������� ����, � ����� ��������� �볺��

                    // ³������� ����������� �볺��� ��� �������������� �� ���������� ��������� ���� � ���������� userAllInfo 
                    string systemInfo = stringFromChat.Length == 0 ? SystemInfo.SystemInfo_DeletedCurrentChat : SystemInfo.SystemInfo_RemovedFromCurrentChatUsers;

                    // ϳ��������� �� ������ ���� - ���������� ��������� ����
                    UserConnectingToANewChat(i, 
                        groupChat.Id, 
                        usersAllInfoList[i].CurrentChatId, 
                        usersAllInfoList[i].CurrentChatName, 
                        systemInfo, 
                        userAllInfo_New, 
                        SystemInfo.SystemInfo_MessageYouHaveBeenRemovedFromTheCurrentChat);
                }
                else
                {
                    // ��������� (����� �� ������������� ��'���) ��� ��� ����������� �� �������� ���������� ������������ (�������� ��'���� ModelUserAllInfo)
                    usersAllInfoList[i] = userAllInfo_New;

                    // ³������� ����������� �� ��������� ��� �볺���
                    // ��������� ����������� ��� �������� �볺��� 
                    ChatLibrary.Message messageNewUserAllInfo = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody,
                        SystemInfo.SystemInfo_UpdatedUserInformation, server.Id, serverSystemChat.Id);

                    // ������������� � ������ ��� �������� MessageForSending
                    MessageForSending messageForSendingNewUserAllInfo = creator.ConvertMessageToMessageForSending(
                        messageNewUserAllInfo, server.Login, serverSystemChat.ChatName
                        );

                    // �������� ��������� �� SystemBody 
                    // ������������� ��'���� ModelUserAllInfo � UserForSending - ������� ������������ �� �볺��� 
                    UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo_New);
                    // ���������� ��'���� UserForSending
                    string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);
                    // �������� ������������� ��'���� UserForSending �� ��'���� MessageForSending
                    messageForSendingNewUserAllInfo.SystemBody = userAllInfoForSendingJson;

                    // ���������� �����������
                    string messageForSendingNewUserAllInfoJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingNewUserAllInfo);

                    // ³������� �����������
                    SendMessagesForUser(messageForSendingNewUserAllInfoJson, usersAllInfoList[i].UsersIPEndPoint);

                    // �������� ����������� �� �������� ����������
                    messagesList.Add(messageNewUserAllInfo);

                    // ���������� ����������� �� ��
                    databaseService.SaveMessageToDb(messageNewUserAllInfo);

                    // ��������� ������������ ����������� 
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageNewUserAllInfo));
                }

                // ³������� ����������� �볺��� ��� ����������, ���������� �� ����
                foreach (string message in messagesForClientConnectionChange)
                {
                    // ��������� ����������� ��� �������� �볺��� 
                    ChatLibrary.Message messageNewUserAllInfo = creator.CreateMessage(
                        message,
                        SystemInfo.SystemInfo_ServerMessage, server.Id, serverSystemChat.Id);

                    // ������������� � ������ ��� �������� MessageForSending
                    MessageForSending messageForSendingNewUserAllInfo = creator.ConvertMessageToMessageForSending(
                        messageNewUserAllInfo, server.Login, serverSystemChat.ChatName
                        );

                    // ���������� �����������
                    string messageForSendingNewUserAllInfoJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingNewUserAllInfo);

                    // ³������� �����������
                    SendMessagesForUser(messageForSendingNewUserAllInfoJson, usersAllInfoList[i].UsersIPEndPoint);

                    // �������� ����������� �� �������� ����������
                    messagesList.Add(messageNewUserAllInfo);

                    // ���������� ����������� �� ��
                    databaseService.SaveMessageToDb(messageNewUserAllInfo);

                    // ��������� ������������ ����������� 
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageNewUserAllInfo));
                    Thread.Sleep(500);
                }

            }

        }


        // ��������� ����� ���� � ������ ����� � �������� ChatLibrary.Message 
        private string GetHistoryString(List<ChatLibrary.Message> messages, int creatorUserId)
        {
            StringBuilder chatHistoryString = new StringBuilder();
            
            // ��������� �����������, ���� ������� ������ ���� (��� ������ �� ���� BlackList)
            ModelUserAllInfo user = usersAllInfoList.FirstOrDefault(u => u.UserId == creatorUserId);
            foreach (var message in messages)
            {
                if(user.BlackList.Where(b => b.Id == message.CreatorUserId).Count() == 0)
                {
                    chatHistoryString.Append(GetFullMessageLong(message).Trim());
                    chatHistoryString.AppendLine(SystemInfo.SystemInfo_Separator);
                }
            }
            return chatHistoryString.ToString();
        }

        // ��������� ����� ���������� � ������ ����� ��
        // ������������ ���������� ������� � ��������� ���������
        private void LoadChatHistoryToServerStatistic()
        {
            // ��������� ������ ����������, ��������� � ��, � ���� ���������� ������� 
            StringBuilder sb = new StringBuilder();
            foreach (ChatLibrary.Message message in messagesList)
            {
                sb.Append(GetFullMessageLong(message).Trim());
                sb.AppendLine(SystemInfo.SystemInfo_Separator);
            }
            tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), sb.ToString().Trim());
        }

        // ʳ������ �������� ���� �� Id ����
        private int UsersInChat(int chatsId)
        {
            return usersAllInfoList.Where(u => u.CurrentChatId == chatsId).Count();
        }

        private void ControlOfMessageContent(ChatLibrary.Message message)
        {
            if (message.CreatorUserId == 0)
            {
                message.CreatorUserId = userTemp.Id;
            }
            if (message.ChatsId == 0)
            {
                message.ChatsId = serverSystemChat.Id;
            }
        }

        // ϳ�������� ����������� ��� ��������� � ��� �������
        private string GetFullMessageLong(ChatLibrary.Message message)
        {
            User creator = usersList.FirstOrDefault(u => u.Id == message.CreatorUserId, new User());
            Chat currentChat = message.ChatsId == 0 ? new Chat() : chatsList.FirstOrDefault(c => c.Id == message.ChatsId, new Chat());
            return $"{message.SendingTime.ToString()} - from {creator?.Login} - chat {currentChat?.ChatName} \r\n- {message.SystemInfo} \r\n- {message.Body}";
        }


        // ³������� ���������� �� ��� ���������� �� ��������� ���� �볺���
        private void SendMessagesAllUsers(string messageForUsers, int chatsId, int userMessageCreator = 0)
        {
            
            foreach (var user in usersAllInfoList)
            {
                // �������� �������� ���������� � ������� ������ ���������� - ������� �� �� �������� �����������
                if(userMessageCreator != 0 && user.BlackList.Where(u => u.Id == userMessageCreator).Count() != 0)
                {
                    continue;
                }
                // ��������� �����������
                if (user.CurrentChatId == chatsId)
                {
                    SendMessagesForUser(messageForUsers, user.UsersIPEndPoint);
                }
            }
        }

        // ³������� ���������� �� ��� ���������� �볺���
        private void SendMessagesAllUsers(string message)
        {
            foreach (var user in usersAllInfoList)
            {
                SendMessagesForUser(message, user.UsersIPEndPoint);
            }
        }

        // ³������� ���������� �� 1 �볺���
        private void SendMessagesForUser(string textSerialized, IPEndPoint? remoteEP)
        {
            byte[] data = Encoding.Default.GetBytes(textSerialized);
            UdpClient client = new UdpClient();
            try
            {
                client.Send(data, data.Length, remoteEP);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        // ϳ�������� �� ����������� �������� ���������� � ���������� ��������� �������
        private void AddTextToTbServerStatistics(string str)
        {
            StringBuilder sb = new StringBuilder(tbServerStatistics.Text);
            sb.Append(str.Trim());
            sb.AppendLine(SystemInfo.SystemInfo_Separator); // "\r\n"
            tbServerStatistics.Text = sb.ToString();
            tbServerStatistics.SelectionStart = tbServerStatistics.Text.Length; // ���������� ������� � ����� ������
            tbServerStatistics.ScrollToCaret(); // ������� ���� �� ������� ������� (� ����� ��� �� ���� ������)
        }

        // ϳ�������� �� ����������� �������� ���������� � ���������� ��������� �������
        // - ��� ��������� �� ���������� ����������
        private void AddNewTextToTbServerStatistics(string str)
        {
            StringBuilder sb = new StringBuilder(str.Trim());
            sb.AppendLine(SystemInfo.SystemInfo_Separator); // "\r\n"
            tbServerStatistics.Text = sb.ToString();
            tbServerStatistics.SelectionStart = tbServerStatistics.Text.Length; // ���������� ������� � ����� ������
            tbServerStatistics.ScrollToCaret(); // ������� ���� �� ������� ������� (� ����� ��� �� ���� ������)
        }

        private void btnAdditionalInfo_Click(object sender, EventArgs e)
        {
            FormAdditionalInfo form = new FormAdditionalInfo(usersNamesList);
            form.ShowDialog();
        }

        private async void Form1_LoadAsync(object sender, EventArgs e)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                await context.Messages.LoadAsync();
                //messagesList = context.Messages.ToList();
                messagesList = context.Messages.Where(m => m.SendingTime >= startDateToLoadChatHistory).ToList();
                messagesListCount = messagesList.Count;

                await context.Users.LoadAsync();
                usersList = context.Users.ToList();

                await context.Chats.LoadAsync();
                chatsList = context.Chats.ToList();
            }


            // �� ����'������ - ������ ������� ���� ������� ����� (��� ��������) - ������ Client �� ���� ������ �� �����������
            // TODO
            Process.Start("Client_UdpClient.exe");
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Process.Start("Client_UdpClient.exe");
        }

        // ������������ ����� ����
        private async void LoadChatHistoryAsync()
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                await context.Messages.LoadAsync();
                messagesList.Clear();
                messagesList = context.Messages.Where(m => m.SendingTime.Date >= startDateToLoadChatHistory.Date).ToList();
                messagesListCount = messagesList.Count;

                tbServerStatistics.BeginInvoke(new Action<string>(AddNewTextToTbServerStatistics), "");
                LoadChatHistoryToServerStatistic();
            }
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ��������� ������������ ��� ���������� ������ �������
            // ��������� ����������� 
            ChatLibrary.Message messageServerShutDown = creator.CreateMessage(
                $"{SystemInfo.SystemInfo_MessageServerShutDown} {DateTime.Now.ToString()} \r\n-------------------------------------------",
                SystemInfo.SystemInfo_ServerShutDown,
                server.Id, groupChat.Id);
            // ��������� ����������� �� �������� ���������� (�� ������� ������ �������� ���������� �������������, ������������� � ��)
            messagesList.Add(messageServerShutDown);
            // ���������� ����������� �� �� 
            databaseService.SaveMessageToDb(messageServerShutDown);
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            ChatLibrary.Message messageModel = new ChatLibrary.Message();
            messageModel.SendingTime = startDateToLoadChatHistory;
            FormCreateChatHistoryStartDate form = new FormCreateChatHistoryStartDate(messageModel);
            if(form.ShowDialog() == DialogResult.OK)
            {
                startDateToLoadChatHistory = messageModel.SendingTime;

                btnHistory.Text = $"History from {messageModel.SendingTime.ToShortDateString()}";

                LoadChatHistoryAsync();
            }
        }
    }
}

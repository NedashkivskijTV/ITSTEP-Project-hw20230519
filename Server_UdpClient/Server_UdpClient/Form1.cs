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

        // Колекція кінцевих точок - підключених клієнтів
        List<IPEndPoint> clientsList = new List<IPEndPoint>();
        // Колекція логінів підключених користувачів
        List<string> usersNamesList = new List<string>();
        // Колекція Id чатів, що зарааз активні

        // Колекція об'єктів UserForSending, що містить усю інф, що зберігають3 попередні колекції
        List<ModelUserAllInfo> usersAllInfoList = new List<ModelUserAllInfo>();

        // Користувач з логіном "server"
        User? server;

        // Об'єкти, що використовуються при системних діалогах клієнтів та сервера
        // (підставляються у разі відсутності Id коритувача та Id чата)
        User? userTemp;
        Chat? serverSystemChat;

        // Груповий чат
        Chat? groupChat;

        // Об'єкт для взаємодії з БД
        DatabaseService? databaseService;
        // Об'єкт, що створює сутності БД
        ItemCreator? creator;


        // БД усіх повідомлень - підвантажується при старті, додається під час роботи, зберігається при завершенні роботи
        List<ChatLibrary.Message> messagesList = new List<ChatLibrary.Message>();
        // Кількість повідомлень у БД на момент завантаження та старту сервера 
        int messagesListCount = 0;

        // БД усіх користувачів - підвантажується при старті, додається під час роботи, зберігається при завершенні роботи
        List<User> usersList = new List<User>();
        // БД усіх чатів (в усіх користувачів) - підвантажується при старті, додається під час роботи, зберігається при завершенні роботи
        List<Chat> chatsList = new List<Chat>();

        // Початкова дата для завантаження сервером історії (за замовчуванням - 1 день)
        DateTime startDateToLoadChatHistory;

        public Form1()
        {
            InitializeComponent();

            InitData();

            InitSystemDataInDb();
        }

        // Ініціалізація даних та об'єктів
        private void InitData()
        {
            databaseService = new DatabaseService();
            creator = new ItemCreator();
            startDateToLoadChatHistory = DateTime.Today;
        }

        // Ініціалізація (або створення у разі відсутності) системних об'єктів БД (користувачів, чатів)
        private void InitSystemDataInDb()
        {
            server = databaseService.InitServer();
            userTemp = databaseService.InitOrCreateUser(SystemUsers.SystemUser_UserTemp);
            groupChat = databaseService.InitGroupChat();
            serverSystemChat = databaseService.InitOrCreateChat(SystemInfo.SystemInfo_ServerSystemChat, SystemUsers.SystemUser_Server);
        }

        // Алгоритм запуску методу - слухача
        private void btnStartServer_Click(object sender, EventArgs e)
        {
            Task.Run(Listener);

            btnStartServer.Enabled = false;
        }

        // Прослуховування відправлених клієнтами повідомлень
        private void Listener()
        {
            UdpClient listener = new UdpClient(new IPEndPoint(IPAddress.Parse(multicastGroupIp), localPort));
            IPEndPoint remoteEP = null;

            Text = "Server was started !";

            // Завантаження історії чату
            LoadChatHistoryToServerStatistic();

            // Створення повідомлення =============================================================================================
            ChatLibrary.Message messageServerStarted = creator.CreateMessage(
                $"{SystemInfo.SystemInfo_MessageServerStarted} {DateTime.Now.ToString()}",
                SystemInfo.SystemInfo_ServerStarted,
                server.Id, groupChat.Id);
            // Відправка повідомлення
            // Відображення повідомлення у вікні статистики сервера
            tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageServerStarted));
            // Додавання повідомлення до колекції повідомлень (на початку роботи колекція ініціюється повідомленнями, завантаженими з БД)
            messagesList.Add(messageServerStarted);
            // Збереження повідомлення до БД !!!!!!!
            databaseService.SaveMessageToDb(messageServerStarted);

            //int hash = messageServerStarted.Body.GetHashCode();
            //MessageBox.Show("" + messageServerStarted.Body + " - " + hash);

            // Цикл прослуховування
            while (true)
            {
                // Отримання повідомлення
                byte[] buffer = listener.Receive(ref remoteEP);
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(Encoding.Default.GetString(buffer));

                // Перетворення отриманого від клієнта повідомлення у формат рядка
                string clientMessage = builder.ToString();

                // Десеріалізація повідомлення у об'єкт типу MessageForSending
                MessageForSending messageForSendingFormatReceived = JsonSerializer.Deserialize<MessageForSending>(clientMessage);

                // Конвертування MessageForSending у об'єкт Message
                ChatLibrary.Message messageReceived = creator.ConvertMessageForSendingToMessage(messageForSendingFormatReceived);

                // Перевірка заповненості об'єкта Message - Id користувача та чату мають бути реальними !!!
                // (у разі відсутності підставляються значення відовідних системних об'єктів)
                ControlOfMessageContent(messageReceived);


                // Додавання повідомлення до колекції повідомлень (на початку роботи колекція ініціюється повідомленнями, завантаженими з БД)
                messagesList.Add(messageReceived);

                // Збереження отриманого повідомлення до БД
                databaseService.SaveMessageToDb(messageReceived);

                // Виведення отриманого повідомлення (від клієнта до сервера)

                tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageReceived));


                // Алгоритм обробки запиту клієнта на підключення (отримано логін та пароль)
                if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_AuthorizationUser))
                {
                    // Запит до БД на наявність логіна та пароля - Авторизація користувача
                    if (!databaseService.AuthorizationUser(messageForSendingFormatReceived.CreatorUserLogin, messageForSendingFormatReceived.HashBody)) // невдала авторизація
                    {
                        // НЕВДАЛА АВТОРИЗАЦІЯ
                        // Створення повідомлення для відправки клієнту про НЕвдале підключення до чату (невірний логін або пароль)
                        ChatLibrary.Message messageWrongAuthorize = creator.CreateMessage(
                            SystemInfo.SystemInfo_EmptyMessageBody,
                            SystemInfo.SystemInfo_AuthorizUserFalse, server.Id, serverSystemChat.Id);

                        // Конвертування у формат для відправки - MessageForSending
                        MessageForSending messageForSendingWrongAuthorize = creator.ConvertMessageToMessageForSending(messageWrongAuthorize, server.Login, serverSystemChat.ChatName);

                        // Серіалізація повідомлення
                        string messageForSendingWrongAuthorizeJson = JsonSerializer.Serialize(messageForSendingWrongAuthorize);

                        // Відправка повідомлення
                        SendMessagesForUser(messageForSendingWrongAuthorizeJson, remoteEP);

                        // Доєднання повідомлення до колекції повідомлень
                        messagesList.Add(messageWrongAuthorize);

                        // Збереження повідомлення до БД
                        databaseService.SaveMessageToDb(messageWrongAuthorize);

                        // Виведення отриманого повідомлення (від сервера до клієнта)
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageWrongAuthorize));
                    }
                    else
                    {
                        // ВДАЛА АВТОРИЗАЦІЯ - логін та пароль знайдено - користувач підключаєтся,
                        // а йому відправляється об'єкт UserForSending, який відповідає даному
                        // (підключеному) користувачу та включає всю повноту потрібної для робти інф
                        // в т.ч. колекції
                        // - чатів, що ним створені (інф про чат містить Id, назву чата, колекцію учасників чату),
                        // - чатів до яких він має доступ (інф про чат містить Id, назву чата, колекцію учасників чату),
                        // - юзерів, що входять до чорного списку (інф про юзера включає логін, Id, статус)

                        // Додавання логіна користувача (нового) до колекції підключених логінів
                        usersNamesList.Add(messageForSendingFormatReceived.CreatorUserLogin.Trim()); // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                        // Додавання кінцевої точки користувача (нового) до колекції підключених користувачів
                        clientsList.Add(remoteEP); // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                        // Створення Повідомлення from server to client про його (користувача) вдале підключення до чату
                        ChatLibrary.Message messageToUserAuthorizUserTrue = creator.CreateMessage(SystemInfo.SystemInfo_EmptyMessageBody, SystemInfo.SystemInfo_AuthorizUserTrue, server.Id, serverSystemChat.Id);

                        // Конвертування повідомлення (сервер-клієнт, що підключається) у формат для відправки MessageForSending
                        MessageForSending messageForSendingToUserAuthorizUserTrue = creator.ConvertMessageToMessageForSending(messageToUserAuthorizUserTrue, server.Login, serverSystemChat.ChatName);

                        // Формування об'єкта ModelUserAllInfo - об'єкт, що містить максимальну кількість інф про поточний стан користувача 
                        // В т.ч. з Визначенням хто з користувачів перебуває у мережі - з двох колекцій 
                        // - userNew.chatsList.UsersInChat - два цикли
                        // - userNew.blackList - один цикл
                        //CheckingThePresenceOfUsersInTheNetwork(userNew);
                        ModelUserAllInfo userNew = databaseService.CreateUserForSending(
                            messageForSendingFormatReceived.CreatorUserLogin,
                            remoteEP,
                            groupChat.Id,
                            groupChat.ChatName,
                            usersNamesList);

                        //Додавання інф про користувача до колекції підключених користувачів (колекція об'єктів ModelUserAllInfo)
                        // - Спроба використовувати одну колекцію, що містить весь потрібний масив даних для обслуговування роботи програми
                        usersAllInfoList.Add(userNew);

                        // Конвертування об'єкта ModelUserAllInfo у UserForSending - останній пересилається до клієнта (який підключається та містить про нього максимум інф)
                        UserForSending userNewForSending = creator.ConvertModelUserAllInfoToUserForSending(userNew);

                        // Серіалізація об'єкта UserForSending
                        string userNewForSendingJson = JsonSerializer.Serialize<UserForSending>(userNewForSending);

                        // Доєднання серіалізованого об'єкта UserForSending до об'єкта MessageForSending 
                        messageForSendingToUserAuthorizUserTrue.SystemBody = userNewForSendingJson;

                        // Серіалізація об'єкта MessageForSending для відправлення клієнту
                        string messageUserAuthorizTrueJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingToUserAuthorizUserTrue);

                        // Відправка повідомлення
                        SendMessagesForUser(messageUserAuthorizTrueJson, remoteEP);

                        // Доєднання повідомлення до колекції повідомлень
                        messagesList.Add(messageToUserAuthorizUserTrue);

                        // Збереження відправленого повідомлення до БД
                        databaseService.SaveMessageToDb(messageToUserAuthorizUserTrue);

                        // Відображення відправленого повідомлення (про вдале підключення клієнта до чату) на екрані статистики сервера
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageToUserAuthorizUserTrue));

                        // Відправлення у груповий чат повідомлення про приєднання клієнта
                        // - створення повідомлення про приєднання клієнта -----------------------------------------------------------------------------------
                        ChatLibrary.Message messageUserAuthorizeTrueForAll = creator.CreateMessage(
                            $"{SystemInfo.SystemInfo_MessageJoinedTheChat}{messageForSendingFormatReceived.CreatorUserLogin}" +
                            $"{SystemInfo.SystemInfo_Separator}" +
                            $"{SystemInfo.SystemInfo_MessageNumberOfChatParticipants} {UsersInChat(userNew.CurrentChatId)}" +
                            $"{SystemInfo.SystemInfo_Separator}",
                            SystemInfo.SystemInfo_ServerMessage, server.Id, groupChat.Id);

                        // Конвертування у формат для відправки MessageForSending
                        MessageForSending messageForSendingUserAuthorizeTrueForAll = creator.ConvertMessageToMessageForSending(messageUserAuthorizeTrueForAll, server.Login, groupChat.ChatName);

                        // Cеріалізація повідомлення про приєднання клієнта
                        string messageForSendingUserAuthorizeTrueForAllJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingUserAuthorizeTrueForAll);

                        // Відправка повідомлення про приєднання клієнта у груповий чат
                        //SendMessagesAllUsers($"{DateTime.Now}\r\nДо чату приєднався :{clientMessage.Substring(20)}" + $"Клієнтів у чаті: {clientsList.Count}\r\n") ;
                        SendMessagesAllUsers(messageForSendingUserAuthorizeTrueForAllJson);

                        // Доєднання повідомлення до колекції повідомлень
                        messagesList.Add(messageUserAuthorizeTrueForAll);

                        // Збереження до БД повідомлення про приєднання клієнта у груповий чат
                        databaseService.SaveMessageToDb(messageUserAuthorizeTrueForAll);

                        // Виведення у поле статистики чату повідомлення про приєднання клієнта
                        //tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), $"{DateTime.Now}\r\nДо чату приєднався :{clientMessage.Substring(20)}" + $"Клієнтів у чаті: {clientsList.Count}\r\n");
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageUserAuthorizeTrueForAll));
                    }
                }
                // Алгоритм обробки повідомлень про реєстрацію клієнта
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_RegistrationUser))
                {
                    string systemBodyAboutUserRegistration = SystemInfo.SystemInfo_RegistrationUserTrue; // провідомлення про ВДАЛУ реєстрацію
                    if (!databaseService.RegistrationUser(messageForSendingFormatReceived.CreatorUserLogin, messageForSendingFormatReceived.HashBody))
                    {
                        // НЕВДАЛА реєстрація (логін вже використовується)
                        systemBodyAboutUserRegistration = SystemInfo.SystemInfo_RegistrationUserFalse;
                    }

                    // Створення повідомлення для відправки клієнту про результат реєстрації 
                    ChatLibrary.Message messageWrongRegistration = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody,
                        systemBodyAboutUserRegistration,
                        server.Id, serverSystemChat.Id);

                    // Конвертування у формат для відправки - MessageForSending
                    MessageForSending messageForSendingWrongRegistration = creator.ConvertMessageToMessageForSending(
                        messageWrongRegistration, server.Login, serverSystemChat.ChatName);

                    // Серіалізація повідомлення
                    string messageForSendingWrongRegistrationJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingWrongRegistration);

                    // Відправка повідомлення
                    SendMessagesForUser(messageForSendingWrongRegistrationJson, remoteEP);

                    // Доєднання повідомлення до колекції повідомлень
                    messagesList.Add(messageWrongRegistration);

                    // Збереження повідомлення до БД
                    databaseService.SaveMessageToDb(messageWrongRegistration);

                    // Виведення отриманого повідомлення (від сервера до клієнта)
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageWrongRegistration));
                }
                // Алгоритм обробки повідомлень підключеного клієнта
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UserMessage))
                {
                    // Розсилка надісланого клієнтом повідомлення усім підключеним клієнтам у вказаному у повідомленні чаті
                    SendMessagesAllUsers(clientMessage, messageReceived.ChatsId, messageReceived.CreatorUserId); // відправка у вказаний у повідомленні чат
                }
                // Алгоритм обробки запиту на підключення клієнта до іншого (вказаного у повідомленні чату)
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_ConnectChat))
                {

                    // Визначення положення елемента у колекції користувачів List<ModelUserAllInfo> usersAllInfoList
                    // з метою подальшого оновлення інф та перепідключення до іншого чату
                    int posUserName = clientsList.IndexOf(remoteEP); // визначення положення у колекції кінцевих точок (отримується індекс)

                    // Дані про попередній чат
                    int oldChatId = usersAllInfoList[posUserName].CurrentChatId;
                    string oldChatName = usersAllInfoList[posUserName].CurrentChatName;
                    
                    // Визначення, чи існує на даний момент чат до якого бажає підключитись клієнт
                    Chat newChat = databaseService.InitChat(messageReceived.ChatsId);

                    // Формування повідомлення про вдале/НЕвдале підключення до чату
                    string messageAboutConnectionToChat = SystemInfo.SystemInfo_ChatIsConnected; // повідомлення про вдале підключення до чату
                    if (newChat == null)
                    {
                        messageAboutConnectionToChat = SystemInfo.SystemInfo_ChatNotConnected; // у разі відсутності чату - повідомлення про НЕВДАЛЕ підключення до чату
                    }
                    // Створення Повідомлення from server to client про його (користувача) підключення до чату
                    ChatLibrary.Message messageToUserConnectingToChat = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody,
                        messageAboutConnectionToChat, server.Id, serverSystemChat.Id);

                    // Конвертування повідомлення (сервер-клієнт, що підключається) у формат для відправки MessageForSending
                    MessageForSending messageForSendingToUserConnectingToChat = creator.ConvertMessageToMessageForSending(
                        messageToUserConnectingToChat, server.Login, serverSystemChat.ChatName);

                    // Формування об'єкта ModelUserAllInfo - об'єкт, що містить максимальну кількість інф про поточний стан користувача 
                    ModelUserAllInfo userAllInfo = databaseService.CreateUserForSending(
                        usersAllInfoList[posUserName].UserLogin,
                        usersAllInfoList[posUserName].UsersIPEndPoint,
                        (newChat == null ? oldChatId : newChat.Id),
                        (newChat == null ? oldChatName : newChat.ChatName),
                        usersNamesList);

                    // Додавання (заміна на актуалізований об'єкт) інф про користувача до колекції підключених користувачів (колекція об'єктів ModelUserAllInfo)
                    usersAllInfoList[posUserName] = userAllInfo;

                    // Конвертування об'єкта ModelUserAllInfo у UserForSending - останній пересилається до клієнта (який підключається та містить про нього максимум інф)
                    UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);

                    // Серіалізація об'єкта UserForSending
                    string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);

                    // Доєднання серіалізованого об'єкта UserForSending до об'єкта MessageForSending 
                    messageForSendingToUserConnectingToChat.SystemBody = userAllInfoForSendingJson;

                    // Серіалізація об'єкта MessageForSending для відправлення клієнту
                    string messageForSendingToUserConnectingToChatJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingToUserConnectingToChat);

                    // Відправка повідомлення користувачу, що підключається до чату - повідомлення клієнту про вдале/НУвдале підключення до нового чату
                    SendMessagesForUser(messageForSendingToUserConnectingToChatJson, remoteEP);

                    // Доєднання повідомлення до колекції повідомлень
                    messagesList.Add(messageToUserConnectingToChat);

                    // Збереження відправленого повідомлення до БД
                    databaseService.SaveMessageToDb(messageToUserConnectingToChat);

                    // Відображення відправленого повідомлення (про вдале підключення клієнта до чату) на екрані статистики сервера
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageToUserConnectingToChat));

                    // Повідомлення у попередній чат про вихід користувача - бажано публікувати після виходу клієнта з чату
                    if (newChat != null)
                    {
                        // Відправка повідомлення про від'єднання клієнта - від чату
                        // Створення повідомлення для відправки у чат 
                        ChatLibrary.Message messageUserOutFromChat = creator.CreateMessage(
                            $"{SystemInfo.SystemInfo_MessageUserLeftTheChat}{usersAllInfoList[posUserName].UserLogin}" +
                            $"{SystemInfo.SystemInfo_Separator}" +
                            $"{SystemInfo.SystemInfo_MessageNumberOfChatParticipants}{UsersInChat(oldChatId)}", // -1
                            SystemInfo.SystemInfo_UserDisconnect, server.Id, oldChatId);

                        // Конвертування у формат для відправки MessageForSending
                        MessageForSending messageeForSendingUserOutFromChat = creator.ConvertMessageToMessageForSending(
                            messageUserOutFromChat,
                            server.Login, oldChatName);

                        // Серіалізація повідомлення
                        string messageeForSendingUserOutFromChatJson = JsonSerializer.Serialize<MessageForSending>(messageeForSendingUserOutFromChat);

                        // Відправка повідомлення - відправляється після повідомлення клієнту про вдале підключення до нового чату
                        // - у чат в якому перебував клієнт на момент виходу (id чату приходить у повідомленні)
                        SendMessagesAllUsers(messageeForSendingUserOutFromChatJson, oldChatId);

                        // Доєднання повідомлення до колекції повідомлень
                        messagesList.Add(messageUserOutFromChat);

                        // Збереження повідомлення до БД
                        databaseService.SaveMessageToDb(messageUserOutFromChat);

                        // Виведення відправленого повідомлення 
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageUserOutFromChat));

                        // Відправлення у новий чат повідомлення про приєднання клієнта
                        // - створення повідомлення про приєднання клієнта 
                        ChatLibrary.Message messageUserConnectedToChat = creator.CreateMessage(
                            $"{SystemInfo.SystemInfo_MessageJoinedTheChat} {usersAllInfoList[posUserName].UserLogin}" +
                            $"{SystemInfo.SystemInfo_Separator}" +
                            $"{SystemInfo.SystemInfo_MessageNumberOfChatParticipants} {UsersInChat(newChat.Id)}", // +1
                            SystemInfo.SystemInfo_ServerMessage, server.Id, newChat.Id);

                        // Конвертування у формат для відправки MessageForSending
                        MessageForSending messageForSendingUserConnectedToChat = creator.ConvertMessageToMessageForSending(
                            messageUserConnectedToChat,
                            server.Login, newChat.ChatName);

                        // - серіалізація повідомлення про приєднання клієнта
                        string messageForSendingUserConnectedToChatJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingUserConnectedToChat);

                        // -  відправка повідомлення про приєднання клієнта у новий чат
                        SendMessagesAllUsers(messageForSendingUserConnectedToChatJson, newChat.Id);

                        // Доєднання повідомлення до колекції повідомлень
                        messagesList.Add(messageUserConnectedToChat);

                        // - збереження до БД повідомлення про приєднання клієнта у новий чат
                        databaseService.SaveMessageToDb(messageUserConnectedToChat);

                        // - виведення у поле статистики чату повідомлення про приєднання клієнта
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageUserConnectedToChat));
                    }
                    
                }
                // Алгоритм обробки запитів клієнта на отримання історії чату
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_GetChatHistory))
                {
                    // Отримання стартової дати для вигрузки історії чату
                    DateTime dateStartHistory = JsonSerializer.Deserialize<DateTime>(messageForSendingFormatReceived.SystemBody);

                    // Вигрузка історії чату
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

                        // Формування історії чату у вигляді рядка
                        string chatHistory = GetHistoryString(subMessagesList, messageReceived.CreatorUserId);

                        // Формування параметра systemInfo в залежності від кількості пакетів
                        // - відрізняється для першого та наступних
                        string systemInfo = SystemInfo.SystemInfo_NewChatHistoryContinue;
                        if (firstPackage)
                        {
                            systemInfo = SystemInfo.SystemInfo_NewChatHistoryStart;
                            firstPackage = false;
                        }

                        // Створення повідомлення для відправки клієнту 
                        ChatLibrary.Message messageChatHistory = creator.CreateMessage(
                            SystemInfo.SystemInfo_EmptyMessageBody,
                            systemInfo, server.Id, messageReceived.ChatsId);

                        // Конвертування у формат для відправки MessageForSending
                        MessageForSending messageForSendingChatHistory = creator.ConvertMessageToMessageForSending(
                            messageChatHistory, server.Login, messageForSendingFormatReceived.ChatsName);

                        // Можливо - доєднання сутностей SystemBody та HashBody
                        messageForSendingChatHistory.SystemBody = chatHistory;

                        // Серіалізація повідомлення
                        string messageForSendingChatHistoryJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingChatHistory);

                        // Відправка повідомлення
                        SendMessagesForUser(messageForSendingChatHistoryJson, remoteEP);

                        // Доєднання повідомлення до колекції повідомлень
                        messagesList.Add(messageChatHistory);

                        // Збереження повідомлення до БД
                        databaseService.SaveMessageToDb(messageChatHistory);

                        // Виведення відправленого повідомлення у статистику сервера
                        tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageChatHistory));
                    }
                }
                // Алгоритм обробки запитів на відображення Списку користувачів чату
                // - обробки запитів клієнта на оновлення даних про себе
                // - відправка об'єкта у разі неактуальності даних на боці клієнта (порівняння хеш-кодів)
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UpdateUserAllInfo))
                {
                    // Визначення положення елемента у колекції користувачів List<ModelUserAllInfo> usersAllInfoList
                    // з метою подальшого оновлення інф та перевірки актуальності даних
                    int posUserName = clientsList.IndexOf(remoteEP); // визначення положення у колекції кінцевих точок (отримується індекс)

                    // Формування об'єкта ModelUserAllInfo - об'єкт, що містить максимальну кількість інф про поточний стан користувача 
                    ModelUserAllInfo userAllInfo = databaseService.CreateUserForSending(
                        usersAllInfoList[posUserName].UserLogin,
                        usersAllInfoList[posUserName].UsersIPEndPoint,
                        usersAllInfoList[posUserName].CurrentChatId,
                        usersAllInfoList[posUserName].CurrentChatName,
                        usersNamesList);

                    // Визначення актуальності об'єкта ModelUserAllInfo userAllInfo з його відповідником на клієнті 
                    // - порівняння хеш-кодів
                    UserForSending userForSendingAllInfo = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);
                    string userAllInfoOnServer = userForSendingAllInfo.GetHashCode().ToString();

                    // Створення Повідомлення from server to client про актуальність даних або надсилання актуального об'єкта UserForSending
                    ChatLibrary.Message messageToUserLoadContactsList = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody,
                        SystemInfo.SystemInfo_UserAllInfoIsUpToDate, server.Id, serverSystemChat.Id);

                    // Конвертування повідомлення (сервер-клієнт) у формат для відправки MessageForSending
                    MessageForSending messageForSendingToUserLoadContactsList = creator.ConvertMessageToMessageForSending(
                        messageToUserLoadContactsList, server.Login, serverSystemChat.ChatName);
                    
                    // Коригування даних у разі, якщо інформація на клієнті не актуальна, з метою надсилання актуальної інф
                    if (messageForSendingFormatReceived.HashBody != userAllInfoOnServer)
                    {
                        messageToUserLoadContactsList.SystemInfo = SystemInfo.SystemInfo_SendingTheCurrentUserAllInfo;
                        messageForSendingToUserLoadContactsList.SystemInfo = SystemInfo.SystemInfo_SendingTheCurrentUserAllInfo;

                        // Додавання (заміна на актуалізований об'єкт) інф про користувача до колекції підключених користувачів (колекція об'єктів ModelUserAllInfo)
                        usersAllInfoList[posUserName] = userAllInfo;

                        // Конвертування об'єкта ModelUserAllInfo у UserForSending - останній пересилається до клієнта (який підключається та містить про нього максимум інф)
                        UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);
                        
                        // Серіалізація об'єкта UserForSending
                        string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);

                        // Доєднання серіалізованого об'єкта UserForSending до об'єкта MessageForSending 
                        messageForSendingToUserLoadContactsList.SystemBody = userAllInfoForSendingJson;
                    }

                    // Серіалізація об'єкта MessageForSending для відправлення клієнту
                    string messageForSendingToUserLoadContactsListJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingToUserLoadContactsList);

                    // Відправка повідомлення користувачу, що підключається до чату - повідомлення клієнту про вдале/НУвдале підключення до нового чату
                    SendMessagesForUser(messageForSendingToUserLoadContactsListJson, remoteEP);

                    // Доєднання повідомлення до колекції повідомлень
                    messagesList.Add(messageToUserLoadContactsList);

                    // Збереження відправленого повідомлення до БД
                    databaseService.SaveMessageToDb(messageToUserLoadContactsList);

                    // Відображення відправленого повідомлення (про вдале підключення клієнта до чату) на екрані статистики сервера
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageToUserLoadContactsList));
                }
                // Алгоритм обробки запитів на відображення Чорного Списку
                // - обробки запитів клієнта на оновлення даних про себе
                // - відправка об'єкта у разі неактуальності даних на боці клієнта (порівняння хеш-кодів)
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UpdateUserAllInfoForBlackList))
                {
                    // Визначення положення елемента у колекції користувачів List<ModelUserAllInfo> usersAllInfoList
                    // з метою подальшого оновлення інф та перевірки актуальності даних
                    int posUserName = clientsList.IndexOf(remoteEP); // визначення положення у колекції кінцевих точок (отримується індекс)

                    // Формування об'єкта ModelUserAllInfo - об'єкт, що містить максимальну кількість інф про поточний стан користувача 
                    ModelUserAllInfo userAllInfo = databaseService.CreateUserForSending(
                        usersAllInfoList[posUserName].UserLogin,
                        usersAllInfoList[posUserName].UsersIPEndPoint,
                        usersAllInfoList[posUserName].CurrentChatId,
                        usersAllInfoList[posUserName].CurrentChatName,
                        usersNamesList);

                    // Визначення актуальності об'єкта ModelUserAllInfo userAllInfo з його відповідником на клієнті 
                    // - порівняння хеш-кодів
                    UserForSending userForSendingAllInfo = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);
                    string userAllInfoOnServer = userForSendingAllInfo.GetHashCode().ToString();

                    // Створення Повідомлення from server to client про актуальність даних або надсилання актуального об'єкта UserForSending
                    ChatLibrary.Message messageToUserLoadBlackList = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody,
                        SystemInfo.SystemInfo_UserAllInfoIsUpToDateForBlackList, server.Id, serverSystemChat.Id);

                    // Конвертування повідомлення (сервер-клієнт) у формат для відправки MessageForSending
                    MessageForSending messageForSendingToUserLoadBlackList = creator.ConvertMessageToMessageForSending(
                        messageToUserLoadBlackList, server.Login, serverSystemChat.ChatName);
                    
                    // Коригування даних у разі, якщо інформація на клієнті не актуальна, з метою надсилання актуальної інф
                    if (messageForSendingFormatReceived.HashBody != userAllInfoOnServer)
                    {
                        messageToUserLoadBlackList.SystemInfo = SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForBlackList;
                        messageForSendingToUserLoadBlackList.SystemInfo = SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForBlackList;

                        // Додавання (заміна на актуалізований об'єкт) інф про користувача до колекції підключених користувачів (колекція об'єктів ModelUserAllInfo)
                        usersAllInfoList[posUserName] = userAllInfo;

                        // Конвертування об'єкта ModelUserAllInfo у UserForSending - останній пересилається до клієнта (який підключається та містить про нього максимум інф)
                        UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);
                        
                        // Серіалізація об'єкта UserForSending
                        string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);

                        // Доєднання серіалізованого об'єкта UserForSending до об'єкта MessageForSending 
                        messageForSendingToUserLoadBlackList.SystemBody = userAllInfoForSendingJson;
                    }

                    // Серіалізація об'єкта MessageForSending для відправлення клієнту
                    string messageForSendingToUserLoadBlackListJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingToUserLoadBlackList);

                    // Відправка повідомлення користувачу, що підключається до чату - повідомлення клієнту про вдале/НУвдале підключення до нового чату
                    SendMessagesForUser(messageForSendingToUserLoadBlackListJson, remoteEP);

                    // Доєднання повідомлення до колекції повідомлень
                    messagesList.Add(messageToUserLoadBlackList);

                    // Збереження відправленого повідомлення до БД
                    databaseService.SaveMessageToDb(messageToUserLoadBlackList);

                    // Відображення відправленого повідомлення (про вдале підключення клієнта до чату) на екрані статистики сервера
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageToUserLoadBlackList));
                }
                // Алгоритм обробки запитів на оновлення Чорного Списку клієнта у БД
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UpdateUserBlackListInDb))
                {
                    // Визначення положення елемента у колекції користувачів List<ModelUserAllInfo> usersAllInfoList
                    // з метою подальшого оновлення інф та відправки клієнту оновленого userAllInfo після редагування Чорного Списка
                    int posUserName = clientsList.IndexOf(remoteEP); // визначення положення у колекції кінцевих точок (отримується індекс)

                    // Отримання колекції List<ModelUsersLoginAndId> usersBlackList - редагований Чорний Список
                    string usersBlackListJson = messageForSendingFormatReceived.SystemBody;
                    
                    // Десеріалізація колекції
                    List<ModelUsersLoginAndId> usersBlackList = new List<ModelUsersLoginAndId>(); 
                    if(usersBlackListJson.Length > 0)
                    {
                        usersBlackList = JsonSerializer.Deserialize<List<ModelUsersLoginAndId>>(usersBlackListJson); 
                    }

                    // Оновлення даних у БД
                    databaseService.UpdateUsersBlackListAsync(usersBlackList, messageReceived.CreatorUserId);

                    // Отримання та оновлення об'єкта ModelUserAllInfo
                    ModelUserAllInfo userAllInfo = databaseService.CreateUserForSending(
                        usersAllInfoList[posUserName].UserLogin,
                        remoteEP,
                        usersAllInfoList[posUserName].CurrentChatId,
                        usersAllInfoList[posUserName].CurrentChatName,
                        usersNamesList);
                    // Додавання (заміна на актуалізований об'єкт) інф про користувача до колекції підключених користувачів (колекція об'єктів ModelUserAllInfo)
                    usersAllInfoList[posUserName] = userAllInfo;

                    // Відправка оновленого ModelUserAllInfo клієнту
                    // Створення повідомлення для відправки клієнту 
                    ChatLibrary.Message messageNewUserAllInfo = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody, 
                        SystemInfo.SystemInfo_UpdatedUserInformation, server.Id, serverSystemChat.Id);

                    // Конвертування у формат для відправки MessageForSending
                    MessageForSending messageForSendingNewUserAllInfo = creator.ConvertMessageToMessageForSending(
                        messageNewUserAllInfo, server.Login, serverSystemChat.ChatName
                        );

                    // Доєднання сутностей до SystemBody 
                    // Конвертування об'єкта ModelUserAllInfo у UserForSending - останній пересилається до клієнта 
                    UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);
                    // Серіалізація об'єкта UserForSending
                    string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);
                    // Доєднання серіалізованого об'єкта UserForSending до об'єкта MessageForSending
                    messageForSendingNewUserAllInfo.SystemBody = userAllInfoForSendingJson;

                    // Серіалізація повідомлення
                    string messageForSendingNewUserAllInfoJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingNewUserAllInfo);

                    // Відправка повідомлення
                    SendMessagesForUser(messageForSendingNewUserAllInfoJson, remoteEP);

                    // Доєднання повідомлення до колекції повідомлень
                    messagesList.Add(messageNewUserAllInfo);

                    // Збереження повідомлення до БД
                    databaseService.SaveMessageToDb(messageNewUserAllInfo);

                    // Виведення відправленого повідомлення 
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageNewUserAllInfo));


                }
                // Алгоритм обробки запитів на використання клієнтом редактора Чатів
                // - обробка запиту клієнта на оновлення даних про себе
                // - відправка об'єкта UserForSending у разі неактуальності даних на боці клієнта (порівняння хеш-кодів)
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UpdateUserAllInfoForChatEditor))
                {
                    // Визначення положення елемента у колекції користувачів List<ModelUserAllInfo> usersAllInfoList
                    // з метою подальшого оновлення інф та перевірки актуальності даних
                    int posUserName = clientsList.IndexOf(remoteEP); // визначення положення у колекції кінцевих точок (отримується індекс)

                    // Формування об'єкта ModelUserAllInfo - об'єкт, що містить максимальну кількість інф про поточний стан користувача 
                    ModelUserAllInfo userAllInfo = databaseService.CreateUserForSending(
                        usersAllInfoList[posUserName].UserLogin,
                        usersAllInfoList[posUserName].UsersIPEndPoint,
                        usersAllInfoList[posUserName].CurrentChatId,
                        usersAllInfoList[posUserName].CurrentChatName,
                        usersNamesList);

                    // Визначення актуальності об'єкта ModelUserAllInfo userAllInfo з його відповідником на клієнті 
                    // - порівняння хеш-кодів
                    UserForSending userForSendingAllInfo = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);
                    string userAllInfoOnServer = userForSendingAllInfo.GetHashCode().ToString();

                    // Створення Повідомлення from server to client про актуальність даних або надсилання актуального об'єкта UserForSending
                    ChatLibrary.Message messageToUserLoadChatList = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody,
                        SystemInfo.SystemInfo_UserAllInfoIsUpToDateForChatList, server.Id, serverSystemChat.Id);

                    // Конвертування повідомлення (сервер-клієнт) у формат для відправки MessageForSending
                    MessageForSending messageForSendingToUserLoadChatList = creator.ConvertMessageToMessageForSending(
                        messageToUserLoadChatList, server.Login, serverSystemChat.ChatName);

                    // Коригування даних у разі, якщо інформація на клієнті не актуальна, з метою надсилання актуальної інф
                    if (messageForSendingFormatReceived.HashBody != userAllInfoOnServer)
                    {
                        messageToUserLoadChatList.SystemInfo = SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForChatList;
                        messageForSendingToUserLoadChatList.SystemInfo = SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForChatList;

                        // Додавання (заміна на актуалізований об'єкт) інф про користувача до колекції підключених користувачів (колекція об'єктів ModelUserAllInfo)
                        usersAllInfoList[posUserName] = userAllInfo;

                        // Конвертування об'єкта ModelUserAllInfo у UserForSending - останній пересилається до клієнта (який підключається та містить про нього максимум інф)
                        UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);

                        // Серіалізація об'єкта UserForSending
                        string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);

                        // Доєднання серіалізованого об'єкта UserForSending до об'єкта MessageForSending 
                        messageForSendingToUserLoadChatList.SystemBody = userAllInfoForSendingJson;
                    }

                    // Серіалізація об'єкта MessageForSending для відправлення клієнту
                    string messageForSendingToUserLoadChatListJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingToUserLoadChatList);

                    // Відправка повідомлення користувачу, що підключається до чату - повідомлення клієнту про вдале/НУвдале підключення до нового чату
                    SendMessagesForUser(messageForSendingToUserLoadChatListJson, remoteEP);

                    // Доєднання повідомлення до колекції повідомлень
                    messagesList.Add(messageToUserLoadChatList);

                    // Збереження відправленого повідомлення до БД
                    databaseService.SaveMessageToDb(messageToUserLoadChatList);

                    // Відображення відправленого повідомлення (про вдале підключення клієнта до чату) на екрані статистики сервера
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageToUserLoadChatList));
                }
                // Алгоритм обробки запитів на додавання/редагування/видалення чату користувача
                // - отримує від користувача об'єкт List<ModelChatsNameAndId> (включає список користувачів)
                // - оновлює дані у БД
                // - відправляє повідомлення клієнту (який відправив запит), що містить оновлену інф у об'єкті UserForSending
                // - оновлює інформацію усіх підключених клієнтів (перевизначає для кожного об'єкт ModelUserForSending)
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UpdateUserChatListInDb))
                {
                    // Визначення положення елемента у колекції користувачів List<ModelUserAllInfo> usersAllInfoList
                    // з метою подальшого оновлення інф та відправки клієнту оновленого userAllInfo після редагування колекції чатів
                    int posUserName = clientsList.IndexOf(remoteEP); // визначення положення у колекції кінцевих точок (отримується індекс)

                    // Отримання серіалізованої колекції List<ModelChatsNameAndId> usersChatListJson - редагований Список чатів
                    string usersChatListJson = messageForSendingFormatReceived.SystemBody;
                    
                    // Десеріалізація колекції
                    List<ModelChatsNameAndId> usersChatList = new List<ModelChatsNameAndId>(); 
                    if(usersChatListJson.Length > 0)
                    {
                        usersChatList = JsonSerializer.Deserialize<List<ModelChatsNameAndId>>(usersChatListJson); 
                    }

                    // Оновлення даних у БД
                    databaseService.UpdateUsersChatListAsync(usersChatList, messageReceived.CreatorUserId);

                    Thread.Sleep(1000);
                    Chat chat = databaseService.InitChat(usersAllInfoList[posUserName].CurrentChatId);
                                        
                    string s = "" + chat; // змінна перетворює об'єкт Chat у формат рядка - дозволя обійти помилку,  що виникає у разі відсутності чату із потрібним Id у БД - наприклад після видалення чату у момент коли користувач був до нього підключений
                    
                    int currentChatId = s.Length == 0 ? groupChat.Id : chat.Id;
                    string currentChatName = s.Length == 0 ? groupChat.ChatName : chat.ChatName;

                    ModelUserAllInfo userAllInfo = databaseService.CreateUserForSending(
                        usersAllInfoList[posUserName].UserLogin,
                        usersAllInfoList[posUserName].UsersIPEndPoint,
                        currentChatId,
                        currentChatName,
                        usersNamesList);

                    // Додавання (заміна на актуалізований об'єкт) інф про користувача до колекції підключених користувачів (колекція об'єктів ModelUserAllInfo)
                    usersAllInfoList[posUserName] = userAllInfo;

                    // У разі відсутності у БД чату до якого клієнт був підключений раніше
                    // - запускається алгоритм перепідключення до системного Групового Чату
                    // - інакше - завантажуються оновлені дані про клієнта
                    string systemInfo = chat == null ? SystemInfo.SystemInfo_ChatIsConnected : SystemInfo.SystemInfo_UpdatedUserInformation;

                    // Відправка оновленого ModelUserAllInfo клієнту
                    // Створення повідомлення для відправки клієнту 
                    ChatLibrary.Message messageNewUserAllInfo = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody,
                        systemInfo, 
                        server.Id, serverSystemChat.Id);

                    // Конвертування у формат для відправки MessageForSending
                    MessageForSending messageForSendingNewUserAllInfo = creator.ConvertMessageToMessageForSending(
                        messageNewUserAllInfo, server.Login, serverSystemChat.ChatName);

                    // Доєднання сутностей до SystemBody 
                    // Конвертування об'єкта ModelUserAllInfo у UserForSending - останній пересилається до клієнта 
                    UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);
                    // Серіалізація об'єкта UserForSending
                    string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);
                    // Доєднання серіалізованого об'єкта UserForSending до об'єкта MessageForSending
                    messageForSendingNewUserAllInfo.SystemBody = userAllInfoForSendingJson;

                    // Серіалізація повідомлення
                    string messageForSendingNewUserAllInfoJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingNewUserAllInfo);

                    // Відправка повідомлення
                    SendMessagesForUser(messageForSendingNewUserAllInfoJson, remoteEP);

                    // Доєднання повідомлення до колекції повідомлень
                    messagesList.Add(messageNewUserAllInfo);

                    // Збереження повідомлення до БД
                    databaseService.SaveMessageToDb(messageNewUserAllInfo);

                    // Виведення відправленого повідомлення 
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageNewUserAllInfo));

                    // Зміна інф про клієнтів, що підключені до сервера (відправляється об'єкт UserForSending-повна інф про клієнта)
                    UpdateUsersInfoAfterUsingChatEditor(posUserName);
                }
                // Алгоритм обробки запитів на відключення клієнта
                else if (messageReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UserDisconnect))
                {
                    // Визначення положення елемента у колекції користувачів List<ModelUserAllInfo> usersAllInfoList
                    // з метою подальшого видалення
                    int posUserName = clientsList.IndexOf(remoteEP); // визначення положення у колекції кінцевих точок (отримується індекс)

                    ModelUserAllInfo userToRemove = usersAllInfoList[posUserName];
                    string userNameToRemove = usersNamesList[posUserName]; // отримання логіна з колекціії логінів (за раніше встановленим індексом)

                    // Відправка повідомлення про від'єднання клієнта - у (груповий чат) у чат, де спілкувався клієнт на момент виходу
                    // Створення повідомлення для відправки клієнту 
                    ChatLibrary.Message messageUserOutFromChat = creator.CreateMessage(
                        $"{SystemInfo.SystemInfo_MessageUserLeftTheChat}{userToRemove.UserLogin}" +
                        $"{SystemInfo.SystemInfo_Separator}" +
                        $"{SystemInfo.SystemInfo_MessageNumberOfChatParticipants}{UsersInChat(userToRemove.CurrentChatId) - 1}", // -1
                        SystemInfo.SystemInfo_UserDisconnect, server.Id, messageReceived.ChatsId);

                    // Конвертування у формат для відправки MessageForSending
                    MessageForSending messageeForSendingUserOutFromChat = creator.ConvertMessageToMessageForSending(messageUserOutFromChat, server.Login, messageForSendingFormatReceived.ChatsName);

                    // Видалення даних про користувача, що відключається з колекцій
                    clientsList.Remove(remoteEP); // Видалення кінцевої точки
                    usersNamesList.Remove(userNameToRemove); // видалення логіна з колекції логінів
                    usersAllInfoList.Remove(userToRemove); // видалення з колекції користувачів

                    // Серіалізація повідомлення
                    string messageeForSendingUserOutFromChatJson = JsonSerializer.Serialize<MessageForSending>(messageeForSendingUserOutFromChat);

                    // Відправка повідомлення
                    // - у чат в якому перебував клієнт на момент виходу (id чату приходить у повідомленні)
                    SendMessagesAllUsers(messageeForSendingUserOutFromChatJson, messageReceived.ChatsId);

                    // Доєднання повідомлення до колекції повідомлень
                    messagesList.Add(messageUserOutFromChat);

                    // Збереження повідомлення до БД
                    databaseService.SaveMessageToDb(messageUserOutFromChat);

                    // Виведення відправленого повідомлення 
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageUserOutFromChat));
                }
            }

        }

        // Підключення користувача до іншого чату
        private void UserConnectingToANewChat(int posUserName, 
            int newChatId, 
            int oldChatId, string oldChatName, 
            string systemInfo_AboutChatConnecting, 
            ModelUserAllInfo userAllInfo_New, 
            string accompanyingMessage)
        {

            // Визначення, чи існує на даний момент чат до якого бажає підключитись клієнт
            Chat newChat = databaseService.InitChat(newChatId);

            // Формування повідомлення про вдале/НЕвдале підключення до чату
            string messageAboutConnectionToChat = systemInfo_AboutChatConnecting; // повідомлення про вдале підключення до чату
            if (newChat == null)
            {
                messageAboutConnectionToChat = SystemInfo.SystemInfo_ChatNotConnected; // у разі відсутності чату - повідомлення про НЕВДАЛЕ підключення до чату
            }

            // Створення Повідомлення from server to client про його (користувача) підключення до чату
            ChatLibrary.Message messageToUserConnectingToChat = creator.CreateMessage(
                SystemInfo.SystemInfo_EmptyMessageBody,
                messageAboutConnectionToChat, server.Id, serverSystemChat.Id);

            // Конвертування повідомлення (сервер-клієнт, що підключається) у формат для відправки MessageForSending
            MessageForSending messageForSendingToUserConnectingToChat = creator.ConvertMessageToMessageForSending(
                messageToUserConnectingToChat, server.Login, serverSystemChat.ChatName);

            // Формування об'єкта ModelUserAllInfo - об'єкт, що містить максимальну кількість інф про поточний стан користувача 
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

            // Додавання (заміна на актуалізований об'єкт) інф про користувача до колекції підключених користувачів (колекція об'єктів ModelUserAllInfo)
            usersAllInfoList[posUserName] = userAllInfo;

            // Конвертування об'єкта ModelUserAllInfo у UserForSending - останній пересилається до клієнта (який підключається та містить про нього максимум інф)
            UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo);

            // Серіалізація об'єкта UserForSending
            string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);

            // Доєднання серіалізованого об'єкта UserForSending до об'єкта MessageForSending 
            messageForSendingToUserConnectingToChat.SystemBody = userAllInfoForSendingJson;
            // Доєднання до повідомлення супровідної інф - наприклад про причину перепідключення
            // (видалення користувача з числа користувачів чату... - зазвичай залишається пустим)
            messageForSendingToUserConnectingToChat.Body = accompanyingMessage;

            // Серіалізація об'єкта MessageForSending для відправлення клієнту
            string messageForSendingToUserConnectingToChatJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingToUserConnectingToChat);

            // Відправка повідомлення користувачу, що підключається до чату - повідомлення клієнту про вдале/НЕвдале підключення до нового чату
            SendMessagesForUser(messageForSendingToUserConnectingToChatJson, usersAllInfoList[posUserName].UsersIPEndPoint);

            // Доєднання повідомлення до колекції повідомлень
            messagesList.Add(messageToUserConnectingToChat);

            // Збереження відправленого повідомлення до БД
            databaseService.SaveMessageToDb(messageToUserConnectingToChat);

            // Відображення відправленого повідомлення (про вдале підключення клієнта до чату) на екрані статистики сервера
            tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageToUserConnectingToChat));

            // Повідомлення у попередній чат про вихід користувача - бажано публікувати після виходу клієнта з чату
            if (newChat != null)
            {
                // Відправлення у новий чат повідомлення про приєднання клієнта
                // - створення повідомлення про приєднання клієнта 
                Thread.Sleep(3000);
                ChatLibrary.Message messageUserConnectedToChat = creator.CreateMessage(
                    $"{SystemInfo.SystemInfo_MessageJoinedTheChat} {usersAllInfoList[posUserName].UserLogin}" +
                    $"{SystemInfo.SystemInfo_Separator}" +
                    $"{SystemInfo.SystemInfo_MessageNumberOfChatParticipants} {UsersInChat(newChat.Id)}", // +1
                    SystemInfo.SystemInfo_ServerMessage, server.Id, newChat.Id);

                // Конвертування у формат для відправки MessageForSending
                MessageForSending messageForSendingUserConnectedToChat = creator.ConvertMessageToMessageForSending(
                    messageUserConnectedToChat,
                    server.Login, newChat.ChatName);

                // - серіалізація повідомлення про приєднання клієнта
                string messageForSendingUserConnectedToChatJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingUserConnectedToChat);

                // -  відправка повідомлення про приєднання клієнта у новий чат
                SendMessagesAllUsers(messageForSendingUserConnectedToChatJson, newChat.Id);

                // Доєднання повідомлення до колекції повідомлень
                messagesList.Add(messageUserConnectedToChat);

                // - збереження до БД повідомлення про приєднання клієнта у новий чат
                databaseService.SaveMessageToDb(messageUserConnectedToChat);

                // - виведення у поле статистики чату повідомлення про приєднання клієнта
                tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageUserConnectedToChat));
            }

        }


        // Оновлення даних підключених користувачів після використання одним з них Чат-редактора (додано/видалено чати, додано/видалено користувачів чатів)
        private void UpdateUsersInfoAfterUsingChatEditor(int posUserName)
        {
            for (int i = 0; i < usersAllInfoList.Count(); i++)
            {
                // Перехід до наступного елемента у випадку потраплянні на клієнта, що вносив зміни у колекцію чатів
                // - обробка даного клаєнта відбувається у іншому методі
                if(i == posUserName)
                {
                    continue;
                }

                // Обробка випадку ВИДАЛЕННЯ чату
                // Визначення наявності/відсутності чату, до якого на даний момент підключений клієнт - може бути видалено
                // - у разі відстності клієнта буде перепідключено до системного групового чату
                Chat chat = databaseService.InitChat(usersAllInfoList[i].CurrentChatId);
                string stringFromChat = "" + chat; // змінна перетворює об'єкт Chat у формат рядка - дозволя обійти помилку, що виникає у разі відсутності чату із потрібним Id у БД - наприклад після видалення чату у момент коли користувач був до нього підключений

                // Обробка випадку видалення клієнта з числа користувачів чату (сам чат не видаляється)
                ChatUser chatUser = databaseService.InitChatUser(usersAllInfoList[i].UserId, usersAllInfoList[i].CurrentChatId);
                string stringFromChatUser = "" + chatUser; 

                int currentChatId = (stringFromChat.Length == 0 || stringFromChatUser.Length == 0 ? groupChat.Id : chat.Id);
                string currentChatName = (stringFromChat.Length == 0 || stringFromChatUser.Length == 0 ? groupChat.ChatName : chat.ChatName);

                // Отримання Оновленої повної інф про клієнта
                ModelUserAllInfo userAllInfo_New = databaseService.CreateUserForSending(
                    usersAllInfoList[i].UserLogin,
                    usersAllInfoList[i].UsersIPEndPoint,
                    currentChatId,
                    currentChatName,
                    usersNamesList);

                // Колекція - Текст повідомлення клієнту про зміну підключення
                // (підключення/відключення від чатів, перепідключення до системного групового чату)
                List<string> messagesForClientConnectionChange = new List<string>();

                // Обробка випадку ВІДКЛЮЧЕННЯ від чату
                foreach (var chatFromListOld in usersAllInfoList[i].ChatsList)
                {
                    if(userAllInfo_New.ChatsList.Where(c => c.Id == chatFromListOld.Id).Count() == 0)
                    {
                        // Повідомлення для клієнта "Вас відключили від чату chatFromListOld.ChatName"
                        messagesForClientConnectionChange.Add($"{SystemInfo.SystemInfo_MessageYouHaveBeenDisconnectedFromTheChat} - {chatFromListOld.ChatName}"); // You have been disconnected from the chat
                    }
                }

                // Обробка випадку ПІДКЛЮЧЕННЯ до чату
                foreach (var chatFromListNew in userAllInfo_New.ChatsList)
                {
                    if(usersAllInfoList[i].ChatsList.Where(c => c.Id == chatFromListNew.Id).Count() == 0)
                    {
                        // Повідомлення для клієнта "Вас підключили до чату chatFromListNew.ChatName"
                        messagesForClientConnectionChange.Add($"{SystemInfo.SystemInfo_MessageYouHaveBeenConnectedToTheChat} - {chatFromListNew.ChatName}"); // You have been connected to the chat
                    }
                }

                // Перепідключення клієнта до групового чату у разі знищення чату у якому він перебував
                if(stringFromChat.Length == 0 || stringFromChatUser.Length == 0)
                {
                    // Перепідключення клієнта до системного групового чату - при знищенні чату, у якому перебував клієнт

                    // Відправка повідомлення клієнту про перепідключення до системного групового чату з оновленням userAllInfo 
                    string systemInfo = stringFromChat.Length == 0 ? SystemInfo.SystemInfo_DeletedCurrentChat : SystemInfo.SystemInfo_RemovedFromCurrentChatUsers;

                    // Підключення до іншого чату - системного Групового Чату
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
                    // Додавання (заміна на актуалізований об'єкт) інф про користувача до колекції підключених користувачів (колекція об'єктів ModelUserAllInfo)
                    usersAllInfoList[i] = userAllInfo_New;

                    // Відправка повідомлення на оновлення інф клієнта
                    // Створення повідомлення для відправки клієнту 
                    ChatLibrary.Message messageNewUserAllInfo = creator.CreateMessage(
                        SystemInfo.SystemInfo_EmptyMessageBody,
                        SystemInfo.SystemInfo_UpdatedUserInformation, server.Id, serverSystemChat.Id);

                    // Конвертування у формат для відправки MessageForSending
                    MessageForSending messageForSendingNewUserAllInfo = creator.ConvertMessageToMessageForSending(
                        messageNewUserAllInfo, server.Login, serverSystemChat.ChatName
                        );

                    // Доєднання сутностей до SystemBody 
                    // Конвертування об'єкта ModelUserAllInfo у UserForSending - останній пересилається до клієнта 
                    UserForSending userAllInfoForSending = creator.ConvertModelUserAllInfoToUserForSending(userAllInfo_New);
                    // Серіалізація об'єкта UserForSending
                    string userAllInfoForSendingJson = JsonSerializer.Serialize<UserForSending>(userAllInfoForSending);
                    // Доєднання серіалізованого об'єкта UserForSending до об'єкта MessageForSending
                    messageForSendingNewUserAllInfo.SystemBody = userAllInfoForSendingJson;

                    // Серіалізація повідомлення
                    string messageForSendingNewUserAllInfoJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingNewUserAllInfo);

                    // Відправка повідомлення
                    SendMessagesForUser(messageForSendingNewUserAllInfoJson, usersAllInfoList[i].UsersIPEndPoint);

                    // Доєднання повідомлення до колекції повідомлень
                    messagesList.Add(messageNewUserAllInfo);

                    // Збереження повідомлення до БД
                    databaseService.SaveMessageToDb(messageNewUserAllInfo);

                    // Виведення відправленого повідомлення 
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageNewUserAllInfo));
                }

                // Відправка повідомлення клієнту про підключення, відключення від чатів
                foreach (string message in messagesForClientConnectionChange)
                {
                    // Створення повідомлення для відправки клієнту 
                    ChatLibrary.Message messageNewUserAllInfo = creator.CreateMessage(
                        message,
                        SystemInfo.SystemInfo_ServerMessage, server.Id, serverSystemChat.Id);

                    // Конвертування у формат для відправки MessageForSending
                    MessageForSending messageForSendingNewUserAllInfo = creator.ConvertMessageToMessageForSending(
                        messageNewUserAllInfo, server.Login, serverSystemChat.ChatName
                        );

                    // Серіалізація повідомлення
                    string messageForSendingNewUserAllInfoJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingNewUserAllInfo);

                    // Відправка повідомлення
                    SendMessagesForUser(messageForSendingNewUserAllInfoJson, usersAllInfoList[i].UsersIPEndPoint);

                    // Доєднання повідомлення до колекції повідомлень
                    messagesList.Add(messageNewUserAllInfo);

                    // Збереження повідомлення до БД
                    databaseService.SaveMessageToDb(messageNewUserAllInfo);

                    // Виведення відправленого повідомлення 
                    tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), GetFullMessageLong(messageNewUserAllInfo));
                    Thread.Sleep(500);
                }

            }

        }


        // Отримання Історії чату у вигляді рядка з колекції ChatLibrary.Message 
        private string GetHistoryString(List<ChatLibrary.Message> messages, int creatorUserId)
        {
            StringBuilder chatHistoryString = new StringBuilder();
            
            // Отримання користувача, який замовив історію чату (для виходу на його BlackList)
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

        // Отримання історії повідомлень у форматі рядка та
        // Завантаження статистики сервера у візуальний компонент
        private void LoadChatHistoryToServerStatistic()
        {
            // Виведення списку повідомлень, отриманих з БД, у вікно статистики сервера 
            StringBuilder sb = new StringBuilder();
            foreach (ChatLibrary.Message message in messagesList)
            {
                sb.Append(GetFullMessageLong(message).Trim());
                sb.AppendLine(SystemInfo.SystemInfo_Separator);
            }
            tbServerStatistics.BeginInvoke(new Action<string>(AddTextToTbServerStatistics), sb.ToString().Trim());
        }

        // Кількість учасників чату по Id чату
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

        // Підготовка повідомлення для виведення у чат сервера
        private string GetFullMessageLong(ChatLibrary.Message message)
        {
            User creator = usersList.FirstOrDefault(u => u.Id == message.CreatorUserId, new User());
            Chat currentChat = message.ChatsId == 0 ? new Chat() : chatsList.FirstOrDefault(c => c.Id == message.ChatsId, new Chat());
            return $"{message.SendingTime.ToString()} - from {creator?.Login} - chat {currentChat?.ChatName} \r\n- {message.SystemInfo} \r\n- {message.Body}";
        }


        // Відправка повідомлень до усіх підключених до вказаного чату клієнтів
        private void SendMessagesAllUsers(string messageForUsers, int chatsId, int userMessageCreator = 0)
        {
            
            foreach (var user in usersAllInfoList)
            {
                // Перевірка наявності відправника у чорному списку отримувача - останній не має отримати повідомлення
                if(userMessageCreator != 0 && user.BlackList.Where(u => u.Id == userMessageCreator).Count() != 0)
                {
                    continue;
                }
                // відсилання повідомлення
                if (user.CurrentChatId == chatsId)
                {
                    SendMessagesForUser(messageForUsers, user.UsersIPEndPoint);
                }
            }
        }

        // Відправка повідомлень до усіх підключених клієнтів
        private void SendMessagesAllUsers(string message)
        {
            foreach (var user in usersAllInfoList)
            {
                SendMessagesForUser(message, user.UsersIPEndPoint);
            }
        }

        // Відправка повідомлень до 1 клієнта
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

        // Підготовка та відображення текстової інформації у візуальному компоненті сервера
        private void AddTextToTbServerStatistics(string str)
        {
            StringBuilder sb = new StringBuilder(tbServerStatistics.Text);
            sb.Append(str.Trim());
            sb.AppendLine(SystemInfo.SystemInfo_Separator); // "\r\n"
            tbServerStatistics.Text = sb.ToString();
            tbServerStatistics.SelectionStart = tbServerStatistics.Text.Length; // переміщення каретки в кінець тексту
            tbServerStatistics.ScrollToCaret(); // скролінг вікна до позиції каретки (у дному разі до кінця тексту)
        }

        // Підготовка та відображення текстової інформації у візуальному компоненті сервера
        // - без додавання до попередньої інформації
        private void AddNewTextToTbServerStatistics(string str)
        {
            StringBuilder sb = new StringBuilder(str.Trim());
            sb.AppendLine(SystemInfo.SystemInfo_Separator); // "\r\n"
            tbServerStatistics.Text = sb.ToString();
            tbServerStatistics.SelectionStart = tbServerStatistics.Text.Length; // переміщення каретки в кінець тексту
            tbServerStatistics.ScrollToCaret(); // скролінг вікна до позиції каретки (у дному разі до кінця тексту)
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


            // НЕ обов'язково - запуск сервера після запуску форми (для зручності) - проект Client має бути додано до Залежностей
            // TODO
            Process.Start("Client_UdpClient.exe");
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Process.Start("Client_UdpClient.exe");
        }

        // Завантаження історії чату
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
            // Створення повідомленння про припинення роботи сервера
            // Створення повідомлення 
            ChatLibrary.Message messageServerShutDown = creator.CreateMessage(
                $"{SystemInfo.SystemInfo_MessageServerShutDown} {DateTime.Now.ToString()} \r\n-------------------------------------------",
                SystemInfo.SystemInfo_ServerShutDown,
                server.Id, groupChat.Id);
            // Додавання повідомлення до колекції повідомлень (на початку роботи колекція ініціюється повідомленнями, завантаженими з БД)
            messagesList.Add(messageServerShutDown);
            // Збереження повідомлення до БД 
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

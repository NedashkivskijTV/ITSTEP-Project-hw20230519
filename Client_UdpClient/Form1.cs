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
        // Сокет для приєднання до сервера
        Socket listeningSocket = null;

        ItemCreator? creator;

        UserForSending? user;

        // Змінна для підрахунку кількості спрацювань події на зміну вмісту cbChatsList
        // - при завантаженні даних спрацьовує 2 рази, далі можна використовувати для
        // запуску алгоритма перепідключення чатів
        bool isCbChatsListInitComplete = false;

        // Змінна для збереження індекса поточного чата
        // - використувається для захисту від повторного спрацювання методу (cbChatsList_SelectedIndexChanged)
        // завантаження вже підключеного чату
        // - при натисненні у комбобоксі рядка завантаженого на даний момент чату
        int selectedChatIndexInCB = 0;

        public Form1()
        {
            InitializeComponent();

            // Первинні налаштування інтерфейсу
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

        // Натискання на кнопку Приєнання/Від'єднання
        private async void btnStartChat_Click(object sender, EventArgs e)
        {
            if (listeningSocket != null)
            {
                // Відключення від сервера
                Form1_FormClosing(sender, null);

                // Налаштування інтерфейсу - Від'єднаний режим
                btnStartChat.Text = "Start Chat";
                tbUserName.Enabled = true;
                tbUserMessage.Text = "";
                tbUserMessage.Enabled = false;
                btnSendMessage.Enabled = false;
            }
            else
            {
                // Підключення до сервера
                try
                {
                    // Якщо поле для введення логіна користувача пусте - метод перериває роботу
                    if (tbUserName.Text.Trim().Length == 0)
                    {
                        MessageBox.Show(SystemInfo.SystemInfo_MessageNeedUserLogin); // Потрібно ввести логін користувача
                        return;
                    }

                    // Форма авторизації/реєстрації

                    // Створення об'єкта-моделі для передачі даних між формами (головною та реєстрації)
                    ParaLoginPass paraLoginPass = new ParaLoginPass(tbUserName.Text, "");
                    // Прапорець для завантаження форми у вигляді авторизації / реєстрації
                    bool isAuthorization = true;

                    // Цикл реєстрації / авторизації - реалізація повторного вводу логіна/пароля
                    bool isTryAgain = false; // прапорець запуска цикла на повторну авторизацію / реєстрацію
                    DialogResult isNextTry; // результат виведення модального вікна із запитанням про наступну спробу авторизації / реєстрації
                    do
                    {
                        isTryAgain = false;

                        // Створення / виклик форми реєстрації
                        FormAuthorizationRegistration form = new FormAuthorizationRegistration(paraLoginPass, isAuthorization);
                        // Створення повідомлення для відправлення серверу даних для реєстрації/авторизації
                        MessageForSending clientMessageAuthorizeRegister = null;
                        var result = form.ShowDialog();
                        // Відображення на формі логіна користувача, набраного у вікні авторизації/реєстрації
                        tbUserName.Text = paraLoginPass.Login;
                        if (result == DialogResult.OK)
                        {
                            // Авторизація
                            clientMessageAuthorizeRegister = creator.CreateMessageForSending(
                                SystemInfo.SystemInfo_EmptyMessageBody,
                                SystemInfo.SystemInfo_AuthorizationUser, 0, 0);
                        }
                        else if (result == DialogResult.Yes)
                        {
                            // Реєстрація
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

                        // Ініціалізація сокету
                        listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                        // Алгоритм реєстрації нового користувача
                        // (клієнт відправляє запит на підключення введеного користувачем у відповідне поле логіна)
                        string messageFromServerAboutAuthorizeRegisterJson = ""; // відповідь сервера на запит клієнта
                                                                                 //string message = $"#client-newUser#####{tbUserName.Text}";
                        string message = ""; // запит клієнта

                        // Створення повідомлення
                        // Серіалізація повідомлення
                        message = JsonSerializer.Serialize<MessageForSending>(clientMessageAuthorizeRegister);

                        // Кодування у байтовий масив
                        byte[] data = Encoding.Default.GetBytes(message);
                        EndPoint remoteEndpoint = new IPEndPoint(ipAddress, remotePort);
                        // Відправка запиту
                        listeningSocket.SendTo(data, remoteEndpoint);

                        // Отримання повідомлення-відповіді від сервера
                        messageFromServerAboutAuthorizeRegisterJson = MessageReceiver();

                        // Десеріалізація повідомлення у об'єкт типу MessageForSending
                        MessageForSending messageForSendingFormatReceived = JsonSerializer.Deserialize<MessageForSending>(messageFromServerAboutAuthorizeRegisterJson);

                        // Алгоритм переривання роботи методу у разі, якщо
                        // - на сервері вже зареєстрований користувач, який введений на поточному клієнті при реєстрації (логін вже існує)
                        // - невірний логін або пароль (при авторизації)
                        if (messageForSendingFormatReceived.SystemInfo.Equals(SystemInfo.SystemInfo_AuthorizUserFalse) ||
                            messageForSendingFormatReceived.SystemInfo.Equals(SystemInfo.SystemInfo_RegistrationUserFalse))
                        {
                            // Текст повідомлення для користувача про конкретну помилку
                            string messageToUserAboutAuthorizeOrRegisterError =
                                messageForSendingFormatReceived.SystemInfo.Equals(SystemInfo.SystemInfo_AuthorizUserFalse) ?
                                SystemInfo.SystemInfo_MessageIncorrectLoginOrPassword :
                                SystemInfo.SystemInfo_MessageLoginAlreadyExists;
                            // Виведення повідомлення про помилку у модальному вікні з вибором ПРОДОВЖИТИ / ПРИПИНИТИ
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
                            // Ініціалізація змінної UserForSending user
                            // - містить максимум інф про стан даного користувача на момент підключення до сервера
                            // Отримання серіалізованого об'єкта UserForSending
                            string userForSendingJson = messageForSendingFormatReceived.SystemBody;

                            // Десеріалізація об'єкта UserForSending та ініціалізація змінної екземпляра user
                            user = JsonSerializer.Deserialize<UserForSending>(userForSendingJson);

                            InitComboBoxChatList();
                        }
                        else if (messageForSendingFormatReceived.SystemInfo.Equals(SystemInfo.SystemInfo_RegistrationUserFalse))
                        {
                            // Виведення повідомлення про НЕВДАЛУ реєстрацію у модальному вікні з вибором ПРОДОВЖИТИ / ПРИПИНИТИ
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
                            // ВДАЛА реєстрація - перенаправлення на авторизацію
                            MessageBox.Show(SystemInfo.SystemInfo_MessageLogInToEnterTheChat,
                                "Authorize/Register", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            isTryAgain = true;
                        }
                    } while (isTryAgain);


                    //Запуск алгоритму прослуховування повідомлень від сервера -Запуск чату
                    Task.Run(() =>
                    {
                        Listen();
                    });

                    // Налаштування інтерфейсу - Приєднаний режим
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


        // Прослуховування відправлених від серверва повідомлень
        private void Listen()
        {
            try
            {
                // Цикл прослуховування
                while (true)
                {
                    // Отримання повідомлення - MessageForSending у серіалізованому вигляді
                    string messageReceived = MessageReceiver();

                    // Десеріалізація отриманого повідомлення
                    MessageForSending messageForSendingReceived = JsonSerializer.Deserialize<MessageForSending>(messageReceived);
                    // Умова - якщо SystemInfo містить код, що дане повідомлення user-users, server-users, server-user - текст
                    string message = "";
                    if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UserMessage) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UserDisconnect) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_ServerMessage)
                        ))
                    {
                        // Отримання Тексту повідомлення
                        message = messageForSendingReceived.GetFullMessageLong();
                    }
                    // Алгоритм підключення до іншого чату
                    else if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_ChatIsConnected) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_ChatNotConnected)
                        ))
                    {
                        if (messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_ChatNotConnected))
                        {
                            // Виведення повідомлення про невдале підключення до чату
                            MessageBox.Show(SystemInfo.SystemInfo_MessageFailedToConnectToChat, "Chat connecting error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            // Ініціалізація змінної UserForSending user
                            // - містить максимум інф про стан даного користувача на момент підключення до сервера
                            // Отримання серіалізованого об'єкта UserForSending
                            string userForSendingJson = messageForSendingReceived.SystemBody;

                            // Десеріалізація об'єкта UserForSending та ініціалізація змінної екземпляра user
                            user = JsonSerializer.Deserialize<UserForSending>(userForSendingJson);

                            // Ініціалізація списку доступних чатів з виділенням поточного чату
                            InitComboBoxChatList();

                            // Очищенні візуального компоненту - поля статистики чату
                            tbChatMessages.BeginInvoke(new Action<string>(TbChatMessagesUpdate), "");
                        }
                    }
                    // Завантаження історії чату
                    else if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_NewChatHistoryStart) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_NewChatHistoryContinue)
                        ))
                    {
                        // Отримання історії чату у форматі рядка
                        string chatHistory = messageForSendingReceived.SystemBody;

                        // Оновлення візуального компонента з відображенням історії чату
                        if (messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_NewChatHistoryStart))
                        {
                            tbChatMessages.BeginInvoke(new Action<string>(TbChatMessagesUpdate), chatHistory);
                        }
                        else
                        {
                            tbChatMessages.BeginInvoke(new Action<string>(AddTextToTbChatMessages), chatHistory);
                        }
                    }
                    // Завантаження актуальної інф про поточний чат та виведення списку користувачів чату
                    else if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UserAllInfoIsUpToDate) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_SendingTheCurrentUserAllInfo)
                        ))
                    {
                        // Отримання актуальної інф про поточного користувача
                        if (messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_SendingTheCurrentUserAllInfo))
                        {
                            // Ініціалізація змінної UserForSending user
                            // - містить максимум інф про стан даного користувача на момент підключення до сервера
                            // Отримання серіалізованого об'єкта UserForSending
                            string userForSendingJson = messageForSendingReceived.SystemBody;

                            // Десеріалізація об'єкта UserForSending та ініціалізація змінної екземпляра user
                            user = JsonSerializer.Deserialize<UserForSending>(userForSendingJson);

                            // Ініціалізація активного елемента - перемикача чатів
                            InitComboBoxChatList();
                        }

                        // Завантаження форми для виведення списку користувачів чату - перенесено до Listen
                        int userCreator = user.ChatsList.Find(c => c.Id == user.CurrentChatId).ChatCreatorId;

                        List<ModelUsersLoginAndId> usersList = user.ChatsList.Find(c => c.Id == user.CurrentChatId).UsersInChat;
                        FormUsersList form = new FormUsersList(usersList, userCreator);
                        form.ShowDialog();
                    }
                    // Завантаження актуальної інф про Клієнта та виведення редактора чорного списку користувача
                    else if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UserAllInfoIsUpToDateForBlackList) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForBlackList)
                        ))
                    {
                        // Отримання актуальної інф про поточного користувача
                        if (messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForBlackList))
                        {
                            // Ініціалізація змінної UserForSending user
                            // - містить максимум інф про стан даного користувача на момент підключення до сервера
                            // Отримання серіалізованого об'єкта UserForSending
                            string userForSendingJson = messageForSendingReceived.SystemBody;

                            // Десеріалізація об'єкта UserForSending та ініціалізація змінної екземпляра user
                            user = JsonSerializer.Deserialize<UserForSending>(userForSendingJson);

                            // Ініціалізація активного елемента - перемикача чатів
                            InitComboBoxChatList();
                        }

                        // Завантаження форми редактора чорного списку 
                        List<ModelUsersLoginAndId> usersBlackList = user.BlackList;
                        List<ModelUsersLoginAndId> usersList = user.ChatsList.Find(c => c.ChatName == SystemInfo.SystemInfo_GroupChat).UsersInChat;

                        FormBlackListEditor form = new FormBlackListEditor(usersBlackList, usersList, user.UserId);
                        if(form.ShowDialog() == DialogResult.OK)
                        {
                            // Формування повідомлення на оновлення Чорного списка користувача ... 
                            // Створення повідомлення для відправки серверу MessageForSending
                            MessageForSending messageForSendingBlackListUpdate = new MessageForSending(
                                SystemInfo.SystemInfo_EmptyMessageBody, 
                                SystemInfo.SystemInfo_UpdateUserBlackListInDb,
                                user.UserId, user.UserLogin, user.CurrentChatId, user.CurrentChatName);
                            
                            // Доєднання сутностей SystemBody та HashBody
                            // Серіалізація колекції Чорний Список для відправки на сервер - для збереження змін
                            string usersBlackListJson = JsonSerializer.Serialize<List<ModelUsersLoginAndId>>(usersBlackList);

                            // доєднання серіалізованої колекції Чорний Список до повідомлення 
                            messageForSendingBlackListUpdate.SystemBody = usersBlackListJson;

                            // Відправка повідомлення
                            MessageSender(messageForSendingBlackListUpdate);
                        }
                    }
                    // Завантаження актуальної інф про Клієнта та виведення Чат-редактора користувача
                    else if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UserAllInfoIsUpToDateForChatList) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForChatList)
                        ))
                    {
                        // Отримання актуальної інф про поточного користувача
                        if (messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_SendingTheCurrentUserAllInfoForChatList))
                        {
                            // Ініціалізація змінної UserForSending user
                            // - містить максимум інф про стан даного користувача на момент підключення до сервера
                            // Отримання серіалізованого об'єкта UserForSending
                            string userForSendingJson = messageForSendingReceived.SystemBody;

                            // Десеріалізація об'єкта UserForSending та ініціалізація змінної екземпляра user
                            user = JsonSerializer.Deserialize<UserForSending>(userForSendingJson);

                            // Ініціалізація активного елемента - перемикача чатів
                            InitComboBoxChatList();
                        }

                        // Завантаження форми Чат-редактора
                        List<ModelChatsNameAndId> usersChatList = user.ChatsList;
                        List<ModelUsersLoginAndId> usersList = user.ChatsList.Find(c => c.ChatName == SystemInfo.SystemInfo_GroupChat).UsersInChat;

                        FormChatsList form = new FormChatsList(usersChatList, usersList, user.UserId, user.UserLogin);
                        if(form.ShowDialog() == DialogResult.OK)
                        {
                            // Формування повідомлення на оновлення списку чатів  
                            // Створення повідомлення для відправки серверу MessageForSending
                            MessageForSending messageForSendingChatListUpdate = new MessageForSending(
                                SystemInfo.SystemInfo_EmptyMessageBody, 
                                SystemInfo.SystemInfo_UpdateUserChatListInDb,
                                user.UserId, user.UserLogin, user.CurrentChatId, user.CurrentChatName);
                            
                            // Доєднання сутностей SystemBody та HashBody
                            // Серіалізація колекції Список Чатів для відправки на сервер - для збереження змін
                            string usersChatListJson = JsonSerializer.Serialize<List<ModelChatsNameAndId>>(usersChatList);

                            // доєднання серіалізованої колекції Список Чатів до повідомлення 
                            messageForSendingChatListUpdate.SystemBody = usersChatListJson;

                            // Відправка повідомлення
                            MessageSender(messageForSendingChatListUpdate);
                        }

                    }
                    // Обробка у разі отримання повідомлення про перепідключення до Системного Групового чату після видалення поточного чату або
                    // видалення з числа користувачів поточного чату
                    else if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_DeletedCurrentChat) ||
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_RemovedFromCurrentChatUsers)
                        ))
                    {
                        // Отримання серіалізованого об'єкта UserForSending
                        string userForSendingJson = messageForSendingReceived.SystemBody;

                        // Виведенння повідомлення про перепідключення до Системоного Групового чату
                        MessageBox.Show(messageForSendingReceived.GetFullMessageLong());
                        messageForSendingReceived.Body = "";

                        // Десеріалізація об'єкта UserForSending та ініціалізація змінної екземпляра user
                        user = JsonSerializer.Deserialize<UserForSending>(userForSendingJson);

                        // Ініціалізація списку доступних чатів з виділенням поточного чату
                        InitComboBoxChatList();

                        // Очищенні візуального компоненту - поля статистики чату
                        tbChatMessages.BeginInvoke(new Action<string>(TbChatMessagesUpdate), "");
                    }
                    // Оновлення даний про клієнта - не передбачає інших дій
                    else if (messageForSendingReceived != null && (
                        messageForSendingReceived.SystemInfo.Equals(SystemInfo.SystemInfo_UpdatedUserInformation)
                        ))
                    {
                        // Ініціалізація змінної UserForSending user
                        // - містить максимум інф про стан даного користувача на момент підключення до сервера
                        // Отримання серіалізованого об'єкта UserForSending
                        string userForSendingJson = messageForSendingReceived.SystemBody;

                        // Десеріалізація об'єкта UserForSending та ініціалізація змінної екземпляра user
                        user = JsonSerializer.Deserialize<UserForSending>(userForSendingJson);

                        // Ініціалізація активного елемента - перемикача чатів
                        InitComboBoxChatList();
                    }

                    // Виведення повідомлення у візуальний компонент
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

        // Алгоритм Отримання повідомлень
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

        // Відображення повідомлення у візуальному компоненті
        private void AddTextToTbChatMessages(string str)
        {
            StringBuilder builder = new StringBuilder(tbChatMessages.Text);
            builder.Append(str.Trim());
            builder.AppendLine(SystemInfo.SystemInfo_Separator);
            tbChatMessages.Text = builder.ToString();
            tbChatMessages.SelectionStart = tbChatMessages.Text.Length; // переміщення каретки в кінець тексту
            tbChatMessages.ScrollToCaret(); // скролінг вікна до позиції каретки (у дному разі до кінця тексту)

            // Перенос фокуса на візуальний елемент - курсора у поле введення нового повідомлення
            tbUserMessage.Select();
        }

        // Метод очищення візуального компоненту
        private void TbChatMessagesUpdate(string str)
        {
            tbChatMessages.Text = str;
            tbChatMessages.SelectionStart = tbChatMessages.Text.Length; // переміщення каретки в кінець тексту
            tbChatMessages.ScrollToCaret(); // скролінг вікна до позиції каретки (у дному разі до кінця тексту)

            // Перенос фокуса на візуальний елемент - курсора у поле введення нового повідомлення
            tbUserMessage.Select();
        }


        // Алгоритм підготовки та відправлення повідомлень від клієнта до сервера
        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            // Створення повідомлення для відправки клієнту 
            MessageForSending messageFromClientToClients = new MessageForSending(
                tbUserMessage.Text + SystemInfo.SystemInfo_Separator, SystemInfo.SystemInfo_UserMessage, user.UserId, user.UserLogin, user.CurrentChatId, user.CurrentChatName);

            MessageSender(messageFromClientToClients);

            tbUserMessage.Clear();
        }

        // Відправлення повідомлень
        private void MessageSender(MessageForSending message)
        {
            // Серіалізація повідомлення 
            string messageFromClientToServerJson = JsonSerializer.Serialize<MessageForSending>(message);

            // Відправка повідомлення на адресу сервера
            byte[] data = Encoding.Default.GetBytes(messageFromClientToServerJson);
            EndPoint remoteEndpoint = new IPEndPoint(ipAddress, remotePort);
            listeningSocket.SendTo(data, remoteEndpoint);
        }

        // Алгоритм закриття з'єднання та відправки на сервер повідомлення про вихід клієнта з чату 
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Припинення виконання методу у випадку, коли клієнт не заходив до чату
            if (listeningSocket == null)
            {
                return;
            }

            // Створення повідомлення (MessageForSending) клієнт-сервер про відключення клієнта
            MessageForSending messageForSendingUserDisconnect = creator.CreateMessageForSending(
                SystemInfo.SystemInfo_EmptyMessageBody,
                SystemInfo.SystemInfo_UserDisconnect,
                user.UserId,
                user.CurrentChatId);

            // Серіалізація повідомлення
            string messageForSendingUserDisconnectJson = JsonSerializer.Serialize<MessageForSending>(messageForSendingUserDisconnect);

            // Відправка повідомлення про відключення клієнта
            byte[] data = Encoding.Default.GetBytes(messageForSendingUserDisconnectJson);
            EndPoint remoteEndpoint = new IPEndPoint(ipAddress, remotePort);
            listeningSocket.SendTo(data, remoteEndpoint);

            // Закриття з'єднання
            SocketClose();
        }

        // Зміна інтерфейсу - кнопки відправки повідомлень в залежності
        // від заповненості компонента для введення повідомлень
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

        // Закриття з'єднання
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
            // Захист від спрацювання події до завершення процесу налаштування компонента cbChatsList,
            // а також від запуску методу при повторному натисканні на вже завантажений елемент (чат, до якого
            // підключений на даний момент користувач)
            if (isCbChatsListInitComplete && (selectedChatIndexInCB != int.Parse("" + cbChatsList.SelectedValue)))
            {
                selectedChatIndexInCB = int.Parse("" + cbChatsList.SelectedValue);

                // Створення повідомлення для відправки серверу MessageForSending
                MessageForSending messageForSendingForChangeChat = creator.CreateMessageForSending(
                    SystemInfo.SystemInfo_EmptyMessageBody,
                    SystemInfo.SystemInfo_ConnectChat, user.UserId, int.Parse("" + cbChatsList.SelectedValue));

                // Можливо - доєднання сутностей SystemBody та HashBody
                // Серіалізація повідомлення

                // Відправка повідомлення
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
            // Очищення візуального компонента
            tbChatMessages.BeginInvoke(new Action<string>(TbChatMessagesUpdate), "");

            // Створення повідомлення для відправки серверу MessageForSending
            MessageForSending messageGettingHistory = new MessageForSending(
                SystemInfo.SystemInfo_EmptyMessageBody,
                SystemInfo.SystemInfo_GetChatHistory,
                user.UserId, user.UserLogin, user.CurrentChatId, user.CurrentChatName);

            // Можливо - доєднання сутностей SystemBody та HashBody
            string dateHistoryStart = JsonSerializer.Serialize<DateTime>(date);
            // Доєднання серіалізованого параметра до моделі
            messageGettingHistory.SystemBody = dateHistoryStart;

            // Відправка повідомлення
            MessageSender(messageGettingHistory);
        }

        private void chatContactsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Запит до сервера на предмет оновлення списку користувачів чату
            // Створення повідомлення для відправки серверу MessageForSending
            MessageForSending messageForSendingToGetUserActual = new MessageForSending(
                SystemInfo.SystemInfo_EmptyMessageBody, 
                SystemInfo.SystemInfo_UpdateUserAllInfo, 
                user.UserId, user.UserLogin, user.CurrentChatId, user.CurrentChatName);
            
            // Хешування наявного об'єкта UserForSending? user
            // - сервер перевірить хеш актуального об'єкта з отриманим та надішле у відповідь
            // актуальний об'єкт або повідомлення про актуальність 
            string userHesh = user.GetHashCode().ToString();

            // Доєднання сутностей SystemBody та HashBody
            messageForSendingToGetUserActual.HashBody = userHesh;

            // Відправка повідомлення
            MessageSender(messageForSendingToGetUserActual);
        }

        private void blackListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Запит до сервера на предмет оновлення чорного списку користувача
            // Створення повідомлення MessageForSending для відправки серверу 
            MessageForSending messageForSendingToGetUserActual = new MessageForSending(
                SystemInfo.SystemInfo_EmptyMessageBody,
                SystemInfo.SystemInfo_UpdateUserAllInfoForBlackList,
                user.UserId, user.UserLogin, user.CurrentChatId, user.CurrentChatName);

            // Хешування наявного об'єкта UserForSending? user
            // - сервер перевірить хеш актуального об'єкта з отриманим та надішле у відповідь
            // актуальний об'єкт або повідомлення про актуальність 
            string userHesh = user.GetHashCode().ToString();

            // Доєднання сутностей SystemBody та HashBody
            messageForSendingToGetUserActual.HashBody = userHesh;

            // Відправка повідомлення
            MessageSender(messageForSendingToGetUserActual);
        }

        private void chatEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Запит до сервера на предмет оновлення даних користувача для запуска Чат-Редактора 
            // Створення повідомлення MessageForSending для відправки серверу 
            MessageForSending messageForSendingToGetUserActual = new MessageForSending(
                SystemInfo.SystemInfo_EmptyMessageBody,
                SystemInfo.SystemInfo_UpdateUserAllInfoForChatEditor,
                user.UserId, user.UserLogin, user.CurrentChatId, user.CurrentChatName);

            // Хешування наявного об'єкта UserForSending? user
            // - сервер перевірить хеш актуального об'єкта з отриманим та надішле у відповідь
            // актуальний об'єкт або повідомлення про актуальність 
            string userHesh = user.GetHashCode().ToString();

            // Доєднання сутностей до HashBody
            messageForSendingToGetUserActual.HashBody = userHesh;

            // Відправка повідомлення
            MessageSender(messageForSendingToGetUserActual);
        }

    }
}

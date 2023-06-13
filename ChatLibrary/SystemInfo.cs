using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary
{
    public static class SystemInfo
    {
        public const int SystemInfo_NumberOfMessagesInTheSendingPackage = 40; // Кількість повідомлень у пакеті відправки - використовується при відправленні колекції повідомлень - визначає скільки текстових повідомлень можна покласти у одне та відправити одним об'єктом


        public const string SystemInfo_AuthorizationUser = "client-to-server-authorizUser";
        public const string SystemInfo_RegistrationUser = "client-to-server-registrationUser";
        public const string SystemInfo_NewUser = "client-to-server-newUser";

        public const string SystemInfo_UserTrue = "server-to-client-userTrue";
        public const string SystemInfo_UserFalse = "server-to-client-userFalse";
        public const string SystemInfo_AuthorizUserTrue = "server-to-client-authorizUserTrue";
        public const string SystemInfo_AuthorizUserFalse = "server-to-client-authorizUserFalse";
        public const string SystemInfo_RegistrationUserTrue = "server-to-client-registrationUserTrue";
        public const string SystemInfo_RegistrationUserFalse = "server-to-client-registrationUserFalse";
        
        public const string SystemInfo_ConnectChat = "client-to-server-connectChat"; // запит клієнта на підключення до чату
        public const string SystemInfo_ChatIsConnected = "server-to-client-chatIsConnected"; // відповідь сервера про вдале підключення до чату
        public const string SystemInfo_ChatNotConnected = "server-to-client-chatNotConnected"; // відповідь сервера про НЕвдале підключення до чату
        public const string SystemInfo_ClientInNewChat = "client-to-server-clientInNewChat"; // ??? повідомлення клієнт-сервер про підключення до нового чату
        
        public const string SystemInfo_GetChatHistory = "client-to-server-getChatHistory"; // запит клієнта на отримання історії чату
        public const string SystemInfo_NewChatHistoryStart = "server-to-client-newChatHistoryStart"; // оновлена історія чату - відповідь сервера
        public const string SystemInfo_NewChatHistoryContinue = "server-to-client-newChatHistoryContinue"; // оновлена історія чату - відповідь сервера

        public const string SystemInfo_UserMessage = "client-to-server-newMessage";
        public const string SystemInfo_ServerMessage = "server-to-clients-newMessage";

        public const string SystemInfo_UserOut = "client-to-server-userOut";
        public const string SystemInfo_UserDisconnect = "server-to-client-userOut";
        
        public const string SystemInfo_ServerStarted = "server-serverStarted";
        public const string SystemInfo_ServerShutDown = "server-serverShutDown";

        public const string SystemInfo_UpdateUserAllInfo = "client-to-server-updateUserAllInfo"; // запит клієнта на отримання актуальної інф про себе
        public const string SystemInfo_UserAllInfoIsUpToDate = "server-to-client-userAllInfoIsUpToDate"; // відповідь сервера про те, що інф на клієнті актуальна (за порівнням хеш-коду)
        public const string SystemInfo_SendingTheCurrentUserAllInfo = "server-to-client-sendingTheCurrentUserAllInfo"; // відправка актуального об'єкта на клієнт
        
        public const string SystemInfo_UpdateUserAllInfoForBlackList = "client-to-server-updateUserAllInfoForBlackList"; // запит клієнта на отримання актуальної інф про себе для відображення чорного списка
        public const string SystemInfo_UserAllInfoIsUpToDateForBlackList = "server-to-client-userAllInfoIsUpToDateForBlackList"; // відповідь сервера про те, що інф на клієнті актуальна (за порівнням хеш-коду)
        public const string SystemInfo_SendingTheCurrentUserAllInfoForBlackList = "server-to-client-sendingTheCurrentUserAllInfoForBlackList"; // відправка актуального об'єкта на клієнт
        public const string SystemInfo_UpdateUserBlackListInDb = "client-to-server-updateUserBlackListInDb"; // запит клієнта на оновлення у БД чорного списка клієнта
        
        public const string SystemInfo_UpdateUserAllInfoForChatEditor = "client-to-server-updateUserAllInfoForChatEditor"; // запит клієнта на оновлення даних - для запуска Чат-редактора
        public const string SystemInfo_UserAllInfoIsUpToDateForChatList = "server-to-client-userAllInfoIsUpToDateForChatList"; // відповідь сервера про те, що інф на клієнті актуальна (за порівнням хеш-коду)
        public const string SystemInfo_SendingTheCurrentUserAllInfoForChatList = "server-to-client-sendingTheCurrentUserAllInfoForChatList"; // відправка актуального об'єкта на клієнт
        public const string SystemInfo_UpdateUserChatListInDb = "client-to-server-updateUserChatListInDb"; // оновлення чатів у БД після використання Чат-редактора
        public const string SystemInfo_DeletedCurrentChat = "server-to-client-deletedCurrentChat"; // Видалено поточний чат
        public const string SystemInfo_RemovedFromCurrentChatUsers = "server-to-client-removedFromCurrentChatUsers"; // Видалено з числа користувачів поточного чату

        public const string SystemInfo_UpdatedUserInformation = "server-to-client-updatedUserInformation"; // відправка з сервера на клієнт оновленого об'єкта userAllInfo (в результаті дані на клієнті будуть оновлені - ніяких інших дій)
        



        // колекція сервісних повідомлень
        public static List<string> SystemInfoCollection()
        {
            List<string> list = new List<string>();
            list.Add(SystemInfo_AuthorizationUser);
            list.Add(SystemInfo_RegistrationUser);
            list.Add(SystemInfo_NewUser);
            list.Add(SystemInfo_UserTrue);
            list.Add(SystemInfo_UserFalse);
            list.Add(SystemInfo_AuthorizUserTrue);
            list.Add(SystemInfo_AuthorizUserFalse);
            list.Add(SystemInfo_RegistrationUserTrue);
            list.Add(SystemInfo_RegistrationUserFalse);
            list.Add(SystemInfo_ConnectChat);
            list.Add(SystemInfo_ChatIsConnected);
            list.Add(SystemInfo_ChatNotConnected);
            list.Add(SystemInfo_ClientInNewChat);
            list.Add(SystemInfo_GetChatHistory);
            list.Add(SystemInfo_NewChatHistoryStart);
            list.Add(SystemInfo_NewChatHistoryContinue);
            list.Add(SystemInfo_UserMessage);
            list.Add(SystemInfo_ServerMessage);
            list.Add(SystemInfo_UserOut);
            list.Add(SystemInfo_UserDisconnect);
            list.Add(SystemInfo_ServerStarted);
            list.Add(SystemInfo_ServerShutDown);
            list.Add(SystemInfo_UpdateUserAllInfo);
            list.Add(SystemInfo_UserAllInfoIsUpToDate);
            list.Add(SystemInfo_SendingTheCurrentUserAllInfo);
            list.Add(SystemInfo_UpdateUserAllInfoForBlackList);
            list.Add(SystemInfo_UserAllInfoIsUpToDateForBlackList);
            list.Add(SystemInfo_SendingTheCurrentUserAllInfoForBlackList);
            list.Add(SystemInfo_UpdateUserBlackListInDb);
            list.Add(SystemInfo_UpdateUserAllInfoForChatEditor);
            list.Add(SystemInfo_UserAllInfoIsUpToDateForChatList);
            list.Add(SystemInfo_SendingTheCurrentUserAllInfoForChatList);
            list.Add(SystemInfo_UpdateUserChatListInDb);
            list.Add(SystemInfo_DeletedCurrentChat);
            list.Add(SystemInfo_RemovedFromCurrentChatUsers);
            list.Add(SystemInfo_UpdatedUserInformation);
            return list;
        }


        // наступні змінні не є вмістом сервісних повідомлень --------------------------------------------------------------------
        public const string SystemInfo_EmptyMessageBody = "empty message";
        public const string SystemInfo_System = "system";

        public const string SystemInfo_GroupChat = "GroupChat";
        public const string SystemInfo_ServerSystemChat = "serverSystemChat";
        
        public const string SystemInfo_StatusONline = "ONLINE"; // Користувач в мережі
        public const string SystemInfo_StatusOFFline = "offline"; // Користувач НЕ в мережі
        
        public const string SystemInfo_Creator_Star = "*"; 
        public const string SystemInfo_Creator_creator = "creator"; 

        public const string SystemInfo_Separator = "\r\n";

        public const string SystemInfo_MessageNeedUserLogin = "You need to enter the user login";

        public const string SystemInfo_MessageServerStarted = "Server was started at"; // Сервер розпочав роботу о
        public const string SystemInfo_MessageServerShutDown = "Server shut down at"; // Сервер припинив роботу о

        public const string SystemInfo_MessageAllFieldsMustBeСompleted = "All form fields must be completed"; // Всі поля форми повинні бути заповнені
        public const string SystemInfo_MessageEnteredDifferentPasswords = "Different passwords have been entered"; // Введено різні паролі
        public const string SystemInfo_MessageWantTryAgain = "Do you want to try again?"; // Хочеш спробувати ще раз?
        public const string SystemInfo_MessageIncorrectLoginOrPassword = "Incorrect login or password."; // Неправильний логін або пароль
        public const string SystemInfo_MessageLoginAlreadyInUse = "The entered login is already in use."; // Введений логін вже використовується
        public const string SystemInfo_MessageLoginAlreadyExists = "A user with this login already exists."; // Користувач з таким логіном вже існує
        public const string SystemInfo_MessageLogInToEnterTheChat = "Your account is registered. Log in to enter the chat."; // Ваш обліковий запис зареєстровано. Увійдіть, щоб увійти в чат.
        public const string SystemInfo_MessageJoinedTheChat = "Joined the chat: "; // До чату приєднався :
        public const string SystemInfo_MessageNumberOfChatParticipants = "- Number of chat participants: "; // Кількість учасників чату:
        public const string SystemInfo_MessageUserLeftTheChat = "The user has left the chat: "; // Користувач покинув чат
        public const string SystemInfo_MessageFailedToConnectToChat = "Failed to connect to selected chat\r\nTry again later"; // Не вдалось підключитись до обраного чату. Спробуйте пізніше

        public const string SystemInfo_MessageDeleteChat = "Delete chat"; // Видалення чату
        public const string SystemInfo_MessageChatNameFieldMustBeFilled = "The chat name field must be filled"; // Поле назва чату має бути заповнене
        public const string SystemInfo_MessageUserCanEditOnlyChatsCreatedByHimself = "The user can edit or delete only chats created by himself"; // Користувач може редагувати тільки власноруч створені чати
        public const string SystemInfo_MessageDoYouWantToDeleteTheChat = "Do you want to delete the chat "; // Бажаєте видалити чат 
        public const string SystemInfo_MessageYouHaveBeenRemovedFromTheCurrentChat = $"You have been removed from the current chat and will be connected to a {SystemInfo_GroupChat}"; // Вас видалено з поточного чату та буде підключено до групового чату

        public const string SystemInfo_MessageYouHaveBeenDisconnectedFromTheChat = "You have been disconnected from the chat"; // Вас відключили від чату
        public const string SystemInfo_MessageYouHaveBeenConnectedToTheChat = "You have been connected to the chat"; // Вас підключили до чату

        //public const string SystemInfo_Message = ""; // 
    }
}

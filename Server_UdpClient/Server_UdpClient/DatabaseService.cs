using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ChatLibrary;
using Microsoft.EntityFrameworkCore;

namespace Server_UdpClient
{
    public class DatabaseService
    {
        // Збереження повідомлення до БД
        public async void SaveMessageToDb(ChatLibrary.Message? message)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                context.Entry<ChatLibrary.Message>(message).State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }

        // Збереження Користувача до БД
        public async void SaveUserToDb(User? user)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                context.Entry<User>(user).State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }

        // Збереження Чату до БД
        private async void SaveChatToDb(Chat chat)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                context.Entry<Chat>(chat).State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }

        // Додавання користувача до БД
        private async void AddUserToChat(int chatId, int userId)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                var res = context.ChatUsers
                    .Where(c => c.ChatId == chatId)
                    .Select(x => new { userId = x.ChatUserId })
                    .ToList();
                List<int> usersIdList = new List<int>();
                foreach (var item in res)
                {
                    usersIdList.Add(item.userId);
                }

                if (usersIdList.Contains(userId))
                {
                    return;
                }

                ChatUser chatUser = new ChatUser();
                chatUser.ChatId = chatId;
                chatUser.ChatUserId = userId;
                context.Entry<ChatUser>(chatUser).State = EntityState.Added;
                await context.SaveChangesAsync();
            }
        }

        // Отримання та повернення користувача з логіном server
        public User? InitServer()
        {
            return InitOrCreateUser(SystemUsers.SystemUser_Server);
        }


        public User? InitUser(string userLogin)
        {
            User? user = new User();
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                user = context.Users.FirstOrDefault(u => u.Login.Equals(userLogin));
            }
            return user;
        }


        public User? InitUser(int userId)
        {
            User? user = new User();
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                user = context.Users.FirstOrDefault(u => u.Id.Equals(userId));
            }
            return user;
        }


        public User? InitOrCreateUser(string userLogin, string userPassword = "", int isUserSystem = 1)
        {
            if (userPassword.Equals(""))
            {
                userPassword = userLogin;
            }
            byte[] isSystem = new byte[] { 0 };
            if (isUserSystem == 1)
            {
                isSystem = new byte[] { 1 };
            }
            User? user = new User();
            do
            {
                user = InitUser(userLogin);
                if (user == null)
                {
                    User userTemp = new ItemCreator().CreateUser(userLogin, userPassword, isSystem);
                    SaveUserToDb(userTemp);
                }
            } while (user == null);
            return user;
        }

        public Chat? InitGroupChat()
        {
            Chat? groupChat = InitOrCreateChat(SystemInfo.SystemInfo_GroupChat, SystemUsers.SystemUser_Server);
            User? userAll = InitOrCreateUser(SystemUsers.SystemUser_All);
            AddUserToChat(groupChat.Id, userAll.Id);
            return groupChat;
        }


        public Chat? InitChat(string chatName, string userCreatorLogin)
        {
            Chat? chat = new Chat();
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                chat = context.Chats.FirstOrDefault(c => c.ChatName.Equals(chatName) && c.Creator.Login.Equals(userCreatorLogin));
            }
            return chat;
        }


        public Chat? InitChat(int chatId)
        {
            Chat? chat = null;
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                chat = context.Chats.Find(chatId);
            }
            return chat;
        }

        public Chat? InitOrCreateChat(string chatName, string userCreatorLogin)
        {
            Chat? chat = new Chat();
            User? userCreator = null;
            bool chatAdded = false;
            do
            {
                chat = InitChat(chatName, userCreatorLogin);
                if (chat == null)
                {
                    userCreator = InitUser(userCreatorLogin);
                    Chat chatTemp = new ItemCreator().CreateChat(chatName, userCreator.Id);
                    SaveChatToDb(chatTemp);
                    chatAdded = true;
                }
            } while (chat == null);
            if (chatAdded)
            {
                AddUserToChat(chat.Id, userCreator.Id);
            }
            return chat;
        }


        public List<string> InitLoginsOfChatUsersList(int chatsId)
        {
            List<string> logins = new List<string>();
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                logins = context.ChatUsers.Where(c => c.ChatId == chatsId).Select(x => x.ChatUserNavigation.Login).ToList();
            }
            return logins;
        }


        // Отримання з БД об'єкта,  що вказує на наявність доступу клієнта (за Id) до чату (за Id)
        public ChatUser InitChatUser(int userId, int chatId)
        {
            ChatUser chatUser = null;
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                chatUser = context.ChatUsers.FirstOrDefault(c => c.ChatUserId == userId && c.ChatId == chatId);
            }
            return chatUser;
        }


        public bool AuthorizationUser(string userLogin, string password)
        {
            User user = null;
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                user = context.Users.FirstOrDefault(u => u.Login == userLogin && u.Passvord == password);
            }
            return user != null;
        }


        public ModelUserAllInfo CreateUserForSending(string userLogin, IPEndPoint usersIPEndPoint, int currentChatId, string currentChatName, List<string> usersNamesList)
        {
            // отримання id користувача за логіном
            int userId = InitUser(userLogin).Id;
        
            // Оголошення та ініціалізація колекцій, що використовються для створення об'єкта UserForSending
            // Колекція чатів 
            List<ModelChatsNameAndId> chatsList = new List<ModelChatsNameAndId>();
            // Колекція усіх користувачів (без системних)
            List<ModelUsersLoginAndId> usersRegister = new List<ModelUsersLoginAndId>();
            // Колекція користувачів у чорному списку
            List<ModelUsersLoginAndId> blackList = new List<ModelUsersLoginAndId>();

            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                // Отримання колекція усіх зареєстрованих користувачів 
                usersRegister = context.Users.Where(u => u.IsSystem[0] == 0)
                    .Select(x => new ModelUsersLoginAndId(x.Id, x.Login, false))
                    .ToList();

                // Отримання колекції чатів до яких має доступ поточний користувач
                var chats = context.Chats
                        .Select(ch => new {
                            ch.Id,
                            ch.ChatName,
                            Creator = ch.Creator.Login,
                            CreatorId = ch.CreatorId,
                            ChatUsers = ((ch.ChatUsers as List<ChatUser>)
                            .Where(c => c.ChatUserNavigation.Login.ToLower() == SystemUsers.SystemUser_All || c.ChatUserNavigation.Login.ToLower() == userLogin.ToLower())
                            .ToList()
                            .Count)
                        })
                        .Where(ch2 => ch2.ChatUsers > 0)
                        .ToList();
                 
                // Конвертування колекції чатів у інший тип (для передачі)
                chatsList = chats.Select(c => new ModelChatsNameAndId(c.Id, c.ChatName, c.CreatorId)).ToList();
                
                // Перебір колекції чатів та визначення для кожного чату колекції користувчів та кількості користувачів,
                // що мають до нього доступ
                foreach (var chat in chatsList)
                {
                    // Перебір колекції чатів та визначення для кожного чату колекції користувчів, що мають до нього доступ
                    chat.UsersInChat = context.ChatUsers.Where(c => c.ChatId == chat.Id)
                        .Select(x => new ModelUsersLoginAndId(x.ChatUserId, x.ChatUserNavigation.Login, x.ChatUserNavigation.IsSystem[0] > 0 ? true: false))
                        .ToList();

                    // перебір колекції чатів та визначення для кожного чату кількості користувачів, що мають до нього доступ
                    chat.AmountUsersInChat = chat.UsersInChat.Where(u => u.Login == SystemUsers.SystemUser_All).Count() > 0 ?
                        usersRegister.Count() : 
                        chat.UsersInChat.Count();
                }
                
                // Встановлення статусу (ONLINE/offline) користувачам, що знаходяться у колекції зареєстрованих користувачів
                foreach(var userRegister in usersRegister)
                {
                    userRegister.Status = usersNamesList.Contains(userRegister.Login) ? SystemInfo.SystemInfo_StatusONline : SystemInfo.SystemInfo_StatusOFFline;
                }

                // перебір чатів з метою
                // - визначення статусу (ONLINE/offline) користувачів, що мають доступ до чату
                // - заміни колекції корситувачів, що містить системного користувача ALL на колекцію усіх зареєстрованих користувачів
                foreach (var chat in chatsList)
                {
                    bool isAll = false;
                    foreach(var client in chat.UsersInChat)
                    {
                        // Встановлення статусу (ONLINE/offline) користувачам, що знаходяться у колекції користувачів кожного чату
                        client.Status = usersNamesList.Contains(client.Login) ? SystemInfo.SystemInfo_StatusONline : SystemInfo.SystemInfo_StatusOFFline;

                        if(client.Login == SystemUsers.SystemUser_All)
                        {
                            isAll = true;
                        }
                    }
                    if (isAll)
                    {
                        // заміна колекції корситувачів, що містить системного користувача ALL на колекцію усіх зареєстрованих користувачів
                        chat.UsersInChat = usersRegister;
                    }
                }

                // Створення колекції користовачів, що входять до чорного списку - у форматі для передачі
                blackList = context.BlackLists
                .Where(b => b.CreatorId == userId)
                .Select(c => new ModelUsersLoginAndId(c.BlackUserId, c.BlackUser.Login, c.BlackUser.IsSystem[0] > 0 ? true : false))
                .ToList();
            }

            // перебір чорного списку з метою встановлення поточного статусу(ONLINE/offline) користувачів
            foreach (var client in blackList)
            {
                client.Status = usersNamesList.Contains(client.Login) ? SystemInfo.SystemInfo_StatusONline : SystemInfo.SystemInfo_StatusOFFline;
            }

            // Створення об'єкта, що містить максимальну кількість інф про поточний стан користувача (для передачі)
            ModelUserAllInfo user = new ModelUserAllInfo(userId, userLogin, usersIPEndPoint, currentChatId, currentChatName);
            // Доєднання відповідних колекцій
            user.ChatsList = chatsList;            
            user.BlackList = blackList;

            return user;
        }

        // Реєстрація користувача
        internal bool RegistrationUser(string userLogin, string userPassword)
        {
            User user = null;
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                user = context.Users.FirstOrDefault(u => u.Login == userLogin);
                if(user == null)
                {
                    user = InitOrCreateUser(userLogin, userPassword, 0);
                    return true;
                }
            }
            return false;
        }

        // Отриманні історії чату за id чату та стартовою датою
        internal List<ChatLibrary.Message> GetChatHisrory(int chatsId, DateTime dateStartHistory)
        {
            List<ChatLibrary.Message> messagesList = new List<ChatLibrary.Message>();
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                messagesList = context.Messages
                    .Where(m => m.ChatsId == chatsId && m.SendingTime.Date >= dateStartHistory.Date && m.SystemInfo.Equals(SystemInfo.SystemInfo_UserMessage))
                    .ToList();
            }
            return messagesList;
        }

        internal async Task UpdateUsersBlackListAsync(List<ModelUsersLoginAndId>? usersBlackList, int creatorUserId)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                // Отримання колекції користувачів, що входять до Чорного Списка за Id користувача
                List<User> usersInBlackListBefore = context.BlackLists
                    .Where(x => x.CreatorId == creatorUserId)
                    .Select(b => b.BlackUser)
                    .ToList();

                // Видалення з чорного списка користувачів, що були раніше та видалені під час редагування
                // Формування колекції користувачів - на видалення
                List<User> usersForDeleteFromBlackList = usersInBlackListBefore
                    .Where(u => !(usersBlackList.Select(x => x.Login).ToList()).Contains(u.Login))
                    .ToList();
                // Видалення з БД
                foreach (User user in usersForDeleteFromBlackList)
                {
                    BlackList? blackListForDelete = context.BlackLists
                        .FirstOrDefault(b => b.BlackUserId == user.Id && b.CreatorId == creatorUserId);
                    if (blackListForDelete != null)
                    {
                        context.Entry<BlackList>(blackListForDelete).State = EntityState.Deleted;
                    }
                }
                await context.SaveChangesAsync();

                // Додавання до чорного списку користувачів, доданих після редагування (раніше їх не було)
                // Отримання колекції об'єктів на додавання
                List<ModelUsersLoginAndId> usersForAddedToBlackList = usersBlackList
                    .Where(u => !(usersInBlackListBefore.Select(x => x.Id).ToList()).Contains(u.Id))
                    .ToList();

                // Додавання користувачів до Чорного Списку
                BlackList? blackListForAdded = null;
                foreach (var user in usersForAddedToBlackList)
                {
                    blackListForAdded = context.BlackLists
                        .FirstOrDefault(b => b.BlackUserId == user.Id && b.CreatorId == creatorUserId);
                    if (blackListForAdded == null)
                    {
                        blackListForAdded = new BlackList();
                        blackListForAdded.CreatorId = creatorUserId;
                        blackListForAdded.BlackUserId = user.Id;
                        context.Entry<BlackList>(blackListForAdded).State = EntityState.Added;
                    }
                    blackListForAdded = null;
                }
                await context.SaveChangesAsync();
            }
        }

        internal async Task UpdateUsersChatListAsync(List<ModelChatsNameAndId>? usersChatList_New, int creatorUserId)
        {

            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                // Стара колекція чатів усіх користувачів - до оновлення у Чат-редакторі
                await context.Chats.LoadAsync();
                List<Chat> chatListInDb_Old = context.Chats.ToList();

                // Маніпуляції з колекцією чатів (видалення)
                // - перевірка наявності чатів зі старої колекції (чатів користувача) у новій (усіх користувачів) - ВИДАЛЕННЯ у разі відсутності 
                foreach (var chat in chatListInDb_Old.Where(c => c.CreatorId == creatorUserId))
                {
                    // Видалення чатів, що було видалено в Чат-редакторі (немає в новій колекції - є в старій)
                    if(usersChatList_New.Where(c => c.Id == chat.Id).Count() == 0)
                    {
                        Chat chatToDelete = chatListInDb_Old.Find(c => c.Id == chat.Id);
                        context.Entry<Chat>(chatToDelete).State = EntityState.Deleted;
                    }
                }
                await context.SaveChangesAsync();

                // Маніпуляції з колекцією чатів (додавання, модифікація)
                foreach (var chat in usersChatList_New)
                {
                    Chat chatUpdated = new Chat();

                    // Додавання чатів, що були додані в Чат-редакторі (є в новій колекції - немає в старій)
                    if (chatListInDb_Old.Where(c => c.Id == chat.Id).Count() == 0)
                    {
                        // Контроль унікальності назви чату
                        string newChatName = ChatNameChecking(context, chat.ChatName, creatorUserId);

                        // Створення нового чата
                        Chat chatNew = new Chat();
                        chatNew.CreatorId = chat.ChatCreatorId;
                        chatNew.ChatName = newChatName;

                        context.Entry<Chat>(chatNew).State = EntityState.Added;
                        await context.SaveChangesAsync();

                        // Додавання до новоствореного чату колекції користувачів, що мають до нього доступ
                        Chat chatAdded = context.Chats.Where(c => c.ChatName == newChatName).FirstOrDefault();

                        if (chatAdded != null)
                        {
                            foreach (var user in chat.UsersInChat)
                            {
                                ChatUser chatUser = new ChatUser();
                                chatUser.ChatId = chatAdded.Id;

                                // Присвоєння id користувачу, що додається до списку учасників чату
                                // - у разі вибору користувача all (усі зареєстровані користувачі) визначається
                                // id даного системного користувача (користувача all)
                                if (user.Login.Equals(SystemUsers.SystemUser_All))
                                {
                                    chatUser.ChatUserId = InitUser(user.Login).Id;
                                }
                                else
                                {
                                    chatUser.ChatUserId = user.Id;
                                }
                                context.Entry<ChatUser>(chatUser).State = EntityState.Added;
                            }
                            await context.SaveChangesAsync();
                        }
                    }

                    // Оновлення чатів, що були модифіковані в Чат-редакторі (є і в новій колекції і в старій але відрізняється назва чату)
                    else if (chatListInDb_Old.Where(c => c.Id == chat.Id).Count() > 0)
                    {
                        if (chatListInDb_Old.Where(c => c.Id == chat.Id && !c.ChatName.Equals(chat.ChatName)).Count() > 0)
                        {
                            // Контроль унікальності назви чату
                            string newChatName = ChatNameChecking(context, chat.ChatName, creatorUserId);

                            Chat chatToModified = chatListInDb_Old.Find(c => c.Id == chat.Id);
                            chatToModified.ChatName = newChatName;

                            context.Entry<Chat>(chatToModified).State = EntityState.Modified;
                            await context.SaveChangesAsync();
                        }

                        // Оновлення списку користувачів чату
                        // Видалення контактів, що є у старому списку але відсутні у новому
                        Chat chatOld = chatListInDb_Old.Find(c => c.Id == chat.Id);

                        List<ChatUser> usersInChatOld = context.ChatUsers.Where(c => c.ChatId == chatOld.Id).ToList();

                        foreach (var chatUser in usersInChatOld)
                        {
                            if (chat.UsersInChat.Where(c => c.Id == chatUser.ChatUserId).Count() == 0)
                            {
                                if(chatUser != null)
                                {
                                    context.Entry<ChatUser>(chatUser).State = EntityState.Deleted;
                                }
                            }
                        }
                        await context.SaveChangesAsync();

                        // Додавання контактів, що є у новому списку але відсутні у старому
                        List<ModelUsersLoginAndId> usersInChatNew = chat.UsersInChat;
                        foreach (var user in usersInChatNew)
                        {
                            if(usersInChatOld.Where(c => c.ChatUserId == user.Id).Count() == 0)
                            {
                                ChatUser chatUser = new ChatUser();
                                chatUser.ChatId = chat.Id;
                                chatUser.ChatUserId = user.Id;
                                
                                context.Entry<ChatUser>(chatUser).State = EntityState.Added;

                            }
                        }
                        await context.SaveChangesAsync();
                    }

                }
            }
        }


        // Контроль унікальності назви чату
        private string ChatNameChecking(ChatUdp01Context context, string chatName, int creatorUserId)
        {
            string newChatName = chatName;
            int nameNumber = 0;
            string userCreatorLogin = InitUser(creatorUserId).Login;
            List<Chat> chatsListlRepeatedName = context.Chats.Where(c => c.ChatName.Equals(newChatName)).ToList();

            while (chatsListlRepeatedName.Count() > 0)
            {
                if (nameNumber == 0)
                {
                    newChatName += "" + userCreatorLogin;
                    ++nameNumber;
                }
                else
                {
                    newChatName += "" + nameNumber++;
                }
                chatsListlRepeatedName = context.Chats.Where(c => c.ChatName.Equals(newChatName)).ToList();
            }
            return newChatName;
        }

        internal void UpdateUsersChatList(List<ModelChatsNameAndId>? usersChatList_New, int creatorUserId)
        {
            using (ChatUdp01Context context = new ChatUdp01Context())
            {
                // Стара колекція чатів усіх користувачів - до оновлення у Чат-редакторі
                context.Chats.LoadAsync();
                List<Chat> chatListInDb_Old = context.Chats.ToList();

                // Маніпуляції з колекцією чатів (видалення)
                // - перевірка наявності чатів зі старої колекції (чатів користувача) у новій (усіх користувачів) - ВИДАЛЕННЯ у разі відсутності 
                foreach (var chat in chatListInDb_Old.Where(c => c.CreatorId == creatorUserId))
                {
                    // Видалення чатів, що було видалено в Чат-редакторі (немає в новій колекції - є в старій)
                    if (usersChatList_New.Where(c => c.Id == chat.Id).Count() == 0)
                    {
                        Chat chatToDelete = chatListInDb_Old.Find(c => c.Id == chat.Id);
                        context.Entry<Chat>(chatToDelete).State = EntityState.Deleted;
                    }
                }
                context.SaveChangesAsync();

                // Маніпуляції з колекцією чатів (додавання, модифікація)
                foreach (var chat in usersChatList_New)
                {
                    Chat chatUpdated = new Chat();

                    // Додавання чатів, що були додані в Чат-редакторі (є в новій колекції - немає в старій)
                    if (chatListInDb_Old.Where(c => c.Id == chat.Id).Count() == 0)
                    {
                        // Контроль унікальності назви чату
                        string newChatName = ChatNameChecking(context, chat.ChatName, creatorUserId);

                        // Створення нового чата
                        Chat chatNew = new Chat();
                        chatNew.CreatorId = chat.ChatCreatorId;
                        chatNew.ChatName = newChatName;

                        context.Entry<Chat>(chatNew).State = EntityState.Added;
                        context.SaveChangesAsync();

                        // Додавання до новоствореного чату колекції користувачів, що мають до нього доступ
                        Chat chatAdded = context.Chats.Where(c => c.ChatName == newChatName).FirstOrDefault();
                        if (chatAdded != null)
                        {
                            foreach (var user in chat.UsersInChat)
                            {
                                ChatUser chatUser = new ChatUser();
                                chatUser.ChatId = chatAdded.Id;
                                chatUser.ChatUserId = user.Id;
                                context.Entry<ChatUser>(chatUser).State = EntityState.Added;
                            }
                            context.SaveChangesAsync();
                        }
                    }
                    // Оновлення чатів, що були модифіковані в Чат-редакторі (є і в новій колекції і в старій але відрізняється назва чату)
                    else if (chatListInDb_Old.Where(c => c.Id == chat.Id).Count() > 0)
                    {
                        if (chatListInDb_Old.Where(c => c.Id == chat.Id && !c.ChatName.Equals(chat.ChatName)).Count() > 0)
                        {
                            // Контроль унікальності назви чату
                            string newChatName = ChatNameChecking(context, chat.ChatName, creatorUserId);

                            Chat chatToModified = chatListInDb_Old.Find(c => c.Id == chat.Id);
                            chatToModified.ChatName = newChatName;

                            context.Entry<Chat>(chatToModified).State = EntityState.Modified;
                            context.SaveChangesAsync();
                        }

                        // Оновлення списку користувачів чату
                        // Видалення контактів, що є у старому списку але відсутні у новому
                        Chat chatOld = chatListInDb_Old.Find(c => c.Id == chat.Id);

                        List<ChatUser> usersInChatOld = context.ChatUsers.Where(c => c.ChatId == chatOld.Id).ToList();

                        foreach (var chatUser in usersInChatOld)
                        {
                            if (chat.UsersInChat.Where(c => c.Id == chatUser.ChatUserId).Count() == 0)
                            {
                                if (chatUser != null)
                                {
                                    context.Entry<ChatUser>(chatUser).State = EntityState.Deleted;
                                }
                            }
                        }
                        context.SaveChangesAsync();

                        // Додавання контактів, що є у новому списку але відсутні у старому
                        List<ModelUsersLoginAndId> usersInChatNew = chat.UsersInChat;
                        foreach (var user in usersInChatNew)
                        {
                            if (usersInChatOld.Where(c => c.ChatUserId == user.Id).Count() == 0)
                            {
                                ChatUser chatUser = new ChatUser();
                                chatUser.ChatId = chat.Id;
                                chatUser.ChatUserId = user.Id;

                                context.Entry<ChatUser>(chatUser).State = EntityState.Added;
                            }
                        }
                        context.SaveChangesAsync();
                    }
                }
            }

        }


        // -----
    }



}

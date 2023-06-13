using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary
{
    public class ItemCreator
    {
        // Створення повідомлення
        public Message CreateMessage(string messageBody, string systemInfo, int userId, int chatId)
        {
            ChatLibrary.Message message = new ChatLibrary.Message();
            message.Body = messageBody;
            message.SystemInfo = systemInfo;
            message.CreatorUserId = userId;
            message.ChatsId = chatId;
            message.SendingTime = DateTime.Now;
            return message;
        }

        // Створення повідомлення
        public MessageForSending CreateMessageForSending(string messageBody, string systemInfo, int userId, int chatId)
        {
            MessageForSending message = new MessageForSending();
            message.Body = messageBody;
            message.SystemInfo = systemInfo;
            message.CreatorUserId = userId;
            message.ChatsId = chatId;
            message.SendingTime = DateTime.Now;
            return message;
        }

        public ChatLibrary.Message ConvertMessageForSendingToMessage(MessageForSending messageForSending)
        {
            ChatLibrary.Message message = new ChatLibrary.Message();
            message.Body = messageForSending.Body;
            message.SystemInfo = messageForSending.SystemInfo;
            message.CreatorUserId = messageForSending.CreatorUserId;
            message.ChatsId = messageForSending.ChatsId;
            message.SendingTime = messageForSending.SendingTime;
            return message;
        }

        public MessageForSending ConvertMessageToMessageForSending(ChatLibrary.Message message, string userCreatorLogin, string chatName)
        {
            return new MessageForSending(message.Id,
                message.Body,
                message.SystemInfo,
                message.SendingTime,
                message.CreatorUserId,
                userCreatorLogin,
                message.ChatsId,
                chatName, 
                "", 
                "");
        }

        public User CreateUser(string userLogin, string userPassword, byte[] isSystem)
        {
            User user = new User();
            user.Login = userLogin;
            user.Passvord = userPassword;
            user.IsSystem = isSystem;
            return user; 
        }

        public Chat CreateChat(string chatName, int id)
        {
            Chat chat = new Chat();
            chat.ChatName = chatName;
            chat.CreatorId = id;
            return chat;
        }

        public ModelUserAllInfo CreateUserForSending(User user, IPEndPoint usersIPEndPoint, int currentChatId, string currentChatName)
        {
            ModelUserAllInfo userForSending = new ModelUserAllInfo(user.Id, user.Login, usersIPEndPoint, currentChatId, currentChatName);
            
            return userForSending;
        }

        public UserForSending ConvertModelUserAllInfoToUserForSending(ModelUserAllInfo userAllInfo)
        {
            return new UserForSending(
                userAllInfo.UserId, 
                userAllInfo.UserLogin, 
                userAllInfo.CurrentChatId, 
                userAllInfo.CurrentChatName, 
                userAllInfo.ChatsList, 
                userAllInfo.BlackList);
        }
    }
}

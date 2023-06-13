using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary
{
    public class ChatTools
    {

        //public string GetFullMessageLong(ChatLibrary.Message message, User creator = null, Chat currentChat = null)
        //{
        //    string creatorLogin = creator == null ? "0" : creator.Login;
        //    string chatName = currentChat == null ? "0" : currentChat.ChatName;
        //    return $"{message.SendingTime.ToString()} - from {creator?.Login} - chat {currentChat?.ChatName} \r\n- {message.SystemInfo} \r\n- {message.Body}";
        //}


        //public static async Task<int> GetUsersInCollectionAsync(ICollection<ChatUser> chatUsers)
        //{
        //    List<ChatUser> users = chatUsers.Where(u => u.ChatUserNavigation.Login.Equals(SystemUsers.SystemUser_All)).ToList();
        //    using (ChatUdp01Context context = new ChatUdp01Context())
        //    {
        //        if (users.Count > 0)
        //        {
        //            await context.Users.LoadAsync();
        //            var usersAll = context.Users
        //                .Where(u => !u.Login.Equals(SystemUsers.SystemUser_All))
        //                .ToList()
        //                .Count;
        //            return usersAll;
        //        }
        //    }
        //    return chatUsers.Count;
        //}

    }
}

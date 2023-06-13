using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary
{
    public class UserForSending
    {
        public int UserId { get; set; }

        public string UserLogin { get; set; } = "";

        public int CurrentChatId { get; set; }

        public string CurrentChatName { get; set; } = "";

        public List<ModelChatsNameAndId> ChatsList { get; set; } = new List<ModelChatsNameAndId>();

        public List<ModelUsersLoginAndId> BlackList { get; set; } = new List<ModelUsersLoginAndId>();



        public UserForSending()
        { }

        public UserForSending(int userId, string userLogin, int currentChatId, string currentChatName, List<ModelChatsNameAndId> chatsList, List<ModelUsersLoginAndId> blackList)
        {
            UserId = userId;
            UserLogin = userLogin;
            CurrentChatId = currentChatId;
            CurrentChatName = currentChatName;
            ChatsList = chatsList;
            BlackList = blackList;
        }

    }
}

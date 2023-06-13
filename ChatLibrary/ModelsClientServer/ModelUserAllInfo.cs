using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatLibrary
{
    public class ModelUserAllInfo
    {
        public int UserId { get; set; }
        
        public string UserLogin { get; set; } = "";

        public IPEndPoint? UsersIPEndPoint { get; set; }
        
        public int CurrentChatId { get; set; }
        
        public string CurrentChatName { get; set; } = "";
        
        public List<ModelChatsNameAndId> ChatsList { get; set; } = new List<ModelChatsNameAndId>();
        
        public List<ModelUsersLoginAndId> BlackList { get; set; } = new List<ModelUsersLoginAndId>();



        public ModelUserAllInfo()
        { }

        public ModelUserAllInfo(int userId, string userLogin, IPEndPoint usersIPEndPoint, int currentChatId, string currentChatName) : 
            this(userId, userLogin, usersIPEndPoint, currentChatId, currentChatName, 
                new List<ModelChatsNameAndId>(), new List<ModelUsersLoginAndId>()) 
        { }

        public ModelUserAllInfo(int userId, string userLogin, IPEndPoint usersIPEndPoint, int currentChatId, string currentChatName, List<ModelChatsNameAndId> chatsList, List<ModelUsersLoginAndId> blackList)
        {
            UserId = userId;
            UserLogin = userLogin;
            UsersIPEndPoint = usersIPEndPoint;
            this.CurrentChatId = currentChatId;
            this.CurrentChatName = currentChatName;
            this.ChatsList = chatsList;
            this.BlackList = blackList;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ChatLibrary
{

    public partial class MessageForSending
    {
        public int Id { get; set; }

        public string Body { get; set; } = null!;

        public string SystemInfo { get; set; } = null!;

        public DateTime SendingTime { get; set; }

        public int CreatorUserId { get; set; }

        public int ChatsId { get; set; }

        public string CreatorUserLogin { get; set; } = null!;

        public string ChatsName { get; set; } = null!;

        public string HashBody { get; set; }
    
        public string SystemBody { get; set; } = null!;

        //public string Status { get; set; } = null!;


        public MessageForSending() : this(0, "", "", DateTime.Now, 0, "", 0, "", "", "") { }

        public MessageForSending(string body, string systemInfo, int creatorUserId, int chatsId) : 
            this(0, body, systemInfo, DateTime.Now, creatorUserId, "", chatsId, "", "", "") 
        { }

        public MessageForSending(string body,
            string systemInfo, 
            int creatorUserId, 
            string creatorUserLogin,
            int chatsId, 
            string chatsName) : 
            this(0, body, systemInfo, DateTime.Now, creatorUserId, creatorUserLogin, chatsId, chatsName, "", "") 
        { }

        public MessageForSending(string body, 
            string systemInfo, DateTime sendingTime, 
            int creatorUserId, int chatsId) : 
            this(0, body, systemInfo, sendingTime, creatorUserId, "", chatsId, "", "", "") 
        { }

        public MessageForSending(int id, string body, 
            string systemInfo, DateTime sendingTime, 
            int creatorUserId, string creatorUserLogin, 
            int chatsId, string chatsName, 
            string hashBody, string systemBody)
        {
            this.Id = id;
            this.Body = body;
            this.SystemInfo = systemInfo;
            this.SendingTime = sendingTime;
            this.CreatorUserId = creatorUserId;
            this.CreatorUserLogin = creatorUserLogin;
            this.ChatsId = chatsId;
            this.ChatsName = chatsName;
            this.HashBody = hashBody;
            this.SystemBody = systemBody;
        }

        public void AddHashBodyFromString(string str)
        {
            this.HashBody = str.GetHashCode().ToString();
        }

        public string GetFullMessageLong()
        {
            return $"{this.SendingTime.ToString()} " +
                $"- from {this.CreatorUserLogin} " +
                $"- chat {this.ChatsName} \r\n" +
                $"- {this.SystemInfo} \r\n" +
                $"- {this.Body}";
        }

    }

}
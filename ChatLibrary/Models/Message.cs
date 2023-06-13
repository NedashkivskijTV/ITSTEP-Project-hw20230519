using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ChatLibrary
{

    public partial class Message
    {
        public int Id { get; set; }

        public string Body { get; set; } = null!;

        public string SystemInfo { get; set; } = null!;

        public DateTime SendingTime { get; set; }

        public int CreatorUserId { get; set; }

        public int ChatsId { get; set; }

        public virtual Chat Chats { get; set; } = null!;

        public virtual User CreatorUser { get; set; } = null!;

        public string FullMessage => $"{SendingTime.ToString()} - from {CreatorUser.Login} \r\n- {Body}";
        public string FullMessageLong => $"{SendingTime.ToString()} - from {CreatorUser.Login} - chat {Chats.ChatName} \r\n- {Body} \r\n- {SystemInfo}";

    }

}
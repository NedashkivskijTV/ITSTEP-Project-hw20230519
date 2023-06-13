using System;
using System.Collections.Generic;

namespace ChatLibrary
{

    public partial class User
    {
        public int Id { get; set; }

        public string Login { get; set; } = null!;

        public string Passvord { get; set; } = null!;

        public byte[] IsSystem { get; set; } = null!;

        public virtual ICollection<BlackList> BlackListBlackUsers { get; set; } = new List<BlackList>();

        public virtual ICollection<BlackList> BlackListCreators { get; set; } = new List<BlackList>();

        public virtual ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();

        public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }

}
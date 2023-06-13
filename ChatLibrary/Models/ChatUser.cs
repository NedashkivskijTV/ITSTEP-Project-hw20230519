using System;
using System.Collections.Generic;

namespace ChatLibrary
{

    public partial class ChatUser
    {
        public int Id { get; set; }

        public int ChatId { get; set; }

        public int ChatUserId { get; set; }

        public virtual Chat Chat { get; set; } = null!;

        public virtual User ChatUserNavigation { get; set; } = null!;
    }

}
using System;
using System.Collections.Generic;

namespace Client_UdpClient.Models;

public partial class Chat
{
    public int Id { get; set; }

    public string ChatName { get; set; } = null!;

    public int CreatorId { get; set; }

    public virtual ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();

    public virtual User Creator { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}

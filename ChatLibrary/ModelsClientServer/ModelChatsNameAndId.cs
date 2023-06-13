using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary
{
    public class ModelChatsNameAndId
    {
        public int Id { get; set; }
        
        public string ChatName { get; set; } = "";
        
        public int ChatCreatorId { get; set; }

        public int AmountUsersInChat { get; set; }
        
        public List<ModelUsersLoginAndId> UsersInChat { get; set; } = new List<ModelUsersLoginAndId>();



        public ModelChatsNameAndId()
        { }

        public ModelChatsNameAndId(int id, string chatName, int chatCreatorId) : 
            this(id, chatName, chatCreatorId, 0, new List<ModelUsersLoginAndId>()) 
        { }

        public ModelChatsNameAndId(int id, string chatName, int chatCreatorId, int amountUsersInChat, List<ModelUsersLoginAndId> usersInChat)
        {
            this.Id = id;
            this.ChatName = chatName;
            this.ChatCreatorId = chatCreatorId;
            this.AmountUsersInChat = amountUsersInChat;
            this.UsersInChat = usersInChat;
        }

    }
}

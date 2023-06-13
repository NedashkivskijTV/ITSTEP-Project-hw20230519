using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary
{
    public class ModelUsersLoginAndId
    {
        public int Id { get; set; }

        public string Login { get; set; } = "";

        public string Status { get; set; } = "offline";

        public bool IsSystem { get; set; } = false;



        public ModelUsersLoginAndId() { }

        public ModelUsersLoginAndId(int id) : this(id, "", "offline", false) { }

        public ModelUsersLoginAndId(int id, string login) : this(id, login, "offline", false) { }

        public ModelUsersLoginAndId(int id, string login, bool isSystem) : this(id, login, "offline", isSystem) { }

        public ModelUsersLoginAndId(int id, string login, string status, bool isSystem)
        {
            Id = id;
            Login = login;
            Status = status;
            IsSystem = isSystem;
        }

    }
}

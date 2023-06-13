using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatLibrary
{
    public class ParaLoginPass
    {
        public string Login { get; set; }
        public string Pass { get; set; }


        public ParaLoginPass(string login, string pass)
        {
            Login = login;
            Pass = pass;
        }
    }

}

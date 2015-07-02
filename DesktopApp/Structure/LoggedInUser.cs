using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopApp.Structure
{
    public class LoggedInUser
    {
        private string username;
        private int id;
        private string email;
        

        public LoggedInUser(int id, string userName, string email)
        {
            this.id = id;
            this.username = userName;
            this.email = email;
        }


        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
    }
}

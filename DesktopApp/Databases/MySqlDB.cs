using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopApp.Databases
{
    class MySqlDB
    {
        private MySqlConnection connection;
        private bool isOpened = false;

        public MySqlDB()
        {
            connection = new MySqlConnection("SERVER=eu-cdbr-azure-north-c.cloudapp.net;DATABASE=peterbartha;UID=b8da4e27b29571;PASSWORD=ae54484aafb2a67");
        }

        // Open connection to database
        public bool OpenConnection()
        {
            try
            {
                if (!isOpened)
                    connection.Open();
                isOpened = true;
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server. Contact with administrators.");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username or password, please try again!");
                        break;
                }
                return false;
            }
        }

        // Close connection
        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                isOpened = false;
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }


        public MySqlConnection Connection
        {
            get { return connection; }
            set { connection = value; }
        }

    }
}

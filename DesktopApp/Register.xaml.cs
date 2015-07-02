using DesktopApp.Databases;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        Login caller;
        MySqlDB db;

        public Register(Login caller)
        {
            InitializeComponent();

            this.caller = caller;
            this.db = new MySqlDB();

            exitIcon.MouseLeftButtonDown += new MouseButtonEventHandler(Window_Exit);
            backLabel.MouseLeftButtonDown += new MouseButtonEventHandler(backLabel_Click);
            topPanel.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseDown);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            // Form fields
            String strUser = emailTxt.Text;
            String strPass = passwordTxt.Password;
            String strRePass = rePasswordTxt.Password;

            // Pre-checked passoword fields
            if (strPass != strRePass)
            {
                MessageBox.Show("Password is not same!");
                return;
            }
            if (strPass.Length <= 3)
            {
                MessageBox.Show("Password is too short!");
                return;
            }

            // Create salt and hash
            byte[] salt = Hash.PasswordHash.CreateSalt();
            String hash = Hash.PasswordHash.CreateHash(strPass, salt);
            if (db.OpenConnection())
            {
                // Insert new line to database
                using (var com = new MySqlCommand("INSERT INTO Users (Email, Username, Hash, Salt) VALUES(@Email, @User, @Hash, @Salt)", db.Connection))
                {
                    try
                    {
                        com.Parameters.Add(new MySqlParameter("@Email", strUser));
                        com.Parameters.Add(new MySqlParameter("@User", ""));
                        com.Parameters.Add(new MySqlParameter("@Hash", hash));
                        com.Parameters.Add(new MySqlParameter("@Salt", salt));
                        com.ExecuteNonQuery();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Error happend: Could not insert. (" + exception.Message + ")");
                        return;
                    }
                    finally
                    {
                        db.CloseConnection();
                    }
                }
            }

            if (db.Connection.State != System.Data.ConnectionState.Closed)
                db.CloseConnection();

            MessageBox.Show("Sucessfully registered.");
            this.caller.setUserField(strUser);
            this.caller.Show();
            this.Hide();


        }

        private void Window_Exit(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void backLabel_Click(object sender, EventArgs e)
        {
            this.caller.Show();
            this.Hide();
        }

        ~Register()
        {
            db.CloseConnection();
        }
    }
}

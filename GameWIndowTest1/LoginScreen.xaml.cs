﻿using GameWIndowTest1.Global;
using GameWIndowTest1.Global.Datatypes;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
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

namespace GameWIndowTest1
{
/// <summary>
/// Interaction logic for LoginScreen.xaml
/// </summary>
public partial class LoginScreen : Window
{
        static string cs = @"URI=file:main.db";
        public LoginScreen()
        {
            InitializeComponent();

            using (var con = new SQLiteConnection(cs))
            {
                con.Open();

                using var cmd = new SQLiteCommand(con);


                // this will create the users table if it does not currently exists
                // eg if the database was just created
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS 'users' ('userID' INTEGER  PRIMARY KEY NOT NULL,    'username'  TEXT,    'password'  TEXT,    'salt'  TEXT);";
                cmd.ExecuteNonQuery();

            }
        }

        private void login_button_Click(object sender, RoutedEventArgs e)
        {
            string username = username_box.Text;

            Trace.WriteLine("");


            if (!userExists(username)) // if no user exist with this username
            {
                MessageBox.Show("Username not in use");
                return; // return as no user exists so no point checking database more
            }
            string salt;
            using (var con = new SQLiteConnection(cs))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {

                    cmd.CommandText = $"SELECT salt FROM users WHERE username='{username}'"; // get the salt stored for this user
                    using (var reader = cmd.ExecuteReader())
                    {  
                        // in the database
                        reader.Read(); // this is needed to verify the password of the user as it will be hashed
                        salt = reader[0].ToString();
                    }
                }
            }
            Debug.WriteLine($"Salt: {salt}");
            string password = Password.HashString(password_box.Password, salt);
            Debug.WriteLine($"Password: {password}");
            if (userExists(username, password))
            {
                Account account = new Account(username, password);
                SetupForGame gameSetup = new SetupForGame(account);// new Global.Account(username, password));
                this.Close();
            }
            else
            {
                MessageBox.Show("Data incorrect");
                this.Close();
            }

        }

        protected bool userExists(string username, string password = null)
        {
            bool exists = false;
            using (var con = new SQLiteConnection(cs))
            {
                con.Open();

                using (var cmd = new SQLiteCommand(con))
                {

                    cmd.CommandText = $"SELECT userID FROM users WHERE username='{username}'";
                    if (password != null)
                    {
                        cmd.CommandText += $"AND password='{password}'";
                        Debug.WriteLine(cmd.CommandText);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows) // there is a userID for this user
                        {
                            
                            // so it exists
                            exists = true;
                        }
                    }
                }
            }
            return exists;
        }

        private void Create_user_button_Click(object sender, RoutedEventArgs e)
        {
            string username = username_box.Text;
            string salt = Password.generateSalt();
            string password = Password.HashString(password_box.Password, salt);

            using (var con = new SQLiteConnection(cs))
            {
                con.Open();

                using (var cmd = new SQLiteCommand(con))
                {


                    if (userExists(username))
                    {
                        MessageBox.Show("Username already in use");
                        return;
                    }

                    cmd.CommandText = $"INSERT INTO users (username, password, salt) VALUES (\"{username}\", \"{password}\", \"{salt}\")";
                    cmd.ExecuteNonQuery();
                }
            }
            MessageBox.Show($"Added User {username}");
            this.Close();
        }

        private void Guest_Account_Button_Click(object sender, RoutedEventArgs e)
        {
            // create an account with the geust details
            Account account = new Account(Account.guestUsername, Account.guestPassword);
            SetupForGame gameSetup = new SetupForGame(account);// new Global.Account(username, password));
            this.Close();
        }
    }
}

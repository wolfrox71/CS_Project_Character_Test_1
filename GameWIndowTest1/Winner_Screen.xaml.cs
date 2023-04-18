using GameWIndowTest1.Global;
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
using System.Data.SQLite;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace GameWIndowTest1
{
    /// <summary>
    /// Interaction logic for Winner_Screen.xaml
    /// </summary>
    partial class Winner_Screen : Window
    {
        static string cs = @"URI=file:main.db";

        public bool _won;
        public int _score;
        public GameState _state;
        internal Winner_Screen(bool won, int score, GameState state)
        {
            InitializeComponent();
            WinnerID.Text = "You ";
            WinnerID.Text += (won) ? "Won!" : "Lost";
            scoreBox.Text += $"With a score of {score}";
            _won = won;
            _score = score;
            _state = state;
            saveScore();
        }

        public void saveScore()
        {
            // the player lost 
            if (!_won)
            {
                // do not save the score and show a messagebox saying that the score will not be saved
                MessageBox.Show("Score not saved as lost");
                return;
            }
            // if the user is a guest
            if (_state.username == Account.guestUsername)
            {
                // do not save the score
                MessageBox.Show("Score not saved as guest");
                return;
            }
            int userID = 0;

            using (var con = new SQLiteConnection(cs)) 
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {

                    //create the scores table if it does not already exist
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS 'scores' ('scoreID'INTEGER NOT NULL, 'userID' INTEGER , 'scoreValue ' INTEGER, PRIMARY KEY('scoreID' AUTOINCREMENT), FOREIGN KEY('userID') REFERENCES 'users'('userID'));";
                    cmd.ExecuteNonQuery();
                }
                using (var cmd = new SQLiteCommand(con))
                {
                    // get the userID associated with the username
                    cmd.CommandText = $"SELECT userID from users WHERE username='{_state.username}';";
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read(); // this is needed to verify the password of the user as it will be hashed
                        userID = Convert.ToInt32((reader[0]).ToString());
                    }
                }
            }
            using (var con = new SQLiteConnection(cs))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {

                    // insert the score into the table
                    cmd.CommandText = $"INSERT INTO scores (\"userID\",\"score\") VALUES({userID},{_score});";
                    cmd.ExecuteNonQuery();
                }
            }
            MessageBox.Show("Score saved");
        }
    }
}

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
using System.IO;
using System.Text.Json;
using Microsoft.Win32;

namespace GameWIndowTest1
{
    /// <summary>
    /// Interaction logic for SetupForGame.xaml
    /// </summary>
    public partial class SetupForGame : Window
    {

        public SetupForGame(Account account)
        {
            InitializeComponent();

            List<character> friendly_characters = new List<character> {
                new character(40, "Character1", "Player 1", true),
                new character(40, "Character2", "Player 2", true)
            };

            GameState state = new GameState(0, 5, friendly_characters, 1000, account);


            string json_filename = $"Saves/{account.username}.json";

            // if a save exists for the current user
            if (File.Exists(json_filename))
            {


                MessageBoxButton buttons = MessageBoxButton.YesNo;
                MessageBoxResult result = MessageBox.Show("Do you want to use the save?", "Save found", buttons);
                // if they want to use the save
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {

                        string json_text = File.ReadAllText(json_filename);
                        state = JsonSerializer.Deserialize<GameState>(json_text); // convert the state to that of the save

                    } 
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occured while trying to restore from state");
                    }
                }
            }

            //MainWindow game_Window = new MainWindow(friendly_characters, dead_characters, 0);
            out_of_combat game_Window = new out_of_combat(state);
            game_Window.Show();
            this.Close();
        }
    }
}

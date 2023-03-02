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

namespace GameWIndowTest1
{
    /// <summary>
    /// Interaction logic for SetupForGame.xaml
    /// </summary>
    public partial class SetupForGame : Window
    {

        public SetupForGame()
        {
            InitializeComponent();
            List<character> friendly_characters = new List<character> {
                new character(40, "Character1", true),
                new character(40, "Character2", true)
            };
            List<character> dead_characters = new List<character>();

            //MainWindow game_Window = new MainWindow(friendly_characters, dead_characters, 0);
            out_of_combat game_Window = new out_of_combat(1,5, friendly_characters);
            game_Window.Show();
            this.Close();
        }
    }
}

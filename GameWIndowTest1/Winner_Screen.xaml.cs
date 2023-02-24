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
    /// Interaction logic for Winner_Screen.xaml
    /// </summary>
    public partial class Winner_Screen : Window
    {
        character _winner;
        public Winner_Screen(character Winner_Name)
        {
            InitializeComponent();
            _winner = Winner_Name;
            WinnerID.Text = $"{_winner.name} is the winner";
        }
    }
}

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
    partial class Winner_Screen : Window
    {
        internal Winner_Screen(character winner)
        {
            InitializeComponent();
            WinnerID.Text = $"{winner.name} is the winner";
            WinnerID.Text += $"\nWith {winner.health} health remaining";
        }
    }
}

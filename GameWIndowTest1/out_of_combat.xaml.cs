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
    /// Interaction logic for out_of_combat.xaml
    /// </summary>
    public partial class out_of_combat : Window
    {
        int _r_n;
        int _max_r_n;
        List<character> _friendly_characters;
        List<character> _dead_characters;
        List<TextBlock> _character_blocks;
        List<character> _all_characters = new List<character>();

        int heal_ammount = 10;
        int heal_uses = 5;

        int selected_index = 0;
        public out_of_combat(int round_number, int max_number_of_rounds, List<character> friendly_characters, List<character> dead_characters)
        {
            InitializeComponent();
            _r_n = round_number;
            _max_r_n = max_number_of_rounds;
            _friendly_characters = friendly_characters;
            _dead_characters= dead_characters;

            foreach (character _f in friendly_characters)
            {
                _all_characters.Add(_f);
            }
            foreach (character _f in dead_characters)
            {
                _all_characters.Add(_f);
            }

            _character_blocks = new List<TextBlock> { Character_1_Block, Character_2_Block };

            _all_characters[0].takedamage(15);
            _all_characters[1].takedamage(8);
            set_character_details();
        }

        public void set_character_details()
        {
            character _current = _all_characters[selected_index];
            // if the selected character cannot be healed anymore
            if (_current.dead || _current.health >= _current.max_health)
            {
                Healing_Button.IsEnabled = false;
            }
            else
            {
                Healing_Button.IsEnabled = true;
            }

            for (int index = 0; index<_all_characters.Count; index++)
            {
                character current = _all_characters[index];
                TextBlock block = _character_blocks[index];

                block.Text = $"{current.name} {current.health}/{current.max_health}";
                if (current.dead)
                {
                    block.Foreground = Brushes.Red;
                }
            }
        }

        private void Next_fight(object sender, RoutedEventArgs e)
        {
            MainWindow game_window = new MainWindow(_friendly_characters, _dead_characters, _r_n + 1, _max_r_n);
            game_window.Show();
            this.Close();
        }

        private void Radio_Changed(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            selected_index = Int32.Parse(rb.Name.Substring(rb.Name.Length-1,1))-1;

            if (_all_characters.Count == 0)
            {
                Healing_Button.IsEnabled = false;
                return;
            }
            set_character_details();
        }

        private void Heal_Button(object sender, RoutedEventArgs e)
        {
            character current = _all_characters[selected_index];
            Mid_Block.Text = $"{current.name} had {current.health}";

            Mid_Block.Text += $"\nHealed for {heal_ammount}";

            // heal the character for that ammount
            current.heal(heal_ammount);

            Mid_Block.Text += $"\nand now has {current.health} health";

            heal_uses--;

            Mid_Block.Text += $"\n{heal_uses} uses remaining";
            if (heal_uses <= 0)
            {
                // disable the healing button as it has run out of uses
                Healing_Button.IsEnabled = false;
            }
            set_character_details();
        }
    }
}

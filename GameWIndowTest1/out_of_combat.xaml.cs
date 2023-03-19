using GameWIndowTest1.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using GameWIndowTest1.Abilities;

namespace GameWIndowTest1
{
    /// <summary>
    /// Interaction logic for out_of_combat.xaml
    /// </summary>
    public partial class out_of_combat : Window
    {
        int _r_n;
        int _max_r_n;
        List<character> _characters = new List<character>();
        List<TextBlock> _character_blocks;

        int heal_ammount = 10;
        int heal_cost = 500;
        int revive_cost = 1000;

        int selected_index = 0;

        GameState state;

        public out_of_combat(GameState _current_state)
        {
            InitializeComponent();
            _r_n = _current_state.current_wave_number;
            _max_r_n = _current_state.max_wave_number;
            _characters = _current_state.characters;

            state = _current_state;

            _character_blocks = new List<TextBlock> { Character_1_Block, Character_2_Block };

            //_characters[0].takedamage(15);
            //_characters[1].takedamage(8);
            set_character_details();

            Mid_Block.Text = $"{state.money} money";
            Mid_Block.Text += $"\nHeals cost {heal_cost} for {heal_ammount} healing";
            Mid_Block.Text += $"\nRevives cost {revive_cost}";

        }

        public void set_character_details()
        {
            character _current = _characters[selected_index];
            // if the selected character cannot be healed anymore
            if (_current.IsDead || _current.health >= _current.max_health || state.money <= heal_cost)
            {
                Healing_Button.IsEnabled = false;
            }
            else
            {
                Healing_Button.IsEnabled = true;
            }

            if (_current.IsDead && state.money <= revive_cost)
            {
                Revive_Button.IsEnabled = true;
            }
            else
            {
                Revive_Button.IsEnabled = false;
            }



            for (int index = 0; index<_characters.Count; index++)
            {
                character current = _characters[index];
                TextBlock block = _character_blocks[index];

                block.Text = $"{current.display_name} {current.health}/{current.max_health}";
                if (current.IsDead)
                {
                    block.Foreground = Brushes.Red;
                }
                else
                {
                    block.Foreground = Brushes.Black;
                }
            }

            List<ComboBox> ComboBoxes = new List<ComboBox> { Ability_Box1, Ability_Box2 };
            foreach (ComboBox box in ComboBoxes)
            {
                // clear all the items in the current box
                box.Items.Clear();
                // and add each ability name to the box
                foreach (ability _ability in _current.get_valid_abilities())
                {
                    
                    box.Items.Add(_ability.name);
                }
            }
        }

        private void Next_fight(object sender, RoutedEventArgs e)
        {
            // increment the current wave number of the game
            state.current_wave_number++;
            MainWindow game_window = new MainWindow(state);
            game_window.Show();
            this.Close();
        }

        private void Radio_Changed(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            selected_index = Int32.Parse(rb.Name.Substring(rb.Name.Length-1,1))-1;

            if (_characters.Count == 0)
            {
                Healing_Button.IsEnabled = false;
                return;
            }
            set_character_details();
        }

        private void Heal_Button(object sender, RoutedEventArgs e)
        {
            Mid_Block.Text = $"\n{state.money-heal_cost} money";
            Mid_Block.Text += $"\nHeals cost {heal_cost} for {heal_ammount} healing";
            Mid_Block.Text += $"\nRevives cost {revive_cost}";

            character current = _characters[selected_index];
            Mid_Block.Text += $"\n\n{current.display_name} had {current.health}";

            Mid_Block.Text += $"\nHealed for {heal_ammount}";

            // heal the character for that ammount
            current.heal(heal_ammount);

            Mid_Block.Text += $"\nand now has {current.health} health";

            state.money -= heal_cost;

            if (state.money <= heal_cost);
            {
                // disable the healing button as it has run out of uses
                Healing_Button.IsEnabled = false;
            }
            set_character_details();
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(state, options);

            string fileName = $"Saves/{state.username}.json";

            if (!Directory.Exists("Saves"))
            {
                Directory.CreateDirectory("Saves");
            }

            File.WriteAllText(fileName, jsonString);
            MessageBox.Show("Saved");

        }

        private void Revive_Button_Click(object sender, RoutedEventArgs e)
        {
            character current = _characters[selected_index];
            Mid_Block.Text = $"{state.money-revive_cost} money";
            Mid_Block.Text += $"\nHeals cost {heal_cost} for {heal_ammount} healing";
            Mid_Block.Text += $"\nRevives cost {revive_cost}";

            Mid_Block.Text += $"\n\n{current.display_name} had {current.health}";

            // if the current it not dead
            if (!current.IsDead)
            {
                return;
            }

            current.revive();

            state.money -= revive_cost;

            if (state.money <= revive_cost)
            {
                // disable the healing button as it has run out of uses
                Revive_Button.IsEnabled = false;
            }
            set_character_details();
        }

        private void Ability_box_changed(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            int ability_index = Int32.Parse(box.Name.Substring(box.Name.Length - 1));
            MessageBox.Show($"Box {ability_index} now shows {box.SelectedValue.ToString()}");
        }
    }
}

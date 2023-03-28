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
        List<ComboBox> ComboBoxes;

        int heal_ammount = 10;
        int heal_cost = 500;
        int revive_cost = 1000;
        
        bool init_setup = true;

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
            ComboBoxes = new List<ComboBox> { Ability_Box1, Ability_Box2, Ability_Box3, Ability_Box4 };
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

            if (_current.IsDead && state.money >= revive_cost)
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

            List<ComboBox> ComboBoxes = new List<ComboBox> { Ability_Box1, Ability_Box2, Ability_Box3, Ability_Box4 };
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
            init_setup = true;
            set_drop_menu_values();
            init_setup = false;
        }

        private void Next_fight(object sender, RoutedEventArgs e)
        {
            convert_boxes_to_abilities();
            // increment the current wave number of the game
            state.current_wave_number++;
            MainWindow game_window = new MainWindow(state);
            game_window.Show();
            this.Close();
        }

        private void Radio_Changed(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            // update the last characters abilities
            convert_boxes_to_abilities();
            // then change who is the selected character
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
            if (init_setup) { return; }

            character current_character = state.characters[selected_index];

            ComboBox box = sender as ComboBox;

            if (box == null) { MessageBox.Show("Null so returning");  return; }
            int ability_index = Int32.Parse(box.Name.Substring(box.Name.Length - 1))-1;
            var selected = box.SelectedItem.ToString();
            ability new_ability = find_ability_from_name(selected);
            // change the ability to the new ability

            if (current_character.IsAbilityEquiped(new_ability))
            {
                if (current_character.abilities[ability_index].name == new_ability.name)
                {
                    // this is to stop an a recusive error when you call set_last_item_box
                    return;
                }

                // output a message saying that you cannot have the same ability equiped twice on the same character
                MessageBox.Show($"{new_ability.name} is already equiped so cannot be equiped again");
                set_last_item_box(box, current_character.abilities[ability_index]);
                // and return so that the ability is not change
                return;
            }

            // output that the ability is about to be changed

            MessageBox.Show($"Ability {ability_index} changed from {current_character.abilities[ability_index].name} to {new_ability.name}");
            // and change it
            current_character.updateAbility(new_ability, ability_index);            
       }

        public void set_last_item_box(ComboBox box, ability previous_ability)
        {
            // this function is to set the previous_ability as the current ability
            // as the selection was changed in the box but the ability didnt update

            foreach (object _obj in box.Items)
            {
                if (_obj.ToString() == previous_ability.name)
                {
                    //MessageBox.Show($"Found {previous_ability.name}");
                    box.SelectedItem = _obj;
                    return;
                }
            }
            //MessageBox.Show($"Could not find {previous_ability.name}");
            return;
        }

        public void convert_boxes_to_abilities()
        {
            if (init_setup) { return;}
            /*
            character current = _characters[selected_index];

            // go through each box and assign that ability to the ability of the character
            for (int i = 0; i < current.abilities.Length; i++)
            {
                ComboBox box = ComboBoxes[i];
                var selected = box.SelectedItem.ToString();
                current.abilities[i] = find_ability_from_name(selected);
            */
        }

        public ability find_ability_from_name(string name)
        {
            List<ability> validAbilities = _characters[selected_index].get_valid_abilities();
            // go through each ability of the current character
            foreach (ability _current in validAbilities)
            {
                // and check to see if the ability is the same the one looking for
                if (_current.name == name)
                {
                    // if it is, return this ability
                    return _current;
                }
            }
            MessageBox.Show($"Could not find '{name}'");
            // if no ability is found, return no ability selected
            return character.no_ability_selected;
        }
    
        public void set_drop_menu_values()
        {
            character current_character = state.characters[selected_index];

            for (int i = 0; i < current_character.abilities.Length; i++) 
            {
                ComboBox box = ComboBoxes[i];
                string current_ability = current_character.abilities[i].name;
                //MessageBox.Show(current_ability.ToString());
                box.SelectedIndex = box.Items.IndexOf(current_character.abilities[i].name);
            }
        }
    }
}

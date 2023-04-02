using GameWIndowTest1.Abilities;
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
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;

namespace GameWIndowTest1
{
    /// <summary>
    /// Interaction logic for shop.xaml
    /// </summary>
    public partial class shop : Window
    {
        GameState state;
        int character_ID;


        int heal_ammount = 10;
        int heal_cost = 500;
        int revive_cost = 1000;
        int restore_uses_cost = 200;
        int upgrade_cost = 300;

        public shop(GameState _state, int _characterID)
        {
            character_ID = _characterID;
            state = _state;
            InitializeComponent();
            set_ability_selector_vals();
            set_character_details();
            show_character_infomation(0);
        }

        private void Heal_Button(object sender, RoutedEventArgs e)
        {
            character current = state.characters[character_ID];

            show_character_infomation(heal_cost);

            Mid_Block.Text += $"\n\n{current.display_name} had {current.health}";

            Mid_Block.Text += $"\nHealed for {heal_ammount}";

            // heal the character for that ammount
            current.heal(heal_ammount);

            Mid_Block.Text += $"\nand now has {current.health} health";

            state.money -= heal_cost;

            if (state.money <= heal_cost) ;
            {
                // disable the healing button as it has run out of uses
                Healing_Button.IsEnabled = false;
            }
            set_character_details();
            show_character_infomation(0);
        }

        public void set_character_details()
        {
            character current = state.characters[character_ID];

            // if the selected character cannot be healed anymore
            if (current.IsDead || current.health >= current.max_health || state.money <= heal_cost)
            {
                Healing_Button.IsEnabled = false;
            }
            else
            {
                Healing_Button.IsEnabled = true;
            }

            if (current.IsDead && state.money >= revive_cost)
            {
                Revive_Button.IsEnabled = true;
            }
            else
            {
                Revive_Button.IsEnabled = false;
            }
            ability selected_ability = getSelectedAbility();
            if (state.money <= restore_uses_cost || (selected_ability == character.no_ability_selected) || (selected_ability.uses_remaining == selected_ability.max_number_of_uses))
            {
                // cannot use the restore if:
                // not enough money
                // no ability is selected
                // ability is already at max number of uses remaining
                Restore_Uses.IsEnabled = false;
            }
            else
            {
                Restore_Uses.IsEnabled = true;
            }
            if (state.money <= upgrade_cost)
            {
                Upgrade_Ability.IsEnabled = false;
            }
            else
            {
                Upgrade_Ability.IsEnabled= true;
            }
        }

        public void show_character_infomation(int price)
        {
            character current = state.characters[character_ID];

            Mid_Block.Text = $"{current.name.ToString()}";
            Mid_Block.Text += $"\nHealth: {current.health}/{current.max_health}\n";

            Mid_Block.Text += $"\n{state.money - price} money";
            Mid_Block.Text += $"\nHeals cost {heal_cost} for {heal_ammount} healing";
            Mid_Block.Text += $"\nRevives cost {revive_cost}";
            Mid_Block.Text += $"\nRestores cost {restore_uses_cost}";
            Mid_Block.Text += $"\nUpgrade cost {upgrade_cost}";

        }
        private void Revive_Button_Click(object sender, RoutedEventArgs e)
        {
            character current = state.characters[character_ID];
            show_character_infomation(revive_cost);
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
            show_character_infomation(0);
        }
        public void set_ability_selector_vals()
        {
            character current = state.characters[character_ID];

            ComboBox abilityBox = Ability_Selector;
            abilityBox.Items.Clear();
            for (int i = 0; i < current.abilities.Length; i++)
            {
                ability current_ability = current.abilities[i];
                //MessageBox.Show(current_ability.ToString());
                string val = $"{current_ability.name}: {current_ability.uses_remaining}/{current_ability.max_number_of_uses}";
                abilityBox.Items.Add(val);
            }
            abilityBox.SelectedIndex = 0;
        }

        public ability getSelectedAbility()
        {
            character current = state.characters[character_ID];
            ComboBox abilityBox = Ability_Selector;
            ability selectedAbility = character.no_ability_selected;

            string ability_name = abilityBox.SelectedItem.ToString().Split(":")[0];

            foreach (ability _a in current.abilities)
            {
                // if this is the ability that was selected by the player
                if (_a.name == ability_name) { selectedAbility = _a; break; }
            }
            return selectedAbility;
        }

        private void Restore_Uses_Click(object sender, RoutedEventArgs e)
        {
            character current = state.characters[character_ID];
            show_character_infomation(restore_uses_cost);
            ability selectedAbility = getSelectedAbility();

            if (selectedAbility == character.no_ability_selected)
            {
                // if no ability was selected 
                // return and cost nothing
                return;
            }

            MessageBox.Show($"{selectedAbility.name}: {selectedAbility.uses_remaining}/{selectedAbility.max_number_of_uses}");

            // set the number of uses remaing on this ability to the max number of uses it can have
            selectedAbility.uses_remaining = selectedAbility.max_number_of_uses;

            state.money -= restore_uses_cost;

            if (state.money <= restore_uses_cost)
            {
                // disable the healing button as it has run out of uses
                Restore_Uses.IsEnabled = false;
            }
            set_character_details();
            set_ability_selector_vals();
        }

        private void out_of_combat_click(object sender, RoutedEventArgs e)
        {
            out_of_combat out_of_combat_screen = new out_of_combat(state);
            out_of_combat_screen.Show();
            this.Close();
        }

        private void Upgrade_Ability_Button(object sender, RoutedEventArgs e)
        {
            character current = state.characters[character_ID];
            show_character_infomation(upgrade_cost);
            ability selectedAbility = getSelectedAbility();

            if (selectedAbility == character.no_ability_selected)
            {
                // if no ability was selected 
                // return and cost nothing
                return;
            }

            int old_ammount = selectedAbility.ammount;

            selectedAbility.upgrade();

            MessageBox.Show($"{selectedAbility.name}: {old_ammount} -> {selectedAbility.ammount}");

            // set the number of uses remaing on this ability to the max number of uses it can have

            state.money -= upgrade_cost;

            if (state.money <= upgrade_cost)
            {
                // disable the healing button as it has run out of uses
                Upgrade_Ability.IsEnabled = false;
            }
            set_character_details();
            set_ability_selector_vals();
        }
    }
}

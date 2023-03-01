using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace GameWIndowTest1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int round_count = 0;
        int characterID = -1;
        bool round_complete;
        bool death_in_round = false;
        List<Rectangle> identifiers; // the identifier rectangles above the characters to show whos go it is
        List<character> characters = new List<character>{
            new character(40, "Character1", true),
            new character(30, "Character2", true),
            new character(20, "Character3", false) ,
            new character(10, "Character4", false) };
        List<(character, Rectangle, Rectangle, RadioButton, int)> dead = new List<(character, Rectangle, Rectangle, RadioButton, int)>();
        List<RadioButton> radioButtons;

        List<character> Remaining_Friendly = new List<character>();
        List<character> Remaining_Enemy = new List<character>();
        public MainWindow()
        {
            // LOOK AT VisualTreeHelper class
            // for get child/get parent

            InitializeComponent();

            identifiers = new List<Rectangle> { Character1_Identifier, Character2_Identifier, Character3_Identifier, Character4_Identifier };
            radioButtons = new List<RadioButton> { RB_Character1, RB_Character2, RB_Character3, RB_Character4 };

            // go through each character
            foreach (character _char in characters)
            {
                // if the characer is friendly
                if (_char.Friendly)
                {
                    // add them to the apropriate list
                    Remaining_Friendly.Add(_char);
                    continue;
                }
                // add them to the apropriate list
                Remaining_Enemy.Add(_char);
                continue;
            }
            show_character_details(characters[0]);
            round(); // start a round to init the block
        }

        public void goto_winner_screen()
        {   
            // open the winners screen
            // and pass through the current winning character
            // Remaing_Enemy.Count == 0 will pass through true if you won and 
            // false if you lost
            Winner_Screen winner_screen = new Winner_Screen(Remaining_Enemy.Count() == 0);
            // show the winners screen
            winner_screen.Show();
            // and close this screen
            this.Close();
        }

        public void round()
        {
            round_count++;

            // if only one character is alive
            if (Remaining_Friendly.Count() == 0 || Remaining_Enemy.Count() == 0)
            {
                goto_winner_screen();
                return;
            }

            // if the radio button is on a dead character, move it
            // and only check is a character died otherwise it wont be
            if (death_in_round)
            {
                set_next_nondead_radiobutton();
                characterID = characterID % (characters.Count());
            }
            // if no character dies that round move to the next index position
            // characters are removed when a character dies so the index position moved anyway
            if (!death_in_round)
            {
                // update the characterID and loop with the number of characters
                characterID = (characterID + 1) % characters.Count();
            }
            set_identifiers_colour();
            set_abilities_icons();
            set_buttons_avalablity();

            round_complete = false;
            death_in_round = false;
            // if the current character is an enemy do their round now
            if (!characters[characterID].Friendly)
            {
                // do the enemys round
                do_enemy_round();

            }
        }

        public void set_buttons_avalablity()
        {
            List<Button> abilities = new List<Button> { Ability_1_button, Ability_2_button, Ability_3_button, Ability_4_button };

            character current_character = characters[characterID];

            // disables all buttons on an enemies turn
            if (Remaining_Enemy.Contains(current_character))
            {
                foreach(Button button in abilities)
                {
                    button.IsEnabled = false;
                }
                return;
            }
            // the character is a friendly character
            for (int i = 0; i < current_character.abilities.Count(); i++)
            {
                ability current_ability = current_character.abilities[i];
                // if no uses remaining then disable this ability
                if (current_ability.uses_remaining <= 0 )
                {
                    // disable the ability if it has no uses left
                    abilities[i].IsEnabled = false;
                    continue;
                }
                // if it has uses left enable it
                abilities[i].IsEnabled = true;
                continue;
            }
        }

        public void do_enemy_round(int delay = 1000)
        {
            Thread.Sleep(delay);

            character current_character = characters[characterID];
            // get the picked ability by getting the current characters best ability and using that index in their abilities
            ability picked_ability = current_character.abilities[current_character.pick_ability_id()];

            if (Remaining_Friendly.Count() <= 0)
            {
                goto_winner_screen();
            }
            else
            {
                var a = Remaining_Friendly;
                character target = Remaining_Friendly[0];
                Heading_Info_Box.Text = current_character.name;
                InfoBox.Text = $"{target.name} has {target.health}";
                target.takedamage(picked_ability);
                InfoBox.Text += $"\n{target.name} now has {target.health}";


                // if the target died from that
                if (target.health <= 0)
                {
                    int target_index = characters.IndexOf(target);
                    deal_with_dead(target_index);
                }
                // wait 1s then move to the next so that you can read what happened
                Thread.Sleep(delay);

                round();
                return;
            }
        }

        public void deal_with_dead(int index)
        {
            character target = characters[index];

            // indicate that a character has dies this round
            death_in_round = true;

            Rectangle target_rectangle = this.FindName(target.name) as Rectangle;
            Rectangle identifier_rectangle = identifiers[index];
            RadioButton target_rb = radioButtons[index];

            // make the dead rectangles grey
            target_rectangle.Fill = Brushes.DarkGray;
            identifier_rectangle.Fill = Brushes.DarkGray;

            // disable the target's radio button so that it does not keep getting targeted
            target_rb.IsEnabled = false;


            if (target.Friendly)
            {
                // if the target that died was a friendly
                // removee them from the list of alive friendlies
                Remaining_Friendly.Remove(target);
            }
            else
            {
                // if the target that died was an enemy,
                // remove them from the list of alive enemies
                Remaining_Enemy.Remove(target);
            }

            // add the properties of the removed items to an array of dead atributes for use later if needed
            dead.Append((target, target_rectangle, identifier_rectangle, target_rb, index));



            // remove the character and identifiers so they dont get used again
            characters.Remove(target); // remove the target from the list of characters
            identifiers.RemoveAt(index); // remove the identifier from the list of identifiers
            radioButtons.Remove(target_rb);
        }


        public void set_next_nondead_radiobutton()
        {
            // set the first still alive radiobutton to be checked
            radioButtons[0].IsChecked = true;
            /*
            // if the current radio button is for a dead character
            if (radioButtons[characterID].IsEnabled == false)
            {
                // uncheck this radio button
                radioButtons[characterID].IsChecked = false;
                // go through each radiobutton
                foreach (RadioButton rb in radioButtons)
                {
                    // and check if the owner of that radiobutton is still alive
                    if(rb.IsEnabled == true)
                    {
                        //and if so set it to that one
                        // as it is the first radio button for a still alive character
                        rb.IsChecked = true;
                        // return out of the loop as an alive radio button has been found
                        return;
                    }
                }
            }
            return;
            */
        }

        public void set_identifiers_colour()
        {
            if (round_complete) // if the round has already been completed for this character
            {
                round();
                return;
            }
            // set the colours of all identifiers to Black, (off colour)
            foreach (var identifier in identifiers)
            {
                identifier.Fill = Brushes.Black;
            }
            // get the identifer of the current character to Red, (on colour)
            identifiers[characterID].Fill = Brushes.Red;
        }

        public void set_abilities_icons()
        {
            List<Button> abilities = new List<Button> { Ability_1_button, Ability_2_button, Ability_3_button, Ability_4_button };
            List<Image> images = new List<Image> { Ability_1_button_image, Ability_2_button_image, Ability_3_button_image, Ability_4_button_image };
            if (round_complete)
            {
                round();
                return;
            }

            foreach (Button ability in abilities)
            {
                // get the index of the ability 

                int ability_index = Int32.Parse(ability.Tag.ToString().Remove(0, 7)) - 1;
                string ability_name = characters[characterID].abilities[ability_index].name; //get the name of that ability

                Image img = images[ability_index];
                string uri = @"\Resources\Ability_Icons\Ability_" + $"{ability_name.ToUpper()}.png";
                img.Source = new BitmapImage(new Uri(uri, UriKind.Relative));
            }
        }
        private void Show_Character_Click(object sender, RoutedEventArgs e)
        {
            bool DEBUG = false;

            // get the infobox to desplay the infomation about the character in
            TextBlock infoBox = this.FindName("InfoBox") as TextBlock;
            TextBlock HeadingInfoBox = this.FindName("Heading_Info_Box") as TextBlock;

            MenuItem menu = (MenuItem)sender;
            string header = menu.Header.ToString();
            string tag_value = menu.Tag.ToString();

            // get the owner rectangle of the menu from the tag of the sender item
            // as the name of the owner rectangle
            // so find the name of the tag of the sender than that is the owner rectangle

            // owner rectangle is the rectangle that you right click to get the menu
            // i dont know how you would do this otherwise
            Rectangle owner_rect = this.FindName(tag_value) as Rectangle;


            // get the character class from the array of characters with the name
            // of the rectangle
            character current_character = new character(-1, "Character not found", true);

            for (int countID = 0; countID < characters.Count(); countID++)
            {
                character this_char = characters[countID];
                // if the name of this character matches the name of the rect
                if (this_char.name == tag_value)
                {
                    if (characterID == countID) // if the correct user user id
                    {
                        // asign that character to current_character
                        current_character = this_char;
                        break; // as no need to search the other characters
                    }
                    HeadingInfoBox.Text = this_char.name;
                    InfoBox.Text = $"{round_count}\nIncorrect user move picked";
                    return;
                }
            }


            // output the values assigned at the top to help debug depending on a bool toggle
            if (DEBUG) { MessageBox.Show($"Header Value: {header}"); }
            if (DEBUG) { MessageBox.Show($"Rectangle Name: {tag_value}"); }

            switch (header)
            {
                case "Change Colour":
                    change_character_colour(owner_rect);

                    break;

                case "List Abilities":
                    list_abilities(current_character);
                    break;

                case "Show Character Details":
                    show_character_details(current_character);
                    break;

                default:
                    // if no option is clicked return
                    infoBox.Text = header;
                    HeadingInfoBox.Text = current_character.name;

                    break;
            }
            round();
            return;
        }

        protected void change_character_colour(Rectangle _rect)
        {
            TextBlock infoBox = this.FindName("InfoBox") as TextBlock;
            TextBlock HeadingInfoBox = this.FindName("Heading_Info_Box") as TextBlock;
            HeadingInfoBox.Text = ""; // reset the box so that it does not retain the last value
            infoBox.Text = $"Round {round_count}";
            // change the colour of the rectangle, this is just to make sure that
            // it is working correctly
            Random rnd = new Random();
            Brush[] colours = new Brush[] { Brushes.Red, Brushes.Blue, Brushes.Green, Brushes.Gold };
            Brush new_colour;
            do
            {
                // get a new colour to set the rectangle
                new_colour = colours[rnd.Next(0, colours.Length)];
            } while (new_colour == _rect.Fill); // if the colour is the same as the current colour
                                                // get a new colour


            // set the rectangles colour to the new colour
            _rect.Fill = new_colour;
        }
        protected void list_abilities(character _char)
        {
            TextBlock infoBox = this.FindName("InfoBox") as TextBlock;
            TextBlock HeadingInfoBox = this.FindName("Heading_Info_Box") as TextBlock;
            HeadingInfoBox.Text = $"Abilities for {_char.name}";
            HeadingInfoBox.FontSize = 24;
            infoBox.Text = "In (Name, Damage, Uses Remaining) format\n-----\n";
            infoBox.Text += $"Round {round_count}";
            for (int i = 0; i < _char.abilities.Length; i++)
            {
                infoBox.Text += $"\n{i + 1}: ({_char.abilities[i].name}, {_char.abilities[i].damage}, {_char.abilities[i].uses_remaining})";
            }
            // return not break so that it doesnt use up the turn
            return;
        }

        protected void show_character_details(character _char)
        {
            TextBlock infoBox = this.FindName("InfoBox") as TextBlock;
            TextBlock HeadingInfoBox = this.FindName("Heading_Info_Box") as TextBlock;
            HeadingInfoBox.Text = $"{_char.name}";
            HeadingInfoBox.FontSize = 26;
            infoBox.Text = $"Health: {_char.health}\n";
            infoBox.Text += $"Round {round_count}";
            return;
        }
        public int find_character_index_by_name(string name)
        {
            /* takes a characters name as a parameter
             * and returns the index position of that character
             * in the characters array 
             * or returns -1 if the character does not exist in the array
             */
            for (int index = 0; index < characters.Count(); index++)
            {
                if (characters[index].name == name) return index;
            }
            return -1;
        }

        private void Ability_Click(object sender, RoutedEventArgs e)
        {
            Button send = (Button)sender;
            string header = send.Tag.ToString();

            character current_character = characters[characterID];
            // get the ability index from the name of the button that was click (the "1" from "ability 1")
            int ability_index = Int32.Parse(header.Remove(0, 7)) - 1;

            if (current_character.abilities[ability_index].uses_remaining <= 0)
            {
                // if there are no uses remaining on that ability
                InfoBox.Text = "No uses remaing on this ability";
                return;

            }
            //MessageBox.Show($"Current Character {current_character}\nAbility Name {current_character.abilities[ability_index].name}");

            int index = 0;

            if ((index = get_index_from_radiobutton()) != -1)
            {
                // get the target character
                character target = characters[index];

                // output what health that target character used to have
                InfoBox.Text = $"{target.name} has {target.health.ToString()} health";

                // get  the ability deals from the current characters ability array
                ability _ability = current_character.abilities[ability_index];

                // reduce the health of the current character by that ammount
                target.takedamage(_ability);

                // output the new health of the target character
                InfoBox.Text += $"\n{target.name} now has {target.health.ToString()} health";

                // if the target dies

                if (target.health <= 0)
                {
                    deal_with_dead(index);
                }
            }
            round();
            return;
        }

        protected int get_index_from_radiobutton()
        {
            // this is to stop "varaible may be null here" later on
            // this value will be changed in the next section

            // get the index position of the character who is selected by the radiobutton
            // and if that character exists (which it should allways)
            if (radioButtons is null || radioButtons.Count == 0) return -1;
            RadioButton a = radioButtons[0];

            //  go through each radiobutton
            foreach (RadioButton rb in radioButtons)
            {
                // and if that button was clicked
                if (rb.IsChecked == true)
                {
                    // then that is the target that is wanted
                    a = rb;
                }
            }

            return find_character_index_by_name(a.Name.Remove(0, 3));

        }
        private void Show_Character_Details_RB(object sender, RoutedEventArgs e)
        {
            RadioButton _rb = (RadioButton)sender;

            int index = 0;

            if ((index = get_index_from_radiobutton()) != -1)
            {
                // get the target character
                character target = characters[index];
                show_character_details(target);
            }
        }
    }
}

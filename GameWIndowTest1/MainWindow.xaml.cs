using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
        List<character> characters = new List<character>{ new character(10, "Character1"), new character(20, "Character2"), new character(30, "Character3") , new character(40, "Character4") };
        List<(character, Rectangle, Rectangle, RadioButton, int)> dead = new List<(character, Rectangle, Rectangle, RadioButton, int)>();
        List<RadioButton> radioButtons;
        public MainWindow()
        {
            // LOOK AT VisualTreeHelper class
            // for get child/get parent

            InitializeComponent();

            identifiers = new List<Rectangle> { Character1_Identifier, Character2_Identifier, Character3_Identifier, Character4_Identifier };
            radioButtons = new List<RadioButton> { RB_Character1, RB_Character2, RB_Character3, RB_Character4 };
            round(); // start a round to init the block
        }

        public void round()
        {
            round_count++;

            // if only one character is alive
            if (characters.Count() == 1)
            {
                // open the winners screen
                // and pass through the current winning character
                Winner_Screen winner_screen = new Winner_Screen(characters[0]);
                // show the winners screen
                winner_screen.Show();
                // and close this screen
                this.Close();
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


            round_complete = false;
            death_in_round = false;
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
            character current_character = new character(-1, "Character not found");

            for (int countID = 0; countID < characters.Count(); countID ++)
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
                        new_colour= colours[rnd.Next(0, colours.Length)];
                    } while (new_colour == owner_rect.Fill); // if the colour is the same as the current colour
                    // get a new colour


                    // set the rectangles colour to the new colour
                    owner_rect.Fill = new_colour;

                    break;
                case "List Abilities":
                    HeadingInfoBox.Text = $"Abilities for {current_character.name}";
                    HeadingInfoBox.FontSize = 24;
                    infoBox.Text = "In (Name, Damage, Uses Remaining) format\n-----\n";
                    infoBox.Text += $"Round {round_count}";
                    for (int i = 0; i < current_character.abilities.Length; i++)
                    {
                        infoBox.Text += $"\n{i + 1}: ({current_character.abilities[i].name}, {current_character.abilities[i].damage}, {current_character.abilities[i].uses_remaining})";
                    }
                    break;

                case "Show Character Details":
                    HeadingInfoBox.Text = $"{current_character.name}";
                    HeadingInfoBox.FontSize = 26;
                    infoBox.Text = $"Health: {current_character.health}\n";
                    infoBox.Text += $"Round {round_count}";
                    break;

                default:
                    // if no option is clicked return
                    infoBox.Text = header;
                    HeadingInfoBox.Text = current_character.name;
                    
                    break;
            }

            // if the menu button click starts with "Ability" then they have clicked on something in the abilities tab
            if (header.StartsWith("Ability"))
            {

                // get the ability index from the name of the button that was click (the "1" from "ability 1")
                int ability_index = Int32.Parse(header.Remove(0, 7)) - 1;

                if (current_character.abilities[ability_index].uses_remaining <= 0)
                {
                    // if there are no uses remaining on that ability
                    InfoBox.Text = "No uses remaing on this ability";
                    return;

                }

                // this is to stop "varaible may be null here" later on
                // this value will be changed in the next section
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

                int index = 0;
                // get the index position of the character who is selected by the radiobutton
                // and if that character exists (which it should allways)
                if ((index = find_character_index_by_name(a.Name.Remove(0, 3))) != -1)
                {
                    // get the target character
                    character target = characters[index];

                    // output what health that target character used to have
                    InfoBox.Text = $"{target.name} has {target.health.ToString()} health";
                    
                    // get the damage that ability deals from the current characters ability array
                    int ability_damage = current_character.abilities[ability_index].damage;
                    
                    // reduce the health of the current character by that ammount
                    target.takedamage(ability_damage);

                    // reduce the number remaining by one as it has been used once
                    current_character.abilities[ability_index].uses_remaining--;

                    // output the new health of the target character
                    InfoBox.Text += $"\n{target.name} now has {target.health.ToString()} health";

                    // if the target dies

                    if (target.health <= 0)
                    {
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

                        // add the properties of the removed items to an array of dead atributes for use later if needed
                        dead.Append((target,target_rectangle, identifier_rectangle, target_rb, index));

                        // remove the character and identifiers so they dont get used again
                        characters.Remove(target); // remove the target from the list of characters
                        identifiers.RemoveAt(index); // remove the identifier from the list of identifiers
                        radioButtons.Remove(target_rb);
                    }
                }
            }
            round();
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
    }
}

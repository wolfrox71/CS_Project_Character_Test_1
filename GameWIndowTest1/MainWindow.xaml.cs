using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.PerformanceData;
using System.Linq;
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
        Rectangle[] identifiers; // the identifier rectangles above the characters to show whos go it is
        character[] characters = new character[] { new character(10, "Character1"), new character(20, "Character2"), new character(30, "Character3") };
        public MainWindow()
        {
            // LOOK AT VisualTreeHelper class
            // for get child/get parent

            InitializeComponent();

            identifiers = new Rectangle[] { Character1_Identifier, Character2_Identifier, Character3_Identifier };
            round(); // start a round to init the block
        }

        public void round()
        {
            round_count++;
            // update the characterID and loop with the number of characters
            characterID = (characterID + 1) % characters.Count();
            set_identifiers_colour();
            round_complete = false; ;
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

            for (int countID = 0; countID < characters.Length; countID ++)
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
                    InfoBox.Text = $"{round_count}\n{this_char.name}\nIncorrect user move picked";
                    return;
                }
            }


            // output the values assigned at the top to help debug depending on a bool toggle
            if (DEBUG) { MessageBox.Show($"Header Value: {header}"); }
            if (DEBUG) { MessageBox.Show($"Rectangle Name: {tag_value}"); }

            switch (header)
            {
                case "Change Colour":
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
                    infoBox.Text = "In (Name, Damage) format\n-----\n";
                    infoBox.Text += $"Round {round_count}";
                    for (int i = 0; i < current_character.abilities.Length; i++)
                    {
                        infoBox.Text += $"\n{i + 1}: ({current_character.abilities[i].name}, {current_character.abilities[i].damage})";
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
                    return;
            }
            round();
            return;
        }
    }
}

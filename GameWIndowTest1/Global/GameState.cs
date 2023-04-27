using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GameWIndowTest1.Global
{
    public class GameState
    {
        // this will be passed through all the game windows and store game 
        public int current_wave_number { get; set; }
        public int max_wave_number { get; set; }
        public int money { get; set; }

        public List<character> characters { get; set; }

        public string username;

        protected Account account { get; set; }

        public GameState(int _current_wave_number, int _max_wave_number, List<character> _characters, int _money, Account _account)
        {
            // pass through and assign all the values
            current_wave_number = _current_wave_number;
            max_wave_number = _max_wave_number;
            characters = _characters;
            money = _money;
            account = _account;
            username = _account.username;
        }

        // this is for json to init this function
        public GameState() { }

        public int getScore()
        {
            /* this function returns the score from the current
             game state
             score is given from the
            */

            double totalScore = 0;


            // get the number of each type of character
            int number_of_alive_characters = 0;
            int number_of_dead_characters = 0;
            foreach (character _character in characters)
            {
                if (_character.IsDead) { number_of_dead_characters++;} // if the character is dead, add 1 to the number of dead characters
                else { number_of_alive_characters++; } // if the character is alive, add 1 to the number of alive characters
            }


            // the conversion factor from item to score

            double money_weight = 1.0; 
            double wave_number_weight = 20.0;
            double alive_characters_weight = 50.0; // how much is each alive character worth 
            double dead_character_weight = -20.0; // how much is a dead character worth (this should be negative as dead characters are bad)

            totalScore += money * money_weight;
            totalScore += current_wave_number * wave_number_weight;
            totalScore += number_of_alive_characters * alive_characters_weight;
            totalScore += number_of_dead_characters * dead_character_weight; // this should be negative as the weight should be negative

            return (int)totalScore > 0 ? (int)totalScore : 0; // return the score as an int if positive, or 0 is negative as you cannot have a negative score
        }
    }
}

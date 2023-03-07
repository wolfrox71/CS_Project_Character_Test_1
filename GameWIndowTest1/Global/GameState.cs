using System;
using System.Collections.Generic;
using System.Linq;
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

        public string username { get { return account.username; } }
        protected Account account { get; set; }

        public GameState(int _current_wave_number, int _max_wave_number, List<character> _characters, int _money, Account _account)
        {
            // pass through and assign all the vassslues
            current_wave_number = _current_wave_number;
            max_wave_number = _max_wave_number;
            characters = _characters;
            money = _money;
            account= _account;
        }
    }
}

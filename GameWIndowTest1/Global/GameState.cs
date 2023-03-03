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
        public int current_wave_number;
        public int max_wave_number;
        public int money;

        public List<character> characters;

        public GameState(int _current_wave_number, int _max_wave_number, List<character> _characters, int _money)
        {
            // pass through and assign all the vassslues
            current_wave_number = _current_wave_number;
            max_wave_number = _max_wave_number;
            characters = _characters;
            money = _money;
        }
    }
}

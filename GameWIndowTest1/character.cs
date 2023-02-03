using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWIndowTest1
{
    class character
    {
        public int health;
        public string name;
        public ability[] abilities = new ability[4];

        private void init_abilities()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < 4; i++)
            {
                Random rnd = new Random();
                int val = rnd.Next(0, 26);
                abilities[i] = new ability(val * 10, alphabet[val].ToString());
            }
        }

        public character(int _health, string _name)
        {
            health = _health;
            name = _name;
            init_abilities();
        }

        public character(int _health, int _name)
        {
            health = _health;
            name = _name.ToString();
        }
    }
}

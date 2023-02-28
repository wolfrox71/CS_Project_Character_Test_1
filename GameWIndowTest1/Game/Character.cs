using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWIndowTest1.Game
{
    public class Character
    {
        public int health;
        public string name;


        public Character(int health, string name)
        {
            this.health = health;
            this.name = name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWIndowTest1
{
    struct ability
    {
        public int damage;
        public string name;
        public int uses_remaining;
        public ability(int _damage, string _name, int _uses_remaining)
        {
            damage = _damage;
            name = _name;
            uses_remaining = _uses_remaining;
        }
    }
}

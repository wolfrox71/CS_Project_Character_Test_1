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

        public ability(int _damage, string _name)
        {
            damage = _damage;
            name = _name;
        }
    }
}

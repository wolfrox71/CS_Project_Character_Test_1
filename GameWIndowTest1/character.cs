using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWIndowTest1
{
    public class character
    {
        public int health { get; protected set; }
        public string name;
        public ability[] abilities = new ability[4];
        public bool Friendly;

        private void init_abilities()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < 4; i++)
            {
                Random rnd = new Random();
                int val = rnd.Next(0, 26);
                // times an ability can be used is 26-the damage it does for now
                abilities[i] = new ability(val, alphabet[val].ToString(), 1);
            }
        }

        public void takedamage(ability recived_ability)
        {
            // this function is so that any resistances can go in here rather than having to be dealt with in other places

            // reduce the times  this ability can be used by one
            recived_ability.uses_remaining--;

            // reduce this characters health  by the damage of the ability
            health -= recived_ability.damage;
        }

        public character(int _health, string _name, bool friendly)
        {
            health = _health;
            name = _name;
            Friendly = friendly;
            init_abilities();
        }

        public character(int _health, int _name, bool friendly)
        {
            health = _health;
            name = _name.ToString();
            Friendly = friendly;
            init_abilities();
        }

        public int pick_ability_id()
        {
            // pick the highest damage ability with remaining moves

            int current_best_index = 0;
            for (int i = 1; i < abilities.Count(); i++)
            {
                ability ability_ = abilities[i];
                // if this ability can be used
                // and does more damage than the current best ability
                if (ability_.uses_remaining > 0 && ability_.damage > abilities[current_best_index].damage)
                {
                    current_best_index = i;
                }
            }
            // return the index of the current best ability
            return current_best_index;
        }
    }
}

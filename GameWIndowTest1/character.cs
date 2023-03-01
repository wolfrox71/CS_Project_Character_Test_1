using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameWIndowTest1.Abilities;

namespace GameWIndowTest1
{
    public class character
    {
        public int health { get; protected set; }
        public int max_health { get; protected set; }
        public string name;
        public ability[] abilities = new ability[4];

        public int critical_health = 5;
        public bool Friendly;

        private void alphabet_init_abilities()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < 4; i++)
            {
                Random rnd = new Random();
                int val = rnd.Next(0, 26);
                // times an ability can be used is 26-the damage it does for now
                abilities[i] = new ability(val+1, alphabet[val].ToString(), 27-val, Ability_type.Damage);
            }
        }

        public void init_abilities()
        {
            abilities[0] = new ability(10, "Light Damage", 20, Ability_type.Damage);
            abilities[1] = new ability(30, "Heavy Damage", 5, Ability_type.Damage);
            abilities[2] = new ability(10, "Light Healing", 10, Ability_type.Healing);
            abilities[3] = new ability(40, "Heavy Healing", 2, Ability_type.Healing);
        }

        public void takedamage(ability recived_ability)
        {
            // this function is so that any resistances can go in here
            // rather than having to be dealt with in other places


            // if this is a damage ability do damage
            if (recived_ability.ability_Type == Ability_type.Damage)
            {
                // reduce the times  this ability can be used by one
                recived_ability.uses_remaining--;

                // reduce this characters health  by the damage of the ability
                health -= recived_ability.ammount;
            }
        }

        public void heal(ability recived_ability)
        {
            // if this is a healing ability
            if (recived_ability.ability_Type == Ability_type.Healing)
            {
                // heal by the ammount the ability
                health += recived_ability.ammount;

                // if the health is more than the max health
                if (health > max_health)
                {
                    // set the health to be the max value it can be
                    health = max_health;
                }
            }
        }

        public character(int _max_health, string _name, bool friendly)
        {
            max_health = _max_health;
            health = _max_health;
            name = _name;
            Friendly = friendly;
            init_abilities();
        }

        public character(int _max_health, int _name, bool friendly)
        {
            max_health = _max_health;
            health = _max_health;
            name = _name.ToString();
            Friendly = friendly;
            init_abilities();
        }

        public ability pick_ability()
        {

            // this is the default ability
            // and should only be used actually if no other ability remains
            ability current_ability = new ability(1, "Default Ability", 1, Ability_type.Damage);

            // if the character is low enough health to need healing
            if (health <= critical_health)
            {
                // try find a healing move with uses left
                foreach (ability _a in abilities)
                {
                    // if this is a healing move and has uses left
                    if (_a.ability_Type == Ability_type.Healing && _a.can_be_used )
                    {
                        // if the current ability is the default one
                        if (current_ability.name == "Default Ability")
                        {
                            // set the current ability to be this one
                            current_ability = _a;
                            // then try find a better one
                            continue;
                        }
                        // if this ability does more healing
                        if (_a.ammount > current_ability.ammount)
                        {
                            // set this ability as the current one
                            current_ability = _a;
                            continue;
                        }
                    }
                }
                // if an ability has been picked that is not the default one
                if (current_ability.name != "Default Ability")
                {
                    // return the current ability as it is the best healing one avalable and this character needs healing
                    return current_ability;
                }
            }
            // at this stage the character doesnt need healing or doesnt have any healing moves
            // so should attack with the most damaging ability left

            foreach (ability _a in abilities)
            {
                // if the current ability is a damage ability
                if (_a.ability_Type == Ability_type.Damage)
                {
                    // if this ability is better than the current best ability
                    if (_a.ammount > current_ability.ammount)
                    {
                        // this is the current best ability
                        current_ability= _a;
                    }
                }
            }

            // return the current ability
            return current_ability;
        }
    }
}

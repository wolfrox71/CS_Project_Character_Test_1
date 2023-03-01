using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GameWIndowTest1.Abilities;

namespace GameWIndowTest1
{
    public class character
    {
        public int health { get; protected set; }
        public int max_health { get; protected set; }
        public string name;
        public ability[] abilities = new ability[4];

        public int dodge_percentage = 50;
        public int dodge_reduction_percentage = 50; // how much damage gets reduced by if the dodge is successful
        
        public int critical_health_percentage = 30; // this the % of max health that the ais will heal on so 30 is 30 % of max health
        public bool Friendly;

        private void alphabet_init_abilities()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < 4; i++)
            {
                Random rnd = new Random();
                int val = rnd.Next(0, 26);
                // times an ability can be used is 26-the damage it does for now
                abilities[i] = new ability(val+1, alphabet[val].ToString(), 27-val, Ability_type.Damage, 0, 0, 0);
            }
        }

        public void init_abilities()
        {
            abilities[0] = new ability(10, "Light Damage", 20, Ability_type.Damage, 10, 2, 5);
            abilities[1] = new ability(30, "Heavy Damage", 5, Ability_type.Damage, 1, 10, 5);
            abilities[2] = new ability(10, "Light Healing", 10, Ability_type.Healing, 10, 2, 0);
            abilities[3] = new ability(40, "Heavy Healing", 2, Ability_type.Healing, 1, 10, 0);
        }

        public success_status takedamage(ability recived_ability, bool critical)
        {
            // this function is so that any resistances can go in here
            // rather than having to be dealt with in other places

            bool dodge = false;

            // if this is a damage ability do damage
            if (recived_ability.ability_Type == Ability_type.Damage)
            {

                int damage_to_do = recived_ability.ammount;

                // if its a critical hit, increase the damage done by the critical hit bonus
                if (critical) 
                {
                    damage_to_do += recived_ability.critical_hit_bonus;
                }

                Random rnd = new Random();

                // if the dodge is successfull
                if (rnd.Next(0,101) <= dodge_percentage)
                {
                    // reduce the damage by the damage reduction ammount

                    // this is -= not *= as *= would make 40 reduce the damage by 60% not 40% 
                    damage_to_do -= (damage_to_do * (dodge_reduction_percentage/100));
                    dodge = true;
                    MessageBox.Show($"Dodged reducing damage by {damage_to_do} {(damage_to_do * dodge_reduction_percentage/100)}");
                }

                // reduce the times  this ability can be used by one
                recived_ability.uses_remaining--;

                // reduce this characters health  by the damage of the ability
                health -= damage_to_do;
            }
            // return dodge if they dodged so that the log can show the correct thing
            return (dodge) ? success_status.Dodge : success_status.Success;
        }

        public void heal(ability recived_ability, bool critical)
        {
            // if this is a healing ability
            if (recived_ability.ability_Type == Ability_type.Healing)
            {
                int ammount_to_heal = recived_ability.ammount;

                if (critical)
                {
                    ammount_to_heal += recived_ability.critical_hit_bonus;
                }

                // heal by the ammount the ability
                health += ammount_to_heal;

                recived_ability.uses_remaining--;

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
            ability current_ability = new ability(1, "Default Ability", 1, Ability_type.Damage, 0, 0, 0);

            // if the character is low enough health to need healing
            if (health <= ((critical_health_percentage * max_health)/100))
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

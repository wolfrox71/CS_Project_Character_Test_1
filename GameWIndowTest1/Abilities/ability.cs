using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GameWIndowTest1.Abilities
{
    public class ability
    {
        public int ammount { get; set; }
        public string name { get; set; }
        public Ability_Team team { get; set; }
        public bool defaultly_equipped { get; set; }
        public int max_number_of_uses { get; set; }
        public int uses_remaining { get;  set; }
        public Ability_type ability_Type { get; set; }

        public int cooldown { get; set; }
        public int turns_till_next_use { get; set; }

        /*          Critical Hit Values         */
        public int critical_hit_percentage { get; set; }
        public int critical_hit_bonus { get; set; }

        /*          Missing Values              */
        public int missing_percentage { get; set; }

        public double ammount_percent { get; set; } = 1.10;

        public bool can_be_used { get { return uses_remaining > 0 && turns_till_next_use <= 0 ; } } // if the ability has uses left return true else reuturn false as cannot be used
        public bool onCooldown { get { return turns_till_next_use > 0; } } // return true if the current ability is on cooldown or not
        public ability(int _ammount, string _name, int _max_number_of_uses, Ability_type _ability_Type, int _critical_hit_percentage, int _critical_hit_bonus, int _missing_percentage, Ability_Team _team, bool default_equip, int _cooldown)
        {
            ammount = _ammount;
            name = _name;
            max_number_of_uses = _max_number_of_uses;
            uses_remaining = _max_number_of_uses;
            ability_Type = _ability_Type;
            critical_hit_bonus = _critical_hit_bonus;
            critical_hit_percentage = _critical_hit_percentage;
            missing_percentage = _missing_percentage;
            team = _team;
            defaultly_equipped = default_equip; // if the ability is equiped during the init function
            cooldown = _cooldown;
            turns_till_next_use = 0; // so that the ability can be already
        }

        public void upgrade()
        {
            ammount = (int)(ammount * ammount_percent);
        }


        public void use()
        {
            uses_remaining--;
            turns_till_next_use = cooldown;
        }

        // this is for json
        public ability() { }
    }
}

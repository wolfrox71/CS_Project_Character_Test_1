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
        public int uses_remaining { get;  set; }
        public Ability_type ability_Type { get; set; }

        /*          Critical Hit Values         */
        public int critical_hit_percentage { get; set; }
        public int critical_hit_bonus { get; set; }

        /*          Missing Values              */
        public int missing_percentage { get; set; }

        public bool can_be_used { get { return uses_remaining > 0; } } // if the ability has uses left return true else reuturn false as cannot be used
        public ability(int _ammount, string _name, int _uses_remaining, Ability_type _ability_Type, int _critical_hit_percentage, int _critical_hit_bonus, int _missing_percentage)
        {
            ammount = _ammount;
            name = _name;
            uses_remaining = _uses_remaining;
            ability_Type = _ability_Type;
            critical_hit_bonus = _critical_hit_bonus;
            critical_hit_percentage = _critical_hit_percentage;
            missing_percentage = _missing_percentage;
        }

        // this is for json
        public ability() { }
    }
}

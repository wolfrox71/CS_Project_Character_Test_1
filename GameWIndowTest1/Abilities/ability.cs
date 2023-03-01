﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWIndowTest1.Abilities
{
    public struct ability
    {
        public int ammount;
        public string name;
        public int uses_remaining;
        public Ability_type ability_Type;
        public int critical_hit_percentage;
        public int critical_hit_bonus;
        public bool can_be_used { get { return uses_remaining > 0; } } // if the ability has uses left return true else reuturn false as cannot be used
        public ability(int _ammount, string _name, int _uses_remaining, Ability_type _ability_Type, int _critical_hit_percentage, int _critical_hit_bonus)
        {
            ammount = _ammount;
            name = _name;
            uses_remaining = _uses_remaining;
            ability_Type = _ability_Type;
            critical_hit_bonus = _critical_hit_bonus;
            critical_hit_percentage = _critical_hit_percentage;
        }
    }
}

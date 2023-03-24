using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GameWIndowTest1.Abilities;
using GameWIndowTest1.Global;

namespace GameWIndowTest1
{
    public class character
    {
        public int health { get;  set; }
        public int max_health { get;  set; }
        public string name { get;  set; }
        public string display_name { get;  set; } // this is the name that will be shown when moves happen

        public int revive_health_percentage = 50;

        public static ability no_ability_selected = new ability(0, "No ability Selected", 0, Ability_type.Damage, 0, 0, 0, Ability_Team.Both, true);

        public ability[] abilities { get;  set; }  = new ability[4];

        public bool IsDead { get { return health <= 0; } }

        public bool dodging_enabled { get;  set; } = true;

        public int dodge_percentage { get;  set; } = 10;
        public int dodge_reduction_percentage { get;  set; } = 50; // how much damage gets reduced by if the dodge is successful

        public int critical_health_percentage { get;  set; } = 30; // this the % of max health that the ais will heal on so 30 is 30 % of max health
        public bool Friendly { get; set; }

        private void alphabet_init_abilities()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < 4; i++)
            {
                Random rnd = new Random();
                int val = rnd.Next(0, 26);
                // times an ability can be used is 26-the damage it does for now
                abilities[i] = new ability(val+1, alphabet[val].ToString(), 27-val, Ability_type.Damage, 0, 0, 0, Ability_Team.Both, true);
            }
        }

        public static void write_ability(ability ability)
        {
            if (!Directory.Exists("Abilities"))
            {
                MessageBox.Show("Added Abilities Directory");
                Directory.CreateDirectory("Abilities");
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(ability, options);

            string fileName = $"Abilities/{ability.name}.json";


            File.WriteAllText(fileName, jsonString);
            MessageBox.Show($"Saved Ability {ability.name}");
        }

        public List<ability> get_valid_abilities()
        {
            List<ability> _All_Ability_list = new List<ability>();
            string[] filenames = Directory.GetFiles("Data/Abilities", "*.json", SearchOption.AllDirectories);

            foreach (string filename in filenames)
            {
                string json_text = File.ReadAllText(filename);
                ability _ability = JsonSerializer.Deserialize<ability>(json_text);

                // if this ability is for the wrong team
                if (!(_ability.team == Ability_Team.Both || (Friendly && _ability.team == Ability_Team.Friendly || !Friendly && _ability.team == Ability_Team.Enemy)))
                {
                    //MessageBox.Show($"{display_name} Skipping {_ability.name} as {_ability.team} when {Friendly}");
                    // skip it and go to the next team
                    continue;
                }

                // always add the new ability to the ability list
                _All_Ability_list.Add(_ability);
            }
            return _All_Ability_list;
        }

        public void get_abilities_from_files()
        {
            List<ability> _All_Ability_list = new List<ability>();
            List<ability> _healing_abilities = new List<ability>();
            List<ability> _damage_abilities = new List<ability>();
            List<ability> _default = new List<ability>();

            if (!Directory.Exists("Data/Abilities"))
            {
                MessageBox.Show("Added Abilities Directory");
                Directory.CreateDirectory("Data/Abilities");
            }

            _All_Ability_list = get_valid_abilities();

            foreach (ability _ability in _All_Ability_list)
            {
                // if this ability is equiped by default
                
                switch (_ability.ability_Type)
                {
                    case Ability_type.Damage:
                        if (_ability.defaultly_equipped)
                        {
                            // add it to the list of
                            _default.Add(_ability);

                            // if the ability is a damage ability
                            // add this ability to the damage ability list
                            _damage_abilities.Add(_ability);
                        }
                        break;

                    case Ability_type.Healing:
                        if (_ability.defaultly_equipped)
                        {
                            // if the ability is a damage ability
                            // add this ability to the healing ability list
                            _healing_abilities.Add(_ability);
                        }
                        break;
                }
            }

            //MessageBox.Show($"Abilities {_All_Ability_list.Count()} Abiltiies found\n{_healing_abilities.Count} Healing\n{_damage_abilities.Count} Damage");

            _healing_abilities = _healing_abilities.OrderBy(o => o.ammount).ToList();
            _damage_abilities  = _damage_abilities.OrderBy(o => o.ammount).ToList();

            // if the number of ability from files is less than or equal to the number abilities on this character
            if (_All_Ability_list.Count <= abilities.Length)
            {
                // put in all the abilities that exists
                for (int index = 0; index < abilities.Length; index++)
                {
                    // this is to order damage first then healing

                    // if the current ability is a damage ability
                    if (index < _damage_abilities.Count)
                    {
                        abilities[index] = _damage_abilities[index];
                    }
                    // if the current ability is a healing ability
                    else if (index < _healing_abilities.Count + _damage_abilities.Count)
                    {
                        //MessageBox.Show($"{index}/ {_healing_abilities.Count + _damage_abilities.Count} {index - _damage_abilities.Count} {_healing_abilities[index - _damage_abilities.Count].name}");
                        abilities[index] = _healing_abilities[index - _damage_abilities.Count];
                    }
                    else
                    {
                        // any remaining abilities get filled with "No ability selected" abilities
                        abilities[index] = no_ability_selected;
                    }
                }
            }
            else
            {
                // add the damage ability with the least moves but most damage
                abilities[1] = _damage_abilities[0];
                // add the damage ability with the most moves but least damage
                abilities[0] = _damage_abilities[_damage_abilities.Count-1];

                // and repeats for healing
                abilities[3] = _healing_abilities[0];
                abilities[2] = _healing_abilities[_healing_abilities.Count-1];
            }
        }

        public void init_abilities_friendly()
        {
            get_abilities_from_files();
            /*
            abilities[0] = new ability(10, "Light Damage", 20, Ability_type.Damage, 10, 2, 5, Ability_Team.Both);
            abilities[1] = new ability(30, "Heavy Damage", 5, Ability_type.Damage, 1, 10, 5, Ability_Team.Friendly);
            abilities[2] = new ability(10, "Light Healing", 10, Ability_type.Healing, 10, 2, 0, Ability_Team.Both);
            abilities[3] = new ability(40, "Heavy Healing", 2, Ability_type.Healing, 1, 10, 0, Ability_Team.Friendly);
            */

            
           /*
            abilities[0] = new ability(10, "Light Damage", 20, Ability_type.Damage, 10, 2, 5, Ability_Team.Both);
            abilities[1] = new ability(25, "Medium Damage", 5, Ability_type.Damage, 5, 10, 5, Ability_Team.Enemy);
            abilities[2] = new ability(8, "Light Healing", 10, Ability_type.Healing, 10, 2, 0, Ability_Team.Both);
            abilities[3] = new ability(25, "Medium Healing", 2, Ability_type.Healing, 1, 10, 0, Ability_Team.Enemy);
            */
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

                int dodge_damage_reduction = 0;

                // if its a critical hit, increase the damage done by the critical hit bonus
                if (critical) 
                {
                    damage_to_do += recived_ability.critical_hit_bonus;
                }

                Random rnd = new Random();

                // if the dodge is successfull
                if (rnd.Next(0,101) <= dodge_percentage && dodging_enabled)
                {
                    // reduce the damage by the damage reduction ammount

                    // this is -= not *= as *= would make 40 reduce the damage by 60% not 40% 
                    dodge_damage_reduction = ((damage_to_do * dodge_reduction_percentage)/100);
                    dodge = true;
                }

                damage_to_do -= dodge_damage_reduction;

                // reduce the times  this ability can be used by one
                recived_ability.uses_remaining--;

                // reduce this characters health  by the damage of the ability
                health -= damage_to_do;
            }
            // return dodge if they dodged so that the log can show the correct thing
            return (dodge) ? success_status.Dodge : success_status.Success;
        }

        public void takedamage(int ammount)
        {
            // take the ammount of damage specified
            health -= ammount;
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


        public void heal(int ammount)
        {
            int ammount_to_heal = ammount;

            // heal by the ammount the ability
            health += ammount_to_heal;

            // if the health is more than the max health
            if (health > max_health)
            {
                // set the health to be the max value it can be
                health = max_health;
            }
        }


        public void revive()
        {
            if (IsDead)
            {
                // set health to a percentage of max health
                health = (revive_health_percentage * max_health) / 100;
            }
        }

        public character(int _max_health, string _name, string _display_name, bool friendly)
        {
            max_health = _max_health;
            health = _max_health;
            name = _name;
            display_name = _display_name;
            Friendly = friendly;
            init_abilities_friendly();
        }

        public character(int _max_health, int _name, string _display_name, bool friendly)
        {
            max_health = _max_health;
            health = _max_health;
            name = _name.ToString();
            Friendly = friendly;
            display_name = _display_name;            
            init_abilities_friendly();
        }
       
        // this is for json
        public character() { }

        public ability pick_ability()
        {

            // this is the default ability
            // and should only be used actually if no other ability remains
            ability current_ability = no_ability_selected;

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
                        if (current_ability.name == no_ability_selected.name)
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

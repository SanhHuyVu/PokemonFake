using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AAbilitysDB
{
    static Dictionary<string, AbilityBase> abilities;

    public static void Init()
    {
        abilities = new Dictionary<string, AbilityBase>();

        // load the unit object from a path
        // if use "" means will load all the object from all the directories in the project
        var abilityList = Resources.LoadAll<AbilityBase>("");

        foreach (var ability in abilityList)
        {
            // use "Name" instead of "name" or the game will take the name of the 
            // scriptable objects instead the name of unit 
            if (abilities.ContainsKey(ability.Name))
            {
                Debug.Log($"there are duplicated {ability.Name}s");
                continue;
            }

            abilities[ability.name] = ability;
        }
    }

    public static AbilityBase GetAAbilityByName(string name)
    {
        if (!abilities.ContainsKey(name))
        {
            Debug.Log($"unit {name} is not in the database");
            return null;
        }

        return abilities[name];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDB 
{
    static Dictionary<string, UnitBase> units;

    public static void Init()
    {
        units = new Dictionary<string, UnitBase>();

        // load the unit object from a path
        // if use "" means will load all the object from all the directories in the project
        var unitArray = Resources.LoadAll<UnitBase>("");

        foreach (var unit in unitArray)
        {
            // use "Name" instead of "name" or the game will take the name of the 
            // scriptable objects instead the name of unit 
            if (units.ContainsKey(unit.Name))
            {
                Debug.Log($"there are duplicated units");
                continue;
            }

            units[unit.name] = unit;
        }
    }

    public static UnitBase GetUnitByName(string name)
    {
        if (!units.ContainsKey(name))
        {
            Debug.Log($"unit {name} is not in the database");
            return null;
        }

        return units[name];
    }
}
